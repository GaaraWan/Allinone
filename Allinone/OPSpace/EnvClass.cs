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
using static Allinone.UISpace.ALBUISpace.AllinoneAlbUI;
using Allinone.BasicSpace;
using iTextSharp.text.pdf;
using System.Windows.Input;

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
        public string GeneralBarcodeSetup = string.Empty;

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
            if (strs.Length > 3)
            {
                GeneralBarcodeSetup = strs[2];
            }
        }
        public override string ToString()
        {
            string retstr = "";

            retstr = GeneralLight + Universal.SeperateCharA;
            retstr += GeneralPosition + Universal.SeperateCharA;
            retstr += GeneralBarcodeSetup + Universal.SeperateCharA;
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
            //bool bOK = A01_trainOffsetProcess();
            return isgood;
        }

        public bool A01_trainOffsetProcess()
        {
            bool bOK1 = false;
            int index1 = 0;
            //获取左定位点
            PointF LeftPoint = new PointF(0, 0);
            int i = 0;
            foreach (PageClass page in PageList)
            {
                bOK1 = page.GetBaseMainPointF(AbsoluteAlignEnum.MAIN_LEFT, true, out LeftPoint);
                if (bOK1)
                {
                    //LeftPoint.X += page.PageOPTypeIndex * page.GetbmpORG().Width;
                    index1 = i;
                    break;
                }
                i++;
            }

            bool bOK2 = false;
            int index2 = 0;
            //获取右定位点
            PointF RightPoint = new PointF(0, 0);
            i = 0;
            foreach (PageClass page in PageList)
            {
                bOK2 = page.GetBaseMainPointF(AbsoluteAlignEnum.MAIN_RIGHT, true, out RightPoint);
                if (bOK2)
                {
                    //RightPoint.X += page.PageOPTypeIndex * page.GetbmpORG().Width;
                    index2 = i;
                    break;
                }
                i++;
            }

            if (Universal.IsUseCalibration)
            {
                i = 0;
                foreach (PageClass page in PageList)
                {
                    if (bOK1 && bOK2)
                    {
                        page.PageAutoCalibration(i);
                    }
                    i++;
                }
                if (bOK1 && bOK2)
                {
                    PageList[index1].AoiCalibrationEx.TransformViewToWorld(LeftPoint, out PointF leftpoiintw);
                    PageList[index2].AoiCalibrationEx.TransformViewToWorld(RightPoint, out PointF rightpointw);

                    foreach (PageClass page in PageList)
                    {
                        if (bOK1 && bOK2)
                        {
                            page.SetABSMainPoint(bOK1 && bOK2, leftpoiintw, rightpointw);
                        }
                    }
                }
            }
            else
            {
                foreach (PageClass page in PageList)
                {
                    if (bOK1 && bOK2)
                    {
                        page.SetABSMainPoint(bOK1 && bOK2, LeftPoint, RightPoint);
                    }
                }
            }
            return bOK1 && bOK2;
        }
        public bool A08_RunOffsetProcess()
        {
            bool bOK = true;
            int index1 = 0;
            int index2 = 0;
            int i = 0;

            bool bOK1 = false;
            //获取左定位点
            PointF LeftPoint = new PointF(0, 0);
            i = 0;
            foreach (PageClass page in PageList)
            {
                bOK1 = page.GetBaseMainPointF(AbsoluteAlignEnum.MAIN_LEFT, false, out LeftPoint);
                if (bOK1)
                {
                    //LeftPoint.X += page.PageOPTypeIndex * page.GetbmpORG().Width;
                    index1 = i;
                    break;
                }
                i++;
            }

            bool bOK2 = false;
            //获取右定位点
            PointF RightPoint = new PointF(0, 0);
            i = 0;
            foreach (PageClass page in PageList)
            {
                bOK2 = page.GetBaseMainPointF(AbsoluteAlignEnum.MAIN_RIGHT, false, out RightPoint);
                if (bOK2)
                {
                    //RightPoint.X += page.PageOPTypeIndex * page.GetbmpORG().Width;
                    index2 = i;
                    break;
                }
                i++;
            }
            if (Universal.IsUseCalibration)
            {
                if (bOK1 && bOK2)
                {
                    PageList[index1].AoiCalibrationEx.TransformViewToWorld(LeftPoint, out PointF leftpoiintw);
                    PageList[index2].AoiCalibrationEx.TransformViewToWorld(RightPoint, out PointF rightpointw);

                    foreach (PageClass page in PageList)
                    {
                        if (bOK1 && bOK2)
                        {
                            page.SetABSMainPointRun(bOK1 && bOK2, leftpoiintw, rightpointw);
                        }
                    }
                }
            }
            else
            {
                foreach (PageClass page in PageList)
                {
                    if (bOK1 && bOK2)
                    {
                        page.SetABSMainPointRun(bOK1 && bOK2, LeftPoint, RightPoint);
                    }
                }
            }

            //计算每个框的相对距离偏差
            foreach (PageClass page in PageList)
            {
                page.A100_02RunCheckLeftRight(page.AnalyzeRoot);
            }

            return bOK;
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

        public bool A09_RunRepeatCodeProcess(PageOPTypeEnum pageoptype, bool echeckCurLotRepeatCode)
        {
            bool isgood = true;
            //收集所有页面读取到的二维码
            List<string> _collectCodeList = new List<string>();

            _collectCodeList.Clear();
            for (int i = 0; i < PageList.Count; i++)
            {
                if (PageList[i].AnalyzeRootArray[(int)pageoptype].BranchList.Count > 0)
                {
                    foreach (AnalyzeClass analyze in PageList[i].AnalyzeRootArray[(int)pageoptype].BranchList)
                    {
                        string barcodeStr = analyze.GetAnalyzeOnlyBarcodeStr();
                        if (!string.IsNullOrEmpty(barcodeStr))
                            _collectCodeList.Add(barcodeStr);
                    }
                }
            }

            List<string> _collectRepeatCodeList = new List<string>();
            //查询数据库中所有的条码
            isgood = Universal.JZMAINSDPOSITIONPARA.MySqlTableQuery(_collectCodeList, ref _collectRepeatCodeList) <= 0;
            List<string> _collectNoRepeatCodeList = new List<string>();
            //匹配到各个分支
            if (!isgood)
            {
                _collectNoRepeatCodeList.Clear();
                for (int i = 0; i < PageList.Count; i++)
                {
                    if (PageList[i].AnalyzeRootArray[(int)pageoptype].BranchList.Count > 0)
                    {
                        foreach (AnalyzeClass analyze in PageList[i].AnalyzeRootArray[(int)pageoptype].BranchList)
                        {
                            string barcodeStr = analyze.GetAnalyzeOnlyBarcodeStr();
                            if (!string.IsNullOrEmpty(barcodeStr))
                            {
                                bool bOK = analyze.CheckRepeatCode(_collectRepeatCodeList, 0);
                                //if (analyze.IsByPass)
                                //    analyze.IsByPass = analyze.IsByPass;
                                analyze.IsVeryGood &= (bOK || analyze.IsByPass);
                                //analyze.IsVeryGood &= bOK;
                                if (bOK)
                                {
                                    _collectNoRepeatCodeList.Add(barcodeStr);
                                }
                            }
                        }
                    }
                }

                Universal.JZMAINSDPOSITIONPARA.MySqlTableInsert(_collectNoRepeatCodeList);

            }
            else
            {
                Universal.JZMAINSDPOSITIONPARA.MySqlTableInsert(_collectCodeList);
            }

            //比对同一片的重复码
            for (int i = 0; i < PageList.Count; i++)
            {
                if (PageList[i].AnalyzeRootArray[(int)pageoptype].BranchList.Count > 0)
                {
                    foreach (AnalyzeClass analyze in PageList[i].AnalyzeRootArray[(int)pageoptype].BranchList)
                    {
                        //if (analyze.IsVeryGood)
                        {
                            bool bOK = analyze.CheckRepeatCode(_collectCodeList);
                            analyze.IsVeryGood &= (bOK || analyze.IsByPass);
                            //analyze.IsVeryGood &= bOK;
                            isgood &= bOK;
                        }
                    }
                }
            }
            return isgood;
        }

        #region 备份之前的测试代码
        /*
         * 
         public bool A09_RunRepeatCodeProcess(PageOPTypeEnum pageoptype,bool echeckCurLotRepeatCode)
        {
            bool isgood = true;
            //收集所有页面读取到的二维码
            List<string> _collectCodeList = new List<string>();
            List<string> _collectCodeList_single = new List<string>();
            for (int i = 0; i < PageList.Count; i++)
            {
                if (PageList[i].AnalyzeRootArray[(int)pageoptype].BranchList.Count > 0)
                {
                    foreach (AnalyzeClass analyze in PageList[i].AnalyzeRootArray[(int)pageoptype].BranchList)
                    {
                        _collectCodeList_single.Clear();
                        string barcodeStr = analyze.GetAnalyzeOnlyBarcodeStr();
                        if (!string.IsNullOrEmpty(barcodeStr))
                        {
                            _collectCodeList.Add(barcodeStr);
                            //插入数据
                            //if (INI.IsOpenCheckCurLotRepeatCode)
                            if (echeckCurLotRepeatCode)
                            {
                                //查询表是否存在条码   <=0表示不存在
                                bool bOK = Universal.JZMAINSDPOSITIONPARA.MySqlTableQuery(barcodeStr) <= 0;
                                if (bOK)
                                {
                                    Universal.JZMAINSDPOSITIONPARA.MySqlTableInsert(barcodeStr);
                                    ////检查表是否存在
                                    //bool bExist = Universal.JZMAINSDPOSITIONPARA.MySqlCheckTableExist();
                                    //if (!bExist)
                                    //{
                                    //    //不存在则建立表
                                    //    int iret = Universal.JZMAINSDPOSITIONPARA.MySqlCreateTable();
                                    //    if (iret >= 0)//建立成功插入数据
                                    //        Universal.JZMAINSDPOSITIONPARA.MySqlTableInsert(barcodeStr);
                                    //}
                                }
                                else
                                {
                                    _collectCodeList_single.Add(barcodeStr);
                                    _collectCodeList_single.Add(barcodeStr);
                                    bOK = analyze.CheckRepeatCode(_collectCodeList_single);
                                }
                                analyze.IsVeryGood = bOK || analyze.IsByPass;
                                isgood &= bOK;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < PageList.Count; i++)
            {
                if (PageList[i].AnalyzeRootArray[(int)pageoptype].BranchList.Count > 0)
                {
                    foreach (AnalyzeClass analyze in PageList[i].AnalyzeRootArray[(int)pageoptype].BranchList)
                    {
                        //if (analyze.IsVeryGood)
                        {
                            bool bOK = analyze.CheckRepeatCode(_collectCodeList);
                            analyze.IsVeryGood = bOK || analyze.IsByPass;
                            isgood &= bOK;
                        }
                    }
                }
            }
            return isgood;
        }
         * 
         */
        #endregion


        #region 分步走位 顺序和S型 MAPPING_A

        public int StepCount
        {
            get { return stepRows * stepCols; }
        }
        //public int StepRows
        //{
        //    get { return stepRows; }
        //    //set { stepRows = value; }
        //}
        //public int StepCols
        //{
        //    get { return stepCols; }
        //    //set { stepCols = value; }
        //}
        
        int stepRows = 1;
        int stepCols = 1;
        int stepCurrent = 0;


        // 网格大小
        int GridRows = 1;
        int GridCols = 1;

        // 滑块大小
        int SliderRows = 1;
        int SliderCols = 1;

        // 网格和滑块
        JzSliderItemClass[,] grid = new JzSliderItemClass[1, 1];
        JzSliderItemClass[,] slider = new JzSliderItemClass[1, 1];

        // 滑块的初始位置
        int sliderX = 0;
        int sliderY = 0;

        PathPlan myPathPlan = PathPlan.p1;
        bool myDirToLeft = false;
        bool myDirToDown = false;

        // 初始化网格和滑块
        public void MappingA_Initialize()
        {
            PageList[0].PageAutoReportIndexMappingA(0);

            Light2Settings _light = new Light2Settings();
            _light.GetString(GeneralLight);

            GridRows = _light.ChipRow;
            GridCols = _light.ChipCol;

            SliderRows = PageList[0].m_Mapping_Row;
            SliderCols = PageList[0].m_Mapping_Col;

            if (GridRows == 0)
                GridRows = 1;
            if (GridCols == 0)
                GridCols = 1;
            if (SliderRows == 0)
                SliderRows = 1;
            if (SliderCols == 0)
                SliderCols = 1;

            myPathPlan = _light.ChipPathPlan;
            myDirToLeft = false;

            grid = new JzSliderItemClass[GridRows, GridCols];
            slider = new JzSliderItemClass[SliderRows, SliderCols];

            //计算需要移动的步数
            stepRows = (GridRows / SliderRows) + (GridRows % SliderRows == 0 ? 0 : 1);
            stepCols = (GridCols / SliderCols) + (GridCols % SliderCols == 0 ? 0 : 1);

            // 初始化网格为0（空白）
            for (int i = 0; i < GridRows; i++)
            {
                for (int j = 0; j < GridCols; j++)
                {
                    grid[i, j] = new JzSliderItemClass();
                    grid[i, j].IntOperate = 0;
                }
            }

            int k = 0;
            // 初始化滑块为1（滑块区域）
            for (int i = 0; i < SliderRows; i++)
            {
                for (int j = 0; j < SliderCols; j++)
                {
                    slider[i, j] = new JzSliderItemClass();
                    slider[i, j].IntOperate = 1;

                    if (PageList[0].AnalyzeRoot.BranchList.Count > 0)
                    {
                        slider[i, j].AnalyzeNameStr = PageList[0].AnalyzeRoot.BranchList[k].ToAnalyzeString();
                        slider[i, j].AnalyzeOpeateStr = _rectToString(PageList[0].AnalyzeRoot.BranchList[k].myOPRectF);
                        k++;


                    }
                }
            }

            MappingA_GridClear(); 
            MappingA_SliderClear();
        }
        public void MappingA_GridClear()
        {
            // 清空网格
            for (int i = 0; i < GridRows; i++)
            {
                for (int j = 0; j < GridCols; j++)
                {
                    grid[i, j].Reset();
                }
            }

            switch (myPathPlan)
            {
                case PathPlan.p4:
                    //case PathPlan.p4:

                    sliderX = GridRows - SliderRows;
                    sliderY = GridCols - SliderCols;

                    //if (GridRows % SliderRows == 0)
                    //{
                    //    sliderX = GridRows - SliderRows;
                    //}
                    //else
                    //{
                    //    sliderX = GridRows - (GridRows % SliderRows);
                    //}
                   
                    //if (GridCols % SliderCols == 0)
                    //    sliderY = GridCols - SliderCols;
                    //else
                    //{
                    //    sliderY = GridCols - (GridCols % SliderCols);
                    //}

                    stepCurrent = 0;
                    myDirToDown = true;
                    break;
                case PathPlan.p3:
                    //case PathPlan.p4:
                    sliderX = 0;// GridRows - SliderRows;
                    sliderY = GridCols - SliderCols;

                    //if (GridCols % SliderCols == 0)
                    //    sliderY = GridCols - SliderCols;
                    //else
                    //{
                    //    sliderY = GridCols - (GridCols % SliderCols);
                    //}
                    stepCurrent = 0;
                    myDirToDown = false;
                    break;
                default:
                    sliderX = 0;
                    sliderY = 0;
                    stepCurrent = 0;
                    myDirToLeft = false;
                    break;
            }
        }
        public void MappingA_GridMapping2dClear()
        {
            // 清空网格
            for (int i = 0; i < GridRows; i++)
            {
                for (int j = 0; j < GridCols; j++)
                {
                    grid[i, j].ResetMapping2d();
                }
            }
        }
        public int MappingA_GridSetMapping2d(string[] eValues, ref string datalogStr)
        {
            int grid_length = GridRows * GridCols;
            datalogStr = string.Empty;
            if (eValues == null)
                return -3;
            datalogStr += grid_length.ToString() + ",";
            datalogStr += grid_length.ToString() + ",";
            datalogStr += eValues.Length.ToString();

            if (grid_length != eValues.Length)
                return -5;

            //sliderX = 0;
            //sliderY = 0;

            int k = 0;
            // 清空网格
            for (int i = 0; i < GridRows; i++)
            {
                for (int j = 0; j < GridCols; j++)
                {
                    grid[i, j].Mapping2dStr = eValues[k];
                    k++;
                }
            }

            // 将滑块放入网格 赋值条码
            for (int i = 0; i < SliderRows; i++)
            {
                for (int j = 0; j < SliderCols; j++)
                {
                    if (sliderX + i < GridRows && sliderY + j < GridCols)
                    {
                        string bar = grid[sliderX + i, sliderY + j].Mapping2dStr;
                        //slider[i, j].Mapping2dStr = bar;
                        foreach (PageClass page in PageList)
                        {
                            foreach (AnalyzeClass analyze in page.AnalyzeRoot.BranchList)
                            {
                                if (slider[i, j].AnalyzeNameStr == analyze.ToAnalyzeString())
                                {
                                    analyze.SetAnalyzeCheckBarcodeStr(bar);
                                }
                            }
                        }
                    }
                }
            }


            return 0;
        }
        public int MappingA_GridSetMappingBypass(bool[] eValues, ref string datalogStr)
        {
            int grid_length = GridRows * GridCols;
            datalogStr = string.Empty;
            if (eValues == null)
                return -3;
            datalogStr += grid_length.ToString() + ",";
            datalogStr += grid_length.ToString() + ",";
            datalogStr += eValues.Length.ToString();

            if (grid_length != eValues.Length)
                return -5;

            int k = 0;
            // 清空网格
            for (int i = 0; i < GridRows; i++)
            {
                for (int j = 0; j < GridCols; j++)
                {
                    grid[i, j].AnalyzeBypass = eValues[k];
                    k++;
                }
            }

            // 将滑块放入网格 赋值条码
            for (int i = 0; i < SliderRows; i++)
            {
                for (int j = 0; j < SliderCols; j++)
                {
                    if (sliderX + i < GridRows && sliderY + j < GridCols)
                    {
                        bool bar = grid[sliderX + i, sliderY + j].AnalyzeBypass;
                        //slider[i, j].Mapping2dStr = bar;
                        foreach (PageClass page in PageList)
                        {
                            foreach (AnalyzeClass analyze in page.AnalyzeRoot.BranchList)
                            {
                                if (slider[i, j].AnalyzeNameStr == analyze.ToAnalyzeString())
                                {
                                    analyze.SetAnalyzeByPass(bar);
                                }
                            }
                        }
                    }
                }
            }


            return 0;
        }
        public void MappingA_SliderClear()
        {
            // 初始化滑块为1（滑块区域）
            for (int i = 0; i < SliderRows; i++)
            {
                for (int j = 0; j < SliderCols; j++)
                {
                    slider[i, j].IntResult = -1;
                    slider[i, j].StrMessage = string.Empty;
                }
            }

            //// 将滑块放入网格 赋值条码
            //for (int i = 0; i < SliderRows; i++)
            //{
            //    for (int j = 0; j < SliderCols; j++)
            //    {
            //        if (sliderX + i < GridRows && sliderY + j < GridCols)
            //        {
            //            string bar = grid[sliderX + i, sliderY + j].Mapping2dStr;
            //            //slider[i, j].Mapping2dStr = bar;
            //            foreach (PageClass page in PageList)
            //            {
            //                foreach (AnalyzeClass analyze in page.AnalyzeRoot.BranchList)
            //                {
            //                    if (slider[i, j].AnalyzeNameStr == analyze.ToAnalyzeString())
            //                    {
            //                        analyze.SetAnalyzeCheckBarcodeStr(bar);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
        }
        public void MappingA_CopyAnalyze()
        {
            // 将滑块放入网格 赋值条码
            for (int i = 0; i < SliderRows; i++)
            {
                for (int j = 0; j < SliderCols; j++)
                {
                    if (sliderX + i < GridRows && sliderY + j < GridCols)
                    {
                        int x = sliderX + i;
                        int y = sliderY + j;

                        if (x >= 0 && y >= 0)
                        {
                            string bar = grid[x, y].Mapping2dStr;
                            bool bypass = grid[x, y].AnalyzeBypass;
                            //slider[i, j].Mapping2dStr = bar;
                            foreach (PageClass page in PageList)
                            {
                                foreach (AnalyzeClass analyze in page.AnalyzeRoot.BranchList)
                                {
                                    if (slider[i, j].AnalyzeNameStr == analyze.ToAnalyzeString())
                                    {
                                        analyze.SetAnalyzeCheckBarcodeStr(bar);
                                        analyze.SetAnalyzeByPass(bypass);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public void MappingA_SetCurrentStep(int eCurrentStep = 0)
        {
            stepCurrent = eCurrentStep;

            switch(myPathPlan)
            {
                case PathPlan.p4:

                    if (stepCurrent % stepRows == 0 && stepCurrent > 0)
                    {
                        sliderY -= SliderCols; // 左移
                        sliderX = GridRows - SliderRows;
                    }
                    MappingA_UpdateGrid(eCurrentStep);
                    sliderX -= SliderRows; // 上移
                    break;
                case PathPlan.p3:

                    if (stepCurrent % stepRows == 0 && stepCurrent > 0)
                    {
                        sliderY -= SliderCols; // 左移
                        sliderX = 0;
                    }

                    MappingA_UpdateGrid(eCurrentStep);
                    sliderX += SliderRows; // 下移

                    break;
                case PathPlan.p2:

                    if (stepCurrent % stepCols == 0 && stepCurrent > 0)
                    {
                        sliderX += SliderRows; // 下移
                        myDirToLeft = !myDirToLeft;

                        if (sliderY <= 0 || sliderY + SliderCols > GridCols)
                        {
                            if (myDirToLeft)
                            {
                                if (GridCols % SliderCols == 0)
                                    sliderY = GridCols - SliderCols;
                                else
                                    sliderY = GridCols - (GridCols % SliderCols);
                                //sliderY -= SliderCols; // 左移
                            }
                            else
                            {
                                sliderY = 0;
                            }
                        }
                    }
                        

                    ////var key = JetMoveType.MOVE_RIGHT;
                    //if (stepCurrent % stepCols == 0 && stepCurrent > 0)
                    //{
                    //    sliderX += SliderRows; // 下移
                    //                           //if (sliderX % 2 == 0 && sliderX > 0)
                    //                           //{
                    //                           //    sliderY = 0;
                    //                           //    myDirToLeft = false;
                    //                           //}
                    //                           //else
                    //                           //{
                    //                           //    if (GridCols == SliderCols)
                    //                           //    {
                    //                           //        sliderY = 0;
                    //                           //        myDirToLeft = false;
                    //                           //    }
                    //                           //    else
                    //                           //    {
                    //                           //        if (GridCols % 2 == 0)
                    //                           //            sliderY = GridCols - SliderCols;
                    //                           //        else
                    //                           //            sliderY = GridCols - (GridCols % SliderCols);
                    //                           //        myDirToLeft = true;
                    //                           //    }
                    //                           //}

                    //    myDirToLeft = !myDirToLeft;

                    //    //if (myDirToLeft)
                    //    //{
                    //    //    if (sliderY >= SliderCols)
                    //    //        sliderY -= SliderCols; // 左移
                    //    //}
                    //    //else
                    //    //{
                    //    //    if (sliderY <= GridCols - SliderCols)
                    //    //        sliderY += SliderCols; // 右移
                    //    //}
                    //}
                    if (myDirToLeft)
                    {
                        MappingA_UpdateGrid(eCurrentStep);
                        if (sliderY >= SliderCols)
                            sliderY -= SliderCols; // 左移
                    }
                    else
                    {
                        MappingA_UpdateGrid(eCurrentStep);
                        if (sliderY <= GridCols - SliderCols)
                            sliderY += SliderCols; // 右移
                    }

                    break;
                default:

                    //var key = JetMoveType.MOVE_RIGHT;
                    if (stepCurrent % stepCols == 0 && stepCurrent > 0)
                    {
                        sliderX += SliderRows; // 下移
                        sliderY = 0;
                    }

                    MappingA_UpdateGrid(eCurrentStep);
                    sliderY += SliderCols; // 右移

                    break;
            }
        }
        public void MappingA_GridList(ref List<JzSliderItemClass> mapList)
        {
            mapList.Clear();
            // 打印网格
            for (int i = 0; i < GridRows; i++)
            {
                for (int j = 0; j < GridCols; j++)
                {
                    mapList.Add(grid[i, j]);
                    Console.Write(grid[i, j].IntResult.ToString());
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        public bool IsPass()
        {
            bool isPass = true;
            #region 添加比对码 当前批号重复码 此条的重复码

            //if (!INI.IsOpenForceNoCheckRepeat)
            //{
            //    List<string> _collectCodeList = new List<string>();
            //    _collectCodeList.Clear();

            //    Light2Settings _light = new Light2Settings();
            //    _light.GetString(GeneralLight);
            //    //检测比对码
            //    if (_light.IsCheckBarcodeOpen)
            //    {
            //        for (int i = 0; i < GridRows; i++)
            //        {
            //            for (int j = 0; j < GridCols; j++)
            //            {
            //                if (!grid[i, j].CheckBarcode())
            //                {
            //                    grid[i, j].IntResult = 7;//2d比对错误
            //                    grid[i, j].StrMessage = $"Compare[FAIL] ";
            //                }
            //                else
            //                {
            //                    grid[i, j].StrMessage = $"Compare[PASS] ";
            //                }
            //                grid[i, j].StrMessage += Environment.NewLine;
            //                grid[i, j].StrMessage += $"Marking 2D[{grid[i, j].Mapping2dStr}] ";
            //                grid[i, j].StrMessage += Environment.NewLine;
            //                grid[i, j].StrMessage += $"Read 2D[{grid[i, j].Read2dStr}] ";
            //                grid[i, j].StrMessage += Environment.NewLine;
            //            }
            //        }
            //    }

            //    //检测重复码
            //    if (_light.IsOpenCheckRepeatCode)
            //    {
            //        //判断表是否存在
            //        bool bExist = Universal.JZMAINSDPOSITIONPARA.MySqlCheckTableExist();
            //        if (!bExist)
            //        {
            //            int iret = Universal.JZMAINSDPOSITIONPARA.MySqlCreateTable();
            //        }
            //        for (int i = 0; i < GridRows; i++)
            //        {
            //            for (int j = 0; j < GridCols; j++)
            //            {
            //                if (!string.IsNullOrEmpty(grid[i, j].Read2dStr))
            //                {
            //                    _collectCodeList.Add(grid[i, j].Read2dStr);
            //                }
            //            }
            //        }

            //        List<string> _collectRepeatCodeList = new List<string>();
            //        //查询数据库中所有的条码
            //        bool bOK = Universal.JZMAINSDPOSITIONPARA.MySqlTableQuery(_collectCodeList, ref _collectRepeatCodeList) <= 0;
            //        List<string> _collectNoRepeatCodeList = new List<string>();
            //        //匹配到各个分支
            //        if (!bOK)
            //        {
            //            _collectNoRepeatCodeList.Clear();
            //            for (int i = 0; i < GridRows; i++)
            //            {
            //                for (int j = 0; j < GridCols; j++)
            //                {
            //                    bool bok = true;
            //                    if (!string.IsNullOrEmpty(grid[i, j].Read2dStr))
            //                    {
            //                        foreach (string s in _collectRepeatCodeList)
            //                        {
            //                            //if (s.Contains(m_BarcodeReadStr))
            //                            if (s.Trim() == grid[i, j].Read2dStr.Trim())
            //                            {
            //                                bok = false;
            //                                break;
            //                            }
            //                        }

            //                        if (!bOK)
            //                        {
            //                            grid[i, j].IntResult = 9;//2d重复
            //                            _collectNoRepeatCodeList.Add(grid[i, j].Read2dStr.Trim());
            //                        }
            //                    }
            //                }
            //            }

            //            Universal.JZMAINSDPOSITIONPARA.MySqlTableInsert(_collectNoRepeatCodeList);
            //        }
            //        else
            //        {
            //            Universal.JZMAINSDPOSITIONPARA.MySqlTableInsert(_collectCodeList);
            //        }

            //    }
            //}

            #endregion


            if (INI.IsOpenFaultToleranceRate)
            {
                int ngcount = 0;
                int count = 0;

                for (int i = 0; i < GridRows; i++)
                {
                    for (int j = 0; j < GridCols; j++)
                    {
                        if (grid[i, j].IntResult != 0)
                        {
                            ngcount++;
                        }
                        count++;
                    }
                }

                double rate = ngcount * 1.0 / count;
                isPass = rate <= INI.FaultToleranceRate;

            }
            else
            {
                for (int i = 0; i < GridRows; i++)
                {
                    for (int j = 0; j < GridCols; j++)
                    {
                        if (grid[i, j].IntResult != 0)
                        {
                            isPass = false;
                            break;
                        }
                    }
                }
            }

            MappingA_GridMapping2dClear();
            return isPass;
        }
        public string GetShow2dMessage(AnalyzeClass eAnalyze)
        {
            string str = string.Empty;
            for (int i = 0; i < SliderRows; i++)
            {
                for (int j = 0; j < SliderCols; j++)
                {
                    if (slider[i, j].AnalyzeNameStr == eAnalyze.ToAnalyzeString())
                    {
                        str = slider[i, j].Show2dMessage;
                    }
                }
            }
            return str;
        }
        // 更新网格显示
        void MappingA_UpdateGrid(int eStepIndex)
        {
            //对应结果数据
            WorkStatusCollectionClass RunStatusCollectionTemp = new WorkStatusCollectionClass();
            FillRunStatus(RunStatusCollectionTemp);
            for (int i = 0; i < SliderRows; i++)
            {
                for (int j = 0; j < SliderCols; j++)
                {
                    foreach (PageClass page in PageList)
                    {
                        foreach (AnalyzeClass analyze in page.AnalyzeRoot.BranchList)
                        {
                            PassInfoClass passInfo = new PassInfoClass();
                            Color color = myCheckAnalyzeResult(analyze, RunStatusCollectionTemp, out passInfo);
                            if (slider[i, j].AnalyzeNameStr == analyze.ToAnalyzeString())
                            {
                                slider[i, j].IntResult = _getColorIndex(color);
                                slider[i, j].StrMessage = _getAnalyzeBarcodeStr(analyze);
                                slider[i, j].IntStepIndex = eStepIndex;
                                slider[i, j].Read2dStr = analyze.GetAnalyzeOnlyBarcodeStr();
                                //slider[i, j].Show2dMessage = _getAnalyzeBarcodeStr(analyze);
                                string bar = string.Empty;
                                analyze.CollectAllBarcodeStr(ref bar);
                                slider[i, j].Show2dMessage = bar;
                            }
                        }
                    }
                }
            }


            //// 清空网格
            //for (int i = 0; i < GridRows; i++)
            //{
            //    for (int j = 0; j < GridCols; j++)
            //    {
            //        grid[i, j].IntOperate = 0;
            //    }
            //}

            // 将滑块放入网格
            for (int i = 0; i < SliderRows; i++)
            {
                for (int j = 0; j < SliderCols; j++)
                {
                    if (sliderX + i < GridRows && sliderY + j < GridCols)
                    {
                        int x = sliderX + i;
                        int y = sliderY + j;

                        if (x >= 0 && y >= 0)
                            grid[sliderX + i, sliderY + j].Clone(slider[i, j]);
                        //grid[sliderX + i, sliderY + j] = slider[i, j];
                    }
                }
            }

            #region 添加比对码 当前批号重复码 此条的重复码

            //if (!INI.IsOpenForceNoCheckRepeat)
            //{
            //    List<string> _collectCodeList = new List<string>();
            //    _collectCodeList.Clear();

            //    Light2Settings _light = new Light2Settings();
            //    _light.GetString(GeneralLight);
            //    //检测比对码
            //    if (_light.IsCheckBarcodeOpen)
            //    {
            //        for (int i = 0; i < GridRows; i++)
            //        {
            //            for (int j = 0; j < GridCols; j++)
            //            {
            //                if (!grid[i, j].CheckBarcode())
            //                {
            //                    grid[i, j].IntResult = 7;//2d比对错误
            //                    grid[i, j].StrMessage = $"Compare[FAIL] ";
            //                }
            //                else
            //                {
            //                    grid[i, j].StrMessage = $"Compare[PASS] ";
            //                }
            //                grid[i, j].StrMessage += Environment.NewLine;
            //                grid[i, j].StrMessage += $"Marking 2D[{grid[i, j].Mapping2dStr}] ";
            //                grid[i, j].StrMessage += Environment.NewLine;
            //                grid[i, j].StrMessage += $"Read 2D[{grid[i, j].Read2dStr}] ";
            //                grid[i, j].StrMessage += Environment.NewLine;
            //            }
            //        }
            //    }

            //    //检测重复码
            //    if (_light.IsOpenCheckRepeatCode)
            //    {
            //        //判断表是否存在
            //        bool bExist = Universal.JZMAINSDPOSITIONPARA.MySqlCheckTableExist();
            //        if (!bExist)
            //        {
            //            int iret = Universal.JZMAINSDPOSITIONPARA.MySqlCreateTable();
            //        }
            //        for (int i = 0; i < GridRows; i++)
            //        {
            //            for (int j = 0; j < GridCols; j++)
            //            {
            //                if (!string.IsNullOrEmpty(grid[i, j].Read2dStr))
            //                {
            //                    _collectCodeList.Add(grid[i, j].Read2dStr);
            //                }
            //            }
            //        }

            //        List<string> _collectRepeatCodeList = new List<string>();
            //        //查询数据库中所有的条码
            //        bool bOK = Universal.JZMAINSDPOSITIONPARA.MySqlTableQuery(_collectCodeList, ref _collectRepeatCodeList) <= 0;
            //        List<string> _collectNoRepeatCodeList = new List<string>();
            //        //匹配到各个分支
            //        if (!bOK)
            //        {
            //            _collectNoRepeatCodeList.Clear();
            //            for (int i = 0; i < GridRows; i++)
            //            {
            //                for (int j = 0; j < GridCols; j++)
            //                {
            //                    bool bok = true;
            //                    if (!string.IsNullOrEmpty(grid[i, j].Read2dStr))
            //                    {
            //                        foreach (string s in _collectRepeatCodeList)
            //                        {
            //                            //if (s.Contains(m_BarcodeReadStr))
            //                            if (s.Trim() == grid[i, j].Read2dStr.Trim())
            //                            {
            //                                bok = false;
            //                                break;
            //                            }
            //                        }

            //                        if (!bOK)
            //                        {
            //                            grid[i, j].IntResult = 9;//2d重复
            //                            _collectNoRepeatCodeList.Add(grid[i, j].Read2dStr.Trim());
            //                        }
            //                    }
            //                }
            //            }

            //            Universal.JZMAINSDPOSITIONPARA.MySqlTableInsert(_collectNoRepeatCodeList);
            //        }
            //        else
            //        {
            //            Universal.JZMAINSDPOSITIONPARA.MySqlTableInsert(_collectCodeList);
            //        }

            //    }
            //}

            #endregion

            //// 打印网格
            //for (int i = 0; i < GridRows; i++)
            //{
            //    for (int j = 0; j < GridCols; j++)
            //    {
            //        Console.Write(grid[i, j].IntResult == 1 ? "# " : ". ");
            //    }
            //    Console.WriteLine();
            //}
            //Console.WriteLine();

            //ResetRunStatus();
            //MappingA_SliderClear();
        }
        // 监听键盘输入
        void MappiingA_MoveSlider(JetMoveType eMoveType)
        {
            ////while (true)
            //{
            //    var key = eMoveType;

            //    if (key == JetMoveType.MOVE_UP && sliderX >= SliderRows)
            //    {
            //        sliderX -= SliderRows; // 上移
            //        MappingA_UpdateGrid();
            //    }
            //    else if (key == JetMoveType.MOVE_DOWN && sliderX <= GridRows - SliderRows)
            //    {
            //        sliderX += SliderRows; // 下移
            //        MappingA_UpdateGrid();
            //    }
            //    else if (key == JetMoveType.MOVE_LEFT && sliderY >= SliderCols)
            //    {
            //        sliderY -= SliderCols; // 左移
            //        MappingA_UpdateGrid();
            //    }
            //    else if (key == JetMoveType.MOVE_RIGHT && sliderY <= GridCols - SliderCols)
            //    {
            //        sliderY += SliderCols; // 右移
            //        MappingA_UpdateGrid();
            //    }
            //    //else if (key == ConsoleKey.Q) // 按Q退出
            //    //{
            //    //    break;
            //    //}
            //}
        }
        Color myCheckAnalyzeResult(AnalyzeClass eanalyze, WorkStatusCollectionClass runstatuscollection, out PassInfoClass passInfo)
        {
            Color c = Color.Green;
            int i = 0;

            passInfo = new PassInfoClass();

            if (runstatuscollection.NGCOUNT == 0)
                return c;

            bool bfind = false;
            //先找偏移的错误
            i = 0;
            while (i < runstatuscollection.NGCOUNT)
            {
                if (eanalyze.PassInfo.ToString() == runstatuscollection.GetNGRunStatus(i).PassInfo.ToString()
                    &&
                    runstatuscollection.GetNGRunStatus(i).AnalyzeProcedure == AnanlyzeProcedureEnum.BIAS
                    )
                {
                    bfind = true;
                    break;
                }
                i++;
            }
            if (!bfind)
            {
                i = 0;
                while (i < runstatuscollection.NGCOUNT)
                {
                    if (eanalyze.PassInfo.ToString() == runstatuscollection.GetNGRunStatus(i).PassInfo.ToString())
                    {
                        bfind = true;
                        break;
                    }
                    i++;
                }
            }

            if (bfind)
            {
                c = myAnalyzeProcedure(runstatuscollection.GetNGRunStatus(i).AnalyzeProcedure);
                //passInfo.FromPassInfo(runstatuscollection.GetNGRunStatus(i).PassInfo, OPLevelEnum.COPY);
                passInfo = new PassInfoClass(runstatuscollection.GetNGRunStatus(i).PassInfo, OPLevelEnum.COPY);

                if (runstatuscollection.GetNGRunStatus(i).AnalyzeProcedure == AnanlyzeProcedureEnum.BIAS)
                {
                    passInfo.OperateString = runstatuscollection.GetNGRunStatus(i).ProcessString;
                }

                return c;
            }

            i = 0;
            bfind = false;

            foreach (AnalyzeClass analyze in eanalyze.BranchList)
            {
                i = 0;
                while (i < runstatuscollection.NGCOUNT)
                {
                    if (analyze.PassInfo.ToString() == runstatuscollection.GetNGRunStatus(i).PassInfo.ToString()
                        &&
                        runstatuscollection.GetNGRunStatus(i).AnalyzeProcedure == AnanlyzeProcedureEnum.BIAS
                        )
                    {
                        bfind = true;
                        break;
                    }
                    i++;
                }

                if (!bfind)
                {
                    i = 0;
                    while (i < runstatuscollection.NGCOUNT)
                    {
                        if (analyze.PassInfo.ToString() == runstatuscollection.GetNGRunStatus(i).PassInfo.ToString())
                        {
                            bfind = true;
                            break;
                        }
                        i++;
                    }
                }

                if (bfind)
                {
                    c = myAnalyzeProcedure(runstatuscollection.GetNGRunStatus(i).AnalyzeProcedure);

                    //passInfo.FromPassInfo(runstatuscollection.GetNGRunStatus(i).PassInfo, OPLevelEnum.COPY);
                    passInfo = new PassInfoClass(runstatuscollection.GetNGRunStatus(i).PassInfo, OPLevelEnum.COPY);

                    if (runstatuscollection.GetNGRunStatus(i).AnalyzeProcedure == AnanlyzeProcedureEnum.BIAS)
                    {
                        passInfo.OperateString = runstatuscollection.GetNGRunStatus(i).ProcessString;
                    }

                    break;
                }

                c = myCheckAnalyzeResult(analyze, runstatuscollection, out passInfo);

            }

            return c;
        }

        Color myAnalyzeProcedure(AnanlyzeProcedureEnum ananlyzeProcedure)
        {
            Color c = Color.Red;

            switch (ananlyzeProcedure)
            {
                case AnanlyzeProcedureEnum.LASER:
                case AnanlyzeProcedureEnum.MONTH:
                case AnanlyzeProcedureEnum.YEAR:
                case AnanlyzeProcedureEnum.ALIGNRUN:
                    c = Color.Cyan;
                    break;
                case AnanlyzeProcedureEnum.INSPECTION:
                    c = Color.Red;
                    break;
                case AnanlyzeProcedureEnum.BIAS:
                    c = Color.Violet;
                    break;
                case AnanlyzeProcedureEnum.CHECKDIRT:
                    c = Color.Yellow;
                    break;
                case AnanlyzeProcedureEnum.CHECKBARCODE:
                    c = Color.Fuchsia;
                    break;
                case AnanlyzeProcedureEnum.CHECKMISBARCODE:
                    c = Color.Orange;
                    break;
                case AnanlyzeProcedureEnum.CHECKREPEATBARCODE:
                    c = Color.LightPink;
                    break;
                default:
                    break;
            }

            return c;
        }
        int _getColorIndex(Color eColor)
        {
            int iret = 0;
            if (eColor == Color.Cyan)
            {
                iret = 1;
            }
            else if (eColor == Color.Violet)
            {
                iret = 2;
            }
            else if (eColor == Color.Yellow)
            {
                iret = 3;
            }
            else if (eColor == Color.Red)
            {
                iret = 4;
            }
            else if (eColor == Color.Purple)
            {
                iret = 5;
            }
            else if (eColor == Color.Blue)
            {
                iret = 6;
            }
            else if (eColor == Color.Orange)
            {
                iret = 7;
            }
            else if (eColor == Color.Fuchsia)
            {
                iret = 8;
            }
            else if (eColor == Color.LightPink)
            {
                iret = 9;
            }
            return iret;
        }
        string _getAnalyzeBarcodeStr(AnalyzeClass eAnalyze)
        {
            if (eAnalyze.OCRPara.OCRMethod == OCRMethodEnum.DATAMATRIX || eAnalyze.OCRPara.OCRMethod == OCRMethodEnum.QRCODE)
            {
                string tempstr = $"No Compare;{eAnalyze.ReadBarcode2DRealStr}";
                if (INI.IsCheckBarcodeOpen)
                {
                    if (string.IsNullOrEmpty(eAnalyze.ReadBarcode2DRealStr))
                        tempstr = $"Compare [FAIL];Marking 2D[{eAnalyze.Barcode_2D}];Read 2D[{eAnalyze.ReadBarcode2DRealStr}]";
                    else
                        tempstr = $"Compare [{(eAnalyze.ReadBarcode2DRealStr == eAnalyze.Barcode_2D ? "PASS" : "FAIL")}];Marking 2D[{eAnalyze.Barcode_2D}];Read 2D[{eAnalyze.ReadBarcode2DRealStr}]";
                }
                return tempstr;
                //return eAnalyze.ReadBarcode2DStr;
            }
            else if (eAnalyze.OCRPara.OCRMethod == OCRMethodEnum.DATAMATRIXGRADE)
            {
                string tempstr = $"No Compare;{eAnalyze.ReadBarcode2DRealStr};{eAnalyze.ReadBarcode2DGrade}";
                if (INI.IsCheckBarcodeOpen)
                {
                    if (INI.IsOpenShowGrade)
                    {
                        if (string.IsNullOrEmpty(eAnalyze.ReadBarcode2DRealStr))
                            tempstr = $"Compare [FAIL];Marking 2D[{eAnalyze.Barcode_2D}];Read 2D[{eAnalyze.ReadBarcode2DRealStr}];Grade[{eAnalyze.ReadBarcode2DGrade}]";
                        else
                            tempstr = $"Compare [{(eAnalyze.ReadBarcode2DRealStr == eAnalyze.Barcode_2D ? "PASS" : "FAIL")}];Marking 2D[{eAnalyze.Barcode_2D}];Read 2D[{eAnalyze.ReadBarcode2DRealStr}];Grade[{eAnalyze.ReadBarcode2DGrade}]";

                    }
                    else
                    {
                        if (string.IsNullOrEmpty(eAnalyze.ReadBarcode2DRealStr))
                            tempstr = $"Compare [FAIL];Marking 2D[{eAnalyze.Barcode_2D}];Read 2D[{eAnalyze.ReadBarcode2DRealStr}]";
                        else
                            tempstr = $"Compare [{(eAnalyze.ReadBarcode2DRealStr == eAnalyze.Barcode_2D ? "PASS" : "FAIL")}];Marking 2D[{eAnalyze.Barcode_2D}];Read 2D[{eAnalyze.ReadBarcode2DRealStr}]";

                    }

                }
                return tempstr;
                //return eAnalyze.ReadBarcode2DStr + ";" + eAnalyze.ReadBarcode2DGrade;
            }
            foreach (AnalyzeClass analyzeClass in eAnalyze.BranchList)
            {
                string _barcodeStr = _getAnalyzeBarcodeStr(analyzeClass);
                if (!string.IsNullOrEmpty(_barcodeStr))
                    return _barcodeStr;
            }
            return string.Empty;
        }
        string _rectToString(RectangleF eRectF)
        {
            string STR = string.Empty;

            STR += eRectF.Location.X + ",";
            STR += eRectF.Location.Y + ",";
            STR += eRectF.Width + ",";
            STR += eRectF.Height;

            return STR;
        }
        #endregion

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
