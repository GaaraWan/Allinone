using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using JzDisplay.UISpace;
using JetEazy.UISpace;
using JzDisplay;
using JetEazy.BasicSpace;
using BrightIdeasSoftware;
using MoveGraphLibrary;
using WorldOfMoveableObjects;
using JzASN.OPSpace;
using JetEazy;

namespace JzASN.UISpace
{
    public partial class AsnUI : UserControl
    {
        VersionEnum VERSION;
        OptionEnum OPTION;

        List<ASNItemClass> ASNItemList
        {
            get
            {
                return ASN.ASNItemList;
            }
        }

        ASNClass ASN;

        DispUI DISPUI;

        myPropertyGrid ppgASN;
        DataTreeListView dtlvASN;
        DataGridView dgvASN;
        
        DataTable ASNTable = new DataTable();

        NumericUpDown numStep;

        Button btnReget;

        //Bitmap bmpVIEW;

        bool IsNeedToChange = false;
        public int FirstSelectNo = -1;
        bool IsForward = false;         //把從圖過來和從物件直接操作分開來

        Mover ShowMover = new Mover();

        List<int> subnoSelectList = new List<int>();

        int Step
        {
            get
            {
                return (int)numStep.Value;
            }
        }

        ASNItemClass ASNItemLast
        {
            get
            {
                return ASNItemList[ASNItemList.Count - 1];
            }
        }
        //ASNClass ASNNow;
        ASNItemClass ASNItemNow
        {
            get
            {
                if (FirstSelectNo < 0)
                    return null;
                else
                    return ASNItemList.Find(x => x.No == FirstSelectNo);
            }
        }
        public AsnUI()
        {
            InitializeComponent();
            InitialInside();

        }
        void InitialInside()
        {
            numStep = numericUpDown2;

            btnReget = button2;
            btnReget.Click += BtnReget_Click;

            DISPUI = dispUI1;
            DISPUI.Initial();
            DISPUI.SetDisplayType(DisplayTypeEnum.NORMAL);

            ppgASN = myPropertyGrid1;
            //ppgASN = propertyGrid1;
            //ppgASN.CanShowVisualStyleGlyphs = false;
            ppgASN.PropertySort = PropertySort.CategorizedAlphabetical;
            ppgASN.PropertyValueChanged += PpgASN_PropertyValueChanged;

            dgvASN = dataGridView2;
            dtlvASN = dataTreeListViewNew;

            dgvASN.Location = new Point(10000, 0);

            dtlvASN.RootKeyValue = 0u;
            dtlvASN.SelectedIndexChanged += dtlvASN_SelectedIndexChanged;
            dtlvASN.SelectionChanged += dtlvASN_SelectionChanged;       //全選擇完後會趨動此步驟

            DISPUI.MoverAction += DISPUI_MoverAction;
        }
        //public void Initial(List<ASNItemClass> asnlist,ref Bitmap bmpview)
        //{
        //    ASNList = asnlist;

        //    bmpVIEW = bmpview;

        //    DISPUI.ClearAll();

        //    CollectMover();
        //    DISPUI.SetMover(ShowMover);

        //    DISPUI.SetDisplayImage(bmpVIEW);

        //    SetASNTree(ASNList);

        //    DISPUI.MoverAction += DISPUI_MoverAction;
        //}
        public void Initial(ASNClass asn,VersionEnum version,OptionEnum option)
        {
            VERSION = version;
            OPTION = option;

            ASN = asn;

            IsNeedToChange = false;

            DISPUI.ClearAll();

            CollectMover();
            DISPUI.SetMover(ShowMover);

            DISPUI.ReplaceDisplayImage(ASN.bmpASN);

            SetASNTree();

            IsNeedToChange = true;

            FillDisplay();
        }
        private void BtnReget_Click(object sender, EventArgs e)
        {
            string FileName = JzToolsClass.OpenFilePicker("Image Files (*.png)|*.png|Image Files (*.bmp)|*.bmp|" + "All files (*.*)|*.*", "VIEW");

            if (FileName.Length > 0)
            {
                GetBMP(FileName, ref ASN.bmpASN);

                DISPUI.SetDisplayImage(ASN.bmpASN);
            }
        }
        private void PpgASN_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (!IsNeedToChange)
                return;

            string changeitemstr = e.ChangedItem.Parent.Label + ";" + e.ChangedItem.PropertyDescriptor.Name;

            ChangeAction(changeitemstr, e.ChangedItem.Value.ToString());

