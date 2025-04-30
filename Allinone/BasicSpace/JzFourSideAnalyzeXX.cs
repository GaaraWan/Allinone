using AForge.Controls;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Windows.Forms;
using static AForge.Imaging.Filters.HitAndMiss;

namespace Common
{
    public enum SIDEEmnum : int
    {
        COUNT = 4,
        TOP = 0,
        BOTTOM = 1,
        LEFT = 2,
        RIGHT = 3,
    }
    public enum FirstStepEnum : int
    {
        FMethod1 = 0,   //預設
        FMethod2 = 1,
    }
    public enum SecondStepEnum : int
    {
        SMethod1 = 0,
        SMethod2 = 1,
    }
    public enum ThirdStepEnum : int
    {
        TMethod1 = 0,
        TMethod2 = 1,
    }

    public class JzHistogramClass
    {
        bool IsDebug
        {
            get
            {
                return true;
            }
        }

        int ColorGap = 2;
        int BarRange = 0;

        public int MaxGrade = -1000;
        public int MinGrade = 1000;

        public int TotalGrade = 0;
        public int MeanGrade = 0;
        public int TotalPixels = 1;

        public int TotalPixelForCount = 0;

        int ModeBarIndex = 0;
        public int ModeGrade = 0;
        public int ModeGradeAmount
        {
            get
            {
                return (SortingBars[SortingBars.Length - 1] + SortingBars[SortingBars.Length - 2] + SortingBars[SortingBars.Length - 3]) / 1000;
            }
        }

        public int[] SortingBars;
        public int[] OriginSortingBars;
        public int ModeGradeIndex
        {
            get
            {
                return SortingBars[SortingBars.Length - 1] % 1000;
            }
        }
        public int GetGradeValue(int index)
        {
            return SortingBars[SortingBars.Length - index - 1] % 1000;
        }
        public int GetGradeCount(int index)
        {
            return SortingBars[SortingBars.Length - index - 1] / 1000;
        }
        public JzHistogramClass(int rColorGap)
        {
            ColorGap = rColorGap;

            BarRange = (int)Math.Ceiling(255d / (double)ColorGap) + 1;
            SortingBars = new int[BarRange];
        }
        public void GetHistogram(Bitmap bmp)
        {
            GetHistogram(bmp, SimpleRect(bmp.Size));
        }
        public void GetHistogram(Bitmap bmp, bool IsWithoutZeroFilter)
        {
            GetHistogram(bmp, SimpleRect(bmp.Size), IsWithoutZeroFilter);
        }
        public void GetHistogram(Bitmap bmp, Rectangle rect)
        {
            GetHistogram(bmp, rect, false);
        }
        public void GetHistogram(Bitmap bmp, Rectangle rect, bool IsWithoutZeroFilter)
        {
            int Grade = 0;

            Reset();

            Rectangle rectbmp = rect;
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

                    TotalPixelForCount = 0;

                    y = ymin;
                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                    while (y < ymax)
                    {
                        x = xmin;
                        pucPtr = pucStart;
                        while (x < xmax)
                        {
                            if (pucPtr[0] != 0 || IsWithoutZeroFilter) //Use Zero Filter
                            {
                                Grade = GrayscaleInt(pucPtr[2], pucPtr[1], pucPtr[0]);
                                Add(Grade);
                            }

                            //*((uint*)pucPtr) = 0xFFFF0000;
                            TotalPixelForCount++;

                            pucPtr += 4;
                            x++;
                        }
                        pucStart += iStride;
                        y++;
                    }
                    bmp.UnlockBits(bmpData);
                }
            }
            catch (Exception ex)
            {
                //JetEazy.LoggerClass.Instance.WriteException(ex);
                bmp.UnlockBits(bmpData);

                if (IsDebug)
                    MessageBox.Show("Error :" + ex.ToString());
            }

