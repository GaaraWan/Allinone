using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;
using JzScreenPoints.BasicSpace;
using JzScreenPoints;

namespace JzScreenPoints
{
    enum INIEnum
    {
        //[CCD Setup]
        ALLSIZERATIO,

        CCD_KIND,
        CCD_SIDECOUNT,
        CCD_SHOWCOUNT,

        CCD_TYPE,
        CCD_WIDTH,
        CCD_HEIGHT,
        CCD_HEAD,
        CCD_ROTATE,
        CCD_IP,
        CCD_PORT,

        CCD2_TYPE,
        CCD2_WIDTH,
        CCD2_HEIGHT,
        CCD2_HEAD,
        CCD2_ROTATE,
        CCD2_IP,
        CCD2_PORT,

        CCD3_TYPE,
        CCD3_WIDTH,
        CCD3_HEIGHT,
        CCD3_HEAD,
        CCD3_ROTATE,
        CCD3_IP,
        CCD3_PORT,

        CCD4_TYPE,
        CCD4_WIDTH,
        CCD4_HEIGHT,
        CCD4_HEAD,
        CCD4_ROTATE,
        CCD4_IP,
        CCD4_PORT,

        //[Baisc Control]
        MACHINENAME,            //文字，機器名稱
        LANGUAGE,               //數字，0是中文，1是英文
        DELAYTIME,
        ISNOLIVE,               //停止線上抓圖以減少掉線機會
        SHOPFLOORPATH,
        PRELOADINDEX,           //預載入的資料
        VIEWLOGCOUNT,           //可載入的上限
        ISCALIBLACK,            //校正的小圓是否為黑的
        PICKPOSITION,           //拿光機的位置
        WBSHIFT,                //白平衡偏移的位置
        ISUSEWB,                //是否使用白平衡

        UNBALANCEVAL,           //UnBalance 預設值
        AREATHRESHOLD,          //最後找Area 的門限值

        ISUSEJPMETHOD,          //使用阿本的方式算
        JPMETHODRANGE,          //阿本方式的上限
        ISUSEDEFINESTARTFOCUS,  //是否使用參數設定的焦距

        TRMIN,                  //TR值的最小值
        TRMAX,                  //TR值的最大值

        ORG_ID,                 //報表中的ORG_ID
        ONDELAYTIME,            //調整開機後的延時


        JUMBO_SERVER_IP,
        JUMBO_SERVER_PORT,

        //[Allinone Control]
        Allinone_Pt_Start,
        Allinone_Pt_End,
        Allinone_Offset,
        Allinone_Offset_ADDX,
        Allinone_Offset_ADDY,
        Allinone_IsNoUseMotion,
        Allinone_Calibrate_Cam_Index,
        Allinone_Cam_Ratio,
    }

    class INI
    {
        #region INI Access Functions
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]

