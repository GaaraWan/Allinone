//#define COGNEX
#define AUVISION

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using JetEazy;
using System.Drawing;
#if(AUVISION)
using AUVision;
#endif
#if(COGNEX)
using Cognex.VisionPro;
using Cognex.VisionPro.PMAlign;
using Cognex.VisionPro.CalibFix;
using Cognex.VisionPro.ImageProcessing;
#endif

namespace JzOCR.OPSpace
{
    /// <summary>
    /// OCR参数
    /// </summary>
    [Serializable]
    public class OCRItemClass
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int No = 0;
        /// <summary>
        /// 关联字符
        /// </summary>
        public string strRelateName = "";
        /// <summary>
        /// 将XX强制转换的字符
        /// </summary>
        public string strRelateName2 = "";
        /// <summary>
        /// OCR源图
        /// </summary>
        public Bitmap bmpItem = new Bitmap(1, 1);
        /// <summary>
        /// OCR源图
        /// </summary>
        public Bitmap bmpItemTo = new Bitmap(1, 1);
        /// <summary>
        /// OCR源图
        /// </summary>
        public Bitmap bmpTrain = new Bitmap(1, 1);
        /// <summary>
        /// OCRFind
        /// </summary>
        public Bitmap bmpFind = new Bitmap(1, 1);
        /// <summary>
        /// 差异图
        /// </summary>
        public Bitmap bmpDifference;
        /// <summary>
        /// 比对分数
        /// </summary>
        public float fScore;
        /// <summary>
        /// OCR所在位置
        /// </summary>
        public Rectangle rect;
        /// <summary>
        /// 背景色
        /// </summary>
        public int iBackColor;
        /// <summary>
        /// 前景亮度
        /// </summary>
        public int iFousColor = 0;
        /// <summary>
        /// 参数是否被选中
        /// </summary>
        public bool isSelected = false;
        /// <summary>
        /// 缺失点数
        /// </summary>
        public int iPoint;
        /// <summary>
        /// 缺失最大面积
        /// </summary>
        public int iArea;
        /// <summary>
        /// 缺失是否为良品
        /// </summary>
        public bool isDefect;
        /// <summary>
        /// 使用源图来比较
        /// </summary>
        public bool isTiemTest = false;

#if(AUVISION)
        public xFindResult xResult;
#endif
#if(COGNEX)
        public Cognex.VisionPro.PMAlign.CogPMAlignResult cogResult;
#endif
        public OCRItemClass()
        {


        }
        public OCRItemClass(string str)
        {
            FromString(str);
        }
        /// <summary>
        /// 载入参数图
        /// </summary>
        /// <param name="ocrpath">参数图片地址</param>
        public void Load(string ocrpath)
        {
            GetBMP(ocrpath + "\\I" + No.ToString(OCRClass.OrgOCRNoString) + ".png", ref bmpItem);
        }
        /// <summary>
        /// 保存源图
        /// </summary>
        /// <param name="ocrpath">保存地址</param>
        public void Save(string ocrpath)
        {
            SaveBMP(ocrpath + "\\I" + No.ToString(OCRClass.OrgOCRNoString) + ".png", ref bmpItem);
        }
        /// <summary>
        /// 参数克隆
        /// </summary>
        /// <returns></returns>
        public OCRItemClass Clone()
        {
            OCRItemClass newocritem = new OCRItemClass(this.ToString());

            newocritem.rect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
            newocritem.bmpItem.Dispose();
            newocritem.bmpItem = bmpItem.Clone(new Rectangle(0, 0, bmpItem.Width, bmpItem.Height), PixelFormat.Format24bppRgb);
            newocritem.bmpItemTo = (Bitmap)bmpItemTo.Clone();
            newocritem.fScore = fScore;
#if(AUVISION)
            newocritem.xResult = xResult;
#endif
            newocritem.iBackColor = iBackColor;
            if (bmpDifference != null)
                newocritem.bmpDifference = new Bitmap(bmpDifference);
            if (bmpFind != null)
                newocritem.bmpFind = new Bitmap(bmpFind);
            if (bmpTrain != null)
                newocritem.bmpTrain = new Bitmap(bmpTrain);
            newocritem.iArea = iArea;
            newocritem.iFousColor = iFousColor;
            newocritem.iPoint = iPoint;
            newocritem.isDefect = isDefect;
            newocritem.isSelected = isSelected;
            newocritem.No = No;
            newocritem.rect = rect;
            newocritem.strRelateName = strRelateName;
            newocritem.strRelateName2 = strRelateName2;
            return newocritem;
        }
        /// <summary>
        /// 参数字符化
        /// </summary>
        /// <returns>字符化的参数</returns>
        public override string ToString()
        {
            char seperator = Universal.SeperateCharA;
            string str = "";

            str += No.ToString() + seperator;   //0
            str += strRelateName + seperator;      //1
            str += strRelateName2 + seperator;
            str += iBackColor + seperator;
            str += "";

            return str;
        }
        /// <summary>
        /// 载入参数
        /// </summary>
        /// <param name="str">参数</param>
        public void FromString(string str)
        {
            str = str.Trim();
            char seperator = Universal.SeperateCharA;
            string[] strs = str.Split(seperator);

            No = int.Parse(strs[0]);
            strRelateName = strs[1];
            strRelateName2 = strs[2];
            if (strs.Length > 3 && strs[3] != "")
                iBackColor = int.Parse(strs[3]);
        }
        /// <summary>
        /// 资源清理
        /// </summary>
        public void Suicide()
        {
            bmpItem.Dispose();
        }
        /// <summary>
        /// 获取OCR图片
        /// </summary>
        /// <param name="bmpfilestr">图片地址</param>
        /// <param name="bmp">赋值图</param>
        void GetBMP(string bmpfilestr, ref Bitmap bmp)
        {
            Bitmap bmptmp = new Bitmap(bmpfilestr);

            bmp.Dispose();
            bmp = new Bitmap(bmptmp);

            bmptmp.Dispose();
        }
        /// <summary>
        /// 保存源图
        /// </summary>
        /// <param name="bmpfilestr">保存地址</param>
        /// <param name="bmp">源图片</param>
        void SaveBMP(string bmpfilestr, ref Bitmap bmp)
        {
            Bitmap bmptmp = new Bitmap(bmp);

            bmptmp.Save(bmpfilestr, Universal.GlobalImageFormat);

            bmptmp.Dispose();
        }
    }
    
    [Serializable]
    /// <summary>
    /// 带AuFind的OCR参数
    /// </summary>
    public class OCRTrain
    {
        /// <summary>
        /// 使用源图来比较
        /// </summary>
        public bool isTiemTest = true;
        public string strValue;
        public string strValue2;
        /// <summary>
        /// 未二值化的源图
        /// </summary>
        public Bitmap bmpItemTo;
        /// <summary>
        /// 二值化的源图
        /// </summary>
        public Bitmap bmpItem;
        /// <summary>
        /// 需要Find的图
        /// </summary>
        public Bitmap bmpFind;
        /// <summary>
        /// 结果图
        /// </summary>
        public Bitmap bmpResult;
        /// <summary>
        /// 差异图
        /// </summary>
        public Bitmap bmpDifference;
        /// <summary>
        /// 缺失点数
        /// </summary>
        public int iPoint;
        /// <summary>
        /// 缺失最大面积
        /// </summary>
        public int iArea;
        /// <summary>
        /// 缺失是否为良品
        /// </summary>
        public bool isDefect;
        /// <summary>
        /// Find分数
        /// </summary>
        public float fScore;
        public Rectangle rect;

        /// <summary>
        /// 背景亮度
        /// </summary>
        public int iBackColor = 200;

        public int iBackColorTemp = 0;

        /// <summary>
        /// 前景亮度
        /// </summary>
        public int iFousColor = 200;
#if(AUVISION)
        public xFindResult xResult;
        public xMatchingResult xResultMatch;
        AUFind xFindObj;
        xTrainingInfoF xInfo;
        AUMatch xMatchObj;
        /// <summary>
        /// 缩放大小
        /// </summary>
        public Size setSize = new Size(6, 6);
#endif
#if(COGNEX)
        public Cognex.VisionPro.PMAlign.CogPMAlignResult cogResult;
        CogPMAlignTool myPMAlingn;
        CogFixtureTool cogFixture = new CogFixtureTool();

        public Bitmap bmpFixItem = new Bitmap(1, 1);
        public Bitmap bmpFix = new Bitmap(1, 1);
        public Bitmap bmpFixBmpTO = new Bitmap(1, 1);
#endif

        public bool Train()
        {
#if(AUVISION)
            if (xFindObj == null)
                xFindObj = new AUFind();
#endif
            bmpItemTo = bmpItem.Clone(new Rectangle(0, 0, bmpItem.Width, bmpItem.Height), PixelFormat.Format24bppRgb);
            Bitmap bmpJ2 = bmpItem.Clone(new Rectangle(0, 0, bmpItem.Width, bmpItem.Height), PixelFormat.Format24bppRgb);
            Bitmap bmpBlance2 = new Bitmap(bmpJ2.Width, bmpJ2.Height, PixelFormat.Format8bppIndexed);

            JetEazy.BasicSpace.myImageProcessor.Balance(bmpJ2, ref bmpBlance2, ref iBackColor, JetEazy.BasicSpace.myImageProcessor.myOCRThreshold);
            bmpItem = bmpBlance2;

            bmpJ2.Dispose();
            //    bmpBlance2.Dispose();
            iBackColorTemp = iBackColor;

            if (!isTiemTest)
            {
                Bitmap bmpJ = bmpItem.Clone(new Rectangle(0, 0, bmpItem.Width, bmpItem.Height), PixelFormat.Format24bppRgb);
                Bitmap bmpBlance = new Bitmap(bmpJ.Width, bmpJ.Height, PixelFormat.Format8bppIndexed);

                JetEazy.BasicSpace.myImageProcessor.Balance(bmpJ, ref bmpBlance, ref iBackColor, JetEazy.BasicSpace.myImageProcessor.myOCRThreshold);

                iBackColorTemp = iBackColor;
                JetEazy.BasicSpace.myImageProcessor.SetBimap8To24(bmpJ, bmpBlance, ref iFousColor);

                Bitmap bmpTemp = new Bitmap(bmpJ.Width + setSize.Width, bmpJ.Height + setSize.Height);
                bmpTemp = bmpTemp.Clone(new Rectangle(0, 0, bmpTemp.Width, bmpTemp.Height), PixelFormat.Format24bppRgb);
                Graphics g = Graphics.FromImage(bmpTemp);
                g.FillRectangle(new SolidBrush(Color.FromArgb(255, 255, 255)), new Rectangle(0, 0, bmpTemp.Width, bmpTemp.Height));
                g.DrawImage(bmpJ, new Point(setSize.Width / 2, setSize.Height / 2));
                g.Dispose();
                bmpItem = bmpTemp;
                //   bmpTemp.Save("D://Train.png");

                Bitmap bmpTemp2 = new Bitmap(bmpJ.Width + setSize.Width, bmpJ.Height + setSize.Height);
                bmpTemp2 = bmpTemp2.Clone(new Rectangle(0, 0, bmpTemp2.Width, bmpTemp2.Height), PixelFormat.Format24bppRgb);
                Graphics g2 = Graphics.FromImage(bmpTemp2);
                g2.FillRectangle(new SolidBrush(Color.FromArgb(iBackColor, iBackColor, iBackColor)), new Rectangle(0, 0, bmpTemp2.Width, bmpTemp2.Height));
                g2.DrawImage(bmpItemTo, new Point(setSize.Width / 2, setSize.Height / 2));
                g2.Dispose();
                bmpItemTo = bmpTemp2;
                // bmpItemTo.Save("D://Train_R.png");

                //bmpItem.Dispose();
                //bmpItem = bmpJ;
                // bmpItem = bmpItem.Clone(new Rectangle(0, 0, bmpItem.Width, bmpItem.Height), PixelFormat.Format24bppRgb);

#if (AUVISION)
            }
            AUGrayImg8 xTemplate = new AUGrayImg8();
            //载入AUImage中 
            AUUtility.DrawBitmapToAUGrayImg8(bmpItemTo, ref xTemplate);

            xFindObj.GetTrainingInfo(out xInfo); //获得默认训练设置
            xInfo.fRotationTolerance = 5; //设定旋转角度 : -180 ~ 180
            xInfo.fScalingTolerance = 0f; //设定缩放比例: 90% ~ 110%  
                                          //设定图片是否要缩小,如果不缩小则让它为最小边的值.
            xInfo.nDownSamplingSize = 50;// xTemplate.GetWidth() > xTemplate.GetHeight() ? xTemplate.GetWidth() : xTemplate.GetHeight();
            xInfo.nCannyThresholdHigh = 200;
            xInfo.nCannyThresholdLow = 128;

            xInfo.eFMode = eFindMode.eFindMode_GHT;

            bool bol = xFindObj.Training(xTemplate, xInfo, true); //训练


            xFindObj.SetMaxOcc(1); //设定相似的最大数量
            xFindObj.SetTolerance(0.1f); //设定差异，设定范围为：0.1 ~ 1.0


#endif

#if(COGNEX)
            if (myPMAlingn != null)
                myPMAlingn.Dispose();
            myPMAlingn = new CogPMAlignTool();
            CogImage8Grey cogimginput = new CogImage8Grey(bmpItem);

            RectangleF rectangleF = new RectangleF(0, 0, bmpItem.Width, bmpItem.Height);
            CogRectangleAffine cogaff = AffConvert(rectangleF);

            myPMAlingn.Pattern.TrainRegion = cogaff;
            myPMAlingn.Pattern.Origin.TranslationX = 0;// cogimginput.Width / 2;
            myPMAlingn.Pattern.Origin.TranslationY = 0;// cogimginput.Height / 2;

            myPMAlingn.Pattern.TrainAlgorithm = CogPMAlignTrainAlgorithmConstants.PatQuick;
            myPMAlingn.Pattern.TrainMode = CogPMAlignTrainModeConstants.Image;
            myPMAlingn.Pattern.TrainRegionMode = CogRegionModeConstants.PixelAlignedBoundingBoxAdjustMask;

            myPMAlingn.RunParams.RunAlgorithm = CogPMAlignRunAlgorithmConstants.BestTrained;
            myPMAlingn.RunParams.RunMode = CogPMAlignRunModeConstants.SearchImage;
            myPMAlingn.RunParams.TimeoutEnabled = true;
            myPMAlingn.RunParams.Timeout = 5000;
            myPMAlingn.RunParams.ScoreUsingClutter = true;

            myPMAlingn.RunParams.OutsideRegionThreshold = 0;
            myPMAlingn.RunParams.ApproximateNumberToFind = 1;
            myPMAlingn.RunParams.AcceptThreshold = 0.5;

            myPMAlingn.RunParams.ZoneAngle.Nominal = 0;
            myPMAlingn.RunParams.ZoneAngle.Low = CogMisc.DegToRad(-5);
            myPMAlingn.RunParams.ZoneAngle.High = CogMisc.DegToRad(5);
            myPMAlingn.RunParams.ZoneScale.Nominal = 1;
            myPMAlingn.RunParams.ZoneScale.Low = 1;
            myPMAlingn.RunParams.ZoneScale.High = 1;
            myPMAlingn.Pattern.TrainImage = cogimginput;

            //  cogimginput.ToBitmap().Save("D://PATTERN.bmp");

            myPMAlingn.RunParams.AcceptThreshold = 0.7d;
            myPMAlingn.Pattern.Train();

#endif
            return bol;
        }
#if (COGNEX)

        /// <summary>
        /// RectangleF转CogAffine
        /// </summary>
        /// <param name="rectf"></param>
        /// <returns></returns>
        public CogRectangleAffine AffConvert(RectangleF rectf)
        {
            CogRectangleAffine cogaff = new CogRectangleAffine();
            cogaff.FitToBoundingBox(RectConvert(rectf));

            return cogaff;
        }
        public CogRectangle RectConvert(RectangleF rectf)
        {
            CogRectangle cogRect = new CogRectangle();

            cogRect.Width = rectf.Width;
            cogRect.Height = rectf.Height;
            cogRect.X = rectf.X;
            cogRect.Y = rectf.Y;

            return cogRect;
        }
        public void SetcogPMAlign(CogPMAlignTool cogpmalign)
        {
            cogpmalign.Pattern.Origin.TranslationX = 0;
            cogpmalign.Pattern.Origin.TranslationY = 0;

            cogpmalign.Pattern.TrainAlgorithm = CogPMAlignTrainAlgorithmConstants.PatQuick;

            cogpmalign.Pattern.TrainMode = CogPMAlignTrainModeConstants.Image;
            cogpmalign.Pattern.TrainRegionMode = CogRegionModeConstants.PixelAlignedBoundingBoxAdjustMask;

            cogpmalign.RunParams.RunAlgorithm = CogPMAlignRunAlgorithmConstants.BestTrained;
            cogpmalign.RunParams.RunMode = CogPMAlignRunModeConstants.SearchImage;
            cogpmalign.RunParams.ScoreUsingClutter = false;

            //Open This Feature to Get the rotation Image
            cogpmalign.RunParams.ZoneAngle.Configuration = CogPMAlignZoneConstants.LowHigh;

            //媽的死BK，給我一個錯的程式，害我差點被搞死
            cogpmalign.RunParams.ZoneAngle.Low = CogMisc.DegToRad(-5); //Convert Degree to Rad
            cogpmalign.RunParams.ZoneAngle.High = CogMisc.DegToRad(5); //Convert Degree to Rad

            //cogPMAlign.RunParams.SaveMatchInfo = true;

            cogpmalign.RunParams.TimeoutEnabled = true;
            cogpmalign.RunParams.Timeout = 3000;
        }
#endif
        public void TrainMarch()
        {
#if(AUVISION)
            if (xMatchObj == null)
                xMatchObj = new AUMatch();
#endif

            Bitmap bmpJ = bmpItem.Clone(new Rectangle(0, 0, bmpItem.Width, bmpItem.Height), PixelFormat.Format24bppRgb);
            bmpItemTo = bmpItem.Clone(new Rectangle(0, 0, bmpItem.Width, bmpItem.Height), PixelFormat.Format24bppRgb);
            Bitmap bmpBlance = new Bitmap(bmpJ.Width, bmpJ.Height, PixelFormat.Format8bppIndexed);

            JetEazy.BasicSpace.myImageProcessor.Balance(bmpJ, ref bmpBlance, JetEazy.BasicSpace.myImageProcessor.EnumThreshold.Intermodes);

            JetEazy.BasicSpace.myImageProcessor.SetBimap8To24(bmpJ, bmpBlance, ref iFousColor);

            bmpItem.Dispose();
            bmpItem = bmpJ;
            //  bmpJ.Save("D:\\bmpj.bmp");
#if(AUVISION)
            AUGrayImg8 xTemplate = new AUGrayImg8();
            //载入AUImage中 
            AUUtility.DrawBitmapToAUGrayImg8(bmpItem, ref xTemplate);

            xTrainingInfo xInfo = new xTrainingInfo();
            xInfo.eAccuracy = eMatchingAccuracy.eMatchingAccuracy_High; //设定精度
            xInfo.ePrefilter = eMatchingPrefilter.eMatchingPrefilter_Sobel;
            xInfo.isTargetRotated = false; //如果目标图像寻找对像是否有旋转。
            xInfo.nPyramidSize = 35; //设置采样大小= 35（默认值）
            xMatchObj.TrainingPattern(xTemplate, xInfo); //Training Pattern

            //xMatchObj.SetSubPixel(true); //使suppixel精度支持
            xMatchObj.SetTolerance(0.7f); //设定公差范围
            xMatchObj.SetMaxOcc(1);
#endif

        }
        public void Match(Bitmap bmp)
        {
#if(AUVISION)
            AUGrayImg8 mySrcImg = new AUGrayImg8();
            AUUtility.DrawBitmapToAUGrayImg8(bmp, ref mySrcImg);
            int iCount = xMatchObj.Matching(mySrcImg);

            if (iCount > 0)
            {

                xMatchObj.GetResult(out xResultMatch, 0);
                fScore = xResultMatch.fScore;
            }
#endif
        }
        public void Match()
        {
#if(AUVISION)
            AUGrayImg8 mySrcImg = new AUGrayImg8();
            AUUtility.DrawBitmapToAUGrayImg8(bmpFind, ref mySrcImg);
            int iCount = xMatchObj.Matching(mySrcImg);

            if (iCount > 0)
            {
                xMatchObj.GetResult(out xResultMatch, 0);
                fScore = xResultMatch.fScore;
            }
#endif
        }
        public void Find(Bitmap bmp)
        {
#if(AUVISION)
            AUGrayImg8 mySrcImg = new AUGrayImg8();
            AUUtility.DrawBitmapToAUGrayImg8(bmp, ref mySrcImg);
            xFindObj.Find(mySrcImg);

            if (xFindObj.GetResultCount() > 0)
            {
                xFindResult xr = xFindObj.GetResult(0);
                fScore = xr.fScore;
            }
#endif
        }
        public void Find()
        {
#if(AUVISION)
            AUGrayImg8 mySrcImg = new AUGrayImg8();
            AUUtility.DrawBitmapToAUGrayImg8(bmpFind, ref mySrcImg);
            xFindObj.Find(mySrcImg);

            if (xFindObj.GetResultCount() > 0)
            {
                xResult = xFindObj.GetResult(0);

                if (xResult.fAngle > 2 || xResult.fAngle < -2)
                    xResult.fAngle = 0;
                fScore = xResult.fScore;
            //}
            //if (myPMAlingn.Results.Count > 0)
            //{
                //cogResult = myPMAlingn.Results[0];
                //fScore = (float)cogResult.Score;

                //cogFixture.InputImage = cogimginput;
                //cogFixture.RunParams.UnfixturedFromFixturedTransform = cogResult.GetPose();
                ////cogFixture.RunParams.FixturedSpaceName = "123";
                //cogFixture.Run();

                //RectangleF rectangleF = new RectangleF(0, 0, bmpItem.Width, bmpItem.Height);
                //CogRectangleAffine cogaff = AffConvert(rectangleF);

                //cogaff.Rotation = cogResult.GetPose().Rotation;
                ////  cogaff.Skew =    cogResult.GetPose().Skew;
                //cogaff.CenterX += cogResult.GetPose().TranslationX + 10;
                //cogaff.CenterY += cogResult.GetPose().TranslationY + 10;

                //Bitmap bmpTemp = new Bitmap(bmpFind.Width + 20, bmpFind.Height + 20);
                //Graphics g = Graphics.FromImage(bmpTemp);
                //g.FillRectangle(new SolidBrush(Color.FromArgb(255, 255, 255)), new Rectangle(0, 0, bmpTemp.Width, bmpTemp.Height));
                //g.DrawImage(bmpFind, new Point(10, 10));
                //g.Dispose();

                //CogImage24PlanarColor cogbmp = new CogImage24PlanarColor(bmpTemp);
                //CogImage24PlanarColor cogimgoutput = CopyAffRegion(cogbmp, cogaff, "#");
                //bmpFix = cogimgoutput.ToBitmap();

                //Bitmap bmpTemp2 = new Bitmap(bmpFixBmpTO.Width + 20, bmpFixBmpTO.Height + 20);
                //Graphics g2 = Graphics.FromImage(bmpTemp2);
                //g2.FillRectangle(new SolidBrush(Color.FromArgb(iBackColor, iBackColor, iBackColor)), new Rectangle(0, 0, bmpTemp.Width, bmpTemp.Height));
                //g2.DrawImage(bmpFixBmpTO, new Point(10, 10));
                //g2.Dispose();

                //CogImage24PlanarColor cogbmp2 = new CogImage24PlanarColor(bmpTemp2);
                //CogImage24PlanarColor cogimgoutput2 = CopyAffRegion(cogbmp2, cogaff, "#");
                //bmpFixItem = cogimgoutput2.ToBitmap();

                //if (bmpFix.Size != bmpItem.Size || bmpFixItem.Size != bmpItem.Size || bmpItemTo.Size !=bmpFixItem.Size)
                //{
                //bmpItem.Save(@"D://Train.bmp", ImageFormat.Bmp);
                //bmpTemp.Save(@"D://Find.bmp", ImageFormat.Bmp);
                //bmpFix.Save(@"D://Result.bmp", ImageFormat.Bmp);
                //bmpFixItem.Save(@"D://ResultItem.bmp", ImageFormat.Bmp);
                //}


            }
        }
#endif
#if(COGNEX)
            Bitmap bmpfindTemp = new Bitmap(bmpFind);
            CogImage8Grey cogimginput = new CogImage8Grey(bmpfindTemp);
            bmpfindTemp.Dispose();
            myPMAlingn.InputImage = cogimginput;

            // cogimginput.ToBitmap().Save(@"D://INPUT.bmp");

            myPMAlingn.Run();

            if (myPMAlingn.Results.Count > 0)
            {
                cogResult = myPMAlingn.Results[0];
                fScore = (float)cogResult.Score;

                cogFixture.InputImage = cogimginput;
                cogFixture.RunParams.UnfixturedFromFixturedTransform = cogResult.GetPose();
                //cogFixture.RunParams.FixturedSpaceName = "123";
                cogFixture.Run();

                RectangleF rectangleF = new RectangleF(0, 0, bmpItem.Width, bmpItem.Height);
                CogRectangleAffine cogaff = AffConvert(rectangleF);

                cogaff.Rotation = cogResult.GetPose().Rotation;
                //  cogaff.Skew =    cogResult.GetPose().Skew;
                cogaff.CenterX += cogResult.GetPose().TranslationX+10;
                cogaff.CenterY += cogResult.GetPose().TranslationY+10;

                Bitmap bmpTemp = new Bitmap(bmpFind.Width + 20, bmpFind.Height + 20);
                Graphics g = Graphics.FromImage(bmpTemp);
                g.FillRectangle(new SolidBrush(Color.FromArgb(255, 255, 255)), new Rectangle(0, 0, bmpTemp.Width, bmpTemp.Height));
                g.DrawImage(bmpFind, new Point(10, 10));
                g.Dispose();

                CogImage24PlanarColor cogbmp = new CogImage24PlanarColor(bmpTemp);
                CogImage24PlanarColor cogimgoutput = CopyAffRegion(cogbmp, cogaff, "#");
                bmpFix = cogimgoutput.ToBitmap();

                Bitmap bmpTemp2 = new Bitmap(bmpFixBmpTO.Width + 20, bmpFixBmpTO.Height + 20);
                Graphics g2 = Graphics.FromImage(bmpTemp2);
                g2.FillRectangle(new SolidBrush(Color.FromArgb(iBackColor, iBackColor, iBackColor)), new Rectangle(0, 0, bmpTemp.Width, bmpTemp.Height));
                g2.DrawImage(bmpFixBmpTO, new Point(10, 10));
                g2.Dispose();

                CogImage24PlanarColor cogbmp2 = new CogImage24PlanarColor(bmpTemp2);
                CogImage24PlanarColor cogimgoutput2 = CopyAffRegion(cogbmp2, cogaff, "#");
                bmpFixItem = cogimgoutput2.ToBitmap();

                //if (bmpFix.Size != bmpItem.Size || bmpFixItem.Size != bmpItem.Size || bmpItemTo.Size !=bmpFixItem.Size)
                //{
                //bmpItem.Save(@"D://Train.bmp", ImageFormat.Bmp);
                //bmpTemp.Save(@"D://Find.bmp", ImageFormat.Bmp);
                //bmpFix.Save(@"D://Result.bmp", ImageFormat.Bmp);
                //bmpFixItem.Save(@"D://ResultItem.bmp", ImageFormat.Bmp);
                //}
                

            }
        }

        CogImage8Grey CopyRegion(CogImage8Grey cogimg, ICogRegion cogregion, int fillvalue)
        {
            CogCopyRegionTool cogCopyRegion = new CogCopyRegionTool();

            cogCopyRegion.InputImage = cogimg;
            cogCopyRegion.DestinationImage = null;

            cogCopyRegion.Region = cogregion;

            cogCopyRegion.RunParams.ImageAlignmentEnabled = false;
            cogCopyRegion.RunParams.RegionMode = CogRegionModeConstants.PixelAlignedBoundingBoxAdjustMask;
            cogCopyRegion.RunParams.FillRegion = false;
            cogCopyRegion.RunParams.FillBoundingBox = true;
            cogCopyRegion.RunParams.FillBoundingBoxValue = fillvalue;

            cogCopyRegion.Run();

            return (CogImage8Grey)cogCopyRegion.OutputImage;
        }
        CogImage24PlanarColor CopyAffRegion(CogImage24PlanarColor cogimg, CogRectangleAffine cogaff, string selectspace)
        {
            CogAffineTransformTool cogCopyAffRegion = new CogAffineTransformTool();

            cogCopyAffRegion.InputImage = cogimg;

            cogCopyAffRegion.Region = cogaff;
            cogCopyAffRegion.Region.SelectedSpaceName = selectspace;

            cogCopyAffRegion.Run();

            return (CogImage24PlanarColor)cogCopyAffRegion.OutputImage;
        }
        CogImage24PlanarColor CopyRegion(CogImage24PlanarColor cogimg, ICogRegion cogregion, int fillvalue)
        {
            CogCopyRegionTool cogCopyRegion = new CogCopyRegionTool();

            cogCopyRegion.InputImage = cogimg;
            cogCopyRegion.DestinationImage = null;

            cogCopyRegion.Region = cogregion;

            cogCopyRegion.RunParams.ImageAlignmentEnabled = false;
            cogCopyRegion.RunParams.RegionMode = CogRegionModeConstants.PixelAlignedBoundingBoxAdjustMask;
            cogCopyRegion.RunParams.FillRegion = false;
            cogCopyRegion.RunParams.FillBoundingBox = true;
            cogCopyRegion.RunParams.FillBoundingBoxValue = fillvalue;

            cogCopyRegion.Run();

            return (CogImage24PlanarColor)cogCopyRegion.OutputImage;
        }
