using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetEazy.BasicSpace;
using System.Drawing;

using JetEazy;
using JzDisplay;

namespace Allinone.OPSpace.CPDSpace
{
    public class NORMALClass
    {
        public string Name = "";
        public string RelatePA = "";
        public SizeF OrgSizeF = new SizeF();
        public float Ratio = 1f;
        public int Brightness = 0;
        public int Contrast = 0;
        public int iLeft = 0;
        public int iRight = 0;
        public int iTop = 0;
        public int iBottom = 0;
        
        public NORMALClass()
        {
            //Name = "";
            //AliasName = "";
            //Brightness = 0;
            //IsMask = false;
            //ExtendX = 20;
            //ExtendY = 20;
        }
        public NORMALClass(string str)
        {
            FromString(str);
        }

        public override string ToString()
        {
            char seperator = Universal.SeperateCharB;
            string str = "";

            str += Name + seperator;                        //0
            str += RelatePA + seperator;                    //1
            str += SizeFtoString(OrgSizeF) + seperator;     //2
            str += Ratio.ToString() + seperator;            //3
            str += Brightness.ToString() + seperator;       //4
            str += Contrast.ToString() + seperator;         //5
            str += iLeft.ToString() + seperator;         //5
            str += iRight.ToString() + seperator;         //5
            str += iTop.ToString() + seperator;         //5
            str += iBottom.ToString() + seperator;         //5
            str += "";

            return str;
        }
        public void FromString(string str)
        {
            char seperator = Universal.SeperateCharB;
            string[] strs = str.Split(seperator);

            Name = strs[0];
            RelatePA = strs[1];

            OrgSizeF = StringToSizeF(strs[2]);
            Ratio = float.Parse(strs[3]);
            Brightness = int.Parse(strs[4]);
            Contrast = int.Parse(strs[5]);

            if (strs.Length > 9)
            {
                iLeft = int.Parse(strs[6]);
                iRight = int.Parse(strs[7]);
                iTop = int.Parse(strs[8]);
                iBottom = int.Parse(strs[9]);
            }
        }

        public void FromPropertyChange(string changeitemstring,string valuestring)
        {
            string [] str = changeitemstring.Split(';');

            if (str[0] != "01.Normal")
                return;

            switch(str[1])
            {
                case "Name":
                    Name = valuestring;
                    break;
                case "Ratio":
                    Ratio = float.Parse(valuestring);
                    break;
                case "Brightness":
                    Brightness = int.Parse(valuestring);
                    break;
                case "Contrast":
                    Contrast = int.Parse(valuestring);
                    break;

                case "iLeft":
                    iLeft = int.Parse(valuestring);
                    break;
                case "iRight":
                    iRight = int.Parse(valuestring);
                    break;
                case "iTop":
                    iTop = int.Parse(valuestring);
                    break;
                case "iBottom":
                    iBottom = int.Parse(valuestring);
                    break;
            }
        }

        public void Reset()
        {
            Name = "";
            Ratio = 1f;
            Brightness = 0;
            Contrast = 0;
            iLeft = 0;
            iRight = 0;
            iTop = 0;
            iBottom = 0;
        }
        string SizeFtoString(SizeF sizef)
        {
            return sizef.Width.ToString() + "," + sizef.Height.ToString();
        }
        SizeF StringToSizeF(string str)
        {
            float[] sizevalue = Array.ConvertAll(str.Split(','), float.Parse);

            return new SizeF(sizevalue[0], sizevalue[1]);
        }

    }
}
