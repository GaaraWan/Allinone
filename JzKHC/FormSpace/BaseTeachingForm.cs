using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using JetEazy.BasicSpace;
//using Jumbo301.UniversalSpace;
using JzKHC.ControlSpace;
using JzKHC.AOISpace;
//using Jumbo301.DBSpace;
using JetEazy;
using JetEazy.ControlSpace;

namespace JzKHC.FormSpace
{
    public partial class BaseTeachingForm : Form
    {
        const char Separator = '\xad';

        enum KeycapParaEnum : int
        {
            CONTRAST = 0,
            ISCALIBRATION =1,
            ISFROMBASE =2,
            ISASPLANE=3,
            ISAUTOLOCATION=4,
            ISSPACEFLAT=5,
            FLATINDEX=6,
            ADDHEIGHT =7,

            YMIN = 8,
            RANGE = 9,
            RESOLUTIONRANGE = 10,
        }

        SideEnum mySide;

        Button btnOK;
        Button btnCancel;

        OPScreenUIKeyBaseClass OPScreenUIMain;
        OPScreenUIClass OPScreenUISub;
        protected JzToolsClass JzTools = new JzToolsClass();
        KeyboardClass KEYBOARD
        {
            get
            {
                return Universal.KEYBOARD;
            }
        }
        SideClass SIDE
        {
            get
            {
                return KEYBOARD.SIDES[(int)mySide];
            }
        }

        Bitmap bmpwork = new Bitmap(1, 1);

        CCDCollectionClass CCDCollection;
        CCDOfflineClass CCD
        {
            get
            {
                return Universal.CCD;
            }
        }

        //MainClass MAIN
        //{
        //    get
        //    {
        //        return Universal.MAIN;
        //    }
        //}

        ListBox lstKeybase;

        TextBox txtName;
        Label lblKeybaseInformation;

        NumericUpDown numContrast;
        NumericUpDown numFlatIndex;

        NumericUpDown numYmin;
        NumericUpDown numRange;
        NumericUpDown numResolutionRange;


        Label lblResolution;
        CheckBox chkIsCalibration;
        CheckBox chkIsFromBase;
       
        //Label lblResolution;
        Button btnAdd;
        Button btnDel;

        Button btnClearAllDefined;
        Button btnAutoFind;
        Button btnAnalyseResolution;

        Button btnExportData;

        ComboBox cboKeyassign;
        ComboBox cboKeyposition;

        TextBox txtXPos;
        TextBox txtYPos;
        TextBox txtAddHeight;

        CheckBox chkIsAsPlane;
        CheckBox chkIsAutoLocation;
        CheckBox chkIsSpaceFlat;

        ListBox lstCornerDefined;

        Button btnReget;
        //ScreenClass SCREEN;

        List<string> keynamelist = new List<string>();

        Timer MyTimer;

        bool IsNeedToChange = false;

        public BaseTeachingForm(SideEnum rSide)
        {
            InitializeComponent();
            Initial(rSide);
        }

