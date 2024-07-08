
using System.Collections.Generic;
using System.Drawing;

namespace JzScreenPoints.Interface
{
    public interface IRpiDriver
    {
        int DrawMyPaints(List<Point> ptlist);
        int DrawMyPaintRectS(List<Rectangle> m_rectlist);
    }
}
