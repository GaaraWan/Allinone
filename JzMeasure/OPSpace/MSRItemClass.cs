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

namespace JzMSR.OPSpace
{
    public class MSRItemClass : ICustomClass
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
        
        int no = 0;

        [ReadOnly(true)]
        public int No
        {
            get
            {
                return no;
            }
            set
            {
                no = value;
            }
        }
        [ReadOnly(true)]
        public PointF CenterPointF
        {
            get
            {
                return _centerpointf;
            }
            set
            {
                _centerpointf = value;
            }
        }
        [ReadOnly(true)]
        public PointF RelatePointF
        {
            get
            {
                return _relatepointf;
            }
            set
            {
                _relatepointf = value;
            }
        }
        [ReadOnly(true)]
        public RectangleF RectRange
        {
            get
            {
                return RectEAG.RectAround;
            }
        }
        [ReadOnly(true)]
        public bool IsBlack
        {
            get { return _isblack; }
            set { _isblack = value; }
        }
        [ReadOnly(true)]
        public int Threshold
        {
            get { return _threshold; }
            set { _threshold = value; }
        }

        int brightness = 0;
        int contrast = 0;
        float ratio = 30;
        int _rowtag = 0;

        PointF _centerpointf = new PointF();
        PointF _relatepointf = new PointF();
        bool _isblack = false;
        int _threshold = 30;

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
            }
        }

        [DefaultValue(30)]
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
            }
        }

        [DefaultValue(0)]
        [Description("RowTag \rRange 0 <-> 200, default 0")]
        [DisplayName("RowTag")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100)]
        public int RowTag
        {
            get
            {
                return _rowtag;
            }
            set
            {
                _rowtag = value;
            }
        }

        public Bitmap bmpItem = new Bitmap(1, 1);
        public JzRectEAG RectEAG = new JzRectEAG(Color.FromArgb(60, Color.Red), new RectangleF(0, 0, 50, 20));
        JzFindObjectClass JzFind = new JzFindObjectClass();
        JzToolsClass JzTools = new JzToolsClass();

        public MSRItemClass()
        {

        }
        public MSRItemClass(string str)
        {
            FromString(str);
        }
        public MSRItemClass(Bitmap bmp,Rectangle rect,int no)
        {
            bmpItem.Dispose();
            bmpItem = bmp;

            //_rect = rect;
            
            RectEAG.FromRectangleF(rect);
            RectEAG.RelateNo = no;
            RectEAG.RelatePosition = no;

            _centerpointf = JzTools.RectFCenter(rect);

            No = no;
        }


        public MSRItemClass Clone()
        {
            MSRItemClass newmethod = new MSRItemClass(this.ToString());

            newmethod.bmpItem.Dispose();
            newmethod.bmpItem = new Bitmap(bmpItem);

            return newmethod;
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
            str += JzTools.PointFToString(CenterPointF) + seperator;//5
            str += JzTools.PointFToString(RelatePointF) + seperator;//6
            str += RowTag.ToString() + seperator;//7
            str += (_isblack ? "1" : "0" ) + seperator;//8
            str += _threshold.ToString() + seperator;//8
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

            if (strs[4].Trim() != "")
            {
                RectEAG.FromString(strs[4]);
                RectEAG.RelateNo = No;
                RectEAG.RelatePosition = No;//這個要關聯起來，這樣初始化完參數才能選擇。ADD Gaara
            }

            if (strs.Length > 8)
            {
                CenterPointF = JzTools.StringToPointF(strs[5]);
                RelatePointF = JzTools.StringToPointF(strs[6]);
                RowTag = int.Parse(strs[7]);
            }
            if (strs.Length > 10)
            {
                _isblack = strs[8] == "1";
                _threshold = int.Parse(strs[9]);
            }
        }
        public void Suicide()
        {
            bmpItem.Dispose();
        }

        public string ToMethodIndexString()
        {
            return "M-" + No.ToString(MSRClass.OrgMSRNoString);
        }

        public void ConstructProperty()
        {
            publicproperties.Clear();

            publicproperties.Add(new myProperty("No", ""));
            publicproperties.Add(new myProperty("Brightness", ""));
            publicproperties.Add(new myProperty("Contrast", ""));
            publicproperties.Add(new myProperty("Ratio", ""));
            publicproperties.Add(new myProperty("RowTag", ""));

            publicproperties.Add(new myProperty("CenterPointF", ""));
            publicproperties.Add(new myProperty("RelatePointF", ""));
            publicproperties.Add(new myProperty("RectRange", ""));

            publicproperties.Add(new myProperty("IsBlack", ""));
            publicproperties.Add(new myProperty("Threshold", ""));

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

        public void FindCenter(Bitmap bmp)
        {
            FindCenter(bmp, _isblack, _threshold);
        }
        public void FindCenter(Bitmap bmp,bool ispointblack,int thresholdratio)
        {
            JzFindObjectClass JzFind = new JzFindObjectClass();
            HistogramClass historgram = new HistogramClass(2);
            historgram.GetHistogram(bmp);

            int max = historgram.GetMaxRatioAVG(10);
            int min = historgram.GetMinRatioAVG(10);

            int threshvalue = min + (int)((float)(max - min) * (float)thresholdratio / 100f);

            if (ispointblack)
                JzFind.SetThreshold(bmp, SimpleRect(bmp.Size), threshvalue, 0, 255, true);
            else
            {
                threshvalue = max - (int)((float)(max - min) * (float)thresholdratio / 100f);
                JzFind.SetThreshold(bmp, SimpleRect(bmp.Size), threshvalue, 255, 0, true);
            }
            
            JzFind.Find(bmp, Color.Red);

            Rectangle rect = JzFind.GetRect();

            _centerpointf = JzTools.GetRectFCenter(rect);

            JzTools.DrawCross(ref bmp, JzTools.SimpleRect(CenterPointF, 3), new Pen(Color.Lime, 2));

            _centerpointf.X += RectEAG.Center.X;
            _centerpointf.Y += RectEAG.Center.Y;

        }
        Rectangle SimpleRect(Size Sz)
        {
            return new Rectangle(0, 0, Sz.Width, Sz.Height);
        }
    }
}
