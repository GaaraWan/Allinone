using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

using JetEazy.BasicSpace;
//using Jumbo301.UniversalSpace;
using JetEazy;

namespace JzKHC.ControlSpace
{
    class OPScreenUIClass
    {
        //Control constants
        protected const int ExtendOutLineLength = 40;
        protected const int MovingDelay = 30;
        
        //Interface
        protected OPScreenUIControl OPScreenUI;

        protected bool IsDebug
        {
            get
            {
                return false;
            }
        }
        protected bool IsSlow
        {
            get
            {
                return false;
            }
        }

        //OPScreen Property
        public OPTypeEnum OPType = OPTypeEnum.NONE;
        public CornerEnum OPCorner = CornerEnum.NONE;
        public int OPIndex = -1;

        public bool IsOperating = false;

        protected Point PtStart = new Point();

        protected int MaxRatio = 2;
        protected int MinRatio = -3;

        public int VirtualRatio = 0; //將大圖轉換為小圖的參數

        protected Point PtLive = new Point();
        public Bitmap bmpBareOrigion = new Bitmap(1, 1);
        protected Bitmap bmpOperate = new Bitmap(1, 1);
        protected Rectangle rectOriginViewRange = new Rectangle();
        
        Bitmap bmpViewPort = new Bitmap(1, 1);
        protected Bitmap bmpViewPortCorp = new Bitmap(1, 1);
        protected Bitmap bmpViewPortCorpZoom = new Bitmap(1, 1);

        protected Bitmap bmpTmp = new Bitmap(1, 1);
        protected JzToolsClass JzTools = new JzToolsClass();

        protected Point ptViewPortLocation = new Point();
        protected Point ptViewPortLocationWithVirtialRatio
        {
            get
            {
                return new Point(JzTools.ShiftValue((int)ptViewPortLocation.X, -VirtualRatio), JzTools.ShiftValue((int)ptViewPortLocation.Y, -VirtualRatio));
            }
        }
        
        protected Point ptViewPortLocationLive = new Point();
        protected Point ptViewPortLocationLiveWithVirtialRatio
        {
            get
            {
                return new Point(JzTools.ShiftValue((int)ptViewPortLocationLive.X, -VirtualRatio), JzTools.ShiftValue((int)ptViewPortLocationLive.Y, -VirtualRatio));
            }
        }

        protected Point ptStartLocationIndicator
        {
            get
            {
                Size Sz = new Size(PtStart.X, PtStart.Y);
                Sz = JzTools.Resize(Sz, -iViewPortRatio);

                Point Pt = Point.Add(ptViewPortLocation, Sz);

                Pt.X = Math.Min(Math.Max(0, Pt.X), szOrigin.Width - 1);
                Pt.Y = Math.Min(Math.Max(0, Pt.Y), szOrigin.Height - 1);

                return Pt;

            }
        }
        protected Point ptStartLocationIndicatorWithVirtualRatio
        {
            get
            {
                Size Sz = new Size(PtStart.X, PtStart.Y);
                Sz = JzTools.Resize(Sz, -iViewPortRatio);

                Point Pt = Point.Add(ptViewPortLocationWithVirtialRatio, Sz);

                Pt.X = Math.Min(Math.Max(0, Pt.X), JzTools.ShiftValue(szOrigin.Width, -VirtualRatio) - 1);
                Pt.Y = Math.Min(Math.Max(0, Pt.Y), JzTools.ShiftValue(szOrigin.Height, -VirtualRatio) - 1);

                return Pt;

            }
        }
        protected Point ptLiveLocationIndicator
        {
            get
            {
                Size Sz = new Size(PtLive.X, PtLive.Y);
                Sz = JzTools.Resize(Sz, -iViewPortRatio);

                Point Pt = Point.Add(ptViewPortLocation, Sz);

                Pt.X = Math.Min(Math.Max(0, Pt.X), szOrigin.Width - 1);
                Pt.Y = Math.Min(Math.Max(0, Pt.Y), szOrigin.Height - 1);

                return Pt;

            }
        }
        protected Point ptLiveLocationIndicatorWithVirtualRatio
        {
            get
            {
                Size Sz = new Size(PtLive.X, PtLive.Y);
                Sz = JzTools.Resize(Sz, -iViewPortRatio);

                Point Pt = Point.Add(ptViewPortLocationWithVirtialRatio, Sz);

                Pt.X = Math.Min(Math.Max(0, Pt.X), JzTools.ShiftValue(szOrigin.Width, -VirtualRatio) - 1);
                Pt.Y = Math.Min(Math.Max(0, Pt.Y), JzTools.ShiftValue(szOrigin.Height, -VirtualRatio) - 1);

                return Pt;

            }
        }

