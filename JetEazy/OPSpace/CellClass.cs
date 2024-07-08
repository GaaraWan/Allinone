using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using JetEazy.BasicSpace;

namespace JetEazy.ControlSpace
{
    public enum CellPropertyEnum : int
    {
        COUNT = 2,

        DYNAMIC = 0,
        STATIC =1,
    }
    public enum CelltypeEnum : int
    {
        COUNT = 4,

        RECT = 0,
        CIRCLE = 1,
        POLY = 2,
        AFFECT = 3,
    }

    public class CircleClass
    {
        public PointF PtCenter = new PointF();
        public float Radius = 0;

        public float Area
        {
            get
            {
                return (float)(Math.PI * Math.Pow(Radius, 2));
            }
        }
        public int RowTag = 0;

        public CircleClass()
        {

        }
        public CircleClass(string str)
        {
            FromString(str);
        }

        public CircleClass(PointF ptf, float width)
        {
            PtCenter = ptf;
            Radius = width;
        }
        public string ToYString()
        {
            string Str = PtCenter.Y.ToString("000000.00") + "," + PtCenter.X.ToString("000000.00");

            return Str;
        }
        public string ToXString()
        {
            string Str = PtCenter.X.ToString("000000.00") + "," + PtCenter.Y.ToString("000000.00");

            return Str;
        }
        public Rectangle ToRect()
        {
            RectangleF rectf = new RectangleF(PtCenter.X - Radius, PtCenter.Y - Radius, 2 * Radius, 2 * Radius);
            return RectFToRect(rectf);
        }
        public override string ToString()
        {
            string Str = "";

            Str += PointFToString(PtCenter) + "$";
            Str += Radius.ToString();

            return Str;
        }
        public void FromString(string str)
        {
            string[] strs = str.Split('$');

            if (strs.Length > 1)
            {
                PtCenter = StringToPointF(strs[0]);
                Radius = float.Parse(strs[1]);
            }
        }

        #region Functions

        public Rectangle RectFToRect(RectangleF RectF)
        {
            Rectangle rect = new Rectangle((int)RectF.X, (int)RectF.Y, (int)RectF.Width, (int)RectF.Height);

            return rect;
        }
        public string PointFToString(PointF PTF)
        {
            return PTF.X.ToString() + "," + PTF.Y.ToString();
        }
        public PointF StringToPointF(string Str)
        {
            string[] strs = Str.Split(',');
            return new PointF(float.Parse(strs[0]), float.Parse(strs[1]));
        }

        #endregion
    }

    public class CellClass
    {
        const char Separator = '@';

        const bool IsInsidePointSelected = false;
        const int BorderPixel = 1;
        const int BorderCatchPixel = 7;
        const int CornerPixel = 3;
        const int CornerCatchPixel = 9;

        public Pen PFirstSelect = new Pen(Color.Red, BorderPixel);
        public Pen PSelect = new Pen(Color.Tomato, BorderPixel);
        public Pen PNormal = new Pen(Color.Lime, BorderPixel);
        public Pen PBorder = new Pen(Color.Black, BorderPixel);
        public Pen PDatum = new Pen(Color.SkyBlue, 8);

        public SolidBrush SFirstSelect = new SolidBrush(Color.Red);
        public SolidBrush SSelect = new SolidBrush(Color.Tomato);
        public SolidBrush SNormal = new SolidBrush(Color.Lime);
        public SolidBrush SBorder = new SolidBrush(Color.Black);

       
        public Pen POperate
        {
            get
            {
                Pen retPen = PNormal;

                if (IsFirstSelected)
                {
                    retPen = PFirstSelect;
                }
                else if (IsSelected)
                {
                    retPen = PSelect;
                }
                return retPen;
            }
        }
        public SolidBrush SOperate
        {
            get
            {
                SolidBrush retSolid = SNormal;

                if (IsFirstSelected)
                {
                    retSolid = SFirstSelect;
                }
                else if (IsSelected)
                {
                    retSolid = SSelect;
                }
                return retSolid;
            }
        }

