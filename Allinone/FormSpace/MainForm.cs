using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using JetEazy;
using JetEazy.BasicSpace;
using JetEazy.ControlSpace;
using JetEazy.FormSpace;
using JetEazy.DBSpace;
using JetEazy.UISpace;
using Allinone.UISpace;
using Allinone.UISpace.RUNUISpace;
using Allinone.UISpace.RCPUISpace;
using Allinone.UISpace.STPUISpace;
using Allinone.OPSpace;
using Allinone.ControlSpace;
using MoveGraphLibrary;
using WorldOfMoveableObjects;
using JetEazy.AccountMgr;
using JetEazy.QUtilities;

using JzDisplay;
using JzDisplay.UISpace;
using JzASN.OPSpace;
using JzASN.FormSpace;
using JetEazy.ControlSpace.MotionSpace;
using JetEazy.PlugSpace;
using ZXing;
using Allinone.UISpace.SHOWUISpace;
using Allinone.BasicSpace;
using ServiceMessageClass;
using EzSegClientLib;
using System.Drawing.Imaging;
using System.IO;

namespace Allinone.FormSpace
{
    using HandleCan = IntPtr;
    public partial class MainForm : Form, IMessageFilter
    {
        int WM_MOUSEWHEEL = 0x20A;

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point pt);
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
        VersionEnum VERSION
        {
            get
            {
                return Universal.VERSION;
            }
        }
        OptionEnum OPTION
        {
            get
            {
                return Universal.OPTION;
            }
        }
        LayoutEnum LAYOUT
        {
            get
            {
                return Universal.LAYOUT;
            }

        }
        bool IsDebug
        {
            get
            {
                return Universal.IsDebug;
            }
        }

        BannerForm BANNERFORM;
        EsssDBClass ESSDB
        {
            get
            {
                return Universal.ESSDB;
            }
        }
        AccDBClass ACCDB
        {
            get
            {
                return Universal.ACCDB;
            }
        }
        RcpDBClass RCPDB
        {
            get
            {
                return Universal.RCPDB;
            }
        }
        CCDCollectionClass CCDCollection
        {
            get
            {
                return Universal.CCDCollection;
            }

        }
        AlbumCollectionClass AlbumCollection
        {
            get
            {
                return Universal.ALBCollection;
            }
        }
        AlbumClass AlbumNow
        {
            get
            {
                return AlbumCollection.AlbumNow;
            }
        }
        AlbumClass AlbumWork
        {
            get
            {
                return AlbumCollection.AlbumWork;
            }
        }
        ASNCollectionClass ASNCollection
        {
            get
            {
                return Universal.ASNCollection;
            }
        }
        MachineCollectionClass MACHINECollection
        {
            get
            {
                return Universal.MACHINECollection;
            }
        }
        ResultClass RESULT
        {
            get
            {
                return Universal.RESULT;
            }

        }

        ClientSocket X6_HANDLE_CLIENT
        {
            get { return Universal.X6_HANDLE_CLIENT; }
        }
        ClientSocket X6_LASER_CLIENT
        {
            get { return Universal.X6_LASER_CLIENT; }
        }

        Label lblTotalStatus;
        Label lblDebug;
        Label lblDebug2;
        Label lblTestPath;
        ComboBox cboShowCam;
        Button btnCCDRESET;

        Label lblJumboServer;
        Label lblCrystalServer;
        Label lblConnectionFail;
        EssUI ESSUI;

        TabControl tabCtrl;
        Panel pnlMappingUI;

        MappingClass MappingUI;
        Label lblMappingDataString;
        Label lblMappingBarcodeString;


        #region 1440X900

        RcpUI RCPUI;
        StpUI STPUI;
        RunUI RUNUI;

        #endregion

        #region 1280X800

        RcpV1UI RCPV1UI;
        StpV1UI STPV1UI;
        RunV1UI RUNV1UI;

        #endregion

        CtrlUI CTRLUI;

        SHOWSDM2UI SUPERSHOWSDM2UI;

        DispUI DISPUI;

        JzTimes MainTime = new JzTimes();
        JzTimes RunTime = new JzTimes();
        Timer MainTimer;

        string MoveString = "";
        Mover ShowMover = new Mover();
        bool IsNeedToShowMover = false;

        bool IsLiveCapturing = false;
        bool IsChangeShowCam = false;

        bool IsJumboOnline = false;
        bool IsCrystalOnline = false;
        //用於一次開啟廣播
        bool IsJumboOnlineX = false;
        bool IsCrystalOnlineX = false;
        bool IsNeedToChange = false;
        bool isStart = false;
        /// <summary>
        /// PLC通信状态
        /// </summary>
        int iPLCStatus = 0;

        //主程序扫描时间
        JzTimes JzMainScanTime = new JzTimes();
        int JzScanTimeMS = 0;

        JzTimes JzQFactoryUpload = new JzTimes();
        System.Diagnostics.Stopwatch myloadwatch = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch myTickCheckwatch = new System.Diagnostics.Stopwatch();
        bool isLoadOK = false;
        DateTime plcerrorStart = DateTime.Now;
        bool IsPlcWarningShow = false;
        MessageForm xFrmWarning;
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        //[DllImport("User32.dll", EntryPoint = "FindWindow")]
        //public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        bool _getMxComponent()
        {

            bool ret = false;

            if (Universal.IsNoUseIO)
                return true;

            string _path = Universal.WORKPATH + "\\mx\\MxComponent.exe";
            if (System.IO.File.Exists(_path))
            {
                string _pathname1 = Universal.WORKPATH + "\\mx\\OK.TXT";
                string _pathname2 = Universal.WORKPATH + "\\mx\\NG.TXT";

                if (System.IO.File.Exists(_pathname1))
                    System.IO.File.Delete(_pathname1);

                if (System.IO.File.Exists(_pathname2))
                    System.IO.File.Delete(_pathname2);

                IntPtr hwnd = FindWindow(null, "MxComponent");
                if (hwnd == IntPtr.Zero)
                {
                    System.Diagnostics.Process.Start(_path);
                }

                System.Threading.Thread.Sleep(2500);
                while (true)
                {
                    hwnd = FindWindow(null, "MxComponent");
                    if (hwnd == IntPtr.Zero)
                    {
                        break;
                    }
                }


                if (System.IO.File.Exists(_pathname1))
                {
                    ret = true;
                    System.IO.File.Delete(_pathname1);
                }
                else
                {
                    ret = false;
                }

                if (System.IO.File.Exists(_pathname2))
                    System.IO.File.Delete(_pathname2);

            }

            return ret;
        }

        #region 等比例缩放窗口

        public float Xvalue;
        public float Yvalue;
        bool flag = false;

        //private void Form1_Resize(object sender, EventArgs e)
        //{
        //    if (flag)
        //    {
        //        float newx = (this.Width) / Xvalue;
        //        float newy = this.Height / Yvalue;
        //        setControls(newx, newy, this);
        //    }
        //}

        private void setControls(float newx, float newy, Control cons)
        {
            foreach (Control con in cons.Controls)
            {
                if (con.AccessibleDescription == null)
                    continue;

                string[] mytag = con.AccessibleDescription.ToString().Split(new char[] { ':' });
                float a = Convert.ToSingle(mytag[0]) * newx;
                con.Width = (int)a;
                a = Convert.ToSingle(mytag[1]) * newy;
                con.Height = (int)(a);
                a = Convert.ToSingle(mytag[2]) * newx;
                con.Left = (int)(a);
                a = Convert.ToSingle(mytag[3]) * newy;
                con.Top = (int)(a);
                //Single currentSize = Convert.ToSingle(mytag[4]) * newx;
                Single currentSize = Convert.ToSingle(mytag[4]) * INI.user_screen_scale;
                //Single currentSize = Convert.ToSingle(mytag[4]) * newy;

                //改变字体大小

                //if (con.Name != "RunUI")
                //{
                //    //con.Name = "RunUI";

                //}

                switch (con.Name)
                {
                    case "RunUI":
                    case "RcpUI":
                    case "StpUI":
                        break;
                    default:
                        con.Font = new Font(con.Font.Name, currentSize, (INI.user_screen_bold ? FontStyle.Bold : con.Font.Style), con.Font.Unit);
                        //con.Font.Bold = true;
                        break;
                }



                if (con.Controls.Count > 0)
                {
                    try
                    {
                        setControls(newx, newy, con);
                    }
                    catch
                    { }
                }
            }

        }
        private void setTag(Control cons)
        {
            foreach (Control con in cons.Controls)
            {
                con.AccessibleDescription = con.Width + ":" + con.Height + ":" + con.Left + ":" + con.Top + ":" + con.Font.Size;
                if (con.Controls.Count > 0)
                    setTag(con);
            }
        }

        #endregion


