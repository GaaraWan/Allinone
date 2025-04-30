//using AUVision;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VisionDesigner;
using ZXing;

namespace Allinone.BasicSpace.MVD
{
    public class JzMVDGrayPatMatchClass : IDisposable
    {
        VisionDesigner.GrayPatMatch.CGrayPattern cGrayPatternObj = null;
        VisionDesigner.GrayPatMatch.CGrayPatMatchTool cGrayPatMatchToolObj = null;

        public JzMVDGrayPatMatchClass() { }
        ~JzMVDGrayPatMatchClass()
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

        public bool HikTrainGray()
        {
            bool bOK = false;

            #region HIK_TRAIN

            // CreatePatternInstance
            if (cGrayPatternObj == null)
                cGrayPatternObj = new VisionDesigner.GrayPatMatch.CGrayPattern();

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

            cGrayPatternObj.RegionList.Clear();
            cGrayPatternObj.RegionList.Add(new VisionDesigner.GrayPatMatch.CPatMatchRegion(cRectObj, true));

            // Set basic parameter
            cGrayPatternObj.BasicParam.FixPoint = new MVD_POINT_F(Convert.ToSingle(cInputImg.Width) / 2, Convert.ToSingle(cInputImg.Height) / 2);

            cGrayPatternObj.InputImage = cInputImg;

            string strvaluex = string.Empty;
            string errmessage = string.Empty;
            // Train
            try
            {
                // Train
                cGrayPatternObj.Train();
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
        public bool HikRunGray()
        {
            bool bOK = false;

            #region HIK_RUN

            // CreateToolInstance
            if (cGrayPatMatchToolObj == null)
                cGrayPatMatchToolObj = new VisionDesigner.GrayPatMatch.CGrayPatMatchTool();

            VisionDesigner.CMvdImage cInputImg2 = BitmapToCMvdImage(bmpFind);
            Bitmap bmp24 = new Bitmap(1, 1);
            if (bmpFind.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
                bmp24 = grayscale.Apply(bmpFind);
                cInputImg2 = BitmapToCMvdImage(bmp24);
            }
            // Set input image
            //Bitmap bmp24 = bmpinput.Clone(new Rectangle(0, 0, bmpinput.Width, bmpinput.Height), PixelFormat.Format8bppIndexed);

            if (cInputImg2.PixelFormat != MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08)
            {
                //当前程序仅支持mono8。因此像素格会转换.
                cInputImg2.ConvertImagePixelFormat(MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08);
            }

            // Set input image
            cGrayPatMatchToolObj.InputImage = cInputImg2;

            // Set ROI region (optional)
            cGrayPatMatchToolObj.ROI = new VisionDesigner.CMvdRectangleF(cInputImg2.Width / 2, cInputImg2.Height / 2, cInputImg2.Width, cInputImg2.Height);

            // Set Pattern
            cGrayPatMatchToolObj.Pattern = cGrayPatternObj;

            cGrayPatMatchToolObj.SetRunParam("AngleStart", "-5");
            cGrayPatMatchToolObj.SetRunParam("AngleEnd", "5");
            cGrayPatMatchToolObj.SetRunParam("MinScore", "0.5");
            cGrayPatMatchToolObj.SetRunParam("AngleStep", "2");

            // Running
            cGrayPatMatchToolObj.Run();

            // Get the result
            VisionDesigner.GrayPatMatch.CGrayPatMatchResult cGrayMatchRes = cGrayPatMatchToolObj.Result;
            //foreach (var item in cGrayMatchRes.MatchInfoList)
            //{
            //    Console.WriteLine("MatchPoint: ({0},{1})", item.MatchPoint.fX, item.MatchPoint.fY);
            //}


            int resultcount = cGrayMatchRes.MatchInfoList.Count;
            if (resultcount > 0)
            {
                foreach (var item in cGrayMatchRes.MatchInfoList)
                {
                    //Console.WriteLine("MatchPoint: ({0},{1})", item.MatchPoint.fX, item.MatchPoint.fY);
                    if (item.Score >= 0.8)
                    {
                        bOK = true;
                        break;
                    }
                }
            }
            bmp24.Dispose();

            #endregion

            return bOK;
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
            if (cGrayPatternObj != null)
            {
                cGrayPatternObj.Dispose();
                cGrayPatternObj = null;
            }
            if (cGrayPatMatchToolObj != null)
            {
                cGrayPatMatchToolObj.Dispose();
                cGrayPatMatchToolObj = null;
            }
        }

    }
}
