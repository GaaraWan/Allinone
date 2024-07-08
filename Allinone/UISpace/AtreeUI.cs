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
using Allinone.OPSpace;
using Allinone.FormSpace;
using BrightIdeasSoftware;

namespace Allinone.UISpace
{
    public partial class AtreeUI : UserControl
    {
        enum TagEnum
        {
            ADDSAMELEVEL,
            ADDBRANCHLEVEL,
            DUP,
            DEL,
            
            COMBINE,
            EDITDETAIL,
            EXPAND,
            COLLAPSE,

            TEST,

            CHECKLEARN,
            DELLEARN,
        }

        VersionEnum VERSION = VersionEnum.KBAOI;
        OptionEnum OPTION = OptionEnum.MAIN;

        List<AnalyzeClass> RawAnalyzeList;

        AnalyzeClass AnalyzeOperateNow
        {
            get
            {
                return RawAnalyzeList.Find(x => x.No == FirstSelectNo);
            }
        }

        AnalyzeClass AnalyzeRoot
        {
            get
            {
                return RawAnalyzeList.Find(x => x.No == 1);
            }
        }
        AnalyzeClass LearnAnalyzeNow = null;

        AnalyzeClass GetAnalyzeFromTree(int no)
        {
            return RawAnalyzeList.Find(x => x.No == no);
        }
        
        Button btnAddSameLevel;
        Button btnAddNextLevel;
        Button btnDup;
        Button btnDel;

        Button btnCombine;
        Button btnEditDetail;
        Button btnExpand;
        Button btnCollapse;

        Button btnCheckLearn;
        Button btnDelLearn;
        
        //Button btnTest;

        DataTable AnalyzeTable = new DataTable();

        DataTreeListView dtlvAnalyze;
        DataGridView dgvAnalyze;
        JzTransparentPanel tpnlCover;

        Label lblInformation;

        ListBox lsbLearnAnalyze;
        bool IsNeedToChange = false;
        public int FirstSelectNo = -1;
        List<int> subnoSelectList = new List<int>();

        bool IsForward = false;         //把從圖過來和從物件直接操作分開來
        public bool IsLearn = false;           //是否在學習時操作
        
        /// <summary>
        /// 傳回第一個被選中的位置
        /// </summary>
        int FirstSelectIndex
        {
            get
            {
                //return AnalyzeList.FindIndex(x => x.No == FirstSelectNo);
                return GetIndexFromTree(FirstSelectNo);
            }
        }
        public AtreeUI()
        {
            InitializeComponent();
            InitialInternal();
        }
        void InitialInternal()
        {
            btnAddSameLevel = button2;
            btnAddNextLevel = button5;

            btnDel = button3;
            btnDup = button4;
            //btnTest = button1;

            btnCombine = button9;
            btnEditDetail = button10;
            btnExpand = button1;
            btnCollapse = button6;

            btnCheckLearn = button7;
            btnDelLearn = button8;

            tpnlCover = new JzTransparentPanel();
            tpnlCover.BackColor = System.Drawing.Color.Transparent;
            tpnlCover.Location = new System.Drawing.Point(6, 30);
            tpnlCover.Name = "panel1";
            tpnlCover.Size = this.Size;
            tpnlCover.TabIndex = 0;
            this.Controls.Add(tpnlCover);
            tpnlCover.BringToFront();
            
            btnAddSameLevel.Tag = TagEnum.ADDSAMELEVEL;
            btnAddNextLevel.Tag = TagEnum.ADDBRANCHLEVEL;
            btnDel.Tag = TagEnum.DEL;
            btnDup.Tag = TagEnum.DUP;
            //btnTest.Tag = TagEnum.TEST;
            btnCombine.Tag = TagEnum.COMBINE;
            btnEditDetail.Tag = TagEnum.EDITDETAIL;
            btnExpand.Tag = TagEnum.EXPAND;
            btnCollapse.Tag = TagEnum.COLLAPSE;
            btnCheckLearn.Tag = TagEnum.CHECKLEARN;
            btnDelLearn.Tag = TagEnum.DELLEARN;

            btnAddSameLevel.Click += btn_Click;
            btnAddNextLevel.Click += btn_Click;
            btnDel.Click += btn_Click;
            btnDup.Click += btn_Click;

            btnCombine.Click += btn_Click;
            btnEditDetail.Click += btn_Click;
            btnExpand.Click += btn_Click;
            btnCollapse.Click += btn_Click;

            btnCheckLearn.Click += btn_Click;
            btnDelLearn.Click += btn_Click;

            //不是Learn時為不能用
            btnDelLearn.Enabled = false;

            //btnTest.Click += btn_Click;

            dgvAnalyze = dataGridView2;
            dtlvAnalyze = dataTreeListViewNew;
            tpnlCover.Location = dtlvAnalyze.Location;
            tpnlCover.Size = dtlvAnalyze.Size;
            tpnlCover.Visible = false;

            lblInformation = label2;
            lblInformation.Text = "";
            dgvAnalyze.Location = new Point(1000, 0);
            
            dtlvAnalyze.RootKeyValue = 0u;
            dtlvAnalyze.SelectedIndexChanged += dtlvAnalyze_SelectedIndexChanged;
            dtlvAnalyze.SelectionChanged += dtlvAnalyze_SelectionChanged;       //全選擇完後會趨動此步驟
            
            lsbLearnAnalyze = listBox1;
            lsbLearnAnalyze.SelectedIndexChanged += lsbLearnAnalyze_SelectedIndexChanged;
            lsbLearnAnalyze.Enabled = false;
        }

        public void Initial(VersionEnum version, OptionEnum opt)
        {
            VERSION = version;
            OPTION = opt;

        }

