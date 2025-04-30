using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace JetEazy.OpenCv4
{
    internal class EzImageClass
    {
    }
    /// <summary>
    /// 影像處理: 反向, 亮度, 對比, 角度 0, 90, 180, 270
    /// </summary>
    public class EzImageProcess
    {
        #region PRIVATE_DATA
        int _lastBrightness;
        int _lastContrast;
        byte[] _lut;
        #endregion

        //public void ApplyInvBrightnessContrast(Bitmap bmp, IEzCameraProps settings)
        //{
        //    if (settings.InverseModeEnabled || settings.Contrast != 0 || settings.Brightness != 0)
        //    {
        //        using (var bridge = new QxImageBridge(bmp))
        //        {
        //            if (settings.InverseModeEnabled)
        //            {
        //                ApplyInverse(bridge.Image);
        //            }

        //            ApplyBrightnessContrast(bridge.Image, settings.Brightness, settings.Contrast);
        //        }
        //    }
        //}

        public Bitmap ApplyRotation(Bitmap bmp, int rotateAngle)
        {
            RotateFlipType rotateFlip;

            switch (rotateAngle)
            {
                case 90:
                    rotateFlip = RotateFlipType.Rotate90FlipNone;
                    break;
                case 270:
                    rotateFlip = RotateFlipType.Rotate270FlipNone;
                    break;
                case 180:
                    rotateFlip = RotateFlipType.Rotate180FlipNone;
                    break;
                case 0:
                default:
                    return bmp;
            }

            bmp.RotateFlip(rotateFlip);
            return bmp;
        }

        public void ApplyInverse(Bitmap bmp)
        {
            using (var bridge = new QxImageBridge(bmp))
            {
                ApplyInverse(bridge.Image);
            }
        }
        public void ApplyInverse(Mat img)
        {
            if (img.Channels() < 4)
            {
                Cv2.BitwiseNot(img, img, null);
            }
            else
            {
                var imgs = img.Split();
                for (int i = 0; i < 3; i++)
                    Cv2.BitwiseNot(imgs[i], imgs[i], null);
                Cv2.Merge(imgs, img);
            }
        }

        public void ApplyBrightnessContrast(Bitmap bmp, int brightness, int contrast)
        {
            using (var bridge = new QxImageBridge(bmp))
            {
                ApplyBrightnessContrast(bridge.Image, brightness, contrast);
            }
        }
        public void ApplyBrightnessContrast(Mat img, int brightness, int contrast)
        {
            if (brightness != 0 || contrast != 0)
            {
                if (_lut == null || _lastBrightness != brightness || _lastContrast != contrast)
                {
                    _lastBrightness = brightness;
                    _lastContrast = contrast;
                    _lut = _calcLut(contrast, brightness);
                }
                Cv2.LUT(img, _lut, img);
            }
        }

        /// <summary>
        /// 簡單轉換為單色 (channels數量不變)
        /// </summary>
        public void ApplyMonoColor(Bitmap bmp, int targetChannel = 1)
        {
            if (bmp.PixelFormat == PixelFormat.Format8bppIndexed)
                return;

            using (var bridge = new QxImageBridge(bmp))
            {
                ApplyMonoColor(bridge.Image, targetChannel);
            }
        }
        public void ApplyMonoColor(Mat img, int targetChannel = 1)
        {
            int N = img.Channels();
            if (N == 1)
                return;

            targetChannel = Math.Min(targetChannel, N - 1);
            var imgs = img.Split();

            for (int i = 0; i < imgs.Length && i < 3; i++)
                imgs[i] = imgs[targetChannel];

            if (N == 4)
            {
                imgs[3].SetTo(Scalar.White);
            }

            Cv2.Merge(imgs, img);
        }

        public Bitmap ToRgb24(Bitmap src, bool autoDispose = false)
        {
            if (src.PixelFormat != PixelFormat.Format24bppRgb)
            {
                // NOTE:  32Bit 的 Alpha channel 可能會讓 畫面黑掉
                //var bmp = new Bitmap(src.Width, src.Height, PixelFormat.Format24bppRgb);
                //using (var gx = Graphics.FromImage(bmp))
                //{
                //    // 設定 CompositingMode 讓 Alpha 不要影響本色
                //    gx.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                //    gx.DrawImageUnscaled(src, 0, 0);
                //}
                var rect = new Rectangle(0, 0, src.Width, src.Height);
                var bmp = src.Clone(rect, PixelFormat.Format24bppRgb);
                if (autoDispose)
                    src.Dispose();
                return bmp;
            }
            return src;
        }
        public Bitmap ToU8(Bitmap src, bool autoDispose = false)
        {
            if (src.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                SetGrayPalete(src);
                return src;
            }
            else
            {
                var newBmp = new Bitmap(src.Width, src.Height, PixelFormat.Format8bppIndexed);
                using (var srcBridge = new QxImageBridge(src))
                using (var dstBridge = new QxImageBridge(newBmp))
                {
                    int bits = Image.GetPixelFormatSize(src.PixelFormat);
                    switch (bits)
                    {
                        case 32:
                            Cv2.CvtColor(srcBridge.Image, dstBridge.Image, ColorConversionCodes.BGRA2GRAY);
                            break;
                        case 24:
                            Cv2.CvtColor(srcBridge.Image, dstBridge.Image, ColorConversionCodes.BGR2GRAY);
                            break;
                        case 16:
                            Cv2.CvtColor(srcBridge.Image, dstBridge.Image, ColorConversionCodes.BGR5652GRAY);
                            break;
                        default:
                            throw new Exception("[ToU8] 未知的 PixelFormat!");
                    }
                }
                if (autoDispose)
                    src.Dispose();
                SetGrayPalete(newBmp);
                return newBmp;
            }
        }
        public static void SetGrayPalete(Bitmap bmpU8)
        {
            if (bmpU8.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                // 設置調色板
                ColorPalette palette = bmpU8.Palette;
                for (int i = 0; i < 256; i++)
                {
                    palette.Entries[i] = Color.FromArgb(i, i, i); // 設置灰度調色板
                }
                bmpU8.Palette = palette;
            }
        }

        #region PRIVATE_FUNCTIONS
        private static byte[] _calcLut(int contrast, int brightness)
        {
            byte[] lut = new byte[256];

            if (contrast > 0)
            {
                double delta = 127.0 * contrast / 100;
                double a = 255.0 / (255.0 - delta * 2);
                double b = a * (brightness - delta);
                for (int i = 0; i < 256; i++)
                {
                    int v = (int)Math.Round(a * i + b);
                    if (v < 0)
                        v = 0;
                    if (v > 255)
                        v = 255;
                    lut[i] = (byte)v;
                }
            }
            else
            {
                double delta = -128.0 * contrast / 100;
                double a = (256.0 - delta * 2) / 255.0;
                double b = a * brightness + delta;
                for (int i = 0; i < 256; i++)
                {
                    int v = (int)Math.Round(a * i + b);
                    if (v < 0)
                        v = 0;
                    if (v > 255)
                        v = 255;
                    lut[i] = (byte)v;
                }
            }
            return lut;
        }
        #endregion
    }
    public class QxImageBridge : IDisposable
    {
        private Mat m_cvImage;

        private Bitmap m_bmpSrc;

        private BitmapData m_bmpdLock;

        public Mat Image => m_cvImage;

        private void _CHECK_THIS_FOR_CV3()
        {
            Trace.WriteLine(m_cvImage.Size());
        }

        public QxImageBridge()
        {
            m_bmpSrc = null;
            m_bmpdLock = null;
            m_cvImage = null;
        }

        public QxImageBridge(Bitmap bmpSrc)
        {
            Lock(bmpSrc);
        }

        public QxImageBridge(BitmapData bmpdSrc)
        {
            Lock(bmpdSrc);
        }

        public QxImageBridge(Bitmap bmpSrc, ref Rectangle rectCrop, PixelFormat format)
        {
            Lock(bmpSrc, ref rectCrop, format);
        }

        public void Dispose()
        {
            Unlock();
        }

        public void Lock(Bitmap bmp)
        {
            Rectangle rectCrop = new Rectangle(0, 0, bmp.Width, bmp.Height);
            Lock(bmp, ref rectCrop, bmp.PixelFormat);
        }

        public void Lock(Bitmap bmp, ref Rectangle rectCrop, PixelFormat format)
        {
            Unlock();
            m_bmpSrc = bmp;
            m_bmpdLock = m_bmpSrc.LockBits(rectCrop, ImageLockMode.ReadWrite, format);
            _createCvImage(out m_cvImage, m_bmpdLock);
        }

        public void Lock(BitmapData bmpd)
        {
            Unlock();
            m_bmpSrc = null;
            m_bmpdLock = bmpd;
            _createCvImage(out m_cvImage, m_bmpdLock);
        }

        public void Unlock()
        {
            _disposeCvImage(m_cvImage);
            m_cvImage = null;
            if (m_bmpdLock != null)
            {
                if (m_bmpSrc != null)
                {
                    m_bmpSrc.UnlockBits(m_bmpdLock);
                }

                m_bmpdLock = null;
                m_bmpSrc = null;
            }
        }

        public static implicit operator Mat(QxImageBridge bridge)
        {
            return bridge.m_cvImage;
        }

        public static implicit operator BitmapData(QxImageBridge bridge)
        {
            return bridge.m_bmpdLock;
        }

        public static BitmapData GetBitmapData(Mat img)
        {
            if (img == null)
            {
                throw new ArgumentNullException("img");
            }

            if (img.IsDisposed)
            {
                throw new ArgumentException("The image is disposed.", "img");
            }

            BitmapData bitmapData = new BitmapData();
            bitmapData.Scan0 = img.Data;
            bitmapData.Stride = (int)img.Step();
            bitmapData.Width = img.Width;
            bitmapData.Height = img.Height;
            switch (img.Channels())
            {
                case 1:
                    bitmapData.PixelFormat = PixelFormat.Format8bppIndexed;
                    break;
                case 3:
                    bitmapData.PixelFormat = PixelFormat.Format24bppRgb;
                    break;
                case 4:
                    bitmapData.PixelFormat = PixelFormat.Format32bppArgb;
                    break;
                default:
                    throw new ArgumentException("Number of channels must be 1, 3 or 4.", "img");
            }

            return bitmapData;
        }

        public static void SafeDispose(Mat[] imgs)
        {
            if (imgs != null)
            {
                for (int i = 0; i < imgs.Length; i++)
                {
                    imgs[i]?.Dispose();
                }
            }
        }

        private void _createCvImage(out Mat cvImage, BitmapData bmpdFrom)
        {
            _ = bmpdFrom.PixelFormat;
            _ = 2498570;
            MatType type = (System.Drawing.Image.GetPixelFormatSize(bmpdFrom.PixelFormat) >> 3);
            //switch(type)
            //{
            //    case MatType.CV_8UC1:
            //        break;
            //    1 => MatType.CV_8UC1,
            //    3 => MatType.CV_8UC3,
            //    4 => MatType.CV_8UC4,
            //    _ => throw new ArgumentException("Number of channels must be 1, 3 or 4.", "bmpdFrom"),
            //};
            int height = bmpdFrom.Height;
            int width = bmpdFrom.Width;
            cvImage = new Mat(height, width, type, bmpdFrom.Scan0, bmpdFrom.Stride);
        }

        private void _disposeCvImage(Mat cvImage)
        {
            if (cvImage != null && !cvImage.IsDisposed)
            {
                cvImage.Dispose();
            }
        }

        public void ConvertFromGray(Mat imgGrayU8)
        {
            Mat cvImage = m_cvImage;
            if (m_bmpdLock == null || cvImage == null)
            {
                return;
            }

            if (cvImage.Channels() == imgGrayU8.Channels())
            {
                imgGrayU8.CopyTo(cvImage);
                return;
            }

            if (cvImage.Channels() == 3)
            {
                Cv2.CvtColor(imgGrayU8, cvImage, ColorConversionCodes.GRAY2BGR);
                return;
            }

            if (cvImage.Channels() > 3)
            {
                int rows = imgGrayU8.Rows;
                int cols = imgGrayU8.Cols;
                Mat mat = new Mat(rows, cols, MatType.CV_8UC1);
                mat.SetTo(Scalar.White);
                Cv2.Merge(new Mat[4] { imgGrayU8, imgGrayU8, imgGrayU8, mat }, cvImage);
                return;
            }

            throw new NotSupportedException();
        }

        public unsafe void ConvertColorFromGray(Mat imgGrayU8, ref Rectangle rect, uint uColor, uint uMask)
        {
            BitmapData bitmapData = this;
            if (bitmapData == null)
            {
                return;
            }

            if (imgGrayU8.Type() != MatType.CV_8UC1)
            {
                throw new Exception("imgGrayU8 must be U8C1");
            }

            int num = System.Drawing.Image.GetPixelFormatSize(bitmapData.PixelFormat) / 8;
            int x = rect.X;
            int y = rect.Y;
            int num2 = rect.Y + rect.Height;
            int num3 = rect.X + rect.Width;
            int stride = bitmapData.Stride;
            byte* ptr = (byte*)(void*)bitmapData.Scan0 + y * stride + x * num;
            int num4 = (int)imgGrayU8.Step();
            byte* ptr2 = imgGrayU8.DataPointer + y * num4 + x;
            for (int i = y; i < num2; i++)
            {
                byte* ptr3 = ptr;
                byte* ptr4 = ptr2;
                for (int j = x; j < num3; j++)
                {
                    if (*ptr4 != 0)
                    {
                        uint num5 = _getColor(ptr3, num);
                        num5 = (num5 & ~uMask) | uColor;
                        _setColor(ptr3, num, num5);
                    }

                    ptr3 += num;
                    ptr4++;
                }

                ptr += stride;
                ptr2 += num4;
            }
        }

        public unsafe void ConvertColorFromGray(Mat imgGrayU8, ref Rectangle rect, uint uOpColor, uint uColor, uint uMask)
        {
            BitmapData bitmapData = this;
            if (bitmapData == null)
            {
                return;
            }

            if (imgGrayU8.Type() != MatType.CV_8UC1)
            {
                throw new Exception("imgGrayU8 must be U8C1");
            }

            int num = System.Drawing.Image.GetPixelFormatSize(bitmapData.PixelFormat) / 8;
            int x = rect.X;
            int y = rect.Y;
            int num2 = rect.Y + rect.Height;
            int num3 = rect.X + rect.Width;
            int stride = bitmapData.Stride;
            byte* ptr = (byte*)(void*)bitmapData.Scan0 + y * stride + x * num;
            int num4 = (int)imgGrayU8.Step();
            byte* ptr2 = imgGrayU8.DataPointer + y * num4 + x;
            for (int i = y; i < num2; i++)
            {
                byte* ptr3 = ptr;
                byte* ptr4 = ptr2;
                for (int j = x; j < num3; j++)
                {
                    if (*ptr4 == (byte)uOpColor)
                    {
                        uint num5 = _getColor(ptr3, num);
                        num5 = (num5 & ~uMask) | uColor;
                        _setColor(ptr3, num, num5);
                    }

                    ptr3 += num;
                    ptr4++;
                }

                ptr += stride;
                ptr2 += num4;
            }
        }

        private unsafe uint _getColor(byte* pucPtr, int bytesPerPixel)
        {
            uint num = 0u;
            for (int num2 = bytesPerPixel - 1; num2 >= 0; num2--)
            {
                num <<= 8;
                num |= pucPtr[num2];
            }

            return num;
        }

        private unsafe void _setColor(byte* pucPtr, int bytesPerPixel, uint c)
        {
            for (int i = 0; i < bytesPerPixel; i++)
            {
                pucPtr[i] = (byte)(c & 0xFFu);
                c >>= 8;
            }
        }
    }
}
