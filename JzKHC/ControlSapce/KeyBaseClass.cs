using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

//using Jumbo301.UniversalSpace;
//using Jumbo301.DBSpace;
using JetEazy.BasicSpace;
using JzKHC.AOISpace;
using JetEazy;

namespace JzKHC.ControlSpace
{
    class CornerDefineClass
    {
        const char Separator = '\xad';
        public string Name = "";
        public string AliasName = "";
        public CornerExEnum IndicateCornerEx = CornerExEnum.NONE;
        protected JzToolsClass JzTools = new JzToolsClass();

        public CornerDefineClass(string InitialString)
        {
            string[] Str = InitialString.Split(Separator);

            Name = Str[0];
            AliasName = Str[1];
            IndicateCornerEx = JzTools.StrToCornerEx(Str[2]);
        }
        public override string ToString()
        {
            return Name + Separator + AliasName + Separator + JzTools.CornerEXToStr(IndicateCornerEx);
            
        }
        public string ToFormedString()
        {
            return AliasName.PadRight(20) + "POS:" + JzTools.CornerEXToStr(IndicateCornerEx);
        }
        public string ToDrawTextString()
        {
            return AliasName + "," + JzTools.CornerEXToStr(IndicateCornerEx);
        }
    }

    class KeybaseClass : FrameClass
    {
        const char Separator = '\xae';
        const char SeparatorInside = '\xaf';
        const int FindingRange = 500;
        const int FindMinWidth = 10;
        const int FindMinHeigth = 5;

        const int SizedRatio = -2;          //IN OPERATION SIZE RATIO

        protected JzToolsClass JzTools = new JzToolsClass();

        SideEnum mySide = SideEnum.SIDE0;

        KeyboardClass KEYBOARD
        {
            get
            {
                return Universal.KEYBOARD;
            }
        }
        SideClass SIDE
        {
            get
            {
                return Universal.KEYBOARD.SIDES[(int)mySide];
            }

        }

        //ResultClass RESULT
        //{
        //    get
        //    {
        //        return Universal.RESULT;
        //    }
        //}

        public string Name = "";
        public Rectangle myrect = new Rectangle();
        public Rectangle myrectbak = new Rectangle();

        public bool IsAsPlane = false;
        public bool IsAutoLocation = false;
        public bool IsSpaceFlat = false;

        public double XPos = 0;
        public double YPos = 0;

        public bool IsCalibration = false;
        public bool IsFromBase = false;

        public int Contrast = 5;
        public int FlatIndex = 0;
        public double AddHeight = 0;

        public int Ymin = Universal.FindMinYPosition;
        public int Range = Universal.FindRegion;
        public int ResolutionRange = Universal.AnalyzeDistance;

        public double Resolution = 0.01;

        public Pen myPen = new Pen(Color.Green, 2);
        public Brush myBrush = Brushes.Lime;

        public Rectangle rectFound = new Rectangle();
        public Point FoundCenter
        {
            get
            {
                return JzTools.GetRectCenter(rectFound);
            }
        }
        public Point FoundCenterBias
        {
            get
            {
                return new Point(FoundCenter.X + myrect.X, FoundCenter.Y + myrect.Y);
            }
        }
        public Rectangle rectFoundBias
        {
            get
            {
                Rectangle retRect = rectFound;

                retRect.X += myrect.X;
                retRect.Y += myrect.Y;

                return retRect;
            }
        }

        public RectangleF rectFoundF = new RectangleF();
        public PointF FoundCenterF
        {
            get
            {
                return JzTools.GetRectCenterF(rectFoundF);
            }
        }
        public PointF FoundCenterBiasF
        {
            get
            {
                return new PointF(FoundCenterF.X + (float)myrect.X, FoundCenterF.Y + (float)myrect.Y);
            }
        }
        
        public Rectangle rectAnalyzeFound = new Rectangle();
        public Point FoundAnalyzeCenter = new Point();
        public PointF FoundAnalyzeCenterF = new PointF();

        //Calculation Result
        public Rectangle rectCheckFound = new Rectangle();
        public RectangleF rectCheckFoundF = new RectangleF();
        public Point CheckedCenter = new Point();
        public PointF CheckedCenterF = new PointF();
        public Point LastCheckedCenter = new Point();

        public Rectangle rectCheckFoundBias
        {
            get
            {
                Point Pt = JzTools.GetRectCenter(rectCheckFound);

                Pt.X = CheckedCenter.X - Pt.X;
                Pt.Y = CheckedCenter.Y - Pt.Y;

                return new Rectangle(rectCheckFound.X + Pt.X, rectCheckFound.Y + Pt.Y, rectCheckFound.Width, rectCheckFound.Height);
            }
        }

        public double CheckedkHeight
        {
            get
            {
                //return ((double)(CheckedCenter.Y - FoundCenterBias.Y)) * Resolution;

                if (INI.IS_CHECK_LEVEL)
                    return ((double)(FoundCenterBiasF.X - CheckedCenterF.X)) * Resolution + AddHeight;
                else
                    return ((double)(CheckedCenterF.Y - FoundCenterBiasF.Y)) * Resolution + AddHeight;

               // return ((double)(CheckedCenterF.Y - FoundCenterBiasF.Y)) * Resolution + AddHeight;
            }
        }

        public double TmpHeight = 0;


        public double PlaneHeight = 0;

        //Control Parameters

        public bool IsSelected = false;
        public bool IsSelectedStart = false;

        //Threshold Parameter
        public int MinGrade = 0;
        public int MeaneGrade = 0;
        public int ModeGrade = 0;
        public int MaxGrade = 0;

        public List<CornerDefineClass> CornerDefinedList = new List<CornerDefineClass>();

        //Bitmaps
        public Bitmap bmpOrigion = new Bitmap(1, 1);
        public Bitmap bmpOrigionSized = new Bitmap(1, 1);
        public Bitmap bmpThreshed = new Bitmap(1, 1);
        public Bitmap bmpProcessed = new Bitmap(1, 1);

        //Result Bitmaps
        public Bitmap bmpOrigionComp = new Bitmap(1, 1);
        public Bitmap bmpThreshedComp = new Bitmap(1, 1);
        public Bitmap bmpProcessedComp = new Bitmap(1, 1);

        public double ContrastRatio
        {
            get
            {
                return (double)Contrast / 20d;
            }
        }

        public KeybaseClass()
        {


        }
        public KeybaseClass(SideEnum rSide, List<KeybaseClass> BASELIST,Bitmap bmp)
        {
            mySide = rSide;

            if (BASELIST.Count == 0)
            {
                Name = "BASE-" + ((int)rSide).ToString("00") + "000";

                myrect = new Rectangle(0, 0, 100, 100);

                IsSelected = true;
                IsSelectedStart = true;
            }
            else
            {
                KeybaseClass Lastkeybase = BASELIST[BASELIST.Count - 1];

                foreach (KeybaseClass keybase in BASELIST)
                {
                    keybase.IsSelectedStart = false;
                    keybase.IsSelected = false;
                }

                Name = "BASE-" + ((int)rSide).ToString("00")
                    + (int.Parse(JzTools.GetLastString(Lastkeybase.Name, 3)) + 1).ToString("000");

                myrect = Lastkeybase.myrect;

                myrect.X = Math.Min(myrect.X + myrect.Width + 20, INI.CCDWIDTH - myrect.Width - 1);

                IsSelected = true;
                IsSelectedStart = true;

                Contrast = Lastkeybase.Contrast;
                IsFromBase = Lastkeybase.IsFromBase;

                Resolution = Lastkeybase.Resolution;
            }

            GetBMP(bmp);

            AssignControls();
        }
        public KeybaseClass(SideEnum rSide, List<KeybaseClass> BASELIST, Bitmap bmp,Rectangle rect)
        {
            mySide = rSide;

            if (BASELIST.Count == 0)
            {
                Name = "BASE-" + ((int)rSide).ToString("00") + "000";

                myrect = rect;

                IsSelected = true;
                IsSelectedStart = true;
            }
            else
            {
                KeybaseClass Lastkeybase = BASELIST[BASELIST.Count - 1];

                foreach (KeybaseClass keybase in BASELIST)
                {
                    keybase.IsSelectedStart = false;
                    keybase.IsSelected = false;
                }

                Name = "BASE-" + ((int)rSide).ToString("00")
                    + (int.Parse(JzTools.GetLastString(Lastkeybase.Name, 3)) + 1).ToString("000");

                myrect = rect;

                IsSelected = true;
                IsSelectedStart = true;

                Contrast = Lastkeybase.Contrast;
                IsFromBase = Lastkeybase.IsFromBase;

                Resolution = Lastkeybase.Resolution;
            }

            GetBMP(bmp);

            AssignControls();
        }
        public KeybaseClass(SideEnum rSide, string rStr)
        {
            string[] Str = rStr.Split(Separator);
            string[] Str1;
            int i = 0;

            mySide = rSide;

            Name = Str[0];
            myrect = JzTools.StringtoRect(Str[1]);
            Contrast = int.Parse(Str[2]);
            Resolution = double.Parse(Str[3]);
            IsCalibration = Str[4] == "1";
            IsFromBase = Str[5] == "1";

            Str1 = Str[6].Split(SeparatorInside);
            
            if (Str1[0] != "")
            {
                i = 0;
                while (i < Str1.Length)
                {
                    CornerDefinedList.Add(new CornerDefineClass(Str1[i]));
                    i++;
                }
            }

            if (Str.Length > 7)
            {
                FoundAnalyzeCenter = StringtoPoint(Str[7]);
            }
            if (Str.Length > 8)
            {
                IsAsPlane = Str[8] == "1";
                XPos = double.Parse(Str[9]);
                YPos = double.Parse(Str[10]);
            }
            if (Str.Length > 11)
            {
                IsAutoLocation = Str[11] == "1";
            }
            if (Str.Length > 12)
            {
                IsSpaceFlat = Str[12] == "1";
            }
            if (Str.Length > 13)
            {
                FlatIndex = int.Parse(Str[13]);
            }
            if (Str.Length > 14)
            {
                AddHeight = double.Parse(Str[14]);
            }

            if (Str.Length > 15)
            {
                Ymin = int.Parse(Str[15]);
                Range = int.Parse(Str[16]);
                ResolutionRange = int.Parse(Str[17]);
            }

            GetBMP();
            AssignControls();
        }

        public KeybaseClass Clone()
        {
            KeybaseClass keybase = new KeybaseClass();

            keybase.mySide = mySide;
            keybase.Name = Name;
            keybase.myrect = myrect;

            keybase.Contrast = Contrast;
            keybase.Resolution = Resolution;
            keybase.IsFromBase = IsFromBase;
            keybase.IsCalibration = IsCalibration;

            keybase.rectFound = rectFound;
            keybase.rectFoundF = rectFoundF;
            keybase.FoundAnalyzeCenter = FoundAnalyzeCenter;
            
            keybase.IsSelected = false;
            keybase.IsSelectedStart = false;

            //Bitmaps
            keybase.bmpOrigion.Dispose();
            keybase.bmpOrigion = (Bitmap)bmpOrigion.Clone();

            keybase.bmpOrigionSized.Dispose();
            keybase.bmpOrigionSized = (Bitmap)bmpOrigionSized.Clone();

            keybase.bmpThreshed.Dispose();
            keybase.bmpThreshed = (Bitmap)bmpThreshed.Clone();
            keybase.bmpProcessed.Dispose();
            keybase.bmpProcessed = (Bitmap)bmpProcessed.Clone();

            //Result Bitmaps
            keybase.bmpOrigionComp.Dispose();
            keybase.bmpOrigionComp = (Bitmap)bmpOrigionComp.Clone();
            keybase.bmpThreshedComp.Dispose();
            keybase.bmpThreshedComp = (Bitmap)bmpThreshedComp.Clone();
            keybase.bmpProcessedComp.Dispose();
            keybase.bmpProcessedComp = (Bitmap)bmpProcessedComp.Clone();

            //Threshold Parameter

            keybase.MinGrade = MinGrade;
            keybase.MeaneGrade = MeaneGrade;
            keybase.ModeGrade = ModeGrade;
            keybase.MaxGrade = MaxGrade;

            keybase.IsAsPlane = IsAsPlane;
            keybase.IsAutoLocation = IsAutoLocation;
            keybase.IsSpaceFlat = IsSpaceFlat;
            keybase.FlatIndex = FlatIndex;
            keybase.AddHeight = AddHeight;
            keybase.XPos = XPos;
            keybase.YPos = YPos;

            keybase.Ymin = Ymin;
            keybase.Range = Range;
            keybase.ResolutionRange = ResolutionRange;

            foreach (CornerDefineClass cornerdefined in CornerDefinedList)
            {
                keybase.CornerDefinedList.Add(new CornerDefineClass(cornerdefined.ToString()));
            }

            keybase.AssignControls();

            return keybase;
        }
        public KeybaseClass CloneAdded(int LastIndex,Bitmap bmp)
        {
            return CloneAdded(LastIndex, false, bmp);
        }
        public KeybaseClass CloneAdded(int LastIndex, bool IsCopy,Bitmap bmp)
        {
            KeybaseClass keybase = new KeybaseClass();

            keybase.mySide = mySide;
            keybase.Name = "BASE-" + ((int)mySide).ToString("00") + (LastIndex + 1).ToString("000");
            keybase.myrect = myrect;

            if (!IsCopy)
                keybase.myrect.X = Math.Min(myrect.X + myrect.Width + 20, INI.CCDWIDTH - myrect.Width - 1);
            else
                keybase.myrect.Y = Math.Min(myrect.Y + 10, INI.CCDHEIGHT - myrect.Height - 1);

            keybase.Contrast = Contrast;
            keybase.Resolution = Resolution;

            keybase.IsSelected = IsSelected;
            keybase.IsSelectedStart = IsSelectedStart;

            keybase.GetBMP(bmp);

            keybase.AssignControls();

            return keybase;
        }
        
        public void Initial()
        {
            IsSelected = false;
            IsSelectedStart = false;
        }

        public bool IsInside(Point Pt)
        {
            bool ret = myrect.IntersectsWith(JzTools.SimpleRect(Pt));

            return ret;
        }
        public CornerEnum IsInsideCorner(Point Pt)
        {
            int i = 0;
            CornerEnum retCorner = CornerEnum.NONE;

            while (i < (int)CornerEnum.COUNT)
            {
                if (JzTools.CornerRect(myrect, (CornerEnum)i, CornerSize << 1).IntersectsWith(JzTools.SimpleRect(Pt,CornerSize)))
                {
                    retCorner = (CornerEnum) i;
                    break;
                }

                i++;
            }

            return retCorner;
        }

        public void BackupRect()
        {
            myrectbak = myrect;
        }
        public void RestoreRect()
        {
            myrect = myrectbak;
        }

