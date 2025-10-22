using Allinone.BasicSpace;
using Allinone.ControlSpace;
using Allinone.ControlSpace.MachineSpace;
using Allinone.Crystal.Net;
using Allinone.Crystal.Net.Common;
using Allinone.OPSpace;
using Allinone.OPSpace.ResultSpace;
using Allinone.ZGa.Mvc.Model.BarcodeModel;
using Allinone.ZGa.Mvc.Model.MapModel;
using Allinone.ZGa.Mvc.Model.MarkReferenceSystemModel;
using AllinOne.Jumbo.Net;
using AllinOne.Jumbo.Net.Common;
using EzSegClientLib;
using JetEazy;
using JetEazy.BasicSpace;
using JetEazy.CCDSpace.CamLinkDriver;
using JetEazy.ControlSpace;
using JetEazy.DBSpace;
using JetEazy.Interface;
using JzASN.OPSpace;
using JzKHC.OPSpace;
using JzMSR.OPSpace;
using JzOCR.OPSpace;
using JzScreenPoints.OPSpace;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Documents;
using System.Windows.Forms;

namespace Allinone
{
    public class Universal : JetEazy.Universal
    {
        public static bool IsNoUseCCD = false;
        public static bool IsNoUseIO = false;
        public static bool IsNoUseMotor = IsNoUseIO;

        public static string VersionDate = "2025/10/22";

        public static VersionEnum VERSION = VersionEnum.ALLINONE;
        public static OptionEnum OPTION = OptionEnum.MAIN_X6;

        public static CameraActionMode CAMACT = CameraActionMode.CAM_MOTOR_MODE2;
        public static RobotType myRobotType = RobotType.NONE;
        public static DiskType myDiskType = DiskType.DISK_D;
        public static JetMappingType jetMappingType = JetMappingType.NONE;
        public static FactoryName FACTORYNAME = FactoryName.NONE;

        #region 德龙激光

        #region 离线测试流程
        /*
         * 20240428
         * 1.第一步点击小算盘
         * 2.模拟界面发送触发信号
         */
        #endregion

        /// <summary>
        /// 连接DL的主程序Server
        /// </summary>
        public static bool m_UseCommToDLHandle = (OPTION == OptionEnum.MAIN_X6 && CAMACT != CameraActionMode.CAM_STATIC);

        public static string ShowMessage = "";
        public static bool IsJcetChangeRecipe = false;
        public static bool IsJcetTrainShow = false;
        public static bool IsTimeKeepCheck = false;
        public static JzTimes TimeCheckStop = new JzTimes();
        public static string StripVersionName = "";
        /// <summary>
        /// 开启星科获取mapping功能
        /// </summary>
        public static bool IsOpenJectCipMapping = false;

        ///// <summary>
        ///// 用于德龙读取cip通讯 获取mapping数据
        ///// </summary>
        //public static bool IsUseCip = true;

        /// <summary>
        /// 显示mapping ui
        /// </summary>
        public static bool IsUseMappingUI =
            ((OPTION == OptionEnum.MAIN_X6 || OPTION == OptionEnum.MAIN_SERVICE) && CAMACT != CameraActionMode.CAM_STATIC);
        public static bool IsRunningTest = false;//是否在流程中

        /// <summary>
        /// 德龙和矽品研磨使用
        /// </summary>
        public static bool IsUseThreadReviceTcp = true;
        public static float SetupDefaultOffsetValue
        {
            get
            {
                float ret = 0f;

                switch (OPTION)
                {
                    case OptionEnum.MAIN_X6:

                        //ret = 0.3f;

                        break;
                }

                return ret;
            }
        }
        public static float SetupDefaultResolutionValue
        {
            get
            {
                float ret = 0f;

                switch (OPTION)
                {
                    case OptionEnum.MAIN_X6:

                        switch (CAMACT)
                        {
                            case CameraActionMode.CAM_MOTOR_LINESCAN:
                                ret = 0.014f;
                                break;
                            default:
                                ret = 0.038f;
                                break;
                        }
                        break;
                }

                return ret;
            }
        }
        public static int TcpHandlerCurrentIndex = 0;
        #endregion

        public static Bitmap bmpProvideAI = new Bitmap(1, 1);
        //public static List<JzSliderItemClass> MapListTemp = new List<JzSliderItemClass>();

        /// <summary>
        /// 用485控制电机的画面
        /// </summary>
        public static MotoRs485.SetMotorForm mSetMotor;
        //Environment Variables
        public static int MAINTICK = 100;
        //更新主程序画面
        public static int DISPLAYTICK = 500;

        public static bool IsSaveRaw = false;
        public static bool IsMultiThreadUseToRun = true;

        /// <summary>
        /// 种子功能
        /// </summary>
        public static bool IsUseSeedFuntion = false;
        /// <summary>
        /// 使用本地图片
        /// </summary>
        public static bool IsLocalPicture = false;
        public static bool IsUseCalibration = false;

        public static UserOptionEnum PassOption = UserOptionEnum.ALL;

        public static string PROGRAMME_ROOT
        {
            get
            {
                string iret = @"D:";
                switch (OPTION)
                {
                    case OptionEnum.MAIN_X6:
                        switch (myDiskType)
                        {
                            case DiskType.DISK_C:
                                iret = @"C:";
                                break;
                            case DiskType.DISK_D:
                                iret = @"D:";
                                break;
                        }
                        break;
                }
                return iret;
            }
        }

        public static string CODEPATH = $@"{PROGRAMME_ROOT}\AUTOMATION\Eazy AOI DX";
        public static string VEROPT = VERSION.ToString() + "-" + OPTION.ToString();
        public static string MAINPATH = $@"{PROGRAMME_ROOT}\JETEAZY\" + VEROPT;
        public static string WORKPATH = $@"{PROGRAMME_ROOT}\JETEAZY\" + VEROPT + @"\WORK";

        public static string DBPATH = MAINPATH + @"\DB";
        public static string RCPPATH = MAINPATH + @"\PIC";
        public static string UIPATH = CODEPATH + @"\" + VERSION.ToString() + "UI";

        public static string DATAREPORTPATH = @"D:\DATAREPORT";
        public static string LOGDBPATH = @"D:\JETEAZY\" + VEROPT + @"\LOGDB";
        public static string BACKUPDBPATH = @"D:\JETEAZY\" + VEROPT + @"\BACKUPDB";
        public static string LOGTXTPATH = @"D:\JETEAZY\" + VEROPT + @"\LOGTXT";
        public static string PATTERNPATH = @"D:\JETEAZY\" + VEROPT + @"\PATTERNS";

        public static string DEBUGRAWPATH = @"D:\JETEAZY\" + VEROPT + @"\ORG";              //偵錯儲存的原圖位置
        public static string DEBUGRESULTPATH = @"D:\JETEAZY\" + VEROPT + @"\DEBUG";         //偵錯結果圖位置
        public static string TESTRESULTPATH = @"D:\COPYDATA";                               //偵錯結果圖位置
        public static string DEBUGSRCPATH = @"D:\JETEAZY\" + VEROPT + @"\SRCDEBUG";         //離線測試用的原圖位置
        public static string OCRIMAGEPATH = @"D:\LOA\OCR\";                                 //保存的OCR测试图位置  
        public static string BarcodeIMAGEPATH = @"D:\LOA\Barcode\";                         //保存的OCR测试图位置  
        /// <summary>
        /// 跑线时读到SN.txt里的东西
        /// </summary>
        public static string DATASNTXT = "";
        public static string RELATECOLORSTR = "";
        public static string SHOWBMPSTRING = "view.png";
        public static string PlayerPASSPATH = WORKPATH + @"\TADA.wav";
        public static string PlayerFAILPATH = WORKPATH + @"\RoutingNG.wav";
        public static string PlayerOPPWRATPATH = WORKPATH + @"\OPPWRAP.wav";
        public static string RunDebugOrRelease = "";
        public static string FAILBARCODE = "";


        public static string MainX6_Path = "D:\\CollectPictures\\Inspection\\";
        public static string MainSDM2NG_Path = "D:\\CollectPictures\\InspectionNG\\";
        public static string MainX6_Picture_Path = "D:\\CollectPictures\\Inspection_Single\\";

        public static string No80001Err = "";
        static JzLanguageClass myLanguage = new JzLanguageClass();
        static JzToolsClass JzTools = new JzToolsClass();
        public static JzShareMemoryClass Memory = new JzShareMemoryClass();

        static string DATACNNSTRING = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + DBPATH + @"\DATA.mdb;Jet OLEDB:Database Password=12892414;";

        public static AccDBClass ACCDB;
        public static EsssDBClass ESSDB;
        public static RcpDBClass RCPDB;
        //public static RUNDBClass RUNDB;

        public static MachineCollectionClass MACHINECollection;
        public static CCDCollectionClass CCDCollection;
        public static IxLineScanCam IxLineScan = null;
        public static IxLineScanCam IxAreaCam = null;
        public static IxMapBuilder MapBuilder = null;
        public static int MapCellIndex = 0;
        public static IxCodeBuilder CodeBuilder = null;

        //public static UseIOClass USEIO;
        public static int ALBIndicator = -1;
        public static AlbumCollectionClass ALBCollection;

        public static OPSpace.AnalyzeSpace.CorrestClass Correct;
        public static AlbumClass ALBNow
        {
            get
            {
                return ALBCollection.AlbumNow;
            }
        }

        static int LanguageIndex = 0;
        public static string InitialErrorString = "";

        public static int PAGEOPTYPECOUNT = 0;
        public static int CalPageIndex = 0;
        public static string CalTestPath = @"D:\TESTCOLLECT";
        public static string CalTestBarcode = String.Empty;

        public static OCRCollectionClass OCRCollection;
        public static MSRCollectionClass MSRCollection;
        public static ASNCollectionClass ASNCollection;
        public static JzScreenPointsClass SCREENPOINTS;
        public static KHCClass KHCCollection;
        /// <summary>
        /// 連接Apple Hiveclient
        /// </summary>
        public static JzHiveClass JZHIVECLIENT;
        public static JzQFactoryClass JZQFACTORY;
        public static JzMainSDPositionParaClass JZMAINSDPOSITIONPARA;
        public static OCRByPaddle.OCRByPaddle mOCRByPaddle;
        //public static Mvd2dCNNReader GvlMvd2D_CNNReader = null;

        public static ClientSocket X6_HANDLE_CLIENT = null;
        public static ClientSocket X6_LASER_CLIENT = null;
        public static Bitmap SDM2_BMP_SHOW_CURRENT = new Bitmap(1, 1);

        /// <summary>
        /// 連接至鍵高服務器
        /// </summary>
        public static IxConnectJumbo301 IXCONNECTJUMBO;
        /// <summary>
        /// 服務器向客戶端發送信息
        /// </summary>
        public static AllinoneEvent JUMBOSERVEREVENT;

        /// <summary>
        /// 連接至量測服務器
        /// </summary>
        public static IxConnectCrystal501 IXCONNECTCRYSTAL;
        /// <summary>
        /// 量測服務器向客戶端發送信息
        /// </summary>
        public static AllinoneCrystalEvent CRYSTALSERVEREVENT;

        public static CipExtendClass CipExtend;
        public static JzMVDJudgeRecipeClass JzMVDJudgeRecipe;
        /// <summary>
        /// 研磨自动切换参数成功的标志
        /// </summary>
        public static bool IsChangeRecipeing = false;

        public static ResultClass RESULT;
        public static JzR32ResultClass jzr32eresult;
        public static JzRXXResultClass jzrxxeresult;
        public static JzR15ResultClass jzr15eresult;
        public static JzR9ResultClass jzr9eresult;
        public static JzR5ResultClass jzr5eresult;
        public static JzR1ResultClass jzr1eresult;
        public static JzR3ResultClass jzr3eresult;
        public static JzC3ResultClass jzC3eresult;
        public static JzMainSDResultClass jzMainSDresult;
        public static JzMainX6ResultClass jzMainX6Result;
        public static JzMainSDM1ResultClass jzMainSDM1result;
        public static JzMainSDM2ResultClass jzMainSDM2result;
        public static JzMainServiceResultClass jzMainServiceResult;
        public static JzMainSDM3ResultClass jzMainSDM3result;
        public static JzMainSDM5ResultClass jzMainSDM5result;
        public static bool ISRESERVEPASSIMAGE = false;  //PASS的圖是否要存起來
        public static bool ISCHECKSN = false;
        /// <summary>
        /// OCR 出来的SN
        /// </summary>
        public static string OCRSN = "";
        /// <summary>
        /// 出现Fail02 时, 是否打开复判功能(SN ByPass)
        /// </summary>
        public static bool isFail02_Retrial = false;
        /// <summary>
        /// 镭雕是否NG
        /// </summary>
        public static bool ISBCNG = false;
        /// <summary>
        /// 上一次的条码
        /// </summary>
        public static string OLDBARCODE = "";