            //ASNNow.FromAssembleProperty();

            //DISPUI.RefreshDisplayShape();

            FillDisplay();
        }
        public void HoldSelect()
        {
            DISPUI.HoldSelect();
        }
        public void MoveMover(Keys KEY)
        {
            switch (KEY)
            {
                case Keys.Left:
                    DISPUI.MoveMover(-Step, 0);
                    break;
                case Keys.Right:
                    DISPUI.MoveMover(Step, 0);
                    break;
                case Keys.Up:
                    DISPUI.MoveMover(0, -Step);
                    break;
                case Keys.Down:
                    DISPUI.MoveMover(0, Step);
                    break;
            }
        }
        /// <summary>
        /// 傳回第一個被選中的位置
        /// </summary>
        int FirstSelectIndex
        {
            get
            {
                //return ASNList.FindIndex(x => x.No == FirstSelectNo);
                return GetIndexFromTree(FirstSelectNo);
            }
        }
        public void Add()
        {
            List<ASNItemClass> addedasnlist = new List<ASNItemClass>();
            List<int> selectasnnolist = new List<int>();

            if (FirstSelectNo > -1)
            {
                List<int> selectnolist = CheckUISelect(FirstSelectNo);

                int i = 0;

                foreach (int no in selectnolist)
                {
                    ASNItemClass selectasn = GetASNFromTree(no);

                    ASNItemClass sameasn = selectasn.Clone(new Point(50, 50));

                    sameasn.No = GetMaxNewNoFromRawASNList();
                    sameasn.AliasName = sameasn.ToAsnItemString();
                    sameasn.RelateMover(sameasn.No, 2);

                    //AddRowToASNTable(sameasn);
                    sameasn.AddNewRowToDataTable(ASNTable);
                    ASNItemList.Add(sameasn);

                    addedasnlist.Add(sameasn);
                    selectasnnolist.Add(sameasn.No);

                    if (i == 0)
                        sameasn.SetMoverSelected(true, true);
                    else
                        sameasn.SetMoverSelected(true);

                    //ASNClass parentasn = GetASNFromTree(sameasn.ParentNo);
                    //parentasn.BranchList.Add(sameasn);

                    i++;
                }
                //FirstSelectNo = branchasn.No;

                //OnChangeMover(addedasnlist, DBStatusEnum.ADD);
            }
            else
            {
                ASNItemClass newasn = new ASNItemClass(VERSION, OPTION);

                if (ASNItemList.Count > 0)
                {
                    newasn = ASNItemLast.Clone(new Point(100, 100));
                    newasn.No = GetMaxNewNoFromRawASNList();
                }

                newasn.AliasName = newasn.ToAsnItemString();
                newasn.RelateMover(newasn.No, 2);

                newasn.AddNewRowToDataTable(ASNTable);
                ASNItemList.Add(newasn);

                addedasnlist.Add(newasn);
                selectasnnolist.Add(newasn.No);

            }

            AddMover(addedasnlist);
            DISPUI.RefreshDisplayShape();
            DISPUI.SetMover(ShowMover);
            DISPUI.MappingSelect();
            DISPUI.RefreshDisplayShape();

            //dtlvASN.ExpandAll();

            SetSelectFocus(selectasnnolist);

            FillDisplay();
        }
        /// <summary>
        /// 刪除選定含分支的資料
        /// </summary>
        public void Delete()
        {
            if (MessageBox.Show("是否要刪除所選項目", "SYSTEM", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            int i = 0;

            List<ASNItemClass> deleteasnlist = new List<ASNItemClass>();
            List<int> deletenolist = new List<int>();

            if (FirstSelectNo > -1)
            {
                List<int> selectnolist = CheckUISelect(FirstSelectNo);

                foreach (int no in selectnolist)
                {
                    foreach (ASNItemClass asn in ASNItemList)
                    {
                        if (asn.No == no)
                        {
                            asn.InsertDeleteData(deletenolist, deleteasnlist);
                        }
                    }
                }

                i = ASNTable.Rows.Count - 1;
                while (i > -1)
                {
                    if (deletenolist.IndexOf((int)(UInt32)ASNTable.Rows[i]["No"]) > -1)
                        ASNTable.Rows.RemoveAt(i);

                    i--;
                }

                DeleteMover(deleteasnlist);
                DISPUI.RefreshDisplayShape();
                DISPUI.SetMover(ShowMover);

                DISPUI.MappingSelect();

                i = ASNItemList.Count - 1;
                while (i > -1)
                {
                    ASNItemClass asn = ASNItemList[i];

                    if (deletenolist.IndexOf(asn.No) > -1)
                    {
                        asn.Suicide();
                        ASNItemList.RemoveAt(i);
                    }
                    i--;
                }

                deletenolist.Clear();
                SetSelectFocus(deletenolist);

            }
            else
                MessageBox.Show("請選擇需要刪除的項目", "SYSTEM", MessageBoxButtons.OK);

        }
        ASNItemClass GetASNFromTree(int no)
        {
            return ASNItemList.Find(x => x.No == no);
        }
        /// <summary>
        /// 尋找最大的編號
        /// </summary>
        /// <returns></returns>
        int GetMaxNewNoFromRawASNList()
        {
            int max = -1000;

            foreach (ASNItemClass asn in ASNItemList)
            {
                if (max < asn.No)
                {
                    max = asn.No;
                }
            }

            if (ASNItemList.Count == 0)
                max = 0;

            max++;

            return max;
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

            if (dtlvASN.SelectedIndices.Count > 0)
            {
                selectnolist.Add(firstselectno);
                Str += firstselectno.ToString("00") + ",";
            }

            while (i < dtlvASN.SelectedIndices.Count)
            {
                //if(dtlvASN.Items[i].Selected)
                {
                    int no = int.Parse(dtlvASN.Items[dtlvASN.SelectedIndices[i]].Text.Split('-')[1]);

                    if (selectnolist.IndexOf(no) < 0)
                    {
                        selectnolist.Add(no);

                        Str += no.ToString("00") + ",";
                    }
                }
                i++;
            }

            //lblInformation.Text = Str;
            //lblInformation.Invalidate();

            return selectnolist;

        }

        private void dtlvASN_SelectedIndexChanged(object sender, EventArgs e)
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
            //    if (dtlvASN.SelectedIndex > -1)
            //    {
            //        FirstSelectNo = int.Parse(dtlvASN.Items[dtlvASN.SelectedIndex].Text.Split('-')[1]);

            //        OnChange(FirstSelectNo);
            //        FillDisplay();
            //    }
            //    //else
            //    //    FirstSelectNo = -1;
            //}
        }
        private void dtlvASN_SelectionChanged(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            dtlvASN_SelectionChanged();
            //OnBackward(CheckUISelect(FirstSelectNo));
            //OnChange(FirstSelectNo);
        }
        void dtlvASN_SelectionChanged()
        {
            if (IsForward)
            {
                //lblInformation.Text = FirstSelectNo.ToString();
                //lblInformation.Invalidate();
                IsForward = false;

                ChangeAction(FirstSelectNo);
                //FillDisplay();
            }
            else
            {
                if (dtlvASN.SelectedIndices.Count == 0)
                {
                    FirstSelectNo = -1;

                    foreach (ASNItemClass asn in ASNItemList)
                    {
                        asn.SetMoverSelected(false);
                    }

                    ChangeAction(FirstSelectNo);
                    FillDisplay();
                }
                else
                {
                    if (dtlvASN.SelectedIndex > -1)
                    {
                        FirstSelectNo = int.Parse(dtlvASN.Items[dtlvASN.SelectedIndex].Text.Split('-')[1]);

                        foreach (ASNItemClass asn in ASNItemList)
                        {
                            asn.SetMoverSelected(false);
                        }

                        ASNItemNow.SetMoverSelected(true, true);

                        ChangeAction(FirstSelectNo);
                        FillDisplay();
                    }
                    else
                    {
                        int i = 0;

                        while (i < dtlvASN.SelectedIndices.Count)
                        {
                            int no = int.Parse(dtlvASN.Items[dtlvASN.SelectedIndices[i]].Text.Split('-')[1]);

                            if (no != FirstSelectNo)
                            {
                                ASNItemClass analyze = GetASNFromTree(no);
                                analyze.SetMoverSelected(true);
                            }

                            i++;
                        }

                        ChangeAction(FirstSelectNo);
                        //FillDisplay();
                    }
                }
                //else
                //    FirstSelectNo = -1;
            }
        }

        void ChangeAction(int no)
        {
            //if (no > -1)
            //{
            //    FirstSelectNo = no;
            //    SetASN(ASNNow);
            //}
            //else
            //{
            //    FirstSelectNo = no;
            //    SetASN(null);
            //}
            
            DISPUI.MappingSelect();

        }

        private void ChangeAction(string changeitemstring, string valuestring)
        {
            //若有全選並改變值而不會變的加在這裏
            if (CheckJustChangeOne(changeitemstring))
            {
                ASNItemNow.ToAssembleProperty();
                ASNItemNow.FromAssembleProperty(changeitemstring, valuestring);
            }
            else
            {
                foreach (ASNItemClass asn in ASNItemList)
                {
                    if (asn.IsSelected)
                    {
                        asn.ToAssembleProperty();
                        asn.FromAssembleProperty(changeitemstring, valuestring);
                    }
                }
            }

            CollectMover(false);
            DISPUI.SetMover(ShowMover);
            DISPUI.RefreshDisplayShape();
            DISPUI.ReDraw();
        }

        /// <summary>
        /// 檢查是否這是只能改變選定項的值，而不是一次要改變所有的值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        bool CheckJustChangeOne(string str)
        {
            bool ret = true;

            ret &= str.ToUpper().IndexOf("ALIASNAME") > -1;

            return ret;
        }
        private void dgvASN_SelectionChanged(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            dgvASN_SelectionChanged();
        }
        void dgvASN_SelectionChanged()
        {
            if (dgvASN.SelectedRows.Count == 0)
                return;

            if (dgvASN.CurrentCell == null)
                return;

            //FillDisplay();
        }
        private void DISPUI_MoverAction(MoverOpEnum moverop, string opstring)
        {
            int i = 0;
            string[] strs = opstring.Split(',');

            IsNeedToChange = false;

            switch (moverop)
            {
                case MoverOpEnum.SELECT:
                    DISPSelectActionDX(strs);
                    break;
                    //case MoverOpEnum.ADD:
                    //    foreach (string str in strs)
                    //    {
                    //        lsbShapes.Items.Add(str);
                    //    }
                    //    break;
                    //case MoverOpEnum.DEL:
                    //    List<int> delindexlist = new List<int>();

                    //    foreach (string str in strs)
                    //    {
                    //        delindexlist.Add(int.Parse(str));
                    //    }

                    //    delindexlist.Sort();
                    //    delindexlist.Reverse();

                    //    foreach (int delindex in delindexlist)
                    //    {
                    //        lsbShapes.Items.RemoveAt(delindex);
                    //    }
                    //    break;
            }

            IsNeedToChange = true;
        }
        /// <summary>
        /// With Sub Selection
        /// </summary>
        /// <param name="strs"></param>
        void DISPSelectActionDX(string[] strs)
        {
            List<string> selectasnstringlist = new List<string>();

            if (strs[0] == "")
            {
                SetSelectFocus(selectasnstringlist);
                //SetASN(null);
            }
            else
            {
                foreach (string str in strs)
                {
                    //string[] strxs = str.Split(':');
                    //int analyzeno = int.Parse(strxs[0]);

                    if (selectasnstringlist.IndexOf(str) < 0)
                        selectasnstringlist.Add(str);
                }
                SetSelectFocus(selectasnstringlist);

                //SetASN(ASNNow);
            }

            FillDisplay();
        }

        void AddMover(List<ASNItemClass> newaddasnlist)
        {
            foreach (ASNItemClass asn in newaddasnlist)
            {
                int i = 0;
                Mover mover = asn.myMover;

                while (i < mover.Count)
                {
                    GraphicalObject grobj = mover[i].Source;

                    ShowMover.Add(grobj);

                    i++;
                }
            }
        }
        /// <summary>
        /// 刪除所選ASN的Mover
        /// </summary>
        /// <param name="deleteasnlist"></param>
        void DeleteMover(List<ASNItemClass> deleteasnlist)
        {
            foreach (ASNItemClass asn in deleteasnlist)
            {
                int i = ShowMover.Count - 1;

                while (i > -1)
                {
                    GraphicalObject grpobj = ShowMover[i].Source;

                    if ((grpobj as GeoFigure).RelateNo == asn.No)
                    {
                        ShowMover.RemoveAt(i);
                    }
                    i--;
                }
            }
        }
        /// <summary>
        /// 將Mover裏的選擇清除
        /// </summary>
        void ClearMoverSelection()
        {
            foreach (ASNItemClass asn in ASNItemList)
            {
                int i = 0;

                Mover mover = asn.myMover;

                while (i < mover.Count)
                {
                    GraphicalObject grobj = mover[i].Source;

                    (grobj as GeoFigure).IsSelected = false;
                    (grobj as GeoFigure).IsFirstSelected = false;

                    i++;
                }
            }
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

            while (i < dtlvASN.Items.Count)
            {
                string str = dtlvASN.Items[i].Text;
                string[] strs = str.Split('-');

                int getno = int.Parse(strs[1]);

                if (getno == no)
                {
                    ret = i;
                    break;
                }

                i++;
            }

            return ret;
        }
        /// <summary>
        /// 從圖形選擇轉換到ListView選擇
        /// </summary>
        /// <param name="selectasnnolist"></param>
        public void SetSelectFocus(List<int> selectasnnolist)
        {
            IsForward = true;

            if (selectasnnolist.Count == 0)
            {
                FirstSelectNo = -1;

                //if(dtlvASN.SelectedIndex != -1)
                dtlvASN.SelectedIndex = -1;
            }
            else
            {
                int firstno = -1;
                string Str = "";

                foreach (int no in selectasnnolist)
                {
                    Str += no.ToString("00") + ",";

                    //int foundindex = ASNList.FindIndex(x => x.No == no);
                    int foundindex = GetIndexFromTree(no);

                    if (firstno == -1)
                    {
                        firstno = no;
                        FirstSelectNo = no;

                        IsForward = true;
                        dtlvASN.SelectedIndex = FirstSelectIndex;
                    }

                    IsNeedToChange = false;
                    dtlvASN.Items[foundindex].Selected = true;
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
        /// <param name="selectasnstringlist"></param>
        public void SetSelectFocus(List<string> selectasnstringlist)
        {
            int i = 0;

            IsForward = true;
            subnoSelectList.Clear();

            if (selectasnstringlist.Count == 0)
            {
                FirstSelectNo = -1;
                //if(dtlvASN.SelectedIndex != -1)
                dtlvASN.SelectedIndex = -1;

                //OnChange(FirstSelectNo);
            }
            else
            {
                int firstno = -1;
                string Str = "";
                List<int> realselectindexlist = new List<int>();

                IsNeedToChange = false;

                if (selectasnstringlist.Count > 1)
                    Str = Str;

                foreach (string selstr in selectasnstringlist)
                {
                    Str += selstr + ",";

                    string[] strs = selstr.Split(':');

                    int selectno = int.Parse(strs[0]);
                    //int foundindex = ASNList.FindIndex(x => x.No == no);
                    int foundindex = GetIndexFromTree(selectno);

                    dtlvASN.Items[foundindex].Selected = true;
                    realselectindexlist.Add(foundindex);

                    //if (firstno == -1)
                    if (FirstSelectNo != selectno && subnoSelectList.Count == 0)
                    {
                        firstno = selectno;
                        FirstSelectNo = selectno;

                        subnoSelectList.Add(int.Parse(strs[1]));

                        //OnChange(FirstSelectNo);
                    }
                    else if (selectno == FirstSelectNo)
                    {
                        subnoSelectList.Add(int.Parse(strs[1]));

                        //OnChange(FirstSelectNo);
                    }
                }

                i = 0;

                while (i < dtlvASN.Items.Count)
                {
                    if (realselectindexlist.IndexOf(i) < 0)
                    {
                        dtlvASN.Items[i].Selected = false;
                    }
                    i++;
                }

                //OnChange(FirstSelectNo);

                IsNeedToChange = true;

                Str = Str.Remove(Str.Length - 1, 1);
                //lblInformation.Text = Str;

                //IsForward = true;
                //dtlvASN.SelectedIndex = FirstSelectIndex;
            }

            //lblInformation.Invalidate();

            //IsForward = false;  //Forawrd and Backward Should be Reset
        }
        /// <summary>
        /// 設定ListView
        /// </summary>
        /// <param name="asnlist"></param>
        public void SetASNTree()    //List<ASNItemClass> asnlist)
        {
            //Parent 要設定為 0 才會停在根目錄

            ASNTable.Columns.Clear();
            ASNTable.Clear();

            //ASNList = asnlist;

            DataColumn id = new DataColumn("No", typeof(UInt32));               //一定要用UInt32
            DataColumn parentid = new DataColumn("ParentNo", typeof(UInt32));    //一定要用UInt32
            DataColumn asnname = new DataColumn("Name", typeof(String));

            ASNTable.Columns.Add(id);
            ASNTable.Columns.Add(parentid);
            ASNTable.Columns.Add(asnname);

            //if (ASNItemList.Count == 0)
            //{
            //    ASNItemClass asnitemroot = new ASNItemClass();

            //    asnitemroot.ParentNo = 0;
            //    asnitemroot.No = 1;

            //    asnitemroot.AddNewRowToDataTable(ASNTable);
            //}

            foreach (ASNItemClass asnitem in ASNItemList)
            {
                //if (asn.ASNType == ASNTypeEnum.BRANCH)
                {
                    //AddRowToASNTable(asn);
                    asnitem.AddNewRowToDataTable(ASNTable);
                    //DataRow newdatarow = ASNTable.NewRow();
                    //newdatarow["No"] = asn.No;
                    //newdatarow["ParentNo"] = asn.ParentNo;
                    //newdatarow["Name"] = asn.ToASNString();
                    //newdatarow["Level"] = asn.Level;
                    //ASNTable.Rows.Add(newdatarow);
                }
            }

            dgvASN.DataSource = ASNTable;
            dgvASN.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dtlvASN.AutoGenerateColumns = true;
            dtlvASN.ShowKeyColumns = false;
            dtlvASN.ParentKeyAspectName = "ParentNo";
            dtlvASN.KeyAspectName = "No";
            dtlvASN.HeaderUsesThemes = false;
            dtlvASN.UseFilterIndicator = false;
            dtlvASN.UseFiltering = false;
            dtlvASN.FullRowSelect = true;
            dtlvASN.MultiSelect = true;
            dtlvASN.HideSelection = false;

            dtlvASN.DataSource = ASNTable;

            dtlvASN.Columns[0].Width = 240;
            //dtlvASN.Columns[1].Width = 30;
            //dtlvASN.Columns[2].Width = 30;

            dtlvASN.ExpandAll();

            IsNeedToChange = true;

            IsForward = true;
            FirstSelectNo = -1;
            dtlvASN.SelectedIndex = -1;
            IsForward = false;
        }
        public void SetEnable(bool isenable)
        {
            DISPUI.Enabled = isenable;
            dtlvASN.Enabled = isenable;

            btnReget.Enabled = isenable;
            numStep.Enabled = isenable;
        }
        public void SetDefaultView()
        {
            DISPUI.SetDisplayImage();
        }
        public void ReleaseSelect()
        {
            DISPUI.ReleaseSelect();
        }
        void CollectMover()
        {
            CollectMover(true);
        }
        void CollectMover(bool ischangeselect)
        {
            ShowMover.Clear();

            foreach (ASNItemClass asn in ASNItemList)
            {
                int i = 0;

                Mover mover = asn.myMover;

                while (i < mover.Count)
                {
                    GraphicalObject grobj = mover[i].Source;

                    //if(IsLearn)
                    //{
                    //    if (asn.LearnIndex == -1)   
                    //    {
                    //        PointF offset = asn.myOringineOffsetPointF;

                    //        offset.X = -offset.X;
                    //        offset.Y = -offset.Y;

                    //        (grobj as GeoFigure).MappingToMovingObject(offset, new SizeF(1f, 1f));
                    //    }

                    //}

                    if (ischangeselect)
                    {
                        (grobj as GeoFigure).IsSelected = false;
                        (grobj as GeoFigure).IsFirstSelected = false;
                    }

                    ShowMover.Add(grobj);

                    i++;
                }
            }
        }
        void FillDisplay()
        {
            IsNeedToChange = false;

            if (ASNItemNow != null)
            {
                ASNItemNow.ToAssembleProperty();

                ppgASN.Enabled = true;
                ppgASN.SelectedObject = ASNItemNow.ASSEMBLE;
            }
            else
            {
                ppgASN.Enabled = false;
                ppgASN.SelectedObject = null;
            }

            IsNeedToChange = true;

            //if (lsbShape.Items.Count > 0)
            //    lsbShape.SelectedIndex = 0;

        }
        public void Suicide()
        {
            DISPUI.Suicide();

            //bmpVIEW.Dispose();
        }
        string OpenFilePicker(string DefaultPath, string DefaultName)
        {
            string retStr = "";

            OpenFileDialog dlg = new OpenFileDialog();

            //dlg.Filter = "BMP Files (*.bmp)|*.BMP|" + "All files (*.*)|*.*";
            dlg.Filter = DefaultPath;
            dlg.FileName = DefaultName;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                retStr = dlg.FileName;
            }
            return retStr;
        }
        void GetBMP(string bmpfilestr, ref Bitmap bmp)
        {
            Bitmap bmptmp = new Bitmap(bmpfilestr);

            bmp.Dispose();
            bmp = new Bitmap(bmptmp);

            bmptmp.Dispose();
        }
    }
}