        public void MoveRect(int X, int Y)
        {
            RestoreRect();

            myrect.Offset(X, Y);
            //myrect = BonudRect(myrect, SIDE.bmpBaseOrigin.Size);//Gaara by mask
        }
        public void SizedRect(int Width,int Height,CornerEnum CatchCorner)
        {
            RestoreRect();

            switch (CatchCorner)
            {
                case CornerEnum.RB:
                    myrect.Width = Math.Max(10, myrect.Width + Width);
                    myrect.Height = Math.Max(10, myrect.Height + Height);
                    break;
                case CornerEnum.LT:
                    myrect.X = Math.Min(myrect.X + Width, myrect.X + myrect.Width - 10);
                    myrect.Y = Math.Min(myrect.Y + Height, myrect.Y + myrect.Height - 10);

                    myrect.Width = Math.Max(myrect.Width - Width, 10);
                    myrect.Height = Math.Max(myrect.Height - Height, 10);
                    break;
                case CornerEnum.RT:
                    myrect.Y = Math.Min(myrect.Y + Height, myrect.Y + myrect.Height - 10);

                    myrect.Width = Math.Max(myrect.Width + Width, 10);
                    myrect.Height = Math.Max(10, myrect.Height - Height);
                    break;
                case CornerEnum.LB:
                    myrect.X = Math.Min(myrect.X + Width, myrect.X + myrect.Width - 10);

                    myrect.Width = Math.Max(10, myrect.Width - Width);
                    myrect.Height = Math.Max(myrect.Height + Height, 10);
                    break;
            }

            //myrect = BonudRect(myrect, SIDE.bmpBaseOrigin.Size);//Gaara by mask

        }

        //取得原有圖像
        const double MagicNumber = 5d / 8d;
        const double MagicNumber2 = 3d / 4d;

        public void GetBMP()
        {
            //GetBMP(SIDE.bmpBaseOrigin);//Gaara by mask
        }
        public void GetBMP(Bitmap bmp)
        {
            GetBMP(bmp, ContrastRatio);
        }
        public void GetBMP(Bitmap bmp, double Ratio)
        {
            bmpOrigion.Dispose();
            bmpOrigion = (Bitmap)bmp.Clone(myrect, PixelFormat.Format32bppArgb);

            bmpOrigionSized.Dispose();
            bmpOrigionSized = new Bitmap(bmpOrigion, JzTools.Resize(bmpOrigion.Size, SizedRatio));

            Histogram.GetHistogram(bmpOrigionSized);

            MinGrade = Histogram.MinGrade;
            MeaneGrade = Histogram.MeanGrade;
            ModeGrade = Histogram.ModeGrade;
            MaxGrade = Histogram.MaxGrade;

            bmpThreshed.Dispose();
            bmpThreshed = (Bitmap)bmpOrigion.Clone();

            Threshold.SetThreshold(bmpThreshed, MinGrade, (int)((double)(MaxGrade - MinGrade) * Ratio), MinGrade);

            //Get Point Center
            //myCenter = AnalyzeCenter(bmpThreshed, MinGrade, MeaneGrade, Ratio);
            rectFound = AnalyzeRect(bmpThreshed, MinGrade, MeaneGrade, Ratio);

            rectFoundF = retRectF;
            //myCenter.X += rectBase.X;
            //myCenter.Y += rectBase.Y;

            bmpProcessed.Dispose();
            bmpProcessed = (Bitmap)bmpThreshed.Clone();

            JzTools.DrawRect(bmpProcessed, rectFound, new Pen(Color.Red, 2));
            JzTools.DrawRect(bmpProcessed, JzTools.SimpleRect(new Point(FoundCenter.X - 1, FoundCenter.Y - 1), 2, 2), new SolidBrush(Color.Red));

            //bmpProcessed.Save(@"D:\LOA\TESTRESULT\004.BMP", ImageFormat.Bmp);
        }
        public void CheckResolutionAllinone(Bitmap bmp)
        {
            int XInflateVaue = 20;
            int GetHeight = FindingRange;
            Rectangle rectToFind = myrect;
            RectangleF rectFTmp = new RectangleF();

            rectToFind.Inflate(0, XInflateVaue);
            rectToFind.Width += GetHeight;
            //rectToFind.Location = new Point(myrect.X - GetHeight, myrect.Y);
            rectToFind.X += Ymin;

            rectToFind.Intersect(JzTools.SimpleRect(bmp.Size));

            //Added For Light Noise
            //Bitmap bmpNOISE = (Bitmap)bmp.Clone();
            //JzTools.DrawRect(bmpNOISE, new Rectangle(0, 2400, INI.CCDWIDTH, INI.CCDHEIGHT), new SolidBrush(Color.FromArgb(MinGrade, MinGrade, MinGrade)));

            //bmpNOISE.Save(@"D:\LOA\NOISE.BMP", ImageFormat.Bmp);

            Bitmap bmpTmp = (Bitmap)bmp.Clone(rectToFind, PixelFormat.Format32bppArgb);

            //Bitmap bmpfind = new Bitmap(bmp);
            //JzTools.DrawRect(bmpfind, rectToFind, new Pen(Color.Lime, 3));
            //bmpfind.Save(@"D:\TESTTEST\" + Name + ".bmp", ImageFormat.Bmp);

            if (INI.CUTPOINT > 0)
            {
                if (rectToFind.Y + GetHeight > INI.CUTPOINT)
                {
                    JzTools.DrawRect(bmpTmp, new Rectangle(0, INI.CUTPOINT - rectToFind.Y, INI.CCDWIDTH, INI.CCDHEIGHT), new SolidBrush(Color.FromArgb(MinGrade, MinGrade, MinGrade)));
                }
            }

            Bitmap bmpTmp1 = (Bitmap)bmpTmp.Clone();

            //JzTools.DrawRect(bmpTmp, new Rectangle(0, 2500, INI.CCDWIDTH, 100), new SolidBrush(Color.Black));
            //JzTools.DrawRect(bmpTmp1, new Rectangle(0, 2500, INI.CCDWIDTH, 100), new SolidBrush(Color.Black));

            Threshold.SetThreshold(bmpTmp, MinGrade, (int)((double)(MaxGrade - MinGrade) * ContrastRatio), MinGrade);
            //bmpTmp.Save(@"D:\TESTTEST\1-" + Name + ".bmp", ImageFormat.Bmp);

            AnalyzeXRange = XInflateVaue * 3;

            //AnalyzeYMinRange = 120;
            //if (myrect.Y < 500)
            //    AnalyzeYMinRange = 120;
            //else
            //    AnalyzeYMinRange = 120;
            AnalyzeYMinRange = ResolutionRange;
            AnalyzeYMaxRange = FindingRange;

            rectAnalyzeFound = AnalyzeRect(bmpTmp, MinGrade, MeaneGrade, ContrastRatio, true, rectToFind, bmpTmp1);
            rectFTmp = retRectF;


            FoundAnalyzeCenter = JzTools.GetRectCenter(rectAnalyzeFound);

            FoundAnalyzeCenter.X += rectToFind.X;
            FoundAnalyzeCenter.Y += rectToFind.Y;

            FoundAnalyzeCenterF = JzTools.GetRectCenterF(rectFTmp);

            FoundAnalyzeCenterF.X += rectToFind.X;
            FoundAnalyzeCenterF.Y += rectToFind.Y;


            if (FoundCenterBias.X - FoundAnalyzeCenter.X <= 0)
            {
                Resolution = 0;
            }
            else
            {
                Resolution = INI.DIFFHEIGHT / (double)(FoundCenterBiasF.X - FoundAnalyzeCenterF.X);
            }

            //bmpNOISE.Dispose();
            bmpTmp.Dispose();
            bmpTmp1.Dispose();
        }
        public void CheckResolution(Bitmap bmp)
        {
            int XInflateVaue = 10;
            int GetHeight = FindingRange;
            Rectangle rectToFind = myrect;
            RectangleF rectFTmp = new RectangleF();

            rectToFind.Inflate(XInflateVaue, 0);
            rectToFind.Height += GetHeight;

            rectToFind.Intersect(JzTools.SimpleRect(bmp.Size));

            //Added For Light Noise
            //Bitmap bmpNOISE = (Bitmap)bmp.Clone();
            //JzTools.DrawRect(bmpNOISE, new Rectangle(0, 2400, INI.CCDWIDTH, INI.CCDHEIGHT), new SolidBrush(Color.FromArgb(MinGrade, MinGrade, MinGrade)));

            //bmpNOISE.Save(@"D:\LOA\NOISE.BMP", ImageFormat.Bmp);

            Bitmap bmpTmp = (Bitmap)bmp.Clone(rectToFind, PixelFormat.Format32bppArgb);

            if (INI.CUTPOINT > 0)
            {
                if (rectToFind.Y + GetHeight > INI.CUTPOINT)
                {
                    JzTools.DrawRect(bmpTmp, new Rectangle(0, INI.CUTPOINT - rectToFind.Y, INI.CCDWIDTH, INI.CCDHEIGHT), new SolidBrush(Color.FromArgb(MinGrade, MinGrade, MinGrade)));
                }
            }

            Bitmap bmpTmp1 = (Bitmap)bmpTmp.Clone();

            //JzTools.DrawRect(bmpTmp, new Rectangle(0, 2500, INI.CCDWIDTH, 100), new SolidBrush(Color.Black));
            //JzTools.DrawRect(bmpTmp1, new Rectangle(0, 2500, INI.CCDWIDTH, 100), new SolidBrush(Color.Black));

            Threshold.SetThreshold(bmpTmp, MinGrade, (int)((double)(MaxGrade - MinGrade) * ContrastRatio), MinGrade);

            AnalyzeXRange = XInflateVaue * 3;
            
            //AnalyzeYMinRange = 120;
            //if (myrect.Y < 500)
            //    AnalyzeYMinRange = 120;
            //else
            //    AnalyzeYMinRange = 120;
            AnalyzeYMinRange = ResolutionRange;
            AnalyzeYMaxRange = FindingRange;

            rectAnalyzeFound = AnalyzeRect(bmpTmp, MinGrade, MeaneGrade, ContrastRatio, true, rectToFind, bmpTmp1);
            rectFTmp = retRectF;


            FoundAnalyzeCenter = JzTools.GetRectCenter(rectAnalyzeFound);

            FoundAnalyzeCenter.X += rectToFind.X;
            FoundAnalyzeCenter.Y += rectToFind.Y;

            FoundAnalyzeCenterF = JzTools.GetRectCenterF(rectFTmp);

            FoundAnalyzeCenterF.X += rectToFind.X;
            FoundAnalyzeCenterF.Y += rectToFind.Y;


            if (FoundAnalyzeCenter.Y - FoundCenterBias.Y <= 0)
            {
                Resolution = 0;
            }
            else
            {
                Resolution = INI.DIFFHEIGHT / (double)(FoundAnalyzeCenterF.Y - FoundCenterBiasF.Y);
            }

            //bmpNOISE.Dispose();
            bmpTmp.Dispose();
            bmpTmp1.Dispose();
        }
        public bool CheckCalibrationIsOK(Bitmap bmp)
        {
            Rectangle rectToFind = myrect;
            Rectangle rectMyFound = new Rectangle();

            Point PtMyFound = new Point();

            rectToFind.Intersect(JzTools.SimpleRect(bmp.Size));

            Bitmap bmpTmp = (Bitmap)bmp.Clone(rectToFind, PixelFormat.Format32bppArgb);
            Bitmap bmpTmp1 = (Bitmap)bmpTmp.Clone();

            Threshold.SetThreshold(bmpTmp, MinGrade, (int)((double)(MaxGrade - MinGrade) * ContrastRatio), MinGrade);

            //bmpTmp.Save(@"D:\LOA\NEWERA\N01.BMP", ImageFormat.Bmp);
            AnalyzeXRange = 20;
            AnalyzeYMinRange = -10;
            AnalyzeYMaxRange = 10;

            rectMyFound = AnalyzeRect(bmpTmp, MinGrade, MeaneGrade, ContrastRatio, true, rectToFind, bmpTmp1);

            PtMyFound = JzTools.GetRectCenter(rectMyFound);

            PtMyFound.X += rectToFind.X;
            PtMyFound.Y += rectToFind.Y;

            bmpTmp.Dispose();
            bmpTmp1.Dispose();

            return Math.Abs(PtMyFound.Y - FoundCenterBias.Y) < INI.BIASERROR;
        }

