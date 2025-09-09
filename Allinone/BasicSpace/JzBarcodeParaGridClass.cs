using JetEazy.BasicSpace;
using JetEazy.UISpace;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allinone.BasicSpace
{
    public class JzBarcodeParaGridClass
    {
        public JzBarcodeParaGridClass()
        {

        }
        const string cat1 = "00.图像设定";

        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("A01.亮度")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 255)]
        [Browsable(true)]
        public int nBrightness { get; set; } = 0;
        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("A02.对比")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 255)]
        [Browsable(true)]
        public int nContrast { get; set; } = 0;

        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("A03.相机曝光")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100)]
        [Browsable(true)]
        public float CamExpo { get; set; } = 8;
        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("A04.相机曝光次数")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(1, 100)]
        [Browsable(true)]
        public int CamExpoCount { get; set; } = 1;
        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("A05.相机曝光梯度")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100)]
        [Browsable(true)]
        public float CamExpoOffset { get; set; } = 1;


        const string cat0 = "01.读码设定";

        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("A0.开启读码")]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100000)]
        [Browsable(true)]
        public bool IsOpenBarcode { get; set; } = false;
        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("A2.寻找区域")]
        public RectangleF RectF { get; set; } = new RectangleF(0, 0, 100, 100);
        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("A3.拍照位置")]
        public string MotorPositionStr { get; set; } = "0,0,0";


        public void FromingStr(string str)
        {
            string[] parts = str.Split(',');
            if (parts.Length > 4)
            {
                IsOpenBarcode = parts[0] == "1";
                RectF = StringToRectF(parts[1]);
                MotorPositionStr = parts[2];
                nBrightness = int.Parse(parts[3]);
                nContrast = int.Parse(parts[4]);
            }
            if (parts.Length > 6)
            {
                CamExpo = float.Parse(parts[5]);
            }
            if (parts.Length > 8)
            {
                CamExpoCount = int.Parse(parts[6]);
                CamExpoOffset = float.Parse(parts[7]);
            }
        }
        public string ToParaString()
        {
            string str = string.Empty;

            str += (IsOpenBarcode ? "1" : "0") + ",";
            str += RectFToString(RectF) + ",";
            str += MotorPositionStr + ",";
            str += nBrightness.ToString() + ",";
            str += nContrast.ToString() + ",";
            str += CamExpo.ToString() + ",";
            str += CamExpoCount.ToString() + ",";
            str += CamExpoOffset.ToString() + ",";

            return str;
        }

        public string PointF000ToString(PointF PTF)
        {
            return PTF.X.ToString("0.000") + ";" + PTF.Y.ToString("0.000");
        }
        public PointF StringToPointF(string Str)
        {
            string[] strs = Str.Split(';');
            return new PointF(float.Parse(strs[0]), float.Parse(strs[1]));
        }
        public string RectFToString(RectangleF RectF)
        {
            string Str = "";

            Str += RectF.X.ToString("0.00") + ";";
            Str += RectF.Y.ToString("0.00") + ";";
            Str += RectF.Width.ToString("0.00") + ";";
            Str += RectF.Height.ToString("0.00");

            return Str;
        }
        public RectangleF StringToRectF(string Str)
        {
            string[] strs = Str.Split(';');
            RectangleF rectF = new RectangleF();

            rectF.X = float.Parse(strs[0]);
            rectF.Y = float.Parse(strs[1]);
            rectF.Width = float.Parse(strs[2]);
            rectF.Height = float.Parse(strs[3]);

            return rectF;


        }

    }
}
