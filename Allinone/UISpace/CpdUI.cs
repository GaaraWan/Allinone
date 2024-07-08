using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
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
using Allinone.OPSpace;
using Allinone.FormSpace;

using JzASN.OPSpace;
using JetEazy;

namespace Allinone.UISpace
{
    public partial class CpdUI : UserControl
    {
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
        ASNCollectionClass ASNCollection
        {
            get
            {
                return Universal.ASNCollection;
            }
        }
        AlbumClass ALBNow
        {
            get
            {
                return Universal.ALBNow;
            }
        }
        List<CPDItemClass> CPDItemList
        {
            get
            {
                return CPD.CPDItemList;
            }
        }

        CPDClass CPD;

        DispUI DISPUI;

        myPropertyGrid ppgCPD;
        DataTreeListView dtlvCPD;
        DataGridView dgvCPD;
        
        DataTable CPDTable = new DataTable();

        NumericUpDown numStep;

        Button btnRefresh;
        Button btnResize;

        Button btnUp;
        Button btnDn;

        Bitmap bmpBASE = new Bitmap(1, 1);

        bool IsNeedToChange = false;
        public int FirstSelectNo = -1;
        bool IsForward = false;         //把從圖過來和從物件直接操作分開來

        Mover myMoverCollection = new Mover();

