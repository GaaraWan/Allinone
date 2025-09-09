using MoveGraphLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldOfMoveableObjects;

namespace Allinone.FormSpace.BasicPG
{
    public class PADG2Class
    {
        char SeperateCharG = '\xA6';
        public Mover myMover = new Mover();

        public PADG2Class() { }
        public PADG2Class(string name) { }

        public Bitmap bmpMask = new Bitmap(1, 1);
        public Bitmap bmpRun = new Bitmap(1, 1);
        //public Bitmap bmpRun = new Bitmap(1, 1);

        [Category("定位参数")]
        [DisplayName("角度")]
        [Description("即在此角度范围内寻找")]
        public float Rotation { get; set; } = 15;
        [Category("定位参数")]
        [DisplayName("相似度")]
        [Description("即前后比较的相似度 小于此值则NG")]
        public float Tolerance { get; set; } = 0.5f;

        [Category("判断异常参数")]
        [DisplayName("二值化阈值")]
        [Description("即黑白图像的处理")]
        public int ThresholdValue { get; set; } = 128;
        [Category("判断异常参数")]
        [DisplayName("找白色")]
        [Description("找区域内的白色斑点")]
        public bool IsWhite { get; set; } = true;
        [Category("判断异常参数")]
        [DisplayName("异常最小值")]
        //[Description("二值化阈值")]
        [Browsable(false)]
        public int blobMin { get; set; } = 100;
        [Category("判断异常参数")]
        [DisplayName("异常最大值")]
        //[Description("二值化阈值")]
        [Browsable(false)]
        public int blobMax { get; set; } = 60000;
        [Category("判断异常参数")]
        [DisplayName("缺陷百分比")]
        [Description("即缺陷占寻找区域的比例 大于此比例则NG")]
        public float blobRatio { get; set; } = 0.5f;

        public void FromString(string eStr)
        {
            string[] strings = eStr.Split(SeperateCharG);
            if (strings.Length > 6)
            {
                Rotation = float.Parse(strings[0]);
                Tolerance = float.Parse(strings[1]);
                ThresholdValue = int.Parse(strings[2]);
                IsWhite = strings[3] == "1";
                blobMin = int.Parse(strings[4]);
                blobMax = int.Parse(strings[5]);
                FromMoverString(strings[6]);
            }
            if (strings.Length > 7)
            {
                blobRatio = float.Parse(strings[7]);
                
            }
        }
        public string ToParaString()
        {
            string str = string.Empty;

            str += Rotation.ToString() + SeperateCharG;
            str += Tolerance.ToString() + SeperateCharG;
            str += ThresholdValue.ToString() + SeperateCharG;
            str += (IsWhite ? "1" : "0") + SeperateCharG;
            str += blobMin.ToString() + SeperateCharG;
            str += blobMax.ToString() + SeperateCharG;
            str += ToMoverString() + SeperateCharG;
            str += blobRatio.ToString();
            return str;
        }

        Color DefaultColor = Color.FromArgb(0, Color.Red);
        string ToMoverString()
        {
            string retstr = "";
            char seperator = Universal.SeperateCharD;

            GraphicalObject grobj;

            for (int i = 0; i < myMover.Count; i++)
            {
                grobj = myMover[i].Source;

                if (grobj is JzRectEAG)
                {
                    retstr += (grobj as JzRectEAG).ToString() + seperator;
                }
                else if (grobj is JzCircleEAG)
                {
                    retstr += (grobj as JzCircleEAG).ToString() + seperator;
                }
                else if (grobj is JzPolyEAG)
                {
                    retstr += (grobj as JzPolyEAG).ToString() + seperator;
                }
                else if (grobj is JzRingEAG)
                {
                    retstr += (grobj as JzRingEAG).ToString() + seperator;
                }
                else if (grobj is JzStripEAG)
                {
                    retstr += (grobj as JzStripEAG).ToString() + seperator;
                }
                else if (grobj is JzIdentityHoleEAG)
                {
                    retstr += (grobj as JzIdentityHoleEAG).ToString() + seperator;
                }
                else if (grobj is JzCircleHoleEAG)
                {
                    retstr += (grobj as JzCircleHoleEAG).ToString() + seperator;
                }
            }
            if (retstr != "")
                retstr = retstr.Substring(0, retstr.Length - 1);

            return retstr;
        }
        void FromMoverString(string fromstr)
        {
            int i = 0;
            char seperator = Universal.SeperateCharD;
            string[] strs = fromstr.Split(seperator);
            int No = 0;
            int Level = 2;
            //SetDefaultColor(Level);

            foreach (string str in strs)
            {
                if (str.IndexOf(Figure_EAG.Rectangle.ToString()) > -1)
                {
                    JzRectEAG jzrect = new JzRectEAG(str, DefaultColor);

                    jzrect.RelateNo = No;
                    jzrect.RelatePosition = i;
                    jzrect.RelateLevel = Level;

                    myMover.Add(jzrect);
                }
                else if (str.IndexOf(Figure_EAG.Circle.ToString()) > -1)
                {
                    JzCircleEAG jzcircle = new JzCircleEAG(str, DefaultColor);

                    jzcircle.RelateNo = No;
                    jzcircle.RelatePosition = i;
                    jzcircle.RelateLevel = Level;

                    myMover.Add(jzcircle);
                }
                else if (str.IndexOf(Figure_EAG.ChatoyantPolygon.ToString()) > -1)
                {
                    JzPolyEAG jzpoly = new JzPolyEAG(str, DefaultColor);

                    jzpoly.RelateNo = No;
                    jzpoly.RelatePosition = i;
                    jzpoly.RelateLevel = Level;

                    myMover.Add(jzpoly);
                }
                else if (str.IndexOf(Figure_EAG.Ring.ToString()) > -1 || str.IndexOf(Figure_EAG.ORing.ToString()) > -1)
                {
                    JzRingEAG jzring = new JzRingEAG(str, DefaultColor);

                    jzring.RelateNo = No;
                    jzring.RelatePosition = i;
                    jzring.RelateLevel = Level;

                    myMover.Add(jzring);
                }
                else if (str.IndexOf(Figure_EAG.Strip.ToString()) > -1)
                {
                    JzStripEAG jzstrip = new JzStripEAG(str, DefaultColor);

                    jzstrip.RelateNo = No;
                    jzstrip.RelatePosition = i;
                    jzstrip.RelateLevel = Level;

                    myMover.Add(jzstrip);
                }
                else if (str.IndexOf(Figure_EAG.RectRect.ToString()) > -1 || str.IndexOf(Figure_EAG.HexHex.ToString()) > -1)
                {
                    JzIdentityHoleEAG jzidentityhole = new JzIdentityHoleEAG(str, DefaultColor);

                    jzidentityhole.RelateNo = No;
                    jzidentityhole.RelatePosition = i;
                    jzidentityhole.RelateLevel = Level;

                    myMover.Add(jzidentityhole);
                }
                else if (str.IndexOf(Figure_EAG.RectO.ToString()) > -1 || str.IndexOf(Figure_EAG.HexO.ToString()) > -1)
                {
                    JzCircleHoleEAG jzcirclehole = new JzCircleHoleEAG(str, DefaultColor);

                    jzcirclehole.RelateNo = No;
                    jzcirclehole.RelatePosition = i;
                    jzcirclehole.RelateLevel = Level;

                    myMover.Add(jzcirclehole);
                }

                i++;
            }
        }
    }
}
