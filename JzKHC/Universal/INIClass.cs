using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using JetEazy.BasicSpace;
using System.Runtime.InteropServices;

namespace JzKHC
{
    enum INIVariableEnum : int          
    {
        COUNT = 78,

        SUPERUSER = 0,
        CCDWITDTH = 1,  
        CCDHEIGHT = 2,
        SIDE1LOCATION = 3,
        SIDE2LOCATION = 4,
        SIDE3LOCATION = 5,
        LAST_RECIPEID = 6,

        PASS = 7,
        NG = 8,

        SHOW_SPEC_ERROR = 9,
        SHOW_SLOP_ERROR = 10,
        SHOW_HL_ERROR = 11,
        GOODCHECK = 12,
        DELAYTIME = 13,
        TOLERANCE = 14,
        SLOPTOLERANCE = 15,
        MACHINENAME = 16,

        SIDE4LOCATION = 17,
        SIDE5LOCATION = 18,
        SIDE6LOCATION = 19,

        SIDECOUNT = 20,
        TWOSTAGE = 21,
        HAVEKEYBASE = 22,
        PORT = 23,
        DIFFHEIGHT = 24,
        BIASERROR = 25,
        BASEHEIGHT = 26,
        STABLETIME = 27,
        CUTPOINT = 28,
        LANGUAGE = 29,
        ISUSEARROUND = 30,
        ISUSEDELTA = 31,
        ISDARFONREPORT = 32,
        ISPOPUPMSG = 33,
        FACTOR = 34,
        ISFACTORENABLE = 35,
        RESULTPATH = 36,
        SFRETESTTIME = 37,
        ISDARFONRETEST = 38,
        ISNMBLBADDED = 39,
        ISUSEPLANE=40,
        SPACEDIFF=41,
        ISSPACEFLAT=42,
        COMPENSATION=43,

        ISADJUST = 44,
        ISCHECKINGDUP = 45,

        DUPRATIO = 46,
        DUPCOUNT = 47,

        ISAISYS = 48,
        FINDCONTRAST = 49,

        LSHIFTDIFF =50,
        RSHIFTDIFF =51,

        ISABSSHOWRESULT =52,

        TICK = 53,
        ISUSEDUP = 54,

        FAILCOUNT = 55,
        TESTHEIGHT = 56,

        BASEZLOCATION =57,
        READYZLOCATION = 58,
        MAGZLOCATION = 59,
        TESTLOCATION = 60,

        BASEULOCATION = 61,
        READYULOCATION = 62,

        BASEYLOCATION = 63,
        READYYLOCATION = 64,

        SLOWULOCATION = 65,

        MACHINETYPE =66,

        ISMAINPROGRAM =67,

        ISROTATE = 68,

        ISSUNREXSF = 69,

        ISLIMITBARCODE=70,
        BARCODELENGTH=71,

        SIDE7LOCATION=72,
        SIDE8LOCATION=73,
        SIDE9LOCATION=74,

        SIDE10LOCATION=75,

        ISCHECKALARM = 76,
        ISMODE_NO_USE_PASSWAY=77,
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

        static string INIFILE = "";

        //public static string[] INIVariables = new string[(int)INIVariableEnum.COUNT];

        public static string SUPERUSER = "1234509876";
        public static int LAST_RECIPEID = 1;

        public static int CCDWIDTH = 2592;
        public static int CCDHEIGHT = 1944;

        public static Point SIDE1LOCATION = new Point(CCDWIDTH - 10, 0);
        public static Point SIDE2LOCATION = new Point((CCDWIDTH << 1) - 20, 0);
        public static Point SIDE3LOCATION = new Point((CCDWIDTH * 3) - 20, 0);
        
        public static Point SIDE4LOCATION = new Point((CCDWIDTH * 4) - 20, 0);
        public static Point SIDE5LOCATION = new Point((CCDWIDTH * 5) - 20, 0);
        public static Point SIDE6LOCATION = new Point((CCDWIDTH * 6) - 20, 0);

