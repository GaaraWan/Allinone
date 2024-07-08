using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using JetEazy;
using JetEazy.BasicSpace;
using JetEazy.FormSpace;
using JetEazy.DBSpace;
using JzDisplay;
using JetEazy.ControlSpace;
using Allinone;
using Allinone.OPSpace;
using Allinone.UISpace;
using JetEazy.PlugSpace;
using Allinone.ControlSpace;
using Allinone.ControlSpace.MachineSpace;

namespace Allinone.FormSpace
{
    public partial class DetailForm : Form
    {
        enum TagEnum
        {
            PAGENO,
            CAMINDEX,
            PAGEOPTYPE,
            RELATESTATIC,

            OK,
            CANCEL,
            TEST,

            REGET,
            SHOWRANGE,
            SHOWLUMINA,
            CHECKGREYSCALE,
            CHECKCOLOR,

            SETEXPOSURESTR,
            EXPORT,

            KBAOI,

            AddPage,
            CutPage,
            /// <summary>
            /// 视检高翘
            /// </summary>
            CHECKStilts,

            /// <summary>
            /// 一键取像
            /// </summary>
            ONEKEY_REGET,
            /// <summary>
            /// 一键亮度
            /// </summary>
            ONEKEY_EXPOSURE,

            /// <summary>
            /// 同步页面与步数画面
            /// </summary>
            SYN_PAGECOUNTANDSTEPCOUNT,
            /// <summary>
            /// 自动生成首尾呼应的框的位置
            /// </summary>
            AUTO_RECTPOSITION,
            /// <summary>
            /// 定位至当前页面位置
            /// </summary>
            GO_CURRENT_POSITION,

            /// <summary>
            /// 设定页面多个位置
            /// </summary>
            SET_MUTIL_POSITION,

            SET_LED,
        }

        int PAGEOPTYPECOUNT
        {
            get
            {
                return Universal.PAGEOPTYPECOUNT;
            }
        }

        VersionEnum VERSION = VersionEnum.KBAOI;
        OptionEnum OPTION = OptionEnum.MAIN;

        CCDCollectionClass CCDCollection;

        MachineCollectionClass MACHINECollection
        {
            get { return Universal.MACHINECollection; }
        }
        JzMainSDM1MachineClass MACHINESDM1
        {
            get { return (JzMainSDM1MachineClass)MACHINECollection.MACHINE; }
        }
        JzMainSDM2MachineClass MACHINE
        {
            get { return (JzMainSDM2MachineClass)MACHINECollection.MACHINE; }
        }

        AlbumCollectionClass ALBCollection
        {
            get
            {
                return Universal.ALBCollection;
            }
        }

        ComboBox cboPageNo;
        ComboBox cboCamIndex;
        NumericUpDown numExposure;
        TextBox txtExposure;
        TextBox txtAliasname;
        TextBox txtRelateToVersion;
        ComboBox cboPageOPType;
        Label lblExposure;
        Label lblRelateStatic;
        Label lblRelateToVersion;

        CheckBox chkMactching;
        ComboBox cboMatchingMethod;
        ComboBox cboRelateStatic;

        Button btnReget;
        Button btnShowRange;
        Button btnShowLumina;

        Button btnCheckGreysacle;
        Button btnCheckColor;

        Button btnOK;
        Button btnCancel;
        Button btnTest;
        Button btnSetExposureStr;
        Button btnExport;

        Button btnKBAOI;
        Button btnAddPage;
        Button btnCutPage;

        Button btnCHECKStilts;
        Button btnOneKeyReget;
        Button btnOneKeyExposure;

        Button btnSynPageCountAndStepCount;
        Button btnAutoRectPosition;
        Button btnGOCurrentPosition;

        Button btnSetMutiPostion;
        Button btnLEDControl;

       public PageUI PAGEUI;
        EnvClass ENVNow;
        AnalyzeClass ANALYZENow;

        MessageForm MESSAGEFORM;

        bool IsStatic = false;
        bool IsLearn = false;

        Timer myTimer = new Timer();

        bool IsMatching
        {
            get
            {
                return chkMactching.Checked;
            }
        }
        MatchMethodEnum MatchingMethod
        {
            get
            {
                return (MatchMethodEnum)cboMatchingMethod.SelectedIndex;
            }
        }
       public PageClass PageNow
        {
            get
            {
                return ENVNow.PageList[cboPageNo.SelectedIndex];
            }
        }

        bool IsNeedToChange = false;

        public DetailForm(CCDCollectionClass ccdcollection, EnvClass backupenv,
            VersionEnum version, OptionEnum opt, bool isstatic)
        {
            InitializeComponent();
            Initial(ccdcollection, backupenv, version, opt, isstatic);
        }

