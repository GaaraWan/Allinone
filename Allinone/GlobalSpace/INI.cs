using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;

using JetEazy;
using JetEazy.BasicSpace;

namespace Allinone
{
    enum INIEnum
    {
        //[Baisc Control]
        MACHINENAMEID,
        MACHINENAME,            //文字，機器名稱
        LANGUAGE,               //數字，0是中文，1是英文
        DELAYTIME,
        SHOPFLOORPATH,
        PRELOADNO,              //預載入的資料
        PRELOADSTATICNO,        //預載入80000的編號，此為預載的編號
        PRELOADCOUNT,           //可載入的上限

        ISSAVEWITHTIMESTAMP,    //儲存同序號的東西時，是否要加入TIMESTAMP
        ISLIVECAPTURE,          //是否要即時抓圖

        ISONLYSHOWNG,           //是否只秀出NG
        RETESTTIME,
        ISPLAYSOUND,
        ISONLYCHECKSN,          //是否只检查SN
        CHECKPAGE,              //是否检查对应页

        CHECKSNERRORCODE,

        ISNEEDSN,               //是否必需要SN    
        ISMANYPAR,             //是否多机种混测

        ISONLYCHICKWB,         //只检查黑白的螺丝

        ISCHECKMEMBRANE,       //检查有无放膜

        LASERNGADDBC,          //如果雷雕不良，是否在回复SF时增加BC 字样

        FINGERPRINT,        //是否是用指纹模块登入登出


        BCNGCOUNT,            //雷雕的数量
        ALLCOUNT,             //当天生产的数量
        DATATIMERNOW,         //当天生产的日期

        R3VENDOR,              //R3中默认的供应商（sn.txt中如没有指定时，默认）


        R5MOTORTO485,             //R5 用485来控制电机

        R5RUNCOUNT,             //R5 有多少个位置
        R5MOTORCOUNT,
        /// <summary>
        /// 是否是SF给颜色
        /// </summary>
        ISSFCOLOR,


        //[R32 Control]
        ISCHECKQSMCDUP,
        /// <summary>
        /// 是否保存OCR测试图
        /// </summary>
        ISSAVEOCRIMAGE,
        /// <summary>
        /// 是否检查SN缺失
        /// </summary>
        ISCHECKSNDEFECT,
        CAMBIASCOUNT,
        /// <summary>
        /// SF工厂
        /// </summary>
        SFFACTORY,
        /// <summary>
        /// SF的应用程式地址
        /// </summary>
        SFPATHEXE,



        ISQSMCALLSAVE,
        ALLRESULTPIC,
        //[DFLY Control]
        IPSTR,
        CAMERALOCATION,
        LENSLOCATION,

        //[Allinone Server Control]
        ISUSEJUMBOSERVER,
        ISUSECRYSTALSERVER,
        GAP_ZAXIS_OFFSET,
        JUMBO_SERVER_CAM_COUNT,
        JUMBO_SERVER_CAM_RELATION,
        CRYSTAL_SERVER_CAM_COUNT,
        CRYSTAL_SERVER_CAM_RELATION,

        KEYCAPEXPOSURE,
        FRAMEEXPOSURE,


        //Hiveclient Control
        ISHIVECLIENT,
        HIVE_publisher_id,
        HIVE_site,
        HIVE_building,
        HIVE_line_type,
        HIVE_line,
        HIVE_station_type,
        HIVE_station_instance,
        HIVE_vendor,
        HIVE_exe_path,

        //DataCollection Control
        DATA_Program,
        DATA_Building_Config,
        DATA_FIXTUREID,

        //FOXCONN Control
        ISFOXCONNSF,
        DATA_SCREW_TEN,

        /// <summary>
        /// CCD运动拍照延时
        /// </summary>
        NextDuriation,
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

            StringBuilder temp = new StringBuilder(200);
            int Length = GetPrivateProfileString(section, key, "", temp, 200, filepath);

            retStr = temp.ToString();

            if (retStr == "")
                retStr = defaultvaluestring;
            //else
            //    retStr = retStr.Split('/')[0]; //把說明排除掉

            return retStr;

        }
        static string ReadINIValueOLD(string section, string key, string defaultvaluestring, string filepath)
        {
            string retStr = "";

            StringBuilder temp = new StringBuilder(200);
            int Length = GetPrivateProfileString(section, key, "", temp, 200, filepath);

            retStr = temp.ToString();

            if (retStr == "")
                retStr = defaultvaluestring;
            //else
            //    retStr = retStr.Split('/')[0]; //把說明排除掉

            return retStr;

        }
        #endregion

        static string MAINPATH = "";
        static string INIFILE = "";

        static JzToolsClass JzTools = new JzToolsClass();

        public static bool ShowMarkFrm = false;

        public static bool IsOpenIOWindows = false;

        //[Baisc Control]
        public static string MACHINENAME = "ALLINONE";
        public static string MACHINENAMEID = "AOI001";
        public static int LANGUAGE = 0;
        public static int DELAYTIME = 1000;
        public static string SHOPFLOORPATH = @"D:\DATA";
        public static string PRELOADNO = "";        //除了 80000 之外的需要預載的參數
        public static string PRELOADSTATICNO = "";  //80000 的指定數字可以改變，用逗號分隔
        public static int PRELOADCOUNT = 5;

        public static bool ISSAVEWITHTIMESTAMP = false;
        public static bool ISLIVECAPTURE = false;
        public static bool ISONLYSHOWNG = false;
        public static int RETESTTIME = 0;
        public static bool ISPLAYSOUND = false;
        public static bool ISONLYCHECKSN = false;
        //只检查黑白的螺丝
        public static bool ISONLYCHICKWB = false;
        /// <summary>
        /// 是否检测 页码
        /// </summary>
        public static bool[] CHECKPAGE;
        public static string CHECKSNERRORCODE = "N";
        /// <summary>
        /// 是多机种混测(此项打开,会检查参数名及参数版本)
        /// </summary>
        public static bool ISMANYPAR = false;

        public static string R3VENDOR = "";

        /// <summary>
        /// 是否是SF给颜色
        /// </summary>
        public static bool ISSFCOLOR = false;


        //[R32 Control]
        public static bool ISCHECKQSMCDUP = false;
        /// <summary>
        /// 是否保存OCR测试图
        /// </summary>
        public static bool ISSAVEOCRIMAGE = false;

        /// <summary>
        /// 是否保存侦错图
        /// </summary>
        public static bool ISSAVEDebugIMAGE = false;
        /// <summary>
        /// 是否检查SN缺失
        /// </summary>
        public static bool ISCHECKSNDEFECT = false;
        public static int CAMBIASCOUNT = 9;

        public static bool ISQSMCALLSAVE = false;
        public static string ALLRESULTPIC = @"D:\ALLRESULTPIC";
        public static string APPLERESURT = @"D:\SKYNETDATA";



        public static int R5RUNCOUNT = 4;           //R5 有多少个位置

        //[DFLY Control]
        public static string IPSTR = "192.168.0.178:4001";
        public static string CAMERALOCATION = "0,0,0";
        public static string LENSLOCATION = "0,0,0";

        public static float[] LENSOFFSET = new float[3];

        //[Allinone Server Control]
        public static bool ISUSEJUMBOSERVER = false;
        public static bool ISUSECRYSTALSERVER = false;
        public static float GAP_ZAXIS_OFFSET = 0f;
        public static int JUMBO_SERVER_CAM_COUNT = 6;
        public static string JUMBO_SERVER_CAM_RELATION = "006,007,008";
        public static int CRYSTAL_SERVER_CAM_COUNT = 6;
        public static string CRYSTAL_SERVER_CAM_RELATION = "000,001,002,003,004,005";

        public static string KEYCAPEXPOSURE = "0#40;1#40;2#40;3#40;4#40";
        public static string FRAMEEXPOSURE = "0#70;1#70;2#70;3#70;4#70";

        //Hiveclient Control
        public static bool ISHIVECLIENT = false;
        public static string HIVE_publisher_id = "9eefa293-7f48-41ed-bb9c-e0700a9e3c52";
        public static string HIVE_site = "QSMC";
        public static string HIVE_building = "F5";
        public static string HIVE_line_type = "CR";
        public static string HIVE_line = "L25";
        public static string HIVE_station_type = "Particle_Inspection";
        public static string HIVE_station_instance = "1";
        public static string HIVE_vendor = "JetEazy";
        public static string HIVE_exe_path = "";
        public static string HIVE_model = "";
        public static bool HIVE_islocalsystemupload = true;
        public static Rectangle HIVE_rectangle_corp = new Rectangle(0, 0, 3840, 2764);

        //Data Collection Control
        public static string DATA_Program = "NA";
        public static string DATA_Building_Config = "NA";
        public static string DATA_FIXTUREID = "FIXTUREID";
        public static bool DATA_SCREW_TEN = false;

        public static string SDM5FindCount = "";

        //MainX6 Control
        /// <summary>
        /// 0:原始参数 1:TYPE_18 切换位1800W 用数字代替 为了以后更好的扩展
        /// 将来扩展的话 就在原始的文件夹后面 + _TYPE18 定义名称
        /// 重启生效
        /// </summary>
        public static int CHANGE_FILE_PATH = 0;
        public static bool show_simform = false;

        public static string tcp_ip = "127.0.0.1";
        public static int tcp_port = 6000;

        public static bool tcp_handle_open = false;
        public static string tcp_handle_ip = "127.0.0.1";
        public static int tcp_handle_port = 32001;

        public static int user_screen_width = 1280;
        public static int user_screen_height = 1024;
        public static float user_screen_scale = 0.95f;
        public static bool user_screen_bold = false;

        /// <summary>
        /// 开启和关闭读取德龙返回的接收到了的信号
        /// </summary>
        public static bool IsReadHandlerOKSign = false;
        /// <summary>
        /// 不接收handler的完成信号
        /// </summary>
        public static bool IsNoUseHandlerOKSign = false;

        /// <summary>
        /// 接收tcp的启动信号
        /// </summary>
        public static bool IsUseTcpStart = false;

        /// <summary>
        /// 开启和关闭发送德龙完成信号 通过tcp
        /// </summary>
        public static bool IsSendHandlerTcpOKSign = false;

        public static bool IsCollectErrorSmall = false;
        public static bool IsCollectStripPictures = false;
        public static bool IsLightAlwaysOn = false;
        public static bool IsCollectPictures = false;
        public static bool IsSaveScreen = false;
        public static bool IsCheckBarcodeOpen = false;
        /// <summary>
        /// 画面只显示当前图像
        /// </summary>
        public static bool IsOnlyShowCurrentImage = false;
        public static bool IsAdminMode = false;
        public static bool IsCollectPicturesSingle = false;
        public static string AI_Model_FilenamePath = string.Empty;
        public static bool IsOpenCheckRepeatCode = false;
        public static bool IsOpenCheckCurLotRepeatCode = false;
        /// <summary>
        /// 强制全检
        /// </summary>
        public static bool IsOpenForceAllCheck = false;
        public static bool IsOpenBehindOKSign = false;
        public static bool IsOpenCip = false;
        /// <summary>
        /// 开启QC抽检功能
        /// </summary>
        public static bool IsOpenQcRandom = false;
        /// <summary>
        /// 北京使用文件传递map资料
        /// </summary>
        public static bool IsOpenUseFileMap = false;
        public static string FileMapPath = "D:\\";
        public static int AutoLogoutTime = 30;
        /// <summary>
        /// 强制关闭重复码
        /// </summary>
        public static bool IsOpenForceNoCheckRepeat = true;//强制关闭重复码
        public static int FactoryNameIndex = 0;
        public static int MappingTypeIndex = 0;
        public static bool bUse2DCNNReader = false;


