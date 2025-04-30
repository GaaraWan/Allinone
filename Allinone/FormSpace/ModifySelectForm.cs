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
using System.Data.OleDb;
using Allinone.OPSpace.AnalyzeSpace;
using static System.Windows.Forms.ListBox;
using System.Text.RegularExpressions;

namespace Allinone.FormSpace
{
    public partial class ModifySelectForm : Form
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
        string DATACNNSTRING = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=%\\Album.mdb;Jet OLEDB:Database Password=12892414;";
         INSPECTIONClass INSPECTIONPara = new INSPECTIONClass();
        InspectionMethodEnum InspectionOLD = InspectionMethodEnum.NONE;
        //Language Setup
        RcpDBClass RCPDB;
        AccDBClass ACCDB
        {
            get
            {
                return Universal.ACCDB;
            }
        }

        ListBox lstFilter;
        Button btnSelect;
        Button btnExit;
        ComboBox cbBefore;
        ComboBox cbNow;
        ComboBox cbCheckModey;
        NumericUpDown numArea;
        NumericUpDown numCount;
        NumericUpDown numTolerance;

        List<FilterItemClass> FilterItemList = new List<FilterItemClass>();
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
        public ModifySelectForm(RcpDBClass rcpdb)
        {
            InitializeComponent();
            RCPDB = rcpdb.Clone();
            this.Load += RcpSelectForm_Load;
        }
        private void RcpSelectForm_Load(object sender, EventArgs e)
        {
            //myLanguage.Initial(INI.UI_PATH + "\\RcpSelectForm.jdb", INI.LANGUAGE, this);
            JzToolsClass.PassingString = "";
            btnSelect = button1;
            btnSelect.Tag = TagEnum.SELECT;
            cbBefore = comboBox2;
            cbNow = comboBox1;
            cbCheckModey = comboBox3;
            numCount = numericUpDown1;
            numArea = numericUpDown2;
            numTolerance = numericUpDown3;

            btnExit = button6;
            btnExit.Tag = TagEnum.EXIT;

            btnSelect.Click += new EventHandler(btn_Click);
            btnExit.Click += new EventHandler(btn_Click);
            lstFilter = listBox1;

            SearchFilter();

            foreach (InspectionMethodEnum suit in Enum.GetValues(typeof(InspectionMethodEnum)))
            {
                cbBefore.Items.Add(suit);
                cbNow.Items.Add(suit);
            }
            foreach (Inspection_A_B_Enum suit in Enum.GetValues(typeof(Inspection_A_B_Enum)))
            {
                cbCheckModey.Items.Add(suit);
            }
            if (cbCheckModey.Items.Count > 0)
                cbCheckModey.SelectedIndex = 0;

            if (cbBefore.Items.Count > 0)
                cbBefore.SelectedIndex = 0;
            if (cbNow.Items.Count > 2)
                cbNow.SelectedIndex = 2;

            this.CenterToParent();

            JetEazy.BasicSpace.LanguageExClass.Instance.EnumControls(this);
        }
        void btn_Click(object sender, EventArgs e)
        {
            TagEnum KEYS = (TagEnum)((Button)sender).Tag;
            switch (KEYS)
            {
                case TagEnum.SELECT:
                    InspectionOLD= (InspectionMethodEnum)Enum.Parse(typeof(InspectionMethodEnum),cbBefore.Text);
                    INSPECTIONPara.InspectionMethod = (InspectionMethodEnum)Enum.Parse(typeof(InspectionMethodEnum), cbNow.Text);
                    INSPECTIONPara.IBArea =(int) numArea.Value;
                    INSPECTIONPara.IBCount =(int) numCount.Value;
                    INSPECTIONPara.IBTolerance = (int)numTolerance.Value;

                    INSPECTIONPara.InspectionAB= (Inspection_A_B_Enum)Enum.Parse(typeof(Inspection_A_B_Enum),cbCheckModey .Text);

                    int strCount = lstFilter.SelectedItems.Count;
                    string mess = "您确定要修改吗？" + Environment.NewLine
                                + "重启程式即生效。" + Environment.NewLine
                                + "总共选中了" + strCount + "个参数！";

                    JetEazy.FormSpace.MessageForm form = new JetEazy.FormSpace.MessageForm(true, mess);
                    if (form.ShowDialog() == DialogResult.Yes)
                    {
                        JetEazy.FormSpace.MessageForm form2 = new JetEazy.FormSpace.MessageForm("修改中，请稍等...");
                        form2.Show();
                        form2.Refresh();

                        int PassCount = 0;
                        int FailCount = 0;
                        for (int i = 0; i < lstFilter.SelectedItems.Count; i++)
                        {
                            string str = lstFilter.SelectedItems[i].ToString();
                            Regex reg = new Regex(@"str=|\d{5}");
                            Match m = reg.Match(str);
                            string path = Universal.RCPPATH + "\\" + m.Value;

                              bool isupdate= Change(path);
                            if (isupdate)
                                PassCount++;
                            else
                                FailCount++;
                        }
                        form2.Close();

                        string strMess = "修改已完成,立即请重启程式!" + Environment.NewLine;
                        strMess += "选中: " + strCount + Environment.NewLine;
                        strMess += "成功: " + PassCount + Environment.NewLine;
                        strMess += "失败: " + FailCount + Environment.NewLine;

                        MessageBox.Show(strMess, "SYS", MessageBoxButtons.OK);
                        this.Close();
                    }
                    break;
                case TagEnum.EXIT:
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    break;
            }
        }
        void SearchFilter()
        {
            string FilterStr = "XQ4*";
            FilterItemList.Clear();

            foreach (RcpClass rcp in RCPDB.myDataList)
            {
                if ((FilterStr == "XQ4*" || rcp.CheckFilter(FilterStr)) && rcp.No != 0)
                    FilterItemList.Add(new FilterItemClass(rcp));
            }

            FilterItemList.Sort(CompareSortStr);
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
            if (lstFilter.Items.Count > 0)
                lstFilter.SelectedIndex = 0;
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

            //string fromitemTemp = fromitem.RcpIndexString;
            //string fromitemTemp2 = toitem.RcpIndexString;
            //return fromitemTemp.CompareTo(fromitemTemp2);
        }

