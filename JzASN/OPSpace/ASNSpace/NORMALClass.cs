using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetEazy.BasicSpace;

using JetEazy;
using JzDisplay;

namespace JzASN.OPSpace.ASNSpace
{
    public class NORMALClass
    {
        public string Name = "";
        public string AliasName = "";
        public ShapeEnum Shape = ShapeEnum.RECT;

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
            str += Shape.ToString() + seperator;      //2
            str += "";

            return str;
        }
        public void FromString(string str)
        {
            char seperator = Universal.SeperateCharB;
            string[] strs = str.Split(seperator);

            Name = strs[0];
            AliasName = strs[1];

            if(str.Length > 3)
                Shape = (ShapeEnum)Enum.Parse(typeof(ShapeEnum), strs[2], true);
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
                case "Shape":
                    Shape = (ShapeEnum)Enum.Parse(typeof(ShapeEnum), valuestring, true);
                    break;
            }
        }

        public void Reset()
        {
            AliasName = Name;
            Shape = ShapeEnum.RECT;
        }


    }
}
