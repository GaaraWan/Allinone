using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace JzScreenPoints.BasicSpace
{
    public enum CornerPosition
    {
        NONE = -1,
        LEFTTOP = 0,
        RIGHTTOP = 1,
        LEFTBOTTOM = 2,
        RIGTHBOTTOM = 3,
    }
    public enum CornerEnum : int
    {
        COUNT = 4,

        LT = 0,
        LB = 1,
        RT = 2,
        RB = 3,

        NONE = -1,
    }
    public enum CornerExEnum : int
    {
        COUNT = 12,

        LT = 0,
        LB = 1,
        RT = 2,
        RB = 3,

        PT1 = 4,
        PT2 = 5,
        PT3 = 6,
        PT4 = 7,
        PT5 = 8,
        PT6 = 9,

        MPT = 10, // <-= Mutual Point
        DPT = 11, // <-= Define Point

        NONE = -1,
    }
    public class DibToBitmap
    {

        /// &lt;summary&gt;
        /// Convert DIB to Bitmap.
        /// &lt;/summary&gt;
        /// &lt;param name=&quot;dibPtrArg&quot;&gt;Pointer to memory DIB, starting with BITMAPINFOHEADER.&lt;/param&gt;
        /// &lt;returns&gt;A Bitmap&lt;/returns&gt;
        public static Bitmap Convert(IntPtr dibPtrArg)
        {
            BITMAPINFOHEADER bmi;
            IntPtr pixptr;

            GetPixelInfo(dibPtrArg, out pixptr, out bmi);

            Bitmap bitMap = new Bitmap(bmi.biWidth, bmi.biHeight);

            Graphics scannedImageGraphics = Graphics.FromImage(bitMap);

            IntPtr hdc = scannedImageGraphics.GetHdc();

            SetDIBitsToDevice(
               hdc,
               0,             // XDest
               0,             // YDest
               bmi.biWidth,
               bmi.biHeight,
               0,             // XSrc
               0,             // YSrc
               0,             // uStartScan
               bmi.biHeight,  // cScanLines
               pixptr,        // lpvBits
               dibPtrArg,     // lpbmi
               0);            // 0 = literal RGB values rather than palette

            scannedImageGraphics.ReleaseHdc(hdc);

            const float inchPerMeter = 39.3700787F;
            bitMap.SetResolution(bmi.biXPelsPerMeter / inchPerMeter, bmi.biYPelsPerMeter / inchPerMeter);

            // bitMap.Save(@&quot;c:\0\2.bmp&quot;, ImageFormat.Bmp); // debug code

            return bitMap;
        }


        static private void GetPixelInfo(IntPtr bmpptr, out IntPtr pix, out BITMAPINFOHEADER bmi)
        {

            bmi = new BITMAPINFOHEADER();
            Marshal.PtrToStructure(bmpptr, bmi); // copy into struct.

            if (bmi.biSizeImage == 0)
            {
                bmi.biSizeImage = ((((bmi.biWidth * bmi.biBitCount) + 31) & ~31) >> 3) * bmi.biHeight;
            }

            int p = bmi.biClrUsed;

            if ((p == 0) && (bmi.biBitCount <= 8))
            {
                p = 1 << bmi.biBitCount;
            }

            pix = (IntPtr)((p * 4) + bmi.biSize + (int)bmpptr);

        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        private class BITMAPINFOHEADER
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
        }

        [DllImport("gdi32.dll", ExactSpelling = true)]
        internal static extern int SetDIBitsToDevice(
           IntPtr hdc,
           int xdst,
           int ydst,
           int width,
           int height,
           int xsrc,
           int ysrc,
           int start,
           int lines,
           IntPtr bitsptr,
           IntPtr bmiptr,
           int color);

    } // class DibToImage
    class JzTools
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SwapMouseButton([param: MarshalAs(UnmanagedType.Bool)] bool fSwap);
        
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);
        
        [DllImport("GdiPlus.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int GdipCreateBitmapFromGdiDib(IntPtr pBIH, IntPtr pPix, out IntPtr pBitmap);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02; 
        private const int MOUSEEVENTF_LEFTUP = 0x04; 
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08; 
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        public static int PassingInteger = 0;
        public static string PassingString = "";
        
        public static Color UsedColor = Color.FromArgb(255, 192, 192);
        public static Color NormalColor = Color.FromArgb(192, 255, 192);

        public static void DoMouseClick() 
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0); 
        }
        public static void DoMouseLeftDown()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        }
        public static void DoMouseLeftUp()
        {
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }
        public static void DoMouseRightUp()
        {
            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
        } 
        public static void SwapMouse(bool IsSwap)
        {
            SwapMouseButton(IsSwap);
        }

        public static void BoundRect(ref Rectangle InnerRect, Size BoundSize)
        {
            InnerRect.X = Math.Min(Math.Max(InnerRect.X, 0), (BoundSize.Width - InnerRect.Width < 0 ? 0 : BoundSize.Width - InnerRect.Width));
            InnerRect.Y = Math.Min(Math.Max(InnerRect.Y, 0), (BoundSize.Height - InnerRect.Height < 0 ? 0 : BoundSize.Height - InnerRect.Height));

            if (BoundSize.Width <= InnerRect.X + InnerRect.Width)
                InnerRect.Width = JzTools.BoundValue(InnerRect.Width, BoundSize.Width - InnerRect.X, 1);
            if (BoundSize.Height <= InnerRect.Height + InnerRect.Height)
                InnerRect.Height = JzTools.BoundValue(InnerRect.Height, BoundSize.Height - InnerRect.Y, 1);
        }
        public static Rectangle BoundRect(Rectangle InnerRect, Size BoundSize)
        {
            InnerRect.X = Math.Min(Math.Max(InnerRect.X, 0), (BoundSize.Width - InnerRect.Width < 0 ? 0 : BoundSize.Width - InnerRect.Width));
            InnerRect.Y = Math.Min(Math.Max(InnerRect.Y, 0), (BoundSize.Height - InnerRect.Height < 0 ? 0 : BoundSize.Height - InnerRect.Height));

            if (BoundSize.Width <= InnerRect.X + InnerRect.Width)
                InnerRect.Width = JzTools.BoundValue(InnerRect.Width, BoundSize.Width - InnerRect.X, 1);
            if (BoundSize.Height <= InnerRect.Height + InnerRect.Height)
                InnerRect.Height = JzTools.BoundValue(InnerRect.Height, BoundSize.Height - InnerRect.Y, 1);

            return InnerRect;
        }

        public static Rectangle InteractionRect(Rectangle InnerRect, Size BoundSize)
        {
            InnerRect.X = Math.Max(InnerRect.X, 0);
            InnerRect.Y = Math.Max(InnerRect.Y, 0);

            if (BoundSize.Width <= InnerRect.X + InnerRect.Width)
                InnerRect.Width = BoundSize.Width - InnerRect.X;
            if (BoundSize.Height <= InnerRect.Height + InnerRect.Height)
                InnerRect.Height = BoundSize.Height - InnerRect.Y;

            return InnerRect;
        }
        public static int BoundValue(int Value, int Max, int Min)
        {
            return Math.Max(Math.Min(Value, Max), Min);

        }

        public static Point PtZero = new Point(0, 0);
        public static PointF PtFZero = new PointF(0, 0);

        public static Rectangle Resize(Rectangle OrgRect, int Ratio)
        {
            Rectangle rect = new Rectangle();

            rect.X = JzTools.ShiftValue(OrgRect.X, Ratio);
            rect.Y = JzTools.ShiftValue(OrgRect.Y, Ratio);
            rect.Width = Math.Max(JzTools.ShiftValue(OrgRect.Width, Ratio), 1);
            rect.Height = Math.Max(JzTools.ShiftValue(OrgRect.Height, Ratio), 1);

            return rect;
        }
        public static Size ExtendSize(Size OrgSize, int Extend)
        {
            return new Size(OrgSize.Width + Extend, OrgSize.Height + Extend);
        }
        public static Rectangle Resize(Size OrgSize, int Shift, Point BiasPoint, Point MouseLocation, Size BoundSize)
        {
            Size SizeTmp = new Size();
            Point NBiasPoint = new Point();

            if (Shift > 0)
                SizeTmp = new Size(OrgSize.Width << Shift, OrgSize.Height << Shift);
            else
                SizeTmp = new Size(OrgSize.Width >> -Shift, OrgSize.Height >> -Shift);

            if (SizeTmp.Width <= BoundSize.Width && SizeTmp.Height <= BoundSize.Height)
                return new Rectangle(0, 0, OrgSize.Width, OrgSize.Height);

            else
            {
                if (Shift > 0)
                {
                    SizeTmp = new Size(BoundSize.Width >> Shift, BoundSize.Height >> Shift);
                    NBiasPoint = new Point(BiasPoint.X >> Shift, BiasPoint.Y >> Shift);
                }
                else
                {
                    SizeTmp = new Size(BoundSize.Width << -Shift, BoundSize.Height << -Shift);
                    NBiasPoint = new Point(BiasPoint.X << -Shift, BiasPoint.Y << -Shift);
                }

                int X = (MouseLocation.X - NBiasPoint.X < 0 ? 0 : MouseLocation.X - NBiasPoint.X);
                int Y = (MouseLocation.Y - NBiasPoint.Y < 0 ? 0 : MouseLocation.Y - NBiasPoint.Y);

                return new Rectangle(X, Y, SizeTmp.Width + 10, SizeTmp.Height + 10);
            }
        }
        public static Size Resize(Size OrgSize, int Shift, Size BoundSize)
        {
            Size SizeTmp = new Size();

            if (Shift > 0)
                SizeTmp = new Size(OrgSize.Width << Shift, OrgSize.Height << Shift);
            else
                SizeTmp = new Size(OrgSize.Width >> -Shift, OrgSize.Height >> -Shift);

            if (SizeTmp.Width <= BoundSize.Width && SizeTmp.Height <= BoundSize.Height)
                return SizeTmp;
            else
                return BoundSize;
        }
        public static Size Resize(Size OrgSize, int Ratio)
        {
            Size retSize;

            if (Ratio > 0)
                retSize = new Size(OrgSize.Width << Ratio, OrgSize.Height << Ratio);
            else
                retSize = new Size(OrgSize.Width >> -Ratio, OrgSize.Height >> -Ratio);

            retSize.Width = Math.Max(retSize.Width, 1);
            retSize.Height = Math.Max(retSize.Height, 1);

            return retSize;
        }
        public static int ShiftValue(int Value, int Ratio)
        {
            if (Ratio >= 0)
                return Value << Ratio;
            else
            {
                //int add = Value %2;
                return (Value >> (-Ratio));
            }
        }
        public static void ClearPoint(ref Point rPoint)
        {
            rPoint.X = 0;
            rPoint.Y = 0;
        }
        public static void ClearSize(ref Size rSize)
        {
            rSize.Width = 0;
            rSize.Height = 0;
        }
        public static void SetSize(ref Size SizeEnd, Point PointStart, Point PointEnd)
        {
            SetSize(ref SizeEnd, PointStart, PointEnd, false);
        }
        public static void SetSize(ref Size SizeEnd, Point PointStart, Point PointEnd, bool IsWholeDirection)
        {
            SizeEnd.Width = PointStart.X - PointEnd.X;
            SizeEnd.Height = PointStart.Y - PointEnd.Y;

            if (IsWholeDirection)
            {
                SizeEnd.Width = -SizeEnd.Width;
                SizeEnd.Height = -SizeEnd.Height;
            }
        }
        public static void SetPoint(ref Point rPt, MouseEventArgs e)
        {
            rPt.X = e.X;
            rPt.Y = e.Y;
        }
        public static bool CheckTextBoxIsInteger(TextBox txtBox, int MaxValue, int MinValue)
        {
            int i = 0;
            bool ret = false;

            if (int.TryParse(txtBox.Text, out i))
            {
                if (i < MinValue || i > MaxValue)
                {
                    MessageBox.Show("輸入值需要在" + MinValue.ToString() + "及" + MaxValue.ToString() + "之間", "MAIN", MessageBoxButtons.OK);
                }
                else
                    ret = true;
            }
            else
            {
                MessageBox.Show("錯誤的輸入值。", "MAIN", MessageBoxButtons.OK);
            }

            return ret;
        }
        public static bool CheckTextBoxIsDouble(TextBox txtBox, double MaxValue, double MinValue)
        {
            double i = 0;
            bool ret = false;

            if (double.TryParse(txtBox.Text, out i))
            {
                if (i < MinValue || i > MaxValue)
                {
                    MessageBox.Show("輸入值需要在" + MinValue.ToString() + "及" + MaxValue.ToString() + "之間", "MAIN", MessageBoxButtons.OK);
                }
                else
                    ret = true;
            }
            else
            {
                MessageBox.Show("錯誤的輸入值。", "MAIN", MessageBoxButtons.OK);
            }

            return ret;
        }
        public static bool CheckStringIsDouble(string Str, double MaxValue, double MinValue)
        {
            double i = 0;
            bool ret = false;

            if (double.TryParse(Str, out i))
            {
                if (i < MinValue || i > MaxValue)
                {
                    MessageBox.Show("輸入值需要在" + MinValue.ToString() + "及" + MaxValue.ToString() + "之間", "MAIN", MessageBoxButtons.OK);
                }
                else
                    ret = true;
            }
            else
            {
                MessageBox.Show("錯誤的輸入值。", "MAIN", MessageBoxButtons.OK);
            }

            return ret;
        }

        public static string ColorInformation(Color mColor)
        {
            string Str = "";

            Str += "亮度:" + Grayscale(mColor.R, mColor.G, mColor.B).ToString().PadLeft(3) + " ,";
            Str += "紅色:" + mColor.R.ToString().PadLeft(3) + " ,";
            Str += "綠色:" + mColor.G.ToString().PadLeft(3) + " ,";
            Str += "藍色:" + mColor.B.ToString().PadLeft(3) + "";

            return Str;
        }
        public static string ColortoString(Color Colr)
        {
            return Colr.R.ToString() + "," + Colr.G.ToString() + "," + Colr.B.ToString();
        }
        public static uint ColorValue(Color Colr)
        {
            return (uint)((Colr.A << 24) | (Colr.R << 16) |
               (Colr.G << 8) | (Colr.B << 0));
        }
        public static byte Grayscale(byte R, byte G, byte B)
        {
            return (byte)((double)R * 0.3 + (double)G * 0.59 + (double)B * 0.11);
        }
        public static int GrayscaleInt(byte R, byte G, byte B)
        {
            return (int)((double)R * 0.3 + (double)G * 0.59 + (double)B * 0.11);
        }
        public static void SaveData(string DataStr, string FileName)
        {
            StreamWriter Swr = new StreamWriter(FileName, false, Encoding.Default);

            Swr.Write(DataStr);

            Swr.Flush();
            Swr.Close();
            Swr.Dispose();
        }

        public static void SaveData(string DataStr, string FileName,bool append)
        {
            StreamWriter Swr = new StreamWriter(FileName, append, Encoding.Default);

            Swr.Write(DataStr);

            Swr.Flush();
            Swr.Close();
            Swr.Dispose();
        }
        public static void ReadData(ref string DataStr, string FileName)
        {
                StreamReader Srr = new StreamReader(FileName, Encoding.Default);

                DataStr = Srr.ReadToEnd();

                Srr.Close();
                Srr.Dispose();
        }
       
        public static string RemoveLastChar(string Str, int Count)
        {
            if (Str.Length < Count)
                return "";

            return Str.Remove(Str.Length - Count, Count);
        }

        public static PointF PointFsAverage(PointF ptf1, PointF ptf2)
        {
            return new PointF((ptf1.X + ptf2.X) / 2f, (ptf1.Y + ptf2.Y) / 2f);
        }
        public static PointF PonitsAverage(List<PointF> ptlist)
        {
            PointF retptf = new PointF();

            foreach (PointF ptf in ptlist)
            {
                retptf.X += ptf.X;
                retptf.Y += ptf.Y;
            }

            retptf.X = retptf.X / (float)ptlist.Count;
            retptf.Y = retptf.Y / (float)ptlist.Count;

            return retptf;
        }

        public static RectangleF PointFToRectF(PointF PTF,float length)
        {
            PointF PTFTMP = PTF;

            PTFTMP.X = PTFTMP.X - length /2;
            PTFTMP.Y = PTFTMP.Y - length /2 ;

            return new RectangleF(PTFTMP, new SizeF(length, length));
        }
        public static RectangleF PointFToRectF(PointF PTF, float length,float Ratio)
        {
            PointF PTFTMP = PTF;

            PTFTMP.X = PTFTMP.X - ((length / Ratio) / 2);
            PTFTMP.Y = PTFTMP.Y - ((length / Ratio) / 2) ;

            return new RectangleF(PTFTMP, new SizeF(length /Ratio, length / Ratio));
        }
        public static string PointFToString(PointF PTF)
        {
            return PTF.X.ToString() + "," + PTF.Y.ToString();
        }
        public static string PointFToString(PointF PTF, char sep)
        {
            return PTF.X.ToString() + sep + PTF.Y.ToString();
        }
        public static string PointFToString000(PointF PTF)
        {
            return PTF.X.ToString("0.000") + "," + PTF.Y.ToString("0.000");
        }
        public static string PointFToString000(PointF PTF, char sep)
        {
            return PTF.X.ToString("0.000") + sep + PTF.Y.ToString("0.000");
        }
        public static PointF StringToPointF(string Str)
        {
            string[] strs = Str.Split(',');
            return new PointF(float.Parse(strs[0]), float.Parse(strs[1]));
        }
        public static string CornerEXToStr(CornerExEnum corner)
        {
            string retStr = "";
            switch (corner)
            {
                case CornerExEnum.LT:
                    retStr = "左上";
                    break;
                case CornerExEnum.LB:
                    retStr = "左下";
                    break;
                case CornerExEnum.RT:
                    retStr = "右上";
                    break;
                case CornerExEnum.RB:
                    retStr = "右下";
                    break;
                case CornerExEnum.MPT:
                    retStr = "量測";
                    break;
                case CornerExEnum.DPT:
                    retStr = "定點";
                    break;
                case CornerExEnum.PT1:
                case CornerExEnum.PT2:
                case CornerExEnum.PT3:
                case CornerExEnum.PT4:
                case CornerExEnum.PT5:
                case CornerExEnum.PT6:

                    retStr = "測點" + corner.ToString().Substring(corner.ToString().Length - 1, 1);
                    break;
            }

            return retStr;
        }
        public static string CornerToStr(CornerEnum corner)
        {
            string retStr = "";
            switch (corner)
            {
                case CornerEnum.LT:
                    retStr = "左上";
                    break;
                case CornerEnum.LB:
                    retStr = "左下";
                    break;
                case CornerEnum.RT:
                    retStr = "右上";
                    break;
                case CornerEnum.RB:
                    retStr = "右下";
                    break;
            }

            return retStr;
        }
        public static CornerEnum StrToCorner(string str)
        {
            CornerEnum retCorner = CornerEnum.LT;
            switch (str)
            {
                case "左上":
                    retCorner = CornerEnum.LT;
                    break;
                case "左下":
                    retCorner = CornerEnum.LB;
                    break;
                case "右上":
                    retCorner = CornerEnum.RT;
                    break;
                case "右下":
                    retCorner = CornerEnum.RB;
                    break;
            }

            return retCorner;
        }
        public static CornerExEnum StrToCornerEx(string str)
        {
            CornerExEnum retCorner = CornerExEnum.LT;
            switch (str)
            {
                case "左上":
                    retCorner = CornerExEnum.LT;
                    break;
                case "左下":
                    retCorner = CornerExEnum.LB;
                    break;
                case "右上":
                    retCorner = CornerExEnum.RT;
                    break;
                case "右下":
                    retCorner = CornerExEnum.RB;
                    break;
                case "量測":
                    retCorner = CornerExEnum.MPT;
                    break;
                case "定點":
                    retCorner = CornerExEnum.DPT;
                    break;
                default:
                    retCorner = (CornerExEnum)(int.Parse(str.Substring(str.Length - 1, 1)) + 3);
                    break;
            }

            return retCorner;
        }

        public static Rectangle CornerRect(Rectangle Rect, CornerEnum Corner, int CornerSize)
        {
            switch (Corner)
            {
                case CornerEnum.LT:
                    return new Rectangle(Rect.X - (CornerSize >> 1), Rect.Y - (CornerSize >> 1), CornerSize, CornerSize);
                case CornerEnum.RT:
                    return new Rectangle(Rect.X + Rect.Width - (CornerSize >> 1), Rect.Y - (CornerSize >> 1), CornerSize, CornerSize);
                case CornerEnum.LB:
                    return new Rectangle(Rect.X - (CornerSize >> 1), Rect.Y + Rect.Height - (CornerSize >> 1), CornerSize, CornerSize);
                case CornerEnum.RB:
                    return new Rectangle(Rect.X + Rect.Width - (CornerSize >> 1), Rect.Y + Rect.Height - (CornerSize >> 1), CornerSize, CornerSize);
            }

            return new Rectangle(Rect.X - (CornerSize >> 1), Rect.Y - (CornerSize >> 1), CornerSize, CornerSize);
        }
        public static string PointToString(Point PT)
        {
            return PT.X.ToString() + "," + PT.Y.ToString();
        }
        public static Point StringToPoint(string Str)
        {
            string[] strs = Str.Split(',');
            return new Point(int.Parse(strs[0]), int.Parse(strs[1]));
        }
        public static Rectangle StringtoRect(string RectStr)
        {
            string[] str = RectStr.Split(',');
            return new Rectangle(int.Parse(str[0]), int.Parse(str[1]), int.Parse(str[2]), int.Parse(str[3]));
        }
        public static string GetLastString(string Str, int NoFromLast)
        {
            return Str.Substring(Str.Length - NoFromLast, NoFromLast);
        }
        public static string PointtoString(PointF Pt)
        {
            return Pt.X.ToString().PadLeft(8) + "," + Pt.Y.ToString().PadLeft(8);
        }
        public static string PointtoString(MouseEventArgs e)
        {
            return e.X.ToString().PadLeft(4) + "," + e.Y.ToString().PadLeft(4);
        }
        public static string SizetoString(Size rSize)
        {
            return rSize.Width.ToString() + "," + rSize.Height.ToString();
        }

        public static string RecttoString(Rectangle Rect)
        {
            return Rect.X.ToString().PadLeft(4) + "," + Rect.Y.ToString().PadLeft(4) + "," + Rect.Width.ToString().PadLeft(4) + "," + Rect.Height.ToString().PadLeft(4);
        }
        public static string RecttoStringEX(Rectangle Rect)
        {
            return Rect.X.ToString("0000") + "," + Rect.Y.ToString("0000") + "," + Rect.Width.ToString("0000") + "," + Rect.Height.ToString("0000");
        }

        public static string RectFToString(RectangleF RectF)
        {
            string Str = "";

            Str += RectF.X.ToString("0.00") + ",";
            Str += RectF.Y.ToString("0.00") + ",";
            Str += RectF.Width.ToString("0.00") + ",";
            Str += RectF.Height.ToString("0.00");

            return Str;
        }
        public static RectangleF StringToRectF(string Str)
        {
            string[] strs = Str.Split(',');
            RectangleF rectF = new RectangleF();

            rectF.X = float.Parse(strs[0]);
            rectF.Y = float.Parse(strs[1]);
            rectF.Width = float.Parse(strs[2]);
            rectF.Height = float.Parse(strs[3]);

            return rectF;


        }
        public static Rectangle RectFToRect(RectangleF RectF)
        {
            Rectangle rect = new Rectangle((int)RectF.X, (int)RectF.Y, (int)RectF.Width, (int)RectF.Height);

            return rect;
        }
        public static RectangleF RectToRectF(Rectangle Rect)
        {
            RectangleF rectF = new RectangleF((float)Rect.X, (float)Rect.Y, (float)Rect.Width, (float)Rect.Height);

            return rectF;
        }
        public static Rectangle RectTwoPoint(Point StartPoint, Point EndPoint, Size BoundSize)
        {
            Point RecEndPoint = StartPoint;
            Point RecStartPoint = EndPoint;
            Rectangle Rect = new Rectangle();

            if (RecEndPoint.X >= RecStartPoint.X && RecEndPoint.Y >= RecStartPoint.Y)
            {
                Rect.X = RecStartPoint.X + 1;
                Rect.Y = RecStartPoint.Y + 1;
                Rect.Width = RecEndPoint.X - RecStartPoint.X - 1;
                Rect.Height = RecEndPoint.Y - RecStartPoint.Y - 1;
            }
            else if (RecEndPoint.X > RecStartPoint.X && RecEndPoint.Y < RecStartPoint.Y)
            {
                Rect.X = RecStartPoint.X + 1;
                Rect.Y = RecEndPoint.Y + 1;
                Rect.Width = RecEndPoint.X - RecStartPoint.X - 1;
                Rect.Height = RecStartPoint.Y - RecEndPoint.Y - 1;
            }
            else if (RecEndPoint.X < RecStartPoint.X && RecEndPoint.Y < RecStartPoint.Y)
            {
                Rect.X = RecEndPoint.X + 1;
                Rect.Y = RecEndPoint.Y + 1;
                Rect.Width = RecStartPoint.X - RecEndPoint.X - 1;
                Rect.Height = RecStartPoint.Y - RecEndPoint.Y - 1;
            }
            else if (RecEndPoint.X < RecStartPoint.X && RecEndPoint.Y > RecStartPoint.Y)
            {
                Rect.X = RecEndPoint.X + 1;
                Rect.Y = RecStartPoint.Y + 1;
                Rect.Width = RecStartPoint.X - RecEndPoint.X - 1;
                Rect.Height = RecEndPoint.Y - RecStartPoint.Y - 1;
            }

            //if (Rect.X < 0)
            //    Rect.X = 0;
            //if ((Rect.X + Rect.Width) > BoundSize.Width)
            //    Rect.Width = BoundSize.Width - Rect.X;
            //if (Rect.Width <= 0)
            //    Rect.Width = 1;

            //if (Rect.Y < 0)
            //    Rect.Y = 0;
            //if ((Rect.Y + Rect.Height) > BoundSize.Height)
            //    Rect.Height = BoundSize.Height - Rect.Y;

            //if (Rect.Height <= 0)
            //    Rect.Height = 1;
            return Rect;
        }
        public static Rectangle RectTwoPoint(Point StartPoint, Point EndPoint)
        {
            Point RecEndPoint = StartPoint;
            Point RecStartPoint = EndPoint;
            Rectangle Rect = new Rectangle();

            if (RecEndPoint.X >= RecStartPoint.X && RecEndPoint.Y >= RecStartPoint.Y)
            {
                Rect.X = RecStartPoint.X + 1;
                Rect.Y = RecStartPoint.Y + 1;
                Rect.Width = RecEndPoint.X - RecStartPoint.X - 1;
                Rect.Height = RecEndPoint.Y - RecStartPoint.Y - 1;
            }
            else if (RecEndPoint.X >= RecStartPoint.X && RecEndPoint.Y <= RecStartPoint.Y)
            {
                Rect.X = RecStartPoint.X + 1;
                Rect.Y = RecEndPoint.Y + 1;
                Rect.Width = RecEndPoint.X - RecStartPoint.X - 1;
                Rect.Height = RecStartPoint.Y - RecEndPoint.Y - 1;
            }
            else if (RecEndPoint.X <= RecStartPoint.X && RecEndPoint.Y <= RecStartPoint.Y)
            {
                Rect.X = RecEndPoint.X + 1;
                Rect.Y = RecEndPoint.Y + 1;
                Rect.Width = RecStartPoint.X - RecEndPoint.X - 1;
                Rect.Height = RecStartPoint.Y - RecEndPoint.Y - 1;
            }
            else if (RecEndPoint.X <= RecStartPoint.X && RecEndPoint.Y >= RecStartPoint.Y)
            {
                Rect.X = RecEndPoint.X + 1;
                Rect.Y = RecStartPoint.Y + 1;
                Rect.Width = RecStartPoint.X - RecEndPoint.X - 1;
                Rect.Height = RecEndPoint.Y - RecStartPoint.Y - 1;
            }

            if (Rect.X < 0)
                Rect.X = 0;
            if (Rect.Width <= 0)
                Rect.Width = 1;

            if (Rect.Y < 0)
                Rect.Y = 0;

            if (Rect.Height <= 0)
                Rect.Height = 1;
            return Rect;

        }


        public static bool IsRectFIntersection(List<RectangleF> RectFList, RectangleF RectF)
        {
            bool ret = false;

            foreach (RectangleF rectf in RectFList)
            {
                if (rectf.IntersectsWith(RectF))
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }
        public static void MergeRectF(List<RectangleF> InputRectFList, ref List<RectangleF> OutputRectFList,bool IsIntersectionMerge,int MergeEnlarge)
        {
            RectangleF rectfrom = new RectangleF();
            RectangleF rectfnew = new RectangleF();
            
            bool[] IsMerged = new bool[InputRectFList.Count];

            OutputRectFList.Clear();

            int i = 0;
            int j = 0;
            while (i < InputRectFList.Count)
            {
                if (!IsMerged[i])
                {
                    rectfrom = InputRectFList[i];
                    j = i + 1;
                    while (j < InputRectFList.Count)
                    {
                        rectfnew = InputRectFList[j];
 
                        if (IsIntersectionMerge)
                            rectfnew.Inflate(MergeEnlarge, MergeEnlarge);

                        if (rectfrom.IntersectsWith(rectfnew) || !IsIntersectionMerge)
                        {
                            rectfrom = MergeTwoRects(rectfrom, InputRectFList[j]);
                            IsMerged[j] = true;
                        }

                        j++;
                    }
                    OutputRectFList.Add(rectfrom);
                }
                i++;
            }
         }

        public static Rectangle MergeTwoRects(Rectangle rect1, Rectangle rect2)
        {
            Rectangle rect = new Rectangle();

            if (rect1.Width == 0)
                return rect2;
            if (rect2.Width == 0)
                return rect1;

            rect.X = Math.Min(rect1.X, rect2.X);
            rect.Y = Math.Min(rect1.Y, rect2.Y);

            rect.Width = Math.Max(rect1.X + rect1.Width, rect2.X + rect2.Width) - rect.X;
            rect.Height = Math.Max(rect1.Y + rect1.Height, rect2.Y + rect2.Height) - rect.Y;

            return rect;
        }
        public static RectangleF MergeTwoRects(RectangleF rect1, RectangleF rect2)
        {
            RectangleF rect = new RectangleF();

            if (rect1.Width == 0)
                return rect2;
            if (rect2.Width == 0)
                return rect1;

            rect.X = Math.Min(rect1.X, rect2.X);
            rect.Y = Math.Min(rect1.Y, rect2.Y);

            rect.Width = Math.Max(rect1.X + rect1.Width, rect2.X + rect2.Width) - rect.X;
            rect.Height = Math.Max(rect1.Y + rect1.Height, rect2.Y + rect2.Height) - rect.Y;

            return rect;
        }
        public static bool IsInRange(int FromValue, int CompValue, int DiffValue)
        {
            return (FromValue >= CompValue - DiffValue) && (FromValue <= CompValue + DiffValue);
        }
        public static bool IsInRange(double FromValue, double CompValue, double DiffValue)
        {
            return Math.Abs(FromValue - CompValue) < DiffValue;
        }
        public static bool IsInRange(float FromValue, float CompValue, float DiffValue)
        {
            return Math.Abs(FromValue - CompValue) < DiffValue;
        }
        public static bool IsInRangeRatio(double FromValue, double CompValue, double Ratio)
        {
            return (FromValue >= (CompValue * (1 - (Ratio / 100d)))) && (FromValue <= (CompValue * (1 + (Ratio / 100d))));
        }


        public static bool IsInRangeEx(int FromValue, int MaxValue, int MinValue)
        {
            return (FromValue >= MinValue) && (FromValue <= MaxValue);
        }
        public static bool IsInRangeEx(double FromValue, double MaxValue, double MinValue)
        {
            return (FromValue >= MinValue) && (FromValue <= MaxValue);
        }

        public static bool IsPointInRect(Point Pt, Rectangle Rect)
        {
            return (Pt.X >= Rect.X) && (Pt.X < (Rect.X + Rect.Width)) && (Pt.Y >= Rect.Y) && (Pt.Y < (Rect.Y + Rect.Height));
        }
        public static bool IsPointInRect(PointF Pt, RectangleF Rect)
        {
            return (Pt.X >= Rect.X) && (Pt.X < (Rect.X + Rect.Width)) && (Pt.Y >= Rect.Y) && (Pt.Y < (Rect.Y + Rect.Height));
        }
        public static bool IsTwoPtFTheSame(PointF PtFOrg,PointF PtFAfter, float Range)
        {
            RectangleF rectf = PointFToRectF(PtFOrg, Range);
            return (IsPointInRect(PtFAfter, rectf));
        }
        public static bool IsRectInMyRect(Rectangle Rect, Rectangle MyRect)
        {
            return IsPointInRect(Rect.Location, MyRect) && IsPointInRect(new Point(Rect.Location.X + Rect.Width, Rect.Y), MyRect) && IsPointInRect(new Point(Rect.Location.X, Rect.Y + Rect.Height), MyRect) && IsPointInRect(new Point(Rect.Location.X + Rect.Width, Rect.Y + Rect.Height), MyRect);
        }
        public static bool IsRectInMyRect(RectangleF RectF, RectangleF MyRectF)
        {
            Rectangle rect = new Rectangle((int)RectF.X, (int)RectF.Y, (int)RectF.Width, (int)RectF.Height);
            Rectangle myrect = new Rectangle((int)MyRectF.X, (int)MyRectF.Y, (int)MyRectF.Width, (int)MyRectF.Height);

            return IsRectInMyRect(rect, myrect);
        }
        public static bool IsRectCenterInMyRect(RectangleF RectF, RectangleF MyRectF)
        {
            Rectangle rect = new Rectangle((int)RectF.X, (int)RectF.Y, (int)RectF.Width, (int)RectF.Height);
            Rectangle myrect = new Rectangle((int)MyRectF.X, (int)MyRectF.Y, (int)MyRectF.Width, (int)MyRectF.Height);

            return IsPointInRect(GetRectCenter(rect), myrect);
        }

        public static RectangleF RectFConverge(RectangleF RectF, int ConvergeLength)
        {
            RectangleF rectf = RectF;

            rectf.Inflate(ConvergeLength, ConvergeLength);

            return rectf;
        }
        public static RectangleF RectFConverge(RectangleF RectF, float Ratio) //Ratio 0 to 1
        {
            RectangleF rectf = RectF;

            rectf.Inflate(RectF.Width * ((Ratio - 1f) / 2f), RectF.Height * ((Ratio - 1f) / 2f));

            return rectf;
        }
        public static bool RectFInside(RectangleF RectFIn, RectangleF RectFOut)
        {
            RectangleF rectF = RectFIn;
            RectFIn.Intersect(RectFOut);

            return rectF.Equals(RectFIn);
        }

        public static Rectangle RectConverge(Rectangle Rect, int ConvergeLength)
        {
            Rectangle rect = Rect;

            rect.Inflate(ConvergeLength, ConvergeLength);

            return rect;
        }
        public static Rectangle RectConverge(Size size, double ratio)
        {
            Rectangle rect = new Rectangle(new Point(), size);

            int xx = (int)((double)size.Width * ratio / 2);
            int yy = (int)((double)size.Height * ratio / 2);

            rect.Inflate(xx, yy);

            return rect;
        }

        public static void GetOutLineBMP(ref Bitmap bmp, Bitmap FromBMP, double MaskRatio)
        {
            GetOutLineBMP(ref bmp, FromBMP, MaskRatio, Color.FromArgb(0, 0, 0));
        }
        public static void GetOutLineBMP(ref Bitmap bmp, Bitmap FromBMP, double MaskRatio,Color color)
        {
            SolidBrush CareColor = new SolidBrush(Color.FromArgb(255, 255, 255));
            SolidBrush DontCareColor = new SolidBrush(color);

            bmp.Dispose();
            bmp = new Bitmap(FromBMP.Width, FromBMP.Height);

            Graphics g = Graphics.FromImage(bmp);
            g.FillRectangle(CareColor, new Rectangle(0, 0, FromBMP.Width, FromBMP.Height));

            Rectangle rect = RectConverge(FromBMP.Size, MaskRatio);
            g.FillRectangle(DontCareColor, rect);

            g.Dispose();
        }
        public static void GetOutLineBMP(ref Bitmap bmp, double MaskRatio, Color color)
        {
            SolidBrush DontCareColor = new SolidBrush(color);

            Graphics g = Graphics.FromImage(bmp);

            Rectangle rect = RectConverge(bmp.Size, MaskRatio);
            g.FillRectangle(DontCareColor, rect);

            g.Dispose();
        }
        public static PointF RectFCenter(RectangleF Rectf)
        {
            RectangleF rectf = Rectf;

            return new PointF(rectf.X + rectf.Width / 2, rectf.Y + rectf.Height / 2);
        }

        public static RectangleF RectTwoPoint(PointF StartPoint, PointF EndPoint)
        {
            PointF RecEndPoint = StartPoint;
            PointF RecStartPoint = EndPoint;
            RectangleF Rect = new RectangleF(StartPoint, new Size(1, 1));

            if (RecEndPoint.X >= RecStartPoint.X && RecEndPoint.Y >= RecStartPoint.Y)
            {
                Rect.X = RecStartPoint.X + 1;
                Rect.Y = RecStartPoint.Y + 1;
                Rect.Width = RecEndPoint.X - RecStartPoint.X - 1;
                Rect.Height = RecEndPoint.Y - RecStartPoint.Y - 1;
            }
            else if (RecEndPoint.X > RecStartPoint.X && RecEndPoint.Y < RecStartPoint.Y)
            {
                Rect.X = RecStartPoint.X + 1;
                Rect.Y = RecEndPoint.Y + 1;
                Rect.Width = RecEndPoint.X - RecStartPoint.X - 1;
                Rect.Height = RecStartPoint.Y - RecEndPoint.Y - 1;
            }
            else if (RecEndPoint.X < RecStartPoint.X && RecEndPoint.Y < RecStartPoint.Y)
            {
                Rect.X = RecEndPoint.X + 1;
                Rect.Y = RecEndPoint.Y + 1;
                Rect.Width = RecStartPoint.X - RecEndPoint.X - 1;
                Rect.Height = RecStartPoint.Y - RecEndPoint.Y - 1;
            }
            else if (RecEndPoint.X < RecStartPoint.X && RecEndPoint.Y > RecStartPoint.Y)
            {
                Rect.X = RecEndPoint.X + 1;
                Rect.Y = RecStartPoint.Y + 1;
                Rect.Width = RecStartPoint.X - RecEndPoint.X - 1;
                Rect.Height = RecEndPoint.Y - RecStartPoint.Y - 1;
            }

            return Rect;
        }

        public static string PathPicker(string Description, string DefaultPath)
        {
            string retStr = "";

            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.Description = Description;
            fd.ShowNewFolderButton = false;
            fd.SelectedPath = DefaultPath;

            if (fd.ShowDialog().Equals(DialogResult.OK))
            {
                if (fd.SelectedPath != "")
                    retStr = fd.SelectedPath;
            }
            else
                retStr = "";

            return retStr;
        }
        public static string OpenFilePicker(string DefaultPath,string DefaultName)
        {
            string retStr = "";
            
            OpenFileDialog dlg = new OpenFileDialog();

            //dlg.Filter = "BMP Files (*.bmp)|*.BMP|" + "All files (*.*)|*.*";
            dlg.Filter = DefaultPath;
            dlg.FileName = DefaultName;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                retStr = dlg.FileName;
            }
            return retStr;
        }
        public static string SaveFilePicker(string DefaultPath, string DefaultName)
        {
            string retStr = "";

            SaveFileDialog dlg = new SaveFileDialog();

            //dlg.Filter = "BMP Files (*.bmp)|*.BMP|" + "All files (*.*)|*.*";
            dlg.Filter = DefaultPath;
            dlg.FileName = DefaultName;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                retStr = dlg.FileName;
            }
            return retStr;
        }

        public static double RadToDegree(double Rad)
        {
            return Rad * 180 / Math.PI;
        }

        public static void CGOperate()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        public static Point GetRectCenter(Rectangle Rect)
        {
            return new Point(Rect.X + (Rect.Width >> 1), Rect.Y + (Rect.Height >> 1));
        }
        public static int GetRectArea(Rectangle Rect)
        {
            return Rect.Width * Rect.Height;
        }
        public static int GetRectArea(Rectangle Rect, double Ratio)
        {
            return (int)(Rect.Width * Rect.Height * Ratio);
        }

        public static PointF GetRectFCenter(RectangleF RectF)
        {
            return new PointF(RectF.X + (RectF.Width / 2), RectF.Y + (RectF.Height /2 ));
        }
        public static double GetRectFArea(RectangleF RectF)
        {
            return RectF.Width * RectF.Height;
        }
        public static double GetRecFtArea(RectangleF RectF, double Ratio)
        {
            return (double)(RectF.Width * RectF.Height * Ratio);
        }

        public static bool RectIsTheSame(Rectangle OrgRect, Rectangle ComRect, int Percent)
        {
            bool ret = true;
            double UB = (100 + (double)Percent) / 100;
            double LB = (100 - (double)Percent) / 100;

            ret = ret & (((int)((double)OrgRect.Width * UB)) >= ComRect.Width && ((int)((double)OrgRect.Width * LB)) <= ComRect.Width);
            ret = ret & (((int)((double)OrgRect.Height * UB)) >= ComRect.Height && ((int)((double)OrgRect.Height * LB)) <= ComRect.Height);
            ret = ret & (((int)((double)GetRectArea(OrgRect) * UB)) >= GetRectArea(ComRect) && ((int)((double)GetRectArea(OrgRect) * LB)) <= GetRectArea(ComRect));

            return ret;
        }
        public static bool RectIsTheSameEx(Rectangle OrgRect, Rectangle ComRect, int Percent)
        {
            bool ret = true;
            double UB = (100 + (double)Percent) / 100;
            double LB = (100 - (double)Percent) / 100;

            ret = ret & (((int)((double)OrgRect.Width * UB)) >= ComRect.Width && ((int)((double)OrgRect.Width * LB)) <= ComRect.Width);
            ret = ret & (((int)((double)OrgRect.Height * UB)) >= ComRect.Height && ((int)((double)OrgRect.Height * LB)) <= ComRect.Height);
            //ret = ret & (((int)((double)RectAreaSize(OrgRect) * UB)) >= RectAreaSize(ComRect) && ((int)((double)RectAreaSize(OrgRect) * LB)) <= RectAreaSize(ComRect));

            return ret;
        }

        public static bool RectIsTheSameFx(Rectangle OrgRect, Rectangle ComRect, int Percent, int CenterDistance)
        {
            bool ret = true;
            double UB = (100 + (double)Percent) / 100;
            double LB = (100 - (double)Percent) / 100;

            ret = ret & (((int)((double)OrgRect.Width * UB)) >= ComRect.Width && ((int)((double)OrgRect.Width * LB)) <= ComRect.Width);
            ret = ret & (((int)((double)OrgRect.Height * UB)) >= ComRect.Height && ((int)((double)OrgRect.Height * LB)) <= ComRect.Height);
            ret = ret & ((Math.Abs(GetRectCenter(OrgRect).X - GetRectCenter(ComRect).X) < CenterDistance) && (Math.Abs(GetRectCenter(OrgRect).Y - GetRectCenter(ComRect).Y) < CenterDistance));

            //ret = ret & (((int)((double)RectAreaSize(OrgRect) * UB)) >= RectAreaSize(ComRect) && ((int)((double)RectAreaSize(OrgRect) * LB)) <= RectAreaSize(ComRect));

            return ret;
        }
        public static bool RectIsTheSameFx(Rectangle OrgRect, Rectangle ComRect, int MoreWidth, int MoreHeight, int CenterDistance)
        {
            bool ret = true;

            ret = ret & (((int)((double)OrgRect.Width + MoreWidth)) >= ComRect.Width && ((int)((double)OrgRect.Width - MoreWidth)) <= ComRect.Width);
            ret = ret & (((int)((double)OrgRect.Height + MoreHeight)) >= ComRect.Height && ((int)((double)OrgRect.Height - MoreHeight)) <= ComRect.Height);
            ret = ret & ((Math.Abs(GetRectCenter(OrgRect).X - GetRectCenter(ComRect).X) < CenterDistance) && (Math.Abs(GetRectCenter(OrgRect).Y - GetRectCenter(ComRect).Y) < CenterDistance));

            //ret = ret & (((int)((double)RectAreaSize(OrgRect) * UB)) >= RectAreaSize(ComRect) && ((int)((double)RectAreaSize(OrgRect) * LB)) <= RectAreaSize(ComRect));

            return ret;
        }

        public static bool RectIsTheSame(Rectangle OrgRect, Rectangle ComRect, int Percent, int Area)
        {
            bool ret = true;
            double UB = (100 + (double)Percent) / 100;
            double LB = (100 - (double)Percent) / 100;

            ret = ret & (((int)((double)OrgRect.Width * UB)) >= ComRect.Width && ((int)((double)OrgRect.Width * LB)) <= ComRect.Width);
            ret = ret & (((int)((double)OrgRect.Height * UB)) >= ComRect.Height && ((int)((double)OrgRect.Height * LB)) <= ComRect.Height);
            ret = ret & (((int)((double)GetRectArea(OrgRect) * UB)) >= GetRectArea(ComRect) && ((int)((double)GetRectArea(OrgRect) * LB)) <= GetRectArea(ComRect));
            ret = ret & (((int)((double)GetRectArea(OrgRect) * (UB + 0.3))) >= Area && ((int)((double)GetRectArea(OrgRect) * (LB - 0.3))) <= Area);

            return ret;
        }
        public static bool RectIsSmallThan(Rectangle OrgRect, int Size)
        {
            return (OrgRect.Width < Size) || (OrgRect.Height < Size);
        }
        public static bool RectIsSmallThan(Rectangle OrgRect, Size CompSize)
        {
            return RectIsSmallThan(OrgRect, CompSize.Width, CompSize.Height);
        }
        //public static bool RectIsSmallThan(Rectangle OrgRect, int Width,int Height)
        //{
        //    return ((OrgRect.Width < Width) && (OrgRect.Height < Height)) || ((OrgRect.Width < (Width << 2)) && (OrgRect.Height < Height)) || ((OrgRect.Width < Width) && (OrgRect.Height < (Height << 2)));
        //}
        public static bool RectIsSmallThan(Rectangle OrgRect, int Width, int Height)
        {
            return ((OrgRect.Width < Width) && (OrgRect.Height < Height)) || ((OrgRect.Width < (int)((double)Width * 1.5)) && (OrgRect.Height < Height)) || ((OrgRect.Width < Width) && (OrgRect.Height < (int)((double)Height * 1.5)));
        }

        public static void DrawRect(ref Bitmap BMP, List<RectangleF> RectFList, Pen RectPen, double Ratio)
        {
            if (RectFList.Count > 0)
            {
                Graphics g = Graphics.FromImage(BMP);

                foreach (RectangleF rectF in RectFList)
                    g.DrawRectangle(RectPen, Resize(RectFToRect(rectF), Ratio));

                g.Dispose();
            }
        }
        public static void DrawRect(ref Bitmap BMP, List<RectangleF> RectFList, Pen ColorPen)
        {
            if (RectFList.Count > 0)
            {
                Graphics g = Graphics.FromImage(BMP);

                foreach (RectangleF rectF in RectFList)
                    g.DrawRectangle(ColorPen, RectFToRect(rectF));

                g.Dispose();
            }
        }
        public static void DrawRect(ref Bitmap BMP, List<RectangleF> RectFList, List<int> IndexList, Pen ColorPen)
        {
            if (RectFList.Count > 0)
            {
                int i = 0;
                Graphics g = Graphics.FromImage(BMP);
                Font MyFont = new Font("Arial", 64);
                SolidBrush B = new SolidBrush(Color.Red);
                Pen GPen = new Pen(Color.Lime, 6);

                foreach (RectangleF rectF in RectFList)
                {
                    if (IndexList[i] != -1)
                    {
                        g.DrawRectangle(ColorPen, RectFToRect(rectF));
                        g.DrawString((IndexList[i] + 1).ToString(), MyFont, B, rectF);
                    }
                    else
                    {
                        g.DrawRectangle(GPen, RectFToRect(rectF));
                    }
                    i++;
                }

                g.Dispose();
            }
        }
        public static void DrawRect(ref Bitmap BMP, List<RectangleF> RectFList, List<int> IndexList, List<string> DescriptionList, Pen ColorPen)
        {
            if (RectFList.Count > 0)
            {
                int i = 0;
                Graphics g = Graphics.FromImage(BMP);
                Font MyFont = new Font("Arial", 64);
                SolidBrush B = new SolidBrush(Color.Red);
                Pen GPen = new Pen(Color.Lime, 6);

                foreach (RectangleF rectF in RectFList)
                {
                    if (DescriptionList[i] != "完美")
                    {
                        g.DrawRectangle(ColorPen, RectFToRect(rectF));
                        g.DrawString((IndexList[i] + 1).ToString(), MyFont, B, rectF);
                    }
                    else
                    {
                        g.DrawRectangle(GPen, RectFToRect(rectF));
                    }
                    i++;
                }

                g.Dispose();
            }
        }
        public static void DrawRect(PictureBox pic, Rectangle rect, Pen RoundPen)
        {
            Graphics g = pic.CreateGraphics();
            g.DrawRectangle(RoundPen, rect);
            g.Dispose();
        }
        public static void DrawRect(PictureBox pic, Rectangle[] rect, Pen RoundPen)
        {
            if (rect.Length != 0)
            {
                Graphics g = pic.CreateGraphics();
                g.DrawRectangles(RoundPen, rect);
                g.Dispose();
            }
        }
        public static void DrawCross(ref Bitmap BMP, Rectangle Rect, Pen P)
        {
            Graphics g = Graphics.FromImage(BMP);
            //g.DrawRectangle(P, Rect);
            g.DrawLine(P, new Point(Rect.Left, Rect.Top), new Point(Rect.Right, Rect.Bottom));
            g.DrawLine(P, new Point(Rect.Left, Rect.Bottom), new Point(Rect.Right, Rect.Top));
            g.Dispose();
        }

        public static void DrawRect(Bitmap BMP, Pen RoundPen)
        {
            DrawRect(BMP, new Rectangle((int)RoundPen.Width >> 1, (int)RoundPen.Width >> 1, BMP.Width - ((int)RoundPen.Width), BMP.Height - ((int)RoundPen.Width)), RoundPen);
        }

        public static void DrawRect(Bitmap BMP, Rectangle Rect, SolidBrush B)
        {
            Graphics g = Graphics.FromImage(BMP);
            g.FillRectangle(B, Rect);
            g.Dispose();

        }
        public static void DrawCircle(Bitmap BMP,Point Ptcirclecenter, int rlength)
        {
            if (Ptcirclecenter.X > 5000 || Ptcirclecenter.X < -5000)
            {
                Ptcirclecenter = new Point();
                rlength = 1;
            }
            
            Graphics g = Graphics.FromImage(BMP);

            g.DrawEllipse(new Pen(Color.Pink, 3), Ptcirclecenter.X - rlength, Ptcirclecenter.Y - rlength, rlength << 1, rlength << 1);

            g.Dispose();
        }
        public static void DrawCircle(Bitmap BMP, Point Ptcirclecenter, int rlength,int width)
        {
            if (Ptcirclecenter.X > 5000 || Ptcirclecenter.X < -5000)
            {
                Ptcirclecenter = new Point();
                rlength = 1;
            }

            Graphics g = Graphics.FromImage(BMP);

            g.DrawEllipse(new Pen(Color.Pink, width), Ptcirclecenter.X - rlength, Ptcirclecenter.Y - rlength, rlength << 1, rlength << 1);

            g.Dispose();
        }
        public static void DrawRect(Bitmap BMP, string[] RectStr, SolidBrush B)
        {
            Graphics g = Graphics.FromImage(BMP);
            int i = 0;
            while (i < RectStr.Length)
            {
                g.FillRectangle(B, JzTools.StringtoRect(RectStr[i]));
                i++;
            }
            g.Dispose();
        }
        public static void DrawRect(Bitmap BMP, Rectangle Rect, Pen RoundPen, int Enlarge)
        {
            DrawRect(BMP, new Rectangle(Rect.X - Enlarge, Rect.Y - Enlarge, ((int)Rect.Width) + (Enlarge << 1), ((int)Rect.Height) + (Enlarge << 1)), RoundPen);
        }
        public static void DrawRectEx(Bitmap BMP, Rectangle Rect, Pen RoundPen)
        {
            DrawRect(BMP, new Rectangle(Rect.X, Rect.Y, ((int)Rect.Width), ((int)Rect.Height)), RoundPen);
        }
        public static void DrawRect(Bitmap BMP, Rectangle Rect, Pen RoundPen)
        {
            Graphics g = Graphics.FromImage(BMP);
            g.DrawRectangle(RoundPen, Rect);
            g.Dispose();
        }
        public static void DrawRect(Graphics g, Rectangle Rect, Color PenColor, int LineSize)
        {
            g.DrawRectangle(new Pen(PenColor, (float)LineSize), Rect);
        }
        
        public static void DrawLine(Bitmap BMP, Pen P, Point FromPt, Point ToPt)
        {
            Graphics g = Graphics.FromImage(BMP);
            g.DrawLine(P, FromPt, ToPt);
            g.Dispose();
        }
        public static void DrawLine(Bitmap BMP, Pen P, PointF FromPt, PointF ToPt)
        {
            Point Fpt = new Point((int)FromPt.X, (int)FromPt.Y);
            Point Tpt = new Point((int)ToPt.X, (int)ToPt.Y);

            DrawLine(BMP, P, Fpt, Tpt);
        }
        
        public static void DrawText(Bitmap BMP, string Text, SolidBrush B, Font MyFont, PointF Locate)
        {
            Graphics g = Graphics.FromImage(BMP);
            g.DrawString(Text, MyFont, B, new PointF(Locate.X - 20, Locate.Y - 10));
            g.Dispose();
        }
        public static void DrawText(Bitmap BMP, string Text)
        {
            SolidBrush B = new SolidBrush(Color.Lime);
            Font MyFont = new Font("Arial", 24);

            Graphics g = Graphics.FromImage(BMP);
            g.DrawString(Text, MyFont, B, new PointF(5, 5));
            g.Dispose();
        }
        public static void DrawText(Bitmap BMP, string Text, int size)
        {
            SolidBrush B = new SolidBrush(Color.Lime);
            Font MyFont = new Font("Arial", size);

            Graphics g = Graphics.FromImage(BMP);
            g.DrawString(Text, MyFont, B, new PointF(5, 5));
            g.Dispose();
        }
        public static void DrawText(Bitmap BMP, string Text, bool IsFromBottom)
        {
            SolidBrush B = new SolidBrush(Color.Red);
            Font MyFont = new Font("Arial", 24);

            Graphics g = Graphics.FromImage(BMP);

            g.FillRectangle(Brushes.Yellow, new Rectangle(5, BMP.Height - 50, 400, 40));

            if (!IsFromBottom)
                g.DrawString(Text, MyFont, B, new PointF(5, 5));
            else
                g.DrawString(Text, MyFont, B, new PointF(5, BMP.Height - 50));

            g.Dispose();
        }
        public static void DrawText(Bitmap BMP, string Text, Point Pt, int FontSize, Color Clor)
        {
            SolidBrush B = new SolidBrush(Clor);
            Font MyFont = new Font("Arial", FontSize);

            Graphics g = Graphics.FromImage(BMP);
            g.DrawString(Text, MyFont, B, new PointF(Pt.X, Pt.Y));
            g.Dispose();
        }

        public static void DrawImage(Bitmap BMPFrom,Bitmap BMPTo, Rectangle DestRect)
        {
            Graphics g = Graphics.FromImage(BMPTo);
            g.DrawImage(BMPFrom, DestRect, JzTools.SimpleRect(BMPFrom.Size), GraphicsUnit.Pixel);
            g.Dispose();
        }

        public static Rectangle Resize(Rectangle rect, double Ratio)
        {
            int rectw = (int)(rect.Width * Ratio / 2);
            int recth = (int) (rect.Height * Ratio / 2);

            rect.Inflate(rectw, recth);

            return rect;
        }

        public static Rectangle SimpleRect(Size Sz)
        {
            return new Rectangle(0, 0, Sz.Width, Sz.Height);
        }
        public static Rectangle SimpleRect(Rectangle Rect)
        {
            return new Rectangle(0, 0, Rect.Width, Rect.Height);
        }
        public static Rectangle SimpleRect(Point Pt)
        {
            return new Rectangle(Pt.X, Pt.Y, 1, 1);
        }
        public static Rectangle SimpleRect(Point Pt, int SizeValue)
        {
            Rectangle rect = new Rectangle(Pt.X - SizeValue, Pt.Y - SizeValue, SizeValue << 1, SizeValue << 1);
            return rect;
        }
        public static Rectangle SimpleRect(PointF PtF)
        {
            Point Pt = new Point((int)PtF.X, (int)PtF.Y);

            return SimpleRect(Pt);
        }
        public static Rectangle SimpleRect(PointF PtF, int SizeValue)
        {
            Point Pt = new Point((int)PtF.X, (int)PtF.Y);

            return SimpleRect(Pt, SizeValue);
        }
        public static Rectangle GetCenterBiasRect(Rectangle FromRect, Point FromPt, Point ToPt)
        {
            Rectangle rect = FromRect;

            rect.X += (ToPt.X - FromPt.X);
            rect.Y += (ToPt.Y - FromPt.Y);

            return rect;
        }
  
        public static Rectangle SimpleRect(Point Pt, int Width, int Height)
        {
            Rectangle rect = SimpleRect(Pt);
            rect.Inflate(Width, Height);

            return rect;
        }
        public static Point SubPoint(Point Pt1, Point Pt2)
        {
            return new Point(Pt1.X - Pt2.X, Pt1.Y - Pt2.Y);
        }

        public static PointF AddPointF(PointF Pt1, PointF Pt2)
        {
            return new PointF(Pt1.X + Pt2.X, Pt1.Y +  Pt2.Y);
        }
        public static PointF SubPointF(PointF Pt1, PointF Pt2)
        {
            return new PointF(Pt1.X - Pt2.X, Pt1.Y - Pt2.Y);
        }

        public static int GetPointLength(Point P1, Point P2)
        {
            return (int)Math.Sqrt(Math.Pow((P1.X - P2.X), 2) + Math.Pow((P1.Y - P2.Y), 2));
        }
        public static double GetPointLengthD(Point P1, Point P2)
        {
            return Math.Sqrt((double)Math.Pow((P1.X - P2.X), 2) + Math.Pow((P1.Y - P2.Y), 2));
        }
        public static double GetPointLength(PointF P1, PointF P2)
        {
            return Math.Sqrt((double)Math.Pow((P1.X - P2.X), 2) + Math.Pow((P1.Y - P2.Y), 2));
        }
        
        public static double GetLTSuggestion(Point P1, Point P2, Rectangle Rect)
        {
            double Xp = (double)(P1.X - P2.X) / (double)(Rect.Width >> 1);
            double Yp = (double)(P1.Y - P2.Y) / (double)(Rect.Height >> 1);

            return Math.Pow(Xp, 2) + Math.Pow(Yp, 2);
        }
        public static double GetPointTan(Point P1, Point P2)
        {
            //return (int)Math.Sqrt(Math.Pow((P1.X - P2.X), 2) + Math.Pow((P1.Y - P2.Y), 2));

            //if (P1.X == P2.X)
            //    return 90;
            //else
            return Math.Atan((double)(P1.Y - P2.Y) / ((double)(P1.X - P2.X))) * 180 / Math.PI;
        }
        public static PointF GetRectCenterF(RectangleF Rect)
        {
            return new PointF(Rect.X + (Rect.Width / 2f), Rect.Y + (Rect.Height / 2f));
        }
        public static Point GetRectCenter(Size Size)
        {
            Rectangle Rect = JzTools.SimpleRect(Size);
            return new Point(Rect.X + (int)Round((double)Rect.Width / 2d), Rect.Y + (int)Round((double)Rect.Height / 2d));
        }
        public static double Round(double FromValue)
        {
            double ret = Math.Floor(FromValue);

            if (FromValue - ret >= 0.5)
            {
                ret = Math.Ceiling(FromValue);
            }
            else
            {
                ret = Math.Floor(FromValue);
            }

            return ret;
        }
        public static double RoundDown(double Org, int digit)
        {
            string Str = Org.ToString("0.000000");

            return double.Parse(RemoveLastChar(Str, 6 - digit));
        }

        public static PointF PointFConvert(Point PT, float ratio)
        {
            PointF PTF = new PointF();

            PTF.X = (float)PT.X / ratio;
            PTF.Y = (float)PT.Y / ratio;

            return PTF;
        }
        public static Point PointFConvert(PointF PTF)
        {
            return new Point((int)PTF.X, (int)PTF.Y);
        }

        static Bitmap bmpTMP = new Bitmap(1, 1);
        public static void GetBMP(string BMPFileStr, ref Bitmap BMP)
        {
            if (File.Exists(BMPFileStr))
            {
                bmpTMP.Dispose();
                bmpTMP = new Bitmap(BMPFileStr);
            }

            BMP.Dispose();
            BMP = new Bitmap(bmpTMP);

            bmpTMP.Dispose();
            bmpTMP = new Bitmap(1, 1);
        }
        public static void GetBMP(Rectangle rect, ref Bitmap BMP)
        {
            bmpTMP.Dispose();
            bmpTMP = new Bitmap(BMP);

            BMP.Dispose();

            rect = JzTools.BoundRect(rect, bmpTMP.Size);

            BMP = bmpTMP.Clone(rect, PixelFormat.Format24bppRgb);

            bmpTMP.Dispose();
            bmpTMP = new Bitmap(1, 1);
        }
        public static void SaveBMP(string BMPFileStr, ref Bitmap BMP)
        {
            bmpTMP.Dispose();
            bmpTMP = new Bitmap(BMP);

            bmpTMP.Save(BMPFileStr,ImageFormat.Bmp);

            bmpTMP.Dispose();
            bmpTMP = new Bitmap(1, 1);
        }
        public static void SaveBMP(string BMPFileStr, ref Bitmap BMP, ImageFormat imgformat)
        {
            Bitmap bmpTMP = new Bitmap(BMP);

            bmpTMP.Save(BMPFileStr, imgformat);

            bmpTMP.Dispose();
        }

        public static void CreateDirectory(string PathName)
        {
            if(!Directory.Exists(PathName))
                Directory.CreateDirectory(PathName);
        }
        public static void CreateFile(string FileName)
        {
            if (!File.Exists(FileName))
            {
                JzTools.SaveData("", FileName);
            }
        }

        public static void SetBrightContrast(Bitmap bmp, int brightvalue, int contrastvalue)
        {
            if (brightvalue == 0 && contrastvalue == 0)
                return;

            SetBrightContrast(bmp, SimpleRect(bmp.Size), brightvalue, contrastvalue);
        }
        public static void SetBrightContrast(Bitmap bmp, Rectangle Rect, int brightvalue, int contrastvalue)
        {
            if (brightvalue == 0 && contrastvalue == 0)
                return;

            int Grade = 0;
            double contrast = (100.0 + contrastvalue) / 100.0;
            contrast *= contrast;

            double ContrastGrade = 0;

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

                    bmp.UnlockBits(bmpData);
                }
            }
            catch (Exception e)
            {
                string Str = e.ToString();

                bmp.UnlockBits(bmpData);
            }
        }

        public static string ArrayToString(bool[] array)
        {
            string Str = "";

            foreach (bool cell in array)
            {
                Str += (cell ? "1," : "0,");
            }
            return RemoveLastChar(Str, 1);
        }
        public static void StringToArray(string Str,ref bool[] array)
        {
            string[] strs = Str.Split(',');

            int i=0;

            foreach (string str in strs)
            {
                array[i] = (str == "1");

                i++;
            }
        }

        public static bool IsBiggerRatio(double val1, double val2, double ratio)
        {
            double Min = Math.Min(val1, val2);
            double Max = Math.Max(val1, val2);

            return (Max / Min) > ratio;

        }

        public static void FindSlopeEquation(Point Pt1, Point Pt2, ref double vslope,ref double vintercept,ref Point ptcenter)
        {
            //Y = Slope * X + Intercept

            double slope = (double)(Pt2.Y - Pt1.Y) / (double)(Pt2.X - Pt1.X);
            double intercept = (double)Pt1.Y - slope * (double)Pt1.X;

            ptcenter = new Point((Pt1.X + Pt2.X) >> 1, (Pt1.Y + Pt2.Y) >> 1);

            vslope = -(double)(Pt2.X - Pt1.X) / (double)(Pt2.Y - Pt1.Y);
            vintercept = (double)ptcenter.Y - vslope * (double)ptcenter.X;
        }
        public static void FindCircleEquation(Point Pt1, Point Pt2, Point Pt3, ref Point ptcirclecenter)
        {
            double Slope1 = 0;
            double Intersect1 = 0;
            Point pt1center = new Point();

            double Slope2 = 0;
            double Intersect2 = 0;
            Point pt2center = new Point();

            FindSlopeEquation(Pt1, Pt3, ref Slope1, ref Intersect1, ref pt1center);
            FindSlopeEquation(Pt2, Pt3, ref Slope2, ref Intersect2, ref pt2center);

            ptcirclecenter.X = (int)-((Intersect1 - Intersect2) / (Slope1 - Slope2));
            ptcirclecenter.Y = (int)(Slope1 * (-((Intersect1 - Intersect2) / (Slope1 - Slope2))) + Intersect1);
        }

        public static double GetAngle(int D, int L)
        {
            return Math.Atan((double)D / (double)L);
        }
        public static double GetContactAngle(double D, double L)
        {
            return 2 * (Math.Atan(D / L) * 180d / Math.PI);
        }
        public static void GetYFromEquation(double slope, double intersection, ref Point ptXgood)
        {
            ptXgood.Y = (int)((double)ptXgood.X * slope + intersection);
        }
        public static void GetXFromEquation(double slope, double intersection, ref Point ptYgood)
        {
            ptYgood.X = (int)(((double)ptYgood.Y - intersection) / slope);
        }

        public static string ValueToHEX(long Value, int Length)
        {
            return ("00000000" + Value.ToString("X")).Substring(("00000000" + Value.ToString("X")).Length - Length, Length);
        }

        public static double GetRandom(double minval, double maxval)
        {
            Random rnd = new Random();

            double diffval = maxval -minval;

            diffval = diffval * (double)rnd.Next(1, 17) / 17d;

            return minval + diffval;
        }


        public static Point PointFToPoint(PointF PT)
        {
            return new Point((int)PT.X, (int)PT.Y);
        }





    }
}