        public string CellGroup = "";

        public bool IsFirstSelected = false;
        public bool IsSelected = false;

        public string RelateIndexName = "";

        public CelltypeEnum CellType = CelltypeEnum.RECT;

        CellPropertyEnum myCellProperty = CellPropertyEnum.DYNAMIC;
        public CellPropertyEnum CellProperty
        {
            get
            {
                return myCellProperty;
            }
            set
            {
                myCellProperty = value;

                switch (myCellProperty)
                {
                    case CellPropertyEnum.DYNAMIC:
                        PFirstSelect = new Pen(Color.Red, BorderPixel);

                        PSelect = new Pen(Color.Tomato, BorderPixel);
                        PNormal = new Pen(Color.Lime, BorderPixel);
                        PBorder = new Pen(Color.Black, BorderPixel);

                        SFirstSelect = new SolidBrush(Color.Red);
                        SSelect = new SolidBrush(Color.Tomato);
                        SNormal = new SolidBrush(Color.Lime);
                        SBorder = new SolidBrush(Color.Black);
                        break;
                    case CellPropertyEnum.STATIC:
                        PFirstSelect = new Pen(Color.Red, BorderPixel);
                        PSelect = new Pen(Color.Blue, BorderPixel);
                        PNormal = new Pen(Color.Yellow, BorderPixel);
                        PBorder = new Pen(Color.Black, BorderPixel);

                        SFirstSelect = new SolidBrush(Color.Red);
                        SSelect = new SolidBrush(Color.Blue);
                        SNormal = new SolidBrush(Color.Yellow);
                        SBorder = new SolidBrush(Color.Black);
                        break;
                }

            }
        }

        public Rectangle RectBoundary = new Rectangle();
        
        public Rectangle RectCellBound
        {
            get
            {
                return RectCell;
            }
        }

        Rectangle RectCellInsideBorder
        {
            get
            {
                Rectangle rect = RectCell;
                rect.Inflate(-BorderCatchPixel, -BorderCatchPixel);

                return rect;
            }
        }
        Rectangle RectCellOutSideBorder
        {
            get
            {
                Rectangle rect = RectCell;
                rect.Inflate(BorderCatchPixel, BorderCatchPixel);

                return rect;
            }
        }

        public Rectangle RectCell = new Rectangle(100, 100, 100, 100);
        public Rectangle RectBack = new Rectangle();

        public CircleClass CircleCell = new CircleClass();

        public CornerEnum CornerCatched = CornerEnum.NONE;

        public CellClass()
        {


        }
        public CellClass(string Str)
        {
            FromString(Str);
        }

        public void SetFirstSelected()
        {
            IsFirstSelected = true;
            IsSelected = true;
        }
        public void SetSelected()
        {
            IsFirstSelected = false;
            IsSelected = true;
        }
        public void SetNoSelected()
        {
            IsFirstSelected = false;
            IsSelected = false;
        }
        public void TransferSelected(CellClass cell)
        {
            IsFirstSelected = cell.IsFirstSelected;
            IsSelected = cell.IsSelected;
            RectBoundary = cell.RectBoundary;

            cell.SetNoSelected();
        }

        public void MoveNew()
        {
            Move(new Size(50, 50));
        }

        public CellClass Clone(string Str)
        {
            CellClass retCell = new CellClass(Str);

            retCell.Move(new Size(20, 20));

            return retCell;
        }

        public CellClass Clone()
        {
            CellClass retCell = new CellClass();
            retCell.FromString(this.ToString());

            return retCell;
        }

        public bool IsIntersected(Rectangle rect)
        {
            return RectCell.IntersectsWith(rect);
        }