        protected int iDefaultViewPortRatio = 0;
        protected int iViewPortRatio = 0;
        protected float ViewPortRatio
        {
            get
            {
                float retfloat = 0;
                if (iViewPortRatio > 0)
                    retfloat = 1f * (1 << (iViewPortRatio - VirtualRatio));
                else
                    retfloat = 1f / (1 << -(iViewPortRatio - VirtualRatio));

                return retfloat;
            }
        }

        protected Size szOrigin = new Size();
        protected Size szViewPort = new Size();
        protected Size szViewPortInsideMove = new Size();
        protected Size szViewPortLocationMove
        {
            get
            {
                Size szTmp = new Size();

                szTmp.Width = JzTools.ShiftValue(szViewPortInsideMove.Width, -(iViewPortRatio -VirtualRatio));
                szTmp.Height = JzTools.ShiftValue(szViewPortInsideMove.Height, -(iViewPortRatio - VirtualRatio));

                return szTmp;
            }
        }
        protected Size szViewPortCorpSize
        {
            get
            {
                Size szTmp = new Size();

                szTmp.Width = JzTools.ShiftValue(szViewPort.Width + ExtendOutLineLength, -(iViewPortRatio - VirtualRatio));
                szTmp.Height = JzTools.ShiftValue(szViewPort.Height + ExtendOutLineLength, -(iViewPortRatio - VirtualRatio));

                return szTmp;
            }


        }

        //protected TimerClass tmMovingDelay = new TimerClass();
        protected JzTimes tmMovingDelay = new JzTimes();

        public OPScreenUIClass(OPScreenUIControl rOPScreenUI,int DefaultRatio,int rMinRatio,int rMaxRatio)
        {
            OPScreenUI = rOPScreenUI;
            iViewPortRatio = DefaultRatio;
            iDefaultViewPortRatio = DefaultRatio;

            MinRatio = rMinRatio;
            MaxRatio = rMaxRatio;

            Initial();
        }

        protected void Initial()
        {
            //Bitmap bmp = new Bitmap(@"D:\TEST\KR.BMP");
            szOrigin = bmpBareOrigion.Size;
            szViewPort = OPScreenUI.picOperationSize;

            OPScreenUI.picOperation.MouseMove += new MouseEventHandler(picOperation_MouseMove);
            OPScreenUI.picOperation.MouseDown += new MouseEventHandler(picOperation_MouseDown);
            OPScreenUI.picOperation.MouseDoubleClick += new MouseEventHandler(picOperation_MouseDoubleClick);
            OPScreenUI.picOperation.MouseUp += new MouseEventHandler(picOperation_MouseUp);
            OPScreenUI.picOperation.MouseWheel += new MouseEventHandler(picOperation_MouseWheel);
            OPScreenUI.picOperation.MouseEnter += new EventHandler(picOperation_MouseEnter);

            JzTools.ClearPoint(ref ptViewPortLocation);
            JzTools.ClearPoint(ref ptViewPortLocationLive);

            //CorpImageInViewPort(ref ptViewPortLocationLive);
            //bmp.Dispose();
        }

        public void SetbmpOrigion(Bitmap bmp)
        {
            bmpBareOrigion.Dispose();
            bmpBareOrigion = (Bitmap) bmp.Clone();

            bmpOperate.Dispose();
            bmpOperate = (Bitmap)bmp.Clone();

            szOrigin = bmpBareOrigion.Size;
        }

