using FreeImageAPI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VisionDesigner;

namespace Allinone.BasicSpace.MVD
{
    public class JzMVDIntensityMeasureClass : IDisposable
    {
        //VisionDesigner.IntensityMeasure.CIntensityMeasureTool cIntensityToolToolObj = null;

        public JzMVDIntensityMeasureClass() { }
        ~JzMVDIntensityMeasureClass()
        {
            Dispose();
        }
        public float Run(Bitmap bmpInput)
        {

            float meanLum = 0.0f;
            try

            {
                using (VisionDesigner.IntensityMeasure.CIntensityMeasureTool cIntensityToolToolObj
                    = new VisionDesigner.IntensityMeasure.CIntensityMeasureTool())
                {
                    using (Bitmap bmp = To8bit(bmpInput))
                    {
                        VisionDesigner.CMvdImage cInputImg = BitmapToCMvdImage(bmp);

                        //cInputImg.InitImage("InputTest.bmp");

                        cIntensityToolToolObj.InputImage = cInputImg;

                        // Set ROI region (optional)

                        cIntensityToolToolObj.ROI = new VisionDesigner.CMvdRectangleF(cInputImg.Width / 2, cInputImg.Height / 2, cInputImg.Width / 4, cInputImg.Height / 4);

                        // Running

                        cIntensityToolToolObj.Run();

                        // Get the result

                        VisionDesigner.IntensityMeasure.CIntensityMeasureResult cIntensityToolRes = cIntensityToolToolObj.Result;
                        if (cIntensityToolRes != null)
                            meanLum = cIntensityToolRes.MeanLum;
                    }

                }
            }

            catch (MvdException ex)

            {

                Console.WriteLine("Fail with ErrorCode: 0x" + ex.ErrorCode.ToString("X"));

            }

            catch (System.Exception ex)

            {

                Console.WriteLine("Fail with error " + ex.Message);

            }

            return meanLum;

        }
        Bitmap To8bit(Bitmap original)
        {
            using (Bitmap bigBmp = original)
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
        /// <summary>
        /// caller 負責 original 的生命
        /// (直接調用 ToU8)
        /// </summary>
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

            //return ToU8(original, false);
        }
        /// <summary>
        /// caller 負責 original 的生命.
        /// (直接調用 ToU8)
        /// </summary>
        Bitmap Convert24bppTo8bpp(Bitmap original)
        {
            ////if (original.PixelFormat != PixelFormat.Format24bppRgb)
            ////    throw new ArgumentException("源图像必须是24位位图");

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

            //return ToU8(original, false);
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
            else if (bitPixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                Int32 bitmapDataSize = bmData.Stride * bmData.Height;//bitmap图像缓存长度
                int offset = bmData.Stride - bmData.Width * 4;
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
                        // 获取32bpp像素值
                        byte b = _BitImageBufferBytes[bitmapIndex];
                        byte g = _BitImageBufferBytes[bitmapIndex + 1];
                        byte r = _BitImageBufferBytes[bitmapIndex + 2];
                        byte a = _BitImageBufferBytes[bitmapIndex + 3];
                        bitmapIndex += 4;
                        // 转换为灰度值（8bpp）
                        byte gray = (byte)((r * 0.299 + g * 0.587 + b * 0.114) * (a / 255.0));

                        _ImageBaseDataBufferBytes[ImageBaseDataIndex++] = gray;// _BitImageBufferBytes[bitmapIndex++];
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
            else
            {
                cMvdImage.InitImage((uint)bmData.Width, (uint)bmData.Height, MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08);
            }
            bmpInputImg.UnlockBits(bmData);  // 解除锁定
            return cMvdImage;
        }

        public void Dispose()
        {
            //if (cIntensityToolToolObj != null)
            //{
            //    cIntensityToolToolObj.Dispose();
            //    cIntensityToolToolObj = null;
            //}
        }

    }
}
