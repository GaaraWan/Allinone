using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Data.OleDb;

using JetEazy.BasicSpace;
using JzKHC.AOISpace;
using JzKHC.FormSpace;
using JzKHC.ControlSpace;
//using Jumbo301.UniversalSpace;
using JetEazy;

namespace JzKHC.DBSpace
{
    class RecipeDBClass : DBClass
    {
        public char IndexSeg = '\xa9';
        public char SectionSeg = '\xa5';

        //LightControlDBClass LIGTHCONTROLDB
        //{
        //    get
        //    {
        //        return Universal.LIGHTCONTROLDB;
        //    }
        //}
        KeyboardClass KEYBOARD
        {
            get
            {
                return Universal.KEYBOARD;
            }
        }

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

        public int ID
        {
            get
            {
                return (int)tbl.Rows[RecordIndex]["rcp00"];
            }
            set
            {
                tbl.Rows[RecordIndex]["rcp00"] = value;
            }
        }
        public string NAME
        {
            get
            {
                return (string)tbl.Rows[RecordIndex]["rcp01"];
            }
            set
            {
                tbl.Rows[RecordIndex]["rcp01"] = value;
            }
        }
        public string CREATEDATETIME
        {
            get
            {
                return (string)tbl.Rows[RecordIndex]["rcp02"];
            }
            set
            {
                tbl.Rows[RecordIndex]["rcp02"] = value;
            }
        }
        public string MODIFYDATETIME
        {
            get
            {
                return (string)tbl.Rows[RecordIndex]["rcp03"];
            }
            set
            {
                tbl.Rows[RecordIndex]["rcp03"] = value;
            }
        }
        public string VERSION
        {
            get
            {
                return (string)tbl.Rows[RecordIndex]["rcp06"];
            }
            set
            {
                tbl.Rows[RecordIndex]["rcp06"] = value;
            }
        }
        public string RECIPEINFORMATION
        {
            get
            {
                return (string)tbl.Rows[RecordIndex]["rcp07"];
            }
            set
            {
                tbl.Rows[RecordIndex]["rcp07"] = value;
            }
        }
        public string LASTCONTROLSTRING
        {
            get
            {
                return (string)tbl.Rows[RecordIndex]["rcp08"];
            }
            set
            {
                tbl.Rows[RecordIndex]["rcp08"] = value;
            }
        }

        //ScreenClass SCREEN
        //{
        //    get
        //    {
        //        return Universal.SCREEN;
        //    }

        //}

        public bool IsCheckHighest = false;
        public double HighestValue = 0;

        public bool IsCheckLowest = false;
        public double LowestValue = 0;
        
        public bool IsCheckSlope = false;
        public double SlopeValue = 0;
        
        public bool IsCheckMutual = false;
        public double MutualValue = 0;

        public bool IsCheckWrongCount = false;
        public int WrongCount = 0;

        public bool IsCriteriaCheck
        {
            get
            {
                return IsCheckHighest || IsCheckLowest || IsCheckSlope || IsCheckMutual || IsCheckWrongCount;
            }
        }

        public Rectangle rectRecipeRange = new Rectangle();

        public Button btnRecipeSelection;
        public Label lblRecipeName;

        //MessageForm MESSAGEFrm;
        //RecipeSelectionForm RECIPESELECTIONFRM;
        JzToolsClass JzTools = new JzToolsClass();

        BaseTeachingForm BASETEACHINGFrm;

        KeyAssignForm KEYASSIGNFrm;

        OleDbCommand cmdtmp = new OleDbCommand();
        DataTable tblTmp = new DataTable();

        Bitmap bmpFindHatBackgroud = new Bitmap(1, 1);
        Bitmap bmpFindHatForegroud = new Bitmap(1, 1);
        public Bitmap bmpKeyboard = new Bitmap(1, 1);

        public RECIPEDBUIControl RECIPEDBUI;

        //ProcedureClass FindHatProcess = new ProcedureClass();

        SubstractClass Substract = new SubstractClass();
        AOISpace.HistogramClass Histogram = new AOISpace.HistogramClass(4);
        ThresholdClass Threshold = new ThresholdClass();
        FindObjectClass FindObject = new FindObjectClass();

        PropertyFilterClass HeightPropertyFilter = new PropertyFilterClass();
        PropertyFilterClass WidthPropertyFilter = new PropertyFilterClass();

        public Button btnClose;
        public Button btnOpen;
        public Button btnHome;

        public Button btnPassageWayLoad;
        public Button btnPassageWayUnLoad;

        public Button btnBigOrSmallKBSelect;
        public Button btnPaasWaySelect;

        //ResultClass RESULT
        //{
        //    get
        //    {
        //        return Universal.RESULT;
        //    }
        //}
        //FBs44mnClass PLC
        //{
        //    get
        //    {
        //        return Universal.PLC;
        //    }
        //}

        bool IsDebug
        {
            get
            {
                return false;// Universal.IsDebug;
            }
        }

        public RecipeDBClass(string rTableString, string rIndexString, string rNameField, string rDBPresentName,OleDbConnection rDatacn)
            : base(rTableString, rIndexString, rNameField, rDBPresentName,rDatacn)
        {
            TableString = rTableString;
            IndexField = rIndexString;
            NameField = rNameField;
            DBPresentName = rDBPresentName;

            Datacn = rDatacn;
        }

        public override void Initial()
        {
            base.Initial();

            if (IsIDExist(INI.LAST_RECIPEID))
                Goto(INI.LAST_RECIPEID);
            else
            {
                Goto(1);
                INI.LAST_RECIPEID = 1;
                INI.Save();
            }

            //lblRecipeName.Text = "[" + ID.ToString() + "] " + NAME + "(" + VERSION + ")";

            Load(false);

            //MyTimer = new Timer();
            //MyTimer.Interval = 100;
            //MyTimer.Tick += new EventHandler(MyTimer_Tick);
            //MyTimer.Stop();

            //switch (Universal.COMPANY_MODE)
            //{
            //    case MODE_DIFFERENT_COMPANY.MODE_Allinone:
            //    case MODE_DIFFERENT_COMPANY.MODE_SUNREX:
            //        btnBigOrSmallKBSelect.Visible = false;
            //        btnPaasWaySelect.Visible = false;
            //        btnPassageWayLoad.Visible = false;
            //        btnPassageWayUnLoad.Visible = false;
            //        break;
            //}


        }

        public void RefreshUI()
        {
            //btnRecipeSelection.Click += new EventHandler(btnRecipeSelection_Click);

            //RECIPEDBUI.btnAdd.Click += new EventHandler(btnAdd_Click);
            RECIPEDBUI.btnModify.Click += new EventHandler(btnModify_Click);
            RECIPEDBUI.btnCopy.Click += new EventHandler(btnCopy_Click);
            RECIPEDBUI.btnOK.Click += new EventHandler(btnOK_Click);
            RECIPEDBUI.btnCancel.Click += new EventHandler(btnCancel_Click);
            //RECIPEDBUI.btnSetup.Click += new EventHandler(btnSetup_Click);

            RECIPEDBUI.btnGetBase.Click += new EventHandler(btnGetBase_Click);
            RECIPEDBUI.btnGetAnalyze.Click += new EventHandler(btnGetAnalyze_Click);
            RECIPEDBUI.btnGetBackgroud.Click += new EventHandler(btnGetBackgroud_Click);
            RECIPEDBUI.btnImport.Click += new EventHandler(btnImport_Click);

            RECIPEDBUI.btnGoBase.Click += new EventHandler(btnGoBase_Click);
            RECIPEDBUI.btnGoAnalyze.Click += new EventHandler(btnGoAnalyze_Click);

            if (!INI.ISUSEPLANE)
                RECIPEDBUI.btnImport.Visible = false;


            RECIPEDBUI.btnGetBase.Text = RECIPEDBUI.btnGetBase.Text.Replace("10", (10 + INI.BASEHEIGHT).ToString());
            RECIPEDBUI.btnGetAnalyze.Text = RECIPEDBUI.btnGetAnalyze.Text.Replace("12", (12 + INI.BASEHEIGHT).ToString());

            //RECIPEDBUI.btnStep1.Click += new EventHandler(btnStep1_Click);
            RECIPEDBUI.btnEDIT.Click += new EventHandler(btnEDIT_Click);

            RECIPEDBUI.btnKeyAssign.Click += new EventHandler(btnKeyAssign_Click);

            //RECIPEDBUI.btnReadData.Click += new EventHandler(btnReadData_Click);
            //RECIPEDBUI.btnExportData.Click += new EventHandler(btnExportData_Click);
            //RECIPEDBUI.btnImportXY.Click += new EventHandler(btnImportXY_Click);

            //btnClose.Click += new EventHandler(btnClose_Click);
            //btnOpen.Click += new EventHandler(btnOpen_Click);
            //btnHome.Click += new EventHandler(btnHome_Click);

            //btnPassageWayLoad.Click += new EventHandler(btnPassageWayLoad_Click);
            //btnPassageWayUnLoad.Click += new EventHandler(btnPassageWayUnLoad_Click);
            //btnBigOrSmallKBSelect.Click += new EventHandler(btnBigOrSmallKBSelect_Click);
            //btnPaasWaySelect.Click += new EventHandler(btnPaasWaySelect_Click);

            //LIGTHCONTROLDB.RecipeChangeAction += new LightControlDBClass.RecipeChangeHandler(LIGTHCONTROLDB_RecipeChangeAction);

            //RECIPEDBUI.LIGHTUI.SetEnable();

            RECIPEDBUI.DBStatus = DBStatusEnum.NONE;
            SetControlComboBox();

            FillDisplay();

        }
        void btnPaasWaySelect_Click(object sender, EventArgs e)
        {
            //PLC.PassWaySelect = !PLC.PassWaySelect;
        }

        void btnBigOrSmallKBSelect_Click(object sender, EventArgs e)
        {
            //PLC.BigOrSmallKBSelect = !PLC.BigOrSmallKBSelect;
        }

        void btnPassageWayUnLoad_Click(object sender, EventArgs e)
        {
            //if (PLC.IsEMC || !PLC.IsSafe)
            //    return;

            //if (!PassageWayUnLoadProcess.IsOn)
            //{
            //    if (MessageBox.Show(SCREEN.Messages("msg1"), "MAIN", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //    {
            //        PassageWayLoadProcess.Stop();

            //        PassageWayUnLoadProcess.Start();
            //    }
            //}
        }

