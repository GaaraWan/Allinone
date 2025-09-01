using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allinone.FormSpace.BasicPG
{
    public class N1Class
    {
        public N1Class() { }
        public N1Class(string name) { }
        [DisplayName("角度")]
        [Description("角度")]
        public float Rotation { get; set; } = 15;
        [DisplayName("相似度")]
        [Description("相似度")]
        public float Tolerance { get; set; } = 0.7f;

        public void FromString(string eStr)
        {
            string[] strings = eStr.Split(',');
            if (strings.Length > 1)
            {
                Rotation = float.Parse(strings[0]);
                Tolerance = float.Parse(strings[1]);
            }
        }
        public string ToParaString()
        {
            string str = string.Empty;

            str += Rotation.ToString() + ",";
            str += Tolerance.ToString();

            return str;
        }

    }
}
