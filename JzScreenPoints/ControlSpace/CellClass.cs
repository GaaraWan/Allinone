using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using JzScreenPoints.BasicSpace;

namespace JzScreenPoints.ControlSpace
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

        public int RowTag = 0;

        public CircleClass()
        {

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
            return JzTools.RectFToRect(rectf);
        }
    }

    public class CellClass
    {
        const char Separator = '@';

        const bool IsInsidePointSelected = false;
        const int BorderPixel = 3;
        const int BorderCatchPixel = 20;
        const int CornerPixel = 5;
        const int CornerCatchPixel = 10;

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

        public bool IsFirstSelected = false;
        public bool IsSelected = false;

        public string RelateIndexNoStr = "";

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

                        //if (!INI.ISREVERSE)
                        //    PSelect = new Pen(Color.Tomato, BorderPixel);
                        //else
                            PSelect = new Pen(Color.Orange, BorderPixel);

                        //PSelect = new Pen(Color.White, BorderPixel);
                        PNormal = new Pen(Color.Lime, BorderPixel);
                        PBorder = new Pen(Color.Black, BorderPixel);

                        SFirstSelect = new SolidBrush(Color.Red);

                        //SSelect = new SolidBrush(Color.White);

                        //if (!INI.ISREVERSE)
                        //    SSelect = new SolidBrush(Color.Tomato);
                        //else
                            SSelect = new SolidBrush(Color.Orange);

                        SNormal = new SolidBrush(Color.Lime);
                        SBorder = new SolidBrush(Color.Black);
                        break;
                    case CellPropertyEnum.STATIC:
                        PFirstSelect = new Pen(Color.Red, BorderPixel);
                        PSelect = new Pen(Color.Blue, BorderPixel);

                        //if(!INI.ISREVERSE)
                        //    PNormal = new Pen(Color.Yellow, BorderPixel);
                        //else
                            PNormal = new Pen(Color.Green, BorderPixel);
                        
                        PBorder = new Pen(Color.Black, BorderPixel);

                        SFirstSelect = new SolidBrush(Color.Red);
                        SSelect = new SolidBrush(Color.Blue);

                        //if(!INI.ISREVERSE)
                        //    SNormal = new SolidBrush(Color.Yellow);
                        //else
                            SNormal = new SolidBrush(Color.Green);
                        
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
                    retRect = JzTools.SimpleRect(new Point(rect.Left, rect.Bottom), catchpixel, catchpixel);
                    break;
                case CornerEnum.RT:
                    retRect = JzTools.SimpleRect(new Point(rect.Right, rect.Top), catchpixel, catchpixel);
                    break;
                case CornerEnum.RB:
                    retRect = JzTools.SimpleRect(new Point(rect.Right, rect.Bottom), catchpixel, catchpixel);
                    break;
                case CornerEnum.LT:
                default:
                    retRect = JzTools.SimpleRect(rect.Location, catchpixel, catchpixel);
                    break;
            }
            return retRect;
        }

        public CornerEnum CatchCorner(Point pt)
        {
            CornerCatched = CornerEnum.NONE;

            if (CellProperty == CellPropertyEnum.STATIC)
                return CornerCatched;

            Rectangle rectpt = JzTools.SimpleRect(pt);

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
            Rectangle rectpt = JzTools.SimpleRect(pt);

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

            JzTools.BoundRect(ref RectCell, RectBoundary.Size);
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

            RectCell = JzTools.BoundRect(RectCell, RectBoundary.Size);
        }

        public override string ToString()
        {
            string Str = "";

            Str += ((int)CellType).ToString() + Separator;
            Str += JzTools.RecttoString(RectCell) + Separator;
            Str += ((int)CellProperty).ToString();

            return Str;
        }
        public void FromString(string Str)
        {
            string[] strs = Str.Split(Separator);

            CellType = (CelltypeEnum)int.Parse(strs[0]);
            RectCell = JzTools.StringtoRect(strs[1]);

            if (strs.Length > 2)
            {
                CellProperty = (CellPropertyEnum)int.Parse(strs[2]);
            }
        }

    }
}