        public MainForm()
        {
            switch (LAYOUT)
            {
                case LayoutEnum.L1280X800:
                    //  InitializeComponent();
                    InitializeComponent1280X800();
                    break;
                case LayoutEnum.L1440X900:
                    InitializeComponent1440X900();
                    break;
                default:
                    InitializeComponent();
                    //  InitializeComponent1440X900();
                    break;
            }

            //Universal.OCRCollection = new JzOCR.OPSpace.OCRCollectionClass();
            //Universal.OCRCollection.Initial();

            //JzOCR.FormSpace.OCRForm form = new JzOCR.FormSpace.OCRForm();
            //form.ShowDialog();
            //return;

            InitialInternal();

            this.Load += MainForm_Load;
            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (Universal.VERSION)
            {
                case VersionEnum.ALLINONE:
                    //恢复校正点位
                    if (e.Alt && e.Control && e.KeyCode == Keys.R)
                    {
                        if (Universal.SCREENPOINTS != null)
                        {
                            Universal.SCREENPOINTS.AllinoneCalibrate();
                        }
                    }
                    else if (e.Alt && e.Control && e.KeyCode == Keys.C)
                    {
                        if (Universal.SCREENPOINTS != null)
                        {
                            List<Point> ptlist = new List<Point>();
                            Universal.SCREENPOINTS.m_showmypoints.DrawMyPaints(ptlist);
                        }
                    }

                    break;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            DISPUI.Initial(5, 0.02f);
            isStart = DISPUI.DispUIload(this);

            isStart = true;

            JetEazy.LoggerClass.Instance.WriteLog("型号:" + Universal.OPTION + "版本:" + Universal.VERSION + "系统启动中...");
            Initial();
            JetEazy.LoggerClass.Instance.WriteLog("系统启动完成.");

            myloadwatch.Restart();
            myloadwatch.Start();

            myTickCheckwatch.Restart();

            switch (Universal.VERSION)
            {
                case VersionEnum.ALLINONE:

                    ESSUI.FillBCCOUNT(INI.ALLCOUNT, INI.BCNGCOUNT);
                    //ESSUI.ShowPLC_RxTime(Universal.VersionDate + "_" + Universal.OPTION.ToString() + " " + JzHiveClass.HiveVersion);// + "_ " + ms.ToString() + " ms");

                    switch (Universal.OPTION)
                    {
                        case OptionEnum.MAIN_SDM3:
                        case OptionEnum.MAIN_SDM2:
                            ESSUI.ShowPLC_RxTime(Universal.VersionDate + "_" + Universal.OPTION.ToString());

                            break;
                        case OptionEnum.MAIN_SDM1:
                        case OptionEnum.MAIN_SD:
                        case OptionEnum.MAIN_X6:
                        case JetEazy.OptionEnum.MAIN_SERVICE:

                            ESSUI.ShowPLC_RxTime(Universal.VersionDate + "_" + Universal.OPTION.ToString());

                            this.StartPosition = FormStartPosition.Manual;
                            this.Location = new Point(0, 0);
                            this.FormBorderStyle = FormBorderStyle.Sizable;
                            this.ControlBox = true;
                            this.FormClosing += MainForm_FormClosing;
                            this.SizeChanged += MainForm_SizeChanged;
                            _updateUI();

                            break;

                        //case OptionEnum.MAIN_SD:
                        //    ESSUI.ShowPLC_RxTime(Universal.VersionDate + "_" + Universal.OPTION.ToString());
                        //    break;
                        default:
                            ESSUI.ShowPLC_RxTime(Universal.VersionDate + "_" + Universal.OPTION.ToString() + " " + JzHiveClass.HiveVersion);
                            break;
                    }

                    break;
            }
        }

        void _updateUI()
        {
            Xvalue = 1150;// this.Width;
            Yvalue = 1024;// this.Height;
                          //Yvalue = this.Height;

            Xvalue = Screen.PrimaryScreen.Bounds.Width - 130;// this.Width;
            Yvalue = Screen.PrimaryScreen.Bounds.Height;// this.Height;

            Xvalue = INI.user_screen_width - 130;
            Yvalue = INI.user_screen_height;

            switch (Universal.OPTION)
            {
                case OptionEnum.MAIN_SDM3:
                case OptionEnum.MAIN_SDM2:
                case OptionEnum.MAIN_SDM1:
                case OptionEnum.MAIN_SD:
                    Xvalue = INI.user_screen_width;
                    Yvalue = INI.user_screen_height - 30;
                    break;
            }

            flag = true;
            setTag(this);

            if (flag)
            {
                //float newx = this.Width / Xvalue;
                //float newy = this.Height / Yvalue;

                float newx = Xvalue / this.Width;
                float newy = Yvalue / this.Height;

                setControls(newx, newy, this);
            }

            this.Width = 1280;
            this.Height = 1024;

            this.Width = INI.user_screen_width;
            this.Height = INI.user_screen_height;

            //this.Width = Screen.PrimaryScreen.Bounds.Width;
            //this.Height = Screen.PrimaryScreen.Bounds.Height;

            this.WindowState = FormWindowState.Maximized;
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            //_updateUI();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        public static int IsCheckOK()
        {
            string path = Universal.MAINPATH + @"\WORK\";
            JetEazy.Universal.MYDECODE = path;
            DispUI disp = new DispUI();
            bool bOK = disp.DispUIload();
            Universal.InitialEx();
            //ProjectForAllinone.ProjectClass project = new ProjectForAllinone.ProjectClass();
            //bool isok = project.GetDecode(JetEazy.Universal.MYDECODE);
            return (bOK ? 0 : 1);
        }

        void InitialInternal()
        {
            string path = Universal.MAINPATH + @"\WORK\";
            JetEazy.Universal.MYDECODE = path;
            SUPERSHOWSDM2UI = showsdM2UI1;
            DISPUI = dispUI1;
            switch (LAYOUT)
            {
                case LayoutEnum.L1280X800:
                    DISPUI.Size = new Size(1054, 799);
                    break;
                default:
                    DISPUI.Size = new Size(1439, 580);
                    break;
            }

            switch (Universal.OPTION)
            {
                case OptionEnum.MAIN_SD:

                    if (!Universal.IsNoUseIO)
                    {
                        if (!_getMxComponent())
                        {
                            //LogClass.Instance.Log("Mx加载错误");

                            MessageBox.Show("初始化错误", "Initial MxComponent", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            //this.Close();
                            Application.Exit();
                            return;
                        }
                    }

                    break;
            }

            Application.AddMessageFilter(this);
        }
        void Initial()
        {
            BANNERFORM = new BannerForm(out Universal.ProBar);
            Universal.ProBar.Maximum = 8;
            BANNERFORM.Show();
            BANNERFORM.Refresh();

            if (isStart)
            {

                this.BackColor = SystemColors.Control;

                INI.Initial();

                Universal.LoadProgressBarValueADD();

                lblTotalStatus = label1;

                lblDebug = label2;
                lblDebug2 = label3;
                lblTestPath = label4;
                cboShowCam = comboBox1;
                btnCCDRESET = button1;
                lblConnectionFail = label5;
                lblDebug.Visible = false;
                lblDebug2.Visible = false;

                tabCtrl = tabControl1;
                pnlMappingUI = panel1;

#if (OPT_OVER_USE)
            switch (Universal.VERSION)
            {
                case VersionEnum.ALLINONE:

                    switch (Universal.OPTION)
                    {
                        case OptionEnum.MAIN:

                            lblJumboServer = new Label();
                            lblJumboServer.AutoSize = true;
                            lblJumboServer.BackColor = System.Drawing.Color.Black;
                            lblJumboServer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
                            lblJumboServer.ForeColor = System.Drawing.Color.Lime;
                            lblJumboServer.Location = new System.Drawing.Point(1, 25);
                            lblJumboServer.Name = "label5";
                            lblJumboServer.Size = new System.Drawing.Size(43, 14);
                            lblJumboServer.TabIndex = 13;
                            lblJumboServer.Text = "JumboServerOnline";
                            lblCrystalServer = new Label();
                            this.lblCrystalServer.AutoSize = true;
                            this.lblCrystalServer.BackColor = System.Drawing.Color.Black;
                            this.lblCrystalServer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
                            this.lblCrystalServer.ForeColor = System.Drawing.Color.Lime;
                            this.lblCrystalServer.Location = new System.Drawing.Point(1, 39);
                            this.lblCrystalServer.Name = "label6";
                            this.lblCrystalServer.Size = new System.Drawing.Size(43, 14);
                            this.lblCrystalServer.TabIndex = 14;
                            this.lblCrystalServer.Text = "CrystalServerOnline";

                            this.Controls.Add(this.lblJumboServer);
                            this.Controls.Add(this.lblCrystalServer);

                            //lblJumboServer.Visible = true;
                            //lblCrystalServer.Visible = true;

                            lblJumboServer.DoubleClick += LblJumboServer_DoubleClick;
                            lblCrystalServer.DoubleClick += LblCrystalServer_DoubleClick;
                            break;
                    }

                    break;
            }
#endif
                Universal.LoadProgressBarValueADD();
                //100MB Used
                bool bInitialOK = Universal.Initial(0);
                //ADD Gaara 檢查初始化是否完成
                if (!bInitialOK)
                {
                    JetEazy.LoggerClass.Instance.WriteLog("測試初始化錯誤...");
                    //MessageBox.Show("測試初始化錯誤。", "Initial", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    MessageBox.Show(Universal.InitialErrorString, "Initial Err" + Environment.NewLine + "始化錯誤 请检查CCD、DB等文件是否正常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //強制退出應用程式
                    Environment.Exit(0);
                }

                //鍵高機點位界面初始化
                Universal.JSPInitial();

                switch (OPTION)
                {
                    case OptionEnum.MAIN_SDM3:
                    case OptionEnum.MAIN_SDM2:
                    case OptionEnum.MAIN_SDM1:
                    case OptionEnum.MAIN_SD:
                    case OptionEnum.MAIN_X6:
                    case JetEazy.OptionEnum.MAIN_SERVICE:

                        this.dispUI1.Height = this.dispUI1.Height - 14;

                        //this.label1.BackColor = System.Drawing.Color.Yellow;
                        this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
                        this.label1.Location = new System.Drawing.Point(1, this.label1.Location.Y - 14);
                        //this.label1.Name = "label1";
                        this.label1.Size = new System.Drawing.Size(this.label1.Size.Width, this.label1.Size.Height + 14);
                        //this.label1.TabIndex = 5;
                        //this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

                        break;
                }



                InitialSTPUI();
                InitialESSUI();
                InitialRCPUI();
                InitialRUNUI();
                InitialCTRLUI();
                InitialDISPUI();
                InitialRESULT();

                Universal.LoadProgressBarValueADD();

                ESSUI.SetMainStatus(ESSStatusEnum.RUN);
                ESSUI.SetRecipeName(RCPDB.DataNow.ToESSString());

                IsLiveCapturing = INI.ISLIVECAPTURE;
                IsNeedToShowMover = true;

                MainTimer = new Timer();
                MainTimer.Interval = Universal.MainTimerInterval;
                MainTimer.Tick += MainTimer_Tick;
                MainTimer.Start();


                CCDCollection.iNextDuriation = INI.NextDuriation;

                // SetCamLight(AlbumNow, 0);
                SetCamLight(AlbumNow);

                GetStatus();

                switch (OPTION)
                {
                    case OptionEnum.MAIN_SDM2:
                    case OptionEnum.MAIN_SDM3:
                        //tabCtrl.Visible = true;
                        //tabCtrl.Width = CTRLUI.Width;
                        //tabCtrl.Location = new Point(CTRLUI.Location.X, CTRLUI.Location.Y - tabCtrl.Height - 5);

                        tabCtrl.Visible = true;
                        tabCtrl.Width = DISPUI.Width;
                        tabCtrl.Height = DISPUI.Height - lblTestPath.Height;
                        tabCtrl.Location = new Point(DISPUI.Location.X, DISPUI.Location.Y + lblTestPath.Height);

                        MappingUI = new MappingClass();

                        break;
                }

                switch(OPTION)
                {
                    case OptionEnum.MAIN_SDM2:
                    case OptionEnum.MAIN_SDM3:
                        this.StartPosition = FormStartPosition.Manual;
                        this.Location = new Point(0, 0);
                        this.FormBorderStyle = FormBorderStyle.Sizable;
                        this.ControlBox = true;
                        this.FormClosing += MainForm_FormClosing;
                        this.SizeChanged += MainForm_SizeChanged;
                        _updateUI();

                        LblDataRecordInit();

                        break;
                }

                //ADD GAARA
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
                this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
                this.Text = "Allinone.Control";

                //Should Disable When Very First Use Data Contruction
                TrainAlbum(-1, true);

                if (RUNUI != null)
                    RUNUI.MappingInit();



                //if(AlbumNow.ENVCount > 0)
                //    CCDCollection.SetExposure(AlbumNow.ENVList[0].GetCamString());

                if (INI.ISFOXCONNSF)
                {
                    if (System.IO.File.Exists(INI.SFPATHEXE))
                    {
                        Universal.Memory.OpenShareMemory("BYD");
                        System.Diagnostics.Process.Start(INI.SFPATHEXE);
                    }
                }
                Deletebackupfile(Universal.BACKUPDBPATH);

                if (INI.ISHIVECLIENT)
                {
                    if (System.IO.File.Exists(INI.HIVE_exe_path))
                    {
                        IntPtr hwnd = FindWindow(null, "JetEazy Hive 5.5");
                        //IntPtr hwnd = FindWindow(null, "JetEazy Hive" + JzHiveClass.HiveVersion);
                        if (hwnd == IntPtr.Zero)
                        {
                            System.Diagnostics.Process.Start(INI.HIVE_exe_path);
                        }
                    }
                }

                switch(Universal.OPTION)
                {
                    case JetEazy.OptionEnum.MAIN_SERVICE:
                    case OptionEnum.MAIN_X6:
                        string _viewer_path = "AJZReportViewer.exe";
                        if (System.IO.File.Exists(_viewer_path))
                        {
                            IntPtr hwnd = FindWindow(null, "JetEazy Viewer");
                            if (hwnd == IntPtr.Zero)
                            {
                                System.Diagnostics.Process.Start(_viewer_path);
                            }
                        }

                        break;
                }

                btnCCDRESET.Click += BtnCCDRESET_Click;
                lblConnectionFail.Click += LblConnectionFail_Click;

                //switch(Universal.MACHINECollection.MACHINE.mRobotType)
                //{
                //    case RobotType.NONE:
                        
                //        break;
                //}

                if (Universal.MACHINECollection.MACHINE.PLCCollection.Length > 0)
                    Universal.MACHINECollection.MACHINE.PLCCollection[0].ReadAction += MainForm_ReadAction;


                if (INI.ISONLYCHECKSN)
                    lblTotalStatus.BackColor = Color.Red;

                if (AlbumNow.ENVList.Count > 0)
                {
                    for (int i = 0; i < AlbumNow.ENVList[0].PageList.Count; i++)
                    {
                        if (!INI.CHECKPAGE[i] && lblTotalStatus.BackColor != Color.Red)
                            lblTotalStatus.BackColor = Color.Red;
                    }
                }

                if (INI.ISUSE_QFACTORY)
                {
                    Universal.RESULT.myResult.QFactorySend(Universal.JZQFACTORY, QFactoryErrorCode.Err_Normal);
                }





                Universal.R3TickStop += Universal_R3TickStop;

                BANNERFORM.Close();
                BANNERFORM.Dispose();
            }
            else
            {
                BANNERFORM.Close();
                BANNERFORM.Dispose();
                this.Close();
                return;
            }

            if (Universal.OPTION == OptionEnum.R5)
            {
                ControlSpace.MachineSpace.JzR5MachineClass jzR5 = (ControlSpace.MachineSpace.JzR5MachineClass)MACHINECollection.MACHINE;
                jzR5.PLCIO.STATR5 = true;

                //FormSpace.MessageFormFoUser message = new MessageFormFoUser();
                //message.Show();
                //message.TopMost = true;


                if (INI.isR5_MOTOR_TO_Rs485)
                {
                    Universal.mSetMotor = new MotoRs485.SetMotorForm();

                    Universal.mSetMotor.Initial(Universal.WORKPATH + "//Motor//", INI.iR5MOTORCOUNT, INI.R5RUNCOUNT, Universal.IsDebug);
                    Universal.mSetMotor.Show();
                    Universal.mSetMotor.Hide();
                }

                MACHINECollection.MACHINE.PLCCollection[0].SetData("D8", INI.R5RUNCOUNT, 1);
            }

            switch (OPTION)
            {
                case OptionEnum.MAIN_SDM2:
                case OptionEnum.MAIN_SDM1:
                case OptionEnum.MAIN_SDM3:
                    lblTestPath.DoubleClick += LblTestPath_DoubleClick;

                    LanguageExClass.Instance.EnumControls(this);
                    switch (Universal.CAMACT)
                    {
                        case CameraActionMode.CAM_MOTOR_MODE2:
                        case CameraActionMode.CAM_MOTOR:

                            int _count = AlbumNow.ENVList[0].PageList.Count;
                            CamActClass.Instance.SetStepCount(_count);

                            break;
                    }

                    break;

                case OptionEnum.MAIN_SD:
                    LanguageExClass.Instance.EnumControls(this);
                    break;
                case OptionEnum.MAIN_X6:

                    if (!Universal.IsNoUseIO)
                    {
                        ((ControlSpace.MachineSpace.JzMainX6MachineClass)MACHINECollection.MACHINE).PLCIO.Ready = false;
                        ((ControlSpace.MachineSpace.JzMainX6MachineClass)MACHINECollection.MACHINE).PLCIO.Busy = false;

                        ((ControlSpace.MachineSpace.JzMainX6MachineClass)MACHINECollection.MACHINE).PLCIO.TopLight = true;
                        ((ControlSpace.MachineSpace.JzMainX6MachineClass)MACHINECollection.MACHINE).PLCIO.FrontLight = true;
                        ((ControlSpace.MachineSpace.JzMainX6MachineClass)MACHINECollection.MACHINE).PLCIO.BackLight = true;
                    }

                    LanguageExClass.Instance.EnumControls(this);

                    //ClientSocket.Instance.TriggerAction += Instance_TriggerAction;
                    if (X6_HANDLE_CLIENT != null)
                    {
                        X6_HANDLE_CLIENT.TriggerAction += X6_HANDLE_CLIENT_TriggerAction;
                    }
                    if (X6_LASER_CLIENT != null)
                    {
                        X6_LASER_CLIENT.TriggerAction += X6_LASER_CLIENT_TriggerAction;
                    }

                    switch (Universal.CAMACT)
                    {
                        case CameraActionMode.CAM_MOTOR_LINESCAN:
                        case CameraActionMode.CAM_MOTOR_MODE2:
                        case CameraActionMode.CAM_MOTOR:

                            int _count = AlbumNow.ENVList[0].PageList.Count;
                            CamActClass.Instance.SetStepCount(_count);

                            break;
                    }


                    break;
            }

        }

        private void LblTestPath_DoubleClick(object sender, EventArgs e)
        {
            if (!m_IsAutoRunSMD2)
            {
                m_DirCount = RESULT.GetDirsCount();
                if (MessageBox.Show("继续运行，Index=" + m_DirIndex.ToString(), "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                {
                    m_DirIndex = 0;
                }
                m_IsAutoRunSMD2 = true;
                watchAutoTestTime.Restart();
            }
            else
            {
                m_IsAutoRunSMD2 = false;
                watchAutoTestTime.Stop();
            }
        }

        private void X6_LASER_CLIENT_TriggerAction(tcpItemData opstr)
        {
            if (m_tcpAction)
                return;

            tcpdata = opstr;
            m_tcpAction = true;
        }

        private void X6_HANDLE_CLIENT_TriggerAction(tcpItemData opstr)
        {
            if (m_tcpHandleAction)
                return;

            tcpHandledata = opstr;
            m_tcpHandleAction = true;
        }

        #region MAIN_X6

        bool m_tcpAction = false;
        //string _recipename = string.Empty;
        //tcpCmd _cmd = opstr.Cmd;
        tcpItemData tcpdata = null;
        string m_tcp_dataCheck = string.Empty;

        /// <summary>
        /// 多线程的测试
        /// </summary>
        private void tcp_TriggerAct()
        {
            switch (VERSION)
            {
                case VersionEnum.ALLINONE:

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_X6:

                            if (m_tcpAction)
                            {
                                m_tcpAction = false;
                                switch (tcpdata.Cmd)
                                {
                                    case tcpCmd.CMD_CHANGE:

                                        ESSStatusEnum _currentStatu = ESSUI.GetMainStatus();
                                        if (_currentStatu == ESSStatusEnum.RUN)
                                        {
                                            if (!Universal.RESULT.myResult.MainProcess.IsOn)
                                            {
                                                bool bOK = CheckAlbumCollection(tcpdata.RecipeName, false);
                                                GetStatus();
                                                RUNUI.MappingInit();
                                                if (bOK)
                                                {

                                                    //获取LOT号 来自于handler的参数+LOT
                                                    JzMainSDPositionParas.Report_LOT = tcpdata.LotName;
                                                    JzMainSDPositionParas.INSPECT_NGINDEX = 0;
                                                    JzMainSDPositionParas.SaveRecord();

                                                    X6_LASER_CLIENT.Log.Log2("tcpdata.RecipeName:" + tcpdata.RecipeName);
                                                    X6_LASER_CLIENT.Log.Log2("tcpdata.LotName:" + tcpdata.LotName);
                                                }

                                                X6_LASER_CLIENT.Log.Log2("tcpCmd.CMD_CHANGE" + (bOK ? "成功" : "失败"));
                                                X6_LASER_CLIENT.Send(tcpdata.CmdStr + (bOK ? "0001" : "0003"));// 0001 切换成功 0003 切换失败
                                            }
                                            else
                                            {
                                                X6_LASER_CLIENT.Send(tcpdata.CmdStr + "0004");//测试中
                                            }
                                        }
                                        else
                                        {
                                            X6_LASER_CLIENT.Send(tcpdata.CmdStr + "0005");//不在跑线状态
                                        }

                                        break;
                                    case tcpCmd.CMD_QCBYPASS:

                                        _currentStatu = ESSUI.GetMainStatus();
                                        if (_currentStatu == ESSStatusEnum.RUN)
                                        {
                                            if (!Universal.RESULT.myResult.MainProcess.IsOn)
                                            {
                                                if (RUNUI != null)
                                                {
                                                    int iret = RUNUI.SetByPass(tcpdata.QcByPass, ref m_tcp_dataCheck);
                                                    if (iret == 0)
                                                    {
                                                        X6_LASER_CLIENT.Log.Log2("tcpHandledata.Qc2ddata" + tcpHandledata.Qc2ddata);

                                                    }
                                                    X6_LASER_CLIENT.Send(tcpdata.CmdStr + (iret == 0 ? "0001" : "0003"));// 0001 切换成功 0003 切换失败
                                                }
                                                else
                                                {
                                                    X6_LASER_CLIENT.Send(tcpdata.CmdStr + "0003");// 切换失败
                                                }
                                            }
                                            else
                                            {
                                                X6_LASER_CLIENT.Send(tcpdata.CmdStr + "0004");//测试中
                                            }
                                        }
                                        else
                                        {
                                            X6_LASER_CLIENT.Send(tcpdata.CmdStr + "0005");//不在跑线状态
                                        }

                                        break;
                                    //case tcpCmd.CMD_QC2DDATA:
                                    //    _currentStatu = ESSUI.GetMainStatus();
                                    //    if (_currentStatu == ESSStatusEnum.RUN)
                                    //    {
                                    //        ClientSocket.Instance.Send(tcpdata.CmdStr + "0001");//模拟成功返回
                                    //        //if (!Universal.RESULT.myResult.MainProcess.IsOn)
                                    //        //{
                                    //        //    if (RUNUI != null)
                                    //        //    {
                                    //        //        int iret = RUNUI.SetByPass(tcpdata.QcByPass);
                                    //        //        ClientSocket.Instance.Send(tcpdata.CmdStr + (iret == 0 ? "0001" : "0003"));// 0001 切换成功 0003 切换失败
                                    //        //    }
                                    //        //    else
                                    //        //    {
                                    //        //        ClientSocket.Instance.Send(tcpdata.CmdStr + "0003");// 切换失败
                                    //        //    }
                                    //        //}
                                    //        //else
                                    //        //{
                                    //        //    ClientSocket.Instance.Send(tcpdata.CmdStr + "0004");//测试中
                                    //        //}
                                    //    }
                                    //    else
                                    //    {
                                    //        ClientSocket.Instance.Send(tcpdata.CmdStr + "0005");//不在跑线状态
                                    //    }
                                    //    break;
                                    default:

                                        X6_LASER_CLIENT.Log.Log2("tcpCmd.NONE" + " 无效指令。");
                                        //ClientSocket.Instance.Send("0002");// 0002 无法识别的指令

                                        break;
                                }
                            }

                            break;
                    }

                    break;
            }
        }

        bool m_tcpHandleAction = false;
        //string _recipename = string.Empty;
        //tcpCmd _cmd = opstr.Cmd;
        tcpItemData tcpHandledata = null;


        /// <summary>
        /// 多线程的测试
        /// </summary>
        private void tcp_HandleTriggerAct()
        {
            switch (VERSION)
            {
                case VersionEnum.ALLINONE:

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_X6:

                            if (m_tcpHandleAction)
                            {
                                m_tcpHandleAction = false;
                                switch (tcpHandledata.Cmd)
                                {
                                    case tcpCmd.CMD_QC2DBARCODE:
                                        ESSStatusEnum _currentStatu = ESSUI.GetMainStatus();
                                        if (_currentStatu == ESSStatusEnum.RUN)
                                        {
                                            if (!Universal.RESULT.myResult.MainProcess.IsOn)
                                            {
                                                if (RUNUI != null)
                                                {
                                                    int iret = RUNUI.SetCheckBarcode(tcpHandledata.QC2dbarcode, ref m_tcp_dataCheck);
                                                    if (iret == 0)
                                                    {
                                                        X6_HANDLE_CLIENT.Log.Log2("tcpHandledata.QC2dbarcode" + tcpHandledata.Qc2ddata);

                                                    }
                                                    X6_HANDLE_CLIENT.Log.Log2("tcpCmd.CMD_QC2DBARCODE" + " return=" + iret.ToString() + " " + m_tcp_dataCheck);
                                                    byte[] bytedata = new byte[36];
                                                    bytedata[0] = 27;
                                                    bytedata[4] = 4;
                                                    bytedata[8] = 0;
                                                    bytedata[32] = (iret == 0 ? (byte)1 : (byte)3);
                                                    X6_HANDLE_CLIENT.Send(bytedata);
                                                    //X6_HANDLE_CLIENT.Send(tcpHandledata.CmdStr + (iret == 0 ? "0001" : "0003"));// 0001 切换成功 0003 切换失败
                                                }
                                                else
                                                {
                                                    byte[] bytedata = new byte[36];
                                                    bytedata[0] = 27;
                                                    bytedata[4] = 4;
                                                    bytedata[8] = 0;
                                                    bytedata[32] = (byte)3;
                                                    X6_HANDLE_CLIENT.Send(bytedata);
                                                    //X6_HANDLE_CLIENT.Send(tcpHandledata.CmdStr + "0003");// 切换失败
                                                }
                                            }
                                            else
                                            {
                                                byte[] bytedata = new byte[36];
                                                bytedata[0] = 27;
                                                bytedata[4] = 4;
                                                bytedata[8] = 0;
                                                bytedata[32] = (byte)4;
                                                X6_HANDLE_CLIENT.Send(bytedata);
                                                //X6_HANDLE_CLIENT.Send(tcpHandledata.CmdStr + "0004");//测试中
                                            }
                                        }
                                        else
                                        {
                                            byte[] bytedata = new byte[36];
                                            bytedata[0] = 27;
                                            bytedata[4] = 4;
                                            bytedata[8] = 0;
                                            bytedata[32] = (byte)5;
                                            X6_HANDLE_CLIENT.Send(bytedata);
                                            //X6_HANDLE_CLIENT.Send(tcpHandledata.CmdStr + "0005");//不在跑线状态
                                        }
                                        break;
                                    case tcpCmd.CMD_QC2DDATA:
                                        _currentStatu = ESSUI.GetMainStatus();
                                        if (_currentStatu == ESSStatusEnum.RUN)
                                        {
                                            if (!Universal.RESULT.myResult.MainProcess.IsOn)
                                            {
                                                if (RUNUI != null)
                                                {
                                                    int iret = RUNUI.SetByPass(tcpHandledata.QcByPass, ref m_tcp_dataCheck);
                                                    if (iret == 0)
                                                    {
                                                        X6_HANDLE_CLIENT.Log.Log2("tcpHandledata.Qc2ddata" + tcpHandledata.Qc2ddata);

                                                    }
                                                    X6_HANDLE_CLIENT.Log.Log2("tcpCmd.CMD_QC2DDATA" + " return=" + iret.ToString() + " " + m_tcp_dataCheck);
                                                    byte[] bytedata = new byte[36];
                                                    bytedata[0] = 24;
                                                    bytedata[4] = 4;
                                                    bytedata[8] = 0;
                                                    bytedata[32] = (iret == 0 ? (byte)1 : (byte)3);
                                                    X6_HANDLE_CLIENT.Send(bytedata);
                                                    //X6_HANDLE_CLIENT.Send(tcpHandledata.CmdStr + (iret == 0 ? "0001" : "0003"));// 0001 切换成功 0003 切换失败
                                                }
                                                else
                                                {
                                                    byte[] bytedata = new byte[36];
                                                    bytedata[0] = 24;
                                                    bytedata[4] = 4;
                                                    bytedata[8] = 0;
                                                    bytedata[32] = (byte)3;
                                                    X6_HANDLE_CLIENT.Send(bytedata);
                                                    //X6_HANDLE_CLIENT.Send(tcpHandledata.CmdStr + "0003");// 切换失败
                                                }
                                            }
                                            else
                                            {
                                                byte[] bytedata = new byte[36];
                                                bytedata[0] = 24;
                                                bytedata[4] = 4;
                                                bytedata[8] = 0;
                                                bytedata[32] = (byte)4;
                                                X6_HANDLE_CLIENT.Send(bytedata);
                                                //X6_HANDLE_CLIENT.Send(tcpHandledata.CmdStr + "0004");//测试中
                                            }
                                        }
                                        else
                                        {
                                            byte[] bytedata = new byte[36];
                                            bytedata[0] = 24;
                                            bytedata[4] = 4;
                                            bytedata[8] = 0;
                                            bytedata[32] = (byte)5;
                                            X6_HANDLE_CLIENT.Send(bytedata);
                                            //X6_HANDLE_CLIENT.Send(tcpHandledata.CmdStr + "0005");//不在跑线状态
                                        }
                                        break;
                                    case tcpCmd.CMD_QCCHANGE_MODEL:
                                        _currentStatu = ESSUI.GetMainStatus();
                                        if (_currentStatu == ESSStatusEnum.RUN)
                                        {
                                            if (!Universal.RESULT.myResult.MainProcess.IsOn)
                                            {
                                                int iret = _changeModelBackgroudImage();
                                                X6_HANDLE_CLIENT.Log.Log2("tcpCmd.CMD_QCCHANGE_MODEL" + " return=" + iret.ToString());
                                                byte[] bytedata = new byte[36];
                                                bytedata[0] = 25;
                                                bytedata[4] = 4;
                                                bytedata[8] = 0;
                                                bytedata[32] = (iret == 0 ? (byte)1 : (byte)3);
                                                X6_HANDLE_CLIENT.Send(bytedata);

                                                //if (RUNUI != null)
                                                //{

                                                //    //X6_HANDLE_CLIENT.Send(tcpHandledata.CmdStr + (iret == 0 ? "0001" : "0003"));// 0001 切换成功 0003 切换失败
                                                //}
                                                //else
                                                //{
                                                //    byte[] bytedata = new byte[36];
                                                //    bytedata[0] = 24;
                                                //    bytedata[4] = 4;
                                                //    bytedata[8] = 0;
                                                //    bytedata[32] = (byte)3;
                                                //    X6_HANDLE_CLIENT.Send(bytedata);
                                                //    //X6_HANDLE_CLIENT.Send(tcpHandledata.CmdStr + "0003");// 切换失败
                                                //}
                                            }
                                            else
                                            {
                                                byte[] bytedata = new byte[36];
                                                bytedata[0] = 25;
                                                bytedata[4] = 4;
                                                bytedata[8] = 0;
                                                bytedata[32] = (byte)4;
                                                X6_HANDLE_CLIENT.Send(bytedata);
                                                //X6_HANDLE_CLIENT.Send(tcpHandledata.CmdStr + "0004");//测试中
                                            }
                                        }
                                        else
                                        {
                                            byte[] bytedata = new byte[36];
                                            bytedata[0] = 25;
                                            bytedata[4] = 4;
                                            bytedata[8] = 0;
                                            bytedata[32] = (byte)5;
                                            X6_HANDLE_CLIENT.Send(bytedata);
                                            //X6_HANDLE_CLIENT.Send(tcpHandledata.CmdStr + "0005");//不在跑线状态
                                        }
                                        break;
                                    default:
                                        X6_HANDLE_CLIENT.Log.Log2("tcpCmd.NONE" + " 无效指令。");
                                        break;
                                }
                            }

                            break;
                    }

                    break;
            }
        }

        #endregion


        private void Universal_R3TickStop(string movestring)
        {
            if (Universal.OPTION == OptionEnum.R3 || Universal.OPTION == OptionEnum.C3)
            {
                if (INI.ISONLYCHECKSN || movestring == "1")
                    lblTotalStatus.BackColor = Color.Red;

                if (!INI.ISONLYCHECKSN && movestring == "2")
                {
                    lblTotalStatus.BackColor = Color.Yellow;
                    Universal.isR3ByPass = false;
                }
            }
            else
            {
                if (movestring == "1")
                    lblTotalStatus.BackColor = Color.Red;

                if (movestring == "2")
                {
                    lblTotalStatus.BackColor = Color.Yellow;
                    Universal.isR3ByPass = false;
                }
            }
        }

        /// <summary>
        /// PLC缓冲区接收到数据(此处为让主画面上显示PLC连线状态)
        /// </summary>
        /// <param name="readbuffer"></param>
        /// <param name="operationstring"></param>
        /// <param name="myname"></param>
        private void MainForm_ReadAction(char[] readbuffer, string operationstring, string myname)
        {
            lblConnectionFail.BeginInvoke(new EventHandler(delegate
            {
                if (lblConnectionFail.BackColor != Color.Lime)
                    lblConnectionFail.BackColor = Color.Lime;
                if (lblConnectionFail.ForeColor != Color.Black)
                    lblConnectionFail.ForeColor = Color.Black;

                if (lblConnectionFail.IsAccessible)
                {
                    lblConnectionFail.Font = new System.Drawing.Font("Cambria", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

                    if (iPLCStatus == 1)
                        lblConnectionFail.Text = "➀";
                    else if (iPLCStatus == 2)
                        lblConnectionFail.Text = "➁";
                    else if (iPLCStatus == 3)
                        lblConnectionFail.Text = "➂";
                    else if (iPLCStatus == 4)
                        lblConnectionFail.Text = "➃";
                    else if (iPLCStatus == 5)
                        lblConnectionFail.Text = "➄";
                    else if (iPLCStatus == 6)
                        lblConnectionFail.Text = "➅";
                    else if (iPLCStatus == 7)
                        lblConnectionFail.Text = "➆";
                    else if (iPLCStatus == 8)
                        lblConnectionFail.Text = "➇";
                    else if (iPLCStatus == 9)
                        lblConnectionFail.Text = "➈";

                    iPLCStatus++;
                    if (iPLCStatus > 9)
                        iPLCStatus = 1;
                }
                else
                {
                    lblConnectionFail.Font = new System.Drawing.Font("Cambria", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    lblConnectionFail.Text = MACHINECollection.MACHINE.PLCCollection[0].iCount.ToString();
                }
                //lblConnectionFail.Refresh();
            }));
        }

        private void LblConnectionFail_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("您需要重新连接PLC或机械臂吗?", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Universal.MACHINECollection.RetryConnect();
            }
            lblConnectionFail.IsAccessible = !lblConnectionFail.IsAccessible;
            //if (Universal.MACHINECollection.MACHINE.PLCCollection.Length > 0)
            //{
            //    if (Universal.MACHINECollection.MACHINE.PLCCollection[0].IsConnectionFail)
            //    {
            //        if (MessageBox.Show("您需要重新连接PLC吗?", "COM", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //        {
            //            Universal.MACHINECollection.MACHINE.PLCCollection[0].RetryConn();

            //            if (INI.ISHIVECLIENT)
            //            {
            //                Universal.JZHIVECLIENT.Hiveclient_ErrorData("PLC Connect Error", "1001", plcerrorStart, DateTime.Now);
            //            }
            //            if (INI.ISUSE_QFACTORY)
            //            {
            //                Universal.RESULT.myResult.QFactorySend(Universal.JZQFACTORY, QFactoryErrorCode.Err_Normal);
            //            }
            //        }
            //    }
            //    else
            //        lblConnectionFail.IsAccessible = !lblConnectionFail.IsAccessible;

            //}

        }

        private void BtnCCDRESET_Click(object sender, EventArgs e)
        {
            string strmess = "您是否要重置相机? " + Environment.NewLine + "Sure to reset CCD?";
            JetEazy.FormSpace.MessageForm form = new MessageForm(true, strmess);
            form.Refresh();
            if (form.ShowDialog() == DialogResult.Yes)
            {

                strmess = "重置中请稍候! " + Environment.NewLine + " During resetting, please wait";
                JetEazy.FormSpace.MessageForm form2 = new MessageForm(false, strmess);
                form2.Show();
                form2.Refresh();

                CCDCollection.Close();
                CCDCollection.Initial(Universal.WORKPATH);

                if (INI.ISHIVECLIENT)
                {
                    Universal.JZHIVECLIENT.Hiveclient_ErrorData("CCD Connect Error", "1000", plcerrorStart, DateTime.Now);
                    CCDCollection.Reset();

                }
                if (INI.ISUSE_QFACTORY)
                {
                    Universal.RESULT.myResult.QFactorySend(Universal.JZQFACTORY, QFactoryErrorCode.Err_Normal);
                }

                form2.Close();
                GC.Collect();
            }
        }

        private void LblCrystalServer_DoubleClick(object sender, EventArgs e)
        {
            //if(IsCrystalOnline)
            //    RESULT.ReStartCrystalBroadCast();
        }

        private void LblJumboServer_DoubleClick(object sender, EventArgs e)
        {
            //if (IsJumboOnline)
            //    RESULT.ReStartJumboBroadCast();
        }


        //System.Diagnostics.Stopwatch stopwatch1 = new System.Diagnostics.Stopwatch();
        //System.Diagnostics.Stopwatch stopwatch2 = new System.Diagnostics.Stopwatch();
        //string MessSave = "";
        private void MainTimer_Tick(object sender, EventArgs e)
        {
            JzScanTimeMS = JzMainScanTime.msDuriation;
            JzMainScanTime.Cut();

            //if (JzMainScanTime.msDuriation >= 100)
            CTRLUI_TriggerAct();

            //try
            {
                tcp_TriggerAct();
                tcp_HandleTriggerAct();


                CTRLUI.Tick();
                ESSUI.Tick();
                //savelog(2);

                RESULT.Tick();

                //savelog(3);

                switch (LAYOUT)
                {
                    case LayoutEnum.L1280X800:
                        RUNV1UI.Tick();
                        break;
                    case LayoutEnum.L1440X900:
                        RUNUI.Tick();
                        break;
                }
                //savelog(4);

                #region ADD GAARA 這個是檢查SERVER是否掉線及重連機制
#if (OPT_OVER_USE)
            switch (Universal.VERSION)
            {
                case VersionEnum.ALLINONE:

                    switch (Universal.OPTION)
                    {
                        case OptionEnum.MAIN:

                            if(INI.ISUSEJUMBOSERVER)
                            {
                                try
                                {
                                    //IsJumboOnline = Universal.IXCONNECTCRYSTAL.IsOnline;
                                    if (IsJumboOnline)
                                    {
                                        lblJumboServer.Text = "JumboServerOnline";
                                        lblJumboServer.ForeColor = Color.Lime;

                                        if(!IsJumboOnlineX)
                                        {
                                            IsJumboOnlineX = true;
                                            RESULT.ReStartJumboBroadCast();
                                        }
                                    }
                                    else
                                    {
                                        IsJumboOnlineX = false;
                                        lblJumboServer.Text = "JumboServerOffline-1";
                                        lblJumboServer.ForeColor = Color.Red;
                                    }
                                }
                                catch
                                { JetEazy.LoggerClass.Instance.WriteException(ex);
                                    IsJumboOnlineX = false;
                                    lblJumboServer.Text = "JumboServerOffline-2";
                                    lblJumboServer.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                lblJumboServer.Text = "JumboServer No Use";
                                lblJumboServer.ForeColor = Color.Red;
                            }
                            if(INI.ISUSECRYSTALSERVER)
                            {
                                try
                                {
                                    IsCrystalOnline = Universal.IXCONNECTCRYSTAL.IsOnline;
                                    if (IsCrystalOnline)
                                    {
                                        lblCrystalServer.Text = "CrystalServerOnline";
                                        lblCrystalServer.ForeColor = Color.Lime;

                                        if(!IsCrystalOnlineX)
                                        {
                                            IsCrystalOnlineX = true;
                                            RESULT.ReStartCrystalBroadCast();
                                        }
                                    }
                                    else
                                    {
                                        IsCrystalOnlineX = false;
                                        lblCrystalServer.Text = "CrystalServerOffline-1";
                                        lblCrystalServer.ForeColor = Color.Red;
                                    }
                                }
                                catch
                                { JetEazy.LoggerClass.Instance.WriteException(ex);
                                    IsCrystalOnlineX = false;
                                    lblCrystalServer.Text = "CrystalServerOffline-2";
                                    lblCrystalServer.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                lblCrystalServer.Text = "CrystalServer No Use";
                                lblCrystalServer.ForeColor = Color.Red;
                            }

                            break;
                    }

                    break;
            }
#endif
                #endregion

                //savelog(5);



                //savelog(6);
                switch (VERSION)
                {
                    case VersionEnum.ALLINONE:
                        switch (OPTION)
                        {
                            case OptionEnum.MAIN_SDM3:
                            case OptionEnum.MAIN_SDM2:
                                _recoredDataSevenPoint();
                                _getAutoMainSDM2Test();
                                break;
                            case JetEazy.OptionEnum.MAIN_SERVICE:
                            case OptionEnum.MAIN_X6:
                                break;
                            default:
                                if (myloadwatch.ElapsedMilliseconds > 50000)
                                {
                                    isLoadOK = true;
                                    myloadwatch.Stop();
                                }
                                if (myTickCheckwatch.ElapsedMilliseconds > 60000)
                                {
                                    bool ischenger = INI.CheckBCData();
                                    if (ischenger)
                                        ESSUI.FillBCCOUNT(INI.ALLCOUNT, INI.BCNGCOUNT);

                                    myTickCheckwatch.Restart();
                                }
                                break;
                        }
                        break;
                }

                //savelog(7);

                LiveCaptureTick();

                //savelog(8);

            }
            //catch (Exception ex)
            //{
            //    JetEazy.LoggerClass.Instance.WriteException(ex);
            //}

            //if(stopwatch2.ElapsedMilliseconds>500)
            //{
            //    string strMess = Environment.NewLine;
            //    strMess += stopwatch2.ElapsedMilliseconds+ Environment.NewLine;

            //    MessSave = strMess + MessSave;
            //    System.IO.File.AppendAllText("D:\\Log\\test.log", MessSave);
            //}
            //savelog(1);

            switch (Universal.OPTION)
            {
                case JetEazy.OptionEnum.MAIN_SERVICE:
                case OptionEnum.MAIN_X6:
                    ESSUI.ShowPLC_RxTime(Universal.VersionDate + "_" + Universal.OPTION.ToString() + " " + JzScanTimeMS.ToString() + "ms");

                    switch (Universal.CAMACT)
                    {
                        case CameraActionMode.CAM_MOTOR:
                        case CameraActionMode.CAM_MOTOR_MODE2:




                            break;
                    }

                    break;
                case OptionEnum.MAIN_SDM3:
                case OptionEnum.MAIN_SDM2:
                case OptionEnum.MAIN_SDM1:
                case OptionEnum.MAIN_SD:
                    ESSUI.ShowPLC_RxTime(Universal.VersionDate + "_" + 
                        Universal.OPTION.ToString() + " " + JzScanTimeMS.ToString() + "ms "
                        + MACHINECollection.GetFps());
                    break;
                default:
                    //ESSUI.ShowPLC_RxTime(Universal.VersionDate + "_" + Universal.OPTION.ToString() + " " + JzHiveClass.HiveVersion);
                    break;
            }


        }



        //void savelog(int ID,string Mess="")
        //{
        //long tim = stopwatch1.ElapsedMilliseconds;
        //stopwatch1.Restart();

        //MessSave += ID.ToString("0000") + "," + tim.ToString() + "," + Mess + Environment.NewLine;

        //IEnumerable<string> m_oEnum = new List<string>() { ID.ToString("0000"), tim.ToString(), Mess };
        //System.IO. File.AppendAllLines( "D:\\Log\\20210819\\test.log", m_oEnum);
        //}
        void LiveCaptureTick()
        {
            CCDCollection.Tick();
            //savelog(9);
            //   label6.Text = CCDClass.Exposure;

            if (!Universal.IsNoUseCCD)
            {
                if (INI.ISHIVECLIENT || INI.ISUSE_QFACTORY)
                {
                    if (CCDCollection.IsCCDError && isLoadOK)
                    {
                        if (!IsPlcWarningShow)
                        {
                            plcerrorStart = DateTime.Now;
                            IsPlcWarningShow = true;

                            if (INI.ISHIVECLIENT)
                            {
                                SetMachineState(MachineState.Error);
                                Universal.JZHIVECLIENT.Hiveclient_MachineState_Errmsg("CCD Connect Error", "1000", plcerrorStart);
                            }
                            if (INI.ISUSE_QFACTORY)
                            {
                                Universal.RESULT.myResult.QFactorySend(Universal.JZQFACTORY, QFactoryErrorCode.Err_Abnormal_Cam);
                            }

                            xFrmWarning = new MessageForm(true, "CCD Connect Error");
                            xFrmWarning.Show();

                            JzToolsClass jzTools2 = new JzToolsClass();
                            string strmess = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            strmess += " CCD Connect Error" + Environment.NewLine;
                            jzTools2.AppendData(strmess, Universal.MAINPATH + @"\Err.log");
                        }
                    }
                    else
                    {
                        if (GetMachineState == MachineState.Error)
                        {
                            if (MACHINECollection.MACHINE.PLCCollection.Length > 0)
                            {
                                if (!MACHINECollection.MACHINE.PLCCollection[0].IsConnectionFail)
                                {
                                    SetMachineState(MachineState.Idle);
                                }
                            }
                        }
                    }
                }
            }
            //savelog(10);

            if (!Universal.IsDebug)
            {
                if (INI.ISUSE_QFACTORY)//每隔多少时间 发一次状态
                {
                    if (JzQFactoryUpload.minDuriation >= INI.QFACTORY_CHECK_TIME)
                    {
                        Universal.RESULT.myResult.QFactorySend(Universal.JZQFACTORY, QFactoryErrorCode.Err_Normal);
                        JzQFactoryUpload.Cut();
                    }
                }
            }

            //savelog(11);

            if (!IsLiveCapturing)
                return;

            if (!Universal.IsNoUseIO)
            {
                if (MACHINECollection.MACHINE.PLCCollection.Length > 0)
                {
                    if (MACHINECollection.MACHINE.PLCCollection[0].IsConnectionFail)
                    {
                        lblConnectionFail.Font = new System.Drawing.Font("Cambria", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        lblConnectionFail.Text = "○";// "●";
                        lblConnectionFail.BackColor = Color.Red;
                        lblConnectionFail.ForeColor = Color.Yellow;

                        if (INI.ISHIVECLIENT || INI.ISUSE_QFACTORY)
                        {
                            if (!IsPlcWarningShow)
                            {

                                plcerrorStart = DateTime.Now;
                                IsPlcWarningShow = true;
                                if (INI.ISHIVECLIENT)
                                {
                                    SetMachineState(MachineState.Error);
                                    Universal.JZHIVECLIENT.Hiveclient_MachineState_Errmsg("PLC Connect Error", "1001", plcerrorStart);
                                }
                                if (INI.ISUSE_QFACTORY)
                                {
                                    Universal.RESULT.myResult.QFactorySend(Universal.JZQFACTORY, QFactoryErrorCode.Err_Abnormal_Plc_Disconnect);
                                }

                                xFrmWarning = new MessageForm(true, "PLC Connect Error");
                                xFrmWarning.Show();

                                JzToolsClass jzTools2 = new JzToolsClass();
                                string strmess = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                strmess += " PLC Connect Error" + Environment.NewLine;
                                jzTools2.AppendData(strmess, Universal.MAINPATH + @"\Err.log");
                            }
                        }
                    }
                    else
                    {
                        if (GetMachineState == MachineState.Error)
                        {
                            if (!Universal.IsNoUseCCD)
                            {
                                if (!CCDCollection.IsCCDError)
                                {
                                    SetMachineState(MachineState.Idle);
                                }
                            }
                        }
                    }
                }
            }

            //savelog(812);
            //This is For Work View
            CCDCollection.SetDebugPath(Universal.WORKPATH);

            //savelog(13);

            CCDCollection.SetDebugEnvPath("");

            //savelog(14);


            if (MainTime.msDuriation > Universal.DISPLAYTICK)
            {
                if (!RESULT.myResult.MainProcess.IsOn && !DISPUI.ISMOUSEDOWN)
                {
                    if (cboShowCam.Text == "ALL")
                    {
                        CCDCollection.GetBmpSeq();
                        DISPUI.ReplaceDisplayImage(CCDCollection.bmpAll);

                        if (IsChangeShowCam)
                        {
                            DISPUI.SetDisplayImage();
                            IsChangeShowCam = false;
                        }

                        DISPUIShowMover();
                    }
                    else
                    {
                        int camno = int.Parse(cboShowCam.Text.Substring(cboShowCam.Text.Length - 2, 2));

                        DISPUI.ReplaceDisplayImage(CCDCollection.GetBMP(camno, true));

                        if (IsChangeShowCam)
                        {
                            DISPUI.SetDisplayImage();
                            IsChangeShowCam = false;
                        }

                        DISPUIShowMover();
                    }

                }
                MainTime.Cut();
            }

            //savelog(15);
        }
        void DISPUIShowCurrentMover(int index = 0)
        {
            DISPUI.ClearMover();
            if (cboShowCam.Text != "ALL")
            {
                ShowMover.Clear();
                return;
            }

            AlbumNow.FillEnvMover(ShowMover, CCDCollection, index);
            DISPUI.SetMover(ShowMover);
            DISPUI.RefreshDisplayShape();
        }
        /// <summary>
        /// 將 Mover 畫在螢幕上
        /// </summary>
        void DISPUIShowMover()
        {
            if (IsNeedToShowMover)
            {
                DISPUI.ClearMover();

                if (cboShowCam.Text != "ALL")
                {
                    ShowMover.Clear();
                    return;
                }

                switch (VERSION)
                {
                    case VersionEnum.ALLINONE:
                        switch (OPTION)
                        {
                            default:
                                AlbumNow.FillFirstEnvMover(ShowMover, CCDCollection);
                                break;
                        }


                        break;
                    case VersionEnum.AUDIX:
                        switch (OPTION)
                        {
                            case OptionEnum.MAIN:
                                AlbumNow.FillFirstEnvMover(ShowMover, CCDCollection);
                                break;
                            case OptionEnum.MAIN_DFLY:

                                break;
                        }
                        break;
                }

                DISPUI.SetMover(ShowMover);
                DISPUI.RefreshDisplayShape();

                IsNeedToShowMover = false;
            }
        }
        void DISPUIShowResult()
        {
            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                case VersionEnum.AUDIX:

                    DISPUI.ClearMover();

                    //AlbumWork.FillFirstEnvResultMover(ShowMover, CCDCollection);
                    AlbumWork.FillCompoundMover(ShowMover);

                    DISPUI.SetMover(ShowMover);
                    DISPUI.RefreshDisplayShape();

                    //DISPUI.GetOrgBMP().Save(@"D:\LOA\TEST.PNG");
                    //DISPUI.SaveScreen();

                    break;
            }
        }
        void InitialESSUI()
        {
            ESSUI = essUI1;
            ESSUI.Initial(ESSDB, ACCDB, Universal.UIPATH, INI.LANGUAGE, VERSION, OPTION, 500, Universal.VersionDate + JzHiveClass.HiveVersion + "_" + Universal.OPTION);

            if (INI.isAccounFingerprint)
            {
                // 初始化
                JetEazy.DB.CmUserAccountMgr.Attach(
                  "MYDB",              //  DB_FILE_NAME,
                  ESSUI.GetAccountName,//  lblUserName,
                  new Button(),        //  ESSUI.GetbtnLogin,//   btnLogin,
                  new Button()       // ESSUI.GetbtnAccountManagement //   btnAccMgr
                );
            }

            ESSUI.TriggerAction += ESSUI_TriggerAction;

        }
        void InitialRCPUI()
        {
            //RCPUI = rcpUI1;

            switch (LAYOUT)
            {
                case LayoutEnum.L1280X800:
                    RCPV1UI = new RcpV1UI();
                    this.Controls.Add(RCPV1UI);
                    RCPV1UI.Location = new Point(1053, 289);

                    RCPV1UI.Initial(Universal.UIPATH, INI.LANGUAGE, VERSION, OPTION, RCPDB, AlbumCollection, Universal.SHOWBMPSTRING);
                    RCPV1UI.TriggerAction += RCPUI_TriggerAction;

                    break;
                case LayoutEnum.L1440X900:

                    RCPUI = new RcpUI();
                    this.Controls.Add(RCPUI);
                    RCPUI.Location = new Point(228, 596);
                    RCPUI.Initial(Universal.UIPATH, INI.LANGUAGE, VERSION, OPTION, RCPDB, AlbumCollection, Universal.SHOWBMPSTRING);
                    RCPUI.TriggerAction += RCPUI_TriggerAction;

                    break;
                default:

                    RCPUI = new RcpUI();
                    this.Controls.Add(RCPUI);
                    RCPUI.Location = new Point(228, 596);
                    RCPUI.Initial(Universal.UIPATH, INI.LANGUAGE, VERSION, OPTION, RCPDB, AlbumCollection, Universal.SHOWBMPSTRING);
                    RCPUI.TriggerAction += RCPUI_TriggerAction;

                    break;
            }



        }
        void InitialSTPUI()
        {
            switch (LAYOUT)
            {
                case LayoutEnum.L1280X800:
                    STPV1UI = new StpV1UI();
                    this.Controls.Add(STPV1UI);
                    STPV1UI.Location = new Point(1053, 289);

                    STPV1UI.Initial(VERSION, OPTION, CCDCollection.bmpBackGround.Width > 1);

                    STPV1UI.TriggerAction += STPUI_TriggerAction;
                    STPV1UI.TriggerMoveScreen += STPUI_TriggerMoveScreen;


                    break;
                case LayoutEnum.L1440X900:
                    STPUI = new StpUI();
                    this.Controls.Add(STPUI);
                    STPUI.Location = new Point(228, 596);

                    STPUI.Initial(VERSION, OPTION, CCDCollection.CCDRectRelateIndexList, CCDCollection.bmpBackGround.Width > 1);

                    STPUI.TriggerAction += STPUI_TriggerAction;
                    STPUI.TriggerMoveScreen += STPUI_TriggerMoveScreen;
                    break;
                default:
                    STPUI = new StpUI();
                    this.Controls.Add(STPUI);
                    STPUI.Location = new Point(228, 596);

                    STPUI.Initial(VERSION, OPTION, CCDCollection.CCDRectRelateIndexList, CCDCollection.bmpBackGround.Width > 1);

                    STPUI.TriggerAction += STPUI_TriggerAction;
                    STPUI.TriggerMoveScreen += STPUI_TriggerMoveScreen;
                    break;
            }




        }
        void InitialRUNUI()
        {
            switch (LAYOUT)
            {
                case LayoutEnum.L1280X800:
                    RUNV1UI = new RunV1UI();
                    this.Controls.Add(RUNV1UI);

                    RUNV1UI.Initial();
                    RUNV1UI.Location = new Point(1053, 289);
                    break;
                case LayoutEnum.L1440X900:
                    RUNUI = new RunUI();
                    this.Controls.Add(RUNUI);

                    RUNUI.Initial();
                    RUNUI.Location = new Point(228, 596);

                    switch (VERSION)
                    {
                        case VersionEnum.ALLINONE:
                        case VersionEnum.AUDIX:

                            RUNUI.SetShopFloorVisible = false;
                            RUNUI.SetUserDataVisible = true;
                            RUNUI.SetReportUIVisible = true;

                            break;
                    }

                    RUNUI.TriggerAction += RUNUI_TriggerAction;
                    RUNUI.LearnAction += RUNUI_LearnAction;
                    RUNUI.GrpEnabled = false;
                    break;
            }


        }
        void InitialCTRLUI()
        {
            CTRLUI = ctrlUI1;
            CTRLUI.Initial(VERSION, OPTION, MACHINECollection.MACHINE);

            CTRLUI.TriggerAction += CTRLUI_TriggerAction;


        }

        void InitialDISPUI()
        {
            DISPUI.MoverAction += DISPUI_MoverAction;
            DISPUI.AdjustAction += DISPUI_AdjustAction;

            //這行在用Compound之後就沒用了，但之後還是可用
            //DISPUI.CaptureAction += DISPUI_CaptureAction;

            int i = 0;

            IsNeedToChange = false;

            if (cboShowCam == null)
                cboShowCam = new ComboBox();

            cboShowCam.Items.Add("ALL");

            if (CCDCollection.GetCCDCount > 1)
            {
                while (i < CCDCollection.GetCCDCount)
                {
                    cboShowCam.Items.Add("CAM" + i.ToString("00"));
                    i++;
                }
            }
            else
                cboShowCam.Visible = false;

            cboShowCam.SelectedIndex = 0;

            cboShowCam.SelectedIndexChanged += CboShowCam_SelectedIndexChanged;

            IsNeedToChange = true;
        }

        private void CboShowCam_SelectedIndexChanged(object sender, EventArgs e)
        {
            IsChangeShowCam = true;
            IsNeedToShowMover = true;
        }

        void InitialRESULT()
        {
            RESULT.TriggerAction += RESULT_TriggerAction;
            RESULT.EnvTriggerAction += RESULT_EnvTriggerAction;
            RESULT.TriggerOPAction += RESULT_TriggerOPAction;
            RESULT.TriggerOPMess += RESULT_TriggerOPMess;
            RESULT.TriggerShowImageCurrent += RESULT_TriggerShowImageCurrent;

            CCDCollection.TriggerAction += CCDCollection_TriggerAction;
            MACHINECollection.TriggerAction += MACHINECollection_TriggerAction;
        }

        private void RESULT_TriggerShowImageCurrent(Bitmap ebmpInput)
        {
            switch (Universal.VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch (Universal.OPTION)
                    {
                        case OptionEnum.MAIN_SDM2:

                            //pnlMappingUI.BackgroundImage = (Bitmap)ebmpInput.Clone();

                            break;
                    }
                    break;
            }
        }

        private void RESULT_TriggerOPMess(string str)
        {
            Bitmap bmp = new Bitmap(5000, 2000);
            Graphics graphics = Graphics.FromImage(bmp);

            SizeF sizeF = graphics.MeasureString(str, new Font("宋体", 150));

            float y = 2000f / 2 - sizeF.Height / 2;
            float x = 5000f / 2 - sizeF.Width / 2;
            graphics.FillRectangle(new SolidBrush(Color.Black), new RectangleF(0, 0, 5000, 2000));
            graphics.DrawString(str, new Font("宋体", 150), new SolidBrush(Color.Yellow), x, y);
            graphics.Dispose();
            DISPUI.ClearAll();
            DISPUI.SetDisplayImage(bmp);
        }



        /// <summary>
        /// 取得所有Album的字串
        /// </summary>
        void GetStatus()
        {
            string str = "";
            str += "CLTN: " + AlbumCollection.ToStatusString();
            lblTotalStatus.Text = str;
        }

        #region Event Operation
        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_MOUSEWHEEL)
            {
                // WM_MOUSEWHEEL, find the control at screen position m.LParam
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);

                IntPtr hWnd = WindowFromPoint(pos);
                if (hWnd != IntPtr.Zero && hWnd != m.HWnd && Control.FromHandle(hWnd) != null)
                {
                    SendMessage(hWnd, m.Msg, m.WParam, m.LParam);
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region ESSUI Actions
        private void ESSUI_TriggerAction(ESSStatusEnum status)
        {
            switch (status)
            {
                case ESSStatusEnum.LOGIN:
                    Login();
                    // if(!INI.isAccounFingerprint)
                    ESSUI.SetMainStatus(ESSUI.GetMainStatus());
                    if (INI.ISFOXCONNSF)
                        Universal.Memory.Write("Z," + ESSUI.AccountName);

                    break;
                case ESSStatusEnum.LOGOUT:

                    JetEazy.LoggerClass.Instance.WriteLog("帐户登出!");
                    RUNUI.GrpEnabled = false;
                    ESSUI.SetMainStatus(ESSStatusEnum.RUN);

                    if (INI.ISFOXCONNSF)
                        Universal.Memory.Write("Z," + ESSUI.AccountName);
                    if (Universal.OPTION == OptionEnum.R3)
                        Universal.OnR3TickStop("2");
                    if (Universal.OPTION == OptionEnum.C3)
                        Universal.OnC3TickStop("2");
                    break;
                case ESSStatusEnum.ACCOUNTMANAGE:
                    JetEazy.LoggerClass.Instance.WriteLog("点击帐号管理!");
                    AccountManagement();
                    break;
                case ESSStatusEnum.EXIT:
                    JetEazy.LoggerClass.Instance.WriteLog("程式退出!");
                    SetMachineState(MachineState.Idle);

                    if (INI.ISUSE_QFACTORY)
                    {
                        Universal.RESULT.myResult.QFactorySend(Universal.JZQFACTORY, QFactoryErrorCode.Err_Abnormal_CloseSystem);
                    }

                    if (INI.ISFOXCONNSF)
                        Universal.Memory.Write("Q,ESC");

                    if (CTRLUI != null)
                    {
                        CTRLUI.myDispose();
                    }

                    if (X6_HANDLE_CLIENT != null)
                        X6_HANDLE_CLIENT.DisConnectServer();

                    if (X6_HANDLE_CLIENT != null)
                        X6_LASER_CLIENT.DisConnectServer();

                    //ClientSocket.Instance.DisConnectServer();
                    Universal.Close();
                    this.Close();

                    break;
                case ESSStatusEnum.CHANGERECIPE:
                    JetEazy.LoggerClass.Instance.WriteLog("点击参数选择!");
                    ChangeRecipe();
                    break;
                case ESSStatusEnum.RUN:
                    switch (LAYOUT)
                    {
                        case LayoutEnum.L1280X800:
                            RUNV1UI.Visible = true;
                            RCPV1UI.Visible = false;
                            STPV1UI.Visible = false;
                            break;
                        case LayoutEnum.L1440X900:

                            cboShowCam.Enabled = true;

                            RUNUI.Visible = true;
                            RCPUI.Visible = false;
                            STPUI.Visible = false;
                            break;
                    }

                    CTRLUI.SetEnable(false);
                    SetMachineState(MachineState.Idle);
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_SDM3:
                        case OptionEnum.MAIN_SDM2:
                            tabCtrl.Visible = true;
                            break;
                    }
                    //switch (Universal.VERSION)
                    //{
                    //    case VersionEnum.ALLINONE:
                    //        switch (Universal.OPTION)
                    //        {
                    //            case OptionEnum.R32:
                    //                //if (INI.ISHIVECLIENT)
                    //                {
                    //                    Allinone.ControlSpace.MachineSpace.JzR32MachineClass jzr32machine = (Allinone.ControlSpace.MachineSpace.JzR32MachineClass)MACHINECollection.MACHINE;
                    //                    jzr32machine.SetMachineState(MachineState.Idle);
                    //                }
                    //                break;
                    //            case OptionEnum.R26:
                    //              //  if (INI.ISHIVECLIENT)
                    //                {
                    //                    Allinone.ControlSpace.MachineSpace.JzRXXMachineClass jzr26machine = (Allinone.ControlSpace.MachineSpace.JzRXXMachineClass)MACHINECollection.MACHINE;
                    //                    jzr26machine.SetMachineState(MachineState.Idle);
                    //                }
                    //                break;
                    //            case OptionEnum.R15:
                    //                if (INI.ISHIVECLIENT)
                    //                {
                    //                    Allinone.ControlSpace.MachineSpace.JzR15MachineClass jzr15machine = (Allinone.ControlSpace.MachineSpace.JzR15MachineClass)MACHINECollection.MACHINE;
                    //                    jzr15machine.SetMachineState(MachineState.Idle);
                    //                }
                    //                break;
                    //            case OptionEnum.R9:
                    //                if (INI.ISHIVECLIENT)
                    //                {
                    //                    Allinone.ControlSpace.MachineSpace.JzR9MachineClass jzr9machine = (Allinone.ControlSpace.MachineSpace.JzR9MachineClass)MACHINECollection.MACHINE;
                    //                    jzr9machine.SetMachineState(MachineState.Idle);
                    //                }
                    //                break;
                    //            case OptionEnum.R3:
                    //                if (INI.ISHIVECLIENT)
                    //                {
                    //                    Allinone.ControlSpace.MachineSpace.JzR3MachineClass jzr3machine = (Allinone.ControlSpace.MachineSpace.JzR3MachineClass)MACHINECollection.MACHINE;
                    //                    jzr3machine.SetMachineState(MachineState.Idle);
                    //                }
                    //                break;
                    //        }
                    //        break;
                    //}

                    break;
                case ESSStatusEnum.RECIPE:
                    JetEazy.LoggerClass.Instance.WriteLog("点击参数!");
                    switch (LAYOUT)
                    {
                        case LayoutEnum.L1280X800:
                            RUNV1UI.Visible = false;
                            RCPV1UI.Visible = true;
                            STPV1UI.Visible = false;
                            break;
                        case LayoutEnum.L1440X900:

                            cboShowCam.Enabled = true;
                            RUNUI.Visible = false;
                            RCPUI.Visible = true;
                            STPUI.Visible = false;

                            RUNUI.InitialResultPanel();
                            DISPUI.SetDisplayImage(CCDCollection.bmpAll);
                            break;
                    }

                    if (INI.ISLIVECAPTURE)
                    {
                        IsNeedToShowMover = true;
                        DISPUIShowMover();
                    }

                    CTRLUI.SetEnable(true);
                    SetMachineState(MachineState.Engineering_mode);
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_SDM3:
                        case OptionEnum.MAIN_SDM2:
                            tabCtrl.Visible = false;
                            break;
                    }
                    //switch (Universal.VERSION)
                    //{
                    //    case VersionEnum.ALLINONE:
                    //        switch (Universal.OPTION)
                    //        {
                    //            case OptionEnum.R32:
                    //                if (INI.ISHIVECLIENT)
                    //                {
                    //                    Allinone.ControlSpace.MachineSpace.JzR32MachineClass jzr32machine = (Allinone.ControlSpace.MachineSpace.JzR32MachineClass)MACHINECollection.MACHINE;
                    //                    jzr32machine.SetMachineState(MachineState.Engineering_mode);
                    //                }
                    //                break;
                    //            case OptionEnum.R26:
                    //                if (INI.ISHIVECLIENT)
                    //                {
                    //                    Allinone.ControlSpace.MachineSpace.JzRXXMachineClass jzr26machine = (Allinone.ControlSpace.MachineSpace.JzRXXMachineClass)MACHINECollection.MACHINE;
                    //                    jzr26machine.SetMachineState(MachineState.Engineering_mode);
                    //                }
                    //                break;
                    //            case OptionEnum.R15:
                    //                if (INI.ISHIVECLIENT)
                    //                {
                    //                    Allinone.ControlSpace.MachineSpace.JzR32MachineClass jzr15machine = (Allinone.ControlSpace.MachineSpace.JzR32MachineClass)MACHINECollection.MACHINE;
                    //                    jzr15machine.SetMachineState(MachineState.Engineering_mode);
                    //                }
                    //                break;
                    //            case OptionEnum.R9:
                    //                if (INI.ISHIVECLIENT)
                    //                {
                    //                    Allinone.ControlSpace.MachineSpace.JzR9MachineClass jzr9machine = (Allinone.ControlSpace.MachineSpace.JzR9MachineClass)MACHINECollection.MACHINE;
                    //                    jzr9machine.SetMachineState(MachineState.Engineering_mode);
                    //                }
                    //                break;
                    //            case OptionEnum.R3:
                    //                if (INI.ISHIVECLIENT)
                    //                {
                    //                    Allinone.ControlSpace.MachineSpace.JzR3MachineClass jzr3machine = (Allinone.ControlSpace.MachineSpace.JzR3MachineClass)MACHINECollection.MACHINE;
                    //                    jzr3machine.SetMachineState(MachineState.Engineering_mode);
                    //                }
                    //                break;
                    //        }
                    //        break;
                    //}

                    break;
                case ESSStatusEnum.SETUP:
                    JetEazy.LoggerClass.Instance.WriteLog("点击设定!");
                    switch (LAYOUT)
                    {
                        case LayoutEnum.L1280X800:
                            RUNV1UI.Visible = false;
                            RCPV1UI.Visible = false;
                            STPV1UI.Visible = true;
                            break;
                        case LayoutEnum.L1440X900:

                            cboShowCam.SelectedIndex = 0;
                            cboShowCam.Enabled = false;

                            RUNUI.Visible = false;
                            RCPUI.Visible = false;
                            STPUI.Visible = true;

                            RUNUI.InitialResultPanel();
                            DISPUI.SetDisplayImage(CCDCollection.bmpAll);

                            break;
                    }

                    CTRLUI.SetEnable(true);

                    switch(OPTION)
                    {
                        case OptionEnum.MAIN_SDM3:
                        case OptionEnum.MAIN_SDM2:
                            tabCtrl.Visible = false;
                            break;
                    }

                    SetMachineState(MachineState.Idle);
                    //switch (Universal.VERSION)
                    //{
                    //    case VersionEnum.ALLINONE:
                    //        switch (Universal.OPTION)
                    //        {
                    //            case OptionEnum.R32:
                    //                if (INI.ISHIVECLIENT)
                    //                {
                    //                    Allinone.ControlSpace.MachineSpace.JzR32MachineClass jzr32machine = (Allinone.ControlSpace.MachineSpace.JzR32MachineClass)MACHINECollection.MACHINE;
                    //                    jzr32machine.SetMachineState(MachineState.Idle);
                    //                }
                    //                break;
                    //            case OptionEnum.R15:
                    //                if (INI.ISHIVECLIENT)
                    //                {
                    //                    Allinone.ControlSpace.MachineSpace.JzR15MachineClass jzr15machine = (Allinone.ControlSpace.MachineSpace.JzR15MachineClass)MACHINECollection.MACHINE;
                    //                    jzr15machine.SetMachineState(MachineState.Idle);
                    //                }
                    //                break;
                    //            case OptionEnum.R26:
                    //                if (INI.ISHIVECLIENT)
                    //                {
                    //                    Allinone.ControlSpace.MachineSpace.JzRXXMachineClass jzr26machine = (Allinone.ControlSpace.MachineSpace.JzRXXMachineClass)MACHINECollection.MACHINE;
                    //                    jzr26machine.SetMachineState(MachineState.Idle);
                    //                }
                    //                break;
                    //            case OptionEnum.R9:
                    //                if (INI.ISHIVECLIENT)
                    //                {
                    //                    Allinone.ControlSpace.MachineSpace.JzR9MachineClass jzr9machine = (Allinone.ControlSpace.MachineSpace.JzR9MachineClass)MACHINECollection.MACHINE;
                    //                    jzr9machine.SetMachineState(MachineState.Idle);
                    //                }
                    //                break;
                    //            case OptionEnum.R3:
                    //                if (INI.ISHIVECLIENT)
                    //                {
                    //                    Allinone.ControlSpace.MachineSpace.JzR3MachineClass jzr3machine = (Allinone.ControlSpace.MachineSpace.JzR3MachineClass)MACHINECollection.MACHINE;
                    //                    jzr3machine.SetMachineState(MachineState.Idle);
                    //                }
                    //                break;
                    //        }
                    //        break;
                    //}

                    break;
                case ESSStatusEnum.CHECKLIVE:

                    CheckLive();

                    break;
                case ESSStatusEnum.SHOWSETUP:

                    ShowSetup();

                    break;
                case ESSStatusEnum.FASTCAL:
                    JetEazy.LoggerClass.Instance.WriteLog("点击小算盘测试!");
                    switch (Universal.VERSION)
                    {

                        case VersionEnum.ALLINONE:

                            switch (Universal.OPTION)
                            {
                                case OptionEnum.R32:

                                    string strPath = RESULT.myResult.GetLastDirPath(Universal.DEBUGSRCPATH);
                                    strPath += @"\SN.txt";
                                    if (System.IO.File.Exists(strPath))
                                    {
                                        Universal.RunDebugOrRelease = "Release:";
                                        string strsnPath = INI.SHOPFLOORPATH + @"\SN.txt";
                                        if (!System.IO.File.Exists(strsnPath))
                                            System.IO.File.Copy(strPath, INI.SHOPFLOORPATH + @"\SN.txt");
                                    }
                                    else
                                    {
                                        Universal.RunDebugOrRelease = "Debug:";
                                        //不要執行包含在固定參數的參數
                                        if (("," + INI.PRELOADSTATICNO + ",").IndexOf("," + RCPDB.DataNow.No.ToString() + ",") > -1)
                                            MessageBox.Show("Please Check " + RCPDB.DataNow.Name + " ID = " + RCPDB.DataNow.No.ToString() + ".");
                                        else
                                        {


                                            if (CheckAlbumCollection(RCPDB.DataNow.Version, true, TestMethodEnum.BUTTON))
                                            {
                                                RESULT.TestMethod = TestMethodEnum.BUTTON;
                                                RESULT.Calculate();
                                            }
                                        }
                                    }

                                    break;
                                case OptionEnum.R3:
                                    bool isdebugJPG = false;
                                    string mess = "";
                                    string strpic = @"D:\JETEAZY\ALLINONE-R3\BUGBUG\20200311\";
                                    if (isdebugJPG)
                                    {

                                        string pathname = RESULT.myResult.GetDirPath(strpic);
                                        string[] strmess = pathname.Split('_');
                                        mess = "SN:" + strmess[2] + Environment.NewLine;
                                        mess += "EM: XXXX" + Environment.NewLine;
                                        mess += " MO:J185" + Environment.NewLine;
                                        mess += "CU:LL" + Environment.NewLine;
                                        mess += "LS:Y";

                                        strpic += pathname + "\\";




                                    }

                                    //Universal.IsMultiThread = false;
                                    string strPath2 = RESULT.myResult.GetLastDirPath(Universal.DEBUGSRCPATH);

                                    Universal.FolderName = System.IO.Path.GetFileNameWithoutExtension(strPath2);
                                    Universal.FolderPath = strPath2 + "\\";
                                    strPath2 += @"\OCRTexting.txt";
                                    if (System.IO.File.Exists(strPath2))
                                    {
                                        Universal.RunDebugOrRelease = "Release:";

                                        string strsnPath = INI.SHOPFLOORPATH + @"\OCRTexting.txt";

                                        if (isdebugJPG)
                                        {
                                            System.IO.StreamWriter Swr = new System.IO.StreamWriter(strsnPath, true, Encoding.Default);
                                            Swr.Write(mess);
                                            Swr.Flush();
                                            Swr.Close();

                                            var files = System.IO.Directory.GetFiles(strpic, "*.jpg");
                                            foreach (var file in files)
                                            {
                                                if (file.IndexOf("_CAM000") > 0)
                                                {
                                                    Bitmap bmptemp = new Bitmap(file);
                                                    bmptemp.Save(Universal.FolderPath + "000\\P00-000.png");
                                                    bmptemp.Dispose();
                                                }
                                                else if (file.IndexOf("_CAM001") > 0)
                                                {
                                                    Bitmap bmptemp = new Bitmap(file);
                                                    bmptemp.Save(Universal.FolderPath + "000\\P00-001.png");
                                                    bmptemp.Dispose();
                                                }
                                                else if (file.IndexOf("_CAM002") > 0)
                                                {
                                                    Bitmap bmptemp = new Bitmap(file);
                                                    bmptemp.Save(Universal.FolderPath + "000\\P00-002.png");
                                                    bmptemp.Dispose();
                                                }
                                            }
                                        }

                                        if (!System.IO.File.Exists(strsnPath))
                                            System.IO.File.Copy(strPath2, strsnPath);
                                    }
                                    else
                                    {
                                        Universal.RunDebugOrRelease = "Debug:";
                                        if (("," + INI.PRELOADSTATICNO + ",").IndexOf("," + RCPDB.DataNow.No.ToString() + ",") > -1)
                                            MessageBox.Show("Please Check " + RCPDB.DataNow.Name + " ID = " + RCPDB.DataNow.No.ToString() + ".");
                                        else
                                        {


                                            if (CheckAlbumCollection(RCPDB.DataNow.Version, true, TestMethodEnum.BUTTON))
                                            {
                                                IsLiveCapturing = false;
                                                RESULT.TestMethod = TestMethodEnum.BUTTON;
                                                RESULT.Calculate();

                                                if (RUNUI != null)
                                                {
                                                    Universal.IsSaveRaw = RUNUI.IsSaveRaw;
                                                }
                                            }
                                        }
                                    }

                                    break;
                                case OptionEnum.C3:
                                    bool isdebugJPGC3 = false;
                                    string messC3 = "";
                                    string strpicC3 = @"D:\JETEAZY\ALLINONE-C3\BUGBUG\20200311\";
                                    if (isdebugJPGC3)
                                    {

                                        string pathname = RESULT.myResult.GetDirPath(strpicC3);
                                        string[] strmess = pathname.Split('_');
                                        mess = "SN:" + strmess[2] + Environment.NewLine;
                                        mess += "EM: XXXX" + Environment.NewLine;
                                        mess += " MO:J185" + Environment.NewLine;
                                        mess += "CU:LL" + Environment.NewLine;
                                        mess += "LS:Y";

                                        strpicC3 += pathname + "\\";




                                    }

                                    //Universal.IsMultiThread = false;
                                    string strPath2C3 = RESULT.myResult.GetLastDirPath(Universal.DEBUGSRCPATH);

                                    Universal.FolderName = System.IO.Path.GetFileNameWithoutExtension(strPath2C3);
                                    Universal.FolderPath = strPath2C3 + "\\";
                                    strPath2C3 += @"\OCRTexting.txt";
                                    if (System.IO.File.Exists(strPath2C3))
                                    {
                                        Universal.RunDebugOrRelease = "Release:";

                                        string strsnPath = INI.SHOPFLOORPATH + @"\OCRTexting.txt";

                                        if (isdebugJPGC3)
                                        {
                                            System.IO.StreamWriter Swr = new System.IO.StreamWriter(strsnPath, true, Encoding.Default);
                                            Swr.Write(messC3);
                                            Swr.Flush();
                                            Swr.Close();

                                            var files = System.IO.Directory.GetFiles(strpicC3, "*.jpg");
                                            foreach (var file in files)
                                            {
                                                if (file.IndexOf("_CAM000") > 0)
                                                {
                                                    Bitmap bmptemp = new Bitmap(file);
                                                    bmptemp.Save(Universal.FolderPath + "000\\P00-000.png");
                                                    bmptemp.Dispose();
                                                }
                                                else if (file.IndexOf("_CAM001") > 0)
                                                {
                                                    Bitmap bmptemp = new Bitmap(file);
                                                    bmptemp.Save(Universal.FolderPath + "000\\P00-001.png");
                                                    bmptemp.Dispose();
                                                }
                                                else if (file.IndexOf("_CAM002") > 0)
                                                {
                                                    Bitmap bmptemp = new Bitmap(file);
                                                    bmptemp.Save(Universal.FolderPath + "000\\P00-002.png");
                                                    bmptemp.Dispose();
                                                }
                                            }
                                        }

                                        if (!System.IO.File.Exists(strsnPath))
                                            System.IO.File.Copy(strPath2C3, strsnPath);
                                    }
                                    else
                                    {
                                        Universal.RunDebugOrRelease = "Debug:";
                                        if (("," + INI.PRELOADSTATICNO + ",").IndexOf("," + RCPDB.DataNow.No.ToString() + ",") > -1)
                                            MessageBox.Show("Please Check " + RCPDB.DataNow.Name + " ID = " + RCPDB.DataNow.No.ToString() + ".");
                                        else
                                        {


                                            if (CheckAlbumCollection(RCPDB.DataNow.Version, true, TestMethodEnum.BUTTON))
                                            {
                                                IsLiveCapturing = false;
                                                RESULT.TestMethod = TestMethodEnum.BUTTON;
                                                RESULT.Calculate();

                                                if (RUNUI != null)
                                                {
                                                    Universal.IsSaveRaw = RUNUI.IsSaveRaw;
                                                }
                                            }
                                        }
                                    }

                                    break;
                                case OptionEnum.MAIN_SDM2:
                                case OptionEnum.MAIN_SDM3:
                                    if (CheckAlbumCollection(RCPDB.DataNow.Version, true, TestMethodEnum.BUTTON))
                                    {
                                        IsLiveCapturing = false;
                                        RESULT.TestMethod = TestMethodEnum.BUTTON;
                                        RESULT.CalculateSMD2();

                                        if (RUNUI != null)
                                        {
                                            Universal.IsSaveRaw = RUNUI.IsSaveRaw;
                                        }
                                    }

                                    break;

                                case OptionEnum.MAIN_SDM1:
                                case OptionEnum.MAIN_SD:
                                case OptionEnum.MAIN_X6:
                                case JetEazy.OptionEnum.MAIN_SERVICE:

                                    if (CheckAlbumCollection(RCPDB.DataNow.Version, true, TestMethodEnum.BUTTON))
                                    {
                                        IsLiveCapturing = false;
                                        RESULT.TestMethod = TestMethodEnum.BUTTON;
                                        RESULT.Calculate();

                                        if (RUNUI != null)
                                        {
                                            Universal.IsSaveRaw = RUNUI.IsSaveRaw;
                                        }
                                    }


                                    break;

                                default:
                                    //Universal.IsMultiThread = false;
                                    string strPath3 = RESULT.myResult.GetLastDirPath(Universal.DEBUGSRCPATH);
                                    Universal.FolderName = System.IO.Path.GetFileNameWithoutExtension(strPath3);
                                    Universal.FolderPath = strPath3 + "\\";
                                    strPath3 += @"\SN.txt";
                                    if (System.IO.File.Exists(strPath3))
                                    {
                                        Universal.RunDebugOrRelease = "Release:";

                                        string strsnPath = INI.SHOPFLOORPATH + @"\SN.txt";
                                        if (!System.IO.File.Exists(strsnPath))
                                            System.IO.File.Copy(strPath3, strsnPath);
                                    }
                                    else
                                    {
                                        Universal.RunDebugOrRelease = "Debug:";
                                        if (("," + INI.PRELOADSTATICNO + ",").IndexOf("," + RCPDB.DataNow.No.ToString() + ",") > -1)
                                            MessageBox.Show("Please Check " + RCPDB.DataNow.Name + " ID = " + RCPDB.DataNow.No.ToString() + ".");
                                        else
                                        {


                                            if (CheckAlbumCollection(RCPDB.DataNow.Version, true, TestMethodEnum.BUTTON))
                                            {
                                                IsLiveCapturing = false;
                                                RESULT.TestMethod = TestMethodEnum.BUTTON;
                                                RESULT.Calculate();

                                                if (RUNUI != null)
                                                {
                                                    Universal.IsSaveRaw = RUNUI.IsSaveRaw;
                                                }
                                            }
                                        }
                                    }

                                    break;
                            }

                            break;
                    }
                    break;
            }
        }

        void SetMachineState(MachineState machine)
        {
            switch (Universal.VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch (Universal.OPTION)
                    {
                        case OptionEnum.R32:
                            //if (INI.ISHIVECLIENT)
                            {
                                Allinone.ControlSpace.MachineSpace.JzR32MachineClass jzr32machine = (Allinone.ControlSpace.MachineSpace.JzR32MachineClass)MACHINECollection.MACHINE;
                                jzr32machine.SetMachineState(machine);
                            }
                            break;
                        case OptionEnum.R26:
                            //  if (INI.ISHIVECLIENT)
                            {
                                Allinone.ControlSpace.MachineSpace.JzRXXMachineClass jzr26machine = (Allinone.ControlSpace.MachineSpace.JzRXXMachineClass)MACHINECollection.MACHINE;
                                jzr26machine.SetMachineState(machine);
                            }
                            break;
                        case OptionEnum.R15:
                            // if (INI.ISHIVECLIENT)
                            {
                                Allinone.ControlSpace.MachineSpace.JzR15MachineClass jzr15machine = (Allinone.ControlSpace.MachineSpace.JzR15MachineClass)MACHINECollection.MACHINE;
                                jzr15machine.SetMachineState(machine);
                            }
                            break;
                        case OptionEnum.R9:
                            // if (INI.ISHIVECLIENT)
                            {
                                Allinone.ControlSpace.MachineSpace.JzR9MachineClass jzr9machine = (Allinone.ControlSpace.MachineSpace.JzR9MachineClass)MACHINECollection.MACHINE;
                                jzr9machine.SetMachineState(machine);
                            }
                            break;
                        case OptionEnum.R5:
                            // if (INI.ISHIVECLIENT)
                            {
                                Allinone.ControlSpace.MachineSpace.JzR5MachineClass jzr5machine = (Allinone.ControlSpace.MachineSpace.JzR5MachineClass)MACHINECollection.MACHINE;
                                jzr5machine.SetMachineState(machine);
                            }
                            break;
                        case OptionEnum.R3:
                            //if (INI.ISHIVECLIENT)
                            {
                                Allinone.ControlSpace.MachineSpace.JzR3MachineClass jzr3machine = (Allinone.ControlSpace.MachineSpace.JzR3MachineClass)MACHINECollection.MACHINE;
                                jzr3machine.SetMachineState(machine);
                            }
                            break;
                        case OptionEnum.C3:
                            //if (INI.ISHIVECLIENT)
                            {
                                Allinone.ControlSpace.MachineSpace.JzC3MachineClass jzc3machine = (Allinone.ControlSpace.MachineSpace.JzC3MachineClass)MACHINECollection.MACHINE;
                                jzc3machine.SetMachineState(machine);
                            }
                            break;
                        case OptionEnum.R1:
                            //if (INI.ISHIVECLIENT)
                            {
                                Allinone.ControlSpace.MachineSpace.JzR1MachineClass jzr1machine = (Allinone.ControlSpace.MachineSpace.JzR1MachineClass)MACHINECollection.MACHINE;
                                jzr1machine.SetMachineState(machine);
                            }
                            break;
                    }
                    break;
            }
        }

        MachineState GetMachineState
        {
            get
            {
                switch (Universal.VERSION)
                {
                    case VersionEnum.ALLINONE:
                        switch (Universal.OPTION)
                        {
                            case OptionEnum.R32:
                                //if (INI.ISHIVECLIENT)
                                {
                                    Allinone.ControlSpace.MachineSpace.JzR32MachineClass jzr32machine = (Allinone.ControlSpace.MachineSpace.JzR32MachineClass)MACHINECollection.MACHINE;
                                    // jzr32machine.SetMachineState(machine);

                                    return jzr32machine.GetCurrentMachineState;
                                }
                                break;
                            case OptionEnum.R26:
                                //  if (INI.ISHIVECLIENT)
                                {
                                    Allinone.ControlSpace.MachineSpace.JzRXXMachineClass jzr26machine = (Allinone.ControlSpace.MachineSpace.JzRXXMachineClass)MACHINECollection.MACHINE;
                                    //  jzr26machine.SetMachineState(machine);
                                    return jzr26machine.GetCurrentMachineState;
                                }
                                break;
                            case OptionEnum.R15:
                                // if (INI.ISHIVECLIENT)
                                {
                                    Allinone.ControlSpace.MachineSpace.JzR15MachineClass jzr15machine = (Allinone.ControlSpace.MachineSpace.JzR15MachineClass)MACHINECollection.MACHINE;
                                    // jzr15machine.SetMachineState(machine);
                                    return jzr15machine.GetCurrentMachineState;
                                }
                                break;
                            case OptionEnum.R9:
                                // if (INI.ISHIVECLIENT)
                                {
                                    Allinone.ControlSpace.MachineSpace.JzR9MachineClass jzr9machine = (Allinone.ControlSpace.MachineSpace.JzR9MachineClass)MACHINECollection.MACHINE;
                                    // jzr9machine.SetMachineState(machine);

                                    return jzr9machine.GetCurrentMachineState;
                                }
                                break;
                            case OptionEnum.R5:
                                // if (INI.ISHIVECLIENT)
                                {
                                    Allinone.ControlSpace.MachineSpace.JzR5MachineClass jzr5machine = (Allinone.ControlSpace.MachineSpace.JzR5MachineClass)MACHINECollection.MACHINE;
                                    // jzr9machine.SetMachineState(machine);

                                    return jzr5machine.GetCurrentMachineState;
                                }
                                break;
                            case OptionEnum.R1:
                                // if (INI.ISHIVECLIENT)
                                {
                                    Allinone.ControlSpace.MachineSpace.JzR1MachineClass jzr1machine = (Allinone.ControlSpace.MachineSpace.JzR1MachineClass)MACHINECollection.MACHINE;
                                    // jzr9machine.SetMachineState(machine);

                                    return jzr1machine.GetCurrentMachineState;
                                }
                                break;
                            case OptionEnum.R3:
                                //if (INI.ISHIVECLIENT)
                                {
                                    Allinone.ControlSpace.MachineSpace.JzR3MachineClass jzr3machine = (Allinone.ControlSpace.MachineSpace.JzR3MachineClass)MACHINECollection.MACHINE;
                                    //   jzr3machine.SetMachineState(machine);

                                    return jzr3machine.GetCurrentMachineState;
                                }
                                break;
                            case OptionEnum.C3:
                                //if (INI.ISHIVECLIENT)
                                {
                                    Allinone.ControlSpace.MachineSpace.JzC3MachineClass jzc3machine = (Allinone.ControlSpace.MachineSpace.JzC3MachineClass)MACHINECollection.MACHINE;
                                    //   jzr3machine.SetMachineState(machine);

                                    return jzc3machine.GetCurrentMachineState;
                                }
                                break;
                        }
                        break;
                }

                return MachineState.Idle;
            }
        }

        LoginForm LOGINFRM;

        void Login()
        {
            IsLiveCapturing = false;

            if (!Universal.IsNoUseCCD)
            {
                if (INI.isAccounFingerprint)
                {
                    // 開啟登入視窗
                    JetEazy.DB.CmUserAccount user = null;
                    if (JetEazy.DB.CmUserAccountMgr.PromptLogin())
                    {
                        user = JetEazy.DB.CmUserAccountMgr.CurrentUser;
                    }

                    // 檢查使用者權限
                    //  _TRACE(user);
                    bool isSysAdmin = (user != null && user.AllowSystemSetup);
                    //bool isRecipeMgr = (user != null && user.AllowRecipeEditting);
                    //bool isAccountMgr = (user != null && user.AllowAccountManagement);

                    if (isSysAdmin)
                    {
                        ACCDB.CheckIsCertifiedTo(true);
                        ESSUI.SetMainLoginStatus(ESSStatusEnum.LOGIN);
                        ESSUI.SetMainStatus(ESSStatusEnum.RUN);
                        //    ESSUI.login();

                        RUNUI.GrpEnabled = true;
                    }
                }
                else
                {
                    LOGINFRM = new LoginForm(ACCDB, Universal.UIPATH, INI.LANGUAGE);

                    if (LOGINFRM.ShowDialog() == DialogResult.OK)
                    {
                        ESSUI.SetMainLoginStatus(ESSStatusEnum.LOGIN);
                        ESSUI.SetMainStatus(ESSStatusEnum.RUN);

                        RUNUI.GrpEnabled = true;
                    }

                    LOGINFRM.Close();
                    LOGINFRM.Dispose();
                }
            }
            else
            {
                if (Universal.IsOfflineUserAutoLogin)
                {
                    ACCDB.CheckIsCertifiedTo(true);
                    ESSUI.SetMainLoginStatus(ESSStatusEnum.LOGIN);
                    ESSUI.SetMainStatus(ESSStatusEnum.RUN);
                    //    ESSUI.login();
                    RUNUI.GrpEnabled = true;
                }
                else
                {
                    LOGINFRM = new LoginForm(ACCDB, Universal.UIPATH, INI.LANGUAGE);

                    if (LOGINFRM.ShowDialog() == DialogResult.OK)
                    {
                        ESSUI.SetMainLoginStatus(ESSStatusEnum.LOGIN);
                        ESSUI.SetMainStatus(ESSStatusEnum.RUN);

                        RUNUI.GrpEnabled = true;
                    }

                    LOGINFRM.Close();
                    LOGINFRM.Dispose();
                }
            }

            IsLiveCapturing = INI.ISLIVECAPTURE;
        }

        AccountForm ACCOUNTFRM;
        void AccountManagement()
        {
            IsLiveCapturing = false;

            ACCOUNTFRM = new AccountForm(ACCDB, Universal.UIPATH, INI.LANGUAGE);
            ACCOUNTFRM.ShowDialog();

            ACCOUNTFRM.Close();
            ACCOUNTFRM.Dispose();

            ESSUI.SetMainStatus(ESSUI.GetMainStatus());

            IsLiveCapturing = INI.ISLIVECAPTURE;
        }

        /// <summary>
        /// 切換參數並加入 AlbumCollection
        /// </summary>
        void ChangeRecipe()
        {
            bool IsNeedTrainAlbum = false;

            IsLiveCapturing = false;
            int LastSelectedIndex = RCPDB.DataNow.No;
            JzToolsClass.PassingInteger = RCPDB.DataNow.No;

            RcpSelectForm RCPSELECTFRM = new RcpSelectForm(RCPDB, Universal.SHOWBMPSTRING);

            RCPSELECTFRM.ParentWindosw = new Rectangle(this.Location, this.Size);


            if (RCPSELECTFRM.ShowDialog() == DialogResult.OK)
            {
                if (LastSelectedIndex != JzToolsClass.PassingInteger)
                {
                    RCPDB.GotoIndex(RCPDB.FindIndex(JzToolsClass.PassingInteger));
                    ESSUI.SetRecipeName(RCPDB.DataNow.ToESSString());

                    if (AlbumCollection.FindStaticIndicator(RCPDB.DataNow.No) == -1)
                    {
                        if (AlbumCollection.FindNormalIndicator(RCPDB.DataNow.No) == -1)
                        {
                            AlbumClass album = new AlbumClass(RCPDB.DataNow);
                            AlbumCollection.Add(album);

                            IsNeedTrainAlbum = true;
                        }
                    }
                }

                if (JzToolsClass.PassingString != "")
                {
                    string[] RemoveIndexStr = JzToolsClass.PassingString.Split(',');

                    foreach (string str in RemoveIndexStr)
                        RCPDB.Delete(RCPDB.FindIndex(int.Parse(str)));
                }

                AlbumCollection.Del(RCPDB);

                Universal.BackupDATADB();
                RCPDB.Save();
            }
            else
            {
                //防被裏面有被刪除掉的資料要回復
                RCPDB.Load();
            }

            RCPSELECTFRM.Close();
            RCPSELECTFRM.Dispose();

            RCPDB.GotoIndex(RCPDB.FindIndex(JzToolsClass.PassingInteger));

            GetStatus();

            ESSDB.DataNow.LastRecipeNo = RCPDB.DataNow.No;
            ESSDB.Save();

            //this.Refresh();

            if (IsNeedTrainAlbum)
                TrainAlbum(RCPDB.DataNow.No, true);

            AlbumCollection.GotoIndex(RCPDB.DataNow.No);

            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    RCPUI.Initial(Universal.UIPATH, INI.LANGUAGE, VERSION, OPTION, RCPDB, AlbumCollection, Universal.SHOWBMPSTRING);
                    break;
                case VersionEnum.AUDIX:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN:
                            RCPUI.Initial(Universal.UIPATH, INI.LANGUAGE, VERSION, OPTION, RCPDB, AlbumCollection, Universal.SHOWBMPSTRING);
                            break;
                        case OptionEnum.MAIN_DFLY:
                            RCPV1UI.Initial(Universal.UIPATH, INI.LANGUAGE, VERSION, OPTION, RCPDB, AlbumCollection, Universal.SHOWBMPSTRING);
                            break;
                    }
                    break;
            }
            //设定相机的亮度
            SetCamLight(AlbumNow);

            //Need To Refresh Figures
            if (INI.ISLIVECAPTURE)
            {
                IsNeedToShowMover = true;
                DISPUIShowMover();
            }

            RUNUI.MappingInit();

            IsLiveCapturing = INI.ISLIVECAPTURE;

            CGOperate();
        }
        /// <summary>
        /// 從 需要 LIVE 的狀態改為原來不知是否有 LIVE 的狀態
        /// </summary>
        void CheckLive()
        {
            IsLiveCapturing = INI.ISLIVECAPTURE;
            IsNeedToShowMover = true;

            if (!IsLiveCapturing)
            {
                DISPUI.ClearAll();
            }
        }
        void ShowSetup()
        {
            if (!INI.ISLIVECAPTURE)
            {
                CCDCollection.GetBmpAll(-1);
                DISPUI.ReplaceDisplayImage(CCDCollection.bmpAll);
            }

            DISPUI.ClearMover();

        }

        #endregion

        #region RCPUI Actions
        private void RCPUI_TriggerAction(RCPStatusEnum status, string opstr)
        {
            switch (status)
            {
                case RCPStatusEnum.EDIT:
                    JetEazy.LoggerClass.Instance.WriteLog("点击修改或新增或复制参数");
                    IsLiveCapturing = true;
                    //DISPUI.SetDisplayType(DisplayTypeEnum.CAPTRUE);
                    ESSUI.Disable = true;
                    IsNeedToShowMover = true;
                    DISPUIShowMover();

                    try
                    {
                        int envindexinrcpui = JzToolsClass.PassingInteger;
                        if (envindexinrcpui >= AlbumNow.ENVList.Count)
                            envindexinrcpui = 0;
                        EnvClass backenv = AlbumNow.ENVList[envindexinrcpui].Clone();
                        SetCamLight(AlbumNow, envindexinrcpui);
                        // SetCamLight(AlbumNow);
                    }
                    catch (Exception ex)
                    {
                        JetEazy.LoggerClass.Instance.WriteException(ex);
                    }
                    break;
                case RCPStatusEnum.MODIFYCANCEL:
                    JetEazy.LoggerClass.Instance.WriteLog("点击取消修改参数");
                    IsLiveCapturing = INI.ISLIVECAPTURE;

                    switch (LAYOUT)
                    {
                        case LayoutEnum.L1280X800:
                            RCPV1UI.RestoreBack();
                            break;
                        case LayoutEnum.L1440X900:
                            RCPUI.RestoreBack();
                            break;
                    }

                    ESSUI.Disable = false;
                    ESSUI.SetRecipeName(RCPDB.DataNow.ToESSString());

                    ESSDB.DataNow.LastRecipeNo = RCPDB.DataNow.No;

                    ESSDB.Save();

                    // SetCamLight(AlbumNow, 0);
                    SetCamLight(AlbumNow);

                    TrainAlbum(RCPDB.DataNow.No, true);

                    DISPUI.SetDisplayType(DisplayTypeEnum.SHOW);

                    AlbumCollection.ProcessStaticAlbum(true);

                    RUNUI.MappingInit();

                    GetStatus();

                    CGOperate();

                    break;
                case RCPStatusEnum.MODIFYCOMPLETE:

                    IsLiveCapturing = INI.ISLIVECAPTURE;

                    //  AlbumCollection.ProcessStaticAlbum(false);

                    AlbumCollection.AlbumNow.Save();

                    //RCPUI.RestoreBack(false);

                    switch (LAYOUT)
                    {
                        case LayoutEnum.L1280X800:
                            RCPV1UI.RestoreBack(false);
                            break;
                        case LayoutEnum.L1440X900:
                            RCPUI.RestoreBack(false);
                            break;
                    }

                    ESSUI.Disable = false;
                    ESSUI.SetRecipeName(RCPDB.DataNow.ToESSString());

                    ESSDB.DataNow.LastRecipeNo = RCPDB.DataNow.No;

                    ESSDB.Save();

                    //  SetCamLight(AlbumNow, 0);
                    SetCamLight(AlbumNow);

                    TrainAlbum(RCPDB.DataNow.No);

                    DISPUI.SetDisplayType(DisplayTypeEnum.SHOW);

                    AlbumCollection.ProcessStaticAlbum(false);

                    RUNUI.MappingInit();

                    GetStatus();

                    //CGOperate();

                    break;
                case RCPStatusEnum.SETPOSITION:

                    switch (LAYOUT)
                    {
                        case LayoutEnum.L1280X800:
                            RCPV1UI.SetPosition(MACHINECollection.GetPosition());
                            break;
                        case LayoutEnum.L1440X900:
                            RCPUI.SetPosition(MACHINECollection.GetPosition());
                            break;
                    }

                    //RCPUI.SetPosition(MACHINECollection.GetPosition());
                    //RCPUI.SetPosition(xpos, ypos, zpos);

                    break;
                case RCPStatusEnum.SETEND:
                    RCPUI.SetPosition("");
                    //RCPUI.SetPosition(xpos, ypos, zpos);

                    break;
                case RCPStatusEnum.GOPOSITION:

                    if (opstr.Trim() != "")
                        MACHINECollection.GoPosition(opstr);

                    break;
                case RCPStatusEnum.SHOWDETAIL:
                    ShowDetail();
                    break;
                case RCPStatusEnum.SHOWASSIGN:
                    ShowASN();
                    break;
                case RCPStatusEnum.SHOWCOMPOUND:
                    ShowCPD();
                    break;
                case RCPStatusEnum.CHANGELIGHT:
                    JetEazy.LoggerClass.Instance.WriteLog("点击保存参数");

                    string[] strs = opstr.Split(';');
                    SetCamLight(AlbumNow, int.Parse(strs[0]), strs[1]);

                    SetCamLight(AlbumNow);
                    break;
            }
        }

        public static DetailForm DETAILFRM;
        void ShowDetail()
        {
            IsLiveCapturing = false;

            int envindexinrcpui = JzToolsClass.PassingInteger;

            EnvClass backenv = AlbumNow.ENVList[envindexinrcpui].Clone();

            bool isstatic = INI.PRELOADSTATICNO.IndexOf(RCPDB.DataNow.No.ToString(RcpClass.ORGRCPNOSTRING)) > -1;
            SetCamLight(AlbumNow);
            //  SetCamLight(AlbumNow, envindexinrcpui);
            DETAILFRM = new DetailForm(CCDCollection, backenv, VERSION, Universal.OPTION, isstatic);

            if (DETAILFRM.ShowDialog() == DialogResult.OK)
            {
                EnvClass env = AlbumNow.ENVList[envindexinrcpui];
                env.Suicide();
                AlbumNow.ENVList.RemoveAt(envindexinrcpui);
                AlbumNow.ENVList.Insert(envindexinrcpui, backenv);

                //AlbumNow.FillEnvAction(envindexinrcpui);
            }
            else
                backenv.Suicide();

            DISPUI.ClearMover();

            IsNeedToShowMover = true;
            DISPUIShowMover();

            DETAILFRM.Close();
            DETAILFRM.Dispose();

            //SetCamLight(AlbumNow, envindexinrcpui);
            SetCamLight(AlbumNow);
            if (ESSUI.GetMainStatus() == ESSStatusEnum.EDIT)
                IsLiveCapturing = true;
            else
                IsLiveCapturing = INI.ISLIVECAPTURE;
        }

        ASNForm ASNFRM;
        void ShowASN()
        {
            IsLiveCapturing = false;

            ASNFRM = new ASNForm(ASNCollection, VERSION, OPTION);

            if (ASNFRM.ShowDialog() == DialogResult.OK)
            {
                //RCPUI.SetDISPUIImage(AlbumNow.bmpVIEW);
            }

            IsLiveCapturing = true;
        }

        CPDForm CPDFRM;
        void ShowCPD()
        {
            IsLiveCapturing = false;

            CPDFRM = new CPDForm(AlbumNow.CPD);

            if (CPDFRM.ShowDialog() == DialogResult.OK)
            {
                RCPUI.SetDISPUIImage(AlbumNow.CPD.bmpVIEW);
            }

            IsLiveCapturing = true;
        }

        #endregion

        #region SETUI Action
        private void STPUI_TriggerAction(INIStatusEnum status)
        {
            switch (status)
            {
                case INIStatusEnum.TEST_PAGE_TRAIN:
                    _testPageTrain();
                    break;

                case INIStatusEnum.RECIPE_IMAGE_AND_TRAIN:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_X6:
                            _changeModelBackgroudImage();

                            //if (Universal.IsNoUseCCD)
                            //{
                            //    string strPath3 = RESULT.myResult.GetLastDirPath(Universal.DEBUGSRCPATH);
                            //    CCDCollection.SetDebugPath(RESULT.myResult.LastDirPath);
                            //    CCDCollection.SetDebugEnvPath(0.ToString("000"));
                            //    CCDCollection.SetPageOPType(PageOPTypeEnum.P00.ToString());
                            //    //MyResult_EnvTriggerAction(ResultStatusEnum.CHANGEENVDIRECTORY, 0, PageOPTypeEnum.P00.ToString());
                            //    CCDCollection.GetImage();
                            //}

                            //EnvClass env = AlbumNow.ENVList[0];

                            ////将图片放入测试中
                            //int i = 0;
                            //foreach (PageClass page in env.PageList)
                            //{
                            //    if (Universal.IsNoUseCCD)
                            //        page.SetbmpRUN(PageOPTypeEnum.P00, CCDCollection.GetBMP(i, false));
                            //    else
                            //        page.SetbmpRUN(PageOPTypeEnum.P00, CamActClass.Instance.GetImage(i)); //实测使用  先正常抓图 然后一键更换底图
                            //    i++;
                            //}

                            //bool iscollecttemp = INI.IsCollectPictures;//缓存这个变量

                            //if (INI.IsCollectPictures)
                            //    INI.IsCollectPictures = false;

                            ////开始测试一遍 记录偏移值
                            //AlbumNow.A08_RunProcess(PageOPTypeEnum.P00);

                            //if (iscollecttemp)
                            //    INI.IsCollectPictures = true;

                            ////计算并写入偏移值
                            //foreach (PageClass page in env.PageList)
                            //{
                            //   page.A101_ProcessAnalyzeOffset(PageOPTypeEnum.P00);
                            //}

                            ////更换底图
                            //i = 0;
                            //foreach (PageClass page in env.PageList)
                            //{
                            //    if (Universal.IsNoUseCCD)
                            //        page.SetbmpORG(PageOPTypeEnum.P00, CCDCollection.GetBMP(i, false));
                            //    else
                            //        page.SetbmpORG(PageOPTypeEnum.P00, CamActClass.Instance.GetImage(i)); //实测使用  先正常抓图 然后一键更换底图
                            //    i++;
                            //}

                            //AlbumNow.A00_TrainProcess(true);
                            //AlbumNow.Save();
                            break;
                    }
                    break;
                case INIStatusEnum.CHANGELANGUAGE:

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_SD:
                            //LanguageExClass.Instance.EnumControls(this);
                            break;
                        case OptionEnum.MAIN_X6:

                            break;
                    }

                    break;

                case INIStatusEnum.EDIT:
                    JetEazy.LoggerClass.Instance.WriteLog("点击修改设定");
                    MoveString = "";
                    DISPUI.SetDisplayType(DisplayTypeEnum.ADJUST);
                    ESSUI.Disable = true;

                    break;
                case INIStatusEnum.OK:
                case INIStatusEnum.CANCEL:
                    JetEazy.LoggerClass.Instance.WriteLog("点击设定中的确定或取消");
                    DISPUI.SetDisplayType(DisplayTypeEnum.SHOW);

                    switch (status)
                    {
                        case INIStatusEnum.OK:
                            CCDCollection.SaveCCDLocation();
                            if (INI.ISHIVECLIENT)
                            {
                                if (Universal.JZHIVECLIENT == null)
                                    Universal.JZHIVECLIENT = new JzHiveClass();
                            }
                            INI.Save();
                            if (!Universal.IsNoUseIO)
                                MACHINECollection.SetConfig();
                            break;
                        case INIStatusEnum.CANCEL:
                            CCDCollection.LoadCCDLocation();
                            INI.Load();
                            if (INI.ISHIVECLIENT)
                            {
                                if (Universal.JZHIVECLIENT == null)
                                    Universal.JZHIVECLIENT = new JzHiveClass();
                            }
                            break;
                    }

                    switch (LAYOUT)
                    {
                        case LayoutEnum.L1280X800:
                            STPV1UI.FillDisplay();
                            break;
                        case LayoutEnum.L1440X900:
                            STPUI.ResetChecks();
                            STPUI.FillDisplay();
                            break;
                    }

                    ESSUI.Disable = false;
                    IsLiveCapturing = INI.ISLIVECAPTURE;

                    break;
                case INIStatusEnum.CALIBRATE:
                    if (MessageBox.Show("是否要執行校正抓圖作業?", "SYSTEM", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        JetEazy.LoggerClass.Instance.WriteLog("点击设定中的图像校正");
                        RESULT.StartCalibrate("Local");
                    }
                    break;
                case INIStatusEnum.SETUP_PARA:
                    IsLiveCapturing = false;
                    break;
                case INIStatusEnum.SHOWASSIGN:
                    JetEazy.LoggerClass.Instance.WriteLog("点击设定中的 ASN ");
                    ShowASN();
                    break;
            }
        }
        private void STPUI_TriggerMoveScreen(string movestring)
        {
            MoveString = movestring;
        }
        #endregion

