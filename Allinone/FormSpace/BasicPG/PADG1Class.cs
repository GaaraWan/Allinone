using Allinone.BasicSpace.MVD;
using JetEazy.BasicSpace;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allinone.FormSpace.BasicPG
{
    public class PADG1Class
    {
        public PADG1Class() { }
        public PADG1Class(string name) { }
        [DisplayName("滤波尺寸")]
        [Description("滤波核半宽：" +
            "用于增强边缘和抑制噪声，" +
            "最小值为1。" +
            "当边缘模糊或有噪声干扰时，" +
            "增大该值有利于使检测结果更加稳定，" +
            "但如果边缘与边缘之间挨得太近时反而会影响边缘位置的精度甚至丢失边缘，" +
            "该值需根据实际情况设置。")]
        public int HalfKernelSize { get; set; } = 1;
        [DisplayName("对比阈值")]
        [Description("对比阈值：" +
            "对比阈值即梯度阈值，" +
            "范围0~255，" +
            "只有边缘梯度阈值大于该值的边缘点才能被检测到。" +
            "数值越大，" +
            "抗噪声能力越强，" +
            "得到的边缘数量越少，" +
            "甚至导致目标边缘点被筛除。 ")]
        public int ContrastTH { get; set; } = 15;
        [DisplayName("查找模式")]
        [Description("- “最宽”表示检测范围内间距最大的边缘对。\r\n- “最窄”表示检测范围内间距最小的边缘对。\r\n- “最强”表示检测范围内边缘对平均梯度最大的边缘对。\r\n- “最弱”表示检测范围内梯度最小的边缘对。")]
        [TypeConverter(typeof(JzEnumConverter))]
        public EWFindMode FindMode { get; set; } = EWFindMode.Strongest;
        [DisplayName("外扩X方向")]
        [Description("限定X方向的范围超出则NG")]
        public int FindX { get; set; } = 50;
        [DisplayName("外扩Y方向")]
        [Description("限定Y方向的范围超出则NG")]
        public int FindY { get; set; } = 50;
        [DisplayName("内缩X方向")]
        [Description("限定X方向的内缩范围")]
        public int FindInX { get; set; } = 15;
        [DisplayName("内缩Y方向")]
        [Description("限定Y方向的内缩范围")]
        public int FindInY { get; set; } = 15;


        public void FromString(string eStr)
        {
            string[] strings = eStr.Split(',');
            if (strings.Length > 5)
            {
                HalfKernelSize = int.Parse(strings[0]);
                ContrastTH = int.Parse(strings[1]);
                FindX = int.Parse(strings[2]);
                FindY = int.Parse(strings[3]);
                FindInX = int.Parse(strings[4]);
                FindInY = int.Parse(strings[5]);
                //IsWhite = strings[2] == "1";
            }
            if (strings.Length > 6)
            {
                FindMode = (EWFindMode)int.Parse(strings[6]);
            }
        }
        public string ToParaString()
        {
            string str = string.Empty;

            str += HalfKernelSize.ToString() + ",";
            str += ContrastTH.ToString() + ",";
            str += FindX.ToString() + ",";
            str += FindY.ToString() + ",";
            str += FindInX.ToString() + ",";
            str += FindInY.ToString() + ",";
            str += ((int)FindMode).ToString();
            //str += (IsWhite ? "1" : "0").ToString();

            return str;
        }
    }
}
