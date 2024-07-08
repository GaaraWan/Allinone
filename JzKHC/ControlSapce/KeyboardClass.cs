using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

using JetEazy.BasicSpace;
using JzKHC.AOISpace;
using JetEazy;

namespace JzKHC.ControlSpace
{
    class KeyboardClass
    {
        //CCDClass CCD
        //{
        //    get
        //    {
        //        return Universal.CCD;
        //    }
        //}
        
        public string Name = "KeyboardName";
        public string VERSION = "US";
        public int ID = 1;

        public string LastControlString = "";

        public Bitmap bmpKeyboard = new Bitmap(1, 1);
        public Bitmap bmpOrigin = new Bitmap(1, 1);
        
        public List<KeyAssignClass> KEYASSIGNLIST = new List<KeyAssignClass>();
        public List<KeyAssignClass> vKEYASSIGNLIST = new List<KeyAssignClass>();
        protected JzToolsClass JzTools = new JzToolsClass();

        public Rectangle rectKeyboardRange = new Rectangle();

        public Rectangle rectKeyboardRangeWithVirtualRatio
        {
            get
            {
                return JzTools.Resize(rectKeyboardRange, -2);
            }
        }

        public SideClass[] SIDES = new SideClass[(int)SideEnum.COUNT];

        public KeyboardClass()
        {
            int i = 0;
            while (i < (int)SideEnum.COUNT)
            {
                SIDES[i] = new SideClass((SideEnum)i);
                i++;
            }
        }

        public void GetBackgroudBMP(SideEnum rSide, string FileName)
        {
            Bitmap bmp = new Bitmap(FileName);
            SIDES[(int)rSide].GetBackgroundBMP(bmp);
            bmp.Dispose();
        }
        public void GetAnalyzeBMP(SideEnum rSide, string FileName)
        {
            Bitmap bmp = new Bitmap(FileName);
            SIDES[(int)rSide].GetAnalyzeBMP(bmp);
            bmp.Dispose();
        }
        public void GetBaseBMP(SideEnum rSide, string FileName)
        {
            Bitmap bmp = new Bitmap(FileName);
            SIDES[(int)rSide].GetBaseBMP(bmp);
            bmp.Dispose();
        }
        public void GetBaseBMP(SideEnum rSide, Bitmap bmp)
        {
            SIDES[(int)rSide].GetBaseBMP(bmp);
        }

        public void ClearBaseBMP(SideEnum rSide)
        {
            SIDES[(int)rSide].bmpBaseOrigin.Dispose();
            SIDES[(int)rSide].bmpBaseOrigin = new Bitmap(1, 1);
        }
        public void GetSideBMP(SideEnum rSide, Bitmap bmp)
        {
            SIDES[(int)rSide].GetBackgroundBMP(bmp);
        }
        
        public void SaveBackgroudBMP(SideEnum rSide,string FileName)
        {
            SIDES[(int)rSide].SaveBackgroundBMP(FileName);
        }
        public void ClearBackgroudBMP(SideEnum rSide)
        {
            SIDES[(int)rSide].bmpBackgroundOrigin.Dispose();
            SIDES[(int)rSide].bmpBackgroundOrigin = new Bitmap(1, 1);
        }

        public void SaveAnalyzeBMP(SideEnum rSide, string FileName)
        {
            SIDES[(int)rSide].SaveAnalyzeBMP(FileName);
        }
        public void SaveBaseBMP(SideEnum rSide, string FileName)
        {
            SIDES[(int)rSide].SaveBaseBMP(FileName);
        }

