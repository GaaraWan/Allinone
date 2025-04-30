using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AJZReportViewer
{
    public class INIClass
    {
        private static INIClass _instance = null;
        public static INIClass Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new INIClass();
                return _instance;
            }
        }

        #region INI Access Functions
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        void WriteINIValue(string section, string key, string value, string filepath)
        {
            WritePrivateProfileString(section, key, value, filepath);
        }
        public string ReadINIValue(string section, string key, string defaultvaluestring, string filepath)
        {
            string retStr = "";

            StringBuilder temp = new StringBuilder(1024);
            int Length = GetPrivateProfileString(section, key, "", temp, 1024, filepath);

            retStr = temp.ToString();

            if (retStr == "")
                retStr = defaultvaluestring;
            //else
            //    retStr = retStr.Split('/')[0]; //把說明排除掉

            return retStr;

        }
        #endregion

        string MAINPATH = "";
        string INIFILE = "";

        const string LSCat1 = "A01.基础参数";

        [CategoryAttribute(LSCat1), DescriptionAttribute("true开 false关")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("00.自动删除开关")]
        [Browsable(true)]
        public bool IsOpenAutoDelFiles { get; set; } = false;

        [CategoryAttribute(LSCat1), DescriptionAttribute("3即代表删除当前时间的前3个月")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("01.删除前几个月")]
        [Browsable(true)]
        public int xMonthCount { get; set; } = 3;

        [CategoryAttribute(LSCat1), DescriptionAttribute("单位小时 例:8即代表每天的8点")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("02.自动删除时间节点")]
        [Browsable(true)]
        public int xNowHour { get; set; } = 8;

        [CategoryAttribute(LSCat1), DescriptionAttribute("")]
        //[Editor(typeof(GetPositionPropertyEditor), typeof(UITypeEditor))]
        [DisplayName("02.自动删除时间节点")]
        [Browsable(false)]
        public DateTime xDelFileTimeStart { get; set; } = DateTime.Now;

        public void Initial()
        {
            MAINPATH = Application.StartupPath;
            INIFILE = MAINPATH + "\\JzReportConfig.ini";

            Load();
        }
        public void Load()
        {
            IsOpenAutoDelFiles = ReadINIValue("Basic", "IsOpenAutoDelFiles", (IsOpenAutoDelFiles ? "1" : "0"), INIFILE) == "1";
            xMonthCount = int.Parse(ReadINIValue("Basic", "xMonthCount", xMonthCount.ToString(), INIFILE));
            xNowHour = int.Parse(ReadINIValue("Basic", "xNowHour", xNowHour.ToString(), INIFILE));
            if (xNowHour < 0 || xNowHour > 11)
                xNowHour = 8;

            xDelFileTimeStart = DateTime.Parse(ReadINIValue("Basic", "xDelFileTimeStart", xDelFileTimeStart.ToString(), INIFILE));
        }
        public void Save()
        {
            WriteINIValue("Basic", "IsOpenAutoDelFiles", (IsOpenAutoDelFiles ? "1" : "0"), INIFILE);
            WriteINIValue("Basic", "xMonthCount", xMonthCount.ToString(), INIFILE);
            if (xNowHour < 0 || xNowHour > 11)
                xNowHour = 8;
            WriteINIValue("Basic", "xNowHour", xNowHour.ToString(), INIFILE);
            WriteINIValue("Basic", "xDelFileTimeStart", xDelFileTimeStart.ToString(), INIFILE);
        }

    }
}