        /// <summary>
        /// 是否开启容错率
        /// </summary>
        public static bool IsOpenFaultToleranceRate = false;
        public static double FaultToleranceRate = 0.05;

        /// <summary>
        /// 显示等级码
        /// </summary>
        public static bool IsOpenShowGrade = false;//显示等级码

        public static bool IsOpenAutoChangeRecipe = false;
        public static PMatchType pMatchType = PMatchType.HPM;
        public static float fTolerance = 0.7f;

        //Jcet Contorl
        public static bool JCET_IS_USE_SHOPFLOOR = false;
        public static int JCET_STRIP_BUFF = 0;
        public static string JCET_WEBSERVICE_URL = "http://localhost:34489/Service1.asmx";
        public static int JCET_TIMESTOP_SET = 3;

        /// <summary>
        /// 与主机的通讯延时 单位ms
        /// </summary>
        public static int handle_delaytime = 500;

        //MainSD Control
        /// <summary>
        /// 用户设定满盒数量PASS
        /// </summary>
        public static int USER_SET_FULL_PASSCOUNT = 10;
        public static int USER_SET_FULL_NGCOUNT = 10;

        /// <summary>
        /// 连续NG数目
        /// </summary>
        public static int CONTINUE_NG_COUNT = 3;

        /// <summary>
        /// 拍照延时
        /// </summary>
        public static int MAINSD_GETIMAGE_DELAYTIME = 200;

        /// <summary>
        /// 拍照延时
        /// </summary>
        public static int MAINSDM1_GETSTART_DELAYTIME = 3000;

        public static int AXIS_X_JJS = 500;
        public static int AXIS_Y_JJS = 500;
        public static int AXIS_Z_JJS = 500;
        //public static int AXIS_U_JJS = 500;
        public static double RobotSpeedValue = 20;

        public static int AXIS_MANUAL_JJS_ADD = 100;
        public static int AXIS_MANUAL_JJS_SUB = 100;
        public static int AXIS_AUTO_JJS_ADD = 1000;
        public static int AXIS_AUTO_JJS_SUB = 1000;

        public static double MAINSD_PAD_MIL_RESOLUTION = 0.022;
        public static bool CHIP_NG_SHOW = false;
        public static bool CHIP_NG_collect = false;
        public static bool CHIP_force_pass = false;
        public static bool CHIP_forceALIGNRUN_pass = false;
        public static bool CHIP_ISSMOOTHEN = false;
        /// <summary>
        /// 0无 1线性模式 2平等模式(即长宽相等)
        /// </summary>
        public static int CHIP_CAL_MODE = 0;
        /// <summary>
        /// 用于测试时是否保存图片
        /// </summary>
        public static bool IsDEBUGCHIP = false;

        /// <summary>
        /// 开启通过参数名记录数据
        /// </summary>
        public static bool IsOpenRecipeDataRecord = false;

        /// <summary>
        /// 开启判断sensor
        /// </summary>
        public static bool IsOpenCheckSensor = false;
        public static bool IsOpenCheckSensor2 = false;

        public static DateTime xClearDataTime1 { get; set; } = DateTime.Now;
        public static DateTime xClearDataTime2 { get; set; } = DateTime.Now;

        public static float CamLinescanStartPos = -7;
        public static float CamLinescanEndPos = 270;
        public static int CamLinescanSpeed = 50;
        public static float CamAreaMatchPos = 85;

        public static int TestImageOvertime = 15000;
        public static int TestResultOvertime = 10000;

        public static int chipTestAllCount = 0;
        public static int chipTestPassCount = 0;
        public static int chipTestFailCount = 0;
        public static int chipTestNoChipCount = 0;
        public static int chipN1Count = 0;
        public static int chipN2Count = 0;
        public static int chipN3Count = 0;
        public static int chipN4Count = 0;
        public static int chipN5Count = 0;
        public static bool chipUseAI = false;
        public static string AI_IP = "127.0.0.1";
        public static int AI_Port = 9001;

        public static SaveImageFormat chipSaveImageFormat = SaveImageFormat.IMAGE_JPEG;

        public static Color chipPassColor = Color.Lime;


        public static int ChkArea = 1000000;
        public static int ChkWidth = 1000;
        public static int ChkHeight = 1000;


        public static double keyx = 0;
        public static double keyy = 0;
        public static double keyz = 0;
        public static int keyrow = 1;
        public static int keycol = 1;
        public static double keyoffsetx = 0;
        public static double keyoffsety = 0;
        public static string op_keyxyz = "0,0,0";

        public static double keyendx = 0;
        public static double keyendy = 0;
        public static double keyendz = 0;

        //FOXCONN Control
        public static bool ISFOXCONNSF = false;

        //QFactory Control
        public static bool ISUSE_QFACTORY = false;
        public static int QFACTORY_CHECK_TIME = 60;
        public static string QFACTORY_EQ_SN = "0";
        public static string QFACTORY_EQ_LocationID = "0";
        public static string QFACTORY_EQ_LocationID2 = "0";
        public static string QFACTORY_Station = "EQ";
        public static string QFACTORY_Step = "EQMonitor";

        /// <summary>
        /// 富士康SF是否启用
        /// </summary>
        public static FactoryShopfloor SFFACTORY = FactoryShopfloor.NONE;
        /// <summary>
        /// 富士康SF的应用程式地址
        /// </summary>
        public static string SFPATHEXE = "";
        /// <summary>
        /// 是否必需要SN
        /// </summary>
        public static bool ISNEEDSN = true;

        /// <summary>
        /// R3 下边距补尝
        /// </summary>
        public static float fR3DCompensate = 0;
        /// <summary>
        /// R3 左边距补尝
        /// </summary>
        public static float fR3LCompensate = 0;
        /// <summary>
        /// R3 右边距补尝
        /// </summary>
        public static float fR3RCompensate = 0;


        /// <summary>
        /// C3 补偿值
        /// </summary>
        public static float fC3_A = 0;
        /// <summary>
        /// C3 补偿值
        /// </summary>
        public static float fC3_B = 0;
        /// <summary>
        /// C3 补偿值
        /// </summary>
        public static float fC3_C = 0;
        /// <summary>
        /// C3 补偿值
        /// </summary>
        public static float fC3_D = 0;
        /// <summary>
        /// C3 补偿值
        /// </summary>
        public static float fC3_E = 0;
        /// <summary>
        /// C3 补偿值
        /// </summary>
        public static float fC3_F = 0;
        /// <summary>
        /// C3 补偿值
        /// </summary>
        public static float fC3_G = 0;
        /// <summary>
        /// C3 补偿值
        /// </summary>
        public static float fC3_H = 0;
        /// <summary>
        /// 是否检查有无放膜
        /// </summary>
        public static bool isCheckMembrane = false;


        public static bool isAccounFingerprint = false;


        public static bool isR5_MOTOR_TO_Rs485 = false;
        /// <summary>
        ///有多少个轴
        /// </summary>
        public static int iR5MOTORCOUNT = 9;
        /// <summary>
        /// CCD运动延时
        /// </summary>
        public static int NextDuriation = 200;
        public static void Initial()
        {
            MAINPATH = Universal.MAINPATH;
            INIFILE = MAINPATH + "\\CONFIG.ini";

            Load();
        }

