using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using JetEazy;
using AUVision;
using JetEazy.BasicSpace;

namespace Allinone.OPSpace.AnalyzeSpace
{
    public class ALIGNClass
    {
        public AlignMethodEnum AlignMethod = AlignMethodEnum.NONE;   //採用哪類Align方案
        public float MTPSample = 100;         //Pyramid Size
        public bool MTCannyAuto = true;     //Canny Auto Use
        public int MTCannyH = 200;          //Canny H. Threshold
        public int MTCannyL = 128;          //Canny L. Threshold
        public float MTRotation = 20f;      //Rotation From -20 to 20 degree
        public float MTScaling = 0f;        //Scaling From  90%-110%

        public int MTMaxOcc = 1;            //Max Occ
        public float MTTolerance = 0.1f;    //Tolerance from 0.1 to 1.0
        public bool MTIsSubPixel = false;   //Subpixel(NO Use Now)

        public float MTOffset = 0f;         //Check Offset Value
        public float MTResolution = 0.038f;  //Resolution Value
        public AlignModeEnum AlignMode = AlignModeEnum.AREA;    //Use Area or Borader Symptom for this analyze
        
        xTrainingInfoF AUTrainInfoF = new xTrainingInfoF();
        AUFind AUFIND = new AUFind();
        xTrainingInfo xInfo = new xTrainingInfo();
        AUMatch AUMATCH = new AUMatch();

        public AbsoluteAlignEnum AbsAlignMode = AbsoluteAlignEnum.NONE;
        public float ABSOffset = 0f;         //Check Offset Value


        public float Offset = 0f;
        public float Rotation = 0f;
        public float Score = 0f;
        public float mySize = 4f;

        #region On Line Data

     public   Bitmap bmpPattern;
     public   Bitmap bmpMask;
        public Bitmap bmpContour;

        Bitmap bmpRunInput = new Bitmap(1, 1);
        Bitmap bmpRunOutput = new Bitmap(1, 1);

        //public List<string> myProcessStringList = new List<string>();
        //public List<RunStatusClass> RunStatusList = new List<RunStatusClass>();


        public PointF OrgCenter = new PointF();
        public PointF RunCenter = new PointF();
        public PointF AlignOffset = new PointF();
        public float AlignDegree = 0f;

        public WorkStatusCollectionClass TrainStatusCollection = new WorkStatusCollectionClass();
        public WorkStatusCollectionClass RunStatusCollection = new WorkStatusCollectionClass();

        public string RelateAnalyzeString = "";
        //public string RelateAnalyzeInformation = "";
        public bool CheckGood = true;

        public PassInfoClass PassInfo = new PassInfoClass();

        public bool IsTempSave = false;

        #endregion

        public ALIGNClass()
        {
            //AlignMethod = AlignMethodEnum.NONE;
            //MTPSize = 35;
            //MTCannyH = 200;
            //MTCannyL = 128;
            //MTRotation = 20f;
            //MTScaling = 0f;
            //MTMaxOcc = 1;
            //MTTolerance = 0.7f;
            //MTIsSubPixel = false;
            //MTOffset = 0;
            //MTResolution = 0.01f;

        }
        public ALIGNClass(string str)
        {
            FromString(str);
        }
        public override string ToString()
        {
            string str = "";

            str += ((int)AlignMethod).ToString() + Universal.SeperateCharB; //0
            str += MTPSample.ToString() + Universal.SeperateCharB;            //1
            str += MTCannyH.ToString() + Universal.SeperateCharB;           //2
            str += MTCannyL.ToString() + Universal.SeperateCharB;           //3
            str += MTRotation.ToString() + Universal.SeperateCharB;         //4
            str += MTScaling.ToString() + Universal.SeperateCharB;          //5
            str += MTMaxOcc.ToString() + Universal.SeperateCharB;           //6
            str += MTTolerance.ToString() + Universal.SeperateCharB;        //7
            str += (MTIsSubPixel ? "1" : "0") + Universal.SeperateCharB;    //8
            str += MTOffset.ToString() + Universal.SeperateCharB;           //9
            str += MTResolution.ToString() + Universal.SeperateCharB;       //10
            str += ((int)AlignMode).ToString() + Universal.SeperateCharB;   //11
            str += (MTCannyAuto ? "1" : "0") + Universal.SeperateCharB;     //12
            str += ((int)AbsAlignMode).ToString() + Universal.SeperateCharB;   //13
            str += ABSOffset.ToString() + Universal.SeperateCharB;           //14
            str += "";

            return str;
        }
        public void FromString(string str)
        {
            string[] strs = str.Split(Universal.SeperateCharB);
            
            AlignMethod = (AlignMethodEnum)int.Parse(strs[0]);
            MTPSample = int.Parse(strs[1]);
            MTCannyH = int.Parse(strs[2]);
            MTCannyL = int.Parse(strs[3]);
            MTRotation = float.Parse(strs[4]);
            MTScaling = float.Parse(strs[5]);
            MTMaxOcc = int.Parse(strs[6]);
            MTTolerance = float.Parse(strs[7]);
            MTIsSubPixel = strs[8] == "1";
            MTOffset = float.Parse(strs[9]);
            MTResolution = float.Parse(strs[10]);

            if (strs.Length > 12)
            {
                AlignMode = (AlignModeEnum)int.Parse(strs[11]);
            }
            if (strs.Length > 13)
            {
                MTCannyAuto = int.Parse(strs[12]) == 1;
            }
            if (strs.Length > 15)
            {
                AbsAlignMode = (AbsoluteAlignEnum)int.Parse(strs[13]);
                ABSOffset = float.Parse(strs[14]);
            }
        }
        public void FromPropertyChange(string changeitemstring, string valuestring)
        {
            string[] str = changeitemstring.Split(';');

            if (str[0] != "02.Align")
                return;

           // float value = float.Parse(valuestring);

            switch (str[1])
            {
                case "AlignMethod":
                    AlignMethod = (AlignMethodEnum)Enum.Parse(typeof(AlignMethodEnum), valuestring, true);
                    break;
                case "MTPSample":
                    MTPSample = (int)float.Parse(valuestring);// int.Parse(valuestring);
                    break;
                case "MTCannyH":
                    MTCannyH = (int)float.Parse(valuestring);// int.Parse(valuestring);
                    break;
                case "MTCannyL":
                    MTCannyL = (int)float.Parse(valuestring);// int.Parse(valuestring);
                    break;
                case "MTRotation":
                    MTRotation = (int)float.Parse(valuestring);// int.Parse(valuestring);
                    break;
                case "MTScaling":
                    MTScaling = float.Parse(valuestring);// float.Parse(valuestring);
                    break;
                case "MTMaxOcc":
                    MTMaxOcc = (int)float.Parse(valuestring);// int.Parse(valuestring);
                    break;
                case "MTTolerance":
                    MTTolerance = float.Parse(valuestring);// float.Parse(valuestring);
                    break;
                case "MTOffset":
                    MTOffset = float.Parse(valuestring);// float.Parse(valuestring);
                    break;
                case "MTResolution":
                    MTResolution = float.Parse(valuestring);// float.Parse(valuestring);
                    break;
                case "AlignMode":
                    AlignMode = (AlignModeEnum)Enum.Parse(typeof(AlignModeEnum), valuestring, true);
                    break;
                case "MTCannyAuto":
                    MTCannyAuto = bool.Parse(valuestring);
                    break;

                case "ABSAlignMethod":
                    AbsAlignMode = (AbsoluteAlignEnum)Enum.Parse(typeof(AbsoluteAlignEnum), valuestring, true);
                    break;
                case "ABSOffset":
                    ABSOffset = float.Parse(valuestring);// float.Parse(valuestring);
                    break;
            }
        }
        public void Reset()
        {

            AlignMethod = AlignMethodEnum.NONE;   //採用哪類Align方案
            MTPSample = 100;              //Pyramid Size
            MTCannyAuto = true;     //Canny value for auto
            MTCannyH = 200;          //Canny H. Threshold
            MTCannyL = 128;          //Canny L. Threshold
            MTRotation = 20f;      //Rotation From -20 to 20 degree
            MTScaling = 0f;        //Scaling From  90%-110%

            MTMaxOcc = 1;            //Max Occ
            MTTolerance = 0.1f;    //Tolerance from 0.1 to 1.0
            MTIsSubPixel = false;   //Subpixel(NO Use Now)

            MTOffset = 0f;         //Check Offset Value
            MTResolution = 0.01f;  //Resolution Value
            AlignMode = AlignModeEnum.AREA;    //Use Area or Borader Symptom for this analyze


            AbsAlignMode = AbsoluteAlignEnum.NONE;
            ABSOffset = 0f;         //Check Offset Value
        }
        
