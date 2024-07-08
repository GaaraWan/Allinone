using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//using Microsoft.Office.Core;
//using Excel = Microsoft.Office.Interop.Excel;

using System.IO;

using JetEazy;
using JetEazy.DBSpace;
using JetEazy.BasicSpace;

namespace JetEazy.FormSpace
{
    public partial class ExportForm : Form
    {
        enum KeyCapLocationEnum : int
        {
            LEFT1 = 0,
            LEFT2 = 1,
            TOP1 = 2,
            TOP2 = 3,
            RIGHT1 = 4,
            RIGHT2 = 5,
            BOTTOM1 = 6,
            BOTTOM2 = 7,
        }


        enum TagEnum : int
        {
            COUNT = 1,
            REPORT1 = 0,
        }

        ListBox lstDB;
        TreeView trvDetail;

        Button btnExport;
        Button btnDelete;
        Button btnCancel;
        
        RadioButton[] rdoReport = new RadioButton[(int)TagEnum.COUNT];

        OleDbConnection Mycn;
        OleDbDataAdapter da;
        OleDbCommandBuilder cb;
        DataTable tbl = new DataTable();

        TagEnum ReportType = TagEnum.REPORT1;

        object misValue = System.Reflection.Missing.Value;

        string SQLStr = "";
        string LOGTXTPathString = "";
        string LOGDBPathString = "";

        List<KeyboardDataClass> KeyboardDataList = new List<KeyboardDataClass>();

        class KeyboardDataClass
        {
            public int kpFillCount = 0;

            public string Name = "";

            public List<KeycapDataClass> KeycapDataList = new List<KeycapDataClass>();

            public int KeycapCount
            {
                get
                {
                    return KeycapDataList.Count;
                }
            }

            public KeyboardDataClass(string readpath, string rname)    //將csv文檔內容傳入KeyboardDataClass類中
            {
                kpFillCount = 0;

                Name = rname;

                StreamReader sr = new StreamReader(readpath);

                string readstr = sr.ReadLine();

                int no =0;

                while (!sr.EndOfStream)
                {
                    string[] strs = readstr.Split(',');

                    if (int.TryParse(strs[0], out no))
                    {
                        KeycapDataClass keycap = new KeycapDataClass();

                        keycap.Index = no;
                        keycap.Name = strs[1];
                        keycap.data[(int)KeyCapLocationEnum.LEFT1] = float.Parse( strs[3]);

                        readstr = sr.ReadLine();
                        strs = readstr.Split(',');
                        keycap.data[(int)KeyCapLocationEnum.LEFT2] = float.Parse(strs[3]);

                        readstr = sr.ReadLine();
                        strs = readstr.Split(',');
                        keycap.data[(int)KeyCapLocationEnum.TOP1] = float.Parse(strs[3]);

                        readstr = sr.ReadLine();
                        strs = readstr.Split(',');
                        keycap.data[(int)KeyCapLocationEnum.TOP2] = float.Parse(strs[3]);

                        readstr = sr.ReadLine();
                        strs = readstr.Split(',');
                        keycap.data[(int)KeyCapLocationEnum.RIGHT1] = float.Parse(strs[3]);

                        readstr = sr.ReadLine();
                        strs = readstr.Split(',');
                        keycap.data[(int)KeyCapLocationEnum.RIGHT2] = float.Parse(strs[3]);

                        readstr = sr.ReadLine();
                        strs = readstr.Split(',');
                        keycap.data[(int)KeyCapLocationEnum.BOTTOM1] = float.Parse(strs[3]);

                        readstr = sr.ReadLine();
                        strs = readstr.Split(',');
                        keycap.data[(int)KeyCapLocationEnum.BOTTOM2] = float.Parse(strs[3]);

                        KeycapDataList.Add(keycap);

                        kpFillCount += keycap.DataContain;
                    }

                    readstr = sr.ReadLine();
                }

                sr.Close();
                sr.Dispose();
            }
        }
        class KeycapDataClass
        {
            public int DataContain = 8; 

            public int Index = 0;
            public string Name = "";
            
            public float [] data = new float[8];

            public KeycapDataClass()
            {

            }
        }

        public ExportForm(string logdbpathstring,string logtxtpathstrign)
        {
            LOGDBPathString = logdbpathstring;
            LOGTXTPathString = logtxtpathstrign;

            InitializeComponent();
            Initial();

        }

