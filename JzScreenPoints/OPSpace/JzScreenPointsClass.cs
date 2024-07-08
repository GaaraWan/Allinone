#define RPI

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using JzScreenPoints.OPSpace;
using JzScreenPoints.BasicSpace;
using JzScreenPoints.ControlSpace;
using System.IO;
using System.Drawing.Imaging;
using JzScreenPoints;
using System.Windows.Forms;

using JzScreenPoints.Interface;
using JetEazy.AH;

namespace JzScreenPoints.OPSpace
{
    public class JzScreenPointsClass
    {
#if (!RPI)
        public FormShow m_showmypoints;
#else
        public IRpiDriver m_showmypoints;
#endif

        private int m_stepcount = 7;
        public List<JzScreenItemClass> m_JzScreenList = new List<JzScreenItemClass>();

        public int StepCount
        {
            get
            {
                return m_stepcount;
            }
            set
            {
                m_stepcount = value;
            }
        }

        public bool IsNoUseMotion
        {
            get { return INI.Allinone_IsNoUseMotion; }
        }
        public JzScreenPointsClass(int iStepCount)
        {
            m_stepcount = iStepCount;
        }
        public void Initial()
        {
            INI.Initial();

            //Point ptStart = new Point(420, 350);
            //Point ptEnd = new Point(840, 630);
#if (!RPI)
             m_showmypoints = new FormShow(1, INI.Allinone_Pt_Start, INI.Allinone_Pt_End, INI.Allinone_Offset, INI.Allinone_Offset);
            m_showmypoints.Location = new Point(Screen.AllScreens[0].Bounds.Width, 0);
            m_showmypoints.Show();
#else
            m_showmypoints = new AgentClient();
            AllinoneCalibrate();
#endif
            Load();
        }

        public void AllinoneCalibrate()
        {

            //Point ptStart = new Point(690, 320);// INI.Allinone_Pt_Start;
            //Point ptEnd = new Point(1380, 740);
            //int ioffsetx = INI.Allinone_Offset;
            //int ioffsety = INI.Allinone_Offset;
            Point ptStart = INI.Allinone_Pt_Start;
            Point ptEnd = INI.Allinone_Pt_End;
            int ioffsetx = INI.Allinone_Offset;
            int ioffsety = INI.Allinone_Offset;
            List<Point> pt_Mouse_location_list = new List<Point>();

            for (int i = ptStart.Y; i < ptEnd.Y; i += ioffsety)
            {
                for (int j = ptStart.X; j < ptEnd.X; j += ioffsetx)
                {
                    Point pttmp = new Point(j, i);
                    pt_Mouse_location_list.Add(pttmp);
                }
            }
            m_showmypoints.DrawMyPaints(pt_Mouse_location_list);
        }
        public void Load()
        {
            m_JzScreenList.Clear();
            int i = 0;
            while (i <= m_stepcount)
            {
                JzScreenItemClass item = new JzScreenItemClass();
                item.m_step = i;
                item.Load(Universal.JSPCollectionPath + "\\0000");
                m_JzScreenList.Add(item);
                //item.Save(Universal.JSPCollectionPath + "\\0000");
                i++;
            }
        }
        public void Save()
        {
            foreach(JzScreenItemClass item in m_JzScreenList)
            {
                item.Save(Universal.JSPCollectionPath + "\\0000");
            }
        }
    }

    public class JzScreenItemClass
    {
        public JzScreenItemClass()
        {

        }

        const string itembasis = "&";
        const string itempoints = "$";
        const string item = "#";
        public string name = "step";
        public int m_step = 0;
        public float m_pos = 0f;

        public Bitmap bmpORG = new Bitmap(1, 1);
        public Bitmap bmpOPERATE = new Bitmap(1, 1);

        public List<BASISClass> BasisList = new List<BASISClass>();
        public List<Point> PointList = new List<Point>();
        
        public void Load(string m_datapath)
        {
            if (bmpORG != null)
                bmpORG.Dispose();
            if (bmpOPERATE != null)
                bmpOPERATE.Dispose();

            bmpORG = new Bitmap(1, 1);
            bmpOPERATE = new Bitmap(1, 1);

            JzTools.GetBMP(m_datapath + "\\" + m_step.ToString("000") + Universal.GlobalImageTypeString, ref bmpORG);
            //JzTools.GetBMP(m_datapath + "\\" + m_step.ToString("000") + Universal.GlobalImageTypeString, ref bmpOPERATE);

            bmpOPERATE = new Bitmap(bmpORG);

            string Str = "";
            JzTools.ReadData(ref Str, m_datapath + "\\DataScreen" + m_step.ToString() + ".jdb");
            if (Str != "")
                FormString(Str);
        }
        public void Save(string m_datapath)
        {
            if (!Directory.Exists(m_datapath))
                Directory.CreateDirectory(m_datapath);

            JzTools.SaveBMP(m_datapath + "\\" + m_step.ToString("000") + Universal.GlobalImageTypeString, ref bmpORG, Universal.GlobalImageFormat);

            string Str = ToString();

            JzTools.SaveData(Str, m_datapath + "\\DataScreen" + m_step.ToString() + ".jdb");
        }

