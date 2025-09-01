using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using JetEazy.DBSpace;
using JetEazy.BasicSpace;
using JetEazy.UISpace;
using JetEazy.ControlSpace;

using JzDisplay.UISpace;
using Allinone.OPSpace;

namespace Allinone.FormSpace
{
    enum TagEnum
    {
        SELECT,
        DEL,

        EXIT,
    }

    public partial class RcpSelectForm : Form
    {
        class FilterItemClass
        {
            public int Index = -1;
            public string NameStr = "";
            public string VersionStr = "";
            public string SortStr = "";
            public string Comment = "";

            public string ModifyStr = "";
            public string RcpIndexString = "";

            public FilterItemClass()
            {

            }
            public FilterItemClass(RcpClass rcp)
            {
                Index = rcp.No;
                NameStr = rcp.Name + "(" + rcp.Version + ")";
                SortStr = NameStr.ToUpper();
                Comment = rcp.Remark;
                ModifyStr = rcp.ToModifyString();
                RcpIndexString = rcp.RcpNoString;
            }
            public string ToString(string str1, string str2, string str3)
            {
                string Str = "";

                if (Index != -1)
                {
                    Str += str1 + "[" + Index.ToString(RcpClass.ORGRCPNOSTRING) + "]  " + NameStr + Environment.NewLine + Environment.NewLine;
                    Str += str2 + ModifyStr + Environment.NewLine;
                    Str += str3 + Comment;
                }

                return Str;
            }
        }

        //AlbumCollectionClass AlbumCollection
        //{
        //    get
        //    {
        //        return Universal.ALBCollection;
        //    }
        //}

        bool IsExecuteDel = false;
        int m_CurrentIndex = 0;

        //Language Setup

        RcpDBClass RCPDB;
        AccDBClass ACCDB
        {
            get
            {
                return Universal.ACCDB;
            }
        }

        Label lblInformation;
        TextBox txtSearch;
        ListBox lstFilter;

        Button btnSelect;
        Button btnDel;

        Button btnExit;

        DispUI DISPUI;
        string ShowBmpString;

        JzToolsClass myJzTools = new JzToolsClass();

        bool IsNeedToChange = false;

        List<FilterItemClass> FilterItemList = new List<FilterItemClass>();
        public Rectangle ParentWindosw { private get; set; } = new Rectangle(0, 0, 0, 0);
        public RcpSelectForm(RcpDBClass rcpdb, string showbmpstring)
        {
            InitializeComponent();
            Initial(rcpdb, showbmpstring);

        }
        void Initial(RcpDBClass rcpdb, string showbmpstring)
        {
            //myLanguage.Initial(INI.UI_PATH + "\\RcpSelectForm.jdb", INI.LANGUAGE, this);

            //OpDisplay = new OpDisplayNormal(dispUI1);
            //OpDisplay.Initial();
            IsExecuteDel = false;
            RCPDB = rcpdb.Clone();

            JzToolsClass.PassingString = "";
            m_CurrentIndex = RCPDB.DataNow.No;

            DISPUI = dispUI1;
            DISPUI.Initial();

            ShowBmpString = showbmpstring;

            btnSelect = button1;
            btnSelect.Tag = TagEnum.SELECT;
            btnDel = button4;
            btnDel.Tag = TagEnum.DEL;
            btnExit = button6;
            btnExit.Tag = TagEnum.EXIT;

            btnSelect.Click += new EventHandler(btn_Click);
            btnDel.Click += new EventHandler(btn_Click);
            btnExit.Click += new EventHandler(btn_Click);

            txtSearch = textBox1;
            txtSearch.TextChanged += new EventHandler(txtSearch_TextChanged);

            lstFilter = listBox1;
            lstFilter.SelectedIndexChanged += new EventHandler(lstFilter_SelectedIndexChanged);

            this.Load += RcpSelectForm_Load;
            this.FormClosing += new FormClosingEventHandler(RcpSelectForm_FormClosing);

            lblInformation = label3;

            JetEazy.BasicSpace.LanguageExClass.Instance.EnumControls(this);
        }

