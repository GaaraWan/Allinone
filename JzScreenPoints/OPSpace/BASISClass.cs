using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;

using JzScreenPoints.ControlSpace;
using JzScreenPoints.BasicSpace;
using JzScreenPoints;

namespace JzScreenPoints.OPSpace
{
    public class BASISClass : OPClass
    {
        const int RectToBeAnalyzed = 13;
        const int AnalyzeStep = 1;
        const int SizeRatio = -2;

        protected new OPTypeEnum OPType = OPTypeEnum.BAS;

        public new string IndexNoStr
        {
            get
            {
                return JzTools.ValueToHEX(GroupIndex, 2) + Index.ToString("000");
            }
        }
        public new string IndexName
        {
            get
            {
                return OPType.ToString() + "-" + IndexNoStr;
            }
        }

        public bool IsCalibration = false;
        public bool IsDatum = false;
        public bool IsDummy = false;

        public int RowTag = 0;

        public PointF ImagePoint = new PointF();
        public PointF RealPoint = new PointF();

        public CircleClass viewcircle = new CircleClass();

        double ContrastRatio
        {
            get
            {
                return (double)Contrast / 200d;
            }
        }

        public int Contrast = 50;

        JzFindObjectClass JzFind = new JzFindObjectClass();

        HistogramClass Histogram = new HistogramClass(2);

        int MinGrade = 0;
        int MaxGrade = 0;

        Rectangle MaxRect = new Rectangle();

        Bitmap bmpORIGIN = new Bitmap(1, 1);
        Bitmap bmpOPERATE = new Bitmap(1, 1);
        Bitmap bmpSized = new Bitmap(1, 1);
        Bitmap bmpTmp = new Bitmap(1, 1);

        public string TestString = "";

        public BASISClass()
        {
            Cell.RelateIndexNoStr = IndexNoStr;
        }

        public BASISClass(string Str)
        {
            FromString(Str);
            Cell.RelateIndexNoStr = IndexNoStr;
        }

        public BASISClass(string Str, int newIndex)
        {
            FromString(Str);

            Index = newIndex;
            Cell.RelateIndexNoStr = IndexNoStr;
        }
        public BASISClass(Rectangle rect, int newIndex, int groupindex)
        {
            Cell.RectCell = rect;

            Cell.CellProperty = CellPropertyEnum.DYNAMIC;

            Index = newIndex;
            GroupIndex = groupindex;

            Cell.RelateIndexNoStr = IndexNoStr;
        }
        public BASISClass(PointF ptf, int newIndex,PointF realptf,int groupindex)
        {
            ImagePoint = ptf;

            Cell.CellProperty = CellPropertyEnum.STATIC;
            Cell.RectCell = JzTools.SimpleRect(new Point((int)ImagePoint.X, (int)ImagePoint.Y), 10);

            GroupIndex = groupindex;
            Index = newIndex;
            Cell.RelateIndexNoStr = IndexNoStr;
            RealPoint = realptf;

            IsCalibration = false;
        }

        public BASISClass(PointF ptf, float width, int newIndex, PointF realptf, int groupindex)
        {
            viewcircle = new CircleClass(ptf, width);

            Cell.CellProperty = CellPropertyEnum.STATIC;
            Cell.RectCell = JzTools.RectFToRect(JzTools.PointFToRectF(ptf, (int)(width * 2)));

            Cell.CircleCell = new CircleClass(ptf, width);

            GroupIndex = groupindex;
            Index = newIndex;
            Cell.RelateIndexNoStr = IndexNoStr;
            RealPoint = realptf;

            IsCalibration = false;
        }

        public void SetCell(PointF imageptf, PointF realptf)
        {

            bool IsUpdated = false;

            if (RealPoint == realptf)
            {
                ImagePoint = imageptf;

                Cell.CellProperty = CellPropertyEnum.STATIC;
                Cell.RectCell = JzTools.SimpleRect(new Point((int)ImagePoint.X, (int)ImagePoint.Y), 10);

                //int i = BorderList.Count - 1;

                //while (i > -1)
                //{
                //    if (!BorderList[i].RefillLine(RealPoint, ImagePoint))
                //    {
                //        BorderList.RemoveAt(i);
                //    }
                //    i--;
                //}
            }

        }

        public void ArrangeBorder()
        {
            
        }

