using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JetEazy;
using System.Drawing;

namespace JzMeasure.OPSpace
{
    public class MSRItemXClass
    {
        public int No = 0;
        public string RelateName = "";
        public Bitmap bmpItem = new Bitmap(1, 1);

        public MSRItemXClass()
        {


        }
        public MSRItemXClass(string str)
        {
            FromString(str);
        }
        public void Load(string MSRpath)
        {
            GetBMP(MSRpath + "\\I" + No.ToString(MSRClass.OrgMSRNoString) + ".png", ref bmpItem);
        }
        public void Save(string MSRpath)
        {
            SaveBMP(MSRpath + "\\I" + No.ToString(MSRClass.OrgMSRNoString) + ".png", ref bmpItem);
        }

        public MSRItemXClass Clone()
        {
            MSRItemXClass newMSRitem = new MSRItemXClass(this.ToString());

            newMSRitem.bmpItem.Dispose();
            newMSRitem.bmpItem = new Bitmap(bmpItem);

            return newMSRitem;
        }
        public override string ToString()
        {
            char seperator = Universal.SeperateCharA;
            string str = "";

            str += No.ToString() + seperator;   //0
            str += RelateName + seperator;      //1
            str += "";

            return str;
        }
        public void FromString(string str)
        {
            char seperator = Universal.SeperateCharA;
            string [] strs = str.Split(seperator);

            No = int.Parse(strs[0]);
            RelateName = strs[1];
        }

        public void Suicide()
        {
            bmpItem.Dispose();
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
