using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using JetEazy.BasicSpace;
//using Jumbo301.UniversalSpace;
using JzKHC.DBSpace;
using JzKHC.ControlSpace;
using JetEazy;

namespace JzKHC.FormSpace
{
    public partial class KeyAssignForm : Form
    {
        enum KeycapParaEnum : int
        {
            ALIASNAME = 0,
            STANDARDHEIGTH = 1,
            UPPERBOUND = 2,
            LOWERBOUND = 3,
            EXAMDIFF = 4,
            BESIDEDIFF = 5,
            REPORTINDEX = 6,

            D1 = 7,
            D2 = 8,
            D3 = 9,
            D4 = 10,

            GOODRATIO = 11,
            ADJUST = 12,

            CENTERSTANDERHEIGHT = 13,
            CENTERUPPERBOUND = 14,
            CENTERLOWERBOUND= 15,
            XUPPERBOUND =16,
            YUPPERBOUND = 17,
            ADDHEIGHT = 18,

            FLATNESS = 19,
            DEFINEDCODE = 20,
            CORNERBESIDEDIFF = 21,

            KEYCODE=22,

        }

        Button btnOK;
        Button btnCancel;

        OPScreenUIKeyAssignClass OPScreenUIMain;
        OPScreenUIClass OPScreenUISub;
        
        KeyboardClass KEYBOARD
        {
            get
            {
                return Universal.KEYBOARD;
            }
        }
        RecipeDBClass RECIPEDB
        {
            get
            {
                return Universal.RECIPEDB;
            }

        }
        JzToolsClass JzTools = new JzToolsClass();

        ListBox lstKeyassign;

        TextBox txtName;
        Label lblKeyassignInformation;

        TextBox txtAliasName;
        TextBox txtStandardHeight;
        TextBox txtUpperbound;
        TextBox txtLowerbound;
        TextBox txtExamDiff;
        TextBox txtBesideDiff;
        TextBox txtCornerBesideDiff;
        TextBox txtReportIndex;

        TextBox txtD1;
        TextBox txtD2;
        TextBox txtD3;
        TextBox txtD4;

        CheckBox chkIsNoUseArround;
        CheckBox chkIsNoUseFactor;
        CheckBox chkIsReportIndexSort;

        public TextBox txtGoodRatio;
        public TextBox txtAdjust;

        TextBox txtCenterStandardHeight;
        TextBox txtCenterUpperBound;
        TextBox txtCenterLowerBound;

        TextBox txtXUpperBound;
        TextBox txtYUpperBound;
        TextBox txtFlatness;
        TextBox txtDefinedCode;
        //TextBox txtAddHeight;

        TextBox txtKeyCode;

        Label[] lblinBaseIndicator = new Label[5];
        Label[] lbloutBaseIndicator = new Label[5];
        
        Button btnReget;
        Button btnAutoFindReportIndex;
        Button btnPassLocation;
        Button btnClearPassLocation;
        Button btnExportList;

        Timer AliasTimer;

        //ScreenClass SCREEN;
        

        bool IsNeedToChange = false;

        public KeyAssignForm()
        {
            InitializeComponent();
            Initial();
        }

