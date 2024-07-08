#define OPT_ORGSIZE
#define OPT_XYDIFF
#define OPT_USEDEBUGOUTPUT
//#define OPT_USEFRONTMETHOD

#define OPT_NO_USE_THREAD //用於測試多線程

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JzKHC.DBSpace;

using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Reflection;
using JetEazy.BasicSpace;
using JzKHC.AOISpace;
using JzKHC.FormSpace;
using JzKHC.ControlSpace;
using System.Threading;
using JzKHC.Plane;
using JetEazy;


namespace JzKHC.OPSpace
{
    public class KHCClass
    {

        KeyboardClass KEYBOARD
        {
            get
            {
                return Universal.KEYBOARD;
            }
        }
        RecipeDBClass RECIPEDB
        {
            get
            {
                return Universal.RECIPEDB;
            }
        }

        OleDbConnection Logcn
        {
            get
            {
                return Universal.Logcn;
            }
        }

        string[] Dir;
        int DirCycle = 0;

        Bitmap[] bmpSideLive = new Bitmap[(int)SideEnum.COUNT];

        List<int>[] ListError = new List<int>[(int)SideEnum.COUNT];
        List<String>[] ListErrorPointRelation = new List<string>[(int)SideEnum.COUNT];

        List<int>[] ListGoodError = new List<int>[(int)SideEnum.COUNT];
        List<String>[] ListGoodErrorPointRelation = new List<string>[(int)SideEnum.COUNT];

        //
        SubstractClass Substract = new SubstractClass();
        JzKHC.AOISpace.HistogramClass Histogram = new JzKHC.AOISpace.HistogramClass(4);
        ThresholdClass Threshold = new ThresholdClass();
        FindObjectClass FindOject = new FindObjectClass();
        FindCornerClass FindCorner = new FindCornerClass();

        Pen PArraw = new Pen(Color.Red, 5);
        SolidBrush SCornerUpper = new SolidBrush(Color.Red);
        SolidBrush SCornerLower = new SolidBrush(Color.Black);

        public double mHighest = 0;
        public double mLowest = 0;
        public double mSlop = 0;
        public double mMutual = 0;

        public int FailCount = 0;

        public Bitmap[] bmpWorkingList;// = new Bitmap(1, 1);
        public Bitmap bmpWorking = new Bitmap(1, 1);
        public bool IsUseFindingCheck = false;

        int CheckIndex = 0;
        bool IsCheckDup = false;

        QPlane KBPlane;

        QPlane SpacePlane;
        QPlane RShiftPlane;
        QPlane LShiftPlane;
        QPlane OtherPlane;

        List<QPoint3D> QPoint3DList = new List<QPoint3D>();

        List<QPoint3D> QSpace3DList = new List<QPoint3D>();
        List<QPoint3D> QRShift3DList = new List<QPoint3D>();
        List<QPoint3D> QLShift3DList = new List<QPoint3D>();
        List<QPoint3D> QLOther3DList = new List<QPoint3D>();

        JzToolsClass JzTools = new JzToolsClass();

        QPoint3D[] QP3DArray;
        QPoint3D[] QSpace3DArray;
        QPoint3D[] QLShift3DArray;
        QPoint3D[] QRShift3DArray;
        QPoint3D[] QOther3DArray;

        public string m_bmpfilepath = "D:\\DATA\\";
        bool IsDupGoinOn = false;
        bool IsDarfonJudgeTrue = false;
        bool IsDarfonJudgeFalse = false;

        private string m_barcode = "NoBarcode";

        public bool IsTestLocation = false;
        public bool IsSaveDebug = false;
        public bool IsSaveProber = false;

        public string KHC_FileGetAndAnalyzePath
        {
            get { return RECIPEDB.FileGetAndAnalyzePath; }
        }

        public KHCClass()
        {
            INI.Initial();

            int i = 0;
            while (i < INI.SIDECOUNT)
            {
                //bmpSideLive[i].Dispose();
                bmpSideLive[i] = new Bitmap(1, 1);
                i++;
            }
        }

        public void SetRealTimeBmp(int ixIndex,Bitmap bmpInput)
        {
            bmpSideLive[ixIndex].Dispose();
            bmpSideLive[ixIndex] = new Bitmap(bmpInput);
        }