        public static void Load()
        {
            SDM5FindCount = ReadINIValue("Basic Control", "SDM5FindCount", SDM5FindCount, INIFILE);

            //[Basic Control]
            MACHINENAME = ReadINIValue("Basic Control", INIEnum.MACHINENAME.ToString(), MACHINENAME, INIFILE);
            MACHINENAMEID = ReadINIValue("Basic Control", INIEnum.MACHINENAMEID.ToString(), MACHINENAMEID, INIFILE);
            LANGUAGE = int.Parse(ReadINIValue("Basic Control", INIEnum.LANGUAGE.ToString(), LANGUAGE.ToString(), INIFILE));

            DELAYTIME = int.Parse(ReadINIValue("Basic Control", INIEnum.DELAYTIME.ToString(), DELAYTIME.ToString(), INIFILE));

            SHOPFLOORPATH = ReadINIValue("Basic Control", INIEnum.SHOPFLOORPATH.ToString(), SHOPFLOORPATH, INIFILE);
            PRELOADNO = ReadINIValue("Basic Control", INIEnum.PRELOADNO.ToString(), PRELOADNO, INIFILE);
            PRELOADSTATICNO = ReadINIValue("Basic Control", INIEnum.PRELOADSTATICNO.ToString(), PRELOADSTATICNO, INIFILE);
            PRELOADCOUNT = int.Parse(ReadINIValue("Basic Control", INIEnum.PRELOADCOUNT.ToString(), PRELOADCOUNT.ToString(), INIFILE));

            ISSAVEWITHTIMESTAMP = int.Parse(ReadINIValue("Basic Control", INIEnum.ISSAVEWITHTIMESTAMP.ToString(), "0", INIFILE)) == 1;
            ISLIVECAPTURE = int.Parse(ReadINIValue("Basic Control", INIEnum.ISLIVECAPTURE.ToString(), "0", INIFILE)) == 1;
            ISONLYSHOWNG = int.Parse(ReadINIValue("Basic Control", INIEnum.ISONLYSHOWNG.ToString(), "1", INIFILE)) == 1;

            RETESTTIME = int.Parse(ReadINIValue("Basic Control", INIEnum.RETESTTIME.ToString(), RETESTTIME.ToString(), INIFILE));
            ISPLAYSOUND = int.Parse(ReadINIValue("Basic Control", INIEnum.ISPLAYSOUND.ToString(), "0", INIFILE)) == 1;
            ISONLYCHECKSN = int.Parse(ReadINIValue("Basic Control", INIEnum.ISONLYCHECKSN.ToString(), "0", INIFILE)) == 1;
            R3VENDOR = ReadINIValue("Basic Control", INIEnum.R3VENDOR.ToString(), "", INIFILE);

            isR5_MOTOR_TO_Rs485 = ReadINIValue("Basic Control", INIEnum.R5MOTORTO485.ToString(), "0", INIFILE) == "1";
            iR5MOTORCOUNT = int.Parse(ReadINIValue("Basic Control", INIEnum.R5MOTORCOUNT.ToString(), "9", INIFILE));

            FactoryNameIndex = int.Parse(ReadINIValue("Basic Control", "FactoryNameIndex", FactoryNameIndex.ToString(), INIFILE));
            MappingTypeIndex = int.Parse(ReadINIValue("Basic Control", "MappingTypeIndex", MappingTypeIndex.ToString(), INIFILE));
            bUse2DCNNReader = ReadINIValue("Basic Control", "bUse2DCNNReader", "0", INIFILE) == "1";

            string page = ReadINIValue("Basic Control", INIEnum.CHECKPAGE.ToString(), "0", INIFILE);

            NextDuriation = int.Parse(ReadINIValue("Basic Control", INIEnum.NextDuriation.ToString(), "500", INIFILE));

            R5RUNCOUNT = int.Parse(ReadINIValue("Basic Control", INIEnum.R5RUNCOUNT.ToString(), "4", INIFILE));

            ISSFCOLOR = ReadINIValue("Basic Control", INIEnum.ISSFCOLOR.ToString(), "0", INIFILE) == "1";

            chipPassColor = Color.FromArgb(int.Parse(ReadINIValue("Basic Control", "chipPassColor", chipPassColor.ToArgb().ToString(), INIFILE)));
            

            CHECKPAGE = new bool[100];
            for (int i = 0; i < CHECKPAGE.Length; i++)
                CHECKPAGE[i] = true;
            if (page != "0")
            {
                string[] strTemp = page.Trim().Trim(',').Split(',');
                for (int i = 0; i < strTemp.Length; i++)
                    CHECKPAGE[i] = (strTemp[i] == "1");
            }
            CHECKSNERRORCODE = ReadINIValue("Basic Control", INIEnum.CHECKSNERRORCODE.ToString(), CHECKSNERRORCODE, INIFILE);

            ISNEEDSN = ReadINIValue("Basic Control", INIEnum.ISNEEDSN.ToString(), "1", INIFILE) == "1";

            ISMANYPAR = ReadINIValue("Basic Control", INIEnum.ISMANYPAR.ToString(), "0", INIFILE) == "1";

            ISONLYCHICKWB = ReadINIValue("Basic Control", INIEnum.ISONLYCHICKWB.ToString(), "0", INIFILE) == "1";

            isCheckMembrane = ReadINIValue("Basic Control", INIEnum.ISCHECKMEMBRANE.ToString(), "0", INIFILE) == "1";

            LaserNgAddBC = ReadINIValue("Basic Control", INIEnum.LASERNGADDBC.ToString(), "1", INIFILE) == "1";

            string strFingerprint = ReadINIValue("Basic Control", INIEnum.FINGERPRINT.ToString(), "-1", INIFILE);
            if (strFingerprint == "-1")
            {
                WriteINIValue("Basic Control", INIEnum.FINGERPRINT.ToString(), "0", INIFILE);
                isAccounFingerprint = false;
            }
            else
                isAccounFingerprint = strFingerprint == "1";



            myBCNGCOUNT = int.Parse(ReadINIValue("Basic Control", INIEnum.BCNGCOUNT.ToString(), "-1", INIFILE));
            myALLCOUNT = int.Parse(ReadINIValue("Basic Control", INIEnum.ALLCOUNT.ToString(), "-1", INIFILE));

            if (myBCNGCOUNT == -1)
                BCNGCOUNT = 0;
            if (myALLCOUNT == -1)
                ALLCOUNT = 0;

            string strtemp = ReadINIValueOLD("Basic Control", INIEnum.DATATIMERNOW.ToString(), "-1", INIFILE);
            if (strtemp == "-1")
            {
                DATATIMERNOW = DateTime.Now;
                strtemp = ReadINIValueOLD("Basic Control", INIEnum.DATATIMERNOW.ToString(), DATATIMERNOW.ToString(), INIFILE);
            }
            myDATATIMERNOW = DateTime.Parse(strtemp);
            CheckBCData();


            //[R32 Control]
            ISCHECKQSMCDUP = int.Parse(ReadINIValue("R32 Control", INIEnum.ISCHECKQSMCDUP.ToString(), "0", INIFILE)) == 1;
            ISSAVEOCRIMAGE = ReadINIValue("R32 Control", INIEnum.ISSAVEOCRIMAGE.ToString(), "0", INIFILE) == "1" ? true : false;
            ISCHECKSNDEFECT = ReadINIValue("R32 Control", INIEnum.ISCHECKSNDEFECT.ToString(), "0", INIFILE) == "1" ? true : false;
            CAMBIASCOUNT = int.Parse(ReadINIValue("R32 Control", INIEnum.CAMBIASCOUNT.ToString(), CAMBIASCOUNT.ToString(), INIFILE));
            SFFACTORY = (FactoryShopfloor)Enum.Parse(typeof(FactoryShopfloor), ReadINIValue("R32 Control", INIEnum.SFFACTORY.ToString(), SFFACTORY.ToString(), INIFILE));
            SFPATHEXE = ReadINIValue("R32 Control", INIEnum.SFPATHEXE.ToString(), "null", INIFILE);
            chipSaveImageFormat = (SaveImageFormat)int.Parse(ReadINIValue("Basic Control", "chipSaveImageFormat", ((int)chipSaveImageFormat).ToString(), INIFILE));

            ISQSMCALLSAVE = ReadINIValue("R32 Control", INIEnum.ISQSMCALLSAVE.ToString(), "0", INIFILE) == "1";
            ALLRESULTPIC = ReadINIValue("R32 Control", INIEnum.ALLRESULTPIC.ToString(), ALLRESULTPIC, INIFILE);
            //[DFLY Control]
            IPSTR = ReadINIValue("DFLY Control", INIEnum.IPSTR.ToString(), IPSTR, INIFILE);
            CAMERALOCATION = ReadINIValue("DFLY Control", INIEnum.CAMERALOCATION.ToString(), CAMERALOCATION, INIFILE);
            LENSLOCATION = ReadINIValue("DFLY Control", INIEnum.LENSLOCATION.ToString(), LENSLOCATION, INIFILE);
            GetLENSOFFSET();

            //[Allinone Server  Control]
            ISUSEJUMBOSERVER = int.Parse(ReadINIValue("Allinone Server Control", INIEnum.ISUSEJUMBOSERVER.ToString(), "0", INIFILE)) == 1;
            ISUSECRYSTALSERVER = int.Parse(ReadINIValue("Allinone Server Control", INIEnum.ISUSECRYSTALSERVER.ToString(), "0", INIFILE)) == 1;
            GAP_ZAXIS_OFFSET = float.Parse(ReadINIValue("Allinone Server Control", INIEnum.GAP_ZAXIS_OFFSET.ToString(), "0", INIFILE));
            JUMBO_SERVER_CAM_COUNT = int.Parse(ReadINIValue("Allinone Server Control", INIEnum.JUMBO_SERVER_CAM_COUNT.ToString(), JUMBO_SERVER_CAM_COUNT.ToString(), INIFILE));
            JUMBO_SERVER_CAM_RELATION = ReadINIValue("Allinone Server Control", INIEnum.JUMBO_SERVER_CAM_RELATION.ToString(), JUMBO_SERVER_CAM_RELATION, INIFILE);
            CRYSTAL_SERVER_CAM_COUNT = int.Parse(ReadINIValue("Allinone Server Control", INIEnum.CRYSTAL_SERVER_CAM_COUNT.ToString(), CRYSTAL_SERVER_CAM_COUNT.ToString(), INIFILE));
            CRYSTAL_SERVER_CAM_RELATION = ReadINIValue("Allinone Server Control", INIEnum.CRYSTAL_SERVER_CAM_RELATION.ToString(), CRYSTAL_SERVER_CAM_RELATION, INIFILE);

            KEYCAPEXPOSURE = ReadINIValue("Allinone Server Control", INIEnum.KEYCAPEXPOSURE.ToString(), KEYCAPEXPOSURE, INIFILE);
            FRAMEEXPOSURE = ReadINIValue("Allinone Server Control", INIEnum.FRAMEEXPOSURE.ToString(), FRAMEEXPOSURE, INIFILE);

            //[Hiveclient Control]
            ISHIVECLIENT = int.Parse(ReadINIValue("Hiveclient Control", INIEnum.ISHIVECLIENT.ToString(), "0", INIFILE)) == 1;
            HIVE_publisher_id = ReadINIValue("Hiveclient Control", INIEnum.HIVE_publisher_id.ToString(), "9eefa293-7f48-41ed-bb9c-e0700a9e3c52", INIFILE);
            HIVE_site = ReadINIValue("Hiveclient Control", INIEnum.HIVE_site.ToString(), "QSMC", INIFILE);
            HIVE_building = ReadINIValue("Hiveclient Control", INIEnum.HIVE_building.ToString(), "F5", INIFILE);
            HIVE_line_type = ReadINIValue("Hiveclient Control", INIEnum.HIVE_line_type.ToString(), "CR", INIFILE);
            HIVE_line = ReadINIValue("Hiveclient Control", INIEnum.HIVE_line.ToString(), "L25", INIFILE);
            HIVE_station_type = ReadINIValue("Hiveclient Control", INIEnum.HIVE_station_type.ToString(), "Particle_Inspection", INIFILE);
            HIVE_station_instance = ReadINIValue("Hiveclient Control", INIEnum.HIVE_station_instance.ToString(), "1", INIFILE);
            HIVE_vendor = ReadINIValue("Hiveclient Control", INIEnum.HIVE_vendor.ToString(), "JetEazy", INIFILE);
            HIVE_exe_path = ReadINIValue("Hiveclient Control", INIEnum.HIVE_exe_path.ToString(), "", INIFILE);

            HIVE_model = ReadINIValue("Hiveclient Control", "HIVE_model", "", INIFILE);
            HIVE_islocalsystemupload = int.Parse(ReadINIValue("Hiveclient Control", "HIVE_islocalsystemupload", "1", INIFILE)) == 1;
            HIVE_rectangle_corp = SimpleStringToRect(ReadINIValue("Hiveclient Control", "HIVE_rectangle_corp", RectToStringSimple(HIVE_rectangle_corp), INIFILE));


            //[QFactory Control]
            ISUSE_QFACTORY = int.Parse(ReadINIValue("QFactory Control", "ISUSE_QFACTORY", "0", INIFILE)) == 1;
            QFACTORY_CHECK_TIME = int.Parse(ReadINIValue("QFactory Control", "QFACTORY_CHECK_TIME", "60", INIFILE));
            QFACTORY_EQ_SN = ReadINIValue("QFactory Control", "QFACTORY_EQ_SN", "0", INIFILE);
            QFACTORY_EQ_LocationID = ReadINIValue("QFactory Control", "QFACTORY_EQ_LocationID", "0", INIFILE);
            QFACTORY_EQ_LocationID2 = ReadINIValue("QFactory Control", "QFACTORY_EQ_LocationID2", "0", INIFILE);
            QFACTORY_Station = ReadINIValue("QFactory Control", "QFACTORY_Station", "STATION001", INIFILE);
            QFACTORY_Step = ReadINIValue("QFactory Control", "QFACTORY_Step", "EQMonitor", INIFILE);

            //[Data Collection Control]
            DATA_Program = ReadINIValue("Data Collection Control", INIEnum.DATA_Program.ToString(), "NA", INIFILE);
            DATA_Building_Config = ReadINIValue("Data Collection Control", INIEnum.DATA_Building_Config.ToString(), "NA", INIFILE);
            DATA_FIXTUREID = ReadINIValue("Data Collection Control", INIEnum.DATA_FIXTUREID.ToString(), DATA_FIXTUREID, INIFILE);
            DATA_SCREW_TEN = int.Parse(ReadINIValue("Data Collection Control", INIEnum.DATA_SCREW_TEN.ToString(), "0", INIFILE)) == 1;

            //[FOXCONN Control]
            ISFOXCONNSF = int.Parse(ReadINIValue("FOXCONN Control", INIEnum.ISFOXCONNSF.ToString(), "0", INIFILE)) == 1;

            USER_SET_FULL_PASSCOUNT = int.Parse(ReadINIValue("MainSD Control", "USER_SET_FULL_PASSCOUNT", "10", INIFILE));
            USER_SET_FULL_NGCOUNT = int.Parse(ReadINIValue("MainSD Control", "USER_SET_FULL_NGCOUNT", "10", INIFILE));
            CONTINUE_NG_COUNT = int.Parse(ReadINIValue("MainSD Control", "CONTINUE_NG_COUNT", "3", INIFILE));
            MAINSD_GETIMAGE_DELAYTIME = int.Parse(ReadINIValue("MainSD Control", "MAINSD_GETIMAGE_DELAYTIME", "200", INIFILE));
            ChkArea = int.Parse(ReadINIValue("MainSD Control", "ChkArea", ChkArea.ToString(), INIFILE));
            ChkWidth = int.Parse(ReadINIValue("MainSD Control", "ChkWidth", ChkWidth.ToString(), INIFILE));
            ChkHeight = int.Parse(ReadINIValue("MainSD Control", "ChkHeight", ChkHeight.ToString(), INIFILE));
            MAINSD_PAD_MIL_RESOLUTION = double.Parse(ReadINIValue("MainSD Control", "MAINSD_PAD_MIL_RESOLUTION", MAINSD_PAD_MIL_RESOLUTION.ToString("0.000"), INIFILE));

            MAINSDM1_GETSTART_DELAYTIME = int.Parse(ReadINIValue("MainSD Control", "MAINSDM1_GETSTART_DELAYTIME", "3000", INIFILE));

            AXIS_X_JJS = int.Parse(ReadINIValue("MainSD Control", "AXIS_X_JJS", "500", INIFILE));
            AXIS_Y_JJS = int.Parse(ReadINIValue("MainSD Control", "AXIS_Y_JJS", "500", INIFILE));
            AXIS_Z_JJS = int.Parse(ReadINIValue("MainSD Control", "AXIS_Z_JJS", "500", INIFILE));
            AXIS_MANUAL_JJS_ADD = int.Parse(ReadINIValue("MainSD Control", "AXIS_MANUAL_JJS_ADD", "100", INIFILE));
            AXIS_MANUAL_JJS_SUB = int.Parse(ReadINIValue("MainSD Control", "AXIS_MANUAL_JJS_SUB", "100", INIFILE));
            AXIS_AUTO_JJS_ADD = int.Parse(ReadINIValue("MainSD Control", "AXIS_AUTO_JJS_ADD", "1000", INIFILE));
            AXIS_AUTO_JJS_SUB = int.Parse(ReadINIValue("MainSD Control", "AXIS_AUTO_JJS_SUB", "1000", INIFILE));
            RobotSpeedValue = double.Parse(ReadINIValue("MainSD Control", "RobotSpeedValue", "20", INIFILE));


            CHIP_NG_SHOW = ReadINIValue("MainSD Control", "CHIP_NG_SHOW", (CHIP_NG_SHOW ? "1" : "0"), INIFILE) == "1";
            CHIP_NG_collect = ReadINIValue("MainSD Control", "CHIP_NG_collect", (CHIP_NG_collect ? "1" : "0"), INIFILE) == "1";
            CHIP_force_pass = ReadINIValue("MainSD Control", "CHIP_force_pass", (CHIP_force_pass ? "1" : "0"), INIFILE) == "1";
            CHIP_forceALIGNRUN_pass = ReadINIValue("MainSD Control", "CHIP_forceALIGNRUN_pass", (CHIP_forceALIGNRUN_pass ? "1" : "0"), INIFILE) == "1";
            IsDEBUGCHIP = ReadINIValue("MainSD Control", "IsDEBUGCHIP", (IsDEBUGCHIP ? "1" : "0"), INIFILE) == "1";
            CHIP_CAL_MODE = int.Parse(ReadINIValue("MainSD Control", "CHIP_CAL_MODE", "0", INIFILE));
            CHIP_ISSMOOTHEN = ReadINIValue("MainSD Control", "CHIP_ISSMOOTHEN", (CHIP_ISSMOOTHEN ? "1" : "0"), INIFILE) == "1";

            IsOpenCheckSensor = ReadINIValue("MainSD Control", "IsOpenCheckSensor", (IsOpenCheckSensor ? "1" : "0"), INIFILE) == "1";
            IsOpenCheckSensor2 = ReadINIValue("MainSD Control", "IsOpenCheckSensor2", (IsOpenCheckSensor2 ? "1" : "0"), INIFILE) == "1";

            IsOpenRecipeDataRecord = ReadINIValue("Basic Control", "IsOpenRecipeDataRecord", (IsOpenRecipeDataRecord ? "1" : "0"), INIFILE) == "1";
            DataRecordName = ReadINIValue("Basic Control", "DataRecordName", DataRecordName, INIFILE);

            xClearDataTime1 = DateTime.Parse(ReadINIValue("Basic Control", "xClearDataTime1", xClearDataTime1.ToString(), INIFILE));
            xClearDataTime2 = DateTime.Parse(ReadINIValue("Basic Control", "xClearDataTime2", xClearDataTime2.ToString(), INIFILE));

            //chipTestAllCount = int.Parse(ReadINIValue("Basic Control", "chipTestAllCount", "0", INIFILE));
            //chipTestPassCount = int.Parse(ReadINIValue("Basic Control", "chipTestPassCount", "0", INIFILE));
            //chipTestFailCount = int.Parse(ReadINIValue("Basic Control", "chipTestFailCount", "0", INIFILE));
            //chipTestNoChipCount = int.Parse(ReadINIValue("Basic Control", "chipTestNoChipCount", "0", INIFILE));
            //chipN1Count = int.Parse(ReadINIValue("Basic Control", "chipN1Count", "0", INIFILE));
            //chipN2Count = int.Parse(ReadINIValue("Basic Control", "chipN2Count", "0", INIFILE));
            //chipN3Count = int.Parse(ReadINIValue("Basic Control", "chipN3Count", "0", INIFILE));
            //chipN4Count = int.Parse(ReadINIValue("Basic Control", "chipN4Count", "0", INIFILE));
            //chipN5Count = int.Parse(ReadINIValue("Basic Control", "chipN5Count", "0", INIFILE));
            chipUseAI = ReadINIValue("Basic Control", "chipUseAI", (chipUseAI ? "1" : "0"), INIFILE) == "1";
            AI_IP = ReadINIValue("Basic Control", "AI_IP", "127.0.0.1", INIFILE);
            AI_Port = int.Parse(ReadINIValue("Basic Control", "AI_Port", "9001", INIFILE));

            keyx = double.Parse(ReadINIValue("Basic Control", "keyx", "0", INIFILE));
            keyy = double.Parse(ReadINIValue("Basic Control", "keyy", "0", INIFILE));
            keyz = double.Parse(ReadINIValue("Basic Control", "keyz", "0", INIFILE));
            keyoffsetx = double.Parse(ReadINIValue("Basic Control", "keyoffsetx", "0", INIFILE));
            keyoffsety = double.Parse(ReadINIValue("Basic Control", "keyoffsety", "0", INIFILE));
            keyrow = int.Parse(ReadINIValue("Basic Control", "keyrow", "1", INIFILE));
            keycol = int.Parse(ReadINIValue("Basic Control", "keycol", "1", INIFILE));
            op_keyxyz = ReadINIValue("Basic Control", "op_keyxyz", "0,0,0", INIFILE);
            keyendx = double.Parse(ReadINIValue("Basic Control", "keyendx", "0", INIFILE));
            keyendy = double.Parse(ReadINIValue("Basic Control", "keyendy", "0", INIFILE));
            keyendz = double.Parse(ReadINIValue("Basic Control", "keyendz", "0", INIFILE));

            //[MainX6 Control]
            CHANGE_FILE_PATH = int.Parse(ReadINIValue("MainX6 Control", "CHANGE_FILE_PATH", "0", INIFILE));
            tcp_ip = ReadINIValue("MainX6 Control", "tcp_ip", "127.0.0.1", INIFILE);
            tcp_port = int.Parse(ReadINIValue("MainX6 Control", "tcp_port", tcp_port.ToString(), INIFILE));

            tcp_handle_open = ReadINIValue("MainX6 Control", "tcp_handle_open", "0", INIFILE) == "1";
            tcp_handle_ip = ReadINIValue("MainX6 Control", "tcp_handle_ip", "127.0.0.1", INIFILE);
            tcp_handle_port = int.Parse(ReadINIValue("MainX6 Control", "tcp_handle_port", tcp_handle_port.ToString(), INIFILE));

            user_screen_width = int.Parse(ReadINIValue("MainX6 Control", "user_screen_width", user_screen_width.ToString(), INIFILE));
            user_screen_height = int.Parse(ReadINIValue("MainX6 Control", "user_screen_height", user_screen_height.ToString(), INIFILE));
            user_screen_scale = float.Parse(ReadINIValue("MainX6 Control", "user_screen_scale", user_screen_scale.ToString(), INIFILE));
            user_screen_bold = ReadINIValue("MainX6 Control", "user_screen_bold", (user_screen_bold ? "1" : "0"), INIFILE) == "1";
            IsReadHandlerOKSign = ReadINIValue("MainX6 Control", "IsReadHandlerOKSign", (IsReadHandlerOKSign ? "1" : "0"), INIFILE) == "1";
            IsSendHandlerTcpOKSign = ReadINIValue("MainX6 Control", "IsSendHandlerTcpOKSign", (IsSendHandlerTcpOKSign ? "1" : "0"), INIFILE) == "1";
            IsNoUseHandlerOKSign = ReadINIValue("MainX6 Control", "IsNoUseHandlerOKSign", (IsNoUseHandlerOKSign ? "1" : "0"), INIFILE) == "1";

            IsOpenForceAllCheck = ReadINIValue("MainX6 Control", "IsOpenForceAllCheck", (IsOpenForceAllCheck ? "1" : "0"), INIFILE) == "1";
            IsOpenBehindOKSign = ReadINIValue("MainX6 Control", "IsOpenBehindOKSign", (IsOpenBehindOKSign ? "1" : "0"), INIFILE) == "1";

            IsOpenCheckCurLotRepeatCode = ReadINIValue("MainX6 Control", "IsOpenCheckCurLotRepeatCode", (IsOpenCheckCurLotRepeatCode ? "1" : "0"), INIFILE) == "1";
            IsOpenCheckRepeatCode = ReadINIValue("MainX6 Control", "IsOpenCheckRepeatCode", (IsOpenCheckRepeatCode ? "1" : "0"), INIFILE) == "1";
            IsCollectErrorSmall = ReadINIValue("MainX6 Control", "IsCollectErrorSmall", (IsCollectErrorSmall ? "1" : "0"), INIFILE) == "1";
            IsLightAlwaysOn = ReadINIValue("MainX6 Control", "IsLightAlwaysOn", (IsLightAlwaysOn ? "1" : "0"), INIFILE) == "1";
            IsCollectPictures = ReadINIValue("MainX6 Control", "IsCollectPictures", (IsCollectPictures ? "1" : "0"), INIFILE) == "1";
            IsSaveScreen = ReadINIValue("MainX6 Control", "IsSaveScreen", (IsSaveScreen ? "1" : "0"), INIFILE) == "1";
            //IsCheckBarcodeOpen = ReadINIValue("MainX6 Control", "IsCheckBarcodeOpen", (IsCheckBarcodeOpen ? "1" : "0"), INIFILE) == "1";
            IsOnlyShowCurrentImage = ReadINIValue("MainX6 Control", "IsOnlyShowCurrentImage", (IsOnlyShowCurrentImage ? "1" : "0"), INIFILE) == "1";
            IsCollectStripPictures = ReadINIValue("MainX6 Control", "IsCollectStripPictures", (IsCollectStripPictures ? "1" : "0"), INIFILE) == "1";
            IsCollectPicturesSingle = ReadINIValue("MainX6 Control", "IsCollectPicturesSingle", (IsCollectPicturesSingle ? "1" : "0"), INIFILE) == "1";
            AI_Model_FilenamePath = ReadINIValue("MainX6 Control", "AI_Model_FilenamePath", "", INIFILE);
            IsOpenCip = ReadINIValue("MainX6 Control", "IsOpenCip", (IsOpenCip ? "1" : "0"), INIFILE) == "1";
            IsOpenUseFileMap = ReadINIValue("MainX6 Control", "IsOpenUseFileMap", (IsOpenUseFileMap ? "1" : "0"), INIFILE) == "1";
            FileMapPath = ReadINIValue("MainX6 Control", "FileMapPath", FileMapPath, INIFILE);
            IsOpenQcRandom = ReadINIValue("MainX6 Control", "IsOpenQcRandom", (IsOpenQcRandom ? "1" : "0"), INIFILE) == "1";
            IsOpenAutoChangeRecipe = ReadINIValue("MainX6 Control", "IsOpenAutoChangeRecipe", (IsOpenAutoChangeRecipe ? "1" : "0"), INIFILE) == "1";
            pMatchType = (PMatchType)Enum.Parse(typeof(PMatchType), ReadINIValue("MainX6 Control", "pMatchType", pMatchType.ToString(), INIFILE));
            fTolerance = float.Parse(ReadINIValue("MainX6 Control", "fTolerance", fTolerance.ToString(), INIFILE));
            AutoLogoutTime = int.Parse(ReadINIValue("MainX6 Control", "AutoLogoutTime", AutoLogoutTime.ToString(), INIFILE));
            IsOpenForceNoCheckRepeat = ReadINIValue("MainX6 Control", "IsOpenForceNoCheckRepeat", (IsOpenForceNoCheckRepeat ? "1" : "0"), INIFILE) == "1";
            IsOpenShowGrade = ReadINIValue("MainX6 Control", "IsOpenShowGrade", (IsOpenShowGrade ? "1" : "0"), INIFILE) == "1";

            IsUseTcpStart = ReadINIValue("MainX6 Control", "IsUseTcpStart", (IsUseTcpStart ? "1" : "0"), INIFILE) == "1";

            IsOpenFaultToleranceRate = ReadINIValue("MainX6 Control", "IsOpenFaultToleranceRate", (IsOpenFaultToleranceRate ? "1" : "0"), INIFILE) == "1";
            FaultToleranceRate = double.Parse(ReadINIValue("MainX6 Control", "FaultToleranceRate", FaultToleranceRate.ToString(), INIFILE));

            RootPath = ReadINIValue("MainX6 Control", "RootPath", "D:\\", INIFILE);
            //DeviceName = ReadINIValue("MainX6 Control", "DeviceName", "None", INIFILE);

            CamLinescanStartPos = float.Parse(ReadINIValue("MainX6 Control", "CamLinescanStartPos", CamLinescanStartPos.ToString(), INIFILE));
            CamLinescanEndPos = float.Parse(ReadINIValue("MainX6 Control", "CamLinescanEndPos", CamLinescanEndPos.ToString(), INIFILE));
            CamLinescanSpeed = int.Parse(ReadINIValue("MainX6 Control", "CamLinescanSpeed", CamLinescanSpeed.ToString(), INIFILE));
            CamAreaMatchPos = float.Parse(ReadINIValue("MainX6 Control", "CamAreaMatchPos", CamAreaMatchPos.ToString(), INIFILE));

            TestImageOvertime = int.Parse(ReadINIValue("MainX6 Control", "TestImageOvertime", TestImageOvertime.ToString(), INIFILE));
            TestResultOvertime = int.Parse(ReadINIValue("MainX6 Control", "TestResultOvertime", TestResultOvertime.ToString(), INIFILE));

            handle_delaytime = int.Parse(ReadINIValue("MainX6 Control", "handle_delaytime", handle_delaytime.ToString(), INIFILE));

            JCET_IS_USE_SHOPFLOOR = ReadINIValue("MainX6_JCET Control", "JCET_IS_USE_SHOPFLOOR", (JCET_IS_USE_SHOPFLOOR ? "1" : "0"), INIFILE) == "1";
            JCET_STRIP_BUFF = int.Parse(ReadINIValue("MainX6_JCET Control", "JCET_STRIP_BUFF", JCET_STRIP_BUFF.ToString(), INIFILE));
            JCET_WEBSERVICE_URL = ReadINIValueOLD("MainX6_JCET Control", "JCET_WEBSERVICE_URL", JCET_WEBSERVICE_URL, INIFILE);
            JCET_TIMESTOP_SET = int.Parse(ReadINIValue("MainX6_JCET Control", "JCET_TIMESTOP_SET", JCET_TIMESTOP_SET.ToString(), INIFILE));

            LaserNGConut = int.Parse(ReadINIValue("Statistics", "LASERNG", "0", INIFILE));
            KeyCapNGConut = int.Parse(ReadINIValue("Statistics", "KEYCAPNG", "0", INIFILE));
            ScrewNGConut = int.Parse(ReadINIValue("Statistics", "SCREWNG", "0", INIFILE));
            AllConut = int.Parse(ReadINIValue("Statistics", "ALLCOUNT", "0", INIFILE));
            PassConut = int.Parse(ReadINIValue("Statistics", "PASSCOUNT", "0", INIFILE));
            NGConut = int.Parse(ReadINIValue("Statistics", "NGCOUNT", "0", INIFILE));

            LKNGConut = int.Parse(ReadINIValue("Statistics", "LKNGCOUNT", "0", INIFILE));

            LSNGConut = int.Parse(ReadINIValue("Statistics", "LSNGCOUNT", "0", INIFILE));
            KSNGConut = int.Parse(ReadINIValue("Statistics", "KSNGCOUNT", "0", INIFILE));

            ALLNGConut = int.Parse(ReadINIValue("Statistics", "ALLNGCOUNT", "0", INIFILE));


            fR3RCompensate = float.Parse(ReadINIValue("R3 Compensate", "RCOMPEN", "0", INIFILE));
            fR3LCompensate = float.Parse(ReadINIValue("R3 Compensate", "LCOMPEN", "0", INIFILE));
            fR3DCompensate = float.Parse(ReadINIValue("R3 Compensate", "DCOMPEN", "0", INIFILE));

            fC3_A = float.Parse(ReadINIValue("C3 Compensate", "A", "-1", INIFILE));
            if (fC3_A == -1)
            {
                WriteINIValue("C3 Compensate", "A", "0", INIFILE);
                fC3_A = 0;
            }
            fC3_B = float.Parse(ReadINIValue("C3 Compensate", "B", "-1", INIFILE));
            if (fC3_B == -1)
            {
                WriteINIValue("C3 Compensate", "B", "0", INIFILE);
                fC3_B = 0;
            }
            fC3_C = float.Parse(ReadINIValue("C3 Compensate", "C", "-1", INIFILE));
            if (fC3_C == -1)
            {
                WriteINIValue("C3 Compensate", "C", "0", INIFILE);
                fC3_C = 0;
            }
            fC3_D = float.Parse(ReadINIValue("C3 Compensate", "D", "-1", INIFILE));
            if (fC3_D == -1)
            {
                WriteINIValue("C3 Compensate", "D", "0", INIFILE);
                fC3_D = 0;
            }
            fC3_E = float.Parse(ReadINIValue("C3 Compensate", "E", "-1", INIFILE));
            if (fC3_E == -1)
            {
                WriteINIValue("C3 Compensate", "E", "0", INIFILE);
                fC3_E = 0;
            }
            fC3_F = float.Parse(ReadINIValue("C3 Compensate", "F", "-1", INIFILE));
            if (fC3_F == -1)
            {
                WriteINIValue("C3 Compensate", "F", "0", INIFILE);
                fC3_F = 0;
            }
            fC3_G = float.Parse(ReadINIValue("C3 Compensate", "G", "-1", INIFILE));
            if (fC3_G == -1)
            {
                WriteINIValue("C3 Compensate", "G", "0", INIFILE);
                fC3_G = 0;
            }
            fC3_H = float.Parse(ReadINIValue("C3 Compensate", "H", "-1", INIFILE));
            if (fC3_H == -1)
            {
                WriteINIValue("C3 Compensate", "H", "0", INIFILE);
                fC3_H = 0;
            }

            LoadDataRecord();
        }