        public DetailForm(AnalyzeClass backupanalyze, VersionEnum version, OptionEnum opt)
        {
            InitializeComponent();
            Initial(backupanalyze, version, opt);

        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            PAGEUI = pageUI1;
            PAGEUI.Initial(VERSION, OPTION);

            if (IsLearn)
            {
                PAGEUI.SetIsLearn();
            }

            InitialCamIndex();
            InitialPageOPType();
            InitialMatchingMethod();
            InitialRelateStatic();

            InitialPageIndex();


            if (!IsLearn)
            {
                myTimer = new Timer();
                myTimer.Interval = 500;
                myTimer.Tick += myTimer_Tick;
            }

        }
        void Initial(CCDCollectionClass ccdcollection, EnvClass backupenv,
            VersionEnum version, OptionEnum opt, bool isstatic)
        {
            VERSION = version;
            OPTION = opt;

            CCDCollection = ccdcollection;

            ENVNow = backupenv;

            IsStatic = isstatic;
            //IsLearn = false;
            InitialInside();
        }
        void Initial(AnalyzeClass backupanalyze, VersionEnum version, OptionEnum opt)
        {
            VERSION = version;
            OPTION = opt;

            ANALYZENow = backupanalyze;

            IsLearn = true;

            InitialInside();
        }
        void InitialInside()
        {
            cboPageNo = comboBox1;
            cboCamIndex = comboBox2;
            numExposure = numericUpDown1;
            txtExposure = textBox1;
            txtAliasname = textBox2;
            txtRelateToVersion = textBox3;

            lblExposure = label4;
            lblRelateStatic = label15;
            lblRelateToVersion = label6;
            cboPageOPType = comboBox4;

            btnOK = button4;
            btnCancel = button6;
            btnTest = button9;

            btnReget = button1;
            btnShowRange = button7;
            btnShowLumina = button8;
            btnSetExposureStr = button10;
            btnExport = button12;
            btnCheckGreysacle = button3;
            btnCheckColor = button5;

            btnKBAOI = button2;
            btnAddPage = button11;
            btnCutPage = button13;
            //cboCamIndex.Enabled = IsStatic;
            btnCHECKStilts = button14;
            btnOneKeyReget = button15;
            btnOneKeyExposure = button16;
            btnSynPageCountAndStepCount=button17;
            btnAutoRectPosition = button18;
            btnGOCurrentPosition=button19;
            btnSetMutiPostion = button20;
            btnLEDControl = button21;

            chkMactching = checkBox1;
            cboMatchingMethod = comboBox3;
            cboRelateStatic = comboBox5;

            if (IsLearn)
            {
                cboPageNo.Enabled = false;
                cboCamIndex.Enabled = false;
                numericUpDown1.Enabled = false;
                cboPageNo.Enabled = false;

                btnReget.Enabled = false;
                btnExport.Enabled = false;

                btnKBAOI.Enabled = false;

                chkMactching.Enabled = false;
                cboMatchingMethod.Enabled = false;
                cboRelateStatic.Enabled = false;

                txtExposure.Enabled = false;
                txtRelateToVersion.Enabled = false;
                btnSetExposureStr.Enabled = false;

             //   btnCheckColor.Enabled = false;
                btnCheckGreysacle.Enabled = false;

                btnShowLumina.Enabled = false;
                btnShowRange.Enabled = false;

                btnTest.Visible = false;
            }

            cboPageNo.Tag = TagEnum.PAGENO;
            cboCamIndex.Tag = TagEnum.CAMINDEX;
            cboPageOPType.Tag = TagEnum.PAGEOPTYPE;
            cboRelateStatic.Tag = TagEnum.RELATESTATIC;

            btnOK.Tag = TagEnum.OK;
            btnCancel.Tag = TagEnum.CANCEL;
            btnTest.Tag = TagEnum.TEST;

            btnReget.Tag = TagEnum.REGET;
            btnShowRange.Tag = TagEnum.SHOWRANGE;
            btnShowLumina.Tag = TagEnum.SHOWLUMINA;
            btnCheckGreysacle.Tag = TagEnum.CHECKGREYSCALE;
            btnCheckColor.Tag = TagEnum.CHECKCOLOR;

            btnSetExposureStr.Tag = TagEnum.SETEXPOSURESTR;
            btnExport.Tag = TagEnum.EXPORT;

            btnKBAOI.Tag = TagEnum.KBAOI;
            btnAddPage.Tag = TagEnum.AddPage;
            btnCutPage.Tag = TagEnum.CutPage;
            btnCHECKStilts.Tag = TagEnum.CHECKStilts;
            btnOneKeyReget.Tag = TagEnum.ONEKEY_REGET;
            btnOneKeyExposure.Tag = TagEnum.ONEKEY_EXPOSURE;
            btnSynPageCountAndStepCount.Tag= TagEnum.SYN_PAGECOUNTANDSTEPCOUNT;
            btnAutoRectPosition.Tag = TagEnum.AUTO_RECTPOSITION;
            btnGOCurrentPosition.Tag = TagEnum.GO_CURRENT_POSITION;
            btnSetMutiPostion.Tag = TagEnum.SET_MUTIL_POSITION;
            btnLEDControl.Tag = TagEnum.SET_LED;

            btnOK.Click += btn_Click;
            btnCancel.Click += btn_Click;
            btnTest.Click += btn_Click;
            btnReget.Click += btn_Click;
            btnSetExposureStr.Click += btn_Click;
            btnExport.Click += btn_Click;

            btnKBAOI.Click += btn_Click;
            btnCutPage.Click += btn_Click;
            btnAddPage.Click += btn_Click;
            btnOneKeyReget.Click += btn_Click;
            btnOneKeyExposure.Click += btn_Click;
            btnSynPageCountAndStepCount.Click += btn_Click;
            btnAutoRectPosition.Click += btn_Click;
            btnGOCurrentPosition.Click += btn_Click;
            btnSetMutiPostion.Click += btn_Click;
            btnLEDControl.Click+= btn_Click;

            btnShowRange.MouseDown += btnShowRange_MouseDown;
            btnShowRange.MouseUp += btnShowRange_MouseUp;

            btnShowLumina.MouseDown += BtnShowLumina_MouseDown;
            btnShowLumina.MouseUp += BtnShowLumina_MouseUp;

            btnCheckGreysacle.MouseDown += BtnCheckGreysacle_MouseDown;
            btnCheckGreysacle.MouseUp += BtnCheckGreysacle_MouseUp;

            btnCheckColor.MouseDown += BtnCheckColor_MouseDown;
            btnCheckColor.MouseUp += BtnCheckColor_MouseUp;

            btnCHECKStilts.MouseDown += BtnCHECKStilts_MouseDown;
            btnCHECKStilts.MouseUp += BtnCHECKStilts_MouseUp; ;

            numExposure.ValueChanged += numExposure_ValueChanged;
            //txtExposure.LostFocus += TxtExposure_LostFocus;
            txtAliasname.LostFocus += TxtAliasname_LostFocus;
            txtRelateToVersion.LostFocus += TxtRelateToVersion_LostFocus;
            chkMactching.CheckedChanged += chkMactching_CheckedChanged;

           // cboRelateStatic.SelectedIndexChanged += cboRelateStatic_SelectedIndexChanged;
            cboPageNo.SelectedIndexChanged += cboPageNo_SelectedIndexChanged;
            this.KeyPreview = true;
            this.KeyDown += detailForm_KeyDown;
            this.KeyUp += detailForm_KeyUp;

            CheckVersion();
            btnLEDControl.Visible = false;

            #region 缩放

            switch (Universal.OPTION)
            {
                case OptionEnum.MAIN_SDM3:
                case OptionEnum.MAIN_SDM2:
                case OptionEnum.MAIN_SDM1:
                case OptionEnum.MAIN_SD:
                case OptionEnum.MAIN_X6:

                    btnOneKeyReget.Visible = true;
                    btnOneKeyExposure.Visible = true;

                    //switch (Universal.CAMACT)
                    //{
                    //    case CameraActionMode.CAM_MOTOR:
                    //    case CameraActionMode.CAM_MOTOR_MODE2:

                    //        btnOneKeyExposure.Visible = true;
                    //        btnSetMutiPostion.Visible = true;

                    //        break;
                    //}

                    switch (Universal.OPTION)
                    {
                        case OptionEnum.MAIN_SDM3:
                        case OptionEnum.MAIN_SDM2:
                        case OptionEnum.MAIN_SDM1:

                            btnOneKeyExposure.Visible = true;
                            btnSetMutiPostion.Visible = true;

                            btnLEDControl.Visible = true;

                            //btnAutoRectPosition.Visible = true;
                            btnSynPageCountAndStepCount.Visible = false;
                            btnGOCurrentPosition.Visible = false;

                            btnOneKeyExposure.Visible = false;
                            btnOneKeyReget.Visible = false;

                            Color bkColor = Color.Black;// Control.DefaultBackColor;
                            switch (Universal.OPTION)
                            {
                                case OptionEnum.MAIN_SDM3:
                                    btnLEDControl.BackColor = (((JzMainSDM3MachineClass)MACHINECollection.MACHINE).PLCIO.TopLight ? Color.Red : Color.FromArgb(128, 255, 128));
                                    break;
                                case OptionEnum.MAIN_SDM2:
                                    btnLEDControl.BackColor = (((JzMainSDM2MachineClass)MACHINECollection.MACHINE).PLCIO.TopLight ? Color.Red : Color.FromArgb(128, 255, 128));
                                    break;
                            }




                            break;
                    }

                    #region 界面隐藏
                    checkBox1.Visible = false;
                    comboBox3.Visible = false;
                    comboBox4.Visible = false;
                    button8.Visible = false;
                    button14.Visible = false;
                    button3.Visible = false;
                    button5.Visible = false;
                    #endregion
                    _updateUI();
                    LanguageExClass.Instance.EnumControls(this);


                    break;
            }

            #endregion

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
                        //con.Font = new Font(con.Font.Name, currentSize, con.Font.Style, con.Font.Unit);
                        con.Font = new Font(con.Font.Name, currentSize, (INI.user_screen_bold ? FontStyle.Bold : con.Font.Style), con.Font.Unit);
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

        void _updateUI()
        {
            Xvalue = 1150 - 10;// this.Width;
            //Yvalue = 1024;// this.Height;
                          //Yvalue = this.Height;
            Yvalue = this.Height - 35;

            Xvalue = INI.user_screen_width - 130 - 10;
            Yvalue = INI.user_screen_height - 35;

            switch (Universal.OPTION)
            {
                case OptionEnum.MAIN_SD:
                    Xvalue = INI.user_screen_width;
                    Yvalue = INI.user_screen_height - 35;
                    break;
            }

            //Xvalue = Screen.PrimaryScreen.Bounds.Width - 130;// this.Width;
            //Yvalue = Screen.PrimaryScreen.Bounds.Height - 10;// this.Height;

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

            this.Width = INI.user_screen_width;
            this.Height = INI.user_screen_height;

            //this.Width = Screen.PrimaryScreen.Bounds.Width;
            //this.Height = Screen.PrimaryScreen.Bounds.Height;

            this.WindowState = FormWindowState.Maximized;
        }

        #endregion

        private void BtnCHECKStilts_MouseUp(object sender, MouseEventArgs e)
        {
            PAGEUI.StiltsDispose();
        }

        private void BtnCHECKStilts_MouseDown(object sender, MouseEventArgs e)
        {
            PAGEUI.CheckStilts();
        }

        private void TxtRelateToVersion_LostFocus(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            PageNow.RelateToVersionString = txtRelateToVersion.Text;
        }
        private void TxtAliasname_LostFocus(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            PageNow.AliasName = txtAliasname.Text;
        }
        void CheckVersion()
        {
            btnSetExposureStr.Visible = false;
            txtExposure.Visible = false;
            lblExposure.Visible = false;

            btnExport.Visible = false;
            cboRelateStatic.Visible = false;
            lblRelateStatic.Visible = false;

            lblRelateToVersion.Visible = false;
            txtRelateToVersion.Visible = false;

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
                        case OptionEnum.R3:
                        case OptionEnum.C3:
                        case OptionEnum.R1:
                            btnSetExposureStr.Visible = true;
                            txtExposure.Visible = true;
                            lblExposure.Visible = true;

                            btnExport.Visible = true;
                            cboRelateStatic.Visible = true;
                            lblRelateStatic.Visible = true;

                            if (!IsStatic)
                            {
                                btnExport.Visible = true;
                                cboRelateStatic.Visible = true;
                            }
                            else
                            {
                                lblRelateToVersion.Visible = true;
                                txtRelateToVersion.Visible = true;
                            }

                            break;
                    }
                    break;
                case VersionEnum.AUDIX:


                    break;
            }
        }
        private void TxtExposure_LostFocus(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            PageNow.ExposureString = txtExposure.Text;

            if (PageNow.ExposureString != "")
                CCDCollection.SetExposure(PageNow.ExposureString, PageNow.CamIndex);
        }
        private void BtnShowLumina_MouseUp(object sender, MouseEventArgs e)
        {
            PAGEUI.ClearRange();
        }
        private void BtnShowLumina_MouseDown(object sender, MouseEventArgs e)
        {
            PAGEUI.ShowLumina();
        }
        private void btnShowRange_MouseUp(object sender, MouseEventArgs e)
        {
            PAGEUI.ClearRange();
        }
        private void btnShowRange_MouseDown(object sender, MouseEventArgs e)
        {
            PAGEUI.ShowRange();
        }

