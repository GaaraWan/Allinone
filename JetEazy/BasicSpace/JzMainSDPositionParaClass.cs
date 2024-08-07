﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JetEazy.BasicSpace
{
    [Serializable]
    public class JzMainSDPositionParaClass
    {
        #region INI Access Functions
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        void WriteINIValue(string section, string key, string value, string filepath)
        {
            WritePrivateProfileString(section, key, value, filepath);
        }
        string ReadINIValue(string section, string key, string defaultvaluestring, string filepath)
        {
            string retStr = "";

            StringBuilder temp = new StringBuilder(512);
            int Length = GetPrivateProfileString(section, key, "", temp, 512, filepath);

            retStr = temp.ToString();

            if (retStr == "")
                retStr = defaultvaluestring;
            else
                retStr = retStr.Split('/')[0]; //把說明排除掉

            return retStr;

        }
        #endregion

        string MAINPATH = "";
        string INIFILE = "";
        int INDEX = 0;

        //static JzToolsClass JzTools = new JzToolsClass();

        #region define paras

        //供料区
        //FEED_COUNT=2,
        public float FEED_YPOS1 { get; set; } = 0;
        public float FEED_YPOS2 { get; set; } = 0;

        //测试区
        //TEST_COOUNT=4,
        public float TEST_XPOS1 { get; set; } = 0;
        public float TEST_XPOS2 { get; set; } = 0;
        public float TEST_ZUPPOS { get; set; } = 0;
        public float TEST_ZDOWNPOS { get; set; } = 0;

        //收料区
        //TAKE_COUNT=2,
        public float TAKE_YPOS1 { get; set; } = 0;
        public float TAKE_YPOS2 { get; set; } = 0;

        //public int TAKE_PRODUCT_COUNT_USER { get; set; } = 10;

        public int SETUP_VACC_OVERTIME { get; set; } = 15;

        public float TEST_READY_XPOS { get; set; } = 0;

        public float TAKE_Z1POS1 { get; set; } = 0;
        public float TAKE_Z2POS1 { get; set; } = 0;

        public int INSPECT_PASSINDEX { get; set; } = 0;
        public int INSPECT_NGINDEX { get; set; } = 0;

        /// <summary>
        /// 测试结果
        /// </summary>
        public bool INSPECT_RESULT { get; set; } = false;

        public string mysql_server_ip { get; set; } = "localhost";
        public int mysql_server_port { get; set; } = 3306;

        public string mysql_server_user { get; set; } = "root";
        public string mysql_server_pwd { get; set; } = "12892414";
        public string mysql_server_db { get; set; } = "mainsd";


        #region REPORT USE

        /// <summary>
        /// 储存信息资料 保存label的位置信息(为了查看时画出位置) 及 错误结果 1:OK 2:NG
        /// </summary>
        private List<string> ReportList = new List<string>();

        /// <summary>
        /// 存储等级的信息
        /// </summary>
        private List<string> ReportGradeList = new List<string>();

        private string m_reportpath = @"D:\report\work";
        private string m_reportmappingpath = @"D:\report\mapping";
        public string ReportPath
        {
            get { return m_reportpath; }
            set { m_reportpath = value; }
        }

        /// <summary>
        /// 报表批号
        /// </summary>
        public string Report_LOT { get; set; } = "none";

        public void ReportReset()
        {
            ReportList.Clear();
            ReportGradeList.Clear();
        }
        public void ReportAdd(string eStr)
        {
            ReportList.Add(eStr);
        }
        public void ReportGradeAdd(string eStr)
        {
            ReportGradeList.Add(eStr);
        }
        public void ReportGradeSave(int eIndex, bool eIspass)
        {
            if (string.IsNullOrEmpty(Report_LOT))
                Report_LOT = "none";

            Report_LOT = Report_LOT.Replace('-', '_');

            //路径 + auto + 批号 
            string str = "D:\\ReportGrade" + "\\auto\\" + $"{Report_LOT}";
            string strfilename = Report_LOT + "_" + (eIspass ? "P-" : "F-") + eIndex.ToString("00000") + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");

            ReportGradeSave(str, strfilename);
        }
        public void ReportAUTOSave(int eIndex,bool eIspass,bool eUseDataSave=false)
        {
            if (string.IsNullOrEmpty(Report_LOT))
                Report_LOT = "none";

            Report_LOT = Report_LOT.Replace('-', '_');

            //路径 + auto + 批号 
            string str = m_reportpath + "\\auto\\auto_" + Report_LOT;
            string strfilename =(eIspass?"P-":"F-")+ "auto-" + eIndex.ToString("00000") + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");

            if (eUseDataSave)
            {
                str = m_reportpath + "\\auto\\" + JzTimes.DateSerialString + "\\auto_" + Report_LOT;
            }

            //mySqlTableCreate("AUTO_" + Report_LOT);
            //mySqlTableInsert("AUTO_" + Report_LOT, strfilename);
            //mySqlTableInsert("auto_" + Report_LOT);
            ReportSave(str, strfilename);

            str = m_reportmappingpath + "\\auto\\auto_" + Report_LOT;
            ReportMappingSave(str, strfilename);
        }
        public void ReportMANUALSave(bool eIspass)
        {
            if (string.IsNullOrEmpty(Report_LOT))
                Report_LOT = "none";

            Report_LOT = Report_LOT.Replace('-', '_');

            //路径 + auto + 批号 
            string str = m_reportpath + "\\manual\\manual_" + Report_LOT;
            string strfilename = (eIspass ? "P-" : "F-") + "manual-" + DateTime.Now.ToString("yyyyMMddHHmmss");

            //mySqlTableCreate("MANUAL_" + Report_LOT);
            //mySqlTableInsert("MANUAL_" + Report_LOT, strfilename);
            ReportSave(str, strfilename);

            str = m_reportmappingpath + "\\manual\\manual_" + Report_LOT;
            ReportMappingSave(str, strfilename);
        }
        public void ReportAOISave(int eIndex, bool eIspass)
        {
            if (string.IsNullOrEmpty(Report_LOT))
                Report_LOT = "none";

            Report_LOT = Report_LOT.Replace('-', '_');

            //路径 + auto + 批号 
            string str = m_reportpath + "\\AOI\\AOI_" + Report_LOT;
            string strfilename = (eIspass ? "P-" : "F-") + "AOI-" + eIndex.ToString("00000") + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");

            //mySqlTableCreate("AUTO_" + Report_LOT);
            //mySqlTableInsert("AUTO_" + Report_LOT, strfilename);
            //mySqlTableInsert("auto_" + Report_LOT);
            ReportSave(str, strfilename);

            str = m_reportmappingpath + "\\AOI\\auto_" + Report_LOT;
            ReportMappingSave(str, strfilename);
        }
        private void ReportSave(string ePath,string eFilename)
        {
            if (ReportList.Count == 0)
                return;

            string Str = "";
            foreach (string str in ReportList)
            {
                Str += str + Environment.NewLine;
            }

            if (!System.IO.Directory.Exists(ePath))
                System.IO.Directory.CreateDirectory(ePath);

            _save(Str, ePath + "\\" + eFilename + ".csv");
        }
        private void ReportGradeSave(string ePath, string eFilename)
        {
            if (ReportGradeList.Count == 0)
                return;

            ReportGradeList.Sort();

            string Str = "";
            foreach (string str in ReportGradeList)
            {
                Str += Report_LOT + ";" + str + Environment.NewLine;
            }

            if (!System.IO.Directory.Exists(ePath))
                System.IO.Directory.CreateDirectory(ePath);

            _save(Str, ePath + "\\" + eFilename + ".txt");
        }
        private void ReportMappingSave(string ePath, string eFilename)
        {
            if (ReportList.Count == 0)
                return;

            string Str = "";
            string str_text_tmp = "1";
            foreach (string str in ReportList)
            {
                //Str += str + Environment.NewLine;

                string[] strs = str.Split(',');
                if(strs.Length >= 7)
                {
                    string[] strs_text = strs[6].Split('-');
                    if (str_text_tmp != strs_text[0])
                    {
                        str_text_tmp = strs_text[0];
                        Str += Environment.NewLine;
                    }
                    Str += strs[5] + ",";
                    //Str += (strs[5] == "0" ? "P" : strs[5]) + ",";
                }
            }

            Str += Environment.NewLine;
            if (!System.IO.Directory.Exists(ePath))
                System.IO.Directory.CreateDirectory(ePath);

            _save(Str, ePath + "\\" + eFilename + ".csv");
        }

        private void _save(string DataStr, string FileName)
        {
            System.IO.StreamWriter stm = null;

            try
            {
                stm = new System.IO.StreamWriter(FileName, false, System.Text.Encoding.Default);
                stm.Write(DataStr);
                stm.Flush();
                stm.Close();
                stm.Dispose();
                stm = null;
            }
            catch (Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
            }

            if (stm != null)
                stm.Dispose();
        }

        #endregion


        #endregion

        #region my sql 

        CommonLogClass m_log = new CommonLogClass();

        private void mySqlTableCreate(string tablename)
        {
            //return;

            MySqlConnection sqlCnt = null;
            MySqlCommand cmd = null;

            try
            {

                sqlCnt = new MySqlConnection();
                string ConnectionString = "server=127.0.0.1;port=3306;user=root;password=12892414; database=mainsd;";
                ConnectionString = "server=" + mysql_server_ip +
                                                   ";port=" + mysql_server_port.ToString() +
                                                   ";user=" + mysql_server_user +
                                                   ";password=" + mysql_server_pwd +
                                                   ";database=" + mysql_server_db + ";";

                sqlCnt.ConnectionString = ConnectionString;
                sqlCnt.Open();

                string table_name = tablename;
                string sql = "CREATE TABLE IF NOT EXISTS " + table_name +
               "(" +
               "myFilename TEXT)";

                cmd = new MySqlCommand(sql, sqlCnt);
                int iret = cmd.ExecuteNonQuery();

                m_log.Log2("sql=" + sql);
                m_log.Log2("result=" + iret.ToString());

            }
            catch(Exception ex)
            {
                m_log.Log2(ex.Message);
                m_log.Log2(ex.StackTrace);
                m_log.Log2(ex.Source);
            }

            if (sqlCnt != null)
            {
                sqlCnt.Close();
                sqlCnt.Dispose();
                sqlCnt = null;
            }
            if (cmd != null)
            {
                cmd.Dispose();
                cmd = null;
            }
        }
        private void mySqlTableInsert(string tablename, string filename)
        {
            //return;

            MySqlConnection sqlCnt = null;
            MySqlCommand cmd = null;

            try
            {
                sqlCnt = new MySqlConnection();
                string ConnectionString = "server=127.0.0.1;port=3306;user=root;password=12892414; database=mainsd;";
                ConnectionString = "server=" + mysql_server_ip +
                                                   ";port=" + mysql_server_port.ToString() +
                                                   ";user=" + mysql_server_user +
                                                   ";password=" + mysql_server_pwd +
                                                   ";database=" + mysql_server_db + ";";

                sqlCnt.ConnectionString = ConnectionString;
                sqlCnt.Open();

                string table_name = tablename;
                string sql = "INSERT INTO " + table_name +
               "(" +
               "myFilename) VALUES ('" + filename + "')";


                cmd = new MySqlCommand(sql, sqlCnt);
                int iret = cmd.ExecuteNonQuery();

                m_log.Log2("sql=" + sql);
                m_log.Log2("result=" + iret.ToString());

            }
            catch(Exception ex)
            {
                m_log.Log2(ex.Message);
                m_log.Log2(ex.StackTrace);
                m_log.Log2(ex.Source);
            }

            if (sqlCnt != null)
            {
                sqlCnt.Close();
                sqlCnt.Dispose();
                sqlCnt = null;
            }
            if (cmd != null)
            {
                cmd.Dispose();
                cmd = null;
            }

        }

        private void mySqlTableInsert(string epath)
        {
            //return;

            MySqlConnection sqlCnt = null;
            MySqlCommand cmd = null;

            try
            {
                sqlCnt = new MySqlConnection();
                string ConnectionString = "server=127.0.0.1;port=3306;user=root;password=12892414; database=mainsd;";
                ConnectionString = "server=" + mysql_server_ip +
                                                   ";port=" + mysql_server_port.ToString() +
                                                   ";user=" + mysql_server_user +
                                                   ";password=" + mysql_server_pwd +
                                                   ";database=" + mysql_server_db + ";";

                sqlCnt.ConnectionString = ConnectionString;
                sqlCnt.Open();

                string table_name = "jz_path_tb";
                string sql = "INSERT INTO " + table_name +
               "(" +
               "v01,v02) VALUES ('" + epath + "','" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "')";


                cmd = new MySqlCommand(sql, sqlCnt);
                int iret = cmd.ExecuteNonQuery();

                m_log.Log2("sql=" + sql);
                m_log.Log2("result=" + iret.ToString());

            }
            catch (Exception ex)
            {
                m_log.Log2(ex.Message);
                m_log.Log2(ex.StackTrace);
                m_log.Log2(ex.Source);
            }

            if (sqlCnt != null)
            {
                sqlCnt.Close();
                sqlCnt.Dispose();
                sqlCnt = null;
            }
            if (cmd != null)
            {
                cmd.Dispose();
                cmd = null;
            }

        }

        #endregion

        public JzMainSDPositionParaClass(string ePath)
        {
            //INDEX = eIndex;

            //string _path = ePath + "\\" + eIndex.ToString("000");
            if (!System.IO.Directory.Exists(ePath))
                System.IO.Directory.CreateDirectory(ePath);
            MAINPATH = ePath;
        }
        public void Initial()
        {
            Initial("default");
        }
        public void Initial(string eFileName)
        {
            INIFILE = MAINPATH + "\\" + eFileName + ".ini";

            Load();
        }

        public void Load()
        {
            m_log.LogPath = @"D:\log\log_mainsd";
            

            FEED_YPOS1 = float.Parse(ReadINIValue("Feed Control", "FEED_YPOS1", FEED_YPOS1.ToString(), INIFILE));
            FEED_YPOS2 = float.Parse(ReadINIValue("Feed Control", "FEED_YPOS2", FEED_YPOS2.ToString(), INIFILE));

            TEST_XPOS1 = float.Parse(ReadINIValue("Test Control", "TEST_XPOS1", TEST_XPOS1.ToString(), INIFILE));
            TEST_XPOS2 = float.Parse(ReadINIValue("Test Control", "TEST_XPOS2", TEST_XPOS2.ToString(), INIFILE));
            TEST_ZUPPOS = float.Parse(ReadINIValue("Test Control", "TEST_ZUPPOS", TEST_ZUPPOS.ToString(), INIFILE));
            TEST_ZDOWNPOS = float.Parse(ReadINIValue("Test Control", "TEST_ZDOWNPOS", TEST_ZDOWNPOS.ToString(), INIFILE));

            TAKE_YPOS1 = float.Parse(ReadINIValue("Take Control", "TAKE_YPOS1", TAKE_YPOS1.ToString(), INIFILE));
            TAKE_YPOS2 = float.Parse(ReadINIValue("Take Control", "TAKE_YPOS2", TAKE_YPOS2.ToString(), INIFILE));

            TAKE_Z1POS1 = float.Parse(ReadINIValue("Take Control", "TAKE_Z1POS1", TAKE_Z1POS1.ToString(), INIFILE));
            TAKE_Z2POS1 = float.Parse(ReadINIValue("Take Control", "TAKE_Z2POS1", TAKE_Z2POS1.ToString(), INIFILE));

            //TAKE_PRODUCT_COUNT_USER = int.Parse(ReadINIValue("Take Control", "TAKE_PRODUCT_COUNT_USER", TAKE_PRODUCT_COUNT_USER.ToString(), INIFILE));

            SETUP_VACC_OVERTIME = int.Parse(ReadINIValue("Setup Control", "SETUP_VACC_OVERTIME", SETUP_VACC_OVERTIME.ToString(), INIFILE));
            TEST_READY_XPOS = float.Parse(ReadINIValue("Test Control", "TEST_READY_XPOS", TEST_READY_XPOS.ToString(), INIFILE));

            INSPECT_PASSINDEX = int.Parse(ReadINIValue("Setup Control", "INSPECT_PASSINDEX", INSPECT_PASSINDEX.ToString(), INIFILE));
            INSPECT_NGINDEX = int.Parse(ReadINIValue("Setup Control", "INSPECT_NGINDEX", INSPECT_NGINDEX.ToString(), INIFILE));

            INSPECT_RESULT = int.Parse(ReadINIValue("Setup Control", "INSPECT_RESULT", (INSPECT_RESULT ? "1" : "0"), INIFILE)) == 1;

            Report_LOT = ReadINIValue("Report Control", "Report_LOT", Report_LOT, INIFILE);

            mysql_server_ip = ReadINIValue("MySql Control", "mysql_server_ip", mysql_server_ip, INIFILE);
            mysql_server_port = int.Parse(ReadINIValue("MySql Control", "mysql_server_port", mysql_server_port.ToString(), INIFILE));
            mysql_server_user = ReadINIValue("MySql Control", "mysql_server_user", mysql_server_user, INIFILE);
            mysql_server_pwd = ReadINIValue("MySql Control", "mysql_server_pwd", "", INIFILE);
            mysql_server_db = ReadINIValue("MySql Control", "mysql_server_db", mysql_server_db, INIFILE);
        }

        public void Save()
        {
            WriteINIValue("Feed Control", "FEED_YPOS1", FEED_YPOS1.ToString(), INIFILE);
            WriteINIValue("Feed Control", "FEED_YPOS2", FEED_YPOS2.ToString(), INIFILE);

            WriteINIValue("Test Control", "TEST_XPOS1", TEST_XPOS1.ToString(), INIFILE);
            WriteINIValue("Test Control", "TEST_XPOS2", TEST_XPOS2.ToString(), INIFILE);
            WriteINIValue("Test Control", "TEST_ZUPPOS", TEST_ZUPPOS.ToString(), INIFILE);
            WriteINIValue("Test Control", "TEST_ZDOWNPOS", TEST_ZDOWNPOS.ToString(), INIFILE);

            WriteINIValue("Take Control", "TAKE_YPOS1", TAKE_YPOS1.ToString(), INIFILE);
            WriteINIValue("Take Control", "TAKE_YPOS2", TAKE_YPOS2.ToString(), INIFILE);

            WriteINIValue("Take Control", "TAKE_Z1POS1", TAKE_Z1POS1.ToString(), INIFILE);
            WriteINIValue("Take Control", "TAKE_Z2POS1", TAKE_Z2POS1.ToString(), INIFILE);

            //WriteINIValue("Take Control", "TAKE_PRODUCT_COUNT_USER", TAKE_PRODUCT_COUNT_USER.ToString(), INIFILE);

            WriteINIValue("Setup Control", "SETUP_VACC_OVERTIME", SETUP_VACC_OVERTIME.ToString(), INIFILE);
            WriteINIValue("Test Control", "TEST_READY_XPOS", TEST_READY_XPOS.ToString(), INIFILE);

            WriteINIValue("MySql Control", "mysql_server_ip", mysql_server_ip.ToString(), INIFILE);
            WriteINIValue("MySql Control", "mysql_server_port", mysql_server_port.ToString(), INIFILE);
            WriteINIValue("MySql Control", "mysql_server_user", mysql_server_user.ToString(), INIFILE);
            WriteINIValue("MySql Control", "mysql_server_pwd", mysql_server_pwd.ToString(), INIFILE);
            WriteINIValue("MySql Control", "mysql_server_db", mysql_server_db.ToString(), INIFILE);

        }
        public void SaveRecord()
        {
            WriteINIValue("Setup Control", "INSPECT_PASSINDEX", INSPECT_PASSINDEX.ToString(), INIFILE);
            WriteINIValue("Setup Control", "INSPECT_NGINDEX", INSPECT_NGINDEX.ToString(), INIFILE);
            WriteINIValue("Setup Control", "INSPECT_RESULT", (INSPECT_RESULT ? "1" : "0"), INIFILE);

            WriteINIValue("Report Control", "Report_LOT", Report_LOT, INIFILE);
        }

        public void PassZero()
        {
            INSPECT_PASSINDEX = 0;
            WriteINIValue("Setup Control", "INSPECT_PASSINDEX", INSPECT_PASSINDEX.ToString(), INIFILE);
        }
        public void NgZero()
        {
            INSPECT_NGINDEX = 0;
            WriteINIValue("Setup Control", "INSPECT_NGINDEX", INSPECT_NGINDEX.ToString(), INIFILE);
        }
    }
}
