using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

//using JetEazy.BasicSpace;
using JzKHC.AOISpace;
//using Jumbo301.UniversalSpace;

namespace JzKHC.ControlSpace
{
    class FrameClass
    {
        public const int CornerSize = 8;
        public const int ThreasholdColorGap = 4;
        
        protected HistogramClass Histogram = new HistogramClass(ThreasholdColorGap);
        protected ThresholdClass Threshold = new ThresholdClass();
        protected FindObjectClass FindObject = new FindObjectClass();

        public FrameClass()
        {


        }
    }
}