        private void BtnCheckColor_MouseUp(object sender, MouseEventArgs e)
        {
            PAGEUI.EndCheck();
        }

        private void BtnCheckColor_MouseDown(object sender, MouseEventArgs e)
        {
            if (PAGEUI.PageNow != null)
                PAGEUI.CheckColor(e.Button == MouseButtons.Right);
            else
                PAGEUI.CheckColorRootNow(e.Button == MouseButtons.Right);
        }
        private void BtnCheckGreysacle_MouseUp(object sender, MouseEventArgs e)
        {
            PAGEUI.EndCheck();
        }

        private void BtnCheckGreysacle_MouseDown(object sender, MouseEventArgs e)
        {
            PAGEUI.CheckGresyscale();
        }

        #region Normal Operation
        void OK()
        {
            //Should Release Memory
            PAGEUI.Suicide();

            this.DialogResult = DialogResult.OK;
        }
        void Cancel()
        {
            //Should Release Memory
            PAGEUI.Suicide();

            this.DialogResult = DialogResult.Cancel;
        }
        void ShowRange()
        {
            PAGEUI.ShowRange();
        }

        bool IsNeedToReget = false;
        PageOPTypeEnum RegetPageOPType = PageOPTypeEnum.P00;
        int RegetCamIndex = -1;
        JzTimes myRegetTime = new JzTimes();
        void Reget()
        {
            if (IsLearn)
                return;

            CCDCollection.R5proindex = 10;
            CCDCollection. isGet80002 = false;
            IsNeedToReget = true;
            RegetPageOPType = (PageOPTypeEnum)cboPageOPType.SelectedIndex;
            RegetCamIndex = PageNow.CamIndex;

            int biasvalue = 0;

            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch(OPTION)
                    {
                        case OptionEnum.R32:
                            if(RegetCamIndex > 0)
                            {
                                biasvalue = 9;
                            }
                            break;
                        case OptionEnum.R26:
                        case OptionEnum.R15:
                        case OptionEnum.R9:
                            if (RegetCamIndex > 0)
                            {
                                biasvalue = 5;
                            }
                            break;
                        case OptionEnum.R5:
                            biasvalue = 1;
                            break;
                        case OptionEnum.R1:
                            break;
                        case OptionEnum.R3:
                            biasvalue = 0;
                            break;
                        case OptionEnum.C3:
                            biasvalue = 0;
                            break;

                    }
                    break;
            }
            
            if(PageNow.ExposureString.Trim() == "")
                CCDCollection.SetExposure((float)numExposure.Value, RegetCamIndex + biasvalue);
            else
                CCDCollection.SetExposure(PageNow.ExposureString, RegetCamIndex + biasvalue);
            
