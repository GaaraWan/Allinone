using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using JetEazy.BasicSpace;
using JetEazy.UISpace;
using System.Drawing;
using System.Drawing.Design;

using JetEazy;
using JzDisplay;


namespace Allinone.OPSpace.CPDSpace
{
    public class ASSEMBLEClass : ICustomClass
    {
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
        public class DisplayModeAttribute : Attribute
        {
            private readonly string mode;
            public DisplayModeAttribute(string mode)
            {
                this.mode = mode ?? "";

            }
            public override bool Match(object obj)
            {
                var other = obj as DisplayModeAttribute;
                if (other == null) return false;

                if (other.mode == mode) return true;

                // allow for a comma-separated match, in either direction
                if (mode.IndexOf(',') >= 0)
                {
                    string[] tokens = mode.Split(',');
                    if (Array.IndexOf(tokens, other.mode) >= 0) return true;
                }
                else if (other.mode.IndexOf(',') >= 0)
                {
                    string[] tokens = other.mode.Split(',');
                    if (Array.IndexOf(tokens, mode) >= 0) return true;
                }
                return false;
            }
        }

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

        #region Normal Factors

        [Category("01.Normal"),DefaultValue(""), ReadOnly(true)]
        public string Name
        {
            get; set;
        }
        [Category("01.Normal"),DefaultValue(""), ReadOnly(true)]
        public string RelatePA
        {
            get; set;
        }
        [Category("01.Normal"), DefaultValue(""), ReadOnly(true)]
        public SizeF OrgSizeF
        {
            get; set;
        }
        [Category("01.Normal"), DefaultValue(1f)]
        [Description("Size Ratio.\rRange 0.01 <-> 10,  default is 1")]
        [DisplayName("Ratio")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0.01f, 10f, 0.1f, 2)]
        public float Ratio
        {
            get; set;
        }
        [Category("01.Normal"), DefaultValue(0)]
        [Description("0 is No Need.\rRange -200 <-> 200, default 0")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-200, 200)]
        public int Brightness
        {
            get; set;
        }
        [Category("01.Normal"), DefaultValue(0)]
        [Description("0 is No Need.\rRange -200 <-> 200, default 0")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-200, 200)]
        public int Contrast
        {
            get; set;
        }

        [Category("01.Normal"), DefaultValue(0)]
        [Description("0 is No Need.\r default 0")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 10000000)]
        public int iLeft
        {
            get; set;
        }
        [Category("01.Normal"), DefaultValue(0)]
        [Description("0 is No Need.\r default 0")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 10000000)]
        public int iRight
        {
            get; set;
        }
        [Category("01.Normal"), DefaultValue(0)]
        [Description("0 is No Need.\r default 0")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 10000000)]
        public int iTop
        {
            get; set;
        }
        [Category("01.Normal"), DefaultValue(0)]
        [Description("0 is No Need.\r default 0")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 10000000)]
        public int iBottom
        {
            get; set;
        }

        #endregion

        public void ConstructProperty(VersionEnum ver, OptionEnum opt)
        {
            publicproperties.Add(new myProperty("Name", "01.Normal"));
            publicproperties.Add(new myProperty("RelatePA", "01.Normal"));
            publicproperties.Add(new myProperty("OrgSizeF", "01.Normal"));
            publicproperties.Add(new myProperty("Ratio", "01.Normal"));
            publicproperties.Add(new myProperty("Brightness", "01.Normal"));
            publicproperties.Add(new myProperty("Contrast", "01.Normal"));

            publicproperties.Add(new myProperty("iLeft", "01.Normal"));
            publicproperties.Add(new myProperty("iRight", "01.Normal"));
            publicproperties.Add(new myProperty("iTop", "01.Normal"));
            publicproperties.Add(new myProperty("iBottom", "01.Normal"));
        }
        
        public void GetNormal(NORMALClass normalpara)
        {
            Name = normalpara.Name;
            RelatePA = normalpara.RelatePA;
            OrgSizeF = normalpara.OrgSizeF;
            Ratio = normalpara.Ratio;
            Brightness = normalpara.Brightness;
            Contrast = normalpara.Contrast;

            iLeft = normalpara.iLeft;
            iRight = normalpara.iRight;
            iTop = normalpara.iTop;
            iBottom = normalpara.iBottom;
        }
        public void SetNormal(NORMALClass normalpara)
        {
            normalpara.Name = Name;
            normalpara.RelatePA = RelatePA;
            normalpara.OrgSizeF = OrgSizeF;
            normalpara.Ratio = Ratio;
            normalpara.Brightness = Brightness;
            normalpara.Contrast = Contrast;

            normalpara.iLeft = iLeft;
            normalpara.iRight = iRight;
            normalpara.iTop = iTop;
            normalpara.iBottom = iBottom;

        }

    }
}
