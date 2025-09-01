using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using JetEazy;
using JetEazy.BasicSpace;
using JetEazy.UISpace;
using Allinone.ControlSpace;
using Allinone.ControlSpace.MachineSpace;
using Allinone.ControlSpace.IOSpace;
using JetEazy.PlugSpace;
using System.Threading;
using System.Diagnostics;

namespace Allinone.UISpace
{
    public partial class MainX6CtrlUI : UserControl
    {
        enum TagEnum
        {
            TOPLIGHT,
            FRONTLIGHT,
            BACKLIGHT,
            READY,
            BUSY,
            PASS,
            FAIL,
            RECONNECTSERVER,
            TCPCOMPLETE,

            GETIMAGEOK,
            GETIMAGEINDEX,
            RECONNECT_HANDLE_SERVER,
            CIPMAPPING,
        }


        const int MSDuriation = 10;

        JzTransparentPanel tpnlCover;
        VersionEnum VERSION = VersionEnum.ALLINONE;
        OptionEnum OPTION = OptionEnum.MAIN;
        JzMainX6MachineClass MACHINE;

        System.Threading.Thread m_ThreadPlc = null;
        bool m_ThRunning = false;

        JzMainX6IOClass PLCIO
        {
            get
            {
                return MACHINE.PLCIO;
            }
        }

        ClientSocket X6_HANDLE_CLIENT
        {
            get { return Universal.X6_HANDLE_CLIENT; }
        }
        ClientSocket X6_LASER_CLIENT
        {
            get { return Universal.X6_LASER_CLIENT; }
        }

        Label lblIsStart;
        Label lblIsGetImage;
        Label lblIsGetImageReset;

        Label lblTopLight;
        Label lblFrontLight;
        Label lblBackLight;
        Label lblReady;
        Label lblBusy;
        Label lblPass;
        Label lblFail;
        Label lblGetImageOK;
        Label lblGetImageIndex;
        Label lblHandlerOK;
        Label lblTcpComplete;


        Label lblSoftwareReady;
        Label lblHeart;


        Label lblReConnectServer;
        Label lblReConnectHandleServer;
        Label lblCipMapping;

        JzTimes myJzTimer = new JzTimes();
        public MainX6CtrlUI()
        {
            InitializeComponent();
            InitialInternal();
        }

        void InitialInternal()
        {
            tpnlCover = jzTransparentPanel1;

            lblIsStart = label1;

            lblTopLight = label2;
            lblFrontLight = label3;
            lblBackLight = label6;
            lblReady = label7;
            lblBusy = label8;
            lblPass = label9;
            lblFail = label10;
            lblReConnectServer = label4;
            lblIsGetImage = label5;
            lblGetImageOK = label11;
            lblGetImageIndex = label12;
            lblIsGetImageReset = label13;
            lblReConnectHandleServer = label14;
            lblHandlerOK = label15;
            lblTcpComplete = label16;
            lblCipMapping = label17;

            lblSoftwareReady = label18;
            lblHeart = label19;

            lblTopLight.Tag = TagEnum.TOPLIGHT;
            lblFrontLight.Tag = TagEnum.FRONTLIGHT;
            lblBackLight.Tag = TagEnum.BACKLIGHT;
            lblReady.Tag = TagEnum.READY;
            lblBusy.Tag = TagEnum.BUSY;
            lblPass.Tag = TagEnum.PASS;
            lblFail.Tag = TagEnum.FAIL;
            lblReConnectServer.Tag = TagEnum.RECONNECTSERVER;
            lblGetImageOK.Tag = TagEnum.GETIMAGEOK;
            lblGetImageIndex.Tag = TagEnum.GETIMAGEINDEX;
            lblReConnectHandleServer.Tag = TagEnum.RECONNECT_HANDLE_SERVER;
            lblTcpComplete.Tag = TagEnum.TCPCOMPLETE;
            lblCipMapping.Tag = TagEnum.CIPMAPPING;

            lblTopLight.DoubleClick += lbl_DoubleClick;
            lblFrontLight.DoubleClick += lbl_DoubleClick;
            lblBackLight.DoubleClick += lbl_DoubleClick;
            lblReady.DoubleClick += lbl_DoubleClick;
            lblBusy.DoubleClick += lbl_DoubleClick;
            lblPass.DoubleClick += lbl_DoubleClick;
            lblFail.DoubleClick += lbl_DoubleClick;
            lblReConnectServer.DoubleClick += lbl_DoubleClick;
            lblGetImageOK.DoubleClick += lbl_DoubleClick;
            lblGetImageIndex.DoubleClick += lbl_DoubleClick;
            lblTcpComplete.DoubleClick += lbl_DoubleClick;
            lblTcpComplete.BackColor = Color.Black;
            lblCipMapping.DoubleClick += lbl_DoubleClick;
            lblCipMapping.Visible = false;

            lblSoftwareReady.Visible = false;
            lblHeart.Visible = false;
            lblSoftwareReady.DoubleClick += LblSoftwareReady_DoubleClick;

            switch (Universal.OPTION)
            {
                case OptionEnum.MAIN_X6:

                    if (INI.IsOpenCip)
                    {
                        lblCipMapping.Visible = true;
                    }

                    lblIsStart.Text = "取像";
                    lblIsGetImage.Text = "测试";
                    lblIsGetImageReset.Text = "复位";
                    lblHandlerOK.Text = "完成";

                    switch (Universal.CAMACT)
                    {
                        case CameraActionMode.CAM_STATIC:

                            lblIsGetImage.Visible = false;
                            lblGetImageOK.Visible = false;
                            lblGetImageIndex.Visible = false;
                            lblIsGetImageReset.Visible = false;
                            lblHandlerOK.Visible = false;
                            lblReConnectServer.Visible = false;
                            lblReConnectHandleServer.Visible = false;
                            lblTcpComplete.Visible = false;

                            break;
                    }

                    break;

            }

        }

