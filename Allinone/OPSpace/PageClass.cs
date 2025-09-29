using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using MoveGraphLibrary;

using JetEazy;
using ServiceMessageClass;
using Allinone.BasicSpace;
using Allinone.FormSpace;
using JetEazy.BasicSpace;

namespace Allinone.OPSpace
{
    [Serializable]
    public class PageClass
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
        CameraActionMode CAMACT
        {
            get
            {
                return Universal.CAMACT;
            }
        }

        /// <summary>
        /// 作為所有的對應位數值
        /// </summary>
        public static string ORGPAGENOSTRING = "000";

        #region Basic Data
        public int No = 0;
        public float Exposure = 200;
        public float CamGain = 1.1f;

        public int CamIndex = 0;

        public string AliasName = "";

        public int RelateToRcpNo = -1;                  //非 Static 關聯至 Static 選項的編號
        public string RelateToVersionString = "";       //給 80000 用的資訊

        public string ExposureString = "";
        public string sPagePostion = "";
        public List<string> PagePostionList = new List<string>();
        public string sPagePostionPara = "";
        public string Mark1Para = "";

        public AnalyzeClass AnalyzeRoot
        {
            get
            {
                return AnalyzeRootArray[PageOPTypeIndex];
            }
        }
        public AnalyzeClass[] AnalyzeRootArray;

        public AnalyzeClass AnalyzeSeed = null;

        public AnalyzeClass AnalyzeABSMain = null;

        //public int EnvNo = -1;
        //public int RcpNo = -1;

        public int PageRunNo = -1;
        public string PageRunPos = "0,0,0";
        public PointF PageRunLocation
        {
            get
            {
                PointF pointF = new PointF();
                if (!string.IsNullOrEmpty(PageRunPos))
                {
                    string[] _pos = PageRunPos.Split(',').ToArray();
                    pointF.X = float.Parse(_pos[0]);
                    pointF.Y = float.Parse(_pos[1]);
                }
                return pointF;
            }
        }

        public string ReportRowCol = "";//自动编号mapping 行列标志
        public int ReportIndex = 0;

        #endregion

        #region Online Data
        public string IndexSaveStr
        {
            get
            {
                return No.ToString("000");
            }
        }
        public string RunIndexSaveStr
        {
            get
            {
                return PageRunNo.ToString("000");
            }
        }
        public int PAGEOPTYPECOUNT
        {
            get
            {
                return Universal.PAGEOPTYPECOUNT;
            }
        }

        //string SAVEPATH = "";
        public PassInfoClass PassInfo = new PassInfoClass();

        Bitmap[] bmpORG;    //設定參數時保留的圖
        public Bitmap[] bmpRUN;    //測試時的圖

        public int PageOPTypeIndex = 0;
        WorkStatusCollectionClass RunStatusCollection(PageOPTypeEnum pageoptype)
        {
            return AnalyzeRootArray[(int)pageoptype].RunStatusCollection;
        }
        WorkStatusCollectionClass TrainStatusCollection(PageOPTypeEnum pageoptype)
        {
            return AnalyzeRootArray[(int)pageoptype].TrainStatusCollection;
        }

        /// <summary>
        /// DL页面测试是否完成状态
        /// </summary>
        public bool CalComplete = false;

        #endregion

        public Bitmap GetbmpORG()
        {
            return GetbmpORG((PageOPTypeEnum)PageOPTypeIndex);
        }

