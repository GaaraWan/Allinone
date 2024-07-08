
using System;
using System.IO;
using System.Reflection;

namespace JetEazy
{
    public class LoggerClass
    {
        public enum LogType
        {
            All,
            Information,
            Debug,
            Success,
            Failure,
            Warning,
            Error
        }

        #region Instance
        private static object logLock;

        private static LoggerClass _instance;

        private static string logFileName;
        private LoggerClass() { }

        /// <summary>
        /// LoggerClass instance
        /// </summary>
        public static LoggerClass Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LoggerClass();
                    logLock = new object();
                    //  logFileName = Guid.NewGuid() + ".log";
                    logFileName = "Run.log";

                }
                return _instance;
            }
        }
        #endregion

        /// <summary>
        /// Write log to log file
        /// </summary>
        /// <param name="logContent">Log content</param>
        /// <param name="logType">Log type</param>
        public void WriteLog(string logContent, LogType logType = LogType.Information, string fileName = null)
        {
            try
            {
                string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
               
                basePath = @"D:\LOG\AllinoneLog\";
                if (!Directory.Exists(basePath ))
                {
                    Directory.CreateDirectory(basePath );
                }
                string dataString = DateTime.Now.ToString("yyyy-MM-dd");
                //if (!Directory.Exists(basePath + "\\Log\\" + dataString))
                //{
                //    Directory.CreateDirectory(basePath + "\\Log\\" + dataString);
                //}

                string[] logText = new string[] { DateTime.Now.ToString("hh:mm:ss") + ": " + logType.ToString() + ": " + logContent };
                if (!string.IsNullOrEmpty(fileName))
                {
                    fileName = dataString + fileName + ".log";
                }
                else
                {
                    fileName = dataString+ logFileName;
                }

                lock (logLock)
                {
                    File.AppendAllLines(basePath + "\\" + fileName, logText);
                }
            }
            catch //(Exception ex)
            {
              //  JetEazy.LoggerClass.Instance.WriteException(ex);
            }
        }

        /// <summary>
        /// Write Fail Report file
        /// </summary>
        /// <param name="logContent">Log content</param>
        public void WriteFailReport(string logContent)
        {
            try
            {
                string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                basePath = @"D:\LOG\AllinoneFailRreprt\";
                if (!Directory.Exists(basePath))
                    Directory.CreateDirectory(basePath);

                string logText = DateTime.Now.ToString("hh:mm:ss") + "," + logContent;

                string dataString = DateTime.Now.ToString("yyyy-MM-dd");
                string fileName = basePath + "\\" + dataString + "_Fail.csv";
                if (!File.Exists(fileName))
                    logText = "DateTimer,SN,KB,Screw,Laser,Result" + Environment.NewLine + logText;

                lock (logLock)
                {
                    File.AppendAllText(fileName, logText + Environment.NewLine);
                }
            }
            catch //(Exception ex)
            {
                //  JetEazy.LoggerClass.Instance.WriteException(ex);
            }
        }

        /// <summary>
        /// Write SN Report file
        /// </summary>
        /// <param name="logContent">Log content</param>
        public void WriteSNReport(string logContent)
        {
            try
            {
                string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                basePath = @"D:\LOG\AllinoneSNRreprt\";
                if (!Directory.Exists(basePath))
                    Directory.CreateDirectory(basePath);

                string logText =  DateTime.Now.ToString("hh:mm:ss")+"," + logContent ;

                string dataString = DateTime.Now.ToString("yyyy-MM-dd");
                string fileName = basePath + "\\" + dataString+"_All.csv";
                if (!File.Exists(fileName))
                    logText = "DateTimer,SFSN,OCRSN,Result" + Environment.NewLine+ logText;
                
                lock (logLock)
                {
                    File.AppendAllText(fileName, logText+Environment.NewLine);
                }
            }
            catch //(Exception ex)
            {
                //  JetEazy.LoggerClass.Instance.WriteException(ex);
            }
        }

        /// <summary>
        /// Write SN Report file SN Fail
        /// </summary>
        /// <param name="logContent">Log content</param>
        public void WriteSNReportFail(string logContent)
        {
            try
            {
                string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                basePath = @"D:\LOG\AllinoneSNRreprt\";
                if (!Directory.Exists(basePath))
                    Directory.CreateDirectory(basePath);

                string logText = DateTime.Now.ToString("hh:mm:ss") + "," + logContent;

                string dataString = DateTime.Now.ToString("yyyy-MM-dd");
                string fileName = basePath + "\\" + dataString + "_SNFail.csv";
                if (!File.Exists(fileName))
                    logText = "DateTimer,SFSN,OCRSN,Result" + Environment.NewLine + logText;

                lock (logLock)
                {
                    File.AppendAllText(fileName, logText + Environment.NewLine);
                }
            }
            catch //(Exception ex)
            {
                //  JetEazy.LoggerClass.Instance.WriteException(ex);
            }
        }

        /// <summary>
        /// Write SN Report file SN Fail
        /// </summary>
        /// <param name="logContent">Log content</param>
        public void WriteLaserReportFail(string logContent)
        {
            try
            {
                string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                basePath = @"D:\LOG\AllinoneSNRreprt\";
                if (!Directory.Exists(basePath))
                    Directory.CreateDirectory(basePath);

                string logText = DateTime.Now.ToString("hh:mm:ss") + "," + logContent;

                string dataString = DateTime.Now.ToString("yyyy-MM-dd");
                string fileName = basePath + "\\" + dataString + "_LaserFail.csv";
                if (!File.Exists(fileName))
                    logText = "DateTimer,SFSN,OCRSN,Result" + Environment.NewLine + logText;

                lock (logLock)
                {
                    File.AppendAllText(fileName, logText + Environment.NewLine);
                }
            }
            catch //(Exception ex)
            {
                //  JetEazy.LoggerClass.Instance.WriteException(ex);
            }
        }

        /// <summary>
        /// Write exception to log file
        /// </summary>
        /// <param name="exception">Exception</param>
        public void WriteException(Exception exception, string specialText = null)
        {
            if (exception != null)
            {
                Type exceptionType = exception.GetType();
                string text = string.Empty;
                if (!string.IsNullOrEmpty(specialText))
                {
                    text = text + specialText + Environment.NewLine;
                }
                text += "Exception: " + exceptionType.Name + Environment.NewLine;
                text += "               " + "Message: " + exception.Message + Environment.NewLine;
                text += "               " + "Source: " + exception.Source + Environment.NewLine;
                text += "               " + "StackTrace: " + exception.StackTrace + Environment.NewLine;
                WriteLog(text, LogType.Error,"Error");
            }
        }
    }
}