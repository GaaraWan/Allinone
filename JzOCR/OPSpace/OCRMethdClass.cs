using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Design;
using System.Drawing;
using System.Drawing.Imaging;
using System.ComponentModel;

using JetEazy;
using JetEazy.UISpace;
using JetEazy.BasicSpace;

using WorldOfMoveableObjects;

namespace JzOCR.OPSpace
{
    public class OCRMethdClass : ICustomClass
    {
        PropertyList publicproperties = new PropertyList();
        //ICustomClass implementation
        public PropertyList PublicProperties
        {
            get
            {
                return publicproperties;
            }
            set
            {
                publicproperties = value;
            }
        }
        
        public int No = 0;

        int brightness = 0;
        int contrast = 0;
        float ratio = 0;

        [DefaultValue(0)]
        [Description("Birghtness \rRange -200 <-> 200, default 0")]
        [DisplayName("Brightness")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-200, 200)]
        public  int Brightness
        {
            get
            {
                return brightness;
            }
            set
            {
                brightness = value;
                OnBrightnesChange(this,value);
            }
        }
        public delegate void TriggerHandlerBrightnes(OCRMethdClass myThis, int iValue);
        public event TriggerHandlerBrightnes BrightnesChange;
        public void OnBrightnesChange(OCRMethdClass myThis,int opstatus)
        {
            if (BrightnesChange != null)
            {
                BrightnesChange(myThis,opstatus);
            }
        }


        [DefaultValue(0)]
        [Description("Contrast \rRange -200 <-> 200, default 0")]
        [DisplayName("Contrast")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-200, 200)]
        public int Contrast
        {
            get
            {
                return contrast;
            }
            set
            {
                contrast = value;
                OnCotrastChange(this,value);
            }
        }
        public delegate void TriggerHandlerContrast(OCRMethdClass myThis, int iValue);
        public event TriggerHandlerContrast CotrastChange;
        public void OnCotrastChange(OCRMethdClass myThis, int opstatus)
        {
            if (CotrastChange != null)
            {
                CotrastChange(myThis, opstatus);
            }
        }

        [DefaultValue(0)]
        [Description("Ratio \rRange 0 <-> 100, default 30")]
        [DisplayName("Ratio")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 1f, 2)]
        public float Ratio
        {
            get
            {
                return ratio;
            }
            set
            {
                ratio = value;
                OnRatioChange(this,value);
            }
        }
        public delegate void TriggerHandlerRatio(OCRMethdClass myThis, float iValue);
        public event TriggerHandlerRatio RatioChange;
        public void OnRatioChange(OCRMethdClass myThis, float opstatus)
        {
            if (RatioChange != null)
            {
                RatioChange(myThis,opstatus);
            }
        }

        public Bitmap bmpMethod = new Bitmap(1, 1);
        public JzRectEAG RectEAG = new JzRectEAG(Color.FromArgb(60, Color.Red), new RectangleF(0, 0, 50, 20));

        public Bitmap GetBitmMap
        {
            get
            {
                if (Brightness == 0 && Contrast == 0)
                    return new Bitmap(bmpMethod);

                Bitmap bmpTemp = bmpMethod.Clone(new Rectangle(0, 0, bmpMethod.Width, bmpMethod.Height), 
                    PixelFormat.Format24bppRgb );
                myImageProcessor.SetBrightContrastR(bmpTemp, Brightness, Contrast,true);
                return bmpTemp;
            }
        }
        
        public OCRMethdClass()
        {

        }
        public OCRMethdClass(string str)
        {
            FromString(str);
        }
        public OCRMethdClass Clone()
        {
            OCRMethdClass newmethod = new OCRMethdClass(this.ToString());

            newmethod.bmpMethod.Dispose();
            newmethod.bmpMethod = new Bitmap(bmpMethod);

            return newmethod;
        }
        public void Load(string ocrpath)
        {
            GetBMP(ocrpath + "\\M" + No.ToString(OCRClass.OrgOCRNoString) + ".png", ref bmpMethod);
        }
        public void Save(string ocrpath)
        {
            SaveBMP(ocrpath + "\\M" + No.ToString(OCRClass.OrgOCRNoString) + ".png",ref bmpMethod);
        }
        public override string ToString()
        {
            char seperator = Universal.SeperateCharA;

            string str = "";

            str += No.ToString() + seperator;           //0
            str += Brightness.ToString() + seperator;   //1
            str += Contrast.ToString() + seperator;     //2
            str += Ratio.ToString() + seperator;        //3
            str += RectEAG.ToString() + seperator; //4
            str += "";

            return str;
        }
        public void FromString(string str)
        {
            char seperator = Universal.SeperateCharA;

            string[] strs = str.Split(seperator);

            No = int.Parse(strs[0]);
            Brightness = int.Parse(strs[1]);
            Contrast = int.Parse(strs[2]);
            Ratio = float.Parse(strs[3]);

            if(strs[4].Trim() != "")
                RectEAG.FromString(strs[4]);

        }
        public void Suicide()
        {
            bmpMethod.Dispose();
        }

        public string ToMethodIndexString()
        {
            return "M-" + No.ToString(OCRClass.OrgOCRNoString);
        }

        public void ConstructProperty()
        {
            publicproperties.Clear();

            publicproperties.Add(new myProperty("Brightness", ""));
            publicproperties.Add(new myProperty("Contrast", ""));
            publicproperties.Add(new myProperty("Ratio", ""));
        }

        void GetBMP(string bmpfilestr, ref Bitmap bmp)
        {
            Bitmap bmptmp = new Bitmap(bmpfilestr);

            bmp.Dispose();
            bmp = new Bitmap(bmptmp);

            bmptmp.Dispose();
        }
        void SaveBMP(string bmpfilestr, ref Bitmap bmp)
        {
            Bitmap bmptmp = new Bitmap(bmp);

            bmptmp.Save(bmpfilestr, Universal.GlobalImageFormat);

            bmptmp.Dispose();
        }
    }

 
}