        public void ClearVariables()
        {
            CheckedCenter = new Point(0, 0);
            PlaneHeight = 0;
        }
        public void CheckBMP(Bitmap bmp)
        {
            int XInflateVaue = 10;
            Rectangle rectToFind = myrect;
            int GetHeight = FindingRange;

            if (INI.IS_CHECK_LEVEL)
            {
                XInflateVaue = 20;
                rectToFind.Inflate(0, XInflateVaue);
                rectToFind.Width += Range;
                rectToFind.X += Ymin;
            }
            else
            {
                //if (!IsFromBase)
                {
                    rectToFind.Inflate(XInflateVaue, 0);
                    //Added For PEM Height Check 2011/03/30
                    //rectToFind.Y -= (XInflateVaue << 2);
                    rectToFind.Y += Ymin;
                    rectToFind.Height += Range;
                }
                //else
                //{
                //    rectToFind.Inflate(XInflateVaue, 0);

                //    rectToFind.Y -= (XInflateVaue << 2);
                //    rectToFind.Height += FindingRange;
                //}
            }
            ////if (!IsFromBase)
            //{
            //    rectToFind.Inflate(XInflateVaue, 0);
            //    //Added For PEM Height Check 2011/03/30
            //    //rectToFind.Y -= (XInflateVaue << 2);
            //    rectToFind.Y += Ymin;
            //    rectToFind.Height += Range;
            //}
            ////else
            ////{
            ////    rectToFind.Inflate(XInflateVaue, 0);

            ////    rectToFind.Y -= (XInflateVaue << 2);
            ////    rectToFind.Height += FindingRange;
            ////}

            rectToFind.Intersect(JzTools.SimpleRect(bmp.Size));


            if (Name == "BASE-00021")
            {
                //JzTools.DrawRect(bmp, rectToFind, new Pen(Color.Red, 2));
                //bmp.Save(@"D:\LOA\NEWERA\CK01.BMP", ImageFormat.Bmp);
            }

            //JzTools.DrawRect(bmp, rectToFind, new Pen(Color.Red, 2));
            //bmp.Save(@"D:\LOA\NEWERA\CK01.BMP", ImageFormat.Bmp);
            //Bitmap bmpNOISE = (Bitmap)bmp.Clone();
            //JzTools.DrawRect(bmpNOISE, new Rectangle(0, 2400, INI.CCDWIDTH, INI.CCDHEIGHT), new SolidBrush(Color.FromArgb(MinGrade, MinGrade, MinGrade)));

            //bmpNOISE.Save(@"D:\LOA\NOISE.BMP", ImageFormat.Bmp);

            //Bitmap bmpTmp = (Bitmap)bmpNOISE.Clone(rectToFind, PixelFormat.Format32bppArgb);

            Bitmap bmpTmp = (Bitmap)bmp.Clone(rectToFind, PixelFormat.Format32bppArgb);

            if (INI.CUTPOINT > 0)
            {
                if (rectToFind.Y + GetHeight > INI.CUTPOINT)
                {
                    JzTools.DrawRect(bmpTmp, new Rectangle(0, INI.CUTPOINT - rectToFind.Y, INI.CCDWIDTH, INI.CCDHEIGHT), new SolidBrush(Color.FromArgb(MinGrade, MinGrade, MinGrade)));
                }
            }
            Bitmap bmpTmp1 = (Bitmap)bmpTmp.Clone();

            Threshold.SetThreshold(bmpTmp, MinGrade, (int)((double)(MaxGrade - MinGrade) * ContrastRatio), MinGrade);

            //bmpTmp.Save(@"D:\LOA\NEWERA\N01.BMP", ImageFormat.Bmp);
            if (Name == "BASE-00021")
            {
                //bmpTmp.Save(@"D:\LOA\NEWERA\N01" + Name + ".BMP", ImageFormat.Bmp);
            }

            if (!IsFromBase)
            {
                if (Resolution < 0.018)
                    AnalyzeXRange = XInflateVaue * 5;
                else
                    AnalyzeXRange = (int)((double)XInflateVaue * 5);
            }
            else
            {
                AnalyzeXRange = XInflateVaue * 5;
            }


            //if (!IsFromBase)
            {
                AnalyzeYMinRange = Ymin;
                AnalyzeYMaxRange = Range;
            }
            //else
            //{
            //    AnalyzeYMinRange = Universal.FindMinYPosition;
            //    AnalyzeYMaxRange = 200 + (IsOTrackpad ? 70 : 0);
            //}

            if (Name == "BASE-05093")
            {
                //bmpTmp.Save(@"D:\LOA\NEWERA\N01" + Name + ".BMP", ImageFormat.Bmp);
                Name = Name;
            }


            if (!Universal.IsFindingBackward)
                rectCheckFound = AnalyzeRect(bmpTmp, MinGrade, MeaneGrade, ContrastRatio, true, rectToFind, bmpTmp1);
            else
                rectCheckFound = AnalyzeRectEX(bmpTmp, MinGrade, MeaneGrade, ContrastRatio, true, rectToFind, bmpTmp1);

            rectCheckFoundF = retRectF;


            if (rectCheckFound.X == 0 && rectCheckFound.Y == 0)
            {
                rectCheckFound = rectCheckFound;
            }

            if (rectCheckFound.Width == 0 || rectCheckFound.Height == 0)
            {
                CheckedCenter.X = -100;
                CheckedCenter.Y = -100;

                CheckedCenterF.X = -100;
                CheckedCenterF.Y = -100;
            }
            else
            {
                CheckedCenter = JzTools.GetRectCenter(rectCheckFound);
                CheckedCenter.X += rectToFind.X;
                CheckedCenter.Y += rectToFind.Y;


                CheckedCenterF = JzTools.GetRectCenterF(rectCheckFoundF);
                CheckedCenterF.X += rectToFind.X;
                CheckedCenterF.Y += rectToFind.Y;
            }

            ////Tuning For Ambiguous Height Added By Victor 2009/11/07
            //if (!IsFromBase & !IsCalibration)
            //{
            //    if ((CheckedCenter.Y - FoundCenterBias.Y) < 0)
            //    {
            //        AnalyzeXRange = XInflateVaue << 1;
            //        AnalyzeYMinRange = 60;
            //        AnalyzeYMaxRange = 500;

            //        rectCheckFound = AnalyzeRect(bmpTmp, MinGrade, MeaneGrade, ContrastRatio, true, rectToFind, bmpTmp1);
            //        rectCheckFoundF = retRectF;

            //        if (rectCheckFound.Width == 0 || rectCheckFound.Height == 0)
            //        {
            //            CheckedCenter.X = -100;
            //            CheckedCenter.Y = -100;

            //            CheckedCenterF.X = -100;
            //            CheckedCenterF.Y = -100;
            //        }
            //        else
            //        {
            //            CheckedCenter = JzTools.GetRectCenter(rectCheckFound);
            //            CheckedCenter.X += rectToFind.X;
            //            CheckedCenter.Y += rectToFind.Y;

            //            CheckedCenterF = JzTools.GetRectCenterF(rectCheckFoundF);
            //            CheckedCenterF.X += rectToFind.X;
            //            CheckedCenterF.Y += rectToFind.Y;
            //        }
            //    }
            //    if ((CheckedCenter.Y - FoundCenterBias.Y) * Resolution > 2.8)
            //    {
            //        AnalyzeXRange = XInflateVaue << 1;
            //        AnalyzeYMinRange = 50;
            //        AnalyzeYMaxRange = 300;

            //        rectCheckFound = AnalyzeRect(bmpTmp, MinGrade, MeaneGrade, ContrastRatio, true, rectToFind, bmpTmp1);
            //        rectCheckFoundF = retRectF;

            //        if (rectCheckFound.Width == 0 || rectCheckFound.Height == 0)
            //        {
            //            CheckedCenter.X = -100;
            //            CheckedCenter.Y = -100;
            //        }
            //        else
            //        {
            //            CheckedCenter = JzTools.GetRectCenter(rectCheckFound);
            //            CheckedCenter.X += rectToFind.X;
            //            CheckedCenter.Y += rectToFind.Y;

            //            CheckedCenterF = JzTools.GetRectCenterF(rectCheckFoundF);
            //            CheckedCenterF.X += rectToFind.X;
            //            CheckedCenterF.Y += rectToFind.Y;
            //        }
            //    }
            //    if ((double)(CheckedCenter.Y - FoundCenterBias.Y) * Resolution < 1.0)
            //    {
            //        Point OldCheckedCenter = CheckedCenter;
            //        PointF OldCheckedCenterF = CheckedCenterF;

            //        AnalyzeXRange = 4;
            //        AnalyzeYMinRange = 30;
            //        AnalyzeYMaxRange = 500;

            //        rectCheckFound = AnalyzeRect(bmpTmp, MinGrade, MeaneGrade, ContrastRatio, true, rectToFind, bmpTmp1);
            //        rectCheckFoundF = retRectF;

            //        if (rectCheckFound.Width == 0 || rectCheckFound.Height == 0)
            //        {
            //            CheckedCenter.X = -100;
            //            CheckedCenter.Y = -100;
            //        }
            //        else
            //        {
            //            CheckedCenter = JzTools.GetRectCenter(rectCheckFound);
            //            CheckedCenter.X += rectToFind.X;
            //            CheckedCenter.Y += rectToFind.Y;

            //            CheckedCenterF = JzTools.GetRectCenterF(rectCheckFoundF);
            //            CheckedCenterF.X += rectToFind.X;
            //            CheckedCenterF.Y += rectToFind.Y;
            //        }

            //        if ((double)(CheckedCenter.Y - FoundCenterBias.Y) * Resolution < 0 || (double)(CheckedCenter.Y - FoundCenterBias.Y) * Resolution > 2.8 + INI.BASEHEIGHT)
            //        {
            //            CheckedCenter = OldCheckedCenter;
            //            CheckedCenterF = OldCheckedCenterF;
            //        }

            //    }
            //}


            //Modified for Stability
            //if (!IsFromBase)
            /*
            {
                int DHeight = Math.Abs(LastCheckedCenter.Y - CheckedCenter.Y);

                if (DHeight > 2)
                {
                    LastCheckedCenter = CheckedCenter;
                }
                else if (DHeight > 1)
                {
                    if (LastCheckedCenter.Y - CheckedCenter.Y > 0)
                    {
                        CheckedCenter = LastCheckedCenter;
                        CheckedCenter.Y++;
                    }
                    else
                    {
                        CheckedCenter = LastCheckedCenter;
                        CheckedCenter.Y--;
                    }
                }
                else
                {
                    CheckedCenter = LastCheckedCenter;
                }
            }
            */
            //bmpNOISE.Dispose();

            bmpTmp.Dispose();
            bmpTmp1.Dispose();
        }
        public void CheckBMPEX(Bitmap bmp)
        {
            int XInflateVaue = 10;
            Rectangle rectToFind = myrect;
            int GetHeight = FindingRange;

            if (INI.IS_CHECK_LEVEL)
            {
                XInflateVaue = 20;
                rectToFind.Inflate(0, XInflateVaue);
                rectToFind.Width += Range;
                rectToFind.X += Ymin;
            }
            else
            {
                //if (!IsFromBase)
                {
                    rectToFind.Inflate(XInflateVaue, 0);
                    //Added For PEM Height Check 2011/03/30
                    //rectToFind.Y -= (XInflateVaue << 2);
                    rectToFind.Y += Ymin;
                    rectToFind.Height += Range;
                }
                //else
                //{
                //    rectToFind.Inflate(XInflateVaue, 0);

                //    rectToFind.Y -= (XInflateVaue << 2);
                //    rectToFind.Height += FindingRange;
                //}
            }

            ////if (!IsFromBase)
            //{
            //    rectToFind.Inflate(XInflateVaue, 0);
            //    //Added For PEM Height Check 2011/03/30
            //    //rectToFind.Y -= (XInflateVaue << 2);
            //    rectToFind.Y += Ymin;
            //    rectToFind.Height += Range;
            //}
            ////else
            ////{
            ////    rectToFind.Inflate(XInflateVaue, 0);

            ////    rectToFind.Y -= (XInflateVaue << 2);
            ////    rectToFind.Height += FindingRange;
            ////}

            rectToFind.Intersect(JzTools.SimpleRect(bmp.Size));


            if (Name == "BASE-05002")
            {
                //Name = Name;
                //JzTools.DrawRect(bmp, rectToFind, new Pen(Color.Red, 2));
                //bmp.Save(@"D:\LOA\NEWERA\CK01.BMP", ImageFormat.Bmp);
            }

            //JzTools.DrawRect(bmp, rectToFind, new Pen(Color.Red, 2));
            //bmp.Save(@"D:\LOA\NEWERA\CK01.BMP", ImageFormat.Bmp);
            //Bitmap bmpNOISE = (Bitmap)bmp.Clone();
            //JzTools.DrawRect(bmpNOISE, new Rectangle(0, 2400, INI.CCD_WIDTH, INI.CCD_HEIGHT), new SolidBrush(Color.FromArgb(MinGrade, MinGrade, MinGrade)));

            //bmpNOISE.Save(@"D:\LOA\NOISE.BMP", ImageFormat.Bmp);

            //Bitmap bmpTmp = (Bitmap)bmpNOISE.Clone(rectToFind, PixelFormat.Format32bppArgb);

            Bitmap bmpTmp = (Bitmap)bmp.Clone(rectToFind, PixelFormat.Format32bppArgb);

            //if (INI.CUTPOINT > 0)
            //{
            //    if (rectToFind.Y + GetHeight > INI.CUTPOINT)
            //    {
            //        JzTools.DrawRect(bmpTmp, new Rectangle(0, INI.CUTPOINT - rectToFind.Y, INI.CCD_WIDTH, INI.CCD_HEIGHT), new SolidBrush(Color.FromArgb(MinGrade, MinGrade, MinGrade)));
            //    }
            //}
            Bitmap bmpTmp1 = (Bitmap)bmpTmp.Clone();

            Threshold.SetThreshold(bmpTmp, MinGrade, (int)((double)(MaxGrade - MinGrade) * ContrastRatio), MinGrade);

            //bmpTmp.Save(@"D:\LOA\NEWERA\N01.BMP", ImageFormat.Bmp);
            if (Name == "BASE-05002")
            {
                //bmpTmp.Save(@"D:\LOA\NEWERA\N01" + Name + ".BMP", ImageFormat.Bmp);
            }

            if (!IsFromBase)
            {
                if (Resolution < 0.018)
                    AnalyzeXRange = XInflateVaue * INI.FINDCONTRAST;
                else
                    AnalyzeXRange = (int)((double)XInflateVaue * INI.FINDCONTRAST);
            }
            else
            {
                AnalyzeXRange = XInflateVaue * INI.FINDCONTRAST;
            }

            //if (!IsFromBase)
            {
                AnalyzeYMinRange = Ymin;
                AnalyzeYMaxRange = Range;
            }
            //else
            //{
            //    AnalyzeYMinRange = Universal.FindMinYPosition;
            //    AnalyzeYMaxRange = 200 + (IsOTrackpad ? 70 : 0);
            //}

            if (Name == "BASE-05002")
            {
                //bmpTmp.Save(@"D:\LOA\NEWERA\N01" + Name + ".BMP", ImageFormat.Bmp);
                //Name = Name;
            }



            rectCheckFound = AnalyzeRectFX(bmpTmp, MinGrade, MeaneGrade, ContrastRatio, true, rectToFind, bmpTmp1);

            rectCheckFoundF = retRectF;


            if (rectCheckFound.X == 0 && rectCheckFound.Y == 0)
            {
                rectCheckFound = rectCheckFound;
            }

            if (rectCheckFound.Width == 0 || rectCheckFound.Height == 0)
            {
                CheckedCenter.X = -100;
                CheckedCenter.Y = -100;

                CheckedCenterF.X = -100;
                CheckedCenterF.Y = -100;

                //CheckedBorder.X = -100;
                //CheckedBorder.Y = -100;

                //CheckedBorderF.X = -100;
                //CheckedBorderF.Y = -100;

            }
            else
            {
                CheckedCenter = JzTools.GetRectCenter(rectCheckFound);
                CheckedCenter.X += rectToFind.X;
                CheckedCenter.Y += rectToFind.Y;


                CheckedCenterF = JzTools.GetRectCenterF(rectCheckFoundF);
                CheckedCenterF.X += rectToFind.X;
                CheckedCenterF.Y += rectToFind.Y;


                //CheckedBorder = rectCheckFound.Location;
                //CheckedBorder.X += rectToFind.X;
                //CheckedBorder.Y += rectToFind.Y;

                //CheckedBorderF = rectCheckFoundF.Location;
                //CheckedBorderF.X += rectToFind.X;
                //CheckedBorderF.Y += rectToFind.Y;

            }

            ////Tuning For Ambiguous Height Added By Victor 2009/11/07
            //if (!IsFromBase & !IsCalibration)
            //{
            //    if ((CheckedCenter.Y - FoundCenterBias.Y) < 0)
            //    {
            //        AnalyzeXRange = XInflateVaue << 1;
            //        AnalyzeYMinRange = 60;
            //        AnalyzeYMaxRange = 500;

            //        rectCheckFound = AnalyzeRect(bmpTmp, MinGrade, MeaneGrade, ContrastRatio, true, rectToFind, bmpTmp1);
            //        rectCheckFoundF = retRectF;

            //        if (rectCheckFound.Width == 0 || rectCheckFound.Height == 0)
            //        {
            //            CheckedCenter.X = -100;
            //            CheckedCenter.Y = -100;

            //            CheckedCenterF.X = -100;
            //            CheckedCenterF.Y = -100;
            //        }
            //        else
            //        {
            //            CheckedCenter = JzTools.GetRectCenter(rectCheckFound);
            //            CheckedCenter.X += rectToFind.X;
            //            CheckedCenter.Y += rectToFind.Y;

            //            CheckedCenterF = JzTools.GetRectCenterF(rectCheckFoundF);
            //            CheckedCenterF.X += rectToFind.X;
            //            CheckedCenterF.Y += rectToFind.Y;
            //        }
            //    }
            //    if ((CheckedCenter.Y - FoundCenterBias.Y) * Resolution > 2.8)
            //    {
            //        AnalyzeXRange = XInflateVaue << 1;
            //        AnalyzeYMinRange = 50;
            //        AnalyzeYMaxRange = 300;

            //        rectCheckFound = AnalyzeRect(bmpTmp, MinGrade, MeaneGrade, ContrastRatio, true, rectToFind, bmpTmp1);
            //        rectCheckFoundF = retRectF;

            //        if (rectCheckFound.Width == 0 || rectCheckFound.Height == 0)
            //        {
            //            CheckedCenter.X = -100;
            //            CheckedCenter.Y = -100;
            //        }
            //        else
            //        {
            //            CheckedCenter = JzTools.GetRectCenter(rectCheckFound);
            //            CheckedCenter.X += rectToFind.X;
            //            CheckedCenter.Y += rectToFind.Y;

            //            CheckedCenterF = JzTools.GetRectCenterF(rectCheckFoundF);
            //            CheckedCenterF.X += rectToFind.X;
            //            CheckedCenterF.Y += rectToFind.Y;
            //        }
            //    }
            //    if ((double)(CheckedCenter.Y - FoundCenterBias.Y) * Resolution < 1.0)
            //    {
            //        Point OldCheckedCenter = CheckedCenter;
            //        PointF OldCheckedCenterF = CheckedCenterF;

            //        AnalyzeXRange = 4;
            //        AnalyzeYMinRange = 30;
            //        AnalyzeYMaxRange = 500;

            //        rectCheckFound = AnalyzeRect(bmpTmp, MinGrade, MeaneGrade, ContrastRatio, true, rectToFind, bmpTmp1);
            //        rectCheckFoundF = retRectF;

            //        if (rectCheckFound.Width == 0 || rectCheckFound.Height == 0)
            //        {
            //            CheckedCenter.X = -100;
            //            CheckedCenter.Y = -100;
            //        }
            //        else
            //        {
            //            CheckedCenter = JzTools.GetRectCenter(rectCheckFound);
            //            CheckedCenter.X += rectToFind.X;
            //            CheckedCenter.Y += rectToFind.Y;

            //            CheckedCenterF = JzTools.GetRectCenterF(rectCheckFoundF);
            //            CheckedCenterF.X += rectToFind.X;
            //            CheckedCenterF.Y += rectToFind.Y;
            //        }

            //        if ((double)(CheckedCenter.Y - FoundCenterBias.Y) * Resolution < 0 || (double)(CheckedCenter.Y - FoundCenterBias.Y) * Resolution > 2.8 + INI.BASEHEIGHT)
            //        {
            //            CheckedCenter = OldCheckedCenter;
            //            CheckedCenterF = OldCheckedCenterF;
            //        }

            //    }
            //}


            //Modified for Stability
            //if (!IsFromBase)
            /*
            {
                int DHeight = Math.Abs(LastCheckedCenter.Y - CheckedCenter.Y);

                if (DHeight > 2)
                {
                    LastCheckedCenter = CheckedCenter;
                }
                else if (DHeight > 1)
                {
                    if (LastCheckedCenter.Y - CheckedCenter.Y > 0)
                    {
                        CheckedCenter = LastCheckedCenter;
                        CheckedCenter.Y++;
                    }
                    else
                    {
                        CheckedCenter = LastCheckedCenter;
                        CheckedCenter.Y--;
                    }
                }
                else
                {
                    CheckedCenter = LastCheckedCenter;
                }
            }
            */
            //bmpNOISE.Dispose();

            bmpTmp.Dispose();
            bmpTmp1.Dispose();
        }