        public void GetKeyboardBMP(string FileName)
        {
            Bitmap bmp = new Bitmap(FileName);
            bmpKeyboard.Dispose();
            bmpKeyboard = new Bitmap(bmp);

            bmp.Dispose();
        }
        public void SetKeyboardBMP(Bitmap bmp,Rectangle rect)
        {
            if (!(rect.Width == 1 || rect.Height == 1))
            {
                bmpKeyboard.Dispose();
                bmpKeyboard = (Bitmap)bmp.Clone(rect, PixelFormat.Format32bppPArgb);
            }
        }
        public void SaveKeyboardBMP(string FileName)
        {
            Bitmap bmp = new Bitmap(bmpKeyboard);
            bmp.Save(FileName, ImageFormat.Bmp);
            bmp.Dispose();
        }

        public void GetOrigionBMP(string FileName)
        {
            Bitmap bmp = new Bitmap(FileName);
            bmpOrigin.Dispose();
            bmpOrigin = new Bitmap(bmp);

            bmp.Dispose();
        }
        public void SaveOriginBMP(string FileName)
        {
            Bitmap bmp = new Bitmap(bmpOrigin);
            bmp.Save(FileName, ImageFormat.Bmp);
            bmp.Dispose();
        }
        public void SetOriginBMP(Bitmap bmp)
        {
            bmpOrigin.Dispose();
            bmpOrigin = (Bitmap)bmp.Clone();
        }

        public void ReserveKeyassign()
        {
            foreach (KeyAssignClass keyassign in KEYASSIGNLIST)
            {
                vKEYASSIGNLIST.Add(keyassign.Clone());
            }
        }
        public void WriteBackKeyassign()
        {
            foreach (KeyAssignClass keyassign in KEYASSIGNLIST)
            {
                keyassign.Dispoe();
            }
            KEYASSIGNLIST.Clear();

            foreach (KeyAssignClass keyassign in vKEYASSIGNLIST)
            {
                KEYASSIGNLIST.Add(keyassign.Clone());
            }

            ClearevKeyassign();
        }
        public void ClearevKeyassign()
        {
            foreach (KeyAssignClass keyassign in vKEYASSIGNLIST)
            {
                keyassign.Dispoe();
            }
            vKEYASSIGNLIST.Clear();
        }
        public void AddKeyassign(Bitmap bmp)
        {
            int Index = -1;
            int i = 0, j = 0;

            foreach (KeyAssignClass keyassign in vKEYASSIGNLIST)
            {
                if (keyassign.IsSelectedStart)
                {
                    Index = i;
                    break;
                }
                i++;
            }

            if (Index == -1)
            {
                vKEYASSIGNLIST.Add(new KeyAssignClass(vKEYASSIGNLIST, bmp));
            }
            else
            {
                KeyAssignClass keyassigntmp;

                i = 0;
                j = vKEYASSIGNLIST.Count;

                while (i < j)
                {
                    if (vKEYASSIGNLIST[i].IsSelected)
                    {
                        keyassigntmp = vKEYASSIGNLIST[i].CloneAdded(int.Parse(JzTools.GetLastString(vKEYASSIGNLIST[vKEYASSIGNLIST.Count - 1].Name, 3)), bmp);
                        vKEYASSIGNLIST.Add(keyassigntmp);

                        vKEYASSIGNLIST[i].IsSelected = false;
                        vKEYASSIGNLIST[i].IsSelectedStart = false;
                    }
                    i++;
                }

                //vKEYCAPLIST[Index].IsSelected = false;
                //vKEYCAPLIST[Index].IsSelectedStart = false;

                //KeyAssignClass keycap = vKEYCAPLIST[Index].CloneAdded(int.Parse(JzTools.GetLastString(vKEYCAPLIST[vKEYCAPLIST.Count - 1].Name, 2)));

                //keycap.IsSelected = true;
                //keycap.IsSelectedStart = true;

                //vKEYCAPLIST.Add(keycap);
            }

        }
        public void DeleteKeyassign()
        {
            int Index = -1;
            int i = 0;

            foreach (KeyAssignClass keyassign in vKEYASSIGNLIST)
            {
                if (keyassign.IsSelectedStart)
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
                i = vKEYASSIGNLIST.Count - 1;

                while (i > -1)
                {
                    if (vKEYASSIGNLIST[i].IsSelected)
                    {
                        vKEYASSIGNLIST.RemoveAt(i);
                    }
                    i--;
                }
            }

        }