        void btnPassageWayLoad_Click(object sender, EventArgs e)
        {
            //if (PLC.IsEMC || !PLC.IsSafe)
            //    return;

            //if (!PassageWayLoadProcess.IsOn)
            //{
            //    if (MessageBox.Show(SCREEN.Messages("msg2"), "MAIN", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //    {
            //        PassageWayUnLoadProcess.Stop();

            //        PassageWayLoadProcess.Start();
            //    }
            //}
        }

        void btnGoAnalyze_Click(object sender, EventArgs e)
        {
            //PLC.SetZPosition(INI.BASEZLOCATION + INI.DIFFHEIGHT);
            //PLC.GoZ = true;
        }

        void btnGoBase_Click(object sender, EventArgs e)
        {
            //PLC.SetZPosition(INI.BASEZLOCATION);
            //PLC.GoZ = true;
        }

        void btnHome_Click(object sender, EventArgs e)
        {
            //PLC.Magnetic = false;
            //PLC.SetZPosition(0);
            //PLC.GoZ = true;
        }

        //ImportXYForm IMPORTXYFRM;

        void btnImportXY_Click(object sender, EventArgs e)
        {
            /*
            IMPORTXYFRM = new ImportXYForm();

            string Str = "";
            string[] Strs;
            string[] strs;
            string[] strSub = new string[2];

            int SideIndex = 0;
            int inIndex = 0;

            try
            {

                JzTools.ReadData(ref Str, @"D:\XYLOCATION.TXT");

                Str = Str.Replace(Environment.NewLine, "@");
                Strs = Str.Split('@');


                if (IMPORTXYFRM.ShowDialog() == DialogResult.OK)
                {
                    switch (Universal.GlobalPassInteger)
                    {
                        case 0: //K21USUK

                            #region K21USUK Data
                            foreach (string str in Strs)
                            {
                                if (str.Substring(0, 5) == "K21US")
                                {
                                    strs = str.Split('=');
                                    strSub = strs[1].Split(';');

                                    break;
                                }
                            }

                            foreach (KeyAssignClass keyassign in KEYBOARD.KEYASSIGNLIST)
                            {
                                if (keyassign.AliasName == "SPACE")
                                {
                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LT].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LT].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[0].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[0].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RT].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RT].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[1].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[1].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LB].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LB].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[2].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[2].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RB].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RB].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[3].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[3].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerExEnum.PT1].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerExEnum.PT1].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[4].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[4].Split(',')[1]);

                                }
                                if (keyassign.AliasName == "L-SHIFT")
                                {
                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LT].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LT].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[5].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[5].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RT].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RT].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[6].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[6].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LB].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LB].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[7].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[7].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RB].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RB].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[8].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[8].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerExEnum.PT1].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerExEnum.PT1].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[9].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[9].Split(',')[1]);

                                }
                                if (keyassign.AliasName == "R-SHIFT")
                                {
                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LT].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LT].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[10].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[10].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RT].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RT].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[11].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[11].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LB].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LB].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[12].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[12].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RB].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RB].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[13].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[13].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerExEnum.PT1].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerExEnum.PT1].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[14].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[14].Split(',')[1]);

                                }
                            }

                            #endregion

                            break;
                        case 1: //K78USUK

                            #region K78USUK Data
                            foreach (string str in Strs)
                            {
                                if (str.Substring(0, 5) == "K78US")
                                {
                                    strs = str.Split('=');
                                    strSub = strs[1].Split(';');

                                    break;
                                }
                            }

                            foreach (KeyAssignClass keyassign in KEYBOARD.KEYASSIGNLIST)
                            {
                                if (keyassign.AliasName == "SPACE")
                                {
                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LT].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LT].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[0].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[0].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RT].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RT].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[1].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[1].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LB].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LB].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[2].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[2].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RB].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RB].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[3].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[3].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerExEnum.PT1].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerExEnum.PT1].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[4].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[4].Split(',')[1]);

                                }
                                if (keyassign.AliasName == "L-SHIFT")
                                {
                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LT].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LT].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[5].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[5].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RT].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RT].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[6].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[6].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LB].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LB].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[7].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[7].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RB].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RB].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[8].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[8].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerExEnum.PT1].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerExEnum.PT1].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[9].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[9].Split(',')[1]);

                                }
                                if (keyassign.AliasName == "R-SHIFT")
                                {
                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LT].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LT].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[10].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[10].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RT].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RT].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[11].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[11].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LB].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LB].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[12].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[12].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RB].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RB].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[13].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[13].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerExEnum.PT1].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerExEnum.PT1].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[14].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[14].Split(',')[1]);

                                }
                            }

                            #endregion

                            break;
                        case 2: //K21JP

                            #region K21JP Data
                            foreach (string str in Strs)
                            {
                                if (str.Substring(0, 5) == "K21JP")
                                {
                                    strs = str.Split('=');
                                    strSub = strs[1].Split(';');

                                    break;
                                }
                            }

                            foreach (KeyAssignClass keyassign in KEYBOARD.KEYASSIGNLIST)
                            {
                                if (keyassign.AliasName == "SPACE")
                                {
                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LT].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LT].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[0].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[0].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RT].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RT].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[1].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[1].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LB].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LB].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[2].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[2].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RB].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RB].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[3].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[3].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerExEnum.PT1].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerExEnum.PT1].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[4].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[4].Split(',')[1]);

                                }
                            }

                            #endregion
                            break;

                        case 3: //K78JP

                            #region K78JP Data
                            foreach (string str in Strs)
                            {
                                if (str.Substring(0, 5) == "K78JP")
                                {
                                    strs = str.Split('=');
                                    strSub = strs[1].Split(';');

                                    break;
                                }
                            }

                            foreach (KeyAssignClass keyassign in KEYBOARD.KEYASSIGNLIST)
                            {
                                if (keyassign.AliasName == "SPACE")
                                {
                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LT].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LT].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[0].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[0].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RT].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RT].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[1].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[1].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LB].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.LB].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[2].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[2].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RB].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerEnum.RB].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[3].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[3].Split(',')[1]);

                                    SideIndex = (int)keyassign.inBaseIndicator[(int)CornerExEnum.PT1].mySide;
                                    inIndex = (int)keyassign.inBaseIndicator[(int)CornerExEnum.PT1].Index;

                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].XPos = double.Parse(strSub[4].Split(',')[0]);
                                    KEYBOARD.SIDES[SideIndex].KEYBASELIST[inIndex].YPos = double.Parse(strSub[4].Split(',')[1]);

                                }
                            }

                            #endregion
                            break;
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("尚未定義中心點，請確認定義無誤!");
            }

            IMPORTXYFRM.Dispose();
            */
        }

        bool IsCaptureTick = false;

        void CaptureTick()
        {
            if (!IsCaptureTick)
                return;

            //if (CCD.GetCount == (int)SideEnum.COUNT)
            {
                IsCaptureTick = false;

                if (IsAnalyze)
                    GetAnalyzeSub();
                else
                    GetBackgroundSub();

                //MessageBox.Show("GETALL!");
            }
        }

        void btnImport_Click(object sender, EventArgs e)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int Yi = 0;
            int LineIndex = 0;

            string Str = "";
            string PointString = "";
            string[] PtSr;
            StreamReader sr;
            List<string> ImportList = new List<string>();
            List<string> YSortingList = new List<string>();
            List<string> XSortingList = new List<string>();

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Text Files (*.txt)|*.TXT|" + "All files (*.*)|*.*";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                sr = new StreamReader(dlg.FileName);

                while (!sr.EndOfStream)
                {
                    ImportList.Add(sr.ReadLine());
                }


                #region Get None Base Indicator Data
                i = 0;
                foreach (SideClass side in KEYBOARD.SIDES)
                {
                    j = 0;
                    YSortingList.Clear();

                    foreach (KeybaseClass keybase in side.KEYBASELIST)
                    {
                        if (keybase.IsAutoLocation && !keybase.IsFromBase)
                        {
                            keybase.XPos = 0;
                            keybase.YPos = 0;

                            YSortingList.Add(keybase.rectFoundBias.Y.ToString("00000") + "," + keybase.rectFoundBias.X.ToString("00000") + "@" + j.ToString("000"));
                        }

                        j++;
                    }

                    YSortingList.Sort();

                    double YLocation = double.Parse(YSortingList[0].Split(',')[0]);

                    LineIndex = 0;
                    k = 0;
                    while (LineIndex < 12)
                    {
                        XSortingList.Clear();
                        Yi = 0;
                        k = 0;
                        while(k < YSortingList.Count)
                        {
                            if (YSortingList[k] == "")
                            {
                                k++;
                                continue;
                            }

                            if (Math.Abs(YLocation - double.Parse(YSortingList[k].Split(',')[0])) < 50d)
                            {
                                XSortingList.Add(YSortingList[k].Split(',')[1]);
                                YSortingList[k] = "";
                            }
                            else
                            {
                                YLocation = double.Parse(YSortingList[k].Split(',')[0]);
                                break;
                            }
                            k++;
                            //Yi++;
                        }

                        XSortingList.Sort();

                        PointString = FindLineAndCamera(ImportList, LineIndex, i);

                        PtSr = PointString.Split('@');

                        if (PtSr.Length == XSortingList.Count)
                        {
                            k = 0;
                            foreach (string XStr in XSortingList)
                            {
                                KEYBOARD.SIDES[i].KEYBASELIST[int.Parse(XStr.Split('@')[1])].XPos = double.Parse(PtSr[k].Split(',')[0]);
                                KEYBOARD.SIDES[i].KEYBASELIST[int.Parse(XStr.Split('@')[1])].YPos = double.Parse(PtSr[k].Split(',')[1]);

                                k++;
                            }
                        }
                        LineIndex++;
                    }
                    i++;
                }
                #endregion

                //return;

                #region Get Base indicator Data

