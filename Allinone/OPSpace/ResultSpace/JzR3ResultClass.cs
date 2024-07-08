using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

using JetEazy;
using JetEazy.BasicSpace;
using JetEazy.FormSpace;
using JetEazy.ControlSpace;
using Allinone.ControlSpace;
using Allinone.ControlSpace.MachineSpace;
using JzMSR.OPSpace;

using AllinOne.Jumbo.Net;
using AllinOne.Jumbo.Net.Common;
using Newtonsoft.Json;
using System.Collections;
using System.Media;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Collections.Concurrent;
using JETLIB;
using System.Diagnostics;
//using MoveGraphLibrary;

namespace Allinone.OPSpace.ResultSpace
{
    public class JzR3ResultClass : GeoResultClass
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr _lopen(string lpPathName, int iReadWrite);
        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);
        public const int OF_READWRITE = 2;
        public const int OF_SHARE_DENY_NONE = 0x40;
        public readonly IntPtr HFILE_ERROR = new IntPtr(-1);

        public string BARCODE = "";
        public string VER = "";
        public string Vendor = "";
        public string Colour = "";

        public string ARTWORKNAME = "";
        public string MODELNAME = "";
        public string ORGBARCODESTR = "";
        public string HOUSINGID = "";

        //  public string RELATECOLORSTR = "";
        public string SNSTARTOPSTR
        {
            get
            {
                return VER + "$" + ARTWORKNAME + "-" + RELATECOLORSTR+"_"+Vendor+"_"+Colour;
            }
        }

        JzR3MachineClass MACHINE;

        JzToolsClass JzTools = new JzToolsClass();
        SoundPlayer PlayerPass = new SoundPlayer();
        SoundPlayer PlayerFail = new SoundPlayer();


        QvLineFit RLine = new QvLineFit();
        QvLineFit DOWNLine = new QvLineFit();
        QvLineFit LLine = new QvLineFit();
        Stopwatch runwatch = new Stopwatch();
        Stopwatch runwatchline = new Stopwatch();
        public JzR3ResultClass(Result_EA resultea, VersionEnum version, OptionEnum option, MachineCollectionClass machinecollection)
        {
            myResultEA = resultea;
            VERSION = version;
            OPTION = option;

            PlayerPass.SoundLocation = Universal.PlayerPASSPATH;
            PlayerFail.SoundLocation = Universal.PlayerFAILPATH;
            PlayerPass.Load();
            PlayerFail.Load();

            DUP = new DupClass();

            MACHINE = (JzR3MachineClass)machinecollection.MACHINE;

            MainProcess = new ProcessClass();
        }

        private void CCDCollection_TriggerAction(string operationstr)
        {
            throw new NotImplementedException();
        }

        public override void GetStart(AlbumClass albumwork, CCDCollectionClass ccdcollection, TestMethodEnum testmethod, bool isnouseccd)
        {
            if (MainProcess.IsOn)
            {
                MainProcess.Stop();

                if (INI.ISHIVECLIENT)
                {
                    MACHINE.SetMachineState(MachineState.Idle);
                }

                OnTrigger(ResultStatusEnum.FORECEEND);

                return;
            }

            OnTrigger(ResultStatusEnum.CALSTART);

            AlbumWork = albumwork;
            if (AlbumWork != null && AlbumWork.CPD != null)
                AlbumWork.CPD.bmpOCRCheckErr = null;
            CCDCollection = ccdcollection;

            TestMethod = testmethod;
            IsNoUseCCD = isnouseccd;

            ResetData(-1);

            MainProcess.Start();

            switch (TestMethod)
            {
                case TestMethodEnum.QSMCSF:
                    if (INI.ISHIVECLIENT)
                    {
                        MACHINE.SetMachineState(MachineState.Running);
                        if (MACHINE.GetCurrentMachineState == MachineState.Running)
                            MACHINE.HIVECLIENT.Hiveclient_ConfigurationMap(BARCODE, "SF", INI.DATA_Program, INI.DATA_Building_Config, _get_qsmc_sndata_json());
                    }
                    break;
            }

        }
        public override void Tick()
        {
            if (!IsNoUseCCD && MACHINE.PLCIO.IsUPSError)
            {
                if (MainProcess.IsOn)
                {
                    IsStopNormalTick = false;
                    MainProcess.Stop();

                    MessageBox.Show("UPS Error 请检查UPS 或相关接线！");
                }
            }
            FOXCONNTick();
            MainProcessTick();

            switch (OPTION)
            {
                case OptionEnum.R3:
                    QSMCSFR3Tick();
                    break;
            }


            if(Universal.watchR3RyPass.IsRunning)
            {
                if(Universal.watchR3RyPass.ElapsedMilliseconds>(3000*60))
                {
                    Universal.watchR3RyPass.Stop();
                    Universal.watchR3RyPass.Reset();
                    Universal.isR3ByPass = false;
                    Universal.OnR3TickStop("2");
                }
            }
        }
        public override void GenReport()
        {

        }
        public override void SetDelayTime()
        {
            DelayTime[0] = INI.DELAYTIME;
        }

        JzTimes TestTimer = new JzTimes();
        int[] Testms = new int[100];
        DateTime m_input_time = DateTime.Now;

        protected override void MainProcessTick()
        {
            switch (OPTION)
            {
                case OptionEnum.R3:
                    R3Tick();
                    break;
            }


        }


        int icheckPar = 0;
        bool CheckGAP()
        {
            try
            {
                EnvClass env = AlbumWork.ENVList[EnvIndex];

                bool isresult = true;

                bool isgetbitmap = false;
                Bitmap bmpbarcode = new Bitmap(1, 1);
                Rectangle rectBarcode = new Rectangle();
                int index = 0;
                PointF offsetTemp = new PointF();
                foreach (PageClass page in env.PageList)
                {
                    AnalyzeClass analyzes = page.AnalyzeRootArray[0];
                    offsetTemp = analyzes.ALIGNPara.AlignOffset;
                    bool isok = FindFailRect(analyzes, page.bmpRUN[0], ref rectBarcode, ref bmpbarcode, ref offsetTemp);

                    //        bmpbarcode.Save("D:\\barcode_"+ index+".png");
                    index++;
                    if (isok)
                    {
                        icheckPar++;
                        isgetbitmap = true;
                        break;
                    }
                }

                if (!isgetbitmap)
                {
                    icheckPar = 6;
                    checkGapFail();
                    return false;
                }

                foreach (PageClass page in env.PageList)
                {
                    AnalyzeClass analyzes = page.AnalyzeRootArray[0];
                    foreach (AnalyzeClass anly in analyzes.BranchList)
                    {

                        if (anly.GAPPara.GapMethod == GapMethodEnum.LL)
                        {
                            Universal.R3UI.bmpL = (Bitmap)page.bmpRUN[0].Clone();
                            icheckPar++;
                        }
                        if (anly.GAPPara.GapMethod == GapMethodEnum.RR)
                        {
                            Universal.R3UI.bmpR = (Bitmap)page.bmpRUN[0].Clone();
                            icheckPar++;
                        }
                        if (anly.OCRPara.OCRMethod == OCRMethodEnum.CODE128)
                        {
                            RectangleF rectf = anly.myOPRectF;
                            Bitmap bmp = page.bmpRUN[0].Clone(rectf, PixelFormat.Format32bppArgb);

                            Universal.R3UI.bmpBarcode = bmp;
                            icheckPar++;
                        }
                        //if (anly.OCRPara.OCRMethod == OCRMethodEnum.MAPPING)
                        //{
                        //    RectangleF rectf = anly.myOPRectF;

                        //}
                        //if (anly.GAPPara.GapMethod == GapMethodEnum.ST)
                        //{

                        //}
                        if (anly.GAPPara.GapMethod == GapMethodEnum.SN)
                        {
                            RectangleF rectf = anly.myOPRectF;
                            Universal.R3UI.RectSN = rectf;
                            icheckPar++;
                        }

                        if (anly.GAPPara.GapMethod == GapMethodEnum.ST)
                        {
                            icheckPar++;
                            RectangleF rectf = anly.myOPRectF;
                            Universal.R3UI.RectST = rectf;

                            Bitmap bmp = page.bmpRUN[0].Clone(rectf, PixelFormat.Format32bppArgb);
                            Universal.R3UI.bmpC = (Bitmap)page.bmpRUN[0].Clone();

                            Bitmap bitmap = new Bitmap(1, 1);
                            myImageProcessor.Balance(bmp, ref bitmap, myImageProcessor.EnumThreshold.Minimum);

                            //      bitmap = new Bitmap(bitmap);

                            //    bitmap.Save("D://feind.png");

                            //JzFindObjectClass find = new JzFindObjectClass();
                            //find.SetThreshold(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0, 10, true);
                            //find.Find(bitmap, Color.Red);

                            List<Rectangle> listrect = new List<Rectangle>();
                            Graphics gg = Graphics.FromImage(bmp);

                            JetGrayImg grayimage = new JetGrayImg(bitmap);
                            JetImgproc.Threshold(grayimage, 10, grayimage);
                            JetBlob jetBlob = new JetBlob();
                            jetBlob.Labeling(grayimage, JConnexity.Connexity4, JBlobLayer.BlackLayer);
                            int icount = jetBlob.BlobCount;

                            for (int i = 0; i < icount; i++)
                            {
                                int iArea = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.Area);
                                if (iArea > 100)
                                {
                                    //JRotatedRectangleF jetrect = JetBlobFeature.ComputeMinRectangle(jetBlob, i);

                                    int itop = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.TopMost);
                                    int iLeft = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.LeftMost);
                                    int iRight = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.RightMost);
                                    int iBottom = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.BottomMost);

                                    Rectangle rect = new Rectangle(iLeft, itop, iRight - iLeft, iBottom - itop);

                                    listrect.Add(rect);
                                    gg.DrawRectangle(new Pen(Color.Red, 2), rect);
                                }
                            }



                            PointF a = new PointF(listrect[0].X, listrect[0].Y);
                            PointF b = new PointF(listrect[0].X, listrect[0].Y);

                            int Top = listrect[0].Y;
                            int Down = listrect[0].Y + listrect[0].Height;
                            int Left = listrect[0].X;
                            int Reiht = listrect[0].X + listrect[0].Width;
                            for (int i = 1; i < listrect.Count; i++)
                            {
                                Rectangle recttemp = listrect[i];
                                if (Top > recttemp.Y)
                                    Top = recttemp.Y;
                                if (Down < recttemp.Y + recttemp.Height)
                                    Down = recttemp.Y + recttemp.Height;
                                if (Left > recttemp.X)
                                {
                                    Left = recttemp.X;
                                    a = new Point(recttemp.X, recttemp.Y + recttemp.Height / 2);
                                }
                                if (Reiht < recttemp.X + recttemp.Width)
                                {
                                    Reiht = recttemp.X + recttemp.Width;
                                    b = new Point(recttemp.X + recttemp.Width, recttemp.Y + recttemp.Height / 2);
                                }
                            }
                            gg.DrawLine(new Pen(Color.Blue, 3), a, b);

                            a = new PointF(a.X + rectf.X, a.Y + rectf.Y);
                            b = new PointF(b.X + rectf.X, b.Y + rectf.Y);
                            a = Universal.Correct.CorrestList[1].GetWorld(a);
                            b = Universal.Correct.CorrestList[1].GetWorld(b);
                            //   SortList(listrect);

                            QvLineFit runline = new QvLineFit();
                            PointF[] points = new PointF[2];
                            points[0] = a;
                            points[1] = b;
                            runline.LeastSquareFit(points);

                            PointF Center = new PointF((Left + Reiht) / 2, (Top + Down) / 2);
                            gg.DrawRectangle(new Pen(Color.Yellow, 3), new Rectangle(Left, Top, Reiht - Left, Down - Top));
                            gg.DrawRectangle(new Pen(Color.Yellow, 10), new Rectangle((int)(Center.X) - 5, (int)(Center.Y) - 5, 10, 10));

                            gg.Dispose();


                            Center = new PointF(Center.X + rectf.X, Center.Y + rectf.Y);

                            Bitmap bmpBarcodeTemp = new Bitmap(bmpbarcode);
                            myImageProcessor.Balance(bmpBarcodeTemp, ref bmpBarcodeTemp, myImageProcessor.EnumThreshold.Minimum);
                            JzFindObjectClass find2 = new JzFindObjectClass();
                            find2.SetThreshold(bmpBarcodeTemp, new Rectangle(0, 0, bmpBarcodeTemp.Width, bmpBarcodeTemp.Height), 0, 0, 10, true);
                            find2.Find(bmpBarcodeTemp, Color.Red);

                            //    bmpbarcode.Save("D:\\barcode.png");
                            bmpBarcodeTemp.Dispose();

                            Graphics gg2 = Graphics.FromImage(bmpbarcode);
                            Point poCenter = new Point(0, 0);
                            for (int i = 0; i < find2.FoundList.Count; i++)
                            {
                                //mylist.Add(find.FoundList[i].Area);
                                if (find2.FoundList[i].Area < 100)
                                    continue;

                                if (poCenter.Y < (find2.FoundList[i].rect.Y + find2.FoundList[i].rect.Height))
                                {
                                    poCenter.Y = find2.FoundList[i].rect.Y + find2.FoundList[i].rect.Height;
                                    poCenter.X = find2.FoundList[i].rect.X + find2.FoundList[i].rect.Width / 2;
                                }

                                gg2.DrawRectangle(new Pen(Color.Red, 2), find2.FoundList[i].rect);
                            }
                            Center = new PointF(poCenter.X + rectBarcode.X - offsetTemp.X, poCenter.Y + rectBarcode.Y - offsetTemp.Y);

                            gg2.DrawRectangle(new Pen(Color.Yellow, 10), new Rectangle((int)poCenter.X - 5, (int)poCenter.Y - 5, 10, 10));
                            gg2.Dispose();

                            Center = Universal.Correct.CorrestList[1].GetWorld(Center);
                            double Range = DOWNLine.GetPointLength(Center);
                            Range = Range + INI.fR3DCompensate;

                            double angle = QvLineFit.GetAngle(DOWNLine, runline);

                            PointF pointL = QvLineFit.GetIntersectPoint(LLine, runline);
                            PointF pointR = QvLineFit.GetIntersectPoint(RLine, runline);

                            double RangeL = PointToPoint(pointL, a);
                            double RangeR = PointToPoint(pointR, b);

                            RangeL = RangeL + INI.fR3LCompensate;
                            RangeR = RangeR + INI.fR3RCompensate;

                            if (Range > anly.GAPPara.OffsetToUPMin && Range < anly.GAPPara.OffsetToUPMax)
                                AlbumWork.CPD.mGapResult.ISRange = true;
                            else
                            {
                                AlbumWork.CPD.mGapResult.ISRange = false;
                                isresult = false;
                            }

                            if (Math.Abs(angle) > anly.GAPPara.OffsetAngle)
                            {
                                AlbumWork.CPD.mGapResult.ISANGLE = false;
                                isresult = false;
                            }
                            else
                                AlbumWork.CPD.mGapResult.ISANGLE = true;

                            if (Math.Abs(RangeL - RangeR) > anly.GAPPara.OffsetToLeftRight)
                            {
                                AlbumWork.CPD.mGapResult.ISRangeLR = false;
                                isresult = false;
                            }
                            else
                                AlbumWork.CPD.mGapResult.ISRangeLR = true;

                            AlbumWork.CPD.mGapResult.STRANGLE = "角    度：" + angle.ToString("0.00") + " 度  标准：" + anly.GAPPara.OffsetAngle;
                            AlbumWork.CPD.mGapResult.STRRange = "下边距：" + Range.ToString("0.00") + " 毫米  标准 最大:" + anly.GAPPara.OffsetToUPMax + " 最小:" + anly.GAPPara.OffsetToUPMin;
                            AlbumWork.CPD.mGapResult.STRRangeLR = "左边距：" + RangeL.ToString("0.00") + " 毫米  标准：" + anly.GAPPara.OffsetToLeftRight + Environment.NewLine;
                            AlbumWork.CPD.mGapResult.STRRangeLR += "右边距：" + RangeR.ToString("0.00") + " 毫米  标准：" + anly.GAPPara.OffsetToLeftRight;

                            if (INI.ISSAVEDebugIMAGE)
                            {
                                string path = "d:\\TestTest\\Gap\\";
                                if (!Directory.Exists(path))
                                    Directory.CreateDirectory(path);

                                bmp.Save(path + "GapLine.png");
                                bmpbarcode.Save(path + "Center.png");
                            }
                            bmp.Dispose();
                            bmpbarcode.Dispose();

                            string path2 = "d:\\Report\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
                            if (!Directory.Exists(path2))
                                Directory.CreateDirectory(path2);

                            string strMess = "角度," + angle.ToString("0.00") + Environment.NewLine;
                            strMess += "下边距," + Range.ToString("0.00") + Environment.NewLine;
                            strMess += "左边距," + RangeL.ToString("0.00") + Environment.NewLine;
                            strMess += "右边距," + RangeR.ToString("0.00") + Environment.NewLine;

                            string strFilePath = path2;
                            if (JzToolsClass.PassingBarcode != "")
                                strFilePath += JzToolsClass.PassingBarcode + "_" + DateTime.Now.ToString("HHmmss") + ".csv";
                            else
                                strFilePath += DateTime.Now.ToString("HHmmss") + ".csv";

                            JzTools.SaveData(strMess, strFilePath);

                            string strMessA = "条码,生产时间,角度,";
                            strMessA += "下边距,";
                            strMessA += "左边距,";
                            strMessA += "右边距,";
                            strMessA += "结果";


                            bool ispassing = true;

                            if (!IsPass)
                                ispassing = false;
                            if (!isresult)
                                ispassing = false;

                            string strMessB = "";
                            strMessB += JzToolsClass.PassingBarcode;
                            strMessB += ",'" + DateTime.Now.ToString("yyyyMMddHHmmss");
                            strMessB += "," + angle.ToString("0.00");
                            strMessB += "," + Range.ToString("0.00");
                            strMessB += "," + RangeL.ToString("0.00");
                            strMessB += "," + RangeR.ToString("0.00");
                            strMessB += "," + (ispassing ? "PASS" : "FAIL");


                            string pathFile = path2 + "AllReport.csv";
                            if (!File.Exists(pathFile))
                                JzTools.SaveDataEXD(strMessA, pathFile);

                            JzTools.SaveDataEXD(strMessB, pathFile);

                            string Qsmcpath = INI.APPLERESURT + "\\REPORTS\\" + JzTimes.DateSerialString + "\\";
                            if (!Directory.Exists(Qsmcpath))
                                Directory.CreateDirectory(Qsmcpath);
                            string strFilePathReport = Qsmcpath;
                            if (JzToolsClass.PassingBarcode != "")
                                strFilePathReport += JzToolsClass.PassingBarcode + "_" + DateTime.Now.ToString("HHmmss") + ".csv";
                            else
                                strFilePathReport += DateTime.Now.ToString("HHmmss") + ".csv";

                            JzTools.SaveData(strMess, strFilePathReport);


                        }

                    }
                }

                return isresult;
            }
            catch(Exception ex)
            {
                checkGapFail();
                return false;
            }
        }

        bool checkGapFail()
        {
            AlbumWork.CPD.mGapResult.STRANGLE = "角    度：" + " null";
            AlbumWork.CPD.mGapResult.STRRange = "下边距：" + " null";
            AlbumWork.CPD.mGapResult.STRRangeLR = "左边距：" + " null"+ Environment.NewLine; 
            AlbumWork.CPD.mGapResult.STRRangeLR += "右边距：" + " null";

            AlbumWork.CPD.mGapResult.ISANGLE = false;
            AlbumWork.CPD.mGapResult.ISRange = false;
            AlbumWork.CPD.mGapResult.ISRangeLR = false;


            string path2 = "d:\\Report\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
            if (!Directory.Exists(path2))
                Directory.CreateDirectory(path2);

            string strMess = "角度," + "null" + Environment.NewLine;
            strMess += "下边距," + "null" + Environment.NewLine;
            strMess += "左边距," + "null" + Environment.NewLine;
            strMess += "右边距," + "null" + Environment.NewLine;

            string strFilePath = path2;
            if (JzToolsClass.PassingBarcode != "")
                strFilePath += JzToolsClass.PassingBarcode + "_" + DateTime.Now.ToString("HHmmss") + ".csv";
            else
                strFilePath += DateTime.Now.ToString("HHmmss") + ".csv";

            JzTools.SaveData(strMess, strFilePath);

            string strMessA = "条码,生产时间,角度,";
            strMessA += "下边距,";
            strMessA += "左边距,";
            strMessA += "右边距,";
            strMessA += "结果";


            bool ispassing = false;

            string strMessB = "";
            strMessB += JzToolsClass.PassingBarcode;
            strMessB += ",'" + DateTime.Now.ToString("yyyyMMddHHmmss");
            strMessB += "," + "null";
            strMessB += "," + "null";
            strMessB += "," + "null";
            strMessB += "," + "null";
            strMessB += "," + (ispassing ? "PASS" : "FAIL");


            string pathFile = path2 + "AllReport.csv";
            if (!File.Exists(pathFile))
                JzTools.SaveDataEXD(strMessA, pathFile);

            JzTools.SaveDataEXD(strMessB, pathFile);

            string Qsmcpath = INI.APPLERESURT + "\\REPORTS\\" + JzTimes.DateSerialString + "\\";
            if (!Directory.Exists(Qsmcpath))
                Directory.CreateDirectory(Qsmcpath);
            string strFilePathReport = Qsmcpath;
            if (JzToolsClass.PassingBarcode != "")
                strFilePathReport += JzToolsClass.PassingBarcode + "_" + DateTime.Now.ToString("HHmmss") + ".csv";
            else
                strFilePathReport += DateTime.Now.ToString("HHmmss") + ".csv";

            JzTools.SaveData(strMess, strFilePathReport);

            return false;
        }

        bool FindFailRect(AnalyzeClass anly, Bitmap bmpRun, ref Rectangle rectBarcode, ref Bitmap bmpbarcode, ref PointF offset )
        {
            if (anly.OCRPara.OCRMethod == OCRMethodEnum.MAPPING)
            {
                MoveGraphLibrary.Mover mover = anly.myMover;
                if (mover.Count > 0)
                {
                    MoveGraphLibrary.GraphicalObject grobj = mover[0].Source;

                    if (grobj is WorldOfMoveableObjects.JzRectEAG)
                    {
                        WorldOfMoveableObjects.JzRectEAG rectEAG = (WorldOfMoveableObjects.JzRectEAG)grobj;
                        rectBarcode = rectEAG.GetRect;
                      //  rectBarcode.Inflate(new Size(0, 5));
                        //        bmpbarcode = bmpRun.Clone(rectBarcode, PixelFormat.Format24bppRgb);
                        bmpbarcode = anly.bmpOUTPUT;

                        offset = new PointF(offset.X + anly.ALIGNPara.AlignOffset.X, offset.Y + anly.ALIGNPara.AlignOffset.Y);

                        return true; 
                    }
                }
            }
            if (anly.OCRPara.OCRMethod == OCRMethodEnum.CHICKLINE)
            {
                MoveGraphLibrary.Mover mover = anly.myMover;
                if (mover.Count > 0)
                {
                    MoveGraphLibrary.GraphicalObject grobj = mover[0].Source;

                    if (grobj is WorldOfMoveableObjects.JzRectEAG)
                    {
                        WorldOfMoveableObjects.JzRectEAG rectEAG = (WorldOfMoveableObjects.JzRectEAG)grobj;
                        rectBarcode = rectEAG.GetRect;
                //        rectBarcode.Inflate(new Size(0, 5));
               //         bmpbarcode = bmpRun.Clone(rectBarcode, PixelFormat.Format24bppRgb);
                        bmpbarcode = anly.bmpOUTPUT;
                        offset = new PointF(offset.X + anly.ALIGNPara.AlignOffset.X, offset.Y + anly.ALIGNPara.AlignOffset.Y);

                        return true; 
                    }
                }
            }

            foreach (AnalyzeClass analyze in anly.BranchList)
            {
                offset = new PointF(offset.X + analyze.ALIGNPara.AlignOffset.X, offset.Y + analyze.ALIGNPara.AlignOffset.Y);
                bool isok = FindFailRect(analyze, bmpRun, ref rectBarcode, ref bmpbarcode ,ref  offset);
                if (isok)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 点到点间的距离
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        double PointToPoint(PointF p1, PointF p2)
        {
            double value = Math.Sqrt(Math.Abs(p1.X - p2.X) * Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y) * Math.Abs(p1.Y - p2.Y));

            return value;
        }

        void SortList(List<Rectangle> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int ix = list[i].X + list[i].Width / 2;
                for (int j = i + 1; j < list.Count; j++)
                {
                    int ix2 = list[j].X + list[j].Width / 2;
                    if (ix > ix2)
                    {
                        Rectangle rect = list[i];
                        list[i] = list[j];
                        list[j] = rect;
                        ix = list[i].X + list[i].Width / 2;
                    }
                }
            }
        }

        bool FindLine()
        {
            EnvClass env = AlbumWork.ENVList[EnvIndex];


            AnalyzeClass L = null, LD = null, R = null, RD = null;
            foreach (PageClass page in env.PageList)
            {
                AnalyzeClass analyzes = page.AnalyzeRootArray[0];

                foreach (AnalyzeClass anly in analyzes.BranchList)
                {
                    if (anly.GAPPara.GapMethod == GapMethodEnum.LL)
                        L = anly;
                    if (anly.GAPPara.GapMethod == GapMethodEnum.LD)
                        LD = anly;

                    if (anly.GAPPara.GapMethod == GapMethodEnum.RR)
                        R = anly;
                    if (anly.GAPPara.GapMethod == GapMethodEnum.RD)
                        RD = anly;

                }
            }
            bool isset = false;
            if (L != null && LD != null && R != null && RD != null)
                isset = true;

            if (!isset)
                return isset;

            Bitmap bmpL = L.bmpOUTPUT;
            Bitmap bmpLD = LD.bmpOUTPUT;
            Bitmap bmpR = R.bmpOUTPUT;
            Bitmap bmpRD = RD.bmpOUTPUT;

            if (INI.ISSAVEDebugIMAGE)
            {
                string path = "d:\\TestTest\\Gap\\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                bmpL.Save(path + "L1.png");
                bmpLD.Save(path + "LD1.png");
                bmpR.Save(path + "R1.png");
                bmpRD.Save(path + "RD1.png");
            }

            myImageProcessor.Balance(bmpL, ref bmpL, myImageProcessor.EnumThreshold.IsoData);
            myImageProcessor.Balance(bmpLD, ref bmpLD, myImageProcessor.EnumThreshold.IsoData);
            myImageProcessor.Balance(bmpR, ref bmpR, myImageProcessor.EnumThreshold.IsoData);
            myImageProcessor.Balance(bmpRD, ref bmpRD, myImageProcessor.EnumThreshold.IsoData);


            PointF[] points = FindLine_L(bmpL, 5);

            //排除50% 的点
            QvLineFit LLineTemp = new QvLineFit();
            LLineTemp.LeastSquareFit(points);
            List<TempLine> TempPoint = new List<TempLine>();
            for (int i = 0; i < points.Length; i++)
            {
                TempLine pointtemp = new TempLine();
                pointtemp.myopoit = points[i];
                pointtemp.Lengt = LLineTemp.GetPointLength(points[i]);
                TempPoint.Add(pointtemp);
            }
            for (int i = 0; i < TempPoint.Count; i++)
            {
                for (int j = i + 1; j < TempPoint.Count; j++)
                {
                    if (TempPoint[i].Lengt > TempPoint[j].Lengt)
                    {
                        TempLine pointtemp = TempPoint[i];
                        TempPoint[i] = TempPoint[j];
                        TempPoint[j] = pointtemp;
                    }
                }
            }
            int itemplength = TempPoint.Count / 2;

            if (INI.ISSAVEDebugIMAGE)
            {
                Bitmap bmpLTemp = new Bitmap(L.bmpOUTPUT);

                Graphics gl = Graphics.FromImage(bmpLTemp);
                for (int i = 0; i < itemplength; i++)
                {
                    PointF poif = TempPoint[i].myopoit;
                    Rectangle rect = new Rectangle((int)poif.X - 1, (int)poif.Y - 1, 3, 3);
                    gl.DrawRectangle(new Pen(Color.Red, 1), rect);
                }
                gl.Dispose();

                string path = "d:\\TestTest\\Gap\\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                bmpLTemp.Save(path + "L_Run.png");
                bmpLTemp.Dispose();
            }

            PointF[] point = new PointF[itemplength];
            for (int i = 0; i < itemplength; i++)
                point[i] = new PointF(L.myOPRectF.X + TempPoint[i].myopoit.X, L.myOPRectF.Y + TempPoint[i].myopoit.Y);

            TempPoint.Clear();
            LLineTemp = null;

            point = ViewForWord(point, 0);
            LLine.LeastSquareFit(point);


            PointF[] points2 = FindLine_R(bmpR, 5);

            //排除50% 的点
            LLineTemp = new QvLineFit();
            LLineTemp.LeastSquareFit(points2);
            TempPoint = new List<TempLine>();
            for (int i = 0; i < points2.Length; i++)
            {
                TempLine pointtemp = new TempLine();
                pointtemp.myopoit = points2[i];
                pointtemp.Lengt = LLineTemp.GetPointLength(points2[i]);
                TempPoint.Add(pointtemp);
            }
            for (int i = 0; i < TempPoint.Count; i++)
            {
                for (int j = i + 1; j < TempPoint.Count; j++)
                {
                    if (TempPoint[i].Lengt > TempPoint[j].Lengt)
                    {
                        TempLine pointtemp = TempPoint[i];
                        TempPoint[i] = TempPoint[j];
                        TempPoint[j] = pointtemp;
                    }
                }
            }
            itemplength = TempPoint.Count / 2;

            if (INI.ISSAVEDebugIMAGE)
            {
                Bitmap bmpRTemp = new Bitmap(R.bmpOUTPUT);

                Graphics gl = Graphics.FromImage(bmpRTemp);
                for (int i = 0; i < itemplength; i++)
                {
                    PointF poif = TempPoint[i].myopoit;
                    Rectangle rect = new Rectangle((int)poif.X - 1, (int)poif.Y - 1, 3, 3);
                    gl.DrawRectangle(new Pen(Color.Red, 1), rect);
                }
                gl.Dispose();

                string path = "d:\\TestTest\\Gap\\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                bmpRTemp.Save(path + "R_Run.png");
                bmpRTemp.Dispose();
            }

            PointF[] point2 = new PointF[itemplength];
            for (int i = 0; i < itemplength; i++)
                point2[i] = new PointF(R.myOPRectF.X + TempPoint[i].myopoit.X, R.myOPRectF.Y + TempPoint[i].myopoit.Y);

            TempPoint.Clear();
            LLineTemp = null;
            //PointF[] point2 = new PointF[points2.Length];
            //for (int i = 0; i < points2.Length; i++)
            //    point2[i] = new PointF(R.myOPRectF.X + points2[i].X, R.myOPRectF.Y + points2[i].Y);

            point2 = ViewForWord(point2, 2);
            RLine.LeastSquareFit(point2);




                PointF[] pointsL = FindLine_DownToUP (bmpLD, 20);
            //PointF[] point3 = new PointF[pointsL.Length];
            //for (int i = 0; i < pointsL.Length; i++)
            //    point3[i] = new PointF(LD.myOPRectF.X + pointsL[i].X, LD.myOPRectF.Y + pointsL[i].Y);
           

            //左下
            //排除50% 的点
            LLineTemp = new QvLineFit();
            LLineTemp.LeastSquareFit(pointsL);
            TempPoint = new List<TempLine>();
            for (int i = 0; i < pointsL.Length; i++)
            {
                TempLine pointtemp = new TempLine();
                pointtemp.myopoit = pointsL[i];
                pointtemp.Lengt = LLineTemp.GetPointLength(pointsL[i]);
                TempPoint.Add(pointtemp);
            }
            for (int i = 0; i < TempPoint.Count; i++)
            {
                for (int j = i + 1; j < TempPoint.Count; j++)
                {
                    if (TempPoint[i].Lengt > TempPoint[j].Lengt)
                    {
                        TempLine pointtemp = TempPoint[i];
                        TempPoint[i] = TempPoint[j];
                        TempPoint[j] = pointtemp;
                    }
                }
            }
            itemplength = TempPoint.Count / 2;

            if (INI.ISSAVEDebugIMAGE)
            {
                Bitmap bmpRTemp = new Bitmap(LD.bmpOUTPUT);

                Graphics gl = Graphics.FromImage(bmpRTemp);
                for (int i = 0; i < itemplength; i++)
                {
                    PointF poif = TempPoint[i].myopoit;
                    Rectangle rect = new Rectangle((int)poif.X - 1, (int)poif.Y - 1, 3, 3);
                    gl.DrawRectangle(new Pen(Color.Red, 1), rect);
                }
                gl.Dispose();

                string path = "d:\\TestTest\\Gap\\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                bmpRTemp.Save(path + "LD_Run.png");
                bmpRTemp.Dispose();
            }

            PointF[] point5 = new PointF[itemplength];
            for (int i = 0; i < itemplength; i++)
                point5[i]  = new PointF(LD.myOPRectF.X + TempPoint[i].myopoit.X, LD.myOPRectF.Y + TempPoint[i].myopoit.Y);

            TempPoint.Clear();
            LLineTemp = null;


            point5 = ViewForWord(point5, 0);

            PointF[] pointsR = FindLine_DownToUP(bmpRD, 20);
            //PointF[] point4 = new PointF[pointsR.Length];
            //for (int i = 0; i < pointsR.Length; i++)
            //    point4[i] = new PointF(RD.myOPRectF.X + pointsR[i].X, RD.myOPRectF.Y + pointsR[i].Y);


            //右下
            //排除50% 的点
            LLineTemp = new QvLineFit();
            LLineTemp.LeastSquareFit(pointsR);
            TempPoint = new List<TempLine>();
            for (int i = 0; i < pointsR.Length; i++)
            {
                TempLine pointtemp = new TempLine();
                pointtemp.myopoit = pointsR[i];
                pointtemp.Lengt = LLineTemp.GetPointLength(pointsR[i]);
                TempPoint.Add(pointtemp);
            }
            for (int i = 0; i < TempPoint.Count; i++)
            {
                for (int j = i + 1; j < TempPoint.Count; j++)
                {
                    if (TempPoint[i].Lengt > TempPoint[j].Lengt)
                    {
                        TempLine pointtemp = TempPoint[i];
                        TempPoint[i] = TempPoint[j];
                        TempPoint[j] = pointtemp;
                    }
                }
            }
            itemplength = TempPoint.Count / 2;

            if (INI.ISSAVEDebugIMAGE)
            {
                Bitmap bmpRTemp = new Bitmap(RD.bmpOUTPUT);

                Graphics gl = Graphics.FromImage(bmpRTemp);
                for (int i = 0; i < itemplength; i++)
                {
                    PointF poif = TempPoint[i].myopoit;
                    Rectangle rect = new Rectangle((int)poif.X - 1, (int)poif.Y - 1, 3, 3);
                    gl.DrawRectangle(new Pen(Color.Red, 1), rect);
                }
                gl.Dispose();

                string path = "d:\\TestTest\\Gap\\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                bmpRTemp.Save(path + "RD_Run.png");
                bmpRTemp.Dispose();
            }

            PointF[] point6 = new PointF[itemplength];
            for (int i = 0; i < itemplength; i++)
                point6[i] = new PointF(RD.myOPRectF.X + TempPoint[i].myopoit.X, RD.myOPRectF.Y + TempPoint[i].myopoit.Y);
            TempPoint.Clear();
            LLineTemp = null;

            
            point6 = ViewForWord(point6, 2);

            PointF[] pointsDown = new PointF[point5.Length + point6.Length];
            point5.CopyTo(pointsDown, 0);
            point6.CopyTo(pointsDown, point5.Length);
            DOWNLine.LeastSquareFit(pointsDown);

            if (INI.ISSAVEDebugIMAGE)
            {
                string path = "d:\\TestTest\\Gap\\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                
                bmpL.Save(path + "L.png");
                bmpLD.Save(path + "LD.png");
                bmpR.Save(path + "R.png");
                bmpRD.Save(path + "RD.png");
            }

            #region
            /*
            int y1 = bmpL.Height / 4;
            Point[] Lline = new Point[2];
            Point[] Rline = new Point[2];
            Point LPoint = new Point();
            Point RPoint = new Point();
            Bitmap bmpL2 = new Bitmap(L.bmpPATTERN);
            Bitmap bmpLD2 = new Bitmap(LD.bmpPATTERN);
            Bitmap bmpR2 = new Bitmap(R.bmpPATTERN);
            Bitmap bmpRD2 = new Bitmap(RD.bmpPATTERN);
            for (int i = 1; i < bmpL.Width; i++)
            {
                if (bmpL.GetPixel(i, y1).R > 100)
                {
                    Lline[0] = new Point(i, y1);
                    break;
                }
                bmpL2.SetPixel(i, y1, Color.Red);
            }

            int y2 = bmpL.Height - y1;
            for (int i = 0; i < bmpL.Width; i++)
            {
                if (bmpL.GetPixel(i, y2).R > 100)
                {
                    Lline[1] = new Point(i, y2);
                    break;
                }
                bmpL2.SetPixel(i, y2, Color.Red);
            }

            y1 = bmpR.Height / 4;
            y2 = bmpR.Height - y1;
            bool isoky1 = false, isoky2 = false;
            for (int i = bmpR.Width - 1; i > 0; i--)
            {
                if (bmpR.GetPixel(i, y1).R > 100 && !isoky1)
                {
                    Rline[0] = new Point(i, y1);
                    isoky1 = true;
                }
                if (!isoky1)
                    bmpR2.SetPixel(i, y1, Color.Red);

                if (bmpR.GetPixel(i, y2).R > 100 && !isoky2)
                {
                    Rline[1] = new Point(i, y2);
                    isoky2 = true;
                }
                if (!isoky2)
                    bmpR2.SetPixel(i, y2, Color.Red);

                if (isoky1 && isoky2)
                    break;
            }
            y1 = bmpLD.Width / 2;
            for (int i = bmpLD.Height - 1; i > 0; i--)
            {
                if (bmpLD.GetPixel(y1, i).R > 100)
                {
                    LPoint = new Point(i, y1);
                    break;
                }
                bmpLD2.SetPixel(y1, i, Color.Red);
            }

            y1 = bmpRD.Width / 2;
            for (int i = bmpRD.Height - 1; i > 0; i--)
            {
                if (bmpRD.GetPixel(y1, i).R > 100)
                {
                    RPoint = new Point(i, y1);
                    break;
                }
                bmpRD2.SetPixel(y1, i, Color.Red);
            }


            bmpL2.Save("D:\\LL.png");
            bmpLD2.Save("D:\\LLD.png");
            bmpR2.Save("D:\\RR.png");
            bmpRD2.Save("D:\\RRD.png");
            */
            #endregion

            return isset;
        }

        PointF[] ViewForWord(PointF[] points, int ccdindex)
        {
            PointF[] point = new PointF[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                point[i] = Universal.Correct.CorrestList[ccdindex].GetWorld(points[i]);
            }
            return point;
        }

        /// <summary>
        /// 找左边图像的线
        /// </summary>
        /// <param name="Bmp"></param>
        /// <param name="iGap">间隔</param>
        /// <returns></returns>
        PointF[] FindLine_L(Bitmap Bmp, int iGap)
        {
            PointF[] points = null;
            if (Bmp.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                BitmapData BmpData = Bmp.LockBits(new Rectangle(0, 0, Bmp.Width, Bmp.Height), ImageLockMode.ReadOnly, Bmp.PixelFormat);
                unsafe
                {
                    int X, Y, Width, Height, Stride;
                    byte Blue;
                    byte* Scan0, CurP;

                    Width = BmpData.Width;
                    Height = BmpData.Height;
                    Stride = BmpData.Stride;
                    Scan0 = (byte*)BmpData.Scan0;

                    int count = Height / iGap;

                    if (Height % iGap == 0)
                        count = count - 1;

                    points = new PointF[count];


                    for (Y = 0; Y < count; Y++)
                    {
                        int iY = ((Y * (iGap)) + 1);
                        if (iY < Height)
                            CurP = Scan0 + iY * Stride;
                        else
                            continue;

                        bool isok = false;
                        for (X = 1; X < Width; X++)
                        {
                            Blue = *CurP;

                            if (Blue > 100 && !isok)
                            {
                                *CurP = 200;
                                points[Y] = new PointF(X, iY);
                                isok = true;
                            }
                            else if (Blue < 100 && !isok)
                                *CurP = 255;
                            else
                                *CurP = 0;

                            CurP += 1;

                        }
                    }
                }
                Bmp.UnlockBits(BmpData);

            }
            return points;
        }
        /// <summary>
        /// 找左边图像的线
        /// </summary>
        /// <param name="Bmp"></param>
        /// <param name="iGap">间隔</param>
        /// <returns></returns>
        PointF[] FindLine_R(Bitmap Bmp, int iGap)
        {
            PointF[] points = null;
            if (Bmp.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                BitmapData BmpData = Bmp.LockBits(new Rectangle(0, 0, Bmp.Width, Bmp.Height), ImageLockMode.ReadOnly, Bmp.PixelFormat);
                unsafe
                {
                    int X, Y, Width, Height, Stride;
                    byte Blue;
                    byte* Scan0, CurP;

                    Width = BmpData.Width;
                    Height = BmpData.Height;
                    Stride = BmpData.Stride;
                    Scan0 = (byte*)BmpData.Scan0;

                    int count = Height / iGap;

                    if (Height % iGap == 0)
                        count = count - 1;

                    points = new PointF[count];


                    for (Y = 0; Y < count; Y++)
                    {
                        int iY = ((Y * (iGap)) + 1);
                        if (iY < Height)
                            CurP = Scan0 + iY * Stride;
                        else
                            continue;

                        bool isok = false;
                        for (X = Width - 3; X > 0; X--)
                        {
                            CurP = Scan0 + iY * Stride + X;

                            Blue = *CurP;

                            if (Blue > 100 && !isok)
                            {
                                *CurP = 200;
                                points[Y] = new PointF(X, iY);
                                isok = true;
                            }
                            else if (Blue < 100 && !isok)
                                *CurP = 255;
                            else
                                *CurP = 0;



                        }
                    }
                }
                Bmp.UnlockBits(BmpData);

            }
            return points;
        }

        /// <summary>
        /// 找下边图像的线
        /// </summary>
        /// <param name="Bmp"></param>
        /// <param name="iGap">间隔</param>
        /// <returns></returns>
        PointF[] FindLine_DownToUP(Bitmap Bmp, int iGap)
        {
            PointF[] points = null;
            if (Bmp.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                BitmapData BmpData = Bmp.LockBits(new Rectangle(0, 0, Bmp.Width, Bmp.Height), ImageLockMode.ReadOnly, Bmp.PixelFormat);
                unsafe
                {
                    int X, Y, Width, Height, Stride;
                    byte Blue;
                    byte* Scan0, CurP;

                    Width = BmpData.Width;
                    Height = BmpData.Height;
                    Stride = BmpData.Stride;
                    Scan0 = (byte*)BmpData.Scan0;

                    int count = Width / iGap;

                    if (Height % iGap == 0)
                        count = count - 1;

                    points = new PointF[count];

                    for (X = 0; X < count; X++)
                    {
                        int ix = ((X * (iGap)) + 1);
                        if (ix <= Width)
                            CurP = Scan0 + ix * Stride;
                        else
                            continue;

                        bool isok = false;

                        for (Y = Height - 1; Y > 0; Y--)
                        {
                            CurP = Scan0 + Y * Stride + ix;
                            Blue = *CurP;

                            if (Blue > 100 && !isok)
                            {
                                *CurP = 200;
                                points[X] = new PointF(ix, Y);
                                isok = true;
                            }
                            else if (Blue < 100 && !isok)
                                *CurP = 255;
                            else
                                *CurP = 0;

                        }
                    }
                }
                Bmp.UnlockBits(BmpData);

            }
            return points;
        }


        public override void FillProcessImage()
        {
            EnvClass env = AlbumWork.ENVList[EnvIndex];

            foreach (PageClass page in env.PageList)
            {
                //   if (page.PassInfo.RcpNo != 80002)
                page.SetbmpRUN(PageOPTypeEnum.P00, CCDCollection.GetBMP(page.CamIndex, false));


            }
        }


        public void FillProcessImage(string opstr)
        {
            int i = 0;

            EnvClass env = AlbumWork.ENVList[EnvIndex];

            foreach (PageClass page in env.PageList)
            {
                if ((opstr + ",").IndexOf(page.No.ToString(PageClass.ORGPAGENOSTRING) + ",") > -1)
                    page.SetbmpRUN(PageOPTypeEnum.P00, CCDCollection.GetBMP(page.CamIndex, false));

                i++;
            }
        }
        public void FillProcessImage(string opstr, Bitmap bmp)
        {
            int i = 0;

            EnvClass env = AlbumWork.ENVList[EnvIndex];

            foreach (PageClass page in env.PageList)
            {
                if ((opstr + ",").IndexOf(page.No.ToString(PageClass.ORGPAGENOSTRING) + ",") > -1)
                    page.SetbmpRUN(PageOPTypeEnum.P00, bmp);

                i++;
            }
        }
        public override void ResetData(int operationindex)
        {
            if (operationindex == -1)
            {
                AlbumWork.ResetRunStatus();

                EnvIndex = 0;
                AlbumWork.SetEnvRunIndex(EnvIndex);

                RunStatusCollection.Clear();

                SetDelayTime();
                SetSaveDirectory(Universal.DEBUGRAWPATH);

            }
            else
            {
                EnvIndex = operationindex;
                AlbumWork.SetEnvRunIndex(EnvIndex);
            }
        }

        void QSMCSFR3Tick()
        {
            if (IsStopNormalTick)
                return;

            bool isgotbarcode = false;
            //if (INI.ISQSMCSF)
            {
                string[] Files;

                //bool IsFound = false;

                Files = Directory.GetFiles(INI.SHOPFLOORPATH);

                if (Files.Length > 0)
                {
                    string filestr = "";

                    foreach (string strFile in Files)
                    {
                        if (strFile.ToUpper().IndexOf("\\OCRTEXTING.TXT") > -1)
                        {
                            filestr = strFile;

                            isgotbarcode = TestAndReadData(ref ORGBARCODESTR, strFile);

                            if (isgotbarcode)
                            {
                                ORGBARCODESTR = ORGBARCODESTR.Trim();
                                break;
                            }
                        }
                    }

                    if (isgotbarcode)
                    {
                        try
                        {
                            Universal.DATASNTXT = ORGBARCODESTR;
                            ORGBARCODESTR = ORGBARCODESTR.Replace(Environment.NewLine, ",");
                            ORGBARCODESTR = ORGBARCODESTR.Replace('\r', ',');
                            ORGBARCODESTR += ",,,,,,";

                            VER = ORGBARCODESTR.Split(',')[2].Split(':')[1].Trim();
                            BARCODE = ORGBARCODESTR.Split(',')[0].Split(':')[1].Trim();

                            string[] messs = ORGBARCODESTR.Split(',');
                            foreach (string mess in messs)
                            {
                                if (mess.Length > 2)
                                {
                                    string[] str = mess.Split(':');
                                    {
                                        if (str.Length > 1)
                                        {
                                            if(str[0]== "Vendor")
                                                Vendor = str[1];
                                            
                                            if (str[0] == "Colour")
                                                Colour = str[1];
                                            
                                        }
                                    }
                                }
                            }


                            Universal.R3UI.isSNHaveS = false;
                            if (BARCODE.Length > 3)
                            {
                                if (BARCODE.Substring(0, 4) == "SC02")
                                {
                                    Universal.R3UI.isSNHaveS = true;
                                    BARCODE = BARCODE.Substring(1, BARCODE.Length - 1);
                                }
                                if (BARCODE.Substring(0, 2) == "SH" )
                                {
                                    Universal.R3UI.isSNHaveS = true;
                                    BARCODE = BARCODE.Substring(1, BARCODE.Length - 1);
                                }
                            }
                           
                            JzToolsClass.PassingBarcode = BARCODE;
                            ARTWORKNAME = ORGBARCODESTR.Split(',')[3].Split(':')[1].Trim();


                            Universal.R3UI.isBYPASS = false;
                            if (ORGBARCODESTR.Split(',').Length > 4)
                                Universal.R3UI.isBYPASS = ORGBARCODESTR.Split(',')[4].Split(':')[1].Trim() == "N";


                            //        if (ORGBARCODESTR.Split(',').Length > 3)
                            MODELNAME = ORGBARCODESTR.Split(',')[1].Split(':')[1].Trim();

                            switch (INI.SFFACTORY)
                            {
                                case FactoryShopfloor.FOXCONN:
                                    RELATECOLORSTR = ORGBARCODESTR.Split(',')[6];
                                    HOUSINGID = ORGBARCODESTR.Split(',')[7];
                                    break;
                                default:
                                    RELATECOLORSTR = Universal.COLORTABLE.Check(ORGBARCODESTR.Split(',')[0].Substring(ORGBARCODESTR.Split(',')[0].Trim().Length - 4));

                                    //if (RELATECOLORSTR == "NULL")
                                    //{
                                    //    if (filestr != "")
                                    //        File.Delete(filestr);

                                    //    System.Windows.Forms.MessageBox.Show("无此:' " + BARCODE.Substring(BARCODE.Length - 4, 4) + " ' EEEE CODE,请添加!", "SYS",
                                    //        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                                    //    string strsndata = BARCODE + ",OCR," + JzTimes.DateTimeSerialString + "," + "F";
                                    //    string filename = INI.SHOPFLOORPATH + "\\" + BARCODE + "OCR.txt";
                                    //    JzTools.SaveData(strsndata, filename);

                                    //    return;
                                    //}

                                    bool ischeckok = false;
                                    foreach (JetEazy.DBSpace.RcpClass rcp in Universal.RCPDB.myDataList)
                                    {
                                        if (rcp.Version == VER)
                                        {
                                            ischeckok = true;
                                            break;
                                        }
                                    }
                                    if (!ischeckok)
                                    {
                                        if (filestr != "")
                                            File.Delete(filestr);

                                        System.Windows.Forms.MessageBox.Show("无此:' " + VER + " ' 国别,请联系厂商注册!", "SYS",
                                            System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                                        string strbarcode = "";
                                        if (Universal.R3UI.isSNHaveS)
                                            strbarcode = "S" + BARCODE;
                                        else
                                            strbarcode = BARCODE;

                                        string strsndata = strbarcode + ",OCR," + JzTimes.DateTimeSerialString + "," + "F";
                                        string filename = INI.SHOPFLOORPATH + "\\" + strbarcode + ".txt";
                                        JzTools.SaveData(strsndata, filename);
                                        return;
                                    }

                                    break;
                            }


                            #region Old Code
                            //QSMCSQLSF = new QSMCSQLSFClass(ORGBARCODESTR);
                            //QSMCSQLSF = new QSMCSQLSFClass();

                            //QSMCSQLSF.SetData(SFSQLEnum.UNITSN, BARCODE);
                            //QSMCSQLSF.SetData(SFSQLEnum.MODELNO, MODELNAME);
                            //QSMCSQLSF.SetData(SFSQLEnum.COUNTRYCODE, ORGBARCODESTR.Split(',')[4]);
                            //QSMCSQLSF.SetData(SFSQLEnum.ASSETTAG, ORGBARCODESTR.Split(',')[5]);
                            //REGIONClass.myQSMCSF = QSMCSQLSF.Clone();

                            /*
                            switch (Universal.VER)
                            {
                                default:
                                    switch (Universal.OPTION)
                                    {
                                        default:
                                            VER = ORGBARCODESTR.Split(',')[1].Trim();
                                            BARCODE = ORGBARCODESTR.Split(',')[0].Trim();

                                            if (INI.ISCTOMODE)
                                            {
                                                switch (Universal.VER)
                                                {
                                                    case "R33":
                                                        ORGBARCODESTR += ",,,,,,,,,,,,,,,";

                                                        QSMCKBSQLSF = new QSMCKBOCRSFClass(ORGBARCODESTR);

                                                        VERSION = QSMCKBSQLSF.GetData(KBOCRSFEnum.KBCOUNTRYCODE) + ":" + QSMCKBSQLSF.GetData(KBOCRSFEnum.KBVISABLE);
                                                        BARCODE = ORGBARCODESTR.Split(',')[12].Trim();

                                                        REGIONClass.myQSMCKBSF = QSMCKBSQLSF.Clone();

                                                        break;
                                                    case "R17":
                                                        ARTWORKNAME = ORGBARCODESTR.Split(',')[2].Trim();

                                                        if (ORGBARCODESTR.Split(',').Length > 3)
                                                            MODELNAME = ORGBARCODESTR.Split(',')[3].Trim();

                                                        break;
                                                    case "R27":
                                                        ARTWORKNAME = ORGBARCODESTR.Split(',')[2].Trim();

                                                        if (ORGBARCODESTR.Split(',').Length > 3)
                                                            MODELNAME = ORGBARCODESTR.Split(',')[3].Trim();

                                                        break;

                                                    case "R26":
                                                    case "R32":

                                                        ORGBARCODESTR += ",,,,,,";

                                                        VERSION = ORGBARCODESTR.Split(',')[1].Trim();
                                                        BARCODE = ORGBARCODESTR.Split(',')[0].Trim();

                                                        ARTWORKNAME = ORGBARCODESTR.Split(',')[2].Trim();

                                                        if (ORGBARCODESTR.Split(',').Length > 3)
                                                            MODELNAME = ORGBARCODESTR.Split(',')[3].Trim();

                                                        RELATECOLORSTR = Universal.COLORTABLE.Check(ORGBARCODESTR.Split(',')[0].Substring(ORGBARCODESTR.Split(',')[0].Trim().Length - 4));

                                                        //QSMCSQLSF = new QSMCSQLSFClass(ORGBARCODESTR);
                                                        QSMCSQLSF = new QSMCSQLSFClass();

                                                        QSMCSQLSF.SetData(SFSQLEnum.UNITSN, BARCODE);
                                                        QSMCSQLSF.SetData(SFSQLEnum.MODELNO, MODELNAME);
                                                        QSMCSQLSF.SetData(SFSQLEnum.COUNTRYCODE, ORGBARCODESTR.Split(',')[4]);
                                                        QSMCSQLSF.SetData(SFSQLEnum.ASSETTAG, ORGBARCODESTR.Split(',')[5]);

                                                        REGIONClass.myQSMCSF = QSMCSQLSF.Clone();

                                                        switch (Universal.OPTION)
                                                        {
                                                            case "AUTOBARCODE":
                                                                IsBYDSecondRun = true;
                                                                break;
                                                            case "FOXCONN":
                                                                RELATECOLORSTR = ORGBARCODESTR.Split(',')[6];
                                                                HOUSINGID = ORGBARCODESTR.Split(',')[7];
                                                                break;
                                                            default:
                                                                RELATECOLORSTR = Universal.COLORTABLE.Check(ORGBARCODESTR.Split(',')[0].Substring(ORGBARCODESTR.Split(',')[0].Trim().Length - 4));
                                                                break;
                                                        }

                                                        break;
                                                        //case "R32":
                                                        //    ARTWORKNAME = BARCODE.Split(',')[2].Trim();

                                                        //    if (BARCODE.Split(',').Length > 3)
                                                        //        MODELNAME = BARCODE.Split(',')[3].Trim();

                                                        //    RELATECOLORSTR = Universal.COLORTABLE.Check(BARCODE.Split(',')[0].Substring(BARCODE.Split(',')[0].Trim().Length - 4));

                                                        //    break;
                                                }
                                            }
                                            //VERSION = ORGBARCODESTR.Split(',')[1].Trim();
                                            //BARCODE = ORGBARCODESTR.Split(',')[0].Trim();

                                            break;
                                    }
                                    break;
                            }

                            */
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            JetEazy.LoggerClass.Instance.WriteException(ex);
                            foreach (string filestrrr in Files)
                            {
                                File.Delete(filestrrr);
                            }

                            if (INI.ISFOXCONNSF)
                            {
                                Universal.Memory.Write("E,ErrorSN");
                                //IsSFActive = false;

                                //LogActionClass.LogAction("QSMCTick(),Memory.Write(E-ErrorSN)" + ",IsSFActive = false");
                            }

                            //LogActionClass.LogAction("QSMCTick(),###Exception=" + "SN.txt格式錯誤:" + "/t" + ee.ToString());
                            //MessageBox.Show("SN.txt格式錯誤:" + "/t" + ee.ToString());

                            return;
                        }
                        //if (filestr != "")
                        //{
                        //    //判断文件是否被占用.
                        //    IntPtr vHandle = _lopen(filestr, OF_READWRITE | OF_SHARE_DENY_NONE);
                        //    if (vHandle == HFILE_ERROR)
                        //        System.Threading.Thread.Sleep(200);

                        //    File.Delete(filestr);
                        //}

                        try
                        {
                            Universal.WipeFile(filestr, 100);
                        }
                        catch (Exception ex)
                        {
                            JetEazy.LoggerClass.Instance.WriteException(ex);
                        }

                        IsPass = false;

                        if (INI.ISCHECKQSMCDUP)
                        {
                            if (DUP.CheckIsOK(BARCODE))
                            {
                                OnEnvTrigger(ResultStatusEnum.SNSTART, -1, SNSTARTOPSTR);

                                //bool_SNRun = true;
                                //IsSFActive = true;
                                //OnTrigger(StatusEnum.QSMCSF);
                            }
                            else
                            {
                                MessageForm DUPFRM = new MessageForm("DUPLICATE ID ERROR.", 5);
                                DUPFRM.Show();
                            }
                        }
                        else
                        {
                            //  OnEnvTrigger(ResultStatusEnum.SNSTART, -1, VER);
                            OnEnvTrigger(ResultStatusEnum.SNSTART, -1, SNSTARTOPSTR);
                            //OnTrigger(StatusEnum.COUNTSTART);

                            //bool_SNRun = true;
                            //IsSFActive = true;

                            //OnTrigger(StatusEnum.QSMCSF);
                        }

                        foreach (string myfiles in Files)
                        {
                            if (myfiles.ToUpper().ToUpper().IndexOf("OCR.TXT") > -1)
                            {
                                if (File.Exists(myfiles))
                                    File.Delete(myfiles);
                            }
                        }
                        //IsFound = true;

                        //break;
                    }

                    //else
                    //{
                    //    //IsFound = false;
                    //    //  break;
                    //}
                }
                //if (IsFound)
                //{
                //    LogActionClass.LogAction("QSMCTick(),Find#SN.txt");

                //    //if (Universal.OPTION != "CLIENT")
                //    //{
                //    //    foreach (string filestr in Files)
                //    //    {
                //    //        File.Delete(filestr);
                //    //    }
                //    //}
                //}

            }

        }
        void FOXCONNTick()
        {
            if (INI.ISFOXCONNSF)
            {
                #region FOXCONN MODE
                string st_Read = Universal.Memory.Read();
                if (st_Read != "")
                {
                    try
                    {
                        string[] Mess = st_Read.Split('#');

                        if (Mess[0] == "1-BARCODE")
                        {
                            Universal.Memory.Write("Y,Text");
                            JzTools.SaveData(Mess[1], @"D:\DATA\SN.TXT");

                            //LogActionClass.LogAction("MainTimer:" + Mess[1]);
                            System.Threading.Thread.Sleep(50);
                        }
                    }
                    catch (Exception ex)
                    {
                        JetEazy.LoggerClass.Instance.WriteException(ex);
                        //LogActionClass.LogAction("MainTimer:" + ex.Message);
                    }
                }
                #endregion
            }
        }


        void R3Tick()
        {
            ProcessClass Process = MainProcess;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:
                        SaveLOGNoTeme("");
                        runwatch.Restart();
                        runwatchline.Restart();
                        if (Universal.R3UI.isBYPASS)
                        {
                            SaveLOG("BYPASS");
                            IsPass = true;
                            Process.ID = 40;
                            return;
                        }
                        bool is8001OK = false;
                        foreach (EnvClass env in Universal.ALBCollection.AlbumNow.ENVList)
                        {
                            foreach (PageClass page in env.PageList)
                            {
                                if (page.RelateToRcpNo == 80001 || page.RelateToRcpNo == 80002)
                                {
                                    is8001OK = true;
                                    break;
                                }
                            }
                        }
                        if (!is8001OK)
                        {
                            IsStopNormalTick = false;
                            Process.Stop();

                            SaveLOG("本参数没有对应到 镭雕参数，请相关工程人员确认！");
                            MessageBox.Show("本参数没有对应到 镭雕参数，请相关工程人员确认！");
                            return;
                        }
                        //      Universal.R3UI = new R3UICLASS();
                        Universal.R3UI.IsPass = true;
                        Universal.R3UI.isSNResult = true;
                        Universal.R3UI.isTest = true;
                        Universal.R3UI.isBarcode = true;
                        Universal.R3UI.Barcode1D = "";
                        Universal.ISCHECKSN = false;

                        TestTimer.Cut();

                        m_input_time = DateTime.Now;

                        OnEnvTrigger(ResultStatusEnum.CHANGEDIRECTORY, 0, "");
                        OnTrigger(ResultStatusEnum.COUNTSTART);


                        //把要檢測的東西放進去
                        FillOperaterString(RELATECOLORSTR);
                        Process.NextDuriation = 50;

                        MACHINE.SetLight(Universal.ALBCollection.AlbumNow.ENVList[0].GeneralLight);

                        SaveLOG("完成初步参数整合");

                        if (IsNoUseCCD)
                            Process.ID = 20;
                        else
                            Process.ID = 10;

                        break;
                    case 10:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
                        if (Process.IsTimeup)
                        {
                            OnEnvTrigger(ResultStatusEnum.SETCAMLIGHT, EnvIndex, "ALL");
                            Process.NextDuriation = INI.DELAYTIME;

                            SaveLOG("变换CCD亮度完成 延时:" + INI.DELAYTIME);
                            Process.ID = 20;
                        }
                        break;
                    case 20:
                        if (Process.IsTimeup)       //抓圖
                        {
                            //Testms[0] = TestTimer.msDuriation;
                            //TestTimer.Cut();

                            if (!IsNoUseCCD)
                                OnTrigger(ResultStatusEnum.COUNTSTART);

                            OnEnvTrigger(ResultStatusEnum.CHANGEENVDIRECTORY, EnvIndex, PageOPTypeEnum.P00.ToString());

                            //Testms[1] = TestTimer.msDuriation;
                            //TestTimer.Cut();
                            //      //MACHINE.SetLight("");
                            CCDCollection.GetR3Image();

                            SaveLOG("取像完成");
                            //Testms[1] = TestTimer.msDuriation;
                            //TestTimer.Cut();

                            FillProcessImage();

                            MACHINE.SetLight("");
                            SaveLOG("取像整合完成");
                            //Testms[2] = TestTimer.msDuriation;
                            //TestTimer.Cut();
                            Process.NextDuriation = 0;
                            Process.ID = 25;
                        }
                        break;
                    case 25:
                        if (Process.IsTimeup)
                        {

                            OnEnvTrigger(ResultStatusEnum.SAVEDEBUGRAW, EnvIndex, "-1");
                            SaveLOG("保存图指令SAVEDEBUGRAW完成");

                            if (IsNoUseCCD)
                                OnTrigger(ResultStatusEnum.COUNTSTART);

                            if (Universal.OPTION == JetEazy.OptionEnum.R3)
                                Universal.R3UI.isCheckBarcodeErr = false;
                            
                            AlbumWork.A08_RunProcess(PageOPTypeEnum.P00);
                            AlbumWork.FillRunStatus(RunStatusCollection);

                            //取得Compound 在這個 ENV 裏的資料
                            AlbumWork.CPD.CollectRUNVIEWData(AlbumWork, AlbumWork.ENVList[EnvIndex].No);

                            SaveLOG("运算完成");

                            Testms[0] = TestTimer.msDuriation;
                            IsPass = RunStatusCollection.NGCOUNT == 0;

                            Process.ID = 26;
                        }


                        break;
                    case 26:
                        Process.ID = 30;
                        bool isok = FindLine();
                        SaveLOG("FindLine完成");
                        if (!isok)
                        {
                            IsPass = false;
                            Process.ID = 40;
                            Universal.R3UI.IsPass = IsPass;
                            SaveLOG("参数设定有错，请检查！");
                            MessageBox.Show("参数设定有错，请检查！");
                        }
                        else
                        {
                            icheckPar = 0;

                            if (!CheckGAP())
                                IsPass = false;

                            SaveLOG("GAP 量测完成");

                            if (icheckPar < 5)
                            {
                                if (ARTWORKNAME != "AA" || ARTWORKNAME != "BB")
                                {
                                    if (icheckPar < 3)
                                    {
                                        IsPass = false;
                                        Universal.R3UI.IsPass = IsPass;
                                        Universal.R3UI.isTest = false;
                                        string mess= "国别为:  " + ARTWORKNAME + "  的参数设定有错!" + Environment.NewLine + " 总共需要 6 个项目,当前只有: " + icheckPar + " 个检测项目." + Environment.NewLine + "请检查!";
                                        SaveLOG(mess);
                                        MessageBox.Show(mess);
                                        break;
                                    }
                                }
                                else
                                {
                                    IsPass = false;
                                    Universal.R3UI.IsPass = IsPass;
                                    Universal.R3UI.isTest = false;
                                    string mess = "国别为:  " + ARTWORKNAME + "  的参数设定有错!" + Environment.NewLine + " 总共需要 6 个项目,当前只有: " + icheckPar + " 个检测项目." + Environment.NewLine + "请检查！";
                                   SaveLOG(mess);
                                    MessageBox.Show(mess);
                                    break;
                                }
                            }
                        }

                        Universal.R3UI.IsPass = IsPass;
                        break;
                    case 30:
                        if (Process.IsTimeup)
                        {
                            if (IsPass)
                            {
                                EnvIndex++;
                                if (EnvIndex < AlbumWork.ENVCount)
                                {
                                    ResetData(EnvIndex);
                                    Process.NextDuriation = 50;
                                    Process.ID = 10;

                                    return;
                                }
                            }
                            else
                            {
                                OnEnvTrigger(ResultStatusEnum.SAVENGRAW, EnvIndex, "-1");
                                SaveLOG("保存图指令SAVENGRAW完成");
                            }

                            Process.NextDuriation = 50;

                            SaveLOG("重置参数完成");
                            Process.ID = 40;
                        }
                        break;
                    case 40:

                        Process.Stop();
                        R3LastProcess();
                        SaveLOG("结果保存OK");
                        OnEnvTrigger(ResultStatusEnum.SETCAMLIGHT, EnvIndex, "ALL");
                        GC.Collect();
                        runwatchline.Stop();
                        SaveLOGNoTeme("完成 总用时:"+ runwatchline.ElapsedMilliseconds);
                        break;
                }
            }
        }

        void SaveLOG(string Message)
        {
            string runtime = " 用时:" + runwatch.ElapsedMilliseconds + "毫秒";

            string strID = Universal.OPTION + " ID:" + MainProcess.ID + " ";
            JetEazy.LoggerClass.Instance.WriteLog(strID + Message + runtime);

            runwatch.Restart();
        }
        void SaveLOGNoTeme(string Message)
        {
            JetEazy.LoggerClass.Instance.WriteLog(Message);
        }
        /// <summary>
        /// R3 結束流程後要做的雞巴毛事
        /// </summary>
        void R3LastProcess()
        {
            //if (IsPass && !Universal.ISCHECKSN)
            //{
            //    IsPass = false;
            //    MessageBox.Show("参数中没有检测 SN 的设定，请相关工程人员检查！");

            //}

            if (Universal.isR3ByPass)
                IsPass = true;

            if (IsPass)
            {
                PlayerPass.Play();
                OnTrigger(ResultStatusEnum.CALPASS);
                SaveLOG("播报CALPASS");
            }
            else
            {
                PlayerFail.Play();
                OnTrigger(ResultStatusEnum.CALNG);
                SaveLOG("播报CALNG");
            }
            if (Universal.R3UI.isBYPASS)
                OnTrigger(ResultStatusEnum.COUNTSTART);
            OnTrigger(ResultStatusEnum.COUNTEND);
            OnTrigger(ResultStatusEnum.CALEND);

            SaveLOG("呼叫 COUNTEND CALEND 完成");

            if (Universal.RESULT.TestMethod == TestMethodEnum.QSMCSF && INI.SFFACTORY == FactoryShopfloor.QSMC)
            {
                string strbarcode = "";
                if (Universal.R3UI.isSNHaveS)
                    strbarcode = "S" + BARCODE;
                else
                    strbarcode = BARCODE;

                string strsndata = strbarcode + ",OCR," + JzTimes.DateTimeSerialString + "," + (IsPass ? "P" : "F");
                string filename = INI.SHOPFLOORPATH + "\\" + strbarcode + ".txt";
                JzTools.SaveData(strsndata, filename);

                SaveLOG("保存SF结果完成");
            }
            if (INI.ISFOXCONNSF)
            {
                _playsound();
                _httpuploaddata(IsPass);

                SaveLOG("保存ISFOXCONNSF完成");
            }
            _savefoxconnreport();
            SaveLOG("保存报表完成");

            if (INI.ISHIVECLIENT)
            {
                if (MACHINE.GetCurrentMachineState == MachineState.Running)
                {
                    //MACHINE.HIVECLIENT.Hiveclient_MachineData(BARCODE, BARCODE, IsPass, m_input_time, DateTime.Now, _get_ocr_result_data_json());

                    //Create by Gaara [Find Path_Hive_Pictures all files]

                    List<string> _listHivePicture = new List<string>();
                    if (Directory.Exists(Path_Hive_Pictures))
                    {
                        string[] _filesHivePicture = Directory.GetFiles(Path_Hive_Pictures);
                        if (_filesHivePicture.Length > 0)
                        {
                            _listHivePicture = _filesHivePicture.ToList<string>();
                        }
                    }
                    if (File.Exists(FullPathName_Hive_Reports))
                    {
                        _listHivePicture.Add(FullPathName_Hive_Reports);
                    }
                    string strApplePath = @"D:\ALLRESULTPIC\REPORTS\FORMAT01\";
                    string strAppleFileName = JzTimes.DateSerialString + "_" + INI.DATA_FIXTUREID + ".csv";
                    string strFullFileName = strApplePath + strAppleFileName;
                    if (File.Exists(strFullFileName))
                    {
                        _listHivePicture.Add(strFullFileName);
                    }

                    //MACHINE.HIVECLIENT.Hiveclient_MachineData_Files(BARCODE,
                    //                                                                                                     BARCODE,
                    //                                                                                                     IsPass,
                    //                                                                                                     m_input_time,
                    //                                                                                                     DateTime.Now,
                    //                                                                                                     VER,
                    //                                                                                                     ARTWORKNAME,
                    //                                                                                                     RELATECOLORSTR,
                    //                                                                                                     _listHivePicture);

                    JzHiveItemMessageClass _msghiveitem = new JzHiveItemMessageClass();

                    _msghiveitem.unit_sn = BARCODE;
                    _msghiveitem.serials = BARCODE;
                    _msghiveitem.ispass = IsPass;
                    _msghiveitem.input_time = m_input_time;
                    _msghiveitem.output_time = m_input_time.AddMilliseconds(Testms[0]);
                    _msghiveitem.eVer = VER;
                    _msghiveitem.eArtWorkName = ARTWORKNAME;
                    _msghiveitem.eColor = RELATECOLORSTR;

                    _msghiveitem.machineName = INI.MACHINENAME;
                    _msghiveitem.machineID = INI.MACHINENAMEID;
                    _msghiveitem.KBCountryCode = VER;
                    _msghiveitem.TestTime = Testms[0].ToString();

                    _msghiveitem.format01Head = m_HiveAppleFormat01Head;
                    _msghiveitem.format01Value = m_HiveAppleFormat01Value;

                    MACHINE.HIVECLIENT.Hiveclient_MachineData_Files(_msghiveitem, _listHivePicture);

                    //string strresult_filepath = "";//測試結果檔案路徑+文件名稱
                    //MACHINE.HIVECLIENT.Hiveclient_MachineData_Files(BARCODE, BARCODE, IsPass, strresult_filepath, m_input_time, DateTime.Now);
                }
                //if (IsPass)
                //    MACHINE.SetMachineState(MachineState.Running);
                //else
                    MACHINE.SetMachineState(MachineState.Idle);

                SaveLOG(" HIVE 保存 完成");
            }

            if (INI.ISQSMCALLSAVE && !Universal.R3UI.isBYPASS)
            {
                ThreadForSavePictures();
                SaveLOG("保存Apple图片完成");
            }


        }

        void FillOperaterString(string opstr)
        {
            foreach (EnvClass env in AlbumWork.ENVList)
            {
                env.PassInfo.OperateString = opstr;

                foreach (PageClass page in env.PageList)
                {
                    page.PassInfo.OperateString = opstr;

                    foreach (AnalyzeClass analyzeroot in page.AnalyzeRootArray)
                    {
                        analyzeroot.SetPassInfoOPString(opstr);
                    }
                }
            }
        }

        /// <summary>
        /// 播放声音
        /// </summary>
        void _playsound()
        {
            if (INI.ISPLAYSOUND)
            {
                switch (INI.SFFACTORY)
                {
                    case FactoryShopfloor.FOXCONN:
                        if (IsPass)
                        {
                            switch (HOUSINGID)
                            {
                                case "0":
                                    JzToolsClass.Playing(Universal.MAINPATH + "\\WORK\\Housing0.WAV");
                                    break;
                                case "1":
                                    JzToolsClass.Playing(Universal.MAINPATH + "\\WORK\\Housing1.WAV");
                                    break;
                                default:
                                    JzToolsClass.Playing(Universal.MAINPATH + "\\WORK\\Housing0.WAV");
                                    break;
                            }
                        }
                        else
                            JzToolsClass.Playing(Universal.MAINPATH + "\\WORK\\Fail.WAV");
                        break;
                }
            }
        }
        #region FOXCONN MODE

        /// <summary>
        /// 检查镭雕SN是否错误
        /// </summary>
        bool m_CheckLaserSnError = false;
        /// <summary>
        /// 富士康报表保存
        /// </summary>
        void _savefoxconnreport()
        {
            string strFoxconnPath = @"D:\ALLRESULTPIC\REPORTS\";
            string strFoxconnFileName = JzTimes.DateSerialString + ".csv";
            string strFullFileName = strFoxconnPath + strFoxconnFileName;

            if (!System.IO.Directory.Exists(strFoxconnPath))
                System.IO.Directory.CreateDirectory(strFoxconnPath);

            string strReportMsg = "";
            string strHead = "SN,MachineName,MachineID,Color,KBCountryCode,LaserCountryCode,TestTime,StartTime,Result" + Environment.NewLine;

            if (!System.IO.File.Exists(strFullFileName))
                strReportMsg += strHead;

            strReportMsg += BARCODE + ",";
            strReportMsg += INI.MACHINENAME + ",";
            strReportMsg += INI.MACHINENAMEID + ",";
            strReportMsg += RELATECOLORSTR + ",";
            strReportMsg += VER + ",";
            strReportMsg += ARTWORKNAME + ",";
            strReportMsg += Testms[0].ToString() + ",";
            strReportMsg += m_input_time.ToString("yyyy/MM/dd HH:mm:ss") + ",";
            strReportMsg += (IsPass ? "PASS" : "FAIL") + Environment.NewLine;

            JzTools.SaveDataEX(strReportMsg, strFullFileName);
         

        }
        /// <summary>
        /// 组合错误原因
        /// </summary>
        /// <param name="m_ispass">检测是否PASS</param>
        void _httpuploaddata(bool m_ispass)
        {
            string Str_Name = "";
            string Str_Code = "";
            string HTTPStr_Name = "";
            string HTTPStr_Code = "";

            #region APPLE NAME AND VALUE
            List<string> RegionReportList = new List<string>();
            m_CheckLaserSnError = false;

            int i = 0;
            foreach (PageClass page in AlbumWork.ENVList[0].PageList)
            {
                AnalyzeClass analyzeroot = page.AnalyzeRoot;
                foreach (AnalyzeClass analyze in analyzeroot.BranchList)
                {
                    if (analyze.RunStatusCollection.NGCOUNT > 0)
                    {
                        //m_CheckLaserSnError = false;
                        foreach (WorkStatusClass work in analyze.RunStatusCollection.WorkStatusList)
                        {
                            if (work.AnalyzeProcedure == AnanlyzeProcedureEnum.CHECKOCR)
                            {
                                if (work.Reason == ReasonEnum.NG)
                                {
                                    m_CheckLaserSnError = true;
                                    break;
                                }
                            }
                        }
                        string str = analyze.AliasName + "," + analyze.RunStatusCollection.GetNGRunStatus(0).Reason.ToString();
                        if (m_CheckLaserSnError)
                        {
                            RegionReportList.Add(analyze.AliasName + "," + INI.CHECKSNERRORCODE);
                        }
                        else
                            RegionReportList.Add(str);
                        //string str = analyze.AliasName + "," + analyze.RunStatusCollection.GetNGRunStatus(0).Reason.ToString();
                        ////RegionReportList.Add(str);
                        ////if (!m_CheckLaserSnError)
                        //{
                        //    if (analyze.OCRPara.OCRMethod == OCRMethodEnum.MAPPING)
                        //    {
                        //        if (analyze.RunStatusCollection.GetNGRunStatus(0).Reason == ReasonEnum.NG)
                        //            RegionReportList.Add(analyze.AliasName + "," + INI.CHECKSNERRORCODE);
                        //    }
                        //    else
                        //    {
                        //        RegionReportList.Add(str);
                        //    }
                        //}
                    }
                    else
                    {
                        i = 0;
                        while (i < analyze.BranchList.Count)
                        {
                            if (analyze.BranchList[i].RunStatusCollection.NGCOUNT > 0)
                            {
                                //m_CheckLaserSnError = false;
                                foreach (WorkStatusClass work in analyze.BranchList[i].RunStatusCollection.WorkStatusList)
                                {
                                    if (work.AnalyzeProcedure == AnanlyzeProcedureEnum.CHECKOCR)
                                    {
                                        if (work.Reason == ReasonEnum.NG)
                                        {
                                            m_CheckLaserSnError = true;
                                            break;
                                        }
                                    }
                                }
                                string str = analyze.AliasName + "," + analyze.BranchList[i].RunStatusCollection.GetNGRunStatus(0).Reason.ToString();
                                if (m_CheckLaserSnError)
                                {
                                    RegionReportList.Add(analyze.AliasName + "," + INI.CHECKSNERRORCODE);
                                }
                                else
                                    RegionReportList.Add(str);
                                //string str = analyze.AliasName + "," + analyze.BranchList[i].RunStatusCollection.GetNGRunStatus(0).Reason.ToString();
                                ////RegionReportList.Add(str);
                                ////if (!m_CheckLaserSnError)
                                //{
                                //    if (analyze.OCRPara.OCRMethod == OCRMethodEnum.MAPPING)
                                //    {
                                //        if (analyze.BranchList[i].RunStatusCollection.GetNGRunStatus(0).Reason == ReasonEnum.NG)
                                //            RegionReportList.Add(analyze.AliasName + "," + INI.CHECKSNERRORCODE);
                                //    }
                                //    else
                                //    {
                                //        RegionReportList.Add(str);
                                //    }
                                //}
                            }
                            i++;
                        }
                    }
                }
            }

            RegionReportList.Sort();

            foreach (string str in RegionReportList)
            {
                if (str != "")
                {
                    string[] tmp = str.Split(',');
                    if (tmp.Length >= 2)
                    {
                        //HTTPStr_Name += tmp[1] + "/";
                        //HTTPStr_Code += tmp[0] + "/";
                        HTTPStr_Name = tmp[1] + "/";
                        HTTPStr_Code = (m_CheckLaserSnError ? INI.CHECKSNERRORCODE : "NG") + "/";
                        if (m_CheckLaserSnError)
                            break;
                    }
                }
            }


            foreach (string str in RegionReportList)
            {
                if (str != "")
                {
                    string[] tmp = str.Split(',');
                    if (tmp.Length >= 2)
                    {
                        Str_Name += tmp[0] + "%" + tmp[1] + "^";
                    }
                }
            }

            Str_Code = INI.MACHINENAME;
            #endregion

            if (HTTPStr_Name != "")
                HTTPStr_Name = HTTPStr_Name.Remove(HTTPStr_Name.Length - 1);
            if (HTTPStr_Code != "")
                HTTPStr_Code = HTTPStr_Code.Remove(HTTPStr_Code.Length - 1);

            string Pass = (m_ispass ? "PASS" : "FAIL");
            //传信息给http
            string st_Data = "B," + BARCODE + "," +
                            Pass + "," +
                            Str_Name + "," +
                            Str_Code + "," +
                            Universal.VersionDate + JzHiveClass.HiveVersion + "," +
                            HTTPStr_Name + "," +
                            HTTPStr_Code + "," +
                            VERSION;

            Universal.Memory.Write(st_Data);

            //Universal.WriteLog("Write HttpMessage:" + st_Data);
        }
        #endregion
        private string _get_qsmc_sndata_json()
        {
            Hashtable hash = new Hashtable();
            hash.Add("VER", VER);
            hash.Add("BARCODE", BARCODE);
            hash.Add("ARTWORKNAME", ARTWORKNAME);
            hash.Add("MODELNAME", MODELNAME);
            hash.Add("RELATECOLORSTR", RELATECOLORSTR);
            string strjson = JsonConvert.SerializeObject(hash);//{"key1":"val1","key2":"val2"}
            return strjson;
        }
        private string _get_ocr_result_data_json()
        {
            Hashtable hash = new Hashtable();
            hash.Add("data1", "OK");
            hash.Add("data2", "OK");
            hash.Add("data3", "OK");
            string strjson = JsonConvert.SerializeObject(hash);
            return strjson;
        }


        #region QSMC Saving AllResultPictures //ADD Gaara

        #region apple report formats

        /// <summary>
        /// 用於記錄重複測試SN記錄次數
        /// </summary>
        List<string> m_BarcodeCount = new List<string>();//用於記錄重複測試SN記錄次數
        bool m_IsInit = false;
        void ReadData(ref string DataStr, string FileName)
        {
            FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.None);
            StreamReader Srr = new StreamReader(fs, Encoding.Default);

            DataStr = Srr.ReadToEnd();

            Srr.Close();
            Srr.Dispose();
        }
        public void SaveData(string DataStr, string FileName, bool isappend)
        {
            StreamWriter Swr = new StreamWriter(FileName, isappend, System.Text.Encoding.UTF8);
            Swr.Write(DataStr);
            Swr.Flush();
            Swr.Close();
            Swr.Dispose();
        }
        private void _calBarcodeTestCount(string eBarcode, ref string eCount)
        {
            if (!m_IsInit)
            {
                m_IsInit = true;
                if (File.Exists(Application.StartupPath + "\\DailyReportSN.log.csv"))
                {
                    string strinit = "";
                    ReadData(ref strinit, Application.StartupPath + "\\DailyReportSN.log.csv");
                    string[] strsss = strinit.Replace(Environment.NewLine, "@").Split('@');
                    foreach (string strx in strsss)
                    {
                        if (strx != "")
                            m_BarcodeCount.Add(strx);
                    }
                }
            }

            eCount = "1";
            int k = -1;
            bool isdel = false;
            string strnew = eBarcode + ",1";
            foreach (string str in m_BarcodeCount)
            {
                string[] strs = str.Split(',');
                k++;
                if (strs[0] == eBarcode.Trim())
                {
                    isdel = true;
                    int value = int.Parse(strs[1]) + 1;
                    eCount = value.ToString();
                    strnew = strs[0] + "," + value.ToString();
                    break;
                }
            }

            if (isdel)
                m_BarcodeCount.RemoveAt(k);

            m_BarcodeCount.Add(strnew);

            if (m_BarcodeCount.Count >= 2800)
            {
                k = 0;
                while (k < 2700)
                {
                    m_BarcodeCount.RemoveAt(0);
                    k++;
                }
            }

            string strresult = "";
            foreach (string str in m_BarcodeCount)
            {
                strresult += str + Environment.NewLine;
            }
            SaveData(strresult, Application.StartupPath + "\\DailyReportSN.log.csv", false);
        }

        /*
         * SerialNumber	Date	Time	FixtureName	SNCount	Machine Cycle Time	ShopFloor(1=YES)	Reserved 1	Reserved 2	Reserved 3		
         * 1KV_KB_Version	
         * 2KD_KB_Defect	
         * 3LEV_LaserEtch_Version	
         * 3LESN	
         * 4LED_LaserEtch_Defect	
         * 5Sc1_Screw1_Color	5Sc2	5Sc3	5Sc4	5Sc5	5Sc6	
         * 5Sm1_Screw1_Missing	5Sm2	5Sm3	5Sm4	5Sm5	5Sm6	
         * 6Sh1_Screw1_Height	6Sh2	6Sh3	6Sh4	6Sh5	6Sh6	
         * PASS/FAIL
										USL(mm)	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	
										LSL(mm)	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	
            C02C4010MNHP	20200120	3	J152_AOI_D15_AOI_01	1	8	1					0	0	0	0	0	0	0	0	0	1	0	0	0	0	0	0	0	0	0	0	0	0	0	Fail

         */
        string m_HiveAppleFormat01Head = "";
        string m_HiveAppleFormat01Value = "";

        private void _saveAppleFormat01()
        {
            m_HiveAppleFormat01Head = "";
            m_HiveAppleFormat01Value = "";

            int _screwCount = 6;//判断螺丝的个数
            if (INI.DATA_SCREW_TEN)
                _screwCount = 10;

            string _barcodeTestCount = "1";
            _calBarcodeTestCount(BARCODE, ref _barcodeTestCount);

            string strApplePath = @"D:\ALLRESULTPIC\REPORTS\FORMAT01\";
            string strAppleFileName = JzTimes.DateSerialString + "_" + INI.DATA_FIXTUREID + ".csv";
            string strFullFileName = strApplePath + strAppleFileName;

            if (!System.IO.Directory.Exists(strApplePath))
                System.IO.Directory.CreateDirectory(strApplePath);

            string strReportMsg = "";
            string strHead = "SerialNumber,Date,Time,FixtureName,SNCount,Machine Cycle Time,ShopFloor(1=YES),Reserved 1,Reserved 2,Reserved 3,,";

            if (INI.DATA_SCREW_TEN)
            {
                //H0 标注 为了定义变量 H0[0]=1KV_KB_Vision ...
                strHead += "1KV_KB_Version,2KD_KB_Defect,3LEV_LaserEtch_Version,3LESN,4LED_LaserEtch_Defect,";
                strHead += "5Sc1_Screw1_Color,5Sc2,5Sc3,5Sc4,5Sc5,5Sc6,5Sc7,5Sc8,5Sc9,5Sc10,";//H1 标注 为了定义变量
                strHead += "5Sm1_Screw1_Missing,5Sm2,5Sm3,5Sm4,5Sm5,5Sm6,5Sm7,5Sm8,5Sm9,5Sm10,";//H2 标注 为了定义变量
                strHead += "6Sh1_Screw1_Height,6Sh2,6Sh3,6Sh4,6Sh5,6Sh6,6Sh7,6Sh8,6Sh9,6Sh10,";//H3 标注 为了定义变量
                strHead += "PASS/FAIL," + Environment.NewLine;

                strHead += ",,,,,,,,,,USL(mm),0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999," + Environment.NewLine;
                strHead += ",,,,,,,,,,LSL(mm),0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0," + Environment.NewLine;
            }
            else
            {

                //H0 标注 为了定义变量 H0[0]=1KV_KB_Vision ...
                strHead += "1KV_KB_Version,2KD_KB_Defect,3LEV_LaserEtch_Version,3LESN,4LED_LaserEtch_Defect,";
                strHead += "5Sc1_Screw1_Color,5Sc2,5Sc3,5Sc4,5Sc5,5Sc6,";//H1 标注 为了定义变量
                strHead += "5Sm1_Screw1_Missing,5Sm2,5Sm3,5Sm4,5Sm5,5Sm6,";//H2 标注 为了定义变量
                strHead += "6Sh1_Screw1_Height,6Sh2,6Sh3,6Sh4,6Sh5,6Sh6,";//H3 标注 为了定义变量
                strHead += "PASS/FAIL," + Environment.NewLine;

                strHead += ",,,,,,,,,,USL(mm),0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999," + Environment.NewLine;
                strHead += ",,,,,,,,,,LSL(mm),0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0," + Environment.NewLine;
            }

            if (!System.IO.File.Exists(strFullFileName))
                strReportMsg += strHead;

            strReportMsg += BARCODE + ",";
            strReportMsg += JzTimes.DateSerialString + ",";
            strReportMsg += m_input_time.AddMilliseconds(Testms[0]).ToString("HH:mm:ss") + ",";
            strReportMsg += INI.DATA_FIXTUREID + ",";
            strReportMsg += _barcodeTestCount + ",";
            strReportMsg += Testms[0].ToString() + ",";
            strReportMsg += "1" + ",,,,,";

            int[] _H0 = new int[5];
            int[] _H1 = new int[_screwCount];
            int[] _H2 = new int[_screwCount];
            int[] _H3 = new int[_screwCount];

            _H0[0] = 0;
            _H0[1] = 0;
            _H0[2] = 0;
            _H0[3] = 0;
            _H0[4] = 0;

            #region 结果组合

            int i = 0;
            int j = 0;
            int k = 0;

            i = 0;
            while (i < _screwCount)
            {
                _H1[i] = 0;
                _H2[i] = 0;
                _H3[i] = 0;
                i++;
            }

            foreach (PageClass page in AlbumWork.ENVList[0].PageList)
            {
                AnalyzeClass analyzeroot = page.AnalyzeRoot;

                switch (page.PageRunNo)
                {
                    case 0:

                        foreach (AnalyzeClass analyze in analyzeroot.BranchList)
                        {
                            if (analyze.RunStatusCollection.COUNT > 0)
                            {
                                if (!IsPass)
                                {
                                    if (analyze.RunStatusCollection.NGCOUNT > 0)
                                    {
                                        j = 0;
                                        while (j < analyze.RunStatusCollection.WorkStatusList.Count)
                                        {
                                            switch (analyze.RunStatusCollection.WorkStatusList[j].AnalyzeProcedure)
                                            {
                                                case AnanlyzeProcedureEnum.MEASURE:

                                                    if (analyze.RunStatusCollection.WorkStatusList[j].Reason != ReasonEnum.PASS)
                                                    {
                                                        if (analyze.BranchList.Count > 0)
                                                        {
                                                            k = 1;
                                                            while (k < _screwCount + 1)
                                                            {
                                                                if (analyze.BranchList[0].RelateASNItem.IndexOf("S" + k.ToString() + "(") > -1)
                                                                {
                                                                    _H1[k - 1] = 1;
                                                                }
                                                                k++;
                                                            }
                                                        }
                                                    }

                                                    break;
                                                default:

                                                    if (analyze.RunStatusCollection.WorkStatusList[j].Reason != ReasonEnum.PASS)
                                                    {
                                                        if (analyze.BranchList.Count > 0)
                                                        {
                                                            k = 1;
                                                            while (k < _screwCount + 1)
                                                            {
                                                                if (analyze.BranchList[0].RelateASNItem.IndexOf("S" + k.ToString() + "(") > -1)
                                                                {
                                                                    _H2[k - 1] = 1;
                                                                }
                                                                k++;
                                                            }
                                                        }
                                                    }

                                                    break;
                                            }
                                            j++;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                i = 0;
                                while (i < analyze.BranchList.Count)
                                {
                                    if (analyze.BranchList[i].RunStatusCollection.COUNT > 0)
                                    {
                                        if (!IsPass)
                                        {
                                            if (analyze.BranchList[i].RunStatusCollection.NGCOUNT > 0)
                                            {
                                                j = 0;
                                                while (j < analyze.BranchList[i].RunStatusCollection.WorkStatusList.Count)
                                                {
                                                    switch (analyze.BranchList[i].RunStatusCollection.WorkStatusList[j].AnalyzeProcedure)
                                                    {
                                                        case AnanlyzeProcedureEnum.MEASURE:

                                                            if (analyze.BranchList[i].RunStatusCollection.WorkStatusList[j].Reason != ReasonEnum.PASS)
                                                            {
                                                                k = 1;
                                                                while (k < _screwCount + 1)
                                                                {
                                                                    if (analyze.BranchList[i].RelateASNItem.IndexOf("S" + k.ToString() + "(") > -1)
                                                                    {
                                                                        _H1[k - 1] = 1;
                                                                    }
                                                                    k++;
                                                                }

                                                            }

                                                            break;
                                                        default:

                                                            if (analyze.BranchList[i].RunStatusCollection.WorkStatusList[j].Reason != ReasonEnum.PASS)
                                                            {

                                                                k = 1;
                                                                while (k < _screwCount + 1)
                                                                {
                                                                    if (analyze.BranchList[i].RelateASNItem.IndexOf("S" + k.ToString() + "(") > -1)
                                                                    {
                                                                        _H2[k - 1] = 1;
                                                                    }
                                                                    k++;
                                                                }
                                                            }

                                                            break;
                                                    }
                                                    j++;
                                                }
                                            }
                                        }
                                    }
                                    i++;
                                }
                            }
                        }


                        break;
                    case 1:

                        //if (_H0[4] == 0)
                        {
                            foreach (AnalyzeClass analyze in analyzeroot.BranchList)
                            {
                                if (analyze.RunStatusCollection.COUNT > 0)
                                {
                                    if (!IsPass)
                                    {
                                        if (analyze.RunStatusCollection.NGCOUNT > 0)
                                        {
                                            j = 0;
                                            while (j < analyze.RunStatusCollection.WorkStatusList.Count)
                                            {
                                                switch (analyze.RunStatusCollection.WorkStatusList[j].AnalyzeProcedure)
                                                {
                                                    case AnanlyzeProcedureEnum.CHECKOCR:

                                                        if (analyze.RunStatusCollection.WorkStatusList[j].Reason != ReasonEnum.PASS)
                                                        {
                                                            _H0[3] = 1;
                                                            //break;
                                                        }

                                                        break;
                                                    default:

                                                        if (analyze.RunStatusCollection.WorkStatusList[j].Reason != ReasonEnum.PASS)
                                                        {
                                                            _H0[4] = 1;
                                                            //break;
                                                        }

                                                        break;
                                                }
                                                j++;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    i = 0;
                                    while (i < analyze.BranchList.Count)
                                    {
                                        if (analyze.BranchList[i].RunStatusCollection.COUNT > 0)
                                        {
                                            if (!IsPass)
                                            {
                                                if (analyze.BranchList[i].RunStatusCollection.NGCOUNT > 0)
                                                {
                                                    j = 0;
                                                    while (j < analyze.BranchList[i].RunStatusCollection.WorkStatusList.Count)
                                                    {
                                                        switch (analyze.BranchList[i].RunStatusCollection.WorkStatusList[j].AnalyzeProcedure)
                                                        {
                                                            case AnanlyzeProcedureEnum.CHECKOCR:

                                                                if (analyze.BranchList[i].RunStatusCollection.WorkStatusList[j].Reason != ReasonEnum.PASS)
                                                                {
                                                                    _H0[3] = 1;
                                                                    //break;
                                                                }

                                                                break;
                                                            default:

                                                                if (analyze.BranchList[i].RunStatusCollection.WorkStatusList[j].Reason != ReasonEnum.PASS)
                                                                {
                                                                    _H0[4] = 1;
                                                                    //break;
                                                                }

                                                                break;
                                                        }
                                                        j++;
                                                    }
                                                }
                                            }
                                        }
                                        i++;
                                    }
                                }
                            }
                        }

                        break;
                    case 2:
                    case 3:

                        if (_H0[1] == 0)
                        {
                            foreach (AnalyzeClass analyze in analyzeroot.BranchList)
                            {
                                if (analyze.RunStatusCollection.COUNT > 0)
                                {
                                    if (!IsPass)
                                    {
                                        if (analyze.RunStatusCollection.NGCOUNT > 0)
                                        {
                                            _H0[1] = 1;//发现键盘错误时 跳出
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    i = 0;
                                    while (i < analyze.BranchList.Count)
                                    {
                                        if (analyze.BranchList[i].RunStatusCollection.COUNT > 0)
                                        {
                                            if (!IsPass)
                                            {
                                                if (analyze.BranchList[i].RunStatusCollection.NGCOUNT > 0)
                                                {
                                                    _H0[1] = 1;//发现键盘错误时 跳出
                                                    break;
                                                }
                                            }
                                        }
                                        i++;
                                    }
                                }
                            }
                        }

                        break;
                }
            }

            #endregion

            strReportMsg += _H0[0] + "," + _H0[1] + "," + _H0[2] + "," + _H0[3] + "," + _H0[4] + ",";
            //strReportMsg += _H1[0] + "," + _H1[1] + "," + _H1[2] + "," + _H1[3] + "," + _H1[4] + "," + _H1[5] + ",";
            //strReportMsg += _H2[0] + "," + _H2[1] + "," + _H2[2] + "," + _H2[3] + "," + _H2[4] + "," + _H2[5] + ",";
            //strReportMsg += _H3[0] + "," + _H3[1] + "," + _H3[2] + "," + _H3[3] + "," + _H3[4] + "," + _H3[5] + ",";

            i = 0;
            while (i < _screwCount)
            {
                strReportMsg += _H1[i] + ",";
                i++;
            }
            i = 0;
            while (i < _screwCount)
            {
                strReportMsg += _H2[i] + ",";
                i++;
            }
            i = 0;
            while (i < _screwCount)
            {
                strReportMsg += _H3[i] + ",";
                i++;
            }

            strReportMsg += (IsPass ? "PASS" : "FAIL") + "," + Environment.NewLine;

            m_HiveAppleFormat01Head = strHead;
            //string[] _valuestemp = strReportMsg.Replace(Environment.NewLine, "@").Split('@');
            if (!System.IO.File.Exists(strFullFileName))
                m_HiveAppleFormat01Value = strReportMsg.Replace(Environment.NewLine, "@").Split('@')[3];
            else
                m_HiveAppleFormat01Value = strReportMsg;

            JzTools.SaveDataEX(strReportMsg, strFullFileName);
        }

        #endregion

        string PicturePath = INI.ALLRESULTPIC + "\\Pictures\\" + JzTimes.DateSerialString + "\\" + JzTimes.DateTimeSerialString + "_" + INI.DATA_FIXTUREID + "_NULLSN" + "_Pictures";
        void ThreadForSavePictures()
        {
            System.Threading.Thread m_thread = new System.Threading.Thread(new System.Threading.ThreadStart(_saveAllResultPictures));
            m_thread.Start();
        }
        private void _saveAllResultPictures()
        {
            if (Universal.R3UI.isBYPASS)
                return;

            string DatePath = INI.ALLRESULTPIC + "\\Pictures\\" + JzTimes.DateSerialString;

            if (!Directory.Exists(DatePath))
                Directory.CreateDirectory(DatePath);

            string addstr = (IsPass ? "P-" : "F-");
            string strbarcodetmp = (string.IsNullOrEmpty(BARCODE) ? "Null_SN" : BARCODE);

            //所有CAM存圖的路徑
            string AllSavePath = DatePath + "\\" + addstr + JzTimes.DateTimeSerialString + "_" + INI.DATA_FIXTUREID + "_" + strbarcodetmp + "_" + VER + "_" + ARTWORKNAME + "_" + RELATECOLORSTR + "_Pictures";
            string AllSaveName = INI.DATA_FIXTUREID + "_" + strbarcodetmp + "_" + VER + "_" + ARTWORKNAME + "_" + RELATECOLORSTR + "_CAM";

            PicturePath = AllSavePath;//用於截圖存儲的路徑

            //Create by Gaara
            Path_Hive_Pictures = AllSavePath;
            //_saveSkynetSingleReports();
            _saveAppleFormat01();

            if (!Directory.Exists(AllSavePath))
                Directory.CreateDirectory(AllSavePath);

            EnvClass env = AlbumWork.ENVList[0];

            int qi = 0;
            foreach (PageClass page in env.PageList)
            {
                page.GetbmpRUN().Save(AllSavePath + "\\" + AllSaveName + qi.ToString("000") + ".jpg", ImageFormat.Jpeg);
                qi++;
            }
        }
        private void _saveSkynetSingleReports()
        {
            string DatePath = INI.ALLRESULTPIC + "\\Reports\\" + JzTimes.DateSerialString;
            string _SingleReportName = "Single_";

            if (!Directory.Exists(DatePath))
                Directory.CreateDirectory(DatePath);

            _SingleReportName += (IsPass ? "P-" : "F-");
            _SingleReportName += JzTimes.DateTimeSerialString + "_";
            _SingleReportName += INI.DATA_FIXTUREID + "_";
            _SingleReportName += (string.IsNullOrEmpty(BARCODE) ? "Null_SN" : BARCODE) + "_";
            _SingleReportName += VER + "_";
            _SingleReportName += ARTWORKNAME + "_";
            _SingleReportName += RELATECOLORSTR;

            List<string> RegionReportList = new List<string>();

            int i = 0;
            foreach (PageClass page in AlbumWork.ENVList[0].PageList)
            {
                AnalyzeClass analyzeroot = page.AnalyzeRoot;
                foreach (AnalyzeClass analyze in analyzeroot.BranchList)
                {
                    if (analyze.RunStatusCollection.COUNT > 0)
                    {
                        string str = analyze.AliasName + "," + analyze.RunStatusCollection.GetRunStatus(0).Reason.ToString();
                        if (!IsPass)
                        {
                            if (analyze.RunStatusCollection.NGCOUNT > 0)
                            {
                                str = analyze.AliasName + "," + analyze.RunStatusCollection.GetNGRunStatus(0).Reason.ToString();
                            }
                        }
                        RegionReportList.Add(str);
                    }
                    else
                    {
                        i = 0;
                        while (i < analyze.BranchList.Count)
                        {
                            if (analyze.BranchList[i].RunStatusCollection.COUNT > 0)
                            {
                                string str = analyze.AliasName + "," + analyze.BranchList[i].RunStatusCollection.GetRunStatus(0).Reason.ToString();
                                if (!IsPass)
                                {
                                    if (analyze.BranchList[i].RunStatusCollection.NGCOUNT > 0)
                                    {
                                        str = analyze.AliasName + "," + analyze.BranchList[i].RunStatusCollection.GetNGRunStatus(0).Reason.ToString();
                                    }
                                }
                                RegionReportList.Add(str);
                            }
                            i++;
                        }
                    }
                }
            }

            RegionReportList.Sort();
            Newtonsoft.Json.Linq.JObject jb0 = new Newtonsoft.Json.Linq.JObject();

            i = 0;
            foreach (string str in RegionReportList)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    string[] tmp = str.Split(',');
                    if (tmp.Length >= 2)
                    {
                        if (!string.IsNullOrEmpty(tmp[0]))
                        {
                            jb0.Add(str.Split(',')[0] + "ex" + i.ToString(), str.Split(',')[1]);
                        }
                        else
                        {
                            jb0.Add("Un" + i.ToString(), str.Split(',')[1]);
                        }
                    }
                }
                i++;
            }

            string strjson = jb0.ToString(Formatting.Indented, null);

            FullPathName_Hive_Reports = DatePath + "\\" + _SingleReportName + ".csv";
            SaveData(strjson, FullPathName_Hive_Reports);//保存本地數據

        }
        public void SavePrintScreen()
        {

            if (Universal.R3UI.isBYPASS)
                return;

            string strpath = @"D:\PRINTSCREEN\" + JzTimes.DateSerialString;
            string Qsmcpath = INI.ALLRESULTPIC + "\\NGPictures\\" + JzTimes.DateSerialString;


            string QsmcApplePath = INI.APPLERESURT + "\\NGPictures\\" + JzTimes.DateSerialString;
            if (!Directory.Exists(QsmcApplePath))
                Directory.CreateDirectory(QsmcApplePath);

            string QsmcApplePathPASS = INI.APPLERESURT + "\\Pictures\\" + JzTimes.DateSerialString;
            if (!Directory.Exists(QsmcApplePathPASS))
                Directory.CreateDirectory(QsmcApplePathPASS);

            if (!Directory.Exists(strpath))
                Directory.CreateDirectory(strpath);

            if (!Directory.Exists(Qsmcpath))
                Directory.CreateDirectory(Qsmcpath);

            int width = Screen.PrimaryScreen.Bounds.Width;
            int height = Screen.PrimaryScreen.Bounds.Height;


            Bitmap m = Universal.R3UI.bmpResult;
            if (m == null)
                return;
            //Bitmap m = new Bitmap(width, height);
            //using (Graphics g = Graphics.FromImage(m))
            //{
            //    g.CopyFromScreen(0, 0, 0, 0, Screen.AllScreens[0].Bounds.Size);
            //    g.Dispose();
            //}

            if (m!=null && !IsPass )
                m.Save(Qsmcpath + "\\" + (string.IsNullOrEmpty(BARCODE) ?
                                                                    JzTimes.DateTimeSerialString + "_" + INI.DATA_FIXTUREID + "_NULLSN_OCR"
                                                                    :
                                                                    JzTimes.DateTimeSerialString + "_" + INI.DATA_FIXTUREID + "_" + BARCODE + "_OCR") + ".jpg", ImageFormat.Jpeg);


            string pathtemp = "";
            string pathtempPASS = "";
            if (string.IsNullOrEmpty(BARCODE))
            {
                pathtemp = QsmcApplePath + "\\" + JzTimes.DateTimeSerialString + "_" + INI.DATA_FIXTUREID + "_NULLSN_OCR.jpg";
                pathtempPASS = QsmcApplePathPASS + "\\" + JzTimes.DateTimeSerialString + "_" + INI.DATA_FIXTUREID + "_NULLSN_OCR.jpg";
            }
            else
            {
                pathtemp = QsmcApplePath + "\\" + JzTimes.DateTimeSerialString + "_" + INI.DATA_FIXTUREID + "_" + BARCODE + "_OCR" + ".jpg";
                pathtempPASS = QsmcApplePathPASS + "\\" + JzTimes.DateTimeSerialString + "_" + INI.DATA_FIXTUREID + "_NULLSN_OCR.jpg";
            }

            if (!IsPass)
                m.Save(pathtemp, ImageFormat.Jpeg);
            else
                m.Save(pathtempPASS, ImageFormat.Jpeg);

            string strpath2 = PicturePath + "\\" + (string.IsNullOrEmpty(BARCODE) ?
                JzTimes.DateTimeSerialString
                + "_"
                + INI.DATA_FIXTUREID
                + "_NULLSN_OCR" : JzTimes.DateTimeSerialString
                + "_" + INI.DATA_FIXTUREID +
                "_" + BARCODE + "_OCR") + ".jpg";

            if (!Directory.Exists(PicturePath))
            {
                Directory.CreateDirectory(PicturePath);
            }
            m.Save(strpath2, ImageFormat.Jpeg);

            if (!Directory.Exists(strpath))
            {
                Directory.CreateDirectory(strpath);
            }

            m.Save(strpath + "\\" + (string.IsNullOrEmpty(BARCODE) ? JzTimes.TimeSerialString : BARCODE) + ".jpg", ImageFormat.Jpeg);
            m.Dispose();
        }

        #endregion

         /// <summary>
         /// 获得指定路径下所有文件名
         /// </summary>
         /// <param name="sw">文件写入流</param>
         /// <param name="path">文件写入流</param>
         /// <param name="indent">输出时的缩进量</param>
         public static void getFileName(StreamWriter sw, string path, int indent)
         {
             DirectoryInfo root = new DirectoryInfo(path);
             foreach (FileInfo f in root.GetFiles())
             {
                 for (int i = 0; i<indent; i++)
                 {
                     sw.Write("  ");
                 }
                 sw.WriteLine(f.Name);
             }
         }
 
         /// <summary>
         /// 获得指定路径下所有子目录名
         /// </summary>
         /// <param name="sw">文件写入流</param>
         /// <param name="path">文件夹路径</param>
         /// <param name="indent">输出时的缩进量</param>
         public static void getDirectory(StreamWriter sw, string path, int indent)
         {
             getFileName(sw, path, indent);
             DirectoryInfo root = new DirectoryInfo(path);
             foreach (DirectoryInfo d in root.GetDirectories())
             {
                 for (int i = 0; i<indent; i++)
                 {
                     sw.Write("  ");
                 }
                 sw.WriteLine("文件夹：" + d.Name);
                 getDirectory(sw, d.FullName, indent + 2);
                 sw.WriteLine();
             }
         }

        class TempLine
        {
            public PointF myopoit { get; set; }
            public double Lengt { get; set; }
        }
    }
}
