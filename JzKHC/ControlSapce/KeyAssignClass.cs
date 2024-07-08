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
    class BaseIndicatorClass
    {
        public SideEnum mySide = SideEnum.SIDE0;
        public int Index = 0;
        
        public BaseIndicatorClass()
        {

        }
        public BaseIndicatorClass(string InitialString)
        {
            string[] Strs = InitialString.Split(',');

            mySide = (SideEnum)int.Parse(Strs[0]);
            Index = int.Parse(Strs[1]);
        }

        public override string ToString()
        {
            return ((int)mySide).ToString() + "," + Index.ToString();
        }

        //public BaseIndicatorClass Clone()
        //{
        //    BaseIndicatorClass biTmp = new BaseIndicatorClass();

        //    biTmp.mySide = mySide;
        //    biTmp.Index = Index;

        //    return biTmp;
        //}
    }

    class CheckDuplicateClass
    {
        public List<CheckDuplicateItemClass> CheckHeightList = new List<CheckDuplicateItemClass>();
        protected JzToolsClass JzTools = new JzToolsClass();
        public CheckDuplicateClass()
        {


        }
        public int Count
        {
            get
            {
                return CheckHeightList.Count;
            }
        }

        public bool Check(KeyAssignClass keyassign,int Index)
        {
            bool IsDuplicate = false;

            CheckDuplicateItemClass CheckItem = CheckHeightList[Index];

            if (

               JzTools.IsInRange(CheckItem.CheckHeight[(int)CornerEnum.LT], keyassign.CheckCombine(CornerExEnum.LT), 0.04)
            && JzTools.IsInRange(CheckItem.CheckHeight[(int)CornerEnum.LB], keyassign.CheckCombine(CornerExEnum.LB), 0.04)
            && JzTools.IsInRange(CheckItem.CheckHeight[(int)CornerEnum.RT], keyassign.CheckCombine(CornerExEnum.RT), 0.04)
            && JzTools.IsInRange(CheckItem.CheckHeight[(int)CornerEnum.RB], keyassign.CheckCombine(CornerExEnum.RB), 0.04)
            
                )
            {
                IsDuplicate = true;
            }

            return IsDuplicate;        
        }

        public void Save(KeyAssignClass keyassign,int Index)
        {
            CheckDuplicateItemClass CheckItem = CheckHeightList[Index];

            CheckItem.CheckHeight[(int)CornerEnum.LT] = keyassign.CheckCombine(CornerExEnum.LT);
            CheckItem.CheckHeight[(int)CornerEnum.LB] = keyassign.CheckCombine(CornerExEnum.LB);
            CheckItem.CheckHeight[(int)CornerEnum.RT] = keyassign.CheckCombine(CornerExEnum.RT);
            CheckItem.CheckHeight[(int)CornerEnum.RB] = keyassign.CheckCombine(CornerExEnum.RB);
        }

        Random myrnd = new Random();


            
        public void SameCheck(KeyAssignClass keyassign,int index)
        {
            //CheckHeightList.Add(new CheckDuplicateItemClass());
            double rndtest = (double)myrnd.Next(500) * 0.00001;

            CheckDuplicateItemClass CheckItem = CheckHeightList[index];

            if(JzTools.IsInRange(keyassign.CheckCombine(CornerExEnum.LT),CheckItem.CheckHeight[(int)CornerEnum.LT],0.1))
            {
                if (!JzTools.IsInRange(keyassign.CheckCombine(CornerExEnum.LT), CheckItem.CheckHeight[(int)CornerEnum.LT], 0.018))
                {
                    rndtest = (double)myrnd.Next(1600) * 0.00001;
                    keyassign.MakeTheSame(CheckItem.CheckHeight[(int)CornerEnum.LT], CornerExEnum.LT, rndtest);
                }
            }
            if (JzTools.IsInRange(keyassign.CheckCombine(CornerExEnum.LB), CheckItem.CheckHeight[(int)CornerEnum.LB], 0.1))
            {
                if (!JzTools.IsInRange(keyassign.CheckCombine(CornerExEnum.LB), CheckItem.CheckHeight[(int)CornerEnum.LB], 0.016))
                {
                    rndtest = (double)myrnd.Next(1300) * 0.00001;
                    keyassign.MakeTheSame(CheckItem.CheckHeight[(int)CornerEnum.LB], CornerExEnum.LB, rndtest);
                }
            }
            if (JzTools.IsInRange(keyassign.CheckCombine(CornerExEnum.RT), CheckItem.CheckHeight[(int)CornerEnum.RT], 0.1))
            {
                if (!JzTools.IsInRange(keyassign.CheckCombine(CornerExEnum.RT), CheckItem.CheckHeight[(int)CornerEnum.RT], 0.017))
                {
                    rndtest = (double)myrnd.Next(1600) * 0.00001;
                    keyassign.MakeTheSame(CheckItem.CheckHeight[(int)CornerEnum.RT], CornerExEnum.RT, rndtest);
                }
            }
            if (JzTools.IsInRange(keyassign.CheckCombine(CornerExEnum.RB), CheckItem.CheckHeight[(int)CornerEnum.RB], 0.1))
            {
                if (!JzTools.IsInRange(keyassign.CheckCombine(CornerExEnum.RB), CheckItem.CheckHeight[(int)CornerEnum.RB], 0.018))
                {
                    rndtest = (double)myrnd.Next(1500) * 0.00001;
                    keyassign.MakeTheSame(CheckItem.CheckHeight[(int)CornerEnum.RB], CornerExEnum.RB, rndtest);
                }
            }

            //CheckItem.CheckHeight[(int)CornerEnum.LT] = keyassign.CheckCombine(CornerExEnum.LT);
            //CheckItem.CheckHeight[(int)CornerEnum.LB] = keyassign.CheckCombine(CornerExEnum.LB);
            //CheckItem.CheckHeight[(int)CornerEnum.RT] = keyassign.CheckCombine(CornerExEnum.RT);
            //CheckItem.CheckHeight[(int)CornerEnum.RB] = keyassign.CheckCombine(CornerExEnum.RB);
        }

        public void Save(KeyAssignClass keyassign)
        {

            CheckHeightList.Add(new CheckDuplicateItemClass());

            CheckDuplicateItemClass CheckItem = CheckHeightList[CheckHeightList.Count - 1];

            CheckItem.CheckHeight[(int)CornerEnum.LT] = keyassign.CheckCombine(CornerExEnum.LT);
            CheckItem.CheckHeight[(int)CornerEnum.LB] = keyassign.CheckCombine(CornerExEnum.LB);
            CheckItem.CheckHeight[(int)CornerEnum.RT] = keyassign.CheckCombine(CornerExEnum.RT);
            CheckItem.CheckHeight[(int)CornerEnum.RB] = keyassign.CheckCombine(CornerExEnum.RB);
        }

        public void Remove(int Index)
        {
            CheckHeightList.RemoveAt(Index);
        }
    }

    class CheckDuplicateItemClass
    {

        public string BarcodeStr = "";

        public double[] CheckHeight = new double[(int)CornerExEnum.COUNT];

        public CheckDuplicateItemClass()
        {

        }
    }


    class KeyAssignClass : FrameClass
    {
        const char Separator = '\xae';

        const int SizedRatio = -2;          //IN OPERATION SIZE RATIO
        protected JzToolsClass JzTools = new JzToolsClass();
        KeyboardClass KEYBOARD
        {
            get
            {
                return null;
            }
        }
        //RecipeDBClass RECIPEDB
        //{
        //    get
        //    {
        //        return Universal.RECIPEDB;
        //    }
        //}

        //ResultClass RESULT
        //{
        //    get
        //    {
        //        return Universal.RESULT;
        //    }

        //}

        public string Name = "";

        public Rectangle myrect = new Rectangle();
        public Rectangle myrectbak = new Rectangle();

        public Point myrectCenter
        {
            get
            {
                return JzTools.GetRectCenter(myrect);
            }
        }

        public string AliasName = "";
        public string KeyCode = "Capital";
        
        public Pen myPen = new Pen(Color.Green, 2);
        public Brush myBrush = Brushes.Lime;

        //Control Parameters
        public double StandardHeight = 1.58;
        public double Upperbound = 0.7;
        public double Lowerbound = 0.2;
        public double ExamDiff = 0.35;
        public double BesideDiff = 0.45;
        public double CornerBesideDiff = 0.45;
        public int ReportIndex = 0;
        public bool IsNoUseArround = false;
        public bool IsNoUseFactor = false;
        public double MaxDiff = 0;
        public double MaxSignedDiff = 0;

        public double CenterStandardHeight = 1.58;
        public double CenterUpperBound = 0.7;
        public double CenterLowerBound = 0.2;
        public double XUpperBound = 0.35;
        public double YUpperBound = 0.35;
        public double Flatness = 0.25;
        public string DefinedCode = "C";
        //public double AddHeight = 0;

        public double YMaxDiff = 0;
        public double XMaxDiff = 0;

        public double YSignedMaxDiff = 0;
        public double XSignedMaxDiff = 0;

        public double CenterHeight = 0;

        public double GoodRatio = 0;
        public double Adjust = 0;

        public double[] CenterRealHeight = new double[(int)CornerExEnum.COUNT];
        public double[] CenterPlaneHeight = new double[(int)CornerExEnum.COUNT];
        public int CenterPlaneCount = 0;

        public double D1 = 0;
        public double D2 = 0;
        public double D3 = 0;
        public double D4 = 0;


        public double TD1 = 0;
        public double TD2 = 0;
        public double TD3 = 0;
        public double TD4 = 0;

        public CheckDuplicateClass CheckDupe = new CheckDuplicateClass();

        public BaseIndicatorClass[] inBaseIndicator = new BaseIndicatorClass[(int)CornerExEnum.COUNT];
        public BaseIndicatorClass[] outBaseIndicator = new BaseIndicatorClass[(int)CornerExEnum.COUNT];

        public double[] CheckedOrigin = new double[(int)CornerExEnum.COUNT];
        public double[] CheckedBase = new double[(int)CornerExEnum.COUNT];
        public ResonEnum myCheckReson;
        public List<ResonEnum> myCheckResonList = new List<ResonEnum>();

        public double CheckCombine(CornerExEnum CornerEx)
        {
            //return CheckedOrigin[(int)CornerEx] - 0.18;// -CheckedBase[(int)CornerEx];

            if (INI.ISONLINEUSE5PTPLANE)
                return CheckedOrigin[(int)CornerEx] + INI.BASEHEIGHT;
            else
            {
                if (INI.ISADJUST)
                    return CheckedOrigin[(int)CornerEx] - CheckedBase[(int)CornerEx] + INI.BASEHEIGHT + Adjust;
                else
                    return CheckedOrigin[(int)CornerEx] - CheckedBase[(int)CornerEx] + INI.BASEHEIGHT;
            }
            //return CheckedOrigin[(int)CornerEx];
        }

        public void MakeTheSame(double CompValue,CornerExEnum CornerEx,double slightadd)
        {
            if (INI.ISADJUST)
                CheckedOrigin[(int)CornerEx] = CompValue - INI.BASEHEIGHT + CheckedBase[(int)CornerEx] - Adjust + slightadd;
            else
                CheckedOrigin[(int)CornerEx] = CompValue - INI.BASEHEIGHT + CheckedBase[(int)CornerEx] + slightadd;

        }

        public bool IsCheckCornerUpper(CornerExEnum CornerEx)
        {
            return CheckCombine(CornerEx) > (StandardHeight + Upperbound);
        }
        public bool IsCheckCornerLower(CornerExEnum CornerEx)
        {
            return CheckCombine(CornerEx) < (StandardHeight - Lowerbound);
        }
        public bool IsSelected = false;
        public bool IsSelectedStart = false;
        public List<int>[] ListBeside = new List<int>[(int)BesideEnum.COUNT];
        public List<int>[] CornerListBeside = new List<int>[(int)BesideEnum.COUNT];
        //Bitmaps
        public Bitmap bmpOrigion = new Bitmap(1, 1);

        public KeyAssignClass()
        {


        }
        public KeyAssignClass(List<KeyAssignClass> KEYASSIGNLIST,Bitmap bmp)
        {
            if (KEYASSIGNLIST.Count == 0)
            {
                Name = "ASN-00" + "00";
                AliasName = Name;

                myrect = new Rectangle(0, 0, 100, 100);

                IsSelected = true;
                IsSelectedStart = true;
            }
            else
            {
                KeyAssignClass Lastkeyassign = KEYASSIGNLIST[KEYASSIGNLIST.Count - 1];

                foreach (KeyAssignClass keyassign in KEYASSIGNLIST)
                {
                    keyassign.IsSelectedStart = false;
                    keyassign.IsSelected = false;
                }

                Name = "ASN-0" 
                    + (int.Parse(JzTools.GetLastString(Lastkeyassign.Name, 2)) + 1).ToString("000");

                AliasName = Name;
                myrect = Lastkeyassign.myrect;

                //myrect.X = Math.Min(myrect.X + myrect.Width + 20, RECIPEDB.bmpKeyboard.Width - myrect.Width - 1);//Gaara by mask

                IsSelected = true;
                IsSelectedStart = true;

                StandardHeight = Lastkeyassign.StandardHeight;
                Upperbound = Lastkeyassign.Upperbound;
                Lowerbound = Lastkeyassign.Lowerbound;

                ExamDiff = Lastkeyassign.ExamDiff;
                BesideDiff = Lastkeyassign.BesideDiff;
                IsNoUseArround = Lastkeyassign.IsNoUseArround;
                IsNoUseFactor = Lastkeyassign.IsNoUseFactor;

                GoodRatio = Lastkeyassign.GoodRatio;
                Adjust = Lastkeyassign.Adjust;
                Flatness = Lastkeyassign.Flatness;
                DefinedCode = Lastkeyassign.DefinedCode;
                CornerBesideDiff = Lastkeyassign.CornerBesideDiff;

                D1 = Lastkeyassign.D1;
                D2 = Lastkeyassign.D2;
                D3 = Lastkeyassign.D3;
                D4 = Lastkeyassign.D4;

                CenterStandardHeight = Lastkeyassign.CenterStandardHeight;
                CenterUpperBound = Lastkeyassign.CenterUpperBound;
                CenterLowerBound = Lastkeyassign.CenterLowerBound;

                XUpperBound = Lastkeyassign.XUpperBound;
                YUpperBound = Lastkeyassign.YUpperBound;
                //AddHeight = Lastkeyassign.AddHeight;
            }

            GetBMP(bmp);

            AssignControls();
        }
        public KeyAssignClass(string rStr)
        {
            int i = 0;
            string[] Str = rStr.Split(Separator);

            Name = Str[0];
            myrect = JzTools.StringtoRect(Str[1]);

            AliasName = Str[2];

            StandardHeight = double.Parse(Str[3]);
            Upperbound = double.Parse(Str[4]);
            Lowerbound = double.Parse(Str[5]);
            ExamDiff = double.Parse(Str[6]);
            BesideDiff = double.Parse(Str[7]);
            ReportIndex = int.Parse(Str[8]);

            string[] Str1 = Str[9].Split(';');

            i = 0;
            while (i < Str1.Length)
            {
                if (Str1[i] != "0")
                {
                    inBaseIndicator[i] = new BaseIndicatorClass(Str1[i]);
                }
                i++;
            }

            Str1 = Str[10].Split(';');

            i = 0;
            while (i < Str1.Length)
            {
                if (Str1[i] != "0")
                {
                    outBaseIndicator[i] = new BaseIndicatorClass(Str1[i]);
                }
                i++;
            }

            if (Str.Length > 12)
            {
                D1 = double.Parse(Str[11]);
                D2 = double.Parse(Str[12]);
                D3 = double.Parse(Str[13]);
                D4 = double.Parse(Str[14]);
            }

            if (Str.Length > 15)
            {
                IsNoUseArround = Str[15] == "1";
            }
            if (Str.Length > 16)
            {
                IsNoUseFactor = Str[16] == "1";
            }

            if (Str.Length > 17)
            {
                GoodRatio = double.Parse(Str[17]);
            }

            if (Str.Length > 18)
            {
                Adjust = double.Parse(Str[18]);
            }

            if (Str.Length > 19)
            {
                CenterStandardHeight = double.Parse(Str[19]);
                CenterUpperBound = double.Parse(Str[20]);
                CenterLowerBound = double.Parse(Str[21]);

                XUpperBound = double.Parse(Str[22]);
                YUpperBound = double.Parse(Str[23]);
            }

            if (Str.Length > 24)
            {
                Flatness = double.Parse(Str[24]);
            }
            if (Str.Length > 25)
            {
                DefinedCode = Str[25];
            }
            if (Str.Length > 26)
            {
                CornerBesideDiff = double.Parse(Str[26]);
            }
            if (Str.Length > 27)
            {
                KeyCode = Str[27];
            }
            
            if (INI.ISUSEDELTA)
            {
                if (D1 == 0)
                    D1 = ExamDiff;
                if (D2 == 0)
                    D2 = ExamDiff;
                if (D3 == 0)
                    D3 = ExamDiff;
                if (D4 == 0)
                    D4 = ExamDiff;
            }
            else
            {
                D1 = 0;
                D2 = 0;
                D3 = 0;
                D4 = 0;
            }

            GetBMP();

            AssignControls();

        }

        public KeyAssignClass Clone()
        {
            int i = 0;

            KeyAssignClass keyassign = new KeyAssignClass();

            keyassign.Name = Name;
            keyassign.myrect = myrect;

            keyassign.AliasName = AliasName;

            keyassign.IsSelected = false;
            keyassign.IsSelectedStart = false;

            keyassign.StandardHeight = StandardHeight;
            keyassign.Upperbound = Upperbound;
            keyassign.Lowerbound = Lowerbound;

            keyassign.ExamDiff = ExamDiff;
            keyassign.BesideDiff = BesideDiff;
            keyassign.ReportIndex = ReportIndex;
            keyassign.IsNoUseArround = IsNoUseArround;
            keyassign.IsNoUseFactor = IsNoUseFactor;

            keyassign.GoodRatio = GoodRatio;
            keyassign.Adjust = Adjust;

            keyassign.CenterStandardHeight = CenterStandardHeight;
            keyassign.CenterUpperBound = CenterUpperBound;
            keyassign.CenterLowerBound = CenterLowerBound;

            keyassign.XUpperBound = XUpperBound;
            keyassign.YUpperBound = YUpperBound;
            keyassign.Flatness = Flatness;
            keyassign.DefinedCode = DefinedCode;
            keyassign.CornerBesideDiff = CornerBesideDiff;
            keyassign.KeyCode = KeyCode;
            //keyassign.AddHeight = AddHeight;

            keyassign.D1 = D1;
            keyassign.D2 = D2;
            keyassign.D3 = D3;
            keyassign.D4 = D4;



            i = 0;
            while (i < (int)CornerExEnum.COUNT)
            {
                if (inBaseIndicator[i] != null)
                {
                    keyassign.inBaseIndicator[i] = inBaseIndicator[i];
                }
                i++;
            }

            i = 0;
            while (i < (int)CornerExEnum.COUNT)
            {
                if (outBaseIndicator[i] != null)
                {
                    keyassign.outBaseIndicator[i] = outBaseIndicator[i];
                }
                i++;
            }

            //Bitmaps
            keyassign.bmpOrigion.Dispose();
            keyassign.bmpOrigion = (Bitmap)bmpOrigion.Clone();

            keyassign.AssignControls();

            return keyassign;
        }
        public KeyAssignClass CloneAdded(int LastIndex,Bitmap bmp)
        {
            return CloneAdded(LastIndex, false, bmp);
        }
        public KeyAssignClass CloneAdded(int LastIndex, bool IsCopy,Bitmap bmp)
        {
            int i = 0;
            KeyAssignClass keyassign = new KeyAssignClass();

            keyassign.Name = "ASN-0" + (LastIndex + 1).ToString("000");
            keyassign.myrect = myrect;

            //if (!IsCopy)
            //    keyassign.myrect.X = Math.Min(myrect.X + myrect.Width + 20, RECIPEDB.bmpKeyboard.Width - myrect.Width - 1);
            //else
            //    keyassign.myrect.Y = Math.Min(myrect.Y + 10, RECIPEDB.bmpKeyboard.Height - myrect.Height - 1);//Gaara by mask

            keyassign.AliasName = keyassign.Name;

            keyassign.IsSelected = IsSelected;
            keyassign.IsSelectedStart = IsSelectedStart;

            keyassign.StandardHeight = StandardHeight;
            keyassign.Upperbound = Upperbound;
            keyassign.Lowerbound = Lowerbound;

            keyassign.ExamDiff = ExamDiff;
            keyassign.BesideDiff = BesideDiff;
            keyassign.IsNoUseArround = IsNoUseArround;
            keyassign.IsNoUseFactor = IsNoUseFactor;

            keyassign.GoodRatio = GoodRatio;
            keyassign.Adjust = Adjust;

            keyassign.CenterStandardHeight = CenterStandardHeight;
            keyassign.CenterUpperBound = CenterUpperBound;
            keyassign.CenterLowerBound = CenterLowerBound;

            keyassign.XUpperBound = XUpperBound;
            keyassign.YUpperBound = YUpperBound;
            keyassign.Flatness = Flatness;
            keyassign.DefinedCode = DefinedCode;
            keyassign.CornerBesideDiff = CornerBesideDiff;

            keyassign.D1 = D1;
            keyassign.D2 = D2;
            keyassign.D3 = D3;
            keyassign.D4 = D4;


            keyassign.GetBMP(bmp);
            keyassign.AssignControls();

            return keyassign;
        }
        
        public void Initial()
        {
            IsSelected = false;
            IsSelectedStart = false;
        }

        public void ClearVariables()
        {
            CheckedOrigin = new double[(int)CornerExEnum.COUNT];
            CheckedBase = new double[(int)CornerExEnum.COUNT];
            myCheckReson = ResonEnum.THISTIMEOK;
            myCheckResonList.Clear();

            ListBesidesErrorString.Clear();
            ListCornerBesidesErrorString.Clear();
        }

        public bool IsInside(Point Pt)
        {
            bool ret = myrect.IntersectsWith(JzTools.SimpleRect(Pt));

            return ret;
        }
        public CornerEnum IsInsideCorner(Point Pt)
        {
            int i = 0;
            CornerEnum retCorner = CornerEnum.NONE;

            while (i < (int)CornerEnum.COUNT)
            {
                if (JzTools.CornerRect(myrect, (CornerEnum)i, CornerSize << 1).IntersectsWith(JzTools.SimpleRect(Pt,CornerSize)))
                {
                    retCorner = (CornerEnum) i;
                    break;
                }

                i++;
            }

            return retCorner;
        }

        public void InitialBesides()
        {
            int i = 0;
            while (i < ListBeside.Length)
            {
                ListBeside[i] = new List<int>();
                i++;
            }
            
            i = 0;
            while (i < CornerListBeside.Length)
            {
                CornerListBeside[i] = new List<int>();
                i++;
            }
        }
        public BesideEnum MeetBeside(Rectangle Rect)
        {
            Point Pt = JzTools.GetRectCenter(Rect);
            
            if ((Math.Abs(Pt.Y - myrectCenter.Y) < (myrect.Height >> 1) && myrectCenter.X > Pt.X))
            {
                return BesideEnum.LEFT;
            }
            if ((Math.Abs(Pt.Y - myrectCenter.Y) < (myrect.Height >> 1) && myrectCenter.X < Pt.X))
            {
                return BesideEnum.RIGHT;
            }
            if (Pt.Y < myrectCenter.Y)
            {
                return BesideEnum.TOP;
            }
            if (Pt.Y > myrectCenter.Y)
            {
                return BesideEnum.BOTTOM;
            }

            return BesideEnum.NONE;
        }

        public void BackupRect()
        {
            myrectbak = myrect;
        }
        public void RestoreRect()
        {
            myrect = myrectbak;
        }

        public void MoveRect(int X, int Y)
        {
            RestoreRect();
            
            myrect.Offset(X, Y);
            //myrect = JzTools.BonudRect(myrect, RECIPEDB.bmpKeyboard.Size);//Gaara by mask


            //if (rectKeyassign.Height < 10 || rectKeyassign.Width < 10)
            //    rectKeyassign = rectKeyassign;

        }
        public void SizedRect(int Width,int Height,CornerEnum CatchCorner)
        {
            RestoreRect();

            switch (CatchCorner)
            {
                case CornerEnum.RB:
                    myrect.Width = Math.Max(10, myrect.Width + Width);
                    myrect.Height = Math.Max(10, myrect.Height + Height);
                    break;
                case CornerEnum.LT:
                    myrect.X = Math.Min(myrect.X + Width, myrect.X + myrect.Width - 10);
                    myrect.Y = Math.Min(myrect.Y + Height, myrect.Y + myrect.Height - 10);

                    myrect.Width = Math.Max(myrect.Width - Width, 10);
                    myrect.Height = Math.Max(myrect.Height - Height, 10);
                    break;
                case CornerEnum.RT:
                    myrect.Y = Math.Min(myrect.Y + Height, myrect.Y + myrect.Height - 10);

                    myrect.Width = Math.Max(myrect.Width + Width, 10);
                    myrect.Height = Math.Max(10, myrect.Height - Height);
                    break;
                case CornerEnum.LB:
                    myrect.X = Math.Min(myrect.X + Width, myrect.X + myrect.Width - 10);

                    myrect.Width = Math.Max(10, myrect.Width - Width);
                    myrect.Height = Math.Max(myrect.Height + Height, 10);
                    break;
            }

            //myrect = JzTools.BonudRect(myrect, RECIPEDB.bmpKeyboard.Size);//Gaara by mask

        }
        public void FactorEnabled()
        {
            if (INI.ISFACTORENABLE)
            {
                double AVG = (CheckedOrigin[(int)CornerEnum.LT] + CheckedOrigin[(int)CornerEnum.LB] + CheckedOrigin[(int)CornerEnum.RT] + CheckedOrigin[(int)CornerEnum.RB]) / 4d;


                if (IsNoUseFactor)
                {
                    //CheckedOrigin[(int)CornerEnum.LT] = CheckedOrigin[(int)CornerEnum.LT] + (AVG - CheckedOrigin[(int)CornerEnum.LT]) * 1;
                    //CheckedOrigin[(int)CornerEnum.LB] = CheckedOrigin[(int)CornerEnum.LB] + (AVG - CheckedOrigin[(int)CornerEnum.LB]) * 1;
                    //CheckedOrigin[(int)CornerEnum.RT] = CheckedOrigin[(int)CornerEnum.RT] + (AVG - CheckedOrigin[(int)CornerEnum.RT]) * 1;
                    //CheckedOrigin[(int)CornerEnum.RB] = CheckedOrigin[(int)CornerEnum.RB] + (AVG - CheckedOrigin[(int)CornerEnum.RB]) * 1;

                }
                else
                {
                    if (GoodRatio > 0)
                    {
                        CheckedOrigin[(int)CornerEnum.LT] = CheckedOrigin[(int)CornerEnum.LT] + (AVG - CheckedOrigin[(int)CornerEnum.LT]) * (1d - GoodRatio);
                        CheckedOrigin[(int)CornerEnum.LB] = CheckedOrigin[(int)CornerEnum.LB] + (AVG - CheckedOrigin[(int)CornerEnum.LB]) * (1d - GoodRatio);
                        CheckedOrigin[(int)CornerEnum.RT] = CheckedOrigin[(int)CornerEnum.RT] + (AVG - CheckedOrigin[(int)CornerEnum.RT]) * (1d - GoodRatio);
                        CheckedOrigin[(int)CornerEnum.RB] = CheckedOrigin[(int)CornerEnum.RB] + (AVG - CheckedOrigin[(int)CornerEnum.RB]) * (1d - GoodRatio);
                    }
                    else
                    {
                        CheckedOrigin[(int)CornerEnum.LT] = CheckedOrigin[(int)CornerEnum.LT] + (AVG - CheckedOrigin[(int)CornerEnum.LT]) * (1d - INI.FACTOR);
                        CheckedOrigin[(int)CornerEnum.LB] = CheckedOrigin[(int)CornerEnum.LB] + (AVG - CheckedOrigin[(int)CornerEnum.LB]) * (1d - INI.FACTOR);
                        CheckedOrigin[(int)CornerEnum.RT] = CheckedOrigin[(int)CornerEnum.RT] + (AVG - CheckedOrigin[(int)CornerEnum.RT]) * (1d - INI.FACTOR);
                        CheckedOrigin[(int)CornerEnum.RB] = CheckedOrigin[(int)CornerEnum.RB] + (AVG - CheckedOrigin[(int)CornerEnum.RB]) * (1d - INI.FACTOR);
                    }
                }
            }

            if (INI.ISNMBLBADDED != 0)
            {
                if (CheckCombine(CornerExEnum.LT) < (StandardHeight - Lowerbound))
                {
                    CheckedOrigin[(int)CornerEnum.LT] += INI.ISNMBLBADDED;
                }
                if (CheckCombine(CornerExEnum.LB) < (StandardHeight - Lowerbound))
                {
                    CheckedOrigin[(int)CornerEnum.LB] += INI.ISNMBLBADDED;
                }
                if (CheckCombine(CornerExEnum.RT) < (StandardHeight - Lowerbound))
                {
                    CheckedOrigin[(int)CornerEnum.RT] += INI.ISNMBLBADDED;
                }
                if (CheckCombine(CornerExEnum.RB) < (StandardHeight - Lowerbound))
                {
                    CheckedOrigin[(int)CornerEnum.RB] += INI.ISNMBLBADDED;
                }
            }

        }

        public void CheckCalibration()
        {
            int i = 0, j = 0;

            i = 0;

            //if (AliasName == "space")
            //    Name = Name;

            while (i < (int)CornerEnum.COUNT)
            {
                if (outBaseIndicator[i] != null || inBaseIndicator[i] != null)
                {
                    if (JzTools.RoundDown(CheckCombine((CornerExEnum)i), 2) > (INI.TESTZLOCATION - INI.BASEZLOCATION) + 0.02 || JzTools.RoundDown(CheckCombine((CornerExEnum)i), 2) < (INI.TESTZLOCATION - INI.BASEZLOCATION) - 0.02)
                    {
                        myCheckReson = ResonEnum.STANDARDERROR;

                        //RESULT.FailCount++;//Gaara by mask
                    }

                }
                i++;
            }

            if (myCheckReson == ResonEnum.STANDARDERROR)
            {
                myCheckResonList.Add(ResonEnum.STANDARDERROR);
            }

        }


        public void CheckReson()
        {
            int i = 0, j = 0;

            i = 0;

            if (AliasName == "space")
                Name = Name;

            while (i < (int)CornerEnum.COUNT)
            {
                if (outBaseIndicator[i] != null || inBaseIndicator[i] != null)
                {
                    if (JzTools.RoundDown(CheckCombine((CornerExEnum)i),3) > (StandardHeight + Upperbound) || JzTools.RoundDown(CheckCombine((CornerExEnum)i),3) < (StandardHeight - Lowerbound))
                    {
                        myCheckReson = ResonEnum.STANDARDERROR;

                        //RESULT.FailCount++;//Gaara by mask
                    }

                    //if (CheckCombine((CornerExEnum)i) > RESULT.mHighest)
                    //{
                    //    RESULT.mHighest = CheckCombine((CornerExEnum)i);
                    //}
                    //if (CheckCombine((CornerExEnum)i) < RESULT.mLowest)
                    //{
                    //    RESULT.mLowest = CheckCombine((CornerExEnum)i);
                    //}//Gaara by mask

                }
                i++;
            }

            if (myCheckReson == ResonEnum.STANDARDERROR)
            {
                myCheckResonList.Add(ResonEnum.STANDARDERROR);
            }

            //if (myCheckReson == ResonEnum.THISTIMEOK)
            {

                if (AliasName == "SPACE" || AliasName.IndexOf("SHIFT") > -1 || inBaseIndicator[(int)CornerExEnum.PT1] != null)
                {
                    //if (CheckedOrigin[(int)CornerExEnum.PT1] != null)
                    //{
                    //    i = 0;
                    //    while (i < (int)CornerExEnum.COUNT - 1)
                    //    {
                    //        if (i < (int)CornerExEnum.PT1)
                    //        {
                    //            if (Math.Abs(CheckCombine((CornerExEnum)i) - CheckCombine(CornerExEnum.PT1)) > INI.SPACEDIFF)
                    //            {
                    //                myCheckReson = ResonEnum.NOFLAT;
                    //                myCheckResonList.Add(ResonEnum.NOFLAT);
                    //                i = 1000;
                    //                break;
                    //            }
                    //        }

                    //        i++;
                    //    }

                    //    //if (myCheckReson != ResonEnum.NOTFOUND)
                    //    //    return;
                    //}
                    //if (AliasName == "SPACE")
                    //{
                    //    //if (MaxDiff + INI.COMPENSATION > INI.SPACEDIFF)
                    //    //{
                    //    //    myCheckReson = ResonEnum.NOFLAT;
                    //    //    myCheckResonList.Add(ResonEnum.NOFLAT);
                    //    //}
                    //    if (JzTools.RoundDown(Math.Abs(MaxDiff), 2) > Flatness)
                    //    {
                    //        if (MaxDiff > 0)
                    //        {
                    //            myCheckReson = ResonEnum.PLUSFLAT;
                    //            myCheckResonList.Add(ResonEnum.PLUSFLAT);
                    //        }
                    //        else
                    //        {
                    //            myCheckReson = ResonEnum.NEGFLAT;
                    //            myCheckResonList.Add(ResonEnum.NEGFLAT);
                    //        }

                    //    }
                    //}
                    //else if (AliasName == "L-SHIFT")
                    //{
                    //    //if (MaxDiff> INI.LSHIFTDIFF)
                    //    //{
                    //    //    myCheckReson = ResonEnum.NOFLAT;
                    //    //    myCheckResonList.Add(ResonEnum.NOFLAT);
                    //    //}
                    //    if (JzTools.RoundDown(Math.Abs(MaxDiff), 2) > Flatness)
                    //    {
                    //        if (MaxDiff > 0)
                    //        {
                    //            myCheckReson = ResonEnum.PLUSFLAT;
                    //            myCheckResonList.Add(ResonEnum.PLUSFLAT);
                    //        }
                    //        else
                    //        {
                    //            myCheckReson = ResonEnum.NEGFLAT;
                    //            myCheckResonList.Add(ResonEnum.NEGFLAT);
                    //        }

                    //    }
                    //}
                    //else if (AliasName == "R-SHIFT")
                    //{
                    //    //if (MaxDiff > INI.RSHIFTDIFF)
                    //    //{
                    //    //    myCheckReson = ResonEnum.NOFLAT;
                    //    //    myCheckResonList.Add(ResonEnum.NOFLAT);
                    //    //}
                    //    if (JzTools.RoundDown(Math.Abs(MaxDiff), 2) > Flatness)
                    //    {
                    //        if (MaxDiff > 0)
                    //        {
                    //            myCheckReson = ResonEnum.PLUSFLAT;
                    //            myCheckResonList.Add(ResonEnum.PLUSFLAT);
                    //        }
                    //        else
                    //        {
                    //            myCheckReson = ResonEnum.NEGFLAT;
                    //            myCheckResonList.Add(ResonEnum.NEGFLAT);
                    //        }

                    //    }
                    //}
                    //else
                    {
                        //if (JzTools.RoundDown(Math.Abs(MaxDiffPlanHeight), 2) > Flatness)
                        //{
                        //    if (MaxDiffPlanHeight > 0)
                        //    {
                        //        myCheckReson = ResonEnum.PLUSFLAT;
                        //        myCheckResonList.Add(ResonEnum.PLUSFLAT);
                        //    }
                        //    else
                        //    {
                        //        myCheckReson = ResonEnum.NEGFLAT;
                        //        myCheckResonList.Add(ResonEnum.NEGFLAT);
                        //    }
                        //}

                        
                        int ix = 0;

                        //if (AliasName == "L-CMD")
                        //{
                        //    ix = ix;
                        //}


                        while (ix < CenterPlaneCount)
                        {
                            //if (JzTools.RoundDown(Math.Abs(CenterPlaneHeight[(int)CornerExEnum.PT1 + ix]), 2) > Flatness)
                            if (Flatness > 0)
                            {
                                if (JzTools.RoundDown(Math.Abs(CenterPlaneHeight[(int)CornerExEnum.PT1 + ix]), 2) > Flatness)
                                {
                                    if (CenterPlaneHeight[(int)CornerExEnum.PT1 + ix] > 0)
                                    {
                                        myCheckReson = ResonEnum.PLUSFLAT;
                                        myCheckResonList.Add(ResonEnum.PLUSFLAT);
                                    }
                                    else
                                    {
                                        myCheckReson = ResonEnum.NEGFLAT;
                                        myCheckResonList.Add(ResonEnum.NEGFLAT);
                                    }
                                }
                            }
                            else
                            {
                                if (JzTools.RoundDown(CenterPlaneHeight[(int)CornerExEnum.PT1 + ix], 2) > Flatness)
                                {
                                    //if (CenterPlaneHeight[(int)CornerExEnum.PT1 + ix] > 0)
                                    //{
                                        myCheckReson = ResonEnum.PLUSFLAT;
                                        myCheckResonList.Add(ResonEnum.PLUSFLAT);
                                    //}
                                    //else
                                    //{
                                    //    myCheckReson = ResonEnum.NEGFLAT;
                                    //    myCheckResonList.Add(ResonEnum.NEGFLAT);
                                    //}
                                }
                            }


                            ix++;
                        }



                    }

                    //if (myCheckReson != ResonEnum.NOTFOUND)
                    //    return;

                }
                else
                {
                    if (JzTools.RoundDown(CenterHeight, 2) > (CenterStandardHeight + CenterUpperBound) || JzTools.RoundDown(CenterHeight, 2) < (CenterStandardHeight - CenterLowerBound))
                    {
                        myCheckReson = ResonEnum.CENTEROVER;
                        myCheckResonList.Add(ResonEnum.CENTEROVER);

                        //RESULT.FailCount++;//Gaara by mask
                    }

                    if (JzTools.RoundDown(XMaxDiff,2) > XUpperBound)
                    {
                        myCheckReson = ResonEnum.XOVER;
                        myCheckResonList.Add(ResonEnum.XOVER);
                        //RESULT.FailCount++;//Gaara by mask
                    }
                    if (JzTools.RoundDown(YMaxDiff,2) > YUpperBound)
                    {
                        myCheckReson = ResonEnum.YOVER;
                        myCheckResonList.Add(ResonEnum.YOVER);
                        //RESULT.FailCount++;//Gaara by mask
                    }

                }

                i = 0;
                while (i < (int)CornerEnum.COUNT - 1)
                {
                    if (outBaseIndicator[i] != null || inBaseIndicator[i] != null)
                    {
                        j = i;
                        while (j < (int)CornerEnum.COUNT)
                        {
                            //Modified 
                            if (INI.ISUSEARROUND && !IsNoUseArround)
                            {
                                if ((((CornerEnum)i) == CornerEnum.LT && ((CornerEnum)j) == CornerEnum.RB) || (((CornerEnum)i) == CornerEnum.LB && ((CornerEnum)j) == CornerEnum.RT))
                                {
                                    j++;
                                    continue;
                                }
                            }

                            if (outBaseIndicator[j] != null || inBaseIndicator[j] != null)
                            {

                                if (INI.ISUSEDELTA && !IsNoUseArround)
                                {

                                    if (((CornerEnum)i) == CornerEnum.LT && ((CornerEnum)j) == CornerEnum.RT)
                                    {

                                        TD1 = Math.Abs(CheckCombine((CornerExEnum)i) - CheckCombine((CornerExEnum)j));

                                    }
                                    else if (((CornerEnum)i) == CornerEnum.LT && ((CornerEnum)j) == CornerEnum.LB)
                                    {
                                        TD2 = Math.Abs(CheckCombine((CornerExEnum)i) - CheckCombine((CornerExEnum)j));

                                    }
                                    else if (((CornerEnum)i) == CornerEnum.RT && ((CornerEnum)j) == CornerEnum.RB)
                                    {
                                        TD3 = Math.Abs(CheckCombine((CornerExEnum)i) - CheckCombine((CornerExEnum)j));

                                        if (TD1 > D1)
                                        {
                                            myCheckReson = ResonEnum.SELFERROR;
                                        }
                                        if (TD2 > D2)
                                        {
                                            myCheckReson = ResonEnum.SELFERROR;
                                        }
                                        if (TD3 > D3)
                                        {
                                            myCheckReson = ResonEnum.SELFERROR;
                                        }
                                        if (TD4 > D4)
                                        {
                                            myCheckReson = ResonEnum.SELFERROR;
                                        }

                                        if (myCheckReson == ResonEnum.SELFERROR)
                                        {
                                            myCheckResonList.Add(ResonEnum.SELFERROR);

                                            j = 1000;
                                            i = 1000;
                                            break;
                                        }
                                        else
                                        {
                                            int id = 0;
                                            int jd = 0;

                                            while (id < (int)CornerEnum.COUNT - 1)
                                            {
                                                jd = id;
                                                while (jd < (int)CornerEnum.COUNT)
                                                {
                                                    //if (Math.Abs(CheckCombine((CornerExEnum)id) - CheckCombine((CornerExEnum)jd)) > RESULT.mSlop)
                                                    //{
                                                    //    RESULT.mSlop = Math.Abs(CheckCombine((CornerExEnum)id) - CheckCombine((CornerExEnum)jd));
                                                    //}//Gaara by mask

                                                    if (JzTools.RoundDown(Math.Abs(CheckCombine((CornerExEnum)id) - CheckCombine((CornerExEnum)jd)),2) > ExamDiff)
                                                    {
                                                        myCheckReson = ResonEnum.SELFERROR;
                                                        myCheckResonList.Add(ResonEnum.SELFERROR);

                                                        jd = 1000;
                                                        id = 1000;
                                                        break;
                                                    }

                                                    jd++;
                                                }
                                                id++;
                                            }


                                            if (id == 1000)
                                            {
                                                j = 1000;
                                                i = 1000;
                                                break;
                                            }
                                        }
                                    }
                                    else if (((CornerEnum)i) == CornerEnum.LB && ((CornerEnum)j) == CornerEnum.RB)
                                    {
                                        TD4 = Math.Abs(CheckCombine((CornerExEnum)i) - CheckCombine((CornerExEnum)j));
                                    }
                                }
                                else
                                {
                                    //if (Math.Abs(CheckCombine((CornerExEnum)i) - CheckCombine((CornerExEnum)j)) > RESULT.mSlop)
                                    //{
                                    //    RESULT.mSlop = Math.Abs(CheckCombine((CornerExEnum)i) - CheckCombine((CornerExEnum)j));
                                    //}//Gaara by mask

                                    if (JzTools.RoundDown(Math.Abs(CheckCombine((CornerExEnum)i) - CheckCombine((CornerExEnum)j)),2) > ExamDiff)
                                    {
                                        myCheckReson = ResonEnum.SELFERROR;
                                        myCheckResonList.Add(ResonEnum.SELFERROR);

                                        j = 1000;
                                        i = 1000;
                                        break;
                                    }
                                }
                            }

                            j++;
                        }
                    }
                    i++;
                }
            }
        }

        public List<string> ListBesidesErrorString = new List<string>();
        public List<string> ListCornerBesidesErrorString = new List<string>();

        public void CheckBeside(KeyAssignClass besidekeyassign, BesideEnum Beside, int BesideIndex, int myIndex)
        {

            if (!(myCheckReson == ResonEnum.THISTIMEOK && besidekeyassign.myCheckReson == ResonEnum.THISTIMEOK))
            {
                return;
            }

            switch (Beside)
            {
                case BesideEnum.LEFT:
                    if (JzTools.RoundDown(CenterHeight - besidekeyassign.CenterHeight,2) > BesideDiff)
                    {
                        ListBesidesErrorString.Add(CornerExEnum.LT.ToString() + ";" + CornerExEnum.RT.ToString() + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    }
                    //if ((Math.Abs(JzTools.RoundDown(CenterHeight - besidekeyassign.CenterHeight, 2)) > RESULT.mMutual))
                    //{
                    //    RESULT.mMutual = Math.Abs(JzTools.RoundDown(CenterHeight - besidekeyassign.CenterHeight, 2));
                    //}//Gaara by mask
                    //if ((Math.Abs(CheckCombine(CornerExEnum.LT) - besidekeyassign.CheckCombine(CornerExEnum.RT)) >= BesideDiff))
                    //{
                    //    //ListBesidesErrorString.Add(JzTools.PointtoString(PtCorner[(int)CornerEnum.LT]) + ";" + JzTools.PointtoString(keycapbeside.PtCorner[(int)CornerEnum.RT]) + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //    ListBesidesErrorString.Add(CornerExEnum.LT.ToString() + ";" + CornerExEnum.RT.ToString() + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //}
                    //if ((Math.Abs(CheckCombine(CornerExEnum.LT) - besidekeyassign.CheckCombine(CornerExEnum.RT)) >= RESULT.mMutual))
                    //{
                    //    RESULT.mMutual = Math.Abs(CheckCombine(CornerExEnum.LT) - besidekeyassign.CheckCombine(CornerExEnum.RT));
                    //}
                    //if ((Math.Abs(CheckCombine(CornerExEnum.LT) - besidekeyassign.CheckCombine(CornerExEnum.RB)) >= BesideDiff))
                    //{
                    //    //ListBesidesErrorString.Add(JzTools.PointtoString(PtCorner[(int)CornerEnum.LT]) + ";" + JzTools.PointtoString(keycapbeside.PtCorner[(int)CornerEnum.RB]) + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //    ListBesidesErrorString.Add(CornerEnum.LT.ToString() + ";" + CornerEnum.RB.ToString() + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //}
                    //if ((Math.Abs(CheckCombine(CornerExEnum.LT) - besidekeyassign.CheckCombine(CornerExEnum.RB)) >= RESULT.mMutual))
                    //{
                    //    RESULT.mMutual = Math.Abs(CheckCombine(CornerExEnum.LT) - besidekeyassign.CheckCombine(CornerExEnum.RB));
                    //}
                    //if ((Math.Abs(CheckCombine(CornerExEnum.LB) - besidekeyassign.CheckCombine(CornerExEnum.RT)) >= BesideDiff))
                    //{
                    //    //ListBesidesErrorString.Add(JzTools.PointtoString(PtCorner[(int)CornerEnum.LB]) + ";" + JzTools.PointtoString(keycapbeside.PtCorner[(int)CornerEnum.RT]) + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //    ListBesidesErrorString.Add(CornerEnum.LB.ToString() + ";" + CornerEnum.RT.ToString() + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //}
                    //if ((Math.Abs(CheckCombine(CornerExEnum.LB) - besidekeyassign.CheckCombine(CornerExEnum.RT)) >= RESULT.mMutual))
                    //{
                    //    RESULT.mMutual = Math.Abs(CheckCombine(CornerExEnum.LB) - besidekeyassign.CheckCombine(CornerExEnum.RT));
                    //}

                    //if ((Math.Abs(CheckCombine(CornerExEnum.LB) - besidekeyassign.CheckCombine(CornerExEnum.RB)) >= BesideDiff))
                    //{
                    //    //ListBesidesErrorString.Add(JzTools.PointtoString(PtCorner[(int)CornerEnum.LB]) + ";" + JzTools.PointtoString(keycapbeside.PtCorner[(int)CornerEnum.RB]) + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //    ListBesidesErrorString.Add(CornerEnum.LB.ToString() + ";" + CornerEnum.RB.ToString() + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //}
                    //if ((Math.Abs(CheckCombine(CornerExEnum.LB) - besidekeyassign.CheckCombine(CornerExEnum.RB)) >= RESULT.mMutual))
                    //{
                    //    RESULT.mMutual = Math.Abs(CheckCombine(CornerExEnum.LB) - besidekeyassign.CheckCombine(CornerExEnum.RB));
                    //}

                    break;
                case BesideEnum.RIGHT:
                    if (JzTools.RoundDown(CenterHeight - besidekeyassign.CenterHeight, 2) > BesideDiff)
                    {
                        ListBesidesErrorString.Add(CornerExEnum.RT.ToString() + ";" + CornerExEnum.LT.ToString() + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    }
                    //if ((Math.Abs(JzTools.RoundDown(CenterHeight - besidekeyassign.CenterHeight, 2)) > RESULT.mMutual))
                    //{
                    //    RESULT.mMutual = Math.Abs(JzTools.RoundDown(CenterHeight - besidekeyassign.CenterHeight, 2));
                    //}//Gaara by mask

                    //if ((Math.Abs(CheckCombine(CornerExEnum.RT) - besidekeyassign.CheckCombine(CornerExEnum.LT)) >= BesideDiff))
                    //{
                    //    //ListBesidesErrorString.Add(JzTools.PointtoString(PtCorner[(int)CornerEnum.RT]) + ";" + JzTools.PointtoString(keycapbeside.PtCorner[(int)CornerEnum.LT]) + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //    ListBesidesErrorString.Add(CornerEnum.RT.ToString() + ";" + CornerEnum.LT.ToString() + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //}
                    //if ((Math.Abs(CheckCombine(CornerExEnum.RT) - besidekeyassign.CheckCombine(CornerExEnum.LT)) >= RESULT.mMutual))
                    //{
                    //    RESULT.mMutual = Math.Abs(CheckCombine(CornerExEnum.RT) - besidekeyassign.CheckCombine(CornerExEnum.LT));
                    //}
                    //if ((Math.Abs(CheckCombine(CornerExEnum.RT) - besidekeyassign.CheckCombine(CornerExEnum.LB)) >= BesideDiff))
                    //{
                    //    //ListBesidesErrorString.Add(JzTools.PointtoString(PtCorner[(int)CornerEnum.RT]) + ";" + JzTools.PointtoString(keycapbeside.PtCorner[(int)CornerEnum.LB]) + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //    ListBesidesErrorString.Add(CornerEnum.RT.ToString() + ";" + CornerEnum.LB.ToString() + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //}
                    //if ((Math.Abs(CheckCombine(CornerExEnum.RT) - besidekeyassign.CheckCombine(CornerExEnum.LB)) >= RESULT.mMutual))
                    //{
                    //    RESULT.mMutual = Math.Abs(CheckCombine(CornerExEnum.RT) - besidekeyassign.CheckCombine(CornerExEnum.LB));
                    //}
                    //if ((Math.Abs(CheckCombine(CornerExEnum.RB) - besidekeyassign.CheckCombine(CornerExEnum.LT)) >= BesideDiff))
                    //{
                    //    //ListBesidesErrorString.Add(JzTools.PointtoString(PtCorner[(int)CornerEnum.RB]) + ";" + JzTools.PointtoString(keycapbeside.PtCorner[(int)CornerEnum.LT]) + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //    ListBesidesErrorString.Add(CornerEnum.RB.ToString() + ";" + CornerEnum.LT.ToString() + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //}
                    //if ((Math.Abs(CheckCombine(CornerExEnum.RB) - besidekeyassign.CheckCombine(CornerExEnum.LT)) >= RESULT.mMutual))
                    //{
                    //    RESULT.mMutual = Math.Abs(CheckCombine(CornerExEnum.RB) - besidekeyassign.CheckCombine(CornerExEnum.LT));
                    //}
                    //if ((Math.Abs(CheckCombine(CornerExEnum.RB) - besidekeyassign.CheckCombine(CornerExEnum.LB)) >= BesideDiff))
                    //{
                    //    //ListBesidesErrorString.Add(JzTools.PointtoString(PtCorner[(int)CornerEnum.RB]) + ";" + JzTools.PointtoString(keycapbeside.PtCorner[(int)CornerEnum.LB]) + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //    ListBesidesErrorString.Add(CornerEnum.RB.ToString() + ";" + CornerEnum.LB.ToString() + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //}
                    //if ((Math.Abs(CheckCombine(CornerExEnum.RB) - besidekeyassign.CheckCombine(CornerExEnum.LB)) >= RESULT.mMutual))
                    //{
                    //    RESULT.mMutual = Math.Abs(CheckCombine(CornerExEnum.RB) - besidekeyassign.CheckCombine(CornerExEnum.LB));
                    //}
                    break;
                case BesideEnum.TOP:
                    if (JzTools.RoundDown(CenterHeight - besidekeyassign.CenterHeight, 2) > BesideDiff)
                    {
                        ListBesidesErrorString.Add(CornerExEnum.LT.ToString() + ";" + CornerExEnum.LB.ToString() + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    }
                    //if ((Math.Abs(JzTools.RoundDown(CenterHeight - besidekeyassign.CenterHeight, 2)) > RESULT.mMutual))
                    //{
                    //    RESULT.mMutual = Math.Abs(JzTools.RoundDown(CenterHeight - besidekeyassign.CenterHeight, 2));
                    //}//Gaara by mask

                    //if ((Math.Abs(CheckCombine(CornerExEnum.LT) - besidekeyassign.CheckCombine(CornerExEnum.LB)) >= BesideDiff))
                    //{
                    //    //ListBesidesErrorString.Add(JzTools.PointtoString(PtCorner[(int)CornerEnum.LT]) + ";" + JzTools.PointtoString(keycapbeside.PtCorner[(int)CornerEnum.LB]) + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //    ListBesidesErrorString.Add(CornerEnum.LT.ToString() + ";" + CornerEnum.LB.ToString() + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //}
                    //if ((Math.Abs(CheckCombine(CornerExEnum.LT) - besidekeyassign.CheckCombine(CornerExEnum.LB)) >= RESULT.mMutual))
                    //{
                    //    RESULT.mMutual = Math.Abs(CheckCombine(CornerExEnum.LT) - besidekeyassign.CheckCombine(CornerExEnum.LB));
                    //}
                    //if ((Math.Abs(CheckCombine(CornerExEnum.LT) - besidekeyassign.CheckCombine(CornerExEnum.RB)) >= BesideDiff))
                    //{
                    //    //ListBesidesErrorString.Add(JzTools.PointtoString(PtCorner[(int)CornerEnum.LT]) + ";" + JzTools.PointtoString(keycapbeside.PtCorner[(int)CornerEnum.RB]) + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //    ListBesidesErrorString.Add(CornerEnum.LT.ToString() + ";" + CornerEnum.RB.ToString() + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //}
                    //if ((Math.Abs(CheckCombine(CornerExEnum.LT) - besidekeyassign.CheckCombine(CornerExEnum.RB)) >= RESULT.mMutual))
                    //{
                    //    RESULT.mMutual = Math.Abs(CheckCombine(CornerExEnum.LT) - besidekeyassign.CheckCombine(CornerExEnum.RB));
                    //}

                    //if ((Math.Abs(CheckCombine(CornerExEnum.RT) - besidekeyassign.CheckCombine(CornerExEnum.LB)) >= BesideDiff))
                    //{
                    //    //ListBesidesErrorString.Add(JzTools.PointtoString(PtCorner[(int)CornerEnum.RT]) + ";" + JzTools.PointtoString(keycapbeside.PtCorner[(int)CornerEnum.LB]) + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //    ListBesidesErrorString.Add(CornerEnum.RT.ToString() + ";" + CornerEnum.LB.ToString() + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //}
                    //if ((Math.Abs(CheckCombine(CornerExEnum.RT) - besidekeyassign.CheckCombine(CornerExEnum.LB)) >= RESULT.mMutual))
                    //{
                    //    RESULT.mMutual = Math.Abs(CheckCombine(CornerExEnum.RT) - besidekeyassign.CheckCombine(CornerExEnum.LB));
                    //}

                    //if ((Math.Abs(CheckCombine(CornerExEnum.RT) - besidekeyassign.CheckCombine(CornerExEnum.RB)) >= BesideDiff))
                    //{
                    //    //ListBesidesErrorString.Add(JzTools.PointtoString(PtCorner[(int)CornerEnum.RT]) + ";" + JzTools.PointtoString(keycapbeside.PtCorner[(int)CornerEnum.RB]) + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //    ListBesidesErrorString.Add(CornerEnum.RT.ToString() + ";" + CornerEnum.RB.ToString() + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //}
                    //if ((Math.Abs(CheckCombine(CornerExEnum.RT) - besidekeyassign.CheckCombine(CornerExEnum.RB)) >= RESULT.mMutual))
                    //{
                    //    RESULT.mMutual = Math.Abs(CheckCombine(CornerExEnum.RT) - besidekeyassign.CheckCombine(CornerExEnum.RB));
                    //}

                    break;
                case BesideEnum.BOTTOM:
                    if (JzTools.RoundDown(CenterHeight - besidekeyassign.CenterHeight, 2) > BesideDiff)
                    {
                        ListBesidesErrorString.Add(CornerExEnum.LB.ToString() + ";" + CornerExEnum.LT.ToString() + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    }
                    //if ((Math.Abs(JzTools.RoundDown(CenterHeight - besidekeyassign.CenterHeight, 2)) > RESULT.mMutual))
                    //{
                    //    RESULT.mMutual = Math.Abs(JzTools.RoundDown(CenterHeight - besidekeyassign.CenterHeight, 2));
                    //}//Gaara by mask

                    //if ((Math.Abs(CheckCombine(CornerExEnum.LB) - besidekeyassign.CheckCombine(CornerExEnum.LT)) >= BesideDiff))
                    //{
                    //    //ListBesidesErrorString.Add(JzTools.PointtoString(PtCorner[(int)CornerEnum.LB]) + ";" + JzTools.PointtoString(keycapbeside.PtCorner[(int)CornerEnum.LT]) + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //    ListBesidesErrorString.Add(CornerEnum.LB.ToString() + ";" + CornerEnum.LT.ToString() + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //}
                    //if ((Math.Abs(CheckCombine(CornerExEnum.LB) - besidekeyassign.CheckCombine(CornerExEnum.LT)) >= RESULT.mMutual))
                    //{
                    //    RESULT.mMutual = Math.Abs(CheckCombine(CornerExEnum.LB) - besidekeyassign.CheckCombine(CornerExEnum.LT));
                    //}

                    //if ((Math.Abs(CheckCombine(CornerExEnum.LB) - besidekeyassign.CheckCombine(CornerExEnum.RT)) >= BesideDiff))
                    //{
                    //    //ListBesidesErrorString.Add(JzTools.PointtoString(PtCorner[(int)CornerEnum.LB]) + ";" + JzTools.PointtoString(keycapbeside.PtCorner[(int)CornerEnum.RT]) + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //    ListBesidesErrorString.Add(CornerEnum.LB.ToString() + ";" + CornerEnum.RT.ToString() + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //}
                    //if ((Math.Abs(CheckCombine(CornerExEnum.LB) - besidekeyassign.CheckCombine(CornerExEnum.RT)) >= RESULT.mMutual))
                    //{
                    //    RESULT.mMutual = Math.Abs(CheckCombine(CornerExEnum.LB) - besidekeyassign.CheckCombine(CornerExEnum.RT));
                    //}

                    //if ((Math.Abs(CheckCombine(CornerExEnum.RB) - besidekeyassign.CheckCombine(CornerExEnum.LT)) >= BesideDiff))
                    //{
                    //    //ListBesidesErrorString.Add(JzTools.PointtoString(PtCorner[(int)CornerEnum.RB]) + ";" + JzTools.PointtoString(keycapbeside.PtCorner[(int)CornerEnum.LT]) + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //    ListBesidesErrorString.Add(CornerEnum.RB.ToString() + ";" + CornerEnum.LT.ToString() + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //}
                    //if ((Math.Abs(CheckCombine(CornerExEnum.RB) - besidekeyassign.CheckCombine(CornerExEnum.LT)) >= RESULT.mMutual))
                    //{
                    //    RESULT.mMutual = Math.Abs(CheckCombine(CornerExEnum.RB) - besidekeyassign.CheckCombine(CornerExEnum.LT));
                    //}

                    //if ((Math.Abs(CheckCombine(CornerExEnum.RB) - besidekeyassign.CheckCombine(CornerExEnum.RT)) >= BesideDiff))
                    //{
                    //    //ListBesidesErrorString.Add(JzTools.PointtoString(PtCorner[(int)CornerEnum.RB]) + ";" + JzTools.PointtoString(keycapbeside.PtCorner[(int)CornerEnum.RT]) + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //    ListBesidesErrorString.Add(CornerEnum.RB.ToString() + ";" + CornerEnum.RT.ToString() + ";" + myIndex.ToString() + ";" + BesideIndex.ToString());
                    //}
                    //if ((Math.Abs(CheckCombine(CornerExEnum.RB) - besidekeyassign.CheckCombine(CornerExEnum.RT)) >= RESULT.mMutual))
                    //{
                    //    RESULT.mMutual = Math.Abs(CheckCombine(CornerExEnum.RB) - besidekeyassign.CheckCombine(CornerExEnum.RT));
                    //}
                    break;
            }
        }

        public int inBaseCount
        {
            get
            {
                int i = 0;
                int j = 0;

                while (i < (int)CornerExEnum.COUNT)
                {
                    if (inBaseIndicator[i] != null)
                    {
                        j++;
                    }
                    i++;
                }

                return j;
            }
        }
        public double MaxDiffPlanHeight
        {
            get
            {
                switch (inBaseCount)
                {
                    case 5:
                        return CenterPlaneHeight[(int)CornerExEnum.PT1];
                        break;
                    case 6:
                        if(Math.Abs(CenterPlaneHeight[(int)CornerExEnum.PT1]) > Math.Abs(CenterPlaneHeight[(int)CornerExEnum.PT2]))
                        {
                            return CenterPlaneHeight[(int)CornerExEnum.PT1];
                        }
                        else
                        {
                            return CenterPlaneHeight[(int)CornerExEnum.PT2];
                        }
                        break;
                    case 7:
                    case 8:
                    case 9:
                    case 10:

                        List<string> SortList = new List<string>();

                        int iz = (int)CornerExEnum.PT1;

                        while(iz < (int)CornerExEnum.COUNT)
                        {
                            SortList.Add((CenterPlaneHeight[(int)CornerExEnum.PT1] * 1000).ToString("000000") + "," + (iz - (int)CornerExEnum.PT1).ToString());
                            iz++;
                        }

                        SortList.Sort();

                        return CenterPlaneHeight[(int)CornerExEnum.PT1 + int.Parse(SortList[SortList.Count -1].Split(',')[1])];
                        
                        break;
                    default:

                        return -1;
                }
            }
        }

        public string PlaneHeightString
        {
            get
            {
                string Str = "";

                if (AliasName == "ESC")
                {
                    AliasName = AliasName;
                }


                if (inBaseCount < 5)
                {
                    return Str;
                }
                
                if(inBaseCount > 4)
                {
                    Str += CenterRealHeight[(int)CornerExEnum.PT1].ToString("0.0000") + "," + CenterPlaneHeight[(int)CornerExEnum.PT1].ToString("0.0000") + ",";
                }
                if (inBaseCount > 5)
                {
                    Str += CenterRealHeight[(int)CornerExEnum.PT2].ToString("0.0000") + "," + CenterPlaneHeight[(int)CornerExEnum.PT2].ToString("0.0000") + ",";
                }
                if (inBaseCount > 6)
                {
                    Str += CenterRealHeight[(int)CornerExEnum.PT3].ToString("0.0000") + "," + CenterPlaneHeight[(int)CornerExEnum.PT3].ToString("0.0000") + ",";
                }
                if (inBaseCount > 7)
                {
                    Str += CenterRealHeight[(int)CornerExEnum.PT4].ToString("0.0000") + "," + CenterPlaneHeight[(int)CornerExEnum.PT4].ToString("0.0000") + ",";
                }
                if (inBaseCount > 8)
                {
                    Str += CenterRealHeight[(int)CornerExEnum.PT5].ToString("0.0000") + "," + CenterPlaneHeight[(int)CornerExEnum.PT5].ToString("0.0000") + ",";
                }
                if (inBaseCount > 9)
                {
                    Str += CenterRealHeight[(int)CornerExEnum.PT6].ToString("0.0000") + "," + CenterPlaneHeight[(int)CornerExEnum.PT6].ToString("0.0000") + ",";
                }

                Str = JzTools.RemoveLastChar(Str, 1);


                return Str;
            }
        }

        //取得原有圖像
        public void GetBMP()
        {
            //GetBMP(RECIPEDB.bmpKeyboard);//Gaara by mask
        }
        public void GetBMP(Bitmap bmp)
        {
            if (myrect.Width == 0)
            {
                myrect.Width = 1;
            }
            if (myrect.Height == 0)
            {
                myrect.Height = 1;
            }

            bmpOrigion.Dispose();

            Rectangle rect = myrect;
            BonudRect(ref rect, bmp.Size);

            bmpOrigion = (Bitmap)bmp.Clone(rect, PixelFormat.Format32bppArgb);
        }

        public void AssignControls()
        {
            myPen.Dispose();
            myPen = new Pen(Color.Lime, 2);
            myBrush = Brushes.Lime;
        }
        public override string ToString()
        {
            string Str = "";
            string Str1 = "";


            Str = Name + Separator
                + JzTools.RecttoString(myrect) + Separator
                + AliasName + Separator
                + StandardHeight.ToString() + Separator
                + Upperbound.ToString() + Separator
                + Lowerbound.ToString() + Separator
                + ExamDiff.ToString() + Separator
                + BesideDiff.ToString() + Separator
                + ReportIndex.ToString() + Separator;

            Str1 = "";

            foreach (BaseIndicatorClass baseindicator in inBaseIndicator)
            {
                if (baseindicator != null)
                {
                    Str1 += baseindicator.ToString() + ";";
                }
                else
                {
                    Str1 += "0;";
                }
            }

            Str += JzTools.RemoveLastChar(Str1, 1) + Separator;


            Str1 = "";

            foreach (BaseIndicatorClass baseindicator in outBaseIndicator)
            {
                if (baseindicator != null)
                {
                    Str1 += baseindicator.ToString() + ";";
                }
                else
                {
                    Str1 += "0;";
                }
            }

            Str += JzTools.RemoveLastChar(Str1, 1) + Separator;

            Str += D1.ToString() + Separator
                + D2.ToString() + Separator
                + D3.ToString() + Separator
                + D4.ToString() + Separator;

            Str += (IsNoUseArround ? "1" : "0") + Separator;
            Str += (IsNoUseFactor ? "1" : "0") + Separator;
            Str += GoodRatio.ToString() + Separator;
            Str += Adjust.ToString() + Separator;

            Str += CenterStandardHeight.ToString() + Separator;
            
            Str += CenterUpperBound.ToString() + Separator;
            Str += CenterLowerBound.ToString() + Separator;
            Str += XUpperBound.ToString() + Separator;
            Str += YUpperBound.ToString() + Separator;
            Str += Flatness.ToString() + Separator;
            Str += DefinedCode + Separator;
            Str += CornerBesideDiff.ToString() + Separator;
            Str += KeyCode.Trim();
            //Str += AddHeight.ToString();

            return Str;
        }

        public void Dispoe()
        {
            myPen.Dispose();
            myBrush.Dispose();

            bmpOrigion.Dispose();
        }
        Point StringtoPoint(string PtString)
        {
            string[] str = PtString.Split(',');
            return new Point(int.Parse(str[0]), int.Parse(str[1]));
        }
        private void BonudRect(ref Rectangle InnerRect, Size BoundSize)
        {
            InnerRect.X = Math.Min(Math.Max(InnerRect.X, 0), (BoundSize.Width - InnerRect.Width < 0 ? 0 : BoundSize.Width - InnerRect.Width));
            InnerRect.Y = Math.Min(Math.Max(InnerRect.Y, 0), (BoundSize.Height - InnerRect.Height < 0 ? 0 : BoundSize.Height - InnerRect.Height));

            if (BoundSize.Width <= InnerRect.X + InnerRect.Width)
                InnerRect.Width = _BoundValue(InnerRect.Width, BoundSize.Width - InnerRect.X, 1);
            if (BoundSize.Height <= InnerRect.Height + InnerRect.Height)
                InnerRect.Height = _BoundValue(InnerRect.Height, BoundSize.Height - InnerRect.Y, 1);
        }
        private int _BoundValue(int Value, int Max, int Min)
        {
            return Math.Max(Math.Min(Value, Max), Min);
        }

    }
}