        void Initial()
        {
            //ResultClass RESULT = Universal.RESULT;
            //SCREEN = new ScreenClass("screen_keyassign", this);
            //SCREEN.SetLanguage();

            btnOK = button6;
            btnCancel = button5;
            btnReget = button1;
            btnAutoFindReportIndex = button2;
            btnPassLocation = button3x;
            btnClearPassLocation = button3y;
            btnExportList = button3z;

            btnOK.Click += new EventHandler(btnOK_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
            btnReget.Click += new EventHandler(btnReget_Click); 
            btnAutoFindReportIndex.Click += new EventHandler(btnAutoFindReportIndex_Click);
            btnPassLocation.Click += new EventHandler(btnPassLocation_Click);
            btnClearPassLocation.Click += new EventHandler(btnClearPassLocation_Click);
            btnExportList.Click += new EventHandler(btnExportList_Click);
            lstKeyassign = listBox1;
            lstKeyassign.Click += new EventHandler(lstKeyassign_Click);
            lstKeyassign.SelectedIndexChanged += new EventHandler(lstKeyassign_SelectedIndexChanged);

            lblKeyassignInformation = label285;

            txtName = textBox9;

            txtAliasName = textBox1;
            txtAliasName.Tag = (int)KeycapParaEnum.ALIASNAME;
            txtStandardHeight = textBox6;
            txtStandardHeight.Tag = (int)KeycapParaEnum.STANDARDHEIGTH;
            txtUpperbound = textBox11;
            txtUpperbound.Tag = (int)KeycapParaEnum.UPPERBOUND;
            txtLowerbound = textBox12;
            txtLowerbound.Tag = (int)KeycapParaEnum.LOWERBOUND;
            txtExamDiff = textBox8;
            txtExamDiff.Tag = (int)KeycapParaEnum.EXAMDIFF;
            txtBesideDiff = textBox10;
            txtBesideDiff.Tag = (int)KeycapParaEnum.BESIDEDIFF;
            txtReportIndex = textBox2;
            txtReportIndex.Tag = (int)KeycapParaEnum.REPORTINDEX;
            txtFlatness = textBox20;
            txtFlatness.Tag = (int)KeycapParaEnum.FLATNESS;
            txtDefinedCode = textBox21;
            txtDefinedCode.Tag = (int)KeycapParaEnum.DEFINEDCODE;
            txtCornerBesideDiff = textBox22;
            txtCornerBesideDiff.Tag = (int)KeycapParaEnum.CORNERBESIDEDIFF;

            txtKeyCode = textBox23;
            txtKeyCode.Tag=(int)KeycapParaEnum.KEYCODE;

            txtD1 = textBox3;
            txtD1.Tag = (int)KeycapParaEnum.D1;
            txtD2 = textBox4;
            txtD2.Tag = (int)KeycapParaEnum.D2;
            txtD3 = textBox5;
            txtD3.Tag = (int)KeycapParaEnum.D3;
            txtD4 = textBox7;
            txtD4.Tag = (int)KeycapParaEnum.D4;

            txtGoodRatio = textBox13;
            //if (RESULT.tempG == true)
            //{
            //    txtGoodRatio.Enabled = true;
            //}
            txtGoodRatio.Tag = (int)KeycapParaEnum.GOODRATIO;

            txtAdjust = textBox14;
            //if (RESULT.tempG == true)
            //{
            //    txtAdjust.Enabled = true;
            //}
            txtAdjust.Tag = (int)KeycapParaEnum.ADJUST;

            txtCenterStandardHeight = textBox17;
            txtCenterStandardHeight.Tag = (int)KeycapParaEnum.CENTERSTANDERHEIGHT;
            txtCenterUpperBound = textBox16;
            txtCenterUpperBound.Tag = (int)KeycapParaEnum.CENTERUPPERBOUND;
            txtCenterLowerBound = textBox15;
            txtCenterLowerBound.Tag = (int)KeycapParaEnum.CENTERLOWERBOUND;

            txtXUpperBound = textBox18;
            txtXUpperBound.Tag = (int)KeycapParaEnum.XUPPERBOUND;
            txtYUpperBound = textBox19;
            txtYUpperBound.Tag = (int)KeycapParaEnum.YUPPERBOUND;
            //txtAddHeight = textBox20;
            //txtAddHeight.Tag = (int)KeycapParaEnum.ADDHEIGHT;

            chkIsNoUseArround = checkBox1;
            chkIsNoUseFactor = checkBox2;
            chkIsReportIndexSort = checkBox4;

            if (!INI.ISUSEDELTA)
            {
                txtD1.Visible = false;
                txtD2.Visible = false;
                txtD3.Visible = false;
                txtD4.Visible = false;
                chkIsNoUseArround.Visible = false;
            }

            chkIsNoUseArround.Click += new EventHandler(chkIsNoUseArround_Click);
            chkIsNoUseFactor.Click += new EventHandler(chkIsNoUseFactor_Click);
            

            txtAliasName.LostFocus += new EventHandler(txt_LostFocus);
            txtAliasName.KeyDown += new KeyEventHandler(txtAliasName_KeyDown);
            txtKeyCode.LostFocus += new EventHandler(txt_LostFocus);
            txtKeyCode.KeyDown += new KeyEventHandler(txtKeyCode_KeyDown);

            txtStandardHeight.LostFocus += new EventHandler(txt_LostFocus);
            txtUpperbound.LostFocus += new EventHandler(txt_LostFocus);
            txtLowerbound.LostFocus += new EventHandler(txt_LostFocus);
            txtExamDiff.LostFocus += new EventHandler(txt_LostFocus);
            txtBesideDiff.LostFocus += new EventHandler(txt_LostFocus);
            txtReportIndex.LostFocus += new EventHandler(txt_LostFocus);

            txtD1.LostFocus += new EventHandler(txt_LostFocus);
            txtD2.LostFocus += new EventHandler(txt_LostFocus);
            txtD3.LostFocus += new EventHandler(txt_LostFocus);
            txtD4.LostFocus += new EventHandler(txt_LostFocus);

            txtCenterStandardHeight.LostFocus += new EventHandler(txt_LostFocus);

            txtCenterUpperBound.LostFocus += new EventHandler(txt_LostFocus);
            txtCenterLowerBound.LostFocus += new EventHandler(txt_LostFocus);
            txtFlatness.LostFocus+=new EventHandler(txt_LostFocus);
            txtDefinedCode.LostFocus += new EventHandler(txt_LostFocus);
            txtCornerBesideDiff.LostFocus += new EventHandler(txt_LostFocus);

            txtXUpperBound.LostFocus += new EventHandler(txt_LostFocus);
            txtYUpperBound.LostFocus += new EventHandler(txt_LostFocus);
            //txtAddHeight.LostFocus += new EventHandler(txt_LostFocus);

            txtGoodRatio.LostFocus += new EventHandler(txt_LostFocus);
            txtAdjust.LostFocus += new EventHandler(txt_LostFocus);


            lblinBaseIndicator[(int)CornerExEnum.LT] = label11;
            lblinBaseIndicator[(int)CornerExEnum.RT] = label8;
            lblinBaseIndicator[(int)CornerExEnum.LB] = label14;
            lblinBaseIndicator[(int)CornerExEnum.RB] = label13;
            lblinBaseIndicator[(int)CornerExEnum.PT1] = label18;

            lbloutBaseIndicator[(int)CornerExEnum.LT] = label4;
            lbloutBaseIndicator[(int)CornerExEnum.RT] = label5;
            lbloutBaseIndicator[(int)CornerExEnum.LB] = label7;
            lbloutBaseIndicator[(int)CornerExEnum.RB] = label6;
            lbloutBaseIndicator[(int)CornerExEnum.PT1] = label23;

            KEYBOARD.ReserveKeyassign();

            OPScreenUIMain = new OPScreenUIKeyAssignClass(opScreenUIControl1, 2, -3, 3);

            OPScreenUIMain.SelectHatAction += new OPScreenUIKeyAssignClass.SelectHatHandler(OPScreenUI_SelectHatAction);
            OPScreenUIMain.PlaceHatAction += new OPScreenUIKeyAssignClass.PlaceHatHandler(OPScreenUI_PlaceHatAction);
            OPScreenUIMain.CopyHatAction += new OPScreenUIKeyAssignClass.CopyHatHandler(OPScreenUI_CopyHatAction);

            OPScreenUIMain.SetBMPDirectly(RECIPEDB.bmpKeyboard);

            OPScreenUISub = new OPScreenUIClass(opScreenUIControl2, 0, -1, 2);

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyAssignForm_KeyDown);
            this.KeyUp += new KeyEventHandler(KeyAssignForm_KeyUp);

            FillDisplay();

            AliasTimer = new Timer();
            AliasTimer.Interval = 100;
            AliasTimer.Tick += new EventHandler(AliasTimer_Tick);
            AliasTimer.Start();
        }