        #region Normal Operation
        /// <summary>
        /// 新增同階資料
        /// </summary>
        public void AddSameLevel()
        {
            if (RawAnalyzeList.Count == 1)
            {
                AddBranchLevel();
                return;
            }
            List<AnalyzeClass> sameaddedanalyzelist = new List<AnalyzeClass>();
            List<int> selectanalyzenolist = new List<int>();

            if (FirstSelectNo > -1)
            {
                JzToolsClass.myShowCursor(0);
                List<int> selectnolist = CheckUISelect(FirstSelectNo);
                
                foreach (int no in selectnolist)
                {
                    if (no == 1)    //排除ROOT
                        continue;

                    AnalyzeClass selectanalyze = GetAnalyzeFromTree(no);

                    AnalyzeClass sameanalyze = selectanalyze.Clone(new Point(100, 100), 0d, true, false, false, false);
                    
                    sameanalyze.No = GetMaxNewNoFromRawAnalyzeList();
                    sameanalyze.AliasName = sameanalyze.ToAnalyzeString();
                    sameanalyze.RelateMover(sameanalyze.No, sameanalyze.Level);
                    sameanalyze.RelateASN = "None";
                    sameanalyze.RelateASNItem = "None";
                    
                    sameanalyze.ToPassInfo();

                    //AddRowToAnalyzeTable(sameanalyze);
                    sameanalyze.AddNewRowToDataTable(AnalyzeTable, IsLearn);
                    RawAnalyzeList.Add(sameanalyze);

                    sameaddedanalyzelist.Add(sameanalyze);
                    selectanalyzenolist.Add(sameanalyze.No);

                    AnalyzeClass parentanalyze = GetAnalyzeFromTree(sameanalyze.ParentNo);
                    parentanalyze.BranchList.Add(sameanalyze);
                }
                //FirstSelectNo = branchanalyze.No;

                OnChangeMover(sameaddedanalyzelist, DBStatusEnum.ADD);

                dtlvAnalyze.ExpandAll();

                SetSelectFocus(selectanalyzenolist);
                JzToolsClass.myShowCursor(1);
            }
            else
                MessageBox.Show("請選擇需要新增同層的項目", "SYSTEM", MessageBoxButtons.OK);
        }

        /// <summary>
        /// 自動新增同階資料
        /// </summary>
        public void AddSameLevel(int selectno,DoffsetClass doffset,PointF orgcenterf)
        {
            List<AnalyzeClass> sameaddedanalyzelist = new List<AnalyzeClass>();
            List<int> selectanalyzenolist = new List<int>();

            if (FirstSelectNo > -1)
            {
                List<int> selectnolist = CheckUISelect(FirstSelectNo);

                //foreach (int no in selectnolist)
                {
                //    if (no == 1)    //排除ROOT
                //        continue;

                    AnalyzeClass selectanalyze = GetAnalyzeFromTree(selectno);

                    PointF offsetptf = new PointF(doffset.OffsetF.X - orgcenterf.X, doffset.OffsetF.Y - orgcenterf.Y);

                    AnalyzeClass sameanalyze = selectanalyze.Clone(new Point((int)offsetptf.X, (int)offsetptf.Y), (double)doffset.Degree, true, false, false, false);
                   
                    sameanalyze.No = GetMaxNewNoFromRawAnalyzeList();
                    sameanalyze.AliasName = sameanalyze.ToAnalyzeString();
                    sameanalyze.RelateMover(sameanalyze.No, sameanalyze.Level);
                    sameanalyze.RelateASN = "None";
                    sameanalyze.RelateASNItem = "None";
                    
                    sameanalyze.ToPassInfo();

                    //AddRowToAnalyzeTable(sameanalyze);
                    sameanalyze.AddNewRowToDataTable(AnalyzeTable, IsLearn);
                    RawAnalyzeList.Add(sameanalyze);

                    sameaddedanalyzelist.Add(sameanalyze);
                    selectanalyzenolist.Add(sameanalyze.No);

                    AnalyzeClass parentanalyze = GetAnalyzeFromTree(sameanalyze.ParentNo);
                    parentanalyze.BranchList.Add(sameanalyze);
                }
                //FirstSelectNo = branchanalyze.No;
                
                OnChangeMover(sameaddedanalyzelist, DBStatusEnum.ADD);

                //dtlvAnalyze.ExpandAll();

                SetSelectFocus(selectanalyzenolist);
            }
            else
                MessageBox.Show("請選擇需要新增同層的項目", "SYSTEM", MessageBoxButtons.OK);
        }

