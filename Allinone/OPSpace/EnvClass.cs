using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using JetEazy;
using JetEazy.BasicSpace;
using JetEazy.ControlSpace;

namespace Allinone.OPSpace
{
    [Serializable]
    public class EnvClass
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
        int PAGEOPTYPECOUNT
        {
            get
            {
                return Universal.PAGEOPTYPECOUNT;
            }

        }

        //General Setup
        public int No = 0;
        public string GeneralLight = "1,0,1,0,1,0";
        public string GeneralPosition = "0,0,0";

        public PassInfoClass PassInfo = new PassInfoClass();

        //string SAVEPATH = "";
        //public string NoSaveStr
        //{
        //    get
        //    {
        //        return No.ToString("000");
        //    }
        //}

        /// <summary>
        /// 作為所有的對應位數值
        /// </summary>
        public static string ORGENVNOSTRING = "000";

        //public Bitmap bmpENV = new Bitmap(1, 1);

        public List<PageClass> PageList = new List<PageClass>();
        public EnvClass()
        {

        }
        public EnvClass(int index)
        {
            #region Very First Use

            No = index;

            int i = 0;

            while (i < 10)
            {
                PageClass newpage = new PageClass(i);
                PageList.Add(newpage);

                i++;
            }
            #endregion
        }
        public EnvClass(int index, int pagecount, PassInfoClass passinfo)
        {
            #region Very First Use

            No = index;

            int i = 0;
            PassInfo = new PassInfoClass(passinfo, OPLevelEnum.ENV);
            PassInfo.EnvNo = No;
            PassInfo.OperatePath = passinfo.OperatePath + "\\" + No.ToString(ORGENVNOSTRING);

            Bitmap bmp = new Bitmap(2592, 1944);

            DrawRect(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height), new SolidBrush(Color.Black));