        public void GetKeyAssignBesides()
        {
            int i = 0, j = 0;
            KeyAssignClass keyassign;
            Rectangle RectTmp = new Rectangle();

            List<int> ListIntersects = new List<int>();

            //PropertyFilterClass HeightFilter = new PropertyFilterClass();

            //i = 0;

            //HeightFilter.Initial(10, 1000, 10);

            //while (i < KEYASSIGNLIST.Count)
            //{
            //    HeightFilter.Add(KEYASSIGNLIST[i].myrect.Height);
            //    i++;
            //}

            //HeightFilter.Complete();

            #region Get Center Besides

            i = 0;

            while (i < KEYASSIGNLIST.Count)
            {
                keyassign = KEYASSIGNLIST[i];
                keyassign.InitialBesides();

                if (keyassign.AliasName == "F4")
                    keyassign.AliasName = keyassign.AliasName;

                RectTmp = keyassign.myrect;

                int EnlargeValue = Math.Min(keyassign.myrect.Width,keyassign.myrect.Height);

                if (keyassign.myrect.Height < (keyassign.myrect.Width >> 1))
                {
                    EnlargeValue = (int)((double)EnlargeValue * 1.6d);
                }
                else
                {
                    EnlargeValue = (int)((double)EnlargeValue * 0.8d);
                }


                //EnlargeValue = HeightFilter.Mode >> 1;
                RectTmp.Inflate(EnlargeValue, EnlargeValue);

                ListIntersects.Clear();
                //檢查有相交的鍵帽
                j = 0;
                while (j < KEYASSIGNLIST.Count)
                {
                    if (j != i)
                    {
                        if (RectTmp.IntersectsWith(KEYASSIGNLIST[j].myrect))
                        {
                            ListIntersects.Add(j);
                        }
                    }
                    j++;
                }
                //尋找四週的鍵帽
                BesideEnum Beside = BesideEnum.NONE;

                j = 0;
                while (j < ListIntersects.Count)
                {
                    Beside = keyassign.MeetBeside(KEYASSIGNLIST[ListIntersects[j]].myrect);
                    if (Beside != BesideEnum.NONE)
                    {
                        keyassign.ListBeside[(int)Beside].Add(ListIntersects[j]);
                    }
                    j++;
                }
                i++;
            }

            #endregion

        }
        public void GetKeyAssignCornerBesides()
        {
            return;

            Bitmap bmpAdjCorner = new Bitmap(bmpKeyboard);
            //Bitmap bmpAdjCenter = new Bitmap(RECIPEDB.bmpKeyboard);

            int MinLength = 0;
            int VeryMinLength = 0;

            Rectangle rectFrom = new Rectangle();
            Rectangle rectTo = new Rectangle();

            List<string> PositionStrList = new List<string>();
            List<Pen> DrawPenList = new List<Pen>();

            DrawPenList.Add(new Pen(Color.Red, 2));
            //DrawPenList[0].DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            DrawPenList.Add(new Pen(Color.Red, 2));
            //DrawPenList[1].DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

            int i = 0;
            int j = 0;
            string RectName = "";

            #region Adjacent Reference

            foreach (KeyAssignClass keyassign in KEYASSIGNLIST)
            {
                RectName = keyassign.Name;
                i = 0;
                while (i < (int)CornerEnum.COUNT)
                {
                    MinLength = Math.Min(keyassign.myrect.Width, keyassign.myrect.Height);

                    rectFrom = JzTools.CornerRect(keyassign.myrect, (CornerEnum)i, 1);
                    PositionStrList.Clear();

                    foreach (KeyAssignClass keyassignx in KEYASSIGNLIST)
                    {
                        if (keyassignx.Name == RectName)
                        {
                            continue;
                        }

                        Rectangle rect = rectFrom;

                        rect.Inflate(MinLength, MinLength);

                        j = 0;
                        while (j < (int)CornerEnum.COUNT)
                        {
                            if (rect.IntersectsWith(JzTools.CornerRect(keyassignx.myrect, (CornerEnum)j, 2)))
                            {
                                VeryMinLength = Math.Min(keyassignx.myrect.Width, keyassignx.myrect.Height);
                                VeryMinLength = Math.Min(VeryMinLength, MinLength);

                                if (JzTools.GetPointLength(JzTools.GetRectCenter(rect), JzTools.GetRectCenter(JzTools.CornerRect(keyassignx.myrect, (CornerEnum)j, 2))) < VeryMinLength)
                                {
                                    PositionStrList.Add(
                                        JzTools.GetPointLength(JzTools.GetRectCenter(rect), JzTools.GetRectCenter(JzTools.CornerRect(keyassignx.myrect, (CornerEnum)j, 2))).ToString("00000") + "@" +
                                        JzTools.PointtoString(JzTools.GetRectCenter(rect)) + "@" +
                                        JzTools.PointtoString(JzTools.GetRectCenter(JzTools.CornerRect(keyassignx.myrect, (CornerEnum)j, 2))) + "@" +
                                        keyassign.Name + "@" +
                                        i.ToString() + "@" + 
                                        keyassignx.Name + "@" + 
                                        j.ToString());
                                }
                            }
                            //JzTools.DrawLine(bmpAdjCorner,new Pen(Color.Red,2),JzTools.GetRectCenter(rect),JzTools.GetRectCenter(JzTools.CornerRect(keyassignx.myrect,(CornerEnum)i,2)));
                            j++;
                        }
                    }

                    PositionStrList.Sort();

                    if (PositionStrList.Count > 2)
                    {
                        j = 0;

                        while (j < 2)
                        {
                            string[] posstrs = PositionStrList[j].Split('@');

                            JzTools.DrawLine(bmpAdjCorner, DrawPenList[j], StringtoPoint(posstrs[1]), StringtoPoint(posstrs[2]));
                            j++;
                        }
                    }
                    else
                    {
                        j = 0;
                        foreach (string str in PositionStrList)
                        {
                            string[] posstrs = str.Split('@');

                            JzTools.DrawLine(bmpAdjCorner, DrawPenList[j], StringtoPoint(posstrs[1]), StringtoPoint(posstrs[2]));
                            j++;
                        }
                    }
                    i++;
                }
            }

            bmpAdjCorner.Save(@"D:\LOA\ADJCorner.BMP", ImageFormat.Bmp);

            #endregion


        }


