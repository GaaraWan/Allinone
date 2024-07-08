using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

using JetEazy.BasicSpace;
using JetEazy;

namespace JzKHC.AOISpace
{
    class Found
    {
        public Rectangle rect;

        public int Width = 0;
        public int Height = 0;
        public Point Location = new Point();
        public Point Center = new Point();
        public int Area = 0;
        protected JzToolsClass JzTools = new JzToolsClass();

        public Found(Rectangle rRect,int rArea)
        {
            rect = rRect;
            Width = rect.Width;
            Height = rect.Height;
            Location = rect.Location;
            Area = rArea;

            Center = new Point(rect.X + (rect.Width >> 1), rect.Y + (rect.Height >> 1));
        }
        public override string ToString()
        {
            return JzTools.RecttoString(rect);
        }
    }


    class FindObjectClass
    {
        public List<Found> FoundList = new List<Found>();
        protected JzToolsClass JzTools = new JzToolsClass();
        bool IsDebug
        {
            get
            {
                return false;
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

            foreach (Found found in FoundList)
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



            Rectangle retRect = new Rectangle(0,0,1,1);


            //if (FoundList.Count > 0)
            //    retRect = FoundList[0].rect;

            //retRect.Y = -100;
            int MaxDistance = -10000;

            foreach (Found found in FoundList)
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
                
                /*
                if (found.rect.Y > retRect.Y)
                {
                    retRect = found.rect;
                }
                 */

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
        public Rectangle GetRectNearestFX(Point NearCenter, Rectangle rectBase, int XRange, int YMinRange, int YMaxRange)
        {
            RectList.Clear();

            Rectangle retRect = new Rectangle(0, 0, 1, 1);


            //if (FoundList.Count > 0)
            //    retRect = FoundList[0].rect;

            //retRect.Y = -100;
            int MaxDistance = -10000;

            foreach (Found found in FoundList)
            {
                if (found.rect.Width < 20 || found.rect.Height < 10 || found.rect.Y < 5)
                    continue;


                //if (JzTools.IsInRange(found.rect.Width, rectBase.Width, (int)((double)rectBase.Width * 0.7)) && JzTools.IsInRange(found.rect.Height, rectBase.Height, (int)((double)rectBase.Height * 0.7)))
                //{
                //    //if (MinDistance > Math.Abs(JzTools.GetPointLength(NearCenter, JzTools.GetRectCenter(found.rect))))
                //    //{
                //    //    MinDistance = Math.Abs(JzTools.GetPointLength(NearCenter, JzTools.GetRectCenter(found.rect)));
                //    //    retRect = found.rect;
                //    //}

                //    if (Math.Abs(NearCenter.X - JzTools.GetRectCenter(found.rect).X) < XRange && ((JzTools.GetRectCenter(found.rect).Y - NearCenter.Y > YMinRange && JzTools.GetRectCenter(found.rect).Y - NearCenter.Y < YMaxRange)))
                //    {
                //        RectList.Add(found.rect);
                //        if (MaxDistance < Math.Abs(NearCenter.Y - JzTools.GetRectCenter(found.rect).Y))
                //        {
                //            MaxDistance = Math.Abs(NearCenter.Y - JzTools.GetRectCenter(found.rect).Y);
                //            retRect = found.rect;
                //        }
                //    }
                //}

                if (found.rect.Y > retRect.Y)
                {
                    retRect = found.rect;
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

        public Rectangle GetRectNearestAllinone(Point NearCenter, Rectangle rectBase, int YRange, int XMinRange, int XMaxRange)
        {
            RectList.Clear();

            Rectangle retRect = new Rectangle();
            int MinDistance = 10000;

            foreach (Found found in FoundList)
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

                    if (Math.Abs(NearCenter.Y - JzTools.GetRectCenter(found.rect).Y) < YRange && ((NearCenter.X - JzTools.GetRectCenter(found.rect).X > XMinRange && NearCenter.X - JzTools.GetRectCenter(found.rect).X < XMaxRange)))
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
        public Rectangle GetRectNearestEXAllinone(Point NearCenter, Rectangle rectBase, int YRange, int XMinRange, int XMaxRange)
        {
            RectList.Clear();



            Rectangle retRect = new Rectangle(0, 0, 1, 1);


            //if (FoundList.Count > 0)
            //    retRect = FoundList[0].rect;

            //retRect.Y = -100;
            int MaxDistance = -10000;

            foreach (Found found in FoundList)
            {
                if (found.rect.Width < 20 || found.rect.Height < 10 || found.rect.X < 5)
                    continue;


                if (JzTools.IsInRange(found.rect.Width, rectBase.Width, (int)((double)rectBase.Width * 0.7)) && JzTools.IsInRange(found.rect.Height, rectBase.Height, (int)((double)rectBase.Height * 0.7)))
                {
                    //if (MinDistance > Math.Abs(JzTools.GetPointLength(NearCenter, JzTools.GetRectCenter(found.rect))))
                    //{
                    //    MinDistance = Math.Abs(JzTools.GetPointLength(NearCenter, JzTools.GetRectCenter(found.rect)));
                    //    retRect = found.rect;
                    //}

                    if (Math.Abs(NearCenter.Y - JzTools.GetRectCenter(found.rect).Y) < YRange && ((NearCenter.X - JzTools.GetRectCenter(found.rect).X > XMinRange && NearCenter.X - JzTools.GetRectCenter(found.rect).X < XMaxRange)))
                    {
                        RectList.Add(found.rect);
                        if (MaxDistance < Math.Abs(NearCenter.X - JzTools.GetRectCenter(found.rect).X))
                        {
                            MaxDistance = Math.Abs(NearCenter.X - JzTools.GetRectCenter(found.rect).X);
                            retRect = found.rect;
                        }
                    }
                }

                /*
                if (found.rect.Y > retRect.Y)
                {
                    retRect = found.rect;
                }
                 */

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
        public Rectangle GetRectNearestFXAllinone(Point NearCenter, Rectangle rectBase, int YRange, int XMinRange, int XMaxRange)
        {
            RectList.Clear();

            Rectangle retRect = new Rectangle(0, 0, 1, 1);


            //if (FoundList.Count > 0)
            //    retRect = FoundList[0].rect;

            //retRect.Y = -100;
            int MaxDistance = -10000;

            foreach (Found found in FoundList)
            {
                if (found.rect.Width < 20 || found.rect.Height < 10 || found.rect.X < 5)
                    continue;


                //if (JzTools.IsInRange(found.rect.Width, rectBase.Width, (int)((double)rectBase.Width * 0.7)) && JzTools.IsInRange(found.rect.Height, rectBase.Height, (int)((double)rectBase.Height * 0.7)))
                //{
                //    //if (MinDistance > Math.Abs(JzTools.GetPointLength(NearCenter, JzTools.GetRectCenter(found.rect))))
                //    //{
                //    //    MinDistance = Math.Abs(JzTools.GetPointLength(NearCenter, JzTools.GetRectCenter(found.rect)));
                //    //    retRect = found.rect;
                //    //}

                //    if (Math.Abs(NearCenter.X - JzTools.GetRectCenter(found.rect).X) < XRange && ((JzTools.GetRectCenter(found.rect).Y - NearCenter.Y > YMinRange && JzTools.GetRectCenter(found.rect).Y - NearCenter.Y < YMaxRange)))
                //    {
                //        RectList.Add(found.rect);
                //        if (MaxDistance < Math.Abs(NearCenter.Y - JzTools.GetRectCenter(found.rect).Y))
                //        {
                //            MaxDistance = Math.Abs(NearCenter.Y - JzTools.GetRectCenter(found.rect).Y);
                //            retRect = found.rect;
                //        }
                //    }
                //}

                if (found.rect.X > retRect.X)
                {
                    retRect = found.rect;
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
                int MaxIndex = GetMaxRectIndex();

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
        public int GetArea(int Index)
        {
            if (FoundList.Count == 0)
                return -1;
            else
                return FoundList[Index].Area;
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
        public int GetMaxRectIndex()
        {
            int MaxArea = -100;
            int retIndex = -1;
            int i = 0;

            foreach (Found found in FoundList)
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

        public FindObjectClass()
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

        public void Find(Bitmap bmp,Rectangle Rect, Color FillColor, Size OKSize, Size NGSize)
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
                                    FoundList.Add(new Found(new Rectangle(LEFT, TOP, WIDTH, HEIGHT),AREA));
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
                bmp.UnlockBits(bmpData);
                JetEazy.LoggerClass.Instance.WriteException(ex);
                if (IsDebug)
                    MessageBox.Show("Error :" + ex.ToString());
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
                                        FoundList.Add(new Found(new Rectangle(LEFT, TOP, WIDTH, HEIGHT),AREA));
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
                                        FoundList.Add(new Found(new Rectangle(LEFT, TOP, WIDTH, HEIGHT), AREA));
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

        public void Find(Bitmap bmp, Point PtStart, Color FillColor)
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

                    if (pucStart[0] == 0xFF) //<== ¸T¤î¶ñÂÅ¦â!!!!¡A¶È¬õ¡Aºñ¡A¶À¥Î¸û¦n
                    {
                        Reset(x, y);
                        Find(iStride, pucStart, x, y, rectbmp, ColorValue);

                        WIDTH = RIGHT - LEFT + 1;
                        HEIGHT = BOTTOM - TOP + 1;
                        RECTAREA = WIDTH * HEIGHT;

                        FoundList.Add(new Found(new Rectangle(LEFT, TOP, WIDTH, HEIGHT), AREA));
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

                        FoundList.Add(new Found(new Rectangle(LEFT, TOP, WIDTH, HEIGHT), AREA));
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

            //*((uint*)pucPtr) = FillColorValue;
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
            //*((uint*)pucPtr) = 0xFFFFFFFF;
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

    }

    class FindCornerClass
    {
        List<int> XWay = new List<int>();
        List<int> YWay = new List<int>();
        protected JzToolsClass JzTools = new JzToolsClass();
        bool IsDebug
        {
            get
            {
                return false;
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

        public void Find(Bitmap bmp,CornerEnum Corner,Rectangle Rect)
        {
            Rectangle rectbmp = Rect;
            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr Scan0 = bmpData.Scan0;

            Reset();

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

                    int EndLength = 1;

                    //bool IsEnded = false;
                    //int XDirection = 4;

                    switch (Corner)
                    {
                        case CornerEnum.LT:

                            xmin = rectbmp.X;
                            ymin = rectbmp.Y;

                            xmax = xmin + rectbmp.Width;
                            ymax = ymin + rectbmp.Height;

                            iStride = bmpData.Stride;

                            EndPoint = rectbmp.Location;

                            #region LT Method
                            x = xmin;
                            y = ymin;
                            pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                            //IsEnded = false;
                            while (y < ymax)
                            {
                                YWay.Add(0);
                                x = xmin;
                                pucPtr = pucStart;
                                while (x < xmax)
                                {
                                    if (pucPtr[0] == 0xFF) //<== ¸T¤î¶ñÂÅ¦â!!!!¡A¶È¬õ¡Aºñ¡A¶À¥Î¸û¦n
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        *((uint*)pucPtr) = 0xFFFF0000;
                                        YWay[y - ymin]++;
                                    }


                                    pucPtr += 4;
                                    x++;
                                }

                                if (YWay[y - ymin] == EndLength)
                                    break;

                                pucStart += iStride;
                                y++;
                            }

                            x = xmin;
                            y = ymin;
                            pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                            while (x < xmax)
                            {
                                XWay.Add(0);

                                y = ymin;
                                pucPtr = pucStart;
                                while (y < ymax)
                                {
                                    if (pucPtr[0] == 0xFF) //<== ¸T¤î¶ñÂÅ¦â!!!!¡A¶È¬õ¡Aºñ¡A¶À¥Î¸û¦n
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        *((uint*)pucPtr) = 0xFFFFFF00;
                                        XWay[x - xmin]++;
                                    }

                                    pucPtr += iStride;
                                    y++;
                                }

                                if (XWay[x - xmin] == EndLength)
                                    break;

                                pucStart += 4;
                                x++;
                            }


                           if (XWay.Count >= YWay.Count)
                            {
                                EndPoint.Y = EndPoint.Y + XWay[YWay.Count - 1];
                                EndPoint.X = EndPoint.X + XWay.IndexOf(XWay[YWay.Count - 1]);

                            }
                            else
                            {
                                EndPoint.X = EndPoint.X + YWay[XWay.Count - 1];
                                EndPoint.Y = EndPoint.Y + YWay.IndexOf(YWay[XWay.Count - 1]);
                            }
                            #endregion

                            break;

                        case CornerEnum.RT:

                            xmin = rectbmp.X;
                            ymin = rectbmp.Y;

                            xmax = xmin + rectbmp.Width - 1;
                            ymax = ymin + rectbmp.Height;

                            //XDirection = 4;
                            iStride = bmpData.Stride;

                            EndPoint = new Point(rectbmp.X + rectbmp.Width - 1, rectbmp.Y);

                            #region RT Method
                            x = xmax;
                            y = ymin;
                            pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                            //IsEnded = false;
                            while (y < ymax)
                            {
                                YWay.Add(0);
                                x = xmax;
                                pucPtr = pucStart;
                                while (x >= xmin)
                                {
                                    if (pucPtr[0] == 0xFF) //<== ¸T¤î¶ñÂÅ¦â!!!!¡A¶È¬õ¡Aºñ¡A¶À¥Î¸û¦n
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        *((uint*)pucPtr) = 0xFFFF0000;
                                        YWay[y - ymin]++;
                                    }

                                    pucPtr -= 4;
                                    x--;
                                }

                                if (YWay[y - ymin] == EndLength)
                                    break;

                                pucStart += iStride;
                                y++;
                            }

                            x = xmax;
                            y = ymin;
                            pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                            while (x >= xmin)
                            {
                                XWay.Add(0);

                                y = ymin;
                                pucPtr = pucStart;
                                while (y < ymax)
                                {
                                    if (pucPtr[0] == 0xFF) //<== ¸T¤î¶ñÂÅ¦â!!!!¡A¶È¬õ¡Aºñ¡A¶À¥Î¸û¦n
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        *((uint*)pucPtr) = 0xFFFFFF00;
                                        XWay[xmax - x]++;
                                    }

                                    pucPtr += iStride;
                                    y++;
                                }

                                if (XWay[xmax - x] == EndLength)
                                    break;

                                pucStart -= 4;
                                x--;
                            }


                            if (XWay.Count >= YWay.Count)
                            {
                                EndPoint.Y = EndPoint.Y + XWay[YWay.Count - 1];
                                EndPoint.X = EndPoint.X - XWay.IndexOf(XWay[YWay.Count - 1]);

                            }
                            else
                            {
                                EndPoint.X = EndPoint.X - YWay[XWay.Count - 1];
                                EndPoint.Y = EndPoint.Y + YWay.IndexOf(YWay[XWay.Count - 1]);
                            }
                            #endregion
                            break;
                        case CornerEnum.LB:

                            xmin = rectbmp.X;
                            ymin = rectbmp.Y;

                            xmax = xmin + rectbmp.Width;
                            ymax = ymin + rectbmp.Height - 1;

                            //XDirection = 4;
                            iStride = bmpData.Stride;

                            EndPoint = new Point(rectbmp.X, rectbmp.Y + rectbmp.Height - 1);

                            #region LB Method
                            x = xmin;
                            y = ymax;
                            pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                            //IsEnded = false;
                            while (y >= ymin)
                            {
                                YWay.Add(0);
                                x = xmin;
                                pucPtr = pucStart;
                                while (x < xmax)
                                {
                                    if (pucPtr[0] == 0xFF) //<== ¸T¤î¶ñÂÅ¦â!!!!¡A¶È¬õ¡Aºñ¡A¶À¥Î¸û¦n
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        *((uint*)pucPtr) = 0xFFFF0000;
                                        YWay[ymax - y]++;
                                    }

                                    pucPtr += 4;
                                    x++;
                                }

                                if (YWay[ymax - y] == EndLength)
                                    break;

                                pucStart -= iStride;
                                y--;
                            }

                            x = xmin;
                            y = ymax;
                            pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                            while (x < xmax)
                            {
                                XWay.Add(0);

                                y = ymax;
                                pucPtr = pucStart;
                                while (y >= ymin)
                                {
                                    if (pucPtr[0] == 0xFF) //<== ¸T¤î¶ñÂÅ¦â!!!!¡A¶È¬õ¡Aºñ¡A¶À¥Î¸û¦n
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        *((uint*)pucPtr) = 0xFFFFFF00;
                                        XWay[x - xmin]++;
                                    }

                                    pucPtr -= iStride;
                                    y--;
                                }

                                if (XWay[x - xmin] == EndLength)
                                    break;

                                pucStart += 4;
                                x++;
                            }


                            if (XWay.Count >= YWay.Count)
                            {
                                EndPoint.Y = EndPoint.Y - XWay[YWay.Count - 1];
                                EndPoint.X = EndPoint.X + XWay.IndexOf(XWay[YWay.Count - 1]);
                            }
                            else
                            {
                                EndPoint.X = EndPoint.X + YWay[XWay.Count - 1];
                                EndPoint.Y = EndPoint.Y - YWay.IndexOf(YWay[XWay.Count - 1]);
                            }
                            #endregion
                            break;
                        case CornerEnum.RB:

                            xmin = rectbmp.X;
                            ymin = rectbmp.Y;

                            xmax = xmin + rectbmp.Width - 1;
                            ymax = ymin + rectbmp.Height - 1;

                            //XDirection = 4;
                            iStride = bmpData.Stride;

                            EndPoint = new Point(rectbmp.X + rectbmp.Width - 1, rectbmp.Y + rectbmp.Height - 1);

                            #region RB Method
                            x = xmax;
                            y = ymax;
                            pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                            //IsEnded = false;
                            while (y >= ymin)
                            {
                                YWay.Add(0);
                                x = xmax;
                                pucPtr = pucStart;
                                while (x >= xmin)
                                {
                                    if (pucPtr[0] == 0xFF) //<== ¸T¤î¶ñÂÅ¦â!!!!¡A¶È¬õ¡Aºñ¡A¶À¥Î¸û¦n
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        *((uint*)pucPtr) = 0xFFFF0000;
                                        YWay[ymax - y]++;
                                    }

                                    pucPtr -= 4;
                                    x--;
                                }

                                if (YWay[ymax - y] == EndLength)
                                    break;

                                pucStart -= iStride;
                                y--;
                            }

                            x = xmax;
                            y = ymax;
                            pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                            while (x >= xmin)
                            {
                                XWay.Add(0);

                                y = ymax;
                                pucPtr = pucStart;
                                while (y >= ymin)
                                {
                                    if (pucPtr[0] == 0xFF) //<== ¸T¤î¶ñÂÅ¦â!!!!¡A¶È¬õ¡Aºñ¡A¶À¥Î¸û¦n
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        *((uint*)pucPtr) = 0xFFFFFF00;
                                        XWay[xmax - x]++;
                                    }

                                    pucPtr -= iStride;
                                    y--;
                                }

                                if (XWay[xmax - x] == EndLength)
                                    break;

                                pucStart -= 4;
                                x--;
                            }


                            if (XWay.Count >= YWay.Count)
                            {
                                EndPoint.Y = EndPoint.Y - XWay[YWay.Count - 1];
                                EndPoint.X = EndPoint.X - XWay.IndexOf(XWay[YWay.Count - 1]);
                            }
                            else
                            {
                                EndPoint.X = EndPoint.X - YWay[XWay.Count - 1];
                                EndPoint.Y = EndPoint.Y - YWay.IndexOf(YWay[XWay.Count - 1]);
                            }
                            #endregion
                            break;
                    }

                    CutLength = Math.Min(XWay.Count, YWay.Count);
                }
                bmp.UnlockBits(bmpData);
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
