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

namespace Allinone.UISpace
{
    public partial class MainServiceCtrlUI : UserControl
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

            GETIMAGEOK,
            GETIMAGEINDEX,
            RECONNECT_HANDLE_SERVER,
        }


        const int MSDuriation = 10;

        JzTransparentPanel tpnlCover;
        VersionEnum VERSION = VersionEnum.ALLINONE;
        OptionEnum OPTION = OptionEnum.MAIN;
        JzMainServiceMachineClass MACHINE;

        JzMainServiceIOClass PLCIO
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

        Label lblReConnectServer;
        Label lblReConnectHandleServer;

        JzTimes myJzTimer = new JzTimes();
        public MainServiceCtrlUI()
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

          
            switch (Universal.OPTION)
            {
                case OptionEnum.MAIN_SERVICE:
                case OptionEnum.MAIN_X6:

                    switch (Universal.CAMACT)
                    {
                        case CameraActionMode.CAM_STATIC:

                            lblIsGetImage.Visible = false;
                            lblGetImageOK.Visible = false;
                            lblGetImageIndex.Visible = false;
                            lblIsGetImageReset.Visible = false;
                            lblHandlerOK.Visible = false;
                            //lblReConnectServer.Visible = false;

                            break;
                    }

                    break;

            }

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
            }

        }

        public void Initial(VersionEnum version, OptionEnum option, JzMainServiceMachineClass machine)
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
                    lblReConnectServer.Text = (bOK ? "Server打标连接成功" : "Server打标连接失败");
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
                    lblReConnectHandleServer.Text = (bOK ? "ServerHandle连接成功" : "ServerHandle连接失败");
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
                            lblReConnectServer.Text = "Server打标重连中";

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
                            lblReConnectHandleServer.Text = "ServerHandle重连中";
                            
                            System.Threading.Thread thread_DL_ReConnectServer = new System.Threading.Thread(_reConnectHandleServer);
                            thread_DL_ReConnectServer.Start();
                        }
                    }
                }
                #endregion
            }


            MACHINE.Tick();
            lblIsGetImage.BackColor = (PLCIO.IsGetImage ? Color.Green : Color.Black);
            lblIsStart.BackColor = (PLCIO.IsStart ? Color.Green : Color.Black);
            lblIsGetImageReset.BackColor = (PLCIO.IsGetImageReset ? Color.Green : Color.Black);
            lblTopLight.BackColor = (PLCIO.TopLight ? Color.Green : Color.Black);
            lblFrontLight.BackColor = (PLCIO.FrontLight ? Color.Green : Color.Black);
            lblBackLight.BackColor = (PLCIO.BackLight ? Color.Green : Color.Black);
            lblReady.BackColor = (PLCIO.Ready ? Color.Green : Color.Black);
            lblBusy.BackColor = (PLCIO.Busy ? Color.Green : Color.Black);
            lblPass.BackColor = (PLCIO.Pass ? Color.Red : Color.Black);
            lblFail.BackColor = (PLCIO.Fail ? Color.Green : Color.Black);
            lblGetImageOK.BackColor = (PLCIO.GetImageOK ? Color.Green : Color.Black);
            lblGetImageIndex.BackColor = Color.Black;
            lblGetImageIndex.Text = JetEazy.PlugSpace.CamActClass.Instance.StepCurrent.ToString() + " 总[" +
                JetEazy.PlugSpace.CamActClass.Instance.StepCount.ToString() + "]";

            if(INI.IsReadHandlerOKSign)
                lblHandlerOK.BackColor = (PLCIO.IsHandlerOK ? Color.Green : Color.Black);

            myJzTimer.Cut();
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
    }
}
