using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace JzScreenPoints.BasicSpace
{
    public class LineClass
    {
        //Y = aX + b
        //The Axis is Left to Right and Top to Left

        public PointF FirstPt = new PointF();
        public PointF SencodPt = new PointF();

        public double a = 0d;
        public double b = 0d;

        public bool IsSwap = false;

        public LineClass()
        {

        }
        public LineClass(string Str)
        {
            FromString(Str);
        }
        public LineClass(PointF Pt1, PointF Pt2)
        {
            FirstPt = Pt1;
            SencodPt = Pt2;

            FindSlopeEquation(Pt1, Pt2);
        }

        public void FindSlopeEquation()
        {
            FindSlopeEquation(FirstPt, SencodPt);
        }

        void FindSlopeEquation(PointF Pt1, PointF Pt2)
        {
            a = (double)(Pt2.Y - Pt1.Y) / (double)(Pt2.X - Pt1.X);
            b = (double)Pt1.Y - a * (double)Pt1.X;
        }

        public PointF FindIntersection(LineClass line)
        {
            PointF retptf = new PointF(-1, -1);

            if (double.IsInfinity(line.a) || double.IsInfinity(line.b))
            {
                retptf.X = line.FirstPt.X;
                retptf.Y = (float)(a * retptf.X + b);
            }
            else if (double.IsInfinity(a) || double.IsInfinity(b))
            {
                retptf.X = FirstPt.X;
                retptf.Y = (float)(line.a * retptf.X + line.b);
            }
            else
            {
                retptf.X = (float)((line.b - b) / (a - line.a));
                retptf.Y = (float)(a * retptf.X + b);
            }
            return retptf;
        }

        public double FindaAngle(LineClass line)
        {
            double ret = 0;

            double Angle1 = Math.Atan((a - line.a) / (1 + a * line.a));
            double Angle2 = Math.Atan(-(a - line.a) / (1 + a * line.a));

            ret = Math.Min(Math.Abs(Angle1), Math.Abs(Angle2));
            
            return ret;
        }



        public double GetVerticalLength(PointF Pt1)
        {
            double ret = 0;

            ret = Math.Abs(a * Pt1.X + b - Pt1.Y) / Math.Sqrt(Math.Pow(a, 2) + Math.Pow(-1, 2));

            return ret;
        }
        public PointF GetXYLengthLocation(PointF Pt1,PointF PtDatum)
        {
            PointF retptf = new PointF();

            double SlopeLength = JzTools.GetPointLength(Pt1, PtDatum);

            retptf.Y = (float)GetVerticalLength(Pt1);

            retptf.X = (float)(Math.Cos(Math.Asin(retptf.Y / SlopeLength)) * SlopeLength);


            return retptf;
        }

        public PointF GetPtFromY(float yvalue)
        {
            if (FirstPt.X == SencodPt.X)
            {
                return new PointF(FirstPt.X, yvalue);
            }
            else
            {
                double x = (yvalue - b) / a;

                return new PointF((float)x, yvalue);
            }
        }
        public PointF GetPtFromX(float xvalue)
        {
            if (FirstPt.Y == SencodPt.Y)
            {
                return new PointF(xvalue, FirstPt.Y);
            }
            else
            {
                double y = a * xvalue + b;

                return new PointF(xvalue, (float)y);
            }
        }

        public override string ToString()
        {
            string Str = "";

            Str += a.ToString() + "@";
            Str += b.ToString() + "@";
            Str += JzTools.PointFToString(FirstPt) + "@";
            Str += JzTools.PointFToString(SencodPt);

            return Str;
        }
        public void FromString(string Str)
        {
            string[] strs = Str.Split('@');

            a = double.Parse(strs[0]);
            b = double.Parse(strs[1]);
            FirstPt = JzTools.StringToPointF(strs[2]);
            SencodPt = JzTools.StringToPointF(strs[3]);
        }

    }
    public class AreaHistogramClass
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
        public AreaHistogramClass(int barrange, int gap)
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

            i = Math.Abs(i);

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
    class HistogramClass
    {
        bool IsDebug
        {
            get
            {
                return Universal.IsDebug;
            }
        }

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
        public void GetHistogram(Bitmap bmp,bool IsWithoutZeroFilter)
        {
            GetHistogram(bmp, JzTools.SimpleRect(bmp.Size), IsWithoutZeroFilter);
        }

        public void GetHistogram(Bitmap bmp, Rectangle rect)
        {
            GetHistogram(bmp, rect, false);
        }
        public void GetHistogram(Bitmap bmp, Rectangle rect,bool IsWithoutZeroFilter)
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

                    while (y < ymax)
                    {
                        x = xmin;
                        pucPtr = pucStart;
                        while (x < xmax)
                        {
                            if (pucPtr[0] != 0 || IsWithoutZeroFilter) //Use Zero Filter
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

                    while (y < ymax)
                    {
                        x = xmin;
                        pucPtr = pucStart;
                        while (x < xmax)
                        {
                            if (pucPtr[0] != ExclusiveColor) //Use Zero Filter
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
        bool IsDebug
        {
            get
            {
                return Universal.IsDebug;
            }
        }

        public int[] LineBars;
        public ScanlineHistogramClass()
        {

        }
        public void GetLineHistogram(Bitmap bmp, bool IsVertical)
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

    class FoundClass
    {
        public Rectangle rect;

        public int Width = 0;
        public int Height = 0;
        public Point Location = new Point();
        public Point Center = new Point();
        public int Area = 0;

        public Point FirstFillPoint = new Point();
        public FoundClass()
        {


        }
        public FoundClass(Rectangle rRect,int rArea,Point firstfillpoit)
        {
            rect = rRect;
            Width = rect.Width;
            Height = rect.Height;
            Location = rect.Location;
            Area = rArea;
            FirstFillPoint = firstfillpoit;

            Center = new Point(rect.X + (rect.Width >> 1), rect.Y + (rect.Height >> 1));
        }

        public override string ToString()
        {
            return JzTools.RecttoString(rect);
        }

        public FoundClass Clone()
        {
            FoundClass found = new FoundClass();

            found.rect = rect;
            found.Width = Width;
            found.Height = Height;
            found.Location = Location;
            found.Center = Center;
            found.Area = Area;
            found.FirstFillPoint = FirstFillPoint;

            return found;
        }
    }

    class JzFindObjectClass
    {
        public List<FoundClass> FoundList = new List<FoundClass>();
        bool IsDebug
        {
            get
            {
                return Universal.IsDebug;
            }
        }

        int LEFT = 0;
        int RIGHT = 0;
        int WIDTH = 0;

        int TOP = 0;
        int BOTTOM = 0;
        int HEIGHT = 0;

        int AREA = 0;
        int RECTAREA = 0;

        public int Rank = 30;
        public List<int> ListRectLength = new List<int>();
        public List<Rectangle> RectList = new List<Rectangle>();

        public Rectangle GetRectNearest(Point NearCenter,Rectangle rectBase,int XRange,int YMinRange,int YMaxRange)
        {
            RectList.Clear();

            Rectangle retRect = new Rectangle();
            int MinDistance = 10000;

            foreach (FoundClass found in FoundList)
            {
                if (found.rect.Width < 20 || found.rect.Height < 10)
                    continue;

                if (JzTools.IsInRange(found.rect.Width, rectBase.Width, (int)((double)rectBase.Width * 0.7)) && JzTools.IsInRange(found.rect.Height, rectBase.Height, (int)((double)rectBase.Height * 0.7)))
                {
                    //if (MinDistance > Math.Abs(JzTools.GetPointLength(NearCenter, JzTools.GetRectCenter(found.rect))))
                    //{
                    //    MinDistance = Math.Abs(JzTools.GetPointLength(NearCenter, JzTools.GetRectCenter(found.rect)));
                    //    retRect = found.rect;
                    //}

                    if (Math.Abs(NearCenter.X - JzTools.GetRectCenter(found.rect).X) < XRange && ((JzTools.GetRectCenter(found.rect).Y - NearCenter.Y > YMinRange && JzTools.GetRectCenter(found.rect).Y - NearCenter.Y < YMaxRange)))
                    {
                        RectList.Add(found.rect);
                        if (MinDistance > Math.Abs(JzTools.GetPointLength(NearCenter, JzTools.GetRectCenter(found.rect))))
                        {
                            MinDistance = Math.Abs(JzTools.GetPointLength(NearCenter, JzTools.GetRectCenter(found.rect)));
                            retRect = found.rect;
                        }
                    }
                }

            }

            //MinDistance = 1000;

            //if (RectList.Count > 0)
            //{
            //    foreach (Rectangle rect in RectList)
            //    {
            //        if (MinDistance > Math.Abs(NearCenter.X - JzTools.GetRectCenter(rect).X))
            //        {
            //            MinDistance = Math.Abs(NearCenter.X - JzTools.GetRectCenter(rect).X);
            //            retRect = rect;
            //        }
            //    }
            //}


            return retRect;
        }

        public Rectangle GetRectNearestEX(Point NearCenter, Rectangle rectBase, int XRange, int YMinRange, int YMaxRange)
        {
            RectList.Clear();

            Rectangle retRect = new Rectangle();
            int MaxDistance = -10000;

            foreach (FoundClass found in FoundList)
            {
                if (found.rect.Width < 20 || found.rect.Height < 10 || found.rect.Y < 5)
                    continue;

                if (JzTools.IsInRange(found.rect.Width, rectBase.Width, (int)((double)rectBase.Width * 0.7)) && JzTools.IsInRange(found.rect.Height, rectBase.Height, (int)((double)rectBase.Height * 0.7)))
                {
                    //if (MinDistance > Math.Abs(JzTools.GetPointLength(NearCenter, JzTools.GetRectCenter(found.rect))))
                    //{
                    //    MinDistance = Math.Abs(JzTools.GetPointLength(NearCenter, JzTools.GetRectCenter(found.rect)));
                    //    retRect = found.rect;
                    //}

                    if (Math.Abs(NearCenter.X - JzTools.GetRectCenter(found.rect).X) < XRange && ((JzTools.GetRectCenter(found.rect).Y - NearCenter.Y > YMinRange && JzTools.GetRectCenter(found.rect).Y - NearCenter.Y < YMaxRange)))
                    {
                        RectList.Add(found.rect);
                        if (MaxDistance < Math.Abs(NearCenter.Y - JzTools.GetRectCenter(found.rect).Y))
                        {
                            MaxDistance = Math.Abs(NearCenter.Y - JzTools.GetRectCenter(found.rect).Y);
                            retRect = found.rect;
                        }
                    }
                }

            }

            //MinDistance = 1000;

            //if (RectList.Count > 0)
            //{
            //    foreach (Rectangle rect in RectList)
            //    {
            //        if (MinDistance > Math.Abs(NearCenter.X - JzTools.GetRectCenter(rect).X))
            //        {
            //            MinDistance = Math.Abs(NearCenter.X - JzTools.GetRectCenter(rect).X);
            //            retRect = rect;
            //        }
            //    }
            //}


            return retRect;
        }
        public Rectangle rectMaxRect
        {
            get
            {
                int MaxIndex = GetMaxAndSquareRectIndex();

                if (MaxIndex == -1)
                    return new Rectangle();
                else
                    return FoundList[MaxIndex].rect;
            }
        }

        public int Count
        {
            get
            {
                return FoundList.Count;
            }

        }
        public Rectangle GetRect(int Index)
        {
            if ((FoundList.Count - 1) < Index)
                return new Rectangle();

            return FoundList[Index].rect;
        }
        public Rectangle GetRectBySort(int Index)
        {
            if ((FoundList.Count - 1) < Index)
                return new Rectangle();

            int No = int.Parse(SortingList[Index].Split(',')[1]);

            return FoundList[No].rect;
        }
        public FoundClass GetFoundBySort(int Index)
        {
            int No = 0;

            if (SortingList.Count > 0)
            {
                No = int.Parse(SortingList[Index].Split(',')[1]);

                return FoundList[No];
            }
            else
            {
                return new FoundClass();
            }

        }

        public bool IsCheckAreaOK(int area)
        {
            bool IsOK = true;

            foreach (FoundClass found in FoundList)
            {
                if (found.Area >= area)
                {
                    IsOK = false;
                    break;
                }
            }
            return IsOK;
        }

        public int GetArea(int Index)
        {
            if (FoundList.Count == 0)
                return -1;
            else
                return FoundList[Index].Area;
        }
        public int GetArea()
        {
            int retArea = 0;

            foreach (FoundClass found in FoundList)
            {
                retArea += found.Area;
            }

            return retArea;
        }
        public Point GetRectCenter(int Index)
        {
            return JzTools.GetRectCenter(FoundList[Index].rect);
        }
        public Rectangle GetRect() //Get The Whole Rectangle
        {
            int i = 0;
            Rectangle Rect = new Rectangle();

            while (i < FoundList.Count)
            {
                Rect = JzTools.MergeTwoRects(Rect, FoundList[i].rect);
                i++;
            }

            return Rect;
        }
        public Rectangle GetRect(double AreaThreshold,bool IsArea) //Get The Whole Rectangle
        {
            int i = 0;
            Rectangle Rect = new Rectangle();

            while (i < FoundList.Count)
            {
                if (FoundList[i].Area > AreaThreshold)
                {
                    Rect = JzTools.MergeTwoRects(Rect, FoundList[i].rect);
                }
                i++;
            }

            return Rect;
        }
        
        public int GetMaxRectIndex()
        {
            int MaxArea = -100;
            int retIndex = -1;
            int i = 0;

            foreach (FoundClass found in FoundList)
            {
                if (MaxArea < found.Area)
                {
                    MaxArea = found.Area;
                    retIndex = i;
                }
                i++;
            }

            return retIndex;
        }
        public int GetHighestRectIndex()
        {
            int MaxHeight = -100;
            int retIndex = -1;
            int i = 0;

            foreach (FoundClass found in FoundList)
            {
                if (MaxHeight < found.Height)
                {
                    MaxHeight = found.Height;
                    retIndex = i;
                }
                i++;
            }

            return retIndex;
        }
        public int GetMaxAndSquareRectIndex()
        {
            int MaxArea = -100;
            int retIndex = -1;
            int i = 0;

            foreach (FoundClass found in FoundList)
            {
                if (MaxArea < found.Area) //&& Math.Abs(((float)found.Width / (float)found.Height) - 1) < 0.2)
                {
                    MaxArea = found.Area;
                    retIndex = i;
                }
                i++;
            }

            return retIndex;
        }

        public Rectangle GetRect(Rectangle IntersectRect)
        {
            int i = 0;
            Rectangle Rect = new Rectangle();

            while (i < FoundList.Count)
            {
                if (FoundList[i].rect.IntersectsWith(IntersectRect))
                    Rect = JzTools.MergeTwoRects(Rect, FoundList[i].rect);
                i++;
            }
            return Rect;
        }
        public Point GetCornerGroup(Point Pt,Rectangle Rect)
        {
            int i = 0;
            int iTmp = 0;
            int MinLength = int.MinValue;
            Point MinPt = Pt;
            Rectangle RectTmp = new Rectangle();

            i = 0;
            ListRectLength.Clear();

            while (i < Count)
            {
                iTmp = (int)(JzTools.GetLTSuggestion(Pt, GetRectCenter(i), Rect) * 1000000) /1000;

                ListRectLength.Add(iTmp * 1000 + i);

                if(iTmp > MinLength)
                {
                    MinLength = iTmp;
                    MinPt = GetRectCenter(i);
                }
                i++;
            }
            ListRectLength.Sort();

            i = 0;
            while (i < Rank)
            {
                RectTmp = JzTools.MergeTwoRects(RectTmp, GetRect(ListRectLength[ListRectLength.Count - i - 1] % 1000));

                i++;
            }

            return JzTools.GetRectCenter(RectTmp);
        }

        public JzFindObjectClass()
        {

        }
        void Reset(int X, int Y)
        {
            LEFT = X;
            RIGHT = X;

            TOP = Y;
            BOTTOM = Y;

            WIDTH = 1;
            HEIGHT = 1;

            AREA = 0;
            RECTAREA = 0;
        }
        public void Find(Bitmap bmp, Color FillColor)
        {
            Find(bmp, FillColor, new Size(int.MinValue, int.MinValue), new Size(int.MaxValue, int.MaxValue));
        }
        public void Find(Bitmap bmp, Color FillColor, Size OKSize, Size NGSize)
        {
            Find(bmp, JzTools.SimpleRect(bmp.Size), FillColor, OKSize, NGSize);
        }


        object obj = new object();

        public void Find(Bitmap bmp,Rectangle Rect, Color FillColor, Size OKSize, Size NGSize)
        {
            lock (obj)
            {

                uint ColorValue = (uint)((FillColor.A << 24) + (FillColor.R << 16) + (FillColor.G << 8) + FillColor.B);

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

                        y = ymin;
                        pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                        FoundList.Clear();

                        while (y < ymax)
                        {
                            x = xmin;
                            pucPtr = pucStart;
                            while (x < xmax)
                            {
                                if (pucPtr[0] == 0xFF) //<== ¸T¤î¶ñÂÅ¦â!!!!¡A¶È¬õ¡Aºñ¡A¶À¥Î¸û¦n
                                {
                                    Reset(x, y);
                                    Find(iStride, pucPtr, x, y, rectbmp, ColorValue);

                                    WIDTH = RIGHT - LEFT + 1;
                                    HEIGHT = BOTTOM - TOP + 1;
                                    RECTAREA = WIDTH * HEIGHT;

                                    //if ((OKSize.Width <= WIDTH && OKSize.Height <= HEIGHT) && !(NGSize.Width >= WIDTH && NGSize.Height >= HEIGHT))
                                    if ((OKSize.Width <= WIDTH && OKSize.Height <= HEIGHT) && (NGSize.Width >= WIDTH || NGSize.Height >= HEIGHT))
                                        FoundList.Add(new FoundClass(new Rectangle(LEFT, TOP, WIDTH, HEIGHT), AREA, new Point(x, y)));
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
                catch (Exception e)
                {
                    bmp.UnlockBits(bmpData);

                    if (IsDebug)
                        MessageBox.Show("Error :" + e.ToString());
                }
            }
        }
        public void Find(Bitmap bmp, Rectangle Rect, Color FillColor, Size OKSize, Size NGSize,int Ratio)
        {
            uint ColorValue = (uint)((FillColor.A << 24) + (FillColor.R << 16) + (FillColor.G << 8) + FillColor.B);

            Rectangle rectbmp = Rect;
            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr Scan0 = bmpData.Scan0;
            double CheckRatio = Ratio / 100d;
            double FoundRatio = 0;

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

                    FoundList.Clear();

                    while (y < ymax)
                    {
                        x = xmin;
                        pucPtr = pucStart;
                        while (x < xmax)
                        {
                            if (pucPtr[0] == 0xFF) //<== ¸T¤î¶ñÂÅ¦â!!!!¡A¶È¬õ¡Aºñ¡A¶À¥Î¸û¦n
                            {
                                Reset(x, y);
                                Find(iStride, pucPtr, x, y, rectbmp, ColorValue);

                                WIDTH = RIGHT - LEFT + 1;
                                HEIGHT = BOTTOM - TOP + 1;
                                RECTAREA = WIDTH * HEIGHT;

                                FoundRatio = (double)AREA / (double)RECTAREA;

                                if (FoundRatio >= CheckRatio)
                                {
                                    if ((OKSize.Width <= WIDTH && OKSize.Height <= HEIGHT) && !(NGSize.Width >= WIDTH && NGSize.Height >= HEIGHT))
                                        FoundList.Add(new FoundClass(new Rectangle(LEFT, TOP, WIDTH, HEIGHT), AREA, new Point(x, y)));
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
            catch (Exception e)
            {
                bmp.UnlockBits(bmpData);

                if (IsDebug)
                    MessageBox.Show("Error :" + e.ToString());
            }
        }
        public void FindEx(Bitmap bmp, Rectangle Rect, Color FillColor,Size OKSize, Size NGSize,int MinHeight)
        {
            uint ColorValue = (uint)((FillColor.A << 24) + (FillColor.R << 16) + (FillColor.G << 8) + FillColor.B);

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

                    y = ymin;
                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                    FoundList.Clear();

                    while (y < ymax)
                    {
                        x = xmin;
                        pucPtr = pucStart;
                        while (x < xmax)
                        {
                            if (pucPtr[0] == 0xFF) //<== ¸T¤î¶ñÂÅ¦â!!!!¡A¶È¬õ¡Aºñ¡A¶À¥Î¸û¦n
                            {
                                Reset(x, y);
                                Find(iStride, pucPtr, x, y, rectbmp, ColorValue);

                                WIDTH = RIGHT - LEFT + 1;
                                HEIGHT = BOTTOM - TOP + 1;
                                RECTAREA = WIDTH * HEIGHT;

                                if (HEIGHT >= MinHeight)
                                {
                                    if ((OKSize.Width <= WIDTH && OKSize.Height <= HEIGHT) && !(NGSize.Width >= WIDTH && NGSize.Height >= HEIGHT))
                                        FoundList.Add(new FoundClass(new Rectangle(LEFT, TOP, WIDTH, HEIGHT), AREA, new Point(x, y)));
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
            catch (Exception e)
            {
                bmp.UnlockBits(bmpData);

                if (IsDebug)
                    MessageBox.Show("Error :" + e.ToString());
            }
        }

        public void Find(Bitmap bmp, Point PtStart, Color FillColor,int seedcolor)
        {
            uint ColorValue = (uint)((FillColor.A << 24) + (FillColor.R << 16) + (FillColor.G << 8) + FillColor.B);

            Rectangle rectbmp = JzTools.SimpleRect(bmp.Size);
            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr Scan0 = bmpData.Scan0;
            try
            {
                unsafe
                {
                    byte* scan0 = (byte*)(void*)Scan0;
                    byte* pucStart;

                    int xmin = rectbmp.X;
                    int ymin = rectbmp.Y;
                    int xmax = xmin + rectbmp.Width;
                    int ymax = ymin + rectbmp.Height;

                    int x = PtStart.X;
                    int y = PtStart.Y;
                    int iStride = bmpData.Stride;

                    //y = ymin;
                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                    FoundList.Clear();

                    if (pucStart[0] == seedcolor && pucStart[1] == seedcolor && pucStart[2] == seedcolor) //<== ¸T¤î¶ñÂÅ¦â!!!!¡A¶È¬õ¡Aºñ¡A¶À¥Î¸û¦n
                    {
                        Reset(x, y);
                        Find(iStride, pucStart, x, y, rectbmp, ColorValue);

                        WIDTH = RIGHT - LEFT + 1;
                        HEIGHT = BOTTOM - TOP + 1;
                        RECTAREA = WIDTH * HEIGHT;

                        FoundList.Add(new FoundClass(new Rectangle(LEFT, TOP, WIDTH, HEIGHT), AREA, new Point(x, y)));
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
        public void Find(Bitmap bmp, Point PtStart, Color FillColor)
        {
            Find(bmp, PtStart, FillColor, 255);

        }

        //public void Find(Bitmap bmp, Point PtStart, Color FillColor)
        //{
        //    uint ColorValue = (uint)((FillColor.A << 24) + (FillColor.R << 16) + (FillColor.G << 8) + FillColor.B);

        //    Rectangle rectbmp = JzTools.SimpleRect(bmp.Size);
        //    BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        //    IntPtr Scan0 = bmpData.Scan0;
        //    try
        //    {
        //        unsafe
        //        {
        //            byte* scan0 = (byte*)(void*)Scan0;
        //            byte* pucStart;

        //            int xmin = rectbmp.X;
        //            int ymin = rectbmp.Y;
        //            int xmax = xmin + rectbmp.Width;
        //            int ymax = ymin + rectbmp.Height;

        //            int x = PtStart.X;
        //            int y = PtStart.Y;
        //            int iStride = bmpData.Stride;

        //            //y = ymin;
        //            pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
        //            FoundList.Clear();

        //            if (pucStart[0] == 0xFF) //<== ¸T¤î¶ñÂÅ¦â!!!!¡A¶È¬õ¡Aºñ¡A¶À¥Î¸û¦n
        //            {
        //                Reset(x, y);
        //                Find(iStride, pucStart, x, y, rectbmp, ColorValue);

        //                WIDTH = RIGHT - LEFT + 1;
        //                HEIGHT = BOTTOM - TOP + 1;
        //                RECTAREA = WIDTH * HEIGHT;

        //                FoundList.Add(new Found(new Rectangle(LEFT, TOP, WIDTH, HEIGHT), AREA));
        //            }
        //            bmp.UnlockBits(bmpData);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        bmp.UnlockBits(bmpData);

        //        if (IsDebug)
        //            MessageBox.Show("Error :" + e.ToString());
        //    }
        //}
        
        unsafe void Find(int strid, byte* pucPtr, int x, int y, Rectangle rect, uint FillColorValue)
        {
            //byte* pucStart = pucPtr;

            int xmin = rect.X;
            int xmax = rect.X + rect.Width - 1;
            int ymin = rect.Y;
            int ymax = rect.Y + rect.Height - 1;

            byte* pucStart = pucPtr;

            *((uint*)pucPtr) = 0xFFFFFFFF;

            int xLeft = x;
            int xRight = x;

            ///////////////////GO LEFT
            while (xLeft >= xmin && pucPtr[0] != 0) // Converge to xmin
            {
                *((uint*)pucPtr) = FillColorValue;

                AREA++;
                pucPtr -= 4;
                xLeft--;
            }
            xLeft++;

            LEFT = Math.Min(LEFT, xLeft);

            ////////////////////////////

            pucPtr = pucStart;
            *((uint*)pucPtr) = 0xFFFFFFFF;

            ///////////////////GO RIGHT
            while (xRight <= xmax && pucPtr[0] != 0) //Converge to xmax
            {
                *((uint*)pucPtr) = FillColorValue;

                AREA++;
                pucPtr += 4;
                xRight++;
            }
            xRight--;
            ////////////////////////////

            RIGHT = Math.Max(RIGHT, xRight);

            //AREA += (RIGHT - LEFT) + 1;

            while (xLeft <= xRight)
            {
                if (y - 1 >= ymin)
                {
                    pucPtr = pucStart - ((x - xLeft) << 2);
                    pucPtr -= strid;
                    if (pucPtr[0] == 0xFF)
                    {
                        TOP = Math.Min(y - 1, TOP);
                        Find(strid, pucPtr, xLeft, y - 1, rect, FillColorValue);
                    }
                }
                if (y + 1 <= ymax)
                {
                    pucPtr = pucStart - ((x - xLeft) << 2);
                    pucPtr += strid;
                    if (pucPtr[0] == 0xFF)
                    {
                        BOTTOM = Math.Max(y + 1, BOTTOM);
                        Find(strid, pucPtr, xLeft, y + 1, rect, FillColorValue);
                    }
                }
                xLeft++;
            }
        }
        unsafe void FindDark(int strid, byte* pucPtr, int x, int y, Rectangle rect, uint FillColorValue)
        {
            //byte* pucStart = pucPtr;

            int xmin = rect.X;
            int xmax = rect.X + rect.Width - 1;
            int ymin = rect.Y;
            int ymax = rect.Y + rect.Height - 1;

            byte* pucStart = pucPtr;

            *((uint*)pucPtr) = 0x0;

            int xLeft = x;
            int xRight = x;

            ///////////////////GO LEFT
            while (xLeft >= xmin && (pucPtr[0] == 0 && pucPtr[1] == 0 && pucPtr[2] == 0)) // Converge to xmin
            {
                *((uint*)pucPtr) = FillColorValue;

                AREA++;
                pucPtr -= 4;
                xLeft--;
            }
            xLeft++;

            LEFT = Math.Min(LEFT, xLeft);

            ////////////////////////////

            pucPtr = pucStart;
            *((uint*)pucPtr) = 0x0;

            ///////////////////GO RIGHT
            while (xRight <= xmax && (pucPtr[0] == 0 && pucPtr[1] == 0 && pucPtr[2] == 0)) //Converge to xmax
            {
                *((uint*)pucPtr) = FillColorValue;

                AREA++;
                pucPtr += 4;
                xRight++;
            }
            xRight--;
            ////////////////////////////

            RIGHT = Math.Max(RIGHT, xRight);

            //AREA += (RIGHT - LEFT) + 1;

            while (xLeft <= xRight)
            {
                if (y - 1 >= ymin)
                {
                    pucPtr = pucStart - ((x - xLeft) << 2);
                    pucPtr -= strid;
                    if ((pucPtr[0] == 0 && pucPtr[1] == 0 && pucPtr[2] == 0))
                    {
                        TOP = Math.Min(y - 1, TOP);
                        FindDark(strid, pucPtr, xLeft, y - 1, rect, FillColorValue);
                    }
                }
                if (y + 1 <= ymax)
                {
                    pucPtr = pucStart - ((x - xLeft) << 2);
                    pucPtr += strid;
                    if ((pucPtr[0] == 0 && pucPtr[1] == 0 && pucPtr[2] == 0))
                    {
                        BOTTOM = Math.Max(y + 1, BOTTOM);
                        FindDark(strid, pucPtr, xLeft, y + 1, rect, FillColorValue);
                    }
                }
                xLeft++;
            }
        }

        List<string> SortingList = new List<string>();

        public FoundClass CompFoundWithRect(Rectangle goldfoundrect)
        {
            FoundClass retfound = new FoundClass();

            foreach (FoundClass found in FoundList)
            {
                if (found.rect.IntersectsWith(goldfoundrect))
                {
                    retfound = found;
                    break;
                }
            }
            return retfound;
        }


        public FoundClass CompFoundWithGolden(FoundClass goldenFound, double widthratio, double heightratio, double arearatio)
        {
            FoundClass retfound = new FoundClass();

            foreach (FoundClass found in FoundList)
            {
                if (JzTools.IsInRangeRatio(found.Width, goldenFound.Width, widthratio) &&
                    JzTools.IsInRangeRatio(found.Height, goldenFound.Height, heightratio) &&
                    JzTools.IsInRangeRatio(found.Area, goldenFound.Area, arearatio))
                {
                    retfound = found;
                    break;
                }
            }
            return retfound;
        }
        public bool CompFoundArea(double comparea, double arearatio)
        {
            bool ret = true;

            foreach (FoundClass found in FoundList)
            {
                if (JzTools.IsInRangeRatio((double)found.Area, comparea, arearatio))
                {
                    ret = false;
                    break;
                }
            }
            return ret;
        }


        public void SortByArea()    // Find the Max Area
        {
            int i = 0;

            SortingList.Clear();

            foreach (FoundClass found in FoundList)
            {
                SortingList.Add(found.Area.ToString("00000") + "," + i.ToString());
                i++;
            }

            SortingList.Sort();
            SortingList.Reverse();
        }

        public void FindGrayscale(Bitmap bmp, Point PtStart, Color FillColor, int ColorRange)
        {
            uint ColorValue = (uint)((FillColor.A << 24) + (FillColor.R << 16) + (FillColor.G << 8) + FillColor.B);

            Rectangle rectbmp = JzTools.SimpleRect(bmp.Size);
            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr Scan0 = bmpData.Scan0;
            try
            {
                unsafe
                {
                    byte* scan0 = (byte*)(void*)Scan0;
                    byte* pucStart;

                    int xmin = rectbmp.X;
                    int ymin = rectbmp.Y;
                    int xmax = xmin + rectbmp.Width;
                    int ymax = ymin + rectbmp.Height;

                    int x = PtStart.X;
                    int y = PtStart.Y;
                    int iStride = bmpData.Stride;

                    //y = ymin;
                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                    FoundList.Clear();


                    int BaseColor = pucStart[0];
                    //if (pucStart[0] == 0xFF) //<== ¸T¤î¶ñÂÅ¦â!!!!¡A¶È¬õ¡Aºñ¡A¶À¥Î¸û¦n
                    {
                        Reset(x, y);
                        FindGrayscale(iStride, pucStart, x, y, rectbmp, ColorValue, BaseColor, ColorRange);

                        WIDTH = RIGHT - LEFT + 1;
                        HEIGHT = BOTTOM - TOP + 1;
                        RECTAREA = WIDTH * HEIGHT;

                        FoundList.Add(new FoundClass(new Rectangle(LEFT, TOP, WIDTH, HEIGHT), AREA, new Point(x, y)));
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

        public void FindGrayscale(Bitmap bmp, Point PtStart, Color FillColor,int BaseColor,int ColorRange)
        {
            uint ColorValue = (uint)((FillColor.A << 24) + (FillColor.R << 16) + (FillColor.G << 8) + FillColor.B);

            Rectangle rectbmp = JzTools.SimpleRect(bmp.Size);
            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr Scan0 = bmpData.Scan0;
            try
            {
                unsafe
                {
                    byte* scan0 = (byte*)(void*)Scan0;
                    byte* pucStart;

                    int xmin = rectbmp.X;
                    int ymin = rectbmp.Y;
                    int xmax = xmin + rectbmp.Width;
                    int ymax = ymin + rectbmp.Height;

                    int x = PtStart.X;
                    int y = PtStart.Y;
                    int iStride = bmpData.Stride;

                    //y = ymin;
                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                    FoundList.Clear();

                    //if (pucStart[0] == 0xFF) //<== ¸T¤î¶ñÂÅ¦â!!!!¡A¶È¬õ¡Aºñ¡A¶À¥Î¸û¦n
                    {
                        Reset(x, y);
                        FindGrayscale(iStride, pucStart, x, y, rectbmp, ColorValue, BaseColor, ColorRange);

                        WIDTH = RIGHT - LEFT + 1;
                        HEIGHT = BOTTOM - TOP + 1;
                        RECTAREA = WIDTH * HEIGHT;

                        FoundList.Add(new FoundClass(new Rectangle(LEFT, TOP, WIDTH, HEIGHT), AREA, new Point(x, y)));
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
        unsafe void FindGrayscale(int strid, byte* pucPtr, int x, int y, Rectangle rect, uint FillColorValue,int BaseColor,int ColorRange)
        {
            //byte* pucStart = pucPtr;

            int xmin = rect.X;
            int xmax = rect.X + rect.Width - 1;
            int ymin = rect.Y;
            int ymax = rect.Y + rect.Height - 1;

            byte* pucStart = pucPtr;

            *((uint*)pucPtr) = 0xFFFFFFFF;
            pucPtr[0] = (byte)BaseColor;
            
            int xLeft = x;
            int xRight = x;

            ///////////////////GO LEFT
            while (xLeft >= xmin && (JzTools.IsInRange((int)pucPtr[0], BaseColor, ColorRange)) && *((uint*)pucPtr) != FillColorValue) // Converge to xmin
            {
                *((uint*)pucPtr) = FillColorValue;

                AREA++;
                pucPtr -= 4;
                xLeft--;
            }
            xLeft++;

            LEFT = Math.Min(LEFT, xLeft);

            ////////////////////////////

            pucPtr = pucStart;

            *((uint*)pucPtr) = 0xFFFFFFFF;
            pucPtr[0] = (byte)BaseColor;

            ///////////////////GO RIGHT
            while (xRight <= xmax && (JzTools.IsInRange((int)pucPtr[0], BaseColor, ColorRange)) && *((uint*)pucPtr) != FillColorValue) //Converge to xmax
            {
                *((uint*)pucPtr) = FillColorValue;

                AREA++;
                pucPtr += 4;
                xRight++;
            }
            xRight--;
            ////////////////////////////

            RIGHT = Math.Max(RIGHT, xRight);

            //AREA += (RIGHT - LEFT) + 1;

            while (xLeft <= xRight)
            {
                if (y - 1 >= ymin)
                {
                    pucPtr = pucStart - ((x - xLeft) << 2);
                    pucPtr -= strid;
                    if (JzTools.IsInRange((int)pucPtr[0], BaseColor, ColorRange) && *((uint*)pucPtr) != FillColorValue)
                    {
                        TOP = Math.Min(y - 1, TOP);
                        FindGrayscale(strid, pucPtr, xLeft, y - 1, rect, FillColorValue,BaseColor,ColorRange);
                    }
                }
                if (y + 1 <= ymax)
                {
                    pucPtr = pucStart - ((x - xLeft) << 2);
                    pucPtr += strid;
                    if (JzTools.IsInRange((int)pucPtr[0], BaseColor, ColorRange) && *((uint*)pucPtr) != FillColorValue)
                    {
                        BOTTOM = Math.Max(y + 1, BOTTOM);
                        FindGrayscale(strid, pucPtr, xLeft, y + 1, rect, FillColorValue, BaseColor, ColorRange);
                    }
                }
                xLeft++;
            }
        }

        public void SetThreshold(Bitmap bmp, int ThresholdValue, int ThresholdRangeUpper, int ThresholdRangeLower)
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
        public void SetThreshold(Bitmap bmp, Rectangle Rect, int ThresholdValue, int ThresholdRangeUpper, int ThresholdRangeLower, bool IsInRangeColorWhite)
        {
            lock (obj)
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
        public void SetThresholdEX(Bitmap bmp, Rectangle Rect,int ThresholdRangeUpper, int ThresholdRangeLower, bool IsInRangeColorWhite)
        {
            lock (obj)
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

                        int ThresholdValueMax = Math.Min(255, ThresholdRangeUpper);
                        int ThresholdValueMin = Math.Max(0, ThresholdRangeLower);

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
        }

        public void SetThresholdColor(Bitmap bmp, int ThresholdValue, int ThresholdRangeUpper, int ThresholdRangeLower, int MaxGrade)
        {
            SetThresholdColor(bmp, ThresholdValue, ThresholdRangeUpper, ThresholdRangeLower, MaxGrade, false);
        }
        public void SetThresholdColor(Bitmap bmp, int ThresholdValue, int ThresholdRangeUpper, int ThresholdRangeLower, int MaxGrade,bool IsReverse)
        {
            int Grade = 0;

            Rectangle rectbmp = JzTools.SimpleRect(bmp.Size);
            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr Scan0 = bmpData.Scan0;

            uint CheckColor = 0xFFFFFFFF;


            int Gap = (MaxGrade - ThresholdRangeLower) / 20;

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

                            if (IsReverse)
                                Grade = 255 - Grade;

                            if (Grade > MaxGrade - Gap)
                            {
                                CheckColor = JzTools.ColorValue(Color.Red);
                            }
                            else if (Grade > MaxGrade - (Gap * 2))
                            {
                                CheckColor = JzTools.ColorValue(Color.Orange);
                            }
                            else if (Grade > MaxGrade - (Gap * 3))
                            {
                                CheckColor = JzTools.ColorValue(Color.Yellow);
                            }
                            else if (Grade > MaxGrade - (Gap * 4))
                            {
                                CheckColor = JzTools.ColorValue(Color.Green);
                            }
                            else if (Grade > MaxGrade - (Gap * 5))
                            {
                                CheckColor = JzTools.ColorValue(Color.Blue);
                            }
                            else if (Grade > MaxGrade - (Gap * 6))
                            {
                                CheckColor = JzTools.ColorValue(Color.Purple);
                            }
                            else
                                CheckColor = JzTools.ColorValue(Color.White);


                            *((uint*)pucPtr) = (JzTools.IsInRangeEx(Grade, ThresholdValueMax, ThresholdValueMin) ? 0xFF000000 : CheckColor);

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

        public int GetLine(Bitmap bmp, bool IsReverse, int FromY, double Ratio)
        {
            lock (obj)
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

                        y = FromY;
                        pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                        double LinePixelCount = 0;

                        if (!IsReverse)
                        {
                            while (y < ymax)
                            {
                                LinePixelCount = 0;
                                x = xmin;
                                pucPtr = pucStart;
                                while (x < xmax)
                                {
                                    if (pucPtr[0] == 255)
                                        LinePixelCount++;

                                    pucPtr += 4;
                                    x++;
                                }

                                if ((LinePixelCount / (double)(xmax - xmin)) > Ratio)
                                    break;

                                pucStart += iStride;

                                y++;
                            }
                        }
                        else
                        {
                            while (y > -1)
                            {
                                LinePixelCount = 0;
                                x = xmin;
                                pucPtr = pucStart;
                                while (x < xmax)
                                {
                                    if (pucPtr[0] == 255)
                                        LinePixelCount++;

                                    pucPtr += 4;
                                    x++;
                                }

                                if ((LinePixelCount / (double)(xmax - xmin)) > Ratio)
                                    break;

                                pucStart -= iStride;

                                y--;
                            }
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
        }
        public void GetLineAndFill(Bitmap bmp, double Ratio,bool IsWhite)
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

                    uint InRangeColor = 0xFF000000;
                    uint OutrangeColor = 0xFFFFFFFF;

                    y = ymin;
                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                    double LinePixelCount = 0;

                    while (y < ymax)
                    {
                        LinePixelCount = 0;
                        x = xmin;
                        pucPtr = pucStart;
                        while (x < xmax)
                        {
                            if (pucPtr[0] == 255)
                                LinePixelCount++;

                            pucPtr += 4;
                            x++;
                        }

                        if ((LinePixelCount / (double)(xmax - xmin)) > Ratio)
                        {
                            x = xmin;
                            pucPtr = pucStart;
                            while (x < xmax)
                            {
                                *((uint*)pucPtr) = (!IsWhite ? InRangeColor : OutrangeColor);

                                pucPtr += 4;
                                x++;
                            }

                            pucStart += iStride;
                            y++;
                            continue;
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

        public int GetLine(Bitmap bmp, bool IsReverse, int FromY, int XLocation)
        {
            return GetLine(bmp, IsReverse, FromY, XLocation, false);
        }
        public int GetLine(Bitmap bmp, bool IsReverse, int FromY,int XLocation,bool IsNoDrawing)
        {
            lock (obj)
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

                        int x = XLocation;
                        int y = ymin;
                        int iStride = bmpData.Stride;

                        y = FromY;
                        pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                        pucStart[0] = 0;
                        pucStart[1] = 0;
                        pucStart[2] = 0;

                        if (!IsReverse)
                        {
                            while (y < ymax)
                            {
                                pucPtr = pucStart;

                                if (pucPtr[2] != 0)
                                {
                                    pucPtr[0] = 255;
                                    pucPtr[1] = 255;
                                    break;
                                }
                                else
                                {
                                    if (!IsNoDrawing)
                                    {
                                        pucPtr[0] = 255;
                                        pucPtr[1] = 255;
                                    }
                                }

                                pucStart += iStride;

                                y++;
                            }
                        }
                        else
                        {
                            while (y > -1)
                            {
                                pucPtr = pucStart;

                                if (pucPtr[2] != 0)
                                {
                                    pucPtr[0] = 255;
                                    pucPtr[1] = 255;
                                    break;
                                }
                                else
                                {
                                    if (!IsNoDrawing)
                                    {
                                        pucPtr[0] = 255;
                                        pucPtr[1] = 255;
                                    }
                                }

                                pucStart -= iStride;

                                y--;
                            }
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
        }

        public int GetLineInYDir(Bitmap bmp, bool IsReverse, int FromY, int XLocation, Color StopColor)
        {
            lock (obj)
            {
                Rectangle rectbmp = JzTools.SimpleRect(bmp.Size);
                BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                IntPtr Scan0 = bmpData.Scan0;

                //try
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

                        int x = XLocation;
                        int y = ymin;
                        int iStride = bmpData.Stride;

                        y = FromY;
                        pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                        pucStart[0] = (byte)(255 - StopColor.R);
                        pucStart[1] = (byte)(255 - StopColor.G);
                        pucStart[2] = (byte)(255 - StopColor.B);

                        if (!IsReverse)
                        {
                            while (y < ymax)
                            {
                                pucPtr = pucStart;

                                if (pucPtr[0] == StopColor.R && pucPtr[1] == StopColor.G && pucPtr[2] == StopColor.B)
                                {
                                    pucPtr[0] = 0;
                                    pucPtr[1] = 0;
                                    pucPtr[2] = 255;

                                    break;
                                }
                                else
                                {
                                    pucPtr[0] = 255;
                                    pucPtr[1] = 255;
                                    pucPtr[2] = 0;
                                }

                                pucStart += iStride;

                                y++;
                            }
                        }
                        else
                        {
                            while (y > -1)
                            {
                                pucPtr = pucStart;

                                if (pucPtr[0] == StopColor.R && pucPtr[1] == StopColor.G && pucPtr[2] == StopColor.B)
                                {
                                    pucPtr[0] = 0;
                                    pucPtr[1] = 0;
                                    pucPtr[2] = 255;

                                    break;
                                }
                                else
                                {
                                    pucPtr[0] = 255;
                                    pucPtr[1] = 255;
                                    pucPtr[2] = 0;
                                }

                                pucStart -= iStride;

                                y--;
                            }
                        }

                        bmp.UnlockBits(bmpData);

                        return y;
                    }
                }
                //catch (Exception e)
                //{
                //    bmp.UnlockBits(bmpData);

                //    //if (IsDebug)
                //    //    MessageBox.Show("Error :" + e.ToString());

                //    return -1;
                //}
            }
        }
        public int GetLineInXDir(Bitmap bmp, bool IsReverse, int FromX, int YLocation, Color StopColor)
        {
            lock (obj)
            {
                Rectangle rectbmp = JzTools.SimpleRect(bmp.Size);
                BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                IntPtr Scan0 = bmpData.Scan0;

                //try
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
                        int y = YLocation;
                        int iStride = bmpData.Stride;

                        x = FromX;
                        pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                        pucStart[0] = (byte)(255 - StopColor.R);
                        pucStart[1] = (byte)(255 - StopColor.G);
                        pucStart[2] = (byte)(255 - StopColor.B);

                        if (!IsReverse)
                        {
                            while (x < xmax)
                            {
                                pucPtr = pucStart;

                                if (pucPtr[0] == StopColor.R && pucPtr[1] == StopColor.G && pucPtr[2] == StopColor.B)
                                {
                                    pucPtr[0] = 0;
                                    pucPtr[1] = 0;
                                    pucPtr[2] = 255;

                                    break;
                                }
                                else
                                {
                                    pucPtr[0] = 255;
                                    pucPtr[1] = 255;
                                    pucPtr[2] = 0;

                                }

                                pucStart += 4;

                                x++;
                            }
                        }
                        else
                        {
                            while (x > -1)
                            {
                                pucPtr = pucStart;

                                if (pucPtr[0] == StopColor.R && pucPtr[1] == StopColor.G && pucPtr[2] == StopColor.B)
                                {
                                    pucPtr[0] = 0;
                                    pucPtr[1] = 0;
                                    pucPtr[2] = 255;

                                    break;
                                }
                                else
                                {
                                    pucPtr[0] = 255;
                                    pucPtr[1] = 255;
                                    pucPtr[2] = 0;
                                }

                                pucStart -= 4;

                                x--;
                            }
                        }

                        bmp.UnlockBits(bmpData);

                        return x;
                    }
                }
                //catch (Exception e)
                //{
                //    bmp.UnlockBits(bmpData);

                //    //if (IsDebug)
                //    //    MessageBox.Show("Error :" + e.ToString());

                //    return -1;
                //}
            }
        }

        public int GetLineRedInYDir(Bitmap bmp, bool IsReverse, int FromY, int XLocation)
        {
            lock (obj)
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

                        int x = XLocation;
                        int y = ymin;
                        int iStride = bmpData.Stride;

                        y = FromY;
                        pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                        if (!IsReverse)
                        {
                            while (y < ymax)
                            {
                                pucPtr = pucStart;

                                if (pucPtr[0] == 0 && pucPtr[1] == 0 && pucPtr[2] == 255)
                                    break;
                                else
                                {
                                    pucPtr[0] = 255;
                                    pucPtr[1] = 255;
                                }

                                pucStart += iStride;

                                y++;
                            }
                        }
                        else
                        {
                            while (y > -1)
                            {
                                pucPtr = pucStart;

                                if (pucPtr[0] == 0 && pucPtr[1] == 0 && pucPtr[2] == 255)
                                    break;
                                else
                                {
                                    pucPtr[0] = 255;
                                    pucPtr[1] = 255;
                                }

                                pucStart -= iStride;

                                y--;
                            }
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
        }
        public int GetLineRedInXDir(Bitmap bmp, bool IsReverse, int FromX, int YLocation)
        {
            lock (obj)
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
                        int y = YLocation;
                        int iStride = bmpData.Stride;

                        x = FromX;
                        pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                        if (!IsReverse)
                        {
                            while (x < xmax)
                            {
                                pucPtr = pucStart;

                                if (pucPtr[0] == 0 && pucPtr[1] == 0 && pucPtr[2] == 255)
                                    break;
                                else
                                {
                                    pucPtr[0] = 255;
                                    pucPtr[1] = 255;
                                }

                                pucStart += 4;

                                x++;
                            }
                        }
                        else
                        {   
                            while (x > -1)
                            {
                                pucPtr = pucStart;

                                if (pucPtr[0] == 0 && pucPtr[1] == 0 && pucPtr[2] == 255)
                                    break;
                                else
                                {
                                    pucPtr[0] = 255;
                                    pucPtr[1] = 255;
                                }

                                pucStart -= 4;

                                x--;
                            }
                        }

                        bmp.UnlockBits(bmpData);

                        return x;
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
        }

        public PointF GetCrossCenter(Bitmap bmp, int seed, int gap)
        {
            PointF retPTF = new PointF();

            List<PointF> ptfList = new List<PointF>();


            PointF PTF1 = new PointF();
            PointF PTF2 = new PointF();

            PointF PTFLeft = new PointF();
            PointF PTFRight = new PointF();
            PointF PTFTop = new PointF();
            PointF PTFBottom = new PointF();

            int i = 0;

            #region Find Left Point
            
            i = 0;
            while (i < seed)
            {
                PointF ptf = new PointF();

                //Find From Left Top Down
                ptf.X = (bmp.Width / 8) +  i * gap;
                ptf.Y = GetLineRedInYDir(bmp, false, 0, (int)ptf.X);

                ptfList.Add(ptf);

                i++;
            }
            PTF1 = JzTools.PonitsAverage(ptfList);
            ptfList.Clear();

            i = 0;
            while (i < seed)
            {
                PointF ptf = new PointF();

                //Find From Left Top Down
                ptf.X = (bmp.Width / 8) + i * gap;
                ptf.Y = GetLineRedInYDir(bmp, true, bmp.Height - 1, (int)ptf.X);

                ptfList.Add(ptf);

                i++;
            }
            PTF2 = JzTools.PonitsAverage(ptfList);
            ptfList.Clear();

            PTFLeft = JzTools.PointFsAverage(PTF1, PTF2);

            JzTools.DrawRect(bmp, JzTools.SimpleRect(PTFLeft), new SolidBrush(Color.Lime));

            //bmp.Save(@"D:\LOA\CROSSLEFTFOUND.BMP", ImageFormat.Bmp);


            #endregion

            #region Find Right Point

            i = 0;
            while (i < seed)
            {
                PointF ptf = new PointF();

                //Find From Right Top Down
                ptf.X = ((bmp.Width * 7) / 8) - i * gap;
                ptf.Y = GetLineRedInYDir(bmp, false, 0, (int)ptf.X);

                ptfList.Add(ptf);

                i++;
            }
            PTF1 = JzTools.PonitsAverage(ptfList);
            ptfList.Clear();

            i = 0;
            while (i < seed)
            {
                PointF ptf = new PointF();

                //Find From Left Top Down
                ptf.X = ((bmp.Width * 7) / 8) - i * gap;
                ptf.Y = GetLineRedInYDir(bmp, true, bmp.Height - 1, (int)ptf.X);

                ptfList.Add(ptf);

                i++;
            }
            PTF2 = JzTools.PonitsAverage(ptfList);
            ptfList.Clear();

            PTFRight = JzTools.PointFsAverage(PTF1, PTF2);

            JzTools.DrawRect(bmp, JzTools.SimpleRect(PTFRight), new SolidBrush(Color.Lime));

            JzTools.DrawLine(bmp, new Pen(Color.Green), PTFLeft, PTFRight);

            //bmp.Save(@"D:\LOA\CROSSRIGHTFOUND.BMP", ImageFormat.Bmp);



            #endregion

            #region Find Top Point

            i = 0;
            while (i < seed)
            {
                PointF ptf = new PointF();

                //Find From Left Top Down
                ptf.Y = (bmp.Height / 8) + i * gap;
                ptf.X = GetLineRedInXDir(bmp, false, 0, (int)ptf.Y);

                ptfList.Add(ptf);

                i++;
            }
            PTF1 = JzTools.PonitsAverage(ptfList);
            ptfList.Clear();

            i = 0;
            while (i < seed)
            {
                PointF ptf = new PointF();

                //Find From Left Top Down
                ptf.Y = (bmp.Height / 8) + i * gap;
                ptf.X = GetLineRedInXDir(bmp, true, bmp.Width - 1, (int)ptf.Y);

                ptfList.Add(ptf);

                i++;
            }
            PTF2 = JzTools.PonitsAverage(ptfList);
            ptfList.Clear();

            PTFTop = JzTools.PointFsAverage(PTF1, PTF2);

            JzTools.DrawRect(bmp, JzTools.SimpleRect(PTFTop), new SolidBrush(Color.Lime));

            //bmp.Save(@"D:\LOA\CROSSUPFOUND.BMP", ImageFormat.Bmp);

            #endregion

            #region Find Bottom Point

            i = 0;
            while (i < seed)
            {
                PointF ptf = new PointF();

                //Find From Left Top Down
                ptf.Y = ((bmp.Height * 7) / 8) - i * gap;
                ptf.X = GetLineRedInXDir(bmp, false, 0, (int)ptf.Y);

                ptfList.Add(ptf);

                i++;
            }
            PTF1 = JzTools.PonitsAverage(ptfList);
            ptfList.Clear();

            i = 0;
            while (i < seed)
            {
                PointF ptf = new PointF();

                //Find From Left Top Down
                ptf.Y = ((bmp.Height * 7) / 8) - i * gap;
                ptf.X = GetLineRedInXDir(bmp, true, bmp.Width - 1, (int)ptf.Y);

                ptfList.Add(ptf);

                i++;
            }
            PTF2 = JzTools.PonitsAverage(ptfList);
            ptfList.Clear();

            PTFBottom = JzTools.PointFsAverage(PTF1, PTF2);

            JzTools.DrawRect(bmp, JzTools.SimpleRect(PTFBottom), new SolidBrush(Color.Lime));

            JzTools.DrawLine(bmp, new Pen(Color.Green), PTFTop, PTFBottom);

            //bmp.Save(@"D:\LOA\CROSSCENTERFOUND.BMP", ImageFormat.Bmp);


            #endregion


            LineClass hline = new LineClass(PTFLeft, PTFRight);
            LineClass vline = new LineClass(PTFTop, PTFBottom);

            retPTF = hline.FindIntersection(vline);


            return retPTF;
        }

        public List<int> CenterList = new List<int>();

        public float ULocation = 0;
        public float DLocation = 0;


        public double GetBoraderLine(Bitmap bmp, bool IsXdir)
        {
            double bypass = 0;
            return GetBoraderLine(bmp, IsXdir, ref bypass);
        }
        public double GetBoraderLine(Bitmap bmp,bool IsXdir,ref double centeraverage)
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
                    byte* pucMin;

                    int xmin = rectbmp.X;
                    int ymin = rectbmp.Y;
                    int xmax = xmin + rectbmp.Width;
                    int ymax = ymin + rectbmp.Height;

                    int x = xmin;
                    int y = ymin;
                    int iStride = bmpData.Stride;

                    int LDistance = 0;
                    int RDistance = 0;

                    int Min = 0;
                    int MinLocation = 0;

                    double DistanceAvg = 0;

                    centeraverage = 0;
                    ULocation = 0;
                    DLocation = 0;

                    int xcenter = rectbmp.X + rectbmp.Width >> 1;
                    int ycenter = rectbmp.Y + rectbmp.Height >> 1;

                    List<int> DistanceList = new List<int>();

                    CenterList.Clear();

                    y = 0;

                    if (IsXdir)
                    {
                        #region Check From Width

                        while (y < rectbmp.Height)
                        {
                            x = xcenter;

                            pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                            pucPtr = pucStart;
                            pucMin = pucPtr;

                            Min = 1000;

                            while (x > -1)
                            {
                                if (pucPtr[0] == 0)
                                {
                                    pucPtr[0] = 255;
                                    pucPtr[1] = 0;
                                    pucPtr[2] = 0;
                                }
                                else
                                {
                                    if (pucPtr[0] < Min)
                                    {
                                        Min = pucPtr[0];
                                        MinLocation = x;

                                        pucMin = pucPtr;

                                        LDistance = x;
                                    }
                                }
                                pucPtr -= 4;
                                x--;
                            }

                            pucMin[0] = 0;
                            pucMin[1] = 0;
                            pucMin[2] = 255;

                            x = xcenter + 1;

                            pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                            pucPtr = pucStart;
                            pucMin = pucPtr;

                            Min = 1000;

                            while (x < rectbmp.Width)
                            {
                                if (pucPtr[0] == 0)
                                {
                                    pucPtr[0] = 0;
                                    pucPtr[1] = 255;
                                    pucPtr[2] = 0;
                                }
                                else
                                {
                                    if (pucPtr[0] < Min)
                                    {
                                        Min = pucPtr[0];
                                        MinLocation = x;

                                        pucMin = pucPtr;

                                        RDistance = x;
                                    }
                                }

                                pucPtr += 4;
                                x++;
                            }

                            pucMin[0] = 0;
                            pucMin[1] = 0;
                            pucMin[2] = 255;

                            DistanceList.Add(RDistance - LDistance);
                            DistanceAvg += (RDistance - LDistance);

                            ULocation += LDistance;
                            DLocation += RDistance;


                            CenterList.Add((RDistance + LDistance) >> 1);
                            centeraverage += ((RDistance + LDistance) >> 1);


                            pucStart += iStride;
                            y++;
                        }
                        #endregion
                    }
                    else
                    {
                        while (x < rectbmp.Width)
                        {
                            y = ycenter;

                            pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                            pucPtr = pucStart;
                            pucMin = pucPtr;

                            Min = 1000;

                            while (y > -1)
                            {
                                if (pucPtr[0] == 0)
                                {
                                    pucPtr[0] = 255;
                                    pucPtr[1] = 0;
                                    pucPtr[2] = 0;
                                }
                                else
                                {
                                    if (pucPtr[0] < Min)
                                    {
                                        Min = pucPtr[0];
                                        MinLocation = y;

                                        pucMin = pucPtr;

                                        LDistance = y;
                                    }
                                }
                                pucPtr -= iStride;
                                y--;
                            }

                            pucMin[0] = 0;
                            pucMin[1] = 0;
                            pucMin[2] = 255;

                            y = ycenter + 1;

                            pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                            pucPtr = pucStart;
                            pucMin = pucPtr;

                            Min = 1000;

                            while (y < rectbmp.Height)
                            {
                                if (pucPtr[0] == 0)
                                {

                                    pucPtr[0] = 0;
                                    pucPtr[1] = 255;
                                    pucPtr[2] = 0;
                                }
                                else
                                {
                                    if (pucPtr[0] < Min)
                                    {
                                        Min = pucPtr[0];
                                        MinLocation = y;

                                        pucMin = pucPtr;

                                        RDistance = y;
                                    }
                                }

                                pucPtr += iStride;
                                y++;
                            }

                            pucMin[0] = 0;
                            pucMin[1] = 0;
                            pucMin[2] = 255;

                            DistanceList.Add(RDistance - LDistance);
                            DistanceAvg += (RDistance - LDistance);

                            ULocation += LDistance;
                            DLocation += RDistance;

                            CenterList.Add((RDistance + LDistance) >> 1);
                            centeraverage += ((RDistance + LDistance) >> 1);

                            pucStart += 4;
                            x++;
                        }
                    }

                    DistanceAvg = DistanceAvg / (double)(DistanceList.Count);
                    centeraverage = centeraverage / (double)(DistanceList.Count);

                    ULocation = ULocation / (float)(DistanceList.Count);
                    DLocation = DLocation / (float)(DistanceList.Count);

                    bmp.UnlockBits(bmpData);

                    return DistanceAvg;
                }
            }
            catch (Exception e)
            {
                bmp.UnlockBits(bmpData);

                if (IsDebug)
                    MessageBox.Show("Error :" + e.ToString());

                return 0;
            }


        }
        public double GetBoraderLineEX(Bitmap bmp, bool IsXdir,double borderstep)
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
                    byte* pucMin;

                    int xmin = rectbmp.X;
                    int ymin = rectbmp.Y;
                    int xmax = xmin + rectbmp.Width;
                    int ymax = ymin + rectbmp.Height;

                    int x = xmin;
                    int y = ymin;
                    int iStride = bmpData.Stride;

                    int LDistance = 0;
                    int RDistance = 0;

                    int Min = 0;
                    int MinLocation = 0;

                    double DistanceAvg = 0;
                    int FirstSeed = 0;

                    ULocation = 0;
                    DLocation = 0;

                    int xcenter = rectbmp.X + rectbmp.Width >> 1;
                    int ycenter = rectbmp.Y + rectbmp.Height >> 1;

                    List<int> DistanceList = new List<int>();

                    CenterList.Clear();

                    y = 0;

                    if (IsXdir)
                    {
                        #region Check From Width

                        while (y < rectbmp.Height)
                        {
                            x = xcenter;

                            pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                            pucPtr = pucStart;
                            pucMin = pucPtr;

                            Min = 1000;

                            FirstSeed = -1;
                            LDistance = xmin;

                            while (x > -1)
                            {
                                if (pucPtr[0] == 0)
                                {
                                    pucPtr[0] = 255;
                                    pucPtr[1] = 0;
                                    pucPtr[2] = 0;
                                }
                                else
                                {
                                    if (FirstSeed == -1)
                                        FirstSeed = pucPtr[0];

                                    if (((double)pucPtr[0] / (double)FirstSeed) < borderstep)
                                    {
                                        pucMin = pucPtr;
                                        LDistance = x;

                                        break;
                                    }
                                }
                                pucPtr -= 4;
                                x--;
                            }

                            pucMin[0] = 0;
                            pucMin[1] = 0;
                            pucMin[2] = 255;

                            x = xcenter + 1;

                            pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                            pucPtr = pucStart;
                            pucMin = pucPtr;

                            Min = 1000;

                            FirstSeed = -1;
                            RDistance = xmax;

                            while (x < rectbmp.Width)
                            {
                                if (pucPtr[0] == 0)
                                {
                                    pucPtr[0] = 0;
                                    pucPtr[1] = 255;
                                    pucPtr[2] = 0;
                                }
                                else
                                {
                                    if (FirstSeed == -1)
                                        FirstSeed = pucPtr[0];

                                    if (((double)pucPtr[0] / (double)FirstSeed) < borderstep)
                                    {
                                        pucMin = pucPtr;
                                        RDistance = x;

                                        break;
                                    }

                                }

                                pucPtr += 4;
                                x++;
                            }

                            pucMin[0] = 0;
                            pucMin[1] = 0;
                            pucMin[2] = 255;

                            DistanceList.Add(RDistance - LDistance);
                            DistanceAvg += (RDistance - LDistance);

                            ULocation += LDistance;
                            DLocation += RDistance;


                            CenterList.Add((RDistance + LDistance) >> 1);

                            pucStart += iStride;
                            y++;
                        }
                        #endregion
                    }
                    else
                    {
                        while (x < rectbmp.Width)
                        {
                            y = ycenter;

                            pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                            pucPtr = pucStart;
                            pucMin = pucPtr;

                            Min = 1000;

                            FirstSeed = -1;
                            LDistance = ymin;

                            while (y > -1)
                            {
                                if (pucPtr[0] == 0)
                                {
                                    pucPtr[0] = 255;
                                    pucPtr[1] = 0;
                                    pucPtr[2] = 0;
                                }
                                else
                                {
                                    if (FirstSeed == -1)
                                        FirstSeed = pucPtr[0];

                                    if (((double)pucPtr[0] / (double)FirstSeed) < borderstep)
                                    {
                                        pucMin = pucPtr;
                                        LDistance = y;

                                        break;
                                    }
                                }
                                pucPtr -= iStride;
                                y--;
                            }

                            pucMin[0] = 0;
                            pucMin[1] = 0;
                            pucMin[2] = 255;

                            y = ycenter + 1;

                            pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                            pucPtr = pucStart;
                            pucMin = pucPtr;

                            Min = 1000;
                            FirstSeed = -1;
                            RDistance = ymax;

                            while (y < rectbmp.Height)
                            {
                                if (pucPtr[0] == 0)
                                {

                                    pucPtr[0] = 0;
                                    pucPtr[1] = 255;
                                    pucPtr[2] = 0;
                                }
                                else
                                {
                                    if (FirstSeed == -1)
                                        FirstSeed = pucPtr[0];

                                    if (((double)pucPtr[0] / (double)FirstSeed) < borderstep)
                                    {
                                        pucMin = pucPtr;
                                        RDistance = y;

                                        break;
                                    }

                                }

                                pucPtr += iStride;
                                y++;
                            }

                            pucMin[0] = 0;
                            pucMin[1] = 0;
                            pucMin[2] = 255;

                            DistanceList.Add(RDistance - LDistance);
                            DistanceAvg += (RDistance - LDistance);

                            ULocation += LDistance;
                            DLocation += RDistance;

                            CenterList.Add((RDistance + LDistance) >> 1);
                            pucStart += 4;
                            x++;
                        }
                    }

                    DistanceAvg = DistanceAvg / (double)(DistanceList.Count);

                    ULocation = ULocation / (float)(DistanceList.Count);
                    DLocation = DLocation / (float)(DistanceList.Count);

                    bmp.UnlockBits(bmpData);

                    return DistanceAvg;
                }
            }
            catch (Exception e)
            {
                bmp.UnlockBits(bmpData);

                if (IsDebug)
                    MessageBox.Show("Error :" + e.ToString());

                return 0;
            }


        }
        public double GetBoraderLineEX(Bitmap bmp, bool IsXdir, ref double centeraverage, bool IsBlack, double checkingratio)
        {
            lock (obj)
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
                        byte* pucMin;

                        int xmin = rectbmp.X;
                        int ymin = rectbmp.Y;
                        int xmax = xmin + rectbmp.Width;
                        int ymax = ymin + rectbmp.Height;

                        int x = xmin;
                        int y = ymin;
                        int iStride = bmpData.Stride;

                        int LDistance = 0;
                        int RDistance = 0;

                        int Min = 0;
                        int MinLocation = 0;
                        int FirstSeed = 0;

                        double DistanceAvg = 0;
                        centeraverage = 0;
                        ULocation = 0;
                        DLocation = 0;

                        int xcenter = rectbmp.X + rectbmp.Width >> 1;
                        int ycenter = rectbmp.Y + rectbmp.Height >> 1;

                        List<int> DistanceList = new List<int>();

                        CenterList.Clear();

                        y = 0;

                        if (IsXdir)
                        {
                            #region Check From Width

                            while (y < rectbmp.Height)
                            {
                                x = xcenter;

                                pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                                pucPtr = pucStart;
                                pucMin = pucPtr;

                                Min = 1000;

                                FirstSeed = -1;
                                LDistance = xmin;

                                while (x > -1)
                                {
                                    if (pucPtr[0] == 0)
                                    {
                                        pucPtr[0] = 255;
                                        pucPtr[1] = 0;
                                        pucPtr[2] = 0;
                                    }
                                    else
                                    {
                                        if (FirstSeed == -1)
                                            FirstSeed = pucPtr[0];

                                        if (IsBlack)
                                        {
                                            if (((double)pucPtr[0] / (double)FirstSeed) < 1 - checkingratio)
                                            {
                                                pucMin = pucPtr;
                                                LDistance = x;

                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (((double)pucPtr[0] / (double)FirstSeed) > 1 + checkingratio)
                                            {
                                                pucMin = pucPtr;
                                                LDistance = x;

                                                break;
                                            }
                                        }

                                    }
                                    pucPtr -= 4;
                                    x--;
                                }

                                pucMin[0] = 0;
                                pucMin[1] = 0;
                                pucMin[2] = 255;

                                x = xcenter + 1;

                                pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                                pucPtr = pucStart;
                                pucMin = pucPtr;

                                Min = 1000;

                                FirstSeed = -1;
                                RDistance = xmax;

                                while (x < rectbmp.Width)
                                {
                                    if (pucPtr[0] == 0)
                                    {
                                        pucPtr[0] = 0;
                                        pucPtr[1] = 255;
                                        pucPtr[2] = 0;
                                    }
                                    else
                                    {
                                        if (FirstSeed == -1)
                                            FirstSeed = pucPtr[0];

                                        if (IsBlack)
                                        {
                                            if (((double)pucPtr[0] / (double)FirstSeed) < 1 - checkingratio)
                                            {
                                                pucMin = pucPtr;
                                                RDistance = x;

                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (((double)pucPtr[0] / (double)FirstSeed) > 1 + checkingratio)
                                            {
                                                pucMin = pucPtr;
                                                RDistance = x;

                                                break;
                                            }
                                        }
                                    }

                                    pucPtr += 4;
                                    x++;
                                }

                                pucMin[0] = 0;
                                pucMin[1] = 0;
                                pucMin[2] = 255;

                                DistanceList.Add(RDistance - LDistance);
                                DistanceAvg += (RDistance - LDistance);

                                ULocation += LDistance;
                                DLocation += RDistance;


                                CenterList.Add((RDistance + LDistance) >> 1);
                                centeraverage += ((RDistance + LDistance) >> 1);


                                pucStart += iStride;
                                y++;
                            }
                            #endregion
                        }
                        else
                        {
                            while (x < rectbmp.Width)
                            {
                                y = ycenter;

                                pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                                pucPtr = pucStart;
                                pucMin = pucPtr;

                                Min = 1000;

                                FirstSeed = -1;
                                LDistance = ymin;

                                while (y > -1)
                                {
                                    if (pucPtr[0] == 0)
                                    {
                                        pucPtr[0] = 255;
                                        pucPtr[1] = 0;
                                        pucPtr[2] = 0;
                                    }
                                    else
                                    {
                                        if (FirstSeed == -1)
                                            FirstSeed = pucPtr[0];

                                        if (IsBlack)
                                        {
                                            if (((double)pucPtr[0] / (double)FirstSeed) < 1 - checkingratio)
                                            {
                                                pucMin = pucPtr;
                                                LDistance = y;

                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (((double)pucPtr[0] / (double)FirstSeed) > 1 + checkingratio)
                                            {
                                                pucMin = pucPtr;
                                                LDistance = y;

                                                break;
                                            }
                                        }
                                    }
                                    pucPtr -= iStride;
                                    y--;
                                }

                                pucMin[0] = 0;
                                pucMin[1] = 0;
                                pucMin[2] = 255;

                                y = ycenter + 1;

                                pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                                pucPtr = pucStart;
                                pucMin = pucPtr;

                                Min = 1000;

                                FirstSeed = -1;
                                RDistance = ymax;

                                while (y < rectbmp.Height)
                                {
                                    if (pucPtr[0] == 0)
                                    {

                                        pucPtr[0] = 0;
                                        pucPtr[1] = 255;
                                        pucPtr[2] = 0;
                                    }
                                    else
                                    {
                                        if (FirstSeed == -1)
                                            FirstSeed = pucPtr[0];

                                        if (IsBlack)
                                        {
                                            if (((double)pucPtr[0] / (double)FirstSeed) < 1 - checkingratio)
                                            {
                                                pucMin = pucPtr;
                                                RDistance = y;

                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (((double)pucPtr[0] / (double)FirstSeed) > 1 + checkingratio)
                                            {
                                                pucMin = pucPtr;
                                                RDistance = y;

                                                break;
                                            }
                                        }
                                    }

                                    pucPtr += iStride;
                                    y++;
                                }

                                pucMin[0] = 0;
                                pucMin[1] = 0;
                                pucMin[2] = 255;

                                DistanceList.Add(RDistance - LDistance);
                                DistanceAvg += (RDistance - LDistance);

                                ULocation += LDistance;
                                DLocation += RDistance;

                                CenterList.Add((RDistance + LDistance) >> 1);
                                centeraverage += ((RDistance + LDistance) >> 1);

                                pucStart += 4;
                                x++;
                            }
                        }

                        DistanceAvg = DistanceAvg / (double)(DistanceList.Count);
                        centeraverage = centeraverage / (double)(DistanceList.Count);

                        ULocation = ULocation / (float)(DistanceList.Count);
                        DLocation = DLocation / (float)(DistanceList.Count);

                        bmp.UnlockBits(bmpData);

                        return DistanceAvg;
                    }
                }
                catch (Exception e)
                {
                    bmp.UnlockBits(bmpData);

                    if (IsDebug)
                        MessageBox.Show("Error :" + e.ToString());

                    return 0;
                }
            }
        }

        public PointF GetBoraderPoint(Bitmap bmp, bool IsXdir, bool IsReverse)
        {
            return GetBoraderPoint(bmp, IsXdir, IsReverse, false);
        }
        public PointF GetBoraderPoint(Bitmap bmp, bool IsXdir,bool IsReverse,bool isover)
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
                    byte* pucMin;

                    int xmin = rectbmp.X;
                    int ymin = rectbmp.Y;
                    int xmax = xmin + rectbmp.Width;
                    int ymax = ymin + rectbmp.Height;

                    int x = xmin;
                    int y = ymin;
                    int iStride = bmpData.Stride;

                    PointF ptf = new PointF();

                    float Avaerage = 0;

                    bool IsStage2 = false;
                    
                    if (IsXdir)
                    {
                        if (!IsReverse)
                        {
                            #region Check From Left White

                            while (y < ymax)
                            {
                                x = xmin;

                                pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                                pucPtr = pucStart;
                                pucMin = pucPtr;

                                IsStage2 = false;

                                while (x < xmax)
                                {
                                    if (!isover)
                                    {
                                        if (pucPtr[0] == 0)
                                        {
                                            pucPtr[0] = 0;
                                            pucPtr[1] = 0;
                                            pucPtr[2] = 255;

                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (pucPtr[0] == 0 && !IsStage2)
                                        {
                                            //pucPtr[0] = 0;
                                            //pucPtr[1] = 0;
                                            //pucPtr[2] = 255;
                                            IsStage2 = true;
                                        }
                                        else if (pucPtr[0] == 255 && IsStage2)
                                        {
                                            pucPtr[0] = 0;
                                            pucPtr[1] = 0;
                                            pucPtr[2] = 255;

                                            break;
                                        }
                                    }

                                    pucPtr += 4;
                                    x++;
                                }

                                x--;
                                Avaerage += x;

                                pucStart += iStride;
                                y++;
                            }

                            Avaerage = Avaerage / (float)ymax;

                            ptf.X = Avaerage;
                            ptf.Y = (ymax - ymin) >> 2;

                            #endregion
                        }
                        else
                        {
                            #region Check From Right White

                            while (y < ymax)
                            {
                                x = xmax - 1;

                                pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                                pucPtr = pucStart;
                                pucMin = pucPtr;

                                IsStage2 = false;

                                while (x > -1)
                                {
                                    //if (pucPtr[0] == 0)
                                    //{
                                    //    pucPtr[0] = 0;
                                    //    pucPtr[1] = 0;
                                    //    pucPtr[2] = 255;

                                    //    break;
                                    //}

                                    if (!isover)
                                    {
                                        if (pucPtr[0] == 0)
                                        {
                                            pucPtr[0] = 0;
                                            pucPtr[1] = 0;
                                            pucPtr[2] = 255;

                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (pucPtr[0] == 0 && !IsStage2)
                                        {
                                            //pucPtr[0] = 0;
                                            //pucPtr[1] = 0;
                                            //pucPtr[2] = 255;
                                            IsStage2 = true;
                                        }
                                        else if (pucPtr[0] == 255 && IsStage2)
                                        {
                                            pucPtr[0] = 0;
                                            pucPtr[1] = 0;
                                            pucPtr[2] = 255;

                                            break;
                                        }
                                    }
                                    pucPtr -= 4;
                                    x--;
                                }

                                x++;
                                Avaerage += x;

                                pucStart += iStride;
                                y++;
                            }

                            Avaerage = Avaerage / (float)ymax;

                            ptf.X = Avaerage;
                            ptf.Y = (ymax - ymin) >> 2;

                            #endregion
                        }
                    }
                    else
                    {
                        if (!IsReverse)
                        {
                            #region Check From Top White

                            while (x < xmax)
                            {
                                y = ymin;

                                pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                                pucPtr = pucStart;
                                pucMin = pucPtr;

                                IsStage2 = false;

                                while (y < ymax)
                                {
                                    //if (pucPtr[0] == 0)
                                    //{
                                    //    pucPtr[0] = 0;
                                    //    pucPtr[1] = 0;
                                    //    pucPtr[2] = 255;

                                    //    break;
                                    //}
                                    if (!isover)
                                    {
                                        if (pucPtr[0] == 0)
                                        {
                                            pucPtr[0] = 0;
                                            pucPtr[1] = 0;
                                            pucPtr[2] = 255;

                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (pucPtr[0] == 0 && !IsStage2)
                                        {
                                            //pucPtr[0] = 0;
                                            //pucPtr[1] = 0;
                                            //pucPtr[2] = 255;
                                            IsStage2 = true;
                                        }
                                        else if (pucPtr[0] == 255 && IsStage2)
                                        {
                                            pucPtr[0] = 0;
                                            pucPtr[1] = 0;
                                            pucPtr[2] = 255;

                                            break;
                                        }
                                    }

                                    pucPtr += iStride;
                                    y++;
                                }

                                y--;
                                Avaerage += y;

                                pucStart += 4;
                                x++;
                            }

                            Avaerage = Avaerage / (float)xmax;

                            ptf.X = (xmax - xmin) >> 2;
                            ptf.Y = Avaerage;

                            #endregion
                        }
                        else
                        {
                            #region Check From Bottom White

                            while (x < xmax)
                            {
                                y = ymax - 1;

                                pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                                pucPtr = pucStart;
                                pucMin = pucPtr;

                                IsStage2 = false;

                                while (y > -1)
                                {
                                    //if (pucPtr[0] == 0)
                                    //{
                                    //    pucPtr[0] = 0;
                                    //    pucPtr[1] = 0;
                                    //    pucPtr[2] = 255;

                                    //    break;
                                    //}

                                    if (!isover)
                                    {
                                        if (pucPtr[0] == 0)
                                        {
                                            pucPtr[0] = 0;
                                            pucPtr[1] = 0;
                                            pucPtr[2] = 255;

                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (pucPtr[0] == 0 && !IsStage2)
                                        {
                                            //pucPtr[0] = 0;
                                            //pucPtr[1] = 0;
                                            //pucPtr[2] = 255;
                                            IsStage2 = true;
                                        }
                                        else if (pucPtr[0] == 255 && IsStage2)
                                        {
                                            pucPtr[0] = 0;
                                            pucPtr[1] = 0;
                                            pucPtr[2] = 255;

                                            break;
                                        }
                                    }

                                    pucPtr -= iStride;
                                    y--;
                                }

                                y++;
                                Avaerage += y;

                                pucStart += 4;
                                x++;
                            }

                            Avaerage = Avaerage / (float)xmax;

                            ptf.X = (xmax - xmin) >> 2;
                            ptf.Y = Avaerage;

                            #endregion
                        }
                    }

                    return ptf;
                }
            }
            catch (Exception e)
            {
                bmp.UnlockBits(bmpData);

                if (IsDebug)
                    MessageBox.Show("Error :" + e.ToString());

                return new PointF();
            }


        }
        public PointF GetBoraderPoint(Bitmap bmp, bool IsXdir, bool IsReverse,int diffratio)
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
                    byte* pucMin;

                    int xmin = rectbmp.X;
                    int ymin = rectbmp.Y;
                    int xmax = xmin + rectbmp.Width;
                    int ymax = ymin + rectbmp.Height;

                    int x = xmin;
                    int y = ymin;
                    int iStride = bmpData.Stride;


                    int StartColor = -1;
                    int CutColor = 0;

                    PointF ptf = new PointF();

                    float Avaerage = 0;

                    if (IsXdir)
                    {
                        if (!IsReverse)
                        {
                            #region Check From Left White

                            while (y < ymax)
                            {
                                x = xmin;

                                pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                                pucPtr = pucStart;
                                pucMin = pucPtr;

                                StartColor = -1;

                                while (x < xmax)
                                {
                                    if (pucPtr[0] != 255 && StartColor == -1)
                                    //if (StartColor == -1)
                                    {
                                        StartColor = pucPtr[0];
                                        CutColor = (int)((double)StartColor * (double)diffratio / 100d);
                                    }

                                    if (pucPtr[0] < CutColor && StartColor != -1)
                                    {
                                        pucPtr[0] = 0;
                                        pucPtr[1] = 0;
                                        pucPtr[2] = 255;

                                        break;
                                    }
                                    pucPtr += 4;
                                    x++;
                                }

                                x--;
                                Avaerage += x;

                                pucStart += iStride;
                                y++;
                            }

                            Avaerage = Avaerage / (float)ymax;

                            ptf.X = Avaerage;
                            ptf.Y = (ymax - ymin) >> 2;

                            #endregion
                        }
                        else
                        {
                            #region Check From Right White

                            while (y < ymax)
                            {
                                x = xmax - 1;

                                pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                                pucPtr = pucStart;
                                pucMin = pucPtr;

                                StartColor = -1;

                                while (x > -1)
                                {
                                    if (pucPtr[0] != 255 && StartColor == -1)
                                    //if (StartColor == -1)
                                    {
                                        StartColor = pucPtr[0];
                                        CutColor = (int)((double)StartColor * (double)diffratio / 100d);
                                    }

                                    if (pucPtr[0] < CutColor && StartColor != -1)
                                    {
                                        pucPtr[0] = 0;
                                        pucPtr[1] = 0;
                                        pucPtr[2] = 255;

                                        break;
                                    }
                                    pucPtr -= 4;
                                    x--;
                                }

                                x++;
                                Avaerage += x;

                                pucStart += iStride;
                                y++;
                            }

                            Avaerage = Avaerage / (float)ymax;

                            ptf.X = Avaerage;
                            ptf.Y = (ymax - ymin) >> 2;

                            #endregion
                        }
                    }
                    else
                    {
                        if (!IsReverse)
                        {
                            #region Check From Top White

                            while (x < xmax)
                            {
                                y = ymin;

                                pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                                pucPtr = pucStart;
                                pucMin = pucPtr;

                                StartColor = -1;

                                while (y < ymax)
                                {
                                    if (pucPtr[0] != 255 && StartColor == -1)
                                    //if (StartColor == -1)
                                    {
                                        StartColor = pucPtr[0];
                                        CutColor = (int)((double)StartColor * (double)diffratio / 100d);
                                    }

                                    if (pucPtr[0] < CutColor && StartColor != -1)
                                    {
                                        pucPtr[0] = 0;
                                        pucPtr[1] = 0;
                                        pucPtr[2] = 255;

                                        break;
                                    }

                                    pucPtr += iStride;
                                    y++;
                                }

                                y--;
                                Avaerage += y;

                                pucStart += 4;
                                x++;
                            }

                            Avaerage = Avaerage / (float)xmax;

                            ptf.X = (xmax - xmin) >> 2;
                            ptf.Y = Avaerage;

                            #endregion
                        }
                        else
                        {
                            #region Check From Bottom White

                            while (x < xmax)
                            {
                                y = ymax - 1;

                                pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                                pucPtr = pucStart;
                                pucMin = pucPtr;

                                StartColor = -1;

                                while (y > -1)
                                {
                                    if (pucPtr[0] != 255 && StartColor == -1)
                                    //if (StartColor == -1)
                                    {
                                        StartColor = pucPtr[0];
                                        CutColor = (int)((double)StartColor * (double)diffratio / 100d);
                                    }

                                    if (pucPtr[0] < CutColor && StartColor != -1)
                                    {
                                        pucPtr[0] = 0;
                                        pucPtr[1] = 0;
                                        pucPtr[2] = 255;

                                        break;
                                    }

                                    pucPtr -= iStride;
                                    y--;
                                }

                                y++;
                                Avaerage += y;

                                pucStart += 4;
                                x++;
                            }

                            Avaerage = Avaerage / (float)xmax;

                            ptf.X = (xmax - xmin) >> 2;
                            ptf.Y = Avaerage;

                            #endregion
                        }
                    }

                    return ptf;
                }
            }
            catch (Exception e)
            {
                bmp.UnlockBits(bmpData);

                if (IsDebug)
                    MessageBox.Show("Error :" + e.ToString());

                return new PointF();
            }


        }

        public void GetDevideBorder(Bitmap bmp, int devidevalue, Rectangle rectmax, ref List<Point> ptslist, Color colorvalue)
        {
            Point ptcenter = JzTools.GetRectCenter(rectmax);
            uint ColorValue = (uint)((colorvalue.A << 24) + (colorvalue.R << 16) + (colorvalue.G << 8) + colorvalue.B);

            Rectangle rectbmp = JzTools.SimpleRect(bmp.Size);
            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            IntPtr Scan0 = bmpData.Scan0;

            ptslist.Clear();

            try
            {
                unsafe
                {
                    byte* scan0 = (byte*)(void*)Scan0;
                    byte* pucStart;
                    byte* pucPtr;
                    byte* pucMin;

                    int xmin = rectbmp.X;
                    int ymin = rectbmp.Y;
                    int xmax = xmin + rectbmp.Width;
                    int ymax = ymin + rectbmp.Height;

                    int x = ptcenter.X;
                    int y = ptcenter.Y;

                    int smallY = rectmax.Y;
                    int bigY = rectmax.Y + rectmax.Height - 1;
                    int gap = (int)((float)(bigY - smallY) / (float)devidevalue);

                    int iStride = bmpData.Stride;

                    y = smallY + gap;

                    while (y < bigY)
                    {
                        x = xmin;

                        pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                        pucPtr = pucStart;
                        pucMin = pucPtr;

                        //StartColor = -1;

                        while (x < xmax)
                        {
                            if (pucPtr[0] == colorvalue.B && pucPtr[1] == colorvalue.G && pucPtr[2] == colorvalue.R)
                            {
                                ptslist.Add(new Point(x,y));
                                break;
                            }
                            pucPtr += 4;
                            x++;
                        }

                        x = xmax -1;

                        pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                        pucPtr = pucStart;
                        pucMin = pucPtr;

                        while (x > xmin)
                        {
                            if (pucPtr[0] == colorvalue.B && pucPtr[1] == colorvalue.G && pucPtr[2] == colorvalue.R)
                            {
                                ptslist.Add(new Point(x, y));
                                break;
                            }
                            pucPtr -= 4;
                            x--;
                        }

                        //pucStart += iStride;
                        y+= gap;
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

    class FindCornerClass
    {
        List<int> XWay = new List<int>();
        List<int> YWay = new List<int>();
        bool IsDebug
        {
            get
            {
                return Universal.IsDebug;
            }
        }

        public Point EndPoint = new Point();
        public int CutLength = 0;
        public FindCornerClass()
        {


        }
        public void Reset()
        {
            XWay.Clear();
            YWay.Clear();
            JzTools.ClearPoint(ref EndPoint);
        }
    }

    public class ConvolutionMatrix
    {
        public int MatrixSize = 3;

        public double[,] Matrix;
        public double Factor = 1;
        public double Offset = 1;

        public ConvolutionMatrix(int size)
        {
            MatrixSize = 3;
            Matrix = new double[size, size];
        }

        public void SetAll(double value)
        {
            for (int i = 0; i < MatrixSize; i++)
            {
                for (int j = 0; j < MatrixSize; j++)
                {
                    Matrix[i, j] = value;
                }
            }
        }
    }
    public class ImageProcessor
    {
        private Bitmap bitmapImage = new Bitmap(1, 1);

        public void SetImage(string path)
        {
            bitmapImage = new Bitmap(path);
        }
        public void SetImage(Bitmap bmp)
        {
            bitmapImage.Dispose();
            bitmapImage = new Bitmap(bmp);
        }
        public Bitmap GetImage()
        {
            return bitmapImage;
        }
        public void ApplyInvert()
        {
            byte A, R, G, B;
            Color pixelColor;

            for (int y = 0; y < bitmapImage.Height; y++)
            {
                for (int x = 0; x < bitmapImage.Width; x++)
                {
                    pixelColor = bitmapImage.GetPixel(x, y);
                    A = pixelColor.A;
                    R = (byte)(255 - pixelColor.R);
                    G = (byte)(255 - pixelColor.G);
                    B = (byte)(255 - pixelColor.B);
                    bitmapImage.SetPixel(x, y, Color.FromArgb((int)A, (int)R, (int)G, (int)B));
                }
            }

        }

        public void ApplyGreyscale()
        {
            byte A, R, G, B;
            Color pixelColor;

            for (int y = 0; y < bitmapImage.Height; y++)
            {
                for (int x = 0; x < bitmapImage.Width; x++)
                {
                    pixelColor = bitmapImage.GetPixel(x, y);
                    A = pixelColor.A;
                    R = (byte)((0.299 * pixelColor.R) + (0.587 * pixelColor.G) + (0.114 * pixelColor.B));
                    G = B = R;

                    bitmapImage.SetPixel(x, y, Color.FromArgb((int)A, (int)R, (int)G, (int)B));
                }
            }

        }

        public void ApplyGamma(double r, double g, double b)
        {
            byte A, R, G, B;
            Color pixelColor;

            byte[] redGamma = new byte[256];
            byte[] greenGamma = new byte[256];
            byte[] blueGamma = new byte[256];

            for (int i = 0; i < 256; ++i)
            {
                redGamma[i] = (byte)Math.Min(255, (int)((255.0
                    * Math.Pow(i / 255.0, 1.0 / r)) + 0.5));
                greenGamma[i] = (byte)Math.Min(255, (int)((255.0
                    * Math.Pow(i / 255.0, 1.0 / g)) + 0.5));
                blueGamma[i] = (byte)Math.Min(255, (int)((255.0
                    * Math.Pow(i / 255.0, 1.0 / b)) + 0.5));
            }

            for (int y = 0; y < bitmapImage.Height; y++)
            {
                for (int x = 0; x < bitmapImage.Width; x++)
                {
                    pixelColor = bitmapImage.GetPixel(x, y);
                    A = pixelColor.A;
                    R = redGamma[pixelColor.R];
                    G = greenGamma[pixelColor.G];
                    B = blueGamma[pixelColor.B];
                    bitmapImage.SetPixel(x, y, Color.FromArgb((int)A, (int)R, (int)G, (int)B));
                }
            }
        }

        public void ApplyColorFilter(double r, double g, double b)
        {
            byte A, R, G, B;
            Color pixelColor;

            for (int y = 0; y < bitmapImage.Height; y++)
            {
                for (int x = 0; x < bitmapImage.Width; x++)
                {
                    pixelColor = bitmapImage.GetPixel(x, y);
                    A = pixelColor.A;
                    R = (byte)(pixelColor.R * r);
                    G = (byte)(pixelColor.G * g);
                    B = (byte)(pixelColor.B * b);
                    bitmapImage.SetPixel(x, y, Color.FromArgb((int)A, (int)R, (int)G, (int)B));
                }
            }
        }

        public void ApplySepia(int depth)
        {
            int A, R, G, B;
            Color pixelColor;

            for (int y = 0; y < bitmapImage.Height; y++)
            {
                for (int x = 0; x < bitmapImage.Width; x++)
                {
                    pixelColor = bitmapImage.GetPixel(x, y);
                    A = pixelColor.A;
                    R = (int)((0.299 * pixelColor.R) + (0.587 * pixelColor.G) + (0.114 * pixelColor.B));
                    G = B = R;

                    R += (depth * 2);
                    if (R > 255)
                    {
                        R = 255;
                    }
                    G += depth;
                    if (G > 255)
                    {
                        G = 255;
                    }

                    bitmapImage.SetPixel(x, y, Color.FromArgb(A, R, G, B));
                }
            }
        }

        public void ApplyDecreaseColourDepth(int offset)
        {
            int A, R, G, B;
            Color pixelColor;

            for (int y = 0; y < bitmapImage.Height; y++)
            {
                for (int x = 0; x < bitmapImage.Width; x++)
                {
                    pixelColor = bitmapImage.GetPixel(x, y);
                    A = pixelColor.A;
                    R = ((pixelColor.R + (offset / 2)) - ((pixelColor.R + (offset / 2)) % offset) - 1);
                    if (R < 0)
                    {
                        R = 0;
                    }
                    G = ((pixelColor.G + (offset / 2)) - ((pixelColor.G + (offset / 2)) % offset) - 1);
                    if (G < 0)
                    {
                        G = 0;
                    }
                    B = ((pixelColor.B + (offset / 2)) - ((pixelColor.B + (offset / 2)) % offset) - 1);
                    if (B < 0)
                    {
                        B = 0;
                    }
                    bitmapImage.SetPixel(x, y, Color.FromArgb(A, R, G, B));
                }
            }

        }

        public void ApplyContrast(double contrast)
        {
            double A, R, G, B;

            Color pixelColor;

            contrast = (100.0 + contrast) / 100.0;
            contrast *= contrast;

            for (int y = 0; y < bitmapImage.Height; y++)
            {
                for (int x = 0; x < bitmapImage.Width; x++)
                {
                    pixelColor = bitmapImage.GetPixel(x, y);
                    A = pixelColor.A;

                    R = pixelColor.R / 255.0;
                    R -= 0.5;
                    R *= contrast;
                    R += 0.5;
                    R *= 255;

                    if (R > 255)
                    {
                        R = 255;
                    }
                    else if (R < 0)
                    {
                        R = 0;
                    }

                    G = pixelColor.G / 255.0;
                    G -= 0.5;
                    G *= contrast;
                    G += 0.5;
                    G *= 255;
                    if (G > 255)
                    {
                        G = 255;
                    }
                    else if (G < 0)
                    {
                        G = 0;
                    }

                    B = pixelColor.B / 255.0;
                    B -= 0.5;
                    B *= contrast;
                    B += 0.5;
                    B *= 255;
                    if (B > 255)
                    {
                        B = 255;
                    }
                    else if (B < 0)
                    {
                        B = 0;
                    }

                    bitmapImage.SetPixel(x, y, Color.FromArgb((int)A, (int)R, (int)G, (int)B));
                }
            }

        }

        public void ApplyBrightness(int brightness)
        {
            int A, R, G, B;
            Color pixelColor;

            for (int y = 0; y < bitmapImage.Height; y++)
            {
                for (int x = 0; x < bitmapImage.Width; x++)
                {
                    pixelColor = bitmapImage.GetPixel(x, y);
                    A = pixelColor.A;
                    R = pixelColor.R + brightness;
                    if (R > 255)
                    {
                        R = 255;
                    }
                    else if (R < 0)
                    {
                        R = 0;
                    }

                    G = pixelColor.G + brightness;
                    if (G > 255)
                    {
                        G = 255;
                    }
                    else if (G < 0)
                    {
                        G = 0;
                    }

                    B = pixelColor.B + brightness;
                    if (B > 255)
                    {
                        B = 255;
                    }
                    else if (B < 0)
                    {
                        B = 0;
                    }

                    bitmapImage.SetPixel(x, y, Color.FromArgb(A, R, G, B));
                }
            }

        }

        public void ApplySmooth(double weight)
        {
            ConvolutionMatrix matrix = new ConvolutionMatrix(3);
            matrix.SetAll(1);
            matrix.Matrix[1, 1] = weight;
            matrix.Factor = weight + 8;
            bitmapImage = Convolution3x3(bitmapImage, matrix);

        }

        public void ApplyGaussianBlur(double peakValue)
        {
            ConvolutionMatrix matrix = new ConvolutionMatrix(3);
            matrix.SetAll(1);
            matrix.Matrix[0, 0] = peakValue / 4;
            matrix.Matrix[1, 0] = peakValue / 2;
            matrix.Matrix[2, 0] = peakValue / 4;
            matrix.Matrix[0, 1] = peakValue / 2;
            matrix.Matrix[1, 1] = peakValue;
            matrix.Matrix[2, 1] = peakValue / 2;
            matrix.Matrix[0, 2] = peakValue / 4;
            matrix.Matrix[1, 2] = peakValue / 2;
            matrix.Matrix[2, 2] = peakValue / 4;
            matrix.Factor = peakValue * 4;
            bitmapImage = Convolution3x3(bitmapImage, matrix);

        }

        public void ApplySharpen(double weight)
        {
            ConvolutionMatrix matrix = new ConvolutionMatrix(3);
            matrix.SetAll(1);
            matrix.Matrix[0, 0] = 0;
            matrix.Matrix[1, 0] = -2;
            matrix.Matrix[2, 0] = 0;
            matrix.Matrix[0, 1] = -2;
            matrix.Matrix[1, 1] = weight;
            matrix.Matrix[2, 1] = -2;
            matrix.Matrix[0, 2] = 0;
            matrix.Matrix[1, 2] = -2;
            matrix.Matrix[2, 2] = 0;
            matrix.Factor = weight - 8;
            bitmapImage = Convolution3x3(bitmapImage, matrix);

        }

        public void ApplyMeanRemoval(double weight)
        {
            ConvolutionMatrix matrix = new ConvolutionMatrix(3);
            matrix.SetAll(1);
            matrix.Matrix[0, 0] = -1;
            matrix.Matrix[1, 0] = -1;
            matrix.Matrix[2, 0] = -1;
            matrix.Matrix[0, 1] = -1;
            matrix.Matrix[1, 1] = weight;
            matrix.Matrix[2, 1] = -1;
            matrix.Matrix[0, 2] = -1;
            matrix.Matrix[1, 2] = -1;
            matrix.Matrix[2, 2] = -1;
            matrix.Factor = weight - 8;
            bitmapImage = Convolution3x3(bitmapImage, matrix);

        }

        public void ApplyEmboss(double weight)
        {
            ConvolutionMatrix matrix = new ConvolutionMatrix(3);
            matrix.SetAll(1);
            matrix.Matrix[0, 0] = -1;
            matrix.Matrix[1, 0] = 0;
            matrix.Matrix[2, 0] = -1;
            matrix.Matrix[0, 1] = 0;
            matrix.Matrix[1, 1] = weight;
            matrix.Matrix[2, 1] = 0;
            matrix.Matrix[0, 2] = -1;
            matrix.Matrix[1, 2] = 0;
            matrix.Matrix[2, 2] = -1;
            matrix.Factor = 4;
            matrix.Offset = 127;
            bitmapImage = Convolution3x3(bitmapImage, matrix);

        }

        public Bitmap Convolution3x3(Bitmap b, ConvolutionMatrix m)
        {
            Bitmap newImg = (Bitmap)b.Clone();
            Color[,] pixelColor = new Color[3, 3];
            int A, R, G, B;

            for (int y = 0; y < b.Height - 2; y++)
            {
                for (int x = 0; x < b.Width - 2; x++)
                {
                    pixelColor[0, 0] = b.GetPixel(x, y);
                    pixelColor[0, 1] = b.GetPixel(x, y + 1);
                    pixelColor[0, 2] = b.GetPixel(x, y + 2);
                    pixelColor[1, 0] = b.GetPixel(x + 1, y);
                    pixelColor[1, 1] = b.GetPixel(x + 1, y + 1);
                    pixelColor[1, 2] = b.GetPixel(x + 1, y + 2);
                    pixelColor[2, 0] = b.GetPixel(x + 2, y);
                    pixelColor[2, 1] = b.GetPixel(x + 2, y + 1);
                    pixelColor[2, 2] = b.GetPixel(x + 2, y + 2);

                    A = pixelColor[1, 1].A;

                    R = (int)((((pixelColor[0, 0].R * m.Matrix[0, 0]) +
                                 (pixelColor[1, 0].R * m.Matrix[1, 0]) +
                                 (pixelColor[2, 0].R * m.Matrix[2, 0]) +
                                 (pixelColor[0, 1].R * m.Matrix[0, 1]) +
                                 (pixelColor[1, 1].R * m.Matrix[1, 1]) +
                                 (pixelColor[2, 1].R * m.Matrix[2, 1]) +
                                 (pixelColor[0, 2].R * m.Matrix[0, 2]) +
                                 (pixelColor[1, 2].R * m.Matrix[1, 2]) +
                                 (pixelColor[2, 2].R * m.Matrix[2, 2]))
                                        / m.Factor) + m.Offset);

                    if (R < 0)
                    {
                        R = 0;
                    }
                    else if (R > 255)
                    {
                        R = 255;
                    }

                    G = (int)((((pixelColor[0, 0].G * m.Matrix[0, 0]) +
                                 (pixelColor[1, 0].G * m.Matrix[1, 0]) +
                                 (pixelColor[2, 0].G * m.Matrix[2, 0]) +
                                 (pixelColor[0, 1].G * m.Matrix[0, 1]) +
                                 (pixelColor[1, 1].G * m.Matrix[1, 1]) +
                                 (pixelColor[2, 1].G * m.Matrix[2, 1]) +
                                 (pixelColor[0, 2].G * m.Matrix[0, 2]) +
                                 (pixelColor[1, 2].G * m.Matrix[1, 2]) +
                                 (pixelColor[2, 2].G * m.Matrix[2, 2]))
                                        / m.Factor) + m.Offset);

                    if (G < 0)
                    {
                        G = 0;
                    }
                    else if (G > 255)
                    {
                        G = 255;
                    }

                    B = (int)((((pixelColor[0, 0].B * m.Matrix[0, 0]) +
                                 (pixelColor[1, 0].B * m.Matrix[1, 0]) +
                                 (pixelColor[2, 0].B * m.Matrix[2, 0]) +
                                 (pixelColor[0, 1].B * m.Matrix[0, 1]) +
                                 (pixelColor[1, 1].B * m.Matrix[1, 1]) +
                                 (pixelColor[2, 1].B * m.Matrix[2, 1]) +
                                 (pixelColor[0, 2].B * m.Matrix[0, 2]) +
                                 (pixelColor[1, 2].B * m.Matrix[1, 2]) +
                                 (pixelColor[2, 2].B * m.Matrix[2, 2]))
                                        / m.Factor) + m.Offset);

                    if (B < 0)
                    {
                        B = 0;
                    }
                    else if (B > 255)
                    {
                        B = 255;
                    }
                    newImg.SetPixel(x + 1, y + 1, Color.FromArgb(A, R, G, B));
                }
            }
            return newImg;
        }
    }
}
