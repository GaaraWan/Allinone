using AForge.Imaging.Filters;
using AForge.Imaging;
using AForge.Math;
using JetEazy.BasicSpace;
using JzKHC;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Allinone.BasicSpace
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

    public class SideFoundClass
    {
        List<SideFoundItemClass> itemlist = new List<SideFoundItemClass>();

        public SideFoundClass()
        {

        }
        public void Add(SideFoundItemClass item)
        {
            itemlist.Add(item);
        }
        public void ClearItems()
        {
            itemlist.Clear();
        }
        public int CountItems()
        {
            return itemlist.Count;
        }
        public void FillPts(ref Point[] pts, Point offsetpt)
        {
            int i = 0;
            while (i < pts.Length)
            {
                pts[i] = itemlist[i].ptEnd;
                pts[i].Offset(offsetpt);

                i++;
            }
        }
    }

    public class SideFoundItemClass
    {
        Point ptStart = new Point(0, 0);
        public Point ptEnd = new Point(0, 0);
        public int length = 0;

        public SideFoundItemClass(Point ptst, Point pted, int len)
        {
            ptStart = ptst;
            ptEnd = pted;
            length = len;
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

        SideFoundClass SIDEFound = new SideFoundClass();
        Rectangle myrect = new Rectangle();
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

            //bmpside.Save(@"D:\JETEAZY\SIDE" + SIDEIndex.ToString() + ".BMP",ImageFormat.Bmp);
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
            //bmp2nd.Save(@"D:\JETEAZY\S " + SIDEIndex.ToString() + "GETS.BMP", ImageFormat.Bmp);
        }

        public void Process(Bitmap bmp)
        {
            SIDEFound.ClearItems();

            Bitmap bmptmp = CopyImage(bmp);

            SearchLine(bmptmp, mysideCount, SIDEFound, 5);

            //bmptmp.Save(@"D:\JETEAZY\2ND-" + SIDEIndex.ToString() + ".BMP", ImageFormat.Bmp);
        }

        public void DrawResult(Bitmap bmp)
        {
            Graphics g = Graphics.FromImage(bmp);

            Point[] pts = new Point[SIDEFound.CountItems()];
            SIDEFound.FillPts(ref pts, myrect.Location);

            if (pts.Length > 1)
                g.DrawLines(new Pen(Color.Lime, 3), pts);

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

            //bmpsrc.Save(@"D:\JETEAZY\2TS-" + SIDEIndex.ToString() + ".BMP", ImageFormat.Bmp);

            SISThreshold sis = new SISThreshold();
            bmps[0] = sis.Apply(bmpsrc);

            //bmps[0].Save(@"D:\JETEAZY\2TS-" + SIDEIndex.ToString() + "0.BMP", ImageFormat.Bmp);

            FillHoles fillHoles = new FillHoles();
            fillHoles.CoupledSizeFiltering = true;
            fillHoles.MaxHoleWidth = 100;
            fillHoles.MaxHoleHeight = 100;
            bmps[1] = fillHoles.Apply(bmps[0]);

            //bmps[1].Save(@"D:\JETEAZY\2TS-" + SIDEIndex.ToString() + "1.BMP", ImageFormat.Bmp);

            Closing closing = new Closing();
            bmps[2] = closing.Apply(bmps[1]);
            //bmps[2].Save(@"D:\JETEAZY\2TS-" + SIDEIndex.ToString() + "2.BMP", ImageFormat.Bmp);

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

            //bmps[7].Save(@"D:\JETEAZY\2TS-" + SIDEIndex.ToString() + "7" +
            //    ".BMP", ImageFormat.Bmp);

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

        Rectangle SimpleRect(Size Sz)
        {
            return new Rectangle(0, 0, Sz.Width, Sz.Height);
        }
        public void Suiccied()
        {
            bmp1st.Dispose();
            bmpside.Dispose();
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


            //bmps[1].Save(@"D:\JETEAZY\T1.BMP", ImageFormat.Bmp);

            FillHoles fillHoles = new FillHoles();
            fillHoles.CoupledSizeFiltering = isand;
            fillHoles.MaxHoleWidth = holewidth;
            fillHoles.MaxHoleHeight = holeheight;
            bmps[2] = fillHoles.Apply(bmps[1]);

            //bmps[2].Save(@"D:\JETEAZY\T2.BMP", ImageFormat.Bmp);

            Invert invert = new Invert();
            bmps[3] = invert.Apply(bmps[2]);
            bmps[5] = fillHoles.Apply(bmps[3]);


            //bmps[3].Save(@"D:\JETEAZY\T3.BMP", ImageFormat.Bmp);
            //bmps[5].Save(@"D:\JETEAZY\T5.BMP", ImageFormat.Bmp);

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

            //bmps[6 + i - 1].Save(@"D:\JETEAZY\T" + (6 + i - 1).ToString() + ".BMP", ImageFormat.Bmp);

            bmpOrg = CopyImage(bmporg);
            bmpFirst = CopyImage(bmps[0]);

            jzHistogram.FillColor(bmpFirst, bmps[6 + i - 1], false, (byte)jzhresult.min);

            //bmpFirst.Save(@"D:\JETEAZY\TFIRST.BMP", ImageFormat.Bmp);


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
            Rectangle[] smallrects = new Rectangle[200];
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


        public void SaveAllbmps(Bitmap[] bmps, string headstr)
        {
            int i = 0;

            //while (i < bmps.Length)
            //{
            //    if (bmps[i] != null)
            //        bmps[i].Save(@"D:\JETEAZY\" + headstr + i.ToString("00") + ".BMP", ImageFormat.Bmp);
            //    i++;
            //}
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

            //vBMP.Save(@"D:\JETEAZY\VCOMPACT.BMP", ImageFormat.Bmp);
            //hBMP.Save(@"D:\JETEAZY\HCOMPACT.BMP", ImageFormat.Bmp);

            Clearbmps(bmps);
        }

        Rectangle GetAllRects(Bitmap bmpsrc, ref Rectangle[] ballrects)
        {
            Rectangle bigrect = new Rectangle(0, 0, bmpsrc.Width, bmpsrc.Height);

            Bitmap[] bmps = new Bitmap[200];

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
            ballrects = new Rectangle[blobCounter.ObjectsCount];

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
            int i = 0;

            if (bmpSecond != null)
                bmpSecond.Dispose();

            bmpSecond = CopyImage(bmpOrg);

            while (i < (int)SIDEEmnum.COUNT)
            {
                SIDEData[i].DrawResult(bmpSecond);

                i++;
            }

            //bmpSecond.Save(@"D:\JETEAZY\DRAW.bmp", ImageFormat.Bmp);


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
            if (rect.X < 0 || rect.Y < 0 || rect.Width <= 0 || rect.Height <= 0)
                return (Bitmap)bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), bmp.PixelFormat);
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
    }
}
