
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using AForge.Imaging.Filters;
using Allinone.FormSpace;
using JetEazy.BasicSpace;
using JetEazy.UISpace;

namespace Allinone.BasicSpace
{
    public enum BlobMode
    {
        /// <summary>
        /// 找黑斑
        /// </summary>
        [Description("找黑斑")]
        Black,
        /// <summary>
        /// 找白斑
        /// </summary>
        [Description("找白斑")]
        White,
    }
    public enum BlockDir
    {
        /// <summary>
        /// 水平方向
        /// </summary>
        //[Description("水平方向")]
        HORIZONTAL,
        /// <summary>
        /// 垂直方向
        /// </summary>
        //[Description("垂直方向")]
        VERTICAL,
    }
    public class GetPlugsPropertyEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext pContext)
        {
            if (pContext != null && pContext.Instance != null)
            {
                //以「...」按鈕的方式顯示
                //UITypeEditorEditStyle.DropDown    下拉選單
                //UITypeEditorEditStyle.None        預設的輸入欄位
                return UITypeEditorEditStyle.Modal;
            }
            return base.GetEditStyle(pContext);
        }
        public override object EditValue(ITypeDescriptorContext pContext, IServiceProvider pProvider, object pValue)
        {
            string str = "";
            IWindowsFormsEditorService editorService = null;
            if (pContext != null && pContext.Instance != null && pProvider != null)
            {
                editorService = (IWindowsFormsEditorService)pProvider.GetService(typeof(IWindowsFormsEditorService));
                if (editorService != null)
                {
                    switch (pContext.PropertyDescriptor.Name)
                    {
                        case "HIVE_exe_path":
                            str = OpenFilePicker("", "hiveagent.exe");
                            if (!string.IsNullOrEmpty(str))
                                pValue = str;
                            break;
                        case "RootPath":
                            str = PathPicker("", pValue.ToString());
                            if (!string.IsNullOrEmpty(str))
                                pValue = str;
                            break;
                        case "HIVE_rectangle_corp":
                            m_frmCorpBmp = new frmCorpBmp((System.Drawing.Rectangle)pValue);
                            if (DialogResult.OK == m_frmCorpBmp.ShowDialog())
                            {
                                pValue = m_frmCorpBmp.GetResult();
                            }
                            break;
                    }
                }
            }
            return pValue;
        }

        frmCorpBmp m_frmCorpBmp;

        public string OpenFilePicker(string DefaultPath, string DefaultName)
        {
            string retStr = "";

            OpenFileDialog dlg = new OpenFileDialog();

            //dlg.Filter = "BMP Files (*.bmp)|*.BMP|" + "All files (*.*)|*.*";
            dlg.Filter = DefaultPath;
            dlg.FileName = DefaultName;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                retStr = dlg.FileName;
            }
            return retStr;
        }
        public string PathPicker(string Description, string DefaultPath)
        {
            string retStr = "";

            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.Description = Description;
            fd.ShowNewFolderButton = false;
            fd.SelectedPath = DefaultPath;

            if (fd.ShowDialog().Equals(DialogResult.OK))
            {
                if (fd.SelectedPath != "")
                    retStr = fd.SelectedPath;
            }
            else
                retStr = "";

            return retStr;
        }
    }
    class JzINIPropertyGridClass
    {
        public JzINIPropertyGridClass()
        {

        }

        [CategoryAttribute("Basic Control"), DescriptionAttribute("检查镭雕sn的错误代码")]
        public string CHECKSNERRORCODE
        {
            get { return INI.CHECKSNERRORCODE; }
            set { INI.CHECKSNERRORCODE = value; }
        }
        [CategoryAttribute("Hiveclient Control"), DescriptionAttribute("是否啓用HIVE功能")]
        public bool ISHIVECLIENT
        {
            get { return INI.ISHIVECLIENT; }
            set { INI.ISHIVECLIENT = value; }
        }
        [CategoryAttribute("Hiveclient Control"), DescriptionAttribute("发布者的HIVE_ID")]
        public string HIVE_publisher_id
        {
            get { return INI.HIVE_publisher_id; }
            set { INI.HIVE_publisher_id = value; }
        }
        [CategoryAttribute("Hiveclient Control"), DescriptionAttribute("站点")]
        public string HIVE_site
        {
            get { return INI.HIVE_site; }
            set { INI.HIVE_site = value; }
        }
        [CategoryAttribute("Hiveclient Control"), DescriptionAttribute("厂区楼号")]
        public string HIVE_building
        {
            get { return INI.HIVE_building; }
            set { INI.HIVE_building = value; }
        }
        [CategoryAttribute("Hiveclient Control"), DescriptionAttribute("线别类型")]
        public string HIVE_line_type
        {
            get { return INI.HIVE_line_type; }
            set { INI.HIVE_line_type = value; }
        }
        [CategoryAttribute("Hiveclient Control"), DescriptionAttribute("线别")]
        public string HIVE_line
        {
            get { return INI.HIVE_line; }
            set { INI.HIVE_line = value; }
        }
        [CategoryAttribute("Hiveclient Control"), DescriptionAttribute("站别类型")]
        public string HIVE_station_type
        {
            get { return INI.HIVE_station_type; }
            set { INI.HIVE_station_type = value; }
        }
        [CategoryAttribute("Hiveclient Control"), DescriptionAttribute("站别")]
        public string HIVE_station_instance
        {
            get { return INI.HIVE_station_instance; }
            set { INI.HIVE_station_instance = value; }
        }
        [CategoryAttribute("Hiveclient Control"), DescriptionAttribute("厂商名称")]
        public string HIVE_vendor
        {
            get { return INI.HIVE_vendor; }
            set { INI.HIVE_vendor = value; }
        }
        [CategoryAttribute("Hiveclient Control"), DescriptionAttribute("HIVE后台程序路径")]
        [Editor(typeof(GetPlugsPropertyEditor), typeof(UITypeEditor))]
        public string HIVE_exe_path
        {
            get { return INI.HIVE_exe_path; }
            set { INI.HIVE_exe_path = value; }
        }

        [CategoryAttribute("Hiveclient Control"), DescriptionAttribute("不同机种发送格式不同\r\n填写机种名称 eg:J313,J293")]
        public string HIVE_model
        {
            get { return INI.HIVE_model; }
            set
            {
                INI.HIVE_model = value;

                if (INI.ISHIVECLIENT)
                {
                    Universal.JZHIVECLIENT.Model = INI.HIVE_model;
                }

            }
        }
        [CategoryAttribute("Hiveclient Control"), DescriptionAttribute("是否本系统上传HIVE true:本系统 false:其他系统上传 eg.hiveagent")]
        [DisplayName("本系统上传HIVE")]
        public bool HIVE_IsLocalSystemUpload
        {
            get { return INI.HIVE_islocalsystemupload; }
            set
            {
                INI.HIVE_islocalsystemupload = value;

                if (INI.ISHIVECLIENT)
                {
                    Universal.JZHIVECLIENT.IsLocalSystemUpload = INI.HIVE_islocalsystemupload;
                }

            }
        }
        [CategoryAttribute("Hiveclient Control"), DescriptionAttribute("截取有效的镭雕区域")]
        [DisplayName("截取镭雕区域")]
        [Editor(typeof(GetPlugsPropertyEditor), typeof(UITypeEditor))]
        public System.Drawing.Rectangle HIVE_rectangle_corp
        {
            get { return INI.HIVE_rectangle_corp; }
            set
            {
                INI.HIVE_rectangle_corp = value;
            }
        }

        [CategoryAttribute("Data Collection Control"), DescriptionAttribute("")]
        public string DATA_Program
        {
            get { return INI.DATA_Program; }
            set { INI.DATA_Program = value; }
        }
        [CategoryAttribute("Data Collection Control"), DescriptionAttribute("")]
        public string DATA_Building_Config
        {
            get { return INI.DATA_Building_Config; }
            set { INI.DATA_Building_Config = value; }
        }
        [CategoryAttribute("Data Collection Control"), DescriptionAttribute("机种_KBOCR_线别_AOI_机器编号 \n eg.J152_KBOCR_D15_AOI_01")]
        public string DATA_FIXTUREID
        {
            get { return INI.DATA_FIXTUREID; }
            set { INI.DATA_FIXTUREID = value; }
        }
        [CategoryAttribute("Data Collection Control"), DescriptionAttribute("True:代表10颗螺丝[230] \r\n False:代表6颗螺丝[214,223,152]")]
        public bool DATA_SCREW_TEN
        {
            get { return INI.DATA_SCREW_TEN; }
            set { INI.DATA_SCREW_TEN = value; }
        }


        [CategoryAttribute("QFactory Control"), DescriptionAttribute("是否開啟QFactory")]
        public bool ISUSE_QFACTORY
        {
            get { return INI.ISUSE_QFACTORY; }
            set
            {
                INI.ISUSE_QFACTORY = value;
                if (INI.ISUSE_QFACTORY)
                {
                    if (Universal.JZQFACTORY == null)
                        Universal.JZQFACTORY = new JetEazy.BasicSpace.JzQFactoryClass();

                    Universal.JZQFACTORY.Init(INI.QFACTORY_EQ_SN,
                                                              INI.QFACTORY_EQ_LocationID,
                                                              INI.QFACTORY_EQ_LocationID2,
                                                              INI.QFACTORY_Station,
                                                              INI.QFACTORY_Step);
                }
                else
                {
                    if (Universal.JZQFACTORY != null)
                    {
                        Universal.JZQFACTORY.Dispose();
                        Universal.JZQFACTORY = null;
                    }
                }
            }
        }
        [CategoryAttribute("QFactory Control"), DescriptionAttribute("QFACTORY_EQ_SN")]
        public string QFACTORY_EQ_SN
        {
            get { return INI.QFACTORY_EQ_SN; }
            set { INI.QFACTORY_EQ_SN = value; }
        }
        [CategoryAttribute("QFactory Control"), DescriptionAttribute("QFACTORY_EQ_LocationID")]
        public string QFACTORY_EQ_LocationID
        {
            get { return INI.QFACTORY_EQ_LocationID; }
            set { INI.QFACTORY_EQ_LocationID = value; }
        }
        [CategoryAttribute("QFactory Control"), DescriptionAttribute("QFACTORY_EQ_LocationID2")]
        public string QFACTORY_EQ_LocationID2
        {
            get { return INI.QFACTORY_EQ_LocationID2; }
            set { INI.QFACTORY_EQ_LocationID2 = value; }
        }
        [CategoryAttribute("QFactory Control"), DescriptionAttribute("QFACTORY_Station")]
        public string QFACTORY_Station
        {
            get { return INI.QFACTORY_Station; }
            set { INI.QFACTORY_Station = value; }
        }
        [CategoryAttribute("QFactory Control"), DescriptionAttribute("QFACTORY_Step")]
        public string QFACTORY_Step
        {
            get { return INI.QFACTORY_Step; }
            set { INI.QFACTORY_Step = value; }
        }
        [CategoryAttribute("QFactory Control"), DescriptionAttribute("發送正常狀態的間隔時間[单位:分钟]")]
        public int QFACTORY_CHECK_TIME
        {
            get { return INI.QFACTORY_CHECK_TIME; }
            set { INI.QFACTORY_CHECK_TIME = value; }
        }

        [CategoryAttribute("MainSD Control"), DescriptionAttribute("用户设定满盒PASS数量")]
        [DisplayName("PASS满盒")]
        public int USER_SET_FULL_PASSCOUNT
        {
            get { return INI.USER_SET_FULL_PASSCOUNT; }
            set { INI.USER_SET_FULL_PASSCOUNT = value; }
        }

        [CategoryAttribute("MainSD Control"), DescriptionAttribute("用户设定满盒NG数量")]
        [DisplayName("NG满盒")]
        public int USER_SET_FULL_NGCOUNT
        {
            get { return INI.USER_SET_FULL_NGCOUNT; }
            set { INI.USER_SET_FULL_NGCOUNT = value; }
        }

        [CategoryAttribute("MainSD Control"), DescriptionAttribute("达到连续NG数目报警")]
        [DisplayName("连续NG数目")]
        public int CONTINUE_NG_COUNT
        {
            get { return INI.CONTINUE_NG_COUNT; }
            set { INI.CONTINUE_NG_COUNT = value; }
        }
        [CategoryAttribute("MainSD Control"), DescriptionAttribute("延时拍照时间 ms")]
        [DisplayName("延时拍照时间")]
        public int MAINSD_GETIMAGE_DELAYTIME
        {
            get { return INI.MAINSD_GETIMAGE_DELAYTIME; }
            set { INI.MAINSD_GETIMAGE_DELAYTIME = value; }
        }



    }
    class JzINIMAIN_X6PropertyGridClass
    {
        public JzINIMAIN_X6PropertyGridClass()
        {

        }

        const string cat0 = "00.MainX6 Comm Setup";

        [CategoryAttribute(cat0), DescriptionAttribute("连接打标服务器的ip地址")]
        [DisplayName("打标服务器IP")]
        public string tcp_ip
        {
            get { return INI.tcp_ip; }
            set { INI.tcp_ip = value; }
        }

        [CategoryAttribute(cat0), DescriptionAttribute("连接打标服务器的端口")]
        [DisplayName("打标服务器端口")]
        public int tcp_port
        {
            get { return INI.tcp_port; }
            set { INI.tcp_port = value; }
        }

        [CategoryAttribute(cat0), DescriptionAttribute("是否打开连接Handle服务器 true打开  false关闭")]
        [DisplayName("Handle服务器Open")]
        public bool tcp_handle_open
        {
            get { return INI.tcp_handle_open; }
            set { INI.tcp_handle_open = value; }
        }
        [CategoryAttribute(cat0), DescriptionAttribute("连接Handle服务器的ip地址")]
        [DisplayName("Handle服务器IP")]
        public string tcp_handle_ip
        {
            get { return INI.tcp_handle_ip; }
            set { INI.tcp_handle_ip = value; }
        }

        [CategoryAttribute(cat0), DescriptionAttribute("连接Handle服务器的端口")]
        [DisplayName("Handle服务器端口")]
        public int tcp_handle_port
        {
            get { return INI.tcp_handle_port; }
            set { INI.tcp_handle_port = value; }
        }

        [CategoryAttribute(cat0), DescriptionAttribute("Cip通讯使用 true使用  false不使用")]
        [DisplayName("Cip通讯使用")]
        [Browsable(true)]
        public bool IsOpenCip
        {
            get { return INI.IsOpenCip; }
            set
            {
                INI.IsOpenCip = value;
                if (value)
                    Universal.CipExtend.Init();
            }
        }
        [CategoryAttribute(cat0), DescriptionAttribute("Cip通讯使用 true使用  false不使用")]
        [DisplayName("A0.是否开启抽检功能")]
        [Browsable(true)]
        public bool IsOpenQcRandom
        {
            get { return INI.IsOpenQcRandom; }
            set
            {
                INI.IsOpenQcRandom = value;
            }
        }
        [CategoryAttribute(cat0), DescriptionAttribute("开启自动切换参数(需要重启程序) true使用  false不使用")]
        [DisplayName("A1.是否开启自动切换参数")]
        [Browsable(false)]
        public bool IsOpenAutoChangeRecipe
        {
            get { return INI.IsOpenAutoChangeRecipe; }
            set
            {
                INI.IsOpenAutoChangeRecipe = value;
                //if (value)
                //    Universal.JzMVDJudgeRecipe.Init();
            }
        }
        [CategoryAttribute(cat0), DescriptionAttribute("在RUN状态下长时间未操作的登出时间  单位秒")]
        [DisplayName("A4.登出时间")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(5, 99999999)]
        public int AutoLogoutTime
        {
            get { return INI.AutoLogoutTime; }
            set
            {
                INI.AutoLogoutTime = value;
            }
        }
        [CategoryAttribute(cat0), DescriptionAttribute(" true关闭  false开启")]
        [DisplayName("A5.强制关闭重复码")]
        [Browsable(true)]
        public bool IsOpenForceNoCheckRepeat
        {
            get { return INI.IsOpenForceNoCheckRepeat; }
            set
            {
                INI.IsOpenForceNoCheckRepeat = value;
            }
        }

        const string cat1 = "01.MainX6 Screen Setup";
        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("分辨率宽度")]
        public int user_screen_width
        {
            get { return INI.user_screen_width; }
            set { INI.user_screen_width = value; }
        }
        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("分辨率高度")]
        public int user_screen_height
        {
            get { return INI.user_screen_height; }
            set { INI.user_screen_height = value; }
        }
        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("分辨率字体")]
        public float user_screen_scale
        {
            get { return INI.user_screen_scale; }
            set { INI.user_screen_scale = value; }
        }
        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("分辨率字体加粗")]
        public bool user_screen_bold
        {
            get { return INI.user_screen_bold; }
            set { INI.user_screen_bold = value; }
        }

        const string cat2 = "02.MainX6 Other Setup";
        [CategoryAttribute(cat2), DescriptionAttribute("true: 常亮 false:测完关闭")]
        [DisplayName("灯光是否常亮")]
        public bool IsLightAlwaysOn
        {
            get { return INI.IsLightAlwaysOn; }
            set { INI.IsLightAlwaysOn = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("true: 收集 false:不收集")]
        [DisplayName("是否收集资料")]
        public bool IsCollectPictures
        {
            get { return INI.IsCollectPictures; }
            set { INI.IsCollectPictures = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("true: 收集 false:不收集")]
        [DisplayName("是否收集小图错误资料")]
        public bool IsCollectErrorSmall
        {
            get { return INI.IsCollectErrorSmall; }
            set { INI.IsCollectErrorSmall = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("true: 收集 false:不收集")]
        [DisplayName("是否收集Strip资料")]
        public bool IsCollectStripPictures
        {
            get { return INI.IsCollectStripPictures; }
            set { INI.IsCollectStripPictures = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("true: 保存 false:不保存")]
        [DisplayName("保存屏幕截图")]
        public bool IsSaveScreen
        {
            get { return INI.IsSaveScreen; }
            set { INI.IsSaveScreen = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("延时拍照时间 单位 ms")]
        [DisplayName("延时拍照时间")]
        public int MAINSD_GETIMAGE_DELAYTIME
        {
            get { return INI.MAINSD_GETIMAGE_DELAYTIME; }
            set { INI.MAINSD_GETIMAGE_DELAYTIME = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("延时拍照时间 单位 ms")]
        [DisplayName("延时启动时间")]
        public int MAINSDM1_GETSTART_DELAYTIME
        {
            get { return INI.MAINSDM1_GETSTART_DELAYTIME; }
            set { INI.MAINSDM1_GETSTART_DELAYTIME = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("信号交互延时 单位 ms")]
        [DisplayName("信号交互延时")]
        public int HANDLE_DELAYTIME
        {
            get { return INI.handle_delaytime; }
            set { INI.handle_delaytime = value; }
        }

        [CategoryAttribute(cat2), DescriptionAttribute("true: 比对 false:不比对")]
        [DisplayName("是否比对条码")]
        [Browsable(false)]
        public bool IsCheckBarcodeOpen
        {
            get { return INI.IsCheckBarcodeOpen; }
            set { INI.IsCheckBarcodeOpen = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("true: 比对 false:不比对")]
        [DisplayName("A01.是否比对重复码")]
        [Browsable(false)]
        public bool IsOpenCheckRepeatCode
        {
            get { return INI.IsOpenCheckRepeatCode; }
            set { INI.IsOpenCheckRepeatCode = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("true: 比对 false:不比对 注:需要先开启 是否比对重复码")]
        [DisplayName("A02.是否比对当前批号重复码")]
        [Browsable(false)]
        public bool IsOpenCheckCurLotRepeatCode
        {
            get { return INI.IsOpenCheckCurLotRepeatCode; }
            set { INI.IsOpenCheckCurLotRepeatCode = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("true: 开启 false:关闭 强制全检即跳过Mapping的不检测")]
        [DisplayName("A03.是否强制全检")]
        [Browsable(true)]
        public bool IsOpenForceAllCheck
        {
            get { return INI.IsOpenForceAllCheck; }
            set { INI.IsOpenForceAllCheck = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("true: 开启 false:关闭 ")]
        [DisplayName("A04.是否提前给信号")]
        [Browsable(false)]
        public bool IsOpenBehindOKSign
        {
            get { return INI.IsOpenBehindOKSign; }
            set { INI.IsOpenBehindOKSign = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("true: 开启 false:关闭 ")]
        [DisplayName("A05.是否显示等级码")]
        [Browsable(true)]
        public bool IsOpenShowGrade
        {
            get { return INI.IsOpenShowGrade; }
            set { INI.IsOpenShowGrade = value; }
        }

        [CategoryAttribute(cat2), DescriptionAttribute("true: 只显示当前图片 false:正常显示")]
        [DisplayName("是否只显示当前图片")]
        [Browsable(true)]
        public bool IsOnlyShowCurrentImage
        {
            get { return INI.IsOnlyShowCurrentImage; }
            set { INI.IsOnlyShowCurrentImage = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("true: 收集 false:不收集")]
        [DisplayName("A05a.是否收集结果图")]
        [Browsable(true)]
        public bool IsCollectPicturesSingle
        {
            get { return INI.IsCollectPicturesSingle; }
            set { INI.IsCollectPicturesSingle = value; }
        }
        //[CategoryAttribute(cat2), DescriptionAttribute("")]
        //[DisplayName("A05b.保存图片的格式")]
        //public SaveImageFormat chipSaveImageFormat
        //{
        //    get { return INI.chipSaveImageFormat; }
        //    set { INI.chipSaveImageFormat = value; }
        //}

        [CategoryAttribute(cat2), DescriptionAttribute("true: 开启 false:关闭")]
        [DisplayName("A06.是否开启容错率")]
        public bool IsOpenFaultToleranceRate
        {
            get { return INI.IsOpenFaultToleranceRate; }
            set { INI.IsOpenFaultToleranceRate = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("即 错误的个数占总数的百分比")]
        [DisplayName("A06a.容错率")]
        public double FaultToleranceRate
        {
            get { return INI.FaultToleranceRate; }
            set { INI.FaultToleranceRate = value; }
        }

        [CategoryAttribute(cat2), DescriptionAttribute("true: 开启 false:关闭")]
        [DisplayName("是否开启接收信号")]
        public bool IsReadHandlerOKSign
        {
            get { return INI.IsReadHandlerOKSign; }
            set { INI.IsReadHandlerOKSign = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("true: 屏蔽 false:打开")]
        [DisplayName("是否屏蔽handler完成信号")]
        public bool IsNoUseHandlerOKSign
        {
            get { return INI.IsNoUseHandlerOKSign; }
            set { INI.IsNoUseHandlerOKSign = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("true: 开启 false:关闭")]
        [DisplayName("是否开启发送Tcp完成信号")]
        public bool IsSendHandlerTcpOKSign
        {
            get { return INI.IsSendHandlerTcpOKSign; }
            set { INI.IsSendHandlerTcpOKSign = value; }
        }

        const string cat3 = "03.MainX6 Shopfloor";
        [CategoryAttribute(cat3), DescriptionAttribute("true: 打开 false:关闭")]
        [DisplayName("是否打开SF")]
        [Browsable(true)]
        public bool JCET_IS_USE_SHOPFLOOR
        {
            get { return INI.JCET_IS_USE_SHOPFLOOR; }
            set { INI.JCET_IS_USE_SHOPFLOOR = value; }
        }
        [CategoryAttribute(cat3), DescriptionAttribute("")]
        [DisplayName("BUFF")]
        [Browsable(true)]
        public int JCET_STRIP_BUFF
        {
            get { return INI.JCET_STRIP_BUFF; }
            set { INI.JCET_STRIP_BUFF = value; }
        }
        [CategoryAttribute(cat3), DescriptionAttribute("停止检测时间")]
        [DisplayName("STOPTIME")]
        [Browsable(true)]
        public int JCET_TIMESTOP_SET
        {
            get { return INI.JCET_TIMESTOP_SET; }
            set { INI.JCET_TIMESTOP_SET = value; }
        }
        [CategoryAttribute(cat3), DescriptionAttribute("eg http://localhost:34489/Service1.asmx")]
        [DisplayName("SF地址")]
        [Browsable(true)]
        public string JCET_WEBSERVICE_URL
        {
            get { return INI.JCET_WEBSERVICE_URL; }
            set { INI.JCET_WEBSERVICE_URL = value; }
        }

        const string cat4 = "04.MainSD Setup";
        [CategoryAttribute(cat4), DescriptionAttribute("每像素的實際尺寸 單位mm")]
        [DisplayName("01.圖像解析度")]
        [Browsable(true)]
        public double MAINSD_PAD_MIL_RESOLUTION
        {
            get { return INI.MAINSD_PAD_MIL_RESOLUTION; }
            set { INI.MAINSD_PAD_MIL_RESOLUTION = value; }
        }
        [CategoryAttribute(cat4), DescriptionAttribute("DEBUG使用  只收集NG 正式跑线为false")]
        [DisplayName("03.只收集NG")]
        [Browsable(true)]
        public bool CHIP_NG_collect
        {
            get { return INI.CHIP_NG_collect; }
            set { INI.CHIP_NG_collect = value; }
        }
        [CategoryAttribute(cat4), DescriptionAttribute("DEBUG使用  强制通过 正式跑线为false")]
        [DisplayName("04.强制通过")]
        [Browsable(true)]
        public bool CHIP_force_pass
        {
            get { return INI.CHIP_force_pass; }
            set { INI.CHIP_force_pass = value; }
        }
    }
    
    class JzINIMAINSDMPropertyGridClass
    {
        public JzINIMAINSDMPropertyGridClass()
        {

        }

        const string cat0 = "00.MainX6通讯设置";

        [CategoryAttribute(cat0), DescriptionAttribute("连接打标服务器的ip地址")]
        [DisplayName("打标服务器IP")]
        public string tcp_ip
        {
            get { return INI.tcp_ip; }
            set { INI.tcp_ip = value; }
        }

        [CategoryAttribute(cat0), DescriptionAttribute("连接打标服务器的端口")]
        [DisplayName("打标服务器端口")]
        public int tcp_port
        {
            get { return INI.tcp_port; }
            set { INI.tcp_port = value; }
        }

        [CategoryAttribute(cat0), DescriptionAttribute("是否打开连接Handle服务器 true打开  false关闭")]
        [DisplayName("Handle服务器Open")]
        public bool tcp_handle_open
        {
            get { return INI.tcp_handle_open; }
            set { INI.tcp_handle_open = value; }
        }
        [CategoryAttribute(cat0), DescriptionAttribute("连接Handle服务器的ip地址")]
        [DisplayName("Handle服务器IP")]
        public string tcp_handle_ip
        {
            get { return INI.tcp_handle_ip; }
            set { INI.tcp_handle_ip = value; }
        }

        [CategoryAttribute(cat0), DescriptionAttribute("连接Handle服务器的端口")]
        [DisplayName("Handle服务器端口")]
        public int tcp_handle_port
        {
            get { return INI.tcp_handle_port; }
            set { INI.tcp_handle_port = value; }
        }

        [CategoryAttribute(cat0), DescriptionAttribute("Cip通讯使用 true使用  false不使用")]
        [DisplayName("Cip通讯使用")]
        public bool IsOpenCip
        {
            get { return INI.IsOpenCip; }
            set
            {
                INI.IsOpenCip = value;
                if (value)
                    Universal.CipExtend.Init();
            }
        }
        [CategoryAttribute(cat0), DescriptionAttribute("Cip通讯使用 true使用  false不使用")]
        [DisplayName("A0.是否开启抽检功能")]
        public bool IsOpenQcRandom
        {
            get { return INI.IsOpenQcRandom; }
            set
            {
                INI.IsOpenQcRandom = value;
            }
        }
        [CategoryAttribute(cat0), DescriptionAttribute("开启自动切换参数(需要重启程序) true使用  false不使用")]
        [DisplayName("A1.是否开启自动切换参数")]
        public bool IsOpenAutoChangeRecipe
        {
            get { return INI.IsOpenAutoChangeRecipe; }
            set
            {
                INI.IsOpenAutoChangeRecipe = value;
                //if (value)
                //    Universal.JzMVDJudgeRecipe.Init();
            }
        }
        [CategoryAttribute(cat0), DescriptionAttribute("模型选择(需要重启程序) ")]
        [DisplayName("A2.模型选择")]
        [TypeConverter(typeof(JzEnumConverter))]
        public PMatchType pMatchType
        {
            get { return INI.pMatchType; }
            set
            {
                INI.pMatchType = value;
            }
        }
        [CategoryAttribute(cat0), DescriptionAttribute("相似度(需要重启程序) ")]
        [DisplayName("A3.相似度")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 1, 0.1f, 2)]
        public float fTolerance
        {
            get { return INI.fTolerance; }
            set
            {
                INI.fTolerance = value;
            }
        }
        [CategoryAttribute(cat0), DescriptionAttribute("在RUN状态下长时间未操作的登出时间  单位秒")]
        [DisplayName("A4.登出时间")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(5, 99999999)]
        public int AutoLogoutTime
        {
            get { return INI.AutoLogoutTime; }
            set
            {
                INI.AutoLogoutTime = value;
            }
        }
        [CategoryAttribute(cat0), DescriptionAttribute(" true关闭  false开启")]
        [DisplayName("A5.强制关闭重复码")]
        public bool IsOpenForceNoCheckRepeat
        {
            get { return INI.IsOpenForceNoCheckRepeat; }
            set
            {
                INI.IsOpenForceNoCheckRepeat = value;
            }
        }

        const string cat1 = "01.MainX6分辨率设置";
        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("分辨率宽度")]
        public int user_screen_width
        {
            get { return INI.user_screen_width; }
            set { INI.user_screen_width = value; }
        }
        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("分辨率高度")]
        public int user_screen_height
        {
            get { return INI.user_screen_height; }
            set { INI.user_screen_height = value; }
        }
        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("分辨率字体")]
        public float user_screen_scale
        {
            get { return INI.user_screen_scale; }
            set { INI.user_screen_scale = value; }
        }
        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("分辨率字体加粗")]
        public bool user_screen_bold
        {
            get { return INI.user_screen_bold; }
            set { INI.user_screen_bold = value; }
        }

        const string cat2 = "02.MainX6其他设置";
        [CategoryAttribute(cat2), DescriptionAttribute("true: 常亮 false:测完关闭")]
        [DisplayName("灯光是否常亮")]
        public bool IsLightAlwaysOn
        {
            get { return INI.IsLightAlwaysOn; }
            set { INI.IsLightAlwaysOn = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("true: 收集 false:不收集")]
        [DisplayName("是否收集资料")]
        public bool IsCollectPictures
        {
            get { return INI.IsCollectPictures; }
            set { INI.IsCollectPictures = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("true: 收集 false:不收集")]
        [DisplayName("是否收集小图错误资料")]
        public bool IsCollectErrorSmall
        {
            get { return INI.IsCollectErrorSmall; }
            set { INI.IsCollectErrorSmall = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("true: 收集 false:不收集")]
        [DisplayName("是否收集Strip资料")]
        public bool IsCollectStripPictures
        {
            get { return INI.IsCollectStripPictures; }
            set { INI.IsCollectStripPictures = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("true: 保存 false:不保存")]
        [DisplayName("保存屏幕截图")]
        public bool IsSaveScreen
        {
            get { return INI.IsSaveScreen; }
            set { INI.IsSaveScreen = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("延时拍照时间 单位 ms")]
        [DisplayName("延时拍照时间")]
        public int MAINSD_GETIMAGE_DELAYTIME
        {
            get { return INI.MAINSD_GETIMAGE_DELAYTIME; }
            set { INI.MAINSD_GETIMAGE_DELAYTIME = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("延时拍照时间 单位 ms")]
        [DisplayName("延时启动时间")]
        public int MAINSDM1_GETSTART_DELAYTIME
        {
            get { return INI.MAINSDM1_GETSTART_DELAYTIME; }
            set { INI.MAINSDM1_GETSTART_DELAYTIME = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("信号交互延时 单位 ms")]
        [DisplayName("信号交互延时")]
        public int HANDLE_DELAYTIME
        {
            get { return INI.handle_delaytime; }
            set { INI.handle_delaytime = value; }
        }

        [CategoryAttribute(cat2), DescriptionAttribute("true: 比对 false:不比对")]
        [DisplayName("是否比对条码")]
        [Browsable(false)]
        public bool IsCheckBarcodeOpen
        {
            get { return INI.IsCheckBarcodeOpen; }
            set { INI.IsCheckBarcodeOpen = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("true: 比对 false:不比对")]
        [DisplayName("A01.是否比对重复码")]
        [Browsable(false)]
        public bool IsOpenCheckRepeatCode
        {
            get { return INI.IsOpenCheckRepeatCode; }
            set { INI.IsOpenCheckRepeatCode = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("true: 比对 false:不比对 注:需要先开启 是否比对重复码")]
        [DisplayName("A02.是否比对当前批号重复码")]
        [Browsable(false)]
        public bool IsOpenCheckCurLotRepeatCode
        {
            get { return INI.IsOpenCheckCurLotRepeatCode; }
            set { INI.IsOpenCheckCurLotRepeatCode = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("true: 开启 false:关闭 强制全检即跳过Mapping的不检测")]
        [DisplayName("A03.是否强制全检")]
        public bool IsOpenForceAllCheck
        {
            get { return INI.IsOpenForceAllCheck; }
            set { INI.IsOpenForceAllCheck = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("true: 开启 false:关闭 ")]
        [DisplayName("A04.是否提前给信号")]
        public bool IsOpenBehindOKSign
        {
            get { return INI.IsOpenBehindOKSign; }
            set { INI.IsOpenBehindOKSign = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("true: 开启 false:关闭 ")]
        [DisplayName("A05.是否显示等级码")]
        public bool IsOpenShowGrade
        {
            get { return INI.IsOpenShowGrade; }
            set { INI.IsOpenShowGrade = value; }
        }

        [CategoryAttribute(cat2), DescriptionAttribute("true: 只显示当前图片 false:正常显示")]
        [DisplayName("是否只显示当前图片")]
        public bool IsOnlyShowCurrentImage
        {
            get { return INI.IsOnlyShowCurrentImage; }
            set { INI.IsOnlyShowCurrentImage = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("true: 收集 false:不收集")]
        [DisplayName("A05a.是否收集结果图")]
        public bool IsCollectPicturesSingle
        {
            get { return INI.IsCollectPicturesSingle; }
            set { INI.IsCollectPicturesSingle = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("")]
        [DisplayName("A05b.保存图片的格式")]
        public SaveImageFormat chipSaveImageFormat
        {
            get { return INI.chipSaveImageFormat; }
            set { INI.chipSaveImageFormat = value; }
        }

        [CategoryAttribute(cat2), DescriptionAttribute("true: 开启 false:关闭")]
        [DisplayName("是否开启接收信号")]
        public bool IsReadHandlerOKSign
        {
            get { return INI.IsReadHandlerOKSign; }
            set { INI.IsReadHandlerOKSign = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("true: 屏蔽 false:打开")]
        [DisplayName("是否屏蔽handler完成信号")]
        public bool IsNoUseHandlerOKSign
        {
            get { return INI.IsNoUseHandlerOKSign; }
            set { INI.IsNoUseHandlerOKSign = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("true: 开启 false:关闭")]
        [DisplayName("是否开启发送Tcp完成信号")]
        public bool IsSendHandlerTcpOKSign
        {
            get { return INI.IsSendHandlerTcpOKSign; }
            set { INI.IsSendHandlerTcpOKSign = value; }
        }

        const string cat3 = "03.MainX6 Shopfloor";
        [CategoryAttribute(cat3), DescriptionAttribute("true: 打开 false:关闭")]
        [DisplayName("是否打开SF")]
        public bool JCET_IS_USE_SHOPFLOOR
        {
            get { return INI.JCET_IS_USE_SHOPFLOOR; }
            set { INI.JCET_IS_USE_SHOPFLOOR = value; }
        }
        [CategoryAttribute(cat3), DescriptionAttribute("")]
        [DisplayName("BUFF")]
        public int JCET_STRIP_BUFF
        {
            get { return INI.JCET_STRIP_BUFF; }
            set { INI.JCET_STRIP_BUFF = value; }
        }
        [CategoryAttribute(cat3), DescriptionAttribute("停止检测时间")]
        [DisplayName("STOPTIME")]
        public int JCET_TIMESTOP_SET
        {
            get { return INI.JCET_TIMESTOP_SET; }
            set { INI.JCET_TIMESTOP_SET = value; }
        }
        [CategoryAttribute(cat3), DescriptionAttribute("eg http://localhost:34489/Service1.asmx")]
        [DisplayName("SF地址")]
        public string JCET_WEBSERVICE_URL
        {
            get { return INI.JCET_WEBSERVICE_URL; }
            set { INI.JCET_WEBSERVICE_URL = value; }
        }

        const string cat4 = "04.MainSD設置";
        [CategoryAttribute(cat4), DescriptionAttribute("每像素的實際尺寸 單位mm")]
        [DisplayName("01.圖像解析度")]
        public double MAINSD_PAD_MIL_RESOLUTION
        {
            get { return INI.MAINSD_PAD_MIL_RESOLUTION; }
            set { INI.MAINSD_PAD_MIL_RESOLUTION = value; }
        }
        [CategoryAttribute(cat4), DescriptionAttribute("DEBUG使用 正式跑线为false")]
        [DisplayName("02.显示详细")]
        public bool CHIP_NG_SHOW
        {
            get { return INI.CHIP_NG_SHOW; }
            set { INI.CHIP_NG_SHOW = value; }
        }
        [CategoryAttribute(cat4), DescriptionAttribute("DEBUG使用  只收集NG 正式跑线为false")]
        [DisplayName("03.只收集NG")]
        public bool CHIP_NG_collect
        {
            get { return INI.CHIP_NG_collect; }
            set { INI.CHIP_NG_collect = value; }
        }
        [CategoryAttribute(cat4), DescriptionAttribute("DEBUG使用  强制通过 正式跑线为false")]
        [DisplayName("04.强制通过")]
        public bool CHIP_force_pass
        {
            get { return INI.CHIP_force_pass; }
            set { INI.CHIP_force_pass = value; }
        }
        [CategoryAttribute(cat4), DescriptionAttribute("无芯片通过即没有芯片的位置可以直接PASS")]
        [DisplayName("05.无芯片通过")]
        public bool CHIP_forceALIGNRUN_pass
        {
            get { return INI.CHIP_forceALIGNRUN_pass; }
            set { INI.CHIP_forceALIGNRUN_pass = value; }
        }
        [CategoryAttribute(cat4), DescriptionAttribute("")]
        [DisplayName("06.开启图片处理")]
        public bool IsDEBUGCHIP
        {
            get { return INI.IsDEBUGCHIP; }
            set { INI.IsDEBUGCHIP = value; }
        }
        [CategoryAttribute(cat4), DescriptionAttribute("0:Normal 1:LINE 2.EQUEL")]
        [DisplayName("07.计算模式")]
        public int CHIP_CAL_MODE
        {
            get { return INI.CHIP_CAL_MODE; }
            set { INI.CHIP_CAL_MODE = value; }
        }
        [CategoryAttribute(cat4), DescriptionAttribute("")]
        [DisplayName("08.SMOOTH")]
        public bool CHIP_ISSMOOTHEN
        {
            get { return INI.CHIP_ISSMOOTHEN; }
            set { INI.CHIP_ISSMOOTHEN = value; }
        }
        [CategoryAttribute(cat4), DescriptionAttribute("")]
        [DisplayName("09.X加减速")]
        public int AXIS_X_JJS
        {
            get { return INI.AXIS_X_JJS; }
            set { INI.AXIS_X_JJS = value; }
        }
        [CategoryAttribute(cat4), DescriptionAttribute("")]
        [DisplayName("10.Y加减速")]
        public int AXIS_Y_JJS
        {
            get { return INI.AXIS_Y_JJS; }
            set { INI.AXIS_Y_JJS = value; }
        }
        [CategoryAttribute(cat4), DescriptionAttribute("")]
        [DisplayName("11.Z加减速")]
        public int AXIS_Z_JJS
        {
            get { return INI.AXIS_Z_JJS; }
            set { INI.AXIS_Z_JJS = value; }
        }
        [CategoryAttribute(cat4), DescriptionAttribute("通过参数名和版本记录测试数据")]
        [DisplayName("12.开启参数记录数据")]
        public bool IsOpenRecipeDataRecord
        {
            get { return INI.IsOpenRecipeDataRecord; }
            set { INI.IsOpenRecipeDataRecord = value; }
        }

        [CategoryAttribute(cat4), DescriptionAttribute("选择数据存储的路径 eg.D:\\ 数据将存于D:\\DataRoot")]
        [DisplayName("13.数据存储路径")]
        [Editor(typeof(GetPlugsPropertyEditor), typeof(UITypeEditor))]
        public string RootPath
        {
            get { return INI.RootPath; }
            set { INI.RootPath = value; }
        }
        [CategoryAttribute(cat4), DescriptionAttribute("")]
        [DisplayName("14.开启判断sensor")]
        public bool IsOpenCheckSensor
        {
            get { return INI.IsOpenCheckSensor; }
            set { INI.IsOpenCheckSensor = value; }
        }
        //[CategoryAttribute(cat4), DescriptionAttribute("设备名称 即DeviceName 用于存储归纳数据")]
        //[DisplayName("14.设备名称")]
        //public string DeviceName
        //{
        //    get { return INI.DeviceName; }
        //    set { INI.DeviceName = value; }
        //}

        const string cat5 = "05.其他設置";
        [CategoryAttribute(cat5), DescriptionAttribute("")]
        [DisplayName("手动加速")]
        public int AXIS_MANUAL_JJS_ADD
        {
            get { return INI.AXIS_MANUAL_JJS_ADD; }
            set { INI.AXIS_MANUAL_JJS_ADD = value; }
        }
        [CategoryAttribute(cat5), DescriptionAttribute("")]
        [DisplayName("手动减速")]
        public int AXIS_MANUAL_JJS_SUB
        {
            get { return INI.AXIS_MANUAL_JJS_SUB; }
            set { INI.AXIS_MANUAL_JJS_SUB = value; }
        }
        [CategoryAttribute(cat5), DescriptionAttribute("")]
        [DisplayName("自动加速")]
        public int AXIS_AUTO_JJS_ADD
        {
            get { return INI.AXIS_AUTO_JJS_ADD; }
            set { INI.AXIS_AUTO_JJS_ADD = value; }
        }
        [CategoryAttribute(cat5), DescriptionAttribute("")]
        [DisplayName("自动减速")]
        public int AXIS_AUTO_JJS_SUB
        {
            get { return INI.AXIS_AUTO_JJS_SUB; }
            set { INI.AXIS_AUTO_JJS_SUB = value; }
        }

        [CategoryAttribute(cat5), DescriptionAttribute("0-100% 即是机械臂速度的百分比")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100)]
        [DisplayName("速度百分比")]
        public double RobotSpeedValue
        {
            get { return INI.RobotSpeedValue; }
            set { INI.RobotSpeedValue = value; }
        }

        [CategoryAttribute(cat5), DescriptionAttribute("")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-100, 1000)]
        [DisplayName("01.线扫开始位置")]
        public float CamLinescanStartPos
        {
            get { return INI.CamLinescanStartPos; }
            set { INI.CamLinescanStartPos = value; }
        }
        [CategoryAttribute(cat5), DescriptionAttribute("")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-100, 1000)]
        [DisplayName("02.线扫结束位置")]
        public float CamLinescanEndPos
        {
            get { return INI.CamLinescanEndPos; }
            set { INI.CamLinescanEndPos = value; }
        }
        [CategoryAttribute(cat5), DescriptionAttribute("")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-100, 1000)]
        [DisplayName("03.线扫速度")]
        public int CamLinescanSpeed
        {
            get { return INI.CamLinescanSpeed; }
            set { INI.CamLinescanSpeed = value; }
        }
        [CategoryAttribute(cat5), DescriptionAttribute("")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-100, 1000)]
        [DisplayName("04.面阵相机位置")]
        public float CamAreaMatchPos
        {
            get { return INI.CamAreaMatchPos; }
            set { INI.CamAreaMatchPos = value; }
        }
        [CategoryAttribute(cat5), DescriptionAttribute("单位 毫秒")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 99999999)]
        [DisplayName("05.拍图超时")]
        public int TestImageOvertime
        {
            get { return INI.TestImageOvertime; }
            set { INI.TestImageOvertime = value; }
        }
        [CategoryAttribute(cat5), DescriptionAttribute("单位 毫秒")]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 99999999)]
        [DisplayName("06.结果超时")]
        public int TestResultOvertime
        {
            get { return INI.TestResultOvertime; }
            set { INI.TestResultOvertime = value; }
        }

    }
    class JzINIMAIN_SDPropertyGridClass
    {
        public JzINIMAIN_SDPropertyGridClass()
        {

        }

        const string cat1 = "01.MainSD分辨率设置";
        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("分辨率宽度")]
        public int user_screen_width
        {
            get { return INI.user_screen_width; }
            set { INI.user_screen_width = value; }
        }
        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("分辨率高度")]
        public int user_screen_height
        {
            get { return INI.user_screen_height; }
            set { INI.user_screen_height = value; }
        }
        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("分辨率字体")]
        public float user_screen_scale
        {
            get { return INI.user_screen_scale; }
            set { INI.user_screen_scale = value; }
        }
        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("分辨率字体加粗")]
        public bool user_screen_bold
        {
            get { return INI.user_screen_bold; }
            set { INI.user_screen_bold = value; }
        }


        const string cat2 = "02.MainSD設置";
        [CategoryAttribute(cat2), DescriptionAttribute("每像素的實際尺寸 單位mm")]
        [DisplayName("01.圖像解析度")]
        public double MAINSD_PAD_MIL_RESOLUTION
        {
            get { return INI.MAINSD_PAD_MIL_RESOLUTION; }
            set { INI.MAINSD_PAD_MIL_RESOLUTION = value; }
        }


        [CategoryAttribute("MainSD Control"), DescriptionAttribute("用户设定满盒PASS数量")]
        [DisplayName("PASS满盒")]
        public int USER_SET_FULL_PASSCOUNT
        {
            get { return INI.USER_SET_FULL_PASSCOUNT; }
            set { INI.USER_SET_FULL_PASSCOUNT = value; }
        }

        [CategoryAttribute("MainSD Control"), DescriptionAttribute("用户设定满盒NG数量")]
        [DisplayName("NG满盒")]
        public int USER_SET_FULL_NGCOUNT
        {
            get { return INI.USER_SET_FULL_NGCOUNT; }
            set { INI.USER_SET_FULL_NGCOUNT = value; }
        }

        [CategoryAttribute("MainSD Control"), DescriptionAttribute("达到连续NG数目报警")]
        [DisplayName("连续NG数目")]
        public int CONTINUE_NG_COUNT
        {
            get { return INI.CONTINUE_NG_COUNT; }
            set { INI.CONTINUE_NG_COUNT = value; }
        }
        [CategoryAttribute("MainSD Control"), DescriptionAttribute("延时拍照时间 ms")]
        [DisplayName("延时拍照时间")]
        public int MAINSD_GETIMAGE_DELAYTIME
        {
            get { return INI.MAINSD_GETIMAGE_DELAYTIME; }
            set { INI.MAINSD_GETIMAGE_DELAYTIME = value; }
        }
        [CategoryAttribute("MainSD Control"), DescriptionAttribute("")]
        [DisplayName("无料面积")]
        public int MAINSD_CHKAREA
        {
            get { return INI.ChkArea; }
            set { INI.ChkArea = value; }
        }
        [CategoryAttribute("MainSD Control"), DescriptionAttribute("")]
        [DisplayName("无料宽度")]
        public int MAINSD_CHKWIDTH
        {
            get { return INI.ChkWidth; }
            set { INI.ChkWidth = value; }
        }
        [CategoryAttribute("MainSD Control"), DescriptionAttribute("")]
        [DisplayName("无料高度")]
        public int MAINSD_CHKHEIGHT
        {
            get { return INI.ChkHeight; }
            set { INI.ChkHeight = value; }
        }
    }
    public class JzFindBlockPropertyGridClass
    {
        public JzFindBlockPropertyGridClass()
        {

        }
        const string cat0 = "00.图像处理";

        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("亮度")]
        [Browsable(false)]
        public int brightness { get; set; } = 0;

        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("对比度")]
        [Browsable(false)]
        public int contrast { get; set; } = 0;

        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("灰阶阈值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 255)]
        public int threshlod { get; set; } = 15;

        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("过滤最小面积")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 10000)]
        public int minarea { get; set; } = 50;

        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("外扩大小")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100)]
        public int extend { get; set; } = 5;

        [CategoryAttribute(cat0), DescriptionAttribute("范围0-100 即占整个图片高度的比例")]
        [DisplayName("比例")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100)]
        public int inflant { get; set; } = 33;

        [CategoryAttribute(cat0), DescriptionAttribute("主要看字符方向 合并框 ")]
        [DisplayName("合成方向")]
        [TypeConverter(typeof(JzEnumConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100)]
        public BlockDir blockdir { get; set; } = BlockDir.VERTICAL;

        public void FromingStr(string str)
        {
            string[] parts = str.Split(',');
            if (parts.Length > 6)
            {
                brightness = int.Parse(parts[0]);
                contrast = int.Parse(parts[1]);
                threshlod = int.Parse(parts[2]);
                minarea = int.Parse(parts[3]);
                extend = int.Parse(parts[4]);
                inflant = int.Parse(parts[5]);
                blockdir = (BlockDir)int.Parse(parts[6]);
            }
        }
        public string ToParaString()
        {
            string str = string.Empty;

            str += brightness.ToString() + ",";
            str += contrast.ToString() + ",";
            str += threshlod.ToString() + ",";
            str += minarea.ToString() + ",";
            str += extend.ToString() + ",";
            str += inflant.ToString() + ",";
            str += ((int)blockdir).ToString() + ",";

            return str;
        }
    }

    public class OneKeyPropertyGridClass
    {
        public OneKeyPropertyGridClass()
        {

        }
        const string cat0 = "00.设置位置";

        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("0A.初值X")]
        [Browsable(true)]
        public double keyx
        {
            get { return INI.keyx; }
            set
            {
                INI.keyx = value;
            }
        }
        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("0A.初值Y")]
        [Browsable(true)]
        public double keyy
        {
            get { return INI.keyy; }
            set
            {
                INI.keyy = value;
            }
        }
        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("0A.初值Z")]
        [Browsable(true)]
        public double keyz
        {
            get { return INI.keyz; }
            set
            {
                INI.keyz = value;
            }
        }

        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("0B.终点X")]
        [Browsable(true)]
        public double keyendx
        {
            get { return INI.keyendx; }
            set
            {
                INI.keyendx = value;
            }
        }
        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("0B.终点Y")]
        [Browsable(true)]
        public double keyendy
        {
            get { return INI.keyendy; }
            set
            {
                INI.keyendy = value;
            }
        }
        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("0B.终点Z")]
        [Browsable(true)]
        public double keyendz
        {
            get { return INI.keyendz; }
            set
            {
                INI.keyendz = value;
            }
        }

        [CategoryAttribute(cat0), DescriptionAttribute("Tray的行数")]
        [DisplayName("1.行")]
        [Browsable(true)]
        public int keyrow
        {
            get { return INI.keyrow; }
            set
            {
                INI.keyrow = value;
            }
        }
        [CategoryAttribute(cat0), DescriptionAttribute("Tray的列数")]
        [DisplayName("1.列")]
        [Browsable(true)]
        public int keycol
        {
            get { return INI.keycol; }
            set
            {
                INI.keycol = value;
            }
        }
        [CategoryAttribute(cat0), DescriptionAttribute("横向X的偏移值")]
        [DisplayName("2.间距X")]
        [Browsable(true)]
        public double keyoffsetx
        {
            get { return INI.keyoffsetx; }
            set
            {
                INI.keyoffsetx = value;
            }
        }
        [CategoryAttribute(cat0), DescriptionAttribute("纵向Y的偏移值")]
        [DisplayName("2.间距Y")]
        [Browsable(true)]
        public double keyoffsety
        {
            get { return INI.keyoffsety; }
            set
            {
                INI.keyoffsety = value;
            }
        }
        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("3.手动初值X")]
        [Browsable(false)]
        public string op_keyxyz
        {
            get { return INI.op_keyxyz; }
            set
            {
                INI.op_keyxyz = value;
            }
        }

        public void FromingStr(string str)
        {
            string[] parts = str.Split(',');
            if (parts.Length > 10)
            {
                keyx = float.Parse(parts[0]);
                keyy = float.Parse(parts[1]);
                keyz = float.Parse(parts[2]);
                keyendx = float.Parse(parts[3]);
                keyendy = float.Parse(parts[4]);
                keyendz = float.Parse(parts[5]);
                keyrow = int.Parse(parts[6]);
                keycol = int.Parse(parts[7]);
                keyoffsetx = float.Parse(parts[8]);
                keyoffsety = float.Parse(parts[9]);
                op_keyxyz = parts[10];
            }
        }
        public string ToParaString()
        {
            string str = string.Empty;

            str += keyx.ToString() + ",";
            str += keyy.ToString() + ",";
            str += keyz.ToString() + ",";
            str += keyendx.ToString() + ",";
            str += keyendy.ToString() + ",";
            str += keyendz.ToString() + ",";
            str += keyrow.ToString() + ",";
            str += keycol.ToString() + ",";
            str += keyoffsetx.ToString() + ",";
            str += keyoffsety.ToString() + ",";
            str += op_keyxyz.ToString() + ",";

            return str;
        }

    }

    public class CheckBaseParaPropertyGridClass
    {
        public CheckBaseParaPropertyGridClass()
        {

        }
        const string cat1 = "00.图像设定";

        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("A0.灰阶阈值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 255)]
        [Browsable(true)]
        public int chkThresholdValue { get; set; } = 128;
        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("A1.寻找方式")]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 255)]
        [Browsable(true)]
        public BlobMode chkblobmode { get; set; } = BlobMode.White;


        const string cat0 = "01.检测规格";

        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("A0.开启训练检查")]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100000)]
        [Browsable(true)]
        public bool chkIsOpen { get; set; } = false;
        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("A1.标准宽度")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100000)]
        [Browsable(true)]
        public float chkWidth { get; set; } = 0;
        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("A2.宽度比例")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100)]
        [Browsable(true)]
        public float chkWidthUpratio { get; set; } = 5;
        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("A3.宽度下限比例")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100)]
        [Browsable(false)]
        public float chkWidthLowerratio { get; set; } = 5;

        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("B1.标准高度")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100000)]
        [Browsable(true)]
        public float chkHeight { get; set; } = 0;
        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("B2.高度比例")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100)]
        [Browsable(true)]
        public float chkHeightUpratio { get; set; } = 5;
        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("B3.高度下限比例")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100)]
        [Browsable(false)]
        public float chkHeightLowerratio { get; set; } = 5;

        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("C1.标准面积")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100000)]
        [Browsable(true)]
        public float chkArea { get; set; } = 0;
        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("C2.面积比例")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100)]
        [Browsable(true)]
        public float chkAreaUpratio { get; set; } = 5;
        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("C3.面积下限比例")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100)]
        [Browsable(false)]
        public float chkAreaLowerratio { get; set; } = 5;


        public void FromingStr(string str)
        {
            string[] parts = str.Split(',');
            if (parts.Length > 12)
            {
                chkWidth = float.Parse(parts[0]);
                chkWidthUpratio = float.Parse(parts[1]);
                chkWidthLowerratio = float.Parse(parts[2]);
                chkHeight = float.Parse(parts[3]);
                chkHeightUpratio = float.Parse(parts[4]);
                chkHeightLowerratio = float.Parse(parts[5]);
                chkArea = float.Parse(parts[6]);
                chkAreaUpratio = float.Parse(parts[7]);
                chkAreaLowerratio = float.Parse(parts[8]);
                chkIsOpen = parts[9] == "1";
                chkThresholdValue = int.Parse(parts[10]);
                chkblobmode = (BlobMode)int.Parse(parts[11]);
            }

        }
        public string ToParaString()
        {
            string str = string.Empty;

            str += chkWidth.ToString() + ",";
            str += chkWidthUpratio.ToString() + ",";
            str += chkWidthLowerratio.ToString() + ",";
            str += chkHeight.ToString() + ",";
            str += chkHeightUpratio.ToString() + ",";
            str += chkHeightLowerratio.ToString() + ",";
            str += chkArea.ToString() + ",";
            str += chkAreaUpratio.ToString() + ",";
            str += chkAreaLowerratio.ToString() + ",";
            str += (chkIsOpen ? "1" : "0") + ",";
            str += chkThresholdValue.ToString() + ",";
            str += ((int)chkblobmode).ToString() + ",";

            return str;
        }

    }

    public class MarkParaPropertyGridClass
    {
        public MarkParaPropertyGridClass()
        {

        }
        const string cat1 = "00.图像设定";

        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("A0.灰阶阈值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 255)]
        [Browsable(true)]
        public int chkThresholdValue { get; set; } = 128;
        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("A1.寻找方式")]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 255)]
        [Browsable(true)]
        [TypeConverter(typeof(JzEnumConverter))]
        public BlobMode chkblobmode { get; set; } = BlobMode.White;


        const string cat0 = "01.检测规格";

        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("A0.开启Mark")]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100000)]
        [Browsable(true)]
        public bool chkIsOpen { get; set; } = false;
        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("A1.Mark世界中心")]
        public PointF PtfCenter { get; set; } = new PointF();
        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("A2.Mark世界寻找区域")]
        public RectangleF RectF { get; set; } = new RectangleF();


        public void FromingStr(string str)
        {
            string[] parts = str.Split(',');
            if (parts.Length > 4)
            {
                chkIsOpen = parts[0] == "1";
                chkThresholdValue = int.Parse(parts[1]);
                chkblobmode = (BlobMode)int.Parse(parts[2]);

                PtfCenter = StringToPointF(parts[3]);
                RectF = StringToRectF(parts[4]);
            }

        }
        public string ToParaString()
        {
            string str = string.Empty;

            str += (chkIsOpen ? "1" : "0") + ",";
            str += chkThresholdValue.ToString() + ",";
            str += ((int)chkblobmode).ToString() + ",";
            str += PointF000ToString(PtfCenter) + ",";
            str += RectFToString(RectF) + ",";

            return str;
        }

        public string PointF000ToString(PointF PTF)
        {
            return PTF.X.ToString("0.000") + ";" + PTF.Y.ToString("0.000");
        }
        public PointF StringToPointF(string Str)
        {
            string[] strs = Str.Split(';');
            return new PointF(float.Parse(strs[0]), float.Parse(strs[1]));
        }
        public string RectFToString(RectangleF RectF)
        {
            string Str = "";

            Str += RectF.X.ToString("0.00") + ";";
            Str += RectF.Y.ToString("0.00") + ";";
            Str += RectF.Width.ToString("0.00") + ";";
            Str += RectF.Height.ToString("0.00");

            return Str;
        }
        public RectangleF StringToRectF(string Str)
        {
            string[] strs = Str.Split(';');
            RectangleF rectF = new RectangleF();

            rectF.X = float.Parse(strs[0]);
            rectF.Y = float.Parse(strs[1]);
            rectF.Width = float.Parse(strs[2]);
            rectF.Height = float.Parse(strs[3]);

            return rectF;


        }

    }

}
