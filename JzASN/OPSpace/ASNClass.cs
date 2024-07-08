using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.IO;

using MoveGraphLibrary;
using WorldOfMoveableObjects;
using System.Drawing;
using JetEazy;
using JetEazy.BasicSpace;
using JzDisplay;
using JzASN.OPSpace.ASNSpace;

namespace JzASN.OPSpace
{
    public class ASNCollectionClass
    {
        VersionEnum VERSION;
        OptionEnum OPTION;

        const string ASNCollectionPath = @"D:\JETEAZY\ASN";
        string DATACNNSTRING = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + ASNCollectionPath + @"\DB\DATA.mdb;Jet OLEDB:Database Password=12892414;";
        protected OleDbConnection DATACONNECTION;
        protected OleDbCommand DATACOMMAND;
        protected OleDbCommandBuilder DATACMDBUILDER;
        protected OleDbDataAdapter DATAADAPTER;

        protected DataSet DATASET;

        DataTable myDataTable;
        public List<ASNClass> myDataList = new List<ASNClass>();

        int Index = 0;

        ASNClass DataNull = new ASNClass();
        public ASNClass DataLast
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

        public ASNClass GetASN(int no)
        {
            int index = FindIndex(no);
            return myDataList[index];
        }

        public ASNClass DataNow
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
        public ASNCollectionClass(VersionEnum version,OptionEnum option)
        {
            VERSION = version;
            OPTION = option;
        }
        public void Initial()
        {
            string SQLCMD = "";

            DATASET = new DataSet();

            DATACONNECTION = new OleDbConnection(DATACNNSTRING);
            DATACONNECTION.Open();

            SQLCMD = "select * from ASNDB order by ASNDB.no";
            DATAADAPTER = new OleDbDataAdapter();
            DATACMDBUILDER = new OleDbCommandBuilder(DATAADAPTER);
            DATACOMMAND = new OleDbCommand();
            DATACOMMAND.Connection = DATACONNECTION;

            DATACMDBUILDER.QuotePrefix = "[";
            DATACMDBUILDER.QuoteSuffix = "]";

            DATAADAPTER.SelectCommand = new OleDbCommand(SQLCMD, DATACONNECTION);
            DATAADAPTER.Fill(DATASET, "ASNDB");

            myDataTable = DATASET.Tables["ASNDB"];

            Load();
        }
        public void Load()
        {
            myDataList.Clear();

            foreach (DataRow datarow in myDataTable.Rows)
            {
                ASNClass data = new ASNClass(VERSION,OPTION);

                data.SetASNPath(ASNCollectionPath);

                Mapping(datarow, data);

                data.Load(ASNCollectionPath);

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
                    ASNClass data = new ASNClass(VERSION, OPTION);
                    Mapping(datarow, data);
                    data.Load(ASNCollectionPath);

                    int i = 0;

                    foreach (ASNClass asn in myDataList)
                    {
                        if (asn.No == no)
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

            foreach (ASNClass data in myDataList)
            {
                DataRow datarow = myDataTable.Rows[i];
                Mapping(data, datarow, -1);

                data.Save();

                i++;
            }

            DATAADAPTER.Update(DATASET, "OCRDB");

        }
        public void Save(int no)
        {
            int i = 0;

            foreach (ASNClass data in myDataList)
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
            DATAADAPTER.Update(DATASET, "ASNDB");

        }
        public string[] ToASNComboItem()
        {
            int i = 0;

            string[] strs = new string[myDataList.Count];

            foreach (ASNClass asn in myDataList)
            {
                strs[i] = asn.Name + "".PadLeft(100, ' ') + ":" + asn.No;
                i++;
            }
            return strs;
        }
        public void Mapping(ASNClass fromadata, DataRow todatarow, int asnno)
        {
            if (asnno != -1)
                todatarow["No"] = asnno;
            else
                todatarow["No"] = fromadata.No;

            todatarow["Name"] = fromadata.Name;
            todatarow["Remark"] = fromadata.Remark;

            todatarow["ASNItem"] = fromadata.FromASNItemList();

            todatarow["StartDatetime"] = fromadata.StartDatetime;
            todatarow["ModifyDatetime"] = fromadata.ModifyDatetime;

        }
        public void Mapping(DataRow fromdatarow, ASNClass todata)
        {
            todata.No = (int)fromdatarow["No"];

            todata.Name = (string)fromdatarow["Name"];
            todata.Remark = (string)fromdatarow["Remark"];

            string AsnItemString = fromdatarow["ASNItem"].ToString();

            todata.StartDatetime = (string)fromdatarow["StartDatetime"];
            todata.ModifyDatetime = (string)fromdatarow["ModifyDatetime"];

            todata.ToAsnItemList(AsnItemString);
        }
        public void Add(string ocrnostring, bool iscopy)
        {
            Add(FindIndex(DataNow.No), ocrnostring, iscopy);
        }
        public void Add(int index, string ocrnostring, bool iscopy)
        {
            int LastNo = DataLast.No + 1;
            DataRow newdatarow = myDataTable.NewRow();

            Mapping(myDataList[index], newdatarow, LastNo);

            newdatarow["Name"] = ((string)newdatarow["Name"])[0] + JzTimes.TimeSerialString;

            if (!iscopy)
            {
                newdatarow["AsnItem"] = "";
            }

            newdatarow["StartDatetime"] = JzTimes.DateTimeString;
            newdatarow["ModifyDatetime"] = JzTimes.DateTimeString;

            myDataTable.Rows.Add(newdatarow);

            ASNClass newasn = new ASNClass(VERSION, OPTION);
            Mapping(newdatarow, newasn);

            newasn.SetASNPath(ASNCollectionPath);

            //newocr.bmpLast.Dispose();
            //newocr.bmpLast = new Bitmap(myDataList[index].bmpLast);

            myDataList.Add(newasn);

            Index = myDataList.Count - 1;

            Copy(ASNCollectionPath + "\\PIC\\" + ocrnostring, ASNCollectionPath + "\\PIC\\" + newasn.ASNNoString);

            newasn.Load(ASNCollectionPath);
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

            foreach (ASNClass data in myDataList)
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
    public class ASNClass
    {
        VersionEnum VERSION;
        OptionEnum OPTION;

        public static string OrgASNNoString = "00000";

        public int No = 0;
        public string Name = "";
        public string Remark = "";
        public string StartDatetime = "";
        public string ModifyDatetime = "";

        public Bitmap bmpASN = new Bitmap(1, 1);
        
        public List<ASNItemClass> ASNItemList = new List<ASNItemClass>();
        
        ASNClass ASNBackup;

        string ASNPath = "";        //This SHould be Clone or Copy Or Add For Input

        string myAsnPath
        {
            get
            {
                return ASNPath + "\\PIC\\" + No.ToString(OrgASNNoString);
            }
        }

        public string ASNNoString
        {
            get
            {
                return No.ToString(OrgASNNoString);
            }
        }
        public ASNClass()
        {

        }
        public ASNClass(VersionEnum version,OptionEnum option)
        {
            VERSION = version;
            OPTION = option;
        }
        public void Backup()
        {
            if (ASNBackup == null)
                ASNBackup = new ASNClass();

            ASNBackup.Suicide();
            ASNBackup.Clone(this);
        }
        public void Restore()
        {
            this.Clone(ASNBackup);
            ASNBackup.Suicide();
        }
        void Clone(ASNClass asn)
        {
            VERSION = asn.VERSION;
            OPTION = asn.OPTION;
            
            No = asn.No;
            Name = asn.Name;
            Remark = asn.Remark;
            StartDatetime = asn.StartDatetime;
            ModifyDatetime = asn.ModifyDatetime;

            ASNPath = asn.ASNPath;

            bmpASN.Dispose();
            bmpASN = new Bitmap(asn.bmpASN);

            foreach (ASNItemClass asnitem in ASNItemList)
            {
                asnitem.Suicide();
            }
            ASNItemList.Clear();
            
            foreach (ASNItemClass asnitem in asn.ASNItemList)
            {
                ASNItemList.Add(asnitem.Clone());
            }
        }

        public void SetASNPath(string asnpath)
        {
            ASNPath = asnpath;
        }
        public void Load(string asncollectionpath)
        {
            //AsnPath = ocrcollectionpath + "\\PIC\\" + No.ToString(OrgAsnNoString);

            GetBMP(myAsnPath + "\\ASN.png", ref bmpASN);

            //foreach (AsnItemClass asnitem in AsnItemList)
            //{
            //    asnitem.Load(myAsnPath);
            //}
        }
        public void Save()
        {
            SaveBMP(myAsnPath + "\\ASN.png", ref bmpASN);
            
            //SaveAsnItem();
        }
        public void GetBMP(Bitmap bmp)
        {
            bmpASN.Dispose();
            bmpASN = new Bitmap(bmp);
        }

        public void ToAsnItemList(string str)
        {
            foreach (ASNItemClass asnitem in ASNItemList)
            {
                asnitem.Suicide();
            }

            ASNItemList.Clear();

            if (str.Trim() == "")
                return;

            string[] strs = str.Replace(Environment.NewLine, Universal.NewlineChar.ToString()).Split(Universal.NewlineChar);

            foreach (string strx in strs)
            {
                ASNItemClass newasnitem = new ASNItemClass(strx,VERSION,OPTION);
                //newocritem.Load(AsnPath);
                ASNItemList.Add(newasnitem);
            }
        }
        public string FromASNItemList()
        {
            string str = "";

            foreach (ASNItemClass asnitem in ASNItemList)
            {
                str += asnitem.ToString() + Environment.NewLine;
            }

            if (str.Length > 0)
                str = str.Remove(str.Length - 2, 2);

            return str;
        }
        public void Suicide()
        {
            bmpASN.Dispose();

            foreach (ASNItemClass ocritem in ASNItemList)
            {
                ocritem.Suicide();
            }

            ASNItemList.Clear();
        }
        public string ToModifyString()
        {
            return "Start Time: " + StartDatetime + Environment.NewLine + " Modify Time: " + ModifyDatetime;
        }

        public string ToCPDListString()
        {
            string str = "";

            str += No.ToString(OrgASNNoString) + ":" + Name;

            return str;
        }
        public string ToNoString()
        {
            string str = "";

            str += No.ToString(OrgASNNoString);

            return str;
        }
        public string ToASNString()
        {
            string str = "";

            str += Name + "(" + No.ToString() + ")";

            return str;
        }
        public void Reset()
        {


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

        //public void GetShowResultMover(Mover showmover, PointF biaslocation, SizeF sizeratio, int colorindex, Point offset,bool isonlyshowNG)
        //{
        //    Color showcolor = Color.Red;

        //    foreach (ASNItemClass asnitem in ASNItemList)
        //    {
        //        asnitem.GetShowResultMover(showmover, biaslocation, sizeratio, colorindex, offset,isonlyshowNG);
        //    }
        //}

        #endregion
    }

    public class ASNItemClass
    {
        /// <summary>
        /// 作為所有的對應位數值
        /// </summary>
        public static string ORGASNNOSTRING = "0000";

        VersionEnum VERSION;
        OptionEnum OPTION;

        public int ParentNo = 0;
        public int No = 0;
        public string AliasName = "";
        public Mover myMover = new Mover();

        public NORMALClass NORMALPara = new NORMALClass();
        public ASSEMBLEClass ASSEMBLE = new ASSEMBLEClass();

        public string RelateAnalyzeStr = "";
        public bool IsVeryGood = false;

        public bool IsSelected
        {
            get
            {
                bool isselected = false;
                int i = 0;

                GraphicalObject grobj;

                while (i < myMover.Count)
                {
                    grobj = myMover[i].Source;

                    if ((grobj as GeoFigure).IsSelected)
                    {
                        isselected = true;
                        break;
                    }
                    i++;
                }

                return isselected;
            }
        }
        public ASNItemClass()
        {
            //ASSEMBLE.ConstructProperty(VERSION, OPTION);

            //No = 1;
            //AliasName = ToAsnItemString();

            //JzRectEAG jzrect = new JzRectEAG(Color.FromArgb(0, Color.Red));
            //jzrect.RelateNo = No;
            //jzrect.RelatePosition = 0;
            //jzrect.RelateLevel = 2;

            //jzrect.Resizable = false;
            //jzrect.Rotatable = false;

            //myMover.Add(jzrect);
        }

        public ASNItemClass(VersionEnum version,OptionEnum option)
        {
            VERSION = version;
            OPTION = option;

            ASSEMBLE.ConstructProperty(VERSION, OPTION);

            No = 1;
            AliasName = ToAsnItemString();

            JzRectEAG jzrect = new JzRectEAG(Color.FromArgb(0, Color.Red));
            jzrect.RelateNo = No;
            jzrect.RelatePosition = 0;
            jzrect.RelateLevel = 2;

            myMover.Add(jzrect);

        }
        public ASNItemClass(string str,VersionEnum version,OptionEnum option)
        {
            VERSION = version;
            OPTION = option;

            FromString(str);
            ASSEMBLE.ConstructProperty(VERSION, OPTION);
        }

        public ASNItemClass Clone()
        {
            ASNItemClass asn = new ASNItemClass(this.ToString(), VERSION, OPTION);

            return asn;
        }
        public ASNItemClass Clone(Point offsetpoint)
        {
            ASNItemClass asn = new ASNItemClass(this.ToString(),VERSION,OPTION);

            asn.SetMoverOffset(offsetpoint);

            this.SetMoverSelected(false);

            return asn;
        }

        void SetMoverOffset(Point offsetpoint)
        {
            int i = 0;

            while (i < myMover.Count)
            {
                GraphicalObject grpobj = myMover[i].Source;

                (grpobj as GeoFigure).SetOffset(offsetpoint);

                i++;
            }
        }
        public void SetMoverSelected(bool isselect)
        {
            SetMoverSelected(false, isselect);
        }
        public void SetMoverSelected(bool isselectfirst, bool isselect)
        {
            int i = 0;

            GraphicalObject grobj;

            bool isfirstok = false;

            while (i < myMover.Count)
            {
                grobj = myMover[i].Source;

                if (!isfirstok && isselectfirst && isselect)
                {
                    (grobj as GeoFigure).IsFirstSelected = true;
                    isfirstok = true;
                }
                else
                    (grobj as GeoFigure).IsFirstSelected = false;

                (grobj as GeoFigure).IsSelected = isselect;

                i++;
            }
        }
        public void RelateMover(int relateno, int relatelevel)
        {
            int i = 0;

            while (i < myMover.Count)
            {
                GraphicalObject grpobj = myMover[i].Source;

                (grpobj as GeoFigure).RelateNo = relateno;
                (grpobj as GeoFigure).RelatePosition = i;
                (grpobj as GeoFigure).RelateLevel = relatelevel;

                i++;
            }
        }
        public void AddNewRowToDataTable(DataTable datatable)
        {
            DataRow newdatarow = datatable.NewRow();

            newdatarow["ParentNo"] = this.ParentNo;
            newdatarow["No"] = this.No;
            newdatarow["Name"] = this.ToAsnItemString();
            //newdatarow["LV"] = this.Level;

            //if(islearn)
            //    newdatarow["LN"] = this.LearnNo;
            //else
            //newdatarow["LN"] = this.LearnList.Count;

            datatable.Rows.Add(newdatarow);
        }

        public void InsertDeleteData(List<int> deletenolist, List<ASNItemClass> deleteasnlist)
        {
            if (deletenolist.IndexOf(this.No) < 0)
            {
                deletenolist.Add(this.No);
                deleteasnlist.Add(this);
            }
        }
        void FromString(string str)
        {
            char seperator = Universal.SeperateCharA;
            string[] strs = str.Split(seperator);

            FromNormalString(strs[0]);
        }
        public override string ToString()
        {
            char seperator = Universal.SeperateCharA;
            string str = "";

            str += ToNoramalString() + seperator;
            str += "";
            
            return str;
        }
        void FromNormalString(string str)
        {
            char seperator = Universal.SeperateCharB;
            string[] strs = str.Split(seperator);

            No = int.Parse(strs[0]);
            AliasName = strs[1];
            FromMoverString(strs[2]);
        }
        string ToNoramalString()
        {
            char seperator = Universal.SeperateCharB;
            string str = "";

            str += No.ToString() + seperator;                   //1
            str += AliasName + seperator;                       //2
            str += ToMoverString() + seperator;                 //3
            
            str += "";

            return str;
        }
        void FromMoverString(string fromstr)
        {
            int i = 0;
            char seperator = Universal.SeperateCharC;
            string[] strs = fromstr.Split(seperator);

            Color DefaultColor = Color.FromArgb(0, Color.Red);

            int relatelevel = 2;

            foreach (string str in strs)
            {
                if (str.IndexOf(Figure_EAG.Rectangle.ToString()) > -1)
                {
                    JzRectEAG jzrect = new JzRectEAG(str, DefaultColor);

                    jzrect.RelateNo = No;
                    jzrect.RelatePosition = i;
                    jzrect.RelateLevel = relatelevel;

                    myMover.Add(jzrect);
                }
                else if (str.IndexOf(Figure_EAG.Circle.ToString()) > -1)
                {
                    JzCircleEAG jzcircle = new JzCircleEAG(str, DefaultColor);

                    jzcircle.RelateNo = No;
                    jzcircle.RelatePosition = i;
                    jzcircle.RelateLevel = relatelevel;

                    myMover.Add(jzcircle);
                }
                else if (str.IndexOf(Figure_EAG.ChatoyantPolygon.ToString()) > -1)
                {
                    JzPolyEAG jzpoly = new JzPolyEAG(str, DefaultColor);

                    jzpoly.RelateNo = No;
                    jzpoly.RelatePosition = i;
                    jzpoly.RelateLevel = relatelevel;

                    myMover.Add(jzpoly);
                }
                else if (str.IndexOf(Figure_EAG.Ring.ToString()) > -1 || str.IndexOf(Figure_EAG.ORing.ToString()) > -1)
                {
                    JzRingEAG jzring = new JzRingEAG(str, DefaultColor);

                    jzring.RelateNo = No;
                    jzring.RelatePosition = i;
                    jzring.RelateLevel = relatelevel;

                    myMover.Add(jzring);
                }
                else if (str.IndexOf(Figure_EAG.Strip.ToString()) > -1)
                {
                    JzStripEAG jzstrip = new JzStripEAG(str, DefaultColor);

                    jzstrip.RelateNo = No;
                    jzstrip.RelatePosition = i;
                    jzstrip.RelateLevel = relatelevel;

                    myMover.Add(jzstrip);
                }
                else if (str.IndexOf(Figure_EAG.RectRect.ToString()) > -1 || str.IndexOf(Figure_EAG.HexHex.ToString()) > -1)
                {
                    JzIdentityHoleEAG jzidentityhole = new JzIdentityHoleEAG(str, DefaultColor);

                    jzidentityhole.RelateNo = No;
                    jzidentityhole.RelatePosition = i;
                    jzidentityhole.RelateLevel = relatelevel;

                    myMover.Add(jzidentityhole);
                }
                else if (str.IndexOf(Figure_EAG.RectO.ToString()) > -1 || str.IndexOf(Figure_EAG.HexO.ToString()) > -1)
                {
                    JzCircleHoleEAG jzcirclehole = new JzCircleHoleEAG(str, DefaultColor);

                    jzcirclehole.RelateNo = No;
                    jzcirclehole.RelatePosition = i;
                    jzcirclehole.RelateLevel = relatelevel;

                    myMover.Add(jzcirclehole);
                }

                i++;
            }
        }
        string ToMoverString()
        {
            string retstr = "";
            char seperator = Universal.SeperateCharC;

            GraphicalObject grobj;

            for (int i = 0; i < myMover.Count; i++)
            {
                grobj = myMover[i].Source;

                if (grobj is JzRectEAG)
                {
                    retstr += (grobj as JzRectEAG).ToString() + seperator;
                }
                else if (grobj is JzCircleEAG)
                {
                    retstr += (grobj as JzCircleEAG).ToString() + seperator;
                }
                else if (grobj is JzPolyEAG)
                {
                    retstr += (grobj as JzPolyEAG).ToString() + seperator;
                }
                else if (grobj is JzRingEAG)
                {
                    retstr += (grobj as JzRingEAG).ToString() + seperator;
                }
                else if (grobj is JzStripEAG)
                {
                    retstr += (grobj as JzStripEAG).ToString() + seperator;
                }
                else if (grobj is JzIdentityHoleEAG)
                {
                    retstr += (grobj as JzIdentityHoleEAG).ToString() + seperator;
                }
                else if (grobj is JzCircleHoleEAG)
                {
                    retstr += (grobj as JzCircleHoleEAG).ToString() + seperator;
                }
            }

            retstr = retstr.Substring(0, retstr.Length - 1);

            return retstr;
        }
        public void FromAssembleProperty()
        {
            ASSEMBLE.SetNormal(NORMALPara);

            AliasName = NORMALPara.AliasName;
        }
        public void FromAssembleProperty(string changeitemstring, string valuestring)
        {
            if (IsSelected)
            {
                if (changeitemstring == "RESET")    //RESET To Default Value
                {
                    NORMALPara.Reset();
                }
                else
                {
                    NORMALPara.FromPropertyChange(changeitemstring, valuestring);
                }

                AliasName = NORMALPara.AliasName;
                ReviseShape(NORMALPara.Shape);
            }
        }
        public void ToAssembleProperty()
        {
            NORMALPara.Name = ToAsnItemString();
            NORMALPara.AliasName = AliasName;

            GraphicalObject grobj = myMover[0].Source;

            NORMALPara.Shape = (grobj as GeoFigure).ToShapeEnum();

            ASSEMBLE.GetNormal(NORMALPara);
        }
        public void ReviseShape(ShapeEnum shape)
        {
            //int i = 0;
            //i = myMover.Count - 1;

            GraphicalObject grobj;

            //while (i > -1)
            //{
            //    grobj = movercollection[i].Source;

            //    if ((grobj as GeoFigure).RelateNo == this.No)
            //    {
            //        movercollection.RemoveAt(i);
            //    }
            //    i--;
            //}
            //再刪除原先在Mover裏的對應，再加入新的型態
            //i = myMover.Count - 1;
            //while (i > -1)
            //{

            grobj = myMover[0].Source;

            if ((grobj as GeoFigure).IsSelected)
            {
                RectangleF FromRectF = (grobj as GeoFigure).RealRectangleAround(0, 0);

                ReviseShape(shape, FromRectF, (grobj as GeoFigure).IsFirstSelected, (grobj as GeoFigure).IsSelected, myMover);
            }

            //i--;
            //}
            ////將myMover內重新對應後再加入MoverCollection
            //i = 0;
            //while (i < myMover.Count)
            //{
            //    grobj = myMover[i].Source;

            //    (grobj as GeoFigure).RelatePosition = i;
            //    movercollection.Add(grobj);

            //    i++;
            //}
        }
        void ReviseShape(ShapeEnum shape, RectangleF fromrectf, bool isfirstselect, bool isselect, Mover movercollection)
        {
            int insertindex = 0;
            int positionindex = 0;
            int Level = 2;

            Color DefaultColor = Color.FromArgb(0, Color.Red);

            switch (shape)
            {
                case ShapeEnum.RECT:
                    JzRectEAG jzrect = new JzRectEAG(DefaultColor, fromrectf);

                    jzrect.IsSelected = isselect;
                    jzrect.IsFirstSelected = isfirstselect;

                    jzrect.RelateNo = No;
                    jzrect.RelatePosition = positionindex;
                    jzrect.RelateLevel = Level;

                    movercollection.RemoveAt(insertindex);
                    movercollection.Insert(insertindex, jzrect);

                    break;
                case ShapeEnum.CIRCLE:
                    JzCircleEAG jzcircle = new JzCircleEAG(DefaultColor, fromrectf);

                    jzcircle.IsSelected = isselect;
                    jzcircle.IsFirstSelected = isfirstselect;

                    jzcircle.RelateNo = No;
                    jzcircle.RelatePosition = positionindex;
                    jzcircle.RelateLevel = Level;

                    movercollection.RemoveAt(insertindex);
                    movercollection.Insert(insertindex, jzcircle);

                    break;
                case ShapeEnum.POLY:
                    JzPolyEAG jzpoly = new JzPolyEAG(DefaultColor, fromrectf);

                    jzpoly.IsSelected = isselect;
                    jzpoly.IsFirstSelected = isfirstselect;

                    jzpoly.RelateNo = No;
                    jzpoly.RelatePosition = positionindex;
                    jzpoly.RelateLevel = Level;

                    movercollection.RemoveAt(insertindex);
                    movercollection.Insert(insertindex, jzpoly);
                    break;
                case ShapeEnum.CAPSULE:
                    JzStripEAG jzstrip = new JzStripEAG(DefaultColor, fromrectf);

                    jzstrip.IsSelected = isselect;
                    jzstrip.IsFirstSelected = isfirstselect;

                    jzstrip.RelateNo = No;
                    jzstrip.RelatePosition = positionindex;
                    jzstrip.RelateLevel = Level;

                    movercollection.RemoveAt(insertindex);
                    movercollection.Insert(insertindex, jzstrip);
                    break;
                case ShapeEnum.RING:
                case ShapeEnum.ORING:

                    JzRingEAG jzring;

                    if (shape == ShapeEnum.RING)
                        jzring = new JzRingEAG(DefaultColor, Figure_EAG.Ring, fromrectf);
                    else
                        jzring = new JzRingEAG(DefaultColor, Figure_EAG.ORing, fromrectf);

                    jzring.IsSelected = isselect;
                    jzring.IsFirstSelected = isfirstselect;

                    jzring.RelateNo = No;
                    jzring.RelatePosition = positionindex;
                    jzring.RelateLevel = Level;

                    movercollection.RemoveAt(insertindex);
                    movercollection.Insert(insertindex, jzring);
                    break;
                case ShapeEnum.RECTRECT:
                case ShapeEnum.HEXHEX:

                    JzIdentityHoleEAG jzidhole;

                    if (shape == ShapeEnum.RECTRECT)
                        jzidhole = new JzIdentityHoleEAG(DefaultColor, Figure_EAG.RectRect, fromrectf);
                    else
                        jzidhole = new JzIdentityHoleEAG(DefaultColor, Figure_EAG.HexHex, fromrectf);

                    jzidhole.IsSelected = isselect;
                    jzidhole.IsFirstSelected = isfirstselect;

                    jzidhole.RelateNo = No;
                    jzidhole.RelatePosition = positionindex;
                    jzidhole.RelateLevel = Level;

                    movercollection.RemoveAt(insertindex);
                    movercollection.Insert(insertindex, jzidhole);
                    break;
                case ShapeEnum.RECTO:
                case ShapeEnum.HEXO:

                    JzCircleHoleEAG jzcirclehole;

                    if (shape == ShapeEnum.RECTO)
                        jzcirclehole = new JzCircleHoleEAG(DefaultColor, Figure_EAG.RectO, fromrectf);
                    else
                        jzcirclehole = new JzCircleHoleEAG(DefaultColor, Figure_EAG.HexO, fromrectf);

                    jzcirclehole.IsSelected = isselect;
                    jzcirclehole.IsFirstSelected = isfirstselect;

                    jzcirclehole.RelateNo = No;
                    jzcirclehole.RelatePosition = positionindex;
                    jzcirclehole.RelateLevel = Level;

                    movercollection.RemoveAt(insertindex);
                    movercollection.Insert(insertindex, jzcirclehole);

                    break;
            }
        }
        public string ToAsnItemString()
        {
            string anz = "AIT" + "-" + No.ToString(ORGASNNOSTRING);

            return anz;
        }
        public string ToASNItemRelateString()
        {
            string anz = AliasName + "(" + No.ToString(ORGASNNOSTRING) + ")";

            return anz;

        }

        public void GetShowResultMover(Mover showmover, PointF biaslocation, SizeF sizeratio, int colorindex, Point offset,bool isonlyshowNG)
        {
            Color showcolor = Color.Red;

            if (IsVeryGood)
                showcolor = Color.Lime;
            else
                showcolor = Color.Red;

            if (!isonlyshowNG)
            {
                FromMoverString(showmover, ToMoverString(), showcolor, biaslocation, sizeratio, offset);
            }
            else
            {
                if (!IsVeryGood)
                    FromMoverString(showmover, ToMoverString(), showcolor, biaslocation, sizeratio, offset);
            }
        }

        void FromMoverString(Mover tomover, string fromstr, Color assigncolor, PointF biaslocation, SizeF sizeratio, Point offset)
        {
            int i = 0;
            char seperator = Universal.SeperateCharC;
            string[] strs = fromstr.Split(seperator);

            foreach (string str in strs)
            {
                if (str.IndexOf(Figure_EAG.Rectangle.ToString()) > -1)
                {
                    JzRectEAG jzrect = new JzRectEAG(str, Color.FromArgb(0, Color.White));

                    jzrect.TransparentForMover = true;
                    jzrect.ShowMode = ShowModeEnum.MAINSHOW;
                    jzrect.MainShowPen = new Pen(assigncolor, 1);
                    jzrect.OffsetPoint = offset;
                    jzrect.MappingToMovingObject(biaslocation, sizeratio);

                    tomover.Add(jzrect);
                }
                else if (str.IndexOf(Figure_EAG.Circle.ToString()) > -1)
                {
                    JzCircleEAG jzcircle = new JzCircleEAG(str, Color.FromArgb(0, Color.White));

                    jzcircle.TransparentForMover = true;
                    jzcircle.ShowMode = ShowModeEnum.MAINSHOW;
                    jzcircle.MainShowPen = new Pen(assigncolor, 1);
                    jzcircle.OffsetPoint = offset;
                    jzcircle.MappingToMovingObject(biaslocation, sizeratio);

                    tomover.Add(jzcircle);
                }
                else if (str.IndexOf(Figure_EAG.ChatoyantPolygon.ToString()) > -1)
                {
                    JzPolyEAG jzpoly = new JzPolyEAG(str, Color.FromArgb(0, Color.White));

                    jzpoly.TransparentForMover = true;
                    jzpoly.ShowMode = ShowModeEnum.MAINSHOW;
                    jzpoly.MainShowPen = new Pen(assigncolor, 1);
                    jzpoly.OffsetPoint = offset;
                    jzpoly.MappingToMovingObject(biaslocation, sizeratio);

                    tomover.Add(jzpoly);
                }
                else if (str.IndexOf(Figure_EAG.Ring.ToString()) > -1 || str.IndexOf(Figure_EAG.ORing.ToString()) > -1)
                {
                    JzRingEAG jzring = new JzRingEAG(str, Color.FromArgb(0, Color.White));

                    jzring.TransparentForMover = true;
                    jzring.ShowMode = ShowModeEnum.MAINSHOW;
                    jzring.MainShowPen = new Pen(assigncolor, 1);
                    jzring.OffsetPoint = offset;
                    jzring.MappingToMovingObject(biaslocation, sizeratio);

                    tomover.Add(jzring);
                }
                else if (str.IndexOf(Figure_EAG.Strip.ToString()) > -1)
                {
                    JzStripEAG jzstrip = new JzStripEAG(str, Color.FromArgb(0, Color.White));

                    jzstrip.TransparentForMover = true;
                    jzstrip.ShowMode = ShowModeEnum.MAINSHOW;
                    jzstrip.MainShowPen = new Pen(assigncolor, 1);
                    jzstrip.OffsetPoint = offset;
                    jzstrip.MappingToMovingObject(biaslocation, sizeratio);

                    tomover.Add(jzstrip);
                }
                else if (str.IndexOf(Figure_EAG.RectRect.ToString()) > -1 || str.IndexOf(Figure_EAG.HexHex.ToString()) > -1)
                {
                    JzIdentityHoleEAG jzidentityhole = new JzIdentityHoleEAG(str, Color.FromArgb(0, Color.White));

                    jzidentityhole.TransparentForMover = true;
                    jzidentityhole.ShowMode = ShowModeEnum.MAINSHOW;
                    jzidentityhole.MainShowPen = new Pen(assigncolor, 1);
                    jzidentityhole.OffsetPoint = offset;
                    jzidentityhole.MappingToMovingObject(biaslocation, sizeratio);

                    tomover.Add(jzidentityhole);
                }
                else if (str.IndexOf(Figure_EAG.RectO.ToString()) > -1 || str.IndexOf(Figure_EAG.HexO.ToString()) > -1)
                {
                    JzCircleHoleEAG jzcirclehole = new JzCircleHoleEAG(str, Color.FromArgb(0, Color.White));

                    jzcirclehole.TransparentForMover = true;
                    jzcirclehole.ShowMode = ShowModeEnum.MAINSHOW;
                    jzcirclehole.MainShowPen = new Pen(assigncolor, 1);
                    jzcirclehole.OffsetPoint = offset;
                    jzcirclehole.MappingToMovingObject(biaslocation, sizeratio);

                    tomover.Add(jzcirclehole);
                }

                i++;
            }
        }
        public void Suicide()
        {


        }

    }
}