        #region Application Operation
        public bool AlignTrainProcess(Bitmap bmpinput,ref Bitmap bmppattern,Bitmap bmpmask,
            int brightness,int contrast,string relateanalyzestr,PassInfoClass passinfo,bool isreservebmp)
        {
            string str = "";
            bool isgood = true;

            //保留 Pattern 和 Mask 的圖

            RelateAnalyzeString = relateanalyzestr;

            WorkStatusClass workstatus = new WorkStatusClass(AnanlyzeProcedureEnum.ALIGNTRAIN);
            string processstring = "Start " + RelateAnalyzeString + " Alignment Train." + Environment.NewLine;
            string errorstring = "";
            ReasonEnum reason = ReasonEnum.PASS;

            //Reduce By Victor 2018/02/11
            if (isreservebmp)
            {
                bmpPattern = new Bitmap(bmppattern);
            }
            else
            {
                bmpPattern = bmppattern;
            }


            bmpMask =new Bitmap( bmpmask);
            

            //RelateAnalyzeInformation = relateanalyzeinformation;
            PassInfo = new PassInfoClass(passinfo, OPLevelEnum.COPY);
            
            switch(AlignMethod)
            {
                case AlignMethodEnum.NONE:

                    str = relateanalyzestr + " Use Method None Align For Image Output";

                    processstring += str + Environment.NewLine;
                                        
                    break;
                case AlignMethodEnum.AUFIND:
               // case AlignMethodEnum.AUMATCH:
                    //改變亮度及對比

                    processstring += relateanalyzestr + " Set Brightness to " + brightness.ToString() + " and Contrast to " + contrast.ToString() + Environment.NewLine;
                    SetBrightContrast(bmpPattern, brightness, contrast);
                    str = relateanalyzestr + " Use Method AUFIND Align For Training";

                    processstring += str + Environment.NewLine;
                    isgood = AuFindTrain(bmpPattern, bmpmask, brightness, contrast);

                    if (isreservebmp)
                    {
                        bmppattern.Dispose();
                        bmppattern = new Bitmap(bmpPattern);
                    }
                    break;
                case AlignMethodEnum.AUMATCH:

                    //改變亮度及對比
                    processstring += relateanalyzestr + " Set Brightness to " + brightness.ToString() + " and Contrast to " + contrast.ToString() + Environment.NewLine;
                    SetBrightContrast(bmpPattern, brightness, contrast);
                    str = relateanalyzestr + " Use Method AUFIND Align For Training";
                    processstring += str + Environment.NewLine;

                    isgood = AuMatchTrain(bmpPattern, bmpmask, brightness, contrast);

                    if (isreservebmp)
                    {
                        bmppattern.Dispose();
                        bmppattern = new Bitmap(bmpPattern);
                    }
                    break;
            }

            if (isgood)
            {
                str = relateanalyzestr + " Pattern Train Successful.";
                processstring += str + Environment.NewLine;

                reason = ReasonEnum.PASS;
            }
            else
            {
                str = relateanalyzestr + " Pattern Train Fail.";
                processstring += str + Environment.NewLine;
                reason = ReasonEnum.NG;
            }



            workstatus.SetWorkStatus(bmpPattern, bmpmask, bmpPattern, reason, errorstring, processstring, PassInfo);

            TrainStatusCollection.Add(workstatus);

            return isgood;
        }
        /// <summary>
        /// 訓練圖像
        /// </summary>
        /// <param name="bmpinput"></param>
        /// <param name="bmppattern"></param>
        /// <param name="bmpmask"></param>
        /// <param name="bmpoutput"></param>
        /// <param name="brightness"></param>
        /// <param name="contrast"></param>
        /// <returns></returns>
        bool AuFindTrain(Bitmap bmppattern,Bitmap bmpmask, int brightness, int contrast)
        {
            const int downsampleratiodefault = 88;

            bool isgood = true;

            AUGrayImg8 imgpattern = new AUGrayImg8();
            AUGrayImg8 imgmask = new AUGrayImg8();
            AUColorImg24 imgResult = new AUColorImg24();

            JzTimes mytime = new JzTimes();
            int ms = 0;
            string str = "";

            mytime.Cut();

            if (MTPSample == -1)
            {
                bmppattern = new Bitmap(bmppattern, new Size((int)(bmppattern.Width / mySize), (int)(bmppattern.Height / mySize)));
                bmpmask = new Bitmap(bmpmask, new Size((int)(bmpmask.Width / mySize), (int)(bmpmask.Height / mySize)));
            }

                //畫入 image 中待處理
                AUUtility.DrawBitmapToAUGrayImg8(bmppattern, ref imgpattern);
            AUUtility.DrawBitmapToAUGrayImg8(bmpmask, ref imgmask);
            AUUtility.DrawBitmapToAUColorImg24(bmppattern, ref imgResult);
            //bmpmask.Save(Universal.TESTPATH + "\\ANALYZETEST\\FROMASK32BMP" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            //imgmask.Save(Universal.TESTPATH + "\\ANALYZETEST\\FROMASK32IMG.png", eImageFormat.eImageFormat_PNG);

            //if ((RelateAnalyzeString.IndexOf("0017") > -1)) // || RelateAnalyzeString.IndexOf("0016") > -1 || RelateAnalyzeString.IndexOf("0042") > -1 || RelateAnalyzeString.IndexOf("0054") > -1))
            //{
            //    imgmask.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + RelateAnalyzeString + "-00-MASK.png", eImageFormat.eImageFormat_PNG);
            //    imgpattern.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + RelateAnalyzeString + "-01-PATTERN.png", eImageFormat.eImageFormat_PNG);
            //}

            //設定 Train 的參數
            //AUTrainInfoF.nDownSamplingSize = (MTPSample == 0 ? Math.Min(bmppattern.Width, bmppattern.Height) / downsampleratio : (int)((float)Math.Min(bmppattern.Width, bmppattern.Height) * (MTPSample / 100f)));
            //AUTrainInfoF.nDownSamplingSize = (MTPSample == 0 ? downsampleratiodefault : (int)((float)Math.Min(bmppattern.Width, bmppattern.Height) * (MTPSample / 100f)));

            if (MTPSample == -1)
                AUTrainInfoF.nDownSamplingSize = 160;
            else
                AUTrainInfoF.nDownSamplingSize = (MTPSample == 0 ? downsampleratiodefault : (int)MTPSample);

            AUTrainInfoF.nCannyThresholdHigh = MTCannyH;
            AUTrainInfoF.nCannyThresholdLow = MTCannyL;
            AUTrainInfoF.fRotationTolerance = MTRotation;
            AUTrainInfoF.fScalingTolerance =  MTScaling;
            AUTrainInfoF.eFMode = eFindMode.eFindMode_GHT;

            ms = mytime.msDuriation;
            mytime.Cut();

            //if (AUFIND == null)
            //    AUFIND = new AUFind();
            //訓練圖像

                isgood = AUFIND.Training(imgpattern, imgmask, AUTrainInfoF, MTCannyAuto);
                
           
                //bmppattern.Save("D:\\pattern.png");
                //bmpmask.Save("D:\\mask.png");


            AUFIND.SetMaxOcc(MTMaxOcc);
            AUFIND.SetTolerance(0.7f);
            //xTrainingInfoF tmpInfo = new xTrainingInfoF();
            //AUFIND.GetTrainingInfo(out tmpInfo);

            //bmppattern.Save("D:\\testtest\\Pattern.png");
            //bmpmask.Save("D:\\testtest\\Mask.png");
            //if(bmpmask.Width!=1)
            AUFIND.DrawResultContour(imgResult,10, Color.Lime.R, Color.Lime.G, Color.Lime.B);
            //     AUFIND.DrawTemplateContour(imgResult, Color.Lime.R, Color.Lime.G, Color.Lime.B);
            

            if (bmpContour != null)
                bmpContour.Dispose();

            bmpContour = new Bitmap(bmppattern.Width, bmppattern.Height);
            AUUtility.DrawAUColorImg24ToBitmap(imgResult, ref bmpContour);

       //     bmpContour.Save("d:\\save.png");
            ms = mytime.msDuriation;
            mytime.Cut();

            if (isgood)
            {
                str = "Pattern Training Successful.";
            }
            else
            {
                str = "Pattern Training Error.";
            }

            imgpattern.Dispose();
            imgmask.Dispose();
            imgResult.Dispose();
            
            return isgood;
        }

