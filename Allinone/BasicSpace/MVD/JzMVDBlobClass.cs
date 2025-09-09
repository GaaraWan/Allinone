using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VisionDesigner;

namespace Allinone.BasicSpace.MVD
{
    public class JzMVDBlobClass : IDisposable
    {
        VisionDesigner.ImageBinary.CImageBinaryTool cImageBinaryToolObj = null;// new VisionDesigner.ImageBinary.CImageBinaryTool();
        //VisionDesigner.ImageMorph.CImageMorphTool cImageMorphToolObj = null;// new VisionDesigner.ImageMorph.CImageMorphTool();
        VisionDesigner.BlobFind.CBlobFindTool cBlobFindToolObj = null;// new VisionDesigner.BlobFind.CBlobFindTool();

        public JzMVDBlobClass() { }
        ~JzMVDBlobClass()
        {
            Dispose();
        }

        public List<RectangleF> NGBounds { get; set; } = new List<RectangleF>();
        public Bitmap bmpBlobImage { get; set; } = new Bitmap(1, 1);

        public int ThresholdValue { get; set; } = 128;
        public bool IsWhite { get; set; } = true;
        public int blobMin { get; set; } = 100;
        public int blobMax { get; set; } = 60000;
        public float blobRatio { get; set; } = 0.5f;
        public double FindBaseArea { get; set; } = 100;

        /// <summary>
        /// 找blob
        /// </summary>
        /// <param name="bmpInput">输入图像 必须是8bit</param>
        /// <param name="ebmpMask">掩膜图像 必须是8bit</param>
        /// <returns>true-OK  false-NG</returns>
        public bool Run(Bitmap bmpInput, Bitmap ebmpMask)
        {
            bool bOK = true;

            if (cImageBinaryToolObj == null)
                cImageBinaryToolObj = new VisionDesigner.ImageBinary.CImageBinaryTool();
            if (cBlobFindToolObj == null)
                cBlobFindToolObj = new VisionDesigner.BlobFind.CBlobFindTool();

            //二值化
            cImageBinaryToolObj.InputImage = BitmapToCMvdImage(bmpInput);
            cImageBinaryToolObj.ROI = null;
            //= new CMvdRectangleF(OutputImage.Width / 2, OutputImage.Height / 2, OutputImage.Width / 4, OutputImage.Height / 4);
            cImageBinaryToolObj.SetRunParam("LowThreshold", ThresholdValue.ToString());
            //cImageArithmeticToolObj.SetRunParam("HighThreshold", BlobHighThreshold.ToString());
            cImageBinaryToolObj.Run();
            //cImageBinaryToolObj.Result.OutputImage.SaveImage($"{_path}\\{lblName}_Diff2.bmp", MVD_FILE_FORMAT.MVD_FILE_BMP);

            ////形态学
            //cImageMorphToolObj.InputImage = cImageBinaryToolObj.Result.OutputImage;
            //cImageMorphToolObj.SetRunParam("Type", "Open");
            //cImageMorphToolObj.ROI = _roi;
            ////= new VisionDesigner.CMvdRectangleF(cInputImg.Width / 2, cInputImg.Height / 2, cInputImg.Width / 4, cInputImg.Height / 4);
            //cImageMorphToolObj.Run();

            //blob
            cBlobFindToolObj.InputImage = cImageBinaryToolObj.Result.OutputImage;
            cBlobFindToolObj.RegionImage = BitmapToCMvdImage(ebmpMask);
            cBlobFindToolObj.ROI //= _roi;
                    = new CMvdRectangleF(cImageBinaryToolObj.Result.OutputImage.Width / 2,
            cImageBinaryToolObj.Result.OutputImage.Height / 2,
            cImageBinaryToolObj.Result.OutputImage.Width,
            cImageBinaryToolObj.Result.OutputImage.Height);

            if (!IsWhite)
                cBlobFindToolObj.SetRunParam("Polarity", "BrightObject");
            else
                cBlobFindToolObj.SetRunParam("Polarity", "DarkObject");

            cBlobFindToolObj.BasicParam.ShowBlobImageStatus = true;
            cBlobFindToolObj.Run();
            VisionDesigner.BlobFind.CBlobFindResult cBlobFindRes = cBlobFindToolObj.Result;

            if (cImageBinaryToolObj.Result.OutputImage != null)
            {
                bmpBlobImage.Dispose();
                bmpBlobImage = CMvdImageToBitmap(cImageBinaryToolObj.Result.OutputImage);

                //cBlobFindRes.BlobImage.SaveImage("D:\\1.PNG", MVD_FILE_FORMAT.MVD_FILE_PNG);
            }

            NGBounds.Clear();
            float allArea = cImageBinaryToolObj.Result.OutputImage.Width * cImageBinaryToolObj.Result.OutputImage.Height;
            foreach (var item in cBlobFindToolObj.Result.BlobInfo)
            {
                if (item.AreaF > blobMin && item.AreaF < FindBaseArea * 0.3)
                {
                    double _ratio = item.AreaF / FindBaseArea;
                    //if (item.AreaF > blobMin && item.AreaF < blobMax)
                    if (_ratio > blobRatio)
                    {
                        bOK = false;
                        MVD_RECT_F mVD_RECT_ = item.BoxInfo.GetBoundingRect();
                        NGBounds.Add(new RectangleF(mVD_RECT_.fX, mVD_RECT_.fY, mVD_RECT_.fWidth, mVD_RECT_.fHeight));
                        //break;
                    }
                }
            }
            //if (NGBounds.Count > 0)
            //{
            //    Graphics graphics = Graphics.FromImage(bmpBlobImage);
            //    graphics.DrawRectangles(new Pen(Color.Red, 3), NGBounds.ToArray());
            //    graphics.Dispose();
            //}
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
        private Bitmap CMvdImageToBitmap(CMvdImage eCMvdImage)
        {
            MVD_IMAGE_DATA_INFO _MvdImage = eCMvdImage.GetImageData();
            MVD_DATA_CHANNEL_INFO ch0 = _MvdImage.stDataChannel[0];
            //Bitmap _bmpFromMVD = ByteArrayToBitmap(ch0.arrDataBytes, (int)ch0.nRowStep, (int)(ch0.nLen / ch0.nRowStep));
            return ByteArrayToBitmap(ch0.arrDataBytes, (int)ch0.nRowStep, (int)(ch0.nLen / ch0.nRowStep));
        }
        private Bitmap ByteArrayToBitmap(byte[] byteArray, int width, int height)
        {
            // 创建 Bitmap 对象
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            // 设置调色板为灰度
            ColorPalette palette = bitmap.Palette;
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            bitmap.Palette = palette;
            // 锁定 Bitmap 数据
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            // 将字节数组复制到 Bitmap 数据中
            System.Runtime.InteropServices.Marshal.Copy(byteArray, 0, bitmapData.Scan0, byteArray.Length);
            // 解锁 Bitmap 数据
            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }
        public void Dispose()
        {
            if (cBlobFindToolObj != null)
            {
                cBlobFindToolObj.Dispose();
                cBlobFindToolObj = null;
            }
            if (cImageBinaryToolObj != null)
            {
                cImageBinaryToolObj.Dispose();
                cImageBinaryToolObj = null;
            }
        }

    }
}