        void Initial()
        {
            lstDB = listBox1;
            trvDetail = treeView1;

            btnExport = button1;
            btnCancel = button2;
            btnDelete = button3;

            rdoReport[(int)TagEnum.REPORT1] = radioButton1;
            rdoReport[(int)TagEnum.REPORT1].Tag = TagEnum.REPORT1;
            rdoReport[(int)TagEnum.REPORT1].CheckedChanged += new EventHandler(rdo_CheckedChanged);
         
            lstDB.SelectedIndexChanged += new EventHandler(lstDB_SelectedIndexChanged);
            trvDetail.AfterSelect += new TreeViewEventHandler(trvDetail_AfterSelect);

            FindDirectoryDB();

            btnExport.Click += new EventHandler(btnExport_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
            btnDelete.Click += new EventHandler(btnDelete_Click);

            if (lstDB.Items.Count > 0)
                lstDB.SelectedIndex = 0;

            //CreateExcelFile();
        }

        void rdo_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rdo = (RadioButton)sender;

            ReportType = (TagEnum)rdo.Tag;
        }
        
        void btnDelete_Click(object sender, EventArgs e)
        {
            if (trvDetail.SelectedNode == null)
                return;


            if (MessageBox.Show("是否要刪除此項資料?", "SYS", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                SQLStr = "DELETE FROM logdb WHERE log01 = '" + trvDetail.SelectedNode.FullPath.Split('\\')[0] + "/" + trvDetail.SelectedNode.FullPath.Split('\\')[1].Substring(trvDetail.SelectedNode.FullPath.Split('\\')[1].Length - 2, 2) + "'" +  //AND log03  like '" + trvDetail.SelectedNode.Text.PadRight(20) + ",%'";
                " AND (log03  like '" + trvDetail.SelectedNode.Text.PadRight(20) + ",%'" +
                " OR log03  like '" + trvDetail.SelectedNode.Text + ",%')";

                Mycn.Open();

                OleDbCommand cmdtmp = new OleDbCommand(SQLStr, Mycn);

                cmdtmp.ExecuteNonQuery();

                cmdtmp.Dispose();

                Mycn.Close();

            }
            
            lstDB.SelectedIndex = -1;
            lstDB.SelectedIndex = 0;
        }
        void trvDetail_AfterSelect(object sender, TreeViewEventArgs e)
        {

            if (trvDetail.SelectedNode.Level == 3)
            {
                SQLStr = "SELECT * FROM logdb WHERE log01 = '" + trvDetail.SelectedNode.FullPath.Split('\\')[0] + "/" + trvDetail.SelectedNode.FullPath.Split('\\')[1].Substring(trvDetail.SelectedNode.FullPath.Split('\\')[1].Length - 2, 2) + "'" + 
                " AND log03  like '" + trvDetail.SelectedNode.Parent.Text.PadRight(20) + "," + trvDetail.SelectedNode.Text.PadRight(20) + ",%'" + 
                " ORDER BY log01,log02,log03";

                btnExport.Enabled = true;
                btnDelete.Enabled = true;
            }
            else
            {
                btnDelete.Enabled = false;
                btnExport.Enabled = false;
            }
        }
        void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
        void btnExport_Click(object sender, EventArgs e)
        {
            btnExport.Enabled = false;


            //MSGFRM = new MessageForm("資料產生中，請稍候。", true);
            //MSGFRM.Show();
            //MSGFRM.Refresh();

            KeyboardDataList.Clear(); 

            string FileNameString = "";
                  
            string SRead = "";
          
            da = new OleDbDataAdapter(SQLStr, Mycn);
            cb = new OleDbCommandBuilder(da);
            tbl.Clear();                     
            da.Fill(tbl);

            FileNameString = "Report " + JzTimes.DateTimeSerialString + ".csv";  //文檔名


            int i = 0;
            int LastKeycapCount = -1;       

            i=0;

            foreach (DataRow row in tbl.Rows)
            {
                string FilePathName = LOGTXTPathString + "\\" + lstDB.Text + "\\" + trvDetail.SelectedNode.Parent.Text + "\\" + row["log04"].ToString() + ".csv";

                if (File.Exists(FilePathName))      //獲取tree對應的LOGTXT中的csv文檔
                {
                    KeyboardDataClass keyboarddata = new KeyboardDataClass(FilePathName, row["log03"].ToString().Split(',')[2]);

                    if (LastKeycapCount == -1)
                    {
                        LastKeycapCount = keyboarddata.KeycapCount;
                    }
                    else
                    {
                        if (LastKeycapCount != keyboarddata.KeycapCount)
                            continue;
                    }

                    KeyboardDataList.Add(keyboarddata);

                    i++;
                }
            }

            if (KeyboardDataList.Count > 0)
            {
                //CreateExcelFile(ReportType);
                //MessageBox.Show("產生檔案於 " + DestFileName, "MAIN", MessageBoxButtons.OK);
            }

            btnExport.Enabled = true;

            //this.Close();
        }
        void lstDB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDB.SelectedIndex > -1)
            {
                string ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + LOGDBPathString + "\\Template.MDB";

                ConnectionString = ConnectionString.Replace("Template", "L" + lstDB.Text);
                Mycn = new OleDbConnection(ConnectionString);

                da = new OleDbDataAdapter("SELECT * FROM logdb ORDER BY log01,log03", Mycn);
                cb = new OleDbCommandBuilder(da);

                tbl.Clear();

                da.Fill(tbl);
                RefreshTree();
            }

        }