        void btnExportList_Click(object sender, EventArgs e)
        {
            List<string> CheckList = new List<string>();


            foreach (KeyAssignClass keyassign in KEYBOARD.vKEYASSIGNLIST)
            {
                int i = 0;

                string StrPos = "";

                while (i < (int)CornerExEnum.COUNT)
                {
                    if (keyassign.inBaseIndicator[i] != null)
                    {
                        KeybaseClass kbase = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[i].mySide].KEYBASELIST[keyassign.inBaseIndicator[i].Index];

                        StrPos += ",[" + ((CornerExEnum)i).ToString() + "]" + kbase.Name;
                    }
                    i++;
                }

                CheckList.Add(keyassign.ReportIndex.ToString("000") + "," +
                    keyassign.AliasName.Trim() + "," +
                    keyassign.StandardHeight.ToString("0.000") + "," +
                    keyassign.Upperbound.ToString("0.000") + "," +
                    keyassign.Lowerbound.ToString("0.000") + "," +
                    keyassign.ExamDiff.ToString("0.000") + "," +
                    keyassign.BesideDiff.ToString("0.000") + "," +
                    keyassign.CenterHeight.ToString("0.000") + "," +
                    keyassign.CenterUpperBound.ToString("0.000") + "," +
                    keyassign.CenterLowerBound.ToString("0.000") + "," +
                    keyassign.XMaxDiff.ToString("0.000") + "," +
                    keyassign.YMaxDiff.ToString("0.000") + "," +
                    keyassign.Flatness.ToString("0.000") + "," +
                    keyassign.DefinedCode.Trim() + "," + 
                    keyassign.CornerBesideDiff.ToString("0.000") +","+
                    keyassign.KeyCode.Trim()+
                    StrPos);


            }

            CheckList.Sort();

            string Str = "";

            Str = "Report Index,AliasName,Standard Height,UBound,LBound,Tilt,Adjacent,Center Height,Center UBound,Center LBound,X Max,Y Max,Flatnaess,Defined Code,Corner Beside"+ Environment.NewLine;

            foreach (string Checkstr in CheckList)
            {
                Str += Checkstr + Environment.NewLine;
            }


            JzTools.SaveData(Str, @"D:\LOA\CHECKLIST.CSV");

            MessageBox.Show("輸出資料至 D:\\LOA\\CHECKLIST.CSV ");
        }