        public BASISClass Clone()
        {
            BASISClass basis = new BASISClass(this.ToString());

            basis.SetBMP(this);

            return basis;
        }

        public void SetBMP(BASISClass basis)
        {
            bmpORIGIN.Dispose();
            bmpORIGIN = new Bitmap(basis.bmpORIGIN);

            bmpOPERATE.Dispose();
            bmpOPERATE = new Bitmap(basis.bmpOPERATE);
        }
        public void SetBMP(Bitmap bmp)
        {
            bmpORIGIN.Dispose();
            bmpORIGIN = new Bitmap(bmp);

            bmpOPERATE.Dispose();
            bmpOPERATE = new Bitmap(bmpORIGIN);

            if(IsCalibration)
                FindBMP(bmpOPERATE);
        }
        public void ArrangeBitmap(Bitmap bmp)
        {
            Bitmap bmpArrangeTmp = new Bitmap(1, 1);

            Rectangle ArrangeRect = Cell.RectCell;

            //ArrangeRect.Inflate(30, 30);
            //ArrangeRect = JzTools.BoundRect(ArrangeRect, bmp.Size);

            bmpArrangeTmp.Dispose();
            bmpArrangeTmp = (Bitmap)bmp.Clone(ArrangeRect, PixelFormat.Format32bppArgb);


            //if(Index == 2)
            //bmpArrangeTmp.Save(@"D:\LOA\TST\ARRANGE.BMP", ImageFormat.Bmp);

            FindBMP(bmpArrangeTmp);

            MaxRect = JzTools.SimpleRect(ImagePoint, 20);

            MaxRect.Inflate(30,30); 

            //MaxRect.X += ArrangeRect.X;
            //MaxRect.Y += ArrangeRect.Y;

            MaxRect = JzTools.BoundRect(MaxRect, bmp.Size);

            Cell.RectCell = MaxRect;

            bmpArrangeTmp.Dispose();
            bmpArrangeTmp = (Bitmap)bmp.Clone(MaxRect, PixelFormat.Format32bppArgb);


            //bmpArrangeTmp.Save(@"D:\LOA\TST\ARRANGE.BMP", ImageFormat.Bmp);

            SetBMP(bmpArrangeTmp);

            bmpArrangeTmp.Dispose();
        }

        public Bitmap GetBMP()
        {
            return bmpOPERATE;
        }

