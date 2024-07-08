using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

namespace Allinone.UISpace.CTRLUISpace
{
    public partial class AllinoneSDM1CtrlUI : UserControl
    {

        enum TagEnum
        {
            TOPLIGHT,
            //FRONTLIGHT,
            //BACKLIGHT,
            READY,
            BUSY,
            PASS,
            FAIL,
            //RECONNECTSERVER,

            GETIMAGEOK,
            GETIMAGEINDEX,

            RESET,
            ONEKEYGETIMAGE,

        }


        const int MSDuriation = 10;

        AxisMotionUI[] AxisMotionControl = new AxisMotionUI[1];

        JzTransparentPanel tpnlCover;
        VersionEnum VERSION = VersionEnum.ALLINONE;
        OptionEnum OPTION = OptionEnum.MAIN;
        JzMainSDM1MachineClass MACHINE;

        JzMainSDM1IOClass PLCIO
        {
            get
            {
                return MACHINE.PLCIO;
            }
        }

        Label lblIsStart;
        Label lblIsReset;
        Label lblIsEMC;

        Label lblTopLight;
        //Label lblFrontLight;
        //Label lblBackLight;
        Label lblReady;
        Label lblBusy;
        Label lblPass;
        Label lblFail;
        Label lblGetImageOK;
        Label lblGetImageIndex;

        //Label lblReConnectServer;

        Button btnReset;
        Button btnOnekeyGetImage;

        JzTimes myJzTimer = new JzTimes();

        public AllinoneSDM1CtrlUI()
        {
            InitializeComponent();
            InitialInternal();
        }

        void InitialInternal()
        {
            tpnlCover = jzTransparentPanel1;
            AxisMotionControl[0] = axisMotionUI1;

            lblIsStart = label15;

            lblTopLight = label14;
            //lblFrontLight = label3;
            //lblBackLight = label6;
            lblReady = label7;
            lblBusy = label8;
            lblPass = label9;
            lblFail = label10;
            //lblReConnectServer = label4;
            lblIsReset = label2;
            lblGetImageOK = label1;
            lblGetImageIndex = label12;
            lblIsEMC = label13;

            btnReset = button1;
            btnOnekeyGetImage = button3;

            lblTopLight.Tag = TagEnum.TOPLIGHT;
            //lblFrontLight.Tag = TagEnum.FRONTLIGHT;
            //lblBackLight.Tag = TagEnum.BACKLIGHT;
            lblReady.Tag = TagEnum.READY;
            lblBusy.Tag = TagEnum.BUSY;
            lblPass.Tag = TagEnum.PASS;
            lblFail.Tag = TagEnum.FAIL;
            //lblReConnectServer.Tag = TagEnum.RECONNECTSERVER;
            lblGetImageOK.Tag = TagEnum.GETIMAGEOK;
            lblGetImageIndex.Tag = TagEnum.GETIMAGEINDEX;

            btnOnekeyGetImage.Tag = TagEnum.ONEKEYGETIMAGE;
            btnReset.Tag = TagEnum.RESET;

            lblTopLight.DoubleClick += lbl_DoubleClick;
            //lblFrontLight.DoubleClick += lbl_DoubleClick;
            //lblBackLight.DoubleClick += lbl_DoubleClick;
            lblReady.DoubleClick += lbl_DoubleClick;
            lblBusy.DoubleClick += lbl_DoubleClick;
            lblPass.DoubleClick += lbl_DoubleClick;
            lblFail.DoubleClick += lbl_DoubleClick;
            //lblReConnectServer.DoubleClick += lbl_DoubleClick;
            lblGetImageOK.DoubleClick += lbl_DoubleClick;
            lblGetImageIndex.DoubleClick += lbl_DoubleClick;

            btnReset.Click += Btn_Click;
            btnOnekeyGetImage.Click += Btn_Click;

            switch (Universal.OPTION)
            {
                case OptionEnum.MAIN_SDM1:

                    switch (Universal.CAMACT)
                    {
                        case CameraActionMode.CAM_STATIC:

                            lblIsReset.Visible = false;
                            lblGetImageOK.Visible = false;
                            lblGetImageIndex.Visible = false;
                            lblIsEMC.Visible = false;

                            //lblReConnectServer.Visible = false;

                            break;
                    }

                    break;

            }

        }

        private void Btn_Click(object sender, EventArgs e)
        {
            TagEnum KEYS = (TagEnum)((Button)sender).Tag;
            CommonLogClass.Instance.LogMessage("SDM1_CTRL[BTN]:" + KEYS.ToString());
            switch (KEYS)
            {
                case TagEnum.RESET:
                    OnTrigger(ActionEnum.ACT_RESET, "");
                    break;
                case TagEnum.ONEKEYGETIMAGE:
                    OnTrigger(ActionEnum.ACT_ONEKEYGETIMAGE, "");
                    break;
            }
        }

        private void lbl_DoubleClick(object sender, EventArgs e)
        {
            TagEnum KEYS = (TagEnum)((Label)sender).Tag;
            CommonLogClass.Instance.LogMessage("SDM1_CTRL[LBL]:" + KEYS.ToString());
            switch (KEYS)
            {
                case TagEnum.TOPLIGHT:
                    PLCIO.TopLight = !PLCIO.TopLight;
                    break;
                //case TagEnum.FRONTLIGHT:
                //    PLCIO.FrontLight = !PLCIO.FrontLight;
                //    break;
                //case TagEnum.BACKLIGHT:
                //    PLCIO.BackLight = !PLCIO.BackLight;
                //    break;
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
                //case TagEnum.RECONNECTSERVER:


                //    if (DialogResult.OK != MessageBox.Show("是否重新连接Server ？", "重连Server", MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
                //    {
                //        return;
                //    }
                //    m_ReConnectIndex = 0;
                //    m_ReConnecting = true;
                //    ClientSocket.Instance.Host = INI.tcp_ip;
                //    ClientSocket.Instance.Port = INI.tcp_port;
                //    int iret = ClientSocket.Instance.ReConnectServer();
                //    if (iret != 0)
                //    {
                //        m_ReConnectIndex = 10;
                //        MessageBox.Show("重新连接服务器错误，请检查。", "重连Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    }
                //    m_ReConnecting = false;


                //    break;
            }

        }