        List<int> subnoSelectList = new List<int>();
        int Step
        {
            get
            {
                return (int)numStep.Value;
            }
        }
        CPDItemClass CPDItemLast
        {
            get
            {
                return CPD.CPDItemList[CPD.CPDItemList.Count - 1];
            }
        }
        CPDItemClass CPDItemNow
        {
            get
            {
                if (FirstSelectNo < 0)
                    return null;
                else
                    return CPD.CPDItemList.Find(x => x.No == FirstSelectNo);
            }
        }
        public CpdUI()
        {
            InitializeComponent();
            InitialInside();

        }
        void InitialInside()
        {
            numStep = numericUpDown2;

            btnRefresh = button2;
            btnResize = button1;
            btnUp = button3;
            btnDn = button4;

            btnResize.Click += BtnResize_Click;
            btnRefresh.Click += BtnRefresh_Click;
            btnUp.Click += BtnUp_Click;
            btnDn.Click += BtnDn_Click;

            DISPUI = dispUI1;
            DISPUI.Initial(5f,0.05f);
            DISPUI.SetDisplayType(DisplayTypeEnum.NORMAL);
            DISPUI.DebugAction += DISPUI_DebugAction;


            ppgCPD = myPropertyGrid1;
            //ppgASN = propertyGrid1;
            //ppgASN.CanShowVisualStyleGlyphs = false;
            ppgCPD.PropertySort = PropertySort.CategorizedAlphabetical;
            ppgCPD.PropertyValueChanged += PpgASN_PropertyValueChanged;

            dgvCPD = dataGridView2;
            dtlvCPD = dataTreeListViewNew;

            dgvCPD.Location = new Point(10000, 0);

            dtlvCPD.RootKeyValue = 0u;
            dtlvCPD.SelectedIndexChanged += dtlvASN_SelectedIndexChanged;
            dtlvCPD.SelectionChanged += dtlvASN_SelectionChanged;       //全選擇完後會趨動此步驟


        }
        private void BtnDn_Click(object sender, EventArgs e)
        {
            if (FirstSelectIndex > -1 && FirstSelectIndex < CPDItemList.Count -1)
            {
                int i = 0;
                string CPDItemName = CPDItemNow.Name;
                
                while (i < CPDItemList.Count)
                {
                    if ((string)CPDTable.Rows[i]["Name"] == CPDItemName)
                    {
                        DataRow orgdatarow = CPDTable.NewRow();
                        orgdatarow.ItemArray = CPDTable.Rows[i + 1].ItemArray.Clone() as object[];
                        //DataRow datarow = CPDTable.Rows[i + 1];
                        CPDItemClass cpditem = CPDItemList[i + 1];

                        //if (i > 0)
                        {
                            CPDTable.Rows.RemoveAt(i + 1);
                            CPDTable.Rows.InsertAt(orgdatarow, i);
                            
                            CPDItemList.RemoveAt(i + 1);
                            CPDItemList.Insert(i, cpditem);
                        }
                        break;
                    }
                    i++;
                }
                

                RefreshCompound();
            }
        }
        private void BtnUp_Click(object sender, EventArgs e)
        {
            if(FirstSelectIndex > -1 && FirstSelectIndex > 0)
            {
                int i = 0;
                string CPDItemName = CPDItemNow.Name;

                while(i < CPDItemList.Count)
                {
                    if((string)CPDTable.Rows[i]["Name"] == CPDItemName)
                    {
                        DataRow orgdatarow = CPDTable.NewRow();
                        orgdatarow.ItemArray = CPDTable.Rows[i].ItemArray.Clone() as object [];

                        CPDItemClass cpditem = CPDItemList[i];

                        //if(i > 0)
                        {
                            CPDTable.Rows.RemoveAt(i);
                            CPDTable.Rows.InsertAt(orgdatarow, i - 1);

                            CPDItemList.RemoveAt(i);
                            CPDItemList.Insert(i - 1, cpditem);
                        }

                        //Should Add This For Focus Change
                        FirstSelectNo = cpditem.No;
                        List<int> focusnolist = new List<int>();
                        focusnolist.Add(FirstSelectNo);
                        SetSelectFocus(focusnolist);

                        break;
                    }
                    i++;
                }
                RefreshCompound();
            }
        }
        private void DISPUI_DebugAction(string opstring)
        {
            switch(opstring)
            {
                case "RELEASMOVE":
                    RefreshCompound();
                    break;
            }
        }
        public void ReleaseSelect()
        {
            DISPUI.ReleaseSelect();
        }
        SizeForm SIZEFRM;
        private void BtnResize_Click(object sender, EventArgs e)
        {
            JzToolsClass.PassingString = bmpBASE.Width.ToString() + "," + bmpBASE.Height.ToString();

            SIZEFRM = new SizeForm();

            if(SIZEFRM.ShowDialog() == DialogResult.OK)
            {
                string[] strs = JzToolsClass.PassingString.Split(',');

                bmpBASE.Dispose();
                bmpBASE = new Bitmap(int.Parse(strs[0]), int.Parse(strs[1]));
                DrawRect(bmpBASE, new Rectangle(0, 0, bmpBASE.Width, bmpBASE.Height), new SolidBrush(Color.White));
                
                DISPUI.SetDisplayImage(bmpBASE);

                CPD.BaseSize = new Size(int.Parse(strs[0]), int.Parse(strs[1]));

            }
        }
        public void Initial(CPDClass cpd)
        {
            CPD = cpd;

            bmpBASE.Dispose();
            bmpBASE = new Bitmap(CPD.BaseSize.Width, (int)CPD.BaseSize.Height);
            //DrawRect(bmpBASE, new Rectangle(0, 0, bmpBASE.Width, bmpBASE.Height), new SolidBrush(Color.White));

            DISPUI.ClearAll();

            CollectMover();
            DISPUI.SetMover(myMoverCollection);

            DISPUI.SetDisplayImage(bmpBASE);

            SetCPDTree();

            DISPUI.MoverAction += DISPUI_MoverAction;

            RefreshCompound();
        }
        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            foreach(CPDItemClass cpditem in CPD.CPDItemList)
            {
                //cpditem.bmpITEM.Dispose();
                GetDataFromCompoundData(cpditem.NORMALPara.RelatePA, ref cpditem.bmpITEM);

                cpditem.RenewSize(cpditem.bmpITEM.Size);
                //cpditem.RenewSize(new SizeF(1200f,1200f));

            }

            RefreshCompound();
            FillDisplay();
        }
        private void PpgASN_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (!IsNeedToChange)
                return;

            string changeitemstr = e.ChangedItem.Parent.Label + ";" + e.ChangedItem.PropertyDescriptor.Name;

            ChangeAction(changeitemstr, e.ChangedItem.Value.ToString());

            CPDItemNow.FromAssembleProperty();

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

            RefreshCompound();
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