        /// <summary>
        /// 新增分支資料
        /// </summary>
        public void AddBranchLevel()
        {
            List<AnalyzeClass> nextaddedanalyzelist = new List<AnalyzeClass>();
            List<int> selectanalyzenolist = new List<int>();

            if (FirstSelectNo > -1 || RawAnalyzeList.Count == 1)
            {
                JzToolsClass.myShowCursor(0);

                List<int> selectnolist = new List<int>();

                if (RawAnalyzeList.Count == 1)
                    selectnolist.Add(1);
                else
                    selectnolist = CheckUISelect(FirstSelectNo);

                foreach (int no in selectnolist)
                {
                    AnalyzeClass selectanalyze = GetAnalyzeFromTree(no);

                    AnalyzeClass branchanalyze =  selectanalyze.Clone(new Point(100, 100), 0d, true, false, false, false);
                 
                    if (RawAnalyzeList.Count == 1)
                    {
                        branchanalyze.SetMoverDefault();
                    }
                    
                    branchanalyze.ParentNo = selectanalyze.No;
                    branchanalyze.FromNodeString = selectanalyze.ToNextNodeString();

                    branchanalyze.No = GetMaxNewNoFromRawAnalyzeList();

                    branchanalyze.Level++;
                    branchanalyze.RelateMover(branchanalyze.No, branchanalyze.Level);
                    branchanalyze.AliasName = branchanalyze.ToAnalyzeString();
                    branchanalyze.RelateASN = "None";
                    branchanalyze.RelateASNItem = "None";
                    
                    branchanalyze.ToPassInfo();
                    
                    branchanalyze.FromAssembleProperty("RESET");

                    //AddRowToAnalyzeTable(branchanalyze);
                    branchanalyze.AddNewRowToDataTable(AnalyzeTable, IsLearn);
                    RawAnalyzeList.Add(branchanalyze);

                    nextaddedanalyzelist.Add(branchanalyze);
                    selectanalyzenolist.Add(branchanalyze.No);

                    selectanalyze.BranchList.Add(branchanalyze);
                }
                //FirstSelectNo = branchanalyze.No;

                OnChangeMover(nextaddedanalyzelist, DBStatusEnum.ADD);

                dtlvAnalyze.ExpandAll();

                SetSelectFocus(selectanalyzenolist);

                JzToolsClass.myShowCursor(1);
            }
            else
            {
                MessageBox.Show("請選擇需要新增分支的項目", "SYSTEM", MessageBoxButtons.OK);
            }
        }
        /// <summary>
        /// 自動新增分支資料
        /// </summary>
        public void AddBranchLevel(int analyzeno, Rectangle branchrect, int extend)
        {
            List<AnalyzeClass> nextaddedanalyzelist = new List<AnalyzeClass>();
            List<int> selectanalyzenolist = new List<int>();

            //if (FirstSelectNo > -1)
            {
                //List<int> selectnolist = CheckUISelect(FirstSelectNo);

                //foreach (int no in selectnolist)
                {
                    AnalyzeClass selectanalyze = GetAnalyzeFromTree(analyzeno);

                    //AnalyzeClass branchanalyze = selectanalyze.Clone(new Point(100, 100), 0d, true, false, false, false);

                    AnalyzeClass branchanalyze = new AnalyzeClass(branchrect);
                    
                    branchanalyze.ParentNo = selectanalyze.No;
                    branchanalyze.FromNodeString = selectanalyze.ToNextNodeString();

                    branchanalyze.PageNo = selectanalyze.PageNo;
                    branchanalyze.PageOPtype = selectanalyze.PageOPtype;
                    branchanalyze.AnalyzeType = AnalyzeTypeEnum.BRANCH;

                    branchanalyze.No = GetMaxNewNoFromRawAnalyzeList();
                    branchanalyze.Level = selectanalyze.Level + 1;
                    branchanalyze.RelateMover(branchanalyze.No, branchanalyze.Level);
                    branchanalyze.AliasName = branchanalyze.ToAnalyzeString();
                    branchanalyze.RelateASN = "None";
                    branchanalyze.RelateASNItem = "None";

                    branchanalyze.ToPassInfo();

                    branchanalyze.NORMALPara.ExtendX = extend;
                    branchanalyze.NORMALPara.ExtendY = extend;

                    branchanalyze.ALIGNPara.AlignMethod = AlignMethodEnum.AUFIND;
                    branchanalyze.ALIGNPara.AlignMode = AlignModeEnum.AREA;
                    branchanalyze.INSPECTIONPara.InspectionMethod = InspectionMethodEnum.Equalize;
                    branchanalyze.INSPECTIONPara.InspectionAB = Inspection_A_B_Enum.Histogram;

                    //AddRowToAnalyzeTable(branchanalyze);
                    branchanalyze.AddNewRowToDataTable(AnalyzeTable, IsLearn);
                    RawAnalyzeList.Add(branchanalyze);

                    nextaddedanalyzelist.Add(branchanalyze);
                    //selectanalyzenolist.Add(branchanalyze.No);

                    selectanalyze.BranchList.Add(branchanalyze);
                }
                //FirstSelectNo = branchanalyze.No;

                OnChangeMover(nextaddedanalyzelist, DBStatusEnum.ADD);

                dtlvAnalyze.ExpandAll();

                //SetSelectFocus(selectanalyzenolist);
            }
        }
        /// <summary>
        /// 自動新增分支資料 带角度的新增
        /// </summary>
        public void AddBranchLevel(int analyzeno, RectangleF branchrect, int extend,double angle, AnalyzeClass analyzeBase=null)
        {
            List<AnalyzeClass> nextaddedanalyzelist = new List<AnalyzeClass>();
            List<int> selectanalyzenolist = new List<int>();

            //if (FirstSelectNo > -1)
            {
                //List<int> selectnolist = CheckUISelect(FirstSelectNo);

                //foreach (int no in selectnolist)
                {
                    AnalyzeClass selectanalyze = GetAnalyzeFromTree(analyzeno);

                    //AnalyzeClass brancha0nalyze = selectanalyze.Clone(new Point(100, 100), 0d, true, false, false, false);

                    AnalyzeClass branchanalyze = new AnalyzeClass(branchrect, angle);

                    if (analyzeBase != null)
                    {
                        //branchanalyze.Level = analyzeBase.Level;

                        branchanalyze.NORMALPara.ExtendX = analyzeBase.NORMALPara.ExtendX;
                        branchanalyze.NORMALPara.ExtendY = analyzeBase.NORMALPara.ExtendY;

                        //branchanalyze.NORMALPara.FromString(analyzeBase.NORMALPara.ToString());
                        branchanalyze.ALIGNPara.FromString(analyzeBase.ALIGNPara.ToString());
                        branchanalyze.INSPECTIONPara.FromString(analyzeBase.INSPECTIONPara.ToString());
                        branchanalyze.AOIPara.FromString(analyzeBase.AOIPara.ToString());
                        branchanalyze.OCRPara.FromString(analyzeBase.OCRPara.ToString());
                    }
                    else
                    {
                        //branchanalyze.Level = selectanalyze.Level + 1;

                        branchanalyze.NORMALPara.ExtendX = extend;
                        branchanalyze.NORMALPara.ExtendY = extend;

                        branchanalyze.ALIGNPara.AlignMethod = AlignMethodEnum.AUFIND;
                        branchanalyze.ALIGNPara.AlignMode = AlignModeEnum.AREA;
                        branchanalyze.ALIGNPara.MTOffset = 0f;
                        branchanalyze.ALIGNPara.MTResolution = 0.038f;
                        branchanalyze.ALIGNPara.MTTolerance = 0.2f;

                        branchanalyze.INSPECTIONPara.InspectionMethod = InspectionMethodEnum.Equalize;
                        branchanalyze.INSPECTIONPara.IBArea = 10;
                        branchanalyze.INSPECTIONPara.IBCount = 20;
                        branchanalyze.INSPECTIONPara.IBTolerance = 25;
                        branchanalyze.INSPECTIONPara.InspectionAB = Inspection_A_B_Enum.Histogram;
                    }

                    branchanalyze.ParentNo = selectanalyze.No;
                    branchanalyze.FromNodeString = selectanalyze.ToNextNodeString();

                    branchanalyze.PageNo = selectanalyze.PageNo;
                    branchanalyze.PageOPtype = selectanalyze.PageOPtype;
                    branchanalyze.AnalyzeType = AnalyzeTypeEnum.BRANCH;

                    branchanalyze.No = GetMaxNewNoFromRawAnalyzeList();
                    branchanalyze.Level = selectanalyze.Level + 1;
                    branchanalyze.RelateMover(branchanalyze.No, branchanalyze.Level);
                    branchanalyze.AliasName = branchanalyze.ToAnalyzeString();
                    branchanalyze.RelateASN = "None";
                    branchanalyze.RelateASNItem = "None";

                    branchanalyze.ToPassInfo();

                   

                    //AddRowToAnalyzeTable(branchanalyze);
                    branchanalyze.AddNewRowToDataTable(AnalyzeTable, IsLearn);
                    RawAnalyzeList.Add(branchanalyze);

                    nextaddedanalyzelist.Add(branchanalyze);
                    //selectanalyzenolist.Add(branchanalyze.No);

                    selectanalyze.BranchList.Add(branchanalyze);
                }
                //FirstSelectNo = branchanalyze.No;

                OnChangeMover(nextaddedanalyzelist, DBStatusEnum.ADD);
                
                //Modified By Victor Tsai 2024/06/09
                //dtlvAnalyze.ExpandAll();

                //SetSelectFocus(selectanalyzenolist);
            }
        }

