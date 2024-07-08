using System.Drawing;
using System.Drawing.Imaging;

namespace EzSegClientExamples
{
    public class EzImageUtils
    {
        /// <summary>
        /// 把 maskBmp 疊加畫到 dstBmp 上面
        /// </summary>
        public static void DrawMask(Bitmap dstBmp, Bitmap maskBmp)
        {
            var rect = new Rectangle(0, 0, dstBmp.Width, dstBmp.Height);
            BitmapData dstBmpd = dstBmp.LockBits(rect, ImageLockMode.ReadWrite, dstBmp.PixelFormat);
            BitmapData srcBmpd = maskBmp.LockBits(rect, ImageLockMode.ReadOnly, maskBmp.PixelFormat);

            int dstBytesPerPixel = Image.GetPixelFormatSize(dstBmpd.PixelFormat) >> 3;
            int srcBytesPerPixel = Image.GetPixelFormatSize(srcBmpd.PixelFormat) >> 3;

            unsafe
            {
                int x, xmax, xmin;
                int y, ymax, ymin;

                xmin = rect.X;
                ymin = rect.Y;
                ymax = rect.Y + rect.Height;
                xmax = rect.X + rect.Width;

                int maskStride = srcBmpd.Stride;
                byte* maskLine = (byte*)srcBmpd.Scan0 + (ymin * maskStride) + (xmin * srcBytesPerPixel);
                byte* maskPtr;

                int dstStride = dstBmpd.Stride;
                byte* dstLine = (byte*)dstBmpd.Scan0 + (ymin * dstStride) + (xmin * dstBytesPerPixel);
                byte* dstPtr;

                for (y = ymin; y < ymax; y++)
                {
                    maskPtr = maskLine;
                    dstPtr = dstLine;
                    for (x = xmin; x < xmax; x++)
                    {
                        // 根據 mask 來增顯藍色
                        //>>> dstPtr[0] |= maskPtr[0];
                        if (maskPtr[0] > 128)
                        {
                            dstPtr[0] = 255;
                            dstPtr[1] /= 2;
                            dstPtr[2] /= 2;
                        }

                        dstPtr += dstBytesPerPixel;
                        maskPtr += srcBytesPerPixel;
                    }
                    maskLine += maskStride;
                    dstLine += dstStride;
                }
            }

            dstBmp.UnlockBits(dstBmpd);
            maskBmp.UnlockBits(srcBmpd);
        }

        public static void DisposeImages(Bitmap[] images)
        {
            if (images != null)
            {
                foreach (var img in images)
                {
                    if (img != null)
                        img.Dispose();
                }
            }
        }
    }
}
