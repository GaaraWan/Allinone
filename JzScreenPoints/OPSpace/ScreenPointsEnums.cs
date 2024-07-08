using System.Drawing;

namespace JzScreenPoints
{
    enum ScreenResolutionEnum
    {
        COUNT = 1,

        DEFAULT_1440x900 = 0,
        DEFAULT_1280x700 = 1,
    }
    enum PtSizeEnum
    {
        COUNT = 5,

        PtBk_Color = 0,

        PtSize_5X5 = 1,
        PtSize_11X11 = 2,
        PtSize_1X1 = 3,
        PtSize_3X3 = 4,
        //PtSize_4X4 = 5,
    }
    enum VersionEnum
    {
        None,
        Allinone,
    }
    public class Tools
    {
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
    }
}