        void Initial(SideEnum rSide)
        {
            //SCREEN = new ScreenClass("screen_base", this);
            //SCREEN.SetLanguage();

            mySide = rSide;

            btnOK = button6;
            btnCancel = button5;
            btnReget = button1;

            btnAutoFind = button4;
            btnAnalyseResolution = button8;
            btnClearAllDefined = button7;

            btnOK.Click += new EventHandler(btnOK_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
            btnReget.Click += new EventHandler(btnReget_Click);
            btnAutoFind.Click += new EventHandler(btnAutoFind_Click);
            btnAnalyseResolution.Click += new EventHandler(btnAnalyseResolution_Click);

            chkIsCalibration = checkBox3;
            chkIsCalibration.Tag = KeycapParaEnum.ISCALIBRATION;
            chkIsFromBase = checkBox1;
            chkIsFromBase.Tag = KeycapParaEnum.ISFROMBASE;
            chkIsAsPlane = checkBox4;
            chkIsAsPlane.Tag = KeycapParaEnum.ISASPLANE;
            chkIsAutoLocation = checkBox5;
            chkIsAutoLocation.Tag = KeycapParaEnum.ISAUTOLOCATION;
            chkIsSpaceFlat = checkBox6;
            chkIsSpaceFlat.Tag = KeycapParaEnum.ISSPACEFLAT;

            chkIsCalibration.CheckedChanged += new EventHandler(chk_CheckedChanged);
            chkIsFromBase.CheckedChanged += new EventHandler(chk_CheckedChanged);
            chkIsAsPlane.CheckedChanged += new EventHandler(chk_CheckedChanged);
            chkIsAutoLocation.CheckedChanged += new EventHandler(chk_CheckedChanged);
            chkIsSpaceFlat.CheckedChanged += new EventHandler(chk_CheckedChanged);

            lblResolution = label2;

            lstKeybase = listBox1;
            lstKeybase.SelectedIndexChanged += new EventHandler(lstKeybase_SelectedIndexChanged);

            lblKeybaseInformation = label285;

            txtName = textBox9;

            numContrast = numericUpDown1;
            numFlatIndex = numericUpDown2;

            //lblResolution = label2;

            btnAdd = button2;
            btnDel = button3;

            btnExportData = button9;
            btnExportData.Click += new EventHandler(btnExportData_Click);
            
            cboKeyassign = comboBox1;
            cboKeyposition = comboBox2;
            lstCornerDefined = listBox2;

            if (!INI.ISUSEPLANE && !INI.ISSPACEFLAT)
                lstCornerDefined.Height = 186;


            if (INI.ISSPACEFLAT)
            {
                chkIsAsPlane.Visible = false;
                chkIsAutoLocation.Visible = false;
            }

            txtXPos = textBox1;
            txtYPos = textBox2;

            txtXPos.LostFocus += new EventHandler(txtXPos_LostFocus);
            txtYPos.LostFocus += new EventHandler(txtYPos_LostFocus);

            txtAddHeight = textBox3;
            txtAddHeight.LostFocus += new EventHandler(txtAddHeight_LostFocus);

            btnAdd.Click += new EventHandler(btnAdd_Click);
            btnDel.Click += new EventHandler(btnDel_Click);


            btnClearAllDefined.Click += new EventHandler(btnClearAllDefined_Click);

            foreach (KeyAssignClass keyassign in KEYBOARD.KEYASSIGNLIST)
            {
                keynamelist.Add(keyassign.AliasName + Separator + keyassign.Name);
                //cboKeycap.Items.Add(keycap.AliasName);
            }
            keynamelist.Sort();

            foreach (string kname in keynamelist)
            {
                cboKeyassign.Items.Add(kname.Split(Separator)[0]);
            }

            if (cboKeyassign.Items.Count > 0)
                cboKeyassign.SelectedIndex = 0;

            int i = 0;
            while (i < (int)CornerExEnum.COUNT)
            {
                cboKeyposition.Items.Add(JzTools.CornerEXToStr((CornerExEnum)i));
                i++;
            }

            //cboKeyposition.Items.Add(SCREEN.Messages("msg1"));
            //cboKeyposition.Items.Add(SCREEN.Messages("msg2"));
            //cboKeyposition.Items.Add(SCREEN.Messages("msg3"));

            cboKeyposition.Items.Add("四週");
            cboKeyposition.Items.Add("左側");
            cboKeyposition.Items.Add("右側");

            numContrast.Tag = (int)KeycapParaEnum.CONTRAST;
            numContrast.ValueChanged += new EventHandler(num_ValueChanged);

            numFlatIndex.Tag = (int)KeycapParaEnum.FLATINDEX;
            numFlatIndex.ValueChanged += new EventHandler(num_ValueChanged);

            numYmin = numericUpDown4;
            numRange = numericUpDown5;
            numResolutionRange = numericUpDown3;


            numYmin.Tag = (int)KeycapParaEnum.YMIN;
            numYmin.ValueChanged += new EventHandler(num_ValueChanged);

            numRange.Tag = (int)KeycapParaEnum.RANGE;
            numRange.ValueChanged += new EventHandler(num_ValueChanged);

            numResolutionRange.Tag = (int)KeycapParaEnum.RESOLUTIONRANGE;
            numResolutionRange.ValueChanged += new EventHandler(num_ValueChanged);

            SIDE.ReserveKeybase();

            OPScreenUIMain = new OPScreenUIKeyBaseClass(opScreenUIControl1, -5, -8, 2);
            //OPScreenUIMain = new OPScreenUIKeyBaseClass(opScreenUIControl1, -1, -2, 2);
            OPScreenUIMain.mySide = mySide;

            OPScreenUIMain.rdoNormal = radioButton1;
            OPScreenUIMain.rdoAnalyze = radioButton3;
            OPScreenUIMain.rdoExam = radioButton2;
            OPScreenUIMain.chkIsNoLine = checkBox2;

            OPScreenUIMain.rdoNormal.Text = OPScreenUIMain.rdoNormal.Text.Replace("10", (10 + INI.BASEHEIGHT).ToString());
            OPScreenUIMain.rdoAnalyze.Text = OPScreenUIMain.rdoAnalyze.Text.Replace("12", (12 + INI.BASEHEIGHT).ToString());


            OPScreenUIMain.chkIsNoLine.Click += new EventHandler(chkIsNoLine_Click);
            OPScreenUIMain.SelectHatAction += new OPScreenUIKeyBaseClass.SelectHatHandler(OPScreenUI_SelectHatAction);
            OPScreenUIMain.PlaceHatAction += new OPScreenUIKeyBaseClass.PlaceHatHandler(OPScreenUI_PlaceHatAction);
            OPScreenUIMain.CopyHatAction += new OPScreenUIKeyBaseClass.CopyHatHandler(OPScreenUI_CopyHatAction);
            OPScreenUIMain.rdoNormal.Click += new EventHandler(rdo_Click);
            OPScreenUIMain.rdoAnalyze.Click += new EventHandler(rdo_Click);
            OPScreenUIMain.rdoExam.Click += new EventHandler(rdo_Click);

            OPScreenUIMain.SetBMPDirectly(SIDE.bmpBaseOrigin);

            OPScreenUISub = new OPScreenUIClass(opScreenUIControl2, -1, -2, 2);

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(BaseTeachingForm_KeyDown);
            this.KeyUp += new KeyEventHandler(BaseTeachingForm_KeyUp);

            cboKeyposition.SelectedIndex = 0;


            SIDE.IsReget[(int)TeachingTypeEnum.BASE] = SIDE.IsRegetAlready[(int)TeachingTypeEnum.BASE];
            SIDE.IsReget[(int)TeachingTypeEnum.ANALYZE] = SIDE.IsRegetAlready[(int)TeachingTypeEnum.ANALYZE];
            SIDE.IsReget[(int)TeachingTypeEnum.BACKGROUD] = SIDE.IsRegetAlready[(int)TeachingTypeEnum.BACKGROUD];

            MyTimer = new Timer();
            MyTimer.Interval = 100;
            MyTimer.Tick += new EventHandler(MyTimer_Tick);
            MyTimer.Stop();


            FillDisplay();
        }

        void btnExportData_Click(object sender, EventArgs e)
        {
            string Str = "";
            List<string> BaseStringList = new List<string>();

            foreach (KeybaseClass keybase in SIDE.vKEYBASELIST)
            {
                if (keybase.IsSelected)
                {
                    BaseStringList.Add(keybase.rectFoundBias.Y.ToString("00000") + "," + keybase.rectFoundBias.X.ToString("00000") + "," + keybase.Resolution.ToString());
                }
            }

            BaseStringList.Sort();

            foreach (string str in BaseStringList)
            {
                Str += str + Environment.NewLine;
            }

            JzTools.SaveData(Str, @"D:\SUPER.TXT");
        }

        void txtAddHeight_LostFocus(object sender, EventArgs e)
        {
            double dtmp = 0;

            foreach (KeybaseClass keybase in SIDE.vKEYBASELIST)
            {
                if (keybase.IsSelected)
                {
                    if (double.TryParse(txtAddHeight.Text, out dtmp))
                    {
                        keybase.AddHeight = dtmp;
                        txtAddHeight.Text = dtmp.ToString("0.0000");
                    }
                    else
                    {
                        txtAddHeight.Text = "0.0000";
                    }
                }
            }
        }

        void MyTimer_Tick(object sender, EventArgs e)
        {
            //if (CCD.GetCount == 1)//Gaara by mask
            //{
            //    MyTimer.Stop();
            //    RegetSub();
            //}
        }

        void txtYPos_LostFocus(object sender, EventArgs e)
        {
            double dtmp = 0;

            foreach (KeybaseClass keybase in SIDE.vKEYBASELIST)
            {
                if (keybase.IsSelected)
                {
                    if (double.TryParse(txtYPos.Text, out dtmp))
                    {
                        keybase.YPos = dtmp;
                        txtYPos.Text = dtmp.ToString("0.0000");
                    }
                    else
                    {
                        txtYPos.Text = "0.0000";
                    }
                }

            }
        }

        void txtXPos_LostFocus(object sender, EventArgs e)
        {
            double dtmp =0;

            foreach (KeybaseClass keybase in SIDE.vKEYBASELIST)
            {
                if (keybase.IsSelected)
                {
                    if (double.TryParse(txtXPos.Text, out dtmp))
                    {
                        keybase.XPos = dtmp;
                        txtXPos.Text = dtmp.ToString("0.0000");
                    }
                    else
                    {
                        txtXPos.Text = "0.0000";
                    }
                }

            }
        }

        void chkIsNoLine_Click(object sender, EventArgs e)
        {
            OPScreenUIMain.Refresh();
        }

        void chk_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            CheckBox GetCHK = (CheckBox)sender;
            foreach (KeybaseClass keybase in SIDE.vKEYBASELIST)
            {
                switch ((KeycapParaEnum)GetCHK.Tag)
                {
                    case KeycapParaEnum.ISCALIBRATION:
                        if (keybase.IsSelected)
                        {
                            keybase.IsCalibration = GetCHK.Checked;
                            keybase.Resolution = 0.01;
                            keybase.IsFromBase = false;

                            keybase.CornerDefinedList.Clear();
                        }
                        break;
                    case KeycapParaEnum.ISFROMBASE:
                        if (keybase.IsSelected)
                        {
                            keybase.IsFromBase = GetCHK.Checked;
                        }
                        break;
                    case KeycapParaEnum.ISASPLANE:
                        if (keybase.IsSelected)
                        {
                            keybase.IsAsPlane = GetCHK.Checked;
                        }
                        break;
                    case KeycapParaEnum.ISAUTOLOCATION:
                        if (keybase.IsSelected)
                        {
                            keybase.IsAutoLocation = GetCHK.Checked;
                        }
                        break;
                    case KeycapParaEnum.ISSPACEFLAT:
                        if (keybase.IsSelected)
                        {
                            keybase.IsSpaceFlat = GetCHK.Checked;
                        }
                        break;
                }
            }

            FillDisplay();

            OPScreenUIMain.Refresh();
        }