        /// <summary>
        /// MAIN_SD调试马达测试窗口是否打开标志
        /// </summary>
        public static bool IsOpenMotorWindows = false;
        /// <summary>
        /// 离线模式自动登入账户admin
        /// </summary>
        public static bool IsOfflineUserAutoLogin = false;

        /// <summary>
        /// MAIN_SD报警记录窗口是否打开标志
        /// </summary>
        public static bool IsOpenAlarmWindows = false;

        /// <summary>
        /// 旧的条码集合
        /// </summary>
        public static List<string> OLDBARCODELIST = new List<string>();

        public static IndexTableClass COLORTABLE;

#if OPT_USE_THROUGH
        public static Jumbo301Client.FormSpace.MainForm M_JUMBOCLIENT;
#endif

#if (OPT_USE_THROUGH_CRYSTAL)
        public static JetEazyCrystal.FormSpace.MainForm M_CRYSTALCLIENT;
#endif

        public static ProgressBar ProBar;
        public class IndexTableClass
        {
            List<IndexItemClass> ColorItemList = new List<IndexItemClass>();

            public IndexTableClass(string Str)
            {
                string[] strs = Str.Replace(Environment.NewLine, "@").Split('@');


                foreach (string str in strs)
                {
                    IndexItemClass coloritem = new IndexItemClass(str);
                    ColorItemList.Add(coloritem);
                }
            }

            public string Check(string Str)
            {
                string retStr = "NULL";


                foreach (IndexItemClass coloritem in ColorItemList)
                {
                    retStr = coloritem.Check(Str);

                    if (retStr != "NULL")
                    {
                        break;
                    }
                }

                return retStr;
            }

            public string Assemble()
            {
                string Str = "";

                foreach (IndexItemClass indexitem in ColorItemList)
                {
                    Str += indexitem.ToString() + Environment.NewLine;
                }

                //Str = myJzTools.RemoveLastChar(Str, 2);
                Str = Str.Remove(Str.Length - 2, 2);

                return Str;
            }
        }
        public class IndexItemClass
        {
            string Name = "";

            string IncludeString = "";

            public IndexItemClass(string Str)
            {
                string[] strs = Str.Split(':');

                Name = strs[0].ToUpper();
                IncludeString = strs[1].ToUpper();

            }

            public string Check(string Str)
            {
                string retStr = "NULL";

                if (IncludeString.IndexOf(Str.ToUpper()) > -1)
                {
                    retStr = Name;
                }

                return retStr;
            }

            public override string ToString()
            {
                string Str = "";

                Str += Name + ":" + IncludeString;

                return Str;
            }
        }