        public int Sequence = 0; //Sequential From Backward

        public void CheckSequential(Bitmap bmp)
        {
            Sequence = CheckSequential(myrect, bmp);
        }

        public int CheckSequential(Rectangle FromRect,Bitmap bmp)
        {
            int XInflateVaue = 10;
            Rectangle rectToFind = FromRect;
            //int FindingRange = 50;

            int GetHeight = FindingRange;

            rectToFind.Inflate(XInflateVaue, 0);
            rectToFind.Height += FindingRange;

            rectToFind.Intersect(JzTools.SimpleRect(bmp.Size));

            Bitmap bmpTmp = (Bitmap)bmp.Clone(rectToFind, PixelFormat.Format32bppArgb);


            if (INI.CUTPOINT > 0)
            {
                if (rectToFind.Y + GetHeight > INI.CUTPOINT)
                {
                    JzTools.DrawRect(bmpTmp, new Rectangle(0, INI.CUTPOINT - rectToFind.Y, INI.CCDWIDTH, INI.CCDHEIGHT), new SolidBrush(Color.FromArgb(MinGrade, MinGrade, MinGrade)));
                }
            }

            Bitmap bmpTmp1 = (Bitmap)bmpTmp.Clone();

            Threshold.SetThreshold(bmpTmp1, MinGrade, (int)((double)(MaxGrade - MinGrade) * ContrastRatio), MinGrade);
            FindObject.Find(bmpTmp1, Color.Red);

            //if (Name == "BASE-04046" || Name == "BASE-04053")
            {
                //bmpTmp1.Save(@"D:\LOA\NEWERA\SEQ\SEQCHECK" + Name + ".BMP", ImageFormat.Bmp);
            }

            int i = 0;
            List<string> RectStrList = new List<string>();

            while (i < FindObject.FoundList.Count)
            {
                Found found = FindObject.FoundList[i];

                if (found.rect.Width < FindMinWidth || found.rect.Height < FindMinHeigth || found.rect.Width < (int)((double)rectFound.Width * 0.5))// || found.rect.Height < (int)((double)rectFound.Height * 0.3))
                {
                    i++;
                    continue;
                }

                RectStrList.Add(JzTools.GetRectCenter(found.rect).Y.ToString("0000") + "," + i.ToString("000") + ",");
                i++;
            }

            //JzTools.DrawText(bmpTmp1, RectStrList.Count.ToString());
            //bmpTmp1.Save(@"D:\LOA\NEWERA\SEQ\SEQCHECK" + Name + ".BMP", ImageFormat.Bmp);

            bmpTmp1.Dispose();
            bmpTmp.Dispose();

            return RectStrList.Count;
        }

        public void CheckPixelBMP(Bitmap bmp)
        {
            int XInflateVaue = 10;
            Rectangle rectToFind = myrect;
            int GetHeight = 200;

            //if (!IsFromBase)
            {
                //ORIGIN 10mm
                //rectToFind.Inflate(XInflateVaue, 0);
                //rectToFind.Height += 200;

                ////ORIGIN 12mm
                //rectToFind.Inflate(XInflateVaue, 0);
                //rectToFind.Height += 500;

                //COMP 10.5mm
                //rectToFind.Inflate(XInflateVaue, 0);
                //rectToFind.Height += GetHeight;

                //COMP 11mm
                rectToFind.Inflate(XInflateVaue, 0);
                rectToFind.Height += GetHeight;

            }
            //else
            //{
            //    rectToFind.Inflate(XInflateVaue, 0);

            //    rectToFind.Y -= XInflateVaue;
            //    rectToFind.Height += 500;
            //}

            rectToFind.Intersect(JzTools.SimpleRect(bmp.Size));


            if (Name == "BASE-0500")
            {
                //JzTools.DrawRect(bmp, rectToFind, new Pen(Color.Red, 2));
                //bmp.Save(@"D:\LOA\NEWERA\CK01.BMP", ImageFormat.Bmp);
            }

            //JzTools.DrawRect(bmp, rectToFind, new Pen(Color.Red, 2));
            //bmp.Save(@"D:\LOA\NEWERA\CK01.BMP", ImageFormat.Bmp);
            //Bitmap bmpNOISE = (Bitmap)bmp.Clone();
            //JzTools.DrawRect(bmpNOISE, new Rectangle(0, 2400, INI.CCDWIDTH, INI.CCDHEIGHT), new SolidBrush(Color.FromArgb(MinGrade, MinGrade, MinGrade)));

            //bmpNOISE.Save(@"D:\LOA\NOISE.BMP", ImageFormat.Bmp);

            //Bitmap bmpTmp = (Bitmap)bmpNOISE.Clone(rectToFind, PixelFormat.Format32bppArgb);

            Bitmap bmpTmp = (Bitmap)bmp.Clone(rectToFind, PixelFormat.Format32bppArgb);

            if (INI.CUTPOINT > 0)
            {
                if (rectToFind.Y + GetHeight > INI.CUTPOINT)
                {
                    JzTools.DrawRect(bmpTmp, new Rectangle(0, INI.CUTPOINT - rectToFind.Y, INI.CCDWIDTH, INI.CCDHEIGHT), new SolidBrush(Color.FromArgb(MinGrade, MinGrade, MinGrade)));
                }
            }

            Bitmap bmpTmp1 = (Bitmap)bmpTmp.Clone();

            Histogram.GetHistogram(bmpTmp);
            Threshold.SetThreshold(bmpTmp, Histogram.MinGrade, (int)((double)(Histogram.MaxGrade - Histogram.MinGrade) * ContrastRatio), Histogram.MinGrade);

            //bmpTmp.Save(@"D:\LOA\NEWERA\N01.BMP", ImageFormat.Bmp);
            if (Name == "BASE-05093" || Name == "BASE-05096" || Name == "BASE-05104")
            {
                //bmpTmp.Save(@"D:\LOA\NEWERA\N01.BMP", ImageFormat.Bmp);
            }

            AnalyzeXRange = XInflateVaue << 1;
            //if (!IsFromBase)
            {
                ////ORIGIN 10MM
                //AnalyzeYMinRange = -20;
                //AnalyzeYMaxRange = 20;

                ////ORIGIN 12MM
                //AnalyzeYMinRange = 70;
                //AnalyzeYMaxRange = 500;

                //COMP 10.5MM
                //AnalyzeYMinRange = 20;
                //AnalyzeYMaxRange = 200;

                //COMP 11MM,11.MM
                AnalyzeYMinRange = 40;
                AnalyzeYMaxRange = 200;
            }
            //else
            //{
            //    AnalyzeYMinRange = -50;
            //    AnalyzeYMaxRange = 200;
            //}


            rectCheckFound = AnalyzeRect(bmpTmp, Histogram.MinGrade, Histogram.MeanGrade, ContrastRatio, true, rectToFind, bmpTmp1);
            rectCheckFoundF = retRectF;


            if (rectCheckFound.X == 0 && rectCheckFound.Y == 0)
            {
                rectCheckFound = rectCheckFound;

            }

            if (rectCheckFound.Width == 0 || rectCheckFound.Height == 0)
            {
                CheckedCenter.X = -100;
                CheckedCenter.Y = -100;

                CheckedCenterF.X = -100;
                CheckedCenterF.Y = -100;
            }
            else
            {
                CheckedCenter = JzTools.GetRectCenter(rectCheckFound);
                CheckedCenter.X += rectToFind.X;
                CheckedCenter.Y += rectToFind.Y;

                CheckedCenterF = JzTools.GetRectCenterF(rectCheckFoundF);
                CheckedCenterF.X += rectToFind.X;
                CheckedCenterF.Y += rectToFind.Y;


                //CheckedCenter.X = (int)JzTools.Round(CheckedCenterF.X);
                //CheckedCenter.Y = (int)JzTools.Round(CheckedCenterF.Y);
            }

            //Tuning For Ambiguous Height Added By Victor 2009/11/07
            //if (!IsFromBase & !IsCalibration)
            //{
            //    if ((CheckedCenter.Y - FoundCenterBias.Y) < 0)
            //    {
            //        AnalyzeXRange = XInflateVaue << 1;
            //        AnalyzeYMinRange = 40;
            //        AnalyzeYMaxRange = 500;

            //        rectCheckFound = AnalyzeRect(bmpTmp, MinGrade, MeaneGrade, ContrastRatio, true, rectToFind, bmpTmp1);

            //        if (rectCheckFound.Width == 0 || rectCheckFound.Height == 0)
            //        {
            //            CheckedCenter.X = -100;
            //            CheckedCenter.Y = -100;
            //        }
            //        else
            //        {
            //            CheckedCenter = JzTools.GetRectCenter(rectCheckFound);
            //            CheckedCenter.X += rectToFind.X;
            //            CheckedCenter.Y += rectToFind.Y;
            //        }
            //    }
            //    if ((CheckedCenter.Y - FoundCenterBias.Y) * Resolution > 3)
            //    {
            //        AnalyzeXRange = XInflateVaue << 1;
            //        AnalyzeYMinRange = 20;
            //        AnalyzeYMaxRange = 300;

            //        rectCheckFound = AnalyzeRect(bmpTmp, MinGrade, MeaneGrade, ContrastRatio, true, rectToFind, bmpTmp1);

            //        if (rectCheckFound.Width == 0 || rectCheckFound.Height == 0)
            //        {
            //            CheckedCenter.X = -100;
            //            CheckedCenter.Y = -100;
            //        }
            //        else
            //        {
            //            CheckedCenter = JzTools.GetRectCenter(rectCheckFound);
            //            CheckedCenter.X += rectToFind.X;
            //            CheckedCenter.Y += rectToFind.Y;
            //        }
            //    }
            //    if ((double)(CheckedCenter.Y - FoundCenterBias.Y) * Resolution < 1.4)
            //    {
            //        Point OldCheckedCenter = CheckedCenter;

            //        AnalyzeXRange = 4;
            //        AnalyzeYMinRange = 30;
            //        AnalyzeYMaxRange = 500;

            //        rectCheckFound = AnalyzeRect(bmpTmp, MinGrade, MeaneGrade, ContrastRatio, true, rectToFind, bmpTmp1);

            //        if (rectCheckFound.Width == 0 || rectCheckFound.Height == 0)
            //        {
            //            CheckedCenter.X = -100;
            //            CheckedCenter.Y = -100;
            //        }
            //        else
            //        {
            //            CheckedCenter = JzTools.GetRectCenter(rectCheckFound);
            //            CheckedCenter.X += rectToFind.X;
            //            CheckedCenter.Y += rectToFind.Y;
            //        }

            //        if ((double)(CheckedCenter.Y - FoundCenterBias.Y) * Resolution < 0 || (double)(CheckedCenter.Y - FoundCenterBias.Y) * Resolution > 2.8)
            //        {
            //            CheckedCenter = OldCheckedCenter;

            //        }

            //    }
            //}


            //Modified for Stability
            //if (!IsFromBase)
            //{
            //    int DHeight = Math.Abs(LastCheckedCenter.Y - CheckedCenter.Y);

            //    if (DHeight > 6)
            //    {
            //        LastCheckedCenter = CheckedCenter;
            //    }
            //    else if (DHeight > 3)
            //    {
            //        if (LastCheckedCenter.Y - CheckedCenter.Y > 0)
            //        {
            //            CheckedCenter = LastCheckedCenter;
            //            CheckedCenter.Y++;
            //        }
            //        else
            //        {
            //            CheckedCenter = LastCheckedCenter;
            //            CheckedCenter.Y--;
            //        }
            //    }
            //    else
            //    {
            //        CheckedCenter = LastCheckedCenter;
            //    }
            //}

            //bmpNOISE.Dispose();
            bmpTmp.Dispose();
            bmpTmp1.Dispose();
        }
        public int AnalyzeXRange = 0;
        public int AnalyzeYMinRange = 0;
        public int AnalyzeYMaxRange = 0;