        void btnClearPassLocation_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否要清除所有的座標資料?", "Jumbo301", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                foreach (KeyAssignClass keyassign in KEYBOARD.vKEYASSIGNLIST)
                {
                    if (keyassign.IsSelected)
                    {
                        KeybaseClass kbase = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.LT].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.LT].Index];

                        kbase.IsSpaceFlat = false;
                        kbase.FlatIndex = 0;
                        kbase.XPos = 0;
                        kbase.YPos = 0;

                        kbase = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.RT].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.RT].Index];
                        
                        kbase.IsSpaceFlat = false;
                        kbase.FlatIndex = 0;
                        kbase.XPos = 0;
                        kbase.YPos = 0;

                        kbase = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.LB].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.LB].Index];
                        
                        kbase.IsSpaceFlat = false;
                        kbase.FlatIndex = 0;
                        kbase.XPos = 0;
                        kbase.YPos = 0;

                        kbase = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.RB].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.RB].Index];

                        kbase.IsSpaceFlat = false;
                        kbase.FlatIndex = 0;
                        kbase.XPos = 0;
                        kbase.YPos = 0;

                        if (keyassign.inBaseIndicator[(int)CornerExEnum.PT1] != null)
                        {
                            kbase = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.PT1].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.PT1].Index];

                            kbase.IsSpaceFlat = false;
                            kbase.FlatIndex = 0;
                            kbase.XPos = 0;
                            kbase.YPos = 0;
                        }
                    }
                }

            }
        }

        //XYFrom XYFRM;

        void btnPassLocation_Click(object sender, EventArgs e)
        {
            //XYFRM = new XYFrom();

            //if (XYFRM.ShowDialog() == DialogResult.OK)
            {

                string loaction = "1;1;1";
                string[] strs = loaction.Split(';');// Universal.GlobalPassLocation.Split(';');

                foreach (KeyAssignClass keyassign in KEYBOARD.vKEYASSIGNLIST)
                {
                    if (keyassign.IsSelected)
                    {
                        KeybaseClass kbase = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.LT].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.LT].Index];

                        kbase.IsSpaceFlat = true;
                        kbase.FlatIndex = 1;
                        kbase.XPos = double.Parse(strs[1].Split(',')[0]);
                        kbase.YPos = double.Parse(strs[1].Split(',')[1]);

                        kbase = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.RT].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.RT].Index];

                        kbase.IsSpaceFlat = true;
                        kbase.FlatIndex = 2;
                        kbase.XPos = double.Parse(strs[2].Split(',')[0]);
                        kbase.YPos = double.Parse(strs[2].Split(',')[1]);

                        kbase = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.LB].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.LB].Index];

                        kbase.IsSpaceFlat = true;
                        kbase.FlatIndex = 3;
                        kbase.XPos = double.Parse(strs[3].Split(',')[0]);
                        kbase.YPos = double.Parse(strs[3].Split(',')[1]);

                        kbase = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.RB].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.RB].Index];

                        kbase.IsSpaceFlat = true;
                        kbase.FlatIndex = 4;
                        kbase.XPos = double.Parse(strs[4].Split(',')[0]);
                        kbase.YPos = double.Parse(strs[4].Split(',')[1]);

                        if (keyassign.inBaseIndicator[(int)CornerExEnum.PT1] != null)
                        {
                            kbase = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.PT1].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.PT1].Index];

                            kbase.IsSpaceFlat = true;
                            kbase.FlatIndex = 5;
                            kbase.XPos = double.Parse(strs[5].Split(',')[0]);
                            kbase.YPos = double.Parse(strs[5].Split(',')[1]);
                        }
                        if (keyassign.inBaseIndicator[(int)CornerExEnum.PT2] != null)
                        {
                            kbase = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.PT2].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.PT2].Index];

                            kbase.IsSpaceFlat = true;
                            kbase.FlatIndex = 6;
                            kbase.XPos = double.Parse(strs[6].Split(',')[0]);
                            kbase.YPos = double.Parse(strs[6].Split(',')[1]);
                        }
                        if (keyassign.inBaseIndicator[(int)CornerExEnum.PT3] != null)
                        {
                            kbase = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.PT3].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.PT3].Index];

                            kbase.IsSpaceFlat = true;
                            kbase.FlatIndex = 7;
                            kbase.XPos = double.Parse(strs[7].Split(',')[0]);
                            kbase.YPos = double.Parse(strs[7].Split(',')[1]);
                        }
                        if (keyassign.inBaseIndicator[(int)CornerExEnum.PT4] != null)
                        {
                            kbase = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.PT4].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.PT4].Index];

                            kbase.IsSpaceFlat = true;
                            kbase.FlatIndex = 8;
                            kbase.XPos = double.Parse(strs[8].Split(',')[0]);
                            kbase.YPos = double.Parse(strs[8].Split(',')[1]);
                        }
                        if (keyassign.inBaseIndicator[(int)CornerExEnum.PT5] != null)
                        {
                            kbase = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.PT5].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.PT5].Index];

                            kbase.IsSpaceFlat = true;
                            kbase.FlatIndex = 9;
                            kbase.XPos = double.Parse(strs[9].Split(',')[0]);
                            kbase.YPos = double.Parse(strs[9].Split(',')[1]);
                        }
                    }
                }

            }

            //XYFRM.Dispose();
            
        }

        void chkIsNoUseFactor_Click(object sender, EventArgs e)
        {
            foreach (KeyAssignClass keyassign in KEYBOARD.vKEYASSIGNLIST)
            {
                if (keyassign.IsSelected)
                    keyassign.IsNoUseFactor = chkIsNoUseFactor.Checked;
            }
        }

        void chkIsNoUseArround_Click(object sender, EventArgs e)
        {
            foreach (KeyAssignClass keyassign in KEYBOARD.vKEYASSIGNLIST)
            {
                if (keyassign.IsSelected)
                    keyassign.IsNoUseArround = chkIsNoUseArround.Checked;
            }
        }

        void AliasTimer_Tick(object sender, EventArgs e)
        {
            if (IsAliasFocus)
            {
                txtAliasName.Focus();
                txtAliasName.SelectAll();

                IsAliasFocus = false;
            }
        }

        bool IsAliasFocus = false;

        void txtAliasName_KeyDown(object sender, KeyEventArgs e)
        {
            int i = lstKeyassign.SelectedIndex;

            switch (e.KeyCode)
            {
                case Keys.Down:
                    txt_LostFocus(sender, e);
                    if (lstKeyassign.SelectedIndex < lstKeyassign.Items.Count - 1)
                    {
                        IsNeedToChange = false;
                        lstKeyassign.SetSelected(i, false);
                        IsNeedToChange = true;

                        lstKeyassign.SetSelected(i + 1, true);
                        //lstKeyassign.Refresh();
                        //txtAliasName.Focus();
                        //Application.DoEvents();
                        IsAliasFocus = true;
                        
                    }
                    break;
                case Keys.Up:
                    txt_LostFocus(sender, e);
                    if (lstKeyassign.SelectedIndex > 0)
                    {
                        IsNeedToChange = false;
                        lstKeyassign.SetSelected(i, false);
                        IsNeedToChange = true;

                        lstKeyassign.SetSelected(i - 1, true);
                        //lstKeyassign.Refresh();
                        IsAliasFocus = true;
                    }
                    break;
                case Keys.ControlKey:
                    txtAliasName.SelectAll();
                    break;
            }
        }
        void txtKeyCode_KeyDown(object sender, KeyEventArgs e)
        {
            int i = lstKeyassign.SelectedIndex;

            switch (e.KeyCode)
            {
                case Keys.Down:
                    txt_LostFocus(sender, e);
                    if (lstKeyassign.SelectedIndex < lstKeyassign.Items.Count - 1)
                    {
                        IsNeedToChange = false;
                        lstKeyassign.SetSelected(i, false);
                        IsNeedToChange = true;

                        lstKeyassign.SetSelected(i + 1, true);
                        //lstKeyassign.Refresh();
                        //txtAliasName.Focus();
                        //Application.DoEvents();
                        IsAliasFocus = true;

                    }
                    break;
                case Keys.Up:
                    txt_LostFocus(sender, e);
                    if (lstKeyassign.SelectedIndex > 0)
                    {
                        IsNeedToChange = false;
                        lstKeyassign.SetSelected(i, false);
                        IsNeedToChange = true;

                        lstKeyassign.SetSelected(i - 1, true);
                        //lstKeyassign.Refresh();
                        IsAliasFocus = true;
                    }
                    break;
                case Keys.ControlKey:
                    txtKeyCode.SelectAll();
                    break;
            }
        }

        void lstKeyassign_Click(object sender, EventArgs e)
        {
            txtAliasName.Focus();
            txtAliasName.SelectAll();
        }

        void txt_LostFocus(object sender, EventArgs e)
        {
            TextBox GetTXT = (TextBox)sender;

            KeycapParaEnum Para = (KeycapParaEnum)((int)GetTXT.Tag);

            switch ((KeycapParaEnum)GetTXT.Tag)
            {
                case KeycapParaEnum.REPORTINDEX:
                    if (!JzTools.CheckTextBoxIsInteger(GetTXT, 1000, 0))
                    {
                        GetTXT.Text = "1";
                        Application.DoEvents();

                        GetTXT.Focus();
                        return;
                    }
                    break;
                case KeycapParaEnum.STANDARDHEIGTH:
                case KeycapParaEnum.UPPERBOUND:
                case KeycapParaEnum.LOWERBOUND:
                case KeycapParaEnum.EXAMDIFF:
                case KeycapParaEnum.BESIDEDIFF:
                case KeycapParaEnum.CORNERBESIDEDIFF:
                case KeycapParaEnum.GOODRATIO:
                case KeycapParaEnum.CENTERSTANDERHEIGHT:
                case KeycapParaEnum.CENTERUPPERBOUND:
                case KeycapParaEnum.CENTERLOWERBOUND:
                case KeycapParaEnum.XUPPERBOUND:
                case KeycapParaEnum.YUPPERBOUND:

                    if (!JzTools.CheckTextBoxIsDouble(GetTXT, 10, 0.01))
                    {
                        GetTXT.Text = "1.0000";
                        Application.DoEvents();

                        GetTXT.Focus();
                        return;
                    }
                    break;

                case KeycapParaEnum.D1:
                case KeycapParaEnum.D2:
                case KeycapParaEnum.D3:
                case KeycapParaEnum.D4:
                case KeycapParaEnum.ADJUST:
                case KeycapParaEnum.ADDHEIGHT:
                case KeycapParaEnum.FLATNESS:
                    if (!JzTools.CheckTextBoxIsDouble(GetTXT, 10, -10))
                    {
                        GetTXT.Text = "1.0000";
                        Application.DoEvents();

                        GetTXT.Focus();
                        return;
                    }
                    break;
            }

            foreach (KeyAssignClass keyassign in KEYBOARD.vKEYASSIGNLIST)
            {
                switch (Para)
                {
                    case KeycapParaEnum.ALIASNAME:
                        if (keyassign.IsSelectedStart)
                        {
                            keyassign.AliasName = GetTXT.Text;
                        }
                        break;
                    case KeycapParaEnum.KEYCODE:
                        if (keyassign.IsSelectedStart)
                        {
                            keyassign.KeyCode = GetTXT.Text;
                        }
                        break;
                    case KeycapParaEnum.REPORTINDEX:
                        if (keyassign.IsSelectedStart)
                        {
                            keyassign.ReportIndex = int.Parse(GetTXT.Text);
                            GetTXT.Text = keyassign.ReportIndex.ToString("0000");
                        }
                        break;
                    case KeycapParaEnum.STANDARDHEIGTH:
                        if (keyassign.IsSelected)
                        {
                            keyassign.StandardHeight = double.Parse(GetTXT.Text);
                            GetTXT.Text = keyassign.StandardHeight.ToString("0.000000");
                        }
                        break;
                    case KeycapParaEnum.CENTERSTANDERHEIGHT:
                        if (keyassign.IsSelected)
                        {
                            keyassign.CenterStandardHeight = double.Parse(GetTXT.Text);
                            GetTXT.Text = keyassign.CenterStandardHeight.ToString("0.000000");
                        }
                        break;
                    case KeycapParaEnum.GOODRATIO:
                        if (keyassign.IsSelected)
                        {
                            keyassign.GoodRatio = double.Parse(GetTXT.Text);
                            GetTXT.Text = keyassign.GoodRatio.ToString("0.00");
                        }
                        break;
                    case KeycapParaEnum.UPPERBOUND:
                        if (keyassign.IsSelected)
                        {
                            keyassign.Upperbound = double.Parse(GetTXT.Text);
                            GetTXT.Text = keyassign.Upperbound.ToString("0.000000");
                        }
                        break;
                    case KeycapParaEnum.LOWERBOUND:
                        if (keyassign.IsSelected)
                        {
                            keyassign.Lowerbound = double.Parse(GetTXT.Text);
                            GetTXT.Text = keyassign.Lowerbound.ToString("0.000000");
                        }
                        break;
                    case KeycapParaEnum.CENTERUPPERBOUND:
                        if (keyassign.IsSelected)
                        {
                            keyassign.CenterUpperBound = double.Parse(GetTXT.Text);
                            GetTXT.Text = keyassign.CenterUpperBound.ToString("0.000000");
                        }
                        break;
                    case KeycapParaEnum.CENTERLOWERBOUND:
                        if (keyassign.IsSelected)
                        {
                            keyassign.CenterLowerBound = double.Parse(GetTXT.Text);
                            GetTXT.Text = keyassign.CenterLowerBound.ToString("0.000000");
                        }
                        break;
                    case KeycapParaEnum.EXAMDIFF:
                        if (keyassign.IsSelected)
                        {
                            keyassign.ExamDiff = double.Parse(GetTXT.Text);
                            GetTXT.Text = keyassign.ExamDiff.ToString("0.000000");
                        }
                        break;
                    case KeycapParaEnum.BESIDEDIFF:
                        if (keyassign.IsSelected)
                        {
                            keyassign.BesideDiff = double.Parse(GetTXT.Text);
                            GetTXT.Text = keyassign.BesideDiff.ToString("0.000000");
                        }
                        break;
                    case KeycapParaEnum.D1:
                        if (keyassign.IsSelected)
                        {
                            keyassign.D1 = double.Parse(GetTXT.Text);
                            GetTXT.Text = keyassign.D1.ToString("0.000000");
                        }
                        break;
                    case KeycapParaEnum.D2:
                        if (keyassign.IsSelected)
                        {
                            keyassign.D2 = double.Parse(GetTXT.Text);
                            GetTXT.Text = keyassign.D2.ToString("0.000000");
                        }
                        break;
                    case KeycapParaEnum.D3:
                        if (keyassign.IsSelected)
                        {
                            keyassign.D3 = double.Parse(GetTXT.Text);
                            GetTXT.Text = keyassign.D3.ToString("0.000000");
                        }
                        break;
                    case KeycapParaEnum.D4:
                        if (keyassign.IsSelected)
                        {
                            keyassign.D4 = double.Parse(GetTXT.Text);
                            GetTXT.Text = keyassign.D4.ToString("0.000000");
                        }
                        break;
                    case KeycapParaEnum.XUPPERBOUND:
                        if (keyassign.IsSelected)
                        {
                            keyassign.XUpperBound = double.Parse(GetTXT.Text);
                            GetTXT.Text = keyassign.XUpperBound.ToString("0.000000");
                        }
                        break;
                    case KeycapParaEnum.YUPPERBOUND:
                        if (keyassign.IsSelected)
                        {
                            keyassign.YUpperBound = double.Parse(GetTXT.Text);
                            GetTXT.Text = keyassign.YUpperBound.ToString("0.000000");
                        }
                        break;
                    case KeycapParaEnum.FLATNESS:
                        if (keyassign.IsSelected)
                        {
                            keyassign.Flatness = double.Parse(GetTXT.Text);
                            GetTXT.Text = keyassign.Flatness.ToString("0.00");
                        }
                        break;
                    case KeycapParaEnum.ADJUST:
                        if (keyassign.IsSelected)
                        {
                            keyassign.Adjust = double.Parse(GetTXT.Text);
                            GetTXT.Text = keyassign.Adjust.ToString("0.00");
                        }
                        break;
                    case KeycapParaEnum.DEFINEDCODE:
                        if (keyassign.IsSelected)
                        {
                            keyassign.DefinedCode = GetTXT.Text.Trim();
                            GetTXT.Text = keyassign.DefinedCode;
                        }
                        break;
                    case KeycapParaEnum.CORNERBESIDEDIFF:
                        if (keyassign.IsSelected)
                        {
                            keyassign.CornerBesideDiff = double.Parse(GetTXT.Text);
                            GetTXT.Text = keyassign.CornerBesideDiff.ToString("0.00");
                        }
                        break;
                }
            }
        }

        void lstKeyassign_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            if (lstKeyassign.Items.Count > 0)
            {
                SetDisplay(lstKeyassign.SelectedIndex);

                int i = 0;

                while (i < lstKeyassign.Items.Count)
                {
                    KEYBOARD.vKEYASSIGNLIST[i].IsSelectedStart = false;
                    KEYBOARD.vKEYASSIGNLIST[i].IsSelected = lstKeyassign.GetSelected(i);
                    i++;
                }

                KEYBOARD.vKEYASSIGNLIST[lstKeyassign.SelectedIndex].IsSelectedStart = true;

                OPScreenUIMain.Refresh();
            }
        }

        void KeyAssignForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyValue)
            {
                case 118:   //ADD
                    KEYBOARD.AddKeyassign(OPScreenUIMain.bmpBareOrigion);
                    OPScreenUIMain.Refresh();
                    FillDisplay();
                    break;
                case 119:   //DELETE 
                    KEYBOARD.DeleteKeyassign();
                    OPScreenUIMain.Refresh();
                    FillDisplay();
                    break;
                case 17:
                    OPScreenUIMain.IsWaitForSelection = true;
                    break;
                case 18:
                    OPScreenUIMain.IsWaitForCopy = true;
                    break;
            }
        }
        void KeyAssignForm_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyValue)
            {
                case 17:
                    OPScreenUIMain.IsWaitForSelection = false;
                    break;
                case 18:
                    OPScreenUIMain.IsWaitForCopy = false;
                    break;
            }
        }
        void OPScreenUI_PlaceHatAction()
        {
            FillDisplay(false);
        }
        void OPScreenUI_SelectHatAction()
        {
            FillDisplay(false);
        }
        void OPScreenUI_CopyHatAction()
        {
            FillDisplay(true);
        }

        void FillDisplay(bool IsNeedToRefreshList)
        {
            IsNeedToChange = false;

            int i = 0, j = 0;
            bool IsKeyassignSelected = false;

            if (IsNeedToRefreshList)
            {
                lstKeyassign.Items.Clear();

                foreach (KeyAssignClass keyassign in KEYBOARD.vKEYASSIGNLIST)
                {
                    lstKeyassign.Items.Add(keyassign.Name);
                    lstKeyassign.SetSelected(lstKeyassign.Items.Count - 1, keyassign.IsSelected);

                    if (keyassign.IsSelectedStart)
                        i = lstKeyassign.Items.Count - 1;

                    IsKeyassignSelected |= keyassign.IsSelected;
                }
            }
            else
            {
                j = 0;
                foreach (KeyAssignClass keyassign in KEYBOARD.vKEYASSIGNLIST)
                {
                    lstKeyassign.SetSelected(j, keyassign.IsSelected);
                    if (keyassign.IsSelectedStart)
                        i = j;

                    IsKeyassignSelected |= keyassign.IsSelected;

                    j++;
                }

            }

            if (lstKeyassign.Items.Count > 0 && IsKeyassignSelected)
            {
                SetDisplay(i);
            }
            else
                SetDisplay(lstKeyassign.SelectedIndex);

            IsNeedToChange = true;

        }
        void FillDisplay()
        {
            FillDisplay(true);
        }

        void SetDisplay(int Index)
        {
            bool Enabled = Index != -1;

            txtName.Enabled = Enabled;
            txtAliasName.Enabled = Enabled;
            txtKeyCode.Enabled = Enabled;
            
            txtStandardHeight.Enabled = Enabled;
            txtUpperbound.Enabled = Enabled;
            txtLowerbound.Enabled = Enabled;
            txtExamDiff.Enabled = Enabled;
            txtBesideDiff.Enabled = Enabled;
            txtCornerBesideDiff.Enabled = Enabled;
            txtReportIndex.Enabled = Enabled;

            txtD1.Enabled = Enabled;
            txtD2.Enabled = Enabled;
            txtD3.Enabled = Enabled;
            txtD4.Enabled = Enabled;
            //txtGoodRatio.Enabled = Enabled;
            //txtAdjust.Enabled = Enabled;

            txtCenterStandardHeight.Enabled = Enabled;
            txtCenterUpperBound.Enabled = Enabled;
            txtCenterLowerBound.Enabled = Enabled;

            txtXUpperBound.Enabled = Enabled;
            txtYUpperBound.Enabled = Enabled;
            txtFlatness.Enabled = Enabled;
            txtDefinedCode.Enabled = Enabled;
            //txtAddHeight.Enabled = Enabled;

            chkIsNoUseArround.Enabled = Enabled;
            chkIsNoUseFactor.Enabled = Enabled;

            if (Index == -1)
            {
                //ResultClass RESULT = Universal.RESULT;
                txtName.Text = "";
                txtKeyCode.Text = "";
                lblKeyassignInformation.Text = "";

                txtAliasName.Text = "";

                txtStandardHeight.Text = "0.000000";
                txtUpperbound.Text = "0.000000";
                txtLowerbound.Text = "0.000000";
                txtExamDiff.Text = "0.000000";
                txtBesideDiff.Text = "0.000000";
                txtCornerBesideDiff.Text = "0.000000";
                txtReportIndex.Text = "000000";

                txtD1.Text = "0.000000";
                txtD2.Text = "0.000000";
                txtD3.Text = "0.000000";
                txtD4.Text = "0.000000";
                txtGoodRatio.Text = "0.00";
                txtAdjust.Text = "0.00";

                txtCenterStandardHeight.Text = "0.000000";
                txtCenterUpperBound.Text = "0.000000";
                txtCenterLowerBound.Text = "0.000000";

                txtXUpperBound.Text = "0.000000";
                txtYUpperBound.Text = "0.000000";
                txtFlatness.Text = "0.00";
                txtDefinedCode.Text = "";

                chkIsNoUseArround.Checked = false;
                chkIsNoUseFactor.Checked = false;

                foreach (Label lbl in lblinBaseIndicator)
                {
                    lbl.Text = "";
                }
                foreach (Label lbl in lbloutBaseIndicator)
                {
                    lbl.Text = "";
                }
                OPScreenUISub.SetBMPDirectly(null);
            }
            else
            {
                KeyAssignClass keyassign = KEYBOARD.vKEYASSIGNLIST[Index];
                //ResultClass RESULT = Universal.RESULT;

                txtName.Text = keyassign.Name;
                lblKeyassignInformation.Text = JzTools.RecttoString(keyassign.myrect);

                txtAliasName.Text = keyassign.AliasName;
                txtKeyCode.Text = keyassign.KeyCode;
                txtStandardHeight.Text = keyassign.StandardHeight.ToString("0.000000");
                txtUpperbound.Text = keyassign.Upperbound.ToString("0.000000");
                txtLowerbound.Text = keyassign.Lowerbound.ToString("0.000000");

                txtExamDiff.Text = keyassign.ExamDiff.ToString("0.000000");
                txtBesideDiff.Text = keyassign.BesideDiff.ToString("0.000000");
                txtCornerBesideDiff.Text = keyassign.CornerBesideDiff.ToString("0.000000");

                txtD1.Text = keyassign.D1.ToString("0.000000");
                txtD2.Text = keyassign.D2.ToString("0.000000");
                txtD3.Text = keyassign.D3.ToString("0.000000");
                txtD4.Text = keyassign.D4.ToString("0.000000");

                txtCenterStandardHeight.Text = keyassign.CenterStandardHeight.ToString("0.000000");
                txtCenterUpperBound.Text = keyassign.CenterUpperBound.ToString("0.000000");
                txtCenterLowerBound.Text = keyassign.CenterLowerBound.ToString("0.000000");

                txtXUpperBound.Text = keyassign.XUpperBound.ToString("0.000000");
                txtYUpperBound.Text = keyassign.YUpperBound.ToString("0.000000");
                //txtAddHeight.Text = keyassign.AddHeight.ToString("0.000000");


                txtGoodRatio.Text = keyassign.GoodRatio.ToString("0.00");
                txtAdjust.Text = keyassign.Adjust.ToString("0.00");
                txtFlatness.Text = keyassign.Flatness.ToString("0.00");
                txtDefinedCode.Text = keyassign.DefinedCode;

                chkIsNoUseArround.Checked = keyassign.IsNoUseArround;
                chkIsNoUseFactor.Checked = keyassign.IsNoUseFactor;

                txtReportIndex.Text = keyassign.ReportIndex.ToString("0000");

                lblinBaseIndicator[(int)CornerExEnum.PT1].Text = "";
                lbloutBaseIndicator[(int)CornerExEnum.PT1].Text = "";

                int i = 0;

                while (i < (int)CornerExEnum.COUNT)
                {
                    if (i < 5)
                    {
                        lblinBaseIndicator[i].Text = "";
                        if (keyassign.inBaseIndicator[i] != null)
                        {
                            lblinBaseIndicator[i].Text = keyassign.inBaseIndicator[i].ToString();
                        }
                        lbloutBaseIndicator[i].Text = "";
                        if (keyassign.outBaseIndicator[i] != null)
                        {
                            lbloutBaseIndicator[i].Text = keyassign.outBaseIndicator[i].ToString();
                        }
                    }
                    else
                    {
                        if (keyassign.inBaseIndicator[i] != null)
                        {
                            lblinBaseIndicator[(int)CornerExEnum.PT1].Text += keyassign.inBaseIndicator[i].ToString() + Environment.NewLine;
                        }
                        if (keyassign.outBaseIndicator[i] != null)
                        {
                            lbloutBaseIndicator[(int)CornerExEnum.PT1].Text += keyassign.outBaseIndicator[i].ToString() + Environment.NewLine;
                        }
                    }
                    i++;
                }
                
                OPScreenUISub.SetBMPDirectly(null);


                OPScreenUISub.SetBMPDirectly(keyassign.bmpOrigion);
            }
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            KEYBOARD.ClearevKeyassign();

            OPScreenUIMain.Dispose();
            OPScreenUISub.Dispose();

            this.DialogResult = DialogResult.Cancel;
        }
        void btnOK_Click(object sender, EventArgs e)
        {
            KEYBOARD.WriteBackKeyassign();

            RECIPEDB.bmpKeyboard.Dispose();
            RECIPEDB.bmpKeyboard = (Bitmap)OPScreenUIMain.bmpBareOrigion.Clone();

            OPScreenUIMain.Dispose();
            OPScreenUISub.Dispose();

            this.DialogResult = DialogResult.OK;

        }
        void btnReget_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "BMP Files (*.bmp)|*.BMP|" + "All files (*.*)|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Bitmap bmp = new Bitmap(dlg.FileName);
                OPScreenUIMain.SetBMPDirectly(bmp);

                bmp.Dispose();
                foreach (KeyAssignClass keyassign in KEYBOARD.vKEYASSIGNLIST)
                {
                    keyassign.GetBMP(OPScreenUIMain.bmpBareOrigion);
                }

                FillDisplay();
            }
        }
        void btnAutoFindReportIndex_Click(object sender, EventArgs e)
        {
            //if (MessageBox.Show(SCREEN.Messages("msg1"), "MAIN", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int Highest = 100000;
                int HighestIndex = -1;
                int ReportIndex = 0;
                List<string> CheckList = new List<string>();

                int i = 0;

                //Clear All Index To 0 and Check the Highest

                foreach (KeyAssignClass keyassign in KEYBOARD.vKEYASSIGNLIST)
                {
                    keyassign.ReportIndex = 0;
                    if (chkIsReportIndexSort.Checked)
                        ReportIndex++;
                    else
                        ReportIndex = 1;
                }

                i = 0;
                while (true)
                {
                    i = 0;
                    Highest = 100000;
                    HighestIndex = -1;
                    foreach (KeyAssignClass keyassign in KEYBOARD.vKEYASSIGNLIST)
                    {
                        if (keyassign.ReportIndex == 0)
                        {
                            if (keyassign.myrect.Y < Highest)
                            {
                                Highest = keyassign.myrect.Y;
                                HighestIndex = i;
                            }
                        }

                        i++;
                    }

                    if (HighestIndex == -1)
                        break;

                    CheckList.Clear();

                    //把相同位置的人找出來
                    i = 0;
                    foreach (KeyAssignClass keyassign in KEYBOARD.vKEYASSIGNLIST)
                    {
                        if (keyassign.ReportIndex == 0)
                        {
                            if (JzTools.IsInRange(keyassign.myrect.Y, Highest, 30))
                            {
                                CheckList.Add(keyassign.myrect.X.ToString("0000") + "," + i.ToString());
                            }
                        }
                        i++;
                    }

                    CheckList.Sort();

                    foreach (string Str in CheckList)
                    {
                        string[] Strs = Str.Split(',');

                        KEYBOARD.vKEYASSIGNLIST[int.Parse(Strs[1])].ReportIndex = ReportIndex;
                        if (chkIsReportIndexSort.Checked)
                            ReportIndex--;
                        else
                            ReportIndex++;
                    }
                }


                OPScreenUIMain.Refresh();
                FillDisplay();
            }
        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }
    }
}