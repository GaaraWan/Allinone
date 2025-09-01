using AUVision;
//using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VisionDesigner;
using VisionDesigner.AlmightyPatMatch;

namespace Allinone.BasicSpace.MVD
{
    public class JzMVDPatMatchClass : IDisposable
    {
        private string m_PatternPath = "";
        private PMatchType m_PMatchType = PMatchType.HPM;

        public List<xFindResult> xResults = new List<xFindResult>();
        public RectangleF xFindLastRect = new RectangleF();

        public float MTRotation { get; set; } = 15;
        public float MTTolerance { get; set; } = 0.7f;
        public float MTMaxOcc { get; set; } = 1;

        VisionDesigner.AlmightyPatMatch.CAlmightyPattern cAlmightyPatternObj = null;
        VisionDesigner.AlmightyPatMatch.CAlmightyPatMatchTool cAlmightyPatmatchToolObj = null;
        VisionDesigner.PositionFix.CPositionFixTool cPositionFixToolObj = null;

        public JzMVDPatMatchClass() { }
        ~JzMVDPatMatchClass()
        {
            Dispose();
        }

        /// <summary>
        /// 二值化的源图
        /// </summary>
        public Bitmap bmpItem;
        /// <summary>
        /// 需要Find的图
        /// </summary>
        public Bitmap bmpFind;