        public void GetKeybaseName()
        {
            int i = 0, j = 0;
            
            int cdfIndex = 0;
            bool IsRelateOK = false;
            string DeleteIndexStr = "";
            string[] Str;
            
            while (i < INI.SIDECOUNT)
            {
                SideClass side = SIDES[i];

                foreach (KeybaseClass keybase in side.KEYBASELIST)
                {
                    cdfIndex = 0;
                    DeleteIndexStr = "";

                    foreach (CornerDefineClass cdf in keybase.CornerDefinedList)
                    {
                        IsRelateOK = false;
                        foreach (KeyAssignClass keyassign in KEYASSIGNLIST)
                        {
                            if (cdf.Name == keyassign.Name)
                            {
                                cdf.AliasName = keyassign.AliasName;
                                IsRelateOK = true;
                            }
                        }

                        if (!IsRelateOK)
                        {
                            DeleteIndexStr += cdfIndex.ToString() + ",";
                        }
                        cdfIndex++;
                    }

                    if (DeleteIndexStr.Length > 0)
                    {
                        Str = JzTools.RemoveLastChar(DeleteIndexStr, 1).Split(',');
                        j = Str.Length - 1;

                        while (j > -1)
                        {
                            keybase.CornerDefinedList.RemoveAt(int.Parse(Str[j]));
                            j--;
                        }
                    }
                }
                i++;
            }




        }
        public void GetKeyAssignRelateBase()
        {
            int i = 0;
            int BaseIndex = 0;

            foreach (KeyAssignClass keyassign in KEYASSIGNLIST)
            {

                keyassign.outBaseIndicator = new BaseIndicatorClass[(int)CornerExEnum.COUNT];
                keyassign.inBaseIndicator = new BaseIndicatorClass[(int)CornerExEnum.COUNT];

                i = 0;
                while (i < INI.SIDECOUNT)
                {
                    SideClass side = SIDES[i];
                    BaseIndex = 0;

                    foreach (KeybaseClass keybase in side.KEYBASELIST)
                    {
                        foreach (CornerDefineClass cdf in keybase.CornerDefinedList)
                        {
                            if (cdf.Name == keyassign.Name)
                            {
                                if (keybase.IsFromBase)
                                {
                                    keyassign.outBaseIndicator[(int)cdf.IndicateCornerEx] = new BaseIndicatorClass();
                                    keyassign.outBaseIndicator[(int)cdf.IndicateCornerEx].mySide = (SideEnum)i;
                                    keyassign.outBaseIndicator[(int)cdf.IndicateCornerEx].Index = BaseIndex;
                                }
                                else
                                {
                                    keyassign.inBaseIndicator[(int)cdf.IndicateCornerEx] = new BaseIndicatorClass();
                                    keyassign.inBaseIndicator[(int)cdf.IndicateCornerEx].mySide = (SideEnum)i;
                                    keyassign.inBaseIndicator[(int)cdf.IndicateCornerEx].Index = BaseIndex;
                                }
                            }
                        }

                        BaseIndex++;
                    }
                    i++;
                }
            }


        }

