using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using JetEazy;
using JetEazy.BasicSpace;
using JetEazy.ControlSpace;
using JzOCR;
using JzOCR.OPSpace;
using JzOCR.FormSpace;
using JzKHC.OPSpace;
using JzKHC.FormSpace;

using JzMSR;
using JzMSR.OPSpace;
using JzMSR.FormSpace;
using Allinone.BasicSpace;

namespace Allinone.UISpace.STPUISpace
{
    public partial class StpUI : UserControl
    {
        enum TageEnum
        {
            MODIFY,
            OK,
            CANCEL,

            SETUPKHC,
            SETUPOCR,
            SETUPMSR,
            SETUPJSP,
            PGPOINT,
            CALIBRATE,
            ASSIGN,
            GETPATH,
            RESERVER,

            EEEECODE,
            CHECKSET,

            ICAM0,
            ICAM1,

            ISSAVEWITHTIMESTAMP,
            SETUPEXPOSURE,

            CORRECT,

            CHANGELANGUAGE,
            COMMONTEST,
        }

        VersionEnum VERSION;
        OptionEnum OPTION;

        GroupBox grpsetup;

        AdjUI ADJUI;

        Button btnModify;
        Button btnOK;
        Button btnCancel;

        Button btnSetupKHC;
        Button btnSetupOCR;
        Button btnSetupMSR;
        Button btnSetupJSP;
        Button btnPGPoint;
        Button btnCalibrate;
        Button btnAssign;
        Button btnReServer;
        Button btnEEEECODE;
        Button btnSetExposure;
        Button btnCheckSet;
        Button btnADD80002;
        Button btnADD80003;
        Button btnADD80004;
        Button btnR3Login;
        Button btnResetINI;
        Button btnLanguage;

        Button btnICAM0;
        Button btnICAM1;

        Button btnCorrect;
        Button btnCommonTest;

        ComboBox cboPGIndex;

        NumericUpDown numRetestTime;
        Label lblShopFloorPath;
        Label lblRecodeRepoer;
        Button btnRecodeReset;
        Button btnGetPath;

        CheckBox chkIsSaveWithTimeStamp;
        CheckBox chbIsMultiThread;
        CheckBox chbIsAutoDebug;
        CheckBox chkIsAllSize;

        OCRCollectionClass OCRCollection
        {
            get
            {
                return Universal.OCRCollection;
            }
        }
        MSRCollectionClass MSRCollection
        {
            get
            {
                return Universal.MSRCollection;
            }
        }
        CCDCollectionClass CCDCollection
        {
            get
            {
                return Universal.CCDCollection;
            }
        }

        KHCClass KHCCollection
        {
            get
            {
                return Universal.KHCCollection;
            }
        }

        bool IsNeedToChange = false;
        Allinone.BasicSpace.JzINIPropertyGridClass m_JzINIptGrid = new BasicSpace.JzINIPropertyGridClass();
        Allinone.BasicSpace.JzINIMAIN_X6PropertyGridClass m_mainx6Grid = new BasicSpace.JzINIMAIN_X6PropertyGridClass();
        Allinone.BasicSpace.JzINIMAIN_SDPropertyGridClass m_mainsdGrid = new BasicSpace.JzINIMAIN_SDPropertyGridClass();