        bool AuMatchTrain(Bitmap bmppattern, Bitmap bmpmask, int brightness, int contrast)
        {
            const int downsampleratiodefault = 88;

            bool isgood = true;

            AUGrayImg8 imgpattern = new AUGrayImg8();
            AUGrayImg8 imgmask = new AUGrayImg8();
            AUColorImg24 imgResult = new AUColorImg24();

            JzTimes mytime = new JzTimes();
            int ms = 0;
            string str = "";

            mytime.Cut();

            JzFindObjectClass jzfind = new JzFindObjectClass();
            jzfind.Find(bmpmask, Color.Red);
            if(jzfind.FoundList.Count>0)
            {
                //bmpmask.Save("D:\\testtest\\masktemp.png");

                Rectangle rect = jzfind.FoundList[0].rect;
                bmppattern = bmppattern.Clone(rect, PixelFormat.Format32bppArgb);
            }

            //畫入 image 中待處理
            AUUtility.DrawBitmapToAUGrayImg8(bmppattern, ref imgpattern);
            AUUtility.DrawBitmapToAUGrayImg8(bmpmask, ref imgmask);
            AUUtility.DrawBitmapToAUColorImg24(bmppattern, ref imgResult);

           

            //設定 Train 的參數
            xInfo.eAccuracy = eMatchingAccuracy.eMatchingAccuracy_High; //Set High-Accuracy
            xInfo.isTargetRotated = true; //if the templates in target image is rotated. 
            xInfo.nPyramidSize = (MTPSample == 0 ? downsampleratiodefault : (int)MTPSample); //Set sown-sampling size = 35 (Default)
            xInfo.ePrefilter = eMatchingPrefilter.eMatchingPrefilter_Sobel;

            ms = mytime.msDuriation;
            mytime.Cut();

            //if (AUFIND == null)
            //    AUFIND = new AUFind();
            //訓練圖像
            isgood = AUMATCH.TrainingPattern(imgpattern, xInfo);
            AUMATCH.SetMaxOcc(MTMaxOcc);
            AUMATCH.SetTolerance(MTTolerance);

            //int resultcount = AUMATCH.Matching(imgpattern);
            //Bitmap bmp = bmpmask.Clone(new Rectangle(0, 0, bmpmask.Width, bmpmask.Height), PixelFormat.Format8bppIndexed);
            //bmpmask.Save("D:\\testtest\\Mask.png");
            //Bitmap bmp2 = bmpPattern.Clone(new Rectangle(0, 0, bmpPattern.Width, bmpPattern.Height), PixelFormat.Format8bppIndexed);
            //bmppattern.Save("D:\\testtest\\Pattern.png");

            //xTrainingInfoF tmpInfo = new xTrainingInfoF();
            //AUFIND.GetTrainingInfo(out tmpInfo);

            //AUMATCH.DrawTemplateContour(imgResult, Color.Lime.R, Color.Lime.G, Color.Lime.B);

            if (bmpContour != null)
                bmpContour.Dispose();

            bmpContour = new Bitmap(bmppattern.Width, bmppattern.Height);
            AUUtility.DrawAUColorImg24ToBitmap(imgResult, ref bmpContour);

            ms = mytime.msDuriation;
            mytime.Cut();

            if (isgood)
            {
                str = "Pattern Training Successful.";
            }
            else
            {
                str = "Pattern Training Error.";
            }

            imgpattern.Dispose();
            imgmask.Dispose();
            imgResult.Dispose();

            return isgood;
        }

        public bool IsSeedTrain()
        {
            return IsSeedTrain(0, 0);
        }
        public bool IsSeedTrain(int brightness, int contrast)
        {
            return AuFindTrain(bmpPattern, bmpMask, brightness, contrast);
        }

        public bool CheckAbsOffset(PointF ptfOrg, PointF ptfRun,RectangleF oprectf)
        {
            bool ret = false;
            if (AbsAlignMode == AbsoluteAlignEnum.RELATION)
            {
                PointF ptfpatternORG = new PointF(OrgCenter.X + oprectf.X, OrgCenter.Y + oprectf.Y);
                PointF ptfpatternRUN = new PointF(RunCenter.X + oprectf.X, RunCenter.Y + oprectf.Y);

                double originaldistance = GetPointLength(ptfOrg, ptfpatternORG);
                double runningdistance = GetPointLength(ptfRun, ptfpatternRUN);

                double xshiftorg = ptfOrg.X - OrgCenter.X;
                double yshiftorg = ptfOrg.Y - OrgCenter.Y;

                double xshiftrun = ptfRun.X - RunCenter.X;
                double yshiftrun = ptfRun.Y - RunCenter.Y;

                double xshiftrunxx = Math.Abs(xshiftrun - xshiftorg);
                double yshiftrunyy = Math.Abs(yshiftrun - yshiftorg);

                Offset = (float)Math.Abs(originaldistance - runningdistance);
                Offset *= MTResolution;
                Offset = (float)Math.Round(Offset, 2);
                xshiftrunxx *= MTResolution;
                yshiftrunyy *= MTResolution;

                WorkStatusClass biasrunstatus = new WorkStatusClass(AnanlyzeProcedureEnum.BIAS);
                string processstring = "Start " + RelateAnalyzeString + " Alignment " + (false ? "<TRAIN>" : "<RUN>") + " Run." + Environment.NewLine;
                string errorstring = "";
                ReasonEnum reason = ReasonEnum.PASS;
                string offsetStr = "X偏移=" + xshiftrunxx.ToString("0.00");
                offsetStr += " Y偏移=" + yshiftrunyy.ToString("0.00");
                if (xshiftrunxx > ABSOffset || yshiftrunyy > ABSOffset)
                //if(Offset > ABSOffset)
                {
                    
                    ret = true;
                    //isgood = false;
                    //PassInfo.BiasOffset = "偏移 " + Offset.ToString("0.00");
                    processstring = "The ABSOffset is " + Offset.ToString("0.00") + " > " + ABSOffset.ToString("0.00") + " Error." + Environment.NewLine;
                    processstring = "The ABSOffset is " + offsetStr + " > " + ABSOffset.ToString("0.00") + " Error." + Environment.NewLine;
                    errorstring = RelateAnalyzeString + "The Offset is " + Offset.ToString("0.00") + " > " + ABSOffset.ToString("0.00") + " Error." + Environment.NewLine;
                    errorstring = Offset.ToString("0.00");
                    errorstring = offsetStr;
                    reason = ReasonEnum.NG;
                }
                else
                {
                    //PassInfo.BiasOffset = "偏移 " + Offset.ToString("0.00");
                    processstring = "The ABSOffset is " + Offset.ToString("0.00") + " < " + ABSOffset.ToString("0.00") + " Pass." + Environment.NewLine;
                    processstring = "The ABSOffset is " + offsetStr + " < " + ABSOffset.ToString("0.00") + " Pass." + Environment.NewLine;
                    errorstring = "";
                    reason = ReasonEnum.PASS;
                }

                biasrunstatus.SetWorkStatus(bmpPattern, bmpPattern, bmpPattern, reason, errorstring, processstring, PassInfo);

                //if (istrain)
                //    TrainStatusCollection.Add(biasrunstatus);
                //else
                RunStatusCollection.Add(biasrunstatus);
            }
            return ret;
        }

