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
using Allinone.FormSpace.Motor;
using System.Diagnostics;
using EzSegClientLib;
using System.IO;

namespace Allinone.UISpace.CTRLUISpace
{
    public partial class AllinoneSDM2CtrlUI : UserControl
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

            TOUCH_MOTOR,
            WRITE_JJS,
            ABSMOVE,

            lblAIModelChange,
            ROBOT_CTRL,

            MOTIONENABLE,
        }


        const int MSDuriation = 10;

        AxisMotionUI[] AxisMotionControl = new AxisMotionUI[4];

        JzTransparentPanel tpnlCover;
        VersionEnum VERSION = VersionEnum.ALLINONE;
        OptionEnum OPTION = OptionEnum.MAIN;
        JzMainSDM2MachineClass MACHINE;

        JzMainSDM2IOClass PLCIO
        {
            get
            {
                return MACHINE.PLCIO;
            }
        }

        public IEzSeg model
        {
            get { return Universal.model; }
        }

        Label lblrobotstate;
        Label lblrobotmotionenable;
        Label lblroboterror;

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
        Label lblAIMsg;

        //Label lblReConnectServer;

        Button btnReset;
        Button btnOnekeyGetImage;
        Button btnReady;
        Button btnTouchMotor;
        Button btnWriteJJS;
        Button btnAbsMove;
        Button btnRobot;

        JzTimes myJzTimer = new JzTimes();

        public AllinoneSDM2CtrlUI()
        {
            InitializeComponent();
            InitialInternal();
        }

        void InitialInternal()
        {
            tpnlCover = jzTransparentPanel1;
            AxisMotionControl[0] = axisMotionUI1;
            AxisMotionControl[1] = axisMotionUI2;
            AxisMotionControl[2] = axisMotionUI3;
            AxisMotionControl[3] = axisMotionUI4;

            lblrobotstate = label4;
            lblrobotmotionenable = label5;
            lblroboterror = label6;

            lblAIMsg = label3;
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
            btnReady = button2;
            btnTouchMotor = button4;
            btnWriteJJS = button5;
            btnAbsMove = button6;
            btnRobot = button7;

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
            btnTouchMotor.Tag = TagEnum.TOUCH_MOTOR;
            btnWriteJJS.Tag = TagEnum.WRITE_JJS;
            btnAbsMove.Tag = TagEnum.ABSMOVE;
            btnRobot.Tag = TagEnum.ROBOT_CTRL;
            lblrobotmotionenable.Tag = TagEnum.MOTIONENABLE;

            lblAIMsg.Tag = TagEnum.lblAIModelChange;

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
            lblAIMsg.DoubleClick += lbl_DoubleClick;
            lblrobotmotionenable.DoubleClick += lbl_DoubleClick;

            btnReset.Click += Btn_Click;
            btnOnekeyGetImage.Click += Btn_Click;
            btnTouchMotor.Click += Btn_Click;
            btnWriteJJS.Click += Btn_Click;
            btnAbsMove.Click += Btn_Click;
            btnRobot.Click += Btn_Click;

            btnReady.Visible = false;
            btnOnekeyGetImage.Visible = false;
            btnTouchMotor.Visible = false;
            btnReset.Visible = false;
            button8.Visible = false;
            lblAIMsg.Visible = false;

            switch (Universal.OPTION)
            {
                case OptionEnum.MAIN_SDM1:
                case OptionEnum.MAIN_SDM2:
                    btnWriteJJS.Visible = false;
                    if (INI.chipUseAI)
                    {
                        lblAIMsg.Visible = true;
                    }

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

            switch (Universal.OPTION)
            {
                case OptionEnum.MAIN_SDM2:
                    switch (Universal.CAMACT)
                    {
                        case CameraActionMode.CAM_MOTOR_MODE2:

                            btnTouchMotor.Visible = true;

                            break;
                    }

                    break;

            }

        }

        

        MFApiDemo.RobotHCFAForm mRobotHCFAForm = null;
        frmTouchMotor mTouchMotor = null;
        private void Btn_Click(object sender, EventArgs e)
        {
            TagEnum KEYS = (TagEnum)((Button)sender).Tag;
            CommonLogClass.Instance.LogMessage("SDM1_CTRL[BTN]:" + KEYS.ToString());
            switch (KEYS)
            {
                case TagEnum.ABSMOVE:

                    Task task = new Task(() =>
                    {
                        PLCIO.RobotAbs = true;
                        System.Threading.Thread.Sleep(500);
                        PLCIO.RobotAbs = false;
                    });
                    task.Start();

                    break;
                case TagEnum.WRITE_JJS:
                    switch (MACHINE.mRobotType)
                    {
                        case RobotType.HCFA:

                            PLCIO.RobotSpeedValue = INI.RobotSpeedValue;


                            break;
                        case RobotType.NONE:

                            PLCIO.SetAxisJJS("0:MW0110", INI.AXIS_MANUAL_JJS_ADD);
                            PLCIO.SetAxisJJS("0:MW0111", INI.AXIS_MANUAL_JJS_SUB);
                            PLCIO.SetAxisJJS("0:MW0113", INI.AXIS_AUTO_JJS_ADD);
                            PLCIO.SetAxisJJS("0:MW0114", INI.AXIS_AUTO_JJS_SUB);

                            break;
                    }
                   
                    
                    //PLCIO.ADR_AXIS_X_JJS = INI.AXIS_X_JJS;
                    //PLCIO.ADR_AXIS_Y_JJS = INI.AXIS_Y_JJS;
                    //PLCIO.ADR_AXIS_Z_JJS = INI.AXIS_Z_JJS;

                    //Task task = new Task(() =>
                    //{
                    //    PLCIO.ADR_WRITE_JJS = true;
                    //    System.Threading.Thread.Sleep(500);
                    //    PLCIO.ADR_WRITE_JJS = false;
                    //});
                    //task.Start();

                    break;
                case TagEnum.RESET:
                    OnTrigger(ActionEnum.ACT_RESET, "");
                    break;
                case TagEnum.ONEKEYGETIMAGE:
                    OnTrigger(ActionEnum.ACT_ONEKEYGETIMAGE, "");
                    break;
                case TagEnum.TOUCH_MOTOR:


                    switch (VERSION)
                    {
                        case VersionEnum.ALLINONE:
                            switch (OPTION)
                            {
                                case OptionEnum.MAIN_SDM2:

                                    if (!Universal.IsOpenMotorWindows)
                                    {
                                        Universal.IsOpenMotorWindows = true;
                                        mTouchMotor = new frmTouchMotor();
                                        mTouchMotor.Show();
                                    }

                                    //switch (MACHINE.mRobotType)
                                    //{
                                    //    case RobotType.HCFA:
                                    //        if (!JetEazy.Universal.IsRobotFormOpen)
                                    //        {
                                    //            JetEazy.Universal.IsRobotFormOpen = true;
                                    //            mRobotHCFAForm =
                                    //            new MFApiDemo.RobotHCFAForm(MACHINE.mRobotHCFA.ApiHandle);
                                    //            mRobotHCFAForm.Show();
                                    //        }


                                    //        break;
                                    //    case RobotType.NONE:
                                    //        if (!Universal.IsOpenMotorWindows)
                                    //        {
                                    //            Universal.IsOpenMotorWindows = true;
                                    //            mTouchMotor = new frmTouchMotor();
                                    //            mTouchMotor.Show();
                                    //        }

                                    //        break;
                                    //}

                                    break;
                            }
                            break;
                    }

                    break;

                case TagEnum.ROBOT_CTRL:

                    switch (VERSION)
                    {
                        case VersionEnum.ALLINONE:
                            switch (OPTION)
                            {
                                case OptionEnum.MAIN_SDM2:
                                    if (!JetEazy.Universal.IsRobotFormOpen)
                                    {
                                        JetEazy.Universal.IsRobotFormOpen = true;
                                        mRobotHCFAForm =
                                        new MFApiDemo.RobotHCFAForm(MACHINE.mRobotHCFA.ApiHandle,
                                        MACHINE.mRobotHCFA.UserId);
                                        mRobotHCFAForm.Show();
                                    }

                                    break;
                            }
                            break;
                    }

                    break;
            }
        }

        private void lbl_DoubleClick(object sender, EventArgs e)
        {
            TagEnum KEYS = (TagEnum)((Label)sender).Tag;
            CommonLogClass.Instance.LogMessage("SDM1_CTRL[LBL]:" + KEYS.ToString());
            switch (KEYS)
            {
                case TagEnum.lblAIModelChange:

                    if (INI.chipUseAI)
                    {
                        if (model != null)
                        {
                            INI.AI_Model_FilenamePath = JzToolsClass.OpenFilePicker("PT Files (*.pt)|*.PT|" + "All files (*.*)|*.*", INI.AI_Model_FilenamePath);
                            //加载模型
                            if (File.Exists(INI.AI_Model_FilenamePath))
                            {
                                EzSegError ezSegError = model.LoadModelFile(INI.AI_Model_FilenamePath);
                                if (!ezSegError.Is(Errcode.OK))
                                {
                                    MessageBox.Show($"模型加载失败，错误信息={ezSegError.errMsg}", "提示");
                                }
                                else
                                {
                                    INI.SaveAi();
                                    MessageBox.Show($"模型加载成功", "提示");
                                }
                            }
                            _updateAiMessage();
                        }
                    }

                    break;
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
                case TagEnum.MOTIONENABLE:
                    PLCIO.RobotEnable = !PLCIO.RobotEnable;
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

        public void Initial(VersionEnum version, OptionEnum option, JzMainSDM2MachineClass machine)
        {
            VERSION = version;
            OPTION = option;
            MACHINE = machine;

            tpnlCover.Location = new Point(0, 0);
            tpnlCover.Size = new Size(336, 303);

            myJzTimer.Cut();

            switch (MACHINE.mRobotType)
            {
                case RobotType.HCFA:

                    //btnWriteJJS.Visible = false;
                    btnAbsMove.Visible = false;

                    tabControl1.Controls.RemoveAt(2);
                    tabControl1.Controls.RemoveAt(2);
                    tabControl1.Controls.RemoveAt(2);
                    tabControl1.Controls.RemoveAt(2);

                    break;
            }

            AxisMotionControl[(int)MotionEnum.M0].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M0], VERSION, OPTION);
            AxisMotionControl[(int)MotionEnum.M1].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M1], VERSION, OPTION);
            AxisMotionControl[(int)MotionEnum.M2].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M2], VERSION, OPTION);
            AxisMotionControl[(int)MotionEnum.M3].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M3], VERSION, OPTION);

            //lblReConnectServer.BackColor = Color.Green;
            //lblReConnectServer.Text = "Server连接成功";

            //lblReConnectServer.BackColor = (ClientSocket.Instance.IsConnecting ? Color.Green : Color.Red);
            //lblReConnectServer.BackColor = (ClientSocket.Instance.IsConnecting ? Color.Green : Color.Red);

            //updateReConnectServerUI(ClientSocket.Instance.IsConnecting);
            //ClientSocket.Instance.TriggerStringAction += Instance_TriggerStringAction;

            CommonLogClass.Instance.SetRichTextBox(richTextBox1);
            _updateAiMessage();

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
        private void _updateAiMessage()
        {
            switch (Universal.OPTION)
            {
                case OptionEnum.MAIN_SDM2:
                    if (INI.chipUseAI)
                    {
                        if (model != null)
                        {
                            lblAIMsg.Text = $"Ai版本号:{model.GetVersion()} \nAi模型名称:{model.GetCurrentModelName()} ";
                        }
                    }
                    break;
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
            btnAbsMove.BackColor = (PLCIO.RobotAbs ? Color.Green : Color.FromArgb(192, 255, 192));
            lblReady.BackColor = (PLCIO.Ready ? Color.Green : bkColor);
            lblBusy.BackColor = (PLCIO.Busy ? Color.Green : bkColor);
            lblPass.BackColor = (PLCIO.Pass ? Color.Green : bkColor);
            lblFail.BackColor = (PLCIO.Fail ? Color.Red : bkColor);
            lblGetImageOK.BackColor = (PLCIO.GetImageOK ? Color.Green : bkColor);
            lblGetImageIndex.BackColor = bkColor;
            lblGetImageIndex.Text = JetEazy.PlugSpace.CamActClass.Instance.StepCurrent.ToString() + " 总[" + JetEazy.PlugSpace.CamActClass.Instance.StepCount.ToString() + "]";
            lblrobotmotionenable.BackColor = (PLCIO.RobotEnable ? Color.Green : bkColor);
            lblrobotstate.BackColor = (PLCIO.RobotRunning ? Color.Green : bkColor);
            lblroboterror.BackColor = (false ? Color.Green : bkColor);
            lblroboterror.Text = PLCIO.RobotState;
            numericUpDown1.Value = (decimal)PLCIO.RobotSpeedValue;

            AxisMotionControl[(int)MotionEnum.M0].Tick();
            AxisMotionControl[(int)MotionEnum.M1].Tick();
            AxisMotionControl[(int)MotionEnum.M2].Tick();
            AxisMotionControl[(int)MotionEnum.M3].Tick();

            myJzTimer.Cut();
        }

        /// <summary>
        /// 释放资源并关闭线程
        /// </summary>
        public void SDDispose()
        {
            MACHINE.PLCIO.RobotAbs = false;
            MACHINE.PLCIO.Ready = false;
            MACHINE.PLCIO.Busy = false;
            PLCIO.GetImageOK = false;

            MACHINE.PLCIO.Pass = false;
            MACHINE.PLCIO.Fail = false;

            MACHINE.PLCIO.TopLight = false;
            //MACHINE.PLCIO.FrontLight = false;
            //MACHINE.PLCIO.BackLight = false;
            MACHINE.Dispose();
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