        public StpUI()
        {
            InitializeComponent();
            InitialInternal();
        }
        void InitialInternal()
        {
            grpsetup = groupBox1;
            ADJUI = adjUI1;

            btnOK = button4;
            btnCancel = button6;
            btnModify = button3;
            btnSetupOCR = button1;
            btnSetupMSR = button2;
            btnSetupJSP = button5;
            btnPGPoint = button7;
            btnCalibrate = button8;
            btnAssign = button9;
            btnSetupKHC = button13;
            btnReServer = button14;
            btnEEEECODE = button15;
            btnSetExposure = button16;
            btnCheckSet = button17;
            btnICAM0 = button11;
            btnICAM1 = button12;
            chbIsAutoDebug = checkBox2;
            chbIsMultiThread = checkBox3;
            btnADD80002 = button18;
            btnADD80003 = button21;
            btnADD80004 = button22;
            btnCorrect = button19;
            btnRecodeReset = button20;
            btnR3Login = button23;
            btnResetINI = button24;
            btnLanguage = button25;
            btnCommonTest = button26;

            chkIsAllSize = checkBox4;

            btnRecodeReset.Click += BtnRecodeReset_Click;
            btnResetINI.Click += BtnResetINI_Click;

            chbIsAutoDebug.Checked = Universal.isAutoDebug;
            chbIsMultiThread.Checked = Universal.IsMultiThread;

            chbIsMultiThread.CheckedChanged += ChbIsMultiThread_CheckedChanged;
            chbIsAutoDebug.CheckedChanged += ChbIsAutoDebug_CheckedChanged;

            chkIsSaveWithTimeStamp = checkBox1;
            cboPGIndex = comboBox1;

            btnGetPath = button10;
            numRetestTime = numericUpDown1;
            lblShopFloorPath = label5;

            lblRecodeRepoer = label1;

            btnOK.Tag = TageEnum.OK;
            btnCancel.Tag = TageEnum.CANCEL;
            btnModify.Tag = TageEnum.MODIFY;
            btnSetupOCR.Tag = TageEnum.SETUPOCR;
            btnSetupMSR.Tag = TageEnum.SETUPMSR;
            btnCalibrate.Tag = TageEnum.CALIBRATE;
            btnAssign.Tag = TageEnum.ASSIGN;
            btnGetPath.Tag = TageEnum.GETPATH;
            btnSetupKHC.Tag = TageEnum.SETUPKHC;
            btnReServer.Tag = TageEnum.RESERVER;
            btnEEEECODE.Tag = TageEnum.EEEECODE;
            btnSetExposure.Tag = TageEnum.SETUPEXPOSURE;
            btnCheckSet.Tag = TageEnum.CHECKSET;
            btnICAM0.Tag = TageEnum.ICAM0;
            btnICAM1.Tag = TageEnum.ICAM1;

            btnSetupJSP.Tag = TageEnum.SETUPJSP;
            btnPGPoint.Tag = TageEnum.PGPOINT;
            btnCorrect.Tag = TageEnum.CORRECT;

            chkIsSaveWithTimeStamp.Tag = TageEnum.ISSAVEWITHTIMESTAMP;
            btnLanguage.Tag = TageEnum.CHANGELANGUAGE;
            btnCommonTest.Tag = TageEnum.COMMONTEST;


            btnOK.Click += btn_Click;
            btnCancel.Click += btn_Click;
            btnModify.Click += btn_Click;
            btnSetupKHC.Click += btn_Click;
            btnSetupOCR.Click += btn_Click;
            btnSetupMSR.Click += btn_Click;
            btnSetupJSP.Click += btn_Click;
            btnPGPoint.Click += btn_Click;
            btnCalibrate.Click += btn_Click;
            btnAssign.Click += btn_Click;
            btnGetPath.Click+= btn_Click;
            btnICAM0.Click += btn_Click;
            btnICAM1.Click += btn_Click;
            btnReServer.Click += btn_Click;
            btnEEEECODE.Click += btn_Click;
            btnSetExposure.Click += btn_Click;
            btnCheckSet.Click += btn_Click;
            btnCorrect.Click += btn_Click;
            chkIsSaveWithTimeStamp.CheckedChanged += chkIsSaveWithTimeStamp_CheckedChanged;
            btnR3Login.Click += BtnR3Login_Click;
            btnLanguage.Click += btn_Click;
            btnCommonTest.Click += btn_Click;

            ADJUI.TriggerMoveScreen += ADJUI_TriggerMoveScreen;

            DBStatus = DBStatusEnum.NONE;

            btnCorrect.Visible = true;

            chkIsAllSize.Visible = false;
            switch (Universal.OPTION)
            {
                case OptionEnum.MAIN_SDM3:
                case OptionEnum.MAIN_SDM2:
                case OptionEnum.MAIN_SDM1:
                case OptionEnum.MAIN_X6:
                case OptionEnum.MAIN_SD:
                case OptionEnum.MAIN_SERVICE:
                    #region 隐藏按钮

                    //INIpropertyGrid.Visible = false;
                    button1.Visible = false;
                    button2.Visible = false;
                    button5.Visible = false;
                    button7.Visible = false;
                    button19.Visible = false;
                    button13.Visible = false;
                    button16.Visible = false;
                    button23.Visible = false;
                    button15.Visible = false;
                    button8.Visible = false;
                    //button14.Visible = false;
                    button18.Visible = false;
                    button21.Visible = false;
                    button22.Visible = false;
                    button24.Visible = false;
                    groupBox2.Visible = false;
                    groupBox3.Visible = false;
                    button9.Visible = false;
                    //button17.Visible = false;
                    //button19.Visible = false;
                    checkBox1.Visible = false;
                    checkBox2.Visible = false;
                    //checkBox3.Visible = false;

                    #endregion

                    btnReServer.Text = "测试更换底图";


                    chkIsAllSize.Visible = false;
                    chkIsAllSize.CheckedChanged += ChkIsAllSize_CheckedChanged;
                    ToolTip myToolTip = new ToolTip();
                    myToolTip.SetToolTip(chkIsAllSize, "切换状态后需要重启程序才能生效。");
                    btnCommonTest.Visible = false;

                    break;
            }


            //FillDisplay();

            btnADD80002.Visible = false;
            btnADD80003.Visible = false;
         //   btnR3Login.Visible = false;
            switch (Universal.OPTION)
            {
                case OptionEnum.R32:
                case OptionEnum.R9:
                case OptionEnum.R5:
                case OptionEnum.R1:
                    btnADD80002.Visible = true;
                    btnADD80003.Visible = true;
                    break;
                case OptionEnum.R3:
                case OptionEnum.C3:
                    Universal.C3TickStop += Universal_R3TickStop;
                    //      btnCorrect.Visible = true;
                    btnR3Login.Visible = true;
                           break;

            }
            btnADD80002.Click += BtnADD80002_Click;
            btnADD80003.Click += BtnADD80003_Click;
            btnADD80004.Click += BtnADD80004_Click;
            ShowRecodeData();
          
        }