        //Modified By Victor Tsai 2024/06/09
        //No Start Update
        public void DismissAnalyzeTable()
        {
            int i = 0;

            i = 1;

            dtlvAnalyze.CollapseAll();

            dtlvAnalyze.BeginUpdate() ;
            IsNeedToChange = false;
        }

        //Modified By Victor Tsai 2024/06/09
        //End Start Update
       public void RelateAnalyzeTable()
        {
            dtlvAnalyze.EndUpdate();
            dtlvAnalyze.ExpandAll();

            IsNeedToChange = true;
        }


        /// <summary>
        /// 刪除選定含分支的資料
        /// </summary>
        public void Delete()
        {
            //if (MessageBox.Show("是否要刪除所選項目", "SYSTEM", MessageBoxButtons.YesNo) == DialogResult.No)
            //    return;
            
            int i = 0;

            List<AnalyzeClass> deleteanalyzelist = new List<AnalyzeClass>();
            List<int> deletenolist = new List<int>();

            if (FirstSelectNo > -1)
            {
                JzToolsClass.myShowCursor(0);

                List<int> selectnolist = CheckUISelect(FirstSelectNo);
                
                foreach (int no in selectnolist)
                {
                    if (no == 1)    //排除ROOT
                        continue;

                    foreach(AnalyzeClass analyze in RawAnalyzeList)
                    {
                        if(analyze.ParentNo == no || analyze.No == no)
                        {
                            analyze.InsertDeleteData(deletenolist, deleteanalyzelist);
                        }
                    }
                }

                //Modified By Vitor Tsai 2024/06/09
                DismissAnalyzeTable();
                i = AnalyzeTable.Rows.Count - 1;
                while(i > -1)
                {
                    if(deletenolist.IndexOf((int)(UInt32)AnalyzeTable.Rows[i]["No"]) > -1)
                    {
                        //AnalyzeTable.Rows.RemoveAt(i);
                        AnalyzeTable.Rows[i].Delete();

                    }
                    i--;
                }

                AnalyzeTable.AcceptChanges();

                OnChangeMover(deleteanalyzelist, DBStatusEnum.DELETE);
                
                i = RawAnalyzeList.Count - 1;
                while(i > -1)
                {
                    AnalyzeClass analyze = RawAnalyzeList[i];

                    analyze.DeleteBranch(deletenolist);

                    if(deletenolist.IndexOf(analyze.No) > -1)
                    {
                        analyze.Suicide();
                        RawAnalyzeList.RemoveAt(i);
                    }
                    i--;
                }

                deletenolist.Clear();
                SetSelectFocus(deletenolist);

                //Modified By Vitor Tsai 2024/06/09
                RelateAnalyzeTable();


                JzToolsClass.myShowCursor(1);
            }
            else
                MessageBox.Show("請選擇需要刪除的項目", "SYSTEM", MessageBoxButtons.OK);

        }

        /// <summary>
        /// 刪除 选定好的资料
        /// </summary>
        public void Delete(List<int> selectnolist, bool eSelectFocus = true)
        {
            //if (MessageBox.Show("是否要刪除所選項目", "SYSTEM", MessageBoxButtons.YesNo) == DialogResult.No)
            //    return;

            if (selectnolist == null)
                return;
            if (selectnolist.Count == 0)
                return;

            int i = 0;

            List<AnalyzeClass> deleteanalyzelist = new List<AnalyzeClass>();
            List<int> deletenolist = new List<int>();

            if (FirstSelectNo > -1)
            {
                //Modified By Vitor Tsai 2024/06/09
                DismissAnalyzeTable();

                JzToolsClass.myShowCursor(0);

                //List<int> selectnolist = CheckUISelect(FirstSelectNo);

                foreach (int no in selectnolist)
                {
                    if (no == 1)    //排除ROOT
                        continue;

                    foreach (AnalyzeClass analyze in RawAnalyzeList)
                    {
                        if (analyze.ParentNo == no || analyze.No == no)
                        {
                            analyze.InsertDeleteData(deletenolist, deleteanalyzelist);
                        }
                    }
                }

                i = AnalyzeTable.Rows.Count - 1;
                while (i > -1)
                {
                    if (deletenolist.IndexOf((int)(UInt32)AnalyzeTable.Rows[i]["No"]) > -1)
                        AnalyzeTable.Rows.RemoveAt(i);

                    i--;
                }

                OnChangeMover(deleteanalyzelist, DBStatusEnum.DELETE);

                i = RawAnalyzeList.Count - 1;
                while (i > -1)
                {
                    AnalyzeClass analyze = RawAnalyzeList[i];

                    analyze.DeleteBranch(deletenolist);

                    if (deletenolist.IndexOf(analyze.No) > -1)
                    {
                        analyze.Suicide();
                        RawAnalyzeList.RemoveAt(i);
                    }
                    i--;
                }

                deletenolist.Clear();
                if (eSelectFocus)
                    SetSelectFocus(deletenolist);


                JzToolsClass.myShowCursor(1);

                //Modified By Vitor Tsai 2024/06/09
                RelateAnalyzeTable();
            }
            else
                MessageBox.Show("請選擇需要刪除的項目", "SYSTEM", MessageBoxButtons.OK);

        }