        public void AutoFindPoints(Bitmap m_bmp)
        {
            BasisList.Clear();
            //PointList.Clear();

            int offsetx = 162;
            int offsety = 1560;

            Bitmap bmptmp = new Bitmap(m_bmp);
            Bitmap bmp = new Bitmap(1, 1);// (Bitmap)bmptmp.Clone(new Rectangle(offsetx, 0, bmptmp.Width - 1 - 200, bmptmp.Height - 1), PixelFormat.Format24bppRgb);
            bmp.Dispose();
            m_bmp.Dispose();

            if (m_step == 5)
                bmp = (Bitmap)bmptmp.Clone(new Rectangle(offsetx, offsety, bmptmp.Width - 1 - 2 * offsetx, bmptmp.Height - 1 - offsety), PixelFormat.Format24bppRgb);
            else
                bmp = (Bitmap)bmptmp.Clone(new Rectangle(offsetx, 0, bmptmp.Width - 1 - 2 * offsetx, bmptmp.Height), PixelFormat.Format24bppRgb);

            HistogramClass Histogram = new HistogramClass(2);
            JzFindObjectClass FindObject = new JzFindObjectClass();

            if (!Directory.Exists(@"D:\LOA\NEWERA\"))
                Directory.CreateDirectory(@"D:\LOA\NEWERA\");

            JzTools.SetBrightContrast(bmp, -70, 10);

            bmp.Save(@"D:\LOA\NEWERA\DiplayAutoFind-0.BMP", System.Drawing.Imaging.ImageFormat.Bmp);

            Histogram.GetHistogram(bmp);
            FindObject.SetThreshold(bmp, JzTools.SimpleRect(bmp.Size), Histogram.MeanGrade * 6, 255, 0, true);

            bmp.Save(@"D:\LOA\NEWERA\DiplayAutoFind-1.BMP", System.Drawing.Imaging.ImageFormat.Bmp);

            FindObject.Find(bmp, Color.Red);

            bmp.Save(@"D:\LOA\NEWERA\DiplayAutoFind-2.BMP", System.Drawing.Imaging.ImageFormat.Bmp);
            foreach (FoundClass found in FindObject.FoundList)
            {
                if (found.rect.Width > 30 && found.rect.Height > 30 
                    //&& found.rect.Width < 100 && found.rect.Height < 100
                    )
                {
                    Rectangle rect = found.rect;
                    rect.X += offsetx;
                    if (m_step == 5)
                        rect.Y += offsety;

                    rect.Inflate(5, 5);
                    rect.Intersect(JzTools.SimpleRect(new Size(4912, 3684)));
                    BASISClass basis = new BASISClass(rect, 0, 0);
                    BasisList.Add(basis);

                    Point ptCenter = JzTools.GetRectCenter(rect);
                    //MOD GAARA
                    Point ptCenterEx = ptCenter;// JzTools.PointFToPoint(_dragonDrv.SIDEList[cboCamList.SelectedIndex].ViewToWorldEx(ptCenter));

                    //ptCenterEx.X += INI.Allinone_Offset_ADDX;
                    //ptCenterEx.Y += INI.Allinone_Offset_ADDY;

                    //PointList.Add(ptCenterEx);
                }
            }
            bmp.Dispose();
        }
        public void FormString(string Str)
        {
            if (Str != "")
            {
                string[] strs = Str.Split('#');
                name = strs[0];
                m_step = int.Parse(strs[1]);
                m_pos = float.Parse(strs[2]);

                if (strs.Length > 3)
                {
                    BasisList.Clear();
                    string[] basisstrs = strs[3].Split('&');
                    foreach (string str in basisstrs)
                    {
                        BASISClass basis = new BASISClass(str);
                        BasisList.Add(basis);
                    }
                }
                if (strs.Length > 4)
                {
                    PointList.Clear();
                    string[] pointstrs = strs[4].Split('$');
                    foreach (string str in pointstrs)
                    {
                        Point pt = JzTools.StringToPoint(str);

                        //pt.X += INI.Allinone_Offset_ADDX;
                        //pt.Y += INI.Allinone_Offset_ADDY;

                        PointList.Add(pt);
                    }
                }
            }
        }
        public override string ToString()
        {
            string str = "";

            str += name + item;//0
            str += m_step.ToString("000") + item;//1
            str += m_pos.ToString("0.000") + item;//2

            PointList.Clear();
            foreach (BASISClass basis in BasisList)
            {
                PointList.Add(JzTools.GetRectCenter(basis.Cell.RectCell));
                str += basis.ToString() + itembasis;
            }
            str = JzTools.RemoveLastChar(str, 1);//3

            str += item;
            foreach (Point pt in PointList)
            {
                str += JzTools.PointToString(pt) + itempoints;
            }
            str = JzTools.RemoveLastChar(str, 1);//4
            //str += item;

            return str;
        }
    }
}