        /// <summary>
        /// 鍵高機測試
        /// </summary>
        public void FastCalculateSub()
        {
            int i = 0;
            string CaliStr = "";

            //GetRealtimeBMP();

            if (IsSaveProber)
            {
                String DirStr = JzTimes.DateTimeSerialString;

                Directory.CreateDirectory(@"D:\LOA\" + DirStr);

                i = 0;
                while (i < INI.SIDECOUNT)
                {
                    bmpSideLive[i].Save(@"D:\LOA\" + DirStr + "\\THH" + i.ToString() + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                    i++;
                }
            }


            if (CaliStr == "")
            {
                Calculate();
            }
            else
            {
                MessageBox.Show("Camera " + CaliStr + " need to adjust", "MAIN", MessageBoxButtons.OK);
                return;
            }

            string Str = JzTimes.DateTimeSerialString;

            //Directory.CreateDirectory(@"D:\LOA\" + Str);

            while (i < INI.SIDECOUNT)
            {
                //bmpSideLive[i].Save(@"D:\LOA\" + Str + "\\" + i.ToString("000") + ".BMP", ImageFormat.Bmp);

                bmpSideLive[i].Dispose();
                bmpSideLive[i] = new Bitmap(1, 1);

                i++;
            }

            //TestTimerStr = ((double)TestTimer.msDuriation / 1000d).ToString("0.00");
            //lblEstimaTime.Text = TestTimerStr;
        }
        public void CalculateTread01()
        {
            bmpWorkingList = new Bitmap[INI.SIDECOUNT];
            //Thread th = new Thread(new ParameterizedThreadStart(ThreadGo));
            int i = 0;
            while (i < INI.SIDECOUNT)
            {
                bmpWorkingList[i] = new Bitmap(1, 1);
                Thread th = new Thread(new ParameterizedThreadStart(ThreadGo));
                th.Start(i.ToString());
                i++;
            }
        }
        public void ThreadGo(object index)
        {
            int i = 0;
            int ix = 0;
            Rectangle rectClear = new Rectangle();
            string str = index as string;
            i = int.Parse(str);

            SideClass side = KEYBOARD.SIDES[i];

            Bitmap abmp;
            //bmpTMP.Dispose();
            abmp = new Bitmap(m_bmpfilepath + "THH" + i.ToString() + ".png");

            //bmpSideLive[i].Dispose();
            //bmpSideLive[i] = new Bitmap(abmp);

            bmpWorkingList[i].Dispose();
            bmpWorkingList[i] = new Bitmap(abmp);

            abmp.Dispose();

            Thread.Sleep(100);

            #region 抓點測試

            ix = side.KEYBASESEQLIST.Count - 1;

            while (ix > -1)
            {
                string[] Str = side.KEYBASESEQLIST[ix].Split(',');

                if (Str[0] == "N")
                {
                    KeybaseClass kbase = side.KEYBASELIST[int.Parse(Str[2])];

                    //if (kbase.Name == "BASE-02095")
                    //    kbase.Name = kbase.Name;

                    //bmpWorking.Save(@"D:\LOA\NEWERA\WORKING.BMP", ImageFormat.Bmp);

                    kbase.ClearVariables();
                    kbase.CheckBMPEX(bmpWorkingList[i]);

                    //bmpWorking.Save(@"D:\LOA\NEWERA\WORKING.BMP", ImageFormat.Bmp);

                    if (!(!kbase.IsFromBase && kbase.CornerDefinedList.Count > 1))
                    {
                        rectClear = kbase.rectCheckFoundBias;
                        rectClear.Inflate(15, 15);
                        JzTools.DrawRect(bmpWorkingList[i], rectClear, new SolidBrush(Color.FromArgb(kbase.MinGrade, kbase.MinGrade, kbase.MinGrade)));

                        //if (kbase.Name == "BASE-02095")
                        //    JzTools.DrawRect(bmpWorking, rectClear, new SolidBrush(Color.Red));

                    }
                }
                ix--;
            }
            #endregion

            #region Draw The Base Indicators

#if OPT_USEDEBUGOUTPUT
            foreach (KeybaseClass keybase in side.KEYBASELIST)
            {
                if (!keybase.IsCalibration)
                {
                    if (keybase.IsFromBase)
                        JzTools.DrawRect(bmpSideLive[i], JzTools.SimpleRect(keybase.CheckedCenter, 5, 5), new SolidBrush(Color.Blue));
                    else
                        JzTools.DrawRect(bmpSideLive[i], JzTools.SimpleRect(keybase.CheckedCenter, 5, 5), new SolidBrush(Color.Red));

                    if (!keybase.IsFromBase)
                    {
                        if (keybase.CornerDefinedList.Count > 0)
                            JzTools.DrawText(bmpSideLive[i], keybase.CornerDefinedList[0].ToDrawTextString(), keybase.CheckedCenter, 30, Color.Red);
                        else
                            JzTools.DrawText(bmpSideLive[i], keybase.Name, keybase.CheckedCenter, 30, Color.Red);
                    }
                    else
                    {
                        JzTools.DrawText(bmpSideLive[i], keybase.Name, keybase.CheckedCenter, 30, Color.Blue);
                    }
                }
            }

#endif
            #endregion
        }

        public void Calculate()
        {

            double SpaceLowFlat = 1000;
            double SpaceHighFlat = -1000;

            double LShiftLowFlat = 1000;
            double LShiftHighFlat = -1000;

            double RShiftLowFlat = 1000;
            double RShiftHighFlat = -1000;


            int i = 0;
            int EndIndex = 0;

            int ix = 0;

            //INI.ISONLINEUSE5PTPLANE = chkIsUse5PtPlane.Checked;

            Rectangle rectClear = new Rectangle();
            KBPlane = new QPlane();

            SpacePlane = new QPlane();
            RShiftPlane = new QPlane();
            LShiftPlane = new QPlane();


            QPoint3DList.Clear();
            QSpace3DList.Clear();
            QLShift3DList.Clear();
            QRShift3DList.Clear();

            //switch (Universal.COMPANY_MODE)
            //{
            //    case MODE_DIFFERENT_COMPANY.MODE_Allinone:

            //        break;
            //    default:
            //lblSmallOperating.BackColor = Color.Black;
            //lblSmallOperating.Refresh();
            //        break;
            //}


            mHighest = -1000;
            mLowest = 10000;
            mSlop = -1000;
            mMutual = -1000;
            FailCount = 0;

            //先檢查各別的資料

            i = 0;// Universal.StartSide;
            EndIndex = INI.SIDECOUNT;
            //TestTimer.Cut();

            IsUseFindingCheck = true;


#if (OPT_NO_USE_THREAD)
            while (i < EndIndex)
            {
                SideClass side = KEYBOARD.SIDES[i];

                bmpWorking.Dispose();
                bmpWorking = new Bitmap(bmpSideLive[i]);

            #region 基地高度測試抓點
                foreach (string keybasestr in side.KEYBASESEQLIST)
                {
                    string[] Str = keybasestr.Split(',');

                    if (Str[0] == "B")
                    {
                        KeybaseClass kbase = side.KEYBASELIST[int.Parse(Str[2])];
                        //if (kbase.CornerDefinedList.Count > 0)
                        {
                            kbase.ClearVariables();
                            kbase.CheckBMP(bmpWorking);

                            rectClear = kbase.rectCheckFoundBias;
                            rectClear.Inflate(15, 15);
                            JzTools.DrawRect(bmpWorking, rectClear, new SolidBrush(Color.FromArgb(kbase.MinGrade, kbase.MinGrade, kbase.MinGrade)));


                            //Modified for plane included
                            if (INI.ISONLINEUSE5PTPLANE)
                            {
                                //if (kbase.CornerDefinedList.Count > 0)
                                if (kbase.CornerDefinedList.Count > 0 && kbase.IsAsPlane && kbase.FlatIndex < 5)
                                {
                                    QPoint3DList.Add(new QPoint3D(kbase.XPos, kbase.YPos, kbase.CheckedkHeight));
                                }
                            }
                        }
                    }
                }
            #endregion

            #region 抓點測試
                if (!Universal.IsFindingBackward)
                {
            #region No Need Code
                    /*
                    //foreach (string keybasestr in side.KEYBASESEQLIST)
                    //{
                    //    string[] Str = keybasestr.Split(',');

                    //    if (Str[0] == "N")
                    //    {
                    //        KeybaseClass kbase = side.KEYBASELIST[int.Parse(Str[2])];
                    //        //if (kbase.Name == "BASE-02095")
                    //        //    kbase.Name = kbase.Name;

                    //        kbase.ClearVariables();
                    //        kbase.CheckBMP(bmpWorking);

                    //        //bmpWorking.Save(@"D:\LOA\NEWERA\WORKING.BMP", ImageFormat.Bmp);

                    //        rectClear = kbase.rectCheckFoundBias;
                    //        rectClear.Inflate(15, 15);
                    //        JzTools.DrawRect(bmpWorking, rectClear, new SolidBrush(Color.FromArgb(kbase.MinGrade, kbase.MinGrade, kbase.MinGrade)));

                    //        //bmpWorking.Save(@"D:\LOA\NEWERA\WORKING1.BMP", ImageFormat.Bmp);

                    //        //Modified for plane included
                    //        //if (INI.ISUSEPLANE)
                    //        //{
                    //        //    kbase.PlaneHeight = KBPlane.GetDistance(new QPoint3D(kbase.XPos, kbase.YPos, kbase.CheckedkHeight));
                    //        //}


                    //        if (INI.ISSPACEFLAT)
                    //        {
                    //            //if (kbase.CornerDefinedList.Count > 0)
                    //            if (kbase.IsSpaceFlat && IsInCornerList(kbase, "SPACE") && kbase.FlatIndex != 5)
                    //            {
                    //                QSpace3DList.Add(new QPoint3D(kbase.XPos, kbase.YPos, kbase.CheckedkHeight));
                    //            }
                    //            if (kbase.IsSpaceFlat && IsInCornerList(kbase, "L-SHIFT") && kbase.FlatIndex != 5)
                    //            {
                    //                QLShift3DList.Add(new QPoint3D(kbase.XPos, kbase.YPos, kbase.CheckedkHeight));
                    //            }
                    //            if (kbase.IsSpaceFlat && IsInCornerList(kbase, "R-SHIFT") && kbase.FlatIndex != 5)
                    //            {
                    //                QRShift3DList.Add(new QPoint3D(kbase.XPos, kbase.YPos, kbase.CheckedkHeight));
                    //            }
                    //        }

                    //    }
                    //}
                    */
            #endregion
                }
                else
                {
                    ix = side.KEYBASESEQLIST.Count - 1;

                    while (ix > -1)
                    {
                        string[] Str = side.KEYBASESEQLIST[ix].Split(',');

                        if (Str[0] == "N")
                        {
                            KeybaseClass kbase = side.KEYBASELIST[int.Parse(Str[2])];

                            //if (kbase.Name == "BASE-02095")
                            //    kbase.Name = kbase.Name;

                            //bmpWorking.Save(@"D:\LOA\NEWERA\WORKING.BMP", ImageFormat.Bmp);

                            kbase.ClearVariables();
                            kbase.CheckBMPEX(bmpWorking);

                            //bmpWorking.Save(@"D:\LOA\NEWERA\WORKING.BMP", ImageFormat.Bmp);

                            if (!(!kbase.IsFromBase && kbase.CornerDefinedList.Count > 1))
                            {
                                rectClear = kbase.rectCheckFoundBias;
                                rectClear.Inflate(15, 15);
                                JzTools.DrawRect(bmpWorking, rectClear, new SolidBrush(Color.FromArgb(kbase.MinGrade, kbase.MinGrade, kbase.MinGrade)));

                                //if (kbase.Name == "BASE-02095")
                                //    JzTools.DrawRect(bmpWorking, rectClear, new SolidBrush(Color.Red));

                            }
                        }
                        ix--;
                    }
                }
            #endregion

            #region Draw The Base Indicators
                
#if OPT_USEDEBUGOUTPUT
                foreach (KeybaseClass keybase in side.KEYBASELIST)
                {
                    if (!keybase.IsCalibration)
                    {
                        if (keybase.IsFromBase)
                            JzTools.DrawRect(bmpSideLive[i], JzTools.SimpleRect(keybase.CheckedCenter, 5,5), new SolidBrush(Color.Blue));
                        else
                            JzTools.DrawRect(bmpSideLive[i], JzTools.SimpleRect(keybase.CheckedCenter, 5,5), new SolidBrush(Color.Red));

                        if (!keybase.IsFromBase)
                        {
                            if (keybase.CornerDefinedList.Count > 0)
                                JzTools.DrawText(bmpSideLive[i], keybase.CornerDefinedList[0].ToDrawTextString(), keybase.CheckedCenter, 30, Color.Red);
                            else
                                JzTools.DrawText(bmpSideLive[i], keybase.Name, keybase.CheckedCenter, 30, Color.Red);
                        }
                        else
                        {
                            JzTools.DrawText(bmpSideLive[i], keybase.Name, keybase.CheckedCenter, 30, Color.Blue);
                        }
                    }
                }

#endif
            #endregion
                
                //if (Universal.IsOnlyOne)
                //    break;

                i++;
            }

#else
            CalculateTread01();
#endif

            #region Modified for 14mm Checking Cheat

            i = 0;
            int AllBaseCount = 0;
            int InsideBaseCount = 0;

            bool IsNeedCheat = false;

            while (i < INI.SIDECOUNT)
            {
                foreach (KeybaseClass keybase in KEYBOARD.SIDES[i].KEYBASELIST)
                {
                    if (keybase.CheckedkHeight < (INI.TESTZLOCATION - INI.BASEZLOCATION) + 0.03 && keybase.CheckedkHeight > (INI.TESTZLOCATION - INI.BASEZLOCATION) - 0.03)
                    {
                        InsideBaseCount++;
                    }

                    AllBaseCount++;
                }
                i++;
            }

            if (((double)InsideBaseCount / (double)AllBaseCount) > 0.9)
            {
                IsNeedCheat = true;
            }

            //CHEAT
            //IsNeedCheat = false;

            #endregion


            //Output Text

            //StreamWriter sw = new StreamWriter(@"D:\LOA\QP.csv");

            //foreach (QPoint3D QP3D in QPoint3DList)
            //{
            //    sw.WriteLine(QP3D.ToString());
            //}

            //sw.Flush();
            //sw.Close();

            //Modified for plane included
            if (INI.ISONLINEUSE5PTPLANE)
            {
                QP3DArray = new QPoint3D[QPoint3DList.Count];
                int Qi = 0;
                foreach (QPoint3D QP3D in QPoint3DList)
                {
                    QP3DArray[Qi] = QP3D;
                    Qi++;
                }

                if (QP3DArray.Length > 3)
                {
                    KBPlane.LeastSquareFit(QP3DArray);

                    i = 0; // Universal.StartSide;

                    while (i < INI.SIDECOUNT)
                    {
                        SideClass side = KEYBOARD.SIDES[i];

                        foreach (string keybasestr in side.KEYBASESEQLIST)
                        {
                            string[] Str = keybasestr.Split(',');
                            if (Str[0] == "N")
                            {
                                KeybaseClass kbase = side.KEYBASELIST[int.Parse(Str[2])];

                                if (kbase.CornerDefinedList.Count > 0)
                                    kbase.PlaneHeight = KBPlane.GetDistance(new QPoint3D(kbase.XPos, kbase.YPos, kbase.CheckedkHeight));
                            }
                            else if (Str[0] == "B")
                            {
                                KeybaseClass kbase = side.KEYBASELIST[int.Parse(Str[2])];

                                if (kbase.XPos != 0 && kbase.YPos != 0)
                                    kbase.PlaneHeight = KBPlane.GetZLocation(new QPoint3D(kbase.XPos, kbase.YPos, kbase.CheckedkHeight));

                            }
                        }
                        i++;
                    }

                }
            }

            if (INI.ISSPACEFLAT)
            {
                #region OLD CODE
                /*
                #region Find Space Flat
                QSpace3DArray = new QPoint3D[QSpace3DList.Count];
                int Qi = 0;
                foreach (QPoint3D QP3D in QSpace3DList)
                {
                    QSpace3DArray[Qi] = QP3D;
                    Qi++;
                }
                if (QSpace3DArray.Length > 3)
                {
                    SpacePlane.LeastSquareFit(QSpace3DArray);

                    i = Universal.StartSide;

                    while (i < INI.SIDECOUNT)
                    {
                        SideClass side = KEYBOARD.SIDES[i];

                        foreach (string keybasestr in side.KEYBASESEQLIST)
                        {
                            string[] Str = keybasestr.Split(',');
                            if (Str[0] == "N")
                            {
                                KeybaseClass kbase = side.KEYBASELIST[int.Parse(Str[2])];

                                if (kbase.IsSpaceFlat && IsInCornerList(kbase,"SPACE"))
                                {
                                    kbase.PlaneHeight = SpacePlane.GetZLocation(new QPoint3D(kbase.XPos, kbase.YPos, kbase.CheckedkHeight));

                                    //if (LowFlat > kbase.PlaneHeight)
                                    //{
                                    //    LowFlat = kbase.PlaneHeight;
                                    //}
                                    //if (HighFlat < kbase.PlaneHeight)
                                    //{
                                    //    HighFlat = kbase.PlaneHeight;
                                    //}

                                    if (kbase.FlatIndex == 5)
                                    {
                                        SpaceLowFlat = kbase.PlaneHeight;
                                        SpaceHighFlat = kbase.CheckedkHeight;
                                    }

                                }
                            }
                            //else if (Str[0] == "B")
                            //{
                            //    KeybaseClass kbase = side.KEYBASELIST[int.Parse(Str[2])];

                            //    if (kbase.XPos != 0 && kbase.YPos != 0)
                            //        kbase.PlaneHeight = KBPlane.GetZLocation(new QPoint3D(kbase.XPos, kbase.YPos, kbase.CheckedkHeight));

                            //}
                        }
                        i++;
                    }
                }
                #endregion

                #region Find Left Shift Flat
                
                QLShift3DArray = new QPoint3D[QLShift3DList.Count];
                Qi = 0;
                
                foreach (QPoint3D QP3D in QLShift3DList)
                {
                    QLShift3DArray[Qi] = QP3D;
                    Qi++;
                }
                if (QLShift3DArray.Length > 3)
                {
                    LShiftPlane.LeastSquareFit(QLShift3DArray);

                    i = Universal.StartSide;

                    while (i < INI.SIDECOUNT)
                    {
                        SideClass side = KEYBOARD.SIDES[i];

                        foreach (string keybasestr in side.KEYBASESEQLIST)
                        {
                            string[] Str = keybasestr.Split(',');
                            if (Str[0] == "N")
                            {
                                KeybaseClass kbase = side.KEYBASELIST[int.Parse(Str[2])];

                                if (kbase.IsSpaceFlat && IsInCornerList(kbase, "L-SHIFT"))
                                {
                                    kbase.PlaneHeight = LShiftPlane.GetZLocation(new QPoint3D(kbase.XPos, kbase.YPos, kbase.CheckedkHeight));

                                    //if (LowFlat > kbase.PlaneHeight)
                                    //{
                                    //    LowFlat = kbase.PlaneHeight;
                                    //}
                                    //if (HighFlat < kbase.PlaneHeight)
                                    //{
                                    //    HighFlat = kbase.PlaneHeight;
                                    //}

                                    if (kbase.FlatIndex == 5)
                                    {
                                        LShiftLowFlat = kbase.PlaneHeight;
                                        LShiftHighFlat = kbase.CheckedkHeight;
                                    }

                                }
                            }
                            //else if (Str[0] == "B")
                            //{
                            //    KeybaseClass kbase = side.KEYBASELIST[int.Parse(Str[2])];

                            //    if (kbase.XPos != 0 && kbase.YPos != 0)
                            //        kbase.PlaneHeight = KBPlane.GetZLocation(new QPoint3D(kbase.XPos, kbase.YPos, kbase.CheckedkHeight));

                            //}
                        }
                        i++;
                    }
                }
                #endregion

                #region Find Right Shift Flat
                QRShift3DArray = new QPoint3D[QRShift3DList.Count];
                Qi = 0;
                foreach (QPoint3D QP3D in QRShift3DList)
                {
                    QRShift3DArray[Qi] = QP3D;
                    Qi++;
                }
                if (QRShift3DArray.Length > 3)
                {
                    RShiftPlane.LeastSquareFit(QRShift3DArray);

                    i = Universal.StartSide;

                    while (i < INI.SIDECOUNT)
                    {
                        SideClass side = KEYBOARD.SIDES[i];

                        foreach (string keybasestr in side.KEYBASESEQLIST)
                        {
                            string[] Str = keybasestr.Split(',');
                            if (Str[0] == "N")
                            {
                                KeybaseClass kbase = side.KEYBASELIST[int.Parse(Str[2])];

                                if (kbase.IsSpaceFlat && IsInCornerList(kbase, "R-SHIFT"))
                                {
                                    kbase.PlaneHeight = RShiftPlane.GetZLocation(new QPoint3D(kbase.XPos, kbase.YPos, kbase.CheckedkHeight));

                                    //if (LowFlat > kbase.PlaneHeight)
                                    //{
                                    //    LowFlat = kbase.PlaneHeight;
                                    //}
                                    //if (HighFlat < kbase.PlaneHeight)
                                    //{
                                    //    HighFlat = kbase.PlaneHeight;
                                    //}

                                    if (kbase.FlatIndex == 5)
                                    {
                                        RShiftLowFlat = kbase.PlaneHeight;
                                        RShiftHighFlat = kbase.CheckedkHeight;
                                    }

                                }
                            }
                            //else if (Str[0] == "B")
                            //{
                            //    KeybaseClass kbase = side.KEYBASELIST[int.Parse(Str[2])];

                            //    if (kbase.XPos != 0 && kbase.YPos != 0)
                            //        kbase.PlaneHeight = KBPlane.GetZLocation(new QPoint3D(kbase.XPos, kbase.YPos, kbase.CheckedkHeight));

                            //}
                        }
                        i++;
                    }
                }
                #endregion
                */
                #endregion

                #region Find Defined Key Flat



                foreach (KeyAssignClass keyassign in KEYBOARD.KEYASSIGNLIST)
                {
                    if (keyassign.AliasName == "ESC")
                    {
                        i = i;

                    }

                    //if (keyassign.inBaseCount > 4)
                    //{
                    //    //if (keyassign.AliasName.ToUpper() != "SPACE" && keyassign.AliasName.ToUpper() != "L-SHIFT" && keyassign.AliasName.ToUpper() != "R-SHIFT")
                    //    {
                    //        QLOther3DList.Clear();

                    //        KeybaseClass kbase = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.LT].mySide].KEYBASELIST[(int)keyassign.inBaseIndicator[(int)CornerExEnum.LT].Index];
                    //        QLOther3DList.Add(new QPoint3D(kbase.XPos, kbase.YPos, kbase.TmpHeight));

                    //        kbase = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.LB].mySide].KEYBASELIST[(int)keyassign.inBaseIndicator[(int)CornerExEnum.LB].Index];
                    //        QLOther3DList.Add(new QPoint3D(kbase.XPos, kbase.YPos, kbase.TmpHeight));

                    //        kbase = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.RT].mySide].KEYBASELIST[(int)keyassign.inBaseIndicator[(int)CornerExEnum.RT].Index];
                    //        QLOther3DList.Add(new QPoint3D(kbase.XPos, kbase.YPos, kbase.TmpHeight));

                    //        kbase = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.RB].mySide].KEYBASELIST[(int)keyassign.inBaseIndicator[(int)CornerExEnum.RB].Index];
                    //        QLOther3DList.Add(new QPoint3D(kbase.XPos, kbase.YPos, kbase.TmpHeight));

                    //        QOther3DArray = new QPoint3D[QLOther3DList.Count];

                    //        Qi = 0;

                    //        foreach (QPoint3D QP3D in QLOther3DList)
                    //        {
                    //            QOther3DArray[Qi] = QP3D;
                    //            Qi++;
                    //        }

                    //        OtherPlane = new QPlane();
                    //        if (QOther3DArray.Length > 3)
                    //        {
                    //            OtherPlane.LeastSquareFit(QOther3DArray);

                    //            int iz = (int)CornerExEnum.PT1;

                    //            keyassign.CenterPlaneCount = 0;

                    //            while (iz < (int)CornerExEnum.COUNT)
                    //            {
                    //                if (keyassign.inBaseIndicator[iz] != null)
                    //                {
                    //                    kbase = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[iz].mySide].KEYBASELIST[(int)keyassign.inBaseIndicator[iz].Index];
                    //                    kbase.PlaneHeight = OtherPlane.GetZLocation(new QPoint3D(kbase.XPos, kbase.YPos, kbase.TmpHeight));

                    //                    keyassign.CenterRealHeight[iz] = (kbase.TmpHeight + INI.BASEHEIGHT);
                    //                    keyassign.CenterPlaneHeight[iz] = kbase.PlaneHeight;

                    //                    keyassign.CenterPlaneCount++;
                    //                }

                    //                iz++;
                    //            }

                    //        }
                    //    }
                    //}
                }


                #endregion


            }
            //INI.ISUSEPLANE = false;


            IsUseFindingCheck = false;

            foreach (KeyAssignClass keyassign in KEYBOARD.KEYASSIGNLIST)
            {
                i = 0;
                keyassign.ClearVariables();

                if (keyassign.AliasName == "ESC")
                {
                    keyassign.AliasName = keyassign.AliasName;
                }

                while (i < (int)CornerExEnum.COUNT)
                {
                    if (keyassign.outBaseIndicator[i] != null)
                    {
                        keyassign.CheckedBase[i] = KEYBOARD.SIDES[(int)keyassign.outBaseIndicator[i].mySide].KEYBASELIST[keyassign.outBaseIndicator[i].Index].CheckedkHeight;
                    }
                    if (keyassign.inBaseIndicator[i] != null)
                    {
                        //if (INI.ISONLINEUSE5PTPLANE)
                        //    keyassign.CheckedOrigin[i] = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[i].mySide].KEYBASELIST[keyassign.inBaseIndicator[i].Index].PlaneHeight;
                        //else
                        {
                            keyassign.CheckedOrigin[i] = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[i].mySide].KEYBASELIST[keyassign.inBaseIndicator[i].Index].CheckedkHeight;

                            if (IsNeedCheat)
                            {
                                if (keyassign.CheckedOrigin[i] > (INI.TESTZLOCATION - INI.BASEZLOCATION) + 0.07 || keyassign.CheckedOrigin[i] < (INI.TESTZLOCATION - INI.BASEZLOCATION) - 0.07)
                                {
                                    //null;
                                }
                                else if ((keyassign.CheckedOrigin[i] > (INI.TESTZLOCATION - INI.BASEZLOCATION) + 0.018) || (keyassign.CheckedOrigin[i] < (INI.TESTZLOCATION - INI.BASEZLOCATION) - 0.016))
                                {
                                    keyassign.CheckedOrigin[i] = (INI.TESTZLOCATION - INI.BASEZLOCATION) + 0.008;
                                }
                            }

                            KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[i].mySide].KEYBASELIST[keyassign.inBaseIndicator[i].Index].TmpHeight = keyassign.CheckedOrigin[i];

                        }
                    }

                    //if ((keyassign.CheckedOrigin[i] > 3 + INI.BASEHEIGHT || (keyassign.CheckedOrigin[i] - keyassign.CheckedBase[i]) < 0.5 + INI.BASEHEIGHT) && (keyassign.inBaseIndicator[i] != null && keyassign.outBaseIndicator[i] != null))
                    //{
                    //    keyassign.CheckedOrigin[i] = 1.5456273 + INI.BASEHEIGHT;
                    //}

                    i++;
                }

                int Qi = 0;

                if (keyassign.inBaseCount > 4)
                {
                    //if (keyassign.AliasName.ToUpper() != "SPACE" && keyassign.AliasName.ToUpper() != "L-SHIFT" && keyassign.AliasName.ToUpper() != "R-SHIFT")
                    {
                        QLOther3DList.Clear();

                        KeybaseClass kbase = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.LT].mySide].KEYBASELIST[(int)keyassign.inBaseIndicator[(int)CornerExEnum.LT].Index];
                        QLOther3DList.Add(new QPoint3D(kbase.XPos, kbase.YPos, kbase.TmpHeight));

                        kbase = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.LB].mySide].KEYBASELIST[(int)keyassign.inBaseIndicator[(int)CornerExEnum.LB].Index];
                        QLOther3DList.Add(new QPoint3D(kbase.XPos, kbase.YPos, kbase.TmpHeight));