        #region DISPUI Action
        private void DISPUI_MoverAction(MoverOpEnum moverop, string opstring)
        {
            switch (moverop)
            {
                case MoverOpEnum.READYTOMOVE:

                    CCDCollection.BackupList();

                    lblDebug2.Text = CCDCollection.GetRectRelateData();

                    break;
            }


        }
        /// <summary>
        /// 作為調整圖片相關位置的用途
        /// </summary>
        /// <param name="ptfoffset"></param>
        private void DISPUI_AdjustAction(PointF ptfoffset)
        {
            CCDCollection.RestoreList();
            lblDebug.Text = ptfoffset.ToString() + Environment.NewLine + CCDCollection.GetRectRelateData() + Environment.NewLine;

            CCDCollection.MoveCCDRectRelateIndex(new Point((int)ptfoffset.X, (int)ptfoffset.Y), MoveString);
            DISPUI.ReplaceDisplayImage(CCDCollection.bmpAll);

            lblDebug.Text += CCDCollection.GetRectRelateData();
            lblDebug.Invalidate();

        }

        /// <summary>
        /// 使用滑鼠在畫面上截圖 
        /// </summary>
        /// <param name="rectf"></param>
        private void DISPUI_CaptureAction(RectangleF rectf)
        {
            //    AlbumNow.GetBMP(DISPUI.GetOrgBMP(rectf));
            //    RCPUI.SetDISPUIImage(AlbumNow.bmpVIEW);
        }