        private void LblSoftwareReady_DoubleClick(object sender, EventArgs e)
        {
            MACHINE.PLCIO.SoftwareReady = !MACHINE.PLCIO.SoftwareReady;
        }

        private void lbl_DoubleClick(object sender, EventArgs e)
        {
            TagEnum KEYS = (TagEnum)((Label)sender).Tag;

            switch (KEYS)
            {
                case TagEnum.TOPLIGHT:
                    PLCIO.TopLight = !PLCIO.TopLight;
                    break;
                case TagEnum.FRONTLIGHT:
                    PLCIO.FrontLight = !PLCIO.FrontLight;
                    break;
                case TagEnum.BACKLIGHT:
                    PLCIO.BackLight = !PLCIO.BackLight;
                    break;
                case TagEnum.READY:
                    PLCIO.Ready = !PLCIO.Ready;
                    break;
                case TagEnum.BUSY:
                    PLCIO.Busy = !PLCIO.Busy;
                    break;
                case TagEnum.PASS:
                    PLCIO.Pass = !PLCIO.Pass;
                    break;
                case TagEnum.FAIL:
                    PLCIO.Fail = !PLCIO.Fail;
                    break;
                case TagEnum.GETIMAGEOK:
                    PLCIO.GetImageOK = !PLCIO.GetImageOK;
                    break;
                case TagEnum.GETIMAGEINDEX:
                    JetEazy.PlugSpace.CamActClass.Instance.ResetStepCurrent();
                    break;
                case TagEnum.RECONNECTSERVER:


                    //if (DialogResult.OK != MessageBox.Show("是否重新连接Server ？", "重连Server", MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
                    //{
                    //    return;
                    //}
                    //m_ReConnectIndex = 0;
                    //m_ReConnecting = true;
                    //ClientSocket.Instance.Host = INI.tcp_ip;
                    //ClientSocket.Instance.Port = INI.tcp_port;
                    //int iret = ClientSocket.Instance.ReConnectServer();
                    //if (iret != 0)
                    //{
                    //    m_ReConnectIndex = 10;
                    //    MessageBox.Show("重新连接服务器错误，请检查。", "重连Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //}
                    //m_ReConnecting = false;


                    break;
                case TagEnum.TCPCOMPLETE:

                    int _currentStep = CamActClass.Instance.StepCurrent;
                    _tcpSendCompleteOKSign(1, _currentStep, -1);

                    break;
                case TagEnum.CIPMAPPING:

                    OnTrigger(ActionEnum.ACT_CIPMAPPING, "M");

                    break;
            }

        }