        /// <summary>
        /// 改变
        /// </summary>
        /// <param name="path">数据库地址</param>
        /// <returns>是否修改成功</returns>
        bool Change(string path)
        {
            string ConnectionString = DATACNNSTRING.Replace("%", path);
            string sql = "select * from ENVDB where No=0";
            //获取表1中昵称为LanQ的内容
            OleDbDataAdapter dbDataAdapter = new OleDbDataAdapter(sql, ConnectionString); //创建适配对象
            DataTable envtbl = new DataTable(); //新建表对象
            dbDataAdapter.Fill(envtbl); //用适配对象填充表对象
            dbDataAdapter.Dispose();
            if (envtbl.Rows.Count > 0)
            {
                string strSaveData = "";
                if (envtbl.Rows[0]["EnvData"] != DBNull.Value)
                {
                    DataRow envrow = envtbl.Rows[0];
                    string analyzestr = (envrow["AnalyzeData"] == DBNull.Value ? "" : (string)envrow["AnalyzeData"]);

                    string[] analyzes = analyzestr.Replace(Environment.NewLine, Universal.NewlineChar.ToString()).Split(Universal.NewlineChar);

                    //取得所有正常的ANALYZE
                    if (analyzestr.Length > 0)
                    {
                        for (int iTemp=0;iTemp<  analyzes.Length;iTemp++)
                        {
                            string[] strsTemp = analyzes[iTemp].Split(Universal.SeperateCharA);
                            //if (strsTemp.Length < 5)
                            //    continue;
                            string str = strsTemp[2];
                            string[] strs = str.Split(Universal.SeperateCharB);
                            InspectionMethodEnum InspectionMethod = (InspectionMethodEnum)int.Parse(strs[0]);
                            int IBCount = int.Parse(strs[1]);
                            int IBArea = int.Parse(strs[2]);
                            int IBTolerance = int.Parse(strs[3]);

                            Inspection_A_B_Enum InspectionAB = Inspection_A_B_Enum.AB;
                            if (strs.Length > 3)
                            {
                              if( strs[4]=="")
                                InspectionAB = Inspection_A_B_Enum.AB;
                              else
                                    InspectionAB = (Inspection_A_B_Enum)int.Parse(strs[4]);
                            }
                            if (InspectionMethod== InspectionOLD)
                            {
                                InspectionMethod = INSPECTIONPara.InspectionMethod;
                                InspectionAB = INSPECTIONPara.InspectionAB;
                                if (INSPECTIONPara.IBArea != -1)
                                    IBArea = INSPECTIONPara.IBArea;
                                if (INSPECTIONPara.IBCount != -1)
                                    IBCount = INSPECTIONPara.IBCount;
                                if (INSPECTIONPara.IBTolerance != -1)
                                    IBTolerance = INSPECTIONPara.IBTolerance;
                            }
                            for (int i = 0; i < strsTemp.Length; i++)
                            {
                                if (i != 2)
                                    strSaveData += strsTemp[i] + Universal.SeperateCharA;
                                else
                                {
                                    int iInspection = (int)InspectionMethod;
                                    strSaveData += iInspection.ToString() + Universal.SeperateCharB;
                                    strSaveData += IBCount.ToString() + Universal.SeperateCharB;
                                    strSaveData += IBArea.ToString() + Universal.SeperateCharB;
                                    strSaveData += IBTolerance.ToString() + Universal.SeperateCharB;
                                    int iInspectionab = (int)InspectionAB;
                                    strSaveData += iInspectionab.ToString() + Universal.SeperateCharB  + Universal.SeperateCharA;
                                }
                            }
                            if (iTemp != (analyzes.Length - 1))
                                strSaveData += Universal.NewlineChar;
                        }
                       
                    }
                }
                envtbl.Dispose();

                string sqlUpdate = "update ENVDB set AnalyzeData='"+ strSaveData + "' where No=0";
                OleDbConnection oleDb = new OleDbConnection(ConnectionString);
                oleDb.Open();
                //将表1中昵称为东熊的账号修改成233333
                OleDbCommand oleDbCommand = new OleDbCommand(sqlUpdate, oleDb);
                int iupdate = oleDbCommand.ExecuteNonQuery();
                oleDb.Close();
                oleDbCommand.Dispose();
                if (iupdate > 0)
                    return true;
            }
            return false;
           
        }

    }
}