        void RefreshTree()
        {
            string SMonth, SDate, SPKey, SPRecipe;
            string LastSMonth, LastSDate, LastSPKey, LastSPRecipe;

            int i = 0;

            TreeNode MothNode;
            TreeNode DateNode;
            TreeNode ProductNode;
            TreeNode RecipeNode;

            trvDetail.Nodes.Clear();

            if (tbl.Rows.Count > 0)
            {
                SMonth = tbl.Rows[0]["log01"].ToString().Substring(0, 7);
                SDate = tbl.Rows[0]["log01"].ToString().Substring(8, 2);
                SPKey = tbl.Rows[0]["log03"].ToString().Split(',')[0].Trim();
                SPRecipe = tbl.Rows[0]["log03"].ToString().Split(',')[1].Trim();

                MothNode = trvDetail.Nodes.Add(SMonth);
                DateNode = MothNode.Nodes.Add(SMonth.Substring(SMonth.Length - 2, 2) + "/" + SDate);
                ProductNode = DateNode.Nodes.Add(SPKey);
                RecipeNode = ProductNode.Nodes.Add(SPRecipe);

                ProductNode.Tag = tbl.Rows[0]["log01"].ToString() + "@" + tbl.Rows[0]["log03"].ToString().Split(',')[0].Trim() + "@" + tbl.Rows[0]["log03"].ToString().Split(',')[1].Trim();

                LastSMonth = SMonth;
                LastSDate = SDate;
                LastSPKey = SPKey;
                LastSPRecipe = SPRecipe;

                i = 1;
                while (i < tbl.Rows.Count)
                {
                    SMonth = tbl.Rows[i]["log01"].ToString().Substring(0, 7);
                    SDate = tbl.Rows[i]["log01"].ToString().Substring(8, 2);
                    SPKey = tbl.Rows[i]["log03"].ToString().Split(',')[0].Trim();
                    SPRecipe = tbl.Rows[i]["log03"].ToString().Split(',')[1].Trim();

                    if (SMonth != LastSMonth)
                    {
                        MothNode = trvDetail.Nodes.Add(SMonth);
                        DateNode = MothNode.Nodes.Add(SMonth.Substring(SMonth.Length - 2, 2) + "/" + SDate);
                        ProductNode = DateNode.Nodes.Add(SPKey);
                        RecipeNode = ProductNode.Nodes.Add(SPRecipe);

                        ProductNode.Tag = tbl.Rows[i]["log01"].ToString() + "@" + tbl.Rows[i]["log03"].ToString().Split(',')[0].Trim() + "@" + tbl.Rows[i]["log03"].ToString().Split(',')[1].Trim();
                    }
                    else if (SDate != LastSDate)
                    {
                        DateNode = MothNode.Nodes.Add(SMonth.Substring(SMonth.Length - 2, 2) + "/" + SDate);
                        ProductNode = DateNode.Nodes.Add(SPKey);
                        RecipeNode = ProductNode.Nodes.Add(SPRecipe);

                        ProductNode.Tag = tbl.Rows[i]["log01"].ToString() + "@" + tbl.Rows[i]["log03"].ToString().Split(',')[0].Trim() + "@" + tbl.Rows[i]["log03"].ToString().Split(',')[1].Trim();
                    }
                    else if (SPKey != LastSPKey)
                    {
                        ProductNode = DateNode.Nodes.Add(SPKey);
                        RecipeNode = ProductNode.Nodes.Add(SPRecipe);
                        ProductNode.Tag = tbl.Rows[i]["log01"].ToString() + "@" + tbl.Rows[i]["log03"].ToString().Split(',')[0].Trim() + "@" + tbl.Rows[i]["log03"].ToString().Split(',')[1].Trim();
                    }
                    else if (SPRecipe != LastSPRecipe)
                    {
                        RecipeNode = ProductNode.Nodes.Add(SPRecipe);
                        ProductNode.Tag = tbl.Rows[i]["log01"].ToString() + "@" + tbl.Rows[i]["log03"].ToString().Split(',')[0].Trim() + "@" + tbl.Rows[i]["log03"].ToString().Split(',')[1].Trim();
                    }


                    LastSMonth = SMonth;
                    LastSDate = SDate;
                    LastSPKey = SPKey;
                    LastSPRecipe = SPRecipe;

                    i++;
                }
                trvDetail.SelectedNode = RecipeNode;
            }
        }
        void FindDirectoryDB()
        {
            string[] Dir = Directory.GetFiles(LOGDBPathString, "L*.MDB");

            string Str = "";
            string[] Strs;
            int i = 0;
            while (i < Dir.Length)
            {
                Str = Dir[i].Split('.')[0];

                Strs = Str.Split('\\');

                lstDB.Items.Add(Strs[Strs.Length - 1].Remove(0, 1));
                i++;
            }
        }

