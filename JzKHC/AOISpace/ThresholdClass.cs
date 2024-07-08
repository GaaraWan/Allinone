using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

using JetEazy.BasicSpace;

namespace JzKHC.AOISpace
{
    class ThresholdClass
    {
        const int Ratio = 3;
        protected JzToolsClass JzTools = new JzToolsClass();

        bool IsDebug
        {
            get
            {
                return false;
            }
        }
        public ThresholdClass()
        {

        }
        public void SetThreshold(Bitmap bmp, int ThresholdValue,int ThresholdRangeUpper,int ThresholdRangeLower)
        {
            int Grade = 0;

            Rectangle rectbmp = JzTools.SimpleRect(bmp.Size);
            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
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

                    int ThresholdValueMax = Math.Min(255, ThresholdValue + ThresholdRangeUpper);
                    int ThresholdValueMin = Math.Max(0, ThresholdValue - ThresholdRangeLower);

                    y = ymin;
                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                    while (y < ymax)
                    {
                        x = xmin;
                        pucPtr = pucStart;
                        while (x < xmax)
                        {
                            Grade = JzTools.GrayscaleInt(pucPtr[2], pucPtr[1], pucPtr[0]);

                            *((uint*)pucPtr) = (JzTools.IsInRangeEx(Grade, ThresholdValueMax, ThresholdValueMin) ? 0xFF000000 : 0xFFFFFFFF);

                            pucPtr += 4;
                            x++;
                        }

                        pucStart += iStride;
                        y++;
                    }

                    bmp.UnlockBits(bmpData);
                }
            }
            catch (Exception e)
            {
                bmp.UnlockBits(bmpData);

                if (IsDebug)
                    MessageBox.Show("Error :" + e.ToString());
            }
        }

        public void SetThreshold(Bitmap bmp, Rectangle Rect, int ThresholdValue, int ThresholdRangeUpper, int ThresholdRangeLower)
        {
            SetThreshold(bmp, Rect, ThresholdValue, ThresholdRangeUpper, ThresholdRangeLower, false);
        }
        public void SetThreshold(Bitmap bmp, Rectangle Rect,int ThresholdValue, int ThresholdRangeUpper, int ThresholdRangeLower,bool IsInRangeColorWhite)
        {
            int Grade = 0;

            Rectangle rectbmp = Rect;
            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
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

                    int ThresholdValueMax = Math.Min(255, ThresholdValue + ThresholdRangeUpper);
                    int ThresholdValueMin = Math.Max(0, ThresholdValue - ThresholdRangeLower);

                    uint InRangeColor = 0xFF000000;
                    uint OutrangeColor = 0xFFFFFFFF;

                    if (IsInRangeColorWhite)
                    {
                        InRangeColor = 0xFFFFFFFF;
                        OutrangeColor = 0xFF000000;
                    }

                    y = ymin;
                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                    while (y < ymax)
                    {
                        x = xmin;
                        pucPtr = pucStart;
                        while (x < xmax)
                        {
                            Grade = JzTools.GrayscaleInt(pucPtr[2], pucPtr[1], pucPtr[0]);

                            *((uint*)pucPtr) = (JzTools.IsInRangeEx(Grade, ThresholdValueMax, ThresholdValueMin) ? InRangeColor : OutrangeColor);

                            pucPtr += 4;
                            x++;
                        }

                        pucStart += iStride;
                        y++;
                    }

                    bmp.UnlockBits(bmpData);
                }
            }
            catch (Exception e)
            {
                bmp.UnlockBits(bmpData);

                if (IsDebug)
                    MessageBox.Show("Error :" + e.ToString());
            }
        }

        public void SetThresholdEX(Bitmap bmp, Rectangle Rect, int ThresholdValue, int ThresholdRangeUpper, int ThresholdRangeLower, bool IsInRangeColorWhite)
        {
            int Grade = 0;

            Rectangle rectbmp = Rect;
            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
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

                    int ThresholdValueMax = Math.Min(255, ThresholdValue + ThresholdRangeUpper);
                    int ThresholdValueMin = Math.Max(0, ThresholdValue - ThresholdRangeLower);

                    uint InRangeColor = 0xFF000000;
                    uint OutrangeColor = 0xFFFFFFFF;

                    if (IsInRangeColorWhite)
                    {
                        InRangeColor = 0xFFFFFFFF;
                        OutrangeColor = 0xFF000000;
                    }

                    y = ymin;
                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                    while (y < ymax)
                    {
                        x = xmin;
                        pucPtr = pucStart;
                        while (x < xmax)
                        {
                            Grade = JzTools.GrayscaleInt(pucPtr[2], pucPtr[1], pucPtr[0]);

                            if (Grade != 0)
                                *((uint*)pucPtr) = (JzTools.IsInRangeEx(Grade, ThresholdValueMax, ThresholdValueMin) ? InRangeColor : OutrangeColor);

                            pucPtr += 4;
                            x++;
                        }

                        pucStart += iStride;
                        y++;
                    }

                    bmp.UnlockBits(bmpData);
                }
            }
            catch (Exception e)
            {
                bmp.UnlockBits(bmpData);

                if (IsDebug)
                    MessageBox.Show("Error :" + e.ToString());
            }
        }


        public void SetThresholdEX(Bitmap bmp, Rectangle Rect, Color ThreshColor, bool IsInRangeColorWhite)
        {
            uint ColorValue = (uint)((ThreshColor.A << 24) + (ThreshColor.R << 16) + (ThreshColor.G << 8) + ThreshColor.B);

            Rectangle rectbmp = Rect;
            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
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

                    uint InRangeColor = 0xFF000000;
                    uint OutrangeColor = 0xFFFFFFFF;

                    if (IsInRangeColorWhite)
                    {
                        InRangeColor = 0xFFFFFFFF;
                        OutrangeColor = 0xFF000000;
                    }

                    y = ymin;
                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                    while (y < ymax)
                    {
                        x = xmin;
                        pucPtr = pucStart;
                        while (x < xmax)
                        {
                            if(*((uint*)pucPtr) == ColorValue)
                                *((uint*)pucPtr) = InRangeColor;
                            else
                                *((uint*)pucPtr) = OutrangeColor;

                            pucPtr += 4;
                            x++;
                        }

                        pucStart += iStride;
                        y++;
                    }

                    bmp.UnlockBits(bmpData);
                }
            }
            catch (Exception e)
            {
                bmp.UnlockBits(bmpData);

                if (IsDebug)
                    MessageBox.Show("Error :" + e.ToString());
            }
        }

        public void SetNet(Bitmap bmp,int Gap)
        {
            Rectangle rectbmp = JzTools.SimpleRect(bmp.Size);
            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
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
                            *((uint*)pucPtr) = 0xFF000000;

                            pucPtr += 4;
                            x++;
                        }

                        pucStart += iStride * Gap;
                        y += Gap;
                    }

                    x = xmin;
                    y = ymin;
                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                    while (x < xmax)
                    {
                        y = ymin;
                        pucPtr = pucStart;
                        while (y < ymax)
                        {
                            *((uint*)pucPtr) = 0xFF000000;

                            pucPtr += iStride;
                            y++;
                        }

                        pucStart += 4 * Gap;
                        x += Gap;
                    }

                    bmp.UnlockBits(bmpData);
                }
            }
            catch (Exception e)
            {
                bmp.UnlockBits(bmpData);

                if (IsDebug)
                    MessageBox.Show("Error :" + e.ToString());
            }
        }
        public void SetNet(Bitmap bmp, int Gap,Color TurnWhiteColor)
        {
            Rectangle rectbmp = JzTools.SimpleRect(bmp.Size);
            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr Scan0 = bmpData.Scan0;

            uint ColorValue = (uint)((TurnWhiteColor.A << 24) + (TurnWhiteColor.R << 16) + (TurnWhiteColor.G << 8) + TurnWhiteColor.B);

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

                            if (*((uint*)pucPtr) != ColorValue)
                            {
                                *((uint*)pucPtr) = 0xFF000000;
                            }
                            else
                            {
                                if ((x - xmin) % Gap == 0 || (y - ymin) % Gap == 0)
                                {
                                    *((uint*)pucPtr) = 0xFF000000;
                                }
                                else
                                {
                                    *((uint*)pucPtr) = 0xFFFFFFFF;
                                }
                            }

                            pucPtr += 4;
                            x++;
                        }

                        pucStart += iStride;
                        y ++;
                    }
                    bmp.UnlockBits(bmpData);
                }
            }
            catch (Exception e)
            {
                bmp.UnlockBits(bmpData);

                if (IsDebug)
                    MessageBox.Show("Error :" + e.ToString());
            }
        }

        public Rectangle ClearStinRect(Bitmap bmp, Rectangle rect, Color ReserveColor, double Ratio)
        {
            Rectangle rectbmp = JzTools.SimpleRect(bmp.Size);
            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr Scan0 = bmpData.Scan0;

            uint ColorValue = (uint)((ReserveColor.A << 24) + (ReserveColor.R << 16) + (ReserveColor.G << 8) + ReserveColor.B);

            try
            {
                unsafe
                {
                    byte* scan0 = (byte*)(void*)Scan0;
                    byte* pucPtr;
                    byte* pucStart;

                    int xmin = rect.X;
                    int ymin = rect.Y;
                    int xmax = xmin + rect.Width;
                    int ymax = ymin + rect.Height;

                    int x = xmin;
                    int y = ymin;

                    int Counter = 0;

                    int UpY = 0;
                    int DownY = 0;
                    int LeftX = 0;
                    int RightX = 0;

                    
                    //Find UpY
                    int iStride = bmpData.Stride;
                    y = ymin;

                    pucStart = scan0 + (x << 2) + (iStride * y);

                    while (y < ymax)
                    {
                        x = xmin;
                        pucPtr = pucStart;
                        Counter = 0;
                        while (x < xmax)
                        {
                            if (*((uint*)pucPtr) == ColorValue)
                            {
                                *((uint*)pucPtr) = 0xFFFFFFFF;
                                Counter++;
                            }
                            else
                            {
                                *((uint*)pucPtr) = 0xFF00FF00;
                            }

                            pucPtr += 4;
                            x++;
                        }

                        if (((double)Counter / (double)rect.Width) > Ratio)
                        {
                            UpY = y - ymin;
                            break;
                        }

                        pucStart += iStride;
                        y++;
                    }

                    //Find DownY
                    iStride = bmpData.Stride;

                    x = xmin;
                    y = ymax - 1;
                    pucStart = scan0 + (x << 2) + (iStride * y);

                    while (y >= ymin)
                    {
                        x = xmin;
                        pucPtr = pucStart;
                        Counter = 0;
                        while (x < xmax)
                        {
                            if (*((uint*)pucPtr) == ColorValue)
                            {
                                *((uint*)pucPtr) = 0xFFFFFFFF;
                                Counter++;
                            }
                            else
                            {
                                *((uint*)pucPtr) = 0xFF00FF00;
                            }

                            pucPtr += 4;
                            x++;
                        }

                        if (((double)Counter / (double)rect.Width) > Ratio)
                        {
                            DownY = ymax - 1 - y;
                            break;
                        }

                        pucStart -= iStride;
                        y--;
                    }

                    //Find LeftX
                    iStride = bmpData.Stride;
                    x = xmin;
                    y= ymin;
                    pucStart = scan0 + (x << 2) + (iStride * y);

                    while (x < xmax)
                    {
                        y = ymin;
                        pucStart = scan0 + (x << 2) + (iStride * y);
                        pucPtr = pucStart;
                        
                        Counter = 0;
                        while (y < ymax)
                        {
                            if (*((uint*)pucPtr) == ColorValue)
                            {
                                *((uint*)pucPtr) = 0xFFFFFFFF;
                                Counter++;
                            }
                            else
                            {
                                *((uint*)pucPtr) = 0xFF00FF00;
                            }

                            pucPtr += iStride;
                            y++;
                        }

                        if (((double)Counter / (double)rect.Width) > Ratio)
                        {
                            LeftX = x - xmin;
                            break;
                        }

                        x++;
                    }

                    //Find Right X
                    iStride = bmpData.Stride;
                    x = xmax -1;
                    y = ymin;
                    pucStart = scan0 + (x << 2) + (iStride * y);

                    while (x >= xmin)
                    {
                        y = ymin;

                        pucPtr = pucStart;

                        Counter = 0;
                        while (y < ymax)
                        {
                            if (*((uint*)pucPtr) == ColorValue)
                            {
                                *((uint*)pucPtr) = 0xFFFFFFFF;
                                Counter++;
                            }
                            else
                            {
                                *((uint*)pucPtr) = 0xFF00FF00;
                            }

                            pucPtr += iStride;
                            y++;
                        }

                        if (((double)Counter / (double)rect.Width) > Ratio)
                        {
                            RightX = xmax - x;
                            break;
                        }

                        x--;
                    }

                    bmp.UnlockBits(bmpData);

                    return new Rectangle(rect.X + LeftX, rect.Y + UpY, rect.Width - RightX - 1, rect.Height - DownY - 1);
                }
            }
            catch (Exception e)
            {
                bmp.UnlockBits(bmpData);

                if (IsDebug)
                    MessageBox.Show("Error :" + e.ToString());

                return new Rectangle();
            }
        }


        public void GetLine(Bitmap bmp, Rectangle Rect, ref Point [] Pts,int Ratio)
        {
            Rectangle rectbmp = Rect;
            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr Scan0 = bmpData.Scan0;

            Point Pt = JzTools.GetRectCenter(Rect);

            int EnlargeHeight = Rect.Height >> (-Ratio);
            int EnlargeWidth = Rect.Width >> (-Ratio);

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

                    #region Get P4,P6
                    y = Pt.Y - EnlargeHeight;

                    Pts[4].Y = y;
                    Pts[6].Y = y;

                    x = xmin;
                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                    pucPtr = pucStart;

                    Pts[4].X = x;
                    while (x < xmax)
                    {
                        if (*((uint*)pucPtr) == 0xFFFF0000)
                        {
                            Pts[4].X = x;
                            break;
                        }
                        pucPtr += 4;
                        x++;
                    }

                    x = xmax - 1;
                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                    pucPtr = pucStart;

                    Pts[6].X = x;
                    while (x >= 0)
                    {
                        if (*((uint*)pucPtr) == 0xFFFF0000)
                        {
                            Pts[6].X = x;
                            break;
                        }
                        pucPtr -= 4;
                        x--;
                    }
                    #endregion
                    #region Get P5,P7
                    y = Pt.Y + EnlargeHeight;

                    Pts[5].Y = y;
                    Pts[7].Y = y;

                    x = xmin;
                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                    pucPtr = pucStart;

                    Pts[5].X = x;
                    while (x < xmax)
                    {
                        if (*((uint*)pucPtr) == 0xFFFF0000)
                        {
                            Pts[5].X = x;
                            break;
                        }
                        pucPtr += 4;
                        x++;
                    }

                    x = xmax - 1;
                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                    pucPtr = pucStart;

                    Pts[7].X = x;
                    while (x >= 0)
                    {
                        if (*((uint*)pucPtr) == 0xFFFF0000)
                        {
                            Pts[7].X = x;
                            break;
                        }
                        pucPtr -= 4;
                        x--;
                    }
                    #endregion
                    #region Get P0,P2
                    x = Pt.X - EnlargeWidth;

                    Pts[0].X = x;
                    Pts[2].X = x;


                    y = ymin;
                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                    pucPtr = pucStart;
                    while (y < ymax)
                    {
                        if (*((uint*)pucPtr) == 0xFFFF0000)
                        {
                            Pts[0].Y = y;
                            break;
                        }

                        pucPtr += iStride;
                        y++;
                    }

                    y = ymax - 1;
                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                    pucPtr = pucStart;
                    while (y >= 0)
                    {
                        if (*((uint*)pucPtr) == 0xFFFF0000)
                        {
                            Pts[2].Y = y;
                            break;
                        }
                        pucPtr -= iStride;
                        y--;
                    }
                    #endregion
                    #region Get P1,P3
                    x = Pt.X + EnlargeWidth;

                    Pts[1].X = x;
                    Pts[3].X = x;

                    y = ymin;
                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                    pucPtr = pucStart;
                    while (y < ymax)
                    {
                        if (*((uint*)pucPtr) == 0xFFFF0000)
                        {
                            Pts[1].Y = y;
                            break;
                        }

                        pucPtr += iStride;
                        y++;
                    }

                    y = ymax - 1;
                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                    pucPtr = pucStart;
                    while (y >= 0)
                    {
                        if (*((uint*)pucPtr) == 0xFFFF0000)
                        {
                            Pts[3].Y = y;
                            break;
                        }
                        pucPtr -= iStride;
                        y--;
                    }
                    #endregion

                    bmp.UnlockBits(bmpData);
                }
            }
            catch (Exception e)
            {
                bmp.UnlockBits(bmpData);

                if (IsDebug)
                    MessageBox.Show("Error :" + e.ToString());
            }

        }
        public void GetLineDX(Bitmap bmp, Rectangle Rect, ref Point[] Pts)
        {
            const int SampleCount = 5; //Must Be Odd;
            const int SampleGap = 2;

            Rectangle rectbmp = Rect;
            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr Scan0 = bmpData.Scan0;

            Point Pt = JzTools.GetRectCenter(Rect);

            //int EnlargeHeight = Rect.Height >> (-Ratio);
            //int EnlargeWidth = Rect.Width >> (-Ratio);
            Point[] Ptgrp = new Point[5];
            int i = 0;
            int j = 0;
            int k = 0;

            int EnlargeHeight = (int)((double)Rect.Height / 4d);
            int EnlargeWidth = (int)((double)Rect.Width / 4d);

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

                    #region Get P4,P6
                    y = Pt.Y - EnlargeHeight;

                    Pts[4].Y = y;
                    Pts[6].Y = y;

                    i = 0;
                    k = 0;
                    while (i < SampleCount)
                    {
                        j = 0;
                        while (j < 2)
                        {
                            x = xmin;
                            pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y + i * (j == 0 ? 1 : -1) * SampleGap - ymin));
                            pucPtr = pucStart;

                            //Pts[4].X = x;
                            while (x < xmax)
                            {
                                if (*((uint*)pucPtr) == 0xFFFF0000)
                                {
                                    k += x;
                                    break;
                                }
                                pucPtr += 4;
                                x++;
                            }
                            if (i == 0)
                                break;
                            j++;
                        }
                        i++;
                    }

                    Pts[4].X = (int)((double)k / (double)(SampleCount * 2 - 1));

                    i = 0;
                    k = 0;
                    while (i < SampleCount)
                    {
                        j = 0;
                        while (j < 2)
                        {
                            x = xmax - 1;
                            pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y + i * (j == 0 ? 1 : -1) * SampleGap - ymin));
                            pucPtr = pucStart;

                            //Pts[6].X = x;
                            while (x >= 0)
                            {
                                if (*((uint*)pucPtr) == 0xFFFF0000)
                                {
                                    k += x;
                                    break;
                                }
                                pucPtr -= 4;
                                x--;
                            }
                            if (i == 0)
                                break;
                            j++;
                        }
                        i++;
                    }

                    Pts[6].X = (int)((double)k / (double)(SampleCount * 2 - 1));
                    #endregion
                    #region Get P5,P7
                    y = Pt.Y + EnlargeHeight;

                    Pts[5].Y = y;
                    Pts[7].Y = y;

                    i = 0;
                    k = 0;
                    while (i < SampleCount)
                    {
                        j = 0;
                        while (j < 2)
                        {
                            x = xmin;
                            pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y + i * (j == 0 ? 1 : -1) * SampleGap - ymin));
                            pucPtr = pucStart;

                            //Pts[5].X = x;
                            while (x < xmax)
                            {
                                if (*((uint*)pucPtr) == 0xFFFF0000)
                                {
                                    k += x;
                                    break;
                                }
                                pucPtr += 4;
                                x++;
                            }
                            if (i == 0)
                                break;
                            j++;
                        }
                        i++;
                    }

                    Pts[6].X = (int)((double)k / (double)(SampleCount * 2 - 1));

                    i = 0;
                    k = 0;
                    while (i < SampleCount)
                    {
                        j = 0;
                        while (j < 2)
                        {
                            x = xmax - 1;
                            pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y + i * (j == 0 ? 1 : -1) * SampleGap - ymin));
                            pucPtr = pucStart;

                            //Pts[7].X = x;
                            while (x >= 0)
                            {
                                if (*((uint*)pucPtr) == 0xFFFF0000)
                                {
                                    k += x;
                                    break;
                                }
                                pucPtr -= 4;
                                x--;
                            }
                            if (i == 0)
                                break;
                            j++;
                        }
                        i++;
                    }
                    Pts[7].X = (int)((double)k / (double)(SampleCount * 2 - 1));

                    #endregion
                    #region Get P0,P2
                    x = Pt.X - EnlargeWidth;

                    Pts[0].X = x;
                    Pts[2].X = x;

                    
                    i = 0;
                    k = 0;
                    while (i < SampleCount)
                    {
                        j = 0;
                        while (j < 2)
                        {
                            y = ymin;
                            pucStart = scan0 + ((x + i * (j == 0 ? 1 : -1) * SampleGap - xmin) << 2) + (iStride * (y - ymin));
                            pucPtr = pucStart;
                            while (y < ymax)
                            {
                                if (*((uint*)pucPtr) == 0xFFFF0000)
                                {
                                    k += y;
                                    break;
                                }

                                pucPtr += iStride;
                                y++;
                            }
                            if (i == 0)
                                break;
                            j++;
                        }
                        i++;
                    }

                    Pts[0].Y = (int)((double)k / (double)(SampleCount * 2 - 1));

                    i = 0;
                    k = 0;
                    while (i < SampleCount)
                    {
                        j = 0;
                        while (j < 2)
                        {
                            y = ymax - 1;
                            pucStart = scan0 + ((x + i * (j == 0 ? 1 : -1) * SampleGap - xmin) << 2) + (iStride * (y - ymin));
                            pucPtr = pucStart;
                            while (y >= 0)
                            {
                                if (*((uint*)pucPtr) == 0xFFFF0000)
                                {
                                    k += y;
                                    break;
                                }
                                pucPtr -= iStride;
                                y--;
                            }
                            if (i == 0)
                                break;
                            j++;
                        }
                        i++;
                    }

                    Pts[2].Y = (int)((double)k / (double)(SampleCount * 2 - 1));

                    #endregion
                    #region Get P1,P3
                    x = Pt.X + EnlargeWidth;

                    Pts[1].X = x;
                    Pts[3].X = x;

                    i = 0;
                    k = 0;
                    while (i < SampleCount)
                    {
                        j = 0;
                        while (j < 2)
                        {
                            y = ymin;
                            pucStart = scan0 + ((x + i * (j == 0 ? 1 : -1) * SampleGap - xmin) << 2) + (iStride * (y - ymin));
                            pucPtr = pucStart;
                            while (y < ymax)
                            {
                                if (*((uint*)pucPtr) == 0xFFFF0000)
                                {
                                    k += y;
                                    break;
                                }

                                pucPtr += iStride;
                                y++;
                            }
                            if (i == 0)
                                break;
                            j++;
                        }
                        i++;
                    }

                    Pts[1].Y = (int)((double)k / (double)(SampleCount * 2 - 1));

                    i = 0;
                    k = 0;
                    while (i < SampleCount)
                    {
                        j = 0;
                        while (j < 2)
                        {
                            y = ymax - 1;
                            pucStart = scan0 + ((x + i * (j == 0 ? 1 : -1) * SampleGap - xmin) << 2) + (iStride * (y - ymin));
                            pucPtr = pucStart;
                            while (y >= 0)
                            {
                                if (*((uint*)pucPtr) == 0xFFFF0000)
                                {
                                    k += y;
                                    break;
                                }
                                pucPtr -= iStride;
                                y--;
                            }
                            if (i == 0)
                                break;
                            j++;
                        }
                        i++;
                    }

                    Pts[3].Y = (int)((double)k / (double)(SampleCount * 2 - 1));
                    #endregion

                    bmp.UnlockBits(bmpData);
                }
            }
            catch (Exception e)
            {
                bmp.UnlockBits(bmpData);

                if (IsDebug)
                    MessageBox.Show("Error :" + e.ToString());
            }

        }
        public void GetLineDX(Bitmap bmp, Rectangle Rect, ref Point[] Pts, LineClass TopLeftLine,LineClass BottomRightLine,bool IsHorizontal)
        {
            Rectangle rectbmp = Rect;
            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr Scan0 = bmpData.Scan0;

            try
            {
                unsafe
                {
                    byte* scan0 = (byte*)(void*)Scan0;
                    byte* pucPtr;

                    int xmin = rectbmp.X;
                    int ymin = rectbmp.Y;
                    int xmax = xmin + rectbmp.Width;
                    int ymax = ymin + rectbmp.Height;

                    int x = xmin;
                    int y = ymin;
                    int iStride = bmpData.Stride;

                    if (!IsHorizontal)
                    {
                        #region Get P4,P5

                        x = xmin;
                        Pts[4].X = x;
                        while (x < xmax)
                        {
                            y =(int)TopLeftLine.GetPtFromX(x).Y;
                            if (y >= ymin && y < ymax)
                            {
                                pucPtr = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                                if (*((uint*)pucPtr) == 0xFFFF0000)
                                {
                                    Pts[4].Y = y;
                                    Pts[4].X = x;
                                    break;
                                }
                            }
                            x++;
                        }

                        x = xmin;
                        Pts[5].X = x;
                        while (x < xmax)
                        {
                            y = (int)BottomRightLine.GetPtFromX(x).Y;
                            if (y >= ymin && y < ymax)
                            {
                                pucPtr = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                                if (*((uint*)pucPtr) == 0xFFFF0000)
                                {
                                    Pts[5].Y = y;
                                    Pts[5].X = x;
                                    break;
                                }
                            }
                            x++;
                        }

                        #endregion
                        #region Get P6,P7

                        x = xmax - 1;
                        Pts[6].X = x;
                        while (x >= xmin)
                        {
                            y = (int)TopLeftLine.GetPtFromX(x).Y;
                            if (y >= ymin && y < ymax)
                            {
                                pucPtr = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                                if (*((uint*)pucPtr) == 0xFFFF0000)
                                {
                                    Pts[6].Y = y;
                                    Pts[6].X = x;
                                    break;
                                }
                            }
                            x--;
                        }

                        x = xmax - 1;
                        Pts[7].X = x;
                        while (x >= xmin)
                        {
                            y = (int)BottomRightLine.GetPtFromX(x).Y;
                            if (y >= ymin && y < ymax)
                            {
                                pucPtr = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                                if (*((uint*)pucPtr) == 0xFFFF0000)
                                {
                                    Pts[7].Y = y;
                                    Pts[7].X = x;
                                    break;
                                }
                            }
                            x--;
                        }
                        #endregion
                    }
                    else
                    {
                        #region Get P0,P1

                        y = ymin;
                        Pts[0].Y = y;
                        while (y < xmax)
                        {
                            x = (int)TopLeftLine.GetPtFromY(y).X;
                            if (x >= xmin && x < xmax)
                            {
                                pucPtr = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                                if (*((uint*)pucPtr) == 0xFFFF0000)
                                {
                                    Pts[0].Y = y;
                                    Pts[0].X = x;
                                    break;
                                }
                            }
                            y++;
                        }

                        y = ymin;
                        Pts[1].Y = y;
                        while (y < ymax)
                        {
                            x = (int)BottomRightLine.GetPtFromY(y).X;
                            if (x >= xmin && x < xmax)
                            {
                                pucPtr = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                                if (*((uint*)pucPtr) == 0xFFFF0000)
                                {
                                    Pts[1].Y = y;
                                    Pts[1].X = x;
                                    break;
                                }
                            }
                            y++;
                        }

                        #endregion
                        #region Get P2,P3

                        y = ymax - 1;
                        Pts[2].Y = y;
                        while (y >= ymin)
                        {
                            x =(int)TopLeftLine.GetPtFromY(y).X;
                            if (x >= xmin && x < xmax)
                            {
                                pucPtr = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                                if (*((uint*)pucPtr) == 0xFFFF0000)
                                {
                                    Pts[2].Y = y;
                                    Pts[2].X = x;
                                    break;
                                }
                            }
                            y--;
                        }

                        y = ymax - 1;
                        Pts[3].Y = y;
                        while (y >= ymin)
                        {
                            x = (int)BottomRightLine.GetPtFromY(y).X;
                            if (x >= xmin && x < xmax)
                            {
                                pucPtr = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                                if (*((uint*)pucPtr) == 0xFFFF0000)
                                {
                                    Pts[3].Y = y;
                                    Pts[3].X = x;
                                    break;
                                }
                            }
                            y--;
                        }
                        #endregion

                    }

                    bmp.UnlockBits(bmpData);
                }
            }
            catch (Exception e)
            {
                bmp.UnlockBits(bmpData);

                if (IsDebug)
                    MessageBox.Show("Error :" + e.ToString());
            }

        }

        public int GetLineCenter(Bitmap bmp, Point PtTop, Point PtBottom,ref int iHeight)
        {

            Rectangle rectbmp = JzTools.SimpleRect(bmp.Size);
            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr Scan0 = bmpData.Scan0;

            try
            {
                unsafe
                {
                    byte* scan0 = (byte*)(void*)Scan0;
                    byte* pucPtr;
                    byte* pucStart;

                    int i = 0;
                    //int xmin = 0;
                    int ymin = PtTop.Y;
                    //int xmax = 0;
                    int ymax = PtBottom.Y + 1;

                    int x = PtTop.X;
                    int y = ymin;
                    int iStride = bmpData.Stride;

                    uint InRangeColor = 0xFFFFFFFF;
                    uint OutrangeColor = 0xFF000000;
                    uint AssignColor = 0xFFFF0000;

                    List<int> YList = new List<int>();
                    //uint OutrangeColor = 0xFFFFFFFF;

                    //if (IsInrangeColorWhilte)
                    //{
                    //    InRangeColor = 0xFFFFFFFF;
                    //    OutrangeColor = 0xFF000000;
                    //}

                    y = ymin;
                    pucStart = scan0 + ((x) << 2) + (iStride * (y));

                    uint RangeColor = OutrangeColor;
                    while (y < ymax)
                    {
                        //pucStart = scan0 + (iStride * (y - ymin));
                        //x = xmin;
                        pucPtr = pucStart;
                        //while (x < xmax)
                        //{
                        //Grade = JzTools.GrayscaleInt(pucPtr[2], pucPtr[1], pucPtr[0]);
                        //*((uint*)pucPtr) = (JzTools.IsInRangeEx(Grade, ThresholdValueMax, ThresholdValueMin) ? InRangeColor : OutrangeColor);

                        if (*((uint*)pucPtr) != RangeColor)
                        {
                            if (RangeColor == InRangeColor)
                            {
                                RangeColor = OutrangeColor;
                            }
                            else
                            {
                                RangeColor = InRangeColor;
                            }

                            YList.Add(y * 10 + (RangeColor == InRangeColor ? 1 : 0));
                        }
                        //pucPtr += 4;
                        //x++;
                        //}

                        pucStart += iStride;
                        y++;
                    }

                    i = 0;

                    //找尋有 1 變 0 的區段
                    iHeight = 0;

                    while (i < YList.Count)
                    {
                        if (YList[i] % 10 == 1 && YList[Math.Min(i + 1, YList.Count - 1)] % 10 == 0)
                        {
                            //y = ((((YList[i] / 10) - (YList[i + 1] / 10))) >> 1) + (YList[i + 1] / 10);
                            pucPtr = scan0 + ((x) << 2) + (iStride * (y));

                            *((uint*)pucPtr) = AssignColor;
                            y = (YList[i] / 10);
                            
                            iHeight = (YList[i + 1] / 10) - (YList[i] / 10);

                            break;
                        }
                        i++;
                    }

                    bmp.UnlockBits(bmpData);

                    return y;
                }
            }
            catch (Exception e)
            {
                bmp.UnlockBits(bmpData);

                if (IsDebug)
                    MessageBox.Show("Error :" + e.ToString());

                return -1;
            }
        }

        public int GetLineCenter(Bitmap bmp, Point PtTop, Point PtBottom,bool IsUpDown)
        {

            Rectangle rectbmp = JzTools.SimpleRect(bmp.Size);
            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr Scan0 = bmpData.Scan0;

            try
            {
                unsafe
                {
                    byte* scan0 = (byte*)(void*)Scan0;
                    byte* pucPtr;
                    byte* pucStart;

                    int i = 0;
                    //int xmin = 0;
                    int ymin = PtTop.Y;
                    //int xmax = 0;
                    int ymax = PtBottom.Y + 1;

                    int x = PtTop.X;
                    int y = ymin;
                    int iStride = bmpData.Stride;

                    uint InRangeColor = 0xFFFFFFFF;
                    uint OutrangeColor = 0xFF000000;
                    uint AssignColor = 0xFFFF0000;

                    List<int> YList = new List<int>();
                    //uint OutrangeColor = 0xFFFFFFFF;

                    //if (IsInrangeColorWhilte)
                    //{
                    //    InRangeColor = 0xFFFFFFFF;
                    //    OutrangeColor = 0xFF000000;
                    //}

                    uint RangeColor = OutrangeColor;

                    ymin = PtTop.Y;
                    ymax = PtBottom.Y + 1;
                    if (IsUpDown)
                    {
                        y = ymin;
                        pucStart = scan0 + ((x) << 2) + (iStride * (y));

                        while (y < ymax)
                        {
                            pucPtr = pucStart;

                            if (*((uint*)pucPtr) != RangeColor)
                            {
                                if (RangeColor == InRangeColor)
                                {
                                    RangeColor = OutrangeColor;
                                }
                                else
                                {
                                    RangeColor = InRangeColor;
                                }

                                YList.Add(y * 10 + (RangeColor == InRangeColor ? 1 : 0));
                            }
                            pucStart += iStride;
                            y++;
                        }
                    }
                    else
                    {
                        y = ymax - 1;
                        pucStart = scan0 + ((x) << 2) + (iStride * (y));

                        while (y > ymin)
                        {
                            pucPtr = pucStart;

                            if (*((uint*)pucPtr) != RangeColor)
                            {
                                if (RangeColor == InRangeColor)
                                {
                                    RangeColor = OutrangeColor;
                                }
                                else
                                {
                                    RangeColor = InRangeColor;
                                }

                                YList.Add(y * 10 + (RangeColor == InRangeColor ? 1 : 0));
                            }
                            pucStart -= iStride;
                            y--;
                        }

                    }

                    i = 0;

                    //找尋有 1 變 0 的區段 或 0 變 1 的區段
                    while (i < YList.Count)
                    {
                        //if (!IsUpDown)
                        //{
                            if (YList[i] % 10 == 1 && YList[Math.Min(i + 1, YList.Count - 1)] % 10 == 0)
                            {
                                //y = ((((YList[i] / 10) - (YList[i + 1] / 10))) >> 1) + (YList[i + 1] / 10);
                                y = YList[i] / 10;

                                pucPtr = scan0 + ((x) << 2) + (iStride * (y));
                                *((uint*)pucPtr) = AssignColor;

                                break;
                            }
                            else if (YList.Count == 1 && YList[i] % 10 == 1)
                            {
                                y = (YList[i] / 10);

                                pucPtr = scan0 + ((x) << 2) + (iStride * (y));
                                *((uint*)pucPtr) = AssignColor;
                            }
                        //}
                        //else
                        //{
                        //    if (YList[i] % 10 == 0 && YList[Math.Min(i + 1, YList.Count - 1)] % 10 == 1)
                        //    {
                        //        y = YList[i + 1] / 10;
                        //        //y = ((((YList[i] / 10) - (YList[i + 1] / 10))) >> 1) + (YList[i + 1] / 10);
                        //        //pucPtr = scan0 + ((x) << 2) + (iStride * (y));

                        //        //*((uint*)pucPtr) = AssignColor;

                        //        break;
                        //    }
                        //    else if (YList.Count == 1 && YList[i] % 10 == 1)
                        //    {
                        //        y = YList[i] / 10;
                        //    }

                        //}
                        i++;
                    }

                    bmp.UnlockBits(bmpData);

                    return y;
                }
            }
            catch (Exception e)
            {
                bmp.UnlockBits(bmpData);

                if (IsDebug)
                    MessageBox.Show("Error :" + e.ToString());

                return -1;
            }
        }

        public void SetThresholdAndGetSize(Bitmap bmp, int ThresholdValue, int ThresholdRangeUpper, int ThresholdRangeLower,bool IsVertical,ref Size sz) //Size Width = Left or Top , Height = Right or Bottom
        {
            int Grade = 0;

            Rectangle rectbmp = JzTools.SimpleRect(bmp.Size);
            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr Scan0 = bmpData.Scan0;

            bool IsFrontOK = false;
            bool IsBackOK = false;
            bool IsFrontStart = false;
            bool IsBackStart = false;
            int Front = 0;
            int Back = 0;

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

                    int ThresholdValueMax = Math.Min(255, ThresholdValue + ThresholdRangeUpper);
                    int ThresholdValueMin = Math.Max(1, ThresholdValue - ThresholdRangeLower);

                    y = ymin;
                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * y);

                    while (y < ymax)
                    {
                        x = xmin;
                        pucPtr = pucStart;
                        while (x < xmax)
                        {
                            Grade = JzTools.GrayscaleInt(pucPtr[2], pucPtr[1], pucPtr[0]);
                            *((uint*)pucPtr) = (JzTools.IsInRangeEx(Grade, ThresholdValueMax, ThresholdValueMin) ? 0xFF000000 : 0xFFFFFFFF);

                            //}
                            pucPtr += 4;
                            x++;
                        }

                        pucStart += iStride;
                        y++;
                    }

                    sz.Width = 0;
                    sz.Height = 0;

                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * y);

                    #region Vertical Action

                    if (IsVertical)
                    {
                        y = ymin;
                        while (y < ymax)
                        {
                            if (!IsFrontOK)
                            {
                                x = xmin;
                                pucStart = scan0 + ((x - xmin) << 2) + (iStride * y);
                                pucPtr = pucStart;
                                Front = 0;

                                while (x < xmax)
                                {
                                    if (pucPtr[0] == 255)
                                    {
                                        *((uint*)pucPtr) = 0xFFFF0000;
                                        Front++;
                                    }
                                    pucPtr += 4;
                                    x++;
                                }

                                if (Front > (xmax >> Ratio) && !IsFrontStart)
                                {
                                    IsFrontStart = true;
                                }

                                if (Front > (xmax >> Ratio) || !IsFrontStart)
                                    sz.Width++;
                                else
                                    IsFrontOK = true;
                            }

                            if (!IsBackOK)
                            {
                                x = xmin;
                                pucStart = scan0 + ((x - xmin) << 2) + (iStride * (ymax - y - 1));
                                pucPtr = pucStart;
                                Back = 0;

                                while (x < xmax)
                                {
                                    if (pucPtr[0] == 255)
                                    {
                                        *((uint*)pucPtr) = 0xFFFF0000;
                                        Back++;
                                    }
                                    pucPtr += 4;
                                    x++;
                                }

                                if (Back > (xmax >> Ratio) && !IsBackStart)
                                {
                                    IsBackStart = true;
                                }
                                if (Back > (xmax >> Ratio) || !IsBackStart)
                                    sz.Height++;
                                else
                                    IsBackOK = true;

                            }
                            if (IsFrontOK && IsBackOK)
                            {
                                break;
                            }

                            y++;
                        }
                    }
                    #endregion
                    else
                    {
                        x = xmin;
                        while (x < xmax)
                        {
                            if (!IsFrontOK)
                            {
                                y = ymin;
                                pucStart = scan0 + (x << 2) + (iStride * y);
                                pucPtr = pucStart;
                                Front = 0;

                                while (y < ymax)
                                {
                                    if (pucPtr[0] == 255)
                                    {
                                        *((uint*)pucPtr) = 0xFFFF0000;
                                        Front++;
                                    }
                                    pucPtr += iStride;
                                    y++;
                                }

                                if (Front > (ymax >> Ratio) && !IsFrontStart)
                                {
                                    IsFrontStart = true;
                                }

                                if (Front > (ymax >> Ratio) || !IsFrontStart)
                                    sz.Width++;
                                else
                                    IsFrontOK = true;
                            }

                            if (!IsBackOK)
                            {
                                y = ymin;
                                pucStart = scan0 + ((xmax - x - 1) << 2) + (iStride * y);
                                pucPtr = pucStart;
                                Back = 0;

                                while (y < ymax)
                                {
                                    if (pucPtr[0] == 255)
                                    {
                                        *((uint*)pucPtr) = 0xFFFF0000;
                                        Back++;
                                    }
                                    pucPtr += iStride;
                                    y++;
                                }

                                if (Back > (ymax >> Ratio) && !IsBackStart)
                                {
                                    IsBackStart = true;
                                }
                                if (Back > (ymax >> Ratio) || !IsBackStart)
                                    sz.Height++;
                                else
                                    IsBackOK = true;

                            }
                            if (IsFrontOK && IsBackOK)
                            {
                                break;
                            }

                            x++;
                        }


                    }

                    bmp.UnlockBits(bmpData);
                }
            }
            catch (Exception e)
            {
                bmp.UnlockBits(bmpData);

                if (IsDebug)
                    MessageBox.Show("Error :" + e.ToString());
            }
        }
    }
}