        public static void Save()
        {
            //Write [Basic Control] Parameters

            WriteINIValue("Basic Control", INIEnum.MACHINENAME.ToString(), MACHINENAME, INIFILE);
            WriteINIValue("Basic Control", INIEnum.DELAYTIME.ToString(), DELAYTIME.ToString(), INIFILE);
            WriteINIValue("Basic Control", INIEnum.LANGUAGE.ToString(), LANGUAGE.ToString(), INIFILE);
            WriteINIValue("Basic Control", INIEnum.SHOPFLOORPATH.ToString(), SHOPFLOORPATH, INIFILE);
            WriteINIValue("Basic Control", INIEnum.ISSAVEWITHTIMESTAMP.ToString(), (ISSAVEWITHTIMESTAMP ? "1" : "0"), INIFILE);
            WriteINIValue("Basic Control", INIEnum.RETESTTIME.ToString(), RETESTTIME.ToString(), INIFILE);

            WriteINIValue("Basic Control", "FactoryNameIndex", FactoryNameIndex.ToString(), INIFILE);
            WriteINIValue("Basic Control", "bUse2DCNNReader", (bUse2DCNNReader ? "1" : "0"), INIFILE);

            WriteINIValue("Basic Control", INIEnum.CHECKSNERRORCODE.ToString(), CHECKSNERRORCODE, INIFILE);
            WriteINIValue("Basic Control", "chipSaveImageFormat", ((int)chipSaveImageFormat).ToString(), INIFILE);

            WriteINIValue("Basic Control", "chipPassColor", chipPassColor.ToArgb().ToString(), INIFILE);

            //Write [DFLY Control] Parameters

            WriteINIValue("DFLY Control", INIEnum.CAMERALOCATION.ToString(), CAMERALOCATION, INIFILE);
            WriteINIValue("DFLY Control", INIEnum.LENSLOCATION.ToString(), LENSLOCATION, INIFILE);

            //Write [Allinone Server Control] Parameters

            WriteINIValue("Allinone Server Control", INIEnum.KEYCAPEXPOSURE.ToString(), KEYCAPEXPOSURE, INIFILE);
            WriteINIValue("Allinone Server Control", INIEnum.FRAMEEXPOSURE.ToString(), FRAMEEXPOSURE, INIFILE);

            //Write [Allinone Server Control] Parameters
            WriteINIValue("Hiveclient Control", INIEnum.ISHIVECLIENT.ToString(), (ISHIVECLIENT ? "1" : "0"), INIFILE);
            WriteINIValue("Hiveclient Control", INIEnum.HIVE_publisher_id.ToString(), HIVE_publisher_id, INIFILE);
            WriteINIValue("Hiveclient Control", INIEnum.HIVE_site.ToString(), HIVE_site, INIFILE);
            WriteINIValue("Hiveclient Control", INIEnum.HIVE_building.ToString(), HIVE_building, INIFILE);
            WriteINIValue("Hiveclient Control", INIEnum.HIVE_line_type.ToString(), HIVE_line_type, INIFILE);
            WriteINIValue("Hiveclient Control", INIEnum.HIVE_line.ToString(), HIVE_line, INIFILE);
            WriteINIValue("Hiveclient Control", INIEnum.HIVE_station_type.ToString(), HIVE_station_type, INIFILE);
            WriteINIValue("Hiveclient Control", INIEnum.HIVE_station_instance.ToString(), HIVE_station_instance, INIFILE);
            WriteINIValue("Hiveclient Control", INIEnum.HIVE_vendor.ToString(), HIVE_vendor, INIFILE);
            WriteINIValue("Hiveclient Control", INIEnum.HIVE_exe_path.ToString(), HIVE_exe_path, INIFILE);

            WriteINIValue("Hiveclient Control", "HIVE_model", HIVE_model, INIFILE);
            WriteINIValue("Hiveclient Control", "HIVE_islocalsystemupload", (HIVE_islocalsystemupload ? "1" : "0"), INIFILE);
            WriteINIValue("Hiveclient Control", "HIVE_rectangle_corp", RectToStringSimple(HIVE_rectangle_corp), INIFILE);


            //Write [QFactory Control] Parameters
            WriteINIValue("QFactory Control", "ISUSE_QFACTORY", (ISUSE_QFACTORY ? "1" : "0"), INIFILE);
            WriteINIValue("QFactory Control", "QFACTORY_CHECK_TIME", QFACTORY_CHECK_TIME.ToString(), INIFILE);
            WriteINIValue("QFactory Control", "QFACTORY_EQ_SN", QFACTORY_EQ_SN, INIFILE);
            WriteINIValue("QFactory Control", "QFACTORY_EQ_LocationID", QFACTORY_EQ_LocationID, INIFILE);
            WriteINIValue("QFactory Control", "QFACTORY_EQ_LocationID2", QFACTORY_EQ_LocationID2, INIFILE);
            WriteINIValue("QFactory Control", "QFACTORY_Station", QFACTORY_Station, INIFILE);
            WriteINIValue("QFactory Control", "QFACTORY_Step", QFACTORY_Step, INIFILE);

            WriteINIValue("Data Collection Control", INIEnum.DATA_Program.ToString(), DATA_Program, INIFILE);
            WriteINIValue("Data Collection Control", INIEnum.DATA_Building_Config.ToString(), DATA_Building_Config, INIFILE);
            WriteINIValue("Data Collection Control", INIEnum.DATA_FIXTUREID.ToString(), DATA_FIXTUREID, INIFILE);
            WriteINIValue("Data Collection Control", "DATA_SCREW_TEN", (DATA_SCREW_TEN ? "1" : "0"), INIFILE);


            WriteINIValue("MainSD Control", "USER_SET_FULL_PASSCOUNT", USER_SET_FULL_PASSCOUNT.ToString(), INIFILE);
            WriteINIValue("MainSD Control", "USER_SET_FULL_NGCOUNT", USER_SET_FULL_NGCOUNT.ToString(), INIFILE);
            WriteINIValue("MainSD Control", "CONTINUE_NG_COUNT", CONTINUE_NG_COUNT.ToString(), INIFILE);
            WriteINIValue("MainSD Control", "MAINSD_GETIMAGE_DELAYTIME", MAINSD_GETIMAGE_DELAYTIME.ToString(), INIFILE);
            WriteINIValue("MainSD Control", "MAINSD_PAD_MIL_RESOLUTION", MAINSD_PAD_MIL_RESOLUTION.ToString(), INIFILE);
            WriteINIValue("MainSD Control", "MAINSDM1_GETSTART_DELAYTIME", MAINSDM1_GETSTART_DELAYTIME.ToString(), INIFILE);
            WriteINIValue("MainSD Control", "CHIP_NG_SHOW", (CHIP_NG_SHOW ? "1" : "0"), INIFILE);
            WriteINIValue("MainSD Control", "CHIP_NG_collect", (CHIP_NG_collect ? "1" : "0"), INIFILE);
            WriteINIValue("MainSD Control", "CHIP_force_pass", (CHIP_force_pass ? "1" : "0"), INIFILE);
            WriteINIValue("MainSD Control", "CHIP_forceALIGNRUN_pass", (CHIP_forceALIGNRUN_pass ? "1" : "0"), INIFILE);
            WriteINIValue("MainSD Control", "IsDEBUGCHIP", (IsDEBUGCHIP ? "1" : "0"), INIFILE);
            WriteINIValue("MainSD Control", "CHIP_CAL_MODE", CHIP_CAL_MODE.ToString(), INIFILE);
            WriteINIValue("MainSD Control", "CHIP_ISSMOOTHEN", (CHIP_ISSMOOTHEN ? "1" : "0"), INIFILE);

            WriteINIValue("MainSD Control", "IsOpenCheckSensor", (IsOpenCheckSensor ? "1" : "0"), INIFILE);
            WriteINIValue("MainSD Control", "IsOpenCheckSensor2", (IsOpenCheckSensor2 ? "1" : "0"), INIFILE);
            WriteINIValue("Basic Control", "IsOpenRecipeDataRecord", (IsOpenRecipeDataRecord ? "1" : "0"), INIFILE);
            //WriteINIValue("Basic Control", "chipTestAllCount", chipTestAllCount.ToString(), INIFILE);
            //WriteINIValue("Basic Control", "chipTestPassCount", chipTestPassCount.ToString(), INIFILE);
            //WriteINIValue("Basic Control", "chipTestFailCount", chipTestFailCount.ToString(), INIFILE);
            //WriteINIValue("Basic Control", "chipTestNoChipCount", chipTestNoChipCount.ToString(), INIFILE);
            //WriteINIValue("Basic Control", "chipN1Count", chipN1Count.ToString(), INIFILE);
            //WriteINIValue("Basic Control", "chipN2Count", chipN2Count.ToString(), INIFILE);
            //WriteINIValue("Basic Control", "chipN3Count", chipN3Count.ToString(), INIFILE);
            //WriteINIValue("Basic Control", "chipN4Count", chipN4Count.ToString(), INIFILE);
            //WriteINIValue("Basic Control", "chipN5Count", chipN5Count.ToString(), INIFILE);

            WriteINIValue("Basic Control", "xClearDataTime1", xClearDataTime1.ToString(), INIFILE);
            WriteINIValue("Basic Control", "xClearDataTime2", xClearDataTime2.ToString(), INIFILE);

            WriteINIValue("Basic Control", "chipUseAI", (chipUseAI ? "1" : "0"), INIFILE);

            WriteINIValue("MainSD Control", "AXIS_X_JJS", AXIS_X_JJS.ToString(), INIFILE);
            WriteINIValue("MainSD Control", "AXIS_Y_JJS", AXIS_Y_JJS.ToString(), INIFILE);
            WriteINIValue("MainSD Control", "AXIS_Z_JJS", AXIS_Z_JJS.ToString(), INIFILE);

            WriteINIValue("MainSD Control", "AXIS_MANUAL_JJS_ADD", AXIS_MANUAL_JJS_ADD.ToString(), INIFILE);
            WriteINIValue("MainSD Control", "AXIS_MANUAL_JJS_SUB", AXIS_MANUAL_JJS_SUB.ToString(), INIFILE);
            WriteINIValue("MainSD Control", "AXIS_AUTO_JJS_ADD", AXIS_AUTO_JJS_ADD.ToString(), INIFILE);
            WriteINIValue("MainSD Control", "AXIS_AUTO_JJS_SUB", AXIS_AUTO_JJS_SUB.ToString(), INIFILE);
            WriteINIValue("MainSD Control", "RobotSpeedValue", RobotSpeedValue.ToString(), INIFILE);

            WriteINIValue("MainSD Control", "ChkArea", ChkArea.ToString(), INIFILE);
            WriteINIValue("MainSD Control", "ChkWidth", ChkWidth.ToString(), INIFILE);
            WriteINIValue("MainSD Control", "ChkHeight", ChkHeight.ToString(), INIFILE);

            //[MainX6 Control]
            WriteINIValue("MainX6 Control", "CHANGE_FILE_PATH", CHANGE_FILE_PATH.ToString(), INIFILE);
            WriteINIValue("MainX6 Control", "tcp_ip", tcp_ip.ToString(), INIFILE);
            WriteINIValue("MainX6 Control", "tcp_port", tcp_port.ToString(), INIFILE);
            WriteINIValue("MainX6 Control", "tcp_handle_open", (tcp_handle_open ? "1" : "0"), INIFILE);
            WriteINIValue("MainX6 Control", "tcp_handle_ip", tcp_handle_ip.ToString(), INIFILE);
            WriteINIValue("MainX6 Control", "tcp_handle_port", tcp_handle_port.ToString(), INIFILE);

            WriteINIValue("MainX6 Control", "user_screen_width", user_screen_width.ToString(), INIFILE);
            WriteINIValue("MainX6 Control", "user_screen_height", user_screen_height.ToString(), INIFILE);
            WriteINIValue("MainX6 Control", "user_screen_scale", user_screen_scale.ToString("0.00"), INIFILE);
            WriteINIValue("MainX6 Control", "user_screen_bold", (user_screen_bold ? "1" : "0"), INIFILE);
            WriteINIValue("MainX6 Control", "IsReadHandlerOKSign", (IsReadHandlerOKSign ? "1" : "0"), INIFILE);
            WriteINIValue("MainX6 Control", "IsSendHandlerTcpOKSign", (IsSendHandlerTcpOKSign ? "1" : "0"), INIFILE);
            WriteINIValue("MainX6 Control", "IsNoUseHandlerOKSign", (IsNoUseHandlerOKSign ? "1" : "0"), INIFILE);
            WriteINIValue("MainX6 Control", "IsOpenBehindOKSign", (IsOpenBehindOKSign ? "1" : "0"), INIFILE);

            WriteINIValue("MainX6 Control", "IsOpenForceAllCheck", (IsOpenForceAllCheck ? "1" : "0"), INIFILE);
            WriteINIValue("MainX6 Control", "IsOpenCheckCurLotRepeatCode", (IsOpenCheckCurLotRepeatCode ? "1" : "0"), INIFILE);
            WriteINIValue("MainX6 Control", "IsOpenCheckRepeatCode", (IsOpenCheckRepeatCode ? "1" : "0"), INIFILE);
            WriteINIValue("MainX6 Control", "IsCollectErrorSmall", (IsCollectErrorSmall ? "1" : "0"), INIFILE);
            WriteINIValue("MainX6 Control", "IsLightAlwaysOn", (IsLightAlwaysOn ? "1" : "0"), INIFILE);
            WriteINIValue("MainX6 Control", "IsCollectPictures", (IsCollectPictures ? "1" : "0"), INIFILE);
            WriteINIValue("MainX6 Control", "IsSaveScreen", (IsSaveScreen ? "1" : "0"), INIFILE);
            //WriteINIValue("MainX6 Control", "IsCheckBarcodeOpen", (IsCheckBarcodeOpen ? "1" : "0"), INIFILE);
            WriteINIValue("MainX6 Control", "IsOnlyShowCurrentImage", (IsOnlyShowCurrentImage ? "1" : "0"), INIFILE);
            WriteINIValue("MainX6 Control", "IsCollectStripPictures", (IsCollectStripPictures ? "1" : "0"), INIFILE);
            WriteINIValue("MainX6 Control", "IsCollectPicturesSingle", (IsCollectPicturesSingle ? "1" : "0"), INIFILE);
            WriteINIValue("MainX6 Control", "IsOpenCip", (IsOpenCip ? "1" : "0"), INIFILE);
            WriteINIValue("MainX6 Control", "IsOpenUseFileMap", (IsOpenUseFileMap ? "1" : "0"), INIFILE);
            WriteINIValue("MainX6 Control", "FileMapPath", FileMapPath, INIFILE);
            WriteINIValue("MainX6 Control", "IsOpenQcRandom", (IsOpenQcRandom ? "1" : "0"), INIFILE);
            WriteINIValue("MainX6 Control", "IsOpenAutoChangeRecipe", (IsOpenAutoChangeRecipe ? "1" : "0"), INIFILE);
            WriteINIValue("MainX6 Control", "pMatchType", pMatchType.ToString(), INIFILE);
            WriteINIValue("MainX6 Control", "fTolerance", fTolerance.ToString(), INIFILE);
            WriteINIValue("MainX6 Control", "IsOpenForceNoCheckRepeat", (IsOpenForceNoCheckRepeat ? "1" : "0"), INIFILE);
            WriteINIValue("MainX6 Control", "IsOpenShowGrade", (IsOpenShowGrade ? "1" : "0"), INIFILE);
            WriteINIValue("MainX6 Control", "IsUseTcpStart", (IsUseTcpStart ? "1" : "0"), INIFILE);

            WriteINIValue("MainX6 Control", "IsOpenFaultToleranceRate", (IsOpenFaultToleranceRate ? "1" : "0"), INIFILE);
            WriteINIValue("MainX6 Control", "FaultToleranceRate", FaultToleranceRate.ToString(), INIFILE);

            WriteINIValue("MainX6 Control", "handle_delaytime", handle_delaytime.ToString(), INIFILE);
            WriteINIValue("MainX6 Control", "AutoLogoutTime", AutoLogoutTime.ToString(), INIFILE);

            WriteINIValue("MainX6 Control", "RootPath", RootPath.ToString(), INIFILE);
            //WriteINIValue("MainX6 Control", "DeviceName", DeviceName.ToString(), INIFILE);

            WriteINIValue("MainX6 Control", "CamLinescanStartPos", CamLinescanStartPos.ToString(), INIFILE);
            WriteINIValue("MainX6 Control", "CamLinescanEndPos", CamLinescanEndPos.ToString(), INIFILE);
            WriteINIValue("MainX6 Control", "CamLinescanSpeed", CamLinescanSpeed.ToString(), INIFILE);
            WriteINIValue("MainX6 Control", "CamAreaMatchPos", CamAreaMatchPos.ToString(), INIFILE);

            WriteINIValue("MainX6 Control", "TestImageOvertime", TestImageOvertime.ToString(), INIFILE);
            WriteINIValue("MainX6 Control", "TestResultOvertime", TestResultOvertime.ToString(), INIFILE);

            WriteINIValue("MainX6_JCET Control", "JCET_IS_USE_SHOPFLOOR", (JCET_IS_USE_SHOPFLOOR ? "1" : "0"), INIFILE);
            WriteINIValue("MainX6_JCET Control", "JCET_STRIP_BUFF", JCET_STRIP_BUFF.ToString(), INIFILE);
            WriteINIValue("MainX6_JCET Control", "JCET_WEBSERVICE_URL", JCET_WEBSERVICE_URL.ToString(), INIFILE);
            WriteINIValue("MainX6_JCET Control", "JCET_TIMESTOP_SET", JCET_TIMESTOP_SET.ToString(), INIFILE);

            GetLENSOFFSET();

            SaveDataRecord();
            SaveKeyRecord();
            SaveAi();

        }
        /// <summary>
        /// 保存是否保存OCR测试图 选项
        /// </summary>
        public static void SAVEOCRIMAGE()
        {
            WriteINIValue("R32 Control", INIEnum.ISSAVEOCRIMAGE.ToString(), (ISSAVEOCRIMAGE ? "1" : "0"), INIFILE);
        }
        public static void SaveSDM5Setup()
        {
            WriteINIValue("Basic Control", "SDM5FindCount", SDM5FindCount, INIFILE);
        }
        public static void SaveSDM2Setup()
        {
            WriteINIValue("MainSD Control", "CHIP_force_pass", (CHIP_force_pass ? "1" : "0"), INIFILE);
        }

