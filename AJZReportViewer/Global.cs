using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJZReportViewer
{
    public enum VERSION
    {
        MAIN_X6,
        SDM2,
        SDM5,
    }
    public class Global
    {
        public static VERSION VER = VERSION.MAIN_X6;
    }
}
