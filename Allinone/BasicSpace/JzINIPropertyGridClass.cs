﻿
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Allinone.FormSpace;
using JetEazy.BasicSpace;
using JetEazy.UISpace;

namespace Allinone.BasicSpace
{
    public enum BlockDir
    {
        /// <summary>
        /// 水平方向
        /// </summary>
        [Description("水平方向")]
        HORIZONTAL,
        /// <summary>
        /// 垂直方向
        /// </summary>
        [Description("垂直方向")]
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
        public bool IsCheckBarcodeOpen
        {
            get { return INI.IsCheckBarcodeOpen; }
            set { INI.IsCheckBarcodeOpen = value; }
        }

        [CategoryAttribute(cat2), DescriptionAttribute("true: 只显示当前图片 false:正常显示")]
        [DisplayName("是否只显示当前图片")]
        public bool IsOnlyShowCurrentImage
        {
            get { return INI.IsOnlyShowCurrentImage; }
            set { INI.IsOnlyShowCurrentImage = value; }
        }
        [CategoryAttribute(cat2), DescriptionAttribute("true: 收集 false:不收集")]
        [DisplayName("是否收集结果图")]
        public bool IsCollectPicturesSingle
        {
            get { return INI.IsCollectPicturesSingle; }
            set { INI.IsCollectPicturesSingle = value; }
        }

        [CategoryAttribute(cat2), DescriptionAttribute("true: 开启 false:关闭")]
        [DisplayName("是否开启接收信号")]
        public bool IsReadHandlerOKSign
        {
            get { return INI.IsReadHandlerOKSign; }
            set { INI.IsReadHandlerOKSign = value; }
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

        const string cat5 = "05.机械臂設置";
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

    }



}