            switch (VERSION)
            {
                case VersionEnum.AUDIX:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_DFLY:

                            GeneralPosition = "";
                            GeneralPosition += DFlyMethodEnum.RELATEPOS.ToString() + ";";

                            i = 0;

                            while (i < 100)
                            {
                                GeneralPosition += ";";
                                i++;
                            }

                            GeneralPosition = GeneralPosition.Remove(GeneralPosition.Length - 1, 1);
                            break;
                    }
                    break;
            }

            i = 0;
            while (i < pagecount)
            {
                PageClass newpage = new PageClass(i, bmp, PassInfo);
                PageList.Add(newpage);

                i++;
            }

            bmp.Dispose();

            #endregion
        }
        public EnvClass(DataRow envrow, PassInfoClass passinfo)
        {
            string envstr = (string)envrow["EnvData"];

            FromString(envstr);

            No = (int)envrow["No"];

            PassInfo = new PassInfoClass(passinfo, OPLevelEnum.ENV);
            PassInfo.EnvNo = No;
            PassInfo.OperatePath = passinfo.OperatePath + "\\" + No.ToString(ORGENVNOSTRING);

            //Test For Serialize Deep Clone
            //if(Index == 0)
            //{
            //    //bmpENV.Dispose();
            //    GetBMP(@"D:\LOA\KB.BMP", ref bmpENV);
            //}
            //

            //可以把所有的Analyze 都刪掉然後來自己弄
            string analyzestr = (envrow["AnalyzeData"] == DBNull.Value ? "" : (string)envrow["AnalyzeData"]);
            string pagestr = (envrow["PageData"] == DBNull.Value ? "" : (string)envrow["PageData"]);

            if (pagestr.Trim() != "")
                FillPAList(pagestr, analyzestr, PassInfo);

        }
        /// <summary>
        /// 使用 AlbumWork 時採取的對應方式，需注意會影響的地方
        /// </summary>
        /// <param name="env"></param>
        public EnvClass(EnvClass env)
        {
            FromString(env.ToString());
            No = env.No;

            PassInfo.FromPassInfo(env.PassInfo, OPLevelEnum.COPY);
        }
        public EnvClass Clone()
        {
            //using(var ms = new MemoryStream())
            //{
            //    var formatter = new BinaryFormatter();
            //    formatter.Serialize(ms, this);
            //    ms.Position = 0;

            //    return (EnvClass)formatter.Deserialize(ms);
            //}

            EnvClass newenv = new EnvClass();

            newenv.FromString(this.ToString());
            newenv.No = this.No;
            newenv.PassInfo.FromPassInfo(PassInfo, OPLevelEnum.COPY);

            foreach (PageClass page in this.PageList)
            {
                PageClass newpage = page.Clone();
                //newpage.PrintMessageAction += Page_PrintMessage;

                newenv.PageList.Add(newpage);
            }

            return newenv;
        }
        void FromString(string str)
        {
            string[] strs = str.Split(Universal.SeperateCharA);

            GeneralLight = strs[0];

            if (strs.Length > 2)
            {
                GeneralPosition = strs[1];
            }
        }
        public override string ToString()
        {
            string retstr = "";

            retstr = GeneralLight + Universal.SeperateCharA;
            retstr += GeneralPosition + Universal.SeperateCharA;
            retstr += "";

            return retstr;
        }
        public string ToPageString()
        {
            string retstr = "";

            foreach (PageClass page in PageList)
            {
                retstr += page.ToString() + Environment.NewLine;
            }

            if (retstr.Length > 0)
                retstr = retstr.Substring(0, retstr.Length - 2);

            return retstr;
        }
        /// <summary>
        /// 將根 Analyze 的所有資料弄出來
        /// </summary>
        /// <returns></returns>
        public string ToAnalyzeString()
        {
            string retstr = "";

            foreach (PageClass page in PageList)
            {
                int i = 0;

                while (i < PAGEOPTYPECOUNT)
                {
                    retstr += page.AnalyzeRootArray[i].ToString();
                    i++;
                }
            }

            if (retstr.Length > 0)
                retstr = retstr.Substring(0, retstr.Length - 2);

            return retstr;
        }
        ///// <summary>
        ///// No Use Function
        ///// </summary>
        //public void SaveLearn()
        //{

        //    foreach (PageClass page in PageList)
        //    {
        //        int i = 0;

        //        while (i < PAGEOPTYPECOUNT)
        //        {
        //            page.AnalyzeRootArray[i].SaveLearn();
        //            i++;
        //        }
        //    }
        //}
        void FillPAList(string pagestr, string analyzestr, PassInfoClass passinfo)
        {
            string[] pages = pagestr.Replace(Environment.NewLine, Universal.NewlineChar.ToString()).Split(Universal.NewlineChar);
            string[] analyzes = analyzestr.Replace(Environment.NewLine, Universal.NewlineChar.ToString()).Split(Universal.NewlineChar);

            List<AnalyzeClass> analyzelist = new List<AnalyzeClass>();

            //取得所有正常的ANALYZE
            if (analyzestr.Length > 0)
            {
                foreach (string astr in analyzes)
                {
                    AnalyzeClass analyze = new AnalyzeClass(astr, passinfo);
                    analyzelist.Add(analyze);
                }
            }

            //取得所有在這個ENV裏學習的ANALYZE
            GetLearnAnalzye(analyzelist);

            foreach (string pstr in pages)
            {
                PageClass page = new PageClass(pstr, analyzelist, passinfo);
                //page.PrintMessageAction += Page_PrintMessage;
                PageList.Add(page);
            }
        }
        /// <summary>
        /// 取得在這個Env下所有的Learning Analyze
        /// </summary>
        /// <param name="analyzelist"></param>
        void GetLearnAnalzye(List<AnalyzeClass> analyzelist)
        {
            string[] learndirs = Directory.GetDirectories(PassInfo.OperatePath);

            foreach (string learndir in learndirs)
            {
                string[] filestrs = Directory.GetFiles(learndir, "*" + Universal.GlobalImageTypeString);

                foreach (string filestr in filestrs)
                {
                    string analyzestr = "";

                    ReadData(ref analyzestr, filestr.Replace(Universal.GlobalImageTypeString, ".jdb"));

                    AnalyzeClass learnanalyze = new AnalyzeClass(analyzestr, PassInfo.OperatePath);

                    GetBMP(filestr, ref learnanalyze.bmpPATTERN);

                    analyzelist.Add(learnanalyze);
                }
            }
        }

        public void SaveLearnAnalyze()
        {
            //List<string> alloriginfilelist = new List<string>();
            List<string> allsavefilelist = new List<string>();

            #region 先取得所有之前Learn過的檔名
            //string[] learndirs = Directory.GetDirectories(PassInfo.OperatePath);
            //foreach (string learndir in learndirs)
            //{
            //    string[] filestrs = Directory.GetFiles(learndir, "*.jdb");

            //    foreach (string filestr in filestrs)
            //    {
            //        alloriginfilelist.Add(filestr);
            //        alloriginfilelist.Add(filestr.Replace(".jdb", Universal.GlobalImageTypeString));
            //    }
            //}
            #endregion

            #region 儲存所有的 Learn 並取得儲存時的檔名
            foreach (PageClass page in PageList)
            {
                int i = 0;

                while (i < (int)page.PAGEOPTYPECOUNT)
                {
                    page.AnalyzeRootArray[i].SaveAllLearn(allsavefilelist);
                    i++;
                }
            }
            #endregion

            #region 把沒儲存過的檔刪掉，及內部無檔案的刪除

            //foreach(string filename in alloriginfilelist)
            //{
            //    if(allsavefilelist.IndexOf(filename) < 0)
            //    {
            //        File.Delete(filename);
            //    }
            //}

            //learndirs = Directory.GetDirectories(PassInfo.OperatePath);
            //foreach (string learndir in learndirs)
            //{
            //    string[] filestrs = Directory.GetFiles(learndir, "*.jdb");

            //    if(filestrs.Length < 1)
            //    {
            //        Directory.Delete(learndir);
            //    }
            //}

            #endregion
        }

        /// <summary>
        /// 在反饋回TrainMessageForm時要做的動作
        /// </summary>
        /// <param name="isaction"></param>
        public void ActionDefined(bool isaction)
        {
            if (isaction)
            {
                foreach (PageClass page in PageList)
                {
                    page.PrintMessageAction += Page_PrintMessageAction;
                }
            }
            else
            {
                foreach (PageClass page in PageList)
                {
                    page.PrintMessageAction -= Page_PrintMessageAction;
                }
            }
        }

        private void Page_PrintMessageAction(List<string> processstringlist)
        {
            OnPrintMessage(processstringlist);
        }

        public string ToEnvString()
        {
            return "ENV" + No.ToString("00");
        }
        public void Suicide()
        {
            foreach (PageClass page in PageList)
            {
                page.Suicide();
            }
        }

        public void SaveBMP()
        {
            if (!Directory.Exists(PassInfo.OperatePath))
                Directory.CreateDirectory(PassInfo.OperatePath);

            foreach (PageClass page in PageList)
            {
                page.SaveORGBMP();
            }
        }

        public PageClass GetPage(int pageno)
        {
            PageClass retpage = null;

            foreach (PageClass page in PageList)
            {
                if (page.No == pageno)
                {
                    retpage = page;
                }
            }
            return retpage;
        }
        public PageClass GetPageRun(int pagerunno)
        {
            PageClass retpage = null;

            foreach (PageClass page in PageList)
            {
                if (page.PageRunNo == pagerunno)
                {
                    retpage = page;
                }
            }
            return retpage;
        }

        #region Tools Operation
        void GetBMP(string bmpfilestr, ref Bitmap bmp)
        {
            Bitmap bmpTMP = new Bitmap(bmpfilestr);

            bmp.Dispose();
            bmp = new Bitmap(bmpTMP);

            bmpTMP.Dispose();
        }

        void ReadData(ref string datastr, string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None);
            StreamReader Srr = new StreamReader(fs, Encoding.Default);

            datastr = Srr.ReadToEnd();

            Srr.Close();
            Srr.Dispose();
        }
        public void DrawRect(Bitmap bmp, Rectangle Rect, SolidBrush B)
        {
            Graphics g = Graphics.FromImage(bmp);
            g.FillRectangle(B, Rect);
            g.Dispose();
        }

        #endregion

        #region Application Operation

        List<string> myProcessStringList = new List<string>();


        /// <summary>
        /// 執行預備的多執行緒工作
        /// </summary>
        /// <param name="IsMultiThread">是否启用多线程</param>
        /// <returns></returns>
        public bool A00_TrainProcess(bool IsMultiThread)
        {
            bool isgood = true;
            string str = "";



            if (Universal.IsMultiThread && IsMultiThread)
            {
                int itrain = 0;
                Parallel.ForEach(PageList, page =>
                {
                    page.ResetTrainStatus();
                    itrain++;
                    bool isgoodTemp = page.A00_Train(false);
                    if (!isgoodTemp)
                        isgood = false;
                });
            }
            else
            {
                str = "Start " + ToEnvString() + " Train process.";
                PrintMessage(str);

                if (Universal.isRcpUIOKClick && Universal.IsMultiThread)
                {
                    Parallel.ForEach(PageList, page =>
                    {
                        page.ResetTrainStatus();
                        bool isgoodTemp = page.A00_Train_Show();

                        if (!isgoodTemp)
                        {
                            isgood = false;
                            PrintMessage("Page: " + page.No + " Train NG Please Check! 页码为:" + page.No + " 训练不通过,请进入对应页面中,点'测试'查看");
                        }
                        else
                            PrintMessage("Page: " + page.No + " Train OK");
                    });
                }
                else
                {
                    foreach (PageClass page in PageList)
                    {
                        page.ResetTrainStatus();

                        isgood &= page.A00_Train();

                        if (!isgood)
                            break;
                    }
                }
            }

            return isgood;
        }

        JzTimes TestTime = new JzTimes();
        int[] Testms = new int[100];
        
       
        /// <summary>
        /// 开始跑线
        /// </summary>
        /// <param name="analyzelist"></param>
        /// <returns></returns>
        public bool A08_RunProcess(PageOPTypeEnum pageoptype)
        {
            bool isgood = true;
            List<PageClass> Temp = new List<PageClass>();
            for (int i = 0; i < PageList.Count; i++)
            {
                if (INI.CHECKPAGE[i])
                    Temp.Add(PageList[i]);
                else
                {
                    if (PageList[i].AnalyzeRootArray[(int)pageoptype].BranchList.Count > 0)
                    {
                        PageList[i].AnalyzeRootArray[(int)pageoptype].IsVeryGood = !INI.CHECKPAGE[i];

                        foreach (AnalyzeClass analyze in PageList[i].AnalyzeRootArray[(int)pageoptype].BranchList)
                        {
                            analyze.IsVeryGood = !INI.CHECKPAGE[i];
                            foreach (AnalyzeClass analyzetemp in analyze.BranchList)
                                analyzetemp.IsVeryGood = !INI.CHECKPAGE[i];
                        }
                    }
                }
            }
            if (Universal.IsMultiThread)
            {
                Parallel.ForEach(Temp, page =>
                {
                    bool ispagegood = true;

                    TestTime.Cut();
                    ispagegood = page.A08_RunProcess(pageoptype);

                    Testms[page.CamIndex] = TestTime.msDuriation;

                    if (!ispagegood)
                        isgood = false;
                });

                if (Universal.IsNoUseCCD)
                {
                    string strmess = "";
                    foreach (PageClass page in Temp)
                    {
                        if (page.PassInfo.RcpNo == 80002)
                        {
                            foreach (AnalyzeClass analy in page.AnalyzeRootArray)
                            {
                                foreach (AnalyzeClass branchanalyze in analy.BranchList)
                                {
                                    if (branchanalyze.MaskMethod == MaskMethodEnum.NONE)
                                    {
                                        if (branchanalyze.ALIGNPara.AlignMethod == AlignMethodEnum.AUFIND)
                                            strmess += branchanalyze.AliasName + ", " + branchanalyze.ALIGNPara.Score + Environment.NewLine;
                                    }
                                }
                            }
                        }
                    }
                    if (strmess != "")
                        File.WriteAllText("D:\\80002.txt", strmess, Encoding.Default);
                }
            }
            else
            {
                //单线程跑线
                #region
                int index = 0;
                foreach (PageClass page in Temp)
                {
                    bool ispagegood = true;

                    TestTime.Cut();

                    ispagegood = page.A08_RunProcess(pageoptype);

                    //page.bmpRUN[0].Save(Universal.TESTPATH + "\\ANALYZETEST\\"  + "-BMPRUN" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                    index++;

                    bool isbreak = false;

                    if (!ispagegood)    //如果是NG或是大定位失敗則跳出去
                    {
                        switch (VERSION)
                        {
                            default:
                                //若大定位失敗就跳出去
                                if (!page.IsPageFirstAlignPass(pageoptype))
                                    isbreak = true;

                                break;
                        }
                    }

                    Testms[0] = TestTime.msDuriation;

                    isgood &= ispagegood;

                    if (isbreak)
                        break;
                }
            }

            #endregion
            return isgood;
        }
        public bool A08_RunProcess(PageOPTypeEnum pageoptype,int iPageIndex)
        {
            bool isgood = true;
            List<PageClass> Temp = new List<PageClass>();
            int i = iPageIndex;
            if (INI.CHECKPAGE[i])
                Temp.Add(PageList[i]);
            else
            {
                if (PageList[i].AnalyzeRootArray[(int)pageoptype].BranchList.Count > 0)
                {
                    PageList[i].AnalyzeRootArray[(int)pageoptype].IsVeryGood = !INI.CHECKPAGE[i];

                    foreach (AnalyzeClass analyze in PageList[i].AnalyzeRootArray[(int)pageoptype].BranchList)
                    {
                        analyze.IsVeryGood = !INI.CHECKPAGE[i];
                        foreach (AnalyzeClass analyzetemp in analyze.BranchList)
                            analyzetemp.IsVeryGood = !INI.CHECKPAGE[i];
                    }
                }
            }

            //if (Universal.IsMultiThread)
            {
                //PageClass page = Temp[0];
                bool ispagegood = true;
                if (Temp.Count > 0)
                {
                    TestTime.Cut();
                    ispagegood = Temp[0].A08_RunProcess(pageoptype);

                    Testms[Temp[0].CamIndex] = TestTime.msDuriation;
                }
                else
                {
                    TestTime.Cut();
                    ispagegood = PageList[i].A08_RunProcess(pageoptype);

                    Testms[PageList[i].CamIndex] = TestTime.msDuriation;
                }

                if (!ispagegood)
                    isgood = false;

                if (Universal.IsNoUseCCD)
                {
                    string strmess = "";
                    foreach (PageClass page in Temp)
                    {
                        if (page.PassInfo.RcpNo == 80002)
                        {
                            foreach (AnalyzeClass analy in page.AnalyzeRootArray)
                            {
                                foreach (AnalyzeClass branchanalyze in analy.BranchList)
                                {
                                    if (branchanalyze.MaskMethod == MaskMethodEnum.NONE)
                                    {
                                        if (branchanalyze.ALIGNPara.AlignMethod == AlignMethodEnum.AUFIND)
                                            strmess += branchanalyze.AliasName + ", " + branchanalyze.ALIGNPara.Score + Environment.NewLine;
                                    }
                                }
                            }
                        }
                    }
                    if (strmess != "")
                        File.WriteAllText("D:\\80002.txt", strmess, Encoding.Default);
                }
            }
            //else
            {
                //单线程跑线
                #region
                //int index = 0;
                //foreach (PageClass page in Temp)
                //{
                //    bool ispagegood = true;

                //    TestTime.Cut();

                //    ispagegood = page.A08_RunProcess(pageoptype);

                //    //page.bmpRUN[0].Save(Universal.TESTPATH + "\\ANALYZETEST\\"  + "-BMPRUN" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                //    index++;

                //    bool isbreak = false;

                //    if (!ispagegood)    //如果是NG或是大定位失敗則跳出去
                //    {
                //        switch (VERSION)
                //        {
                //            default:
                //                //若大定位失敗就跳出去
                //                if (!page.IsPageFirstAlignPass(pageoptype))
                //                    isbreak = true;

                //                break;
                //        }
                //    }

                //    Testms[0] = TestTime.msDuriation;

                //    isgood &= ispagegood;

                //    if (isbreak)
                //        break;
                //}
                #endregion
            }


            return isgood;
        }

        /// <summary>
        /// 开始跑线
        /// </summary>
        /// <param name="analyzelist"></param>
        /// <param name="NoTestIndexPage">不要跑线的页码</param>
        /// <returns></returns>
        public bool A08_RunProcess(PageOPTypeEnum pageoptype, List<int> NoTestIndexPage)
        {
            bool isgood = true;
            List<PageClass> Temp = new List<PageClass>();
            for (int i = 0; i < PageList.Count; i++)
            {
                if (INI.CHECKPAGE[i])
                    Temp.Add(PageList[i]);
                else
                {
                    if (PageList[i].AnalyzeRootArray[(int)pageoptype].BranchList.Count > 0)
                    {
                        PageList[i].AnalyzeRootArray[(int)pageoptype].IsVeryGood = !INI.CHECKPAGE[i];

                        foreach (AnalyzeClass analyze in PageList[i].AnalyzeRootArray[(int)pageoptype].BranchList)
                        {
                            analyze.IsVeryGood = !INI.CHECKPAGE[i];
                            foreach (AnalyzeClass analyzetemp in analyze.BranchList)
                                analyzetemp.IsVeryGood = !INI.CHECKPAGE[i];
                        }
                    }
                }
            }
            List<PageClass> Temp2 = new List<PageClass>();
            foreach (PageClass pagetemp in Temp)
            {
                bool ischeck = true;
                foreach (int indextemp in NoTestIndexPage)
                {
                    if (pagetemp.PassInfo.RcpNo == 80005)
                    {
                        ischeck = false;
                        break;
                    }
                }
                if (ischeck)
                    Temp2.Add(pagetemp);
            }
            Temp.Clear();
            if (Universal.IsMultiThread)
            {
                Parallel.ForEach(Temp2, page =>
                {
                    bool ispagegood = true;

                    TestTime.Cut();
                    ispagegood = page.A08_RunProcess(pageoptype);

                    Testms[page.CamIndex] = TestTime.msDuriation;

                    if (!ispagegood)
                        isgood = false;
                });
            }
            else
            {
                //单线程跑线
                #region
                int index = 0;
                foreach (PageClass page in Temp2)
                {
                    bool ispagegood = true;

                    TestTime.Cut();

                    ispagegood = page.A08_RunProcess(pageoptype);

                    //page.bmpRUN[0].Save(Universal.TESTPATH + "\\ANALYZETEST\\"  + "-BMPRUN" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                    index++;

                    bool isbreak = false;

                    if (!ispagegood)    //如果是NG或是大定位失敗則跳出去
                    {
                        switch (VERSION)
                        {
                            default:
                                //若大定位失敗就跳出去
                                if (!page.IsPageFirstAlignPass(pageoptype))
                                    isbreak = true;

                                break;
                        }
                    }

                    Testms[0] = TestTime.msDuriation;

                    isgood &= ispagegood;

                    if (isbreak)
                        break;
                }
            }

            #endregion
            return isgood;
        }
        public void FillRunStatus(WorkStatusCollectionClass runstatuscollection)
        {
            foreach (PageClass page in PageList)
            {
                page.FillRunStatus(runstatuscollection);

            }
        }
        public void FillTrainStatus(WorkStatusCollectionClass trainstatuscollection)
        {
            foreach (PageClass page in PageList)
            {
                page.FillTrainStatus(trainstatuscollection);

            }
        }
        public void ResetRunStatus()
        {
            foreach (PageClass page in PageList)
            {
                page.ResetRunStatus();
            }
        }
        public void ResetTrainStatus()
        {
            foreach (PageClass page in PageList)
            {
                page.ResetTrainStatus();
            }
        }

        /// <summary>
        /// 收集訊息
        /// </summary>
        /// <param name="str"></param>
        void PrintMessage(string str)
        {
            lock (myProcessStringList)
            {
                myProcessStringList.Clear();
                myProcessStringList.Add(str);

                OnPrintMessage(myProcessStringList);
            }
        }

        public string GetCamString()
        {
            string str = "";

            foreach (PageClass page in PageList)
            {
                bool isdefaut = true;
                //str += page.Exposure.ToString() + ",";
                //Revise For Assign Camera
                //加人一個Page 多個相機要設定Exposure的選項
                switch (VERSION)
                {
                    case VersionEnum.ALLINONE:
                        switch (OPTION)
                        {
                            case OptionEnum.R32:
                            case OptionEnum.R15:
                            case OptionEnum.R26:
                            case OptionEnum.R9:
                            case OptionEnum.R5:
                            case OptionEnum.D19:
                            case OptionEnum.R1:
                                if (page.CamIndex == 0)
                                {
                                    //CAMINDEX # XXXXXX # Bias
                                    str += page.CamIndex.ToString() + "#" + (page.ExposureString.Trim() == "" ? page.Exposure.ToString() : page.ExposureString) + "#0;";
                                    isdefaut = false;
                                }
                                else
                                {
                                    str += page.CamIndex.ToString() + "#" + page.Exposure.ToString() + "#" + INI.CAMBIASCOUNT.ToString() + ";";
                                    isdefaut = false;
                                }

                                break;
                            case OptionEnum.R3:
                            case OptionEnum.C3:
                                str += page.CamIndex.ToString() + "#" + page.Exposure.ToString() + "#" + INI.CAMBIASCOUNT.ToString() + ";";
                                isdefaut = false;
                                break;
                        }
                        break;
                }

                if (isdefaut)
                    str += page.CamIndex.ToString() + "#" + page.Exposure.ToString() + "#0;";
            }

            if (str.Length > 1)
                str = str.Remove(str.Length - 1, 1);

            return str;
        }


        #region 自动编号 

        public void SetOffset(int pageindex, Point ePointOffset,bool ison)
        {
            PageClass pageClass = PageList[pageindex];
            pageClass.SetOffset(ePointOffset, ison);

            //pageClass.A00_Train_BMP(pageClass.bmpRUN[(int)PageOPTypeEnum.P00]);
        }

        public int m_Mapping_Col = 0;
        public int m_Mapping_Row = 0;

        public int DrawCol = 0;
        public int DrawRow = 0;
        public int[] DrawMapping;
        public string[] DrawMappingName;
        public Bitmap[] ResultBMPMapping;

        public void ResetMapping()
        {
            int i = 0;

            while (i < DrawMapping.Length)
            {
                DrawMapping[i] = 0;
                i++;
            }
        }

        public void EnvAutoReportIndex()
        {
            List<PageClass> BranchList = PageList;

            int Highest = 100000;
            int HighestIndex = -1;
            int ReportIndex = 0;
            List<string> CheckList = new List<string>();

            int i = 0;
            int j = 0;

            //Clear All Index To 0 and Check the Highest

            foreach (PageClass keyassign in BranchList)
            {
                keyassign.PageAutoReportIndex(keyassign.PageRunNo);

                keyassign.ReportRowCol = "";
                keyassign.ReportIndex = 0;
                ReportIndex = 1;

            }

            i = 0;
            j = 0;
            while (true)
            {
                i = 0;
                Highest = 100000;
                HighestIndex = -1;
                foreach (PageClass keyassign in BranchList)
                {
                    if (keyassign.ReportIndex == 0)
                    {
                        if (keyassign.PageRunLocation.Y < Highest)
                        {
                            Highest = (int)keyassign.PageRunLocation.Y;
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
                foreach (PageClass keyassign in BranchList)
                {
                    if (keyassign.ReportIndex == 0)
                    {
                        if (IsInRange((int)keyassign.PageRunLocation.Y, Highest, 4))
                        {
                            CheckList.Add(keyassign.PageRunLocation.X.ToString("00000000") + "," + i.ToString());
                        }
                    }
                    i++;
                }



                if (j % 2 == 0)
                    CheckList.Sort((item1, item2) =>
                    { return int.Parse(item1.Split(',')[0]) >= int.Parse(item2.Split(',')[0]) ? 1 : -1; });
                //CheckList.Sort();
                else
                    CheckList.Sort((item1, item2) =>
                    { return int.Parse(item1.Split(',')[0]) >= int.Parse(item2.Split(',')[0]) ? -1 : 1; });
                //CheckList.Sort();


                //if (j % 2 == 0)
                //    CheckList.Sort((item1, item2) =>
                //    { return int.Parse(item1.Split(',')[0]) > int.Parse(item2.Split(',')[0]) ? -1 : 1; });
                ////CheckList.Sort();
                //else
                //    CheckList.Sort((item1, item2) =>
                //    { return int.Parse(item1.Split(',')[0]) > int.Parse(item2.Split(',')[0]) ? 1 : -1; });
                ////CheckList.Sort();

                i = 1;
                foreach (string Str in CheckList)
                {
                    string[] Strs = Str.Split(',');

                    BranchList[int.Parse(Strs[1])].ReportIndex = ReportIndex;
                    BranchList[int.Parse(Strs[1])].ReportRowCol = CheckList.Count.ToString() + "-" + i.ToString();

                    ReportIndex++;
                    i++;
                }

                j++;
            }
            //原点在左下角
            BranchList.Sort((item1, item2) => { return item1.ReportIndex >= item2.ReportIndex ? -1 : 1; });
            //////原点在右上角
            //BranchList.Sort((item1, item2) => { return item1.ReportIndex >= item2.ReportIndex ? 1 : -1; });

            m_Mapping_Col = int.Parse(BranchList[0].ReportRowCol.Split('-')[0]);
            m_Mapping_Row = BranchList.Count / m_Mapping_Col;


            //先算出有多少行和列

            DrawCol = 0;
            DrawRow = 0;

            i = 0;
            foreach (PageClass page in BranchList)
            {
                if (i < m_Mapping_Col)
                {
                    DrawCol += page.m_Mapping_Col;
                }
                else
                {
                    break;
                }
                i++;
            }

            i = 0;
            foreach (PageClass page in BranchList)
            {
                if (i == 0 || i % m_Mapping_Col == 0)
                {
                    DrawRow += page.m_Mapping_Row;
                }
                i++;
            }

            int rowindex = 1;
            int colindex = 1;
            int icount = 1;

            int imappingindex = 0;
            DrawMapping = new int[DrawCol * DrawRow];
            DrawMappingName = new string[DrawCol * DrawRow];

            foreach (PageClass page in BranchList)
            {
                foreach (AnalyzeClass analyze in page.AnalyzeRoot.BranchList)
                {
                    if (analyze.CheckAnalyzeReadBarcode())
                        continue;

                    analyze.ReportIndex = icount;
                    analyze.AliasName = rowindex.ToString("000") + "-" + colindex.ToString("000");

                    if (imappingindex >= DrawMapping.Length)
                        break;
                    DrawMapping[imappingindex] = 0;
                    DrawMappingName[imappingindex] = analyze.AliasName;

                    //PassInfoClass passInfoClass = new PassInfoClass(analyze.PassInfo.ToString());
                    //passInfoClass.PassInfoName = rowindex.ToString() + "-" + colindex.ToString();
                    //analyze.PassInfo.FromPassInfo(passInfoClass, OPLevelEnum.COPY);

                    colindex++;
                    if (colindex > 1 && colindex % DrawCol == 1)
                    {
                        colindex = 1;
                        rowindex++;
                    }

                    icount++;
                    imappingindex++;
                }
            }
        }
        bool IsInRange(int FromValue, int CompValue, int DiffValue)
        {
            return (FromValue >= CompValue - DiffValue) && (FromValue <= CompValue + DiffValue);
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
            }
        }
    }
}