        CPDListForm CPDLISTFRM;
        public void Add()
        {
            CPDLISTFRM = new CPDListForm(ALBNow, ASNCollection);

            if(CPDLISTFRM.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            string[] strs = JzToolsClass.PassingString.Split(Universal.NewlineChar);
            

            List<CPDItemClass> addedcpdlist = new List<CPDItemClass>();
            List<int> selectasnnolist = new List<int>();
            
            foreach(string str in strs)
            {
                Bitmap bmp = new Bitmap(1, 1);
                SizeF sizef = GetDataFromCompoundData(str, ref bmp);

                CPDItemClass newcpditem = new CPDItemClass(VERSION, OPTION, sizef);

                if (CPDItemList.Count > 0)
                {
                    PointF ptf = CPDItemLast.RatioRectEAG.GetRectF.Location;

                    int no = GetMaxNewNoFromRawCPDList();
                    newcpditem = new CPDItemClass(VERSION, OPTION, sizef, new PointF(ptf.X + 100, ptf.Y + 100), no);
                    //newcpditem.No = GetMaxNewNoFromRawCPDList();
                }

                newcpditem.NORMALPara.RelatePA = str;
                newcpditem.NORMALPara.OrgSizeF = sizef;
                newcpditem.bmpITEM = bmp;
                newcpditem.Name = newcpditem.ToCpdItemString();
                newcpditem.RelateMover(newcpditem.No, 2);

                newcpditem.AddNewRowToDataTable(CPDTable);
                CPDItemList.Add(newcpditem);

                addedcpdlist.Add(newcpditem);
                selectasnnolist.Add(newcpditem.No);
            }

            AddMover(addedcpdlist);
            DISPUI.RefreshDisplayShape();
            DISPUI.SetMover(myMoverCollection);
            DISPUI.MappingSelect();
            DISPUI.RefreshDisplayShape();

            //dtlvASN.ExpandAll();
            RefreshCompound();

            SetSelectFocus(selectasnnolist);

            FillDisplay();
        }

        SizeF GetDataFromCompoundData(string str,ref Bitmap bmp)
        {
            string[] strs;
            SizeF sizef = new SizeF();

            switch(str[0])
            {
                case 'E':
                    strs = str.Split('-');

                    foreach(EnvClass env in ALBNow.ENVList)
                    {
                        if(env.ToEnvString() == strs[0])
                        {
                            foreach(PageClass page in env.PageList)
                            {
                                if(page.ToPageIndexString() == strs[1])
                                {
                                    bmp.Dispose();
                                    bmp = new Bitmap(page.GetbmpORG((PageOPTypeEnum)int.Parse(strs[2])));

                                    sizef = bmp.Size;
                                }
                            }
                        }
                    }
                    break;
                default:
                    strs = str.Split(':');

                    foreach (ASNClass asn in ASNCollection.myDataList)
                    {
                        if(asn.ToNoString() == strs[0])
                        {
                            bmp.Dispose();
                            bmp = new Bitmap(asn.bmpASN);

                            sizef = bmp.Size;
                        }
                    }

                    break;
            }
            
            return sizef;
        }

        public void RefreshCompound()
        {
            DrawRect(bmpBASE, new Rectangle(0, 0, bmpBASE.Width, bmpBASE.Height), new SolidBrush(Color.White));

            JzToolsClass jztools = new JzToolsClass();
            Bitmap myBitbmp = new Bitmap(1, 1);

            foreach (CPDItemClass cpditem in CPD.CPDItemList)
            {
                Graphics g = Graphics.FromImage(bmpBASE);
                Rectangle rect = cpditem.RatioRectEAG.GetRect;
                switch (OPTION)
                {
                    case JetEazy.OptionEnum.MAIN_SERVICE:
                    case OptionEnum.MAIN_X6:
                    case OptionEnum.MAIN_SDM1:
                    case OptionEnum.MAIN_SDM2:
                    case OptionEnum.MAIN_SDM3:
                        Rectangle myCloneRectbase = jztools.SimpleRect(cpditem.bmpITEM.Size);
                        Rectangle myCloneRect = new Rectangle(myCloneRectbase.X + cpditem.NORMALPara.iLeft,
                                                                                             myCloneRectbase.Y + cpditem.NORMALPara.iTop,
                                                                                             myCloneRectbase.Width - cpditem.NORMALPara.iLeft - cpditem.NORMALPara.iRight,
                                                                                             myCloneRectbase.Height - cpditem.NORMALPara.iTop - cpditem.NORMALPara.iBottom);

                        jztools.BoundRect(ref myCloneRect, cpditem.bmpITEM.Size);

                        if (myCloneRect.Width > 100 && myCloneRect.Height > 100)
                        {
                            myBitbmp.Dispose();
                            myBitbmp = cpditem.bmpITEM.Clone(myCloneRect, cpditem.bmpITEM.PixelFormat);
                            cpditem.RenewSize(myBitbmp.Size);
                            g.DrawImage(myBitbmp, rect);
                        }
                        else
                        {
                            g.DrawImage(cpditem.bmpITEM, rect);
                        }

                        break;
                    default:
                        g.DrawImage(cpditem.bmpITEM, rect);
                        break;
                }
                g.Dispose();

                rect.Intersect(new Rectangle(0, 0, bmpBASE.Width, bmpBASE.Height));

                jztools.SetBrightContrast(bmpBASE, rect, cpditem.NORMALPara.Brightness, cpditem.NORMALPara.Contrast);
            }

            DISPUI.ReplaceDisplayImage(bmpBASE);
        }

        /// <summary>
        /// 刪除選定含分支的資料
        /// </summary>
        public void Delete()
        {
            if (MessageBox.Show("是否要刪除所選項目", "SYSTEM", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            int i = 0;

            List<CPDItemClass> deletecpditemlist = new List<CPDItemClass>();
            List<int> deletenolist = new List<int>();

            if (FirstSelectNo > -1)
            {
                List<int> selectnolist = CheckUISelect(FirstSelectNo);

                foreach (int no in selectnolist)
                {
                    foreach (CPDItemClass cpditem in CPDItemList)
                    {
                        if (cpditem.No == no)
                        {
                            cpditem.InsertDeleteData(deletenolist, deletecpditemlist);
                        }
                    }
                }

                i = CPDTable.Rows.Count - 1;
                while (i > -1)
                {
                    if (deletenolist.IndexOf((int)(UInt32)CPDTable.Rows[i]["No"]) > -1)
                        CPDTable.Rows.RemoveAt(i);

                    i--;
                }

                DeleteMover(deletecpditemlist);
                DISPUI.RefreshDisplayShape();
                DISPUI.SetMover(myMoverCollection);

                DISPUI.MappingSelect();

                i = CPDItemList.Count - 1;
                while (i > -1)
                {
                    CPDItemClass cpditem = CPDItemList[i];

                    if (deletenolist.IndexOf(cpditem.No) > -1)
                    {
                        cpditem.Suicide();
                        CPDItemList.RemoveAt(i);
                    } 
                    i--;
                }

                deletenolist.Clear();
                SetSelectFocus(deletenolist);

                RefreshCompound();
            }
            else
                MessageBox.Show("請選擇需要刪除的項目", "SYSTEM", MessageBoxButtons.OK);

        }
        CPDItemClass GetCPDItemFromTree(int no)
        {
            return CPDItemList.Find(x => x.No == no);
        }
        /// <summary>
        /// 尋找最大的編號
        /// </summary>
        /// <returns></returns>
        int GetMaxNewNoFromRawCPDList()
        {
            int max = -1000;

            foreach (CPDItemClass cpd in CPDItemList)
            {
                if (max < cpd.No)
                {
                    max = cpd.No;
                }
            }

            if (CPDItemList.Count == 0)
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

            if (dtlvCPD.SelectedIndices.Count > 0)
            {
                selectnolist.Add(firstselectno);
                Str += firstselectno.ToString("00") + ",";
            }

            while (i < dtlvCPD.SelectedIndices.Count)
            {
                //if(dtlvASN.Items[i].Selected)
                {
                    int no = int.Parse(dtlvCPD.Items[dtlvCPD.SelectedIndices[i]].Text.Split('-')[1]);

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
                if (dtlvCPD.SelectedIndices.Count == 0)
                {
                    FirstSelectNo = -1;

                    foreach (CPDItemClass cpditem in CPDItemList)
                    {
                        cpditem.SetMoverSelected(false);
                    }

                    ChangeAction(FirstSelectNo);
                    FillDisplay();
                }
                else
                {
                    if (dtlvCPD.SelectedIndex > -1)
                    {
                        FirstSelectNo = int.Parse(dtlvCPD.Items[dtlvCPD.SelectedIndex].Text.Split('-')[1]);

                        foreach (CPDItemClass cpd in CPDItemList)
                        {
                            cpd.SetMoverSelected(false);
                        }

                        CPDItemNow.SetMoverSelected(true, true);

                        ChangeAction(FirstSelectNo);
                        FillDisplay();
                    }
                    else
                    {
                        int i = 0;

                        while (i < dtlvCPD.SelectedIndices.Count)
                        {
                            int no = int.Parse(dtlvCPD.Items[dtlvCPD.SelectedIndices[i]].Text.Split('-')[1]);

                            if (no != FirstSelectNo)
                            {
                                CPDItemClass analyze = GetCPDItemFromTree(no);
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
                CPDItemNow.ToAssembleProperty();
                CPDItemNow.FromAssembleProperty(changeitemstring, valuestring);
            }
            else
            {
                foreach (CPDItemClass cpditem in CPDItemList)
                {
                    if (cpditem.IsSelected)
                    {
                        cpditem.ToAssembleProperty();
                        cpditem.FromAssembleProperty(changeitemstring, valuestring);
                    }
                }
            }

            CollectMover(false);
            DISPUI.SetMover(myMoverCollection);
            DISPUI.RefreshDisplayShape();
            DISPUI.ReDraw();

            FillDisplay();

            RefreshCompound();
        }

        /// <summary>
        /// 檢查是否這是只能改變選定項的值，而不是一次要改變所有的值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        bool CheckJustChangeOne(string str)
        {
            bool ret = true;

            ret &= str.ToUpper().IndexOf("NAME") > -1;

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
            if (dgvCPD.SelectedRows.Count == 0)
                return;

            if (dgvCPD.CurrentCell == null)
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

                    if(str.IndexOf("-1") > -1)
                    {
                        continue;
                    }
                    
                    if (selectasnstringlist.IndexOf(str) < 0)
                        selectasnstringlist.Add(str);
                }
                SetSelectFocus(selectasnstringlist);

                //SetASN(ASNNow);
            }

            FillDisplay();
        }

        void AddMover(List<CPDItemClass> newaddcpdlist)
        {
            foreach (CPDItemClass cpd in newaddcpdlist)
            {
                myMoverCollection.Add(cpd.RatioRectEAG);
            }
        }
        /// <summary>
        /// 刪除所選ASN的Mover
        /// </summary>
        /// <param name="deletecpdlist"></param>
        void DeleteMover(List<CPDItemClass> deletecpdlist)
        {
            foreach (CPDItemClass asn in deletecpdlist)
            {
                int i = myMoverCollection.Count - 1;

                while (i > -1)
                {
                    GraphicalObject grpobj = myMoverCollection[i].Source;

                    if ((grpobj as GeoFigure).RelateNo == asn.No)
                    {
                        myMoverCollection.RemoveAt(i);
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
            foreach (CPDItemClass cpd in CPDItemList)
            {
                //int i = 0;
                //Mover mover = cpd.myMover;
                //while (i < mover.Count)
                //{
                //    GraphicalObject grobj = mover[i].Source;

                //    (grobj as GeoFigure).IsSelected = false;
                //    (grobj as GeoFigure).IsFirstSelected = false;

                //    i++;
                //}

                cpd.RatioRectEAG.IsSelected = false;
                cpd.RatioRectEAG.IsFirstSelected = false;
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

            while (i < dtlvCPD.Items.Count)
            {
                string str = dtlvCPD.Items[i].Text;
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
                dtlvCPD.SelectedIndex = -1;
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
                        dtlvCPD.SelectedIndex = FirstSelectIndex;
                    }

                    IsNeedToChange = false;
                    dtlvCPD.Items[foundindex].Selected = true;
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
                dtlvCPD.SelectedIndex = -1;

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

                    dtlvCPD.Items[foundindex].Selected = true;
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

                while (i < dtlvCPD.Items.Count)
                {
                    if (realselectindexlist.IndexOf(i) < 0)
                    {
                        dtlvCPD.Items[i].Selected = false;
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
        /// <param name="cpdlist"></param>
        public void SetCPDTree()
        {
            //Parent 要設定為 0 才會停在根目錄

            CPDTable.Columns.Clear();
            CPDTable.Clear();

            //CPDItemList = cpdlist;

            DataColumn id = new DataColumn("No", typeof(UInt32));               //一定要用UInt32
            DataColumn parentid = new DataColumn("ParentNo", typeof(UInt32));    //一定要用UInt32
            DataColumn cpdname = new DataColumn("Name", typeof(String));

            CPDTable.Columns.Add(id);
            CPDTable.Columns.Add(parentid);
            CPDTable.Columns.Add(cpdname);

            foreach (CPDItemClass cpditem in CPDItemList)
            {
                //if (asn.ASNType == ASNTypeEnum.BRANCH)
                {
                    //AddRowToASNTable(asn);
                    cpditem.AddNewRowToDataTable(CPDTable);
                    //DataRow newdatarow = ASNTable.NewRow();
                    //newdatarow["No"] = asn.No;
                    //newdatarow["ParentNo"] = asn.ParentNo;
                    //newdatarow["Name"] = asn.ToASNString();
                    //newdatarow["Level"] = asn.Level;
                    //ASNTable.Rows.Add(newdatarow);
                }
            }

            dgvCPD.DataSource = CPDTable;
            dgvCPD.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dtlvCPD.AutoGenerateColumns = true;
            dtlvCPD.ShowKeyColumns = false;
            dtlvCPD.ParentKeyAspectName = "ParentNo";
            dtlvCPD.KeyAspectName = "No";
            dtlvCPD.HeaderUsesThemes = false;
            dtlvCPD.UseFilterIndicator = false;
            dtlvCPD.UseFiltering = false;
            dtlvCPD.FullRowSelect = true;
            dtlvCPD.MultiSelect = true;
            dtlvCPD.HideSelection = false;

            dtlvCPD.DataSource = CPDTable;

            dtlvCPD.Columns[0].Width = 240;
            //dtlvASN.Columns[1].Width = 30;
            //dtlvASN.Columns[2].Width = 30;

            dtlvCPD.ExpandAll();

            IsNeedToChange = true;

            IsForward = true;
            FirstSelectNo = -1;
            dtlvCPD.SelectedIndex = -1;
            IsForward = false;
        }

        void CollectMover()
        {
            CollectMover(true);
        }
        void CollectMover(bool ischangeselect)
        {
            myMoverCollection.Clear();

            myMoverCollection.Add(CPD.RangeRectEAG);

            foreach (CPDItemClass cpditem in CPD.CPDItemList)
            {
                int i = 0;

               //Mover mover = cpd.myMover;

                //while (i < mover.Count)
                {
                    //GraphicalObject grobj = mover[i].Source;

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
                        cpditem.RatioRectEAG.IsSelected = false;
                        cpditem.RatioRectEAG.IsFirstSelected = false;
                        //(grobj as GeoFigure).IsSelected = false;
                        //(grobj as GeoFigure).IsFirstSelected = false;
                    }

                    myMoverCollection.Add(cpditem.RatioRectEAG);

                    i++;
                }
            }
        }
        void FillDisplay()
        {
            IsNeedToChange = false;

            if (CPDItemNow != null)
            {
                CPDItemNow.ToAssembleProperty();

                ppgCPD.Enabled = true;
                ppgCPD.SelectedObject = CPDItemNow.ASSEMBLE;
            }
            else
            {
                ppgCPD.Enabled = false;
                ppgCPD.SelectedObject = null;
            }

            IsNeedToChange = true;

            //if (lsbShape.Items.Count > 0)
            //    lsbShape.SelectedIndex = 0;

        }
        public void Suicide()
        {
            DISPUI.Suicide();
            bmpBASE.Dispose();
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
        void DrawRect(Bitmap bmp, Rectangle rect, SolidBrush b)
        {
            Graphics g = Graphics.FromImage(bmp);
            g.FillRectangle(b, rect);
            g.Dispose();
        }

        public void GetCPDBMP()
        {
            CPD.bmpVIEW.Dispose();

            RectangleF rectf = CPD.RangeRectEAG.GetRectF;

            rectf.Intersect(new RectangleF(0, 0, bmpBASE.Width, bmpBASE.Height));

            CPD.bmpVIEW = bmpBASE.Clone(rectf, PixelFormat.Format32bppArgb);
        }

    }
}
