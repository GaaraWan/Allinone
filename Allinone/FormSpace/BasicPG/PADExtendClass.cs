using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allinone.FormSpace.BasicPG
{
    public class PADExtendClass
    {
        public PADExtendClass() { }
        public PADExtendClass(string name) { }
        [DisplayName("01.屏蔽上边")]
        [Description("")]
        public bool bNoInspectTop { get; set; } = false;
        [DisplayName("02.屏蔽下边")]
        [Description("")]
        public bool bNoInspectBottom { get; set; } = false;
        [DisplayName("03.屏蔽左边")]
        [Description("")]
        public bool bNoInspectLeft { get; set; } = false;
        [DisplayName("04.屏蔽右边")]
        [Description("")]
        public bool bNoInspectRight { get; set; } = false;
        //[DisplayName("01.屏蔽All")]
        [Description(""),Browsable(false)]
        public bool bNoInspectAll
        {
            get
            {
                return bNoInspectTop && bNoInspectBottom && bNoInspectLeft && bNoInspectRight;
            }
        }
        [DisplayName("05.打开碰触元件检测")]
        [Description("")]
        public bool bOpenUseContactEle { get; set; } = false;
        [DisplayName("05a.碰触元件外扩X")]
        [Description("以chip为中心 X方向延伸 单位像素(pixel)")]
        public int cEleX { get; set; } = 50;
        [DisplayName("05b.碰触元件外扩Y")]
        [Description("以chip为中心 Y方向延伸 单位像素(pixel)")]
        public int cEleY { get; set; } = 50;

        public void FromString(string eStr)
        {
            string[] strings = eStr.Split(',');
            if (strings.Length > 6)
            {
                bNoInspectTop = strings[0] == "1";
                bNoInspectBottom = strings[1] == "1";
                bNoInspectLeft = strings[2] == "1";
                bNoInspectRight = strings[3] == "1";
                bOpenUseContactEle = strings[4] == "1";
                cEleX = int.Parse(strings[5]);
                cEleY = int.Parse(strings[6]);
            }
        }
        public string ToParaString()
        {
            string str = string.Empty;

            str += (bNoInspectTop ? "1" : "0") + ",";
            str += (bNoInspectBottom ? "1" : "0") + ",";
            str += (bNoInspectLeft ? "1" : "0") + ",";
            str += (bNoInspectRight ? "1" : "0") + ",";
            str += (bOpenUseContactEle ? "1" : "0") + ",";
            str += cEleX.ToString() + ",";
            str += cEleY.ToString();

            return str;
        }
    }
}
