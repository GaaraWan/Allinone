using JetEazy.UISpace;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using static System.Windows.Forms.MonthCalendar;
using AHBlobPro;
using System.Collections.Generic;
using System.Linq;

namespace Allinone.BasicSpace
{
    public class JzCheckRecipeParaClass
    {
        public JzCheckRecipeParaClass()
        {

        }
        const string cat1 = "00.图像设定";

        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("A01.灰阶阈值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 255)]
        [Browsable(true)]
        public int nThresholdValue { get; set; } = 150;
        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("A02.最小面积")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 99999999)]
        [Browsable(true)]
        public int nAreaMin { get; set; } = 1000;

        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("A03.最大面积")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 99999999)]
        [Browsable(true)]
        public int nAreaMax { get; set; } = 1000000;

        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("A04.长宽比例")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100)]
        [Browsable(true)]
        public float nRatio { get; set; } = 30;

        const string cat0 = "01.切换参数设定";

        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("A0.开启检测个数")]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100000)]
        [Browsable(true)]
        public bool IsOpenFindCount { get; set; } = false;
        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("A2.寻找区域")]
        public RectangleF RectF { get; set; } = new RectangleF(0, 0, 100, 100);
        [CategoryAttribute(cat0), DescriptionAttribute("")]
        //[DisplayName("A3.拍照位置")]
        //public string MotorPositionStr { get; set; } = "0,0,0";


        public long CheckTime = 0;
        public Bitmap bmpOutput = new Bitmap(1, 1);

        public int GetBottomCount(Bitmap ebmpInput, bool istrain = true)
        {
            CheckTime = 0;
            int iret = -1;

            if (!IsOpenFindCount)
                return iret;

            if (ebmpInput == null)
                return iret;
            int iRange = nThresholdValue;

            Stopwatch watch = new Stopwatch();
            watch.Start();
            int iSized = 1;
            Bitmap bmpSized = (Bitmap)ebmpInput.Clone(RectF, System.Drawing.Imaging.PixelFormat.Format24bppRgb);//   new Bitmap(ebmpInput, ebmpInput.Width / iSized, ebmpInput.Height / iSized);
            bmpOutput.Dispose();
            bmpOutput = new Bitmap(ebmpInput);

            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            bmpSized = grayscale.Apply(bmpSized);
            AForge.Imaging.Filters.Threshold threshold = new AForge.Imaging.Filters.Threshold(iRange);
            bmpSized = threshold.Apply(bmpSized);

            Bitmap bmp = new Bitmap(bmpSized);

            int penWidth = 1;
            Font MyFontX = new Font("Arial", 18);

            JetGrayImg grayimage = new JetGrayImg(bmp);
            JetImgproc.Threshold(grayimage, iRange, grayimage);
            JetBlob jetBlob = new JetBlob();

            //if (cboFindMode.Text == "白底黑字")
            //    jetBlob.Labeling(grayimage, JConnexity.Connexity4, JBlobLayer.BlackLayer);
            //else
            jetBlob.Labeling(grayimage, JConnexity.Connexity4, JBlobLayer.WhiteLayer);
            int icount = jetBlob.BlobCount;
            //watch.Stop();
            //CheckTime = watch.ElapsedMilliseconds;
            //this.Text = "用时: " + watch.ElapsedMilliseconds + " ms";// + " ms 共找到: " + icount + " 个斑点";
           

            float set_area_value = nAreaMin;
            //float.TryParse(txtArea.Text.Trim(), out set_area_value);

            float set_area_valueMax = nAreaMax;
            //float.TryParse(txtAreaMax.Text.Trim(), out set_area_valueMax);

            float set_ratio_value = nRatio;
            //float.TryParse(txtRatio.Text.Trim(), out set_ratio_value);

            int.TryParse("3", out penWidth);

            string _collectIndex = string.Empty;
            List<CheckRectangle> boxes = new List<CheckRectangle>();

            for (int i = 0; i < icount; i++)
            {
                int iArea = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.Area);
                JRotatedRectangleF jetrect = JetBlobFeature.ComputeMinRectangle(jetBlob, i);
                if (iArea > set_area_value
                    && iArea < set_area_valueMax
                    && IsInRangeRatio(jetrect.fWidth, jetrect.fHeight, set_ratio_value)
                    )
                {
                    //Point ptCenter = new Point((int)jetrect.fCX, (int)jetrect.fCY);

                    CheckRectangle check = new CheckRectangle(i, jetrect.fCX, jetrect.fCY);
                    boxes.Add(check);
                }
            }

            // 定义 Y 值的波动范围阈值
            double thresholdx = 15;

            if (boxes.Count > 0)
            {
                // 按 Y 坐标排序
                var sortedBoxes = boxes.OrderBy(b => b.Y).ToList();

                // 获取最小的 Y 值（最下面一排的基准线）
                //double minY = sortedBoxes.First().Y;
                double minY = sortedBoxes.Last().Y;

                // 筛选出 Y 值在 [minY, minY + threshold] 范围内的方框
                var bottomRowBoxes = boxes.Where(b => b.Y >= minY - thresholdx && b.Y <= minY + thresholdx).ToList();

                // 输出结果
                Console.WriteLine("最下面一排的方框个数: " + bottomRowBoxes.Count);
                Console.WriteLine("最下面一排的方框坐标:");
                foreach (var box in bottomRowBoxes)
                {
                    Console.WriteLine($"(X: {box.X}, Y: {box.Y})");
                    _collectIndex += $";{box.Index};";
                }
                Console.WriteLine(_collectIndex);
                iret = bottomRowBoxes.Count;
            }

            if (istrain)
            {
                Graphics g = Graphics.FromImage(bmpOutput);
                for (int i = 0; i < icount; i++)
                {
                    int iArea = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.Area);
                    JRotatedRectangleF jetrect = JetBlobFeature.ComputeMinRectangle(jetBlob, i);
                    if (iArea > set_area_value
                        && iArea < set_area_valueMax
                        && IsInRangeRatio(jetrect.fWidth, jetrect.fHeight, set_ratio_value)
                        )

                    {

                        if (_collectIndex.Contains(";" + i.ToString() + ";"))
                        {

                            Point ptCenter = new Point((int)(jetrect.fCX + RectF.X), (int)(jetrect.fCY + RectF.Y));

                            double iWidth = jetrect.fWidth;
                            double iHeight = jetrect.fHeight;
                            if (jetrect.fWidth < jetrect.fHeight)
                            {
                                iWidth = jetrect.fHeight;
                                iHeight = jetrect.fWidth;

                                jetrect.fAngle += 90;
                            }

                            Rectangle myRect = SimpleRect(ptCenter, (int)iWidth / 2, (int)iHeight / 2);


                            string _circleStr = $"面积{iArea.ToString("0.000")} 长{iWidth.ToString("0.000")} 宽{iHeight.ToString("0.000")}";
                            SolidBrush B = new SolidBrush(Color.Red);
                            Font MyFont = new Font("Arial", 10);
                            SolidBrush Bx = new SolidBrush(Color.Red);

                            //坐标系
                            Pen pAxis = new Pen(Color.Blue, 15);
                            pAxis.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                            //转换矩形的四个角
                            Point[] myPts = RectToPoint(myRect, -jetrect.fAngle);

                            //Point[] myPts = RectToPoint(myRect, 0);
                            Pen p = new Pen(Color.Lime, penWidth);
                            p.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;

                            Pen pBottom = new Pen(Color.Red, penWidth);
                            pBottom.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                            g.DrawLine(p, myPts[0], myPts[1]);
                            g.DrawLine(p, myPts[0], myPts[2]);
                            g.DrawLine(p, myPts[1], myPts[3]);
                            g.DrawLine(pBottom, myPts[2], myPts[3]);

                            Point ptStart = GetCenterPoint(myPts[0], myPts[1]);
                            Point ptEnd = GetCenterPoint(myPts[2], myPts[3]);

                            Pen pRobot = new Pen(Color.Lime, penWidth);
                            //pRobot.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                            g.DrawString(_circleStr, new Font("宋体", 3), Brushes.Lime, ptStart);
                            if (myRect.Width <= myRect.Height + 3 && myRect.Width >= myRect.Height - 3)
                                g.DrawLine(pRobot, ptStart, ptEnd);
                            else
                            {
                                g.DrawLine(p, myPts[0], myPts[1]);
                                g.DrawLine(p, myPts[0], myPts[2]);
                                g.DrawLine(p, myPts[1], myPts[3]);
                                g.DrawLine(pBottom, myPts[2], myPts[3]);
                            }

                            g.DrawEllipse(new Pen(Color.Red, 1), myRect);
                            double _length = Math.Sqrt(Math.Pow(ptStart.X - ptEnd.X, 2) + Math.Pow(ptStart.Y - ptEnd.Y, 2));

                        }
                    }

                }
                g.Dispose();
                //bmpSizedPro.Save(Application.StartupPath + "\\result.bmp");
                //picShow.Image = bmpSizedPro;
            }

            bmpSized.Dispose();
            bmp.Dispose();

            watch.Stop();
            CheckTime = watch.ElapsedMilliseconds;


            return iret;

        }

        // 定义一个矩形类，表示方框的位置
        class CheckRectangle
        {
            public int Index { get; }

            public double X { get; }
            public double Y { get; }

            public CheckRectangle(int index, double x, double y)
            {
                Index = index;
                X = x;
                Y = y;
            }
        }


        public void FromingStr(string str)
        {
            string[] parts = str.Split(',');
            if (parts.Length > 4)
            {
                IsOpenFindCount = parts[0] == "1";
                RectF = StringToRectF(parts[1]);
                nThresholdValue = int.Parse(parts[2]);
                nAreaMin = int.Parse(parts[3]);
                nAreaMax = int.Parse(parts[4]);
            }
            if (parts.Length > 5)
            {
                if (!string.IsNullOrEmpty(parts[5]))
                    nRatio = float.Parse(parts[5]);
            }
        }
        public string ToParaString()
        {
            string str = string.Empty;

            str += (IsOpenFindCount ? "1" : "0") + ",";
            str += RectFToString(RectF) + ",";
            str += nThresholdValue.ToString() + ",";
            str += nAreaMin.ToString() + ",";
            str += nAreaMax.ToString() + ",";
            str += nRatio.ToString() + ",";

            return str;
        }

        public string PointF000ToString(PointF PTF)
        {
            return PTF.X.ToString("0.000") + ";" + PTF.Y.ToString("0.000");
        }
        public PointF StringToPointF(string Str)
        {
            string[] strs = Str.Split(';');
            return new PointF(float.Parse(strs[0]), float.Parse(strs[1]));
        }
        public string RectFToString(RectangleF RectF)
        {
            string Str = "";

            Str += RectF.X.ToString("0.00") + ";";
            Str += RectF.Y.ToString("0.00") + ";";
            Str += RectF.Width.ToString("0.00") + ";";
            Str += RectF.Height.ToString("0.00");

            return Str;
        }
        public RectangleF StringToRectF(string Str)
        {
            string[] strs = Str.Split(';');
            RectangleF rectF = new RectangleF();

            rectF.X = float.Parse(strs[0]);
            rectF.Y = float.Parse(strs[1]);
            rectF.Width = float.Parse(strs[2]);
            rectF.Height = float.Parse(strs[3]);

            return rectF;


        }

        //        假设对图片上任意点(x, y)，绕一个坐标点(rx0, ry0)逆时针旋转RotaryAngle角度后的新的坐标设为(x', y')，有公式：
        //    x'= (x - rx0)*cos(RotaryAngle) + (y - ry0)*sin(RotaryAngle) + rx0 ;
        //    y'=-(x - rx0)*sin(RotaryAngle) + (y - ry0)*cos(RotaryAngle) + ry0 ;
        //[csharp]
        //        view plaincopy
        /// <summary>  
        /// 对一个坐标点按照一个中心进行旋转  
        /// </summary>  
        /// <param name="center">中心点</param>  
        /// <param name="p1">要旋转的点</param>  
        /// <param name="angle">旋转角度，笛卡尔直角坐标</param>  
        /// <returns></returns>  
        private Point PointRotate(Point center, Point p1, double angle)
        {
            Point tmp = new Point();
            double angleHude = angle * Math.PI / 180;/*角度变成弧度*/
            double x1 = (p1.X - center.X) * Math.Cos(angleHude) + (p1.Y - center.Y) * Math.Sin(angleHude) + center.X;
            double y1 = -(p1.X - center.X) * Math.Sin(angleHude) + (p1.Y - center.Y) * Math.Cos(angleHude) + center.Y;
            tmp.X = (int)x1;
            tmp.Y = (int)y1;
            return tmp;
        }

        public Rectangle SimpleRect(Point Pt, int Width, int Height)
        {
            Rectangle rect = SimpleRect(Pt);
            rect.Inflate(Width, Height);

            return rect;
        }
        public Rectangle SimpleRect(Point Pt)
        {
            return new Rectangle(Pt.X, Pt.Y, 1, 1);
        }

        public Point[] RectToPoint(Rectangle xRect, double xAngle)
        {
            Point[] pts = new Point[4];

            Point ptCenter = GetRectCenter(xRect);
            pts[0] = xRect.Location;
            pts[1] = new Point(xRect.Location.X, xRect.Bottom);
            pts[2] = new Point(xRect.Right, xRect.Location.Y);
            pts[3] = new Point(xRect.Right, xRect.Bottom);

            pts[0] = PointRotate(ptCenter, pts[0], xAngle);
            pts[1] = PointRotate(ptCenter, pts[1], xAngle);
            pts[2] = PointRotate(ptCenter, pts[2], xAngle);
            pts[3] = PointRotate(ptCenter, pts[3], xAngle);

            return pts;
        }
        public Point GetRectCenter(Rectangle Rect)
        {
            return new Point(Rect.X + (Rect.Width >> 1), Rect.Y + (Rect.Height >> 1));
        }

        public PointF GetCenterPoint(PointF P1, PointF P2)
        {
            return new PointF((P1.X + P2.X) / 2, (P1.Y + P2.Y) / 2);
        }
        public Point GetCenterPoint(Point P1, Point P2)
        {
            return new Point((P1.X + P2.X) / 2, (P1.Y + P2.Y) / 2);
        }

        public bool IsInRangeRatio(double FromValue, double CompValue, double Ratio)
        {
            return (FromValue >= (CompValue * (1 - (Ratio / 100d)))) && (FromValue <= (CompValue * (1 + (Ratio / 100d))));
        }
    }
}
