using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Forms;
using JetEazy.BasicSpace;

namespace JzKHC.ControlSpace
{
    class SideClass
    {
        const int PickLineRange = 3;
        const int PickLineRangeInside = 5;

        public Bitmap bmpBackgroundOrigin = new Bitmap(1, 1);
        public Bitmap bmpAnalyzeOrigin = new Bitmap(1, 1);
        public Bitmap bmpBaseOrigin = new Bitmap(1, 1);

        SideEnum mySide = SideEnum.SIDE0;

        public List<KeybaseClass> KEYBASELIST = new List<KeybaseClass>();
        public List<KeybaseClass> vKEYBASELIST = new List<KeybaseClass>();

        public List<string> KEYBASESEQLIST = new List<string>();
        protected JzToolsClass JzTools = new JzToolsClass();

        public bool[] IsReget = new bool[(int)TeachingTypeEnum.COUNT];
        public bool[] IsRegetAlready = new bool[(int)TeachingTypeEnum.COUNT];

        public string SideInformation = "";
        public SideClass(SideEnum rSide)
        {
            mySide = rSide;
        }

        public void GetBackgroundBMP(Bitmap bmp)
        {
            bmpBackgroundOrigin.Dispose();
            bmpBackgroundOrigin = new Bitmap(bmp);
        }
        public void GetAnalyzeBMP(Bitmap bmp)
        {
            bmpAnalyzeOrigin.Dispose();
            bmpAnalyzeOrigin = new Bitmap(bmp);
        }
        public void GetBaseBMP(Bitmap bmp)
        {
            bmpBaseOrigin.Dispose();
            bmpBaseOrigin = new Bitmap(bmp);
        }
        
        public void ReduceBMP()
        {
            bmpBaseOrigin.Dispose();
            bmpBaseOrigin = new Bitmap(1,1);
            bmpAnalyzeOrigin.Dispose();
            bmpAnalyzeOrigin = new Bitmap(1, 1);
            bmpBackgroundOrigin.Dispose();
            bmpBackgroundOrigin = new Bitmap(1, 1);
        }

        public void ReduceBMP(TeachingTypeEnum teachingtype)
        {
            switch (teachingtype)
            {
                case TeachingTypeEnum.ANALYZE:
                    bmpAnalyzeOrigin.Dispose();
                    bmpAnalyzeOrigin = new Bitmap(1, 1);
                    break;
                case TeachingTypeEnum.BASE:
                    bmpBaseOrigin.Dispose();
                    bmpBaseOrigin = new Bitmap(1, 1);
                    break;
                case TeachingTypeEnum.BACKGROUD:
                    bmpBackgroundOrigin.Dispose();
                    bmpBackgroundOrigin = new Bitmap(1, 1);
                    break;
            }
        }

        public void SaveBackgroundBMP(string FileName)
        {
            Bitmap bmp = new Bitmap(bmpBackgroundOrigin);
            bmp.Save(FileName, ImageFormat.Bmp);
            bmp.Dispose();
        }
        public void SaveAnalyzeBMP(string FileName)
        {
            Bitmap bmp = new Bitmap(bmpAnalyzeOrigin);
            bmp.Save(FileName, ImageFormat.Bmp);
            bmp.Dispose();
        }
        public void SaveBaseBMP(string FileName)
        {
            Bitmap bmp = new Bitmap(bmpBaseOrigin);
            bmp.Save(FileName, ImageFormat.Bmp);
            bmp.Dispose();
        }