        #endregion

        #region Result Action
        private void MACHINECollection_TriggerAction(MachineEventEnum machineevent)
        {

            switch (Universal.VERSION)
            {
                case VersionEnum.ALLINONE:

                    switch (Universal.OPTION)
                    {
                        case OptionEnum.MAIN_SDM3:
                            switch (machineevent)
                            {
                                case MachineEventEnum.START:

                                    //不要執行包含在固定參數的參數
                                    if (("," + INI.PRELOADSTATICNO + ",").IndexOf("," + RCPDB.DataNow.No.ToString() + ",") > -1)
                                        MessageBox.Show("Please Check " + RCPDB.DataNow.Name + " ID = " + RCPDB.DataNow.No.ToString() + ".");
                                    else
                                    {
                                        if (CheckAlbumCollection(RCPDB.DataNow.Version, true))
                                        {
                                            RESULT.TestMethod = TestMethodEnum.IO;
                                            RESULT.CalculateSMD2();

                                            if (RUNUI != null)
                                            {
                                                Universal.IsSaveRaw = RUNUI.IsSaveRaw;
                                            }
                                        }
                                    }

                                    break;
                            }
                            break;
                        case OptionEnum.MAIN_SDM2:
                            switch (machineevent)
                            {
                                case MachineEventEnum.START:

                                    //不要執行包含在固定參數的參數
                                    if (("," + INI.PRELOADSTATICNO + ",").IndexOf("," + RCPDB.DataNow.No.ToString() + ",") > -1)
                                        MessageBox.Show("Please Check " + RCPDB.DataNow.Name + " ID = " + RCPDB.DataNow.No.ToString() + ".");
                                    else
                                    {
                                        if (CheckAlbumCollection(RCPDB.DataNow.Version, true))
                                        {
                                            RESULT.TestMethod = TestMethodEnum.IO;
                                            RESULT.CalculateSMD2();

                                            if (RUNUI != null)
                                            {
                                                Universal.IsSaveRaw = RUNUI.IsSaveRaw;
                                            }
                                        }
                                    }

                                    break;
                            }
                            break;
                        case OptionEnum.MAIN_SDM1:
                            switch (machineevent)
                            {
                                case MachineEventEnum.START:

                                    //不要執行包含在固定參數的參數
                                    if (("," + INI.PRELOADSTATICNO + ",").IndexOf("," + RCPDB.DataNow.No.ToString() + ",") > -1)
                                        MessageBox.Show("Please Check " + RCPDB.DataNow.Name + " ID = " + RCPDB.DataNow.No.ToString() + ".");
                                    else
                                    {
                                        if (CheckAlbumCollection(RCPDB.DataNow.Version, true))
                                        {
                                            RESULT.TestMethod = TestMethodEnum.IO;
                                            RESULT.Calculate();

                                            if (RUNUI != null)
                                            {
                                                Universal.IsSaveRaw = RUNUI.IsSaveRaw;
                                            }
                                        }
                                    }

                                    break;
                            }
                            break;
                        case OptionEnum.MAIN:
                        case OptionEnum.MAIN_X6:
                        case JetEazy.OptionEnum.MAIN_SERVICE:
                            switch (machineevent) //加入IO檢測自動測試 四合一 
                            {
                                case MachineEventEnum.AUTOSTART:

                                    switch (Universal.CAMACT)
                                    {
                                        case CameraActionMode.CAM_MOTOR_LINESCAN:
                                        case CameraActionMode.CAM_MOTOR_MODE2:

                                            #region MODE2
                                            //读取步数=0  并且 读取测试完成信号
                                            if (
                                                CamActClass.Instance.StepCurrent == 0 &&
                                                //!((ControlSpace.MachineSpace.JzMainX6MachineClass)MACHINECollection.MACHINE).PLCIO.Busy &&
                                                !Universal.IsRunningTest
                                                )
                                            {
                                                //不要執行包含在固定參數的參數
                                                if (("," + INI.PRELOADSTATICNO + ",").IndexOf("," + RCPDB.DataNow.No.ToString() + ",") > -1)
                                                    MessageBox.Show("Please Check " + RCPDB.DataNow.Name + " ID = " + RCPDB.DataNow.No.ToString() + ".");
                                                else
                                                {
                                                    if (CheckAlbumCollection(RCPDB.DataNow.Version, true))
                                                    {
                                                        RESULT.TestMethod = TestMethodEnum.IO;
                                                        RESULT.Calculate();


                                                        if (RUNUI != null)
                                                        {
                                                            Universal.IsSaveRaw = RUNUI.IsSaveRaw;
                                                        }
                                                    }
                                                }
                                            }

                                            #endregion

                                            break;
                                        default:


                                            //不要執行包含在固定參數的參數
                                            if (("," + INI.PRELOADSTATICNO + ",").IndexOf("," + RCPDB.DataNow.No.ToString() + ",") > -1)
                                                MessageBox.Show("Please Check " + RCPDB.DataNow.Name + " ID = " + RCPDB.DataNow.No.ToString() + ".");
                                            else
                                            {
                                                if (CheckAlbumCollection(RCPDB.DataNow.Version, true))
                                                {
                                                    RESULT.TestMethod = TestMethodEnum.IO;
                                                    RESULT.Calculate();

                                                    if (RUNUI != null)
                                                    {
                                                        Universal.IsSaveRaw = RUNUI.IsSaveRaw;
                                                    }
                                                }
                                            }

                                            break;
                                    }

                                    break;
                            }
                            break;
                    }
                    break;
            }
        }
        private void CCDCollection_TriggerAction(string operationstr)
        {
            switch (VERSION)
            {
                case VersionEnum.AUDIX:

                    if (ESSUI.GetMainStatus() == ESSStatusEnum.RUN)
                    {
                        if (operationstr.IndexOf("EPIX") > -1)
                        {
                            if (CheckAlbumCollection(RCPDB.DataNow.Version, true))
                            {
                                RESULT.TestMethod = TestMethodEnum.CCDTRIGGER;
                                RESULT.Calculate();
                            }
                        }
                    }
                    break;
            }
        }
        private void RESULT_TriggerAction(ResultStatusEnum resultstatus)
        {
            //try
            //{
            switch (resultstatus)
            {
                case ResultStatusEnum.CALSTART:

                    switch (VERSION)
                    {
                        case VersionEnum.AUDIX:
                            switch (OPTION)
                            {
                                case OptionEnum.MAIN:
                                    IsLiveCapturing = false;
                                    RUNUI.InitialResultPanel();
                                    break;
                                case OptionEnum.MAIN_DFLY:
                                    //RUNUI.InitialResultPanel();
                                    break;
                            }
                            break;
                        default:
                            IsLiveCapturing = false;
                            RUNUI.InitialResultPanel();
                            break;
                    }
                    Collect();
                    break;
                case ResultStatusEnum.CALEND:

                    //CCDCollection.GetBmpAll(-2);
                    AlbumWork.CPD.GenRUNVIEWData(ASNCollection);

                    if (AlbumWork.CPD.bmpOCRCheckErr != null && OPTION != OptionEnum.R3 && OPTION != OptionEnum.C3)
                    {
                        Bitmap bmpresult = AlbumWork.CPD.bmpRUNVIEW;
                        Point lo = new Point();
                        lo.X = 20;
                        lo.Y = bmpresult.Height - AlbumWork.CPD.bmpOCRCheckErr.Height;
                        Graphics g = Graphics.FromImage(bmpresult);
                        g.DrawImage(AlbumWork.CPD.bmpOCRCheckErr, lo);

                        if (Universal.isR3ByPass)
                        {
                            lo.X = bmpresult.Width - 1000;
                            lo.Y = bmpresult.Height - 300;
                            Font fonta = new Font(FontFamily.GenericSansSerif, 120, FontStyle.Bold);
                            g.DrawString("SN复判OK", fonta, new SolidBrush(Color.Red), lo);

                        }
                        g.Dispose();
                    }


                    Bitmap bmpShow = new Bitmap(4000, 2060);
                    switch (OPTION)
                    {
                        case OptionEnum.R3:
                            #region R3
                            if (!Universal.R3UI.isTest)
                            {
                                Graphics ggTest = Graphics.FromImage(bmpShow);
                                ggTest.DrawLine(new Pen(Color.Yellow, 5), new Point(0, 0), new Point(bmpShow.Width, bmpShow.Height));
                                ggTest.DrawLine(new Pen(Color.Yellow, 5), new Point(bmpShow.Width, 0), new Point(0, bmpShow.Height));
                                ggTest.Dispose();

                                DISPUI.SetDisplayImage(bmpShow);
                                Universal.R3UI.bmpResult = bmpShow;

                                break;
                            }
                            if (Universal.R3UI.isBYPASS)
                            {
                                RunTime.Store();
                                Graphics ggTest = Graphics.FromImage(bmpShow);

                                Font fonta1 = new Font(FontFamily.GenericSansSerif, 300, FontStyle.Bold);
                                Point p11 = new Point(1300, 100);
                                //     ggTest.DrawString("BYPASS", fonta1, new SolidBrush(Color.White), p11);

                                fonta1 = new Font(FontFamily.GenericSansSerif, 800, FontStyle.Bold);
                                p11 = new Point(500, 600);
                                ggTest.DrawString("PASS", fonta1, new SolidBrush(Color.Lime), p11);

                                ggTest.Dispose();

                                DISPUI.SetDisplayImage(bmpShow);
                                Universal.R3UI.bmpResult = bmpShow;

                                break;
                            }

                            DISPUI.ClearMover();
                            List<RectangleF> mylist = AlbumWork.FillCompoundMoverR3(false);

                            if (Universal.R3UI.bmpC == null)
                                throw new Exception("有图片为null");

                            Bitmap bmp2Temp = (Bitmap)Universal.R3UI.bmpC.Clone();
                            bool isbmp2 = true;
                            if (mylist.Count > 0)
                            {
                                Graphics gc = Graphics.FromImage(bmp2Temp);
                                foreach (RectangleF rectf in mylist)
                                {
                                    Rectangle rect = new Rectangle((int)rectf.X, (int)rectf.Y, (int)rectf.Width, (int)rectf.Height);
                                    Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

                                    if (Universal.R3UI.RectST.Contains(center))
                                    {
                                        gc.DrawRectangle(new Pen(Color.Red, 2), rect);
                                        isbmp2 = false;
                                    }

                                }
                                gc.Dispose();
                            }

                            #region Bmp1
                            Bitmap bmp1 = new Bitmap(4000, 500);
                            Graphics g1 = Graphics.FromImage(bmp1);
                            g1.FillRectangle(new SolidBrush(Color.Black), new RectangleF(0, 0, 4000, 500));


                            Bitmap bmpA = new Bitmap(Universal.R3UI.bmpL, new Size(700, 500));
                            Bitmap bmpB = new Bitmap(Universal.R3UI.bmpR, new Size(700, 500));
                            g1.DrawImage(bmpA, new PointF(0, 0));
                            g1.DrawImage(bmpB, new PointF(700, 0));
                            bmpB.Dispose();
                            bmpA.Dispose();

                            Color color = Color.White;
                            if (!AlbumWork.CPD.mGapResult.ISANGLE && !Universal.isR3ByPass)
                                color = Color.Red;
                            Point p1 = new Point(1500, 50);
                            Font fonta = new Font(FontFamily.GenericSansSerif, 40, FontStyle.Regular);
                            //SizeF s = g1.MeasureString(AlbumWork.CPD.mGapResult.STRANGLE, fonta);
                            //g1.FillRectangle(new SolidBrush(Color.White), p1.X, p1.Y, (int)s.Width, (int)s.Height);
                            g1.DrawString(AlbumWork.CPD.mGapResult.STRANGLE, fonta, new SolidBrush(color), p1);


                            color = Color.White;
                            if (!AlbumWork.CPD.mGapResult.ISRange && !Universal.isR3ByPass)
                                color = Color.Red;
                            p1 = new Point(1500, 150);
                            //   fonta = new Font(FontFamily.GenericSansSerif, 40, FontStyle.Regular);
                            //s = g1.MeasureString(AlbumWork.CPD.mGapResult.STRRange, fonta);
                            //g1.FillRectangle(new SolidBrush(Color.White), p1.X, p1.Y, (int)s.Width, (int)s.Height);
                            g1.DrawString(AlbumWork.CPD.mGapResult.STRRange, fonta, new SolidBrush(color), p1);

                            string strtemp = AlbumWork.CPD.mGapResult.STRRangeLR.Replace(Environment.NewLine, "#");
                            string[] strs = strtemp.Split('#');


                            color = Color.White;
                            if (!AlbumWork.CPD.mGapResult.ISRangeLR && !Universal.isR3ByPass)
                                color = Color.Red;
                            p1 = new Point(1500, 250);
                            //   fonta = new Font(FontFamily.GenericSansSerif, 40, FontStyle.Regular);
                            //s = g1.MeasureString(strs[0], fonta);
                            //g1.FillRectangle(new SolidBrush(Color.White), p1.X, p1.Y, (int)s.Width, (int)s.Height);
                            g1.DrawString(strs[0], fonta, new SolidBrush(color), p1);

                            p1 = new Point(1500, 350);
                            //   fonta = new Font(FontFamily.GenericSansSerif, 40, FontStyle.Regular);
                            //s = g1.MeasureString(strs[1], fonta);
                            //g1.FillRectangle(new SolidBrush(Color.White), p1.X, p1.Y, (int)s.Width, (int)s.Height);
                            g1.DrawString(strs[1], fonta, new SolidBrush(color), p1);

                            Color colorui = Color.Lime;
                            if (!AlbumWork.CPD.mGapResult.ISANGLE && !Universal.isR3ByPass)
                                colorui = Color.Red;
                            if (!AlbumWork.CPD.mGapResult.ISRange && !Universal.isR3ByPass)
                                colorui = Color.Red;
                            if (!AlbumWork.CPD.mGapResult.ISRangeLR && !Universal.isR3ByPass)
                                colorui = Color.Red;

                            g1.FillRectangle(new SolidBrush(colorui), new RectangleF(3000, 0, 1000, 500));

                            fonta = new Font(FontFamily.GenericSansSerif, 80, FontStyle.Bold);
                            p1 = new Point(3200, 20);
                            g1.DrawString("Placement", fonta, new SolidBrush(Color.White), p1);


                            fonta = new Font(FontFamily.GenericSansSerif, 250, FontStyle.Bold);
                            p1 = new Point(3200, 100);
                            if (colorui == Color.Lime)
                                g1.DrawString("OK", fonta, new SolidBrush(Color.White), p1);
                            else
                                g1.DrawString("NG", fonta, new SolidBrush(Color.White), p1);

                            g1.Dispose();
                            #endregion

                            #region Bmp2
                            Bitmap bmp2 = new Bitmap(4000, 500);
                            Graphics g2 = Graphics.FromImage(bmp2);
                            g2.FillRectangle(new SolidBrush(Color.Black), new RectangleF(0, 0, 4000, 500));

                            Bitmap bmpTemp2 = bmp2Temp.Clone(Universal.R3UI.RectST, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                            bmpTemp2 = new Bitmap(bmpTemp2, new Size(3000, 500));
                            g2.DrawImage(bmpTemp2, new PointF(0, 0));
                            bmp2Temp.Dispose();

                            //bool isst = true;

                            //foreach (WorkStatusClass work in Universal.RESULT.myResult.RunStatusCollection.WorkStatusList)
                            //{
                            //    if (work.AnalyzeProcedure == AnanlyzeProcedureEnum.CHECKOCR ||
                            //        work.AnalyzeProcedure == AnanlyzeProcedureEnum.CHECKBARCODE)
                            //    {

                            //        continue;
                            //    }
                            //    if (work.Reason == ReasonEnum.NG)
                            //    {
                            //        isst = false;
                            //        break;
                            //    }

                            //}

                            //if (Universal.RESULT.myResult.RunStatusCollection.NGCOUNT == 1)
                            //{

                            //    if (Universal.R3UI.isSNResult)
                            //    {
                            //        isst = false;
                            //    }
                            //}
                            //else
                            //{
                            //    isst = false;
                            //}
                            colorui = Color.Lime;
                            if (!isbmp2 && !Universal.isR3ByPass)
                                colorui = Color.Red;


                            g2.FillRectangle(new SolidBrush(colorui), new RectangleF(3000, 0, 1000, 500));

                            fonta = new Font(FontFamily.GenericSansSerif, 80, FontStyle.Bold);
                            p1 = new Point(3150, 10);
                            g2.DrawString("Compliancy", fonta, new SolidBrush(Color.White), p1);

                            fonta = new Font(FontFamily.GenericSansSerif, 60, FontStyle.Bold);
                            p1 = new Point(3300, 120);
                            g2.DrawString("Cosmetc", fonta, new SolidBrush(Color.White), p1);

                            fonta = new Font(FontFamily.GenericSansSerif, 250, FontStyle.Bold);
                            p1 = new Point(3200, 160);
                            if (colorui == Color.Lime)
                                g2.DrawString("OK", fonta, new SolidBrush(Color.White), p1);
                            else
                                g2.DrawString("NG", fonta, new SolidBrush(Color.White), p1);
                            g2.Dispose();
                            #endregion

                            #region Bmp3
                            Bitmap bmp3 = new Bitmap(4000, 500);
                            Graphics g3 = Graphics.FromImage(bmp3);
                            g3.FillRectangle(new SolidBrush(Color.Black), new RectangleF(0, 0, 4000, 500));

                            Bitmap bmp3Temp = (Bitmap)Universal.R3UI.bmpC.Clone();
                            bool isbmp3 = true;
                            if (mylist.Count > 0 && !Universal.isR3ByPass)
                            {
                                Graphics gc = Graphics.FromImage(bmp3Temp);
                                foreach (RectangleF rectf in mylist)
                                {
                                    Rectangle rect = new Rectangle((int)rectf.X, (int)rectf.Y, (int)rectf.Width, (int)rectf.Height);
                                    Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

                                    //          if (Universal.R3UI.RectSN.Contains(center))
                                    {
                                        gc.DrawRectangle(new Pen(Color.Red, 2), rect);
                                        isbmp3 = false;
                                    }

                                }
                                gc.Dispose();
                            }

                            // mylist = AlbumWork.FillCompoundMoverR3(true);

                            //Bitmap bmp3Temp = (Bitmap)Universal.R3UI.bmpC.Clone();
                            //if (mylist.Count > 0)
                            //{
                            //    Graphics gc = Graphics.FromImage(bmp3Temp);
                            //    foreach (RectangleF rectf in mylist)
                            //    {
                            //        Rectangle rect = new Rectangle((int)rectf.X, (int)rectf.Y, (int)rectf.Width, (int)rectf.Height);
                            //        gc.DrawRectangle(new Pen(Color.Red, 2), rect);
                            //    }
                            //    gc.Dispose();
                            //}

                            Bitmap bmpTemp3 = bmp3Temp.Clone(Universal.R3UI.RectSN, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                            bmp3Temp.Dispose();

                            if (Universal.R3UI.isCheckBarcodeErr && !Universal.isR3ByPass)
                                bmpTemp3 = Universal.R3UI.bmpBarcodeCHECKERR;

                            bmpTemp3 = new Bitmap(bmpTemp3, new Size(3000, 500));
                            g3.DrawImage(bmpTemp3, new PointF(0, 0));


                            //if (AlbumWork.CPD.bmpOCRCheckErr != null)
                            //{
                            //    g3.DrawImage(AlbumWork.CPD.bmpOCRCheckErr, new PointF(0, 0));

                            //    bmpTemp3 = new Bitmap(bmpTemp3, new Size(3000 - AlbumWork.CPD.bmpOCRCheckErr.Width, 500));
                            //    g3.DrawImage(bmpTemp3, new PointF(AlbumWork.CPD.bmpOCRCheckErr.Width, 0));
                            //}
                            //else
                            //{
                            //    bmpTemp3 = new Bitmap(bmpTemp3, new Size(3000 , 500));
                            //    g3.DrawImage(bmpTemp3, new PointF(0, 0));
                            //}

                            colorui = Color.Lime;
                            //   if (!Universal.R3UI.isSNResult)
                            if (!isbmp3 && !Universal.isR3ByPass)
                                colorui = Color.Red;

                            g3.FillRectangle(new SolidBrush(colorui), new RectangleF(3000, 0, 1000, 500));

                            fonta = new Font(FontFamily.GenericSansSerif, 80, FontStyle.Bold);
                            p1 = new Point(3400, 10);
                            g3.DrawString("SN", fonta, new SolidBrush(Color.White), p1);

                            fonta = new Font(FontFamily.GenericSansSerif, 60, FontStyle.Bold);
                            p1 = new Point(3300, 120);
                            g3.DrawString("Cosmetc", fonta, new SolidBrush(Color.White), p1);

                            fonta = new Font(FontFamily.GenericSansSerif, 250, FontStyle.Bold);
                            p1 = new Point(3200, 160);
                            if (colorui == Color.Lime)
                                g3.DrawString("OK", fonta, new SolidBrush(Color.White), p1);
                            else
                                g3.DrawString("NG", fonta, new SolidBrush(Color.White), p1);
                            g3.Dispose();
                            #endregion

                            #region Bmp4
                            Bitmap bmp4 = new Bitmap(4000, 500);
                            Graphics g4 = Graphics.FromImage(bmp4);
                            g4.FillRectangle(new SolidBrush(Color.Black), new RectangleF(0, 0, 4000, 500));


                            fonta = new Font(FontFamily.GenericSansSerif, 40, FontStyle.Bold);
                            p1 = new Point(300, 10);
                            g4.DrawString("Line:", fonta, new SolidBrush(Color.White), p1);
                            p1 = new Point(1800, 10);
                            g4.DrawString(INI.HIVE_line, fonta, new SolidBrush(Color.White), p1);

                            p1 = new Point(300, 80);
                            g4.DrawString("PROGRAM:", fonta, new SolidBrush(Color.White), p1);
                            p1 = new Point(1800, 80);
                            g4.DrawString(Universal.VersionDate, fonta, new SolidBrush(Color.White), p1);

                            p1 = new Point(300, 150);
                            g4.DrawString("BUILD:", fonta, new SolidBrush(Color.White), p1);
                            p1 = new Point(1800, 150);
                            g4.DrawString(INI.HIVE_line_type, fonta, new SolidBrush(Color.White), p1);

                            p1 = new Point(300, 230);
                            g4.DrawString("TESTTIMER:", fonta, new SolidBrush(Color.White), p1);
                            p1 = new Point(1800, 230);
                            g4.DrawString(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), fonta, new SolidBrush(Color.White), p1);


                            string strbarcode = Universal.R3UI.isSNHaveS ? "S" + JzToolsClass.PassingBarcode : JzToolsClass.PassingBarcode;
                            p1 = new Point(300, 300);
                            g4.DrawString("SFSN:", fonta, new SolidBrush(Color.White), p1);
                            p1 = new Point(1800, 300);
                            g4.DrawString(strbarcode, fonta, new SolidBrush(Color.White), p1);

                            p1 = new Point(300, 370);
                            g4.DrawString("USETIME:", fonta, new SolidBrush(Color.White), p1);
                            p1 = new Point(1800, 370);
                            g4.DrawString(RunTime.StoreSecond(), fonta, new SolidBrush(Color.White), p1);

                            if (Universal.R3UI.Barcode1D != "")
                            {
                                p1 = new Point(300, 440);
                                g4.DrawString("1DBARCODE:", fonta, new SolidBrush(Color.White), p1);
                                p1 = new Point(1800, 440);
                                g4.DrawString(Universal.R3UI.Barcode1D, fonta, new SolidBrush(Color.White), p1);
                            }

                            colorui = Color.Lime;
                            if (!Universal.R3UI.IsPass && !Universal.isR3ByPass)
                                colorui = Color.Red;

                            g4.FillRectangle(new SolidBrush(colorui), new RectangleF(3000, 0, 1000, 500));

                            fonta = new Font(FontFamily.GenericSansSerif, 80, FontStyle.Bold);
                            p1 = new Point(3350, 20);
                            g4.DrawString("UNIT", fonta, new SolidBrush(Color.White), p1);


                            fonta = new Font(FontFamily.GenericSansSerif, 250, FontStyle.Bold);
                            p1 = new Point(3200, 100);
                            if (colorui == Color.Lime)
                                g4.DrawString("OK", fonta, new SolidBrush(Color.White), p1);
                            else
                                g4.DrawString("NG", fonta, new SolidBrush(Color.White), p1);
                            #endregion


                            Graphics gg = Graphics.FromImage(bmpShow);
                            gg.FillRectangle(new SolidBrush(Color.White), new RectangleF(0, 0, 4000, 2100));


                            gg.DrawImage(bmp1, new Point(0, 0));
                            gg.DrawImage(bmp2, new PointF(0, 520));
                            gg.DrawImage(bmp3, new PointF(0, 1040));
                            gg.DrawImage(bmp4, new PointF(0, 1560));

                            if (Universal.isR3ByPass && !Universal.R3UI.IsPass)
                            {
                                Bitmap bmpbypass = new Bitmap(700, 200);
                                Graphics gtemp = Graphics.FromImage(bmpbypass);
                                fonta = new Font(FontFamily.GenericSansSerif, 100, FontStyle.Bold);
                                p1 = new Point(0, 40);
                                gtemp.FillRectangle(new SolidBrush(Color.Blue), new RectangleF(0, 0, bmpbypass.Width, bmpbypass.Height));
                                gtemp.DrawString("复判PASS", fonta, new SolidBrush(Color.Lime), p1);
                                gtemp.Dispose();

                                gg.DrawImage(bmpbypass, new PointF(bmpShow.Width - bmpbypass.Width - 20, bmpShow.Height - bmpbypass.Height - 20));

                                JzToolsClass tools = new JzToolsClass();
                                string strsavedata = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 复判条码:" + strbarcode + Environment.NewLine;
                                string strpath = "D:\\Log\\Retrial.log";

                                if (!System.IO.Directory.Exists("D:\\Log\\"))
                                    System.IO.Directory.CreateDirectory("D:\\Log\\");
                                tools.SaveDataEX(strsavedata, strpath);

                            }

                            gg.Dispose();

                            DISPUI.SetDisplayImage(bmpShow);

                            Universal.R3UI.bmpResult = bmpShow;
                            #endregion
                            break;
                        case OptionEnum.C3:
                            #region C3
                            if (!Universal.C3UI.isTest)
                            {
                                Graphics ggTestC = Graphics.FromImage(bmpShow);
                                ggTestC.DrawLine(new Pen(Color.Yellow, 5), new Point(0, 0), new Point(bmpShow.Width, bmpShow.Height));
                                ggTestC.DrawLine(new Pen(Color.Yellow, 5), new Point(bmpShow.Width, 0), new Point(0, bmpShow.Height));
                                ggTestC.Dispose();

                                DISPUI.SetDisplayImage(bmpShow);
                                Universal.C3UI.bmpResult = bmpShow;

                                break;
                            }
                            if (Universal.C3UI.isBYPASS)
                            {
                                RunTime.Store();
                                Graphics ggTestC = Graphics.FromImage(bmpShow);

                                Font fonta1C = new Font(FontFamily.GenericSansSerif, 300, FontStyle.Bold);
                                Point p11C = new Point(1300, 100);
                                //     ggTest.DrawString("BYPASS", fonta1, new SolidBrush(Color.White), p11);

                                fonta1C = new Font(FontFamily.GenericSansSerif, 800, FontStyle.Bold);
                                p11C = new Point(500, 600);
                                ggTestC.DrawString("PASS", fonta1C, new SolidBrush(Color.Lime), p11C);

                                ggTestC.Dispose();

                                DISPUI.SetDisplayImage(bmpShow);
                                Universal.C3UI.bmpResult = bmpShow;

                                break;
                            }

                            DISPUI.ClearMover();
                            List<RectangleF> mylistC = AlbumWork.FillCompoundMoverR3(false);

                            if (Universal.C3UI.bmpC != null)
                            {
                                //    throw new Exception("有图片为null");

                                Bitmap bmp2TempC = (Bitmap)Universal.C3UI.bmpC.Clone();
                                bool isbmp2C = true;
                                if (mylistC.Count > 0)
                                {
                                    Graphics gc = Graphics.FromImage(bmp2TempC);
                                    foreach (RectangleF rectf in mylistC)
                                    {
                                        Rectangle rect = new Rectangle((int)rectf.X, (int)rectf.Y, (int)rectf.Width, (int)rectf.Height);
                                        Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

                                        if (Universal.C3UI.RectST.Contains(center))
                                        {
                                            gc.DrawRectangle(new Pen(Color.Red, 2), rect);
                                            isbmp2C = false;
                                        }

                                    }
                                    gc.Dispose();
                                }

                                #region Bmp1
                                Bitmap bmp1C = new Bitmap(4000, 500);
                                Graphics g1C = Graphics.FromImage(bmp1C);
                                g1C.FillRectangle(new SolidBrush(Color.Black), new RectangleF(0, 0, 4000, 500));


                                Bitmap bmpAC = new Bitmap(Universal.C3UI.bmpL, new Size(700, 500));
                                Bitmap bmpBC = new Bitmap(Universal.C3UI.bmpR, new Size(700, 500));
                                g1C.DrawImage(bmpAC, new PointF(0, 0));
                                g1C.DrawImage(bmpBC, new PointF(700, 0));
                                bmpBC.Dispose();
                                bmpAC.Dispose();

                                Color colorC = Color.White;
                                if (!AlbumWork.CPD.mGapResult.ISANGLE && !Universal.isC3ByPass)
                                    colorC = Color.Red;
                                Point p1C = new Point(1450, 50);
                                Font fontaC = new Font(FontFamily.GenericSansSerif, 40, FontStyle.Regular);
                                g1C.DrawString(AlbumWork.CPD.mGapResult.STRANGLE, fontaC, new SolidBrush(colorC), p1C);


                                colorC = Color.White;
                                if (!AlbumWork.CPD.mGapResult.isA && !Universal.isC3ByPass)
                                    colorC = Color.Red;
                                p1C = new Point(1450, 150);
                                //   fonta = new Font(FontFamily.GenericSansSerif, 40, FontStyle.Regular);
                                //s = g1.MeasureString(AlbumWork.CPD.mGapResult.STRRange, fonta);
                                //g1.FillRectangle(new SolidBrush(Color.White), p1.X, p1.Y, (int)s.Width, (int)s.Height);
                                g1C.DrawString(AlbumWork.CPD.mGapResult.strA, fontaC, new SolidBrush(colorC), p1C);



                                colorC = Color.White;
                                if (!AlbumWork.CPD.mGapResult.isB && !Universal.isC3ByPass)
                                    colorC = Color.Red;
                                p1C = new Point(1450, 250);
                                //   fonta = new Font(FontFamily.GenericSansSerif, 40, FontStyle.Regular);
                                //s = g1.MeasureString(strs[0], fonta);
                                //g1.FillRectangle(new SolidBrush(Color.White), p1.X, p1.Y, (int)s.Width, (int)s.Height);
                                g1C.DrawString(AlbumWork.CPD.mGapResult.strB, fontaC, new SolidBrush(colorC), p1C);

                                colorC = Color.White;
                                if (!AlbumWork.CPD.mGapResult.isC && !Universal.isC3ByPass)
                                    colorC = Color.Red;
                                p1C = new Point(1450, 350);
                                //   fonta = new Font(FontFamily.GenericSansSerif, 40, FontStyle.Regular);
                                //s = g1.MeasureString(strs[1], fonta);
                                //g1.FillRectangle(new SolidBrush(Color.White), p1.X, p1.Y, (int)s.Width, (int)s.Height);
                                g1C.DrawString(AlbumWork.CPD.mGapResult.strC, fontaC, new SolidBrush(colorC), p1C);

                                colorC = Color.White;
                                if (!AlbumWork.CPD.mGapResult.isD && !Universal.isC3ByPass)
                                    colorC = Color.Red;
                                p1C = new Point(2200, 50);
                                g1C.DrawString(AlbumWork.CPD.mGapResult.strD, fontaC, new SolidBrush(colorC), p1C);

                                colorC = Color.White;
                                if (!AlbumWork.CPD.mGapResult.isE && !Universal.isC3ByPass)
                                    colorC = Color.Red;
                                p1C = new Point(2200, 150);
                                g1C.DrawString(AlbumWork.CPD.mGapResult.strE, fontaC, new SolidBrush(colorC), p1C);

                                colorC = Color.White;
                                if (!AlbumWork.CPD.mGapResult.isF && !Universal.isC3ByPass)
                                    colorC = Color.Red;
                                p1C = new Point(2200, 250);
                                g1C.DrawString(AlbumWork.CPD.mGapResult.strF, fontaC, new SolidBrush(colorC), p1C);

                                colorC = Color.White;
                                if (!AlbumWork.CPD.mGapResult.isG && !Universal.isC3ByPass)
                                    colorC = Color.Red;
                                p1C = new Point(2200, 350);
                                g1C.DrawString(AlbumWork.CPD.mGapResult.strG, fontaC, new SolidBrush(colorC), p1C);

                                Color coloruiC = Color.Lime;
                                if (!AlbumWork.CPD.mGapResult.ISANGLE && !Universal.isC3ByPass)
                                    coloruiC = Color.Red;
                                if (!AlbumWork.CPD.mGapResult.isA && !Universal.isC3ByPass)
                                    coloruiC = Color.Red;
                                if (!AlbumWork.CPD.mGapResult.isB && !Universal.isC3ByPass)
                                    coloruiC = Color.Red;
                                if (!AlbumWork.CPD.mGapResult.isC && !Universal.isC3ByPass)
                                    coloruiC = Color.Red;
                                if (!AlbumWork.CPD.mGapResult.isD && !Universal.isC3ByPass)
                                    coloruiC = Color.Red;
                                if (!AlbumWork.CPD.mGapResult.isE && !Universal.isC3ByPass)
                                    coloruiC = Color.Red;
                                if (!AlbumWork.CPD.mGapResult.isF && !Universal.isC3ByPass)
                                    coloruiC = Color.Red;
                                if (!AlbumWork.CPD.mGapResult.isG && !Universal.isC3ByPass)
                                    coloruiC = Color.Red;


                                g1C.FillRectangle(new SolidBrush(coloruiC), new RectangleF(3000, 0, 1000, 500));

                                fontaC = new Font(FontFamily.GenericSansSerif, 80, FontStyle.Bold);
                                p1C = new Point(3200, 20);
                                g1C.DrawString("Placement", fontaC, new SolidBrush(Color.White), p1C);


                                fontaC = new Font(FontFamily.GenericSansSerif, 250, FontStyle.Bold);
                                p1C = new Point(3200, 100);
                                if (coloruiC == Color.Lime)
                                    g1C.DrawString("OK", fontaC, new SolidBrush(Color.White), p1C);
                                else
                                    g1C.DrawString("NG", fontaC, new SolidBrush(Color.White), p1C);

                                g1C.Dispose();
                                #endregion

                                #region Bmp2
                                Bitmap bmp2C = new Bitmap(4000, 500);
                                Graphics g2C = Graphics.FromImage(bmp2C);
                                g2C.FillRectangle(new SolidBrush(Color.Black), new RectangleF(0, 0, 4000, 500));

                                Bitmap bmpTemp2C = bmp2TempC.Clone(Universal.C3UI.RectST, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                                bmpTemp2C = new Bitmap(bmpTemp2C, new Size(3000, 500));
                                g2C.DrawImage(bmpTemp2C, new PointF(0, 0));
                                bmp2TempC.Dispose();

                                //bool isst = true;

                                //foreach (WorkStatusClass work in Universal.RESULT.myResult.RunStatusCollection.WorkStatusList)
                                //{
                                //    if (work.AnalyzeProcedure == AnanlyzeProcedureEnum.CHECKOCR ||
                                //        work.AnalyzeProcedure == AnanlyzeProcedureEnum.CHECKBARCODE)
                                //    {

                                //        continue;
                                //    }
                                //    if (work.Reason == ReasonEnum.NG)
                                //    {
                                //        isst = false;
                                //        break;
                                //    }

                                //}

                                //if (Universal.RESULT.myResult.RunStatusCollection.NGCOUNT == 1)
                                //{

                                //    if (Universal.C3UI.isSNResult)
                                //    {
                                //        isst = false;
                                //    }
                                //}
                                //else
                                //{
                                //    isst = false;
                                //}
                                coloruiC = Color.Lime;
                                if (!isbmp2C && !Universal.isC3ByPass)
                                    coloruiC = Color.Red;


                                g2C.FillRectangle(new SolidBrush(coloruiC), new RectangleF(3000, 0, 1000, 500));

                                fontaC = new Font(FontFamily.GenericSansSerif, 80, FontStyle.Bold);
                                p1C = new Point(3150, 10);
                                g2C.DrawString("Compliancy", fontaC, new SolidBrush(Color.White), p1C);

                                fontaC = new Font(FontFamily.GenericSansSerif, 60, FontStyle.Bold);
                                p1C = new Point(3300, 120);
                                g2C.DrawString("Cosmetc", fontaC, new SolidBrush(Color.White), p1C);

                                fontaC = new Font(FontFamily.GenericSansSerif, 250, FontStyle.Bold);
                                p1C = new Point(3200, 160);
                                if (coloruiC == Color.Lime)
                                    g2C.DrawString("OK", fontaC, new SolidBrush(Color.White), p1C);
                                else
                                    g2C.DrawString("NG", fontaC, new SolidBrush(Color.White), p1C);
                                g2C.Dispose();
                                #endregion

                                #region Bmp3
                                Bitmap bmp3C = new Bitmap(4000, 500);
                                Graphics g3C = Graphics.FromImage(bmp3C);
                                g3C.FillRectangle(new SolidBrush(Color.Black), new RectangleF(0, 0, 4000, 500));

                                Bitmap bmp3TempC = (Bitmap)Universal.C3UI.bmpC.Clone();
                                bool isbmp3C = true;
                                if (mylistC.Count > 0 && !Universal.isC3ByPass)
                                {
                                    Graphics gc = Graphics.FromImage(bmp3TempC);
                                    foreach (RectangleF rectf in mylistC)
                                    {
                                        Rectangle rect = new Rectangle((int)rectf.X, (int)rectf.Y, (int)rectf.Width, (int)rectf.Height);
                                        Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

                                        //          if (Universal.C3UI.RectSN.Contains(center))
                                        {
                                            gc.DrawRectangle(new Pen(Color.Red, 2), rect);
                                            isbmp3C = false;
                                        }

                                    }
                                    gc.Dispose();
                                }

                                // mylist = AlbumWork.FillCompoundMoverR3(true);

                                //Bitmap bmp3Temp = (Bitmap)Universal.C3UI.bmpC.Clone();
                                //if (mylist.Count > 0)
                                //{
                                //    Graphics gc = Graphics.FromImage(bmp3Temp);
                                //    foreach (RectangleF rectf in mylist)
                                //    {
                                //        Rectangle rect = new Rectangle((int)rectf.X, (int)rectf.Y, (int)rectf.Width, (int)rectf.Height);
                                //        gc.DrawRectangle(new Pen(Color.Red, 2), rect);
                                //    }
                                //    gc.Dispose();
                                //}

                                Bitmap bmpTemp3C = bmp3TempC.Clone(Universal.C3UI.RectSN, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                                bmp3TempC.Dispose();

                                if (Universal.C3UI.isCheckBarcodeErr && !Universal.isC3ByPass)
                                    bmpTemp3C = Universal.C3UI.bmpBarcodeCHECKERR;

                                bmpTemp3C = new Bitmap(bmpTemp3C, new Size(3000, 500));
                                g3C.DrawImage(bmpTemp3C, new PointF(0, 0));


                                //if (AlbumWork.CPD.bmpOCRCheckErr != null)
                                //{
                                //    g3.DrawImage(AlbumWork.CPD.bmpOCRCheckErr, new PointF(0, 0));

                                //    bmpTemp3 = new Bitmap(bmpTemp3, new Size(3000 - AlbumWork.CPD.bmpOCRCheckErr.Width, 500));
                                //    g3.DrawImage(bmpTemp3, new PointF(AlbumWork.CPD.bmpOCRCheckErr.Width, 0));
                                //}
                                //else
                                //{
                                //    bmpTemp3 = new Bitmap(bmpTemp3, new Size(3000 , 500));
                                //    g3.DrawImage(bmpTemp3, new PointF(0, 0));
                                //}

                                coloruiC = Color.Lime;
                                //   if (!Universal.C3UI.isSNResult)
                                if (!isbmp3C && !Universal.isC3ByPass)
                                    coloruiC = Color.Red;

                                g3C.FillRectangle(new SolidBrush(coloruiC), new RectangleF(3000, 0, 1000, 500));

                                fontaC = new Font(FontFamily.GenericSansSerif, 80, FontStyle.Bold);
                                p1C = new Point(3400, 10);
                                g3C.DrawString("SN", fontaC, new SolidBrush(Color.White), p1C);

                                fontaC = new Font(FontFamily.GenericSansSerif, 60, FontStyle.Bold);
                                p1C = new Point(3300, 120);
                                g3C.DrawString("Cosmetc", fontaC, new SolidBrush(Color.White), p1C);

                                fontaC = new Font(FontFamily.GenericSansSerif, 250, FontStyle.Bold);
                                p1C = new Point(3200, 160);
                                if (coloruiC == Color.Lime)
                                    g3C.DrawString("OK", fontaC, new SolidBrush(Color.White), p1C);
                                else
                                    g3C.DrawString("NG", fontaC, new SolidBrush(Color.White), p1C);
                                g3C.Dispose();
                                #endregion

                                #region Bmp4
                                Bitmap bmp4C = new Bitmap(4000, 500);
                                Graphics g4C = Graphics.FromImage(bmp4C);
                                g4C.FillRectangle(new SolidBrush(Color.Black), new RectangleF(0, 0, 4000, 500));


                                fontaC = new Font(FontFamily.GenericSansSerif, 40, FontStyle.Bold);
                                p1C = new Point(300, 10);
                                g4C.DrawString("Line:", fontaC, new SolidBrush(Color.White), p1C);
                                p1C = new Point(1800, 10);
                                g4C.DrawString(INI.HIVE_line, fontaC, new SolidBrush(Color.White), p1C);

                                p1C = new Point(300, 80);
                                g4C.DrawString("PROGRAM:", fontaC, new SolidBrush(Color.White), p1C);
                                p1C = new Point(1800, 80);
                                g4C.DrawString(Universal.VersionDate, fontaC, new SolidBrush(Color.White), p1C);

                                p1C = new Point(300, 150);
                                g4C.DrawString("BUILD:", fontaC, new SolidBrush(Color.White), p1C);
                                p1C = new Point(1800, 150);
                                g4C.DrawString(INI.HIVE_line_type, fontaC, new SolidBrush(Color.White), p1C);

                                p1C = new Point(300, 230);
                                g4C.DrawString("TESTTIMER:", fontaC, new SolidBrush(Color.White), p1C);
                                p1C = new Point(1800, 230);
                                g4C.DrawString(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), fontaC, new SolidBrush(Color.White), p1C);


                                string strbarcodeC = Universal.C3UI.isSNHaveS ? "S" + JzToolsClass.PassingBarcode : JzToolsClass.PassingBarcode;
                                p1C = new Point(300, 300);
                                g4C.DrawString("SFSN:", fontaC, new SolidBrush(Color.White), p1C);
                                p1C = new Point(1800, 300);
                                g4C.DrawString(strbarcodeC, fontaC, new SolidBrush(Color.White), p1C);

                                p1C = new Point(300, 370);
                                g4C.DrawString("USETIME:", fontaC, new SolidBrush(Color.White), p1C);
                                p1C = new Point(1800, 370);
                                g4C.DrawString(RunTime.StoreSecond(), fontaC, new SolidBrush(Color.White), p1C);

                                if (Universal.C3UI.Barcode1D != "")
                                {
                                    p1C = new Point(300, 440);
                                    g4C.DrawString("1DBARCODE:", fontaC, new SolidBrush(Color.White), p1C);
                                    p1C = new Point(1800, 440);
                                    g4C.DrawString(Universal.C3UI.Barcode1D, fontaC, new SolidBrush(Color.White), p1C);
                                }

                                coloruiC = Color.Lime;
                                if (!Universal.C3UI.IsPass && !Universal.isC3ByPass)
                                    coloruiC = Color.Red;

                                g4C.FillRectangle(new SolidBrush(coloruiC), new RectangleF(3000, 0, 1000, 500));

                                fontaC = new Font(FontFamily.GenericSansSerif, 80, FontStyle.Bold);
                                p1C = new Point(3350, 20);
                                g4C.DrawString("UNIT", fontaC, new SolidBrush(Color.White), p1C);


                                fontaC = new Font(FontFamily.GenericSansSerif, 250, FontStyle.Bold);
                                p1C = new Point(3200, 100);
                                if (coloruiC == Color.Lime)
                                    g4C.DrawString("OK", fontaC, new SolidBrush(Color.White), p1C);
                                else
                                    g4C.DrawString("NG", fontaC, new SolidBrush(Color.White), p1C);
                                #endregion


                                if (Universal.C3UI.bmpLabel != null)
                                    bmpShow = new Bitmap(bmpShow.Width + Universal.C3UI.bmpLabel.Width, bmpShow.Height);
                                Graphics ggC = Graphics.FromImage(bmpShow);
                                ggC.FillRectangle(new SolidBrush(Color.White), new RectangleF(0, 0, 4000, 2100));

                                ggC.DrawImage(bmp1C, new Point(0, 0));
                                ggC.DrawImage(bmp2C, new PointF(0, 520));
                                ggC.DrawImage(bmp3C, new PointF(0, 1040));
                                ggC.DrawImage(bmp4C, new PointF(0, 1560));

                                if (Universal.C3UI.bmpLabel != null)
                                    ggC.DrawImage(Universal.C3UI.bmpLabel, new Point(bmpShow.Width - Universal.C3UI.bmpLabel.Width, 0));

                                if (Universal.isC3ByPass && !Universal.C3UI.IsPass)
                                {
                                    Bitmap bmpbypass = new Bitmap(700, 200);
                                    Graphics gtemp = Graphics.FromImage(bmpbypass);
                                    fontaC = new Font(FontFamily.GenericSansSerif, 100, FontStyle.Bold);
                                    p1C = new Point(0, 40);
                                    gtemp.FillRectangle(new SolidBrush(Color.Blue), new RectangleF(0, 0, bmpbypass.Width, bmpbypass.Height));
                                    gtemp.DrawString("复判PASS", fontaC, new SolidBrush(Color.Lime), p1C);
                                    gtemp.Dispose();

                                    ggC.DrawImage(bmpbypass, new PointF(bmpShow.Width - bmpbypass.Width - 20, bmpShow.Height - bmpbypass.Height - 20));

                                    JzToolsClass tools = new JzToolsClass();
                                    string strsavedata = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 复判条码:" + strbarcodeC + Environment.NewLine;
                                    string strpath = "D:\\Log\\Retrial.log";

                                    if (!System.IO.Directory.Exists("D:\\Log\\"))
                                        System.IO.Directory.CreateDirectory("D:\\Log\\");
                                    tools.SaveDataEX(strsavedata, strpath);

                                }

                                ggC.Dispose();
                            }

                            DISPUI.SetDisplayImage(bmpShow);

                            Universal.C3UI.bmpResult = bmpShow;
                            #endregion
                            break;
                        case OptionEnum.MAIN_X6:
                        case JetEazy.OptionEnum.MAIN_SERVICE:
                            if (!INI.IsOnlyShowCurrentImage)
                            {
                                //Bitmap bmpshowbarcode = _getMainX6ShowBarcode(AlbumWork.CPD.bmpRUNVIEW);
                                DISPUI.SetDisplayImage(AlbumWork.CPD.bmpRUNVIEW);
                            }
                            break;
                        case OptionEnum.MAIN_SDM2:
                        case OptionEnum.MAIN_SDM3:
                            break;
                        default:
                            DISPUI.SetDisplayImage(AlbumWork.CPD.bmpRUNVIEW);
                            break;
                    }

                    //     bmpShow.Dispose();

                    if (OPTION != OptionEnum.R3 && OPTION != OptionEnum.C3)
                    {
                        if (OPTION == OptionEnum.MAIN_X6)
                        {
                            if (!INI.IsOnlyShowCurrentImage)
                            {
                                DISPUIShowResult();
                            }
                        }
                        else
                        {
                            DISPUIShowResult();
                        }
                    }

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_SDM1:

                            string myReportStr = "Name(mil),LEFT_Min,LEFT_Max,TOP_Min,TOP_Max,RIGHT_Min,RIGHT_Max,BOTTOM_Min,BOTTOM_Max,";
                            myReportStr = "Name(mm),LEFT_Min,LEFT_Max,TOP_Min,TOP_Max,RIGHT_Min,RIGHT_Max,BOTTOM_Min,BOTTOM_Max," + Environment.NewLine;

                            int RestoreSeq = Smoothen();

                            int envindex = 0;
                            int pageindex = 0;
                            int analyzeindex = 0;
                            foreach (EnvClass env in AlbumWork.ENVList)
                            {
                                foreach (PageClass page in env.PageList)
                                {
                                    foreach (AnalyzeClass analyze in page.AnalyzeRoot.BranchList)
                                    {
                                        if (analyze.PADPara.PADMethod == PADMethodEnum.GLUECHECK)
                                        {
                                            analyze.CalculateChipWidth();
                                            if (INI.CHIP_ISSMOOTHEN)
                                            {
                                                if (RestoreSeq < 0)
                                                    analyze.BackupData();
                                                else
                                                    analyze.RestoreData(RestoreSeq);
                                            }
                                        }
                                        analyzeindex++;
                                    }
                                    pageindex++;
                                }
                                envindex++;
                            }

                            List<string> list = new List<string>();
                            list.Clear();
                            Bitmap mySDM1 = new Bitmap(AlbumWork.CPD.bmpRUNVIEW);
                            Graphics graphics = Graphics.FromImage(mySDM1);
                            foreach (EnvClass env in AlbumWork.ENVList)
                            {
                                foreach (PageClass page in env.PageList)
                                {
                                    foreach (AnalyzeClass analyze in page.AnalyzeRoot.BranchList)
                                    {
                                        if (analyze.PADPara.PADMethod == PADMethodEnum.GLUECHECK)
                                        {
                                            list.Add("'" + analyze.AliasName + "," + analyze.ToReportString1());
                                            //list.Add("'" + analyze.AliasName + "," + analyze.PADPara.ToChipReportingStr3());
                                            //myReportStr += analyze.AliasName + analyze.PADPara.ToChipReportingStr() + Environment.NewLine;
                                        }

                                        if (analyze.IsVeryGood)
                                            continue;
                                        PointF ptloc = analyze.myDrawAnalyzeStrRectF.Location;

                                        JzToolsClass jzToolsClass = new JzToolsClass();
                                        Rectangle NGRectFill = jzToolsClass.SimpleRect(ptloc, 20);
                                        Color NGColor = Color.Red;
                                        if (analyze.PADPara.DescStr.Contains("无胶"))
                                        {
                                            NGColor = Color.Purple;
                                        }
                                        else if (analyze.PADPara.DescStr.Contains("尺寸"))
                                        {
                                            NGColor = Color.Yellow;
                                        }
                                        else if (analyze.PADPara.DescStr.Contains("溢胶"))
                                        {
                                            NGColor = Color.Orange;
                                        }
                                        graphics.FillRectangle(new SolidBrush(NGColor), NGRectFill);
                                        //graphics.DrawString(analyze.PADPara.DescStr + "(" + analyze.ToAnalyzeString() + ")", new Font("宋体", 18), Brushes.Red, ptloc);
                                        ptloc.Y += 28;
                                        graphics.DrawString(analyze.PADPara.DescStr, new Font("宋体", 18), Brushes.Red, ptloc);
                                        ptloc.Y += 28;
                                        graphics.DrawString("(" + analyze.ToAnalyzeString() + ")", new Font("宋体", 18), Brushes.Red, ptloc);


                                    }
                                }
                            }
                            graphics.Dispose();
                            DISPUI.SetDisplayImage(mySDM1);
                            mySDM1.Dispose();

                            string myChipReportPath = "D:\\REPORT\\(CHIP)FORMAT01\\" + JzTimes.DateSerialString;
                            if (!System.IO.Directory.Exists(myChipReportPath))
                                System.IO.Directory.CreateDirectory(myChipReportPath);

                            list.Sort();
                            foreach (string strss in list)
                            {
                                myReportStr += strss + Environment.NewLine;
                            }

                            System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(myChipReportPath + "\\" + JzTimes.DateTimeSerialString + ".csv");
                            streamWriter.WriteLine(myReportStr);
                            streamWriter.Close();
                            streamWriter.Dispose();
                            streamWriter = null;

                            break;
                        case OptionEnum.MAIN_X6:

                            if (!INI.IsOnlyShowCurrentImage)
                            {
                                Bitmap bmpshowbarcode = _getMainX6ShowBarcode(AlbumWork.CPD.bmpRUNVIEW);
                                DISPUI.SetDisplayImage(bmpshowbarcode);
                            }

                            break;
                    }

                    AlbumWork.RecodeRepoer();
                    STPUI.ShowRecodeData();

                    switch (VERSION)
                    {
                        case VersionEnum.AUDIX:
                            switch (OPTION)
                            {
                                case OptionEnum.MAIN:
                                    //IsLiveCapturing = true;
                                    RUNUI.ShowResult(RESULT.RunstatusCollection);
                                    break;
                                case OptionEnum.MAIN_DFLY:
                                    //RUNUI.InitialResultPanel();
                                    break;
                                case OptionEnum.R3:

                                    break;
                                case OptionEnum.C3:

                                    break;

                            }
                            break;
                        default:
                            RUNUI.ShowResult(RESULT.RunstatusCollection);
                            break;
                    }

                    #region 保存界面显示的图片

                    switch (Universal.VERSION)
                    {
                        case VersionEnum.ALLINONE:

                            Bitmap _showResultBmp = new Bitmap(DISPUI.GetScreen());

                            switch (Universal.OPTION)
                            {
                                case OptionEnum.MAIN_X6:

                                    if (INI.IsCollectStripPictures)
                                    {
                                        if (!ispass)
                                        {
                                            Task task = new Task(() =>
                                            {
                                                try
                                                {
                                                    string _imagePath = "D:\\REPORT\\work\\Image\\auto_" + JzMainSDPositionParas.Report_LOT + "\\" + JzMainSDPositionParas.INSPECT_NGINDEX.ToString("00000");
                                                    _imagePath = "D:\\REPORT\\work\\Image\\" + JzTimes.DateSerialString + "\\auto_" + JzMainSDPositionParas.Report_LOT + "\\" + JzMainSDPositionParas.INSPECT_NGINDEX.ToString("00000");

                                                    if (!System.IO.Directory.Exists(_imagePath + "\\000"))
                                                        System.IO.Directory.CreateDirectory(_imagePath + "\\000");

                                                    _showResultBmp.Save(_imagePath + "\\000\\Result" + ".jpg",
                                                                                            System.Drawing.Imaging.ImageFormat.Jpeg);
                                                }
                                                catch (Exception ex)
                                                {
                                                    JetEazy.LoggerClass.Instance.WriteException(ex);
                                                }
                                            });
                                            task.Start();

                                            //string _imagePath = "D:\\REPORT\\work\\Image\\auto_" + JzMainSDPositionParas.Report_LOT + "\\" + JzMainSDPositionParas.INSPECT_NGINDEX.ToString("00000");
                                            //_imagePath = "D:\\REPORT\\work\\Image\\" + JzTimes.DateSerialString + "\\auto_" + JzMainSDPositionParas.Report_LOT + "\\" + JzMainSDPositionParas.INSPECT_NGINDEX.ToString("00000");

                                            //if (!System.IO.Directory.Exists(_imagePath + "\\000"))
                                            //    System.IO.Directory.CreateDirectory(_imagePath + "\\000");

                                            //_showResultBmp.Save(_imagePath + "\\000\\Result" + ".jpg",
                                            //                                        System.Drawing.Imaging.ImageFormat.Jpeg);
                                        }
                                    }

                                    break;


                                case OptionEnum.R32:
                                    if (INI.ISQSMCALLSAVE)
                                    {
                                        //ADD Gaara
                                        Allinone.OPSpace.ResultSpace.JzR32ResultClass jzr32result = (Allinone.OPSpace.ResultSpace.JzR32ResultClass)RESULT.myResult;
                                        //           jzr32result.SaveResultScreen(_showResultBmp);

                                        jzr32result.ThreadForSavePictures(_showResultBmp);
                                    }
                                    break;
                                case OptionEnum.R26:
                                    //if (INI.ISQSMCALLSAVE)
                                    //{
                                    //    //ADD Gaara
                                    //    Allinone.OPSpace.ResultSpace.JzRXXResultClass jzr26result = (Allinone.OPSpace.ResultSpace.JzRXXResultClass)RESULT.myResult;
                                    //    jzr26result.SavePrintScreen();
                                    //}
                                    break;
                                case OptionEnum.R15:
                                    //if (INI.ISQSMCALLSAVE)
                                    //{
                                    //    //ADD Gaara
                                    //    Allinone.OPSpace.ResultSpace.JzR15ResultClass jzr15result = (Allinone.OPSpace.ResultSpace.JzR15ResultClass)RESULT.myResult;
                                    //    jzr15result.SavePrintScreen();
                                    //}
                                    break;
                                case OptionEnum.R9:

                                    INI.ISQSMCALLSAVE = true;
                                    if (INI.ISQSMCALLSAVE)
                                    {
                                        //ADD Gaara
                                        Allinone.OPSpace.ResultSpace.JzR9ResultClass jzr9result = (Allinone.OPSpace.ResultSpace.JzR9ResultClass)RESULT.myResult;
                                        //     jzr9result.SaveResultScreen(_showResultBmp);

                                        jzr9result.ThreadForSavePictures(_showResultBmp);
                                    }
                                    break;
                                case OptionEnum.R5:

                                    INI.ISQSMCALLSAVE = true;
                                    if (INI.ISQSMCALLSAVE)
                                    {
                                        //ADD Gaara
                                        Allinone.OPSpace.ResultSpace.JzR5ResultClass jzr5result = (Allinone.OPSpace.ResultSpace.JzR5ResultClass)RESULT.myResult;
                                        //     jzr9result.SaveResultScreen(_showResultBmp);

                                        jzr5result.ThreadForSavePictures(_showResultBmp);
                                    }
                                    break;
                                case OptionEnum.R1:

                                    INI.ISQSMCALLSAVE = true;
                                    if (INI.ISQSMCALLSAVE)
                                    {
                                        //ADD Gaara
                                        Allinone.OPSpace.ResultSpace.JzR1ResultClass jzr1result = (Allinone.OPSpace.ResultSpace.JzR1ResultClass)RESULT.myResult;
                                        jzr1result.ThreadForSavePictures(_showResultBmp);
                                    }
                                    break;
                                case OptionEnum.R3:

                                    //INI.ISQSMCALLSAVE = true;
                                    //if (INI.ISQSMCALLSAVE)
                                    //{
                                    //    //ADD Gaara
                                    //    Allinone.OPSpace.ResultSpace.JzR3ResultClass jzr3result = (Allinone.OPSpace.ResultSpace.JzR3ResultClass)RESULT.myResult;
                                    //    jzr3result.SavePrintScreen();
                                    //}
                                    break;
                                case OptionEnum.C3:

                                    //INI.ISQSMCALLSAVE = true;
                                    //if (INI.ISQSMCALLSAVE)
                                    //{
                                    //    //ADD Gaara
                                    //    Allinone.OPSpace.ResultSpace.JzC3ResultClass jzC3result = (Allinone.OPSpace.ResultSpace.JzC3ResultClass)RESULT.myResult;
                                    //    jzC3result.SavePrintScreen();
                                    //}
                                    break;
                            }
                            break;
                    }

                    #endregion

                    GetStatus();

                    if (IsDebug)
                        RESULT.RunstatusCollection.SaveProcessAndError(Universal.TESTPATH);
                    if (Universal.isAutoDebug)
                        AutoDebug();
                    string strmess = JzToolsClass.PassingBarcode + "," + Universal.OCRSN;
                    {
                        if (JzToolsClass.PassingBarcode == Universal.OCRSN)
                            strmess += ",PASS";
                        else
                            strmess += ",FAIL";
                    }
                    JetEazy.LoggerClass.Instance.WriteSNReport(strmess);

                    if (JzToolsClass.PassingBarcode != Universal.OCRSN && Universal.ISCHECKSN)
                        JetEazy.LoggerClass.Instance.WriteSNReportFail(strmess);

                    if (Universal.OLDBARCODE != JzToolsClass.PassingBarcode)
                    {
                        INI.SETBCCOUNT(Universal.ISBCNG);
                        ESSUI.FillBCCOUNT(INI.ALLCOUNT, INI.BCNGCOUNT);
                    }
                    Universal.OLDBARCODE = JzToolsClass.PassingBarcode;
                    if (Universal.OPTION != OptionEnum.R3 && Universal.OPTION != OptionEnum.C3)
                        Universal.OnR3TickStop("2");




                    break;
                case ResultStatusEnum.COUNTSTART:
                    RunTime.Cut();

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_SDM2:
                        case OptionEnum.MAIN_SDM3:
                            tabCtrl.SelectedIndex = 2;

                            AlbumNow.m_EnvNow.ResetMapping();
                            MappingUI.SetBinString(String.Join(",", AlbumNow.m_EnvNow.DrawMapping));
                            MappingUI.DrawMap();
                            SetTrayMapping();

                            break;
                    }

                    break;
                case ResultStatusEnum.COUNT_MAPPING:

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_SDM2:
                        case OptionEnum.MAIN_SDM3:
                            tabCtrl.SelectedIndex = 0;

                            INI.SaveDataRecord();
                            //lblMappingDataString.Text = INI.GetDataResultString();
                            ShowLabelMappingDataString();
                            MappingUI.SetBinString(String.Join(",", AlbumNow.m_EnvNow.DrawMapping));
                            MappingUI.DrawMap();
                            SetTrayMapping();

                            Task task = new Task(() =>
                            {
                                try
                                {
                                    if (Universal.IsNoUseIO)
                                    {
                                        MappingUI.bmpTray.Save(Universal.CalTestPath + "\\" + Universal.CalTestBarcode + ".jpg",
                                               System.Drawing.Imaging.ImageFormat.Jpeg);
                                    }
                                    else
                                    {
                                        string _mapping_path = @"D:\COLLECT_MAPPING\" + JzTimes.DateSerialString;
                                        if (!System.IO.Directory.Exists(_mapping_path))
                                            System.IO.Directory.CreateDirectory(_mapping_path);

                                        MappingUI.bmpTray.Save(_mapping_path + "\\" + Universal.CalTestBarcode + ".jpg",
                                               System.Drawing.Imaging.ImageFormat.Jpeg);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    JetEazy.LoggerClass.Instance.WriteException(ex);
                                }
                            });
                            task.Start();

                            //if (Universal.IsNoUseIO)
                            //{
                            //    MappingUI.bmpTray.Save(Universal.CalTestPath + "\\" + Universal.CalTestBarcode + ".jpg",
                            //           System.Drawing.Imaging.ImageFormat.Jpeg);
                            //}
                            //else
                            //{
                            //    string _mapping_path = @"D:\COLLECT_MAPPING\" + JzTimes.DateSerialString;
                            //    if (!System.IO.Directory.Exists(_mapping_path))
                            //        System.IO.Directory.CreateDirectory(_mapping_path);

                            //    MappingUI.bmpTray.Save(_mapping_path + "\\" + Universal.CalTestBarcode + ".jpg",
                            //           System.Drawing.Imaging.ImageFormat.Jpeg);
                            //}

                            //TrayMappingInit();

                            break;
                    }

                    break;
                case ResultStatusEnum.SHOW_CURRENT_IMAGE:

                    switch (Universal.VERSION)
                    {
                        case VersionEnum.ALLINONE:
                            switch (Universal.OPTION)
                            {
                                case OptionEnum.MAIN_SDM3:
                                case OptionEnum.MAIN_SDM2:
                                    //SUPERSHOWSDM2UI.SetImage(CCDCollection.GetBMP(0, false));

                                    SUPERSHOWSDM2UI.SetImage(Universal.SDM2_BMP_SHOW_CURRENT);
                                    //  SUPERSHOWSDM2UI.BeginInvoke(new EventHandler(delegate
                                    //  /*{*/
                                    //SUPERSHOWSDM2UI.SetImage(Universal.SDM2_BMP_SHOW_CURRENT);

                                    // }));
                                    //pnlMappingUI.BackgroundImage = (Bitmap)Universal.SDM2_BMP_SHOW_CURRENT.Clone();

                                    break;
                            }
                            break;
                    }

                    break;
                //case ResultStatusEnum.GETIMAGECOMPLETE:

                //    switch(OPTION)
                //    {
                //        case OptionEnum.MAIN_SD:

                //            //((Allinone.ControlSpace.MachineSpace.JzMainSDMachineClass)Universal.MACHINECollection.MACHINE).PLCIO.PCGetImageComplete = true;
                //            ((Allinone.ControlSpace.MachineSpace.JzMainSDMachineClass)Universal.MACHINECollection.MACHINE).IsGetImageComplete = true;
                //            break;
                //    }

                //    break;
                case ResultStatusEnum.COUNTEND:
                    RunTime.Store();

                    if (RUNUI != null)
                        RUNUI.SetDuriation(RunTime.StoreSecond());

                    if (RUNV1UI != null)
                        RUNV1UI.SetDuriation(RunTime.StoreSecond());

                    break;
                case ResultStatusEnum.CALNG:
                    ispass = false;
                    switch (Universal.VERSION)
                    {
                        case VersionEnum.ALLINONE:

                            switch (Universal.OPTION)
                            {
                                case OptionEnum.MAIN_SDM3:
                                case OptionEnum.MAIN_SDM2:
                                case OptionEnum.MAIN_SDM1:
                                case OptionEnum.MAIN_X6:
                                case OptionEnum.MAIN_SD:
                                case OptionEnum.MAIN:
                                case JetEazy.OptionEnum.MAIN_SERVICE:

                                    ESSUI.SetNG();

                                    break;
                                default:

                                    bool ISADD = true;
                                    if (Universal.FAILBARCODE == JzToolsClass.PassingBarcode)
                                        ISADD = false;
                                    else
                                        Universal.FAILBARCODE = JzToolsClass.PassingBarcode;

                                    ESSUI.SetNG(ISADD);

                                    break;
                            }

                            break;
                    }

                    if (RUNUI != null)
                        RUNUI.StartShinnig(false);

                    if (RUNV1UI != null)
                        RUNV1UI.StartShinnig(false);

                    CGOperate();

                    break;
                case ResultStatusEnum.CALPASS:
                    ispass = true;
                    switch (Universal.VERSION)
                    {
                        case VersionEnum.ALLINONE:

                            switch (Universal.OPTION)
                            {
                                case OptionEnum.MAIN_SDM3:
                                case OptionEnum.MAIN_SDM2:
                                case OptionEnum.MAIN_SDM1:
                                case OptionEnum.MAIN_X6:
                                case OptionEnum.MAIN_SD:
                                case OptionEnum.MAIN:
                                case JetEazy.OptionEnum.MAIN_SERVICE:

                                    ESSUI.SetPass();

                                    switch (Universal.OPTION)
                                    {
                                        case OptionEnum.MAIN_X6:
                                            if (INI.JCET_IS_USE_SHOPFLOOR)
                                            {
                                                RUNUI.AddInspectCurrentStrip(true);
                                            }
                                            break;
                                    }

                                    break;
                                default:

                                    if (Universal.FAILBARCODE == JzToolsClass.PassingBarcode)
                                        ESSUI.SetPass(true);
                                    else
                                        ESSUI.SetPass();

                                    break;
                            }

                            break;
                    }

                    if (RUNUI != null)
                        RUNUI.StartShinnig(true);

                    if (RUNV1UI != null)
                        RUNV1UI.StartShinnig(true);

                    CGOperate();

                    break;
                case ResultStatusEnum.FORECEEND:

                    MessageBox.Show("停止了自動操作", "SYSTEM", MessageBoxButtons.OK);
                    IsLiveCapturing = INI.ISLIVECAPTURE;
                    RESULT.IsStopNormalTick = false;

                    break;
                case ResultStatusEnum.CALIBRATEEND:

                    MessageBox.Show("自動校正抓圖停止", "SYSTEM", MessageBoxButtons.OK);
                    IsLiveCapturing = INI.ISLIVECAPTURE;
                    break;
            }
            //}
            //catch(Exception ex)
            //{
            //    MessageBox.Show(ex.ToString());
            //}

        }
        /// <summary>
        /// 清资料
        /// </summary>
        void Collect()
        {
            foreach (AlbumClass album in AlbumCollection.AlbumList)
            {
                if (album.PassInfo.RcpNo != AlbumWork.PassInfo.RcpNo && album.PassInfo.RcpNo != AlbumNow.PassInfo.RcpNo && album.PassInfo.RcpNo < 50000)
                    AlbumWork.ResetRunStatus();
            }
        }
        private void RESULT_EnvTriggerAction(ResultStatusEnum resultstatus, int envindex, string operpagestr)
        {
            switch (resultstatus)
            {
                case ResultStatusEnum.SNSTART:
                    if (CheckAlbumCollection(operpagestr, false))
                    {
                        Universal.RunDebugOrRelease = "Release:";
                        RESULT.TestMethod = TestMethodEnum.QSMCSF;
                        RESULT.Calculate();
                        Universal.OCRSN = "";
                        Universal.ISCHECKSN = false;
                        RESULT.IsStopNormalTick = true;
                    }
                    break;
                case ResultStatusEnum.SETCAMLIGHT:
                    SetCamLight(AlbumNow);
                    //SetCamLight(AlbumWork, envindex, operpagestr);
                    break;
                case ResultStatusEnum.SAVEDEBUGRAW:

                    if (RUNUI != null)
                    {
                        if (RUNUI.IsSaveRaw)
                            RESULT.SaveRUNBMP(envindex, operpagestr);

                    }
                    if (RUNV1UI != null)
                    {
                        if (RUNV1UI.IsSaveRaw)
                            RESULT.SaveRUNBMP(envindex, operpagestr);
                    }
                    break;
                case ResultStatusEnum.SAVEHIGHTRAW:
                    if (RUNUI != null)
                    {
                        if (RUNUI.IsSaveRaw)
                            RESULT.SaveHEIGHTBMP(envindex, operpagestr);
                    }
                    break;
                case ResultStatusEnum.SAVENGRAW:
                    if (RUNUI != null)
                    {
                        if (RUNUI.IsSaveNGRaw)
                            RESULT.SaveRUNBMP(envindex, operpagestr);
                        if (RUNUI.IsSaveDebug && ispass)
                            RESULT.SaveDebugBMP(envindex, operpagestr);
                    }
                    if (RUNV1UI != null)
                    {
                        if (RUNV1UI.IsSaveNGRaw)
                            RESULT.SaveRUNBMP(envindex, operpagestr);
                        if (RUNUI.IsSaveDebug && ispass)
                            RESULT.SaveDebugBMP(envindex, operpagestr);
                    }

                    break;
                case ResultStatusEnum.CHANGEDIRECTORY:

                    if (Universal.IsNoUseCCD || Universal.IsNoUseIO || Universal.IsLocalPicture)
                        lblTestPath.Text = operpagestr;//Universal.RunDebugOrRelease+" "+ operpagestr;

                    break;
                case ResultStatusEnum.STARTLOGPROCESS:
                    RUNV1UI.SetLog(operpagestr, true);
                    break;
                case ResultStatusEnum.LOGPROCESS:
                    RUNV1UI.SetLog(operpagestr);
                    break;
            }
        }