        protected virtual void CorpImageInViewPort(ref Point ViewPortLocation)
        {
            Size szTmp = szViewPortLocationMove;

            ViewPortLocation.X = JzTools.BoundValue(ViewPortLocation.X, szOrigin.Width - szTmp.Width - ExtendOutLineLength, 0);
            ViewPortLocation.Y = JzTools.BoundValue(ViewPortLocation.Y, szOrigin.Height - szTmp.Height - ExtendOutLineLength, 0);

            rectOriginViewRange = new Rectangle(ViewPortLocation, szViewPortCorpSize);

            BonudRect(ref rectOriginViewRange, szOrigin);

            ViewPortLocation = rectOriginViewRange.Location;

            bmpViewPortCorp.Dispose();
            bmpViewPortCorp = (Bitmap)bmpOperate.Clone(rectOriginViewRange, PixelFormat.Format32bppArgb);

            bmpViewPortCorpZoom.Dispose();
            bmpViewPortCorpZoom = new Bitmap(bmpViewPortCorp, JzTools.Resize(rectOriginViewRange.Size, (iViewPortRatio - VirtualRatio)));

            OPScreenUI.SetPicture(bmpViewPortCorpZoom);
        }

        protected virtual void picOperation_MouseEnter(object sender, EventArgs e)
        {
            OPScreenUI.picOperation.Focus();
        }
        protected virtual void picOperation_MouseWheel(object sender, MouseEventArgs e)
        {
            int Degree = -e.Delta / 120;

            if (tmMovingDelay.msDuriation > MovingDelay)
            {
                int LatestViewPortRatio = iViewPortRatio;
                int NewViewPortRatio = JzTools.BoundValue(iViewPortRatio + Degree, MaxRatio, MinRatio);

                JzTools.ClearPoint(ref PtStart);

                PtLive.X = JzTools.ShiftValue(e.X, -(NewViewPortRatio - LatestViewPortRatio)) - e.X;
                PtLive.Y = JzTools.ShiftValue(e.Y, -(NewViewPortRatio - LatestViewPortRatio)) - e.Y;

                JzTools.SetSize(ref szViewPortInsideMove, PtStart, PtLive);

                ptViewPortLocationLive = Point.Add(ptViewPortLocation, szViewPortLocationMove);

                iViewPortRatio = NewViewPortRatio;

                CorpImageInViewPort(ref ptViewPortLocationLive);

                ptViewPortLocation = ptViewPortLocationLive;

                SetInformation(e);

                OnZooming();

                tmMovingDelay.Cut();
            }
        }
        protected virtual void picOperation_MouseUp(object sender, MouseEventArgs e)
        {
            IsOperating = false;
            ptViewPortLocation = ptViewPortLocationLive;
            JzTools.ClearSize(ref szViewPortInsideMove);
            //OPType = OPTypeEnum.NONE;

            OnZooming();
        }
        protected virtual void picOperation_MouseDown(object sender, MouseEventArgs e)
        {
            IsOperating = true;
            JzTools.SetPoint(ref PtStart, e);
            //tmMovingDelay.Cut();
        }
        protected virtual void picOperation_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Right)
            {
                PtStart.X = JzTools.ShiftValue(szViewPort.Width, -1);
                PtStart.Y = JzTools.ShiftValue(szViewPort.Height, -1);

                PtLive.X = e.X;
                PtLive.Y = e.Y;

                JzTools.SetSize(ref szViewPortInsideMove, PtLive, PtStart);
                ptViewPortLocationLive = Point.Add(ptViewPortLocation, szViewPortLocationMove);

                CorpImageInViewPort(ref ptViewPortLocationLive);
                ptViewPortLocation = ptViewPortLocationLive;
            }
            if (e.Button == MouseButtons.Left)
            {
                if (OPType == OPTypeEnum.NONE)
                {
                    JzTools.ClearPoint(ref ptViewPortLocationLive);
                    iViewPortRatio = iDefaultViewPortRatio;

                    CorpImageInViewPort(ref ptViewPortLocationLive);

                    ptViewPortLocation = ptViewPortLocationLive;
                }
            }
            SetInformation(e, ptViewPortLocationLiveWithVirtialRatio);

        }
        protected virtual void picOperation_MouseMove(object sender, MouseEventArgs e)
        {
            JzTools.SetPoint(ref PtLive, e);

            if (e.Button == MouseButtons.Right)
            {
                if (tmMovingDelay.msDuriation > MovingDelay)
                {
                    JzTools.SetSize(ref szViewPortInsideMove, PtStart, PtLive);
                    ptViewPortLocationLive = Point.Add(ptViewPortLocation, szViewPortLocationMove);

                    CorpImageInViewPort(ref ptViewPortLocationLive);

                    SetInformation(e, ptViewPortLocationLiveWithVirtialRatio);

                    tmMovingDelay.Cut();
                }
            }
            else
                SetInformation(e, ptViewPortLocationWithVirtialRatio);
        }


        protected void SetInformation(MouseEventArgs e)
        {
            SetInformation(e, ptViewPortLocationLiveWithVirtialRatio);
        }
        protected virtual void SetInformation(MouseEventArgs e,Point ptLocation)
        {
            Color COLOR = bmpBareOrigion.GetPixel(
                JzTools.ShiftValue(ptLiveLocationIndicatorWithVirtualRatio.X, 0),
                JzTools.ShiftValue(ptLiveLocationIndicatorWithVirtualRatio.Y, 0));

            OPScreenUI.SetInformation(" 內部座標:" + JzTools.PointtoString(e) + " 景觀座標:" + JzTools.PointtoString(ptLocation) + " " +
                "原始座標:" + JzTools.PointtoString(ptLiveLocationIndicatorWithVirtualRatio) + " " +
                "景觀尺吋:" + JzTools.SizetoString(rectOriginViewRange.Size) + " " +
                "比例:" + ViewPortRatio.ToString() + 
                JzTools.ColorInformation(COLOR) + "");
        }

        public bool Enabled
        {
            set
            {
                OPScreenUI.Enabled = value;

                if (!value)
                {
                    SetInformationDirectly("");
                }
            }
        }
        public void SetBMPDirectly(Bitmap bmp)
        {
            if (bmp == null)
            {
                bmpBareOrigion.Dispose();
                bmpBareOrigion = new Bitmap(1, 1);

                bmpOperate.Dispose();
                bmpOperate = new Bitmap(1, 1);
            }
            else
            {
                bmpBareOrigion.Dispose();
                bmpBareOrigion = (Bitmap)bmp.Clone();

                bmpOperate.Dispose();
                bmpOperate = (Bitmap)bmp.Clone();
            }
            szOrigin = bmpBareOrigion.Size;
            szViewPort = OPScreenUI.picOperationSize;

            Alignment();
        }
        public void Alignment()
        {
            iViewPortRatio = iDefaultViewPortRatio;

            JzTools.ClearPoint(ref ptViewPortLocation);
            JzTools.ClearPoint(ref ptViewPortLocationLive);

            CorpImageInViewPort(ref ptViewPortLocationLive);

            SetInformationDirectly("");
        }
        public void SetInformationDirectly(string Str)
        {
            OPScreenUI.SetInformation(Str);
        }

        Font TipFont = new Font("Arial Black", 58);
        Brush TipBrush = Brushes.Red;
        Point TipLocation = new Point(10, 10);

        public void SetTip(Bitmap bmp, string Str)
        {
            SetTip(bmp, Str, 58);
        }
        public void SetTip(Bitmap bmp, string Str,float FontSize)
        {
            SetTip(bmp, Str, FontSize, Brushes.Red);
        }
        public void SetTip(Bitmap bmp, string Str, float FontSize,Brush Brsh)
        {
            TipFont.Dispose();
            TipFont = new Font("Arial Black", FontSize);

            TipBrush = Brsh;

            Graphics g = Graphics.FromImage(bmp);
            g.DrawString(Str, TipFont, TipBrush, TipLocation);
            g.Dispose();
        }

        public void Focus()
        {
            OPScreenUI.picOperation.Focus();
        }
        public void ChangeViewPortLocation(int DiffRatio)
        {
            ptViewPortLocation.X = JzTools.ShiftValue(ptViewPortLocation.X, DiffRatio);
            ptViewPortLocation.Y = JzTools.ShiftValue(ptViewPortLocation.Y, DiffRatio);

            ptViewPortLocationLive.X = JzTools.ShiftValue(ptViewPortLocationLive.X, DiffRatio);
            ptViewPortLocationLive.Y = JzTools.ShiftValue(ptViewPortLocationLive.Y, DiffRatio);
        }
        public virtual void Dispose()
        {
            OPScreenUI.DisposeBMP();

            bmpBareOrigion.Dispose();
            bmpOperate.Dispose();
            bmpTmp.Dispose();
            bmpViewPort.Dispose();
            bmpViewPortCorp.Dispose();
            bmpViewPortCorpZoom.Dispose();
        }
        public delegate void ZoomingHandler();
        public event ZoomingHandler ZoomingAction;
        public void OnZooming()
        {
            if (ZoomingAction != null)
            {
                ZoomingAction();
            }
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
