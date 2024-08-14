using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace JetEazy
{
    public class DataMappingClass
    {
        public DataMappingClass()
        {

        }

        public PointF PtfCenter { get; set; } = new PointF();
        public string ReportStr { get; set; } = string.Empty;
        public int ReportBinValue { get; set; } = 0;
        public int ReportIndex { get; set; } = 0;
        public string ReportRowCol { get; set; } = string.Empty;
        public PointF PageRunLocation
        {
            get { return PtfCenter; }
            set { PtfCenter = value; }
        }
        public Bitmap bmpResult = new Bitmap(1, 1);
        public string GetDescStr()
        {
            string str = "PASS";
            switch (ReportBinValue)
            {
                case 1:
                    str = "无胶";
                    break;
                case 2:
                    str = "尺寸错误";
                    break;
                case 3:
                    str = "晶片表面溢胶";
                    break;
                case 4:
                    str = "胶水宽度异常";
                    break;
                case 5:
                    str = "无芯片";
                    break;
            }
            return str;
        }
        public string GetDescStrSorld()
        {
            string str = "PASS";
            switch (ReportBinValue)
            {
                case 1:
                    str = "定位错误";
                    break;
                case 2:
                    str = "检测错误";
                    break;
                case 3:
                    str = "量测错误";
                    break;
                    //case 4:
                    //    str = "胶水宽度异常";
                    //    break;
                    //case 5:
                    //    str = "无芯片";
                    //    break;
            }
            return str;
        }
    }
}