        private void RESULT_TriggerOPAction(ResultStatusEnum resultstatus, string str)
        {
            int pageindex = 0;
            bool isruntest = false;

            switch (resultstatus)
            {
                case ResultStatusEnum.SHOW_BARCODE_RESULT:

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_SDM3:
                        case OptionEnum.MAIN_SDM2:

                            lblMappingBarcodeString.Invoke(new Action(() =>
                            {
                                lblMappingBarcodeString.Text = str;
                            }));

                            break;
                    }

                    break;
                case ResultStatusEnum.SET_CURRENT_IMAGE:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_X6:
                            if (INI.IsOnlyShowCurrentImage)
                            {
                                string[] vs = str.Split('#');
                                bool bOK = int.TryParse(vs[1], out pageindex);
                                DISPUIShowCurrentMover(pageindex);
                                switch (vs[0])
                                {
                                    case "ONLINE":
                                        if (bOK)
                                        {
                                            DISPUI.SetDisplayImage(CamActClass.Instance.GetImage(pageindex));
                                        }
                                        break;
                                    case "DEBUG":
                                        if (bOK)
                                        {
                                            DISPUI.SetDisplayImage(CCDCollection.GetBMP(pageindex, false));
                                        }
                                        break;
                                }
                            }
                            break;
                    }
                    break;
                case ResultStatusEnum.CAPTUREONCE:
                    if (str != "")
                    {
                        pageindex = int.Parse(str.Split(',')[0]);

                        isruntest = str.Split(',')[1] == "T";

                        if (!isruntest)
                        {
                            if (RCPV1UI.GetDBStatus() != DBStatusEnum.NONE)
                            {
                                //CCDCollection.GetImage(0);
                                AlbumNow.ENVList[0].PageList[pageindex].SetbmpORG(PageOPTypeEnum.P00, CCDCollection.GetBMP(0, true));
                            }
                            else
                            {
                                if (ESSUI.GetMainStatus() == ESSStatusEnum.RUN)
                                    AlbumWork.ENVList[0].PageList[pageindex].SetbmpRUN(PageOPTypeEnum.P00, CCDCollection.GetBMP(0, true));
                            }
                        }
                    }
                    break;
                case ResultStatusEnum.CAPTUREONCEEND:
                    //ESSUI.Disable = false;