        public void FindBMP(Bitmap bmp)
        {

            bmpSized.Dispose();
            bmpSized = new Bitmap(bmp, JzTools.Resize(bmp.Size, SizeRatio));

            Histogram.GetHistogram(bmpSized);

            MinGrade = Histogram.MinGrade;
            MaxGrade = Histogram.MaxGrade;

            ImagePoint = AnalyzeBMP(bmp);

            #region Test For Distribution

            //JzFind.SetThresholdColor(bmpOPERATE, MinGrade, (int)((double)(MaxGrade - MinGrade) * ContrastRatio), MinGrade, MaxGrade, INI.ISREVERSE);

            #endregion


            //JzTools.DrawRect(bmpOPERATE, MaxRect, new Pen(Color.Red, 2));

            if (!(float.IsNaN(ImagePoint.X) || float.IsNaN(ImagePoint.Y)))
            {
                JzTools.DrawRect(bmpOPERATE, JzTools.SimpleRect(new Point((int)ImagePoint.X, (int)ImagePoint.Y)), new SolidBrush(Color.Red));
                JzTools.DrawRect(bmpOPERATE, JzTools.SimpleRect(new Point((int)ImagePoint.X - 1, (int)ImagePoint.Y - 1), 2, 2), new SolidBrush(Color.Black));
            }
            //bmpProcessed.Save(@"D:\LOA\TESTRESULT\004.BMP", ImageFormat.Bmp);


        }
        PointF AnalyzeBMP(Bitmap bmp)
        {
            try
            {
                int i = 0;
                int j = 0;
                int MaxIndex = 0;

                List<PointF> PtFList = new List<PointF>();
                PointF PtF = new PointF();
                PointF PtFSum = new PointF();

                bmpTmp.Dispose();
                bmpTmp = new Bitmap(bmp);

                //JzFind.SetThreshold(bmpTmp, MinGrade, (int)((double)(MaxGrade - MinGrade) * ContrastRatio), MinGrade);
                //JzFind.SetThreshold(bmpTmp, JzTools.SimpleRect(bmpTmp.Size), MinGrade, (int)((double)(MaxGrade - MinGrade) * ContrastRatio), MinGrade, INI.ISREVERSE);

                //if(Index == 2)
                //    bmpTmp.Save(@"D:\LOA\ARRANGE.BMP", ImageFormat.Bmp);

                //JzFind.Find(bmpTmp, Color.Red);

                //MaxIndex = JzFind.GetMaxRectIndex();

                //j = 0;

                //foreach (Found found in JzFind.FoundList)
                //{
                //    if (j != MaxIndex)
                //    {
                //        JzTools.DrawRect(bmpTmp, found.rect, new SolidBrush(Color.Black));
                //    }
                //    j++;
                //}

                //bmpTmp.Save(@"D:\LOA\ARRANGE.BMP", ImageFormat.Bmp);

                MaxRect = JzTools.SimpleRect(bmpTmp.Size);

                //MaxRect.Inflate(10, 10);

                //MaxRect = JzTools.BoundRect(MaxRect, bmp.Size);

                i = 0;
                PtFList.Clear();

                while (i < RectToBeAnalyzed)
                {
                    bmpTmp.Dispose();
                    bmpTmp = (Bitmap)bmp.Clone(MaxRect, PixelFormat.Format32bppArgb);

                    bmpSized.Dispose();
                    bmpSized = new Bitmap(bmpTmp, JzTools.Resize(bmpTmp.Size, SizeRatio));

                    Histogram.GetHistogram(bmpSized);

                    int UBound = (int)((double)(Histogram.MaxGrade - Histogram.MinGrade)
                        * ((double)(Contrast + (AnalyzeStep * (((RectToBeAnalyzed - 1) >> 1) - i))) / 200d));

                    //JzFind.SetThreshold(bmpTmp, Histogram.MinGrade,
                    //    UBound, 
                    //    Histogram.MinGrade);

                    


                    JzFind.Find(bmpTmp, Color.Red);

                    MaxIndex = JzFind.GetMaxRectIndex();

                    j = 0;
                    foreach (FoundClass found in JzFind.FoundList)
                    {
                        if (j != MaxIndex)
                        {
                            JzTools.DrawRect(bmpTmp, found.rect, new SolidBrush(Color.Black));
                        }
                        j++;
                    }

                    //Find the Center with average points from Up and Down

                    PointF CenterPtF = JzFind.GetCrossCenter(bmpTmp, 10, 2);

                    //if (float.IsNaN(CenterPtF.X) || float.IsNaN(CenterPtF.Y))
                    //    CenterPtF.X = CenterPtF.X;
                    
                    //
                    //PointF CenterPtF = JzTools.GetRectCenterF(JzFind.rectMaxRect);

                    CenterPtF.X += MaxRect.X + Cell.RectCell.X;
                    CenterPtF.Y += MaxRect.Y + Cell.RectCell.Y;

                    

                    PtFList.Add(CenterPtF);

                    i++;
                }

                i = 0;
                j = 0;

                PtF = PtFList[((RectToBeAnalyzed - 1) >> 1) + 1];

                while (i < RectToBeAnalyzed)
                {
                    if (float.IsNaN(PtF.X) || float.IsNaN(PtF.Y))
                    {
                        i++;
                        continue;
                    }

                    if (Math.Abs(PtF.X - PtFList[i].X) < 2 && Math.Abs(PtF.Y - PtFList[i].Y) < 2)
                    {
                        j++;

                        PtFSum.X += PtFList[i].X;
                        PtFSum.Y += PtFList[i].Y;
                    }
                    i++;
                }


                PtFSum.X = PtFSum.X / (float)j;
                PtFSum.Y = PtFSum.Y / (float)j;

                if (PtFSum.X == float.NaN || PtFSum.Y == float.NaN)
                {
                    PtFSum = new PointF(0, 0);
                }

                TestString = j.ToString("00") + " / " + PtFSum.X.ToString("0.000") + "," + PtFSum.Y.ToString("0.000");

                return PtFSum;
            }
            catch(Exception exp)
            {
                return new PointF(0,0);
            }
        }