                i = 0;
                foreach (SideClass side in KEYBOARD.SIDES)
                {
                    j = 0;
                    YSortingList.Clear();

                    foreach (KeybaseClass keybase in side.KEYBASELIST)
                    {
                        if (keybase.IsAutoLocation && keybase.IsFromBase)
                        {
                            keybase.XPos = 0;
                            keybase.YPos = 0;

                            YSortingList.Add(keybase.rectFoundBias.Y.ToString("00000") + "," + keybase.rectFoundBias.X.ToString("00000") + "@" + j.ToString("000"));
                        }

                        j++;
                    }

                    YSortingList.Sort();

                    double YLocation = double.Parse(YSortingList[0].Split(',')[0]);

                    LineIndex = 0;
                    k = 0;
                    while (LineIndex < 6)
                    {
                        XSortingList.Clear();
                        Yi = 0;
                        k = 0;
                        while (k < YSortingList.Count)
                        {
                            if (YSortingList[k] == "")
                            {
                                k++;
                                continue;
                            }

                            if (Math.Abs(YLocation - double.Parse(YSortingList[k].Split(',')[0])) < 50d)
                            {
                                XSortingList.Add(YSortingList[k].Split(',')[1]);
                                YSortingList[k] = "";
                            }
                            else
                            {
                                YLocation = double.Parse(YSortingList[k].Split(',')[0]);
                                break;
                            }
                            k++;
                            //Yi++;
                        }

                        XSortingList.Sort();

                        PointString = FindTPLineAndCamera(ImportList, LineIndex, i);

                        if (PointString != "")
                        {
                            PtSr = PointString.Split('@');

                            if (PtSr.Length == XSortingList.Count)
                            {
                                k = 0;
                                foreach (string XStr in XSortingList)
                                {
                                    KEYBOARD.SIDES[i].KEYBASELIST[int.Parse(XStr.Split('@')[1])].XPos = double.Parse(PtSr[k].Split(',')[0]);
                                    KEYBOARD.SIDES[i].KEYBASELIST[int.Parse(XStr.Split('@')[1])].YPos = double.Parse(PtSr[k].Split(',')[1]);

                                    k++;
                                }
                            }
                        }
                        LineIndex++;
                    }
                    i++;
                }