#endif
#if(AUVISION)
        /// <summary>
        /// 算出偏移及旋轉值
        /// </summary>
        /// <param name="result"></param>
        /// <param name="imginput24"></param>
        /// <param name="imgoutput24"></param>
        public void ScaleRotate(xFindResult result, AUColorImg24 imginput24, ref AUColorImg24 imgoutput24)
        {
            #region 带Mask功能的
            //float fTargetCX = imginput24.GetWidth() / 2.0f;
            //float fTargetCY = imginput24.GetHeight() / 2.0f;
            //float fAffineCX = imginput24.GetWidth() / 2.0f;
            //float fAffineCY = imginput24.GetHeight() / 2.0f;

            //float fCosSida = (float)Math.Cos(-result.fAngle * Math.PI / 180.0f);
            //float fSinSida = (float)Math.Sin(-result.fAngle * Math.PI / 180.0f);
            //float fX = (result.fCenterX - fTargetCX) * fCosSida - (result.fCenterY - fTargetCY) * fSinSida;
            //float fY = (result.fCenterX - fTargetCX) * fSinSida + (result.fCenterY - fTargetCY) * fCosSida;

            ////eInterpolationBits_1,4,8 8 for best but slowest
            //AUImage.ScaleRotate(imginput24, imgoutput24,
            //         fTargetCX, fTargetCY,
            //        //   result.fCenterX, result.fCenterY,
            //        fAffineCX - fX, fAffineCY - fY,
            //        result.fAngle, 1.0f, 1.0f, eInterpolationBits.eInterpolationBits_8);
            #endregion

            AUImage.ScaleRotate(imginput24, imgoutput24,
                                result.fCenterX, result.fCenterY,
                                imgoutput24.GetWidth() / 2, imgoutput24.GetHeight() / 2,
                                result.fAngle, 1.0f, 1.0f, eInterpolationBits.eInterpolationBits_8);

        }
        /// <summary>
        /// 算出偏移及旋轉值
        /// </summary>
        /// <param name="result"></param>
        /// <param name="imginput24"></param>
        /// <param name="imgoutput24"></param>
        public void ScaleRotate(xMatchingResult result, AUColorImg24 imginput24, ref AUColorImg24 imgoutput24)
        {

            AUImage.ScaleRotate(imginput24, imgoutput24,
                                result.fCenterX, result.fCenterY,
                                imgoutput24.GetWidth() / 2, imgoutput24.GetHeight() / 2,
                                result.fAngle, 1.0f, 1.0f, eInterpolationBits.eInterpolationBits_8);

        }
#endif
        public OCRTrain Clone()
        {
            OCRTrain ocr = new OCRTrain();
            ocr.strValue = strValue;
            ocr.strValue2 = strValue2;
            ocr.bmpItem = bmpItem.Clone() as Bitmap;
            ocr.fScore = fScore;
            ocr.iBackColor = iBackColor;
            ocr.bmpFind = bmpFind.Clone() as Bitmap;
            return ocr;
        }
    }
}