        public void FindBMPCircle(Bitmap bmp)
        {
            bmpSized.Dispose();
            bmpSized = new Bitmap(bmp, JzTools.Resize(bmp.Size, SizeRatio));

            Histogram.GetHistogram(bmpSized);

            MinGrade = Histogram.MinGrade;
            MaxGrade = Histogram.MaxGrade;

            ImagePoint = AnalyzeBMP(bmp);

            #region Test For Distribution

            JzFind.SetThresholdColor(bmpOPERATE, MinGrade, (int)((double)(MaxGrade - MinGrade) * ContrastRatio), MinGrade, MaxGrade, true);

            #endregion

            JzTools.DrawRect(bmpOPERATE, MaxRect, new Pen(Color.Red, 2));
            JzTools.DrawRect(bmpOPERATE, JzTools.SimpleRect(new Point((int)ImagePoint.X, (int)ImagePoint.Y)), new SolidBrush(Color.Red));
            JzTools.DrawRect(bmpOPERATE, JzTools.SimpleRect(new Point((int)ImagePoint.X - 1, (int)ImagePoint.Y - 1), 2, 2), new SolidBrush(Color.Black));

            //bmpProcessed.Save(@"D:\LOA\TESTRESULT\004.BMP", ImageFormat.Bmp);
        }
        PointF AnalyzeBMPCircle(Bitmap bmp)
        {
            int i = 0;
            int j = 0;

            List<PointF> PtFList = new List<PointF>();
            PointF PtF = new PointF();
            PointF PtFSum = new PointF();

            bmpTmp.Dispose();
            bmpTmp = new Bitmap(bmp);

            //JzFind.SetThreshold(bmpTmp, MinGrade, (int)((double)(MaxGrade - MinGrade) * ContrastRatio), MinGrade);
            JzFind.SetThreshold(bmpTmp, JzTools.SimpleRect(bmpTmp.Size), MinGrade, (int)((double)(MaxGrade - MinGrade) * ContrastRatio), MinGrade, true);

            //if (Index == 2)
            //    bmpTmp.Save(@"D:\LOA\ARRANGE.BMP", ImageFormat.Bmp);

            JzFind.Find(bmpTmp, Color.Red);

            MaxRect = JzFind.rectMaxRect;

            MaxRect.Inflate(10, 10);

            MaxRect = JzTools.BoundRect(MaxRect, bmp.Size);

            i = 0;
            PtFList.Clear();

            while (i < RectToBeAnalyzed)
            {
                bmpTmp.Dispose();
                bmpTmp = (Bitmap)bmp.Clone(MaxRect, PixelFormat.Format32bppArgb);

                bmpSized.Dispose();
                bmpSized = new Bitmap(bmpTmp, JzTools.Resize(bmpTmp.Size, SizeRatio));

                Histogram.GetHistogram(bmpSized);

                int UBound = (int)((double)(Histogram.MaxGrade - Histogram.MinGrade)
                    * ((double)(Contrast + (AnalyzeStep * (((RectToBeAnalyzed - 1) >> 1) - i))) / 200d));

                //JzFind.SetThreshold(bmpTmp, Histogram.MinGrade,
                //    UBound, 
                //    Histogram.MinGrade);

                JzFind.SetThreshold(bmpTmp, JzTools.SimpleRect(bmpTmp.Size),
                    Histogram.MinGrade,
                    UBound,
                    Histogram.MinGrade, true);


                JzFind.Find(bmpTmp, Color.Red);

                PointF CenterPtF = JzTools.GetRectCenterF(JzFind.rectMaxRect);

                #region Find least circle center

                //float radius = 0;
                //QvCircleFit CircleFit = new QvCircleFit();
                //List<Point> BorderList = new List<Point>();

                ////bmpTmp.Save(@"D:\LOA\CIRCLETEST.BMP", ImageFormat.Bmp);

                //JzFind.GetDevideBorder(bmpTmp, Universal.CircleFitCount, JzFind.rectMaxRect, ref BorderList, Color.Red);

                //PointF[] ptfs = new PointF[BorderList.Count];

                //int ipt = 0;

                //foreach (Point pts in BorderList)
                //{
                //    ptfs[ipt] = pts;
                //    ipt++;
                //}

                //CircleFit.Fit(ptfs, out CenterPtF, out radius);

                #endregion

                CenterPtF.X += MaxRect.X + Cell.RectCell.X;
                CenterPtF.Y += MaxRect.Y + Cell.RectCell.Y;

                PtFList.Add(CenterPtF);

                i++;
            }

            i = 0;
            j = 0;

            PtF = PtFList[((RectToBeAnalyzed - 1) >> 1) + 1];

            while (i < RectToBeAnalyzed)
            {
                if (Math.Abs(PtF.X - PtFList[i].X) < 2 && Math.Abs(PtF.Y - PtFList[i].Y) < 2)
                {
                    j++;

                    PtFSum.X += PtFList[i].X;
                    PtFSum.Y += PtFList[i].Y;
                }
                i++;
            }


            PtFSum.X = PtFSum.X / (float)j;
            PtFSum.Y = PtFSum.Y / (float)j;


            TestString = j.ToString("00") + " / " + PtFSum.X.ToString("0.000") + "," + PtFSum.Y.ToString("0.000");

            return PtFSum;
        }