                #endregion


            }
        }

        string FindLineAndCamera(List<string> ImportList, int LineNo, int CameraNo)
        {
            string LineStr = "L" + (LineNo + 1).ToString();
            string CameraStr = "C" + (CameraNo + 1).ToString();
            bool IsFoundLine = false;
            bool IsFoundCamera = false;

            string PointStr = "";

            foreach (string ImportStr in ImportList)
            {
                if (IsFoundCamera)
                {
                    if (ImportStr.IndexOf("<") >= 0)
                    {
                        PointStr = PointStr.Remove(PointStr.Length - 1, 1);
                        break;
                    }
                    else
                    {
                        PointStr += ImportStr.Substring(29, 10) + "," + ImportStr.Substring(42, 10) + "@";
                    }
                }
                else
                {
                    if (IsFoundLine)
                    {
                        if (ImportStr.IndexOf(CameraStr) >= 0)
                        {
                            IsFoundCamera = true;
                        }
                        else if (ImportStr.IndexOf("L") >= 0)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (ImportStr.IndexOf(LineStr) >= 0)
                        {
                            IsFoundLine = true;
                        }
                    }
                }
            }

            return PointStr;
        }

        string FindTPLineAndCamera(List<string> ImportList, int TPLineNo, int CameraNo)
        {
            string LineStr = "LT" + (TPLineNo + 1).ToString();
            string CameraStr = "C" + (CameraNo + 1).ToString();
            bool IsFoundLine = false;
            bool IsFoundCamera = false;

            string PointStr = "";

            foreach (string ImportStr in ImportList)
            {
                if (IsFoundCamera)
                {
                    if (ImportStr.IndexOf("<") >= 0)
                    {
                        PointStr = PointStr.Remove(PointStr.Length - 1, 1);
                        break;
                    }
                    else
                    {
                        PointStr += ImportStr.Substring(29, 10) + "," + ImportStr.Substring(42, 10) + "@";
                    }
                }
                else
                {
                    if (IsFoundLine)
                    {
                            if (ImportStr.IndexOf(CameraStr) >= 0)
                            {
                                IsFoundCamera = true;
                            }
                            else if (ImportStr.IndexOf("L") >= 0)
                            {
                                break;
                            }
                    }
                    else
                    {
                        if (ImportStr.IndexOf(LineStr) >= 0)
                        {
                            IsFoundLine = true;
                        }
                    }
                }
            }

            return PointStr;
        }


        void btnKeyAssign_Click(object sender, EventArgs e)
        {
            //MAIN.MainTimer.Stop();
            //MAIN.AISYSAction = AISYSActionEnum.NOTHING;


            KEYASSIGNFrm = new KeyAssignForm();
            if (KEYASSIGNFrm.ShowDialog() == DialogResult.OK)
            {
                RECIPEDBUI.SetBMPDirectly(bmpKeyboard);
                KEYBOARD.GetKeybaseName();
            }

            KEYBOARD.GetKeyAssignBesides();
            KEYBOARD.GetKeyAssignCornerBesides();

            //MAIN.tmFillOPScreen.Cut();
            //MAIN.MainTimer.Start();
        }
        void btnExportData_Click(object sender, EventArgs e)
        {
            string str = "";
            List<string> KeyList = new List<string>();

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = NAME + "_" + VERSION + ".txt";
            dlg.Filter = "Text Files (*.txt)|*.TXT|" + "All files (*.*)|*.*";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                KeyList.Sort();

                foreach (string strtmp in KeyList)
                {
                    str += strtmp;
                }

                StreamWriter Sw = new StreamWriter(dlg.FileName, false, System.Text.Encoding.Default);
                Sw.Write(str);
                Sw.Flush();
                Sw.Close();
                Sw.Dispose();
            }
        }

        class ReadCornerClass
        {
            public string Name = "";
            public int Index = 0;
            public double[] CornerValue = new double[(int)CornerEnum.COUNT];

            public override string ToString()
            {
                return Name + ","
                + CornerValue[(int)CornerEnum.LT].ToString("0.00") + ";"
                + CornerValue[(int)CornerEnum.RT].ToString("0.00") + ";"
                + CornerValue[(int)CornerEnum.RB].ToString("0.00") + ";"
                + CornerValue[(int)CornerEnum.LB].ToString("0.00");
            }
        }
        public void SetLanguage(bool IsWithoutInitial)
        {
            RECIPEDBUI.SetLanguage(IsWithoutInitial);
        }

        List<ReadCornerClass> ReadCornerList = new List<ReadCornerClass>();

        void btnReadData_Click(object sender, EventArgs e)
        {
            StreamReader sr;
            String Str = "";
            String[] Strs;

            ReadCornerList.Clear();

            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "Text Files (*.txt)|*.TXT|" + "All files (*.*)|*.*";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                sr = new StreamReader(dlg.FileName);

                int ix = 0;

                //Str = sr.ReadLine();

                while (!sr.EndOfStream)
                {
                    Str = sr.ReadLine();
                    Strs = Str.Split('\t');

                    ReadCornerList.Add(new ReadCornerClass());

                    ReadCornerList[ReadCornerList.Count - 1].Name = Strs[0].Split(',')[1];
                    ReadCornerList[ReadCornerList.Count - 1].Index = int.Parse(Strs[0].Split(',')[0]);

                    ReadCornerList[ReadCornerList.Count - 1].CornerValue[(int)CornerEnum.LT] = double.Parse(Strs[2]);

                    Str = sr.ReadLine();
                    Strs = Str.Split('\t');
                    ReadCornerList[ReadCornerList.Count - 1].CornerValue[(int)CornerEnum.RT] = double.Parse(Strs[2]);

                    Str = sr.ReadLine();
                    Strs = Str.Split('\t');
                    ReadCornerList[ReadCornerList.Count - 1].CornerValue[(int)CornerEnum.LB] = double.Parse(Strs[2]);

                    Str = sr.ReadLine();
                    Strs = Str.Split('\t');
                    ReadCornerList[ReadCornerList.Count - 1].CornerValue[(int)CornerEnum.RB] = double.Parse(Strs[2]);

                    ix++;
                }


                dlg.Dispose();
                sr.Dispose();
            }
        }
        void btnOpen_Click(object sender, EventArgs e)
        {
            //if (PLC.IsEMC || !PLC.IsSafe)
            //    return;

            //if (!OpenProcess.IsOn)
            //{
            //    if (MessageBox.Show(SCREEN.Messages("msg1"), "MAIN", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //    {
            //        CloseProcess.Stop();

            //        OpenProcess.Start();
            //    }
            //}
        }
        void btnClose_Click(object sender, EventArgs e)
        {
            //if (PLC.IsEMC || !PLC.IsSafe)
            //    return;

            //if (!CloseProcess.IsOn)
            //{
            //    if (MessageBox.Show(SCREEN.Messages("msg2"), "MAIN", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //    {
            //        OpenProcess.Stop();

            //        CloseProcess.Start();
            //    }
            //}
        }
        /*
        double NextUPosition = 0;
        double NextYPosition = 0;
        public bool IStxtSFBarcode = true;
        public ProcedureClass OpenProcess = new ProcedureClass();
        void OpenProcessTick()
        {
            ProcedureClass Process = OpenProcess;

            if (Process.IsOn)
            {
                if (PLC.IsEMC)
                {
                    //PLC.Magnetic = false;
                    PLC.SafeActive();
                    PLC.MaskMag = false;
                    PLC.LED = false;

                    Process.Stop();
                }

                if (!PLC.IsSafe)
                {
                    PLC.SafeActive();
                    PLC.MaskMag = false;
                    PLC.LED = false;

                    Process.Stop();
                    MessageBox.Show("光幕保護,急停后繼續重置", "SYSTEM", MessageBoxButtons.OK);
                    IStxtSFBarcode = false;
                    return;
                }

                switch (Process.ID)
                {
                    case 5:

                        if (INI.ISMAINPROGRAM)
                        {
                            PLC.OpenActive();
                            Process.NextDuriation = 200;

                            Process.ID = 8;

                            return;

                        }


                        //PLC.LED = false;
                        //PLC.MaskMag = false;

                        PLC.SetUPosition(INI.READYULOCATION);
                        PLC.GoU = true;

                        NextUPosition = INI.READYULOCATION;

                        if (RESULT.IsTestLocation)
                        {
                            PLC.SetZPosition(INI.BASEZLOCATION - INI.BASEHEIGHT);
                            PLC.GoZ = true;
                        }

                        IsReachDo = false;

                        Process.NextDuriation = 200;

                        Process.ID = 10;
                        
                        break;
                    case 8:
                        if (Process.Processms > Process.NextDuriation)
                        {

                            PLC.LED = false;
                            if (PLC.IsOpenOK)
                                Process.Stop();
                            
                        }

                        break;
                    case 10:
                        if (Process.Processms > Process.NextDuriation)
                        {
                            if ((PLC.IsUOnSiteOK && JzTools.IsInRange(PLC.UPositionNow ,NextUPosition,0.05)) && PLC.IsZOnSiteOK)
                            {
                                PLC.Press = true;
                                PLC.MaskMag = false;

                                if (INI.MACHINETYPE == 3)
                                {
                                    PLC.SetYPosition(INI.READYYLOCATION);
                                    PLC.GoY = true;

                                    NextYPosition = INI.READYYLOCATION;
                                }

                                Process.NextDuriation = 500;
                                Process.ID = 13;
                             }
                             //if (!IsReachDo)
                             //{
                             //    if (PLC.PositionNow < (INI.BASELOCATION - 3000))
                             //    {
                             //        PLC.MaskMag = false;
                             //        IsReachDo = true;
                             //    }

                             //}
                        }
                        break;
                    case 13:
                        if (Process.Processms > Process.NextDuriation)
                        {
                            if ((PLC.IsYOnSiteOK && JzTools.IsInRange(PLC.YPositionNow,NextYPosition,0.05)) || INI.MACHINETYPE == 2)
                            {
                                PLC.MaskClose = true;
                                //PLC.Front = true;
                                PLC.Press = false;

                                Process.NextDuriation = 100;
                                Process.ID = 14;
                            }
                        }
                        break;
                    case 14:
                        if (Process.Processms > Process.NextDuriation)
                        {
                            //if (PLC.IsMaskUp)
                            {
                                PLC.Front = true;

                                Process.NextDuriation = 100;
                                Process.ID = 15;
                            }
                        }
                        break;
                    case 15:
                        if (Process.Processms > Process.NextDuriation)
                        {
                            if (PLC.IsBack)
                            {
                                Process.Stop();
                            }
                        }
                        break;
                }
            }
        }

        bool IsReachDo = false;
        public ProcedureClass CloseProcess = new ProcedureClass();
        void CloseProcessTick()
        {
            ProcedureClass Process = CloseProcess;

            if (Process.IsOn)
            {
                if (PLC.IsEMC)
                {
                    //PLC.Magnetic = false;
                    PLC.SafeActive();
                    PLC.MaskMag = false;
                    PLC.LED = false;

                    Process.Stop();
                }

                switch (Process.ID)
                {
                    case 5:

                        if (INI.ISMAINPROGRAM)
                        {
                            PLC.SetZMainAutoSpeed(Universal.HighSpeed);
                            PLC.TestActive(RESULT.IsTestLocation);


                            switch (Universal.COMPANY_MODE)
                            {
                                case MODE_DIFFERENT_COMPANY.MODE_SUNREX:
                                    PLC.CloseActive();
                                    break;
                                default:
                                    if (!PLC.PassWaySelect)
                                        PLC.CloseActive();
                                    break;
                            }

                            Process.NextDuriation = 500;
                            Process.ID = 8;

                            PLC.LED = true;

                            //if (RESULT.IsSAFE == false)
                            //{
                            //    if (RESULT.u == 0)
                            //    {
                            //        PLC.CloseActive();
                            //        RESULT.u++;
                            //    }

                            //}
                            //else if (RESULT.IsSAFE == true)
                            //{
                            //    if (RESULT.u == 0)
                            //    {
                            //        PLC.OpenActive();
                            //        RESULT.u++;
                            //        return;
                            //    }

                            //}

                            return;
                        }

                        PLC.Press = false;

                        //if (RESULT.IsTestLocation)
                        //{
                        //    PLC.SetZPosition(INI.TESTZLOCATION);
                        //}
                        //else
                        //{
                        PLC.SetZPosition(INI.BASEZLOCATION - INI.BASEHEIGHT);
                        //}

                        PLC.GoZ = true;

                        Process.NextDuriation = 100;
                        Process.ID = 10;

                        break;
                    case 8:
                        if (Process.Processms > Process.NextDuriation)
                        {
                            if (PLC.IsCloseOK)
                                Process.Stop();
                        }

                        break;
                    case 10:
                        if (Process.Processms > Process.NextDuriation)
                        {
                            //if (PLC.IsPressUp && PLC.IsZOnSiteOK && PLC.IsSafe)
                            if (PLC.IsZOnSiteOK)
                            {
                                PLC.Front = false;
                                PLC.LED = true;

                                Process.NextDuriation = 100;
                                Process.ID = 1000;
                            }
                        }
                        break;
                    case 1000:
                        if (Process.Processms > Process.NextDuriation)
                        {
                            if (PLC.IsFront)
                            {
                                Process.NextDuriation = 500;
                                Process.ID = 1005;
                            }
                        }
                        break;
                    case 1005:
                        if (Process.Processms > Process.NextDuriation)
                        {
                            if (PLC.IsFront)
                            {
                                PLC.MaskClose = false;

                                Process.NextDuriation = 100;
                                Process.ID = 1008;
                            }
                        }
                        break;
                    case 1008:
                        if (Process.Processms > Process.NextDuriation)
                        {
                            if (PLC.IsMaskDown)
                            {
                                Process.NextDuriation = 500;
                                Process.ID = 1010;
                            }
                        }
                        break;
                    //case 1009:
                    //    if (Process.Processms > Process.NextDuriation)
                    //    {
                    //        if (PLC.IsMaskDown)
                    //        {
                    //            PLC.MaskMag = true;
                    //            Process.NextDuriation = 500;
                    //            Process.ID = 1010;
                    //        }
                    //    }
                    //    break;                    
                    case 1010:
                        if (Process.Processms > Process.NextDuriation)
                        {
                            if (PLC.IsMaskDown)
                            {
                                PLC.MaskMag = true;
                                if(INI.MACHINETYPE == 3)
                                {
                                    PLC.SetYAutoSpeed(Universal.YHighSpeed);

                                    PLC.SetYPosition(INI.BASEYLOCATION);
                                    NextYPosition = INI.BASEYLOCATION;
                                    PLC.GoY = true;

                                    Process.NextDuriation = 100;
                                }

                                if (RESULT.IsTestLocation)
                                    Process.ID = 2000;
                                else
                                    Process.ID = 1020;
                            }
                        }
                        break;
                    case 2000:
                        if (Process.Processms > Process.NextDuriation)
                        {
                            if ((PLC.IsYOnSiteOK && JzTools.IsInRange(PLC.YPositionNow,NextYPosition,0.05)) || INI.MACHINETYPE == 2)
                            {
                                PLC.SetZPosition(INI.TESTZLOCATION);
                                PLC.GoZ = true;
                                PLC.SetUAutoSpeed(Universal.UHighSpeed);

                                Process.NextDuriation = 100;
                                Process.ID = 1020;
                            }
                        }
                        break;
                    case 1020:
                        if (Process.Processms > Process.NextDuriation)
                        {
                            if (((PLC.IsYOnSiteOK && JzTools.IsInRange(PLC.YPositionNow, NextYPosition, 0.05)) || INI.MACHINETYPE == 2) && PLC.IsZOnSiteOK)
                            {
                                //PLC.MaskMag = true;
                                PLC.SetUPosition(INI.BASEULOCATION - INI.SLOWULOCATION);
                                NextUPosition = INI.BASEULOCATION - INI.SLOWULOCATION;
                                PLC.GoU = true;

                                Process.NextDuriation = 100;
                                Process.ID = 1030;
                            }
                        }
                        break;
                    case 1030:
                        if (Process.Processms > Process.NextDuriation)
                        {
                            if ((PLC.IsUOnSiteOK && JzTools.IsInRange(PLC.UPositionNow,NextUPosition,0.05)))
                            {
                                PLC.SetUAutoSpeed(Universal.ULowSpeed);
                                PLC.SetUPosition(INI.BASEULOCATION);
                                NextUPosition = INI.BASEULOCATION;
                                PLC.GoU = true;

                                Process.NextDuriation = 100;
                                Process.ID = 20;
                            }
                        }
                        break;   
                    case 20:
                        if (Process.Processms > Process.NextDuriation)
                        {
                            if (PLC.IsUOnSiteOK && JzTools.IsInRange(PLC.UPositionNow, NextUPosition, 0.02))
                            {
                                if(INI.MACHINETYPE == 3)
                                    PLC.SetYAutoSpeed(Universal.YLowSpeed);

                                PLC.SetUAutoSpeed(Universal.UHighSpeed);

                                Process.Stop();
                            }

                        }
                        break;
                }
            }
        }

        public ProcedureClass PassageWayLoadProcess = new ProcedureClass();
        void PassageWayLoadTick()
        {
            ProcedureClass Process = PassageWayLoadProcess;

            if (Process.IsOn)
            {
                if (PLC.IsEMC)
                {
                    PLC.SafeActive();
                    PLC.LED = false;
                    Process.Stop();
                }

                if (!PLC.IsSafe)
                {
                    PLC.SafeActive();
                    PLC.LED = false;

                    Process.Stop();
                    MessageBox.Show("光幕保護,急停后繼續重置", "SYSTEM", MessageBoxButtons.OK);
                    IStxtSFBarcode = false;
                    return;
                }

                switch (Process.ID)
                {
                    case 5:
                        if (INI.ISMAINPROGRAM)
                        {
                            PLC.PassageWay_Load();
                            Process.NextDuriation = 200;
                            Process.ID = 8;
                        }
                        break;
                    case 8:
                        if (Process.Processms > Process.NextDuriation)
                        {
                            if (PLC.IsPassageWay_LoadOK)
                                Process.Stop();
                        }
                        break;
                }
            }
        }

        public ProcedureClass PassageWayUnLoadProcess = new ProcedureClass();
        void PassageWayUnLoadTick()
        {
            ProcedureClass Process = PassageWayUnLoadProcess;

            if (Process.IsOn)
            {
                if (PLC.IsEMC)
                {
                    PLC.SafeActive();
                    PLC.LED = false;
                    Process.Stop();
                }

                if (!PLC.IsSafe)
                {
                    PLC.SafeActive();
                    PLC.LED = false;

                    Process.Stop();
                    MessageBox.Show("光幕保護,急停后繼續重置", "SYSTEM", MessageBoxButtons.OK);
                    IStxtSFBarcode = false;
                    return;
                }

                switch (Process.ID)
                {
                    case 5:
                        if (INI.ISMAINPROGRAM)
                        {
                            PLC.PassageWay_UnLoad();
                            Process.NextDuriation = 200;
                            Process.ID = 8;
                        }
                        break;
                    case 8:
                        if (Process.Processms > Process.NextDuriation)
                        {
                            if (PLC.IsPassageWay_UnLoadOK)
                                Process.Stop();
                        }
                        break;
                }
            }
        }
        */
        void btnEDIT_Click(object sender, EventArgs e)
        {
            //MAIN.MainTimer.Stop();
            //MAIN.AISYSAction = AISYSActionEnum.NOTHING;

            GetAllBMP((SideEnum)RECIPEDBUI.GetSideSelection());

            BASETEACHINGFrm = new BaseTeachingForm((SideEnum)RECIPEDBUI.GetSideSelection());
            if (BASETEACHINGFrm.ShowDialog() == DialogResult.OK)
            {
                KEYBOARD.GetKeyAssignRelateBase();
                KEYBOARD.GetKeybaseCheckingSequence((int)(SideEnum)RECIPEDBUI.GetSideSelection());
            }
            BASETEACHINGFrm.Dispose();

            ReduceSideBMP((SideEnum)RECIPEDBUI.GetSideSelection(), TeachingTypeEnum.BASE);

            //MAIN.tmFillOPScreen.Cut();
            //MAIN.MainTimer.Start();
        }
        void btnGetBase_Click(object sender, EventArgs e)
        {
            int i = 0;
            bool IsRefresh = true;

            if (MessageBox.Show("此動作將會把所有的基準圖換掉，並清除基準資料，是否執行?", "MAIN", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                i = 0;
                while (i < (int)INI.SIDECOUNT)
                {
                    CCD.Snap((SideEnum)i);


                    KEYBOARD.SIDES[i].GetBaseBMP(CCD.GetBMP((SideEnum)i));
                    KEYBOARD.SIDES[i].IsReget[(int)TeachingTypeEnum.BASE] = true;
                    KEYBOARD.SIDES[i].IsRegetAlready[(int)TeachingTypeEnum.BASE] = true;

                    KEYBOARD.SaveBaseBMP((SideEnum)i, Universal.PICPATH + ID.ToString("0000") + "\\TB" + (i).ToString("000") + ".BMP");
                    KEYBOARD.SIDES[i].ReduceBMP(TeachingTypeEnum.BASE);

                    if (IsRefresh)
                    {
                        if (KEYBOARD.SIDES[i].KEYBASELIST != null)
                        {
                            foreach (KeybaseClass keybase in KEYBOARD.SIDES[i].KEYBASELIST)
                            {
                                keybase.Dispoe();
                            }
                            KEYBOARD.SIDES[i].KEYBASELIST.Clear();
                        }
                    }
                    CCD.ClearBMP((SideEnum)i);

                    i++;
                }
            }
        }
        void btnGetBackgroud_Click(object sender, EventArgs e)
        {
            //int i = 0;
            //if (MessageBox.Show(SCREEN.Messages("msg3"), "MAIN", MessageBoxButtons.YesNo) == DialogResult.Yes)
            if (MessageBox.Show("此動作將會把所有的背景圖換掉，是否執行?", "MAIN", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                GetBackgroundSub();
            }
        }

        void GetBackgroundSub()
        {
            int i = 0;
            while (i < (int)INI.SIDECOUNT)
            {
                CCD.Snap((SideEnum)i);

                KEYBOARD.SIDES[i].GetBackgroundBMP(CCD.GetBMP((SideEnum)i));
                KEYBOARD.SIDES[i].IsReget[(int)TeachingTypeEnum.BACKGROUD] = true;
                KEYBOARD.SIDES[i].IsRegetAlready[(int)TeachingTypeEnum.BACKGROUD] = true;

                KEYBOARD.SaveBackgroudBMP((SideEnum)i, Universal.PICPATH + ID.ToString("0000") + "\\TS" + (i).ToString("000") + ".BMP");
                KEYBOARD.SIDES[i].ReduceBMP(TeachingTypeEnum.BACKGROUD);

                CCD.ClearBMP((SideEnum)i);

                i++;
            }
        }

        bool IsAnalyze = false;

        void btnGetAnalyze_Click(object sender, EventArgs e)
        {
            int i = 0;
            bool IsRefresh = true;

            if (MessageBox.Show("此動作將會把所有的解析圖換掉，是否執行?", "MAIN", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //if (MessageBox.Show(SCREEN.Messages("msg4"), "MAIN", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                GetAnalyzeSub();

            }
        }

        void GetAnalyzeSub()
        {
            int i = 0;
            while (i < (int)INI.SIDECOUNT)
            {
                CCD.Snap((SideEnum)i);


                KEYBOARD.SIDES[i].GetAnalyzeBMP(CCD.GetBMP((SideEnum)i));
                KEYBOARD.SIDES[i].IsReget[(int)TeachingTypeEnum.ANALYZE] = true;
                KEYBOARD.SIDES[i].IsRegetAlready[(int)TeachingTypeEnum.ANALYZE] = true;

                KEYBOARD.SaveAnalyzeBMP((SideEnum)i, Universal.PICPATH + ID.ToString("0000") + "\\TA" + (i).ToString("000") + ".BMP");
                KEYBOARD.SIDES[i].ReduceBMP(TeachingTypeEnum.ANALYZE);

                CCD.ClearBMP((SideEnum)i);

                i++;
            }

            if (INI.ISAISYS)
            {
                //MESSAGEFrm.Close();
                //MESSAGEFrm.Dispose();
            }

        }

        void btnStep1_Click(object sender, EventArgs e)
        {

        }

        //TimerClass RecipeTimer = new TimerClass();
        public void Tick()
        {
            //if (Universal.IsNoUsePLC)
            //    return;

            //OpenProcessTick();
            //CloseProcessTick();

            //switch (Universal.COMPANY_MODE)
            //{
            //    case MODE_DIFFERENT_COMPANY.MODE_SUNREX:
            //        break;
            //    default:
            //        PassageWayLoadTick();
            //        PassageWayUnLoadTick();

            //        btnBigOrSmallKBSelect.BackColor = (!PLC.BigOrSmallKBSelect ? Color.FromArgb(255, 224, 192) : Color.Red);
            //        btnBigOrSmallKBSelect.Text = (!PLC.BigOrSmallKBSelect ? "鍵盤切換(小)" : "鍵盤切換(大)");

            //        btnPaasWaySelect.BackColor = (!PLC.PassWaySelect ? Color.FromArgb(255, 224, 192) : Color.Red);
            //        btnPaasWaySelect.Text = (!PLC.PassWaySelect ? "流道打開" : "流道關閉");
            //        break;
            //}

            if (!IsDebug)
                CaptureTick();

            //btnOpen.Enabled = PLC.UPositionNow == INI.BASEULOCATION && !(OpenProcess.IsOn || CloseProcess.IsOn);
            //btnClose.Enabled = PLC.UPositionNow == INI.READYULOCATION && !(OpenProcess.IsOn || CloseProcess.IsOn);

            //btnHome.BackColor = (PLC.IsZHomeOK ? Color.FromArgb(255, 224, 192) : Color.Red);


        }

        void LIGTHCONTROLDB_RecipeChangeAction()
        {
            SetControlComboBox();
        }
        public void SetCameraControl()
        {
            //RECIPEDBUI.LIGHTUI.SetCameraControl(SideEnum.COUNT);
        }
        public void SetLightControl()
        {
            //RECIPEDBUI.LIGHTUI.SetLightControl();
        }

        void btnSetup_Click(object sender, EventArgs e)
        {
            SetControlString();

            SetCameraControl();
            SetLightControl();
        }
        void btnCancel_Click(object sender, EventArgs e)
        {
            //PLC.SetZAutoSpeed(Universal.HighSpeed);


            //btnRecipeSelection.Enabled = true;
            //MESSAGEFrm = new MessageForm(SCREEN.Messages("msg5"));
            //MESSAGEFrm.Show();
            //MESSAGEFrm.Refresh();

            Cancel(RECIPEDBUI.DBStatus);
            RECIPEDBUI.DBStatus = DBStatusEnum.NONE;

            Load(false);

            FillDisplay();

            OnResizeKeyboardRange(false);

            ReduceSideBMP();
            DELTMPFile();

            //MESSAGEFrm.Dispose();
        }
        void btnOK_Click(object sender, EventArgs e)
        {
            //PLC.SetZAutoSpeed(Universal.HighSpeed);

            //btnRecipeSelection.Enabled = true;
            //MESSAGEFrm = new MessageForm(SCREEN.Messages("msg6"));
            //MESSAGEFrm.Show();
            //MESSAGEFrm.Refresh();

            BackupOldDB();

            INI.LAST_RECIPEID = ID;
            INI.Save();

            //Application.DoEvents();

            //MESSAGEFrm.TopMost = false;
            
            Save(RECIPEDBUI.DBStatus == DBStatusEnum.ADD || RECIPEDBUI.DBStatus == DBStatusEnum.COPY);

            Rollback(RECIPEDBUI.DBStatus);
            RECIPEDBUI.DBStatus = DBStatusEnum.NONE;

            OnResizeKeyboardRange(false);

            //lblRecipeName.Text = "[" + ID.ToString() + "] " + NAME + "(" + VERSION + ")";

            ReduceSideBMP();
            DELTMPFile();
            //MESSAGEFrm.Dispose();
        }
        
        void DELTMPFile()
        {
            string[] FileStr;
            
            FileStr = Directory.GetFiles(Universal.PICPATH +  ID.ToString("0000"), "T*");

            foreach (string filestr in FileStr)
            {
                File.Delete(filestr);
            }
        }

        public void BackupOldDB()
        {
            return;

            if (!Directory.Exists(@"D:\AUTOMATION\Eazy Key Height Check\Jumbo\BACKUP"))
            {
                Directory.CreateDirectory(@"D:\AUTOMATION\Eazy Key Height Check\Jumbo\BACKUP");
            }

            string[] Dir = Directory.GetDirectories(@"D:\AUTOMATION\Eazy Key Height Check\Jumbo\BACKUP");

            int i = 0;
            while (i < Dir.Length)
            {
                if (int.Parse(Dir[i].Substring(Dir[i].Length - 14, 8)) <= int.Parse(JzTimes.DateAdd(-180).Replace("/", "").Replace("-", "")))
                    //if (int.Parse(Dir[i].Substring(Dir[i].Length - 14, 8)) <= int.Parse(TimerClass.DateAdd(-180).Replace("/", "").Replace("-", "")))
                    {
                    Directory.Delete(Dir[i], true);
                }
                i++;
            }

            string SaveDate = JzTimes.DateTimeString;// TimerClass.DateTimeSerialString;

            Directory.CreateDirectory(@"D:\AUTOMATION\Eazy Key Height Check\Jumbo\BACKUP\" + SaveDate);
            Directory.CreateDirectory(@"D:\AUTOMATION\Eazy Key Height Check\Jumbo\BACKUP\" + SaveDate + "\\CODE");

            File.Copy(@"D:\AUTOMATION\Eazy Key Height Check\Jumbo\JUMBO301FX.INI", @"D:\AUTOMATION\Eazy Key Height Check\Jumbo\BACKUP\" + SaveDate + "\\JUMBO301FX.INI", true);
            File.Copy(@"D:\AUTOMATION\Eazy Key Height Check\Jumbo\DB\DataDbFX.MDB", @"D:\AUTOMATION\Eazy Key Height Check\Jumbo\BACKUP\" + SaveDate + "\\DataDbFX.MDB", true);

            string[] Files = Directory.GetFiles(@"D:\AUTOMATION\Eazy Key Height Check\CODE 3.52F\Jumbo301\bin\Release");

            i = 0;
            while (i < Files.Length)
            {
                File.Copy(Files[i], @"D:\AUTOMATION\Eazy Key Height Check\Jumbo\BACKUP\" + SaveDate + "\\CODE\\" + Files[i].Split('\\')[7], true);
                i++;
            }
        }

        public void FillDisplay()
        {
            FillDisplay(true);
        }
        public void FillDisplay(bool IsUpdatingKeyboardInformation)
        {
            if (IsUpdatingKeyboardInformation)
            {
                RECIPEDBUI.txtName.Text = NAME;
                RECIPEDBUI.txtVersion.Text = VERSION;

                //RECIPEDBUI.lblDatetime.Text = SCREEN.Messages("msg8") + CREATEDATETIME.PadRight(40, ' ') + SCREEN.Messages("msg9") + MODIFYDATETIME;

                RECIPEDBUI.chkIsCheckHighest.Checked = IsCheckHighest;
                RECIPEDBUI.txtHighestValue.Text = HighestValue.ToString("0.000");

                RECIPEDBUI.chkIsCheckLowest.Checked = IsCheckLowest;
                RECIPEDBUI.txtLowestValue.Text = LowestValue.ToString("0.000");

                RECIPEDBUI.chkIsCheckSlope.Checked = IsCheckSlope;
                RECIPEDBUI.txtSlopeValue.Text = SlopeValue.ToString("0.000");

                RECIPEDBUI.chkIsCheckMutual.Checked = IsCheckMutual;
                RECIPEDBUI.txtMutualValue.Text = MutualValue.ToString("0.000");

                RECIPEDBUI.chkIsCheckWrongCount.Checked = IsCheckWrongCount;
                RECIPEDBUI.txtWrongCount.Text = WrongCount.ToString();

            }

            SetControlString(LASTCONTROLSTRING);

        }

        public void GetSideBMP()
        {
            //MESSAGEFrm = new MessageForm(SCREEN.Messages("msg7"), true);
            //MESSAGEFrm.Show();
            //MESSAGEFrm.Refresh();

            int i = 0;

            //RECIPEDBUI.lstSample.SelectedIndex = j;
            i = 0;
            while (i < (int)SideEnum.COUNT)
            {
                KEYBOARD.GetBaseBMP((SideEnum)i, Universal.PICPATH + ID.ToString("0000") + "\\B" + i.ToString("000") + ".BMP");

                i++;
            }

            //MESSAGEFrm.Close();
            //MESSAGEFrm.Dispose();
        }
        public void GetSideBMP(SideEnum side, TeachingTypeEnum teachingtype)
        {
            //MESSAGEFrm = new MessageForm(SCREEN.Messages("msg7"), true);
            //MESSAGEFrm.Show();
            //MESSAGEFrm.Refresh();

            int i = (int)side;

            if (!KEYBOARD.SIDES[i].IsRegetAlready[(int)TeachingTypeEnum.BASE])
                KEYBOARD.GetBaseBMP(side, Universal.PICPATH + ID.ToString("0000") + "\\B" + i.ToString("000") + ".BMP");
            else
                KEYBOARD.GetBaseBMP(side, Universal.PICPATH + ID.ToString("0000") + "\\TB" + i.ToString("000") + ".BMP");

            if (!KEYBOARD.SIDES[i].IsRegetAlready[(int)TeachingTypeEnum.ANALYZE])
                KEYBOARD.GetBaseBMP(side, Universal.PICPATH + ID.ToString("0000") + "\\A" + i.ToString("000") + ".BMP");
            else
                KEYBOARD.GetBaseBMP(side, Universal.PICPATH + ID.ToString("0000") + "\\TA" + i.ToString("000") + ".BMP");

            if (!KEYBOARD.SIDES[i].IsRegetAlready[(int)TeachingTypeEnum.BACKGROUD])
                KEYBOARD.GetBaseBMP(side, Universal.PICPATH + ID.ToString("0000") + "\\S" + i.ToString("000") + ".BMP");
            else
                KEYBOARD.GetBaseBMP(side, Universal.PICPATH + ID.ToString("0000") + "\\TS" + i.ToString("000") + ".BMP");

            //MESSAGEFrm.Close();
            //MESSAGEFrm.Dispose();
        }
        public void GetAllBMP(SideEnum side)
        {
            //MESSAGEFrm = new MessageForm(SCREEN.Messages("msg7"), true);
            //MESSAGEFrm.Show();
            //MESSAGEFrm.Refresh();

            int i = (int)side;

            if (!KEYBOARD.SIDES[i].IsRegetAlready[(int)TeachingTypeEnum.BASE])
                KEYBOARD.GetBaseBMP(side, Universal.PICPATH + ID.ToString("0000") + "\\B" + i.ToString("000") + ".BMP");
            else
                KEYBOARD.GetBaseBMP(side, Universal.PICPATH + ID.ToString("0000") + "\\TB" + i.ToString("000") + ".BMP");

            if (!KEYBOARD.SIDES[i].IsRegetAlready[(int)TeachingTypeEnum.ANALYZE])
                KEYBOARD.GetAnalyzeBMP(side, Universal.PICPATH + ID.ToString("0000") + "\\A" + i.ToString("000") + ".BMP");
            else
                KEYBOARD.GetAnalyzeBMP(side, Universal.PICPATH + ID.ToString("0000") + "\\TA" + i.ToString("000") + ".BMP");

            if (!KEYBOARD.SIDES[i].IsRegetAlready[(int)TeachingTypeEnum.BACKGROUD])
                KEYBOARD.GetBackgroudBMP(side, Universal.PICPATH + ID.ToString("0000") + "\\S" + i.ToString("000") + ".BMP");
            else
                KEYBOARD.GetBackgroudBMP(side, Universal.PICPATH + ID.ToString("0000") + "\\TS" + i.ToString("000") + ".BMP");

            //MESSAGEFrm.Close();
            //MESSAGEFrm.Dispose();
        }

        public void ReduceSideBMP()
        {
            int i = 0;

            while (i < (int)SideEnum.COUNT)
            {
                KEYBOARD.SIDES[i].ReduceBMP();
                i++;
            }

        }
        public void ReduceSideBMP(SideEnum side, TeachingTypeEnum teachingtype)
        {
            int i = 0;

            i = (int)side;

            switch (teachingtype)
            {
                case TeachingTypeEnum.ANALYZE:
                    if (!KEYBOARD.SIDES[i].IsRegetAlready[(int)teachingtype])
                        KEYBOARD.SIDES[i].ReduceBMP(teachingtype);
                    else
                    {
                        KEYBOARD.SaveAnalyzeBMP(side, Universal.PICPATH + ID.ToString("0000") + "\\TA" + i.ToString("000") + ".BMP");
                        KEYBOARD.SIDES[i].ReduceBMP(teachingtype);
                    }
                    break;
                case TeachingTypeEnum.BASE:
                    if (!KEYBOARD.SIDES[i].IsRegetAlready[(int)teachingtype])
                        KEYBOARD.SIDES[i].ReduceBMP(teachingtype);
                    else
                    {
                        KEYBOARD.GetBaseBMP(side, Universal.PICPATH + ID.ToString("0000") + "\\TB" + i.ToString("000") + ".BMP");
                        KEYBOARD.SIDES[i].ReduceBMP(teachingtype);
                    }
                    break;
                case TeachingTypeEnum.BACKGROUD:
                    if (!KEYBOARD.SIDES[i].IsRegetAlready[(int)teachingtype])
                        KEYBOARD.SIDES[i].ReduceBMP(teachingtype);
                    else
                    {
                        KEYBOARD.SaveBackgroudBMP(side, Universal.PICPATH + ID.ToString("0000") + "\\TS" + i.ToString("000") + ".BMP");
                        KEYBOARD.SIDES[i].ReduceBMP(teachingtype);
                    }
                    break;
            }
        }
        public void GetKeyboardBMP(string FileName)
        {
            Bitmap bmp = new Bitmap(FileName);
            bmpKeyboard.Dispose();
            bmpKeyboard = new Bitmap(bmp);

            bmp.Dispose();
        }

        public void Save(bool IsAdded)
        {
            NAME = RECIPEDBUI.txtName.Text;
            VERSION = RECIPEDBUI.txtVersion.Text;

            //LASTCONTROLSTRING = RECIPEDBUI.LIGHTUI.GetUIControl();

            IsCheckHighest = RECIPEDBUI.chkIsCheckHighest.Checked;
            HighestValue = double.Parse(RECIPEDBUI.txtHighestValue.Text);

            IsCheckLowest = RECIPEDBUI.chkIsCheckLowest.Checked;
            LowestValue = double.Parse(RECIPEDBUI.txtLowestValue.Text);

            IsCheckSlope = RECIPEDBUI.chkIsCheckSlope.Checked;
            SlopeValue = double.Parse(RECIPEDBUI.txtSlopeValue.Text);

            IsCheckMutual = RECIPEDBUI.chkIsCheckMutual.Checked;
            MutualValue = double.Parse(RECIPEDBUI.txtMutualValue.Text);

            IsCheckWrongCount = RECIPEDBUI.chkIsCheckWrongCount.Checked;
            WrongCount = int.Parse(RECIPEDBUI.txtWrongCount.Text);

            string Str = "";

            Str += JzTools.RecttoString(rectRecipeRange) + "@";
            Str += (IsCheckHighest ? "1" : "0") + "@";
            Str +=  HighestValue.ToString() + "@";
            Str += (IsCheckLowest ? "1" : "0") + "@";
            Str += LowestValue.ToString() + "@";
            Str += (IsCheckSlope ? "1" : "0") + "@";
            Str += SlopeValue.ToString() + "@";
            Str += (IsCheckMutual ? "1" : "0") + "@";
            Str += MutualValue.ToString() + "@";
            Str += (IsCheckWrongCount ? "1" : "0") + "@";
            Str += WrongCount.ToString();


            RECIPEINFORMATION = Str;
            MODIFYDATETIME = JzTimes.DateTimeString;// TimerClass.DateTimeString;

            bmpKeyboard.Save(Universal.PICPATH + ID.ToString("0000") + @"\KB.BMP", ImageFormat.Bmp);
            RECIPEDBUI.SetBMPDirectly(bmpKeyboard);

            SaveKeyassign();
            SaveKeybase();

            SaveSide(false);

            KEYBOARD.GetKeyAssignBesides();
            KEYBOARD.GetKeyAssignCornerBesides();
        }
        public void Load(bool IsAdded)
        {
            int i = 0, j = 0;

            string[] Str = RECIPEINFORMATION.Split('@');

            rectRecipeRange = JzTools.StringtoRect(Str[0]);

            if (Str.Length > 1)
            {
                if (Str[1] != "")
                {
                    IsCheckHighest = Str[1] == "1";
                    HighestValue = double.Parse(Str[2]);

                    IsCheckLowest = Str[3] == "1";
                    LowestValue = double.Parse(Str[4]);

                    IsCheckSlope = Str[5] == "1";
                    SlopeValue = double.Parse(Str[6]);

                    IsCheckMutual = Str[7] == "1";
                    MutualValue = double.Parse(Str[8]);

                    IsCheckWrongCount = Str[9] == "1";
                    WrongCount = int.Parse(Str[10]);
                }
            }
            GetKeyboardBMP(Universal.PICPATH + ID.ToString("0000") + "\\KB.BMP");

            KEYBOARD.ID = ID;
            KEYBOARD.Name = NAME;
            KEYBOARD.VERSION = VERSION;

            if (!IsAdded)
            {
                KEYBOARD.DisposeBMP();
            }

            //Gaara by mask
            if (RECIPEDBUI != null)
                RECIPEDBUI.SetBMPDirectly(bmpKeyboard);

            LoadKeyassign();
            LoadSide(j);


            //TimerClass LoadTime = new TimerClass();
            //TimerClass LoadTime1 = new TimerClass();

            //LoadTime.Cut();

            if (!IsAdded)
            {
                //i = Universal.StartSide;

                while (i < INI.SIDECOUNT)
                {
                    //LoadTime1.Cut();

                    KEYBOARD.GetBaseBMP((SideEnum)i, Universal.PICPATH + ID.ToString("0000") + "\\B" + i.ToString("000") + ".BMP");

                    LoadKeybase((SideEnum)i);
                    KEYBOARD.GetKeybaseCheckingSequence(i);

                    KEYBOARD.SIDES[i].ReduceBMP();

                    //if (Universal.IsOnlyOne)
                    //    break;

                    //MessageBox.Show(LoadTime1.msDuriation.ToString());

                    i++;
                }
            }

            //MessageBox.Show(LoadTime.msDuriation.ToString());

            KEYBOARD.GetKeyAssignBesides();
            KEYBOARD.GetKeyAssignCornerBesides();
            
            KEYBOARD.GetKeybaseName();
            KEYBOARD.GetKeyAssignRelateBase();
            

            GenerateTestFile();

        }

        public void GenerateTestFile()
        {
            //int i = 0;
            string CheckString = "";
            string CheckSubString = "";

            #region Generate Keyassign and Keybesides File
            foreach (KeyAssignClass keyassign in KEYBOARD.KEYASSIGNLIST)
            {
                CheckString += "NAME:" + keyassign.Name + ",";

                //if (keyassign.belongKeycapIndex != -1)
                //{
                //    //CheckString += "RELATE_KEYCAP:" + KEYBOARD.SIDES[(int)keyassign.belongSide].KEYCAPLIST[keyassign.belongKeycapIndex].AliasName + ",";

                //    CheckSubString = "";

                //    i = 0;

                //    while (i < (int)BesideEnum.COUNT)
                //    {
                //        List<int> besidelist = keyassign.ListBeside[i];

                //        //foreach (List<int> besidelist in keyassign.ListBeside)
                //        //{
                //        if (besidelist.Count < 1)
                //        {
                //            CheckSubString += ((BesideEnum)i).ToString() + ":NONE,";
                //        }
                //        else
                //        {
                //            CheckSubString += ((BesideEnum)i).ToString() + ":";

                //            foreach (int BesideIndex in keyassign.ListBeside[i])
                //            {
                //                KeyAssignClass kass = KEYBOARD.KEYASSIGNLIST[BesideIndex];

                //                //if (kass.belongKeycapIndex == -1)
                //                //{
                //                //    CheckSubString += kass.Name + ",";
                //                //}
                //                //else
                //                //{
                //                //    CheckSubString += KEYBOARD.SIDES[(int)kass.belongSide].KEYCAPLIST[kass.belongKeycapIndex].AliasName + ",";
                //                //}
                //            }
                //        }
                //        //CheckSubString += CheckSubString;
                //        //}
                //        i++;
                //    }
                //}

                if (CheckSubString != "")
                    CheckString += CheckSubString.Remove(CheckSubString.Length - 1) + Environment.NewLine;
            }

            JzTools.SaveData(CheckString, @"D:\LOA\CheckBesideStr.TXT");
            #endregion

        }

        public void SaveSide(bool IsDuringAdded)
        {
            int i = 0;

            string SaveStr = "";
            StreamWriter Sw = new StreamWriter(Universal.PICPATH + ID.ToString("0000") + "\\" + "side.db");

            while (i < INI.SIDECOUNT)
            {
                SideClass side = KEYBOARD.SIDES[i];

                if (!IsDuringAdded)
                {
                    if (side.IsRegetAlready[(int)TeachingTypeEnum.BASE])
                        File.Copy(Universal.PICPATH + ID.ToString("0000") + "\\TB" + i.ToString("000") + ".BMP", Universal.PICPATH + ID.ToString("0000") + "\\B" + i.ToString("000") + ".BMP", true);
                    if (side.IsRegetAlready[(int)TeachingTypeEnum.ANALYZE])
                        File.Copy(Universal.PICPATH + ID.ToString("0000") + "\\TA" + i.ToString("000") + ".BMP", Universal.PICPATH + ID.ToString("0000") + "\\A" + i.ToString("000") + ".BMP", true);
                    if (side.IsRegetAlready[(int)TeachingTypeEnum.BACKGROUD])
                        File.Copy(Universal.PICPATH + ID.ToString("0000") + "\\TS" + i.ToString("000") + ".BMP", Universal.PICPATH + ID.ToString("0000") + "\\S" + i.ToString("000") + ".BMP", true);
                }

                SaveStr += i.ToString() + IndexSeg + i.ToString() + SectionSeg;

                i++;
            }

            if (SaveStr.Length > 0)
                SaveStr = SaveStr.Remove(SaveStr.Length - 1, 1);

            Sw.Write(SaveStr);
            Sw.Flush();
            Sw.Dispose();       
        }
        public void LoadSide(int KeyboardIndex)
        {
            if (!File.Exists(Universal.PICPATH + ID.ToString("0000") + "\\side.db"))
                return;

            StreamReader Sr = new StreamReader(Universal.PICPATH + ID.ToString("0000") + "\\side.db");
            string ReadStr = Sr.ReadToEnd();

            Sr.Close();
            Sr.Dispose();

            if (ReadStr == "")
                return;


            int i = 0;
            string[] SectionStr = ReadStr.Split(SectionSeg);
            string[] PartStr;

            while (i < SectionStr.Length)
            {
                PartStr = SectionStr[i].Split(IndexSeg);

                //if (KEYBOARD.SIDES[int.Parse(PartStr[0])].KEYCAPLIST.Count > 0)
                //{
                //    foreach (KeycapClass keycap in KEYBOARD.SIDES[int.Parse(PartStr[0])].KEYCAPLIST)
                //    {
                //        keycap.Dispoe();
                //    }

                //    KEYBOARD.SIDES[int.Parse(PartStr[0])].KEYCAPLIST.Clear();
                //}

                KEYBOARD.SIDES[int.Parse(PartStr[0])].SideInformation = PartStr[1];
                i++;
            }
        }

        public string FileGetAndAnalyzePath
        {
            get { return Universal.PICPATH + ID.ToString("0000"); }
        }

        public void SaveKeybase()
        {
            int i = 0;

            string SaveStr = "";
            StreamWriter Sw = new StreamWriter(Universal.PICPATH + ID.ToString("0000") + "\\" + "keybase.db");

            i = 0;
            while(i < INI.SIDECOUNT)
            {
                SideClass side = KEYBOARD.SIDES[i];
                foreach (KeybaseClass keybase in side.KEYBASELIST)
                {
                    SaveStr += i.ToString() + IndexSeg + keybase.ToString() + SectionSeg;
                }
                i++;
            }

            if (SaveStr.Length > 0)
                SaveStr = SaveStr.Remove(SaveStr.Length - 1, 1);

            Sw.Write(SaveStr);
            Sw.Flush();
            Sw.Dispose();
        }
        public void LoadKeybase(SideEnum side)
        {
            if (!File.Exists(Universal.PICPATH + ID.ToString("0000") + "\\keybase.db"))
                return;

            StreamReader Sr = new StreamReader(Universal.PICPATH + ID.ToString("0000") + "\\keybase.db");
            string ReadStr = Sr.ReadToEnd();

            Sr.Close();
            Sr.Dispose();

            if (ReadStr == "")
                return;


            string[] SectionStr = ReadStr.Split(SectionSeg);
            string[] PartStr;
            int i = 0;

            while (i < SectionStr.Length)
            {
                PartStr = SectionStr[i].Split(IndexSeg);
                if (int.Parse(PartStr[0]) == (int)side)
                {
                    KEYBOARD.SIDES[int.Parse(PartStr[0])].KEYBASELIST.Add(new KeybaseClass((SideEnum)int.Parse(PartStr[0]), PartStr[1]));
                }
                i++;
            }
        }

        public void SaveKeyassign()
        {
            string SaveStr = "";
            StreamWriter Sw = new StreamWriter(Universal.PICPATH + ID.ToString("0000") + "\\" + "keyassign.db");

            foreach (KeyAssignClass keyassign in KEYBOARD.KEYASSIGNLIST)
            {
                SaveStr += keyassign.ToString() + SectionSeg;
            }

            if (SaveStr.Length > 0)
                SaveStr = SaveStr.Remove(SaveStr.Length - 1, 1);

            Sw.Write(SaveStr);
            Sw.Flush();
            Sw.Dispose();
        }
        public void LoadKeyassign()
        {
            if (!File.Exists(Universal.PICPATH + ID.ToString("0000") + "\\keyassign.db"))
                return;

            StreamReader Sr = new StreamReader(Universal.PICPATH + ID.ToString("0000") + "\\keyassign.db");
            string ReadStr = Sr.ReadToEnd();

            Sr.Close();
            Sr.Dispose();

            if (ReadStr == "")
                return;

            string[] SectionStr = ReadStr.Split(SectionSeg);
            int i = 0;

            while (i < SectionStr.Length)
            {
                KEYBOARD.KEYASSIGNLIST.Add(new KeyAssignClass(SectionStr[i]));
                i++;
            }
        }

        public void SetControlComboBox()
        {
            //RECIPEDBUI.cboControl.Items.Clear();

            //int i = 0;
            //while (i < LIGTHCONTROLDB.ListName.Count)
            //{
            //    RECIPEDBUI.cboControl.Items.Add(LIGTHCONTROLDB.ListName[i].Split(',')[0]);
            //    i++;
            //}
            //RECIPEDBUI.cboControl.SelectedIndex = 0;
        }
        public void SetControlString()
        {
            SetControlString("");
        }
        public void SetControlString(string ControlStr)
        {
            //string Str = ControlStr;
            //if (Str == "")
            //{
            //    int ID = int.Parse(LIGTHCONTROLDB.ListName[RECIPEDBUI.cboControl.SelectedIndex].Split(',')[1]);

            //    LIGTHCONTROLDB.Goto(ID);
            //    Str = LIGTHCONTROLDB.CONTROLString;
            //}
            //RECIPEDBUI.LIGHTUI.SetUIControl(Str);
        }

        void btnRecipeSelection_Click(object sender, EventArgs e)
        {
            //Universal.GlobalPassInteger = ID;

            //Universal.MAIN.IsOperating = true;

            //RECIPESELECTIONFRM = new RecipeSelectionForm();
            //if (RECIPESELECTIONFRM.ShowDialog() == DialogResult.OK)
            //{
            //    MESSAGEFrm = new MessageForm(SCREEN.Messages("msg10"));
            //    MESSAGEFrm.Show();
            //    MESSAGEFrm.Refresh();
            //    //Application.DoEvents();

            //    Goto(Universal.GlobalPassInteger);

            //    Load(false);

            //    FillDisplay();

            //    RECIPEDBUI.LIGHTUI.SetCameraControl(SideEnum.COUNT);
            //    RECIPEDBUI.LIGHTUI.SetLightControl();

            //    lblRecipeName.Text = "[" + ID.ToString() + "] " + NAME + "(" + VERSION + ")";

            //    INI.LAST_RECIPEID = ID;
            //    INI.Save();

            //    MESSAGEFrm.Dispose();
            //}

            //Universal.MAIN.IsOperating = false;
        }
        void btnCopy_Click(object sender, EventArgs e)
        {
            //Bitmap[] SIDEBmp = new Bitmap[(int)SideEnum.COUNT];

            int i = 0;
            bool IsRenewOrg = false;

            //GetSideBMP();

            //i = 0;
            //while (i < (int)INI.SIDECOUNT)
            //{
            //    SIDEBmp[i] = (Bitmap)KEYBOARD.SIDES[i].bmpOrigin.Clone();

            //    i++;
            //}
            //MESSAGEFrm = new MessageForm(SCREEN.Messages("msg10"));
            //MESSAGEFrm.Show();
            //MESSAGEFrm.Refresh();

            int PreviousID = ID;

            //btnRecipeSelection.Enabled = false;
            CopyID(ID);

            NAME = "NEW" + ID.ToString();

            CREATEDATETIME = JzTimes.DateTimeString;// TimerClass.DateTimeString;
            MODIFYDATETIME = JzTimes.DateTimeString; //TimerClass.DateTimeString;

            KEYBOARD.ID = ID;
            KEYBOARD.Name = NAME;

            FillDisplay();

            if (!Directory.Exists(Universal.PICPATH + ID.ToString("0000")))
            {
                Directory.CreateDirectory(Universal.PICPATH + ID.ToString("0000"));
            }
            else
            {
                Directory.Delete(Universal.PICPATH + ID.ToString("0000"),true);
                Directory.CreateDirectory(Universal.PICPATH + ID.ToString("0000"));
            }

            //if (MessageBox.Show("是否要更新圖示?", "MAIN", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //{
            //    IsRenewOrg = true;

            //    i = 0;
            //    while (i < (int)SideEnum.COUNT)
            //    {
            //        CCD.Snap((SideEnum)i);
            //        Bitmap bmp = new Bitmap(CCD.GetBMP((SideEnum)i));

            //        if (!IsDebug)
            //            CCD.ClearBMP((SideEnum)i);

            //        KEYBOARD.GetBaseBMP((SideEnum)i, bmp);
            //        KEYBOARD.GetBaseBMP((SideEnum)i, Universal.PICPATH + ID.ToString("0000") + "\\TB" + i.ToString("000") + ".BMP");
            //        KEYBOARD.ClearBaseBMP((SideEnum)i);

            //        bmp.Dispose();
            //        i++;
            //    }
            //}


            bmpKeyboard.Save(Universal.PICPATH + ID.ToString("0000") + @"\KB.BMP", ImageFormat.Bmp);

            i = 0;
            while (i < INI.SIDECOUNT)
            {
                if (!IsRenewOrg)
                    File.Copy(Universal.PICPATH + PreviousID.ToString("0000") + "\\B" + i.ToString("000") + ".BMP", Universal.PICPATH + ID.ToString("0000") + "\\TB" + i.ToString("000") + ".BMP", true);

                File.Copy(Universal.PICPATH + PreviousID.ToString("0000") + "\\A" + i.ToString("000") + ".BMP", Universal.PICPATH + ID.ToString("0000") + "\\TA" + i.ToString("000") + ".BMP", true);
                File.Copy(Universal.PICPATH + PreviousID.ToString("0000") + "\\S" + i.ToString("000") + ".BMP", Universal.PICPATH + ID.ToString("0000") + "\\TS" + i.ToString("000") + ".BMP", true);


                KEYBOARD.SIDES[i].IsRegetAlready[(int)TeachingTypeEnum.BASE] = true;
                KEYBOARD.SIDES[i].IsRegetAlready[(int)TeachingTypeEnum.ANALYZE] = true;
                KEYBOARD.SIDES[i].IsRegetAlready[(int)TeachingTypeEnum.BACKGROUD] = true;

                i++;
            }
            
            RECIPEDBUI.DBStatus = DBStatusEnum.COPY;
            OnResizeKeyboardRange(true);

            //MESSAGEFrm.Close();
            //MESSAGEFrm.Dispose();
        }
        void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteID(ID);
        }
        void btnModify_Click(object sender, EventArgs e)
        {
            //PLC.SetZAutoSpeed(Universal.LowSpeed);

            int i = 0, j = 0;

            while (j < INI.SIDECOUNT)
            {
                SideClass side = KEYBOARD.SIDES[j];

                i = 0;
                while (i < (int)TeachingTypeEnum.COUNT)
                {
                    side.IsReget[i] = false;
                    side.IsRegetAlready[i] = false;

                    i++;
                }
                j++;
            }


            //btnRecipeSelection.Enabled = false;
            Modify();

            RECIPEDBUI.DBStatus = DBStatusEnum.MODIFY;

            RECIPEDBUI.txtName.Enabled = ID != 1;
            RECIPEDBUI.txtVersion.Enabled = ID != 1;

            //GetSideBMP();
            
            OnResizeKeyboardRange(true);

        }
        void btnAdd_Click(object sender, EventArgs e)
        {
            
        }

        public delegate void ResizeKeyboardRangeHandler(bool On);
        public event ResizeKeyboardRangeHandler ResizeKeyboardRangeAction;
        public void OnResizeKeyboardRange(bool On)
        {
            if (ResizeKeyboardRangeAction != null)
            {
                ResizeKeyboardRangeAction(On);
            }
        }
 
    }
}