        /// <summary>
        /// 複製整體資料
        /// </summary>
        public void Dup()
        {
            List<AnalyzeClass> dupanalyzelist = new List<AnalyzeClass>();
            List<int> dupanalyzenolist = new List<int>();

            if (FirstSelectNo > -1)
            {
                JzToolsClass.myShowCursor(0);
                List<int> selectnolist = CheckUISelect(FirstSelectNo);

                foreach (int no in selectnolist)
                {
                    if (no == 1)    //排除ROOT
                        continue;

                    AnalyzeClass selectanalyze = GetAnalyzeFromTree(no);

                    selectanalyze.SetMoverSelected(false, false);
                    AnalyzeClass dupanalyze = selectanalyze.Clone(new Point(100, 100), 0d, true, true, false, false);
                    //將所有的Analyze 包含 Branch設定為 Select
                    dupanalyze.SetMoverSelected(true, true);
                    dupanalyze.SetBranchMoverSelected(true);

                    AnalyzeClass.DupMaxNo = GetMaxNewNoFromRawAnalyzeList();

                    //sameanalyze.RelateMover(sameanalyze.No, sameanalyze.Level);

                    dupanalyze.InsertDupData(dupanalyzenolist, dupanalyzelist, RawAnalyzeList, AnalyzeTable,IsLearn);
                    
                    //RawAnalyzeList.Add(dupanalyze);
                    //AddRowToAnalyzeTable(dupanalyze);
                    //dupanalyzelist.Add(dupanalyze);
                    //dupanalyzenolist.Add(dupanalyze.No);
                    //selectanalyze.BranchList.Add(dupanalyze);

                    AnalyzeClass parentanalyze = GetAnalyzeFromTree(dupanalyze.ParentNo);
                    parentanalyze.BranchList.Add(dupanalyze);
                }
                //FirstSelectNo = branchanalyze.No;

                OnChangeMover(dupanalyzelist, DBStatusEnum.ADD);

                dtlvAnalyze.ExpandAll();

                SetSelectFocus(dupanalyzenolist);
                JzToolsClass.myShowCursor(1);
            }
            else
                MessageBox.Show("請選擇需要複製的項目", "SYSTEM", MessageBoxButtons.OK);


        }
        /// <summary>
        /// 設定ListView
        /// </summary>
        /// <param name="analyzelist"></param>
        public void SetAnalyzeTree(List<AnalyzeClass> analyzelist)
        {
            //Parent 要設定為 0 才會停在根目錄
            
            if(analyzelist != null)
                RawAnalyzeList = analyzelist;

            if (AnalyzeTable.Columns.Count < 1)
            {
                AnalyzeTable.Columns.Clear();
                DataColumn id = new DataColumn("No", typeof(UInt32));               //一定要用UInt32
                DataColumn parentid = new DataColumn("ParentNo", typeof(UInt32));    //一定要用UInt32
                DataColumn analyzename = new DataColumn("Name", typeof(String));
                DataColumn no = new DataColumn("NO", typeof(UInt32));
                DataColumn level = new DataColumn("LV", typeof(UInt32));
                DataColumn learncount = new DataColumn("LN", typeof(UInt32));

                AnalyzeTable.Columns.Add(id);
                AnalyzeTable.Columns.Add(parentid);
                AnalyzeTable.Columns.Add(analyzename);
                AnalyzeTable.Columns.Add(no);
                AnalyzeTable.Columns.Add(level);
                AnalyzeTable.Columns.Add(learncount);
            }
            AnalyzeTable.Clear();
            foreach (AnalyzeClass analyze in RawAnalyzeList)
            {
                //if (analyze.AnalyzeType == AnalyzeTypeEnum.BRANCH)
                {
                    //AddRowToAnalyzeTable(analyze);
                    analyze.AddNewRowToDataTable(AnalyzeTable, IsLearn);
                    //DataRow newdatarow = AnalyzeTable.NewRow();
                    //newdatarow["No"] = analyze.No;
                    //newdatarow["ParentNo"] = analyze.ParentNo;
                    //newdatarow["Name"] = analyze.ToAnalyzeString();
                    //newdatarow["Level"] = analyze.Level;
                    //AnalyzeTable.Rows.Add(newdatarow);
                }
            }

            //Modified By Victor Tsai 2024/06/09
            //dgvAnalyze.DataSource = AnalyzeTable;
            //dgvAnalyze.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dtlvAnalyze.BeginUpdate();

            dtlvAnalyze.AutoGenerateColumns = true;
            dtlvAnalyze.ShowKeyColumns = false;
            dtlvAnalyze.ParentKeyAspectName = "ParentNo";
            dtlvAnalyze.KeyAspectName = "No";
            dtlvAnalyze.HeaderUsesThemes = false;
            dtlvAnalyze.UseFilterIndicator = false;
            dtlvAnalyze.UseFiltering = false;
            dtlvAnalyze.FullRowSelect = true;
            dtlvAnalyze.MultiSelect = true;
            dtlvAnalyze.HideSelection = false;

            dtlvAnalyze.DataSource = AnalyzeTable;

            dtlvAnalyze.Columns[0].Width = 190;
            dtlvAnalyze.Columns[1].Width = 30;
            dtlvAnalyze.Columns[2].Width = 30;
            dtlvAnalyze.Columns[3].Width = 30;

            dtlvAnalyze.EndUpdate();

            dtlvAnalyze.ExpandAll();

            IsNeedToChange = true;

            IsForward = true;
            FirstSelectNo = -1;
            dtlvAnalyze.SelectedIndex = -1;
            IsForward = false;
        }
        ///// <summary>
        ///// 新增新的Row於Table中，就可組成tree
        ///// </summary>
        ///// <param name="analyze"></param>
        //void AddRowToAnalyzeTable(AnalyzeClass analyze)
        //{
        //    DataRow newdatarow = AnalyzeTable.NewRow();

        //    newdatarow["No"] = analyze.No;
        //    newdatarow["ParentNo"] = analyze.ParentNo;
        //    newdatarow["Name"] = analyze.ToAnalyzeString();
        //    newdatarow["Level"] = analyze.Level;