        public Bitmap GetbmpORG(PageOPTypeEnum pageoptype)
        {
            return bmpORG[(int)pageoptype];
        }
        public Bitmap GetbmpRUN()
        {
            return GetbmpRUN((PageOPTypeEnum)PageOPTypeIndex);
        }
        public Bitmap GetbmpRUN(PageOPTypeEnum pageoptype)
        {
            return bmpRUN[(int)pageoptype];
        }
        public void SetbmpRUN(PageOPTypeEnum pageoptype, Bitmap bmp)
        {
            bmpRUN[(int)pageoptype]?.Dispose();
            //bmpRUN[(int)pageoptype] = new Bitmap(bmp);
            switch (OPTION)
            {
                case OptionEnum.MAIN_SDM5:
                case OptionEnum.MAIN_X6:
                    switch (CAMACT)
                    {
                        case CameraActionMode.CAM_MOTOR_LINESCAN:
                            bmpRUN[(int)pageoptype] = (Bitmap)bmp.Clone();
                            break;
                        default:
                            bmpRUN[(int)pageoptype] = new Bitmap(bmp);
                            break;
                    }
                    break;
                default:
                    bmpRUN[(int)pageoptype] = new Bitmap(bmp);
                    break;
            }

            //bmpRUN[(int)pageoptype] = new Bitmap(bmp);
            //bmpRUN[(int)pageoptype] = (Bitmap)bmp.Clone();
        }
        public void SetbmpORG(PageOPTypeEnum pageoptype, Bitmap bmp)
        {
            // bmpORG[(int)pageoptype].Dispose();
            bmpORG[(int)pageoptype] = new Bitmap(bmp);

            // bmpRUN[(int)pageoptype].Dispose();
            bmpRUN[(int)pageoptype] = new Bitmap(bmp);
        }
        public PageClass()
        {

        }
        public PageClass(int index)
        {
            #region Very First Use

            No = index;

            Exposure = 10;
            AliasName = "PAGE-" + No.ToString("00");
            CamGain = 1;
            CamIndex = No;
            RelateToRcpNo = -1;

            AnalyzeRootArray = new AnalyzeClass[PAGEOPTYPECOUNT];
            AnalyzeRootArray[0] = new AnalyzeClass(No, (PageOPTypeEnum)0);

            #endregion
        }
        public PageClass(int index, Bitmap bmppage, PassInfoClass passinfo)
        {
            #region Very First Use

            int i = 0;

            No = index;

            PassInfo = new PassInfoClass(passinfo, OPLevelEnum.PAGE);
            PassInfo.PageNo = No;

            Exposure = 10;
            AliasName = "PAGE-" + No.ToString("00");
            CamGain = 1;
            CamIndex = 0;
            RelateToRcpNo = -1;

            AnalyzeRootArray = new AnalyzeClass[PAGEOPTYPECOUNT];
            bmpORG = new Bitmap[PAGEOPTYPECOUNT];
            bmpRUN = new Bitmap[PAGEOPTYPECOUNT];

            i = 0;
            while (i < PAGEOPTYPECOUNT)
            {
                AnalyzeRootArray[i] = new AnalyzeClass(No, (PageOPTypeEnum)i, PassInfo);
                bmpORG[i] = new Bitmap(bmppage);
                bmpRUN[i] = new Bitmap(bmppage);

                i++;
            }

            #endregion
        }
        public PageClass(string pagestr, List<AnalyzeClass> analyzelist, PassInfoClass passinfo)
        {
            int i = 0;

            FromString(pagestr);

            PassInfo = new PassInfoClass(passinfo, OPLevelEnum.PAGE);
            PassInfo.PageNo = No;

            InitialRelateData();

            List<AnalyzeClass> ThisPageAnalyzeList = new List<AnalyzeClass>();
            //取得關聯這頁所有的ANAYZE
            foreach (AnalyzeClass analyze in analyzelist)
            {
                if (analyze.PageNo == No)
                {
                    analyze.IsUsed = false;
                    ThisPageAnalyzeList.Add(analyze);
                }
            }

            List<AnalyzeClass> ThisPageRootList = new List<AnalyzeClass>();

            //找到各PageOPType的 Root ANALYZE
            foreach (AnalyzeClass analyze in ThisPageAnalyzeList)
            {
                //if(analyze.ParentNo == 0 &&　analyze.LearnNo == 0 && (int)analyze.PageOPtype < PAGEOPTYPECOUNT) //Parent Index 為 0 時為 Root
                if (analyze.FromNodeString == "")
                {
                    analyze.IsUsed = true;
                    AnalyzeRootArray[(int)analyze.PageOPtype] = analyze;
                    //break;
                }
            }

            //若有不足OPType的Analyze就自動增加一個AnalyzeRoot
            while (i < PAGEOPTYPECOUNT)
            {
                if (AnalyzeRootArray[i] == null)
                {
                    AnalyzeRootArray[i] = new AnalyzeClass(No, (PageOPTypeEnum)i);
                }
                i++;
            }

            //取得分支及學習的ANALYZE

            i = 0;
            while (i < PAGEOPTYPECOUNT)
            {
                bool noanalyzeused = false;
                int noanalyzecount = 0;
                int haveanalyzecount = 0;

                while (!noanalyzeused)
                {
                    noanalyzeused = true;

                    haveanalyzecount = 0;

                    foreach (AnalyzeClass insideanalyze in ThisPageAnalyzeList)
                    {
                        //if(insideanalyze.No == 69 || insideanalyze.No == 70)
                        //{
                        //    haveanalyzecount = haveanalyzecount;
                        //}

                        if (!insideanalyze.IsUsed && insideanalyze.PageOPtype == (PageOPTypeEnum)i)
                        {
                            if (AnalyzeRootArray[i].CheckAnalyzeEX(insideanalyze))
                            {
                                noanalyzeused = false;
                                haveanalyzecount++;
                            }
                        }
                    }

                    noanalyzecount++;

                    if (noanalyzecount > 500 || haveanalyzecount == 0)
                        break;
                }
                i++;
            }
        }
        public PageClass(string pagestr, string pageanalyzestr, string pagepassinfostr)
        {
            PassInfoClass passinfo = new PassInfoClass(pagepassinfostr);

            string[] analyzes = pageanalyzestr.Replace(Environment.NewLine, Universal.NewlineChar.ToString()).Split(Universal.NewlineChar);

            List<AnalyzeClass> analyzelist = new List<AnalyzeClass>();

            //取得所有正常的ANALYZE
            if (pageanalyzestr.Length > 0)
            {
                foreach (string astr in analyzes)
                {
                    AnalyzeClass analyze = new AnalyzeClass(astr, passinfo);
                    analyzelist.Add(analyze);
                }
            }

            int i = 0;

            FromString(pagestr);

            PassInfo = new PassInfoClass(passinfo, OPLevelEnum.PAGE);
            PassInfo.PageNo = No;

            InitialRelateData(false);

            //若有不足OPType的Analyze就自動增加一個AnalyzeRoot
            while (i < PAGEOPTYPECOUNT)
            {
                if (AnalyzeRootArray[i] == null)
                {
                    AnalyzeRootArray[i] = new AnalyzeClass(No, (PageOPTypeEnum)i);
                }
                i++;
            }

            List<AnalyzeClass> ThisPageAnalyzeList = new List<AnalyzeClass>();
            //取得關聯這頁所有的ANAYZE
            foreach (AnalyzeClass analyze in analyzelist)
            {
                if (analyze.PageNo == No)
                {
                    analyze.IsUsed = false;
                    ThisPageAnalyzeList.Add(analyze);
                }
            }

            List<AnalyzeClass> ThisPageRootList = new List<AnalyzeClass>();

            //找到各PageOPType的 Root ANALYZE
            foreach (AnalyzeClass analyze in ThisPageAnalyzeList)
            {
                //if(analyze.ParentNo == 0 &&　analyze.LearnNo == 0 && (int)analyze.PageOPtype < PAGEOPTYPECOUNT) //Parent Index 為 0 時為 Root
                if (analyze.FromNodeString == "")
                {
                    analyze.IsUsed = true;
                    AnalyzeRootArray[(int)analyze.PageOPtype] = analyze;
                    //break;
                }
            }

            //////若有不足OPType的Analyze就自動增加一個AnalyzeRoot
            ////while (i < PAGEOPTYPECOUNT)
            ////{
            ////    if (AnalyzeRootArray[i] == null)
            ////    {
            ////        AnalyzeRootArray[i] = new AnalyzeClass(No, (PageOPTypeEnum)i);
            ////    }
            ////    i++;
            ////}

            //取得分支及學習的ANALYZE

            i = 0;
            while (i < PAGEOPTYPECOUNT)
            {
                bool noanalyzeused = false;
                int noanalyzecount = 0;
                int haveanalyzecount = 0;

                while (!noanalyzeused)
                {
                    noanalyzeused = true;

                    haveanalyzecount = 0;

                    foreach (AnalyzeClass insideanalyze in ThisPageAnalyzeList)
                    {
                        //if(insideanalyze.No == 69 || insideanalyze.No == 70)
                        //{
                        //    haveanalyzecount = haveanalyzecount;
                        //}

                        if (!insideanalyze.IsUsed && insideanalyze.PageOPtype == (PageOPTypeEnum)i)
                        {
                            if (AnalyzeRootArray[i].CheckAnalyzeEX(insideanalyze))
                            {
                                noanalyzeused = false;
                                haveanalyzecount++;
                            }
                        }
                    }

                    noanalyzecount++;

                    if (noanalyzecount > 500 || haveanalyzecount == 0)
                        break;
                }
                i++;
            }
        }