        public static string DATA_ROOT
        {
            get
            {
                string iret = RootPath + $"DataRoot\\{DateTime.Now.ToString("yyyyMMdd")}";
                if (string.IsNullOrEmpty(RootPath))
                    iret = $"D:\\DataRoot\\{DateTime.Now.ToString("yyyyMMdd")}";

                //string iret = RootPath + $"DataRoot\\{DeviceName}\\{DateTime.Now.ToString("yyyyMMdd")}";
                //if (string.IsNullOrEmpty(RootPath))
                //    iret = $"D:\\DataRoot\\{DeviceName}\\{DateTime.Now.ToString("yyyyMMdd")}";
                return iret;
            }
        }

        public static string RootPath = "D:\\";
        //public static string DeviceName = "None";

        static string m_DataRecordPath // = "D:\\report\\DataRecord";
        {
            get
            {
                string iret = DATA_ROOT + $"\\{DataRecordName}\\DataRecord";
                if (!Directory.Exists(iret))
                    Directory.CreateDirectory(iret);
                return iret;
            }
        }
        public static string DataRecordName = "None";

        public static void LoadDataRecord()
        {
            WriteINIValue("Basic Control", "DataRecordName", DataRecordName, INIFILE);

            string _path = INIFILE;
            if (IsOpenRecipeDataRecord)
                _path = $"{m_DataRecordPath}\\{DataRecordName}.ini";

            chipTestAllCount = int.Parse(ReadINIValue("Basic Control", "chipTestAllCount", "0", _path));
            chipTestPassCount = int.Parse(ReadINIValue("Basic Control", "chipTestPassCount", "0", _path));
            chipTestFailCount = int.Parse(ReadINIValue("Basic Control", "chipTestFailCount", "0", _path));
            chipTestNoChipCount = int.Parse(ReadINIValue("Basic Control", "chipTestNoChipCount", "0", _path));
            chipN1Count = int.Parse(ReadINIValue("Basic Control", "chipN1Count", "0", _path));
            chipN2Count = int.Parse(ReadINIValue("Basic Control", "chipN2Count", "0", _path));
            chipN3Count = int.Parse(ReadINIValue("Basic Control", "chipN3Count", "0", _path));
            chipN4Count = int.Parse(ReadINIValue("Basic Control", "chipN4Count", "0", _path));
            chipN5Count = int.Parse(ReadINIValue("Basic Control", "chipN5Count", "0", _path));
        }
        public static void SaveDataRecord()
        {
            string _path = INIFILE;
            if (IsOpenRecipeDataRecord)
                _path = $"{m_DataRecordPath}\\{DataRecordName}.ini";

            WriteINIValue("Basic Control", "chipTestAllCount", chipTestAllCount.ToString(), _path);
            WriteINIValue("Basic Control", "chipTestPassCount", chipTestPassCount.ToString(), _path);
            WriteINIValue("Basic Control", "chipTestFailCount", chipTestFailCount.ToString(), _path);
            WriteINIValue("Basic Control", "chipTestNoChipCount", chipTestNoChipCount.ToString(), _path);
            WriteINIValue("Basic Control", "chipN1Count", chipN1Count.ToString(), _path);
            WriteINIValue("Basic Control", "chipN2Count", chipN2Count.ToString(), _path);
            WriteINIValue("Basic Control", "chipN3Count", chipN3Count.ToString(), _path);
            WriteINIValue("Basic Control", "chipN4Count", chipN4Count.ToString(), _path);
            WriteINIValue("Basic Control", "chipN5Count", chipN5Count.ToString(), _path);
        }