        //    AnalyzeTable.Rows.Add(newdatarow);
        //}
        /// <summary>
        /// 從圖形選擇轉換到ListView選擇
        /// </summary>
        /// <param name="selectanalyzenolist"></param>
        public void SetSelectFocus(List<int> selectanalyzenolist)
        {
            IsForward = true;

            if (selectanalyzenolist.Count == 0)
            {
                FirstSelectNo = -1;

                //if(dtlvAnalyze.SelectedIndex != -1)
                dtlvAnalyze.SelectedIndex = -1;
            }
            else
            {
                int firstno = -1;
                string Str = "";

                foreach (int no in selectanalyzenolist)
                {
                    Str += no.ToString("00") + ",";

                    //int foundindex = AnalyzeList.FindIndex(x => x.No == no);
                    int foundindex = GetIndexFromTree(no);

                    if (firstno == -1)
                    {
                        firstno = no;
                        FirstSelectNo = no;

                        IsForward = true;
                        dtlvAnalyze.SelectedIndex = FirstSelectIndex;
                    }

                    IsNeedToChange = false;
                    dtlvAnalyze.Items[foundindex].Selected = true;
                    IsNeedToChange = true;
                }
                Str = Str.Remove(Str.Length - 1, 1);
                //lblInformation.Text = Str;
            }

            //lblInformation.Invalidate();

            IsForward = false;  //Forawrd and Backward Should be Reset
        }
        /// <summary>
        /// 圖形選擇轉換到包含sub的ListView選擇
        /// </summary>
        /// <param name="selectanalyzestringlist"></param>
        public void SetSelectFocus(List<string> selectanalyzestringlist)
        {
            int i = 0;

            IsForward = true;
            subnoSelectList.Clear();

            if (selectanalyzestringlist.Count == 0)
            {
                FirstSelectNo = -1;
                //if(dtlvAnalyze.SelectedIndex != -1)
                dtlvAnalyze.SelectedIndex = -1;

                //OnChange(FirstSelectNo);
            }
            else
            {
                int firstno = -1;
                string Str = "";
                List<int> realselectindexlist = new List<int>();

                IsNeedToChange = false;

                if (selectanalyzestringlist.Count > 1)
                    Str = Str;

                foreach (string selstr in selectanalyzestringlist)
                {
                    Str += selstr + ",";

                    string[] strs = selstr.Split(':');

                    int selectno = int.Parse(strs[0]);
                    //int foundindex = AnalyzeList.FindIndex(x => x.No == no);
                    int foundindex = GetIndexFromTree(selectno);
                    
                    dtlvAnalyze.Items[foundindex].Selected = true;
                    realselectindexlist.Add(foundindex);

                    //if (firstno == -1)
                    if(FirstSelectNo != selectno && subnoSelectList.Count == 0)
                    {
                        firstno = selectno;
                        FirstSelectNo = selectno;

                        subnoSelectList.Add(int.Parse(strs[1]));

                        //OnChange(FirstSelectNo);
                    }
                    else if(selectno == FirstSelectNo)
                    {
                        subnoSelectList.Add(int.Parse(strs[1]));

                        //OnChange(FirstSelectNo);
                    }
                }

                i = 0;

                while(i < dtlvAnalyze.Items.Count)
                {
                    if(realselectindexlist.IndexOf(i) < 0)
                    {
                        dtlvAnalyze.Items[i].Selected = false;
                    }
                    i++;
                }

                //OnChange(FirstSelectNo);

                IsNeedToChange = true;

                Str = Str.Remove(Str.Length - 1, 1);
                //lblInformation.Text = Str;

                //IsForward = true;
                //dtlvAnalyze.SelectedIndex = FirstSelectIndex;
            }

            //lblInformation.Invalidate();

            //IsForward = false;  //Forawrd and Backward Should be Reset
        }

        public void SetLearn()
        {
            IsLearn = true;
            lsbLearnAnalyze.Enabled = true;

            btnDelLearn.Enabled = true;
        }

        void FillDisplay()
        {
            if (IsLearn)
                return;

            IsNeedToChange = false;

            lsbLearnAnalyze.Items.Clear();

            int i = -1;

            if (FirstSelectNo > -1)
            {
                if (AnalyzeOperateNow != null)
                {
                    while (i < AnalyzeOperateNow.LearnList.Count)
                    {
                        if (i == -1)
                            lsbLearnAnalyze.Items.Add("LRN" + "-" + AnalyzeOperateNow.No.ToString("000") + "-" + (i + 1).ToString("00"));
                        else
                            lsbLearnAnalyze.Items.Add("LRN" + "-" + AnalyzeOperateNow.LearnList[i].No.ToString("000") + "-" + (i + 1).ToString("00"));

                        i++;
                    }
                }
            }

            IsNeedToChange = true;

            if (lsbLearnAnalyze.Items.Count > 0)
                lsbLearnAnalyze.SelectedIndex = 0;
        }
        public void FillLearnList(AnalyzeClass learnanalyzenow)
        {
            IsNeedToChange = false;

            LearnAnalyzeNow = learnanalyzenow;

            lsbLearnAnalyze.Items.Clear();

            int i = 0;

            while (i < LearnAnalyzeNow.LearnList.Count + 1)
            {
                lsbLearnAnalyze.Items.Add("LRN" + "-" + LearnAnalyzeNow.GetLearnByIndex(i).No.ToString("000") + "-" + (LearnAnalyzeNow.GetLearnByIndex(i).LearnNo).ToString("00"));
                i++;
            }

            if (lsbLearnAnalyze.Items.Count > 0)
                lsbLearnAnalyze.SelectedIndex = lsbLearnAnalyze.Items.Count - 1;

            IsNeedToChange = true;

        }


