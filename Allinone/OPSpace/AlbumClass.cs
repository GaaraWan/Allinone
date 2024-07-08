using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using JetEazy.FormSpace;
using MoveGraphLibrary;
using WorldOfMoveableObjects;
using JetEazy.ControlSpace;

using JetEazy;
using JetEazy.BasicSpace;
using JetEazy.DBSpace;
using JzASN.OPSpace;
using Allinone.OPSpace.CPDSpace;
using System.Runtime.InteropServices;

namespace Allinone.OPSpace
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public class AlbumClass
    {
        ASNCollectionClass ASNCollection
        {
            get
            {
                return Universal.ASNCollection;
            }
        }

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
        AccDBClass ACCDB
        {
            get
            {
                return Universal.ACCDB;
            }
        }
        public int UseCount = 0;
        public RcpClass RelateRCP = new RcpClass();
        
        //string RCPPATH = "";
        public CPDClass CPD;

        const string DATACNNSTRING = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=%\\Album.mdb;Jet OLEDB:Database Password=12892414;";

        OleDbConnection DATACONNECTION;
        OleDbCommand[] DATACOMMAND;
        OleDbCommandBuilder[] DATACMDBUILDER;
        OleDbDataAdapter[] DATAADAPTER;
        DataSet DATASET;

        int EnvRunIndex = 0;

        public PassInfoClass PassInfo = new PassInfoClass();

        #region AlbumData

        Rectangle rectDefine = new Rectangle();

        public bool IsNeedToReTrain = false;

        #endregion
        public int ENVCount
        {
            get
            {
                return ENVList.Count;
            }
        }
        public List<EnvClass> ENVList = new List<EnvClass>();
        //public List<AssignClass> AssignList = new List<AssignClass>();

        public EnvClass m_EnvNow = null;

        public EnvClass LastENV
        {
            get
            {
                return ENVList[ENVList.Count - 1];
            }
        }
        public AlbumClass()
        {


        }
        public AlbumClass(RcpClass relatercp)
        {
            RelateRCP = new RcpClass(relatercp);
        }
        /// <summary>
        /// 新增參數或複制參數時所使用的建構子
        /// </summary>
        /// <param name="relatercp"></param>
        /// <param name="rcppath"></param>
        /// <param name="RcpNoString"></param>
        public AlbumClass(RcpClass relatercp, string rcppath,string fromrcpnostr,bool isneedcopy)
        {
            RelateRCP = new RcpClass(relatercp);
            
            PassInfo = new PassInfoClass();
            PassInfo.RcpNo = RelateRCP.No;
            PassInfo.OperatePath = rcppath + "\\" + RelateRCP.RcpNoString;

            if(isneedcopy)
                Copy(rcppath + "\\" + fromrcpnostr, PassInfo.OperatePath);
        }
        /// <summary>
        /// 使用 AlbumWork 時採取的對應方式，需注意會影響的地方
        /// </summary>
        /// <param name="album"></param>
        public AlbumClass(AlbumClass album,List<AlbumClass> staticealbumlist,string opstring,TestMethodEnum testmethod)
        {
            string version = "";
            string artwork = "";
            string relatecolor = "";

            string Vendor="";
            string Colour="";

            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch (OPTION)
                    {
                        case OptionEnum.R32:
                        case OptionEnum.R26:
                        case OptionEnum.R15:
                        case OptionEnum.R9:
                        case OptionEnum.R5:
                        case OptionEnum.R1:
                            if (testmethod == TestMethodEnum.QSMCSF)
                            {
                                string[] strbyte = opstring.Split('$');
                                version = strbyte[0];

                                string[] strbyteTemp = strbyte[1].Split('-');

                                relatecolor = strbyteTemp[strbyteTemp.Length - 1];
                                int index = strbyte[1].IndexOf(relatecolor);

                                artwork = strbyte[1].Substring(0, index - 1);
                            }
                            break;
                        case OptionEnum.R3:
                            if (testmethod == TestMethodEnum.QSMCSF)
                            {
                                string[] strbyte = opstring.Split('$');
                                version = strbyte[0];

                                string[] strbyteTemp = strbyte[1].Split('-');
                                string strtemp = strbyteTemp[strbyteTemp.Length - 1];
                                string[] strtemps = strtemp.Split('_') ;
                                if (strtemps.Length > 0)
                                {
                                    relatecolor = strtemps[0];
                                    if (strtemps.Length > 1)
                                        Vendor = strtemps[1];
                                    if (strtemps.Length > 2)
                                        Colour = strtemps[2];
                                }
                                else
                                    relatecolor = strbyteTemp[strbyteTemp.Length - 1];

                                if(Vendor=="" && INI.R3VENDOR !="")
                                    Vendor = INI.R3VENDOR;
                                

                                int index = strbyte[1].IndexOf(relatecolor);

                                artwork = strbyte[1].Substring(0, index - 1);
                            }
                            break;
                        case OptionEnum.C3:
                            if (testmethod == TestMethodEnum.QSMCSF)
                            {
                                string[] strbyte = opstring.Split('$');
                                version = strbyte[0];

                                string[] strbyteTemp = strbyte[1].Split('-');
                                string strtemp = strbyteTemp[strbyteTemp.Length - 1];
                                string[] strtemps = strtemp.Split('_');
                                if (strtemps.Length > 0)
                                {
                                    relatecolor = strtemps[0];
                                    if (strtemps.Length > 1)
                                        Vendor = strtemps[1];
                                    if (strtemps.Length > 2)
                                        Colour = strtemps[2];
                                }
                                else
                                    relatecolor = strbyteTemp[strbyteTemp.Length - 1];

                                if (Vendor == "" && INI.R3VENDOR != "")
                                    Vendor = INI.R3VENDOR;


                                int index = strbyte[1].IndexOf(relatecolor);

                                artwork = strbyte[1].Substring(0, index - 1);
                            }
                            break;
                    }
                    break;
            }

            album.UseCount++;

            RelateRCP = new RcpClass(album.RelateRCP);
            PassInfo = new PassInfoClass(album.PassInfo, OPLevelEnum.COPY);

            rectDefine = album.rectDefine;

            foreach(EnvClass env in album.ENVList)
            {
                EnvClass envwork = new EnvClass(env);

                foreach(PageClass page in env.PageList)
                {
                    if(page.RelateToRcpNo == -1 || testmethod != TestMethodEnum.QSMCSF)
                    {
                        envwork.PageList.Add(page);
                    }
                    else
                    {
                        foreach(AlbumClass staticalbum in staticealbumlist)
                        {
                            bool ismatchedpage = false;

                            if (staticalbum.RelateRCP.No == page.RelateToRcpNo)
                            {
                                foreach (EnvClass staticenv in staticalbum.ENVList)
                                {
                                    bool isbreak = false;
                                    foreach (PageClass staticpage in staticenv.PageList)
                                    {
                                        isbreak = false;
                                        //如果有對應的Page，則放入ENV，對應的格式為 ARTWORK_RELATECOLOR
                                        //if (("," + staticpage.RelateToVersionString.ToUpper() + ",").IndexOf("," + artwork.ToUpper() + "_*" + ",") > -1 ||
                                        //    ("," + staticpage.RelateToVersionString.ToUpper() + ",").IndexOf("," + "*_" + relatecolor.ToUpper() + ",") > -1 ||
                                        //    ("," + staticpage.RelateToVersionString.ToUpper() + ",").IndexOf("," + artwork.ToUpper() + "-" + relatecolor.ToUpper() + ",") > -1 ||
                                        //    ("," + staticpage.RelateToVersionString.ToUpper() + ",").IndexOf("," + "*_*,") > -1 ||
                                        //     staticpage.RelateToVersionString.ToUpper() .IndexOf(artwork.ToUpper()) > -1)
                                        //{
                                        //    envwork.PageList.Add(staticpage);
                                        //    ismatchedpage = true;
                                        //    break;
                                        //}
                                        switch (Universal.OPTION)
                                        {
                                            case OptionEnum.R15:
                                            case OptionEnum.R26:
                                            case OptionEnum.R32:
                                            case OptionEnum.R9:
                                            case OptionEnum.R5:
                                            case OptionEnum.R1:
                                                if ((staticalbum.RelateRCP.No == 80001 && ("," + staticpage.RelateToVersionString.ToUpper() + ",").IndexOf("," + artwork.ToUpper() + "-" + relatecolor.ToUpper() + ",") > -1) ||
                                                   (staticalbum.RelateRCP.No == 80000 && ("," + staticpage.RelateToVersionString.ToUpper() + ",").IndexOf("," + "*_" + relatecolor.ToUpper() + ",") > -1) ||
                                                   (staticalbum.RelateRCP.No == 80002 && ("," + staticpage.RelateToVersionString.ToUpper() + ",").IndexOf("," + "*=" + relatecolor.ToUpper() + ",") > -1) ||
                                                    (staticalbum.RelateRCP.No == 80003 && ("," + staticpage.RelateToVersionString.ToUpper() + ",").IndexOf("," + "*=" + relatecolor.ToUpper() + ",") > -1) ||
                                                    (staticalbum.RelateRCP.No == 80004 && ("," + staticpage.RelateToVersionString.ToUpper() + ",").IndexOf("," + "*=" + relatecolor.ToUpper() + ",") > -1)||
                                                    (staticalbum.RelateRCP.No == 80005 && ("," + staticpage.RelateToVersionString.ToUpper() + ",").IndexOf("," + "*=" + relatecolor.ToUpper() + ",") > -1) ||
                                                    (staticalbum.RelateRCP.No == 80006 && ("," + staticpage.RelateToVersionString.ToUpper() + ",").IndexOf("," + "*=" + relatecolor.ToUpper() + ",") > -1) ||
                                                    (staticalbum.RelateRCP.No == 80007 && ("," + staticpage.RelateToVersionString.ToUpper() + ",").IndexOf("," + "*=" + relatecolor.ToUpper() + ",") > -1) ||
                                                    (staticalbum.RelateRCP.No == 80008 && ("," + staticpage.RelateToVersionString.ToUpper() + ",").IndexOf("," + "*=" + relatecolor.ToUpper() + ",") > -1))
                                                {
                                                    envwork.PageList.Add(staticpage);
                                                    ismatchedpage = true;
                                                    isbreak = true;
                                                    break;
                                                }
                                                break;
                                            case OptionEnum.R3:
                                                if (Vendor != "")
                                                {
                                                    if ((staticalbum.RelateRCP.No == 80001 && ("," + staticpage.RelateToVersionString.ToUpper() + ",").IndexOf("," + artwork.ToUpper() + ",") > -1) && (("," + staticpage.AliasName.ToUpper() + ",").IndexOf("," + Vendor.ToUpper() + ",") > -1))
                                                    {
                                                        envwork.PageList.Add(staticpage);
                                                        ismatchedpage = true;
                                                        isbreak = true;
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    if ((staticalbum.RelateRCP.No == 80001 && ("," + staticpage.RelateToVersionString.ToUpper() + ",").IndexOf("," + artwork.ToUpper() + ",") > -1))
                                                    {
                                                        envwork.PageList.Add(staticpage);
                                                        ismatchedpage = true;
                                                        isbreak = true;
                                                        break;
                                                    }
                                                }
                                                break;
                                            case OptionEnum.C3:
                                                if (Vendor != "")
                                                {
                                                    if ((staticalbum.RelateRCP.No == 80001 && ("," + staticpage.RelateToVersionString.ToUpper() + ",").IndexOf("," + artwork.ToUpper()+"-"+Colour + ",") > -1) && (("," + staticpage.AliasName. ToUpper() + ",").IndexOf("," + Vendor.ToUpper() + ",") > -1))
                                                    {
                                                        envwork.PageList.Add(staticpage);
                                                        ismatchedpage = true;
                                                        isbreak = true;
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    if ((staticalbum.RelateRCP.No == 80001 && ("," + staticpage.RelateToVersionString.ToUpper() + ",").IndexOf("," + artwork.ToUpper() + ",") > -1))
                                                    {
                                                        envwork.PageList.Add(staticpage);
                                                        ismatchedpage = true;
                                                        isbreak = true;
                                                        break;
                                                    }
                                                }
                                                break;
                                        }
                                        if (isbreak)
                                            break;
                                    }
                                }
                                if (staticalbum.RelateRCP.No == 80001 && !ismatchedpage)
                                {
                                    Universal.No80001Err += "无对应80001的雷雕参数，请检查。" +Environment.NewLine;
                                }
                                //如果沒有對應的Page，則取第一個 Env 的 第一個 Page放入 envwork
                                if (!ismatchedpage)
                                {
                                    //若沒有第一個ENV，就加入原有的Page

                                    if(staticalbum.ENVCount == 0)
                                        envwork.PageList.Add(page);
                                    else
                                        envwork.PageList.Add(staticalbum.ENVList[0].PageList[0]);

                                }
                            }
                            if (ismatchedpage)
                                break;
                        }
                    }

                    //For CPD Relate Page Run No 

                    int pagerunno = 0;

                    foreach(PageClass pagerun in envwork.PageList)
                    {
                        pagerun.PageRunNo = pagerunno;
                        pagerunno++;
                    }
                }

                if (envwork.PageList.Count != env.PageList.Count)
                {
                    break;
                }
                else
                    ENVList.Add(envwork);
                
                //不要忘了還有CPD
                CPD = album.CPD;
            }
        }
        public AlbumClass Clone()
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, this);
                ms.Position = 0;

                return (AlbumClass)formatter.Deserialize(ms);
            }
        }
        /// <summary>
        /// Start Loading Recipe Path
        /// </summary>
        /// <param name="rcppath"></param>
        public void Load(string rcppath)
        {
            PassInfo = new PassInfoClass();
            PassInfo.RcpNo = RelateRCP.No;
            PassInfo.OperatePath = rcppath + "\\" + RelateRCP.RcpNoString;
            
            Load();
        }
        public void Load()
        {
            //先自殺
            Suicide();

            CPD = new CPDClass(VERSION,OPTION);
            //GetBMP(PassInfo.OperatePath + "\\VIEW" + Universal.GlobalImageTypeString, ref CPD.bmpVIEW);
            CPD.LoadBMP(PassInfo.OperatePath);

            LoadSubRoutine();

            FromString((string)DATASET.Tables[OPDataTableEnum.ALBUMDB.ToString()].Rows[0]["AlbumData"]);

            if((string)DATASET.Tables[OPDataTableEnum.ALBUMDB.ToString()].Rows[0]["CPDData"] == "XXX")
            {
                DATASET.Tables[OPDataTableEnum.ALBUMDB.ToString()].Rows[0]["CPDData"] = "";
                UpdateTable("ALBUMDB");
            }

            //Very first Use
            //DATASET.Tables[OPDataTableEnum.ALBUMDB.ToString()].Rows[0]["CPDData"] = CPD.ToString();
            //UpdateTable("ALBUMDB");

            CPD.FromString((string)DATASET.Tables[OPDataTableEnum.ALBUMDB.ToString()].Rows[0]["CPDData"]);
            //ASNFromString((string)DATASET.Tables[OPDataTableEnum.ALBUMDB.ToString()].Rows[0]["AssignData"]);

            FillEnvList(DATASET.Tables[OPDataTableEnum.ENVDB.ToString()], PassInfo);
        }
        void LoadSubRoutine()
        {
            int i = 0;
            string SQLCMD = "";
            string DataConnString = DATACNNSTRING.Replace("%", PassInfo.OperatePath);

            if (DATACOMMAND != null)
            {
                i = DATACOMMAND.Length - 1;
                while (i > -1)
                {
                    DATACOMMAND[i].Dispose();
                    i--;
                }
            }
            DATACOMMAND = new OleDbCommand[(int)OPDataTableEnum.COUNT];

            if (DATACMDBUILDER != null)
            {
                i = DATACMDBUILDER.Length-1;
                while (i > -1)
                {
                    DATACMDBUILDER[i].Dispose();
                    i--;
                }
            }
            
            DATACMDBUILDER = new OleDbCommandBuilder[(int)OPDataTableEnum.COUNT];

            if (DATAADAPTER != null)
            {
                i = DATAADAPTER.Length - 1;
                while (i > 0)
                {
                    DATAADAPTER[i].Dispose();
                    i--;
                }
            }

            DATAADAPTER = new OleDbDataAdapter[(int)OPDataTableEnum.COUNT];

            if (DATASET != null)
                DATASET.Dispose();

            DATASET = new DataSet();

            if (DATACONNECTION != null)
                DATACONNECTION.Dispose();

            DATACONNECTION = new OleDbConnection(DataConnString);

            DATACONNECTION.Open();
            DATACONNECTION.Close();

            i = 0;
            while (i < (int)OPDataTableEnum.COUNT)
            {
                SQLCMD = "SELECT * FROM " + ((OPDataTableEnum)i).ToString() + " ORDER BY No";
                DATAADAPTER[i] = new OleDbDataAdapter();
                DATACMDBUILDER[i] = new OleDbCommandBuilder(DATAADAPTER[i]);
                DATACOMMAND[i] = new OleDbCommand();
                DATACOMMAND[i].Connection = DATACONNECTION;
                

                //DATACOMMAND[i] = new  
                DATACMDBUILDER[i].QuotePrefix = "[";
                DATACMDBUILDER[i].QuoteSuffix = "]";
                
                DATACONNECTION.Open();

                DATAADAPTER[i].SelectCommand = new OleDbCommand(SQLCMD, DATACONNECTION);
                DATAADAPTER[i].Fill(DATASET, ((OPDataTableEnum)i).ToString());

                DATACONNECTION.Close();

                i++;
            }
            
            //DeleteTableRow("ENVDB", "DELETE FROM ENVDB WHERE [No] > 5000");

            #region Very First Use

            //DATASET.Tables[OPDataTableEnum.ALBUMDB.ToString()].Rows[0]["AlbumData"] = ToString();
            //DATASET.Tables[OPDataTableEnum.ALBUMDB.ToString()].Rows[0]["AssignData"] = "XXX";
            //UpdateTable("ALBUMDB");

            #endregion

        }
        //public void SaveNew()
        //{
        //    File.Copy(RCPPATPATH + "\\" + RcpClass.OrgRcpIndexString + "\\Album.mdb", RCPPATPATH + "\\" + RelateRCP.RcpIndexString + "\\Album.mdb");

        //    LoadSubRoutine();
        //    Save();
        //}
        public void Save()
        {
            #region Add Backup fuctions

            //CompactAccessDB(DATACNNSTRING, PassInfo.OperatePath + "\\Album.mdb");
            try
            {
                string BackupFileName = Universal.BACKUPDBPATH + "\\" + JzTimes.DateTimeSerialString + "-" + ACCDB.DataNow.Name + "-" + RelateRCP.ToESSString().Replace("/", "") + "-Album.mdb";
                File.Copy(PassInfo.OperatePath + "\\Album.mdb", BackupFileName);
            }
            catch
            {

            }

            #endregion

            DATASET.Tables[OPDataTableEnum.ALBUMDB.ToString()].Rows[0]["AlbumData"] = ToString();
            DATASET.Tables[OPDataTableEnum.ALBUMDB.ToString()].Rows[0]["CPDData"] = CPD.ToString();
            UpdateTable("ALBUMDB");

            CPD.SaveBMP(PassInfo.OperatePath);
            //CPD.SaveCPDItem(PassInfo.OperatePath);

            DATASET.Tables[OPDataTableEnum.ENVDB.ToString()].Rows.Clear();
            DeleteTableRow("ENVDB", "DELETE FROM ENVDB");

            //UpdateTable("ENVDB");
            //SaveBMP(PassInfo.OperatePath + "\\VIEW" + Universal.GlobalImageTypeString, ref CPD.bmpVIEW);

            //int i = DATASET.Tables[OPDataTableEnum.ENVDB.ToString()].Rows.Count - 1;

            //while(i > -1)
            //{
            //    DATASET.Tables[OPDataTableEnum.ENVDB.ToString()].Rows[i]["No"] = (int)DATASET.Tables[OPDataTableEnum.ENVDB.ToString()].Rows[i]["No"] + 10000;
            //    i--;
            //}

            //UpdateTable("ENVDB");

            //DATASET.Tables[OPDataTableEnum.ENVDB.ToString()].Rows.Clear();

            foreach (EnvClass env in ENVList)
            {
                DataRow newdatarow = DATASET.Tables[OPDataTableEnum.ENVDB.ToString()].NewRow();

                newdatarow["No"] = env.No;

                newdatarow["EnvData"] = env.ToString();
                newdatarow["PageData"] = env.ToPageString();
                newdatarow["AnalyzeData"] = env.ToAnalyzeString();

                env.SaveBMP();
                env.SaveLearnAnalyze();

                DATASET.Tables[OPDataTableEnum.ENVDB.ToString()].Rows.Add(newdatarow);
            }
            UpdateTable("ENVDB");

            //DATASET.Tables[OPDataTableEnum.ENVDB.ToString()].Rows.Clear();
            //DeleteTableRow("ENVDB", "DELETE FROM ENVDB WHERE [No] > 5000");
        }
        public void SaveAnalyze(PassInfoClass passinfo)
        {
            int i = 0;
            int j = 0;

            int rowcount = DATASET.Tables[OPDataTableEnum.ENVDB.ToString()].Rows.Count;

            while (i < rowcount)
            {
                if (passinfo.EnvNo == (int)DATASET.Tables[OPDataTableEnum.ENVDB.ToString()].Rows[i]["No"])
                {
                    foreach (EnvClass env in ENVList)
                    {
                        if (env.No == passinfo.EnvNo)
                        {
                            DATASET.Tables[OPDataTableEnum.ENVDB.ToString()].Rows[i]["AnalyzeData"] = env.ToAnalyzeString();

                            //env.SaveLearn();

                            UpdateTable("ENVDB");

                            break;
                        }
                    }
                    break;
                }
                i++;
            }
                    
        }
        void FromString(string str)
        {
            string[] strs = str.Split(Universal.SeperateCharA);

            rectDefine = StringtoRect(strs[0]);

        }
        public override string ToString()
        {
            string retStr = "";

            retStr = RecttoString(rectDefine) + Universal.SeperateCharA;
            retStr += "";

            return retStr;
        }
        //public string ASNToString()
        //{
        //    string str = "";
            
        //    foreach(AssignItemClass assign in AssignList)
        //    {
        //        str += assign.ToString() + Environment.NewLine;
        //    }

        //    str += "";

        //    return str;
        //}
        public string ToStatusString()
        {
            string retstr = "";

            switch (OPTION)
            {
                case JetEazy.OptionEnum.MAIN_SERVICE:
                case OptionEnum.MAIN_X6:
                    retstr = "[" + RelateRCP.No.ToString("00000-") + RelateRCP.Name + "(" + RelateRCP.Version + ")" + "]";// + "[" + UseCount.ToString() + "]";
                    break;
                default:
                    retstr = RelateRCP.No.ToString("00") + "(" + RelateRCP.Version + ")";// + "[" + UseCount.ToString() + "]";
                    break;
            }

            return retstr;
        }
        public string ToStatusStringVersion()
        {
            string retstr = "";

            retstr =  RelateRCP.Version ;

            return retstr;
        }
        public string ToPreProcessString()
        {
            string retstr = "";

            retstr = RelateRCP.No.ToString("00") + "(" + RelateRCP.Version + ")";

            return retstr;
        }
        public string ToRelateStaticString()
        {
            string Str = "";

            Str += "(" + RelateRCP.RcpNoString + ")" + RelateRCP.Name + "-" + RelateRCP.Version;

            return Str;
        }
        public void Suicide()
        {
            if(CPD != null)
                CPD.Suicide();

            if (DATACONNECTION != null)
            {
                DATACONNECTION.Close();
                DATACONNECTION.Dispose();
            }

            foreach(EnvClass env in ENVList)
            {
                env.Suicide();
            }

        }
        public void FillEnvList(DataTable envtbl,PassInfoClass passinfo)
        {
            #region Very First Use For First Time Initial Use

            //envtbl.Rows.Clear();
            //DeleteTableRow("ENVDB", "DELETE FROM ENVDB");

            //DataRow newdatarow = envtbl.NewRow();

            //newdatarow["No"] = 0;
            //EnvClass FirstENV = new EnvClass(0);

            //newdatarow["EnvData"] = FirstENV.ToString();
            //newdatarow["PageData"] = FirstENV.ToPageString();
            //newdatarow["AnalyzeData"] = FirstENV.ToAnalyzeString();

            //envtbl.Rows.Add(newdatarow);

            //UpdateTable("ENVDB");

            #endregion

            int i = 0;

            foreach(EnvClass env in ENVList)
            {
                env.Suicide();
            }

            ENVList.Clear();

            while(i < envtbl.Rows.Count)
            {
                if (envtbl.Rows[i]["EnvData"] != DBNull.Value)
                {
                    EnvClass env = new EnvClass(envtbl.Rows[i], PassInfo);

                    //env.PrintMessageAction += Env_PrintMessage;

                    ENVList.Add(env);
                }

                i++;
            }
        }
        //public void FillEnvAction(int envindex)
        //{
        //    EnvClass env = ENVList[envindex];

        //    env.PrintMessageAction += Env_PrintMessage;
        //}
        string RecttoString(Rectangle rect)
        {
            return rect.X.ToString().PadLeft(4) + "," + rect.Y.ToString().PadLeft(4) + "," + rect.Width.ToString().PadLeft(4) + "," + rect.Height.ToString().PadLeft(4);
        }
        Rectangle StringtoRect(string rectstr)
        {
            string[] str = rectstr.Split(',');
            return new Rectangle(int.Parse(str[0]), int.Parse(str[1]), int.Parse(str[2]), int.Parse(str[3]));
        }
        void UpdateTable(string tablename)
        {
            DATACONNECTION.Open();

            //DATACMDBUILDER[(int)(DataTableEnum)Enum.Parse(typeof(DataTableEnum), tablename, false)].GetDeleteCommand();
            DATAADAPTER[(int)(OPDataTableEnum)Enum.Parse(typeof(OPDataTableEnum), tablename, false)].Update(DATASET, tablename);

            DATACONNECTION.Close();

        }
        void DeleteTableRow(string tablename, string deletecommand)
        {
            DATACONNECTION.Open();

            DATACOMMAND[(int)(OPDataTableEnum)Enum.Parse(typeof(OPDataTableEnum), tablename, false)].CommandText = deletecommand;
            DATACOMMAND[(int)(OPDataTableEnum)Enum.Parse(typeof(OPDataTableEnum), tablename, false)].ExecuteNonQuery();

            DATACONNECTION.Close();
        }
        #region Normal Operation
        public void AddEnv(EnvClass env)
        {
            EnvClass newenv = env.Clone();
            newenv.No = LastENV.No + 1;
            
            ENVList.Add(newenv);
        }
        public void DelEnv(int index)
        {
            int i = ENVList.Count - 1;

            while(i > -1)
            {
                if(ENVList[i].No == index)
                {
                    ENVList.RemoveAt(i);
                    break;
                }
                i++;
            }
        }
        /// <summary>
        /// Copy Tree 的功能
        /// </summary>
        /// <param name="fromdir"></param>
        /// <param name="todir"></param>
        void Copy(string fromdir,string todir)
        {
            DirectoryInfo difrom = new DirectoryInfo(fromdir);
            DirectoryInfo dito = new DirectoryInfo(todir);

            CopyAll(difrom, dito);
        }
        void CopyAll(DirectoryInfo difrom,DirectoryInfo dito)
        {
            if (Directory.Exists(dito.FullName))
                Directory.Delete(dito.FullName, true);

            Directory.CreateDirectory(dito.FullName);

            foreach(FileInfo fi in difrom.GetFiles())
            {
                fi.CopyTo(Path.Combine(dito.FullName, fi.Name), true);
            }

            foreach(DirectoryInfo disourcesubdir in difrom.GetDirectories())
            {
                DirectoryInfo nextsubdir = dito.CreateSubdirectory(disourcesubdir.Name);
                CopyAll(disourcesubdir, nextsubdir);
            }
        }
        public void FillFirstEnvMover(Mover mover,CCDCollectionClass ccdcollection)
        {
            int i = 0;
            mover.Clear();

            if (ENVList.Count == 0 || (","+ INI.PRELOADSTATICNO + ",").IndexOf("," + RelateRCP.No.ToString() + ",") > -1 )
                return;

            EnvClass env = ENVList[0];

            foreach(PageClass page in env.PageList)
            {
                CCDRectRelateIndexClass cti = ccdcollection.GetRectRelateIndexData(page.CamIndex);

                page.AnalyzeRootArray[(int)PageOPTypeEnum.P00].GetShowMover(mover, cti.SizedRect.Location, cti.SizedRatio, i, new Point(ccdcollection.EXTEND, ccdcollection.EXTEND));

                i++;
            }
        }
        public void FillEnvMover(Mover mover, CCDCollectionClass ccdcollection, int index = 0)
        {
            int i = 0;
            mover.Clear();

            if (ENVList.Count == 0 || ("," + INI.PRELOADSTATICNO + ",").IndexOf("," + RelateRCP.No.ToString() + ",") > -1)
                return;

            EnvClass env = ENVList[0];

            //foreach (PageClass page in env.PageList)
            {
                PageClass page = env.PageList[index];//第几个页面
                CCDRectRelateIndexClass cti = ccdcollection.GetRectRelateIndexData(0);//只有一个相机

                page.AnalyzeRootArray[(int)PageOPTypeEnum.P00].GetShowMover(mover, cti.SizedRect.Location, cti.SizedRatio, index, new Point(ccdcollection.EXTEND, ccdcollection.EXTEND));

                //i++;
            }
        }
        public void FillFirstEnvResultMover(Mover mover, CCDCollectionClass ccdcollection)
        {
            int i = 0;
            mover.Clear();

            EnvClass env = ENVList[0];

            foreach (PageClass page in env.PageList)
            {
                CCDRectRelateIndexClass cti = ccdcollection.GetRectRelateIndexData(page.CamIndex);

                page.AnalyzeRootArray[(int)PageOPTypeEnum.P00].GetShowResultMover(mover, cti.SizedRect.Location, cti.SizedRatio, i, new Point(ccdcollection.EXTEND, ccdcollection.EXTEND));

                i++;
            }
        }
        public void FillCompoundMover(Mover mover)
        {
            mover.Clear();
            
            foreach(CPDItemClass cpditem in CPD.CPDItemList)
            {
                if (cpditem.NORMALPara.RelatePA.IndexOf("ENV") > -1)
                {
                    string[] strs = cpditem.NORMALPara.RelatePA.Split('-');

                    int envno = int.Parse(strs[0].Replace("ENV", ""));
                    int pageno = int.Parse(strs[1].Replace("PAGE", ""));
                    int pageopindex = int.Parse(strs[2]);

                    EnvClass env = GetEnv(envno);
                    PageClass page = env.GetPageRun(pageno);

                    if (page == null)
                        break;

                    PointF biaslocationF = cpditem.RatioRectEAG.GetRectF.Location;

                    biaslocationF.X = biaslocationF.X - CPD.RangeRectEAG.GetRectF.X;
                    biaslocationF.Y = biaslocationF.Y - CPD.RangeRectEAG.GetRectF.Y;

                    

                    switch(OPTION)
                    {
                        case OptionEnum.MAIN_X6:
                        case OptionEnum.MAIN_SDM1:
                        case OptionEnum.MAIN_SDM2:
                        case JetEazy.OptionEnum.MAIN_SERVICE:
                        case OptionEnum.MAIN_SDM3:
                            page.AnalyzeRootArray[pageopindex].GetShowResultMover(mover,
                                                                                                                             biaslocationF,
                                                                                                                             new SizeF(cpditem.NORMALPara.Ratio, cpditem.NORMALPara.Ratio),
                                                                                                                             0,
                                                                                                                             new Point(-(int)(cpditem.NORMALPara.iLeft * cpditem.NORMALPara.Ratio), -(int)(cpditem.NORMALPara.iTop * cpditem.NORMALPara.Ratio)));

                            break;
                        default:

                            page.AnalyzeRootArray[pageopindex].GetShowResultMover(mover,
                                                                                                                             biaslocationF,
                                                                                                                             new SizeF(cpditem.NORMALPara.Ratio, cpditem.NORMALPara.Ratio),
                                                                                                                             0,
                                                                                                                             new Point(0, 0));

                            break;
                    }
                }
                else
                {
                    string[] strs = cpditem.NORMALPara.RelatePA.Split(':');

                    int asnno = int.Parse(strs[0]);

                    ASNClass asn = ASNCollection.GetASN(asnno);

                    //Mapping ASN Item IsVeryGood First
                    
                    PointF biaslocationF = cpditem.RatioRectEAG.GetRectF.Location;

                    biaslocationF.X = biaslocationF.X - CPD.RangeRectEAG.GetRectF.X;
                    biaslocationF.Y = biaslocationF.Y - CPD.RangeRectEAG.GetRectF.Y;

                    //asn.GetShowResultMover(mover, biaslocationF, cpditem.RatioRectEAG.GetRectF.Size, 0, new Point(0, 0), INI.ISONLYSHOWNG);

                    foreach (ASNItemClass asnitem in asn.ASNItemList)
                    {
                        if (asnitem.RelateAnalyzeStr == "")
                        {
                            continue;
                        }

                        string[] analyzestrs = asnitem.RelateAnalyzeStr.Split(';');

                        foreach (string analyzecheckstr in analyzestrs)
                        {
                            if (analyzecheckstr == "")
                                continue;

                            string[] itemstrs = analyzecheckstr.Split('-');

                            int envindex = int.Parse(itemstrs[0]);
                            int pageindex = int.Parse(itemstrs[1]);
                            int pageoptypeindex = int.Parse(itemstrs[2]);
                            int analyzeno = int.Parse(itemstrs[3]);

                            EnvClass env = ENVList[envindex];
                            PageClass page = env.PageList[pageindex];
                            AnalyzeClass analyzeroot = page.AnalyzeRootArray[pageoptypeindex];

                            AnalyzeClass analyze = analyzeroot.GetAnalyze(analyzeno);

                            asnitem.IsVeryGood = analyze.IsVeryGood;

                            asnitem.GetShowResultMover(mover, biaslocationF,new SizeF(cpditem.NORMALPara.Ratio, cpditem.NORMALPara.Ratio), 0, new Point(0, 0), INI.ISONLYSHOWNG);
                        }
                    }
                }
            }
            //EnvClass env = ENVList[0];

            //foreach (PageClass page in env.PageList)
            //{
            //    CCDRectRelateIndexClass cti = ccdcollection.GetRectRelateIndexData(page.CamIndex);

            //    page.AnalyzeRootArray[(int)PageOPTypeEnum.P00].GetShowResultMover(mover, cti.SizedRect.Location, cti.SizedRatio, i, new Point(ccdcollection.EXTEND, ccdcollection.EXTEND));

            //    i++;
            //}
        }

      
        public void RecodeRepoer()
        {
            bool isok = true;
            foreach (string strBar in Universal.OLDBARCODELIST)
            {
                if (strBar == JzToolsClass.PassingBarcode)
                    isok = false;
            }

            if (!isok)
                return;
            

            Universal.OLDBARCODELIST.Add(JzToolsClass.PassingBarcode);

            while (Universal.OLDBARCODELIST.Count - 10 > 0)
                Universal.OLDBARCODELIST.RemoveAt(0);

            INI.RecodeRepoer recode = new INI.RecodeRepoer();

            EnvClass env = ENVList[0];
            foreach (PageClass page in env.PageList)
            {
                foreach (AnalyzeClass analyzeClass in page.AnalyzeRootArray)
                    FindFailRect(analyzeClass, ref recode);
            }
            INI.ReCodeRepoer(recode);
        }

        void FindFailRect(AnalyzeClass analyzeClass, ref INI.RecodeRepoer recode)
        {
            if (!analyzeClass.IsVeryGood)
            {
                if (analyzeClass.PassInfo.RcpNo == 80000)
                {
                    recode.isScrewNG = true;
                }
                else if (analyzeClass.PassInfo.RcpNo == 80002)
                {
                    recode.isScrewNG = true;
                }
                else if (analyzeClass.PassInfo.RcpNo == 80001)
                {
                    //if (analyzeClass.OCRPara.OCRMethod == OCRMethodEnum.MAPPING)
                    //    recode.isSNNG = true;
                    //else
                        recode.isLaserNG = true;

                }
                else
                    recode.isKeycapNG = true;

                // analyzeClass.bmpPATTERN.Save("D:\\pattern.png");
            }

            foreach (AnalyzeClass analyze in analyzeClass.BranchList)
                FindFailRect(analyze, ref recode);
        }

        public List<RectangleF> FillCompoundMoverR3( bool isbarcode=false)
        {
            //mover.Clear();

            List<RectangleF> list = new List<RectangleF>();
            foreach (CPDItemClass cpditem in CPD.CPDItemList)
            {
                if (cpditem.NORMALPara.RelatePA.IndexOf("ENV") > -1)
                {
                    string[] strs = cpditem.NORMALPara.RelatePA.Split('-');

                    int envno = int.Parse(strs[0].Replace("ENV", ""));
                    int pageno = int.Parse(strs[1].Replace("PAGE", ""));
                    int pageopindex = int.Parse(strs[2]);

                    EnvClass env = GetEnv(envno);
                    PageClass page = env.GetPageRun(pageno);

                    //PointF biaslocationF = cpditem.RatioRectEAG.GetRectF.Location;


                    FindFailRect(page.AnalyzeRootArray[pageopindex], ref list,isbarcode);
                    
                    //biaslocationF.X = biaslocationF.X - CPD.RangeRectEAG.GetRectF.X;
                    //biaslocationF.Y = biaslocationF.Y - CPD.RangeRectEAG.GetRectF.Y;

                    //page.AnalyzeRootArray[pageopindex].GetShowResultMover(mover, biaslocationF, new SizeF(cpditem.NORMALPara.Ratio, cpditem.NORMALPara.Ratio), 0, new Point(0, 0));
                }
                else
                {
                    string[] strs = cpditem.NORMALPara.RelatePA.Split(':');

                    int asnno = int.Parse(strs[0]);

                    ASNClass asn = ASNCollection.GetASN(asnno);

                    //Mapping ASN Item IsVeryGood First

                    PointF biaslocationF = cpditem.RatioRectEAG.GetRectF.Location;

                    biaslocationF.X = biaslocationF.X - CPD.RangeRectEAG.GetRectF.X;
                    biaslocationF.Y = biaslocationF.Y - CPD.RangeRectEAG.GetRectF.Y;

                    //asn.GetShowResultMover(mover, biaslocationF, cpditem.RatioRectEAG.GetRectF.Size, 0, new Point(0, 0), INI.ISONLYSHOWNG);

                    foreach (ASNItemClass asnitem in asn.ASNItemList)
                    {
                        if (asnitem.RelateAnalyzeStr == "")
                        {
                            continue;
                        }

                        string[] analyzestrs = asnitem.RelateAnalyzeStr.Split(';');

                        foreach (string analyzecheckstr in analyzestrs)
                        {
                            if (analyzecheckstr == "")
                                continue;

                            string[] itemstrs = analyzecheckstr.Split('-');

                            int envindex = int.Parse(itemstrs[0]);
                            int pageindex = int.Parse(itemstrs[1]);
                            int pageoptypeindex = int.Parse(itemstrs[2]);
                            int analyzeno = int.Parse(itemstrs[3]);

                            EnvClass env = ENVList[envindex];
                            PageClass page = env.PageList[pageindex];
                            AnalyzeClass analyzeroot = page.AnalyzeRootArray[pageoptypeindex];

                            AnalyzeClass analyze = analyzeroot.GetAnalyze(analyzeno);

                            asnitem.IsVeryGood = analyze.IsVeryGood;

                            //asnitem.GetShowResultMover(mover, biaslocationF, new SizeF(cpditem.NORMALPara.Ratio, cpditem.NORMALPara.Ratio), 0, new Point(0, 0), INI.ISONLYSHOWNG);
                        }
                    }
                }

               
            }
            //EnvClass env = ENVList[0];

            //foreach (PageClass page in env.PageList)
            //{
            //    CCDRectRelateIndexClass cti = ccdcollection.GetRectRelateIndexData(page.CamIndex);

            //    page.AnalyzeRootArray[(int)PageOPTypeEnum.P00].GetShowResultMover(mover, cti.SizedRect.Location, cti.SizedRatio, i, new Point(ccdcollection.EXTEND, ccdcollection.EXTEND));

            //    i++;
            //}

            return list;
        }

        void FindFailRect(AnalyzeClass analyzeClass, ref List<RectangleF> rectangles,bool isbarcode)
        {
            if (!analyzeClass.IsVeryGood)
            {
              
                //if (isbarcode)
                //{
                //    bool isok = true;
                //    if (analyzeClass.OCRPara.OCRMethod == OCRMethodEnum.CODE128)
                //    {
                //        isok = false;
                //    }
                //    if (analyzeClass.OCRPara.OCRMethod == OCRMethodEnum.MAPPING)
                //    {
                //        isok = false;
                //    }

                //    if (isok)
                //        return;
                //}
                //else
                //{
                //    bool isok = true;
                //    if (analyzeClass.OCRPara.OCRMethod == OCRMethodEnum.CODE128)
                //    {
                //        isok = false;
                //    }
                //    if (analyzeClass.OCRPara.OCRMethod == OCRMethodEnum.MAPPING)
                //    {
                //        isok = false;
                //    }

                //    if (!isok)
                //        return;
                //}
            
                Mover mover = analyzeClass.myMover;
                if (mover.Count > 0)
                {
                    GraphicalObject grobj = mover[0].Source;

                    if (grobj is JzRectEAG)
                    {
                        JzRectEAG rectEAG = (JzRectEAG)grobj;
                        rectangles.Add(rectEAG.GetRect);
                        return;
                    }
                }
            }

            foreach (AnalyzeClass analyze in analyzeClass.BranchList)
                FindFailRect(analyze, ref rectangles, isbarcode);
        }
        AnalyzeClass GetInsideAnalyze(string analyzestr)
        {
            AnalyzeClass retanalyze = null;

            string[] strs = analyzestr.Split('-');
            
            return retanalyze;
        }
        public EnvClass GetEnv(int envno)
        {
            EnvClass retenv = null;

            foreach(EnvClass env in ENVList)
            {
                if(env.No == envno)
                {
                    retenv = env;
                    break;
                }
            }

            return retenv;
        }
        public EnvClass GetEnvByIndex(int envindex)
        {
            EnvClass retenv = ENVList[envindex];
            
            return retenv;
        }
        #endregion

        #region Application Operation

        TrainMessageForm TRAINFORM;
        Allinone.FormSpace.LoadingPARForm LoadingParForm;
        //List<string> myProcessStringList = new List<string>();

        /// <summary>
        /// 執行預備的多執行緒工作
        /// </summary>
        /// <param name="IsMultiThread">是否启用多线程</param>
        public void A00_TrainProcess(bool IsMultiThread = false)
        {
            bool isgood = true;

            ActionDefined(true);

            ResetRunStatus();

            if (IsMultiThread && Universal.IsMultiThread)
            {
                LoadingParForm = new FormSpace.LoadingPARForm(ToPreProcessString());
                LoadingParForm.Show();
                LoadingParForm.Refresh();
            }
            else
            {
                TRAINFORM = new TrainMessageForm(ToPreProcessString());
                TRAINFORM.Show();
                TRAINFORM.Refresh();
            }

            foreach (EnvClass env in ENVList)
            {
                isgood &= env.A00_TrainProcess(IsMultiThread);

                if (!isgood)
                    break;
            }
            if (IsMultiThread && Universal.IsMultiThread)
            {
                LoadingParForm.Dispose();
            }
            else
            {
                if (isgood)
                    TRAINFORM.SetComplete();
                else
                    TRAINFORM.SetCancel();
            }
            switch(VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch(OPTION)
                    {
                        case OptionEnum.MAIN_SDM3:
                        case OptionEnum.MAIN_SDM2:
                            SDM2Test();
                            break;
                    }
                    break;
            }

            ActionDefined(false);
            //TRAINFORM.Dispose();
        }
        /// <summary>
        /// 在反饋回TrainMessageForm時要做的動作
        /// </summary>
        /// <param name="isaction"></param>
        void ActionDefined(bool isaction)
        {   
            if(isaction)
            {
                foreach(EnvClass env in ENVList)
                {
                    env.PrintMessageAction += Env_PrintMessageAction;
                    env.ActionDefined(isaction);
                }
            }
            else
            {
                foreach (EnvClass env in ENVList)
                {
                    env.PrintMessageAction -= Env_PrintMessageAction;
                    env.ActionDefined(isaction);
                }
            }
        }
        private void Env_PrintMessageAction(List<string> processstringlist)
        {
            if (TRAINFORM != null)
                TRAINFORM.SetString(processstringlist);
        }

        /// <summary>
        /// 執行預備的多執行緒工作
        /// </summary>
        public bool A08_RunProcess(PageOPTypeEnum pageoptype)
        {
            bool isgood = true;

            EnvClass env = ENVList[EnvRunIndex];

            //myProcessStringList.Clear();
            //myProcessStringList.Add("Start " + env.ToEnvString() + " Preprocess.");

            isgood &= env.A08_RunProcess(pageoptype);

            return isgood;
        }
        public bool A08_RunProcess(PageOPTypeEnum pageoptype,int iPageIndex)
        {
            //bool isgood = true;

            //EnvClass env = ENVList[EnvRunIndex];

            ////myProcessStringList.Clear();
            ////myProcessStringList.Add("Start " + env.ToEnvString() + " Preprocess.");

            //isgood &= env.A08_RunProcess(pageoptype, iPageIndex);

            //return isgood;
            return A08_RunProcess(pageoptype, iPageIndex, EnvRunIndex);
        }
        public bool A08_RunProcess(PageOPTypeEnum pageoptype, int iPageIndex, int iEnvindex)
        {
            bool isgood = true;

            EnvClass env = ENVList[iEnvindex];

            //myProcessStringList.Clear();
            //myProcessStringList.Add("Start " + env.ToEnvString() + " Preprocess.");

            isgood &= env.A08_RunProcess(pageoptype, iPageIndex);

            return isgood;
        }
        /// <summary>
        /// 執行預備的多執行緒工作
        /// </summary>
        public bool A08_RunProcess(PageOPTypeEnum pageoptype ,List<int> NoTestIndexPage)
        {
            bool isgood = true;

            EnvClass env = ENVList[EnvRunIndex];

            //myProcessStringList.Clear();
            //myProcessStringList.Add("Start " + env.ToEnvString() + " Preprocess.");

            isgood &= env.A08_RunProcess(pageoptype,NoTestIndexPage);

            return isgood;
        }
        public void FillRunStatus(WorkStatusCollectionClass runstatuscollection)
        {
            EnvClass env = ENVList[EnvRunIndex];

            env.FillRunStatus(runstatuscollection);
        }
        public void GetDebugImage(string debugstr)
        {





        }
        public void SetEnvRunIndex(int index)
        {
            EnvRunIndex = index;
        }
        public void ResetRunStatus()
        {
            foreach(EnvClass env in ENVList)
            {
                env.ResetRunStatus();
            }
        }

        /// <summary>
        /// 將 ASN CPD 和 內部的 Analyze 取得關聯
        /// </summary>
        public void RelateToASN()
        {
            int envindex = 0;
            int pageindex = 0;
            int pageoptypeindex = 0;

            //Clear All Relate String
            foreach(ASNClass asn in ASNCollection.myDataList)
            {
                foreach(ASNItemClass asnitem in asn.ASNItemList)
                {
                    asnitem.RelateAnalyzeStr = "";
                    asnitem.IsVeryGood = false;
                }
            }

            //Relate ASN to Every Analyze
            envindex = 0;
            foreach(EnvClass env in ENVList)
            {
                pageindex = 0;
                foreach(PageClass page in env.PageList)
                {
                    pageoptypeindex = 0;
                    foreach(AnalyzeClass analyzeroot in page.AnalyzeRootArray)
                    {
                        //foreach (ASNClass asn in ASNCollection.myDataList)
                        //{
                        //    foreach (ASNItemClass asnitem in asn.ASNItemList)
                        //    {
                        //        asnitem.RelateAnalyzeStr = "";
                        //    }

                        //}
                                    analyzeroot.SetRelateASN(ASNCollection, envindex, pageindex, pageoptypeindex);

                        /*
                        //foreach(AnalyzeClass branchanalyze in analyzeroot.BranchList)
                        //{
                        //    foreach (ASNClass asn in ASNCollection.myDataList)
                        //    {
                        //        if (branchanalyze.RelateASN == asn.ToASNString())
                        //        {
                        //            foreach (ASNItemClass asnitem in asn.ASNItemList)
                        //            {
                        //                if(branchanalyze.RelateASNItem == asnitem.ToASNItemRelateString())
                        //                {
                        //                    asnitem.RelateAnalyzeStr += env.No.ToString() + "-" 
                        //                        + page.No.ToString() + "-" 
                        //                        + analyzeroot.PageOPtype.ToString() + "-" 
                        //                        + branchanalyze.No.ToString() + ",";
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                        */

                        pageoptypeindex++;
                    }

                    pageindex++;
                }

                envindex++;
            }
        }
        
        /// <summary>
        /// FUCK, The Damn Function is not working!!! FUCK
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="mdwfilename"></param>
        void CompactAccessDB(string connectionString, string mdwfilename)
        {
            //object[] oParams;

            ////string Src = System.Configuration.confi
            
            //JRO.JetEngine objJRO = new JRO.JetEngine();
            ////create an inctance of a Jet Replication Object
            ////object objJRO =
            ////  Activator.CreateInstance(Type.GetTypeFromProgID("JRO.JetEngine"));

            ////filling Parameters array
            ////cnahge "Jet OLEDB:Engine Type=5" to an appropriate value
            //// or leave it as is if you db is JET4X format (access 2000,2002)
            ////(yes, jetengine5 is for JET4X, no misprint here)
            //oParams = new object[] {
            //connectionString,
            //"Provider=Microsoft.Jet.OLEDB.12.0;Data" +
            //" Source=C:\\tempdb.mdb;Jet OLEDB:Database Password=12892414;"};

            ////objJRO.CompactDatabase()

            ////invoke a CompactDatabase method of a JRO object
            ////pass Parameters array
            //objJRO.GetType().InvokeMember("CompactDatabase",
            //    System.Reflection.BindingFlags.InvokeMethod,
            //    null,
            //    objJRO,
            //    oParams);

            ////database is compacted now
            ////to a new file C:\\tempdb.mdw
            ////let's copy it over an old one and delete it

            //System.IO.File.Delete(mdwfilename);
            //System.IO.File.Move("C:\\tempdb.mdb", mdwfilename);

            ////clean up (just in case)
            //System.Runtime.InteropServices.Marshal.ReleaseComObject(objJRO);
            //objJRO = null;

            //var dbe = new Microsoft.Office.Interop.Access.Dao.DBEngine();

            //dbe.CompactDatabase(mdwfilename, "C:\\tempdb.mdb");

        }

        public void AddPage(PageClass orgpage)
        {
            PageClass page = orgpage.Clone();

            page.No = ENVList[0].PageList.Count;
            page.GetPassInfoIncludeAnalyze(ENVList[0].PageList[0].PassInfo);

            ENVList[0].PageList.Add(page);
        }
        public void ReplacePage(PageClass replacepage,int replaceindex)
        {
            PageClass page = replacepage.Clone();

            PageClass deletepage = ENVList[0].PageList[replaceindex];

            int deleteno = deletepage.No;
            PassInfoClass deletepassinfo = new PassInfoClass(deletepage.PassInfo, OPLevelEnum.COPY);
            deletepage.Suicide();

            ENVList[0].PageList.RemoveAt(replaceindex);
            
            page.No = deleteno;
            page.GetPassInfoIncludeAnalyze(deletepassinfo);
            page.RelateToVersionString = deletepage.RelateToVersionString;

            ENVList[0].PageList.Insert(replaceindex, page);
        }
        #endregion

        public void SDM2Test()
        {
            m_EnvNow = new EnvClass();

            int ipageindex = 0;
            foreach (PageClass page in ENVList[0].PageList)
            {
                if (page.AliasName == "ALIGN")
                    continue;

                //获取位置
                if (string.IsNullOrEmpty(page.sPagePostion))
                {
                    //没有的位置的页面不添加
                    //PageClass page1 = page.Clone();
                    //page1.PageRunNo = ipageindex;
                    //page1.PageRunPos = "0,0,0";
                    //ipageindex++;

                    //m_EnvNow.PageList.Add(page1);
                }
                else
                {
                    foreach (string pos in page.PagePostionList)
                    {
                        PageClass page1 = page.Clone();
                        page1.PageRunNo = ipageindex;
                        page1.PageRunPos = pos;
                        ipageindex++;

                        m_EnvNow.PageList.Add(page1);
                    }
                }
            }
            if (m_EnvNow.PageList.Count > 0)
            {
                m_EnvNow.EnvAutoReportIndex();
                ////训练线程
                //System.Threading.Thread thread_Train = new System.Threading.Thread(DLTrainSDM2);
                //thread_Train.Start(new object());

                //m_EnvTrainOK = false;
                m_EnvNow.ResetTrainStatus();
                //m_EnvNow.A00_TrainProcess(true);
                //m_EnvTrainOK = true;
            }
        }

        /// <summary>
        /// 设定page的测试状态
        /// </summary>
        /// <param name="ipage">页面</param>
        /// <param name="ison">是否测试完成</param>
        public void SetPageTestState(int envindex, int ipage, bool ison)
        {
            EnvClass env = ENVList[envindex];
            env.PageList[ipage].CalComplete = ison;
        }

        public void SetOffset(int envindex, int pageindex, Point ePoint, bool ison)
        {
            EnvClass env = ENVList[EnvRunIndex];
            env.SetOffset(pageindex, ePoint, ison);


        }

        /// <summary>
        /// 设定page的测试状态
        /// </summary>
        /// <param name="ipage">页面</param>
        /// <param name="ison">是否测试完成</param>
        public void SetPageTestState(int ipage, bool ison)
        {
            //EnvClass env = ENVList[EnvRunIndex];
            //env.PageList[ipage].CalComplete = ison;
            SetPageTestState(EnvRunIndex, ipage, ison);
        }

        public void SetOffset(int pageindex, Point ePoint,bool ison)
        {
            //EnvClass env = ENVList[EnvRunIndex];
            //env.SetOffset(pageindex, ePoint,ison);
            SetOffset(EnvRunIndex, pageindex, ePoint, ison);


        }

        //public bool GetPageTestState(int ipage)
        //{
        //    EnvClass env = ENVList[EnvRunIndex];
        //    return env.PageList[ipage].CalComplete;
        //}
        //public bool GetLastPageTestState()
        //{
        //    EnvClass env = ENVList[EnvRunIndex];
        //    return env.PageList[env.PageList.Count - 1].CalComplete;
        //}

        /// <summary>
        /// 获取所有页面测试完成信号
        /// </summary>
        /// <returns></returns>
        public bool GetAllPageTestComplete()
        {
            bool ret = true;
            EnvClass env = ENVList[EnvRunIndex];
            foreach (var item in env.PageList)
            {
                ret &= item.CalComplete;
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
            Bitmap bmpTMP = new Bitmap(bmp);

            bmpTMP.Save(bmpfilestr, Universal.GlobalImageFormat);

            bmpTMP.Dispose();
        }

        #endregion

        public delegate void PreProcessHandler(List<string> processstringlist);
        public event PreProcessHandler PreProcessAction;
        public void OnPreProcess(List<string> processstringlist)
        {
            if (PreProcessAction != null)
            {
                PreProcessAction(processstringlist);
            }
        }
    }
    public class AlbumCollectionClass
    {
        int Indicator = 0;

        public int PRELOADCOUNT = 16;
        public string RCPPATH = "";

        public List<AlbumClass> StaticAlbumList = new List<AlbumClass>();
        public List<AlbumClass> AlbumList = new List<AlbumClass>();

        int StaticCount
        {
            get
            {
                return StaticAlbumList.Count;
            }
        }

        public AlbumClass AlbumWork;
        public AlbumClass AlbumNow
        {
            get
            {
                if (Indicator < StaticCount)
                    return StaticAlbumList[Indicator];
                else
                    return AlbumList[Indicator - StaticCount];
            }
        }
        public AlbumCollectionClass()
        {


        }
        public void AddStatic(AlbumClass staticalbum) //在新增時直接LOAD
        {
            StaticAlbumList.Add(staticalbum);
            staticalbum.Load(RCPPATH);
        }
        /// <summary>
        /// 新增一個ALBUM並且把相關的資料載入
        /// </summary>
        /// <param name="album"></param>
        /// <returns></returns>
        public int Add(AlbumClass album)
        {
            int existindicator = FindNormalIndicator(album.RelateRCP.No);

            if (existindicator < 0)
            {
                if (AlbumList.Count + 1 <= PRELOADCOUNT)
                {
                    AlbumList.Add(album);
                    album.Load(RCPPATH);
                }
                else
                {
                    //若超出 PRELOADCOUNT 則把最少用的那個幹掉
                    int orgindex = 0;
                    List<string> CountList = new List<string>();

                    foreach(AlbumClass orgalbum in AlbumList)
                    {
                        string str = orgalbum.UseCount.ToString("0000000") + "," + orgindex.ToString("0000");

                        CountList.Add(str);

                        orgindex++;
                    }

                    CountList.Sort();

                    orgindex = int.Parse(CountList[0].Split(',')[1]);

                    AlbumClass delalbum = AlbumList[orgindex];

                    delalbum.Suicide();

                    AlbumList.RemoveAt(orgindex);

                    AlbumList.Add(album);
                    album.Load(RCPPATH);
                }
            }

            GotoIndex(album.RelateRCP.No);

            return existindicator;
        }

        /// <summary>
        /// 將Page加入 Static
        /// </summary>
        /// <param name="rcpno"></param>
        /// <param name="newpage"></param>
        public void AddPage(int rcpno,PageClass orgpage)
        {
            int i = 0;
            int index = 0;

            while(i < StaticAlbumList.Count)
            {
                if(rcpno == StaticAlbumList[i].RelateRCP.No)
                {
                    index = i;
                    break;
                }
                i++;
            }

            AlbumClass staticalbum = StaticAlbumList[index];

            staticalbum.IsNeedToReTrain = true;
            staticalbum.AddPage(orgpage);
        }
        /// <summary>
        /// 取代現有的Page
        /// </summary>
        /// <param name="rcpno"></param>
        /// <param name="replacepage"></param>
        /// <param name="replaceindex"></param>
        public void ReplacePage(int rcpno,PageClass replacepage,int replaceindex)
        {
            int i = 0;
            int index = 0;

            while (i < StaticAlbumList.Count)
            {
                if (rcpno == StaticAlbumList[i].RelateRCP.No)
                {
                    index = i;
                    break;
                }
                i++;
            }

            AlbumClass staticalbum = StaticAlbumList[index];

            staticalbum.IsNeedToReTrain = true;
            staticalbum.ReplacePage(replacepage, replaceindex);
        }

        public void ProcessStaticAlbum(bool iscancel)
        {
            foreach(AlbumClass staticalbum in StaticAlbumList)
            {
                if(staticalbum.IsNeedToReTrain)
                {
                    if(iscancel)
                    {
                        staticalbum.Load();
                    }
                    else
                    {
                        staticalbum.Save();
                    }
                    staticalbum.A00_TrainProcess();
                    staticalbum.IsNeedToReTrain = false;
                }
            }
        }
        
        /// <summary>
        /// 直接刪除所有在的位置
        /// </summary>
        /// <param name="indicator"></param>
        public void Del(int indicator)
        {
            AlbumList[indicator].Suicide();
            AlbumList.RemoveAt(indicator);
        }
        public void Del(string delstring)
        {


        }
        /// <summary>
        /// 在新增和複製參數時，若要取消參數並回到前一個參數位置
        /// </summary>
        /// <param name="fromindex"></param>
        public void DelLast(int fromindex)  
        {
            //AlbumClass lastalbum = AlbumList[AlbumList.Count - 1];
            //lastalbum.Suicide();
            //AlbumList.RemoveAt(AlbumList.Count - 1);
            Del(AlbumList.Count - 1);
            GotoIndex(fromindex);
        }
        /// <summary>
        /// 在選擇參數畫面結束後，檢查ALBUMCOllection中是否有已刪除的Recipe，並將他從AlbumCollection移除
        /// </summary>
        /// <param name="rcpdb"></param>
        public void Del(RcpDBClass rcpdb)
        {
            int i = 0;
            List<int> RemoveList = new List<int>();

            foreach(AlbumClass album in AlbumList)
            {
                if(rcpdb.FindIndex(album.RelateRCP.No) < 0)
                {
                    RemoveList.Add(i);
                }
                i++;
            }

            i = RemoveList.Count - 1;

            while(i > -1)
            {
                Del(RemoveList[i]);
                i--;
            }
        }
        /// <summary>
        /// 尋找對應參數編號的位置
        /// </summary>
        /// <param name="rcpno"></param>
        /// <returns></returns>
        public int FindIndicator(int rcpno)
        {
            int i = 0;
            int ret = -1;
            
            foreach (AlbumClass album in AlbumList)
            {
                if (album.RelateRCP.No == rcpno)
                {
                    ret = i;

                    ret += StaticCount;

                    break;
                }
                i++;
            }

            return ret;
        }
        public int FindNormalIndicator(int rcpno)
        {
            int i = 0;
            int ret = -1;

            foreach (AlbumClass album in AlbumList)
            {
                if (album.RelateRCP.No == rcpno)
                {
                    ret = i;
                    break;
                }
                i++;
            }

            return ret;
        }
        public int FindStaticIndicator(int rcpno)
        {
            int i = 0;
            int ret = -1;

            foreach (AlbumClass album in StaticAlbumList)
            {
                if (album.RelateRCP.No == rcpno)
                {
                    ret = i;
                    break;
                }
                i++;
            }

            return ret;
        }
        public void FindStatic(string staticinsdexstr)
        {

        }
        /// <summary>
        /// 直接跳到指定的 RcpIndex
        /// </summary>
        /// <param name="rcpno"></param>
        public void GotoIndex(int rcpno)
        {
            int i = 0;

            bool isstatic = false;

            i = 0;
            foreach (AlbumClass staticalbum in StaticAlbumList)
            {
                if (staticalbum.RelateRCP.No == rcpno)
                {
                    Indicator = i;
                    isstatic = true;
                    break;
                }
                i++;
            }

            if (!isstatic)
            {
                i = 0;
                foreach (AlbumClass album in AlbumList)
                {
                    if (album.RelateRCP.No == rcpno)
                    {
                        Indicator = i;
                        Indicator += StaticCount;
                        break;
                    }
                    i++;
                }
            }
        }
        /// <summary>
        /// 確認指定的 RcpIndex 是否在
        /// </summary>
        /// <param name="rcpno"></param>
        public bool CheckIndex(int rcpno)
        {
            bool ret = false;
            int i = 0;

            foreach (AlbumClass album in AlbumList)
            {
                if (album.RelateRCP.No == rcpno)
                {
                    ret = true;
                    break;
                }
                i++;
            }

            return ret;
        }
        /// <summary>
        /// 現在指定的Album在計算時使用了一次
        /// </summary>
        public void UseOnce()
        {
            AlbumNow.UseCount++;
        }
        /// <summary>
        /// 顯示現在有多少Album在AlbumCollection裏
        /// </summary>
        /// <returns></returns>
        public string ToStatusString()
        {
            string retstr = "";

            foreach (AlbumClass staticalbum in StaticAlbumList)
            {
                retstr += staticalbum.ToStatusString() + ",";
            }
            

            foreach (AlbumClass album in AlbumList)
            {
                retstr += album.ToStatusString() + ",";

            }
            retstr = retstr.Substring(0, retstr.Length - 1);
            return retstr;
        }

        /// <summary>
        /// 顯示現在有多少Album在AlbumCollection裏
        /// </summary>
        /// <returns></returns>
        public string ToStatusStringVersion()
        {
            string retstr = "";

            foreach (AlbumClass staticalbum in StaticAlbumList)
            {
                retstr += staticalbum.ToStatusStringVersion() + ";";
            }


            foreach (AlbumClass album in AlbumList)
            {
                retstr += album.ToStatusStringVersion() + ";";

            }
            retstr = retstr.Substring(0, retstr.Length - 1);
            return retstr;
        }

        /// <summary>
        /// 取得合起來要運算的 AlbumWork
        /// </summary>
        /// <returns></returns>
        public bool GetAlbumWork(string opstring, TestMethodEnum testmethod = TestMethodEnum.QSMCSF)
        {
            AlbumWork = new AlbumClass(AlbumNow, StaticAlbumList, opstring, testmethod);

            return AlbumWork.ENVList.Count == AlbumNow.ENVList.Count;
        }


        public AlbumClass GetStaticAlbum(int rcpno)
        {
            AlbumClass retalbum = null;

            foreach(AlbumClass album in StaticAlbumList)
            {
                if(album.RelateRCP.No == rcpno)
                {
                    retalbum = album;
                    break;
                }
            }

            return retalbum;
        }



        public AnalyzeClass GetAnalyze(PassInfoClass passinfo,LearnOperEnum learnop)
        {
            AnalyzeClass retAnalyze = null;

            List<AlbumClass> AlbumUsedList = AlbumList;

            if (INI.PRELOADSTATICNO != "")
            {
                if (("," + INI.PRELOADSTATICNO + ",").IndexOf("," + passinfo.RcpNo.ToString() + ",") > -1)
                {
                    AlbumUsedList = StaticAlbumList;
                }
            }

            foreach (AlbumClass album in AlbumUsedList)
            {
                if (album.RelateRCP.No == passinfo.RcpNo)
                {
                    foreach(EnvClass env  in album.ENVList)
                    {
                        if(env.No == passinfo.EnvNo)
                        {
                            foreach(PageClass page in env.PageList)
                            {
                                if(page.No == passinfo.PageNo)
                                {
                                    AnalyzeClass analyzeroot = page.AnalyzeRootArray[(int)passinfo.PageOpType];

                                    retAnalyze = analyzeroot.GetAnalyze(passinfo, learnop);

                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }

            return retAnalyze;
        }
        public void GetAnalyzeMaxNo(PassInfoClass passinfo)
        {      
            foreach (AlbumClass album in AlbumList)
            {
                if (album.RelateRCP.No == passinfo.RcpNo)
                {
                    foreach (EnvClass env in album.ENVList)
                    {
                        if (env.No == passinfo.EnvNo)
                        {
                            foreach (PageClass page in env.PageList)
                            {
                                if (page.No == passinfo.PageNo)
                                {
                                    AnalyzeClass analyzeroot = page.AnalyzeRootArray[(int)passinfo.PageOpType];

                                    AnalyzeClass.LearnMaxNo = analyzeroot.GetAnalyzeMaxNo();

                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// 儲存學習後的Analyze
        /// </summary>
        /// <param name="passinfo"></param>
        public void SaveAnalyze(PassInfoClass passinfo)
        {
            foreach(AlbumClass album in AlbumList)
            {
                if(album.RelateRCP.No == passinfo.RcpNo)
                {
                    album.SaveAnalyze(passinfo);
                }
            }
        }




    }


}