        public void Initial(VersionEnum version, OptionEnum option, JzMainX6MachineClass machine)
        {
            VERSION = version;
            OPTION = option;
            MACHINE = machine;

            tpnlCover.Location = new Point(0, 0);
            tpnlCover.Size = new Size(336, 303);

            myJzTimer.Cut();

            lblReConnectServer.BackColor = Color.Green;
            lblReConnectServer.Text = "Server打标连接成功";

            lblReConnectHandleServer.BackColor = Color.Green;
            lblReConnectHandleServer.Text = "ServerHandle连接成功";

            //lblReConnectServer.BackColor = (ClientSocket.Instance.IsConnecting ? Color.Green : Color.Red);
            //lblReConnectServer.BackColor = (ClientSocket.Instance.IsConnecting ? Color.Green : Color.Red);
            if (Universal.m_UseCommToDLHandle)
            {
                updateReConnectServerUI(X6_LASER_CLIENT.IsConnecting);
                X6_LASER_CLIENT.TriggerStringAction += X6_LASER_CLIENT_TriggerStringAction;

                updateReConnectHandleServerUI(X6_HANDLE_CLIENT.IsConnecting);
                X6_HANDLE_CLIENT.TriggerStringAction += X6_HANDLE_CLIENT_TriggerStringAction;
            }

            switch (Universal.FACTORYNAME)
            {
                case FactoryName.RIYUEXING:
                    lblSoftwareReady.Visible = true;
                    lblHeart.Visible = true;
                    break;
            }

            if (Universal.IsUseThreadReviceTcp)
            {
                if (m_ThreadPlc == null)
                {
                    m_ThRunning = true;
                    m_ThreadPlc = new System.Threading.Thread(new System.Threading.ThreadStart(PlcTick));
                    m_ThreadPlc.IsBackground = true;
                    m_ThreadPlc.Start();
                }
            }
        }

        private void X6_LASER_CLIENT_TriggerStringAction(string opstr)
        {
            string[] str = opstr.Split(',');
            switch (str[0])
            {
                case "S"://状态
                    //lblReConnectServer.BackColor = (str[1] == "OK" ? Color.Green : Color.Red);
                    updateReConnectServerUI(str[1] == "OK");
                    break;
            }
        }

        private void X6_HANDLE_CLIENT_TriggerStringAction(string opstr)
        {
            string[] str = opstr.Split(',');
            switch (str[0])
            {
                case "S"://状态
                    //lblReConnectServer.BackColor = (str[1] == "OK" ? Color.Green : Color.Red);
                    updateReConnectHandleServerUI(str[1] == "OK");
                    break;
            }
        }

        private void updateReConnectServerUI(bool bOK)
        {

            try
            {
                this.Invoke(new Action(() =>
                {
                    lblReConnectServer.BackColor = (bOK ? Color.Green : Color.Red);
                    lblReConnectServer.Text = ToChangeLanguage((bOK ? "Server打标连接成功" : "Server打标连接失败"));
                }));
            }
            catch
            {

            }
        }
        private void updateReConnectHandleServerUI(bool bOK)
        {

            try
            {
                this.Invoke(new Action(() =>
                {
                    lblReConnectHandleServer.BackColor = (bOK ? Color.Green : Color.Red);
                    lblReConnectHandleServer.Text = ToChangeLanguage((bOK ? "ServerHandle连接成功" : "ServerHandle连接失败"));
                }));
            }
            catch
            {

            }
        }


        #region 服务器重新连接

        int m_ReConnectIndex = 0;
        int m_ReConnectCount = 10;
        bool m_ReConnecting = false;
        JzTimes m_ReConnectTime = new JzTimes();

        bool m_ReHandleConnecting = false;
        JzTimes m_ReHandleConnectTime = new JzTimes();

        #endregion