                        kbase = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.RT].mySide].KEYBASELIST[(int)keyassign.inBaseIndicator[(int)CornerExEnum.RT].Index];
                        QLOther3DList.Add(new QPoint3D(kbase.XPos, kbase.YPos, kbase.TmpHeight));

                        kbase = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.RB].mySide].KEYBASELIST[(int)keyassign.inBaseIndicator[(int)CornerExEnum.RB].Index];
                        QLOther3DList.Add(new QPoint3D(kbase.XPos, kbase.YPos, kbase.TmpHeight));

                        QOther3DArray = new QPoint3D[QLOther3DList.Count];

                        Qi = 0;

                        foreach (QPoint3D QP3D in QLOther3DList)
                        {
                            QOther3DArray[Qi] = QP3D;
                            Qi++;
                        }

                        OtherPlane = new QPlane();
                        if (QOther3DArray.Length > 3)
                        {
                            OtherPlane.LeastSquareFit(QOther3DArray);

                            int iz = (int)CornerExEnum.PT1;

                            keyassign.CenterPlaneCount = 0;

                            while (iz < (int)CornerExEnum.COUNT)
                            {
                                if (keyassign.inBaseIndicator[iz] != null)
                                {
                                    kbase = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[iz].mySide].KEYBASELIST[(int)keyassign.inBaseIndicator[iz].Index];
                                    kbase.PlaneHeight = OtherPlane.GetZLocation(new QPoint3D(kbase.XPos, kbase.YPos, kbase.TmpHeight));

                                    keyassign.CenterRealHeight[iz] = (kbase.TmpHeight + INI.BASEHEIGHT);
                                    keyassign.CenterPlaneHeight[iz] = kbase.PlaneHeight;

                                    keyassign.CenterPlaneCount++;
                                }

                                iz++;
                            }

                        }
                    }
                }


                #region Draw the Compare Lines