        public void Initial(VersionEnum version, OptionEnum option, JzMainSDM1MachineClass machine)
        {
            VERSION = version;
            OPTION = option;
            MACHINE = machine;

            tpnlCover.Location = new Point(0, 0);
            tpnlCover.Size = new Size(336, 303);

            myJzTimer.Cut();

            AxisMotionControl[(int)MotionEnum.M0].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M0], VERSION, OPTION);

            //lblReConnectServer.BackColor = Color.Green;
            //lblReConnectServer.Text = "Server连接成功";

            //lblReConnectServer.BackColor = (ClientSocket.Instance.IsConnecting ? Color.Green : Color.Red);
            //lblReConnectServer.BackColor = (ClientSocket.Instance.IsConnecting ? Color.Green : Color.Red);

            //updateReConnectServerUI(ClientSocket.Instance.IsConnecting);
            //ClientSocket.Instance.TriggerStringAction += Instance_TriggerStringAction;

            CommonLogClass.Instance.SetRichTextBox(richTextBox1);

        }

        private void Instance_TriggerStringAction(string opstr)
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

        private void updateReConnectServerUI(bool bOK)
        {

            try
            {
                this.Invoke(new Action(() =>
                {
                    //lblReConnectServer.BackColor = (bOK ? Color.Green : Color.Red);
                    //lblReConnectServer.Text = (bOK ? "Server连接成功" : "Server连接失败");
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

        #endregion

        public void SetEnable(bool isenable)
        {
            tpnlCover.Visible = !isenable;

            this.BackColor = (isenable ? SystemColors.Control : Color.DarkGray);
            //tpnlCover.Visible = false;
            //groupBox2.Enabled = isenable;
            //axisMotionUI1.Enabled = isenable;
        }
        public void Tick()
        {
            if (myJzTimer.msDuriation < MSDuriation)
                return;

            //if (Universal.m_UseCommToDLHandle)
            //{
            //    if (ClientSocket.Instance.IsConnecting)
            //    {
            //        m_ReConnectTime.Cut();
            //    }
            //    else
            //    {
            //        if (m_ReConnectTime.msDuriation > 3 * 1000)
            //        {
            //            m_ReConnectTime.Cut();
            //            if (!m_ReConnecting)
            //            {
            //                m_ReConnecting = true;
            //                //m_ReConnectIndex++;

            //                lblReConnectServer.BackColor = Color.Red;
            //                lblReConnectServer.Text = "Server重连中";

            //                System.Threading.Thread thread_DL_ReConnectServer = new System.Threading.Thread(_reConnectServer);
            //                thread_DL_ReConnectServer.Start();
            //            }
            //        }
            //    }

            //    //if (!ClientSocket.Instance.IsConnecting && !m_ReConnecting && m_ReConnectIndex < m_ReConnectCount)
            //    //{
            //    //    m_ReConnecting = true;
            //    //    m_ReConnectIndex++;

            //    //    System.Threading.Thread thread_DL_ReConnectServer = new System.Threading.Thread(_reConnectServer);
            //    //    thread_DL_ReConnectServer.Start();
            //    //}
            //}


            MACHINE.Tick();

            Color bkColor = Color.Black;// Control.DefaultBackColor;

            lblIsReset.BackColor = (PLCIO.IsRESET ? Color.Green : bkColor);
            lblIsStart.BackColor = (PLCIO.IsStart ? Color.Green : bkColor);
            lblIsEMC.BackColor = (PLCIO.IsEMC ? Color.Green : bkColor);
            lblTopLight.BackColor = (PLCIO.TopLight ? Color.Green : bkColor);
            //lblFrontLight.BackColor = (PLCIO.FrontLight ? Color.Green : Color.Black);
            //lblBackLight.BackColor = (PLCIO.BackLight ? Color.Green : Color.Black);
            lblReady.BackColor = (PLCIO.Ready ? Color.Green : bkColor);
            lblBusy.BackColor = (PLCIO.Busy ? Color.Green : bkColor);
            lblPass.BackColor = (PLCIO.Pass ? Color.Red : bkColor);
            lblFail.BackColor = (PLCIO.Fail ? Color.Green : bkColor);
            lblGetImageOK.BackColor = (PLCIO.GetImageOK ? Color.Green : bkColor);
            lblGetImageIndex.BackColor = bkColor;
            lblGetImageIndex.Text = JetEazy.PlugSpace.CamActClass.Instance.StepCurrent.ToString() + " 总[" + JetEazy.PlugSpace.CamActClass.Instance.StepCount.ToString() + "]";

            AxisMotionControl[(int)MotionEnum.M0].Tick();

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
            //MACHINE.PLCIO.FrontLight = false;
            //MACHINE.PLCIO.BackLight = false;
        }

        //private void _reConnectServer()
        //{
        //    ClientSocket.Instance.Host = INI.tcp_ip;
        //    ClientSocket.Instance.Port = INI.tcp_port;
        //    int iret = ClientSocket.Instance.ReConnectServer();
        //    m_ReConnecting = false;
        //}

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
