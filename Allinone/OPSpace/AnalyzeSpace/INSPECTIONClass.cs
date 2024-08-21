using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using JetEazy;
using AUVision;
using JetEazy.BasicSpace;
using System.IO;

using AForge.Imaging.Filters;

#if CollectGIF
using AnimatedGif;
#endif

namespace Allinone.OPSpace.AnalyzeSpace
{
    public class INSPECTIONClass
    {
        public InspectionMethodEnum InspectionMethod = InspectionMethodEnum.NONE;
        public Inspection_A_B_Enum InspectionAB = Inspection_A_B_Enum.AB;
        public int IBCount = 3; //點數
        public int IBArea = 5;  //面積
        public int IBTolerance = 20;  //差異容許值

        float Value = 0;

        AUGrayImg8 imgpattern = new AUGrayImg8();
        AUGrayImg8 imgmask = new AUGrayImg8();

        AUGrayImg8 imginput;
        AUGrayImg8 imgoutput;

        public Bitmap bmpPattern;
        Bitmap bmpInput;  //= new Bitmap(1, 1);
        public Bitmap bmpMask;
        
        #region Online Data

        string RelateAnalyzeString = "";
        //string RelateAnalyzeInformation = "";
        PassInfoClass PassInfo = new PassInfoClass();

        JzFindObjectClass JzFind = new JzFindObjectClass();

        public WorkStatusCollectionClass TrainStatusCollection = new WorkStatusCollectionClass();
        public WorkStatusCollectionClass RunStatusCollection = new WorkStatusCollectionClass();


        public bool IsPass = true;

        #endregion

        public INSPECTIONClass()
        {
            //InspectionMethod = InspectionMethodEnum.NONE;
            //IBCount = 0;
            //IBArea = 5;
            //IBTolerance = 20;

        }
        public INSPECTIONClass(string str)
        {
            FromString(str);
        }
        public override string ToString()
        {
            string str = "";

            str += ((int)InspectionMethod).ToString() + Universal.SeperateCharB;    //0
            str += IBCount.ToString() + Universal.SeperateCharB;                    //1
            str += IBArea.ToString() + Universal.SeperateCharB;                     //2
            str += IBTolerance.ToString() + Universal.SeperateCharB;                //3
            str += ((int) InspectionAB).ToString() + Universal.SeperateCharB;
            str += "";

            return str;
        }
        public void FromString(string str)
        {
            string[] strs = str.Split(Universal.SeperateCharB);

            InspectionMethod = (InspectionMethodEnum)int.Parse(strs[0]);
            IBCount = int.Parse(strs[1]);
            IBArea = int.Parse(strs[2]);
            IBTolerance = int.Parse(strs[3]);

            if (strs.Length > 3)
            {
                if(strs[4]=="")
                InspectionAB = (Inspection_A_B_Enum)int.Parse("0");
                else
                    InspectionAB = (Inspection_A_B_Enum)int.Parse(strs[4] );
            }
        }
        public void Reset()
        {
            InspectionMethod = InspectionMethodEnum.NONE;
            InspectionAB =  Inspection_A_B_Enum.AB;
            IBCount = 3;
            IBArea = 5;
            IBTolerance = 20;
        }

        public void FromPropertyChange(string changeitemstring, string valuestring)
        {
            string[] str = changeitemstring.Split(';');

            if (str[0] != "03.Inspection")
                return;

            switch (str[1])
            {
                case "InspectionMethod":
                    InspectionMethod = (InspectionMethodEnum)Enum.Parse(typeof(InspectionMethodEnum), valuestring, true);
                    break;
                case "IBCount":
                    IBCount = int.Parse(valuestring);
                    break;
                case "IBArea":
                    IBArea = int.Parse(valuestring);
                    break;
                case "IBTolerance":
                    IBTolerance = int.Parse(valuestring);
                    break;
                case "Inspection_A_B_Method":
                    InspectionAB = (Inspection_A_B_Enum)Enum.Parse(typeof(Inspection_A_B_Enum), valuestring, true);
                    break;
            }
        }