        /// <summary>
        /// 尋找編號在樹狀圖的順序
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        int GetIndexFromTree(int no)
        {
            int i = 0;
            int ret = 0;

            while(i < dtlvAnalyze.Items.Count)
            {
                string str = dtlvAnalyze.Items[i].Text;
                string[] strs = str.Split('-');

                int getno = int.Parse(strs[2]);

                if(getno == no)
                {
                    ret = i;
                    break;
                }

                i++;
            }

            return ret;
        }
        /// <summary>
        /// 尋找最大的編號
        /// </summary>
        /// <returns></returns>
        int GetMaxNewNoFromRawAnalyzeList()
        {
            int max = -1000;

            if (IsLearn)
            {
                max = AnalyzeClass.LearnMaxNo;

                AnalyzeClass.LearnMaxNo++;
            }
            else
            {
                foreach (AnalyzeClass analyze in RawAnalyzeList)
                {
                    if (max < analyze.No)
                    {
                        max = analyze.No;
                    }
                }
            }
            
            max++;
            return max;
        }
        /// <summary>
        /// 展開樹
        /// </summary>
        public void ExpandTree()
        {
            dtlvAnalyze.ExpandAll();
        }
        /// <summary>
        /// 合起樹
        /// </summary>
        public void CollapseTree()
        {
            dtlvAnalyze.CollapseAll();

        }
        public void Lock(bool islock)
        {
            tpnlCover.Visible = islock;
        }
        /// <summary>
        /// 檢查 Learning 的設定並且可以刪除
        /// </summary>
        public void CheckLearn()
        {
            if (AnalyzeOperateNow == null)
                return;
            if (AnalyzeOperateNow.LearnList.Count > 0)
            {
                int no = FirstSelectNo;

                OnGetOperate(AnalyzeOperateNow.No, AnalyzeOperateNow.LearnNo);
                OnLearnTune(AnalyzeOperateNow);

                //FirstSelectNo = no;
                //OnChange(FirstSelectNo);
                //FillDisplay();

                AnalyzeOperateNow.UpdateRowToDataTable(AnalyzeTable);

                IsForward = false;
                dtlvAnalyze_SelectionChanged();
            }
            else
                MessageBox.Show("無需要查看項目。", "SYSTEM", MessageBoxButtons.OK);
        }
        /// <summary>
        /// 刪除所選定的 Learning
        /// </summary>
        public void DelLearn()
        {
            if(lsbLearnAnalyze.SelectedIndex > 0)
            {
                if (MessageBox.Show("是否要刪除此學習?","SYSYTEM",MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int lastlearnindex = lsbLearnAnalyze.SelectedIndex;


                    LearnAnalyzeNow.LearnList.RemoveAt(lastlearnindex - 1);

                    IsNeedToChange = false;
                    lsbLearnAnalyze.Items.RemoveAt(lastlearnindex);
                    IsNeedToChange = true;

                    if (lastlearnindex == lsbLearnAnalyze.Items.Count)
                        lsbLearnAnalyze.SelectedIndex = lastlearnindex - 1;
                    else
                        lsbLearnAnalyze.SelectedIndex = lastlearnindex;
                }
            }
        }

        void Combine()
        {
            JzToolsClass.myShowCursor(0);

            int i = 0;
            int j = 0;

            int levelcount = 0;

            int insidecount = 0;


            int RawAnalyzeCount = RawAnalyzeList.Count;

            //先清除所有的List連結
            AnalyzeRoot.ClearBranchList();


            #region 整理 RawAnalyze 裏的資料
            //先全數填入第一層
            foreach(AnalyzeClass analyze in RawAnalyzeList)
            {
                if(analyze.No != 1)
                {
                    analyze.ParentNo = 1;
                    analyze.Level = 2;
                }
            }
            List<AnalyzeClass> checkanalyzelist = new List<AnalyzeClass>();
            levelcount = 2;
            while (levelcount < 100)
            {
                insidecount = 0;
                checkanalyzelist.Clear();

                //從LEVEL Count開始找
                foreach (AnalyzeClass analyze in RawAnalyzeList)
                {
                    if (analyze.Level == levelcount)
                    {
                        checkanalyzelist.Add(analyze);
                    }
                }

                i = 0;
                while (i < checkanalyzelist.Count)
                {
                    j = 0;
                    while (j < checkanalyzelist.Count)
                    {
                        if(j == i)
                        {
                            j++;
                            continue;
                        }

                        if(checkanalyzelist[j].CheckAnalyeInside(checkanalyzelist[i]))
                        {
                            insidecount++;
                            break;
                        }
                        j++;
                    }
                    i++;
                }

                if (insidecount == 0)
                    break;

                levelcount++;
            }
            #endregion

            #region 把 AnalyzeRoot 的 Branch 處理好

            foreach(AnalyzeClass analyze in RawAnalyzeList)
            {
                analyze.IsUsed = false;
            }

            

            while(true)
            {
                insidecount = 0;

                foreach(AnalyzeClass analyze in RawAnalyzeList)
                {
                    //if(analyze.No == 69 || analyze.No == 70)
                    //{
                    //    insidecount = insidecount;
                    //}

                    if (AnalyzeRoot.CheckAnalyzeFX(analyze))
                        insidecount++;
                }

                if (insidecount == 0)
                    break;
            }
            
            #endregion

            SetAnalyzeTree(null);

            OnTrigger(RCPStatusEnum.COMBINEANALYZE);

            JzToolsClass.myShowCursor(1);
        }

        ListForm LISTFRM;
        void EditDetail()
        {
            LISTFRM = new ListForm(RawAnalyzeList);

            if(LISTFRM.ShowDialog() == DialogResult.OK)
            {
                OnTrigger(RCPStatusEnum.EDITDETAIL);
            }
        }

        #endregion
        #region Event Operation
        private void btn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            switch ((TagEnum)btn.Tag)
            {
                case TagEnum.ADDSAMELEVEL:
                    AddSameLevel();
                    break;
                case TagEnum.ADDBRANCHLEVEL:
                    AddBranchLevel();
                    break;
                case TagEnum.DEL:
                    Delete();
                    break;
                case TagEnum.DUP:
                    Dup();
                    break;
                case TagEnum.EXPAND:
                    ExpandTree();
                    break;
                case TagEnum.COLLAPSE:
                    CollapseTree();
                    break;
                case TagEnum.TEST:
                    Test();
                    break;
                case TagEnum.CHECKLEARN:
                    CheckLearn();
                    break;
                case TagEnum.DELLEARN:
                    DelLearn();
                    break;
                case TagEnum.COMBINE:
                    Combine();
                    break;
                case TagEnum.EDITDETAIL:
                    EditDetail();
                    break;
            }
        }
        private void dtlvAnalyze_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            //if (IsForward)
            //{
            //    //lblInformation.Text = FirstSelectNo.ToString();
            //    //lblInformation.Invalidate();

