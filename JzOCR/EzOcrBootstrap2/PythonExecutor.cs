using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;


namespace EzUtils
{
    //using PS = EzFaceClientLib.EzProcessUtilEx;

    /// <summary>
    /// Execute Python interpreter
    /// </summary>
    public class PythonExecutor
    {
        public static string TITLE = "OcrBootstrap";
        const string DEFAULT_WORKING_DIR = "D:\\";
        const string STOP_PYTHON_BY_STDIN = "q";    // not so useful !!!

        #region PRIVATE_MEMBERS
        private string _workingDir = DEFAULT_WORKING_DIR;
        private Process _process = null;
        private Thread _thread = null;
        #endregion

        public string getCmdFile()
        {
            string launch_cmd = "start_ocr_server.cmd";
            string cmdFile = System.IO.Path.Combine(_workingDir, launch_cmd);
            return cmdFile;
        }
        public string CONDA_CMD(string arg = null)
        {
            string cmdFile = getCmdFile();
            string cmdStr;
            if (arg != null && arg.Length>0)
                cmdStr = string.Format("/K \"{0}\" {1}", cmdFile, arg);
            else
                cmdStr = string.Format("/K \"{0}\"", cmdFile);
            return cmdStr;
        }

        public delegate void PythonAppMessageEvent(string output);
        public event PythonAppMessageEvent OnPythonOutput;
        public event PythonAppMessageEvent OnPythonClosed;
        public event PythonAppMessageEvent OnPythonError;

        /// <summary>
        /// true: 會停用 OptUsingStdOut/In<br/>
        /// false: 會讓 OptUsingStdOut/In 決定是否用 StdOut/In 與 Owner 通訊
        /// <br/> DEBUG: OptFreeShellRun= true; OptShowWindow= true;
        /// <br/> RELEASE: OptFreeShellRun= true; OptShowWindow = false;
        /// </summary>
        public static bool OptFreeShellRun = true;
        public static bool OptShowWindow = false;
        public static bool OptUsingStdOut = true;
        public static bool OptUsingStdIn = !string.IsNullOrEmpty(STOP_PYTHON_BY_STDIN);