        public double RatioIndicator(int ix)
        {
            switch (ix)
            {
                case 0:
                    return 1d;
                case 1:
                    return 1.01d;
                case 2:
                    return 1.02d;
                case 3:
                    return 0.99d;
                case 4:
                    return 0.98d;
                case 5:
                    return 1.03d;
                case 6:
                    return 0.97d;
                case 7:
                    return 1.015d;
                case 8:
                    return 0.985d;
                case 9:
                    return 1.025d;
                case 10:
                    return 0.975d;
                case 11:
                    return 1.03d;
                case 12:
                    return 0.97d;
                case 13:
                    return 1.04d;
                case 14:
                    return 0.96d;
                case 15:
                    return 1.05d;
                case 16:
                    return 0.95d;
                case 17:
                    return 1.06d;
                case 18:
                    return 0.94d;
            }

            return 1d;
        }

        public RectangleF retRectF = new RectangleF();

        public Rectangle AnalyzeRect(Bitmap bmp, int MinGrade, int MeanGrade, double Ratio)
        {
            return AnalyzeRect(bmp, MinGrade, MeaneGrade, Ratio, false, new Rectangle(), bmpOrigion);
        }
        public Rectangle AnalyzeRect(Bitmap bmp,int MinGrade,int MeanGrade,double Ratio,bool IsFinding,Rectangle AlterRect,Bitmap bmpPreFind)
        {
            const int RectToBeAnalyzed = 17;
            int ix = 0;

            int X = 0;
            int Y = 0;
            int W = 0;
            int H = 0;

            Rectangle[] ixRect = new Rectangle[RectToBeAnalyzed];

            Bitmap bmp0 = (Bitmap)bmp.Clone();

            if (Name == "BASE-04046")
            {
                //Name = Name;
                //bmp.Save(@"D:\LOA\NEWERA\KK0.BMP", ImageFormat.Bmp);
                //bmp0.Save(@"D:\LOA\NEWERA\KK1.BMP", ImageFormat.Bmp);
            }


            FindObject.Find(bmp0, Color.Red);

            if (Name == "BASE-04053")
            {
                //Name = Name;
                //bmp0.Save(@"D:\LOA\NEWERA\KK2.BMP", ImageFormat.Bmp);
            }

            

            Rectangle MaxRect = FindObject.rectMaxRect;
            Rectangle MaxRect2Stage = FindObject.rectMaxRect;
            RectangleF MaxRect2StageF = (RectangleF)FindObject.rectMaxRect;

            if (IsFinding)
            {
                //Point PtAlterPoint = myCenter;
                Point PtAlterPoint = FoundCenter;

                PtAlterPoint.X -= (AlterRect.X - myrect.X);
                PtAlterPoint.Y -= (AlterRect.Y - myrect.Y);

                //if (Name == "BASE-0519")
                //{
                //JzTools.DrawRect(bmpThreshedComp, JzTools.SimpleRect(PtAlterPoint), new Pen(Color.Red));
                //bmpThreshedComp.Save(@"D:\LOA\NEWERA\CHECK.BMP", ImageFormat.Bmp);
                //}

                //JzTools.DrawRect(bmp, MaxRect, new Pen(Color.Red, 2));
                //bmp.Save(@"D:\LOA\NEWERA\CHECK.BMP", ImageFormat.Bmp);

                //MaxRect.Height = rectFound.Height;
                //MaxRect.Width = rectFound.Width << 3;
                MaxRect.Height = rectFound.Height;
                MaxRect.Width = rectFound.Width;

                if (Name == "BASE-04046")
                {
                    Name = Name;
                    //bmp0.Save(@"D:\LOA\NEWERA\KK2.BMP", ImageFormat.Bmp);
                }

                if (INI.IS_CHECK_LEVEL)
                    MaxRect = FindObject.GetRectNearestAllinone(PtAlterPoint, MaxRect, AnalyzeXRange, AnalyzeYMinRange, AnalyzeYMaxRange);
                else
                    MaxRect = FindObject.GetRectNearest(PtAlterPoint, MaxRect, AnalyzeXRange, AnalyzeYMinRange, AnalyzeYMaxRange);
                //MaxRect = FindObject.GetRectNearest(PtAlterPoint, MaxRect, AnalyzeXRange, AnalyzeYMinRange,AnalyzeYMaxRange);


                Rectangle MaxTmpRect = MaxRect;

                //if (RESULT.IsUseFindingCheck)//Gaara by mask
                    if (true)
                    {
                    if (FindObject.RectList.Count > 1)
                    {
                        //Name = Name;
                        //If there is something ambiguous
                        ix = FindObject.RectList.Count - 1;

                        List<string> MoreRectstrList = new List<string>();

                        while (ix > -1)
                        {
                            Rectangle rectTmp = FindObject.RectList[ix];

                            rectTmp.X += AlterRect.X;
                            rectTmp.Y += AlterRect.Y;

                            int GetSeq = CheckSequential(rectTmp, new Bitmap(3840, 2764));
                            //int GetSeq = CheckSequential(rectTmp, RESULT.bmpWorking);//Gaara by mask

                            if (Sequence == GetSeq)
                            {
                                MoreRectstrList.Add(FindObject.RectList[ix].Y.ToString("0000") + "@" + JzTools.RecttoStringEX(FindObject.RectList[ix]));
                                //MaxRect = FindObject.RectList[ix];
                                //break;
                            }
                            ix--;
                        }

                        if (MoreRectstrList.Count == 1)
                        {
                            MaxRect = JzTools.StringtoRect(MoreRectstrList[0].Split('@')[1]);
                        }
                        else if (MoreRectstrList.Count > 1)
                        {
                            MoreRectstrList.Sort();
                            MaxRect = JzTools.StringtoRect(MoreRectstrList[0].Split('@')[1]);
                        }
                    }
                }

                if (Name == "BASE-04052" || Name == "BASE-04045" || Name == "BASE-05104")
                {
                    Name = Name;
                    //bmp0.Save(@"D:\LOA\NEWERA\KK2.BMP", ImageFormat.Bmp);
                }

                if (!IsFromBase && !IsCalibration)
                {
                    if (MaxRect.Y > MaxTmpRect.Y)
                    {
                        MaxRect = MaxTmpRect;
                    }
                }

                MaxRect.Inflate(10, 10);
                MaxRect.Intersect(JzTools.SimpleRect(bmpPreFind.Size));

                ix = 0;

                while (ix < RectToBeAnalyzed)
                {
                    bmp0.Dispose();
                    bmp0 = (Bitmap)bmpPreFind.Clone(MaxRect, PixelFormat.Format32bppArgb);

                    //bmp0.Save(@"D:\LOA\TESTRESULT\ATP0.BMP", ImageFormat.Bmp);
                    bmpOrigionSized.Dispose();
                    bmpOrigionSized = new Bitmap(bmp0, JzTools.Resize(bmp0.Size, SizedRatio));

                    Histogram.GetHistogram(bmpOrigionSized);
                    Threshold.SetThreshold(bmp0, Histogram.MinGrade, (int)((double)(Histogram.MaxGrade - Histogram.MinGrade) * Ratio * RatioIndicator(ix)), Histogram.MinGrade);

                    //bmp0.Save(@"D:\LOA\TESTRESULT\ATP1.BMP", ImageFormat.Bmp);
                    FindObject.Find(bmp0, Color.Red);
                    //bmp0.Save(@"D:\LOA\TESTRESULT\ATP2.BMP", ImageFormat.Bmp);
                    MaxRect2Stage = FindObject.rectMaxRect;

                    //bmp0.Dispose();
                    //bmp0 = (Bitmap)bmpPreFind.Clone(MaxRect, PixelFormat.Format32bppArgb);

                    //MaxRect2Stage = Threshold.ClearStinRect(bmp0, MaxRect2Stage, Color.Red, 0.1);

                    ixRect[ix] = MaxRect2Stage;

                    ix++;
                }


                ix = 0;
                while (ix < RectToBeAnalyzed)
                {
                    X += ixRect[ix].X;
                    Y += ixRect[ix].Y;
                    W += ixRect[ix].Width;
                    H += ixRect[ix].Height;

                    ix++;
                }

                MaxRect2Stage.X = (int)JzTools.Round((double)X / (double)RectToBeAnalyzed);
                MaxRect2Stage.Y = (int)JzTools.Round((double)Y / (double)RectToBeAnalyzed);
                MaxRect2Stage.Width = (int)JzTools.Round((double)W / (double)RectToBeAnalyzed);
                MaxRect2Stage.Height = (int)JzTools.Round((double)H / (double)RectToBeAnalyzed);


                MaxRect2StageF.X = (float)((float)X / (float)RectToBeAnalyzed);
                MaxRect2StageF.Y = (float)((float)Y / (float)RectToBeAnalyzed);
                MaxRect2StageF.Width = (float)((float)W / (float)RectToBeAnalyzed);
                MaxRect2StageF.Height = (float)((float)H / (float)RectToBeAnalyzed);

                //bmpPreFind.Save(@"D:\LOA\TESTRESULT\ATPZ.BMP", ImageFormat.Bmp);

                //bmp0.Dispose();
                //bmp0 = (Bitmap)bmpPreFind.Clone(MaxRect, PixelFormat.Format32bppArgb);

                //bmp0.Save(@"D:\LOA\NEWERA\ATP0.BMP", ImageFormat.Bmp);

                //bmpOrigionSized.Dispose();
                //bmpOrigionSized = new Bitmap(bmp0, JzTools.Resize(bmp0.Size, SizedRatio));

                //Histogram.GetHistogram(bmpOrigionSized);
                //Threshold.SetThreshold(bmp0, Histogram.MinGrade, (int)((double)(Histogram.MaxGrade - Histogram.MinGrade) * Ratio), Histogram.MinGrade);
                ////Threshold.SetThreshold(bmp0, MinGrade, (int)((double)(MaxGrade - MinGrade) * Ratio), MinGrade);

                ////bmp0.Save(@"D:\LOA\TESTRESULT\ATP1.BMP", ImageFormat.Bmp);

                //FindObject.Find(bmp0, Color.Red);

                ////bmp0.Save(@"D:\LOA\TESTRESULT\ATP2.BMP", ImageFormat.Bmp);

                //MaxRect2Stage = FindObject.rectMaxRect;

                //bmp0.Dispose();
                //bmp0 = (Bitmap)bmpPreFind.Clone(MaxRect, PixelFormat.Format32bppArgb);

                //JzTools.DrawRect(bmp0, JzTools.SimpleRect(JzTools.GetRectCenter(MaxRect2Stage), 3), new SolidBrush(Color.FromArgb(Histogram.MaxGrade, Histogram.MaxGrade, Histogram.MaxGrade)));
                //FindObject.FindGrayscale(bmp0, JzTools.GetRectCenter(MaxRect2Stage), Color.Red, Histogram.MaxGrade, (int)((double)(Histogram.MaxGrade - Histogram.MinGrade) * MagicNumber2));

                ////bmp0.Save(@"D:\LOA\TESTRESULT\ATP3.BMP", ImageFormat.Bmp);
                //MaxRect2Stage = FindObject.rectMaxRect;

                ////清除毛剌

                ////bmp0.Save(@"D:\LOA\TESTRESULT\ATP3.BMP", ImageFormat.Bmp);

                //MaxRect2Stage = Threshold.ClearStinRect(bmp0, MaxRect2Stage, Color.Red, 0.1);

                //bmp0.Save(@"D:\LOA\TESTRESULT\ATP4.BMP", ImageFormat.Bmp);

                //JzTools.DrawRect(bmp0, MaxRect2Stage, new Pen(Color.Blue, 1));

                //bmp0.Save(@"D:\LOA\TESTRESULT\ATP5.BMP", ImageFormat.Bmp);

                if (!(MaxRect2Stage.X == 0 && MaxRect2Stage.Y == 0))
                {

                    MaxRect2Stage.X += MaxRect.X;
                    MaxRect2Stage.Y += MaxRect.Y;

                    MaxRect2StageF.X += MaxRect.X;
                    MaxRect2StageF.Y += MaxRect.Y;
                }

            }
            else
            {
                MaxRect.Inflate(10, 10);

                MaxRect.Intersect(JzTools.SimpleRect(bmpPreFind.Size));

                //bmpPreFind.Save(@"D:\LOA\TESTRESULT\ATPZ.BMP", ImageFormat.Bmp);
                ix = 0;

                while (ix < RectToBeAnalyzed)
                {
                    bmp0.Dispose();
                    bmp0 = (Bitmap)bmpPreFind.Clone(MaxRect, PixelFormat.Format32bppArgb);

                    //bmp0.Save(@"D:\LOA\TESTRESULT\ATP0.BMP", ImageFormat.Bmp);
                    bmpOrigionSized.Dispose();
                    bmpOrigionSized = new Bitmap(bmp0, JzTools.Resize(bmp0.Size, SizedRatio));

                    Histogram.GetHistogram(bmpOrigionSized);
                    Threshold.SetThreshold(bmp0, Histogram.MinGrade, (int)((double)(Histogram.MaxGrade - Histogram.MinGrade) * Ratio * RatioIndicator(ix)), Histogram.MinGrade);

                    //bmp0.Save(@"D:\LOA\TESTRESULT\ATP1.BMP", ImageFormat.Bmp);
                    FindObject.Find(bmp0, Color.Red);
                    //bmp0.Save(@"D:\LOA\TESTRESULT\ATP2.BMP", ImageFormat.Bmp);
                    MaxRect2Stage = FindObject.rectMaxRect;

                    //bmp0.Dispose();
                    //bmp0 = (Bitmap)bmpPreFind.Clone(MaxRect, PixelFormat.Format32bppArgb);

                    //MaxRect2Stage = Threshold.ClearStinRect(bmp0, MaxRect2Stage, Color.Red, 0.1);

                    ixRect[ix] = MaxRect2Stage;

                    ix++;
                }


                ix = 0;
                while (ix < RectToBeAnalyzed)
                {
                    X += ixRect[ix].X;
                    Y += ixRect[ix].Y;
                    W += ixRect[ix].Width;
                    H += ixRect[ix].Height;

                    ix++;
                }

                MaxRect2Stage.X = (int)JzTools.Round((double)X / (double)RectToBeAnalyzed);
                MaxRect2Stage.Y = (int)JzTools.Round((double)Y / (double)RectToBeAnalyzed);
                MaxRect2Stage.Width = (int)JzTools.Round((double)W / (double)RectToBeAnalyzed);
                MaxRect2Stage.Height = (int)JzTools.Round((double)H / (double)RectToBeAnalyzed);


                MaxRect2StageF.X = (float)((double)X / (double)RectToBeAnalyzed);
                MaxRect2StageF.Y = (float)((double)Y / (double)RectToBeAnalyzed);
                MaxRect2StageF.Width = (float)((double)W / (double)RectToBeAnalyzed);
                MaxRect2StageF.Height = (float)((double)H / (double)RectToBeAnalyzed);

                //JzTools.DrawRect(bmp0, JzTools.SimpleRect(JzTools.GetRectCenter(MaxRect2Stage), 3), new SolidBrush(Color.FromArgb(Histogram.MaxGrade, Histogram.MaxGrade, Histogram.MaxGrade)));
                //FindObject.FindGrayscale(bmp0, JzTools.GetRectCenter(MaxRect2Stage), Color.Red, Histogram.MaxGrade, (int)((double)(Histogram.MaxGrade - Histogram.MinGrade) * Ratio));

                ////bmp0.Save(@"D:\LOA\TESTRESULT\ATP3.BMP", ImageFormat.Bmp);
                //MaxRect2Stage = FindObject.rectMaxRect;

                //MaxRect.X += MaxRect2Stage.X;
                //MaxRect.Y += MaxRect2Stage.Y;

                if (!(MaxRect2Stage.X == 0 && MaxRect2Stage.Y == 0))
                {
                    MaxRect2Stage.X += MaxRect.X;
                    MaxRect2Stage.Y += MaxRect.Y;

                    MaxRect2StageF.X += MaxRect.X;
                    MaxRect2StageF.Y += MaxRect.Y;
                }
            }
            
            //Rectangle recttmp= JzTools.GetVerticalRect(FindObject.rectMaxRect, -3);

            Rectangle rectOrg = MaxRect2Stage;

            retRectF = MaxRect2StageF;
            //rectOrg.Inflate(0, -1);
            //取得直的中心寬度

            //rectOrg.Width = Math.Max(1, rectOrg.Width);
            //rectOrg.Height = Math.Max(1, rectOrg.Height);


            //if (rectOrg != new Rectangle())
            //{
            //    bmp0.Dispose();
            //    bmp0 = (Bitmap)bmp.Clone(rectOrg, PixelFormat.Format32bppArgb);

            //    ////bmp0.Save(@"D:\LOA\BASECENTER\002.BMP", ImageFormat.Bmp);
            //    ////Threshold.SetThreshold(bmp0, MinGrade, (int)((double)(MeaneGrade - MinGrade) * Ratio), MinGrade);
            //    //if (IsCalibration)
            //    //{
            //    //    bmp0.Dispose();
            //    //    bmp0 = (Bitmap)bmp.Clone(rectOrg, PixelFormat.Format32bppArgb);
            //    //}
            //    //else
            //    //{
            //    //    bmp0.Dispose();
            //    //    bmp0 = (Bitmap)bmpPreFind.Clone(rectOrg, PixelFormat.Format32bppArgb);

            //    //    //bmp0.Save(@"D:\LOA\BASECENTER\002.BMP", ImageFormat.Bmp);

            //    //    Histogram.GetHistogram(bmp0);
            //    //    Threshold.SetThreshold(bmp0, MinGrade, (int)((double)(MeanGrade - MinGrade) * Ratio), MinGrade);
            //    //}

            //    FindObject.Find(bmp0, Color.Red);
            //    //bmp0.Save(@"D:\LOA\BASECENTER\003.BMP", ImageFormat.Bmp);

            //    Rectangle rect = FindObject.rectMaxRect;

            //    rect.X = rectOrg.X;
            //    rect.Y += rectOrg.Y;

            //    rect.Width = rectOrg.Width;

            //    rectOrg = rect;
            //    //retPoint = JzTools.GetRectCenter(rect);
            //}

            bmp0.Dispose();

            return rectOrg;
        }
        public Rectangle AnalyzeRectEX(Bitmap bmp, int MinGrade, int MeanGrade, double Ratio, bool IsFinding, Rectangle AlterRect, Bitmap bmpPreFind)
        {
            const int RectToBeAnalyzed = 18;

            int ix = 0;

            int X = 0;
            int Y = 0;
            int W = 0;
            int H = 0;

            Rectangle[] ixRect = new Rectangle[RectToBeAnalyzed];

            Bitmap bmp0 = (Bitmap)bmp.Clone();

            if (Name == "BASE-00021")
            {
                //Name = Name;
                //bmp.Save(@"D:\LOA\NEWERA\KK0.BMP", ImageFormat.Bmp);
                //bmp0.Save(@"D:\LOA\NEWERA\KK1.BMP", ImageFormat.Bmp);
            }


            FindObject.Find(bmp0, Color.Red);

            if (Name == "BASE-00021")
            {
                //Name = Name;
                //bmp0.Save(@"D:\LOA\NEWERA\KK2.BMP", ImageFormat.Bmp);
            }



            Rectangle MaxRect = FindObject.rectMaxRect;
            Rectangle MaxRect2Stage = FindObject.rectMaxRect;
            RectangleF MaxRect2StageF = (RectangleF)FindObject.rectMaxRect;

            if (IsFinding)
            {
                //Point PtAlterPoint = myCenter;
                Point PtAlterPoint = FoundCenter;

                PtAlterPoint.X -= (AlterRect.X - myrect.X);
                PtAlterPoint.Y -= (AlterRect.Y - myrect.Y);

                //if (Name == "BASE-0519")
                //{
                //JzTools.DrawRect(bmpThreshedComp, JzTools.SimpleRect(PtAlterPoint), new Pen(Color.Red));
                //bmpThreshedComp.Save(@"D:\LOA\NEWERA\CHECK.BMP", ImageFormat.Bmp);
                //}

                //JzTools.DrawRect(bmp, MaxRect, new Pen(Color.Red, 2));
                //bmp.Save(@"D:\LOA\NEWERA\CHECK.BMP", ImageFormat.Bmp);

                //MaxRect.Height = rectFound.Height;
                //MaxRect.Width = rectFound.Width << 3;
                MaxRect.Height = rectFound.Height;
                MaxRect.Width = rectFound.Width;

                //if (Name == "BASE-00091")
                //{
                //    Name = Name;
                //    //bmp0.Save(@"D:\LOA\NEWERA\KK2.BMP", ImageFormat.Bmp);
                //}

                //MaxRect = FindObject.GetRectNearestEX(PtAlterPoint, MaxRect, AnalyzeXRange, AnalyzeYMinRange, AnalyzeYMaxRange);
                //MaxRect = FindObject.GetRectNearestEX(PtAlterPoint, MaxRect, AnalyzeXRange, AnalyzeYMinRange, AnalyzeYMaxRange);

                if (INI.IS_CHECK_LEVEL)
                    MaxRect = FindObject.GetRectNearestEXAllinone(PtAlterPoint, MaxRect, AnalyzeXRange, AnalyzeYMinRange, AnalyzeYMaxRange);
                else
                    MaxRect = FindObject.GetRectNearestEX(PtAlterPoint, MaxRect, AnalyzeXRange, AnalyzeYMinRange, AnalyzeYMaxRange);


                Rectangle MaxTmpRect = MaxRect;

                //if (RESULT.IsUseFindingCheck)
                //{
                //    if (FindObject.RectList.Count > 1)
                //    {
                //        //Name = Name;
                //        //If there is something ambiguous
                //        ix = FindObject.RectList.Count - 1;

                //        List<string> MoreRectstrList = new List<string>();

                //        while (ix > -1)
                //        {
                //            Rectangle rectTmp = FindObject.RectList[ix];

                //            rectTmp.X += AlterRect.X;
                //            rectTmp.Y += AlterRect.Y;

                //            int GetSeq = CheckSequential(rectTmp, RESULT.bmpWorking);

                //            if (Sequence == GetSeq)
                //            {
                //                MoreRectstrList.Add(FindObject.RectList[ix].Y.ToString("0000") + "@" + JzTools.RecttoStringEX(FindObject.RectList[ix]));
                //                //MaxRect = FindObject.RectList[ix];
                //                //break;
                //            }
                //            ix--;
                //        }

                //        if (MoreRectstrList.Count == 1)
                //        {
                //            MaxRect = JzTools.StringtoRect(MoreRectstrList[0].Split('@')[1]);
                //        }
                //        else if (MoreRectstrList.Count > 1)
                //        {
                //            MoreRectstrList.Sort();
                //            MaxRect = JzTools.StringtoRect(MoreRectstrList[0].Split('@')[1]);
                //        }
                //    }
                //}

                //if (Name == "BASE-00091")
                //{
                //    Name = Name;
                //    bmp0.Save(@"D:\LOA\NEWERA\KK2.BMP", ImageFormat.Bmp);
                //}

                //if (!IsFromBase && !IsCalibration)
                //{
                //    if (MaxRect.Y > MaxTmpRect.Y)
                //    {
                //        MaxRect = MaxTmpRect;
                //    }
                //}

                MaxRect.Inflate(10, 10);
                MaxRect.Intersect(JzTools.SimpleRect(bmpPreFind.Size));

                ix = 0;

                while (ix < RectToBeAnalyzed)
                {
                    bmp0.Dispose();
                    bmp0 = (Bitmap)bmpPreFind.Clone(MaxRect, PixelFormat.Format32bppArgb);

                    //bmp0.Save(@"D:\LOA\TESTRESULT\ATP0.BMP", ImageFormat.Bmp);
                    bmpOrigionSized.Dispose();
                    bmpOrigionSized = new Bitmap(bmp0, JzTools.Resize(bmp0.Size, SizedRatio));

                    Histogram.GetHistogram(bmpOrigionSized);
                    Threshold.SetThreshold(bmp0, Histogram.MinGrade, (int)((double)(Histogram.MaxGrade - Histogram.MinGrade) * Ratio * RatioIndicator(ix)), Histogram.MinGrade);

                    //bmp0.Save(@"D:\LOA\TESTRESULT\ATP1.BMP", ImageFormat.Bmp);
                    FindObject.Find(bmp0, Color.Red);
                    //bmp0.Save(@"D:\LOA\TESTRESULT\ATP2.BMP", ImageFormat.Bmp);
                    MaxRect2Stage = FindObject.rectMaxRect;

                    //bmp0.Dispose();
                    //bmp0 = (Bitmap)bmpPreFind.Clone(MaxRect, PixelFormat.Format32bppArgb);

                    //MaxRect2Stage = Threshold.ClearStinRect(bmp0, MaxRect2Stage, Color.Red, 0.1);

                    ixRect[ix] = MaxRect2Stage;

                    ix++;
                }


                ix = 0;
                while (ix < RectToBeAnalyzed)
                {
                    X += ixRect[ix].X;
                    Y += ixRect[ix].Y;
                    W += ixRect[ix].Width;
                    H += ixRect[ix].Height;

                    ix++;
                }

                MaxRect2Stage.X = (int)JzTools.Round((double)X / (double)RectToBeAnalyzed);
                MaxRect2Stage.Y = (int)JzTools.Round((double)Y / (double)RectToBeAnalyzed);
                MaxRect2Stage.Width = (int)JzTools.Round((double)W / (double)RectToBeAnalyzed);
                MaxRect2Stage.Height = (int)JzTools.Round((double)H / (double)RectToBeAnalyzed);


                MaxRect2StageF.X = (float)((float)X / (float)RectToBeAnalyzed);
                MaxRect2StageF.Y = (float)((float)Y / (float)RectToBeAnalyzed);
                MaxRect2StageF.Width = (float)((float)W / (float)RectToBeAnalyzed);
                MaxRect2StageF.Height = (float)((float)H / (float)RectToBeAnalyzed);

                //bmpPreFind.Save(@"D:\LOA\TESTRESULT\ATPZ.BMP", ImageFormat.Bmp);

                //bmp0.Dispose();
                //bmp0 = (Bitmap)bmpPreFind.Clone(MaxRect, PixelFormat.Format32bppArgb);

                //bmp0.Save(@"D:\LOA\NEWERA\ATP0.BMP", ImageFormat.Bmp);

                //bmpOrigionSized.Dispose();
                //bmpOrigionSized = new Bitmap(bmp0, JzTools.Resize(bmp0.Size, SizedRatio));

                //Histogram.GetHistogram(bmpOrigionSized);
                //Threshold.SetThreshold(bmp0, Histogram.MinGrade, (int)((double)(Histogram.MaxGrade - Histogram.MinGrade) * Ratio), Histogram.MinGrade);
                ////Threshold.SetThreshold(bmp0, MinGrade, (int)((double)(MaxGrade - MinGrade) * Ratio), MinGrade);

                ////bmp0.Save(@"D:\LOA\TESTRESULT\ATP1.BMP", ImageFormat.Bmp);

                //FindObject.Find(bmp0, Color.Red);

                ////bmp0.Save(@"D:\LOA\TESTRESULT\ATP2.BMP", ImageFormat.Bmp);

                //MaxRect2Stage = FindObject.rectMaxRect;

                //bmp0.Dispose();
                //bmp0 = (Bitmap)bmpPreFind.Clone(MaxRect, PixelFormat.Format32bppArgb);

                //JzTools.DrawRect(bmp0, JzTools.SimpleRect(JzTools.GetRectCenter(MaxRect2Stage), 3), new SolidBrush(Color.FromArgb(Histogram.MaxGrade, Histogram.MaxGrade, Histogram.MaxGrade)));
                //FindObject.FindGrayscale(bmp0, JzTools.GetRectCenter(MaxRect2Stage), Color.Red, Histogram.MaxGrade, (int)((double)(Histogram.MaxGrade - Histogram.MinGrade) * MagicNumber2));

                ////bmp0.Save(@"D:\LOA\TESTRESULT\ATP3.BMP", ImageFormat.Bmp);
                //MaxRect2Stage = FindObject.rectMaxRect;

                ////清除毛剌

                ////bmp0.Save(@"D:\LOA\TESTRESULT\ATP3.BMP", ImageFormat.Bmp);

                //MaxRect2Stage = Threshold.ClearStinRect(bmp0, MaxRect2Stage, Color.Red, 0.1);

                //bmp0.Save(@"D:\LOA\TESTRESULT\ATP4.BMP", ImageFormat.Bmp);

                //JzTools.DrawRect(bmp0, MaxRect2Stage, new Pen(Color.Blue, 1));

                //bmp0.Save(@"D:\LOA\TESTRESULT\ATP5.BMP", ImageFormat.Bmp);

                if (!(MaxRect2Stage.X == 0 && MaxRect2Stage.Y == 0))
                {

                    MaxRect2Stage.X += MaxRect.X;
                    MaxRect2Stage.Y += MaxRect.Y;

                    MaxRect2StageF.X += MaxRect.X;
                    MaxRect2StageF.Y += MaxRect.Y;
                }

            }
            else
            {
                MaxRect.Inflate(10, 10);

                MaxRect.Intersect(JzTools.SimpleRect(bmpPreFind.Size));

                //bmpPreFind.Save(@"D:\LOA\TESTRESULT\ATPZ.BMP", ImageFormat.Bmp);
                ix = 0;

                while (ix < RectToBeAnalyzed)
                {
                    bmp0.Dispose();
                    bmp0 = (Bitmap)bmpPreFind.Clone(MaxRect, PixelFormat.Format32bppArgb);

                    //bmp0.Save(@"D:\LOA\TESTRESULT\ATP0.BMP", ImageFormat.Bmp);
                    bmpOrigionSized.Dispose();
                    bmpOrigionSized = new Bitmap(bmp0, JzTools.Resize(bmp0.Size, SizedRatio));

                    Histogram.GetHistogram(bmpOrigionSized);
                    Threshold.SetThreshold(bmp0, Histogram.MinGrade, (int)((double)(Histogram.MaxGrade - Histogram.MinGrade) * Ratio * RatioIndicator(ix)), Histogram.MinGrade);

                    //bmp0.Save(@"D:\LOA\TESTRESULT\ATP1.BMP", ImageFormat.Bmp);
                    FindObject.Find(bmp0, Color.Red);
                    //bmp0.Save(@"D:\LOA\TESTRESULT\ATP2.BMP", ImageFormat.Bmp);
                    MaxRect2Stage = FindObject.rectMaxRect;

                    //bmp0.Dispose();
                    //bmp0 = (Bitmap)bmpPreFind.Clone(MaxRect, PixelFormat.Format32bppArgb);

                    //MaxRect2Stage = Threshold.ClearStinRect(bmp0, MaxRect2Stage, Color.Red, 0.1);

                    ixRect[ix] = MaxRect2Stage;

                    ix++;
                }


                ix = 0;
                while (ix < RectToBeAnalyzed)
                {
                    X += ixRect[ix].X;
                    Y += ixRect[ix].Y;
                    W += ixRect[ix].Width;
                    H += ixRect[ix].Height;

                    ix++;
                }

                MaxRect2Stage.X = (int)JzTools.Round((double)X / (double)RectToBeAnalyzed);
                MaxRect2Stage.Y = (int)JzTools.Round((double)Y / (double)RectToBeAnalyzed);
                MaxRect2Stage.Width = (int)JzTools.Round((double)W / (double)RectToBeAnalyzed);
                MaxRect2Stage.Height = (int)JzTools.Round((double)H / (double)RectToBeAnalyzed);


                MaxRect2StageF.X = (float)((double)X / (double)RectToBeAnalyzed);
                MaxRect2StageF.Y = (float)((double)Y / (double)RectToBeAnalyzed);
                MaxRect2StageF.Width = (float)((double)W / (double)RectToBeAnalyzed);
                MaxRect2StageF.Height = (float)((double)H / (double)RectToBeAnalyzed);

                //JzTools.DrawRect(bmp0, JzTools.SimpleRect(JzTools.GetRectCenter(MaxRect2Stage), 3), new SolidBrush(Color.FromArgb(Histogram.MaxGrade, Histogram.MaxGrade, Histogram.MaxGrade)));
                //FindObject.FindGrayscale(bmp0, JzTools.GetRectCenter(MaxRect2Stage), Color.Red, Histogram.MaxGrade, (int)((double)(Histogram.MaxGrade - Histogram.MinGrade) * Ratio));

                ////bmp0.Save(@"D:\LOA\TESTRESULT\ATP3.BMP", ImageFormat.Bmp);
                //MaxRect2Stage = FindObject.rectMaxRect;

                //MaxRect.X += MaxRect2Stage.X;
                //MaxRect.Y += MaxRect2Stage.Y;

                if (!(MaxRect2Stage.X == 0 && MaxRect2Stage.Y == 0))
                {
                    MaxRect2Stage.X += MaxRect.X;
                    MaxRect2Stage.Y += MaxRect.Y;

                    MaxRect2StageF.X += MaxRect.X;
                    MaxRect2StageF.Y += MaxRect.Y;
                }
            }

            //Rectangle recttmp= JzTools.GetVerticalRect(FindObject.rectMaxRect, -3);

            Rectangle rectOrg = MaxRect2Stage;

            retRectF = MaxRect2StageF;
            //rectOrg.Inflate(0, -1);
            //取得直的中心寬度

            //rectOrg.Width = Math.Max(1, rectOrg.Width);
            //rectOrg.Height = Math.Max(1, rectOrg.Height);


            //if (rectOrg != new Rectangle())
            //{
            //    bmp0.Dispose();
            //    bmp0 = (Bitmap)bmp.Clone(rectOrg, PixelFormat.Format32bppArgb);

            //    ////bmp0.Save(@"D:\LOA\BASECENTER\002.BMP", ImageFormat.Bmp);
            //    ////Threshold.SetThreshold(bmp0, MinGrade, (int)((double)(MeaneGrade - MinGrade) * Ratio), MinGrade);
            //    //if (IsCalibration)
            //    //{
            //    //    bmp0.Dispose();
            //    //    bmp0 = (Bitmap)bmp.Clone(rectOrg, PixelFormat.Format32bppArgb);
            //    //}
            //    //else
            //    //{
            //    //    bmp0.Dispose();
            //    //    bmp0 = (Bitmap)bmpPreFind.Clone(rectOrg, PixelFormat.Format32bppArgb);

            //    //    //bmp0.Save(@"D:\LOA\BASECENTER\002.BMP", ImageFormat.Bmp);

            //    //    Histogram.GetHistogram(bmp0);
            //    //    Threshold.SetThreshold(bmp0, MinGrade, (int)((double)(MeanGrade - MinGrade) * Ratio), MinGrade);
            //    //}

            //    FindObject.Find(bmp0, Color.Red);
            //    //bmp0.Save(@"D:\LOA\BASECENTER\003.BMP", ImageFormat.Bmp);

            //    Rectangle rect = FindObject.rectMaxRect;

            //    rect.X = rectOrg.X;
            //    rect.Y += rectOrg.Y;

            //    rect.Width = rectOrg.Width;

            //    rectOrg = rect;
            //    //retPoint = JzTools.GetRectCenter(rect);
            //}

            bmp0.Dispose();

            return rectOrg;
        }
        public Rectangle AnalyzeRectFX(Bitmap bmp, int MinGrade, int MeanGrade, double Ratio, bool IsFinding, Rectangle AlterRect, Bitmap bmpPreFind)
        {
            const int RectToBeAnalyzed = 11;

            int ix = 0;

            int X = 0;
            int Y = 0;
            int W = 0;
            int H = 0;


            double POSFinal = 0;
            double HFinal = 0;

            Rectangle[] ixRect = new Rectangle[RectToBeAnalyzed];

            Bitmap bmp0 = (Bitmap)bmp.Clone();

            if (Name == "BASE-00091")
            {
                //Name = Name;
                //bmp.Save(@"D:\LOA\NEWERA\KK0.BMP", ImageFormat.Bmp);
                //bmp0.Save(@"D:\LOA\NEWERA\KK1.BMP", ImageFormat.Bmp);
            }


            FindObject.Find(bmp0, Color.Red);

            if (Name == "BASE-00091")
            {
                //Name = Name;
                //bmp0.Save(@"D:\LOA\NEWERA\KK2.BMP", ImageFormat.Bmp);
            }



            Rectangle MaxRect = FindObject.rectMaxRect;
            Rectangle MaxRect2Stage = FindObject.rectMaxRect;
            RectangleF MaxRect2StageF = (RectangleF)FindObject.rectMaxRect;

            if (IsFinding)
            {
                //Point PtAlterPoint = myCenter;
                Point PtAlterPoint = FoundCenter;

                PtAlterPoint.X -= (AlterRect.X - myrect.X);
                PtAlterPoint.Y -= (AlterRect.Y - myrect.Y);

                //if (Name == "BASE-0519")
                //{
                //JzTools.DrawRect(bmpThreshedComp, JzTools.SimpleRect(PtAlterPoint), new Pen(Color.Red));
                //bmpThreshedComp.Save(@"D:\LOA\NEWERA\CHECK.BMP", ImageFormat.Bmp);
                //}

                //JzTools.DrawRect(bmp, MaxRect, new Pen(Color.Red, 2));
                //bmp.Save(@"D:\LOA\NEWERA\CHECK.BMP", ImageFormat.Bmp);

                //MaxRect.Height = rectFound.Height;
                //MaxRect.Width = rectFound.Width << 3;
                MaxRect.Height = rectFound.Height;
                MaxRect.Width = rectFound.Width;

                //if (Name == "BASE-00091")
                //{
                //    Name = Name;
                //    //bmp0.Save(@"D:\LOA\NEWERA\KK2.BMP", ImageFormat.Bmp);
                //}

                //MaxRect = FindObject.GetRectNearestEX(PtAlterPoint, MaxRect, AnalyzeXRange, AnalyzeYMinRange, AnalyzeYMaxRange);
               // MaxRect = FindObject.GetRectNearestFX(PtAlterPoint, MaxRect, AnalyzeXRange, AnalyzeYMinRange, AnalyzeYMaxRange);

                if (INI.IS_CHECK_LEVEL)
                    MaxRect = FindObject.GetRectNearestFXAllinone(PtAlterPoint, MaxRect, AnalyzeXRange, AnalyzeYMinRange, AnalyzeYMaxRange);
                else
                    MaxRect = FindObject.GetRectNearestFX(PtAlterPoint, MaxRect, AnalyzeXRange, AnalyzeYMinRange, AnalyzeYMaxRange);


                Rectangle MaxTmpRect = MaxRect;

                //if (RESULT.IsUseFindingCheck)
                //{
                //    if (FindObject.RectList.Count > 1)
                //    {
                //        //Name = Name;
                //        //If there is something ambiguous
                //        ix = FindObject.RectList.Count - 1;

                //        List<string> MoreRectstrList = new List<string>();

                //        while (ix > -1)
                //        {
                //            Rectangle rectTmp = FindObject.RectList[ix];

                //            rectTmp.X += AlterRect.X;
                //            rectTmp.Y += AlterRect.Y;

                //            int GetSeq = CheckSequential(rectTmp, RESULT.bmpWorking);

                //            if (Sequence == GetSeq)
                //            {
                //                MoreRectstrList.Add(FindObject.RectList[ix].Y.ToString("0000") + "@" + JzTools.RecttoStringEX(FindObject.RectList[ix]));
                //                //MaxRect = FindObject.RectList[ix];
                //                //break;
                //            }
                //            ix--;
                //        }

                //        if (MoreRectstrList.Count == 1)
                //        {
                //            MaxRect = JzTools.StringtoRect(MoreRectstrList[0].Split('@')[1]);
                //        }
                //        else if (MoreRectstrList.Count > 1)
                //        {
                //            MoreRectstrList.Sort();
                //            MaxRect = JzTools.StringtoRect(MoreRectstrList[0].Split('@')[1]);
                //        }
                //    }
                //}

                //if (Name == "BASE-00091")
                //{
                //    Name = Name;
                //    bmp0.Save(@"D:\LOA\NEWERA\KK2.BMP", ImageFormat.Bmp);
                //}

                //if (!IsFromBase && !IsCalibration)
                //{
                //    if (MaxRect.Y > MaxTmpRect.Y)
                //    {
                //        MaxRect = MaxTmpRect;
                //    }
                //}

                MaxRect.Inflate(10, 10);
                MaxRect.Intersect(JzTools.SimpleRect(bmpPreFind.Size));

                ix = 0;

                while (ix < RectToBeAnalyzed)
                {
                    bmp0.Dispose();
                    bmp0 = (Bitmap)bmpPreFind.Clone(MaxRect, PixelFormat.Format32bppArgb);

                    //bmp0.Save(@"D:\LOA\TESTRESULT\ATP0.BMP", ImageFormat.Bmp);
                    bmpOrigionSized.Dispose();
                    bmpOrigionSized = new Bitmap(bmp0, JzTools.Resize(bmp0.Size, SizedRatio));

                    Histogram.GetHistogram(bmpOrigionSized);
                    Threshold.SetThreshold(bmp0, Histogram.MinGrade, (int)((double)(Histogram.MaxGrade - Histogram.MinGrade) * Ratio * RatioIndicator(ix)), Histogram.MinGrade);

                    //bmp0.Save(@"D:\LOA\TESTRESULT\ATP1.BMP", ImageFormat.Bmp);
                    FindObject.Find(bmp0, Color.Red);
                    //bmp0.Save(@"D:\LOA\TESTRESULT\ATP2.BMP", ImageFormat.Bmp);
                    MaxRect2Stage = FindObject.rectMaxRect;

                    //bmp0.Dispose();
                    //bmp0 = (Bitmap)bmpPreFind.Clone(MaxRect, PixelFormat.Format32bppArgb);

                    //MaxRect2Stage = Threshold.ClearStinRect(bmp0, MaxRect2Stage, Color.Red, 0.1);

                    ixRect[ix] = MaxRect2Stage;

                    ix++;
                }

                //PosFilter.Initial(0, 2000, 1);
                //HFilter.Initial(0, 2000, 1);


                ix = 0;
                while (ix < RectToBeAnalyzed)
                {
                    X += ixRect[ix].X;
                    Y += ixRect[ix].Y;
                    W += ixRect[ix].Width;
                    H += ixRect[ix].Height;

                    //PosFilter.Add(ixRect[ix].Y);
                    //HFilter.Add(ixRect[ix].Height);
                    ix++;
                }


                //PosFilter.Complete();
                //HFilter.Complete();



#if !OPT_MODEFILTER

                MaxRect2Stage.X = (int)JzTools.Round((double)X / (double)RectToBeAnalyzed);
                MaxRect2Stage.Y = (int)JzTools.Round((double)Y / (double)RectToBeAnalyzed);
                MaxRect2Stage.Width = (int)JzTools.Round((double)W / (double)RectToBeAnalyzed);
                MaxRect2Stage.Height = (int)JzTools.Round((double)H / (double)RectToBeAnalyzed);


                MaxRect2StageF.X = (float)((float)X / (float)RectToBeAnalyzed);
                MaxRect2StageF.Y = (float)((float)Y / (float)RectToBeAnalyzed);
                MaxRect2StageF.Width = (float)((float)W / (float)RectToBeAnalyzed);
                MaxRect2StageF.Height = (float)((float)H / (float)RectToBeAnalyzed);

#else
                if (Math.Abs(PosFilter.GetPorperty(1) - PosFilter.GetPorperty(2)) > 1)
                {
                    POSFinal = (double)PosFilter.GetPorperty(1);
                }
                else
                {
                    POSFinal = ((double)(PosFilter.GetPorperty(1) * PosFilter.GetPorpertyCount(1)) + (double)(PosFilter.GetPorperty(2) * PosFilter.GetPorpertyCount(2))) / (double)(PosFilter.GetPorpertyCount(1) + PosFilter.GetPorpertyCount(2));
                }

                if (Math.Abs(HFilter.GetPorperty(1) - HFilter.GetPorperty(2)) > 1)
                {
                    HFinal = (double)HFilter.GetPorperty(1);
                }
                else
                {
                    HFinal = ((double)(HFilter.GetPorperty(1) * HFilter.GetPorpertyCount(1)) + (double)(HFilter.GetPorperty(2) * HFilter.GetPorpertyCount(2))) / (double)(HFilter.GetPorpertyCount(1) + HFilter.GetPorpertyCount(2));
                }

                MaxRect2Stage.X = (int)JzTools.Round((double)X / (double)RectToBeAnalyzed);
                MaxRect2Stage.Y = (int)POSFinal;
                MaxRect2Stage.Width = (int)JzTools.Round((double)W / (double)RectToBeAnalyzed);
                MaxRect2Stage.Height = (int)HFinal;


                MaxRect2StageF.X = (float)((float)X / (float)RectToBeAnalyzed);
                MaxRect2StageF.Y = (float)POSFinal;
                MaxRect2StageF.Width = (float)((float)W / (float)RectToBeAnalyzed);
                MaxRect2StageF.Height = (float)HFinal;

#endif


                //bmpPreFind.Save(@"D:\LOA\TESTRESULT\ATPZ.BMP", ImageFormat.Bmp);

                //bmp0.Dispose();
                //bmp0 = (Bitmap)bmpPreFind.Clone(MaxRect, PixelFormat.Format32bppArgb);

                //bmp0.Save(@"D:\LOA\NEWERA\ATP0.BMP", ImageFormat.Bmp);

                //bmpOrigionSized.Dispose();
                //bmpOrigionSized = new Bitmap(bmp0, JzTools.Resize(bmp0.Size, SizedRatio));

                //Histogram.GetHistogram(bmpOrigionSized);
                //Threshold.SetThreshold(bmp0, Histogram.MinGrade, (int)((double)(Histogram.MaxGrade - Histogram.MinGrade) * Ratio), Histogram.MinGrade);
                ////Threshold.SetThreshold(bmp0, MinGrade, (int)((double)(MaxGrade - MinGrade) * Ratio), MinGrade);

                ////bmp0.Save(@"D:\LOA\TESTRESULT\ATP1.BMP", ImageFormat.Bmp);

                //FindObject.Find(bmp0, Color.Red);

                ////bmp0.Save(@"D:\LOA\TESTRESULT\ATP2.BMP", ImageFormat.Bmp);

                //MaxRect2Stage = FindObject.rectMaxRect;

                //bmp0.Dispose();
                //bmp0 = (Bitmap)bmpPreFind.Clone(MaxRect, PixelFormat.Format32bppArgb);

                //JzTools.DrawRect(bmp0, JzTools.SimpleRect(JzTools.GetRectCenter(MaxRect2Stage), 3), new SolidBrush(Color.FromArgb(Histogram.MaxGrade, Histogram.MaxGrade, Histogram.MaxGrade)));
                //FindObject.FindGrayscale(bmp0, JzTools.GetRectCenter(MaxRect2Stage), Color.Red, Histogram.MaxGrade, (int)((double)(Histogram.MaxGrade - Histogram.MinGrade) * MagicNumber2));

                ////bmp0.Save(@"D:\LOA\TESTRESULT\ATP3.BMP", ImageFormat.Bmp);
                //MaxRect2Stage = FindObject.rectMaxRect;

                ////清除毛剌

                ////bmp0.Save(@"D:\LOA\TESTRESULT\ATP3.BMP", ImageFormat.Bmp);

                //MaxRect2Stage = Threshold.ClearStinRect(bmp0, MaxRect2Stage, Color.Red, 0.1);

                //bmp0.Save(@"D:\LOA\TESTRESULT\ATP4.BMP", ImageFormat.Bmp);

                //JzTools.DrawRect(bmp0, MaxRect2Stage, new Pen(Color.Blue, 1));

                //bmp0.Save(@"D:\LOA\TESTRESULT\ATP5.BMP", ImageFormat.Bmp);

                if (!(MaxRect2Stage.X == 0 && MaxRect2Stage.Y == 0))
                {

                    MaxRect2Stage.X += MaxRect.X;
                    MaxRect2Stage.Y += MaxRect.Y;

                    MaxRect2StageF.X += MaxRect.X;
                    MaxRect2StageF.Y += MaxRect.Y;
                }

            }
            else
            {
                MaxRect.Inflate(10, 10);

                MaxRect.Intersect(JzTools.SimpleRect(bmpPreFind.Size));

                //bmpPreFind.Save(@"D:\LOA\TESTRESULT\ATPZ.BMP", ImageFormat.Bmp);
                ix = 0;

                while (ix < RectToBeAnalyzed)
                {
                    bmp0.Dispose();
                    bmp0 = (Bitmap)bmpPreFind.Clone(MaxRect, PixelFormat.Format32bppArgb);

                    //bmp0.Save(@"D:\LOA\TESTRESULT\ATP0.BMP", ImageFormat.Bmp);
                    bmpOrigionSized.Dispose();
                    bmpOrigionSized = new Bitmap(bmp0, JzTools.Resize(bmp0.Size, SizedRatio));

                    Histogram.GetHistogram(bmpOrigionSized);
                    Threshold.SetThreshold(bmp0, Histogram.MinGrade, (int)((double)(Histogram.MaxGrade - Histogram.MinGrade) * Ratio * RatioIndicator(ix)), Histogram.MinGrade);

                    //bmp0.Save(@"D:\LOA\TESTRESULT\ATP1.BMP", ImageFormat.Bmp);
                    FindObject.Find(bmp0, Color.Red);
                    //bmp0.Save(@"D:\LOA\TESTRESULT\ATP2.BMP", ImageFormat.Bmp);
                    MaxRect2Stage = FindObject.rectMaxRect;

                    //bmp0.Dispose();
                    //bmp0 = (Bitmap)bmpPreFind.Clone(MaxRect, PixelFormat.Format32bppArgb);

                    //MaxRect2Stage = Threshold.ClearStinRect(bmp0, MaxRect2Stage, Color.Red, 0.1);

                    ixRect[ix] = MaxRect2Stage;

                    ix++;
                }


                //PosFilter.Initial(0, 2000, 1);
                //HFilter.Initial(0, 2000, 1);

                ix = 0;
                while (ix < RectToBeAnalyzed)
                {
                    X += ixRect[ix].X;
                    Y += ixRect[ix].Y;
                    W += ixRect[ix].Width;
                    H += ixRect[ix].Height;

                    //PosFilter.Add(ixRect[ix].Y);
                    //HFilter.Add(ixRect[ix].Height);

                    ix++;
                }

                //PosFilter.Complete();
                //HFilter.Complete();

#if !OPT_MODEFILTER

                MaxRect2Stage.X = (int)JzTools.Round((double)X / (double)RectToBeAnalyzed);
                MaxRect2Stage.Y = (int)JzTools.Round((double)Y / (double)RectToBeAnalyzed);
                MaxRect2Stage.Width = (int)JzTools.Round((double)W / (double)RectToBeAnalyzed);
                MaxRect2Stage.Height = (int)JzTools.Round((double)H / (double)RectToBeAnalyzed);


                MaxRect2StageF.X = (float)((float)X / (float)RectToBeAnalyzed);
                MaxRect2StageF.Y = (float)((float)Y / (float)RectToBeAnalyzed);
                MaxRect2StageF.Width = (float)((float)W / (float)RectToBeAnalyzed);
                MaxRect2StageF.Height = (float)((float)H / (float)RectToBeAnalyzed);

#else

                if (Math.Abs(PosFilter.GetPorperty(1) - PosFilter.GetPorperty(2)) > 1)
                {
                    POSFinal = (double)PosFilter.GetPorperty(1);
                }
                else
                {
                    POSFinal = ((double)(PosFilter.GetPorperty(1) * PosFilter.GetPorpertyCount(1)) + (double)(PosFilter.GetPorperty(2) * PosFilter.GetPorpertyCount(2))) / (double)(PosFilter.GetPorpertyCount(1) + PosFilter.GetPorpertyCount(2));
                }

                if (Math.Abs(HFilter.GetPorperty(1) - HFilter.GetPorperty(2)) > 1)
                {
                    HFinal = (double)HFilter.GetPorperty(1);
                }
                else
                {
                    HFinal = ((double)(HFilter.GetPorperty(1) * HFilter.GetPorpertyCount(1)) + (double)(HFilter.GetPorperty(2) * HFilter.GetPorpertyCount(2))) / (double)(HFilter.GetPorpertyCount(1) + HFilter.GetPorpertyCount(2));
                }


                MaxRect2Stage.X = (int)JzTools.Round((double)X / (double)RectToBeAnalyzed);
                MaxRect2Stage.Y = (int)POSFinal;
                MaxRect2Stage.Width = (int)JzTools.Round((double)W / (double)RectToBeAnalyzed);
                MaxRect2Stage.Height = (int)HFinal;


                MaxRect2StageF.X = (float)((float)X / (float)RectToBeAnalyzed);
                MaxRect2StageF.Y = (float)POSFinal;
                MaxRect2StageF.Width = (float)((float)W / (float)RectToBeAnalyzed);
                MaxRect2StageF.Height = (float)HFinal;

#endif

                //JzTools.DrawRect(bmp0, JzTools.SimpleRect(JzTools.GetRectCenter(MaxRect2Stage), 3), new SolidBrush(Color.FromArgb(Histogram.MaxGrade, Histogram.MaxGrade, Histogram.MaxGrade)));
                //FindObject.FindGrayscale(bmp0, JzTools.GetRectCenter(MaxRect2Stage), Color.Red, Histogram.MaxGrade, (int)((double)(Histogram.MaxGrade - Histogram.MinGrade) * Ratio));

                ////bmp0.Save(@"D:\LOA\TESTRESULT\ATP3.BMP", ImageFormat.Bmp);
                //MaxRect2Stage = FindObject.rectMaxRect;

                //MaxRect.X += MaxRect2Stage.X;
                //MaxRect.Y += MaxRect2Stage.Y;

                if (!(MaxRect2Stage.X == 0 && MaxRect2Stage.Y == 0))
                {
                    MaxRect2Stage.X += MaxRect.X;
                    MaxRect2Stage.Y += MaxRect.Y;

                    MaxRect2StageF.X += MaxRect.X;
                    MaxRect2StageF.Y += MaxRect.Y;
                }
            }

            //Rectangle recttmp= JzTools.GetVerticalRect(FindObject.rectMaxRect, -3);

            Rectangle rectOrg = MaxRect2Stage;

            retRectF = MaxRect2StageF;
            //rectOrg.Inflate(0, -1);
            //取得直的中心寬度

            //rectOrg.Width = Math.Max(1, rectOrg.Width);
            //rectOrg.Height = Math.Max(1, rectOrg.Height);


            //if (rectOrg != new Rectangle())
            //{
            //    bmp0.Dispose();
            //    bmp0 = (Bitmap)bmp.Clone(rectOrg, PixelFormat.Format32bppArgb);

            //    ////bmp0.Save(@"D:\LOA\BASECENTER\002.BMP", ImageFormat.Bmp);
            //    ////Threshold.SetThreshold(bmp0, MinGrade, (int)((double)(MeaneGrade - MinGrade) * Ratio), MinGrade);
            //    //if (IsCalibration)
            //    //{
            //    //    bmp0.Dispose();
            //    //    bmp0 = (Bitmap)bmp.Clone(rectOrg, PixelFormat.Format32bppArgb);
            //    //}
            //    //else
            //    //{
            //    //    bmp0.Dispose();
            //    //    bmp0 = (Bitmap)bmpPreFind.Clone(rectOrg, PixelFormat.Format32bppArgb);

            //    //    //bmp0.Save(@"D:\LOA\BASECENTER\002.BMP", ImageFormat.Bmp);

            //    //    Histogram.GetHistogram(bmp0);
            //    //    Threshold.SetThreshold(bmp0, MinGrade, (int)((double)(MeanGrade - MinGrade) * Ratio), MinGrade);
            //    //}

            //    FindObject.Find(bmp0, Color.Red);
            //    //bmp0.Save(@"D:\LOA\BASECENTER\003.BMP", ImageFormat.Bmp);

            //    Rectangle rect = FindObject.rectMaxRect;

            //    rect.X = rectOrg.X;
            //    rect.Y += rectOrg.Y;

            //    rect.Width = rectOrg.Width;

            //    rectOrg = rect;
            //    //retPoint = JzTools.GetRectCenter(rect);
            //}

            bmp0.Dispose();

            return rectOrg;
        }