                    ESSUI.Enabled = true;
                    RCPV1UI.Enabled = true;

                    break;
            }
        }
        void SetCamLight(AlbumClass album, int envindex, string operpagestr = "")
        {
            if (album.ENVList.Count < 1)
                return;

            string camstring = "";
            EnvClass env = album.ENVList[envindex];

            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                case VersionEnum.AUDIX:

                    if (operpagestr != "")
                        MACHINECollection.SetLight(operpagestr);
                    else
                    {
                        MACHINECollection.SetLight(env.GeneralLight);

                        camstring = env.GetCamString();

                        if (camstring.Trim() != "")
                            CCDCollection.SetExposure(camstring);
                    }

                    break;
            }
        }
        /// <summary>
        /// 设定相机的亮度
        /// </summary>
        /// <param name="album"></param>
        void SetCamLight(AlbumClass album)
        {
            if (album == null)
                return;
            if (album.ENVList.Count < 1)
                return;


            switch (Universal.OPTION)
            {
                case OptionEnum.MAIN_SDM3:
                case OptionEnum.MAIN_SD:
                case OptionEnum.MAIN_X6:
                case OptionEnum.MAIN_SDM1:
                case OptionEnum.MAIN_SDM2:
                case JetEazy.OptionEnum.MAIN_SERVICE:
                    //设定相机的亮度
                    foreach (EnvClass env in album.ENVList)
                    {
                        foreach (PageClass page in env.PageList)
                        {
                            CCDCollection.SetExposure(page.Exposure, page.CamIndex);
                        }
                    }

                    break;
                default:
                    //设定相机的亮度
                    foreach (EnvClass env in album.ENVList)
                    {
                        foreach (PageClass page in env.PageList)
                        {
                            if (page.PassInfo.RcpNo == 80002)
                                continue;

                            //if (page.RelateToRcpNo == 80002)
                            //    continue;

                            int index = 0;
                            //前10个相机需组成1个图片,因此设定亮度时后面的相机需要加上9
                            switch (Universal.OPTION)
                            {
                                case OptionEnum.R32:
                                    if (page.CamIndex != 0)
                                        index = 9;
                                    break;
                                case OptionEnum.R15:
                                case OptionEnum.R26:
                                case OptionEnum.R9:
                                    if (page.CamIndex != 0)
                                        index = 5;
                                    break;
                                case OptionEnum.R5:
                                    if (page.CamIndex != 0)
                                        index = 1;
                                    break;
                                case OptionEnum.R1:
                                    index = 0;
                                    break;
                                case OptionEnum.R3:
                                    index = 0;
                                    break;
                                case OptionEnum.C3:
                                    index = 0;
                                    break;
                            }

                            if (page.PassInfo.RcpNo == 80000)
                            {
                                if (page.ExposureString.Trim() == "")
                                    CCDCollection.SetExposure(page.Exposure, page.CamIndex + index);
                                else
                                    CCDCollection.SetExposure(page.ExposureString, page.CamIndex + index);
                            }
                            else
                            {
                                if (page.PassInfo.RcpNo != 80002 && page.PassInfo.RcpNo != 80003)
                                {
                                    if (page.RelateToRcpNo == -1 || page.PassInfo.RcpNo == 80001)
                                        CCDCollection.SetExposure(page.Exposure, page.CamIndex + index);
                                }

                            }
                        }
                    }
                    break;
            }


        }


        /// <summary>
        /// 设定相机的亮度
        /// </summary>
        /// <param name="album"></param>
        void SetCamLight80002(AlbumClass album)
        {
            if (album.ENVList.Count < 1)
                return;
            //设定相机的亮度
            foreach (EnvClass env in album.ENVList)
            {
                foreach (PageClass page in env.PageList)
                {
                    if (page.PassInfo.RcpNo == 80002)
                    {
                        int index = 0;
                        //前10个相机需组成1个图片,因此设定亮度时后面的相机需要加上9
                        switch (Universal.OPTION)
                        {
                            case OptionEnum.R32:
                                if (page.CamIndex != 0)
                                    index = 9;
                                break;
                            case OptionEnum.R15:
                            case OptionEnum.R26:
                            case OptionEnum.R9:
                                if (page.CamIndex != 0)
                                    index = 5;
                                break;
                            case OptionEnum.R5:
                                if (page.CamIndex != 0)
                                    index = 5;
                                break;
                            case OptionEnum.R1:
                                index = 0;
                                break;
                            case OptionEnum.R3:
                                index = 0;
                                break;
                            case OptionEnum.C3:
                                index = 0;
                                break;
                        }


                        if (page.ExposureString.Trim() == "")
                            CCDCollection.SetExposure(page.Exposure, page.CamIndex + index);
                        else
                            CCDCollection.SetExposure(page.ExposureString, page.CamIndex + index);
                    }
                }
            }
        }
        #endregion

        #region RUNUI Action


        private void RUNUI_TriggerAction(RunStatusEnum runstatus)
        {
            switch (runstatus)
            {
                case RunStatusEnum.JCET_CLEAR:

                    //lblShopfloorShow.Text = "";
                    //pbxCAMs[0].Image = null;
                    //pbxCAMs[1].Image = null;
                    //pbxCAMs[2].Image = null;
                    //ESSUI.ClearDisplay();

                    break;
                case RunStatusEnum.JCET_CHANGE_RECIPE:

                    ESSStatusEnum _currentStatu = ESSUI.GetMainStatus();
                    if (_currentStatu == ESSStatusEnum.RUN)
                    {
                        if (!Universal.RESULT.myResult.MainProcess.IsOn)
                        {
                            bool bOK = CheckAlbumCollection(Universal.StripVersionName, false);
                            CommonLogClass.Instance.Log2("AOI切换参数 " + (bOK ? "成功" : "失败"));
                        }
                        else
                        {
                            CommonLogClass.Instance.Log2("AOI测试中");
                        }
                    }
                    else
                    {
                        CommonLogClass.Instance.Log2("AOI不在跑线状态");
                    }

                    break;

                case RunStatusEnum.X6_READY:

                    bool _isready = ((ControlSpace.MachineSpace.JzMainX6MachineClass)MACHINECollection.MACHINE).PLCIO.Ready;
                    _isready = !_isready;
                    ((ControlSpace.MachineSpace.JzMainX6MachineClass)MACHINECollection.MACHINE).PLCIO.Ready = _isready;
                    //if (RUNUI != null)
                    //    ((ControlSpace.MachineSpace.JzMainX6MachineClass)MACHINECollection.MACHINE).PLCIO.Ready = !((ControlSpace.MachineSpace.JzMainX6MachineClass)MACHINECollection.MACHINE).PLCIO.Ready;
                    if (RUNUI != null)
                    {
                        RUNUI.btnReadyBK(_isready);
                    }

                    break;
                case RunStatusEnum.SDM1_READY:

                    _isready = ((ControlSpace.MachineSpace.JzMainSDM1MachineClass)MACHINECollection.MACHINE).PLCIO.Ready;
                    _isready = !_isready;
                    ((ControlSpace.MachineSpace.JzMainSDM1MachineClass)MACHINECollection.MACHINE).PLCIO.Ready = _isready;
                    //if (RUNUI != null)
                    //    ((ControlSpace.MachineSpace.JzMainX6MachineClass)MACHINECollection.MACHINE).PLCIO.Ready = !((ControlSpace.MachineSpace.JzMainX6MachineClass)MACHINECollection.MACHINE).PLCIO.Ready;
                    //if (RUNUI != null)
                    //{
                    //    RUNUI.btnReadyBK(_isready);
                    //}

                    break;
                case RunStatusEnum.SDM1_BYPASS:

                    //_isready = ((ControlSpace.MachineSpace.JzMainSDM1MachineClass)MACHINECollection.MACHINE).PLCIO.Pass;
                    //_isready = !_isready;
                    ((ControlSpace.MachineSpace.JzMainSDM1MachineClass)MACHINECollection.MACHINE).PLCIO.Pass = false;
                    //if (RUNUI != null)
                    //    ((ControlSpace.MachineSpace.JzMainX6MachineClass)MACHINECollection.MACHINE).PLCIO.Ready = !((ControlSpace.MachineSpace.JzMainX6MachineClass)MACHINECollection.MACHINE).PLCIO.Ready;
                    //if (RUNUI != null)
                    //{
                    //    RUNUI.btnBYPASSBK(_isready);
                    //}

                    break;
                case RunStatusEnum.SDM2_READY:

                    _isready = ((ControlSpace.MachineSpace.JzMainSDM2MachineClass)MACHINECollection.MACHINE).PLCIO.Ready;
                    _isready = !_isready;
                    ((ControlSpace.MachineSpace.JzMainSDM2MachineClass)MACHINECollection.MACHINE).PLCIO.Ready = _isready;

                    break;
                case RunStatusEnum.SDM2_BYPASS:

                    ((ControlSpace.MachineSpace.JzMainSDM2MachineClass)MACHINECollection.MACHINE).PLCIO.Pass = false;


                    break;

                case RunStatusEnum.SDM3_READY:

                    _isready = ((ControlSpace.MachineSpace.JzMainSDM3MachineClass)MACHINECollection.MACHINE).PLCIO.Ready;
                    _isready = !_isready;
                    ((ControlSpace.MachineSpace.JzMainSDM3MachineClass)MACHINECollection.MACHINE).PLCIO.Ready = _isready;

                    break;
                case RunStatusEnum.SDM3_BYPASS:

                    ((ControlSpace.MachineSpace.JzMainSDM3MachineClass)MACHINECollection.MACHINE).PLCIO.Pass = false;


                    break;
                case RunStatusEnum.SHINNIGEND:

                    RESULT.IsStopNormalTick = false;

                    switch (Universal.VERSION)
                    {
                        case VersionEnum.ALLINONE:
                            switch (Universal.OPTION)
                            {
                                case OptionEnum.MAIN_SDM3:
                                    //ADD Gaara
                                    if (INI.IsSaveScreen)
                                    {
                                        Allinone.OPSpace.ResultSpace.JzMainSDM3ResultClass jzMainSDM3Result = (Allinone.OPSpace.ResultSpace.JzMainSDM3ResultClass)RESULT.myResult;
                                        jzMainSDM3Result.SavePrintScreenForMainX6();
                                    }
                                    break;
                                case OptionEnum.MAIN_SDM2:
                                    //ADD Gaara
                                    if (INI.IsSaveScreen)
                                    {
                                        Allinone.OPSpace.ResultSpace.JzMainSDM2ResultClass jzMainSDM2Result = (Allinone.OPSpace.ResultSpace.JzMainSDM2ResultClass)RESULT.myResult;
                                        jzMainSDM2Result.SavePrintScreenForMainX6();
                                    }
                                    break;
                                case OptionEnum.MAIN_SDM1:
                                    //ADD Gaara
                                    if (INI.IsSaveScreen)
                                    {
                                        Allinone.OPSpace.ResultSpace.JzMainSDM1ResultClass jzMainSDM1Result = (Allinone.OPSpace.ResultSpace.JzMainSDM1ResultClass)RESULT.myResult;
                                        jzMainSDM1Result.SavePrintScreenForMainX6();
                                    }
                                    break;
                                case OptionEnum.MAIN_X6:
                                    //ADD Gaara
                                    if (INI.IsSaveScreen)
                                    {
                                        Allinone.OPSpace.ResultSpace.JzMainX6ResultClass jzrmainresult = (Allinone.OPSpace.ResultSpace.JzMainX6ResultClass)RESULT.myResult;
                                        jzrmainresult.SavePrintScreenForMainX6();
                                    }
                                    break;

                                case OptionEnum.R32:
                                    if (INI.ISQSMCALLSAVE)
                                    {
                                        //ADD Gaara
                                        Allinone.OPSpace.ResultSpace.JzR32ResultClass jzr32result = (Allinone.OPSpace.ResultSpace.JzR32ResultClass)RESULT.myResult;
                                        jzr32result.SavePrintScreen();
                                    }
                                    break;
                                case OptionEnum.R26:
                                    if (INI.ISQSMCALLSAVE)
                                    {
                                        //ADD Gaara
                                        Allinone.OPSpace.ResultSpace.JzRXXResultClass jzr26result = (Allinone.OPSpace.ResultSpace.JzRXXResultClass)RESULT.myResult;
                                        jzr26result.SavePrintScreen();
                                    }
                                    break;
                                case OptionEnum.R15:
                                    if (INI.ISQSMCALLSAVE)
                                    {
                                        //ADD Gaara
                                        Allinone.OPSpace.ResultSpace.JzR15ResultClass jzr15result = (Allinone.OPSpace.ResultSpace.JzR15ResultClass)RESULT.myResult;
                                        jzr15result.SavePrintScreen();
                                    }
                                    break;
                                case OptionEnum.R9:

                                    INI.ISQSMCALLSAVE = true;
                                    if (INI.ISQSMCALLSAVE)
                                    {
                                        //ADD Gaara
                                        Allinone.OPSpace.ResultSpace.JzR9ResultClass jzr9result = (Allinone.OPSpace.ResultSpace.JzR9ResultClass)RESULT.myResult;
                                        jzr9result.SavePrintScreen();
                                    }
                                    break;
                                case OptionEnum.R5:

                                    INI.ISQSMCALLSAVE = true;
                                    if (INI.ISQSMCALLSAVE)
                                    {
                                        //ADD Gaara
                                        Allinone.OPSpace.ResultSpace.JzR5ResultClass jzr5result = (Allinone.OPSpace.ResultSpace.JzR5ResultClass)RESULT.myResult;
                                        jzr5result.SavePrintScreen();
                                    }
                                    break;
                                case OptionEnum.R1:

                                    INI.ISQSMCALLSAVE = true;
                                    if (INI.ISQSMCALLSAVE)
                                    {
                                        //ADD Gaara
                                        Allinone.OPSpace.ResultSpace.JzR1ResultClass jzr1result = (Allinone.OPSpace.ResultSpace.JzR1ResultClass)RESULT.myResult;
                                        jzr1result.SavePrintScreen();
                                    }
                                    break;
                                case OptionEnum.R3:

                                    INI.ISQSMCALLSAVE = true;
                                    if (INI.ISQSMCALLSAVE)
                                    {
                                        //ADD Gaara
                                        Allinone.OPSpace.ResultSpace.JzR3ResultClass jzr3result = (Allinone.OPSpace.ResultSpace.JzR3ResultClass)RESULT.myResult;
                                        jzr3result.SavePrintScreen();
                                    }
                                    break;
                                case OptionEnum.C3:

                                    INI.ISQSMCALLSAVE = true;
                                    if (INI.ISQSMCALLSAVE)
                                    {
                                        //ADD Gaara
                                        Allinone.OPSpace.ResultSpace.JzC3ResultClass jzC3result = (Allinone.OPSpace.ResultSpace.JzC3ResultClass)RESULT.myResult;
                                        jzC3result.SavePrintScreen();
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
                case RunStatusEnum.BACKTONORMAL:

                    IsLiveCapturing = INI.ISLIVECAPTURE;

                    if (RUNUI != null)
                        RUNUI.InitialResultPanel();

                    if (!IsLiveCapturing)
                    {
                        DISPUI.ClearAll();
                    }
                    else
                    {
                        IsNeedToShowMover = true;
                        DISPUIShowMover();
                    }

                    break;
            }
        }
        private void RUNUI_LearnAction(PassInfoClass passinfo, LearnOperEnum learnoper)
        {
            AlbumCollection.GetAnalyzeMaxNo(passinfo);

            switch (learnoper)
            {
                case LearnOperEnum.TUNE:
                    JetEazy.LoggerClass.Instance.WriteLog("点击测试画面源图" +
                        ";参数序号:" + passinfo.RcpNo +
                        ";Level:" + passinfo.Level +
                        ";AnalyzeNo:" + passinfo.AnalyzeNo);
                    ShowTune(passinfo, false);
                    break;
                case LearnOperEnum.LEARN:
                    JetEazy.LoggerClass.Instance.WriteLog("点击测试画面及时图" +
                       ";参数序号:" + passinfo.RcpNo +
                       ";Level:" + passinfo.Level +
                       ";AnalyzeNo:" + passinfo.AnalyzeNo);
                    ShowTune(passinfo, true);
                    break;
                case LearnOperEnum.COMP:
                    JetEazy.LoggerClass.Instance.WriteLog("点击测试画面差异图" +
                     ";参数序号:" + passinfo.RcpNo +
                     ";Level:" + passinfo.Level +
                     ";AnalyzeNo:" + passinfo.AnalyzeNo);
                    break;
            }
        }

        TrainMessageForm TRAINFORM;
        void ShowTune(PassInfoClass passinfo, bool islearn)
        {
            //int i = 0;

            if (!ESSUI.btnRecipe.Enabled)
                return;

            IsLiveCapturing = false;

            AnalyzeClass TuneAnalyze = null;

            TuneAnalyze = AlbumCollection.GetAnalyze(passinfo, LearnOperEnum.THIS);

            if (TuneAnalyze == null)
                return;

            TuneAnalyze.PrepareForLearnOperation(islearn);

            DETAILFRM = new DetailForm(TuneAnalyze, VERSION, Universal.OPTION);

            if (DETAILFRM.ShowDialog() == DialogResult.OK)
            {
                AnalyzeClass parentanalyze = AlbumCollection.GetAnalyze(passinfo, LearnOperEnum.PARENT);

                if (parentanalyze == null)
                {
                    MessageBox.Show("学习或调整 不成功!");
                    return;
                }

                TuneAnalyze.EndLearnOperation(true);
                AlbumCollection.SaveAnalyze(passinfo);

                //單獨 Train 這個 Analyze
                //i = 0;


                //TuneAnalyze.IsTempSave = true;

                //Findout Why This will Error 2017/12/26!!!!!!!!!FUCK YOU!

                TRAINFORM = new TrainMessageForm(parentanalyze.ToLogAnalyzeString());
                TRAINFORM.Show();

                TuneAnalyze.PrintMessageAction += TuneAnalyze_PrintMessageAction;

                //TuneAnalyze.ResetRunStatus();
                TuneAnalyze.ResetTrainStatus();

                bool isgood = TuneAnalyze.A00_Train(parentanalyze.bmpPATTERN, true, parentanalyze.myOringinOffsetPointF);

                if (isgood)
                    TRAINFORM.SetComplete(true);
                else
                    TRAINFORM.SetCancel();

                TuneAnalyze.PrintMessageAction -= TuneAnalyze_PrintMessageAction;

                //TuneAnalyze.IsTempSave = false;
            }
            else
            {
                //if(TuneAnalyze.LearnList.Count > 0)
                TuneAnalyze.EndLearnOperation(false);
            }

            DISPUI.ClearMover();

            IsNeedToShowMover = true;
            DISPUIShowMover();

            DETAILFRM.Dispose();

            IsLiveCapturing = true;
        }

        private void TuneAnalyze_PrintMessageAction(List<string> processstringlist)
        {
            if (TRAINFORM != null)
                TRAINFORM.SetString(processstringlist);
        }

        #endregion

        #region CTRL Action

        private void CTRLUI_TriggerAction(ActionEnum action, string opstr)
        {
            switch (Universal.OPTION)
            {
                case OptionEnum.MAIN_SDM2:
                case OptionEnum.MAIN_SDM1:
                case OptionEnum.MAIN_SDM3:
                    switch (action)
                    {
                        case ActionEnum.ACT_RESET:
                            RESULT.myResult.Reset();
                            break;
                        case ActionEnum.ACT_ONEKEYGETIMAGE:
                            RESULT.myResult.SetPara(AlbumNow, CCDCollection);
                            RESULT.myResult.OneKeyGetImage();
                            break;
                    }

                    break;

                case OptionEnum.MAIN_SD:

                    switch (action)
                    {
                        case ActionEnum.ACT_MOTOR_SETUP:
                            IsLiveCapturing = false;
                            break;

                        case ActionEnum.ACT_TEST_GETIMAGE:

                            m_IsStartTest = true;

                            break;
                        case ActionEnum.ACT_USER_FULL_PASS:
                            SetBuzzer(true);
                            M_WARNING_FRM = new MessageForm(true, "PASS区达到片数设定值，请取出。", "");
                            M_WARNING_FRM.ShowDialog();
                            break;
                        case ActionEnum.ACT_USER_FULL_NG:
                            SetBuzzer(true);
                            M_WARNING_FRM = new MessageForm(true, "NG区达到片数设定值，请取出。", "");
                            M_WARNING_FRM.ShowDialog();
                            break;
                        case ActionEnum.ACT_SENSOR_FULL_PASS:
                            SetBuzzer(true);
                            M_WARNING_FRM = new MessageForm(true, "PASS区硬件收料盒已满，请取出。", "");
                            M_WARNING_FRM.ShowDialog();
                            break;
                        case ActionEnum.ACT_SENSOR_FULL_NG:
                            SetBuzzer(true);
                            M_WARNING_FRM = new MessageForm(true, "NG区硬件收料盒已满，请取出。", "");
                            M_WARNING_FRM.ShowDialog();
                            break;
                        case ActionEnum.ACT_CONTINUE_COUNT:
                            SetBuzzer(true);
                            M_WARNING_FRM = new MessageForm(true, "连续测试NG " + INI.CONTINUE_NG_COUNT.ToString() + " 片", "");
                            M_WARNING_FRM.ShowDialog();
                            ((ControlSpace.MachineSpace.JzMainSDMachineClass)MACHINECollection.MACHINE).ContinueNGIndex = 0;
                            break;
                        case ActionEnum.ACT_ISEMC:
                            //M_WARNING_FRM = new MessageForm(true, "急停已按下。");
                            //M_WARNING_FRM.ShowDialog();



                            break;
                        case ActionEnum.ACT_SETCAMEXPOSURE:

                            if (AlbumWork != null)
                                SetCamLight(AlbumWork);

                            break;
                    }

                    break;
                default:

                    bool ismicroscope = opstr == "M";

                    switch (action)
                    {
                        case ActionEnum.CAPTUREONCE:
                            //if (ESSUI.GetMainStatus() == ESSStatusEnum.RECIPE)
                            if (ESSUI.GetMainStatus() == ESSStatusEnum.EDIT && RCPV1UI.GetDBStatus() != DBStatusEnum.NONE)
                            {
                                ESSUI.Enabled = false;
                                RCPV1UI.Enabled = false;

                                RESULT.StartCaptureOnce(AlbumNow.GetEnvByIndex(0).GeneralPosition, ismicroscope);
                            }
                            else if (RESULT.GetStartCaptureOnceStatus() == true)
                            {
                                ESSUI.Enabled = true;
                                RCPV1UI.Enabled = true;

                                RESULT.StartCaptureOnce("", false);
                            }
                            else
                                MessageBox.Show("Please Go To RECIPE EDIT Status");

                            break;
                        case ActionEnum.CAPTURETEST:

                            RESULT.StartCaptureTest(AlbumNow.GetEnvByIndex(0).GeneralPosition, ismicroscope);

                            break;
                        case ActionEnum.ALLRESET:

                            MACHINECollection.GoHome();

                            break;


                    }

                    break;
            }
        }

        void SetNormalLight()
        {
            ((ControlSpace.MachineSpace.JzMainSDMachineClass)MACHINECollection.MACHINE).PLCIO.Red = false;
            ((ControlSpace.MachineSpace.JzMainSDMachineClass)MACHINECollection.MACHINE).PLCIO.Yellow = false;
            ((ControlSpace.MachineSpace.JzMainSDMachineClass)MACHINECollection.MACHINE).PLCIO.Green = true;
        }
        void SetAbnormalLight()
        {
            ((ControlSpace.MachineSpace.JzMainSDMachineClass)MACHINECollection.MACHINE).PLCIO.Red = true;
            ((ControlSpace.MachineSpace.JzMainSDMachineClass)MACHINECollection.MACHINE).PLCIO.Yellow = false;
            ((ControlSpace.MachineSpace.JzMainSDMachineClass)MACHINECollection.MACHINE).PLCIO.Green = false;
        }
        void SetRunningLight()
        {
            ((ControlSpace.MachineSpace.JzMainSDMachineClass)MACHINECollection.MACHINE).PLCIO.Red = false;
            ((ControlSpace.MachineSpace.JzMainSDMachineClass)MACHINECollection.MACHINE).PLCIO.Yellow = true;
            ((ControlSpace.MachineSpace.JzMainSDMachineClass)MACHINECollection.MACHINE).PLCIO.Green = false;
        }

        void SetBuzzer(bool IsON)
        {
            ((ControlSpace.MachineSpace.JzMainSDMachineClass)MACHINECollection.MACHINE).PLCIO.ADR_BUZZER = IsON;
        }

        bool IsPassHave
        {
            get { return ((ControlSpace.MachineSpace.JzMainSDMachineClass)MACHINECollection.MACHINE).PLCIO.TAKEPASS_IsHaveProduct; }
        }
        bool IsNgHave
        {
            get { return ((ControlSpace.MachineSpace.JzMainSDMachineClass)MACHINECollection.MACHINE).PLCIO.TAKENG_IsHaveProduct; }
        }

        /// <summary>
        /// 多线程启动测试标志
        /// </summary>
        bool m_IsStartTest = false;

        //bool m_IsUserFull = false;

        MessageForm M_WARNING_FRM;

        /// <summary>
        /// 多线程的测试
        /// </summary>
        private void CTRLUI_TriggerAct()
        {
            switch (VERSION)
            {
                case VersionEnum.ALLINONE:

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_SD:

                            //if (m_IsUserFull)
                            //{
                            //    m_IsUserFull = false;
                            //    M_WARNING_FRM = new MessageForm(true, "达到片数设定值，请取出。");
                            //    M_WARNING_FRM.ShowDialog();
                            //    M_WARNING_FRM.Close();
                            //    M_WARNING_FRM.Dispose();
                            //}


                            if (m_IsStartTest)
                            {
                                m_IsStartTest = false;

                                //if (!((Allinone.ControlSpace.MachineSpace.JzMainSDMachineClass)MACHINECollection.MACHINE).IsGetImageComplete)
                                {
                                    ////不要執行包含在固定參數的參數
                                    //if (("," + INI.PRELOADSTATICNO + ",").IndexOf("," + RCPDB.DataNow.No.ToString() + ",") > -1)
                                    //    MessageBox.Show("Please Check " + RCPDB.DataNow.Name + " ID = " + RCPDB.DataNow.No.ToString() + ".");
                                    //else
                                    //{
                                    //    if (CheckAlbumCollection(RCPDB.DataNow.Version, true))
                                    //    {
                                    //        RESULT.TestMethod = TestMethodEnum.IO;
                                    //        RESULT.Calculate();

                                    //        if (RUNUI != null)
                                    //        {
                                    //            Universal.IsSaveRaw = RUNUI.IsSaveRaw;
                                    //        }
                                    //    }
                                    //}

                                    #region 来自小算盘

                                    //Universal.IsMultiThread = false;
                                    string strPath3 = RESULT.myResult.GetLastDirPath(Universal.DEBUGSRCPATH);
                                    Universal.FolderName = System.IO.Path.GetFileNameWithoutExtension(strPath3);
                                    Universal.FolderPath = strPath3 + "\\";
                                    strPath3 += @"\SN.txt";
                                    //if (System.IO.File.Exists(strPath3))
                                    //{
                                    //    Universal.RunDebugOrRelease = "Release:";

                                    //    string strsnPath = INI.SHOPFLOORPATH + @"\SN.txt";
                                    //    if (!System.IO.File.Exists(strsnPath))
                                    //        System.IO.File.Copy(strPath3, strsnPath);
                                    //}
                                    //else
                                    {
                                        Universal.RunDebugOrRelease = "Debug:";
                                        if (("," + INI.PRELOADSTATICNO + ",").IndexOf("," + RCPDB.DataNow.No.ToString() + ",") > -1)
                                            MessageBox.Show("Please Check " + RCPDB.DataNow.Name + " ID = " + RCPDB.DataNow.No.ToString() + ".");
                                        else
                                        {


                                            if (CheckAlbumCollection(RCPDB.DataNow.Version, true, TestMethodEnum.IO))
                                            {
                                                IsLiveCapturing = false;
                                                RESULT.TestMethod = TestMethodEnum.IO;
                                                RESULT.Calculate();

                                                if (RUNUI != null)
                                                {
                                                    Universal.IsSaveRaw = RUNUI.IsSaveRaw;
                                                }
                                            }
                                        }
                                    }

                                    #endregion


                                    //RESULT.TestMethod = TestMethodEnum.IO;
                                    //RESULT.Calculate();

                                    //if (RUNUI != null)
                                    //{
                                    //    Universal.IsSaveRaw = RUNUI.IsSaveRaw;
                                    //}
                                }
                            }


                            RegionTick();

                            break;
                    }

                    break;
            }
        }


        JzMainSDPositionParaClass JzMainSDPositionParas
        {
            get { return Universal.JZMAINSDPOSITIONPARA; }
        }

        PLCMotionClass MOTOR_REGION_PASS
        {
            get { return MACHINECollection.MACHINE.PLCMOTIONCollection[(int)MotionEnum.M6]; }
        }
        PLCMotionClass MOTOR_REGION_NG
        {
            get { return MACHINECollection.MACHINE.PLCMOTIONCollection[(int)MotionEnum.M7]; }
        }

        void RegionTick()
        {
            BuzzerTick();

            //TakePASSUnLoadTick();
            //TakePASSLoadTick();

            //TakeNGUnLoadTick();
            //TakeNGLoadTick();

            //TakeFullPASSTick();
            //TakeFullNGTick();
        }

        ProcessClass m_TakeFullPASSProcess = new ProcessClass();
        private void TakeFullPASSTick()
        {
            ProcessClass Process = m_TakeFullPASSProcess;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:
                        SetRunningLight();
                        m_TakePASSUnloadprocess.Start();

                        Process.NextDuriation = 200;
                        Process.ID = 10;

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (!m_TakePASSUnloadprocess.IsOn)
                            {
                                Process.NextDuriation = 200;
                                Process.ID = 15;
                                m_BuzzerProcess.Start();
                            }
                        }
                        break;
                    case 15:
                        if (Process.IsTimeup)
                        {
                            if (!m_TakePASSUnloadprocess.IsOn)
                            {
                                Process.Pause();
                                M_WARNING_FRM = new MessageForm(true, "PASS区达到片数设定值，请取出后点击确定。", "");
                                if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                                {
                                    if (IsPassHave)
                                    {
                                        JzMainSDPositionParas.PassZero();

                                        Process.NextDuriation = 100;
                                        Process.ID = 15;
                                    }
                                    else
                                    {
                                        m_TakePASSloadprocess.Start();

                                        Process.NextDuriation = 200;
                                        Process.ID = 20;
                                    }
                                }
                                Process.Continue();
                                M_WARNING_FRM.Close();
                                M_WARNING_FRM.Dispose();
                            }
                        }
                        break;
                    case 20:
                        if (Process.IsTimeup)
                        {
                            if (!m_TakePASSloadprocess.IsOn)
                            {
                                Process.Stop();
                                SetNormalLight();
                            }
                        }
                        break;
                }
            }
        }
        ProcessClass m_TakeFullNGProcess = new ProcessClass();
        private void TakeFullNGTick()
        {
            ProcessClass Process = m_TakeFullNGProcess;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:
                        SetRunningLight();
                        m_TakeNGUnloadprocess.Start();

                        Process.NextDuriation = 200;
                        Process.ID = 10;

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (!m_TakeNGUnloadprocess.IsOn)
                            {
                                Process.NextDuriation = 200;
                                Process.ID = 15;
                                m_BuzzerProcess.Start();
                            }
                        }
                        break;
                    case 15:
                        if (Process.IsTimeup)
                        {
                            if (!m_TakeNGUnloadprocess.IsOn)
                            {
                                Process.Pause();
                                M_WARNING_FRM = new MessageForm(true, "NG区达到片数设定值，请取出后点击确定。", "");
                                if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                                {
                                    if (IsNgHave)
                                    {
                                        JzMainSDPositionParas.NgZero();

                                        Process.NextDuriation = 100;
                                        Process.ID = 15;
                                    }
                                    else
                                    {
                                        m_TakeNGloadprocess.Start();

                                        Process.NextDuriation = 200;
                                        Process.ID = 20;
                                    }
                                }
                                Process.Continue();
                                M_WARNING_FRM.Close();
                                M_WARNING_FRM.Dispose();
                            }
                        }
                        break;
                    case 20:
                        if (Process.IsTimeup)
                        {
                            if (!m_TakeNGloadprocess.IsOn)
                            {
                                Process.Stop();
                                SetNormalLight();
                            }
                        }
                        break;
                }
            }
        }


        ProcessClass m_TakePASSloadprocess = new ProcessClass();
        private void TakePASSLoadTick()
        {
            ProcessClass Process = m_TakePASSloadprocess;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:
                        m_TakePASSUnloadprocess.Stop();
                        MOTOR_REGION_PASS.Go(JzMainSDPositionParas.TAKE_Z1POS1);
                        Process.NextDuriation = 200;
                        Process.ID = 10;

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            //if (MOTOR_REGION_PASS.IsReachUpperBound)
                            if (IsInRange(MOTOR_REGION_PASS.PositionNow, JzMainSDPositionParas.TAKE_Z1POS1, 0.5f) && MOTOR_REGION_PASS.IsOnSite)
                            {
                                Process.Stop();
                                MOTOR_REGION_PASS.Stop();
                            }
                        }
                        break;
                }
            }
        }
        ProcessClass m_TakeNGloadprocess = new ProcessClass();
        private void TakeNGLoadTick()
        {
            ProcessClass Process = m_TakeNGloadprocess;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:
                        m_TakeNGUnloadprocess.Stop();
                        MOTOR_REGION_NG.Go(JzMainSDPositionParas.TAKE_Z2POS1);
                        Process.NextDuriation = 200;
                        Process.ID = 10;

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            //if (MOTOR_REGION_NG.IsReachUpperBound)
                            if (IsInRange(MOTOR_REGION_NG.PositionNow, JzMainSDPositionParas.TAKE_Z2POS1, 0.5f) && MOTOR_REGION_NG.IsOnSite)
                            {
                                Process.Stop();
                                MOTOR_REGION_NG.Stop();
                            }
                        }
                        break;
                }
            }
        }


        ProcessClass m_TakePASSUnloadprocess = new ProcessClass();
        private void TakePASSUnLoadTick()
        {
            ProcessClass Process = m_TakePASSUnloadprocess;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        m_TakePASSloadprocess.Stop();
                        //MOTOR_REGION_PASS.Backward();
                        MOTOR_REGION_PASS.Go(0);

                        Process.NextDuriation = 200;
                        Process.ID = 10;

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (IsInRange(MOTOR_REGION_PASS.PositionNow, 0, 0.5f) && MOTOR_REGION_PASS.IsOnSite)
                            //if(MOTOR_REGION_PASS.IsReachLowerBound)
                            {
                                Process.Stop();
                                //MOTOR_REGION_PASS.Stop();
                            }
                        }
                        break;
                }
            }
        }
        ProcessClass m_TakeNGUnloadprocess = new ProcessClass();
        private void TakeNGUnLoadTick()
        {
            ProcessClass Process = m_TakeNGUnloadprocess;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        m_TakeNGloadprocess.Stop();
                        //MOTOR_REGION_NG.Backward();
                        MOTOR_REGION_NG.Go(0);


                        Process.NextDuriation = 200;
                        Process.ID = 10;

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            //if(MOTOR_REGION_NG.IsReachLowerBound)
                            if (IsInRange(MOTOR_REGION_NG.PositionNow, 0, 0.5f) && MOTOR_REGION_NG.IsOnSite)
                            {
                                Process.Stop();
                                //MOTOR_REGION_NG.Stop();
                            }
                        }
                        break;
                }
            }
        }

        bool IsInRange(float FromValue, float CompValue, float DiffValue)
        {
            return Math.Abs(FromValue - CompValue) < DiffValue;
        }

        /// <summary>
        /// 叫的第几次
        /// </summary>
        int m_BuzzerIndex = 0;
        /// <summary>
        /// 叫几声
        /// </summary>
        int m_BuzzerCount = 3;

        /// <summary>
        /// 蜂鸣器叫几声流程
        /// </summary>
        ProcessClass m_BuzzerProcess = new ProcessClass();
        private void BuzzerTick()
        {
            ProcessClass Process = m_BuzzerProcess;
            //iNextDurtime[3] = 1000;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        Process.NextDuriation = 100;
                        Process.ID = 10;

                        switch (Process.RelateString)
                        {
                            default:
                                m_BuzzerIndex = 0;
                                m_BuzzerCount = 3;
                                break;
                        }

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (m_BuzzerIndex < m_BuzzerCount)
                            {
                                //MACHINE.PLCIO.ADR_BUZZER = true;
                                SetBuzzer(true);

                                Process.NextDuriation = 500;
                                Process.ID = 15;

                                m_BuzzerIndex++;
                            }
                            else
                            {
                                //MACHINE.PLCIO.ADR_BUZZER = false;
                                SetBuzzer(false);
                                Process.Stop();
                            }
                        }
                        break;
                    case 15:
                        if (Process.IsTimeup)
                        {
                            //MACHINE.PLCIO.ADR_BUZZER = false;
                            SetBuzzer(false);

                            Process.NextDuriation = 500;
                            Process.ID = 10;
                        }
                        break;
                }
            }
        }

        #endregion

        #region Application Operation

        #region TrainMessage Operation
        /// <summary>
        /// 參數訓練
        /// </summary>
        /// <param name="rcpno"></param>
        void TrainAlbum(int rcpno, bool IsMultiThread = false)
        {
            int i = 0;

            if (rcpno == -1)
            {
                i = 0;
                while (i < AlbumCollection.StaticAlbumList.Count)
                {
                    //   long size = Marshal.SizeOf(AlbumCollection.StaticAlbumList[i]);
                    AlbumCollection.StaticAlbumList[i].A00_TrainProcess(IsMultiThread);
                    //     long size2 = Marshal.SizeOf(AlbumCollection.StaticAlbumList[i]);
                    //ALBCollection.AlbumList[i].A08_RunProcess();
                    i++;
                }

                i = 0;
                while (i < AlbumCollection.AlbumList.Count)
                {
                    AlbumCollection.AlbumList[i].A00_TrainProcess(IsMultiThread);
                    //ALBCollection.AlbumList[i].A08_RunProcess();

                    i++;
                }
                GC.Collect();
                //AlbumCollection.GotoIndex(RCPDB.DataNow.No);
            }
            else
            {
                if (AlbumCollection.FindStaticIndicator(rcpno) > -1)
                {
                    AlbumCollection.StaticAlbumList[AlbumCollection.FindStaticIndicator(rcpno)].A00_TrainProcess(IsMultiThread);
                }
                else
                {
                    AlbumCollection.AlbumList[AlbumCollection.FindNormalIndicator(rcpno)].A00_TrainProcess(IsMultiThread);
                }
            }

            switch (OPTION)
            {
                case OptionEnum.MAIN_SDM2:
                case OptionEnum.MAIN_SDM3:
                    TrayMappingInit();
                    break;
            }
        }
        /// <summary>
        /// 檢查ALBUMCollection裏是否有沒載入的參數並且整理AlbumWork
        /// </summary>
        /// <param name="version"></param>
        bool CheckAlbumCollection(string opstring, bool isusethisrecipe, TestMethodEnum testmethod = TestMethodEnum.QSMCSF)
        {
            bool isgood = false;

            string version = "";
            string artwork = "";
            string relatecolor = "";
            Universal.No80001Err = "";
            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch (OPTION)
                    {
                        case OptionEnum.R32:
                        case OptionEnum.R26:
                        case OptionEnum.R15:
                        case OptionEnum.R9:
                        case OptionEnum.R5:
                        case OptionEnum.R1:
                            if (testmethod == TestMethodEnum.QSMCSF)
                            {
                                version = opstring.Split('$')[0];
                                artwork = opstring.Split('$')[1].Split('_')[0];
                                relatecolor = opstring.Split('$')[1].Split('_')[1];
                            }
                            break;
                        case OptionEnum.R3:
                            if (testmethod == TestMethodEnum.QSMCSF)
                            {
                                version = opstring.Split('$')[0];
                                artwork = opstring.Split('$')[1].Split('_')[0];
                                relatecolor = opstring.Split('$')[0];
                            }
                            break;
                        case OptionEnum.C3:
                            if (testmethod == TestMethodEnum.QSMCSF)
                            {
                                version = opstring.Split('$')[0];
                                artwork = opstring.Split('$')[1].Split('_')[0];
                                relatecolor = opstring.Split('$')[0];
                            }
                            break;
                    }
                    break;
            }
            Universal.RELATECOLORSTR = opstring;
            bool isNoVer = false;
            if (!isusethisrecipe)
            {
                int rcpno = 1;

                switch (VERSION)
                {
                    case VersionEnum.ALLINONE:

                        switch (OPTION)
                        {
                            case OptionEnum.MAIN_SDM2:
                            case OptionEnum.MAIN_X6:
                            case JetEazy.OptionEnum.MAIN_SERVICE:
                            case OptionEnum.MAIN_SDM3:
                                rcpno = RCPDB.FindName(opstring);

                                break;
                        }

                        break;

                    default:

                        string parName = relatecolor.Split('-')[0];
                        if (!INI.ISMANYPAR)
                            rcpno = RCPDB.FindVersion(version);
                        else if (INI.SFFACTORY == FactoryShopfloor.FOXCONN)
                            rcpno = RCPDB.FindVersion(version, artwork);
                        else if (INI.SFFACTORY == FactoryShopfloor.QSMC)
                            rcpno = RCPDB.FindVersion(version, parName);

                        break;

                }

                switch (VERSION)
                {
                    case VersionEnum.ALLINONE:

                        switch (OPTION)
                        {
                            case OptionEnum.MAIN_SDM2:
                            case OptionEnum.MAIN_X6:
                            case JetEazy.OptionEnum.MAIN_SERVICE:
                            case OptionEnum.MAIN_SDM3:
                                if (rcpno == -1)   //若找不到這個版本，則 Load 第 1 筆參數
                                {
                                    return false;
                                }

                                break;
                        }

                        break;

                    default:

                        if (rcpno == -1)   //若找不到這個版本，則 Load 第 1 筆參數
                        {
                            isNoVer = true;
                            rcpno = 0;
                        }

                        break;

                }




                int indicator = AlbumCollection.FindNormalIndicator(rcpno);
                int staticindicator = AlbumCollection.FindStaticIndicator(rcpno);

                if (indicator == -1 && staticindicator == -1)
                {
                    int ircpno = RCPDB.FindIndex(rcpno);
                    AlbumClass newloadalbum = new AlbumClass(RCPDB.GetRCP(ircpno));
                    AlbumCollection.Add(newloadalbum);
                    TrainAlbum(rcpno, true);
                }

                RCPDB.GotoIndex(RCPDB.FindIndex(rcpno));
                ESSDB.DataNow.LastRecipeNo = RCPDB.DataNow.No;
                ESSDB.Save();

                AlbumCollection.GotoIndex(rcpno);
                RCPUI.FillDisplay();
                ESSUI.SetRecipeName(RCPDB.DataNow.ToESSString());
                SetCamLight(AlbumNow);
            }

            if (!AlbumCollection.GetAlbumWork(opstring, testmethod))
            {
                // 如果出現這個，代表參數中的 ENV 的數目和現在 Load進去的不同
                MessageBox.Show("參數對應錯誤，請確認參數是否正確", "SYSTEM", MessageBoxButtons.OK);
            }
            else
            {
                //將Assign及Compound 及 Analyze 整理一下
                AlbumWork.RelateToASN();
                isgood = true;
            }

            switch (OPTION)
            {
                case OptionEnum.MAIN_SDM3:
                case OptionEnum.MAIN_SDM2:
                case OptionEnum.MAIN_X6:
                case JetEazy.OptionEnum.MAIN_SERVICE:
                    break;
                default:
                    if (Universal.No80001Err != "")
                    {
                        System.Windows.Forms.MessageBox.Show("无此:' " + opstring + "  ' 镭雕,请联系厂商注册!", "SYS",
                                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return false;
                    }
                    if (isNoVer)
                    {
                        System.Windows.Forms.MessageBox.Show("无此:' " + artwork + "  ' 参数编号,请联系厂商注册!", "SYS",
                                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return false;

                    }
                    break;
            }

            return isgood;
        }

        #endregion

        #endregion

        #region Tools Operation
        /// <summary>
        /// 删除多余的备份文件件
        /// </summary>
        void Deletebackupfile(string strPath)
        {
            System.IO.DirectoryInfo TheFolder = new System.IO.DirectoryInfo(strPath);
            List<string> namelist = new List<string>();
            foreach (System.IO.FileInfo NextFile in TheFolder.GetFiles())
                namelist.Add(NextFile.Name);

            namelist.Sort();

            for (int i = 30; i < namelist.Count; i++)
            {
                string strpathTemp = strPath + "\\" + namelist[i];
                System.IO.File.Delete(strpathTemp);
            }
            namelist.Clear();
        }


        void CGOperate()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        #endregion

        #region MAIN_SDM2

        

        //int m_UIWidth = 100;
        public void TrayMappingInit()
        {
            pnlMappingUI.Width = (int)(tabCtrl.TabPages[0].Width * 0.7);
            pnlMappingUI.Height = tabCtrl.TabPages[0].Height;
            pnlMappingUI.Location = new Point(0, 0);

            //lblMappingBarcodeString = new Label();
            //lblMappingBarcodeString.Name = "lblMappingBarcodeString";
            //lblMappingBarcodeString.Text = "...";
            //lblMappingBarcodeString.AutoSize = false;
            //lblMappingBarcodeString.Font = new Font("宋体", 18);
            //lblMappingBarcodeString.TextAlign = ContentAlignment.MiddleLeft;
            lblMappingBarcodeString.Width = (int)(tabCtrl.TabPages[0].Width * 0.3);
            lblMappingBarcodeString.Height = 30;
            //lblMappingBarcodeString.BackColor = Color.White;
            //lblMappingBarcodeString.DoubleClick += LblMappingDataString_DoubleClick;
            //tabCtrl.TabPages[0].Controls.Add(lblMappingBarcodeString);
            lblMappingBarcodeString.Location = new Point(pnlMappingUI.Width + 5, 5);

            //lblMappingDataString = new Label();
            //lblMappingDataString.Name = "lblMappingDataString";
            //lblMappingDataString.Text = INI.GetDataResultString();
            //lblMappingDataString.AutoSize = false;
            //lblMappingDataString.Font = new Font("宋体", 18);
            //lblMappingDataString.TextAlign = ContentAlignment.MiddleLeft;
            lblMappingDataString.Width = (int)(tabCtrl.TabPages[0].Width * 0.3);
            lblMappingDataString.Height = tabCtrl.TabPages[0].Height - 30;
            //lblMappingDataString.BackColor = Color.Lime;
            //lblMappingDataString.DoubleClick += LblMappingDataString_DoubleClick;
            //tabCtrl.TabPages[0].Controls.Add(lblMappingDataString);
            lblMappingDataString.Location = new Point(pnlMappingUI.Width + 5, 5 + 30);

            if (AlbumNow.m_EnvNow.DrawMapping != null)
                MappingUI.Initial(AlbumNow.m_EnvNow.DrawCol,
                                                        AlbumNow.m_EnvNow.DrawRow,
                                                        pnlMappingUI.Width,
                                                        pnlMappingUI.Height,
                                                        5,
                                                        String.Join(",", AlbumNow.m_EnvNow.DrawMapping),
                                                        false);

            TrayMappingLabel();
            SetTrayMapping();
        }
        public void LblDataRecordInit()
        {
            pnlMappingUI.Width = (int)(tabCtrl.TabPages[0].Width * 0.7);
            pnlMappingUI.Height = tabCtrl.TabPages[0].Height;
            pnlMappingUI.Location = new Point(0, 0);

            lblMappingBarcodeString = new Label();
            lblMappingBarcodeString.Name = "lblMappingBarcodeString";
            lblMappingBarcodeString.Text = "...";
            lblMappingBarcodeString.AutoSize = false;
            lblMappingBarcodeString.Font = new Font("宋体", 18);
            lblMappingBarcodeString.TextAlign = ContentAlignment.MiddleLeft;
            lblMappingBarcodeString.Width = (int)(tabCtrl.TabPages[0].Width * 0.3);
            lblMappingBarcodeString.Height = 30;
            lblMappingBarcodeString.BackColor = Color.White;
            lblMappingBarcodeString.DoubleClick += LblMappingDataString_DoubleClick;
            tabCtrl.TabPages[0].Controls.Add(lblMappingBarcodeString);
            lblMappingBarcodeString.Location = new Point(pnlMappingUI.Width + 5, 5);

            lblMappingDataString = new Label();
            lblMappingDataString.Name = "lblMappingDataString";
            lblMappingDataString.Text = INI.GetDataResultString();
            lblMappingDataString.AutoSize = false;
            lblMappingDataString.Font = new Font("宋体", 18);
            lblMappingDataString.TextAlign = ContentAlignment.MiddleLeft;
            lblMappingDataString.Width = (int)(tabCtrl.TabPages[0].Width * 0.3);
            lblMappingDataString.Height = tabCtrl.TabPages[0].Height - 30;
            lblMappingDataString.BackColor = Color.Lime;
            lblMappingDataString.DoubleClick += LblMappingDataString_DoubleClick;
            tabCtrl.TabPages[0].Controls.Add(lblMappingDataString);
            lblMappingDataString.Location = new Point(pnlMappingUI.Width + 5, 5 + 30);

            //if (AlbumNow.m_EnvNow.DrawMapping != null)
            //    MappingUI.Initial(AlbumNow.m_EnvNow.DrawCol,
            //                                            AlbumNow.m_EnvNow.DrawRow,
            //                                            pnlMappingUI.Width,
            //                                            pnlMappingUI.Height,
            //                                            5,
            //                                            String.Join(",", AlbumNow.m_EnvNow.DrawMapping),
            //                                            false);

            //TrayMappingLabel();
            //SetTrayMapping();
        }

        private void LblMappingDataString_DoubleClick(object sender, EventArgs e)
        {
            //清零操作
            if (MessageBox.Show("清零所有当前数据？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            INI.ResetDataResult();
            INI.SaveDataRecord();
            ShowLabelMappingDataString();
            //lblMappingDataString.Text = INI.GetDataResultString();
            //lblMappingDataString.Refresh();
        }

        private void ShowLabelMappingDataString()
        {
            lblMappingDataString.BeginInvoke(new EventHandler(delegate
            {
                lblMappingDataString.Text = INI.GetDataResultString();
            }));
        }

        public void SetTrayMapping()
        {
            pnlMappingUI.BackgroundImage = MappingUI.bmpTray;

            
        }
        public void ClearTrayMapping()
        {
            pnlMappingUI.BackgroundImage = null;
        }
        Label[] labels = null;
        public void TrayMappingLabel()
        {
            if (AlbumNow.m_EnvNow.DrawMapping == null)
                return;
            pnlMappingUI.BackgroundImage = null;
            pnlMappingUI.Controls.Clear();
            if (labels != null)
            {
                foreach (Label bin in labels)
                {
                    bin.DoubleClick -= MainForm_DoubleClick;
                }
            }

            labels = new Label[AlbumNow.m_EnvNow.DrawCol * AlbumNow.m_EnvNow.DrawRow];

            float linewidth = (float)(pnlMappingUI.Width) / (float)AlbumNow.m_EnvNow.DrawCol;
            float lineheight = (float)pnlMappingUI.Height / (float)AlbumNow.m_EnvNow.DrawRow;

            int i = 0;
            while (i < labels.Length)
            {
                labels[i] = new Label();
                labels[i].Name = "lbl" + i.ToString();
                labels[i].Text = "";// i.ToString();
                labels[i].Tag = i;
                labels[i].Width = (int)linewidth;
                labels[i].Height = (int)lineheight;
                labels[i].DoubleClick += MainForm_DoubleClick;
                labels[i].BackColor = Color.Transparent;
                //labels[i].BackColor = Color.Lime;
                pnlMappingUI.Controls.Add(labels[i]);
                i++;
            }

            i = 0;
            int j = 0;

            int k = 0;

            foreach (Label bin in labels)
            {
                bin.Location = new Point((int)linewidth * i + i * 2, (int)lineheight * j + j * 2);

                i++;
                if (i == AlbumNow.m_EnvNow.DrawCol)
                {
                    j++;
                    i = 0;
                }

                k++;
            }
        }

        private void MainForm_DoubleClick(object sender, EventArgs e)
        {
            Label label = (Label)sender;
            int index = (int)label.Tag;
            switch (Universal.OPTION)
            {
                case OptionEnum.MAIN_SDM3:
                case OptionEnum.MAIN_SDM2:

                    if (AlbumNow.m_EnvNow.ResultBMPMapping == null)
                        return;

                    if (index >= AlbumNow.m_EnvNow.ResultBMPMapping.Length)
                        return;

                    Bitmap bmp = AlbumNow.m_EnvNow.ResultBMPMapping[index];
                    RUNUI.SetShowChip(bmp);
                    break;
            }
        }

        #endregion


        #region MAIN_SERVICE

        ServiceClientClass xClient = new ServiceClientClass();
        private int _testPageTrain()
        {
            EnvClass env = AlbumNow.ENVList[0];
            Parallel.ForEach(env.PageList, xPageClass =>
            {
                xPageClass.A00_ServiceTrain(false);
            });
            //foreach (PageClass page in env.PageList)
            //{
            //    page.A00_ServiceTrain(false);
            //}

            //SvPageInfo xPageInfo = new SvPageInfo();
            //PageClass xPageClass = AlbumNow.ENVList[0].PageList[0];
            //xPageInfo.m_Org = new Bitmap(xPageClass.GetbmpORG());
            //xPageInfo.m_PassInfoStr = xPageClass.PassInfo.ToString();
            //xPageInfo.m_PageStr = xPageClass.ToString();
            //xPageInfo.m_AnalyzeStr = xPageClass.ToAnalyzeString();

            //string trainstr = string.Empty;
            //xClient.PageTrain(xPageInfo, ref trainstr, "127.0.0.1", 6000);
            //WorkStatusCollectionClass TrainStatusCollection = new WorkStatusCollectionClass();
            //if (!string.IsNullOrEmpty(trainstr))
            //{
            //    TrainStatusCollection.FromString(trainstr);
            //}

            return 0;
        }

        #endregion


        bool ispass = false;
        void AutoDebug()
        {


            if (!Universal.isAutoDebug)
                return;

            if (!ispass)
                Universal.CopyFolder(Universal.FolderPath, Universal.DEBUGSRCPATH + "Test\\" + Universal.FolderName + "\\");

            if (RESULT.myResult.DirIndex != 0)
            {
                string strPath2 = RESULT.myResult.GetLastDirPath(Universal.DEBUGSRCPATH);
                Universal.FolderName = System.IO.Path.GetFileNameWithoutExtension(strPath2);
                Universal.FolderPath = strPath2 + "\\";
                if (Universal.OPTION == OptionEnum.R3 || Universal.OPTION == OptionEnum.C3)
                    strPath2 += @"\OCRTexting.txt";
                else
                    strPath2 += @"\SN.txt";
                if (System.IO.File.Exists(strPath2))
                {
                    Universal.RunDebugOrRelease = "Release:";
                    string strsnPath = "";
                    if (Universal.OPTION == OptionEnum.R3 || Universal.OPTION == OptionEnum.C3)
                        strsnPath = INI.SHOPFLOORPATH + @"\OCRTexting.txt";
                    else
                        strsnPath = INI.SHOPFLOORPATH + @"\SN.txt";
                    if (!System.IO.File.Exists(strsnPath))
                        System.IO.File.Copy(strPath2, strsnPath);
                }
            }
            else
            {
                Universal.isAutoDebug = false;
                MessageBox.Show("自动跑线完成!");

            }

        }


        List<string> AssignRankList = new List<string>();
        List<AnalyzeClass> analyzeClasses = new List<AnalyzeClass>();

        public int Smoothen()
        {
            AssignRankList.Clear();
            analyzeClasses.Clear();
            int envindex = 0;
            int pageindex = 0;
            int analyzeindex = 0;
            foreach (EnvClass env in AlbumWork.ENVList)
            {
                pageindex = 0;
                foreach (PageClass page in env.PageList)
                {
                    analyzeindex = 0;
                    foreach (AnalyzeClass analyze in page.AnalyzeRoot.BranchList)
                    {
                        if (analyze.PADPara.PADMethod == PADMethodEnum.GLUECHECK)
                        {
                            analyzeClasses.Add(analyze);
                            AssignRankList.Add(envindex.ToString() + "," + pageindex.ToString() + "," + analyzeindex.ToString());
                        }
                        analyzeindex++;
                    }
                    pageindex++;
                }
                envindex++;
            }

            if (!INI.CHIP_ISSMOOTHEN)
                return -1;

            int i = 0;
            int j = 0;
            int RestoreSeq = -1;

            //VICTOR MADE
            float SimilarCheck = 0;
            float SimiliarCount = 0;
            float SimiliarStep = 0;
            bool IsRestored = false;

            if (analyzeClasses.Count <= 0)
                return -1;

            i = Math.Min(100, analyzeClasses[0].BackupCount);

            if (i > 0)
            {
                j = 0;
                while (j < i)
                {
                    //Check Similiar First

                    SimilarCheck = 0;
                    SimiliarCount = 0;
                    SimiliarStep = 0;

                    foreach (string str in AssignRankList)
                    {
                        int envseq = int.Parse(str.Split(',')[0]);
                        int pageseq = int.Parse(str.Split(',')[1]);
                        int analyzeseq = int.Parse(str.Split(',')[2]);
                        AnalyzeClass assign = AlbumWork.ENVList[envseq].PageList[pageseq].AnalyzeRoot.BranchList[analyzeseq];

                        SimilarCheck += assign.CheckSimilar(j, ref SimiliarStep);
                        SimiliarCount += SimiliarStep;
                        //SETUPList[assign.SetupIndex].SIDEList[assign.SideIndex].DecorateBMP(bmpwork[assign.SideIndex]);
                    }

                    float SimilarRatio = SimilarCheck / SimiliarCount;
                    //Modified By Victor In 2015/08/24 ORG is 0.7
                    if (SimilarRatio > 0.88)
                    {
                        foreach (string str in AssignRankList)
                        {
                            int envseq = int.Parse(str.Split(',')[0]);
                            int pageseq = int.Parse(str.Split(',')[1]);
                            int analyzeseq = int.Parse(str.Split(',')[2]);
                            AnalyzeClass assign = AlbumWork.ENVList[envseq].PageList[pageseq].AnalyzeRoot.BranchList[analyzeseq];
                            assign.RestoreData(j);
                            RestoreSeq = j;
                        }

                        IsRestored = true;

                        break;
                    }

                    j++;
                }
            }

            if (!IsRestored)
            {
                foreach (string str in AssignRankList)
                {
                    int envseq = int.Parse(str.Split(',')[0]);
                    int pageseq = int.Parse(str.Split(',')[1]);
                    int analyzeseq = int.Parse(str.Split(',')[2]);
                    AnalyzeClass assign = AlbumWork.ENVList[envseq].PageList[pageseq].AnalyzeRoot.BranchList[analyzeseq];

                    assign.BackupData();
                }
            }

            return RestoreSeq;
        }

        public int _changeModelBackgroudImage()
        {
            if (Universal.IsNoUseCCD)
            {
                string strPath3 = RESULT.myResult.GetLastDirPath(Universal.DEBUGSRCPATH);
                CCDCollection.SetDebugPath(RESULT.myResult.LastDirPath);
                CCDCollection.SetDebugEnvPath(0.ToString("000"));
                CCDCollection.SetPageOPType(PageOPTypeEnum.P00.ToString());
                //MyResult_EnvTriggerAction(ResultStatusEnum.CHANGEENVDIRECTORY, 0, PageOPTypeEnum.P00.ToString());
                CCDCollection.GetImage();
            }

            EnvClass env = AlbumNow.ENVList[0];

            //将图片放入测试中
            int i = 0;
            foreach (PageClass page in env.PageList)
            {
                if (Universal.IsNoUseCCD)
                    page.SetbmpRUN(PageOPTypeEnum.P00, CCDCollection.GetBMP(i, false));
                else
                    page.SetbmpRUN(PageOPTypeEnum.P00, CamActClass.Instance.GetImage(i)); //实测使用  先正常抓图 然后一键更换底图
                i++;
            }

            bool iscollecttemp = INI.IsCollectPictures;//缓存这个变量

            if (INI.IsCollectPictures)
                INI.IsCollectPictures = false;

            //开始测试一遍 记录偏移值
            AlbumNow.A08_RunProcess(PageOPTypeEnum.P00);

            if (iscollecttemp)
                INI.IsCollectPictures = true;

            //计算并写入偏移值
            foreach (PageClass page in env.PageList)
            {
                page.A101_ProcessAnalyzeOffset(PageOPTypeEnum.P00);
            }

            //更换底图
            i = 0;
            foreach (PageClass page in env.PageList)
            {
                if (Universal.IsNoUseCCD)
                    page.SetbmpORG(PageOPTypeEnum.P00, CCDCollection.GetBMP(i, false));
                else
                    page.SetbmpORG(PageOPTypeEnum.P00, CamActClass.Instance.GetImage(i)); //实测使用  先正常抓图 然后一键更换底图
                i++;
            }

            AlbumNow.A00_TrainProcess(true);
            AlbumNow.Save();
            return 0;
        }

        private Bitmap _getMainX6ShowBarcode(Bitmap eBmpInput)
        {
            Bitmap bmpinputtemp = new Bitmap(eBmpInput);

            Graphics graphics = Graphics.FromImage(bmpinputtemp);
            foreach (EnvClass env in AlbumWork.ENVList)
            {
                foreach (PageClass page in env.PageList)
                {
                    foreach (AnalyzeClass analyze in page.AnalyzeRoot.BranchList)
                    {
                        //if (analyze.IsVeryGood)
                        //    continue;
                        PointF ptloc = analyze.myDrawAnalyzeStrRectF.Location;

                        ptloc.Y += 28;
                        string _barcode = analyze.GetAnalyzeBarcodeStr();// _getAnalyzeBarcodeStr(analyze);
                        graphics.DrawString(_barcode, new Font("宋体", 18), Brushes.Lime, ptloc);

                        //JzToolsClass jzToolsClass = new JzToolsClass();
                        //Rectangle NGRectFill = jzToolsClass.SimpleRect(ptloc, 20);
                        //Color NGColor = Color.Red;
                        //if (analyze.PADPara.DescStr.Contains("无胶"))
                        //{
                        //    NGColor = Color.Purple;
                        //}
                        //else if (analyze.PADPara.DescStr.Contains("尺寸"))
                        //{
                        //    NGColor = Color.Yellow;
                        //}
                        //else if (analyze.PADPara.DescStr.Contains("溢胶"))
                        //{
                        //    NGColor = Color.Orange;
                        //}
                        //graphics.FillRectangle(new SolidBrush(NGColor), NGRectFill);
                        //graphics.DrawString(analyze.PADPara.DescStr + "(" + analyze.ToAnalyzeString() + ")", new Font("宋体", 18), Brushes.Red, ptloc);
                        //ptloc.Y += 28;
                        //graphics.DrawString(analyze.PADPara.DescStr, new Font("宋体", 18), Brushes.Red, ptloc);
                        //ptloc.Y += 28;
                        //graphics.DrawString("(" + analyze.ToAnalyzeString() + ")", new Font("宋体", 18), Brushes.Red, ptloc);


                    }
                }
            }
            graphics.Dispose();


            return bmpinputtemp;
        }

        //string _getAnalyzeBarcodeStr(AnalyzeClass eAnalyze)
        //{
        //    if (eAnalyze.OCRPara.OCRMethod == OCRMethodEnum.DATAMATRIX)
        //    {
        //        return eAnalyze.ReadBarcode2DStr;
        //    }
        //    foreach (AnalyzeClass analyzeClass in eAnalyze.BranchList)
        //    {
        //        string _barcodeStr = _getAnalyzeBarcodeStr(analyzeClass);
        //        if (!string.IsNullOrEmpty(_barcodeStr))
        //            return _barcodeStr;
        //    }
        //    return string.Empty;
        //}

        System.Diagnostics.Stopwatch watchAutoTestTime = new System.Diagnostics.Stopwatch();
        int m_DirIndex = 0;
        int m_DirCount = 0;
        bool m_IsAutoRunSMD2 = false;
        private void _getAutoMainSDM2Test()
        {
            if (Universal.IsNoUseIO && m_IsAutoRunSMD2)
            {
                if (watchAutoTestTime.ElapsedMilliseconds > 1 * 33 * 1000)
                {
                    watchAutoTestTime.Stop();
                    watchAutoTestTime.Restart();

                    if (m_DirIndex < m_DirCount)
                    {
                        ESSUI_TriggerAction(ESSStatusEnum.FASTCAL);
                        m_DirIndex++;
                        label8.Text = "当前测试的Index=" + m_DirIndex.ToString() + ";总数=" + m_DirCount.ToString();
                    }
                    else
                    {
                        watchAutoTestTime.Stop();
                        m_IsAutoRunSMD2 = false;
                    }
                }
                else
                {
                    label9.Text = watchAutoTestTime.ElapsedMilliseconds.ToString("0.000") + " ms";
                }
                label9.BackColor = (watchAutoTestTime.IsRunning ? Color.Red : Control.DefaultBackColor);
            }
        }

        bool m_WriteToRecordRunning = false;
        string m_RecordFileName = DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt";
        private void _recoredDataSevenPoint()
        {
            if (DateTime.Now.ToString("HHmmss") == "043000" && !m_WriteToRecordRunning)
            {
                m_WriteToRecordRunning = true;
                m_RecordFileName = DateTime.Now.ToString("yyyyMMdd-HH") + ".txt";
                if (!System.IO.File.Exists(Universal.DATAREPORTPATH + "\\" + m_RecordFileName))
                {
                    SaveDataEX(INI.GetDataResultString(), Universal.DATAREPORTPATH + "\\" + m_RecordFileName);

                    INI.ResetDataResult();
                    INI.SaveDataRecord();
                    //lblMappingDataString.Text = INI.GetDataResultString();
                    ShowLabelMappingDataString();
                    //ESSDB.Reset(true);
                    //ESSDB.Reset(false);

                    ESSUI.ClearPass();
                    ESSUI.ClearFail();
                }
            }
            else if (DateTime.Now.ToString("HHmmss") == "163000" && !m_WriteToRecordRunning)
            {
                m_WriteToRecordRunning = true;

                m_RecordFileName = DateTime.Now.ToString("yyyyMMdd-HH") + ".txt";
                if (!System.IO.File.Exists(Universal.DATAREPORTPATH + "\\" + m_RecordFileName))
                {
                    SaveDataEX(INI.GetDataResultString(), Universal.DATAREPORTPATH + "\\" + m_RecordFileName);

                    INI.ResetDataResult();
                    INI.SaveDataRecord();
                    //lblMappingDataString.Text = INI.GetDataResultString();
                    ShowLabelMappingDataString();
                    //ESSDB.Reset(true);
                    //ESSDB.Reset(false);

                    ESSUI.ClearPass();
                    ESSUI.ClearFail();
                }
            }
            //else if (DateTime.Now.ToString("HHmmss") == "181700" && !m_WriteToRecordRunning)
            //{
            //    m_WriteToRecordRunning = true;

            //    m_RecordFileName = DateTime.Now.ToString("yyyyMMdd-HH") + ".txt";
            //    if (!System.IO.File.Exists(Universal.DATAREPORTPATH + "\\" + m_RecordFileName))
            //    {
            //        SaveDataEX(INI.GetDataResultString(), Universal.DATAREPORTPATH + "\\" + m_RecordFileName);

            //        INI.ResetDataResult();
            //        INI.SaveDataRecord();

            //        lblMappingDataString.Text = INI.GetDataResultString();

            //        //ESSDB.Reset(true);
            //        //ESSDB.Reset(false);

            //        ESSUI.ClearPass();
            //        ESSUI.ClearFail();
            //    }
            //}
            else
            {
                if (m_WriteToRecordRunning)
                    m_WriteToRecordRunning = false;
            }
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        public void SaveDataEX(string DataStr, string FileName)
        {
            System.IO.StreamWriter stm = null;

            try
            {
                stm = new System.IO.StreamWriter(FileName, true, System.Text.Encoding.Default);
                stm.Write(DataStr);
                stm.Flush();
                stm.Close();
                stm.Dispose();
                stm = null;
            }
            catch (Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
            }

            if (stm != null)
                stm.Dispose();
        }
    }
}
