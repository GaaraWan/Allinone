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
    class HistogramClass
    {
        bool IsDebug
        {
            get
            {
                return false;
            }
        }
        protected JzToolsClass JzTools = new JzToolsClass();

        int ColorGap = 2;
        int BarRange = 0;

        public int MaxGrade = -1000;
        public int MinGrade = 1000;

        public int TotalGrade = 0;
        public int MeanGrade = 0;
        public int TotalPixels = 1;

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
        public HistogramClass(int rColorGap)
        {
            ColorGap = rColorGap;

            BarRange = (int)Math.Ceiling(255d / (double)ColorGap) + 1;
            SortingBars = new int[BarRange];
        }
        public void GetHistogram(Bitmap bmp)
        {
            GetHistogram(bmp, JzTools.SimpleRect(bmp.Size));
        }
        public void GetHistogram(Bitmap bmp,Rectangle rect)
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

                    while (y  < ymax)
                    {
                        x = xmin;
                        pucPtr = pucStart;
                        while (x < xmax)
                        {
                            if (pucPtr[0] != 0) //Use Zero Filter
                            {
                                Grade = JzTools.GrayscaleInt(pucPtr[2], pucPtr[1], pucPtr[0]);
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
            catch (Exception e)
            {
                bmp.UnlockBits(bmpData);

                if (IsDebug)
                    MessageBox.Show("Error :" + e.ToString());
            }

            Complete();
        }
        public void GetHistogram(Bitmap bmpOrigin, Bitmap bmpThreshed)
        {
            int OGrade = 0;
            int TGrade = 0;

            Rectangle rectbmp = JzTools.SimpleRect(bmpOrigin.Size);

            BitmapData bmpOData = bmpOrigin.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData bmpTData = bmpThreshed.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            IntPtr OScan0 = bmpOData.Scan0;
            IntPtr TScan0 = bmpTData.Scan0;

            Reset();
            try
            {
                unsafe
                {
                    byte* Oscan0 = (byte*)(void*)OScan0;
                    byte* OpucPtr;
                    byte* OpucStart;

                    byte* Tscan0 = (byte*)(void*)TScan0;
                    byte* TpucPtr;
                    byte* TpucStart;

                    int xmin = rectbmp.X;
                    int ymin = rectbmp.Y;

                    int xmax = xmin + rectbmp.Width;
                    int ymax = ymin + rectbmp.Height;

                    int x = xmin;
                    int y = ymin;

                    int iOStride = bmpOData.Stride;
                    int iTStride = bmpTData.Stride;

                    y = ymin;
                    OpucStart = Oscan0 + ((x - xmin) << 2) + (iOStride * (y - ymin));
                    TpucStart = Tscan0 + ((x - xmin) << 2) + (iTStride * (y - ymin));

                    while (y < ymax)
                    {
                        x = xmin;

                        OpucPtr = OpucStart;
                        TpucPtr = TpucStart;

                        while (x < xmax)
                        {
                            OGrade = JzTools.GrayscaleInt(OpucPtr[2], OpucPtr[1], OpucPtr[0]);
                            TGrade = JzTools.GrayscaleInt(TpucPtr[2], TpucPtr[1], TpucPtr[0]);

                            if (*((uint*)TpucPtr) == 0xFFFFFFFF)
                            {
                                *((uint*)TpucPtr) = 0xFF00FFFF;
                                OGrade = JzTools.GrayscaleInt(OpucPtr[2], OpucPtr[1], OpucPtr[0]);
                                Add(OGrade);
                            }

                            OpucPtr += 4;
                            TpucPtr += 4;
                            x++;
                        }
                        OpucStart += iOStride;
                        TpucStart += iTStride;
                        y++;
                    }
                    bmpOrigin.UnlockBits(bmpOData);
                    bmpThreshed.UnlockBits(bmpTData);
                }
            }
            catch (Exception e)
            {
                bmpOrigin.UnlockBits(bmpOData);
                bmpThreshed.UnlockBits(bmpTData);

                if (IsDebug)
                    MessageBox.Show("Error :" + e.ToString());
            }
            Complete();
        }

        public void GetHistogram(Bitmap bmp, int UppercutGrade)
        {
            int Grade = 0;

            Reset();
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
                            if (pucPtr[0] != 0) //Use Zero Filter
                            {
                                Grade = JzTools.GrayscaleInt(pucPtr[2], pucPtr[1], pucPtr[0]);

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
            catch (Exception e)
            {
                bmp.UnlockBits(bmpData);

                if (IsDebug)
                    MessageBox.Show("Error :" + e.ToString());
            }

            Complete();
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
    }
    class ScanlineHistogramClass
    {
        protected JzToolsClass JzTools = new JzToolsClass();
        bool IsDebug
        {
            get
            {
                return false;
            }
        }

        public int[] LineBars;
        public ScanlineHistogramClass()
        {

        }
        public void GetLineHistogram(Bitmap bmp,bool IsVertical)
        {
            int Grade = 0;

            Rectangle rectbmp = JzTools.SimpleRect(bmp.Size);
            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr Scan0 = bmpData.Scan0;

            if (IsVertical)
                LineBars = new int[rectbmp.Height];
            else
                LineBars = new int[rectbmp.Width];

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

                    if (IsVertical)
                    {
                        x = xmin;
                        y = ymin;
                        pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                        while (y < ymax)
                        {
                            x = xmin;
                            pucPtr = pucStart;
                            while (x < xmax)
                            {
                                Grade = JzTools.GrayscaleInt(pucPtr[2], pucPtr[1], pucPtr[0]);
                                LineBars[y] += Grade;

                                pucPtr += 4;
                                x++;
                            }

                            pucStart += iStride;
                            y++;
                        }
                    }
                    else
                    {
                        x = xmin;
                        y = ymin;
                        pucStart = scan0 + ((x - xmin) << 2) + (iStride * y);

                        while (x < xmax)
                        {
                            y = ymin;
                            pucPtr = pucStart;
                            while (y < ymax)
                            {
                                Grade = JzTools.GrayscaleInt(pucPtr[2], pucPtr[1], pucPtr[0]);
                                LineBars[x] += Grade;

                                pucPtr += iStride;
                                y++;
                            }

                            pucStart += 4;
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