        public static int InitialEx()
        {
            //LanguageIndex = langindex;
            InitialRelateData();
            return 0;
        }
        public static bool Initial(int langindex)
        {
            TestProgram();

            //int iWeek = GetWeekNumber();

            bool ret = true;
            WORKPATH = MAINPATH + @"\WORK";

            //string input = "Hello123!@# World456原始字符串";
            //string result = Regex.Replace(input, @"[^a-zA-Z0-9]", "");
            ////StringBuilder sb = new StringBuilder();

            ////foreach (char c in input)
            ////{
            ////    if (char.IsLetterOrDigit(c))
            ////    {
            ////        sb.Append(c);
            ////    }
            ////}

            ////string result = sb.ToString();
            //Console.WriteLine($"原始字符串: {input}");
            //Console.WriteLine($"过滤后: {result}");
            //// 输出: Hello123World456

            //byte[] m_VersionByte = new byte[3] { 0x06, 0x01, 0x05 };
            //string value = System.Text.Encoding.ASCII.GetString(m_VersionByte);
            //Console.WriteLine(value);
            //try
            //{
            //    FACTORYNAME = (FactoryName)INI.FactoryNameIndex;
            //}
            //catch
            //{
            //    FACTORYNAME = FactoryName.NONE;
            //}

            JetEazy.BasicSpace.LanguageExClass.Instance.Load(Universal.WORKPATH);
            JetEazy.BasicSpace.LanguageExClass.Instance.LanguageIndex = INI.LANGUAGE;
            //JetEazy.BasicSpace.LanguageExClass.Instance.FirstCsv = true;

            string ccd_type_filepath = "";

            switch (Universal.OPTION)
            {
                case OptionEnum.MAIN_X6:

                    try
                    {
                        FACTORYNAME = (FactoryName)INI.FactoryNameIndex;
                    }
                    catch
                    {
                        FACTORYNAME = FactoryName.NONE;
                    }

                    try
                    {
                        jetMappingType = (JetMappingType)INI.MappingTypeIndex;
                    }
                    catch
                    {
                        jetMappingType = JetMappingType.NONE;
                    }

                    switch (INI.CHANGE_FILE_PATH)
                    {
                        //TYPE18 1800W
                        case 1:

                            ccd_type_filepath = "_TYPE18";

                            DBPATH = DBPATH + ccd_type_filepath;
                            RCPPATH = RCPPATH + ccd_type_filepath;
                            DEBUGSRCPATH = DEBUGSRCPATH + ccd_type_filepath;

                            break;
                    }

                    MapBuilder = Allinone.ZGa.Mvc.GaMvcConfig.CreateMapBuilder();

                    if (INI.bUse2DCNNReader)
                    {
                        //GvlMvd2D_CNNReader = new Mvd2dCNNReader();
                        //GvlMvd2D_CNNReader.DecodeTrain();
                        CodeBuilder = new ZGa.Mvc.Model.BarcodeModel.CodeBuilderClassV0();
                        CodeBuilder.Init();
                    }

                    break;
            }

            mOCRByPaddle = new OCRByPaddle.OCRByPaddle();

            string spath = System.AppDomain.CurrentDomain.BaseDirectory;
            if (File.Exists(spath + @"\license.dat"))
                File.Delete(spath + @"\license.dat");
            if (File.Exists(WORKPATH + @"\JetAu.dat"))
                File.Copy(WORKPATH + @"\JetAu.dat", spath + "license.dat");

            int i = 0;

            string SQLCMD = "";

            DATACOMMAND = new OleDbCommand[(int)DataTableEnum.COUNT];
            DATACMDBUILDER = new OleDbCommandBuilder[(int)DataTableEnum.COUNT];
            DATAADAPTER = new OleDbDataAdapter[(int)DataTableEnum.COUNT];

            DATASET = new DataSet();

            DATACONNECTION = new OleDbConnection(DATACNNSTRING);
            DATACONNECTION.Open();
            DATACONNECTION.Close();

            while (i < (int)DataTableEnum.COUNT)
            {
                SQLCMD = "select * from " + ((DataTableEnum)i).ToString() + " order by " + ((DataTableEnum)i).ToString() + ".no";
                DATAADAPTER[i] = new OleDbDataAdapter();
                DATACMDBUILDER[i] = new OleDbCommandBuilder(DATAADAPTER[i]);
                DATACOMMAND[i] = new OleDbCommand();
                DATACOMMAND[i].Connection = DATACONNECTION;

                //DATACOMMAND[i] = new  
                DATACMDBUILDER[i].QuotePrefix = "[";
                DATACMDBUILDER[i].QuoteSuffix = "]";

                DATACONNECTION.Open();

                DATAADAPTER[i].SelectCommand = new OleDbCommand(SQLCMD, DATACONNECTION);
                DATAADAPTER[i].Fill(DATASET, ((DataTableEnum)i).ToString());

                DATACONNECTION.Close();

                i++;
            }
            //TestProgram();


            #region 测试代码

            //Bitmap bmpinput = new Bitmap("D:\\JETEAZY\\ALLINONE-MAIN_SDM2\\SRCDEBUG\\20250423112544-20250423112544-F\\000\\P00-000.jpg");
            //HistogramClass histogram = new HistogramClass(2);
            //Bitmap bmp1 = (Bitmap)bmpinput.Clone(new Rectangle(1116, 576, 275, 2315), PixelFormat.Format24bppRgb);
            //histogram.GetHistogram(bmp1, 100);

            //int mode = histogram.ModeGrade;

            ////JzFindObjectClass jzFindObjectClass = new JzFindObjectClass();
            //Bitmap bmpinput = new Bitmap("D:\\JETEAZY\\ALLINONE-MAIN_X6\\PIC\\00009\\000\\P00-000.png");
            //Bitmap bmpsave = new Bitmap(bmpinput, bmpinput.Width / 2, bmpinput.Height / 2);
            //bmpsave.Save("D:\\JETEAZY\\ALLINONE-MAIN_X6\\PIC\\00009\\000\\P00-000_0.png", ImageFormat.Png);
            //bmpinput.Dispose();
            //bmpsave.Dispose();
            //Bitmap bmppattern = new Bitmap("D:\\_tmp\\pattern.png");

            ////pictureBox1.Image = new Bitmap("test.png");
            ////String tempImg_path = ("t1.png");
            ////String srcImg_path = ("test.png");
            //Bitmap bitmap = OpencvMatchClass.Recoganize(bmpinput, bmppattern, 0.7, 1, "target", 100);
            ////pictureBox2.Image = bitmap;
            //bitmap.Save("D:\\_tmp\\bmpMatched.png", System.Drawing.Imaging.ImageFormat.Png);
            ////OpencvMatchClass.FindSimilarEx(bmpinput, bmppattern, 0.5f, new List<DoffsetClass>());
            //bmpinput.Dispose();
            //bmppattern.Dispose();
            //bitmap.Dispose();

            //Bitmap bmpinput = new Bitmap("D:\\JETEAZY\\ALLINONE-MAIN_X6\\SRCDEBUG\\00007 _20230215162525\\000\\P00-000.png");
            //Bitmap bmporg = (Bitmap)bmpinput.Clone();//  new Bitmap(bmpinput);
            //Bitmap bmporg = new Bitmap(bmpinput, new Size(bmpinput.Width / 2, bmpinput.Height / 2));
            //bmporg.Save("D:\\LOA\\P00-000.png", ImageFormat.Png);

            //FreeImageBitmap bmpinput = new FreeImageBitmap("D:\\JETEAZY\\ALLINONE-MAIN_X6\\SRCDEBUG\\00007 _20230215162525\\000\\P00-000.png");
            //FreeImageBitmap bmporg = new FreeImageBitmap(bmpinput);
            //bmporg.Dispose();
            //bmpinput.Dispose();
            #endregion

            LanguageIndex = langindex;
            InitialRelateData();

            CreateDebugDirectories();

            myLanguage.Initial(UIPATH + "\\Universal.jdb", LanguageIndex);

            ACCDB = new AccDBClass(DATASET.Tables[DataTableEnum.ACCDB.ToString()]);
            ESSDB = new EsssDBClass(DATASET.Tables[DataTableEnum.ESSDB.ToString()]);
            RCPDB = new RcpDBClass(DATASET.Tables[DataTableEnum.RCPDB.ToString()]);
            //RUNDB = new RUNDBClass(DBPATH + @"\RUNDB.jdb");

            //200M
            InitialOCR();
            //360M
            InitialMSR();

            InitialASN();

            //1369M
            InitialAlbum();

            //2810M -> 1440M
            switch (VERSION)
            {
                default:
                    ISRESERVEPASSIMAGE = false;
                    break;
            }

            ret &= InitialMachineCollection();
            ret &= MyLogInitial();

            if (!ret)
            {
                InitialErrorString = ToChangeLanguageCode("Universal.msg1");// myLanguage.Messages("msg1", LanguageIndex);
                return false;
            }

            ret &= InitialExtend();

            if (!ret)
            {
                InitialErrorString = "连接欧姆龙plc失败，Cip通讯失败。";
                return false;
            }

            ret &= InitialChangeRecipe() == 0;

            if (!ret)
            {
                InitialErrorString = "加载Pattern资料失败。";
                return false;
            }

            ret &= InitialCCD();

            LoadProgressBarValueADD();

            if (!ret)
            {
                InitialErrorString = ToChangeLanguageCode("Universal.msg2");// myLanguage.Messages("msg2", LanguageIndex);
                return false;
            }

            ret &= MyServerInitial();
            //ret &= MyLogInitial();

            if (m_UseCommToDLHandle)
                ret &= MyTcpSocketInitial();

            if (!ret)
            {
                return false;
            }

            ret &= MyQFactoryInitial();

            if (!ret)
            {
                return false;
            }

            ret &= MyAIModelInitial();

            if (!ret)
            {
                InitialErrorString = "AI模型加载失败";
                return false;
            }

            InitialResult();

            //switch(OPTION)
            //{
            //    case OptionEnum.R3:
            //        {
            Correct = new OPSpace.AnalyzeSpace.CorrestClass();
            Correct.Initial(WORKPATH);
            //        }
            //        break;
            //}

            return ret;
        }
        public static void LoadProgressBarValueADD()
        {

            if (ProBar.Maximum > ProBar.Value)
            {
                ProBar.Value++;
                ProBar.Refresh();
            }

        }
        public static void SetLanguage(int langindex)
        {
            LanguageIndex = langindex;
        }
        public static void JSPInitial()
        {
            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN:

                            SCREENPOINTS = new JzScreenPointsClass(6);
                            SCREENPOINTS.Initial();

                            break;
                    }
                    break;
            }
        }

        static void InitialAlbum()
        {
            ALBCollection = new AlbumCollectionClass();
            ALBCollection.RCPPATH = RCPPATH;
            ALBCollection.PRELOADCOUNT = INI.PRELOADCOUNT;

            #region 預載 80000 等參數，之前是定死80000，現在是用指定的 
            if (INI.PRELOADSTATICNO.Trim() != "") //先把要預載的80000那些的先載入 
            {
                string[] preloadstaticindexstring = INI.PRELOADSTATICNO.Split(',');

                foreach (string preloadindexstr in preloadstaticindexstring)
                {
                    int preloadstaticindex = int.Parse(preloadindexstr);
                    int preloadindicator = RCPDB.FindIndex(preloadstaticindex);

                    if (preloadindicator != -1)
                    {
                        RCPDB.GotoIndex(preloadindicator);

                        AlbumClass preloadstaticalbum = new AlbumClass(RCPDB.DataNow);
                        ALBCollection.AddStatic(preloadstaticalbum);
                    }
                }
            }
            #endregion

            #region 載入ESSDB的最後一筆參數，若最後一筆參數在Static中，則不再產生Album

            int rcpindex = RCPDB.FindIndex(ESSDB.DataNow.LastRecipeNo);

            if (rcpindex == -1) //若找不到有包含ESSDB指定的INDEX，就回到第0001個參數，並存起來
            {
                ESSDB.DataNow.LastRecipeNo = 1;
                rcpindex = RCPDB.FindIndex(ESSDB.DataNow.LastRecipeNo);
            }

            RCPDB.GotoIndex(rcpindex);

            if (ALBCollection.FindStaticIndicator(ESSDB.DataNow.LastRecipeNo) == -1)
            {
                AlbumClass album = new AlbumClass(RCPDB.DataNow);
                ALBCollection.Add(album);
            }

            #endregion

            #region 載入預載參數
            if (INI.PRELOADNO.Trim() != "") //重覆的在 Album.Add裏會去檢查 
            {
                string[] preloadindexstring = INI.PRELOADNO.Split(',');

                foreach (string preloadindexstr in preloadindexstring)
                {
                    int preloadindex = int.Parse(preloadindexstr);
                    int preloadindicator = RCPDB.FindIndex(preloadindex);

                    if (preloadindicator != -1)
                    {
                        RCPDB.GotoIndex(preloadindicator);

                        AlbumClass preloadalbum = new AlbumClass(RCPDB.DataNow);
                        ALBCollection.Add(preloadalbum);
                    }
                }
            }
            #endregion

            RCPDB.GotoIndex(rcpindex);
            ALBCollection.GotoIndex(RCPDB.DataNow.No);

            //BackupDATADB();
            //ESSDB.Save();
        }

        public static void ADD80002(ProgressBar progressBar, Label lblShow)
        {

            progressBar.Maximum = RCPDB.myDataList.Count;
            progressBar.Value = 0;
            lblShow.Text = "0/" + RCPDB.myDataList.Count;
            for (int i = 0; i < RCPDB.myDataList.Count; i++)
            {
                progressBar.Value++;
                progressBar.Refresh();

                lblShow.Text = i + "/" + RCPDB.myDataList.Count;
                lblShow.Refresh();

                int preloadstaticindex = RCPDB.myDataList[i].No;
                if (preloadstaticindex > 70000)
                    continue;

                int preloadindicator = RCPDB.FindIndex(preloadstaticindex);

                if (preloadindicator != -1)
                {
                    RCPDB.GotoIndex(preloadindicator);

                    AlbumClass preloadstaticalbum = new AlbumClass(RCPDB.DataNow);
                    preloadstaticalbum.Load(RCPPATH);

                    bool isOK = false;
                    if (preloadstaticalbum.ENVList.Count == 0)
                        continue;
                    foreach (PageClass patetemp in preloadstaticalbum.ENVList[0].PageList)
                    {
                        if (patetemp.RelateToRcpNo == 80002)
                        {
                            isOK = true;
                            break;
                        }
                    }
                    if (isOK)
                        continue;


                    PageClass page = preloadstaticalbum.ENVList[0].PageList[0].Clone();

                    int index = 0;
                    foreach (PageClass patetemp in preloadstaticalbum.ENVList[0].PageList)
                    {
                        if (index <= patetemp.No)
                            index = patetemp.No + 1;
                    }
                    page.No = index;// ENVNow.PageList.Count;
                    page.GetPassInfoIncludeAnalyze(preloadstaticalbum.ENVList[0].PageList[0].PassInfo);

                    page.RelateToRcpNo = 80002;
                    preloadstaticalbum.ENVList[0].PageList.Add(page);

                    preloadstaticalbum.Save();
                }
            }
        }
        public static void ADD80003(ProgressBar progressBar, Label lblShow)
        {

            progressBar.Maximum = RCPDB.myDataList.Count;
            progressBar.Value = 0;
            lblShow.Text = "0/" + RCPDB.myDataList.Count;
            for (int i = 0; i < RCPDB.myDataList.Count; i++)
            {
                progressBar.Value++;
                progressBar.Refresh();

                lblShow.Text = i + "/" + RCPDB.myDataList.Count;
                lblShow.Refresh();

                int preloadstaticindex = RCPDB.myDataList[i].No;
                if (preloadstaticindex > 70000)
                    continue;

                int preloadindicator = RCPDB.FindIndex(preloadstaticindex);

                if (preloadindicator != -1)
                {
                    RCPDB.GotoIndex(preloadindicator);

                    AlbumClass preloadstaticalbum = new AlbumClass(RCPDB.DataNow);
                    preloadstaticalbum.Load(RCPPATH);

                    bool isOK = false;
                    if (preloadstaticalbum.ENVList.Count == 0)
                        continue;
                    foreach (PageClass patetemp in preloadstaticalbum.ENVList[0].PageList)
                    {
                        if (patetemp.RelateToRcpNo == 80003)
                        {
                            isOK = true;
                            break;
                        }
                    }
                    if (isOK)
                        continue;


                    PageClass page = preloadstaticalbum.ENVList[0].PageList[0].Clone();

                    int index = 0;
                    foreach (PageClass patetemp in preloadstaticalbum.ENVList[0].PageList)
                    {
                        if (index <= patetemp.No)
                            index = patetemp.No + 1;
                    }
                    page.No = index;// ENVNow.PageList.Count;
                    page.GetPassInfoIncludeAnalyze(preloadstaticalbum.ENVList[0].PageList[0].PassInfo);

                    page.RelateToRcpNo = 80003;
                    preloadstaticalbum.ENVList[0].PageList.Add(page);

                    preloadstaticalbum.Save();
                }
            }
        }

        public static void ADD80004(ProgressBar progressBar, Label lblShow)
        {

            progressBar.Maximum = RCPDB.myDataList.Count;
            progressBar.Value = 0;
            lblShow.Text = "0/" + RCPDB.myDataList.Count;
            for (int i = 0; i < RCPDB.myDataList.Count; i++)
            {
                progressBar.Value++;
                progressBar.Refresh();

                lblShow.Text = i + "/" + RCPDB.myDataList.Count;
                lblShow.Refresh();

                int preloadstaticindex = RCPDB.myDataList[i].No;
                if (preloadstaticindex > 70000)
                    continue;

                int preloadindicator = RCPDB.FindIndex(preloadstaticindex);

                if (preloadindicator != -1)
                {
                    RCPDB.GotoIndex(preloadindicator);

                    AlbumClass preloadstaticalbum = new AlbumClass(RCPDB.DataNow);
                    preloadstaticalbum.Load(RCPPATH);

                    bool isOK = false;
                    if (preloadstaticalbum.ENVList.Count == 0)
                        continue;
                    foreach (PageClass patetemp in preloadstaticalbum.ENVList[0].PageList)
                    {
                        if (patetemp.RelateToRcpNo == 80004)
                        {
                            isOK = true;
                            break;
                        }
                    }
                    if (isOK)
                        continue;


                    PageClass page = preloadstaticalbum.ENVList[0].PageList[0].Clone();

                    int index = 0;
                    foreach (PageClass patetemp in preloadstaticalbum.ENVList[0].PageList)
                    {
                        if (index <= patetemp.No)
                            index = patetemp.No + 1;
                    }
                    page.No = index;// ENVNow.PageList.Count;
                    page.GetPassInfoIncludeAnalyze(preloadstaticalbum.ENVList[0].PageList[0].PassInfo);

                    page.RelateToRcpNo = 80004;
                    preloadstaticalbum.ENVList[0].PageList.Add(page);

                    preloadstaticalbum.Save();
                }
            }
        }
        /// <summary>
        /// 決定測試有幾個共同的畫面，根據不同的版本而變化
        /// </summary>
        static void InitialRelateData()
        {
            switch (VERSION)
            {
                case VersionEnum.KBAOI:
                    PAGEOPTYPECOUNT = 1;
                    break;
                default:
                    PAGEOPTYPECOUNT = 1;
                    break;
            }
        }
        static void InitialResult()
        {
            switch (VERSION)
            {
                case VersionEnum.ALLINONE:

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN:

                            JzAllInoneResultClass jzallinoneresult = new JzAllInoneResultClass(Result_EA.Allinone, VERSION, OPTION, MACHINECollection);
                            RESULT = new ResultClass(jzallinoneresult);
                            break;
                        case OptionEnum.R32:

                            string r32str = "";

                            ReadData(ref r32str, DBPATH + "\\COLORTABLE.jdb");
                            COLORTABLE = new IndexTableClass(r32str);

                            jzr32eresult = new JzR32ResultClass(Result_EA.R32, VERSION, OPTION, MACHINECollection);
                            RESULT = new ResultClass(jzr32eresult);
                            break;
                        case OptionEnum.R26:

                            string rxxstr = "";

                            ReadData(ref rxxstr, DBPATH + "\\COLORTABLE.jdb");
                            COLORTABLE = new IndexTableClass(rxxstr);

                            jzrxxeresult = new JzRXXResultClass(Result_EA.RXX, VERSION, OPTION, MACHINECollection);
                            RESULT = new ResultClass(jzrxxeresult);
                            break;
                        case OptionEnum.R15:

                            string r15str = "";

                            ReadData(ref r15str, DBPATH + "\\COLORTABLE.jdb");
                            COLORTABLE = new IndexTableClass(r15str);

                            jzr15eresult = new JzR15ResultClass(Result_EA.R15, VERSION, OPTION, MACHINECollection);
                            RESULT = new ResultClass(jzr15eresult);
                            break;
                        case OptionEnum.R9:

                            string r9str = "";

                            ReadData(ref r9str, DBPATH + "\\COLORTABLE.jdb");
                            COLORTABLE = new IndexTableClass(r9str);

                            jzr9eresult = new JzR9ResultClass(Result_EA.R9, VERSION, OPTION, MACHINECollection);
                            RESULT = new ResultClass(jzr9eresult);
                            break;
                        case OptionEnum.R5:

                            string r5str = "";

                            ReadData(ref r5str, DBPATH + "\\COLORTABLE.jdb");
                            COLORTABLE = new IndexTableClass(r5str);

                            jzr5eresult = new JzR5ResultClass(Result_EA.R5, VERSION, OPTION, MACHINECollection);
                            RESULT = new ResultClass(jzr5eresult);
                            break;
                        case OptionEnum.R1:

                            string r1str = "";

                            ReadData(ref r1str, DBPATH + "\\COLORTABLE.jdb");
                            COLORTABLE = new IndexTableClass(r1str);

                            jzr1eresult = new JzR1ResultClass(Result_EA.R1, VERSION, OPTION, MACHINECollection);
                            RESULT = new ResultClass(jzr1eresult);
                            break;
                        case OptionEnum.R3:

                            string r3str = "";

                            ReadData(ref r3str, DBPATH + "\\COLORTABLE.jdb");
                            COLORTABLE = new IndexTableClass(r3str);

                            jzr3eresult = new JzR3ResultClass(Result_EA.R3, VERSION, OPTION, MACHINECollection);
                            RESULT = new ResultClass(jzr3eresult);
                            break;
                        case OptionEnum.C3:

                            string C3str = "";

                            ReadData(ref C3str, DBPATH + "\\COLORTABLE.jdb");
                            COLORTABLE = new IndexTableClass(C3str);

                            jzC3eresult = new JzC3ResultClass(Result_EA.C3, VERSION, OPTION, MACHINECollection);
                            RESULT = new ResultClass(jzC3eresult);
                            break;

                        case OptionEnum.MAIN_SD:

                            jzMainSDresult = new JzMainSDResultClass(Result_EA.MAIN_SD, VERSION, OPTION, MACHINECollection);
                            RESULT = new ResultClass(jzMainSDresult);

                            break;
                        case OptionEnum.MAIN_X6:

                            jzMainX6Result = new JzMainX6ResultClass(Result_EA.MAIN_X6, VERSION, OPTION, MACHINECollection);
                            //if (IsUseThreadReviceTcp)
                            //    jzMainX6Result.start_scan_thread();
                            RESULT = new ResultClass(jzMainX6Result);

                            break;

                        case OptionEnum.MAIN_SDM1:

                            jzMainSDM1result = new JzMainSDM1ResultClass(Result_EA.MAIN_SDM1, VERSION, OPTION, MACHINECollection);
                            RESULT = new ResultClass(jzMainSDM1result);

                            break;
                        case OptionEnum.MAIN_SDM2:

                            jzMainSDM2result = new JzMainSDM2ResultClass(Result_EA.MAIN_SDM2, VERSION, OPTION, MACHINECollection);
                            RESULT = new ResultClass(jzMainSDM2result);

                            break;
                        case OptionEnum.MAIN_SERVICE:

                            jzMainServiceResult = new JzMainServiceResultClass(Result_EA.MAIN_SERVICE, VERSION, OPTION, MACHINECollection);
                            RESULT = new ResultClass(jzMainServiceResult);

                            break;
                        case OptionEnum.MAIN_SDM3:

                            jzMainSDM3result = new JzMainSDM3ResultClass(Result_EA.MAIN_SDM3, VERSION, OPTION, MACHINECollection);
                            RESULT = new ResultClass(jzMainSDM3result);

                            break;
                        case OptionEnum.MAIN_SDM5:

                            jzMainSDM5result = new JzMainSDM5ResultClass(Result_EA.MAIN_SDM5, VERSION, OPTION, MACHINECollection);
                            RESULT = new ResultClass(jzMainSDM5result);

                            break;
                    }

                    break;
                case VersionEnum.AUDIX:

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN:
                            JzAudixResultClass jzaudixresult = new JzAudixResultClass(Result_EA.Audix, VERSION, OPTION, MACHINECollection);
                            RESULT = new ResultClass(jzaudixresult);
                            break;
                        case OptionEnum.MAIN_DFLY:
                            JzAudixDflyResultClass jzaudixdflyresult = new JzAudixDflyResultClass(Result_EA.AudixDfly, VERSION, OPTION, MACHINECollection);
                            RESULT = new ResultClass(jzaudixdflyresult);
                            break;
                    }
                    break;
            }

            RESULT.RefreshDebugSrcDirectory(DEBUGSRCPATH);

        }


        public static void LOADCOLORTABLE()
        {
            JetEazy.LoggerClass.Instance.WriteLog("登入 编写EEEE CODE页面.");
            FormSpace.ADDEEEECODE eeeeform = new FormSpace.ADDEEEECODE(DBPATH + "\\COLORTABLE.jdb");
            eeeeform.ShowDialog();

            string r32str = "";
            ReadData(ref r32str, DBPATH + "\\COLORTABLE.jdb");
            COLORTABLE = new IndexTableClass(r32str);
        }
        static void InitialOCR()
        {
            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    OCRCollectionClass.OCRCollectionPath = $@"{PROGRAMME_ROOT}\JETEAZY\OCR";
                    OCRCollection = new OCRCollectionClass();
                    OCRCollection.Initial();
                    break;
                default:

                    break;
            }
        }
        static void InitialMSR()
        {
            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN:
                            MSRCollectionClass.MSRCollectionPath = $@"{PROGRAMME_ROOT}\JETEAZY\MSR";
                            MSRCollection = new MSRCollectionClass();
                            MSRCollection.Initial();
                            break;
                        case OptionEnum.MAIN_X6:
                            //switch(FACTORYNAME)
                            //{
                            //    case FactoryName.DAGUI:
                            //        MSRCollectionClass.MSRCollectionPath = $@"{PROGRAMME_ROOT}\JETEAZY\MSR";
                            //        MSRCollection = new MSRCollectionClass();
                            //        MSRCollection.Initial();
                            //        break;
                            //}
                           
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
        static bool MyQFactoryInitial()
        {
            bool ret = true;

            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch (OPTION)
                    {
                        case OptionEnum.R32:
                        case OptionEnum.R26:
                        case OptionEnum.R15:
                        case OptionEnum.R9:
                        case OptionEnum.R5:
                        case OptionEnum.R3:
                        case OptionEnum.C3:

                            JZQFACTORY = new JzQFactoryClass();
                            if (INI.ISUSE_QFACTORY)
                            {
                                JZQFACTORY.Init(INI.QFACTORY_EQ_SN,
                                                               INI.QFACTORY_EQ_LocationID,
                                                               INI.QFACTORY_EQ_LocationID2,
                                                               INI.QFACTORY_Station,
                                                               INI.QFACTORY_Step);
                            }


                            break;
                    }
                    break;
            }

            return ret;
        }
        static bool MyAIModelInitial()
        {
            bool ret = true;

            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_X6:
                        case OptionEnum.MAIN_SDM2:
                            ret = AIConnect();
                            break;
                    }
                    break;
            }

            return ret;
        }
        static bool MyLogInitial()
        {
            bool ret = true;

            switch (VERSION)
            {
                case VersionEnum.ALLINONE:

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_SD:

                            JetEazy.BasicSpace.CommonLogClass.Instance.LogPath = @"D:\LOG\ACT_MAIN_SD";
                            //JetEazy.BasicSpace.LanguageExClass.Instance.Load(Universal.WORKPATH);
                            //JetEazy.BasicSpace.LanguageExClass.Instance.LanguageIndex = INI.LANGUAGE;


                            break;
                        case OptionEnum.MAIN_X6:

                            JetEazy.BasicSpace.CommonLogClass.Instance.LogPath = @"D:\LOG\ACT_MAIN_X6";
                            JetEazy.BasicSpace.CommonLogClass.Instance.LogFilename = "tcp";

                            //JetEazy.BasicSpace.LanguageExClass.Instance.Load(Universal.WORKPATH);
                            //JetEazy.BasicSpace.LanguageExClass.Instance.LanguageIndex = INI.LANGUAGE;
                            //JetEazy.BasicSpace.LanguageExClass.Instance.FirstCsv = true;

                            break;

                        case OptionEnum.MAIN_SDM1:

                            JetEazy.BasicSpace.CommonLogClass.Instance.LogPath = @"D:\LOG\ACT_MAIN_SDM1";
                            //JetEazy.BasicSpace.LanguageExClass.Instance.Load(Universal.WORKPATH);
                            //JetEazy.BasicSpace.LanguageExClass.Instance.LanguageIndex = INI.LANGUAGE;

                            break;
                        case OptionEnum.MAIN_SDM2:

                            JetEazy.BasicSpace.CommonLogClass.Instance.LogPath = @"D:\LOG\ACT_MAIN_SDM2";
                            //JetEazy.BasicSpace.LanguageExClass.Instance.Load(Universal.WORKPATH);
                            //JetEazy.BasicSpace.LanguageExClass.Instance.LanguageIndex = INI.LANGUAGE;

                            break;
                        case OptionEnum.MAIN_SERVICE:

                            JetEazy.BasicSpace.CommonLogClass.Instance.LogPath = @"D:\LOG\ACT_MAIN_SERVICE";
                            JetEazy.BasicSpace.CommonLogClass.Instance.LogFilename = "tcp";

                            //JetEazy.BasicSpace.LanguageExClass.Instance.Load(Universal.WORKPATH);
                            //JetEazy.BasicSpace.LanguageExClass.Instance.LanguageIndex = 0;

                            break;
                        case OptionEnum.MAIN_SDM3:

                            JetEazy.BasicSpace.CommonLogClass.Instance.LogPath = @"D:\LOG\ACT_MAIN_SDM3";
                            //JetEazy.BasicSpace.LanguageExClass.Instance.Load(Universal.WORKPATH);
                            //JetEazy.BasicSpace.LanguageExClass.Instance.LanguageIndex = INI.LANGUAGE;

                            break;
                    }

                    break;

            }

            return ret;
        }
        static bool MyTcpSocketInitial()
        {
            bool ret = true;

            switch (VERSION)
            {
                case VersionEnum.ALLINONE:

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_SD:



                            break;
                        case OptionEnum.MAIN_X6:
                            if (X6_LASER_CLIENT == null)
                            {
                                X6_LASER_CLIENT = new ClientSocket("laser");
                            }
                            X6_LASER_CLIENT.Host = INI.tcp_ip;
                            X6_LASER_CLIENT.Port = INI.tcp_port;
                            int iret = X6_LASER_CLIENT.ConnectServer();
                            //ret = iret == 0;
                            if (iret != 0)
                            {
                                MessageBox.Show(ToChangeLanguage("连接打标服务器错误请检查") + "ip=" + INI.tcp_ip + ",port=" + INI.tcp_port.ToString(), "init", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            if (Universal.IsNoUseCCD)
                            {
                                if (X6_HANDLE_CLIENT == null)
                                    X6_HANDLE_CLIENT = new ClientSocket("handle");
                                //if (X6_HANDLE_CLIENT == null)
                                //    X6_HANDLE_CLIENT = new ClientSocket("handle32002");
                            }
                            else
                            {
                                if (X6_HANDLE_CLIENT == null)
                                    X6_HANDLE_CLIENT = new ClientSocket("handle");
                            }

                            X6_HANDLE_CLIENT.Host = INI.tcp_handle_ip;
                            X6_HANDLE_CLIENT.Port = INI.tcp_handle_port;
                            iret = X6_HANDLE_CLIENT.ConnectServer(!INI.tcp_handle_open);
                            if (iret != 0)
                            {
                                MessageBox.Show(ToChangeLanguage("连接handle服务器错误请检查") + "ip=" + INI.tcp_handle_ip + ",port=" + INI.tcp_handle_port.ToString(), "Init", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            X6_LASER_CLIENT.Log.LogPath = @"D:\LOG\ACT_MAIN_X6";
                            X6_LASER_CLIENT.Log.LogFilename = "laserServer_tcp";

                            X6_HANDLE_CLIENT.Log.LogPath = @"D:\LOG\ACT_MAIN_X6";
                            X6_HANDLE_CLIENT.Log.LogFilename = "handleServer_tcp";

                            break;
                    }

                    break;

            }

            return ret;
        }
        static bool MyServerInitial()
        {
            bool ret = true;

            if (INI.ISHIVECLIENT)
            {
                JZHIVECLIENT = new JzHiveClass();
                JZHIVECLIENT.Model = INI.HIVE_model;
                JZHIVECLIENT.IsLocalSystemUpload = INI.HIVE_islocalsystemupload;
            }

            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN:

#if (OPT_USE_THROUGH)
                            M_JUMBOCLIENT = new Jumbo301Client.FormSpace.MainForm();
                            M_JUMBOCLIENT.Show();
                            M_JUMBOCLIENT.WindowState = System.Windows.Forms.FormWindowState.Minimized;

                            LoadProgressBarValueADD();
#endif

#if (OPT_USE_THROUGH_CRYSTAL)
                            M_CRYSTALCLIENT = new JetEazyCrystal.FormSpace.MainForm();
                            M_CRYSTALCLIENT.Show();
                            M_CRYSTALCLIENT.WindowState = System.Windows.Forms.FormWindowState.Minimized;

                            //System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ThreadStart(KGMANDKHC));
                            //th.Start();

                            LoadProgressBarValueADD();
#else
                            //if(INI.ISUSECRYSTALSERVER)
                            //{
                            //    string crystal_EXE_path = @"D:\AUTOMATION\Eazy AOI DX\Allinone\CRYSTAL501\CRYSTAL501\bin\Debug\CRYSTAL501.exe";
                            //    if (System.IO.File.Exists(crystal_EXE_path))
                            //    {
                            //        //Universal.Memory.OpenShareMemory("BYD");
                            //        System.Diagnostics.Process.Start(crystal_EXE_path);
                            //    }
                            //    else
                            //    {
                            //        InitialErrorString = "間隙程式打開失敗，請手動打開。路徑如下:" + Environment.NewLine;
                            //        InitialErrorString += crystal_EXE_path;

                            //        MessageBox.Show(Universal.InitialErrorString, "Initial", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //        //return false;
                            //    }
                            //}
#endif


                            //SCREENPOINTS = new JzScreenPointsClass(6);
                            //SCREENPOINTS.Initial();

                            if (INI.ISUSEJUMBOSERVER)
                                ret &= InitialJumboServer();

                            if (!ret)
                            {
                                InitialErrorString = ToChangeLanguageCode("Universal.msg3");//myLanguage.Messages("msg3", LanguageIndex);
                                return false;
                            }

                            if (INI.ISUSECRYSTALSERVER)
                                ret &= InitialCrystalServer();

                            if (!ret)
                            {
                                InitialErrorString = ToChangeLanguageCode("Universal.msg4");// myLanguage.Messages("msg4", LanguageIndex);
                                return false;
                            }

                            #region GAARA OFFLINE TEST
                            //**********************************************
                            //**測試JzFind 尋找報錯問題                                           ***
                            //**解決方式:將圖片縮小尋找，之後結果再放大。 ***
                            //**********************************************

#if (OPT_GAARA_USE_FIND)
                            JzFindObjectClass jzfind = new JzFindObjectClass();
                            HistogramClass histogram = new HistogramClass(2);
                            Bitmap _BMP_TEST = new Bitmap("D:\\BAK\\TEST.bmp");
                            Bitmap _org = new Bitmap(_BMP_TEST);
                            Bitmap bmpSized = new Bitmap(_BMP_TEST, JzTools.Resize(_BMP_TEST.Size, -1));
                            Bitmap _bmp01 = new Bitmap(bmpSized);
                            _BMP_TEST.Dispose();
                            bmpSized.Dispose();
                            jzfind.SetThreshold(_bmp01, JzTools.SimpleRect(_bmp01.Size), 20, 255, 0, true);
                            _bmp01.Save("D:\\BAK\\TEST01-1.bmp", ImageFormat.Bmp);
                            jzfind.Find(_bmp01, Color.Red);
                            _bmp01.Save("D:\\BAK\\TEST01.bmp", ImageFormat.Bmp);

                            foreach(FoundClass found in jzfind.FoundList)
                            {
                                Rectangle _rect = JzTools.Resize(found.rect, 1);
                                JzTools.DrawRectEx(_org, _rect, new Pen(Color.Lime, 2));
                            }
                           
                            _org.Save("D:\\BAK\\Org_TEST01.bmp", ImageFormat.Bmp);
                            _bmp01.Dispose();
                            _org.Dispose();
#endif

                            #endregion

                            //string strX = IXCONNECTJUMBO.GetAssginResult("ESC");
                            //鍵高機初始化
                            //InitialKHC();
                            break;
                        case OptionEnum.R32:

                            if (INI.ISHIVECLIENT)
                            {
                                ((JzR32MachineClass)MACHINECollection.MACHINE).SetMachineState(MachineState.Idle);
                            }

                            break;
                        case OptionEnum.R26:

                            if (INI.ISHIVECLIENT)
                            {
                                ((JzRXXMachineClass)MACHINECollection.MACHINE).SetMachineState(MachineState.Idle);
                            }

                            break;
                        case OptionEnum.R15:

                            if (INI.ISHIVECLIENT)
                            {
                                ((JzR15MachineClass)MACHINECollection.MACHINE).SetMachineState(MachineState.Idle);
                            }

                            break;
                        case OptionEnum.R9:

                            if (INI.ISHIVECLIENT)
                            {
                                ((JzR9MachineClass)MACHINECollection.MACHINE).SetMachineState(MachineState.Idle);
                            }

                            break;
                        case OptionEnum.R5:

                            if (INI.ISHIVECLIENT)
                            {
                                ((JzR5MachineClass)MACHINECollection.MACHINE).SetMachineState(MachineState.Idle);
                            }

                            break;
                        case OptionEnum.R1:

                            if (INI.ISHIVECLIENT)
                            {
                                ((JzR1MachineClass)MACHINECollection.MACHINE).SetMachineState(MachineState.Idle);
                            }

                            break;
                        case OptionEnum.R3:

                            if (INI.ISHIVECLIENT)
                            {
                                ((JzR3MachineClass)MACHINECollection.MACHINE).SetMachineState(MachineState.Idle);
                            }

                            break;
                        case OptionEnum.C3:

                            if (INI.ISHIVECLIENT)
                            {
                                ((JzC3MachineClass)MACHINECollection.MACHINE).SetMachineState(MachineState.Idle);
                            }

                            break;
                    }
                    break;
            }

            return ret;
        }
        static bool MyServerInitial_old_no_use()
        {
            bool ret = true;

            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN:

#if (OPT_USE_THROUGH)
                            M_JUMBOCLIENT = new Jumbo301Client.FormSpace.MainForm();
                            M_JUMBOCLIENT.Show();
                            M_JUMBOCLIENT.WindowState = System.Windows.Forms.FormWindowState.Minimized;

                            LoadProgressBarValueADD();
#endif

#if (OPT_USE_THROUGH_CRYSTAL)
                            M_CRYSTALCLIENT = new JetEazyCrystal.FormSpace.MainForm();
                            M_CRYSTALCLIENT.Show();
                            M_CRYSTALCLIENT.WindowState = System.Windows.Forms.FormWindowState.Minimized;

                            //System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ThreadStart(KGMANDKHC));
                            //th.Start();

                            LoadProgressBarValueADD();
#else
                            //if(INI.ISUSECRYSTALSERVER)
                            //{
                            //    string crystal_EXE_path = @"D:\AUTOMATION\Eazy AOI DX\Allinone\CRYSTAL501\CRYSTAL501\bin\Debug\CRYSTAL501.exe";
                            //    if (System.IO.File.Exists(crystal_EXE_path))
                            //    {
                            //        //Universal.Memory.OpenShareMemory("BYD");
                            //        System.Diagnostics.Process.Start(crystal_EXE_path);
                            //    }
                            //    else
                            //    {
                            //        InitialErrorString = "間隙程式打開失敗，請手動打開。路徑如下:" + Environment.NewLine;
                            //        InitialErrorString += crystal_EXE_path;

                            //        MessageBox.Show(Universal.InitialErrorString, "Initial", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //        //return false;
                            //    }
                            //}
#endif


                            //SCREENPOINTS = new JzScreenPointsClass(6);
                            //SCREENPOINTS.Initial();

                            if (INI.ISUSEJUMBOSERVER)
                                ret &= InitialJumboServer();

                            if (!ret)
                            {
                                InitialErrorString = ToChangeLanguageCode("Universal.msg3");// myLanguage.Messages("msg3", LanguageIndex);
                                return false;
                            }

                            if (INI.ISUSECRYSTALSERVER)
                                ret &= InitialCrystalServer();

                            if (!ret)
                            {
                                InitialErrorString = ToChangeLanguageCode("Universal.msg4");// myLanguage.Messages("msg4", LanguageIndex);
                                return false;
                            }

                            #region GAARA OFFLINE TEST
                            //**********************************************
                            //**測試JzFind 尋找報錯問題                                           ***
                            //**解決方式:將圖片縮小尋找，之後結果再放大。 ***
                            //**********************************************

#if (OPT_GAARA_USE_FIND)
                            JzFindObjectClass jzfind = new JzFindObjectClass();
                            HistogramClass histogram = new HistogramClass(2);
                            Bitmap _BMP_TEST = new Bitmap("D:\\BAK\\TEST.bmp");
                            Bitmap _org = new Bitmap(_BMP_TEST);
                            Bitmap bmpSized = new Bitmap(_BMP_TEST, JzTools.Resize(_BMP_TEST.Size, -1));
                            Bitmap _bmp01 = new Bitmap(bmpSized);
                            _BMP_TEST.Dispose();
                            bmpSized.Dispose();
                            jzfind.SetThreshold(_bmp01, JzTools.SimpleRect(_bmp01.Size), 20, 255, 0, true);
                            _bmp01.Save("D:\\BAK\\TEST01-1.bmp", ImageFormat.Bmp);
                            jzfind.Find(_bmp01, Color.Red);
                            _bmp01.Save("D:\\BAK\\TEST01.bmp", ImageFormat.Bmp);

                            foreach(FoundClass found in jzfind.FoundList)
                            {
                                Rectangle _rect = JzTools.Resize(found.rect, 1);
                                JzTools.DrawRectEx(_org, _rect, new Pen(Color.Lime, 2));
                            }
                           
                            _org.Save("D:\\BAK\\Org_TEST01.bmp", ImageFormat.Bmp);
                            _bmp01.Dispose();
                            _org.Dispose();
#endif

                            #endregion

                            //string strX = IXCONNECTJUMBO.GetAssginResult("ESC");
                            //鍵高機初始化
                            //InitialKHC();
                            break;
                        case OptionEnum.R32:

                            if (INI.ISHIVECLIENT)
                            {
                                //string strhiveclientpathfile = @"C:\hive\hiveclient\hiveclient.exe";
                                //if (!File.Exists(strhiveclientpathfile))
                                //{
                                //    InitialErrorString = "Hiveclient soft 未在本機安裝及配置";
                                //    //return false;
                                //}

                                JZHIVECLIENT = new JzHiveClass();
                                //string strconfigpathfile = @"C:\hive\hiveclient\config\machineconfig.json";
                                //if (!File.Exists(strconfigpathfile))
                                {
                                    JZHIVECLIENT.PublisherIDOrMachineID = INI.HIVE_publisher_id;
                                    JZHIVECLIENT.Hiveclient_Init(INI.HIVE_site,
                                                                                        INI.HIVE_building,
                                                                                        INI.HIVE_line_type,
                                                                                        INI.HIVE_line,
                                                                                        INI.HIVE_station_type,
                                                                                        INI.HIVE_station_instance,
                                                                                        INI.HIVE_vendor);
                                }

                                ((JzR32MachineClass)MACHINECollection.MACHINE).SetMachineState(MachineState.Idle);
                            }

                            break;
                        case OptionEnum.R26:

                            if (INI.ISHIVECLIENT)
                            {
                                //string strhiveclientpathfile = @"C:\hive\hiveclient\hiveclient.exe";
                                //if (!File.Exists(strhiveclientpathfile))
                                //{
                                //    InitialErrorString = "Hiveclient soft 未在本機安裝及配置";
                                //    //return false;
                                //}

                                JZHIVECLIENT = new JzHiveClass();
                                //string strconfigpathfile = @"C:\hive\hiveclient\config\machineconfig.json";
                                //if (!File.Exists(strconfigpathfile))
                                {
                                    JZHIVECLIENT.PublisherIDOrMachineID = INI.HIVE_publisher_id;
                                    JZHIVECLIENT.Hiveclient_Init(INI.HIVE_site,
                                                                                        INI.HIVE_building,
                                                                                        INI.HIVE_line_type,
                                                                                        INI.HIVE_line,
                                                                                        INI.HIVE_station_type,
                                                                                        INI.HIVE_station_instance,
                                                                                        INI.HIVE_vendor);
                                }

                                ((JzRXXMachineClass)MACHINECollection.MACHINE).SetMachineState(MachineState.Idle);
                            }

                            break;
                        case OptionEnum.R15:

                            if (INI.ISHIVECLIENT)
                            {
                                //string strhiveclientpathfile = @"C:\hive\hiveclient\hiveclient.exe";
                                //if (!File.Exists(strhiveclientpathfile))
                                //{
                                //    InitialErrorString = "Hiveclient soft 未在本機安裝及配置";
                                //    //return false;
                                //}

                                JZHIVECLIENT = new JzHiveClass();
                                //string strconfigpathfile = @"C:\hive\hiveclient\config\machineconfig.json";
                                //if (!File.Exists(strconfigpathfile))
                                {
                                    JZHIVECLIENT.PublisherIDOrMachineID = INI.HIVE_publisher_id;
                                    JZHIVECLIENT.Hiveclient_Init(INI.HIVE_site,
                                                                                        INI.HIVE_building,
                                                                                        INI.HIVE_line_type,
                                                                                        INI.HIVE_line,
                                                                                        INI.HIVE_station_type,
                                                                                        INI.HIVE_station_instance,
                                                                                        INI.HIVE_vendor);
                                }

                                ((JzR15MachineClass)MACHINECollection.MACHINE).SetMachineState(MachineState.Idle);
                            }

                            break;
                        case OptionEnum.R9:

                            if (INI.ISHIVECLIENT)
                            {
                                //string strhiveclientpathfile = @"C:\hive\hiveclient\hiveclient.exe";
                                //if (!File.Exists(strhiveclientpathfile))
                                //{
                                //    InitialErrorString = "Hiveclient soft 未在本機安裝及配置";
                                //    //return false;
                                //}

                                JZHIVECLIENT = new JzHiveClass();
                                //string strconfigpathfile = @"C:\hive\hiveclient\config\machineconfig.json";
                                //if (!File.Exists(strconfigpathfile))
                                {
                                    JZHIVECLIENT.PublisherIDOrMachineID = INI.HIVE_publisher_id;
                                    JZHIVECLIENT.Hiveclient_Init(INI.HIVE_site,
                                                                                        INI.HIVE_building,
                                                                                        INI.HIVE_line_type,
                                                                                        INI.HIVE_line,
                                                                                        INI.HIVE_station_type,
                                                                                        INI.HIVE_station_instance,
                                                                                        INI.HIVE_vendor);
                                }

                                ((JzR9MachineClass)MACHINECollection.MACHINE).SetMachineState(MachineState.Idle);
                            }

                            break;
                        case OptionEnum.R5:

                            if (INI.ISHIVECLIENT)
                            {
                                JZHIVECLIENT = new JzHiveClass();
                                {
                                    JZHIVECLIENT.PublisherIDOrMachineID = INI.HIVE_publisher_id;
                                    JZHIVECLIENT.Hiveclient_Init(INI.HIVE_site,
                                                                                        INI.HIVE_building,
                                                                                        INI.HIVE_line_type,
                                                                                        INI.HIVE_line,
                                                                                        INI.HIVE_station_type,
                                                                                        INI.HIVE_station_instance,
                                                                                        INI.HIVE_vendor);
                                }

                                ((JzR5MachineClass)MACHINECollection.MACHINE).SetMachineState(MachineState.Idle);
                            }

                            break;
                        case OptionEnum.R1:

                            if (INI.ISHIVECLIENT)
                            {
                                //string strhiveclientpathfile = @"C:\hive\hiveclient\hiveclient.exe";
                                //if (!File.Exists(strhiveclientpathfile))
                                //{
                                //    InitialErrorString = "Hiveclient soft 未在本機安裝及配置";
                                //    //return false;
                                //}

                                JZHIVECLIENT = new JzHiveClass();
                                //string strconfigpathfile = @"C:\hive\hiveclient\config\machineconfig.json";
                                //if (!File.Exists(strconfigpathfile))
                                {
                                    JZHIVECLIENT.PublisherIDOrMachineID = INI.HIVE_publisher_id;
                                    JZHIVECLIENT.Hiveclient_Init(INI.HIVE_site,
                                                                                        INI.HIVE_building,
                                                                                        INI.HIVE_line_type,
                                                                                        INI.HIVE_line,
                                                                                        INI.HIVE_station_type,
                                                                                        INI.HIVE_station_instance,
                                                                                        INI.HIVE_vendor);
                                }

                                ((JzR9MachineClass)MACHINECollection.MACHINE).SetMachineState(MachineState.Idle);
                            }

                            break;
                        case OptionEnum.R3:

                            if (INI.ISHIVECLIENT)
                            {
                                //string strhiveclientpathfile = @"C:\hive\hiveclient\hiveclient.exe";
                                //if (!File.Exists(strhiveclientpathfile))
                                //{
                                //    InitialErrorString = "Hiveclient soft 未在本機安裝及配置";
                                //    //return false;
                                //}

                                JZHIVECLIENT = new JzHiveClass();
                                //string strconfigpathfile = @"C:\hive\hiveclient\config\machineconfig.json";
                                //if (!File.Exists(strconfigpathfile))
                                {
                                    JZHIVECLIENT.PublisherIDOrMachineID = INI.HIVE_publisher_id;
                                    JZHIVECLIENT.Hiveclient_Init(INI.HIVE_site,
                                                                                        INI.HIVE_building,
                                                                                        INI.HIVE_line_type,
                                                                                        INI.HIVE_line,
                                                                                        INI.HIVE_station_type,
                                                                                        INI.HIVE_station_instance,
                                                                                        INI.HIVE_vendor);
                                }

                                ((JzR3MachineClass)MACHINECollection.MACHINE).SetMachineState(MachineState.Idle);
                            }

                            break;
                        case OptionEnum.C3:

                            if (INI.ISHIVECLIENT)
                            {
                                JZHIVECLIENT = new JzHiveClass();
                                //string strconfigpathfile = @"C:\hive\hiveclient\config\machineconfig.json";
                                //if (!File.Exists(strconfigpathfile))
                                {
                                    JZHIVECLIENT.PublisherIDOrMachineID = INI.HIVE_publisher_id;
                                    JZHIVECLIENT.Hiveclient_Init(INI.HIVE_site,
                                                                                        INI.HIVE_building,
                                                                                        INI.HIVE_line_type,
                                                                                        INI.HIVE_line,
                                                                                        INI.HIVE_station_type,
                                                                                        INI.HIVE_station_instance,
                                                                                        INI.HIVE_vendor);
                                }

                                ((JzC3MachineClass)MACHINECollection.MACHINE).SetMachineState(MachineState.Idle);
                            }

                            break;
                    }
                    break;
            }

            return ret;
        }
        static bool InitialJumboServer()
        {
            try
            {
                string serverpathstring = WORKPATH + "\\AllinOne.Jumbo.Client.config";
                IXCONNECTJUMBO = JumboNetClient.GetService2(serverpathstring);
                JUMBOSERVEREVENT = new AllinoneEvent();
            }
            catch (Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
                return false;
            }

            return true;
        }
        static bool InitialCrystalServer()
        {
            try
            {
                string serverpathstring = WORKPATH + "\\AllinOne.Crystal.Client.config";
                IXCONNECTCRYSTAL = CrystalNetClient.GetService(serverpathstring);
                CRYSTALSERVEREVENT = new AllinoneCrystalEvent();
            }
            catch (Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
                return false;
            }

            return true;
        }
        static void InitialASN()
        {
            ASNCollectionClass.ASNCollectionPath = $@"{PROGRAMME_ROOT}\JETEAZY\ASN";
            ASNCollection = new ASNCollectionClass(VERSION, OPTION);
            ASNCollection.Initial();
        }

        static void InitialKHC()
        {
            //KHCCollection = new KHCClass();
            //KHCCollection.Initial();
        }
        static bool InitialExtend()
        {
            bool ret = true;
            switch (VERSION)
            {
                case VersionEnum.ALLINONE:

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_X6:
                            CipExtend = new CipExtendClass(WORKPATH);
                            if (INI.IsOpenCip)
                            {
                                ret = CipExtend.Init();
                            }

                            break;
                    }

                    break;
            }

            return ret;
        }
        public static int InitialChangeRecipe()
        {
            int ret = 0;
            switch (VERSION)
            {
                case VersionEnum.ALLINONE:

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_SDM5:
                            JzMVDJudgeRecipe = new JzMVDJudgeRecipeClass(PATTERNPATH, INI.pMatchType);
                            if (INI.IsOpenAutoChangeRecipe)
                            {
                                JzMVDJudgeRecipe.MTTolerance = INI.fTolerance;
                                ret = JzMVDJudgeRecipe.Init();


                                //string testpath = "C:\\Users\\SuperGaara\\Desktop\\testtmp\\P00-000-x3.bmp";
                                //Bitmap bb = new Bitmap(testpath);
                                //Bitmap aa = new Bitmap(bb);
                                //bb.Dispose();
                                //string _nameStr = JzMVDJudgeRecipe.GetRecipeName(aa);
                            }



                            break;
                    }

                    break;
            }

            return ret;
        }
        static bool InitialMachineCollection()
        {
            bool ret = true;

            string opstr = "";

            switch (VERSION)
            {
                case VersionEnum.ALLINONE:

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN:
                            opstr += "1,";  //一個 PLC
                            opstr += "3,";  //三個 軸

                            JzAllinoneMachineClass jzallinonemachine = new JzAllinoneMachineClass(Machine_EA.Allinone, VERSION, OPTION, opstr, WORKPATH, IsNoUseIO);

                            jzallinonemachine.Initial(IsNoUseIO, IsNoUseMotor);

                            MACHINECollection = new MachineCollectionClass();
                            MACHINECollection.Intial(VERSION, OPTION, jzallinonemachine);
                            break;
                        case OptionEnum.R32:
                            opstr += "1,";  //一個 PLC
                            opstr += "0,";  //零個 軸

                            JzR32MachineClass jzr32machine = new JzR32MachineClass(Machine_EA.R32, VERSION, OPTION, opstr, WORKPATH, IsNoUseIO);

                            jzr32machine.Initial(IsNoUseIO, IsNoUseMotor);

                            MACHINECollection = new MachineCollectionClass();
                            MACHINECollection.Intial(VERSION, OPTION, jzr32machine);


                            break;
                        case OptionEnum.R26:
                            opstr += "1,";  //一個 PLC
                            opstr += "0,";  //零個 軸

                            JzRXXMachineClass jzrxxmachine = new JzRXXMachineClass(Machine_EA.RXX, VERSION, OPTION, opstr, WORKPATH, IsNoUseIO);

                            jzrxxmachine.Initial(IsNoUseIO, IsNoUseMotor);

                            MACHINECollection = new MachineCollectionClass();
                            MACHINECollection.Intial(VERSION, OPTION, jzrxxmachine);


                            break;
                        case OptionEnum.R15:
                            opstr += "1,";  //一個 PLC
                            opstr += "0,";  //零個 軸

                            JzR15MachineClass jzr15machine = new JzR15MachineClass(Machine_EA.R15, VERSION, OPTION, opstr, WORKPATH, IsNoUseIO);

                            jzr15machine.Initial(IsNoUseIO, IsNoUseMotor);

                            MACHINECollection = new MachineCollectionClass();
                            MACHINECollection.Intial(VERSION, OPTION, jzr15machine);


                            break;
                        case OptionEnum.R9:
                            opstr += "1,";  //一個 PLC
                            opstr += "0,";  //零個 軸

                            JzR9MachineClass jzr9machine = new JzR9MachineClass(Machine_EA.R9, VERSION, OPTION, opstr, WORKPATH, IsNoUseIO);

                            jzr9machine.Initial(IsNoUseIO, IsNoUseMotor);

                            MACHINECollection = new MachineCollectionClass();
                            MACHINECollection.Intial(VERSION, OPTION, jzr9machine);


                            break;
                        case OptionEnum.R5:
                            opstr += "1,";  //一個 PLC
                            opstr += "0,";  //零個 軸

                            JzR5MachineClass jzr5machine = new JzR5MachineClass(Machine_EA.R5, VERSION, OPTION, opstr, WORKPATH, IsNoUseIO);

                            jzr5machine.Initial(IsNoUseIO, IsNoUseMotor);

                            MACHINECollection = new MachineCollectionClass();
                            MACHINECollection.Intial(VERSION, OPTION, jzr5machine);


                            break;
                        case OptionEnum.R1:
                            opstr += "1,";  //一個 PLC
                            opstr += "0,";  //零個 軸

                            JzR1MachineClass jzr1machine = new JzR1MachineClass(Machine_EA.R1, VERSION, OPTION, opstr, WORKPATH, IsNoUseIO);

                            jzr1machine.Initial(IsNoUseIO, IsNoUseMotor);

                            MACHINECollection = new MachineCollectionClass();
                            MACHINECollection.Intial(VERSION, OPTION, jzr1machine);


                            break;
                        case OptionEnum.R3:
                            opstr += "1,";  //一個 PLC
                            opstr += "0,";  //零個 軸

                            JzR3MachineClass jzr3machine = new JzR3MachineClass(Machine_EA.R3, VERSION, OPTION, opstr, WORKPATH, IsNoUseIO);

                            jzr3machine.Initial(IsNoUseIO, IsNoUseMotor);

                            MACHINECollection = new MachineCollectionClass();
                            MACHINECollection.Intial(VERSION, OPTION, jzr3machine);

                            break;
                        case OptionEnum.C3:
                            opstr += "1,";  //一個 PLC
                            opstr += "0,";  //零個 軸

                            JzC3MachineClass jzC3machine = new JzC3MachineClass(Machine_EA.C3, VERSION, OPTION, opstr, WORKPATH, IsNoUseIO);

                            jzC3machine.Initial(IsNoUseIO, IsNoUseMotor);

                            MACHINECollection = new MachineCollectionClass();
                            MACHINECollection.Intial(VERSION, OPTION, jzC3machine);

                            break;

                        case OptionEnum.MAIN_SD:

                            opstr += "1,";  //一個 PLC
                            opstr += "8,";  //零個 軸

                            JzMainSDMachineClass jzMainSDmachine = new JzMainSDMachineClass(Machine_EA.MAIN_SD, VERSION, OPTION, opstr, WORKPATH, IsNoUseIO);

                            jzMainSDmachine.Initial(IsNoUseIO, IsNoUseMotor);

                            MACHINECollection = new MachineCollectionClass();
                            MACHINECollection.Intial(VERSION, OPTION, jzMainSDmachine);

                            JZMAINSDPOSITIONPARA = new JzMainSDPositionParaClass(Universal.WORKPATH + "\\pos");
                            JZMAINSDPOSITIONPARA.Initial();

                            break;
                        case OptionEnum.MAIN_X6:

                            opstr += "1,";  //一個 PLC
                            opstr += "0,";  //零個 軸
                            opstr += "1,";  //一个灯 控制器

                            JzMainX6MachineClass jzMainX6machine = new JzMainX6MachineClass(Machine_EA.MAIN_X6, VERSION, OPTION, CAMACT, opstr, WORKPATH, IsNoUseIO);

                            ret = jzMainX6machine.Initial(IsNoUseIO, IsNoUseMotor);

                            MACHINECollection = new MachineCollectionClass();
                            MACHINECollection.Intial(VERSION, OPTION, jzMainX6machine);

                            JZMAINSDPOSITIONPARA = new JzMainSDPositionParaClass(Universal.WORKPATH + "\\pos");
                            JZMAINSDPOSITIONPARA.Initial();

                            //JZMAINSDPOSITIONPARA.SaveMySqlControl();
                            JZMAINSDPOSITIONPARA.SetLogPath("D:\\log\\log.db.collect");
                            JZMAINSDPOSITIONPARA.OpenDB();

                            //JZMAINSDPOSITIONPARA.MySqlCreateTable();
                            //JZMAINSDPOSITIONPARA.MySqlTableInsert("TT000ZQ01");
                            //JZMAINSDPOSITIONPARA.MySqlTableInsert("TT000ZQ02");
                            //JZMAINSDPOSITIONPARA.MySqlTableInsert("TT000ZQ01");

                            break;

                        case OptionEnum.MAIN_SDM1:

                            opstr += "1,";  //一個 PLC
                            opstr += "1,";  //零個 軸

                            JzMainSDM1MachineClass jzMainSDM1machine = new JzMainSDM1MachineClass(Machine_EA.MAIN_SDM1, VERSION, OPTION, opstr, WORKPATH, IsNoUseIO);

                            jzMainSDM1machine.Initial(IsNoUseIO, IsNoUseMotor);

                            MACHINECollection = new MachineCollectionClass();
                            MACHINECollection.Intial(VERSION, OPTION, jzMainSDM1machine);

                            //JZMAINSDPOSITIONPARA = new JzMainSDPositionParaClass(Universal.WORKPATH + "\\pos");
                            //JZMAINSDPOSITIONPARA.Initial();

                            break;
                        case OptionEnum.MAIN_SDM2:

                            switch (myRobotType)
                            {
                                case RobotType.HCFA:
                                    opstr += "1,";  //一個 PLC
                                    opstr += "4,";  //零個 軸
                                    opstr += "HCFA0,";  //机械臂
                                    break;
                                case RobotType.NONE:
                                default:
                                    opstr += "1,";  //一個 PLC
                                    opstr += "4,";  //零個 軸
                                    opstr += "EPSON,";  //机械臂
                                    break;
                            }

                            JzMainSDM2MachineClass jzMainSDM2machine = new JzMainSDM2MachineClass(Machine_EA.MAIN_SDM2, VERSION, OPTION, opstr, WORKPATH, IsNoUseIO);

                            jzMainSDM2machine.Initial(IsNoUseIO, IsNoUseMotor);

                            MACHINECollection = new MachineCollectionClass();
                            MACHINECollection.Intial(VERSION, OPTION, jzMainSDM2machine);

                            //JZMAINSDPOSITIONPARA = new JzMainSDPositionParaClass(Universal.WORKPATH + "\\pos");
                            //JZMAINSDPOSITIONPARA.Initial();

                            break;
                        case OptionEnum.MAIN_SDM3:

                            switch (myRobotType)
                            {
                                case RobotType.HCFA:
                                    opstr += "1,";  //一個 PLC
                                    opstr += "4,";  //零個 軸
                                    opstr += "HCFA0,";  //机械臂
                                    break;
                                case RobotType.NONE:
                                default:
                                    opstr += "1,";  //一個 PLC
                                    opstr += "4,";  //零個 軸
                                    opstr += "EPSON,";  //机械臂
                                    break;
                            }

                            JzMainSDM3MachineClass jzMainSDM3machine = new JzMainSDM3MachineClass(Machine_EA.MAIN_SDM3, VERSION, OPTION, opstr, WORKPATH, IsNoUseIO);

                            jzMainSDM3machine.Initial(IsNoUseIO, IsNoUseMotor);

                            MACHINECollection = new MachineCollectionClass();
                            MACHINECollection.Intial(VERSION, OPTION, jzMainSDM3machine);

                            //JZMAINSDPOSITIONPARA = new JzMainSDPositionParaClass(Universal.WORKPATH + "\\pos");
                            //JZMAINSDPOSITIONPARA.Initial();

                            break;
                        case OptionEnum.MAIN_SDM5:

                            opstr += "1,";  //一個 PLC
                            opstr += "4,";  //零個 軸

                            JzMainSDM5MachineClass jzMainSDM5machine = new JzMainSDM5MachineClass(Machine_EA.MAIN_SDM5, opstr, WORKPATH, IsNoUseIO);

                            jzMainSDM5machine.Initial(IsNoUseIO, IsNoUseMotor);

                            MACHINECollection = new MachineCollectionClass();
                            MACHINECollection.Intial(VERSION, OPTION, jzMainSDM5machine);

                            JZMAINSDPOSITIONPARA = new JzMainSDPositionParaClass(Universal.WORKPATH + "\\pos");
                            JZMAINSDPOSITIONPARA.Initial();

                            break;
                        case OptionEnum.MAIN_SERVICE:

                            opstr += "1,";  //一個 PLC
                            opstr += "0,";  //零個 軸
                            opstr += "1,";  //一个灯 控制器

                            JzMainServiceMachineClass jzMainServicemachine = new JzMainServiceMachineClass(Machine_EA.MAIN_SERVICE, VERSION, OPTION, CAMACT, opstr, WORKPATH, IsNoUseIO);

                            ret = jzMainServicemachine.Initial(IsNoUseIO, IsNoUseMotor);

                            MACHINECollection = new MachineCollectionClass();
                            MACHINECollection.Intial(VERSION, OPTION, jzMainServicemachine);

                            JZMAINSDPOSITIONPARA = new JzMainSDPositionParaClass(Universal.WORKPATH + "\\pos");
                            JZMAINSDPOSITIONPARA.Initial();

                            break;

                    }

                    break;
                case VersionEnum.AUDIX:

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_DFLY:
                            opstr += "1,";  //1個 PLC  
                            opstr += "3,";   //3個軸

                            JzDFlyMachineClass jzdflymachine = new JzDFlyMachineClass(Machine_EA.AudixDfly, VERSION, OPTION, opstr, WORKPATH, IsNoUseIO);
                            jzdflymachine.Initial(IsNoUseIO, IsNoUseMotor);

                            MACHINECollection = new MachineCollectionClass();
                            MACHINECollection.Intial(VERSION, OPTION, jzdflymachine);

                            break;
                        case OptionEnum.MAIN:
                            opstr += "0,";  //0個 PLC  
                            opstr += "0,";   //0個軸

                            JzAudixMachineClass jzaudixmachine = new JzAudixMachineClass(Machine_EA.Audix, VERSION, OPTION, opstr, WORKPATH, IsNoUseIO);
                            jzaudixmachine.Initial(IsNoUseIO, IsNoUseMotor);

                            MACHINECollection = new MachineCollectionClass();
                            MACHINECollection.Intial(VERSION, OPTION, jzaudixmachine);

                            break;
                    }
                    break;
                default:

                    //opstr += "0,";  //0個 PLC  
                    //opstr += "0,";   //0個軸

                    //JzAudixMachineClass jzaudixmachine = new JzAudixMachineClass(Machine_EA.Audix, VERSION, OPTION, opstr, WORKPATH, IsSimulator);

                    //MACHINECollection = new MachineCollectionClass();
                    //MACHINECollection.Intial(VERSION, OPTION, jzaudixmachine);

                    break;
            }

            return ret;
        }
        static bool InitialCCD()
        {
            bool ret = true;

            //这里加入线扫
            switch (CAMACT)
            {
                case CameraActionMode.CAM_MOTOR_LINESCAN:

                    JetEazy.CCDSpace.CameraConfig.Instance.Initial(WORKPATH);
                    //switch (LINESCANTYPE)
                    //{
                    //    case LinescanTypeEnum.HUARUI:
                    //        IxLineScan = new LINESCAN_HUARUI();
                    //        break;
                    //    case LinescanTypeEnum.DVP2:
                    //        IxLineScan = new Linescan_Dvp2();
                    //        break;
                    //}
                    IxLineScan = new Linescan_Dvp2();
                    IxLineScan.Init(JetEazy.CCDSpace.CameraConfig.Instance.cameras[0].IsDebug,
                                    JetEazy.CCDSpace.CameraConfig.Instance.cameras[0].ToCameraString());
                    ret = IxLineScan.Open();


                    if (ret)
                    {
                        if (INI.IsOpenAutoChangeRecipe)
                        {
                            IxAreaCam = new Linescan_Dvp2();
                            IxAreaCam.Init(JetEazy.CCDSpace.CameraConfig.Instance.cameras[1].IsDebug,
                                           JetEazy.CCDSpace.CameraConfig.Instance.cameras[1].ToCameraString());
                            ret = IxAreaCam.Open();


                            //IxAreaCam.SoftTrigger();
                        }
                    }


                    break;
            }

            CCDCollection = new CCDCollectionClass(WORKPATH, IsNoUseCCD, VERSION, OPTION);

            ret = CCDCollection.Initial(WORKPATH);

            if (ret)
                CCDCollection.GetBmpAll(-2);

            return ret;
        }
        static void CreateDebugDirectories()
        {
            if (!Directory.Exists(DATAREPORTPATH))
                Directory.CreateDirectory(DATAREPORTPATH);


            if (!Directory.Exists(MainSDM2NG_Path))
                Directory.CreateDirectory(MainSDM2NG_Path);

            if (!Directory.Exists(DEBUGRAWPATH))
                Directory.CreateDirectory(DEBUGRAWPATH);

            if (!Directory.Exists(DEBUGSRCPATH))
                Directory.CreateDirectory(DEBUGSRCPATH);

            if (!Directory.Exists(LOGDBPATH))
                Directory.CreateDirectory(LOGDBPATH);

            if (!Directory.Exists(BACKUPDBPATH))
                Directory.CreateDirectory(BACKUPDBPATH);

            if (!Directory.Exists(LOGTXTPATH))
                Directory.CreateDirectory(LOGTXTPATH);

            if (!Directory.Exists(DEBUGRESULTPATH))
                Directory.CreateDirectory(DEBUGRESULTPATH);

            if (!Directory.Exists(TESTPATH))
                Directory.CreateDirectory(TESTPATH);

            if (!Directory.Exists(TESTPATH + "\\ANALYZETEST"))
                Directory.CreateDirectory(TESTPATH + "\\ANALYZETEST");

            if (!Directory.Exists(INI.SHOPFLOORPATH))
                Directory.CreateDirectory(INI.SHOPFLOORPATH);

            if (!Directory.Exists("D:\\report\\DataRecord"))
                Directory.CreateDirectory("D:\\report\\DataRecord");
        }
        public static void Close()
        {
            CCDCollection.Close();
            switch (OPTION)
            {
                case OptionEnum.MAIN_SDM5:
                case OptionEnum.MAIN_X6:
                    //if (IsUseThreadReviceTcp)
                    //    jzMainX6Result.stop_scan_thread();

                    if (INI.bUse2DCNNReader)
                    {
                        CodeBuilder?.Dispose();
                    }

                    switch (CAMACT)
                    {
                        case CameraActionMode.CAM_MOTOR_MODE2:
                            break;
                        case CameraActionMode.CAM_MOTOR_LINESCAN:

                            IxLineScan.Close();

                            break;
                    }

                    if (INI.IsOpenCip)
                    {
                        CipExtend.Close();
                    }

                    break;

                case OptionEnum.MAIN_SDM2:
                    AIClose();
                    break;
            }
        }

        public static void BackupDATADB()
        {
            #region Add Backup fuctions
            //Should Add 7z for it

            string BackupFileName = Universal.BACKUPDBPATH + "\\" + JzTimes.DateTimeSerialString + "-" + ACCDB.DataNow.Name + "-DATA.mdb";
            File.Copy(DBPATH + "\\DATA.mdb", BackupFileName);

            #endregion

        }
        /// <summary>
        /// 自动跑线
        /// </summary>
        public static bool isAutoDebug = false;
        /// <summary>
        /// 文件夹路径
        /// </summary>
        public static string FolderPath = "";
        /// <summary>
        /// 文件夹路径
        /// </summary>
        public static string FolderName = "";
        /// <summary>
        /// 复文件夹
        /// </summary>
        /// <param name="from">源路径</param>
        /// <param name="to">新的路径</param>
        public static void CopyFolder(string from, string to)
        {
            if (!Directory.Exists(to))
                Directory.CreateDirectory(to);

            // 子文件夹
            foreach (string sub in Directory.GetDirectories(from))
                CopyFolder(sub + "\\", to + Path.GetFileName(sub) + "\\");

            // 文件
            foreach (string file in Directory.GetFiles(from))
                File.Copy(file, to + Path.GetFileName(file), true);
        }
        /// <summary>
        /// 如果文件被占用 则强制删除
        /// </summary>
        /// <param name="filename">文件地址</param>
        /// <param name="timesToWrite"></param>
        public static void WipeFile(string filename, int timesToWrite)
        {
            try
            {
                if (File.Exists(filename))
                {
                    //设置文件的属性为正常，这是为了防止文件是只读
                    File.SetAttributes(filename, FileAttributes.Normal);
                    //计算扇区数目
                    double sectors = Math.Ceiling(new FileInfo(filename).Length / 512.0);
                    // 创建一个同样大小的虚拟缓存
                    byte[] dummyBuffer = new byte[512];
                    // 创建一个加密随机数目生成器
                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                    // 打开这个文件的FileStream
                    FileStream inputStream = new FileStream(filename, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
                    for (int currentPass = 0; currentPass < timesToWrite; currentPass++)
                    {
                        // 文件流位置
                        inputStream.Position = 0;
                        //循环所有的扇区
                        for (int sectorsWritten = 0; sectorsWritten < sectors; sectorsWritten++)
                        {
                            //把垃圾数据填充到流中
                            rng.GetBytes(dummyBuffer);
                            // 写入文件流中
                            inputStream.Write(dummyBuffer, 0, dummyBuffer.Length);
                        }
                    }
                    // 清空文件
                    inputStream.SetLength(0);
                    // 关闭文件流
                    inputStream.Close();
                    // 清空原始日期需要
                    DateTime dt = new DateTime(2037, 1, 1, 0, 0, 0);
                    File.SetCreationTime(filename, dt);
                    File.SetLastAccessTime(filename, dt);
                    File.SetLastWriteTime(filename, dt);
                    // 删除文件
                    File.Delete(filename);
                }
            }
            catch (Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
            }
        }

        #region AI Model

        //const string IP = "127.0.0.1";
        //const int PORT = 9001;
        public static IEzSeg model = null;
        static bool AIConnect()
        {
            if (!INI.chipUseAI)
                return true;

            bool ret = false;
            try
            {
                _TRACE("連線中...");
                model = EzSegClientFactory.OpenConnection(INI.AI_IP, INI.AI_Port);
                //加载模型
                if (File.Exists(INI.AI_Model_FilenamePath))
                {
                    model.LoadModelFile(INI.AI_Model_FilenamePath);
                }
                string version = model.GetVersion();
                string modelName = model.GetCurrentModelName();
                var err = model.SwitchModel(ModelCategory.Baseline);
                if (!err.Is(Errcode.OK))
                {
                    MessageBox.Show($"异常:({err.errCode},{err.errMsg})", "AI Init", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                _TRACE($"版本號 = {version}");
                _TRACE($"訓練模型名稱 = {modelName}");
                ret = true;
            }
            catch (Exception ex)
            {
                _TRACE(ex.ToString());

            }
            return ret;
        }
        static void AIClose()
        {
            if (!INI.chipUseAI)
                return;

            if (model != null)
            {
                _TRACE("關閉連線中...");
                model.Dispose();
            }
        }

        static void _TRACE(string msg)
        {
            Console.WriteLine(msg);
        }

        #endregion

        #region Tool Region

        static void ReadData(ref string DataStr, string FileName)
        {
            FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.None);
            StreamReader Srr = new StreamReader(fs, Encoding.Default);

            DataStr = Srr.ReadToEnd();

            Srr.Close();
            Srr.Dispose();
        }

        static void TestCalibration()
        {
            List<string> m_list = new List<string>();
            System.IO.StreamReader sr = new StreamReader(@"D:\LOA\20211103105730.csv");
            sr.ReadLine();
            while (!sr.EndOfStream)
            {

                m_list.Add(sr.ReadLine());
                //string[] strs = sr.ReadLine().Split(',');
                //if (strs.Length >= 5)
                //{

                //}
            }

            sr.Close();
            sr.Dispose();



        }

        static int GetWeekNumber()
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return weekNum;
        }

        #endregion

        static string ToChangeLanguage(string eText)
        {
            string retStr = eText;
            retStr = LanguageExClass.Instance.GetLanguageText(eText);
            return retStr;
        }
        static string ToChangeLanguageCode(string eName)
        {
            string retStr = eName;
            retStr = LanguageExClass.Instance.GetLanguageIDName(eName);
            return retStr;
        }

        #region TestProgram

        static void TestProgram()
        {
            //List<AnalyzeClass> AList = new List<AnalyzeClass>();
            //List<AnalyzeClass> BList = new List<AnalyzeClass>();

            //AList.Add(new AnalyzeClass());
            //AList[0].AliasName = "FUCK";
            //AList.Add(new AnalyzeClass());
            //AList[1].AliasName = "DAMN";
            //AList.Add(new AnalyzeClass());
            //AList[2].AliasName = "SHIT";

            //foreach (AnalyzeClass analye in AList)
            //{
            //    BList.Add(analye);
            //}

            //AList.Clear();

            PointF p0 = new PointF(0, 0);
            PointF p0run = new PointF(0, 1);
            PointF p1 = new PointF(10, 0);
            PointF p1run = new PointF(10, 1);
            //PointF OrgCenter = new PointF(5, 0);
            //PointF RunCenter = new PointF(5, 1);

            PointF ptfpatternORG = new PointF(5,0);
            PointF ptfpatternRUN = new PointF(5,2);

            // 创建坐标系转换器
            var coordSystem0 = new MarkCoordinateSystem();
            coordSystem0.Initialize(p0, p1);
            PointF org = coordSystem0.WorldToMarkCoordinates(ptfpatternORG);

            coordSystem0.Initialize(p0run, p1run);
            PointF run = coordSystem0.WorldToMarkCoordinates(ptfpatternRUN);

            double xshiftrunxx = Math.Abs(org.X - run.X);
            double yshiftrunyy = Math.Abs(org.Y - run.Y);

        }

        #endregion

        #region R3UI的画面资料
        public static R3UICLASS R3UI = new R3UICLASS();

        public static bool isR3ByPass = false;
        public static Stopwatch watchR3RyPass = new Stopwatch();


        public delegate void R3WatchStopTick(string movestring);
        public static event R3WatchStopTick R3TickStop;
        public static void OnR3TickStop(string movestring)
        {
            if (R3TickStop != null)
            {
                R3TickStop(movestring);
            }
        }
        #endregion
        #region R3UI的画面资料
        public static C3UICLASS C3UI = new C3UICLASS();

        public static bool isC3ByPass = false;
        public static Stopwatch watchC3RyPass = new Stopwatch();


        public delegate void C3WatchStopTick(string movestring);
        public static event C3WatchStopTick C3TickStop;
        public static void OnC3TickStop(string movestring)
        {
            if (C3TickStop != null)
            {
                C3TickStop(movestring);
            }
        }
        #endregion
    }
    public class R3UICLASS
    {
        public Bitmap bmpL { get; set; }
        public Bitmap bmpR { get; set; }
        public Bitmap bmpC { get; set; }
        public RectangleF RectST { get; set; }
        public RectangleF RectSN { get; set; }
        public Bitmap bmpBarcode { get; set; }

        public bool isBarcode { get; set; }
        public bool isSNResult { get; set; }
        public bool isTest { get; set; }

        public bool IsPass { get; set; }

        public string Barcode1D { get; set; }

        public Bitmap bmpResult { get; set; }

        public bool isSNHaveS = false;

        public bool isBYPASS = false;

        public Bitmap bmpBarcodeCHECKERR { get; set; }
        public bool isCheckBarcodeErr = false;
    }
    public class C3UICLASS
    {
        public Bitmap bmpL { get; set; }
        public Bitmap bmpR { get; set; }
        public Bitmap bmpC { get; set; }
        public RectangleF RectST { get; set; }
        public RectangleF RectSN { get; set; }
        public Bitmap bmpBarcode { get; set; }

        public Bitmap bmpLogo { get; set; }

        public Bitmap bmpSN1 { get; set; }
        public Bitmap bmpSN2 { get; set; }

        public Bitmap bmpLabel { get; set; }
        /// <summary>
        /// SN前面的位置
        /// </summary>
        public PointF pSN1 { get; set; }
        /// <summary>
        /// 临近SN的位置
        /// </summary>
        public PointF pSN2 { get; set; }

        public bool isBarcode { get; set; }
        public bool isSNResult { get; set; }
        public bool isTest { get; set; }

        public bool IsPass { get; set; }

        public string Barcode1D { get; set; }

        public Bitmap bmpResult { get; set; }

        public bool isSNHaveS = false;

        public bool isBYPASS = false;

        public Bitmap bmpBarcodeCHECKERR { get; set; }
        public bool isCheckBarcodeErr = false;
    }
}