        void btnAnalyseResolution_Click(object sender, EventArgs e)
        {
            //if (MessageBox.Show(SCREEN.Messages("msg8"), "MAIN", MessageBoxButtons.YesNo) == DialogResult.Yes)
                if (MessageBox.Show("是否要尋找所有的解析度?", "MAIN", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                foreach (KeybaseClass keybase in SIDE.vKEYBASELIST)
                {

                    if (INI.IS_CHECK_LEVEL)
                        keybase.CheckResolutionAllinone(SIDE.bmpAnalyzeOrigin);
                    else
                        keybase.CheckResolution(SIDE.bmpAnalyzeOrigin);
                    //keybase.CheckResolution(SIDE.bmpAnalyzeOrigin);
                }

                OPScreenUIMain.Refresh();
                FillDisplay();
            }
        }
        void btnClearAllDefined_Click(object sender, EventArgs e)
        {
            bool IsSomethingCleared = false;

            foreach (KeybaseClass keybase in SIDE.vKEYBASELIST)
            {
                if (keybase.IsSelected)
                {
                    keybase.CornerDefinedList.Clear();
                    keybase.IsSpaceFlat = false;
                    keybase.FlatIndex = 0;
                    keybase.XPos = 0;
                    keybase.YPos = 0;
                    keybase.AddHeight = 0;
                    IsSomethingCleared = true;
                }
            }

            if (IsSomethingCleared)
                lstCornerDefined.Items.Clear();

            OPScreenUIMain.Refresh();
        }

        void btnDel_Click(object sender, EventArgs e)
        {
            if (lstKeybase.SelectedIndex == -1)
                return;

            int i = lstCornerDefined.Items.Count - 1;

            int StartIndex = 0;
            foreach (KeybaseClass keybasex in SIDE.vKEYBASELIST)
            {
                if (keybasex.IsSelectedStart)
                {
                    break;
                }
                StartIndex++;
            }

            KeybaseClass keybase = SIDE.vKEYBASELIST[StartIndex];

            while (i > -1)
            {
                if (lstCornerDefined.GetSelected(i))
                {
                    keybase.CornerDefinedList.RemoveAt(i);
                    lstCornerDefined.Items.RemoveAt(i);
                }
                i--;
            }

            OPScreenUIMain.Refresh();
        }
        void btnAdd_Click(object sender, EventArgs e)
        {
            int i = 0;
            int j = 0;
            int BaseIndex = 0;
            int MiddleAdd = 0;

            if (lstKeybase.SelectedIndex == -1)
                return;

            i = 0;
            j = 0;

            foreach (KeybaseClass keybasx in SIDE.vKEYBASELIST)
            {
                if (keybasx.IsFromBase && keybasx.IsSelected)
                {
                    i++;
                    BaseIndex = j;
                }

                if (!keybasx.IsFromBase && keybasx.IsSelected)
                {
                    MiddleAdd++;
                }

                j++;
            }

            if (i > 1)
            {
                MessageBox.Show("選擇基底數目過多，請重新選擇", "MAIN", MessageBoxButtons.OK);
                //MessageBox.Show(SCREEN.Messages("msg7"), "MAIN", MessageBoxButtons.OK);
                return;
            }
            else if (i == 1)
            {
                #region Assign Base Indicators

                KeybaseClass RealKeybase = SIDE.vKEYBASELIST[BaseIndex];

                foreach (KeybaseClass keybasex in SIDE.vKEYBASELIST)
                {
                    if (!keybasex.IsCalibration && !keybasex.IsFromBase && keybasex.IsSelected)
                    {
                        foreach (CornerDefineClass cdf in keybasex.CornerDefinedList)
                        {
                            RealKeybase.CornerDefinedList.Add(new CornerDefineClass(cdf.ToString()));
                        }
                    }
                }

                OPScreenUIMain.Refresh();

                #endregion
            }
            else if (MiddleAdd == 5)
            {
                #region Select Keycap Indicators

                {
                    string UsedCornerStr = "";
                    string OriginCornerStr = "";

                    List<KeybaseClass> KeybaseCorners = new List<KeybaseClass>();

                    int SelectedStart = 0;
                    List<int> SelectList = new List<int>();
                    i = 0;
                    foreach (KeybaseClass keybase in SIDE.vKEYBASELIST)
                    {
                        if (keybase.IsSelected)
                        {
                            SelectList.Add(i);

                            if (keybase.IsSelectedStart)
                                SelectedStart = i;
                        }
                        i++;
                    }

                    {
                        int SortingMax = 0;
                        int SortingSecond = 0;
                        int RealMax = 0;
                        int RealSecond = 0;

                        KeybaseCorners.Clear();
                        KeybaseCorners.Add(SIDE.vKEYBASELIST[SelectList[0]]);
                        KeybaseCorners.Add(SIDE.vKEYBASELIST[SelectList[1]]);
                        KeybaseCorners.Add(SIDE.vKEYBASELIST[SelectList[2]]);
                        KeybaseCorners.Add(SIDE.vKEYBASELIST[SelectList[3]]);
                        KeybaseCorners.Add(SIDE.vKEYBASELIST[SelectList[4]]);

                        OriginCornerStr = SelectList[0].ToString("0000") + ",";
                        OriginCornerStr += SelectList[1].ToString("0000") + ",";
                        OriginCornerStr += SelectList[2].ToString("0000") + ",";
                        OriginCornerStr += SelectList[3].ToString("0000") + ",";
                        OriginCornerStr += SelectList[4].ToString("0000");


                        //先找出左邊排序的最大兩員
                        if (INI.ISROTATE)
                        {
                            #region Find Left Side
                            i = 0;

                            SortingMax = 0;

                            //找出最左邊的
                            j = 1;
                            while (j < KeybaseCorners.Count)
                            {
                                if (KeybaseCorners[SortingMax].FoundCenterBias.X > KeybaseCorners[j].FoundCenterBias.X)
                                {
                                    SortingMax = j;
                                }
                                j++;
                            }

                            RealMax = SelectList[SortingMax];

                            UsedCornerStr = RealMax.ToString("0000") + ",";
                            //找出次左邊的

                            if (SortingMax == 0)
                            {
                                j = 1;
                                SortingSecond = 1;
                            }
                            else
                            {
                                j = 0;
                                SortingSecond = 0;
                            }

                            while (j < KeybaseCorners.Count)
                            {
                                if (j != SortingMax)
                                {
                                    if (KeybaseCorners[SortingSecond].FoundCenterBias.X > KeybaseCorners[j].FoundCenterBias.X)
                                    {
                                        SortingSecond = j;
                                    }
                                }
                                j++;
                            }

                            RealSecond = SelectList[SortingSecond];

                            UsedCornerStr += RealSecond.ToString("0000") + ",";

                            if (SIDE.vKEYBASELIST[RealMax].FoundCenterBias.Y < SIDE.vKEYBASELIST[RealSecond].FoundCenterBias.Y)
                            {
                                SIDE.vKEYBASELIST[RealMax].CornerDefinedList.Clear();
                                SIDE.vKEYBASELIST[RealSecond].CornerDefinedList.Clear();

                                SIDE.vKEYBASELIST[RealMax].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "右下"));
                                SIDE.vKEYBASELIST[RealSecond].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "右上"));

                                SIDE.vKEYBASELIST[RealMax].IsSpaceFlat = true;
                                SIDE.vKEYBASELIST[RealSecond].IsSpaceFlat = true;

                                SIDE.vKEYBASELIST[RealMax].FlatIndex = 1;
                                SIDE.vKEYBASELIST[RealSecond].FlatIndex = 3;
                            }
                            else
                            {

                                SIDE.vKEYBASELIST[RealMax].CornerDefinedList.Clear();
                                SIDE.vKEYBASELIST[RealSecond].CornerDefinedList.Clear();

                                SIDE.vKEYBASELIST[RealMax].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "右上"));
                                SIDE.vKEYBASELIST[RealSecond].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "右下"));

                                SIDE.vKEYBASELIST[RealMax].IsSpaceFlat = true;
                                SIDE.vKEYBASELIST[RealSecond].IsSpaceFlat = true;

