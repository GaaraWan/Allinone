using JetEazy;
using AHBlobPro;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetEazy.BasicSpace;
using EzSegClientLib;
using System.Drawing.Imaging;
using System.IO;

namespace Allinone.OPSpace.AnalyzeSpace
{
    public class PADINSPECTClass
    {
        public PADMethodEnum PADMethod = PADMethodEnum.NONE;

        PassInfoClass PassInfo = new PassInfoClass();

        public WorkStatusCollectionClass TrainStatusCollection = new WorkStatusCollectionClass();
        public WorkStatusCollectionClass RunStatusCollection = new WorkStatusCollectionClass();

        public string RelateAnalyzeString = "";
        public string RelateAnalyzeInformation = "";

        //double m_width = 10;
        //double m_height = 10;
        //double m_area = 100;

        public PADThresholdEnum PADThresholdMode { get; set; } = PADThresholdEnum.Threshold;
        public PADCalModeEnum PADCalMode { get; set; } = PADCalModeEnum.BlacktoBlack;
        public PADChipSize PADChipSizeMode { get; set; } = PADChipSize.CHIP_NORMAL;

        public double Resolution_Mil { get; set; } = 0.0254 / INI.MAINSD_PAD_MIL_RESOLUTION;//轉換爲 1 mil = 1.155 pixel

        public double OWidthRatio { get; set; } = 15;
        public double OHeightRatio { get; set; } = 15;
        public double OAreaRatio { get; set; } = 15;

        public int PADGrayThreshold { get; set; } = 128;
        public int PADBlobGrayThreshold { get; set; } = 128;

        public double CheckDWidth { get; set; } = 15;
        public double CheckDHeight { get; set; } = 15;
        public double CheckDArea { get; set; } = 15;

        public double ExtendX { get; set; } = 5;
        public double ExtendY { get; set; } = 5;


        public double CalExtendX { get; set; } = 66;
        public double CalExtendY { get; set; } = 66;

        public double BlackCalExtendX { get; set; } = 40;
        public double BlackCalExtendY { get; set; } = 40;

        public double BlackOffsetX { get; set; } = 0;
        public double BlackOffsetY { get; set; } = 0;

        #region Spec

        public double GlueMaxTop { get; set; } = 0.6;
        public double GlueMinTop { get; set; } = 0.1;

        public double GlueMaxBottom { get; set; } = 0.6;
        public double GlueMinBottom { get; set; } = 0.1;

        public double GlueMaxLeft { get; set; } = 1.2;
        public double GlueMinLeft { get; set; } = 0.1;

        public double GlueMaxRight { get; set; } = 0.9;
        public double GlueMinRight { get; set; } = 0.1;

        #endregion

        public double GlueMax { get; set; } = 180;
        public double GlueMin { get; set; } = 6;
        public bool GlueCheck { get; set; } = true;
        public double NoGlueThresholdValue { get; set; } = 0.7;
        public double BloodFillValueRatio { get; set; } = 0.33;

        string m_format = "0.000";

        private string m_DescStr = string.Empty;
        public string DescStr
        {
            get { return m_DescStr; }
            set { m_DescStr = value; }
        }

        public bool IsPass = true;
        private PointF m_PtfCenter = new PointF();
        public PointF PtfCenter
        {
            get { return m_PtfCenter; }
            set { m_PtfCenter = value; }
        }

        JetEazy.BasicSpace.JzFindObjectClass m_JzFind = new JetEazy.BasicSpace.JzFindObjectClass();
        PADRegionClass m_PADRegion = new PADRegionClass();
        bool m_IsSaveTemp = true;

        Bitmap imginput;
        Bitmap imgoutput;
        Bitmap imgmask;

        Bitmap bmpPattern;
        Bitmap bmpInput;
        Bitmap bmpMask;

        public Bitmap bmpPadFindOutput;
        public Bitmap bmpPadBolbOutput;

        public Bitmap bmpMeasureOutput = new Bitmap(1, 1);

        double m_BadArea = 0;
        int m_BadCount = 0;
        double m_BadWidth = 0;
        double m_BadHeight = 0;

        public GlueRegionClass[] glues = null;
        BorderLineRunClass[] borderLineRun = null;
        PointF p1 = new PointF();
        PointF p2 = new PointF();
        LineClass lineClass_top = null;
        LineClass lineClass_bottom = null;

        LineClass lineClass_left = null;
        LineClass lineClass_right = null;

        bool m_RunDataOK = false;
        public bool RunDataOK
        {
            get { return m_RunDataOK; }
            set { m_RunDataOK = value; }
        }
        private List<RectangleF> m_listRectFMask = new List<RectangleF>();
        /// <summary>
        /// 屏蔽chip上锡球的位置 填充黑色
        /// </summary>
        public List<RectangleF> ListRectFMask
        {
            set { m_listRectFMask = value; }
        }

        public void P10_GetPADInspectionRequirement(Bitmap bmppattern, Bitmap bmpmask, string relateanalyzestring, PassInfoClass passinfo)
        {
            //0.025/0.022 = 1 mil/1 pixel
            Resolution_Mil = 0.0254 / INI.MAINSD_PAD_MIL_RESOLUTION;

            ////胶水检测使用mm换算单位
            //switch (Universal.OPTION)
            //{
            //    case OptionEnum.MAIN_SDM1:
            //    case OptionEnum.MAIN_SDM2:
            //        Resolution_Mil = INI.MAINSD_PAD_MIL_RESOLUTION;
            //        break;
            //}

            RelateAnalyzeString = relateanalyzestring;
            //RelateAnalyzeInformation = relateanalyzeinformation;
            PassInfo = new PassInfoClass(passinfo, OPLevelEnum.COPY);

            switch (PADMethod)
            {
                case PADMethodEnum.PLACODE_CHECK:
                case PADMethodEnum.PADCHECK:
                case PADMethodEnum.GLUECHECK:
                    bmpPattern = new Bitmap(bmppattern);
                    bmpMask = new Bitmap(bmpmask);

                    bmpPadFindOutput = new Bitmap(bmppattern);
                    //bmpPadBolbOutput = new Bitmap(bmppattern);

                    //if (RelateAnalyzeString == "A00-02-0002" && m_IsSaveTemp)
                    //{
                    //    bmpPattern.Save("D:\\testtest\\inginput2.png", System.Drawing.Imaging.ImageFormat.Png);
                    //    bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                    //}

                    //if (m_IsSaveTemp)
                    //{
                    //    bmpPattern.Save("D:\\testtest\\" + RelateAnalyzeString + "bmpPattern" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                    //}

                    PADRegionFind(bmpPattern, PADGrayThreshold, true, out m_PADRegion);

                    if (GlueCheck)
                    {
                        try
                        {
                            m_IsSaveTemp = INI.IsDEBUGCHIP;
                            imginput = new Bitmap(bmppattern);
                            imgmask = new Bitmap(bmppattern.Width, bmppattern.Height);

                            Rectangle rect1 = new Rectangle(m_PADRegion.RegionForEdgeRect.X,
                                                                                   m_PADRegion.RegionForEdgeRect.Y,
                                                                                   m_PADRegion.RegionForEdgeRect.Width,
                                                                                   m_PADRegion.RegionForEdgeRect.Height);
                            rect1.Inflate(2, 2);

                            Bitmap bitmap = new Bitmap(imginput);
                            switch (PADChipSizeMode)
                            {
                                case PADChipSize.CHIP_V1:
                                    bitmap = new Bitmap(_getV1bmpInput(rect1));
                                    break;
                                case PADChipSize.CHIP_V2:
                                    bitmap = new Bitmap(_getV2_2bmpInput(rect1));
                                    break;
                                case PADChipSize.CHIP_V3:
                                    bitmap = new Bitmap(_getV3bmpInput(rect1));
                                    break;
                                case PADChipSize.CHIP_V6:
                                    bitmap = new Bitmap(_getV6bmpInput(rect1));
                                    break;
                                case PADChipSize.CHIP_V8:
                                    bitmap = new Bitmap(_getV8bmpInput(rect1, _from_bmpinputSize_to_iSized()));
                                    break;
                                case PADChipSize.CHIP_NORMAL:
                                default:
                                    bitmap = new Bitmap(_getG1bmpInput(rect1));
                                    break;
                            }
                            bmpPadFindOutput = new Bitmap(bitmap);
                        }
                        catch
                        {

                        }
                    }
                    break;
                case PADMethodEnum.GLUECHECK_BlackEdge:

                    bmpPattern = new Bitmap(bmppattern);
                    bmpMask = new Bitmap(bmpmask);

                    bmpPadFindOutput = new Bitmap(bmppattern);
                    //bmpPadBolbOutput = new Bitmap(bmppattern);

                    PADRegionFind_BlackEdge(bmpPattern, PADGrayThreshold, true, out m_PADRegion);

                    if (GlueCheck)
                    {
                        try
                        {
                            m_IsSaveTemp = INI.IsDEBUGCHIP;
                            imginput = new Bitmap(bmppattern);
                            imgmask = new Bitmap(bmppattern.Width, bmppattern.Height);

                            Bitmap bitmap = new Bitmap(imginput);
                            switch (PADChipSizeMode)
                            {
                                case PADChipSize.CHIP_V8:
                                    bitmap = new Bitmap(blackAICal(m_PADRegion, 1));
                                    break;
                                case PADChipSize.CHIP_NORMAL:
                                default:
                                    bitmap = new Bitmap(blackNormal(m_PADRegion));
                                    break;
                            }
                            bmpPadFindOutput.Dispose();
                            bmpPadFindOutput = new Bitmap(bitmap);
                            bitmap.Dispose();
                        }
                        catch
                        {

                        }
                    }


                    //if (m_IsSaveTemp)
                    {
                        bmpPadFindOutput.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpPadFindOutput" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                        //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                    }

                    
                    break;

                case PADMethodEnum.NONE:

                    break;
                default:

                    break;
            }
        }

        public bool PB10_PADInspectionProcess(Bitmap bmpinput, ref Bitmap bmpoutput)
        {
            //0.025/0.022 = 1 mil/1 pixel
            Resolution_Mil = 0.0254 / INI.MAINSD_PAD_MIL_RESOLUTION;

            ////胶水检测使用mm换算单位
            //switch (Universal.OPTION)
            //{
            //    case OptionEnum.MAIN_SDM1:
            //    case OptionEnum.MAIN_SDM2:
            //        Resolution_Mil = INI.MAINSD_PAD_MIL_RESOLUTION;
            //        break;
            //}

            if (PADMethod == PADMethodEnum.NONE)
            {
                IsPass = true;
                return true;
            }

            bool isgood = true;

            WorkStatusClass runstatus = new WorkStatusClass(AnanlyzeProcedureEnum.PADINSPECT);
            string processstring = "Start " + RelateAnalyzeString + " PAD Inspection." + Environment.NewLine;
            string errorstring = "";
            ReasonEnum reason = ReasonEnum.PASS;

            bmpInput = new Bitmap(bmpinput);

            bmpPadFindOutput = new Bitmap(bmpinput);
            //bmpPadBolbOutput = new Bitmap(bmpinput);

            #region 先判断PAD的尺寸是否正确 和 正确定位到

            PADRegionClass PADTempRegion = new PADRegionClass();
            PADRegionFind(bmpInput, PADGrayThreshold, false, out PADTempRegion);

            if (!IsInRangeRatio(PADTempRegion.RegionArea, m_PADRegion.RegionArea, OAreaRatio))
            {
                isgood = false;
                processstring += "Error in " + RelateAnalyzeString + " PAD AREA OVER Ratio= " + OAreaRatio.ToString() + " , " + PADTempRegion.RegionArea.ToString() + " , " + m_PADRegion.RegionArea.ToString() + Environment.NewLine;
                errorstring += RelateAnalyzeString + " PAD AREA OVER Ratio= " + OAreaRatio.ToString() + " , " + PADTempRegion.RegionArea.ToString() + " , " + m_PADRegion.RegionArea.ToString() + Environment.NewLine;

                reason = ReasonEnum.NG;
            }
            else if (!IsInRangeRatio(PADTempRegion.RegionWidth, m_PADRegion.RegionWidth, OWidthRatio))
            {
                isgood = false;
                processstring += "Error in " + RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + PADTempRegion.RegionWidth.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;
                errorstring += RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + PADTempRegion.RegionWidth.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;

                reason = ReasonEnum.NG;
            }
            else if (!IsInRangeRatio(PADTempRegion.RegionHeight, m_PADRegion.RegionHeight, OHeightRatio))
            {
                isgood = false;
                processstring += "Error in " + RelateAnalyzeString + " PAD HEIGHT OVER Ratio= " + OHeightRatio.ToString() + " , " + PADTempRegion.RegionHeight.ToString() + " , " + m_PADRegion.RegionHeight.ToString() + Environment.NewLine;
                errorstring += RelateAnalyzeString + " PAD HEIGHT OVER Ratio= " + OHeightRatio.ToString() + " , " + PADTempRegion.RegionHeight.ToString() + " , " + m_PADRegion.RegionHeight.ToString() + Environment.NewLine;

                reason = ReasonEnum.NG;
            }

            #endregion

            //if (RelateAnalyzeString == "A00-02-0002" && m_IsSaveTemp)
            //{
            //    bmpInput.Save("D:\\testtest\\inginput4.png", System.Drawing.Imaging.ImageFormat.Png);
            //    //imgpattern.Save("D:\\testtest\\imgpattern.png", eImageFormat.eImageFormat_PNG);
            //    //imginput.Save("D:\\testtest\\imginput.png", eImageFormat.eImageFormat_PNG);
            //    //bmpMask.Save("D:\\testtest\\mask.png", ImageFormat.Png);
            //}

            //if (RelateAnalyzeString == "A00-02-0004")
            //{
            //    bmpInput.Save("D:\\testtest\\" + RelateAnalyzeString + "bmpInput" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
            //}

            if (!isgood)
            {
                if (m_IsSaveTemp)
                {
                    bmpPadFindOutput.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "PadFind" + (false ? "_Train" : "_Run") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);

                    //bmp.Save("D:\\testtest\\" + RelateAnalyzeString + "PadFind" + (eIsTrain ? "_Train" : "_Run") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                }
            }


            //这里判断周围有无胶水  及 胶水的宽度

            if (isgood && GlueCheck && false)
            {
                ////RegionGlue(bmpinput);

                //GlueRegionClass[] glues = new GlueRegionClass[(int)BorderTypeEnum.COUNT];

                //PointF[] pts = null;
                //PointF[] ptsIN = null;

                //imginput = new Bitmap(bmpinput);
                //imgmask = new Bitmap(bmpinput.Width, bmpinput.Height);
                //Graphics gPrepared = Graphics.FromImage(imgmask);
                //gPrepared.Clear(Color.White);
                //gPrepared.FillPolygon(Brushes.Black, PADTempRegion.RegionPtFCornerORG);
                //gPrepared.Dispose();

                //m_JzFind.GetMaskedImage(imginput, imgmask, Color.White, Color.White, false);
                //Rectangle rect = new Rectangle(PADTempRegion.RegionForEdgeRect.X,
                //                                                       PADTempRegion.RegionForEdgeRect.Y,
                //                                                       PADTempRegion.RegionForEdgeRect.Width,
                //                                                       PADTempRegion.RegionForEdgeRect.Height);
                //rect.Inflate(-5, -5);

                //Bitmap bitmap = new Bitmap(imginput);

                //AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
                //Bitmap bitmap1 = grayscale.Apply(bitmap);
                //AForge.Imaging.Filters.HistogramEqualization histogramEqualization = new AForge.Imaging.Filters.HistogramEqualization();
                //Bitmap bitmap2 = histogramEqualization.Apply(bitmap1);
                //bitmap1.Dispose();

                //AForge.Imaging.Filters.GaussianBlur gaussianBlur = new AForge.Imaging.Filters.GaussianBlur();
                //gaussianBlur.Size = 5;
                //Bitmap bmpgaussian = gaussianBlur.Apply(bitmap2);
                //bitmap2.Dispose();
                //AForge.Imaging.Filters.BradleyLocalThresholding bradleyLocalThresholding = new AForge.Imaging.Filters.BradleyLocalThresholding();
                //Bitmap bitmap3 = bradleyLocalThresholding.Apply(bmpgaussian);
                //bmpgaussian.Dispose();

                //bitmap.Dispose();
                //bitmap = new Bitmap(bitmap3);
                //bitmap3.Dispose();

                ////if (RelateAnalyzeString == "A00-02-0002")
                //if (m_IsSaveTemp)
                //{
                //    bitmap.Save("D:\\testtest\\" + RelateAnalyzeString + "imginput" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                //}

                //int i = 0;
                //int j = 0;
                //while (i < (int)BorderTypeEnum.COUNT)
                //{
                //    _get_border_pointf(bitmap, rect, (BorderTypeEnum)i, out glues[i]);
                //    i++;
                //}

                //Bitmap bmpglueout = new Bitmap(bmpinput);
                //Graphics g = Graphics.FromImage(bmpglueout);
                //bool m_ischeckgluepass = true;

                //i = 0;
                //while (i < (int)BorderTypeEnum.COUNT)
                //{

                //    if (glues[i].LengthMax > (GlueMax * Resolution_Mil) || glues[i].LengthMin < (GlueMin * Resolution_Mil))
                //    {
                //        m_ischeckgluepass = false;
                //    }

                //    i++;
                //}


                ////画图 及 显示比对图
                //i = 0;

                //while (i < (int)BorderTypeEnum.COUNT)
                //{

                //    pts = glues[i].GetPointF();
                //    ptsIN = glues[i].GetPointFIN();

                //    j = 0;
                //    while (j < pts.Length)
                //    {
                //        double lengthx = GetPointLength(pts[j], ptsIN[j]);
                //        if (lengthx >= (GlueMin * Resolution_Mil) && lengthx <= (GlueMax * Resolution_Mil))
                //        {
                //            g.DrawLine(new Pen(Color.Lime, 2), pts[j], ptsIN[j]);
                //        }
                //        else
                //        {
                //            g.DrawLine(new Pen(Color.Red, 2), pts[j], ptsIN[j]);
                //        }
                //        j++;
                //    }

                //    i++;
                //}

                //if (m_IsSaveTemp)
                //{
                //    bmpglueout.Save("D:\\testtest\\" + RelateAnalyzeString + "bmpInputout" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                //}
                ////bmpglueout.Save("D:\\testtest\\" + RelateAnalyzeString + "bmpInputout" + ".png", System.Drawing.Imaging.ImageFormat.Png);

                //bmpoutput = new Bitmap(bmpglueout);
                //g.Dispose();
                //bmpglueout.Dispose();

                //if (!m_ischeckgluepass)
                //{
                //    isgood = false;
                //    processstring += "Error in " + RelateAnalyzeString + " Glue OVER " + Environment.NewLine;
                //    errorstring += RelateAnalyzeString + " Glue OVER " + Environment.NewLine;

                //    reason = ReasonEnum.NG;
                //}

            }


            #region 判断PAD 区域里面的blob

            if (isgood)
            {
                PADRegionFindBlob(bmpInput, PADBlobGrayThreshold, PADTempRegion, ref bmpoutput);

                ////mil 計算

                //m_BadArea = m_BadArea / Resolution_Mil / Resolution_Mil;
                //m_BadWidth = m_BadWidth / Resolution_Mil;
                //m_BadHeight = m_BadHeight / Resolution_Mil;

                if (m_BadArea > CheckDArea)
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " PAD AREA OVER area= " + " , " + m_BadArea.ToString() + " , " + CheckDArea.ToString() + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " PAD AREA OVER area= " + " , " + m_BadArea.ToString() + " , " + CheckDArea.ToString() + Environment.NewLine;

                    reason = ReasonEnum.NG;
                }
                else if (m_BadWidth > CheckDWidth)
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " PAD WIDTH OVER WIDTH= " + " , " + m_BadWidth.ToString() + " , " + CheckDWidth.ToString() + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " PAD WIDTH OVER WIDTH= " + " , " + m_BadWidth.ToString() + " , " + CheckDWidth.ToString() + Environment.NewLine;

                    reason = ReasonEnum.NG;
                }
                else if (m_BadHeight > CheckDHeight)
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " PAD HEIGHT OVER HEIGHT= " + " , " + m_BadHeight.ToString() + " , " + CheckDHeight.ToString() + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " PAD HEIGHT OVER HEIGHT= " + " , " + m_BadHeight.ToString() + " , " + CheckDHeight.ToString() + Environment.NewLine;

                    reason = ReasonEnum.NG;
                }


                //pixel 計算

                //if (m_BadArea > CheckDArea)
                //{
                //    isgood = false;
                //    processstring += "Error in " + RelateAnalyzeString + " PAD AREA OVER area= " + " , " + m_BadArea.ToString() + " , " + CheckDArea.ToString() + Environment.NewLine;
                //    errorstring += RelateAnalyzeString + " PAD AREA OVER area= " + " , " + m_BadArea.ToString() + " , " + CheckDArea.ToString() + Environment.NewLine;

                //    reason = ReasonEnum.NG;
                //}
                //else if (m_BadWidth > CheckDWidth)
                //{
                //    isgood = false;
                //    processstring += "Error in " + RelateAnalyzeString + " PAD WIDTH OVER WIDTH= " + " , " + m_BadWidth.ToString() + " , " + CheckDWidth.ToString() + Environment.NewLine;
                //    errorstring += RelateAnalyzeString + " PAD WIDTH OVER WIDTH= " + " , " + m_BadWidth.ToString() + " , " + CheckDWidth.ToString() + Environment.NewLine;

                //    reason = ReasonEnum.NG;
                //}
                //else if (m_BadHeight > CheckDHeight)
                //{
                //    isgood = false;
                //    processstring += "Error in " + RelateAnalyzeString + " PAD HEIGHT OVER HEIGHT= " + " , " + m_BadHeight.ToString() + " , " + CheckDHeight.ToString() + Environment.NewLine;
                //    errorstring += RelateAnalyzeString + " PAD HEIGHT OVER HEIGHT= " + " , " + m_BadHeight.ToString() + " , " + CheckDHeight.ToString() + Environment.NewLine;

                //    reason = ReasonEnum.NG;
                //}

            }

            #endregion

            //imginput.Dispose();
            //imgoutput.Dispose();

            runstatus.SetWorkStatus(bmpPattern, bmpInput, bmpoutput, reason, errorstring, processstring, PassInfo);
            RunStatusCollection.Add(runstatus);
            IsPass = isgood;

            bmpInput.Dispose();

            return isgood;
        }
        public bool PB10_GlueInspectionProcess(Bitmap bmpinput, ref Bitmap bmpoutput)
        {
            //0.025/0.022 = 1 mil/1 pixel
            Resolution_Mil = 0.0254 / INI.MAINSD_PAD_MIL_RESOLUTION;
            m_DescStr = string.Empty;

            bmpMeasureOutput.Dispose();
            //bmpMeasureOutput = new Bitmap(bmpinput);
            bmpMeasureOutput = (Bitmap)bmpinput.Clone();

            ////胶水检测使用mm换算单位
            //switch (Universal.OPTION)
            //{
            //    case OptionEnum.MAIN_SDM1:
            //    case OptionEnum.MAIN_SDM2:
            //        Resolution_Mil = INI.MAINSD_PAD_MIL_RESOLUTION;
            //        break;
            //}

            if (PADMethod == PADMethodEnum.NONE)
            {
                bmpoutput.Dispose();
                bmpoutput = new Bitmap(bmpinput);
                IsPass = true;
                return true;
            }
            p1 = new PointF(2710, 5.55f);
            p2 = new PointF(4257, 7.55f);
            lineClass_top = new LineClass(p1, p2);

            p1 = new PointF(1180, 7.75f);
            p2 = new PointF(4257, 5.55f);
            lineClass_bottom = new LineClass(p1, p2);

            p1 = new PointF(450, 5.55f);
            p2 = new PointF(3250, 7.55f);
            lineClass_left = new LineClass(p1, p2);

            p1 = new PointF(1600, 7.55f);
            p2 = new PointF(2850, 5.55f);
            lineClass_right = new LineClass(p1, p2);

            m_IsSaveTemp = INI.IsDEBUGCHIP;
            bool isgood = true;

            WorkStatusClass runstatus = new WorkStatusClass(AnanlyzeProcedureEnum.GLUEINSPECT);
            string processstring = "Start " + RelateAnalyzeString + " PAD Inspection." + Environment.NewLine;
            string errorstring = "";
            string descstriing = "";
            ReasonEnum reason = ReasonEnum.PASS;

            bmpInput = new Bitmap(bmpinput);

            //bmpPadFindOutput = new Bitmap(bmpinput);
            //bmpPadBolbOutput = new Bitmap(bmpinput);
            bmpPadFindOutput = (Bitmap)bmpinput.Clone();
            //bmpPadBolbOutput = (Bitmap)bmpinput.Clone();

            #region 判断有无芯片 目前用AU定位并判断
            ////先判断有无芯片//适用于整片都是chip的产品
            //Point centertemp = new Point(bmpinput.Width / 2, bmpinput.Height / 2);
            //Rectangle rectcenter = SimpleRect(centertemp, bmpinput.Width / 5, bmpinput.Height / 5);
            //Bitmap bmp0000 = bmpinput.Clone(rectcenter, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            //AForge.Imaging.Filters.Grayscale grayscale = AForge.Imaging.Filters.Grayscale.CommonAlgorithms.Y;
            //bmp0000 = grayscale.Apply(bmp0000);
            ////HistogramClass histogramClass = new HistogramClass(2);
            ////histogramClass.GetHistogram(bmp0000);
            ////AForge.Imaging.Filters.OtsuThreshold otsuThreshold = new AForge.Imaging.Filters.OtsuThreshold();
            ////Bitmap bmp0002 = otsuThreshold.Apply(bmp0001);
            ////AForge.Imaging.Filters.Threshold threshold = new AForge.Imaging.Filters.Threshold();
            ////Bitmap bmp0002 = threshold.Apply(bmp0001);

            //AForge.Imaging.Filters.HistogramEqualization histogramEqualization =
            //    new AForge.Imaging.Filters.HistogramEqualization();
            //bmp0000 = histogramEqualization.Apply(bmp0000);

            ////AForge.Imaging.Filters.Invert invert = new AForge.Imaging.Filters.Invert();
            ////Bitmap bmp0003 = invert.Apply(bmp0002);

            //AForge.Imaging.Filters.Threshold threshold = new AForge.Imaging.Filters.Threshold(20);
            //bmp0000 = threshold.Apply(bmp0000);

            //JzFindObjectClass jzFindObjectClass = new JzFindObjectClass();
            ////jzFindObjectClass.AH_SetThreshold(ref bmp0003, 20);
            //jzFindObjectClass.AH_FindBlob(bmp0000, false);

            //if (jzFindObjectClass.FoundList.Count > 3)
            //{
            //    isgood = false;
            //    processstring += "Error in " + RelateAnalyzeString + "no chip" + Environment.NewLine;
            //    errorstring += RelateAnalyzeString + "no chip" + Environment.NewLine;
            //    descstriing = "无芯片";
            //    reason = ReasonEnum.NG;
            //    if (INI.CHIP_forceALIGNRUN_pass)
            //        reason = ReasonEnum.PASS;

            //    bmpoutput.Dispose();
            //    //bmpoutput = new Bitmap(bmpinput);
            //    bmpoutput = (Bitmap)bmpinput.Clone();// new Bitmap(bmpinput);
            //}
            //if (m_IsSaveTemp)
            //{
            //    bmp0000.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString
            //        + "bmp0003_none_glue" + jzFindObjectClass.FoundList.Count.ToString()
            //        + ".png", System.Drawing.Imaging.ImageFormat.Png);
            //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
            //}
            #endregion

            #region 判断有无胶水  VICTOR模式
            //56ms
            if (isgood)
            {
                Bitmap bmpsize = new Bitmap(bmpinput, Resize(bmpinput.Size, -3));
                AForge.Imaging.Filters.HistogramEqualization histogramEqualization11 =
                    new AForge.Imaging.Filters.HistogramEqualization();
                Bitmap bmphistogramEqualization11 = histogramEqualization11.Apply(bmpsize);

                bmphistogramEqualization11 = FloodFill(bmphistogramEqualization11, new Point(bmphistogramEqualization11.Width / 2, bmphistogramEqualization11.Height / 2),
                                                                    Color.White, (int)(255 * NoGlueThresholdValue));

                AForge.Imaging.Filters.Grayscale grayscale11 =
                    new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
                bmphistogramEqualization11 = grayscale11.Apply(bmphistogramEqualization11);

                AForge.Imaging.Filters.ExtractBiggestBlob extractBiggestBlob11 =
                    new AForge.Imaging.Filters.ExtractBiggestBlob();
                bmphistogramEqualization11 = extractBiggestBlob11.Apply(bmphistogramEqualization11);

                if (m_IsSaveTemp)
                {
                    bmphistogramEqualization11.Save("D:\\testtest\\" + _CalPageIndex() +
                        RelateAnalyzeString + "NoDispensing" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    
                }

                Rectangle rectangletemp = new Rectangle(extractBiggestBlob11.BlobPosition.X,
                                                                           extractBiggestBlob11.BlobPosition.Y,
                                                                           bmphistogramEqualization11.Width,
                                                                           bmphistogramEqualization11.Height);

                RectangleF rectangletemp2 = ResizeWithLocation2(rectangletemp, 3);

                double iwidthtmp = Math.Max(rectangletemp2.Width, rectangletemp2.Height);
                double iheighttmp = Math.Min(rectangletemp2.Width, rectangletemp2.Height);

                //为了节省时间这里无胶不画图
                //Rectangle rect1pattrem = new Rectangle(m_PADRegion.RegionForEdgeRect.X,
                //                                                          m_PADRegion.RegionForEdgeRect.Y,
                //                                                          m_PADRegion.RegionForEdgeRect.Width,
                //                                                          m_PADRegion.RegionForEdgeRect.Height);

                //JzToolsClass jzToolsClass = new JzToolsClass();

                if (!IsInRangeRatio(m_PADRegion.RegionWidth, iwidthtmp, OWidthRatio))
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + rectangletemp2.Width.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + rectangletemp2.Width.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;
                    descstriing = "无胶";
                    reason = ReasonEnum.NG;

                    bmpoutput.Dispose();
                    bmpoutput = (Bitmap)bmpinput.Clone();// new Bitmap(bmpinput);
                    //jzToolsClass.DrawRect(bmpoutput, rect1pattrem, new Pen(Color.Red, 5));
                    //if (INI.CHIP_NG_SHOW)
                    //{
                    //    bmpoutput.Dispose();
                    //    bmpoutput = new Bitmap(bmpgray11);
                    //}
                }
                else if (!IsInRangeRatio(m_PADRegion.RegionHeight, iheighttmp, OHeightRatio))
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " PAD HEIGHT OVER Ratio= " + OHeightRatio.ToString() + " , " + rectangletemp2.Height.ToString() + " , " + m_PADRegion.RegionHeight.ToString() + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " PAD HEIGHT OVER Ratio= " + OHeightRatio.ToString() + " , " + rectangletemp2.Height.ToString() + " , " + m_PADRegion.RegionHeight.ToString() + Environment.NewLine;
                    descstriing = "无胶";
                    reason = ReasonEnum.NG;

                    bmpoutput.Dispose();
                    bmpoutput = (Bitmap)bmpinput.Clone();// new Bitmap(bmpinput);
                    //jzToolsClass.DrawRect(bmpoutput, rect1pattrem, new Pen(Color.Red, 5));
                    //if (INI.CHIP_NG_SHOW)
                    //{
                    //    bmpoutput.Dispose();
                    //    bmpoutput = new Bitmap(bmpgray11);
                    //}
                }


                bmphistogramEqualization11.Dispose();
                //bmpfloodfill.Dispose();
                ////bmpgray11.Dispose();
                //bmpextractBiggestBlob11.Dispose();
            }

            #endregion


            #region 先判断PAD的尺寸是否正确 和 正确定位到 及 判断PAD 区域里面的blob

            PADRegionClass PADTempRegion = new PADRegionClass();
           
            if (isgood)
            {
                //166ms
                PADRegionFind(bmpInput, PADGrayThreshold, false, out PADTempRegion);

                if (!IsInRangeRatio(PADTempRegion.RegionArea, m_PADRegion.RegionArea, OAreaRatio))
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " PAD AREA OVER Ratio= " + OAreaRatio.ToString() + " , " + PADTempRegion.RegionArea.ToString() + " , " + m_PADRegion.RegionArea.ToString() + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " PAD AREA OVER Ratio= " + OAreaRatio.ToString() + " , " + PADTempRegion.RegionArea.ToString() + " , " + m_PADRegion.RegionArea.ToString() + Environment.NewLine;

                    reason = ReasonEnum.NG;
                    descstriing = "尺寸面积超标";
                }
                else if (!IsInRangeRatio(PADTempRegion.RegionWidth, m_PADRegion.RegionWidth, OWidthRatio))
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + PADTempRegion.RegionWidth.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + PADTempRegion.RegionWidth.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;

                    reason = ReasonEnum.NG;
                    descstriing = "尺寸宽度超标";
                }
                else if (!IsInRangeRatio(PADTempRegion.RegionHeight, m_PADRegion.RegionHeight, OHeightRatio))
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " PAD HEIGHT OVER Ratio= " + OHeightRatio.ToString() + " , " + PADTempRegion.RegionHeight.ToString() + " , " + m_PADRegion.RegionHeight.ToString() + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " PAD HEIGHT OVER Ratio= " + OHeightRatio.ToString() + " , " + PADTempRegion.RegionHeight.ToString() + " , " + m_PADRegion.RegionHeight.ToString() + Environment.NewLine;

                    reason = ReasonEnum.NG;
                    descstriing = "尺寸高度超标";
                }

                #region 判断PAD 区域里面的blob
              
                //if (isgood)
                {
                    //78ms
                    PADRegionFindBlob(bmpInput, PADBlobGrayThreshold, PADTempRegion, ref bmpoutput);

                    ////mil 計算

                    //m_BadArea = m_BadArea / Resolution_Mil / Resolution_Mil;
                    //m_BadWidth = m_BadWidth / Resolution_Mil;
                    //m_BadHeight = m_BadHeight / Resolution_Mil;

                    if (m_BadArea > CheckDArea)
                    {
                        isgood = false;
                        processstring += "Error in " + RelateAnalyzeString + " PAD AREA OVER area= " + " , " + m_BadArea.ToString() + " , " + CheckDArea.ToString() + Environment.NewLine;
                        errorstring += RelateAnalyzeString + " PAD AREA OVER area= " + " , " + m_BadArea.ToString() + " , " + CheckDArea.ToString() + Environment.NewLine;

                        reason = ReasonEnum.NG;
                        descstriing = "晶片表面溢胶";
                    }
                    else if (m_BadWidth > CheckDWidth)
                    {
                        isgood = false;
                        processstring += "Error in " + RelateAnalyzeString + " PAD WIDTH OVER WIDTH= " + " , " + m_BadWidth.ToString() + " , " + CheckDWidth.ToString() + Environment.NewLine;
                        errorstring += RelateAnalyzeString + " PAD WIDTH OVER WIDTH= " + " , " + m_BadWidth.ToString() + " , " + CheckDWidth.ToString() + Environment.NewLine;

                        reason = ReasonEnum.NG;
                        descstriing = "晶片表面溢胶";
                    }
                    else if (m_BadHeight > CheckDHeight)
                    {
                        isgood = false;
                        processstring += "Error in " + RelateAnalyzeString + " PAD HEIGHT OVER HEIGHT= " + " , " + m_BadHeight.ToString() + " , " + CheckDHeight.ToString() + Environment.NewLine;
                        errorstring += RelateAnalyzeString + " PAD HEIGHT OVER HEIGHT= " + " , " + m_BadHeight.ToString() + " , " + CheckDHeight.ToString() + Environment.NewLine;

                        reason = ReasonEnum.NG;
                        descstriing = "晶片表面溢胶";
                    }
                }

                #endregion
            }

            #endregion

            if (!isgood)
            {
                if (m_IsSaveTemp)
                {
                    bmpPadFindOutput.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "PadFind" + (false ? "_Train" : "_Run") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);

                    //bmp.Save("D:\\testtest\\" + RelateAnalyzeString + "PadFind" + (eIsTrain ? "_Train" : "_Run") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                }
            }

            #region 判断PAD 区域里面的blob 移动至判断尺寸中
            //144ms
            ////if (isgood)
            //{
            //    PADRegionFindBlob(bmpInput, PADBlobGrayThreshold, PADTempRegion, ref bmpoutput);

            //    ////mil 計算

            //    //m_BadArea = m_BadArea / Resolution_Mil / Resolution_Mil;
            //    //m_BadWidth = m_BadWidth / Resolution_Mil;
            //    //m_BadHeight = m_BadHeight / Resolution_Mil;

            //    if (m_BadArea > CheckDArea)
            //    {
            //        isgood = false;
            //        processstring += "Error in " + RelateAnalyzeString + " PAD AREA OVER area= " + " , " + m_BadArea.ToString() + " , " + CheckDArea.ToString() + Environment.NewLine;
            //        errorstring += RelateAnalyzeString + " PAD AREA OVER area= " + " , " + m_BadArea.ToString() + " , " + CheckDArea.ToString() + Environment.NewLine;

            //        reason = ReasonEnum.NG;
            //        descstriing = "晶片表面溢胶";
            //    }
            //    else if (m_BadWidth > CheckDWidth)
            //    {
            //        isgood = false;
            //        processstring += "Error in " + RelateAnalyzeString + " PAD WIDTH OVER WIDTH= " + " , " + m_BadWidth.ToString() + " , " + CheckDWidth.ToString() + Environment.NewLine;
            //        errorstring += RelateAnalyzeString + " PAD WIDTH OVER WIDTH= " + " , " + m_BadWidth.ToString() + " , " + CheckDWidth.ToString() + Environment.NewLine;

            //        reason = ReasonEnum.NG;
            //        descstriing = "晶片表面溢胶";
            //    }
            //    else if (m_BadHeight > CheckDHeight)
            //    {
            //        isgood = false;
            //        processstring += "Error in " + RelateAnalyzeString + " PAD HEIGHT OVER HEIGHT= " + " , " + m_BadHeight.ToString() + " , " + CheckDHeight.ToString() + Environment.NewLine;
            //        errorstring += RelateAnalyzeString + " PAD HEIGHT OVER HEIGHT= " + " , " + m_BadHeight.ToString() + " , " + CheckDHeight.ToString() + Environment.NewLine;

            //        reason = ReasonEnum.NG;
            //        descstriing = "晶片表面溢胶";
            //    }




            //}

            #endregion

            //胶水的宽度

            #region 胶水宽度判断

            glues = null;
            if (isgood && GlueCheck)
            {

                glues = new GlueRegionClass[(int)BorderTypeEnum.COUNT];
                PointF[] pts = null;
                PointF[] ptsIN = null;

                imginput = (Bitmap)bmpinput.Clone();// new Bitmap(bmpinput);
                imgmask = new Bitmap(bmpinput.Width, bmpinput.Height);

                Rectangle rect1 = new Rectangle(PADTempRegion.RegionForEdgeRect.X,
                                                                       PADTempRegion.RegionForEdgeRect.Y,
                                                                       PADTempRegion.RegionForEdgeRect.Width,
                                                                       PADTempRegion.RegionForEdgeRect.Height);
                rect1.Inflate(2, 2);



                Rectangle rect = new Rectangle(PADTempRegion.RegionForEdgeRect.X,
                                                                       PADTempRegion.RegionForEdgeRect.Y,
                                                                       PADTempRegion.RegionForEdgeRect.Width,
                                                                       PADTempRegion.RegionForEdgeRect.Height);


                Bitmap bitmap = (Bitmap)bmpInput.Clone();// new Bitmap(bmpInput);
                Bitmap bitmap0 = (Bitmap)bmpInput.Clone();//new Bitmap(bmpInput);
                if (m_IsSaveTemp)
                {
                    bitmap0.Save(Universal.CalTestPath + "\\" + _CalPageIndex() + RelateAnalyzeString + "bmpTrainCollect" + ".png",
                       System.Drawing.Imaging.ImageFormat.Png);


                    bitmap0.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bitmap0" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                }
                switch (PADChipSizeMode)
                {
                    case PADChipSize.CHIP_V1:
                        bitmap = (Bitmap)_getV1bmpInput(rect1).Clone(); //new Bitmap(_getV1bmpInput(rect1));
                        break;

                    case PADChipSize.CHIP_V2:
                        bitmap = (Bitmap)_getV2_2bmpInput(rect1).Clone(); // new Bitmap(_getV2_2bmpInput(rect1));
                        break;
                    case PADChipSize.CHIP_V3:
                        bitmap = (Bitmap)_getV3bmpInput(rect1).Clone(); // new Bitmap(_getV3bmpInput(rect1));
                        break;
                    case PADChipSize.CHIP_V6:
                        bitmap = (Bitmap)_getV6bmpInput(rect1).Clone(); // new Bitmap(_getV6bmpInput(rect1));
                        break;
                    case PADChipSize.CHIP_V8:
                        //bitmap = (Bitmap)_getV8bmpInput(rect1).Clone();
                        bitmap = new Bitmap(_getV8bmpInput(rect1, _from_bmpinputSize_to_iSized()));
                        break;
                    case PADChipSize.CHIP_V5:
                        break;
                    case PADChipSize.CHIP_NORMAL:
                    default:
                        //bitmap = new Bitmap(_getG1bmpInput(rect1));
                        bitmap = (Bitmap)_getG1bmpInput(rect1).Clone();
                        break;
                }

                //这里是寻找点的起点位置
                //rect.Inflate(-5, -5);
                rect.Inflate(-(int)(CalExtendX * Resolution_Mil), -(int)(CalExtendY * Resolution_Mil));

                if (m_IsSaveTemp)
                {
                    bitmap.Save("D:\\testtest\\" + RelateAnalyzeString + "imginputorg" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                }

                Rectangle rectin = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                //rectin.Inflate(5, 5);
                int minin = (int)(GlueMin * Resolution_Mil);
                rectin.Inflate(minin, minin);
                //g.DrawRectangle(new Pen(Color.Blue), rectin);

                Rectangle rectout = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                //rectout.Inflate(5, 5);
                int minout = (int)(GlueMax * Resolution_Mil);
                rectout.Inflate(minout, minout);
                //g.DrawRectangle(new Pen(Color.Yellow), rectout);

                
                switch (PADChipSizeMode)
                {
                    case PADChipSize.CHIP_V8:
                        //RectangleF rectangleFxxx = ResizeWithLocation2(rectout, -2);
                        //gx.DrawRectangle(new Pen(Color.Yellow), rectangleFxxx.X, rectangleFxxx.Y,
                        //    rectangleFxxx.Width, rectangleFxxx.Height);
                        break;
                    default:
                        Graphics gx = Graphics.FromImage(bitmap);
                        gx.DrawRectangle(new Pen(Color.Yellow), rectout);
                        gx.Dispose();
                        break;
                }
                
                //

                //if (m_IsSaveTemp)
                //{
                //    bitmap.Save("D:\\testtest\\" + RelateAnalyzeString + "imginputrectout" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                //}

                int i = 0;
                int j = 0;
                //while (i < (int)BorderTypeEnum.COUNT)
                //{

                //    switch (PADChipSizeMode)
                //    {

                //        case PADChipSize.CHIP_V5:

                //            Rectangle recttempcorner = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                //            bitmap0 = new Bitmap(bmpInput);

                //            switch ((BorderTypeEnum)i)
                //            {
                //                case BorderTypeEnum.LEFT:
                //                    recttempcorner = new Rectangle(0, rect.Y, rect.X, rect.Height);
                //                    break;
                //                case BorderTypeEnum.TOP:
                //                    recttempcorner = new Rectangle(rect.X, 0, rect.Width, rect.Y);
                //                    break;
                //                case BorderTypeEnum.RIGHT:
                //                    recttempcorner = new Rectangle(rect.Right, rect.Y, bitmap0.Width - rect.Right, rect.Height);
                //                    break;
                //                case BorderTypeEnum.BOTTOM:
                //                    recttempcorner = new Rectangle(rect.X, rect.Bottom, rect.Width, bitmap0.Height - rect.Bottom);
                //                    break;
                //            }

                //            Bitmap bitmap1 = (Bitmap)bitmap0.Clone(recttempcorner,
                //                System.Drawing.Imaging.PixelFormat.Format32bppArgb);


                //            bitmap = _getV5bmpInput(bitmap1, (BorderTypeEnum)i);
                //            _get_border_pointf_v5(bitmap, recttempcorner, (BorderTypeEnum)i, out glues[i]);


                //            break;

                //        case PADChipSize.CHIP_V1:
                //            _get_border_pointfEx01(bitmap, PADTempRegion.bmpThreshold, rect, (BorderTypeEnum)i, out glues[i]);
                //            //_get_border_pointf(bitmap, rect, (BorderTypeEnum)i, out glues[i]);
                //            break;
                //        case PADChipSize.CHIP_V3:
                //            _get_border_pointf_v3_1(bitmap, PADTempRegion.bmpThreshold, rect, (BorderTypeEnum)i, out glues[i]);
                //            break;
                //        case PADChipSize.CHIP_V6:

                //            if (m_IsSaveTemp)
                //            {
                //                PADTempRegion.bmpThreshold.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpThreshold" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //                //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                //            }

                //            _get_border_pointf_v6_1(bitmap, PADTempRegion.bmpThreshold, rect, (BorderTypeEnum)i, out glues[i]);
                //            break;
                //        case PADChipSize.CHIP_V8:

                //            if (m_IsSaveTemp)
                //            {
                //                PADTempRegion.bmpThreshold.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpThreshold" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //                //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                //            }

                //            if (INI.chipUseAI)
                //                _get_border_pointf_v8_1(bitmap, PADTempRegion.bmpThreshold, rect, (BorderTypeEnum)i, out glues[i]);
                //            else
                //                _get_border_pointf_v6_1(bitmap, PADTempRegion.bmpThreshold, rect, (BorderTypeEnum)i, out glues[i]);

                //            break;
                //        case PADChipSize.CHIP_V2:
                //        //_get_border_pointf_v2(bitmap, rect, (BorderTypeEnum)i, out glues[i]);
                //        //break;
                //        case PADChipSize.CHIP_NORMAL:
                //        default:

                //            SetDilatation3x3(ref PADTempRegion.bmpThreshold, 2);
                //            SetErosion3x3(ref PADTempRegion.bmpThreshold, 2);

                //            //if (m_IsSaveTemp)
                //            //{
                //            //    PADTempRegion.bmpThreshold.Save("D:\\testtest\\" + RelateAnalyzeString + "bitmap1" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //            //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                //            //}

                //            _get_border_pointf_v3(bitmap, PADTempRegion.bmpThreshold, rect, (BorderTypeEnum)i, out glues[i]);
                //            break;
                //    }
                //    i++;
                //}

                //多线程计算4条边
                switch (PADChipSizeMode)
                {
                    //case PADChipSize.CHIP_V5:
                    //    break;

                    case PADChipSize.CHIP_V1:

                        #region V1
                        borderLineRun = new BorderLineRunClass[4];
                        i = 0;
                        while (i < (int)BorderTypeEnum.COUNT)
                        {

                            borderLineRun[i] = new BorderLineRunClass();
                            borderLineRun[i].bmp0 = new Bitmap(bitmap);
                            borderLineRun[i].bmp1 = new Bitmap(PADTempRegion.bmpThreshold);
                            borderLineRun[i].rect0 = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                            borderLineRun[i].Border = (BorderTypeEnum)i;
                            borderLineRun[i].index = i;

                            i++;
                        }

                        Parallel.ForEach(borderLineRun, item =>
                        {
                            _get_border_pointfEx01(item.bmp0, item.bmp1, item.rect0, item.Border, out glues[item.index]);

                        });

                        i = 0;
                        while (i < (int)BorderTypeEnum.COUNT)
                        {

                            //borderLineRun[i] = new BorderLineRunClass();
                            borderLineRun[i].bmp0.Dispose();
                            borderLineRun[i].bmp1.Dispose();

                            i++;
                        }
                        GC.Collect();
                        #endregion

                        break;
                    case PADChipSize.CHIP_V3:


                        #region V3
                        borderLineRun = new BorderLineRunClass[4];
                        i = 0;
                        while (i < (int)BorderTypeEnum.COUNT)
                        {

                            borderLineRun[i] = new BorderLineRunClass();
                            borderLineRun[i].bmp0 = new Bitmap(bitmap);
                            borderLineRun[i].bmp1 = new Bitmap(PADTempRegion.bmpThreshold);
                            borderLineRun[i].rect0 = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                            borderLineRun[i].Border = (BorderTypeEnum)i;
                            borderLineRun[i].index = i;

                            i++;
                        }

                        Parallel.ForEach(borderLineRun, item =>
                        {
                            _get_border_pointf_v3_1(item.bmp0, item.bmp1, item.rect0, item.Border, out glues[item.index]);

                        });

                        i = 0;
                        while (i < (int)BorderTypeEnum.COUNT)
                        {

                            //borderLineRun[i] = new BorderLineRunClass();
                            borderLineRun[i].bmp0.Dispose();
                            borderLineRun[i].bmp1.Dispose();

                            i++;
                        }
                        GC.Collect();
                        #endregion

                        break;
                    case PADChipSize.CHIP_V6:
                        #region V6

                        if (m_IsSaveTemp)
                        {
                            PADTempRegion.bmpThreshold.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpThreshold" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                            //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                        }
                        borderLineRun = new BorderLineRunClass[4];
                        i = 0;
                        while (i < (int)BorderTypeEnum.COUNT)
                        {

                            borderLineRun[i] = new BorderLineRunClass();
                            borderLineRun[i].bmp0 = new Bitmap(bitmap);
                            borderLineRun[i].bmp1 = new Bitmap(PADTempRegion.bmpThreshold);
                            borderLineRun[i].rect0 = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                            borderLineRun[i].Border = (BorderTypeEnum)i;
                            borderLineRun[i].index = i;

                            i++;
                        }

                        Parallel.ForEach(borderLineRun, item =>
                        {
                            _get_border_pointf_v6_1(item.bmp0, item.bmp1, item.rect0, item.Border, out glues[item.index]);

                        });

                        i = 0;
                        while (i < (int)BorderTypeEnum.COUNT)
                        {

                            //borderLineRun[i] = new BorderLineRunClass();
                            borderLineRun[i].bmp0.Dispose();
                            borderLineRun[i].bmp1.Dispose();

                            i++;
                        }
                        GC.Collect();
                        #endregion
                        break;
                    case PADChipSize.CHIP_V8:

                        if (m_IsSaveTemp)
                        {
                            PADTempRegion.bmpThreshold.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpThreshold" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                            //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                        }
                        borderLineRun = new BorderLineRunClass[4];
                        i = 0;
                        while (i < (int)BorderTypeEnum.COUNT)
                        {

                            borderLineRun[i] = new BorderLineRunClass();
                            borderLineRun[i].bmp0 = (Bitmap)bitmap.Clone();// new Bitmap(bitmap);
                            borderLineRun[i].bmp1 = new Bitmap(PADTempRegion.bmpThreshold,
                                Resize(PADTempRegion.bmpThreshold.Size, _from_bmpinputSize_to_iSized()));

                            RectangleF rectSize = ResizeWithLocation2(rect, _from_bmpinputSize_to_iSized());

                            //(Bitmap)PADTempRegion.bmpThreshold.Clone();// new Bitmap(PADTempRegion.bmpThreshold);
                            borderLineRun[i].rect0 =
                                new Rectangle((int)rectSize.X, (int)rectSize.Y, (int)rectSize.Width, (int)rectSize.Height);
                                //new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                            borderLineRun[i].Border = (BorderTypeEnum)i;
                            borderLineRun[i].index = i;

                            i++;
                        }

                        //foreach (BorderLineRunClass item in borderLineRun)
                        //{
                        //    _get_border_pointf_v8_1(item.bmp0, item.bmp1, item.rect0, item.Border, out glues[item.index]);

                        //}
                        Parallel.ForEach(borderLineRun, item =>
                        {
                            _get_border_pointf_v8_1(item.bmp0, item.bmp1, item.rect0, item.Border, out glues[item.index], _from_bmpinputSize_to_iSized());

                            //if (INI.chipUseAI)
                            //    _get_border_pointf_v8_1(item.bmp0, item.bmp1, item.rect0, item.Border, out glues[item.index]);
                            //else
                            //    _get_border_pointf_v6_1(item.bmp0, item.bmp1, item.rect0, item.Border, out glues[item.index]);

                        });

                        i = 0;
                        while (i < (int)BorderTypeEnum.COUNT)
                        {

                            //borderLineRun[i] = new BorderLineRunClass();
                            borderLineRun[i].bmp0.Dispose();
                            borderLineRun[i].bmp1.Dispose();

                            i++;
                        }
                        GC.Collect();

                        //if (INI.chipUseAI)
                        //    _get_border_pointf_v8_1(bitmap, PADTempRegion.bmpThreshold, rect, (BorderTypeEnum)i, out glues[i]);
                        //else
                        //    _get_border_pointf_v6_1(bitmap, PADTempRegion.bmpThreshold, rect, (BorderTypeEnum)i, out glues[i]);

                        break;
                    case PADChipSize.CHIP_V2:
                    case PADChipSize.CHIP_NORMAL:
                    default:

                        //SetDilatation3x3(ref PADTempRegion.bmpThreshold, 2);
                        //SetErosion3x3(ref PADTempRegion.bmpThreshold, 2);
                        //if (m_IsSaveTemp)
                        //{
                        //    PADTempRegion.bmpThreshold.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpThreshold" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                        //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                        //}
                        borderLineRun = new BorderLineRunClass[4];
                        i = 0;
                        while (i < (int)BorderTypeEnum.COUNT)
                        {

                            borderLineRun[i] = new BorderLineRunClass();
                            borderLineRun[i].bmp0 = (Bitmap)bitmap.Clone();// new Bitmap(bitmap);
                            borderLineRun[i].bmp1 = (Bitmap)PADTempRegion.bmpThreshold.Clone();//  new Bitmap(PADTempRegion.bmpThreshold);
                            borderLineRun[i].rect0 = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                            borderLineRun[i].Border = (BorderTypeEnum)i;
                            borderLineRun[i].index = i;

                            i++;
                        }

                        Parallel.ForEach(borderLineRun, item =>
                       {
                           _get_border_pointf_v3(item.bmp0, item.bmp1, item.rect0, item.Border, out glues[item.index]);

                       });

                        i = 0;
                        while (i < (int)BorderTypeEnum.COUNT)
                        {

                            //borderLineRun[i] = new BorderLineRunClass();
                            borderLineRun[i].bmp0.Dispose();
                            borderLineRun[i].bmp1.Dispose();

                            i++;
                        }
                        GC.Collect();

                        //_get_border_pointf_v3(bitmap, PADTempRegion.bmpThreshold, rect, (BorderTypeEnum)i, out glues[i]);
                        break;
                }


                Bitmap bmpglueout = new Bitmap(bmpinput);
                Graphics g = Graphics.FromImage(bmpglueout);
                bool m_ischeckgluepass = true;
                string ngstr = string.Empty;
                string measureStr = string.Empty;

                double GlueTmpMax = GlueMax * Resolution_Mil;
                double GlueTmpMin = GlueMin * Resolution_Mil;


                if (INI.CHIP_CAL_MODE == 2)
                {
                    ////上下
                    //double tbmin = (glues[(int)BorderTypeEnum.TOP].LengthMin + glues[(int)BorderTypeEnum.BOTTOM].LengthMax) / 2;
                    //double tbmax = (glues[(int)BorderTypeEnum.TOP].LengthMax + glues[(int)BorderTypeEnum.BOTTOM].LengthMin) / 2;
                    //glues[(int)BorderTypeEnum.TOP].LengthMin = Math.Min(tbmin, tbmax);
                    //glues[(int)BorderTypeEnum.BOTTOM].LengthMin = Math.Min(tbmin, tbmax);
                    //glues[(int)BorderTypeEnum.TOP].LengthMax = Math.Max(tbmin, tbmax);
                    //glues[(int)BorderTypeEnum.BOTTOM].LengthMax = Math.Max(tbmin, tbmax);
                    ////左右
                    //double lrmin = (glues[(int)BorderTypeEnum.LEFT].LengthMin + glues[(int)BorderTypeEnum.RIGHT].LengthMax) / 2;
                    //double lrmax = (glues[(int)BorderTypeEnum.LEFT].LengthMax + glues[(int)BorderTypeEnum.RIGHT].LengthMin) / 2;
                    //glues[(int)BorderTypeEnum.LEFT].LengthMin = Math.Min(lrmin, lrmax);
                    //glues[(int)BorderTypeEnum.RIGHT].LengthMin = Math.Min(lrmin, lrmax);
                    //glues[(int)BorderTypeEnum.LEFT].LengthMax = Math.Max(lrmin, lrmax);
                    //glues[(int)BorderTypeEnum.RIGHT].LengthMax = Math.Max(lrmin, lrmax);

                    //上下
                    double tbmin = (glues[(int)BorderTypeEnum.TOP].LengthMin + glues[(int)BorderTypeEnum.BOTTOM].LengthMin) / 2;
                    double tbmax = (glues[(int)BorderTypeEnum.TOP].LengthMax + glues[(int)BorderTypeEnum.BOTTOM].LengthMax) / 2;
                    glues[(int)BorderTypeEnum.TOP].LengthMin = glues[(int)BorderTypeEnum.TOP].LengthMin +
                        (tbmin - glues[(int)BorderTypeEnum.TOP].LengthMin) * 0.75;// + GetRandom(-0.01, 0.01);
                    glues[(int)BorderTypeEnum.BOTTOM].LengthMin = glues[(int)BorderTypeEnum.BOTTOM].LengthMin +
                        (tbmin - glues[(int)BorderTypeEnum.BOTTOM].LengthMin) * 0.85;// + GetRandom(-0.01, 0.01);
                    glues[(int)BorderTypeEnum.TOP].LengthMax = glues[(int)BorderTypeEnum.TOP].LengthMax +
                        (tbmax - glues[(int)BorderTypeEnum.TOP].LengthMax) * 0.85;// + GetRandom(-0.01, 0.01);
                    glues[(int)BorderTypeEnum.BOTTOM].LengthMax = glues[(int)BorderTypeEnum.BOTTOM].LengthMax +
                        (tbmax - glues[(int)BorderTypeEnum.BOTTOM].LengthMax) * 0.75;// + GetRandom(-0.01, 0.01);
                    //左右
                    double lrmin = (glues[(int)BorderTypeEnum.LEFT].LengthMin + glues[(int)BorderTypeEnum.RIGHT].LengthMin) / 2;
                    double lrmax = (glues[(int)BorderTypeEnum.LEFT].LengthMax + glues[(int)BorderTypeEnum.RIGHT].LengthMax) / 2;
                    glues[(int)BorderTypeEnum.LEFT].LengthMin = glues[(int)BorderTypeEnum.LEFT].LengthMin +
                        (lrmin - glues[(int)BorderTypeEnum.LEFT].LengthMin) * 0.83;// + GetRandom(-0.01, 0.01);
                    glues[(int)BorderTypeEnum.RIGHT].LengthMin = glues[(int)BorderTypeEnum.RIGHT].LengthMin +
                        (lrmin - glues[(int)BorderTypeEnum.RIGHT].LengthMin) * 0.73;// + GetRandom(-0.01, 0.01);
                    glues[(int)BorderTypeEnum.LEFT].LengthMax = glues[(int)BorderTypeEnum.LEFT].LengthMax +
                        (lrmax - glues[(int)BorderTypeEnum.LEFT].LengthMax) * 0.73;// + GetRandom(-0.01, 0.01);
                    glues[(int)BorderTypeEnum.RIGHT].LengthMax = glues[(int)BorderTypeEnum.RIGHT].LengthMax +
                        (lrmax - glues[(int)BorderTypeEnum.RIGHT].LengthMax) * 0.83;// + GetRandom(-0.01, 0.01);
                }



                i = 0;
                while (i < (int)BorderTypeEnum.COUNT)
                {

                    switch ((BorderTypeEnum)i)
                    {
                        case BorderTypeEnum.LEFT:
                            GlueTmpMax = GlueMaxLeft / INI.MAINSD_PAD_MIL_RESOLUTION;
                            GlueTmpMin = GlueMinLeft / INI.MAINSD_PAD_MIL_RESOLUTION;
                            break;
                        case BorderTypeEnum.TOP:
                            GlueTmpMax = GlueMaxTop / INI.MAINSD_PAD_MIL_RESOLUTION;
                            GlueTmpMin = GlueMinTop / INI.MAINSD_PAD_MIL_RESOLUTION;
                            break;
                        case BorderTypeEnum.RIGHT:
                            GlueTmpMax = GlueMaxRight / INI.MAINSD_PAD_MIL_RESOLUTION;
                            GlueTmpMin = GlueMinRight / INI.MAINSD_PAD_MIL_RESOLUTION;
                            break;
                        case BorderTypeEnum.BOTTOM:
                            GlueTmpMax = GlueMaxBottom / INI.MAINSD_PAD_MIL_RESOLUTION;
                            GlueTmpMin = GlueMinBottom / INI.MAINSD_PAD_MIL_RESOLUTION;
                            break;
                    }

                    double min = glues[i].LengthMin * INI.MAINSD_PAD_MIL_RESOLUTION;
                    double max = glues[i].LengthMax * INI.MAINSD_PAD_MIL_RESOLUTION;

                    //ngstr += ((BorderTypeEnum)i).ToString() + "_NG,";

                    switch ((BorderTypeEnum)i)
                    {
                        case BorderTypeEnum.LEFT:
                            measureStr += "左";// + Environment.NewLine;
                            break;
                        case BorderTypeEnum.RIGHT:
                            measureStr += "右";// + Environment.NewLine;
                            break;
                        case BorderTypeEnum.TOP:
                            measureStr += "上";// + Environment.NewLine;
                            break;
                        case BorderTypeEnum.BOTTOM:
                            measureStr += "下";// + Environment.NewLine;
                            break;
                    }

                    measureStr += "[min:" + min.ToString("0.000000") + " mm]";
                    measureStr += "[max:" + max.ToString("0.000000") + " mm]" + Environment.NewLine;

                    if (glues[i].LengthMax > (GlueTmpMax) || glues[i].LengthMin < (GlueTmpMin))
                    {
                        m_ischeckgluepass = false;

                        switch ((BorderTypeEnum)i)
                        {
                            case BorderTypeEnum.LEFT:
                                ngstr += "左";
                                break;
                            case BorderTypeEnum.RIGHT:
                                ngstr += "右";
                                break;
                            case BorderTypeEnum.TOP:
                                ngstr += "上";
                                break;
                            case BorderTypeEnum.BOTTOM:
                                ngstr += "下";
                                break;
                        }

                        ngstr += "[min:" + min.ToString("0.000000") + " mm]";
                        ngstr += "[max:" + max.ToString("0.000000") + " mm]";
                    }

                    i++;
                }

                //填写数据的区域
                RectangleF _rectF = new RectangleF(0, 0, 1100, 290);
                _rectF = new RectangleF(0, 0, 1100, 40);
                g.FillRectangle(Brushes.Black, _rectF);
                int linewidth = 1;
                int fontsize = 8;
                //画图 及 显示比对图
                i = 0;
                int drawIndex = 0;
                while (drawIndex < (int)BorderTypeEnum.COUNT)
                    //Parallel.For(0, (int)BorderTypeEnum.COUNT, (drawIndex) =>
                {
                    switch ((BorderTypeEnum)drawIndex)
                    {
                        case BorderTypeEnum.LEFT:
                            GlueTmpMax = GlueMaxLeft / INI.MAINSD_PAD_MIL_RESOLUTION;
                            GlueTmpMin = GlueMinLeft / INI.MAINSD_PAD_MIL_RESOLUTION;
                            break;
                        case BorderTypeEnum.TOP:
                            GlueTmpMax = GlueMaxTop / INI.MAINSD_PAD_MIL_RESOLUTION;
                            GlueTmpMin = GlueMinTop / INI.MAINSD_PAD_MIL_RESOLUTION;
                            break;
                        case BorderTypeEnum.RIGHT:
                            GlueTmpMax = GlueMaxRight / INI.MAINSD_PAD_MIL_RESOLUTION;
                            GlueTmpMin = GlueMinRight / INI.MAINSD_PAD_MIL_RESOLUTION;
                            break;
                        case BorderTypeEnum.BOTTOM:
                            GlueTmpMax = GlueMaxBottom / INI.MAINSD_PAD_MIL_RESOLUTION;
                            GlueTmpMin = GlueMinBottom / INI.MAINSD_PAD_MIL_RESOLUTION;
                            break;
                    }

                    //pts = glues[drawIndex].GetPointF();
                    //ptsIN = glues[drawIndex].GetPointFIN();

                    if (m_ischeckgluepass)
                    {
                        g.DrawLines(new Pen(Color.Lime, linewidth), glues[drawIndex].GetPointF());
                        g.DrawLines(new Pen(Color.Lime, linewidth), glues[drawIndex].GetPointFIN());
                        g.DrawString(measureStr, new Font("宋体", fontsize), Brushes.Lime, 2, 2);
                        //g.DrawString(measureStr, new Font("宋体", 22), Brushes.Lime, _rectF);
                    }
                    else
                    {
                        g.DrawLines(new Pen(Color.Red, linewidth), glues[drawIndex].GetPointF());
                        g.DrawLines(new Pen(Color.Red, linewidth), glues[drawIndex].GetPointFIN());
                        g.DrawString(measureStr, new Font("宋体", fontsize), Brushes.Red, 2, 2);
                        //g.DrawString(measureStr, new Font("宋体", 22), Brushes.Red, _rectF);
                    }

                    drawIndex++;
                }
                //);

                if (m_IsSaveTemp)
                {
                    g.DrawRectangle(new Pen(Color.Red), rect);
                    g.DrawRectangle(new Pen(Color.Blue), rectin);
                    g.DrawRectangle(new Pen(Color.Yellow), rectout);

                    bmpglueout.Save(Universal.CalTestPath + "\\" + _CalPageIndex() + RelateAnalyzeString + "bmpInputout" + ".png",
                        System.Drawing.Imaging.ImageFormat.Png);

                    bmpglueout.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpInputout" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                }
                //bmpglueout.Save("D:\\testtest\\" + RelateAnalyzeString + "bmpInputout" + ".png", System.Drawing.Imaging.ImageFormat.Png);

                bmpoutput = (Bitmap)bmpglueout.Clone();// new Bitmap(bmpglueout);
                g.Dispose();
                bmpglueout.Dispose();

                if (!m_ischeckgluepass)
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " Glue OVER " + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " Glue OVER " + Environment.NewLine;

                    reason = ReasonEnum.NG;
                    descstriing = "胶水异常" + ngstr;
                }
                m_RunDataOK = true;
            }

            #endregion

            //imginput.Dispose();
            //imgoutput.Dispose();

            runstatus.SetWorkStatus(bmpPattern,
                                                   bmpInput,
                                                   bmpoutput,
                                                   reason,
                                                   errorstring,
                                                   processstring,
                                                   PassInfo,
                                                   descstriing);

            RunStatusCollection.Add(runstatus);
            IsPass = isgood;
            m_DescStr = descstriing;
            if (INI.CHIP_forceALIGNRUN_pass)
            {
                if (descstriing == "无芯片")
                    isgood = true;
            }

            bmpMeasureOutput.Dispose();
            bmpMeasureOutput = new Bitmap(bmpoutput);

            bmpInput.Dispose();

            return isgood;
        }
        public bool PB10_GlueInspectionProcess_BlackEdge(Bitmap bmpinput, ref Bitmap bmpoutput)
        {
            //0.025/0.022 = 1 mil/1 pixel
            Resolution_Mil = 0.0254 / INI.MAINSD_PAD_MIL_RESOLUTION;
            m_DescStr = string.Empty;

            bmpMeasureOutput.Dispose();
            //bmpMeasureOutput = new Bitmap(bmpinput);
            bmpMeasureOutput = (Bitmap)bmpinput.Clone();

            if (PADMethod == PADMethodEnum.NONE)
            {
                bmpoutput.Dispose();
                bmpoutput = new Bitmap(bmpinput);
                IsPass = true;
                return true;
            }
            m_IsSaveTemp = INI.IsDEBUGCHIP;
            bool isgood = true;

            WorkStatusClass runstatus = new WorkStatusClass(AnanlyzeProcedureEnum.GLUEINSPECT);
            string processstring = "Start " + RelateAnalyzeString + " GLUEINSPECT BlackEdge Inspection." + Environment.NewLine;
            string errorstring = "";
            string descstriing = "";
            ReasonEnum reason = ReasonEnum.PASS;

            bmpInput = new Bitmap(bmpinput);

            //bmpPadFindOutput = new Bitmap(bmpinput);
            //bmpPadBolbOutput = new Bitmap(bmpinput);
            bmpPadFindOutput = (Bitmap)bmpinput.Clone();
            //bmpPadBolbOutput = (Bitmap)bmpinput.Clone();

            PADRegionClass PADTempRegion = new PADRegionClass();
            PADRegionFind_BlackEdge(bmpInput, PADGrayThreshold, false, out PADTempRegion);

            #region 判断有无胶水  VICTOR模式
            //60ms
            if (isgood)
            {
                int isized = 10;
                PointF fillCenterPointF = new PointF(0, 0);
                Bitmap bmpsize = PADTempRegion.GetChipGlue(-isized, out fillCenterPointF);//  new Bitmap(bmpinput, Resize(bmpinput.Size, -isized));
                AForge.Imaging.Filters.HistogramEqualization histogramEqualization11 =
                    new AForge.Imaging.Filters.HistogramEqualization();
                Bitmap bmphistogramEqualization11 = histogramEqualization11.Apply(bmpsize);

                //bmphistogramEqualization11 = FloodFill(bmphistogramEqualization11, 
                //        new Point(bmphistogramEqualization11.Width / 2, bmphistogramEqualization11.Height / 2),
                //                                                    Color.White, (int)(255 * NoGlueThresholdValue));

                bmphistogramEqualization11 = FloodFill(bmphistogramEqualization11,
                                                                                 new Point((int)fillCenterPointF.X, (int)fillCenterPointF.Y),
                                                                                 Color.White,
                                                                                 (int)(255 * NoGlueThresholdValue));

                AForge.Imaging.Filters.Grayscale grayscale11 =
                    new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
                bmphistogramEqualization11 = grayscale11.Apply(bmphistogramEqualization11);

                AForge.Imaging.Filters.ExtractBiggestBlob extractBiggestBlob11 =
                    new AForge.Imaging.Filters.ExtractBiggestBlob();
                bmphistogramEqualization11 = extractBiggestBlob11.Apply(bmphistogramEqualization11);

                if (m_IsSaveTemp)
                {
                    bmphistogramEqualization11.Save("D:\\testtest\\" + _CalPageIndex() +
                        RelateAnalyzeString + "NoDispensing" + ".png", System.Drawing.Imaging.ImageFormat.Png);

                }

                Rectangle rectangletemp = new Rectangle(extractBiggestBlob11.BlobPosition.X,
                                                                           extractBiggestBlob11.BlobPosition.Y,
                                                                           bmphistogramEqualization11.Width,
                                                                           bmphistogramEqualization11.Height);

                RectangleF rectangletemp2 = ResizeWithLocation3(rectangletemp, isized);

                double iwidthtmp = Math.Max(rectangletemp2.Width, rectangletemp2.Height);
                double iheighttmp = Math.Min(rectangletemp2.Width, rectangletemp2.Height);

                if (!IsInRangeRatio(m_PADRegion.RegionWidth, iwidthtmp, OWidthRatio))
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + rectangletemp2.Width.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + rectangletemp2.Width.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;
                    descstriing = "无胶";
                    reason = ReasonEnum.NG;

                    bmpoutput.Dispose();
                    bmpoutput = (Bitmap)bmpinput.Clone();// new Bitmap(bmpinput);
                    //jzToolsClass.DrawRect(bmpoutput, rect1pattrem, new Pen(Color.Red, 5));
                    //if (INI.CHIP_NG_SHOW)
                    //{
                    //    bmpoutput.Dispose();
                    //    bmpoutput = new Bitmap(bmpgray11);
                    //}
                }
                else if (!IsInRangeRatio(m_PADRegion.RegionHeight, iheighttmp, OHeightRatio))
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " PAD HEIGHT OVER Ratio= " + OHeightRatio.ToString() + " , " + rectangletemp2.Height.ToString() + " , " + m_PADRegion.RegionHeight.ToString() + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " PAD HEIGHT OVER Ratio= " + OHeightRatio.ToString() + " , " + rectangletemp2.Height.ToString() + " , " + m_PADRegion.RegionHeight.ToString() + Environment.NewLine;
                    descstriing = "无胶";
                    reason = ReasonEnum.NG;

                    bmpoutput.Dispose();
                    bmpoutput = (Bitmap)bmpinput.Clone();// new Bitmap(bmpinput);
                    //jzToolsClass.DrawRect(bmpoutput, rect1pattrem, new Pen(Color.Red, 5));
                    //if (INI.CHIP_NG_SHOW)
                    //{
                    //    bmpoutput.Dispose();
                    //    bmpoutput = new Bitmap(bmpgray11);
                    //}
                }


                bmphistogramEqualization11.Dispose();
                bmpsize.Dispose();
                //bmpfloodfill.Dispose();
                ////bmpgray11.Dispose();
                //bmpextractBiggestBlob11.Dispose();
            }

            #endregion


            #region 先判断PAD的尺寸是否正确 和 正确定位到 及 判断PAD 区域里面的blob

            if (isgood)
            {
                //166ms
                //PADRegionFind(bmpInput, PADGrayThreshold, false, out PADTempRegion);

                //if (!IsInRangeRatio(PADTempRegion.RegionArea, m_PADRegion.RegionArea, OAreaRatio))
                //{
                //    isgood = false;
                //    processstring += "Error in " + RelateAnalyzeString + " PAD AREA OVER Ratio= " + OAreaRatio.ToString() + " , " + PADTempRegion.RegionArea.ToString() + " , " + m_PADRegion.RegionArea.ToString() + Environment.NewLine;
                //    errorstring += RelateAnalyzeString + " PAD AREA OVER Ratio= " + OAreaRatio.ToString() + " , " + PADTempRegion.RegionArea.ToString() + " , " + m_PADRegion.RegionArea.ToString() + Environment.NewLine;

                //    reason = ReasonEnum.NG;
                //    descstriing = "尺寸面积超标";
                //}
                //else if (!IsInRangeRatio(PADTempRegion.RegionWidth, m_PADRegion.RegionWidth, OWidthRatio))
                if (!IsInRangeRatio(PADTempRegion.RegionWidth, m_PADRegion.RegionWidth, OWidthRatio))
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + PADTempRegion.RegionWidth.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + PADTempRegion.RegionWidth.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;

                    reason = ReasonEnum.NG;
                    descstriing = "尺寸宽度超标";
                }
                else if (!IsInRangeRatio(PADTempRegion.RegionHeight, m_PADRegion.RegionHeight, OHeightRatio))
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " PAD HEIGHT OVER Ratio= " + OHeightRatio.ToString() + " , " + PADTempRegion.RegionHeight.ToString() + " , " + m_PADRegion.RegionHeight.ToString() + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " PAD HEIGHT OVER Ratio= " + OHeightRatio.ToString() + " , " + PADTempRegion.RegionHeight.ToString() + " , " + m_PADRegion.RegionHeight.ToString() + Environment.NewLine;

                    reason = ReasonEnum.NG;
                    descstriing = "尺寸高度超标";
                }

                #region 判断PAD 区域里面的blob

                if (isgood)
                {
                    //78ms
                    PADRegionFindBlob(bmpInput, PADBlobGrayThreshold, PADTempRegion, ref bmpoutput, 3);

                    ////mil 計算

                    //m_BadArea = m_BadArea / Resolution_Mil / Resolution_Mil;
                    //m_BadWidth = m_BadWidth / Resolution_Mil;
                    //m_BadHeight = m_BadHeight / Resolution_Mil;

                    if (m_BadArea > CheckDArea)
                    {
                        isgood = false;
                        processstring += "Error in " + RelateAnalyzeString + " PAD AREA OVER area= " + " , " + m_BadArea.ToString() + " , " + CheckDArea.ToString() + Environment.NewLine;
                        errorstring += RelateAnalyzeString + " PAD AREA OVER area= " + " , " + m_BadArea.ToString() + " , " + CheckDArea.ToString() + Environment.NewLine;

                        reason = ReasonEnum.NG;
                        descstriing = "晶片表面溢胶";
                    }
                    else if (m_BadWidth > CheckDWidth)
                    {
                        isgood = false;
                        processstring += "Error in " + RelateAnalyzeString + " PAD WIDTH OVER WIDTH= " + " , " + m_BadWidth.ToString() + " , " + CheckDWidth.ToString() + Environment.NewLine;
                        errorstring += RelateAnalyzeString + " PAD WIDTH OVER WIDTH= " + " , " + m_BadWidth.ToString() + " , " + CheckDWidth.ToString() + Environment.NewLine;

                        reason = ReasonEnum.NG;
                        descstriing = "晶片表面溢胶";
                    }
                    else if (m_BadHeight > CheckDHeight)
                    {
                        isgood = false;
                        processstring += "Error in " + RelateAnalyzeString + " PAD HEIGHT OVER HEIGHT= " + " , " + m_BadHeight.ToString() + " , " + CheckDHeight.ToString() + Environment.NewLine;
                        errorstring += RelateAnalyzeString + " PAD HEIGHT OVER HEIGHT= " + " , " + m_BadHeight.ToString() + " , " + CheckDHeight.ToString() + Environment.NewLine;

                        reason = ReasonEnum.NG;
                        descstriing = "晶片表面溢胶";
                    }
                }

                if (!isgood)
                {
                    if (m_IsSaveTemp)
                    {
                        bmpPadFindOutput.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "PadFind" + (false ? "_Train" : "_Run") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                        //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);

                        //bmp.Save("D:\\testtest\\" + RelateAnalyzeString + "PadFind" + (eIsTrain ? "_Train" : "_Run") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    }
                }

                #endregion
            }

            #endregion

            //胶水的宽度

            #region 胶水宽度判断

            glues = null;
            if (isgood && GlueCheck)
            {
                int sized = 2;
                glues = new GlueRegionClass[(int)BorderTypeEnum.COUNT];
                PointF[] pts = null;
                PointF[] ptsIN = null;

                imginput = (Bitmap)bmpinput.Clone();// new Bitmap(bmpinput);
                imgmask = new Bitmap(bmpinput.Width, bmpinput.Height);

                Rectangle rect1 = new Rectangle(PADTempRegion.RegionForEdgeRect.X,
                                                                       PADTempRegion.RegionForEdgeRect.Y,
                                                                       PADTempRegion.RegionForEdgeRect.Width,
                                                                       PADTempRegion.RegionForEdgeRect.Height);
                rect1.Inflate(2, 2);



                Rectangle rect = new Rectangle(PADTempRegion.RegionForEdgeRect.X,
                                                                       PADTempRegion.RegionForEdgeRect.Y,
                                                                       PADTempRegion.RegionForEdgeRect.Width,
                                                                       PADTempRegion.RegionForEdgeRect.Height);


                Bitmap bitmap = (Bitmap)bmpInput.Clone();// new Bitmap(bmpInput);
                Bitmap bitmap0 = (Bitmap)bmpInput.Clone();//new Bitmap(bmpInput);
                if (m_IsSaveTemp)
                {
                    bitmap0.Save(Universal.CalTestPath + "\\" + _CalPageIndex() + RelateAnalyzeString + "bmpTrainCollect" + ".png",
                       System.Drawing.Imaging.ImageFormat.Png);


                    bitmap0.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bitmap0" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                }
                switch (PADChipSizeMode)
                {
                    case PADChipSize.CHIP_V8:
                        //bitmap = (Bitmap)_getV8bmpInput(rect1).Clone();
                        bitmap = new Bitmap(blackAICal(PADTempRegion, sized));
                        break;
                    case PADChipSize.CHIP_NORMAL:
                    default:
                        //bitmap = (Bitmap)blackNormal(PADTempRegion, 1).Clone();

                        Bitmap bmptempxxx = new Bitmap(blackNormal(PADTempRegion));
                        //bitmap = new Bitmap(blackNormal(PADTempRegion));
                        bitmap = new Bitmap(bmptempxxx, new Size(bmptempxxx.Width * 5, bmptempxxx.Height * 5));
                        bmptempxxx.Dispose();
                        break;
                }

                //这里是寻找点的起点位置
                //rect.Inflate(-5, -5);
                rect.Inflate(-(int)(CalExtendX * Resolution_Mil), -(int)(CalExtendY * Resolution_Mil));
                //rect.Inflate(-(int)(150 * Resolution_Mil), -(int)(150 * Resolution_Mil));

                //if (m_IsSaveTemp)
                //{
                //    bitmap.Save("D:\\testtest\\" + RelateAnalyzeString + "imginputorg" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                //}

                Rectangle rectin = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                //rectin.Inflate(5, 5);
                int minin = (int)(GlueMin * Resolution_Mil);
                rectin.Inflate(minin, minin);
                //g.DrawRectangle(new Pen(Color.Blue), rectin);

                Rectangle rectout = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                //rectout.Inflate(5, 5);
                int minout = (int)(GlueMax * Resolution_Mil);
                rectout.Inflate(minout, minout);
                //g.DrawRectangle(new Pen(Color.Yellow), rectout);


                switch (PADChipSizeMode)
                {
                    case PADChipSize.CHIP_V8:
                        //RectangleF rectangleFxxx = ResizeWithLocation2(rectout, -2);
                        //gx.DrawRectangle(new Pen(Color.Yellow), rectangleFxxx.X, rectangleFxxx.Y,
                        //    rectangleFxxx.Width, rectangleFxxx.Height);
                        break;
                    default:
                        Graphics gx = Graphics.FromImage(bitmap);
                        gx.DrawRectangle(new Pen(Color.Yellow), rectout);
                        gx.Dispose();
                        break;
                }

                int i = 0;
                int j = 0;
                
                //多线程计算4条边
                switch (PADChipSizeMode)
                {
                    case PADChipSize.CHIP_V8:
                      
                        if (m_IsSaveTemp)
                        {
                            PADTempRegion.bmpThreshold.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpThreshold" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                            //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                        }
                        borderLineRun = new BorderLineRunClass[4];
                        i = 0;
                        while (i < (int)BorderTypeEnum.COUNT)
                        {

                            borderLineRun[i] = new BorderLineRunClass();
                            borderLineRun[i].bmp0 = (Bitmap)bitmap.Clone();// new Bitmap(bitmap);
                            borderLineRun[i].bmp1 = new Bitmap(PADTempRegion.bmpThreshold,
                                Resize(PADTempRegion.bmpThreshold.Size, -sized));

                            RectangleF rectSize = ResizeWithLocation2(rect, -sized);

                            //(Bitmap)PADTempRegion.bmpThreshold.Clone();// new Bitmap(PADTempRegion.bmpThreshold);
                            borderLineRun[i].rect0 =
                                new Rectangle((int)rectSize.X, (int)rectSize.Y, (int)rectSize.Width, (int)rectSize.Height);
                            //new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                            borderLineRun[i].Border = (BorderTypeEnum)i;
                            borderLineRun[i].index = i;

                            i++;
                        }

                        //foreach (BorderLineRunClass item in borderLineRun)
                        //{
                        //    _get_border_pointf_v8_1(item.bmp0, item.bmp1, item.rect0, item.Border, out glues[item.index]);

                        //}
                        Parallel.ForEach(borderLineRun, item =>
                        {
                            _get_border_pointf_v8_1(item.bmp0, item.bmp1, item.rect0, item.Border, out glues[item.index], sized);

                            //if (INI.chipUseAI)
                            //    _get_border_pointf_v8_1(item.bmp0, item.bmp1, item.rect0, item.Border, out glues[item.index]);
                            //else
                            //    _get_border_pointf_v6_1(item.bmp0, item.bmp1, item.rect0, item.Border, out glues[item.index]);

                        });

                        i = 0;
                        while (i < (int)BorderTypeEnum.COUNT)
                        {

                            //borderLineRun[i] = new BorderLineRunClass();
                            borderLineRun[i].bmp0.Dispose();
                            borderLineRun[i].bmp1.Dispose();

                            i++;
                        }
                        GC.Collect();

                        //if (INI.chipUseAI)
                        //    _get_border_pointf_v8_1(bitmap, PADTempRegion.bmpThreshold, rect, (BorderTypeEnum)i, out glues[i]);
                        //else
                        //    _get_border_pointf_v6_1(bitmap, PADTempRegion.bmpThreshold, rect, (BorderTypeEnum)i, out glues[i]);

                        break;
                    case PADChipSize.CHIP_NORMAL:
                    default:

                        //SetDilatation3x3(ref PADTempRegion.bmpThreshold, 2);
                        //SetErosion3x3(ref PADTempRegion.bmpThreshold, 2);

                        borderLineRun = new BorderLineRunClass[4];
                        i = 0;
                        while (i < (int)BorderTypeEnum.COUNT)
                        {

                            borderLineRun[i] = new BorderLineRunClass();
                            borderLineRun[i].bmp0 = (Bitmap)bitmap.Clone();// new Bitmap(bitmap);
                            borderLineRun[i].bmp1 = (Bitmap)PADTempRegion.bmpThreshold.Clone();//  new Bitmap(PADTempRegion.bmpThreshold);
                            borderLineRun[i].rect0 = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                            borderLineRun[i].Border = (BorderTypeEnum)i;
                            borderLineRun[i].index = i;

                            i++;
                        }

                        Parallel.ForEach(borderLineRun, item =>
                        {
                            _get_border_pointf_v3(item.bmp0, item.bmp1, item.rect0, item.Border, out glues[item.index]);

                        });

                        i = 0;
                        while (i < (int)BorderTypeEnum.COUNT)
                        {

                            //borderLineRun[i] = new BorderLineRunClass();
                            borderLineRun[i].bmp0.Dispose();
                            borderLineRun[i].bmp1.Dispose();

                            i++;
                        }
                        GC.Collect();

                        //_get_border_pointf_v3(bitmap, PADTempRegion.bmpThreshold, rect, (BorderTypeEnum)i, out glues[i]);
                        break;
                }


                Bitmap bmpglueout = new Bitmap(bmpinput);
                Graphics g = Graphics.FromImage(bmpglueout);
                bool m_ischeckgluepass = true;
                string ngstr = string.Empty;
                string measureStr = string.Empty;

                double GlueTmpMax = GlueMax * Resolution_Mil;
                double GlueTmpMin = GlueMin * Resolution_Mil;


                if (INI.CHIP_CAL_MODE == 2)
                {
                    ////上下
                    //double tbmin = (glues[(int)BorderTypeEnum.TOP].LengthMin + glues[(int)BorderTypeEnum.BOTTOM].LengthMax) / 2;
                    //double tbmax = (glues[(int)BorderTypeEnum.TOP].LengthMax + glues[(int)BorderTypeEnum.BOTTOM].LengthMin) / 2;
                    //glues[(int)BorderTypeEnum.TOP].LengthMin = Math.Min(tbmin, tbmax);
                    //glues[(int)BorderTypeEnum.BOTTOM].LengthMin = Math.Min(tbmin, tbmax);
                    //glues[(int)BorderTypeEnum.TOP].LengthMax = Math.Max(tbmin, tbmax);
                    //glues[(int)BorderTypeEnum.BOTTOM].LengthMax = Math.Max(tbmin, tbmax);
                    ////左右
                    //double lrmin = (glues[(int)BorderTypeEnum.LEFT].LengthMin + glues[(int)BorderTypeEnum.RIGHT].LengthMax) / 2;
                    //double lrmax = (glues[(int)BorderTypeEnum.LEFT].LengthMax + glues[(int)BorderTypeEnum.RIGHT].LengthMin) / 2;
                    //glues[(int)BorderTypeEnum.LEFT].LengthMin = Math.Min(lrmin, lrmax);
                    //glues[(int)BorderTypeEnum.RIGHT].LengthMin = Math.Min(lrmin, lrmax);
                    //glues[(int)BorderTypeEnum.LEFT].LengthMax = Math.Max(lrmin, lrmax);
                    //glues[(int)BorderTypeEnum.RIGHT].LengthMax = Math.Max(lrmin, lrmax);

                    //上下
                    double tbmin = (glues[(int)BorderTypeEnum.TOP].LengthMin + glues[(int)BorderTypeEnum.BOTTOM].LengthMin) / 2;
                    double tbmax = (glues[(int)BorderTypeEnum.TOP].LengthMax + glues[(int)BorderTypeEnum.BOTTOM].LengthMax) / 2;
                    glues[(int)BorderTypeEnum.TOP].LengthMin = glues[(int)BorderTypeEnum.TOP].LengthMin +
                        (tbmin - glues[(int)BorderTypeEnum.TOP].LengthMin) * 0.75;// + GetRandom(-0.01, 0.01);
                    glues[(int)BorderTypeEnum.BOTTOM].LengthMin = glues[(int)BorderTypeEnum.BOTTOM].LengthMin +
                        (tbmin - glues[(int)BorderTypeEnum.BOTTOM].LengthMin) * 0.85;// + GetRandom(-0.01, 0.01);
                    glues[(int)BorderTypeEnum.TOP].LengthMax = glues[(int)BorderTypeEnum.TOP].LengthMax +
                        (tbmax - glues[(int)BorderTypeEnum.TOP].LengthMax) * 0.85;// + GetRandom(-0.01, 0.01);
                    glues[(int)BorderTypeEnum.BOTTOM].LengthMax = glues[(int)BorderTypeEnum.BOTTOM].LengthMax +
                        (tbmax - glues[(int)BorderTypeEnum.BOTTOM].LengthMax) * 0.75;// + GetRandom(-0.01, 0.01);
                    //左右
                    double lrmin = (glues[(int)BorderTypeEnum.LEFT].LengthMin + glues[(int)BorderTypeEnum.RIGHT].LengthMin) / 2;
                    double lrmax = (glues[(int)BorderTypeEnum.LEFT].LengthMax + glues[(int)BorderTypeEnum.RIGHT].LengthMax) / 2;
                    glues[(int)BorderTypeEnum.LEFT].LengthMin = glues[(int)BorderTypeEnum.LEFT].LengthMin +
                        (lrmin - glues[(int)BorderTypeEnum.LEFT].LengthMin) * 0.83;// + GetRandom(-0.01, 0.01);
                    glues[(int)BorderTypeEnum.RIGHT].LengthMin = glues[(int)BorderTypeEnum.RIGHT].LengthMin +
                        (lrmin - glues[(int)BorderTypeEnum.RIGHT].LengthMin) * 0.73;// + GetRandom(-0.01, 0.01);
                    glues[(int)BorderTypeEnum.LEFT].LengthMax = glues[(int)BorderTypeEnum.LEFT].LengthMax +
                        (lrmax - glues[(int)BorderTypeEnum.LEFT].LengthMax) * 0.73;// + GetRandom(-0.01, 0.01);
                    glues[(int)BorderTypeEnum.RIGHT].LengthMax = glues[(int)BorderTypeEnum.RIGHT].LengthMax +
                        (lrmax - glues[(int)BorderTypeEnum.RIGHT].LengthMax) * 0.83;// + GetRandom(-0.01, 0.01);
                }



                i = 0;
                while (i < (int)BorderTypeEnum.COUNT)
                {

                    switch ((BorderTypeEnum)i)
                    {
                        case BorderTypeEnum.LEFT:
                            GlueTmpMax = GlueMaxLeft / INI.MAINSD_PAD_MIL_RESOLUTION;
                            GlueTmpMin = GlueMinLeft / INI.MAINSD_PAD_MIL_RESOLUTION;
                            break;
                        case BorderTypeEnum.TOP:
                            GlueTmpMax = GlueMaxTop / INI.MAINSD_PAD_MIL_RESOLUTION;
                            GlueTmpMin = GlueMinTop / INI.MAINSD_PAD_MIL_RESOLUTION;
                            break;
                        case BorderTypeEnum.RIGHT:
                            GlueTmpMax = GlueMaxRight / INI.MAINSD_PAD_MIL_RESOLUTION;
                            GlueTmpMin = GlueMinRight / INI.MAINSD_PAD_MIL_RESOLUTION;
                            break;
                        case BorderTypeEnum.BOTTOM:
                            GlueTmpMax = GlueMaxBottom / INI.MAINSD_PAD_MIL_RESOLUTION;
                            GlueTmpMin = GlueMinBottom / INI.MAINSD_PAD_MIL_RESOLUTION;
                            break;
                    }

                    double min = glues[i].LengthMin * INI.MAINSD_PAD_MIL_RESOLUTION;
                    double max = glues[i].LengthMax * INI.MAINSD_PAD_MIL_RESOLUTION;

                    //ngstr += ((BorderTypeEnum)i).ToString() + "_NG,";

                    switch ((BorderTypeEnum)i)
                    {
                        case BorderTypeEnum.LEFT:
                            measureStr += "左";// + Environment.NewLine;
                            break;
                        case BorderTypeEnum.RIGHT:
                            measureStr += "右";// + Environment.NewLine;
                            break;
                        case BorderTypeEnum.TOP:
                            measureStr += "上";// + Environment.NewLine;
                            break;
                        case BorderTypeEnum.BOTTOM:
                            measureStr += "下";// + Environment.NewLine;
                            break;
                    }

                    measureStr += "[min:" + min.ToString("0.000000") + " mm]";
                    measureStr += "[max:" + max.ToString("0.000000") + " mm]" + Environment.NewLine;

                    if (glues[i].LengthMax > (GlueTmpMax) || glues[i].LengthMin < (GlueTmpMin))
                    {
                        m_ischeckgluepass = false;

                        switch ((BorderTypeEnum)i)
                        {
                            case BorderTypeEnum.LEFT:
                                ngstr += "左";
                                break;
                            case BorderTypeEnum.RIGHT:
                                ngstr += "右";
                                break;
                            case BorderTypeEnum.TOP:
                                ngstr += "上";
                                break;
                            case BorderTypeEnum.BOTTOM:
                                ngstr += "下";
                                break;
                        }

                        ngstr += "[min:" + min.ToString("0.000000") + " mm]";
                        ngstr += "[max:" + max.ToString("0.000000") + " mm]";
                    }

                    i++;
                }

                //填写数据的区域
                RectangleF _rectF = new RectangleF(0, 0, 1100, 290);
                g.FillRectangle(Brushes.Black, _rectF);

                //画图 及 显示比对图
                i = 0;
                int drawIndex = 0;
                while (drawIndex < (int)BorderTypeEnum.COUNT)
                //Parallel.For(0, (int)BorderTypeEnum.COUNT, (drawIndex) =>
                {
                    switch ((BorderTypeEnum)drawIndex)
                    {
                        case BorderTypeEnum.LEFT:
                            GlueTmpMax = GlueMaxLeft / INI.MAINSD_PAD_MIL_RESOLUTION;
                            GlueTmpMin = GlueMinLeft / INI.MAINSD_PAD_MIL_RESOLUTION;
                            break;
                        case BorderTypeEnum.TOP:
                            GlueTmpMax = GlueMaxTop / INI.MAINSD_PAD_MIL_RESOLUTION;
                            GlueTmpMin = GlueMinTop / INI.MAINSD_PAD_MIL_RESOLUTION;
                            break;
                        case BorderTypeEnum.RIGHT:
                            GlueTmpMax = GlueMaxRight / INI.MAINSD_PAD_MIL_RESOLUTION;
                            GlueTmpMin = GlueMinRight / INI.MAINSD_PAD_MIL_RESOLUTION;
                            break;
                        case BorderTypeEnum.BOTTOM:
                            GlueTmpMax = GlueMaxBottom / INI.MAINSD_PAD_MIL_RESOLUTION;
                            GlueTmpMin = GlueMinBottom / INI.MAINSD_PAD_MIL_RESOLUTION;
                            break;
                    }

                    //pts = glues[drawIndex].GetPointF();
                    //ptsIN = glues[drawIndex].GetPointFIN();

                    if (m_ischeckgluepass)
                    {
                        g.DrawLines(new Pen(Color.Lime, 5), glues[drawIndex].GetPointF());
                        g.DrawLines(new Pen(Color.Lime, 5), glues[drawIndex].GetPointFIN());
                        g.DrawString(measureStr, new Font("宋体", 43), Brushes.Lime, 5, 5);
                        //g.DrawString(measureStr, new Font("宋体", 22), Brushes.Lime, _rectF);
                    }
                    else
                    {
                        g.DrawLines(new Pen(Color.Red, 5), glues[drawIndex].GetPointF());
                        g.DrawLines(new Pen(Color.Red, 5), glues[drawIndex].GetPointFIN());
                        g.DrawString(measureStr, new Font("宋体", 43), Brushes.Red, 5, 5);
                        //g.DrawString(measureStr, new Font("宋体", 22), Brushes.Red, _rectF);
                    }

                    drawIndex++;
                }
                //);

                if (m_IsSaveTemp)
                {
                    g.DrawRectangle(new Pen(Color.Red), rect);
                    g.DrawRectangle(new Pen(Color.Blue), rectin);
                    g.DrawRectangle(new Pen(Color.Yellow), rectout);

                    bmpglueout.Save(Universal.CalTestPath + "\\" + _CalPageIndex() + RelateAnalyzeString + "bmpInputout" + ".png",
                        System.Drawing.Imaging.ImageFormat.Png);

                    bmpglueout.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpInputout" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                }
                //bmpglueout.Save("D:\\testtest\\" + RelateAnalyzeString + "bmpInputout" + ".png", System.Drawing.Imaging.ImageFormat.Png);

                bmpoutput = (Bitmap)bmpglueout.Clone();// new Bitmap(bmpglueout);
                g.Dispose();
                bmpglueout.Dispose();

                if (!m_ischeckgluepass)
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " Glue OVER " + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " Glue OVER " + Environment.NewLine;

                    reason = ReasonEnum.NG;
                    descstriing = "胶水异常" + ngstr;
                }
                m_RunDataOK = true;
            }

            #endregion

            //imginput.Dispose();
            //imgoutput.Dispose();

            runstatus.SetWorkStatus(bmpPattern,
                                                   bmpInput,
                                                   bmpoutput,
                                                   reason,
                                                   errorstring,
                                                   processstring,
                                                   PassInfo,
                                                   descstriing);

            RunStatusCollection.Add(runstatus);
            IsPass = isgood;
            m_DescStr = descstriing;
            if (INI.CHIP_forceALIGNRUN_pass)
            {
                if (descstriing == "无芯片")
                    isgood = true;
            }

            bmpMeasureOutput.Dispose();
            bmpMeasureOutput = new Bitmap(bmpoutput);

            bmpInput.Dispose();

            return isgood;
        }
        public bool PB10_GlacodeInspectionProcess(Bitmap bmpinput, ref Bitmap bmpoutput)
        {
            //0.025/0.022 = 1 mil/1 pixel
            Resolution_Mil = 0.0254 / INI.MAINSD_PAD_MIL_RESOLUTION;
            m_DescStr = string.Empty;

            bmpMeasureOutput.Dispose();
            //bmpMeasureOutput = new Bitmap(bmpinput);
            bmpMeasureOutput = (Bitmap)bmpinput.Clone();

            ////胶水检测使用mm换算单位
            //switch (Universal.OPTION)
            //{
            //    case OptionEnum.MAIN_SDM1:
            //    case OptionEnum.MAIN_SDM2:
            //        Resolution_Mil = INI.MAINSD_PAD_MIL_RESOLUTION;
            //        break;
            //}

            if (PADMethod == PADMethodEnum.NONE)
            {
                bmpoutput.Dispose();
                bmpoutput = new Bitmap(bmpinput);
                IsPass = true;
                return true;
            }

            m_IsSaveTemp = INI.IsDEBUGCHIP;
            bool isgood = true;

            WorkStatusClass runstatus = new WorkStatusClass(AnanlyzeProcedureEnum.GLUEINSPECT);
            string processstring = "Start " + RelateAnalyzeString + " PAD Inspection." + Environment.NewLine;
            string errorstring = "";
            string descstriing = "";
            ReasonEnum reason = ReasonEnum.PASS;

            bmpInput = new Bitmap(bmpinput);

            //bmpPadFindOutput = new Bitmap(bmpinput);
            //bmpPadBolbOutput = new Bitmap(bmpinput);
            bmpPadFindOutput = (Bitmap)bmpinput.Clone();
            //bmpPadBolbOutput = (Bitmap)bmpinput.Clone();

            #region 先判断PAD的尺寸是否正确 和 正确定位到 及 判断PAD 区域里面的blob

            PADRegionClass PADTempRegion = new PADRegionClass();

            if (isgood)
            {
                //166ms
                PADRegionFind(bmpInput, PADGrayThreshold, false, out PADTempRegion);

                if (!IsInRangeRatio(PADTempRegion.RegionArea, m_PADRegion.RegionArea, OAreaRatio))
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " PAD AREA OVER Ratio= " + OAreaRatio.ToString() + " , " + PADTempRegion.RegionArea.ToString() + " , " + m_PADRegion.RegionArea.ToString() + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " PAD AREA OVER Ratio= " + OAreaRatio.ToString() + " , " + PADTempRegion.RegionArea.ToString() + " , " + m_PADRegion.RegionArea.ToString() + Environment.NewLine;

                    reason = ReasonEnum.NG;
                    descstriing = "基板边角缺陷";
                }
                else if (!IsInRangeRatio(PADTempRegion.RegionWidth, m_PADRegion.RegionWidth, OWidthRatio))
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + PADTempRegion.RegionWidth.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + PADTempRegion.RegionWidth.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;

                    reason = ReasonEnum.NG;
                    descstriing = "基板边角缺陷";
                }
                else if (!IsInRangeRatio(PADTempRegion.RegionHeight, m_PADRegion.RegionHeight, OHeightRatio))
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " PAD HEIGHT OVER Ratio= " + OHeightRatio.ToString() + " , " + PADTempRegion.RegionHeight.ToString() + " , " + m_PADRegion.RegionHeight.ToString() + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " PAD HEIGHT OVER Ratio= " + OHeightRatio.ToString() + " , " + PADTempRegion.RegionHeight.ToString() + " , " + m_PADRegion.RegionHeight.ToString() + Environment.NewLine;

                    reason = ReasonEnum.NG;
                    descstriing = "基板边角缺陷";
                }

                #region 判断PAD 区域里面的blob

                //if (isgood)
                {
                    //78ms
                    PADRegionFindBlob_Glacode(bmpInput, PADBlobGrayThreshold, PADTempRegion, ref bmpoutput, 1, true, m_listRectFMask);

                    ////mil 計算

                    //m_BadArea = m_BadArea / Resolution_Mil / Resolution_Mil;
                    //m_BadWidth = m_BadWidth / Resolution_Mil;
                    //m_BadHeight = m_BadHeight / Resolution_Mil;

                    if (m_BadArea > CheckDArea)
                    {
                        isgood = false;
                        processstring += "Error in " + RelateAnalyzeString + " PAD AREA OVER area= " + " , " + m_BadArea.ToString() + " , " + CheckDArea.ToString() + Environment.NewLine;
                        errorstring += RelateAnalyzeString + " PAD AREA OVER area= " + " , " + m_BadArea.ToString() + " , " + CheckDArea.ToString() + Environment.NewLine;

                        reason = ReasonEnum.NG;
                        descstriing = "基本表面边角缺陷";
                    }
                    else if (m_BadWidth > CheckDWidth)
                    {
                        isgood = false;
                        processstring += "Error in " + RelateAnalyzeString + " PAD WIDTH OVER WIDTH= " + " , " + m_BadWidth.ToString() + " , " + CheckDWidth.ToString() + Environment.NewLine;
                        errorstring += RelateAnalyzeString + " PAD WIDTH OVER WIDTH= " + " , " + m_BadWidth.ToString() + " , " + CheckDWidth.ToString() + Environment.NewLine;

                        reason = ReasonEnum.NG;
                        descstriing = "基本表面边角缺陷";
                    }
                    else if (m_BadHeight > CheckDHeight)
                    {
                        isgood = false;
                        processstring += "Error in " + RelateAnalyzeString + " PAD HEIGHT OVER HEIGHT= " + " , " + m_BadHeight.ToString() + " , " + CheckDHeight.ToString() + Environment.NewLine;
                        errorstring += RelateAnalyzeString + " PAD HEIGHT OVER HEIGHT= " + " , " + m_BadHeight.ToString() + " , " + CheckDHeight.ToString() + Environment.NewLine;

                        reason = ReasonEnum.NG;
                        descstriing = "基本表面边角缺陷";
                    }
                }

                #endregion
            }

            #endregion

            if (!isgood)
            {
                if (m_IsSaveTemp)
                {
                    bmpPadFindOutput.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "PlacaodeFind" + (false ? "_Train" : "_Run") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);

                    //bmp.Save("D:\\testtest\\" + RelateAnalyzeString + "PadFind" + (eIsTrain ? "_Train" : "_Run") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                }
            }

            //imginput.Dispose();
            //imgoutput.Dispose();

            runstatus.SetWorkStatus(bmpPattern,
                                                   bmpInput,
                                                   bmpoutput,
                                                   reason,
                                                   errorstring,
                                                   processstring,
                                                   PassInfo,
                                                   descstriing);

            RunStatusCollection.Add(runstatus);
            IsPass = isgood;
            m_DescStr = descstriing;
            //if (INI.CHIP_forceALIGNRUN_pass)
            //{
            //    if (descstriing == "无芯片")
            //        isgood = true;
            //}

            bmpMeasureOutput.Dispose();
            bmpMeasureOutput = new Bitmap(bmpoutput);

            bmpInput.Dispose();

            return isgood;
        }
        #region BAK_20231117
        /*
         * 
         public bool PB10_GlueInspectionProcess(Bitmap bmpinput, ref Bitmap bmpoutput)
        {
            //0.025/0.022 = 1 mil/1 pixel
            Resolution_Mil = 0.0254 / INI.MAINSD_PAD_MIL_RESOLUTION;
            m_DescStr = string.Empty;

            bmpMeasureOutput.Dispose();
            bmpMeasureOutput = new Bitmap(bmpinput);

            ////胶水检测使用mm换算单位
            //switch (Universal.OPTION)
            //{
            //    case OptionEnum.MAIN_SDM1:
            //    case OptionEnum.MAIN_SDM2:
            //        Resolution_Mil = INI.MAINSD_PAD_MIL_RESOLUTION;
            //        break;
            //}

            if (PADMethod == PADMethodEnum.NONE)
            {
                bmpoutput.Dispose();
                bmpoutput = new Bitmap(bmpinput);
                IsPass = true;
                return true;
            }
            p1 = new PointF(2710, 5.55f);
            p2 = new PointF(4257, 7.55f);
            lineClass_top = new LineClass(p1, p2);

            p1 = new PointF(1180, 7.75f);
            p2 = new PointF(4257, 5.55f);
            lineClass_bottom = new LineClass(p1, p2);

            p1 = new PointF(450, 5.55f);
            p2 = new PointF(3250, 7.55f);
            lineClass_left = new LineClass(p1, p2);

            p1 = new PointF(1600, 7.55f);
            p2 = new PointF(2850, 5.55f);
            lineClass_right = new LineClass(p1, p2);

            m_IsSaveTemp = INI.IsDEBUGCHIP;
            bool isgood = true;

            WorkStatusClass runstatus = new WorkStatusClass(AnanlyzeProcedureEnum.GLUEINSPECT);
            string processstring = "Start " + RelateAnalyzeString + " PAD Inspection." + Environment.NewLine;
            string errorstring = "";
            string descstriing = "";
            ReasonEnum reason = ReasonEnum.PASS;

            bmpInput = new Bitmap(bmpinput);

            bmpPadFindOutput = new Bitmap(bmpinput);
            bmpPadBolbOutput = new Bitmap(bmpinput);

            //先判断有无芯片
            Point centertemp = new Point(bmpinput.Width / 2, bmpinput.Height / 2);
            Rectangle rectcenter = SimpleRect(centertemp, bmpinput.Width / 5, bmpinput.Height / 5);
            Bitmap bmp0000 = bmpinput.Clone(rectcenter, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            AForge.Imaging.Filters.Grayscale grayscale = AForge.Imaging.Filters.Grayscale.CommonAlgorithms.Y;
            bmp0000 = grayscale.Apply(bmp0000);
            //HistogramClass histogramClass = new HistogramClass(2);
            //histogramClass.GetHistogram(bmp0000);
            //AForge.Imaging.Filters.OtsuThreshold otsuThreshold = new AForge.Imaging.Filters.OtsuThreshold();
            //Bitmap bmp0002 = otsuThreshold.Apply(bmp0001);
            //AForge.Imaging.Filters.Threshold threshold = new AForge.Imaging.Filters.Threshold();
            //Bitmap bmp0002 = threshold.Apply(bmp0001);

            AForge.Imaging.Filters.HistogramEqualization histogramEqualization =
                new AForge.Imaging.Filters.HistogramEqualization();
            bmp0000 = histogramEqualization.Apply(bmp0000);

            //AForge.Imaging.Filters.Invert invert = new AForge.Imaging.Filters.Invert();
            //Bitmap bmp0003 = invert.Apply(bmp0002);

            AForge.Imaging.Filters.Threshold threshold = new AForge.Imaging.Filters.Threshold(20);
            bmp0000 = threshold.Apply(bmp0000);

            JzFindObjectClass jzFindObjectClass = new JzFindObjectClass();
            //jzFindObjectClass.AH_SetThreshold(ref bmp0003, 20);
            jzFindObjectClass.AH_FindBlob(bmp0000, false);

            if (jzFindObjectClass.FoundList.Count > 5)
            {
                isgood = false;
                processstring += "Error in " + RelateAnalyzeString + "no chip" + Environment.NewLine;
                errorstring += RelateAnalyzeString + "no chip" + Environment.NewLine;
                descstriing = "无芯片";
                reason = ReasonEnum.NG;
                if (INI.CHIP_forceALIGNRUN_pass)
                    reason = ReasonEnum.PASS;

                bmpoutput.Dispose();
                bmpoutput = new Bitmap(bmpinput);
            }
            if (m_IsSaveTemp)
            {
                bmp0000.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString
                    + "bmp0003_none_glue" + jzFindObjectClass.FoundList.Count.ToString()
                    + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
            }


            #region 判断有无胶水  VICTOR模式

            if (isgood)
            {
                AForge.Imaging.Filters.HistogramEqualization histogramEqualization11 = new AForge.Imaging.Filters.HistogramEqualization();
                Bitmap bmphistogramEqualization11 = histogramEqualization11.Apply(bmpinput);

                //if (m_IsSaveTemp)
                //{
                //    bmphistogramEqualization11.Save("D:\\testtest\\" + RelateAnalyzeString + "bmphistogramEqualization11" + (false ? "_Train" : "_Run") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //}

                Bitmap bmpfloodfill = FloodFill(bmphistogramEqualization11, new Point(bmphistogramEqualization11.Width / 2, bmphistogramEqualization11.Height / 2),
                                                                    Color.White, (int)(255 * NoGlueThresholdValue));

                AForge.Imaging.Filters.Grayscale grayscale11 = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
                Bitmap bmpgray11 = grayscale11.Apply(bmpfloodfill);

                //if (m_IsSaveTemp)
                //{
                //    bmpgray11.Save("D:\\testtest\\" + RelateAnalyzeString + "bmpgray11" + (false ? "_Train" : "_Run") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //}

                //AForge.Imaging.Filters.Threshold threshold11 = new AForge.Imaging.Filters.Threshold();
                //threshold11.ThresholdValue = (int)(255 * 0.3);
                //Bitmap bmpthreshold11 = threshold11.Apply(bmphistogramEqualization11);
                AForge.Imaging.Filters.ExtractBiggestBlob extractBiggestBlob11 = new AForge.Imaging.Filters.ExtractBiggestBlob();
                Bitmap bmpextractBiggestBlob11 = extractBiggestBlob11.Apply(bmpgray11);

                int iwidthtmp = Math.Max(bmpextractBiggestBlob11.Width, bmpextractBiggestBlob11.Height);
                int iheighttmp = Math.Min(bmpextractBiggestBlob11.Width, bmpextractBiggestBlob11.Height);

                Rectangle rect1pattrem = new Rectangle(m_PADRegion.RegionForEdgeRect.X,
                                                                          m_PADRegion.RegionForEdgeRect.Y,
                                                                          m_PADRegion.RegionForEdgeRect.Width,
                                                                          m_PADRegion.RegionForEdgeRect.Height);

                JzToolsClass jzToolsClass = new JzToolsClass();

                if (!IsInRangeRatio(iwidthtmp, m_PADRegion.RegionWidth, OWidthRatio))
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + bmpextractBiggestBlob11.Width.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + bmpextractBiggestBlob11.Width.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;
                    descstriing = "无胶";
                    reason = ReasonEnum.NG;
                    //if (Math.Abs(bmpinput.Width - iwidthtmp) <= 10 || Math.Abs(bmpinput.Height - iheighttmp) <= 10)
                    //{
                    //    descstriing = "无芯片";
                    //    if (INI.CHIP_forceALIGNRUN_pass)
                    //        reason = ReasonEnum.PASS;
                    //}

                    bmpoutput.Dispose();
                    bmpoutput = new Bitmap(bmpinput);
                    jzToolsClass.DrawRect(bmpoutput, rect1pattrem, new Pen(Color.Red, 5));
                    if (INI.CHIP_NG_SHOW)
                    {
                        bmpoutput.Dispose();
                        bmpoutput = new Bitmap(bmpgray11);
                    }
                }
                else if (!IsInRangeRatio(iheighttmp, m_PADRegion.RegionHeight, OHeightRatio))
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " PAD HEIGHT OVER Ratio= " + OHeightRatio.ToString() + " , " + bmpextractBiggestBlob11.Height.ToString() + " , " + m_PADRegion.RegionHeight.ToString() + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " PAD HEIGHT OVER Ratio= " + OHeightRatio.ToString() + " , " + bmpextractBiggestBlob11.Height.ToString() + " , " + m_PADRegion.RegionHeight.ToString() + Environment.NewLine;
                    descstriing = "无胶";
                    reason = ReasonEnum.NG;
                    //if (Math.Abs(bmpinput.Width - iwidthtmp) <= 10 || Math.Abs(bmpinput.Height - iheighttmp) <= 10)
                    //{
                    //    descstriing = "无芯片";
                    //    if (INI.CHIP_forceALIGNRUN_pass)
                    //        reason = ReasonEnum.PASS;
                    //}

                    bmpoutput.Dispose();
                    bmpoutput = new Bitmap(bmpinput);
                    jzToolsClass.DrawRect(bmpoutput, rect1pattrem, new Pen(Color.Red, 5));
                    if (INI.CHIP_NG_SHOW)
                    {
                        bmpoutput.Dispose();
                        bmpoutput = new Bitmap(bmpgray11);
                    }
                }


                bmphistogramEqualization11.Dispose();
                bmpfloodfill.Dispose();
                //bmpgray11.Dispose();
                bmpextractBiggestBlob11.Dispose();
            }

            #endregion


            #region 先判断PAD的尺寸是否正确 和 正确定位到

            PADRegionClass PADTempRegion = new PADRegionClass();

            if (isgood)
            {
                PADRegionFind(bmpInput, PADGrayThreshold, false, out PADTempRegion);

                if (!IsInRangeRatio(PADTempRegion.RegionArea, m_PADRegion.RegionArea, OAreaRatio))
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " PAD AREA OVER Ratio= " + OAreaRatio.ToString() + " , " + PADTempRegion.RegionArea.ToString() + " , " + m_PADRegion.RegionArea.ToString() + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " PAD AREA OVER Ratio= " + OAreaRatio.ToString() + " , " + PADTempRegion.RegionArea.ToString() + " , " + m_PADRegion.RegionArea.ToString() + Environment.NewLine;

                    reason = ReasonEnum.NG;
                    descstriing = "尺寸面积超标";
                }
                else if (!IsInRangeRatio(PADTempRegion.RegionWidth, m_PADRegion.RegionWidth, OWidthRatio))
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + PADTempRegion.RegionWidth.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + PADTempRegion.RegionWidth.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;

                    reason = ReasonEnum.NG;
                    descstriing = "尺寸宽度超标";
                }
                else if (!IsInRangeRatio(PADTempRegion.RegionHeight, m_PADRegion.RegionHeight, OHeightRatio))
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " PAD HEIGHT OVER Ratio= " + OHeightRatio.ToString() + " , " + PADTempRegion.RegionHeight.ToString() + " , " + m_PADRegion.RegionHeight.ToString() + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " PAD HEIGHT OVER Ratio= " + OHeightRatio.ToString() + " , " + PADTempRegion.RegionHeight.ToString() + " , " + m_PADRegion.RegionHeight.ToString() + Environment.NewLine;

                    reason = ReasonEnum.NG;
                    descstriing = "尺寸高度超标";
                }
            }

            #endregion

            //if (RelateAnalyzeString == "A00-02-0002" && m_IsSaveTemp)
            //{
            //    bmpInput.Save("D:\\testtest\\inginput4.png", System.Drawing.Imaging.ImageFormat.Png);
            //    //imgpattern.Save("D:\\testtest\\imgpattern.png", eImageFormat.eImageFormat_PNG);
            //    //imginput.Save("D:\\testtest\\imginput.png", eImageFormat.eImageFormat_PNG);
            //    //bmpMask.Save("D:\\testtest\\mask.png", ImageFormat.Png);
            //}

            //if (RelateAnalyzeString == "A00-02-0004")
            //{
            //    bmpInput.Save("D:\\testtest\\" + RelateAnalyzeString + "bmpInput" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
            //}

            if (!isgood)
            {
                if (m_IsSaveTemp)
                {
                    bmpPadFindOutput.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "PadFind" + (false ? "_Train" : "_Run") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);

                    //bmp.Save("D:\\testtest\\" + RelateAnalyzeString + "PadFind" + (eIsTrain ? "_Train" : "_Run") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                }
            }

            #region 判断PAD 区域里面的blob

            if (isgood)
            {
                PADRegionFindBlob(bmpInput, PADBlobGrayThreshold, PADTempRegion, ref bmpoutput);

                ////mil 計算

                //m_BadArea = m_BadArea / Resolution_Mil / Resolution_Mil;
                //m_BadWidth = m_BadWidth / Resolution_Mil;
                //m_BadHeight = m_BadHeight / Resolution_Mil;

                if (m_BadArea > CheckDArea)
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " PAD AREA OVER area= " + " , " + m_BadArea.ToString() + " , " + CheckDArea.ToString() + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " PAD AREA OVER area= " + " , " + m_BadArea.ToString() + " , " + CheckDArea.ToString() + Environment.NewLine;

                    reason = ReasonEnum.NG;
                    descstriing = "晶片表面溢胶";
                }
                else if (m_BadWidth > CheckDWidth)
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " PAD WIDTH OVER WIDTH= " + " , " + m_BadWidth.ToString() + " , " + CheckDWidth.ToString() + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " PAD WIDTH OVER WIDTH= " + " , " + m_BadWidth.ToString() + " , " + CheckDWidth.ToString() + Environment.NewLine;

                    reason = ReasonEnum.NG;
                    descstriing = "晶片表面溢胶";
                }
                else if (m_BadHeight > CheckDHeight)
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " PAD HEIGHT OVER HEIGHT= " + " , " + m_BadHeight.ToString() + " , " + CheckDHeight.ToString() + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " PAD HEIGHT OVER HEIGHT= " + " , " + m_BadHeight.ToString() + " , " + CheckDHeight.ToString() + Environment.NewLine;

                    reason = ReasonEnum.NG;
                    descstriing = "晶片表面溢胶";
                }


                //pixel 計算

                //if (m_BadArea > CheckDArea)
                //{
                //    isgood = false;
                //    processstring += "Error in " + RelateAnalyzeString + " PAD AREA OVER area= " + " , " + m_BadArea.ToString() + " , " + CheckDArea.ToString() + Environment.NewLine;
                //    errorstring += RelateAnalyzeString + " PAD AREA OVER area= " + " , " + m_BadArea.ToString() + " , " + CheckDArea.ToString() + Environment.NewLine;

                //    reason = ReasonEnum.NG;
                //}
                //else if (m_BadWidth > CheckDWidth)
                //{
                //    isgood = false;
                //    processstring += "Error in " + RelateAnalyzeString + " PAD WIDTH OVER WIDTH= " + " , " + m_BadWidth.ToString() + " , " + CheckDWidth.ToString() + Environment.NewLine;
                //    errorstring += RelateAnalyzeString + " PAD WIDTH OVER WIDTH= " + " , " + m_BadWidth.ToString() + " , " + CheckDWidth.ToString() + Environment.NewLine;

                //    reason = ReasonEnum.NG;
                //}
                //else if (m_BadHeight > CheckDHeight)
                //{
                //    isgood = false;
                //    processstring += "Error in " + RelateAnalyzeString + " PAD HEIGHT OVER HEIGHT= " + " , " + m_BadHeight.ToString() + " , " + CheckDHeight.ToString() + Environment.NewLine;
                //    errorstring += RelateAnalyzeString + " PAD HEIGHT OVER HEIGHT= " + " , " + m_BadHeight.ToString() + " , " + CheckDHeight.ToString() + Environment.NewLine;

                //    reason = ReasonEnum.NG;
                //}

            }

            #endregion

            //胶水的宽度

            #region 胶水宽度判断

            glues = null;
            if (isgood && GlueCheck)
            {
                
                glues = new GlueRegionClass[(int)BorderTypeEnum.COUNT];
                PointF[] pts = null;
                PointF[] ptsIN = null;

                imginput = new Bitmap(bmpinput);
                imgmask = new Bitmap(bmpinput.Width, bmpinput.Height);

                Rectangle rect1 = new Rectangle(PADTempRegion.RegionForEdgeRect.X,
                                                                       PADTempRegion.RegionForEdgeRect.Y,
                                                                       PADTempRegion.RegionForEdgeRect.Width,
                                                                       PADTempRegion.RegionForEdgeRect.Height);
                rect1.Inflate(2, 2);

                

                Rectangle rect = new Rectangle(PADTempRegion.RegionForEdgeRect.X,
                                                                       PADTempRegion.RegionForEdgeRect.Y,
                                                                       PADTempRegion.RegionForEdgeRect.Width,
                                                                       PADTempRegion.RegionForEdgeRect.Height);


                Bitmap bitmap = new Bitmap(bmpInput);
                Bitmap bitmap0 = new Bitmap(bmpInput);
                if (m_IsSaveTemp)
                {
                    bitmap0.Save(Universal.CalTestPath + "\\" + _CalPageIndex() + RelateAnalyzeString + "bmpTrainCollect" + ".png",
                       System.Drawing.Imaging.ImageFormat.Png);


                    bitmap0.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bitmap0" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                }
                switch (PADChipSizeMode)
                {
                    case PADChipSize.CHIP_V1:
                        bitmap = new Bitmap(_getV1bmpInput(rect1));
                        break;
                    
                    case PADChipSize.CHIP_V2:
                        bitmap = new Bitmap(_getV2_2bmpInput(rect1));
                        break;
                    case PADChipSize.CHIP_V3:
                        bitmap = new Bitmap(_getV3bmpInput(rect1));
                        break;
                    case PADChipSize.CHIP_V6:
                        bitmap = new Bitmap(_getV6bmpInput(rect1));
                        break;
                    case PADChipSize.CHIP_V8:
                        bitmap = new Bitmap(_getV8bmpInput(rect1));
                        break;
                    case PADChipSize.CHIP_V5:
                        break;
                    case PADChipSize.CHIP_NORMAL:
                    default:
                        bitmap = new Bitmap(_getG1bmpInput(rect1));
                        break;
                }

                //这里是寻找点的起点位置
                //rect.Inflate(-5, -5);
                rect.Inflate(-(int)(CalExtendX * Resolution_Mil), -(int)(CalExtendY * Resolution_Mil));
                
                //if (m_IsSaveTemp)
                //{
                //    bitmap.Save("D:\\testtest\\" + RelateAnalyzeString + "imginputorg" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                //}

                Rectangle rectin = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                //rectin.Inflate(5, 5);
                int minin = (int)(GlueMin * Resolution_Mil);
                rectin.Inflate(minin, minin);
                //g.DrawRectangle(new Pen(Color.Blue), rectin);

                Rectangle rectout = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                //rectout.Inflate(5, 5);
                int minout = (int)(GlueMax * Resolution_Mil);
                rectout.Inflate(minout, minout);
                //g.DrawRectangle(new Pen(Color.Yellow), rectout);

                Graphics gx = Graphics.FromImage(bitmap);
                gx.DrawRectangle(new Pen(Color.Yellow), rectout);
                gx.Dispose();
                //

                //if (m_IsSaveTemp)
                //{
                //    bitmap.Save("D:\\testtest\\" + RelateAnalyzeString + "imginputrectout" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                //}

                int i = 0;
                int j = 0;
                while (i < (int)BorderTypeEnum.COUNT)
                {

                    switch (PADChipSizeMode)
                    {

                        case PADChipSize.CHIP_V5:

                            Rectangle recttempcorner = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                            bitmap0 = new Bitmap(bmpInput);

                            switch ((BorderTypeEnum)i)
                            {
                                case BorderTypeEnum.LEFT:
                                    recttempcorner = new Rectangle(0, rect.Y, rect.X, rect.Height);
                                    break;
                                case BorderTypeEnum.TOP:
                                    recttempcorner = new Rectangle(rect.X, 0, rect.Width, rect.Y);
                                    break;
                                case BorderTypeEnum.RIGHT:
                                    recttempcorner = new Rectangle(rect.Right, rect.Y, bitmap0.Width - rect.Right, rect.Height);
                                    break;
                                case BorderTypeEnum.BOTTOM:
                                    recttempcorner = new Rectangle(rect.X, rect.Bottom, rect.Width, bitmap0.Height - rect.Bottom);
                                    break;
                            }

                            Bitmap bitmap1 = (Bitmap)bitmap0.Clone(recttempcorner,
                                System.Drawing.Imaging.PixelFormat.Format32bppArgb);


                            bitmap = _getV5bmpInput(bitmap1, (BorderTypeEnum)i);
                            _get_border_pointf_v5(bitmap, recttempcorner,(BorderTypeEnum)i, out glues[i]);


                            break;

                        case PADChipSize.CHIP_V1:
                            _get_border_pointfEx01(bitmap, PADTempRegion.bmpThreshold, rect, (BorderTypeEnum)i, out glues[i]);
                            //_get_border_pointf(bitmap, rect, (BorderTypeEnum)i, out glues[i]);
                            break;
                        case PADChipSize.CHIP_V3:
                            _get_border_pointf_v3_1(bitmap, PADTempRegion.bmpThreshold, rect, (BorderTypeEnum)i, out glues[i]);
                            break;
                        case PADChipSize.CHIP_V6:

                            if (m_IsSaveTemp)
                            {
                                PADTempRegion.bmpThreshold.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpThreshold" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                                //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                            }

                            _get_border_pointf_v6_1(bitmap, PADTempRegion.bmpThreshold, rect, (BorderTypeEnum)i, out glues[i]);
                            break;
                        case PADChipSize.CHIP_V8:

                            if (m_IsSaveTemp)
                            {
                                PADTempRegion.bmpThreshold.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpThreshold" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                                //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                            }

                            if(INI.chipUseAI)
                                _get_border_pointf_v8_1(bitmap, PADTempRegion.bmpThreshold, rect, (BorderTypeEnum)i, out glues[i]);
                            else
                                _get_border_pointf_v6_1(bitmap, PADTempRegion.bmpThreshold, rect, (BorderTypeEnum)i, out glues[i]);

                            break;
                        case PADChipSize.CHIP_V2:
                            //_get_border_pointf_v2(bitmap, rect, (BorderTypeEnum)i, out glues[i]);
                            //break;
                        case PADChipSize.CHIP_NORMAL:
                        default:

                            SetDilatation3x3(ref PADTempRegion.bmpThreshold, 2);
                            SetErosion3x3(ref PADTempRegion.bmpThreshold, 2);

                            //if (m_IsSaveTemp)
                            //{
                            //    PADTempRegion.bmpThreshold.Save("D:\\testtest\\" + RelateAnalyzeString + "bitmap1" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                            //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                            //}

                            _get_border_pointf_v3(bitmap, PADTempRegion.bmpThreshold, rect, (BorderTypeEnum)i, out glues[i]);
                            break;
                    }
                    i++;
                }

                Bitmap bmpglueout = new Bitmap(bmpinput);
                Graphics g = Graphics.FromImage(bmpglueout);
                bool m_ischeckgluepass = true;
                string ngstr = string.Empty;
                string measureStr = string.Empty;

                double GlueTmpMax = GlueMax * Resolution_Mil;
                double GlueTmpMin = GlueMin * Resolution_Mil;


                if (INI.CHIP_CAL_MODE == 2)
                {
                    ////上下
                    //double tbmin = (glues[(int)BorderTypeEnum.TOP].LengthMin + glues[(int)BorderTypeEnum.BOTTOM].LengthMax) / 2;
                    //double tbmax = (glues[(int)BorderTypeEnum.TOP].LengthMax + glues[(int)BorderTypeEnum.BOTTOM].LengthMin) / 2;
                    //glues[(int)BorderTypeEnum.TOP].LengthMin = Math.Min(tbmin, tbmax);
                    //glues[(int)BorderTypeEnum.BOTTOM].LengthMin = Math.Min(tbmin, tbmax);
                    //glues[(int)BorderTypeEnum.TOP].LengthMax = Math.Max(tbmin, tbmax);
                    //glues[(int)BorderTypeEnum.BOTTOM].LengthMax = Math.Max(tbmin, tbmax);
                    ////左右
                    //double lrmin = (glues[(int)BorderTypeEnum.LEFT].LengthMin + glues[(int)BorderTypeEnum.RIGHT].LengthMax) / 2;
                    //double lrmax = (glues[(int)BorderTypeEnum.LEFT].LengthMax + glues[(int)BorderTypeEnum.RIGHT].LengthMin) / 2;
                    //glues[(int)BorderTypeEnum.LEFT].LengthMin = Math.Min(lrmin, lrmax);
                    //glues[(int)BorderTypeEnum.RIGHT].LengthMin = Math.Min(lrmin, lrmax);
                    //glues[(int)BorderTypeEnum.LEFT].LengthMax = Math.Max(lrmin, lrmax);
                    //glues[(int)BorderTypeEnum.RIGHT].LengthMax = Math.Max(lrmin, lrmax);

                    //上下
                    double tbmin = (glues[(int)BorderTypeEnum.TOP].LengthMin + glues[(int)BorderTypeEnum.BOTTOM].LengthMin) / 2;
                    double tbmax = (glues[(int)BorderTypeEnum.TOP].LengthMax + glues[(int)BorderTypeEnum.BOTTOM].LengthMax) / 2;
                    glues[(int)BorderTypeEnum.TOP].LengthMin = glues[(int)BorderTypeEnum.TOP].LengthMin +
                        (tbmin - glues[(int)BorderTypeEnum.TOP].LengthMin) * 0.75;// + GetRandom(-0.01, 0.01);
                    glues[(int)BorderTypeEnum.BOTTOM].LengthMin = glues[(int)BorderTypeEnum.BOTTOM].LengthMin +
                        (tbmin - glues[(int)BorderTypeEnum.BOTTOM].LengthMin) * 0.85;// + GetRandom(-0.01, 0.01);
                    glues[(int)BorderTypeEnum.TOP].LengthMax = glues[(int)BorderTypeEnum.TOP].LengthMax +
                        (tbmax - glues[(int)BorderTypeEnum.TOP].LengthMax) * 0.85;// + GetRandom(-0.01, 0.01);
                    glues[(int)BorderTypeEnum.BOTTOM].LengthMax = glues[(int)BorderTypeEnum.BOTTOM].LengthMax +
                        (tbmax - glues[(int)BorderTypeEnum.BOTTOM].LengthMax) * 0.75;// + GetRandom(-0.01, 0.01);
                    //左右
                    double lrmin = (glues[(int)BorderTypeEnum.LEFT].LengthMin + glues[(int)BorderTypeEnum.RIGHT].LengthMin) / 2;
                    double lrmax = (glues[(int)BorderTypeEnum.LEFT].LengthMax + glues[(int)BorderTypeEnum.RIGHT].LengthMax) / 2;
                    glues[(int)BorderTypeEnum.LEFT].LengthMin = glues[(int)BorderTypeEnum.LEFT].LengthMin +
                        (lrmin - glues[(int)BorderTypeEnum.LEFT].LengthMin) * 0.83;// + GetRandom(-0.01, 0.01);
                    glues[(int)BorderTypeEnum.RIGHT].LengthMin = glues[(int)BorderTypeEnum.RIGHT].LengthMin +
                        (lrmin - glues[(int)BorderTypeEnum.RIGHT].LengthMin) * 0.73;// + GetRandom(-0.01, 0.01);
                    glues[(int)BorderTypeEnum.LEFT].LengthMax = glues[(int)BorderTypeEnum.LEFT].LengthMax +
                        (lrmax - glues[(int)BorderTypeEnum.LEFT].LengthMax) * 0.73;// + GetRandom(-0.01, 0.01);
                    glues[(int)BorderTypeEnum.RIGHT].LengthMax = glues[(int)BorderTypeEnum.RIGHT].LengthMax +
                        (lrmax - glues[(int)BorderTypeEnum.RIGHT].LengthMax) * 0.83;// + GetRandom(-0.01, 0.01);
                }

                

                i = 0;
                while (i < (int)BorderTypeEnum.COUNT)
                {

                    switch ((BorderTypeEnum)i)
                    {
                        case BorderTypeEnum.LEFT:
                            GlueTmpMax = GlueMaxLeft / INI.MAINSD_PAD_MIL_RESOLUTION;
                            GlueTmpMin = GlueMinLeft / INI.MAINSD_PAD_MIL_RESOLUTION;
                            break;
                        case BorderTypeEnum.TOP:
                            GlueTmpMax = GlueMaxTop / INI.MAINSD_PAD_MIL_RESOLUTION;
                            GlueTmpMin = GlueMinTop / INI.MAINSD_PAD_MIL_RESOLUTION;
                            break;
                        case BorderTypeEnum.RIGHT:
                            GlueTmpMax = GlueMaxRight / INI.MAINSD_PAD_MIL_RESOLUTION;
                            GlueTmpMin = GlueMinRight / INI.MAINSD_PAD_MIL_RESOLUTION;
                            break;
                        case BorderTypeEnum.BOTTOM:
                            GlueTmpMax = GlueMaxBottom / INI.MAINSD_PAD_MIL_RESOLUTION;
                            GlueTmpMin = GlueMinBottom / INI.MAINSD_PAD_MIL_RESOLUTION;
                            break;
                    }

                    double min = glues[i].LengthMin * INI.MAINSD_PAD_MIL_RESOLUTION;
                    double max = glues[i].LengthMax * INI.MAINSD_PAD_MIL_RESOLUTION;

                    //ngstr += ((BorderTypeEnum)i).ToString() + "_NG,";

                    switch ((BorderTypeEnum)i)
                    {
                        case BorderTypeEnum.LEFT:
                            measureStr += "左";// + Environment.NewLine;
                            break;
                        case BorderTypeEnum.RIGHT:
                            measureStr += "右";// + Environment.NewLine;
                            break;
                        case BorderTypeEnum.TOP:
                            measureStr += "上";// + Environment.NewLine;
                            break;
                        case BorderTypeEnum.BOTTOM:
                            measureStr += "下";// + Environment.NewLine;
                            break;
                    }

                    measureStr += "[min:" + min.ToString("0.000000") + " mm]";
                    measureStr += "[max:" + max.ToString("0.000000") + " mm]" + Environment.NewLine;

                    if (glues[i].LengthMax > (GlueTmpMax) || glues[i].LengthMin < (GlueTmpMin))
                    {
                        m_ischeckgluepass = false;

                        switch ((BorderTypeEnum)i)
                        {
                            case BorderTypeEnum.LEFT:
                                ngstr += "左";
                                break;
                            case BorderTypeEnum.RIGHT:
                                ngstr += "右";
                                break;
                            case BorderTypeEnum.TOP:
                                ngstr += "上";
                                break;
                            case BorderTypeEnum.BOTTOM:
                                ngstr += "下";
                                break;
                        }

                        ngstr += "[min:" + min.ToString("0.000000") + " mm]";
                        ngstr += "[max:" + max.ToString("0.000000") + " mm]";
                    }

                    i++;
                }

                //填写数据的区域
                RectangleF _rectF = new RectangleF(0, 0, 550, 150);
                g.FillRectangle(Brushes.Black, _rectF);

                //画图 及 显示比对图
                i = 0;

                while (i < (int)BorderTypeEnum.COUNT)
                {

                    switch ((BorderTypeEnum)i)
                    {
                        case BorderTypeEnum.LEFT:
                            GlueTmpMax = GlueMaxLeft / INI.MAINSD_PAD_MIL_RESOLUTION;
                            GlueTmpMin = GlueMinLeft / INI.MAINSD_PAD_MIL_RESOLUTION;
                            break;
                        case BorderTypeEnum.TOP:
                            GlueTmpMax = GlueMaxTop / INI.MAINSD_PAD_MIL_RESOLUTION;
                            GlueTmpMin = GlueMinTop / INI.MAINSD_PAD_MIL_RESOLUTION;
                            break;
                        case BorderTypeEnum.RIGHT:
                            GlueTmpMax = GlueMaxRight / INI.MAINSD_PAD_MIL_RESOLUTION;
                            GlueTmpMin = GlueMinRight / INI.MAINSD_PAD_MIL_RESOLUTION;
                            break;
                        case BorderTypeEnum.BOTTOM:
                            GlueTmpMax = GlueMaxBottom / INI.MAINSD_PAD_MIL_RESOLUTION;
                            GlueTmpMin = GlueMinBottom / INI.MAINSD_PAD_MIL_RESOLUTION;
                            break;
                    }

                    pts = glues[i].GetPointF();
                    ptsIN = glues[i].GetPointFIN();

                    if (m_ischeckgluepass)
                    {
                        g.DrawLines(new Pen(Color.Lime, 5), pts);
                        g.DrawLines(new Pen(Color.Lime, 5), ptsIN);
                        g.DrawString(measureStr, new Font("宋体", 22), Brushes.Lime, 5, 5);
                        //g.DrawString(measureStr, new Font("宋体", 22), Brushes.Lime, _rectF);
                    }
                    else
                    {
                        g.DrawLines(new Pen(Color.Red, 5), pts);
                        g.DrawLines(new Pen(Color.Red, 5), ptsIN);
                        g.DrawString(measureStr, new Font("宋体", 22), Brushes.Red, 5, 5);
                        //g.DrawString(measureStr, new Font("宋体", 22), Brushes.Red, _rectF);
                    }

                    //j = 0;
                    //while (j < pts.Length)
                    //{
                    //    double lengthx = GetPointLength(pts[j], ptsIN[j]);
                    //    if (lengthx >= (GlueTmpMin) && lengthx <= (GlueTmpMax))
                    //    {
                    //        if (INI.CHIP_NG_SHOW)
                    //        {
                    //            g.DrawLine(new Pen(Color.Lime, 2), pts[j], ptsIN[j]);
                    //        }

                    //    }
                    //    else
                    //    {
                    //        g.DrawLine(new Pen(Color.Red, 2), pts[j], ptsIN[j]);
                    //    }
                    //    j++;
                    //}

                    i++;
                }

                //g.DrawRectangle(new Pen(Color.Red), rect);




                ////判断外围的blob 如果有 则NG
                //if (m_ischeckgluepass)//只有宽度合适时 才检查外部blob
                //{
                //    if (_check_region_out_blob(bmpinput, rectout))
                //        m_ischeckgluepass = false;
                //}


                if (m_IsSaveTemp)
                {
                    g.DrawRectangle(new Pen(Color.Red), rect);
                    g.DrawRectangle(new Pen(Color.Blue), rectin);
                    g.DrawRectangle(new Pen(Color.Yellow), rectout);

                    bmpglueout.Save(Universal.CalTestPath + "\\" + _CalPageIndex() + RelateAnalyzeString + "bmpInputout" + ".png",
                        System.Drawing.Imaging.ImageFormat.Png);

                    bmpglueout.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpInputout" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                }
                //bmpglueout.Save("D:\\testtest\\" + RelateAnalyzeString + "bmpInputout" + ".png", System.Drawing.Imaging.ImageFormat.Png);

                bmpoutput = new Bitmap(bmpglueout);
                g.Dispose();
                bmpglueout.Dispose();

                if (!m_ischeckgluepass)
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " Glue OVER " + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " Glue OVER " + Environment.NewLine;

                    reason = ReasonEnum.NG;
                    descstriing = "胶水异常" + ngstr;
                }
                m_RunDataOK = true;
            }

            #endregion

            //imginput.Dispose();
            //imgoutput.Dispose();

            runstatus.SetWorkStatus(bmpPattern,
                                                   bmpInput,
                                                   bmpoutput,
                                                   reason,
                                                   errorstring,
                                                   processstring,
                                                   PassInfo,
                                                   descstriing);
           
            RunStatusCollection.Add(runstatus);
            IsPass = isgood;
            m_DescStr = descstriing;
            if (INI.CHIP_forceALIGNRUN_pass)
            {
                if (descstriing == "无芯片")
                    isgood = true;
            }

            bmpMeasureOutput.Dispose();
            bmpMeasureOutput = new Bitmap(bmpoutput);

            bmpInput.Dispose();

            return isgood;
        }
         * 
         */
        #endregion

        private int blobFindPADRegion(Bitmap ebmpInput, int eThresholdValue, out double owidth, out double oheight, out double oarea)
        {
            owidth = 10;
            oheight = 10;
            oarea = 10;

            int irange = eThresholdValue;
            Bitmap bmp = new Bitmap(ebmpInput);
            JetGrayImg grayimage = new JetGrayImg(bmp);
            JetImgproc.Threshold(grayimage, irange, grayimage);

            //grayimage.ToBitmap().Save("threshold.bmp");

            JetBlob jetBlob = new JetBlob();
            jetBlob.Labeling(grayimage, JConnexity.Connexity4, JBlobLayer.WhiteLayer);
            int icount = jetBlob.BlobCount;

            Graphics g = Graphics.FromImage(bmp);

            for (int i = 0; i < icount; i++)
            {
                int iArea = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.Area);
                if (iArea > 5000)
                {
                    JRotatedRectangleF jetrect = JetBlobFeature.ComputeMinRectangle(jetBlob, i);
                    Point ptCenter = new Point((int)jetrect.fCX, (int)jetrect.fCY);

                    double iWidth = jetrect.fWidth;
                    double iHeight = jetrect.fHeight;
                    if (jetrect.fWidth < jetrect.fHeight)
                    {
                        iWidth = jetrect.fHeight;
                        iHeight = jetrect.fWidth;
                        jetrect.fAngle += 90;
                    }

                    RectangleF myRectF = SimpleRectF(ptCenter, (float)iWidth / 2, (float)iHeight / 2);
                    //转换矩形的四个角
                    PointF[] myPts = RectFToPointF(myRectF, -jetrect.fAngle);

                    Pen p = new Pen(Color.Lime, 2);
                    //p.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;

                    Pen pBottom = new Pen(Color.Red, 2);
                    pBottom.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                    g.DrawLine(p, myPts[0], myPts[1]);
                    g.DrawLine(p, myPts[0], myPts[2]);
                    g.DrawLine(p, myPts[1], myPts[3]);
                    g.DrawLine(pBottom, myPts[2], myPts[3]);


                }

            }
            g.Dispose();
            //bmp.Save(Application.StartupPath + "\\result.bmp");

            if (RelateAnalyzeString == "A00-02-0002")
            {
                bmp.Save("D:\\testtest\\inginput3.png", System.Drawing.Imaging.ImageFormat.Png);
                //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
            }
            //if (m_IsSaveTemp)
            //{
            //    bmp.Save("D:\\testtest\\" + RelateAnalyzeString + "PadFind" + (eIsTrain ? "_Train" : "_Run") + ".png", System.Drawing.Imaging.ImageFormat.Png);
            //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
            //}

            return 0;
        }
        private int PADRegionFind(Bitmap ebmpInput, int eThresholdValue, bool eIsTrain, out PADRegionClass OutPADRegion)
        {
            OutPADRegion = new PADRegionClass();
            OutPADRegion.Reset();

            //这边可加入自动分析阈值功能 简化OP操作

            int irange = eThresholdValue;
            Bitmap bmp = (Bitmap)ebmpInput.Clone();// new Bitmap(ebmpInput);
            Bitmap bmp2 = (Bitmap)ebmpInput.Clone();//new Bitmap(ebmpInput);

            switch (PADThresholdMode)
            {
                case PADThresholdEnum.Ostu_Threshold:

                    AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
                    Bitmap bitmap1 = grayscale.Apply(bmp);

                    AForge.Imaging.Filters.OtsuThreshold otsu = new AForge.Imaging.Filters.OtsuThreshold();
                    Bitmap bitmap2 = otsu.Apply(bitmap1);

                    bmp.Dispose();
                    bmp = new Bitmap(bitmap2);

                    bitmap1.Dispose();
                    bitmap2.Dispose();

                    break;
            }

            //JzFindObjectClass jzFindObjectClass = new JzFindObjectClass();
            //jzFindObjectClass.SetThreshold(bmp2, 0, irange, 0);

            JetGrayImg grayimage = new JetGrayImg(bmp);
            JetImgproc.Threshold(grayimage, irange, grayimage);


            AForge.Imaging.Filters.Grayscale grayscale1 = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            Bitmap bitmap3 = grayscale1.Apply(bmp2);
            AForge.Imaging.Filters.Threshold threshold = new AForge.Imaging.Filters.Threshold(irange);
            bitmap3 = threshold.Apply(bitmap3);
            AForge.Imaging.Filters.Invert invert = new AForge.Imaging.Filters.Invert();
            bitmap3 = invert.Apply(bitmap3);
            AForge.Imaging.Filters.BlobsFiltering blobsFiltering = new AForge.Imaging.Filters.BlobsFiltering();
            blobsFiltering.CoupledSizeFiltering = true;
            blobsFiltering.MinWidth = ebmpInput.Width / 3;
            blobsFiltering.MinHeight = ebmpInput.Height / 3;
            bitmap3 = blobsFiltering.Apply(bitmap3);
            bitmap3 = invert.Apply(bitmap3);
            OutPADRegion.bmpThreshold.Dispose();
            //OutPADRegion.bmpThreshold = new Bitmap(grayimage.ToBitmap());
            OutPADRegion.bmpThreshold = new Bitmap(bitmap3);
            //grayimage.ToBitmap().Save("threshold.bmp");

            int iMAX = -10000000;
            int iMAXIndex = 0;

            JetBlob jetBlob = new JetBlob();
            jetBlob.Labeling(grayimage, JConnexity.Connexity4, JBlobLayer.WhiteLayer);
            int icount = jetBlob.BlobCount;
            //if (m_IsSaveTemp)

            //OutPADRegion.bmpThreshold = new Bitmap(bmp);

            //switch (PADChipSizeMode)
            //{
            //    case PADChipSize.CHIP_NORMAL:

            //        AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            //        Bitmap bitmap1 = grayscale.Apply(bmp);

            //        AForge.Imaging.Filters.Threshold threshold = new AForge.Imaging.Filters.Threshold(irange);
            //        Bitmap bitmap2 = threshold.Apply(bitmap1);

            //        OutPADRegion.bmpThreshold.Dispose();
            //        OutPADRegion.bmpThreshold = new Bitmap(bitmap2);

            //        bitmap1.Dispose();
            //        bitmap2.Dispose();
            //        break;
            //}



            //找到最大面积
            for (int i = 0; i < icount; i++)
            {
                int iArea = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.Area);
                if (iArea > 5000)
                {
                    if (iMAX > iArea)
                        continue;
                    else
                    {
                        iMAX = iArea;
                        iMAXIndex = i;
                    }
                }

            }

            if (icount > 0)
            {

                //画出图形
                Graphics g = Graphics.FromImage(bmp);
                JRotatedRectangleF jetrect = JetBlobFeature.ComputeMinRectangle(jetBlob, iMAXIndex);
                Point ptCenter = new Point((int)jetrect.fCX, (int)jetrect.fCY);

                int itop = JetBlobFeature.ComputeIntegerFeature(jetBlob, iMAXIndex, JBlobIntFeature.TopMost);
                int iLeft = JetBlobFeature.ComputeIntegerFeature(jetBlob, iMAXIndex, JBlobIntFeature.LeftMost);
                int iRight = JetBlobFeature.ComputeIntegerFeature(jetBlob, iMAXIndex, JBlobIntFeature.RightMost);
                int iBottom = JetBlobFeature.ComputeIntegerFeature(jetBlob, iMAXIndex, JBlobIntFeature.BottomMost);

                OutPADRegion.RegionForEdgeRect = new Rectangle(iLeft, itop, iRight - iLeft, iBottom - itop);

                double iWidth = jetrect.fWidth;
                double iHeight = jetrect.fHeight;
                if (jetrect.fWidth < jetrect.fHeight)
                {
                    iWidth = jetrect.fHeight;
                    iHeight = jetrect.fWidth;
                    jetrect.fAngle += 90;
                }

                RectangleF myRectF = SimpleRectF(ptCenter, (float)iWidth / 2, (float)iHeight / 2);

                //原来的外框
                //转换矩形的四个角
                PointF[] _myPointFs = RectFToPointF(myRectF, -jetrect.fAngle);

                Pen p = new Pen(Color.Violet, 2);
                p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                Pen pBottom = new Pen(Color.Violet, 2);
                pBottom.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                g.DrawLine(p, _myPointFs[0], _myPointFs[1]);
                g.DrawLine(p, _myPointFs[1], _myPointFs[2]);
                g.DrawLine(p, _myPointFs[2], _myPointFs[3]);
                g.DrawLine(pBottom, _myPointFs[0], _myPointFs[3]);

                OutPADRegion.SetPointFORG(_myPointFs);

                // mil 計算

                myRectF.Inflate(-(float)(ExtendX * Resolution_Mil), -(float)(ExtendY * Resolution_Mil));

                // pixel 計算

                //myRectF.Inflate(-ExtendX, -ExtendY);

                //转换矩形的四个角
                _myPointFs = RectFToPointF(myRectF, -jetrect.fAngle);

                p = new Pen(Color.Lime, 2);
                p.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

                pBottom = new Pen(Color.Lime, 2);
                pBottom.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

                g.DrawLine(p, _myPointFs[0], _myPointFs[1]);
                g.DrawLine(p, _myPointFs[1], _myPointFs[2]);
                g.DrawLine(p, _myPointFs[2], _myPointFs[3]);
                g.DrawLine(pBottom, _myPointFs[0], _myPointFs[3]);

                OutPADRegion.RegionWidth = Math.Max(iWidth, iHeight);
                OutPADRegion.RegionHeight = Math.Min(iWidth, iHeight);
                OutPADRegion.RegionArea = iMAX;
                OutPADRegion.SetPointF(_myPointFs);

                bmpPadFindOutput = (Bitmap)bmp.Clone();// new Bitmap(bmp);

                g.Dispose();
            }

            //g.Dispose();
            //bmp.Save(Application.StartupPath + "\\result.bmp");

            //if (m_IsSaveTemp && !eIsTrain)
            //{
            //    bmp.Save("D:\\testtest\\" + RelateAnalyzeString + "PadFind" + (eIsTrain ? "_Train" : "_Run") + ".png", System.Drawing.Imaging.ImageFormat.Png);
            //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);

            //    //bmp.Save("D:\\testtest\\" + RelateAnalyzeString + "PadFind" + (eIsTrain ? "_Train" : "_Run") + ".png", System.Drawing.Imaging.ImageFormat.Png);
            //}

            return 0;
        }
        private int PADRegionFind_BlackEdge(Bitmap ebmpInput, int eThresholdValue, bool eIsTrain, out PADRegionClass OutPADRegion)
        {
            OutPADRegion = new PADRegionClass();
            OutPADRegion.Reset();

            //缩放倍数
            int iSized = 5;
            Bitmap bmpinputUse = new Bitmap(ebmpInput, new Size(ebmpInput.Width / iSized, ebmpInput.Height / iSized));
            //Bitmap bmpinputUse = new Bitmap(ebmpInput, Resize(ebmpInput.Size, -iSized));
            Bitmap bmpdrawUse = (Bitmap)ebmpInput.Clone();

            int irange = eThresholdValue;
            Bitmap bmp = (Bitmap)bmpinputUse.Clone();// new Bitmap(ebmpInput);
            //Bitmap bmp2 = (Bitmap)bmpinputUse.Clone();//new Bitmap(ebmpInput);

            JetGrayImg grayimage = new JetGrayImg(bmp);
            JetImgproc.Threshold(grayimage, irange, grayimage);

            int iMAX = -10000000;
            int iMAXIndex = 0;

            JetBlob jetBlob = new JetBlob();
            jetBlob.Labeling(grayimage, JConnexity.Connexity4, JBlobLayer.WhiteLayer);
            int icount = jetBlob.BlobCount;
            List<string> checkListStr = new List<string>();
            //找到最大面积
            for (int i = 0; i < icount; i++)
            {
                int iArea = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.Area);
                if (iArea > 1000)
                {
                    //checkListStr.Add(i.ToString() + "," + iArea.ToString());
                    checkListStr.Add(iArea.ToString() + "," + i.ToString());
                }

            }
            //排序 从大到小排序
            if (checkListStr.Count > 1)
                checkListStr.Sort((item1, item2) =>
                { return int.Parse(item1.Split(',')[0]) > int.Parse(item2.Split(',')[0]) ? -1 : 1; });
            //checkListStr.Sort();
            JRotatedRectangleF jRotatedRectangleF = new JRotatedRectangleF();
            //JRotatedRectangleF jRotatedRectangleF2 = new JRotatedRectangleF();

            if (checkListStr.Count > 0)
            {
                iMAXIndex = int.Parse(checkListStr[0].Split(',')[1]);
                JRotatedRectangleF jetrect0x = JetBlobFeature.ComputeMinRectangle(jetBlob, iMAXIndex);
                jRotatedRectangleF = ResizeWithLocation2(jetrect0x, iSized);
                //jRotatedRectangleF2 = ResizeWithLocation2(jetrect0x, iSized);

                List<PointF> pointFs = new List<PointF>();//收集所有的角点 以便于找到最大的旋转矩形
                pointFs.Clear();
                //画出图形 在原图上画出来
                Graphics g = Graphics.FromImage(bmpdrawUse);
                int penwidth = 5;

                int i = 0;
                while (i < checkListStr.Count)
                {
                    iMAXIndex = int.Parse(checkListStr[i].Split(',')[1]);
                    JRotatedRectangleF jetrect0 = JetBlobFeature.ComputeMinRectangle(jetBlob, iMAXIndex);
                    JRotatedRectangleF jetrect = ResizeWithLocation2(jetrect0, iSized);
                
                    PointF ptCenter = new PointF((float)jetrect.fCX, (float)jetrect.fCY);

                    double iWidth = jetrect.fWidth;
                    double iHeight = jetrect.fHeight;
                    if (jetrect.fWidth < jetrect.fHeight)
                    {
                        iWidth = jetrect.fHeight;
                        iHeight = jetrect.fWidth;
                        jetrect.fAngle += 90;
                    }

                    RectangleF myRectF = SimpleRectF(ptCenter, (float)iWidth / 2, (float)iHeight / 2);

                    //原来的外框
                    //转换矩形的四个角
                    PointF[] _myPointFs = RectFToPointF(myRectF, -jetrect.fAngle);
                    pointFs.Add(_myPointFs[0]);
                    pointFs.Add(_myPointFs[1]);
                    pointFs.Add(_myPointFs[2]);
                    pointFs.Add(_myPointFs[3]);

                    //if(eIsTrain)
                    {
                        Pen p = new Pen(Color.Violet, penwidth);
                        p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                        Pen pBottom = new Pen(Color.Violet, penwidth);
                        pBottom.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                        g.DrawLine(p, _myPointFs[0], _myPointFs[1]);
                        g.DrawLine(p, _myPointFs[1], _myPointFs[2]);
                        g.DrawLine(p, _myPointFs[2], _myPointFs[3]);
                        g.DrawLine(pBottom, _myPointFs[0], _myPointFs[3]);

                        // mil 計算

                        myRectF.Inflate(-(float)(ExtendX * Resolution_Mil), -(float)(ExtendY * Resolution_Mil));

                        // pixel 計算

                        //myRectF.Inflate(-ExtendX, -ExtendY);
                        //内缩的点位
                        //转换矩形的四个角
                        _myPointFs = RectFToPointF(myRectF, -jetrect.fAngle);

                        OutPADRegion.listPointF.Add(_myPointFs);

                        p = new Pen(Color.Lime, penwidth);
                        p.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

                        pBottom = new Pen(Color.Lime, penwidth);
                        pBottom.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

                        g.DrawLine(p, _myPointFs[0], _myPointFs[1]);
                        g.DrawLine(p, _myPointFs[1], _myPointFs[2]);
                        g.DrawLine(p, _myPointFs[2], _myPointFs[3]);
                        g.DrawLine(pBottom, _myPointFs[0], _myPointFs[3]);
                    }

                    i++;
                }

                List<string> checkListStr1 = new List<string>();
                //找到X坐标的最左和最右
                int j = 0;
                foreach (PointF ptf in pointFs)
                {
                    checkListStr1.Add($"{ptf.X.ToString()},{j.ToString()}");
                    j++;
                }
                checkListStr1.Sort((item1, item2) =>
                { return float.Parse(item1.Split(',')[0]) >= float.Parse(item2.Split(',')[0]) ? 1 : -1; });
                //checkListStr1.Sort();
                //float ileftx = float.Parse(checkListStr1[0].Split(',')[0]);
                //float irightx = float.Parse(checkListStr1[checkListStr1.Count - 1].Split(',')[0]);

                PointF ptfleft = pointFs[int.Parse(checkListStr1[0].Split(',')[1])];
                PointF ptfright = pointFs[int.Parse(checkListStr1[checkListStr1.Count - 1].Split(',')[1])];

                checkListStr1.Clear();
                //找到Y坐标的最上和最下
                j = 0;
                foreach (PointF ptf in pointFs)
                {
                    checkListStr1.Add($"{ptf.Y.ToString()},{j.ToString()}");
                    j++;
                }
                checkListStr1.Sort((item1, item2) =>
                { return float.Parse(item1.Split(',')[0]) >= float.Parse(item2.Split(',')[0]) ? 1 : -1; });
                //checkListStr1.Sort();
                //float itopy = float.Parse(checkListStr1[0].Split(',')[0]);
                //float ibottomy = float.Parse(checkListStr1[checkListStr1.Count - 1].Split(',')[0]);

                PointF ptftop = pointFs[int.Parse(checkListStr1[0].Split(',')[1])];
                PointF ptfbottom = pointFs[int.Parse(checkListStr1[checkListStr1.Count - 1].Split(',')[1])];

                double _angle = GetP1P2Angle(ptfleft, ptftop);
                OutPADRegion.Isleftoverright = Math.Abs(_angle) < 45;
                PointF[] _myPointFs1 = _getEdgeConcor(jRotatedRectangleF.fAngle,
                                                                                   ptfleft, ptfright, ptftop, ptfbottom, BlackOffsetX, BlackOffsetY, OutPADRegion.Isleftoverright);

                checkListStr1.Clear();
                j = 0;
                foreach (PointF ptf in _myPointFs1)
                {
                    checkListStr1.Add($"{ptf.X.ToString()},{j.ToString()}");
                    j++;
                }
                checkListStr1.Sort((item1, item2) =>
                { return float.Parse(item1.Split(',')[0]) >= float.Parse(item2.Split(',')[0]) ? 1 : -1; });
                //checkListStr1.Sort();
                //float ileftx = float.Parse(checkListStr1[0].Split(',')[0]);
                //float irightx = float.Parse(checkListStr1[checkListStr1.Count - 1].Split(',')[0]);

                float iLeft = _myPointFs1[int.Parse(checkListStr1[0].Split(',')[1])].X;
                float iRight = _myPointFs1[int.Parse(checkListStr1[checkListStr1.Count - 1].Split(',')[1])].X;

                checkListStr1.Clear();
                //找到Y坐标的最上和最下
                j = 0;
                foreach (PointF ptf in _myPointFs1)
                {
                    checkListStr1.Add($"{ptf.Y.ToString()},{j.ToString()}");
                    j++;
                }
                checkListStr1.Sort((item1, item2) =>
                { return float.Parse(item1.Split(',')[0]) >= float.Parse(item2.Split(',')[0]) ? 1 : -1; });
                //checkListStr1.Sort();
                //float itopy = float.Parse(checkListStr1[0].Split(',')[0]);
                //float ibottomy = float.Parse(checkListStr1[checkListStr1.Count - 1].Split(',')[0]);

                float itop = _myPointFs1[int.Parse(checkListStr1[0].Split(',')[1])].Y;
                float iBottom = _myPointFs1[int.Parse(checkListStr1[checkListStr1.Count - 1].Split(',')[1])].Y;

                OutPADRegion.RegionForEdgeRect = new Rectangle((int)iLeft, (int)itop, (int)(iRight - iLeft), (int)(iBottom - itop));

                //if(eIsTrain)
                {
                    Pen p1 = new Pen(Color.Green, penwidth);
                    p1.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                    Pen pBottom1 = new Pen(Color.Green, penwidth);
                    pBottom1.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                    g.DrawLine(p1, _myPointFs1[0], _myPointFs1[1]);
                    g.DrawLine(p1, _myPointFs1[1], _myPointFs1[2]);
                    g.DrawLine(p1, _myPointFs1[2], _myPointFs1[3]);
                    g.DrawLine(pBottom1, _myPointFs1[0], _myPointFs1[3]);
                }
               
                g.Dispose();

                Bitmap bmpdrawUse2 = (Bitmap)ebmpInput.Clone();
                Graphics g2 = Graphics.FromImage(bmpdrawUse2);
                g2.Clear(Color.Black);
                g2.FillPolygon(Brushes.White, _myPointFs1);
                g2.Dispose();

                bmpPadFindOutput.Dispose();
                bmpPadFindOutput = (Bitmap)bmpdrawUse.Clone();// new Bitmap(bmp);
                OutPADRegion.bmpChipForWetherGlue.Dispose();
                OutPADRegion.bmpChipForWetherGlue = new Bitmap(bmpdrawUse);
                bmpdrawUse.Dispose();

                OutPADRegion.bmpThreshold.Dispose();
                OutPADRegion.bmpThreshold = new Bitmap(bmpdrawUse2);
                bmpdrawUse2.Dispose();

                OutPADRegion.SetPointFORG(_myPointFs1, jRotatedRectangleF.fAngle);
            }
            
            return 0;
        }

        private int PADRegionFindBlob(Bitmap ebmpInput, 
            int eThresholdValue, 
            PADRegionClass eInputPADRegion, 
            ref Bitmap ebmpOutput,
            int iSized=3)
        {
            //首先MASK 区域出来

            m_BadArea = 0;
            m_BadCount = 0;
            m_BadWidth = 0;
            m_BadHeight = 0;

            int sizeblob = iSized;

            Bitmap bmpdraw = (Bitmap)ebmpInput.Clone();// new Bitmap(ebmpInput);

            imginput = new Bitmap(ebmpInput);
            imgmask = new Bitmap(ebmpInput.Width, ebmpInput.Height);
            Graphics gPrepared = Graphics.FromImage(imgmask);
            gPrepared.Clear(Color.White);
            if (eInputPADRegion.listPointF.Count > 0)
            {
                //Parallel.ForEach(eInputPADRegion.listPointF, (item) =>
                //{
                //    gPrepared.FillPolygon(Brushes.Black, item);
                //});
                foreach (PointF[] pts in eInputPADRegion.listPointF)
                {
                    gPrepared.FillPolygon(Brushes.Black, pts);
                }
            }
            else
            {
                gPrepared.FillPolygon(Brushes.Black, eInputPADRegion.RegionPtFCorner);
            }
            gPrepared.Dispose();

            imginput = new Bitmap(imginput, Resize(imginput.Size, -sizeblob));
            imgmask = new Bitmap(imgmask, Resize(imgmask.Size, -sizeblob));

            m_JzFind.GetMaskedImage(imginput, imgmask, Color.White, Color.White, true);

            int irange = eThresholdValue;
            Bitmap bmp = (Bitmap)imginput.Clone();// new Bitmap(imginput);

            switch (PADThresholdMode)
            {
                case PADThresholdEnum.Ostu_Threshold:

                    AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
                    Bitmap bitmap1 = grayscale.Apply(bmp);

                    AForge.Imaging.Filters.OtsuThreshold otsu = new AForge.Imaging.Filters.OtsuThreshold();
                    Bitmap bitmap2 = otsu.Apply(bitmap1);

                    bmp.Dispose();
                    bmp = new Bitmap(bitmap2);

                    bitmap1.Dispose();
                    bitmap2.Dispose();

                    break;
            }

            //Gray
            AForge.Imaging.Filters.Grayscale grayscale1 = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            bmp = grayscale1.Apply(bmp);

            //Threshold
            AForge.Imaging.Filters.Threshold threshold = new AForge.Imaging.Filters.Threshold(eThresholdValue);
            bmp = threshold.Apply(bmp);

            //Invert
            AForge.Imaging.Filters.Invert invert = new AForge.Imaging.Filters.Invert();
            bmp = invert.Apply(bmp);

            //BlobCounter
            AForge.Imaging.BlobCounter blobCounter = new AForge.Imaging.BlobCounter();
            blobCounter.ObjectsOrder = AForge.Imaging.ObjectsOrder.Area;
            blobCounter.ProcessImage(bmp);
            //Rectangle[] rectangles = blobCounter.GetObjectsRectangles();
            AForge.Imaging.Blob[] blobs = blobCounter.GetObjectsInformation();

            bool bNG = false;
            Graphics g = Graphics.FromImage(bmpdraw);
            string msg = "标准长度:" + CheckDWidth.ToString() + ",标准宽度:" + CheckDHeight.ToString() + ",标准面积:" + CheckDArea.ToString() + Environment.NewLine;
            g.DrawString(msg, new Font("宋体", 18), Brushes.Black, 5, 5);

            if (blobs.Length > 0)
            {
                bNG = false;
                RectangleF rectangleF = ResizeWithLocation2(blobs[0].Rectangle, sizeblob);
                m_BadArea = blobs[0].Area * (1 << sizeblob) * (1 << sizeblob) * Resolution_Mil * Resolution_Mil;
                m_BadWidth = rectangleF.Width * Resolution_Mil;
                m_BadHeight = rectangleF.Height * Resolution_Mil;

                //如果最大的方框都NG了 则是NG
                if (m_BadWidth > CheckDWidth)
                {
                    //m_BadWidth = jetrect.fWidth;
                    bNG = true;
                    m_BadCount++;
                }
                else if (m_BadHeight > CheckDHeight)
                {
                    //m_BadHeight = jetrect.fHeight;
                    bNG = true;
                    m_BadCount++;
                }
                else if (m_BadArea > CheckDArea)
                {
                    //m_BadArea = iArea;
                    bNG = true;
                    m_BadCount++;
                }
                if (bNG)
                {
                    RectangleF[] fs = new RectangleF[1];
                    fs[0] = rectangleF;
                    g.DrawRectangles(new Pen(Color.Red, 3), fs);
                    //g.DrawRectangle(new Pen(Color.Red, 3), blobs[0].Rectangle);
                }
            }

            g.Dispose();

            if (m_IsSaveTemp && m_BadCount > 0)
            {
                bmp.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "_blob03" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }

            ebmpOutput = (Bitmap)bmpdraw.Clone();// new Bitmap(bmpdraw);
            //bmpPadBolbOutput = (Bitmap)bmpdraw.Clone();//  new Bitmap(bmpdraw);

            imginput.Dispose();
            imgmask.Dispose();
            bmp.Dispose();
            bmpdraw.Dispose();

            return 0;
        }
        private int PADRegionFindBlob_Glacode(Bitmap ebmpInput,
            int eThresholdValue,
            PADRegionClass eInputPADRegion,
            ref Bitmap ebmpOutput,
            int iSized = 3, bool eWhite = false,List<RectangleF> rectangleFs=null)
        {
            //首先MASK 区域出来

            m_BadArea = 0;
            m_BadCount = 0;
            m_BadWidth = 0;
            m_BadHeight = 0;

            int sizeblob = iSized;

            Bitmap bmpdraw = (Bitmap)ebmpInput.Clone();// new Bitmap(ebmpInput);

            imginput = new Bitmap(ebmpInput);
            imgmask = new Bitmap(ebmpInput.Width, ebmpInput.Height);
            Graphics gPrepared = Graphics.FromImage(imgmask);
            gPrepared.Clear(Color.White);
            if (eInputPADRegion.listPointF.Count > 0)
            {
                //Parallel.ForEach(eInputPADRegion.listPointF, (item) =>
                //{
                //    gPrepared.FillPolygon(Brushes.Black, item);
                //});
                foreach (PointF[] pts in eInputPADRegion.listPointF)
                {
                    gPrepared.FillPolygon(Brushes.Black, pts);
                }
            }
            else
            {
                gPrepared.FillPolygon(Brushes.Black, eInputPADRegion.RegionPtFCorner);
            }

            if (rectangleFs != null)
            {
                if (rectangleFs.Count > 0)
                {
                    gPrepared.FillRectangles(Brushes.White, rectangleFs.ToArray());
                }
            }

            gPrepared.Dispose();

            imginput = new Bitmap(imginput, new Size(imginput.Width / sizeblob, imginput.Height / sizeblob));
            imgmask = new Bitmap(imgmask, new Size(imgmask.Width / sizeblob, imgmask.Height / sizeblob));

            //imginput = new Bitmap(imginput, Resize(imginput.Size, -sizeblob));
            //imgmask = new Bitmap(imgmask, Resize(imgmask.Size, -sizeblob));

            m_JzFind.GetMaskedImage(imginput, imgmask, Color.White, Color.Black, true);
            //m_JzFind.GetMaskedImage(imginput, imgmask, Color.White, Color.White, true);

            int irange = eThresholdValue;
            Bitmap bmp = (Bitmap)imginput.Clone();// new Bitmap(imginput);

            switch (PADThresholdMode)
            {
                case PADThresholdEnum.Ostu_Threshold:

                    AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
                    Bitmap bitmap1 = grayscale.Apply(bmp);

                    AForge.Imaging.Filters.OtsuThreshold otsu = new AForge.Imaging.Filters.OtsuThreshold();
                    Bitmap bitmap2 = otsu.Apply(bitmap1);

                    bmp.Dispose();
                    bmp = new Bitmap(bitmap2);

                    bitmap1.Dispose();
                    bitmap2.Dispose();

                    break;
            }

            //Gray
            AForge.Imaging.Filters.Grayscale grayscale1 = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            bmp = grayscale1.Apply(bmp);

            //Threshold
            AForge.Imaging.Filters.Threshold threshold = new AForge.Imaging.Filters.Threshold(eThresholdValue);
            bmp = threshold.Apply(bmp);

            if (!eWhite)
            {
                //Invert
                AForge.Imaging.Filters.Invert invert = new AForge.Imaging.Filters.Invert();
                bmp = invert.Apply(bmp);
            }

            //BlobCounter
            AForge.Imaging.BlobCounter blobCounter = new AForge.Imaging.BlobCounter();
            blobCounter.ObjectsOrder = AForge.Imaging.ObjectsOrder.Area;
            blobCounter.ProcessImage(bmp);
            //Rectangle[] rectangles = blobCounter.GetObjectsRectangles();
            AForge.Imaging.Blob[] blobs = blobCounter.GetObjectsInformation();

            bool bNG = false;
            Graphics g = Graphics.FromImage(bmpdraw);
            string msg = "标准长度:" + CheckDWidth.ToString() + ",标准宽度:" + CheckDHeight.ToString() + ",标准面积:" + CheckDArea.ToString() + Environment.NewLine;
            g.DrawString(msg, new Font("宋体", 22), Brushes.Lime, 5, 5);

            if (blobs.Length > 0)
            {
                bNG = false;
                RectangleF rectangleF = ResizeWithLocation3(blobs[0].Rectangle, sizeblob);
                m_BadArea = blobs[0].Area * sizeblob * sizeblob * Resolution_Mil * Resolution_Mil;
                m_BadWidth = rectangleF.Width * Resolution_Mil;
                m_BadHeight = rectangleF.Height * Resolution_Mil;

                //如果最大的方框都NG了 则是NG
                if (m_BadWidth > CheckDWidth)
                {
                    //m_BadWidth = jetrect.fWidth;
                    bNG = true;
                    m_BadCount++;
                }
                else if (m_BadHeight > CheckDHeight)
                {
                    //m_BadHeight = jetrect.fHeight;
                    bNG = true;
                    m_BadCount++;
                }
                else if (m_BadArea > CheckDArea)
                {
                    //m_BadArea = iArea;
                    bNG = true;
                    m_BadCount++;
                }
                if (bNG)
                {
                    RectangleF[] fs = new RectangleF[1];
                    fs[0] = rectangleF;
                    g.DrawRectangles(new Pen(Color.Red, 7), fs);
                    //g.DrawRectangle(new Pen(Color.Red, 3), blobs[0].Rectangle);
                }

                List<RectangleF> list = new List<RectangleF>();
                foreach (AForge.Imaging.Blob blobx in blobs)
                {
                    rectangleF = ResizeWithLocation3(blobx.Rectangle, sizeblob);
                    double _areax = blobs[0].Area * sizeblob * sizeblob * Resolution_Mil * Resolution_Mil;
                    double _widthx = rectangleF.Width * Resolution_Mil;
                    double _heightx = rectangleF.Height * Resolution_Mil;
                    bool _bOK = true;
                    //如果最大的方框都NG了 则是NG
                    if (m_BadWidth > CheckDWidth)
                    {
                        _bOK = false;
                    }
                    else if (m_BadHeight > CheckDHeight)
                    {
                        _bOK = false;
                    }
                    else if (m_BadArea > CheckDArea)
                    {
                        _bOK = false;
                    }
                    if (!_bOK)
                    {
                        list.Add(blobx.Rectangle);
                    }
                }
                if (list.Count > 0)
                    g.DrawRectangles(new Pen(Color.Red, 3), list.ToArray());
            }

            g.Dispose();

            if (m_IsSaveTemp && m_BadCount > 0)
            {
                bmp.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "_blob03" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }

            ebmpOutput = (Bitmap)bmpdraw.Clone();// new Bitmap(bmpdraw);
            //bmpPadBolbOutput = (Bitmap)bmpdraw.Clone();//  new Bitmap(bmpdraw);

            if (m_IsSaveTemp && m_BadCount > 0)
            {
                bmpdraw.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "_draw" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }

            imginput.Dispose();
            imgmask.Dispose();
            bmp.Dispose();
            bmpdraw.Dispose();

            return 0;
        }

        private PointF[] _getEdgeConcor(double angle, PointF ptfleft, PointF ptfright, PointF ptftop, PointF ptfbottom, double centeroffsetx = 0, double centeroffsety = 0,bool leftoverright=false)
        {
            JRotatedRectangleF jRotatedRectangleF = new JRotatedRectangleF();
            jRotatedRectangleF.fCX = (ptfleft.X + ptfright.X) / 2;
            jRotatedRectangleF.fCY = (ptftop.Y + ptfbottom.Y) / 2;

            double d1 = GetPointLength(ptfleft, ptfbottom) + BlackCalExtendY * Resolution_Mil * 2;
            double d2 = GetPointLength(ptfleft, ptftop) + BlackCalExtendX * Resolution_Mil * 2;

            if (leftoverright)
            {
                if (d1 < d2)
                {
                    jRotatedRectangleF.fWidth = d1;
                    jRotatedRectangleF.fHeight = d2;
                }
                else
                {
                    jRotatedRectangleF.fWidth = d2;
                    jRotatedRectangleF.fHeight = d1;
                }
                //jRotatedRectangleF.fWidth = GetPointLength(ptfleft, ptfbottom) + BlackCalExtendY * Resolution_Mil * 2;
                //jRotatedRectangleF.fHeight = GetPointLength(ptfleft, ptftop) + BlackCalExtendX * Resolution_Mil * 2;
            }
            else
            {
                if (d1 > d2)
                {
                    jRotatedRectangleF.fWidth = d1;
                    jRotatedRectangleF.fHeight = d2;
                }
                else
                {
                    jRotatedRectangleF.fWidth = d2;
                    jRotatedRectangleF.fHeight = d1;
                }
                //jRotatedRectangleF.fWidth = GetPointLength(ptfleft, ptfbottom) + BlackCalExtendX * Resolution_Mil * 2;
                //jRotatedRectangleF.fHeight = GetPointLength(ptfleft, ptftop) + BlackCalExtendY * Resolution_Mil * 2;
            }
           
            jRotatedRectangleF.fAngle = angle;

            PointF[] pointFs = _getJRotatedRectangleFConcor(jRotatedRectangleF);
            if (centeroffsetx == 0 && centeroffsety == 0)
                return pointFs;

            JRotatedRectangleF jRotatedRectangleF1 = new JRotatedRectangleF();
            jRotatedRectangleF1.fCX = (ptfleft.X + ptfright.X) / 2;
            jRotatedRectangleF1.fCY = (ptftop.Y + ptfbottom.Y) / 2;
            if (leftoverright)
            {
                jRotatedRectangleF1.fWidth = GetPointLength(ptfleft, ptfbottom) + (BlackCalExtendY + centeroffsety) * Resolution_Mil * 2;
                jRotatedRectangleF1.fHeight = GetPointLength(ptfleft, ptftop) + (BlackCalExtendX + centeroffsetx) * Resolution_Mil * 2;
            }
            else
            {
                jRotatedRectangleF1.fWidth = GetPointLength(ptfleft, ptfbottom) + (BlackCalExtendX + centeroffsetx) * Resolution_Mil * 2;
                jRotatedRectangleF1.fHeight = GetPointLength(ptfleft, ptftop) + (BlackCalExtendY + centeroffsety) * Resolution_Mil * 2;
            }
            
            jRotatedRectangleF1.fAngle = angle;

            PointF[] pointFs1 = _getJRotatedRectangleFConcor(jRotatedRectangleF1);

            List<string> checkListStr1 = new List<string>();
            //找到X坐标的最左和最右
            int j = 0;
            foreach (PointF ptf in pointFs)
            {
                checkListStr1.Add($"{ptf.X.ToString()},{j.ToString()}");
                j++;
            }
            checkListStr1.Sort((item1, item2) =>
            { return float.Parse(item1.Split(',')[0]) > float.Parse(item2.Split(',')[0]) ? 1 : -1; });

            PointF ptfleft1 = pointFs[int.Parse(checkListStr1[0].Split(',')[1])];
            PointF ptfright1 = pointFs[int.Parse(checkListStr1[1].Split(',')[1])];

            checkListStr1.Clear();
            j = 0;
            foreach (PointF ptf in pointFs1)
            {
                checkListStr1.Add($"{ptf.X.ToString()},{j.ToString()}");
                j++;
            }
            checkListStr1.Sort((item1, item2) =>
            { return float.Parse(item1.Split(',')[0]) > float.Parse(item2.Split(',')[0]) ? -1 : 1; });

            PointF ptfleft2 = pointFs1[int.Parse(checkListStr1[0].Split(',')[1])];
            PointF ptfright2 = pointFs1[int.Parse(checkListStr1[1].Split(',')[1])];

            List<PointF> checkList = new List<PointF>();
            checkList.Add(ptfleft1);
            checkList.Add(ptfright1);
            checkList.Add(ptfleft2);
            checkList.Add(ptfright2);

            return checkList.ToArray();

        }

        private PointF[] _getEdgeConcorBAK20240320(double angle, PointF ptfleft, PointF ptfright, PointF ptftop, PointF ptfbottom, double centeroffsetx = 0, double centeroffsety = 0, bool leftoverright = false)
        {
            JRotatedRectangleF jRotatedRectangleF = new JRotatedRectangleF();
            jRotatedRectangleF.fCX = (ptfleft.X + ptfright.X) / 2;
            jRotatedRectangleF.fCY = (ptfleft.Y + ptfright.Y) / 2;
            if (leftoverright)
            {
                jRotatedRectangleF.fWidth = GetPointLength(ptfleft, ptfbottom) + BlackCalExtendY * Resolution_Mil * 2;
                jRotatedRectangleF.fHeight = GetPointLength(ptfleft, ptftop) + BlackCalExtendX * Resolution_Mil * 2;
            }
            else
            {
                jRotatedRectangleF.fWidth = GetPointLength(ptfleft, ptfbottom) + BlackCalExtendX * Resolution_Mil * 2;
                jRotatedRectangleF.fHeight = GetPointLength(ptfleft, ptftop) + BlackCalExtendY * Resolution_Mil * 2;
            }

            jRotatedRectangleF.fAngle = angle;

            PointF[] pointFs = _getJRotatedRectangleFConcor(jRotatedRectangleF);
            if (centeroffsetx == 0 && centeroffsety == 0)
                return pointFs;

            JRotatedRectangleF jRotatedRectangleF1 = new JRotatedRectangleF();
            jRotatedRectangleF1.fCX = (ptfleft.X + ptfright.X) / 2;
            jRotatedRectangleF1.fCY = (ptfleft.Y + ptfright.Y) / 2;
            if (leftoverright)
            {
                jRotatedRectangleF1.fWidth = GetPointLength(ptfleft, ptfbottom) + (BlackCalExtendY + centeroffsety) * Resolution_Mil * 2;
                jRotatedRectangleF1.fHeight = GetPointLength(ptfleft, ptftop) + (BlackCalExtendX + centeroffsetx) * Resolution_Mil * 2;
            }
            else
            {
                jRotatedRectangleF1.fWidth = GetPointLength(ptfleft, ptfbottom) + (BlackCalExtendX + centeroffsetx) * Resolution_Mil * 2;
                jRotatedRectangleF1.fHeight = GetPointLength(ptfleft, ptftop) + (BlackCalExtendY + centeroffsety) * Resolution_Mil * 2;
            }

            jRotatedRectangleF1.fAngle = angle;

            PointF[] pointFs1 = _getJRotatedRectangleFConcor(jRotatedRectangleF1);

            List<string> checkListStr1 = new List<string>();
            //找到X坐标的最左和最右
            int j = 0;
            foreach (PointF ptf in pointFs)
            {
                checkListStr1.Add($"{ptf.X.ToString()},{j.ToString()}");
                j++;
            }
            checkListStr1.Sort((item1, item2) =>
            { return float.Parse(item1.Split(',')[0]) > float.Parse(item2.Split(',')[0]) ? 1 : -1; });

            PointF ptfleft1 = pointFs[int.Parse(checkListStr1[0].Split(',')[1])];
            PointF ptfright1 = pointFs[int.Parse(checkListStr1[1].Split(',')[1])];

            checkListStr1.Clear();
            j = 0;
            foreach (PointF ptf in pointFs1)
            {
                checkListStr1.Add($"{ptf.X.ToString()},{j.ToString()}");
                j++;
            }
            checkListStr1.Sort((item1, item2) =>
            { return float.Parse(item1.Split(',')[0]) > float.Parse(item2.Split(',')[0]) ? -1 : 1; });

            PointF ptfleft2 = pointFs1[int.Parse(checkListStr1[0].Split(',')[1])];
            PointF ptfright2 = pointFs1[int.Parse(checkListStr1[1].Split(',')[1])];

            List<PointF> checkList = new List<PointF>();
            checkList.Add(ptfleft1);
            checkList.Add(ptfright1);
            checkList.Add(ptfleft2);
            checkList.Add(ptfright2);

            return checkList.ToArray();

        }

        private PointF[] _getJRotatedRectangleFConcor(JRotatedRectangleF jRotatedRectangleF)
        {
            PointF ptCenter1 = new PointF((float)jRotatedRectangleF.fCX, (float)jRotatedRectangleF.fCY);
            double iWidth1 = jRotatedRectangleF.fWidth;
            double iHeight1 = jRotatedRectangleF.fHeight;
            if (jRotatedRectangleF.fWidth < jRotatedRectangleF.fHeight)
            {
                iWidth1 = jRotatedRectangleF.fHeight;
                iHeight1 = jRotatedRectangleF.fWidth;
                jRotatedRectangleF.fAngle += 90;

                jRotatedRectangleF.fWidth = (float)iWidth1;
                jRotatedRectangleF.fHeight = (float)iHeight1;
            }

            RectangleF myRectF1 = SimpleRectF(ptCenter1, (float)iWidth1 / 2, (float)iHeight1 / 2);
            //转换矩形的四个角
            PointF[] _myPointFs1 = RectFToPointF(myRectF1, -jRotatedRectangleF.fAngle);
            return _myPointFs1;
        }

        #region BAK20231117最小矩形blob
        /*
         * 
         private int PADRegionFindBlob(Bitmap ebmpInput, int eThresholdValue, PADRegionClass eInputPADRegion, ref Bitmap ebmpOutput)
        {
            //首先MASK 区域出来

            m_BadArea = 0;
            m_BadCount = 0;
            m_BadWidth = 0;
            m_BadHeight = 0;

            imginput = new Bitmap(ebmpInput);
            imgmask = new Bitmap(ebmpInput.Width, ebmpInput.Height);
            Graphics gPrepared = Graphics.FromImage(imgmask);
            gPrepared.Clear(Color.White);
            gPrepared.FillPolygon(Brushes.Black, eInputPADRegion.RegionPtFCorner);
            gPrepared.Dispose();

            m_JzFind.GetMaskedImage(imginput, imgmask, Color.White, Color.White, true);

            //if (RelateAnalyzeString == "A00-02-0002" && m_IsSaveTemp)
            //{
            //    imginput.Save("D:\\testtest\\imgmask1.png", System.Drawing.Imaging.ImageFormat.Png);
            //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
            //}


            //if(m_IsSaveTemp)
            //{
            //    //imginput.Save("D:\\testtest\\" + RelateAnalyzeString + "_blob01" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            //}

            int irange = eThresholdValue;
            Bitmap bmp = new Bitmap(imginput);

            switch (PADThresholdMode)
            {
                case PADThresholdEnum.Ostu_Threshold:

                    AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
                    Bitmap bitmap1 = grayscale.Apply(bmp);

                    AForge.Imaging.Filters.OtsuThreshold otsu = new AForge.Imaging.Filters.OtsuThreshold();
                    Bitmap bitmap2 = otsu.Apply(bitmap1);

                    bmp.Dispose();
                    bmp = new Bitmap(bitmap2);

                    bitmap1.Dispose();
                    bitmap2.Dispose();

                    break;
            }

            JetGrayImg grayimage = new JetGrayImg(bmp);
            JetImgproc.Threshold(grayimage, irange, grayimage);

            //if (RelateAnalyzeString == "A00-02-0002" && m_IsSaveTemp)
            //{
            //    grayimage.ToBitmap().Save("D:\\testtest\\threshold.png", System.Drawing.Imaging.ImageFormat.Png);
            //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
            //}

            //if (m_IsSaveTemp)
            //{
            //    //grayimage.ToBitmap().Save("D:\\testtest\\" + RelateAnalyzeString + "_blob02" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            //}

            //grayimage.ToBitmap().Save("threshold.bmp");
            //int iMAX = -10000000;
            JetBlob jetBlob = new JetBlob();
            jetBlob.Labeling(grayimage, JConnexity.Connexity4, JBlobLayer.BlackLayer);
            int icount = jetBlob.BlobCount;

            bool bNG = false;
            Graphics g = Graphics.FromImage(bmp);
            string msg = "标准长度:" + CheckDWidth.ToString() + ",标准宽度:" + CheckDHeight.ToString() + ",标准面积:" + CheckDArea.ToString() + Environment.NewLine;
            g.DrawString(msg, new Font("宋体", 18), Brushes.Black, 5, 5);

            int jErrorIndex = 0;


            for (int i = 0; i < icount; i++)
            {
                bNG = false;
                int iArea = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.Area);
                JRotatedRectangleF jetrect = JetBlobFeature.ComputeMinRectangle(jetBlob, i);


                //mil 計算

                m_BadArea = iArea * Resolution_Mil * Resolution_Mil;
                m_BadWidth = jetrect.fWidth * Resolution_Mil;
                m_BadHeight = jetrect.fHeight * Resolution_Mil;


                if (m_BadWidth > CheckDWidth)
                {
                    //m_BadWidth = jetrect.fWidth;
                    bNG = true;
                    m_BadCount++;
                }
                else if (m_BadHeight > CheckDHeight)
                {
                    //m_BadHeight = jetrect.fHeight;
                    bNG = true;
                    m_BadCount++;
                }
                else if (m_BadArea > CheckDArea)
                {
                    //m_BadArea = iArea;
                    bNG = true;
                    m_BadCount++;
                }

                //if (jetrect.fWidth > CheckDWidth)
                //{
                //    m_BadWidth = jetrect.fWidth;
                //    bNG = true;
                //    m_BadCount++;
                //}
                //else if (jetrect.fHeight > CheckDHeight)
                //{
                //    m_BadHeight = jetrect.fHeight;
                //    bNG = true;
                //    m_BadCount++;
                //}
                //else if (iArea > CheckDArea)
                //{
                //    m_BadArea = iArea;
                //    bNG = true;
                //    m_BadCount++;
                //}

                if (bNG)
                {
                    Point ptCenter = new Point((int)jetrect.fCX, (int)jetrect.fCY);

                    double iWidth = jetrect.fWidth;
                    double iHeight = jetrect.fHeight;
                    if (jetrect.fWidth < jetrect.fHeight)
                    {
                        iWidth = jetrect.fHeight;
                        iHeight = jetrect.fWidth;
                        jetrect.fAngle += 90;
                    }


                    //msg = "编号:" + i.ToString() +
                    //    ",长度:" + iWidth.ToString("0.0") +
                    //    ",宽度:" + iHeight.ToString("0.0") +
                    //    ",面积:" + iArea.ToString("0.0") + Environment.NewLine;

                    msg = "编号:" + i.ToString() +
                       ",长度:" + m_BadWidth.ToString("0.0") +
                       ",宽度:" + m_BadHeight.ToString("0.0") +
                       ",面积:" + m_BadArea.ToString("0.0") + Environment.NewLine;

                    g.DrawString(msg, new Font("宋体", 10), Brushes.Red, 5, 20 + jErrorIndex * 15);

                    RectangleF myRectF = SimpleRectF(ptCenter, (float)iWidth / 2, (float)iHeight / 2);
                    //转换矩形的四个角
                    PointF[] _myPointFs = RectFToPointF(myRectF, -jetrect.fAngle);

                    Pen p = new Pen(Color.Red, 2);
                    //p.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;

                    Pen pBottom = new Pen(Color.Red, 7);
                    //pBottom.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                    g.DrawLine(p, _myPointFs[0], _myPointFs[1]);
                    g.DrawLine(p, _myPointFs[1], _myPointFs[2]);
                    g.DrawLine(p, _myPointFs[2], _myPointFs[3]);
                    g.DrawLine(pBottom, _myPointFs[0], _myPointFs[3]);

                    //OutPADRegion.RegionWidth = iWidth;
                    //OutPADRegion.RegionHeight = iHeight;
                    //OutPADRegion.RegionArea = iArea;
                    //OutPADRegion.SetPointF(_myPointFs);

                    //m_BadCount++;

                    jErrorIndex++;

                    break;
                }

            }

            g.Dispose();
            //bmp.Save(Application.StartupPath + "\\result.bmp");

            //if (RelateAnalyzeString == "A00-02-0002" && m_IsSaveTemp)
            //{
            //    bmp.Save("D:\\testtest\\outputblob.png", System.Drawing.Imaging.ImageFormat.Png);
            //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
            //}

            if (m_IsSaveTemp && m_BadCount > 0)
            {
                bmp.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "_blob03" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }

            ebmpOutput = new Bitmap(bmp);
            bmpPadBolbOutput = new Bitmap(bmp);

            imginput.Dispose();
            imgmask.Dispose();
            bmp.Dispose();

            return 0;
        }

         */
        #endregion

        private bool _check_region_out_blob(Bitmap ebmpinput, Rectangle erect)
        {
            bool ret = false;

            //先填入rect的黑色 otsu  然后rect白色 找blob 如果blob面积大于20  则NG 返回true

            Bitmap bmpblack = new Bitmap(erect.Width, erect.Height);
            Graphics graphicsb = Graphics.FromImage(bmpblack);
            graphicsb.Clear(Color.Black);
            graphicsb.Dispose();

            Bitmap bmptest = new Bitmap(ebmpinput);
            Graphics graphics = Graphics.FromImage(bmptest);
            graphics.DrawImage(bmpblack, erect);
            graphics.Dispose();

            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            Bitmap bitmap1 = grayscale.Apply(bmptest);
            AForge.Imaging.Filters.OtsuThreshold otsuThreshold = new AForge.Imaging.Filters.OtsuThreshold();
            Bitmap otsubmp = otsuThreshold.Apply(bitmap1);

            Bitmap bmpwhite = new Bitmap(erect.Width, erect.Height);
            Graphics graphicsw = Graphics.FromImage(bmpwhite);
            graphicsw.Clear(Color.White);
            graphicsw.Dispose();

            Bitmap bmmplast = new Bitmap(otsubmp);
            Graphics graphics1 = Graphics.FromImage(bmmplast);
            graphics1.DrawImage(bmpwhite, erect);
            graphics1.Dispose();

            JetGrayImg grayimage = new JetGrayImg(bmmplast);
            JetImgproc.Threshold(grayimage, 80, grayimage);

            //grayimage.ToBitmap().Save("threshold.bmp");

            int iMAX = -10000000;
            int iMAXIndex = 0;

            JetBlob jetBlob = new JetBlob();
            jetBlob.Labeling(grayimage, JConnexity.Connexity4, JBlobLayer.BlackLayer);
            int icount = jetBlob.BlobCount;
            for (int i = 0; i < icount; i++)
            {
                int iArea = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.Area);
                if (iArea > 1)
                {
                    if (iMAX > iArea)
                        continue;
                    else
                    {
                        iMAX = iArea;
                        iMAXIndex = i;
                    }
                }
            }

            if (iMAX > 20)
                ret = true;

            bmpblack.Dispose();
            bmpwhite.Dispose();
            bmptest.Dispose();
            otsubmp.Dispose();
            bmmplast.Dispose();

            return ret;
        }

        private int RegionGlue(Bitmap ebmpInput)
        {
            //GlueRegionClass[] glues = new GlueRegionClass[(int)BorderTypeEnum.COUNT];
            //int i = 0;
            //while (i < (int)BorderTypeEnum.COUNT)
            //{
            //    _get_border_pointf(ebmpInput, (BorderTypeEnum)i, out glues[i]);
            //    i++;
            //}

            return 0;
        }
        private void _get_border_pointf(Bitmap ebmpInput, Rectangle eRect, BorderTypeEnum borderType, out GlueRegionClass glueRegion)
        {
            glueRegion = new GlueRegionClass();
            glueRegion.Reset();

            //int iwidth = 60;
            int iheight = 3;
            Bitmap bmpGlueOrg = new Bitmap(ebmpInput);
            Rectangle m_rect_org = new Rectangle(eRect.X, eRect.Y, eRect.Width, eRect.Height);
            //m_rect_org.Inflate(-iwidth, -iwidth);

            int j = 0;
            PointF pointF0 = new PointF();
            PointF pointF1 = new PointF();
            double[] m_distance = new double[10];
            int m_samplinggap = 3;

            double maxv = -1000;
            double minv = 1000;

            //DataHistogramClass dataHistogram = new DataHistogramClass(15000, 2);
            //dataHistogram.Reset();

            switch (borderType)
            {
                case BorderTypeEnum.LEFT:
                case BorderTypeEnum.RIGHT:
                    m_distance = new double[m_rect_org.Height / m_samplinggap];
                    break;
                case BorderTypeEnum.TOP:
                case BorderTypeEnum.BOTTOM:
                    m_distance = new double[m_rect_org.Width / m_samplinggap];
                    break;
            }

            j = 0;
            while (j < m_distance.Length)
            {
                Rectangle rect0 = new Rectangle(0, m_rect_org.Y + j * m_samplinggap, eRect.X, iheight);

                switch (borderType)
                {
                    case BorderTypeEnum.LEFT:
                        rect0 = new Rectangle(0, m_rect_org.Y + j * m_samplinggap, eRect.X, iheight);
                        break;
                    case BorderTypeEnum.RIGHT:
                        rect0 = new Rectangle(m_rect_org.Right, m_rect_org.Y + j * m_samplinggap, ebmpInput.Width - eRect.Right, iheight);
                        break;
                    case BorderTypeEnum.TOP:
                        rect0 = new Rectangle(m_rect_org.X + j * m_samplinggap, 0, iheight, eRect.Y);
                        break;
                    case BorderTypeEnum.BOTTOM:
                        rect0 = new Rectangle(m_rect_org.X + j * m_samplinggap, m_rect_org.Bottom, iheight, ebmpInput.Height - eRect.Bottom);
                        break;
                }

                Bitmap bitmap = bmpGlueOrg.Clone(rect0, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                //AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
                //Bitmap bitmap1 = grayscale.Apply(bitmap);
                //AForge.Imaging.Filters.HistogramEqualization histogramEqualization = new AForge.Imaging.Filters.HistogramEqualization();
                //Bitmap bitmap2 = histogramEqualization.Apply(bitmap1);
                //bitmap1.Dispose();
                //AForge.Imaging.Filters.BradleyLocalThresholding bradleyLocalThresholding = new AForge.Imaging.Filters.BradleyLocalThresholding();
                //Bitmap bitmap3 = bradleyLocalThresholding.Apply(bitmap2);
                //bitmap2.Dispose();

                //bitmap.Dispose();
                //bitmap = new Bitmap(bitmap3);
                //bitmap3.Dispose();


                JzFindObjectClass jzfind = new JzFindObjectClass();
                //HistogramClass histogram = new HistogramClass(2);
                //histogram.GetHistogram(bitmap);
                //jzfind.SetThreshold(bitmap, SimpleRect(bitmap.Size), (int)((histogram.MaxGrade - histogram.MinGrade) * 0.22), 255, 0, true);

                switch (PADCalMode)
                {
                    case PADCalModeEnum.BlackLast:
                        switch (borderType)
                        {
                            case BorderTypeEnum.LEFT:
                                pointF0 = jzfind.GetBoraderPoint(bitmap, true, true);
                                //if (m_IsSaveTemp)
                                //{
                                //    bitmap.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "0000bmpInput_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                                //}
                                pointF1 = jzfind.GetBoraderPoint(bitmap, true, true, Color.Yellow);
                                //if (m_IsSaveTemp)
                                //{
                                //    bitmap.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "0001bmpInput_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                                //}
                                break;
                            case BorderTypeEnum.RIGHT:
                                pointF0 = jzfind.GetBoraderPoint(bitmap, true, false);
                                //if (m_IsSaveTemp)
                                //{
                                //    bitmap.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "0000bmpInput_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                                //}
                                pointF1 = jzfind.GetBoraderPoint(bitmap, true, false, Color.Yellow);
                                //if (m_IsSaveTemp)
                                //{
                                //    bitmap.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "0001bmpInput_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                                //}
                                break;
                            case BorderTypeEnum.TOP:
                                pointF0 = jzfind.GetBoraderPoint(bitmap, false, true);
                                //if (m_IsSaveTemp)
                                //{
                                //    bitmap.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "0000bmpInput_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                                //}
                                pointF1 = jzfind.GetBoraderPoint(bitmap, false, true, Color.Yellow);
                                //if (m_IsSaveTemp)
                                //{
                                //    bitmap.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "0001bmpInput_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                                //}
                                break;
                            case BorderTypeEnum.BOTTOM:
                                pointF0 = jzfind.GetBoraderPoint(bitmap, false, false);
                                pointF1 = jzfind.GetBoraderPoint(bitmap, false, false, Color.Yellow);
                                break;
                        }
                        break;
                    default:
                        switch (borderType)
                        {
                            case BorderTypeEnum.LEFT:
                                pointF0 = jzfind.GetBoraderPoint(bitmap, true, true);
                                pointF1 = jzfind.GetBoraderPoint(bitmap, true, true, true);
                                break;
                            case BorderTypeEnum.RIGHT:
                                pointF0 = jzfind.GetBoraderPoint(bitmap, true, false);
                                pointF1 = jzfind.GetBoraderPoint(bitmap, true, false, true);
                                break;
                            case BorderTypeEnum.TOP:
                                pointF0 = jzfind.GetBoraderPoint(bitmap, false, true);
                                pointF1 = jzfind.GetBoraderPoint(bitmap, false, true, true);
                                break;
                            case BorderTypeEnum.BOTTOM:
                                pointF0 = jzfind.GetBoraderPoint(bitmap, false, false);
                                pointF1 = jzfind.GetBoraderPoint(bitmap, false, false, true);
                                break;
                        }
                        //switch (borderType)
                        //{
                        //    case BorderTypeEnum.LEFT:
                        //        pointF0 = jzfind.GetBoraderPoint(bitmap, true, true);
                        //        pointF1 = jzfind.GetBoraderPoint(bitmap, true, true, true);
                        //        break;
                        //    case BorderTypeEnum.RIGHT:
                        //        pointF0 = jzfind.GetBoraderPoint(bitmap, true, false);
                        //        pointF1 = jzfind.GetBoraderPoint(bitmap, true, false, true);
                        //        break;
                        //    case BorderTypeEnum.TOP:
                        //        pointF0 = jzfind.GetBoraderPoint(bitmap, false, true);
                        //        pointF1 = jzfind.GetBoraderPoint(bitmap, false, true, true);
                        //        break;
                        //    case BorderTypeEnum.BOTTOM:
                        //        pointF0 = jzfind.GetBoraderPoint(bitmap, false, false);
                        //        pointF1 = jzfind.GetBoraderPoint(bitmap, false, false, true);
                        //        break;
                        //}
                        break;
                }




                m_distance[j] = GetPointLength(pointF0, pointF1);

                //dataHistogram.Add((int)m_distance[j]);

                maxv = Math.Max(maxv, m_distance[j]);
                minv = Math.Min(minv, m_distance[j]);

                pointF0.X += rect0.X;
                pointF0.Y += rect0.Y;

                pointF1.X += rect0.X;
                pointF1.Y += rect0.Y;

                glueRegion.AddPt(pointF0);
                glueRegion.AddPtIN(pointF1);


                //if (m_IsSaveTemp)
                //{
                //    bitmap.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "bmpInput_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                //}

                j++;

                bitmap.Dispose();
            }

            //dataHistogram.Complete();

            //glueRegion.LengthMax = dataHistogram.MaxGrade;
            //glueRegion.LengthMin = dataHistogram.MinGrade;

            glueRegion.LengthMax = maxv;
            glueRegion.LengthMin = minv;

            bmpGlueOrg.Dispose();
        }
        private void _get_border_pointfEx01(Bitmap ebmpInput, Bitmap ebmpInput0, Rectangle eRect, BorderTypeEnum borderType, out GlueRegionClass glueRegion)
        {
            glueRegion = new GlueRegionClass();
            glueRegion.Reset();

            //int iwidth = 60;
            int iheight = 3;
            Bitmap bmpGlueOrg = new Bitmap(ebmpInput);
            Bitmap bmpGlueOrg0 = new Bitmap(ebmpInput0);
            Rectangle m_rect_org = new Rectangle(eRect.X, eRect.Y, eRect.Width, eRect.Height);
            //m_rect_org.Inflate(-iwidth, -iwidth);

            int j = 0;
            PointF pointF0 = new PointF();
            PointF pointF1 = new PointF();
            double[] m_distance = new double[10];
            int m_samplinggap = 3;

            double maxv = -1000;
            double minv = 1000;

            //DataHistogramClass dataHistogram = new DataHistogramClass(15000, 2);
            //dataHistogram.Reset();

            switch (borderType)
            {
                case BorderTypeEnum.LEFT:
                case BorderTypeEnum.RIGHT:
                    m_distance = new double[m_rect_org.Height / m_samplinggap];
                    break;
                case BorderTypeEnum.TOP:
                case BorderTypeEnum.BOTTOM:
                    m_distance = new double[m_rect_org.Width / m_samplinggap];
                    break;
            }

            j = 0;
            while (j < m_distance.Length)
            {
                Rectangle rect0 = new Rectangle(0, m_rect_org.Y + j * m_samplinggap, eRect.X, iheight);

                switch (borderType)
                {
                    case BorderTypeEnum.LEFT:
                        rect0 = new Rectangle(0, m_rect_org.Y + j * m_samplinggap, eRect.X, iheight);
                        break;
                    case BorderTypeEnum.RIGHT:
                        rect0 = new Rectangle(m_rect_org.Right, m_rect_org.Y + j * m_samplinggap, ebmpInput.Width - eRect.Right, iheight);
                        break;
                    case BorderTypeEnum.TOP:
                        rect0 = new Rectangle(m_rect_org.X + j * m_samplinggap, 0, iheight, eRect.Y);
                        break;
                    case BorderTypeEnum.BOTTOM:
                        rect0 = new Rectangle(m_rect_org.X + j * m_samplinggap, m_rect_org.Bottom, iheight, ebmpInput.Height - eRect.Bottom);
                        break;
                }
                Bitmap bitmap0 = bmpGlueOrg0.Clone(rect0, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                Bitmap bitmap = bmpGlueOrg.Clone(rect0, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                //AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
                //Bitmap bitmap1 = grayscale.Apply(bitmap);
                //AForge.Imaging.Filters.HistogramEqualization histogramEqualization = new AForge.Imaging.Filters.HistogramEqualization();
                //Bitmap bitmap2 = histogramEqualization.Apply(bitmap1);
                //bitmap1.Dispose();
                //AForge.Imaging.Filters.BradleyLocalThresholding bradleyLocalThresholding = new AForge.Imaging.Filters.BradleyLocalThresholding();
                //Bitmap bitmap3 = bradleyLocalThresholding.Apply(bitmap2);
                //bitmap2.Dispose();

                //bitmap.Dispose();
                //bitmap = new Bitmap(bitmap3);
                //bitmap3.Dispose();


                JzFindObjectClass jzfind = new JzFindObjectClass();
                //HistogramClass histogram = new HistogramClass(2);
                //histogram.GetHistogram(bitmap);
                //jzfind.SetThreshold(bitmap, SimpleRect(bitmap.Size), (int)((histogram.MaxGrade - histogram.MinGrade) * 0.22), 255, 0, true);

                switch (PADCalMode)
                {
                    case PADCalModeEnum.BlackLast:
                        switch (borderType)
                        {
                            case BorderTypeEnum.LEFT:
                                pointF0 = jzfind.GetBoraderPoint(bitmap0, true, true);
                                //if (m_IsSaveTemp)
                                //{
                                //    bitmap.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "0000bmpInput_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                                //}
                                pointF1 = jzfind.GetBoraderPoint(bitmap, true, true, Color.Yellow);
                                //if (m_IsSaveTemp)
                                //{
                                //    bitmap.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "0001bmpInput_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                                //}
                                break;
                            case BorderTypeEnum.RIGHT:
                                pointF0 = jzfind.GetBoraderPoint(bitmap0, true, false);
                                //if (m_IsSaveTemp)
                                //{
                                //    bitmap.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "0000bmpInput_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                                //}
                                pointF1 = jzfind.GetBoraderPoint(bitmap, true, false, Color.Yellow);
                                //if (m_IsSaveTemp)
                                //{
                                //    bitmap.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "0001bmpInput_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                                //}
                                break;
                            case BorderTypeEnum.TOP:
                                pointF0 = jzfind.GetBoraderPoint(bitmap0, false, true);
                                //if (m_IsSaveTemp)
                                //{
                                //    bitmap.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "0000bmpInput_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                                //}
                                pointF1 = jzfind.GetBoraderPoint(bitmap, false, true, Color.Yellow);
                                //if (m_IsSaveTemp)
                                //{
                                //    bitmap.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "0001bmpInput_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                                //}
                                break;
                            case BorderTypeEnum.BOTTOM:
                                pointF0 = jzfind.GetBoraderPoint(bitmap0, false, false);
                                pointF1 = jzfind.GetBoraderPoint(bitmap, false, false, Color.Yellow);
                                break;
                        }
                        break;
                    default:
                        switch (borderType)
                        {
                            case BorderTypeEnum.LEFT:
                                pointF0 = jzfind.GetBoraderPoint(bitmap0, true, true);
                                pointF1 = jzfind.GetBoraderPoint(bitmap, true, true, true);
                                break;
                            case BorderTypeEnum.RIGHT:
                                pointF0 = jzfind.GetBoraderPoint(bitmap0, true, false);
                                pointF1 = jzfind.GetBoraderPoint(bitmap, true, false, true);
                                break;
                            case BorderTypeEnum.TOP:
                                pointF0 = jzfind.GetBoraderPoint(bitmap0, false, true);
                                pointF1 = jzfind.GetBoraderPoint(bitmap, false, true, true);
                                break;
                            case BorderTypeEnum.BOTTOM:
                                pointF0 = jzfind.GetBoraderPoint(bitmap0, false, false);
                                pointF1 = jzfind.GetBoraderPoint(bitmap, false, false, true);
                                break;
                        }
                        //switch (borderType)
                        //{
                        //    case BorderTypeEnum.LEFT:
                        //        pointF0 = jzfind.GetBoraderPoint(bitmap, true, true);
                        //        pointF1 = jzfind.GetBoraderPoint(bitmap, true, true, true);
                        //        break;
                        //    case BorderTypeEnum.RIGHT:
                        //        pointF0 = jzfind.GetBoraderPoint(bitmap, true, false);
                        //        pointF1 = jzfind.GetBoraderPoint(bitmap, true, false, true);
                        //        break;
                        //    case BorderTypeEnum.TOP:
                        //        pointF0 = jzfind.GetBoraderPoint(bitmap, false, true);
                        //        pointF1 = jzfind.GetBoraderPoint(bitmap, false, true, true);
                        //        break;
                        //    case BorderTypeEnum.BOTTOM:
                        //        pointF0 = jzfind.GetBoraderPoint(bitmap, false, false);
                        //        pointF1 = jzfind.GetBoraderPoint(bitmap, false, false, true);
                        //        break;
                        //}
                        break;
                }




                m_distance[j] = GetPointLength(pointF0, pointF1);

                //dataHistogram.Add((int)m_distance[j]);

                maxv = Math.Max(maxv, m_distance[j]);
                minv = Math.Min(minv, m_distance[j]);

                pointF0.X += rect0.X;
                pointF0.Y += rect0.Y;

                pointF1.X += rect0.X;
                pointF1.Y += rect0.Y;

                glueRegion.AddPt(pointF0);
                glueRegion.AddPtIN(pointF1);


                //if (m_IsSaveTemp)
                //{
                //    bitmap.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "bmpInput_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                //}

                j++;

                bitmap.Dispose();
            }

            //dataHistogram.Complete();

            //glueRegion.LengthMax = dataHistogram.MaxGrade;
            //glueRegion.LengthMin = dataHistogram.MinGrade;

            glueRegion.LengthMax = maxv;
            glueRegion.LengthMin = minv;

            bmpGlueOrg.Dispose();
        }

        private void _get_border_pointf_v2(Bitmap ebmpInput, Rectangle eRect, BorderTypeEnum borderType, out GlueRegionClass glueRegion)
        {
            glueRegion = new GlueRegionClass();
            glueRegion.Reset();

            //int iwidth = 60;
            int iheight = 3;
            Bitmap bmpGlueOrg = new Bitmap(ebmpInput);
            Rectangle m_rect_org = new Rectangle(eRect.X, eRect.Y, eRect.Width, eRect.Height);
            //m_rect_org.Inflate(-iwidth, -iwidth);

            int j = 0;
            PointF pointF0 = new PointF();
            PointF pointF1 = new PointF();
            double[] m_distance = new double[10];
            int m_samplinggap = 3;

            double maxv = -1000;
            double minv = 1000;

            //DataHistogramClass dataHistogram = new DataHistogramClass(15000, 2);
            //dataHistogram.Reset();

            switch (borderType)
            {
                case BorderTypeEnum.LEFT:
                case BorderTypeEnum.RIGHT:
                    m_distance = new double[m_rect_org.Height / m_samplinggap];
                    break;
                case BorderTypeEnum.TOP:
                case BorderTypeEnum.BOTTOM:
                    m_distance = new double[m_rect_org.Width / m_samplinggap];
                    break;
            }

            j = 0;
            while (j < m_distance.Length)
            {
                Rectangle rect0 = new Rectangle(0, m_rect_org.Y + j * m_samplinggap, eRect.X, iheight);

                switch (borderType)
                {
                    case BorderTypeEnum.LEFT:
                        rect0 = new Rectangle(0, m_rect_org.Y + j * m_samplinggap, eRect.X, iheight);
                        break;
                    case BorderTypeEnum.RIGHT:
                        rect0 = new Rectangle(m_rect_org.Right, m_rect_org.Y + j * m_samplinggap, ebmpInput.Width - eRect.Right, iheight);
                        break;
                    case BorderTypeEnum.TOP:
                        rect0 = new Rectangle(m_rect_org.X + j * m_samplinggap, 0, iheight, eRect.Y);
                        break;
                    case BorderTypeEnum.BOTTOM:
                        rect0 = new Rectangle(m_rect_org.X + j * m_samplinggap, m_rect_org.Bottom, iheight, ebmpInput.Height - eRect.Bottom);
                        break;
                }

                Bitmap bitmap = bmpGlueOrg.Clone(rect0, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                JzFindObjectClass jzfind = new JzFindObjectClass();

                switch (borderType)
                {
                    case BorderTypeEnum.LEFT:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, true, true);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap, true, true, Color.FromArgb(0, 255, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, true, true, Color.FromArgb(0, 255, 0));
                        break;
                    case BorderTypeEnum.RIGHT:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, true, false);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap, true, false, Color.FromArgb(0, 255, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, true, false, Color.FromArgb(0, 255, 0));
                        break;
                    case BorderTypeEnum.TOP:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, false, true);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap, false, true, Color.FromArgb(0, 255, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, false, true, Color.FromArgb(0, 255, 0));
                        break;
                    case BorderTypeEnum.BOTTOM:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, false, false);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap, false, false, Color.FromArgb(0, 255, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, false, false, Color.FromArgb(0, 255, 0));
                        break;
                }

                //switch (PADCalMode)
                //{
                //    case PADCalModeEnum.BlackLast:

                //        break;
                //    default:
                //        switch (borderType)
                //        {
                //            case BorderTypeEnum.LEFT:
                //                pointF0 = jzfind.GetBoraderPoint(bitmap, true, true);
                //                pointF1 = jzfind.GetBoraderPoint(bitmap, true, true, true);
                //                break;
                //            case BorderTypeEnum.RIGHT:
                //                pointF0 = jzfind.GetBoraderPoint(bitmap, true, false);
                //                pointF1 = jzfind.GetBoraderPoint(bitmap, true, false, true);
                //                break;
                //            case BorderTypeEnum.TOP:
                //                pointF0 = jzfind.GetBoraderPoint(bitmap, false, true);
                //                pointF1 = jzfind.GetBoraderPoint(bitmap, false, true, true);
                //                break;
                //            case BorderTypeEnum.BOTTOM:
                //                pointF0 = jzfind.GetBoraderPoint(bitmap, false, false);
                //                pointF1 = jzfind.GetBoraderPoint(bitmap, false, false, true);
                //                break;
                //        }
                //        break;
                //}




                m_distance[j] = GetPointLength(pointF0, pointF1);

                //dataHistogram.Add((int)m_distance[j]);

                maxv = Math.Max(maxv, m_distance[j]);
                minv = Math.Min(minv, m_distance[j]);

                pointF0.X += rect0.X;
                pointF0.Y += rect0.Y;

                pointF1.X += rect0.X;
                pointF1.Y += rect0.Y;

                glueRegion.AddPt(pointF0);
                glueRegion.AddPtIN(pointF1);


                //if (m_IsSaveTemp)
                //{
                //    bitmap.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "bmpInput_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                //}

                j++;

                bitmap.Dispose();
            }

            //dataHistogram.Complete();

            //glueRegion.LengthMax = dataHistogram.MaxGrade;
            //glueRegion.LengthMin = dataHistogram.MinGrade;

            glueRegion.LengthMax = maxv;
            glueRegion.LengthMin = minv;

            bmpGlueOrg.Dispose();
        }
        private void _get_border_pointf_v3(Bitmap ebmpInput, Bitmap ebmpInput0, Rectangle eRect, BorderTypeEnum borderType, out GlueRegionClass glueRegion)
        {
            glueRegion = new GlueRegionClass();
            glueRegion.Reset();

            //int iwidth = 60;
            int iheight = 3;
            Bitmap bmpGlueOrg = (Bitmap)ebmpInput.Clone();// new Bitmap(ebmpInput);
            Bitmap bmpGlueOrg0 = (Bitmap)ebmpInput0.Clone();//new Bitmap(ebmpInput0);
            Rectangle m_rect_org = new Rectangle(eRect.X, eRect.Y, eRect.Width, eRect.Height);
            //m_rect_org.Inflate(-iwidth, -iwidth);

            int j = 0;
            PointF pointF0 = new PointF();
            PointF pointF1 = new PointF();
            double[] m_distance = new double[10];
            int m_samplinggap = 137;
            m_samplinggap = 37;

            int _minsize = Math.Min(eRect.Width, eRect.Height);
            if (_minsize >= 1500)
                m_samplinggap = 137;
            else
                m_samplinggap = 37;

            double maxv = -1000;
            double minv = 1000;
            int iyoffset = (int)(m_samplinggap / 1.5);

            //DataHistogramClass dataHistogram = new DataHistogramClass(15000, 2);
            //dataHistogram.Reset();

            switch (borderType)
            {
                case BorderTypeEnum.LEFT:
                case BorderTypeEnum.RIGHT:
                    m_distance = new double[m_rect_org.Height / m_samplinggap];
                    break;
                case BorderTypeEnum.TOP:
                case BorderTypeEnum.BOTTOM:
                    m_distance = new double[m_rect_org.Width / m_samplinggap];
                    break;
            }

            j = 0;
            while (j < m_distance.Length)
            {
                Rectangle rect0 = new Rectangle(0, m_rect_org.Y + j * m_samplinggap, eRect.X, iheight);

                switch (borderType)
                {
                    case BorderTypeEnum.LEFT:
                        rect0 = new Rectangle(0, m_rect_org.Y + j * m_samplinggap + iyoffset, eRect.X, iheight);
                        break;
                    case BorderTypeEnum.RIGHT:
                        rect0 = new Rectangle(m_rect_org.Right, m_rect_org.Y + j * m_samplinggap + iyoffset, ebmpInput.Width - eRect.Right, iheight);
                        break;
                    case BorderTypeEnum.TOP:
                        rect0 = new Rectangle(m_rect_org.X + j * m_samplinggap + iyoffset, 0, iheight, eRect.Y);
                        break;
                    case BorderTypeEnum.BOTTOM:
                        rect0 = new Rectangle(m_rect_org.X + j * m_samplinggap + iyoffset, m_rect_org.Bottom, iheight, ebmpInput.Height - eRect.Bottom);
                        break;
                }

                Bitmap bitmap0 = bmpGlueOrg0.Clone(rect0, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                Bitmap bitmap = bmpGlueOrg.Clone(rect0, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                //AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
                //Bitmap bitmap1 = grayscale.Apply(bitmap0);

                //AForge.Imaging.Filters.OtsuThreshold otsu = new AForge.Imaging.Filters.OtsuThreshold();
                //Bitmap bitmap2 = otsu.Apply(bitmap1);

                //Bitmap bitmap3 = otsu.Apply(bitmap2);
                //bitmap1.Dispose();
                //bitmap2.Dispose();

                JzFindObjectClass jzfind = new JzFindObjectClass();

                switch (borderType)
                {
                    case BorderTypeEnum.LEFT:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, true, true);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, true, true, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, true, true, Color.FromArgb(0, 255, 0));
                        break;
                    case BorderTypeEnum.RIGHT:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, true, false);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, true, false, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, true, false, Color.FromArgb(0, 255, 0));
                        break;
                    case BorderTypeEnum.TOP:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, false, true);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, false, true, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, false, true, Color.FromArgb(0, 255, 0));
                        break;
                    case BorderTypeEnum.BOTTOM:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, false, false);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, false, false, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, false, false, Color.FromArgb(0, 255, 0));
                        //pointF1 = jzfind.GetBoraderPointv2(bitmap, false, false, Color.FromArgb(0, 0, 0), true);
                        break;
                }
                if (INI.CHIP_CAL_MODE == 1)
                {
                    switch (borderType)
                    {
                        case BorderTypeEnum.LEFT:
                            if (m_PtfCenter.Y >= 450)
                                pointF0.X += lineClass_left.GetPtFromX(m_PtfCenter.X).Y;
                            break;
                        case BorderTypeEnum.RIGHT:
                            //if (m_PtfCenter.Y <= 3250)
                            //    pointF0.X -= lineClass_right.GetPtFromX(m_PtfCenter.X).Y;
                            pointF0.X -= 4.3f;
                            break;
                        case BorderTypeEnum.TOP:
                            if (m_PtfCenter.Y >= 1400)
                                pointF0.Y += lineClass_top.GetPtFromX(m_PtfCenter.Y).Y;
                            break;
                        case BorderTypeEnum.BOTTOM:
                            if (m_PtfCenter.Y <= 4000)
                                pointF0.Y -= lineClass_bottom.GetPtFromX(m_PtfCenter.Y).Y;
                            break;
                    }
                }

                //switch (PADCalMode)
                //{
                //    case PADCalModeEnum.BlackLast:

                //        break;
                //    default:
                //        switch (borderType)
                //        {
                //            case BorderTypeEnum.LEFT:
                //                pointF0 = jzfind.GetBoraderPoint(bitmap, true, true);
                //                pointF1 = jzfind.GetBoraderPoint(bitmap, true, true, true);
                //                break;
                //            case BorderTypeEnum.RIGHT:
                //                pointF0 = jzfind.GetBoraderPoint(bitmap, true, false);
                //                pointF1 = jzfind.GetBoraderPoint(bitmap, true, false, true);
                //                break;
                //            case BorderTypeEnum.TOP:
                //                pointF0 = jzfind.GetBoraderPoint(bitmap, false, true);
                //                pointF1 = jzfind.GetBoraderPoint(bitmap, false, true, true);
                //                break;
                //            case BorderTypeEnum.BOTTOM:
                //                pointF0 = jzfind.GetBoraderPoint(bitmap, false, false);
                //                pointF1 = jzfind.GetBoraderPoint(bitmap, false, false, true);
                //                break;
                //        }
                //        break;
                //}




                m_distance[j] = GetPointLength(pointF0, pointF1);

                //dataHistogram.Add((int)m_distance[j]);

                maxv = Math.Max(maxv, m_distance[j]);
                minv = Math.Min(minv, m_distance[j]);

                pointF0.X += rect0.X;
                pointF0.Y += rect0.Y;

                pointF1.X += rect0.X;
                pointF1.Y += rect0.Y;

                glueRegion.AddPt(pointF0);//内芯片的点
                glueRegion.AddPtIN(pointF1);//外围胶的点


                //if (m_IsSaveTemp)
                //{
                //    bitmap.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "bmpInput_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                //}

                j++;

                bitmap.Dispose();
            }

            //dataHistogram.Complete();

            //glueRegion.LengthMax = dataHistogram.MaxGrade;
            //glueRegion.LengthMin = dataHistogram.MinGrade;

            glueRegion.LengthMax = maxv;
            glueRegion.LengthMin = minv;

            bmpGlueOrg.Dispose();
        }
        private void _get_border_pointf_v3_1(Bitmap ebmpInput, Bitmap ebmpInput0, Rectangle eRect, BorderTypeEnum borderType, out GlueRegionClass glueRegion)
        {
            glueRegion = new GlueRegionClass();
            glueRegion.Reset();

            //int iwidth = 60;
            int iheight = 3;
            Bitmap bmpGlueOrg = new Bitmap(ebmpInput);
            Bitmap bmpGlueOrg0 = new Bitmap(ebmpInput0);
            Rectangle m_rect_org = new Rectangle(eRect.X, eRect.Y, eRect.Width, eRect.Height);
            //m_rect_org.Inflate(-iwidth, -iwidth);

            int j = 0;
            PointF pointF0 = new PointF();
            PointF pointF1 = new PointF();
            double[] m_distance = new double[10];
            int m_samplinggap = 13;

            double maxv = -1000;
            double minv = 1000;

            //DataHistogramClass dataHistogram = new DataHistogramClass(15000, 2);
            //dataHistogram.Reset();

            switch (borderType)
            {
                case BorderTypeEnum.LEFT:
                case BorderTypeEnum.RIGHT:
                    m_distance = new double[m_rect_org.Height / m_samplinggap];
                    break;
                case BorderTypeEnum.TOP:
                case BorderTypeEnum.BOTTOM:
                    m_distance = new double[m_rect_org.Width / m_samplinggap];
                    break;
            }

            j = 0;
            while (j < m_distance.Length)
            {
                Rectangle rect0 = new Rectangle(0, m_rect_org.Y + j * m_samplinggap, eRect.X, iheight);

                switch (borderType)
                {
                    case BorderTypeEnum.LEFT:
                        rect0 = new Rectangle(0, m_rect_org.Y + j * m_samplinggap, eRect.X, iheight);
                        break;
                    case BorderTypeEnum.RIGHT:
                        rect0 = new Rectangle(m_rect_org.Right, m_rect_org.Y + j * m_samplinggap, ebmpInput.Width - eRect.Right, iheight);
                        break;
                    case BorderTypeEnum.TOP:
                        rect0 = new Rectangle(m_rect_org.X + j * m_samplinggap, 0, iheight, eRect.Y);
                        break;
                    case BorderTypeEnum.BOTTOM:
                        rect0 = new Rectangle(m_rect_org.X + j * m_samplinggap, m_rect_org.Bottom, iheight, ebmpInput.Height - eRect.Bottom);
                        break;
                }

                Bitmap bitmap0 = bmpGlueOrg0.Clone(rect0, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                Bitmap bitmap = bmpGlueOrg.Clone(rect0, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                //AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
                //Bitmap bitmap1 = grayscale.Apply(bitmap0);

                //AForge.Imaging.Filters.OtsuThreshold otsu = new AForge.Imaging.Filters.OtsuThreshold();
                //Bitmap bitmap2 = otsu.Apply(bitmap1);

                //Bitmap bitmap3 = otsu.Apply(bitmap2);
                //bitmap1.Dispose();
                //bitmap2.Dispose();

                JzFindObjectClass jzfind = new JzFindObjectClass();

                switch (borderType)
                {
                    case BorderTypeEnum.LEFT:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, true, true);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, true, true, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, true, true, Color.FromArgb(255, 255, 255));
                        break;
                    case BorderTypeEnum.RIGHT:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, true, false);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, true, false, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, true, false, Color.FromArgb(255, 255, 255));
                        break;
                    case BorderTypeEnum.TOP:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, false, true);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, false, true, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, false, true, Color.FromArgb(255, 255, 255));
                        break;
                    case BorderTypeEnum.BOTTOM:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, false, false);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, false, false, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, false, false, Color.FromArgb(255, 255, 255));
                        break;
                }
                if (INI.CHIP_CAL_MODE == 1)
                {
                    switch (borderType)
                    {
                        case BorderTypeEnum.LEFT:
                            if (m_PtfCenter.Y >= 450)
                                pointF0.X += lineClass_left.GetPtFromX(m_PtfCenter.X).Y;
                            break;
                        case BorderTypeEnum.RIGHT:
                            //if (m_PtfCenter.Y <= 3250)
                            //    pointF0.X -= lineClass_right.GetPtFromX(m_PtfCenter.X).Y;
                            pointF0.X -= 4.3f;
                            break;
                        case BorderTypeEnum.TOP:
                            if (m_PtfCenter.Y >= 1400)
                                pointF0.Y += lineClass_top.GetPtFromX(m_PtfCenter.Y).Y;
                            break;
                        case BorderTypeEnum.BOTTOM:
                            if (m_PtfCenter.Y <= 4000)
                                pointF0.Y -= lineClass_bottom.GetPtFromX(m_PtfCenter.Y).Y;
                            break;
                    }
                }

                //switch (PADCalMode)
                //{
                //    case PADCalModeEnum.BlackLast:

                //        break;
                //    default:
                //        switch (borderType)
                //        {
                //            case BorderTypeEnum.LEFT:
                //                pointF0 = jzfind.GetBoraderPoint(bitmap, true, true);
                //                pointF1 = jzfind.GetBoraderPoint(bitmap, true, true, true);
                //                break;
                //            case BorderTypeEnum.RIGHT:
                //                pointF0 = jzfind.GetBoraderPoint(bitmap, true, false);
                //                pointF1 = jzfind.GetBoraderPoint(bitmap, true, false, true);
                //                break;
                //            case BorderTypeEnum.TOP:
                //                pointF0 = jzfind.GetBoraderPoint(bitmap, false, true);
                //                pointF1 = jzfind.GetBoraderPoint(bitmap, false, true, true);
                //                break;
                //            case BorderTypeEnum.BOTTOM:
                //                pointF0 = jzfind.GetBoraderPoint(bitmap, false, false);
                //                pointF1 = jzfind.GetBoraderPoint(bitmap, false, false, true);
                //                break;
                //        }
                //        break;
                //}




                m_distance[j] = GetPointLength(pointF0, pointF1);

                //dataHistogram.Add((int)m_distance[j]);

                maxv = Math.Max(maxv, m_distance[j]);
                minv = Math.Min(minv, m_distance[j]);

                pointF0.X += rect0.X;
                pointF0.Y += rect0.Y;

                pointF1.X += rect0.X;
                pointF1.Y += rect0.Y;

                glueRegion.AddPt(pointF0);//内芯片的点
                glueRegion.AddPtIN(pointF1);//外围胶的点


                //if (m_IsSaveTemp)
                //{
                //    bitmap.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "bmpInput_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                //}

                j++;

                bitmap.Dispose();
            }

            //dataHistogram.Complete();

            //glueRegion.LengthMax = dataHistogram.MaxGrade;
            //glueRegion.LengthMin = dataHistogram.MinGrade;

            glueRegion.LengthMax = maxv;
            glueRegion.LengthMin = minv;

            bmpGlueOrg.Dispose();
        }
        private void _get_border_pointf_v5(Bitmap ebmpInput, Rectangle rectlocation, BorderTypeEnum borderType, out GlueRegionClass glueRegion)
        {
            glueRegion = new GlueRegionClass();
            glueRegion.Reset();

            //int iwidth = 60;
            int iheight = 3;
            Bitmap bmpGlueOrg = new Bitmap(ebmpInput);
            Rectangle m_rect_org = new Rectangle(0, 0, ebmpInput.Width, ebmpInput.Height);
            //m_rect_org.Inflate(-iwidth, -iwidth);

            int j = 0;
            PointF pointF0 = new PointF();
            PointF pointF1 = new PointF();
            double[] m_distance = new double[10];
            int m_samplinggap = 13;

            double maxv = -1000;
            double minv = 1000;

            //DataHistogramClass dataHistogram = new DataHistogramClass(15000, 2);
            //dataHistogram.Reset();

            switch (borderType)
            {
                case BorderTypeEnum.LEFT:
                case BorderTypeEnum.RIGHT:
                    m_distance = new double[m_rect_org.Height / m_samplinggap];
                    break;
                case BorderTypeEnum.TOP:
                case BorderTypeEnum.BOTTOM:
                    m_distance = new double[m_rect_org.Width / m_samplinggap];
                    break;
            }

            j = 0;
            while (j < m_distance.Length)
            {
                Rectangle rect0 = new Rectangle(0, m_rect_org.Y + j * m_samplinggap, m_rect_org.Width, iheight);

                switch (borderType)
                {
                    case BorderTypeEnum.LEFT:
                    case BorderTypeEnum.RIGHT:
                        rect0 = new Rectangle(0, m_rect_org.Y + j * m_samplinggap, m_rect_org.Width - 1, iheight);
                        break;
                    case BorderTypeEnum.TOP:
                    case BorderTypeEnum.BOTTOM:
                        rect0 = new Rectangle(m_rect_org.X + j * m_samplinggap, 0, iheight, m_rect_org.Height - 1);
                        break;
                }


                Bitmap bitmap = bmpGlueOrg.Clone(rect0, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                JzFindObjectClass jzfind = new JzFindObjectClass();

                switch (borderType)
                {
                    case BorderTypeEnum.LEFT:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, true, true);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap, true, true, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, true, false, Color.FromArgb(0, 0, 0), true);
                        break;
                    case BorderTypeEnum.RIGHT:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, true, false);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap, true, false, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, true, true, Color.FromArgb(0, 0, 0), true);
                        break;
                    case BorderTypeEnum.TOP:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, false, true);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap, false, true, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, false, false, Color.FromArgb(0, 0, 0), true);
                        break;
                    case BorderTypeEnum.BOTTOM:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, false, false);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap, false, false, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, false, true, Color.FromArgb(0, 0, 0), true);
                        break;
                }
                if (INI.CHIP_CAL_MODE == 1)
                {
                    switch (borderType)
                    {
                        case BorderTypeEnum.LEFT:
                            if (m_PtfCenter.Y >= 450)
                                pointF0.X += lineClass_left.GetPtFromX(m_PtfCenter.X).Y;
                            break;
                        case BorderTypeEnum.RIGHT:
                            //if (m_PtfCenter.Y <= 3250)
                            //    pointF0.X -= lineClass_right.GetPtFromX(m_PtfCenter.X).Y;
                            pointF0.X -= 4.3f;
                            break;
                        case BorderTypeEnum.TOP:
                            if (m_PtfCenter.Y >= 1400)
                                pointF0.Y += lineClass_top.GetPtFromX(m_PtfCenter.Y).Y;
                            break;
                        case BorderTypeEnum.BOTTOM:
                            if (m_PtfCenter.Y <= 4000)
                                pointF0.Y -= lineClass_bottom.GetPtFromX(m_PtfCenter.Y).Y;
                            break;
                    }
                }

                m_distance[j] = GetPointLength(pointF0, pointF1);

                //dataHistogram.Add((int)m_distance[j]);

                maxv = Math.Max(maxv, m_distance[j]);
                minv = Math.Min(minv, m_distance[j]);

                pointF0.X += rect0.X + rectlocation.X;
                pointF0.Y += rect0.Y + rectlocation.Y;

                pointF1.X += rect0.X + rectlocation.X;
                pointF1.Y += rect0.Y + rectlocation.Y;

                glueRegion.AddPt(pointF0);//内芯片的点
                glueRegion.AddPtIN(pointF1);//外围胶的点


                //if (m_IsSaveTemp)
                //{
                //    bitmap.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "bmpInput_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                //}

                j++;

                bitmap.Dispose();
            }

            //dataHistogram.Complete();

            //glueRegion.LengthMax = dataHistogram.MaxGrade;
            //glueRegion.LengthMin = dataHistogram.MinGrade;

            glueRegion.LengthMax = maxv;
            glueRegion.LengthMin = minv;

            bmpGlueOrg.Dispose();
        }
        private void _get_border_pointf_v6_1(Bitmap ebmpInput, Bitmap ebmpInput0, Rectangle eRect, BorderTypeEnum borderType, out GlueRegionClass glueRegion)
        {
            glueRegion = new GlueRegionClass();
            glueRegion.Reset();

            //int iwidth = 60;
            int iheight = 3;
            Bitmap bmpGlueOrg = new Bitmap(ebmpInput);
            Bitmap bmpGlueOrg0 = new Bitmap(ebmpInput0);
            Rectangle m_rect_org = new Rectangle(eRect.X, eRect.Y, eRect.Width, eRect.Height);
            //m_rect_org.Inflate(-iwidth, -iwidth);

            int j = 0;
            PointF pointF0 = new PointF();
            PointF pointF1 = new PointF();
            double[] m_distance = new double[10];
            int m_samplinggap = 137;

            double maxv = -1000;
            double minv = 1000;

            //DataHistogramClass dataHistogram = new DataHistogramClass(15000, 2);
            //dataHistogram.Reset();

            switch (borderType)
            {
                case BorderTypeEnum.LEFT:
                case BorderTypeEnum.RIGHT:
                    m_distance = new double[m_rect_org.Height / m_samplinggap];
                    break;
                case BorderTypeEnum.TOP:
                case BorderTypeEnum.BOTTOM:
                    m_distance = new double[m_rect_org.Width / m_samplinggap];
                    break;
            }

            j = 0;
            while (j < m_distance.Length)
            {
                Rectangle rect0 = new Rectangle(0, m_rect_org.Y + j * m_samplinggap, eRect.X, iheight);

                switch (borderType)
                {
                    case BorderTypeEnum.LEFT:
                        rect0 = new Rectangle(0, m_rect_org.Y + j * m_samplinggap, eRect.X, iheight);
                        break;
                    case BorderTypeEnum.RIGHT:
                        rect0 = new Rectangle(m_rect_org.Right, m_rect_org.Y + j * m_samplinggap, ebmpInput.Width - eRect.Right, iheight);
                        break;
                    case BorderTypeEnum.TOP:
                        rect0 = new Rectangle(m_rect_org.X + j * m_samplinggap, 0, iheight, eRect.Y);
                        break;
                    case BorderTypeEnum.BOTTOM:
                        rect0 = new Rectangle(m_rect_org.X + j * m_samplinggap, m_rect_org.Bottom, iheight, ebmpInput.Height - eRect.Bottom);
                        break;
                }

                Bitmap bitmap0 = bmpGlueOrg0.Clone(rect0, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                Bitmap bitmap = bmpGlueOrg.Clone(rect0, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                //AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
                //Bitmap bitmap1 = grayscale.Apply(bitmap0);

                //AForge.Imaging.Filters.OtsuThreshold otsu = new AForge.Imaging.Filters.OtsuThreshold();
                //Bitmap bitmap2 = otsu.Apply(bitmap1);

                //Bitmap bitmap3 = otsu.Apply(bitmap2);
                //bitmap1.Dispose();
                //bitmap2.Dispose();

                JzFindObjectClass jzfind = new JzFindObjectClass();

                switch (borderType)
                {
                    case BorderTypeEnum.LEFT:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, true, true);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, true, true, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, true, true, Color.FromArgb(255, 255, 255));
                        break;
                    case BorderTypeEnum.RIGHT:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, true, false);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, true, false, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, true, false, Color.FromArgb(255, 255, 255));
                        break;
                    case BorderTypeEnum.TOP:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, false, true);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, false, true, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, false, true, Color.FromArgb(255, 255, 255));
                        break;
                    case BorderTypeEnum.BOTTOM:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, false, false);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, false, false, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, false, false, Color.FromArgb(255, 255, 255));
                        break;
                }
                if (INI.CHIP_CAL_MODE == 1)
                {
                    switch (borderType)
                    {
                        case BorderTypeEnum.LEFT:
                            if (m_PtfCenter.Y >= 450)
                                pointF0.X += lineClass_left.GetPtFromX(m_PtfCenter.X).Y;
                            break;
                        case BorderTypeEnum.RIGHT:
                            //if (m_PtfCenter.Y <= 3250)
                            //    pointF0.X -= lineClass_right.GetPtFromX(m_PtfCenter.X).Y;
                            pointF0.X -= 4.3f;
                            break;
                        case BorderTypeEnum.TOP:
                            if (m_PtfCenter.Y >= 1400)
                                pointF0.Y += lineClass_top.GetPtFromX(m_PtfCenter.Y).Y;
                            break;
                        case BorderTypeEnum.BOTTOM:
                            if (m_PtfCenter.Y <= 4000)
                                pointF0.Y -= lineClass_bottom.GetPtFromX(m_PtfCenter.Y).Y;
                            break;
                    }
                }

                //switch (PADCalMode)
                //{
                //    case PADCalModeEnum.BlackLast:

                //        break;
                //    default:
                //        switch (borderType)
                //        {
                //            case BorderTypeEnum.LEFT:
                //                pointF0 = jzfind.GetBoraderPoint(bitmap, true, true);
                //                pointF1 = jzfind.GetBoraderPoint(bitmap, true, true, true);
                //                break;
                //            case BorderTypeEnum.RIGHT:
                //                pointF0 = jzfind.GetBoraderPoint(bitmap, true, false);
                //                pointF1 = jzfind.GetBoraderPoint(bitmap, true, false, true);
                //                break;
                //            case BorderTypeEnum.TOP:
                //                pointF0 = jzfind.GetBoraderPoint(bitmap, false, true);
                //                pointF1 = jzfind.GetBoraderPoint(bitmap, false, true, true);
                //                break;
                //            case BorderTypeEnum.BOTTOM:
                //                pointF0 = jzfind.GetBoraderPoint(bitmap, false, false);
                //                pointF1 = jzfind.GetBoraderPoint(bitmap, false, false, true);
                //                break;
                //        }
                //        break;
                //}




                m_distance[j] = GetPointLength(pointF0, pointF1);

                //dataHistogram.Add((int)m_distance[j]);

                maxv = Math.Max(maxv, m_distance[j]);
                minv = Math.Min(minv, m_distance[j]);

                pointF0.X += rect0.X;
                pointF0.Y += rect0.Y;

                pointF1.X += rect0.X;
                pointF1.Y += rect0.Y;

                glueRegion.AddPt(pointF0);//内芯片的点
                glueRegion.AddPtIN(pointF1);//外围胶的点


                //if (m_IsSaveTemp)
                //{
                //    bitmap.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "bmpInput_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                //}

                j++;

                bitmap.Dispose();
            }

            //dataHistogram.Complete();

            //glueRegion.LengthMax = dataHistogram.MaxGrade;
            //glueRegion.LengthMin = dataHistogram.MinGrade;

            glueRegion.LengthMax = maxv;
            glueRegion.LengthMin = minv;

            bmpGlueOrg.Dispose();
        }
        private void _get_border_pointf_v8_1(Bitmap ebmpInput, Bitmap ebmpInput0, Rectangle eRect, BorderTypeEnum borderType, out GlueRegionClass glueRegion,int iSized = 2)
        {
            glueRegion = new GlueRegionClass();
            glueRegion.Reset();
            int sized = iSized;
            //int iwidth = 60;
            int iheight = 3;
            Bitmap bmpGlueOrg = new Bitmap(ebmpInput);
            Bitmap bmpGlueOrg0 = new Bitmap(ebmpInput0);
            Rectangle m_rect_org = new Rectangle(eRect.X, eRect.Y, eRect.Width, eRect.Height);
            //m_rect_org.Inflate(-iwidth, -iwidth);
            //if (m_IsSaveTemp)
            //{
            //    Bitmap bmpGlueOrg1 = new Bitmap(ebmpInput);
            //    Graphics graphics = Graphics.FromImage(bmpGlueOrg1);
            //    graphics.DrawRectangle(new Pen(Color.Red, 2), m_rect_org);
            //    graphics.Dispose();
            //    bmpGlueOrg1.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + ".png",
            //        System.Drawing.Imaging.ImageFormat.Png);
            //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
            //}

            int j = 0;
            PointF pointF0 = new PointF();
            PointF pointF1 = new PointF();
            double[] m_distance = new double[10];
            int m_samplinggap = 21;
            int iyoffset = (int)(m_samplinggap / 1.5);
            double maxv = -1000;
            double minv = 1000;
            m_samplinggap = 7;

            //int _minsize = Math.Min(eRect.Width, eRect.Height);
            //if (_minsize >= 1500)
            //    m_samplinggap = 137;
            //else
            //    m_samplinggap = 37;

            //DataHistogramClass dataHistogram = new DataHistogramClass(15000, 2);
            //dataHistogram.Reset();

            switch (borderType)
            {
                case BorderTypeEnum.LEFT:
                case BorderTypeEnum.RIGHT:
                    m_distance = new double[m_rect_org.Height / m_samplinggap];
                    break;
                case BorderTypeEnum.TOP:
                case BorderTypeEnum.BOTTOM:
                    m_distance = new double[m_rect_org.Width / m_samplinggap];
                    break;
            }

            j = 0;
            while (j < m_distance.Length)
            {
                Rectangle rect0 = new Rectangle(0, m_rect_org.Y + j * m_samplinggap, eRect.X, iheight);

                switch (borderType)
                {
                    case BorderTypeEnum.LEFT:
                        rect0 = new Rectangle(0, m_rect_org.Y + j * m_samplinggap + iyoffset, eRect.X, iheight);
                        break;
                    case BorderTypeEnum.RIGHT:
                        rect0 = new Rectangle(m_rect_org.Right, m_rect_org.Y + j * m_samplinggap + iyoffset, ebmpInput.Width - eRect.Right, iheight);
                        break;
                    case BorderTypeEnum.TOP:
                        rect0 = new Rectangle(m_rect_org.X + j * m_samplinggap + iyoffset, 0, iheight, eRect.Y);
                        break;
                    case BorderTypeEnum.BOTTOM:
                        rect0 = new Rectangle(m_rect_org.X + j * m_samplinggap + iyoffset, m_rect_org.Bottom, iheight, ebmpInput.Height - eRect.Bottom);
                        break;
                }

                Bitmap bitmap0 = bmpGlueOrg0.Clone(rect0, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                Bitmap bitmap = bmpGlueOrg.Clone(rect0, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                //AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
                //Bitmap bitmap1 = grayscale.Apply(bitmap0);

                //AForge.Imaging.Filters.OtsuThreshold otsu = new AForge.Imaging.Filters.OtsuThreshold();
                //Bitmap bitmap2 = otsu.Apply(bitmap1);

                //Bitmap bitmap3 = otsu.Apply(bitmap2);
                //bitmap1.Dispose();
                //bitmap2.Dispose();

                JzFindObjectClass jzfind = new JzFindObjectClass();

                switch (borderType)
                {
                    case BorderTypeEnum.LEFT:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, true, true);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, true, true, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, true, true, Color.FromArgb(255, 255, 255));
                        break;
                    case BorderTypeEnum.RIGHT:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, true, false);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, true, false, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, true, false, Color.FromArgb(255, 255, 255));
                        break;
                    case BorderTypeEnum.TOP:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, false, true);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, false, true, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, false, true, Color.FromArgb(255, 255, 255));
                        break;
                    case BorderTypeEnum.BOTTOM:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, false, false);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, false, false, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, false, false, Color.FromArgb(255, 255, 255));
                        break;
                }

                pointF0 = ResizeWithLocation2(pointF0, sized);
                pointF1 = ResizeWithLocation2(pointF1, sized);

                if (INI.CHIP_CAL_MODE == 1 && false)
                {
                    switch (borderType)
                    {
                        case BorderTypeEnum.LEFT:
                            if (m_PtfCenter.Y >= 450)
                                pointF0.X += lineClass_left.GetPtFromX(m_PtfCenter.X).Y;
                            break;
                        case BorderTypeEnum.RIGHT:
                            //if (m_PtfCenter.Y <= 3250)
                            //    pointF0.X -= lineClass_right.GetPtFromX(m_PtfCenter.X).Y;
                            pointF0.X -= 4.3f;
                            break;
                        case BorderTypeEnum.TOP:
                            if (m_PtfCenter.Y >= 1400)
                                pointF0.Y += lineClass_top.GetPtFromX(m_PtfCenter.Y).Y;
                            break;
                        case BorderTypeEnum.BOTTOM:
                            if (m_PtfCenter.Y <= 4000)
                                pointF0.Y -= lineClass_bottom.GetPtFromX(m_PtfCenter.Y).Y;
                            break;
                    }
                }

                //switch (PADCalMode)
                //{
                //    case PADCalModeEnum.BlackLast:

                //        break;
                //    default:
                //        switch (borderType)
                //        {
                //            case BorderTypeEnum.LEFT:
                //                pointF0 = jzfind.GetBoraderPoint(bitmap, true, true);
                //                pointF1 = jzfind.GetBoraderPoint(bitmap, true, true, true);
                //                break;
                //            case BorderTypeEnum.RIGHT:
                //                pointF0 = jzfind.GetBoraderPoint(bitmap, true, false);
                //                pointF1 = jzfind.GetBoraderPoint(bitmap, true, false, true);
                //                break;
                //            case BorderTypeEnum.TOP:
                //                pointF0 = jzfind.GetBoraderPoint(bitmap, false, true);
                //                pointF1 = jzfind.GetBoraderPoint(bitmap, false, true, true);
                //                break;
                //            case BorderTypeEnum.BOTTOM:
                //                pointF0 = jzfind.GetBoraderPoint(bitmap, false, false);
                //                pointF1 = jzfind.GetBoraderPoint(bitmap, false, false, true);
                //                break;
                //        }
                //        break;
                //}




                m_distance[j] = GetPointLength(pointF0, pointF1);

                //dataHistogram.Add((int)m_distance[j]);

                maxv = Math.Max(maxv, m_distance[j]);
                minv = Math.Min(minv, m_distance[j]);

                RectangleF rectangleFx = ResizeWithLocation2(rect0, sized);

                pointF0.X += rectangleFx.X;
                pointF0.Y += rectangleFx.Y;

                pointF1.X += rectangleFx.X;
                pointF1.Y += rectangleFx.Y;

                //pointF0.X += rect0.X;
                //pointF0.Y += rect0.Y;

                //pointF1.X += rect0.X;
                //pointF1.Y += rect0.Y;

                glueRegion.AddPt(pointF0);//内芯片的点
                glueRegion.AddPtIN(pointF1);//外围胶的点


                //if (m_IsSaveTemp)
                //{
                //    bitmap.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "bmpInput_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                //}

                j++;

                bitmap.Dispose();
            }

            //dataHistogram.Complete();

            //glueRegion.LengthMax = dataHistogram.MaxGrade;
            //glueRegion.LengthMin = dataHistogram.MinGrade;

            glueRegion.LengthMax = maxv;
            glueRegion.LengthMin = minv;

            bmpGlueOrg.Dispose();
        }

        private void _get_border_pointf(Bitmap ebmpInput, BorderTypeEnum borderType, out GlueRegionClass glueRegion)
        {
            glueRegion = new GlueRegionClass();
            glueRegion.Reset();

            int iwidth = 60;
            int iheight = 3;
            Bitmap bmpGlueOrg = new Bitmap(ebmpInput);
            Rectangle m_rect_org = SimpleRect(bmpGlueOrg.Size);
            m_rect_org.Inflate(-iwidth, -iwidth);

            int j = 0;
            PointF pointF0 = new PointF();
            PointF pointF1 = new PointF();
            double[] m_distance = new double[10];
            int m_samplinggap = 10;

            DataHistogramClass dataHistogram = new DataHistogramClass(15000, 2);
            dataHistogram.Reset();

            switch (borderType)
            {
                case BorderTypeEnum.LEFT:
                case BorderTypeEnum.RIGHT:
                    m_distance = new double[m_rect_org.Height / m_samplinggap];
                    break;
                case BorderTypeEnum.TOP:
                case BorderTypeEnum.BOTTOM:
                    m_distance = new double[m_rect_org.Width / m_samplinggap];
                    break;
            }

            j = 0;
            while (j < m_distance.Length)
            {
                Rectangle rect0 = new Rectangle(0, m_rect_org.Y + j * m_samplinggap, iwidth, iheight);

                switch (borderType)
                {
                    case BorderTypeEnum.LEFT:
                        rect0 = new Rectangle(0, m_rect_org.Y + j * m_samplinggap, iwidth, iheight);
                        break;
                    case BorderTypeEnum.RIGHT:
                        rect0 = new Rectangle(m_rect_org.Right, m_rect_org.Y + j * m_samplinggap, iwidth, iheight);
                        break;
                    case BorderTypeEnum.TOP:
                        rect0 = new Rectangle(m_rect_org.X + j * m_samplinggap, 0, iheight, iwidth);
                        break;
                    case BorderTypeEnum.BOTTOM:
                        rect0 = new Rectangle(m_rect_org.X + j * m_samplinggap, m_rect_org.Bottom, iheight, iwidth);
                        break;
                }

                Bitmap bitmap = bmpGlueOrg.Clone(rect0, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
                Bitmap bitmap1 = grayscale.Apply(bitmap);
                AForge.Imaging.Filters.HistogramEqualization histogramEqualization = new AForge.Imaging.Filters.HistogramEqualization();
                Bitmap bitmap2 = histogramEqualization.Apply(bitmap1);
                bitmap1.Dispose();
                AForge.Imaging.Filters.BradleyLocalThresholding bradleyLocalThresholding = new AForge.Imaging.Filters.BradleyLocalThresholding();
                Bitmap bitmap3 = bradleyLocalThresholding.Apply(bitmap2);
                bitmap2.Dispose();

                bitmap.Dispose();
                bitmap = new Bitmap(bitmap3);
                bitmap3.Dispose();


                JzFindObjectClass jzfind = new JzFindObjectClass();
                //HistogramClass histogram = new HistogramClass(2);
                //histogram.GetHistogram(bitmap);
                //jzfind.SetThreshold(bitmap, SimpleRect(bitmap.Size), (int)((histogram.MaxGrade - histogram.MinGrade) * 0.22), 255, 0, true);


                switch (borderType)
                {
                    case BorderTypeEnum.LEFT:
                        pointF0 = jzfind.GetBoraderPoint(bitmap, true, true);
                        pointF1 = jzfind.GetBoraderPoint(bitmap, true, true, true);
                        break;
                    case BorderTypeEnum.RIGHT:
                        pointF0 = jzfind.GetBoraderPoint(bitmap, true, false);
                        pointF1 = jzfind.GetBoraderPoint(bitmap, true, false, true);
                        break;
                    case BorderTypeEnum.TOP:
                        pointF0 = jzfind.GetBoraderPoint(bitmap, false, true);
                        pointF1 = jzfind.GetBoraderPoint(bitmap, false, true, true);
                        break;
                    case BorderTypeEnum.BOTTOM:
                        pointF0 = jzfind.GetBoraderPoint(bitmap, false, false);
                        pointF1 = jzfind.GetBoraderPoint(bitmap, false, false, true);
                        break;
                }

                m_distance[j] = GetPointLength(pointF0, pointF1);

                dataHistogram.Add((int)m_distance[j]);

                pointF0.X += rect0.X;
                pointF0.Y += rect0.Y;

                pointF1.X += rect0.X;
                pointF1.Y += rect0.Y;

                glueRegion.AddPt(pointF0);
                glueRegion.AddPtIN(pointF1);


                if (RelateAnalyzeString == "A00-02-0002")
                {
                    bitmap.Save("D:\\testtest\\x\\" + RelateAnalyzeString + "bmpInput_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                }

                j++;

                bitmap.Dispose();
            }

            dataHistogram.Complete();

            glueRegion.LengthMax = dataHistogram.MaxGrade;
            glueRegion.LengthMin = dataHistogram.MinGrade;

            bmpGlueOrg.Dispose();
        }
        public class PADRegionClass
        {
            public double RegionWidth { get; set; } = 10;
            public double RegionHeight { get; set; } = 10;
            public double RegionArea { get; set; } = 100;
            /// <summary>
            /// 内缩的角点位置
            /// </summary>
            public PointF[] RegionPtFCorner = new PointF[4];
            /// <summary>
            /// 原始的角点位置
            /// </summary>
            public PointF[] RegionPtFCornerORG = new PointF[4];
            public Rectangle RegionForEdgeRect = new Rectangle(0, 0, 1, 1);
            public Bitmap bmpThreshold = new Bitmap(1, 1);
            public JRotatedRectangleF jRotatedRectangleF2 = new JRotatedRectangleF();
            /// <summary>
            /// 生成给有无胶使用
            /// </summary>
            public Bitmap bmpChipForWetherGlue = new Bitmap(1, 1);
            public bool Isleftoverright { get; set; } = false;
            public List<PointF[]> listPointF = new List<PointF[]>();

            public Bitmap GetChipGlue(int fSized, out PointF oCenterPointF)
            {
                Size size = new Size(bmpChipForWetherGlue.Width, bmpChipForWetherGlue.Height);
                if (fSized > 0)
                {
                    oCenterPointF = new PointF((float)jRotatedRectangleF2.fCX * fSized,
                                                                   (float)jRotatedRectangleF2.fCY * fSized);
                    size = new Size(bmpChipForWetherGlue.Width * fSized, bmpChipForWetherGlue.Height * fSized);
                }
                else
                {
                    oCenterPointF = new PointF((float)jRotatedRectangleF2.fCX / -fSized,
                                                                   (float)jRotatedRectangleF2.fCY / -fSized);
                    size = new Size(bmpChipForWetherGlue.Width / -fSized, bmpChipForWetherGlue.Height / -fSized);
                }
                Bitmap bmptemp = new Bitmap(bmpChipForWetherGlue, size);
                return bmptemp;
            }

            public void Reset()
            {
                listPointF.Clear();
                Isleftoverright = false;

                jRotatedRectangleF2.fCX = 0;
                jRotatedRectangleF2.fCY = 0;
                jRotatedRectangleF2.fAngle = 0;
                jRotatedRectangleF2.fWidth = 100;
                jRotatedRectangleF2.fHeight = 100;

                RegionWidth = 10;
                RegionHeight = 10;
                RegionArea = 100;

                RegionPtFCorner[0] = new PointF(0, 10);
                RegionPtFCorner[1] = new PointF(10, 10);
                RegionPtFCorner[2] = new PointF(10, 0);
                RegionPtFCorner[3] = new PointF(0, 0);

                RegionPtFCornerORG[0] = new PointF(0, 10);
                RegionPtFCornerORG[1] = new PointF(10, 10);
                RegionPtFCornerORG[2] = new PointF(10, 0);
                RegionPtFCornerORG[3] = new PointF(0, 0);
            }
            public void SetPointF(PointF[] eInputPtF)
            {
                if (eInputPtF.Length == 4)
                {
                    RegionPtFCorner[0] = eInputPtF[0];
                    RegionPtFCorner[1] = eInputPtF[1];
                    RegionPtFCorner[2] = eInputPtF[2];
                    RegionPtFCorner[3] = eInputPtF[3];
                }
            }
            public void SetPointFORG(PointF[] eInputPtF, double eAngle = 0)
            {
                if (eInputPtF.Length == 4)
                {
                    RegionPtFCornerORG[0] = eInputPtF[0];
                    RegionPtFCornerORG[1] = eInputPtF[1];
                    RegionPtFCornerORG[2] = eInputPtF[2];
                    RegionPtFCornerORG[3] = eInputPtF[3];

                    jRotatedRectangleF2.fCX = (eInputPtF[0].X + eInputPtF[2].X) / 2;
                    jRotatedRectangleF2.fCY = (eInputPtF[0].Y + eInputPtF[2].Y) / 2;
                    jRotatedRectangleF2.fAngle = eAngle;
                    double iwidth = GetPointLength(eInputPtF[0], eInputPtF[1]);
                    double iheight = GetPointLength(eInputPtF[1], eInputPtF[2]);
                    jRotatedRectangleF2.fWidth = iwidth;
                    jRotatedRectangleF2.fHeight = iheight;
                    if (iwidth < iheight)
                    {
                        jRotatedRectangleF2.fWidth = iheight;
                        jRotatedRectangleF2.fHeight = iwidth;
                    }

                    RegionWidth = jRotatedRectangleF2.fWidth;
                    RegionHeight = jRotatedRectangleF2.fHeight;
                }
            }
            //public 
            /// <summary>
            /// 取得旋转矩形框的上边缘中点坐标 并且可以偏移一个位置 主要用于魔棒工具抓取胶水
            /// </summary>
            /// <param name="offsetx">x方向偏移</param>
            /// <param name="offsety">y方向偏移</param>
            /// <returns>返回正确的点</returns>
            public PointF GetTopEdgeCenter(int iSized = 1, float offsetx = 0, float offsety = 0)
            {
                PointF pointF = new PointF();
                List<string> checkListStr1 = new List<string>();
                checkListStr1.Clear();
                //找到Y坐标的最上和最下
                int j = 0;
                foreach (PointF ptf in RegionPtFCornerORG)
                {
                    checkListStr1.Add($"{ptf.Y.ToString()},{j.ToString()}");
                    j++;
                }
                checkListStr1.Sort((item1, item2) =>
                { return float.Parse(item1.Split(',')[0]) > float.Parse(item2.Split(',')[0]) ? 1 : -1; });
                //checkListStr1.Sort();
                //float itopy = float.Parse(checkListStr1[0].Split(',')[0]);
                //float ibottomy = float.Parse(checkListStr1[checkListStr1.Count - 1].Split(',')[0]);

                PointF p1 = RegionPtFCornerORG[int.Parse(checkListStr1[0].Split(',')[1])];
                PointF p2 = RegionPtFCornerORG[int.Parse(checkListStr1[1].Split(',')[1])];
                pointF.X = (p1.X + p2.X) / 2 / iSized - offsetx;
                pointF.Y = (p1.Y + p2.Y) / 2 / iSized - offsety;
                //if (Isleftoverright)
                //{
                //    pointF.X = (RegionPtFCornerORG[0].X + RegionPtFCornerORG[3].X) / 2 / iSized - offsetx;
                //    pointF.Y = (RegionPtFCornerORG[0].Y + RegionPtFCornerORG[3].Y) / 2 / iSized - offsety;
                //}
                //else
                //{
                //    pointF.X = (RegionPtFCornerORG[0].X + RegionPtFCornerORG[3].X) / 2 / iSized + offsetx;
                //    pointF.Y = (RegionPtFCornerORG[0].Y + RegionPtFCornerORG[3].Y) / 2 / iSized + offsety;
                //}
                return pointF;
            }
            double GetPointLength(PointF P1, PointF P2)
            {
                return Math.Sqrt((double)Math.Pow((P1.X - P2.X), 2) + Math.Pow((P1.Y - P2.Y), 2));
            }
        }

        public class GlueRegionClass
        {
            public double LengthMax { get; set; } = 0;
            public double LengthMin { get; set; } = 0;
            private PointF[] LengthPointFs = new PointF[4];
            private PointF[] LengthPointFsIN = new PointF[4];
            private List<PointF> LengthPointFsList = new List<PointF>();
            private List<PointF> LengthPointFsListIN = new List<PointF>();
            public void Reset()
            {
                LengthMax = 0;
                LengthMin = 0;

                LengthPointFs[0] = new PointF(0, 10);
                LengthPointFs[1] = new PointF(10, 10);
                LengthPointFs[2] = new PointF(10, 0);
                LengthPointFs[3] = new PointF(0, 0);

                LengthPointFsIN[0] = new PointF(0, 10);
                LengthPointFsIN[1] = new PointF(10, 10);
                LengthPointFsIN[2] = new PointF(10, 0);
                LengthPointFsIN[3] = new PointF(0, 0);

                LengthPointFsList.Clear();
                LengthPointFsListIN.Clear();
            }
            public void AddPt(PointF ept)
            {
                LengthPointFsList.Add(ept);
            }
            public PointF[] GetPointF()
            {
                if (LengthPointFsList.Count >= 4)
                {
                    LengthPointFs = new PointF[LengthPointFsList.Count];
                    int i = 0;
                    foreach (PointF p in LengthPointFsList)
                    {
                        LengthPointFs[i] = p;
                        i++;
                    }
                }
                else
                {
                    LengthPointFs = new PointF[4];
                    LengthPointFs[0] = new PointF(0, 10);
                    LengthPointFs[1] = new PointF(10, 10);
                    LengthPointFs[2] = new PointF(10, 0);
                    LengthPointFs[3] = new PointF(0, 0);
                }

                return LengthPointFs;

            }

            public void AddPtIN(PointF ept)
            {
                LengthPointFsListIN.Add(ept);
            }
            public PointF[] GetPointFIN()
            {
                if (LengthPointFsListIN.Count >= 4)
                {
                    LengthPointFsIN = new PointF[LengthPointFsListIN.Count];
                    int i = 0;
                    foreach (PointF p in LengthPointFsListIN)
                    {
                        LengthPointFsIN[i] = p;
                        i++;
                    }
                }
                else
                {
                    LengthPointFsIN = new PointF[4];
                    LengthPointFsIN[0] = new PointF(0, 10);
                    LengthPointFsIN[1] = new PointF(10, 10);
                    LengthPointFsIN[2] = new PointF(10, 0);
                    LengthPointFsIN[3] = new PointF(0, 0);
                }

                return LengthPointFsIN;

            }

            public double GetMinMM()
            {
                double a = 0.0254 / INI.MAINSD_PAD_MIL_RESOLUTION;
                return LengthMin * INI.MAINSD_PAD_MIL_RESOLUTION;// a * 0.0254001;
            }
            public double GetMaxMM()
            {
                double a = 0.0254 / INI.MAINSD_PAD_MIL_RESOLUTION;
                return LengthMax * INI.MAINSD_PAD_MIL_RESOLUTION;//  a * 0.0254001;
            }

        }
        public class BorderLineRunClass
        {
            public int index;
            public Bitmap bmp0;
            public Bitmap bmp1;
            public Rectangle rect0;
            public BorderTypeEnum Border;

        }


        public PADINSPECTClass()
        {

        }
        public PADINSPECTClass(string str)
        {
            FromString(str);
        }
        public override string ToString()
        {
            string str = "";

            str += ((int)PADMethod).ToString() + Universal.SeperateCharB;     //0
            str += OWidthRatio.ToString(m_format) + Universal.SeperateCharB;
            str += OHeightRatio.ToString(m_format) + Universal.SeperateCharB;
            str += OAreaRatio.ToString(m_format) + Universal.SeperateCharB;
            str += PADGrayThreshold.ToString() + Universal.SeperateCharB;
            str += PADBlobGrayThreshold.ToString() + Universal.SeperateCharB;
            str += CheckDArea.ToString(m_format) + Universal.SeperateCharB;
            str += CheckDWidth.ToString(m_format) + Universal.SeperateCharB;
            str += CheckDHeight.ToString(m_format) + Universal.SeperateCharB;
            str += ExtendX.ToString() + Universal.SeperateCharB;
            str += ExtendY.ToString() + Universal.SeperateCharB;
            str += GlueMax.ToString() + Universal.SeperateCharB;
            str += GlueMin.ToString() + Universal.SeperateCharB;
            str += (GlueCheck ? "1" : "0") + Universal.SeperateCharB;
            str += ((int)PADThresholdMode).ToString() + Universal.SeperateCharB;     //0
            str += (NoGlueThresholdValue).ToString() + Universal.SeperateCharB;     //0
            str += ((int)PADCalMode).ToString() + Universal.SeperateCharB;     //0
            str += ((int)PADChipSizeMode).ToString() + Universal.SeperateCharB;     //0
            str += CalExtendX.ToString() + Universal.SeperateCharB;
            str += CalExtendY.ToString() + Universal.SeperateCharB;
            str += (BloodFillValueRatio).ToString() + Universal.SeperateCharB;     //0

            str += GlueMaxTop.ToString() + Universal.SeperateCharB;
            str += GlueMinTop.ToString() + Universal.SeperateCharB;
            str += GlueMaxBottom.ToString() + Universal.SeperateCharB;
            str += GlueMinBottom.ToString() + Universal.SeperateCharB;
            str += GlueMaxLeft.ToString() + Universal.SeperateCharB;
            str += GlueMinLeft.ToString() + Universal.SeperateCharB;
            str += GlueMaxRight.ToString() + Universal.SeperateCharB;
            str += GlueMinRight.ToString() + Universal.SeperateCharB;

            str += BlackCalExtendX.ToString() + Universal.SeperateCharB;
            str += BlackCalExtendY.ToString() + Universal.SeperateCharB;
            str += BlackOffsetX.ToString() + Universal.SeperateCharB;
            str += BlackOffsetY.ToString() + Universal.SeperateCharB;

            str += "";

            return str;
        }
        public void FromString(string str)
        {
            string[] strs = str.Split(Universal.SeperateCharB);
            if (strs.Length > 1)
            {
                PADMethod = (PADMethodEnum)int.Parse(strs[0]);
                OWidthRatio = double.Parse(strs[1]);
                OHeightRatio = double.Parse(strs[2]);
                OAreaRatio = double.Parse(strs[3]);
                PADGrayThreshold = int.Parse(strs[4]);
                PADBlobGrayThreshold = int.Parse(strs[5]);
                CheckDArea = double.Parse(strs[6]);

                if (strs.Length > 8)
                {
                    CheckDWidth = double.Parse(strs[7]);
                    CheckDHeight = double.Parse(strs[8]);
                }
                if (strs.Length > 10)
                {
                    ExtendX = double.Parse(strs[9]);
                    ExtendY = double.Parse(strs[10]);
                }
                if (strs.Length > 13)
                {
                    GlueMax = double.Parse(strs[11]);
                    GlueMin = double.Parse(strs[12]);
                    GlueCheck = strs[13] == "1";
                }
                if (strs.Length > 14)
                {
                    if (string.IsNullOrEmpty(strs[14]))
                        strs[14] = "0";
                    PADThresholdMode = (PADThresholdEnum)int.Parse(strs[14]);
                }
                if (strs.Length > 15)
                {
                    if (string.IsNullOrEmpty(strs[15]))
                        strs[15] = "0.75";
                    NoGlueThresholdValue = double.Parse(strs[15]);
                }
                if (strs.Length > 16)
                {
                    if (string.IsNullOrEmpty(strs[16]))
                        strs[16] = "0";
                    PADCalMode = (PADCalModeEnum)int.Parse(strs[16]);
                }
                if (strs.Length > 17)
                {
                    if (string.IsNullOrEmpty(strs[17]))
                        strs[17] = "0";
                    PADChipSizeMode = (PADChipSize)int.Parse(strs[17]);
                }
                if (strs.Length > 19)
                {
                    CalExtendX = double.Parse(strs[18]);
                    CalExtendY = double.Parse(strs[19]);
                }
                if (strs.Length > 20)
                {
                    if (string.IsNullOrEmpty(strs[20]))
                        strs[20] = "0.33";
                    BloodFillValueRatio = double.Parse(strs[20]);
                }
                if (strs.Length > 28)
                {
                    GlueMaxTop = double.Parse(strs[21]);
                    GlueMinTop = double.Parse(strs[22]);
                    GlueMaxBottom = double.Parse(strs[23]);
                    GlueMinBottom = double.Parse(strs[24]);
                    GlueMaxLeft = double.Parse(strs[25]);
                    GlueMinLeft = double.Parse(strs[26]);
                    GlueMaxRight = double.Parse(strs[27]);
                    GlueMinRight = double.Parse(strs[28]);
                }
                if (strs.Length > 32)
                {
                    BlackCalExtendX = double.Parse(strs[29]);
                    BlackCalExtendY = double.Parse(strs[30]);
                    BlackOffsetX = double.Parse(strs[31]);
                    BlackOffsetY = double.Parse(strs[32]);
                }
            }
        }
        public void Reset()
        {
            PADMethod = PADMethodEnum.NONE;
            OWidthRatio = 15;
            OHeightRatio = 15;
            OAreaRatio = 15;
            PADGrayThreshold = 180;
            PADBlobGrayThreshold = 30;
            CheckDArea = 15;
            CheckDWidth = 15;
            CheckDHeight = 15;
            ExtendX = 5;
            ExtendY = 5;
            GlueMax = 120;
            GlueMin = 6;
            GlueCheck = true;
            PADThresholdMode = PADThresholdEnum.Threshold;
            NoGlueThresholdValue = 0.7;
            PADCalMode = PADCalModeEnum.BlacktoBlack;
            PADChipSizeMode = PADChipSize.CHIP_NORMAL;
            CalExtendX = 66;
            CalExtendY = 66;
            BloodFillValueRatio = 0.33;

            GlueMaxTop = 0.6;
            GlueMinTop = 0.1;

            GlueMaxBottom = 0.6;
            GlueMinBottom = 0.1;

            GlueMaxLeft = 1.2;
            GlueMinLeft = 0.1;

            GlueMaxRight = 0.9;
            GlueMinRight = 0.1;

            BlackCalExtendX = 40;
            BlackCalExtendY = 40;

            BlackOffsetX = 0;
            BlackOffsetY = 0;

        }
        public void FromPropertyChange(string changeitemstring, string valuestring)
        {
            string[] str = changeitemstring.Split(';');

            if (str[0] != "10.PADCHECK")
                return;

            switch (str[1])
            {
                case "PADMethod":
                    PADMethod = (PADMethodEnum)Enum.Parse(typeof(PADMethodEnum), valuestring, true);
                    break;
                case "PADThresholdMode":
                    PADThresholdMode = (PADThresholdEnum)Enum.Parse(typeof(PADThresholdEnum), valuestring, true);
                    break;
                case "PADOWidthRatio":
                    OWidthRatio = double.Parse(valuestring);
                    break;
                case "PADOHeightRatio":
                    OHeightRatio = double.Parse(valuestring);
                    break;
                case "PADOAreaRatio":
                    OAreaRatio = double.Parse(valuestring);
                    break;
                case "PADGrayThreshold":
                    PADGrayThreshold = int.Parse(valuestring);
                    break;
                case "PADBlobGrayThreshold":
                    PADBlobGrayThreshold = int.Parse(valuestring);
                    break;
                case "PADCheckDArea":
                    CheckDArea = double.Parse(valuestring);
                    break;
                case "PADCheckDWidth":
                    CheckDWidth = double.Parse(valuestring);
                    break;
                case "PADCheckDHeight":
                    CheckDHeight = double.Parse(valuestring);
                    break;
                case "PADExtendX":
                    ExtendX = double.Parse(valuestring);
                    break;
                case "PADExtendY":
                    ExtendY = double.Parse(valuestring);
                    break;

                case "GlueMax":
                    GlueMax = double.Parse(valuestring);
                    break;
                case "GlueMin":
                    GlueMin = double.Parse(valuestring);
                    break;
                case "GlueCheck":
                    GlueCheck = bool.Parse(valuestring);
                    break;
                case "NoGlueThresholdValue":
                    NoGlueThresholdValue = double.Parse(valuestring);
                    break;
                case "PADCalMode":
                    PADCalMode = (PADCalModeEnum)Enum.Parse(typeof(PADCalModeEnum), valuestring, true);
                    break;
                case "PADChipSizeMode":
                    PADChipSizeMode = (PADChipSize)Enum.Parse(typeof(PADChipSize), valuestring, true);
                    break;
                case "CalExtendX":
                    CalExtendX = double.Parse(valuestring);
                    break;
                case "CalExtendY":
                    CalExtendY = double.Parse(valuestring);
                    break;
                case "BloodFillValueRatio":
                    BloodFillValueRatio = double.Parse(valuestring);
                    break;

                case "GlueMaxTop":
                    GlueMaxTop = double.Parse(valuestring);
                    break;
                case "GlueMinTop":
                    GlueMinTop = double.Parse(valuestring);
                    break;
                case "GlueMaxBottom":
                    GlueMaxBottom = double.Parse(valuestring);
                    break;
                case "GlueMinBottom":
                    GlueMinBottom = double.Parse(valuestring);
                    break;
                case "GlueMaxLeft":
                    GlueMaxLeft = double.Parse(valuestring);
                    break;
                case "GlueMinLeft":
                    GlueMinLeft = double.Parse(valuestring);
                    break;
                case "GlueMaxRight":
                    GlueMaxRight = double.Parse(valuestring);
                    break;
                case "GlueMinRight":
                    GlueMinRight = double.Parse(valuestring);
                    break;

                case "BlackCalExtendX":
                    BlackCalExtendX = double.Parse(valuestring);
                    break;
                case "BlackCalExtendY":
                    BlackCalExtendY = double.Parse(valuestring);
                    break;
                case "BlackOffsetX":
                    BlackOffsetX = double.Parse(valuestring);
                    break;
                case "BlackOffsetY":
                    BlackOffsetY = double.Parse(valuestring);
                    break;
            }
        }

        public void Suicide()
        {
            if (PADMethod == PADMethodEnum.PADCHECK
                || PADMethod == PADMethodEnum.GLUECHECK
                || PADMethod == PADMethodEnum.GLUECHECK_BlackEdge
                || PADMethod == PADMethodEnum.PLACODE_CHECK)
            {
                TrainStatusCollection.Clear();
                RunStatusCollection.Clear();
            }
        }
        /// <summary>
        /// 在做大量運算前要清除的相關資料
        /// </summary>
        public void ResetRunStatus()
        {
            RunStatusCollection.Clear();
        }
        /// <summary>
        /// 將產生出來的過程寫出去
        /// </summary>
        /// <param name="processstringlist"></param>
        /// <param name="runstatuslist"></param>
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
        public void FillTrainStatus(WorkStatusCollectionClass trainstatuscollection, string filltoanalyzestr)
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

        public string ToChipReportingStr()
        {
            string str = string.Empty;
            if (glues == null)
                return str;

            int i = 0;
            while (i < (int)BorderTypeEnum.COUNT)
            {
                double min = glues[i].LengthMin * Resolution_Mil;
                double max = glues[i].LengthMax * Resolution_Mil;

                str += min.ToString("0.00") + "," + max.ToString("0.00") + ",";

                i++;
            }

            return str;
        }
        public string ToChipReportingStr2()
        {
            string str = string.Empty;
            if (glues == null)
                return str;
            if (!m_RunDataOK)
                return str;

            int i = 0;
            while (i < (int)BorderTypeEnum.COUNT)
            {
                if (glues[i] == null)
                {
                    str += 0.ToString("0.00") + "," + 0.ToString("0.00") + ",";
                    continue;
                }
                double min = glues[i].LengthMin * Resolution_Mil;
                double max = glues[i].LengthMax * Resolution_Mil;

                str += min.ToString("0.00") + "," + max.ToString("0.00") + ",";

                i++;
            }
            str += ",";
            i = 0;
            while (i < (int)BorderTypeEnum.COUNT)
            {
                if (glues[i] == null)
                {
                    str += 0.ToString("0.00") + "," + 0.ToString("0.00") + ",";
                    continue;
                }
                double min = glues[i].LengthMin / Resolution_Mil * 0.0254;
                double max = glues[i].LengthMax / Resolution_Mil * 0.0254;
                //double min = glues[i].LengthMin * Resolution_Mil;
                //double max = glues[i].LengthMax * Resolution_Mil;

                str += min.ToString("0.00") + "," + max.ToString("0.00") + ",";

                i++;
            }

            return str;
        }
        public string ToChipReportingStr3()
        {
            string str = string.Empty;
            if (glues == null)
                return str;
            if (!m_RunDataOK)
                return str;

            int i = 0;

            i = 0;
            while (i < (int)BorderTypeEnum.COUNT)
            {
                if (glues[i] == null)
                {
                    str += 0.ToString("0.00") + "," + 0.ToString("0.00") + ",";
                    continue;
                }
                double min = glues[i].LengthMin / Resolution_Mil * 0.0254;
                double max = glues[i].LengthMax / Resolution_Mil * 0.0254;
                //double min = glues[i].LengthMin * Resolution_Mil;
                //double max = glues[i].LengthMax * Resolution_Mil;

                str += min.ToString("0.00") + "," + max.ToString("0.00") + ",";

                i++;
            }

            return str;
        }

        private string _CalPageIndex()
        {
            string ret = string.Empty;
            switch (Universal.OPTION)
            {
                case OptionEnum.MAIN_SDM2:
                case OptionEnum.MAIN_SDM3:
                    ret = Universal.CalPageIndex.ToString("00000") + "_";
                    break;
            }
            return ret;

            //if (Universal.OPTION != OptionEnum.MAIN_SDM2 || Universal.OPTION != OptionEnum.MAIN_SDM3)
            //    return "";
            //return Universal.CalPageIndex.ToString("00000") + "_";
        }

        #region TOOLS

        private Bitmap _getV1bmpInput(Rectangle rect1)
        {
            Graphics gPrepared = Graphics.FromImage(imgmask);
            gPrepared.Clear(Color.White);
            //gPrepared.FillPolygon(Brushes.Black, PADTempRegion.RegionPtFCornerORG);
            gPrepared.FillRectangle(Brushes.Black, rect1);
            gPrepared.Dispose();

            HistogramClass histogramClass = new HistogramClass(2);
            histogramClass.GetHistogram(imginput);

            //Bitmap bmpfloodfill = FloodFill(bmphistogramEqualization11, new Point(bmphistogramEqualization11.Width / 2, bmphistogramEqualization11.Height / 2),
            //                                                Color.White, (int)(255 * NoGlueThresholdValue));

            switch (PADChipSizeMode)
            {
                case PADChipSize.CHIP_V1:
                    //填充中心的位置
                    int fillthresholdvalue = (int)(255 * (1 - NoGlueThresholdValue));
                    fillthresholdvalue = histogramClass.MinGrade;
                    m_JzFind.GetMaskedImage(imginput, imgmask, Color.White, Color.FromArgb(fillthresholdvalue, fillthresholdvalue, fillthresholdvalue), false);
                    break;
            }

            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            Bitmap bitmap1 = grayscale.Apply(imginput);
            if (m_IsSaveTemp)
            {
                imginput.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "imginput" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
            }

            //默认处理成大芯片
            AForge.Imaging.Filters.HistogramEqualization histogramEqualization = new AForge.Imaging.Filters.HistogramEqualization();
            Bitmap bitmap5 = histogramEqualization.Apply(bitmap1);

            switch (PADChipSizeMode)
            {
                case PADChipSize.CHIP_V1:
                    AForge.Imaging.Filters.ContrastStretch contrastStretch = new AForge.Imaging.Filters.ContrastStretch();
                    bitmap5 = contrastStretch.Apply(bitmap1);
                    break;
            }
            bitmap1.Dispose();

            AForge.Imaging.Filters.SISThreshold sISThreshold = new AForge.Imaging.Filters.SISThreshold();
            Bitmap bitmap6 = sISThreshold.Apply(bitmap5);
            //if (m_IsSaveTemp)
            //{
            //    bitmap6.Save("D:\\testtest\\" + RelateAnalyzeString + "bitmap6" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
            //}
            bitmap5.Dispose();

            m_JzFind.GetMaskedImage(bitmap6, imgmask, Color.White, Color.White, false);
            return bitmap6;
        }
        //private Bitmap _getV2bmpInput(Rectangle rect1)
        //{
        //    Graphics gPrepared = Graphics.FromImage(imgmask);
        //    gPrepared.Clear(Color.White);
        //    //gPrepared.FillPolygon(Brushes.Black, PADTempRegion.RegionPtFCornerORG);
        //    gPrepared.FillRectangle(Brushes.Black, rect1);
        //    gPrepared.Dispose();

        //    Point point = new Point((rect1.Right - rect1.X) / 2, rect1.Y - 5);

        //    HistogramClass histogramClass = new HistogramClass(2);
        //    histogramClass.GetHistogram(imginput);
        //    Color color = imginput.GetPixel(point.X, point.Y);
        //    int gray = (int)(color.R * 0.299 + color.G * 0.587 + color.B * 0.114);
        //    //int max=
        //    //Bitmap bmpfloodfill = FloodFill2(imginput, point, Color.FromArgb(0, 255, 0), (int)(gray * 0.45));
        //    Bitmap bmpfloodfill = new Bitmap(imginput);
        //    JzFindObjectClass jzFindObjectClass = new JzFindObjectClass();
        //    jzFindObjectClass.FindGrayscale(bmpfloodfill, point, Color.FromArgb(0, 255, 0), (int)(histogramClass.MinGrade * 0.55));
        //    if (bmpfloodfill == null)
        //        bmpfloodfill = imginput;
        //    if (m_IsSaveTemp)
        //    {
        //        bmpfloodfill.Save("D:\\testtest\\" + RelateAnalyzeString + "bmpfloodfill" + ".png", System.Drawing.Imaging.ImageFormat.Png);
        //        //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
        //    }

        //    //switch (PADChipSizeMode)
        //    //{
        //    //    case PADChipSize.CHIP_SMALL:
        //    //        //填充中心的位置
        //    //        int fillthresholdvalue = (int)(255 * (1 - NoGlueThresholdValue));
        //    //        fillthresholdvalue = histogramClass.MinGrade;
        //    //        m_JzFind.GetMaskedImage(imginput, imgmask, Color.White, Color.FromArgb(fillthresholdvalue, fillthresholdvalue, fillthresholdvalue), false);
        //    //        break;
        //    //}

        //    //AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
        //    //Bitmap bitmap1 = grayscale.Apply(imginput);
        //    //if (m_IsSaveTemp)
        //    //{
        //    //    imginput.Save("D:\\testtest\\" + RelateAnalyzeString + "imginput" + ".png", System.Drawing.Imaging.ImageFormat.Png);
        //    //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
        //    //}

        //    ////默认处理成大芯片
        //    //AForge.Imaging.Filters.HistogramEqualization histogramEqualization = new AForge.Imaging.Filters.HistogramEqualization();
        //    //Bitmap bitmap5 = histogramEqualization.Apply(bitmap1);

        //    //switch (PADChipSizeMode)
        //    //{
        //    //    case PADChipSize.CHIP_SMALL:
        //    //        AForge.Imaging.Filters.ContrastStretch contrastStretch = new AForge.Imaging.Filters.ContrastStretch();
        //    //        bitmap5 = contrastStretch.Apply(bitmap1);
        //    //        break;
        //    //}
        //    //bitmap1.Dispose();

        //    //AForge.Imaging.Filters.SISThreshold sISThreshold = new AForge.Imaging.Filters.SISThreshold();
        //    //Bitmap bitmap6 = sISThreshold.Apply(bitmap5);
        //    ////if (m_IsSaveTemp)
        //    ////{
        //    ////    bitmap6.Save("D:\\testtest\\" + RelateAnalyzeString + "bitmap6" + ".png", System.Drawing.Imaging.ImageFormat.Png);
        //    ////    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
        //    ////}
        //    //bitmap5.Dispose();

        //    //m_JzFind.GetMaskedImage(bitmap6, imgmask, Color.White, Color.White, false);
        //    //return bitmap6;
        //    return bmpfloodfill;
        //}
        //private Bitmap _getV2_1bmpInput(Rectangle rect1)
        //{
        //    Graphics gPrepared = Graphics.FromImage(imgmask);
        //    gPrepared.Clear(Color.White);
        //    //gPrepared.FillPolygon(Brushes.Black, PADTempRegion.RegionPtFCornerORG);
        //    gPrepared.FillRectangle(Brushes.Black, rect1);
        //    gPrepared.Dispose();

        //    HistogramClass histogramClass = new HistogramClass(2);
        //    histogramClass.GetHistogram(imginput);

        //    //Bitmap bmpfloodfill = FloodFill(bmphistogramEqualization11, new Point(bmphistogramEqualization11.Width / 2, bmphistogramEqualization11.Height / 2),
        //    //                                                Color.White, (int)(255 * NoGlueThresholdValue));

        //    //填充中心的位置
        //    int fillthresholdvalue = (int)(255 * (1 - NoGlueThresholdValue));
        //    fillthresholdvalue = histogramClass.MinGrade;
        //    m_JzFind.GetMaskedImage(imginput, imgmask, Color.White, Color.FromArgb(fillthresholdvalue, fillthresholdvalue, fillthresholdvalue), false);


        //    //switch (PADChipSizeMode)
        //    //{
        //    //    case PADChipSize.CHIP_SMALL:
        //    //        break;
        //    //}

        //    AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
        //    Bitmap bitmap1 = grayscale.Apply(imginput);
        //    if (m_IsSaveTemp)
        //    {
        //        imginput.Save("D:\\testtest\\" + RelateAnalyzeString + "imginput" + ".png", System.Drawing.Imaging.ImageFormat.Png);
        //        //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
        //    }

        //    //默认处理成大芯片
        //    AForge.Imaging.Filters.HistogramEqualization histogramEqualization = new AForge.Imaging.Filters.HistogramEqualization();
        //    Bitmap bitmap5 = histogramEqualization.Apply(bitmap1);

        //    AForge.Imaging.Filters.ContrastStretch contrastStretch = new AForge.Imaging.Filters.ContrastStretch();
        //    bitmap5 = contrastStretch.Apply(bitmap1);
        //    //switch (PADChipSizeMode)
        //    //{
        //    //    case PADChipSize.CHIP_SMALL:

        //    //        break;
        //    //}
        //    bitmap1.Dispose();

        //    AForge.Imaging.Filters.SISThreshold sISThreshold = new AForge.Imaging.Filters.SISThreshold();
        //    Bitmap bitmap6 = sISThreshold.Apply(bitmap5);
        //    //if (m_IsSaveTemp)
        //    //{
        //    //    bitmap6.Save("D:\\testtest\\" + RelateAnalyzeString + "bitmap6" + ".png", System.Drawing.Imaging.ImageFormat.Png);
        //    //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
        //    //}
        //    bitmap5.Dispose();

        //    m_JzFind.GetMaskedImage(bitmap6, imgmask, Color.White, Color.White, false);
        //    if (m_IsSaveTemp)
        //    {
        //        bitmap6.Save("D:\\testtest\\" + RelateAnalyzeString + "bitmap6" + ".png", System.Drawing.Imaging.ImageFormat.Png);
        //        //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
        //    }
        //    //AForge.Imaging.Filters.Dilatation3x3 dilatation3X3 = new AForge.Imaging.Filters.Dilatation3x3();
        //    //Bitmap bitmap7 = dilatation3X3.Apply(bitmap6);
        //    //AForge.Imaging.Filters.Erosion3x3 erosion3X3 = new AForge.Imaging.Filters.Erosion3x3();
        //    //Bitmap bitmap8 = erosion3X3.Apply(bitmap7);

        //    //SetErosion3x3(ref bitmap6, 3);
        //    //SetDilatation3x3(ref bitmap6, 2);

        //    Point point = new Point((rect1.Right - rect1.X) / 2, rect1.Y - 5);
        //    Bitmap bmpfloodfill = FloodFill(bitmap6, point, Color.FromArgb(0, 255, 0), 18);
        //    bitmap6.Dispose();
        //    //bitmap7.Dispose();
        //    //bitmap8.Dispose();
        //    if (m_IsSaveTemp)
        //    {
        //        bmpfloodfill.Save("D:\\testtest\\" + RelateAnalyzeString + "bmpfloodfill" + ".png", System.Drawing.Imaging.ImageFormat.Png);
        //        //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
        //    }

        //    return bmpfloodfill;
        //}
        private Bitmap _getV2_2bmpInput(Rectangle rect1)
        {
            //rect1.Inflate(2, 2);

            Graphics gPrepared = Graphics.FromImage(imgmask);
            gPrepared.Clear(Color.White);
            //gPrepared.FillPolygon(Brushes.Black, PADTempRegion.RegionPtFCornerORG);
            gPrepared.FillRectangle(Brushes.Black, rect1);
            gPrepared.Dispose();

            HistogramClass histogramClass = new HistogramClass(2);
            histogramClass.GetHistogram(imginput);

            int fillthresholdvalue = (int)(255 * (1 - NoGlueThresholdValue));
            fillthresholdvalue = histogramClass.MinGrade;
            m_JzFind.GetMaskedImage(imginput, imgmask, Color.White, Color.FromArgb(fillthresholdvalue, fillthresholdvalue, fillthresholdvalue), false);

            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            Bitmap bitmap1 = grayscale.Apply(imginput);

            AForge.Imaging.Filters.SISThreshold sISThreshold = new AForge.Imaging.Filters.SISThreshold();
            Bitmap bitmap2 = sISThreshold.Apply(bitmap1);

            //SetErosion3x3(ref bitmap2, 2);
            //SetDilatation3x3(ref bitmap2, 1);

            Point point = new Point((rect1.Right - rect1.X) / 2, (rect1.Bottom - rect1.Y) / 2);
            Bitmap bmpfloodfill = new Bitmap(bitmap2);
            //JzFindObjectClass jzFindObjectClass = new JzFindObjectClass();
            //jzFindObjectClass.FindGrayscale(bmpfloodfill, point, Color.FromArgb(0, 255, 0), 10);
            bmpfloodfill = FloodFill(bitmap2, point, Color.FromArgb(0, 255, 0), 10);
            if (bmpfloodfill == null)
                bmpfloodfill = imginput;
            if (m_IsSaveTemp)
            {
                bmpfloodfill.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpfloodfill" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
            }
            m_JzFind.GetMaskedImage(bmpfloodfill, imgmask, Color.White, Color.White, false);
            //return bitmap6;
            return bmpfloodfill;
        }
        private Bitmap _getV3bmpInput(Rectangle rect1)
        {
            //rect1.Inflate(2, 2);

            Graphics gPrepared = Graphics.FromImage(imgmask);
            gPrepared.Clear(Color.White);
            //gPrepared.FillPolygon(Brushes.Black, PADTempRegion.RegionPtFCornerORG);
            gPrepared.FillRectangle(Brushes.Black, rect1);
            gPrepared.Dispose();

            HistogramClass histogramClass = new HistogramClass(2);
            histogramClass.GetHistogram(imginput);

            int fillthresholdvalue = (int)(255 * (1 - NoGlueThresholdValue));
            fillthresholdvalue = histogramClass.MinGrade;
            m_JzFind.GetMaskedImage(imginput, imgmask, Color.White, Color.FromArgb(fillthresholdvalue, fillthresholdvalue, fillthresholdvalue), false);

            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            Bitmap bitmap1 = grayscale.Apply(imginput);

            AForge.Imaging.Filters.CannyEdgeDetector detector = new AForge.Imaging.Filters.CannyEdgeDetector();
            detector.GaussianSigma = 1.4;
            detector.LowThreshold = 20;
            detector.HighThreshold = 20;
            Bitmap bitmap2 = detector.Apply(bitmap1);

            AForge.Imaging.Filters.Threshold threshold = new AForge.Imaging.Filters.Threshold(99);
            Bitmap bitmap3 = threshold.Apply(bitmap2);

            if (m_IsSaveTemp)
            {
                bitmap3.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bitmap3ca" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
            }
            m_JzFind.GetMaskedImage(bitmap3, imgmask, Color.White, Color.White, false);
            //return bitmap6;
            return bitmap3;
        }
        private Bitmap _getV6bmpInput(Rectangle rect1)
        {
            AForge.Imaging.Filters.ContrastStretch contrastStretch = new AForge.Imaging.Filters.ContrastStretch();
            Bitmap bitmap0 = contrastStretch.Apply(imginput);

            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            Bitmap bitmap1 = grayscale.Apply(bitmap0);

            AForge.Imaging.Filters.BradleyLocalThresholding bradleyLocalThresholding =
                                                        new AForge.Imaging.Filters.BradleyLocalThresholding();
            Bitmap bitmap2 = bradleyLocalThresholding.Apply(bitmap1);

            AForge.Imaging.Filters.BlobsFiltering blobsFiltering = new AForge.Imaging.Filters.BlobsFiltering();
            blobsFiltering.CoupledSizeFiltering = false;//or
            blobsFiltering.MinWidth = imginput.Width / 10;
            blobsFiltering.MinHeight = imginput.Height / 10;
            Bitmap bitmap3 = blobsFiltering.Apply(bitmap2);

            AForge.Imaging.Filters.Invert invert = new AForge.Imaging.Filters.Invert();
            Bitmap bitmap4 = invert.Apply(bitmap3);

            AForge.Imaging.Filters.Opening opening = new AForge.Imaging.Filters.Opening();
            Bitmap bmpopening = opening.Apply(bitmap4);

            AForge.Imaging.Filters.BlobsFiltering blobsFiltering2 = new AForge.Imaging.Filters.BlobsFiltering();
            blobsFiltering2.CoupledSizeFiltering = false;//or
            blobsFiltering2.MinWidth = imginput.Width / 10;
            blobsFiltering2.MinHeight = imginput.Height / 10;
            Bitmap bitmap5 = blobsFiltering2.Apply(bmpopening);

            if (m_IsSaveTemp)
            {
                bitmap5.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpfloodfill" + ".png",
                    System.Drawing.Imaging.ImageFormat.Png);
            }
            return bitmap5;
        }

        public IEzSeg model
        {
            get { return Universal.model; }
        }
        private int _from_bmpinputSize_to_iSized()
        {
            int isized = 0;
            int minSize = Math.Min(imginput.Size.Width, imginput.Size.Height);
            if (minSize >= 1500)
                isized = -2;
            else if (minSize >= 1000)
                isized = -1;
            else
                isized = 0;
            return isized;
        }
        private Bitmap _getV8bmpInput(Rectangle rect1, int isized = -2)
        {
            if (!INI.chipUseAI)
                return _getV6bmpInput(rect1);
            int minSize = Math.Min(imginput.Size.Width, imginput.Size.Height);
            //if (minSize >= 1500)
            //    isized = -2;
            //else if (minSize >= 750)
            //    isized = -2;
            //else
            //    isized = 0;
            Bitmap img = new Bitmap(imginput, Resize(imginput.Size, isized));
            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            img = grayscale.Apply(img);
            if (m_IsSaveTemp)
            {
                img.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "imgPredict_Pre" + ".png",
                    System.Drawing.Imaging.ImageFormat.Png);
            }
            // 預測 (單張)
            Bitmap mask = model.Predict(img);
            //mask = new Bitmap(mask, Resize(mask.Size, 2));
            //BitmapData bmpData = mask.LockBits(new Rectangle(0, 0, mask.Width, mask.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            //Bitmap mask1 = new Bitmap(Resize(mask.Size, 2).Width, Resize(mask.Size, 2).Height,
            //    bmpData.Stride, PixelFormat.Format32bppArgb, bmpData.Scan0);
            //mask.UnlockBits(bmpData);
            //Bitmap mask2 = (Bitmap)mask1.Clone();
            //// 預測 (單張)
            //Bitmap mask = model.Predict(imginput);

            //Bitmap bmpDraw = (Bitmap)imginput.Clone();
            //EzSegClientExamples.EzImageUtils.DrawMask(bmpDraw, mask);
            //mask.Dispose();

            if (m_IsSaveTemp)
            {
                mask.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "imgPredict" + ".png",
                    System.Drawing.Imaging.ImageFormat.Png);
            }
            return mask;
        }

        private Bitmap _getV5bmpInput(Bitmap bitmapinput, BorderTypeEnum borderType)
        {
            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            Bitmap bitmap1 = grayscale.Apply(bitmapinput);

            AForge.Imaging.Filters.GaussianBlur gaussianBlur = new AForge.Imaging.Filters.GaussianBlur(1.4, 5);
            Bitmap bitmap2 = gaussianBlur.Apply(bitmap1);

            AForge.Imaging.Filters.SISThreshold sISThreshold = new AForge.Imaging.Filters.SISThreshold();
            Bitmap bitmap3 = sISThreshold.Apply(bitmap2);

            SetDilatation3x3(ref bitmap3, 1);
            SetErosion3x3(ref bitmap3, 1);


            if (m_IsSaveTemp)
            {
                bitmap3.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bitmap3ca" + borderType.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
            }

            bitmap1.Dispose();
            bitmap2.Dispose();
            return bitmap3;
        }
        private void SetErosion3x3(ref Bitmap bmp, int edilatationCount = 0)
        {
            if (edilatationCount == 0)
                return;
            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            AForge.Imaging.Filters.Erosion3x3 erosion3X3 = new AForge.Imaging.Filters.Erosion3x3();
            Bitmap bmptemp = (Bitmap)bmp.Clone();// new Bitmap(bmp);
            int i = 0;
            while (i < edilatationCount)
            {
                Bitmap bitmap = grayscale.Apply(bmptemp);
                Bitmap bitmap1 = erosion3X3.Apply(bitmap);
                bmptemp = new Bitmap(bitmap1);
                bitmap1.Dispose();

                i++;
            }
            bmp.Dispose();
            bmp = (Bitmap)bmptemp.Clone();//new Bitmap(bmptemp);
        }
        private void SetDilatation3x3(ref Bitmap bmp, int edilatationCount = 0)
        {
            if (edilatationCount == 0)
                return;
            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            AForge.Imaging.Filters.Dilatation3x3 dilatation3X3 = new AForge.Imaging.Filters.Dilatation3x3();
            Bitmap bmptemp = (Bitmap)bmp.Clone();// new Bitmap(bmp);
            int i = 0;
            while (i < edilatationCount)
            {
                Bitmap bitmap = grayscale.Apply(bmptemp);
                Bitmap bitmap1 = dilatation3X3.Apply(bitmap);
                bmptemp = new Bitmap(bitmap1);
                bitmap1.Dispose();

                i++;
            }
            bmp.Dispose();
            bmp = (Bitmap)bmptemp.Clone();// new Bitmap(bmptemp);
        }


        #region BLACK EDGE CAL
        private Bitmap blackAICal(PADRegionClass pADRegionClass,int iSized = 2)
        {
            if (!INI.chipUseAI)
                return blackNormal(pADRegionClass);
            int isized = iSized;
            Bitmap img = new Bitmap(imginput, Resize(imginput.Size, -isized));
                //new Size(imginput.Width / isized, imginput.Height / isized));

            PointF[] points = new PointF[pADRegionClass.RegionPtFCornerORG.Length];
            points[0] = ResizeWithLocation2(pADRegionClass.RegionPtFCornerORG[0], -isized);
            points[1] = ResizeWithLocation2(pADRegionClass.RegionPtFCornerORG[1], -isized);
            points[2] = ResizeWithLocation2(pADRegionClass.RegionPtFCornerORG[2], -isized);
            points[3] = ResizeWithLocation2(pADRegionClass.RegionPtFCornerORG[3], -isized);

            Graphics gPrepared = Graphics.FromImage(img);
            gPrepared.FillPolygon(Brushes.White, points);
            gPrepared.Dispose();

            //if (m_IsSaveTemp)
            //    img.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpfloodfillimg" + ".png",
            //                       System.Drawing.Imaging.ImageFormat.Png);
            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            img = grayscale.Apply(img);
            if (m_IsSaveTemp)
                img.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpfloodfill_AiPre" + ".png",
                                   System.Drawing.Imaging.ImageFormat.Png);
            // 預測 (單張)
            Bitmap mask = model.Predict(img);
            if (m_IsSaveTemp)
            {
                mask.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpfloodfill_Ai" + ".png",
                                   System.Drawing.Imaging.ImageFormat.Png);
            }
            return mask;
        }
        private Bitmap blackNormal(PADRegionClass pADRegionClass,int iSized = 5)
        {
            int isizedx = 5;
            Bitmap bmphistogram = new Bitmap(imginput, new Size(imginput.Width / isizedx, imginput.Height / isizedx));
            HistogramClass histogramClass = new HistogramClass(2);
            histogramClass.GetHistogram(bmphistogram);

            int isized = iSized;
            Bitmap img = new Bitmap(imginput, new Size(imginput.Width / isized, imginput.Height / isized));

            PointF[] points = new PointF[pADRegionClass.RegionPtFCornerORG.Length];
            points[0] = ResizeWithLocation2(pADRegionClass.RegionPtFCornerORG[0], -(float)isized);
            points[1] = ResizeWithLocation2(pADRegionClass.RegionPtFCornerORG[1], -(float)isized);
            points[2] = ResizeWithLocation2(pADRegionClass.RegionPtFCornerORG[2], -(float)isized);
            points[3] = ResizeWithLocation2(pADRegionClass.RegionPtFCornerORG[3], -(float)isized);
            //points[0] = pADRegionClass.RegionPtFCornerORG[0];
            //points[1] = pADRegionClass.RegionPtFCornerORG[1];
            //points[2] = pADRegionClass.RegionPtFCornerORG[2];
            //points[3] = pADRegionClass.RegionPtFCornerORG[3];

            Graphics gPrepared = Graphics.FromImage(img);
            gPrepared.FillPolygon(Brushes.White, points);
            gPrepared.Dispose();

            //if (m_IsSaveTemp)
            //{
            //    img.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpffimg" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
            //}

            PointF pointF = pADRegionClass.GetTopEdgeCenter(isized, 0, 5);
            Point point = new Point((int)pointF.X, (int)pointF.Y);
            Bitmap bmpfloodfill = (Bitmap)img.Clone();

            bmpfloodfill = FloodFill(img, point, Color.FromArgb(0, 255, 0),
                                                (int)(histogramClass.MinGrade * BloodFillValueRatio));
            if (bmpfloodfill == null)
                bmpfloodfill = imginput;
            if (m_IsSaveTemp)
            {
                bmpfloodfill.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpff" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
            }
            img.Dispose();
            bmphistogram.Dispose();
            return bmpfloodfill;
        }

        #endregion


        private Bitmap _getG1bmpInput(Rectangle rect1)
        {
            //rect1.Inflate(2, 2);

            Graphics gPrepared = Graphics.FromImage(imgmask);
            gPrepared.Clear(Color.White);
            //gPrepared.FillPolygon(Brushes.Black, PADTempRegion.RegionPtFCornerORG);
            gPrepared.FillRectangle(Brushes.Black, rect1);
            gPrepared.Dispose();

            //Bitmap bmpfloodfill = (Bitmap)imginput.Clone();// new Bitmap(imginput);
            Bitmap bmphistogram = new Bitmap(imginput, Resize(imginput.Size, -5));
            HistogramClass histogramClass = new HistogramClass(2);
            histogramClass.GetHistogram(bmphistogram);

            //int fillthresholdvalue = (int)(255 * (1 - NoGlueThresholdValue));
            //fillthresholdvalue = histogramClass.MinGrade;
            //m_JzFind.GetMaskedImage(imginput, imgmask, Color.White, Color.FromArgb(fillthresholdvalue, fillthresholdvalue, fillthresholdvalue), false);

            Point point = new Point((rect1.Right - rect1.X) / 2, (rect1.Bottom - rect1.Y) / 2);
            point = new Point((rect1.Right - rect1.X) / 2, rect1.Y - 10);
            Bitmap bmpfloodfill = (Bitmap)imginput.Clone();// new Bitmap(imginput);
            //JzFindObjectClass jzFindObjectClass = new JzFindObjectClass();
            //jzFindObjectClass.FindGrayscale(bmpfloodfill, point, Color.FromArgb(0, 255, 0), (int)(histogramClass.MinGrade * BloodFillValueRatio));

            bmpfloodfill = FloodFill(imginput, point, Color.FromArgb(0, 255, 0), (int)(histogramClass.MinGrade * BloodFillValueRatio));
            if (bmpfloodfill == null)
                bmpfloodfill = imginput;
            if (m_IsSaveTemp)
            {
                bmpfloodfill.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpff" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
            }
            //m_JzFind.GetMaskedImage(bmpfloodfill, imgmask, Color.White, Color.White, false);
            //return bitmap6;
            return bmpfloodfill;
        }
        /// <summary>
        /// 魔棒工具
        /// </summary>
        /// <param name="src"></param>
        /// <param name="location"></param>
        /// <param name="fillColor"></param>
        /// <param name="threshould"></param>
        /// <returns></returns>
        public Bitmap FloodFill(Bitmap src, Point location, Color fillColor, int threshould)
        {
            try
            {
                Bitmap srcbmp = src;
                Bitmap dstbmp = new Bitmap(src.Width, src.Height);
                int w = srcbmp.Width;
                int h = srcbmp.Height;
                Stack<Point> fillPoints = new Stack<Point>(w * h);
                System.Drawing.Imaging.BitmapData bmpData = srcbmp.LockBits(new Rectangle(0, 0, srcbmp.Width, srcbmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                System.Drawing.Imaging.BitmapData dstbmpData = dstbmp.LockBits(new Rectangle(0, 0, dstbmp.Width, dstbmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                IntPtr ptr = bmpData.Scan0;
                int stride = bmpData.Stride;
                int bytes = bmpData.Stride * srcbmp.Height;
                byte[] grayValues = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(ptr, grayValues, 0, bytes);
                Color backColor = Color.FromArgb(grayValues[location.X * 3 + 2 + location.Y * stride], grayValues[location.X * 3 + 1 + location.Y * stride], grayValues[location.X * 3 + location.Y * stride]);

                IntPtr dstptr = dstbmpData.Scan0;
                byte[] temp = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(dstptr, temp, 0, bytes);

                int gray = (int)((backColor.R + backColor.G + backColor.B) / 3);
                if (location.X < 0 || location.X >= w || location.Y < 0 || location.Y >= h) return null;
                fillPoints.Push(new Point(location.X, location.Y));
                int[,] mask = new int[w, h];

                while (fillPoints.Count > 0)
                {

                    Point p = fillPoints.Pop();
                    mask[p.X, p.Y] = 1;
                    temp[3 * p.X + p.Y * stride] = (byte)fillColor.B;
                    temp[3 * p.X + 1 + p.Y * stride] = (byte)fillColor.G;
                    temp[3 * p.X + 2 + p.Y * stride] = (byte)fillColor.R;
                    if (p.X > 0 && (Math.Abs(gray - (int)((grayValues[3 * (p.X - 1) + p.Y * stride] + grayValues[3 * (p.X - 1) + 1 + p.Y * stride] + grayValues[3 * (p.X - 1) + 2 + p.Y * stride]) / 3)) < threshould) && (mask[p.X - 1, p.Y] != 1))
                    {
                        temp[3 * (p.X - 1) + p.Y * stride] = (byte)fillColor.B;
                        temp[3 * (p.X - 1) + 1 + p.Y * stride] = (byte)fillColor.G;
                        temp[3 * (p.X - 1) + 2 + p.Y * stride] = (byte)fillColor.R;
                        fillPoints.Push(new Point(p.X - 1, p.Y));
                        mask[p.X - 1, p.Y] = 1;
                    }
                    if (p.X < w - 1 && (Math.Abs(gray - (int)((grayValues[3 * (p.X + 1) + p.Y * stride] + grayValues[3 * (p.X + 1) + 1 + p.Y * stride] + grayValues[3 * (p.X + 1) + 2 + p.Y * stride]) / 3)) < threshould) && (mask[p.X + 1, p.Y] != 1))
                    {
                        temp[3 * (p.X + 1) + p.Y * stride] = (byte)fillColor.B;
                        temp[3 * (p.X + 1) + 1 + p.Y * stride] = (byte)fillColor.G;
                        temp[3 * (p.X + 1) + 2 + p.Y * stride] = (byte)fillColor.R;
                        fillPoints.Push(new Point(p.X + 1, p.Y));
                        mask[p.X + 1, p.Y] = 1;
                    }
                    if (p.Y > 0 && (Math.Abs(gray - (int)((grayValues[3 * p.X + (p.Y - 1) * stride] + grayValues[3 * p.X + 1 + (p.Y - 1) * stride] + grayValues[3 * p.X + 2 + (p.Y - 1) * stride]) / 3)) < threshould) && (mask[p.X, p.Y - 1] != 1))
                    {
                        temp[3 * p.X + (p.Y - 1) * stride] = (byte)fillColor.B;
                        temp[3 * p.X + 1 + (p.Y - 1) * stride] = (byte)fillColor.G;
                        temp[3 * p.X + 2 + (p.Y - 1) * stride] = (byte)fillColor.R;
                        fillPoints.Push(new Point(p.X, p.Y - 1));
                        mask[p.X, p.Y - 1] = 1;
                    }
                    if (p.Y < h - 1 && (Math.Abs(gray - (int)((grayValues[3 * p.X + (p.Y + 1) * stride] + grayValues[3 * p.X + 1 + (p.Y + 1) * stride] + grayValues[3 * p.X + 2 + (p.Y + 1) * stride]) / 3)) < threshould) && (mask[p.X, p.Y + 1] != 1))
                    {
                        temp[3 * p.X + (p.Y + 1) * stride] = (byte)fillColor.B;
                        temp[3 * p.X + 1 + (p.Y + 1) * stride] = (byte)fillColor.G;
                        temp[3 * p.X + 2 + (p.Y + 1) * stride] = (byte)fillColor.R;
                        fillPoints.Push(new Point(p.X, p.Y + 1));
                        mask[p.X, p.Y + 1] = 1;
                    }
                }
                fillPoints.Clear();

                System.Runtime.InteropServices.Marshal.Copy(temp, 0, dstptr, bytes);
                srcbmp.UnlockBits(bmpData);
                dstbmp.UnlockBits(dstbmpData);

                return dstbmp;
            }
            catch (Exception exp)
            {
                //MessageBox.Show(exp.Message);
                return null;
            }
        }
        public Bitmap FloodFill(Bitmap src, Point location, Color fillColor, int threshouldmin, int threshouldmax)
        {
            try
            {
                Bitmap srcbmp = src;
                Bitmap dstbmp = new Bitmap(src.Width, src.Height);
                int w = srcbmp.Width;
                int h = srcbmp.Height;
                Stack<Point> fillPoints = new Stack<Point>(w * h);
                System.Drawing.Imaging.BitmapData bmpData = srcbmp.LockBits(new Rectangle(0, 0, srcbmp.Width, srcbmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                System.Drawing.Imaging.BitmapData dstbmpData = dstbmp.LockBits(new Rectangle(0, 0, dstbmp.Width, dstbmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                IntPtr ptr = bmpData.Scan0;
                int stride = bmpData.Stride;
                int bytes = bmpData.Stride * srcbmp.Height;
                byte[] grayValues = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(ptr, grayValues, 0, bytes);
                Color backColor = Color.FromArgb(grayValues[location.X * 3 + 2 + location.Y * stride], grayValues[location.X * 3 + 1 + location.Y * stride], grayValues[location.X * 3 + location.Y * stride]);

                IntPtr dstptr = dstbmpData.Scan0;
                byte[] temp = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(dstptr, temp, 0, bytes);

                int gray = (int)((backColor.R + backColor.G + backColor.B) / 3);
                if (location.X < 0 || location.X >= w || location.Y < 0 || location.Y >= h) return null;
                fillPoints.Push(new Point(location.X, location.Y));
                int[,] mask = new int[w, h];

                while (fillPoints.Count > 0)
                {

                    Point p = fillPoints.Pop();
                    mask[p.X, p.Y] = 1;
                    temp[3 * p.X + p.Y * stride] = (byte)fillColor.B;
                    temp[3 * p.X + 1 + p.Y * stride] = (byte)fillColor.G;
                    temp[3 * p.X + 2 + p.Y * stride] = (byte)fillColor.R;
                    if (p.X > 0 &&
                        (Math.Abs(gray - (int)((grayValues[3 * (p.X - 1) + p.Y * stride] + grayValues[3 * (p.X - 1) + 1 + p.Y * stride] + grayValues[3 * (p.X - 1) + 2 + p.Y * stride]) / 3)) > threshouldmin) &&
                        (Math.Abs(gray - (int)((grayValues[3 * (p.X - 1) + p.Y * stride] + grayValues[3 * (p.X - 1) + 1 + p.Y * stride] + grayValues[3 * (p.X - 1) + 2 + p.Y * stride]) / 3)) < threshouldmax) && (mask[p.X - 1, p.Y] != 1))
                    {
                        temp[3 * (p.X - 1) + p.Y * stride] = (byte)fillColor.B;
                        temp[3 * (p.X - 1) + 1 + p.Y * stride] = (byte)fillColor.G;
                        temp[3 * (p.X - 1) + 2 + p.Y * stride] = (byte)fillColor.R;
                        fillPoints.Push(new Point(p.X - 1, p.Y));
                        mask[p.X - 1, p.Y] = 1;
                    }
                    if (p.X < w - 1 &&
                        (Math.Abs(gray - (int)((grayValues[3 * (p.X + 1) + p.Y * stride] + grayValues[3 * (p.X + 1) + 1 + p.Y * stride] + grayValues[3 * (p.X + 1) + 2 + p.Y * stride]) / 3)) > threshouldmin) &&
                        (Math.Abs(gray - (int)((grayValues[3 * (p.X + 1) + p.Y * stride] + grayValues[3 * (p.X + 1) + 1 + p.Y * stride] + grayValues[3 * (p.X + 1) + 2 + p.Y * stride]) / 3)) < threshouldmax) && (mask[p.X + 1, p.Y] != 1))
                    {
                        temp[3 * (p.X + 1) + p.Y * stride] = (byte)fillColor.B;
                        temp[3 * (p.X + 1) + 1 + p.Y * stride] = (byte)fillColor.G;
                        temp[3 * (p.X + 1) + 2 + p.Y * stride] = (byte)fillColor.R;
                        fillPoints.Push(new Point(p.X + 1, p.Y));
                        mask[p.X + 1, p.Y] = 1;
                    }
                    if (p.Y > 0 &&
                        (Math.Abs(gray - (int)((grayValues[3 * p.X + (p.Y - 1) * stride] + grayValues[3 * p.X + 1 + (p.Y - 1) * stride] + grayValues[3 * p.X + 2 + (p.Y - 1) * stride]) / 3)) > threshouldmin) &&
                        (Math.Abs(gray - (int)((grayValues[3 * p.X + (p.Y - 1) * stride] + grayValues[3 * p.X + 1 + (p.Y - 1) * stride] + grayValues[3 * p.X + 2 + (p.Y - 1) * stride]) / 3)) < threshouldmax) && (mask[p.X, p.Y - 1] != 1))
                    {
                        temp[3 * p.X + (p.Y - 1) * stride] = (byte)fillColor.B;
                        temp[3 * p.X + 1 + (p.Y - 1) * stride] = (byte)fillColor.G;
                        temp[3 * p.X + 2 + (p.Y - 1) * stride] = (byte)fillColor.R;
                        fillPoints.Push(new Point(p.X, p.Y - 1));
                        mask[p.X, p.Y - 1] = 1;
                    }
                    if (p.Y < h - 1 &&
                        (Math.Abs(gray - (int)((grayValues[3 * p.X + (p.Y + 1) * stride] + grayValues[3 * p.X + 1 + (p.Y + 1) * stride] + grayValues[3 * p.X + 2 + (p.Y + 1) * stride]) / 3)) > threshouldmin) &&
                        (Math.Abs(gray - (int)((grayValues[3 * p.X + (p.Y + 1) * stride] + grayValues[3 * p.X + 1 + (p.Y + 1) * stride] + grayValues[3 * p.X + 2 + (p.Y + 1) * stride]) / 3)) < threshouldmax) && (mask[p.X, p.Y + 1] != 1))
                    {
                        temp[3 * p.X + (p.Y + 1) * stride] = (byte)fillColor.B;
                        temp[3 * p.X + 1 + (p.Y + 1) * stride] = (byte)fillColor.G;
                        temp[3 * p.X + 2 + (p.Y + 1) * stride] = (byte)fillColor.R;
                        fillPoints.Push(new Point(p.X, p.Y + 1));
                        mask[p.X, p.Y + 1] = 1;
                    }
                }
                fillPoints.Clear();

                System.Runtime.InteropServices.Marshal.Copy(temp, 0, dstptr, bytes);
                srcbmp.UnlockBits(bmpData);
                dstbmp.UnlockBits(dstbmpData);

                return dstbmp;
            }
            catch (Exception exp)
            {
                //MessageBox.Show(exp.Message);
                return null;
            }
        }
        public Bitmap FloodFill2(Bitmap src, Point location, Color fillColor, int threshould)
        {
            try
            {
                Bitmap srcbmp = src;
                Bitmap dstbmp = new Bitmap(src.Width, src.Height);
                int w = srcbmp.Width;
                int h = srcbmp.Height;
                Stack<Point> fillPoints = new Stack<Point>(w * h);
                System.Drawing.Imaging.BitmapData bmpData = srcbmp.LockBits(new Rectangle(0, 0, srcbmp.Width, srcbmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                System.Drawing.Imaging.BitmapData dstbmpData = dstbmp.LockBits(new Rectangle(0, 0, dstbmp.Width, dstbmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                IntPtr ptr = bmpData.Scan0;
                int stride = bmpData.Stride;
                int bytes = bmpData.Stride * srcbmp.Height;
                byte[] grayValues = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(ptr, grayValues, 0, bytes);
                Color backColor = Color.FromArgb(grayValues[location.X * 3 + 2 + location.Y * stride], grayValues[location.X * 3 + 1 + location.Y * stride], grayValues[location.X * 3 + location.Y * stride]);

                IntPtr dstptr = dstbmpData.Scan0;
                byte[] temp = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(dstptr, temp, 0, bytes);

                int gray = (int)((backColor.R + backColor.G + backColor.B) / 3);
                gray = (int)((backColor.R * 0.299 + backColor.G * 0.587 + backColor.B * 0.114));
                if (location.X < 0 || location.X >= w || location.Y < 0 || location.Y >= h) return null;
                fillPoints.Push(new Point(location.X, location.Y));
                int[,] mask = new int[w, h];

                while (fillPoints.Count > 0)
                {

                    Point p = fillPoints.Pop();
                    mask[p.X, p.Y] = 1;
                    temp[3 * p.X + p.Y * stride] = (byte)fillColor.B;
                    temp[3 * p.X + 1 + p.Y * stride] = (byte)fillColor.G;
                    temp[3 * p.X + 2 + p.Y * stride] = (byte)fillColor.R;
                    if (p.X > 0 && (Math.Abs(gray - (int)((grayValues[3 * (p.X - 1) + p.Y * stride] * 0.299 + grayValues[3 * (p.X - 1) + 1 + p.Y * stride] * 0.587 + grayValues[3 * (p.X - 1) + 2 + p.Y * stride] * 0.114) / 1)) < threshould) && (mask[p.X - 1, p.Y] != 1))
                    {
                        temp[3 * (p.X - 1) + p.Y * stride] = (byte)fillColor.B;
                        temp[3 * (p.X - 1) + 1 + p.Y * stride] = (byte)fillColor.G;
                        temp[3 * (p.X - 1) + 2 + p.Y * stride] = (byte)fillColor.R;
                        fillPoints.Push(new Point(p.X - 1, p.Y));
                        mask[p.X - 1, p.Y] = 1;
                    }
                    if (p.X < w - 1 && (Math.Abs(gray - (int)((grayValues[3 * (p.X + 1) + p.Y * stride] * 0.299 + grayValues[3 * (p.X + 1) + 1 + p.Y * stride] * 0.587 + grayValues[3 * (p.X + 1) + 2 + p.Y * stride] * 0.114) / 1)) < threshould) && (mask[p.X + 1, p.Y] != 1))
                    {
                        temp[3 * (p.X + 1) + p.Y * stride] = (byte)fillColor.B;
                        temp[3 * (p.X + 1) + 1 + p.Y * stride] = (byte)fillColor.G;
                        temp[3 * (p.X + 1) + 2 + p.Y * stride] = (byte)fillColor.R;
                        fillPoints.Push(new Point(p.X + 1, p.Y));
                        mask[p.X + 1, p.Y] = 1;
                    }
                    if (p.Y > 0 && (Math.Abs(gray - (int)((grayValues[3 * p.X + (p.Y - 1) * stride] * 0.299 + grayValues[3 * p.X + 1 + (p.Y - 1) * stride] * 0.587 + grayValues[3 * p.X + 2 + (p.Y - 1) * stride] * 0.114) / 1)) < threshould) && (mask[p.X, p.Y - 1] != 1))
                    {
                        temp[3 * p.X + (p.Y - 1) * stride] = (byte)fillColor.B;
                        temp[3 * p.X + 1 + (p.Y - 1) * stride] = (byte)fillColor.G;
                        temp[3 * p.X + 2 + (p.Y - 1) * stride] = (byte)fillColor.R;
                        fillPoints.Push(new Point(p.X, p.Y - 1));
                        mask[p.X, p.Y - 1] = 1;
                    }
                    if (p.Y < h - 1 && (Math.Abs(gray - (int)((grayValues[3 * p.X + (p.Y + 1) * stride] * 0.299 + grayValues[3 * p.X + 1 + (p.Y + 1) * stride] * 0.587 + grayValues[3 * p.X + 2 + (p.Y + 1) * stride] * 0.114) / 1)) < threshould) && (mask[p.X, p.Y + 1] != 1))
                    {
                        temp[3 * p.X + (p.Y + 1) * stride] = (byte)fillColor.B;
                        temp[3 * p.X + 1 + (p.Y + 1) * stride] = (byte)fillColor.G;
                        temp[3 * p.X + 2 + (p.Y + 1) * stride] = (byte)fillColor.R;
                        fillPoints.Push(new Point(p.X, p.Y + 1));
                        mask[p.X, p.Y + 1] = 1;
                    }
                    //if (p.X > 0 && (Math.Abs(gray - (int)((grayValues[3 * (p.X - 1) + (p.Y - 1) * stride] * 0.299 + grayValues[3 * (p.X - 1) + 1 + (p.Y - 1) * stride] * 0.587 + grayValues[3 * (p.X - 1) + 2 + (p.Y - 1) * stride] * 0.114) / 1)) < threshould) && (mask[p.X - 1, p.Y - 1] != 1))
                    //{
                    //    temp[3 * (p.X - 1) + p.Y * stride] = (byte)fillColor.B;
                    //    temp[3 * (p.X - 1) + 1 + p.Y * stride] = (byte)fillColor.G;
                    //    temp[3 * (p.X - 1) + 2 + p.Y * stride] = (byte)fillColor.R;
                    //    fillPoints.Push(new Point(p.X - 1, (p.Y - 1)));
                    //    mask[p.X - 1, p.Y - 1] = 1;
                    //}
                    //if (p.X < w - 1 && (Math.Abs(gray - (int)((grayValues[3 * (p.X + 1) + (p.Y + 1) * stride] * 0.299 + grayValues[3 * (p.X + 1) + 1 + (p.Y + 1) * stride] * 0.587 + grayValues[3 * (p.X + 1) + 2 + (p.Y + 1) * stride] * 0.114) / 1)) < threshould) && (mask[p.X + 1, p.Y - 1] != 1))
                    //{
                    //    temp[3 * (p.X + 1) + p.Y * stride] = (byte)fillColor.B;
                    //    temp[3 * (p.X + 1) + 1 + p.Y * stride] = (byte)fillColor.G;
                    //    temp[3 * (p.X + 1) + 2 + p.Y * stride] = (byte)fillColor.R;
                    //    fillPoints.Push(new Point(p.X + 1, (p.Y + 1)));
                    //    mask[p.X + 1, p.Y + 1] = 1;
                    //}


                }
                fillPoints.Clear();

                System.Runtime.InteropServices.Marshal.Copy(temp, 0, dstptr, bytes);
                srcbmp.UnlockBits(bmpData);
                dstbmp.UnlockBits(dstbmpData);

                return dstbmp;
            }
            catch (Exception exp)
            {
                //MessageBox.Show(exp.Message);
                return null;
            }
        }

        public bool IsInRangeRatio(double CompValue, double FromValue, double Ratio)
        {
            return (FromValue >= (CompValue * (1 - (Ratio / 100d)))) && (FromValue <= (CompValue * (1 + (Ratio / 100d))));
        }

        public Rectangle SimpleRect(Point Pt, int Width, int Height)
        {
            Rectangle rect = SimpleRect(Pt);
            rect.Inflate(Width, Height);

            return rect;
        }
        public Rectangle SimpleRect(Point Pt)
        {
            return new Rectangle(Pt.X, Pt.Y, 1, 1);
        }
        public RectangleF SimpleRectF(PointF Pt, float Width, float Height)
        {
            RectangleF rectF = SimpleRectF(Pt);
            rectF.Inflate(Width, Height);

            return rectF;
        }
        public RectangleF SimpleRectF(PointF PtF)
        {
            return new RectangleF(PtF.X, PtF.Y, 1, 1);
        }
        private Point PointRotate(Point center, Point p1, double angle)
        {
            Point tmp = new Point();
            double angleHude = angle * Math.PI / 180;/*角度变成弧度*/
            double x1 = (p1.X - center.X) * Math.Cos(angleHude) + (p1.Y - center.Y) * Math.Sin(angleHude) + center.X;
            double y1 = -(p1.X - center.X) * Math.Sin(angleHude) + (p1.Y - center.Y) * Math.Cos(angleHude) + center.Y;
            tmp.X = (int)x1;
            tmp.Y = (int)y1;
            return tmp;
        }
        private PointF PointRotate(PointF center, PointF p1, double angle)
        {
            PointF tmp = new PointF();
            double angleHude = angle * Math.PI / 180;/*角度变成弧度*/
            double x1 = (p1.X - center.X) * Math.Cos(angleHude) + (p1.Y - center.Y) * Math.Sin(angleHude) + center.X;
            double y1 = -(p1.X - center.X) * Math.Sin(angleHude) + (p1.Y - center.Y) * Math.Cos(angleHude) + center.Y;
            tmp.X = (float)x1;
            tmp.Y = (float)y1;
            return tmp;
        }
        public Point[] RectToPoint(Rectangle xRect, double xAngle)
        {
            Point[] pts = new Point[4];

            Point ptCenter = GetRectCenter(xRect);
            pts[0] = xRect.Location;
            pts[1] = new Point(xRect.Location.X, xRect.Bottom);
            pts[2] = new Point(xRect.Right, xRect.Location.Y);
            pts[3] = new Point(xRect.Right, xRect.Bottom);

            pts[0] = PointRotate(ptCenter, pts[0], xAngle);
            pts[1] = PointRotate(ptCenter, pts[1], xAngle);
            pts[2] = PointRotate(ptCenter, pts[2], xAngle);
            pts[3] = PointRotate(ptCenter, pts[3], xAngle);

            return pts;
        }
        public PointF[] RectFToPointF(RectangleF xRectF, double xAngle)
        {
            PointF[] ptFs = new PointF[4];

            PointF ptCenter = GetRectFCenter(xRectF);
            ptFs[0] = xRectF.Location;
            ptFs[1] = new PointF(xRectF.Location.X, xRectF.Bottom);
            ptFs[2] = new PointF(xRectF.Right, xRectF.Bottom);
            ptFs[3] = new PointF(xRectF.Right, xRectF.Location.Y);

            ptFs[0] = PointRotate(ptCenter, ptFs[0], xAngle);
            ptFs[1] = PointRotate(ptCenter, ptFs[1], xAngle);
            ptFs[2] = PointRotate(ptCenter, ptFs[2], xAngle);
            ptFs[3] = PointRotate(ptCenter, ptFs[3], xAngle);

            return ptFs;
        }
        public Point GetRectCenter(Rectangle Rect)
        {
            return new Point(Rect.X + (Rect.Width >> 1), Rect.Y + (Rect.Height >> 1));
        }
        public PointF GetRectFCenter(RectangleF RectF)
        {
            return new PointF(RectF.X + (RectF.Width / 2), RectF.Y + (RectF.Height / 2));
        }

        public Rectangle SimpleRect(Size Sz)
        {
            return new Rectangle(0, 0, Sz.Width, Sz.Height);
        }
        public double GetPointLength(PointF P1, PointF P2)
        {
            return Math.Sqrt((double)Math.Pow((P1.X - P2.X), 2) + Math.Pow((P1.Y - P2.Y), 2));
        }
        //double GetRandom(double minval, double maxval)
        //{
        //    Random rnd = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

        //    double diffval = maxval - minval;

        //    diffval = diffval * (double)rnd.Next(1, 17) / 17d;

        //    return minval + diffval;
        //}

        public Size Resize(Size OrgSize, int Ratio)
        {
            Size retSize;

            if (Ratio > 0)
                retSize = new Size(OrgSize.Width << Ratio, OrgSize.Height << Ratio);
            else
                retSize = new Size(OrgSize.Width >> -Ratio, OrgSize.Height >> -Ratio);

            retSize.Width = Math.Max(retSize.Width, 1);
            retSize.Height = Math.Max(retSize.Height, 1);

            return retSize;
        }
        public RectangleF ResizeWithLocation2(Rectangle rect, int ratio)
        {
            Size retSize;
            PointF retPtF;

            if (ratio > 0)
            {
                retPtF = new PointF(rect.X << ratio, rect.Y << ratio);
                retSize = new Size(rect.Width << ratio, rect.Height << ratio);
            }
            else
            {
                retPtF = new PointF(rect.X >> -ratio, rect.Y >> -ratio);
                retSize = new Size(rect.Width >> -ratio, rect.Height >> -ratio);
            }

            retSize.Width = Math.Max(retSize.Width, 1);
            retSize.Height = Math.Max(retSize.Height, 1);

            return new RectangleF(retPtF.X, retPtF.Y, retSize.Width, retSize.Height);
        }
        public RectangleF ResizeWithLocation3(Rectangle rect, int ratio)
        {
            Size retSize;
            PointF retPtF;

            if (ratio > 0)
            {
                retPtF = new PointF(rect.X * ratio, rect.Y * ratio);
                retSize = new Size(rect.Width * ratio, rect.Height * ratio);
            }
            else
            {
                retPtF = new PointF(rect.X / -ratio, rect.Y / -ratio);
                retSize = new Size(rect.Width / -ratio, rect.Height / -ratio);
            }

            retSize.Width = Math.Max(retSize.Width, 1);
            retSize.Height = Math.Max(retSize.Height, 1);

            return new RectangleF(retPtF.X, retPtF.Y, retSize.Width, retSize.Height);
        }
        public JRotatedRectangleF ResizeWithLocation2(JRotatedRectangleF rect, int ratio)
        {
            JRotatedRectangleF jRotatedRectangleF = new JRotatedRectangleF();
            jRotatedRectangleF.fAngle = rect.fAngle;
            if (ratio == 0)
            {

                jRotatedRectangleF.fCX = rect.fCX * 1;
                jRotatedRectangleF.fCY = rect.fCY * 1;
                jRotatedRectangleF.fWidth = rect.fWidth * 1;
                jRotatedRectangleF.fHeight = rect.fHeight * 1;

                return jRotatedRectangleF;
            }
           
            if (ratio > 0)
            {
                jRotatedRectangleF.fCX = rect.fCX * ratio;
                jRotatedRectangleF.fCY = rect.fCY * ratio;
                jRotatedRectangleF.fWidth = rect.fWidth * ratio;
                jRotatedRectangleF.fHeight = rect.fHeight * ratio;
            }
            else if (ratio < 0)
            {
                jRotatedRectangleF.fCX = rect.fCX / ratio;
                jRotatedRectangleF.fCY = rect.fCY / ratio;
                jRotatedRectangleF.fWidth = rect.fWidth / ratio;
                jRotatedRectangleF.fHeight = rect.fHeight / ratio;
            }

            return jRotatedRectangleF;
        }
        JRotatedRectangleF MergeTwoRects(JRotatedRectangleF rect1, JRotatedRectangleF rect2)
        {
            JRotatedRectangleF rect = new JRotatedRectangleF();

            if (rect1.fWidth == 0)
                return rect2;
            if (rect2.fWidth == 0)
                return rect1;

            rect.fCX = Math.Min(rect1.fCX, rect2.fCX);
            rect.fCY = Math.Min(rect1.fCY, rect2.fCY);

            rect.fWidth = Math.Max(rect1.fCX + rect1.fWidth, rect2.fCX + rect2.fWidth) - rect.fCX;
            rect.fHeight = Math.Max(rect1.fCY + rect1.fHeight, rect2.fCY + rect2.fHeight) - rect.fCY;

            rect.fAngle = rect1.fAngle;

            return rect;
        }
        RectangleF MergeTwoRects(RectangleF rect1, RectangleF rect2)
        {
            RectangleF rect = new RectangleF();

            if (rect1.Width == 0)
                return rect2;
            if (rect2.Width == 0)
                return rect1;

            rect.X = Math.Min(rect1.X, rect2.X);
            rect.Y = Math.Min(rect1.Y, rect2.Y);

            rect.Width = Math.Max(rect1.X + rect1.Width, rect2.X + rect2.Width) - rect.X;
            rect.Height = Math.Max(rect1.Y + rect1.Height, rect2.Y + rect2.Height) - rect.Y;

            return rect;
        }
        public PointF ResizeWithLocation2(PointF ptf, int ratio)
        {
            Size retSize;
            PointF retPtF;
            Point epoint = new Point((int)ptf.X, (int)ptf.Y);

            if (ratio > 0)
            {
                retPtF = new PointF(epoint.X << ratio, epoint.Y << ratio);
                //retSize = new Size(rect.Width << ratio, rect.Height << ratio);
            }
            else
            {
                retPtF = new PointF(epoint.X >> -ratio, epoint.Y >> -ratio);
                //retSize = new Size(rect.Width >> -ratio, rect.Height >> -ratio);
            }

            //retSize.Width = Math.Max(retSize.Width, 1);
            //retSize.Height = Math.Max(retSize.Height, 1);
            return retPtF;
            //return new RectangleF(retPtF.X, retPtF.Y, retSize.Width, retSize.Height);
        }
        /// <summary>
        /// 乘积缩小
        /// </summary>
        /// <param name="ptf"></param>
        /// <param name="ratio"></param>
        /// <returns></returns>
        public PointF ResizeWithLocation2(PointF ptf, float ratio)
        {
            Size retSize;
            PointF retPtF;
            //Point epoint = new Point((int)ptf.X, (int)ptf.Y);

            if (ratio > 0)
            {
                retPtF = new PointF(ptf.X * ratio, ptf.Y * ratio);
                //retSize = new Size(rect.Width << ratio, rect.Height << ratio);
            }
            else
            {
                retPtF = new PointF(ptf.X / -ratio, ptf.Y / -ratio);
                //retSize = new Size(rect.Width >> -ratio, rect.Height >> -ratio);
            }

            //retSize.Width = Math.Max(retSize.Width, 1);
            //retSize.Height = Math.Max(retSize.Height, 1);
            return retPtF;
            //return new RectangleF(retPtF.X, retPtF.Y, retSize.Width, retSize.Height);
        }

        public double GetP1P2Angle(PointF p1,PointF p2)
        {
            double angleOfLine = Math.Atan2((p2.Y - p1.Y), (p2.X - p1.X)) * 180 / Math.PI;
            return angleOfLine;
        }
        #endregion

    }
}
