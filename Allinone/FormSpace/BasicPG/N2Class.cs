using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allinone.FormSpace.BasicPG
{
    public class N2Class
    {
        public N2Class() { }
        public N2Class(string name) { }
        [DisplayName("二值化阈值")]
        [Description("二值化阈值")]
        public int ThresholdValue { get; set; } = 128;
        [DisplayName("找白色")]
        [Description("找白色")]
        public bool IsWhite { get; set; } = true;
        [DisplayName("允许数量")]
        [Description("允许数量")]
        public int Count { get; set; } = 15;

        public void FromString(string eStr)
        {
            string[] strings = eStr.Split(',');
            if (strings.Length > 2)
            {
                ThresholdValue = int.Parse(strings[0]);
                Count = int.Parse(strings[1]);
                IsWhite = strings[2] == "1";
            }
        }
        public string ToParaString()
        {
            string str = string.Empty;

            str += ThresholdValue.ToString() + ",";
            str += Count.ToString() + ",";
            str += (IsWhite ? "1" : "0").ToString();

            return str;
        }

    }
}
