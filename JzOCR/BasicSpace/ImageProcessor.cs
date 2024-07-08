using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using  System.Collections.Concurrent;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;

namespace JetEazy.BasicSpace
{
  public  class myImageProcessor
    {
        /// <summary>  
        ///  Resize图片   
        /// </summary>  
        /// <param name="bmp">原始Bitmap </param>  
        /// <param name="newW">新的宽度</param>  
        /// <param name="newH">新的高度</param>  
        /// <param name="Mode">保留着，暂时未用</param>  
        /// <returns>处理以后的图片</returns>  
        public static Bitmap ResizeImage(Bitmap bmp, int newW, int newH, int Mode)
        {
            try
            {
                Bitmap b = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(b);
                // 插值算法的质量   
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return b;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>  
        /// 剪裁 -- 用GDI+   
        /// </summary>  
        /// <param name="b">原始Bitmap</param>  
        /// <param name="StartX">开始坐标X</param>  
        /// <param name="StartY">开始坐标Y</param>  
        /// <param name="iWidth">宽度</param>  
        /// <param name="iHeight">高度</param>  
        /// <returns>剪裁后的Bitmap</returns>  
        public static Bitmap Cut(Bitmap b, int StartX, int StartY, int iWidth, int iHeight)
        {
            if (b == null)
            {
                return null;
            }
            int w = b.Width;
            int h = b.Height;
            if (StartX >= w || StartY >= h)
            {
                return null;
            }
            if (StartX + iWidth > w)
            {
                iWidth = w - StartX;
            }
            if (StartY + iHeight > h)
            {
                iHeight = h - StartY;
            }
            try
            {
                Bitmap bmpOut = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);
                Graphics g = Graphics.FromImage(bmpOut);
                g.DrawImage(b, new Rectangle(0, 0, iWidth, iHeight), new Rectangle(StartX, StartY, iWidth, iHeight), GraphicsUnit.Pixel);
                g.Dispose();
                return bmpOut;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>  
        /// 剪裁 -- 用GDI+   
        /// </summary>  
        /// <param name="b">原始Bitmap</param>  
        /// <param name="rect">所需剪裁的地方</param>   
        /// <returns>剪裁后的Bitmap</returns>  
        public static Bitmap Cut(Bitmap bmp, Rectangle rect)
        {
            return Cut(bmp, rect.X, rect.Y, rect.Width, rect.Height);
        }
        /// <summary>  
        /// 剪裁 -- 用GDI+   
        /// </summary>  
        /// <param name="b">原始Bitmap</param>  
        /// <param name="listrect">载剪区的集合</param>   
        /// <returns>剪裁后的Bitmap集合</returns>  
        public static List<Bitmap> Cut(Bitmap b, List<Rectangle> listrect)
        {
            try
            {
                List<Bitmap> list = new List<Bitmap>();
                foreach (Rectangle rect in listrect)
                {
                    Bitmap bmpOut = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);
                    Graphics g = Graphics.FromImage(bmpOut);
                    g.DrawImage(b, new Rectangle(0, 0, rect.Width, rect.Height), rect, GraphicsUnit.Pixel);
                    g.Dispose();
                    list.Add(bmpOut);
                }
                return list;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>  
        /// 以逆时针为方向对图像进行旋转  
        /// </summary>  
        /// <param name="bmp">位图流</param>  
        /// <param name="angle">旋转角度[0,360](前台给的)</param>  
        /// <returns></returns>  
        public static Bitmap RotateImg(Image bmp, int angle)
        {
            angle = angle % 360;
            //弧度转换  
            double radian = angle * Math.PI / 180.0;
            double cos = Math.Cos(radian);
            double sin = Math.Sin(radian);

            //原图的宽和高  
            int w = bmp.Width;
            int h = bmp.Height;
            int W = (int)(Math.Max(Math.Abs(w * cos - h * sin), Math.Abs(w * cos + h * sin)));
            int H = (int)(Math.Max(Math.Abs(w * sin - h * cos), Math.Abs(w * sin + h * cos)));


            //目标位图  
            Bitmap dsImage = new Bitmap(W, H);

            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(dsImage);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //计算偏移量  
            Point Offset = new Point((W - w) / 2, (H - h) / 2);
            //构造图像显示区域：让图像的中心与窗口的中心点一致  
            Rectangle rect = new Rectangle(Offset.X, Offset.Y, w, h);
            Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(360 - angle);
            //恢复图像在水平和垂直方向的平移  
            g.TranslateTransform(-center.X, -center.Y);
            g.DrawImage(bmp, rect);
            //重至绘图的所有变换  
            g.ResetTransform();
            g.Save();
            g.Dispose();
            //保存旋转后的图片  
            //  bmp.Dispose();
            //  dsImage.Save("FocusPoint.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            return dsImage;
        }
        /// <summary>
        /// 任意角度旋转
        /// </summary>
        /// <param name="bmp">原始图Bitmap</param>
        /// <param name="angle">旋转角度</param>
        /// <param name="bkColor">背景色</param>
        /// <returns>输出Bitmap</returns>
        public static Bitmap KiRotate(Bitmap bmp, float angle, Color bkColor)
        {
            int w = bmp.Width + 2;
            int h = bmp.Height + 2;

            PixelFormat pf;

            if (bkColor == Color.Transparent)
            {
                pf = PixelFormat.Format32bppArgb;
            }
            else
            {
                pf = bmp.PixelFormat;
            }

            Bitmap tmp = new Bitmap(w, h, pf);
            Graphics g = Graphics.FromImage(tmp);
            g.Clear(bkColor);
            g.DrawImageUnscaled(bmp, 1, 1);
            g.Dispose();

            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new RectangleF(0f, 0f, w, h));
            Matrix mtrx = new Matrix();
            mtrx.Rotate(angle);
            RectangleF rct = path.GetBounds(mtrx);

            Bitmap dst = new Bitmap((int)rct.Width, (int)rct.Height, pf);
            g = Graphics.FromImage(dst);
            g.Clear(bkColor);
            g.TranslateTransform(-rct.X, -rct.Y);
            g.RotateTransform(angle);
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            g.DrawImageUnscaled(tmp, 0, 0);
            g.Dispose();

            tmp.Dispose();

            return dst;
        }
        /// <summary>
        /// 任意角度旋转
        /// </summary>
        /// <param name="bmp">原始图Bitmap</param>
        /// <param name="angle">旋转角度</param>
        /// <param name="bkColor">背景色</param>
        /// <returns>输出Bitmap</returns>
        public static Bitmap KiRotate(Bitmap bmp, float angle,Rectangle Rotate, Color bkColor)
        {
            int w = bmp.Width + 2;
            int h = bmp.Height + 2;

            PixelFormat pf;

            if (bkColor == Color.Transparent)
            {
                pf = PixelFormat.Format32bppArgb;
            }
            else
            {
                pf = bmp.PixelFormat;
            }

            Bitmap tmp = new Bitmap(w, h, pf);
            Graphics g = Graphics.FromImage(tmp);
            g.Clear(bkColor);
            g.DrawImageUnscaled(bmp, 1, 1);
            g.Dispose();

            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new RectangleF(0f, 0f, w, h));
            Matrix mtrx = new Matrix();
            mtrx.Rotate(angle);
            RectangleF rct = path.GetBounds(mtrx);

            Bitmap dst = new Bitmap((int)rct.Width, (int)rct.Height, pf);
            g = Graphics.FromImage(dst);
            g.Clear(bkColor);
            g.TranslateTransform(-rct.X, -rct.Y);
            g.RotateTransform(angle);
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            g.DrawImageUnscaled(tmp, 0, 0);
            g.Dispose();

            tmp.Dispose();

            return dst;
        }

        /// <summary>
        /// 角度旋转
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Bitmap KiRotate90(Bitmap img, RotateFlipType Rotate)
        {
            try
            {
                img.RotateFlip(Rotate);//RotateFlipType.Rotate90FlipNone);
                return img;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Gamma校正
        /// </summary>
        /// <param name="bmp">输入Bitmap</param>
        /// <param name="val">[0 <-明- 1 -暗-> 2]</param>
        /// <returns>输出Bitmap</returns>
        public static Bitmap KiGamma(Bitmap bmp, float val)
        {
            if (bmp == null)
            {
                return null;
            }

            // 1表示无变化，就不做
            if (val == 1.0000f) return bmp;

            try
            {
                Bitmap b = new Bitmap(bmp.Width, bmp.Height);
                Graphics g = Graphics.FromImage(b);
                ImageAttributes attr = new ImageAttributes();

                attr.SetGamma(val, ColorAdjustType.Bitmap);
                g.DrawImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attr);
                g.Dispose();
                return b;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 图像马赛克
        /// </summary>
        /// <param name="bmp">原图</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public static Bitmap KiMosaic(Bitmap bmp, int val)
        {
            if (bmp.Equals(null))
            {
                return null;
            }

            int w = bmp.Width;
            int h = bmp.Height;

            int stdR, stdG, stdB;

            stdR = 0;
            stdG = 0;
            stdB = 0;

            BitmapData srcData = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            unsafe
            {
                byte* p = (byte*)srcData.Scan0.ToPointer();

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        if (y % val == 0)
                        {
                            if (x % val == 0)
                            {
                                stdR = p[2]; stdG = p[1]; stdB = p[0];
                            }
                            else
                            {
                                p[0] = (byte)stdB;
                                p[1] = (byte)stdG;
                                p[2] = (byte)stdR;
                            }
                        }
                        else
                        {
                            // 复制上一行
                            byte* pTemp = p - srcData.Stride;

                            p[0] = (byte)pTemp[0];
                            p[1] = (byte)pTemp[1];
                            p[2] = (byte)pTemp[2];
                        }

                        p += 3;

                    } // end of x

                    p += srcData.Stride - w * 3;

                } // end of y

                bmp.UnlockBits(srcData);
            }

            return bmp;

        }
        /// <summary>
        /// 柔化
        /// <param name="b">原始图</param>
        /// <returns>输出图</returns>
        public static Bitmap KiBlur(Bitmap b)
        {

            if (b == null)
            {
                return null;
            }

            int w = b.Width;
            int h = b.Height;

            try
            {

                Bitmap bmpRtn = new Bitmap(w, h, PixelFormat.Format24bppRgb);

                BitmapData srcData = b.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                BitmapData dstData = bmpRtn.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

                unsafe
                {
                    byte* pIn = (byte*)srcData.Scan0.ToPointer();
                    byte* pOut = (byte*)dstData.Scan0.ToPointer();
                    int stride = srcData.Stride;
                    byte* p;

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            //取周围9点的值
                            if (x == 0 || x == w - 1 || y == 0 || y == h - 1)
                            {
                                //不做
                                pOut[0] = pIn[0];
                                pOut[1] = pIn[1];
                                pOut[2] = pIn[2];
                            }
                            else
                            {
                                int r1, r2, r3, r4, r5, r6, r7, r8, r9;
                                int g1, g2, g3, g4, g5, g6, g7, g8, g9;
                                int b1, b2, b3, b4, b5, b6, b7, b8, b9;

                                float vR, vG, vB;

                                //左上
                                p = pIn - stride - 3;
                                r1 = p[2];
                                g1 = p[1];
                                b1 = p[0];

                                //正上
                                p = pIn - stride;
                                r2 = p[2];
                                g2 = p[1];
                                b2 = p[0];

                                //右上
                                p = pIn - stride + 3;
                                r3 = p[2];
                                g3 = p[1];
                                b3 = p[0];

                                //左侧
                                p = pIn - 3;
                                r4 = p[2];
                                g4 = p[1];
                                b4 = p[0];

                                //右侧
                                p = pIn + 3;
                                r5 = p[2];
                                g5 = p[1];
                                b5 = p[0];

                                //右下
                                p = pIn + stride - 3;
                                r6 = p[2];
                                g6 = p[1];
                                b6 = p[0];

                                //正下
                                p = pIn + stride;
                                r7 = p[2];
                                g7 = p[1];
                                b7 = p[0];

                                //右下
                                p = pIn + stride + 3;
                                r8 = p[2];
                                g8 = p[1];
                                b8 = p[0];

                                //自己
                                p = pIn;
                                r9 = p[2];
                                g9 = p[1];
                                b9 = p[0];

                                vR = (float)(r1 + r2 + r3 + r4 + r5 + r6 + r7 + r8 + r9);
                                vG = (float)(g1 + g2 + g3 + g4 + g5 + g6 + g7 + g8 + g9);
                                vB = (float)(b1 + b2 + b3 + b4 + b5 + b6 + b7 + b8 + b9);

                                vR /= 9;
                                vG /= 9;
                                vB /= 9;

                                pOut[0] = (byte)vB;
                                pOut[1] = (byte)vG;
                                pOut[2] = (byte)vR;

                            }

                            pIn += 3;
                            pOut += 3;
                        }// end of x

                        pIn += srcData.Stride - w * 3;
                        pOut += srcData.Stride - w * 3;
                    } // end of y
                }

                b.UnlockBits(srcData);
                bmpRtn.UnlockBits(dstData);

                return bmpRtn;
            }
            catch
            {
                return null;
            }

        }
        /// <summary>
        /// 锐化
        /// </summary>
        /// <param name="b">原始Bitmap</param>
        /// <param name="val">锐化程度。取值[0,1]。值越大锐化程度越高</param>
        /// <returns>锐化后的图像</returns>
        public static Bitmap KiSharpen(Bitmap b, float val)
        {
            if (b == null)
            {
                return null;
            }

            int w = b.Width;
            int h = b.Height;

            try
            {

                Bitmap bmpRtn = new Bitmap(w, h, PixelFormat.Format24bppRgb);

                BitmapData srcData = b.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                BitmapData dstData = bmpRtn.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

                unsafe
                {
                    byte* pIn = (byte*)srcData.Scan0.ToPointer();
                    byte* pOut = (byte*)dstData.Scan0.ToPointer();
                    int stride = srcData.Stride;
                    byte* p;

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            //取周围9点的值。位于边缘上的点不做改变。
                            if (x == 0 || x == w - 1 || y == 0 || y == h - 1)
                            {
                                //不做
                                pOut[0] = pIn[0];
                                pOut[1] = pIn[1];
                                pOut[2] = pIn[2];
                            }
                            else
                            {
                                int r1, r2, r3, r4, r5, r6, r7, r8, r0;
                                int g1, g2, g3, g4, g5, g6, g7, g8, g0;
                                int b1, b2, b3, b4, b5, b6, b7, b8, b0;

                                float vR, vG, vB;

                                //左上
                                p = pIn - stride - 3;
                                r1 = p[2];
                                g1 = p[1];
                                b1 = p[0];

                                //正上
                                p = pIn - stride;
                                r2 = p[2];
                                g2 = p[1];
                                b2 = p[0];

                                //右上
                                p = pIn - stride + 3;
                                r3 = p[2];
                                g3 = p[1];
                                b3 = p[0];

                                //左侧
                                p = pIn - 3;
                                r4 = p[2];
                                g4 = p[1];
                                b4 = p[0];

                                //右侧
                                p = pIn + 3;
                                r5 = p[2];
                                g5 = p[1];
                                b5 = p[0];

                                //右下
                                p = pIn + stride - 3;
                                r6 = p[2];
                                g6 = p[1];
                                b6 = p[0];

                                //正下
                                p = pIn + stride;
                                r7 = p[2];
                                g7 = p[1];
                                b7 = p[0];

                                //右下
                                p = pIn + stride + 3;
                                r8 = p[2];
                                g8 = p[1];
                                b8 = p[0];

                                //自己
                                p = pIn;
                                r0 = p[2];
                                g0 = p[1];
                                b0 = p[0];

                                vR = (float)r0 - (float)(r1 + r2 + r3 + r4 + r5 + r6 + r7 + r8) / 8;
                                vG = (float)g0 - (float)(g1 + g2 + g3 + g4 + g5 + g6 + g7 + g8) / 8;
                                vB = (float)b0 - (float)(b1 + b2 + b3 + b4 + b5 + b6 + b7 + b8) / 8;

                                vR = r0 + vR * val;
                                vG = g0 + vG * val;
                                vB = b0 + vB * val;

                                if (vR > 0)
                                {
                                    vR = Math.Min(255, vR);
                                }
                                else
                                {
                                    vR = Math.Max(0, vR);
                                }

                                if (vG > 0)
                                {
                                    vG = Math.Min(255, vG);
                                }
                                else
                                {
                                    vG = Math.Max(0, vG);
                                }

                                if (vB > 0)
                                {
                                    vB = Math.Min(255, vB);
                                }
                                else
                                {
                                    vB = Math.Max(0, vB);
                                }

                                pOut[0] = (byte)vB;
                                pOut[1] = (byte)vG;
                                pOut[2] = (byte)vR;

                            }

                            pIn += 3;
                            pOut += 3;
                        }// end of x

                        pIn += srcData.Stride - w * 3;
                        pOut += srcData.Stride - w * 3;
                    } // end of y
                }

                b.UnlockBits(srcData);
                bmpRtn.UnlockBits(dstData);

                return bmpRtn;
            }
            catch
            {
                return null;
            }

        }
        /// <summary>
        /// 色彩调整
        /// </summary>
        /// <param name="bmp">原始图</param>
        /// <param name="rVal">r增量</param>
        /// <param name="gVal">g增量</param>
        /// <param name="bVal">b增量</param>
        /// <returns>处理后的图</returns>
        public static Bitmap KiColorBalance(Bitmap bmp, int rVal, int gVal, int bVal)
        {

            if (bmp == null)
            {
                return null;
            }


            int h = bmp.Height;
            int w = bmp.Width;

            try
            {
                if (rVal > 255 || rVal < -255 || gVal > 255 || gVal < -255 || bVal > 255 || bVal < -255)
                {
                    return null;
                }

                BitmapData srcData = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                unsafe
                {
                    byte* p = (byte*)srcData.Scan0.ToPointer();

                    int nOffset = srcData.Stride - w * 3;
                    int r, g, b;

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {

                            b = p[0] + bVal;
                            if (bVal >= 0)
                            {
                                if (b > 255) b = 255;
                            }
                            else
                            {
                                if (b < 0) b = 0;
                            }

                            g = p[1] + gVal;
                            if (gVal >= 0)
                            {
                                if (g > 255) g = 255;
                            }
                            else
                            {
                                if (g < 0) g = 0;
                            }

                            r = p[2] + rVal;
                            if (rVal >= 0)
                            {
                                if (r > 255) r = 255;
                            }
                            else
                            {
                                if (r < 0) r = 0;
                            }

                            p[0] = (byte)b;
                            p[1] = (byte)g;
                            p[2] = (byte)r;

                            p += 3;
                        }

                        p += nOffset;


                    }
                } // end of unsafe

                bmp.UnlockBits(srcData);

                return bmp;
            }
            catch
            {
                return null;
            }

        } // end of color
        /// <summary>
        /// 合并两张图片，支持不透明度和透明色
        /// </summary>
        /// <param name="b0">图片一</param>
        /// <param name="b1">图片二</param>
        /// <param name="X">起始坐标X</param>
        /// <param name="Y">起始坐标Y</param>
        /// <param name="b1_alpha">图片二的不透明度</param>
        /// <param name="TransColor">被作为透明色处理的颜色</param>
        /// <param name="delta">透明色的容差</param>
        /// <returns>合并后的图片</returns>
        public static Bitmap KiMerge(Bitmap b0, Bitmap b1, int X, int Y, int b1_alpha, Color TransColor, int delta)
        {
            if (b0.Equals(null) || b1.Equals(null))
            {
                return null;
            }

            int w0 = b0.Width;
            int h0 = b0.Height;

            int w1 = b1.Width;
            int h1 = b1.Height;

            int w, h;

            if (X + w1 > w0)
            {
                w = w0 - X;
            }
            else
            {
                w = w1;
            }

            if (Y + h1 > h0)
            {
                h = h0 - Y;
            }
            else
            {
                h = h1;
            }

            BitmapData srcData = b0.LockBits(new Rectangle(X, Y, w, h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData dstData = b1.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            unsafe
            {
                byte* pIn = (byte*)srcData.Scan0.ToPointer();
                byte* pLogo = (byte*)dstData.Scan0.ToPointer();

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {

                        // 判断透明色
                        Color c = Color.FromArgb(pLogo[2], pLogo[1], pLogo[0]);
                        if (!ColorIsSimilar(c, TransColor, delta))
                        {

                            float bili = (float)b1_alpha / (float)255; // 不是透明色，加权平均
                            float inbili = 1.0f - bili;

                            int r, g, b;

                            b = (int)(pIn[0] * inbili + pLogo[0] * bili);
                            g = (int)(pIn[1] * inbili + pLogo[1] * bili);
                            r = (int)(pIn[2] * inbili + pLogo[2] * bili);

                            pIn[0] = (byte)b;
                            pIn[1] = (byte)g;
                            pIn[2] = (byte)r;
                        }
                        pIn += 3;
                        pLogo += 3;
                    }


                    pIn += srcData.Stride - w * 3;
                    pLogo += dstData.Stride - w * 3;
                }

                b0.UnlockBits(srcData);
                b1.UnlockBits(dstData);
            }

            return b0;
        }
        /// <summary>
        /// 颜色是否近似
        /// </summary>
        /// <param name="c0">颜色0</param>
        /// <param name="c1">颜色1</param>
        /// <param name="delta">容差</param>
        /// <returns>是/否</returns>
        public static bool ColorIsSimilar(Color c0, Color c1, int delta)
        {
            int r0, r1, g0, g1, b0, b1;

            r0 = c0.R;
            r1 = c1.R;

            g0 = c0.G;
            g1 = c1.G;

            b0 = c0.B;
            b1 = c1.B;

            if ((r0 - r1) * (r0 - r1) + (g0 - g1) * (g0 - g1) + (b0 - b1) * (b0 - b1) <= delta * delta)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        /// <summary>
        /// Resize图片
        /// </summary>
        /// <param name="bmp">原始Bitmap</param>
        /// <param name="newW">新的宽度</param>
        /// <param name="newH">新的高度</param>
        /// <returns>处理以后的图片</returns>
        public static Bitmap KiResizeImage(Bitmap bmp, int newW, int newH)
        {
            try
            {
                Bitmap b = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(b);

                // 插值算法的质量
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                g.Dispose();

                return b;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 剪裁 -- 用GDI+
        /// </summary>
        /// <param name="b">原始Bitmap</param>
        /// <param name="StartX">开始坐标X</param>
        /// <param name="StartY">开始坐标Y</param>
        /// <param name="iWidth">宽度</param>
        /// <param name="iHeight">高度</param>
        /// <returns>剪裁后的Bitmap</returns>
        public static Bitmap KiCut(Bitmap b, int StartX, int StartY, int iWidth, int iHeight)
        {
            if (b == null)
            {
                return null;
            }

            int w = b.Width;
            int h = b.Height;

            if (StartX >= w || StartY >= h)
            {
                return null;
            }

            if (StartX + iWidth > w)
            {
                iWidth = w - StartX;
            }

            if (StartY + iHeight > h)
            {
                iHeight = h - StartY;
            }

            try
            {
                Bitmap bmpOut = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);

                Graphics g = Graphics.FromImage(bmpOut);
                g.DrawImage(b, new Rectangle(0, 0, iWidth, iHeight), new Rectangle(StartX, StartY, iWidth, iHeight), GraphicsUnit.Pixel);
                g.Dispose();

                return bmpOut;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 保存为JPEG格式，支持压缩质量选项
        /// </summary>
        /// <param name="bmp">原图</param>
        /// <param name="FileName">保存位置</param>
        /// <param name="Qty">保存质量</param>
        /// <returns></returns>
        public static bool KiSaveAsJPEG(Bitmap bmp, string FileName, int Qty)
        {
            try
            {
                EncoderParameter p;
                EncoderParameters ps;
                ps = new EncoderParameters(1);

                p = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, Qty);
                ps.Param[0] = p;

                bmp.Save(FileName, GetCodecInfo("image/jpeg"), ps);

                return true;
            }
            catch
            {
                return false;
            }

        }
        /// <summary>
        /// 保存JPG时用
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns>得到指定mimeType的ImageCodecInfo</returns>
        private static ImageCodecInfo GetCodecInfo(string mimeType)
        {
            ImageCodecInfo[] CodecInfo = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo ici in CodecInfo)
            {
                if (ici.MimeType == mimeType) return ici;
            }
            return null;
        }

        /// <summary>
        /// 边缘检测
        /// </summary>
        /// <param name="a">源图</param>
        /// <returns></returns>
        public static Bitmap robert(Bitmap a)
        {
            int w = a.Width;
            int h = a.Height;
            try
            {
                Bitmap dstBitmap = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                System.Drawing.Imaging.BitmapData srcData = a.LockBits(new Rectangle
                    (0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                System.Drawing.Imaging.BitmapData dstData = dstBitmap.LockBits(new Rectangle
                    (0, 0, w, h), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                unsafe
                {
                    byte* pIn = (byte*)srcData.Scan0.ToPointer();
                    byte* pOut = (byte*)dstData.Scan0.ToPointer();
                    byte* p;
                    int stride = srcData.Stride;
                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            //边缘八个点像素不变
                            if (x == 0 || x == w - 1 || y == 0 || y == h - 1)
                            {
                                pOut[0] = pIn[0];
                                pOut[1] = pIn[1];
                                pOut[2] = pIn[2];

                            }
                            else
                            {
                                int r0, r5, r6, r7;
                                int g5, g6, g7, g0;
                                int b5, b6, b7, b0;
                                double vR, vG, vB;
                                //右
                                p = pIn + 3;
                                r5 = p[2];
                                g5 = p[1];
                                b5 = p[0];
                                //左下
                                p = pIn + stride - 3;
                                r6 = p[2];
                                g6 = p[1];
                                b6 = p[0];
                                //正下
                                p = pIn + stride;
                                r7 = p[2];
                                g7 = p[1];
                                b7 = p[0];
                                //中心点
                                p = pIn;
                                r0 = p[2];
                                g0 = p[1];
                                b0 = p[0];
                                vR = (double)(Math.Abs(r0 - r5) + Math.Abs(r5 - r7));
                                vG = (double)(Math.Abs(g0 - g5) + Math.Abs(g5 - g7));
                                vB = (double)(Math.Abs(b0 - b5) + Math.Abs(b5 - b7));
                                if (vR > 0)
                                {
                                    vR = Math.Min(255, vR);
                                }
                                else
                                {
                                    vR = Math.Max(0, vR);
                                }

                                if (vG > 0)
                                {
                                    vG = Math.Min(255, vG);
                                }
                                else
                                {
                                    vG = Math.Max(0, vG);
                                }

                                if (vB > 0)
                                {
                                    vB = Math.Min(255, vB);
                                }
                                else
                                {
                                    vB = Math.Max(0, vB);
                                }
                                pOut[0] = (byte)vB;
                                pOut[1] = (byte)vG;
                                pOut[2] = (byte)vR;

                            }
                            pIn += 3;
                            pOut += 3;
                        }
                        pIn += srcData.Stride - w * 3;
                        pOut += srcData.Stride - w * 3;
                    }
                }
                a.UnlockBits(srcData);
                dstBitmap.UnlockBits(dstData);

                return dstBitmap;
            }
            catch
            {
                return null;
            }


        }


        /// <summary>
        /// 去噪处理
        /// </summary>
        /// <param name="image">源图</param>
        public static void ToDenoising5x5(Bitmap image)
        {
            Rectangle rectimage = new Rectangle(new Point(), image.Size);
            BitmapData data = image.LockBits(new Rectangle(new Point(), image.Size),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);//将图像锁定到内存中
            byte[] datas = new byte[data.Stride * image.Height];    //图像数组
            byte[,] a = new byte[image.Height, image.Width];
            Marshal.Copy(data.Scan0, datas, 0, datas.Length);       //将图像在内存中的数据复制到图像数组中
            for (int y = 0; y < image.Height * data.Stride; y += data.Stride)
            {
                for (int x = 0; x < image.Width * 3; x += 3)
                {
                    int index = y + x;
                    byte blue = datas[index];
                    byte green = datas[index + 1];
                    byte red = datas[index + 2];
                    a[y / data.Stride, x / 3] = datas[index] = (byte)((red * 19595 + green * 38469 + blue * 7472) >> 16);
                }
            }

            unsafe
            {
                byte* pImage = (byte*)data.Scan0.ToPointer();//获取图像头指针
                for (int i = 2; i < image.Height - 2; i++)
                {
                    for (int j = 2; j < image.Width - 2; j++)
                    {
                        byte[] aa = new byte[9];
                        aa[0] = (byte)Math.Abs((a[i, j - 1] + a[i - 1, j] + a[i, j + 1] + a[i + 1, j]) / 4 - a[i, j]);
                        aa[1] = (byte)Math.Abs((a[i - 1, j - 1] + a[i - 2, j] + a[i - 1, j] + a[i - 1, j + 1]) / 4 - a[i, j]);
                        aa[2] = (byte)Math.Abs((a[i - 1, j + 1] + a[i, j + 1] + a[i, j + 2] + a[i + 1, j + 1]) / 4 - a[i, j]);
                        aa[3] = (byte)Math.Abs((a[i + 1, j - 1] + a[i + 1, j] + a[i + 1, j + 1] + a[i + 2, j]) / 4 - a[i, j]);
                        aa[4] = (byte)Math.Abs((a[i, j - 2] + a[i - 1, j - 1] + a[i, j - 1] + a[i + 1, j - 1]) / 4 - a[i, j]);
                        aa[5] = (byte)Math.Abs((a[i - 2, j - 2] + a[i - 1, j - 1] + a[i, j - 1] + a[i - 1, j]) / 4 - a[i, j]);
                        aa[6] = (byte)Math.Abs((a[i - 1, j] + a[i - 1, j + 1] + a[i - 2, j + 2] + a[i, j + 1]) / 4 - a[i, j]);
                        aa[7] = (byte)Math.Abs((a[i + 1, j] + a[i, j + 1] + a[i + 1, j + 1] + a[i + 2, j + 2]) / 4 - a[i, j]);
                        aa[8] = (byte)Math.Abs((a[i + 2, j - 2] + a[i, j - 1] + a[i + 1, j - 1] + a[i + 1, j]) / 4 - a[i, j]);
                        Array.Sort(aa);
                        byte ab = (byte)aa.GetValue(0);

                        List<byte>[] dat = new List<byte>[3];     //创建存放8邻居像素颜色值的列表
                        for (int k = 0; k < 3; k++) dat[k] = new List<byte>();
                        for (int yy = -1; yy < 2; yy++)
                        {
                            for (int xx = -1; xx < 2; xx++)
                            {
                                int index = (i + yy) * data.Stride + (j + xx) * 3;//8邻域像素索引
                                //将8邻域像素颜色值添加到列表中
                                for (int k = 0; k < 3; k++) dat[k].Add(pImage[index + k]);
                            }
                        }
                        for (int k = 0; k < 3; k++) dat[k].Sort();    //对八邻域颜色值排序
                        int indexMedian = i * data.Stride + j * 3;
                        if (ab > a[i, j] / 5)
                        {
                            for (int k = 0; k < 3; k++)
                            {
                                pImage[indexMedian + k] = (byte)((dat[k][0] + dat[k][1] + dat[k][2] + dat[k][3] + dat[k][5] + dat[k][4] + dat[k][6] + dat[k][8] + dat[k][7]) / 9);
                                dat[k].Clear();
                            }
                            dat = null;
                        }

                    }
                }
            }
            image.UnlockBits(data);//将图像从内存中解锁

        }
        public static void ToDenoising3X3(Bitmap image)
        {
            Random random = new Random();
            for (int i = 0; i < 500; i++)
            {
                image.SetPixel(random.Next(image.Width), random.Next(image.Height)
                    , Color.FromArgb(random.Next(int.MaxValue)));//产生噪声
            }
            Rectangle rectimage = new Rectangle(new Point(), image.Size);
            //g.DrawImage(image, rectimage);//绘制噪声图像
            BitmapData data = image.LockBits(new Rectangle(new Point(), image.Size),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);//将图像锁定到内存中
            byte[] datas = new byte[data.Stride * image.Height];    //图像数组
            byte[,] a = new byte[image.Height, image.Width];
            Marshal.Copy(data.Scan0, datas, 0, datas.Length);       //将图像在内存中的数据复制到图像数组中
            for (int y = 0; y < image.Height * data.Stride; y += data.Stride)
            {
                for (int x = 0; x < image.Width * 3; x += 3)
                {
                    int index = y + x;
                    byte blue = datas[index];
                    byte green = datas[index + 1];
                    byte red = datas[index + 2];
                    a[y / data.Stride, x / 3] = datas[index] = (byte)((red * 19595 + green * 38469 + blue * 7472) >> 16);
                }
            }
            byte aa, ab, ac, ad, ae;
            unsafe
            {
                byte* pImage = (byte*)data.Scan0.ToPointer();//获取图像头指针
                for (int i = 1; i < image.Height - 1; i++)
                {
                    for (int j = 1; j < image.Width - 1; j++)
                    {
                        aa = (byte)(a[i - 1, j - 1] + a[i - 1, j] + a[i - 1, j + 1] + a[i, j - 1] - 8 * a[i, j]
                            + a[i, j + 1] + a[i + 1, j - 1] + a[i + 1, j] + a[i + 1, j + 1]);
                        ab = (byte)(-a[i - 1, j - 1] - a[i - 1, j] - a[i - 1, j + 1] + 2 * a[i, j - 1] + 2 * a[i, j]
                            + 2 * a[i, j + 1] - a[i + 1, j - 1] - a[i + 1, j] - a[i + 1, j + 1]);
                        ac = (byte)(-a[i - 1, j - 1] + 2 * a[i - 1, j] - a[i - 1, j + 1] - a[i, j - 1] + 2 * a[i, j]
                            - a[i, j + 1] - a[i + 1, j - 1] + 2 * a[i + 1, j] - a[i + 1, j + 1]);
                        ad = (byte)(2 * a[i - 1, j - 1] - a[i - 1, j] - a[i - 1, j + 1] - a[i, j - 1] + 2 * a[i, j]
                            - a[i, j + 1] - a[i + 1, j - 1] - a[i + 1, j] + 2 * a[i + 1, j + 1]);
                        ae = (byte)(-a[i - 1, j - 1] - a[i - 1, j] + 2 * a[i - 1, j + 1] - a[i, j - 1] + 2 * a[i, j]
                            - a[i, j + 1] + 2 * a[i + 1, j - 1] - a[i + 1, j] - a[i + 1, j + 1]);
                        List<byte>[] dat = new List<byte>[3];     //创建存放8邻居像素颜色值的列表
                        for (int k = 0; k < 3; k++) dat[k] = new List<byte>();
                        for (int yy = -1; yy < 2; yy++)
                        {
                            for (int xx = -1; xx < 2; xx++)
                            {
                                int index = (i + yy) * data.Stride + (j + xx) * 3;//8邻域像素索引
                                //将8邻域像素颜色值添加到列表中
                                for (int k = 0; k < 3; k++) dat[k].Add(pImage[index + k]);
                            }
                        }
                        for (int k = 0; k < 3; k++) dat[k].Sort();    //对八邻域颜色值排序
                        int indexMedian = i * data.Stride + j * 3;
                        if (aa > ab && aa > ac && aa > ad && aa > ae)
                        {
                            for (int k = 0; k < 3; k++)
                            {
                                pImage[indexMedian + k] = (byte)((dat[k][3] + dat[k][5] + dat[k][4]) / 3);
                                dat[k].Clear();
                            }
                            dat = null;
                        }
                    }
                }
            }
            image.UnlockBits(data);//将图像从内存中解锁
            rectimage.Offset(0, rectimage.Height);

        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="Bmp"></param>
       /// <param name="Max">最大值</param>
       /// <param name="Min">最小值</param>
       /// <param name="isBack">最大值与最小值之间的颜色</param>
        public static void SetThreshold(Bitmap Bmp, int Max, int Min, bool isBack)
        {
            if (Bmp.PixelFormat == PixelFormat.Format32bppArgb)
            {
                BitmapData BmpData = Bmp.LockBits(new Rectangle(0, 0, Bmp.Width, Bmp.Height), ImageLockMode.ReadOnly, Bmp.PixelFormat);
                unsafe
                {
                    int R = Bmp.Width;
                    if (R < Bmp.Height)
                        R = Bmp.Height;
                    R = R / 2;
                    Point p1 = new Point(Bmp.Width / 2, Bmp.Height / 2);
                    Parallel.ForEach(Partitioner.Create(0, BmpData.Height), (H) =>
                    {

                        int X, Y, Width, Height, Stride;
                        byte Red;
                        byte* Scan0, CurP;

                        Width = BmpData.Width;
                        Height = BmpData.Height;
                        Stride = BmpData.Stride;
                        Scan0 = (byte*)BmpData.Scan0;

                        for (Y = H.Item1; Y < H.Item2; Y++)
                        {
                            CurP = Scan0 + Y * Stride;
                            for (X = 0; X < Width; X++)
                            {
                             //   Red = (byte)((float)*(CurP) * 0.114f + (float)*(CurP + 1) * 0.587f + (float)*(CurP + 2) * 0.299f);
                                Red = (byte)((*(CurP) * 19595 + *(CurP + 1) * 38469 + *(CurP + 2) * 7472) >> 16);
                                Point p2 = new Point(X, Y);
                                double JL = Distance(p1,p2);
                                if (Red < Max && Red > Min && R > JL)
                                {
                                    if (isBack)
                                    {
                                        *(CurP) = 0;
                                        *(CurP + 1) = 0;
                                        *(CurP + 2) = 0;
                                    }
                                    else
                                    {
                                        *(CurP) = 255;
                                        *(CurP + 1) = 255;
                                        *(CurP + 2) = 255;
                                    }

                                }
                                else
                                {
                                    if (!isBack)
                                    {
                                        *(CurP) = 0;
                                        *(CurP + 1) = 0;
                                        *(CurP + 2) = 0;
                                    }
                                    else
                                    {
                                        *(CurP) = 255;
                                        *(CurP + 1) = 255;
                                        *(CurP + 2) = 255;
                                    }

                                }




                                CurP += 4;
                            }
                        }
                    });
                }
                Bmp.UnlockBits(BmpData);
            }
        }
        /// <summary>
        /// 两点间的距离
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double Distance(Point p1,Point p2)
        {
            int xdiff = p2.X - p1.X;
            int ydiff = p2.Y - p1.Y;

            return Math.Sqrt(xdiff * xdiff + ydiff * ydiff);

        }

        /// 亮度 对比一次性调整
        /// </summary>
        /// <param name="bmp">需要调的图</param>
        /// <param name="brightvalue">亮度</param>
        /// <param name="contrastvalue">对比度</param>
        /// <returns></returns>
        public static Bitmap BMPBrightContrast(Bitmap bmp, int brightvalue, int contrastvalue)
        {
            return SetBrightContrast(bmp, SimpleRect(bmp.Size), brightvalue, contrastvalue);

        }
        static Rectangle SimpleRect(Size Sz)
        {
            return new Rectangle(0, 0, Sz.Width, Sz.Height);
        }
        /// <summary>
        /// 亮度 对比一次性调整
        /// </summary>
        /// <param name="bmp">需要调的图</param>
        /// <param name="Rect">需要调的地方</param>
        /// <param name="brightvalue">高度</param>
        /// <param name="contrastvalue">对比度</param>
        /// <returns></returns>
        public static Bitmap SetBrightContrast(Bitmap bmp, Rectangle Rect, int brightvalue, int contrastvalue)
        {
            Bitmap bmpTemp = (Bitmap)bmp.Clone();
            if (brightvalue == 0 && contrastvalue == 0)
                return bmpTemp;
            int Grade = 0;
            double contrast = (100.0 + contrastvalue) / 100.0;
            contrast *= contrast;

            double ContrastGrade = 0;

            Rectangle rectbmp = Rect;
            BitmapData bmpData = bmpTemp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr Scan0 = bmpData.Scan0;

            try
            {
                unsafe
                {
                    byte* scan0 = (byte*)(void*)Scan0;
                    byte* pucPtr;
                    byte* pucStart;

                    int xmin = rectbmp.X;
                    int ymin = rectbmp.Y;
                    int xmax = xmin + rectbmp.Width;
                    int ymax = ymin + rectbmp.Height;

                    int x = xmin;
                    int y = ymin;
                    int iStride = bmpData.Stride;

                    y = ymin;
                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                    while (y < ymax)
                    {
                        x = xmin;
                        pucPtr = pucStart;
                        while (x < xmax)
                        {
                            Grade = (int)pucPtr[2];

                            if (brightvalue != 0)
                            {
                                Grade += brightvalue;
                                Grade = Math.Min(255, Math.Max(0, Grade));
                            }

                            ContrastGrade = (double)Grade;

                            if (contrastvalue != 0)
                            {
                                ContrastGrade /= 255d;
                                ContrastGrade -= 0.5;
                                ContrastGrade *= (double)contrast;
                                ContrastGrade += 0.5;
                                ContrastGrade *= 255;

                                ContrastGrade = Math.Min(255, Math.Max(0, ContrastGrade));
                            }

                            pucPtr[2] = (byte)ContrastGrade;
                            pucPtr[1] = (byte)ContrastGrade;
                            pucPtr[0] = (byte)ContrastGrade;

                            pucPtr += 4;
                            x++;
                        }

                        pucStart += iStride;
                        y++;
                    }

                    bmpTemp.UnlockBits(bmpData);
                    return bmpTemp;
                }
            }
            catch (Exception e)
            {
                string Str = e.ToString();

                bmpTemp.UnlockBits(bmpData);
            }

            return bmpTemp;
        }

        public static void Desaturate(Bitmap Bmp)
        {
            if (Bmp.PixelFormat == PixelFormat.Format32bppArgb)
            {
                BitmapData BmpData = Bmp.LockBits(new Rectangle(0, 0, Bmp.Width, Bmp.Height), ImageLockMode.ReadOnly, Bmp.PixelFormat);
                unsafe
                {

                    Parallel.ForEach(Partitioner.Create(0, BmpData.Height), (H) =>
                    {
                        int X, Y, Width, Height, Stride;
                        byte Red, Green, Blue, Max, Min, Value;
                        byte* Scan0, CurP;
                        Width = BmpData.Width; Height = BmpData.Height; Stride = BmpData.Stride; Scan0 = (byte*)BmpData.Scan0;
                        for (Y = H.Item1; Y < H.Item2; Y++)
                        {
                            CurP = Scan0 + Y * Stride;
                            for (X = 0; X < Width; X++)
                            {
                                Blue = *CurP;
                                Green = *(CurP + 1);
                                Red = *(CurP + 2);

                                if (Blue > Green)
                                {
                                    Max = Blue;
                                    Min = Green;
                                }
                                else
                                {
                                    Max = Green;
                                    Min = Blue;
                                }
                                if (Red > Max)
                                    Max = Red;
                                else if (Red < Min)
                                    Min = Red;
                                Value = (byte)((Max + Min) >> 1);
                                *CurP = Value; *(CurP + 1) = Value; *(CurP + 2) = Value;
                                CurP += 4;



                            }
                        }
                    });
                }
                Bmp.UnlockBits(BmpData);
            }
        }
        /// <summary>
        /// 亮度对比调整（RGB都做调整）
        /// </summary>
        /// <param name="bmp">需要调的图</param>
        /// <param name="bright">高度</param>
        /// <param name="contrast">对比度</param>
        public static void SetBrightContrastRGB(Bitmap Bmp, int bright, int contrast)
        {
            if (Bmp.PixelFormat == PixelFormat.Format32bppArgb)
            {
                BitmapData BmpData = Bmp.LockBits(new Rectangle(0, 0, Bmp.Width, Bmp.Height), ImageLockMode.ReadOnly, Bmp.PixelFormat);
                unsafe
                {
                    if (bright < -255)
                        bright = -255;
                    if (bright > 255)
                        bright = 255;

                    if (contrast < -100)
                        contrast = -100;
                    if (contrast > 100)
                        contrast = 100;
                    double contrastT = (100.0 + contrast) / 100.0;
                    contrastT *= contrastT;

                    Parallel.ForEach(Partitioner.Create(0, BmpData.Height), (H) =>
                    {

                        int X, Y, Width, Height, Stride, pixB = 0, pixG = 0, pixR = 0;
                        byte Red, Green, Blue;
                        byte* Scan0, CurP;

                        double pixelR = 0, pixelG = 0, pixelB = 0;
                        Width = BmpData.Width;
                        Height = BmpData.Height;
                        Stride = BmpData.Stride;
                        Scan0 = (byte*)BmpData.Scan0;

                        for (Y = H.Item1; Y < H.Item2; Y++)
                        {
                            CurP = Scan0 + Y * Stride;
                            for (X = 0; X < Width; X++)
                            {
                                Blue = *CurP;
                                Green = *(CurP + 1);
                                Red = *(CurP + 2);

                                #region 亮度处理
                                if (bright != 0)
                                {
                                    pixR = Red + bright;
                                    pixG = Green + bright;
                                    pixB = Blue + bright;
                                    if (bright < 0)
                                    {
                                        if (pixR < -255)
                                            pixR = -255;
                                        if (pixG < 0)
                                            pixG = 0;
                                        if (pixB < 0)
                                            pixB = 0;
                                    }
                                    if (bright > 0)
                                    {
                                        if (pixR > 255)
                                            pixR = 255;
                                        if (pixG > 255)
                                            pixG = 255;
                                        if (pixG > 255)
                                            pixG = 255;
                                    }
                                    Red = (byte)pixR;
                                    Green = (byte)pixG;
                                    Blue = (byte)pixB;

                                }
                                #endregion

                                #region 对比度处理
                                if (contrast != 0)
                                {
                                    pixelR = ((Red / 255.0 - 0.5) * contrastT + 0.5) * 255;
                                    if (pixelR < 0)
                                        pixelR = 0;
                                    if (pixelR > 255)
                                        pixelR = 255;
                                    Red = (byte)pixelR;

                                    pixelG = ((Green / 255.0 - 0.5) * contrastT + 0.5) * 255;
                                    if (pixelG < 0)
                                        pixelG = 0;
                                    if (pixelG > 255)
                                        pixelG = 255;
                                    Green = (byte)pixelG;

                                    pixelB = ((Blue / 255.0 - 0.5) * contrastT + 0.5) * 255;
                                    if (pixelB < 0)
                                        pixelB = 0;
                                    if (pixelB > 255)
                                        pixelB = 255;
                                    Blue = (byte)pixelB;
                                }
                                #endregion

                                *CurP = Blue;
                                *(CurP + 1) = Green;
                                *(CurP + 2) = Red;

                                CurP += 4;

                            }
                        }
                    });
                }
                Bmp.UnlockBits(BmpData);
            }
        }
        /// <summary>
        /// 亮度对比调整（GRB用平均值来调整）
        /// </summary>
        /// <param name="bmp">需要调的图</param>
        /// <param name="bright">高度</param>
        /// <param name="contrast">对比度</param>
        public static void SetBrightContrastR(Bitmap Bmp, int bright, int contrast)
        {
            if (bright == 0 && contrast == 0)
                return;
            if (Bmp.PixelFormat == PixelFormat.Format32bppArgb)
            {
                SetBrightContrast32Bit(Bmp, bright, contrast);
                return;
            }
            else if (Bmp.PixelFormat == PixelFormat.Format24bppRgb)
            {
                BitmapData BmpData = Bmp.LockBits(new Rectangle(0, 0, Bmp.Width, Bmp.Height), ImageLockMode.ReadOnly, Bmp.PixelFormat);
                unsafe
                {
                    if (bright < -255)
                        bright = -255;
                    if (bright > 255)
                        bright = 255;

                    if (contrast < -100)
                        contrast = -100;
                    if (contrast > 100)
                        contrast = 100;
                    double contrastT = (100.0 + contrast) / 100.0;
                    contrastT *= contrastT;

                    Parallel.ForEach(Partitioner.Create(0, BmpData.Height), (H) =>
                    {

                        int X, Y, Width, Height, Stride, pixR = 0;
                        byte Red;
                        byte* Scan0, CurP;

                        double pixelR = 0;
                        Width = BmpData.Width;
                        Height = BmpData.Height;
                        Stride = BmpData.Stride;
                        Scan0 = (byte*)BmpData.Scan0;

                        for (Y = H.Item1; Y < H.Item2; Y++)
                        {
                            CurP = Scan0 + Y * Stride;
                            for (X = 0; X < Width; X++)
                            {
                                //  gray = (pIn[0] * 19595 + pIn[1] * 38469 + pIn[2] * 7472) >> 16; 
                                // Red = (byte)((*(CurP + 2) + *(CurP + 1) + *CurP) / 3);

                                Red = (byte)((float)*CurP * 0.114f + (float)*(CurP + 1) * 0.587f + (float)*(CurP + 2) * 0.299f);
                                Red = (byte)((*CurP * 19595 + *(CurP + 1) * 38469 + *(CurP + 2) * 7472) >> 16);

                                #region 亮度处理
                                if (bright != 0)
                                {
                                    pixR = Red + bright;
                                    if (bright < 0)
                                    {
                                        if (pixR < -255)
                                            pixR = -255;
                                    }
                                    if (bright > 0)
                                    {
                                        if (pixR > 255)
                                            pixR = 255;
                                    }
                                    Red = (byte)pixR;

                                }
                                #endregion

                                #region 对比度处理
                                if (contrast != 0)
                                {
                                    pixelR = ((Red / 255.0 - 0.5) * contrastT + 0.5) * 255;

                                    if (pixelR < -255)
                                        pixelR = -255;
                                    if (pixelR > 255)
                                        pixelR = 255;


                                    if (contrast == 255)
                                    {
                                        if (pixelR < 127)
                                            pixelR = -255;
                                        if (pixelR >= 127)
                                            pixelR = 255;
                                    }
                                    Red = (byte)pixelR;

                                }
                                #endregion
                                *CurP = Red;
                                *(CurP + 1) = Red;
                                *(CurP + 2) = Red;

                                CurP += 3;
                            }
                        }
                    });
                }
                Bmp.UnlockBits(BmpData);
            }
        }
        /// <summary>
        /// 亮度对比调整（GRB用平均值来调整）
        /// </summary>
        /// <param name="bmp">需要调的图</param>
        /// <param name="bright">高度</param>
        /// <param name="contrast">对比度</param>
         static void SetBrightContrast32Bit(Bitmap Bmp, int bright, int contrast)
        {
            if (Bmp.PixelFormat == PixelFormat.Format32bppArgb)
            {
                BitmapData BmpData = Bmp.LockBits(new Rectangle(0, 0, Bmp.Width, Bmp.Height), ImageLockMode.ReadOnly, Bmp.PixelFormat);
                unsafe
                {
                    if (bright < -255)
                        bright = -255;
                    if (bright > 255)
                        bright = 255;

                    if (contrast < -100)
                        contrast = -100;
                    if (contrast > 100)
                        contrast = 100;
                    double contrastT = (100.0 + contrast) / 100.0;
                    contrastT *= contrastT;

                    Parallel.ForEach(Partitioner.Create(0, BmpData.Height), (H) =>
                    {

                        int X, Y, Width, Height, Stride, pixR = 0;
                        byte Red;
                        byte* Scan0, CurP;

                        double pixelR = 0;
                        Width = BmpData.Width;
                        Height = BmpData.Height;
                        Stride = BmpData.Stride;
                        Scan0 = (byte*)BmpData.Scan0;

                        for (Y = H.Item1; Y < H.Item2; Y++)
                        {
                            CurP = Scan0 + Y * Stride;
                            for (X = 0; X < Width; X++)
                            {
                                //  gray = (pIn[0] * 19595 + pIn[1] * 38469 + pIn[2] * 7472) >> 16; 
                                // Red = (byte)((*(CurP + 2) + *(CurP + 1) + *CurP) / 3);

                                Red = (byte)((float)*(CurP) * 0.114f + (float)*(CurP + 1) * 0.587f + (float)*(CurP + 2) * 0.299f);
                                Red = (byte)((*(CurP ) * 19595 + *(CurP + 1) * 38469 + *(CurP + 2) * 7472) >> 16);

                                #region 亮度处理
                                if (bright != 0)
                                {
                                    pixR = Red + bright;
                                    if (bright < 0)
                                    {
                                        if (pixR < -255)
                                            pixR = -255;
                                    }
                                    if (bright > 0)
                                    {
                                        if (pixR > 255)
                                            pixR = 255;
                                    }
                                    Red = (byte)pixR;

                                }
                                #endregion

                                #region 对比度处理
                                if (contrast != 0)
                                {
                                    pixelR = ((Red / 255.0 - 0.5) * contrastT + 0.5) * 255;

                                    if (pixelR < -255)
                                        pixelR = -255;
                                    if (pixelR > 255)
                                        pixelR = 255;


                                    if (contrast == 255)
                                    {
                                        if (pixelR < 127)
                                            pixelR = -255;
                                        if (pixelR >= 127)
                                            pixelR = 255;
                                    }
                                    Red = (byte)pixelR;

                                }
                                #endregion
                                *(CurP) = Red;
                                *(CurP + 1) = Red;
                                *(CurP + 2) = Red;

                                CurP += 4;
                            }
                        }
                    });
                }
                Bmp.UnlockBits(BmpData);
            }
        }
        /// <summary>
        /// 亮度对比调整（GRB用平均值来调整）
        /// </summary>
        /// <param name="bmp">需要调的图</param>
        /// <param name="bright">高度</param>
        /// <param name="contrast">对比度</param>
        /// /// <param name="ismode">是否为加权平均算法（True) 平均算法(False)，</param>
        public static void SetBrightContrastR(Bitmap Bmp, int bright, int contrast, bool ismode)
        {
            if (Bmp.PixelFormat == PixelFormat.Format24bppRgb)
            {
                BitmapData BmpData = Bmp.LockBits(new Rectangle(0, 0, Bmp.Width, Bmp.Height), ImageLockMode.ReadOnly, Bmp.PixelFormat);
                unsafe
                {
                    if (bright < -255)
                        bright = -255;
                    if (bright > 255)
                        bright = 255;

                    if (contrast < -100)
                        contrast = -100;
                    if (contrast > 100)
                        contrast = 100;
                    double contrastT = (100.0 + contrast) / 100.0;
                    contrastT *= contrastT;

                    Parallel.ForEach(Partitioner.Create(0, BmpData.Height), (H) =>
                    {

                        int X, Y, Width, Height, Stride, pixR = 0;
                        byte Red;
                        byte* Scan0, CurP;

                        double pixelR = 0;
                        Width = BmpData.Width;
                        Height = BmpData.Height;
                        Stride = BmpData.Stride;
                        Scan0 = (byte*)BmpData.Scan0;

                        for (Y = H.Item1; Y < H.Item2; Y++)
                        {
                            CurP = Scan0 + Y * Stride;
                            for (X = 0; X < Width; X++)
                            {
                                //  gray = (pIn[0] * 19595 + pIn[1] * 38469 + pIn[2] * 7472) >> 16; 
                                // Red = (byte)((*(CurP + 2) + *(CurP + 1) + *CurP) / 3);

                                if (ismode)
                                    Red = (byte)((float)*CurP * 0.114f + (float)*(CurP + 1) * 0.587f + (float)*(CurP + 2) * 0.299f);
                                else
                                    Red = (byte)((*CurP * 19595 + *(CurP + 1) * 38469 + *(CurP + 2) * 7472) >> 16);

                                #region 亮度处理
                                if (bright != 0)
                                {
                                    pixR = Red + bright;
                                    if (bright < 0)
                                    {
                                        if (pixR < -255)
                                            pixR = -255;
                                    }
                                    if (bright > 0)
                                    {
                                        if (pixR > 255)
                                            pixR = 255;
                                    }
                                    Red = (byte)pixR;

                                }
                                #endregion

                                #region 对比度处理
                                if (contrast != 0)
                                {
                                    pixelR = ((Red / 255.0 - 0.5) * contrastT + 0.5) * 255;

                                    if (pixelR < -255)
                                        pixelR = -255;
                                    if (pixelR > 255)
                                        pixelR = 255;
                                    Red = (byte)pixelR;

                                }
                                #endregion
                                *CurP = Red;
                                *(CurP + 1) = Red;
                                *(CurP + 2) = Red;

                                CurP += 3;
                            }
                        }
                    });
                }
                Bmp.UnlockBits(BmpData);
            }
        }
        /// <summary>
        /// 亮度对比调整 16位图（GRB用平均值来调整）
        /// </summary>
        /// <param name="bmp">需要调的图</param>
        /// <param name="bright">高度</param>
        /// <param name="contrast">对比度</param>
        public static void SetBrightContrastR_16Bit(Bitmap Bmp, int bright, int contrast)
        {
            if (Bmp.PixelFormat == PixelFormat.Format16bppGrayScale)
            {
                BitmapData BmpData = Bmp.LockBits(new Rectangle(0, 0, Bmp.Width, Bmp.Height), ImageLockMode.ReadOnly, Bmp.PixelFormat);
                unsafe
                {
                    if (bright < -255)
                        bright = -255;
                    if (bright > 255)
                        bright = 255;

                    if (contrast < -100)
                        contrast = -100;
                    if (contrast > 100)
                        contrast = 100;
                    double contrastT = (100.0 + contrast) / 100.0;
                    contrastT *= contrastT;

                    Parallel.ForEach(Partitioner.Create(0, BmpData.Height), (H) =>
                    {

                        int X, Y, Width, Height, Stride, pixR = 0;
                        byte Red;
                        byte* Scan0, CurP;

                        double pixelR = 0;
                        Width = BmpData.Width;
                        Height = BmpData.Height;
                        Stride = BmpData.Stride;
                        Scan0 = (byte*)BmpData.Scan0;

                        for (Y = H.Item1; Y < H.Item2; Y++)
                        {
                            CurP = Scan0 + Y * Stride;
                            for (X = 0; X < Width; X++)
                            {
                                Red = (byte)*CurP;

                                #region 亮度处理
                                if (bright != 0)
                                {
                                    pixR = Red + bright;
                                    if (bright < 0)
                                    {
                                        if (pixR < 0)
                                            pixR = 0;
                                    }
                                    if (bright > 0)
                                    {
                                        if (pixR > 65535)
                                            pixR = 65535;
                                    }
                                    Red = (byte)pixR;

                                }
                                #endregion

                                #region 对比度处理
                                if (contrast != 0)
                                {
                                    pixelR = ((Red / 255.0 - 0.5) * contrastT + 0.5) * 255;
                                    if (pixelR < 0)
                                        pixelR = 0;
                                    if (pixelR > 65535)
                                        pixelR = 65535;
                                    Red = (byte)pixelR;

                                }
                                #endregion
                                *CurP = Red;

                                CurP += 1;
                            }
                        }
                    });
                }
                Bmp.UnlockBits(BmpData);
            }
        }
        /// <summary>
        /// 亮度对比调整（GRB用平均值来调整）
        /// </summary>
        /// <param name="bmp">需要调的图</param>
        /// <param name="bright">高度</param>
        /// <param name="contrast">对比度</param>
        public static void SetBrightContrastR_For32Bit(Bitmap Bmp, int bright, int contrast, List<Point> list)
        {
            if (Bmp.PixelFormat == PixelFormat.Format32bppArgb)
            {
                BitmapData BmpData = Bmp.LockBits(new Rectangle(0, 0, Bmp.Width, Bmp.Height), ImageLockMode.ReadOnly, Bmp.PixelFormat);
                unsafe
                {
                    if (bright < -255)
                        bright = -255;
                    if (bright > 255)
                        bright = 255;

                    if (contrast < -100)
                        contrast = -100;
                    if (contrast > 100)
                        contrast = 100;
                    double contrastT = (100.0 + contrast) / 100.0;
                    contrastT *= contrastT;

                    int inumber = 0;

                    Parallel.ForEach(list, lis =>
                      {

                          //});

                          //Parallel.ForEach(Partitioner.Create(0, BmpData.Height), (H) =>
                          //{

                          inumber++;

                          int X, Y, Width, Height, Stride, pixB = 0, pixG = 0, pixR = 0;
                          byte Red, Green, Blue;
                          byte* Scan0, CurP;

                          double pixelR = 0, pixelG = 0, pixelB = 0;
                          Width = BmpData.Width;
                          Height = BmpData.Height;
                          Stride = BmpData.Stride;
                          Scan0 = (byte*)BmpData.Scan0;

                          for (Y = lis.X; Y < lis.Y; Y++)
                          {
                              CurP = Scan0 + Y * Stride;
                              for (X = 0; X < Width; X++)
                              {
                                  // Red = *CurP;
                                  //Green = *(CurP + 1);
                                  Red = (byte)((*(CurP + 2) + *(CurP + 1) + *CurP) / 3);

                                  #region 亮度处理
                                  if (bright != 0)
                                  {
                                      pixR = Red + bright;
                                      //pixG = Green + bright;
                                      //pixB = Blue + bright;
                                      if (bright < 0)
                                      {
                                          if (pixR < -255)
                                              pixR = -255;
                                          //if (pixG < 0)
                                          //    pixG = 0;
                                          //if (pixB < 0)
                                          //    pixB = 0;
                                      }
                                      if (bright > 0)
                                      {
                                          if (pixR > 255)
                                              pixR = 255;
                                          //if (pixG > 255)
                                          //    pixG = 255;
                                          //if (pixG > 255)
                                          //    pixG = 255;
                                      }
                                      Red = (byte)pixR;
                                      //Green = (byte)pixG;
                                      //Blue = (byte)pixB;

                                  }
                                  #endregion

                                  #region 对比度处理
                                  if (contrast != 0)
                                  {
                                      pixelR = ((Red / 255.0 - 0.5) * contrastT + 0.5) * 255;
                                      if (pixelR < 0)
                                          pixelR = 0;
                                      if (pixelR > 255)
                                          pixelR = 255;
                                      Red = (byte)pixelR;

                                      //pixelG = ((Green / 255.0 - 0.5) * contrastT + 0.5) * 255;
                                      //if (pixelG < 0)
                                      //    pixelG = 0;
                                      //if (pixelG > 255)
                                      //    pixelG = 255;
                                      //Green = (byte)pixelG;

                                      //pixelB = ((Blue / 255.0 - 0.5) * contrastT + 0.5) * 255;
                                      //if (pixelB < 0)
                                      //    pixelB = 0;
                                      //if (pixelB > 255)
                                      //    pixelB = 255;
                                      //Blue = (byte)pixelB;
                                  }
                                  #endregion
                                  *CurP = Red;
                                  *(CurP + 1) = Red;
                                  *(CurP + 2) = Red;

                                  //*CurP = Blue;
                                  //*(CurP + 1) = Green;
                                  //*(CurP + 2) = Red;

                                  CurP += 4;

                              }
                          }
                      });
                }
                Bmp.UnlockBits(BmpData);
            }
        }

        /// <summary>
        /// 图像对比度调整
        /// </summary>
        /// <param name="b">原始图</param>
        /// <param name="degree">对比度[-100, 100]</param>
        /// <returns></returns>
        public static Bitmap KiContrast(Bitmap b, int degree)
        {
            if (b == null)
            {
                return null;
            }

            if (degree < -100) degree = -100;
            if (degree > 100) degree = 100;

            try
            {

                double pixel = 0;
                double contrast = (100.0 + degree) / 100.0;
                contrast *= contrast;
                int width = b.Width;
                int height = b.Height;
                BitmapData data = b.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                unsafe
                {
                    byte* p = (byte*)data.Scan0;
                    int offset = data.Stride - width * 3;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            // 处理指定位置像素的对比度
                            for (int i = 0; i < 3; i++)
                            {
                                pixel = ((p[i] / 255.0 - 0.5) * contrast + 0.5) * 255;

                                if (pixel < 0) pixel = 0;
                                if (pixel > 255) pixel = 255;
                                if (degree >= 100)
                                {
                                    if (pixel < 127) pixel = 0;
                                    if (pixel >= 127) pixel = 255;
                                }
                                p[i] = (byte)pixel;
                            } // i
                            p += 3;
                        } // x
                        p += offset;
                    } // y
                }
                b.UnlockBits(data);
                return b;
            }
            catch
            {
                return null;
            }
        } // end of Contrast
        /// <summary>
        /// 定义对比度调整函数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Bitmap KiContrastP(Bitmap a, double v)
        {
            System.Drawing.Imaging.BitmapData bmpData = a.LockBits(new Rectangle(0, 0, a.Width, a.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            int bytes = a.Width * a.Height * 3;
            IntPtr ptr = bmpData.Scan0;
            int stride = bmpData.Stride;
            unsafe
            {
                byte* p = (byte*)ptr;
                int temp;
                for (int j = 0; j < a.Height; j++)
                {
                    for (int i = 0; i < a.Width * 3; i++)
                    {
                        temp = (int)((p[0] - 127) * v + 127);
                        temp = (temp > 255) ? 255 : temp < 0 ? 0 : temp;
                        p[0] = (byte)temp;
                        p++;
                    }
                    p += stride - a.Width * 3;
                }
            }
            a.UnlockBits(bmpData);
            return a;
        }
        /// <summary>
        /// A-B
        /// </summary>
        /// <param name="BmpA">第一张图</param>
        /// <param name="BmpB">第二张图</param>
        /// <param name="Value">差异范围</param>
        public static void SetBimap_A_B(Bitmap BmpA, Bitmap BmpB, int Value)
        {
            if (BmpB.PixelFormat == PixelFormat.Format24bppRgb &&
                BmpA.PixelFormat == PixelFormat.Format24bppRgb)
            {
                BitmapData BmpDataA = BmpA.LockBits(new Rectangle(0, 0, BmpA.Width, BmpA.Height), ImageLockMode.ReadOnly, BmpA.PixelFormat);
                BitmapData BmpDataB = BmpB.LockBits(new Rectangle(0, 0, BmpB.Width, BmpB.Height), ImageLockMode.ReadOnly, BmpB.PixelFormat);

                unsafe
                {
                    Parallel.ForEach(Partitioner.Create(0, BmpDataA.Height), (H) =>
                    {

                        int X, Y, Width, Height, Stride, pixR = 0;
                        int PointA, PointB;
                        byte* Scan0, CurP, Scan0B, CurPB;

                        double pixelR = 0;
                        Width = BmpDataA.Width;
                        Height = BmpDataA.Height;
                        Stride = BmpDataA.Stride;
                        Scan0 = (byte*)BmpDataA.Scan0;
                        Scan0B = (byte*)BmpDataB.Scan0;

                        for (Y = H.Item1; Y < H.Item2; Y++)
                        {
                            CurP = Scan0 + Y * Stride;
                            CurPB = Scan0B + Y * Stride;
                            for (X = 0; X < Width; X++)
                            {
                                //  gray = (pIn[0] * 19595 + pIn[1] * 38469 + pIn[2] * 7472) >> 16; 
                                // Red = (byte)((*(CurP + 2) + *(CurP + 1) + *CurP) / 3);

                                PointA = (int)((float)*CurP + (float)*(CurP + 1) + (float)*(CurP + 2));
                                PointB = (int)((float)*CurPB + (float)*(CurPB + 1) + (float)*(CurPB + 2));

                                if (Math.Abs(PointA - PointB) > Value)
                                {
                                    *CurP = 255;
                                    *(CurP + 1) = 0;
                                    *(CurP + 2) = 0;
                                }

                                CurP += 3;
                                CurPB += 3;
                            }
                        }
                    });
                }
                BmpA.UnlockBits(BmpDataA);
            }
        }
        /// <summary>
        /// A-B
        /// </summary>
        /// <param name="BmpA">第一张图</param>
        /// <param name="BmpB">第二张图</param>
        /// <param name="Value">差异范围</param>
        /// <param name="iArea">差异面积</param>
        public static void SetBimap_A_B(Bitmap BmpA, Bitmap BmpB, int Value,ref int iArea)
        {
           int  iAreaTemp = 0;
            if (BmpB.PixelFormat == PixelFormat.Format32bppArgb &&
               BmpA.PixelFormat == PixelFormat.Format32bppArgb)
            {
                SetBimap_A_BFormat32(BmpA, BmpB, Value, ref iArea);
                return;
            }
            if (BmpB.PixelFormat == PixelFormat.Format8bppIndexed &&
                BmpA.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                SetBimap_A_BFormat8(BmpA, BmpB, Value, ref iArea);
                return;
            }
            if (BmpB.PixelFormat == PixelFormat.Format24bppRgb &&
                BmpA.PixelFormat == PixelFormat.Format24bppRgb)
            {
                BitmapData BmpDataA = BmpA.LockBits(new Rectangle(0, 0, BmpA.Width, BmpA.Height), ImageLockMode.ReadOnly, BmpA.PixelFormat);
                BitmapData BmpDataB = BmpB.LockBits(new Rectangle(0, 0, BmpB.Width, BmpB.Height), ImageLockMode.ReadOnly, BmpB.PixelFormat);

                unsafe
                {
                    Parallel.ForEach(Partitioner.Create(0, BmpDataA.Height), (H) =>
                    {

                        int X, Y, Width, Height, Stride, pixR = 0;
                        int PointA, PointB;
                        byte* Scan0, CurP, Scan0B, CurPB;

                        double pixelR = 0;
                        Width = BmpDataA.Width;
                        Height = BmpDataA.Height;
                        Stride = BmpDataA.Stride;
                        Scan0 = (byte*)BmpDataA.Scan0;
                        Scan0B = (byte*)BmpDataB.Scan0;

                        for (Y = H.Item1; Y < H.Item2; Y++)
                        {
                            CurP = Scan0 + Y * Stride;
                            CurPB = Scan0B + Y * Stride;
                            for (X = 0; X < Width; X++)
                            {
                                //  gray = (pIn[0] * 19595 + pIn[1] * 38469 + pIn[2] * 7472) >> 16; 
                                // Red = (byte)((*(CurP + 2) + *(CurP + 1) + *CurP) / 3);

                                //PointA = (int)((float)*CurP + (float)*(CurP + 1) + (float)*(CurP + 2));
                                //PointB = (int)((float)*CurPB + (float)*(CurPB + 1) + (float)*(CurPB + 2));
                                PointA = (int)((float)*CurP);
                                PointB = (int)((float)*CurPB);
                                // if (Math.Abs(PointA - PointB) < Value)
                                if (Math.Abs(PointA - PointB) > Value)
                                {
                                    iAreaTemp++;
                                    *CurP = 255;
                                    *(CurP + 1) = 255;
                                    *(CurP + 2) = 255;
                                }
                                else
                                {
                                    *CurP = 0;
                                    *(CurP + 1) = 0;
                                    *(CurP + 2) = 0;

                                }
                                CurP += 3;
                                CurPB += 3;
                            }
                        }
                    });
                }
                iArea = iAreaTemp;
                BmpA.UnlockBits(BmpDataA);
                BmpB.UnlockBits(BmpDataB);
            }
        }
        
        /// <summary>
        /// A-B
        /// </summary>
        /// <param name="BmpA">第一张图 会变成差异图</param>
        /// <param name="BmpB">第二张图</param>
        /// <param name="bmpDifference">第三张图 和源图比较的差异图</param>
        /// <param name="Value">差异范围</param>
        /// <param name="iArea">差异面积</param>
        public static void SetBimap_A_B(Bitmap BmpA, Bitmap BmpB, out Bitmap bmpDifference, int Value, ref int iArea)
        {
            if (BmpA.Size != BmpB.Size)
                System.Windows.Forms.MessageBox.Show("两张图不一样大");

            int iAreaTemp = 0;
            bmpDifference = null;
            if (BmpB.PixelFormat == PixelFormat.Format32bppArgb &&
               BmpA.PixelFormat == PixelFormat.Format32bppArgb)
            {
                SetBimap_A_BFormat32(BmpA, BmpB,out bmpDifference, Value, ref iArea);
                return;
            }
            if (BmpB.PixelFormat == PixelFormat.Format8bppIndexed &&
                BmpA.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                SetBimap_A_BFormat8(BmpA, BmpB, Value, ref iArea);
                return;
            }
            if (BmpB.PixelFormat == PixelFormat.Format24bppRgb &&
                BmpA.PixelFormat == PixelFormat.Format24bppRgb)
            {
                bmpDifference = BmpA.Clone(new Rectangle(0, 0, BmpA.Width, BmpA.Height), PixelFormat.Format24bppRgb);
                BitmapData BmpDataA = BmpA.LockBits(new Rectangle(0, 0, BmpA.Width, BmpA.Height), ImageLockMode.ReadOnly, BmpA.PixelFormat);
                BitmapData BmpDataB = BmpB.LockBits(new Rectangle(0, 0, BmpB.Width, BmpB.Height), ImageLockMode.ReadOnly, BmpB.PixelFormat);
                BitmapData BmpDataC = bmpDifference.LockBits(new Rectangle(0, 0, bmpDifference.Width, bmpDifference.Height), ImageLockMode.ReadOnly, bmpDifference.PixelFormat);

                unsafe
                {
                    Parallel.ForEach(Partitioner.Create(0, BmpDataB.Height), (H) =>
                    {

                        int X, Y, Width, Height, Stride;// pixR = 0;
                        int PointA, PointB;
                        byte* Scan0, CurP, Scan0B, CurPB, Scan0C, CurPC; ;

                        //   double pixelR = 0;
                        Width = BmpDataB.Width;
                        Height = BmpDataB.Height;
                        Stride = BmpDataB.Stride;
                        Scan0 = (byte*)BmpDataA.Scan0;
                        Scan0B = (byte*)BmpDataB.Scan0;
                        Scan0C = (byte*)BmpDataC.Scan0;
                        for (Y = H.Item1; Y < H.Item2; Y++)
                        {
                            CurP = Scan0 + Y * Stride;
                            CurPB = Scan0B + Y * Stride;
                            CurPC = Scan0C + Y * Stride;
                            for (X = 0; X < Width; X++)
                            {
                                PointA = (int)((float)*CurP);
                                PointB = (int)((float)*CurPB);
                                if (Math.Abs(PointA - PointB) > Value )
                                {
                                    iAreaTemp++;
                                    *CurP = 255;
                                    *(CurP + 1) = 255;
                                    *(CurP + 2) = 255;

                                    *CurPC = 0;
                                    *(CurPC + 1) = 0;
                                    *(CurPC + 2) = 255;
                                }
                                else
                                {
                                    *CurP = 0;
                                    *(CurP + 1) = 0;
                                    *(CurP + 2) = 0;

                                }
                                CurP += 3;
                                CurPB += 3;
                                CurPC += 3;
                            }
                        }
                    });
                }
                iArea = iAreaTemp;
                BmpA.UnlockBits(BmpDataA);
                BmpB.UnlockBits(BmpDataB);
                bmpDifference.UnlockBits(BmpDataC);
            }
        }

        /// <summary>
        /// A-B
        /// </summary>
        /// <param name="BmpA">第一张图 会变成差异图</param>
        /// <param name="BmpB">第二张图</param>
        /// <param name="BmpAItem">第三张图，二进制图</param>
        /// <param name="BmpBItem">第四张图，二进制图</param>
        /// <param name="bmpDifference">第三张图 和源图比较的差异图</param>
        /// <param name="Value">差异范围</param>
        /// <param name="iArea">差异面积</param>
        public static void SetBimap_A_B(Bitmap BmpA, Bitmap BmpB, Bitmap BmpAItem, Bitmap BmpBItem, out Bitmap bmpDifference, int Value, ref int iArea)
        {
            if (BmpA.Size != BmpB.Size)
                System.Windows.Forms.MessageBox.Show("两张图不一样大");

            int iAreaTemp = 0;
            bmpDifference = null;
            if (BmpB.PixelFormat == PixelFormat.Format32bppArgb &&
               BmpA.PixelFormat == PixelFormat.Format32bppArgb)
            {
                SetBimap_A_BFormat32(BmpA, BmpB, Value, ref iArea);
                return;
            }
            if (BmpB.PixelFormat == PixelFormat.Format8bppIndexed &&
                BmpA.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                SetBimap_A_BFormat8(BmpA, BmpB, Value, ref iArea);
                return;
            }
            if (BmpB.PixelFormat == PixelFormat.Format24bppRgb &&
                BmpA.PixelFormat == PixelFormat.Format24bppRgb &&
                BmpAItem.PixelFormat == PixelFormat.Format24bppRgb &&
                BmpBItem.PixelFormat == PixelFormat.Format24bppRgb)
            {
                bmpDifference = BmpA.Clone(new Rectangle(0, 0, BmpA.Width, BmpA.Height), PixelFormat.Format24bppRgb);
                BitmapData BmpDataA = BmpA.LockBits(new Rectangle(0, 0, BmpA.Width, BmpA.Height), ImageLockMode.ReadOnly, BmpA.PixelFormat);
                BitmapData BmpDataB = BmpB.LockBits(new Rectangle(0, 0, BmpB.Width, BmpB.Height), ImageLockMode.ReadOnly, BmpB.PixelFormat);
                BitmapData BmpDataAItem = BmpAItem.LockBits(new Rectangle(0, 0, BmpAItem.Width, BmpAItem.Height), ImageLockMode.ReadOnly, BmpAItem.PixelFormat);
                BitmapData BmpDataBItem = BmpBItem.LockBits(new Rectangle(0, 0, BmpBItem.Width, BmpBItem.Height), ImageLockMode.ReadOnly, BmpBItem.PixelFormat);
                BitmapData BmpDataC = bmpDifference.LockBits(new Rectangle(0, 0, bmpDifference.Width, bmpDifference.Height), ImageLockMode.ReadOnly, bmpDifference.PixelFormat);

                unsafe
                {
                    Parallel.ForEach(Partitioner.Create(0, BmpDataB.Height), (H) =>
                    {

                        int X, Y, Width, Height, Stride;// pixR = 0;
                        int PointA, PointB, PointAItem, PointBItem;
                        byte* Scan0, CurP, Scan0B, CurPB, Scan0C, CurPC, Scan0Item, Scan0BItem, CurPItem, CurPBItem;

                        //   double pixelR = 0;
                        Width = BmpDataB.Width;
                        Height = BmpDataB.Height;
                        Stride = BmpDataB.Stride;
                        Scan0 = (byte*)BmpDataA.Scan0;
                        Scan0B = (byte*)BmpDataB.Scan0;
                        Scan0Item = (byte*)BmpDataAItem.Scan0;
                        Scan0BItem = (byte*)BmpDataBItem.Scan0;
                        Scan0C = (byte*)BmpDataC.Scan0;
                        for (Y = H.Item1; Y < H.Item2; Y++)
                        {
                            CurP = Scan0 + Y * Stride;
                            CurPB = Scan0B + Y * Stride;
                            CurPItem = Scan0Item + Y * Stride;
                            CurPBItem = Scan0BItem + Y * Stride;

                            CurPC = Scan0C + Y * Stride;
                            for (X = 0; X < Width; X++)
                            {
                                PointA = (int)((float)*CurP);
                                PointB = (int)((float)*CurPB);
                                PointAItem = (int)((float)*CurPItem);
                                PointBItem = (int)((float)*CurPBItem);

                                bool isMaxk = true;
                               // if ( Math.Abs( PointB-PointA) > Value )
                                if (Math.Abs(PointAItem - PointBItem) > Value)
                                {
                                  //  if (!(PointAItem > 254 && PointBItem > 254))
                                    if (Math.Abs(PointA - PointB) > Value)
                                    {
                                        isMaxk = false;
                                        iAreaTemp++;
                                        *CurP = 255;
                                        *(CurP + 1) = 255;
                                        *(CurP + 2) = 255;

                                        *CurPC = 0;
                                        *(CurPC + 1) = 0;
                                        *(CurPC + 2) = 255;
                                    }
                                }
                               if(isMaxk)
                                {
                                    *CurP = 0;
                                    *(CurP + 1) = 0;
                                    *(CurP + 2) = 0;
                                }

                                CurP += 3;
                                CurPB += 3;
                                CurPItem += 3;
                                CurPBItem += 3;
                                CurPC += 3;
                            }
                        }
                    });
                }
                iArea = iAreaTemp;
                BmpA.UnlockBits(BmpDataA);
                BmpB.UnlockBits(BmpDataB);
                BmpAItem.UnlockBits(BmpDataAItem);
                BmpBItem.UnlockBits(BmpDataBItem);
                bmpDifference.UnlockBits(BmpDataC);
            }
        }

        /// <summary>
        /// A-B
        /// </summary>
        /// <param name="BmpA">第一张图 会变成差异图</param>
        /// <param name="BmpB">第二张图</param>
        /// <param name="BmpAItem">第三张图，二进制图</param>
        /// <param name="BmpBItem">第四张图，二进制图</param>
        /// <param name="bmpDifference">第三张图 和源图比较的差异图</param>
        /// <param name="Value">差异范围</param>
        /// <param name="iArea">差异面积</param>
        /// <param name="iAreaFobackl">面积</param>
        public static void SetBimap_A_B(Bitmap BmpA, Bitmap BmpB, Bitmap BmpAItem, Bitmap BmpBItem, out Bitmap bmpDifference, int Value, ref int iArea,ref int iAreaFobackl,int ideffBack=0)
        {
            if (BmpA.Size != BmpB.Size)
                System.Windows.Forms.MessageBox.Show("两张图不一样大");

            iAreaFobackl = 0;
            int iFobakl = 0;
            int iAreaTemp = 0;
            bmpDifference = null;
            if (BmpB.PixelFormat == PixelFormat.Format32bppArgb &&
               BmpA.PixelFormat == PixelFormat.Format32bppArgb)
            {
                SetBimap_A_BFormat32(BmpA, BmpB, Value, ref iArea);
                return;
            }
            if (BmpB.PixelFormat == PixelFormat.Format8bppIndexed &&
                BmpA.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                SetBimap_A_BFormat8(BmpA, BmpB, Value, ref iArea);
                return;
            }
            if (BmpB.PixelFormat == PixelFormat.Format24bppRgb &&
                BmpA.PixelFormat == PixelFormat.Format24bppRgb &&
                BmpAItem.PixelFormat == PixelFormat.Format8bppIndexed &&
                BmpBItem.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                bmpDifference = BmpA.Clone(new Rectangle(0, 0, BmpA.Width, BmpA.Height), PixelFormat.Format24bppRgb);
                BitmapData BmpDataA = BmpA.LockBits(new Rectangle(0, 0, BmpA.Width, BmpA.Height), ImageLockMode.ReadOnly, BmpA.PixelFormat);
                BitmapData BmpDataB = BmpB.LockBits(new Rectangle(0, 0, BmpB.Width, BmpB.Height), ImageLockMode.ReadOnly, BmpB.PixelFormat);
                BitmapData BmpDataAItem = BmpAItem.LockBits(new Rectangle(0, 0, BmpAItem.Width, BmpAItem.Height), ImageLockMode.ReadOnly, BmpAItem.PixelFormat);
                BitmapData BmpDataBItem = BmpBItem.LockBits(new Rectangle(0, 0, BmpBItem.Width, BmpBItem.Height), ImageLockMode.ReadOnly, BmpBItem.PixelFormat);
                BitmapData BmpDataC = bmpDifference.LockBits(new Rectangle(0, 0, bmpDifference.Width, bmpDifference.Height), ImageLockMode.ReadOnly, bmpDifference.PixelFormat);

                unsafe
                {
                    Parallel.ForEach(Partitioner.Create(0, BmpDataB.Height), (H) =>
                    {

                        int X, Y, Width, Height, Stride , Stride2;// pixR = 0;
                        int PointA, PointB, PointAItem, PointBItem;
                        byte* Scan0, CurP, Scan0B, CurPB, Scan0C, CurPC, Scan0Item, Scan0BItem, CurPItem, CurPBItem;

                        //   double pixelR = 0;
                        Width = BmpDataB.Width;
                        Height = BmpDataB.Height;
                        Stride = BmpDataB.Stride;
                        Scan0 = (byte*)BmpDataA.Scan0;
                        Scan0B = (byte*)BmpDataB.Scan0;
                        Scan0Item = (byte*)BmpDataAItem.Scan0;
                        Scan0BItem = (byte*)BmpDataBItem.Scan0;
                        Stride2 = BmpDataAItem.Stride;
                        Scan0C = (byte*)BmpDataC.Scan0;
                        for (Y = H.Item1; Y < H.Item2; Y++)
                        {
                            CurP = Scan0 + Y * Stride;
                            CurPB = Scan0B + Y * Stride;
                            CurPItem = Scan0Item + Y * Stride2;
                            CurPBItem = Scan0BItem + Y * Stride2;

                            CurPC = Scan0C + Y * Stride;
                            for (X = 0; X < Width; X++)
                            {
                                PointA = (int)((float)*CurP);
                                PointB = (int)((float)*CurPB);
                                PointAItem = (int)((float)*CurPItem);
                                PointBItem = (int)((float)*CurPBItem);

                                if (PointAItem < 100)
                                    iFobakl++;

                                bool isMaxk = true;
                                // if ( Math.Abs( PointB-PointA) > Value )
                                if (PointAItem <100 ||  PointBItem <100)
                                {
                                    //  if (!(PointAItem > 254 && PointBItem > 254))
                                    int iValueTemp = (PointA - PointB) - ideffBack;
                                    if (Math.Abs(iValueTemp) > Value) 
                                    {
                                        isMaxk = false;
                                        iAreaTemp++;
                                        *CurP = 255;
                                        *(CurP + 1) = 255;
                                        *(CurP + 2) = 255;

                                        *CurPC = 0;
                                        *(CurPC + 1) = 0;
                                        *(CurPC + 2) = 255;
                                    }
                                }
                                if (isMaxk)
                                {
                                    *CurP = 0;
                                    *(CurP + 1) = 0;
                                    *(CurP + 2) = 0;
                                }

                                CurP += 3;
                                CurPB += 3;
                                CurPItem += 1;
                                CurPBItem += 1;
                                CurPC += 3;
                            }
                        }
                    });
                }
                iAreaFobackl = iFobakl;
                iArea = iAreaTemp;
                BmpA.UnlockBits(BmpDataA);
                BmpB.UnlockBits(BmpDataB);
                BmpAItem.UnlockBits(BmpDataAItem);
                BmpBItem.UnlockBits(BmpDataBItem);
                bmpDifference.UnlockBits(BmpDataC);
            }
        }
        private static void SetBimap_A_BFormat8(Bitmap BmpA, Bitmap BmpB, int Value, ref int iArea)
        {
            int iAreaTemp = 0;
            if (BmpB.PixelFormat == PixelFormat.Format8bppIndexed &&
                BmpA.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                BitmapData BmpDataA = BmpA.LockBits(new Rectangle(0, 0, BmpA.Width, BmpA.Height), ImageLockMode.ReadOnly, BmpA.PixelFormat);
                BitmapData BmpDataB = BmpB.LockBits(new Rectangle(0, 0, BmpB.Width, BmpB.Height), ImageLockMode.ReadOnly, BmpB.PixelFormat);

                unsafe
                {
                    Parallel.ForEach(Partitioner.Create(0, BmpDataA.Height), (H) =>
                    {

                        int X, Y, Width, Height, Stride;// pixR = 0;
                        int PointA, PointB;
                        byte* Scan0, CurP, Scan0B, CurPB;

                        //double pixelR = 0;
                        Width = BmpDataA.Width;
                        Height = BmpDataA.Height;
                        Stride = BmpDataA.Stride;
                        Scan0 = (byte*)BmpDataA.Scan0;
                        Scan0B = (byte*)BmpDataB.Scan0;

                        for (Y = H.Item1; Y < H.Item2; Y++)
                        {
                            CurP = Scan0 + Y * Stride;
                            CurPB = Scan0B + Y * Stride;
                            for (X = 0; X < Width; X++)
                            {
                                //  gray = (pIn[0] * 19595 + pIn[1] * 38469 + pIn[2] * 7472) >> 16; 
                                // Red = (byte)((*(CurP + 2) + *(CurP + 1) + *CurP) / 3);

                                //PointA = (int)((float)*CurP + (float)*(CurP + 1) + (float)*(CurP + 2));
                                //PointB = (int)((float)*CurPB + (float)*(CurPB + 1) + (float)*(CurPB + 2));
                                PointA = (int)((float)*CurP);
                                PointB = (int)((float)*CurPB);
                                // if (Math.Abs(PointA - PointB) < Value)
                                if (Math.Abs(PointA - PointB) > Value)
                                {
                                    iAreaTemp++;
                                    *CurP = 255;
                                    //*(CurP + 1) = 0;
                                    //*(CurP + 2) = 255;
                                }
                                else
                                    *CurP = 0;



                                CurP += 1;
                                CurPB += 1;
                            }
                        }
                    });
                }
                iArea = iAreaTemp;
                BmpA.UnlockBits(BmpDataA);
            }
        }
        private static void SetBimap_A_BFormat8(Bitmap BmpA, Bitmap BmpB, ref int iBackColor)
        {
            long lAreaTemp = 0, lValue = 0;
            if (BmpB.PixelFormat == PixelFormat.Format8bppIndexed &&
                BmpA.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                BitmapData BmpDataA = BmpA.LockBits(new Rectangle(0, 0, BmpA.Width, BmpA.Height), ImageLockMode.ReadOnly, BmpA.PixelFormat);
                BitmapData BmpDataB = BmpB.LockBits(new Rectangle(0, 0, BmpB.Width, BmpB.Height), ImageLockMode.ReadOnly, BmpB.PixelFormat);

                unsafe
                {
                    Parallel.ForEach(Partitioner.Create(0, BmpDataA.Height), (H) =>
                    {
                        int X, Y, Width, Height, Stride;// pixR = 0;
                        int PointA, PointB;
                        byte* Scan0, CurP, Scan0B, CurPB;

                        //double pixelR = 0;
                        Width = BmpDataA.Width;
                        Height = BmpDataA.Height;
                        Stride = BmpDataA.Stride;
                        Scan0 = (byte*)BmpDataA.Scan0;
                        Scan0B = (byte*)BmpDataB.Scan0;

                        for (Y = H.Item1; Y < H.Item2; Y++)
                        {
                            CurP = Scan0 + Y * Stride;
                            CurPB = Scan0B + Y * Stride;
                            for (X = 0; X < Width; X++)
                            {
                                PointA = (int)((float)*CurP);
                                PointB = (int)((float)*CurPB);
                                if (PointB  > 200)
                                {
                                    lAreaTemp++;
                                    lValue += PointA;
                                }
                                CurP += 1;
                                CurPB += 1;
                            }
                        }
                    });
                }
               if (lValue != 0 && lAreaTemp != 0)  
                {
                    int iback = (int)(lValue / lAreaTemp);
                    iBackColor = iback;

                    if (iBackColor > 255)
                        iBackColor = 255;
                    else if (iBackColor < 0)
                        iBackColor = 0;

                }
                else
                    iBackColor = 255;

                BmpA.UnlockBits(BmpDataA);
                BmpB.UnlockBits(BmpDataB);
            }
        }
        private static void SetBimap_A_BFormat8(Bitmap BmpA, Bitmap BmpB, ref int iBackColor,ref int iForeColor)
        {
            long lAreaTemp = 0, lValue = 0;
            long lAreaTemp2 = 0, lValue2 = 0;
            if (BmpB.PixelFormat == PixelFormat.Format8bppIndexed &&
                BmpA.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                BitmapData BmpDataA = BmpA.LockBits(new Rectangle(0, 0, BmpA.Width, BmpA.Height), ImageLockMode.ReadOnly, BmpA.PixelFormat);
                BitmapData BmpDataB = BmpB.LockBits(new Rectangle(0, 0, BmpB.Width, BmpB.Height), ImageLockMode.ReadOnly, BmpB.PixelFormat);

                unsafe
                {
                    Parallel.ForEach(Partitioner.Create(0, BmpDataA.Height), (H) =>
                    {
                        int X, Y, Width, Height, Stride;// pixR = 0;
                        int PointA, PointB;
                        byte* Scan0, CurP, Scan0B, CurPB;

                        //double pixelR = 0;
                        Width = BmpDataA.Width;
                        Height = BmpDataA.Height;
                        Stride = BmpDataA.Stride;
                        Scan0 = (byte*)BmpDataA.Scan0;
                        Scan0B = (byte*)BmpDataB.Scan0;

                        for (Y = H.Item1; Y < H.Item2; Y++)
                        {
                            CurP = Scan0 + Y * Stride;
                            CurPB = Scan0B + Y * Stride;
                            for (X = 0; X < Width; X++)
                            {
                                PointA = (int)((float)*CurP);
                                PointB = (int)((float)*CurPB);
                                if (PointB > 200)
                                {
                                    lAreaTemp++;
                                    lValue += PointA;
                                }
                                else
                                {
                                    lAreaTemp2++;
                                    lValue2 += PointA;
                                }
                                CurP += 1;
                                CurPB += 1;
                            }
                        }
                    });
                }
                if (lValue != 0 && lAreaTemp != 0)
                {
                    int iback = (int)(lValue / lAreaTemp);
                    iBackColor = iback;

                    if (iBackColor > 255)
                        iBackColor = 255;
                    else if (iBackColor < 0)
                        iBackColor = 0;

                }
                else
                    iBackColor = 255;

                if (lValue2 != 0 && lAreaTemp2 != 0)
                {
                    int iback = (int)(lValue2 / lAreaTemp2);
                    iForeColor = iback;

                    if (iForeColor > 255)
                        iForeColor = 255;
                    else if (iForeColor < 0)
                        iForeColor = 0;

                }
                else
                    iForeColor = 255;

                BmpA.UnlockBits(BmpDataA);
                BmpB.UnlockBits(BmpDataB);
            }
        }
        private static void SetBimap_A_BFormat32(Bitmap BmpA, Bitmap BmpB, int Value, ref int iArea)
        {
            int iAreaTemp = 0;
            if (BmpB.PixelFormat == PixelFormat.Format32bppArgb &&
                BmpA.PixelFormat == PixelFormat.Format32bppArgb)
            {
                BitmapData BmpDataA = BmpA.LockBits(new Rectangle(0, 0, BmpA.Width, BmpA.Height), ImageLockMode.ReadOnly, BmpA.PixelFormat);
                BitmapData BmpDataB = BmpB.LockBits(new Rectangle(0, 0, BmpB.Width, BmpB.Height), ImageLockMode.ReadOnly, BmpB.PixelFormat);

                unsafe
                {
                    Parallel.ForEach(Partitioner.Create(0, BmpDataA.Height), (H) =>
                    {

                        int X, Y, Width, Height, Stride;// pixR = 0;
                        int PointA, PointB;
                        byte* Scan0, CurP, Scan0B, CurPB;

                        //double pixelR = 0;
                        Width = BmpDataA.Width;
                        Height = BmpDataA.Height;
                        Stride = BmpDataA.Stride;
                        Scan0 = (byte*)BmpDataA.Scan0;
                        Scan0B = (byte*)BmpDataB.Scan0;

                        for (Y = H.Item1; Y < H.Item2; Y++)
                        {
                            CurP = Scan0 + Y * Stride;
                            CurPB = Scan0B + Y * Stride;
                            for (X = 0; X < Width; X++)
                            {
                                //  gray = (pIn[0] * 19595 + pIn[1] * 38469 + pIn[2] * 7472) >> 16; 
                                // Red = (byte)((*(CurP + 2) + *(CurP + 1) + *CurP) / 3);

                                //PointA = (int)((float)*CurP + (float)*(CurP + 1) + (float)*(CurP + 2));
                                //PointB = (int)((float)*CurPB + (float)*(CurPB + 1) + (float)*(CurPB + 2));
                                PointA = (int)((float)*CurP);
                                PointB = (int)((float)*CurPB);
                                // if (Math.Abs(PointA - PointB) < Value)
                                if (Math.Abs(PointA - PointB) > Value)
                                {
                                    iAreaTemp++;
                                    *CurP = 0;
                                    *(CurP + 1) = 0;
                                    *(CurP + 2) = 0;
                                    *(CurP + 3) = 255;
                                }
                                else
                                {
                                    *CurP = 0;
                                    *(CurP + 1) = 0;
                                    *(CurP + 2) = 0;
                                    *(CurP + 3) = 0;

                                }

                                CurP += 4;
                                CurPB += 4;
                            }
                        }
                    });
                }
                iArea = iAreaTemp;
                BmpA.UnlockBits(BmpDataA);
                BmpB.UnlockBits(BmpDataB);
            }
        }

        public static void SetBimap_A_BFormat32(Bitmap BmpA, Bitmap BmpB, Bitmap bmpMask, out Bitmap bmpDifference, float Value, ref int iArea)
        {
            bmpDifference = new Bitmap(BmpA.Width, BmpA.Height);
            int iAreaTemp = 0;
            if (BmpB.PixelFormat == PixelFormat.Format32bppArgb &&
                BmpA.PixelFormat == PixelFormat.Format32bppArgb)
            {
            
                   BitmapData BmpDataA = BmpA.LockBits(new Rectangle(0, 0, BmpA.Width, BmpA.Height), ImageLockMode.ReadOnly, BmpA.PixelFormat);
                BitmapData BmpDataB = BmpB.LockBits(new Rectangle(0, 0, BmpB.Width, BmpB.Height), ImageLockMode.ReadOnly, BmpB.PixelFormat);
                BitmapData BmpDataM = bmpMask.LockBits(new Rectangle(0, 0, bmpMask.Width, bmpMask.Height), ImageLockMode.ReadOnly, bmpMask.PixelFormat);
                BitmapData BmpDataD = bmpDifference.LockBits(new Rectangle(0, 0, bmpDifference.Width, bmpDifference.Height), ImageLockMode.ReadOnly, bmpDifference.PixelFormat);
                unsafe
                {
                    Parallel.ForEach(Partitioner.Create(0, BmpDataA.Height), (H) =>
                    {

                        int X, Y, Width, Height, Stride;// pixR = 0;
                        int PointA, PointB, PointM;
                        byte* Scan0, CurP, Scan0B, CurPB, CurM, ScanM,CurD, ScanD;

                        //double pixelR = 0;
                        Width = BmpDataA.Width;
                        Height = BmpDataA.Height;
                        Stride = BmpDataA.Stride;
                        Scan0 = (byte*)BmpDataA.Scan0;
                        Scan0B = (byte*)BmpDataB.Scan0;
                        ScanD = (byte*)BmpDataD.Scan0;

                        ScanM = (byte*)BmpDataM.Scan0;
                        for (Y = H.Item1; Y < H.Item2; Y++)
                        {
                            CurP = Scan0 + Y * Stride;
                            CurPB = Scan0B + Y * Stride;
                            CurD = ScanD + Y * Stride;
                            CurM = ScanM + Y * Stride;
                            for (X = 0; X < Width; X++)
                            {
                                PointA = (CurP[0] * 19595 + CurP[1] * 38469 + CurP[2] * 7472) >> 16;
                                PointB = (CurPB[0] * 19595 + CurPB[1] * 38469 + CurPB[2] * 7472) >> 16;
                                PointM = (CurM[0] * 19595 + CurM[1] * 38469 + CurM[2] * 7472) >> 16;
                                // Red = (byte)((*(CurP + 2) + *(CurP + 1) + *CurP) / 3);

                                //   PointA = (int)((float)*CurP + (float)*(CurP + 1) + (float)*(CurP + 2))/3;
                                //   PointB = (int)((float)*CurPB + (float)*(CurPB + 1) + (float)*(CurPB + 2));
                                //PointA = (int)((float)*CurP);
                                //PointB = (int)((float)*CurPB);
                                // if (Math.Abs(PointA - PointB) < Value)

                                if (PointM > 128)
                                {
                                    if (Math.Abs(PointA - PointB) > Value )
                                    {
                                        iAreaTemp++;
                                        *CurD = 255;
                                        *(CurD + 1) = 255;
                                        *(CurD + 2) = 255;
                                        *(CurD + 3) = 255;
                                    }
                                    else
                                    {
                                        *CurD = 0;
                                        *(CurD + 1) = 0;
                                        *(CurD + 2) = 0;
                                        *(CurD + 3) = 255;

                                    }
                                }
                                else
                                {
                                    *CurD = 0;
                                    *(CurD + 1) = 0;
                                    *(CurD + 2) = 0;
                                    *(CurD + 3) = 255;

                                }

                                CurP += 4;
                                CurPB += 4;
                                CurD += 4;
                                CurM += 4;
                            }
                        }
                    });
                }
                iArea = iAreaTemp;
                BmpA.UnlockBits(BmpDataA);
                BmpB.UnlockBits(BmpDataB);
                bmpMask.UnlockBits(BmpDataM);
                bmpDifference.UnlockBits(BmpDataD);
            }
        }

        private static void SetBimap_A_BFormat32(Bitmap BmpA, Bitmap BmpB, out Bitmap bmpDifference, float Value, ref int iArea)
        {
            bmpDifference = new Bitmap(BmpA.Width, BmpA.Height);
            int iAreaTemp = 0;
            if (BmpB.PixelFormat == PixelFormat.Format32bppArgb &&
                BmpA.PixelFormat == PixelFormat.Format32bppArgb)
            {

                BitmapData BmpDataA = BmpA.LockBits(new Rectangle(0, 0, BmpA.Width, BmpA.Height), ImageLockMode.ReadOnly, BmpA.PixelFormat);
                BitmapData BmpDataB = BmpB.LockBits(new Rectangle(0, 0, BmpB.Width, BmpB.Height), ImageLockMode.ReadOnly, BmpB.PixelFormat);
                BitmapData BmpDataD = bmpDifference.LockBits(new Rectangle(0, 0, bmpDifference.Width, bmpDifference.Height), ImageLockMode.ReadOnly, bmpDifference.PixelFormat);
                unsafe
                {
                    Parallel.ForEach(Partitioner.Create(0, BmpDataA.Height), (H) =>
                    {

                        int X, Y, Width, Height, Stride;// pixR = 0;
                        int PointA, PointB;
                        byte* Scan0, CurP, Scan0B, CurPB, CurD, ScanD;

                        //double pixelR = 0;
                        Width = BmpDataA.Width;
                        Height = BmpDataA.Height;
                        Stride = BmpDataA.Stride;
                        Scan0 = (byte*)BmpDataA.Scan0;
                        Scan0B = (byte*)BmpDataB.Scan0;
                        ScanD = (byte*)BmpDataD.Scan0;
                        for (Y = H.Item1; Y < H.Item2; Y++)
                        {
                            CurP = Scan0 + Y * Stride;
                            CurPB = Scan0B + Y * Stride;
                            CurD = ScanD + Y * Stride;
                            for (X = 0; X < Width; X++)
                            {
                                PointA = (CurP[0] * 19595 + CurP[1] * 38469 + CurP[2] * 7472) >> 16;
                                PointB = (CurPB[0] * 19595 + CurPB[1] * 38469 + CurPB[2] * 7472) >> 16;
                               
                                if (Math.Abs(PointA - PointB) > Value / 2d)
                                {
                                    iAreaTemp++;
                                    *CurD = 0;
                                    *(CurD + 1) = 0;
                                    *(CurD + 2) = 0;
                                    *(CurD + 3) = 255;
                                }
                                else
                                {
                                    *CurD = 0;
                                    *(CurD + 1) = 0;
                                    *(CurD + 2) = 0;
                                    *(CurD + 3) = 0;

                                }

                                CurP += 4;
                                CurPB += 4;
                                CurD += 4;
                            }
                        }
                    });
                }
                iArea = iAreaTemp;
                BmpA.UnlockBits(BmpDataA);
                BmpB.UnlockBits(BmpDataB);
                bmpDifference.UnlockBits(BmpDataD);
            }
        }

        public static void SetBimap_A_BFormat32(Bitmap BmpA, Bitmap BmpB, Bitmap RunMask, Bitmap prainMask, Bitmap bmpMask, out Bitmap bmpDifference, int Value, ref int iArea)
        {
            bmpDifference = new Bitmap(BmpA.Width, BmpA.Height);
            int iAreaTemp = 0;
            if (BmpB.PixelFormat == PixelFormat.Format32bppArgb &&
                BmpA.PixelFormat == PixelFormat.Format32bppArgb &&
                prainMask.PixelFormat == PixelFormat.Format32bppArgb)
            {
                BitmapData BmpDataM2 = bmpMask.LockBits(new Rectangle(0, 0, bmpMask.Width, bmpMask.Height), ImageLockMode.ReadOnly, bmpMask.PixelFormat);
                BitmapData BmpDataM = prainMask.LockBits(new Rectangle(0, 0, prainMask.Width, prainMask.Height), ImageLockMode.ReadOnly, prainMask.PixelFormat);
                BitmapData BmpDataRM = RunMask.LockBits(new Rectangle(0, 0, RunMask.Width, RunMask.Height), ImageLockMode.ReadOnly, RunMask.PixelFormat);
                BitmapData BmpDataA = BmpA.LockBits(new Rectangle(0, 0, BmpA.Width, BmpA.Height), ImageLockMode.ReadOnly, BmpA.PixelFormat);
                BitmapData BmpDataB = BmpB.LockBits(new Rectangle(0, 0, BmpB.Width, BmpB.Height), ImageLockMode.ReadOnly, BmpB.PixelFormat);
                BitmapData BmpDataD = bmpDifference.LockBits(new Rectangle(0, 0, bmpDifference.Width, bmpDifference.Height), ImageLockMode.ReadOnly, bmpDifference.PixelFormat);
                unsafe
                {
                    Parallel.ForEach(Partitioner.Create(0, BmpDataA.Height), (H) =>
                    {

                        int X, Y, Width, Height, Stride;// pixR = 0;
                        int PointA, PointB, pointM, pointRM, pointM2;
                        byte* Scan0, CurP, Scan0B, CurPB, CurD, ScanD, CurM, ScanRM, CurRM, ScanM, CurM2, ScanM2;

                        //double pixelR = 0;
                        Width = BmpDataA.Width;
                        Height = BmpDataA.Height;
                        Stride = BmpDataA.Stride;
                        Scan0 = (byte*)BmpDataA.Scan0;
                        Scan0B = (byte*)BmpDataB.Scan0;
                        ScanD = (byte*)BmpDataD.Scan0;
                        ScanRM = (byte*)BmpDataRM.Scan0;
                        ScanM = (byte*)BmpDataM.Scan0;
                        ScanM2 = (byte*)BmpDataM2.Scan0;
                        for (Y = H.Item1; Y < H.Item2; Y++)
                        {
                            CurP = Scan0 + Y * Stride;
                            CurPB = Scan0B + Y * Stride;
                            CurD = ScanD + Y * Stride;
                            CurRM = ScanRM + Y * Stride;
                            CurM = ScanM + Y * Stride;
                            CurM2 = ScanM2 + Y * Stride;
                            for (X = 0; X < Width; X++)
                            {
                                PointA = (CurP[0] * 19595 + CurP[1] * 38469 + CurP[2] * 7472) >> 16;
                                PointB = (CurPB[0] * 19595 + CurPB[1] * 38469 + CurPB[2] * 7472) >> 16;
                                pointRM = (CurRM[0] * 19595 + CurRM[1] * 38469 + CurRM[2] * 7472) >> 16;
                                pointM = (CurM[0] * 19595 + CurM[1] * 38469 + CurM[2] * 7472) >> 16;
                                pointM2 = (CurM2[0] * 19595 + CurM2[1] * 38469 + CurM2[2] * 7472) >> 16;

                                // Red = (byte)((*(CurP + 2) + *(CurP + 1) + *CurP) / 3);

                                //   PointA = (int)((float)*CurP + (float)*(CurP + 1) + (float)*(CurP + 2))/3;
                                //   PointB = (int)((float)*CurPB + (float)*(CurPB + 1) + (float)*(CurPB + 2));
                                //PointA = (int)((float)*CurP);
                                //PointB = (int)((float)*CurPB);
                                // if (Math.Abs(PointA - PointB) < Value)

                                if (pointM2 > 128)
                                {
                                    if (pointM < 128 && Math.Abs(PointA - PointB) > (Value /1.5d))
                                    {
                                        iAreaTemp++;
                                        *CurD = 255;
                                        *(CurD + 1) = 255;
                                        *(CurD + 2) = 255;
                                        *(CurD + 3) = 255;
                                    }
                                   else if (pointRM < 128 && Math.Abs(PointA - PointB) > Value )
                                    {
                                        iAreaTemp++;
                                        *CurD = 255;
                                        *(CurD + 1) = 255;
                                        *(CurD + 2) = 255;
                                        *(CurD + 3) = 255;
                                    }
                                    else 
                                    {
                                        *CurD = 0;
                                        *(CurD + 1) = 0;
                                        *(CurD + 2) = 0;
                                        *(CurD + 3) = 255;

                                    }
                                }
                                else
                                {
                                    *CurD = 0;
                                    *(CurD + 1) = 0;
                                    *(CurD + 2) = 0;
                                    *(CurD + 3) = 255;

                                }

                                CurP += 4;
                                CurPB += 4;
                                CurD += 4;
                                CurM += 4;
                                CurM2 += 4;
                                CurRM += 4;
                            }
                        }
                    });
                }
                iArea = iAreaTemp;
                BmpA.UnlockBits(BmpDataA);
                BmpB.UnlockBits(BmpDataB);
                bmpDifference.UnlockBits(BmpDataD);
                prainMask.UnlockBits(BmpDataM);
                bmpMask.UnlockBits(BmpDataM2);
                RunMask.UnlockBits(BmpDataRM);

            }
        }

        public static void SetBimap_A_BFormat32(Bitmap BmpA, Bitmap BmpB, Bitmap RunMask, Bitmap RunMask2, Bitmap prainMask, Bitmap prainMask2, Bitmap bmpMask, out Bitmap bmpDifference, int Value, ref int iArea)
        {
            bmpDifference = new Bitmap(BmpA.Width, BmpA.Height);
            int iAreaTemp = 0;
            if (BmpB.PixelFormat == PixelFormat.Format32bppArgb &&
                BmpA.PixelFormat == PixelFormat.Format32bppArgb &&
                prainMask.PixelFormat == PixelFormat.Format32bppArgb)
            {
                BitmapData BmpDataM2 = bmpMask.LockBits(new Rectangle(0, 0, bmpMask.Width, bmpMask.Height), ImageLockMode.ReadOnly, bmpMask.PixelFormat);
                BitmapData BmpDataP = prainMask.LockBits(new Rectangle(0, 0, prainMask.Width, prainMask.Height), ImageLockMode.ReadOnly, prainMask.PixelFormat);
                BitmapData BmpDataP2 = prainMask2.LockBits(new Rectangle(0, 0, prainMask2.Width, prainMask2.Height), ImageLockMode.ReadOnly, prainMask2.PixelFormat);
                BitmapData BmpDataRM = RunMask.LockBits(new Rectangle(0, 0, RunMask.Width, RunMask.Height), ImageLockMode.ReadOnly, RunMask.PixelFormat);
                BitmapData BmpDataRM2 = RunMask2.LockBits(new Rectangle(0, 0, RunMask2.Width, RunMask2.Height), ImageLockMode.ReadOnly, RunMask2.PixelFormat);
                BitmapData BmpDataA = BmpA.LockBits(new Rectangle(0, 0, BmpA.Width, BmpA.Height), ImageLockMode.ReadOnly, BmpA.PixelFormat);
                BitmapData BmpDataB = BmpB.LockBits(new Rectangle(0, 0, BmpB.Width, BmpB.Height), ImageLockMode.ReadOnly, BmpB.PixelFormat);
                BitmapData BmpDataD = bmpDifference.LockBits(new Rectangle(0, 0, bmpDifference.Width, bmpDifference.Height), ImageLockMode.ReadOnly, bmpDifference.PixelFormat);
                unsafe
                {
                    Parallel.ForEach(Partitioner.Create(0, BmpDataA.Height), (H) =>
                    {

                        int X, Y, Width, Height, Stride;// pixR = 0;
                        int PointA, PointB, pointP, pointP2, pointRM2, pointRM, pointM2;
                        byte* Scan0, CurP, Scan0B, CurPB, CurD, ScanD,  ScanRM2, CurRM2, ScanRM, CurRM, CurPP2, ScanP2, CurPP, ScanP, CurM2, ScanM2;

                        //double pixelR = 0;
                        Width = BmpDataA.Width;
                        Height = BmpDataA.Height;
                        Stride = BmpDataA.Stride;
                        Scan0 = (byte*)BmpDataA.Scan0;
                        Scan0B = (byte*)BmpDataB.Scan0;
                        ScanD = (byte*)BmpDataD.Scan0;
                        ScanRM = (byte*)BmpDataRM.Scan0;
                        ScanRM2 = (byte*)BmpDataRM2.Scan0;
                        ScanP = (byte*)BmpDataP.Scan0;
                        ScanP2 = (byte*)BmpDataP2.Scan0;
                        ScanM2 = (byte*)BmpDataM2.Scan0;
                        for (Y = H.Item1; Y < H.Item2; Y++)
                        {
                            CurP = Scan0 + Y * Stride;
                            CurPB = Scan0B + Y * Stride;
                            CurD = ScanD + Y * Stride;
                            CurRM = ScanRM + Y * Stride;
                            CurRM2 = ScanRM2 + Y * Stride;
                            CurPP = ScanP + Y * Stride;
                            CurPP2 = ScanP2 + Y * Stride;
                            CurM2 = ScanM2 + Y * Stride;
                            for (X = 0; X < Width; X++)
                            {
                                PointA = (CurP[0] * 19595 + CurP[1] * 38469 + CurP[2] * 7472) >> 16;
                                PointB = (CurPB[0] * 19595 + CurPB[1] * 38469 + CurPB[2] * 7472) >> 16;
                                pointRM = (CurRM[0] * 19595 + CurRM[1] * 38469 + CurRM[2] * 7472) >> 16;
                                pointRM2 = (CurRM2[0] * 19595 + CurRM2[1] * 38469 + CurRM2[2] * 7472) >> 16;
                                pointP = (CurPP[0] * 19595 + CurPP[1] * 38469 + CurPP[2] * 7472) >> 16;
                                pointP2 = (CurPP2[0] * 19595 + CurPP2[1] * 38469 + CurPP2[2] * 7472) >> 16;
                                pointM2 = (CurM2[0] * 19595 + CurM2[1] * 38469 + CurM2[2] * 7472) >> 16;

                                // Red = (byte)((*(CurP + 2) + *(CurP + 1) + *CurP) / 3);

                                //   PointA = (int)((float)*CurP + (float)*(CurP + 1) + (float)*(CurP + 2))/3;
                                //   PointB = (int)((float)*CurPB + (float)*(CurPB + 1) + (float)*(CurPB + 2));
                                //PointA = (int)((float)*CurP);
                                //PointB = (int)((float)*CurPB);
                                // if (Math.Abs(PointA - PointB) < Value)

                                if (pointM2 > 128)
                                {
                                    if (pointP < 128 && Math.Abs(PointA - PointB) > (Value / 1.5d))
                                    {
                                        iAreaTemp++;
                                        *CurD = 255;
                                        *(CurD + 1) = 255;
                                        *(CurD + 2) = 255;
                                        *(CurD + 3) = 255;
                                    }
                                    else if (pointRM < 128 && Math.Abs(PointA - PointB) > Value)
                                    {
                                        iAreaTemp++;
                                        *CurD = 255;
                                        *(CurD + 1) = 255;
                                        *(CurD + 2) = 255;
                                        *(CurD + 3) = 255;
                                    }
                                    else
                                    {
                                        *CurD = 0;
                                        *(CurD + 1) = 0;
                                        *(CurD + 2) = 0;
                                        *(CurD + 3) = 255;

                                    }
                                    if (pointP2 < 128)
                                    {
                                        if (Math.Abs(pointP2 - pointRM2) != 0)
                                        {
                                            *CurD = 255;
                                            *(CurD + 1) = 255;
                                            *(CurD + 2) = 255;
                                            *(CurD + 3) = 255;
                                        }
                                    }


                                }
                                else
                                {
                                    *CurD = 0;
                                    *(CurD + 1) = 0;
                                    *(CurD + 2) = 0;
                                    *(CurD + 3) = 255;

                                }

                                CurP += 4;
                                CurPB += 4;
                                CurD += 4;
                                CurPP += 4;
                                CurPP2 += 4;
                                CurM2 += 4;
                                CurRM += 4;
                                CurRM2 += 4;
                            }
                        }
                    });
                }
                iArea = iAreaTemp;
                BmpA.UnlockBits(BmpDataA);
                BmpB.UnlockBits(BmpDataB);
                bmpDifference.UnlockBits(BmpDataD);
                prainMask.UnlockBits(BmpDataP);
                prainMask2.UnlockBits(BmpDataP2);
                bmpMask.UnlockBits(BmpDataM2);
                RunMask.UnlockBits(BmpDataRM);
                RunMask2.UnlockBits(BmpDataRM2);
            }
        }

        /// <summary>
        /// 算出背景平均亮度
        /// </summary>
        /// <param name="BmpA"></param>
        /// <param name="BmpB"></param>
        /// <param name="iAverage"></param>
        public static void SetBimap_A_B_Average(Bitmap BmpA, Bitmap BmpB, ref int iAverage)
        {
            int iAreaTemp = 0;
            long lValue = 0;
            if (BmpB.PixelFormat == PixelFormat.Format32bppArgb &&
               BmpA.PixelFormat == PixelFormat.Format32bppArgb)
            {
               // SetBimap_A_BFormat32(BmpA, BmpB, Value, ref iArea);
                return;
            }
          else  if (BmpB.PixelFormat == PixelFormat.Format8bppIndexed &&
                BmpA.PixelFormat == PixelFormat.Format8bppIndexed)
            {
             //   SetBimap_A_BFormat8(BmpA, BmpB, Value, ref iArea);
                return;
            }
            else if (BmpB.PixelFormat == PixelFormat.Format8bppIndexed &&
              BmpA.PixelFormat == PixelFormat.Format24bppRgb)
            {
                   SetBimap_A_B_Average24fo8(BmpA, BmpB, ref iAverage);
                return;
            }
            else if (BmpB.PixelFormat == PixelFormat.Format24bppRgb &&
                BmpA.PixelFormat == PixelFormat.Format24bppRgb)
            {
                BitmapData BmpDataA = BmpA.LockBits(new Rectangle(0, 0, BmpA.Width, BmpA.Height), ImageLockMode.ReadOnly, BmpA.PixelFormat);
                BitmapData BmpDataB = BmpB.LockBits(new Rectangle(0, 0, BmpB.Width, BmpB.Height), ImageLockMode.ReadOnly, BmpB.PixelFormat);

                unsafe
                {
                    Parallel.ForEach(Partitioner.Create(0, BmpDataA.Height), (H) =>
                    {

                        int X, Y, Width, Height, Stride, pixR = 0;
                        int PointA, PointB;
                        byte* Scan0, CurP, Scan0B, CurPB;

                        double pixelR = 0;
                        Width = BmpDataA.Width;
                        Height = BmpDataA.Height;
                        Stride = BmpDataA.Stride;
                        Scan0 = (byte*)BmpDataA.Scan0;
                        Scan0B = (byte*)BmpDataB.Scan0;

                        for (Y = H.Item1; Y < H.Item2; Y++)
                        {
                            CurP = Scan0 + Y * Stride;
                            CurPB = Scan0B + Y * Stride;
                            for (X = 0; X < Width; X++)
                            {
                                //  gray = (pIn[0] * 19595 + pIn[1] * 38469 + pIn[2] * 7472) >> 16; 
                                // Red = (byte)((*(CurP + 2) + *(CurP + 1) + *CurP) / 3);

                                //PointA = (int)((float)*CurP + (float)*(CurP + 1) + (float)*(CurP + 2));
                                //PointB = (int)((float)*CurPB + (float)*(CurPB + 1) + (float)*(CurPB + 2));
                                PointA = (int)((float)*CurP);
                                PointB = (int)((float)*CurPB);
                                // if (Math.Abs(PointA - PointB) < Value)
                                if (PointB <10)
                                {
                                    iAreaTemp++;
                                    lValue += PointA;
                                    *CurPB = 0;
                                    *(CurPB + 1) = 255;
                                    *(CurPB + 2) = 0;
                                }
                                
                                CurP += 3;
                                CurPB += 3;
                            }
                        }
                    });
                }
                iAverage =(int)(lValue/ iAreaTemp);
                BmpA.UnlockBits(BmpDataA);
                BmpB.UnlockBits(BmpDataB);
            }
        }

        private static void SetBimap_A_B_Average24fo8(Bitmap BmpA, Bitmap BmpB, ref int iAverage)
        {
            int iAreaTemp = 0;
            long lValue = 0;
          
            if (BmpB.PixelFormat == PixelFormat.Format8bppIndexed &&
                BmpA.PixelFormat == PixelFormat.Format24bppRgb)
            {
                BitmapData BmpDataA = BmpA.LockBits(new Rectangle(0, 0, BmpA.Width, BmpA.Height), ImageLockMode.ReadOnly, BmpA.PixelFormat);
                BitmapData BmpDataB = BmpB.LockBits(new Rectangle(0, 0, BmpB.Width, BmpB.Height), ImageLockMode.ReadOnly, BmpB.PixelFormat);

                unsafe
                {
                    Parallel.ForEach(Partitioner.Create(0, BmpDataB.Height), (H) =>
                    {

                        int X, Y, Width, Height, Stride, StrideB, pixR = 0;
                        int PointA, PointB;
                        byte* Scan0, CurP, Scan0B, CurPB;

                        double pixelR = 0;
                        Width = BmpDataA.Width;
                        Height = BmpDataA.Height;
                        Stride = BmpDataA.Stride;
                        StrideB = BmpDataB.Stride;
                        Scan0 = (byte*)BmpDataA.Scan0;
                        Scan0B = (byte*)BmpDataB.Scan0;

                        for (Y = H.Item1; Y < H.Item2; Y++)
                        {
                            CurP = Scan0 + Y * Stride;
                            CurPB = Scan0B + Y * StrideB;
                            for (X = 0; X < Width; X++)
                            {
                                //  gray = (pIn[0] * 19595 + pIn[1] * 38469 + pIn[2] * 7472) >> 16; 
                                // Red = (byte)((*(CurP + 2) + *(CurP + 1) + *CurP) / 3);

                                //PointA = (int)((float)*CurP + (float)*(CurP + 1) + (float)*(CurP + 2));
                                //PointB = (int)((float)*CurPB + (float)*(CurPB + 1) + (float)*(CurPB + 2));
                                PointA = (int)((float)*CurP);
                                PointB = (int)((float)*CurPB);
                                // if (Math.Abs(PointA - PointB) < Value)
                                if (PointB > 200)
                                {
                                    iAreaTemp++;
                                    lValue += PointA;
                                    *CurP = 0;
                                    *(CurP + 1) = 255;
                                    *(CurP + 2) = 0;
                                }

                                CurP += 3;
                                CurPB += 1;
                            }
                        }
                    });
                }
                iAverage = (int)(lValue / iAreaTemp);
                BmpA.UnlockBits(BmpDataA);
                BmpB.UnlockBits(BmpDataB);
            }
        }
        public static void SetBimap8To24(Bitmap BmpA, Bitmap BmpB,ref  int iFousColor)
        {
            long myFounsColor = 0, myFousCount = 0;
            if (BmpB.PixelFormat == PixelFormat.Format8bppIndexed &&
                BmpA.PixelFormat == PixelFormat.Format24bppRgb)
            {
                BitmapData BmpDataA = BmpA.LockBits(new Rectangle(0, 0, BmpA.Width, BmpA.Height), ImageLockMode.ReadOnly, BmpA.PixelFormat);
                BitmapData BmpDataB = BmpB.LockBits(new Rectangle(0, 0, BmpB.Width, BmpB.Height), ImageLockMode.ReadOnly, BmpB.PixelFormat);

                unsafe
                {
                    Parallel.ForEach(Partitioner.Create(0, BmpDataB.Height), (H) =>
                    {

                        int X, Y, Width, Height, Stride, StrideB, pixR = 0;
                        int PointA, PointB;
                        byte* Scan0, CurP, Scan0B, CurPB;

                        double pixelR = 0;
                        Width = BmpDataA.Width;
                        Height = BmpDataA.Height;
                        Stride = BmpDataA.Stride;
                        StrideB = BmpDataB.Stride;
                        Scan0 = (byte*)BmpDataA.Scan0;
                        Scan0B = (byte*)BmpDataB.Scan0;

                        for (Y = H.Item1; Y < H.Item2; Y++)
                        {
                            CurP = Scan0 + Y * Stride;
                            CurPB = Scan0B + Y * StrideB;
                            for (X = 0; X < Width; X++)
                            {
                                //  gray = (pIn[0] * 19595 + pIn[1] * 38469 + pIn[2] * 7472) >> 16; 
                                // Red = (byte)((*(CurP + 2) + *(CurP + 1) + *CurP) / 3);

                                //PointA = (int)((float)*CurP + (float)*(CurP + 1) + (float)*(CurP + 2));
                                //PointB = (int)((float)*CurPB + (float)*(CurPB + 1) + (float)*(CurPB + 2));
                                PointA = (int)((float)*CurP);
                                PointB = (int)((float)*CurPB);
                                // if (Math.Abs(PointA - PointB) < Value)
                                if (PointB > 200)
                                {
                                    *CurP = 255;
                                    *(CurP + 1) = 255;
                                    *(CurP + 2) = 255;
                                }
                                else
                                {
                                    myFousCount++;
                                    myFounsColor += PointA;
                                }

                                CurP += 3;
                                CurPB += 1;
                            }
                        }
                    });
                }
                if (myFounsColor != 0 && myFousCount != 0)
                    iFousColor = (int)(myFounsColor / myFousCount);
                else
                    iFousColor = 0;

                if (iFousColor > 255)
                    iFousColor = 255;
                if (iFousColor < 0)
                    iFousColor = 0;

                BmpA.UnlockBits(BmpDataA);
                BmpB.UnlockBits(BmpDataB);
            }
        }


        /// <summary>
        /// 把遮挡的图转换到源图上
        /// </summary>
        /// <param name="BmpA">第一张图</param>
        /// <param name="BmpMask">黑白Mask图</param>
        /// <param name="color">黑色部分要变成的颜色</param>
        public static void SetMaskToStilts(Bitmap BmpA, Bitmap BmpMask,Color color)
        {
            if (BmpMask.PixelFormat == PixelFormat.Format32bppArgb &&
                BmpA.PixelFormat == PixelFormat.Format32bppArgb)
            {
                BitmapData BmpDataA = BmpA.LockBits(new Rectangle(0, 0, BmpA.Width, BmpA.Height), ImageLockMode.ReadOnly, BmpA.PixelFormat);
                BitmapData BmpDataB = BmpMask.LockBits(new Rectangle(0, 0, BmpMask.Width, BmpMask.Height), ImageLockMode.ReadOnly, BmpMask.PixelFormat);

                unsafe
                {
                    Parallel.ForEach(Partitioner.Create(0, BmpDataA.Height), (H) =>
                    {

                        int X, Y, Width, Height, Stride, pixR = 0;
                        int PointA, PointB;
                        byte* Scan0, CurP, Scan0B, CurPB;

                        double pixelR = 0;
                        Width = BmpDataA.Width;
                        Height = BmpDataA.Height;
                        Stride = BmpDataA.Stride;
                        Scan0 = (byte*)BmpDataA.Scan0;
                        Scan0B = (byte*)BmpDataB.Scan0;

                        for (Y = H.Item1; Y < H.Item2; Y++)
                        {
                            CurP = Scan0 + Y * Stride;
                            CurPB = Scan0B + Y * Stride;
                            for (X = 0; X < Width; X++)
                            {
                                //  gray = (pIn[0] * 19595 + pIn[1] * 38469 + pIn[2] * 7472) >> 16; 
                                // Red = (byte)((*(CurP + 2) + *(CurP + 1) + *CurP) / 3);

                              //  PointA = (int)((float)*CurP + (float)*(CurP + 1) + (float)*(CurP + 2));
                                PointB = (int)((float)*CurPB + (float)*(CurPB + 1) + (float)*(CurPB + 2));

                                if ( PointB <100)
                                {
                                    *CurP = color.G;
                                    *(CurP + 1) = color.B;
                                    *(CurP + 2) = color.R;
                                    *(CurP + 3) = color.A;
                                }

                                CurP += 4;
                                CurPB += 4;
                            }
                        }
                    });
                }
                BmpA.UnlockBits(BmpDataA);
                BmpMask.UnlockBits(BmpDataB);
            }
        }


        /// <summary>
        /// 把遮挡的图转换到源图上
        /// </summary>
        /// <param name="BmpRunline">跑线图</param>
        /// <param name="BmpMask">黑白Mask图</param>
        /// <param name="MaskValue">影阴的值</param>
        /// <param name="BackValue">背景最大亮度值</param>
        /// <param name="color">阴影要变成的颜色</param>
        public static void SetMaskToStilts(Bitmap BmpRunline, Bitmap BmpMask, int MaskValue,int BackValue,Color color)
        {
            if (BmpMask.PixelFormat == PixelFormat.Format32bppArgb &&
                BmpRunline.PixelFormat == PixelFormat.Format32bppArgb)
            {
                BitmapData BmpDataA = BmpRunline.LockBits(new Rectangle(0, 0, BmpRunline.Width, BmpRunline.Height), ImageLockMode.ReadOnly, BmpRunline.PixelFormat);
                BitmapData BmpDataB = BmpMask.LockBits(new Rectangle(0, 0, BmpMask.Width, BmpMask.Height), ImageLockMode.ReadOnly, BmpMask.PixelFormat);

                int value= (BackValue + MaskValue) / 2;
                unsafe
                {
                    
                    Parallel.ForEach(Partitioner.Create(0, BmpDataA.Height), (H) =>
                    {

                        int X, Y, Width, Height, Stride, StrideB, pixR = 0;
                        int PointA, PointB;
                        byte* Scan0, CurP, Scan0B, CurPB;

                        double pixelR = 0;
                        Width = BmpDataA.Width;
                        Height = BmpDataA.Height;
                        Stride = BmpDataA.Stride;
                        StrideB = BmpDataB.Stride;
                        Scan0 = (byte*)BmpDataA.Scan0;
                        Scan0B = (byte*)BmpDataB.Scan0;
                        for (Y = H.Item1; Y < H.Item2; Y++)
                        {
                            CurP = Scan0 + Y * Stride ;
                            CurPB = Scan0B + Y * StrideB;
                            for (X = 0; X < Width; X++)
                            {
                                PointB = (CurPB[0] * 19595 + CurPB[1] * 38469 + CurPB[2] * 7472) >> 16;
                            //    PointB = ((byte)*CurPB * 19595 + (byte)*(CurPB + 1) * 38469 + (byte)*(CurPB + 2) * 7472) >> 16; 
                                // Red = (byte)((*(CurP + 2) + *(CurP + 1) + *CurP) / 3);
                                //PointB = (int)((float)*CurPB + (float)*(CurPB + 1) + (float)*(CurPB + 2));

                                if (PointB > 100)
                                {
                                    PointA = ((byte)*CurP * 19595 + (byte)*(CurP + 1) * 38469 + (byte)*(CurP + 2) * 7472) >> 16;
                                    if (PointA < value)
                                    {
                                        *CurP = color.G;
                                        *(CurP + 1) = color.B;
                                        *(CurP + 2) = color.R;
                                        *(CurP + 3) = color.A;
                                    }
                                    else
                                    {
                                        *CurP = 255;
                                        *(CurP + 1) = 255;
                                        *(CurP + 2) = 255;
                                        *(CurP + 3) = 255;
                                    }
                                }
                                else
                                {
                                    *CurP = 255;
                                    *(CurP + 1) = 255;
                                    *(CurP + 2) = 255;
                                    *(CurP + 3) = 255;
                                }

                                CurP += 4;
                                CurPB += 4;
                            }
                        }
                    });
                }
                BmpRunline.UnlockBits(BmpDataA);
                BmpMask.UnlockBits(BmpDataB);
            }
        }

        /// <summary>
        /// 彩色图片转换成灰度图片代码
        /// </summary>
        /// <param name="img">源图片</param>
        /// <returns></returns>
        public static Bitmap BitmapConvetGray(Bitmap img)
        {

            int h = img.Height;

            int w = img.Width;

            int gray = 0;    //灰度值

            Bitmap bmpOut = new Bitmap(w, h, PixelFormat.Format24bppRgb);    //每像素3字节

            BitmapData dataIn = img.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            BitmapData dataOut = bmpOut.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            unsafe
            {

                byte* pIn = (byte*)(dataIn.Scan0.ToPointer());      //指向源文件首地址
                byte* pOut = (byte*)(dataOut.Scan0.ToPointer());  //指向目标文件首地址
                for (int y = 0; y < dataIn.Height; y++)  //列扫描
                {

                    for (int x = 0; x < dataIn.Width; x++)   //行扫描
                    {

                        gray = (pIn[0] * 19595 + pIn[1] * 38469 + pIn[2] * 7472) >> 16;  //灰度计算公式

                        if (gray > 255)
                        {
                            int i = gray;
                        }

                        pOut[0] = (byte)gray;     //R分量

                        //pOut[1] = (byte)gray;     //G分量

                        //pOut[2] = (byte)gray;     //B分量

                        pIn += 3;
                        pOut += 1;      //指针后移3个分量位置

                    }

                    pIn += dataIn.Stride - dataIn.Width * 3;

                    pOut += dataOut.Stride - dataOut.Width;

                }

            }

            bmpOut.UnlockBits(dataOut);

            img.UnlockBits(dataIn);

            return bmpOut;

        }
        /// <summary>
        /// 位图灰度化
        /// </summary>
        /// <param name="bmp">原始位图</param>
        /// <returns>灰度位图</returns>
        public static Bitmap ToGrayBitmap(Bitmap bmp)
        {
            Int32 PixelHeight = bmp.Height; // 图像高度
            Int32 PixelWidth = bmp.Width;   // 图像宽度
            Int32 Stride = ((PixelWidth * 3 + 3) >> 2) << 2;    // 跨距宽度
            Byte[] Pixels = new Byte[PixelHeight * Stride];

            // 锁定位图到系统内存
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, PixelWidth, PixelHeight), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            Marshal.Copy(bmpData.Scan0, Pixels, 0, Pixels.Length);  // 从非托管内存拷贝数据到托管内存
            bmp.UnlockBits(bmpData);    // 从系统内存解锁位图

            // 将像素数据转换为灰度数据
            Int32 GrayStride = ((PixelWidth + 3) >> 2) << 2;
            Byte[] GrayPixels = new Byte[PixelHeight * GrayStride];
            for (Int32 i = 0; i < PixelHeight; i++)
            {
                Int32 Index = i * Stride;
                Int32 GrayIndex = i * GrayStride;
                for (Int32 j = 0; j < PixelWidth; j++)
                {
                    GrayPixels[GrayIndex++] = Convert.ToByte((Pixels[Index + 2] * 19595 + Pixels[Index + 1] * 38469 + Pixels[Index] * 7471 + 32768) >> 16);
                    Index += 3;
                }
            }

            // 创建灰度图像
            Bitmap GrayBmp = new Bitmap(PixelWidth, PixelHeight, PixelFormat.Format8bppIndexed);

            // 设置调色表
            ColorPalette cp = GrayBmp.Palette;
            for (int i = 0; i < 256; i++) cp.Entries[i] = Color.FromArgb(i, i, i);
            GrayBmp.Palette = cp;

            // 设置位图图像特性
            BitmapData GrayBmpData = GrayBmp.LockBits(new Rectangle(0, 0, PixelWidth, PixelHeight), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            Marshal.Copy(GrayPixels, 0, GrayBmpData.Scan0, GrayPixels.Length);
            GrayBmp.UnlockBits(GrayBmpData);

            return GrayBmp;
        }
        /// <summary>
        /// 图像二值化
        /// </summary>
        /// <param name="bmp">源图</param>
        /// <param name="iv">阀值</param>
        /// <returns></returns>
        public static Bitmap PBinary(Bitmap bmp, int iv)
        {
            int w = bmp.Width;
            int h = bmp.Height;
            Bitmap dstBitmap = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            System.Drawing.Imaging.BitmapData srcData = bmp.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            System.Drawing.Imaging.BitmapData dstData = dstBitmap.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* pIn = (byte*)srcData.Scan0.ToPointer();
                byte* pOut = (byte*)dstData.Scan0.ToPointer();
                byte* p;
                int stride = srcData.Stride;
                int r, g, b;
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        p = pIn;
                        r = p[2];
                        g = p[1];
                        b = p[0];
                        pOut[0] = pOut[1] = pOut[2] = (byte)(((byte)(0.2125 * r + 0.7154 * g + 0.0721 * b) >= iv)
                        ? 255 : 0);
                        pIn += 3;
                        pOut += 3;
                    }
                    pIn += srcData.Stride - w * 3;
                    pOut += srcData.Stride - w * 3;
                }
                bmp.UnlockBits(srcData);
                dstBitmap.UnlockBits(dstData);
                return dstBitmap;
            }
        }

        /// <summary>
        /// 转换成16位565格式
        /// </summary>
        /// <param name="bmp">原图</param>
        /// <returns></returns>
        public static Bitmap ToBit565(Bitmap bmp)
        {
            Int32 PixelHeight = bmp.Height; // 图像高度
            Int32 PixelWidth = bmp.Width;   // 图像宽度
            Int32 Stride = ((PixelWidth * 3 + 3) >> 2) << 2;    // 跨距宽度
            Byte[] Pixels = new Byte[PixelHeight * Stride];

            // 锁定位图到系统内存
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, PixelWidth, PixelHeight), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            Marshal.Copy(bmpData.Scan0, Pixels, 0, Pixels.Length);  // 从非托管内存拷贝数据到托管内存
            bmp.UnlockBits(bmpData);    // 从系统内存解锁位图

            // Bgr565格式为 RRRRR GGGGGG BBBBB
            Int32 TargetStride = ((PixelWidth + 1) >> 1) << 2;  // 每个像素占2字节，且跨距要求4字节对齐
            Byte[] TargetPixels = new Byte[PixelHeight * TargetStride];
            for (Int32 i = 0; i < PixelHeight; i++)
            {
                Int32 Index = i * Stride;
                Int32 Loc = i * TargetStride;
                for (Int32 j = 0; j < PixelWidth; j++)
                {
                    Byte B = Pixels[Index++];
                    Byte G = Pixels[Index++];
                    Byte R = Pixels[Index++];

                    TargetPixels[Loc++] = (Byte)(((G << 3) & 0xe0) | ((B >> 3) & 0x1f));
                    TargetPixels[Loc++] = (Byte)((R & 0xf8) | ((G >> 5) & 7));
                }
            }

            // 创建Bgr565图像
            Bitmap TargetBmp = new Bitmap(PixelWidth, PixelHeight, PixelFormat.Format16bppRgb565);

            // 设置位图图像特性
            BitmapData TargetBmpData = TargetBmp.LockBits(new Rectangle(0, 0, PixelWidth, PixelHeight), ImageLockMode.WriteOnly, PixelFormat.Format16bppRgb565);
            Marshal.Copy(TargetPixels, 0, TargetBmpData.Scan0, TargetPixels.Length);
            TargetBmp.UnlockBits(TargetBmpData);

            return TargetBmp;
        }
        /// <summary>
        /// 将原始图像转换成格式为Bgr555的16位图像
        /// </summary>
        /// <param name="bmp">用于转换的原始图像</param>
        /// <returns>转换后格式为Bgr555的16位图像</returns>
        public static Bitmap ToBgr555(Bitmap bmp)
        {
            Int32 PixelHeight = bmp.Height; // 图像高度
            Int32 PixelWidth = bmp.Width;   // 图像宽度
            Int32 Stride = ((PixelWidth * 3 + 3) >> 2) << 2;    // 跨距宽度
            Byte[] Pixels = new Byte[PixelHeight * Stride];

            // 锁定位图到系统内存
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, PixelWidth, PixelHeight), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            Marshal.Copy(bmpData.Scan0, Pixels, 0, Pixels.Length);  // 从非托管内存拷贝数据到托管内存
            bmp.UnlockBits(bmpData);    // 从系统内存解锁位图

            // Bgr555格式为 X RRRRR GGGGG BBBBB
            Int32 TargetStride = ((PixelWidth + 1) >> 1) << 2;  // 每个像素占2字节，且跨距要求4字节对齐
            Byte[] TargetPixels = new Byte[PixelHeight * TargetStride];
            for (Int32 i = 0; i < PixelHeight; i++)
            {
                Int32 Index = i * Stride;
                Int32 Loc = i * TargetStride;
                for (Int32 j = 0; j < PixelWidth; j++)
                {
                    Byte B = Pixels[Index++];
                    Byte G = Pixels[Index++];
                    Byte R = Pixels[Index++];

                    TargetPixels[Loc++] = (Byte)(((G << 2) & 0xe0) | ((B >> 3) & 0x1f));
                    TargetPixels[Loc++] = (Byte)(((R >> 1) & 0x7c) | ((G >> 6) & 3));
                }
            }

            // 创建Bgr555图像
            Bitmap TargetBmp = new Bitmap(PixelWidth, PixelHeight, PixelFormat.Format16bppRgb555);

            // 设置位图图像特性
            BitmapData TargetBmpData = TargetBmp.LockBits(new Rectangle(0, 0, PixelWidth, PixelHeight), ImageLockMode.WriteOnly, PixelFormat.Format16bppRgb555);
            Marshal.Copy(TargetPixels, 0, TargetBmpData.Scan0, TargetPixels.Length);
            TargetBmp.UnlockBits(TargetBmpData);

            return TargetBmp;
        }
        /// <summary>
        /// BitmapScurce转 Bitmap
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Bitmap BitmapFromSource(BitmapSource source)
        {
            using (System.IO.MemoryStream outStream = new System.IO.MemoryStream())
            {
                BitmapEncoder enc = new PngBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(source));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                // return bitmap; <-- leads to problems, stream is closed/closing ...
                return new Bitmap(bitmap);
            }
        }
        /// <summary>
        /// Bitmap 转BitmapScurce
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapSource CreateBitmapSourceFromBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
              System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }
        /// <summary>
        /// Bitmap 转BitmapScurce
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapSource BitmapToBitmapSource(Bitmap source)
        {
            using (System.IO.MemoryStream memory = new System.IO.MemoryStream())
            {
                source.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
        /// <summary>
        /// 把文件读进内存解读成BitmapSource
        /// </summary>
        /// <param name="stream">文件位置</param>
        /// <returns></returns>
        public static BitmapSource CreateBitmapSourceFromBitmap(System.IO.Stream stream)
        {
            BitmapDecoder bitmapDecoder = BitmapDecoder.Create(
                stream,
                BitmapCreateOptions.PreservePixelFormat,
                BitmapCacheOption.OnLoad);

            // This will disconnect the stream from the image completely...
            WriteableBitmap writable = new WriteableBitmap(bitmapDecoder.Frames.Single());
            writable.Freeze();

            return writable;
        }

        /// <summary>
        /// 将位图转换为彩色数组
        /// </summary>
        /// <param name="bmp">原始位图</param>
        /// <returns>彩色数组</returns>
        public static Color[,] ToColorArray(Bitmap bmp)
        {
            Int32 PixelHeight = bmp.Height; // 图像高度
            Int32 PixelWidth = bmp.Width;   // 图像宽度
            Int32[] Pixels = new Int32[PixelHeight * PixelWidth];

            // 锁定位图到系统内存
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, PixelWidth, PixelHeight), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(bmpData.Scan0, Pixels, 0, Pixels.Length);  // 从非托管内存拷贝数据到托管内存
            bmp.UnlockBits(bmpData);    // 从系统内存解锁位图

            // 将像素数据转换为彩色数组
            Color[,] ColorArray = new Color[PixelHeight, PixelWidth];
            for (Int32 i = 0; i < PixelHeight; i++)
            {
                for (Int32 j = 0; j < PixelWidth; j++)
                {
                    ColorArray[i, j] = Color.FromArgb(Pixels[i * PixelWidth + j]);
                }
            }

            return ColorArray;
        }
        /// <summary>
        /// 将位图转换为灰度数组（256级灰度）
        /// </summary>
        /// <param name="bmp">原始位图</param>
        /// <returns>灰度数组</returns>
        public static Byte[,] ToGrayArray(Bitmap bmp)
        {
            Int32 PixelHeight = bmp.Height; // 图像高度
            Int32 PixelWidth = bmp.Width;   // 图像宽度
            Int32 Stride = ((PixelWidth * 3 + 3) >> 2) << 2;    // 跨距宽度
            Byte[] Pixels = new Byte[PixelHeight * Stride];

            // 锁定位图到系统内存
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, PixelWidth, PixelHeight), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            Marshal.Copy(bmpData.Scan0, Pixels, 0, Pixels.Length);  // 从非托管内存拷贝数据到托管内存
            bmp.UnlockBits(bmpData);    // 从系统内存解锁位图

            // 将像素数据转换为灰度数组
            Byte[,] GrayArray = new Byte[PixelHeight, PixelWidth];
            for (Int32 i = 0; i < PixelHeight; i++)
            {
                Int32 Index = i * Stride;
                for (Int32 j = 0; j < PixelWidth; j++)
                {
                    GrayArray[i, j] = Convert.ToByte((Pixels[Index + 2] * 19595 + Pixels[Index + 1] * 38469 + Pixels[Index] * 7471 + 32768) >> 16);
                    Index += 3;
                }
            }

            return GrayArray;
        }
        /// <summary>
        /// 将灰度数组转换为灰度图像（256级灰度）
        /// </summary>
        /// <param name="grayArray">灰度数组</param>
        /// <returns>灰度图像</returns>
        public static Bitmap GrayArrayToGrayBitmap(Byte[,] grayArray)
        {   // 将灰度数组转换为灰度数据
            Int32 PixelHeight = grayArray.GetLength(0);     // 图像高度
            Int32 PixelWidth = grayArray.GetLength(1);      // 图像宽度
            Int32 Stride = ((PixelWidth + 3) >> 2) << 2;    // 跨距宽度
            Byte[] Pixels = new Byte[PixelHeight * Stride];
            for (Int32 i = 0; i < PixelHeight; i++)
            {
                Int32 Index = i * Stride;
                for (Int32 j = 0; j < PixelWidth; j++)
                {
                    Pixels[Index++] = grayArray[i, j];
                }
            }

            // 创建灰度图像
            Bitmap GrayBmp = new Bitmap(PixelWidth, PixelHeight, PixelFormat.Format8bppIndexed);

            // 设置调色表
            ColorPalette cp = GrayBmp.Palette;
            for (int i = 0; i < 256; i++) cp.Entries[i] = Color.FromArgb(i, i, i);
            GrayBmp.Palette = cp;

            // 设置位图图像特性
            BitmapData GrayBmpData = GrayBmp.LockBits(new Rectangle(0, 0, PixelWidth, PixelHeight), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            Marshal.Copy(Pixels, 0, GrayBmpData.Scan0, Pixels.Length);
            GrayBmp.UnlockBits(GrayBmpData);

            return GrayBmp;
        }
        /// <summary>
        /// 将二值化数组转换为二值化图像
        /// </summary>
        /// <param name="binaryArray">二值化数组</param>
        /// <returns>二值化图像</returns>
        public static Bitmap BinaryArrayToBinaryBitmap(Byte[,] binaryArray)
        {   // 将二值化数组转换为二值化数据
            Int32 PixelHeight = binaryArray.GetLength(0);
            Int32 PixelWidth = binaryArray.GetLength(1);
            Int32 Stride = ((PixelWidth + 31) >> 5) << 2;
            Byte[] Pixels = new Byte[PixelHeight * Stride];
            for (Int32 i = 0; i < PixelHeight; i++)
            {
                Int32 Base = i * Stride;
                for (Int32 j = 0; j < PixelWidth; j++)
                {
                    if (binaryArray[i, j] != 0)
                    {
                        Pixels[Base + (j >> 3)] |= Convert.ToByte(0x80 >> (j & 0x7));
                    }
                }
            }

            // 创建黑白图像
            Bitmap BinaryBmp = new Bitmap(PixelWidth, PixelHeight, PixelFormat.Format1bppIndexed);

            // 设置调色表
            ColorPalette cp = BinaryBmp.Palette;
            cp.Entries[0] = Color.Black;    // 黑色
            cp.Entries[1] = Color.White;    // 白色
            BinaryBmp.Palette = cp;

            // 设置位图图像特性
            BitmapData BinaryBmpData = BinaryBmp.LockBits(new Rectangle(0, 0, PixelWidth, PixelHeight), ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);
            Marshal.Copy(Pixels, 0, BinaryBmpData.Scan0, Pixels.Length);
            BinaryBmp.UnlockBits(BinaryBmpData);

            return BinaryBmp;
        }
        /// <summary>
        /// 将图片转换为byre[]
        /// </summary>
        /// <param name="bitmap">源图</param>
        /// <returns></returns>
        public static byte[] ToBitmap2Byte(Bitmap bitmap)
        {
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Jpeg);
                byte[] data = new byte[stream.Length];
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                stream.Read(data, 0, Convert.ToInt32(stream.Length));
                return data;
            }
        }


        /// <summary>
        /// 取得图像的一个连续的块
        /// 既是：连通分量（极大连通子图），Connected Component
        /// </summary>
        /// <param name="bm"></param>
        /// <param name="x">x起点</param>
        /// <param name="y">y起点</param>
        /// <returns>tslw</returns>
        private static Dictionary<string, Point> GetBlock(Bitmap bm, int x, int y)
        {
            // 极大连通分量的点的集合
            Dictionary<string, Point> Track = new Dictionary<string, Point>();
            string strKeyOfPoint;
            // 工作栈
            Stack<Point> stk = new Stack<Point>();

            Color Cr = bm.GetPixel(x, y);
            if (ArgbEqual(Cr, Color.White) == true)
            {
                // 测试点不是黑色
                return Track;
            }
            // 入栈起始位置
            stk.Push(new Point(x, y));

            // 深度优先搜索
            for (; stk.Count != 0;)
            {
                // 弹出栈顶元素
                Point Pt = stk.Pop();
                // 加入访问过的路径集合中
                strKeyOfPoint = Pt.X + "#" + Pt.Y;
                Track[strKeyOfPoint] = new Point(Pt.X, Pt.Y);

                #region 取得邻接点集合

                List<Point> lstAdjacency = new List<Point>();

                // 右
                Point ptTest = new Point(Pt.X + 1, Pt.Y);
                if (ptTest.X < bm.Width)
                {
                    Color crTest = bm.GetPixel(ptTest.X, ptTest.Y);
                    if (ArgbEqual(crTest, Color.Black))
                    {
                        lstAdjacency.Add(ptTest);
                    }
                }

                // 左
                ptTest = new Point(Pt.X - 1, Pt.Y);
                if (ptTest.X >= 0)
                {
                    Color crTest = bm.GetPixel(ptTest.X, ptTest.Y);
                    if (ArgbEqual(crTest, Color.Black))
                    {
                        lstAdjacency.Add(ptTest);
                    }
                }

                // 下
                ptTest = new Point(Pt.X, Pt.Y + 1);
                if (ptTest.Y < bm.Height)
                {
                    Color crTest = bm.GetPixel(ptTest.X, ptTest.Y);
                    if (ArgbEqual(crTest, Color.Black))
                    {
                        lstAdjacency.Add(ptTest);
                    }
                }

                // 上
                ptTest = new Point(Pt.X, Pt.Y - 1);
                if (ptTest.Y >= 0)
                {
                    Color crTest = bm.GetPixel(ptTest.X, ptTest.Y);
                    if (ArgbEqual(crTest, Color.Black))
                    {
                        lstAdjacency.Add(ptTest);
                    }
                }

                #endregion

                #region 遍历邻接点，加入路径栈

                for (int i = 0; i < lstAdjacency.Count; ++i)
                {
                    Point ptAdjacency = lstAdjacency[i];
                    strKeyOfPoint = ptAdjacency.X + "#" + ptAdjacency.Y;
                    if (Track.ContainsKey(strKeyOfPoint) == false)
                    {
                        stk.Push(ptAdjacency);
                    }
                }

                #endregion

            }
            // end for


            return Track;
        }
        /// <summary>
        /// 去除块。降噪
        /// </summary>
        /// <param name="bm">要操作的位图对象</param>
        /// /// <param name="nBelowBlockSize">块大小，低于指定的大小的块，将被抹成白色</param>
        public static void RemoveBlock(Bitmap bm, int nBlockSize)
        {
            // 曾经遍历过的点
            Dictionary<string, Point> Track = new Dictionary<string, Point>();

            for (int i = 0; i < bm.Width; ++i)
            {
                for (int j = 0; j < bm.Height; ++j)
                {
                    if (Track.ContainsKey(i + "#" + j) == true)
                        continue;

                    Dictionary<string, Point> Block = GetBlock(bm, i, j);
                    foreach (string strkey in Block.Keys)
                    {
                        //if (Track.ContainsKey(strkey))
                        //{

                        //}
                        // Track[strkey] = Block[strkey];
                        Track.Add(strkey, Block[strkey]);
                    }

                    if (Block.Count < nBlockSize)
                    {
                        foreach (KeyValuePair<string, Point> Item in Block)
                        {
                            Point pt = Item.Value;
                            bm.SetPixel(pt.X, pt.Y, Color.White);
                        }

                        //foreach (string strkey in Block.Keys)
                        //{
                        //    Point pt = Block[strkey];
                        //    bm.SetPixel(pt.X, pt.Y, Color.White);
                        //}
                    }
                }
            }
        }
        /// <summary>
        /// 水平切割
        /// </summary>
        /// <param name="bm"></param>
        ///  <param name="nThickness">可切断的粗细度</param>
        public static void CutHorizontally(Bitmap bm, int nThickness)
        {
            // 状态标志。0白色区域状态，1黑色区域状态
            int nState = 0;
            int nPosStart = 0;

            for (int idxRow = 0; idxRow < bm.Height; ++idxRow)
            {
                nState = 0; // 初始化状态

                for (int idxCol = 0; idxCol < bm.Width; ++idxCol)
                {
                    Color Cr = bm.GetPixel(idxCol, idxRow);

                    #region 状态处理

                    switch (nState)
                    {
                        case 0: // 白色
                            {
                                if (ArgbEqual(Cr, Color.Black) == true)
                                {
                                    nPosStart = idxCol;
                                    nState = 1;
                                }
                            }
                            break;

                        case 1: // 黑色
                            {
                                if (ArgbEqual(Cr, Color.White) == true)
                                {
                                    int nThicknessTemp = idxCol - nPosStart;    // 宽度粗细

                                    if (nThicknessTemp <= nThickness)
                                    {
                                        // 切断
                                        for (int i = nPosStart; i < idxCol; ++i)
                                        {
                                            bm.SetPixel(i, idxRow, Color.White);
                                        }
                                    }

                                    nState = 0;
                                }
                            }
                            break;
                    }

                    #endregion

                }
                // end for

                if (nState == 1)
                {
                    int nThicknessTemp = bm.Width - nPosStart;    // 宽度粗细

                    if (nThicknessTemp <= nThickness)
                    {
                        // 切断
                        for (int i = nPosStart; i < bm.Width; ++i)
                        {
                            bm.SetPixel(i, idxRow, Color.White);
                        }
                    }
                }
            }
            // end for
        }

        /// <summary>
        /// 垂直切割
        /// </summary>
        /// <param name="bm"></param>
        /// <param name="nThickness">可切断的粗细度</param>
        public static void CutVerticality(Bitmap bm, int nThickness)
        {
            // 状态标志。0白色区域状态，1黑色区域状态
            int nState = 0;
            int nPosStart = 0;

            for (int idxCol = 0; idxCol < bm.Width; ++idxCol)   // 列
            {
                nState = 0; // 初始化状态

                for (int idxRow = 0; idxRow < bm.Height; ++idxRow) // 行
                {
                    Color Cr = bm.GetPixel(idxCol, idxRow);

                    #region 状态处理

                    switch (nState)
                    {
                        case 0: // 白色
                            {
                                if (ArgbEqual(Cr, Color.Black) == true)
                                {
                                    nPosStart = idxRow;
                                    nState = 1;
                                }
                            }
                            break;

                        case 1: // 黑色
                            {
                                if (ArgbEqual(Cr, Color.White) == true)
                                {
                                    int nThicknessTemp = idxRow - nPosStart;    // 宽度粗细

                                    if (nThicknessTemp <= nThickness)
                                    {
                                        // 切断
                                        for (int i = nPosStart; i < idxRow; ++i)
                                        {
                                            bm.SetPixel(idxCol, i, Color.White);
                                        }
                                    }

                                    nState = 0;
                                }
                            }
                            break;
                    }

                    #endregion

                }
                // end for

                if (nState == 1)
                {
                    int nThicknessTemp = bm.Height - nPosStart;    // 宽度粗细

                    if (nThicknessTemp <= nThickness)
                    {
                        // 切断
                        for (int i = nPosStart; i < bm.Height; ++i)
                        {
                            bm.SetPixel(idxCol, i, Color.White);
                        }
                    }
                }
            }
            // end for
        }
        /// <summary>
        /// Argb值判等
        /// </summary>
        /// <param name="cr1"></param>
        /// <param name="cr2"></param>
        /// <returns></returns>
        public static bool ArgbEqual(Color cr1, Color cr2)
        {
            if (cr1.A == cr2.A &&
                cr1.R == cr2.R &&
                cr1.G == cr2.G &&
                cr1.B == cr2.B)
            {
                return true;
            }

            return false;

        }
        /// <summary>
        /// 腐蚀
        /// </summary>
        /// <param name="bm">要腐蚀的图像</param>
        /// <param name="nDeep">腐蚀的深度</param>
        public static void Corrode(Bitmap bm, Direction Direction)
        {
            using (Bitmap bmOld = (Bitmap)bm.Clone())
            {
                switch (Direction)
                {
                    case Direction.Up:
                        {
                            #region

                            for (int idxCol = 0; idxCol < bm.Width; ++idxCol)
                            {
                                for (int idxRow = 1; idxRow < bm.Height; ++idxRow)
                                {
                                    // 先设置目标图像的像素点
                                    bm.SetPixel(idxCol, idxRow, Color.White);

                                    if (
                                        ArgbEqual(bmOld.GetPixel(idxCol, idxRow), Color.Black) &&
                                        ArgbEqual(bmOld.GetPixel(idxCol, idxRow - 1), Color.Black)
                                        )
                                    {
                                        bm.SetPixel(idxCol, idxRow, Color.Black);
                                    }

                                }
                                // end for
                            }

                            #endregion
                        }
                        break;

                    case Direction.Left:
                        {
                            #region

                            for (int idxCol = 1; idxCol < bm.Width; ++idxCol)
                            {
                                for (int idxRow = 0; idxRow < bm.Height; ++idxRow)
                                {
                                    // 先设置目标图像的像素点
                                    bm.SetPixel(idxCol, idxRow, Color.White);

                                    if (
                                        ArgbEqual(bmOld.GetPixel(idxCol, idxRow), Color.Black) &&
                                        ArgbEqual(bmOld.GetPixel(idxCol - 1, idxRow), Color.Black)
                                        )
                                    {
                                        bm.SetPixel(idxCol, idxRow, Color.Black);
                                    }

                                }
                                // end for
                            }

                            #endregion
                        }
                        break;

                    case Direction.Down:
                        {
                            #region

                            for (int idxCol = 0; idxCol < bm.Width; ++idxCol)
                            {
                                for (int idxRow = 0; idxRow < bm.Height - 1; ++idxRow)
                                {
                                    // 先设置目标图像的像素点
                                    bm.SetPixel(idxCol, idxRow, Color.White);

                                    if (
                                        ArgbEqual(bmOld.GetPixel(idxCol, idxRow), Color.Black) &&
                                        ArgbEqual(bmOld.GetPixel(idxCol, idxRow + 1), Color.Black)
                                        )
                                    {
                                        bm.SetPixel(idxCol, idxRow, Color.Black);
                                    }

                                }
                                // end for
                            }

                            #endregion
                        }
                        break;

                    case Direction.Right:
                        {
                            #region

                            for (int idxCol = 0; idxCol < bm.Width - 1; ++idxCol)
                            {
                                for (int idxRow = 0; idxRow < bm.Height; ++idxRow)
                                {
                                    // 先设置目标图像的像素点
                                    bm.SetPixel(idxCol, idxRow, Color.White);

                                    if (
                                        ArgbEqual(bmOld.GetPixel(idxCol, idxRow), Color.Black) &&
                                        ArgbEqual(bmOld.GetPixel(idxCol + 1, idxRow), Color.Black)
                                        )
                                    {
                                        bm.SetPixel(idxCol, idxRow, Color.Black);
                                    }

                                }
                                // end for
                            }

                            #endregion
                        }
                        break;
                        #region
                        /*
                        case Direction.Left | Direction.Right:
                            {
                                #region

                                for (int idxCol = 1; idxCol < bm.Width - 1; ++idxCol)
                                {
                                    for (int idxRow = 0; idxRow < bm.Height; ++idxRow)
                                    {
                                        // 先设置目标图像的像素点
                                        bm.SetPixel(idxCol, idxRow, Color.White);

                                        if (
                                            ArgbEqual(bmOld.GetPixel(idxCol, idxRow), Color.Black) &&
                                            ArgbEqual(bmOld.GetPixel(idxCol - 1, idxRow), Color.Black) &&
                                            ArgbEqual(bmOld.GetPixel(idxCol + 1, idxRow), Color.Black)
                                            )
                                        {
                                            bm.SetPixel(idxCol, idxRow, Color.Black);
                                        }

                                    }
                                    // end for
                                }

                                #endregion
                            }
                            break;

                        case Direction.Up | Direction.Down:
                            {
                                #region

                                for (int idxCol = 0; idxCol < bm.Width; ++idxCol)
                                {
                                    for (int idxRow = 1; idxRow < bm.Height - 1; ++idxRow)
                                    {
                                        // 先设置目标图像的像素点
                                        bm.SetPixel(idxCol, idxRow, Color.White);

                                        if (
                                            ArgbEqual(bmOld.GetPixel(idxCol, idxRow), Color.Black) &&
                                            ArgbEqual(bmOld.GetPixel(idxCol, idxRow - 1), Color.Black) &&
                                            ArgbEqual(bmOld.GetPixel(idxCol, idxRow + 1), Color.Black)
                                            )
                                        {
                                            bm.SetPixel(idxCol, idxRow, Color.Black);
                                        }

                                    }
                                    // end for
                                }

                                #endregion
                            }
                            break;
                             */
                        #endregion
                }
            }
            // end for
        }
        /// <summary>
        /// 平移图像
        /// </summary>
        /// <param name="bm"></param>
        /// <param name="Direction">平移的方向</param>
        /// <param name="nOffset">平移的偏移量</param>
        /// <param name="crFill">平移的填充像素</param>
        public static void Translate(Bitmap bm, Direction Direction, int nOffset, Color crFill)
        {
            switch (Direction)
            {
                case Direction.Left:
                    {
                        // 向左平移 nDeep 位
                        for (int idxCol = nOffset; idxCol < bm.Width; ++idxCol)
                        {
                            int idxColDst = idxCol - nOffset;

                            for (int idxRow = 0; idxRow < bm.Height; ++idxRow)
                            {
                                Color crSrc = bm.GetPixel(idxCol, idxRow);
                                bm.SetPixel(idxColDst, idxRow, crSrc);
                            }
                        }
                        // 被移空的地方填充crFill设定的背景
                        FillRect(bm,
                            new Point(bm.Width - nOffset, 0),
                            new Point(bm.Width - 1, bm.Height - 1),
                            crFill);
                    }
                    break;

                case Direction.Right:
                    {
                        // 向右平移 
                        for (int idxCol = bm.Width - nOffset - 1; idxCol >= 0; --idxCol)
                        {
                            int idxColDst = idxCol + nOffset;

                            for (int idxRow = 0; idxRow < bm.Height; ++idxRow)
                            {
                                Color crSrc = bm.GetPixel(idxCol, idxRow);
                                bm.SetPixel(idxColDst, idxRow, crSrc);
                            }
                        }
                        // 填充
                        FillRect(bm,
                            new Point(0, 0),
                            new Point(nOffset - 1, bm.Height - 1),
                            crFill);
                    }
                    break;

                case Direction.Down:
                    {
                        // 向下平移 
                        for (int idxRow = bm.Height - nOffset - 1; idxRow >= 0; --idxRow)
                        {
                            int idxRowDst = idxRow + nOffset;

                            for (int idxCol = 0; idxCol < bm.Width; ++idxCol)
                            {
                                Color crSrc = bm.GetPixel(idxCol, idxRow);
                                bm.SetPixel(idxCol, idxRowDst, crSrc);
                            }
                        }
                        // 填充
                        FillRect(bm,
                            new Point(0, 0),
                            new Point(bm.Width - 1, nOffset - 1),
                            crFill);
                    }
                    break;

                case Direction.Up:
                    {
                        // 向上平移 
                        for (int idxRow = nOffset; idxRow < bm.Height; ++idxRow)
                        {
                            int idxRowDst = idxRow - nOffset;

                            for (int idxCol = 0; idxCol < bm.Width; ++idxCol)
                            {
                                Color crSrc = bm.GetPixel(idxCol, idxRow);
                                bm.SetPixel(idxCol, idxRowDst, crSrc);
                            }
                        }
                        // 填充
                        FillRect(bm,
                            new Point(0, bm.Height - nOffset),
                            new Point(bm.Width - 1, bm.Height - 1),
                            crFill);
                    }
                    break;

            }


        }
        /// <summary>
        /// 填充矩形
        /// </summary>
        /// <param name="bm">要填充的图像</param>
        /// <param name="ptStart">填充开始位置</param>
        /// <param name="ptEnd">填充结束位置</param>
        /// <param name="ptEnd">填充的像素值</param>
        public static void FillRect(Bitmap bm, Point ptStart, Point ptEnd, Color crFill)
        {
            for (int idxCol = ptStart.X; idxCol <= ptEnd.X; ++idxCol)
            {
                for (int idxRow = ptStart.Y; idxRow <= ptEnd.Y; ++idxRow)
                {
                    bm.SetPixel(idxCol, idxRow, crFill);
                }
            }
        }

        /// <summary>
        /// 边缘检测
        /// </summary>
        /// <param name="bmp">原图</param>
        /// <param name="rectbmp">需要检测的地方</param>
        /// <returns></returns>
        public static Bitmap SetRoberts(Bitmap BmpT)
        {
            Bitmap Bmp = BmpT.Clone(new Rectangle(0, 0, BmpT.Width, BmpT.Height), PixelFormat.Format24bppRgb);

            BitmapData BmpData = Bmp.LockBits(new Rectangle(0, 0, Bmp.Width, Bmp.Height), ImageLockMode.ReadOnly, Bmp.PixelFormat);
            unsafe
            {
                Parallel.ForEach(Partitioner.Create(0, BmpData.Height), (H) =>
                {

                    int rr, gg, bb, r1, r2, r3, r4, fxr, fyr, i, j;
                    int g1, g2, g3, g4, fxg, fyg, b1, b2, b3, b4, fxb, fyb;

                    int X, Y, Width, Height, Stride;
                    byte* Scan0, CurP;

                    Width = BmpData.Width;
                    Height = BmpData.Height;
                    Stride = BmpData.Stride;
                    Scan0 = (byte*)BmpData.Scan0;

                    for (Y = H.Item1; Y < H.Item2; Y++)
                    {
                        CurP = Scan0 + Y * Stride;
                        if (Y > Height - 2)
                            break;

                        for (X = 0; X < Width; X++)
                        {
                            if (X > Width - 2)
                                break;

                            r1 = *CurP;
                            r2 = *(CurP + Stride + 3);
                            r3 = *(CurP + Stride);
                            r4 = *(CurP + 3);
                            fxr = r1 - r2;
                            fyr = r3 - r4;
                            rr = Math.Abs(fxr) + Math.Abs(fyr) + 128;
                            if (rr < 0)
                                rr = 0;
                            if (rr > 255)
                                rr = 255;

                            g1 = *(CurP + 1);
                            g2 = *(CurP + 1 + Stride + 3);
                            g3 = *(CurP + 1 + Stride);
                            g4 = *(CurP + 1 + 3);
                            fxg = g1 - g2;
                            fyg = g3 - g4;
                            gg = Math.Abs(fxg) + Math.Abs(fyg) + 128;
                            if (gg < 0) gg = 0;
                            if (gg > 255) gg = 255;

                            b1 = *(CurP + 2);
                            b2 = *(CurP + 2 + Stride + 3);
                            b3 = *(CurP + 2 + Stride);
                            b4 = *(CurP + 2 + 3);
                            fxb = b1 - b2;
                            fyb = b3 - b4;
                            bb = Math.Abs(fxb) + Math.Abs(fyb) + 128;
                            if (bb < 0) bb = 0;
                            if (bb > 255) bb = 255;

                            *CurP = (byte)rr;
                            *(CurP + 1) = (byte)gg;
                            *(CurP + 2) = (byte)bb;

                            CurP += 3;
                        }
                    }
                });
            }
            Bmp.UnlockBits(BmpData);

            return Bmp;

        }
        /// <summary>
        /// 边缘检测
        /// </summary>
        /// <param name="bmp">原图</param>
        /// <param name="rectbmp">需要检测的地方</param>
        /// <returns></returns>
        public static Bitmap SetRoberts(Bitmap bmp, Rectangle rectbmp)
        {
            Bitmap bmpTemp = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), PixelFormat.Format24bppRgb);

            BitmapData bmpData = bmpTemp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr Scan0 = bmpData.Scan0;

            try
            {
                unsafe
                {

                    int rr, gg, bb, r1, r2, r3, r4, fxr, fyr, i, j;
                    int g1, g2, g3, g4, fxg, fyg, b1, b2, b3, b4, fxb, fyb;

                    byte* scan0 = (byte*)(void*)Scan0;
                    byte* pucPtr;
                    byte* pucStart;

                    int xmin = rectbmp.X;
                    int ymin = rectbmp.Y;
                    int xmax = xmin + rectbmp.Width;
                    int ymax = ymin + rectbmp.Height;

                    int x = xmin;
                    int y = ymin;
                    int iStride = bmpData.Stride;

                    y = ymin;
                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                    while (y < ymax - 1)
                    {
                        x = xmin;
                        pucPtr = pucStart;
                        while (x < xmax - 1)
                        {

                            r1 = *pucPtr;
                            r2 = *(pucPtr + iStride + 3);
                            r3 = *(pucPtr + iStride);
                            r4 = *(pucPtr + 3);
                            fxr = r1 - r2;
                            fyr = r3 - r4;
                            rr = Math.Abs(fxr) + Math.Abs(fyr) + 128;
                            if (rr < 0)
                                rr = 0;
                            if (rr > 255)
                                rr = 255;

                            g1 = *(pucPtr + 1);
                            g2 = *(pucPtr + 1 + iStride + 3);
                            g3 = *(pucPtr + 1 + iStride);
                            g4 = *(pucPtr + 1 + 3);
                            fxg = g1 - g2;
                            fyg = g3 - g4;
                            gg = Math.Abs(fxg) + Math.Abs(fyg) + 128;
                            if (gg < 0) gg = 0;
                            if (gg > 255) gg = 255;

                            b1 = *(pucPtr + 2);
                            b2 = *(pucPtr + 2 + iStride + 3);
                            b3 = *(pucPtr + 2 + iStride);
                            b4 = *(pucPtr + 2 + 3);
                            fxb = b1 - b2;
                            fyb = b3 - b4;
                            bb = Math.Abs(fxb) + Math.Abs(fyb) + 128;
                            if (bb < 0) bb = 0;
                            if (bb > 255) bb = 255;

                            *pucPtr = (byte)rr;
                            *(pucPtr + 1) = (byte)gg;
                            *(pucPtr + 2) = (byte)bb;

                            pucPtr += 3;

                            x++;
                        }

                        pucStart += iStride;
                        y++;
                    }

                    bmpTemp.UnlockBits(bmpData);
                    return bmpTemp;
                }
            }
            catch (Exception e)
            {
                string Str = e.ToString();

                bmpTemp.UnlockBits(bmpData);
            }

            return bmpTemp;
        }
        /// <summary>
        /// 边缘检测
        /// </summary>
        /// <param name="bmp">原图</param>
        /// <param name="rectbmp">需要检测的地方</param>
        /// <returns></returns>
        public static Bitmap SetSobel(Bitmap bmp, Rectangle rectbmp)
        {
            Bitmap bmpTemp = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), PixelFormat.Format24bppRgb);

            BitmapData bmpData = bmpTemp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr Scan0 = bmpData.Scan0;

            try
            {
                unsafe
                {
                    double rr, r1, r2, r3, r4, r5, r6, r7, r8, r9, fxr, fyr, i, j;


                    byte* scan0 = (byte*)(void*)Scan0;
                    byte* pucPtr;
                    byte* pucStart;

                    int xmin = rectbmp.X;
                    int ymin = rectbmp.Y;
                    int xmax = xmin + rectbmp.Width;
                    int ymax = ymin + rectbmp.Height;

                    int x = xmin;
                    int y = ymin;
                    int iStride = bmpData.Stride;

                    y = ymin;
                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                    while (y < ymax - 2)
                    {
                        x = xmin;
                        //if (y == ymin)
                        //    break;

                        pucStart += iStride;
                        pucPtr = pucStart + 3;
                        while (x < xmax - 2)
                        {
                            //if (x == xmin)
                            //    break;
                            //c1 = box1.GetPixel(i, j - 1);
                            //c2 = box1.GetPixel(i - 1, j);
                            //c3 = box1.GetPixel(i, j);
                            //c4 = box1.GetPixel(i + 1, j);
                            //c5 = box1.GetPixel(i, j + 1);
                            //c6 = box1.GetPixel(i - 1, j - 1);
                            //c7 = box1.GetPixel(i - 1, j + 1);
                            //c8 = box1.GetPixel(i + 1, j - 1);
                            //c9 = box1.GetPixel(i + 1, j + 1);

                            r1 = *(pucPtr - iStride);
                            r2 = *(pucPtr - 3);
                            r3 = *(pucPtr);
                            r4 = *(pucPtr + 3);
                            r5 = *(pucPtr + iStride);
                            r6 = *(pucPtr - iStride - 3);
                            r7 = *(pucPtr - 3 + iStride);
                            r8 = *(pucPtr + 3 - iStride);
                            r9 = *(pucPtr + 3 + iStride);

                            fxr = r6 + 2 * r2 + r7 - r8 - 2 * r4 - r9;
                            fyr = r6 + 2 * r1 + r8 - r7 - 2 * r5 - r9;
                            rr = (Math.Abs(fxr) + Math.Abs(fyr)) / 2;
                            if (rr < 0)
                                rr = 0;
                            if (rr > 255)
                                rr = 255;


                            *pucPtr = (byte)rr;
                            *(pucPtr + 1) = (byte)rr;
                            *(pucPtr + 2) = (byte)rr;


                            pucPtr += 3;
                            x++;
                        }


                        y++;
                    }

                    bmpTemp.UnlockBits(bmpData);
                    return bmpTemp;
                }
            }
            catch (Exception e)
            {
                string Str = e.ToString();

                bmpTemp.UnlockBits(bmpData);
            }

            return bmpTemp;
        }
        /// <summary>  
        /// 直方图均衡化 直方图均衡化就是对图像进行非线性拉伸，重新分配图像像素值，使一定灰度范围内的像素数量大致相同  
        /// 增大对比度，从而达到图像增强的目的。是图像处理领域中利用图像直方图对对比度进行调整的方法  
        /// </summary>  
        /// <param name="srcBmp">原始图像</param>  
        /// <param name="dstBmp">处理后图像</param>  
        /// <returns>处理成功 true 失败 false</returns>  
        public static bool Balance(Bitmap srcBmp, out Bitmap dstBmp)
        {
            if (srcBmp == null)
            {
                dstBmp = null;
                return false;
            }
            int[] histogramArrayR = new int[256];//各个灰度级的像素数R  
            int[] histogramArrayG = new int[256];//各个灰度级的像素数G  
            int[] histogramArrayB = new int[256];//各个灰度级的像素数B  
            int[] tempArrayR = new int[256];
            int[] tempArrayG = new int[256];
            int[] tempArrayB = new int[256];
            byte[] pixelMapR = new byte[256];
            byte[] pixelMapG = new byte[256];
            byte[] pixelMapB = new byte[256];
            dstBmp = new Bitmap(srcBmp);
            Rectangle rt = new Rectangle(0, 0, srcBmp.Width, srcBmp.Height);
            BitmapData bmpData = dstBmp.LockBits(rt, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                //统计各个灰度级的像素个数  
                for (int i = 0; i < bmpData.Height; i++)
                {
                    byte* ptr = (byte*)bmpData.Scan0 + i * bmpData.Stride;
                    for (int j = 0; j < bmpData.Width; j++)
                    {
                        histogramArrayB[*(ptr + j * 3)]++;
                        histogramArrayG[*(ptr + j * 3 + 1)]++;
                        histogramArrayR[*(ptr + j * 3 + 2)]++;
                    }
                }
                //计算各个灰度级的累计分布函数  
                for (int i = 0; i < 256; i++)
                {
                    if (i != 0)
                    {
                        tempArrayB[i] = tempArrayB[i - 1] + histogramArrayB[i];
                        tempArrayG[i] = tempArrayG[i - 1] + histogramArrayG[i];
                        tempArrayR[i] = tempArrayR[i - 1] + histogramArrayR[i];
                    }
                    else
                    {
                        tempArrayB[0] = histogramArrayB[0];
                        tempArrayG[0] = histogramArrayG[0];
                        tempArrayR[0] = histogramArrayR[0];
                    }
                    //计算累计概率函数，并将值放缩至0~255范围内  
                    pixelMapB[i] = (byte)(255.0 * tempArrayB[i] / (bmpData.Width * bmpData.Height) + 0.5);//加0.5为了四舍五入取整  
                    pixelMapG[i] = (byte)(255.0 * tempArrayG[i] / (bmpData.Width * bmpData.Height) + 0.5);
                    pixelMapR[i] = (byte)(255.0 * tempArrayR[i] / (bmpData.Width * bmpData.Height) + 0.5);
                }
                //映射转换  
                for (int i = 0; i < bmpData.Height; i++)
                {
                    byte* ptr = (byte*)bmpData.Scan0 + i * bmpData.Stride;
                    for (int j = 0; j < bmpData.Width; j++)
                    {
                        *(ptr + j * 3) = pixelMapB[*(ptr + j * 3)];
                        *(ptr + j * 3 + 1) = pixelMapG[*(ptr + j * 3 + 1)];
                        *(ptr + j * 3 + 2) = pixelMapR[*(ptr + j * 3 + 2)];
                    }
                }
            }
            dstBmp.UnlockBits(bmpData);
            return true;
        }
        /// <summary>  
        /// 直方图均衡化 直方图均衡化就是对图像进行非线性拉伸，重新分配图像像素值，使一定灰度范围内的像素数量大致相同  
        /// 增大对比度，从而达到图像增强的目的。是图像处理领域中利用图像直方图对对比度进行调整的方法  
        /// </summary>  
        /// <param name="srcBmp">原始图像</param>  
        /// <param name="dstBmp">处理后图像</param>  
        /// <returns>直方图均值</returns>  
        public static int Balance(Bitmap srcBmp, ref Bitmap dstBmp, EnumThreshold myThreshold)
        {

            dstBmp = CreateGrayBitmap(srcBmp.Width, srcBmp.Height);
            srcBmp = ConvertToGrayBitmap(srcBmp);
            GetHistGram(srcBmp, HistGram);

            int iThr = GetThreshold(myThreshold);
            DoBinaryzation(srcBmp, ref dstBmp, iThr);
           // DrawHistGram(srcBmp, HistGram, iThr);
            return iThr;
        }
        /// <summary>  
        /// 直方图均衡化 直方图均衡化就是对图像进行非线性拉伸，重新分配图像像素值，使一定灰度范围内的像素数量大致相同  
        /// 增大对比度，从而达到图像增强的目的。是图像处理领域中利用图像直方图对对比度进行调整的方法  
        /// </summary>  
        /// <param name="srcBmp">原始图像</param>  
        /// <param name="dstBmp">处理后图像</param>  
        /// <returns>直方图均值</returns>  
        public static int Balance24(Bitmap srcBmp, ref Bitmap dstBmp, EnumThreshold myThreshold)
        {

            dstBmp = CreateGrayBitmap(srcBmp.Width, srcBmp.Height);
            srcBmp = ConvertToGrayBitmap(srcBmp);
            GetHistGram(srcBmp, HistGram);

            int iThr = GetThreshold(myThreshold);
            DoBinaryzation(srcBmp, ref dstBmp, iThr);
            // DrawHistGram(srcBmp, HistGram, iThr);
            return iThr;
        }
        /// <summary>  
        /// 直方图均衡化 直方图均衡化就是对图像进行非线性拉伸，重新分配图像像素值，使一定灰度范围内的像素数量大致相同  
        /// 增大对比度，从而达到图像增强的目的。是图像处理领域中利用图像直方图对对比度进行调整的方法  
        /// </summary>  
        /// <param name="srcBmp">原始图像</param>  
        /// <param name="dstBmp">处理后图像</param>  
        /// <param name="dstBmp">背景亮度</param>  
        /// <returns>直方图均值</returns>  
        public static int Balance(Bitmap srcBmp, ref Bitmap dstBmp,ref int iBackColor, EnumThreshold myThreshold)
        {

            dstBmp = CreateGrayBitmap(srcBmp.Width, srcBmp.Height);
            srcBmp = ConvertToGrayBitmap(srcBmp);
            GetHistGram(srcBmp, HistGram);

            int iThr = GetThreshold(myThreshold);
            DoBinaryzation(srcBmp, ref dstBmp, iThr);


            SetBimap_A_BFormat8(srcBmp, dstBmp, ref iBackColor);
            // DrawHistGram(srcBmp, HistGram, iThr);
            return iThr;
        }
        /// <summary>  
        /// 直方图均衡化 直方图均衡化就是对图像进行非线性拉伸，重新分配图像像素值，使一定灰度范围内的像素数量大致相同  
        /// 增大对比度，从而达到图像增强的目的。是图像处理领域中利用图像直方图对对比度进行调整的方法  
        /// </summary>  
        /// <param name="srcBmp">原始图像</param>  
        /// <param name="dstBmp">处理后图像</param>  
        /// <param name="dstBmp">背景亮度</param>  
        /// <returns>直方图均值</returns>  
        public static int Balance(Bitmap srcBmp, ref Bitmap dstBmp, ref int iBackColor, ref int iForeColor, EnumThreshold myThreshold)
        {

            dstBmp = CreateGrayBitmap(srcBmp.Width, srcBmp.Height);
            srcBmp = ConvertToGrayBitmap(srcBmp);
            GetHistGram(srcBmp, HistGram);

            int iThr = GetThreshold(myThreshold);
            DoBinaryzation(srcBmp, ref dstBmp, iThr);


            SetBimap_A_BFormat8(srcBmp, dstBmp, ref iBackColor,ref  iForeColor);
            // DrawHistGram(srcBmp, HistGram, iThr);
            return iThr;
        }
        void TIFF()
        {
            int x = 500;   //欲搜尋pixel的X位置 最小設定為1
            int y = 500;   //欲搜尋pixel的Y位置 最小設定為1

            System.IO.Stream imageStreamSource = new System.IO.FileStream("DispImg.tiff", System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read); //將檔案位置放置在第一個參數
            System.Windows.Media.Imaging.TiffBitmapDecoder decoder = new TiffBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            BitmapSource bitmapSource = decoder.Frames[0];

            if (bitmapSource.Format == System.Windows.Media.PixelFormats.Gray16)
            {

                Bitmap disparityImage16Bit = new Bitmap(bitmapSource.PixelWidth, bitmapSource.PixelHeight, System.Drawing.Imaging.PixelFormat.Format16bppGrayScale);

                System.Drawing.Imaging.BitmapData dispData = disparityImage16Bit.LockBits(
                new System.Drawing.Rectangle(0, 0, disparityImage16Bit.Width, disparityImage16Bit.Height),
                ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format16bppGrayScale);

                int m_oriHeight = disparityImage16Bit.Height;
                ushort[] dispInShort = new ushort[m_oriHeight * (dispData.Stride) / 2];
                bitmapSource.CopyPixels(dispInShort, dispData.Stride, 0);


                int pixel_address = ((y - 1) * disparityImage16Bit.Width) + (x - 1); //求出pixel(x,y) 在矩陣裡的位置

                System.Console.WriteLine(dispInShort[pixel_address]); //找出pixe(x,y)的灰階值



                disparityImage16Bit.UnlockBits(dispData);

                disparityImage16Bit.Dispose();

            }
        }
        public enum Direction
        {
            Up,
            Down,
            Right,
            Left
        }
        public enum EnumThreshold
        {
            /// <summary>
            ///  "灰度平均值"
            /// </summary>
            Mean,
            /// <summary>
            /// "黄式模糊阈值":
            /// </summary>
            HuangFuzzy,
            /// <summary>
            ///   谷底最小值
            /// </summary>
            Minimum,
            /// <summary>
            ///  "双峰平均值"
            /// </summary>
            Intermodes,
            /// <summary>
            ///  百分比阈值
            /// </summary>
            PTile,
            /// <summary>
            ///  "迭代阈值法"
            /// </summary>
            IterativeBest,
            /// <summary>
            ///  "大津法"
            /// </summary>
            OSTU,
            /// <summary>
            ///  "一维最大熵"
            /// </summary>
            MaxEntropy1D,
            /// <summary>
            ///   "动能保持"
            /// </summary>
            MomentPreserving,
            /// <summary>
            ///   "Kittler最小错误"
            /// </summary>
            KittlerMinError,
            /// <summary>
            ///     "ISODATA法"
            /// </summary>
            IsoData,
            /// <summary>
            ///   "Shanbhag法"
            /// </summary>
            Shanbhag,
            /// <summary>
            ///   "Yen法"
            /// </summary>
            Yen,
        }
        public static myImageProcessor.EnumThreshold myOCRThreshold = EnumThreshold.Minimum;
        private static int[] HistGram = new int[256];
        private static int[] HistGramS = new int[256];
        private static int GetThreshold(EnumThreshold myThreshold)
        {
            switch (myThreshold)
            {
                case EnumThreshold.Mean:// "灰度平均值":
                    return Threshold.GetMeanThreshold(HistGram);
                case EnumThreshold.HuangFuzzy:// "黄式模糊阈值":
                    return Threshold.GetHuangFuzzyThreshold(HistGram);
                case EnumThreshold.Minimum:// "谷底最小值":
                    return Threshold.GetMinimumThreshold(HistGram, HistGramS);
                case EnumThreshold.Intermodes:// "双峰平均值":
                    return Threshold.GetIntermodesThreshold(HistGram, HistGramS);
                case EnumThreshold.PTile:// "百分比阈值":
                    return Threshold.GetPTileThreshold(HistGram);
                case EnumThreshold.IterativeBest://"迭代阈值法":
                    return Threshold.GetIterativeBestThreshold(HistGram);
                case EnumThreshold.OSTU:// "大津法":
                    return Threshold.GetOSTUThreshold(HistGram);
                case EnumThreshold.MaxEntropy1D:// "一维最大熵":
                    return Threshold.Get1DMaxEntropyThreshold(HistGram);
                case EnumThreshold.MomentPreserving:// "动能保持":
                    return Threshold.GetMomentPreservingThreshold(HistGram);
                case EnumThreshold.KittlerMinError:// "Kittler最小错误":
                    return Threshold.GetKittlerMinError(HistGram);
                case EnumThreshold.IsoData:// "ISODATA法":
                    return Threshold.GetIsoDataThreshold(HistGram);
                case EnumThreshold.Shanbhag:// "Shanbhag法":
                    return Threshold.GetShanbhagThreshold(HistGram);
                case EnumThreshold.Yen:// "Yen法":
                    return Threshold.GetYenThreshold(HistGram);
                default:
                    break;
            }
            return -1;
        }
        public static Bitmap ConvertToGrayBitmap(Bitmap Src)
        {
            Bitmap Dest = CreateGrayBitmap(Src.Width, Src.Height);
            BitmapData SrcData = Src.LockBits(new Rectangle(0, 0, Src.Width, Src.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData DestData = Dest.LockBits(new Rectangle(0, 0, Dest.Width, Dest.Height), ImageLockMode.ReadWrite, Dest.PixelFormat);
            int Width = SrcData.Width, Height = SrcData.Height;
            int SrcStride = SrcData.Stride, DestStride = DestData.Stride;
            unsafe
            {
                byte* SrcP, DestP;
                for (int Y = 0; Y < Height; Y++)
                {
                    SrcP = (byte*)SrcData.Scan0 + Y * SrcStride;         // 必须在某个地方开启unsafe功能，其实C#中的unsafe很safe，搞的好吓人。            
                    DestP = (byte*)DestData.Scan0 + Y * DestStride;
                    for (int X = 0; X < Width; X++)
                    {
                        *DestP = (byte)((*SrcP + (*(SrcP + 1) << 1) + *(SrcP + 2)) >> 2);
                        SrcP += 3;
                        DestP++;
                    }
                }
            }
            Src.UnlockBits(SrcData);
            Dest.UnlockBits(DestData);
            return Dest;
        }
        private static Bitmap CreateGrayBitmap(int Width, int Height)
        {
            Bitmap Bmp = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed);
            ColorPalette Pal = Bmp.Palette;
            for (int Y = 0; Y < Pal.Entries.Length; Y++)
                Pal.Entries[Y] = Color.FromArgb(255, Y, Y, Y);
            Bmp.Palette = Pal;
            return Bmp;
        }
        private static void GetHistGram(Bitmap Src, int[] HistGram)
        {
            BitmapData SrcData = Src.LockBits(new Rectangle(0, 0, Src.Width, Src.Height), ImageLockMode.ReadWrite, Src.PixelFormat);
            int Width = SrcData.Width, Height = SrcData.Height, SrcStride = SrcData.Stride;
            unsafe
            {
                byte* SrcP;
                for (int Y = 0; Y < 256; Y++)
                    HistGram[Y] = 0;
                for (int Y = 0; Y < Height; Y++)
                {
                    SrcP = (byte*)SrcData.Scan0 + Y * SrcStride;
                    for (int X = 0; X < Width; X++, SrcP++)
                        HistGram[*SrcP]++;
                }
            }
            Src.UnlockBits(SrcData);
        }
        private static void DrawHistGram(Bitmap SrcBmp, int[] Histgram, int Thr)
        {
            BitmapData HistData = SrcBmp.LockBits(new Rectangle(0, 0, SrcBmp.Width, SrcBmp.Height), ImageLockMode.ReadWrite, SrcBmp.PixelFormat);
            int X, Y, Max = 0;
            unsafe
            {
                byte* P;
                for (Y = 0; Y < 256; Y++) if (Max < Histgram[Y]) Max = Histgram[Y];
                for (X = 0; X < 256; X++)
                {
                    P = (byte*)HistData.Scan0 + X;
                    for (Y = 0; Y < 100; Y++)
                    {
                        if ((100 - Y) > Histgram[X] * 100 / Max)
                            *P = 220;
                        else
                            *P = 0;
                        P += HistData.Stride;
                    }
                }

                P = (byte*)HistData.Scan0 + Thr;
                for (Y = 0; Y < 100; Y++)
                {
                    *P = 255;
                    P += HistData.Stride;
                }
            }
            SrcBmp.UnlockBits(HistData);
        }
        /// <summary>
        /// 还原图像
        /// </summary>
        /// <param name="Src"></param>
        /// <param name="Dest"></param>
        /// <param name="Threshold"></param>
        private static void DoBinaryzation(Bitmap Src, ref Bitmap Dest, int Threshold)
        {
            //"选择了非法的阈值变量."
            if (Threshold == -1)
                return;

            BitmapData SrcData = Src.LockBits(new Rectangle(0, 0, Src.Width, Src.Height), ImageLockMode.ReadWrite, Src.PixelFormat);
            BitmapData DestData = Dest.LockBits(new Rectangle(0, 0, Dest.Width, Dest.Height), ImageLockMode.ReadWrite, Dest.PixelFormat);
            int Width = SrcData.Width, Height = SrcData.Height;
            int SrcStride = SrcData.Stride, DestStride = DestData.Stride;
            unsafe
            {
                byte* SrcP, DestP;
                for (int Y = 0; Y < Height; Y++)
                {
                    SrcP = (byte*)SrcData.Scan0 + Y * SrcStride;         // 必须在某个地方开启unsafe功能，其实C#中的unsafe很safe，搞的好吓人。            
                    DestP = (byte*)DestData.Scan0 + Y * DestStride;
                    for (int X = 0; X < Width; X++, SrcP++, DestP++)
                        *DestP = *SrcP > Threshold ? byte.MaxValue : byte.MinValue;     // 写成255和0，C#编译器不认。
                }
            }
            Src.UnlockBits(SrcData);
            Dest.UnlockBits(DestData);

        }
    }
    public static class Threshold
    {
        /// <summary>
        /// 基于灰度平均值的阈值
        /// </summary>
        /// <param name="HistGram">灰度图像的直方图</param>
        /// <returns></returns>
        public static int GetMeanThreshold(int[] HistGram)
        {
            int Sum = 0, Amount = 0;
            for (int Y = 0; Y < 256; Y++)
            {
                Amount += HistGram[Y];
                Sum += Y * HistGram[Y];
            }
            return Sum / Amount;
        }
        /// <summary>
        /// 基于模糊集的黄式阈值算法
        /// http://www.ktl.elf.stuba.sk/study/vacso/Zadania-Cvicenia/Cvicenie_3/TimA2/Huang_E016529624.pdf
        /// </summary>
        /// <param name="HistGram">灰度图像的直方图</param>
        /// <returns></returns>
        public static int GetHuangFuzzyThreshold(int[] HistGram)
        {
            int X, Y;
            int First, Last;
            int Threshold = -1;
            double BestEntropy = Double.MaxValue, Entropy;
            //   找到第一个和最后一个非0的色阶值
            for (First = 0; First < HistGram.Length && HistGram[First] == 0; First++) ;
            for (Last = HistGram.Length - 1; Last > First && HistGram[Last] == 0; Last--) ;
            if (First == Last) return First;                // 图像中只有一个颜色
            if (First + 1 == Last) return First;            // 图像中只有二个颜色

            // 计算累计直方图以及对应的带权重的累计直方图
            int[] S = new int[Last + 1];
            int[] W = new int[Last + 1];            // 对于特大图，此数组的保存数据可能会超出int的表示范围，可以考虑用long类型来代替
            S[0] = HistGram[0];
            for (Y = First > 1 ? First : 1; Y <= Last; Y++)
            {
                S[Y] = S[Y - 1] + HistGram[Y];
                W[Y] = W[Y - 1] + Y * HistGram[Y];
            }

            // 建立公式（4）及（6）所用的查找表
            double[] Smu = new double[Last + 1 - First];
            for (Y = 1; Y < Smu.Length; Y++)
            {
                double mu = 1 / (1 + (double)Y / (Last - First));               // 公式（4）
                Smu[Y] = -mu * Math.Log(mu) - (1 - mu) * Math.Log(1 - mu);      // 公式（6）
            }

            // 迭代计算最佳阈值
            for (Y = First; Y <= Last; Y++)
            {
                Entropy = 0;
                int mu = (int)Math.Round((double)W[Y] / S[Y]);             // 公式17
                for (X = First; X <= Y; X++)
                    Entropy += Smu[Math.Abs(X - mu)] * HistGram[X];
                mu = (int)Math.Round((double)(W[Last] - W[Y]) / (S[Last] - S[Y]));  // 公式18       
                for (X = Y + 1; X <= Last; X++)
                    Entropy += Smu[Math.Abs(X - mu)] * HistGram[X];       // 公式8
                if (BestEntropy > Entropy)
                {
                    BestEntropy = Entropy;      // 取最小熵处为最佳阈值
                    Threshold = Y;
                }
            }
            return Threshold;
        }
        /// <summary>
        /// 基于谷底最小值的阈值
        /// 此方法实用于具有明显双峰直方图的图像，其寻找双峰的谷底作为阈值
        /// References: 
        /// J. M. S. Prewitt and M. L. Mendelsohn, "The analysis of cell images," in
        /// nnals of the New York Academy of Sciences, vol. 128, pp. 1035-1053, 1966.
        /// C. A. Glasbey, "An analysis of histogram-based thresholding algorithms,"
        /// CVGIP: Graphical Models and Image Processing, vol. 55, pp. 532-537, 1993.
        /// </summary>
        /// <param name="HistGram">灰度图像的直方图</param>
        /// <param name="HistGramS">返回平滑后的直方图</param>
        /// <returns></returns>
        public static int GetMinimumThreshold(int[] HistGram, int[] HistGramS)
        {
            int Y, Iter = 0;
            double[] HistGramC = new double[256];           // 基于精度问题，一定要用浮点数来处理，否则得不到正确的结果
            double[] HistGramCC = new double[256];          // 求均值的过程会破坏前面的数据，因此需要两份数据
            for (Y = 0; Y < 256; Y++)
            {
                HistGramC[Y] = HistGram[Y];
                HistGramCC[Y] = HistGram[Y];
            }

            // 通过三点求均值来平滑直方图
            while (IsDimodal(HistGramCC) == false)                                        // 判断是否已经是双峰的图像了      
            {
                HistGramCC[0] = (HistGramC[0] + HistGramC[0] + HistGramC[1]) / 3;                 // 第一点
                for (Y = 1; Y < 255; Y++)
                    HistGramCC[Y] = (HistGramC[Y - 1] + HistGramC[Y] + HistGramC[Y + 1]) / 3;     // 中间的点
                HistGramCC[255] = (HistGramC[254] + HistGramC[255] + HistGramC[255]) / 3;         // 最后一点
                System.Buffer.BlockCopy(HistGramCC, 0, HistGramC, 0, 256 * sizeof(double));
                Iter++;
                if (Iter >= 1000) return -1;                                                   // 直方图无法平滑为双峰的，返回错误代码
            }
            for (Y = 0; Y < 256; Y++) HistGramS[Y] = (int)HistGramCC[Y];
            // 阈值极为两峰之间的最小值 
            bool Peakfound = false;
            for (Y = 1; Y < 255; Y++)
            {
                if (HistGramCC[Y - 1] < HistGramCC[Y] && HistGramCC[Y + 1] < HistGramCC[Y]) Peakfound = true;
                if (Peakfound == true && HistGramCC[Y - 1] >= HistGramCC[Y] && HistGramCC[Y + 1] >= HistGramCC[Y])
                    return Y - 1;
            }
            return -1;
        }
        /// <summary>
        /// 基于双峰平均值的阈值
        /// 此方法实用于具有明显双峰直方图的图像，其寻找双峰的谷底作为阈值
        /// References: 
        /// J. M. S. Prewitt and M. L. Mendelsohn, "The analysis of cell images," in
        /// nnals of the New York Academy of Sciences, vol. 128, pp. 1035-1053, 1966.
        /// C. A. Glasbey, "An analysis of histogram-based thresholding algorithms,"
        /// CVGIP: Graphical Models and Image Processing, vol. 55, pp. 532-537, 1993.
        /// </summary>
        /// <param name="HistGram">灰度图像的直方图</param>
        /// <param name="HistGramS">返回平滑后的直方图</param>
        /// <returns></returns>
        public static int GetIntermodesThreshold(int[] HistGram, int[] HistGramS)
        {
            int Y, Iter = 0, Index;
            double[] HistGramC = new double[256];           // 基于精度问题，一定要用浮点数来处理，否则得不到正确的结果
            double[] HistGramCC = new double[256];          // 求均值的过程会破坏前面的数据，因此需要两份数据
            for (Y = 0; Y < 256; Y++)
            {
                HistGramC[Y] = HistGram[Y];
                HistGramCC[Y] = HistGram[Y];
            }
            // 通过三点求均值来平滑直方图
            while (IsDimodal(HistGramCC) == false)                                                  // 判断是否已经是双峰的图像了      
            {
                HistGramCC[0] = (HistGramC[0] + HistGramC[0] + HistGramC[1]) / 3;                   // 第一点
                for (Y = 1; Y < 255; Y++)
                    HistGramCC[Y] = (HistGramC[Y - 1] + HistGramC[Y] + HistGramC[Y + 1]) / 3;       // 中间的点
                HistGramCC[255] = (HistGramC[254] + HistGramC[255] + HistGramC[255]) / 3;           // 最后一点
                System.Buffer.BlockCopy(HistGramCC, 0, HistGramC, 0, 256 * sizeof(double));         // 备份数据，为下一次迭代做准备
                Iter++;
                if (Iter >= 10000) return -1;                                                       // 似乎直方图无法平滑为双峰的，返回错误代码
            }
            for (Y = 0; Y < 256; Y++) HistGramS[Y] = (int)HistGramCC[Y];
            // 阈值为两峰值的平均值
            int[] Peak = new int[2];
            for (Y = 1, Index = 0; Y < 255; Y++)
                if (HistGramCC[Y - 1] < HistGramCC[Y] && HistGramCC[Y + 1] < HistGramCC[Y]) Peak[Index++] = Y - 1;
            return ((Peak[0] + Peak[1]) / 2);
        }
        /// <summary>
        /// 百分比阈值
        /// </summary>
        /// <param name="HistGram">灰度图像的直方图</param>
        /// <param name="Tile">背景在图像中所占的面积百分比</param>
        /// <returns></returns>
        public static int GetPTileThreshold(int[] HistGram, int Tile = 50)
        {
            int Y, Amount = 0, Sum = 0;
            for (Y = 0; Y < 256; Y++) Amount += HistGram[Y];        //  像素总数
            for (Y = 0; Y < 256; Y++)
            {
                Sum = Sum + HistGram[Y];
                if (Sum >= Amount * Tile / 100) return Y;
            }
            return -1;
        }
        /// <summary>
        /// 迭代法获得阈值
        /// </summary>
        /// <param name="HistGram">灰度图像的直方图</param>
        /// <returns></returns>
        public static int GetIterativeBestThreshold(int[] HistGram)
        {
            int X, Iter = 0;
            int MeanValueOne, MeanValueTwo, SumOne, SumTwo, SumIntegralOne, SumIntegralTwo;
            int MinValue, MaxValue;
            int Threshold, NewThreshold;

            for (MinValue = 0; MinValue < 256 && HistGram[MinValue] == 0; MinValue++) ;
            for (MaxValue = 255; MaxValue > MinValue && HistGram[MinValue] == 0; MaxValue--) ;

            if (MaxValue == MinValue) return MaxValue;          // 图像中只有一个颜色             
            if (MinValue + 1 == MaxValue) return MinValue;      // 图像中只有二个颜色

            Threshold = MinValue;
            NewThreshold = (MaxValue + MinValue) >> 1;
            while (Threshold != NewThreshold)    // 当前后两次迭代的获得阈值相同时，结束迭代    
            {
                SumOne = 0; SumIntegralOne = 0;
                SumTwo = 0; SumIntegralTwo = 0;
                Threshold = NewThreshold;
                for (X = MinValue; X <= Threshold; X++)         //根据阈值将图像分割成目标和背景两部分，求出两部分的平均灰度值      
                {
                    SumIntegralOne += HistGram[X] * X;
                    SumOne += HistGram[X];
                }
                MeanValueOne = SumIntegralOne / SumOne;
                for (X = Threshold + 1; X <= MaxValue; X++)
                {
                    SumIntegralTwo += HistGram[X] * X;
                    SumTwo += HistGram[X];
                }
                if (SumIntegralTwo != 0)
                    MeanValueTwo = SumIntegralTwo / SumTwo;
                else
                    MeanValueTwo = 0;
                NewThreshold = (MeanValueOne + MeanValueTwo) >> 1;       //求出新的阈值
                Iter++;
                if (Iter >= 1000) return -1;
            }
            return Threshold;
        }
        /// <summary>
        /// 大津法
        /// </summary>
        /// <param name="HistGram"></param>
        /// <returns></returns>
        public static int GetOSTUThreshold(int[] HistGram)
        {
            int X, Y, Amount = 0;
            int PixelBack = 0, PixelFore = 0, PixelIntegralBack = 0, PixelIntegralFore = 0, PixelIntegral = 0;
            double OmegaBack, OmegaFore, MicroBack, MicroFore, SigmaB, Sigma;              // 类间方差;
            int MinValue, MaxValue;
            int Threshold = 0;

            for (MinValue = 0; MinValue < 256 && HistGram[MinValue] == 0; MinValue++) ;
            for (MaxValue = 255; MaxValue > MinValue && HistGram[MinValue] == 0; MaxValue--) ;
            if (MaxValue == MinValue) return MaxValue;          // 图像中只有一个颜色             
            if (MinValue + 1 == MaxValue) return MinValue;      // 图像中只有二个颜色

            for (Y = MinValue; Y <= MaxValue; Y++) Amount += HistGram[Y];        //  像素总数

            PixelIntegral = 0;
            for (Y = MinValue; Y <= MaxValue; Y++) PixelIntegral += HistGram[Y] * Y;
            SigmaB = -1;
            for (Y = MinValue; Y < MaxValue; Y++)
            {
                PixelBack = PixelBack + HistGram[Y];
                PixelFore = Amount - PixelBack;
                OmegaBack = (double)PixelBack / Amount;
                OmegaFore = (double)PixelFore / Amount;
                PixelIntegralBack += HistGram[Y] * Y;
                PixelIntegralFore = PixelIntegral - PixelIntegralBack;
                MicroBack = (double)PixelIntegralBack / PixelBack;
                MicroFore = (double)PixelIntegralFore / PixelFore;
                Sigma = OmegaBack * OmegaFore * (MicroBack - MicroFore) * (MicroBack - MicroFore);
                if (Sigma > SigmaB)
                {
                    SigmaB = Sigma;
                    Threshold = Y;
                }
            }
            return Threshold;
        }
        /// <summary>
        /// 一维最大熵
        /// </summary>
        /// <param name="HistGram"></param>
        /// <returns></returns>
        public static int Get1DMaxEntropyThreshold(int[] HistGram)
        {
            int X, Y, Amount = 0;
            double[] HistGramD = new double[256];
            double SumIntegral, EntropyBack, EntropyFore, MaxEntropy;
            int MinValue = 255, MaxValue = 0;
            int Threshold = 0;

            for (MinValue = 0; MinValue < 256 && HistGram[MinValue] == 0; MinValue++) ;
            for (MaxValue = 255; MaxValue > MinValue && HistGram[MinValue] == 0; MaxValue--) ;
            if (MaxValue == MinValue) return MaxValue;          // 图像中只有一个颜色             
            if (MinValue + 1 == MaxValue) return MinValue;      // 图像中只有二个颜色

            for (Y = MinValue; Y <= MaxValue; Y++) Amount += HistGram[Y];        //  像素总数

            for (Y = MinValue; Y <= MaxValue; Y++) HistGramD[Y] = (double)HistGram[Y] / Amount + 1e-17;

            MaxEntropy = double.MinValue; ;
            for (Y = MinValue + 1; Y < MaxValue; Y++)
            {
                SumIntegral = 0;
                for (X = MinValue; X <= Y; X++) SumIntegral += HistGramD[X];
                EntropyBack = 0;
                for (X = MinValue; X <= Y; X++) EntropyBack += (-HistGramD[X] / SumIntegral * Math.Log(HistGramD[X] / SumIntegral));
                EntropyFore = 0;
                for (X = Y + 1; X <= MaxValue; X++) EntropyFore += (-HistGramD[X] / (1 - SumIntegral) * Math.Log(HistGramD[X] / (1 - SumIntegral)));
                if (MaxEntropy < EntropyBack + EntropyFore)
                {
                    Threshold = Y;
                    MaxEntropy = EntropyBack + EntropyFore;
                }
            }
            return Threshold;
        }
        // http://fiji.sc/wiki/index.php/Auto_Threshold#Huang
        //   W. Tsai, "Moment-preserving thresholding: a new approach," Computer
        //   Vision, Graphics, and Image Processing, vol. 29, pp. 377-393, 1985.
        //
        //  C. A. Glasbey, "An analysis of histogram-based thresholding algorithms,"
        //  CVGIP: Graphical Models and Image Processing, vol. 55, pp. 532-537, 1993.
        /// <summary>
        /// 动能保持
        /// </summary>
        /// <param name="HistGram"></param>
        /// <returns></returns>
        public static byte GetMomentPreservingThreshold(int[] HistGram)
        {
            int X, Y, Index = 0, Amount = 0;
            double[] Avec = new double[256];
            double X2, X1, X0, Min;

            for (Y = 0; Y <= 255; Y++) Amount += HistGram[Y];        //  像素总数
            for (Y = 0; Y < 256; Y++) Avec[Y] = (double)A(HistGram, Y) / Amount;       // The threshold is chosen such that A(y,t)/A(y,n) is closest to x0.

            // The following finds x0.

            X2 = (double)(B(HistGram, 255) * C(HistGram, 255) - A(HistGram, 255) * D(HistGram, 255)) / (double)(A(HistGram, 255) * C(HistGram, 255) - B(HistGram, 255) * B(HistGram, 255));
            X1 = (double)(B(HistGram, 255) * D(HistGram, 255) - C(HistGram, 255) * C(HistGram, 255)) / (double)(A(HistGram, 255) * C(HistGram, 255) - B(HistGram, 255) * B(HistGram, 255));
            X0 = 0.5 - (B(HistGram, 255) / A(HistGram, 255) + X2 / 2) / Math.Sqrt(X2 * X2 - 4 * X1);

            for (Y = 0, Min = double.MaxValue; Y < 256; Y++)
            {
                if (Math.Abs(Avec[Y] - X0) < Min)
                {
                    Min = Math.Abs(Avec[Y] - X0);
                    Index = Y;
                }
            }
            return (byte)Index;
        }
        /// <summary>
        /// Kittler最小错误
        /// </summary>
        /// <param name="HistGram"></param>
        /// <returns></returns>
        public static int GetKittlerMinError(int[] HistGram)
        {
            int X, Y;
            int MinValue, MaxValue;
            int Threshold;
            int PixelBack, PixelFore;
            double OmegaBack, OmegaFore, MinSigma, Sigma, SigmaBack, SigmaFore;
            for (MinValue = 0; MinValue < 256 && HistGram[MinValue] == 0; MinValue++) ;
            for (MaxValue = 255; MaxValue > MinValue && HistGram[MinValue] == 0; MaxValue--) ;
            if (MaxValue == MinValue) return MaxValue;          // 图像中只有一个颜色             
            if (MinValue + 1 == MaxValue) return MinValue;      // 图像中只有二个颜色
            Threshold = -1;
            MinSigma = 1E+20;
            for (Y = MinValue; Y < MaxValue; Y++)
            {
                PixelBack = 0; PixelFore = 0;
                OmegaBack = 0; OmegaFore = 0;
                for (X = MinValue; X <= Y; X++)
                {
                    PixelBack += HistGram[X];
                    OmegaBack = OmegaBack + X * HistGram[X];
                }
                for (X = Y + 1; X <= MaxValue; X++)
                {
                    PixelFore += HistGram[X];
                    OmegaFore = OmegaFore + X * HistGram[X];
                }
                OmegaBack = OmegaBack / PixelBack;
                OmegaFore = OmegaFore / PixelFore;
                SigmaBack = 0; SigmaFore = 0;
                for (X = MinValue; X <= Y; X++) SigmaBack = SigmaBack + (X - OmegaBack) * (X - OmegaBack) * HistGram[X];
                for (X = Y + 1; X <= MaxValue; X++) SigmaFore = SigmaFore + (X - OmegaFore) * (X - OmegaFore) * HistGram[X];
                if (SigmaBack == 0 || SigmaFore == 0)
                {
                    if (Threshold == -1)
                        Threshold = Y;
                }
                else
                {
                    SigmaBack = Math.Sqrt(SigmaBack / PixelBack);
                    SigmaFore = Math.Sqrt(SigmaFore / PixelFore);
                    Sigma = 1 + 2 * (PixelBack * Math.Log(SigmaBack / PixelBack) + PixelFore * Math.Log(SigmaFore / PixelFore));
                    if (Sigma < MinSigma)
                    {
                        MinSigma = Sigma;
                        Threshold = Y;
                    }
                }
            }
            return Threshold;
        }
        // Also called intermeans
        // Iterative procedure based on the isodata algorithm [T.W. Ridler, S. Calvard, Picture 
        // thresholding using an iterative selection method, IEEE Trans. System, Man and 
        // Cybernetics, SMC-8 (1978) 630-632.] 
        // The procedure divides the image into objects and background by taking an initial threshold,
        // then the averages of the pixels at or below the threshold and pixels above are computed. 
        // The averages of those two values are computed, the threshold is incremented and the 
        // process is repeated until the threshold is larger than the composite average. That is,
        //  threshold = (average background + average objects)/2
        // The code in ImageJ that implements this function is the getAutoThreshold() method in the ImageProcessor class. 
        //
        // From: Tim Morris (dtm@ap.co.umist.ac.uk)
        // Subject: Re: Thresholding method?
        // posted to sci.image.processing on 1996/06/24
        // The algorithm implemented in NIH Image sets the threshold as that grey
        // value, G, for which the average of the averages of the grey values
        // below and above G is equal to G. It does this by initialising G to the
        // lowest sensible value and iterating:

        // L = the average grey value of pixels with intensities < G
        // H = the average grey value of pixels with intensities > G
        // is G = (L + H)/2?
        // yes => exit
        // no => increment G and repeat
        //
        // There is a discrepancy with IJ because they are slightly different methods
        /// <summary>
        /// ISODATA法
        /// </summary>
        /// <param name="HistGram"></param>
        /// <returns></returns>
        public static int GetIsoDataThreshold(int[] HistGram)
        {
            int i, l, toth, totl, h, g = 0;
            for (i = 1; i < HistGram.Length; i++)
            {
                if (HistGram[i] > 0)
                {
                    g = i + 1;
                    break;
                }
            }
            while (true)
            {
                l = 0;
                totl = 0;
                for (i = 0; i < g; i++)
                {
                    totl = totl + HistGram[i];
                    l = l + (HistGram[i] * i);
                }
                h = 0;
                toth = 0;
                for (i = g + 1; i < HistGram.Length; i++)
                {
                    toth += HistGram[i];
                    h += (HistGram[i] * i);
                }
                if (totl > 0 && toth > 0)
                {
                    l /= totl;
                    h /= toth;
                    if (g == (int)Math.Round((l + h) / 2.0))
                        break;
                }
                g++;
                if (g > HistGram.Length - 2)
                {
                    return 0;
                }
            }
            return g;
        }
        // Shanhbag A.G. (1994) "Utilization of Information Measure as a Means of
        //  Image Thresholding" Graphical Models and Image Processing, 56(5): 414-419
        // Ported to ImageJ plugin by G.Landini from E Celebi's fourier_0.8 routines
        /// <summary>
        /// Shanbhag法
        /// </summary>
        /// <param name="HistGram"></param>
        /// <returns></returns>
        public static int GetShanbhagThreshold(int[] HistGram)
        {
            int threshold;
            int ih, it;
            int first_bin;
            int last_bin;
            double term;
            double tot_ent;  /* total entropy */
            double min_ent;  /* max entropy */
            double ent_back; /* entropy of the background pixels at a given threshold */
            double ent_obj;  /* entropy of the object pixels at a given threshold */
            double[] norm_histo = new double[HistGram.Length]; /* normalized histogram */
            double[] P1 = new double[HistGram.Length]; /* cumulative normalized histogram */
            double[] P2 = new double[HistGram.Length];

            int total = 0;
            for (ih = 0; ih < HistGram.Length; ih++)
                total += HistGram[ih];

            for (ih = 0; ih < HistGram.Length; ih++)
                norm_histo[ih] = (double)HistGram[ih] / total;

            P1[0] = norm_histo[0];
            P2[0] = 1.0 - P1[0];
            for (ih = 1; ih < HistGram.Length; ih++)
            {
                P1[ih] = P1[ih - 1] + norm_histo[ih];
                P2[ih] = 1.0 - P1[ih];
            }

            /* Determine the first non-zero bin */
            first_bin = 0;
            for (ih = 0; ih < HistGram.Length; ih++)
            {
                if (!(Math.Abs(P1[ih]) < 2.220446049250313E-16))
                {
                    first_bin = ih;
                    break;
                }
            }

            /* Determine the last non-zero bin */
            last_bin = HistGram.Length - 1;
            for (ih = HistGram.Length - 1; ih >= first_bin; ih--)
            {
                if (!(Math.Abs(P2[ih]) < 2.220446049250313E-16))
                {
                    last_bin = ih;
                    break;
                }
            }

            // Calculate the total entropy each gray-level
            // and find the threshold that maximizes it 
            threshold = -1;
            min_ent = Double.MaxValue;

            for (it = first_bin; it <= last_bin; it++)
            {
                /* Entropy of the background pixels */
                ent_back = 0.0;
                term = 0.5 / P1[it];
                for (ih = 1; ih <= it; ih++)
                { //0+1?
                    ent_back -= norm_histo[ih] * Math.Log(1.0 - term * P1[ih - 1]);
                }
                ent_back *= term;

                /* Entropy of the object pixels */
                ent_obj = 0.0;
                term = 0.5 / P2[it];
                for (ih = it + 1; ih < HistGram.Length; ih++)
                {
                    ent_obj -= norm_histo[ih] * Math.Log(1.0 - term * P2[ih]);
                }
                ent_obj *= term;

                /* Total entropy */
                tot_ent = Math.Abs(ent_back - ent_obj);

                if (tot_ent < min_ent)
                {
                    min_ent = tot_ent;
                    threshold = it;
                }
            }
            return threshold;
        }
        // M. Emre Celebi
        // 06.15.2007
        // Ported to ImageJ plugin by G.Landini from E Celebi's fourier_0.8 routines
        /// <summary>
        /// Yen法
        /// </summary>
        /// <param name="HistGram"></param>
        /// <returns></returns>
        public static int GetYenThreshold(int[] HistGram)
        {
            int threshold;
            int ih, it;
            double crit;
            double max_crit;
            double[] norm_histo = new double[HistGram.Length]; /* normalized histogram */
            double[] P1 = new double[HistGram.Length]; /* cumulative normalized histogram */
            double[] P1_sq = new double[HistGram.Length];
            double[] P2_sq = new double[HistGram.Length];

            int total = 0;
            for (ih = 0; ih < HistGram.Length; ih++)
                total += HistGram[ih];

            for (ih = 0; ih < HistGram.Length; ih++)
                norm_histo[ih] = (double)HistGram[ih] / total;

            P1[0] = norm_histo[0];
            for (ih = 1; ih < HistGram.Length; ih++)
                P1[ih] = P1[ih - 1] + norm_histo[ih];

            P1_sq[0] = norm_histo[0] * norm_histo[0];
            for (ih = 1; ih < HistGram.Length; ih++)
                P1_sq[ih] = P1_sq[ih - 1] + norm_histo[ih] * norm_histo[ih];

            P2_sq[HistGram.Length - 1] = 0.0;
            for (ih = HistGram.Length - 2; ih >= 0; ih--)
                P2_sq[ih] = P2_sq[ih + 1] + norm_histo[ih + 1] * norm_histo[ih + 1];

            /* Find the threshold that maximizes the criterion */
            threshold = -1;
            max_crit = Double.MinValue;
            for (it = 0; it < HistGram.Length; it++)
            {
                crit = -1.0 * ((P1_sq[it] * P2_sq[it]) > 0.0 ? Math.Log(P1_sq[it] * P2_sq[it]) : 0.0) + 2 * ((P1[it] * (1.0 - P1[it])) > 0.0 ? Math.Log(P1[it] * (1.0 - P1[it])) : 0.0);
                if (crit > max_crit)
                {
                    max_crit = crit;
                    threshold = it;
                }
            }
            return threshold;
        }

        private static double A(int[] HistGram, int Index)
        {
            double Sum = 0;
            for (int Y = 0; Y <= Index; Y++)
                Sum += HistGram[Y];
            return Sum;
        }
        private static double B(int[] HistGram, int Index)
        {
            double Sum = 0;
            for (int Y = 0; Y <= Index; Y++)
                Sum += (double)Y * HistGram[Y];
            return Sum;
        }
        private static double C(int[] HistGram, int Index)
        {
            double Sum = 0;
            for (int Y = 0; Y <= Index; Y++)
                Sum += (double)Y * Y * HistGram[Y];
            return Sum;
        }
        private static double D(int[] HistGram, int Index)
        {
            double Sum = 0;
            for (int Y = 0; Y <= Index; Y++)
                Sum += (double)Y * Y * Y * HistGram[Y];
            return Sum;
        }
        /// <summary>
        /// 检测直方图是否为双峰的
        /// </summary>
        /// <param name="HistGram"></param>
        /// <returns></returns>
        private static bool IsDimodal(double[] HistGram)
        {
            // 对直方图的峰进行计数，只有峰数位2才为双峰 
            int Count = 0;
            for (int Y = 1; Y < 255; Y++)
            {
                if (HistGram[Y - 1] < HistGram[Y] && HistGram[Y + 1] < HistGram[Y])
                {
                    Count++;
                    if (Count > 2) return false;
                }
            }
            if (Count == 2)
                return true;
            else
                return false;
        }

    }
}