        public void GetKeybaseCheckingSequence(int SideIndex)
        {
            int j = 0;

            SideClass side = SIDES[SideIndex];

            side.KEYBASESEQLIST.Clear();
            j = 0;
            foreach (KeybaseClass keybase in side.KEYBASELIST)
            {
                if (!keybase.IsCalibration)
                    side.KEYBASESEQLIST.Add((keybase.IsFromBase ? "B" : "N") + "," + keybase.rectFoundBias.Y.ToString("00000") + "," + j.ToString("000") + "," + keybase.ToSeqString());
                    //side.KEYBASESEQLIST.Add((keybase.IsFromBase ? "B" : "N") + "," + keybase.myrect.Y.ToString("00000") + "," + j.ToString("000") + "," + keybase.ToSeqString());

                j++;
            }

            side.KEYBASESEQLIST.Sort();
            side.CheckBaseSeqence(side.bmpBaseOrigin);
        }


        public void DisposeBMP()
        {
            int i = 0;
            while (i < INI.SIDECOUNT)
            {
                SideClass side = SIDES[i];
                side.DisposeBMP();
                i++;
            }

            foreach (KeyAssignClass keyassign in KEYASSIGNLIST)
            {
                keyassign.Dispoe();
            }

            KEYASSIGNLIST.Clear();

            bmpKeyboard.Dispose();
            bmpOrigin.Dispose();
        }
        public void DisposeBMP(bool IsKeeping)
        {
            int i = 0;
            while (i < INI.SIDECOUNT)
            {
                SideClass side = SIDES[i];
                side.DisposeBMP();
                i++;
            }

            foreach (KeyAssignClass keyassign in KEYASSIGNLIST)
            {
                keyassign.Dispoe();
            }

            KEYASSIGNLIST.Clear();

            bmpKeyboard.Dispose();
            bmpOrigin.Dispose();
        }
        Point StringtoPoint(string PtString)
        {
            string[] str = PtString.Split(',');
            return new Point(int.Parse(str[0]), int.Parse(str[1]));
        }

    }
}
