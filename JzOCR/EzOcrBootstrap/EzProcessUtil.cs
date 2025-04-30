using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;


namespace EzUtils
{
    public class EzProcessUtil
    {
        internal const string FILE_DEFAULT_DUMP = "ps_dump.txt";

        public static void ScanProcesses(IList dstList)
        {
            Process[] pssArr = Process.GetProcesses();
            Array.Sort(pssArr, new Comparison<Process>((p1, p2) =>
            {
                return string.Compare(p1.ProcessName, p2.ProcessName, StringComparison.OrdinalIgnoreCase);
            }));

            dstList.Clear();
            int idx = 0;
            foreach (var ps in pssArr)
            {
                dstList.Add(Pack(ps, idx++));
            }
        }
        public static int KillProcesses(string processName)
        {
            int count = 0;
            var pssArr = Process.GetProcessesByName(processName);
            foreach (var ps in pssArr)
            {
                ps.Kill();
                count++;
            }
            return count;
        }
        public static bool KillProcess(int pid)
        {
            var ps = Process.GetProcessById(pid);
            if (ps != null)
            {
                ps.Kill();
                return true;
            }
            return false;
        }
        public static void DumpProcesses(IList srcList = null, string fileName = null)
        {
            if (fileName == null)
                fileName = FILE_DEFAULT_DUMP;

            if (srcList == null)
            {
                srcList = new List<string>();
                ScanProcesses(srcList);
            }

            using (var stm = new StreamWriter(fileName, false, System.Text.Encoding.UTF8))
            {
                foreach (var line in srcList)
                {
                    System.Diagnostics.Trace.WriteLine(line);
                    stm.WriteLine(line);
                }
                stm.Flush();
                stm.Close();
            }
        }
        public static bool Parse(string line, out int pid, out string name, out string wtitle)
        {
            pid = -1;
            name = "";
            wtitle = "";
            if (line != null)
            {
                var strs = line.Split('$');
                if (strs.Length > 2) wtitle = strs[2].Trim();
                if (strs.Length > 1) name = strs[1].Trim();
                if (strs.Length > 0)
                {
                    string pidStr = strs[0];
                    if (pidStr.Contains(")"))
                        pidStr = pidStr.Split(')')[1];
                    int.TryParse(pidStr, out pid);
                }
            }
            bool ok = pid >= 0 && name != "";
            return ok;
        }
        public static string Pack(Process ps, int? idx = null)
        {
            string sep = "$";
            string wtitle = ps.MainWindowTitle.Trim();
            string str = (idx != null) ?
                $"({idx}) {ps.Id:00000} {sep} " :
                $"{ps.Id:00000} {sep} ";
            str += ps.ProcessName;
            if (!string.IsNullOrEmpty(wtitle))
                str += $" {sep} {wtitle}";
            return str;
        }
    }
}