        public void RemoveBorder(List<string> removelist)
        {
            int i = 0;

            foreach (string str in removelist)
            {
                
            }
        }

        public string ToYString()
        {
            return ImagePoint.Y.ToString("000000.00") + "," + ImagePoint.X.ToString("000000.00"); 
        }
        public string ToXString()
        {
            return ImagePoint.X.ToString("000000.00") + "," + ImagePoint.Y.ToString("000000.00");
        }

        protected override void FromString(string Str)
        {
            string[] strs = Str.Split(Separator);

            GroupIndex = int.Parse(strs[0]);
            Index = int.Parse(strs[1]);
            IsCalibration = (strs[2] == "1");
            ImagePoint = JzTools.StringToPointF(strs[3]);
            RealPoint = JzTools.StringToPointF(strs[4]);
            Contrast = int.Parse(strs[5]);
            Cell = new CellClass(strs[6]);
            FromOtherString(strs[7]);

        }
        public override string ToString()
        {
            string Str = "";

            Str += GroupIndex.ToString() + Separator;
            Str += Index.ToString() + Separator;
            Str += (IsCalibration ? "1" : "0") + Separator;
            Str += JzTools.PointFToString(ImagePoint) + Separator;
            Str += JzTools.PointFToString(RealPoint) + Separator;
            Str += Contrast.ToString() + Separator;
            Str += Cell.ToString() + Separator;
            Str += ToOtherString();

            //if (BorderList.Count > 1)
            //{
            //    Str = Str;
            //}


            return Str;
        }
        
        protected override void FromOtherString(string Str)
        {
            string[] strs = Str.Split(SubSeparator);

            

            if (strs.Length > 1)
            {
                IsDatum = strs[1] == "1";
            }
            if (strs.Length > 2)
            {
                IsDummy = strs[2] == "1";
            }
        }
        protected override string ToOtherString()
        {
            string Str = "";

            

            Str = JzTools.RemoveLastChar(Str, 1);

            Str += SubSeparator;

            //if(IsCalibration)
            //    Str += "0" + SubSeparator;
            //else
            //    Str += (IsDatum ? "1" : "0") + SubSeparator;

            Str += (IsDatum ? "1" : "0") + SubSeparator;
            Str += (IsDummy ? "1" : "0") + SubSeparator;
            
            Str += "";

            return Str;
        }
        
        public override void Suicide()
        {
            //orderList.Clear();

            bmpORIGIN.Dispose();
            bmpOPERATE.Dispose();
            bmpSized.Dispose();
            bmpTmp.Dispose();
            
        }
       
    }

    public class OPClass
    {
        protected const char LstSeparator = '\x1F';
        protected const char Separator = '\x1E';
        protected const char SubSeparator = '\x1D';

        protected const char CR = '\x0D';
        protected const char LF = '\x0A';

        public int GroupIndex = 0;
        public int Index = 0;
        protected OPTypeEnum OPType = OPTypeEnum.BAS;

        public CellClass Cell = new CellClass();

        public string IndexNoStr
        {
            get
            {
                return JzTools.ValueToHEX(GroupIndex, 2) + Index.ToString("000");
            }
        }
        public string IndexName
        {
            get
            {
                return OPType.ToString() + "-" + IndexNoStr;
            }
        }

        protected virtual void FromString(string Str)
        {


        }
        protected virtual string ToOtherString()
        {
            string Str = "";

            return Str;
        }
        protected virtual void FromOtherString(string Str)
        {


        }

        public virtual void Suicide()
        {

        }
    }
}
