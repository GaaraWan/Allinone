using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetEazy.BasicSpace;

namespace Allinone.OPSpace.AnalyzeSpace
{
    public class NORMALClass
    {
        public string Name = "";
        public string AliasName = "";

        public int Brightness = 0;  //所有的定位都是先加強後定位，然後再定位
        public int Contrast = 0;    //所有的加強都是利用最原始的圖來加強，而不會繼承加強的圖像

        public MaskMethodEnum MaskMethod = MaskMethodEnum.NONE; //指定 Mask 的方式
        public int ExtendX = 20;    //X 外擴
        public int ExtendY = 20;    //Y 外擴

        public string RelateASN = "None";
        public string RelateASNItem = "None";

        public bool IsSeed = false;

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

            str += Name + seperator;                  //0
            str += AliasName + seperator;             //1
            str += Brightness.ToString() + seperator; //2
            str += Contrast.ToString() + seperator;   //3
            str += ((int)MaskMethod).ToString() + seperator;  //4

            str += ExtendX.ToString() + seperator;    //5
            str += ExtendY.ToString() + seperator;    //6
            str += RelateASN.ToString() + seperator;    //7
            str += RelateASNItem.ToString() + seperator;    //8
            str += (IsSeed ? "1":"0") + seperator;    //9
            str += "";

            return str;
        }
        public void FromString(string str)
        {
            char seperator = Universal.SeperateCharB;
            string[] strs = str.Split(seperator);

            Name = strs[0];
            AliasName = strs[1];
            Brightness = int.Parse(strs[2]);
            Contrast = int.Parse(strs[3]);
            MaskMethod = (MaskMethodEnum)int.Parse(strs[4]);
            ExtendX = int.Parse(strs[5]);
            ExtendY = int.Parse(strs[6]);

            if(strs.Length > 8)
            {
                RelateASN = strs[7];
                RelateASNItem = strs[8];

                if(RelateASN.Trim() == "")
                {
                    RelateASN = "None";
                }
                if (RelateASNItem.Trim() == "")
                {
                    RelateASNItem = "None";
                }

            }
            if (strs.Length > 9)
            {
                IsSeed = strs[9] == "1";
            }

        }
        public void FromPropertyChange(string changeitemstring,string valuestring)
        {
            string [] str = changeitemstring.Split(';');

            if (str[0] != "01.Normal")
                return;

            switch(str[1])
            {
                case "AliasName":
                    AliasName = valuestring;
                    break;
                case "Brightness":
                    Brightness = int.Parse(valuestring);
                    break;
                case "Contrast":
                    Contrast = int.Parse(valuestring);
                    break;
                case "MaskMethod":
                    MaskMethod = (MaskMethodEnum)Enum.Parse(typeof(MaskMethodEnum), valuestring, true);
                    break;
                case "ExtendX":
                    ExtendX = int.Parse(valuestring);
                    break;
                case "ExtendY":
                    ExtendY = int.Parse(valuestring);
                    break;
                case "RelateASN":
                    RelateASN = valuestring;
                    break;
                case "RelateASNItem":
                    RelateASNItem = valuestring;
                    break;
                case "IsSeed":
                    IsSeed = bool.Parse(valuestring);
                    break;
            }
        }

        public void Reset()
        {
            AliasName = Name;

            Brightness = 0;
            Contrast = 0;
            MaskMethod = MaskMethodEnum.NONE;
            ExtendX = 20;
            ExtendY = 20;

            RelateASN = "None";
            RelateASNItem = "None";

            IsSeed = false;
        }


    }
}