        public static void SaveKeyRecord()
        {
            WriteINIValue("Basic Control", "keyx", keyx.ToString(), INIFILE);
            WriteINIValue("Basic Control", "keyy", keyy.ToString(), INIFILE);
            WriteINIValue("Basic Control", "keyz", keyz.ToString(), INIFILE);
            WriteINIValue("Basic Control", "keyoffsetx", keyoffsetx.ToString(), INIFILE);
            WriteINIValue("Basic Control", "keyoffsety", keyoffsety.ToString(), INIFILE);
            WriteINIValue("Basic Control", "keyrow", keyrow.ToString(), INIFILE);
            WriteINIValue("Basic Control", "keycol", keycol.ToString(), INIFILE);
            WriteINIValue("Basic Control", "op_keyx", op_keyxyz.ToString(), INIFILE);
            WriteINIValue("Basic Control", "keyendx", keyendx.ToString(), INIFILE);
            WriteINIValue("Basic Control", "keyendy", keyendy.ToString(), INIFILE);
            WriteINIValue("Basic Control", "keyendz", keyendz.ToString(), INIFILE);
        }
        public static void SaveAi()
        {
            WriteINIValue("MainX6 Control", "AI_Model_FilenamePath", AI_Model_FilenamePath.ToString(), INIFILE);

        }

