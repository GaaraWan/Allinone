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

using JzMSR;
using JzMSR.OPSpace;
using JzMSR.FormSpace;
using Allinone.ControlSpace;

namespace Allinone.UISpace.STPUISpace
{
    public partial class StpV1UI : UserControl
    {
        enum TageEnum
        {
            MODIFY,
            OK,
            CANCEL,

            SETUPOCR,
            SETUPMSR,
            ASSIGN,

            GETPATH,
            
            ISSAVEWITHTIMESTAMP,


            GOCAMERALOCATION,
            SETCAMERALOCATION,

            GOLENSLOCATION,
            SETLENSLOCATION,

        }

        VersionEnum VERSION;
        OptionEnum OPTION;

        GroupBox grpsetup;

        Button btnModify;
        Button btnOK;
        Button btnCancel;
        
        Button btnSetupOCR;
        Button btnSetupMSR;
        Button btnAssign;

        NumericUpDown numDelayTime;
        NumericUpDown numRetestTime;
        Label lblShopFloorPath;
        Button btnGetPath;

        Label lblCameraLocation;
        Button btnGoCameraLocation;
        Button btnSetCameraLocation;

        Label lblLensLocation;
        Button btnGoLensLocation;
        Button btnSetLensLocation;
        
        CheckBox chkIsSaveWithTimeStamp;

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
        
        MachineCollectionClass MACHINECollection
        {
            get
            {
                return Universal.MACHINECollection;
            }
        }
        


        bool IsNeedToChange = false;

        public StpV1UI()
        {
            InitializeComponent();
            InitialInternal();
        }
        void InitialInternal()
        {
            grpsetup = groupBox1;

            btnOK = button8;
            btnCancel = button9;
            btnModify = button7;

            btnSetupOCR = button3;
            btnSetupMSR = button1;
            btnAssign = button4;

            btnGetPath = button2;

            numDelayTime = numericUpDown2;
            numRetestTime = numericUpDown1;
            lblShopFloorPath = label5;

            lblCameraLocation = label1;
            lblLensLocation = label2;

            btnSetCameraLocation = button6;
            btnGoCameraLocation = button5;
            btnSetLensLocation = button11;
            btnGoLensLocation = button10;

            chkIsSaveWithTimeStamp = checkBox1;
            
            btnOK.Tag = TageEnum.OK;
            btnCancel.Tag = TageEnum.CANCEL;
            btnModify.Tag = TageEnum.MODIFY;

            btnSetupOCR.Tag = TageEnum.SETUPOCR;
            btnSetupMSR.Tag = TageEnum.SETUPMSR;
            btnGetPath.Tag = TageEnum.GETPATH;
            btnAssign.Tag = TageEnum.ASSIGN;

            btnSetCameraLocation.Tag = TageEnum.SETCAMERALOCATION;
            btnGoCameraLocation.Tag = TageEnum.GOCAMERALOCATION;

            btnSetLensLocation.Tag = TageEnum.SETLENSLOCATION;
            btnGoLensLocation.Tag = TageEnum.GOLENSLOCATION;

            chkIsSaveWithTimeStamp.Tag = TageEnum.ISSAVEWITHTIMESTAMP;

            btnOK.Click += btn_Click;
            btnCancel.Click += btn_Click;
            btnModify.Click += btn_Click;
            btnSetupOCR.Click += btn_Click;
            btnSetupMSR.Click += btn_Click;
            btnGetPath.Click += btn_Click;
            btnAssign.Click += btn_Click;

            btnSetCameraLocation.Click += btn_Click;
            btnGoCameraLocation.Click += btn_Click;
            btnSetLensLocation.Click += btn_Click;
            btnGoLensLocation.Click += btn_Click;

            chkIsSaveWithTimeStamp.CheckedChanged += chkIsSaveWithTimeStamp_CheckedChanged;

            DBStatus = DBStatusEnum.NONE;

            FillDisplay();
        }


        private void chkIsSaveWithTimeStamp_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            INI.ISSAVEWITHTIMESTAMP = chkIsSaveWithTimeStamp.Checked;
        }

        public void Initial(VersionEnum version, OptionEnum option, bool ishavebackground)
        {
            VERSION = version;
            OPTION = option;

            switch (VERSION)
            {
                case VersionEnum.AUDIX:

                    break;
                default:

                    btnSetupOCR.Visible = false;
                    btnSetupMSR.Visible = false;

                    break;
            }
        }
        private void btn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            switch ((TageEnum)btn.Tag)
            {
                case TageEnum.OK:
                    DBStatus = DBStatusEnum.NONE;

                    WriteBack();

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
                    SetupOCR();
                    break;
                case TageEnum.SETUPMSR:
                    SetupMSR();
                    break;
                case TageEnum.ASSIGN:
                    OnTrigger(INIStatusEnum.SHOWASSIGN);
                    break;
                case TageEnum.GETPATH:
                    GetPath();
                    break;
                case TageEnum.GOCAMERALOCATION:
                    MACHINECollection.GoPosition(lblCameraLocation.Text);
                    break;
                case TageEnum.SETCAMERALOCATION:
                    INI.CAMERALOCATION = MACHINECollection.GetPosition();
                    lblCameraLocation.Text = INI.CAMERALOCATION;
                    break;
                case TageEnum.GOLENSLOCATION:
                    MACHINECollection.GoPosition(lblLensLocation.Text);
                    break;
                case TageEnum.SETLENSLOCATION:
                    INI.LENSLOCATION = MACHINECollection.GetPosition();
                    lblLensLocation.Text = INI.LENSLOCATION;
                    break;
            }
        }

        MSRClass MSR
        {
            get { return Universal.MSRCollection.DataLast; }
        }

        OCRForm OCRFrm;
        void SetupOCR()
        {
            OnTrigger(INIStatusEnum.SETUP_PARA);
            OCRFrm = new OCRForm(OCRCollection, Universal.mOCRByPaddle, CCDCollection, Universal.IsNoUseIO, Universal.OPTION);
            OCRFrm.ShowDialog();
        }

        MSRForm MSRFrm;
        void SetupMSR()
        {
            OnTrigger(INIStatusEnum.SETUP_PARA);
            MSRFrm = new MSRForm(MSRCollection, CCDCollection);
            MSRFrm.ShowDialog();
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

            lblShopFloorPath.Text = INI.SHOPFLOORPATH;
            chkIsSaveWithTimeStamp.Checked = INI.ISSAVEWITHTIMESTAMP;
            numRetestTime.Value = INI.RETESTTIME;
            numDelayTime.Value = INI.DELAYTIME;

            lblCameraLocation.Text = INI.CAMERALOCATION;
            lblLensLocation.Text = INI.LENSLOCATION;

            IsNeedToChange = true;
        }

        void WriteBack()
        {
            INI.RETESTTIME = (int)numRetestTime.Value;
            INI.DELAYTIME = (int)numDelayTime.Value;
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