        /// <summary>
        /// 定位圖像並輸出轉正後的圖像
        /// </summary>
        /// <param name="bmpinput"></param>
        /// <param name="bmpoutput"></param>
        /// <param name="istrain"></param>
        /// <param name="brightness"></param>
        /// <param name="contrast"></param>
        /// <returns></returns>
        public bool AuFindRun(Bitmap bmpinput, ref Bitmap bmpoutput, bool istrain, int brightness, int contrast)
        {
            bool isgood = true;
            CheckGood = true;
            //bmpoutput.Dispose();
            bmpoutput = new Bitmap(1, 1);

            if (MTPSample == -1)
            {

                if (bmpinput.Width / mySize < 1)
                    bmpinput = new Bitmap(bmpinput);
                else
                    bmpinput = new Bitmap(bmpinput, new Size((int)(bmpinput.Width / mySize), (int)(bmpinput.Height / mySize)));
                //   bmpmask = new Bitmap(bmpmask, new Size((int)(bmpmask.Width / mySize), (int)(bmpmask.Height / mySize)));
            }

            WorkStatusClass workstatus = new WorkStatusClass(AnanlyzeProcedureEnum.ALIGNRUN);
            string processstring = "Start " + RelateAnalyzeString + " Alignment " + (istrain ? "<TRAIN>" : "<RUN>") + " Run." + Environment.NewLine;
            string errorstring = "";
            ReasonEnum reason = ReasonEnum.PASS;

            switch (AlignMethod)
            {
                case AlignMethodEnum.NONE:

                    processstring += "Alignment For NONE." + Environment.NewLine;

                    bmpoutput.Dispose();
                    //bmpoutput = new Bitmap(bmpinput);
                    bmpoutput = (Bitmap)bmpinput.Clone();

                    //SetBrightContrast(bmpoutput, brightness, contrast);

                    //IsPass = true;
                    workstatus.SetWorkStatus(bmpPattern, bmpoutput, bmpPattern, reason, errorstring, processstring, PassInfo);

                    return true;
            }

            JzTimes mytime = new JzTimes();
            int resultcount = 0;
            int ms = 0;

            AUGrayImg8 imginput = new AUGrayImg8();
            AUColorImg24 imginput24 = new AUColorImg24();
            AUColorImg24 imgoutput24 = new AUColorImg24();



            //Bitmap bmptest = new Bitmap(Universal.TESTPATH + "\\ANALYZETEST\\ALIGN.png");
            bmpRunInput.Dispose();
            //bmpRunInput = new Bitmap(bmpinput);
            bmpRunInput = (Bitmap)bmpinput.Clone();


            //bmpPattern.Save(Universal.TESTPATH + "\\ANALYZETEST\\Pattern" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            //bmpRunInput.Save(Universal.TESTPATH + "\\ANALYZETEST\\RUNINPUT" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            //bmpMask.Save(Universal.TESTPATH + "\\ANALYZETEST\\MASK" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

            AUUtility.DrawBitmapToAUColorImg24(bmpinput, ref imginput24);

            processstring += "Set Brightness " + brightness.ToString() + " ,Contrast " + contrast.ToString() + "." + Environment.NewLine;

            SetBrightContrast(bmpRunInput, brightness, contrast);

            //Universal.IsMultiThread = false;
            //IsTempSave = true;
            if (IsTempSave)
            {
                //xTrainingInfoF tmpInfo = new xTrainingInfoF();
                //AUFIND.GetTrainingInfo(out tmpInfo);
                string strpath = "D:\\TestTest\\" + Universal.RESULT.myResult.RELATECOLORSTR + "\\" + RelateAnalyzeString + "\\";
                if (!Directory.Exists(strpath))
                    Directory.CreateDirectory(strpath);
                if (!Directory.Exists(strpath + "Data\\"))
                    Directory.CreateDirectory(strpath + "Data\\");

                Bitmap bmpsavePatt = new Bitmap(bmpPattern, new Size((int)(bmpPattern.Width / mySize), (int)(bmpPattern.Height / mySize)));

                bmpsavePatt.Save(strpath + "\\TRAINPATTERN" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                bmpsavePatt.Dispose();
                //bmpMask Should Disposed if there is no inspection to do
                //    if (!(bmpMask.Width == 1 && bmpMask.Height == 1))

                Bitmap bmpsavemask = new Bitmap(bmpMask, new Size((int)(bmpMask.Width / mySize), (int)(bmpMask.Height / mySize)));
                bmpsavemask.Save(strpath + "\\TRAINMASK" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                bmpsavemask.Dispose();
                bmpRunInput.Save(strpath + "Data\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

            }

            AUUtility.DrawBitmapToAUGrayImg8(bmpRunInput, ref imginput);

            //    bmpRunInput.Dispose();

            processstring += "Do Run Alignment." + Environment.NewLine;


            switch (AlignMethod)
            {
                case AlignMethodEnum.AUFIND:
                    //  case AlignMethodEnum.AUMATCH:
                    resultcount = AUFIND.Find(imginput, 3000);
                    break;
                case AlignMethodEnum.AUMATCH:
                    resultcount = AUMATCH.Matching(imginput);
                    //bmpRunInput.Save("D:\\testtest\\input.png");
                    break;
            }
            bmpRunInput.Dispose();
            ms = mytime.msDuriation;

            //檢查訓練後的數目是否和MaxOCC相同
            if (istrain)
            {
                if (resultcount > MTMaxOcc)
                {
                    isgood = false;
                    //CheckGood = false;
                    processstring += "Error For More Than " + MTMaxOcc + " Results." + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " Error For More Than " + MTMaxOcc + " Results." + Environment.NewLine;
                    reason = ReasonEnum.NG;
                }
                else if (resultcount < MTMaxOcc)
                {
                    isgood = false;
                    //CheckGood = false;
                    //bmpPattern.Save(Universal.TESTPATH + "\\ANALYZETEST\\TRAINPATTERN" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                    //bmpMask.Save(Universal.TESTPATH + "\\ANALYZETEST\\TRAINMASK" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                    //bmpRunInput.Save(Universal.TESTPATH + "\\ANALYZETEST\\RUNINPUT" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                    processstring += "Error For Less Than " + MTMaxOcc + " Results." + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " Error For Less Than " + MTMaxOcc + " Results." + Environment.NewLine;
                    reason = ReasonEnum.NG;
                }
            }
            else
            {
                if (resultcount < MTMaxOcc)
                {
                    isgood = false;
                    //CheckGood = false;
                    processstring += "Error For Less Than " + MTMaxOcc + " Results." + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " Error For Less Than " + MTMaxOcc + " Results." + Environment.NewLine;
                    reason = ReasonEnum.NG;
                }
            }

            if (resultcount >= MTMaxOcc && isgood)
            {
                if (MTPSample == -1)
                {
                    Bitmap bmpparre = new Bitmap(bmpPattern, new Size((int)(bmpPattern.Width / mySize), (int)(bmpPattern.Height / mySize)));
                    imgoutput24 = new AUColorImg24(bmpparre.Width, bmpparre.Height);
                    // imgoutput24.SetImage(130);
                    AUUtility.DrawBitmapToAUColorImg24(bmpparre, ref imgoutput24);
                }
                else
                {
                    imgoutput24 = new AUColorImg24(bmpPattern.Width, bmpPattern.Height);
                    // imgoutput24.SetImage(130);
                    AUUtility.DrawBitmapToAUColorImg24(bmpPattern, ref imgoutput24);
                }

                //if (RelateAnalyzeString.IndexOf("ANZ-03-0021") > -1 && !istrain)
                //{
                //    imgoutput24.Save(Universal.TESTPATH + "\\ANALYZETEST\\imgoutput.bmp", eImageFormat.eImageFormat_BMP);
                //}

                if (AlignMethod == AlignMethodEnum.AUFIND)
                {
                    xFindResult result = new xFindResult();
                    AUFIND.GetResult(out result, 0);

                    Score = result.fScore;

                    if (Score < MTTolerance)
                    {

                        isgood = false;
                        //CheckGood = false;
                        processstring += "The Result Score is " + Score.ToString("0.00") + " < " + MTTolerance.ToString("0.00") + " Error." + Environment.NewLine;
                        errorstring += RelateAnalyzeString + " The Result Score is " + Score.ToString("0.00") + " < " + MTTolerance.ToString("0.00") + " Error." + Environment.NewLine;
                        reason = ReasonEnum.NG;
                    }
                    else
                    {
                        processstring += "The Result Score is " + Score.ToString("0.00") + " >= " + MTTolerance.ToString("0.00") + " Pass." + Environment.NewLine;
                    }
                    //      imginput.Save("d:\\temp2.png", eImageFormat.eImageFormat_PNG);
                    if (isgood)
                    {


                        ScaleRotateEX2(result, imginput24, ref imgoutput24);

                        processstring += "The Result Angle is " + Rotation.ToString("0.000") + " , Offset is " + Offset.ToString("0.000") + " ." + Environment.NewLine;

                        //if ((RelateAnalyzeString.IndexOf("0001") > -1 || RelateAnalyzeString.IndexOf("0016") > -1 || RelateAnalyzeString.IndexOf("0042") > -1 || RelateAnalyzeString.IndexOf("0054") > -1) && !istrain)
                        //{
                        //    string str = "Angle:" + result.fAngle.ToString() + Environment.NewLine;
                        //    str += "XPos:" + result.fCenterX.ToString() + Environment.NewLine;
                        //    str += "YPos:" + result.fCenterY.ToString() + Environment.NewLine;
                        //    str += "Scale:" + result.fScale.ToString() + Environment.NewLine;
                        //    str += "Score:" + result.fScore.ToString() + Environment.NewLine;
                        //    //str += "ROI:" + result.fScore.ToString() + Environment.NewLine;

                        //    SaveData(str, Universal.TESTPATH + "\\ANALYZETEST\\" + RelateAnalyzeString + "-0A-ALIGNDATA.txt");

                        //    //bmpPattern.Save(Universal.TESTPATH + "\\ANALYZETEST\\Pattern-" + RelateAnalyzeString + ".bmp", ImageFormat.Bmp);
                        //    //bmpMask.Save(Universal.TESTPATH + "\\ANALYZETEST\\Mask-" + RelateAnalyzeString + ".bmp", ImageFormat.Bmp);

                        //    imginput.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + RelateAnalyzeString + "-02-INPUT.png", eImageFormat.eImageFormat_PNG);
                        //    imgoutput24.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + RelateAnalyzeString + "-03-OUTPUT.png", eImageFormat.eImageFormat_PNG);

                        if (IsTempSave)
                        {

                            string strpath = "D:\\TestTest\\" + Universal.RESULT.myResult.RELATECOLORSTR + "\\" + RelateAnalyzeString + "\\";
                            if (!Directory.Exists(strpath))
                                Directory.CreateDirectory(strpath);

                            imginput24.Save(strpath + "Input" + ".png", eImageFormat.eImageFormat_PNG);
                            imgoutput24.Save(strpath + "output" + ".png", eImageFormat.eImageFormat_PNG);

                        }

                        if (istrain)
                        {
                            OrgCenter = new PointF(result.fCenterX, result.fCenterY);
                            AlignOffset = new PointF(0, 0);
                        }
                        else
                        {
                            RunCenter = new PointF(result.fCenterX, result.fCenterY);
                            AlignOffset = new PointF(OrgCenter.X - result.fCenterX, OrgCenter.Y - result.fCenterY);
                        }

                    }

                    if (isgood)
                    {
                        processstring += RelateAnalyzeString + " Alignment Successful." + Environment.NewLine;
                    }
                }
                else if (AlignMethod == AlignMethodEnum.AUMATCH)
                {
                    xMatchingResult resultMatch = new xMatchingResult();
                    AUMATCH.GetResult(out resultMatch, 0);

                    Score = resultMatch.fScore;

                    if (Score < MTTolerance)
                    {

                        isgood = false;
                        //CheckGood = false;
                        processstring += "The Result Score is " + Score.ToString("0.00") + " < " + MTTolerance.ToString("0.00") + " Error." + Environment.NewLine;
                        errorstring += RelateAnalyzeString + " The Result Score is " + Score.ToString("0.00") + " < " + MTTolerance.ToString("0.00") + " Error." + Environment.NewLine;
                        reason = ReasonEnum.NG;
                    }
                    else
                    {
                        processstring += "The Result Score is " + Score.ToString("0.00") + " >= " + MTTolerance.ToString("0.00") + " Pass." + Environment.NewLine;
                    }
                    //      imginput.Save("d:\\temp2.png", eImageFormat.eImageFormat_PNG);
                    if (isgood)
                    {


                        ScaleRotateEX(resultMatch, imginput24, ref imgoutput24);

                        processstring += "The Result Angle is " + Rotation.ToString("0.000") + " , Offset is " + Offset.ToString("0.000") + " ." + Environment.NewLine;

                        //if ((RelateAnalyzeString.IndexOf("0001") > -1 || RelateAnalyzeString.IndexOf("0016") > -1 || RelateAnalyzeString.IndexOf("0042") > -1 || RelateAnalyzeString.IndexOf("0054") > -1) && !istrain)
                        //{
                        //    string str = "Angle:" + result.fAngle.ToString() + Environment.NewLine;
                        //    str += "XPos:" + result.fCenterX.ToString() + Environment.NewLine;
                        //    str += "YPos:" + result.fCenterY.ToString() + Environment.NewLine;
                        //    str += "Scale:" + result.fScale.ToString() + Environment.NewLine;
                        //    str += "Score:" + result.fScore.ToString() + Environment.NewLine;
                        //    //str += "ROI:" + result.fScore.ToString() + Environment.NewLine;

                        //    SaveData(str, Universal.TESTPATH + "\\ANALYZETEST\\" + RelateAnalyzeString + "-0A-ALIGNDATA.txt");

                        //    //bmpPattern.Save(Universal.TESTPATH + "\\ANALYZETEST\\Pattern-" + RelateAnalyzeString + ".bmp", ImageFormat.Bmp);
                        //    //bmpMask.Save(Universal.TESTPATH + "\\ANALYZETEST\\Mask-" + RelateAnalyzeString + ".bmp", ImageFormat.Bmp);

                        //    imginput.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + RelateAnalyzeString + "-02-INPUT.png", eImageFormat.eImageFormat_PNG);
                        //    imgoutput24.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + RelateAnalyzeString + "-03-OUTPUT.png", eImageFormat.eImageFormat_PNG);

                        if (IsTempSave)
                        {
                            //imginput24.Save(Universal.TESTPATH + "\\ANALYZETEST\\2-ImageInput-" + RelateAnalyzeString + ".bmp", eImageFormat.eImageFormat_BMP);
                            //  imgoutput24.Save(Universal.TESTPATH + "\\ANALYZETEST\\2-Rotateimgoutput-2" + RelateAnalyzeString + ".bmp", eImageFormat.eImageFormat_BMP);

                        }

                        if (istrain)
                        {
                            OrgCenter = new PointF(resultMatch.fCenterX, resultMatch.fCenterY);
                            AlignOffset = new PointF(0, 0);
                        }
                        else
                        {
                            RunCenter = new PointF(resultMatch.fCenterX, resultMatch.fCenterY);
                            AlignOffset = new PointF(OrgCenter.X - resultMatch.fCenterX, OrgCenter.Y - resultMatch.fCenterY);
                        }

                    }

                    if (isgood)
                    {
                        processstring += RelateAnalyzeString + " Alignment Successful." + Environment.NewLine;
                    }
                }

                if (isgood)
                    AUUtility.DrawAUColorImg24ToBitmap(imgoutput24, ref bmpoutput);
                else
                    bmpoutput = new Bitmap(bmpinput);



                if (MTPSample == -1)
                    bmpoutput = new Bitmap(bmpoutput, bmpPattern.Size);

                //string strpath2 = "D:\\TestTest\\" + Universal.RESULT.myResult.RELATECOLORSTR + "\\" + RelateAnalyzeString + "\\";
                //bmpoutput.Save(strpath2 + "Input2" + ".png");

            }

            imginput.Dispose();
            imginput24.Dispose();
            imgoutput24.Dispose();


            if (bmpoutput.Width > 1)
            {
                workstatus.SetWorkStatus(bmpPattern, bmpoutput, bmpPattern, reason, errorstring, processstring, PassInfo);
            }
            else
            {
                if (bmpPattern == null || bmpinput == null)
                    workstatus.SetWorkStatus(new Bitmap(1, 1), bmpinput, new Bitmap(1, 1), reason, errorstring, processstring, PassInfo);
                else
                    workstatus.SetWorkStatus(bmpPattern, bmpinput, bmpPattern, reason, errorstring, processstring, PassInfo);
            }

            if (istrain)
                TrainStatusCollection.Add(workstatus);
            else
                RunStatusCollection.Add(workstatus);

            if (isgood)
            {
                //Checking Offset Result
                if (MTOffset != 0)
                {
                    WorkStatusClass biasrunstatus = new WorkStatusClass(AnanlyzeProcedureEnum.BIAS);

                    if (Offset > MTOffset)
                    {
                        isgood = false;
                        //PassInfo.BiasOffset = "偏移 " + Offset.ToString("0.00");
                        processstring = "The Offset is " + Offset.ToString("0.00") + " > " + MTOffset.ToString("0.00") + " Error." + Environment.NewLine;
                        errorstring = RelateAnalyzeString + "The Offset is " + Offset.ToString("0.00") + " > " + MTOffset.ToString("0.00") + " Error." + Environment.NewLine;
                        reason = ReasonEnum.NG;
                        //CheckGood = false;
                        //if (!System.IO.Directory.Exists(Universal.MainX6_Path))
                        //    System.IO.Directory.CreateDirectory(Universal.MainX6_Path);

                        if (INI.IsCollectErrorSmall)
                        {
                            if (!System.IO.Directory.Exists(Universal.MainX6_Path + "\\offset"))
                                System.IO.Directory.CreateDirectory(Universal.MainX6_Path + "\\offset");

                            //bmpInput.Save(Universal.MainX6_Path + "\\" + RelateAnalyzeString + "_Input" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                            bmpoutput.Save(Universal.MainX6_Path + "\\offset" + "\\" + RelateAnalyzeString + "_Output" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                            bmpPattern.Save(Universal.MainX6_Path + "\\offset" + "\\" + RelateAnalyzeString + "_Pattern" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                        }

                        //Add Codes 2017/08/19
                        //Draw Offset Lines
                        //
                    }
                    else
                    {
                        //PassInfo.BiasOffset = "偏移 " + Offset.ToString("0.00");
                        processstring = "The Offset is " + Offset.ToString("0.00") + " < " + MTOffset.ToString("0.00") + " Pass." + Environment.NewLine;
                        errorstring = "";
                        reason = ReasonEnum.PASS;
                    }

                    biasrunstatus.SetWorkStatus(bmpPattern, bmpoutput, bmpPattern, reason, errorstring, processstring, PassInfo);

                    if (istrain)
                        TrainStatusCollection.Add(biasrunstatus);
                    else
                        RunStatusCollection.Add(biasrunstatus);
                }
            }

            //IsPass = isgood;
            CheckGood = isgood;
            return isgood;
        }
        /// <summary>
        /// 算出偏移及旋轉值
        /// </summary>
        /// <param name="result"></param>
        /// <param name="imginput24"></param>
        /// <param name="imgoutput24"></param>
        public void ScaleRotate(xFindResult result,AUColorImg24 imginput24,ref AUColorImg24 imgoutput24)
        {
            imgoutput24.Dispose();
            imgoutput24 = new AUColorImg24(imginput24.GetWidth(), imginput24.GetHeight());

            float fTargetCX = imginput24.GetWidth() / 2.0f;
            float fTargetCY = imginput24.GetHeight() / 2.0f;
            float fAffineCX = imginput24.GetWidth() / 2.0f;
            float fAffineCY = imginput24.GetHeight() / 2.0f;

            float fCosSida = (float)Math.Cos(-result.fAngle * Math.PI / 180.0f);
            float fSinSida = (float)Math.Sin(-result.fAngle * Math.PI / 180.0f);
            float fX = (result.fCenterX - fTargetCX) * fCosSida - (result.fCenterY - fTargetCY) * fSinSida;
            float fY = (result.fCenterX - fTargetCX) * fSinSida + (result.fCenterY - fTargetCY) * fCosSida;

            float fX1 = fAffineCX - fX;
            float fY1 = fAffineCY - fY;

            //eInterpolationBits_1,4,8 8 for best but slowest
            AUImage.ScaleRotate(imginput24, imgoutput24,
                    fTargetCX, fTargetCY,
                    //Result.fCenterX, Result.fCenterY,
                    fX1, fY1,
                    result.fAngle, 1.0f, 1.0f, eInterpolationBits.eInterpolationBits_8);

            //取得旋轉和偏移的值
            Rotation = result.fAngle;
            Offset = (float)Math.Sqrt(Math.Pow(fX, 2) + Math.Pow(fY, 2));
            Offset *= MTResolution;
            
            AlignDegree = result.fAngle;
        }

        /// <summary>
        /// 算出偏移及旋轉值
        /// </summary>
        /// <param name="result"></param>
        /// <param name="imginput24"></param>
        /// <param name="imgoutput24"></param>
        public void ScaleRotateEX2(xFindResult result, AUColorImg24 imginput24, ref AUColorImg24 imgoutput24)
        {
            //imgoutput24.Dispose();
            //imgoutput24 = new AUColorImg24(imginput24.GetWidth(), imginput24.GetHeight());

            double fTargetCX = imginput24.GetWidth() / 2.0d;
            double fTargetCY = imginput24.GetHeight() / 2.0d;
            //double fAffineCX = imginput24.GetWidth() / 2.0d;
            //double fAffineCY = imginput24.GetHeight() / 2.0d;

            double fCosSida = Math.Cos(-result.fAngle * Math.PI / 180.0f);
            double fSinSida = Math.Sin(-result.fAngle * Math.PI / 180.0f);
            double fX = (result.fCenterX - fTargetCX) * fCosSida - (result.fCenterY - fTargetCY) * fSinSida;
            double fY = (result.fCenterX - fTargetCX) * fSinSida + (result.fCenterY - fTargetCY) * fCosSida;

            //double fX1 = fAffineCX - fX;
            //double fY1 = fAffineCY - fY;

            ////eInterpolationBits_1,4,8 8 for best but slowest
            //AUImage.ScaleRotate(imginput24, imgoutput24,
            //        (float)fTargetCX,
            //        (float)fTargetCY,
            //        //Result.fCenterX, Result.fCenterY,
            //        (float)fX1, (float)fY1,
            //        result.fAngle,
            //        1.0f,
            //        1.0f,
            //        eInterpolationBits.eInterpolationBits_8);


            float fSrcCX = result.fCenterX; //旋轉中心 X
            float fSrcCY = result.fCenterY; //旋轉中心 Y
            float fDstCX = imgoutput24.GetWidth() / 2; //目標影像中心 X
            float fDstCY = imgoutput24.GetHeight() / 2; //目標影像中心 Y

            //eInterpolationBits_1,4,8 8 for best but slowest
            AUImage.ScaleRotate(imginput24, imgoutput24,
                    //Result.fCenterX, Result.fCenterY,
                    fSrcCX, fSrcCY,
                    fDstCX, fDstCY,
                    result.fAngle,
                    1.0f,
                    1.0f,
                    eInterpolationBits.eInterpolationBits_8);

            //取得旋轉和偏移的值

            Rotation = result.fAngle;
            Offset = (float)Math.Sqrt(Math.Pow(fX, 2) + Math.Pow(fY, 2));
            Offset *= MTResolution;

            AlignDegree = result.fAngle;
        }

        /// <summary>
        /// 算出偏移及旋轉值
        /// </summary>
        /// <param name="result"></param>
        /// <param name="imginput24"></param>
        /// <param name="imgoutput24"></param>
        public void ScaleRotateEX(xFindResult result, AUColorImg24 imginput24, ref AUColorImg24 imgoutput24)
        {
            //imgoutput24.Dispose();
            //imgoutput24 = new AUColorImg24(imginput24.GetWidth(), imginput24.GetHeight());

            double fTargetCX = imginput24.GetWidth() / 2.0d;
            double fTargetCY = imginput24.GetHeight() / 2.0d;
            double fAffineCX = imginput24.GetWidth() / 2.0d;
            double fAffineCY = imginput24.GetHeight() / 2.0d;

            double fCosSida = Math.Cos(-result.fAngle * Math.PI / 180.0f);
            double fSinSida = Math.Sin(-result.fAngle * Math.PI / 180.0f);
            double fX = (result.fCenterX - fTargetCX) * fCosSida - (result.fCenterY - fTargetCY) * fSinSida;
            double fY = (result.fCenterX - fTargetCX) * fSinSida + (result.fCenterY - fTargetCY) * fCosSida;

            double fX1 = fAffineCX - fX;
            double fY1 = fAffineCY - fY;

            //eInterpolationBits_1,4,8 8 for best but slowest
            AUImage.ScaleRotate(imginput24, imgoutput24,
                    (float)fTargetCX,
                    (float)fTargetCY,
                    //Result.fCenterX, Result.fCenterY,
                    (float)fX1, (float)fY1,
                    result.fAngle, 
                    1.0f, 
                    1.0f, 
                    eInterpolationBits.eInterpolationBits_8);

            //取得旋轉和偏移的值

            Rotation = result.fAngle;
            Offset = (float)Math.Sqrt(Math.Pow(fX, 2) + Math.Pow(fY, 2));
            Offset *= MTResolution;

            AlignDegree = result.fAngle;
        }

        /// <summary>
        /// 算出偏移及旋轉值
        /// </summary>
        /// <param name="result"></param>
        /// <param name="imginput24"></param>
        /// <param name="imgoutput24"></param>
        public void ScaleRotateEX(xMatchingResult result, AUColorImg24 imginput24, ref AUColorImg24 imgoutput24)
        {
            //imgoutput24.Dispose();
            //imgoutput24 = new AUColorImg24(imginput24.GetWidth(), imginput24.GetHeight());

            double fTargetCX = imginput24.GetWidth() / 2.0d;
            double fTargetCY = imginput24.GetHeight() / 2.0d;
            double fAffineCX = imginput24.GetWidth() / 2.0d;
            double fAffineCY = imginput24.GetHeight() / 2.0d;

            double fCosSida = Math.Cos(-result.fAngle * Math.PI / 180.0f);
            double fSinSida = Math.Sin(-result.fAngle * Math.PI / 180.0f);
            double fX = (result.fCenterX - fTargetCX) * fCosSida - (result.fCenterY - fTargetCY) * fSinSida;
            double fY = (result.fCenterX - fTargetCX) * fSinSida + (result.fCenterY - fTargetCY) * fCosSida;

            double fX1 = fAffineCX - fX;
            double fY1 = fAffineCY - fY;

            //eInterpolationBits_1,4,8 8 for best but slowest
            AUImage.ScaleRotate(imginput24, imgoutput24,
                    (float)fTargetCX,
                    (float)fTargetCY,
                    //Result.fCenterX, Result.fCenterY,
                    (float)fX1, (float)fY1,
                    result.fAngle,
                    1.0f,
                    1.0f,
                    eInterpolationBits.eInterpolationBits_8);

            //取得旋轉和偏移的值

            Rotation = result.fAngle;
            Offset = (float)Math.Sqrt(Math.Pow(fX, 2) + Math.Pow(fY, 2));
            Offset *= MTResolution;

            AlignDegree = result.fAngle;
        }
        /// <summary>
        /// 神風!!!!
        /// </summary>
        public void Suicide()
        {
            //AUFIND.Dispose(); //Don't know why

            if(bmpMask != null)
                bmpMask.Dispose();

            if (bmpPattern != null)
                bmpPattern.Dispose();

            if (bmpRunInput != null)
                bmpRunInput.Dispose();

            if (bmpRunOutput != null)
                bmpRunOutput.Dispose();

            if (bmpContour != null)
                bmpContour.Dispose();


            TrainStatusCollection.Clear();
            RunStatusCollection.Clear();
        }
        /// <summary>
        /// 在做大量運算前要清除的相關資料
        /// </summary>
        public void ResetTrainStatus()
        {
            TrainStatusCollection.Clear();
            //CheckGood = true;
        }
        /// <summary>
        /// 在做大量運算前要清除的相關資料
        /// </summary>
        public void ResetRunStatus()
        {
            RunStatusCollection.Clear();
            //CheckGood = true;
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
                if (filltoanalyzestr == null)
                {
                    if (runstatus.LogString == "")
                    {
                        runstatuscollection.Add(runstatus);
                    }
                }
                else
                {
                    if (runstatus.LogString.IndexOf(filltoanalyzestr) < 0)
                    {
                        runstatus.LogString += filltoanalyzestr;
                        runstatuscollection.Add(runstatus);
                    }
                }
            }
        }

        /// <summary>
        /// 將產生出來的過程寫出去
        /// </summary>
        /// <param name="trainstatuscollection"></param>
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
                if(filltoanalyzestr == null)
                {
                    if(trainstatus.LogString == "")
                    {
                        trainstatuscollection.Add(trainstatus);
                    }
                }
                else 
                {
                    if (trainstatus.LogString.IndexOf(filltoanalyzestr) < 0)
                    {
                        trainstatus.LogString += filltoanalyzestr;
                        trainstatuscollection.Add(trainstatus);
                    }
                }
            }
        }
        public void AddTrainLogString(string logstr)
        {
            foreach(WorkStatusClass works in TrainStatusCollection.WorkStatusList)
            {
                if(works.LogString.IndexOf(logstr) < 0)
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
        public bool AuFindSimilar(Bitmap bmpinput,int maxocc,float tolerance,List<DoffsetClass> doffsetlist)
        {
            bool ret = false;

            AUFIND.SetMaxOcc(maxocc);
            AUFIND.SetTolerance(tolerance);

            AUGrayImg8 imginput = new AUGrayImg8();
            AUUtility.DrawBitmapToAUGrayImg8(bmpinput, ref imginput);
            
            int resultcount = AUFIND.Find(imginput);

            xFindResult result = new xFindResult();
            
            int i = 0;

            Rectangle rect = new Rectangle(0, 0, bmpPattern.Width, bmpPattern.Height);

            //bmpRunOutput.Dispose();
            //bmpRunOutput = new Bitmap(bmpinput);

            while (i < resultcount)
            {   
                AUFIND.GetResult(out result, i);

                if (result.fScore >= tolerance)
                {
                    if (IsRectBounded(rect, new Rectangle(0, 0, bmpinput.Width, bmpinput.Height), new PointF(result.fCenterX, result.fCenterY)))
                    {
                        //DrawRect(bmpRunOutput, rect, new PointF(result.fCenterX, result.fCenterY), new Pen(Color.Red, 10));

                        DoffsetClass doffset = new DoffsetClass(result.fAngle, new PointF(result.fCenterX, result.fCenterY));
                        doffsetlist.Add(doffset);
                    }
                }

                i++;
            }

            //bmpRunOutput.Save(Universal.TESTPATH + "\\ANALYZETEST\\FOUNDRESULT" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            
            return ret;
        }
        #endregion

        #region Tools Operation
        Rectangle SimpleRect(Size sz, int devide)
        {
            return new Rectangle(0, 0, sz.Width / devide, sz.Height / devide);
        }
        void SetBrightContrast(Bitmap bmp, int brightvalue, int contrastvalue)
        {
            SetBrightContrast(bmp, SimpleRect(bmp.Size, 1), brightvalue, contrastvalue);
        }
        void SetBrightContrast(Bitmap bmp, Rectangle rect, int brightvalue, int contrastvalue)
        {
            if (brightvalue == 0 && contrastvalue == 0)
                return;

            int Grade = 0;
            double contrast = (100.0 + contrastvalue) / 100.0;
            contrast *= contrast;

            double ContrastGrade = 0;

            Rectangle rectbmp = rect;
            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr Scan0 = bmpData.Scan0;

            try
            {
                unsafe
                {
                    byte* scan0 = (byte*)(void*)Scan0;
                    byte* pucPtr;
                    byte* pucStart;

                    int xmin = rectbmp.X;
                    int ymin = rectbmp.Y;
                    int xmax = xmin + rectbmp.Width;
                    int ymax = ymin + rectbmp.Height;

                    int x = xmin;
                    int y = ymin;
                    int iStride = bmpData.Stride;

                    y = ymin;
                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                    while (y < ymax)
                    {
                        x = xmin;
                        pucPtr = pucStart;
                        while (x < xmax)
                        {
                            Grade = (int)pucPtr[2];

                            if (brightvalue != 0)
                            {
                                Grade += brightvalue;
                                Grade = Math.Min(255, Math.Max(0, Grade));
                            }

                            ContrastGrade = (double)Grade;

                            if (contrastvalue != 0)
                            {
                                ContrastGrade /= 255d;
                                ContrastGrade -= 0.5;
                                ContrastGrade *= (double)contrast;
                                ContrastGrade += 0.5;
                                ContrastGrade *= 255;

                                ContrastGrade = Math.Min(255, Math.Max(0, ContrastGrade));
                            }

                            pucPtr[2] = (byte)ContrastGrade;
                            pucPtr[1] = (byte)ContrastGrade;
                            pucPtr[0] = (byte)ContrastGrade;

                            pucPtr += 4;
                            x++;
                        }

                        pucStart += iStride;
                        y++;
                    }

                    bmp.UnlockBits(bmpData);
                }
            }
            catch (Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
                string Str = ex.ToString();

                bmp.UnlockBits(bmpData);
            }
        }
        void DrawRect(Bitmap bmp, Rectangle rect, SolidBrush B)
        {
            Graphics g = Graphics.FromImage(bmp);
            g.FillRectangle(B, rect);
            g.Dispose();
        }
        void DrawRect(Bitmap bmp, Rectangle rect, PointF centerf, Pen p)
        {
            Graphics g = Graphics.FromImage(bmp);

            rect.Location = new Point((int)centerf.X,(int)centerf.Y);
            rect.X = rect.X - (rect.Width / 2);
            rect.Y = rect.Y - (rect.Height / 2);

            g.DrawRectangle(p, rect);
            g.Dispose();

        }
        bool IsRectBounded(Rectangle rect,Rectangle boundrect,PointF centerf)
        {
            rect.Location = new Point((int)centerf.X, (int)centerf.Y);
            rect.X = rect.X - (rect.Width / 2);
            rect.Y = rect.Y - (rect.Height / 2);

            Rectangle recttmp = rect;

            recttmp.Intersect(boundrect);
            
            return rect == recttmp;
        }

        void SaveData(string DataStr, string FileName)
        {
            File.WriteAllText(FileName, DataStr, Encoding.Default);
        }

        public string ToAlignParaString()
        {
            string str = "";

            str += "DownSample: " + AUTrainInfoF.nDownSamplingSize.ToString() + Environment.NewLine;
            str += "CannyAuto:" + (MTCannyAuto ? "Y" : "N") + Environment.NewLine;
            str += "CannyHigh:" + AUTrainInfoF.nCannyThresholdHigh.ToString() + Environment.NewLine;
            str += "CannyLow:" + AUTrainInfoF.nCannyThresholdLow.ToString() + Environment.NewLine;
            str += "Rotation:" + AUTrainInfoF.fRotationTolerance.ToString() + Environment.NewLine;
            str += "Scaling:" + AUTrainInfoF.fScalingTolerance.ToString() + Environment.NewLine;
            str += "eFMode:" + AUTrainInfoF.eFMode.ToString() + Environment.NewLine;
            str += "MaxOCC:" + MTMaxOcc.ToString() + Environment.NewLine;
            str += "Tolerance:" + MTTolerance.ToString();

            return str;
        }

        public double GetPointLength(PointF P1, PointF P2)
        {
            return Math.Sqrt((double)Math.Pow((P1.X - P2.X), 2) + Math.Pow((P1.Y - P2.Y), 2));
        }

        #endregion
    }
}