        private void RcpSelectForm_Load(object sender, EventArgs e)
        {
            SearchFilter();

            if (ParentWindosw.Width != 0 && ParentWindosw.Height != 0)
            {
                int ix = ParentWindosw.Width - this.Width + ParentWindosw.X;
                int iy = (ParentWindosw.Height - this.Height) / 2 + ParentWindosw.Y;
                this.Location = new Point(ix, iy);
            }
        }

        void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SearchFilter();
        }

        void btn_Click(object sender, EventArgs e)
        {
            TagEnum KEYS = (TagEnum)((Button)sender).Tag;

            switch (KEYS)
            {
                case TagEnum.SELECT:

                    JzToolsClass.PassingInteger = FilterItemNow.Index;

                    if (JzToolsClass.PassingString != "")
                        JzToolsClass.PassingString = myJzTools.RemoveLastChar(JzToolsClass.PassingString, 1);

                    ProgramClose(true);

                    break;
                case TagEnum.DEL:
                    if (lstFilter.SelectedIndices.Count > 0)
                    {
                        if (MessageBox.Show(ToChangeLanguage("是否要刪除此筆資料?"), "SYS", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        //if(MessageBox.Show(myLanguage.Messages("msg1",INI.LANGUAGE),"SYS",MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            Delete();
                        }
                    }
                    break;
                case TagEnum.EXIT:

                    if (IsExecuteDel)
                    {
                        JzToolsClass.PassingInteger = m_CurrentIndex;// RCPDB.DataNow.No;// FilterItemNow.Index;

                        if (JzToolsClass.PassingString != "")
                            JzToolsClass.PassingString = myJzTools.RemoveLastChar(JzToolsClass.PassingString, 1);

                        ProgramClose(true);
                    }
                    else
                        ProgramClose(false);

                    break;
            }
        }


        void Delete()
        {
            string FilterDelStr = "";
            string[] Delstrs;
            int i = 0;

            while (i < lstFilter.Items.Count)
            {
                if (lstFilter.GetSelected(i))
                {
                    if (FilterItemList[i].Index != 0)
                        FilterDelStr += FilterItemList[i].Index.ToString() + ",";
                }
                i++;
            }

            if (FilterDelStr == "")
                return;

            FilterDelStr = myJzTools.RemoveLastChar(FilterDelStr, 1);
            Delstrs = FilterDelStr.Split(',');

            //FilterDelStr = "";
            //foreach (string str in Delstrs)
            //{
            //    int DelIndex = int.Parse(str);
            //    i = 0;
            //    foreach (RcpClass RCPItem in RCPDB.myDataList)
            //    {
            //        if (RCPItem.Index == 0)
            //        {
            //            i++;
            //            continue;
            //        }
            //        if (RCPItem.Index == DelIndex)
            //        {
            //            FilterDelStr += i.ToString() + ",";
            //        }
            //        i++;
            //    }
            //}

            //FilterDelStr = myJzTools.RemoveLastChar(FilterDelStr, 1);
            //Delstrs = FilterDelStr.Split(',');

            i = Delstrs.Length - 1;

            while (i > -1)
            {
                //Add For Avoid Delete Static No
                if (int.Parse(Delstrs[i]) >= Universal.StaticStartNo)
                {
                    i--;
                    continue;
                }

                int indicator = RCPDB.FindIndex(int.Parse(Delstrs[i]));

                if (indicator > -1)
                    RCPDB.myDataList.RemoveAt(indicator);

                JzToolsClass.PassingString += Delstrs[i] + ",";

                i--;
            }


            //if (JzToolsClass.PassingString != "")
            //{
            //    string[] RemoveIndexStr = JzToolsClass.PassingString.Split(',');

            //    foreach (string str in RemoveIndexStr)
            //        if (!string.IsNullOrEmpty(str))
            //            RCPDB.Delete(RCPDB.FindIndex(int.Parse(str)));
            //}

            //AlbumCollection.Del(RCPDB);

            //Universal.BackupDATADB();
            //RCPDB.Save();

            IsExecuteDel = true;
            SearchFilter();
        }

        void SearchFilter()
        {
            string FilterStr = "XQ4*";

            if (txtSearch.Text.Trim() != "")
            {
                FilterStr = txtSearch.Text.ToUpper();
            }

            FilterItemList.Clear();

            foreach (RcpClass rcp in RCPDB.myDataList)
            {
                if ((FilterStr == "XQ4*" || rcp.CheckFilter(FilterStr)) && rcp.No != 0)
                {
                    FilterItemList.Add(new FilterItemClass(rcp));
                }
            }

            FilterItemList.Sort(CompareSortStr);

            IsNeedToChange = false;

            lstFilter.Items.Clear();

            foreach (FilterItemClass FilterItem in FilterItemList)
            {
                int start = FilterItem.SortStr.IndexOf('(') + 1;
                int stop = FilterItem.SortStr.Length - 1;
                int length = stop - start;
                string fromitemTemp = "(" + FilterItem.SortStr.Substring(start, length) + ")";
                string name = FilterItem.SortStr.Substring(0, start - 1);

                fromitemTemp = fromitemTemp.PadRight(7, ' ');

                lstFilter.Items.Add("[" + FilterItem.Index.ToString(RcpClass.ORGRCPNOSTRING) + "] " + fromitemTemp + name);
            }

            IsNeedToChange = true;

            if (lstFilter.Items.Count > 0)
                lstFilter.SelectedIndex = 0;
            else
                FillDisplay();
        }

        int CompareSortStr(FilterItemClass fromitem, FilterItemClass toitem)
        {
            int start = fromitem.SortStr.IndexOf('(') + 1;
            int stop = fromitem.SortStr.Length - 1;
            int length = stop - start;
            string fromitemTemp = fromitem.SortStr.Substring(start, length);

            int start2 = toitem.SortStr.IndexOf('(') + 1;
            int stop2 = toitem.SortStr.Length - 1;
            int length2 = stop2 - start2;
            string fromitemTemp2 = toitem.SortStr.Substring(start2, length2);

            return fromitemTemp.CompareTo(fromitemTemp2);
            // return (fromitem.SortStr.CompareTo(toitem.SortStr));
        }

        void lstFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsNeedToChange)
            {
                FillDisplay();
            }
        }

        FilterItemClass FilterNull = new FilterItemClass();
        FilterItemClass FilterItemNow
        {
            get
            {
                if (lstFilter.SelectedIndex < 0)
                    return FilterNull;
                else
                    return FilterItemList[lstFilter.SelectedIndex];
            }
        }

        void FillDisplay()
        {

            try
            {
                //lblInformation.Text = FilterItemNow.ToString(myLanguage.Messages("msg2", INI.LANGUAGE),
                //                            myLanguage.Messages("msg3", INI.LANGUAGE),
                //                            myLanguage.Messages("msg4", INI.LANGUAGE));

                lblInformation.Text = FilterItemNow.ToString(ToChangeLanguage("編　　號"),
                                            ToChangeLanguage("修改日期"),
                                            ToChangeLanguage("其他說明"));

                //OpDisplay.SetDispImage(RCPDB.GetRCPItemBmp(FilterItemNow.Index));

                btnDel.Enabled = (INI.PRELOADSTATICNO.IndexOf(FilterItemNow.Index.ToString(RcpClass.ORGRCPNOSTRING)) < 0
                                && (FilterItemNow.Index != 1)
                                && (FilterItemNow.Index != JzToolsClass.PassingInteger)
                                && ACCDB.DataNow.AllowSetupRecipe);

                DISPUI.SetDisplayImage(Universal.RCPPATH + "\\" + FilterItemNow.RcpIndexString + "\\" + ShowBmpString);

            }
            catch (Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
                MessageBox.Show(ToChangeLanguage("無此參數"));
            }

        }
        void ProgramClose(bool IsSelected)
        {
            //Should Release Data
            DISPUI.Suicide();

            if (IsSelected)
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            else
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
        void RcpSelectForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //OpDisplay.Kill();
        }

        string ToChangeLanguage(string eText)
        {
            string retStr = eText;
            retStr = LanguageExClass.Instance.GetLanguageText(eText);
            return retStr;
        }


    }
}
