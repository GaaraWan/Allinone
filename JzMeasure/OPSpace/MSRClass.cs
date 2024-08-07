using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
using System.Drawing;
using System.IO;

using System.Drawing.Imaging;
using JetEazy;
using JetEazy.BasicSpace;
using WorldOfMoveableObjects;



namespace JzMSR.OPSpace
{
    public class MSRCollectionClass
    {
        public static string MSRCollectionPath = @"D:\JETEAZY\MSR";
        string DATACNNSTRING = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + MSRCollectionPath + @"\DB\DATA.mdb;Jet OLEDB:Database Password=12892414;";
        protected OleDbConnection DATACONNECTION;
        protected OleDbCommand DATACOMMAND;
        protected OleDbCommandBuilder DATACMDBUILDER;
        protected OleDbDataAdapter DATAADAPTER;

        protected DataSet DATASET;

        DataTable myDataTable;
        List<MSRClass> myDataList = new List<MSRClass>();

        int Index = 0;

        MSRClass DataNull = new MSRClass();
        public MSRClass DataLast
        {
            get
            {
                return myDataList[myDataList.Count - 1];
            }
        }
        public void GotoIndex(int index)
        {
            Index = index;
        }
        public int FindIndex(int no)
        {
            int ret = -1;
            int i = 0;

            while (i < myDataList.Count)
            {
                if (myDataList[i].No == no)
                {
                    ret = i;
                    break;
                }
                i++;
            }

            return ret;
        }
        public MSRClass DataNow
        {
            get
            {
                if (Index == -1)
                    return DataNull;
                else
                {
                    return myDataList[Index];
                }
            }
        }
        public MSRCollectionClass()
        {

        }
        public MSRCollectionClass(string diskPath)
        {
            MSRCollectionPath = $@"{diskPath}\JETEAZY\MSR";
            //DATACNNSTRING = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + $@"{diskPath}\JETEAZY\MSR" + @"\DB\DATA.mdb;Jet OLEDB:Database Password=12892414;";
        }
        public void Initial()
        {
            string SQLCMD = "";

            DATASET = new DataSet();

            DATACONNECTION = new OleDbConnection(DATACNNSTRING);
            DATACONNECTION.Open();

            SQLCMD = "select * from MSRDB order by MSRDB.no";
            DATAADAPTER = new OleDbDataAdapter();
            DATACMDBUILDER = new OleDbCommandBuilder(DATAADAPTER);
            DATACOMMAND = new OleDbCommand();
            DATACOMMAND.Connection = DATACONNECTION;

            DATACMDBUILDER.QuotePrefix = "[";
            DATACMDBUILDER.QuoteSuffix = "]";

            DATAADAPTER.SelectCommand = new OleDbCommand(SQLCMD, DATACONNECTION);
            DATAADAPTER.Fill(DATASET, "MSRDB");

            myDataTable = DATASET.Tables["MSRDB"];

            Load();
        }
        public void Load()
        {
            myDataList.Clear();

            foreach (DataRow datarow in myDataTable.Rows)
            {
                MSRClass data = new MSRClass();

                data.SetMSRPath(MSRCollectionPath);

                Mapping(datarow, data);

                data.Load(MSRCollectionPath);

                myDataList.Add(data);
            }

            Index = 0;
        }
        public void Load(int no)
        {
            foreach (DataRow datarow in myDataTable.Rows)
            {
                if ((int)datarow["No"] == no)
                {
                    MSRClass data = new MSRClass();
                    Mapping(datarow, data);
                    data.Load(MSRCollectionPath);

                    int i = 0;

                    foreach(MSRClass MSR in myDataList)
                    {
                        if (MSR.No == no)
                            break;

                        i++;
                    }

                    myDataList.RemoveAt(i);
                    myDataList.Insert(i, data);

                    break;
                }
            }

        }
        public void Save()
        {
            int i = 0;

            foreach (MSRClass data in myDataList)
            {
                DataRow datarow = myDataTable.Rows[i];
                Mapping(data, datarow, -1);

                data.Save();

                i++;
            }
            
            DATAADAPTER.Update(DATASET, "MSRDB");

        }
        public void Save(int no)
        {
            int i = 0;

            foreach (MSRClass data in myDataList)
            {
                DataRow datarow = myDataTable.Rows[i];

                if ((int)datarow["No"] == no)
                {
                    Mapping(data, datarow, -1);
                    data.Save();
                    break;
                }
                i++;
            }
            DATAADAPTER.Update(DATASET, "MSRDB");

        }
        public string[] ToMSRComboItem()
        {
            int i = 0;

            string[] strs = new string[myDataList.Count];
            
            foreach (MSRClass MSR in myDataList)
            {
                strs[i] = MSR.Name + "".PadLeft(100,' ') + ":" + MSR.No;
                i++;
            }
            return strs;
        }
        public void Mapping(MSRClass fromadata, DataRow todatarow, int assignno)
        {
            if (assignno != -1)
                todatarow["No"] = assignno;
            else
                todatarow["No"] = fromadata.No;

            todatarow["Name"] = fromadata.Name;

            todatarow["MSRItem"] = fromadata.FromMSRItemList();
            todatarow["CamIndex"] = fromadata.CamIndex;
            todatarow["Exposure"] = fromadata.Exposure;

            todatarow["Remark"] = fromadata.Remark;
            todatarow["StartDatetime"] = fromadata.StartDatetime;
            todatarow["ModifyDatetime"] = fromadata.ModifyDatetime;

            todatarow["MSROther"] = fromadata.MSROtherList();

        }
        public void Mapping(DataRow fromdatarow, MSRClass todata)
        {
            todata.No = (int)fromdatarow["No"];
            
            string MSRItemString = fromdatarow["MSRItem"].ToString();
            string MSROther= fromdatarow["MSROther"].ToString();

            todata.Name = (string)fromdatarow["Name"];
            todata.Remark = (string)fromdatarow["Remark"];

            todata.CamIndex = (int)fromdatarow["CamIndex"];
            todata.Exposure = (int)fromdatarow["Exposure"];

            todata.StartDatetime = (string)fromdatarow["StartDatetime"];
            todata.ModifyDatetime = (string)fromdatarow["ModifyDatetime"];

            todata.ToMSRItemList(MSRItemString);
            todata.FromString(MSROther);
        }
        public void Add(string MSRnostring, bool iscopy)
        {
            Add(FindIndex(DataNow.No), MSRnostring, iscopy);
        }
        public void Add(int index, string MSRnostring,bool iscopy)
        {
            int LastNo = DataLast.No + 1;
            DataRow newdatarow = myDataTable.NewRow();

            Mapping(myDataList[index], newdatarow, LastNo);

            newdatarow["Name"] = ((string)newdatarow["Name"])[0] + JzTimes.TimeSerialString;

            if (!iscopy)
            {
                newdatarow["MSRItem"] = "";
            }

            newdatarow["StartDatetime"] = JzTimes.DateTimeString;
            newdatarow["ModifyDatetime"] = JzTimes.DateTimeString;

            myDataTable.Rows.Add(newdatarow);

            MSRClass newMSR = new MSRClass();
            Mapping(newdatarow, newMSR);
            
            newMSR.SetMSRPath(MSRCollectionPath);
            
            myDataList.Add(newMSR);

            Index = myDataList.Count - 1;

            Copy(MSRCollectionPath + "\\PIC\\" + MSRnostring, MSRCollectionPath + "\\PIC\\" + newMSR.MSRNoString);

            newMSR.Load(MSRCollectionPath);
        }
        public void Delete(int index)
        {
            string deletestring = "DELETE FROM " + myDataTable.TableName + " WHERE " + myDataTable.TableName + ".No = " + myDataList[index].No;

            myDataList.RemoveAt(index);
            myDataTable.Rows.RemoveAt(index);
            
            DATACOMMAND.CommandText = deletestring;
            DATACOMMAND.ExecuteNonQuery();
        }
        public void DeleteLast(int originno)
        {
            Delete(myDataList.Count - 1);
            GotoIndex(FindIndex(originno));
        }
        public bool CheckIsDuplicate(string namestr, int checkno)
        {
            bool ret = false;

            foreach (MSRClass data in myDataList)
            {
                if (data.No == checkno)
                    continue;

                if (data.Name.Trim().ToUpper() == namestr.Trim().ToUpper())
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        /// <summary>
        /// Copy Tree 的功能
        /// </summary>
        /// <param name="fromdir"></param>
        /// <param name="todir"></param>
        void Copy(string fromdir, string todir)
        {
            DirectoryInfo difrom = new DirectoryInfo(fromdir);
            DirectoryInfo dito = new DirectoryInfo(todir);

            CopyAll(difrom, dito);
        }
        void CopyAll(DirectoryInfo difrom, DirectoryInfo dito)
        {
            if (Directory.Exists(dito.FullName))
                Directory.Delete(dito.FullName, true);

            Directory.CreateDirectory(dito.FullName);

            foreach (FileInfo fi in difrom.GetFiles())
            {
                fi.CopyTo(Path.Combine(dito.FullName, fi.Name), true);
            }

            foreach (DirectoryInfo disourcesubdir in difrom.GetDirectories())
            {
                DirectoryInfo nextsubdir = dito.CreateSubdirectory(disourcesubdir.Name);
                CopyAll(disourcesubdir, nextsubdir);
            }
        }
    }

    public class MSRClass
    {
        public static string OrgMSRNoString = "00000";

        public int No = -1;

        public string Name = "";
        public string Remark = "";
        public string StartDatetime = "";
        public string ModifyDatetime = "";

        public Bitmap bmpCalibrate = new Bitmap(1, 1);

        public int CamIndex = 0;
        public int Exposure = 0;

        public int XDirCount = 10;
        public int YDirCount = 10;
        public float LTX = 100f;
        public float LTY = 100f;
        public float XGap = 10f;
        public float YGap = 10f;

        public List<MSRItemClass> MSRItemList = new List<MSRItemClass>();

        public CAoiCalibration MSRCalibration = new CAoiCalibration();

        MSRClass MSRBackup;

        string MSRPath = "";        //This SHould be Clone or Copy Or Add For Input

        string myMSRPath
        {
            get
            {
                return MSRPath + "\\PIC\\" + No.ToString(OrgMSRNoString);
            }
        }
        
        public string MSRNoString
        {
            get
            {
                return No.ToString(OrgMSRNoString);
            }
        }

        void LoadMatrix(string bmppath)
        {
            //string MatrixFile = bmppath + "\\" + IndexNoStr + ".jdb";
            string MatrixFile = System.IO.Path.Combine(bmppath, "#" + CamIndex.ToString() + ".bin");

            if (!File.Exists(MatrixFile))
                SaveMatrix(bmppath);

            MSRCalibration.Load(MatrixFile);
        }
        void SaveMatrix(string bmppath)
        {
            //string MatrixFile = bmppath + "\\" + IndexNoStr + ".jdb";
            string MatrixFile = System.IO.Path.Combine(bmppath, "#" + CamIndex.ToString() + ".bin");

            MSRCalibration.Save(MatrixFile);
        }

        public PointF ViewToWorld(PointF ptfview)
        {
            PointF PtFWorld; // = new PointF();

            MSRCalibration.TransformViewToWorld(ptfview, out PtFWorld);

            return PtFWorld;
        }
        public PointF WorldToView(PointF ptfWorld)
        {
            PointF PtView; // = new PointF();

            MSRCalibration.TransformWorldToView(ptfWorld, out PtView);

            return PtView;
        }

        public MSRClass()
        {


        }
        public void Backup()
        {
            if (MSRBackup == null)
                MSRBackup = new MSRClass();

            MSRBackup.Suicide();
            MSRBackup.Clone(this);
        }
        public void Restore()
        {
            this.Clone(MSRBackup);
            MSRBackup.Suicide();
        }
        void Clone(MSRClass MSR)
        {
            No = MSR.No;
            Name = MSR.Name;
            Remark = MSR.Remark;

            CamIndex = MSR.CamIndex;
            Exposure = MSR.Exposure;

            StartDatetime = MSR.StartDatetime;
            ModifyDatetime = MSR.ModifyDatetime;

            MSRPath = MSR.MSRPath;

            bmpCalibrate.Dispose();
            bmpCalibrate = new Bitmap(MSR.bmpCalibrate);

            foreach(MSRItemClass msritem in MSRItemList)
            {
                msritem.Suicide();
            }
            MSRItemList.Clear();

            foreach (MSRItemClass msritem in MSR.MSRItemList)
            {
                MSRItemList.Add(msritem.Clone());
            }
        }

        public void SetMSRPath(string MSRpath)
        {
            MSRPath = MSRpath;
        }
        public void Load(string MSRcollectionpath)
        {
            GetBMP(myMSRPath + "\\CALIBRATE.png", ref bmpCalibrate);
            LoadMatrix(myMSRPath);
        }
        public void Save()
        {
            SaveBMP(myMSRPath + "\\CALIBRATE.png", ref bmpCalibrate);
            SaveMatrix(myMSRPath);
        }
       
        public void ToMSRItemList(string str)
        {
            foreach(MSRItemClass msritem in MSRItemList)
            {
                msritem.Suicide();
            }

            MSRItemList.Clear();

            if (str.Trim() == "")
                return;

            string[] strs = str.Replace(Environment.NewLine, Universal.SeperateCharB.ToString()).Split(Universal.SeperateCharB);

            foreach(string strx in strs)
            {
                MSRItemClass msritem = new MSRItemClass(strx);
                MSRItemList.Add(msritem);
            }
            
        }
        public string FromMSRItemList()
        {
            string str = "";

            foreach(MSRItemClass msritem in MSRItemList)
            {
                str += msritem.ToString() + Environment.NewLine;
            }

            if (str.Length > 0)
                str = str.Remove(str.Length - 2, 2);

            return str;
        }
        public string MSROtherList()
        {
            char seperator = Universal.SeperateCharA;

            string str = "";

            str += XDirCount.ToString() + seperator;
            str += YDirCount.ToString() + seperator;
            str += LTX.ToString() + seperator;
            str += LTY.ToString() + seperator;
            str += XGap.ToString() + seperator;
            str += YGap.ToString();

            return str;
        }
        public void FromString(string str)
        {
            char seperator = Universal.SeperateCharA;

            string[] strs = str.Split(seperator);

            if (strs.Length > 5)
            {
                XDirCount = int.Parse(strs[0]);
                YDirCount = int.Parse(strs[1]);
                LTX = float.Parse(strs[2]);
                LTY = float.Parse(strs[3]);
                XGap = float.Parse(strs[4]);
                YGap = float.Parse(strs[5]);
            }
        }
        public void Suicide()
        {
            bmpCalibrate.Dispose();

            foreach(MSRItemClass msritem in MSRItemList)
            {
                msritem.Suicide();
            }
            MSRItemList.Clear();
        }
        public string ToModifyString()
        {
            return "Start Time: " + StartDatetime + Environment.NewLine + " Modify Time: " + ModifyDatetime;
        }

        public void Reset()
        {
            foreach(MSRItemClass msritem in MSRItemList)
            {
                msritem.RectEAG.IsSelected = false;
                msritem.RectEAG.IsFirstSelected = false;
            }
        }

        /// <summary>
        /// 自動尋找白白或黑黑的定位點
        /// </summary>
        /// <param name="ispointblack"></param>
        /// <param name="thresholdratio"></param>
        public void AutoFind(bool ispointblack, int thresholdratio, int extend)
        {
            foreach(MSRItemClass msritem in MSRItemList)
            {
                msritem.Suicide();
            }
            MSRItemList.Clear();

            int i = 0;
            Bitmap bmp = new Bitmap(bmpCalibrate);

            JzFindObjectClass JzFind = new JzFindObjectClass();
            HistogramClass historgram = new HistogramClass(2);
            
            historgram.GetHistogram(bmpCalibrate);

            int max = historgram.GetMaxRatioAVG(10);
            int min = historgram.GetMinRatioAVG(10);

            max = historgram.MaxGrade;
            min = historgram.MinGrade;

            int threshvalue = min + (int)((float)(max - min) * (float)thresholdratio / 100f);

            if (ispointblack)
                JzFind.SetThreshold(bmp, SimpleRect(bmp.Size), threshvalue, 0, 255, true);
            else
            {
                threshvalue = max - (int)((float)(max - min) * (float)thresholdratio / 100f);
                JzFind.SetThreshold(bmp, SimpleRect(bmp.Size), threshvalue, 255, 0, true);
            }

            bmp.Save("D:\\LOA\\MSR_Threshold-1" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            JzFind.Find(bmp, Color.Red);
            bmp.Save("D:\\LOA\\MSR_Threshold-2" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

            i = 0;

            foreach(FoundClass found in JzFind.FoundList)
            {
                Rectangle rect = found.rect;

                rect.Inflate(extend, extend);
                rect.Intersect(SimpleRect(bmp.Size));

                Bitmap bmprect = (Bitmap)bmpCalibrate.Clone(rect, PixelFormat.Format32bppArgb);

                MSRItemClass msritem = new MSRItemClass(bmprect, rect, i);
                msritem.IsBlack = ispointblack;
                msritem.Threshold = thresholdratio;

                MSRItemList.Add(msritem);

                i++;
            }

            bmp.Dispose();
        }

        public void AssignPosition(string operationstr)
        {
            if (operationstr == "")
                return;

            string[] strs = operationstr.Split(',');
            XDirCount = int.Parse(strs[0]);
            YDirCount = int.Parse(strs[1]);
            LTX = float.Parse(strs[2]);
            LTY = float.Parse(strs[3]);
            XGap = float.Parse(strs[4]);
            YGap = float.Parse(strs[5]);

            if (!_assginposition())
                System.Windows.Forms.MessageBox.Show("請確認相關資料是否正確。");
        }

        private bool _assginposition()
        {
            int i = 0;
            int j = 0;
            bool ret = true;
            float updownoffset = 80f;//方框上下波動範圍

            int RowIndex = 0;
            int ColumnIndex = 0;

            List<string> RegionCheckList = new List<string>();
            List<string> RegionArrayList = new List<string>();

            int Highest = 1000000;

            foreach (MSRItemClass msritem in MSRItemList)
            {
                //if (region.Cell.CellProperty == CellPropertyEnum.STATIC)
                //{
                msritem.RowTag = 0;
                //}
            }

            RowIndex = 0;

            while (true)
            {
                Highest = 1000000;
                RegionCheckList.Clear();

                foreach (MSRItemClass msritem in MSRItemList)
                {
                    //if (region.Cell.CellProperty == CellPropertyEnum.STATIC)
                    //{
                        if (msritem.CenterPointF.Y < Highest && msritem.RowTag == 0)
                        {
                            Highest = (int)msritem.CenterPointF.Y;
                        }
                    //}
                }

                if (Highest == 1000000)
                {
                    if (RowIndex != YDirCount)
                    {
                        //MessageBox.Show("請確認相關資料是否正確。");
                        ret = false;
                    }
                    break;
                }

                i = 0;
                foreach (MSRItemClass msritem in MSRItemList)
                {
                    //if (region.Cell.CellProperty == CellPropertyEnum.STATIC)
                    //{
                        if (IsInRange(Highest, msritem.CenterPointF.Y, updownoffset) && msritem.RowTag == 0)
                        {
                            RegionCheckList.Add((((int)msritem.CenterPointF.X).ToString("000000")) + "#" + i.ToString());
                        }
                    //}
                    i++;
                }

                RegionCheckList.Sort();

                if (RegionCheckList.Count != XDirCount)
                {
                    //MessageBox.Show("請確認相關資料是否正確。");

                    ret = false;
                    break;
                }

                ColumnIndex = 0;

                string RegionArrayString = "";

                foreach (string str in RegionCheckList)
                {
                    string[] strs = str.Split('#');

                    int regionindex = int.Parse(strs[1]);

                    MSRItemList[regionindex].RelatePointF = new PointF(LTX + XGap * (float)ColumnIndex, LTY + YGap * (float)RowIndex);
                    MSRItemList[regionindex].RowTag = 10;

                    RegionArrayString += regionindex.ToString() + ",";

                    ColumnIndex++;
                }

                RegionArrayString = RemoveLastChar(RegionArrayString, 1);

                RegionArrayList.Add(RegionArrayString);

                RowIndex++;
            }

            if (ret)
            {
                PointF[,] ScreenArray = new PointF[YDirCount, XDirCount];
                PointF[,] RealArray = new PointF[YDirCount, XDirCount];

                i = 0;

                foreach (string Str in RegionArrayList)
                {
                    string[] strs = Str.Split(',');

                    j = 0;
                    foreach (string str in strs)
                    {
                        MSRItemClass msritem = MSRItemList[int.Parse(str)];

                        ScreenArray[i, j] = msritem.CenterPointF;
                        RealArray[i, j] = msritem.RelatePointF;

                        j++;
                    }
                    i++;
                }

                CAoiCalibration LtCalibration = MSRCalibration;

                LtCalibration.Dispose();
                LtCalibration.SetCalibrationPoints(ScreenArray, RealArray);
                LtCalibration.CalculateTransformMatrix();
            }

            return ret;
        }

        #region Tools Operation
        void GetBMP(string bmpfilestr, ref Bitmap bmp)
        {
            Bitmap bmptmp = new Bitmap(bmpfilestr);

            bmp.Dispose();
            bmp = new Bitmap(bmptmp);

            bmptmp.Dispose();
        }
        void SaveBMP(string bmpfilestr, ref Bitmap bmp)
        {
            Bitmap bmptmp = new Bitmap(bmp);

            bmptmp.Save(bmpfilestr, Universal.GlobalImageFormat);

            bmptmp.Dispose();
        }

        Rectangle SimpleRect(Size Sz)
        {
            return new Rectangle(0, 0, Sz.Width, Sz.Height);
        }
        bool IsInRange(float FromValue, float CompValue, float DiffValue)
        {
            return Math.Abs(FromValue - CompValue) < DiffValue;
        }
        string RemoveLastChar(string Str, int Count)
        {
            if (Str.Length < Count)
                return "";

            return Str.Remove(Str.Length - Count, Count);
        }
        #endregion

    }
}