        public static Point SIDE7LOCATION = new Point((CCDWIDTH * 7) - 20, 0);
        public static Point SIDE8LOCATION = new Point((CCDWIDTH * 8) - 20, 0);
        public static Point SIDE9LOCATION = new Point((CCDWIDTH * 9) - 20, 0);
        public static Point SIDE10LOCATION = new Point((CCDWIDTH * 10) - 20, 0);
        public static Point SIDE1LOCATIONLIVE = new Point(CCDWIDTH - 10, 0);
        public static Point SIDE2LOCATIONLIVE = new Point((CCDWIDTH << 1) - 20, 0);
        public static Point SIDE3LOCATIONLIVE = new Point((CCDWIDTH * 3) - 20, 0);

        public static Point SIDE4LOCATIONLIVE = new Point((CCDWIDTH * 4) - 20, 0);
        public static Point SIDE5LOCATIONLIVE = new Point((CCDWIDTH * 5) - 20, 0);
        public static Point SIDE6LOCATIONLIVE = new Point((CCDWIDTH * 6) - 20, 0);
        public static Point SIDE7LOCATIONLIVE = new Point((CCDWIDTH * 7) - 20, 0);
        public static Point SIDE8LOCATIONLIVE = new Point((CCDWIDTH * 8) - 20, 0);
        public static Point SIDE9LOCATIONLIVE = new Point((CCDWIDTH * 9) - 20, 0);
        public static Point SIDE10LOCATIONLIVE = new Point((CCDWIDTH * 10) - 20, 0);

        public static int PASS = 0;
        public static int NG = 0;

        public static bool SHOW_SPEC_ERROR = true;
        public static bool SHOW_SLOP_ERROR = true;
        public static bool SHOW_HL_ERROR = true;
        public static bool GOODCHECK = false;
        public static int DELAYTIME = 2000;
        public static double TOLERANCE = 0.03d;
        public static double SLOPTOLERANCE = 0.1d;
        public static string MACHINENAME = "JUMBO301";

        public static int SIDECOUNT = 4;
        public static bool TWOSTAGE = true;
        public static bool HAVEKEYBASE = false;

        public static string PORT = "COM1";
        public static double DIFFHEIGHT = 2.00d;
        public static int BIASERROR = 3;

        public static double BASEHEIGHT = 0.00d;
        public static int STABLETIME = 20;
        public static int CUTPOINT = 0;

        public static int LANGUAGE = 0;
        public static bool ISUSEARROUND = false;
        public static bool ISUSEDELTA = false;
        public static bool ISDARFONREPORT = false;
        public static bool ISPOPUPMSG = false;
        public static bool ISFACTORENABLE = false;
        public static double FACTOR = 1;
        public static string RESULTPATH = "D:\\AOI";
        public static bool ISSFCUSES1FORMAT = true;
        public static int SFRETESTTIME = 1;
        public static bool ISDARFONRETEST = false;
        public static double ISNMBLBADDED = 0;
        public static bool ISUSEPLANE = false;
        public static bool ISONLINEUSE5PTPLANE = false;
        public static double SPACEDIFF = 0.25;
        public static bool ISSPACEFLAT = false;
        public static double COMPENSATION = 0;

        public static bool ISADJUST = false;
        public static bool ISCHECKINGDUP = false;

        public static double DUPRATIO = 0.8;
        public static int DUPCOUNT = 10;

        public static bool ISAISYS = false;
        public static int FINDCONTRAST = 3;

        public static double LSHIFTDIFF = 0.25;
        public static double RSHIFTDIFF = 0.25;

        public static bool ISABSSHOWRESULT = false;
        public static int TICK = 300;
        public static bool ISUSEDUP = false;

        public static double FAILCOUNT = 0;
        public static double TESTHEIGHT = 0;

