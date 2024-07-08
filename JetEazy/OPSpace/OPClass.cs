using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using JetEazy;
using JetEazy.BasicSpace;
using JetEazy.ControlSpace;

namespace JetEazy.OPSpace
{
    public class OPClass
    {
        protected const char LstSeparator = '\x1F';
        protected const char Separator = '\x1E';
        protected const char SubSeparator = '\x1D';

        protected const char CR = '\x0D';
        protected const char LF = '\x0A';

        public int GroupIndex = 0;
        public int Index = 0;
        protected OPTypeEnum OPType = OPTypeEnum.ASN;

        public CellClass Cell = new CellClass();

        public string IndexNoStr
        {
            get
            {
                return ValueToHEX(GroupIndex, 2) + Index.ToString("000");
            }
        }
        public string IndexName
        {
            get
            {
                return OPType.ToString() + "-" + IndexNoStr;
            }
        }

        protected virtual void FromString(string Str)
        {


        }
        protected virtual string ToOtherString()
        {
            string Str = "";

            return Str;
        }
        protected virtual void FromOtherString(string Str)
        {


        }

        public virtual void Suicide()
        {

        }

        public string ValueToHEX(long Value, int Length)
        {
            return ("00000000" + Value.ToString("X")).Substring(("00000000" + Value.ToString("X")).Length - Length, Length);
        }
    }
}