        private void BtnLanguage_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ChkIsAllSize_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            INI.CHANGE_FILE_PATH = (chkIsAllSize.Checked ? 1 : 0);
        }

        private void BtnResetINI_Click(object sender, EventArgs e)
        {
            INI.Load();
            MessageBox.Show("INI 已重新载入,有的设定需要重启程序才能启用!");
        }

        private void BtnR3Login_Click(object sender, EventArgs e)
        {
          //  btnR3Login.BackColor = Color.Red;
            switch (Universal.OPTION)
            {
                case OptionEnum.R3:
                case OptionEnum.C3:
                    FormSpace.R3LoginForm form = new FormSpace.R3LoginForm();
                    if (form.ShowDialog() == DialogResult.Cancel)
                        Universal.OnR3TickStop("2");
                    form.Dispose();
                    break;
                case OptionEnum.R9:
                case OptionEnum.R32:
                case OptionEnum.R5:
                case OptionEnum.R15:
                case OptionEnum.R1:

                    FormSpace.R3LoginForm form2 = new FormSpace.R3LoginForm("请PE扫入权限码!");
                    if (form2.ShowDialog() == DialogResult.Cancel)
                        Universal.OnR3TickStop("2");
                    form2.Dispose();
                    break;
        }
        }

        private void Universal_R3TickStop(string movestring)
        {
          
            if (movestring == "1")
                btnR3Login.BackColor = Color.Red;

            if (movestring == "2")
            {
                Universal.isR3ByPass = false;
                btnR3Login.BackColor = Color.FromArgb(192, 255, 192);
                JzToolsClass tools = new JzToolsClass();
                string strsavedata = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 退出复判功能:" +Environment.NewLine;
                string strpath = "D:\\Log\\Retrial.log";

                if (!System.IO.Directory.Exists("D:\\Log\\"))
                    System.IO.Directory.CreateDirectory("D:\\Log\\");
                tools.SaveDataEX(strsavedata, strpath);
            }
        }

        private void BtnRecodeReset_Click(object sender, EventArgs e)
        {
          //  INI.iSNNGCOUNT = 0;
            INI.iALLCOUNT = 0;
            INI.iKEYCAPNG = 0;
            INI.iLASERNG = 0;
            INI.iPASSCOUNT = 0;
            INI.iSCREWNG = 0;
            INI.iNGCOUNT = 0;
            INI.iALLNGCOUNT = 0;
            INI.iLKNGCOUNT = 0;
            INI.iLSNGCOUNT = 0;
            INI.iKSNGCOUNT = 0;

            ShowRecodeData();
        }