        public static void ResetDataResult()
        {
            chipTestAllCount = 0;
            chipTestPassCount = 0;
            chipTestFailCount = 0;
            chipTestNoChipCount = 0;
            chipN1Count = 0;
            chipN2Count = 0;
            chipN3Count = 0;
            chipN4Count = 0;
            chipN5Count = 0;
        }
        public static string GetDataResultString()
        {
            int passcount = chipTestPassCount + chipN5Count;
            chipTestFailCount = chipN1Count + chipN2Count + chipN3Count + chipN4Count;
            chipTestAllCount = passcount + chipTestFailCount;
            string str = string.Empty;

            switch (Universal.OPTION)
            {
                case OptionEnum.MAIN_SDM2:

                    str += "总颗数：" + chipTestAllCount.ToString() + " pcs" + Environment.NewLine;
                    str += "Pass颗数：" + passcount.ToString() + " pcs" + "(" + _getPercent(chipTestAllCount, passcount) + ")" + Environment.NewLine;
                    str += "Fail颗数：" + chipTestFailCount.ToString() + " pcs" + "(" + _getPercent(chipTestAllCount, chipTestFailCount) + ")" + Environment.NewLine;
                    //str += "无芯片颗数：" + chipTestNoChipCount.ToString() +" pcs"+ Environment.NewLine;
                    str += "无胶颗数：" + chipN1Count.ToString() + " pcs" + "(" + _getPercent(chipTestAllCount, chipN1Count) + ")" + Environment.NewLine;
                    str += "尺寸错误颗数：" + chipN2Count.ToString() + " pcs" + "(" + _getPercent(chipTestAllCount, chipN2Count) + ")" + Environment.NewLine;
                    str += "晶片溢胶颗数：" + chipN3Count.ToString() + " pcs" + "(" + _getPercent(chipTestAllCount, chipN3Count) + ")" + Environment.NewLine;
                    str += "胶水宽度异常颗数：" + chipN4Count.ToString() + " pcs" + "(" + _getPercent(chipTestAllCount, chipN4Count) + ")" + Environment.NewLine;
                    str += "无芯片颗数：" + chipN5Count.ToString() + " pcs" + "(" + _getPercent(chipTestAllCount, chipN5Count) + ")" + Environment.NewLine;


                    break;
                case OptionEnum.MAIN_SDM3:

                    str += "总颗数：" + chipTestAllCount.ToString() + " pcs" + Environment.NewLine;
                    str += "Pass颗数：" + passcount.ToString() + " pcs" + "(" + _getPercent(chipTestAllCount, passcount) + ")" + Environment.NewLine;
                    str += "Fail颗数：" + chipTestFailCount.ToString() + " pcs" + "(" + _getPercent(chipTestAllCount, chipTestFailCount) + ")" + Environment.NewLine;
                    //str += "无芯片颗数：" + chipTestNoChipCount.ToString() +" pcs"+ Environment.NewLine;
                    str += "定位错误颗数：" + chipN1Count.ToString() + " pcs" + "(" + _getPercent(chipTestAllCount, chipN1Count) + ")" + Environment.NewLine;
                    str += "检测错误颗数：" + chipN2Count.ToString() + " pcs" + "(" + _getPercent(chipTestAllCount, chipN2Count) + ")" + Environment.NewLine;
                    str += "量测错误颗数：" + chipN3Count.ToString() + " pcs" + "(" + _getPercent(chipTestAllCount, chipN3Count) + ")" + Environment.NewLine;
                    str += "表面边角缺陷颗数：" + chipN4Count.ToString() + " pcs" + "(" + _getPercent(chipTestAllCount, chipN4Count) + ")" + Environment.NewLine;


                    break;
            }

            //str += "总颗数：" + Environment.NewLine + chipTestAllCount.ToString() + Environment.NewLine;
            //str += "Pass颗数：" + Environment.NewLine + chipTestPassCount.ToString() + Environment.NewLine;
            //str += "Fail颗数：" + Environment.NewLine + chipTestFailCount.ToString() + Environment.NewLine;
            ////str += "Chip无芯片颗数："+ Environment.NewLine + chipTestNoChipCount.ToString() + Environment.NewLine;
            //str += "无胶颗数：" + Environment.NewLine + chipN1Count.ToString() + Environment.NewLine;
            //str += "尺寸错误颗数：" + Environment.NewLine + chipN2Count.ToString() + Environment.NewLine;
            //str += "晶片溢胶颗数：" + Environment.NewLine + chipN3Count.ToString() + Environment.NewLine;
            //str += "胶水宽度异常颗数：" + Environment.NewLine + chipN4Count.ToString() + Environment.NewLine;
            //str += "无芯片颗数：" + Environment.NewLine + chipN5Count.ToString() + Environment.NewLine;
            SaveDataRecord();
            return str;
        }
        static string _getPercent(int iAllCount, int iCount)
        {
            if (iAllCount == 0)
                return "0.00%";
            double per = iCount * 1.0 / iAllCount * 100;
            return per.ToString("0.00") + "%";
        }