        public void ReserveKeybase()
        {
            foreach (KeybaseClass keybase in KEYBASELIST)
            {
                vKEYBASELIST.Add(keybase.Clone());
            }
        }
        public void WriteBackKeybase()
        {
            foreach (KeybaseClass keybase in KEYBASELIST)
            {
                keybase.Dispoe();
            }
            KEYBASELIST.Clear();

            foreach (KeybaseClass keybase in vKEYBASELIST)
            {
                KEYBASELIST.Add(keybase.Clone());
            }

            ClearevKeybase();
        }
        public void ClearevKeybase()
        {
            foreach (KeybaseClass keybase in vKEYBASELIST)
            {
                keybase.Dispoe();
            }
            vKEYBASELIST.Clear();
        }
        public void AddKeybase(Bitmap bmp)
        {
            int Index = -1;
            int i = 0, j = 0;

            foreach (KeybaseClass keybase in vKEYBASELIST)
            {
                if (keybase.IsSelectedStart)
                {
                    Index = i;
                    break;
                }
                i++;
            }

            if (Index == -1)
            {
                vKEYBASELIST.Add(new KeybaseClass(mySide, vKEYBASELIST, bmp));
            }
            else
            {
                KeybaseClass keybasstmp;

                i = 0;
                j = vKEYBASELIST.Count;

                while (i < j)
                {
                    if (vKEYBASELIST[i].IsSelected)
                    {
                        keybasstmp = vKEYBASELIST[i].CloneAdded(int.Parse(JzTools.GetLastString(vKEYBASELIST[vKEYBASELIST.Count - 1].Name, 3)), bmp);
                        vKEYBASELIST.Add(keybasstmp);

                        vKEYBASELIST[i].IsSelected = false;
                        vKEYBASELIST[i].IsSelectedStart = false;
                    }
                    i++;
                }

                //vKEYCAPLIST[Index].IsSelected = false;
                //vKEYCAPLIST[Index].IsSelectedStart = false;

                //KeycapClass keycap = vKEYCAPLIST[Index].CloneAdded(int.Parse(JzTools.GetLastString(vKEYCAPLIST[vKEYCAPLIST.Count - 1].Name, 2)));

                //keycap.IsSelected = true;
                //keycap.IsSelectedStart = true;

                //vKEYCAPLIST.Add(keycap);
            }

        }
        public void DeleteKeybase()
        {
            int Index = -1;
            int i = 0;

            foreach (KeybaseClass keybase in vKEYBASELIST)
            {
                if (keybase.IsSelectedStart)
                {
                    Index = i;
                    break;
                }
                i++;
            }

            if (Index == -1)
            {
                return;
            }
            else
            {
                i = vKEYBASELIST.Count - 1;

                while (i > -1)
                {
                    if (vKEYBASELIST[i].IsSelected)
                    {
                        vKEYBASELIST.RemoveAt(i);
                    }
                    i--;
                }
            }

        }

        public override string ToString()
        {
            string Str = "";

            return Str;
        }

        public void CheckBaseSeqence(Bitmap bmpSource)
        {
            return;

            Bitmap bmp = (Bitmap)bmpSource.Clone();
            KeybaseClass keybase;
            Rectangle rectTmp;
            string[] Str;

            foreach (string keybaseseqstr in KEYBASESEQLIST)
            {
                Str = keybaseseqstr.Split(',');

                if (Str[0] == "B")
                {
                    keybase = KEYBASELIST[int.Parse(Str[2])];
                    keybase.CheckSequential(bmp);

                    rectTmp = keybase.rectFoundBias;
                    rectTmp.Inflate(15, 15);

                    JzTools.DrawRect(bmp, rectTmp, new SolidBrush(Color.FromArgb(keybase.MinGrade, keybase.MinGrade, keybase.MinGrade)));
                }
                else if (Str[0] == "N")
                {
                    keybase = KEYBASELIST[int.Parse(Str[2])];
                    keybase.CheckSequential(bmp);

                    rectTmp = keybase.rectFoundBias;
                    rectTmp.Inflate(15, 15);
                    JzTools.DrawRect(bmp, rectTmp, new SolidBrush(Color.FromArgb(keybase.MinGrade, keybase.MinGrade, keybase.MinGrade)));

                    //bmp.Save(@"D:\LOA\NEWERA\SEQFIND.BMP", ImageFormat.Bmp);
                }
            }

            //bmp.Save(@"D:\LOA\NEWERA\SEQFIND.BMP", ImageFormat.Bmp);

            bmp.Dispose();
        }

        public delegate void ModifyHatHandler();
        public event ModifyHatHandler ModifyHatAction;
        public void OnModifyHat()
        {
            if (ModifyHatAction != null)
            {
                ModifyHatAction();
            }
        }

        public void DisposeBMP()
        {

            foreach (KeybaseClass keybase in KEYBASELIST)
            {
                keybase.Dispoe();
            }
            KEYBASELIST.Clear();

            bmpBackgroundOrigin.Dispose();
            bmpAnalyzeOrigin.Dispose();
            bmpBaseOrigin.Dispose();
        }
        public void DisposeBMP(bool IsKeeping)
        {
            foreach (KeybaseClass keybase in KEYBASELIST)
            {
                keybase.Dispoe();
            }
            KEYBASELIST.Clear();

            if (!IsKeeping)
            {
                bmpBackgroundOrigin.Dispose();
                bmpAnalyzeOrigin.Dispose();
                bmpBaseOrigin.Dispose();
            }
        }
    }
}
