using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Allinone.BasicSpace
{
    class JzASNItemStringConverter : StringConverter
    {
        public static string[] ParaData;
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
            return new StandardValuesCollection(ParaData);
        }
    }
}