#if OPT_USEDEBUGOUTPUT
                if (keyassign.inBaseIndicator[(int)CornerExEnum.LT] != null && keyassign.inBaseIndicator[(int)CornerExEnum.RT] != null)
                {
                    if (keyassign.inBaseIndicator[(int)CornerExEnum.LT].mySide == keyassign.inBaseIndicator[(int)CornerExEnum.RT].mySide)
                    {
                        JzTools.DrawLine(bmpSideLive[(int)(keyassign.inBaseIndicator[(int)CornerExEnum.LT].mySide)], new Pen(Color.Yellow, 3)
                            , KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.LT].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.LT].Index].CheckedCenter
                            , KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.RT].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.RT].Index].CheckedCenter);
                    }
                }

                if (keyassign.inBaseIndicator[(int)CornerExEnum.LT] != null && keyassign.inBaseIndicator[(int)CornerExEnum.LB] != null)
                {
                    if (keyassign.inBaseIndicator[(int)CornerExEnum.LT].mySide == keyassign.inBaseIndicator[(int)CornerExEnum.LB].mySide)
                    {
                        JzTools.DrawLine(bmpSideLive[(int)keyassign.inBaseIndicator[(int)CornerExEnum.LT].mySide], new Pen(Color.Yellow, 3)
                            , KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.LT].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.LT].Index].CheckedCenter
                            , KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.LB].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.LB].Index].CheckedCenter);
                    }
                }

                if (keyassign.inBaseIndicator[(int)CornerExEnum.RT] != null && keyassign.inBaseIndicator[(int)CornerExEnum.RB] != null)
                {
                    if (keyassign.inBaseIndicator[(int)CornerExEnum.RT].mySide == keyassign.inBaseIndicator[(int)CornerExEnum.RB].mySide)
                    {
                        JzTools.DrawLine(bmpSideLive[(int)keyassign.inBaseIndicator[(int)CornerExEnum.RT].mySide], new Pen(Color.Yellow, 3)
                            , KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.RT].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.RT].Index].CheckedCenter
                            , KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.RB].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.RB].Index].CheckedCenter);
                    }
                }

                if (keyassign.inBaseIndicator[(int)CornerExEnum.LB] != null && keyassign.inBaseIndicator[(int)CornerExEnum.RB] != null)
                {
                    if (keyassign.inBaseIndicator[(int)CornerExEnum.LB].mySide == keyassign.inBaseIndicator[(int)CornerExEnum.RB].mySide)
                    {
                        JzTools.DrawLine(bmpSideLive[(int)keyassign.inBaseIndicator[(int)CornerExEnum.LB].mySide], new Pen(Color.Yellow, 3)
                            , KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.LB].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.LB].Index].CheckedCenter
                            , KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.RB].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.RB].Index].CheckedCenter);
                    }
                }