        public PageClass Clone(bool eCloneDeepPicture = true)
        {
            int i = 0;

            //using (var ms = new MemoryStream())
            //{
            //    var formatter = new BinaryFormatter();
            //    formatter.Serialize(ms, this);
            //    ms.Position = 0;

            //    return (PageClass)formatter.Deserialize(ms);
            //}

            PageClass newpage = new PageClass();

            newpage.FromString(this.ToString());

            newpage.AnalyzeRootArray = new AnalyzeClass[PAGEOPTYPECOUNT];

            //if (eCloneDeepPicture)
            {
                i = 0;
                while (i < PAGEOPTYPECOUNT)
                {
                    newpage.AnalyzeRootArray[i] = this.AnalyzeRootArray[i].Clone(new Point(0, 0), 0d, false, true, eCloneDeepPicture, true);

                    i++;
                }
            }

            newpage.PassInfo = new PassInfoClass(this.PassInfo, OPLevelEnum.COPY);

            newpage.bmpORG = new Bitmap[PAGEOPTYPECOUNT];
            newpage.bmpRUN = new Bitmap[PAGEOPTYPECOUNT];


            for (int j = 0; j < bmpORG.Length; j++)
            {
                try
                {
                    newpage.bmpORG[j] = new Bitmap(this.bmpORG[j]);
                    newpage.bmpRUN[j] = new Bitmap(this.bmpRUN[j]);
                }
                catch
                {
                    //       newpage.bmpORG[j] = new Bitmap(1,1);
                    newpage.bmpRUN[j] = new Bitmap(1, 1);
                }
            }


            return newpage;
        }
        void FromString(string str)
        {
            string[] strs = str.Split(Universal.SeperateCharA);

            No = int.Parse(strs[0]);
            Exposure = float.Parse(strs[1]);
            AliasName = strs[2];
            RelateToRcpNo = int.Parse(strs[3]);

            if (strs.Length > 5)
            {
                CamIndex = int.Parse(strs[4]);
                RelateToVersionString = strs[5];
            }
            if (strs.Length > 6)
            {
                ExposureString = strs[6];
            }
            if (strs.Length > 7)
            {
                sPagePostion = strs[7];
                PagePostionList = sPagePostion.Split(';').ToList();
            }
            if (strs.Length > 8)
            {
                sPagePostionPara = strs[8];
            }
            if (strs.Length > 9)
            {
                Mark1Para = strs[9];
            }
            if (strs.Length > 10)
            {
                //CamGain = float.Parse(strs[10]);
                float.TryParse(strs[10], out CamGain);
            }
        }
        public override string ToString()
        {
            string retstr = "";

            retstr = No.ToString() + Universal.SeperateCharA;
            retstr += Exposure.ToString() + Universal.SeperateCharA;
            retstr += AliasName.ToUpper() + Universal.SeperateCharA;
            retstr += RelateToRcpNo.ToString() + Universal.SeperateCharA;
            retstr += CamIndex.ToString() + Universal.SeperateCharA;
            retstr += RelateToVersionString.ToUpper() + Universal.SeperateCharA;
            retstr += ExposureString + Universal.SeperateCharA;
            retstr += sPagePostion + Universal.SeperateCharA;
            retstr += sPagePostionPara + Universal.SeperateCharA;
            retstr += Mark1Para + Universal.SeperateCharA;
            retstr += CamGain.ToString() + Universal.SeperateCharA;
            retstr += "";

            PagePostionList = sPagePostion.Split(';').ToList();

            return retstr;
        }
        public string ToPageIndexString()
        {
            return "PAGE" + No.ToString("00");
        }

        public string ToPageSelectString()
        {
            string Str = "";

            Str = ToPageIndexString() + "/" + AliasName;

            return Str;

        }
        public string ToPageSelectString_RelateToVersion()
        {
            string Str = "";

            Str = ToPageIndexString() + "/" + RelateToVersionString;

            return Str;

        }

        /// <summary>
        /// 將根 Analyze 的所有資料弄出來
        /// </summary>
        /// <returns></returns>
        public string ToAnalyzeString()
        {
            string retstr = "";

            //foreach (PageClass page in PageList)
            {
                int i = 0;

                while (i < PAGEOPTYPECOUNT)
                {
                    retstr += AnalyzeRootArray[i].ToString();
                    i++;
                }
            }

            if (retstr.Length > 0)
                retstr = retstr.Substring(0, retstr.Length - 2);

            return retstr;
        }