        public void IsSeed_GetInspectionRequirement(Bitmap bmppattern, Bitmap bmpmask)
        {
            //RelateAnalyzeString = relateanalyzestring;
            ////RelateAnalyzeInformation = relateanalyzeinformation;
            //PassInfo = new PassInfoClass(passinfo, OPLevelEnum.COPY);

            switch (InspectionMethod)
            {
                case InspectionMethodEnum.NONE:

                    break;
                default:
                    AUUtility.DrawBitmapToAUGrayImg8(bmppattern, ref imgpattern);
                    AUUtility.DrawBitmapToAUGrayImg8(bmpmask, ref imgmask);

                    //bmppattern.Save(Universal.TESTPATH + "\\ANALYZETEST\\IBPATTERN " + RelateAnalyzeString + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                    //bmpmask.Save(Universal.TESTPATH + "\\ANALYZETEST\\IBMASK " + RelateAnalyzeString + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);


                    //Reduce By Victor  2018/02/11
                    //bmpPattern.Dispose();
                    bmpPattern = new Bitmap(bmppattern);
                    bmpMask = new Bitmap(bmpmask);
                    // bmpPattern = bmppattern;

                    HistogramClass histog = new HistogramClass(1);
                    histog.GetHistogram(bmpPattern);

                    int max = histog.GetMaxRatioAVG(0.25f);
                    int min = histog.GetMinRatioAVG(0.25f);

                    Value = (max - min) * IBTolerance / 100f;

                    break;
            }
        }
        public void I01_GetInspectionRequirement(Bitmap bmppattern,Bitmap bmpmask, string relateanalyzestring,PassInfoClass passinfo)
        {
            RelateAnalyzeString = relateanalyzestring;
            //RelateAnalyzeInformation = relateanalyzeinformation;
            PassInfo = new PassInfoClass(passinfo, OPLevelEnum.COPY);

            switch(InspectionMethod)
            {
                case InspectionMethodEnum.NONE:

                    break;
                default:
                    AUUtility.DrawBitmapToAUGrayImg8(bmppattern, ref imgpattern);
                    AUUtility.DrawBitmapToAUGrayImg8(bmpmask, ref imgmask);

                    //bmppattern.Save(Universal.TESTPATH + "\\ANALYZETEST\\IBPATTERN " + RelateAnalyzeString + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                    //bmpmask.Save(Universal.TESTPATH + "\\ANALYZETEST\\IBMASK " + RelateAnalyzeString + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);


                    //Reduce By Victor  2018/02/11
                    //bmpPattern.Dispose();
                    bmpPattern = new Bitmap(bmppattern);
                    bmpMask = new Bitmap(bmpmask);
                    // bmpPattern = bmppattern;

                    HistogramClass histog = new HistogramClass(1);
                    histog.GetHistogram(bmpPattern);

                    int max = histog.GetMaxRatioAVG(0.25f);
                    int min = histog.GetMinRatioAVG(0.25f);

                    Value = (max - min) * IBTolerance/ 100f;

                    break;
            }
        }
        public bool I08_InspectionProcess(Bitmap bmpinput, ref Bitmap bmpoutput)
        {
            if (InspectionMethod == InspectionMethodEnum.NONE || InspectionMethod == InspectionMethodEnum.BAR_CHECK)
            {
                IsPass = true;
                return true;
            }

            bool isgood = true;

            WorkStatusClass runstatus = new WorkStatusClass(AnanlyzeProcedureEnum.INSPECTION);
            string processstring = "Start " + RelateAnalyzeString + " Inspection." + Environment.NewLine;
            string errorstring = "";
            ReasonEnum reason = ReasonEnum.PASS;


            //Recude By Victor 2018/02/12
            //bmpInput.Dispose();
            //bmpInput = new Bitmap(bmpinput);
            bmpInput = bmpinput;

            //imginput.Dispose();
            imginput = new AUGrayImg8();
            AUUtility.DrawBitmapToAUGrayImg8(bmpinput, ref imginput);

            //imgoutput.Dispose();
            imgoutput = new AUGrayImg8();
            imgoutput.SetResolution(imgpattern.GetWidth(), imgpattern.GetHeight());

            //if (RelateAnalyzeString == "A04-02-0022")
            //{
            //    imgpattern.Save("D:\\testtest\\pattern.bmp", eImageFormat.eImageFormat_BMP);
            //    imginput.Save("D:\\testtest\\inginput.png", eImageFormat.eImageFormat_PNG);
            //    imgmask.Save("D:\\testtest\\mask.bmp", eImageFormat.eImageFormat_BMP);

            //    //imgpattern.Save("D:\\pattern.png", eImageFormat.eImageFormat_PNG);
            //    //imginput.Save("D:\\Input.png", eImageFormat.eImageFormat_PNG);
            //    //AUImage.HistogramEqualize(imgpattern, imginput);
            //    //imginput.Save("D:\\Input2.png", eImageFormat.eImageFormat_PNG);
            //    //imgpattern.Save("D:\\pattern2.png", eImageFormat.eImageFormat_PNG);
            //}
            if (InspectionMethod == InspectionMethodEnum.Equalize)
                AUImage.IntensityTransfer(imginput, imgpattern, imginput);
            else if (InspectionMethod == InspectionMethodEnum.HISTO)
                Inspection_JE01.IntensityEqualize_JE01(imginput, imgpattern, imginput, 0.25d);

            //if (RelateAnalyzeString == "A00-02-0002")
            //{
            //    imginput.Save("D:\\testtest\\inginput2.png", eImageFormat.eImageFormat_PNG);
            //    imgpattern.Save("D:\\testtest\\imgpattern.png", eImageFormat.eImageFormat_PNG);
            //    imginput.Save("D:\\testtest\\imginput.png", eImageFormat.eImageFormat_PNG);
            //    bmpMask.Save("D:\\testtest\\mask.png", ImageFormat.Png);
            //}

            switch (InspectionAB)
            {
                case Inspection_A_B_Enum.Histogram:

                    Bitmap bmpRun2 = new Bitmap(1, 1);
                    AUUtility.DrawAUGrayImg8ToBitmap(imginput, ref bmpRun2);
                    bmpRun2 = new Bitmap(bmpRun2);
                    int iArea2 = 0;
                    myImageProcessor.SetBimap_A_BFormat32(bmpRun2, bmpPattern, bmpMask, out bmpoutput, Value, ref iArea2);

                    //          bmpoutput.Save("D:\\bmpout.png");
                    bmpRun2.Dispose();
                    break;

                case Inspection_A_B_Enum.AB:

                    Inspection_JE01.Inspection(imgpattern, imginput, imgmask, imgoutput, IBTolerance, false, eInspectionType.eInspectionType_Positive);
                    AUUtility.DrawAUGrayImg8ToBitmap(imgoutput, ref bmpoutput);

                    break;
                case Inspection_A_B_Enum.ABPlus:
                    Bitmap bmpRun = new Bitmap(1, 1);
                    AUUtility.DrawAUGrayImg8ToBitmap(imginput, ref bmpRun);
                    bmpRun = new Bitmap(bmpRun);

                    Bitmap PrainMask = new Bitmap(1, 1);
                    myImageProcessor.Balance(bmpPattern, ref PrainMask, myImageProcessor.EnumThreshold.Minimum);
                    PrainMask = new Bitmap(PrainMask);
                    if (PrainMask == null)
                        PrainMask = new Bitmap(bmpInput.Width, bmpInput.Height);

                    Bitmap RunMask = new Bitmap(1, 1);
                    myImageProcessor.Balance(bmpRun, ref RunMask, myImageProcessor.EnumThreshold.IsoData);
                    RunMask = new Bitmap(RunMask);

                    if (RunMask == null)
                        RunMask = new Bitmap(bmpInput.Width, bmpInput.Height);


                    int iArea = 0;
                    myImageProcessor.SetBimap_A_BFormat32(bmpRun, bmpPattern, RunMask, PrainMask, bmpMask, out bmpoutput, IBTolerance, ref iArea);

                    //bmpInput.Save("D:\\testtest\\test\\bmpout.png");
                    //bmpPattern.Save("D:\\testtest\\test\\bmpPattern.png");
                    //PrainMask.Save("D:\\testtest\\test\\PrainMask.png");
                    //RunMask.Save("D:\\testtest\\test\\RunMask.png");
                    ////PrainMask2.Save("D:\\testtest\\test\\PrainMask2.png");
                    ////RunMask2.Save("D:\\testtest\\test\\RunMask2.png");
                    //bmpMask.Save("D:\\testtest\\test\\bmpMask.png");
                    //bmpoutput.Save("D:\\testtest\\test\\bmpoutput.png");

                    PrainMask.Dispose();
                    RunMask.Dispose();
                    bmpRun.Dispose();

                    break;
            }

            //if (RelateAnalyzeString == "A00-02-0002")
            //{
            //    bmpoutput.Save("D:\\testtest\\bmpoutput2.png", ImageFormat.Png);
            //}

            Graphics gg = Graphics.FromImage(bmpoutput);
            gg.DrawRectangle(new Pen(new SolidBrush(Color.Black), 3), new Rectangle(1, 1, bmpoutput.Width - 3, bmpoutput.Height - 3));
            gg.Dispose();

            // 3 是會被過濾掉的值，要注意調整
            JzFind.Find(bmpoutput, SimpleRect(bmpoutput.Size, 1), Color.Red, 3);
            //if (RelateAnalyzeString == "A00-02-0002")
            //{
            //    bmpoutput.Save("D:\\testtest\\bmpoutput.png", ImageFormat.Png);
            //}
            //int GetMaxArea = JzFind.GetMaxArea(10, 10);
            int GetMaxArea = JzFind.GetMaxArea();
            int getOverAreaCount = JzFind.Count;
            switch (Universal.OPTION)
            {
                case OptionEnum.MAIN_SDM3:
                    getOverAreaCount = JzFind.IsCheckOverAreaCount(IBArea);
                    break;
            }
            if (GetMaxArea > IBArea)
            {
                isgood = false;

                processstring += "Error in " + RelateAnalyzeString + " Inspection Max Area " + GetMaxArea.ToString() + " > " + IBArea.ToString() + Environment.NewLine;
                errorstring += RelateAnalyzeString + " Inspection Max Area " + GetMaxArea.ToString() + " > " + IBArea.ToString() + Environment.NewLine;
                //   bmpoutput.Save("D:\\test.png");
                reason = ReasonEnum.NG;


                //bmpPattern.Save("D:\\LOA\\PPFUCK01.BMP");
                //bmpInput.Save("D:\\LOA\\PPFUCK02.BMP");


                //foreach (FoundClass found in JzFind.FoundList)
                //{
                //    if(found.Area > IBArea)
                //        DrawRect(bmpoutput, found.rect, new Pen(Color.Lime, 10), 10);
                //}
            }
            else if (getOverAreaCount > IBCount && IBCount != -1)
            //else if (JzFind.Count > IBCount && IBCount != -1)
            {
                isgood = false;

                processstring += "Error in " + RelateAnalyzeString + " Inspection Count " + JzFind.Count.ToString() + " > " + IBCount.ToString() + Environment.NewLine;
                errorstring += RelateAnalyzeString + " Inspection Count " + getOverAreaCount.ToString() + " > " + IBCount.ToString() + Environment.NewLine;
                //errorstring += RelateAnalyzeString + " Inspection Count " + JzFind.Count.ToString() + " > " + IBCount.ToString() + Environment.NewLine;

                reason = ReasonEnum.NG;

                //foreach (FoundClass found in JzFind.FoundList)
                //{
                //    DrawRect(bmpoutput, found.rect, new Pen(Color.Lime, 10), 10);
                //}
            }
            else
            {
                processstring = "Inspection Max Area " + GetMaxArea.ToString() + " < " + IBArea.ToString()
                    + ", Count " + getOverAreaCount.ToString() + " < " + IBCount.ToString() + Environment.NewLine;

                //processstring = "Inspection Max Area " + GetMaxArea.ToString() + " < " + IBArea.ToString()
                //   + ", Count " + JzFind.Count.ToString() + " < " + IBCount.ToString() + Environment.NewLine;
            }
            //Universal.IsMultiThread = true;
            if (!isgood && PassInfo.OperatePath.IndexOf("80001") > -1)
            {
                //string strPath = Universal.TESTPATH + "\\ANALYZETEST\\Inspection\\" + RelateAnalyzeString + "\\";
                //if (!System.IO.Directory.Exists(strPath))
                //    System.IO.Directory.CreateDirectory(strPath);
                ////RelateAnalyzeString = RelateAnalyzeString;
                //bmpInput.Save(strPath + "Input"+ Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                //bmpoutput.Save(strPath + "Output"+ Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                //imgpattern.Save(strPath + "Pattern" + Universal.GlobalImageTypeString, eImageFormat.eImageFormat_PNG);
                //this.bmpMask.Save(strPath + "Mask" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            }

            if (!isgood)
            {
                if (INI.IsCollectErrorSmall)
                {
                    if (!System.IO.Directory.Exists(Universal.MainX6_Path))
                        System.IO.Directory.CreateDirectory(Universal.MainX6_Path);

                    bmpInput.Save(Universal.MainX6_Path + "\\" + RelateAnalyzeString + "_Input" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                    bmpoutput.Save(Universal.MainX6_Path + "\\" + RelateAnalyzeString + "_Output" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                    bmpPattern.Save(Universal.MainX6_Path + "\\" + RelateAnalyzeString + "_Pattern" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

#if CollectGIF
                    if (!System.IO.Directory.Exists(Universal.MainX6_Path + "\\gif"))
                        System.IO.Directory.CreateDirectory(Universal.MainX6_Path + "\\gif");

                    //GIF CODE
                    Task task = new Task(() =>
                    {

                        using (var gif = AnimatedGif.AnimatedGif.Create(Universal.MainX6_Path + "\\gif" + "\\" + RelateAnalyzeString + "_Result.gif", 2))
                        {
                            Bitmap bmptmp = new Bitmap(Universal.MainX6_Path + "\\" + RelateAnalyzeString + "_Pattern" + Universal.GlobalImageTypeString);
                            Bitmap img = new Bitmap(bmptmp);
                            bmptmp.Dispose();

                        //var img1 = Image.FromFile(img);
                        gif.AddFrame(img, 1000, quality: GifQuality.Grayscale);
                        //var img2 = Image.FromFile("img2.png");

                        bmptmp = new Bitmap(Universal.MainX6_Path + "\\" + RelateAnalyzeString + "_Input" + Universal.GlobalImageTypeString);
                            img = new Bitmap(bmptmp);
                            bmptmp.Dispose();
                            gif.AddFrame(img, 1000, quality: GifQuality.Grayscale);
                        //var img3 = Image.FromFile("img3.png");
                        //gif.AddFrame(img3, delay: -1, quality: GifQuality.Bit8);
                    }


                    //using (System.IO.FileStream fs = new FileStream(Universal.MainX6_Path + "\\gif" + "\\" + RelateAnalyzeString + "_Result.gif", FileMode.Create))
                    //using (var encoder = new GifEncoder(fs))
                    //{
                    //    Bitmap img = new Bitmap(1, 1);
                    //    Bitmap imgtemp = new Bitmap(Universal.MainX6_Path + "\\" + RelateAnalyzeString + "_Pattern" + Universal.GlobalImageTypeString);
                    //    img = new Bitmap(imgtemp);
                    //    imgtemp.Dispose();

                    //    TimeSpan ts = new TimeSpan(0, 0, 1);

                    //    encoder.AddFrame(img, 0, 0, ts);
                    //    System.Threading.Thread.Sleep(200);

                    //    imgtemp = new Bitmap(Universal.MainX6_Path + "\\" + RelateAnalyzeString + "_Input" + Universal.GlobalImageTypeString);
                    //    img = new Bitmap(imgtemp);
                    //    imgtemp.Dispose();

                    //    encoder.AddFrame(img,0,0, ts);
                    //}
                });
                    task.Start();
#endif

                }
            }


            imginput.Dispose();
            imgoutput.Dispose();

            runstatus.SetWorkStatus(bmpPattern, bmpInput, bmpoutput, reason, errorstring, processstring, PassInfo);

            RunStatusCollection.Add(runstatus);

            IsPass = isgood;

            return isgood;
        }
        public void Suicide()
        {
            if (imgpattern != null)
                imgpattern.Dispose();

            if (imgmask != null)
                imgmask.Dispose();

            if(imginput !=null)
                imginput.Dispose();

            if (imgoutput != null)
                imgoutput.Dispose();

            if (bmpInput != null)
                bmpInput.Dispose();

            if (bmpPattern != null)
                bmpPattern.Dispose();

            TrainStatusCollection.Clear();
            RunStatusCollection.Clear();
        }

        public void ResetTrainStatus()
        {
            TrainStatusCollection.Clear();
        }
        public void ResetRunStatus()
        {
            RunStatusCollection.Clear();
        }

        public void AddTrainLogString(string logstr)
        {
            foreach (WorkStatusClass works in TrainStatusCollection.WorkStatusList)
            {
                if (works.LogString.IndexOf(logstr) < 0)
                {
                    works.LogString += logstr;
                }
            }
        }
        public void AddRunLogString(string logstr)
        {
            foreach (WorkStatusClass works in RunStatusCollection.WorkStatusList)
            {
                if (works.LogString.IndexOf(logstr) < 0)
                {
                    works.LogString += logstr;
                }
            }
        }

        /// <summary>
        /// 將產生出來的過程寫出去
        /// </summary>
        /// <param name="runstatuscollection"></param>
        public void FillRunStatus(WorkStatusCollectionClass runstatuscollection)
        {
            foreach (WorkStatusClass runstatus in RunStatusCollection.WorkStatusList)
            {
                runstatuscollection.Add(runstatus);
            }
        }
        public void FillRunStatus(WorkStatusCollectionClass runstatuscollection, string filltoanalyzestr)
        {
            foreach (WorkStatusClass runstatus in RunStatusCollection.WorkStatusList)
            {
                if (runstatus.LogString.IndexOf(filltoanalyzestr) < 0)
                {
                    runstatus.LogString += filltoanalyzestr;
                    runstatuscollection.Add(runstatus);
                }
            }
        }
        /// <summary>
        /// 將產生出來的過程寫出去
        /// </summary>
        /// <param name="runstatuscollection"></param>
        public void FillTrainStatus(WorkStatusCollectionClass trainstatuscollection)
        {
            foreach (WorkStatusClass trainstatus in TrainStatusCollection.WorkStatusList)
            {
                trainstatuscollection.Add(trainstatus);
            }
        }
        public void FillTrainStatus(WorkStatusCollectionClass trainstatuscollection,string filltoanalyzestr)
        {
            foreach (WorkStatusClass trainstatus in TrainStatusCollection.WorkStatusList)
            {
                if (trainstatus.LogString.IndexOf(filltoanalyzestr) < 0)
                {
                    trainstatus.LogString += filltoanalyzestr;
                    trainstatuscollection.Add(trainstatus);
                }
            }
        }
        #region Tool Operation
        Rectangle SimpleRect(Size sz, int devide)
        {
            return new Rectangle(0, 0, sz.Width / devide, sz.Height / devide);
        }

        void DrawRect(Bitmap BMP, Rectangle Rect, Pen RoundPen, int Enlarge)
        {
            DrawRect(BMP, new Rectangle(Rect.X - Enlarge, Rect.Y - Enlarge, ((int)Rect.Width) + (Enlarge << 1), ((int)Rect.Height) + (Enlarge << 1)), RoundPen);
        }
        void DrawRect(Bitmap BMP, Rectangle Rect, Pen RoundPen)
        {
            Graphics g = Graphics.FromImage(BMP);
            g.DrawRectangle(RoundPen, Rect);
            g.Dispose();
        }
        #endregion
    }
}
