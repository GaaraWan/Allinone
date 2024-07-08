using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using JetEazy.BasicSpace;
using JzKHC.ControlSpace;
using JzKHC.FormSpace;
//using Jumbo301.UniversalSpace;
using JetEazy;

namespace JzKHC.DBSpace
{
    public partial class RECIPEDBUIControl : UserControl
    {
        enum KeyParaEnum : int
        {
            HIGHEST = 0,
            LOWEST = 1,
            SLOPE = 2,
            MUTUAL = 3,
            WRONGCOUNT = 4,
        }

        DBStatusEnum mDBStatus;

        public TextBox txtName;
        public TextBox txtVersion;
        
        public ComboBox cboControl;
        public Button btnSetup;
        //public LIGHTUIControl /*LIGHTUI*/;
        JzToolsClass JzTools = new JzToolsClass();
        
        OPScreenUIClass OPScreenUI;

        public Button btnAnalyseHat;

        public Label lblDatetime;

        public RadioButton[] rdoSide = new RadioButton[(int)SideEnum.COUNT];
        public RadioButton[] rdoTeachingType = new RadioButton[(int)TeachingTypeEnum.COUNT];

        public Button btnManual;

        public Button btnAdd;
        public Button btnModify;
        public Button btnDelete;
        public Button btnCopy;

        public Button btnGetBase;
        public Button btnGetAnalyze;
        public Button btnGetBackgroud;
        public Button btnImport;

        public Button btnStep1;
        public Button btnEDIT;

        public Button btnGoBase;
        public Button btnGoAnalyze;

        public Button btnKeyAssign;

        public Button btnOK;
        public Button btnCancel;

        public Button btnReadData;
        public Button btnExportData;
        public Button btnImportXY;


        public CheckBox chkIsCheckHighest;
        public TextBox txtHighestValue;

        public CheckBox chkIsCheckLowest;
        public TextBox txtLowestValue;

        public CheckBox chkIsCheckSlope;
        public TextBox txtSlopeValue;
        
        public CheckBox chkIsCheckMutual;
        public TextBox txtMutualValue;

        public CheckBox chkIsCheckWrongCount;
        public TextBox txtWrongCount;

        //ScreenClass SCREEN;

        GroupBox grpEditWindow;

        public RECIPEDBUIControl()
        {
            InitializeComponent();
            Initial();
        }
        public void SetBMPDirectly(Bitmap bmp)
        {
            OPScreenUI.SetBMPDirectly(bmp);
        }
        void Initial()
        {
            txtName = textBox1;
            txtVersion = textBox2;

            //cboControl = comboBox1;
            //btnSetup = button10;
            
            rdoSide[(int)SideEnum.SIDE0] = radioButton1;
            rdoSide[(int)SideEnum.SIDE1] = radioButton2;
            rdoSide[(int)SideEnum.SIDE2] = radioButton3;
            rdoSide[(int)SideEnum.SIDE3] = radioButton4;
            rdoSide[(int)SideEnum.SIDE4] = radioButton5;
            rdoSide[(int)SideEnum.SIDE5] = radioButton6;
            rdoSide[(int)SideEnum.SIDE6] = radioButton7;

            //rdoSide[(int)SideEnum.SIDE7] = radioButton11;
            //rdoSide[(int)SideEnum.SIDE8] = radioButton14;
            //rdoSide[(int)SideEnum.SIDE9] = radioButton13;
            //rdoSide[(int)SideEnum.SIDE10] = radioButton12;


            //rdoTeachingType[(int)TeachingTypeEnum.LINE] = radioButton8;
            //rdoTeachingType[(int)TeachingTypeEnum.BASE] = radioButton9;
            //rdoTeachingType[(int)TeachingTypeEnum.KEYCAP] = radioButton10;

            btnKeyAssign = button15;


            OPScreenUI = new OPScreenUIClass(opScreenUIControl1, -1, -2, 1);

            //LIGHTUI = lightuiControl1;
            //LIGHTUI.HideInput();
            //LIGHTUI.SetEnable();

            //btnAdd = button5;
            btnModify = button1;
            //btnDelete = button2;
            btnCopy = button3;

            btnOK = button6;
            btnCancel = button4;

            btnGetBase = button8;
            btnGetAnalyze = button11;
            btnGetBackgroud = button12;
            btnImport = button16;

            //btnStep1 = button7;
            btnEDIT = button9;

            grpEditWindow = groupBox1;

            //btnReadData = button13;
            //btnExportData = button14;
            btnImportXY = button17;

            btnGoBase = button19;
            btnGoAnalyze = button18;

            lblDatetime = label10;

            chkIsCheckHighest = checkBox6;
            txtHighestValue = textBox8;
            txtHighestValue.Tag = (int)KeyParaEnum.HIGHEST;
            txtHighestValue.LostFocus += new EventHandler(txt_LostFocus);


            chkIsCheckLowest = checkBox5;
            txtLowestValue = textBox7;
            txtLowestValue.Tag = (int)KeyParaEnum.LOWEST;
            txtLowestValue.LostFocus += new EventHandler(txt_LostFocus);

            chkIsCheckSlope = checkBox4;
            txtSlopeValue = textBox6;
            txtSlopeValue.Tag = (int)KeyParaEnum.SLOPE;
            txtSlopeValue.LostFocus += new EventHandler(txt_LostFocus);

            chkIsCheckMutual = checkBox8;
            txtMutualValue = textBox10;
            txtMutualValue.Tag = (int)KeyParaEnum.MUTUAL;
            txtMutualValue.LostFocus += new EventHandler(txt_LostFocus);

            chkIsCheckWrongCount = checkBox9;
            txtWrongCount = textBox11;
            txtWrongCount.Tag = (int)KeyParaEnum.WRONGCOUNT;
            txtWrongCount.LostFocus += new EventHandler(txt_LostFocus);

        }