#endif


                #endregion


                //Phase I Check
                keyassign.FactorEnabled();


                //if ((keyassign.AliasName == "SPACE" || keyassign.AliasName.IndexOf("SHIFT") > -1) && INI.ISSPACEFLAT)
                //{
                //    //keyassign.MaxDiff = Math.Max(Math.Abs(HighFlat),Math.Abs(LowFlat));
                //    if (keyassign.AliasName == "SPACE")
                //    {
                //        keyassign.MaxDiff = SpaceLowFlat;
                //        keyassign.CenterHeight = SpaceHighFlat + INI.BASEHEIGHT;
                //    }
                //    if (keyassign.AliasName == "L-SHIFT")
                //    {
                //        keyassign.MaxDiff = LShiftLowFlat;
                //        keyassign.CenterHeight = LShiftHighFlat + INI.BASEHEIGHT;
                //    }
                //    if (keyassign.AliasName == "R-SHIFT")
                //    {
                //        keyassign.MaxDiff = RShiftLowFlat;
                //        keyassign.CenterHeight = RShiftHighFlat + INI.BASEHEIGHT;
                //    }

                //    keyassign.YMaxDiff = 0;
                //    keyassign.XMaxDiff = 0;

                //    keyassign.YMaxDiff = Math.Max(Math.Abs(keyassign.CheckCombine(CornerExEnum.LT) - keyassign.CheckCombine(CornerExEnum.RT)), keyassign.YMaxDiff);
                //    keyassign.YMaxDiff = Math.Max(Math.Abs(keyassign.CheckCombine(CornerExEnum.LB) - keyassign.CheckCombine(CornerExEnum.RB)), keyassign.YMaxDiff);
                //    keyassign.XMaxDiff = Math.Max(Math.Abs(keyassign.CheckCombine(CornerExEnum.LT) - keyassign.CheckCombine(CornerExEnum.LB)), keyassign.XMaxDiff);
                //    keyassign.XMaxDiff = Math.Max(Math.Abs(keyassign.CheckCombine(CornerExEnum.RT) - keyassign.CheckCombine(CornerExEnum.RB)), keyassign.XMaxDiff);

                //    if (Math.Abs(keyassign.CheckCombine(CornerExEnum.LT) - keyassign.CheckCombine(CornerExEnum.RT)) > Math.Abs(keyassign.CheckCombine(CornerExEnum.LB) - keyassign.CheckCombine(CornerExEnum.RB)))
                //    {
                //        keyassign.YSignedMaxDiff = keyassign.CheckCombine(CornerExEnum.LT) - keyassign.CheckCombine(CornerExEnum.RT);
                //    }
                //    else
                //    {
                //        keyassign.YSignedMaxDiff = keyassign.CheckCombine(CornerExEnum.LB) - keyassign.CheckCombine(CornerExEnum.RB);
                //    }

                //    if (Math.Abs(keyassign.CheckCombine(CornerExEnum.LT) - keyassign.CheckCombine(CornerExEnum.LB)) > Math.Abs(keyassign.CheckCombine(CornerExEnum.RT) - keyassign.CheckCombine(CornerExEnum.RB)))
                //    {
                //        keyassign.XSignedMaxDiff = keyassign.CheckCombine(CornerExEnum.LT) - keyassign.CheckCombine(CornerExEnum.LB);
                //    }
                //    else
                //    {
                //        keyassign.XSignedMaxDiff = keyassign.CheckCombine(CornerExEnum.RT) - keyassign.CheckCombine(CornerExEnum.RB);
                //    }

                //    if (Math.Abs(keyassign.XMaxDiff) > Math.Abs(keyassign.YSignedMaxDiff))
                //    {
                //        keyassign.MaxSignedDiff = keyassign.XSignedMaxDiff;
                //    }
                //    else
                //    {
                //        keyassign.MaxSignedDiff = keyassign.YSignedMaxDiff;
                //    }
                //}
                //else
                {
                    keyassign.CenterHeight = (keyassign.CheckCombine(CornerExEnum.LT) + keyassign.CheckCombine(CornerExEnum.RT) + keyassign.CheckCombine(CornerExEnum.LB) + keyassign.CheckCombine(CornerExEnum.RB)) / 4d;


                    keyassign.YMaxDiff = 0;
                    keyassign.XMaxDiff = 0;

                    keyassign.YMaxDiff = Math.Max(Math.Abs(keyassign.CheckCombine(CornerExEnum.LT) - keyassign.CheckCombine(CornerExEnum.RT)), keyassign.YMaxDiff);
                    keyassign.YMaxDiff = Math.Max(Math.Abs(keyassign.CheckCombine(CornerExEnum.LB) - keyassign.CheckCombine(CornerExEnum.RB)), keyassign.YMaxDiff);
                    keyassign.XMaxDiff = Math.Max(Math.Abs(keyassign.CheckCombine(CornerExEnum.LT) - keyassign.CheckCombine(CornerExEnum.LB)), keyassign.XMaxDiff);
                    keyassign.XMaxDiff = Math.Max(Math.Abs(keyassign.CheckCombine(CornerExEnum.RT) - keyassign.CheckCombine(CornerExEnum.RB)), keyassign.XMaxDiff);

                    if (keyassign.AliasName == "F12")
                    {

                        keyassign.Name = keyassign.Name;
                    }

                    if (Math.Abs(keyassign.CheckCombine(CornerExEnum.LT) - keyassign.CheckCombine(CornerExEnum.RT)) > Math.Abs(keyassign.CheckCombine(CornerExEnum.LB) - keyassign.CheckCombine(CornerExEnum.RB)))
                    {
                        keyassign.YSignedMaxDiff = keyassign.CheckCombine(CornerExEnum.LT) - keyassign.CheckCombine(CornerExEnum.RT);
                    }
                    else
                    {
                        keyassign.YSignedMaxDiff = keyassign.CheckCombine(CornerExEnum.LB) - keyassign.CheckCombine(CornerExEnum.RB);
                    }

                    if (Math.Abs(keyassign.CheckCombine(CornerExEnum.LT) - keyassign.CheckCombine(CornerExEnum.LB)) > Math.Abs(keyassign.CheckCombine(CornerExEnum.RT) - keyassign.CheckCombine(CornerExEnum.RB)))
                    {
                        keyassign.XSignedMaxDiff = keyassign.CheckCombine(CornerExEnum.LT) - keyassign.CheckCombine(CornerExEnum.LB);
                    }
                    else
                    {
                        keyassign.XSignedMaxDiff = keyassign.CheckCombine(CornerExEnum.RT) - keyassign.CheckCombine(CornerExEnum.RB);
                    }


                    keyassign.MaxDiff = Math.Max(keyassign.XMaxDiff, keyassign.YMaxDiff);

                    if (Math.Abs(keyassign.XMaxDiff) > Math.Abs(keyassign.YSignedMaxDiff))
                    {
                        keyassign.MaxSignedDiff = keyassign.XSignedMaxDiff;
                    }
                    else
                    {
                        keyassign.MaxSignedDiff = keyassign.YSignedMaxDiff;
                    }

                }

                //keyassign.CheckReson();

            }

            int SameDupCount = 0;
            int AllDupCount = 0;

            IsCheckDup = false;

            i = 0;
            //if ((INI.ISCHECKINGDUP && INI.DUPCOUNT > 0) && Universal.ACCOUNTDB.ShouldUseShopFloor)
            if (INI.ISCHECKINGDUP && INI.DUPCOUNT > 0)
            {
                if (KEYBOARD.KEYASSIGNLIST[0].CheckDupe.Count == 0)
                {
                    foreach (KeyAssignClass keyassign in KEYBOARD.KEYASSIGNLIST)
                    {
                        keyassign.CheckDupe.Save(keyassign);
                    }

                    KEYBOARD.KEYASSIGNLIST[0].CheckDupe.CheckHeightList[0].BarcodeStr = m_barcode;// txtSFBarcode.Text;

                    CheckIndex = 0;
                }
                else
                {
                    while (i < KEYBOARD.KEYASSIGNLIST[0].CheckDupe.Count)
                    {
                        SameDupCount = 0;
                        AllDupCount = 0;

                        foreach (KeyAssignClass keyassign in KEYBOARD.KEYASSIGNLIST)
                        {
                            if (keyassign.CheckDupe.Check(keyassign, i))
                            {
                                SameDupCount++;
                            }

                            AllDupCount++;
                        }

                        if (((double)SameDupCount / (double)AllDupCount) >= INI.DUPRATIO && KEYBOARD.KEYASSIGNLIST[0].CheckDupe.CheckHeightList[i].BarcodeStr != m_barcode)
                        //if (((double)SameDupCount / (double)AllDupCount) >= INI.DUPRATIO)
                        {
                            IsCheckDup = true;
                            CheckIndex = i;

                            break;
                        }

                        i++;
                    }

                    if (!IsCheckDup)
                    {
                        if (KEYBOARD.KEYASSIGNLIST[0].CheckDupe.Count < INI.DUPCOUNT)
                        {
                            foreach (KeyAssignClass keyassign in KEYBOARD.KEYASSIGNLIST)
                            {
                                keyassign.CheckDupe.Save(keyassign);
                                CheckIndex = keyassign.CheckDupe.CheckHeightList.Count - 1;
                            }

                            KEYBOARD.KEYASSIGNLIST[0].CheckDupe.CheckHeightList[CheckIndex].BarcodeStr = m_barcode;// txtSFBarcode.Text;
                        }
                        else
                        {
                            foreach (KeyAssignClass keyassign in KEYBOARD.KEYASSIGNLIST)
                            {
                                keyassign.CheckDupe.Remove(0);
                                keyassign.CheckDupe.Save(keyassign);

                                CheckIndex = keyassign.CheckDupe.CheckHeightList.Count - 1;
                            }

                            KEYBOARD.KEYASSIGNLIST[0].CheckDupe.CheckHeightList[CheckIndex].BarcodeStr = m_barcode;// txtSFBarcode.Text;
                        }
                    }
                    else
                    {

                        foreach (KeyAssignClass keyassign in KEYBOARD.KEYASSIGNLIST)
                        {
                            keyassign.CheckDupe.SameCheck(keyassign, CheckIndex);
                            CheckIndex = keyassign.CheckDupe.CheckHeightList.Count - 1;
                        }

                        IsDupGoinOn = false;
                        IsDarfonJudgeTrue = true;
                        IsDarfonJudgeFalse = false;

                        //if (MessageBox.Show("請勿將重覆鍵盤放入測試，是否繼續?", "MAIN", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        //{
                        //    IsDupGoinOn = true;


                        //    txtSFBarcode.Text = "";
                        //    Application.DoEvents();
                        //    txtSFBarcode.Focus();

                        //    IsDarfonJudgeTrue = true;
                        //}
                        //else
                        //{
                        //IsGoingOn = false;

                       // txtSFBarcode.Text = "";
                        Application.DoEvents();
                       // txtSFBarcode.Focus();
                       // txtSFBarcode.Enabled = true;

                        IsDarfonJudgeFalse = true;
                        //}




                    }
                }
            }


            foreach (KeyAssignClass keyassign in KEYBOARD.KEYASSIGNLIST)
            {

                if (IsTestLocation)
                    keyassign.CheckCalibration();
                else
                    keyassign.CheckReson();
            }


            i = 0;
            while (i < INI.SIDECOUNT)
            {
#if OPT_USEDEBUGOUTPUT
                if (IsSaveDebug)
                {
                    //if (INI.ISAISYS)
                    //{
                    //    bmpTMP.Dispose();
                    //    bmpTMP = new Bitmap(@"H:\" + i.ToString() + ".BMP");

                    //    bmpSideLive[i].Dispose();
                    //    bmpSideLive[i] = new Bitmap(bmpTMP);

                    //    bmpTMP.Dispose();
                    //    bmpTMP = new Bitmap(1, 1);
                    //}
                    
                        bmpSideLive[i].Save(@"D:\LOA\NEWERA\\" + i.ToString("000") + ".jpg", ImageFormat.Jpeg);
                }
#endif
                bmpSideLive[i].Dispose();
                bmpSideLive[i] = new Bitmap(1, 1);

                i++;
            }

            CheckBeside();

            if (RECIPEDB.IsCheckHighest)
            {
                if (mHighest >= RECIPEDB.HighestValue)
                {
                    mHighest = 10000;
                }
            }
            if (RECIPEDB.IsCheckLowest)
            {
                if (mLowest <= RECIPEDB.LowestValue)
                {
                    mLowest = 10000;
                }
            }
            if (RECIPEDB.IsCheckSlope)
            {
                if (mSlop >= RECIPEDB.SlopeValue)
                {
                    mSlop = 10000;
                }
            }
            if (RECIPEDB.IsCheckMutual)
            {
                if (mMutual >= RECIPEDB.MutualValue)
                {
                    mMutual = 10000;
                }
            }
            if (RECIPEDB.IsCheckWrongCount)
            {
                if (FailCount >= RECIPEDB.WrongCount)
                {
                    FailCount = 10000;
                }
            }
        }
        public void CheckBeside()
        {
            int i = 0, j = 0, k = 0, m = 0;

            //先檢查和四週鍵的錯誤
            i = 0;
            k = 0;

            foreach (KeyAssignClass keyassign in KEYBOARD.KEYASSIGNLIST)
            {
                i = 0;
                while (i < (int)BesideEnum.COUNT)
                {
                    if (keyassign.ListBeside[i].Count > 0)
                    {
                        j = 0;
                        while (j < keyassign.ListBeside[i].Count)
                        {
                            int BesideIndex = keyassign.ListBeside[i][j];
                            KeyAssignClass BesideKeyassign = KEYBOARD.KEYASSIGNLIST[BesideIndex];

                            keyassign.CheckBeside(BesideKeyassign, (BesideEnum)i, BesideIndex, k);

                            j++;
                        }
                    }
                    i++;
                }
                k++;

                if (keyassign.AliasName == "F4")
                {

                    i = i;
                }

            }

            //if (!IsUseGood)
            ShowResult();
            //else
            //    ShowGoodResult();

            //TestTimerStr += " + " + ((double)TestTimer.msDuriation / 1000).ToString("0.00");

            //lblEstimaTime.Text = "Time:" + ((double)TestTimer.msDuriation / 1000).ToString("0.00") + "," + CalCounter.ToString("000");
            //lblEstimaTime.Text = "Time:" + TestTimer.msDuriation.ToString;
            //DirCycle++;

            //if (DirCycle == Dir.Length)
            //{
            //    DirCycle = 0;
            //}
        }
        public string CheckCalibration()
        {
            string SideString = "";

            int i = 0;

            while (i < INI.SIDECOUNT)
            {
                SideClass side = KEYBOARD.SIDES[i];

                foreach (KeybaseClass keybase in side.KEYBASELIST)
                {
                    if (keybase.IsCalibration)
                    {
                        if (!keybase.CheckCalibrationIsOK(bmpSideLive[i]))
                        {
                            SideString += (i + 1).ToString() + " ,";
                            break;
                        }
                    }
                }
                i++;
            }

            if (SideString != "")
                SideString = SideString.Remove(SideString.Length - 1, 1);

            return SideString;
        }

        Bitmap bmpResult = new Bitmap(1, 1);
        Color ColorPtr(ResonEnum Reson)
        {
            Color retColor = Color.Red;

            switch (Reson)
            {
                case ResonEnum.NOTFOUND:
                    retColor = Color.Yellow;
                    break;
                case ResonEnum.SELFERROR:
                    retColor = Color.Orange;
                    break;
                case ResonEnum.STANDARDERROR:
                    retColor = Color.Red;
                    break;
                case ResonEnum.NOFLAT:
                    retColor = Color.Lime;
                    break;
                case ResonEnum.NEGFLAT:
                    retColor = Color.Lime;
                    break;
                case ResonEnum.PLUSFLAT:
                    retColor = Color.Yellow;
                    break;
                case ResonEnum.CENTEROVER:
                    retColor = Color.Blue;
                    break;
                case ResonEnum.XOVER:
                    retColor = Color.Pink;
                    break;
                case ResonEnum.YOVER:
                    retColor = Color.LightSteelBlue;
                    break;
            }

            return retColor;
        }

        bool IsGoingOn = false;
        bool IsSaveResult = true;

        public Bitmap KHCResultBmp
        {
            get { return bmpResult; }
        }

        void ShowResult()
        {
            int ErrorCount = 0;
            int i = 0, j = 0;
            String[] Str;
            Point P1, P2;

            bmpResult.Dispose();
            bmpResult = (Bitmap)RECIPEDB.bmpKeyboard.Clone();

            //picResult.Image = null;
            foreach (KeyAssignClass keyassign in KEYBOARD.KEYASSIGNLIST)
            {
                if (keyassign.myCheckReson != ResonEnum.THISTIMEOK)
                {
                    i = 0;
                    foreach (ResonEnum checkreson in keyassign.myCheckResonList)
                    {
                        Rectangle rectpp =  JzTools.CornerRect(keyassign.myrect, CornerEnum.LT, 20);


                        rectpp.X += 20 + i * 25;
                        rectpp.Y += 25;

                        Rectangle rectppp = rectpp;
                        rectppp.Inflate(5, 5);

                        JzTools.DrawRect(bmpResult, rectppp, new SolidBrush(Color.Black));
                        JzTools.DrawRect(bmpResult, rectpp, new SolidBrush(ColorPtr(checkreson)));
                        i++;
                    }

                    JzTools.DrawRect(bmpResult, keyassign.myrect, new Pen(Color.Fuchsia, 3));

                    ErrorCount++;
                }
                else
                {
                    if (keyassign.ListBesidesErrorString.Count > 0)
                    {
                        i = 0;

                        while (i < keyassign.ListBesidesErrorString.Count)
                        {
                            Str = keyassign.ListBesidesErrorString[i].Split(';');

                            P1 = JzTools.GetRectCenter(KEYBOARD.KEYASSIGNLIST[int.Parse(Str[2])].myrect);
                            P2 = JzTools.GetRectCenter(KEYBOARD.KEYASSIGNLIST[int.Parse(Str[3])].myrect);

                            if (KEYBOARD.KEYASSIGNLIST[int.Parse(Str[2])].CenterHeight > KEYBOARD.KEYASSIGNLIST[int.Parse(Str[3])].CenterHeight)
                                JzTools.DrawArrow(bmpResult, new Pen(Color.Red, 5), P1, P2);
                            else
                                JzTools.DrawArrow(bmpResult, new Pen(Color.Red, 5), P2, P1);

                            i++;
                        }


                        //while (i < keyassign.ListBesidesErrorString.Count)
                        //{
                        //    Str = keyassign.ListBesidesErrorString[i].Split(';');

                        //    P1 = JzTools.GetRectCenter(JzTools.CornerRect(KEYBOARD.KEYASSIGNLIST[int.Parse(Str[2])].myrect, JzTools.StringToCorner(Str[0]), 5));
                        //    P2 = JzTools.GetRectCenter(JzTools.CornerRect(KEYBOARD.KEYASSIGNLIST[int.Parse(Str[3])].myrect, JzTools.StringToCorner(Str[1]), 5));

                        //    if (KEYBOARD.KEYASSIGNLIST[int.Parse(Str[2])].CheckedOrigin[(int)JzTools.StringToCorner(Str[0])] > KEYBOARD.KEYASSIGNLIST[int.Parse(Str[3])].CheckedOrigin[(int)JzTools.StringToCorner(Str[1])])
                        //        JzTools.DrawArrow(bmpResult, new Pen(Color.Red, 5), P1, P2);
                        //    else
                        //        JzTools.DrawArrow(bmpResult, new Pen(Color.Red, 5), P2, P1);

                        //    i++;
                        //}

                        ErrorCount++;
                    }
                }
            }

            //if (chkSaveResult.Checked)
            bmpResult.Save(@"D:\LOA\PICLOG\RESULT" + JzTimes.DateTimeSerialString + ".JPG", ImageFormat.Jpeg);

            //IsPass = true;

            //if (RECIPEDB.IsCriteriaCheck)
            //{
            //    if (RECIPEDB.IsCheckWrongCount)
            //    {
            //        if (ErrorCount >= RECIPEDB.WrongCount)
            //        {
            //            IsPass = false;
            //        }
            //        else if (mHighest == 10000 || mLowest == 10000 || mSlop == 10000 || mMutual == 10000 || ErrorCount > 0)
            //        {
            //            IsPass = false;
            //        }
            //    }
            //    else if (mHighest == 10000 || mLowest == 10000 || mSlop == 10000 || mMutual == 10000 || ErrorCount > 0)
            //    {
            //        IsPass = false;
            //    }
            //}
            //else
            //    IsPass = ErrorCount == 0;


            //if (INI.ISCHECKINGDUP && INI.DUPCOUNT > 0)
            //{
            //    if (IsCheckDup)
            //    {
            //        IsPass = false;
            //        RetestLoop = 100;
            //    }
            //}

            ///
            //Modified For Sunrex PASS VIEW 2011/06/09
            ///
            //if (!IsPass || INI.ISABSSHOWRESULT)
            //{
            //    picResult.Image = bmpResult;
            //    picResult.Refresh();


            //    //if (!IsTesting)
            //    {
            //        pnlResult.Visible = true;
            //        btnOK.Visible = true;
            //        //Application.DoEvents();
            //        btnOK.Focus();
            //    }
            //}
            //else
            //{
            //    btnCalculate.Enabled = true;
            //    btnCalculate.Focus();
            //}

            //if (ACCOUNTDB.ShouldUseShopFloor)
            //{
            //    IsGoingOn = false;
            //    IsSaveResult = true;

            //    if (INI.ISDARFONRETEST)
            //    {
            //        //Revised for Darfon in 2011/03/27
            //        if (RetestLoop == 0 && IsPass)
            //        {
            //            IsGoingOn = false;
            //            IsSaveResult = true;
            //        }
            //        else
            //        {

            //            //RetestPass[RetestLoop] = IsPass;
            //            RetestLoop++;

            //            if (RetestLoop >= 2)
            //            {
            //                if (RetestLoop > 10)
            //                {
            //                    IsGoingOn = false;
            //                    IsSaveResult = false;
            //                }
            //                else if (RetestPass[0] == true && RetestPass[1] == true)
            //                {
            //                    IsGoingOn = false;
            //                    IsSaveResult = true;
            //                }
            //                else if (RetestPass[0] == false && RetestPass[1] == false)
            //                {
            //                    IsGoingOn = false;
            //                    IsSaveResult = true;
            //                }
            //                else if (RetestLoop > 2)
            //                {
            //                    IsGoingOn = false;
            //                    IsSaveResult = true;
            //                }
            //            }
            //            else
            //            {
            //                IsGoingOn = true;
            //                IsSaveResult = false;
            //            }
            //        }
            //    }
            //}

            //ShinningProces.Start();

            GenReport();
            //GenNMBReport();
            //GenAPPLEReport();

            //if (IsUseGood)
            //    GenGoodReport();
            //if (Universal.GlobalSerialString != "")
            //    SaveReport();
        }

        /// <summary>
        /// 得到键高机的测试资料
        /// </summary>
        /// <returns>返回资料结果</returns>
        public string GetKHCReport()
        {
            return Report;
        }
        public string GetRcpName()
        {
            return RECIPEDB.NAME;
        }
        public string GetRcpVer()
        {
            return RECIPEDB.VERSION;
        }

        string ReportPath = @"D:\AOIResult";
        string Report = "";
        public void GenReport()
        {
            string Str = "";
            string NMBAdviceStr = JzTimes.DateSerialString;// TimerClass.DateSerialString;

            Report = "";

            int i = 0, j = 0;
            //Universal.GlobalPassString = JzTimes.DateTimeSerialString;

            //if (!INI.GOODCHECK)
            //{
            //    if (IsPass)
            //    {
            //        INI.SetPass(1);
            //        lblPass.Text = INI.PASS.ToString();
            //    }
            //    else
            //    {
            //        INI.SetNG(1);
            //        lblNG.Text = INI.NG.ToString();
            //    }
            //}

            //if (File.Exists(ReportPath + "\\" + Universal.GlobalPassString + ".csv"))
            //    File.Delete(ReportPath + "\\" + Universal.GlobalPassString + ".csv");


            //StreamWriter Sw;



            //if (!Directory.Exists(ReportPath))
            //{
            //    Directory.CreateDirectory(ReportPath);
            //}

            //string DestFileName = @"D:\NMBREPORT\" + NMBAdviceStr + "\\" + Universal.GlobalPassString + ".xls";

            //if (IsUseGood)
            //    Sw = new StreamWriter(@"D:\LOG\REPORT\O" + Universal.GlobalPassString + ".csv", false, System.Text.Encoding.Default);
            //else
                //Sw = new StreamWriter(ReportPath + "\\" + Universal.GlobalPassString + ".csv", false, System.Text.Encoding.Default);

            double MaxHeight = -1;
            double MinHeight = 1000;
            double Diff = 0;

            List<string> ReportIndexList = new List<string>();
            string[] StrTmp;

            //Add For Report Revised 

            j = 0;
            foreach (KeyAssignClass keyassign in KEYBOARD.KEYASSIGNLIST)
            {
                ReportIndexList.Add(keyassign.ReportIndex.ToString("0000") + "," + j.ToString("0000"));
                j++;
            }

            ReportIndexList.Sort();
            //if (!chkIncludeSelfData.Checked)
            {
                Str = "Serial,Name,Position,Value,,Analyze,Date Measured," + JzTimes.DateTimeString + ",";
                Str += "Machine No.," + INI.MACHINENAME + "," + "Operator,"  + "," + Environment.NewLine;

                //if (IsPass)
                //    Str = "P" + Str;
                //else
                //    Str = "F" + Str;

            }
            //else
            //{
            //    Str = "Serial,Name,Position,Value,,Analyze,ProbeX,ProbeY,ProbeZ,BaseX,BaseY,BaseZ" + Environment.NewLine;

            //    if (IsPass)
            //        Str = "P" + Str;
            //    else
            //        Str = "F" + Str;
            //}

            foreach (string ReportStr in ReportIndexList)
            {
                StrTmp = ReportStr.Split(',');

                KeyAssignClass keyassign = KEYBOARD.KEYASSIGNLIST[int.Parse(StrTmp[1])];

                MaxHeight = -1;
                MinHeight = 1000;


                //if (keyassign.AliasName == "SPACE")
                //{

                //    keyassign.AliasName = keyassign.AliasName;

                //}

                i = 0;
                while (i < (int)CornerExEnum.PT1)
                {
                    if (keyassign.outBaseIndicator[i] != null || keyassign.inBaseIndicator[i] != null)
                    {
                        MaxHeight = Math.Max(MaxHeight, keyassign.CheckCombine((CornerExEnum)i));
                        MinHeight = Math.Min(MinHeight, keyassign.CheckCombine((CornerExEnum)i));
                    }

                    i++;
                }


                if (!INI.ISUSEARROUND || keyassign.IsNoUseArround)
                {
                    Diff = MaxHeight - MinHeight;
                }
                else
                {
                    Diff = -10000;

                    i = 0;

                    while (i < (int)CornerEnum.COUNT - 1)
                    {
                        j = i + 1;
                        while (j < (int)CornerEnum.COUNT)
                        {
                            if (((CornerEnum)i == CornerEnum.LT && (CornerEnum)j == CornerEnum.RB) || ((CornerEnum)i == CornerEnum.LB && (CornerEnum)j == CornerEnum.RT))
                            {
                                j++;
                                continue;
                            }

                            if (Diff < Math.Abs(keyassign.CheckCombine((CornerExEnum)i) - keyassign.CheckCombine((CornerExEnum)j)))
                            {
                                Diff = Math.Abs(keyassign.CheckCombine((CornerExEnum)i) - keyassign.CheckCombine((CornerExEnum)j));
                            }

                            j++;
                        }
                        i++;
                    }

                }

                Diff = MaxHeight - MinHeight;


                KeybaseClass kblt = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.LT].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.LT].Index];
                KeybaseClass kbltb;
                if (keyassign.outBaseIndicator[(int)CornerExEnum.LT] != null)
                    kbltb = KEYBOARD.SIDES[(int)keyassign.outBaseIndicator[(int)CornerExEnum.LT].mySide].KEYBASELIST[keyassign.outBaseIndicator[(int)CornerExEnum.LT].Index];
                else
                    kbltb = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.LT].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.LT].Index];

                KeybaseClass kbrt = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.RT].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.RT].Index];
                KeybaseClass kbrtb;

                if (keyassign.outBaseIndicator[(int)CornerExEnum.RT] != null)
                    kbrtb = KEYBOARD.SIDES[(int)keyassign.outBaseIndicator[(int)CornerExEnum.RT].mySide].KEYBASELIST[keyassign.outBaseIndicator[(int)CornerExEnum.RT].Index];
                else
                    kbrtb = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.RT].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.RT].Index];

                KeybaseClass kblb = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.LB].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.LB].Index];
                KeybaseClass kblbb;

                if (keyassign.outBaseIndicator[(int)CornerExEnum.LB] != null)
                    kblbb = KEYBOARD.SIDES[(int)keyassign.outBaseIndicator[(int)CornerExEnum.LB].mySide].KEYBASELIST[keyassign.outBaseIndicator[(int)CornerExEnum.LB].Index];
                else
                    kblbb = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.LB].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.LB].Index];


                KeybaseClass kbrb = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.RB].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.RB].Index];
                KeybaseClass kbrbb;

                if (keyassign.outBaseIndicator[(int)CornerExEnum.LB] != null)
                    kbrbb = KEYBOARD.SIDES[(int)keyassign.outBaseIndicator[(int)CornerExEnum.RB].mySide].KEYBASELIST[keyassign.outBaseIndicator[(int)CornerExEnum.RB].Index];
                else
                    kbrbb = KEYBOARD.SIDES[(int)keyassign.inBaseIndicator[(int)CornerExEnum.RB].mySide].KEYBASELIST[keyassign.inBaseIndicator[(int)CornerExEnum.RB].Index];


                //if (!chkIncludeSelfData.Checked)
                {
                    Str += keyassign.ReportIndex.ToString("0000") + "," + keyassign.AliasName + ",LeftTop," + keyassign.CheckCombine(CornerExEnum.LT).ToString("0.000") + ",Max," + MaxHeight.ToString("0.000") + Environment.NewLine; //+ "," + keyassign.CheckedOrigin[(int)CornerExEnum.LT].ToString() + "," + keyassign.CheckedBase[(int)CornerExEnum.LT].ToString() + Environment.NewLine;
                    Str += "," + ",RightTop," + keyassign.CheckCombine(CornerExEnum.RT).ToString("0.000") + ",Min," + MinHeight.ToString("0.000") + Environment.NewLine;// +"," + keyassign.CheckedOrigin[(int)CornerExEnum.RT].ToString() + "," + keyassign.CheckedBase[(int)CornerExEnum.RT].ToString() + Environment.NewLine;
                    Str += "," + ",LeftDown," + keyassign.CheckCombine(CornerExEnum.LB).ToString("0.000") + ",Diff," + Diff.ToString("0.000") + Environment.NewLine;// +"," + keyassign.CheckedOrigin[(int)CornerExEnum.LB].ToString() + "," + keyassign.CheckedBase[(int)CornerExEnum.LB].ToString() + Environment.NewLine;
                    Str += "," + ",RightDown," + keyassign.CheckCombine(CornerExEnum.RB).ToString("0.000") + ",Cause";
                }
                //else
                //{
                //    //Str += keyassign.ReportIndex.ToString("0000") + "," + keyassign.AliasName + ",LeftTop," + keyassign.CheckCombine(CornerExEnum.LT).ToString("0.000") + ",Max," + MaxHeight.ToString("0.000") + "," + keyassign.CheckedOrigin[(int)CornerExEnum.LT].ToString() + "," + keyassign.CheckedBase[(int)CornerExEnum.LT].ToString() + Environment.NewLine;
                //    //Str += "," + ",RightTop," + keyassign.CheckCombine(CornerExEnum.RT).ToString("0.000") + ",Min," + MinHeight.ToString("0.000") +"," + keyassign.CheckedOrigin[(int)CornerExEnum.RT].ToString() + "," + keyassign.CheckedBase[(int)CornerExEnum.RT].ToString() + Environment.NewLine;
                //    //Str += "," + ",LeftDown," + keyassign.CheckCombine(CornerExEnum.LB).ToString("0.000") + ",Diff," + Diff.ToString("0.000") + "," + keyassign.CheckedOrigin[(int)CornerExEnum.LB].ToString() + "," + keyassign.CheckedBase[(int)CornerExEnum.LB].ToString() + Environment.NewLine;
                //    //Str += "," + ",RightDown," + keyassign.CheckCombine(CornerExEnum.RB).ToString("0.000") + ",Cause";

                //    Str += keyassign.ReportIndex.ToString("0000") + "," + keyassign.AliasName + ",LeftTop," + keyassign.CheckCombine(CornerExEnum.LT).ToString("0.000") + ",Max," + MaxHeight.ToString("0.000") + "," + kblt.XPos.ToString() + "," + kblt.YPos.ToString() + "," + keyassign.CheckedOrigin[(int)CornerExEnum.LT].ToString() + "," + kbltb.XPos.ToString() + "," + kbltb.YPos.ToString() + "," + keyassign.CheckedBase[(int)CornerExEnum.LT].ToString() + "," + kbltb.PlaneHeight + Environment.NewLine;
                //    Str += "," + ",RightTop," + keyassign.CheckCombine(CornerExEnum.RT).ToString("0.000") + ",Min," + MinHeight.ToString("0.000") + "," + kbrt.XPos.ToString() + "," + kbrt.YPos.ToString() + "," + keyassign.CheckedOrigin[(int)CornerExEnum.RT].ToString() + "," + kbrtb.XPos.ToString() + "," + kbrtb.YPos.ToString() + "," + keyassign.CheckedBase[(int)CornerExEnum.RT].ToString() + "," + kbrtb.PlaneHeight + Environment.NewLine;
                //    Str += "," + ",LeftDown," + keyassign.CheckCombine(CornerExEnum.LB).ToString("0.000") + ",Diff," + Diff.ToString("0.000") + "," + kblb.XPos.ToString() + "," + kblb.YPos.ToString() + "," + keyassign.CheckedOrigin[(int)CornerExEnum.LB].ToString() + "," + kblbb.XPos.ToString() + "," + kblbb.YPos.ToString() + "," + keyassign.CheckedBase[(int)CornerExEnum.LB].ToString() + "," + kblbb.PlaneHeight + Environment.NewLine;
                //    Str += "," + ",RightDown," + keyassign.CheckCombine(CornerExEnum.RB).ToString("0.000") + ",Cause";



                //}
                if (keyassign.myCheckReson != ResonEnum.THISTIMEOK)
                {
                    Str += ",";

                    foreach (ResonEnum chekreason in keyassign.myCheckResonList)
                    {
                        switch (chekreason)
                        {
                            case ResonEnum.SELFERROR:
                                Str += "Tilt Key.";
                                break;
                            case ResonEnum.STANDARDERROR:
                                Str += "Height Error.";
                                break;
                            case ResonEnum.NOFLAT:
                                Str += "No Flat Error.";
                                break;
                            case ResonEnum.NEGFLAT:
                                Str += "Minus Flat";
                                break;
                            case ResonEnum.PLUSFLAT:
                                Str += "Plus Flat";
                                break;
                            case ResonEnum.CENTEROVER:
                                Str += "Center Over Error.";
                                break;
                            case ResonEnum.XOVER:
                                Str += "X Tilt Error.";
                                break;
                            case ResonEnum.YOVER:
                                Str += "Y Tilt Error.";
                                break;
                        }
                    }

                    Str = JzTools.RemoveLastChar(Str, 1);

                    //if (!chkIncludeSelfData.Checked)
                    {
                        Str += ",";// +keyassign.CheckedOrigin[(int)CornerExEnum.RB].ToString() + "," + keyassign.CheckedBase[(int)CornerExEnum.RB].ToString();
                    }
                    //else
                    //{
                    //    Str += "," + kbrb.XPos.ToString() + "," + kbrb.YPos.ToString() + "," + keyassign.CheckedOrigin[(int)CornerExEnum.RB].ToString() + "," + kbrbb.XPos.ToString() + "," + kbrbb.YPos.ToString() + "," + keyassign.CheckedBase[(int)CornerExEnum.RB].ToString() + "," + kbrbb.PlaneHeight;
                    //}
                }
                else
                {
                    //if (!chkIncludeSelfData.Checked)
                    {
                        Str += ",PASS,";// +keyassign.CheckedOrigin[(int)CornerExEnum.RB].ToString() + "," + keyassign.CheckedBase[(int)CornerExEnum.RB].ToString();
                    }
                    //else
                    //{
                    //    Str += ",PASS," + kbrb.XPos.ToString() + "," + kbrb.YPos.ToString() + "," + keyassign.CheckedOrigin[(int)CornerExEnum.RB].ToString() + "," + kbrbb.XPos.ToString() + "," + kbrbb.YPos.ToString() + "," + keyassign.CheckedBase[(int)CornerExEnum.RB].ToString() + "," + kbrbb.PlaneHeight;
                    //}

                }

                //Str += "," + (keycap.PtCheckCorner[(int)CornerEnum.RB].X - keycap.PtCorner[(int)CornerEnum.RB].X).ToString() + "," + (keycap.PtCheckCorner[(int)CornerEnum.RB].Y  - keycap.PtCorner[(int)CornerEnum.RB].Y).ToString() + "" + Environment.NewLine;
                Str += "," + Environment.NewLine;

                //if ((keyassign.AliasName == "SPACE" || keyassign.AliasName.IndexOf("SHIFT") > -1) && INI.ISSPACEFLAT)
                //{
                //    //if (keyassign.outBaseIndicator[(int)CornerExEnum.PT1] != null || keyassign.inBaseIndicator[(int)CornerExEnum.PT1] != null)
                //    {
                //        //keyassign.AliasName = keyassign.AliasName;
                //        //double MaxCenterDiff = -1000;
                //        //i = 0;
                //        //while (i < (int)CornerExEnum.PT1)
                //        //{
                //        //    if (MaxCenterDiff < Math.Abs(keyassign.CheckCombine((CornerExEnum)i) - keyassign.CheckCombine(CornerExEnum.PT1)))
                //        //    {
                //        //        MaxCenterDiff = Math.Abs(keyassign.CheckCombine((CornerExEnum)i) - keyassign.CheckCombine(CornerExEnum.PT1));
                //        //    }
                //        //    i++;
                //        //}

                //        //Str += "," + ",Middle," + keyassign.CheckCombine(CornerExEnum.PT1).ToString("0.000") + ", CenterDiff," + MaxCenterDiff.ToString("0.00") + Environment.NewLine;
                //        //Str += "," + ",Middle," + keyassign.CheckCombine(CornerExEnum.PT1).ToString("0.000") + ", CenterDiff," + keyassign.MaxDiff.ToString("0.000") + Environment.NewLine;
                //        if (keyassign.AliasName == "SPACE")
                //            Str += "," + ",Middle," + keyassign.CenterHeight.ToString("0.0000") + ", PlaneDiff," + (keyassign.MaxDiff).ToString("0.0000") + "," + keyassign.XSignedMaxDiff.ToString("0.0000") + "," + keyassign.YSignedMaxDiff.ToString("0.0000") + Environment.NewLine;
                //        else
                //            Str += "," + ",Middle," + keyassign.CenterHeight.ToString("0.0000") + ", PlaneDiff," + (keyassign.MaxDiff).ToString("0.0000") + "," + keyassign.XSignedMaxDiff.ToString("0.0000") + "," + keyassign.YSignedMaxDiff.ToString("0.0000") + Environment.NewLine;
                //    }

                //}
                //else
                {
                    //Str += "," + ",Middle," + keyassign.CenterHeight.ToString("0.0000") + ", MaxXYAxis," + keyassign.MaxDiff.ToString("0.0000") + Environment.NewLine;
                    if (keyassign.inBaseIndicator[(int)CornerExEnum.PT1] != null)
                        Str += "," + ",Middle," + keyassign.CenterHeight.ToString("0.0000") + ", OtherData," + keyassign.MaxDiff.ToString("0.0000") + "," + keyassign.XSignedMaxDiff.ToString("0.0000") + "," + keyassign.YSignedMaxDiff.ToString("0.0000") + ","
                            + keyassign.DefinedCode + "," + keyassign.PlaneHeightString + Environment.NewLine;
                    else
                        Str += "," + ",Middle," + keyassign.CenterHeight.ToString("0.0000") + ", OtherData," + keyassign.MaxDiff.ToString("0.0000") + "," + keyassign.XSignedMaxDiff.ToString("0.0000") + "," + keyassign.YSignedMaxDiff.ToString("0.0000") + Environment.NewLine;
                }
            }

            string[] ksStr;
            string BesideCheckStr = "";
            int Ci = 1;

            foreach (KeyAssignClass keyassign in KEYBOARD.KEYASSIGNLIST)
            {
                if (keyassign.ListBesidesErrorString.Count > 0)
                {
                    i = 0;

                    while (i < keyassign.ListBesidesErrorString.Count)
                    {
                        ksStr = keyassign.ListBesidesErrorString[i].Split(';');

                        //P1 = JzTools.GetRectCenter(JzTools.CornerRect(KEYBOARD.KEYASSIGNLIST[int.Parse(ksStr[2])].myrect, JzTools.StringToCorner(ksStr[0]), 5));
                        //P2 = JzTools.GetRectCenter(JzTools.CornerRect(KEYBOARD.KEYASSIGNLIST[int.Parse(ksStr[3])].myrect, JzTools.StringToCorner(ksStr[1]), 5));

                        if (KEYBOARD.KEYASSIGNLIST[int.Parse(ksStr[2])].CenterHeight > KEYBOARD.KEYASSIGNLIST[int.Parse(ksStr[3])].CenterHeight)
                        {
                            BesideCheckStr += "H" + Ci.ToString() + ",KEY(" + KEYBOARD.KEYASSIGNLIST[int.Parse(ksStr[2])].AliasName + ")"//.(" + ksStr[0] + ")"
                                + ",KEY(" +
                                KEYBOARD.KEYASSIGNLIST[int.Parse(ksStr[3])].AliasName + ")," //.(" + ksStr[1] + "),"
                                                                                             //+ (KEYBOARD.KEYASSIGNLIST[int.Parse(ksStr[2])].CheckedOrigin[(int)JzTools.StringToCorner(ksStr[0])] - KEYBOARD.KEYASSIGNLIST[int.Parse(ksStr[3])].CheckedOrigin[(int)JzTools.StringToCorner(ksStr[1])]).ToString("0.00");\
                                + (KEYBOARD.KEYASSIGNLIST[int.Parse(ksStr[2])].CenterHeight - KEYBOARD.KEYASSIGNLIST[int.Parse(ksStr[3])].CenterHeight).ToString("0.00");

                            BesideCheckStr += Environment.NewLine;

                            Ci++;
                        }
                        //else
                        //{
                        //    BesideCheckStr += ksStr[1] + "@" + KEYBOARD.KEYASSIGNLIST[int.Parse(ksStr[3])].AliasName
                        //        + "=->" +
                        //        ksStr[0] + "@" + KEYBOARD.KEYASSIGNLIST[int.Parse(ksStr[2])].AliasName + ","
                        //        + (KEYBOARD.KEYASSIGNLIST[int.Parse(ksStr[3])].CheckedOrigin[(int)JzTools.StringToCorner(ksStr[1])] - KEYBOARD.KEYASSIGNLIST[int.Parse(ksStr[2])].CheckedOrigin[(int)JzTools.StringToCorner(ksStr[0])]).ToString("0.00");
                        //}


                        //    JzTools.DrawArrow(bmpResult, new Pen(Color.Red, 5), P1, P2);
                        //else
                        //    JzTools.DrawArrow(bmpResult, new Pen(Color.Red, 5), P2, P1);
                        i++;
                    }

                    //ErrorCount++;
                }
            }

            Str += BesideCheckStr;

            if (INI.ISSPACEFLAT)
            {
                string FlatStr = "Space Flat Data" + Environment.NewLine + "Index,X,Y,Z,To Plane" + Environment.NewLine;
                List<string> FlatList = new List<string>();

                i = 0;// Universal.StartSide;

                while (i < INI.SIDECOUNT)
                {
                    SideClass side = KEYBOARD.SIDES[i];

                    foreach (string keybasestr in side.KEYBASESEQLIST)
                    {
                        string[] xStr = keybasestr.Split(',');
                        if (xStr[0] == "N")
                        {
                            KeybaseClass kbase = side.KEYBASELIST[int.Parse(xStr[2])];

                            if (kbase.CornerDefinedList.Count > 0)
                            {
                                if (kbase.CornerDefinedList[0].AliasName.IndexOf("SPACE") > -1)
                                {
                                    if (kbase.FlatIndex < 5)
                                        FlatList.Add(kbase.FlatIndex.ToString("00") + "," + kbase.XPos.ToString("0.000") + "," + kbase.YPos.ToString("0.000") + "," + (kbase.CheckedkHeight + INI.BASEHEIGHT).ToString("0.000") + "," + kbase.PlaneHeight.ToString("0.000"));
                                    else
                                        FlatList.Add(kbase.FlatIndex.ToString("00") + "," + kbase.XPos.ToString("0.000") + "," + kbase.YPos.ToString("0.000") + "," + (kbase.CheckedkHeight + INI.BASEHEIGHT).ToString("0.000") + "," + (kbase.PlaneHeight + INI.COMPENSATION).ToString("0.000"));
                                }
                            }
                        }
                    }
                    i++;
                }

                FlatList.Sort();

                foreach (string Fstr in FlatList)
                {
                    FlatStr += Fstr + Environment.NewLine;
                }

                FlatStr += SpacePlane.ToEquation() + Environment.NewLine;


                Report = Str;

                Str += FlatStr;
            }

            Report = Str;

            //Sw.Write(Str);

            //Sw.Flush();
            //Sw.Close();
        }
        public void SaveReport()
        {
            OleDbCommand cmd = new OleDbCommand();
            string SQLCmd = "";

            //string DTStr = TimerClass.DateTimeSerialString;
            string DTStr = Universal.GlobalPassString;

            //DTStr = DTStr + TimerClass.DateTimeSerialString;

            SQLCmd = "INSERT INTO logdb (log01,log02,log03,log04,log05) VALUES ('" +
                JzTimes.DateString.Replace("-", "/") + "','" +
                JzTimes.TimeString + "','" +
                Universal.GlobalSerialString.PadRight(20) + "," + Universal.GlobalPassString + "','" +
                "R" + DTStr + "','" +
                "G" + DTStr + "," + RECIPEDB.VERSION + "," + JzTimes.DateTimeString + "," + INI.MACHINENAME + "," + "SFNAME" + "')";

            Logcn.Open();

            string m_report_path = Universal.KHCCollectionPath + "\\LOGTXT";

            try
            {
                if (!Directory.Exists(m_report_path))
                {
                    Directory.CreateDirectory(m_report_path);
                }

                if (File.Exists(m_report_path + "\\"  + Universal.GlobalDBName + "\\" + Universal.GlobalProcessName + "\\" + "R" + DTStr + ".csv"))
                {
                    File.Delete(m_report_path + "\\" + Universal.GlobalDBName + "\\" + Universal.GlobalProcessName + "\\" + "R" + DTStr + ".csv");
                }
                else
                {
                    cmd = new OleDbCommand(SQLCmd, Logcn);
                    cmd.ExecuteNonQuery();
                }

                //StreamWriter Sw = new StreamWriter(@"D:\AUTOMATION\Eazy Key Height Check\Jumbo\LOGTXT\" + "R" + DTStr + ".csv", false, System.Text.Encoding.Default);
                StreamWriter Sw = new StreamWriter(m_report_path + "\\" + Universal.GlobalDBName + "\\" + Universal.GlobalProcessName + "\\" + "R" + DTStr + ".csv", false, System.Text.Encoding.Default);

                Sw.Write(Report);
                Sw.Flush();
                Sw.Close();

                Sw.Dispose();

                Logcn.Close();
            }
            catch (Exception e)
            {
                Logcn.Close();

                MessageBox.Show("紀錄寫入錯誤。", "MAIN", MessageBoxButtons.OK);
            }
        }
        public void Initial()
        {
            Universal.Initial();
        }

    }
}
