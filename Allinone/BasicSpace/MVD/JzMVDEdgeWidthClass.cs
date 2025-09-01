using PdfSharp.Pdf.AcroForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VisionDesigner;
using VisionDesigner.EdgeWidth;

namespace Allinone.BasicSpace.MVD
{
    public class JzMVDEdgeWidthClass : IDisposable
    {

        VisionDesigner.EdgeWidth.CEdgeWidthTool cEdgeWidthToolObj = null;

        public JzMVDEdgeWidthClass() { }
        ~JzMVDEdgeWidthClass()
        {
            Dispose();
        }

        public int HalfKernelSize { get; set; } = 2;
        public int ContrastTH { get; set; } = 15;
        /// <summary>
        /// true从上到下 false从左到右
        /// </summary>
        public bool FindOrient { get; set; } = true;
        //public Rectangle RoiPos { get; set; } = new Rectangle();
        //public VisionDesigner.CMvdImage cInputImg2 { get; set; } = new CMvdImage();

        private PointF p0 = new PointF(0, 0);
        private PointF p1 = new PointF(10, 10);

        public PointF p0ret
        {
            get { return p0; }
        }
        public PointF p1ret
        {
            get { return p1; }
        }
        
        public bool Run(Bitmap ebmpInput)
        {

            bool bOK = false;
            //try

            //{

            //    // CreateInstance
            //    if (cEdgeWidthToolObj == null)
            //        cEdgeWidthToolObj = new VisionDesigner.EdgeWidth.CEdgeWidthTool();

            //    // Set input image

            //    AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            //    Bitmap bmp24 = grayscale.Apply(ebmpInput);
            //    VisionDesigner.CMvdImage cInputImg2 = BitmapToCMvdImage(bmp24);
            //    if (cInputImg2.PixelFormat != MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08)
            //    {
            //        //当前程序仅支持mono8。因此像素格会转换.
            //        cInputImg2.ConvertImagePixelFormat(MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08);
            //    }
            //    //cInputImg2.InitImage("InputTest2.bmp");

            //    //cInputImg2.SaveImage($"D:\\Data\\cInputImg2run_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.bmp", MVD_FILE_FORMAT.MVD_FILE_BMP);

            //    bmp24.Dispose();
            //    grayscale = null;

            //    cEdgeWidthToolObj.InputImage = cInputImg2;

            //    // Set ROI region (optional)

            //    cEdgeWidthToolObj.ROI = new VisionDesigner.CMvdRectangleF(RoiPos.X + RoiPos.Width / 2, RoiPos.Y + RoiPos.Height / 2,
            //        RoiPos.Width, RoiPos.Height);

            //    //cEdgeWidthToolObj.ROI = new VisionDesigner.CMvdRectangleF(cInputImg2.Width / 2, cInputImg2.Height / 2,
            //    //    cInputImg2.Width, cInputImg2.Height);

            //    cEdgeWidthToolObj.SetRunParam("HalfKernelSize", HalfKernelSize.ToString());
            //    cEdgeWidthToolObj.SetRunParam("ContrastTH", ContrastTH.ToString());

            //    cEdgeWidthToolObj.SetRunParam("Maximum", "1");
            //    cEdgeWidthToolObj.SetRunParam("Edge0Polarity", "Both");
            //    cEdgeWidthToolObj.SetRunParam("Edge1Polarity", "Both");
            //    if (FindOrient)
            //        cEdgeWidthToolObj.SetRunParam("FindOrient", "UpToDown");
            //    else
            //        cEdgeWidthToolObj.SetRunParam("FindOrient", "LeftToRight");

            //    cEdgeWidthToolObj.SetRunParam("EdgeWidthFindMode", "Widest");

            //    // Running

            //    //cEdgeWidthToolObj.Run();

            //    //// Get the result

            //    //VisionDesigner.EdgeWidth.CEdgeWidthResult cEdgeWidthRes = cEdgeWidthToolObj.Result;

            //    //Console.WriteLine("The number of edge pair: {0}", cEdgeWidthRes.EdgePairInfo.Count);

            //    //List<CEdgeWidthEdgePairInfo> lcEdgePairInfo = cEdgeWidthRes.EdgePairInfo;

            //    //if (lcEdgePairInfo.Count > 0)
            //    //{
            //    //    p0 = new PointF(lcEdgePairInfo[0].Edge0Info.EdgePoint.fX, lcEdgePairInfo[0].Edge0Info.EdgePoint.fY);
            //    //    p1 = new PointF(lcEdgePairInfo[0].Edge1Info.EdgePoint.fX, lcEdgePairInfo[0].Edge1Info.EdgePoint.fY);
            //    //}

            //    //foreach (CEdgeWidthEdgePairInfo cCurEdgePair in lcEdgePairInfo)

            //    //{

            //    //    Console.WriteLine("Center of edge pair: x={0}, y={1}", cCurEdgePair.Center.fX, cCurEdgePair.Center.fY);

            //    //    //…More information

            //    //}
            //    bOK = true;
            //}

            //catch (MvdException ex)

            //{

            //    Console.WriteLine("Fail with ErrorCode: 0x" + ex.ErrorCode.ToString("X"));

            //}

            //catch (System.Exception ex)

            //{

            //    Console.WriteLine("Fail with error " + ex.Message);

            //}
            return bOK;
        }
        public bool Run2(Rectangle RoiPos)
        {

            bool bOK = false;
            try

            {

                //// CreateInstance
                //if (cEdgeWidthToolObj == null)
                //    cEdgeWidthToolObj = new VisionDesigner.EdgeWidth.CEdgeWidthTool();

                //// Set input image

                //AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
                //Bitmap bmp24 = grayscale.Apply(ebmpInput);
                //VisionDesigner.CMvdImage cInputImg2 = BitmapToCMvdImage(bmp24);
                //if (cInputImg2.PixelFormat != MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08)
                //{
                //    //当前程序仅支持mono8。因此像素格会转换.
                //    cInputImg2.ConvertImagePixelFormat(MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08);
                //}
                ////cInputImg2.InitImage("InputTest2.bmp");

                ////cInputImg2.SaveImage($"D:\\Data\\cInputImg2run_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.bmp", MVD_FILE_FORMAT.MVD_FILE_BMP);

                //bmp24.Dispose();
                //grayscale = null;

                //cEdgeWidthToolObj.InputImage = cInputImg2;

                // Set ROI region (optional)

                cEdgeWidthToolObj.ROI = new VisionDesigner.CMvdRectangleF(RoiPos.X + RoiPos.Width / 2, RoiPos.Y + RoiPos.Height / 2,
                    RoiPos.Width, RoiPos.Height);

                //cEdgeWidthToolObj.ROI = new VisionDesigner.CMvdRectangleF(cInputImg2.Width / 2, cInputImg2.Height / 2,
                //    cInputImg2.Width, cInputImg2.Height);

                //cEdgeWidthToolObj.SetRunParam("HalfKernelSize", HalfKernelSize.ToString());
                //cEdgeWidthToolObj.SetRunParam("ContrastTH", ContrastTH.ToString());

                //cEdgeWidthToolObj.SetRunParam("Maximum", "1");
                //cEdgeWidthToolObj.SetRunParam("Edge0Polarity", "Both");
                //cEdgeWidthToolObj.SetRunParam("Edge1Polarity", "Both");
                //if (FindOrient)
                //    cEdgeWidthToolObj.SetRunParam("FindOrient", "UpToDown");
                //else
                //    cEdgeWidthToolObj.SetRunParam("FindOrient", "LeftToRight");

                //cEdgeWidthToolObj.SetRunParam("EdgeWidthFindMode", "Widest");

                // Running

                cEdgeWidthToolObj.Run();

                // Get the result

                VisionDesigner.EdgeWidth.CEdgeWidthResult cEdgeWidthRes = cEdgeWidthToolObj.Result;

                Console.WriteLine("The number of edge pair: {0}", cEdgeWidthRes.EdgePairInfo.Count);

                List<CEdgeWidthEdgePairInfo> lcEdgePairInfo = cEdgeWidthRes.EdgePairInfo;

                if (lcEdgePairInfo.Count > 0)
                {
                    p0 = new PointF(lcEdgePairInfo[0].Edge0Info.EdgePoint.fX, lcEdgePairInfo[0].Edge0Info.EdgePoint.fY);
                    p1 = new PointF(lcEdgePairInfo[0].Edge1Info.EdgePoint.fX, lcEdgePairInfo[0].Edge1Info.EdgePoint.fY);
                }

                //foreach (CEdgeWidthEdgePairInfo cCurEdgePair in lcEdgePairInfo)

                //{

                //    Console.WriteLine("Center of edge pair: x={0}, y={1}", cCurEdgePair.Center.fX, cCurEdgePair.Center.fY);

                //    //…More information

                //}
                bOK = true;
            }

            catch (MvdException ex)

            {

                Console.WriteLine("Fail with ErrorCode: 0x" + ex.ErrorCode.ToString("X"));

            }

            catch (System.Exception ex)

            {

                Console.WriteLine("Fail with error " + ex.Message);

            }
            return bOK;
        }
        public void SetImage(VisionDesigner.CMvdImage cInputImg2)
        {
            // CreateInstance
            if (cEdgeWidthToolObj == null)
                cEdgeWidthToolObj = new VisionDesigner.EdgeWidth.CEdgeWidthTool();

            cEdgeWidthToolObj.InputImage = cInputImg2;
        }
        public bool Run3(Rectangle RoiPos)
        {

            bool bOK = false;
            try

            {

                //// CreateInstance
                //if (cEdgeWidthToolObj == null)
                //    cEdgeWidthToolObj = new VisionDesigner.EdgeWidth.CEdgeWidthTool();

                // Set input image

                //AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
                //Bitmap bmp24 = grayscale.Apply(ebmpInput);
                //VisionDesigner.CMvdImage cInputImg2 = BitmapToCMvdImage(bmp24);
                //if (cInputImg2.PixelFormat != MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08)
                //{
                //    //当前程序仅支持mono8。因此像素格会转换.
                //    cInputImg2.ConvertImagePixelFormat(MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08);
                //}
                ////cInputImg2.InitImage("InputTest2.bmp");

                ////cInputImg2.SaveImage($"D:\\Data\\cInputImg2run_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.bmp", MVD_FILE_FORMAT.MVD_FILE_BMP);

                //bmp24.Dispose();
                //grayscale = null;

                //cEdgeWidthToolObj.InputImage = cInputImg2;

                // Set ROI region (optional)

                cEdgeWidthToolObj.ROI = new VisionDesigner.CMvdRectangleF(RoiPos.X + RoiPos.Width / 2, RoiPos.Y + RoiPos.Height / 2,
                    RoiPos.Width, RoiPos.Height);

                //cEdgeWidthToolObj.ROI = new VisionDesigner.CMvdRectangleF(cInputImg2.Width / 2, cInputImg2.Height / 2,
                //    cInputImg2.Width, cInputImg2.Height);

                cEdgeWidthToolObj.SetRunParam("HalfKernelSize", HalfKernelSize.ToString());
                cEdgeWidthToolObj.SetRunParam("ContrastTH", ContrastTH.ToString());

                cEdgeWidthToolObj.SetRunParam("Maximum", "1");
                cEdgeWidthToolObj.SetRunParam("Edge0Polarity", "Both");
                cEdgeWidthToolObj.SetRunParam("Edge1Polarity", "Both");
                if (FindOrient)
                    cEdgeWidthToolObj.SetRunParam("FindOrient", "UpToDown");
                else
                    cEdgeWidthToolObj.SetRunParam("FindOrient", "LeftToRight");

                cEdgeWidthToolObj.SetRunParam("EdgeWidthFindMode", "Widest");

                // Running

                cEdgeWidthToolObj.Run();

                // Get the result

                VisionDesigner.EdgeWidth.CEdgeWidthResult cEdgeWidthRes = cEdgeWidthToolObj.Result;

                Console.WriteLine("The number of edge pair: {0}", cEdgeWidthRes.EdgePairInfo.Count);

                List<CEdgeWidthEdgePairInfo> lcEdgePairInfo = cEdgeWidthRes.EdgePairInfo;

                if (lcEdgePairInfo.Count > 0)
                {
                    p0 = new PointF(lcEdgePairInfo[0].Edge0Info.EdgePoint.fX, lcEdgePairInfo[0].Edge0Info.EdgePoint.fY);
                    p1 = new PointF(lcEdgePairInfo[0].Edge1Info.EdgePoint.fX, lcEdgePairInfo[0].Edge1Info.EdgePoint.fY);
                }

                //foreach (CEdgeWidthEdgePairInfo cCurEdgePair in lcEdgePairInfo)

                //{

                //    Console.WriteLine("Center of edge pair: x={0}, y={1}", cCurEdgePair.Center.fX, cCurEdgePair.Center.fY);

                //    //…More information

                //}
                bOK = true;
            }

            catch (MvdException ex)

            {

                Console.WriteLine("Fail with ErrorCode: 0x" + ex.ErrorCode.ToString("X"));

            }

            catch (System.Exception ex)

            {

                Console.WriteLine("Fail with error " + ex.Message);

            }
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
            if (cEdgeWidthToolObj != null)
            {
                cEdgeWidthToolObj.Dispose();
                cEdgeWidthToolObj = null;
            }
        }
    }


}