        public Rectangle RectCorner(CornerEnum corner)
        {
            return RectCorner(RectCell, corner);
        }
        public Rectangle RectCorner(CornerEnum corner, int catchpixel)
        {
            return RectCorner(RectCell, corner, catchpixel);
        }
        public Rectangle RectCorner(Rectangle rect, CornerEnum corner)
        {
            return RectCorner(rect, corner, CornerPixel);
        }
        public Rectangle RectCorner(Rectangle rect,CornerEnum corner, int catchpixel)
        {
            Rectangle retRect = new Rectangle();

            switch (corner)
            {
                case CornerEnum.LB:
                    retRect = SimpleRect(new Point(rect.Left, rect.Bottom), catchpixel, catchpixel);
                    break;
                case CornerEnum.RT:
                    retRect = SimpleRect(new Point(rect.Right, rect.Top), catchpixel, catchpixel);
                    break;
                case CornerEnum.RB:
                    retRect = SimpleRect(new Point(rect.Right, rect.Bottom), catchpixel, catchpixel);
                    break;
                case CornerEnum.LT:
                default:
                    retRect = SimpleRect(rect.Location, catchpixel, catchpixel);
                    break;
            }
            return retRect;
        }

        public CornerEnum CatchCorner(Point pt)
        {
            CornerCatched = CornerEnum.NONE;

            if (CellProperty == CellPropertyEnum.STATIC)
                return CornerCatched;

            Rectangle rectpt = SimpleRect(pt);

            int i = 0;

            while (i < (int)CornerEnum.COUNT)
            {
                if (RectCorner((CornerEnum)i, CornerCatchPixel).IntersectsWith(rectpt))
                {
                    CornerCatched = (CornerEnum)i;
                    break;
                }
                i++;
            }

            return CornerCatched;
        }
        public bool IsCatchBorder(Point pt)
        {
            bool ret = false;
            Rectangle rectpt = SimpleRect(pt);

            if (IsInsidePointSelected)
                ret = RectCellOutSideBorder.IntersectsWith(rectpt);
            else
                ret = !RectCellInsideBorder.IntersectsWith(rectpt) && RectCellOutSideBorder.IntersectsWith(rectpt);

            return ret;

        }

        public void Backup()
        {
            RectBack = RectCell;
        }
        public void Restore()
        {
            RectCell = RectBack;
        }

        public void Move(Size sz)
        {
            Move(sz, false);
        }
        public void Move(Size sz, bool IsNeedRestore)
        {
            if (IsNeedRestore)
                Restore();

            if (CellProperty == CellPropertyEnum.STATIC)
                return;

            RectCell.Offset(sz.Width, sz.Height);

            BoundRect(ref RectCell, RectBoundary.Size);
        }

        public void ReSize(Size sz)
        {
            ReSize(sz, false, CornerCatched);
        }
        public void ReSize(Size sz,bool IsNeedRestore,CornerEnum cornerchached)
        {
            if (IsNeedRestore)
                Restore();

            if (CellProperty == CellPropertyEnum.STATIC)
                return;

            int Width = sz.Width;
            int Height = sz.Height;

            switch (cornerchached)
            {
                case CornerEnum.RB:
                    RectCell.Width = Math.Max(10, RectCell.Width + Width);
                    RectCell.Height = Math.Max(10, RectCell.Height + Height);
                    break;
                case CornerEnum.LT:
                    RectCell.X = Math.Min(RectCell.X + Width, RectCell.X + RectCell.Width - 10);
                    RectCell.Y = Math.Min(RectCell.Y + Height, RectCell.Y + RectCell.Height - 10);

                    RectCell.Width = Math.Max(RectCell.Width - Width, 10);
                    RectCell.Height = Math.Max(RectCell.Height - Height, 10);
                    break;
                case CornerEnum.RT:
                    RectCell.Y = Math.Min(RectCell.Y + Height, RectCell.Y + RectCell.Height - 10);

                    RectCell.Width = Math.Max(RectCell.Width + Width, 10);
                    RectCell.Height = Math.Max(10, RectCell.Height - Height);
                    break;
                case CornerEnum.LB:
                    RectCell.X = Math.Min(RectCell.X + Width, RectCell.X + RectCell.Width - 10);

                    RectCell.Width = Math.Max(10, RectCell.Width - Width);
                    RectCell.Height = Math.Max(RectCell.Height + Height, 10);
                    break;
            }

            RectCell = BoundRect(RectCell, RectBoundary.Size);
        }