                                SIDE.vKEYBASELIST[RealMax].FlatIndex = 3;
                                SIDE.vKEYBASELIST[RealSecond].FlatIndex = 1;

                            }
                            #endregion

                            #region Find Right Side

                            i = 0;

                            SortingMax = 0;

                            //找出最右邊的
                            j = 1;
                            while (j < KeybaseCorners.Count)
                            {
                                if (KeybaseCorners[SortingMax].FoundCenterBias.X < KeybaseCorners[j].FoundCenterBias.X)
                                {
                                    SortingMax = j;
                                }
                                j++;
                            }

                            RealMax = SelectList[SortingMax];

                            UsedCornerStr += RealMax.ToString("0000") + ",";


                            //找出次右邊的

                            if (SortingMax == 0)
                            {
                                j = 1;
                                SortingSecond = 1;
                            }
                            else
                            {
                                j = 0;
                                SortingSecond = 0;
                            }

                            while (j < KeybaseCorners.Count)
                            {
                                if (j != SortingMax)
                                {
                                    if (KeybaseCorners[SortingSecond].FoundCenterBias.X < KeybaseCorners[j].FoundCenterBias.X)
                                    {
                                        SortingSecond = j;
                                    }
                                }
                                j++;
                            }

                            RealSecond = SelectList[SortingSecond];
                            UsedCornerStr += RealSecond.ToString("0000");

                            if (SIDE.vKEYBASELIST[RealMax].FoundCenterBias.Y < SIDE.vKEYBASELIST[RealSecond].FoundCenterBias.Y)
                            {

                                SIDE.vKEYBASELIST[RealMax].CornerDefinedList.Clear();
                                SIDE.vKEYBASELIST[RealSecond].CornerDefinedList.Clear();

                                SIDE.vKEYBASELIST[RealMax].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "左下"));
                                SIDE.vKEYBASELIST[RealSecond].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "左上"));

                                SIDE.vKEYBASELIST[RealMax].IsSpaceFlat = true;
                                SIDE.vKEYBASELIST[RealSecond].IsSpaceFlat = true;

                                SIDE.vKEYBASELIST[RealMax].FlatIndex = 2;
                                SIDE.vKEYBASELIST[RealSecond].FlatIndex = 4;
                            }
                            else
                            {
                                SIDE.vKEYBASELIST[RealMax].CornerDefinedList.Clear();
                                SIDE.vKEYBASELIST[RealSecond].CornerDefinedList.Clear();

                                SIDE.vKEYBASELIST[RealMax].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "左上"));
                                SIDE.vKEYBASELIST[RealSecond].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "左下"));

                                SIDE.vKEYBASELIST[RealMax].IsSpaceFlat = true;
                                SIDE.vKEYBASELIST[RealSecond].IsSpaceFlat = true;

                                SIDE.vKEYBASELIST[RealMax].FlatIndex = 4;
                                SIDE.vKEYBASELIST[RealSecond].FlatIndex = 2;
                            }

                            #endregion

                        }
                        else
                        {
                            #region Find Left Side
                            i = 0;

                            SortingMax = 0;

                            //找出最左邊的
                            j = 1;
                            while (j < KeybaseCorners.Count)
                            {
                                if (KeybaseCorners[SortingMax].FoundCenterBias.X > KeybaseCorners[j].FoundCenterBias.X)
                                {
                                    SortingMax = j;
                                }
                                j++;
                            }

                            RealMax = SelectList[SortingMax];

                            UsedCornerStr = RealMax.ToString("0000") + ",";
                            //找出次左邊的

                            if (SortingMax == 0)
                            {
                                j = 1;
                                SortingSecond = 1;
                            }
                            else
                            {
                                j = 0;
                                SortingSecond = 0;
                            }

                            while (j < KeybaseCorners.Count)
                            {
                                if (j != SortingMax)
                                {
                                    if (KeybaseCorners[SortingSecond].FoundCenterBias.X > KeybaseCorners[j].FoundCenterBias.X)
                                    {
                                        SortingSecond = j;
                                    }
                                }
                                j++;
                            }

                            RealSecond = SelectList[SortingSecond];

                            UsedCornerStr += RealSecond.ToString("0000") + ",";

                            if (SIDE.vKEYBASELIST[RealMax].FoundCenterBias.Y < SIDE.vKEYBASELIST[RealSecond].FoundCenterBias.Y)
                            {
                                SIDE.vKEYBASELIST[RealMax].CornerDefinedList.Clear();
                                SIDE.vKEYBASELIST[RealSecond].CornerDefinedList.Clear();

                                SIDE.vKEYBASELIST[RealMax].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "左上"));
                                SIDE.vKEYBASELIST[RealSecond].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "左下"));

                                SIDE.vKEYBASELIST[RealMax].IsSpaceFlat = true;
                                SIDE.vKEYBASELIST[RealSecond].IsSpaceFlat = true;

                                SIDE.vKEYBASELIST[RealMax].FlatIndex = 1;
                                SIDE.vKEYBASELIST[RealSecond].FlatIndex = 3;
                            }
                            else
                            {

                                SIDE.vKEYBASELIST[RealMax].CornerDefinedList.Clear();
                                SIDE.vKEYBASELIST[RealSecond].CornerDefinedList.Clear();

                                SIDE.vKEYBASELIST[RealMax].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "左下"));
                                SIDE.vKEYBASELIST[RealSecond].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "左上"));

                                SIDE.vKEYBASELIST[RealMax].IsSpaceFlat = true;
                                SIDE.vKEYBASELIST[RealSecond].IsSpaceFlat = true;

                                SIDE.vKEYBASELIST[RealMax].FlatIndex = 3;
                                SIDE.vKEYBASELIST[RealSecond].FlatIndex = 1;

                            }
                            #endregion

                            #region Find Right Side

                            i = 0;

                            SortingMax = 0;

                            //找出最右邊的
                            j = 1;
                            while (j < KeybaseCorners.Count)
                            {
                                if (KeybaseCorners[SortingMax].FoundCenterBias.X < KeybaseCorners[j].FoundCenterBias.X)
                                {
                                    SortingMax = j;
                                }
                                j++;
                            }

                            RealMax = SelectList[SortingMax];

                            UsedCornerStr += RealMax.ToString("0000") + ",";


                            //找出次右邊的

                            if (SortingMax == 0)
                            {
                                j = 1;
                                SortingSecond = 1;
                            }
                            else
                            {
                                j = 0;
                                SortingSecond = 0;
                            }

                            while (j < KeybaseCorners.Count)
                            {
                                if (j != SortingMax)
                                {
                                    if (KeybaseCorners[SortingSecond].FoundCenterBias.X < KeybaseCorners[j].FoundCenterBias.X)
                                    {
                                        SortingSecond = j;
                                    }
                                }
                                j++;
                            }

                            RealSecond = SelectList[SortingSecond];
                            UsedCornerStr += RealSecond.ToString("0000");

                            if (SIDE.vKEYBASELIST[RealMax].FoundCenterBias.Y < SIDE.vKEYBASELIST[RealSecond].FoundCenterBias.Y)
                            {

                                SIDE.vKEYBASELIST[RealMax].CornerDefinedList.Clear();
                                SIDE.vKEYBASELIST[RealSecond].CornerDefinedList.Clear();

                                SIDE.vKEYBASELIST[RealMax].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "右上"));
                                SIDE.vKEYBASELIST[RealSecond].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "右下"));

                                SIDE.vKEYBASELIST[RealMax].IsSpaceFlat = true;
                                SIDE.vKEYBASELIST[RealSecond].IsSpaceFlat = true;

                                SIDE.vKEYBASELIST[RealMax].FlatIndex = 2;
                                SIDE.vKEYBASELIST[RealSecond].FlatIndex = 4;
                            }
                            else
                            {
                                SIDE.vKEYBASELIST[RealMax].CornerDefinedList.Clear();
                                SIDE.vKEYBASELIST[RealSecond].CornerDefinedList.Clear();

                                SIDE.vKEYBASELIST[RealMax].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "右下"));
                                SIDE.vKEYBASELIST[RealSecond].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "右上"));

                                SIDE.vKEYBASELIST[RealMax].IsSpaceFlat = true;
                                SIDE.vKEYBASELIST[RealSecond].IsSpaceFlat = true;

                                SIDE.vKEYBASELIST[RealMax].FlatIndex = 4;
                                SIDE.vKEYBASELIST[RealSecond].FlatIndex = 2;
                            }

                            #endregion

                        }

                        string[] strs = UsedCornerStr.Split(',');

                        foreach (string str in strs)
                        {
                            OriginCornerStr = OriginCornerStr.Replace(str, "");
                        }

                        OriginCornerStr = OriginCornerStr.Replace(",", "");

                        RealMax = int.Parse(OriginCornerStr);
                        SIDE.vKEYBASELIST[RealMax].CornerDefinedList.Clear();
                        SIDE.vKEYBASELIST[RealMax].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "測點1"));


                        SIDE.vKEYBASELIST[RealMax].IsSpaceFlat = true;

                        SIDE.vKEYBASELIST[RealMax].FlatIndex = 5;                    }


                    lstCornerDefined.Items.Clear();

                    foreach (CornerDefineClass cdf in SIDE.vKEYBASELIST[SelectedStart].CornerDefinedList)
                    {
                        lstCornerDefined.Items.Add(cdf.ToFormedString());
                    }
                }

                #endregion
            }
            else
            {
                #region Select Keycap Indicators With Middle

                if (cboKeyposition.Text != "四週" && 
                    cboKeyposition.Text != "左側" && 
                    cboKeyposition.Text != "右側")

                    //if (cboKeyposition.Text != SCREEN.Messages("msg1") && cboKeyposition.Text != SCREEN.Messages("msg2") && cboKeyposition.Text != SCREEN.Messages("msg3"))
                    {
                    int Indexx = 0;

                    foreach (KeybaseClass keybasex in SIDE.vKEYBASELIST)
                    {
                        if (keybasex.IsSelectedStart)
                        {
                            break;
                        }
                        Indexx++;
                    }

                    KeybaseClass keybase = SIDE.vKEYBASELIST[Indexx];


                    keybase.CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + cboKeyposition.Text));
                    lstCornerDefined.Items.Add(keybase.CornerDefinedList[keybase.CornerDefinedList.Count - 1].ToFormedString());
                }
                else
                {
                    List<KeybaseClass> KeybaseCorners = new List<KeybaseClass>();

                    int SelectedStart = 0;
                    List<int> SelectList = new List<int>();
                    i = 0;
                    foreach (KeybaseClass keybase in SIDE.vKEYBASELIST)
                    {
                        if (keybase.IsSelected)
                        {
                            SelectList.Add(i);

                            if (keybase.IsSelectedStart)
                                SelectedStart = i;
                        }
                        i++;
                    }

                    if (((cboKeyposition.Text == "左側" || 
                        cboKeyposition.Text == "右側") && SelectList.Count != 2) || 
                        (cboKeyposition.Text == "四週" && SelectList.Count != 4))
                        //if (((cboKeyposition.Text == SCREEN.Messages("msg2") ||
                        //cboKeyposition.Text == SCREEN.Messages("msg3")) && SelectList.Count != 2) ||
                        //(cboKeyposition.Text == SCREEN.Messages("msg1") && SelectList.Count != 4))
                        {
                        MessageBox.Show("選擇數目和加入項目不匹配", "MAIN", MessageBoxButtons.OK);
                        return;
                    }

                    if (cboKeyposition.Text == "左側")
                    {
                        if (SIDE.vKEYBASELIST[SelectList[0]].FoundCenterBias.Y < SIDE.vKEYBASELIST[SelectList[1]].FoundCenterBias.Y)
                        {
                            //new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + cboKeyposition.Text)
                            if (INI.ISROTATE)
                            {
                                SIDE.vKEYBASELIST[SelectList[0]].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "左下"));
                                SIDE.vKEYBASELIST[SelectList[1]].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "左上"));
                            }
                            else
                            {
                                SIDE.vKEYBASELIST[SelectList[0]].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "左上"));
                                SIDE.vKEYBASELIST[SelectList[1]].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "左下"));
                            }
                            //lstKeycapCorr.Items.Add(keybase.CorrList[keybase.CorrList.Count - 1]);
                        }
                        else
                        {
                            if (INI.ISROTATE)
                            {
                                SIDE.vKEYBASELIST[SelectList[0]].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "左上"));
                                SIDE.vKEYBASELIST[SelectList[1]].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "左下"));
                            }
                            else
                            {
                                SIDE.vKEYBASELIST[SelectList[0]].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "左下"));
                                SIDE.vKEYBASELIST[SelectList[1]].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "左上"));
                            }
                        }
                    }
                    else if (cboKeyposition.Text == "右側")
                    {
                        if (SIDE.vKEYBASELIST[SelectList[0]].FoundCenterBias.Y < SIDE.vKEYBASELIST[SelectList[1]].FoundCenterBias.Y)
                        {
                            if (INI.ISROTATE)
                            {
                                SIDE.vKEYBASELIST[SelectList[0]].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "右下"));
                                SIDE.vKEYBASELIST[SelectList[1]].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "右上"));
                            }
                            else
                            {
                                SIDE.vKEYBASELIST[SelectList[0]].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "右上"));
                                SIDE.vKEYBASELIST[SelectList[1]].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "右下"));
                            }
                            //lstKeycapCorr.Items.Add(keybase.CorrList[keybase.CorrList.Count - 1]);
                        }
                        else
                        {
                            if (INI.ISROTATE)
                            {
                                SIDE.vKEYBASELIST[SelectList[0]].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "右上"));
                                SIDE.vKEYBASELIST[SelectList[1]].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "右下"));
                            }
                            else
                            {
                                SIDE.vKEYBASELIST[SelectList[0]].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "右下"));
                                SIDE.vKEYBASELIST[SelectList[1]].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "右上"));
                            }
                        }
                    }
                    else if (cboKeyposition.Text == "四週")
                    {
                        int SortingMax = 0;
                        int SortingSecond = 0;
                        int RealMax = 0;
                        int RealSecond = 0;

                        KeybaseCorners.Clear();
                        KeybaseCorners.Add(SIDE.vKEYBASELIST[SelectList[0]]);
                        KeybaseCorners.Add(SIDE.vKEYBASELIST[SelectList[1]]);
                        KeybaseCorners.Add(SIDE.vKEYBASELIST[SelectList[2]]);
                        KeybaseCorners.Add(SIDE.vKEYBASELIST[SelectList[3]]);

                        //先找出左邊排序的最大兩員
                        #region Find Left Side
                        i = 0;

                        SortingMax = 0;

                        //找出最左邊的
                        j = 1;
                        while (j < KeybaseCorners.Count)
                        {
                            if (KeybaseCorners[SortingMax].FoundCenterBias.X > KeybaseCorners[j].FoundCenterBias.X)
                            {
                                SortingMax = j;
                            }
                            j++;
                        }

                        RealMax = SelectList[SortingMax];
                        //找出次左邊的

                        if (SortingMax == 0)
                        {
                            j = 1;
                            SortingSecond = 1;
                        }
                        else
                        {
                            j = 0;
                            SortingSecond = 0;
                        }

                        while (j < KeybaseCorners.Count)
                        {
                            if (j != SortingMax)
                            {
                                if (KeybaseCorners[SortingSecond].FoundCenterBias.X > KeybaseCorners[j].FoundCenterBias.X)
                                {
                                    SortingSecond = j;
                                }
                            }
                            j++;
                        }

                        RealSecond = SelectList[SortingSecond];

                        if (SIDE.vKEYBASELIST[RealMax].FoundCenterBias.Y < SIDE.vKEYBASELIST[RealSecond].FoundCenterBias.Y)
                        {
                            if (INI.ISROTATE)
                            {
                                SIDE.vKEYBASELIST[RealMax].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "右下"));
                                SIDE.vKEYBASELIST[RealSecond].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "右上"));
                            }
                            else
                            {
                                SIDE.vKEYBASELIST[RealMax].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "左上"));
                                SIDE.vKEYBASELIST[RealSecond].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "左下"));
                            }
                        }
                        else
                        {
                            if (INI.ISROTATE)
                            {
                                SIDE.vKEYBASELIST[RealMax].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "右上"));
                                SIDE.vKEYBASELIST[RealSecond].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "右下"));
                            }
                            else
                            {
                                SIDE.vKEYBASELIST[RealMax].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "左下"));
                                SIDE.vKEYBASELIST[RealSecond].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "左上"));
                            }
                        }
                        #endregion

                        #region Find Right Side

                        i = 0;

                        SortingMax = 0;

                        //找出最左邊的
                        j = 1;
                        while (j < KeybaseCorners.Count)
                        {
                            if (KeybaseCorners[SortingMax].FoundCenterBias.X < KeybaseCorners[j].FoundCenterBias.X)
                            {
                                SortingMax = j;
                            }
                            j++;
                        }

                        RealMax = SelectList[SortingMax];
                        //找出次左邊的

                        if (SortingMax == 0)
                        {
                            j = 1;
                            SortingSecond = 1;
                        }
                        else
                        {
                            j = 0;
                            SortingSecond = 0;
                        }

                        while (j < KeybaseCorners.Count)
                        {
                            if (j != SortingMax)
                            {
                                if (KeybaseCorners[SortingSecond].FoundCenterBias.X < KeybaseCorners[j].FoundCenterBias.X)
                                {
                                    SortingSecond = j;
                                }
                            }
                            j++;
                        }

                        RealSecond = SelectList[SortingSecond];

                        if (SIDE.vKEYBASELIST[RealMax].FoundCenterBias.Y < SIDE.vKEYBASELIST[RealSecond].FoundCenterBias.Y)
                        {
                            if (INI.ISROTATE)
                            {
                                SIDE.vKEYBASELIST[RealMax].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "左下"));
                                SIDE.vKEYBASELIST[RealSecond].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "左上"));
                            }
                            else
                            {
                                SIDE.vKEYBASELIST[RealMax].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "右上"));
                                SIDE.vKEYBASELIST[RealSecond].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "右下"));
                            }
                        }
                        else
                        {
                            if (INI.ISROTATE)
                            {
                                SIDE.vKEYBASELIST[RealMax].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "左上"));
                                SIDE.vKEYBASELIST[RealSecond].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "左下"));
                            }
                            else
                            {
                                SIDE.vKEYBASELIST[RealMax].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "右下"));
                                SIDE.vKEYBASELIST[RealSecond].CornerDefinedList.Add(new CornerDefineClass(keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[1] + Separator + keynamelist[cboKeyassign.SelectedIndex].Split(Separator)[0] + Separator + "右上"));
                            }
                        }

                        #endregion
                    }


                    lstCornerDefined.Items.Clear();

                    foreach (CornerDefineClass cdf in SIDE.vKEYBASELIST[SelectedStart].CornerDefinedList)
                    {
                        lstCornerDefined.Items.Add(cdf.ToFormedString());
                    }
                }
                #endregion

            }

            OPScreenUIMain.Refresh();
        }
        void rdo_Click(object sender, EventArgs e)
        {
            OPScreenUIMain.Refresh();
        }

        void num_ValueChanged(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            NumericUpDown GetNum = (NumericUpDown)sender;
            foreach (KeybaseClass keybase in SIDE.vKEYBASELIST)
            {
                switch ((KeycapParaEnum)GetNum.Tag)
                {
                    case KeycapParaEnum.CONTRAST:
                        if (keybase.IsSelected)
                        {
                            keybase.Contrast = (int)numContrast.Value;
                            keybase.GetBMP(OPScreenUIMain.bmpBareOrigion, (double)keybase.Contrast / 20d);

                            if (keybase.IsSelectedStart)
                            {
                                OPScreenUISub.SetBMPDirectly(keybase.bmpProcessed);
                            }
                        }
                        break;
                    case KeycapParaEnum.YMIN:
                        if (keybase.IsSelected)
                        {
                            keybase.Ymin = (int)numYmin.Value;
                        }
                        break;
                    case KeycapParaEnum.RANGE:
                        if (keybase.IsSelected)
                        {
                            keybase.Range = (int)numRange.Value;
                        }
                        break;
                    case KeycapParaEnum.RESOLUTIONRANGE:
                        if (keybase.IsSelected)
                        {
                            keybase.ResolutionRange = (int)numResolutionRange.Value;
                        }
                        break;
                    case KeycapParaEnum.FLATINDEX:
                        if (keybase.IsSelectedStart)
                        {
                            keybase.FlatIndex = (int)numFlatIndex.Value;
                            //keybase.GetBMP(OPScreenUIMain.bmpBareOrigion, (double)keybase.Contrast / 20d);

                            //if (keybase.IsSelectedStart)
                            //{
                            //    OPScreenUISub.SetBMPDirectly(keybase.bmpProcessed);
                            //}
                        }
                        break;
                }
            }
        
            FillDisplay();
            OPScreenUIMain.Refresh();

        }
        void lstKeybase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            if (lstKeybase.Items.Count > 0)
            {
                SetDisplay(lstKeybase.SelectedIndex);

                int i = 0;

                while (i < lstKeybase.Items.Count)
                {
                    SIDE.vKEYBASELIST[i].IsSelectedStart = false;
                    SIDE.vKEYBASELIST[i].IsSelected = lstKeybase.GetSelected(i);
                    i++;
                }

                SIDE.vKEYBASELIST[lstKeybase.SelectedIndex].IsSelectedStart = true;

                OPScreenUIMain.Refresh();
            }

            if (lstKeybase.SelectedIndex != -1)
            {
                lstCornerDefined.Items.Clear();

                int Index = 0;
                foreach (KeybaseClass keybase in SIDE.vKEYBASELIST)
                {
                    if (keybase.IsSelectedStart)
                    {
                        break;
                    }
                    Index++;
                }


                foreach (CornerDefineClass cdf in SIDE.vKEYBASELIST[Index].CornerDefinedList)
                {
                    lstCornerDefined.Items.Add(cdf.ToFormedString());
                }

                if (lstCornerDefined.Items.Count > 0)
                    lstCornerDefined.SelectedIndex = 0;
            }
            else
            {
                lstCornerDefined.Items.Clear();
            }
        }

        void BaseTeachingForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyValue)
            {
                case 118:   //ADD
                    SIDE.AddKeybase(OPScreenUIMain.bmpBareOrigion);
                    OPScreenUIMain.Refresh();
                    FillDisplay();
                    break;
                case 119:   //DELETE 
                    SIDE.DeleteKeybase();
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
        void BaseTeachingForm_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyValue)
            {
                case 17:
                    OPScreenUIMain.IsWaitForSelection = false;
                    break;
                case 18:
                    OPScreenUIMain.IsWaitForCopy = false;
                    //SIDE.SetUndo();
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
            bool IsKeybaseSelected = false;

            if (IsNeedToRefreshList)
            {
                lstKeybase.Items.Clear();

                foreach (KeybaseClass keybase in SIDE.vKEYBASELIST)
                {
                    lstKeybase.Items.Add(keybase.Name);
                    lstKeybase.SetSelected(lstKeybase.Items.Count - 1, keybase.IsSelected);

                    if (keybase.IsSelectedStart)
                        i = lstKeybase.Items.Count - 1;

                    IsKeybaseSelected |= keybase.IsSelected;
                }
            }
            else
            {
                j = 0;
                foreach (KeybaseClass keybase in SIDE.vKEYBASELIST)
                {
                    lstKeybase.SetSelected(j, keybase.IsSelected);

                    if (keybase.IsSelectedStart)
                        i = j;

                    IsKeybaseSelected |= keybase.IsSelected;

                    j++;
                }

            }

            IsNeedToChange = true;

            if (lstKeybase.Items.Count > 0 && IsKeybaseSelected)
            {
                SetDisplay(i);
            }
            else
                SetDisplay(lstKeybase.SelectedIndex);


            //cboKeycorner.SelectedIndex = 0;

            //if (cboKeycap.Items.Count > 0)
            //    cboKeycap.SelectedIndex = 0;

            if (lstKeybase.SelectedIndex != -1)
            {
                lstCornerDefined.Items.Clear();


                int Index = 0;
                foreach (KeybaseClass keybase in SIDE.vKEYBASELIST)
                {
                    if (keybase.IsSelectedStart)
                    {
                        break;
                    }
                    Index++;
                }

                if (Index < SIDE.vKEYBASELIST.Count)
                {
                    foreach (CornerDefineClass cdf in SIDE.vKEYBASELIST[Index].CornerDefinedList)
                    {
                        lstCornerDefined.Items.Add(cdf.ToFormedString());
                    }

                    if (lstCornerDefined.Items.Count > 0)
                        lstCornerDefined.SelectedIndex = 0;
                }

            }
            else
            {
                lstCornerDefined.Items.Clear();
            }
        }

        void FillDisplay()
        {
            FillDisplay(true);

            //IsNeedToChange = false;

            //int i = 0;
            //bool IsKeycapSelected = false;


            //lstKeycap.Items.Clear();

            //foreach (KeycapClass keycap in SIDE.vKEYCAPLIST)
            //{
            //    lstKeycap.Items.Add(keycap.Name);
            //    lstKeycap.SetSelected(lstKeycap.Items.Count - 1, keycap.IsSelected);

            //    if (keycap.IsSelectedStart)
            //        i = lstKeycap.Items.Count - 1;

            //    IsKeycapSelected |= keycap.IsSelected;
            //}

            //IsNeedToChange = true;

            //if (lstKeycap.Items.Count > 0 && IsKeycapSelected)
            //{
            //    SetDisplay(i);
            //}
            //else
            //    SetDisplay(lstKeycap.SelectedIndex);            
        }

        void SetDisplay(int Index)
        {
            bool Enabled = Index != -1;

            txtName.Enabled = Enabled;
            numContrast.Enabled = Enabled;
            chkIsCalibration.Enabled = Enabled;
            chkIsFromBase.Enabled = Enabled;

            btnAdd.Enabled = Enabled;
            btnDel.Enabled = Enabled;

            btnClearAllDefined.Enabled = Enabled;

            cboKeyassign.Enabled = Enabled;
            cboKeyposition.Enabled = Enabled;
            lstCornerDefined.Enabled = Enabled;

            
            chkIsAsPlane.Enabled = Enabled;
            chkIsSpaceFlat.Enabled = Enabled;
            chkIsAutoLocation.Enabled = Enabled;
            numFlatIndex.Enabled = Enabled;

            numYmin.Enabled = Enabled;
            numRange.Enabled = Enabled;
            numResolutionRange.Enabled = Enabled;

            txtXPos.Enabled = Enabled;
            txtYPos.Enabled = Enabled;
            txtAddHeight.Enabled = Enabled;

            IsNeedToChange = false;

            if (Index == -1)
            {
                txtName.Text = "";
                lblKeybaseInformation.Text = "";

                numContrast.Value = numContrast.Minimum;


                numRange.Value = 0;
                numYmin.Value = 0;
                numResolutionRange.Value = 0;

                lblResolution.Text = "0.000000";
                chkIsCalibration.Checked = false;
                chkIsFromBase.Checked = false;

                numFlatIndex.Value = numFlatIndex.Minimum;

                txtXPos.Text = "0.0000";
                txtYPos.Text = "0.0000";
                txtAddHeight.Text = "0.0000";

                //lblResolution.Text = "";
                
                OPScreenUISub.SetBMPDirectly(null);
            }
            else
            {
                KeybaseClass keybase = SIDE.vKEYBASELIST[Index];

                txtName.Text = keybase.Name;
                lblKeybaseInformation.Text = JzTools.RecttoString(keybase.myrect) + "◎" + JzTools.PointtoString(keybase.FoundCenterBias);

                
                numContrast.Value = (int)keybase.Contrast;
                chkIsCalibration.Checked = keybase.IsCalibration;

                lblResolution.Text = keybase.Resolution.ToString("0.000000");
                chkIsFromBase.Checked = keybase.IsFromBase;

                chkIsAsPlane.Checked = keybase.IsAsPlane;
                chkIsAutoLocation.Checked = keybase.IsAutoLocation;
                chkIsSpaceFlat.Checked = keybase.IsSpaceFlat;

                txtXPos.Text = keybase.XPos.ToString("0.0000");
                txtYPos.Text = keybase.YPos.ToString("0.0000");
                txtAddHeight.Text = keybase.AddHeight.ToString("0.0000");

                lblResolution.Enabled = !keybase.IsCalibration;
                chkIsFromBase.Enabled = !keybase.IsCalibration;

                btnAdd.Enabled = !keybase.IsCalibration;
                btnDel.Enabled = !keybase.IsCalibration;
                cboKeyassign.Enabled = !keybase.IsCalibration;
                cboKeyposition.Enabled = !keybase.IsCalibration;

                chkIsAsPlane.Enabled = !keybase.IsCalibration;
                chkIsSpaceFlat.Enabled = !keybase.IsCalibration;
                numFlatIndex.Value = (int)keybase.FlatIndex;
                txtXPos.Enabled = !keybase.IsCalibration;
                txtYPos.Enabled = !keybase.IsCalibration;


                numYmin.Value = keybase.Ymin;
                numRange.Value = keybase.Range;
                numResolutionRange.Value = keybase.ResolutionRange;

                lstCornerDefined.Enabled = Enabled;

                //lblResolution.Text = SIDE.vKEYBASELIST[Index].Resolution.ToString("0.00");

                OPScreenUISub.SetBMPDirectly(SIDE.vKEYBASELIST[Index].bmpProcessed);
            }

            IsNeedToChange = true;
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            SIDE.ClearevKeybase();

            SIDE.bmpBaseOrigin.Dispose();
            //SIDE.bmpBaseOrigin = (Bitmap)OPScreenUIMain.bmpBareOrigion.Clone();
            SIDE.bmpBaseOrigin = new Bitmap(1, 1);

            SIDE.bmpAnalyzeOrigin.Dispose();
            SIDE.bmpAnalyzeOrigin = new Bitmap(1, 1);

            SIDE.bmpBackgroundOrigin.Dispose();
            SIDE.bmpBackgroundOrigin = new Bitmap(1, 1);


            OPScreenUIMain.Dispose();
            OPScreenUISub.Dispose();

            this.DialogResult = DialogResult.Cancel;
        }
        void btnOK_Click(object sender, EventArgs e)
        {
            SIDE.WriteBackKeybase();
            SIDE.ClearevKeybase();


            //SIDE.bmpBaseOrigin.Dispose();
            ////SIDE.bmpBaseOrigin = (Bitmap)OPScreenUIMain.bmpBareOrigion.Clone();
            //SIDE.bmpBaseOrigin = new Bitmap(1, 1);

            SIDE.bmpAnalyzeOrigin.Dispose();
            SIDE.bmpAnalyzeOrigin = new Bitmap(1, 1);

            SIDE.bmpBackgroundOrigin.Dispose();
            SIDE.bmpBackgroundOrigin = new Bitmap(1, 1);


            OPScreenUIMain.Dispose();
            OPScreenUISub.Dispose();

            SIDE.IsRegetAlready[(int)TeachingTypeEnum.BASE] = SIDE.IsReget[(int)TeachingTypeEnum.BASE];
            SIDE.IsRegetAlready[(int)TeachingTypeEnum.ANALYZE] = SIDE.IsReget[(int)TeachingTypeEnum.ANALYZE];
            SIDE.IsRegetAlready[(int)TeachingTypeEnum.BACKGROUD] = SIDE.IsReget[(int)TeachingTypeEnum.BACKGROUD];

            this.DialogResult = DialogResult.OK;

        }

        void RegetSub()
        {
           // CCD.AllinoneBmpFile_Path = Universal.PICPATH + KEYBOARD.ID.ToString("0000");
            if (OPScreenUIMain.IsAnalyze)
            {
                SIDE.IsReget[(int)TeachingTypeEnum.ANALYZE] = true;

                //CCD.Snap(mySide);
                //OPScreenUIMain.SetBMPDirectly(CCDCollection.GetBMP((int)mySide, true));
                //CCD.SaveBMP(Universal.PICPATH + KEYBOARD.ID.ToString("0000") + "\\TA" + ((int)mySide).ToString("000") + ".BMP", mySide);
                //CCD.ClearBMP(mySide);

                CCD.Snap(mySide);
                OPScreenUIMain.SetBMPDirectly(CCD.GetBMP(mySide));
                CCD.SaveBMP(Universal.PICPATH + KEYBOARD.ID.ToString("0000") + "\\TA" + ((int)mySide).ToString("000") + ".BMP", mySide);
                CCD.ClearBMP(mySide);

                SIDE.GetAnalyzeBMP(OPScreenUIMain.bmpBareOrigion);

            }
            else if (OPScreenUIMain.IsExam)
            {
                SIDE.IsReget[(int)TeachingTypeEnum.BACKGROUD] = true;

                CCD.Snap(mySide);
                OPScreenUIMain.SetBMPDirectly(CCD.GetBMP(mySide));
                CCD.SaveBMP(Universal.PICPATH + KEYBOARD.ID.ToString("0000") + "\\TS" + ((int)mySide).ToString("000") + ".BMP", mySide);

                CCD.ClearBMP(mySide);

                SIDE.GetBackgroundBMP(OPScreenUIMain.bmpBareOrigion);
            }
            else
            {
                SIDE.IsReget[(int)TeachingTypeEnum.BASE] = true;

                CCD.Snap(mySide);
                OPScreenUIMain.SetBMPDirectly(CCD.GetBMP(mySide));
                CCD.SaveBMP(Universal.PICPATH + KEYBOARD.ID.ToString("0000") + "\\TB" + ((int)mySide).ToString("000") + ".BMP", mySide);

                CCD.ClearBMP(mySide);

                SIDE.GetBaseBMP(OPScreenUIMain.bmpBareOrigion);

                if (MessageBox.Show("是否要重新自行定位?", "MAIN", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {

                    //開始找
                    Bitmap bmp = (Bitmap)SIDE.bmpBaseOrigin.Clone();
                    List<Rectangle> RectList = new List<Rectangle>();

                    ThresholdClass Threshold = new ThresholdClass();
                    AOISpace.HistogramClass Histogram = new AOISpace.HistogramClass(4);

                    FindObjectClass FindObject = new FindObjectClass();

                    Histogram.GetHistogram(bmp);


                    Threshold.SetThreshold(bmp, JzTools.SimpleRect(bmp.Size), (Histogram.MinGrade < 5 ? 5 : Histogram.MinGrade) << INI.FINDCONTRAST, 255, 10, true);
                    //Threshold.SetThreshold(bmp, JzTools.SimpleRect(bmp.Size), 80, 255, 10, true);

                    FindObject.Find(bmp, Color.Red);

                    foreach (Found found in FindObject.FoundList)
                    {
                        if (found.rect.Width > 10 && found.rect.Height > 2)
                        {
                            Rectangle rect = found.rect;
                            rect.Inflate(20, 20);
                            rect.Intersect(JzTools.SimpleRect(bmp.Size));

                            RectList.Add(rect);

                            //JzTools.DrawRect(bmp, rect, new Pen(Color.Lime));

                            //SIDE.vKEYBASELIST.Add(new KeybaseClass(mySide, SIDE.vKEYBASELIST, SIDE.bmpBaseOrigin, rect));
                        }
                    }
                    //bmp.Save(@"D:\LOA\NEWERA\AUTOFIND.BMP", ImageFormat.Bmp);
                    bmp.Dispose();

                    int MinDistance = 10000;
                    int itmp = 0;
                    Rectangle MinRect = new Rectangle();



                    foreach (KeybaseClass keybase in SIDE.vKEYBASELIST)
                    {
                        //Point PtTmp = keybase.FoundCenterBias;
                        Point PtTmp = keybase.FoundCenterBias;

                        //if (keybase.Name == "BASE-06026")
                        //{
                        //    keybase.Name = keybase.Name;
                        //}

                        MinDistance = 10000;
                        foreach (Rectangle foundrect in RectList)
                        {
                            //if (foundrect.X == 2697)
                            //    keybase.Name = keybase.Name;


                            if (foundrect.IntersectsWith(keybase.myrect))
                            {
                                //itmp = JzTools.GetPointLength(JzTools.GetRectCenter(foundrect), keybase.FoundCenterBias);
                                itmp = JzTools.GetPointLength(JzTools.GetRectCenter(foundrect), PtTmp);
                                if (itmp < MinDistance)
                                {
                                    MinDistance = itmp;
                                    MinRect = foundrect;
                                    keybase.myrect = foundrect;
                                }
                            }
                        }
                        //keybase.myrect = MinRect;
                    }
                }

                foreach (KeybaseClass keybase in SIDE.vKEYBASELIST)
                {
                    keybase.GetBMP(OPScreenUIMain.bmpBareOrigion);
                }
            }

            OPScreenUIMain.Refresh();
            FillDisplay();
        }

        void btnReget_Click(object sender, EventArgs e)
        {
            RegetSub();
        }

        void btnAutoFind_Click(object sender, EventArgs e)
        {

            if(MessageBox.Show("是否要重新尋找所有的探針位置?", "MAIN",MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //清除所有的Keybase

                foreach (KeybaseClass keybase in SIDE.vKEYBASELIST)
                {
                    keybase.Dispoe();
                }

                SIDE.vKEYBASELIST.Clear();

                //開始找
                Bitmap bmp = (Bitmap)SIDE.bmpBaseOrigin.Clone();

                if (INI.CUTPOINT > 0)
                    JzTools.DrawRect(bmp, new Rectangle(0, INI.CUTPOINT, INI.CCDWIDTH, INI.CCDHEIGHT), new SolidBrush(Color.Black));
                

                ThresholdClass Threshold = new ThresholdClass();
                AOISpace.HistogramClass Histogram = new AOISpace.HistogramClass(4);

                FindObjectClass FindObject = new FindObjectClass();

                Histogram.GetHistogram(bmp);

                Threshold.SetThreshold(bmp, JzTools.SimpleRect(bmp.Size), (Histogram.MinGrade < 5 ? 5 : Histogram.MinGrade) << INI.FINDCONTRAST, 255, 10, true);
                //Threshold.SetThreshold(bmp, JzTools.SimpleRect(bmp.Size), 120, 255, 10, true);

                //bmp.Save(@"D:\LOA\NEWERA\AUTOFIND.BMP", ImageFormat.Bmp);

                FindObject.Find(bmp, Color.Red);

                foreach (Found found in FindObject.FoundList)
                {
                    if (found.rect.Width > 10 && found.rect.Height > 2)
                    {
                        Rectangle rect = found.rect;
                        rect.Inflate(20, 20);
                        rect.Intersect(JzTools.SimpleRect(bmp.Size));

                        SIDE.vKEYBASELIST.Add(new KeybaseClass(mySide, SIDE.vKEYBASELIST, SIDE.bmpBaseOrigin, rect));
                    }
                }
                //bmp.Save(@"D:\LOA\NEWERA\AUTOFIND.BMP", ImageFormat.Bmp);
                bmp.Dispose();

                OPScreenUIMain.Refresh();
                FillDisplay();

            }

        }
    }
}