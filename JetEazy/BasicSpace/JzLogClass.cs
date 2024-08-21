using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JetEazy.BasicSpace
{
    class JzLogClass
    {
    }
    public class CommonLogClass
    {
        bool m_IsRecordLog = true;
        RichTextBox richTextBoxRemote = null;
        #region 日志记录、支持其他线程访问  
        public delegate void LogAppendDelegate(Color color, string text);

        public void SetRichTextBox(RichTextBox ertb, bool eIsRecord = true)
        {
            richTextBoxRemote = ertb;
            m_IsRecordLog = eIsRecord;
        }
        /// <summary>  
        /// 追加显示文本  
        /// </summary>  
        /// <param name="color">文本颜色</param>  
        /// <param name="text">显示文本</param>  
        public void LogAppend(Color color, string text)
        {
            if (richTextBoxRemote.TextLength >= 20000)
                richTextBoxRemote.Text = "";

            //richTextBoxRemote.AppendText("/n");
            richTextBoxRemote.SelectionColor = color;
            richTextBoxRemote.AppendText(text + Environment.NewLine);

            richTextBoxRemote.SelectionStart = richTextBoxRemote.TextLength;
            richTextBoxRemote.ScrollToCaret();
        }
        /// <summary>  
        /// 显示错误日志  
        /// </summary>  
        /// <param name="text"></param>  
        public void LogError(string text)
        {
            if (m_IsRecordLog)
                _log(text);
            LogAppendDelegate la = new LogAppendDelegate(LogAppend);
            richTextBoxRemote.Invoke(la, Color.Red, DateTime.Now.ToString("HH:mm:ss ") + text);
        }
        /// <summary>  
        /// 显示警告信息  
        /// </summary>  
        /// <param name="text"></param>  
        public void LogWarning(string text)
        {
            if (m_IsRecordLog)
                _log(text);
            LogAppendDelegate la = new LogAppendDelegate(LogAppend);
            richTextBoxRemote.Invoke(la, Color.Violet, DateTime.Now.ToString("HH:mm:ss ") + text);
        }
        /// <summary>  
        /// 显示信息  
        /// </summary>  
        /// <param name="text"></param>  
        public void LogMessage(string text)
        {
            if (m_IsRecordLog)
                _log(text);
            LogAppendDelegate la = new LogAppendDelegate(LogAppend);
            richTextBoxRemote.Invoke(la, Color.Black, DateTime.Now.ToString("HH:mm:ss ") + text);
        }
        public void LogMessageOK(string text)
        {
            if (m_IsRecordLog)
                _log(text);
            LogAppendDelegate la = new LogAppendDelegate(LogAppend);
            richTextBoxRemote.Invoke(la, Color.Lime, DateTime.Now.ToString("HH:mm:ss ") + text);
        }
        #endregion

        ListBox m_skLstLogMessage = null;
        public void SetSkinListBox(ListBox eskLstLogMessage)
        {
            m_skLstLogMessage = eskLstLogMessage;
        }

        private static CommonLogClass m_log = null;// new CommonLogClass();
        public static CommonLogClass Instance
        {
            get
            {
                if (m_log == null)
                    m_log = new CommonLogClass();

                return m_log;
            }
        }
        private string m_log_path = Application.StartupPath + @"\log";

        public string LogPath
        {
            set { m_log_path = value; }
        }
        private string m_log_filename = "log_filename";
        public string LogFilename
        {
            set { m_log_filename = value; }
        }

        int ilogCount = 10;
        public void LogListbox(string _message, string strMode = "1")
        {
            //return;//先不记录

            if (m_skLstLogMessage != null)
            {
                switch (strMode)
                {
                    default:

                        //SkinListBoxItem _skitem = new SkinListBoxItem("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "] " + _message);
                        string _skitem = "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "] " + _message;

                        m_skLstLogMessage.Invoke(new Action(() =>
                        {
                            m_skLstLogMessage.Items.Add(_skitem);
                            if (m_skLstLogMessage.Items.Count > 5)
                                m_skLstLogMessage.SelectedIndex = m_skLstLogMessage.Items.Count - 1;
                            if (m_skLstLogMessage.Items.Count > ilogCount)
                            {
                                int i = 0;
                                while (i < ilogCount / 2)
                                {
                                    m_skLstLogMessage.Items.RemoveAt(0);
                                    i++;
                                }
                            }
                        }));

                        break;
                }
            }

            _log(_message);
        }
        public void Log(string _message, string strExt = ".log")
        {
            _log(_message, strExt);
        }
        private void _log(string strMsg, string strExt = ".log")
        {
            //lock (LAST_RECIPE_NAME)
            //{
            string strPath = m_log_path + "\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
            if (!System.IO.Directory.Exists(strPath))
                System.IO.Directory.CreateDirectory(strPath);

            string strFileName = strPath + DateTime.Now.ToString("yyyyMMdd_HH") + strExt;
            System.IO.StreamWriter stm = null;

            try
            {
                stm = new System.IO.StreamWriter(strFileName, true, System.Text.Encoding.UTF8);
                stm.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                stm.Write(",");
                stm.Write(Application.ProductVersion);
                stm.Write(", ");
                stm.WriteLine(strMsg);
                stm.Flush();
                stm.Close();
                stm.Dispose();
                stm = null;
            }
            catch
            {
            }

            if (stm != null)
                stm.Dispose();
            //}
        }

        public void Log2(string _message, string strExt = ".log")
        {
            _log2(_message, strExt);
        }
        private void _log2(string strMsg, string strExt = ".log")
        {
            //lock (LAST_RECIPE_NAME)
            //{
            string strPath = m_log_path + "\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
            if (!System.IO.Directory.Exists(strPath))
                System.IO.Directory.CreateDirectory(strPath);

            string strFileName = strPath + m_log_filename + "_" + DateTime.Now.ToString("yyyyMMdd_HH") + strExt;
            System.IO.StreamWriter stm = null;

            try
            {
                stm = new System.IO.StreamWriter(strFileName, true, System.Text.Encoding.UTF8);
                stm.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                stm.Write(",");
                stm.Write(Application.ProductVersion);
                stm.Write(", ");
                stm.WriteLine(strMsg);
                stm.Flush();
                stm.Close();
                stm.Dispose();
                stm = null;
            }
            catch
            {
            }

            if (stm != null)
                stm.Dispose();
            //}
        }
    }

    public class MyLog
    {
        public enum LogStyle : int
        {
            CMD_MESPlus,
            CMD_Exception,
            CMD_NORMAL,
        }
        public static void LogShopfloorCMD(string _logcmd, string strMsg)
        {
            LogShopfloor(_logcmd, strMsg);
        }
        public static void LogShopfloorCMD(LogStyle _logcmd, string strMsg)
        {
            LogShopfloor(_logcmd.ToString(), strMsg);
        }

        private static void LogShopfloor(string strCMD, string strMsg)
        {
            //lock (LAST_RECIPE_NAME)
            //{
            string strPath = "D:\\Log(JcetShopfloor)\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
            if (!System.IO.Directory.Exists(strPath))
                System.IO.Directory.CreateDirectory(strPath);

            string strFileName = strPath + DateTime.Now.ToString("yyyyMMdd_HH") + ".log.txt";
            System.IO.StreamWriter stm = null;

            try
            {
                stm = new System.IO.StreamWriter(strFileName, true, System.Text.Encoding.Default);
                stm.Write(DateTime.Now.ToString("HH:mm:ss") + "".PadLeft(8));
                stm.Write(",");
                stm.Write(strCMD);
                stm.Write(",".PadLeft(8));
                stm.WriteLine(strMsg);
                stm.Flush();
                stm.Close();
                stm.Dispose();
                stm = null;
            }
            catch
            {
            }

            if (stm != null)
                stm.Dispose();
            //}
        }
    }
}