        //private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal,
        //    int size, string filePath);

        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal,
            int size, string filePath);

        static void WriteINIValue(string section, string key, string value, string filepath)
        {
            WritePrivateProfileString(section, key, value, filepath);
        }
        static string ReadINIValue(string section, string key, string defaultvaluestring, string filepath)
        {
            string retStr = "";
            //StringBuilder temp = new StringBuilder(100);
            StringBuilder temp = new StringBuilder(100);
            int Length = GetPrivateProfileString(section, key, "", temp, 100, filepath);

            retStr = temp.ToString();

            if (retStr == "")
                retStr = defaultvaluestring;

            return retStr;

        }
        #endregion

        static string MAINPATH = "";
        static string INIFILE = "";
        static string LOCATIONFILE = "";
        public static Size CCDVIEWSIZE = new Size();

        //static JzToolsClass JzTools = new JzToolsClass();

        //[CCD Setup]

        public static float ALLSIZERATIO = 1f;
        public static int CCD_KIND = 1;
        public static int CCD_SIDECOUNT = 3;
        public static int CCD_SHOWCOUNT = 3;

        public static string CCDSEQSTRING = "";

        public static int CCD_TOTALHEAD = 0;
        public static int CCD_MAXWIDTH = 640;
        public static int CCD_MAXHEIGHT = 480;
        public static int CCD_MINWIDTH = 640000;
        public static int CCD_MINHEIGHT = 480000;

        public static CCDTYPEEnum CCD_TYPE = CCDTYPEEnum.FILE;
        public static int CCD_WIDTH = 2592;
        public static int CCD_HEIGHT = 1944;
        public static int CCD_HEAD = 3;
        public static int CCD_ROTATE = 0;
        public static string CCD_IP = "127.0.0.1";
        public static int CCD_PORT = 34100;
        public static SizeF CCD_RATIO = new SizeF();

        public static CCDTYPEEnum CCD2_TYPE = CCDTYPEEnum.FILE;
        public static int CCD2_WIDTH = 640;
        public static int CCD2_HEIGHT = 480;
        public static int CCD2_HEAD = 3;
        public static int CCD2_ROTATE = 0;
        public static string CCD2_IP = "127.0.0.1";
        public static int CCD2_PORT = 34100;
        public static SizeF CCD2_RATIO = new SizeF();

        public static CCDTYPEEnum CCD3_TYPE = CCDTYPEEnum.FILE;
        public static int CCD3_WIDTH = 640;
        public static int CCD3_HEIGHT = 480;
        public static int CCD3_HEAD = 3;
        public static int CCD3_ROTATE = 0;
        public static string CCD3_IP = "127.0.0.1";
        public static int CCD3_PORT = 34100;
        public static SizeF CCD3_RATIO = new SizeF();

        public static CCDTYPEEnum CCD4_TYPE = CCDTYPEEnum.FILE;
        public static int CCD4_WIDTH = 640;
        public static int CCD4_HEIGHT = 480;
        public static int CCD4_HEAD = 3;
        public static int CCD4_ROTATE = 0;
        public static string CCD4_IP = "127.0.0.1";
        public static int CCD4_PORT = 34100;
        public static SizeF CCD4_RATIO = new SizeF();

        //[Jumbo Control]
        public static string JUMBO_SERVER_IP = "127.0.0.1";
        public static int JUMBO_SERVER_PORT = 34400;

        //[Baisc Control]
        public static string MACHINENAME = "STEROPES";
        public static int LANGUAGE = 0;
        public static int DELAYTIME = 1000;
        public static bool ISNOLIVE = false;
        public static string SHOPFLOORPATH = @"D:\DATA";
        public static string PRELOADINDEX = "";
        public static int VIEWLOGCOUNT = 5;
        public static bool ISCALIBLACK = true;
        public static float PICKPOSITION = 100f;
        public static int WBSHIFT = 20;
        public static bool ISUSEWB = false;
        public static float UNBALANCEVAL = 20f;

        public static int AREATHRESHOLD = 40;
        public static bool ISUSEJPMETHOD = false;

        public static float JPMETHODRANGE = 0.001f;
        public static bool ISUSEDEFINESTARTFOCUS = false;

        public static float TRMIN = 0.6f;
        public static float TRMAX = 0.8f;

        public static string ORG_ID = "2";
        public static int ONDELAYTIME = 10000;

        //[Allinone Control]
        public static Point Allinone_Pt_Start = new Point(0, 0);
        public static Point Allinone_Pt_End = new Point(0, 0);
        public static int Allinone_Offset = 20;
        public static int Allinone_Offset_ADDX = 2;
        public static int Allinone_Offset_ADDY = 2;
        public static bool Allinone_IsNoUseMotion = false;

        public static int Allinone_Calibrate_Cam_Index = 0;
        public static float Allinone_Cam_Ratio = 0.2f;

        //Inside Control
        public static Point[] CCDLOCATIONS;
        public static Point[] TmpCCDLOCATIONS; //於移動總體畫面上的個別畫面時用的暫存位置

        public static void Initial()
        {
            MAINPATH = Universal.JSPCollectionPath;
            INIFILE = MAINPATH + "\\CONFIG.ini";
            LOCATIONFILE = Universal.JSPCollectionPath + "\\CCDLOCATION.ini";

            Load();
        }

        public static void Load()
        {
            //Get [CCD Setup] Parameters
            ALLSIZERATIO = float.Parse(ReadINIValue("CCD Setup", INIEnum.ALLSIZERATIO.ToString(), ALLSIZERATIO.ToString(), INIFILE));

            CCD_KIND = int.Parse(ReadINIValue("CCD Setup", INIEnum.CCD_KIND.ToString(), CCD_KIND.ToString(),INIFILE));
            CCD_SIDECOUNT = int.Parse(ReadINIValue("CCD Setup", INIEnum.CCD_SIDECOUNT.ToString(), CCD_SIDECOUNT.ToString(), INIFILE));
            CCD_SHOWCOUNT = int.Parse(ReadINIValue("CCD Setup", INIEnum.CCD_SHOWCOUNT.ToString(), CCD_SHOWCOUNT.ToString(), INIFILE));

            CCD_TYPE = (CCDTYPEEnum)Enum.Parse(typeof(CCDTYPEEnum), ReadINIValue("CCD Setup", INIEnum.CCD_TYPE.ToString(), CCD_TYPE.ToString(), INIFILE), false);
            CCD_WIDTH = int.Parse(ReadINIValue("CCD Setup", INIEnum.CCD_WIDTH.ToString(), CCD_WIDTH.ToString(), INIFILE));
            CCD_HEIGHT = int.Parse(ReadINIValue("CCD Setup", INIEnum.CCD_HEIGHT.ToString(), CCD_HEIGHT.ToString(), INIFILE));
            CCD_HEAD = int.Parse(ReadINIValue("CCD Setup", INIEnum.CCD_HEAD.ToString(), CCD_HEAD.ToString(), INIFILE));
            CCD_ROTATE = int.Parse(ReadINIValue("CCD Setup", INIEnum.CCD_ROTATE.ToString(), CCD_ROTATE.ToString(), INIFILE));
            CCD_IP = ReadINIValue("CCD Setup", INIEnum.CCD_IP.ToString(), CCD_IP.ToString(), INIFILE);
            CCD_PORT = int.Parse(ReadINIValue("CCD Setup", INIEnum.CCD_PORT.ToString(), CCD_PORT.ToString(), INIFILE));

            CCD_TOTALHEAD = CCD_HEAD;
            CCD_MAXWIDTH = CCD_WIDTH;
            CCD_MAXHEIGHT = CCD_HEIGHT;
            CCD_MINWIDTH = CCD_WIDTH;
            CCD_MINHEIGHT = CCD_HEIGHT;
            CCDSEQSTRING = CCDSEQSTRING + "".PadLeft(CCD_HEAD, '1');

            CCD2_TYPE = (CCDTYPEEnum)Enum.Parse(typeof(CCDTYPEEnum), ReadINIValue("CCD Setup", INIEnum.CCD2_TYPE.ToString(), CCD2_TYPE.ToString(), INIFILE), false);
            CCD2_WIDTH = int.Parse(ReadINIValue("CCD Setup", INIEnum.CCD2_WIDTH.ToString(), CCD2_WIDTH.ToString(), INIFILE));
            CCD2_HEIGHT = int.Parse(ReadINIValue("CCD Setup", INIEnum.CCD2_HEIGHT.ToString(), CCD2_HEIGHT.ToString(), INIFILE));
            CCD2_HEAD = int.Parse(ReadINIValue("CCD Setup", INIEnum.CCD2_HEAD.ToString(), CCD2_HEAD.ToString(), INIFILE));
            CCD2_ROTATE = int.Parse(ReadINIValue("CCD Setup", INIEnum.CCD2_ROTATE.ToString(), CCD2_ROTATE.ToString(), INIFILE));
            CCD2_IP = ReadINIValue("CCD Setup", INIEnum.CCD2_IP.ToString(), CCD2_IP.ToString(), INIFILE);
            CCD2_PORT = int.Parse(ReadINIValue("CCD Setup", INIEnum.CCD2_PORT.ToString(), CCD2_PORT.ToString(), INIFILE));
            
            if (CCD_KIND > 1)
            {
                CCD_TOTALHEAD += CCD2_HEAD;
                CCDSEQSTRING = CCDSEQSTRING + "".PadLeft(CCD2_HEAD, '2');
            }

            CCD_MAXWIDTH = Math.Max(CCD_MAXWIDTH, CCD2_WIDTH);
            CCD_MAXHEIGHT = Math.Max(CCD_MAXHEIGHT, CCD2_HEIGHT);

            if (CCD_KIND > 1)
            {
                CCD_MINWIDTH = Math.Min(CCD_MINWIDTH, CCD2_WIDTH);
                CCD_MINHEIGHT = Math.Min(CCD_MINHEIGHT, CCD2_HEIGHT);
            }
            CCD3_TYPE = (CCDTYPEEnum)Enum.Parse(typeof(CCDTYPEEnum), ReadINIValue("CCD Setup", INIEnum.CCD3_TYPE.ToString(), CCD3_TYPE.ToString(), INIFILE), false);
            CCD3_WIDTH = int.Parse(ReadINIValue("CCD Setup", INIEnum.CCD3_WIDTH.ToString(), CCD3_WIDTH.ToString(), INIFILE));
            CCD3_HEIGHT = int.Parse(ReadINIValue("CCD Setup", INIEnum.CCD3_HEIGHT.ToString(), CCD3_HEIGHT.ToString(), INIFILE));
            CCD3_HEAD = int.Parse(ReadINIValue("CCD Setup", INIEnum.CCD3_HEAD.ToString(), CCD3_HEAD.ToString(), INIFILE));
            CCD3_ROTATE = int.Parse(ReadINIValue("CCD Setup", INIEnum.CCD3_ROTATE.ToString(), CCD3_ROTATE.ToString(), INIFILE));
            CCD3_IP = ReadINIValue("CCD Setup", INIEnum.CCD3_IP.ToString(), CCD3_IP.ToString(), INIFILE);
            CCD3_PORT = int.Parse(ReadINIValue("CCD Setup", INIEnum.CCD3_PORT.ToString(), CCD3_PORT.ToString(), INIFILE));
            //CCD3_INDEX = ReadINIValue("Basic", INIEnum.CCD3_INDEX.ToString(), "0,1");

            if (CCD_KIND > 2)
            {
                CCD_TOTALHEAD += CCD3_HEAD;
                CCDSEQSTRING = CCDSEQSTRING + "".PadLeft(CCD3_HEAD, '3');
            }

            CCD_MAXWIDTH = Math.Max(CCD_MAXWIDTH, CCD3_WIDTH);
            CCD_MAXHEIGHT = Math.Max(CCD_MAXHEIGHT, CCD3_HEIGHT);

            if (CCD_KIND > 2)
            {
                CCD_MINWIDTH = Math.Min(CCD_MINWIDTH, CCD3_WIDTH);
                CCD_MINHEIGHT = Math.Min(CCD_MINHEIGHT, CCD3_HEIGHT);
            }
            CCD4_TYPE = (CCDTYPEEnum)Enum.Parse(typeof(CCDTYPEEnum), ReadINIValue("CCD Setup", INIEnum.CCD4_TYPE.ToString(), CCD4_TYPE.ToString(), INIFILE), false);
            CCD4_WIDTH = int.Parse(ReadINIValue("CCD Setup", INIEnum.CCD4_WIDTH.ToString(), CCD4_WIDTH.ToString(), INIFILE));
            CCD4_HEIGHT = int.Parse(ReadINIValue("CCD Setup", INIEnum.CCD4_HEIGHT.ToString(), CCD4_HEIGHT.ToString(), INIFILE));
            CCD4_HEAD = int.Parse(ReadINIValue("CCD Setup", INIEnum.CCD4_HEAD.ToString(), CCD4_HEAD.ToString(), INIFILE));
            CCD4_ROTATE = int.Parse(ReadINIValue("CCD Setup", INIEnum.CCD4_ROTATE.ToString(), CCD4_ROTATE.ToString(), INIFILE));
            CCD4_IP = ReadINIValue("CCD Setup", INIEnum.CCD4_IP.ToString(), CCD4_IP.ToString(), INIFILE);
            CCD4_PORT = int.Parse(ReadINIValue("CCD Setup", INIEnum.CCD4_PORT.ToString(), CCD4_PORT.ToString(), INIFILE));
            //CCD4_INDEX = ReadINIValue("Basic", INIEnum.CCD4_INDEX.ToString(), "0,1");

            if (CCD_KIND > 3)
            {
                CCD_TOTALHEAD += CCD4_HEAD;
                CCDSEQSTRING = CCDSEQSTRING + "".PadLeft(CCD4_HEAD, '4');
            }

            CCD_MAXWIDTH = Math.Max(CCD_MAXWIDTH, CCD4_WIDTH);
            CCD_MAXHEIGHT = Math.Max(CCD_MAXHEIGHT, CCD4_HEIGHT);

            if (CCD_KIND > 3)
            {
                CCD_MINWIDTH = Math.Min(CCD_MINWIDTH, CCD4_WIDTH);
                CCD_MINHEIGHT = Math.Min(CCD_MINHEIGHT, CCD4_HEIGHT);
            }

            CCD_RATIO = new SizeF(((float)CCD_MINWIDTH / (float)CCD_WIDTH), ((float)CCD_MINHEIGHT / (float)CCD_HEIGHT));
            CCD2_RATIO = new SizeF(((float)CCD_MINWIDTH / (float)CCD2_WIDTH), ((float)CCD_MINHEIGHT / (float)CCD2_HEIGHT));
            CCD3_RATIO = new SizeF(((float)CCD_MINWIDTH / (float)CCD3_WIDTH), ((float)CCD_MINHEIGHT / (float)CCD3_HEIGHT));
            CCD4_RATIO = new SizeF(((float)CCD_MINWIDTH / (float)CCD4_WIDTH), ((float)CCD_MINHEIGHT / (float)CCD4_HEIGHT));

            //[Basic Control]
            MACHINENAME = ReadINIValue("Basic Control", INIEnum.MACHINENAME.ToString(), MACHINENAME, INIFILE);
            LANGUAGE = int.Parse(ReadINIValue("Basic Control", INIEnum.LANGUAGE.ToString(), LANGUAGE.ToString(), INIFILE));

            DELAYTIME = int.Parse(ReadINIValue("Basic Control", INIEnum.DELAYTIME.ToString(), DELAYTIME.ToString(), INIFILE));
            ISNOLIVE = int.Parse(ReadINIValue("Basic Control", INIEnum.ISNOLIVE.ToString(), "0", INIFILE)) == 1;

            SHOPFLOORPATH = ReadINIValue("Basic Control", INIEnum.SHOPFLOORPATH.ToString(), SHOPFLOORPATH, INIFILE);
            PRELOADINDEX = ReadINIValue("Basic Control", INIEnum.PRELOADINDEX.ToString(), PRELOADINDEX, INIFILE);
            VIEWLOGCOUNT = int.Parse(ReadINIValue("Basic Control", INIEnum.VIEWLOGCOUNT.ToString(), VIEWLOGCOUNT.ToString(), INIFILE));

            ISCALIBLACK = ReadINIValue("Basic Control", INIEnum.ISCALIBLACK.ToString(), (ISCALIBLACK ? "1" : "0"), INIFILE) == "1";
            PICKPOSITION = float.Parse(ReadINIValue("Basic Control", INIEnum.PICKPOSITION.ToString(), PICKPOSITION.ToString(), INIFILE));

            WBSHIFT = int.Parse(ReadINIValue("Basic Control", INIEnum.WBSHIFT.ToString(), WBSHIFT.ToString(), INIFILE));

            UNBALANCEVAL = float.Parse(ReadINIValue("Basic Control", INIEnum.UNBALANCEVAL.ToString(), UNBALANCEVAL.ToString(), INIFILE));
            //ISUSEWB = ReadINIValue("Basic Control", INIEnum.ISUSEWB.ToString(), (ISUSEWB ? "1" : "0"), INIFILE) == "1";

            AREATHRESHOLD = int.Parse(ReadINIValue("Basic Control", INIEnum.AREATHRESHOLD.ToString(), AREATHRESHOLD.ToString(), INIFILE));

            ISUSEJPMETHOD = ReadINIValue("Basic Control", INIEnum.ISUSEJPMETHOD.ToString(), (ISUSEJPMETHOD ? "1" : "0"), INIFILE) == "1";
            JPMETHODRANGE = float.Parse(ReadINIValue("Basic Control", INIEnum.JPMETHODRANGE.ToString(), JPMETHODRANGE.ToString(), INIFILE));

            ISUSEDEFINESTARTFOCUS = ReadINIValue("Basic Control", INIEnum.ISUSEDEFINESTARTFOCUS.ToString(), (ISUSEDEFINESTARTFOCUS ? "1" : "0"), INIFILE) == "1";

            TRMIN = float.Parse(ReadINIValue("Basic Control", INIEnum.TRMIN.ToString(), TRMIN.ToString(), INIFILE));
            TRMAX = float.Parse(ReadINIValue("Basic Control", INIEnum.TRMAX.ToString(), TRMAX.ToString(), INIFILE));

            ORG_ID = ReadINIValue("Basic Control", INIEnum.ORG_ID.ToString(), ORG_ID, INIFILE);
            ONDELAYTIME = int.Parse(ReadINIValue("Basic Control", INIEnum.ONDELAYTIME.ToString(), ONDELAYTIME.ToString(), INIFILE));


            JUMBO_SERVER_IP = ReadINIValue("Jumbo Control", INIEnum.JUMBO_SERVER_IP.ToString(), JUMBO_SERVER_IP.ToString(), INIFILE);
            JUMBO_SERVER_PORT = int.Parse(ReadINIValue("Jumbo Control", INIEnum.JUMBO_SERVER_PORT.ToString(), JUMBO_SERVER_PORT.ToString(), INIFILE));

            //[Allinone Control]
            Allinone_Pt_Start = JzTools.StringToPoint(ReadINIValue("Allinone Control", INIEnum.Allinone_Pt_Start.ToString(), JzTools.PointToString(Allinone_Pt_Start), INIFILE));
            Allinone_Pt_End = JzTools.StringToPoint(ReadINIValue("Allinone Control", INIEnum.Allinone_Pt_End.ToString(), JzTools.PointToString(Allinone_Pt_End), INIFILE));
            Allinone_Offset = int.Parse(ReadINIValue("Allinone Control", INIEnum.Allinone_Offset.ToString(), Allinone_Offset.ToString(), INIFILE));
            Allinone_Offset_ADDX = int.Parse(ReadINIValue("Allinone Control", INIEnum.Allinone_Offset_ADDX.ToString(), Allinone_Offset_ADDX.ToString(), INIFILE));
            Allinone_Offset_ADDY = int.Parse(ReadINIValue("Allinone Control", INIEnum.Allinone_Offset_ADDY.ToString(), Allinone_Offset_ADDY.ToString(), INIFILE));
            Allinone_IsNoUseMotion = ReadINIValue("Allinone Control", INIEnum.Allinone_IsNoUseMotion.ToString(), (Allinone_IsNoUseMotion ? "1" : "0"), INIFILE) == "1";

            Allinone_Calibrate_Cam_Index = int.Parse(ReadINIValue("Allinone Control", INIEnum.Allinone_Calibrate_Cam_Index.ToString(), Allinone_Calibrate_Cam_Index.ToString(), INIFILE));
            Allinone_Cam_Ratio = float.Parse(ReadINIValue("Allinone Control", INIEnum.Allinone_Cam_Ratio.ToString(), Allinone_Cam_Ratio.ToString(), INIFILE));


            LoadCCDLocation();
        }

        public static void LoadCCDLocation()
        {
            string Str = "";
            int i = 0;

            if (!File.Exists(LOCATIONFILE))
            {
                CreateCCDLocation();
            }
            else
            {
                JzTools.ReadData(ref Str, LOCATIONFILE);

                string[] strs = Str.Replace(Environment.NewLine, "@").Split('@');

                CCDLOCATIONS = new Point[strs.Length];
                TmpCCDLOCATIONS = new Point[strs.Length];

                i = 0;
                foreach (string str in strs)
                {
                    CCDLOCATIONS[i] = JzTools.StringToPoint(strs[i]);
                    TmpCCDLOCATIONS[i] = JzTools.StringToPoint(strs[i]);
                    i++;
                }

                CheckCCDLocation();
            }

            CCDVIEWSIZE = CreateCCDViewSize();
        }

        public static void Save()
        {
            //Write [Basic Control] Parameters

            //WriteINIValue("Basic Control", INIEnum.MACHINENAME.ToString(), MACHINENAME, INIFILE);
            //WriteINIValue("Basic Control", INIEnum.DELAYTIME.ToString(), DELAYTIME.ToString(),INIFILE);
            //WriteINIValue("Basic Control", INIEnum.LANGUAGE.ToString(), LANGUAGE.ToString(), INIFILE);
            //WriteINIValue("Basic Control", INIEnum.SHOPFLOORPATH.ToString(), SHOPFLOORPATH, INIFILE);
            //WriteINIValue("Basic Control", INIEnum.PICKPOSITION.ToString(), PICKPOSITION.ToString(), INIFILE);
            //WriteINIValue("Basic Control", INIEnum.WBSHIFT.ToString(), WBSHIFT.ToString(), INIFILE);
            //WriteINIValue("Basic Control", INIEnum.UNBALANCEVAL.ToString(), UNBALANCEVAL.ToString(), INIFILE);
            //WriteINIValue("Basic Control", INIEnum.AREATHRESHOLD.ToString(), AREATHRESHOLD.ToString(), INIFILE);
            //WriteINIValue("Basic Control", INIEnum.ISUSEJPMETHOD.ToString(), (ISUSEJPMETHOD ? "1" : "0"), INIFILE);
            //WriteINIValue("Basic Control", INIEnum.JPMETHODRANGE.ToString(), JPMETHODRANGE.ToString(), INIFILE);

            //WriteINIValue("Basic Control", INIEnum.TRMIN.ToString(), TRMIN.ToString(), INIFILE);
            //WriteINIValue("Basic Control", INIEnum.TRMAX.ToString(), TRMAX.ToString(), INIFILE);


            //WriteINIValue("Basic Control", INIEnum.ORG_ID.ToString(), ORG_ID, INIFILE);
            //WriteINIValue("Basic Control", INIEnum.ONDELAYTIME.ToString(), ONDELAYTIME.ToString(), INIFILE);

            WriteINIValue("Allinone Control", INIEnum.Allinone_Calibrate_Cam_Index.ToString(), Allinone_Calibrate_Cam_Index.ToString(), INIFILE);
            WriteINIValue("Allinone Control", INIEnum.Allinone_Cam_Ratio.ToString(), Allinone_Cam_Ratio.ToString("0.00"), INIFILE);
        }

        public static void SaveCCDLocation()
        {
            int i = 0;
            string Str = "";

            while (i < CCDLOCATIONS.Length)
            {
                Str += JzTools.PointtoString(CCDLOCATIONS[i]) + Environment.NewLine;
                i++;
            }

            Str = JzTools.RemoveLastChar(Str, 2);
            JzTools.SaveData(Str, LOCATIONFILE);
        }
        static void CreateCCDLocation()
        {
            int i = 0;
            string Str = "";

            CCDLOCATIONS = new Point[CCD_SHOWCOUNT];
            TmpCCDLOCATIONS = new Point[CCD_SHOWCOUNT];

            while (i < CCDLOCATIONS.Length)
            {
                CCDLOCATIONS[i].X = (CCD_WIDTH * (i % 3));
                CCDLOCATIONS[i].Y = (CCD_HEIGHT * (i / 3));

                TmpCCDLOCATIONS[i].X = (CCD_WIDTH * (i % 3));
                TmpCCDLOCATIONS[i].Y = (CCD_HEIGHT * (i / 3));

                Str += JzTools.PointtoString(CCDLOCATIONS[i]) + Environment.NewLine;

                i++;
            }

            Str = JzTools.RemoveLastChar(Str, 2);

            JzTools.SaveData(Str, LOCATIONFILE);
        }
        public static void CheckCCDLocation()
        {
            if (TmpCCDLOCATIONS.Length != CCD_SHOWCOUNT)
            {
                CreateCCDLocation();
            }
        }
        public static void WriteBackCCDLocation()
        {
            int i = 0;

            foreach (Point pt in TmpCCDLOCATIONS)
            {
                CCDLOCATIONS[i] = pt;
                i++;
            }
        }

        public static RectangleF TransferCCDRatio(RectangleF srcrectf, int ccdIndex)
        {
            RectangleF rectf = srcrectf;

            switch (CCDSEQSTRING[ccdIndex])
            {
                case '1':
                    rectf.X = rectf.X * INI.CCD_RATIO.Width;
                    rectf.Y = rectf.Y * INI.CCD_RATIO.Height;
                    rectf.Width = rectf.Width * INI.CCD_RATIO.Width;
                    rectf.Height = rectf.Height * INI.CCD_RATIO.Height;
                    break;
                case '2':
                    rectf.X = INI.CCDLOCATIONS[1].X + (rectf.X - INI.CCDLOCATIONS[1].X) * INI.CCD2_RATIO.Width;
                    rectf.Y = INI.CCDLOCATIONS[1].Y + (rectf.Y - INI.CCDLOCATIONS[1].Y) * INI.CCD2_RATIO.Height;
                    rectf.Width = rectf.Width * INI.CCD2_RATIO.Width;
                    rectf.Height = rectf.Height * INI.CCD2_RATIO.Height;
                    break;
                case '3':
                    rectf.X = INI.CCDLOCATIONS[2].X + (rectf.X - INI.CCDLOCATIONS[2].X) * INI.CCD3_RATIO.Width;
                    rectf.Y = INI.CCDLOCATIONS[2].Y + (rectf.Y - INI.CCDLOCATIONS[2].Y) * INI.CCD3_RATIO.Height;
                    rectf.Width = rectf.Width * INI.CCD3_RATIO.Width;
                    rectf.Height = rectf.Height * INI.CCD3_RATIO.Height;
                    break;
                case '4':
                    rectf.X = INI.CCDLOCATIONS[3].X + (rectf.X - INI.CCDLOCATIONS[3].X) * INI.CCD4_RATIO.Width;
                    rectf.Y = INI.CCDLOCATIONS[3].Y + (rectf.Y - INI.CCDLOCATIONS[3].Y) * INI.CCD4_RATIO.Height;
                    rectf.Width = rectf.Width * INI.CCD4_RATIO.Width;
                    rectf.Height = rectf.Height * INI.CCD4_RATIO.Height;
                    break;
            }

            return rectf;
        }

        public static void AdjustLocation(string screenmovestr, Point PT)
        {
            string[] strs = screenmovestr.Split(',');

            int i = 1;

            foreach (string str in strs)
            {
                TmpCCDLOCATIONS[i] = CCDLOCATIONS[i];

                if (str == "1")
                {
                    TmpCCDLOCATIONS[i].Offset(PT);
                }

                i++;
            }
        }
        public static void AdjustLocation(string screenmovestr)
        {
            string[] strs = screenmovestr.Split(',');

            int i = 1;

            foreach (string str in strs)
            {
                TmpCCDLOCATIONS[i] = CCDLOCATIONS[i];

                if (str == "1")
                {
                    TmpCCDLOCATIONS[i].Y = 0;
                }

                i++;
            }
        }

        public static Size CreateCCDViewSize()
        {
            Rectangle rect = new Rectangle(0, 0, 1, 1);

            int i = 0;

            foreach (Point location in TmpCCDLOCATIONS)
            {
                if (i < CCD_SHOWCOUNT)
                {
                    rect = JzTools.MergeTwoRects(rect, new Rectangle(new Point(location.X, location.Y),
                                    new Size(INI.CCD_MINWIDTH, INI.CCD_MINHEIGHT)));
                }
                i++;
            }
            return rect.Size;
        }

    }
}