            //    OnChange(FirstSelectNo);
            //    FillDisplay();
            //}
            //else
            //{
            //    if (dtlvAnalyze.SelectedIndex > -1)
            //    {
            //        FirstSelectNo = int.Parse(dtlvAnalyze.Items[dtlvAnalyze.SelectedIndex].Text.Split('-')[2]);

            //        OnChange(FirstSelectNo);
            //        FillDisplay();
            //    }
            //    //else
            //    //    FirstSelectNo = -1;
            //}
        }
        private void dtlvAnalyze_SelectionChanged(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            dtlvAnalyze_SelectionChanged();
            //OnBackward(CheckUISelect(FirstSelectNo));
            //OnChange(FirstSelectNo);
        }
        void dtlvAnalyze_SelectionChanged()
        {
            if (IsForward)
            {
                //lblInformation.Text = FirstSelectNo.ToString();
                //lblInformation.Invalidate();
                IsForward = false;

                OnChange(FirstSelectNo);
                FillDisplay();
            }
            else
            {
                if (dtlvAnalyze.SelectedIndices.Count == 0)
                {
                    FirstSelectNo = -1;

                    foreach (AnalyzeClass analyze in RawAnalyzeList)
                    {
                        analyze.SetMoverSelected(false);
                    }

                    OnChange(FirstSelectNo);
                    FillDisplay();
                }
                else
                {
                    if (dtlvAnalyze.SelectedIndex > -1)
                    {
                        FirstSelectNo = int.Parse(dtlvAnalyze.Items[dtlvAnalyze.SelectedIndex].Text.Split('-')[2]);

                        foreach (AnalyzeClass analyze in RawAnalyzeList)
                        {
                            analyze.SetMoverSelected(false);
                        }

                        AnalyzeOperateNow.SetMoverSelected(true, true);

                        OnChange(FirstSelectNo);
                        FillDisplay();
                    }
                    else
                    {
                        int i = 0;

                        while (i < dtlvAnalyze.SelectedIndices.Count)
                        {
                            int no = int.Parse(dtlvAnalyze.Items[dtlvAnalyze.SelectedIndices[i]].Text.Split('-')[2]);

                            if (no != FirstSelectNo)
                            {
                                AnalyzeClass analyze = GetAnalyzeFromTree(no);
                                analyze.SetMoverSelected(true);
                            }

                            i++;
                        }

                        OnChange(FirstSelectNo);
                        FillDisplay();
                    }
                }
                //else
                //    FirstSelectNo = -1;
            }
        }

        /// <summary>
        /// 傳回介面上選擇的項目
        /// </summary>
        /// <param name="firstselectno"></param>
        List<int> CheckUISelect(int firstselectno)
        {
            int i = 0;
            string Str = "";

            List<int> selectnolist = new List<int>();

            if (dtlvAnalyze.SelectedIndices.Count > 0)
            {
                selectnolist.Add(firstselectno);
                Str += firstselectno.ToString("00") + ",";
            }

            while (i < dtlvAnalyze.SelectedIndices.Count)
            {
                //if(dtlvAnalyze.Items[i].Selected)
                {
                    int no = int.Parse(dtlvAnalyze.Items[dtlvAnalyze.SelectedIndices[i]].Text.Split('-')[2]);

                    if (selectnolist.IndexOf(no) < 0)
                    {
                        selectnolist.Add(no);

                        Str += no.ToString("00") + ",";
                    }
                }
                i++;
            }

            lblInformation.Text = Str;
            lblInformation.Invalidate();

            return selectnolist;
            
        }
        private void dgvAnalyze_SelectionChanged(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            dgvAnalyze_SelectionChanged();
        }
        void dgvAnalyze_SelectionChanged()
        {
            if (dgvAnalyze.SelectedRows.Count == 0)
                return;

            if (dgvAnalyze.CurrentCell == null)
                return;

            FillDisplay();
        }

        private void lsbLearnAnalyze_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            if(IsLearn)
                OnChangeLearn(lsbLearnAnalyze.SelectedIndex);

        }

        #endregion
        void Test()
        {
            dtlvAnalyze.SelectedIndex = 1;
        }

        #region Trigger Operation
        public delegate void TriggerHandler(RCPStatusEnum status);
        public event TriggerHandler TriggerAction;
        public void OnTrigger(RCPStatusEnum status)
        {
            if (TriggerAction != null)
            {
                TriggerAction(status);
            }
        }

        public delegate void ChangeHandler(int index);
        public event ChangeHandler ChangeAction;
        public void OnChange(int index)
        {
            if (ChangeAction != null)
            {
                ChangeAction(index);
            }
        }

        public delegate void ChangeLearnHandler(int index);
        public event ChangeLearnHandler ChangeLearnAction;
        public void OnChangeLearn(int index)
        {
            if (ChangeLearnAction != null)
            {
                ChangeLearnAction(index);
            }
        }

        public delegate void BackwardHandler(List<int> selectnolist);
        public event BackwardHandler BackwardAction;
        public void OnBackward(List<int> selectnolist)
        {
            if (BackwardAction != null)
            {
                BackwardAction(selectnolist);
            }
        }

        public delegate void ChangeMoverHandler(List<AnalyzeClass> changedanalyzelist, DBStatusEnum changeaction);
        public event ChangeMoverHandler ChangeMoverAction;
        public void OnChangeMover(List<AnalyzeClass> changedanalyzelist, DBStatusEnum changeaction)
        {
            if (ChangeMoverAction != null)
            {
                ChangeMoverAction(changedanalyzelist, changeaction);
            }
        }

        public delegate void LearnTuneHandler(AnalyzeClass tuneanalyze);
        public event LearnTuneHandler LearnTuneAction;
        public void OnLearnTune(AnalyzeClass tuneanalyze)
        {
            if (LearnTuneAction != null)
            {
                LearnTuneAction(tuneanalyze);
            }
        }

        public delegate void GetOperateHandler(int no,int learnno);
        public event GetOperateHandler GetOperateAction;
        public void OnGetOperate(int no,int learnno)
        {
            if (GetOperateAction != null)
            {
                GetOperateAction(no, learnno);
            }
        }

        #endregion
    }
}