            Complete();
        }
        public void GetHistogram(Bitmap bmp, Rectangle rect, int ExclusiveColor)
        {
            int Grade = 0;

            Reset();
            Rectangle rectbmp = rect;
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
                    TotalPixelForCount = 0;

                    while (y < ymax)
                    {
                        x = xmin;
                        pucPtr = pucStart;
                        while (x < xmax)
                        {
                            if (pucPtr[0] != ExclusiveColor) //Use Zero Filter
                            {
                                Grade = GrayscaleInt(pucPtr[2], pucPtr[1], pucPtr[0]);
                                Add(Grade);
                            }

                            //*((uint*)pucPtr) = 0xFFFF0000;
                            TotalPixelForCount++;
                            pucPtr += 4;
                            x++;
                        }
                        pucStart += iStride;
                        y++;
                    }
                    bmp.UnlockBits(bmpData);
                }
            }
            catch (Exception ex)
            {
                //JetEazy.LoggerClass.Instance.WriteException(ex);
                bmp.UnlockBits(bmpData);

                if (IsDebug)
                    MessageBox.Show("Error :" + ex.ToString());
            }

            Complete();
        }
        public void GetHistogram(Bitmap bmp, Bitmap bmpmask, bool ischeckwhite)
        {
            int Grade = 0;
            int maskGrade = 0;

            Rectangle rectbmp = SimpleRect(bmp.Size);

            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData bmpmaskData = bmpmask.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            IntPtr Scan0 = bmpData.Scan0;
            IntPtr maskScan0 = bmpmaskData.Scan0;

            Reset();
            try
            {
                unsafe
                {
                    byte* scan0 = (byte*)(void*)Scan0;
                    byte* pucPtr;
                    byte* pucStart;

                    byte* maskscan0 = (byte*)(void*)maskScan0;
                    byte* maskpucPtr;
                    byte* maskpucStart;

                    int xmin = rectbmp.X;
                    int ymin = rectbmp.Y;

                    int xmax = xmin + rectbmp.Width;
                    int ymax = ymin + rectbmp.Height;

                    int x = xmin;
                    int y = ymin;

                    int iStride = bmpData.Stride;
                    int imaskStride = bmpmaskData.Stride;

                    y = ymin;
                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                    maskpucStart = maskscan0 + ((x - xmin) << 2) + (imaskStride * (y - ymin));

                    int CheckColor = 0;

                    if (ischeckwhite)
                        CheckColor = 255;
                    else
                        CheckColor = 0;

                    TotalPixelForCount = 0;

                    while (y < ymax)
                    {
                        x = xmin;

                        pucPtr = pucStart;
                        maskpucPtr = maskpucStart;

                        while (x < xmax)
                        {
                            maskGrade = (int)maskpucPtr[2];
                            //if (*((uint*)maskpucPtr) == 0xFFFFFFFF)
                            if (maskGrade == CheckColor)
                            {
                                Grade = GrayscaleInt(pucPtr[2], pucPtr[1], pucPtr[0]);

                                //*((uint*)pucPtr) = 0xFF00FFFF;

                                Add(Grade);

                                TotalPixelForCount++;
                            }

                            pucPtr += 4;
                            maskpucPtr += 4;
                            x++;
                        }
                        pucStart += iStride;
                        maskpucStart += imaskStride;
                        y++;
                    }
                    bmp.UnlockBits(bmpData);
                    bmpmask.UnlockBits(bmpmaskData);
                }
            }
            catch (Exception ex)
            {
                //JetEazy.LoggerClass.Instance.WriteException(ex);
                bmp.UnlockBits(bmpData);
                bmpmask.UnlockBits(bmpmaskData);

                if (IsDebug)
                    MessageBox.Show("Error :" + ex.ToString());
            }
            Complete();
        }
        public void GetHistogram(Bitmap bmp, int UppercutGrade)
        {
            int Grade = 0;

            Reset();
            Rectangle rectbmp = SimpleRect(bmp.Size);
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
                            if (pucPtr[0] != 0) //Use Zero Filter
                            {
                                Grade = GrayscaleInt(pucPtr[2], pucPtr[1], pucPtr[0]);

                                if (Grade < UppercutGrade)
                                    Add(Grade);
                            }

                            //*((uint*)pucPtr) = 0xFFFF0000;

                            pucPtr += 4;
                            x++;
                        }
                        pucStart += iStride;
                        y++;
                    }
                    bmp.UnlockBits(bmpData);
                }
            }
            catch (Exception ex)
            {
                //JetEazy.LoggerClass.Instance.WriteException(ex);
                bmp.UnlockBits(bmpData);

                if (IsDebug)
                    MessageBox.Show("Error :" + ex.ToString());
            }

            Complete();
        }
        public void GetWBArray(Bitmap bmp, int mean, int[,] wbarray)
        {
            int Grade = 0;

            Rectangle rectbmp = SimpleRect(bmp.Size);
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
                            if (pucPtr[0] != 0) //Use Zero Filter
                            {
                                Grade = GrayscaleInt(pucPtr[2], pucPtr[1], pucPtr[0]);

                                wbarray[x, y] = mean - Grade;
                            }

                            pucPtr += 4;
                            x++;
                        }
                        pucStart += iStride;
                        y++;
                    }
                    bmp.UnlockBits(bmpData);
                }
            }
            catch (Exception ex)
            {
                //JetEazy.LoggerClass.Instance.WriteException(ex);
                bmp.UnlockBits(bmpData);

                if (IsDebug)
                    MessageBox.Show("Error :" + ex.ToString());
            }
        }
        public void SetWBArray(Bitmap bmp, int threshold, int[,] wbarray) //Threshold is No Use
        {
            int Grade = 0;

            Rectangle rectbmp = SimpleRect(bmp.Size);
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
                            if (pucPtr[0] != 0) //Use Zero Filter
                            {
                                Grade = GrayscaleInt(pucPtr[2], pucPtr[1], pucPtr[0]);

                                if (Grade > threshold)
                                {
                                    Grade += wbarray[x, y];

                                    Grade = Math.Min(255, Grade);
                                    Grade = Math.Max(0, Grade);

                                    pucPtr[2] = (byte)Grade;
                                    pucPtr[1] = (byte)Grade;
                                    pucPtr[0] = (byte)Grade;
                                }
                            }

                            pucPtr += 4;
                            x++;
                        }
                        pucStart += iStride;
                        y++;
                    }
                    bmp.UnlockBits(bmpData);
                }
            }
            catch (Exception ex)
            {
                //JetEazy.LoggerClass.Instance.WriteException(ex);
                bmp.UnlockBits(bmpData);

                if (IsDebug)
                    MessageBox.Show("Error :" + ex.ToString());
            }
        }
        /// <summary>
        /// 眔眖┕计琘ゑㄒず
        /// </summary>
        /// <param name="ratio"></param>
        /// <returns></returns>
        public int GetMaxRatio(float ratio)
        {
            int i = 0;

            int RatioIndex = (int)((float)TotalPixelForCount * ratio);
            int RatioCount = 0;

            i = OriginSortingBars.Length - 1;

            while (i > -1)
            {
                RatioCount += (OriginSortingBars[i] / 1000);

                if (RatioCount > RatioIndex)
                    break;
                i--;
            }
            return i * ColorGap;
        }
        /// <summary>
        /// 眔眖┕计琘ゑㄒず
        /// </summary>
        /// <param name="ratio"></param>
        /// <returns></returns>
        public int GetMinRatio(float ratio)
        {
            int i = 0;

            int RatioIndex = (int)((float)TotalPixelForCount * ratio);
            int RatioCount = 0;

            i = 0;

            while (i < OriginSortingBars.Length)
            {
                RatioCount += (OriginSortingBars[i] / 1000);

                if (RatioCount > RatioIndex)
                    break;
                i++;
            }
            return i * ColorGap;
        }
        /// <summary>
        /// 眔计眖蔼┕琘ゑㄒ计キА
        /// </summary>
        /// <param name="ratio"></param>
        /// <returns></returns>
        public int GetMaxRatioAVG(float ratio)
        {
            int i = 0;

            //int RatioIndex = (int)((float)TotalPixelForCount * ratio);
            int RatioCount = 0;

            int iTempCount = (int)(OriginSortingBars.Length * ratio);

            float AVG = 0f;

            i = OriginSortingBars.Length - 1;

            while (i > -1)
            {
                int singlecount = (OriginSortingBars[i] / 1000);

                RatioCount += singlecount;

                AVG += (float)(i * singlecount * ColorGap);

                //if (RatioCount > RatioIndex)
                //    break;
                if (RatioCount > OriginSortingBars.Length)
                    break;
                if (RatioCount > iTempCount)
                    break;
                i--;
            }

            return (int)(AVG / (float)RatioCount);
        }
        /// <summary>
        /// 眔计眖┕蔼琘ゑㄒ计キА
        /// </summary>
        /// <param name="ratio"></param>
        /// <returns></returns>
        public int GetMinRatioAVG(float ratio)
        {
            int i = 0;

            //int RatioIndex = (int)((float)TotalPixelForCount * ratio);
            int RatioCount = 0;
            int iTempCount = (int)(OriginSortingBars.Length * ratio);

            float AVG = 0f;

            i = 0;

            while (i < OriginSortingBars.Length)
            {
                int singlecount = (OriginSortingBars[i] / 1000);

                RatioCount += (OriginSortingBars[i] / 1000);

                AVG += (float)(i * singlecount * ColorGap);

                //if (RatioCount > RatioIndex)
                //    break;
                if (RatioCount > OriginSortingBars.Length)
                    break;
                if (RatioCount > iTempCount)
                    break;
                i++;
            }

            return (int)(AVG / (float)RatioCount);
        }
        void Reset()
        {
            MaxGrade = -1000;
            MinGrade = 1000;
            ModeGrade = 0;

            TotalGrade = 0;
            MeanGrade = 0;

            SortingBars = new int[BarRange];
            //BarsBeSort = new int[BarRange];

            TotalPixels = 1;
        }
        void Complete()
        {
            MeanGrade = TotalGrade / TotalPixels;

            int i = 0;
            int MaxValue = -1;

            ModeBarIndex = -1;

            while (i < BarRange)
            {
                if (MaxValue < SortingBars[i])
                {
                    MaxValue = SortingBars[i];
                    ModeBarIndex = i;
                }

                SortingBars[i] += i;
                i++;
            }
            ModeGrade = Math.Min(ModeBarIndex * ColorGap, 255);

            OriginSortingBars = new int[SortingBars.Length];

            Array.Copy(SortingBars, OriginSortingBars, SortingBars.Length);

            Array.Sort(SortingBars);

        }
        void Add(int Grade)
        {
            MaxGrade = Math.Max(Grade, MaxGrade);
            MinGrade = Math.Min(Grade, MinGrade);

            int i = (int)Math.Ceiling(Grade / (double)ColorGap);
            SortingBars[i] += 1000;

            TotalGrade += Grade;
            TotalPixels++;
        }

        Rectangle SimpleRect(Size Sz)
        {
            return new Rectangle(0, 0, Sz.Width, Sz.Height);
        }
        int GrayscaleInt(byte R, byte G, byte B)
        {
            return (int)((double)R * 0.3 + (double)G * 0.59 + (double)B * 0.11);
        }
        public void GetHistogramData(Bitmap bmp, Rectangle rect, int[] histogramdata)
        {
            int Grade = 0;

            Reset();

            Rectangle rectbmp = rect;
            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
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

                    int index = 0;

                    TotalPixelForCount = 0;

                    y = ymin;
                    pucStart = scan0 + (x - xmin) + (iStride * (y - ymin));

                    while (y < ymax)
                    {
                        x = xmin;
                        pucPtr = pucStart;
                        while (x < xmax)
                        {
                            histogramdata[*pucPtr]++;
                            //*((uint*)pucPtr) = 0xFFFF0000;
                            //TotalPixelForCount++;

                            index++;

                            pucPtr++;
                            x++;
                        }
                        pucStart += iStride;
                        y++;
                    }
                    bmp.UnlockBits(bmpData);
                }
            }
            catch (Exception ex)
            {
                //JetEazy.LoggerClass.Instance.WriteException(ex);
                bmp.UnlockBits(bmpData);

                if (IsDebug)
                    MessageBox.Show("Error :" + ex.ToString());
            }

            //Complete();
        }
        public void GetHistogramData(Bitmap bmp, Bitmap bmpmask, bool ischeckwhite, int[] histogramdata)
        {
            int Grade = 0;
            int maskGrade = 0;

            Rectangle rectbmp = SimpleRect(bmp.Size);

            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            BitmapData bmpmaskData = bmpmask.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

            IntPtr Scan0 = bmpData.Scan0;
            IntPtr maskScan0 = bmpmaskData.Scan0;

            Reset();
            try
            {
                unsafe
                {
                    byte* scan0 = (byte*)(void*)Scan0;
                    byte* pucPtr;
                    byte* pucStart;

                    byte* maskscan0 = (byte*)(void*)maskScan0;
                    byte* maskpucPtr;
                    byte* maskpucStart;

                    int xmin = rectbmp.X;
                    int ymin = rectbmp.Y;

                    int xmax = xmin + rectbmp.Width;
                    int ymax = ymin + rectbmp.Height;

                    int x = xmin;
                    int y = ymin;

                    int iStride = bmpData.Stride;
                    int imaskStride = bmpmaskData.Stride;

                    int index = 0;

                    y = ymin;
                    pucStart = scan0 + (x - xmin) + (iStride * (y - ymin));
                    maskpucStart = maskscan0 + (x - xmin) + (imaskStride * (y - ymin));

                    int CheckColor = 0;

                    if (ischeckwhite)
                        CheckColor = 255;
                    else
                        CheckColor = 0;

                    TotalPixelForCount = 0;

                    while (y < ymax)
                    {
                        x = xmin;

                        pucPtr = pucStart;
                        maskpucPtr = maskpucStart;

                        while (x < xmax)
                        {
                            maskGrade = *maskpucPtr;
                            //if (*((uint*)maskpucPtr) == 0xFFFFFFFF)
                            if (maskGrade == CheckColor)
                            {
                                histogramdata[*pucPtr]++;

                                index++;
                            }

                            pucPtr++;
                            maskpucPtr++;
                            x++;
                        }
                        pucStart += iStride;
                        maskpucStart += imaskStride;
                        y++;
                    }
                    bmp.UnlockBits(bmpData);
                    bmpmask.UnlockBits(bmpmaskData);
                }
            }
            catch (Exception ex)
            {
                //JetEazy.LoggerClass.Instance.WriteException(ex);
                bmp.UnlockBits(bmpData);
                bmpmask.UnlockBits(bmpmaskData);

                if (IsDebug)
                    MessageBox.Show("Error :" + ex.ToString());
            }
            //Complete();
        }

        public void FillColor(Bitmap bmp, Bitmap bmpmask, bool ischeckwhite, byte graycolor)
        {
            int Grade = 0;
            int maskGrade = 0;

            Rectangle rectbmp = SimpleRect(bmp.Size);

            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData bmpmaskData = bmpmask.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            IntPtr Scan0 = bmpData.Scan0;
            IntPtr maskScan0 = bmpmaskData.Scan0;

            Reset();
            try
            {
                unsafe
                {
                    byte* scan0 = (byte*)(void*)Scan0;
                    byte* pucPtr;
                    byte* pucStart;

                    byte* maskscan0 = (byte*)(void*)maskScan0;
                    byte* maskpucPtr;
                    byte* maskpucStart;

                    int xmin = rectbmp.X;
                    int ymin = rectbmp.Y;

                    int xmax = xmin + rectbmp.Width;
                    int ymax = ymin + rectbmp.Height;

                    int x = xmin;
                    int y = ymin;

                    int iStride = bmpData.Stride;
                    int imaskStride = bmpmaskData.Stride;

                    y = ymin;
                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                    maskpucStart = maskscan0 + ((x - xmin) << 2) + (imaskStride * (y - ymin));

                    int CheckColor = 0;

                    if (ischeckwhite)
                        CheckColor = 255;
                    else
                        CheckColor = 0;

                    TotalPixelForCount = 0;

                    while (y < ymax)
                    {
                        x = xmin;

                        pucPtr = pucStart;
                        maskpucPtr = maskpucStart;

                        while (x < xmax)
                        {
                            maskGrade = (int)maskpucPtr[2];
                            //if (*((uint*)maskpucPtr) == 0xFFFFFFFF)
                            if (maskGrade == CheckColor)
                            {
                                pucPtr[2] = graycolor;
                                pucPtr[1] = graycolor;
                                pucPtr[0] = graycolor;
                                //Grade = GrayscaleInt(pucPtr[2], pucPtr[1], pucPtr[0]);

                                //*((uint*)pucPtr) = 0xFF00FFFF;

                                //Add(Grade);

                                //TotalPixelForCount++;
                            }

                            pucPtr += 4;
                            maskpucPtr += 4;
                            x++;
                        }
                        pucStart += iStride;
                        maskpucStart += imaskStride;
                        y++;
                    }
                    bmp.UnlockBits(bmpData);
                    bmpmask.UnlockBits(bmpmaskData);
                }
            }
            catch (Exception ex)
            {
                //JetEazy.LoggerClass.Instance.WriteException(ex);
                bmp.UnlockBits(bmpData);
                bmpmask.UnlockBits(bmpmaskData);

                if (IsDebug)
                    MessageBox.Show("Error :" + ex.ToString());
            }
            //Complete();
        }

    }
    public class JzHistogramResult
    {
        public int count = 0;
        public int min = 0;
        public int max = 0;
        public long total = 0;

        public double mean = 0;
        public double mode = 0;
        public double median = 0;
        public double stddev = 0;

        public JzHistogramResult()
        {

        }
    }
    public class DataHistogramClass
    {
        bool IsDebug
        {
            get
            {
                return false;
            }
        }

        //JzToolsClass JzTools = new JzToolsClass();

        int Gap = 2;
        int BarRange = 0;

        public int MaxGrade = -1000;
        public int MinGrade = 1000;

        public int TotalGrade = 0;
        public int MeanGrade = 0;
        public int TotalCount = 1;

        public int TotalPixelForCount = 0;

        int ModeBarIndex = 0;
        public int ModeGrade = 0;
        public int ModeGradeAmount
        {
            get
            {
                return (SortingBars[SortingBars.Length - 1] + SortingBars[SortingBars.Length - 2] + SortingBars[SortingBars.Length - 3]) / 1000;
            }
        }

        public int[] SortingBars;
        public int[] OriginSortingBars;

        public int ModeGradeIndex
        {
            get
            {
                return SortingBars[SortingBars.Length - 1] % 1000;
            }
        }

        public int GetGradeIndex(int index)
        {
            return SortingBars[SortingBars.Length - index - 1] % 1000;
        }
        public int GetGrade(int index)
        {
            return SortingBars[SortingBars.Length - index - 1] / 1000;
        }
        public DataHistogramClass(int barrange, int gap)
        {
            Gap = gap;

            BarRange = (int)Math.Ceiling(barrange / (double)Gap) + 1;
            SortingBars = new int[BarRange];
        }

        public int GetMaxRatio(float ratio)
        {
            int i = 0;

            int RatioIndex = (int)((float)TotalPixelForCount * ratio);
            int RatioCount = 0;

            i = OriginSortingBars.Length - 1;

            while (i > -1)
            {
                RatioCount += (OriginSortingBars[i] / 1000);

                if (RatioCount > RatioIndex)
                    break;
                i--;
            }
            return i * Gap;
        }
        public int GetMinRatio(float ratio)
        {
            int i = 0;

            int RatioIndex = (int)((float)TotalPixelForCount * ratio);
            int RatioCount = 0;

            i = 0;

            while (i < OriginSortingBars.Length)
            {
                RatioCount += (OriginSortingBars[i] / 1000);

                if (RatioCount > RatioIndex)
                    break;
                i++;
            }
            return i * Gap;
        }

        public void Reset()
        {
            MaxGrade = -1000;
            MinGrade = 1000;
            ModeGrade = 0;

            TotalGrade = 0;
            MeanGrade = 0;

            SortingBars = new int[BarRange];
            //BarsBeSort = new int[BarRange];

            TotalCount = 1;
        }
        public void Complete()
        {
            MeanGrade = TotalGrade / TotalCount;

            int i = 0;
            int MaxValue = -1;

            ModeBarIndex = -1;

            while (i < BarRange)
            {
                if (MaxValue < SortingBars[i])
                {
                    MaxValue = SortingBars[i];
                    ModeBarIndex = i;
                }

                SortingBars[i] += i;
                i++;
            }
            ModeGrade = Math.Min(ModeBarIndex * Gap, BarRange);

            OriginSortingBars = new int[SortingBars.Length];

            Array.Copy(SortingBars, OriginSortingBars, SortingBars.Length);

            Array.Sort(SortingBars);

        }
        public void Add(int Grade)
        {
            MaxGrade = Math.Max(Grade, MaxGrade);
            MinGrade = Math.Min(Grade, MinGrade);

            int i = (int)Math.Ceiling(Grade / (double)Gap);

            if (i > SortingBars.Length - 1)
                i = SortingBars.Length - 1;

            SortingBars[i] += 1000;

            TotalGrade += Grade;
            TotalCount++;
        }

        public float GetBiggerModeRatio(int threshold)
        {
            int i = Math.Min(ModeBarIndex + 1, threshold / Gap + 1);

            int count = 0;

            while (i < BarRange)
            {
                count += (OriginSortingBars[i] / 1000);
                i++;
            }

            return (float)count / (float)TotalCount;

        }
        public float GetBiggerModeRatioAdv()
        {
            int threshold = Gap;

            int i = 0;
            int imode = Math.Min(ModeBarIndex + 1, threshold / Gap + 1);

            int count = 0;

            float ModeAdv = 0f;
            float ModeCount = 0f;
            float DefModeCount = 0;
            bool IsStartBigger = false;

            while (i < BarRange)
            {
                int singlecount = (OriginSortingBars[i] / 1000);

                if (i < imode)
                {
                    ModeAdv += (float)(singlecount * Gap * i);
                    ModeCount += singlecount;
                }
                else
                {
                    if (!IsStartBigger)
                    {
                        DefModeCount = ((ModeAdv / ModeCount) / Gap);

                        IsStartBigger = true;
                    }

                    count += (int)((float)singlecount * (float)(i / DefModeCount));
                }
                i++;
            }

            return Math.Min(Math.Abs((float)count / (float)TotalCount), 20);

        }
        public float GetSmallerModeRatio(int threshold)
        {
            int i = Math.Min(ModeBarIndex + 1, threshold / Gap + 1);

            int count = 0;

            while (i < BarRange)
            {
                count += (OriginSortingBars[i] / 1000);
                i++;
            }

            return (float)count / (float)TotalCount;

        }
        public float GetSmallerModeRatioAdv(int threshold)
        {
            int i = Math.Min(ModeBarIndex + 1, threshold / Gap + 1);

            int count = 0;

            while (i < BarRange)
            {
                count += (OriginSortingBars[i] / 1000);
                i++;
            }

            return (float)count / (float)TotalCount;

        }
    }


    public class SideFoundCollectionClass
    {
        const int foundcount = 50;
        public SideFoundClass[] sidefounds = new SideFoundClass[foundcount];

        public SideFoundCollectionClass()
        {
            int i = 0;

            while (i < foundcount)
            {
                sidefounds[i] = new SideFoundClass();
                i++;
            }
        }

        public void Suicide()
        {
            int i = 0;

            while (i < foundcount)
            {
                sidefounds[i].ClearItems();
                i++;
            }

        }
    }
    public class SideFoundClass
    {
        List<SideFoundItemClass> itemlist = new List<SideFoundItemClass>();

        public int maxval = -100000;
        public int minval = 100000;

        public SideFoundClass()
        {

        }
        public void Add(SideFoundItemClass item)
        {
            itemlist.Add(item);

            if (maxval < item.length)
                maxval = item.length;
            if (minval > item.length)
                minval = item.length;
        }


        public void ClearItems()
        {
            itemlist.Clear();

            maxval = -100000;
            minval = 100000;
        }
        public int CountItems()
        {
            return itemlist.Count;
        }
        public void FillPts(ref Point[] pts, Point offsetpt, bool isnew = false)
        {
            if (isnew)
                NGPtsFilter(1d);

            pts = new Point[itemlist.Count];

            int i = 0;
            while (i < pts.Length)
            {
                pts[i] = itemlist[i].ptEnd;
                pts[i].Offset(offsetpt);

                i++;
            }
        }
        public void FillPtsEX(ref Point[] pts, Point offsetpt, bool isnew = false)
        {
            if (isnew)
                NGPtsFilter(1d);

            pts = new Point[itemlist.Count];

            int i = 0;
            while (i < pts.Length)
            {
                pts[i] = itemlist[i].ptStart;
                pts[i].Offset(offsetpt);

                i++;
            }
        }
        public void FillDistance(List<int> dist, ref int max, ref int min, ref double mean)
        {
            max = -10000;
            min = 100000;
            double totoal = 0;
            double count = 0;

            foreach (SideFoundItemClass item in itemlist)
            {
                dist.Add(item.length);

                if (max < item.length)
                {
                    max = item.length;
                }
                if (min > item.length)
                {
                    min = item.length;
                }
                totoal += item.length;
                count++;
            }

            mean = totoal / count;
        }

        public void NGPtsFilter(double rangeratio)
        {
            int range = maxval - minval + 1;

            if (range < 10)
                return;

            int[] histodata = new int[range];

            int i = 0;

            foreach (SideFoundItemClass sidefound in itemlist)
            {
                histodata[(sidefound.length - minval)]++;
            }

            JzHistogramResult jzhistoresult = new JzHistogramResult();

            GetHistogramData(histodata, jzhistoresult);

            int upperval = (int)((jzhistoresult.mean + (jzhistoresult.stddev * rangeratio)) + minval);
            int lowerval = (int)((jzhistoresult.mean - (jzhistoresult.stddev * rangeratio)) + minval);

            i = itemlist.Count - 1;

            while (i > -1)
            {
                if (itemlist[i].length > upperval || itemlist[i].length < lowerval)
                {
                    itemlist.RemoveAt(i);
                }
                i--;
            }
        }
        public void GetHistogramData(int[] histogramdata, JzHistogramResult jzhresult)
        {
            int num = histogramdata.Length;
            int max = 0;
            int min = num;
            long total = 0L;
            for (int i = 0; i < num; i++)
            {
                if (histogramdata[i] != 0)
                {
                    if (i > max)
                    {
                        max = i;
                    }

                    if (i < min)
                    {
                        min = i;
                    }

                    total += histogramdata[i];
                }
            }

            jzhresult.min = min;
            jzhresult.max = max;
            jzhresult.count = num;
            jzhresult.total = total;

            jzhresult.mean = Statistics.Mean(histogramdata);
            jzhresult.stddev = Statistics.StdDev(histogramdata, jzhresult.mean);
            jzhresult.median = Statistics.Median(histogramdata);
            jzhresult.mode = Statistics.Mode(histogramdata);

        }
    }

    public class SideFoundItemClass
    {
        public Point ptStart = new Point(0, 0);
        public Point ptEnd = new Point(0, 0);
        public int length = 0;

        public SideFoundItemClass(Point ptst, Point pted, int len)
        {
            ptStart = ptst;
            ptEnd = pted;
            length = len;
        }

        public SideFoundItemClass(Point pt, int len)
        {
            this.ptStart = pt;
            this.length = len;
        }
        public Point GetEndOffset(Point pt)
        {
            Point retpt = pt;

            retpt.X += ptEnd.X;
            retpt.Y += ptEnd.Y;

            return retpt;
        }
    }

    public class SideDataClass
    {
        const double rangeratio = 0.8d;

        Bitmap bmp1st;
        Bitmap bmp2nd;

        public Bitmap bmpside;
        public SIDEEmnum SIDEIndex;

        double lratio;
        double sratio;

        double maxgap;
        double mingap;
        double meangap;

        SideFoundCollectionClass SideFoundCollection = new SideFoundCollectionClass();
        SideFoundClass SIDEFound = new SideFoundClass();
        public Rectangle myrect = new Rectangle();
        public int mysideCount = 0;


        public void GetSide(SIDEEmnum side, Bitmap bmpfirst, Rectangle centerrect, double rangeratio)
        {
            SIDEIndex = side;
            //bmp1st = (Bitmap)bmpfirst.Clone(new Rectangle(0,0,bmpfirst.Width,bmpfirst.Height),bmpfirst.PixelFormat);
            bmp1st = CopyImage(bmpfirst);

            int infwidth = (int)((centerrect.Width - ((double)centerrect.Width * rangeratio)) / 2);
            int infheight = (int)((centerrect.Height - ((double)centerrect.Height * rangeratio)) / 2);

            Rectangle rect = centerrect;
            rect.Inflate(-infwidth, -infheight);

            switch (side)
            {
                case SIDEEmnum.TOP:
                    myrect = new Rectangle(rect.X, 0, rect.Width, rect.Y);
                    break;
                case SIDEEmnum.BOTTOM:
                    myrect = new Rectangle(rect.X, rect.Bottom, rect.Width, bmp1st.Height - rect.Bottom);
                    break;
                case SIDEEmnum.LEFT:
                    myrect = new Rectangle(0, rect.Y, rect.X, rect.Height);
                    break;
                case SIDEEmnum.RIGHT:
                    myrect = new Rectangle(rect.Right, rect.Y, bmp1st.Width - rect.Right, rect.Height);
                    break;
            }

            bmpside = CopyImage(bmp1st, myrect);

            SIDEFound.ClearItems();

            bmpside.Save(@"D:\JETEAZY\SIDE" + SIDEIndex.ToString() + ".BMP", ImageFormat.Bmp);
        }

        public void GetSide(SIDEEmnum side, Bitmap bmpfirst, Rectangle centerrect, double longratio, double shortratio)
        {
            SIDEIndex = side;
            //bmp1st = (Bitmap)bmpfirst.Clone(new Rectangle(0,0,bmpfirst.Width,bmpfirst.Height),bmpfirst.PixelFormat);
            bmp1st = CopyImage(bmpfirst);

            int infwidth = (int)((centerrect.Width - ((double)centerrect.Width * longratio)) / 2);
            int infheight = (int)((centerrect.Height - ((double)centerrect.Height * shortratio)) / 2);

            Rectangle rect = centerrect;

            switch (side)
            {
                case SIDEEmnum.TOP:

                    infwidth = (int)((centerrect.Width - ((double)centerrect.Width * longratio)) / 2);
                    infheight = (int)((centerrect.Height - ((double)centerrect.Height * shortratio)) / 2);
                    rect.Inflate(-infwidth, -infheight);

                    myrect = new Rectangle(rect.X, 0, rect.Width, rect.Y);
                    break;
                case SIDEEmnum.BOTTOM:

                    infwidth = (int)((centerrect.Width - ((double)centerrect.Width * longratio)) / 2);
                    infheight = (int)((centerrect.Height - ((double)centerrect.Height * shortratio)) / 2);
                    rect.Inflate(-infwidth, -infheight);

                    myrect = new Rectangle(rect.X, rect.Bottom, rect.Width, bmp1st.Height - rect.Bottom);
                    break;
                case SIDEEmnum.LEFT:

                    infwidth = (int)((centerrect.Width - ((double)centerrect.Width * shortratio)) / 2);
                    infheight = (int)((centerrect.Height - ((double)centerrect.Height * longratio)) / 2);
                    rect.Inflate(-infwidth, -infheight);

                    myrect = new Rectangle(0, rect.Y, rect.X, rect.Height);
                    break;
                case SIDEEmnum.RIGHT:

                    infwidth = (int)((centerrect.Width - ((double)centerrect.Width * shortratio)) / 2);
                    infheight = (int)((centerrect.Height - ((double)centerrect.Height * longratio)) / 2);
                    rect.Inflate(-infwidth, -infheight);

                    myrect = new Rectangle(rect.Right, rect.Y, bmp1st.Width - rect.Right, rect.Height);
                    break;
            }

            bmpside = CopyImage(bmp1st, myrect);

            SIDEFound.ClearItems();

            //bmpside.Save(@"D:\JETEAZY\SIDE" + SIDEIndex.ToString() + ".BMP", ImageFormat.Bmp);
        }

        public void GetSideEX(SIDEEmnum side, Bitmap bmp, Rectangle rect)
        {
            SIDEIndex = side;
            //bmp1st = (Bitmap)bmpfirst.Clone(new Rectangle(0,0,bmpfirst.Width,bmpfirst.Height),bmpfirst.PixelFormat);
            bmp1st = CopyImage(bmp);

            Rectangle bmprect = new Rectangle(new Point(0, 0), bmp.Size);


            switch (side)
            {
                case SIDEEmnum.TOP:

                    bmprect = new Rectangle(0, 0, bmp.Width, rect.Y);
                    myrect = new Rectangle(rect.X, 0, rect.Width, rect.Height + rect.Y);

                    myrect.Intersect(bmprect);
                    break;
                case SIDEEmnum.BOTTOM:

                    bmprect = new Rectangle(0, rect.Bottom, bmp.Width, bmp.Height - rect.Bottom);
                    myrect = new Rectangle(rect.X, rect.Y, rect.Width, bmp.Height - rect.Top);

                    myrect.Intersect(bmprect);

                    break;
                case SIDEEmnum.LEFT:

                    bmprect = new Rectangle(0, 0, rect.Left, bmp.Height);
                    myrect = new Rectangle(0, rect.Y, rect.Width + rect.X, rect.Height);

                    myrect.Intersect(bmprect);
                    break;
                case SIDEEmnum.RIGHT:

                    bmprect = new Rectangle(rect.Right, 0, bmp.Width - rect.Right, bmp.Height);
                    myrect = new Rectangle(rect.X, rect.Y, bmp.Width - rect.X, rect.Height);

                    myrect.Intersect(bmprect);
                    break;
            }

            bmpside = CopyImage(bmp1st, myrect);

            SIDEFound.ClearItems();

            //bmpside.Save(@"D:\JETEAZY\SIDE" + SIDEIndex.ToString() + ".BMP", ImageFormat.Bmp);
        }


        public void GetSide(SIDEEmnum side, Bitmap bmpfirst, double longratio, double shortatio)
        {
            SIDEIndex = side;
            //bmp1st = (Bitmap)bmpfirst.Clone(new Rectangle(0,0,bmpfirst.Width,bmpfirst.Height),bmpfirst.PixelFormat);
            bmp1st = CopyImage(bmpfirst);

            lratio = longratio;
            sratio = shortatio;

            int rowwidth = (int)((double)bmpfirst.Width * longratio);
            int rowheight = (int)((double)bmpfirst.Height * shortatio);
            int colwidth = (int)((double)bmpfirst.Width * shortatio);
            int colheight = (int)((double)bmpfirst.Height * longratio);

            switch (side)
            {
                case SIDEEmnum.TOP:

                    myrect.X = (bmpfirst.Width - rowwidth) / 2;
                    myrect.Y = 0;
                    myrect.Width = rowwidth;
                    myrect.Height = rowheight;

                    break;
                case SIDEEmnum.BOTTOM:

                    myrect.X = (bmpfirst.Width - rowwidth) / 2;
                    myrect.Y = bmpfirst.Height - 1 - rowheight;
                    myrect.Width = rowwidth;
                    myrect.Height = rowheight;

                    break;
                case SIDEEmnum.LEFT:

                    myrect.X = 0;
                    myrect.Y = (bmpfirst.Height - colheight) / 2;
                    myrect.Width = colwidth;
                    myrect.Height = colheight;

                    break;
                case SIDEEmnum.RIGHT:

                    myrect.X = bmpfirst.Width - 1 - colwidth;
                    myrect.Y = (bmpfirst.Height - colheight) / 2;
                    myrect.Width = colwidth;
                    myrect.Height = colheight;

                    break;
            }

            //imgCrop.Rectangle = rect;
            //bmpside = imgCrop.Apply(bmp1st);

            bmpside = CopyImage(bmp1st, myrect);
            //bmpside = (Bitmap)bmp1st.Clone(rect, PixelFormat.Format8bppIndexed);
        }
        public void Process(SecondStepEnum method)
        {
            bmp2nd = UseMethod1(bmpside);

            //bmp2nd.Save(@"D:\JETEAZY\2ND.BMP", ImageFormat.Bmp);
            GetPts(bmp2nd, SIDEFound);
            bmp2nd.Save(@"D:\JETEAZY\S " + SIDEIndex.ToString() + "GETS.BMP", ImageFormat.Bmp);
        }

        public void Process(Bitmap bmp)
        {
            SIDEFound.ClearItems();

            Bitmap bmptmp = CopyImage(bmp);

            SearchLine(bmptmp, mysideCount, SIDEFound, 5);

            //bmptmp.Save(@"D:\JETEAZY\2ND-" + SIDEIndex.ToString() + ".BMP", ImageFormat.Bmp);
        }
        public void ProcessEX()
        {
            SIDEFound.ClearItems();

            Bitmap bmptmp = CopyImage(bmpside);

            //bmptmp.Save(@"D:\JETEAZY\1ST-" + SIDEIndex.ToString() + ".png", ImageFormat.Png);

            SearchLine8bitFX(bmptmp, true, SideFoundCollection, 1);

            //bmptmp.Save(@"D:\JETEAZY\2ND-" + SIDEIndex.ToString() + ".png", ImageFormat.Png);
        }
        public void ProcessIX(Bitmap bmp, double holesratio)
        {
            int index = 0;
            SIDEFound.ClearItems();
            Bitmap[] bmps = new Bitmap[20];

            bmps[index] = CopyImage(bmp);

            //DifferenceEdgeDetector diff = new DifferenceEdgeDetector();
            //bmps[index + 1] = diff.Apply(bmps[index]);
            //index++;

            OtsuThreshold otsu = new OtsuThreshold();
            bmps[index + 1] = otsu.Apply(bmps[index]);
            index++;

            CannyEdgeDetector canny = new CannyEdgeDetector(20, 100, 1.4d);
            bmps[index + 1] = canny.Apply(bmps[index]);
            index++;

            SISThreshold sis = new SISThreshold();
            bmps[index + 1] = sis.Apply(bmps[index]);
            index++;

            ConnectedComponentsLabeling connected = new ConnectedComponentsLabeling();
            bmps[index + 1] = connected.Apply(bmps[index]);
            index++;

            SearchLine(bmps[index], mysideCount, SIDEFound, 5);
            //bmps[index].Save(@"D:\JETEAZY\2ND-" + SIDEIndex.ToString() + ".BMP", ImageFormat.Bmp);

            SaveAllbmps(bmps, "IXT" + SIDEIndex.ToString() + "-");

            Clearbmps(bmps);
        }

        //尋找邊的距離的最大最小值，如果是allinside代表計算到棒棒
        public void ProcessDistance(ref string resultstr)
        {
            int i = 0;

            resultstr += SIDEIndex.ToString() + Environment.NewLine;

            foreach (SideFoundClass sideFound in SideFoundCollection.sidefounds)
            {
                if (i == 0)
                {
                    i++;
                    continue;
                }

                if (sideFound.CountItems() > 30)
                {
                    resultstr += "G" + i.ToString() + " max: " + sideFound.maxval.ToString().PadLeft(4) + ",min: " + sideFound.minval.ToString().PadLeft(4) + Environment.NewLine;
                    i++;
                }
            }
        }


        public void ProcessEssential(Bitmap bmp, double holesratio)
        {
            int index = 0;
            SIDEFound.ClearItems();
            Bitmap[] bmps = new Bitmap[20];

            bmps[index] = CopyImage(bmp);

            DifferenceEdgeDetector diff = new DifferenceEdgeDetector();
            bmps[index + 1] = diff.Apply(bmps[index]);
            index++;

            SISThreshold sis = new SISThreshold();
            bmps[index + 1] = sis.Apply(bmps[index]);
            index++;

            Invert invert = new Invert();
            bmps[index + 1] = invert.Apply(bmps[index]);
            index++;

            FillHoles fillHoles = new FillHoles();
            fillHoles.CoupledSizeFiltering = true;
            fillHoles.MaxHoleWidth = (int)(bmp.Width * holesratio / 100d);
            fillHoles.MaxHoleHeight = (int)(bmp.Height * holesratio / 100d);
            bmps[index + 1] = fillHoles.Apply(bmps[index]);
            index++;

            bmps[index + 1] = invert.Apply(bmps[index]);
            index++;

            ConnectedComponentsLabeling connected = new ConnectedComponentsLabeling();
            bmps[index + 1] = connected.Apply(bmps[index]);
            index++;

            SearchLine(bmps[index], mysideCount, SIDEFound, 5);
            //bmps[index].Save(@"D:\JETEAZY\2ND-" + SIDEIndex.ToString() + ".BMP", ImageFormat.Bmp);

            SaveAllbmps(bmps, "IXT" + SIDEIndex.ToString() + "-");

            Clearbmps(bmps);
        }
        void SaveAllbmps(Bitmap[] bmps, string headstr)
        {
            int i = 0;

            while (i < bmps.Length)
            {
                if (bmps[i] != null)
                    bmps[i].Save(@"D:\JETEAZY\" + headstr + i.ToString("00") + ".BMP", ImageFormat.Bmp);
                i++;
            }
        }
        void SaveAllbmps(Bitmap[] bmps, string headstr, ref string filenames)
        {
            int i = 0;

            while (i < bmps.Length)
            {
                if (bmps[i] != null)
                {
                    string filename = @"D:\JETEAZY\" + headstr + i.ToString("00") + ".BMP";

                    bmps[i].Save(filename, ImageFormat.Bmp);

                    filenames += filename + ",";

                }
                i++;
            }
        }

        public void DrawResult(Bitmap bmp, Color color)
        {
            DrawResult(bmp, color, 3);
        }

        public void DrawResult(Bitmap bmp, Color color, int width, bool isnew = false)
        {
            Graphics g = Graphics.FromImage(bmp);

            Point[] pts = new Point[SIDEFound.CountItems()];
            SIDEFound.FillPts(ref pts, myrect.Location, isnew);

            if (pts.Length > 1)
                g.DrawLines(new Pen(color, width), pts);

            g.Dispose();
        }
        public void DrawResultEX(Bitmap bmp, Color color, int width, bool isnew = false)
        {
            Graphics g = Graphics.FromImage(bmp);

            foreach (SideFoundClass sidedfound in SideFoundCollection.sidefounds)
            {
                Point[] pts = new Point[sidedfound.CountItems()];
                sidedfound.FillPtsEX(ref pts, myrect.Location, isnew);

                if (pts.Length > 1)
                    g.DrawLines(new Pen(color, width), pts);

            }
            g.Dispose();
        }
        public Point[] GetLinePoints()
        {
            Point[] pts = new Point[SIDEFound.CountItems()];
            SIDEFound.FillPts(ref pts, myrect.Location);
            return pts;
        }
        Bitmap UseMethod1(Bitmap bmpsrc)
        {
            Bitmap retbmp;
            Bitmap[] bmps = new Bitmap[10];

            bmpsrc.Save(@"D:\JETEAZY\2TS-" + SIDEIndex.ToString() + ".BMP", ImageFormat.Bmp);

            SISThreshold sis = new SISThreshold();
            bmps[0] = sis.Apply(bmpsrc);

            bmps[0].Save(@"D:\JETEAZY\2TS-" + SIDEIndex.ToString() + "0.BMP", ImageFormat.Bmp);

            FillHoles fillHoles = new FillHoles();
            fillHoles.CoupledSizeFiltering = true;
            fillHoles.MaxHoleWidth = 100;
            fillHoles.MaxHoleHeight = 100;
            bmps[1] = fillHoles.Apply(bmps[0]);

            bmps[1].Save(@"D:\JETEAZY\2TS-" + SIDEIndex.ToString() + "1.BMP", ImageFormat.Bmp);

            Closing closing = new Closing();
            bmps[2] = closing.Apply(bmps[1]);
            bmps[2].Save(@"D:\JETEAZY\2TS-" + SIDEIndex.ToString() + "2.BMP", ImageFormat.Bmp);

            fillHoles.CoupledSizeFiltering = true;
            fillHoles.MaxHoleWidth = 30;
            fillHoles.MaxHoleHeight = 30;
            bmps[3] = fillHoles.Apply(bmps[2]);

            CannyEdgeDetector canny = new CannyEdgeDetector(20, 100, 1.4d); //預設值
            bmps[5] = canny.Apply(bmps[3]);

            bmps[6] = closing.Apply(bmps[5]);

            Invert invert = new Invert();
            bmps[7] = invert.Apply(bmps[6]);

            //外框畫一圈白色讓洞洞分離，為了Fillholes參數
            DrawRectOutLine(bmps[7], 255);

            bmps[7].Save(@"D:\JETEAZY\2TS-" + SIDEIndex.ToString() + "7" +
                ".BMP", ImageFormat.Bmp);

            int holewidth = (int)((double)bmps[7].Width * 0.5d);
            int holeheight = (int)((double)bmps[7].Height * 0.5d);


            fillHoles.CoupledSizeFiltering = true;
            fillHoles.MaxHoleWidth = holewidth;
            fillHoles.MaxHoleHeight = holeheight;
            retbmp = fillHoles.Apply(bmps[7]);

            Clearbmps(bmps);

            return retbmp;
        }
        public void GetPts(Bitmap bmpsrc, SideFoundClass sidefound)
        {
            int fromx = 0;
            int fromy = 0;

            Point ptstart = new Point(0, 0);
            Point ptend = new Point(0, 0);

            switch (SIDEIndex)
            {
                case SIDEEmnum.TOP:
                case SIDEEmnum.BOTTOM:
                    ptstart.X = (int)(((double)bmpside.Width * (1d - rangeratio)) / 2d);
                    ptstart.Y = 0;

                    ptend.X = bmpside.Width - (int)(((double)bmpside.Width * (1d - rangeratio)) / 2d);
                    ptend.Y = 0;
                    break;
                case SIDEEmnum.LEFT:
                case SIDEEmnum.RIGHT:
                    ptstart.X = 0;
                    ptstart.Y = (int)(((double)bmpside.Height * (1d - rangeratio)) / 2d);

                    ptend.X = 0;
                    ptend.Y = bmpside.Height - (int)(((double)bmpside.Height * (1d - rangeratio)) / 2d);
                    break;
            }

            SearchLine8bit(bmpsrc, ptstart, ptend, false, sidefound, 5);
        }
        void SearchLine8bit(Bitmap bmpsrc, Point ptstart, Point ptend, bool ischeckwhite, SideFoundClass sidefound, int gap = 1)
        {
            byte CheckColor = 0;

            if (ischeckwhite)
                CheckColor = 255;
            else
                CheckColor = 0;

            Rectangle rectbmp = SimpleRect(bmpsrc.Size);
            BitmapData bmpData = bmpsrc.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

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

                    int xmax = ptend.X;
                    int ymax = ptend.Y;

                    int x = ptstart.X;
                    int y = ptstart.Y;

                    int iStride = bmpData.Stride;

                    switch (SIDEIndex)
                    {
                        case SIDEEmnum.TOP:
                            xmin = rectbmp.Left;
                            ymin = rectbmp.Top;

                            xmax = ptend.X;
                            ymax = rectbmp.Bottom - 1;

                            x = ptstart.X;
                            y = ymin;

                            pucStart = scan0 + (x - xmin) + (iStride * (y - ymin));

                            while (x < xmax)
                            {
                                y = ymin;
                                pucPtr = pucStart;

                                while (y < ymax)
                                {
                                    if (*pucPtr == CheckColor)
                                    {
                                        *pucPtr = 100;

                                        SideFoundItemClass sidefounditem =
                                            new SideFoundItemClass(
                                            new Point(x, ymin), new Point(x, y), y - ymin + 1);

                                        sidefound.Add(sidefounditem);

                                        break;
                                    }

                                    pucPtr += iStride;
                                    y++;
                                }
                                pucStart += gap;
                                x += gap;
                            }
                            break;
                        case SIDEEmnum.BOTTOM:
                            xmin = rectbmp.Left;
                            ymin = rectbmp.Top;

                            xmax = ptend.X;
                            ymax = rectbmp.Bottom - 1;

                            x = ptstart.X;
                            y = ymax;

                            pucStart = scan0 + (x - xmin) + (iStride * (y - ymin));

                            while (x < xmax)
                            {
                                y = ymax;
                                pucPtr = pucStart;

                                while (y >= ymin)
                                {
                                    if (*pucPtr == CheckColor)
                                    {
                                        *pucPtr = 100;

                                        SideFoundItemClass sidefounditem =
                                            new SideFoundItemClass(
                                            new Point(x, ymax), new Point(x, y), ymax - y + 1);

                                        sidefound.Add(sidefounditem);

                                        break;
                                    }

                                    pucPtr -= iStride;
                                    y--;
                                }
                                pucStart += gap;
                                x += gap;
                            }
                            break;
                        case SIDEEmnum.LEFT:
                            xmin = rectbmp.X;
                            ymin = rectbmp.Y;

                            xmax = rectbmp.Right - 1;
                            ymax = ptend.Y;

                            x = ptstart.X;
                            y = ptstart.Y;

                            pucStart = scan0 + (x - xmin) + (iStride * (y - ymin));

                            while (y < ymax)
                            {
                                x = ptstart.X;
                                pucPtr = pucStart;

                                while (x < xmax)
                                {
                                    if (*pucPtr == CheckColor)
                                    {
                                        *pucPtr = 100;

                                        SideFoundItemClass sidefounditem =
                                            new SideFoundItemClass(
                                            new Point(xmin, y), new Point(x, y), x - xmin + 1);

                                        sidefound.Add(sidefounditem);

                                        break;
                                    }

                                    pucPtr++;
                                    x++;
                                }
                                pucStart += iStride * gap;
                                y += gap;
                            }
                            break;
                        case SIDEEmnum.RIGHT:
                            xmin = rectbmp.X;
                            ymin = rectbmp.Y;

                            xmax = rectbmp.Right - 1;
                            ymax = ptend.Y;

                            x = xmax;
                            y = ptstart.Y;

                            pucStart = scan0 + (x - xmin) + (iStride * (y - ymin));

                            while (y < ymax)
                            {
                                x = xmax;
                                pucPtr = pucStart;

                                while (x >= xmin)
                                {
                                    if (*pucPtr == CheckColor)
                                    {
                                        *pucPtr = 100;

                                        SideFoundItemClass sidefounditem =
                                            new SideFoundItemClass(
                                            new Point(xmax, y), new Point(x, y), xmax - x + 1);

                                        sidefound.Add(sidefounditem);

                                        break;
                                    }

                                    pucPtr--;
                                    x--;
                                }
                                pucStart += iStride * gap;
                                y += gap;
                            }
                            break;
                    }

                    bmpsrc.UnlockBits(bmpData);
                }
            }
            catch (Exception ex)
            {
                //JetEazy.LoggerClass.Instance.WriteException(ex);
                bmpsrc.UnlockBits(bmpData);
            }
        }
        void SearchLine8bitEX(Bitmap bmpsrc, bool ischeckwhite, SideFoundCollectionClass sidefoundcollection, int dupgap, int gap = 1)
        {
            byte CheckColor = 0;

            if (ischeckwhite)
                CheckColor = 255;
            else
                CheckColor = 0;

            Rectangle rectbmp = SimpleRect(bmpsrc.Size);
            BitmapData bmpData = bmpsrc.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

            IntPtr Scan0 = bmpData.Scan0;

            try
            {
                unsafe
                {
                    byte* scan0 = (byte*)(void*)Scan0;
                    byte* pucPtr;
                    byte* pucStart;

                    int xmin = rectbmp.Left;
                    int ymin = rectbmp.Top;

                    int xmax = rectbmp.Right;
                    int ymax = rectbmp.Bottom;

                    int x = xmin;
                    int y = ymin;

                    int iStride = bmpData.Stride;

                    int sidefoundindex = 0;
                    int livedupgap = 1;
                    int lastval = 0;

                    switch (SIDEIndex)
                    {
                        case SIDEEmnum.TOP:
                            x = xmin;
                            y = ymax - 1;

                            pucStart = scan0 + (x - xmin) + (iStride * (y - ymin));

                            while (x < xmax)
                            {
                                y = ymax - 1;
                                pucPtr = pucStart;
                                sidefoundindex = 0;
                                lastval = y;

                                while (y > ymin - 1)
                                {
                                    livedupgap = 1;

                                    if (*pucPtr == CheckColor)
                                    {
                                        *pucPtr = 100;

                                        SideFoundItemClass sidefounditem = new SideFoundItemClass(new Point(x, y), Math.Abs(y - lastval));
                                        lastval = y;

                                        sidefoundcollection.sidefounds[sidefoundindex].Add(sidefounditem);

                                        sidefoundindex++;
                                        livedupgap = dupgap;
                                    }
                                    else
                                        *pucPtr = 30;

                                    pucPtr -= iStride * livedupgap;
                                    y -= livedupgap;
                                }

                                pucStart += gap;
                                x += gap;
                            }
                            break;
                        case SIDEEmnum.BOTTOM:
                            x = xmin;
                            y = ymin;

                            pucStart = scan0 + (x - xmin) + (iStride * (y - ymin));

                            while (x < xmax)
                            {
                                y = ymin;
                                pucPtr = pucStart;
                                sidefoundindex = 0;
                                lastval = y;

                                while (y < ymax)
                                {
                                    livedupgap = 1;

                                    if (*pucPtr == CheckColor)
                                    {
                                        *pucPtr = 100;

                                        SideFoundItemClass sidefounditem = new SideFoundItemClass(new Point(x, y), Math.Abs(y - lastval));
                                        lastval = y;

                                        sidefoundcollection.sidefounds[sidefoundindex].Add(sidefounditem);

                                        sidefoundindex++;
                                        livedupgap = dupgap;
                                    }
                                    //else
                                    //    *pucPtr = 30;

                                    pucPtr += iStride * livedupgap;
                                    y += livedupgap;
                                }
                                pucStart += gap;
                                x += gap;
                            }
                            break;
                        case SIDEEmnum.LEFT:
                            x = xmax - 1;
                            y = ymin;

                            pucStart = scan0 + (x - xmin) + (iStride * (y - ymin));

                            while (y < ymax)
                            {
                                x = xmax;
                                pucPtr = pucStart;
                                sidefoundindex = 0;
                                lastval = x;

                                while (x > xmin - 1)
                                {
                                    livedupgap = 1;

                                    if (*pucPtr == CheckColor)
                                    {
                                        *pucPtr = 100;

                                        SideFoundItemClass sidefounditem = new SideFoundItemClass(new Point(x, y), Math.Abs(x - lastval));
                                        lastval = x;

                                        sidefoundcollection.sidefounds[sidefoundindex].Add(sidefounditem);

                                        sidefoundindex++;
                                        livedupgap = dupgap;
                                    }
                                    //else
                                    //    *pucPtr = 30;

                                    pucPtr -= livedupgap;
                                    x -= livedupgap;
                                }
                                pucStart += iStride * gap;
                                y += gap;
                            }
                            break;
                        case SIDEEmnum.RIGHT:

                            x = xmin;
                            y = ymin;

                            pucStart = scan0 + (x - xmin) + (iStride * (y - ymin));

                            while (y < ymax)
                            {
                                x = xmin;
                                pucPtr = pucStart;
                                sidefoundindex = 0;
                                lastval = x;

                                while (x < xmax)
                                {
                                    livedupgap = 1;

                                    if (*pucPtr == CheckColor)
                                    {
                                        *pucPtr = 100;

                                        SideFoundItemClass sidefounditem = new SideFoundItemClass(new Point(x, y), Math.Abs(x - lastval));
                                        lastval = x;

                                        sidefoundcollection.sidefounds[sidefoundindex].Add(sidefounditem);

                                        sidefoundindex++;
                                        livedupgap = dupgap;
                                    }
                                    //else
                                    //    *pucPtr = 30;

                                    pucPtr += livedupgap;
                                    x += livedupgap;
                                }
                                pucStart += iStride * gap;
                                y += gap;
                            }
                            break;
                    }

                    bmpsrc.UnlockBits(bmpData);
                }
            }
            catch (Exception ex)
            {
                //JetEazy.LoggerClass.Instance.WriteException(ex);
                bmpsrc.UnlockBits(bmpData);
            }
        }
        void SearchLine8bitFX(Bitmap bmpsrc, bool ischeckwhite, SideFoundCollectionClass sidefoundcollection, int dupgap, int gap = 1)
        {
            byte CheckColor = 0;

            if (ischeckwhite)
                CheckColor = 255;
            else
                CheckColor = 0;

            Rectangle rectbmp = SimpleRect(bmpsrc.Size);
            BitmapData bmpData = bmpsrc.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

            IntPtr Scan0 = bmpData.Scan0;

            try
            {
                unsafe
                {
                    byte* scan0 = (byte*)(void*)Scan0;
                    byte* pucPtr;
                    byte* pucStart;

                    int xmin = rectbmp.Left;
                    int ymin = rectbmp.Top;

                    int xmax = rectbmp.Right;
                    int ymax = rectbmp.Bottom;

                    int x = xmin;
                    int y = ymin;

                    int iStride = bmpData.Stride;

                    int sidefoundindex = 0;
                    int livedupgap = 1;
                    int lastval = 0;

                    switch (SIDEIndex)
                    {
                        case SIDEEmnum.TOP:
                            x = xmin;
                            y = ymax - 1;

                            pucStart = scan0 + (x - xmin) + (iStride * (y - ymin));

                            while (x < xmax)
                            {
                                y = ymax - 1;
                                pucPtr = pucStart;
                                sidefoundindex = 0;
                                lastval = y;

                                while (y > ymin - 1)
                                {
                                    livedupgap = 1;

                                    if (*pucPtr == CheckColor)
                                    {
                                        if (CheckColor == 255)
                                        {
                                            *pucPtr = 100;

                                            SideFoundItemClass sidefounditem = new SideFoundItemClass(new Point(x, y), Math.Abs(y - lastval));
                                            lastval = y;

                                            sidefoundcollection.sidefounds[sidefoundindex].Add(sidefounditem);

                                            sidefoundindex++;
                                            livedupgap = 1;

                                            CheckColor = 0;
                                        }
                                        else
                                            CheckColor = 255;
                                    }
                                    else
                                    {
                                        if (*pucPtr == 0)
                                            *pucPtr = 30;
                                    }
                                    pucPtr -= iStride * livedupgap;
                                    y -= livedupgap;
                                }

                                pucStart += gap;
                                x += gap;
                            }
                            break;
                        case SIDEEmnum.BOTTOM:
                            x = xmin;
                            y = ymin;

                            pucStart = scan0 + (x - xmin) + (iStride * (y - ymin));

                            while (x < xmax)
                            {
                                y = ymin;
                                pucPtr = pucStart;
                                sidefoundindex = 0;
                                lastval = y;

                                while (y < ymax)
                                {
                                    livedupgap = 1;

                                    if (*pucPtr == CheckColor)
                                    {
                                        if (CheckColor == 255)
                                        {
                                            *pucPtr = 100;

                                            SideFoundItemClass sidefounditem = new SideFoundItemClass(new Point(x, y), Math.Abs(y - lastval));
                                            lastval = y;

                                            sidefoundcollection.sidefounds[sidefoundindex].Add(sidefounditem);

                                            sidefoundindex++;
                                            livedupgap = 1;

                                            CheckColor = 0;
                                        }
                                        else
                                            CheckColor = 255;
                                    }
                                    else
                                        if (*pucPtr == 0)
                                        *pucPtr = 30;

                                    pucPtr += iStride * livedupgap;
                                    y += livedupgap;
                                }
                                pucStart += gap;
                                x += gap;
                            }
                            break;
                        case SIDEEmnum.LEFT:
                            x = xmax - 1;
                            y = ymin;

                            pucStart = scan0 + (x - xmin) + (iStride * (y - ymin));

                            while (y < ymax)
                            {
                                x = xmax;
                                pucPtr = pucStart;
                                sidefoundindex = 0;
                                lastval = x;

                                while (x > xmin - 1)
                                {
                                    livedupgap = 1;

                                    if (*pucPtr == CheckColor)
                                    {
                                        if (CheckColor == 255)
                                        {
                                            *pucPtr = 100;

                                            SideFoundItemClass sidefounditem = new SideFoundItemClass(new Point(x, y), Math.Abs(x - lastval));
                                            lastval = x;

                                            sidefoundcollection.sidefounds[sidefoundindex].Add(sidefounditem);

                                            sidefoundindex++;
                                            livedupgap = 1;
                                            CheckColor = 0;
                                        }
                                        else
                                            CheckColor = 255;
                                    }
                                    else
                                        if (*pucPtr == 0)
                                        *pucPtr = 30;

                                    pucPtr -= livedupgap;
                                    x -= livedupgap;
                                }
                                pucStart += iStride * gap;
                                y += gap;
                            }
                            break;
                        case SIDEEmnum.RIGHT:

                            x = xmin;
                            y = ymin;

                            pucStart = scan0 + (x - xmin) + (iStride * (y - ymin));

                            while (y < ymax)
                            {
                                x = xmin;
                                pucPtr = pucStart;
                                sidefoundindex = 0;
                                lastval = x;

                                while (x < xmax)
                                {
                                    livedupgap = 1;

                                    if (*pucPtr == CheckColor)
                                    {
                                        if (CheckColor == 255)
                                        {
                                            *pucPtr = 100;

                                            SideFoundItemClass sidefounditem = new SideFoundItemClass(new Point(x, y), Math.Abs(x - lastval));
                                            lastval = x;

                                            sidefoundcollection.sidefounds[sidefoundindex].Add(sidefounditem);

                                            sidefoundindex++;
                                            livedupgap = 1;
                                            CheckColor = 0;
                                        }
                                        else
                                            CheckColor = 255;
                                    }
                                    else
                                        if (*pucPtr == 0)
                                        *pucPtr = 30;

                                    pucPtr += livedupgap;
                                    x += livedupgap;
                                }
                                pucStart += iStride * gap;
                                y += gap;
                            }
                            break;
                    }

                    bmpsrc.UnlockBits(bmpData);
                }
            }
            catch (Exception ex)
            {
                //JetEazy.LoggerClass.Instance.WriteException(ex);
                bmpsrc.UnlockBits(bmpData);
            }
        }

        void SearchLine(Bitmap bmpsrc, int sidecount, SideFoundClass sidefound, int gap = 1)
        {
            Rectangle rectbmp = SimpleRect(bmpsrc.Size);
            BitmapData bmpData = bmpsrc.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

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

                    int xmax = rectbmp.Right - 1;
                    int ymax = rectbmp.Bottom - 1;

                    int x = rectbmp.X;
                    int y = rectbmp.Y;

                    int iStride = bmpData.Stride;


                    byte[] lastcolor = new byte[3];

                    int ccount = sidecount;
                    Point firstpt = new Point();

                    switch (SIDEIndex)
                    {
                        case SIDEEmnum.BOTTOM:
                            xmin = rectbmp.X;
                            ymin = rectbmp.Y;

                            xmax = rectbmp.Right - 1;
                            ymax = rectbmp.Bottom - 1;

                            x = xmin;
                            y = ymin;

                            pucStart = scan0 + (x - xmin) * 4 + (iStride * (y - ymin));

                            while (x < xmax)
                            {
                                y = ymin;
                                pucPtr = pucStart;
                                ccount = sidecount;

                                while (y < ymax)
                                {
                                    if (pucPtr[0] + pucPtr[1] + pucPtr[2] != 0)
                                    {
                                        if (lastcolor[0] == pucPtr[0] &&
                                            lastcolor[1] == pucPtr[1] &&
                                            lastcolor[2] == pucPtr[2])
                                        {
                                            pucPtr[0] = 255;
                                            pucPtr[1] = 255;
                                            pucPtr[2] = 255;

                                            pucPtr += iStride;
                                            y++;

                                            continue;
                                        }

                                        lastcolor[0] = pucPtr[0];
                                        lastcolor[1] = pucPtr[1];
                                        lastcolor[2] = pucPtr[2];

                                        pucPtr[0] = 255;
                                        pucPtr[1] = 255;
                                        pucPtr[2] = 255;

                                        if (ccount == sidecount)
                                            firstpt = new Point(x, y);

                                        ccount--;

                                        if (ccount == 0)
                                        {
                                            SideFoundItemClass sidefounditem =
                                                new SideFoundItemClass(
                                                firstpt, new Point(x, y), y - firstpt.Y + 1);

                                            sidefound.Add(sidefounditem);
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        lastcolor[0] = pucPtr[0];
                                        lastcolor[1] = pucPtr[1];
                                        lastcolor[2] = pucPtr[2];
                                    }

                                    pucPtr += iStride;
                                    y++;
                                }
                                pucStart += gap * 4;
                                x += gap;
                            }
                            break;
                        case SIDEEmnum.TOP:
                            xmin = rectbmp.Left;
                            ymin = rectbmp.Top;

                            xmax = rectbmp.Right - 1;
                            ymax = rectbmp.Bottom - 1;

                            x = xmin;
                            y = ymax;

                            pucStart = scan0 + (x - xmin) * 4 + (iStride * (y - ymin));

                            while (x < xmax)
                            {
                                y = ymax;
                                pucPtr = pucStart;
                                ccount = sidecount;

                                while (y >= ymin)
                                {
                                    if (pucPtr[0] + pucPtr[1] + pucPtr[2] != 0)
                                    {
                                        if (lastcolor[0] == pucPtr[0] &&
                                            lastcolor[1] == pucPtr[1] &&
                                            lastcolor[2] == pucPtr[2])
                                        {
                                            pucPtr[0] = 255;
                                            pucPtr[1] = 255;
                                            pucPtr[2] = 255;

                                            pucPtr -= iStride;
                                            y--;

                                            continue;
                                        }

                                        lastcolor[0] = pucPtr[0];
                                        lastcolor[1] = pucPtr[1];
                                        lastcolor[2] = pucPtr[2];

                                        pucPtr[0] = 255;
                                        pucPtr[1] = 255;
                                        pucPtr[2] = 255;

                                        if (ccount == sidecount)
                                            firstpt = new Point(x, y);

                                        ccount--;

                                        if (ccount == 0)
                                        {
                                            SideFoundItemClass sidefounditem =
                                                new SideFoundItemClass(
                                                firstpt, new Point(x, y), firstpt.Y - y + 1);

                                            sidefound.Add(sidefounditem);
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        lastcolor[0] = pucPtr[0];
                                        lastcolor[1] = pucPtr[1];
                                        lastcolor[2] = pucPtr[2];
                                    }

                                    pucPtr -= iStride;
                                    y--;
                                }
                                pucStart += gap * 4;
                                x += gap;
                            }
                            break;
                        case SIDEEmnum.RIGHT:
                            xmin = rectbmp.X;
                            ymin = rectbmp.Y;

                            xmax = rectbmp.Right - 1;
                            ymax = rectbmp.Bottom - 1;

                            x = xmin;
                            y = ymin;

                            pucStart = scan0 + (x - xmin) * 4 + (iStride * (y - ymin));

                            while (y < ymax)
                            {
                                x = xmin;
                                pucPtr = pucStart;
                                ccount = sidecount;

                                while (x < xmax)
                                {
                                    if (pucPtr[0] + pucPtr[1] + pucPtr[2] != 0)
                                    {
                                        if (lastcolor[0] == pucPtr[0] &&
                                            lastcolor[1] == pucPtr[1] &&
                                            lastcolor[2] == pucPtr[2])
                                        {
                                            pucPtr[0] = 255;
                                            pucPtr[1] = 255;
                                            pucPtr[2] = 255;

                                            pucPtr += 4;
                                            x++;
                                            continue;
                                        }

                                        lastcolor[0] = pucPtr[0];
                                        lastcolor[1] = pucPtr[1];
                                        lastcolor[2] = pucPtr[2];

                                        pucPtr[0] = 255;
                                        pucPtr[1] = 255;
                                        pucPtr[2] = 255;

                                        if (ccount == sidecount)
                                            firstpt = new Point(x, y);

                                        ccount--;

                                        if (ccount == 0)
                                        {
                                            SideFoundItemClass sidefounditem =
                                                new SideFoundItemClass(
                                                firstpt, new Point(x, y), x - firstpt.X + 1);

                                            sidefound.Add(sidefounditem);
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        lastcolor[0] = pucPtr[0];
                                        lastcolor[1] = pucPtr[1];
                                        lastcolor[2] = pucPtr[2];
                                    }

                                    pucPtr += 4;
                                    x++;
                                }
                                pucStart += iStride * gap;
                                y += gap;
                            }
                            break;
                        case SIDEEmnum.LEFT:
                            xmin = rectbmp.X;
                            ymin = rectbmp.Y;

                            xmax = rectbmp.Right - 1;
                            ymax = rectbmp.Bottom - 1;

                            x = xmax;
                            y = ymin;

                            pucStart = scan0 + (x - xmin) * 4 + (iStride * (y - ymin));

                            while (y < ymax)
                            {
                                x = xmax;
                                pucPtr = pucStart;
                                ccount = sidecount;

                                while (x >= xmin)
                                {
                                    if (pucPtr[0] + pucPtr[1] + pucPtr[2] != 0)
                                    {
                                        if (lastcolor[0] == pucPtr[0] &&
                                            lastcolor[1] == pucPtr[1] &&
                                            lastcolor[2] == pucPtr[2])
                                        {
                                            pucPtr[0] = 255;
                                            pucPtr[1] = 255;
                                            pucPtr[2] = 255;

                                            pucPtr -= 4;
                                            x--;
                                            continue;
                                        }

                                        lastcolor[0] = pucPtr[0];
                                        lastcolor[1] = pucPtr[1];
                                        lastcolor[2] = pucPtr[2];

                                        pucPtr[0] = 255;
                                        pucPtr[1] = 255;
                                        pucPtr[2] = 255;

                                        if (ccount == sidecount)
                                            firstpt = new Point(x, y);

                                        ccount--;

                                        if (ccount == 0)
                                        {
                                            SideFoundItemClass sidefounditem =
                                                new SideFoundItemClass(
                                                firstpt, new Point(x, y), firstpt.X - x + 1);

                                            sidefound.Add(sidefounditem);
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        lastcolor[0] = pucPtr[0];
                                        lastcolor[1] = pucPtr[1];
                                        lastcolor[2] = pucPtr[2];
                                    }

                                    pucPtr -= 4;
                                    x--;
                                }
                                pucStart += iStride * gap;
                                y += gap;
                            }
                            break;
                    }

                    bmpsrc.UnlockBits(bmpData);
                }
            }
            catch (Exception ex)
            {
                //JetEazy.LoggerClass.Instance.WriteException(ex);
                bmpsrc.UnlockBits(bmpData);
            }
        }
        void SearchLineIX(Bitmap bmpsrc, int sidecount, SideFoundClass sidefound, int gap = 1)
        {
            Rectangle rectbmp = SimpleRect(bmpsrc.Size);
            BitmapData bmpData = bmpsrc.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

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

                    int xmax = rectbmp.Right - 1;
                    int ymax = rectbmp.Bottom - 1;

                    int x = rectbmp.X;
                    int y = rectbmp.Y;

                    int iStride = bmpData.Stride;


                    byte[] lastcolor = new byte[3];

                    int ccount = sidecount;
                    Point firstpt = new Point();

                    switch (SIDEIndex)
                    {
                        case SIDEEmnum.BOTTOM:
                            xmin = rectbmp.X;
                            ymin = rectbmp.Y;

                            xmax = rectbmp.Right - 1;
                            ymax = rectbmp.Bottom - 1;

                            x = xmin;
                            y = ymin;

                            pucStart = scan0 + (x - xmin) * 4 + (iStride * (y - ymin));

                            while (x < xmax)
                            {
                                y = ymin;
                                pucPtr = pucStart;
                                ccount = sidecount;

                                while (y < ymax)
                                {
                                    if (pucPtr[0] + pucPtr[1] + pucPtr[2] != 0)
                                    {
                                        if (lastcolor[0] == pucPtr[0] &&
                                            lastcolor[1] == pucPtr[1] &&
                                            lastcolor[2] == pucPtr[2])
                                        {
                                            pucPtr[0] = 255;
                                            pucPtr[1] = 255;
                                            pucPtr[2] = 255;

                                            pucPtr += iStride;
                                            y++;

                                            continue;
                                        }

                                        lastcolor[0] = pucPtr[0];
                                        lastcolor[1] = pucPtr[1];
                                        lastcolor[2] = pucPtr[2];

                                        pucPtr[0] = 255;
                                        pucPtr[1] = 255;
                                        pucPtr[2] = 255;

                                        if (ccount == sidecount)
                                            firstpt = new Point(x, y);

                                        ccount--;

                                        if (ccount == 0)
                                        {
                                            SideFoundItemClass sidefounditem =
                                                new SideFoundItemClass(
                                                firstpt, new Point(x, y), y - firstpt.Y + 1);

                                            sidefound.Add(sidefounditem);
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        lastcolor[0] = pucPtr[0];
                                        lastcolor[1] = pucPtr[1];
                                        lastcolor[2] = pucPtr[2];
                                    }

                                    pucPtr += iStride;
                                    y++;
                                }
                                pucStart += gap * 4;
                                x += gap;
                            }
                            break;
                        case SIDEEmnum.TOP:
                            xmin = rectbmp.Left;
                            ymin = rectbmp.Top;

                            xmax = rectbmp.Right - 1;
                            ymax = rectbmp.Bottom - 1;

                            x = xmin;
                            y = ymax;

                            pucStart = scan0 + (x - xmin) * 4 + (iStride * (y - ymin));

                            while (x < xmax)
                            {
                                y = ymax;
                                pucPtr = pucStart;
                                ccount = sidecount;

                                while (y >= ymin)
                                {
                                    if (pucPtr[0] + pucPtr[1] + pucPtr[2] != 0)
                                    {
                                        if (lastcolor[0] == pucPtr[0] &&
                                            lastcolor[1] == pucPtr[1] &&
                                            lastcolor[2] == pucPtr[2])
                                        {
                                            pucPtr[0] = 255;
                                            pucPtr[1] = 255;
                                            pucPtr[2] = 255;

                                            pucPtr -= iStride;
                                            y--;

                                            continue;
                                        }

                                        lastcolor[0] = pucPtr[0];
                                        lastcolor[1] = pucPtr[1];
                                        lastcolor[2] = pucPtr[2];

                                        pucPtr[0] = 255;
                                        pucPtr[1] = 255;
                                        pucPtr[2] = 255;

                                        if (ccount == sidecount)
                                            firstpt = new Point(x, y);

                                        ccount--;

                                        if (ccount == 0)
                                        {
                                            SideFoundItemClass sidefounditem =
                                                new SideFoundItemClass(
                                                firstpt, new Point(x, y), firstpt.Y - y + 1);

                                            sidefound.Add(sidefounditem);
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        lastcolor[0] = pucPtr[0];
                                        lastcolor[1] = pucPtr[1];
                                        lastcolor[2] = pucPtr[2];
                                    }

                                    pucPtr -= iStride;
                                    y--;
                                }
                                pucStart += gap * 4;
                                x += gap;
                            }
                            break;
                        case SIDEEmnum.RIGHT:
                            xmin = rectbmp.X;
                            ymin = rectbmp.Y;

                            xmax = rectbmp.Right - 1;
                            ymax = rectbmp.Bottom - 1;

                            x = xmin;
                            y = ymin;

                            pucStart = scan0 + (x - xmin) * 4 + (iStride * (y - ymin));

                            while (y < ymax)
                            {
                                x = xmin;
                                pucPtr = pucStart;
                                ccount = sidecount;

                                while (x < xmax)
                                {
                                    if (pucPtr[0] + pucPtr[1] + pucPtr[2] != 0)
                                    {
                                        if (lastcolor[0] == pucPtr[0] &&
                                            lastcolor[1] == pucPtr[1] &&
                                            lastcolor[2] == pucPtr[2])
                                        {
                                            pucPtr[0] = 255;
                                            pucPtr[1] = 255;
                                            pucPtr[2] = 255;

                                            pucPtr += 4;
                                            x++;
                                            continue;
                                        }

                                        lastcolor[0] = pucPtr[0];
                                        lastcolor[1] = pucPtr[1];
                                        lastcolor[2] = pucPtr[2];

                                        pucPtr[0] = 255;
                                        pucPtr[1] = 255;
                                        pucPtr[2] = 255;

                                        if (ccount == sidecount)
                                            firstpt = new Point(x, y);

                                        ccount--;

                                        if (ccount == 0)
                                        {
                                            SideFoundItemClass sidefounditem =
                                                new SideFoundItemClass(
                                                firstpt, new Point(x, y), x - firstpt.X + 1);

                                            sidefound.Add(sidefounditem);
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        lastcolor[0] = pucPtr[0];
                                        lastcolor[1] = pucPtr[1];
                                        lastcolor[2] = pucPtr[2];
                                    }

                                    pucPtr += 4;
                                    x++;
                                }
                                pucStart += iStride * gap;
                                y += gap;
                            }
                            break;
                        case SIDEEmnum.LEFT:
                            xmin = rectbmp.X;
                            ymin = rectbmp.Y;

                            xmax = rectbmp.Right - 1;
                            ymax = rectbmp.Bottom - 1;

                            x = xmax;
                            y = ymin;

                            pucStart = scan0 + (x - xmin) * 4 + (iStride * (y - ymin));

                            while (y < ymax)
                            {
                                x = xmax;
                                pucPtr = pucStart;
                                ccount = sidecount;

                                while (x >= xmin)
                                {
                                    if (pucPtr[0] + pucPtr[1] + pucPtr[2] != 0)
                                    {
                                        if (lastcolor[0] == pucPtr[0] &&
                                            lastcolor[1] == pucPtr[1] &&
                                            lastcolor[2] == pucPtr[2])
                                        {
                                            pucPtr[0] = 255;
                                            pucPtr[1] = 255;
                                            pucPtr[2] = 255;

                                            pucPtr -= 4;
                                            x--;
                                            continue;
                                        }

                                        lastcolor[0] = pucPtr[0];
                                        lastcolor[1] = pucPtr[1];
                                        lastcolor[2] = pucPtr[2];

                                        pucPtr[0] = 255;
                                        pucPtr[1] = 255;
                                        pucPtr[2] = 255;

                                        if (ccount == sidecount)
                                            firstpt = new Point(x, y);

                                        ccount--;

                                        if (ccount == 0)
                                        {
                                            SideFoundItemClass sidefounditem =
                                                new SideFoundItemClass(
                                                firstpt, new Point(x, y), firstpt.X - x + 1);

                                            sidefound.Add(sidefounditem);
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        lastcolor[0] = pucPtr[0];
                                        lastcolor[1] = pucPtr[1];
                                        lastcolor[2] = pucPtr[2];
                                    }

                                    pucPtr -= 4;
                                    x--;
                                }
                                pucStart += iStride * gap;
                                y += gap;
                            }
                            break;
                    }

                    bmpsrc.UnlockBits(bmpData);
                }
            }
            catch (Exception ex)
            {
                //JetEazy.LoggerClass.Instance.WriteException(ex);
                bmpsrc.UnlockBits(bmpData);
            }
        }

        Rectangle SimpleRect(Size Sz)
        {
            return new Rectangle(0, 0, Sz.Width, Sz.Height);
        }
        public void Suiccied()
        {
            bmp1st.Dispose();
            bmpside.Dispose();

            SideFoundCollection.Suicide();
        }
        Bitmap CopyImage(Bitmap bmp)
        {
            return CopyImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
        }
        Bitmap CopyImage(Bitmap bmp, Rectangle rect)
        {
            return (Bitmap)bmp.Clone(rect, bmp.PixelFormat);
        }
        void Clearbmps(Bitmap[] bmps)
        {
            int i = 0;
            while (i < bmps.Length)
            {
                if (bmps[i] != null)
                    bmps[i].Dispose();
                i++;
            }
        }
        void DrawRectOutLine(Bitmap bmp, byte color)
        {
            Rectangle rectbmp = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
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
                    pucStart = scan0 + (x - xmin) + (iStride * (y - ymin));

                    while (y < ymax)
                    {
                        if (y != 0 && y != ymax - 1)
                        {
                            pucPtr = pucStart;

                            *pucPtr = color;

                            pucPtr += (xmax - xmin - 1);

                            *pucPtr = color;

                            pucStart += iStride;
                            y++;
                            continue;
                        }

                        x = xmin;
                        pucPtr = pucStart;
                        while (x < xmax)
                        {
                            *pucPtr = color;

                            pucPtr++;
                            x++;
                        }
                        pucStart += iStride;
                        y++;
                    }
                    bmp.UnlockBits(bmpData);
                }
            }
            catch (Exception ex)
            {
                bmp.UnlockBits(bmpData);
            }

        }

    }

    public class JzFourSideAnalyze
    {
        public Bitmap bmpOrg;
        public Bitmap bmpFirst;
        public Bitmap bmpSecond;
        Bitmap bmpThird;

        public string allresultstr = "";

        public SideDataClass[] SIDEData = new SideDataClass[(int)SIDEEmnum.COUNT];

        public JzFourSideAnalyze()
        {

        }

        /// <summary>
        /// 第一階段，把中央晶片的白色去除，變成週圍顏色環繞的膠的大致顏色
        /// </summary>
        /// <param name="bmporg"></param>
        /// <param name="holewidth"></param>
        /// <param name="holeheight"></param>
        /// <param name="isand"></param>
        /// <param name="erosioncount"></param>
        public void GetData(Bitmap bmporg, int holewidth, int holeheight, bool isand, int erosioncount, FirstStepEnum firstStep = FirstStepEnum.FMethod1)
        {
            int i = 0;
            Bitmap[] bmps = new Bitmap[20];

            if (bmpOrg != null)
                bmpOrg.Dispose();
            if (bmpFirst != null)
                bmpFirst.Dispose();
            if (bmpSecond != null)
                bmpSecond.Dispose();
            if (bmpThird != null)
                bmpThird.Dispose();

            bmps[0] = ConvertToGrayScale(bmporg);

            OtsuThreshold otsu = new OtsuThreshold();
            bmps[1] = otsu.Apply(bmps[0]);


            bmps[1].Save(@"D:\JETEAZY\T1.BMP", ImageFormat.Bmp);

            FillHoles fillHoles = new FillHoles();
            fillHoles.CoupledSizeFiltering = isand;
            fillHoles.MaxHoleWidth = holewidth;
            fillHoles.MaxHoleHeight = holeheight;
            bmps[2] = fillHoles.Apply(bmps[1]);

            bmps[2].Save(@"D:\JETEAZY\T2.BMP", ImageFormat.Bmp);

            Invert invert = new Invert();
            bmps[3] = invert.Apply(bmps[2]);
            bmps[5] = fillHoles.Apply(bmps[3]);


            bmps[3].Save(@"D:\JETEAZY\T3.BMP", ImageFormat.Bmp);
            bmps[5].Save(@"D:\JETEAZY\T5.BMP", ImageFormat.Bmp);

            Erosion erosion = new Erosion();

            while (i < erosioncount)
            {
                bmps[6 + i] = erosion.Apply(bmps[5 + i]);
                i++;
            }

            int[] histogramdata = new int[256];
            JzHistogramClass jzHistogram = new JzHistogramClass(1);
            JzHistogramResult jzhresult = new JzHistogramResult();
            jzHistogram.GetHistogramData(bmps[0], bmps[6 + i - 1], true, histogramdata);
            GetHistogramData(histogramdata, jzhresult);

            bmps[6 + i - 1].Save(@"D:\JETEAZY\T" + (6 + i - 1).ToString() + ".BMP", ImageFormat.Bmp);

            bmpOrg = CopyImage(bmporg);
            bmpFirst = CopyImage(bmps[0]);

            jzHistogram.FillColor(bmpFirst, bmps[6 + i - 1], false, (byte)jzhresult.min);

            bmpFirst.Save(@"D:\JETEAZY\TFIRST.BMP", ImageFormat.Bmp);


            Clearbmps(bmps);
        }

        //小
        public void GetDataEX(Bitmap bmporg, int holewidth, int holeheight, bool isand, int erosioncount)
        {
            int i = 0;
            int bmpindex = 0;

            Bitmap[] bmps = new Bitmap[20];

            if (bmpOrg != null)
                bmpOrg.Dispose();
            if (bmpFirst != null)
                bmpFirst.Dispose();
            if (bmpSecond != null)
                bmpSecond.Dispose();
            if (bmpThird != null)
                bmpThird.Dispose();

            HistogramEqualization histogramEqualization = new HistogramEqualization();
            bmps[bmpindex] = histogramEqualization.Apply(bmporg);

            Blur blur = new Blur();
            bmps[bmpindex + 1] = blur.Apply(bmps[bmpindex]);
            bmpindex++;

            ContrastStretch contraststrch = new ContrastStretch();
            bmps[bmpindex + 1] = contraststrch.Apply(bmps[bmpindex]);
            bmpindex++;

            bmps[bmpindex + 1] = ConvertToGrayScale(bmps[bmpindex]);
            bmpindex++;

            SISThreshold sis = new SISThreshold();
            bmps[bmpindex + 1] = sis.Apply(bmps[bmpindex]);
            bmpindex++;

            CannyEdgeDetector canny = new CannyEdgeDetector(20, 100, 1.4d);
            bmps[bmpindex + 1] = canny.Apply(bmps[bmpindex]);
            bmpindex++;

            Invert invert = new Invert();
            bmps[bmpindex + 1] = invert.Apply(bmps[bmpindex]);
            bmpindex++;

            FillHoles fillHoles = new FillHoles();
            fillHoles.CoupledSizeFiltering = isand;
            fillHoles.MaxHoleWidth = holewidth;
            fillHoles.MaxHoleHeight = holeheight;

            bmps[bmpindex + 1] = fillHoles.Apply(bmps[bmpindex]);
            bmpindex++;

            bmps[bmpindex + 1] = invert.Apply(bmps[bmpindex]);
            bmpindex++;

            //Closing closing = new Closing();
            //bmps[bmpindex + 1] = closing.Apply(bmps[bmpindex]);
            //bmpindex++;

            ConnectedComponentsLabeling clabeling = new ConnectedComponentsLabeling();
            bmps[bmpindex + 1] = clabeling.Apply(bmps[bmpindex]);
            bmpindex++;

            SaveAllbmps(bmps, "NEW-");

            bmpFirst = CopyImage(bmps[bmpindex]);

            Clearbmps(bmps);
        }

        public void GetDataFX(Bitmap bmporg, int holewidth, int holeheight, bool isand, int erosioncount)
        {
            int i = 0;
            int bmpindex = 0;

            Bitmap[] bmps = new Bitmap[20];

            if (bmpOrg != null)
                bmpOrg.Dispose();
            if (bmpFirst != null)
                bmpFirst.Dispose();
            if (bmpSecond != null)
                bmpSecond.Dispose();
            if (bmpThird != null)
                bmpThird.Dispose();


            HistogramEqualization histogramEqualization = new HistogramEqualization();
            bmps[bmpindex] = histogramEqualization.Apply(bmporg);

            Blur blur = new Blur();
            bmps[bmpindex + 1] = blur.Apply(bmps[bmpindex]);
            bmpindex++;

            ContrastStretch contraststrch = new ContrastStretch();
            bmps[bmpindex + 1] = contraststrch.Apply(bmps[bmpindex]);
            bmpindex++;

            bmps[bmpindex + 1] = ConvertToGrayScale(bmps[bmpindex]);
            bmpindex++;

            SISThreshold sis = new SISThreshold();
            bmps[bmpindex + 1] = sis.Apply(bmps[bmpindex]);
            bmpindex++;

            CannyEdgeDetector canny = new CannyEdgeDetector(20, 100, 1.4d);
            bmps[bmpindex + 1] = canny.Apply(bmps[bmpindex]);
            bmpindex++;

            Invert invert = new Invert();
            bmps[bmpindex + 1] = invert.Apply(bmps[bmpindex]);
            bmpindex++;

            FillHoles fillHoles = new FillHoles();
            fillHoles.CoupledSizeFiltering = isand;
            fillHoles.MaxHoleWidth = holewidth;
            fillHoles.MaxHoleHeight = holeheight;

            bmps[bmpindex + 1] = fillHoles.Apply(bmps[bmpindex]);
            bmpindex++;

            bmps[bmpindex + 1] = invert.Apply(bmps[bmpindex]);
            bmpindex++;

            Closing closing = new Closing();
            bmps[bmpindex + 1] = closing.Apply(bmps[bmpindex]);
            bmpindex++;

            ConnectedComponentsLabeling clabeling = new ConnectedComponentsLabeling();
            bmps[bmpindex + 1] = clabeling.Apply(bmps[bmpindex]);
            bmpindex++;

            SaveAllbmps(bmps, "NEW-");

            bmpFirst = CopyImage(bmps[bmpindex]);

            Clearbmps(bmps);
        }


        public void GetDataGX(Bitmap bmporg,
            double gammacor, int blurcount, double holeratio, bool isneedclose
            , ComboBox cbo)
        {
            int i = 0;
            int bmpindex = 0;

            Bitmap[] bmps = new Bitmap[200];

            if (bmpOrg != null)
                bmpOrg.Dispose();
            if (bmpFirst != null)
                bmpFirst.Dispose();
            if (bmpSecond != null)
                bmpSecond.Dispose();
            if (bmpThird != null)
                bmpThird.Dispose();

            bmpOrg = CopyImage(bmporg);

            HistogramEqualization histogramEqualization = new HistogramEqualization();
            Blur blur = new Blur();
            ContrastStretch contraststrch = new ContrastStretch();

            //做histogram euqlization 和 blur 的循環處理
            i = 0;

            bmps[bmpindex] = CopyImage(bmporg);

            while (i < blurcount)
            {
                bmps[bmpindex + 1] = histogramEqualization.Apply(bmps[bmpindex]);
                bmpindex++;

                bmps[bmpindex + 1] = blur.Apply(bmps[bmpindex]);
                bmpindex++;

                i++;
            }

            bmps[bmpindex + 1] = ConvertToGrayScale(bmps[bmpindex]);
            bmpindex++;

            GammaCorrection gammaCorrection = new GammaCorrection();
            gammaCorrection.Gamma = gammacor;
            bmps[bmpindex + 1] = gammaCorrection.Apply(bmps[bmpindex]);
            bmpindex++;

            SISThreshold sis = new SISThreshold();
            bmps[bmpindex + 1] = sis.Apply(bmps[bmpindex]);
            bmpindex++;

            //CannyEdgeDetector canny = new CannyEdgeDetector(20, 100, 1.4d);
            //bmps[bmpindex + 1] = canny.Apply(bmps[bmpindex]);
            //bmpindex++;
            DifferenceEdgeDetector diff = new DifferenceEdgeDetector();
            bmps[bmpindex + 1] = diff.Apply(bmps[bmpindex]);
            bmpindex++;

            bmps[bmpindex + 1] = sis.Apply(bmps[bmpindex]);
            bmpindex++;

            Invert invert = new Invert();
            bmps[bmpindex + 1] = invert.Apply(bmps[bmpindex]);
            bmpindex++;

            FillHoles fillHoles = new FillHoles();
            fillHoles.CoupledSizeFiltering = true;
            fillHoles.MaxHoleWidth = (int)(bmporg.Width * holeratio / 100d);
            fillHoles.MaxHoleHeight = (int)(bmporg.Height * holeratio / 100d);

            bmps[bmpindex + 1] = fillHoles.Apply(bmps[bmpindex]);
            bmpindex++;

            bmps[bmpindex + 1] = invert.Apply(bmps[bmpindex]);
            bmpindex++;

            if (isneedclose)
            {
                Closing closing = new Closing();
                bmps[bmpindex + 1] = closing.Apply(bmps[bmpindex]);
                bmpindex++;
            }

            ConnectedComponentsLabeling clabeling = new ConnectedComponentsLabeling();
            bmps[bmpindex + 1] = clabeling.Apply(bmps[bmpindex]);
            bmpindex++;

            string filenames = "";
            string firstfilename = @"D:\\JETEAZY\\FIRST.BMP";

            bmpFirst = CopyImage(bmps[bmpindex]);
            SaveAllbmps(bmps, "NEW-", ref filenames);

            bmpFirst.Save(firstfilename, ImageFormat.Bmp);
            filenames = firstfilename + "," + filenames.Remove(filenames.Length - 1, 1);

            string[] fnames = filenames.Split(',');
            cbo.Items.Clear();

            foreach (string fname in fnames)
            {
                cbo.Items.Add(fname);
            }

            cbo.SelectedIndex = 0;

            Clearbmps(bmps);
        }

        public void GetDataHX(Bitmap bmporg,
            double gammacor, int blurcount, double holeratio, bool isneedclose
            , ComboBox cbo)
        {
            int i = 0;
            int bmpindex = 0;

            Bitmap[] bmps = new Bitmap[200];

            if (bmpOrg != null)
                bmpOrg.Dispose();
            if (bmpFirst != null)
                bmpFirst.Dispose();
            if (bmpSecond != null)
                bmpSecond.Dispose();
            if (bmpThird != null)
                bmpThird.Dispose();

            bmpOrg = CopyImage(bmporg);

            HistogramEqualization histogramEqualization = new HistogramEqualization();
            Blur blur = new Blur();
            ContrastStretch contraststrch = new ContrastStretch();

            //做histogram euqlization 和 blur 的循環處理
            i = 0;

            bmps[bmpindex] = CopyImage(bmporg);

            bmps[bmpindex + 1] = ConvertToGrayScale(bmps[bmpindex]);
            bmpindex++;

            while (i < blurcount)
            {
                bmps[bmpindex + 1] = blur.Apply(bmps[bmpindex]);
                bmpindex++;

                bmps[bmpindex + 1] = histogramEqualization.Apply(bmps[bmpindex]);
                bmpindex++;

                i++;
            }


            //GammaCorrection gammaCorrection = new GammaCorrection();
            //gammaCorrection.Gamma = gammacor;
            //bmps[bmpindex + 1] = gammaCorrection.Apply(bmps[bmpindex]);
            //bmpindex++;

            SISThreshold sis = new SISThreshold();
            bmps[bmpindex + 1] = sis.Apply(bmps[bmpindex]);
            bmpindex++;

            CannyEdgeDetector canny = new CannyEdgeDetector(20, 100, 1.4d);
            bmps[bmpindex + 1] = canny.Apply(bmps[bmpindex]);
            bmpindex++;

            bmps[bmpindex + 1] = sis.Apply(bmps[bmpindex]);
            bmpindex++;

            Invert invert = new Invert();
            bmps[bmpindex + 1] = invert.Apply(bmps[bmpindex]);
            bmpindex++;

            FillHoles fillHoles = new FillHoles();
            fillHoles.CoupledSizeFiltering = true;
            fillHoles.MaxHoleWidth = (int)(bmporg.Width * holeratio / 100d);
            fillHoles.MaxHoleHeight = (int)(bmporg.Height * holeratio / 100d);

            bmps[bmpindex + 1] = fillHoles.Apply(bmps[bmpindex]);
            bmpindex++;

            bmps[bmpindex + 1] = invert.Apply(bmps[bmpindex]);
            bmpindex++;

            if (isneedclose)
            {
                Closing closing = new Closing();
                bmps[bmpindex + 1] = closing.Apply(bmps[bmpindex]);
                bmpindex++;
            }

            ConnectedComponentsLabeling clabeling = new ConnectedComponentsLabeling();
            bmps[bmpindex + 1] = clabeling.Apply(bmps[bmpindex]);
            bmpindex++;

            string filenames = "";
            string firstfilename = @"D:\\JETEAZY\\FIRST.BMP";

            bmpFirst = CopyImage(bmps[bmpindex]);
            SaveAllbmps(bmps, "NEW-", ref filenames);

            bmpFirst.Save(firstfilename, ImageFormat.Bmp);
            filenames = firstfilename + "," + filenames.Remove(filenames.Length - 1, 1);

            string[] fnames = filenames.Split(',');
            cbo.Items.Clear();

            foreach (string fname in fnames)
            {
                cbo.Items.Add(fname);
            }

            cbo.SelectedIndex = 0;

            Clearbmps(bmps);
        }

        /// <summary>
        /// 萬子的超級大晶粒
        /// </summary>
        /// <param name="bmporg"></param>
        /// <param name="gammacor"></param>
        /// <param name="blurcount"></param>
        /// <param name="holeratio"></param>
        /// <param name="isneedclose"></param>
        /// <param name="cbo"></param>
        /// <param name="sidecount"></param>
        /// <param name="longratio"></param>
        /// <param name="shortratio"></param>
        /// <param name="enlarge"></param>
        /// <param name="insidecolor"></param>
        public void GetDataIX(Bitmap bmporg,
            double gammacor, int blurcount, double holeratio, bool isneedclose
            , ComboBox cbo, int[] sidecount
            , double longratio, double shortratio, int enlarge, byte insidecolor)
        {
            int i = 0;
            int bmpindex = 0;

            Bitmap[] bmps = new Bitmap[200];

            if (bmpOrg != null)
                bmpOrg.Dispose();
            if (bmpFirst != null)
                bmpFirst.Dispose();
            if (bmpSecond != null)
                bmpSecond.Dispose();
            if (bmpThird != null)
                bmpThird.Dispose();

            bmpOrg = CopyImage(bmporg);

            //HistogramEqualization histogramEqualization = new HistogramEqualization();
            //Blur blur = new Blur();
            //ContrastStretch contraststrch = new ContrastStretch();

            i = 0;

            bmps[bmpindex] = CopyImage(bmporg);

            bmps[bmpindex + 1] = ConvertToGrayScale(bmps[bmpindex]);
            bmpindex++;

            //GammaCorrection gammaCorrection = new GammaCorrection();
            //gammaCorrection.Gamma = gammacor;
            //bmps[bmpindex + 1] = gammaCorrection.Apply(bmps[bmpindex]);
            //bmpindex++;

            //GetBoarderIX(bmps[bmpindex], 0.6d,0.9d, sidecount,20);
            GetBoarderIX(bmps[bmpindex], longratio, shortratio, sidecount, enlarge, holeratio, insidecolor);

            string filenames = "";
            string firstfilename = @"D:\\JETEAZY\\FIRST.BMP";

            bmpFirst = CopyImage(bmps[bmpindex]);
            //SaveAllbmps(bmps, "NEW-", ref filenames);

            //bmpFirst.Save(firstfilename, ImageFormat.Bmp);
            //filenames = firstfilename + "," + filenames.Remove(filenames.Length - 1, 1);

            //string[] fnames = filenames.Split(',');
            //cbo.Items.Clear();

            //foreach (string fname in fnames)
            //{
            //    cbo.Items.Add(fname);
            //}

            //cbo.SelectedIndex = 0;

            Clearbmps(bmps);
        }

        public void GetBoarder(Bitmap bmp, double rangeratio, int[] sidecount)
        {
            Rectangle[] smallrects = new Rectangle[100];
            Rectangle bigrect = GetAllRects(bmp, ref smallrects);

            int i = 0;

            SuiccideSideData();

            while (i < (int)SIDEEmnum.COUNT)
            {
                SideDataClass sidedata = new SideDataClass();
                sidedata.GetSide((SIDEEmnum)i, bmpFirst, bigrect, rangeratio);
                sidedata.mysideCount = sidecount[i];
                sidedata.Process(sidedata.bmpside);

                SIDEData[i] = sidedata;
                i++;
            }
        }

        public void GetBoarderIX(Bitmap bmp, double longratio, double shortratio,
            int[] sidecount, int enlarge, double holesratio, byte insidecolor)
        {
            Rectangle[] smallrects = new Rectangle[100];
            Rectangle bigrect = GetAllRectsIX(bmp, 500);

            Rectangle bigrectext = bigrect;
            bigrectext.Inflate(enlarge, enlarge);

            FillRect8bit(bmp, bigrectext, insidecolor);

            int i = 0;

            SuiccideSideData();

            while (i < (int)SIDEEmnum.COUNT)
            {
                SideDataClass sidedata = new SideDataClass();
                sidedata.GetSide((SIDEEmnum)i, bmp, bigrect, longratio, (1 - shortratio));
                sidedata.mysideCount = sidecount[i];
                sidedata.ProcessIX(sidedata.bmpside, holesratio);

                SIDEData[i] = sidedata;
                i++;
            }
        }

        void FillRect8bit(Bitmap bmp, Rectangle rect, byte color8bit)
        {
            Rectangle rectbmp = new Rectangle(new Point(0, 0), bmp.Size);

            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

            IntPtr Scan0 = bmpData.Scan0;

            try
            {
                unsafe
                {
                    byte* scan0 = (byte*)(void*)Scan0;
                    byte* pucPtr;
                    byte* pucStart;

                    int xmin = rect.X;
                    int ymin = rect.Y;

                    int xmax = rect.Right - 1;
                    int ymax = rect.Bottom - 1;

                    int x = xmin;
                    int y = ymin;

                    int iStride = bmpData.Stride;

                    y = ymin;
                    pucStart = scan0 + (x - xmin) + (iStride * (y - ymin));

                    while (y < ymax)
                    {
                        x = xmin;
                        pucPtr = pucStart;

                        while (x < xmax)
                        {
                            *pucPtr = color8bit;

                            pucPtr++;
                            x++;
                        }
                        pucStart += iStride;
                        y++;
                    }
                    bmp.UnlockBits(bmpData);
                }
            }
            catch (Exception ex)
            {
                bmp.UnlockBits(bmpData);
            }
        }

        public void SaveAllbmps(Bitmap[] bmps, string headstr)
        {
            string filenames = "";
            SaveAllbmps(bmps, headstr, ref filenames);
        }
        public void SaveAllbmps(Bitmap[] bmps, string headstr, ref string filenames)
        {
            int i = 0;

            while (i < bmps.Length)
            {
                if (bmps[i] != null)
                {
                    string filename = @"D:\JETEAZY\" + headstr + i.ToString("00") + ".BMP";

                    bmps[i].Save(filename, ImageFormat.Bmp);

                    if (filenames != "")
                        filenames += filename + ",";
                }
                i++;
            }
        }
        public void GetDietBMP(Bitmap bmpsrc, double deitrangeratio)
        {
            Bitmap[] bmps = new Bitmap[20];

            int i = 0;
            int bmpindex = 0;

            //先找到大白白晶片位置
            bmps[bmpindex] = CopyImage(bmpsrc);
            bmps[bmpindex + 1] = ConvertToGrayScale(bmps[bmpindex]);
            bmpindex++;

            OtsuThreshold otsu = new OtsuThreshold();
            bmps[bmpindex + 1] = otsu.Apply(bmps[bmpindex]);
            bmpindex++;

            BlobCounter blobCounter = new BlobCounter();
            blobCounter.ProcessImage(bmps[bmpindex]);
            Blob[] blobs = blobCounter.GetObjectsInformation();

            Rectangle bigrect = new Rectangle();

            int area = -1000;
            i = 0;
            while (i < blobCounter.ObjectsCount)
            {
                if (area < blobs[i].Area)
                {
                    area = blobs[i].Area;
                    bigrect = blobs[i].Rectangle;
                }
                i++;
            }

            SaveAllbmps(bmps, "DEIT");
            //Check Rectangle 

            int VW = (bigrect.Width - (int)((double)bigrect.Width * deitrangeratio)) / 2;
            int VL = bigrect.X + VW;
            int VR = bigrect.Right - VW;

            Rectangle vlRect = new Rectangle(0, 0, VL, bmpsrc.Height);
            Rectangle vrRect = new Rectangle(VR, 0, bmpsrc.Width - VR - 1, bmpsrc.Height);

            int HH = (bigrect.Height - (int)((double)bigrect.Height * deitrangeratio)) / 2;
            int HT = bigrect.Y + HH;
            int HB = bigrect.Bottom - HH;

            Rectangle htRect = new Rectangle(0, 0, bmpsrc.Width, HT);
            Rectangle hbRect = new Rectangle(0, HB, bmpsrc.Width, bmpsrc.Height - HB - 1);

            Bitmap vBMP = new Bitmap(vlRect.Width + vrRect.Width, vlRect.Height);
            Bitmap hBMP = new Bitmap(htRect.Width, htRect.Height + hbRect.Height);

            DrawCompactImage(bmpsrc, vlRect, vrRect, vBMP, false);
            DrawCompactImage(bmpsrc, htRect, hbRect, hBMP, true);

            vBMP.Save(@"D:\JETEAZY\VCOMPACT.BMP", ImageFormat.Bmp);
            hBMP.Save(@"D:\JETEAZY\HCOMPACT.BMP", ImageFormat.Bmp);

            Clearbmps(bmps);
        }

        public string meanstr = "";
        public string thresholdstr = "";
        public string PASSNG = "PASS";

        public void GetIPDDietBMP(Bitmap bmpsrc,
            short rgb,
            double thresholdratio,
            double objectfilterratio,
            int edcount,
            double shortenratio,
            string bangbangstr)
        {
            Bitmap[] bmps = new Bitmap[30];

            int i = 0;
            int bmpindex = 0;

            int[] histogramdata = new int[256];
            JzHistogramClass jzHistogram = new JzHistogramClass(1);
            JzHistogramResult jzhresult = new JzHistogramResult();

            FillHoles fillholes = new FillHoles();
            CannyEdgeDetector canny = new CannyEdgeDetector();
            SISThreshold sis = new SISThreshold();
            OtsuThreshold otsu = new OtsuThreshold();
            Erosion erosion = new Erosion();
            Dilatation dilatation = new Dilatation();
            BlobCounter blobCounter = new BlobCounter();
            Opening opening = new Opening();
            Threshold threshold = new Threshold();
            ContrastStretch contrastStretch = new ContrastStretch();
            Invert invert = new Invert();
            Rectangle rectBangBang = new Rectangle();

            PASSNG = "PASS";

            bmpOrg = CopyImage(bmpsrc);

            //先取出綠色的Channel
            bmps[bmpindex] = CopyImage(bmpsrc);

            //如果有𣗘𣗘，就把棒棒畫上去
            if (bangbangstr != "")
            {
                string[] rectstrs = bangbangstr.Split(';');
                rectBangBang = new Rectangle(int.Parse(rectstrs[0]),
                                             int.Parse(rectstrs[1]),
                                             int.Parse(rectstrs[2]),
                                             int.Parse(rectstrs[3]));

                Graphics g = Graphics.FromImage(bmps[bmpindex]);
                g.FillRectangle(new SolidBrush(Color.Black), rectBangBang);
                g.Dispose();
            }
            bmps[bmpindex + 1] = GetChannel(bmps[bmpindex], rgb);
            bmpindex++;

            SaveAllbmps(bmps, "PRE");


            if (bangbangstr != "")
                ProcessBangBang(bmps[bmpindex], thresholdratio, objectfilterratio, edcount, shortenratio, rectBangBang);
        }

        void ProcessBangBang(Bitmap bmp,
             double thresholdratio,
             double objectfilterratio,
             int edcount,
             double shortenratio,
             Rectangle bangbangrect)
        {
            Bitmap[] bmps = new Bitmap[30];

            int i = 0;
            int bmpindex = 0;

            int[] histogramdata = new int[256];
            JzHistogramClass jzHistogram = new JzHistogramClass(1);
            JzHistogramResult jzhresult = new JzHistogramResult();

            FillHoles fillholes = new FillHoles();
            CannyEdgeDetector canny = new CannyEdgeDetector();
            SISThreshold sis = new SISThreshold();
            OtsuThreshold otsu = new OtsuThreshold();
            Erosion erosion = new Erosion();
            Dilatation dilatation = new Dilatation();
            BlobCounter blobCounter = new BlobCounter();
            Opening opening = new Opening();
            Closing closing = new Closing();
            Threshold threshold = new Threshold();
            ContrastStretch contrastStretch = new ContrastStretch();
            Invert invert = new Invert();

            //Step 1: 先把棒棒、晶片和可見膠分出來
            //otsu 處理
            bmps[bmpindex] = CopyImage(bmp);
            bmps[bmpindex + 1] = otsu.Apply(bmps[bmpindex]);
            bmpindex++;
            //bmps[bmpindex].Save(@"D:\JETEAZY\FUCK.PNG", ImageFormat.Png);

            int CheckChipSizeIndex = bmpindex;

            //畫外框白色
            DrawRectOutLine(bmps[bmpindex], 255);
            //bmps[bmpindex].Save(@"D:\JETEAZY\FUCK1.PNG", ImageFormat.Png);
            //int WhiteOutlineIndex = bmpindex;

            //外蝕
            i = 0;
            while (i < edcount)
            {
                bmps[bmpindex + 1] = dilatation.Apply(bmps[bmpindex]);
                bmpindex++;
                i++;
            }

            //消除小雜毛邊
            bmps[bmpindex + 1] = closing.Apply(bmps[bmpindex]);
            bmpindex++;

            //清除雜訊
            fillholes.CoupledSizeFiltering = true;//true and false or
            fillholes.MaxHoleWidth = (int)(bmpOrg.Width * objectfilterratio);
            fillholes.MaxHoleHeight = (int)(bmpOrg.Height * objectfilterratio);
            bmps[bmpindex + 1] = fillholes.Apply(bmps[bmpindex]);
            bmpindex++;

            //外擴
            i = 0;
            while (i < edcount)
            {
                bmps[bmpindex + 1] = erosion.Apply(bmps[bmpindex]);
                bmpindex++;
                i++;
            }

            //尋外框
            canny.GaussianSigma = 1.4;
            canny.HighThreshold = 100;
            canny.LowThreshold = 20;
            bmps[bmpindex + 1] = canny.Apply(bmps[bmpindex]);
            bmpindex++;

            //加強
            bmps[bmpindex + 1] = sis.Apply(bmps[bmpindex]);
            bmpindex++;

            //加強
            bmps[bmpindex + 1] = closing.Apply(bmps[bmpindex]);
            bmpindex++;

            bmpFirst = CopyImage(bmps[bmpindex]);

            //取出所有的物件
            blobCounter.ProcessImage(bmps[bmpindex]);
            Blob[] blobs = blobCounter.GetObjectsInformation();

            List<string> sortlist = new List<string>();
            i = 0;
            while (i < blobCounter.ObjectsCount)
            {
                string str = blobs[i].Area.ToString("000000000000") + "," + i.ToString("000");
                sortlist.Add(str);

                i++;
            }

            sortlist.Sort();
            sortlist.Reverse();

            //檢查相交的方形尺吋是否比原來的大，大了就代表溢出
            //STEP1 先確認bangbang 是上下還是左右的bangbang
            SIDEEmnum bangbangside = SIDEEmnum.TOP;

            if (bangbangrect.Width > bangbangrect.Height) //棒棒是否位於上下左右位置
            {
                if (bangbangrect.Y < (bmp.Height >> 1))
                    bangbangside = SIDEEmnum.TOP;
                else
                    bangbangside = SIDEEmnum.BOTTOM;
            }
            else
            {
                if (bangbangrect.X < (bmp.Width >> 1))
                    bangbangside = SIDEEmnum.LEFT;
                else
                    bangbangside = SIDEEmnum.RIGHT;
            }

            //檢查blob的位置是否比一開始定義的blob位置要上下左右距離大，只要更大就是NG
            Rectangle foundbanbangrect = new Rectangle();
            foreach (Blob foundblob in blobs)
            {
                if (foundblob.Rectangle.IntersectsWith(bangbangrect))
                {
                    foundbanbangrect = foundblob.Rectangle;
                }
            }

            int NGRange = 4;
            switch (bangbangside)
            {
                case SIDEEmnum.TOP:
                    if (bangbangrect.Top - foundbanbangrect.Top > NGRange)
                    {
                        PASSNG = "NG";
                    }
                    break;
                case SIDEEmnum.BOTTOM:
                    if (foundbanbangrect.Bottom - bangbangrect.Bottom > NGRange)
                    {
                        PASSNG = "NG";
                    }
                    break;
                case SIDEEmnum.LEFT:
                    if (bangbangrect.Left - foundbanbangrect.Left > NGRange)
                    {
                        PASSNG = "NG";
                    }
                    break;
                case SIDEEmnum.RIGHT:
                    if (foundbanbangrect.Right - bangbangrect.Right > NGRange)
                    {
                        PASSNG = "NG";
                    }
                    break;
            }

            //排名第2的是晶片 //NG!!! //REVISED By VICTOR 2025/03/11

            blobCounter.ProcessImage(bmps[CheckChipSizeIndex]);
            Blob[] chckchipblobs = blobCounter.GetObjectsInformation();

            i = 0;
            Rectangle bmpcenterrect = new Rectangle(bmpOrg.Width >> 1, bmpOrg.Height >> 1, 10, 10);

            foreach (Blob bb in chckchipblobs)
            {
                if (bb.Rectangle.IntersectsWith(bmpcenterrect))
                {
                    Rectangle orgrect = bb.Rectangle;
                    Rectangle bmprect = new Rectangle(0, 0, bmp.Width, bmp.Height);

                    orgrect.Inflate(20, 20);
                    orgrect.Intersect(bmprect);

                    if (orgrect != bmprect)
                    {
                        break;
                    }
                }

                i++;
            }


            Blob chipblob = chckchipblobs[i];
            //取得晶片的四方型
            Rectangle chiprect = chipblob.Rectangle;

            int swidth = (chiprect.Width - (int)(chiprect.Width * shortenratio)) / 2;
            int sheight = (chiprect.Height - (int)(chiprect.Height * shortenratio)) / 2;

            chiprect.Inflate(-swidth, -sheight);

            i = 0;

            SuiccideSideData();

            while (i < (int)SIDEEmnum.COUNT)
            {
                SideDataClass sidedata = new SideDataClass();
                sidedata.GetSideEX((SIDEEmnum)i, bmpFirst, chiprect);
                //bmpFirst.Save("D:\\JETEAZY\\SIDEXXX" + i.ToString("00") + ".png", ImageFormat.Png);
                sidedata.ProcessEX();
                //sidedata.bmpside.Save("D:\\JETEAZY\\SIDEPROCESSXXX" + i.ToString("00") + ".png", ImageFormat.Png);
                SIDEData[i] = sidedata;
                i++;
            }

            if (bmpSecond == null)
                bmpSecond = new Bitmap(1, 1);
            //畫出結果圖
            bmpSecond.Dispose();
            bmpSecond = (Bitmap)bmpOrg.Clone(new Rectangle(0, 0, bmpOrg.Width, bmpOrg.Height), PixelFormat.Format32bppArgb);
            foreach (SideDataClass sidedata in SIDEData)
            {
                sidedata.DrawResultEX(bmpSecond, Color.Red, 3);
            }

            //bmpSecond.Save("D:\\JETEAZY\\COMBINE.png", ImageFormat.Png);

            //往後是檢測棒棒是否在整團外圈內
            //確認是否透明膠有黏住棒棒
            //Step 2: 先把整個範圍找出來

            //Thrshold 處理
            jzHistogram.GetHistogramData(bmp,
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                histogramdata);
            GetHistogramData(histogramdata, jzhresult);

            meanstr = jzhresult.mean.ToString("0.000");
            thresholdstr = ((int)(jzhresult.mean * (1 - thresholdratio))).ToString();

            threshold.ThresholdValue = (int)(jzhresult.mean * (1 - thresholdratio));
            bmps[bmpindex + 1] = threshold.Apply(bmp);
            bmpindex++;

            //bmps[bmpindex].Save(@"D:\JETEAZY\FUCK.PNG", ImageFormat.Png);

            //畫外框白色
            DrawRectOutLine(bmps[bmpindex], 255);

            bmps[bmpindex + 1] = fillholes.Apply(bmps[bmpindex]);
            bmpindex++;

            //確認是否透明膠有黏住棒棒，先找外框
            bmps[bmpindex + 1] = canny.Apply(bmps[bmpindex]);
            bmpindex++;

            //強化外框
            bmps[bmpindex + 1] = sis.Apply(bmps[bmpindex]);
            bmpindex++;
            //反白
            bmps[bmpindex + 1] = invert.Apply(bmps[bmpindex]);
            bmpindex++;
            //連結線
            bmps[bmpindex + 1] = opening.Apply(bmps[bmpindex]);
            bmpindex++;
            //清除雜訊
            bmps[bmpindex + 1] = fillholes.Apply(bmps[bmpindex]);
            bmpindex++;
            //反白
            bmps[bmpindex + 1] = invert.Apply(bmps[bmpindex]);
            bmpindex++;

            if (bmpThird == null)
                bmpThird = new Bitmap(1, 1);
            //畫出結果圖
            bmpThird.Dispose();
            bmpThird = CopyImage(bmps[bmpindex]);

            //bmpThird.Save("D:\\JETEAZY\\THIRD.png", ImageFormat.Png);

            //取得最大範圍的外圍
            blobCounter.ProcessImage(bmps[bmpindex]);
            Blob[] Rangeblobs = blobCounter.GetObjectsInformation();

            sortlist.Clear();
            i = 0;
            while (i < blobCounter.ObjectsCount)
            {
                string str = Rangeblobs[i].Area.ToString("000000000000") + "," + i.ToString("000");
                sortlist.Add(str);

                i++;
            }

            sortlist.Sort();
            sortlist.Reverse();


            //排名第1大的是最外圍，並檢查blobs所有的blob是否都收編全範圍，是的話就表示有透明膠
            Blob Allblob = Rangeblobs[int.Parse(sortlist[0].Split(',')[1])];

            Rectangle AllblobRect = Allblob.Rectangle;
            AllblobRect.Inflate(10, 10);

            bool isallinside = true;
            foreach (Blob blob in blobs)
            {
                Rectangle blbrect = blob.Rectangle;

                blbrect.Intersect(AllblobRect);

                if (blbrect != blob.Rectangle)
                {
                    isallinside = false;
                    break;
                }
            }
            //有透明膠才會檢查資料的距離
            if (isallinside)
            {
                allresultstr = "";

                foreach (SideDataClass sidedata in SIDEData)
                {
                    sidedata.ProcessDistance(ref allresultstr);
                }
            }

            DrawText(bmpSecond, allresultstr);

            bmpSecond.Save("D:\\JETEAZY\\COMBINE.png", ImageFormat.Png);


            SaveAllbmps(bmps, "BANGBANG");
            Clearbmps(bmps);
        }

        void DrawText(Bitmap bmp, string str)
        {
            Graphics g = Graphics.FromImage(bmp);
            g.DrawString(str, new Font("Arial", 8), new SolidBrush(Color.Red), new PointF(0, 0));
            g.Dispose();

        }

        Rectangle GetAllRects(Bitmap bmpsrc, ref Rectangle[] ballrects)
        {
            Rectangle bigrect = new Rectangle();

            Bitmap[] bmps = new Bitmap[20];

            int i = 0;
            int bmpindex = 0;

            //先找到大白白晶片位置
            bmps[bmpindex] = CopyImage(bmpsrc);
            bmps[bmpindex + 1] = ConvertToGrayScale(bmps[bmpindex]);
            bmpindex++;

            OtsuThreshold otsu = new OtsuThreshold();
            bmps[bmpindex + 1] = otsu.Apply(bmps[bmpindex]);
            bmpindex++;

            BlobCounter blobCounter = new BlobCounter();
            blobCounter.ProcessImage(bmps[bmpindex]);
            Blob[] blobs = blobCounter.GetObjectsInformation();

            int area = -1000;
            i = 0;
            while (i < blobCounter.ObjectsCount)
            {
                if (area < blobs[i].Area)
                {
                    area = blobs[i].Area;
                    bigrect = blobs[i].Rectangle;
                }
                i++;
            }

            i = 0;
            int index = 0;
            ballrects = new Rectangle[blobCounter.ObjectsCount - 1];

            while (i < blobCounter.ObjectsCount)
            {
                if (bigrect != blobs[i].Rectangle)
                {
                    ballrects[index] = blobs[i].Rectangle;
                    index++;
                }
                i++;
            }

            //SaveAllbmps(bmps, "DEIT");
            Clearbmps(bmps);

            return bigrect;
        }

        Rectangle GetAllRectsIX(Bitmap bmpsrc, int mincombinwh)
        {
            Rectangle bigrect = new Rectangle();

            Bitmap[] bmps = new Bitmap[20];

            int i = 0;
            int bmpindex = 0;

            //先找到大白白晶片位置
            bmps[bmpindex] = CopyImage(bmpsrc);
            //bmps[bmpindex + 1] = ConvertToGrayScale(bmps[bmpindex]);
            //bmpindex++;

            OtsuThreshold otsu = new OtsuThreshold();
            bmps[bmpindex + 1] = otsu.Apply(bmps[bmpindex]);
            bmpindex++;

            BlobCounter blobCounter = new BlobCounter();
            blobCounter.ProcessImage(bmps[bmpindex]);
            Blob[] blobs = blobCounter.GetObjectsInformation();

            i = 0;
            while (i < blobCounter.ObjectsCount)
            {
                if (blobs[i].Rectangle.Width > mincombinwh && blobs[i].Rectangle.Height > mincombinwh)
                {
                    if (bigrect.X == 0)
                    {
                        bigrect = blobs[i].Rectangle;
                    }
                    else
                    {
                        bigrect = Rectangle.Union(bigrect, blobs[i].Rectangle);
                    }
                }
                i++;
            }

            //SaveAllbmps(bmps, "DEIT");
            Clearbmps(bmps);

            return bigrect;
        }
        public void DrawCompactImage(Bitmap bmporg, Rectangle rect1, Rectangle rect2, Bitmap bmpdest, bool isverticle)
        {
            Graphics g = Graphics.FromImage(bmpdest);

            Rectangle srcRect1 = new Rectangle(new Point(0, 0), rect1.Size);

            g.DrawImage(bmporg, rect1, srcRect1, GraphicsUnit.Pixel);

            Rectangle srcRect2 = new Rectangle();

            if (!isverticle)
                srcRect2 = new Rectangle(new Point(srcRect1.Right, 0), rect2.Size);
            else
                srcRect2 = new Rectangle(new Point(0, srcRect1.Bottom), rect2.Size);

            g.DrawImage(bmporg, srcRect2, rect2, GraphicsUnit.Pixel);

            g.Dispose();
        }

        /// <summary>
        /// 第二階段，把四邊的部份圖切出來
        /// </summary>
        public void SplitSides()
        {
            int i = 0;

            SuiccideSideData();

            while (i < (int)SIDEEmnum.COUNT)
            {
                SideDataClass sidedata = new SideDataClass();
                sidedata.GetSide((SIDEEmnum)i, bmpFirst, 0.6d, 0.1d);

                SIDEData[i] = sidedata;

                i++;
            }
        }
        /// <summary>
        /// 第三階段，把四邊部份的圖作演算法並且存入每個部份
        /// </summary>
        /// <param name="methods"></param>
        public void ProcessSides(SecondStepEnum[] methods)
        {
            int i = 0;

            if (bmpSecond != null)
                bmpSecond.Dispose();

            //bmpSecond = CopyImage(bmpFirst);

            while (i < (int)SIDEEmnum.COUNT)
            {
                SIDEData[i].Process(methods[i]);
                i++;
            }
        }
        /// <summary>
        /// 第四階段，把四邊找出來的點位畫出來。
        /// </summary>

        public void DrawProcessedSides()
        {
            DrawProcessedSides(Color.Lime, 3);
        }

        public void DrawProcessedSides(Color color, int width)
        {
            int i = 0;

            if (bmpSecond != null)
                bmpSecond.Dispose();

            bmpSecond = CopyImage(bmpOrg);

            while (i < (int)SIDEEmnum.COUNT)
            {
                SIDEData[i].DrawResult(bmpSecond, color, width, true);

                i++;
            }

            bmpSecond.Save(@"D:\JETEAZY\DRAW.bmp", ImageFormat.Bmp);
        }
        public void SaveAllBmp()
        {
            int i = 0;

            //bmpOrg.Save(@"D:\JETEAZY\ORG.bmp", ImageFormat.Bmp);
            //bmpFirst.Save(@"D:\JETEAZY\FIRST.bmp", ImageFormat.Bmp);

            while (i < (int)SIDEEmnum.COUNT)
            {
                SIDEData[i].bmpside.Save(@"D:\JETEAZY\S" + ((SIDEEmnum)i).ToString() + ".bmp", ImageFormat.Bmp);

                i++;
            }
        }
        Bitmap ConvertToGrayScale(Bitmap bmp)
        {
            return AForge.Imaging.Filters.Grayscale.CommonAlgorithms.BT709.Apply(bmp);
        }
        Bitmap GetChannel(Bitmap bmp, short rgb)
        {
            ExtractChannel extract = new ExtractChannel();
            extract.Channel = rgb;

            return extract.Apply(bmp);
        }
        public void GetHistogramData(int[] histogramdata)
        {
            int num = histogramdata.Length;
            int max = 0;
            int min = num;
            long total = 0L;
            for (int i = 0; i < num; i++)
            {
                if (histogramdata[i] != 0)
                {
                    if (i > max)
                    {
                        max = i;
                    }

                    if (i < min)
                    {
                        min = i;
                    }

                    total += histogramdata[i];
                }
            }

            double mean = Statistics.Mean(histogramdata);
            double stdDev = Statistics.StdDev(histogramdata, mean);
            double median = Statistics.Median(histogramdata);
        }
        public void GetHistogramData(int[] histogramdata, JzHistogramResult jzhresult)
        {
            int num = histogramdata.Length;
            int max = 0;
            int min = num;
            long total = 0L;
            for (int i = 0; i < num; i++)
            {
                if (histogramdata[i] != 0)
                {
                    if (i > max)
                    {
                        max = i;
                    }

                    if (i < min)
                    {
                        min = i;
                    }

                    total += histogramdata[i];
                }
            }

            jzhresult.min = min;
            jzhresult.max = max;
            jzhresult.count = num;
            jzhresult.total = total;

            jzhresult.mean = Statistics.Mean(histogramdata);
            jzhresult.stddev = Statistics.StdDev(histogramdata, jzhresult.mean);
            jzhresult.median = Statistics.Median(histogramdata);
            jzhresult.mode = Statistics.Mode(histogramdata);

        }
        public void Suiccide()
        {
            if (bmpOrg != null)
                bmpOrg.Dispose();
            if (bmpFirst != null)
                bmpFirst.Dispose();
            if (bmpSecond != null)
                bmpSecond.Dispose();
            if (bmpThird != null)
                bmpThird.Dispose();
        }
        void SuiccideSideData()
        {
            int i = 0;

            while (i < (int)SIDEEmnum.COUNT)
            {
                if (SIDEData[i] != null)
                {
                    SIDEData[i].Suiccied();
                }
                i++;
            }
        }
        Bitmap CopyImage(Bitmap bmp)
        {
            return CopyImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
        }
        Bitmap CopyImage(Bitmap bmp, Rectangle rect)
        {
            return (Bitmap)bmp.Clone(rect, bmp.PixelFormat);
        }
        void DrawRectOutside(Bitmap bmp, Color color, int linewidth)
        {
            Graphics g = Graphics.FromImage(bmp);
            Rectangle rect = new Rectangle(0, 0, bmp.Width - 1, bmp.Height - 1);

            g.DrawRectangle(new Pen(Color.Red, linewidth), rect);
            g.Dispose();
        }
        void DrawRectOutLine(Bitmap bmp, byte color)
        {
            Rectangle rectbmp = new Rectangle(0, 0, bmp.Width, bmp.Height);
            DrawRectOutLine(bmp, color, rectbmp);
        }

        void DrawRectOutLine(Bitmap bmp, byte color, Rectangle rect)
        {
            Rectangle recttmp = rect;
            BitmapData bmpData = bmp.LockBits(recttmp, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            IntPtr Scan0 = bmpData.Scan0;

            try
            {
                unsafe
                {
                    byte* scan0 = (byte*)(void*)Scan0;
                    byte* pucPtr;
                    byte* pucStart;

                    int xmin = recttmp.Left;
                    int ymin = recttmp.Top;

                    int xmax = recttmp.Right;
                    int ymax = recttmp.Bottom;

                    int x = recttmp.Left;
                    int y = recttmp.Top;
                    int iStride = bmpData.Stride;

                    y = recttmp.Top;
                    pucStart = scan0 + (x - xmin) + (iStride * (y - ymin));

                    while (y < ymax)
                    {
                        if (y != recttmp.Top && y != ymax - 1)
                        {
                            pucPtr = pucStart;

                            *pucPtr = color;

                            pucPtr += (xmax - xmin - 1);

                            *pucPtr = color;

                            pucStart += iStride;
                            y++;
                            continue;
                        }

                        x = recttmp.Left;
                        pucPtr = pucStart;
                        while (x < xmax)
                        {
                            *pucPtr = color;

                            pucPtr++;
                            x++;
                        }
                        pucStart += iStride;
                        y++;
                    }
                    bmp.UnlockBits(bmpData);
                }
            }
            catch (Exception ex)
            {
                bmp.UnlockBits(bmpData);
            }

        }
        void Clearbmps(Bitmap[] bmps)
        {
            int i = 0;
            while (i < bmps.Length)
            {
                if (bmps[i] != null)
                    bmps[i].Dispose();
                i++;
            }
        }
    }
}