        static void GetLENSOFFSET()
        {
            string[] camerastrs = CAMERALOCATION.Split(',');
            string[] lensstrs = LENSLOCATION.Split(',');

            LENSOFFSET[0] = float.Parse(lensstrs[0]) - float.Parse(camerastrs[0]);
            LENSOFFSET[1] = float.Parse(lensstrs[1]) - float.Parse(camerastrs[1]);
            LENSOFFSET[2] = float.Parse(lensstrs[2]) - float.Parse(camerastrs[2]);
        }

        static string RectToStringSimple(Rectangle Rect)
        {
            return Rect.X.ToString() + "," + Rect.Y.ToString() + "," + Rect.Width.ToString() + "," + Rect.Height.ToString();
        }
        static Rectangle SimpleStringToRect(string Str)
        {
            string[] strs = Str.Split(',');
            Rectangle rectF = new Rectangle();

            rectF.X = int.Parse(strs[0]);
            rectF.Y = int.Parse(strs[1]);
            rectF.Width = int.Parse(strs[2]);
            rectF.Height = int.Parse(strs[3]);

            return rectF;


        }


        static int LaserNGConut = 0;
        static int KeyCapNGConut = 0;
        static int ScrewNGConut = 0;

        static int AllConut = 0;
        static int PassConut = 0;
        static int NGConut = 0;

        static int LKNGConut = 0;
        static int LSNGConut = 0;
        static int KSNGConut = 0;

        static int ALLNGConut = 0;

        static bool LaserNgAddBC;

        /// <summary>
        /// 雷雕不良
        /// </summary>
        public static bool isLaserNgAddBC
        {
            get
            {
                return LaserNgAddBC;
            }
            set
            {
                LaserNgAddBC = value;
                WriteINIValue("Basic Control", INIEnum.LASERNGADDBC.ToString(), LaserNgAddBC ? "1" : "0", INIFILE);
            }
        }

        static int myBCNGCOUNT = 0;
        /// <summary>
        /// 雷雕不良数量
        /// </summary>
        public static int BCNGCOUNT
        {
            get
            {
                return myBCNGCOUNT;
            }
            set
            {
                myBCNGCOUNT = value;
                WriteINIValue("Basic Control", INIEnum.BCNGCOUNT.ToString(), myBCNGCOUNT.ToString(), INIFILE);
            }
        }
        static int myALLCOUNT = 0;//当天生产的数量
        /// <summary>
        /// 当日总测试量
        /// </summary>
        public static int ALLCOUNT
        {
            get
            {
                return myALLCOUNT;
            }
            set
            {
                myALLCOUNT = value;
                WriteINIValue("Basic Control", INIEnum.ALLCOUNT.ToString(), myALLCOUNT.ToString(), INIFILE);
            }
        }
        static DateTime myDATATIMERNOW; //当天生产的日期
        /// <summary>
        /// 当日日期
        /// </summary>
        public static DateTime DATATIMERNOW
        {
            get
            {
                return myDATATIMERNOW;
            }
            set
            {
                myDATATIMERNOW = value;
                WriteINIValue("Basic Control", INIEnum.DATATIMERNOW.ToString(), myDATATIMERNOW.ToString(), INIFILE);
            }
        }

        public static void SETBCCOUNT(bool isBCNG)
        {
            ALLCOUNT += 1;
            if (isBCNG)
                BCNGCOUNT += 1;

            CheckBCData();
        }
        /// <summary>
        /// 检查BC日期，如果过期就更新，同时更新BC的数量
        /// </summary>
        /// <returns></returns>
        public static bool CheckBCData()
        {
            bool isok = true;
            if (DATATIMERNOW.Year != DateTime.Now.Year)
                isok = false;
            if (DATATIMERNOW.Month != DateTime.Now.Month)
                isok = false;
            if (DATATIMERNOW.Day != DateTime.Now.Day)
                isok = false;

            if (!isok)
            {
                ALLCOUNT = 0;
                BCNGCOUNT = 0;

                DATATIMERNOW = DateTime.Now;

                return true;
            }

            return false;
        }


        /// <summary>
        /// 雷雕不良
        /// </summary>
        public static int iLASERNG
        {
            get
            {
                return LaserNGConut;
            }
            set
            {
                LaserNGConut = value;
                WriteINIValue("Statistics", "LASERNG", LaserNGConut.ToString(), INIFILE);
            }
        }
        /// <summary>
        /// 键帽不良
        /// </summary>
        public static int iKEYCAPNG
        {
            get
            {
                return KeyCapNGConut;
            }
            set
            {
                KeyCapNGConut = value;
                WriteINIValue("Statistics", "KEYCAPNG", KeyCapNGConut.ToString(), INIFILE);
            }
        }
        /// <summary>
        /// 螺丝不良
        /// </summary>
        public static int iSCREWNG
        {
            get
            {
                return ScrewNGConut;
            }
            set
            {
                ScrewNGConut = value;
                WriteINIValue("Statistics", "SCREWNG", ScrewNGConut.ToString(), INIFILE);
            }
        }
        /// <summary>
        /// 总数
        /// </summary>
        public static int iALLCOUNT
        {
            get
            {
                return AllConut;
            }
            set
            {
                AllConut = value;
                WriteINIValue("Statistics", "ALLCOUNT", AllConut.ToString(), INIFILE);
            }
        }
        /// <summary>
        /// PASS数
        /// </summary>
        public static int iPASSCOUNT
        {
            get
            {
                return PassConut;
            }
            set
            {
                PassConut = value;
                WriteINIValue("Statistics", "PASSCOUNT", PassConut.ToString(), INIFILE);
            }
        }

        /// <summary>
        /// NG数
        /// </summary>
        public static int iNGCOUNT
        {
            get
            {
                return NGConut;
            }
            set
            {
                NGConut = value;
                WriteINIValue("Statistics", "NGCOUNT", NGConut.ToString(), INIFILE);
            }
        }
        /// <summary>
        /// 镭雕键盘NG
        /// </summary>
        public static int iLKNGCOUNT
        {
            get
            {
                return LKNGConut;
            }
            set
            {
                LKNGConut = value;
                WriteINIValue("Statistics", "LKNGCOUNT", LKNGConut.ToString(), INIFILE);
            }
        }
        /// <summary>
        /// 镭雕螺丝NG
        /// </summary>
        public static int iLSNGCOUNT
        {
            get
            {
                return LSNGConut;
            }
            set
            {
                LSNGConut = value;
                WriteINIValue("Statistics", "LSNGCOUNT", LSNGConut.ToString(), INIFILE);
            }
        }
        /// <summary>
        /// 键盘螺丝NG
        /// </summary>
        public static int iKSNGCOUNT
        {
            get
            {
                return KSNGConut;
            }
            set
            {
                KSNGConut = value;
                WriteINIValue("Statistics", "KSNGCOUNT", KSNGConut.ToString(), INIFILE);
            }
        }

        /// <summary>
        /// 全部NG
        /// </summary>
        public static int iALLNGCOUNT
        {
            get
            {
                return ALLNGConut;
            }
            set
            {
                ALLNGConut = value;
                WriteINIValue("Statistics", "ALLNGCOUNT", ALLNGConut.ToString(), INIFILE);
            }
        }


        public static void ReCodeRepoer(RecodeRepoer recode)
        {
            if (recode.isKeycapNG && recode.isLaserNG && recode.isScrewNG)
            {
                iALLNGCOUNT++;
            }
            else if (recode.isKeycapNG && recode.isLaserNG)
            {
                iLKNGCOUNT++;
            }
            else if (recode.isKeycapNG && recode.isScrewNG)
            {
                iKSNGCOUNT++;
            }
            else if (recode.isLaserNG && recode.isScrewNG)
            {
                iLSNGCOUNT++;
            }
            else
            {
                if (recode.isKeycapNG)
                    iKEYCAPNG++;
                if (recode.isLaserNG)
                    iLASERNG++;
                if (recode.isScrewNG)
                    iSCREWNG++;
            }



            //if (recode.isSNNG)
            //    iSNNGCOUNT++;

            if (!recode.isKeycapNG && !recode.isLaserNG && !recode.isScrewNG)
                iPASSCOUNT++;
            else
                iNGCOUNT++;

            iALLCOUNT++;
        }

        public class RecodeRepoer
        {
            public bool isLaserNG = false;
            public bool isKeycapNG = false;
            public bool isScrewNG = false;
            //  public bool isSNNG = false;
        }
    }
}