            myRegetTime.Cut();
            myTimer.Start();

        }

        PageInsertForm PAGEINSERTFRM;
        void Export()
        {
            if(cboRelateStatic.SelectedIndex > 0)
            {
                string rcpnostr = cboRelateStatic.Text.Split(')')[0].Replace("(", "");

                PAGEINSERTFRM = new PageInsertForm(ALBCollection.GetStaticAlbum(int.Parse(rcpnostr)));

                switch (PAGEINSERTFRM.ShowDialog())
                {
                    case DialogResult.Yes:
                        if(rcpnostr== ENVNow.PassInfo.RcpNo.ToString())
                            ReplacePage( PageNow, JzToolsClass.PassingInteger);
                        else
                            ALBCollection.ReplacePage(int.Parse(rcpnostr), PageNow, JzToolsClass.PassingInteger);
                       
                        break;
                    case DialogResult.OK:
                        if (rcpnostr == ENVNow.PassInfo.RcpNo.ToString())
                            AddPage(PageNow);
                        else
                            ALBCollection.AddPage(int.Parse(rcpnostr), PageNow);
                        break;
                }
            }
        }
        public void CutPage(PageClass orgpage)
        {
            ENVNow.PageList.Remove(orgpage);
            InitialPageIndex();

            switch(OPTION)
            {
                case OptionEnum.MAIN_SDM3:
                case OptionEnum.MAIN_SDM2:
                case OptionEnum.MAIN_SDM1:
                    break;
                default:
                    switch (Universal.CAMACT)
                    {
                        case CameraActionMode.CAM_MOTOR_LINESCAN:
                        case CameraActionMode.CAM_MOTOR_MODE2:
                        case CameraActionMode.CAM_MOTOR:
                            CamActClass.Instance.CutStep();
                            break;
                    }
                    break;
            }
        }
        public void AddPage(PageClass orgpage)
        {
            PageClass page = orgpage.Clone();

            int index = 0;
            foreach(PageClass patetemp in ENVNow.PageList)
            {
                if (index <= patetemp.No)
                    index = patetemp.No + 1;
            }
            page.No = index;// ENVNow.PageList.Count;
            page.GetPassInfoIncludeAnalyze(ENVNow.PageList[0].PassInfo);

            ENVNow.PageList.Add(page);
            InitialPageIndex();
            cboPageNo.SelectedIndex = cboPageNo.Items.Count-1;

            switch (OPTION)
            {
                case OptionEnum.MAIN_SDM3:
                case OptionEnum.MAIN_SDM2:
                case OptionEnum.MAIN_SDM1:
                    break;
                default:
                    switch (Universal.CAMACT)
                    {
                        case CameraActionMode.CAM_MOTOR_LINESCAN:
                        case CameraActionMode.CAM_MOTOR_MODE2:
                        case CameraActionMode.CAM_MOTOR:
                            CamActClass.Instance.AddStep();
                            break;
                    }
                    break;
            }

           
        }
        public void ReplacePage(PageClass replacepage, int replaceindex)
        {
            PageClass page = replacepage.Clone();

            PageClass deletepage =ENVNow.PageList[replaceindex];

            int deleteno = deletepage.No;
            PassInfoClass deletepassinfo = new PassInfoClass(deletepage.PassInfo, OPLevelEnum.COPY);
            deletepage.Suicide();

            ENVNow.PageList.RemoveAt(replaceindex);

            page.No = deleteno;
            page.GetPassInfoIncludeAnalyze(deletepassinfo);
            page.RelateToVersionString = deletepage.RelateToVersionString;
            ENVNow.PageList.Insert(replaceindex, page);

            InitialPageIndex();
        }
        void KBAOI()
        {
            //KBAOIForm KBAOIFrm = new KBAOIForm(PAGEUI);
            KBAOIForm KBAOIFrm = new KBAOIForm(PAGEUI, ENVNow);
            KBAOIFrm.Show();
        }

        CamExportForm CAMEXPOSUREFORM;
        void SetExposureStr()
        {
            int ccdcount = 1;

            switch(VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch(OPTION)
                    {
                        case OptionEnum.R32:
                            ccdcount = 10;
                            break;
                        case OptionEnum.R26:
                            ccdcount = 11;
                            break;
                        case OptionEnum.R15:
                            ccdcount = 14;
                            break;
                        case OptionEnum.R9:
                            ccdcount = 9;
                            break;
                        case OptionEnum.R5:
                            ccdcount = 5;
                            break;
                        case OptionEnum.R3:
                            ccdcount = 3;
                            break;
                        case OptionEnum.C3:
                            ccdcount = 3;
                            break;
                        case OptionEnum.R1:
                            ccdcount = 1;
                            break;
                    }
                    break;
                case VersionEnum.AUDIX:


                    break;
            }

            CAMEXPOSUREFORM = new CamExportForm(txtExposure.Text, ccdcount);

            if(CAMEXPOSUREFORM.ShowDialog() == DialogResult.OK)
            {
                txtExposure.Text = JzToolsClass.PassingString;
                PageNow.ExposureString = txtExposure.Text;

                if (PageNow.ExposureString != "")
                    CCDCollection.SetExposure(PageNow.ExposureString, PageNow.CamIndex);
            }
        }

        TrainMessageForm TRAINFORM;
        void Test()
        {
            bool isgood = true;

            PageNow.ResetRunStatus();

            PageNow.PrintMessageAction += PageNow_PrintMessageAction;

            TRAINFORM = new TrainMessageForm(PageNow.ToPageIndexString());
            TRAINFORM.Show();

            PageNow.ResetTrainStatus();

            isgood = PageNow.A00_Train();

            if (isgood)
                TRAINFORM.SetComplete(true);
            else
                TRAINFORM.SetCancel();

            PageNow.PrintMessageAction -= PageNow_PrintMessageAction;
        }

        private void PageNow_PrintMessageAction(List<string> processstringlist)
        {
            if (TRAINFORM != null)
                TRAINFORM.SetString(processstringlist);
        }



        void FindAllPagePattern()
        {
            //foreach (PageClass page in ENVNow.PageList)
            //{

            //    List<DoffsetClass> DoffsetList = new List<DoffsetClass>();

            //    //Get the Anallyze Data For Find
            //    Bitmap bmpPageOrg = PageNow.GetbmpORG((PageOPTypeEnum)PageNow.PageOPTypeIndex);

            //    //AnalyzeSelectNow.IsTempSave = true;

            //    //先把 Analyze Train 完
            //    AnalyzeSelectNow.A02_CreateTrainRequirement(bmpPageOrg, new PointF(0, 0));
            //    AnalyzeSelectNow.A05_AlignTrainProcess();

            //    AnalyzeSelectNow.B08_RunAndFindSimilar(bmpPageOrg, tolerance, DoffsetList);

            //    //AnalyzeSelectNow.IsTempSave = false;

            //    //取得自身的區域
            //    int selectno = AnalyzeSelectNow.No;
            //    RectangleF myRectF = AnalyzeSelectNow.GetMoverRectF(bmpPageOrg);
            //    List<RectangleF> OrgRectFList = new List<RectangleF>();

            //    //取得所有定義過的區域
            //    foreach (AnalyzeClass analyze in AnalyzeList)
            //    {
            //        if (analyze.No != 1)
            //        {
            //            OrgRectFList.Add(analyze.GetMoverRectF(bmpPageOrg));
            //        }
            //    }

            //    bool IsIncluded = false;

            //    //Check Duplicate and Copy Ananlyze
            //    foreach (DoffsetClass doffset in DoffsetList)
            //    {
            //        IsIncluded = false;

            //        RectangleF foundrectf = OffsetRect(myRectF, doffset.OffsetF);

            //        //檢查是否有和現有的Analyze重覆超過30%的，有的話視為同一個就不加了
            //        foreach (RectangleF rectf in OrgRectFList)
            //        {
            //            RectangleF rectfintersect = foundrectf;
            //            rectfintersect.Intersect(rectf);

            //            if ((float)(rectfintersect.Width * rectfintersect.Height) / (float)(rectf.Width * rectf.Height) > 0.3)
            //            {
            //                IsIncluded = true;
            //                break;
            //            }
            //        }

            //        //若沒有被包含則加入框裏
            //        if (!IsIncluded)
            //            ATREEUI.AddSameLevel(selectno, doffset, new PointF(myRectF.X + myRectF.Width / 2, myRectF.Y + myRectF.Height / 2));
            //    }

            //}
        }




        #endregion
        void InitialPageOPType()
        {
            cboPageOPType.Items.Clear();

            if(IsLearn)
                return;

            for (int i = 0; i < PAGEOPTYPECOUNT; i++)
            {
                cboPageOPType.Items.Add((PageOPTypeEnum)i);
            }

            cboPageOPType.SelectedIndex = 0;
            
            if (PAGEOPTYPECOUNT == 1)
                cboPageOPType.Enabled = false;

            cboPageOPType.SelectedIndexChanged += cboPageOPType_SelectedIndexChanged;
        }
        void InitialCamIndex()
        {
            int i = 0;

            cboCamIndex.Items.Clear();

            if(IsLearn)
                return;

            while(i < CCDCollection.GetCCDCount)
            {
                cboCamIndex.Items.Add("CAM" + i.ToString("00"));
                i++;
            }

            cboCamIndex.SelectedIndexChanged += cboCamIndex_SelectedIndexChanged;
        }
        void InitialPageIndex()
        {
            cboPageNo.Items.Clear();

            if (IsLearn)
            {
                FillDisplay();

                return;
            }
            //重新赋值页面No
            //int i = 0;
            foreach (PageClass page in ENVNow.PageList)
            {
                //page.No = i;
                cboPageNo.Items.Add(page.ToPageIndexString());
                //i++;
            }

         //   cboPageNo.SelectedIndexChanged += cboPageNo_SelectedIndexChanged;

            IsNeedToChange = true;

            cboPageNo.SelectedIndex = 0;
            if (ENVNow.PageList.Count < 2)
                btnCutPage.Enabled = false;
            else
                btnCutPage.Enabled = true;
        }
        void InitialMatchingMethod()
        {
            cboMatchingMethod.Items.Clear();

            if(IsLearn)
                return;

            int i = 0;

            while (i < (int)MatchMethodEnum.COUNT)
            {
                cboMatchingMethod.Items.Add((MatchMethodEnum)i);
                i++;
            }

            cboMatchingMethod.SelectedIndex = 0;
            cboMatchingMethod.Enabled = false;
        }
        void InitialRelateStatic()
        {
            int i = 0;

            cboRelateStatic.Items.Clear();

            if (IsLearn)
                return;

            cboRelateStatic.Items.Add("None");

            while (i < ALBCollection.StaticAlbumList.Count)
            {
                AlbumClass album = ALBCollection.StaticAlbumList[i];

                cboRelateStatic.Items.Add(album.ToRelateStaticString());
                i++;
            }

            cboRelateStatic.SelectedIndex = 0;

            cboRelateStatic.SelectedIndexChanged += cboRelateStatic_SelectedIndexChanged;
        }
        void FillDisplay()
        {
            MESSAGEFORM = new MessageForm("資料載入中，請稍候...", true);
            MESSAGEFORM.Show();
            MESSAGEFORM.Refresh();

            IsNeedToChange = false;

            if (IsLearn)
            {
                PAGEUI.SetAnalyze(ANALYZENow, true);

                if(ANALYZENow.LearnList.Count > 0)
                    PAGEUI.SetAnalyze(ANALYZENow.GetLearnByIndex(ANALYZENow.LearnList.Count), false);
            }
            else
            {
                if (cboCamIndex.Items.Count > PageNow.CamIndex)
                    cboCamIndex.SelectedIndex = PageNow.CamIndex;
                else
                    cboCamIndex.SelectedIndex = cboCamIndex.Items.Count-1;
                numExposure.Value = PageNow.Exposure;
                PageNow.PageOPTypeIndex = cboPageOPType.SelectedIndex;
                txtExposure.Text = PageNow.ExposureString;
                txtAliasname.Text = PageNow.AliasName;
                txtRelateToVersion.Text = PageNow.RelateToVersionString;

                cboRelateStatic.SelectedIndex = CheckcboRelateStaticIndex(PageNow);
                
                PAGEUI.SetPage(PageNow);
            }

            IsNeedToChange = true;

            MESSAGEFORM.Close();
        }
        int CheckcboRelateStaticIndex(PageClass page)
        {
            int i = 0;
            int ret = 0;

            string compstr = "(" + page.RelateToRcpNo.ToString(RcpClass.ORGRCPNOSTRING) + ")";

            i = 0;

            while(i < cboRelateStatic.Items.Count)
            {
                if(cboRelateStatic.Items[i].ToString().IndexOf(compstr) > -1)
                {
                    ret = i;
                    break;
                }
                i++;
            }

            if(ret == 0)
            {
                page.RelateToRcpNo = -1;
                
            }

            return ret;
        }


        #region Event Operation
        private void detailForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift)
            {
                PAGEUI.SizeMover(e.KeyCode);
            }
            else if (e.Control)
            {
                PAGEUI.HoldSelect();
                PAGEUI.MoveMover(e.KeyCode);
            }

            switch(e.KeyCode)
            {
                case Keys.F6:
                    PAGEUI.AddBranchLevel();
                    break;
                case Keys.F7:
                    PAGEUI.AddSameLevel();
                    break;
                case Keys.F8:
                    PAGEUI.Delete();
                    break;
                case Keys.F9:
                    PAGEUI.Dup();
                    break;
                case Keys.F10:

                    break;
                case Keys.F11:

                    break;
            }
        }
        private void detailForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
                PAGEUI.ReleaseSelect();

        }
        frmSetMutilPosition frmSetMutil = null;
        private void btn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            switch ((TagEnum)btn.Tag)
            {
                case TagEnum.SET_LED:
                    //MACHINE.PLCIO.TopLight = !MACHINE.PLCIO.TopLight;
                    //Color bkColor = Color.Black;// Control.DefaultBackColor;
                    //btnLEDControl.BackColor = (!MACHINE.PLCIO.TopLight ? Color.Red : Color.FromArgb(128, 255, 128));

                    switch (Universal.OPTION)
                    {
                        case OptionEnum.MAIN_SDM3:
                            ((JzMainSDM3MachineClass)MACHINECollection.MACHINE).PLCIO.TopLight = !((JzMainSDM3MachineClass)MACHINECollection.MACHINE).PLCIO.TopLight;
                            btnLEDControl.BackColor = (((JzMainSDM3MachineClass)MACHINECollection.MACHINE).PLCIO.TopLight ? Color.Red : Color.FromArgb(128, 255, 128));
                            break;
                        case OptionEnum.MAIN_SDM2:
                            ((JzMainSDM2MachineClass)MACHINECollection.MACHINE).PLCIO.TopLight = !((JzMainSDM2MachineClass)MACHINECollection.MACHINE).PLCIO.TopLight;
                            btnLEDControl.BackColor = (((JzMainSDM2MachineClass)MACHINECollection.MACHINE).PLCIO.TopLight ? Color.Red : Color.FromArgb(128, 255, 128));
                            break;
                    }

                    break;

                case TagEnum.SET_MUTIL_POSITION:

                    frmSetMutil = new frmSetMutilPosition(PageNow.sPagePostion);
                    if (frmSetMutil.ShowDialog() == DialogResult.OK)
                    {
                        PageNow.sPagePostion = JzToolsClass.PassingString;
                        INI.SaveKeyRecord();
                    }
                    frmSetMutil.Dispose();

                    break;

                case TagEnum.GO_CURRENT_POSITION:
                    MessageForm _msgQuestionFormx2 = new MessageForm(true, "是否要定位至当前页面位置？");
                    if (_msgQuestionFormx2.ShowDialog() == DialogResult.Yes)
                        _ax_GoCurrentPosition();

                    _msgQuestionFormx2.Close();
                    _msgQuestionFormx2.Dispose();
                    break;
                case TagEnum.SYN_PAGECOUNTANDSTEPCOUNT:

                    MessageForm _msgQuestionFormx1 = new MessageForm(true, "是否要同步页面与步数相同并取像(已进行过一键取像)？");
                    if (_msgQuestionFormx1.ShowDialog() == DialogResult.Yes)
                        _ax_SynPageCountAndStepCount();

                    _msgQuestionFormx1.Close();
                    _msgQuestionFormx1.Dispose();

                    break;
                case TagEnum.AUTO_RECTPOSITION:

                    //MessageForm _msgQuestionFormx3 = new MessageForm(true, "是否要自动生成框的位置，必须添加左上角和右下角的框？");
                    //if (_msgQuestionFormx3.ShowDialog() == DialogResult.Yes)
                    //    _ax_AutoRectPosition();

                    //_msgQuestionFormx3.Close();
                    //_msgQuestionFormx3.Dispose();

                    break;
                case TagEnum.ONEKEY_EXPOSURE:
                    MessageForm _msgQuestionFormx = new MessageForm(true, "是否要一键设置亮度？");
                    if (_msgQuestionFormx.ShowDialog() == DialogResult.Yes)
                        _ax_SetExposureForAllPages();

                    _msgQuestionFormx.Close();
                    _msgQuestionFormx.Dispose();
                    break;
                case TagEnum.ONEKEY_REGET:
                    MessageForm _msgQuestionForm = new MessageForm(true, "是否要一键取像？");
                    if (_msgQuestionForm.ShowDialog() == DialogResult.Yes)
                        _ax_GetImageForAllPages();

                    _msgQuestionForm.Close();
                    _msgQuestionForm.Dispose();
                    break;
                case TagEnum.REGET:
                    Reget();
                    break;
                case TagEnum.OK:
                    if (ANALYZENow != null && ANALYZENow.IsPrepareForLearn)
                    {
                        JetEazy.LoggerClass.Instance.WriteLog("点击学习保存" + ";参数名:" + ANALYZENow.AliasName);


                        string basePath = @"D:\LOG\AllinoneLog\Image\";
                        if (!System.IO.Directory.Exists(basePath))
                        {
                            System.IO.Directory.CreateDirectory(basePath);
                        }
                        string dataString = DateTime.Now.ToString("yyyyMMddHHmmss");
                        //ANALYZENow.bmpPATTERN.Save(basePath + ANALYZENow.AliasName + "_" + dataString + "_bmpPATTERN.png");
                        int w = ANALYZENow.bmpPATTERN.Width + ANALYZENow.bmpWIP.Width;
                        int h = ANALYZENow.bmpPATTERN.Height > ANALYZENow.bmpWIP.Height ? ANALYZENow.bmpPATTERN.Height : ANALYZENow.bmpWIP.Height;
                        Bitmap bmpsave = new Bitmap(w, h);

                        Graphics g = Graphics.FromImage(bmpsave);
                        g.DrawImage(ANALYZENow.bmpPATTERN, new PointF(0, 0));
                        g.DrawImage(ANALYZENow.bmpWIP, new PointF(ANALYZENow.bmpPATTERN.Width, 0));
                        g.Dispose();
                        bmpsave.Save(basePath + ANALYZENow.AliasName + "_" + dataString + ".png");
                        bmpsave.Dispose();
                        //ANALYZENow.bmpOUTPUT.Save(basePath + ANALYZENow.AliasName + "_" + dataString + "_bmpOUTPUT.png");
                        //ANALYZENow.bmpORGLEARNININPUT.Save(basePath + ANALYZENow.AliasName + "_" + dataString + "_bmpORGLEARNININPUT.png");
                    }
                    else
                    {
                        if (ANALYZENow != null && !ANALYZENow.IsPrepareForLearn)
                            JetEazy.LoggerClass.Instance.WriteLog("点击参数保存" + ";参数:" + ANALYZENow.PassInfo.RcpNo.ToString("0000"));
                        else if (ENVNow != null)
                            JetEazy.LoggerClass.Instance.WriteLog("点击参数保存" + ";参数:" + ENVNow.PassInfo.RcpNo.ToString("0000"));
                    }
                    OK();
                    break;
                case TagEnum.CANCEL:
                    if (ANALYZENow != null && ANALYZENow.IsPrepareForLearn)
                        JetEazy.LoggerClass.Instance.WriteLog("取消学习" + ";参数名:" + ANALYZENow.AliasName);
                    else if (ANALYZENow != null && !ANALYZENow.IsPrepareForLearn)
                        JetEazy.LoggerClass.Instance.WriteLog("点击参数取消" + ";参数名:" + ANALYZENow.AliasName);
                    else if (ENVNow != null)
                        JetEazy.LoggerClass.Instance.WriteLog("点击参数取消" + ";参数:" + ENVNow.PassInfo.RcpNo.ToString("0000"));
                    Cancel();
                    break;
                case TagEnum.KBAOI:
                    KBAOI();
                    break;
                case TagEnum.TEST:
                    Test();
                    break;
                case TagEnum.SETEXPOSURESTR:
                    SetExposureStr();
                    break;
                case TagEnum.EXPORT:
                    Export();
                    break;
                case TagEnum.CutPage:
                    CutPage(PageNow);
                    break;
                case TagEnum.AddPage:
                    AddPage(PageNow);
                    break;
            }
        }
        private void numExposure_ValueChanged(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            PageNow.Exposure = (int)numExposure.Value;
            int biasvalue = 0;

            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch (OPTION)
                    {
                        case OptionEnum.R32:
                            if (RegetCamIndex > 0)
                            {
                                biasvalue = 9;
                            }
                            break;
                        case OptionEnum.R26:
                        case OptionEnum.R15:
                        case OptionEnum.R9:
                            if (RegetCamIndex > 0)
                            {
                                biasvalue = 5;
                            }
                            break;
                        case OptionEnum.R5:
                            if (RegetCamIndex > 0)
                            {
                                biasvalue = 1;
                            }
                            break;
                        case OptionEnum.R3:
                            
                                biasvalue = 0;
                            
                            break;
                        case OptionEnum.C3:

                            biasvalue = 0;

                            break;
                        case OptionEnum.R1:

                            biasvalue = 0;

                            break;
                    }
                    break;
            }

            CCDCollection.SetExposure((float)numExposure.Value, PageNow.CamIndex+ biasvalue);

        }
        private void cboCamIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            PageNow.CamIndex = cboCamIndex.SelectedIndex;
        }
        private void cboPageNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            FillDisplay();

        }
        private void cboPageOPType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            FillDisplay();

        }
        private void cboRelateStatic_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            if (cboRelateStatic.SelectedIndex == 0)
            {
                PageNow.RelateToRcpNo = -1;
            }
            else
            {
                string str = cboRelateStatic.Text.Split(')')[0].Replace("(", "");
                PageNow.RelateToRcpNo = int.Parse(str);
            }
        }
        private void chkMactching_CheckedChanged(object sender, EventArgs e)
        {
            if(chkMactching.Checked)
            {
                myTimer.Start();
                cboMatchingMethod.Enabled = true;
            }
            else
            {
                myTimer.Stop();
                PAGEUI.SetMatching(null, MatchMethodEnum.NONE);
                cboMatchingMethod.Enabled = false;
            }
        }
        private void myTimer_Tick(object sender, EventArgs e)
        {
            if (IsMatching)
            {
                PAGEUI.SetMatching(CCDCollection.GetBMP(cboCamIndex.SelectedIndex, true), MatchingMethod);
            }

            if (IsNeedToReget)
            {
                // if (myRegetTime.msDuriation > INI.DELAYTIME)
                {
                    if (RegetCamIndex > cboCamIndex.Items.Count - 1)
                        RegetCamIndex = cboCamIndex.Items.Count - 1;
                    if (Universal.OPTION != OptionEnum.R5 || RegetCamIndex != 0)
                    {
                        PageNow.SetbmpORG(RegetPageOPType, CCDCollection.GetBMP(RegetCamIndex, true));
                        PAGEUI.RegetPage(RegetPageOPType);
                        IsNeedToReget = false;
                        myTimer.Stop();
                    }
                    else
                    {
                        CCDCollection.isGet80002 = false;
                        if (PageNow.ExposureString != "")
                        {
                            string[] str = PageNow.ExposureString.Split(',');
                            if (str.Length > 1)
                            {
                                string[] str2 = str[0].Split(':');
                                string[] str3 = str[1].Split(':');

                                int iccd1 = int.Parse(str2[1]);
                                int iccd2 = int.Parse(str3[1]);
                                CCDCollection.iCCD1 = iccd1;
                                CCDCollection.iCCD2 = iccd2;
                            }
                            else
                            {
                                CCDCollection.iCCD1 = -1;
                                CCDCollection.iCCD2 = -1;
                            }
                        }

                        CCDCollection.R5RUNCount = INI.R5RUNCOUNT;
                        bool isok;
                        if (INI.isR5_MOTOR_TO_Rs485)
                            isok = CCDCollection.GetR5ImageOneTick(Universal.mSetMotor);
                        else
                            isok = CCDCollection.GetR5ImageOneTick(Universal.MACHINECollection.MACHINE.PLCCollection[0]);
                        if (isok)
                        {
                            PageNow.SetbmpORG(RegetPageOPType, CCDCollection.GetBMP(0, false));
                            PAGEUI.RegetPage(RegetPageOPType);
                            IsNeedToReget = false;
                            myTimer.Stop();
                        }
                    }


                }

            }


            #endregion

           
        }


        #region PRIVATE FUNTION CREATE BY GAARA 20220625
        /*
         * 更新于20221213
         * 1.删除页面从最后删除
         * 2.增加页面从后面添加
         * 3.增加计算行列的框的计算
         * 
         */

        /// <summary>
        /// 一键重新取图 页面
        /// </summary>
        private void _ax_GetImageForAllPages()
        {
            if (!_ax_CheckPageCountAndStepCount())
            {
                MessageBox.Show("页面与步数不符，请检查。", "一键取像", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageForm _msgProcessForm = new MessageForm("一键取像中...", true);
            _msgProcessForm.Show();
            _msgProcessForm.Refresh();


            switch(Universal.CAMACT)
            {
                case CameraActionMode.CAM_MOTOR_LINESCAN:
                case CameraActionMode.CAM_MOTOR_MODE2:
                case CameraActionMode.CAM_MOTOR:
                    CCDCollection.SetExposure((float)numExposure.Value, PageNow.CamIndex);

                    PageNow.SetbmpORG((PageOPTypeEnum)cboPageOPType.SelectedIndex, CamActClass.Instance.GetImage(PageNow.No));
                    PAGEUI.RegetPage((PageOPTypeEnum)cboPageOPType.SelectedIndex);

                    //int i = 1;
                    foreach (PageClass pageClass in ENVNow.PageList)
                    {
                        if (pageClass.No == PageNow.No)
                            continue;
                        //CCDCollection.SetExposure(pageClass.Exposure, pageClass.CamIndex);
                        pageClass.SetbmpORG((PageOPTypeEnum)cboPageOPType.SelectedIndex, CamActClass.Instance.GetImage(pageClass.No));
                        //pageClass.SetbmpORG((PageOPTypeEnum)cboPageOPType.SelectedIndex, CamActClass.Instance.GetImage(pageClass.CamIndex));
                        //i++;
                    }

                    break;
                case CameraActionMode.CAM_STATIC:

                    //current page
                    //if (PageNow.ExposureString.Trim() == "")
                    CCDCollection.SetExposure((float)numExposure.Value, PageNow.CamIndex);
                    //else
                    //    CCDCollection.SetExposure(PageNow.ExposureString, PageNow.CamIndex);

                    PageNow.SetbmpORG((PageOPTypeEnum)cboPageOPType.SelectedIndex, CCDCollection.GetBMP(PageNow.CamIndex, true));
                    PAGEUI.RegetPage((PageOPTypeEnum)cboPageOPType.SelectedIndex);


                    foreach (PageClass pageClass in ENVNow.PageList)
                    {
                        if (pageClass.No == PageNow.No)
                            continue;

                        //if (pageClass.ExposureString.Trim() == "")
                        //    CCDCollection.SetExposure((float)numExposure.Value, pageClass.CamIndex);
                        //else
                        CCDCollection.SetExposure(pageClass.Exposure, pageClass.CamIndex);
                        pageClass.SetbmpORG((PageOPTypeEnum)cboPageOPType.SelectedIndex, CCDCollection.GetBMP(pageClass.CamIndex, true));
                    }

                    break;
            }

            _msgProcessForm.Close();
            _msgProcessForm.Dispose();

        }
        /// <summary>
        /// 一键设置亮度
        /// </summary>
        private void _ax_SetExposureForAllPages()
        {
            if (!_ax_CheckPageCountAndStepCount())
            {
                MessageBox.Show("页面与步数不符，请检查。", "一键亮度", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageForm _msgProcessForm = new MessageForm("一键设定亮度中...", true);
            _msgProcessForm.Show();
            _msgProcessForm.Refresh();
           
            CCDCollection.SetExposure((float)numExposure.Value, PageNow.CamIndex);
            foreach (PageClass pageClass in ENVNow.PageList)
            {
                if (pageClass.No == PageNow.No)
                    continue;

                pageClass.Exposure = (int)numExposure.Value;
                CCDCollection.SetExposure(pageClass.Exposure, pageClass.CamIndex);
            }

            _msgProcessForm.Close();
            _msgProcessForm.Dispose();

        }
        private bool _ax_CheckPageCountAndStepCount()
        {
            switch (OPTION)
            {
                case OptionEnum.MAIN_SERVICE:
                case OptionEnum.MAIN_X6:
                    return true;//德龙激光 自动通过增加减少页面来判断步数
                    break;
            }

            EnvAnalyzePostionSettings envAnalyzePostionSettings = new EnvAnalyzePostionSettings(ENVNow.GeneralPosition);
            envAnalyzePostionSettings.EnvAnalyzePostions();
            return ENVNow.PageList.Count == envAnalyzePostionSettings.GetImageCount;
            //return ENVNow.PageList.Count == CamActClass.Instance.StepCount;
        }
        private void _ax_SynPageCountAndStepCount()
        {
            int _count = 0;
            if (ENVNow.PageList.Count > CamActClass.Instance.StepCount)
            {
                _count = ENVNow.PageList.Count - CamActClass.Instance.StepCount;
                int index = 0;
                while (index < _count)
                {
                    //btnCutPage.PerformClick();

                    PageClass tmppage = ENVNow.PageList[cboPageNo.Items.Count - 1];
                    CutPage(tmppage);

                    index++;
                }
            }
            else if (ENVNow.PageList.Count < CamActClass.Instance.StepCount)
            {
                _count = CamActClass.Instance.StepCount - ENVNow.PageList.Count;
                int index = 0;
                while (index < _count)
                {
                    btnAddPage.PerformClick();
                    index++;
                }
            }
            _ax_GetImageForAllPages();
        }
        private void _ax_GoCurrentPosition()
        {
            if (!_ax_CheckPageCountAndStepCount())
            {
                MessageBox.Show("页面与步数不符，请检查。", "定位", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            EnvAnalyzePostionSettings envAnalyzePostionSettings = new EnvAnalyzePostionSettings(ENVNow.GeneralPosition);
            envAnalyzePostionSettings.EnvAnalyzePostions();

            MACHINESDM1.GoXPosition(envAnalyzePostionSettings.GetImagePostions[cboPageNo.SelectedIndex].ToString());

        }
        private void _ax_AutoRectPosition()
        {
            if (!_ax_CheckPageCountAndStepCount())
            {
                MessageBox.Show("页面与步数不符，请检查。", "自动生成", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (ENVNow.PageList.Count < 1)
            {
                MessageBox.Show("页面数量不足，请检查。", "自动生成", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            PageClass pagefrist = ENVNow.PageList[0];
            PageClass pagelast = ENVNow.PageList[cboPageNo.Items.Count - 1];

            if (pagefrist.AnalyzeRoot.BranchList.Count < 0 || pagelast.AnalyzeRoot.BranchList.Count < 0)
            {
                MessageBox.Show("左上角和右下角的框不存在，生成失败。", "自动生成", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageForm _msgProcessForm = new MessageForm("自动生成中...", true);
            _msgProcessForm.Show();
            _msgProcessForm.Refresh();

            //1.先算出所有页面的宽度(bmp.width*page.count)和高度bmp.height
            int iwidth = pagefrist.GetbmpORG().Width * ENVNow.PageList.Count;
            int iheight = pagefrist.GetbmpORG().Height;

            //2.拿到页面的框的位置

            AnalyzeClass analyze1 = pagefrist.AnalyzeRoot.BranchList[0];

            RectangleF rectffrist = new RectangleF(analyze1.myOPRectF.X,
                                                analyze1.myOPRectF.Y,
                                                analyze1.myOPRectF.Width,
                                                analyze1.myOPRectF.Height);

            AnalyzeClass analyze2 = pagelast.AnalyzeRoot.BranchList[0];

            RectangleF rectflast = new RectangleF(analyze2.myOPRectF.X,
                                                analyze2.myOPRectF.Y + pagefrist.GetbmpORG().Width * (ENVNow.PageList.Count - 1),
                                                analyze2.myOPRectF.Width,
                                                analyze2.myOPRectF.Height);

            //PageNow.SetbmpORG((PageOPTypeEnum)cboPageOPType.SelectedIndex, CamActClass.Instance.GetImage(PageNow.No));
            //PAGEUI.RegetPage((PageOPTypeEnum)cboPageOPType.SelectedIndex);

            ////int i = 1;
            //foreach (PageClass pageClass in ENVNow.PageList)
            //{
            //    if (pageClass.No == PageNow.No)
            //        continue;
            //    //CCDCollection.SetExposure(pageClass.Exposure, pageClass.CamIndex);
            //    pageClass.SetbmpORG((PageOPTypeEnum)cboPageOPType.SelectedIndex, CamActClass.Instance.GetImage(pageClass.No));
            //    //pageClass.SetbmpORG((PageOPTypeEnum)cboPageOPType.SelectedIndex, CamActClass.Instance.GetImage(pageClass.CamIndex));
            //    //i++;
            //}

            _msgProcessForm.Close();
            _msgProcessForm.Dispose();

        }
        #endregion
    }
}