        private void BtnADD80002_Click(object sender, EventArgs e)
        {
            FormSpace.Modify80000Form form = new FormSpace.Modify80000Form(true,80002);
            if (form.ShowDialog() == DialogResult.OK)
                MessageBox.Show("请重新启动程式!");

           
        }
        private void BtnADD80003_Click(object sender, EventArgs e)
        {
            FormSpace.Modify80000Form form = new FormSpace.Modify80000Form(true, 80003);
            if (form.ShowDialog() == DialogResult.OK)
                MessageBox.Show("请重新启动程式!");
        }
        private void BtnADD80004_Click(object sender, EventArgs e)
        {
            FormSpace.Modify80000Form form = new FormSpace.Modify80000Form(true, 80004);
            if (form.ShowDialog() == DialogResult.OK)
                MessageBox.Show("请重新启动程式!");
        }

        private void ChbIsAutoDebug_CheckedChanged(object sender, EventArgs e)
        {
            Universal.isAutoDebug = chbIsAutoDebug.Checked;
        }

        private void ChbIsMultiThread_CheckedChanged(object sender, EventArgs e)
        {
            Universal.IsMultiThread = chbIsMultiThread.Checked;
        }

        private void chkIsSaveWithTimeStamp_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            INI.ISSAVEWITHTIMESTAMP = chkIsSaveWithTimeStamp.Checked;
        }

        public void Initial(VersionEnum version,OptionEnum option, List<CCDRectRelateIndexClass> ccdrelateindexlist, bool ishavebackground)
        {
            VERSION = version;
            OPTION = option;

            ADJUI.Initial(ccdrelateindexlist, ishavebackground);

            switch(VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch(OPTION)
                    {
                        case OptionEnum.MAIN:

                            int i = 0;
                            while (i < 7)
                            {
                                cboPGIndex.Items.Add(i.ToString());
                                i++;
                            }
                            cboPGIndex.SelectedIndex = 0;

                            break;
                        case OptionEnum.R32:
                        case OptionEnum.R26:
                        case OptionEnum.R15:
                        case OptionEnum.R9:
                        case OptionEnum.R5:
                        case OptionEnum.R3:
                        case OptionEnum.C3:
                        case OptionEnum.R1:
                            btnSetupOCR.Visible = true;
                            btnSetupMSR.Visible = false;
                            btnSetupJSP.Visible = false;
                            btnPGPoint.Visible = false;
                            btnCalibrate.Visible = false;
                            cboPGIndex.Visible = false;

                            btnICAM0.Visible = true;
                            btnICAM1.Visible = true;
                            btnEEEECODE.Visible = true;
                            break;
                        default:
                            btnEEEECODE.Visible = false;
                            btnSetupOCR.Visible = false;
                            btnSetupMSR.Visible = false;
                            btnSetupJSP.Visible = false;
                            btnPGPoint.Visible = false;
                            btnCalibrate.Visible = false;
                            cboPGIndex.Visible = false;

                            break;
                    }
                    break;
                default:
                    btnEEEECODE.Visible = false;
                    btnSetupOCR.Visible = false;
                    btnSetupMSR.Visible = false;
                    btnSetupJSP.Visible = false;
                    btnPGPoint.Visible = false;
                    btnCalibrate.Visible = false;
                    cboPGIndex.Visible = false;

                    break;
            }

            FillDisplay();
        }

        private void ADJUI_TriggerMoveScreen(string movestring)
        {
            OnTriggerMoveScreen(movestring);
        }
        private void btn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            switch ((TageEnum)btn.Tag)
            {
                case TageEnum.OK:
                    DBStatus = DBStatusEnum.NONE;

                    INI.RETESTTIME = (int)numRetestTime.Value;

                    OnTrigger(INIStatusEnum.OK);
                    break;
                case TageEnum.CANCEL:
                    DBStatus = DBStatusEnum.NONE;
                    OnTrigger(INIStatusEnum.CANCEL);
                    break;
                case TageEnum.MODIFY:

                    DBStatus = DBStatusEnum.MODIFY;
                    OnTrigger(INIStatusEnum.EDIT);
                    break;
                case TageEnum.SETUPOCR:
                    JetEazy.LoggerClass.Instance.WriteLog("打开 OCR 设定");
                    SetupOCR();
                    break;
                case TageEnum.SETUPMSR:
                    JetEazy.LoggerClass.Instance.WriteLog("打开 MSR 设定");
                    SetupMSR();
                    break;
                case TageEnum.SETUPKHC:
                    JetEazy.LoggerClass.Instance.WriteLog("打开 KHC 设定");
                    SetupKHC();
                    break;
                case TageEnum.SETUPJSP:
                    JetEazy.LoggerClass.Instance.WriteLog("打开 JSP 设定");
                    SetupJSP();
                    break;
                case TageEnum.PGPOINT:
                    JetEazy.LoggerClass.Instance.WriteLog("打开 PGPOINT 设定");
                    PGPoint();
                    break;
                case TageEnum.CALIBRATE:
                    OnTrigger(INIStatusEnum.CALIBRATE);
                    break;
                case TageEnum.ASSIGN:
                    OnTrigger(INIStatusEnum.SHOWASSIGN);
                    break;
                case TageEnum.GETPATH:
                    GetPath();
                    break;
                case TageEnum.ICAM0:
                    JetEazy.LoggerClass.Instance.WriteLog("打开 ICAM0 设定");
                    CCDCollection.SetPropetry(this.Handle, 1);
                    break;
                case TageEnum.ICAM1:
                    JetEazy.LoggerClass.Instance.WriteLog("打开 ICAM1 设定");
                    CCDCollection.SetPropetry(this.Handle, 2);
                    break;
                case TageEnum.RESERVER:

                    switch(OPTION)
                    {
                        case OptionEnum.MAIN_SERVICE:
                            TestPageTrain();
                            break;
                        case OptionEnum.MAIN_X6:
                            ReRecipeImageAndTrain();
                            break;
                        case OptionEnum.MAIN:
                            ReConnectServer();
                            break;
                    }
                    break;
                case TageEnum.EEEECODE:
                    JetEazy.LoggerClass.Instance.WriteLog("打开 EEEECODE 设定");
                    Universal.LOADCOLORTABLE();
                    break;
                case TageEnum.SETUPEXPOSURE:
                    JetEazy.LoggerClass.Instance.WriteLog("打开 SETUPEXPOSURE 设定");
                    SetExposure();
                    break;
                case TageEnum.CHECKSET:
                    CHECKSET();
                    break;
                case TageEnum.CORRECT:

                    FormSpace.CorrectForm form = new FormSpace.CorrectForm();
                    form.ShowDialog();
                    break;
                case TageEnum.CHANGELANGUAGE:
                    JetEazy.LoggerClass.Instance.WriteLog("切换语言");

                    INI.LANGUAGE = 1 - INI.LANGUAGE;
                    INI.Save();
                    //OnTrigger(INIStatusEnum.CHANGELANGUAGE);

                    string msg = "切换 " + (INI.LANGUAGE == 1 ? "英文" : "中文") + " 成功";
                    MessageBox.Show(msg + " \n\r重启程序生效。");

                    break;
                case TageEnum.COMMONTEST:
                    Allinone.FormSpace.frmTestCommon frmTestCommon = new Allinone.FormSpace.frmTestCommon();
                    frmTestCommon.ShowDialog();
                    break;
            }
        }
        Allinone.FormSpace.OtherSetExposureForm SetExposureFrm;
        void SetExposure()
        {
            OnTrigger(INIStatusEnum.SETUP_PARA);
            SetExposureFrm = new FormSpace.OtherSetExposureForm();
            SetExposureFrm.ShowDialog();
        }
        void CHECKSET()
        {
            Allinone.FormSpace.ModifySelectForm modify = new FormSpace.ModifySelectForm(Universal.RCPDB);
            if (modify.ShowDialog() == DialogResult.OK)
            {

            }
        }
        void ReConnectServer()
        {

            if (MessageBox.Show("Question:\t\nDo you ReConnecting ?", "ReConnectServer", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                return;

            bool bCrystalOK = true;
            bool bJumboOK = true;
            string strmessage = "";
            if (INI.ISUSECRYSTALSERVER)
            {
                bCrystalOK = false;
                try
                {
                    strmessage += Environment.NewLine;
                    strmessage += "Crystal Check Connecting..." + Environment.NewLine;
                    bCrystalOK = Universal.IXCONNECTCRYSTAL.GetBroadCast();
                    if (!bCrystalOK)
                    {
                        Universal.RESULT.ReStartCrystalBroadCast();
                        strmessage += "Crystal ReConnect OK." + Environment.NewLine;
                    }
                    else
                        strmessage += "Crystal Do Not ReConnect." + Environment.NewLine;
                }
                catch(Exception ex)
                {
                    JetEazy.LoggerClass.Instance.WriteException(ex);
                    bCrystalOK = false;
                    strmessage += "Crystal ReConnect Exception: Reason:" + ex.Message + Environment.NewLine;
                }
            }

            if (INI.ISUSEJUMBOSERVER)
            {
                bJumboOK = false;
                try
                {
                    strmessage += Environment.NewLine;
                    strmessage += "Jumbo Check Connecting..." + Environment.NewLine;
                    bJumboOK = Universal.IXCONNECTJUMBO.GetBroadCast();
                    if (!bJumboOK)
                    {
                        Universal.RESULT.ReStartJumboBroadCast();
                        strmessage += "Jumbo ReConnect OK." + Environment.NewLine;
                    }
                    else
                        strmessage += "Jumbo Do Not ReConnect." + Environment.NewLine;
                }
                catch(Exception ex)
                {
                    JetEazy.LoggerClass.Instance.WriteException(ex);
                    bJumboOK = false;
                    strmessage += "Jumbo ReConnect Exception: Reason:" + ex.Message + Environment.NewLine;
                }
            }

            //if(!bCrystalOK || !bJumboOK)
            {
                MessageBox.Show("Information:\t\n" + strmessage, "ReConnectServer", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }
        void ReRecipeImageAndTrain()
        {
            OnTrigger(INIStatusEnum.RECIPE_IMAGE_AND_TRAIN);
        }

        //ServiceClientClass xClient = new ServiceClientClass();
        void TestPageTrain()
        {
            //xClient.PageTrain("127.0.0.1", 6008);
            OnTrigger(INIStatusEnum.TEST_PAGE_TRAIN);
        }

        List<Point> PGList = new List<Point>();//
        List<Rectangle> PGRECTList = new List<Rectangle>();
        JzToolsClass JzTools = new JzToolsClass();
        MSRClass MSR
        {
            get { return Universal.MSRCollection.DataLast; }
        }
        void PGPoint()
        {
            if (cboPGIndex.SelectedIndex < cboPGIndex.Items.Count - 1)
            {
                PGList.Clear();
                foreach (Point pt in Universal.SCREENPOINTS.m_JzScreenList[cboPGIndex.SelectedIndex].PointList)
                {
                    PointF ptf = MSR.ViewToWorld(pt);
                    PGList.Add(new Point((int)ptf.X, (int)ptf.Y));
                }
                Universal.SCREENPOINTS.m_showmypoints.DrawMyPaints(PGList);
            }
            else
            {
                if (Universal.SCREENPOINTS.m_JzScreenList.Count > 6)
                {
                    PGRECTList.Clear();
                    #region PG-Rects

                    //加入矩形打光
                    foreach (JzScreenPoints.OPSpace.BASISClass basis in Universal.SCREENPOINTS.m_JzScreenList[6].BasisList)
                    {
                        Rectangle rect = basis.Cell.RectCell;
                        Point ptLeftTop = rect.Location;
                        Point ptRightBottom = new Point(rect.Right, rect.Bottom);

                        PointF ptfLeftTop = MSR.ViewToWorld(ptLeftTop);
                        PointF ptfRightBottom = MSR.ViewToWorld(ptRightBottom);

                        ptLeftTop = new Point((int)ptfLeftTop.X, (int)ptfLeftTop.Y);
                        ptRightBottom = new Point((int)ptfRightBottom.X, (int)ptfRightBottom.Y);

                        rect = JzTools.RectTwoPoint(ptLeftTop, ptRightBottom);
                        PGRECTList.Add(rect);
                    }
                    Universal.SCREENPOINTS.m_showmypoints.DrawMyPaintRectS(PGRECTList);

                    #endregion
                }
            }



        }

        OCRForm OCRFrm;
        void SetupOCR()
        {
            OnTrigger(INIStatusEnum.SETUP_PARA);
            OCRFrm = new OCRForm(OCRCollection, CCDCollection);
            OCRFrm.ShowDialog();
        }

        MSRForm MSRFrm;
        void SetupMSR()
        {
            OnTrigger(INIStatusEnum.SETUP_PARA);
            MSRFrm = new MSRForm(MSRCollection, CCDCollection);
            MSRFrm.ShowDialog();
        }

        KHCForm KHCFrm;
        void SetupKHC()
        {
            return;
            OnTrigger(INIStatusEnum.SETUP_PARA);
            KHCFrm = new KHCForm(KHCCollection);
            KHCFrm.ShowDialog();
        }

        JzScreenPoints.JzScreenForm JSPFrm;
        void SetupJSP()
        {
            OnTrigger(INIStatusEnum.SETUP_PARA);
            JSPFrm = new JzScreenPoints.JzScreenForm(Universal.SCREENPOINTS, 0, Universal.CCDCollection);
            JSPFrm.ShowDialog();
        }
        void GetPath()
        {
            string pathstr = PathPicker("Please Select Path", INI.SHOPFLOORPATH);

            if (pathstr != "")
            {
                INI.SHOPFLOORPATH = pathstr;
            }

            FillDisplay();
        }
        public void ResetChecks()
        {
            ADJUI.ResetChks();
        }

        DBStatusEnum myDBStatus = DBStatusEnum.NONE;
        DBStatusEnum DBStatus
        {
            get
            {
                return myDBStatus;
            }
            set
            {
                myDBStatus = value;

                switch (myDBStatus)
                {
                    case DBStatusEnum.MODIFY:
                        grpsetup.Enabled = true;

                        btnModify.Visible = false;

                        btnOK.Visible = true;
                        btnCancel.Visible = true;

                        break;
                    case DBStatusEnum.NONE:
                        grpsetup.Enabled = false;

                        btnModify.Visible = true;

                        btnOK.Visible = false;
                        btnCancel.Visible = false;

                        break;
                }
            }
        }

        public void FillDisplay()
        {
            IsNeedToChange = false;

            chkIsSaveWithTimeStamp.Checked = INI.ISSAVEWITHTIMESTAMP;

            lblShopFloorPath.Text = INI.SHOPFLOORPATH;
            chkIsSaveWithTimeStamp.Checked = INI.ISSAVEWITHTIMESTAMP;
            numRetestTime.Value = INI.RETESTTIME;
            chkIsAllSize.Checked = INI.CHANGE_FILE_PATH == 1;

            switch(VERSION)
            {
                case VersionEnum.ALLINONE:

                    switch(OPTION)
                    {
                        case OptionEnum.MAIN_SD:
                            INIpropertyGrid.SelectedObject = m_mainsdGrid;
                            break;
                        case OptionEnum.MAIN_SDM3:
                        case OptionEnum.MAIN_SDM2:
                        case OptionEnum.MAIN_SDM1:
                        case OptionEnum.MAIN_X6:
                        case OptionEnum.MAIN_SERVICE:
                            INIpropertyGrid.SelectedObject = m_mainx6Grid;
                            break;
                        default:
                            INIpropertyGrid.SelectedObject = m_JzINIptGrid;
                            break;
                    }

                    break;
            }


            IsNeedToChange = true;
        }

        public void ShowRecodeData()
        {
            lblRecodeRepoer.Text = "总数:" + INI.iALLCOUNT + Environment.NewLine +
            "PASS:" + INI.iPASSCOUNT + Environment.NewLine +
            "FAIL:" + INI.iNGCOUNT + Environment.NewLine +
            "键盘NG:" + INI.iKEYCAPNG + Environment.NewLine +
            "镭雕NG:" + INI.iLASERNG + Environment.NewLine +
            "螺丝NG:" + INI.iSCREWNG + Environment.NewLine +
            "键盘/镭雕NG:" + INI.iLKNGCOUNT + Environment.NewLine +
            "键盘/螺丝NG:" + INI.iKSNGCOUNT + Environment.NewLine +
            "螺丝/镭雕NG:" + INI.iLSNGCOUNT + Environment.NewLine +
            "螺丝/镭雕/键盘NG:" + INI.iALLNGCOUNT;
        }

        string PathPicker(string Description, string DefaultPath)
        {
            string retStr = "";

            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.Description = Description;
            fd.ShowNewFolderButton = false;
            fd.SelectedPath = DefaultPath;

            if (fd.ShowDialog().Equals(DialogResult.OK))
            {
                if (fd.SelectedPath != "")
                    retStr = fd.SelectedPath;
            }
            else
                retStr = "";

            return retStr;
        }

        public delegate void TriggerHandler(INIStatusEnum status);
        public event TriggerHandler TriggerAction;
        public void OnTrigger(INIStatusEnum status)
        {
            if (TriggerAction != null)
            {
                TriggerAction(status);
            }
        }

        public delegate void TriggerMoveScreenHandler(string movestring);
        public event TriggerMoveScreenHandler TriggerMoveScreen;
        public void OnTriggerMoveScreen(string movestring)
        {
            if (TriggerMoveScreen != null)
            {
                TriggerMoveScreen(movestring);
            }
        }
    }
}
