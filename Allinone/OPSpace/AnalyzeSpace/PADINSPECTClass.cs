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
using FreeImageAPI;
using System.Windows.Forms;
using JzKHC;
using Allinone.BasicSpace;
using System.ComponentModel;
using System.Windows.Media.Media3D;
using JetEazy.OpenCv4;
using Common;
using System.Windows.Shell;
using System.Diagnostics;
using Allinone.BasicSpace.MVD;
using iTextSharp.text.html.simpleparser;
using AForge.Imaging.Filters;
using AForge.Imaging;
using AForge.Math;
using AForge.Controls;
using static Allinone.OPSpace.AnalyzeSpace.PADINSPECTClass;
using System.Runtime.InteropServices;
using VisionDesigner;
using Allinone.FormSpace.BasicPG;
using MoveGraphLibrary;
using WorldOfMoveableObjects;
//using System.Windows;

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
        public AICategory PADAICategory { get; set; } = AICategory.Baseline;
        public ChipSlotDir GlueChipSlotDir { get; set; } = ChipSlotDir.NONE;
        /// <summary>
        /// 芯片有无检测模式
        /// </summary>
        public ChipNoHave ChipNoHaveMode { get; set; } = ChipNoHave.NONE;
        public string ChipNoHaveModeOpString { get; set; } = "";
        /// <summary>
        /// 无胶检测模式
        /// </summary>
        public ChipNoGlueMethod ChipNoGlueMode { get; set; } = ChipNoGlueMethod.NONE;

        IEzSeg m_Model = null;

        public double Resolution_Mil { get; set; } = 0.0254 / INI.MAINSD_PAD_MIL_RESOLUTION;//轉換爲 1 mil = 1.155 pixel

        public double OWidthRatio { get; set; } = 15;
        public double OHeightRatio { get; set; } = 15;
        public double OAreaRatio { get; set; } = 15;

        /// <summary>
        /// 找芯片的白色
        /// </summary>
        public bool ChipFindWhite { get; set; } = true;
        /// <summary>
        /// 找银胶黑色
        /// </summary>
        public bool GLEFindWhite { get; set; } = false;

        public int PADGrayThreshold { get; set; } = 128;
        public int PADBlobGrayThreshold { get; set; } = 128;
        public int PADChipInBlobGrayThreshold { get; set; } = 100;

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

        public double GlueTopBottomOffset { get; set; } = 0.1;//银胶上下的偏移量
        public double GlueLeftRightOffset{ get; set; } = 0.1;//银胶左右的偏移量


        public double GleWidthUpper { get; set; } = 10;
        public double GleWidthLower { get; set; } = 0;
        public double GleHeightUpper { get; set; } = 10;
        public double GleHeightLower { get; set; } = 0;
        public double GleAreaUpper { get; set; } = 10;
        public double GleAreaLower { get; set; } = 0;



        #endregion

        public double GlueMax { get; set; } = 1;//薄膜胶最大值
        public double GlueMin { get; set; } = 0.1;//薄膜胶最小值


        public bool GlueCheck { get; set; } = true;
        public double NoGlueThresholdValue { get; set; } = 0.7;
        public double BloodFillValueRatio { get; set; } = 0.33;

        public bool ChipDirlevel { get; set; } = true;

        public int FontSize { get; set; } = 35;
        public int LineWidth { get; set; } = 5;
        public bool ChipGleCheck { get; set; } = false;

        /// <summary>
        /// 检测四边无胶PASS
        /// </summary>
        public int FourSideNoGluePassValue { get; set; } = 0;//=0不检测 >0检测

        public PadInspectMethodEnum PadInspectMethod { get; set; } = PadInspectMethodEnum.NONE;
        public string PADINSPECTOPString { get; set; } = "";

        public string PADExtendOPString { get; set; } = "";

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

        //JzMVDGrayPatMatchClass m_MvdGrayPatMatch = new JzMVDGrayPatMatchClass();
        JetEazy.BasicSpace.JzFindObjectClass m_JzFind = new JetEazy.BasicSpace.JzFindObjectClass();
        PADRegionClass m_PADRegion = new PADRegionClass();
        bool m_IsSaveTemp = false;
        HistogramClass m_Histogram = new HistogramClass(2);
        JzToolsClass m_Tools = new JzToolsClass();

        JzMVDPatMatchClass mVDPatMatchClass = new JzMVDPatMatchClass();
        public JzMVDPatMatchClass EzMVDPatMatchPADG2 = new JzMVDPatMatchClass();
        public JzMVDBlobClass EzMVDBLOB = new JzMVDBlobClass();
        public PADExtendClass PADExtend = new PADExtendClass();

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
        string m_QLERunDataStr = string.Empty;
        public bool RunDataOK
        {
            get { return m_RunDataOK; }
            set { m_RunDataOK = value; }
        }
        public string QLERunDataStr
        {
            get { return m_QLERunDataStr; }
            set { m_QLERunDataStr = value; }
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
            m_IsSaveTemp = INI.IsDEBUGCHIP;
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

            if (!string.IsNullOrEmpty(PADExtendOPString))
                PADExtend.FromString(PADExtendOPString);

            switch (PADMethod)
            {
                case PADMethodEnum.QLE_CHECK:
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

                    switch(ChipNoHaveMode)
                    {
                        case ChipNoHave.Normal:

                            Rectangle _cropItem = m_PADRegion.RegionForEdgeRect;
                            _cropItem.Inflate(10, 10);
                            BoundRect(ref _cropItem, bmpPattern.Size);
                            Bitmap bmpitemx = bmpPattern.Clone(_cropItem, PixelFormat.Format24bppRgb);
                            checkNoHaveTrain(bmpitemx);
                            bmpitemx.Dispose();

                            break;
                    }

                    switch(PADMethod)
                    {
                        case PADMethodEnum.QLE_CHECK:

                            switch (PadInspectMethod)
                            {
                                case PadInspectMethodEnum.PAD_G2:
                                    RectangleF rectCrop = FindAxisAlignedBoundingRectangle(m_PADRegion.RegionPtFCornerQLE);
                                    BoundRect(ref rectCrop, bmppattern.Size);
                                    CheckYinJiaoIrregular(true, bmppattern, rectCrop);

                                    break;
                            }

                            break;
                    }

                    //m_MvdGrayPatMatch.bmpItem = new Bitmap(bmpPattern);//, new Size(bmpPattern.Width >> 1, bmpPattern.Height >> 1));
                    //m_MvdGrayPatMatch.HikTrainGray();

                    //m_MvdGrayPatMatch.bmpItem.Save("D:\\testtest\\" + RelateAnalyzeString + "bmpPattern" + ".png", System.Drawing.Imaging.ImageFormat.Png);

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
                                    //rect1.Inflate(2, 2);
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
                                    bitmap = new Bitmap(_getV8bmpInput(rect1, _from_bmpinputSize_to_iSized(imginput)));
                                    break;
                                case PADChipSize.CHIP_NORMAL_EX:

                                    bitmap = new Bitmap(_getNormalEx1(rect1));

                                    break;
                                case PADChipSize.CHIP_NORMAL_IPD_EX:
                                    break;
                                case PADChipSize.CHIP_NORMAL:
                                default:

                                    switch (PadInspectMethod)
                                    {
                                        case PadInspectMethodEnum.PAD_V1:
                                        case PadInspectMethodEnum.PAD_SMALL:
                                            checkOutGlue(bmppattern, out Bitmap bitmap1, true);

                                            //Bitmap _bmpnewIpd = new Bitmap(bitmap);
                                            GetDataIPD(bmppattern, true);
                                            //_bmpnewIpd.Dispose();

                                            bitmap = new Bitmap(bmplist[3]);

                                            break;

                                        case PadInspectMethodEnum.PAD_G1:

                                            //画出内缩外扩的位置
                                            bitmap = new Bitmap(_getNormalPADG1(rect1));

                                            break;

                                        default:
                                            bitmap = new Bitmap(_getG1bmpInput(rect1));
                                            break;
                                    }

                                    break;
                            }


                            if (PADExtend.bOpenUseContactEle)
                            {
                                Graphics grapContactEle = Graphics.FromImage(bitmap);
                                Rectangle rect2 = new Rectangle(m_PADRegion.RegionForEdgeRect.X,
                                                                                       m_PADRegion.RegionForEdgeRect.Y,
                                                                                       m_PADRegion.RegionForEdgeRect.Width,
                                                                                       m_PADRegion.RegionForEdgeRect.Height);

                                switch(PadInspectMethod)
                                {
                                    case PadInspectMethodEnum.PAD_V1:

                                        #region 使用IPD的限流外围

                                        if (!string.IsNullOrEmpty(PADINSPECTOPString))
                                        {
                                            string[] strs = PADINSPECTOPString.Split(',');
                                            if (strs.Length > 5)
                                            {
                                                GetRGB = short.Parse(strs[0]);
                                                numThresholdRatio = int.Parse(strs[1]);
                                                numObjectFilterRatio = int.Parse(strs[2]);
                                                numEDCount = int.Parse(strs[3]);
                                                numShortenRatio = int.Parse(strs[4]);
                                                txtBangBangRectStr = strs[5];
                                            }
                                            if (strs.Length > 8)
                                            {
                                                numBangBangOffsetVal = int.Parse(strs[6]);
                                                txtNeverOutsideRect = strs[7];
                                                cboIPDMethod = int.Parse(strs[8]);
                                            }
                                        }

                                        rect2 = StringtoRect(txtNeverOutsideRect);

                                        #endregion

                                        break;
                                    default:
                                        double baseResu = INI.MAINSD_PAD_MIL_RESOLUTION;
                                        if (baseResu <= 0)
                                        {
                                            baseResu = 0.03;
                                        }
                                        double _x = PADExtend.cEleX / baseResu;
                                        double _y = PADExtend.cEleY / baseResu;
                                        rect2.Inflate((int)_x, (int)_y);
                                        break;
                                }
                                grapContactEle.DrawRectangle(new Pen(Color.Yellow, LineWidth), rect2);
                                grapContactEle.Dispose();


                            }
                            bmpPadFindOutput.Dispose();
                            bmpPadFindOutput = new Bitmap(bitmap);
                            bitmap.Dispose();
                        }
                        catch
                        {

                        }
                    }

                    //switch(PADMethod)
                    //{
                    //    case PADMethodEnum.QLE_CHECK:
                    //        Bitmap bmpoutputtemp = new Bitmap(1, 1);
                    //        PADRegionFindBlob_QLE(bmpPattern, PADBlobGrayThreshold, m_PADRegion, ref bmpoutputtemp, 1, GLEFindWhite);
                    //        bmpPadFindOutput = new Bitmap(bmpoutputtemp);
                    //        bmpoutputtemp.Dispose();
                    //        break;
                    //}

                    break;
                case PADMethodEnum.GLUECHECK_BlackEdge:

                    bmpPattern = new Bitmap(bmppattern);
                    bmpMask = new Bitmap(bmpmask);

                    bmpPadFindOutput = new Bitmap(bmppattern);
                    //bmpPadBolbOutput = new Bitmap(bmppattern);

                    Rectangle rect1x = new Rectangle(m_PADRegion.RegionForEdgeRect.X,
                                                                                   m_PADRegion.RegionForEdgeRect.Y,
                                                                                   m_PADRegion.RegionForEdgeRect.Width,
                                                                                   m_PADRegion.RegionForEdgeRect.Height);

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
                                    bitmap = new Bitmap(blackAICal(m_PADRegion, -_from_bmpinputSize_to_iSized(imginput)));
                                    break;
                                case PADChipSize.CHIP_NORMAL:
                                default:

                                    switch(PadInspectMethod)
                                    {
                                        case PadInspectMethodEnum.PAD_G1:

                                            //画出内缩外扩的位置
                                            bitmap = new Bitmap(_getNormalPADG1(rect1x));

                                            break;
                                        default:
                                            bitmap = new Bitmap(blackNormal(m_PADRegion));
                                            break;
                                    }
                                    break;
                            }

                            if (PADExtend.bOpenUseContactEle)
                            {
                                Graphics grapContactEle = Graphics.FromImage(bitmap);
                                Rectangle rect2 = new Rectangle(m_PADRegion.RegionForEdgeRect.X,
                                                                                       m_PADRegion.RegionForEdgeRect.Y,
                                                                                       m_PADRegion.RegionForEdgeRect.Width,
                                                                                       m_PADRegion.RegionForEdgeRect.Height);

                                double baseResu = INI.MAINSD_PAD_MIL_RESOLUTION;
                                if (baseResu <= 0)
                                {
                                    baseResu = 0.03;
                                }
                                double _x = PADExtend.cEleX / baseResu;
                                double _y = PADExtend.cEleY / baseResu;
                                rect2.Inflate((int)_x, (int)_y);
                                grapContactEle.DrawRectangle(new Pen(Color.Yellow, LineWidth), rect2);
                                grapContactEle.Dispose();


                            }

                            bmpPadFindOutput.Dispose();
                            bmpPadFindOutput = new Bitmap(bitmap);
                            bitmap.Dispose();
                        }
                        catch
                        {

                        }
                    }


                    if (m_IsSaveTemp)
                    {
                        bmpPadFindOutput.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpPadFindOutput" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                        //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                    }


                    break;

                case PADMethodEnum.CHIPCHECKNOHAVE:
                    bmpPattern = new Bitmap(bmppattern);
                    bmpMask = new Bitmap(bmpmask);
                    bmpPadFindOutput = new Bitmap(bmppattern);

                    PB10_ChipCheckNoHaveProcess(bmpPattern, true, ref bmpPadFindOutput);

                    if (m_IsSaveTemp)
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
            if (!string.IsNullOrEmpty(PADExtendOPString))
                PADExtend.FromString(PADExtendOPString);

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
            bmpPadFindOutput = (Bitmap)bmpinput.Clone();

            #region 先判断PAD的尺寸是否正确 和 正确定位到 及 判断PAD 区域里面的blob
            //这里不需要自己自动找框了 用train的位置框
            //PADRegionClass PADTempRegion = new PADRegionClass();

            if (isgood)
            {
                #region 检测有无芯片

                switch(ChipNoHaveMode)
                {
                    case ChipNoHave.Normal:

                        Bitmap bmprunxxxx = new Bitmap(bmpinput);
                        isgood = checkNoHaveRun(bmprunxxxx);
                        bmprunxxxx.Dispose();

                        if (!isgood)
                        {
                            isgood = false;
                            processstring += "Error in FastPH" + RelateAnalyzeString + Environment.NewLine;
                            errorstring += RelateAnalyzeString + Environment.NewLine;

                            reason = ReasonEnum.PASS;
                            descstriing = "无芯片";
                        }
                        break;
                    case ChipNoHave.BlobCount:

                        Rectangle _cropItem = m_PADRegion.RegionForEdgeRect;
                        int iws = _cropItem.Width / 4;
                        int ihs = _cropItem.Height / 4;
                        _cropItem.Inflate(-iws, -ihs);
                        BoundRect(ref _cropItem, bmpinput.Size);
                        Bitmap bmpitemx = bmpinput.Clone(_cropItem, PixelFormat.Format24bppRgb);
                        //int _thresholdv = 100;
                        N2Class n2Class = new N2Class();
                        n2Class.FromString(ChipNoHaveModeOpString);
                        isgood = checkNoHaveRunBlobCount(bmpitemx, n2Class.ThresholdValue, n2Class.IsWhite, n2Class.Count);
                        bmpitemx.Dispose();

                        if (!isgood)
                        {
                            isgood = false;
                            processstring += "Error in Blob" + RelateAnalyzeString + Environment.NewLine;
                            errorstring += RelateAnalyzeString + Environment.NewLine;

                            reason = ReasonEnum.PASS;
                            descstriing = "无芯片";
                        }

                        break;
                    default:
                        break;
                }

                #endregion

                #region 检测芯片上的溢胶
                if (isgood)
                {

                    switch(PadInspectMethod)
                    {
                        case PadInspectMethodEnum.PAD_V1:
                            //78ms
                            PADRegionFindBlob(bmpInput, PADBlobGrayThreshold, m_PADRegion, ref bmpoutput, 0);
                            break;
                        default:
                            //78ms
                            PADRegionFindBlob(bmpInput, PADBlobGrayThreshold, m_PADRegion, ref bmpoutput);
                            break;
                    }

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

                #region 判断四周有无胶水

                if (isgood && FourSideNoGluePassValue > 0)
                {
                    //m_MvdGrayPatMatch.bmpFind = new Bitmap(bmpinput);//, new Size(bmpinput.Width >> 1, bmpinput.Height >> 1));
                    //isgood = m_MvdGrayPatMatch.HikRunGray();

                    //m_MvdGrayPatMatch.bmpFind.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpinput_Gray" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    Bitmap bmpFourSide = new Bitmap(bmpinput);
                    //Bitmap bmpFourSide = new Bitmap(bmpinput, new Size(bmpinput.Width >> 3, bmpinput.Height >> 3));
                    //m_Histogram.GetHistogram(bmpFourSide, 100);
                    int _mean = 0;// m_Histogram.MeanGrade;

                    //截取芯片周围一部分 找到灰阶的最低值
                    _mean = GetGrayMinValue(bmpFourSide);

                    isgood = _mean < FourSideNoGluePassValue;

                    if (!isgood)
                    {
                        if (m_IsSaveTemp)
                            bmpFourSide.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + $"bmpinput_Gray{_mean}" + ".png", System.Drawing.Imaging.ImageFormat.Png);

                        bmpoutput.Dispose();
                        bmpoutput = (Bitmap)bmpinput.Clone();

                        descstriing = "无胶";
                        reason = ReasonEnum.NG;

                        //reason = ReasonEnum.PASS;
                        //descstriing = "四周";
                    }

                    bmpFourSide.Dispose();
                }

                #endregion

                #region 判断有无胶水  VICTOR模式
                //56ms
                if (isgood || (!isgood && descstriing == "晶片表面溢胶"))
                {
                    RectangleF _checkRectF = new RectangleF();
                    JzToolsClass jzToolsClass = new JzToolsClass();
                    double iwidthtmp = Math.Max(_checkRectF.Width, _checkRectF.Height);
                    double iheighttmp = Math.Min(_checkRectF.Width, _checkRectF.Height);

                    switch (ChipNoGlueMode)
                    {
                        case ChipNoGlueMethod.NoGlueNormal:

                            #region NO GLUE NORMAL

                            _checkRectF = checkNoHaveGlue(bmpInput);

                            iwidthtmp = Math.Max(_checkRectF.Width, _checkRectF.Height);
                            iheighttmp = Math.Min(_checkRectF.Width, _checkRectF.Height);

                            if (!IsInRangeRatio(m_PADRegion.RegionWidth, iwidthtmp, OWidthRatio))
                            {
                                isgood = false;
                                processstring += "Error in " + RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + _checkRectF.Width.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;
                                errorstring += RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + _checkRectF.Width.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;
                                descstriing = "无胶";
                                reason = ReasonEnum.NG;

                                bmpoutput.Dispose();
                                //bmpoutput = (Bitmap)bmpinput.Clone();// new Bitmap(bmpinput);
                                bmpoutput = new Bitmap(bmpinput);
                                jzToolsClass.DrawRect(bmpoutput, _checkRectF, new Pen(Color.Red, 5));
                                //if (INI.CHIP_NG_SHOW)
                                //{
                                //    bmpoutput.Dispose();
                                //    bmpoutput = new Bitmap(bmpgray11);
                                //}
                            }
                            else if (!IsInRangeRatio(m_PADRegion.RegionHeight, iheighttmp, OHeightRatio))
                            {
                                isgood = false;
                                processstring += "Error in " + RelateAnalyzeString + " PAD HEIGHT OVER Ratio= " + OHeightRatio.ToString() + " , " + _checkRectF.Height.ToString() + " , " + m_PADRegion.RegionHeight.ToString() + Environment.NewLine;
                                errorstring += RelateAnalyzeString + " PAD HEIGHT OVER Ratio= " + OHeightRatio.ToString() + " , " + _checkRectF.Height.ToString() + " , " + m_PADRegion.RegionHeight.ToString() + Environment.NewLine;
                                descstriing = "无胶";
                                reason = ReasonEnum.NG;

                                bmpoutput.Dispose();
                                //bmpoutput = (Bitmap)bmpinput.Clone();// new Bitmap(bmpinput);
                                bmpoutput = new Bitmap(bmpinput);
                                jzToolsClass.DrawRect(bmpoutput, _checkRectF, new Pen(Color.Red, 5));
                                //jzToolsClass.DrawRect(bmpoutput, rect1pattrem, new Pen(Color.Red, 5));
                                //if (INI.CHIP_NG_SHOW)
                                //{
                                //    bmpoutput.Dispose();
                                //    bmpoutput = new Bitmap(bmpgray11);
                                //}
                            }

                            #endregion

                            break;
                        case ChipNoGlueMethod.NoGlueV1:

                            #region NO GLUE V1
                            _checkRectF = checkNoHaveGlueV4(bmpInput);

                            iwidthtmp = Math.Max(_checkRectF.Width, _checkRectF.Height);
                            iheighttmp = Math.Min(_checkRectF.Width, _checkRectF.Height);

                            if (!(IsInRangeRatio(m_PADRegion.RegionWidth, iwidthtmp, OWidthRatio)
                                && 
                                IsInRangeRatio(m_PADRegion.RegionHeight, iheighttmp, OHeightRatio)))
                            {
                                isgood = false;
                                processstring += "Error in " + RelateAnalyzeString + " PAD WH OVER ";// + OWidthRatio.ToString() + " , " + _checkRectF.Width.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;
                                errorstring += RelateAnalyzeString + " PAD WH OVER ";// + OWidthRatio.ToString() + " , " + _checkRectF.Width.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;
                                descstriing = "无胶";
                                reason = ReasonEnum.NG;

                                bmpoutput.Dispose();
                                bmpoutput = new Bitmap(bmpinput);
                                jzToolsClass.DrawRect(bmpoutput, _checkRectF, new Pen(Color.Red, 5));
                            }
                            #endregion

                            break;
                        case ChipNoGlueMethod.NONE:
                        default:
                            break;
                    }
                    
                }

                #endregion

                #region 用来检测碰到锡球的算法
                if (isgood || descstriing == "晶片表面溢胶")
                {
                    if (ChipGleCheck)
                    {
                        isgood = checkOutGlue(bmpInput, out Bitmap bitmapResult);
                        //if (ChipGleCheck)
                        //    isgood = PADRegionCheckSize(bmpInput, PADBlobGrayThreshold, PADTempRegion, ref bmpoutput) == 0;
                        if (!isgood)
                        {
                            isgood = false;
                            processstring += "Error in " + RelateAnalyzeString + " Glue OVER " + Environment.NewLine;
                            errorstring += RelateAnalyzeString + " Glue OVER " + Environment.NewLine;

                            bmpoutput.Dispose();
                            bmpoutput = new Bitmap(bitmapResult);

                            reason = ReasonEnum.NG;
                            descstriing = "胶水异常";
                        }
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
                    
                    bmpoutput.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpfindblob" + (false ? "_Train" : "_Run") + ".png", System.Drawing.Imaging.ImageFormat.Png);

                }
            }

            #region 胶水宽度判断
            glues = null;
            if (isgood && GlueCheck || (!isgood && descstriing == "晶片表面溢胶" && GlueCheck))
            {

                glues = new GlueRegionClass[(int)BorderTypeEnum.COUNT];
                PointF[] pts = null;
                PointF[] ptsIN = null;

                imginput = (Bitmap)bmpinput.Clone();
                imgmask = new Bitmap(bmpinput.Width, bmpinput.Height);

                Rectangle rect1 = new Rectangle(m_PADRegion.RegionForEdgeRect.X,
                                                                       m_PADRegion.RegionForEdgeRect.Y,
                                                                       m_PADRegion.RegionForEdgeRect.Width,
                                                                       m_PADRegion.RegionForEdgeRect.Height);
                rect1.Inflate(2, 2);



                Rectangle rect = new Rectangle(m_PADRegion.RegionForEdgeRect.X,
                                                                       m_PADRegion.RegionForEdgeRect.Y,
                                                                       m_PADRegion.RegionForEdgeRect.Width,
                                                                       m_PADRegion.RegionForEdgeRect.Height);


                Bitmap bitmap = (Bitmap)bmpInput.Clone();// new Bitmap(bmpInput);
                Bitmap bitmap0 = (Bitmap)bmpInput.Clone();//new Bitmap(bmpInput);
                if (m_IsSaveTemp)
                {
                    bitmap0.Save(Universal.CalTestPath + "\\" + _CalPageIndex() + RelateAnalyzeString + "bmpTrainCollect" + ".png",
                       System.Drawing.Imaging.ImageFormat.Png);
                    bitmap0.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bitmap0" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                   
                }

                int isize = _from_bmpinputSize_to_iSized(imginput);
                switch (PADChipSizeMode)
                {
                    case PADChipSize.CHIP_V1:
                        bitmap = (Bitmap)_getV1bmpInput(rect1).Clone();
                        break;
                    case PADChipSize.CHIP_V5:
                        break;
                    case PADChipSize.CHIP_NORMAL_EX:
                        break;
                    case PADChipSize.CHIP_NORMAL:
                    default:
                        //bitmap = new Bitmap(_getG1bmpInput(rect1));
                        switch (PadInspectMethod)
                        {
                            case PadInspectMethodEnum.PAD_SMALL:
                            case PadInspectMethodEnum.PAD_V1:
                            case PadInspectMethodEnum.PAD_G1:
                                break;
                            default:
                                bitmap = (Bitmap)_getG1bmpInput(rect1).Clone();
                                break;
                        }
                        break;
                }

                //这里是寻找点的起点位置
                //rect.Inflate(-5, -5);
                rect.Inflate(-(int)(CalExtendX * Resolution_Mil), -(int)(CalExtendY * Resolution_Mil));
                //BoundRect(ref rect, bitmap.Size);
                if (m_IsSaveTemp)
                {
                    bitmap.Save("D:\\testtest\\" + RelateAnalyzeString + "imginputorg" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                }

                int i = 0;
                int j = 0;

                //多线程计算4条边
                switch (PADChipSizeMode)
                {
                    case PADChipSize.CHIP_V8:

                        #region 原始的通用做法

                        if (m_IsSaveTemp)
                        {
                            m_PADRegion.bmpThreshold.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpThreshold" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                            //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                        }
                        borderLineRun = new BorderLineRunClass[4];
                        //int isize = _from_bmpinputSize_to_iSized(imginput);
                        i = 0;
                        while (i < (int)BorderTypeEnum.COUNT)
                        {

                            borderLineRun[i] = new BorderLineRunClass();
                            borderLineRun[i].bmp0 = (Bitmap)bitmap.Clone();// new Bitmap(bitmap);

                            switch (PADAICategory)
                            {
                                case AICategory.Median:
                                case AICategory.Small:
                                    //case AICategory.BigKotor:
                                    isize = 0;
                                    break;
                                default:
                                    break;
                            }

                            borderLineRun[i].bmp1 = new Bitmap(m_PADRegion.bmpThreshold,
                                                                                                    Resize(m_PADRegion.bmpThreshold.Size, isize));

                            RectangleF rectSize = ResizeWithLocation2(rect, isize);

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
                        //    switch (PADAICategory)
                        //    {
                        //        case AICategory.Median:
                        //        case AICategory.Small:
                        //        case AICategory.BigKotor:
                        //            _get_border_pointf_v8_1_blackBigtor(item.bmp0, item.bmp1, item.rect0, item.Border, out glues[item.index], -isize);
                        //            break;
                        //        default:
                        //            _get_border_pointf_v8_1(item.bmp0, item.bmp1, item.rect0, item.Border, out glues[item.index], -isize);
                        //            break;
                        //    }
                        //}
                        Parallel.ForEach(borderLineRun, item =>
                        {
                            switch (PADAICategory)
                            {
                                //case AICategory.Median:
                                //case AICategory.Small:
                                //case AICategory.BigKotor:
                                //    _get_border_pointf_v8_1_blackBigtor(item.bmp0, item.bmp1, item.rect0, item.Border, out glues[item.index], -isize);
                                //    break;
                                default:
                                    _get_border_pointf_v8_1(item.bmp0, item.bmp1, item.rect0, item.Border, out glues[item.index], -isize);
                                    break;
                            }

                            //_get_border_pointf_v8_1(item.bmp0, item.bmp1, item.rect0, item.Border, out glues[item.index], isize);
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

                        #endregion

                        break;

                   
                    case PADChipSize.CHIP_NORMAL_EX:

                        #region 原始的通用做法

                        borderLineRun = new BorderLineRunClass[4];
                        i = 0;
                        while (i < (int)BorderTypeEnum.COUNT)
                        {

                            borderLineRun[i] = new BorderLineRunClass();
                            borderLineRun[i].bmp0 = (Bitmap)bitmap.Clone();// new Bitmap(bitmap);
                            borderLineRun[i].bmp1 = (Bitmap)m_PADRegion.bmpThreshold.Clone();//  new Bitmap(PADTempRegion.bmpThreshold);
                            borderLineRun[i].rect0 = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                            borderLineRun[i].Border = (BorderTypeEnum)i;
                            borderLineRun[i].index = i;

                            i++;
                        }

                        Parallel.ForEach(borderLineRun, item =>
                        {
                            _get_border_pointf_NormalEx(item.bmp0, item.bmp1, item.rect0, item.Border, out glues[item.index]);

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

                        #endregion

                        break;
                    case PADChipSize.CHIP_V1:

                        #region 原始的通用做法

                        borderLineRun = new BorderLineRunClass[4];
                        i = 0;
                        while (i < (int)BorderTypeEnum.COUNT)
                        {

                            borderLineRun[i] = new BorderLineRunClass();
                            borderLineRun[i].bmp0 = (Bitmap)bitmap.Clone();// new Bitmap(bitmap);
                            borderLineRun[i].bmp1 = (Bitmap)m_PADRegion.bmpThreshold.Clone();//  new Bitmap(PADTempRegion.bmpThreshold);
                            borderLineRun[i].rect0 = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                            borderLineRun[i].Border = (BorderTypeEnum)i;
                            borderLineRun[i].index = i;

                            i++;
                        }

                        Parallel.ForEach(borderLineRun, item =>
                        {
                            _get_border_pointf_v1_antu(item.bmp0, item.bmp1, item.rect0, item.Border, out glues[item.index]);

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

                        #endregion

                        break;
                    case PADChipSize.CHIP_NORMAL:
                    default:
                        switch (PadInspectMethod)
                        {
                            case PadInspectMethodEnum.PAD_SMALL:

                                #region VICTOR_PAD_SMALL

                                Bitmap bmpin0 = new Bitmap(bitmap);
                                Bitmap bmpout0 = new Bitmap(bitmap);

                                GetDataHXIn(bmpin0);
                                GetDataHX(bmpout0);

                                bmpin0.Dispose();
                                bmpout0.Dispose();

                                i = 0;
                                while (i < (int)SIDEEmnum.COUNT)
                                {
                                    switch ((SIDEEmnum)i)
                                    {
                                        case SIDEEmnum.TOP:
                                            JzFourSideCal(out glues[(int)BorderTypeEnum.TOP], sideAnalyzeIN.SIDEData[i], sideAnalyze.SIDEData[i]);
                                            break;
                                        case SIDEEmnum.BOTTOM:
                                            JzFourSideCal(out glues[(int)BorderTypeEnum.BOTTOM], sideAnalyzeIN.SIDEData[i], sideAnalyze.SIDEData[i]);
                                            break;
                                        case SIDEEmnum.LEFT:
                                            JzFourSideCal(out glues[(int)BorderTypeEnum.LEFT], sideAnalyzeIN.SIDEData[i], sideAnalyze.SIDEData[i]);
                                            break;
                                        case SIDEEmnum.RIGHT:
                                            JzFourSideCal(out glues[(int)BorderTypeEnum.RIGHT], sideAnalyzeIN.SIDEData[i], sideAnalyze.SIDEData[i]);
                                            break;
                                    }

                                    i++;
                                }

                                #endregion

                                break;
                            case PadInspectMethodEnum.PAD_V1:

                                #region VICTOR_PAD_V1

                                Bitmap _bmpnewIpd = new Bitmap(bitmap);
                                //填充中间的芯片位置
                                Graphics _gPADV1 = Graphics.FromImage(_bmpnewIpd);
                                _gPADV1.FillPolygon(Brushes.White, m_PADRegion.GetConner(-0.5f));
                                //_gPADV1.FillPolygon(Brushes.White, m_PADRegion.RegionPtFCorner);
                                //_gPADV1.FillPolygon(Brushes.White, m_PADRegion.RegionPtFCornerORG);
                                _gPADV1.Dispose();

                                if (m_IsSaveTemp)
                                {
                                    _bmpnewIpd.Save("D:\\testtest\\" + RelateAnalyzeString + "_padv1_org" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                                }

                                GetDataIPD(_bmpnewIpd, false);
                                _bmpnewIpd.Dispose();

                                i = 0;
                                while (i < (int)SIDEEmnum.COUNT)
                                {
                                    switch ((SIDEEmnum)i)
                                    {
                                        case SIDEEmnum.TOP:
                                            JzFourSideCalV1(out glues[(int)BorderTypeEnum.TOP], jzsideanalyzeex, SIDEEmnum.TOP, GlueChipSlotDir == ChipSlotDir.SlotTop);
                                            break;
                                        case SIDEEmnum.BOTTOM:
                                            JzFourSideCalV1(out glues[(int)BorderTypeEnum.BOTTOM], jzsideanalyzeex, SIDEEmnum.BOTTOM, GlueChipSlotDir == ChipSlotDir.SlotBottom);
                                            break;
                                        case SIDEEmnum.LEFT:
                                            JzFourSideCalV1(out glues[(int)BorderTypeEnum.LEFT], jzsideanalyzeex, SIDEEmnum.LEFT, GlueChipSlotDir == ChipSlotDir.SlotLeft);
                                            break;
                                        case SIDEEmnum.RIGHT:
                                            JzFourSideCalV1(out glues[(int)BorderTypeEnum.RIGHT], jzsideanalyzeex, SIDEEmnum.RIGHT, GlueChipSlotDir == ChipSlotDir.SlotRight);
                                            break;
                                    }

                                    i++;
                                }

                                #endregion

                                break;

                            case PadInspectMethodEnum.PAD_G1:

                                #region 寻找间距的做法

                                borderLineRun = new BorderLineRunClass[4];
                                i = 0;
                                while (i < (int)BorderTypeEnum.COUNT)
                                {

                                    borderLineRun[i] = new BorderLineRunClass();
                                    borderLineRun[i].bmp0 = (Bitmap)bitmap.Clone();// new Bitmap(bitmap);
                                    borderLineRun[i].bmp1 = (Bitmap)bitmap.Clone();
                                    //borderLineRun[i].bmp1 = (Bitmap)PADTempRegion.bmpThreshold.Clone();//  new Bitmap(PADTempRegion.bmpThreshold);
                                    borderLineRun[i].rect0 = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                                    borderLineRun[i].Border = (BorderTypeEnum)i;
                                    borderLineRun[i].index = i;

                                    borderLineRun[i].rectORG = new Rectangle(m_PADRegion.RegionForEdgeRect.X,
                                                                       m_PADRegion.RegionForEdgeRect.Y,
                                                                       m_PADRegion.RegionForEdgeRect.Width,
                                                                       m_PADRegion.RegionForEdgeRect.Height);
                                    borderLineRun[i].padG1 = new PADG1Class();
                                    borderLineRun[i].padG1.FromString(PADINSPECTOPString);

                                    i++;
                                }

                                //这里写间距的算法

                                Parallel.ForEach(borderLineRun, item =>
                                {
                                    _get_border_pointf_PADG1(item.bmp0,
                                        item.bmp1,
                                        item.rect0,
                                        item.rectORG,
                                        item.padG1,
                                        item.Border,
                                        out glues[item.index]);

                                });

                                //foreach (var item in borderLineRun)
                                //{
                                //    _get_border_pointf_PADG1(item.bmp0,
                                //        item.bmp1,
                                //        item.rect0,
                                //        item.rectORG,
                                //        item.padG1,
                                //        item.Border,
                                //        out glues[item.index]);
                                //}

                                i = 0;
                                while (i < (int)BorderTypeEnum.COUNT)
                                {

                                    //borderLineRun[i] = new BorderLineRunClass();
                                    borderLineRun[i].bmp0.Dispose();
                                    borderLineRun[i].bmp1.Dispose();

                                    i++;
                                }
                                GC.Collect();

                                //_bmpnewIpd.Dispose();

                                #endregion

                                break;

                            default:

                                #region 原始的通用做法

                                borderLineRun = new BorderLineRunClass[4];
                                i = 0;
                                while (i < (int)BorderTypeEnum.COUNT)
                                {

                                    borderLineRun[i] = new BorderLineRunClass();
                                    borderLineRun[i].bmp0 = (Bitmap)bitmap.Clone();// new Bitmap(bitmap);
                                    borderLineRun[i].bmp1 = (Bitmap)m_PADRegion.bmpThreshold.Clone();//  new Bitmap(PADTempRegion.bmpThreshold);
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

                                #endregion

                                break;
                        }

                        break;
                }


                Bitmap bmpglueout = new Bitmap(bmpoutput);
                //Bitmap bmpglueout = new Bitmap(bmpinput);//BAK
                Graphics g = Graphics.FromImage(bmpglueout);
                bool m_ischeckgluepass = true;
                string ngstr = string.Empty;
                string measureStr = string.Empty;
                bool _bNoInspect = false;
                bool _bSlot = false;

                double GlueTmpMax = 0;// GlueMax * Resolution_Mil;
                double GlueTmpMin = 0;// GlueMin * Resolution_Mil;

                double GlueSlotTmpMax = GlueMax / INI.MAINSD_PAD_MIL_RESOLUTION;
                double GlueSlotTmpMin = GlueMin / INI.MAINSD_PAD_MIL_RESOLUTION;

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
                    Random random = new Random();
                    random.Next(0, 100);
                    double rd_0609 = random.Next(0, 100) * 0.00001;

                    double min = glues[i].LengthMin * INI.MAINSD_PAD_MIL_RESOLUTION- rd_0609;
                    double max = glues[i].LengthMax * INI.MAINSD_PAD_MIL_RESOLUTION- rd_0609;

                    double minSlot = glues[i].LengthSlotMin * INI.MAINSD_PAD_MIL_RESOLUTION- rd_0609;
                    double maxSlot = glues[i].LengthSlotMax * INI.MAINSD_PAD_MIL_RESOLUTION- rd_0609;
                    bool isSlotPass = true;

                    string format = "0.0000";

                    switch ((BorderTypeEnum)i)
                    {
                        case BorderTypeEnum.LEFT:
                            _bNoInspect = PADExtend.bNoInspectLeft;
                            _bSlot = GlueChipSlotDir == ChipSlotDir.SlotLeft;
                            measureStr += "左";// + Environment.NewLine;
                            //measureStr += "[min:" + min.ToString(format) + " mm]";
                            //measureStr += "[max:" + max.ToString(format) + " mm]" + Environment.NewLine;

                            //if (GlueChipSlotDir == ChipSlotDir.SlotLeft)
                            //{
                            //    measureStr += "薄膜胶";// + Environment.NewLine;
                            //    measureStr += "[min:" + minSlot.ToString(format) + " mm]";
                            //    measureStr += "[max:" + maxSlot.ToString(format) + " mm]" + Environment.NewLine;

                            //    isSlotPass = (glues[i].LengthSlotMax <= (GlueSlotTmpMax) && glues[i].LengthSlotMin >= (GlueSlotTmpMin));
                            //}

                            break;
                        case BorderTypeEnum.RIGHT:
                            _bNoInspect = PADExtend.bNoInspectRight;
                            _bSlot = GlueChipSlotDir == ChipSlotDir.SlotRight;
                            measureStr += "右";// + Environment.NewLine;
                            //measureStr += "[min:" + min.ToString(format) + " mm]";
                            //measureStr += "[max:" + max.ToString(format) + " mm]" + Environment.NewLine;

                            //if (GlueChipSlotDir == ChipSlotDir.SlotRight)
                            //{
                            //    measureStr += "薄膜胶";// + Environment.NewLine;
                            //    measureStr += "[min:" + minSlot.ToString(format) + " mm]";
                            //    measureStr += "[max:" + maxSlot.ToString(format) + " mm]" + Environment.NewLine;

                            //    isSlotPass = (glues[i].LengthSlotMax <= (GlueSlotTmpMax) && glues[i].LengthSlotMin >= (GlueSlotTmpMin));
                            //}

                            break;
                        case BorderTypeEnum.TOP:
                            _bNoInspect = PADExtend.bNoInspectTop;
                            _bSlot = GlueChipSlotDir == ChipSlotDir.SlotTop;
                            measureStr += "上";// + Environment.NewLine;
                            //measureStr += "[min:" + min.ToString(format) + " mm]";
                            //measureStr += "[max:" + max.ToString(format) + " mm]" + Environment.NewLine;

                            //if (GlueChipSlotDir == ChipSlotDir.SlotTop)
                            //{
                            //    measureStr += "薄膜胶";// + Environment.NewLine;
                            //    measureStr += "[min:" + minSlot.ToString(format) + " mm]";
                            //    measureStr += "[max:" + maxSlot.ToString(format) + " mm]" + Environment.NewLine;

                            //    isSlotPass = (glues[i].LengthSlotMax <= (GlueSlotTmpMax) && glues[i].LengthSlotMin >= (GlueSlotTmpMin));
                            //}

                            break;
                        case BorderTypeEnum.BOTTOM:
                            _bNoInspect = PADExtend.bNoInspectBottom;
                            _bSlot = GlueChipSlotDir == ChipSlotDir.SlotBottom;
                            measureStr += "下";// + Environment.NewLine;
                            //measureStr += "[min:" + min.ToString(format) + " mm]";
                            //measureStr += "[max:" + max.ToString(format) + " mm]" + Environment.NewLine;

                            //if (GlueChipSlotDir == ChipSlotDir.SlotBottom)
                            //{
                            //    measureStr += "薄膜胶";// + Environment.NewLine;
                            //    measureStr += "[min:" + minSlot.ToString(format) + " mm]";
                            //    measureStr += "[max:" + maxSlot.ToString(format) + " mm]" + Environment.NewLine;

                            //    isSlotPass = (glues[i].LengthSlotMax <= (GlueSlotTmpMax) && glues[i].LengthSlotMin >= (GlueSlotTmpMin));
                            //}

                            break;
                    }


                    if (_bNoInspect)
                    {
                        measureStr += "[min:no measure]";
                        measureStr += "[max:no measure]" + Environment.NewLine;
                        isSlotPass = true;
                    }
                    else
                    {
                        measureStr += "[min:" + min.ToString(format) + " mm]";
                        measureStr += "[max:" + max.ToString(format) + " mm]" + Environment.NewLine;
                        if (_bSlot)
                        {
                            measureStr += "薄膜胶";// + Environment.NewLine;
                            measureStr += "[min:" + minSlot.ToString(format) + " mm]";
                            measureStr += "[max:" + maxSlot.ToString(format) + " mm]" + Environment.NewLine;

                            isSlotPass = (glues[i].LengthSlotMax <= (GlueSlotTmpMax) && glues[i].LengthSlotMin >= (GlueSlotTmpMin));
                        }
                    }


                    if (glues[i].LengthMax > (GlueTmpMax) || glues[i].LengthMin < (GlueTmpMin))
                    {
                        m_ischeckgluepass = false;

                        switch ((BorderTypeEnum)i)
                        {
                            case BorderTypeEnum.LEFT:
                                ngstr += "左";
                                //ngstr += "[min:" + min.ToString(format) + " mm]";
                                //ngstr += "[max:" + max.ToString(format) + " mm]";
                                break;
                            case BorderTypeEnum.RIGHT:
                                ngstr += "右";
                                //ngstr += "[min:" + min.ToString(format) + " mm]";
                                //ngstr += "[max:" + max.ToString(format) + " mm]";
                                break;
                            case BorderTypeEnum.TOP:
                                ngstr += "上";
                                //ngstr += "[min:" + min.ToString(format) + " mm]";
                                //ngstr += "[max:" + max.ToString(format) + " mm]";
                                break;
                            case BorderTypeEnum.BOTTOM:
                                ngstr += "下";
                                //ngstr += "[min:" + min.ToString(format) + " mm]";
                                //ngstr += "[max:" + max.ToString(format) + " mm]";
                                break;
                        }
                    }

                    if(_bNoInspect)
                    {
                        ngstr += "[min:no measure]";
                        ngstr += "[max:no measure]" + Environment.NewLine;
                        m_ischeckgluepass = true;
                    }
                    else
                    {
                        ngstr += "[min:" + min.ToString(format) + " mm]";
                        ngstr += "[max:" + max.ToString(format) + " mm]";
                        if (_bSlot && !isSlotPass)
                        {
                            m_ischeckgluepass = false;

                            ngstr += "薄膜胶";// + Environment.NewLine;
                            ngstr += "[min:" + minSlot.ToString(format) + " mm]";
                            ngstr += "[max:" + maxSlot.ToString(format) + " mm]";
                        }
                    }

                    //if (GlueChipSlotDir != ChipSlotDir.NONE && !isSlotPass)
                    //{
                    //    m_ischeckgluepass = false;

                    //    ngstr += "薄膜胶";// + Environment.NewLine;
                    //    ngstr += "[min:" + minSlot.ToString(format) + " mm]";
                    //    ngstr += "[max:" + maxSlot.ToString(format) + " mm]";
                    //}

                    i++;
                }

                //填写数据的区域
                RectangleF _rectF = new RectangleF(0, 0, 1100, 290);
                _rectF = new RectangleF(0, 0, 1100, 40);
                //g.FillRectangle(Brushes.Black, _rectF);
                int linewidth = LineWidth;
                int fontsize = FontSize;

                if (PADExtend.bOpenUseContactEle)
                {
                    Rectangle org = new Rectangle(m_PADRegion.RegionForEdgeRect.X,
                                                                        m_PADRegion.RegionForEdgeRect.Y,
                                                                        m_PADRegion.RegionForEdgeRect.Width,
                                                                        m_PADRegion.RegionForEdgeRect.Height);

                    switch(PadInspectMethod)
                    {
                        case PadInspectMethodEnum.PAD_V1:

                            #region 使用IPD的限流外围

                            if (!string.IsNullOrEmpty(PADINSPECTOPString))
                            {
                                string[] strs = PADINSPECTOPString.Split(',');
                                if (strs.Length > 5)
                                {
                                    GetRGB = short.Parse(strs[0]);
                                    numThresholdRatio = int.Parse(strs[1]);
                                    numObjectFilterRatio = int.Parse(strs[2]);
                                    numEDCount = int.Parse(strs[3]);
                                    numShortenRatio = int.Parse(strs[4]);
                                    txtBangBangRectStr = strs[5];
                                }
                                if (strs.Length > 8)
                                {
                                    numBangBangOffsetVal = int.Parse(strs[6]);
                                    txtNeverOutsideRect = strs[7];
                                    cboIPDMethod = int.Parse(strs[8]);
                                }
                            }

                            org = StringtoRect(txtNeverOutsideRect);

                            #endregion

                            break;
                        default:

                            //if(PADExtend.cEleX > 0 && PADExtend.cEleY >0)
                            double baseResu = INI.MAINSD_PAD_MIL_RESOLUTION;
                            if (baseResu <= 0)
                            {
                                baseResu = 0.03;
                            }
                            double _x = PADExtend.cEleX / baseResu;
                            double _y = PADExtend.cEleY / baseResu;
                            org.Inflate((int)_x, (int)_y);

                            break;
                    }

                    bool bInstert = true;
                    //判别边缘点都在范围内
                    for (int glueindex = 0; glueindex < 4; glueindex++)
                    {
                        bool a0 = true;
                        switch ((BorderTypeEnum)glueindex)
                        {
                            case BorderTypeEnum.LEFT:
                                a0 = PADExtend.bNoInspectLeft;
                                break;
                            case BorderTypeEnum.RIGHT:
                                a0 = PADExtend.bNoInspectRight;
                                break;
                            case BorderTypeEnum.TOP:
                                a0 = PADExtend.bNoInspectTop;
                                break;
                            case BorderTypeEnum.BOTTOM:
                                a0 = PADExtend.bNoInspectBottom;
                                break;
                        }
                        if (a0) //不检测的话跳过判断碰到元件
                            continue;

                        foreach (var pt in glues[glueindex].GetPointF())
                        {
                            RectangleF r0 = SimpleRectF(pt, 1, 1);
                            bInstert = r0.IntersectsWith(org);
                            if (!bInstert)
                            {
                                break;
                            }
                        }
                        if (!bInstert)
                        {
                            break;
                        }
                    }

                    if (!bInstert)
                    {
                        m_ischeckgluepass = false;
                        //measureStr += "";
                        measureStr += $"胶水碰到元件NG" + Environment.NewLine;
                        ngstr += $"胶水碰到元件NG" + Environment.NewLine;
                    }

                    if (!m_ischeckgluepass)
                    {
                        g.DrawRectangle(new Pen(Color.Yellow, linewidth), org);
                    }
                }

                //画图 及 显示比对图
                i = 0;
                int drawIndex = 0;
                while (drawIndex < (int)BorderTypeEnum.COUNT)
                //Parallel.For(0, (int)BorderTypeEnum.COUNT, (drawIndex) =>
                {
                    if (m_ischeckgluepass)
                    {
                        g.DrawLines(new Pen(INI.chipPassColor, linewidth), glues[drawIndex].GetPointF());
                        g.DrawLines(new Pen(INI.chipPassColor, linewidth), glues[drawIndex].GetPointFIN());
                        g.DrawString(measureStr, new Font("宋体", fontsize), new SolidBrush(INI.chipPassColor), 2, 2);
                        //g.DrawString(measureStr, new Font("宋体", 22), Brushes.Lime, _rectF);
                        //g.DrawPolygon(new Pen(INI.chipPassColor, linewidth), m_PADRegion.GetConner());
                        switch (PADChipSizeMode)
                        {
                            case PADChipSize.CHIP_NORMAL:
                                switch (PadInspectMethod)
                                {
                                    case PadInspectMethodEnum.PAD_V1:
                                        if (GlueChipSlotDir != ChipSlotDir.NONE)
                                            g.DrawLines(new Pen(INI.chipPassColor, linewidth), glues[drawIndex].GetPointFIN2());
                                        break;
                                }
                                break;
                        }
                    }
                    else
                    {
                        g.DrawLines(new Pen(Color.Red, linewidth), glues[drawIndex].GetPointF());
                        g.DrawLines(new Pen(Color.Red, linewidth), glues[drawIndex].GetPointFIN());
                        g.DrawString(measureStr, new Font("宋体", fontsize), Brushes.Red, 2, 2);
                        //g.DrawString(measureStr, new Font("宋体", 22), Brushes.Red, _rectF);
                        //g.DrawPolygon(new Pen(Color.Red, linewidth), m_PADRegion.GetConner());
                        switch (PADChipSizeMode)
                        {
                            case PADChipSize.CHIP_NORMAL:
                                switch (PadInspectMethod)
                                {
                                    case PadInspectMethodEnum.PAD_V1:
                                        if (GlueChipSlotDir != ChipSlotDir.NONE)
                                            g.DrawLines(new Pen(Color.Red, linewidth), glues[drawIndex].GetPointFIN2());
                                        break;
                                }
                                break;
                        }
                    }

                    drawIndex++;
                }
                //);

                

                if (m_IsSaveTemp)
                {
                    //g.DrawRectangle(new Pen(Color.Red), rect);

                    bmpglueout.Save(Universal.CalTestPath + "\\" + _CalPageIndex() + RelateAnalyzeString + "bmpInputout" + ".png",
                        System.Drawing.Imaging.ImageFormat.Png);

                    bmpglueout.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpInputout" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                }
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

            if (descstriing == "四周")
            {
                isgood = true;
                IsPass = isgood;
                m_DescStr = string.Empty;
            }
                
            bmpMeasureOutput.Dispose();
            bmpMeasureOutput = new Bitmap(bmpoutput);

            bmpInput.Dispose();

            return isgood;
        }

        #region BAK_20250517
#if BAK_20250517

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
            if (isgood && false)
            {
                int isizeDispening = _from_bmpinputSize_to_iSized(bmpinput);

                Bitmap bmpsize = new Bitmap(bmpinput, Resize(bmpinput.Size, isizeDispening));
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

                RectangleF rectangletemp2 = ResizeWithLocation2(rectangletemp, -isizeDispening);

                double iwidthtmp = Math.Max(rectangletemp2.Width, rectangletemp2.Height);
                double iheighttmp = Math.Min(rectangletemp2.Width, rectangletemp2.Height);

                //为了节省时间这里无胶不画图
                //Rectangle rect1pattrem = new Rectangle(m_PADRegion.RegionForEdgeRect.X,
                //                                                          m_PADRegion.RegionForEdgeRect.Y,
                //                                                          m_PADRegion.RegionForEdgeRect.Width,
                //                                                          m_PADRegion.RegionForEdgeRect.Height);

                JzToolsClass jzToolsClass = new JzToolsClass();

                if (!IsInRangeRatio(m_PADRegion.RegionWidth, iwidthtmp, OWidthRatio))
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + rectangletemp2.Width.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + rectangletemp2.Width.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;
                    descstriing = "无胶";
                    reason = ReasonEnum.NG;

                    bmpoutput.Dispose();
                    //bmpoutput = (Bitmap)bmpinput.Clone();// new Bitmap(bmpinput);
                    bmpoutput = new Bitmap(bmpinput);
                    jzToolsClass.DrawRect(bmpoutput, rectangletemp2, new Pen(Color.Red, 5));
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
                    //bmpoutput = (Bitmap)bmpinput.Clone();// new Bitmap(bmpinput);
                    bmpoutput = new Bitmap(bmpinput);
                    jzToolsClass.DrawRect(bmpoutput, rectangletemp2, new Pen(Color.Red, 5));
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

                //if (!IsInRangeRatio(PADTempRegion.RegionArea, m_PADRegion.RegionArea, OAreaRatio))
                //{
                //    isgood = false;
                //    processstring += "Error in " + RelateAnalyzeString + " PAD AREA OVER Ratio= " + OAreaRatio.ToString() + " , " + PADTempRegion.RegionArea.ToString() + " , " + m_PADRegion.RegionArea.ToString() + Environment.NewLine;
                //    errorstring += RelateAnalyzeString + " PAD AREA OVER Ratio= " + OAreaRatio.ToString() + " , " + PADTempRegion.RegionArea.ToString() + " , " + m_PADRegion.RegionArea.ToString() + Environment.NewLine;

                //    //reason = ReasonEnum.NG;
                //    //descstriing = "尺寸面积超标";

                //    reason = ReasonEnum.PASS;
                //    descstriing = "无芯片";
                //}
                if (!IsInRangeRatio(PADTempRegion.RegionWidth, m_PADRegion.RegionWidth, OWidthRatio))
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + PADTempRegion.RegionWidth.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + PADTempRegion.RegionWidth.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;

                    //reason = ReasonEnum.NG;
                    //descstriing = "尺寸宽度超标";

                    reason = ReasonEnum.PASS;
                    descstriing = "无芯片";
                }
                else if (!IsInRangeRatio(PADTempRegion.RegionHeight, m_PADRegion.RegionHeight, OHeightRatio))
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " PAD HEIGHT OVER Ratio= " + OHeightRatio.ToString() + " , " + PADTempRegion.RegionHeight.ToString() + " , " + m_PADRegion.RegionHeight.ToString() + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " PAD HEIGHT OVER Ratio= " + OHeightRatio.ToString() + " , " + PADTempRegion.RegionHeight.ToString() + " , " + m_PADRegion.RegionHeight.ToString() + Environment.NewLine;

                    //reason = ReasonEnum.NG;
                    //descstriing = "尺寸高度超标";

                    reason = ReasonEnum.PASS;
                    descstriing = "无芯片";
                }

                #region 判断四周有无胶水

                if (isgood && FourSideNoGluePassValue > 0)
                {
                    //m_MvdGrayPatMatch.bmpFind = new Bitmap(bmpinput);//, new Size(bmpinput.Width >> 1, bmpinput.Height >> 1));
                    //isgood = m_MvdGrayPatMatch.HikRunGray();

                    //m_MvdGrayPatMatch.bmpFind.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpinput_Gray" + ".png", System.Drawing.Imaging.ImageFormat.Png);

                    Bitmap bmpFourSide = new Bitmap(bmpinput, new Size(bmpinput.Width >> 3, bmpinput.Height >> 3));
                    m_Histogram.GetHistogram(bmpFourSide, 100);
                    int _mean = m_Histogram.MeanGrade;

                    isgood = _mean < FourSideNoGluePassValue;

                    if (!isgood)
                    {
                        if (m_IsSaveTemp)
                            bmpFourSide.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + $"bmpinput_Gray{_mean}" + ".png", System.Drawing.Imaging.ImageFormat.Png);

                        bmpoutput.Dispose();
                        bmpoutput = (Bitmap)bmpinput.Clone();

                        reason = ReasonEnum.PASS;
                        descstriing = "四周";
                    }

                    bmpFourSide.Dispose();
                }

                #endregion

                #region 判断有无胶水  VICTOR模式
                //56ms
                if (isgood)
                {
                    int isizeDispening = _from_bmpinputSize_to_iSized(bmpinput);

                    Bitmap bmpsize = new Bitmap(bmpinput, Resize(bmpinput.Size, isizeDispening));
                    //AForge.Imaging.Filters.HistogramEqualization histogramEqualization11 =
                    //    new AForge.Imaging.Filters.HistogramEqualization();
                    //Bitmap bmphistogramEqualization11 = histogramEqualization11.Apply(bmpsize);

                    //AForge.Imaging.Filters.ContrastStretch contrastStretch = new AForge.Imaging.Filters.ContrastStretch();
                    //bmphistogramEqualization11 = contrastStretch.Apply(bmphistogramEqualization11);

                    Bitmap bmphistogramEqualization11 = new Bitmap(bmpsize);
                    switch (PadInspectMethod)
                    {
                        case PadInspectMethodEnum.PAD_V1:
                            bmphistogramEqualization11 = filterVictor002(bmphistogramEqualization11, NoGlueThresholdValue);
                            break;
                        default:
                            filterORG001(bmphistogramEqualization11);
                            //bmphistogramEqualization11 = FloodFill(bmphistogramEqualization11, new Point(bmphistogramEqualization11.Width / 2, bmphistogramEqualization11.Height / 2),
                            //                                                    Color.White, (int)(255 * NoGlueThresholdValue));

                            //AForge.Imaging.Filters.Grayscale grayscale11 =
                            //    new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
                            //bmphistogramEqualization11 = grayscale11.Apply(bmphistogramEqualization11);
                            break;
                    }

                    if (m_IsSaveTemp)
                    {
                        bmphistogramEqualization11.Save("D:\\testtest\\" + _CalPageIndex() +
                            RelateAnalyzeString + "NoDispensingOrg" + ".png", System.Drawing.Imaging.ImageFormat.Png);

                    }

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

                    RectangleF rectangletemp2 = ResizeWithLocation2(rectangletemp, -isizeDispening);

                    double iwidthtmp = Math.Max(rectangletemp2.Width, rectangletemp2.Height);
                    double iheighttmp = Math.Min(rectangletemp2.Width, rectangletemp2.Height);

                    //为了节省时间这里无胶不画图
                    //Rectangle rect1pattrem = new Rectangle(m_PADRegion.RegionForEdgeRect.X,
                    //                                                          m_PADRegion.RegionForEdgeRect.Y,
                    //                                                          m_PADRegion.RegionForEdgeRect.Width,
                    //                                                          m_PADRegion.RegionForEdgeRect.Height);

                    JzToolsClass jzToolsClass = new JzToolsClass();

                    if (!IsInRangeRatio(m_PADRegion.RegionWidth, iwidthtmp, OWidthRatio))
                    {
                        isgood = false;
                        processstring += "Error in " + RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + rectangletemp2.Width.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;
                        errorstring += RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + rectangletemp2.Width.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;
                        descstriing = "无胶";
                        reason = ReasonEnum.NG;

                        bmpoutput.Dispose();
                        //bmpoutput = (Bitmap)bmpinput.Clone();// new Bitmap(bmpinput);
                        bmpoutput = new Bitmap(bmpinput);
                        jzToolsClass.DrawRect(bmpoutput, rectangletemp2, new Pen(Color.Red, 5));
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
                        //bmpoutput = (Bitmap)bmpinput.Clone();// new Bitmap(bmpinput);
                        bmpoutput = new Bitmap(bmpinput);
                        jzToolsClass.DrawRect(bmpoutput, rectangletemp2, new Pen(Color.Red, 5));
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

                #region 判断PAD 区域里面的blob
                if (isgood)
                {
                    if (ChipGleCheck)
                        isgood = PADRegionCheckSize(bmpInput, PADBlobGrayThreshold, PADTempRegion, ref bmpoutput) == 0;
                    if (!isgood)
                    {
                        isgood = false;
                        processstring += "Error in " + RelateAnalyzeString + " Glue OVER " + Environment.NewLine;
                        errorstring += RelateAnalyzeString + " Glue OVER " + Environment.NewLine;

                        reason = ReasonEnum.NG;
                        descstriing = "胶水异常";
                    }
                }

                if (isgood)
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

                    bmpoutput.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpfindblob" + (false ? "_Train" : "_Run") + ".png", System.Drawing.Imaging.ImageFormat.Png);


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

            glues = null; //if (isgood && GlueCheck && descstriing == "晶片表面溢胶")
            if (isgood && GlueCheck || (!isgood && descstriing == "晶片表面溢胶" && GlueCheck))
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

                int isize = _from_bmpinputSize_to_iSized(imginput);
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
                        bitmap = new Bitmap(_getV8bmpInput(rect1, isize));

                        break;
                    case PADChipSize.CHIP_V5:
                        break;
                    case PADChipSize.CHIP_NORMAL_EX:

                        break;
                    case PADChipSize.CHIP_NORMAL:
                    default:
                        //bitmap = new Bitmap(_getG1bmpInput(rect1));
                        switch (PadInspectMethod)
                        {
                            case PadInspectMethodEnum.PAD_SMALL:
                            case PadInspectMethodEnum.PAD_V1:
                                break;
                            default:
                                bitmap = (Bitmap)_getG1bmpInput(rect1).Clone();
                                break;
                        }
                        break;
                }

                //这里是寻找点的起点位置
                //rect.Inflate(-5, -5);
                rect.Inflate(-(int)(CalExtendX * Resolution_Mil), -(int)(CalExtendY * Resolution_Mil));
                //BoundRect(ref rect, bitmap.Size);
                if (m_IsSaveTemp)
                {
                    bitmap.Save("D:\\testtest\\" + RelateAnalyzeString + "imginputorg" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                }

                //Rectangle rectin = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                ////rectin.Inflate(5, 5);
                //int minin = (int)(GlueMin * Resolution_Mil);
                //rectin.Inflate(minin, minin);
                ////g.DrawRectangle(new Pen(Color.Blue), rectin);

                //Rectangle rectout = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                ////rectout.Inflate(5, 5);
                //int minout = (int)(GlueMax * Resolution_Mil);
                //rectout.Inflate(minout, minout);
                ////g.DrawRectangle(new Pen(Color.Yellow), rectout);


                switch (PADChipSizeMode)
                {
                    case PADChipSize.CHIP_V8:
                        //RectangleF rectangleFxxx = ResizeWithLocation2(rectout, -2);
                        //gx.DrawRectangle(new Pen(Color.Yellow), rectangleFxxx.X, rectangleFxxx.Y,
                        //    rectangleFxxx.Width, rectangleFxxx.Height);
                        break;
                    default:

                        switch (PadInspectMethod)
                        {
                            case PadInspectMethodEnum.PAD_SMALL:
                            case PadInspectMethodEnum.PAD_V1:
                                break;
                            default:
                                //Graphics gx = Graphics.FromImage(bitmap);
                                //gx.DrawRectangle(new Pen(Color.Yellow), rectout);
                                //gx.Dispose();
                                break;
                        }
                        break;
                }

                int i = 0;
                int j = 0;

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

                        #region 原始的通用做法

                        if (m_IsSaveTemp)
                        {
                            PADTempRegion.bmpThreshold.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpThreshold" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                            //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                        }
                        borderLineRun = new BorderLineRunClass[4];
                        //int isize = _from_bmpinputSize_to_iSized(imginput);
                        i = 0;
                        while (i < (int)BorderTypeEnum.COUNT)
                        {

                            borderLineRun[i] = new BorderLineRunClass();
                            borderLineRun[i].bmp0 = (Bitmap)bitmap.Clone();// new Bitmap(bitmap);

                            switch (PADAICategory)
                            {
                                case AICategory.Median:
                                case AICategory.Small:
                                    //case AICategory.BigKotor:
                                    isize = 0;
                                    break;
                                default:
                                    break;
                            }

                            //switch (PADAICategory)
                            //{
                            //    case AICategory.Median:
                            //    case AICategory.Small:
                            //    case AICategory.BigKotor:
                            //        borderLineRun[i].bmp1 = new Bitmap(bitmap);
                            //        break;
                            //    default:

                            //        break;
                            //}

                            borderLineRun[i].bmp1 = new Bitmap(PADTempRegion.bmpThreshold,
                                                                                                    Resize(PADTempRegion.bmpThreshold.Size, isize));

                            RectangleF rectSize = ResizeWithLocation2(rect, isize);

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
                        //    switch (PADAICategory)
                        //    {
                        //        case AICategory.Median:
                        //        case AICategory.Small:
                        //        case AICategory.BigKotor:
                        //            _get_border_pointf_v8_1_blackBigtor(item.bmp0, item.bmp1, item.rect0, item.Border, out glues[item.index], -isize);
                        //            break;
                        //        default:
                        //            _get_border_pointf_v8_1(item.bmp0, item.bmp1, item.rect0, item.Border, out glues[item.index], -isize);
                        //            break;
                        //    }
                        //}
                        Parallel.ForEach(borderLineRun, item =>
                        {
                            switch (PADAICategory)
                            {
                                //case AICategory.Median:
                                //case AICategory.Small:
                                //case AICategory.BigKotor:
                                //    _get_border_pointf_v8_1_blackBigtor(item.bmp0, item.bmp1, item.rect0, item.Border, out glues[item.index], -isize);
                                //    break;
                                default:
                                    _get_border_pointf_v8_1(item.bmp0, item.bmp1, item.rect0, item.Border, out glues[item.index], -isize);
                                    break;
                            }

                            //_get_border_pointf_v8_1(item.bmp0, item.bmp1, item.rect0, item.Border, out glues[item.index], isize);
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

                        #endregion

                        break;
                   
                    case PADChipSize.CHIP_NORMAL_EX:


                        #region 原始的通用做法

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
                            _get_border_pointf_NormalEx(item.bmp0, item.bmp1, item.rect0, item.Border, out glues[item.index]);

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

                        #endregion

                        break;
                    case PADChipSize.CHIP_V2:
                    case PADChipSize.CHIP_NORMAL:
                    default:


                        switch (PadInspectMethod)
                        {
                            case PadInspectMethodEnum.PAD_SMALL:


                                #region 来自Victor的做法

                                Bitmap bmpin0 = new Bitmap(bitmap);
                                Bitmap bmpout0 = new Bitmap(bitmap);

                                GetDataHXIn(bmpin0);
                                GetDataHX(bmpout0);

                                bmpin0.Dispose();
                                bmpout0.Dispose();

                                i = 0;
                                while (i < (int)SIDEEmnum.COUNT)
                                {
                                    switch ((SIDEEmnum)i)
                                    {
                                        case SIDEEmnum.TOP:
                                            JzFourSideCal(out glues[(int)BorderTypeEnum.TOP], sideAnalyzeIN.SIDEData[i], sideAnalyze.SIDEData[i]);

                                            //glues[(int)BorderTypeEnum.TOP] = new GlueRegionClass();
                                            //glues[(int)BorderTypeEnum.TOP].Reset();

                                            //foreach (Point point in sideAnalyzeIN.SIDEData[i].GetLinePoints())
                                            //{
                                            //    glues[(int)BorderTypeEnum.TOP].AddPtIN(point);
                                            //}
                                            //foreach (Point point in sideAnalyze.SIDEData[i].GetLinePoints())
                                            //{
                                            //    glues[(int)BorderTypeEnum.TOP].AddPt(point);
                                            //}

                                            //glues[(int)BorderTypeEnum.TOP].Run();

                                            break;
                                        case SIDEEmnum.BOTTOM:
                                            JzFourSideCal(out glues[(int)BorderTypeEnum.BOTTOM], sideAnalyzeIN.SIDEData[i], sideAnalyze.SIDEData[i]);
                                            break;
                                        case SIDEEmnum.LEFT:
                                            JzFourSideCal(out glues[(int)BorderTypeEnum.LEFT], sideAnalyzeIN.SIDEData[i], sideAnalyze.SIDEData[i]);
                                            break;
                                        case SIDEEmnum.RIGHT:
                                            JzFourSideCal(out glues[(int)BorderTypeEnum.RIGHT], sideAnalyzeIN.SIDEData[i], sideAnalyze.SIDEData[i]);
                                            break;
                                    }

                                    i++;
                                }

                                #endregion


                                break;
                            case PadInspectMethodEnum.PAD_V1:


                                #region 来自Victor的做法

                                Bitmap _bmpnewIpd = new Bitmap(bitmap);
                                GetDataIPD(_bmpnewIpd, false);
                                _bmpnewIpd.Dispose();

                                //JzFourSideCalV1(out glues[(int)BorderTypeEnum.TOP], jzsideanalyzeex, SIDEEmnum.TOP);

                                //Bitmap bmpin0 = new Bitmap(bitmap);
                                //Bitmap bmpout0 = new Bitmap(bitmap);

                                //GetDataHXIn(bmpin0);
                                //GetDataHX(bmpout0);

                                //bmpin0.Dispose();
                                //bmpout0.Dispose();

                                i = 0;
                                while (i < (int)SIDEEmnum.COUNT)
                                {
                                    switch ((SIDEEmnum)i)
                                    {
                                        case SIDEEmnum.TOP:
                                            JzFourSideCalV1(out glues[(int)BorderTypeEnum.TOP], jzsideanalyzeex, SIDEEmnum.TOP, GlueChipSlotDir == ChipSlotDir.SlotTop);
                                            break;
                                        case SIDEEmnum.BOTTOM:
                                            JzFourSideCalV1(out glues[(int)BorderTypeEnum.BOTTOM], jzsideanalyzeex, SIDEEmnum.BOTTOM, GlueChipSlotDir == ChipSlotDir.SlotBottom);
                                            break;
                                        case SIDEEmnum.LEFT:
                                            JzFourSideCalV1(out glues[(int)BorderTypeEnum.LEFT], jzsideanalyzeex, SIDEEmnum.LEFT, GlueChipSlotDir == ChipSlotDir.SlotLeft);
                                            break;
                                        case SIDEEmnum.RIGHT:
                                            JzFourSideCalV1(out glues[(int)BorderTypeEnum.RIGHT], jzsideanalyzeex, SIDEEmnum.RIGHT, GlueChipSlotDir == ChipSlotDir.SlotRight);
                                            break;
                                    }

                                    i++;
                                }

                                #endregion


                                break;
                            default:


                                #region 原始的通用做法

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

                                #endregion

                                break;
                        }


                        break;
                }


                Bitmap bmpglueout = new Bitmap(bmpoutput);
                //Bitmap bmpglueout = new Bitmap(bmpinput);//BAK
                Graphics g = Graphics.FromImage(bmpglueout);
                bool m_ischeckgluepass = true;
                string ngstr = string.Empty;
                string measureStr = string.Empty;

                double GlueTmpMax = 0;// GlueMax * Resolution_Mil;
                double GlueTmpMin = 0;// GlueMin * Resolution_Mil;


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

                double GlueSlotTmpMax = GlueMax / INI.MAINSD_PAD_MIL_RESOLUTION;
                double GlueSlotTmpMin = GlueMin / INI.MAINSD_PAD_MIL_RESOLUTION;

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

                    double minSlot = glues[i].LengthSlotMin * INI.MAINSD_PAD_MIL_RESOLUTION;
                    double maxSlot = glues[i].LengthSlotMax * INI.MAINSD_PAD_MIL_RESOLUTION;
                    bool isSlotPass = true;

                    //ngstr += ((BorderTypeEnum)i).ToString() + "_NG,";

                    switch ((BorderTypeEnum)i)
                    {
                        case BorderTypeEnum.LEFT:
                            measureStr += "左";// + Environment.NewLine;
                            measureStr += "[min:" + min.ToString("0.000000") + " mm]";
                            measureStr += "[max:" + max.ToString("0.000000") + " mm]" + Environment.NewLine;

                            if (GlueChipSlotDir == ChipSlotDir.SlotLeft)
                            {
                                measureStr += "薄膜胶";// + Environment.NewLine;
                                measureStr += "[min:" + minSlot.ToString("0.000000") + " mm]";
                                measureStr += "[max:" + maxSlot.ToString("0.000000") + " mm]" + Environment.NewLine;

                                isSlotPass = (glues[i].LengthSlotMax <= (GlueSlotTmpMax) && glues[i].LengthSlotMin >= (GlueSlotTmpMin));
                            }

                            break;
                        case BorderTypeEnum.RIGHT:
                            measureStr += "右";// + Environment.NewLine;
                            measureStr += "[min:" + min.ToString("0.000000") + " mm]";
                            measureStr += "[max:" + max.ToString("0.000000") + " mm]" + Environment.NewLine;

                            if (GlueChipSlotDir == ChipSlotDir.SlotRight)
                            {
                                measureStr += "薄膜胶";// + Environment.NewLine;
                                measureStr += "[min:" + minSlot.ToString("0.000000") + " mm]";
                                measureStr += "[max:" + maxSlot.ToString("0.000000") + " mm]" + Environment.NewLine;

                                isSlotPass = (glues[i].LengthSlotMax <= (GlueSlotTmpMax) && glues[i].LengthSlotMin >= (GlueSlotTmpMin));
                            }

                            break;
                        case BorderTypeEnum.TOP:
                            measureStr += "上";// + Environment.NewLine;
                            measureStr += "[min:" + min.ToString("0.000000") + " mm]";
                            measureStr += "[max:" + max.ToString("0.000000") + " mm]" + Environment.NewLine;

                            if (GlueChipSlotDir == ChipSlotDir.SlotTop)
                            {
                                measureStr += "薄膜胶";// + Environment.NewLine;
                                measureStr += "[min:" + minSlot.ToString("0.000000") + " mm]";
                                measureStr += "[max:" + maxSlot.ToString("0.000000") + " mm]" + Environment.NewLine;

                                isSlotPass = (glues[i].LengthSlotMax <= (GlueSlotTmpMax) && glues[i].LengthSlotMin >= (GlueSlotTmpMin));
                            }

                            break;
                        case BorderTypeEnum.BOTTOM:
                            measureStr += "下";// + Environment.NewLine;
                            measureStr += "[min:" + min.ToString("0.000000") + " mm]";
                            measureStr += "[max:" + max.ToString("0.000000") + " mm]" + Environment.NewLine;

                            if (GlueChipSlotDir == ChipSlotDir.SlotBottom)
                            {
                                measureStr += "薄膜胶";// + Environment.NewLine;
                                measureStr += "[min:" + minSlot.ToString("0.000000") + " mm]";
                                measureStr += "[max:" + maxSlot.ToString("0.000000") + " mm]" + Environment.NewLine;

                                isSlotPass = (glues[i].LengthSlotMax <= (GlueSlotTmpMax) && glues[i].LengthSlotMin >= (GlueSlotTmpMin));
                            }

                            break;
                    }

                    //measureStr += "[min:" + min.ToString("0.000000") + " mm]";
                    //measureStr += "[max:" + max.ToString("0.000000") + " mm]" + Environment.NewLine;

                    if (glues[i].LengthMax > (GlueTmpMax) || glues[i].LengthMin < (GlueTmpMin))
                    {
                        m_ischeckgluepass = false;

                        switch ((BorderTypeEnum)i)
                        {
                            case BorderTypeEnum.LEFT:
                                ngstr += "左";
                                ngstr += "[min:" + min.ToString("0.000000") + " mm]";
                                ngstr += "[max:" + max.ToString("0.000000") + " mm]";
                                break;
                            case BorderTypeEnum.RIGHT:
                                ngstr += "右";
                                ngstr += "[min:" + min.ToString("0.000000") + " mm]";
                                ngstr += "[max:" + max.ToString("0.000000") + " mm]";
                                break;
                            case BorderTypeEnum.TOP:
                                ngstr += "上";
                                ngstr += "[min:" + min.ToString("0.000000") + " mm]";
                                ngstr += "[max:" + max.ToString("0.000000") + " mm]";
                                break;
                            case BorderTypeEnum.BOTTOM:
                                ngstr += "下";
                                ngstr += "[min:" + min.ToString("0.000000") + " mm]";
                                ngstr += "[max:" + max.ToString("0.000000") + " mm]";
                                break;
                        }
                    }

                    if (GlueChipSlotDir != ChipSlotDir.NONE && !isSlotPass)
                    {
                        m_ischeckgluepass = false;

                        ngstr += "薄膜胶";// + Environment.NewLine;
                        ngstr += "[min:" + minSlot.ToString("0.000000") + " mm]";
                        ngstr += "[max:" + maxSlot.ToString("0.000000") + " mm]";
                    }

                    //switch (GlueChipSlotDir)
                    //{
                    //    case ChipSlotDir.SlotLeft:
                    //        break;
                    //    case ChipSlotDir.SlotRight:
                    //        break;
                    //    case ChipSlotDir.SlotTop:
                    //        break;
                    //    case ChipSlotDir.SlotBottom:
                    //        break;
                    //}


                    i++;
                }

                //填写数据的区域
                RectangleF _rectF = new RectangleF(0, 0, 1100, 290);
                _rectF = new RectangleF(0, 0, 1100, 40);
                //g.FillRectangle(Brushes.Black, _rectF);
                int linewidth = LineWidth;
                int fontsize = FontSize;

                //画图 及 显示比对图
                i = 0;
                int drawIndex = 0;
                while (drawIndex < (int)BorderTypeEnum.COUNT)
                //Parallel.For(0, (int)BorderTypeEnum.COUNT, (drawIndex) =>
                {
                    //switch ((BorderTypeEnum)drawIndex)
                    //{
                    //    case BorderTypeEnum.LEFT:
                    //        GlueTmpMax = GlueMaxLeft / INI.MAINSD_PAD_MIL_RESOLUTION;
                    //        GlueTmpMin = GlueMinLeft / INI.MAINSD_PAD_MIL_RESOLUTION;
                    //        break;
                    //    case BorderTypeEnum.TOP:
                    //        GlueTmpMax = GlueMaxTop / INI.MAINSD_PAD_MIL_RESOLUTION;
                    //        GlueTmpMin = GlueMinTop / INI.MAINSD_PAD_MIL_RESOLUTION;
                    //        break;
                    //    case BorderTypeEnum.RIGHT:
                    //        GlueTmpMax = GlueMaxRight / INI.MAINSD_PAD_MIL_RESOLUTION;
                    //        GlueTmpMin = GlueMinRight / INI.MAINSD_PAD_MIL_RESOLUTION;
                    //        break;
                    //    case BorderTypeEnum.BOTTOM:
                    //        GlueTmpMax = GlueMaxBottom / INI.MAINSD_PAD_MIL_RESOLUTION;
                    //        GlueTmpMin = GlueMinBottom / INI.MAINSD_PAD_MIL_RESOLUTION;
                    //        break;
                    //}

                    //pts = glues[drawIndex].GetPointF();
                    //ptsIN = glues[drawIndex].GetPointFIN();

                    if (m_ischeckgluepass)
                    {
                        g.DrawLines(new Pen(INI.chipPassColor, linewidth), glues[drawIndex].GetPointF());
                        g.DrawLines(new Pen(INI.chipPassColor, linewidth), glues[drawIndex].GetPointFIN());
                        g.DrawString(measureStr, new Font("宋体", fontsize), new SolidBrush(INI.chipPassColor), 2, 2);
                        //g.DrawString(measureStr, new Font("宋体", 22), Brushes.Lime, _rectF);

                        switch (PADChipSizeMode)
                        {
                            case PADChipSize.CHIP_NORMAL:
                                switch (PadInspectMethod)
                                {
                                    case PadInspectMethodEnum.PAD_V1:
                                        if (GlueChipSlotDir != ChipSlotDir.NONE)
                                            g.DrawLines(new Pen(INI.chipPassColor, linewidth), glues[drawIndex].GetPointFIN2());
                                        break;
                                }
                                break;
                        }
                    }
                    else
                    {
                        g.DrawLines(new Pen(Color.Red, linewidth), glues[drawIndex].GetPointF());
                        g.DrawLines(new Pen(Color.Red, linewidth), glues[drawIndex].GetPointFIN());
                        g.DrawString(measureStr, new Font("宋体", fontsize), Brushes.Red, 2, 2);
                        //g.DrawString(measureStr, new Font("宋体", 22), Brushes.Red, _rectF);

                        switch (PADChipSizeMode)
                        {
                            case PADChipSize.CHIP_NORMAL:
                                switch (PadInspectMethod)
                                {
                                    case PadInspectMethodEnum.PAD_V1:
                                        if (GlueChipSlotDir != ChipSlotDir.NONE)
                                            g.DrawLines(new Pen(Color.Red, linewidth), glues[drawIndex].GetPointFIN2());
                                        break;
                                }
                                break;
                        }
                    }

                    drawIndex++;
                }
                //);

                if (m_IsSaveTemp)
                {
                    g.DrawRectangle(new Pen(Color.Red), rect);
                    //g.DrawRectangle(new Pen(Color.Blue), rectin);
                    //g.DrawRectangle(new Pen(Color.Yellow), rectout);

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

            if (descstriing == "四周")
            {
                isgood = true;
                IsPass = isgood;
                m_DescStr = string.Empty;
            }
                

            bmpMeasureOutput.Dispose();
            bmpMeasureOutput = new Bitmap(bmpoutput);

            bmpInput.Dispose();

            return isgood;
        }


#endif
        #endregion

        public bool PB10_GlueInspectionProcess_BlackEdge(Bitmap bmpinput, ref Bitmap bmpoutput)
        {
            if (!string.IsNullOrEmpty(PADExtendOPString))
                PADExtend.FromString(PADExtendOPString);
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
                //int isized = 10;
                int isized = _from_bmpinputSize_to_iSized(bmpInput);
                if (isized == 0)
                    isized = 1;
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
                try
                {
                    bmphistogramEqualization11 = extractBiggestBlob11.Apply(bmphistogramEqualization11);
                }
                catch
                {

                }

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

            bool bChkAll = PADExtend.bNoInspectAll;

            glues = null;
            if (isgood && GlueCheck && !bChkAll || (!isgood && descstriing == "晶片表面溢胶" && GlueCheck && !bChkAll))
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
                //Bitmap bitmap0 = (Bitmap)bmpInput.Clone();//new Bitmap(bmpInput);
                if (m_IsSaveTemp)
                {
                    bitmap.Save(Universal.CalTestPath + "\\" + _CalPageIndex() + RelateAnalyzeString + "bmpTrainCollect" + ".png",
                       System.Drawing.Imaging.ImageFormat.Png);


                    bitmap.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bitmap" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                }
                int sized = _from_bmpinputSize_to_iSized(imginput);
                switch (PADChipSizeMode)
                {
                    case PADChipSize.CHIP_V8:
                        //bitmap = (Bitmap)_getV8bmpInput(rect1).Clone();
                        bitmap = new Bitmap(blackAICal(PADTempRegion, -sized));
                        break;
                    case PADChipSize.CHIP_NORMAL:
                    default:

                        switch(PadInspectMethod)
                        {
                            case PadInspectMethodEnum.PAD_G1:
                                break;
                            default:
                                //bitmap = (Bitmap)blackNormal(PADTempRegion, 1).Clone();

                                Bitmap bmptempxxx = new Bitmap(blackNormal(PADTempRegion, -sized));
                                //bitmap = new Bitmap(blackNormal(PADTempRegion));
                                bitmap = new Bitmap(bmptempxxx, new Size(bmptempxxx.Width * -sized, bmptempxxx.Height * -sized));
                                bmptempxxx.Dispose();
                                break;
                        }
                         
                        break;
                }

                //这里是寻找点的起点位置
                //rect.Inflate(-5, -5);
                rect.Inflate(-(int)(CalExtendX * Resolution_Mil), -(int)(CalExtendY * Resolution_Mil));
                
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

                        switch (PADAICategory)
                        {
                            case AICategory.Median:
                            case AICategory.Small:
                                //case AICategory.BigKotor:
                                sized = 0;
                                break;
                            default:

                                break;
                        }

                        Bitmap bmpthresholdtemp = new Bitmap(PADTempRegion.bmpThreshold, Resize(PADTempRegion.bmpThreshold.Size, sized));

                        borderLineRun = new BorderLineRunClass[4];
                        i = 0;
                        while (i < (int)BorderTypeEnum.COUNT)
                        {

                            borderLineRun[i] = new BorderLineRunClass();
                            borderLineRun[i].bmp0 = (Bitmap)bitmap.Clone();// new Bitmap(bitmap);
                            switch (PADAICategory)
                            {
                                case AICategory.Median:
                                case AICategory.Small:
                                case AICategory.BigKotor:
                                    borderLineRun[i].bmp1 = new Bitmap(bitmap);
                                    break;
                                default:
                                    borderLineRun[i].bmp1 = new Bitmap(bmpthresholdtemp);
                                    break;

                            }



                            RectangleF rectSize = ResizeWithLocation2(rect, sized);

                            //(Bitmap)PADTempRegion.bmpThreshold.Clone();// new Bitmap(PADTempRegion.bmpThreshold);
                            borderLineRun[i].rect0 =
                                new Rectangle((int)rectSize.X, (int)rectSize.Y, (int)rectSize.Width, (int)rectSize.Height);
                            //new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                            borderLineRun[i].Border = (BorderTypeEnum)i;
                            borderLineRun[i].index = i;

                            i++;
                        }

                        bmpthresholdtemp.Dispose();

                        //foreach (BorderLineRunClass item in borderLineRun)
                        //{
                        //    _get_border_pointf_v8_1(item.bmp0, item.bmp1, item.rect0, item.Border, out glues[item.index]);

                        //}
                        Parallel.ForEach(borderLineRun, item =>
                        {
                            switch (PADAICategory)
                            {
                                case AICategory.Median:
                                case AICategory.Small:
                                case AICategory.BigKotor:
                                    _get_border_pointf_v8_1_blackBigtor(item.bmp0, item.bmp1, item.rect0, item.Border, out glues[item.index], -sized);
                                    break;
                                default:
                                    _get_border_pointf_v8_1(item.bmp0, item.bmp1, item.rect0, item.Border, out glues[item.index], -sized);
                                    break;

                            }

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

                        switch(PadInspectMethod)
                        {
                            case PadInspectMethodEnum.PAD_G1:

                                #region 寻找间距的做法

                                borderLineRun = new BorderLineRunClass[4];
                                i = 0;
                                while (i < (int)BorderTypeEnum.COUNT)
                                {

                                    borderLineRun[i] = new BorderLineRunClass();
                                    borderLineRun[i].bmp0 = (Bitmap)bitmap.Clone();// new Bitmap(bitmap);
                                    borderLineRun[i].bmp1 = (Bitmap)bitmap.Clone();
                                    //borderLineRun[i].bmp1 = (Bitmap)PADTempRegion.bmpThreshold.Clone();//  new Bitmap(PADTempRegion.bmpThreshold);
                                    borderLineRun[i].rect0 = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                                    borderLineRun[i].Border = (BorderTypeEnum)i;
                                    borderLineRun[i].index = i;

                                    borderLineRun[i].rectORG = new Rectangle(PADTempRegion.RegionForEdgeRect.X,
                                                                       PADTempRegion.RegionForEdgeRect.Y,
                                                                       PADTempRegion.RegionForEdgeRect.Width,
                                                                       PADTempRegion.RegionForEdgeRect.Height);
                                    borderLineRun[i].padG1 = new PADG1Class();
                                    borderLineRun[i].padG1.FromString(PADINSPECTOPString);

                                    i++;
                                }

                                //这里写间距的算法

                                Parallel.ForEach(borderLineRun, item =>
                                {
                                    _get_border_pointf_PADG1(item.bmp0,
                                        item.bmp1,
                                        item.rect0,
                                        item.rectORG,
                                        item.padG1,
                                        item.Border,
                                        out glues[item.index]);

                                });

                                //foreach (var item in borderLineRun)
                                //{
                                //    _get_border_pointf_PADG1(item.bmp0,
                                //        item.bmp1,
                                //        item.rect0,
                                //        item.rectORG,
                                //        item.padG1,
                                //        item.Border,
                                //        out glues[item.index]);
                                //}

                                i = 0;
                                while (i < (int)BorderTypeEnum.COUNT)
                                {

                                    //borderLineRun[i] = new BorderLineRunClass();
                                    borderLineRun[i].bmp0.Dispose();
                                    borderLineRun[i].bmp1.Dispose();

                                    i++;
                                }
                                GC.Collect();

                                //_bmpnewIpd.Dispose();

                                #endregion

                                break;
                            default:

                                #region 原始的做法

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

                                #endregion

                                break;
                        }

                        
                        break;
                }


                Bitmap bmpglueout = new Bitmap(bmpinput);
                Graphics g = Graphics.FromImage(bmpglueout);
                bool m_ischeckgluepass = true;
                string ngstr = string.Empty;
                string measureStr = string.Empty;
                bool _bNoInspect = false;

                double GlueTmpMax = 0;// GlueMax * Resolution_Mil;
                double GlueTmpMin = 0;// GlueMin * Resolution_Mil;

                #region 整合数据及画图

                if (INI.CHIP_CAL_MODE == 2)
                {

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
                            GlueTmpMax = GlueMaxLeft;// / INI.MAINSD_PAD_MIL_RESOLUTION;
                            GlueTmpMin = GlueMinLeft;// / INI.MAINSD_PAD_MIL_RESOLUTION;
                            break;
                        case BorderTypeEnum.TOP:
                            GlueTmpMax = GlueMaxTop;// / INI.MAINSD_PAD_MIL_RESOLUTION;
                            GlueTmpMin = GlueMinTop;// / INI.MAINSD_PAD_MIL_RESOLUTION;
                            break;
                        case BorderTypeEnum.RIGHT:
                            GlueTmpMax = GlueMaxRight;// / INI.MAINSD_PAD_MIL_RESOLUTION;
                            GlueTmpMin = GlueMinRight;// / INI.MAINSD_PAD_MIL_RESOLUTION;
                            break;
                        case BorderTypeEnum.BOTTOM:
                            GlueTmpMax = GlueMaxBottom;// / INI.MAINSD_PAD_MIL_RESOLUTION;
                            GlueTmpMin = GlueMinBottom;/// INI.MAINSD_PAD_MIL_RESOLUTION;
                            break;
                    }

                    double min = glues[i].LengthMin * INI.MAINSD_PAD_MIL_RESOLUTION;
                    double max = glues[i].LengthMax * INI.MAINSD_PAD_MIL_RESOLUTION;

                    min = glues[i].GetMinMM();
                    max = glues[i].GetMaxMM();

                    //ngstr += ((BorderTypeEnum)i).ToString() + "_NG,";

                    switch ((BorderTypeEnum)i)
                    {
                        case BorderTypeEnum.LEFT:
                            _bNoInspect = PADExtend.bNoInspectLeft;
                            measureStr += "左";// + Environment.NewLine;
                            break;
                        case BorderTypeEnum.RIGHT:
                            _bNoInspect = PADExtend.bNoInspectRight;
                            measureStr += "右";// + Environment.NewLine;
                            break;
                        case BorderTypeEnum.TOP:
                            _bNoInspect = PADExtend.bNoInspectTop;
                            measureStr += "上";// + Environment.NewLine;
                            break;
                        case BorderTypeEnum.BOTTOM:
                            _bNoInspect = PADExtend.bNoInspectBottom;
                            measureStr += "下";// + Environment.NewLine;
                            break;
                    }
                    if (_bNoInspect)
                    {
                        measureStr += "[min:no measure]";
                        measureStr += "[max:no measure]" + Environment.NewLine;
                    }
                    else
                    {
                        measureStr += "[min:" + min.ToString("0.000000") + " mm]";
                        measureStr += "[max:" + max.ToString("0.000000") + " mm]" + Environment.NewLine;
                    }

                    //if (glues[i].LengthMax > (GlueTmpMax) || glues[i].LengthMin < (GlueTmpMin))
                    if (max > (GlueTmpMax) || min < (GlueTmpMin))
                    {
                        m_ischeckgluepass = false;

                        switch ((BorderTypeEnum)i)
                        {
                            case BorderTypeEnum.LEFT:
                                
                                ngstr += "左";
                                //if (!PADExtend.bNoInspectLeft)
                                //    ngstr += "左";
                                //else
                                //{
                                //    m_ischeckgluepass = true;
                                //    ngstr += "左";
                                //    ngstr += "[min:no measure]";
                                //    ngstr += "[max:no measure]";
                                //}
                                break;
                            case BorderTypeEnum.RIGHT:
                           
                                ngstr += "右";
                                //if (!PADExtend.bNoInspectRight)
                                //    ngstr += "右";
                                //else
                                //{
                                //    m_ischeckgluepass = true;
                                //    ngstr += "右";
                                //    ngstr += "[min:no measure]";
                                //    ngstr += "[max:no measure]";
                                //}
                                break;
                            case BorderTypeEnum.TOP:
                            
                                ngstr += "上";
                                //if (!PADExtend.bNoInspectTop)
                                //    ngstr += "上";
                                //else
                                //{
                                //    m_ischeckgluepass = true;
                                //    ngstr += "上";
                                //    ngstr += "[min:no measure]";
                                //    ngstr += "[max:no measure]";
                                //}
                                break;
                            case BorderTypeEnum.BOTTOM:
                            
                                ngstr += "下";
                                //if (!PADExtend.bNoInspectBottom)
                                //    ngstr += "下";
                                //else
                                //{
                                //    m_ischeckgluepass = true;
                                //    ngstr += "下";
                                //    ngstr += "[min:no measure]";
                                //    ngstr += "[max:no measure]";
                                //}
                                break;
                        }

                        if (_bNoInspect)
                        {
                            m_ischeckgluepass = true;
                            ngstr += "[min:no measure]";
                            ngstr += "[max:no measure]";
                        }
                        else
                        {
                            ngstr += "[min:" + min.ToString("0.000000") + " mm]";
                            ngstr += "[max:" + max.ToString("0.000000") + " mm]";
                        }
                    }

                    i++;
                }

                //填写数据的区域
                RectangleF _rectF = new RectangleF(0, 0, 1100, 290);
                //g.FillRectangle(Brushes.Black, _rectF);

                int fontsize = FontSize;// 34;
                int linewidth = LineWidth;// 5;

                if (PADExtend.bOpenUseContactEle)
                {
                    Rectangle org = new Rectangle(m_PADRegion.RegionForEdgeRect.X,
                                                                        m_PADRegion.RegionForEdgeRect.Y,
                                                                        m_PADRegion.RegionForEdgeRect.Width,
                                                                        m_PADRegion.RegionForEdgeRect.Height);
                    //if(PADExtend.cEleX > 0 && PADExtend.cEleY >0)
                    double baseResu = INI.MAINSD_PAD_MIL_RESOLUTION;
                    if (baseResu <= 0)
                    {
                        baseResu = 0.03;
                    }
                    double _x = PADExtend.cEleX / baseResu;
                    double _y = PADExtend.cEleY / baseResu;
                    org.Inflate((int)_x, (int)_y);
                    //org.Inflate(PADExtend.cEleX, PADExtend.cEleY);

                    bool bInstert = true;
                    //判别边缘点都在范围内
                    for (int glueindex = 0; glueindex < 4; glueindex++)
                    {
                        bool a0 = true;
                        switch ((BorderTypeEnum)glueindex)
                        {
                            case BorderTypeEnum.LEFT:
                                a0 = PADExtend.bNoInspectLeft;
                                break;
                            case BorderTypeEnum.RIGHT:
                                a0 = PADExtend.bNoInspectRight;
                                break;
                            case BorderTypeEnum.TOP:
                                a0 = PADExtend.bNoInspectTop;
                                break;
                            case BorderTypeEnum.BOTTOM:
                                a0 = PADExtend.bNoInspectBottom;
                                break;
                        }
                        if (a0) //不检测的话跳过判断碰到元件
                            continue;

                        foreach (var pt in glues[glueindex].GetPointF())
                        {
                            RectangleF r0 = SimpleRectF(pt, 1, 1);
                            bInstert = r0.IntersectsWith(org);
                            if (!bInstert)
                            {
                                break;
                            }
                        }
                        if (!bInstert)
                        {
                            break;
                        }
                    }

                    if (!bInstert)
                    {
                        m_ischeckgluepass = false;
                        //measureStr += "";
                        measureStr += $"胶水碰到元件NG" + Environment.NewLine;
                        ngstr += $"胶水碰到元件NG" + Environment.NewLine;
                    }

                    if (!m_ischeckgluepass)
                    {
                        g.DrawRectangle(new Pen(Color.Yellow, linewidth), org);
                    }
                }

                //画图 及 显示比对图
                i = 0;
                int drawIndex = 0;
                while (drawIndex < (int)BorderTypeEnum.COUNT)
                //Parallel.For(0, (int)BorderTypeEnum.COUNT, (drawIndex) =>
                {
                    //switch ((BorderTypeEnum)drawIndex)
                    //{
                    //    case BorderTypeEnum.LEFT:
                    //        GlueTmpMax = GlueMaxLeft / INI.MAINSD_PAD_MIL_RESOLUTION;
                    //        GlueTmpMin = GlueMinLeft / INI.MAINSD_PAD_MIL_RESOLUTION;
                    //        break;
                    //    case BorderTypeEnum.TOP:
                    //        GlueTmpMax = GlueMaxTop / INI.MAINSD_PAD_MIL_RESOLUTION;
                    //        GlueTmpMin = GlueMinTop / INI.MAINSD_PAD_MIL_RESOLUTION;
                    //        break;
                    //    case BorderTypeEnum.RIGHT:
                    //        GlueTmpMax = GlueMaxRight / INI.MAINSD_PAD_MIL_RESOLUTION;
                    //        GlueTmpMin = GlueMinRight / INI.MAINSD_PAD_MIL_RESOLUTION;
                    //        break;
                    //    case BorderTypeEnum.BOTTOM:
                    //        GlueTmpMax = GlueMaxBottom / INI.MAINSD_PAD_MIL_RESOLUTION;
                    //        GlueTmpMin = GlueMinBottom / INI.MAINSD_PAD_MIL_RESOLUTION;
                    //        break;
                    //}

                    //pts = glues[drawIndex].GetPointF();
                    //ptsIN = glues[drawIndex].GetPointFIN();

                    if (m_ischeckgluepass)
                    {
                        g.DrawLines(new Pen(Color.Lime, linewidth), glues[drawIndex].GetPointF());
                        g.DrawLines(new Pen(Color.Lime, linewidth), glues[drawIndex].GetPointFIN());
                        g.DrawString(measureStr, new Font("宋体", fontsize), Brushes.Lime, 5, 5);
                        //g.DrawString(measureStr, new Font("宋体", 22), Brushes.Lime, _rectF);
                    }
                    else
                    {
                        g.DrawLines(new Pen(Color.Red, linewidth), glues[drawIndex].GetPointF());
                        g.DrawLines(new Pen(Color.Red, linewidth), glues[drawIndex].GetPointFIN());
                        g.DrawString(measureStr, new Font("宋体", fontsize), Brushes.Red, 5, 5);
                        //g.DrawString(measureStr, new Font("宋体", 22), Brushes.Red, _rectF);
                    }

                    drawIndex++;
                }
                //);

                if (m_IsSaveTemp)
                {
                    //g.DrawRectangle(new Pen(Color.Red), rect);
                    //g.DrawRectangle(new Pen(Color.Blue), rectin);
                    //g.DrawRectangle(new Pen(Color.Yellow), rectout);

                    bmpglueout.Save(Universal.CalTestPath + "\\" + _CalPageIndex() + RelateAnalyzeString + "bmpInputout" + ".png",
                        System.Drawing.Imaging.ImageFormat.Png);

                    bmpglueout.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpInputout" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                }
                //bmpglueout.Save("D:\\testtest\\" + RelateAnalyzeString + "bmpInputout" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                #endregion

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
                        descstriing = "基板表面边角缺陷";
                    }
                    else if (m_BadWidth > CheckDWidth)
                    {
                        isgood = false;
                        processstring += "Error in " + RelateAnalyzeString + " PAD WIDTH OVER WIDTH= " + " , " + m_BadWidth.ToString() + " , " + CheckDWidth.ToString() + Environment.NewLine;
                        errorstring += RelateAnalyzeString + " PAD WIDTH OVER WIDTH= " + " , " + m_BadWidth.ToString() + " , " + CheckDWidth.ToString() + Environment.NewLine;

                        reason = ReasonEnum.NG;
                        descstriing = "基板表面边角缺陷";
                    }
                    else if (m_BadHeight > CheckDHeight)
                    {
                        isgood = false;
                        processstring += "Error in " + RelateAnalyzeString + " PAD HEIGHT OVER HEIGHT= " + " , " + m_BadHeight.ToString() + " , " + CheckDHeight.ToString() + Environment.NewLine;
                        errorstring += RelateAnalyzeString + " PAD HEIGHT OVER HEIGHT= " + " , " + m_BadHeight.ToString() + " , " + CheckDHeight.ToString() + Environment.NewLine;

                        reason = ReasonEnum.NG;
                        descstriing = "基板表面边角缺陷";
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
        public bool PB10_QLECheckInspectionProcess(Bitmap bmpinput, ref Bitmap bmpoutput)
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

                #region 检测有无芯片

                switch (ChipNoHaveMode)
                {
                    case ChipNoHave.Normal:

                        Bitmap bmprunxxxx = new Bitmap(bmpinput);
                        isgood = checkNoHaveRun(bmprunxxxx);
                        bmprunxxxx.Dispose();

                        if (!isgood)
                        {
                            isgood = false;
                            processstring += "Error in FastPH" + RelateAnalyzeString + Environment.NewLine;
                            errorstring += RelateAnalyzeString + Environment.NewLine;

                            reason = ReasonEnum.PASS;
                            descstriing = "无芯片";
                        }
                        break;
                    case ChipNoHave.BlobCount:

                        Rectangle _cropItem = PADTempRegion.RegionForEdgeRect;
                        int iws = _cropItem.Width / 4;
                        int ihs = _cropItem.Height / 4;
                        _cropItem.Inflate(-iws, -ihs);
                        BoundRect(ref _cropItem, bmpinput.Size);
                        Bitmap bmpitemx = bmpinput.Clone(_cropItem, PixelFormat.Format24bppRgb);
                        //int _thresholdv = 100;
                        N2Class n2Class = new N2Class();
                        n2Class.FromString(ChipNoHaveModeOpString);
                        isgood = checkNoHaveRunBlobCount(bmpitemx, n2Class.ThresholdValue, n2Class.IsWhite, n2Class.Count);
                        bmpitemx.Dispose();

                        if (!isgood)
                        {
                            isgood = false;
                            processstring += "Error in Blob" + RelateAnalyzeString + Environment.NewLine;
                            errorstring += RelateAnalyzeString + Environment.NewLine;

                            reason = ReasonEnum.PASS;
                            descstriing = "无芯片";
                        }

                        break;
                    default:
                        break;
                }

                #endregion

                //if (!IsInRangeRatio(PADTempRegion.RegionArea, m_PADRegion.RegionArea, OAreaRatio))
                //{
                //    isgood = false;
                //    processstring += "Error in " + RelateAnalyzeString + " PAD AREA OVER Ratio= " + OAreaRatio.ToString() + " , " + PADTempRegion.RegionArea.ToString() + " , " + m_PADRegion.RegionArea.ToString() + Environment.NewLine;
                //    errorstring += RelateAnalyzeString + " PAD AREA OVER Ratio= " + OAreaRatio.ToString() + " , " + PADTempRegion.RegionArea.ToString() + " , " + m_PADRegion.RegionArea.ToString() + Environment.NewLine;

                //    //reason = ReasonEnum.NG;
                //    //descstriing = "尺寸面积超标";

                //    reason = ReasonEnum.NG;
                //    descstriing = "无胶";
                //}

                #region 判断有无胶

                if (!IsInRangeRatio(PADTempRegion.RegionWidth, m_PADRegion.RegionWidth, OWidthRatio))
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + PADTempRegion.RegionWidth.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + PADTempRegion.RegionWidth.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;

                    //reason = ReasonEnum.NG;
                    //descstriing = "尺寸宽度超标";

                    reason = ReasonEnum.NG;
                    descstriing = "无胶";
                }
                else if (!IsInRangeRatio(PADTempRegion.RegionHeight, m_PADRegion.RegionHeight, OHeightRatio))
                {
                    isgood = false;
                    processstring += "Error in " + RelateAnalyzeString + " PAD HEIGHT OVER Ratio= " + OHeightRatio.ToString() + " , " + PADTempRegion.RegionHeight.ToString() + " , " + m_PADRegion.RegionHeight.ToString() + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " PAD HEIGHT OVER Ratio= " + OHeightRatio.ToString() + " , " + PADTempRegion.RegionHeight.ToString() + " , " + m_PADRegion.RegionHeight.ToString() + Environment.NewLine;

                    //reason = ReasonEnum.NG;
                    //descstriing = "尺寸高度超标";

                    reason = ReasonEnum.NG;
                    descstriing = "无胶";
                }

                #endregion


                #region 判断银胶大面积异形

                if (isgood)
                {
                    if (!IsInRangeEx(PADTempRegion.RegionAreaReal, GleAreaUpper, GleAreaLower))
                    {
                        isgood = false;
                        processstring += "Error in " + RelateAnalyzeString + " PAD AREA OVER Ratio= " + OAreaRatio.ToString() + " , " + PADTempRegion.RegionArea.ToString() + " , " + m_PADRegion.RegionArea.ToString() + Environment.NewLine;
                        errorstring += RelateAnalyzeString + " PAD AREA OVER Ratio= " + OAreaRatio.ToString() + " , " + PADTempRegion.RegionArea.ToString() + " , " + m_PADRegion.RegionArea.ToString() + Environment.NewLine;

                        reason = ReasonEnum.NG;
                        descstriing = "胶水异常胶水面积不合格";
                    }
                    else if (!IsInRangeEx(PADTempRegion.RegionWidthReal, GleWidthUpper, GleWidthLower))
                    {
                        isgood = false;
                        processstring += "Error in " + RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + PADTempRegion.RegionWidth.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;
                        errorstring += RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + PADTempRegion.RegionWidth.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;

                        reason = ReasonEnum.NG;
                        descstriing = "胶水异常胶水长度不合格";
                    }
                    else if (!IsInRangeEx(PADTempRegion.RegionHeightReal, GleHeightUpper, GleHeightLower))
                    {
                        isgood = false;
                        processstring += "Error in " + RelateAnalyzeString + " PAD HEIGHT OVER Ratio= " + OHeightRatio.ToString() + " , " + PADTempRegion.RegionHeight.ToString() + " , " + m_PADRegion.RegionHeight.ToString() + Environment.NewLine;
                        errorstring += RelateAnalyzeString + " PAD HEIGHT OVER Ratio= " + OHeightRatio.ToString() + " , " + PADTempRegion.RegionHeight.ToString() + " , " + m_PADRegion.RegionHeight.ToString() + Environment.NewLine;

                        reason = ReasonEnum.NG;
                        descstriing = "胶水异常胶水宽度不合格";
                    }
                }

                #endregion

                bmpoutput.Dispose();
                //bmpoutput = new Bitmap(bmpPadFindOutput);
                bmpoutput = (Bitmap)bmpPadFindOutput.Clone(new Rectangle(0, 0, bmpPadFindOutput.Width, bmpPadFindOutput.Height), PixelFormat.Format24bppRgb);


                #region 判断银胶胶型正常内部异常的胶水

                if (isgood)
                {
                    switch(PadInspectMethod)
                    {
                        case PadInspectMethodEnum.PAD_G2:

                            //定位看异常
                            isgood = CheckYinJiaoIrregular(false, bmpInput, new RectangleF(0, 0, 1, 1));
                            if (!isgood)
                            {
                                isgood = false;
                                processstring += "Error in " + RelateAnalyzeString + " Irregular" + Environment.NewLine;
                                errorstring += RelateAnalyzeString + " Irregular" + Environment.NewLine;

                                reason = ReasonEnum.NG;
                                descstriing = "胶水异常表面异物或凹陷Irr";
                            }

                            break;
                    }
                }

                #endregion

                //if (!IsInRangeRatio(PADTempRegion.RegionArea, m_PADRegion.RegionArea, OAreaRatio))
                //{
                //    isgood = false;
                //    processstring += "Error in " + RelateAnalyzeString + " PAD AREA OVER Ratio= " + OAreaRatio.ToString() + " , " + PADTempRegion.RegionArea.ToString() + " , " + m_PADRegion.RegionArea.ToString() + Environment.NewLine;
                //    errorstring += RelateAnalyzeString + " PAD AREA OVER Ratio= " + OAreaRatio.ToString() + " , " + PADTempRegion.RegionArea.ToString() + " , " + m_PADRegion.RegionArea.ToString() + Environment.NewLine;

                //    reason = ReasonEnum.NG;
                //    descstriing = "胶水面积缺陷";
                //}
                //else if (!IsInRangeRatio(PADTempRegion.RegionWidth, m_PADRegion.RegionWidth, OWidthRatio))
                //{
                //    isgood = false;
                //    processstring += "Error in " + RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + PADTempRegion.RegionWidth.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;
                //    errorstring += RelateAnalyzeString + " PAD WIDTH OVER Ratio= " + OWidthRatio.ToString() + " , " + PADTempRegion.RegionWidth.ToString() + " , " + m_PADRegion.RegionWidth.ToString() + Environment.NewLine;

                //    reason = ReasonEnum.NG;
                //    descstriing = "胶水宽度缺陷";
                //}
                //else if (!IsInRangeRatio(PADTempRegion.RegionHeight, m_PADRegion.RegionHeight, OHeightRatio))
                //{
                //    isgood = false;
                //    processstring += "Error in " + RelateAnalyzeString + " PAD HEIGHT OVER Ratio= " + OHeightRatio.ToString() + " , " + PADTempRegion.RegionHeight.ToString() + " , " + m_PADRegion.RegionHeight.ToString() + Environment.NewLine;
                //    errorstring += RelateAnalyzeString + " PAD HEIGHT OVER Ratio= " + OHeightRatio.ToString() + " , " + PADTempRegion.RegionHeight.ToString() + " , " + m_PADRegion.RegionHeight.ToString() + Environment.NewLine;

                //    reason = ReasonEnum.NG;
                //    descstriing = "胶水高度缺陷";
                //}

                #region 判断chip表面的脏污

                if (isgood)
                {
                    //78ms
                    PADRegionFindBlob_QLE(bmpInput, PADBlobGrayThreshold, PADTempRegion, ref bmpoutput, 1, GLEFindWhite);

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
                        descstriing = "溢胶表面脏污面积缺陷";
                    }
                    else if (m_BadWidth > CheckDWidth)
                    {
                        isgood = false;
                        processstring += "Error in " + RelateAnalyzeString + " PAD WIDTH OVER WIDTH= " + " , " + m_BadWidth.ToString() + " , " + CheckDWidth.ToString() + Environment.NewLine;
                        errorstring += RelateAnalyzeString + " PAD WIDTH OVER WIDTH= " + " , " + m_BadWidth.ToString() + " , " + CheckDWidth.ToString() + Environment.NewLine;

                        reason = ReasonEnum.NG;
                        descstriing = "溢胶表面脏污宽度缺陷";
                    }
                    else if (m_BadHeight > CheckDHeight)
                    {
                        isgood = false;
                        processstring += "Error in " + RelateAnalyzeString + " PAD HEIGHT OVER HEIGHT= " + " , " + m_BadHeight.ToString() + " , " + CheckDHeight.ToString() + Environment.NewLine;
                        errorstring += RelateAnalyzeString + " PAD HEIGHT OVER HEIGHT= " + " , " + m_BadHeight.ToString() + " , " + CheckDHeight.ToString() + Environment.NewLine;

                        reason = ReasonEnum.NG;
                        descstriing = "溢胶表面脏污高度缺陷";
                    }
                }

                #endregion
            }

            #endregion

            //画出银胶到四边的距离
            Graphics _gGle = Graphics.FromImage(bmpoutput);
            if(descstriing.Contains("异物或凹陷Irr"))
            {
                RectangleF rectCrop = FindAxisAlignedBoundingRectangle(m_PADRegion.RegionPtFCornerQLE);
                BoundRect(ref rectCrop, bmpPattern.Size);
                _gGle.DrawImage(bmpPadBolbOutput, rectCrop);
            }

            List<PointF> list_corner_org_points = PADTempRegion.RegionPtFCornerORG.ToList();
            list_corner_org_points = SortCornersClockwise(list_corner_org_points);
            List<PointF> list_corner_qle_points = PADTempRegion.RegionPtFCornerQLE.ToList();
            list_corner_qle_points = SortCornersClockwise(list_corner_qle_points);

            string showmsg = $"长度{PADTempRegion.RegionWidthReal}mm,宽度{PADTempRegion.RegionHeightReal}mm,面积{PADTempRegion.RegionAreaReal}mm";
            _gGle.DrawString(showmsg, new Font("宋体", FontSize), Brushes.Lime, 5, 15);
            m_QLERunDataStr = $"{Math.Round(PADTempRegion.RegionWidthReal, 6)},{Math.Round(PADTempRegion.RegionHeightReal, 6)},{Math.Round(PADTempRegion.RegionAreaReal, 6)}";
            _gGle.DrawString($"P0", new Font("宋体", FontSize), Brushes.Lime, list_corner_qle_points[0]);
            _gGle.DrawString($"P1", new Font("宋体", FontSize), Brushes.Lime, list_corner_qle_points[1]);
            _gGle.DrawString($"P2", new Font("宋体", FontSize), Brushes.Lime, list_corner_qle_points[2]);
            _gGle.DrawString($"P3", new Font("宋体", FontSize), Brushes.Lime, list_corner_qle_points[3]);

            _gGle.DrawString($"P0", new Font("宋体", FontSize), Brushes.Lime, list_corner_org_points[0]);
            _gGle.DrawString($"P1", new Font("宋体", FontSize), Brushes.Lime, list_corner_org_points[1]);
            _gGle.DrawString($"P2", new Font("宋体", FontSize), Brushes.Lime, list_corner_org_points[2]);
            _gGle.DrawString($"P3", new Font("宋体", FontSize), Brushes.Lime, list_corner_org_points[3]);

            //_gGle.DrawString($"P0", new Font("宋体", FontSize), Brushes.Lime, PADTempRegion.RegionPtFCornerORG[0]);
            //_gGle.DrawString($"P1", new Font("宋体", FontSize), Brushes.Lime, PADTempRegion.RegionPtFCornerORG[1]);
            //_gGle.DrawString($"P2", new Font("宋体", FontSize), Brushes.Lime, PADTempRegion.RegionPtFCornerORG[2]);
            //_gGle.DrawString($"P3", new Font("宋体", FontSize), Brushes.Lime, PADTempRegion.RegionPtFCornerORG[3]);


            #region 到四边的距离

            if (!descstriing.Contains("胶水异常") && !descstriing.Contains("无胶"))
            {
                string measuredStr = string.Empty;
                glues = new GlueRegionClass[(int)BorderTypeEnum.COUNT];
                int i = 0;
                while (i < (int)BorderTypeEnum.COUNT)
                {
                    glues[i] = new GlueRegionClass();

                    i++;
                }

                //内围
                PointF pleftin = new PointF(PADTempRegion.QleMaxRect.X, PADTempRegion.QleMaxRect.Y + PADTempRegion.QleMaxRect.Height / 2);
                PointF pupin = new PointF(PADTempRegion.QleMaxRect.X + PADTempRegion.QleMaxRect.Width / 2, PADTempRegion.QleMaxRect.Y);
                PointF prightin = new PointF(PADTempRegion.QleMaxRect.X + PADTempRegion.QleMaxRect.Width, PADTempRegion.QleMaxRect.Y + PADTempRegion.QleMaxRect.Height / 2);
                PointF pbottomin = new PointF(PADTempRegion.QleMaxRect.X + PADTempRegion.QleMaxRect.Width / 2, PADTempRegion.QleMaxRect.Y + PADTempRegion.QleMaxRect.Height);

                //这里排序四个点的位置
                //外围
                //PointF pleftout = GetCenterPoint(PADTempRegion.RegionPtFCornerORG[0], PADTempRegion.RegionPtFCornerORG[1]);
                //PointF pupout = GetCenterPoint(PADTempRegion.RegionPtFCornerORG[0], PADTempRegion.RegionPtFCornerORG[3]);
                //PointF prightout = GetCenterPoint(PADTempRegion.RegionPtFCornerORG[2], PADTempRegion.RegionPtFCornerORG[3]);
                //PointF pbottomout = GetCenterPoint(PADTempRegion.RegionPtFCornerORG[2], PADTempRegion.RegionPtFCornerORG[1]);

                PointF pleftout = GetCenterPoint(list_corner_org_points[1], list_corner_org_points[0]);
                PointF pupout = GetCenterPoint(list_corner_org_points[1], list_corner_org_points[3]);
                PointF prightout = GetCenterPoint(list_corner_org_points[2], list_corner_org_points[3]);
                PointF pbottomout = GetCenterPoint(list_corner_org_points[2], list_corner_org_points[0]);


                double tempmax = 0;
                double tempmin = 0;
                bool bok = false;
                //Brush bUsh = new SolidBrush(Color.Lime);
                Color cush = Color.Lime;

                glues[(int)BorderTypeEnum.LEFT].LengthMin = GetPointLength(pleftin, pleftout) * INI.MAINSD_PAD_MIL_RESOLUTION;
                glues[(int)BorderTypeEnum.LEFT].LengthMax = GetPointLength(pleftin, pleftout) * INI.MAINSD_PAD_MIL_RESOLUTION;
                GetSideMaxMinValue(BorderTypeEnum.LEFT, out tempmax, out tempmin);
                bok = (glues[(int)BorderTypeEnum.LEFT].LengthMin < tempmin) || (glues[(int)BorderTypeEnum.LEFT].LengthMax > tempmax);
                cush = (!bok ? Color.Lime : Color.Red);
                _gGle.DrawLine(new Pen(cush, LineWidth), pleftin, pleftout);
                _gGle.DrawString($"左边距:{Math.Round(glues[(int)BorderTypeEnum.LEFT].LengthMin, 6)}mm", new Font("宋体", FontSize), new SolidBrush(cush), pleftout);
                _gGle.DrawString($"左边距:{Math.Round(glues[(int)BorderTypeEnum.LEFT].LengthMin, 6)}mm", new Font("宋体", FontSize), new SolidBrush(cush), 5, FontSize * 1 + 30);
                if (bok)
                    measuredStr += "胶水异常左边距";

                glues[(int)BorderTypeEnum.TOP].LengthMin = GetPointLength(pupin, pupout) * INI.MAINSD_PAD_MIL_RESOLUTION;
                glues[(int)BorderTypeEnum.TOP].LengthMax = GetPointLength(pupin, pupout) * INI.MAINSD_PAD_MIL_RESOLUTION;
                GetSideMaxMinValue(BorderTypeEnum.TOP, out tempmax, out tempmin);
                bok = (glues[(int)BorderTypeEnum.TOP].LengthMin < tempmin) || (glues[(int)BorderTypeEnum.TOP].LengthMax > tempmax);
                cush = (!bok ? Color.Lime : Color.Red);
                _gGle.DrawLine(new Pen(cush, LineWidth), pupin, pupout);
                _gGle.DrawString($"上边距:{Math.Round(glues[(int)BorderTypeEnum.TOP].LengthMin, 6)}mm", new Font("宋体", FontSize), new SolidBrush(cush), pupout);
                _gGle.DrawString($"上边距:{Math.Round(glues[(int)BorderTypeEnum.TOP].LengthMin, 6)}mm", new Font("宋体", FontSize), new SolidBrush(cush), 5, FontSize * 2 + 35);
                if (bok)
                    measuredStr += "胶水异常上边距";

                glues[(int)BorderTypeEnum.RIGHT].LengthMin = GetPointLength(prightin, prightout) * INI.MAINSD_PAD_MIL_RESOLUTION;
                glues[(int)BorderTypeEnum.RIGHT].LengthMax = GetPointLength(prightin, prightout) * INI.MAINSD_PAD_MIL_RESOLUTION;
                GetSideMaxMinValue(BorderTypeEnum.RIGHT, out tempmax, out tempmin);
                bok = (glues[(int)BorderTypeEnum.RIGHT].LengthMin < tempmin) || (glues[(int)BorderTypeEnum.RIGHT].LengthMax > tempmax);
                cush = (!bok ? Color.Lime : Color.Red);
                _gGle.DrawLine(new Pen(cush, LineWidth), prightin, prightout);
                _gGle.DrawString($"右边距:{Math.Round(glues[(int)BorderTypeEnum.RIGHT].LengthMin, 6)}mm", new Font("宋体", FontSize), new SolidBrush(cush), prightin);
                _gGle.DrawString($"右边距:{Math.Round(glues[(int)BorderTypeEnum.RIGHT].LengthMin, 6)}mm", new Font("宋体", FontSize), new SolidBrush(cush), 5, FontSize * 3 + 40);
                if (bok)
                    measuredStr += "胶水异常右边距";

                glues[(int)BorderTypeEnum.BOTTOM].LengthMin = GetPointLength(pbottomin, pbottomout) * INI.MAINSD_PAD_MIL_RESOLUTION;
                glues[(int)BorderTypeEnum.BOTTOM].LengthMax = GetPointLength(pbottomin, pbottomout) * INI.MAINSD_PAD_MIL_RESOLUTION;
                GetSideMaxMinValue(BorderTypeEnum.BOTTOM, out tempmax, out tempmin);
                bok = (glues[(int)BorderTypeEnum.BOTTOM].LengthMin < tempmin) || (glues[(int)BorderTypeEnum.BOTTOM].LengthMax > tempmax);
                cush = (!bok ? Color.Lime : Color.Red);
                _gGle.DrawLine(new Pen(cush, LineWidth), pbottomin, pbottomout);
                _gGle.DrawString($"下边距:{Math.Round(glues[(int)BorderTypeEnum.BOTTOM].LengthMin, 6)}mm", new Font("宋体", FontSize), new SolidBrush(cush), pbottomin);
                _gGle.DrawString($"下边距:{Math.Round(glues[(int)BorderTypeEnum.BOTTOM].LengthMin, 6)}mm", new Font("宋体", FontSize), new SolidBrush(cush), 5, FontSize * 4 + 45);
                if (bok)
                    measuredStr += "胶水异常下边距";

                double topbottomoffset = (glues[(int)BorderTypeEnum.TOP].LengthMax - glues[(int)BorderTypeEnum.BOTTOM].LengthMax) / 2;
                bok = (topbottomoffset < -GlueTopBottomOffset) || (topbottomoffset > GlueTopBottomOffset);
                cush = (!bok ? Color.Lime : Color.Red);
                _gGle.DrawString($"上-下边距偏移量:{Math.Round(topbottomoffset, 6)}mm", new Font("宋体", FontSize), new SolidBrush(cush), 5, FontSize * 5 + 50);
                if (bok)
                    measuredStr += "胶水异常上下边距偏移量";

                double leftrightoffset = (glues[(int)BorderTypeEnum.LEFT].LengthMax - glues[(int)BorderTypeEnum.RIGHT].LengthMax) / 2;
                bok = (leftrightoffset < -GlueLeftRightOffset) || (leftrightoffset > GlueLeftRightOffset);
                cush = (!bok ? Color.Lime : Color.Red);
                _gGle.DrawString($"左-右边距偏移量:{Math.Round(leftrightoffset, 6)}mm", new Font("宋体", FontSize), new SolidBrush(cush), 5, FontSize * 6 + 55);
                if (bok)
                    measuredStr += "胶水异常左右边距偏移量";

                RunDataOK = true;

                if (!string.IsNullOrEmpty(measuredStr))
                {
                    reason = ReasonEnum.NG;
                    descstriing = measuredStr;
                    isgood = false;
                }
            }
            else
            {
                _gGle.DrawString(descstriing, new Font("宋体", FontSize), Brushes.Red, 5, FontSize * 1 + 30);
            }
            #endregion

            _gGle.DrawPolygon(new Pen(Color.Yellow, LineWidth), PADTempRegion.RegionPtFCornerORG);
            _gGle.DrawPolygon(new Pen(Color.Blue, LineWidth), PADTempRegion.RegionPtFCornerQLE);
            _gGle.DrawRectangle(new Pen(Color.Lime, LineWidth), PADTempRegion.QleMaxRect);

            _gGle.Dispose();
            if (!isgood)
            {
                if (m_IsSaveTemp)
                {
                    bmpoutput.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "QLEFind" + (false ? "_Train" : "_Run") + ".png", System.Drawing.Imaging.ImageFormat.Png);
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
        public bool PB10_ChipCheckNoHaveProcess(Bitmap bmpinput, bool istrain, ref Bitmap bmpoutput)
        {
            //0.025/0.022 = 1 mil/1 pixel
            Resolution_Mil = 0.0254 / INI.MAINSD_PAD_MIL_RESOLUTION;
            m_DescStr = string.Empty;

            //bmpMeasureOutput.Dispose();
            ////bmpMeasureOutput = new Bitmap(bmpinput);
            //bmpMeasureOutput = (Bitmap)bmpinput.Clone();

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

            bmpInput = new Bitmap(bmpinput, bmpinput.Width >> 2, bmpinput.Height >> 2);

            //bmpPadFindOutput = new Bitmap(bmpinput);
            //bmpPadBolbOutput = new Bitmap(bmpinput);
            //bmpPadFindOutput = (Bitmap)bmpinput.Clone();
            //bmpPadBolbOutput = (Bitmap)bmpinput.Clone();

            bmpoutput.Dispose();
            bmpoutput = new Bitmap(bmpinput);

            #region 直接判断有没有芯片

            JzMVDIntensityMeasureClass jzMVDIntensityMeasureClass = new JzMVDIntensityMeasureClass();
            float meanLum = jzMVDIntensityMeasureClass.Run(bmpInput);
            isgood = meanLum > PADGrayThreshold;//小于此值代表无芯片
            if (!isgood)
            {
                isgood = false;
                processstring += $"Error 均值 {meanLum}   " + RelateAnalyzeString + Environment.NewLine;
                errorstring += RelateAnalyzeString + Environment.NewLine;

                reason = ReasonEnum.PASS;
                descstriing = "无芯片";
            }
            else
            {
                processstring += $"均值 {meanLum}    " + RelateAnalyzeString + Environment.NewLine;
                errorstring += RelateAnalyzeString + Environment.NewLine;
                reason = ReasonEnum.PASS;
                descstriing = "";
            }

            string msg = string.Empty;
            Graphics graphics = Graphics.FromImage(bmpoutput);
            if (isgood)
            {
                if (istrain)
                {
                    msg = $"测量数值 {meanLum} 大于 中心灰阶值 {PADGrayThreshold} 有芯片";
                    graphics.DrawString(msg, new Font("宋体", 25), Brushes.Lime, new PointF(5, 25));
                }
            }
            else
            {
                msg = $"测量数值 {meanLum} 小于 中心灰阶值 {PADGrayThreshold} 无芯片";
                graphics.DrawString(msg, new Font("宋体", 25), Brushes.Red, new PointF(5, 25));
            }

            graphics.Dispose();

            #endregion


            runstatus.SetWorkStatus(bmpPattern,
                                                   bmpInput,
                                                   bmpoutput,
                                                   reason,
                                                   errorstring,
                                                   processstring,
                                                   PassInfo,
                                                   descstriing);

            if (istrain)
                TrainStatusCollection.Add(runstatus);
            else
                RunStatusCollection.Add(runstatus);
            IsPass = isgood;
            m_DescStr = descstriing;

            bmpMeasureOutput.Dispose();
            bmpMeasureOutput = new Bitmap(bmpoutput);

            //if (m_IsSaveTemp)
            //{
            //    bmpoutput.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "CheckNoHave" + (false ? "_Train" : "_Run") + ".png", System.Drawing.Imaging.ImageFormat.Png);
            //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);

            //    //bmp.Save("D:\\testtest\\" + RelateAnalyzeString + "PadFind" + (eIsTrain ? "_Train" : "_Run") + ".png", System.Drawing.Imaging.ImageFormat.Png);
            //}

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

        public int GetGrayMinValue(Bitmap ebmpinput)
        {
            //Bitmap bmpFourSide = new Bitmap(ebmpinput);
            //截取芯片周围一部分 找到灰阶的最低值
            int min = m_PADRegion.GetGrayMin(ebmpinput);

            //bmpFourSide.Dispose();
            return min;
        }


        bool checkNoHaveTrain(Bitmap bmptrain)
        {
            if (m_IsSaveTemp)
            {
                bmptrain.Save("D:\\testtest\\" + RelateAnalyzeString + "bmpHPItemTrain" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }
            mVDPatMatchClass.bmpItem = bmptrain;
            return mVDPatMatchClass.Train();
        }
        public bool checkNoHaveRun(Bitmap bmprun)
        {
            if (m_IsSaveTemp)
            {
                bmprun.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpHPItemRun" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }
            N1Class n1Class = new N1Class();
            n1Class.FromString(ChipNoHaveModeOpString);
            mVDPatMatchClass.MTRotation = n1Class.Rotation;
            mVDPatMatchClass.MTTolerance = n1Class.Tolerance;
            mVDPatMatchClass.bmpFind = bmprun;
            return mVDPatMatchClass.Run();
        }
        /// <summary>
        /// 判断有无芯片 通过数blob个数
        /// </summary>
        /// <param name="bmprun">输入图片</param>
        /// <param name="eThreshold">二值化阈值</param>
        /// <param name="eWhite">找白斑</param>
        /// <param name="eBlobCountSetup">设定的个数</param>
        /// <returns></returns>
        public bool checkNoHaveRunBlobCount(Bitmap bmprun, int eThreshold = 128, bool eWhite = true,int eBlobCountSetup=15)
        {
            bool bret = false;

            VisionDesigner.ImageBinary.CImageBinaryTool cImageBinaryToolObj = null;
            VisionDesigner.BlobFind.CBlobFindTool cBlobFindToolObj = null;

            if (cImageBinaryToolObj == null)
                cImageBinaryToolObj = new VisionDesigner.ImageBinary.CImageBinaryTool();
            if (cBlobFindToolObj == null)
                cBlobFindToolObj = new VisionDesigner.BlobFind.CBlobFindTool();

            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            Bitmap bmp24 = grayscale.Apply(bmprun);
            VisionDesigner.CMvdImage cInputImg2 = BitmapToCMvdImage(bmp24);
            if (cInputImg2.PixelFormat != MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08)
            {
                //当前程序仅支持mono8。因此像素格会转换.
                cInputImg2.ConvertImagePixelFormat(MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08);
            }
            //cInputImg2.InitImage("InputTest2.bmp");

            //cInputImg2.SaveImage($"D:\\Data\\cInputImg2run_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.bmp", MVD_FILE_FORMAT.MVD_FILE_BMP);

            bmp24.Dispose();
            grayscale = null;

            //二值化
            cImageBinaryToolObj.InputImage = cInputImg2;// BitmapToCMvdImage(ebmpInput);
            cImageBinaryToolObj.ROI = null;
            cImageBinaryToolObj.SetRunParam("LowThreshold", eThreshold.ToString());
            //cImageArithmeticToolObj.SetRunParam("HighThreshold", BlobHighThreshold.ToString());
            cImageBinaryToolObj.Run();

            //blob
            cBlobFindToolObj.InputImage = cImageBinaryToolObj.Result.OutputImage;
            //cBlobFindToolObj.RegionImage = BitmapToCMvdImage(eBmpMask);

            if (m_IsSaveTemp)
            {
                cInputImg2.SaveImage("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + $"bmpBlobItemRun0" + ".png", MVD_FILE_FORMAT.MVD_FILE_JPEG);
                cImageBinaryToolObj.Result.OutputImage.SaveImage("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + $"bmpBlobItemRun1" + ".png", MVD_FILE_FORMAT.MVD_FILE_JPEG);

            }

            cBlobFindToolObj.ROI = null;
            if (eWhite)
                cBlobFindToolObj.SetRunParam("Polarity", "BrightObject");
            else
                cBlobFindToolObj.SetRunParam("Polarity", "DarkObject");
            cBlobFindToolObj.BasicParam.ShowBlobImageStatus = true;
            cBlobFindToolObj.Run();
            VisionDesigner.BlobFind.CBlobFindResult cBlobFindRes = cBlobFindToolObj.Result;
            
            //找到的blob个数 小于 设定值 代表有芯片
            bret = cBlobFindToolObj.Result.BlobInfo.Count < eBlobCountSetup;

            if (m_IsSaveTemp)
            {
                cBlobFindRes.BlobImage.SaveImage("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + $"bmpBlobItemRun[{cBlobFindToolObj.Result.BlobInfo.Count}]" + ".png", MVD_FILE_FORMAT.MVD_FILE_JPEG);
            }

            cImageBinaryToolObj.Dispose();
            cBlobFindToolObj.Dispose();

            cImageBinaryToolObj = null;
            cBlobFindToolObj = null;


            return bret;
        }
        private CMvdImage BitmapToCMvdImage(Bitmap bmpInputImg)
        {
            CMvdImage cMvdImage = new CMvdImage();
            System.Drawing.Imaging.PixelFormat bitPixelFormat = bmpInputImg.PixelFormat;
            BitmapData bmData = bmpInputImg.LockBits(new Rectangle(0, 0, bmpInputImg.Width, bmpInputImg.Height), ImageLockMode.ReadOnly, bitPixelFormat);//锁定

            if (bitPixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
            {
                Int32 bitmapDataSize = bmData.Stride * bmData.Height;//bitmap图像缓存长度
                int offset = bmData.Stride - bmData.Width;
                Int32 ImageBaseDataSize = bmData.Width * bmData.Height;//imageBaseData_V2图像真正的缓存长度
                byte[] _BitImageBufferBytes = new byte[bitmapDataSize];
                byte[] _ImageBaseDataBufferBytes = new byte[ImageBaseDataSize];
                Marshal.Copy(bmData.Scan0, _BitImageBufferBytes, 0, bitmapDataSize);
                int bitmapIndex = 0;
                int ImageBaseDataIndex = 0;
                for (int i = 0; i < bmData.Height; i++)
                {
                    for (int j = 0; j < bmData.Width; j++)
                    {
                        _ImageBaseDataBufferBytes[ImageBaseDataIndex++] = _BitImageBufferBytes[bitmapIndex++];
                    }
                    bitmapIndex += offset;
                }
                MVD_IMAGE_DATA_INFO stImageData = new MVD_IMAGE_DATA_INFO();
                stImageData.stDataChannel[0].nRowStep = (uint)bmData.Width;
                stImageData.stDataChannel[0].nLen = (uint)ImageBaseDataSize;
                stImageData.stDataChannel[0].nSize = (uint)ImageBaseDataSize;
                stImageData.stDataChannel[0].arrDataBytes = _ImageBaseDataBufferBytes;
                cMvdImage.InitImage((uint)bmData.Width, (uint)bmData.Height, MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08, stImageData);
            }
            else if (bitPixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
            {
                Int32 bitmapDataSize = bmData.Stride * bmData.Height;//bitmap图像缓存长度
                int offset = bmData.Stride - bmData.Width * 3;
                Int32 ImageBaseDataSize = bmData.Width * bmData.Height * 3;//imageBaseData_V2图像真正的缓存长度
                byte[] _BitImageBufferBytes = new byte[bitmapDataSize];
                byte[] _ImageBaseDataBufferBytes = new byte[ImageBaseDataSize];
                Marshal.Copy(bmData.Scan0, _BitImageBufferBytes, 0, bitmapDataSize);
                int bitmapIndex = 0;
                int ImageBaseDataIndex = 0;
                for (int i = 0; i < bmData.Height; i++)
                {
                    for (int j = 0; j < bmData.Width; j++)
                    {
                        _ImageBaseDataBufferBytes[ImageBaseDataIndex++] = _BitImageBufferBytes[bitmapIndex + 2];
                        _ImageBaseDataBufferBytes[ImageBaseDataIndex++] = _BitImageBufferBytes[bitmapIndex + 1];
                        _ImageBaseDataBufferBytes[ImageBaseDataIndex++] = _BitImageBufferBytes[bitmapIndex];
                        bitmapIndex += 3;
                    }
                    bitmapIndex += offset;
                }
                MVD_IMAGE_DATA_INFO stImageData = new MVD_IMAGE_DATA_INFO();
                stImageData.stDataChannel[0].nRowStep = (uint)bmData.Width * 3;
                stImageData.stDataChannel[0].nLen = (uint)ImageBaseDataSize;
                stImageData.stDataChannel[0].nSize = (uint)ImageBaseDataSize;
                stImageData.stDataChannel[0].arrDataBytes = _ImageBaseDataBufferBytes;
                cMvdImage.InitImage((uint)bmData.Width, (uint)bmData.Height, MVD_PIXEL_FORMAT.MVD_PIXEL_RGB_RGB24_C3, stImageData);
            }
            bmpInputImg.UnlockBits(bmData);  // 解除锁定
            return cMvdImage;
        }


        #region 银胶检测异形的方法

        public RectangleF FindAxisAlignedBoundingRectangle(PointF[] points)
        {
            if (points == null || points.Length != 4)
                throw new ArgumentException("Exactly four points are required.");

            float minX = points.Min(p => p.X);
            float maxX = points.Max(p => p.X);
            float minY = points.Min(p => p.Y);
            float maxY = points.Max(p => p.Y);

            PointF bottomLeft = new PointF(minX, minY);
            PointF topRight = new PointF(maxX, maxY);

            return new RectangleF(
                bottomLeft.X,
                bottomLeft.Y,
                Math.Abs(topRight.X - bottomLeft.X),
                Math.Abs(topRight.Y - bottomLeft.Y));
        }

        public bool CheckYinJiaoIrregular(bool bTrain, Bitmap bmpInput, RectangleF eRect)
        {
            bool bret = false;

            PADG2Class pADG2Class = new PADG2Class();
            pADG2Class.FromString(PADINSPECTOPString);

            if (bTrain)
            {
                EzMVDPatMatchPADG2.bmpItem = (Bitmap)bmpInput.Clone(eRect, bmpInput.PixelFormat);
                bret = EzMVDPatMatchPADG2.Train();

                pADG2Class.bmpRun.Dispose();
                pADG2Class.bmpRun = (Bitmap)bmpInput.Clone(eRect, bmpInput.PixelFormat);

                RectangleF rectCrop = FindAxisAlignedBoundingRectangle(m_PADRegion.RegionPtFCornerQLE);
                BoundRect(ref rectCrop, bmpPattern.Size);
                pADG2Class.bmpMask.Dispose();
                pADG2Class.bmpMask = (Bitmap)bmpPattern.Clone(rectCrop, bmpInput.PixelFormat);

                if (pADG2Class.myMover.Count > 0)
                {
                    using (Graphics gfx = Graphics.FromImage(pADG2Class.bmpMask))
                    {
                        gfx.Clear(Color.White);
                        foreach (var geof in iterAllGeoFigures(pADG2Class.myMover, false))
                        {
                            if (geof != null)
                            {
                                //geof.PenWidth = 5;
                                //geof.FontSize = 100;
                                //geof.MainShowPen.Width = 5;
                                geof.DrawMask(gfx, new PointF(0, 0), 0, null, new SolidBrush(Color.Black), new Size(1, 1));
                                //geof.MainShowPen.Width = 1;
                                //geof.PenWidth = PenWidth;
                                //geof.FontSize = FontSize;
                            }
                        }
                    }
                }


            }
            else
            {

                EzMVDPatMatchPADG2.MTRotation = pADG2Class.Rotation;
                EzMVDPatMatchPADG2.MTTolerance = pADG2Class.Tolerance;
                EzMVDPatMatchPADG2.bmpFind = bmpInput;
                bret = EzMVDPatMatchPADG2.Run();
                if (bret)
                {
                    pADG2Class.bmpRun.Dispose();
                    pADG2Class.bmpRun = (Bitmap)bmpInput.Clone(EzMVDPatMatchPADG2.xFindLastRect, bmpInput.PixelFormat);

                    RectangleF rectCrop = FindAxisAlignedBoundingRectangle(m_PADRegion.RegionPtFCornerQLE);
                    BoundRect(ref rectCrop, bmpPattern.Size);
                    pADG2Class.bmpMask.Dispose();
                    pADG2Class.bmpMask = (Bitmap)bmpPattern.Clone(rectCrop, bmpInput.PixelFormat);

                    if (pADG2Class.myMover.Count > 0)
                    {
                        using (Graphics gfx = Graphics.FromImage(pADG2Class.bmpMask))
                        {
                            gfx.Clear(Color.White);
                            foreach (var geof in iterAllGeoFigures(pADG2Class.myMover, false))
                            {
                                if (geof != null)
                                {
                                    //geof.PenWidth = 5;
                                    //geof.FontSize = 100;
                                    //geof.MainShowPen.Width = 5;
                                    geof.DrawMask(gfx, new PointF(0, 0), 0, null, new SolidBrush(Color.Black), new Size(1, 1));
                                    //geof.MainShowPen.Width = 1;
                                    //geof.PenWidth = PenWidth;
                                    //geof.FontSize = FontSize;
                                }
                            }
                        }
                    }
                }
                //else
                //{
                //    RectangleF rectCrop = FindAxisAlignedBoundingRectangle(m_PADRegion.RegionPtFCornerQLE);
                //    BoundRect(ref rectCrop, bmpInput.Size);
                //    pADG2Class.bmpRun.Dispose();
                //    pADG2Class.bmpRun = (Bitmap)bmpInput.Clone(rectCrop, bmpInput.PixelFormat);
                //}

                if (m_IsSaveTemp)
                {
                    pADG2Class.bmpRun.Save("D:\\testtest\\" + RelateAnalyzeString + $"bmpIrregualr_{(bTrain ? "Train" : "Run")}" + ".png",
                        System.Drawing.Imaging.ImageFormat.Png);
                }
            }

            #region 找blob
            if (bret)
            {
                using (Bitmap bmprun = get8bitbmp(pADG2Class.bmpRun))
                {
                    using (Bitmap bmpmask = get8bitbmp(pADG2Class.bmpMask))
                    {
                        if (m_IsSaveTemp)
                        {
                            bmpmask.Save("D:\\testtest\\" + RelateAnalyzeString + $"bmpIrregualrMask_{(bTrain ? "Train" : "Run")}" + ".png",
                                System.Drawing.Imaging.ImageFormat.Png);
                            bmprun.Save("D:\\testtest\\" + RelateAnalyzeString + $"bmpIrregualrRun_{(bTrain ? "Train" : "Run")}" + ".png",
                             System.Drawing.Imaging.ImageFormat.Png);
                        }

                        EzMVDBLOB.ThresholdValue = pADG2Class.ThresholdValue;
                        EzMVDBLOB.IsWhite = pADG2Class.IsWhite;
                        EzMVDBLOB.blobMin = pADG2Class.blobMin;
                        EzMVDBLOB.blobMax = pADG2Class.blobMax;
                        EzMVDBLOB.blobRatio = pADG2Class.blobRatio;
                        EzMVDBLOB.FindBaseArea = m_PADRegion.RegionArea;
                        bret = EzMVDBLOB.Run(bmprun, bmpmask);

                        if (bTrain)
                        {
                            bmpPadBolbOutput?.Dispose();
                            bmpPadBolbOutput = new Bitmap(bmprun);
                        }
                        else
                        {
                            if (!bret)
                            {
                                if (EzMVDBLOB.NGBounds.Count > 0)
                                {
                                    bmpPadBolbOutput?.Dispose();
                                    bmpPadBolbOutput = new Bitmap(bmprun);

                                    Graphics graphics = Graphics.FromImage(bmpPadBolbOutput);
                                    graphics.DrawRectangles(new Pen(Color.Red, 3), EzMVDBLOB.NGBounds.ToArray());
                                    graphics.Dispose();

                                    if (m_IsSaveTemp)
                                    {
                                        bmpPadBolbOutput.Save("D:\\testtest\\" + RelateAnalyzeString + $"bmpIrregualrResult_{(bTrain ? "Train" : "Run")}" + ".png",
                                            System.Drawing.Imaging.ImageFormat.Png);
                                    }
                                }
                            }
                            else
                            {
                                bmpPadBolbOutput?.Dispose();
                                bmpPadBolbOutput = new Bitmap(bmprun);
                            }
                        }
                    }
                }
            }
            else
            {
                using (Bitmap bmprun = get8bitbmp(pADG2Class.bmpRun))
                {
                    bmpPadBolbOutput?.Dispose();
                    bmpPadBolbOutput = new Bitmap(bmprun);

                    //Graphics graphics = Graphics.FromImage(bmpPadBolbOutput);
                    //graphics.DrawString($"", new Font("宋体", 18), Brushes.Red, new PointF(5, 5));
                    //graphics.Dispose();

                }
            }
            #endregion

            return bret;
        }

        Bitmap get8bitbmp(Bitmap ebmp)
        {
            using (Bitmap bigBmp = new Bitmap(ebmp))
            {
                var pixelFormat = bigBmp.PixelFormat;
                if (pixelFormat == PixelFormat.Format32bppArgb)
                {
                    var bmp = Convert32bppTo8bpp(bigBmp);
                    return bmp;
                }
                else if (pixelFormat == PixelFormat.Format24bppRgb)
                {
                    var bmp = Convert24bppTo8bpp(bigBmp);
                    return bmp;

                }
                else if (pixelFormat == PixelFormat.Format8bppIndexed)
                {
                    return (Bitmap)bigBmp.Clone();
                }
                else
                {
                    throw new Exception("加载图片格式不支持！");
                }
            }
        }

        Bitmap Convert32bppTo8bpp(Bitmap original)
        {
            // 创建一个新的8bpp位图
            Bitmap newBitmap = new Bitmap(original.Width, original.Height, PixelFormat.Format8bppIndexed);

            // 设置调色板（这里使用灰度调色板）
            ColorPalette palette = newBitmap.Palette;
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            newBitmap.Palette = palette;

            // 锁定位图数据
            BitmapData originalData = original.LockBits(
                new Rectangle(0, 0, original.Width, original.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            BitmapData newData = newBitmap.LockBits(
                new Rectangle(0, 0, newBitmap.Width, newBitmap.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            // 转换像素数据
            unsafe
            {
                byte* originalPtr = (byte*)originalData.Scan0;
                byte* newPtr = (byte*)newData.Scan0;

                for (int y = 0; y < original.Height; y++)
                {
                    for (int x = 0; x < original.Width; x++)
                    {
                        // 获取32bpp像素值
                        byte b = originalPtr[y * originalData.Stride + x * 4];
                        byte g = originalPtr[y * originalData.Stride + x * 4 + 1];
                        byte r = originalPtr[y * originalData.Stride + x * 4 + 2];
                        byte a = originalPtr[y * originalData.Stride + x * 4 + 3];

                        // 转换为灰度值（8bpp）
                        byte gray = (byte)((r * 0.299 + g * 0.587 + b * 0.114) * (a / 255.0));

                        // 写入8bpp位图
                        newPtr[y * newData.Stride + x] = gray;
                    }
                }
            }

            // 解锁位图
            original.UnlockBits(originalData);
            newBitmap.UnlockBits(newData);

            return newBitmap;
        }
        Bitmap Convert24bppTo8bpp(Bitmap original)
        {
            //if (original.PixelFormat != PixelFormat.Format24bppRgb)
            //    throw new ArgumentException("源图像必须是24位位图");

            // 创建新的8位位图
            Bitmap newBitmap = new Bitmap(original.Width, original.Height, PixelFormat.Format8bppIndexed);

            // 设置灰度调色板
            ColorPalette palette = newBitmap.Palette;
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            newBitmap.Palette = palette;

            // 锁定位图数据进行操作
            BitmapData originalData = original.LockBits(
                new Rectangle(0, 0, original.Width, original.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            BitmapData newData = newBitmap.LockBits(
                new Rectangle(0, 0, newBitmap.Width, newBitmap.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            unsafe
            {
                byte* originalPtr = (byte*)originalData.Scan0;
                byte* newPtr = (byte*)newData.Scan0;

                for (int y = 0; y < original.Height; y++)
                {
                    for (int x = 0; x < original.Width; x++)
                    {
                        // 获取24bpp像素值
                        byte b = originalPtr[y * originalData.Stride + x * 3];
                        byte g = originalPtr[y * originalData.Stride + x * 3 + 1];
                        byte r = originalPtr[y * originalData.Stride + x * 3 + 2];

                        // 转换为灰度值（8bpp）
                        byte gray = (byte)(r * 0.299 + g * 0.587 + b * 0.114);

                        // 写入8bpp位图
                        newPtr[y * newData.Stride + x] = gray;
                    }
                }
            }

            // 解锁位图
            original.UnlockBits(originalData);
            newBitmap.UnlockBits(newData);

            return newBitmap;
        }

        #region PRIVATE_ITERATION_FUNCTIONS
        /// <summary>
        /// 迭代尋訪 指定 JzMover 與 JzStaticMover 內所有的 GeoFigure
        /// </summary>
        IEnumerable<GeoFigure> iterAllGeoFigures(Mover mover, bool reverse = false)
        {
            foreach (var geo in iterGeoFigures(mover, reverse))
                yield return geo;
            //foreach (var geo in iterGeoFigures(mover, reverse))
            //    yield return geo;
        }
        /// <summary>
        /// 迭代尋訪 指定 Mover 內所有的 GeoFigure
        /// </summary>
        IEnumerable<GeoFigure> iterGeoFigures(Mover mover, bool reverse = false)
        {
            if (mover != null)
            {
                if (reverse)
                {
                    for (int i = mover.Count - 1; i >= 0; i--)
                        if (mover[i].Source is GeoFigure geof)
                            yield return geof;
                        else
                            yield return null;
                }
                else
                {
                    for (int i = 0, N = mover.Count; i < N; i++)
                        if (mover[i].Source is GeoFigure geof)
                            yield return geof;
                        else
                            yield return null;
                }
            }
        }
        #endregion

        #endregion


        void filterORG001(Bitmap ebmpinput)
        {
            AForge.Imaging.Filters.HistogramEqualization histogramEqualization11 =
                        new AForge.Imaging.Filters.HistogramEqualization();
            ebmpinput = histogramEqualization11.Apply(ebmpinput);
        }
        void filterVictor001(Bitmap ebmpinput)
        {
            ExtractChannel extract = new ExtractChannel();
            extract.Channel = 1;
            ebmpinput = extract.Apply(ebmpinput);

            //AForge.Imaging.Filters.HistogramEqualization histogramEqualization11 =
            //            new AForge.Imaging.Filters.HistogramEqualization();
            //ebmpinput = histogramEqualization11.Apply(ebmpinput);

            ContrastStretch contrastStretch = new ContrastStretch();
            ebmpinput = contrastStretch.Apply(ebmpinput);

            Erosion erosion = new Erosion();
            int i = 0;
            while (i < 3)
            {
                ebmpinput = erosion.Apply(ebmpinput);
                i++;
            }
        }
        Bitmap filterVictor002(Bitmap ebmpinput, double ratio)
        {
            ExtractChannel extract = new ExtractChannel();
            extract.Channel = 1;
            ebmpinput = extract.Apply(ebmpinput);

            //AForge.Controls.Histogram histogram = new AForge.Controls.Histogram();


            //JzHistogramClass jzHistogram = new JzHistogramClass(1);
            //JzHistogramResult jzhresult = new JzHistogramResult();

            Bitmap bb = ebmpinput.Clone(new Rectangle(0, 0, ebmpinput.Width, ebmpinput.Height), PixelFormat.Format8bppIndexed);
            ImageStatistics imageStatistics = new ImageStatistics(bb);
            AForge.Math.Histogram activeHistogram = imageStatistics.Gray;
            //int[] histogramdata = new int[256];
            //jzHistogram.GetHistogramData(bb,
            //        new Rectangle(0, 0, bb.Width, bb.Height),
            //        histogramdata);

            //GetHistogramData(histogramdata, jzhresult);

            int _thresholdvalue = (int)((activeHistogram.Median - activeHistogram.Min) * 2 / 3 + activeHistogram.Min);
            _thresholdvalue = (int)((activeHistogram.Median - activeHistogram.Min) * ratio + activeHistogram.Min);
            AForge.Imaging.Filters.Threshold threshold = new AForge.Imaging.Filters.Threshold(_thresholdvalue);
            ebmpinput = threshold.Apply(ebmpinput);

            bb.Dispose();
            //ebmpinput.Save("D:\\LOA\\11.png");
            return ebmpinput;
        }


        bool checkOutGlueBAK1(Bitmap eInput, bool istrain = false)
        {
            //return checkOutGlueV2(eInput, istrain);

            bool ret = true;

            if (!string.IsNullOrEmpty(PADINSPECTOPString))
            {
                string[] strs = PADINSPECTOPString.Split(',');
                if (strs.Length > 5)
                {
                    GetRGB = short.Parse(strs[0]);
                    numThresholdRatio = int.Parse(strs[1]);
                    numObjectFilterRatio = int.Parse(strs[2]);
                    numEDCount = int.Parse(strs[3]);
                    numShortenRatio = int.Parse(strs[4]);
                    txtBangBangRectStr = strs[5];
                }
                if (strs.Length > 8)
                {
                    numBangBangOffsetVal = int.Parse(strs[6]);
                    txtNeverOutsideRect = strs[7];
                    cboIPDMethod = int.Parse(strs[8]);
                }
            }

            if (m_IsSaveTemp)
            {
                eInput.Save("D:\\testtest\\" + _CalPageIndex() +
                    RelateAnalyzeString + "outCheckGlue_input" + ".png", System.Drawing.Imaging.ImageFormat.Png);

            }

            Bitmap ebmpinput = new Bitmap(eInput);
            Bitmap ebmpinputDraw = new Bitmap(eInput);

            //填充
            Graphics graphics = Graphics.FromImage(ebmpinput);
            graphics.FillRectangles(Brushes.White, new RectangleF[] { StringtoRect(txtNeverOutsideRect) });//no use
            graphics.Dispose();

            //抽取绿色
            ExtractChannel extract = new ExtractChannel();
            extract.Channel = 1;
            ebmpinput = extract.Apply(ebmpinput);
            //local
            BradleyLocalThresholding bradleyLocalThresholding = new BradleyLocalThresholding();
            ebmpinput = bradleyLocalThresholding.Apply(ebmpinput);

            //反向
            Invert invert = new Invert();
            ebmpinput = invert.Apply(ebmpinput);

            if (m_IsSaveTemp)
            {
                ebmpinput.Save("D:\\testtest\\" + _CalPageIndex() +
                    RelateAnalyzeString + "outCheckGlue_org_" + (istrain ? "_train" : "_run") + ".png", System.Drawing.Imaging.ImageFormat.Png);

            }

            //找球的blob 记录起来
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.ProcessImage(ebmpinput);
            Blob[] blobs = blobCounter.GetObjectsInformation();
            List<Rectangle> rectangles = new List<Rectangle>();
            int filterArea = 150;
            Graphics graphicsXX = Graphics.FromImage(ebmpinputDraw);
            if (istrain)
            {
                if (blobs.Length > 0)
                {
                    foreach (Blob bb in blobs)
                    {
                        if (bb.Area > filterArea)
                        {
                            BallBlobClass ballBlob = new BallBlobClass();
                            ballBlob.rect = bb.Rectangle;
                            ballBlob.area = bb.Area;
                            m_PADRegion.ballBlobClasses.Add(ballBlob);

                            rectangles.Add(bb.Rectangle);
                        }
                    }
                }
            }
            else
            {

                List<Rectangle> rectanglesIN = new List<Rectangle>();
                foreach (BallBlobClass ballBlobClass in m_PADRegion.ballBlobClasses)
                {
                    rectanglesIN.Add(ballBlobClass.rect);
                }
                graphicsXX.DrawRectangles(new Pen(Color.Yellow, 3), rectanglesIN.ToArray());
                if (blobs.Length > 0)
                {
                    foreach (Blob bb in blobs)
                    {
                        if (bb.Area > filterArea)
                        {
                            //BallBlobClass ballBlob = new BallBlobClass();
                            //ballBlob.rect = bb.Rectangle;
                            //ballBlob.area = bb.Area;
                            //m_PADRegion.ballBlobClasses.Add(ballBlob);

                            rectangles.Add(bb.Rectangle);
                        }
                    }
                }
            }
            graphicsXX.DrawRectangles(new Pen(Color.Red, 3), rectangles.ToArray());
            graphicsXX.Dispose();

            if (m_IsSaveTemp)
            {
                ebmpinputDraw.Save("D:\\testtest\\" + _CalPageIndex() +
                    RelateAnalyzeString + "outCheckGlue" + (istrain ? "_train" : "_run") + ".png", System.Drawing.Imaging.ImageFormat.Png);

            }

            return ret;
        }
        bool checkOutGlueBAK2(Bitmap eInput, out Bitmap bmpResult, bool istrain = false)
        {
            bool ret = true;

            if (!string.IsNullOrEmpty(PADINSPECTOPString))
            {
                string[] strs = PADINSPECTOPString.Split(',');
                if (strs.Length > 5)
                {
                    GetRGB = short.Parse(strs[0]);
                    numThresholdRatio = int.Parse(strs[1]);
                    numObjectFilterRatio = int.Parse(strs[2]);
                    numEDCount = int.Parse(strs[3]);
                    numShortenRatio = int.Parse(strs[4]);
                    txtBangBangRectStr = strs[5];
                }
                if (strs.Length > 8)
                {
                    numBangBangOffsetVal = int.Parse(strs[6]);
                    txtNeverOutsideRect = strs[7];
                    cboIPDMethod = int.Parse(strs[8]);
                }
            }

            if (m_IsSaveTemp)
            {
                eInput.Save("D:\\testtest\\" + _CalPageIndex() +
                    RelateAnalyzeString + "outCheckGlue_input" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }

            Bitmap ebmpinput = new Bitmap(eInput);
            bmpResult = new Bitmap(eInput);

            //抽取绿色
            ExtractChannel extract = new ExtractChannel();
            extract.Channel = 1;
            ebmpinput = extract.Apply(ebmpinput);

            Mean mean = new Mean();
            ebmpinput = mean.Apply(ebmpinput);
            //local
            BradleyLocalThresholding bradleyLocalThresholding = new BradleyLocalThresholding();
            ebmpinput = bradleyLocalThresholding.Apply(ebmpinput);


            Bitmap bmpGR = new Bitmap(ebmpinput);
            //填充
            Graphics graphics = Graphics.FromImage(bmpGR);
            graphics.FillRectangles(Brushes.White, new RectangleF[] { StringtoRect(txtNeverOutsideRect) });//no use
            graphics.Dispose();

            //Grayscale grayscale = new Grayscale(0.299, 0.587, 0.114);
            bmpGR = extract.Apply(bmpGR);

            //反向
            Invert invert = new Invert();
            bmpGR = invert.Apply(bmpGR);

            if (m_IsSaveTemp)
            {
                bmpGR.Save("D:\\testtest\\" + _CalPageIndex() +
                    RelateAnalyzeString + "outCheckGlue_org_" + (istrain ? "_train" : "_run") + ".png", System.Drawing.Imaging.ImageFormat.Png);

            }

            //找球的blob 记录起来
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.ProcessImage(bmpGR);
            Blob[] blobs = blobCounter.GetObjectsInformation();
            List<Rectangle> rectangles = new List<Rectangle>();
            int filterArea = 150;
            Graphics graphicsXX = Graphics.FromImage(bmpResult);
            if (istrain)
            {
                if (blobs.Length > 0)
                {
                    foreach (Blob bb in blobs)
                    {
                        if (bb.Area > filterArea)
                        {
                            BallBlobClass ballBlob = new BallBlobClass();
                            ballBlob.rect = bb.Rectangle;
                            ballBlob.area = bb.Area;
                            m_PADRegion.ballBlobClasses.Add(ballBlob);

                            rectangles.Add(bb.Rectangle);
                        }
                    }
                }
            }
            else
            {

                List<Rectangle> rectanglesIN = new List<Rectangle>();
                foreach (BallBlobClass ballBlobClass in m_PADRegion.ballBlobClasses)
                {
                    rectanglesIN.Add(ballBlobClass.rect);
                }
                if (rectanglesIN.Count > 0)
                    graphicsXX.DrawRectangles(new Pen(Color.Yellow, 3), rectanglesIN.ToArray());
                if (blobs.Length > 0)
                {
                    foreach (Blob bb in blobs)
                    {
                        if (bb.Area > filterArea)
                        {
                            //BallBlobClass ballBlob = new BallBlobClass();
                            //ballBlob.rect = bb.Rectangle;
                            //ballBlob.area = bb.Area;
                            //m_PADRegion.ballBlobClasses.Add(ballBlob);

                            rectangles.Add(bb.Rectangle);
                            int _count = 0;
                            foreach (BallBlobClass ballBlobClass in m_PADRegion.ballBlobClasses)
                            {
                                if (bb.Rectangle.IntersectsWith(ballBlobClass.rect))
                                {
                                    _count++;
                                }
                            }
                            if (_count >= 2)
                            {
                                ret = false;
                                break;
                            }
                        }
                    }
                }

                //ret = rectangles.Count == m_PADRegion.ballBlobClasses.Count;

            }
            if (rectangles.Count > 0)
                graphicsXX.DrawRectangles(new Pen(Color.Red, 3), rectangles.ToArray());
            graphicsXX.Dispose();

            if (m_IsSaveTemp)
            {
                bmpResult.Save("D:\\testtest\\" + _CalPageIndex() +
                    RelateAnalyzeString + "outCheckGlue" + (istrain ? "_train" : "_run") + ".png", System.Drawing.Imaging.ImageFormat.Png);

            }

            bmpGR.Dispose();
            //eInput.Dispose();

            return ret;
        }
        bool checkOutGlue(Bitmap eInput, out Bitmap bmpResult, bool istrain = false)
        {
            bool ret = true;

            if (!string.IsNullOrEmpty(PADINSPECTOPString))
            {
                string[] strs = PADINSPECTOPString.Split(',');
                if (strs.Length > 5)
                {
                    GetRGB = short.Parse(strs[0]);
                    numThresholdRatio = int.Parse(strs[1]);
                    numObjectFilterRatio = int.Parse(strs[2]);
                    numEDCount = int.Parse(strs[3]);
                    numShortenRatio = int.Parse(strs[4]);
                    txtBangBangRectStr = strs[5];
                }
                if (strs.Length > 8)
                {
                    numBangBangOffsetVal = int.Parse(strs[6]);
                    txtNeverOutsideRect = strs[7];
                    cboIPDMethod = int.Parse(strs[8]);
                }
            }

            if (m_IsSaveTemp)
            {
                eInput.Save("D:\\testtest\\" + _CalPageIndex() +
                    RelateAnalyzeString + "outCheckGlue_input" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }

            Bitmap _bmp000 = new Bitmap(eInput);
            Bitmap _bmp001 = new Bitmap(eInput);
            bmpResult = new Bitmap(eInput);

            //抽取绿色
            ExtractChannel extract = new ExtractChannel();
            extract.Channel = 1;
            _bmp000 = extract.Apply(_bmp000);

            CannyEdgeDetector detector = new CannyEdgeDetector(10, 50);
            _bmp000 = detector.Apply(_bmp000);

            SISThreshold sISThreshold = new SISThreshold();
            _bmp000 = sISThreshold.Apply(_bmp000);

            _bmp001 = extract.Apply(_bmp001);
            HistogramEqualization histogramEqualization = new HistogramEqualization();
            _bmp001 = histogramEqualization.Apply(_bmp001);

            Add add = new Add(_bmp000);
            _bmp001 = add.Apply(_bmp001);

            //local
            BradleyLocalThresholding bradleyLocalThresholding = new BradleyLocalThresholding();
            _bmp001 = bradleyLocalThresholding.Apply(_bmp001);


            Bitmap bmpGR = new Bitmap(_bmp001);
            //填充
            Graphics graphics = Graphics.FromImage(bmpGR);
            graphics.FillRectangles(Brushes.White, new RectangleF[] { StringtoRect(txtNeverOutsideRect) });
            graphics.Dispose();

            //Grayscale grayscale = new Grayscale(0.299, 0.587, 0.114);
            bmpGR = extract.Apply(bmpGR);

            //反向
            Invert invert = new Invert();
            bmpGR = invert.Apply(bmpGR);

            if (m_IsSaveTemp)
            {
                bmpGR.Save("D:\\testtest\\" + _CalPageIndex() +
                    RelateAnalyzeString + "outCheckGlue_org_" + (istrain ? "_train" : "_run") + ".png", System.Drawing.Imaging.ImageFormat.Png);

            }

            //找球的blob 记录起来
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.ProcessImage(bmpGR);
            Blob[] blobs = blobCounter.GetObjectsInformation();
            List<Rectangle> rectangles = new List<Rectangle>();
            int filterArea = 150;
            Graphics graphicsXX = Graphics.FromImage(bmpResult);
            if (istrain)
            {
                if (blobs.Length > 0)
                {
                    foreach (Blob bb in blobs)
                    {
                        if (bb.Area > filterArea)
                        {
                            BallBlobClass ballBlob = new BallBlobClass();
                            ballBlob.rect = bb.Rectangle;
                            ballBlob.area = bb.Area;
                            m_PADRegion.ballBlobClasses.Add(ballBlob);

                            rectangles.Add(bb.Rectangle);
                        }
                    }
                }
            }
            else
            {

                List<Rectangle> rectanglesIN = new List<Rectangle>();
                foreach (BallBlobClass ballBlobClass in m_PADRegion.ballBlobClasses)
                {
                    rectanglesIN.Add(ballBlobClass.rect);
                }
                if (rectanglesIN.Count > 0)
                    graphicsXX.DrawRectangles(new Pen(Color.Yellow, 3), rectanglesIN.ToArray());
                if (blobs.Length > 0)
                {
                    foreach (Blob bb in blobs)
                    {
                        if (bb.Area > filterArea)
                        {
                            //BallBlobClass ballBlob = new BallBlobClass();
                            //ballBlob.rect = bb.Rectangle;
                            //ballBlob.area = bb.Area;
                            //m_PADRegion.ballBlobClasses.Add(ballBlob);

                            rectangles.Add(bb.Rectangle);
                            int _count = 0;
                            foreach (BallBlobClass ballBlobClass in m_PADRegion.ballBlobClasses)
                            {
                                if (bb.Rectangle.IntersectsWith(ballBlobClass.rect))
                                {
                                    _count++;
                                }
                            }
                            if (_count >= 2)
                            {
                                ret = false;
                                break;
                            }
                        }
                    }
                }

                //ret = rectangles.Count == m_PADRegion.ballBlobClasses.Count;

            }
            if (rectangles.Count > 0)
                graphicsXX.DrawRectangles(new Pen(Color.Red, 3), rectangles.ToArray());
            graphicsXX.Dispose();

            if (m_IsSaveTemp)
            {
                bmpResult.Save("D:\\testtest\\" + _CalPageIndex() +
                    RelateAnalyzeString + "outCheckGlue" + (istrain ? "_train" : "_run") + ".png", System.Drawing.Imaging.ImageFormat.Png);

            }

            bmpGR.Dispose();
            //eInput.Dispose();

            return ret;
        }
        bool checkOutGlueE1(Bitmap eInput, out Bitmap bmpResult, bool istrain = false)
        {
            bool ret = true;

            if (!string.IsNullOrEmpty(PADINSPECTOPString))
            {
                string[] strs = PADINSPECTOPString.Split(',');
                if (strs.Length > 5)
                {
                    GetRGB = short.Parse(strs[0]);
                    numThresholdRatio = int.Parse(strs[1]);
                    numObjectFilterRatio = int.Parse(strs[2]);
                    numEDCount = int.Parse(strs[3]);
                    numShortenRatio = int.Parse(strs[4]);
                    txtBangBangRectStr = strs[5];
                }
                if (strs.Length > 8)
                {
                    numBangBangOffsetVal = int.Parse(strs[6]);
                    txtNeverOutsideRect = strs[7];
                    cboIPDMethod = int.Parse(strs[8]);
                }
            }

            if (m_IsSaveTemp)
            {
                eInput.Save("D:\\testtest\\" + _CalPageIndex() +
                    RelateAnalyzeString + "outCheckGlue_input" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }

            Bitmap ebmpinput = new Bitmap(eInput);
            bmpResult = new Bitmap(eInput);

            //抽取绿色
            ExtractChannel extract = new ExtractChannel();
            extract.Channel = 1;
            ebmpinput = extract.Apply(ebmpinput);

            //Mean mean = new Mean();
            //ebmpinput = mean.Apply(ebmpinput);
            //local
            //BradleyLocalThresholding bradleyLocalThresholding = new BradleyLocalThresholding();
            //ebmpinput = bradleyLocalThresholding.Apply(ebmpinput);


            Bitmap bmpGR = new Bitmap(ebmpinput);
            //外围加框
            Graphics graphics = Graphics.FromImage(bmpGR);
            //graphics.FillRectangles(Brushes.White, new RectangleF[] { StringtoRect(txtNeverOutsideRect) });
            graphics.DrawRectangles(new Pen(Color.White, 2), new RectangleF[] { new RectangleF(0, 0, bmpGR.Width, bmpGR.Height) });
            graphics.Dispose();

            //Grayscale grayscale = new Grayscale(0.299, 0.587, 0.114);
            bmpGR = extract.Apply(bmpGR);

            //local
            BradleyLocalThresholding bradleyLocalThresholding = new BradleyLocalThresholding();
            bmpGR = bradleyLocalThresholding.Apply(bmpGR);

            //反向
            Invert invert = new Invert();
            bmpGR = invert.Apply(bmpGR);

            Bitmap bmpGR1 = new Bitmap(bmpGR);
            //填充
            Graphics graphics1 = Graphics.FromImage(bmpGR1);
            graphics1.FillRectangles(Brushes.Black, new RectangleF[] { StringtoRect(txtNeverOutsideRect) });//no use
            graphics1.Dispose();

            if (m_IsSaveTemp)
            {
                bmpGR1.Save("D:\\testtest\\" + _CalPageIndex() +
                    RelateAnalyzeString + "outCheckGlue_org_" + (istrain ? "_train" : "_run") + ".png", System.Drawing.Imaging.ImageFormat.Png);

            }

            bmpGR1 = extract.Apply(bmpGR1);

            //找球的blob 记录起来
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.ProcessImage(bmpGR1);
            Blob[] blobs = blobCounter.GetObjectsInformation();
            List<Rectangle> rectangles = new List<Rectangle>();
            int filterArea = 150;
            Graphics graphicsXX = Graphics.FromImage(bmpResult);
            if (istrain)
            {
                if (blobs.Length > 0)
                {
                    foreach (Blob bb in blobs)
                    {
                        if (bb.Area > filterArea)
                        {
                            BallBlobClass ballBlob = new BallBlobClass();
                            ballBlob.rect = bb.Rectangle;
                            ballBlob.area = bb.Area;
                            m_PADRegion.ballBlobClasses.Add(ballBlob);

                            rectangles.Add(bb.Rectangle);
                        }
                    }
                }
            }
            else
            {

                List<Rectangle> rectanglesIN = new List<Rectangle>();
                foreach (BallBlobClass ballBlobClass in m_PADRegion.ballBlobClasses)
                {
                    rectanglesIN.Add(ballBlobClass.rect);
                }
                graphicsXX.DrawRectangles(new Pen(Color.Yellow, 3), rectanglesIN.ToArray());
                if (blobs.Length > 0)
                {
                    foreach (Blob bb in blobs)
                    {
                        if (bb.Area > filterArea)
                        {
                            //BallBlobClass ballBlob = new BallBlobClass();
                            //ballBlob.rect = bb.Rectangle;
                            //ballBlob.area = bb.Area;
                            //m_PADRegion.ballBlobClasses.Add(ballBlob);

                            rectangles.Add(bb.Rectangle);
                            int _count = 0;
                            foreach (BallBlobClass ballBlobClass in m_PADRegion.ballBlobClasses)
                            {
                                if (bb.Rectangle.IntersectsWith(ballBlobClass.rect))
                                {
                                    _count++;
                                }
                            }
                            if (_count >= 2)
                            {
                                ret = false;
                                break;
                            }
                        }
                    }
                }

                //ret = rectangles.Count == m_PADRegion.ballBlobClasses.Count;

            }
            graphicsXX.DrawRectangles(new Pen(Color.Red, 3), rectangles.ToArray());
            graphicsXX.Dispose();

            if (m_IsSaveTemp)
            {
                bmpResult.Save("D:\\testtest\\" + _CalPageIndex() +
                    RelateAnalyzeString + "outCheckGlue" + (istrain ? "_train" : "_run") + ".png", System.Drawing.Imaging.ImageFormat.Png);

            }

            bmpGR.Dispose();
            //eInput.Dispose();

            return ret;
        }
        RectangleF checkNoHaveGlue(Bitmap eInput)
        {
            //Rectangle rectangleRet = new Rectangle();

            Bitmap bmpinput = eInput.Clone(new Rectangle(0, 0, eInput.Width, eInput.Height), PixelFormat.Format24bppRgb);

            int isizeDispening = _from_bmpinputSize_to_iSized(bmpinput);
            Bitmap bmpsize = new Bitmap(bmpinput, Resize(bmpinput.Size, isizeDispening));
            Bitmap bmphistogramEqualization11 = new Bitmap(bmpsize);

            filterORG001(bmphistogramEqualization11);
            if (m_IsSaveTemp)
            {
                bmphistogramEqualization11.Save("D:\\testtest\\" + _CalPageIndex() +
                    RelateAnalyzeString + "NoDispensingOrg" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }

            bmphistogramEqualization11 = FloodFill(bmphistogramEqualization11,
                                                                new Point(bmphistogramEqualization11.Width / 2, bmphistogramEqualization11.Height / 2),
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

            RectangleF rectangletemp2 = ResizeWithLocation2(rectangletemp, -isizeDispening);

            bmphistogramEqualization11.Dispose();
            return rectangletemp2;
        }
        RectangleF checkNoHaveGlueV1(Bitmap eInput)
        {
            //Rectangle rectangleRet = new Rectangle();

            Bitmap bmpinput = eInput.Clone(new Rectangle(0, 0, eInput.Width, eInput.Height), PixelFormat.Format24bppRgb);

            int isizeDispening = _from_bmpinputSize_to_iSized(bmpinput);
            Bitmap bmpsize = new Bitmap(bmpinput, Resize(bmpinput.Size, isizeDispening));
            Bitmap bmpFilter = new Bitmap(bmpsize);

            if (m_IsSaveTemp)
            {
                bmpFilter.Save("D:\\testtest\\" + _CalPageIndex() +
                    RelateAnalyzeString + "NoDispensingOrg" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }

            ExtractChannel extract = new ExtractChannel();
            extract.Channel = 1;
            bmpFilter = extract.Apply(bmpFilter);

            Sharpen sharpen = new Sharpen();
            bmpFilter = sharpen.Apply(bmpFilter);

            DifferenceEdgeDetector differenceEdgeDetector = new DifferenceEdgeDetector();
            bmpFilter = differenceEdgeDetector.Apply(bmpFilter);

            SISThreshold sISThreshold = new SISThreshold();
            bmpFilter = sISThreshold.Apply(bmpFilter);

            ExtractBiggestBlob extractBiggestBlob = new ExtractBiggestBlob();
            bmpFilter = extractBiggestBlob.Apply(bmpFilter);

            if (m_IsSaveTemp)
            {
                bmpFilter.Save("D:\\testtest\\" + _CalPageIndex() +
                    RelateAnalyzeString + "NoDispensingFiter" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }

            Rectangle rectangletemp = new Rectangle(extractBiggestBlob.BlobPosition.X,
                                                                       extractBiggestBlob.BlobPosition.Y,
                                                                       bmpFilter.Width,
                                                                       bmpFilter.Height);

            RectangleF rectangletemp2 = ResizeWithLocation2(rectangletemp, -isizeDispening);

            bmpFilter.Dispose();
            bmpsize.Dispose();
            bmpinput.Dispose();
            return rectangletemp2;
        }
        RectangleF checkNoHaveGlueV2(Bitmap eInput)
        {
            //Rectangle rectangleRet = new Rectangle();

            if (!string.IsNullOrEmpty(PADINSPECTOPString))
            {
                string[] strs = PADINSPECTOPString.Split(',');
                if (strs.Length > 5)
                {
                    GetRGB = short.Parse(strs[0]);
                    numThresholdRatio = int.Parse(strs[1]);
                    numObjectFilterRatio = int.Parse(strs[2]);
                    numEDCount = int.Parse(strs[3]);
                    numShortenRatio = int.Parse(strs[4]);
                    txtBangBangRectStr = strs[5];
                }
                if (strs.Length > 8)
                {
                    numBangBangOffsetVal = int.Parse(strs[6]);
                    txtNeverOutsideRect = strs[7];
                    cboIPDMethod = int.Parse(strs[8]);
                }
            }

            Rectangle _crop = StringtoRect(txtNeverOutsideRect);//no use
            //Bitmap bmpinput = eInput.Clone(new Rectangle(0, 0, eInput.Width, eInput.Height), PixelFormat.Format24bppRgb);
            Bitmap bmpinput = eInput.Clone(_crop, PixelFormat.Format24bppRgb);

            int isizeDispening = _from_bmpinputSize_to_iSized(bmpinput);
            Bitmap bmpsize = new Bitmap(bmpinput, Resize(bmpinput.Size, isizeDispening));
            Bitmap bmpFilter = new Bitmap(bmpsize);

            //m_Tools.DrawRect(bmpFilter, SimpleRect(bmpFilter.Size), new Pen(Color.Black, 3));

            if (m_IsSaveTemp)
            {
                bmpFilter.Save("D:\\testtest\\" + _CalPageIndex() +
                    RelateAnalyzeString + "NoDispensingOrg" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }

            ExtractChannel extract = new ExtractChannel();
            extract.Channel = 1;
            bmpFilter = extract.Apply(bmpFilter);

            //Sharpen sharpen = new Sharpen();
            //bmpFilter = sharpen.Apply(bmpFilter);

            //CannyEdgeDetector cannyEdgeDetector = new CannyEdgeDetector();
            //bmpFilter = cannyEdgeDetector.Apply(bmpFilter);

            //DifferenceEdgeDetector differenceEdgeDetector = new DifferenceEdgeDetector();
            //bmpFilter = differenceEdgeDetector.Apply(bmpFilter);

            Blur blur = new Blur();
            bmpFilter = blur.Apply(bmpFilter);

            SISThreshold sISThreshold = new SISThreshold();
            bmpFilter = sISThreshold.Apply(bmpFilter);

            //Closing closing = new Closing();
            //bmpFilter = closing.Apply(bmpFilter);

            Dilatation3x3 dilatation3X3 = new Dilatation3x3();
            Erosion3x3 erosion3X3 = new Erosion3x3();

            //bmpFilter = dilatation3X3.Apply(bmpFilter);
            //bmpFilter = dilatation3X3.Apply(bmpFilter);
            //bmpFilter = dilatation3X3.Apply(bmpFilter);
            //bmpFilter = erosion3X3.Apply(bmpFilter);
            //bmpFilter = erosion3X3.Apply(bmpFilter);
            //bmpFilter = erosion3X3.Apply(bmpFilter);

            int i = 0;
            while (i < 1)
            {
                bmpFilter = erosion3X3.Apply(bmpFilter);
                bmpFilter = dilatation3X3.Apply(bmpFilter);

                i++;
            }

            //找球的blob 记录起来
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.ProcessImage(bmpFilter);
            Blob[] blobs = blobCounter.GetObjectsInformation();

            //ExtractBiggestBlob extractBiggestBlob = new ExtractBiggestBlob();
            //bmpFilter = extractBiggestBlob.Apply(bmpFilter);

            //List<Rectangle> rectangles = new List<Rectangle>();

            Rectangle rectangle = new Rectangle();
            foreach (Blob b in blobs)
            {
                if (b.Area < 30)
                    continue;
                //if (b.Rectangle.Width > m_PADRegion.RegionWidth * 1 / 3 || b.Rectangle.Height > m_PADRegion.RegionHeight * 1 / 3)
                {
                    //if (m_PADRegion.ConnerRectF.IntersectsWith(b.Rectangle))
                        rectangle = MergeTwoRects(rectangle, b.Rectangle);
                }
            }


            if (m_IsSaveTemp)
            {
                bmpFilter.Save("D:\\testtest\\" + _CalPageIndex() +
                    RelateAnalyzeString + "NoDispensingFiter" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }

            Rectangle rectangletemp = new Rectangle(rectangle.X,
                                                                       rectangle.Y,
                                                                       rectangle.Width,
                                                                       rectangle.Height);

            RectangleF rectangletemp2 = ResizeWithLocation2(rectangletemp, -isizeDispening);
            rectangletemp2.X += _crop.X;
            rectangletemp2.Y += _crop.Y;

            Bitmap bb = new Bitmap(eInput);
            //Graphics graphics = Graphics.FromImage(bmpFilter);

            m_Tools.DrawRect(bb, rectangletemp2, new Pen(Color.Red, 3));
            if (m_IsSaveTemp)
            {
                bb.Save("D:\\testtest\\" + _CalPageIndex() +
                    RelateAnalyzeString + "NoDispensingFiter_bb" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }

            bmpFilter.Dispose();
            bmpsize.Dispose();
            bmpinput.Dispose();
            bb.Dispose();
            return rectangletemp2;
        }
        RectangleF checkNoHaveGlueV3(Bitmap eInput)
        {
            Bitmap bmpinput = new Bitmap(eInput);// eInput.Clone(new Rectangle(0, 0, eInput.Width, eInput.Height), PixelFormat.Format24bppRgb);
            
            int isizeDispening = _from_bmpinputSize_to_iSized(bmpinput);
            Bitmap bmpsize = new Bitmap(bmpinput, Resize(bmpinput.Size, isizeDispening));
            Bitmap bmpFilter = new Bitmap(bmpsize);

            //m_Tools.DrawRect(bmpFilter, SimpleRect(bmpFilter.Size), new Pen(Color.Black, 3));

            if (m_IsSaveTemp)
            {
                bmpFilter.Save("D:\\testtest\\" + _CalPageIndex() +
                    RelateAnalyzeString + "NoDispensingOrg" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }

            ExtractChannel extract = new ExtractChannel();
            extract.Channel = 1;
            bmpFilter = extract.Apply(bmpFilter);

            Blur blur = new Blur();
            bmpFilter = blur.Apply(bmpFilter);

            SISThreshold sISThreshold = new SISThreshold();
            bmpFilter = sISThreshold.Apply(bmpFilter);

            Dilatation3x3 dilatation3X3 = new Dilatation3x3();
            Erosion3x3 erosion3X3 = new Erosion3x3();

            int i = 0;
            while (i < 1)
            {
                bmpFilter = erosion3X3.Apply(bmpFilter);
                bmpFilter = dilatation3X3.Apply(bmpFilter);

                i++;
            }

            //找球的blob 记录起来
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.ProcessImage(bmpFilter);
            Blob[] blobs = blobCounter.GetObjectsInformation();

            //ExtractBiggestBlob extractBiggestBlob = new ExtractBiggestBlob();
            //bmpFilter = extractBiggestBlob.Apply(bmpFilter);

            //List<Rectangle> rectangles = new List<Rectangle>();

            Rectangle rectangle = new Rectangle();
            foreach (Blob b in blobs)
            {
                if (b.Area < 30)
                    continue;
                if (b.Rectangle.Width > m_PADRegion.RegionWidth * 1 / 3 || b.Rectangle.Height > m_PADRegion.RegionHeight * 1 / 3)
                {
                    if (m_PADRegion.ConnerRectF.IntersectsWith(b.Rectangle))
                        rectangle = MergeTwoRects(rectangle, b.Rectangle);
                }
            }

            if (m_IsSaveTemp)
            {
                bmpFilter.Save("D:\\testtest\\" + _CalPageIndex() +
                    RelateAnalyzeString + "NoDispensingFiter" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }

            Rectangle rectangletemp = new Rectangle(rectangle.X,
                                                                       rectangle.Y,
                                                                       rectangle.Width,
                                                                       rectangle.Height);

            RectangleF rectangletemp2 = ResizeWithLocation2(rectangletemp, -isizeDispening);


            Bitmap bb = new Bitmap(eInput);
            //Graphics graphics = Graphics.FromImage(bmpFilter);

            m_Tools.DrawRect(bb, rectangletemp2, new Pen(Color.Red, 3));
            if (m_IsSaveTemp)
            {
                bb.Save("D:\\testtest\\" + _CalPageIndex() +
                    RelateAnalyzeString + "NoDispensingFiter_bb" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }

            bmpFilter.Dispose();
            bmpsize.Dispose();
            bmpinput.Dispose();
            bb.Dispose();
            return rectangletemp2;
        }
        RectangleF checkNoHaveGlueV4(Bitmap eInput)
        {
            Bitmap bmpinput = new Bitmap(eInput);// eInput.Clone(new Rectangle(0, 0, eInput.Width, eInput.Height), PixelFormat.Format24bppRgb);

            int isizeDispening = _from_bmpinputSize_to_iSized(bmpinput);
            Bitmap bmpsize = new Bitmap(bmpinput, Resize(bmpinput.Size, isizeDispening));
            Bitmap bmpFilter = new Bitmap(bmpsize);

            //m_Tools.DrawRect(bmpFilter, SimpleRect(bmpFilter.Size), new Pen(Color.Black, 3));

            if (m_IsSaveTemp)
            {
                bmpFilter.Save("D:\\testtest\\" + _CalPageIndex() +
                    RelateAnalyzeString + "NoDispensingOrg" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }

            ImageStatistics stat = new ImageStatistics(bmpFilter);
            AForge.Math.Histogram activeHistogram = stat.Green;

            Bitmap bmpMean = m_PADRegion.GetFillMean(bmpFilter, (int)activeHistogram.Mean);

            ExtractChannel extract = new ExtractChannel();
            extract.Channel = 1;
            bmpFilter = extract.Apply(bmpMean);

            SISThreshold sISThreshold = new SISThreshold();
            bmpFilter = sISThreshold.Apply(bmpFilter);

            bmpFilter = FloodFill(bmpFilter,
                new Point(bmpFilter.Width / 2, bmpFilter.Height / 2),
                Color.White, 200);


            //找球的blob 记录起来
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.ProcessImage(bmpFilter);
            Blob[] blobs = blobCounter.GetObjectsInformation();

            Rectangle rectangle = new Rectangle();
            foreach (Blob b in blobs)
            {
                if (b.Area < 30)
                    continue;
                if (b.Rectangle.Width > m_PADRegion.RegionWidth * 1 / 3 || b.Rectangle.Height > m_PADRegion.RegionHeight * 1 / 3)
                {
                    if (m_PADRegion.ConnerRectF.IntersectsWith(b.Rectangle))
                        rectangle = MergeTwoRects(rectangle, b.Rectangle);
                }
            }

            if (m_IsSaveTemp)
            {
                bmpFilter.Save("D:\\testtest\\" + _CalPageIndex() +
                    RelateAnalyzeString + "NoDispensingFiter" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }

            Rectangle rectangletemp = new Rectangle(rectangle.X,
                                                                       rectangle.Y,
                                                                       rectangle.Width,
                                                                       rectangle.Height);

            RectangleF rectangletemp2 = ResizeWithLocation2(rectangletemp, -isizeDispening);


            Bitmap bb = new Bitmap(eInput);
            //Graphics graphics = Graphics.FromImage(bmpFilter);

            m_Tools.DrawRect(bb, rectangletemp2, new Pen(Color.Red, 3));
            if (m_IsSaveTemp)
            {
                bb.Save("D:\\testtest\\" + _CalPageIndex() +
                    RelateAnalyzeString + "NoDispensingFiter_bb" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }

            bmpFilter.Dispose();
            bmpsize.Dispose();
            bmpinput.Dispose();
            bb.Dispose();
            return rectangletemp2;
        }
        public void GetHistogramData(int[] histogramdata, JzHistogramResult jzhresult)
        {
            int num = histogramdata.Length;
            int max = 0;
            int min = num;
            long total = 0L;
            for (int i = 0; i < num; i++)
            {
                if (histogramdata[i] != 0)
                {
                    if (i > max)
                    {
                        max = i;
                    }

                    if (i < min)
                    {
                        min = i;
                    }

                    total += histogramdata[i];
                }
            }

            jzhresult.min = min;
            jzhresult.max = max;
            jzhresult.count = num;
            jzhresult.total = total;

            jzhresult.mean = Statistics.Mean(histogramdata);
            jzhresult.stddev = Statistics.StdDev(histogramdata, jzhresult.mean);
            jzhresult.median = Statistics.Median(histogramdata);
            jzhresult.mode = Statistics.Mode(histogramdata);

        }

        Rectangle MergeTwoRects(Rectangle rect1, Rectangle rect2)
        {
            Rectangle rect = new Rectangle();

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

        public void GetSideMaxMinValue(BorderTypeEnum eborder, out double eMax, out double eMin)
        {
            eMax = 0;// double.MinValue;
            eMin = 0;// double.MinValue;
            //int i = 0;
            //while (i < (int)BorderTypeEnum.COUNT)
            {
                switch (eborder)
                {
                    case BorderTypeEnum.LEFT:
                        eMax = GlueMaxLeft;
                        eMin = GlueMinLeft;
                        break;
                    case BorderTypeEnum.TOP:
                        eMax = GlueMaxTop;
                        eMin = GlueMinTop;
                        break;
                    case BorderTypeEnum.RIGHT:
                        eMax = GlueMaxRight;
                        eMin = GlueMinRight;
                        break;
                    case BorderTypeEnum.BOTTOM:
                        eMax = GlueMaxBottom;
                        eMin = GlueMinBottom;
                        break;
                }
            }

            //switch (eborder)
            //{
            //    case BorderTypeEnum.LEFT:
            //        eMax = GlueMaxLeft / INI.MAINSD_PAD_MIL_RESOLUTION;
            //        eMin = GlueMinLeft / INI.MAINSD_PAD_MIL_RESOLUTION;
            //        break;
            //    case BorderTypeEnum.TOP:
            //        eMax = GlueMaxTop / INI.MAINSD_PAD_MIL_RESOLUTION;
            //        eMin = GlueMinTop / INI.MAINSD_PAD_MIL_RESOLUTION;
            //        break;
            //    case BorderTypeEnum.RIGHT:
            //        eMax = GlueMaxRight / INI.MAINSD_PAD_MIL_RESOLUTION;
            //        eMin = GlueMinRight / INI.MAINSD_PAD_MIL_RESOLUTION;
            //        break;
            //    case BorderTypeEnum.BOTTOM:
            //        eMax = GlueMaxBottom / INI.MAINSD_PAD_MIL_RESOLUTION;
            //        eMin = GlueMinBottom / INI.MAINSD_PAD_MIL_RESOLUTION;
            //        break;
            //}
        }
        public void JzFourSideCal(out GlueRegionClass glue, SideDataClass sidein, SideDataClass sideout)
        {
            glue = new GlueRegionClass();
            glue.Reset();

            //foreach (Point point in sidein.GetLinePoints())
            //{
            //    glue.AddPtIN(point);
            //}
            //foreach (Point point in sideout.GetLinePoints())
            //{
            //    glue.AddPt(point);
            //}

            glue.Run();
        }
        public void JzFourSideCalV1(out GlueRegionClass glue, JzSideAnalyzeEXClass eSide, SIDEEmnum sIDE, bool isCheckSlot = false)
        {
            glue = new GlueRegionClass();
            glue.Reset();

            foreach (Point point in eSide.sidedataexs[(int)sIDE].GetPoints())
            {
                glue.AddPtIN(point);
            }
            foreach (Point point in eSide.sidedataexs[(int)sIDE].GetPoints(1))
            {
                glue.AddPt(point);
            }
            glue.Run();
            if (isCheckSlot)
            {
                foreach (Point point in eSide.sidedataexs[(int)sIDE].GetPoints(3))
                {
                    glue.AddPtIN2(point);//记录薄膜胶的点位
                }
                glue.RunSlot();
            }

        }

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
            //Bitmap bmp = new Bitmap(ebmpInput);
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

            //JetGrayImg grayimage = new JetGrayImg(bmp);
            switch (PADMethod)
            {
                case PADMethodEnum.GLUECHECK:
                    if (!ChipFindWhite)
                    {
                        AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
                        Bitmap bitmap1x = grayscale.Apply(bmp);

                        AForge.Imaging.Filters.Threshold thresholdx = new AForge.Imaging.Filters.Threshold(irange);
                        Bitmap bitmap2x = thresholdx.Apply(bitmap1x);

                        bmp.Dispose();
                        bmp = new Bitmap(bitmap2x);
                        //Graphics gx = Graphics.FromImage(bmp);
                        //gx.DrawRectangle(new Pen(Color.White, 100), new Rectangle(0, 0, bmp.Width, bmp.Height));
                        //gx.Dispose();


                        bitmap1x.Dispose();
                        bitmap2x.Dispose();
                        //grayimage = new JetGrayImg(bbb);
                        //bmp.Save("D:\\LOA\\bmp.bmp");
                    }
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
            //grayimage.ToBitmap().Save("D:\\LOA\\threshold.bmp");

            int iMAX = -10000000;
            int iMAXIndex = 0;

            JetBlob jetBlob = new JetBlob();
            jetBlob.Labeling(grayimage, JConnexity.Connexity4, (ChipFindWhite ? JBlobLayer.WhiteLayer : JBlobLayer.BlackLayer));
            int icount = jetBlob.BlobCount;

            //找到最大面积
            for (int i = 0; i < icount; i++)
            {
                int iArea = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.Area);
                if (iArea > 1000)
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
                int linewidth = 5;
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
                OutPADRegion.ConnerRectF = myRectF;
                OutPADRegion.ConnerAngle = jetrect.fAngle;

                //原来的外框
                //转换矩形的四个角
                PointF[] _myPointFs = RectFToPointF(myRectF, -jetrect.fAngle);

                Pen p = new Pen(Color.Violet, linewidth);
                p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                Pen pBottom = new Pen(Color.Violet, linewidth);
                pBottom.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                g.DrawLine(p, _myPointFs[0], _myPointFs[1]);
                g.DrawLine(p, _myPointFs[1], _myPointFs[2]);
                g.DrawLine(p, _myPointFs[2], _myPointFs[3]);
                g.DrawLine(pBottom, _myPointFs[0], _myPointFs[3]);

                OutPADRegion.SetPointFORG(_myPointFs);

                // mil 計算

                myRectF.Inflate(-(float)(0.5 * Resolution_Mil), -(float)(0.5 * Resolution_Mil));//强制内缩一个MIL
                myRectF.Inflate(-(float)(ExtendX * Resolution_Mil), -(float)(ExtendY * Resolution_Mil));

                // pixel 計算

                //myRectF.Inflate(-ExtendX, -ExtendY);

                //转换矩形的四个角
                _myPointFs = RectFToPointF(myRectF, -jetrect.fAngle);

                p = new Pen(Color.Lime, linewidth);
                p.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

                pBottom = new Pen(Color.Lime, linewidth);
                pBottom.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

                g.DrawLine(p, _myPointFs[0], _myPointFs[1]);
                g.DrawLine(p, _myPointFs[1], _myPointFs[2]);
                g.DrawLine(p, _myPointFs[2], _myPointFs[3]);
                g.DrawLine(pBottom, _myPointFs[0], _myPointFs[3]);

                OutPADRegion.RegionWidth = Math.Max(iWidth, iHeight);
                OutPADRegion.RegionHeight = Math.Min(iWidth, iHeight);
                OutPADRegion.RegionArea = iMAX;
                OutPADRegion.SetPointF(_myPointFs);

                //银胶
                switch (PADMethod)
                {
                    case PADMethodEnum.QLE_CHECK:

                        #region QLE

                        RectangleF myRectFQle = SimpleRectF(ptCenter, (float)(CalExtendX * Resolution_Mil), (float)(CalExtendY * Resolution_Mil));
                        //转换矩形的四个角
                        _myPointFs = RectFToPointF(myRectFQle, -jetrect.fAngle);

                        p = new Pen(Color.Blue, linewidth);
                        p.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

                        pBottom = new Pen(Color.Blue, linewidth);
                        pBottom.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

                        g.DrawLine(p, _myPointFs[0], _myPointFs[1]);
                        g.DrawLine(p, _myPointFs[1], _myPointFs[2]);
                        g.DrawLine(p, _myPointFs[2], _myPointFs[3]);
                        g.DrawLine(pBottom, _myPointFs[0], _myPointFs[3]);

                        OutPADRegion.SetPointFQLE(_myPointFs);
                        //
                        JzFindObjectClass jzFindObject = new JzFindObjectClass();

                        if (!ChipDirlevel)
                            myRectFQle = SimpleRectF(ptCenter, (float)(CalExtendY * Resolution_Mil), (float)(CalExtendX * Resolution_Mil));

                        Rectangle croprecttt = Rectangle.Round(myRectFQle);
                        BoundRect(ref croprecttt, ebmpInput.Size);
                        Bitmap bmpQLE = (Bitmap)ebmpInput.Clone(croprecttt, PixelFormat.Format24bppRgb);// new Bitmap(ebmpInput);
                        jzFindObject.AH_SetThreshold(ref bmpQLE, PADChipInBlobGrayThreshold);
                        jzFindObject.AH_FindBlob(bmpQLE, GLEFindWhite);
                        Rectangle maxrecttemp = jzFindObject.rectMaxRect;

                        OutPADRegion.RegionWidth = Math.Max(maxrecttemp.Width, maxrecttemp.Height);
                        OutPADRegion.RegionHeight = Math.Min(maxrecttemp.Width, maxrecttemp.Height);
                        OutPADRegion.RegionArea = jzFindObject.GetMaxArea();

                        OutPADRegion.RegionWidthReal = Math.Max(maxrecttemp.Width, maxrecttemp.Height) * INI.MAINSD_PAD_MIL_RESOLUTION;
                        OutPADRegion.RegionHeightReal = Math.Min(maxrecttemp.Width, maxrecttemp.Height) * INI.MAINSD_PAD_MIL_RESOLUTION;
                        OutPADRegion.RegionAreaReal = jzFindObject.GetMaxArea() * INI.MAINSD_PAD_MIL_RESOLUTION * INI.MAINSD_PAD_MIL_RESOLUTION;

                        maxrecttemp.X += (int)croprecttt.X;
                        maxrecttemp.Y += (int)croprecttt.Y;

                        string showmsg = $"长度{OutPADRegion.RegionWidthReal.ToString("0.00")}mm,宽度{OutPADRegion.RegionHeightReal.ToString("0.00")}mm,面积{OutPADRegion.RegionAreaReal.ToString("0.00")}mm";
                        //g.DrawString(showmsg, new Font("宋体", FontSize), Brushes.Lime, new PointF(5, 5));
                        //g.DrawRectangle(new Pen(Color.Red, linewidth), maxrecttemp);

                        //bmpQLE.Save("D:\\QLE.png", ImageFormat.Png);

                        OutPADRegion.QleMaxRect = new Rectangle(maxrecttemp.X, maxrecttemp.Y, maxrecttemp.Width, maxrecttemp.Height);

                        g.DrawImage(bmpQLE, croprecttt.Location);
                        g.DrawRectangle(new Pen(Color.Lime, 3), maxrecttemp);
                        g.DrawString(showmsg, new Font("宋体", 20), Brushes.Lime, new PointF(maxrecttemp.X, maxrecttemp.Y - 25));

                        if (m_IsSaveTemp && !eIsTrain)
                        {
                            bmpQLE.Save("D:\\testtest\\" + RelateAnalyzeString + "GLEFind1X" + (eIsTrain ? "_Train" : "_Run") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                            //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);

                            bmp.Save("D:\\testtest\\" + RelateAnalyzeString + "GlePadFind" + (eIsTrain ? "_Train" : "_Run") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                        }

                        bmpQLE.Dispose();

                        #endregion

                        break;
                }

                bmpPadFindOutput = (Bitmap)bmp.Clone();// new Bitmap(bmp);

                g.Dispose();
            }
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

                //RectangleF rect_max_meges = new RectangleF();

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
                    //rect_max_meges = MergeTwoRects(rect_max_meges, myRectF);

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

                Rectangle recttt = JzToolsClass.CvBoundingRect(pointFs);

                int d1 = (int)(BlackCalExtendX * Resolution_Mil);
                int d2 = (int)(BlackCalExtendY * Resolution_Mil);
                recttt.Inflate(d1, d2);

                int offsetx = (int)(BlackOffsetX * Resolution_Mil);
                int offsety = (int)(BlackOffsetY * Resolution_Mil);
                recttt.X += offsetx;
                recttt.Y += offsety;

                double _angle = JzToolsClass.CvminareaRectPointFsAngle(pointFs);// * 180 / Math.PI;
                if (ChipDirlevel)
                {
                    if (_angle < -15 || _angle > 15)
                        _angle = 0;
                }
                PointF[] pointFs2 = RectFToPointF(recttt, -_angle);
                //PointF[] pointFs2 = JzToolsClass.CvminareaRectPointFs(pointFs);


                OutPADRegion.RegionForEdgeRect = new Rectangle(recttt.X, recttt.Y, recttt.Width, recttt.Height);

                OutPADRegion.ConnerRectF = new Rectangle(recttt.X, recttt.Y, recttt.Width, recttt.Height);
                OutPADRegion.ConnerAngle = -_angle;

                //PointF[] pointFs1 = new PointF[4];
                //pointFs1[0] = new PointF(OutPADRegion.RegionForEdgeRect.X, OutPADRegion.RegionForEdgeRect.Y);
                //pointFs1[1] = new PointF(OutPADRegion.RegionForEdgeRect.X + OutPADRegion.RegionForEdgeRect.Width, OutPADRegion.RegionForEdgeRect.Y);
                //pointFs1[2] = new PointF(OutPADRegion.RegionForEdgeRect.X + OutPADRegion.RegionForEdgeRect.Width, OutPADRegion.RegionForEdgeRect.Y + OutPADRegion.RegionForEdgeRect.Height);
                //pointFs1[3] = new PointF(OutPADRegion.RegionForEdgeRect.X, OutPADRegion.RegionForEdgeRect.Y + OutPADRegion.RegionForEdgeRect.Height);

                //new Rectangle((int)rect_max_meges.X, (int)rect_max_meges.Y, (int)(rect_max_meges.Width), (int)(rect_max_meges.Height));

                ////if(eIsTrain)
                //{
                Pen p1 = new Pen(Color.Green, penwidth);
                p1.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                //    Pen pBottom1 = new Pen(Color.Green, penwidth);
                //    pBottom1.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                //    g.DrawLine(p1, _myPointFs1[0], _myPointFs1[1]);
                //    g.DrawLine(p1, _myPointFs1[1], _myPointFs1[2]);
                //    g.DrawLine(p1, _myPointFs1[2], _myPointFs1[3]);
                //    g.DrawLine(pBottom1, _myPointFs1[0], _myPointFs1[3]);
                //}

                g.DrawRectangle(p1, OutPADRegion.RegionForEdgeRect);
                g.Dispose();

                Bitmap bmpdrawUse2 = (Bitmap)ebmpInput.Clone();
                Graphics g2 = Graphics.FromImage(bmpdrawUse2);
                g2.Clear(Color.Black);
                g2.FillPolygon(Brushes.White, pointFs2);
                g2.Dispose();

                bmpPadFindOutput.Dispose();
                bmpPadFindOutput = (Bitmap)bmpdrawUse.Clone();// new Bitmap(bmp);
                OutPADRegion.bmpChipForWetherGlue.Dispose();
                OutPADRegion.bmpChipForWetherGlue = new Bitmap(bmpdrawUse);
                bmpdrawUse.Dispose();

                OutPADRegion.bmpThreshold.Dispose();
                OutPADRegion.bmpThreshold = new Bitmap(bmpdrawUse2);
                bmpdrawUse2.Dispose();

                //OutPADRegion.SetPointFORG(pointFs1, jRotatedRectangleF.fAngle);
                OutPADRegion.SetPointFORG(pointFs2, -_angle);
            }

            return 0;
        }
        private int PADRegionFind_BlackEdge_bak(Bitmap ebmpInput, int eThresholdValue, bool eIsTrain, out PADRegionClass OutPADRegion)
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
                OutPADRegion.Isleftoverright = !ChipDirlevel;// Math.Abs(_angle) < 45;
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

        /// <summary>
        /// 用来检测碰到锡球的算法
        /// </summary>
        /// <param name="ebmpInput"></param>
        /// <param name="eThresholdValue"></param>
        /// <param name="eInputPADRegion"></param>
        /// <param name="ebmpOutput"></param>
        /// <param name="iSized"></param>
        /// <returns></returns>
        private int PADRegionCheckSize(Bitmap ebmpInput,
           int eThresholdValue,
           PADRegionClass eInputPADRegion,
           ref Bitmap ebmpOutput,
           int iSized = 3)
        {
            int sizeblob = iSized;
            //double res_mil = INI.MAINSD_PAD_MIL_RESOLUTION / 0.0254;
            int ret = 0;

            Bitmap bmpdraw = (Bitmap)ebmpInput.Clone();
            imginput = new Bitmap(ebmpInput);

            if (m_IsSaveTemp)
            {
                imginput.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + $"_blob03sizeorg" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }

            Bitmap bmp = (Bitmap)imginput.Clone();
            AForge.Imaging.Filters.YCbCrExtractChannel yCbCrExtractChannel = new AForge.Imaging.Filters.YCbCrExtractChannel(2);
            bmp = yCbCrExtractChannel.Apply(bmp);
            AForge.Imaging.Filters.ContrastStretch contrastStretch = new AForge.Imaging.Filters.ContrastStretch();
            bmp = contrastStretch.Apply(bmp);
            AForge.Imaging.Filters.HistogramEqualization histogramEqualization = new AForge.Imaging.Filters.HistogramEqualization();
            bmp = histogramEqualization.Apply(bmp);
            //AForge.Imaging.Filters.Grayscale grayscale1 = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            //bmp = grayscale1.Apply(bmp);
            AForge.Imaging.Filters.SISThreshold sISThreshold = new AForge.Imaging.Filters.SISThreshold();
            bmp = sISThreshold.Apply(bmp);
            AForge.Imaging.Filters.Closing closing = new AForge.Imaging.Filters.Closing();
            bmp = closing.Apply(bmp);
            AForge.Imaging.Filters.ExtractBiggestBlob extractBiggestBlob11 =
                      new AForge.Imaging.Filters.ExtractBiggestBlob();
            bmp = extractBiggestBlob11.Apply(bmp);

            Rectangle rectangletemp = new Rectangle(extractBiggestBlob11.BlobPosition.X,
                                                                           extractBiggestBlob11.BlobPosition.Y,
                                                                           bmp.Width,
                                                                           bmp.Height);
            //算交集的rect
            Rectangle rectangletemp1 = new Rectangle(extractBiggestBlob11.BlobPosition.X,
                                                                       extractBiggestBlob11.BlobPosition.Y,
                                                                       bmp.Width,
                                                                       bmp.Height);

            Bitmap bmp1 = extractBiggestBlob11.Apply(eInputPADRegion.bmpThreshold);
            Rectangle chiprect = new Rectangle(extractBiggestBlob11.BlobPosition.X,
                                                                           extractBiggestBlob11.BlobPosition.Y,
                                                                           bmp1.Width,
                                                                           bmp1.Height);

            int intersectindex = 0;
            List<Rectangle> blobrects2 = new List<Rectangle>();

            //先判断chip与找到blob重合 再判断与周围锡球个数超过 3个则算NG
            rectangletemp1.Intersect(chiprect);
            if (rectangletemp1.Width * rectangletemp1.Height >= chiprect.Width * chiprect.Height)
            {
                //芯片周围的锡球
                AForge.Imaging.BlobCounter blobCounter = new AForge.Imaging.BlobCounter(eInputPADRegion.bmpThreshold);
                Rectangle[] blobrects = blobCounter.GetObjectsRectangles();

                foreach (Rectangle rectangle in blobrects)
                {
                    if (rectangletemp.IntersectsWith(rectangle))
                    {
                        blobrects2.Add(rectangle);
                        intersectindex++;
                    }
                }
            }

            if (intersectindex >= 3)
            {
                ret = -1;
            }

            Graphics g = Graphics.FromImage(bmpdraw);
            RectangleF[] fs = new RectangleF[1];
            fs[0] = rectangletemp;
            g.DrawRectangles(new Pen(Color.Red, 3), fs);
            if (blobrects2.Count > 0)
            {
                g.DrawRectangles(new Pen(Color.Red, 3), blobrects2.ToArray());
            }
            g.Dispose();

            if (m_IsSaveTemp)
            {
                bmp.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + $"_blob03size{intersectindex.ToString()}" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                bmpdraw.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + $"_blob03size1" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }

            ebmpOutput = (Bitmap)bmpdraw.Clone();

            imginput.Dispose();
            bmp.Dispose();
            bmpdraw.Dispose();
            bmp1.Dispose();

            return ret;
        }
        private int PADRegionFindBlob(Bitmap ebmpInput,
            int eThresholdValue,
            PADRegionClass eInputPADRegion,
            ref Bitmap ebmpOutput,
            int iSized = 3)
        {
            //首先MASK 区域出来

            m_BadArea = 0;
            m_BadCount = 0;
            m_BadWidth = 0;
            m_BadHeight = 0;

            int sizeblob = iSized;
            //换算mil值
            double res_mil = INI.MAINSD_PAD_MIL_RESOLUTION / 0.0254;
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
            //g.DrawString(msg, new Font("宋体", 18), Brushes.Black, 5, 5);

            if (blobs.Length > 0)
            {
                bNG = false;
                RectangleF rectangleF = ResizeWithLocation2(blobs[0].Rectangle, sizeblob);
                m_BadArea = blobs[0].Area * (1 << sizeblob) * (1 << sizeblob) * res_mil * res_mil;
                m_BadWidth = rectangleF.Width * res_mil;
                m_BadHeight = rectangleF.Height * res_mil;

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

                List<RectangleF> list = new List<RectangleF>();
                int errorindex = 0;
                foreach (AForge.Imaging.Blob blobx in blobs)
                {
                    rectangleF = ResizeWithLocation2(blobx.Rectangle, sizeblob);
                    double _areax = blobx.Area * (1 << sizeblob) * (1 << sizeblob) * res_mil * res_mil;
                    double _widthx = rectangleF.Width * res_mil;
                    double _heightx = rectangleF.Height * res_mil;
                    bool _bOK = true;
                    //如果最大的方框都NG了 则是NG
                    if (_widthx > CheckDWidth)
                    {
                        _bOK = false;
                    }
                    else if (_heightx > CheckDHeight)
                    {
                        _bOK = false;
                    }
                    else if (_areax > CheckDArea)
                    {
                        _bOK = false;
                    }
                    if (!_bOK)
                    {
                        list.Add(rectangleF);
                        if (errorindex <= 5)
                        {
                            g.DrawString($"w:{_widthx.ToString("0.00")},h:{_heightx.ToString("0.00")},a:{_areax.ToString("0.00")}",
                                new Font("宋体", 10),
                                Brushes.Red,
                                new PointF(rectangleF.Location.X, rectangleF.Location.Y - 15));
                        }
                        errorindex++;
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
            int iSized = 3, bool eWhite = false, List<RectangleF> rectangleFs = null)
        {
            //首先MASK 区域出来

            m_BadArea = 0;
            m_BadCount = 0;
            m_BadWidth = 0;
            m_BadHeight = 0;

            int sizeblob = iSized;
            //换算mil值
            double res_mil = INI.MAINSD_PAD_MIL_RESOLUTION / 0.0254;

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
            //g.DrawString(msg, new Font("宋体", 22), Brushes.Lime, 5, 5);

            if (blobs.Length > 0)
            {
                bNG = false;
                RectangleF rectangleF = ResizeWithLocation3(blobs[0].Rectangle, sizeblob);
                m_BadArea = blobs[0].Area * sizeblob * sizeblob * res_mil * res_mil;
                m_BadWidth = rectangleF.Width * res_mil;
                m_BadHeight = rectangleF.Height * res_mil;

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
                    double _areax = blobs[0].Area * sizeblob * sizeblob * res_mil * res_mil;
                    double _widthx = rectangleF.Width * res_mil;
                    double _heightx = rectangleF.Height * res_mil;
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
        private int PADRegionFindBlob_QLE(Bitmap ebmpInput,
            int eThresholdValue,
            PADRegionClass eInputPADRegion,
            ref Bitmap ebmpOutput,
            int iSized = 3, bool eWhite = false, List<RectangleF> rectangleFs = null)
        {
            //首先MASK 区域出来

            m_BadArea = 0;
            m_BadCount = 0;
            m_BadWidth = 0;
            m_BadHeight = 0;

            int sizeblob = iSized;
            //换算mil值
            double res_mil = INI.MAINSD_PAD_MIL_RESOLUTION / 0.0254;

            Bitmap bmpdraw = (Bitmap)ebmpInput.Clone();// new Bitmap(ebmpInput);
                                                       //ebmpInput.Save("D:\\QLE.png", ImageFormat.Png);
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
                gPrepared.FillPolygon(Brushes.White, eInputPADRegion.RegionPtFCornerQLE);
                //if (eWhite)
                //{
                //    gPrepared.FillPolygon(Brushes.White, eInputPADRegion.RegionPtFCorner);
                //    gPrepared.FillPolygon(Brushes.Black, eInputPADRegion.RegionPtFCornerQLE);
                //}
                //else
                //{
                //    gPrepared.FillPolygon(Brushes.Black, eInputPADRegion.RegionPtFCorner);
                //    gPrepared.FillPolygon(Brushes.White, eInputPADRegion.RegionPtFCornerQLE);
                //}

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
            //imgmask.Save("D:\\QLE1.png", ImageFormat.Png);


            if (eWhite)
            {
                m_JzFind.GetMaskedImage(imginput, imgmask, Color.White, Color.Black, true);
            }
            else
            {
                m_JzFind.GetMaskedImage(imginput, imgmask, Color.White, Color.White, true);
            }

            //m_JzFind.GetMaskedImage(imginput, imgmask, Color.White, Color.White, true);
            //imginput.Save("D:\\QLE.png", ImageFormat.Png);
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
            //bmp.Save("D:\\QLE2.png", ImageFormat.Png);
            bool bNG = false;
            Graphics g = Graphics.FromImage(bmpdraw);
            //string msg = "标准长度:" + CheckDWidth.ToString() + ",标准宽度:" + CheckDHeight.ToString() + ",标准面积:" + CheckDArea.ToString() + Environment.NewLine;
            //g.DrawString(msg, new Font("宋体", 22), Brushes.Lime, 5, 5);

            //string showmsg = $"长度{eInputPADRegion.RegionWidthReal}mm,宽度{eInputPADRegion.RegionHeightReal}mm,面积{eInputPADRegion.RegionAreaReal}mm";
            //g.DrawString(showmsg, new Font("宋体", FontSize), Brushes.Lime, 5, 15);

            //g.DrawPolygon(new Pen(Color.Blue, LineWidth), eInputPADRegion.RegionPtFCornerQLE);
            //g.DrawRectangle(new Pen(Color.Lime, LineWidth), eInputPADRegion.QleMaxRect);

            if (blobs.Length > 0)
            {
                bNG = false;
                RectangleF rectangleF = ResizeWithLocation3(blobs[0].Rectangle, sizeblob);
                m_BadArea = blobs[0].Area * sizeblob * sizeblob * res_mil * res_mil;
                m_BadWidth = rectangleF.Width * res_mil;
                m_BadHeight = rectangleF.Height * res_mil;

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
                int errorindex = 0;
                foreach (AForge.Imaging.Blob blobx in blobs)
                {
                    rectangleF = ResizeWithLocation3(blobx.Rectangle, sizeblob);
                    double _areax = blobx.Area * sizeblob * sizeblob * res_mil * res_mil;
                    double _widthx = rectangleF.Width * res_mil;
                    double _heightx = rectangleF.Height * res_mil;
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
                        if (errorindex <= 5)
                        {
                            g.DrawString($"w:{m_BadWidth.ToString("0.00")},h:{m_BadHeight.ToString("0.00")},a:{m_BadArea.ToString("0.00")}",
                                new Font("宋体", 10),
                                Brushes.Red,
                                new Point(blobx.Rectangle.Location.X, blobx.Rectangle.Location.Y - 15));
                        }
                        errorindex++;
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

        private PointF[] _getEdgeConcor(double angle, PointF ptfleft, PointF ptfright, PointF ptftop, PointF ptfbottom, double centeroffsetx = 0, double centeroffsety = 0, bool leftoverright = false)
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
            PointF pointF1tmp = new PointF();
            double[] m_distance = new double[10];
            int m_samplinggap = 137;
            m_samplinggap = 37;

            int _minsize = Math.Min(eRect.Width, eRect.Height);
            if (_minsize >= 1500)
                m_samplinggap = 137;
            else if (_minsize >= 800)
                m_samplinggap = 37;
            else
                m_samplinggap = 11;

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

                bool bOK = true;

                switch (borderType)
                {
                    case BorderTypeEnum.LEFT:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, true, true);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, true, true, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, true, true, Color.FromArgb(0, 255, 0));

                        //if (pointF0.X >= rect0.Width - 1 || pointF1.X >= rect0.Width - 1)
                        //    bOK = false;

                        if (pointF1.X >= rect0.Width - 1 || pointF1.X <= 1)
                        {
                            pointF1 = new PointF(pointF1tmp.X, pointF1tmp.Y);
                        }
                        else
                        {
                            pointF1tmp = new PointF(pointF1.X, pointF1.Y);
                        }

                        break;
                    case BorderTypeEnum.RIGHT:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, true, false);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, true, false, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, true, false, Color.FromArgb(0, 255, 0));

                        //if (pointF0.X <= 0 || pointF1.X <= 0)
                        //    bOK = false;

                        if (pointF1.X >= rect0.Width - 1 || pointF1.X <= 1)
                        {
                            pointF1 = new PointF(pointF1tmp.X, pointF1tmp.Y);
                        }
                        else
                        {
                            pointF1tmp = new PointF(pointF1.X, pointF1.Y);
                        }
                        break;
                    case BorderTypeEnum.TOP:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, false, true);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, false, true, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, false, true, Color.FromArgb(0, 255, 0));

                        //if (pointF0.Y >= rect0.Height - 1 || pointF1.Y >= rect0.Height - 1)
                        //    bOK = false;

                        if (pointF1.Y >= rect0.Height - 1 || pointF1.Y <= 1)
                        {
                            pointF1 = new PointF(pointF1tmp.X, pointF1tmp.Y);
                        }
                        else
                        {
                            pointF1tmp = new PointF(pointF1.X, pointF1.Y);
                        }
                        break;
                    case BorderTypeEnum.BOTTOM:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, false, false);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, false, false, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, false, false, Color.FromArgb(0, 255, 0));
                        //pointF1 = jzfind.GetBoraderPointv2(bitmap, false, false, Color.FromArgb(0, 0, 0), true);

                        //if (pointF0.Y <= 0 || pointF1.Y <= 0)
                        //    bOK = false;

                        if (pointF1.Y >= rect0.Height - 1 || pointF1.Y <= 1)
                        {
                            pointF1 = new PointF(pointF1tmp.X, pointF1tmp.Y);
                        }
                        else
                        {
                            pointF1tmp = new PointF(pointF1.X, pointF1.Y);
                        }
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


                //if (pointF0.X > 0 && pointF0.Y > 0 && pointF1.X > 0 && pointF1.Y > 0)
                if(bOK)
                {
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
                }

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
        private void _get_border_pointf_v8_1(Bitmap ebmpInput, Bitmap ebmpInput0, Rectangle eRect, BorderTypeEnum borderType, out GlueRegionClass glueRegion, int iSized = 2)
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
            if (m_IsSaveTemp)
            {
                Bitmap bmpGlueOrg1 = new Bitmap(ebmpInput0);
                Graphics graphics = Graphics.FromImage(bmpGlueOrg1);
                graphics.DrawRectangle(new Pen(Color.Red, 2), m_rect_org);
                graphics.Dispose();

                if (!System.IO.Directory.Exists("D:\\testtest\\x"))
                    System.IO.Directory.CreateDirectory("D:\\testtest\\x");
                bmpGlueOrg1.Save("D:\\testtest\\x\\x" + RelateAnalyzeString + borderType.ToString() + ".png",
                    System.Drawing.Imaging.ImageFormat.Png);
                //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
            }

            int j = 0;
            PointF pointF0 = new PointF();
            PointF pointF1 = new PointF();
            double[] m_distance = new double[10];
            int m_samplinggap = 21;
            //int iyoffset = (int)(m_samplinggap / 1.5);
            //double maxv = -1000;
            //double minv = 1000;
            //m_samplinggap = 7;

            //int m_samplinggap = 137;
            //m_samplinggap = 37;

            int _minsize = Math.Min(eRect.Width, eRect.Height);
            if (_minsize >= 1500)
                m_samplinggap = 137;
            else if (_minsize >= 800)
                m_samplinggap = 37;
            else
                m_samplinggap = 11;

            double maxv = -1000;
            double minv = 1000;
            int iyoffset = (int)(m_samplinggap / 1.5);

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

                //if (m_IsSaveTemp)
                //{
                //    //bitmap.Save("D:\\testtest\\x\\org" + RelateAnalyzeString + borderType.ToString() + "bmpInput_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //    bitmap0.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "bmpInput0_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                //}


                JzFindObjectClass jzfind = new JzFindObjectClass();

                switch (borderType)
                {
                    case BorderTypeEnum.LEFT:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, true, true);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, true, true, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, true, true, Color.FromArgb(255, 255, 255));
                        //pointF1 = jzfind.GetBoraderPointv2(bitmap, true, true, Color.FromArgb(0, 0, 0), true);
                        break;
                    case BorderTypeEnum.RIGHT:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, true, false);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, true, false, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, true, false, Color.FromArgb(255, 255, 255));
                        //pointF1 = jzfind.GetBoraderPointv2(bitmap, true, false, Color.FromArgb(0, 0, 0), false);
                        break;
                    case BorderTypeEnum.TOP:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, false, true);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, false, true, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, false, true, Color.FromArgb(255, 255, 255));
                        //pointF1 = jzfind.GetBoraderPointv2(bitmap, false, true, Color.FromArgb(0, 0, 0), false);
                        break;
                    case BorderTypeEnum.BOTTOM:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, false, false);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, false, false, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, false, false, Color.FromArgb(255, 255, 255));
                        //pointF1 = jzfind.GetBoraderPointv2(bitmap, false, false, Color.FromArgb(0, 0, 0), true);
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
                //    //bitmap.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "bmpInput_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //    bitmap0.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "bmpInput0_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
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
        private void _get_border_pointf_v8_1_blackBigtor(Bitmap ebmpInput, Bitmap ebmpInput0, Rectangle eRect, BorderTypeEnum borderType, out GlueRegionClass glueRegion, int iSized = 2)
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
            if (m_IsSaveTemp)
            {
                Bitmap bmpGlueOrg1 = new Bitmap(ebmpInput0);
                Graphics graphics = Graphics.FromImage(bmpGlueOrg1);
                graphics.DrawRectangle(new Pen(Color.Red, 2), m_rect_org);
                graphics.Dispose();
                if (!System.IO.Directory.Exists("D:\\testtest\\x"))
                    System.IO.Directory.CreateDirectory("D:\\testtest\\x");
                bmpGlueOrg1.Save("D:\\testtest\\x\\x" + RelateAnalyzeString + borderType.ToString() + ".png",
                    System.Drawing.Imaging.ImageFormat.Png);
                //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
            }

            int j = 0;
            PointF pointF0 = new PointF();
            PointF pointF1 = new PointF();
            double[] m_distance = new double[10];
            int m_samplinggap = 21;
            //int iyoffset = (int)(m_samplinggap / 1.5);
            //double maxv = -1000;
            //double minv = 1000;
            //m_samplinggap = 7;

            //int m_samplinggap = 137;
            //m_samplinggap = 37;

            int _minsize = Math.Min(eRect.Width, eRect.Height);
            if (_minsize >= 1500)
                m_samplinggap = 137;
            else if (_minsize >= 800)
                m_samplinggap = 37;
            else
                m_samplinggap = 11;

            double maxv = -1000;
            double minv = 1000;
            int iyoffset = (int)(m_samplinggap / 1.5);

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

                //if (m_IsSaveTemp)
                //{
                //    //bitmap.Save("D:\\testtest\\x\\org" + RelateAnalyzeString + borderType.ToString() + "bmpInput_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //    bitmap0.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "bmpInput0_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                //}


                JzFindObjectClass jzfind = new JzFindObjectClass();

                switch (borderType)
                {
                    case BorderTypeEnum.LEFT:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, true, true);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, true, true, Color.FromArgb(255, 255, 255), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, true, true, Color.FromArgb(255, 255, 255));
                        //pointF1 = jzfind.GetBoraderPointv2(bitmap, true, true, Color.FromArgb(0, 0, 0), true);
                        break;
                    case BorderTypeEnum.RIGHT:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, true, false);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, true, false, Color.FromArgb(255, 255, 255), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, true, false, Color.FromArgb(255, 255, 255));
                        //pointF1 = jzfind.GetBoraderPointv2(bitmap, true, false, Color.FromArgb(0, 0, 0), false);
                        break;
                    case BorderTypeEnum.TOP:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, false, true);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, false, true, Color.FromArgb(255, 255, 255), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, false, true, Color.FromArgb(255, 255, 255));
                        //pointF1 = jzfind.GetBoraderPointv2(bitmap, false, true, Color.FromArgb(0, 0, 0), false);
                        break;
                    case BorderTypeEnum.BOTTOM:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, false, false);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, false, false, Color.FromArgb(255, 255, 255), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, false, false, Color.FromArgb(255, 255, 255));
                        //pointF1 = jzfind.GetBoraderPointv2(bitmap, false, false, Color.FromArgb(0, 0, 0), true);
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
                //    //bitmap.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "bmpInput_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //    bitmap0.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "bmpInput0_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
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

            JetEazy.BasicSpace.DataHistogramClass dataHistogram = new JetEazy.BasicSpace.DataHistogramClass(15000, 2);
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
                    if (!System.IO.Directory.Exists("D:\\testtest\\x"))
                        System.IO.Directory.CreateDirectory("D:\\testtest\\x");
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
        private void _get_border_pointf_v1_antu(Bitmap ebmpInput, Bitmap ebmpInput0, Rectangle eRect, BorderTypeEnum borderType, out GlueRegionClass glueRegion)
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
            PointF pointF1tmp = new PointF();
            double[] m_distance = new double[10];
            int m_samplinggap = 137;
            m_samplinggap = 37;

            int _minsize = Math.Min(eRect.Width, eRect.Height);
            if (_minsize >= 1500)
                m_samplinggap = 137;
            else if (_minsize >= 800)
                m_samplinggap = 37;
            else
                m_samplinggap = 11;

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

                bool bOK = true;

                switch (borderType)
                {
                    case BorderTypeEnum.LEFT:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, true, true);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, true, true, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, true, true, Color.FromArgb(255, 255, 255));

                        //if (pointF0.X >= rect0.Width - 1 || pointF1.X >= rect0.Width - 1)
                        //    bOK = false;

                        if (pointF1.X >= rect0.Width - 1 || pointF1.X <= 1)
                        {
                            pointF1 = new PointF(pointF1tmp.X, pointF1tmp.Y);
                        }
                        else
                        {
                            pointF1tmp = new PointF(pointF1.X, pointF1.Y);
                        }

                        break;
                    case BorderTypeEnum.RIGHT:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, true, false);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, true, false, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, true, false, Color.FromArgb(255, 255, 255));

                        //if (pointF0.X <= 0 || pointF1.X <= 0)
                        //    bOK = false;

                        if (pointF1.X >= rect0.Width - 1 || pointF1.X <= 1)
                        {
                            pointF1 = new PointF(pointF1tmp.X, pointF1tmp.Y);
                        }
                        else
                        {
                            pointF1tmp = new PointF(pointF1.X, pointF1.Y);
                        }
                        break;
                    case BorderTypeEnum.TOP:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, false, true);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, false, true, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, false, true, Color.FromArgb(255, 255, 255));

                        //if (pointF0.Y >= rect0.Height - 1 || pointF1.Y >= rect0.Height - 1)
                        //    bOK = false;

                        if (pointF1.Y >= rect0.Height - 1 || pointF1.Y <= 1)
                        {
                            pointF1 = new PointF(pointF1tmp.X, pointF1tmp.Y);
                        }
                        else
                        {
                            pointF1tmp = new PointF(pointF1.X, pointF1.Y);
                        }
                        break;
                    case BorderTypeEnum.BOTTOM:
                        //pointF0 = jzfind.GetBoraderPoint(bitmap, false, false);
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, false, false, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointv2(bitmap, false, false, Color.FromArgb(255, 255, 255));
                        //pointF1 = jzfind.GetBoraderPointv2(bitmap, false, false, Color.FromArgb(0, 0, 0), true);

                        //if (pointF0.Y <= 0 || pointF1.Y <= 0)
                        //    bOK = false;

                        if (pointF1.Y >= rect0.Height - 1 || pointF1.Y <= 1)
                        {
                            pointF1 = new PointF(pointF1tmp.X, pointF1tmp.Y);
                        }
                        else
                        {
                            pointF1tmp = new PointF(pointF1.X, pointF1.Y);
                        }
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


                //if (pointF0.X > 0 && pointF0.Y > 0 && pointF1.X > 0 && pointF1.Y > 0)
                if (bOK)
                {
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
                }

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

        private void _get_border_pointf_NormalEx(Bitmap ebmpInput, Bitmap ebmpInput0, Rectangle eRect, BorderTypeEnum borderType, out GlueRegionClass glueRegion)
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
            PointF pointF1tmp = new PointF();
            double[] m_distance = new double[10];
            int m_samplinggap = 137;
            m_samplinggap = 37;

            int _minsize = Math.Min(eRect.Width, eRect.Height);
            if (_minsize >= 1500)
                m_samplinggap = 137;
            else if (_minsize >= 800)
                m_samplinggap = 37;
            else
                m_samplinggap = 11;

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

                //if (m_IsSaveTemp)
                //{
                //    if (!System.IO.Directory.Exists("D:\\testtest\\x"))
                //        System.IO.Directory.CreateDirectory("D:\\testtest\\x");
                //    bitmap.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "bmpInputORG_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                //}

                Bitmap bitmapHIS = bmpGlueOrg.Clone(rect0, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                JzHistogramClass histogramEx = new JzHistogramClass(2);
                int _upperCutGrade = 255;
                histogramEx.GetHistogram(bitmapHIS, _upperCutGrade);
                bitmapHIS.Dispose();

                AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
                bitmap = grayscale.Apply(bitmap);
                int thvalue = 80;// (histogramEx.MeanGrade - histogramEx.MinGrade) / 2 + histogramEx.MinGrade;
                thvalue = histogramEx.MeanGrade - 2;
                AForge.Imaging.Filters.Threshold threshold = new AForge.Imaging.Filters.Threshold(thvalue);
                bitmap = threshold.Apply(bitmap);

                int icount = 3;
                int ix = 0;
                while (ix < icount)
                {
                    AForge.Imaging.Filters.Erosion3x3 erosion3X3 = new AForge.Imaging.Filters.Erosion3x3();
                    bitmap = erosion3X3.Apply(bitmap);
                    ix++;
                }
                ix = 0;
                while (ix < icount)
                {
                    AForge.Imaging.Filters.Dilatation3x3 dilatation3X3 = new AForge.Imaging.Filters.Dilatation3x3();
                    bitmap = dilatation3X3.Apply(bitmap);
                    ix++;
                }

                //AForge.Imaging.Filters.Invert invert = new AForge.Imaging.Filters.Invert();
                //bitmap = invert.Apply(bitmap);

                //if (m_IsSaveTemp)
                //{
                //    if (!System.IO.Directory.Exists("D:\\testtest\\x"))
                //        System.IO.Directory.CreateDirectory("D:\\testtest\\x");
                //    bitmap.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "bmpInputORG1_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                //}

                JzFindObjectClass jzfind = new JzFindObjectClass();

                bool bOK = true;

                switch (borderType)
                {
                    case BorderTypeEnum.LEFT:
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, true, true, Color.FromArgb(0, 0, 0), true);
                        //pointF1 = jzfind.GetBoraderPointv2(bitmap, true, true, Color.FromArgb(0, 0, 0));
                        pointF1 = jzfind.GetBoraderPointBW(bitmap, true, true);

                        if (pointF1.X >= rect0.Width - 1 || pointF1.X <= 1)
                            bOK = false;

                        //if (pointF1.X >= rect0.Width - 1 || pointF1.X <= 1)
                        //{
                        //    pointF1 = new PointF(pointF1tmp.X, pointF1tmp.Y);
                        //}
                        //else
                        //{
                        //    pointF1tmp = new PointF(pointF1.X, pointF1.Y);
                        //}

                        break;
                    case BorderTypeEnum.RIGHT:
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, true, false, Color.FromArgb(0, 0, 0), true);
                        //pointF1 = jzfind.GetBoraderPointv2(bitmap, true, false, Color.FromArgb(0, 0, 0));
                        pointF1 = jzfind.GetBoraderPointBW(bitmap, true, false);

                        if (pointF1.X >= rect0.Width - 1 || pointF1.X <= 1)
                            bOK = false;

                        //if (pointF1.X >= rect0.Width - 1 || pointF1.X <= 1)
                        //{
                        //    pointF1 = new PointF(pointF1tmp.X, pointF1tmp.Y);
                        //}
                        //else
                        //{
                        //    pointF1tmp = new PointF(pointF1.X, pointF1.Y);
                        //}
                        break;
                    case BorderTypeEnum.TOP:
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, false, true, Color.FromArgb(0, 0, 0), true);
                        pointF1 = jzfind.GetBoraderPointBW(bitmap, false, true);

                        if (pointF1.Y >= rect0.Height - 1 || pointF1.Y <= 1)
                            bOK = false;

                        //if (pointF1.Y >= rect0.Height - 1 || pointF1.Y <= 1)
                        //{
                        //    pointF1 = new PointF(pointF1tmp.X, pointF1tmp.Y);
                        //}
                        //else
                        //{
                        //    pointF1tmp = new PointF(pointF1.X, pointF1.Y);
                        //}
                        break;
                    case BorderTypeEnum.BOTTOM:
                        pointF0 = jzfind.GetBoraderPointv2(bitmap0, false, false, Color.FromArgb(0, 0, 0), true);
                        //pointF1 = jzfind.GetBoraderPointv2(bitmap, false, false, Color.FromArgb(0, 0, 0));
                        pointF1 = jzfind.GetBoraderPointBW(bitmap, false, false);

                        if (pointF1.Y >= rect0.Height - 1 || pointF1.Y <= 1)
                            bOK = false;

                        //if (pointF1.Y >= rect0.Height - 1 || pointF1.Y <= 1)
                        //{
                        //    pointF1 = new PointF(pointF1tmp.X, pointF1tmp.Y);
                        //}
                        //else
                        //{
                        //    pointF1tmp = new PointF(pointF1.X, pointF1.Y);
                        //}
                        break;
                }

                if (bOK)
                {
                    m_distance[j] = GetPointLength(pointF0, pointF1);

                    maxv = Math.Max(maxv, m_distance[j]);
                    minv = Math.Min(minv, m_distance[j]);

                    pointF0.X += rect0.X;
                    pointF0.Y += rect0.Y;

                    pointF1.X += rect0.X;
                    pointF1.Y += rect0.Y;

                    glueRegion.AddPt(pointF0);//内芯片的点
                    glueRegion.AddPtIN(pointF1);//外围胶的点
                }

                //if (m_IsSaveTemp)
                //{
                //    if (!System.IO.Directory.Exists("D:\\testtest\\x"))
                //        System.IO.Directory.CreateDirectory("D:\\testtest\\x");
                //    bitmap.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "bmpInput_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                //}

                j++;

                bitmap.Dispose();
            }

            glueRegion.LengthMax = maxv;
            glueRegion.LengthMin = minv;

            bmpGlueOrg.Dispose();
        }


        private void _get_border_pointf_PADG1(Bitmap ebmpInput, 
            Bitmap ebmpInput0, 
            Rectangle eRect,
            Rectangle eRectORG,
            PADG1Class ePADG1,
            BorderTypeEnum borderType, 
            out GlueRegionClass glueRegion)
        {
            glueRegion = new GlueRegionClass();
            glueRegion.Reset();

            //int iwidth = 60;
            int iheight = 3;
            Bitmap bmpGlueOrg = (Bitmap)ebmpInput.Clone();// new Bitmap(ebmpInput);
            //Bitmap bmpGlueOrg0 = (Bitmap)ebmpInput0.Clone();//new Bitmap(ebmpInput0);
            Rectangle m_rect_org = new Rectangle(eRect.X, eRect.Y, eRect.Width, eRect.Height);
            //m_rect_org.Inflate(-iwidth, -iwidth);

            //原始的尺寸 扩大 之后的限流外围
            Rectangle m_rect_last = new Rectangle(eRectORG.X, eRectORG.Y, eRectORG.Width, eRectORG.Height);
            m_rect_last.Inflate(ePADG1.FindX, ePADG1.FindY);

            Rectangle m_rect_before = new Rectangle(eRectORG.X, eRectORG.Y, eRectORG.Width, eRectORG.Height);
            m_rect_before.Inflate(-ePADG1.FindInX, -ePADG1.FindInY);

            int j = 0;
            PointF pointF0 = new PointF();
            PointF pointF1 = new PointF();
            PointF pointF1tmp = new PointF();
            double[] m_distance = new double[10];
            int m_samplinggap = 137;
            m_samplinggap = 37;

            int _minsize = Math.Min(eRect.Width, eRect.Height);
            if (_minsize >= 1500)
                m_samplinggap = 137;
            else if (_minsize >= 800)
                m_samplinggap = 37;
            else
                m_samplinggap = 11;

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


            JzMVDEdgeWidthClass jzMVDEdgeWidthClass = new JzMVDEdgeWidthClass();
            jzMVDEdgeWidthClass.HalfKernelSize = ePADG1.HalfKernelSize;
            jzMVDEdgeWidthClass.ContrastTH = ePADG1.ContrastTH;
            jzMVDEdgeWidthClass.FindMode = ePADG1.FindMode;

            //Bitmap bitmap = bmpGlueOrg.Clone(new Rectangle(0, 0, bmpGlueOrg.Width, bmpGlueOrg.Height),
            //        System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            //jzMVDEdgeWidthClass.Run(bitmap);

            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            Bitmap bmp24 = grayscale.Apply(bmpGlueOrg);
            VisionDesigner.CMvdImage cInputImg2 = BitmapToCMvdImage(bmp24);
            if (cInputImg2.PixelFormat != MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08)
            {
                //当前程序仅支持mono8。因此像素格会转换.
                cInputImg2.ConvertImagePixelFormat(MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08);
            }
            //cInputImg2.InitImage("InputTest2.bmp");

            //cInputImg2.SaveImage($"D:\\Data\\cInputImg2run_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.bmp", MVD_FILE_FORMAT.MVD_FILE_BMP);

            //if (m_IsSaveTemp)
            //{
            //    //g.DrawRectangle(new Pen(Color.Red), rect);
            //    cInputImg2.SaveImage("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "cInputImg2run" + ".png",
            //        MVD_FILE_FORMAT.MVD_FILE_PNG);
            //    //bmpglueout.Save(Universal.CalTestPath + "\\" + _CalPageIndex() + RelateAnalyzeString + "bmpInputout" + ".png",
            //    //    System.Drawing.Imaging.ImageFormat.Png);

            //    //bmpglueout.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpInputout" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            //    ////bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
            //}

            bmp24.Dispose();
            grayscale = null;
            jzMVDEdgeWidthClass.SetImage(cInputImg2);

            j = 0;
            while (j < m_distance.Length)
            {
                Rectangle rect0 = new Rectangle(0, m_rect_org.Y + j * m_samplinggap, eRect.X, iheight);

                switch (borderType)
                {
                    case BorderTypeEnum.LEFT:
                        rect0 = new Rectangle(m_rect_last.X, m_rect_org.Y + j * m_samplinggap + iyoffset, m_rect_before.X - m_rect_last.X, iheight);
                        break;
                    case BorderTypeEnum.RIGHT:
                        rect0 = new Rectangle(m_rect_before.Right, m_rect_org.Y + j * m_samplinggap + iyoffset, m_rect_last.Right - m_rect_before.Right, iheight);
                        break;
                    case BorderTypeEnum.TOP:
                        rect0 = new Rectangle(m_rect_org.X + j * m_samplinggap + iyoffset, m_rect_last.Y, iheight, m_rect_before.Y - m_rect_last.Y);
                        break;
                    case BorderTypeEnum.BOTTOM:
                        rect0 = new Rectangle(m_rect_org.X + j * m_samplinggap + iyoffset, m_rect_before.Bottom, iheight, m_rect_last.Bottom - m_rect_before.Bottom);
                        break;
                }

                //Bitmap bitmap0 = bmpGlueOrg0.Clone(rect0, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                //Bitmap bitmap = bmpGlueOrg.Clone(rect0, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                //Bitmap bitmap = bmpGlueOrg.Clone(new Rectangle(0,0, bmpGlueOrg.Width, bmpGlueOrg.Height), 
                //    System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                //JzFindObjectClass jzfind = new JzFindObjectClass();

                bool bOK = true;
                //bool bchk = true;

                switch (borderType)
                {
                    case BorderTypeEnum.LEFT:
                        jzMVDEdgeWidthClass.FindOrient = false;
                        //bOK = jzMVDEdgeWidthClass.Run(bitmap, rect0);
                        break;
                    case BorderTypeEnum.RIGHT:
                        jzMVDEdgeWidthClass.FindOrient = false;
                        //bOK = jzMVDEdgeWidthClass.Run(bitmap, rect0);
                        break;
                    case BorderTypeEnum.TOP:
                        jzMVDEdgeWidthClass.FindOrient = true;
                        //bOK = jzMVDEdgeWidthClass.Run(bitmap, rect0);
                        break;
                    case BorderTypeEnum.BOTTOM:
                        jzMVDEdgeWidthClass.FindOrient = true;
                        //bOK = jzMVDEdgeWidthClass.Run(bitmap, rect0);
                        break;
                }
                bOK = jzMVDEdgeWidthClass.Run3(rect0);
                if (bOK)
                {
                    pointF0 = new PointF(jzMVDEdgeWidthClass.p0ret.X, jzMVDEdgeWidthClass.p0ret.Y);
                    pointF1 = new PointF(jzMVDEdgeWidthClass.p1ret.X, jzMVDEdgeWidthClass.p1ret.Y);
                }
                else
                {

                    pointF0 = new PointF(rect0.X, rect0.Y);
                    pointF1 = new PointF(rect0.X, rect0.Y);
                    //switch (borderType)
                    //{
                    //    case BorderTypeEnum.LEFT:
                    //        pointF0 = new PointF(m_rect_last.X, rect0.Y);
                    //        pointF1 = new PointF(m_rect_last.X, rect0.Y);
                    //        break;
                    //    case BorderTypeEnum.RIGHT:
                    //        jzMVDEdgeWidthClass.FindOrient = false;
                    //        //bOK = jzMVDEdgeWidthClass.Run(bitmap, rect0);
                    //        break;
                    //    case BorderTypeEnum.TOP:
                    //        jzMVDEdgeWidthClass.FindOrient = true;
                    //        //bOK = jzMVDEdgeWidthClass.Run(bitmap, rect0);
                    //        break;
                    //    case BorderTypeEnum.BOTTOM:
                    //        jzMVDEdgeWidthClass.FindOrient = true;
                    //        //bOK = jzMVDEdgeWidthClass.Run(bitmap, rect0);
                    //        break;
                    //}
                }

                //if (pointF0.X > 0 && pointF0.Y > 0 && pointF1.X > 0 && pointF1.Y > 0)
                if (bOK)
                {
                    //pointF0 = new PointF(jzMVDEdgeWidthClass.p0ret.X, jzMVDEdgeWidthClass.p0ret.Y);
                    //pointF1 = new PointF(jzMVDEdgeWidthClass.p1ret.X, jzMVDEdgeWidthClass.p1ret.Y);

                    m_distance[j] = GetPointLength(pointF0, pointF1);

                    maxv = Math.Max(maxv, m_distance[j]);
                    minv = Math.Min(minv, m_distance[j]);

                    //pointF0.X += rect0.X;
                    //pointF0.Y += rect0.Y;

                    //pointF1.X += rect0.X;
                    //pointF1.Y += rect0.Y;

                    glueRegion.AddPt(pointF0);//内芯片的点
                    glueRegion.AddPtIN(pointF1);//外围胶的点
                }

                //if (m_IsSaveTemp)
                //{
                //    bitmap.Save("D:\\testtest\\x\\" + RelateAnalyzeString + borderType.ToString() + "bmpInput_" + j.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
                //}

                j++;

                
            }

            glueRegion.LengthMax = maxv;
            glueRegion.LengthMin = minv;

            //bitmap.Dispose();
            bmpGlueOrg.Dispose();
        }


        public class PADRegionClass
        {
            HistogramClass m_Histogram = new HistogramClass(2);

            public RectangleF ConnerRectF = new RectangleF(0, 0, 100, 100);
            public double ConnerAngle = 0;
            public PointF[] GetConner(float inflantValue = 0)
            {
                RectangleF tempRectFx = ConnerRectF;
                if (inflantValue != 0)
                    tempRectFx.Inflate(inflantValue, inflantValue);
                PointF[] _myPointFs = RectFToPointF(tempRectFx, -ConnerAngle);
                return _myPointFs;
            }

            public int GetGrayMin(Bitmap ebmpInput)
            {
                Bitmap _bmpnewIpd = new Bitmap(ebmpInput);
                Bitmap _bmpnewIpdmask = new Bitmap(ebmpInput);
                //填充中间的芯片位置
                Graphics _gPADV1 = Graphics.FromImage(_bmpnewIpdmask);
                _gPADV1.Clear(Color.Black);
                _gPADV1.FillPolygon(Brushes.White, GetConner(3.5f));
                //_gPADV1.FillPolygon(Brushes.White, m_PADRegion.RegionPtFCorner);
                //_gPADV1.FillPolygon(Brushes.White, m_PADRegion.RegionPtFCornerORG);
                _gPADV1.Dispose();
                m_Histogram.GetHistogram(_bmpnewIpd, _bmpnewIpdmask, true);
                //m_Histogram.GetHistogram(_bmpnewIpd, 200);
                int _min = m_Histogram.MeanGrade;

                return _min;
            }
            public Bitmap GetFillMean(Bitmap ebmpInput,int eMean)
            {
                Bitmap _bmpnewIpd = new Bitmap(ebmpInput);
                //填充中间的芯片位置
                Graphics _gPADV1 = Graphics.FromImage(_bmpnewIpd);
                //_gPADV1.Clear(Color.Black);
                Brush brush = new SolidBrush(Color.FromArgb(eMean, eMean, eMean));
                _gPADV1.FillPolygon(brush, GetConner(0.5f));
                //_gPADV1.FillPolygon(Brushes.White, m_PADRegion.RegionPtFCorner);
                //_gPADV1.FillPolygon(Brushes.White, m_PADRegion.RegionPtFCornerORG);
                _gPADV1.Dispose();
                //m_Histogram.GetHistogram(_bmpnewIpd, _bmpnewIpdmask, true);
                //m_Histogram.GetHistogram(_bmpnewIpd, 200);
                //int _min = m_Histogram.MeanGrade;

                return _bmpnewIpd;
            }

            public double RegionWidthReal { get; set; } = 10;
            public double RegionHeightReal { get; set; } = 10;
            public double RegionAreaReal { get; set; } = 100;

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

            /// <summary>
            /// QLE检测银胶的角点位置
            /// </summary>
            public PointF[] RegionPtFCornerQLE = new PointF[4];
            public Rectangle QleMaxRect = new Rectangle(0, 0, 1, 1);
            public Rectangle RegionForEdgeRect = new Rectangle(0, 0, 1, 1);
            public Bitmap bmpThreshold = new Bitmap(1, 1);
            public JRotatedRectangleF jRotatedRectangleF2 = new JRotatedRectangleF();
            /// <summary>
            /// 生成给有无胶使用
            /// </summary>
            public Bitmap bmpChipForWetherGlue = new Bitmap(1, 1);
            public bool Isleftoverright { get; set; } = false;
            public List<PointF[]> listPointF = new List<PointF[]>();
            public List<BallBlobClass> ballBlobClasses = new List<BallBlobClass>();

            public Bitmap GetChipGlue(int fSized, out PointF oCenterPointF)
            {
                Size size = new Size(bmpChipForWetherGlue.Width, bmpChipForWetherGlue.Height);
                if (fSized == 0)
                    fSized = 1;
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
                ballBlobClasses.Clear();
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
            public void SetPointFQLE(PointF[] eInputPtF)
            {
                if (eInputPtF.Length == 4)
                {
                    RegionPtFCornerQLE[0] = eInputPtF[0];
                    RegionPtFCornerQLE[1] = eInputPtF[1];
                    RegionPtFCornerQLE[2] = eInputPtF[2];
                    RegionPtFCornerQLE[3] = eInputPtF[3];
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

            PointF[] RectFToPointF(RectangleF xRectF, double xAngle)
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
            public PointF GetRectFCenter(RectangleF RectF)
            {
                return new PointF(RectF.X + (RectF.Width / 2), RectF.Y + (RectF.Height / 2));
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
        }

        public class GlueRegionClass
        {
            public double LengthMax { get; set; } = 0;
            public double LengthMin { get; set; } = 0;

            public double LengthSlotMax { get; set; } = 0;
            public double LengthSlotMin { get; set; } = 0;

            private PointF[] LengthPointFs = new PointF[4];
            private PointF[] LengthPointFsIN = new PointF[4];

            private PointF[] LengthPointFsIN2 = new PointF[4];

            /// <summary>
            /// 最外层
            /// </summary>
            private List<PointF> LengthPointFsList = new List<PointF>();
            /// <summary>
            /// 最内层
            /// </summary>
            private List<PointF> LengthPointFsListIN = new List<PointF>();

            /// <summary>
            /// IPD的实体胶
            /// </summary>
            private List<PointF> LengthPointFsListIN2 = new List<PointF>();
            public void Reset()
            {
                LengthMax = 0;
                LengthMin = 0;

                LengthSlotMax = 0;
                LengthSlotMin = 0;

                LengthPointFs[0] = new PointF(0, 10);
                LengthPointFs[1] = new PointF(10, 10);
                LengthPointFs[2] = new PointF(10, 0);
                LengthPointFs[3] = new PointF(0, 0);

                LengthPointFsIN[0] = new PointF(0, 10);
                LengthPointFsIN[1] = new PointF(10, 10);
                LengthPointFsIN[2] = new PointF(10, 0);
                LengthPointFsIN[3] = new PointF(0, 0);

                LengthPointFsIN2[0] = new PointF(0, 10);
                LengthPointFsIN2[1] = new PointF(10, 10);
                LengthPointFsIN2[2] = new PointF(10, 0);
                LengthPointFsIN2[3] = new PointF(0, 0);

                LengthPointFsList.Clear();
                LengthPointFsListIN.Clear();
                LengthPointFsListIN2.Clear();
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

            public void AddPtIN2(PointF ept)
            {
                LengthPointFsListIN2.Add(ept);
            }
            public PointF[] GetPointFIN2()
            {
                if (LengthPointFsListIN2.Count >= 4)
                {
                    LengthPointFsIN2 = new PointF[LengthPointFsListIN2.Count];
                    int i = 0;
                    foreach (PointF p in LengthPointFsListIN2)
                    {
                        LengthPointFsIN2[i] = p;
                        i++;
                    }
                }
                else
                {
                    LengthPointFsIN2 = new PointF[4];
                    LengthPointFsIN2[0] = new PointF(0, 10);
                    LengthPointFsIN2[1] = new PointF(10, 10);
                    LengthPointFsIN2[2] = new PointF(10, 0);
                    LengthPointFsIN2[3] = new PointF(0, 0);
                }

                return LengthPointFsIN2;

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

            public void Run()
            {
                double maxv = -1000;
                double minv = 1000;

                int i = 0;
                while (i < LengthPointFsList.Count)
                {
                    double dis = GetPointLength(LengthPointFsListIN[i], LengthPointFsList[i]);
                    maxv = Math.Max(maxv, dis);
                    minv = Math.Min(minv, dis);

                    i++;
                }

                LengthMax = maxv;
                LengthMin = minv;

            }
            double GetPointLength(PointF P1, PointF P2)
            {
                return Math.Sqrt((double)Math.Pow((P1.X - P2.X), 2) + Math.Pow((P1.Y - P2.Y), 2));
            }

            public void RunSlot()
            {
                double maxv = -1000;
                double minv = 1000;

                int i = 0;
                while (i < LengthPointFsListIN2.Count)
                {
                    double dis = GetPointLength(LengthPointFsListIN2[i], LengthPointFsList[i]);
                    maxv = Math.Max(maxv, dis);
                    minv = Math.Min(minv, dis);

                    i++;
                }

                LengthSlotMax = maxv;
                LengthSlotMin = minv;

            }

        }
        public class BorderLineRunClass
        {
            public int index;
            public Bitmap bmp0;
            public Bitmap bmp1;
            public Rectangle rect0;
            public BorderTypeEnum Border;

            public Rectangle rectORG;
            public PADG1Class padG1;

        }
        public class BallBlobClass
        {
            //public int w = 0;
            //public int h = 0;
            public int area = 0;
            public Rectangle rect = new Rectangle();
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
            str += (ChipDirlevel ? "1" : "0") + Universal.SeperateCharB;
            str += FontSize.ToString() + Universal.SeperateCharB;
            str += LineWidth.ToString() + Universal.SeperateCharB;
            str += ((int)PADAICategory).ToString() + Universal.SeperateCharB;     //0
            str += (ChipGleCheck ? "1" : "0") + Universal.SeperateCharB;

            str += GleWidthUpper.ToString() + Universal.SeperateCharB;
            str += GleWidthLower.ToString() + Universal.SeperateCharB;
            str += GleHeightUpper.ToString() + Universal.SeperateCharB;
            str += GleHeightLower.ToString() + Universal.SeperateCharB;
            str += GleAreaUpper.ToString() + Universal.SeperateCharB;
            str += GleAreaLower.ToString() + Universal.SeperateCharB;

            str += ((int)PadInspectMethod).ToString() + Universal.SeperateCharB;
            str += PADINSPECTOPString.ToString() + Universal.SeperateCharB;
            str += PADChipInBlobGrayThreshold.ToString() + Universal.SeperateCharB;

            str += (ChipFindWhite ? "1" : "0") + Universal.SeperateCharB;
            str += (GLEFindWhite ? "1" : "0") + Universal.SeperateCharB;

            str += FourSideNoGluePassValue.ToString() + Universal.SeperateCharB;
            str += ((int)GlueChipSlotDir).ToString() + Universal.SeperateCharB;     //0

            str += GlueTopBottomOffset.ToString() + Universal.SeperateCharB;
            str += GlueLeftRightOffset.ToString() + Universal.SeperateCharB;
            str += ((int)ChipNoGlueMode).ToString() + Universal.SeperateCharB;     //0
            str += ((int)ChipNoHaveMode).ToString() + Universal.SeperateCharB;     //0
            str += ChipNoHaveModeOpString.ToString() + Universal.SeperateCharB;
            str += PADExtendOPString + Universal.SeperateCharB;

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
                if (strs.Length > 33)
                {
                    ChipDirlevel = strs[33] == "1";
                }
                if (strs.Length > 35)
                {
                    FontSize = int.Parse(strs[34]);
                    LineWidth = int.Parse(strs[35]);
                }
                if (strs.Length > 36)
                {
                    if (string.IsNullOrEmpty(strs[36]))
                        strs[36] = "0";
                    PADAICategory = (AICategory)int.Parse(strs[36]);
                }
                if (strs.Length > 37)
                {
                    ChipGleCheck = strs[37] == "1";
                }
                if (strs.Length > 43)
                {
                    GleWidthUpper = double.Parse(strs[38]);
                    GleWidthLower = double.Parse(strs[39]);
                    GleHeightUpper = double.Parse(strs[40]);
                    GleHeightLower = double.Parse(strs[41]);
                    GleAreaUpper = double.Parse(strs[42]);
                    GleAreaLower = double.Parse(strs[43]);
                }
                if (strs.Length > 45)
                {
                    PadInspectMethod = (PadInspectMethodEnum)int.Parse(strs[44]);
                    PADINSPECTOPString = strs[45];
                }
                if (strs.Length > 46)
                {
                    if (!string.IsNullOrEmpty(strs[46]))
                        PADChipInBlobGrayThreshold = int.Parse(strs[46]);
                }
                if (strs.Length > 48)
                {
                    if (!string.IsNullOrEmpty(strs[47]))
                        ChipFindWhite = strs[47] == "1";
                    if (!string.IsNullOrEmpty(strs[48]))
                        GLEFindWhite = strs[48] == "1";
                }
                if (strs.Length > 49)
                {
                    bool bOK = int.TryParse(strs[49], out int iresult);
                    if (bOK)
                        FourSideNoGluePassValue = iresult;
                }
                if (strs.Length > 50)
                {
                    if (!string.IsNullOrEmpty(strs[50]))
                        GlueChipSlotDir = (ChipSlotDir)int.Parse(strs[50]);
                }
                if (strs.Length > 52)
                {
                    if (!string.IsNullOrEmpty(strs[51]))
                        GlueTopBottomOffset = double.Parse(strs[51]);
                    if (!string.IsNullOrEmpty(strs[52]))
                        GlueLeftRightOffset = double.Parse(strs[52]);
                }
                if (strs.Length > 54)
                {
                    if (string.IsNullOrEmpty(strs[53]))
                        strs[53] = "0";
                    ChipNoGlueMode = (ChipNoGlueMethod)int.Parse(strs[53]);
                    if (string.IsNullOrEmpty(strs[54]))
                        strs[54] = "0";
                    ChipNoHaveMode = (ChipNoHave)int.Parse(strs[54]);
                }
                if (strs.Length > 55)
                {
                    ChipNoHaveModeOpString = strs[55];
                }
                if (strs.Length > 56)
                {
                    PADExtendOPString = strs[56];
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
            PADChipInBlobGrayThreshold = 100;
            CheckDArea = 15;
            CheckDWidth = 15;
            CheckDHeight = 15;
            ExtendX = 5;
            ExtendY = 5;
            GlueMax = 1;
            GlueMin = 0.6;
            GlueCheck = true;
            PADThresholdMode = PADThresholdEnum.Threshold;
            NoGlueThresholdValue = 0.7;
            PADCalMode = PADCalModeEnum.BlacktoBlack;
            PADChipSizeMode = PADChipSize.CHIP_NORMAL;
            CalExtendX = 66;
            CalExtendY = 66;
            BloodFillValueRatio = 0.33;
            PADAICategory = AICategory.Baseline;
            GlueChipSlotDir = ChipSlotDir.NONE;
            GlueMaxTop = 0.6;
            GlueMinTop = 0.1;

            GlueTopBottomOffset = 0.1;
            GlueLeftRightOffset = 0.1;

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

            ChipDirlevel = true;
            ChipGleCheck = false;

            FontSize = 35;
            LineWidth = 5;

            GleWidthUpper = 10;
            GleWidthLower = 0;

            GleHeightUpper = 10;
            GleHeightLower = 0;

            GleAreaUpper = 10;
            GleAreaLower = 0;

            PadInspectMethod = PadInspectMethodEnum.NONE;
            PADINSPECTOPString = "";


            ChipFindWhite = true;
            GLEFindWhite = false;
            FourSideNoGluePassValue = 0;

            ChipNoGlueMode = ChipNoGlueMethod.NONE;
            ChipNoHaveMode = ChipNoHave.NONE;
            ChipNoHaveModeOpString = "";
            PADExtendOPString = "";

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
                case "PADChipInBlobGrayThreshold":
                    PADChipInBlobGrayThreshold = int.Parse(valuestring);
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
                case "ChipNoGlueMode":
                    ChipNoGlueMode = (ChipNoGlueMethod)Enum.Parse(typeof(ChipNoGlueMethod), valuestring, true);
                    break;
                case "ChipNoHaveMode":
                    ChipNoHaveMode = (ChipNoHave)Enum.Parse(typeof(ChipNoHave), valuestring, true);
                    break;
                case "GlueChipSlotDir":
                    GlueChipSlotDir = (ChipSlotDir)Enum.Parse(typeof(ChipSlotDir), valuestring, true);
                    break;
                case "PADChipSizeMode":
                    PADChipSizeMode = (PADChipSize)Enum.Parse(typeof(PADChipSize), valuestring, true);
                    break;
                case "PADAICategory":
                    PADAICategory = (AICategory)Enum.Parse(typeof(AICategory), valuestring, true);
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

                case "GlueTopBottomOffset":
                    GlueTopBottomOffset = double.Parse(valuestring);
                    break;
                case "GlueLeftRightOffset":
                    GlueLeftRightOffset = double.Parse(valuestring);
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
                case "ChipDirLevel":
                    ChipDirlevel = bool.Parse(valuestring);
                    break;
                case "FontSize":
                    FontSize = int.Parse(valuestring);
                    break;
                case "LineWidth":
                    LineWidth = int.Parse(valuestring);
                    break;
                case "ChipGleCheck":
                    ChipGleCheck = bool.Parse(valuestring);
                    break;
                case "ChipFindWhite":
                    ChipFindWhite = bool.Parse(valuestring);
                    break;
                case "GLEFindWhite":
                    GLEFindWhite = bool.Parse(valuestring);
                    break;
                case "FourSideNoGluePassValue":
                    FourSideNoGluePassValue = int.Parse(valuestring);
                    break;

                case "GleWidthUpper":
                    GleWidthUpper = double.Parse(valuestring);
                    break;
                case "GleWidthLower":
                    GleWidthLower = double.Parse(valuestring);
                    break;
                case "GleHeightUpper":
                    GleHeightUpper = double.Parse(valuestring);
                    break;
                case "GleHeightLower":
                    GleHeightLower = double.Parse(valuestring);
                    break;
                case "GleAreaUpper":
                    GleAreaUpper = double.Parse(valuestring);
                    break;
                case "GleAreaLower":
                    GleAreaLower = double.Parse(valuestring);
                    break;
                case "PADINSPECTOPString":
                    PADINSPECTOPString = valuestring.Split('#')[1];
                    PadInspectMethod = (PadInspectMethodEnum)Enum.Parse(typeof(PadInspectMethodEnum), valuestring.Split('#')[0], true);
                    break;
                case "ChipNoHaveModeOpString":
                    ChipNoHaveModeOpString = valuestring.Split('#')[1];
                    ChipNoHaveMode = (ChipNoHave)Enum.Parse(typeof(ChipNoHave), valuestring.Split('#')[0], true);
                    break;
                case "PADExtendOPString":
                    PADExtendOPString = valuestring;
                    break;
            }
        }

        public void Suicide()
        {
            if (PADMethod == PADMethodEnum.PADCHECK
                || PADMethod == PADMethodEnum.GLUECHECK
                || PADMethod == PADMethodEnum.GLUECHECK_BlackEdge
                || PADMethod == PADMethodEnum.PLACODE_CHECK
                               || PADMethod == PADMethodEnum.CHIPCHECKNOHAVE
                || PADMethod == PADMethodEnum.QLE_CHECK)
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

            RunDataOK = false;//复位数据
            //DescStr = "";
            DescStr = "无芯片";
            QLERunDataStr = string.Empty;

            bmpMeasureOutput.Dispose();
            bmpMeasureOutput = new Bitmap(1, 1);
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
            Bitmap bmp000 = new Bitmap(imginput);

            Graphics gPrepared = Graphics.FromImage(bmp000);
            PointF[] xxx = m_PADRegion.GetConner(5);
            gPrepared.FillPolygon(Brushes.Black, xxx);
            gPrepared.Dispose();

            //if (m_IsSaveTemp)
            //{
            //    bmp000.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "imginput1" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
            //}

            //抽取绿色
            ExtractChannel extract = new ExtractChannel();
            extract.Channel = 1;
            bmp000 = extract.Apply(bmp000);

            AForge.Imaging.Filters.ContrastStretch contrastStretch = new AForge.Imaging.Filters.ContrastStretch();
            bmp000 = contrastStretch.Apply(bmp000);

            Blur blur = new Blur();
            bmp000 = blur.Apply(bmp000);

            AForge.Imaging.Filters.SISThreshold sISThreshold = new AForge.Imaging.Filters.SISThreshold();
            bmp000 = sISThreshold.Apply(bmp000);

            Invert invert = new Invert();
            bmp000 = invert.Apply(bmp000);

            BlobsFiltering blobsFiltering = new BlobsFiltering();
            blobsFiltering.MinWidth = bmp000.Width / 3;
            blobsFiltering.MinHeight = bmp000.Height / 3;
            blobsFiltering.CoupledSizeFiltering = true;
            bmp000 = blobsFiltering.Apply(bmp000);


            //Bitmap bmp001 = new Bitmap(bmp000);
            //gPrepared = Graphics.FromImage(bmp001);
            //xxx = m_PADRegion.GetConner(0);
            //gPrepared.FillPolygon(Brushes.Black, xxx);
            //gPrepared.Dispose();

            //bmp000 = invert.Apply(bmp000);

            //if (m_IsSaveTemp)
            //{
            //    bmp001.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "imginput" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
            //}
            return bmp000;
        }
        private Bitmap _getV1_1bmpInput(Rectangle rect1)
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

        object obj = new object();
        public IEzSeg model
        {
            get { return Universal.model; }
        }
        private int _from_bmpinputSize_to_iSized(Bitmap bmpInputX)
        {
            int isized = 0;
            if (bmpInputX == null)
                return isized;
            int minSize = Math.Min(bmpInputX.Size.Width, bmpInputX.Size.Height);
            if (minSize >= 2000)
                isized = -3;
            else if (minSize >= 1500)
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

            switch (PADAICategory)
            {
                case AICategory.Median:
                case AICategory.Small:
                    isized = 0;
                    break;
                default:
                    //case AICategory.BigKotor:
                    break;
            }

            Bitmap img = new Bitmap(imginput, Resize(imginput.Size, isized));
            Bitmap bbtemp24 = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);

            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            bbtemp24 = grayscale.Apply(bbtemp24);
            if (m_IsSaveTemp)
                bbtemp24.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpfloodfill_AiPre" + ".png",
                                   System.Drawing.Imaging.ImageFormat.Png);

            //if (m_IsSaveTemp)
            //{
            //    img.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "imgPredict_Pre" + ".png",
            //        System.Drawing.Imaging.ImageFormat.Png);
            //}
            Bitmap mask = bbtemp24;
            //lock (obj)
            {
                var modeCx = ModelCategory.Baseline;
                switch (PADAICategory)
                {
                    case AICategory.Median:
                        modeCx = ModelCategory.Medium;
                        m_Model = EzSegClientFactory.OpenConnection(INI.AI_IP, INI.AI_Port + 1);
                        m_Model.SwitchModel(modeCx);
                        mask = m_Model.Predict(bbtemp24);
                        break;
                    case AICategory.Small:
                        modeCx = ModelCategory.Small;
                        m_Model = EzSegClientFactory.OpenConnection(INI.AI_IP, INI.AI_Port + 2);
                        m_Model.SwitchModel(modeCx);
                        mask = m_Model.Predict(bbtemp24);
                        break;
                    case AICategory.BigKotor:
                        modeCx = ModelCategory.BigKotor;
                        m_Model = EzSegClientFactory.OpenConnection(INI.AI_IP, INI.AI_Port + 3);
                        m_Model.SwitchModel(modeCx);
                        mask = m_Model.Predict(bbtemp24);
                        break;
                    default:
                        var err = model.SwitchModel(modeCx);
                        // 預測 (單張)
                        mask = model.Predict(bbtemp24);
                        break;
                }

                if (m_Model != null)
                {
                    //_TRACE("關閉連線中...");
                    m_Model.Dispose();
                }
            }
            //if (!err.Is(Errcode.OK))
            //{
            //    MessageBox.Show($"异常:({err.errCode},{err.errMsg})", "AI Init", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}

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
        private Bitmap blackAICal(PADRegionClass pADRegionClass, int iSized = 2)
        {
            if (!INI.chipUseAI)
                return blackNormal(pADRegionClass);
            int isized = 0;// iSized;

            switch (PADAICategory)
            {
                case AICategory.Median:
                case AICategory.Small:
                    //case AICategory.BigKotor:
                    break;
                default:
                    isized = iSized;
                    break;
            }
            Bitmap img = new Bitmap(imginput, Resize(imginput.Size, -isized));
            //new Size(imginput.Width / isized, imginput.Height / isized));


            PointF[] points = new PointF[pADRegionClass.RegionPtFCornerORG.Length];
            points[0] = ResizeWithLocation2(pADRegionClass.RegionPtFCornerORG[0], -isized);
            points[1] = ResizeWithLocation2(pADRegionClass.RegionPtFCornerORG[1], -isized);
            points[2] = ResizeWithLocation2(pADRegionClass.RegionPtFCornerORG[2], -isized);
            points[3] = ResizeWithLocation2(pADRegionClass.RegionPtFCornerORG[3], -isized);


            switch (PADAICategory)
            {
                case AICategory.Median:
                case AICategory.Small:
                case AICategory.BigKotor:
                    break;
                default:
                    Graphics gPrepared = Graphics.FromImage(img);
                    gPrepared.FillPolygon(Brushes.White, points);
                    gPrepared.Dispose();
                    break;
            }



            //if (m_IsSaveTemp)
            //    img.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpfloodfillimg" + ".png",
            //                       System.Drawing.Imaging.ImageFormat.Png);

            Bitmap bbtemp24 = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);

            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            bbtemp24 = grayscale.Apply(bbtemp24);
            if (m_IsSaveTemp)
                bbtemp24.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpfloodfill_AiPre" + ".png",
                                   System.Drawing.Imaging.ImageFormat.Png);
            Bitmap mask = bbtemp24;
            //lock (obj)
            {
                var modeCx = ModelCategory.Baseline;
                switch (PADAICategory)
                {
                    case AICategory.Median:
                        modeCx = ModelCategory.Medium;
                        m_Model = EzSegClientFactory.OpenConnection(INI.AI_IP, INI.AI_Port + 1);
                        m_Model.SwitchModel(modeCx);
                        mask = m_Model.Predict(bbtemp24);
                        break;
                    case AICategory.Small:
                        modeCx = ModelCategory.Small;
                        m_Model = EzSegClientFactory.OpenConnection(INI.AI_IP, INI.AI_Port + 2);
                        m_Model.SwitchModel(modeCx);
                        mask = m_Model.Predict(bbtemp24);
                        break;
                    case AICategory.BigKotor:
                        modeCx = ModelCategory.BigKotor;
                        m_Model = EzSegClientFactory.OpenConnection(INI.AI_IP, INI.AI_Port + 3);
                        m_Model.SwitchModel(modeCx);
                        mask = m_Model.Predict(bbtemp24);
                        break;
                    default:
                        var err = model.SwitchModel(modeCx);

                        // 預測 (單張)
                        mask = model.Predict(bbtemp24);
                        break;
                }

                if (m_Model != null)
                {
                    //_TRACE("關閉連線中...");
                    m_Model.Dispose();
                }
            }
            if (m_IsSaveTemp)
            {
                mask.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpfloodfill_Ai" + ".png",
                                   System.Drawing.Imaging.ImageFormat.Png);
            }
            return mask;
        }
        private Bitmap blackNormal(PADRegionClass pADRegionClass, int iSized = 5)
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

            #region 增加准确找到胶水位置 进行油漆桶填充

            Rectangle blackRegion = new Rectangle(point.X - 5, 0, 10, point.Y - 1);
            //截取一小块找blob
            Bitmap bmpCheckBlackRegion = (Bitmap)img.Clone(blackRegion, img.PixelFormat);

            //if (m_IsSaveTemp)
            //{
            //    bmpCheckBlackRegion.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpCheckBlackRegion0" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
            //}

            m_JzFind.SetThreshold(bmpCheckBlackRegion, 0, histogramClass.MinGrade * 2, 0);

            //if (m_IsSaveTemp)
            //{
            //    bmpCheckBlackRegion.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpCheckBlackRegion1" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
            //}

            m_JzFind.AH_FindBlob(bmpCheckBlackRegion, false);
            //m_JzFind.SortByArea();
            if (m_JzFind.FoundList.Count > 0)
            {
                Point point1 = GetRectCenter(m_JzFind.rectMaxRect);
                point = new Point(point1.X + blackRegion.X, point1.Y + blackRegion.Y);
            }

            bmpCheckBlackRegion.Dispose();
            #endregion


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


        private Bitmap _getNormalEx1(Rectangle rect1)
        {
            Graphics gPrepared = Graphics.FromImage(imgmask);
            gPrepared.Clear(Color.White);
            //gPrepared.FillPolygon(Brushes.Black, PADTempRegion.RegionPtFCornerORG);
            gPrepared.FillRectangle(Brushes.Black, rect1);
            gPrepared.Dispose();

            Bitmap _bmpNormal = imginput.Clone(new Rectangle(0, 0, imginput.Width, imginput.Height), PixelFormat.Format24bppRgb);

            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            _bmpNormal = grayscale.Apply(_bmpNormal);
            AForge.Imaging.Filters.Threshold threshold = new AForge.Imaging.Filters.Threshold((int)(255 * BloodFillValueRatio));
            _bmpNormal = threshold.Apply(_bmpNormal);
            AForge.Imaging.Filters.Invert invert = new AForge.Imaging.Filters.Invert();
            _bmpNormal = invert.Apply(_bmpNormal);
            if (m_IsSaveTemp)
            {
                _bmpNormal.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpff" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                imgmask.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpffmask" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }
            return _bmpNormal;
        }
        private Bitmap _getNormalPADG1(Rectangle eRectORG)
        {

            Bitmap _bmpNormal = new Bitmap(imginput);

            PADG1Class ePADG1 = new PADG1Class();
            ePADG1.FromString(PADINSPECTOPString);
            //原始的尺寸 扩大 之后的限流外围
            Rectangle m_rect_last = new Rectangle(eRectORG.X, eRectORG.Y, eRectORG.Width, eRectORG.Height);
            m_rect_last.Inflate(ePADG1.FindX, ePADG1.FindY);

            Rectangle m_rect_before = new Rectangle(eRectORG.X, eRectORG.Y, eRectORG.Width, eRectORG.Height);
            m_rect_before.Inflate(-ePADG1.FindInX, -ePADG1.FindInY);

            Graphics gPrepared = Graphics.FromImage(_bmpNormal);
            //gPrepared.Clear(Color.White);
            //gPrepared.FillPolygon(Brushes.Black, PADTempRegion.RegionPtFCornerORG);
            //gPrepared.FillRectangle(Brushes.Black, m_rect_before);
            gPrepared.DrawRectangle(new Pen(Color.Blue, 3f), m_rect_before);
            gPrepared.DrawRectangle(new Pen(Color.Red, 3f), m_rect_last);
            gPrepared.Dispose();

            if (m_IsSaveTemp)
            {
                _bmpNormal.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpff" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }
            return _bmpNormal;
        }

        private Bitmap _getG1bmpInput(Rectangle rect1)
        {
            //rect1.Inflate(2, 2);

            //Bitmap bmpRect0 = new Bitmap(imginput);
            //Graphics gPrepared = Graphics.FromImage(bmpRect0);
            //gPrepared.DrawRectangle(new Pen(Color.Lime, 3), rect1);
            ////gPrepared.Clear(Color.White);
            ////gPrepared.FillPolygon(Brushes.Black, PADTempRegion.RegionPtFCornerORG);
            ////gPrepared.FillRectangle(Brushes.Black, rect1);
            //gPrepared.Dispose();
            //if (m_IsSaveTemp)
            //{
            //    bmpRect0.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpRect0" + ".png", System.Drawing.Imaging.ImageFormat.Png);
            //    //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
            //}
            //bmpRect0.Dispose();


            //Bitmap bmpfloodfill = (Bitmap)imginput.Clone();// new Bitmap(imginput);
            Bitmap bmphistogram = new Bitmap(imginput, Resize(imginput.Size, -5));
            HistogramClass histogramClass = new HistogramClass(2);
            histogramClass.GetHistogram(bmphistogram);
            bmphistogram.Dispose();
            //int fillthresholdvalue = (int)(255 * (1 - NoGlueThresholdValue));
            //fillthresholdvalue = histogramClass.MinGrade;
            //m_JzFind.GetMaskedImage(imginput, imgmask, Color.White, Color.FromArgb(fillthresholdvalue, fillthresholdvalue, fillthresholdvalue), false);

            Point point = new Point((rect1.Right - rect1.X) / 2, (rect1.Bottom - rect1.Y) / 2);
            point = new Point((rect1.Right - rect1.X) / 2, rect1.Y - 5);

            #region 增加准确找到胶水位置 进行油漆桶填充

            Rectangle blackRegion = new Rectangle(point.X - 10, 0, 20, rect1.Y - 5);
            //截取一小块找blob
            Bitmap bmpCheckBlackRegion = (Bitmap)imginput.Clone(blackRegion, imginput.PixelFormat);
            m_JzFind.SetThreshold(bmpCheckBlackRegion, 0, histogramClass.MinGrade, 0);

            if (m_IsSaveTemp)
            {
                bmpCheckBlackRegion.Save("D:\\testtest\\" + _CalPageIndex() + RelateAnalyzeString + "bmpCheckBlackRegion" + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //bmpMask.Save("D:\\testtest\\imgpattern.png", System.Drawing.Imaging.ImageFormat.Png);
            }

            m_JzFind.AH_FindBlob(bmpCheckBlackRegion, false);
            //m_JzFind.SortByArea();
            if (m_JzFind.FoundList.Count > 0)
            {
                Point point1 = GetRectCenter(m_JzFind.rectMaxRect);
                point = new Point(point1.X + blackRegion.X, point1.Y + blackRegion.Y);
            }

            bmpCheckBlackRegion.Dispose();
            #endregion


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
        public bool IsInRangeEx(double FromValue, double MaxValue, double MinValue)
        {
            return (FromValue > MinValue) && (FromValue <= MaxValue);
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
        public PointF GetCenterPoint(PointF P1, PointF P2)
        {
            return new PointF((P1.X + P2.X) / 2, (P1.Y + P2.Y) / 2);
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

            //if (Ratio > 0)
            //    retSize = new Size(OrgSize.Width * Ratio, OrgSize.Height * Ratio);
            //else
            //    retSize = new Size(OrgSize.Width / -Ratio, OrgSize.Height / -Ratio);

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

            //if (ratio > 0)
            //{
            //    retPtF = new PointF(rect.X * ratio, rect.Y * ratio);
            //    retSize = new Size(rect.Width * ratio, rect.Height * ratio);
            //}
            //else
            //{
            //    retPtF = new PointF(rect.X / -ratio, rect.Y / -ratio);
            //    retSize = new Size(rect.Width / -ratio, rect.Height / -ratio);
            //}

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
                jRotatedRectangleF.fCX = rect.fCX / -ratio;
                jRotatedRectangleF.fCY = rect.fCY / -ratio;
                jRotatedRectangleF.fWidth = rect.fWidth / -ratio;
                jRotatedRectangleF.fHeight = rect.fHeight / -ratio;
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

            //if (ratio > 0)
            //{
            //    retPtF = new PointF(epoint.X * ratio, epoint.Y * ratio);
            //    //retSize = new Size(rect.Width << ratio, rect.Height << ratio);
            //}
            //else
            //{
            //    retPtF = new PointF(epoint.X / -ratio, epoint.Y / -ratio);
            //    //retSize = new Size(rect.Width >> -ratio, rect.Height >> -ratio);
            //}

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

        public double GetP1P2Angle(PointF p1, PointF p2)
        {
            double angleOfLine = Math.Atan2((p2.Y - p1.Y), (p2.X - p1.X)) * 180 / Math.PI;
            return angleOfLine;
        }

        public void BoundRect(ref Rectangle InnerRect, Size BoundSize)
        {
            InnerRect.X = Math.Min(Math.Max(InnerRect.X, 0), (BoundSize.Width - InnerRect.Width < 0 ? 0 : BoundSize.Width - InnerRect.Width));
            InnerRect.Y = Math.Min(Math.Max(InnerRect.Y, 0), (BoundSize.Height - InnerRect.Height < 0 ? 0 : BoundSize.Height - InnerRect.Height));

            if (BoundSize.Width <= InnerRect.X + InnerRect.Width)
                InnerRect.Width = BoundValue(InnerRect.Width, BoundSize.Width - InnerRect.X, 1);
            if (BoundSize.Height <= InnerRect.Height + InnerRect.Height)
                InnerRect.Height = BoundValue(InnerRect.Height, BoundSize.Height - InnerRect.Y, 1);
        }
        public int BoundValue(int Value, int Max, int Min)
        {
            return Math.Max(Math.Min(Value, Max), Min);

        }
        public void BoundRect(ref RectangleF InnerRect, Size BoundSize)
        {
            InnerRect.X = Math.Min(Math.Max(InnerRect.X, 0), (BoundSize.Width - InnerRect.Width < 0 ? 0 : BoundSize.Width - InnerRect.Width));
            InnerRect.Y = Math.Min(Math.Max(InnerRect.Y, 0), (BoundSize.Height - InnerRect.Height < 0 ? 0 : BoundSize.Height - InnerRect.Height));

            if (BoundSize.Width <= InnerRect.X + InnerRect.Width)
                InnerRect.Width = BoundValue(InnerRect.Width, BoundSize.Width - InnerRect.X, 1);
            if (BoundSize.Height <= InnerRect.Height + InnerRect.Height)
                InnerRect.Height = BoundValue(InnerRect.Height, BoundSize.Height - InnerRect.Y, 1);
        }
        public float BoundValue(float Value, float Max, float Min)
        {
            return Math.Max(Math.Min(Value, Max), Min);

        }
        #endregion

        #region 排序四边型的角点


        public List<PointF> SortCornersClockwise(List<PointF> corners)
        {
            List<PointF> result = new List<PointF>();
            //从小到大排序
            corners.Sort((item1, item2) => { return item1.X >= item2.X ? 1 : -1; });
            //foreach (PointF item in corners)
            //{
            //    result.Add(item);
            //}

            if (corners[0].Y >= corners[1].Y)
            {
                result.Add(corners[0]);
                result.Add(corners[1]);
            }
            else
            {
                result.Add(corners[1]);
                result.Add(corners[0]);
            }

            if (corners[2].Y >= corners[3].Y)
            {
                result.Add(corners[2]);
                result.Add(corners[3]);
            }
            else
            {
                result.Add(corners[3]);
                result.Add(corners[2]);
            }

            //// 假设四边形的边是对角线，可以通过对角线的起点和终点来确定方向
            //var topLeft = corners.OrderBy(p => p.X).ThenBy(p => p.Y).First();
            //var bottomRight = corners.OrderByDescending(p => p.X).ThenByDescending(p => p.Y).First();

            //// 确定对角线的方向
            //var line = new Line(topLeft, bottomRight);

            //// 根据对角线方向和Y坐标排序
            //corners.Sort((p1, p2) =>
            //{
            //    var relativePos1 = line.GetRelativePosition(p1);
            //    var relativePos2 = line.GetRelativePosition(p2);

            //    if (relativePos1 == relativePos2)
            //    {
            //        return p1.Y.CompareTo(p2.Y);
            //    }
            //    return relativePos1.CompareTo(relativePos2);
            //});

            return result;
        }

        public class Line
        {
            public PointF Start { get; }
            public PointF End { get; }

            public Line(PointF start, PointF end)
            {
                Start = start;
                End = end;
            }

            public double Slope => (double)(End.Y - Start.Y) / (End.X - Start.X);

            public int GetRelativePosition(PointF point)
            {
                if (point.X < Start.X)
                {
                    return -1;
                }
                else if (point.X > End.X)
                {
                    return 1;
                }
                else
                {
                    if (Slope == 0)
                    {
                        return point.Y < Start.Y ? -1 : 1;
                    }
                    else
                    {
                        var yIntercept = Start.Y - Slope * Start.X;
                        var relativeY = point.Y - Slope * point.X;
                        return relativeY < yIntercept ? -1 : 1;
                    }
                }
            }
        }

        #endregion

        #region 来自Victor的小颗粒算法
        public JzFourSideAnalyze sideAnalyze = new JzFourSideAnalyze();

        double mGammaCorrelation = 0.8;
        int mBlurCount = 3;
        double mholesratio = 20;
        bool isneedclose = false;
        int mleft = 2;
        int mright = 2;
        int mtop = 2;
        int mbottom = 2;
        double mRangeRatio = 0.6;

        public void GetDataHX(Bitmap bmp)
        {
            //IsNeedChange = false;

            //pbx1.Image = null;
            //pbx2.Image = null;

            if (!string.IsNullOrEmpty(PADINSPECTOPString))
            {
                //string[] strings = PADINSPECTOPString.Split('#');
                //if (strings.Length >= 2)
                {
                    string[] strs = PADINSPECTOPString.Split(',');
                    if (strs.Length > 8)
                    {
                        mGammaCorrelation = double.Parse(strs[0]);
                        mBlurCount = int.Parse(strs[1]);
                        mholesratio = double.Parse(strs[2]);
                        mRangeRatio = double.Parse(strs[3]);
                        mleft = int.Parse(strs[4]);
                        mright = int.Parse(strs[5]);
                        mtop = int.Parse(strs[6]);
                        mbottom = int.Parse(strs[7]);
                        isneedclose = strs[8] == "1";
                    }
                }
            }


            sideAnalyze.Suiccide();
            sideAnalyze.GetDataGX(bmp, mGammaCorrelation,
               mBlurCount, mholesratio, isneedclose,
                null);


            int[] sidecount = new int[(int)SIDEEmnum.COUNT];

            sidecount[(int)SIDEEmnum.LEFT] = mleft;
            sidecount[(int)SIDEEmnum.RIGHT] = mright;
            sidecount[(int)SIDEEmnum.TOP] = mtop;
            sidecount[(int)SIDEEmnum.BOTTOM] = mbottom;

            sideAnalyze.GetBoarder(bmp, mRangeRatio, sidecount);

            sideAnalyze.DrawProcessedSides();


            //IsNeedChange = true;

            //pbx1.Image = sideAnalyze.bmpOrg;
            //pbx2.Image = sideAnalyze.bmpFirst;
            //pbx22.Image = sideAnalyze.bmpSecond;
        }

        public JzFourSideAnalyze sideAnalyzeIN = new JzFourSideAnalyze();
        public void GetDataHXIn(Bitmap bmp)
        {
            //IsNeedChange = false;

            //pbx1.Image = null;
            //pbx2.Image = null;

            if (!string.IsNullOrEmpty(PADINSPECTOPString))
            {
                //string[] strings = PADINSPECTOPString.Split('#');
                //if (strings.Length >= 2)
                {
                    string[] strs = PADINSPECTOPString.Split(',');
                    if (strs.Length > 8)
                    {
                        mGammaCorrelation = double.Parse(strs[0]);
                        mBlurCount = int.Parse(strs[1]);
                        mholesratio = double.Parse(strs[2]);
                        mRangeRatio = double.Parse(strs[3]);
                        mleft = 1;
                        mright = 1;
                        mtop = 1;
                        mbottom = 1;
                        isneedclose = strs[8] == "1";
                    }
                }
            }

            sideAnalyzeIN.Suiccide();
            sideAnalyzeIN.GetDataGX(bmp, mGammaCorrelation,
               mBlurCount, mholesratio, isneedclose,
                null);


            int[] sidecount = new int[(int)SIDEEmnum.COUNT];

            sidecount[(int)SIDEEmnum.LEFT] = mleft;
            sidecount[(int)SIDEEmnum.RIGHT] = mright;
            sidecount[(int)SIDEEmnum.TOP] = mtop;
            sidecount[(int)SIDEEmnum.BOTTOM] = mbottom;

            sideAnalyzeIN.GetBoarder(bmp, mRangeRatio, sidecount);

            sideAnalyzeIN.DrawProcessedSides();


            //IsNeedChange = true;

            //pbx1.Image = sideAnalyze.bmpOrg;
            //pbx2.Image = sideAnalyze.bmpFirst;
            //pbx22.Image = sideAnalyze.bmpSecond;
        }
        #endregion

        #region IPD_V1

        short GetRGB = 1;
        int numThresholdRatio = 10;
        int numObjectFilterRatio = 25;
        int numEDCount = 3;
        int numShortenRatio = 50;
        string txtBangBangRectStr = "";

        int numBangBangOffsetVal = 5;
        string txtNeverOutsideRect = "";
        int cboIPDMethod = 0;

       public JzSideAnalyzeEXClass jzsideanalyzeex = new JzSideAnalyzeEXClass();
        public List<Bitmap> bmplist = new List<Bitmap>();

        public string GetDataIPD(Bitmap bmp, bool istrain)
        {
            string passStr = "PASS";

            //IsNeedChange = false;

            //pbx1.Image = null;
            //pbx2.Image = null;

            //List<Bitmap> bmplist = new List<Bitmap>();

            if (!string.IsNullOrEmpty(PADINSPECTOPString))
            {
                string[] strs = PADINSPECTOPString.Split(',');
                if (strs.Length > 5)
                {
                    GetRGB = short.Parse(strs[0]);
                    numThresholdRatio = int.Parse(strs[1]);
                    numObjectFilterRatio = int.Parse(strs[2]);
                    numEDCount = int.Parse(strs[3]);
                    numShortenRatio = int.Parse(strs[4]);
                    txtBangBangRectStr = strs[5];
                }
                if (strs.Length > 8)
                {
                    numBangBangOffsetVal = int.Parse(strs[6]);
                    txtNeverOutsideRect = strs[7];
                    cboIPDMethod = int.Parse(strs[8]);
                }
            }


            JzAnalyzeParaEXClass jzpara = new JzAnalyzeParaEXClass();

            jzpara.mymethod = (MethodEnum)cboIPDMethod;
            jzpara.rgb = GetRGB;
            jzpara.bangbangoffsetval = numBangBangOffsetVal;
            jzpara.edcount = numEDCount;
            jzpara.shrinkratio = numShortenRatio;
            jzpara.objfilterratio = numObjectFilterRatio;
            jzpara.threshodratio = numThresholdRatio;

            jzpara.rectbangbang = StringtoRect(txtBangBangRectStr);
            jzpara.rectneveroutrange = StringtoRect(txtNeverOutsideRect);

            //JzSideAnalyzeEXClass jzsideanalyzeex1 = new JzSideAnalyzeEXClass();

            Clearbmplist(bmplist);
            if (istrain)
                jzsideanalyzeex.Train(jzpara, bmp, bmplist);

            Stopwatch stopwatch = Stopwatch.StartNew();
            stopwatch.Restart();

            passStr = jzsideanalyzeex.Run(bmp, bmplist);

            stopwatch.Stop();
            long ms = stopwatch.ElapsedMilliseconds;

            int a = 100;

            //sideAnalyze.Suiccide();
            //sideAnalyze.GetIPDDietBMP(bmp,
            //    GetRGB,
            //    (double)numThresholdRatio / 100d,
            //    (double)numObjectFilterRatio / 100d,
            //    (int)numEDCount,
            //    (double)numShortenRatio / 100d,
            //    txtBangBangRectStr);

            //sideAnalyze .g


            //int[] sidecount = new int[(int)SIDEEmnum.COUNT];

            //sidecount[(int)SIDEEmnum.LEFT] = (int)numleft.Value;
            //sidecount[(int)SIDEEmnum.RIGHT] = (int)numright.Value;
            //sidecount[(int)SIDEEmnum.TOP] = (int)numtop.Value;
            //sidecount[(int)SIDEEmnum.BOTTOM] = (int)numbottom.Value;

            //sideAnalyze.Suiccide();
            //sideAnalyze.GetDataIX(bmp, (double)numGammaCorrelation.Value,
            //    (int)numBlurCount.Value, (double)numholesratio.Value, chkisneedclose.Checked,
            //    cboResult, sidecount,
            //    (double)numLongRatio.Value, (double)numShortRatio.Value, (int)numEnlarge.Value,
            //    (byte)numInsideColor.Value);

            ////sideAnalyze.GetBoarderIX(bmp, (double)numRangeRatio.Value,(double)numRangeRatio.Value, sidecount, 20);

            //sideAnalyze.DrawProcessedSides(Color.Red, 10);


            //IsNeedChange = true;

            //pbx1.Image = sideAnalyze.bmpOrg;
            //pbx2.Image = sideAnalyze.bmpSecond;

            //txtIPDresult.Text = sideAnalyze.allresultstr;

            //lblIPDMeanValue.Text = sideAnalyze.meanstr;
            //lblIPDThreshold.Text = sideAnalyze.thresholdstr;

            //lblIPDPassNG.Text = sideAnalyze.PASSNG;

            //if (sideAnalyze.PASSNG == "PASS")
            //{
            //    lblIPDPassNG.BackColor = Color.Lime;
            //}
            //else
            //{
            //    lblIPDPassNG.BackColor = Color.Red;
            //}

            //pbx22.Image = sideAnalyze.bmpSecond;

            return passStr;
        }

        public void Clearbmplist(List<Bitmap> bmplist)
        {
            int i = bmplist.Count - 1;

            while (i > -1)
            {
                if (bmplist[i] != null)
                    bmplist[i].Dispose();

                bmplist.RemoveAt(i);

                i--;
            }

        }

        Rectangle StringtoRect(string RectStr)
        {
            string[] str = RectStr.Split(';');
            return new Rectangle(int.Parse(str[0]), int.Parse(str[1]), int.Parse(str[2]), int.Parse(str[3]));
        }

        #endregion

    }
}