        public bool IsRealted(KeybaseClass keybase)
        {
            bool ret = false;

            foreach (CornerDefineClass cdf in CornerDefinedList)
            {
                foreach (CornerDefineClass cdf1 in keybase.CornerDefinedList)
                {
                    if (cdf.ToFormedString() == cdf1.ToFormedString())
                    {
                        ret = true;
                        break;
                    }
                }

                if (ret)
                    break;
            }

            return ret;
        }
        public void AssignControls()
        {
            switch (mySide)
            {
                case SideEnum.SIDE0:
                case SideEnum.SIDE4:
                    myPen.Dispose();
                    myPen = new Pen(Color.Lime, 2);
                    myBrush = Brushes.Lime;
                    break;
                case SideEnum.SIDE1:
                case SideEnum.SIDE5:
                    myPen.Dispose();
                    myPen = new Pen(Color.Cyan, 2);
                    myBrush = Brushes.Cyan;
                    break;
                case SideEnum.SIDE2:
                case SideEnum.SIDE6:
                    myPen.Dispose();
                    myPen = new Pen(Color.LightSalmon, 2);
                    myBrush = Brushes.LightSalmon;
                    break;
                case SideEnum.SIDE3:
                    myPen.Dispose();
                    myPen = new Pen(Color.Violet, 2);
                    myBrush = Brushes.Violet;
                    break;
            }
        }
        public override string ToString()
        {
            string Str = "";
            string Str1 = "";

            Str = Name + Separator
                + JzTools.RecttoString(myrect) + Separator
                + Contrast.ToString() + Separator
                + Resolution.ToString() + Separator
                + (IsCalibration ? "1" : "0") + Separator
                + (IsFromBase ? "1" : "0") + Separator;

            Str1 = "";

            foreach (CornerDefineClass CornerDefine in CornerDefinedList)
            {
                Str1 += CornerDefine.ToString() + SeparatorInside;
            }

            Str += JzTools.RemoveLastChar(Str1, 1) + Separator
                + JzTools.PointtoString(FoundAnalyzeCenter) + Separator
                + (IsAsPlane  ? "1" : "0") + Separator
                + XPos.ToString() + Separator
                + YPos.ToString() + Separator
                + (IsAutoLocation ? "1" : "0") + Separator
                + (IsSpaceFlat ? "1" : "0") + Separator
                + FlatIndex.ToString() + Separator
                + AddHeight.ToString() + Separator
                + Ymin.ToString() + Separator
                + Range.ToString() + Separator 
                +ResolutionRange.ToString();


            return Str;
        }
        