        public bool Train()
        {
            bool bOK = false;

            #region HIK_TRAIN

            // CreatePatternInstance
            if (cAlmightyPatternObj == null)
                cAlmightyPatternObj = new VisionDesigner.AlmightyPatMatch.CAlmightyPattern();

            //Set type
            switch (m_PMatchType)
            {
                case PMatchType.HPM:
                    cAlmightyPatternObj.Type = PatMatchAlgorithmType.HPFeature;
                    break;
                case PMatchType.FM:
                    cAlmightyPatternObj.Type = PatMatchAlgorithmType.FastFeature;
                    break;
            }

            // Set input image
            VisionDesigner.CMvdImage cInputImg = BitmapToCMvdImage(bmpItem);
            Bitmap bmp24 = new Bitmap(1, 1);
            if (bmpItem.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
                bmp24 = grayscale.Apply(bmpItem);
                cInputImg = BitmapToCMvdImage(bmp24);
            }

            //VisionDesigner.CMvdImage cInputImg = BitmapToCMvdImage(bmp24);
            if (cInputImg.PixelFormat != MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08)
            {
                //当前程序仅支持mono8。因此像素格会转换.
                cInputImg.ConvertImagePixelFormat(MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08);
            }

            // Set ROI region (optional)
            var cRectObj = new VisionDesigner.CMvdRectangleF(cInputImg.Width / 2, cInputImg.Height / 2, cInputImg.Width, cInputImg.Height);

            cAlmightyPatternObj.RegionList.Clear();
            cAlmightyPatternObj.RegionList.Add(new CAlmightyPatMatchRegion(cRectObj, true));

            // Set basic parameter
            cAlmightyPatternObj.BasicParam.FixPoint = new MVD_POINT_F(Convert.ToSingle(cInputImg.Width) / 2, Convert.ToSingle(cInputImg.Height) / 2);

            cAlmightyPatternObj.InputImage = cInputImg;

            string strvaluex = string.Empty;
            string errmessage = string.Empty;
            // Train
            try
            {
                //cAlmightyPatternObj.InputImage.SaveImage("D:\\train.png", MVD_FILE_FORMAT.MVD_FILE_PNG);
                // Train
                cAlmightyPatternObj.Train();
                //cAlmightyPatternObj.ExportPattern("D:\\fmPattern.fmxml");
                bOK = true;
            }
            catch (Exception ex)
            {
                errmessage = ex.Message;
                bOK = false;
            }
            #endregion

            return bOK;

        }
        public bool Run()
        {
            bool bOK = false;

            #region HIK_RUN

            // CreateToolInstance
            if (cAlmightyPatmatchToolObj == null)
                cAlmightyPatmatchToolObj = new VisionDesigner.AlmightyPatMatch.CAlmightyPatMatchTool();

            //Set type
            switch (m_PMatchType)
            {
                case PMatchType.HPM:
                    cAlmightyPatmatchToolObj.Type = PatMatchAlgorithmType.HPFeature;
                    break;
                case PMatchType.FM:
                    cAlmightyPatmatchToolObj.Type = PatMatchAlgorithmType.FastFeature;
                    break;
            }

            //// Set ROI region (optional)
            cAlmightyPatmatchToolObj.RegionList.Clear();
            var region2 = new VisionDesigner.CMvdRectangleF(bmpFind.Width * 0.5f, bmpFind.Height * 0.5f, bmpFind.Width, bmpFind.Height);
            cAlmightyPatmatchToolObj.RegionList.Add(new CAlmightyPatMatchRegion(region2, true));

            // Set basic parameter

            cAlmightyPatmatchToolObj.BasicParam.ShowOutlineStatus = false;

            cAlmightyPatmatchToolObj.SetRunParam("AngleStart", (-MTRotation).ToString());
            cAlmightyPatmatchToolObj.SetRunParam("AngleEnd", MTRotation.ToString());
            cAlmightyPatmatchToolObj.SetRunParam("MinScore", MTTolerance.ToString());
            cAlmightyPatmatchToolObj.SetRunParam("MaxMatchNum", MTMaxOcc.ToString());

            // Set input image
            //Bitmap bmp24 = bmpinput.Clone(new Rectangle(0, 0, bmpinput.Width, bmpinput.Height), PixelFormat.Format8bppIndexed);
            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            Bitmap bmp24 = grayscale.Apply(bmpFind);
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

            cAlmightyPatmatchToolObj.InputImage = cInputImg2;
            try
            {
                //cAlmightyPatmatchToolObj.InputImage.SaveImage("D:\\run.png", MVD_FILE_FORMAT.MVD_FILE_PNG);
                cAlmightyPatmatchToolObj.Pattern = cAlmightyPatternObj;
                // Running
                cAlmightyPatmatchToolObj.Run();
            }
            catch
            {

            }

            // Get the result
            VisionDesigner.AlmightyPatMatch.CAlmightyPatMatchResult cHPMatchRes = cAlmightyPatmatchToolObj.Result;


            int resultcount = cHPMatchRes.MatchInfoList.Count;
            if (resultcount > 0)
            {
                foreach (var item in cHPMatchRes.MatchInfoList)
                {
                    //Console.WriteLine("MatchPoint: ({0},{1})", item.MatchPoint.fX, item.MatchPoint.fY);
                    if (item.Score >= MTTolerance)
                    {
                        xFindResult result = new xFindResult();
                        //var item = cHPMatchRes.MatchInfoList[0];
                        result.fCenterX = item.MatchBox.CenterX;
                        result.fCenterY = item.MatchBox.CenterY;
                        result.fAngle = item.MatchBox.Angle;
                        result.fScale = item.Scale;
                        result.fScore = item.Score;

                        xResults.Add(result);

                        CMvdRectangleF mvdRectangleF = PositionFixRun(
                            new RectangleF(0, 0, cAlmightyPatternObj.InputImage.Width, cAlmightyPatternObj.InputImage.Height),
                                                                            new Rectangle(0, 0, bmpFind.Width, bmpFind.Height),
                                                                                result);

                        //定位完成后裁切位置
                        xFindLastRect = new RectangleF(mvdRectangleF.CenterX - cAlmightyPatternObj.InputImage.Width / 2,
                            mvdRectangleF.CenterY - cAlmightyPatternObj.InputImage.Height / 2,
                            cAlmightyPatternObj.InputImage.Width,
                            cAlmightyPatternObj.InputImage.Height);

                        bOK = true;
                        break;
                    }
                }
            }
            bmp24.Dispose();

            #endregion

            return bOK;
        }
        /// <summary>
        /// 计算修正后的位置框
        /// </summary>
        /// <param name="templateRectF">模板尺寸</param>
        /// <param name="runRect">输入图片尺寸</param>
        /// <param name="templateRunResult">定位的结果</param>
        public CMvdRectangleF PositionFixRun(RectangleF templateRectF, Rectangle runRect, xFindResult templateRunResult)
        {
            // CreateInstance
            if (cPositionFixToolObj == null)
                cPositionFixToolObj = new VisionDesigner.PositionFix.CPositionFixTool();

            // Set basic parameter

            cPositionFixToolObj.BasicParam.BasePoint
                = new VisionDesigner.PositionFix.MVD_FIDUCIAL_POINT_F(
                    new MVD_POINT_F(templateRectF.Width / 2, templateRectF.Height / 2), 0);

            cPositionFixToolObj.BasicParam.RunningPoint
                = new VisionDesigner.PositionFix.MVD_FIDUCIAL_POINT_F(
                    new MVD_POINT_F(templateRunResult.fCenterX, templateRunResult.fCenterY), templateRunResult.fAngle);

            cPositionFixToolObj.BasicParam.RunImageSize = new MVD_SIZE_I(runRect.Width, runRect.Height);

            cPositionFixToolObj.BasicParam.FixMode = VisionDesigner.PositionFix.MVD_POSFIX_MODE.MVD_POSFIX_MODE_HVA;

            var RectangleShape
                = new CMvdRectangleF(templateRectF.Width / 2, templateRectF.Height / 2, templateRectF.Width, templateRectF.Height);

            cPositionFixToolObj.BasicParam.InitialShape = RectangleShape;

            // Running

            cPositionFixToolObj.Run();

            // Get the result

            return cPositionFixToolObj.Result.CorrectedShape as CMvdRectangleF;

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

        public void Dispose()
        {
            if (cAlmightyPatternObj != null)
            {
                cAlmightyPatternObj.Dispose();
                cAlmightyPatternObj = null;
            }
            if (cAlmightyPatmatchToolObj != null)
            {
                cAlmightyPatmatchToolObj.Dispose();
                cAlmightyPatmatchToolObj = null;
            }
            if (cPositionFixToolObj != null)
            {
                cPositionFixToolObj.Dispose();
                cPositionFixToolObj = null;
            }
        }

    }
}
