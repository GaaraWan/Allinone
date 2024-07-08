using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Drawing.Imaging;
using JzScreenPoints.ControlSpace;
using System.Windows.Forms;
using System.IO;

namespace JzScreenPoints
{
    class Universal
    {
        public static VersionEnum VER = VersionEnum.Allinone;
        public static bool IsDebug = false;

        public static string GlobalImageTypeString = ".png";
        public static ImageFormat GlobalImageFormat = ImageFormat.Png;
        
        public const string JSPCollectionPath = @"D:\JETEAZY\JSP";
    }
}