        public void Suicide()
        {
            int i = 0;

            foreach (AnalyzeClass analyzeroot in AnalyzeRootArray)
            {
                analyzeroot.Suicide();
            }
            //try
            //{
            i = 0;
            while (i < bmpORG.Length)
            {
                bmpORG[i] = new Bitmap(1, 1);
                bmpRUN[i] = new Bitmap(1, 1);
                i++;
            }
            //}
            //catch
            //{
            //    bmpORG[i] = new Bitmap(1, 1);
            //    bmpRUN[i] = new Bitmap(1, 1);
            //}
        }
        public void SaveORGBMP()
        {
            for (int i = 0; i < bmpORG.Length; i++)
            {
                bmpORG[i].Save(PassInfo.OperatePath + "\\" +
                    ((PageOPTypeEnum)i).ToString() + "-" +
                    IndexSaveStr + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            }
        }

        #region Normal Operation
        /// <summary>
        /// 初始化圖形
        /// </summary>
        void InitialRelateData(bool isLoadLocal = true)        //Need To Modify When 
        {
            int i = 0;

            AnalyzeRootArray = new AnalyzeClass[PAGEOPTYPECOUNT];
            bmpORG = new Bitmap[PAGEOPTYPECOUNT];
            bmpRUN = new Bitmap[PAGEOPTYPECOUNT];

            i = 0;
            while (i < bmpORG.Length)
            {
                bmpORG[i] = new Bitmap(1, 1);

                if (isLoadLocal)
                    GetBMP(PassInfo.OperatePath + "\\" +
                        ((PageOPTypeEnum)i).ToString() + "-" +
                        IndexSaveStr + Universal.GlobalImageTypeString, ref bmpORG[i]);

                //Reduce By Victor 2018/02/11
                //bmpRUN[i] = new Bitmap(bmpORG[i]);
                bmpRUN[i] = new Bitmap(1, 1);


                i++;
            }

            //SaveORGBMP();

        }

        #endregion

        #region Tools Operation
        void GetBMP(string BMPFileStr, ref Bitmap BMP)
        {
            Bitmap bmpTMP = new Bitmap(BMPFileStr);

            BMP.Dispose();
            BMP = new Bitmap(bmpTMP);

            bmpTMP.Dispose();
        }
        void SaveData(string DataStr, string FileName)
        {
            File.WriteAllText(FileName, DataStr, Encoding.Default);
        }

        /// <summary>
        /// 此為Static 新增及取代專用，必需連 Analyze 裏的資料一併改
        /// </summary>
        /// <param name="passinfo"></param>
        public void GetPassInfoIncludeAnalyze(PassInfoClass passinfo)
        {
            PassInfo = new PassInfoClass(passinfo, OPLevelEnum.COPY);

            PassInfo.PageNo = No;

            foreach (AnalyzeClass analyze in AnalyzeRootArray)
            {
                analyze.SetPageNo(No);
            }
        }


        #endregion

        #region Application Operation

        ServiceClientClass xClient = new ServiceClientClass();
        public bool A00_ServiceTrain(bool isShowMessage = true)
        {
            bool isgood = true;
            string str = "";

            str = "Start " + ToPageIndexString() + "#" + PageOPTypeEnum.P00.ToString() + " Train process.";
            if (isShowMessage)
            {
                ShowTrainMessage(str);
                ShowTrainMessage("");
            }
            AnalyzeRootArray[(int)PageOPTypeEnum.P00].ResetTrainStatus();

            SvPageInfo xPageInfo = new SvPageInfo();
            //PageClass xPageClass = this;
            xPageInfo.m_Org = new Bitmap(this.GetbmpORG());
            xPageInfo.m_PassInfoStr = this.PassInfo.ToString();
            xPageInfo.m_PageStr = this.ToString();
            xPageInfo.m_AnalyzeStr = this.ToAnalyzeString();

            string trainstr = string.Empty;
            xClient.PageTrain(xPageInfo, ref trainstr, "127.0.0.1", 6000 + this.No);
            if (!string.IsNullOrEmpty(trainstr))
            {
                AnalyzeRootArray[(int)PageOPTypeEnum.P00].TrainStatusCollection.FromString(trainstr);
            }

            if (isShowMessage)
                ShowTrainMessage(TrainStatusCollection(PageOPTypeEnum.P00));

            return isgood;
        }
        /// <summary>
        /// 執行預備的多執行緒工作
        /// </summary>
        public bool A00_Train(bool isShowMessage = true)
        {
            bool isgood = true;
            string str = "";
            //System.Threading.Thread.Sleep(1000);

            switch (VERSION)
            {
                case VersionEnum.ALLINONE:

                    switch(OPTION)
                    {
                        //case OptionEnum.MAIN_SERVICE:

                        //    str = "Start " + ToPageIndexString() + "#" + PageOPTypeEnum.P00.ToString() + " Train process.";
                        //    if (isShowMessage)
                        //    {
                        //        ShowTrainMessage(str);
                        //        ShowTrainMessage("");
                        //    }
                        //    AnalyzeRootArray[(int)PageOPTypeEnum.P00].ResetTrainStatus();

                        //    SvPageInfo xPageInfo = new SvPageInfo();
                        //    //PageClass xPageClass = this;
                        //    xPageInfo.m_Org = new Bitmap(this.GetbmpORG());
                        //    xPageInfo.m_PassInfoStr = this.PassInfo.ToString();
                        //    xPageInfo.m_PageStr = this.ToString();
                        //    xPageInfo.m_AnalyzeStr = this.ToAnalyzeString();

                        //    string trainstr = string.Empty;
                        //    xClient.PageTrain(xPageInfo, ref trainstr, "127.0.0.1", 6000 + this.No);
                        //    if (!string.IsNullOrEmpty(trainstr))
                        //    {
                        //        AnalyzeRootArray[(int)PageOPTypeEnum.P00].TrainStatusCollection.FromString(trainstr);
                        //    }

                        //    if (isShowMessage)
                        //        ShowTrainMessage(TrainStatusCollection(PageOPTypeEnum.P00));


                        //    break;
                        default:

                            str = "Start " + ToPageIndexString() + "#" + PageOPTypeEnum.P00.ToString() + " Train process.";
                            if (isShowMessage)
                            {
                                ShowTrainMessage(str);
                                ShowTrainMessage("");
                            }

                            AnalyzeRootArray[(int)PageOPTypeEnum.P00].ResetTrainStatus();
                            isgood = AnalyzeRootArray[(int)PageOPTypeEnum.P00].A00_Train(bmpORG[(int)PageOPTypeEnum.P00], new PointF(0, 0), false, false, !isShowMessage);
                            if (isShowMessage)
                                ShowTrainMessage(TrainStatusCollection(PageOPTypeEnum.P00));


                            break;
                    }

                    break;
                case VersionEnum.AUDIX:
                    str = "Start " + ToPageIndexString() + "#" + PageOPTypeEnum.P00.ToString() + " Train process.";
                    if (isShowMessage)
                    {
                        ShowTrainMessage(str);
                        ShowTrainMessage("");
                    }

                    AnalyzeRootArray[(int)PageOPTypeEnum.P00].ResetTrainStatus();
                    isgood = AnalyzeRootArray[(int)PageOPTypeEnum.P00].A00_Train(bmpORG[(int)PageOPTypeEnum.P00], new PointF(0, 0), false, false, !isShowMessage);
                    if (isShowMessage)
                        ShowTrainMessage(TrainStatusCollection(PageOPTypeEnum.P00));

                    break;
                default:

                    break;
            }

            return isgood;
        }
        public bool A00_Train_BMP(Bitmap ebmpinput, bool isShowMessage = true)
        {
            bool isgood = true;
            string str = "";
            //System.Threading.Thread.Sleep(1000);

            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                case VersionEnum.AUDIX:
                    str = "Start " + ToPageIndexString() + "#" + PageOPTypeEnum.P00.ToString() + " Train process.";
                    if (isShowMessage)
                    {
                        ShowTrainMessage(str);
                        ShowTrainMessage("");
                    }

                    AnalyzeRootArray[(int)PageOPTypeEnum.P00].ResetTrainStatus();
                    isgood = AnalyzeRootArray[(int)PageOPTypeEnum.P00].A00_Train(ebmpinput, new PointF(0, 0), false, false, !isShowMessage);
                    if (isShowMessage)
                        ShowTrainMessage(TrainStatusCollection(PageOPTypeEnum.P00));

                    break;
                default:

                    break;
            }

            return isgood;
        }

