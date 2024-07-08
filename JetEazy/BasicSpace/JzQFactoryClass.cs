using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JetEazy.BasicSpace
{
    public enum QFactoryErrorCode : int
    {
        /// <summary>
        /// 正常
        /// </summary>
        Err_Normal = 0,

        /// <summary>
        /// 關閉系統
        /// </summary>
        Err_Abnormal_CloseSystem = 2000,

        /// <summary>
        /// 相機異常
        /// </summary>
        Err_Abnormal_Cam = 1000,
        /// <summary>
        /// PLC異常
        /// </summary>
        Err_Abnormal_Plc_Disconnect = 1001,
        /// <summary>
        /// 急停
        /// </summary>
        Err_Abnormal_EMC = 1002,
        /// <summary>
        /// 馬達異常
        /// </summary>
        Err_Abnormal_Motor = 1003,
    }
    public class JzQFactoryClass
    {

        SFSATPortal.Portal m_Portal = null;

        private string m_EQ_SN = "0";
        private string m_EQ_LocationID = "0";
        private string m_EQ_LocationID2 = "0";

        private string m_Station = "EQ";
        private string m_Step = "EQMonitor";

        private QFactoryErrorCode m_QFactoryErrorCode = QFactoryErrorCode.Err_Normal;
        private bool m_IsUploadRunning = false;
        private string m_QFactoryTmpPath = @"D:\QFactoryTmp";

        private bool m_IsInit = false;

        public string EQ_SN
        {
            get { return m_EQ_SN; }
            set { m_EQ_SN = value; }
        }
        public string EQ_LocationID
        {
            get { return m_EQ_LocationID; }
            set { m_EQ_LocationID = value; }
        }
        public string EQ_LocationID2
        {
            get { return m_EQ_LocationID2; }
            set { m_EQ_LocationID2 = value; }
        }
        public string Station
        {
            get { return m_Station; }
            set { m_Station = value; }
        }
        public string Step
        {
            get { return m_Step; }
            set { m_Step = value; }
        }

        public JzQFactoryClass()
        {

        }
        public void Init(string eEQ_SN, string eEQ_LocationID, string eEQ_LocationID2, string eStation, string eStep)
        {
            m_EQ_SN = eEQ_SN;
            m_EQ_LocationID = eEQ_LocationID;
            m_EQ_LocationID2 = eEQ_LocationID2;

            m_Station = eStation;
            m_Step = eStep;

            m_Portal = new SFSATPortal.Portal();

            m_IsInit = true;
            _log("10000,[INIT]");
        }
        public void Dispose()
        {
            _log("10001,[Dispose]");
            m_IsInit = false;
        }
        public string Send(QFactoryErrorCode eErrorCode)
        {

            m_QFactoryErrorCode = eErrorCode;

            System.Threading.Thread _threadQFactory = new System.Threading.Thread(new System.Threading.ThreadStart(Send2));
            _threadQFactory.Priority = System.Threading.ThreadPriority.Lowest;
            _threadQFactory.IsBackground = true;
            _threadQFactory.Start();

            return "";

            if (!m_IsInit)
            {
                _log("10002,[Send_NO_INIT]");
                return "NO_INIT";
            }

            //string data = "EQ_SN =" + EQ_SN + ";$;" +
            //    "EQ_LocationID =" + EQ_LocationID + ";$;" + 
            //    "EQ_LocationID2 =" + EQ_LocationID2 + ";$;EQ_Status=Normal;$;EQ_ErrCode=0;$;EQ_Msg=;$;EQ_Datetime=" + DateTime.Now.ToString("yyyyMMddHHmmss") + ";$;";

            //string Result = "";
            //if (Universal.myPortal != null)
            //    Result = Universal.myPortal.ATPortal(INI.STATION, "EQMonitor", data);

            string _sendData = "";

            _sendData += "EQ_SN=" + m_EQ_SN + ";$;";
            _sendData += "EQ_LocationID=" + m_EQ_LocationID + ";$;";
            _sendData += "EQ_LocationID2=" + m_EQ_LocationID2 + ";$;";

            int iErrorCode = (int)eErrorCode;

            switch (eErrorCode)
            {
                case QFactoryErrorCode.Err_Normal:
                    _sendData += "EQ_Status=" + "Normal" + ";$;";
                    _sendData += "EQ_ErrCode=" + "0" + ";$;";
                    _sendData += "EQ_Msg=" + "" + ";$;";
                    break;
                case QFactoryErrorCode.Err_Abnormal_Cam:
                    _sendData += "EQ_Status=" + "Abnormal" + ";$;";
                    _sendData += "EQ_ErrCode=" + iErrorCode.ToString() + ";$;";
                    _sendData += "EQ_Msg=" + "Camera Connect Fail" + ";$;";
                    break;
                case QFactoryErrorCode.Err_Abnormal_EMC:
                    _sendData += "EQ_Status=" + "Abnormal" + ";$;";
                    _sendData += "EQ_ErrCode=" + iErrorCode.ToString() + ";$;";
                    _sendData += "EQ_Msg=" + "E-Stop Trigger" + ";$;";
                    break;
                case QFactoryErrorCode.Err_Abnormal_Motor:
                    _sendData += "EQ_Status=" + "Abnormal" + ";$;";
                    _sendData += "EQ_ErrCode=" + iErrorCode.ToString() + ";$;";
                    _sendData += "EQ_Msg=" + "Motor Abnormalities" + ";$;";
                    break;
                case QFactoryErrorCode.Err_Abnormal_Plc_Disconnect:
                    _sendData += "EQ_Status=" + "Abnormal" + ";$;";
                    _sendData += "EQ_ErrCode=" + iErrorCode.ToString() + ";$;";
                    _sendData += "EQ_Msg=" + "PLC Connect Fail" + ";$;";
                    break;
                case QFactoryErrorCode.Err_Abnormal_CloseSystem:
                    _sendData += "EQ_Status=" + "Abnormal" + ";$;";
                    _sendData += "EQ_ErrCode=" + iErrorCode.ToString() + ";$;";
                    _sendData += "EQ_Msg=" + "System Close" + ";$;";
                    break;
            }

            _sendData += "EQ_Datetime=" + DateTime.Now.ToString("yyyyMMddHHmmss") + ";$;";

            if (m_Portal == null)
            {
                m_Portal = new SFSATPortal.Portal();
                _log("10003,[Send_new Portal]");
            }

            _log("QFactory[Send]" + _sendData);

            string _result = m_Portal.ATPortal(m_Station, m_Step, _sendData);

            _log("QFactory[Receive]" + _result);

            return _result;
        }
        public void Send2()
        {

            QFactoryErrorCode eErrorCode = m_QFactoryErrorCode;

            if (!m_IsInit)
            {
                _log("10002,[Send_NO_INIT]");
                //return "NO_INIT";
                return;
            }

            //string data = "EQ_SN =" + EQ_SN + ";$;" +
            //    "EQ_LocationID =" + EQ_LocationID + ";$;" + 
            //    "EQ_LocationID2 =" + EQ_LocationID2 + ";$;EQ_Status=Normal;$;EQ_ErrCode=0;$;EQ_Msg=;$;EQ_Datetime=" + DateTime.Now.ToString("yyyyMMddHHmmss") + ";$;";

            //string Result = "";
            //if (Universal.myPortal != null)
            //    Result = Universal.myPortal.ATPortal(INI.STATION, "EQMonitor", data);

            string _sendData = "";

            _sendData += "EQ_SN=" + m_EQ_SN + ";$;";
            _sendData += "EQ_LocationID=" + m_EQ_LocationID + ";$;";
            _sendData += "EQ_LocationID2=" + m_EQ_LocationID2 + ";$;";

            int iErrorCode = (int)eErrorCode;

            switch (eErrorCode)
            {
                case QFactoryErrorCode.Err_Normal:
                    _sendData += "EQ_Status=" + "Normal" + ";$;";
                    _sendData += "EQ_ErrCode=" + "0" + ";$;";
                    _sendData += "EQ_Msg=" + "" + ";$;";
                    break;
                case QFactoryErrorCode.Err_Abnormal_Cam:
                    _sendData += "EQ_Status=" + "Abnormal" + ";$;";
                    _sendData += "EQ_ErrCode=" + iErrorCode.ToString() + ";$;";
                    _sendData += "EQ_Msg=" + "Camera Connect Fail" + ";$;";
                    break;
                case QFactoryErrorCode.Err_Abnormal_EMC:
                    _sendData += "EQ_Status=" + "Abnormal" + ";$;";
                    _sendData += "EQ_ErrCode=" + iErrorCode.ToString() + ";$;";
                    _sendData += "EQ_Msg=" + "E-Stop Trigger" + ";$;";
                    break;
                case QFactoryErrorCode.Err_Abnormal_Motor:
                    _sendData += "EQ_Status=" + "Abnormal" + ";$;";
                    _sendData += "EQ_ErrCode=" + iErrorCode.ToString() + ";$;";
                    _sendData += "EQ_Msg=" + "Motor Abnormalities" + ";$;";
                    break;
                case QFactoryErrorCode.Err_Abnormal_Plc_Disconnect:
                    _sendData += "EQ_Status=" + "Abnormal" + ";$;";
                    _sendData += "EQ_ErrCode=" + iErrorCode.ToString() + ";$;";
                    _sendData += "EQ_Msg=" + "PLC Connect Fail" + ";$;";
                    break;
                case QFactoryErrorCode.Err_Abnormal_CloseSystem:
                    _sendData += "EQ_Status=" + "Abnormal" + ";$;";
                    _sendData += "EQ_ErrCode=" + iErrorCode.ToString() + ";$;";
                    _sendData += "EQ_Msg=" + "System Close" + ";$;";
                    break;
            }

            _sendData += "EQ_Datetime=" + DateTime.Now.ToString("yyyyMMddHHmmss") + ";$;";

            if (m_IsUploadRunning)
            {
                if (!System.IO.Directory.Exists(m_QFactoryTmpPath))
                    System.IO.Directory.CreateDirectory(m_QFactoryTmpPath);

                string _data = "";
                _data += m_Station + Environment.NewLine;
                _data += m_Step + Environment.NewLine;
                _data += _sendData + Environment.NewLine;

                string _filename = DateTime.Now.ToString("yyyyMMddHHmmss") + ".qfactory";

                _saveData(_data, m_QFactoryTmpPath + "\\" + _filename);

                return;
            }

            m_IsUploadRunning = true;

            if (m_Portal == null)
            {
                m_Portal = new SFSATPortal.Portal();
                _log("10003,[Send_new Portal]");
            }

            _log("QFactory[Send]" + _sendData);
            string _result = "";
            try
            {
                _result = m_Portal.ATPortal(m_Station, m_Step, _sendData);
            }
            catch (Exception ex)
            {
                _result = "Send2=" + ex.Message;
            }

            _log("QFactory[Receive]" + _result);

            m_IsUploadRunning = false;

            //return _result;
        }
        private void _saveData(string DataStr, string FileName)
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
        private void _log(string eMesssge)
        {
            //lock (LAST_RECIPE_NAME)
            //{
            string strPath = "D:\\log\\log.QFactory\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
            if (!System.IO.Directory.Exists(strPath))
                System.IO.Directory.CreateDirectory(strPath);

            string strFileName = strPath + DateTime.Now.ToString("yyyyMMdd_HH") + ".log";
            System.IO.StreamWriter stm = null;

            try
            {
                stm = new System.IO.StreamWriter(strFileName, true, System.Text.Encoding.Default);
                stm.Write(DateTime.Now.ToString("HH:mm:ss"));
                stm.Write(",QFactory");
                stm.Write(", ");
                stm.WriteLine(eMesssge);
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
            //}
        }
    }
}