        public string ToSeqString()
        {
            string Str = "";

            Str += myrect.Y.ToString("0000") + ",";
            Str += Name + ",";
            Str += JzTools.RecttoString(myrect);

            return Str;
        }

        public void Dispoe()
        {
            myPen.Dispose();
            myBrush.Dispose();

            bmpOrigion.Dispose();
            bmpOrigionSized.Dispose();
            bmpThreshed.Dispose();
            bmpProcessed.Dispose();

            bmpOrigionComp.Dispose();
            bmpThreshedComp.Dispose();
            bmpProcessedComp.Dispose();
        }

        Point StringtoPoint(string PtString)
        {
            string[] str = PtString.Split(',');
            return new Point(int.Parse(str[0]), int.Parse(str[1]));
        }
        private void BonudRect(ref Rectangle InnerRect, Size BoundSize)
        {
            InnerRect.X = Math.Min(Math.Max(InnerRect.X, 0), (BoundSize.Width - InnerRect.Width < 0 ? 0 : BoundSize.Width - InnerRect.Width));
            InnerRect.Y = Math.Min(Math.Max(InnerRect.Y, 0), (BoundSize.Height - InnerRect.Height < 0 ? 0 : BoundSize.Height - InnerRect.Height));

            if (BoundSize.Width <= InnerRect.X + InnerRect.Width)
                InnerRect.Width = _BoundValue(InnerRect.Width, BoundSize.Width - InnerRect.X, 1);
            if (BoundSize.Height <= InnerRect.Height + InnerRect.Height)
                InnerRect.Height = _BoundValue(InnerRect.Height, BoundSize.Height - InnerRect.Y, 1);
        }
        private int _BoundValue(int Value, int Max, int Min)
        {
            return Math.Max(Math.Min(Value, Max), Min);

        }
    }
}