        public static double BASEZLOCATION = 0;
        public static double READYZLOCATION = 0;
        public static double MAGZLOCATION = 0;
        public static double TESTZLOCATION = 0;

        public static double BASEULOCATION = 0;
        public static double READYULOCATION = 0;
        public static double SLOWULOCATION = 2;

        public static double BASEYLOCATION = 0;
        public static double READYYLOCATION = 0;

        public static int MACHINETYPE = 3;
        public static bool ISMAINPROGRAM = true;

        public static bool ISROTATE = false;
        public static bool ISSUNREXSF = false;

        public static bool ISLIMITBARCODE = false;
        public static int BARCODELENGTH = 17;

        public static bool ISCHECKALARM = true;
        public static bool ISMODE_NO_USE_PASSWAY = false;

        /// <summary>
        /// 橫向抓取為true 反之為 false
        /// </summary>
        public static bool IS_CHECK_LEVEL = true;//是否啟用橫向測試模式，五合一中使用

        public static void Initial()
        {
            INIFILE = Universal.KHCCollectionPath + @"\CONFIG.ini";
            Load();
        }
        public static void Load()
        {
            SUPERUSER = ReadINIValue("Environment", INIVariableEnum.SUPERUSER.ToString(), SUPERUSER, INIFILE);
            LAST_RECIPEID = int.Parse(ReadINIValue("Environment", INIVariableEnum.LAST_RECIPEID.ToString(), LAST_RECIPEID.ToString(), INIFILE));
            CCDWIDTH = int.Parse(ReadINIValue("Environment", INIVariableEnum.CCDWITDTH.ToString(), CCDWIDTH.ToString(), INIFILE));
            CCDHEIGHT = int.Parse(ReadINIValue("Environment", INIVariableEnum.CCDHEIGHT.ToString(), CCDHEIGHT.ToString(), INIFILE));

            SIDE1LOCATION = StringtoPoint(ReadINIValue("Environment", INIVariableEnum.SIDE1LOCATION.ToString(), SIDE1LOCATION.ToString(), INIFILE));
            SIDE2LOCATION = StringtoPoint(ReadINIValue("Environment", INIVariableEnum.SIDE2LOCATION.ToString(), SIDE2LOCATION.ToString(), INIFILE));
            SIDE3LOCATION = StringtoPoint(ReadINIValue("Environment", INIVariableEnum.SIDE3LOCATION.ToString(), SIDE3LOCATION.ToString(), INIFILE));
            SIDE4LOCATION = StringtoPoint(ReadINIValue("Environment", INIVariableEnum.SIDE4LOCATION.ToString(), SIDE4LOCATION.ToString(), INIFILE));
            SIDE5LOCATION = StringtoPoint(ReadINIValue("Environment", INIVariableEnum.SIDE5LOCATION.ToString(), SIDE5LOCATION.ToString(), INIFILE));
            SIDE6LOCATION = StringtoPoint(ReadINIValue("Environment", INIVariableEnum.SIDE6LOCATION.ToString(), SIDE6LOCATION.ToString(), INIFILE));

            SIDE1LOCATIONLIVE = StringtoPoint(ReadINIValue("Environment", INIVariableEnum.SIDE1LOCATION.ToString(), SIDE1LOCATION.ToString(), INIFILE));
            SIDE2LOCATIONLIVE = StringtoPoint(ReadINIValue("Environment", INIVariableEnum.SIDE2LOCATION.ToString(), SIDE2LOCATION.ToString(), INIFILE));
            SIDE3LOCATIONLIVE = StringtoPoint(ReadINIValue("Environment", INIVariableEnum.SIDE3LOCATION.ToString(), SIDE3LOCATION.ToString(), INIFILE));
            SIDE4LOCATIONLIVE = StringtoPoint(ReadINIValue("Environment", INIVariableEnum.SIDE4LOCATION.ToString(), SIDE4LOCATION.ToString(), INIFILE));
            SIDE5LOCATIONLIVE = StringtoPoint(ReadINIValue("Environment", INIVariableEnum.SIDE5LOCATION.ToString(), SIDE5LOCATION.ToString(), INIFILE));
            SIDE6LOCATIONLIVE = StringtoPoint(ReadINIValue("Environment", INIVariableEnum.SIDE6LOCATION.ToString(), SIDE6LOCATION.ToString(), INIFILE));

            PASS = int.Parse(ReadINIValue("Environment", INIVariableEnum.PASS.ToString(), PASS.ToString(), INIFILE));
            NG = int.Parse(ReadINIValue("Environment", INIVariableEnum.NG.ToString(), NG.ToString(), INIFILE));

            SHOW_SPEC_ERROR = ReadINIValue("Environment", INIVariableEnum.SHOW_SPEC_ERROR.ToString(), SHOW_SPEC_ERROR.ToString(), INIFILE) == "1";
            SHOW_SLOP_ERROR = ReadINIValue("Environment", INIVariableEnum.SHOW_SLOP_ERROR.ToString(), SHOW_SLOP_ERROR.ToString(), INIFILE) == "1";
            SHOW_HL_ERROR = ReadINIValue("Environment", INIVariableEnum.SHOW_HL_ERROR.ToString(), SHOW_HL_ERROR.ToString(), INIFILE) == "1";
            GOODCHECK = ReadINIValue("Environment", INIVariableEnum.GOODCHECK.ToString(), GOODCHECK.ToString(), INIFILE) == "1";
            DELAYTIME = int.Parse(ReadINIValue("Environment", INIVariableEnum.DELAYTIME.ToString(), DELAYTIME.ToString(), INIFILE));

            if (DELAYTIME < 500)
                DELAYTIME = 500;

            TOLERANCE = double.Parse(ReadINIValue("Environment", INIVariableEnum.TOLERANCE.ToString(), TOLERANCE.ToString(), INIFILE));
            SLOPTOLERANCE = double.Parse(ReadINIValue("Environment", INIVariableEnum.SLOPTOLERANCE.ToString(), SLOPTOLERANCE.ToString(), INIFILE));
            MACHINENAME = ReadINIValue("Environment", INIVariableEnum.MACHINENAME.ToString(), MACHINENAME.ToString(), INIFILE);
            SIDECOUNT = int.Parse(ReadINIValue("Environment", INIVariableEnum.SIDECOUNT.ToString(), SIDECOUNT.ToString(), INIFILE));
            TWOSTAGE = ReadINIValue("Environment", INIVariableEnum.TWOSTAGE.ToString(), TWOSTAGE.ToString(), INIFILE) == "1";
            PORT = ReadINIValue("Environment", INIVariableEnum.PORT.ToString(), PORT.ToString(), INIFILE);
            RESULTPATH = ReadINIValue("Environment", INIVariableEnum.RESULTPATH.ToString(), RESULTPATH.ToString(), INIFILE);
            DIFFHEIGHT = int.Parse(ReadINIValue("Environment", INIVariableEnum.DIFFHEIGHT.ToString(), DIFFHEIGHT.ToString(), INIFILE));
            BIASERROR = int.Parse(ReadINIValue("Environment", INIVariableEnum.BIASERROR.ToString(), BIASERROR.ToString(), INIFILE));
            BASEHEIGHT = double.Parse(ReadINIValue("Environment", INIVariableEnum.BASEHEIGHT.ToString(), BASEHEIGHT.ToString(), INIFILE));
            STABLETIME = int.Parse(ReadINIValue("Environment", INIVariableEnum.STABLETIME.ToString(), STABLETIME.ToString(), INIFILE));
            CUTPOINT = int.Parse(ReadINIValue("Environment", INIVariableEnum.CUTPOINT.ToString(), CUTPOINT.ToString(), INIFILE));
            LANGUAGE = int.Parse(ReadINIValue("Environment", INIVariableEnum.LANGUAGE.ToString(), LANGUAGE.ToString(), INIFILE));
            FACTOR = double.Parse(ReadINIValue("Environment", INIVariableEnum.FACTOR.ToString(), FACTOR.ToString(), INIFILE));
            SFRETESTTIME = int.Parse(ReadINIValue("Environment", INIVariableEnum.SFRETESTTIME.ToString(), SFRETESTTIME.ToString(), INIFILE));
            ISNMBLBADDED = double.Parse(ReadINIValue("Environment", INIVariableEnum.ISNMBLBADDED.ToString(), ISNMBLBADDED.ToString(), INIFILE));
            SPACEDIFF = double.Parse(ReadINIValue("Environment", INIVariableEnum.SPACEDIFF.ToString(), SPACEDIFF.ToString(), INIFILE));
            COMPENSATION = double.Parse(ReadINIValue("Environment", INIVariableEnum.COMPENSATION.ToString(), COMPENSATION.ToString(), INIFILE));
            DUPRATIO = double.Parse(ReadINIValue("Environment", INIVariableEnum.DUPRATIO.ToString(), DUPRATIO.ToString(), INIFILE));
            DUPCOUNT = int.Parse(ReadINIValue("Environment", INIVariableEnum.DUPCOUNT.ToString(), DUPCOUNT.ToString(), INIFILE));
            FINDCONTRAST = int.Parse(ReadINIValue("Environment", INIVariableEnum.FINDCONTRAST.ToString(), FINDCONTRAST.ToString(), INIFILE));
            LSHIFTDIFF = double.Parse(ReadINIValue("Environment", INIVariableEnum.LSHIFTDIFF.ToString(), LSHIFTDIFF.ToString(), INIFILE));
            RSHIFTDIFF = double.Parse(ReadINIValue("Environment", INIVariableEnum.RSHIFTDIFF.ToString(), RSHIFTDIFF.ToString(), INIFILE));
            TICK = int.Parse(ReadINIValue("Environment", INIVariableEnum.TICK.ToString(), TICK.ToString(), INIFILE));
            FAILCOUNT = double.Parse(ReadINIValue("Environment", INIVariableEnum.FAILCOUNT.ToString(), FAILCOUNT.ToString(), INIFILE));
            HAVEKEYBASE = ReadINIValue("Environment", INIVariableEnum.HAVEKEYBASE.ToString(), HAVEKEYBASE.ToString(), INIFILE) == "1";
            ISUSEARROUND = ReadINIValue("Environment", INIVariableEnum.ISUSEARROUND.ToString(), ISUSEARROUND.ToString(), INIFILE) == "1";
            ISUSEDELTA = ReadINIValue("Environment", INIVariableEnum.ISUSEDELTA.ToString(), ISUSEDELTA.ToString(), INIFILE) == "1";
            ISDARFONREPORT = ReadINIValue("Environment", INIVariableEnum.ISDARFONREPORT.ToString(), ISDARFONREPORT.ToString(), INIFILE) == "1";
            ISPOPUPMSG = ReadINIValue("Environment", INIVariableEnum.ISPOPUPMSG.ToString(), ISPOPUPMSG.ToString(), INIFILE) == "1";
            ISFACTORENABLE = ReadINIValue("Environment", INIVariableEnum.ISFACTORENABLE.ToString(), ISFACTORENABLE.ToString(), INIFILE) == "1";
            ISDARFONRETEST = ReadINIValue("Environment", INIVariableEnum.ISDARFONRETEST.ToString(), ISDARFONRETEST.ToString(), INIFILE) == "1";
            ISUSEPLANE = ReadINIValue("Environment", INIVariableEnum.ISUSEPLANE.ToString(), ISUSEPLANE.ToString(), INIFILE) == "1";
            ISSPACEFLAT = ReadINIValue("Environment", INIVariableEnum.ISSPACEFLAT.ToString(), ISSPACEFLAT.ToString(), INIFILE) == "1";
            ISADJUST = ReadINIValue("Environment", INIVariableEnum.ISADJUST.ToString(), ISADJUST.ToString(), INIFILE) == "1";
            ISCHECKINGDUP = ReadINIValue("Environment", INIVariableEnum.ISCHECKINGDUP.ToString(), ISCHECKINGDUP.ToString(), INIFILE) == "1";
            ISAISYS = ReadINIValue("Environment", INIVariableEnum.ISAISYS.ToString(), ISAISYS.ToString(), INIFILE) == "1";
            ISABSSHOWRESULT = ReadINIValue("Environment", INIVariableEnum.ISABSSHOWRESULT.ToString(), ISABSSHOWRESULT.ToString(), INIFILE) == "1";
            ISUSEDUP = ReadINIValue("Environment", INIVariableEnum.ISUSEDUP.ToString(), ISUSEDUP.ToString(), INIFILE) == "1";
            TESTHEIGHT = double.Parse(ReadINIValue("Environment", INIVariableEnum.TESTHEIGHT.ToString(), TESTHEIGHT.ToString(), INIFILE));
            BASEZLOCATION = double.Parse(ReadINIValue("Environment", INIVariableEnum.BASEZLOCATION.ToString(), BASEZLOCATION.ToString(), INIFILE));
            READYZLOCATION = double.Parse(ReadINIValue("Environment", INIVariableEnum.READYZLOCATION.ToString(), READYZLOCATION.ToString(), INIFILE));
            MAGZLOCATION = double.Parse(ReadINIValue("Environment", INIVariableEnum.MAGZLOCATION.ToString(), MAGZLOCATION.ToString(), INIFILE));
            TESTZLOCATION = double.Parse(ReadINIValue("Environment", INIVariableEnum.TESTLOCATION.ToString(), TESTZLOCATION.ToString(), INIFILE));
            BASEULOCATION = double.Parse(ReadINIValue("Environment", INIVariableEnum.BASEULOCATION.ToString(), BASEULOCATION.ToString(), INIFILE));
            READYULOCATION = double.Parse(ReadINIValue("Environment", INIVariableEnum.READYULOCATION.ToString(), READYULOCATION.ToString(), INIFILE));
            SLOWULOCATION = double.Parse(ReadINIValue("Environment", INIVariableEnum.SLOWULOCATION.ToString(), SLOWULOCATION.ToString(), INIFILE));
            BASEYLOCATION = double.Parse(ReadINIValue("Environment", INIVariableEnum.BASEYLOCATION.ToString(), BASEYLOCATION.ToString(), INIFILE));
            READYYLOCATION = double.Parse(ReadINIValue("Environment", INIVariableEnum.READYYLOCATION.ToString(), READYYLOCATION.ToString(), INIFILE));
            MACHINETYPE = int.Parse(ReadINIValue("Environment", INIVariableEnum.MACHINETYPE.ToString(), MACHINETYPE.ToString(), INIFILE));
            BARCODELENGTH = int.Parse(ReadINIValue("Environment", INIVariableEnum.BARCODELENGTH.ToString(), BARCODELENGTH.ToString(), INIFILE));
            ISMAINPROGRAM = ReadINIValue("Environment", INIVariableEnum.ISMAINPROGRAM.ToString(), ISMAINPROGRAM.ToString(), INIFILE) == "1";
            ISROTATE = ReadINIValue("Environment", INIVariableEnum.ISROTATE.ToString(), ISROTATE.ToString(), INIFILE) == "1";
            ISSUNREXSF = ReadINIValue("Environment", INIVariableEnum.ISSUNREXSF.ToString(), ISSUNREXSF.ToString(), INIFILE) == "1";
            ISLIMITBARCODE = ReadINIValue("Environment", INIVariableEnum.ISLIMITBARCODE.ToString(), ISLIMITBARCODE.ToString(), INIFILE) == "1";
            ISCHECKALARM = ReadINIValue("Environment", INIVariableEnum.ISCHECKALARM.ToString(), ISCHECKALARM.ToString(), INIFILE) == "1";
            ISMODE_NO_USE_PASSWAY = ReadINIValue("Environment", INIVariableEnum.ISMODE_NO_USE_PASSWAY.ToString(), ISMODE_NO_USE_PASSWAY.ToString(), INIFILE) == "1";
        }
        public static void Save()
        {
            WriteINIValue("Environment", INIVariableEnum.SUPERUSER.ToString(), SUPERUSER, INIFILE);
            WriteINIValue("Environment", INIVariableEnum.LAST_RECIPEID.ToString(), LAST_RECIPEID.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.CCDWITDTH.ToString(), CCDWIDTH.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.CCDHEIGHT.ToString(), CCDHEIGHT.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.SIDE1LOCATION.ToString(), PointtoString(SIDE1LOCATION).Replace(" ", ""), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.SIDE2LOCATION.ToString(), PointtoString(SIDE2LOCATION).Replace(" ", ""), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.SIDE3LOCATION.ToString(), PointtoString(SIDE3LOCATION).Replace(" ", ""), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.SIDE4LOCATION.ToString(), PointtoString(SIDE4LOCATION).Replace(" ", ""), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.SIDE5LOCATION.ToString(), PointtoString(SIDE5LOCATION).Replace(" ", ""), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.SIDE6LOCATION.ToString(), PointtoString(SIDE6LOCATION).Replace(" ", ""), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.SIDE7LOCATION.ToString(), PointtoString(SIDE7LOCATION).Replace(" ", ""), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.SIDE8LOCATION.ToString(), PointtoString(SIDE8LOCATION).Replace(" ", ""), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.SIDE9LOCATION.ToString(), PointtoString(SIDE9LOCATION).Replace(" ", ""), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.SIDE10LOCATION.ToString(), PointtoString(SIDE10LOCATION).Replace(" ", ""), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.PASS.ToString(), PASS.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.NG.ToString(), NG.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.DELAYTIME.ToString(), DELAYTIME.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.TOLERANCE.ToString(), TOLERANCE.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.SLOPTOLERANCE.ToString(), SLOPTOLERANCE.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.MACHINENAME.ToString(), DELAYTIME.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.SIDECOUNT.ToString(), SIDECOUNT.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.PORT.ToString(), PORT.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.DIFFHEIGHT.ToString(), DIFFHEIGHT.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.BIASERROR.ToString(), BIASERROR.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.BASEHEIGHT.ToString(), BASEHEIGHT.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.STABLETIME.ToString(), STABLETIME.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.CUTPOINT.ToString(), CUTPOINT.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.LANGUAGE.ToString(), LANGUAGE.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.RESULTPATH.ToString(), RESULTPATH.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.SFRETESTTIME.ToString(), SFRETESTTIME.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.SPACEDIFF.ToString(), SPACEDIFF.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.COMPENSATION.ToString(), COMPENSATION.ToString(), INIFILE);

            WriteINIValue("Environment", INIVariableEnum.SHOW_SPEC_ERROR.ToString(), SHOW_SPEC_ERROR ? "1" : "0", INIFILE);
            WriteINIValue("Environment", INIVariableEnum.SHOW_SLOP_ERROR.ToString(), SHOW_SLOP_ERROR ? "1" : "0", INIFILE);
            WriteINIValue("Environment", INIVariableEnum.SHOW_HL_ERROR.ToString(), SHOW_HL_ERROR ? "1" : "0", INIFILE);
            WriteINIValue("Environment", INIVariableEnum.GOODCHECK.ToString(), GOODCHECK ? "1" : "0", INIFILE);
            WriteINIValue("Environment", INIVariableEnum.TWOSTAGE.ToString(), TWOSTAGE ? "1" : "0", INIFILE);
            WriteINIValue("Environment", INIVariableEnum.HAVEKEYBASE.ToString(), HAVEKEYBASE ? "1" : "0", INIFILE);
            WriteINIValue("Environment", INIVariableEnum.ISUSEARROUND.ToString(), ISUSEARROUND ? "1" : "0", INIFILE);
            WriteINIValue("Environment", INIVariableEnum.ISUSEDELTA.ToString(), ISUSEDELTA ? "1" : "0", INIFILE);
            WriteINIValue("Environment", INIVariableEnum.ISDARFONREPORT.ToString(), ISDARFONREPORT ? "1" : "0", INIFILE);
            WriteINIValue("Environment", INIVariableEnum.ISFACTORENABLE.ToString(), ISFACTORENABLE ? "1" : "0", INIFILE);
            WriteINIValue("Environment", INIVariableEnum.ISPOPUPMSG.ToString(), ISPOPUPMSG ? "1" : "0", INIFILE);
            WriteINIValue("Environment", INIVariableEnum.ISSPACEFLAT.ToString(), ISSPACEFLAT ? "1" : "0", INIFILE);
            WriteINIValue("Environment", INIVariableEnum.ISADJUST.ToString(), ISADJUST ? "1" : "0", INIFILE);
            WriteINIValue("Environment", INIVariableEnum.ISCHECKINGDUP.ToString(), ISCHECKINGDUP ? "1" : "0", INIFILE);
            WriteINIValue("Environment", INIVariableEnum.ISAISYS.ToString(), ISAISYS ? "1" : "0", INIFILE);
            WriteINIValue("Environment", INIVariableEnum.ISABSSHOWRESULT.ToString(), ISABSSHOWRESULT ? "1" : "0", INIFILE);
            WriteINIValue("Environment", INIVariableEnum.ISUSEDUP.ToString(), ISUSEDUP ? "1" : "0", INIFILE);
            WriteINIValue("Environment", INIVariableEnum.ISMAINPROGRAM.ToString(), ISMAINPROGRAM ? "1" : "0", INIFILE);
            WriteINIValue("Environment", INIVariableEnum.ISROTATE.ToString(), ISROTATE ? "1" : "0", INIFILE);
            WriteINIValue("Environment", INIVariableEnum.ISSUNREXSF.ToString(), ISSUNREXSF ? "1" : "0", INIFILE);
            WriteINIValue("Environment", INIVariableEnum.ISLIMITBARCODE.ToString(), ISLIMITBARCODE ? "1" : "0", INIFILE);

            WriteINIValue("Environment", INIVariableEnum.DUPRATIO.ToString(), DUPRATIO.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.DUPCOUNT.ToString(), DUPCOUNT.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.FINDCONTRAST.ToString(), FINDCONTRAST.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.LSHIFTDIFF.ToString(), LSHIFTDIFF.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.RSHIFTDIFF.ToString(), RSHIFTDIFF.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.BASEZLOCATION.ToString(), BASEZLOCATION.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.READYZLOCATION.ToString(), READYZLOCATION.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.MAGZLOCATION.ToString(), MAGZLOCATION.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.TESTLOCATION.ToString(), TESTZLOCATION.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.BASEULOCATION.ToString(), BASEULOCATION.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.READYULOCATION.ToString(), BASEULOCATION.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.SLOWULOCATION.ToString(), SLOWULOCATION.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.BASEYLOCATION.ToString(), BASEYLOCATION.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.READYYLOCATION.ToString(), READYYLOCATION.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.MACHINETYPE.ToString(), MACHINETYPE.ToString(), INIFILE);
            WriteINIValue("Environment", INIVariableEnum.BARCODELENGTH.ToString(), BARCODELENGTH.ToString(), INIFILE);
        }

        static Point StringtoPoint(string PtString)
        {
            string[] str = PtString.Split(',');
            return new Point(int.Parse(str[0]), int.Parse(str[1]));
        }
        static string PointtoString(Point Pt)
        {
            return Pt.X.ToString().PadLeft(4) + "," + Pt.Y.ToString().PadLeft(4);
        }
    }
}