        public void SetEnable(bool isenable)
        {
            tpnlCover.Visible = !isenable;

            this.BackColor = (isenable ? SystemColors.Control : Color.DarkGray);
        }
        public void Tick()
        {
            if (myJzTimer.msDuriation < MSDuriation)
                return;

            if (Universal.m_UseCommToDLHandle)
            {
                #region 打标服务器重连
                if (X6_LASER_CLIENT.IsConnecting)
                {
                    m_ReConnectTime.Cut();
                }
                else
                {
                    if (m_ReConnectTime.msDuriation > 3 * 1000)
                    {
                        m_ReConnectTime.Cut();
                        if (!m_ReConnecting)
                        {
                            m_ReConnecting = true;
                            //m_ReConnectIndex++;

                            lblReConnectServer.BackColor = Color.Red;
                            lblReConnectServer.Text = ToChangeLanguage("Server打标重连中");

                            System.Threading.Thread thread_DL_ReConnectServer = new System.Threading.Thread(_reConnectServer);
                            thread_DL_ReConnectServer.Start();
                        }
                    }
                }
                #endregion

                lblReConnectHandleServer.Visible = INI.tcp_handle_open;
                #region handle服务器重连
                if (X6_HANDLE_CLIENT.IsConnecting)
                {
                    m_ReHandleConnectTime.Cut();
                }
                else
                {
                    if (m_ReHandleConnectTime.msDuriation > 3 * 1000)
                    {
                        m_ReHandleConnectTime.Cut();
                        if (!m_ReHandleConnecting)
                        {
                            m_ReHandleConnecting = true;
                            //m_ReConnectIndex++;

                            lblReConnectHandleServer.BackColor = Color.Red;
                            lblReConnectHandleServer.Text = ToChangeLanguage("ServerHandle重连中");

                            System.Threading.Thread thread_DL_ReConnectServer = new System.Threading.Thread(_reConnectHandleServer);
                            thread_DL_ReConnectServer.Start();
                        }
                    }
                }
                #endregion
            }

            if (!Universal.IsUseThreadReviceTcp)
                MACHINE.Tick();
            lblIsGetImage.BackColor = (PLCIO.IsGetImage ? Color.Green : Color.Black);
            lblIsStart.BackColor = (PLCIO.IsStart ? Color.Green : Color.Black);
            lblIsGetImageReset.BackColor = (PLCIO.IsGetImageReset ? Color.Green : Color.Black);
            lblTopLight.BackColor = (PLCIO.TopLight ? Color.Green : Color.Black);
            lblFrontLight.BackColor = (PLCIO.FrontLight ? Color.Green : Color.Black);
            lblBackLight.BackColor = (PLCIO.BackLight ? Color.Green : Color.Black);
            lblReady.BackColor = (PLCIO.Ready ? Color.Green : Color.Black);
            lblBusy.BackColor = (PLCIO.Busy ? Color.Green : Color.Black);
            lblPass.BackColor = (PLCIO.Pass ? Color.Green : Color.Black);
            lblFail.BackColor = (PLCIO.Fail ? Color.Red : Color.Black);
            lblGetImageOK.BackColor = (PLCIO.GetImageOK ? Color.Green : Color.Black);
            lblGetImageIndex.BackColor = Color.Black;
            lblGetImageIndex.Text = JetEazy.PlugSpace.CamActClass.Instance.StepCurrent.ToString() + " Total [" +
                JetEazy.PlugSpace.CamActClass.Instance.StepCount.ToString() + "]";

            if (INI.IsReadHandlerOKSign && !INI.IsNoUseHandlerOKSign)
                lblHandlerOK.BackColor = (PLCIO.IsHandlerOK ? Color.Green : Color.Black);

            switch(Universal.FACTORYNAME)
            {
                case FactoryName.RIYUEXING:
                    lblSoftwareReady.BackColor = (PLCIO.SoftwareReady ? Color.Green : Color.Black);
                    lblHeart.BackColor = (PLCIO.HeartBeat ? Color.Green : Color.Black);
                    break;
            }
           

            myJzTimer.Cut();
        }


        Stopwatch m_HeartTime = new Stopwatch();