        void txt_LostFocus(object sender, EventArgs e)
        {
            TextBox GetTXT = (TextBox)sender;


            switch ((KeyParaEnum)GetTXT.Tag)
            {
                case KeyParaEnum.HIGHEST:
                case KeyParaEnum.LOWEST:
                case KeyParaEnum.MUTUAL:
                case KeyParaEnum.SLOPE:
                    if (!JzTools.CheckTextBoxIsDouble(GetTXT, 10, 0.01))
                    {
                        GetTXT.Text = "0.010";
                        Application.DoEvents();

                        GetTXT.Focus();
                        return;
                    }
                    else
                    {
                        GetTXT.Text = double.Parse(GetTXT.Text).ToString("0.000");
                    }
                    break;
                case KeyParaEnum.WRONGCOUNT:
                    if (!JzTools.CheckTextBoxIsInteger(GetTXT, 100, 1))
                    {
                        GetTXT.Text = "1";
                        Application.DoEvents();

                        GetTXT.Focus();
                        return;
                    }
                    else
                    {
                        GetTXT.Text = double.Parse(GetTXT.Text).ToString();
                    }
                    break;
            }

        }

        public void SetLanguage(bool IsWithoutInitial)
        {
            //LIGHTUI.SetLanguage(IsWithoutInitial);

            //if (!IsWithoutInitial)
            //{
            //    SCREEN = new ScreenClass("screen_recipe", this);
            //}

            //SCREEN.SetControlLanguage();
        }

        public int GetSideSelection()
        {
            int i = 0;
            while (i < (int)SideEnum.COUNT)
            {
                if (rdoSide[i].Checked)
                    break;
                i++;
            }
            return i;
        }
        public int GetTeachTypeSelection()
        {
            int i = 0;
            while (i < (int)TeachingTypeEnum.COUNT)
            {
                if (rdoTeachingType[i].Checked)
                    break;
                i++;
            }
            return i;
        }
        public DBStatusEnum DBStatus
        {
            get
            {
                return mDBStatus;
            }
            set
            {
                int i = 0;

                while (i < (int)SideEnum.COUNT)
                {
                    rdoSide[i].Enabled = i < INI.SIDECOUNT;
                    i++;
                }

                //rdoTeachingType[(int)TeachingTypeEnum.BASE].Enabled = INI.HAVEKEYBASE;


                mDBStatus = value;
                switch (mDBStatus)
                {
                    case DBStatusEnum.ADD:
                    case DBStatusEnum.MODIFY:
                    case DBStatusEnum.COPY:

                        btnOK.Visible = true;
                        btnCancel.Visible = true;

                        //btnAdd.Visible = false;
                        btnModify.Visible = false;
                        //btnDelete.Visible = false;
                        btnCopy.Visible = false;
                        //btnExportData.Visible = false;

                        grpEditWindow.Enabled = true;

                        break;
                    case DBStatusEnum.NONE:

                        btnOK.Visible = false;
                        btnCancel.Visible = false;

                        //btnAdd.Visible = true;
                        btnModify.Visible = true;
                        //btnDelete.Visible = false;
                        btnCopy.Visible = true;
                        //btnExportData.Visible = true;

                        grpEditWindow.Enabled = false;

                        //rdoSide[(int)SideEnum.SIDE0].Checked = true;
                        //rdoTeachingType[(int)TeachingTypeEnum.LINE].Checked = true;

                        OPScreenUI.OPType = OPTypeEnum.NONE;

                        OPScreenUI.Alignment();

                        break;
                }
            }

        }

        
    }
}