        public PythonExecutor(string workingDir = null)
        {
            if (!string.IsNullOrEmpty(workingDir))
            {
                //System.Diagnostics.Trace.Assert(System.IO.Directory.Exists(workingDir), "資料夾不存在: " + workingDir);
                _workingDir = workingDir;
            }
            if (!System.IO.Directory.Exists(_workingDir))
            {
                System.Windows.Forms.MessageBox.Show(
                    $"找不到 {TITLE} 的 python 相關 資料夾與檔案 !", TITLE,
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
                System.Diagnostics.Trace.Assert(false, "資料夾不存在: " + _workingDir);
                Application.Exit();
            }
        }

        public bool CanRun
        {
            get
            {
                return System.IO.File.Exists(getCmdFile());
            }
        }

        /// <summary>
        /// Run Python script in different thread
        /// </summary>
        public void Run(string pyFile, params string[] args)
        {
            if (pyFile != null)
            {
                // CHECK python file
                string filename = System.IO.Path.Combine(_workingDir, pyFile);
                if (!System.IO.File.Exists(filename))
                {
                    throw new FileNotFoundException(String.Format("{0} does not exist.", filename));
                }
            }

            // CHECK launch file
            if (!CanRun)
                throw new Exception("找不到相對應的 Python 啟動批次檔!");

            Terminate(0);

            // 如果 pyFile 內有空白字元, 前後加入 " 號
            string arg = null;
            if (pyFile != null)
                arg = string.Format("\"{0}\" {1}", pyFile, concateParameters(args)).Trim();
            else if (args != null && args.Length > 0)
                arg = concateParameters(args).Trim();
            else
                arg = null;

            _thread = new Thread(() => _runPythonEx(arg));
            _thread.Name = TITLE;
            _thread.Start();
        }

        /// <summary>
        /// Close Python interpreter running on the other thread
        /// </summary>
        public void Terminate(int delay = 500)
        {
            if (_process != null)
            {
                int pid = _process.Id;
                if (OptUsingStdIn)
                {
                    // Send "q" + enter to stop python app loop
                    stop_process_by_stdin(_process);
                    if (delay > 0)
                    {
                        Thread.Sleep(delay);
                        delay = 500;
                    }
                    try
                    {
                        _TRACE("Kill Process.");
                        _process.Kill();
                    }
                    catch
                    {

                    }
                }
                _process = null;

                //_TRACE("Kill Processes. ID");
                //EzProcessUtil.KillProcess(pid);
                //EzProcessUtil.KillProcesses("run_ocr_server");
            }

            if (_thread != null && _thread.ThreadState == System.Threading.ThreadState.Running)
            {
                if (delay > 0)
                {
                    Thread.Sleep(delay);
                    delay = 0;
                }
                _TRACE("Abort Thread.");
                _thread.Abort();
                _thread = null;
            }

            try
            {
                EzProcessUtil.KillProcesses("run_ocr_server");
            }
            catch
            {

            }
        }

        public void CloseStdOut()
        {
            //////if (_process != null)
            //////{
            //////    // 沒有用!
            //////    _process.StartInfo.RedirectStandardOutput = false;
            //////    //if (_process.StandardOutput != null)
            //////    //{
            //////    //    _process.StandardOutput.Close();
            //////    //}
            //////}
        }


        #region PRIVATE_FUNCTIONS

        /// <summary>
        /// Convert string[] parameters to a formatted parameter (e.g., "param1" "param2")
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static string concateParameters(params string[] args)
        {
            if (args.Length == 0)
                return "";

            StringBuilder sb = new StringBuilder();
            foreach (string s in args)
            {
                // 使用 cmd.exe "/K" 之後的 每一個參數 一定要加 雙引號 !!!
                sb.AppendFormat("\"{0}\" ", s);
            }
            return sb.ToString().Trim();
        }
        /// <summary>
        /// Execute Python interpreter Ex
        /// </summary>
        private string _runPythonEx(string arg)
        {
            try
            {
                string output = _runPython(arg);
                _thread = null;
                return output;
            }
            catch (Exception ex)
            {
                _thread = null;
                string errMsg = "exception : " + ex.Message;
                _TRACE(errMsg);
                return errMsg;
            }
        }
        /// <summary>
        /// Execute Python interpreter
        /// </summary>
        private string _runPython(string pythonFileAndArgs)
        {
            // (0.0) Options
            bool isFreeShellRun = OptFreeShellRun;
            bool usingWindow = OptShowWindow;
            bool usingStdErr = false;
            bool usingStdOut = OptUsingStdOut;      
            bool usingStdIn = OptUsingStdIn;        // 只要 stdIn 打開, stdOut 也會自動轉向

            // (0.1) Run-time Flags
            bool isAsync = (_thread != null);
            string output = isAsync ? "AsyncRun" : "BlockedRun";

            // (1) cmdArgs
            string cmdArgs = CONDA_CMD(pythonFileAndArgs);
            _TRACE(cmdArgs);
            _DUMP(_workingDir, "cmd.exe", cmdArgs);

            // (2) ProcessStartInfo
            ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe", cmdArgs);

            startInfo.WorkingDirectory = _workingDir;
            if (isFreeShellRun)
            {
                //startInfo.WindowStyle = ProcessWindowStyle.Normal;
                //startInfo.CreateNoWindow = false;
                startInfo.WindowStyle = usingWindow ? ProcessWindowStyle.Minimized : ProcessWindowStyle.Hidden;
                startInfo.CreateNoWindow = !usingWindow;
                startInfo.UseShellExecute = true;
                startInfo.RedirectStandardOutput = false;
                startInfo.RedirectStandardInput = false;
                startInfo.RedirectStandardError = false;
                var ps = Process.Start(startInfo);
                _process = ps;
                return "FreeRun";
            }
            else
            {
                startInfo.WindowStyle = usingWindow ? ProcessWindowStyle.Minimized : ProcessWindowStyle.Hidden;
                startInfo.CreateNoWindow = !usingWindow;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = usingStdOut;
                startInfo.RedirectStandardInput = usingStdIn;
                startInfo.RedirectStandardError = usingStdErr;
            }

            // (3) Process
            Process process = new Process() { StartInfo = startInfo };
            if (usingStdOut && isAsync)
            {
                process.OutputDataReceived += onProcess_OutputDataReceived;
                process.Start();
                process.BeginOutputReadLine();
            }
            else
            {
                process.Start();
            }
            _process = process;
                        
            // (4) Read output lines from stdOut (block operation)
            if (usingStdOut && !isAsync)
            {
                var accumOutputLines = new StringBuilder();
                using (StreamReader stdout = process.StandardOutput)
                {
                    do
                    {
                        output = stdout.ReadLine();
                        accumOutputLines.Append(output);
                        _TRACE_PYTHON(output);
                    }
                    while (stdout.Peek() != -1);
                    stdout.Close();
                    _TRACE("End of stream (stdout).");
                }
                output = accumOutputLines.ToString();
                //////if (output.Length > 2)
                //////    output = output.Substring(0, output.Length - 2);
            }

            // (5) WaitForExit
            process.WaitForExit();
            process.Close();
            _process = null;

            // (6) stdErr
            if (usingStdErr)
            {
                using (var stderr = process.StandardError)
                {
                    if (stderr.Peek() != -1)
                    {
                        issuePythonErrorMessage(stderr.ReadToEnd());
                    }
                    stderr.Close();
                }
            }

            // (7) Event
            OnPythonClosed?.Invoke(output);

            _TRACE("End of Process.");
            return output;
        }
        private void stop_process_by_stdin(Process ps)
        {
            if (!string.IsNullOrEmpty(STOP_PYTHON_BY_STDIN))
            {
                try
                {
                    ps.StandardInput.WriteLine(STOP_PYTHON_BY_STDIN);
                }
                catch
                {

                }
            }
        }
        private void issuePythonErrorMessage(string errorMessage)
        {
            OnPythonError?.Invoke(errorMessage);
            _TRACE(errorMessage);
            if (false)
            {
                throw new ApplicationException(errorMessage);
            }
        }
        private void onProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                if(OnPythonOutput!=null)
                    OnPythonOutput(e.Data);
                else
                    _TRACE_PYTHON(e.Data);
            }
        }

        #endregion


        private void _TRACE(string msg)
        {
            System.Diagnostics.Trace.WriteLine("PyExec: " + msg);
        }
        private void _TRACE_PYTHON(string msg)
        {
            System.Diagnostics.Trace.WriteLine("[Python] >>> " + msg);
        }
        private void _DUMP(string path, string cmd, string cmdArgs)
        {
#if (DEBUG)
            if(true)
            {
                using (var stm = new StreamWriter("D:\\cmdArgs.txt", false, Encoding.Default))
                {
                    stm.WriteLine("cd " + path);
                    stm.WriteLine(cmd + " " + cmdArgs);
                    stm.Flush();
                    stm.Close();
                }
            }
#endif
        }
    }
}