        public override string ToString()
        {
            string Str = "";

            Str += ((int)CellType).ToString() + Separator;
            Str += RecttoString(RectCell) + Separator;
            Str += ((int)CellProperty).ToString() + Separator;
            Str += CircleCell.ToString() + Separator;
            Str += CellGroup;

            return Str;
        }
        public void FromString(string Str)
        {
            string[] strs = Str.Split(Separator);

            CellType = (CelltypeEnum)int.Parse(strs[0]);
            RectCell = StringtoRect(strs[1]);

            if (strs.Length > 2)
            {
                CellProperty = (CellPropertyEnum)int.Parse(strs[2]);
            }
            if (strs.Length > 3)
            {
                CircleCell = new CircleClass(strs[3]);
            }
            if (strs.Length > 4)
            {
                CellGroup = strs[4];
            }
        }
        Rectangle SimpleRect(Point Pt, int Width, int Height)
        {
            Rectangle rect = SimpleRect(Pt);
            rect.Inflate(Width, Height);

            return rect;
        }
        Rectangle SimpleRect(Point Pt)
        {
            return new Rectangle(Pt.X, Pt.Y, 1, 1);
        }
        string RecttoString(Rectangle Rect)
        {
            return Rect.X.ToString().PadLeft(4) + "," + Rect.Y.ToString().PadLeft(4) + "," + Rect.Width.ToString().PadLeft(4) + "," + Rect.Height.ToString().PadLeft(4);
        }
        Rectangle StringtoRect(string RectStr)
        {
            string[] str = RectStr.Split(',');
            return new Rectangle(int.Parse(str[0]), int.Parse(str[1]), int.Parse(str[2]), int.Parse(str[3]));
        }
        Rectangle BoundRect(Rectangle InnerRect, Size BoundSize)
        {
            InnerRect.X = Math.Min(Math.Max(InnerRect.X, 0), (BoundSize.Width - InnerRect.Width < 0 ? 0 : BoundSize.Width - InnerRect.Width));
            InnerRect.Y = Math.Min(Math.Max(InnerRect.Y, 0), (BoundSize.Height - InnerRect.Height < 0 ? 0 : BoundSize.Height - InnerRect.Height));

            if (BoundSize.Width <= InnerRect.X + InnerRect.Width)
                InnerRect.Width = BoundValue(InnerRect.Width, BoundSize.Width - InnerRect.X, 1);
            if (BoundSize.Height <= InnerRect.Height + InnerRect.Height)
                InnerRect.Height = BoundValue(InnerRect.Height, BoundSize.Height - InnerRect.Y, 1);

            return InnerRect;
        }
        int BoundValue(int Value, int Max, int Min)
        {
            return Math.Max(Math.Min(Value, Max), Min);

        }
        void BoundRect(ref Rectangle InnerRect, Size BoundSize)
        {
            InnerRect.X = Math.Min(Math.Max(InnerRect.X, 0), (BoundSize.Width - InnerRect.Width < 0 ? 0 : BoundSize.Width - InnerRect.Width));
            InnerRect.Y = Math.Min(Math.Max(InnerRect.Y, 0), (BoundSize.Height - InnerRect.Height < 0 ? 0 : BoundSize.Height - InnerRect.Height));

            if (BoundSize.Width <= InnerRect.X + InnerRect.Width)
                InnerRect.Width = BoundValue(InnerRect.Width, BoundSize.Width - InnerRect.X, 1);
            if (BoundSize.Height <= InnerRect.Height + InnerRect.Height)
                InnerRect.Height = BoundValue(InnerRect.Height, BoundSize.Height - InnerRect.Y, 1);
        }
    }
}