        /// <summary>
        /// 執行預備的多執行緒工作
        /// </summary>
        public bool A00_Train_Show()
        {
            bool isgood = true;

            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                case VersionEnum.AUDIX:

                    AnalyzeRootArray[(int)PageOPTypeEnum.P00].ResetTrainStatus();
                    isgood = AnalyzeRootArray[(int)PageOPTypeEnum.P00].A00_Train(bmpORG[(int)PageOPTypeEnum.P00], new PointF(0, 0), false, false, true);
                    if (!isgood)
                        ShowTrainMessage(TrainStatusCollection(PageOPTypeEnum.P00));

                    break;
                default:

                    break;
            }



            return isgood;
        }

        //public string RunStatusStr
        //{
        //    get { return m_RunStatusStr; }
        //}
        //string m_RunStatusStr = string.Empty;

        public void A08_RunServiceProcess(PageOPTypeEnum pageoptype)
        {
            if (AnalyzeRootArray[(int)pageoptype].BranchList.Count > 0)
            {
                SvPageInfo xPageInfo = new SvPageInfo();
                xPageInfo.m_Org = new Bitmap(this.GetbmpRUN());
                //xPageInfo.m_PassInfoStr = xPageClass.PassInfo.ToString();
                //xPageInfo.m_PageStr = xPageClass.ToString();
                //xPageInfo.m_AnalyzeStr = xPageClass.ToAnalyzeString();

                AnalyzeRootArray[(int)pageoptype].ResetRunStatus();
                string runstr = string.Empty;
                xClient.PageRun(xPageInfo, ref runstr, "127.0.0.1", 6000 + this.No);
                //xClient.PageRun(bmpRUN[(int)pageoptype], ref runstr, "127.0.0.1", 6000 + this.No);
                if (!string.IsNullOrEmpty(runstr))
                {
                    AnalyzeRootArray[(int)pageoptype].RunStatusCollection.FromString(runstr);
                }
            }
        }
        /// <summary>
        /// 執行預備的多執行緒工作
        /// </summary>
        public bool A08_RunProcess(PageOPTypeEnum pageoptype)
        {
            bool isgood = true;

            //计算Mark点
            Point ptfOffset = new Point(0, 0);
            switch (Universal.OPTION)
            {
                case OptionEnum.MAIN_X6:
                    switch (Universal.CAMACT)
                    {
                        case CameraActionMode.CAM_MOTOR_LINESCAN:


                            #region 计算Mark的偏移

                            MarkParaPropertyGridClass markParaPropertyGridClass = new MarkParaPropertyGridClass();
                            markParaPropertyGridClass.FromingStr(Mark1Para);
                            if (markParaPropertyGridClass.chkIsOpen && false)
                            {
                                PointF ptfrun = calMarkBlob(bmpRUN[(int)pageoptype],
                               markParaPropertyGridClass.RectF, markParaPropertyGridClass.chkThresholdValue, out RectangleF maxrectresult,
                               markParaPropertyGridClass.chkblobmode == BlobMode.White);

                                //ptfOffset.X = (int)(markParaPropertyGridClass.PtfCenter.X - ptfrun.X);
                                //ptfOffset.Y = (int)(markParaPropertyGridClass.PtfCenter.Y - ptfrun.Y);

                                ptfOffset.X = (int)(ptfrun.X - markParaPropertyGridClass.PtfCenter.X);
                                ptfOffset.Y = (int)(ptfrun.Y - markParaPropertyGridClass.PtfCenter.Y);

                            }
                            else
                                ptfOffset = new Point(0, 0);



                            #endregion


                            SetOffset(ptfOffset, true);

                            break;
                    }
                    break;
            }


            if (Universal.IsUseSeedFuntion && false)
            {
                #region create 20211216 by gaara

                //AnalyzeSeed = null;
                //if (AnalyzeRootArray[(int)pageoptype].BranchList.Count > 0)
                //{
                //    //循环找种子
                //    foreach (AnalyzeClass analyze1 in AnalyzeRootArray[(int)pageoptype].BranchList)
                //    {
                //        if (analyze1.IsSeed && AnalyzeSeed == null)
                //        {
                //            AnalyzeSeed = analyze1;
                //            break;
                //        }
                //    }

                //    //填充种子的测试数据
                //    foreach (AnalyzeClass analyze1 in AnalyzeRootArray[(int)pageoptype].BranchList)
                //    {
                //        if (!analyze1.IsSeed && AnalyzeSeed != null)
                //        {
                //            analyze1.BranchList.Add(AnalyzeSeed);

                //            //foreach (AnalyzeClass analyze2 in AnalyzeSeed.LearnList)
                //            //{
                //            //    analyze1.LearnList.Add(analyze2);
                //            //}
                //        }
                //    }

                //    //开始测试填充好的数据

                //    AnalyzeRootArray[(int)pageoptype].ResetRunStatus();
                //    //lock (bmpRUN[(int)pageoptype])
                //    isgood = AnalyzeRootArray[(int)pageoptype].A01_Run(bmpRUN[(int)pageoptype]);


                //    //清除种子的测试数据  及  获取结果信息
                //    foreach (AnalyzeClass analyze1 in AnalyzeRootArray[(int)pageoptype].BranchList)
                //    {

                //        foreach (AnalyzeClass analyze2 in analyze1.BranchList)
                //        {
                //            if (!analyze2.IsVeryGood)
                //            {
                //                analyze1.IsVeryGood = false;
                //                break;
                //            }
                //        }
                //        //if (!analyze1.IsVeryGood)
                //        //{
                //        //    foreach (AnalyzeClass analyze2 in analyze1.LearnList)
                //        //    {
                //        //        if (!analyze2.IsVeryGood)
                //        //        {
                //        //            analyze1.IsVeryGood = false;
                //        //            break;
                //        //        }
                //        //    }
                //        //}

                //        if (!analyze1.IsSeed && AnalyzeSeed != null)
                //        {
                //            analyze1.BranchList.Clear();
                //            //analyze1.LearnList.Clear();
                //        }
                //    }

                //}

                #endregion

#if GAARA_TEST
                //Use Runstatus Collection
                if (AnalyzeRootArray[(int)pageoptype].BranchList.Count > 0)
                {

                    if (AnalyzeSeed == null)
                    {
                        AnalyzeSeed = AnalyzeRootArray[(int)pageoptype].BranchList[0];
                    }


                    //种子的子框加入其他的框
                    foreach (AnalyzeClass analyze in AnalyzeSeed.BranchList)
                    {

                        //AnalyzeClass analyze1 = analyze.Clone();
                        //analyze1.A00_Train(bmpORG[(int)PageOPTypeEnum.P00], new PointF(0, 0), false, false);
                        ////analyze1.A00_Train()
                        //AnalyzeRootArray[(int)pageoptype].BranchList[1].BranchList.Add(analyze1);
                        int iseedindex1 = 0;
                        foreach (AnalyzeClass analyze1 in AnalyzeRootArray[(int)pageoptype].BranchList)
                        {
                            if (iseedindex1 == 0)
                            {
                                iseedindex1++;
                                continue;
                            }

                            analyze1.BranchList.Add(analyze);

                            iseedindex1++;
                        }


                        //AnalyzeRootArray[(int)pageoptype].BranchList[1].BranchList.Add(analyze);
                    }
                    //AnalyzeRootArray[(int)pageoptype].BranchList[1].BranchList.Add()


                    AnalyzeRootArray[(int)pageoptype].ResetRunStatus();
                    //lock (bmpRUN[(int)pageoptype])
                    isgood = AnalyzeRootArray[(int)pageoptype].A01_Run(bmpRUN[(int)pageoptype]);



                    int iseedindex = 0;
                    foreach (AnalyzeClass analyze1 in AnalyzeRootArray[(int)pageoptype].BranchList)
                    {
                        if (iseedindex == 0)
                        {
                            iseedindex++;
                            continue;
                        }

                        analyze1.BackupInsideList(AnalyzeSeed);
                        iseedindex++;
                    }

                    iseedindex = 0;
                    foreach (AnalyzeClass analyze1 in AnalyzeRootArray[(int)pageoptype].BranchList)
                    {
                        if (iseedindex == 0)
                        {
                            analyze1.RestoreInsideList(true);
                            iseedindex++;
                            continue;
                        }

                        //analyze1.BranchList.Clear();

                        analyze1.RestoreInsideList(false);

                        iseedindex++;
                    }

                    iseedindex = 0;
                    foreach (AnalyzeClass analyze1 in AnalyzeRootArray[(int)pageoptype].BranchList)
                    {
                        if (iseedindex == 0)
                        {
                            iseedindex++;
                            continue;
                        }

                        analyze1.BranchList.Clear();

                        iseedindex++;
                    }

                    ////种子的子框加入其他的框 使用完清除
                    //AnalyzeRootArray[(int)pageoptype].BranchList[1].BranchList.Clear();

                }
                //AnalyzeRootArray[(int)pageoptype].RunStatusCollection.SaveProcessAndError(Universal.TESTPATH + "\\F1", ToPageIndexString());

#endif

            }
            else
            {

                //Use Runstatus Collection
                if (AnalyzeRootArray[(int)pageoptype].BranchList.Count > 0)
                {
                    AnalyzeRootArray[(int)pageoptype].ResetRunStatus();
                    //lock (bmpRUN[(int)pageoptype])
                    isgood = AnalyzeRootArray[(int)pageoptype].A01_Run(bmpRUN[(int)pageoptype]);
                }
                //AnalyzeRootArray[(int)pageoptype].RunStatusCollection.SaveProcessAndError(Universal.TESTPATH + "\\F1", ToPageIndexString());
            }

            //

            AnalyzeABSMain = A100_01CheckABSMain(AnalyzeRootArray[(int)pageoptype]);
            if (AnalyzeABSMain != null)
            {
                A100_02RunCheckABSMain(AnalyzeRootArray[(int)pageoptype]);
            }

            return isgood;
        }

        public bool A101_ProcessAnalyzeOffset(PageOPTypeEnum pageoptype)
        {
            bool isgood = true;
            //Use Runstatus Collection
            if (AnalyzeRootArray[(int)pageoptype].BranchList.Count > 0)
            {
                AnalyzeRootArray[(int)pageoptype].ResetRunStatus();
                //lock (bmpRUN[(int)pageoptype])
                isgood = AnalyzeRootArray[(int)pageoptype].A101_01();
            }
            return isgood;
        }

        AnalyzeClass A100_01CheckABSMain(AnalyzeClass analyzeroot)
        {
            if (analyzeroot != null)
            {
                if (analyzeroot.ALIGNPara.AbsAlignMode == AbsoluteAlignEnum.MAIN)
                    return analyzeroot;

                foreach (AnalyzeClass analyze in analyzeroot.BranchList)
                {
                    if (analyze.ALIGNPara.AbsAlignMode != AbsoluteAlignEnum.MAIN)
                        continue;
                    AnalyzeClass temp = A100_01CheckABSMain(analyze);
                    if (temp != null)
                        return temp;
                }
                return null;
            }
            else
                return null;
        }
        void A100_02RunCheckABSMain(AnalyzeClass analyzeroot)
        {
            if (analyzeroot != null)
            {
                //if (analyzeroot.ALIGNPara.AbsAlignMode == AbsoluteAlignEnum.MAIN)
                //    return;
                foreach (AnalyzeClass analyze in analyzeroot.BranchList)
                {
                    if (analyze.ALIGNPara.AbsAlignMode == AbsoluteAlignEnum.RELATION)
                    {
                        //bool bOK = analyze.ALIGNPara.CheckAbsOffset(AnalyzeABSMain.ALIGNPara.OrgCenter, AnalyzeABSMain.ALIGNPara.RunCenter);

                        PointF ptfORG = new PointF(AnalyzeABSMain.ALIGNPara.OrgCenter.X + AnalyzeABSMain.myOPRectF.X,
                           AnalyzeABSMain.ALIGNPara.OrgCenter.Y + AnalyzeABSMain.myOPRectF.Y);

                        PointF ptfRUN = new PointF(AnalyzeABSMain.ALIGNPara.RunCenter.X + AnalyzeABSMain.myOPRectF.X,
                          AnalyzeABSMain.ALIGNPara.RunCenter.Y + AnalyzeABSMain.myOPRectF.Y);


                        bool bOK = analyze.ALIGNPara.CheckAbsOffset(ptfORG, ptfRUN, analyze.myOPRectF);
                        if (analyze.IsVeryGood)
                            analyze.IsVeryGood = !bOK;
                        A100_02RunCheckABSMain(analyze);
                    }
                }
            }
        }

        public void FillRunStatus(WorkStatusCollectionClass runstatuscollection)
        {
            //switch(VERSION)
            //{
            //    case VersionEnum.ALLINONE:
            //        switch(OPTION)
            //        {
            //            case OptionEnum.MAIN_SERVICE:

            //                if (!string.IsNullOrEmpty(m_RunStatusStr))
            //                {
            //                    runstatuscollection.FromString(m_RunStatusStr);
            //                }

            //                break;
            //        }
            //        break;
            //    default:

                   

            //        break;
            //}

            foreach (AnalyzeClass analyzeroot in AnalyzeRootArray)
            {
                analyzeroot.FillRunStatus(runstatuscollection);
            }
        }
        public void FillTrainStatus(WorkStatusCollectionClass trainstatuscollection)
        {
            foreach (AnalyzeClass analyzeroot in AnalyzeRootArray)
            {
                analyzeroot.FillTrainStatus(trainstatuscollection);
            }
        }
        public void ResetRunStatus()
        {
            //m_RunStatusStr = string.Empty;

            foreach (AnalyzeClass analyzeroot in AnalyzeRootArray)
            {
                analyzeroot.ResetRunStatus();
            }
        }
        public void ResetTrainStatus()
        {
            foreach (AnalyzeClass analyzeroot in AnalyzeRootArray)
            {
                analyzeroot.ResetTrainStatus();
            }
        }
        public bool IsPageFirstAlignPass(PageOPTypeEnum pageoptype)
        {
            bool ret = true;

            if (!AnalyzeRootArray[(int)pageoptype].IsAlignPass())
                ret = false;

            return ret;
        }

        public void SaveRUNBMP(string savepath, string savestr, PageOPTypeEnum pageoptype)
        {
            string[] secstr = savestr.Split(',');

            //if (savestr.IndexOf(CamIndex.ToString("000")) > -1 || savestr == "-1")//Gaara
            if (savestr.IndexOf(RunIndexSaveStr) > -1 || savestr == "-1")
            {
                //bmpRUN[(int)pageoptype].Save(savepath + "\\" +
                //    (pageoptype).ToString() + "-" +
                //    CamIndex.ToString("000") + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                bmpRUN[(int)pageoptype].Save(savepath + "\\" +
                    (pageoptype).ToString() + "-" +
                    RunIndexSaveStr + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            }
        }
        public void SaveHEIGHTBMP(string savepath, string savestr, PageOPTypeEnum pageoptype)
        {
            string[] secstr = savestr.Split(',');

            if (secstr[0].IndexOf(IndexSaveStr) > -1 || savestr == "-1")
            {
                bmpRUN[(int)pageoptype].Save(savepath + "\\THH" +
                    secstr[1] + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            }
        }
        void ShowTrainMessage(WorkStatusCollectionClass trainstatuscollection)
        {
            List<string> messagelist = new List<string>();

            string allstr = "";

            foreach (WorkStatusClass works in trainstatuscollection.WorkStatusList)
            {
                allstr += works.ProcessString + Environment.NewLine;

                string[] strs = works.ProcessString.Replace(Environment.NewLine, Universal.NewlineChar.ToString()).Split(Universal.NewlineChar);

                foreach (string str in strs)
                {
                    messagelist.Add(str);
                }
            }

            OnPrintMessage(messagelist);

            //    SaveData(allstr, Universal.TESTPATH + "\\" + ToPageIndexString() + " Process.log");


            //trainstatuscollection.SaveProcessAndError(Universal.WORKPATH);
        }
        void ShowTrainMessage(string str)
        {
            List<string> messagelist = new List<string>();

            messagelist.Add(str);

            OnPrintMessage(messagelist);
        }


        #region 自动编号 
        public void SetOffset(Point ePointOffset,bool ison)
        {
            AnalyzeRoot.A00_SetOffset(ePointOffset, ison);
        }
        public Point GetFirstOffset()
        {
            Point pointF = new Point(0, 0);
            if (AnalyzeRoot.ALIGNPara.AlignMethod != AlignMethodEnum.NONE)
            {
                pointF.X += (int)AnalyzeRoot.ALIGNPara.AlignOffset.X;
                pointF.Y += (int)AnalyzeRoot.ALIGNPara.AlignOffset.Y;
            }
            if (AnalyzeRoot.BranchList.Count > 0)
            {
                if (AnalyzeRoot.BranchList[0].ALIGNPara.AlignMethod != AlignMethodEnum.NONE)
                {
                    pointF.X += (int)AnalyzeRoot.BranchList[0].ALIGNPara.AlignOffset.X;
                    pointF.Y += (int)AnalyzeRoot.BranchList[0].ALIGNPara.AlignOffset.Y;
                }
            }
            return pointF;
        }

        public int m_Mapping_Col = 0;
        public int m_Mapping_Row = 0;

        public void PageAutoReportIndex(int pageIndex)
        {
            List<AnalyzeClass> BranchList = AnalyzeRoot.BranchList;

            int Highest = 100000;
            int HighestIndex = -1;
            int ReportIndex = 0;
            List<string> CheckList = new List<string>();

            int i = 0;

            //Clear All Index To 0 and Check the Highest

            foreach (AnalyzeClass keyassign in BranchList)
            {
                if (keyassign.CheckAnalyzeReadBarcode())
                {
                    keyassign.ReportRowCol = "";
                    keyassign.ReportIndex = -1;
                }
                else
                {

                    //keyassign.PageNo = pageIndex;
                    keyassign.ReportRowCol = "";
                    keyassign.ReportIndex = 0;
                    ReportIndex = 1;

                    //keyassign.PassInfo.PageNo = pageIndex;
                    //keyassign.PassInfo.Level = keyassign.Level;
                    //keyassign.PassInfo.AnalyzeNo = keyassign.No;
                }
            }

            i = 0;
            while (true)
            {
                i = 0;
                Highest = 100000;
                HighestIndex = -1;
                foreach (AnalyzeClass keyassign in BranchList)
                {
                    if (keyassign.ReportIndex == 0)
                    {
                        if (keyassign.myOPRectF.Y < Highest)
                        {
                            Highest = (int)keyassign.myOPRectF.Y;
                            HighestIndex = i;
                        }
                    }

                    i++;
                }

                if (HighestIndex == -1)
                    break;

                CheckList.Clear();

                //把相同位置的人找出來
                i = 0;
                foreach (AnalyzeClass keyassign in BranchList)
                {
                    if (keyassign.ReportIndex == 0)
                    {
                        if (IsInRange((int)keyassign.myOPRectF.Y, Highest, 138))
                        {
                            CheckList.Add(keyassign.myOPRectF.X.ToString("00000000") + "," + i.ToString());
                        }
                    }
                    i++;
                }

                CheckList.Sort();

                i = 1;
                foreach (string Str in CheckList)
                {
                    string[] Strs = Str.Split(',');

                    BranchList[int.Parse(Strs[1])].ReportIndex = ReportIndex;
                    BranchList[int.Parse(Strs[1])].ReportRowCol = CheckList.Count.ToString() + "-" + i.ToString();
                    BranchList[int.Parse(Strs[1])].DataReportIndex = ReportIndex;
                    ReportIndex++;
                    i++;
                }
            }

            ////从大到小排序
            //BranchList.Sort((item1, item2) => { return item1.ReportIndex > item2.ReportIndex ? -1 : 1; });

            //从小到大排序
            BranchList.Sort((item1, item2) => { return item1.ReportIndex > item2.ReportIndex ? 1 : -1; });

            int icount = 0;
            foreach (AnalyzeClass analyze in BranchList)
            {
                if (!analyze.CheckAnalyzeReadBarcode())
                {
                    m_Mapping_Col = int.Parse(analyze.ReportRowCol.Split('-')[0]);
                    icount++;
                }
            }
            if (m_Mapping_Col != 0)
                m_Mapping_Row = icount / m_Mapping_Col;
            else
                m_Mapping_Row = 0;
            //m_Mapping_Col = int.Parse(BranchList[0].ReportRowCol.Split('-')[0]);
            //m_Mapping_Row = BranchList.Count / m_Mapping_Col;

        }
        public void PageAutoReportIndexMappingA(int pageIndex)
        {
            List<AnalyzeClass> BranchList = AnalyzeRoot.BranchList;

            int Highest = 100000;
            int HighestIndex = -1;
            int ReportIndex = 0;
            List<string> CheckList = new List<string>();

            int i = 0;

            //Clear All Index To 0 and Check the Highest

            foreach (AnalyzeClass keyassign in BranchList)
            {
                keyassign.ReportRowCol = "";
                keyassign.ReportIndex = 0;
                ReportIndex = 1;
            }

            i = 0;
            while (true)
            {
                i = 0;
                Highest = 100000;
                HighestIndex = -1;
                foreach (AnalyzeClass keyassign in BranchList)
                {
                    if (keyassign.ReportIndex == 0)
                    {
                        if (keyassign.myOPRectF.Y < Highest)
                        {
                            Highest = (int)keyassign.myOPRectF.Y;
                            HighestIndex = i;
                        }
                    }

                    i++;
                }

                if (HighestIndex == -1)
                    break;

                CheckList.Clear();

                //把相同位置的人找出來
                i = 0;
                foreach (AnalyzeClass keyassign in BranchList)
                {
                    if (keyassign.ReportIndex == 0)
                    {
                        if (IsInRange((int)keyassign.myOPRectF.Y, Highest, 138))
                        {
                            CheckList.Add(keyassign.myOPRectF.X.ToString("00000000") + "," + i.ToString());
                        }
                    }
                    i++;
                }

                CheckList.Sort();

                i = 1;
                foreach (string Str in CheckList)
                {
                    string[] Strs = Str.Split(',');

                    BranchList[int.Parse(Strs[1])].ReportIndex = ReportIndex;
                    BranchList[int.Parse(Strs[1])].ReportRowCol = CheckList.Count.ToString() + "-" + i.ToString();
                    BranchList[int.Parse(Strs[1])].DataReportIndex = ReportIndex;
                    ReportIndex++;
                    i++;
                }
            }

            ////从大到小排序
            //BranchList.Sort((item1, item2) => { return item1.ReportIndex > item2.ReportIndex ? -1 : 1; });

            //从小到大排序
            BranchList.Sort((item1, item2) => { return item1.ReportIndex > item2.ReportIndex ? 1 : -1; });

            int icount = 0;
            foreach (AnalyzeClass analyze in BranchList)
            {
                //if (!analyze.CheckAnalyzeReadBarcode())
                {
                    m_Mapping_Col = int.Parse(analyze.ReportRowCol.Split('-')[0]);
                    icount++;
                }
            }
            if (m_Mapping_Col != 0)
                m_Mapping_Row = icount / m_Mapping_Col;
            else
                m_Mapping_Row = 0;
            //m_Mapping_Col = int.Parse(BranchList[0].ReportRowCol.Split('-')[0]);
            //m_Mapping_Row = BranchList.Count / m_Mapping_Col;

        }
        bool IsInRange(int FromValue, int CompValue, int DiffValue)
        {
            return (FromValue >= CompValue - DiffValue) && (FromValue <= CompValue + DiffValue);
        }

        JzFindObjectClass m_Find = new JzFindObjectClass();
        PointF calMarkBlob(Bitmap bmpinput, RectangleF cropRect, int threshold, out RectangleF maxrect, bool isfindWhite = true)
        {
            PointF ret = new PointF(cropRect.X + cropRect.Width / 2, cropRect.Y + cropRect.Height / 2);
            maxrect = new RectangleF(cropRect.X + 1, cropRect.Y + 1, cropRect.Width - 2, cropRect.Height - 2);
            Bitmap bmptemp = (Bitmap)bmpinput.Clone(cropRect, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            m_Find.AH_SetThreshold(ref bmptemp, threshold);
            m_Find.AH_FindBlob(bmptemp, isfindWhite);

            if (m_Find.FoundList.Count > 0)
            {
                int maxindex = m_Find.GetMaxRectIndex();
                ret = new PointF((float)m_Find.FoundList[maxindex].rotatedRectangleF.fCX + cropRect.X,
                                 (float)m_Find.FoundList[maxindex].rotatedRectangleF.fCY + cropRect.Y);

                maxrect = new RectangleF(m_Find.rectMaxRect.X + cropRect.X, m_Find.rectMaxRect.Y + cropRect.Y, m_Find.rectMaxRect.Width, m_Find.rectMaxRect.Height);
            }
            bmptemp.Dispose();

            return ret;
        }

        #endregion


        #endregion

        public delegate void PrintMessageHandler(List<string> processstringlist);
        public event PrintMessageHandler PrintMessageAction;
        public void OnPrintMessage(List<string> processstringlist)
        {
            if (PrintMessageAction != null)
            {
                PrintMessageAction(processstringlist);

                processstringlist.Clear();
            }
        }
    }
}
