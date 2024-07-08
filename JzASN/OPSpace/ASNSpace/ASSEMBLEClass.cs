using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using JetEazy.BasicSpace;
using JetEazy.UISpace;

using JetEazy;
using JetEazy.BasicSpace;
using JzDisplay;


namespace JzASN.OPSpace.ASNSpace
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
        [Category("01.Normal"),DefaultValue("")]
        public string AliasName
        {
            get; set;
        }
        [Category("01.Normal"), DefaultValue(ShapeDefineEnum.RECT)]
        [Description("Display Shape.")]
        [DisplayName("Shape")]
        [TypeConverter(typeof(JzEnumConverter))]
        public ShapeDefineEnum Shape
        {
            get; set;
        }

        #endregion

        public void ConstructProperty(VersionEnum ver, OptionEnum opt)
        {
            publicproperties.Add(new myProperty("Name", "01.Normal"));
            publicproperties.Add(new myProperty("AliasName", "01.Normal"));
            publicproperties.Add(new myProperty("Shape", "01.Normal"));

        }
        
        public void GetNormal(NORMALClass normal)
        {
            Name = normal.Name;
            AliasName = normal.AliasName;
            //Shape =  normal.Shape;
            Shape = (ShapeDefineEnum)Enum.Parse(typeof(ShapeDefineEnum), normal.Shape.ToString(), true);
        }
        public void SetNormal(NORMALClass normal)
        {
            normal.AliasName = AliasName;
            //normal.Shape = Shape;
            normal.Shape = (ShapeEnum)Enum.Parse(typeof(ShapeEnum), Shape.ToString(), true);
        }

    }
}