        public void PlcTick()
        {
           
            switch (Universal.FACTORYNAME)
            {
                case FactoryName.RIYUEXING:
                    MACHINE.PLCIO.SoftwareReady = true;
                    break;
            }
            while (m_ThRunning)
            {
                MACHINE.Tick();
                Thread.Sleep(50);

                switch (Universal.FACTORYNAME)
                {
                    case FactoryName.RIYUEXING:
                        if (m_HeartTime.ElapsedMilliseconds > 1000)
                        {
                            m_HeartTime.Restart();
                            MACHINE.PLCIO.HeartBeat = !MACHINE.PLCIO.HeartBeat;
                        }
                        else
                        {
                            if (!m_HeartTime.IsRunning)
                                m_HeartTime.Restart();
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 释放资源并关闭线程
        /// </summary>
        public void SDDispose()
        {
            MACHINE.PLCIO.Ready = false;
            MACHINE.PLCIO.Busy = false;
            PLCIO.GetImageOK = false;

            MACHINE.PLCIO.Pass = false;
            MACHINE.PLCIO.Fail = false;

            MACHINE.PLCIO.TopLight = false;
            MACHINE.PLCIO.FrontLight = false;
            MACHINE.PLCIO.BackLight = false;

            switch (Universal.FACTORYNAME)
            {
                case FactoryName.RIYUEXING:
                    MACHINE.PLCIO.SoftwareReady = false;
                    MACHINE.PLCIO.HeartBeat = false;
                    break;
            }

            m_ThRunning = false;
            if (m_ThreadPlc != null)
            {
                m_ThreadPlc.Abort();
                m_ThreadPlc = null;
            }
        }

        private void _reConnectServer()
        {
            X6_LASER_CLIENT.Host = INI.tcp_ip;
            X6_LASER_CLIENT.Port = INI.tcp_port;
            int iret = X6_LASER_CLIENT.ReConnectServer();
            m_ReConnecting = false;
        }
        private void _reConnectHandleServer()
        {
            X6_HANDLE_CLIENT.Host = INI.tcp_handle_ip;
            X6_HANDLE_CLIENT.Port = INI.tcp_handle_port;
            int iret = X6_HANDLE_CLIENT.ReConnectServer();
            m_ReHandleConnecting = false;
        }

        private void _tcpSendCompleteOKSign(int eCompleteSign, int eCurrentStep, int eResult)
        {
            string Str = "CompleteSign=" + eCompleteSign.ToString();
            Str += "CurrentStep=" + eCurrentStep.ToString();
            Str += "Result=" + eResult.ToString();

            X6_HANDLE_CLIENT.Log.Log2("tcpCmd.CMD_SENDCOMPLETESIGN" + Str);
            string _cmd = eCompleteSign.ToString() + "," + eCurrentStep.ToString() + "," + eResult.ToString();
            byte[] bytedata2 = Encoding.UTF8.GetBytes(_cmd);

            byte[] bytedata = new byte[32 + bytedata2.Length];
            bytedata[0] = 29;
            //bytedata[4] = 4;
            bytedata[8] = 0;
            //bytedata[32] = (iret == 0 ? (byte)1 : (byte)3);

            for (int i = 0; i < bytedata2.Length; i++)
            {
                bytedata[32 + i] = bytedata2[i];
            }

            int tu5x = bytedata2.Length;
            bytedata[4] = (byte)(tu5x & 0xFF);
            bytedata[5] = (byte)(tu5x >> 8 & 0xFF);
            bytedata[6] = (byte)(tu5x >> 16 & 0xFF);
            bytedata[7] = (byte)(tu5x >> 24 & 0xFF);

            //byte[] a2 = bytes.Skip(4).Take(4).ToArray();
            //Int32 aa2 = BitConverter.ToInt32(bytes5x, 0);

            //bytedata[4] = (byte)bytedata2.Length;//数据长度

            try
            {
                X6_HANDLE_CLIENT.Send(bytedata);
            }
            catch (Exception ex)
            {
                X6_HANDLE_CLIENT.Log.Log2("tcpCmd.CMD_SENDCOMPLETESIGN:Exception" + ex.Message);
            }

        }


        string ToChangeLanguage(string eText)
        {
            string retStr = eText;
            retStr = LanguageExClass.Instance.GetLanguageText(eText);
            return retStr;
        }

        public delegate void TriggerHandler(ActionEnum action, string opstr);
        public event TriggerHandler TriggerAction;
        public void OnTrigger(ActionEnum action, string opstr)
        {
            if (TriggerAction != null)
            {
                TriggerAction(action, opstr);
            }
        }
    }
}
