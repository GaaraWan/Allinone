using GetNGNum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

namespace AJZReportViewer
{
    public partial class frmMain : Form
    {
        /// <summary>
        /// 清理资料线程
        /// </summary>
        Thread m_DelFilesThread = null;


        string m_path = @"D:\report\work";

        TreeView trvLot;
        ListBox lstFilename;

        AllinoneViewerUI Viewer;

        AnalyzeClass root = null;
        AnalyzeClass AnalyzeLst = null;

        Button btnTrvUpdate;

        Button btnLstUp;
        Button btnLstDown;
        Button btnLstGo;
        Button btnLstUpdate;
        Button btnShowWholeImage;

        DataGridView dgvData => dataGridView1;

        public frmMain()
        {
            InitializeComponent();

            this.Load += FrmMain_Load;
            this.FormClosed += FrmMain_FormClosed;
        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_running = false;
            if (m_DelFilesThread != null)
            {
                m_DelFilesThread.Abort();
                m_DelFilesThread = null;
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            Init();
        }

        void Init()
        {
            this.Text = "德龙AOI 资料查看器 Ver:" + Application.ProductVersion;
            this.Text = "资料查看器 Ver:" + Application.ProductVersion;
            this.Text = "JetEazy Viewer";// + Global.VER.ToString();

            trvLot = treeView1;
            lstFilename = listBox1;

            btnTrvUpdate = button4;

            btnLstUp = button5;
            btnLstDown = button6;
            btnLstGo = button7;
            btnLstUpdate = button8;
            btnShowWholeImage = button10;

            Viewer = allinoneViewerUI1;

            trvLot.HideSelection = false; //可让选中节点保持高亮。
            trvLot.DrawMode = TreeViewDrawMode.OwnerDrawText;
            trvLot.AfterSelect += TrvLot_AfterSelect;
            lstFilename.SelectedIndexChanged += LstFilename_SelectedIndexChanged;
            trvLot.DrawNode += TrvLot_DrawNode;

            updateTreeview();

            btnTrvUpdate.Click += BtnTrvUpdate_Click;

            btnLstUp.Click += BtnLstUp_Click;
            btnLstDown.Click += BtnLstDown_Click;
            btnLstUpdate.Click += BtnLstUpdate_Click;
            btnShowWholeImage.Click += BtnShowWholeImage_Click;

            df_init();

            if (m_DelFilesThread == null)
            {
                m_DelFilesThread = new Thread(new ThreadStart(df_thread));
            }
            m_running = true;
            m_DelFilesThread.Start();


            var watcher = new FileSystemWatcher($"{m_path}");
            watcher.NotifyFilter = NotifyFilters.Attributes
                            | NotifyFilters.CreationTime
                            | NotifyFilters.DirectoryName
                            | NotifyFilters.FileName
                            | NotifyFilters.LastAccess
                            | NotifyFilters.LastWrite
                            | NotifyFilters.Security
                            | NotifyFilters.Size;
            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;
            watcher.Filter = "*.csv";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            autoTrvLotAfterSelect();

            switch(Global.VER)
            {
                case VERSION.SDM2:

                    tabControl1.Controls.RemoveAt(0);
                    rtblog.Visible = true;
                    rtblog.Dock = DockStyle.Fill;

                    break;
                case VERSION.SDM5:

                    grpErrorColor.Visible = false;

                    break;
                case VERSION.MAIN_X6:

                    dgvData.Visible = true;
                    dgvData.Dock = DockStyle.Fill;
                    dgv_init();

                    break;
            }

            this.WindowState = FormWindowState.Minimized;
        }
        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            //AppendMessageToRichTextBox($"Changed: {e.FullPath}");
            autoTrvLotAfterSelect();
        }
        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            string value = $"Created: {e.FullPath}";
            AppendMessageToRichTextBox($"Created: {e.FullPath}");
            autoTrvLotAfterSelect();
        }
        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            //AppendMessageToRichTextBox($"Deleted: {e.FullPath}");
            autoTrvLotAfterSelect();
        }
        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            //AppendMessageToRichTextBox($"Renamed:");
            //AppendMessageToRichTextBox($"    Old: {e.OldFullPath}");
            //AppendMessageToRichTextBox($"    New: {e.FullPath} ");
            autoTrvLotAfterSelect();
        }
        private void AppendMessageToRichTextBox(string message)
        {
            // 在RichTextBox中添加提示信息        
            this.Invoke(new Action(() =>
            {
                if (richTextBox1.TextLength >= 20000)
                    richTextBox1.Text = "";

                richTextBox1.AppendText(message + Environment.NewLine);

                richTextBox1.SelectionStart = richTextBox1.TextLength;
                richTextBox1.ScrollToCaret();

            }));
        }

        void autoTrvLotAfterSelect()
        {
            if (chkManualLook.Checked)
                return;
            if (trvLot.Nodes.Count > 0)
            {
                this.Invoke(new Action(() => trvLot.SelectedNode = trvLot.Nodes[0]));
                if (trvLot.Nodes[0].Nodes.Count > 0)
                {
                    if (trvLot.Nodes[0].Nodes[0].Nodes.Count > 0)
                    {
                        if (trvLot.Nodes[0].Nodes[0].Nodes[0].Nodes.Count > 0)
                        {
                            this.Invoke(new Action(() => trvLot.SelectedNode = trvLot.Nodes[0].Nodes[0].Nodes[0].Nodes[0]));
                        }
                    }
                }
            }
        }

        frmStripShow m_ShowStrip = null;
        private void BtnShowWholeImage_Click(object sender, EventArgs e)
        {
            string pathfilename = Viewer.WholeImage;
            if (File.Exists(pathfilename))
            {
                m_ShowStrip = new frmStripShow(pathfilename, new RectangleF(0, 0, 10, 10), Color.Lime, "");
                m_ShowStrip.ShowDialog();
            }
        }

        private void TrvLot_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            e.DrawDefault = true; //我这里用默认颜色即可，只需要在TreeView失去焦点时选中节点仍然突显

            if ((e.State & TreeNodeStates.Selected) != 0)
            {
                //演示为绿底白字
                e.Graphics.FillRectangle(Brushes.DarkBlue, e.Node.Bounds);
                Font nodeFont = e.Node.NodeFont;
                if (nodeFont == null) nodeFont = ((TreeView)sender).Font;
                e.Graphics.DrawString(e.Node.Text, nodeFont, Brushes.White, Rectangle.Inflate(e.Bounds, 2, 0));
            }
            else
            {
                e.DrawDefault = true;
            }


            if ((e.State & TreeNodeStates.Focused) != 0)
            {
                using (Pen focusPen = new Pen(Color.Black))
                {
                    focusPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    Rectangle focusBounds = e.Node.Bounds;
                    focusBounds.Size = new Size(focusBounds.Width - 1,
                    focusBounds.Height - 1);
                    e.Graphics.DrawRectangle(focusPen, focusBounds);
                }
            }
        }

        private void BtnLstDown_Click(object sender, EventArgs e)
        {
            if (AnalyzeLst == null)
                return;

            if (lstFilename.Items.Count == 0)
                return;

            if (lstFilename.SelectedIndex < 0 || lstFilename.SelectedIndex > lstFilename.Items.Count)
                lstFilename.SelectedIndex = 0;
            else if (lstFilename.SelectedIndex == lstFilename.Items.Count - 1)
                lstFilename.SelectedIndex = 0;
            else
                lstFilename.SelectedIndex++;
        }

        private void BtnLstUp_Click(object sender, EventArgs e)
        {
            if (AnalyzeLst == null)
                return;

            if (lstFilename.Items.Count == 0)
                return;

            if (lstFilename.SelectedIndex <= 0 || lstFilename.SelectedIndex >= lstFilename.Items.Count)
                lstFilename.SelectedIndex = lstFilename.Items.Count - 1;
            else if (lstFilename.SelectedIndex > 0)
                lstFilename.SelectedIndex--;
        }

        private void BtnLstUpdate_Click(object sender, EventArgs e)
        {
            updateListbox(AnalyzeLst);
        }

        private void LstFilename_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AnalyzeLst == null)
                return;

            int index = lstFilename.SelectedIndex;

            if (index < 0)
                return;

            Viewer.Set(AnalyzeLst.FullPath + "\\" + lstFilename.SelectedItem.ToString() + ".csv", lstFilename.Items.Count);
            Viewer.UpdateViewer();

        }

        private void BtnTrvUpdate_Click(object sender, EventArgs e)
        {
            updateTreeview();
        }

        private void TrvLot_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level == 2)
            {
                //root.BranchList[0].BranchList[]
                //updateListbox()
                //e.Node.BackColor = Color.Blue;
                TreeNode par1 = e.Node.Parent;
                TreeNode par2 = par1.Parent;
                int i = 0;
                int j = 0;
                int k = 0;
                bool bOK = false;
                foreach (AnalyzeClass analyze in root.BranchList)
                {
                    foreach (AnalyzeClass analyze1 in analyze.BranchList)
                    {
                        if (analyze1.Name == e.Node.Text && analyze.Name == par1.Text)
                        {
                            bOK = true;
                            break;
                        }

                        j++;
                    }

                    if (bOK)
                        break;

                    i++;
                }

                if (i >= root.BranchList.Count)
                {
                    AnalyzeLst = null;
                    ClearListbox();
                    return;
                }
                if (j >= root.BranchList[i].BranchList.Count)
                {
                    AnalyzeLst = null;
                    ClearListbox();
                    return;
                }

                AnalyzeLst = root.BranchList[i].BranchList[j];
                updateListbox(AnalyzeLst);

            }
            else if (e.Node.Level == 3)
            {
                //e.Node.BackColor = Color.Blue;
                //root.BranchList[0].BranchList[]
                //updateListbox()

                TreeNode par1 = e.Node.Parent;
                TreeNode par2 = par1.Parent;
                int i = 0;
                int j = 0;
                int k = 0;
                bool bOK = false;
                foreach (AnalyzeClass analyze in root.BranchList)
                {
                    j = 0;
                    foreach (AnalyzeClass analyze1 in analyze.BranchList)
                    {
                        k = 0;
                        foreach (AnalyzeClass analyze2 in analyze1.BranchList)
                        {
                            if (analyze2.Name == e.Node.Text && analyze1.Name == par1.Text)
                            {
                                bOK = true;
                                break;
                            }
                            k++;
                        }

                        if (bOK)
                            break;

                        j++;
                    }

                    if (bOK)
                        break;

                    i++;
                }

                if (i >= root.BranchList.Count)
                {
                    AnalyzeLst = null;
                    ClearListbox();
                    return;
                }
                if (j >= root.BranchList[i].BranchList.Count)
                {
                    AnalyzeLst = null;
                    ClearListbox();
                    return;
                }
                if (k >= root.BranchList[i].BranchList[j].BranchList.Count)
                {
                    AnalyzeLst = null;
                    ClearListbox();
                    return;
                }

                AnalyzeLst = root.BranchList[i].BranchList[j].BranchList[k];
                updateListbox(AnalyzeLst);

            }
            else
            {
                ClearListbox();
            }
        }

        void ClearTreeview()
        {
            trvLot.Nodes.Clear();
        }
        void updateTreeview()
        {
            ClearTreeview();


            if (!Directory.Exists(m_path))
                Directory.CreateDirectory(m_path);

            root = new AnalyzeClass(m_path);
            TreeNode tnroot = new TreeNode(root.Name);
            trvLot.Nodes.Add(tnroot);
            string[] rootDirs = Directory.GetDirectories(root.FullPath);
            foreach (string str in rootDirs)
            {
                if (str == "D:\\report\\work\\auto")
                {
                    AnalyzeClass branch = new AnalyzeClass(str);
                    root.BranchList.Add(branch);

                    TreeNode tn1 = new TreeNode(branch.Name);
                    tnroot.Nodes.Add(tn1);

                    DirectoryInfo di = new DirectoryInfo(branch.FullPath);
                    DirectoryInfo[] arrDir = di.GetDirectories();
                    SortAsFolderCreationTime(ref arrDir);

                    //string[] branchDirs = Directory.GetDirectories(branch.FullPath);
                    foreach (var str2 in arrDir)
                    {
                        AnalyzeClass branch2 = new AnalyzeClass(str2.FullName);
                        branch.BranchList.Add(branch2);

                        TreeNode tn2 = new TreeNode(branch2.Name);
                        tn1.Nodes.Add(tn2);


                        DirectoryInfo di2 = new DirectoryInfo(branch2.FullPath);
                        DirectoryInfo[] arrDir2 = di2.GetDirectories();
                        SortAsFolderCreationTime(ref arrDir2);

                        //string[] branchDirs2 = Directory.GetDirectories(branch2.FullPath);
                        foreach (var str3 in arrDir2)
                        {
                            AnalyzeClass branch3 = new AnalyzeClass(str3.FullName);
                            branch2.BranchList.Add(branch3);

                            TreeNode tn3 = new TreeNode(branch3.Name);
                            tn2.Nodes.Add(tn3);

                            //string[] branchFiles2 = Directory.GetFiles(branch2.FullPath);
                            //foreach (string str3 in branchFiles2)
                            //{
                            //    AnalyzeClass branch3 = new AnalyzeClass(str3);
                            //    branch2.BranchList.Add(branch3);

                            //    TreeNode tn3 = new TreeNode(branch3.Name);
                            //    tn2.Nodes.Add(tn3);
                            //}

                        }
                    }
                }
            }

            trvLot.ExpandAll();

        }

        void ClearListbox()
        {
            lstFilename.Items.Clear();
        }

        void updateListbox(AnalyzeClass eAnalyze)
        {
            if (eAnalyze == null)
                return;

            ClearListbox();

            DirectoryInfo di = new DirectoryInfo(eAnalyze.FullPath);
            FileInfo[] arrFi = di.GetFiles("*.csv");
            SortAsFileCreationTime(ref arrFi);

            //string[] branchFiles2 = Directory.GetFiles(eAnalyze.FullPath);
            foreach (var str3 in arrFi)
            {
                string[] strs = str3.FullName.Split('\\');
                string csv = strs[strs.Length - 1];

                lstFilename.Items.Add(csv.Replace(".csv", ""));
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 添加路径ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = PathPicker("选择要清理的路径:", "");
            if (!string.IsNullOrEmpty(path))
            {
                lstFiles.Items.Add(path);
                m_FilesPathList.Add(path);
                SaveDataEX(path, recFile);
            }
        }

        private void 删除路径ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int sel = lstFiles.SelectedIndex;
            if (sel > -1)
            {
                lstFiles.Items.RemoveAt(sel);
                m_FilesPathList.RemoveAt(sel);

                string data = string.Empty;
                int i = 0;
                while (i < lstFiles.Items.Count)
                {
                    string dir = lstFiles.Items[i].ToString();
                    if (!string.IsNullOrEmpty(dir))
                        data += dir + Environment.NewLine;

                    i++;
                }
                SaveDataEX(data, recFile, false);
            }
        }

        private void 设定ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 清理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_manual = true;
        }

        #region 清理资料
        List<string> m_FilesPathList = new List<string>();
        ListBox lstFiles;
        string recFile = "appFiles.txt";
        bool m_running = false;
        bool m_manual = false;
        void df_thread()
        {
            while (m_running)
            {
                Thread.Sleep(100);
                if (m_manual)
                {
                    this.Invoke(new Action(() => toolStripTextBox1.Text = "清理中(手动)..."));
                    m_manual = false;
                    df_delFiles();
                    this.Invoke(new Action(() => toolStripTextBox1.Text = "清理(手动)完成"));
                }
                if (INIClass.Instance.IsOpenAutoDelFiles)
                {
                    TimeSpan _ts = DateTime.Now - INIClass.Instance.xDelFileTimeStart;
                    if (_ts.TotalSeconds > 5) // && _ts.TotalSeconds < 10)
                    {
                        INIClass.Instance.xDelFileTimeStart = GetNextRunTime(DateTime.Now);
                        INIClass.Instance.Save();

                        this.Invoke(new Action(() => toolStripTextBox1.Text = "清理中(自动)..."));
                        df_delFiles();
                        this.Invoke(new Action(() => toolStripTextBox1.Text = "清理(自动)完成"));

                    }
                }
            }
        }
        private DateTime GetNextRunTime(DateTime currentTime)
        {
            DateTime nextRunTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, INIClass.Instance.xNowHour, 0, 0);
            if (currentTime > nextRunTime)
            {
                nextRunTime = nextRunTime.AddDays(1);
            }
            return nextRunTime;
        }

        void df_delFiles()
        {
            if (m_FilesPathList.Count <= 0)
                return;

            this.Invoke(new Action(() => menuStrip1.Enabled = false));
            this.Invoke(new Action(() => lstFiles.Enabled = false));

            //menuStrip1.Enabled = false;
            //lstFiles.Enabled = false;

            int i = 0;
            while (i < m_FilesPathList.Count)
            {
                string dir = m_FilesPathList[i];

                DelFiles(dir, INIClass.Instance.xMonthCount);

                i++;
            }

            this.Invoke(new Action(() => menuStrip1.Enabled = true));
            this.Invoke(new Action(() => lstFiles.Enabled = true));
            //menuStrip1.Enabled = true;
            //lstFiles.Enabled = true;
        }
        void df_init()
        {
            INIClass.Instance.Initial();

            pgINI.SelectedObject = INIClass.Instance;

            lstFiles = listBox2;
            m_FilesPathList.Clear();

            string dataStr = string.Empty;
            ReadData(ref dataStr, recFile);

            if (!string.IsNullOrEmpty(dataStr))
            {
                string[] files = dataStr.Replace(Environment.NewLine, "#").Split('#');
                foreach (string str in files)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        lstFiles.Items.Add(str);
                        m_FilesPathList.Add(str);
                    }
                }
            }

            this.Invoke(new Action(() => toolStripTextBox1.Text = "初始化完成"));
        }
        string PathPicker(string Description, string DefaultPath)
        {
            string retStr = "";

            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.Description = Description;
            fd.ShowNewFolderButton = false;
            //fd.SelectedPath = DefaultPath;

            if (fd.ShowDialog().Equals(DialogResult.OK))
            {
                if (fd.SelectedPath != "")
                    retStr = fd.SelectedPath;
            }
            else
                retStr = "";

            return retStr;
        }
        void DelFiles(string targetDirectory, int eMonth = 3)
        {
            //// 设置要删除的目标目录路径
            //string targetDirectory = @"C:\Path\To\Your\Directory";

            // 获取当前时间
            DateTime currentTime = DateTime.Now;

            // 设置时间限制（3个月前）
            DateTime timeLimit = currentTime.AddMonths(-eMonth);

            // 删除目录及其子目录中的文件和文件夹
            DeleteOldFilesAndDirectories(targetDirectory, timeLimit);
        }
        void DeleteOldFilesAndDirectories(string directoryPath, DateTime timeLimit)
        {
            try
            {
                // 获取当前目录中的所有文件
                string[] files = Directory.GetFiles(directoryPath);
                // 获取当前目录中的所有子目录
                string[] directories = Directory.GetDirectories(directoryPath);

                // 遍历文件并删除旧文件
                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.LastWriteTime < timeLimit)
                    {
                        try
                        {
                            File.Delete(file);
                            Console.WriteLine($"Deleted file: {file}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error deleting file {file}: {ex.Message}");
                        }
                    }
                }

                // 遍历子目录并递归删除旧文件和空目录
                foreach (string dir in directories)
                {
                    DeleteOldFilesAndDirectories(dir, timeLimit);

                    // 检查当前目录是否为空，如果为空则删除
                    DirectoryInfo dirInfo = new DirectoryInfo(dir);
                    if (dirInfo.GetFiles().Length == 0 && dirInfo.GetDirectories().Length == 0)
                    {
                        try
                        {
                            Directory.Delete(dir);
                            Console.WriteLine($"Deleted directory: {dir}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error deleting directory {dir}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accessing directory {directoryPath}: {ex.Message}");
            }
        }
        void SaveDataEX(string DataStr, string FileName, bool append = true)
        {
            System.IO.StreamWriter stm = null;

            try
            {
                stm = new System.IO.StreamWriter(FileName, append, System.Text.Encoding.Default);
                stm.WriteLine(DataStr);
                stm.Flush();
                stm.Close();
                stm.Dispose();
                stm = null;
            }
            catch (Exception ex)
            {

            }

            if (stm != null)
                stm.Dispose();
        }
        void ReadData(ref string DataStr, string FileName)
        {
            if (!File.Exists(FileName))
                return;

            FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.None);
            StreamReader Srr = new StreamReader(fs, Encoding.Default);

            DataStr = Srr.ReadToEnd();

            Srr.Close();
            Srr.Dispose();
        }

        #endregion

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            INIClass.Instance.Save();
            this.Invoke(new Action(() => toolStripTextBox1.Text = "保存成功"));
        }


        /// <summary>
        /// C#按文件夹创建时间排序（倒序）
        /// </summary>
        /// <param name="dirs">待排序文件夹数组</param>
        private void SortAsFolderCreationTime(ref DirectoryInfo[] dirs)
        {
            Array.Sort(dirs, delegate (DirectoryInfo x, DirectoryInfo y) { return y.CreationTime.CompareTo(x.CreationTime); });
        }
        /// <summary>
        /// C#按创建时间排序（倒序）
        /// </summary>
        /// <param name="arrFi">待排序数组</param>
        private void SortAsFileCreationTime(ref FileInfo[] arrFi)
        {
            Array.Sort(arrFi, delegate (FileInfo x, FileInfo y) { return y.CreationTime.CompareTo(x.CreationTime); });
        }

        private void 查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (Global.VER)
            {
                case VERSION.SDM2:
                    break;
                case VERSION.MAIN_X6:
                    ControlEnable(false);
                    query_dgv_data();
                    ControlEnable(true);
                    break;
            }
        }
        private void 报表一ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            switch (Global.VER)
            {
                case VERSION.SDM2:
                    System.Threading.Thread th_report = new Thread(new ThreadStart(_report1));
                    th_report.IsBackground = true;
                    th_report.Start();
                    break;
                case VERSION.MAIN_X6:
                    ControlEnable(false);
                    output_dgv_data();
                    ControlEnable(true);
                    break;
            }
        }

        #region 报表输出

        List<ReportItem> ReportItems = new List<ReportItem>();

        private void _report1()
        {
            ControlEnable(false);
            LOG_MSG($"开始导出中...");
            try
            {
                //获取报表路径
                string _dataroot = INIClass.Instance.ReadINIValue("MainX6 Control", "RootPath", "D:\\DataRoot", "D:\\JETEAZY\\ALLINONE-MAIN_SDM2\\CONFIG.ini");
                string[] dataDirs = Directory.GetDirectories(_dataroot + "DataRoot");
                ReportItems.Clear();
                foreach (string dir in dataDirs)
                {
                    string[] strings = dir.Split('\\');
                    //DateTime.TryParse(strings[strings.Length - 1], out DateTime myDir);
                    bool bOK = double.TryParse(strings[strings.Length - 1], out double myDir);
                    if (!bOK)
                    {
                        LOG_MSG($"!文件夹异常:{strings[strings.Length - 1]}");
                        continue;
                    }

                    //double myDir = double.Parse(strings[strings.Length - 1]);
                    double myDir1 = double.Parse(dtp1.Value.ToString("yyyyMMdd"));
                    double myDir2 = double.Parse(dtp2.Value.ToString("yyyyMMdd"));

                    if (myDir >= myDir1 && myDir <= myDir2)
                    {
                        string[] dataDirs1 = Directory.GetDirectories(dir);
                        foreach (string dir1 in dataDirs1)
                        {
                            string[] strings1 = dir1.Split('\\');
                            string datapath = $"{dir1}\\DataRecord\\{strings1[strings1.Length - 1]}.ini";
                            string chipTestAllCount = INIClass.Instance.ReadINIValue("Basic Control", "chipTestAllCount", "0", datapath);
                            string chipTestFailCount = INIClass.Instance.ReadINIValue("Basic Control", "chipTestFailCount", "0", datapath);

                            ReportItem reportItem = new ReportItem();
                            reportItem.DateStr = strings[strings.Length - 1];
                            reportItem.RecipeName = strings1[strings1.Length - 1];
                            reportItem.AllCount = chipTestAllCount;
                            reportItem.FailCount = chipTestFailCount;

                            ReportItems.Add(reportItem);

                            LOG_MSG($"@读到数据...{reportItem.ToReport1()}");
                        }
                    }
                }
                if (ReportItems.Count > 0)
                {

                    LOG_MSG($"写入数据开始...");

                    string dataStr = string.Empty;
                    dataStr += $"开始时间,{dtp1.Text}," + Environment.NewLine;
                    dataStr += $"结束时间,{dtp2.Text}," + Environment.NewLine;
                    dataStr += Environment.NewLine;
                    dataStr += $"日期,档案名(参数名称),作业数,不良数,不良率,{Environment.NewLine}";
                    foreach (ReportItem reportItem in ReportItems)
                    {
                        dataStr += reportItem.ToReport1();
                    }

                    string reportpath = "D:\\reports";
                    if (!Directory.Exists(reportpath))
                        Directory.CreateDirectory(reportpath);
                    string filename = $"Report_{DateTime.Now.ToString("yyyyMMddHHmmss")}_({dtp1.Value.ToString("yyyyMMdd")}-{dtp2.Value.ToString("yyyyMMdd")}).csv";

                    StreamWriter streamWriter = null;
                    streamWriter = new StreamWriter(reportpath + "\\" + filename, false, Encoding.Default);
                    streamWriter.Write(dataStr);
                    streamWriter.Close();


                    LOG_MSG($"@写入数据完成...路径:{reportpath + "\\" + filename}");
                }
            }
            catch (Exception ex)
            {
                LOG_MSG($"!导出数据异常:{ex.Message}");
            }
            LOG_MSG($"@导出完成");
            ControlEnable(true);
        }

        GetNgNum getNgNum = new GetNgNum();
        void dgv_init()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            // 第一列：NG类型
            var NgtypeCol = new DataGridViewTextBoxColumn
            {
                Name = "NgType",
                HeaderText = "测试结果",
                Width = 120,
                ReadOnly = true
            };
            dataGridView1.Columns.Add(NgtypeCol);

            var dateList = new List<string>();
            var cur = dtp1.Value.Date;
            var end = dtp2.Value.Date;

            string dateStr = cur.ToString("yyyyMMdd");
            string dateFolder = Path.Combine(getNgNum.SourcePath, dateStr);

            if (Directory.Exists(dateFolder))
            {
                var col = new DataGridViewTextBoxColumn
                {
                    Name = dateStr,
                    HeaderText = dateStr,
                    ReadOnly = true
                };
                dataGridView1.Columns.Add(col);
                dateList.Add(dateStr);
            }
            cur = cur.AddDays(1);


            // 每个 NG 类型一行
            foreach (var kv in getNgNum.ngTypeMap.OrderBy(k => k.Key))
            {
                var row = new List<object>();
                row.Add(kv.Value); // 第一列是 NG 类型名称

                foreach (var date in dateList)
                {
                    var dailyCounts = getNgNum.GetNgCountsByDate(date);
                    int count = dailyCounts.ContainsKey(kv.Key) ? dailyCounts[kv.Key] : 0;
                    row.Add(count);
                }

                dataGridView1.Rows.Add(row.ToArray());
            }


            // 行上色（区分不同 NG 类型）
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string ngType = row.Cells[0].Value?.ToString();
                switch (ngType)
                {
                    case "正确":
                        row.DefaultCellStyle.BackColor = Color.Green;
                        break;
                    case "印字错误":
                        row.DefaultCellStyle.BackColor = Color.LightBlue;
                        break;
                    case "印字偏移":
                        row.DefaultCellStyle.BackColor = Color.LightYellow;
                        break;
                    case "油墨错误":
                        row.DefaultCellStyle.BackColor = Color.LightCyan;
                        break;
                    case "印字缺失":
                        row.DefaultCellStyle.BackColor = Color.Red;
                        break;
                    case "不检测":
                        row.DefaultCellStyle.BackColor = Color.DarkViolet;
                        break;
                    case "其他":
                        row.DefaultCellStyle.BackColor = Color.LightGray;
                        break;
                    case "2D对比错误":
                        row.DefaultCellStyle.BackColor = Color.Orange;
                        break;
                    case "2D读取错误":
                        row.DefaultCellStyle.BackColor = Color.LightGoldenrodYellow
                            ;
                        break;
                    case "2D重复":
                        row.DefaultCellStyle.BackColor = Color.Pink;
                        break;
                }
                dataGridView1.AllowUserToAddRows = false;
            }
        }
        void query_dgv_data()
        {
            DateTime start = dtp1.Value.Date;
            DateTime end = dtp2.Value.Date;

            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();

            // 第一列：测试类型
            var typeCol = new DataGridViewTextBoxColumn
            {
                Name = "NgType",
                HeaderText = "测试结果",
                Width = 120,
                ReadOnly = true
            };
            dataGridView1.Columns.Add(typeCol);

            // 第二列：时间
            string dateRangeStr = start.ToString("yyyyMMdd") + "-" + end.ToString("yyyyMMdd");
            var rangeCol = new DataGridViewTextBoxColumn
            {
                Name = "DateRange",
                HeaderText = dateRangeStr,
                ReadOnly = true
            };
            rangeCol.Width = 220;
            dataGridView1.Columns.Add(rangeCol);

            //第三列：占比
            var percentageCol = new DataGridViewTextBoxColumn
            {
                Name = "PercentageCol",
                HeaderText = "百分比",
                Width = 120,
                ReadOnly = true
            };
            dataGridView1.Columns.Add(percentageCol);


            // 计算每个 NG 类型在选定区间内的总数
            var ngKeys = getNgNum.ngTypeMap.Keys.OrderBy(k => k).ToList();
            foreach (var kv in getNgNum.ngTypeMap.OrderBy(k => k.Key))
            {
                int totalCount = 0;
                var cur = start;
                while (cur <= end)
                {
                    string dateStr = cur.ToString("yyyyMMdd");
                    string dateFolder = Path.Combine(getNgNum.SourcePath, dateStr);
                    if (Directory.Exists(dateFolder))
                    {
                        var dailyCounts = getNgNum.GetNgCountsByDate(dateStr);
                        totalCount += dailyCounts.ContainsKey(kv.Key) ? dailyCounts[kv.Key] : 0;
                    }
                    cur = cur.AddDays(1);
                }

                dataGridView1.Rows.Add(kv.Value, totalCount);
            }


            // 总计数量
            int totalAll = 0;
            foreach (var kv in getNgNum.ngTypeMap.Keys)
            {
                totalAll += dataGridView1.Rows
                    .Cast<DataGridViewRow>()
                    .Where(r => r.Cells[0].Value != null && r.Cells[0].Value.ToString() == getNgNum.ngTypeMap[kv])
                    .Sum(r => Convert.ToInt32(r.Cells[1].Value));
            }
            int totalRowIndex = dataGridView1.Rows.Add("数量总计", totalAll);

            var totalRow = dataGridView1.Rows[totalRowIndex];
            totalRow.DefaultCellStyle.BackColor = Color.Gray;


            // 计算百分比列
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[0].Value?.ToString() == "数量总计")
                {
                    // 总计行占比显示 100%
                    row.Cells[2].Value = "100%";
                    continue;
                }

                int count = Convert.ToInt32(row.Cells[1].Value);
                double percent = totalAll > 0 ? (count * 100.0 / totalAll) : 0;
                row.Cells[2].Value = percent.ToString("0.0") + "%";
            }


            // 行上色
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string ngType = row.Cells[0].Value?.ToString();
                switch (ngType)
                {
                    case "正确":
                        row.DefaultCellStyle.BackColor = Color.Green;
                        break;
                    case "印字错误":
                        row.DefaultCellStyle.BackColor = Color.LightBlue;
                        break;
                    case "印字偏移":
                        row.DefaultCellStyle.BackColor = Color.LightYellow;
                        break;
                    case "油墨错误":
                        row.DefaultCellStyle.BackColor = Color.LightCyan;
                        break;
                    case "印字缺失":
                        row.DefaultCellStyle.BackColor = Color.Red;
                        break;
                    case "不检测":
                        row.DefaultCellStyle.BackColor = Color.DarkViolet;
                        break;
                    case "其他":
                        row.DefaultCellStyle.BackColor = Color.LightGray;
                        break;
                    case "2D对比错误":
                        row.DefaultCellStyle.BackColor = Color.Orange;
                        break;
                    case "2D读取错误":
                        row.DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
                        break;
                    case "2D重复":
                        row.DefaultCellStyle.BackColor = Color.Pink;
                        break;
                }
            }
        }
        
        void output_dgv_data()
        {
            DateTime start = dtp1.Value.Date;
            DateTime end = dtp2.Value.Date;

            var dateList = new List<string>();
            var cur = start;
            while (cur <= end)
            {
                string dateStr = cur.ToString("yyyyMMdd");
                string dateFolder = Path.Combine(getNgNum.SourcePath, dateStr);
                if (Directory.Exists(dateFolder))
                    dateList.Add(dateStr);
                cur = cur.AddDays(1);
            }

            if (dateList.Count == 0)
            {
                MessageBox.Show("选定日期区间内没有可导出的目录。");
                return;
            }

            try
            {
                string newFileName = getNgNum.ParseByDates(dateList, start, end);

                if (!string.IsNullOrEmpty(newFileName))
                {
                    string fileName = Path.GetFileName(newFileName);
                    MessageBox.Show($"导出成功!\n目录: {getNgNum.TargetPath}\n文件: {fileName}", "提示");

                    // 自动打开文件
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = newFileName,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show("打开文件失败: " + ex2.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出出错: " + ex.Message);
            }
        }

        #endregion


        void LOG_MSG(string logStr)
        {
            rtblog.Invoke(new Action(() =>
            {
                rtblog.Text += logStr + Environment.NewLine;
                if (rtblog.Text.StartsWith("!"))
                {
                    rtblog.BackColor = Color.Red;
                }
                else if (rtblog.Text.StartsWith("@"))
                {
                    rtblog.BackColor = Color.Lime;
                }
                else
                {
                    //rtblog.BackColor = Color.Transparent;
                }
            }));
        }
        void ControlEnable(bool isenable)
        {
            this.Invoke(new Action(() =>
            {
                menuStrip2.Enabled = isenable;
                dtp1.Enabled = isenable;
                dtp2.Enabled = isenable;
                rtblog.Enabled = isenable;
            }));
        }

        
    }
}
