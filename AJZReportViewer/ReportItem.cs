using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJZReportViewer
{
    public class ReportItem
    {
        public string DateStr { get; set; } = string.Empty;
        public string RecipeName { get; set; } = string.Empty;
        public string AllCount { get; set; } = string.Empty;
        public string FailCount { get; set; } = string.Empty;
        public string ToReport1()
        {
            string str = string.Empty;

            str += DateStr + ",";
            str += RecipeName + ",";
            str += AllCount + ",";
            str += FailCount + ",";

            double.TryParse(AllCount, out double all);
            double.TryParse(FailCount, out double fail);

            if (all > 0 && fail >= 0)
            {
                double per = fail / all * 100;
                str += per.ToString("0.00") + "%" + Environment.NewLine;
            }
            else
                str += "0" + Environment.NewLine;

            return str;
        }
    }
}