        #region Reports
        
        /*
        Excel.Application myExcelApp;
        Excel.Workbooks myExcelWorkBooks;
        Excel.Workbook myExcelWorkBook;
        Excel.Range myExcelRange;

        MessageForm MSGFRM;
        string DataHeader = "";

        string DestFileName = "";

        void CreateExcelFile(TagEnum RptType)
        {
            string DestString = JzTimes.DateTimeSerialString;

            myExcelApp = new Excel.ApplicationClass();
            //myExcelApp.Visible = true;

            myExcelWorkBooks = myExcelApp.Workbooks;

            string SourceFileName = INI.LOGDB_PATH + "\\Template.xls";
            DestFileName = SourceFileName.Replace("LOGDB", "REPORT").Replace("Template", "Normal Report " + DestString);

            File.Copy(SourceFileName, DestFileName);
            myExcelWorkBook = myExcelWorkBooks.Open(DestFileName, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue);
            switch (RptType)
            {
                case TagEnum.REPORT1:
                    CreateReport1(myExcelWorkBook);
                    break;
            }
            myExcelWorkBook.Save();

            myExcelApp.Visible = true;

            MSGFRM.Close();
        }

        void CreateReport1(Excel.Workbook ExcelWorkBook)
        {
            if (KeyboardDataList.Count < 1)
                return;

            int PageCount = 200;

            int BaseIndex = 4;
            int ContainCount = 4;

            int i = 0, j = 0, k = 0;
            int BiasIndex = 0;

            int ShiftIndex = 0;
            int SpaceIndex = 0;

            Excel.Worksheet myExcelWorksheet = (Excel.Worksheet)ExcelWorkBook.Sheets.Add(misValue, misValue, misValue, misValue);

            ((Excel.Worksheet)ExcelWorkBook.Sheets["Template"]).Delete();

            myExcelWorksheet.Name = "001";

            //myExcelApp.Visible = true;

            //產生表頭

            myExcelRange = myExcelWorksheet.get_Range(myExcelWorksheet.Cells[2, 1], myExcelWorksheet.Cells[3, 3]);
            myExcelRange.Merge(misValue);
            myExcelRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            myExcelRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            myExcelRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin, Excel.XlColorIndex.xlColorIndexAutomatic, misValue);

            myExcelRange.Formula = INI.MACHINE_NAME;

            BaseIndex = 4;

            KeyboardDataClass kbdata = KeyboardDataList[0];

            foreach (KeycapDataClass kpdata in kbdata.KeycapDataList)
            {
                //產生Index欄位
                ContainCount = kpdata.DataContain;

                BiasIndex = ContainCount * i + BaseIndex + ShiftIndex;

                myExcelRange = myExcelWorksheet.get_Range(myExcelWorksheet.Cells[BiasIndex, 1], myExcelWorksheet.Cells[BiasIndex + ContainCount - 1, 1]);
                myExcelRange.Merge(misValue);
                myExcelRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                myExcelRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                myExcelRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin, Excel.XlColorIndex.xlColorIndexAutomatic, misValue);

                myExcelRange.Formula = (kpdata.Index).ToString();

                //產生鍵帽名稱欄位
                BiasIndex = ContainCount * i + BaseIndex + ShiftIndex;

                myExcelRange = myExcelWorksheet.get_Range(myExcelWorksheet.Cells[BiasIndex, 2], myExcelWorksheet.Cells[BiasIndex + ContainCount - 1, 2]);
                myExcelRange.Merge(misValue);
                myExcelRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                myExcelRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                myExcelRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin, Excel.XlColorIndex.xlColorIndexAutomatic, misValue);

                myExcelRange.Formula = kpdata.Name;

                ((Excel.Range)myExcelWorksheet.Columns[3, misValue]).ColumnWidth = 15;

                BiasIndex = ContainCount * i + BaseIndex + ShiftIndex;


                //產生角落資料表頭
                myExcelRange = myExcelWorksheet.get_Range(myExcelWorksheet.Cells[BiasIndex, 3], myExcelWorksheet.Cells[BiasIndex, 3]);
                myExcelRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                myExcelRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                myExcelRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin, Excel.XlColorIndex.xlColorIndexAutomatic, misValue);
                myExcelRange.Formula = "Left1";

                myExcelRange = myExcelWorksheet.get_Range(myExcelWorksheet.Cells[BiasIndex + 1, 3], myExcelWorksheet.Cells[BiasIndex + 1, 3]);
                myExcelRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                myExcelRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                myExcelRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin, Excel.XlColorIndex.xlColorIndexAutomatic, misValue);
                myExcelRange.Formula = "Left2";

                myExcelRange = myExcelWorksheet.get_Range(myExcelWorksheet.Cells[BiasIndex + 2, 3], myExcelWorksheet.Cells[BiasIndex + 2, 3]);
                myExcelRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                myExcelRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                myExcelRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin, Excel.XlColorIndex.xlColorIndexAutomatic, misValue);
                myExcelRange.Formula = "Top1";

                myExcelRange = myExcelWorksheet.get_Range(myExcelWorksheet.Cells[BiasIndex + 3, 3], myExcelWorksheet.Cells[BiasIndex + 3, 3]);
                myExcelRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                myExcelRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                myExcelRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin, Excel.XlColorIndex.xlColorIndexAutomatic, misValue);
                myExcelRange.Formula = "Top2";

                myExcelRange = myExcelWorksheet.get_Range(myExcelWorksheet.Cells[BiasIndex + 4, 3], myExcelWorksheet.Cells[BiasIndex + 4, 3]);
                myExcelRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                myExcelRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                myExcelRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin, Excel.XlColorIndex.xlColorIndexAutomatic, misValue);
                myExcelRange.Formula = "Right1";

                myExcelRange = myExcelWorksheet.get_Range(myExcelWorksheet.Cells[BiasIndex + 5, 3], myExcelWorksheet.Cells[BiasIndex + 5, 3]);
                myExcelRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                myExcelRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                myExcelRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin, Excel.XlColorIndex.xlColorIndexAutomatic, misValue);
                myExcelRange.Formula = "Right2";

                myExcelRange = myExcelWorksheet.get_Range(myExcelWorksheet.Cells[BiasIndex + 6, 3], myExcelWorksheet.Cells[BiasIndex + 6, 3]);
                myExcelRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                myExcelRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                myExcelRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin, Excel.XlColorIndex.xlColorIndexAutomatic, misValue);
                myExcelRange.Formula = "Bottom1";

                myExcelRange = myExcelWorksheet.get_Range(myExcelWorksheet.Cells[BiasIndex + 7, 3], myExcelWorksheet.Cells[BiasIndex + 7, 3]);
                myExcelRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                myExcelRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                myExcelRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin, Excel.XlColorIndex.xlColorIndexAutomatic, misValue);
                myExcelRange.Formula = "Bottom2";

                i++;
            }

            //return;


            string[] Heads = DataHeader.Split(',');


            BaseIndex = 4;
            ContainCount = 1;

            ShiftIndex = 0;

            i = 0;
            while (i < 1)
            {
                //產生表頭欄位p
                BiasIndex = ContainCount * (i % PageCount) + BaseIndex;

                ((Excel.Range)myExcelWorksheet.Columns[BiasIndex, misValue]).ColumnWidth = 13;
                ((Excel.Range)myExcelWorksheet.Columns[BiasIndex + 1, misValue]).ColumnWidth = 13;

                myExcelRange = myExcelWorksheet.get_Range(myExcelWorksheet.Cells[2, BiasIndex], myExcelWorksheet.Cells[2, BiasIndex + ContainCount - 1]);
                myExcelRange.Merge(misValue);
                myExcelRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                myExcelRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                myExcelRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin, Excel.XlColorIndex.xlColorIndexAutomatic, misValue);

                //myExcelRange.Formula = (i + 1).ToString("0000");


                //產生表頭說明欄位
                myExcelRange = myExcelWorksheet.get_Range(myExcelWorksheet.Cells[3, BiasIndex], myExcelWorksheet.Cells[3, BiasIndex]);
                myExcelRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                myExcelRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                myExcelRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin, Excel.XlColorIndex.xlColorIndexAutomatic, misValue);

                //myExcelRange.Formula = Heads[i];
                //填入格式
                j = 0;
                foreach (KeycapDataClass kpdata in kbdata.KeycapDataList)
                {
                    int NextFieldCount = kpdata.DataContain;

                    k = 0;

                    while (k < kpdata.DataContain)
                    {
                        myExcelRange = myExcelWorksheet.get_Range(myExcelWorksheet.Cells[4 + k + j * NextFieldCount + ShiftIndex, BiasIndex], myExcelWorksheet.Cells[4 + k + j * NextFieldCount + ShiftIndex, BiasIndex]);
                        myExcelRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        myExcelRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        myExcelRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin, Excel.XlColorIndex.xlColorIndexAutomatic, misValue);
                        myExcelRange.NumberFormat = "0.000";

                        k++;
                    }

                    j++;
                }
                i++;
            }


            myExcelRange = myExcelWorksheet.get_Range(myExcelWorksheet.Columns[4, misValue], myExcelWorksheet.Columns[4, misValue]);

            i = 0;
            while (i < 10)
            {
                myExcelRange.Copy(myExcelWorksheet.Columns[5 + i, misValue]);
                i++;
            }

            myExcelRange = myExcelWorksheet.get_Range(myExcelWorksheet.Columns[4, misValue], myExcelWorksheet.Columns[13, misValue]);

            i = 0;
            while (i < 19)
            {
                myExcelRange.Copy(myExcelWorksheet.Columns[14 + i * 10, misValue]);
                i++;
            }


            //檢查是否需要分Sheet
            if (KeyboardDataList.Count <= PageCount)
            {
                ((Excel.Worksheet)ExcelWorkBook.Sheets[1]).Name = "1 ~ " + KeyboardDataList.Count.ToString();
            }
            else
            {
                ((Excel.Worksheet)ExcelWorkBook.Sheets[1]).Name = "1 ~ " + PageCount.ToString();

                i = 0;
                while (i < ((KeyboardDataList.Count - 1) / PageCount))
                {
                    myExcelWorksheet.Copy(misValue, ExcelWorkBook.Sheets[ExcelWorkBook.Sheets.Count]);
                    ((Excel.Worksheet)ExcelWorkBook.Sheets[ExcelWorkBook.Sheets.Count]).Name = (PageCount * (i + 1) + 1).ToString() + " ~ " + (KeyboardDataList.Count - PageCount * (i + 2) < 0 ? KeyboardDataList.Count.ToString() : (PageCount * (i + 2)).ToString());
                    i++;
                }
            }

            //return; 


            string[,] HeaderData = new string[2, PageCount];
            string[,] TailData = new string[1, PageCount];
            double[,] FillData = new double[kbdata.kpFillCount + 2, PageCount]; //Last + 1 is for Space Middle,Last +1 +1 is for Shift Middle

            //string[] MKBesideString = rptlist[rptlist.Count - 1].BesideString.Split('%');

            BaseIndex = 4;
            ContainCount = 1;

            i = 0;
            while (i < KeyboardDataList.Count)
            {
                KeyboardDataClass kbd = KeyboardDataList[i];

                HeaderData[0, (i % PageCount)] = "'" + (i + 1).ToString("0000");
                HeaderData[1, (i % PageCount)] = "'" + kbd.Name;

                //填入資料
                j = 0;
                double MaxResult = -100;
                double MinResult = 100;

                ShiftIndex = 0;

                foreach (KeycapDataClass kpd in kbd.KeycapDataList)
                {
                    k = 0;

                    while (k < kpd.DataContain)
                    {
                        FillData[j * kpd.DataContain + ShiftIndex + k, (i % PageCount)] = kpd.data[k];

                        if (MaxResult < kpd.data[k])
                            MaxResult = kpd.data[k];

                        if (MinResult > kpd.data[k])
                            MinResult = kpd.data[k];

                        k++;
                    }

                    j++;
                }

                //FillData[(j - 1) * 4 + 4 + ShiftIndex, (i % 100) * 2 + 1] = MaxResult;
                //FillData[(j - 1) * 4 + 4 + ShiftIndex, (i % 100) * 2] = MaxSingleResult;

                i++;
                
                if (i % PageCount == 0)
                {
                    myExcelRange = myExcelWorksheet.get_Range(myExcelWorksheet.Cells[2, 4], myExcelWorksheet.Cells[3, 4 + PageCount - 1]);
                    myExcelRange.Value2 = HeaderData;

                    myExcelRange = myExcelWorksheet.get_Range(myExcelWorksheet.Cells[4, 4], myExcelWorksheet.Cells[4 + kbd.kpFillCount - 1, 4 + PageCount - 1]);
                    myExcelRange.Value2 = FillData;

                    //myExcelRange = myExcelWorksheet.get_Range(myExcelWorksheet.Cells[4 + (4 * (rptlist.Count - 1)) + 3 + 1 + 1 + 1 + 1 + 1, 4], myExcelWorksheet.Cells[4 + (4 * (rptlist.Count - 1)) + 3 + 1 + 1 + 1 + 1 + 1, 4 + 100 * 2 - 1 + 1]);
                    //myExcelRange.Value2 = TailData;

                    if ((int)(i / PageCount + 1) <= ExcelWorkBook.Sheets.Count)
                        myExcelWorksheet = (Excel.Worksheet)ExcelWorkBook.Sheets[(int)(i / PageCount + 1)];

                    HeaderData = new string[2, PageCount];
                    TailData = new string[1, PageCount];
                    FillData = new double[kbdata.kpFillCount + 2, PageCount];
                }
            }

            if (i % PageCount != 0)
            {
                myExcelRange = myExcelWorksheet.get_Range(myExcelWorksheet.Cells[2, 4], myExcelWorksheet.Cells[3, 4 + PageCount - 1]);
                myExcelRange.Value2 = HeaderData;
                myExcelRange = myExcelWorksheet.get_Range(myExcelWorksheet.Cells[4, 4], myExcelWorksheet.Cells[4 + kbdata.kpFillCount - 1, 4 + PageCount - 1]);
                myExcelRange.Value2 = FillData;

                //myExcelRange = myExcelWorksheet.get_Range(myExcelWorksheet.Cells[4 + (4 * (rptlist.Count - 1)) + 3 + 1 + 1 + 1 + 1 + 1, 4], myExcelWorksheet.Cells[4 + (4 * (rptlist.Count - 1)) + 3 + 1 + 1 + 1 + 1 + 1, 4 + 100 * 2 - 1 + 1]);
                //myExcelRange.Value2 = TailData;
            }

            ExcelWorkBook.Save();

            ((Microsoft.Office.Interop.Excel._Worksheet)ExcelWorkBook.Sheets[1]).Activate();

        }
        
        */

        #endregion

    }
}