using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace JetEazy.BasicSpace
{
    class JzStringConverter : StringConverter
    {
        public class Para
        {
            public static string[] data;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(Para.data);
        }
    }
}
