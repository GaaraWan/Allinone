using AHBlobPro;
using Allinone.ControlSpace.IOSpace;
using Allinone.ControlSpace.MachineSpace;
using JetEazy;
using JetEazy.BasicSpace;
using JetEazy.ControlSpace;
using JetEazy.ControlSpace.MotionSpace;
using JetEazy.DBSpace;
using JetEazy.FormSpace;
using JetEazy.UISpace;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Allinone.UISpace.CTRLUISpace
{
    enum TagSD
    {
        START,
        PAUSE,
        STOP,

        MODE_ATUO_MANUAL,

        ALARM_CLEAN,
        MUTE,
        RESET,
        /// <summary>
        /// 局部复位
        /// </summary>
        RESET_PARTIAL,

        EMC,
        LAMP_RED,
        LAMP_GREEN,
        LAMP_YELLOW,

        BUZZER,
        DOOR,
        LIGHT,

        VACC_FEED,
        VACC_TAKE,

        MOTOR_SETUP,

        RESET_BUFF,
        CHECK_BUFF,

        BLOW,

        TAKEPASSLOAD,
        TAKEPASSUNLOAD,
        TAKEPASSCLEAN,
        TAKENGLOAD,
        TAKENGUNLOAD,
        TAKENGCLEAN,
        
        LOAD_ONE,
        UNLOAD_ONE,

        QUERY_ALARM_LOG,

    }

    public partial class AllinoneSDCtrlUI : UserControl
    {
        Button btnStart;
        Button btnPause;
        Button btnStop;
        Button btnModeAutoManual;
        //Button btnAlarmClean;

        Label lblEMC;
        Label lblLampRed;
        Label lblLampGreen;
        Label lblLampYellow;
        Label lblBuzzer;
        Label lblDoor;
        Label lblVaccFeed;
        Label lblVaccTake;
        Label lblMotorSetup;
        Label lblLight;

        Button btnMute;
        Button btnRemoveAlarm;
        Button btnReset;
        Button btnResetPartial;

        Label lblAlarm;
        ListBox lsbEvent;
        Label lblState;
        Label lblBUFF;
        Label lblCHECKBUFF;
        Label lblBLOW;

        Label lblLoadOne;
        Label lblUnLoadOne;

        #region 操作

        Button btnTAKEPASSLOAD;
        Button btnTAKEPASSUNLOAD;
        Button btnTAKEPASSCLEAN;
        Button btnTAKENGLOAD;
        Button btnTAKENGUNLOAD;
        Button btnTAKENGCLEAN;

        Button btnLoadOne;
        Button btnUnloadOne;
        Button btnMotorSetup;
        Button btnCheckBuff;

        #endregion

        MainSDOpUI[] regionUI = new MainSDOpUI[(int)RegionEnum.COUNT];


        //JzAllinoneMachineClass MACHINE;
        MessageForm M_WARNING_FRM;

        JzTransparentPanel tpnlCover;

        VersionEnum VERSION = VersionEnum.ALLINONE;
        OptionEnum OPTION = OptionEnum.MAIN_SD;
        JzMainSDMachineClass MACHINE;

        AccDBClass ACCDB
        {
            get
            {
                return Universal.ACCDB;
            }
        }

        JzMainSDPositionParaClass JzMainSDPositionParas
        {
            get { return Universal.JZMAINSDPOSITIONPARA; }
        }

        PLCMotionClass MOTOR_TEST_X
        {
            get { return MACHINE.PLCMOTIONCollection[(int)MotionEnum.M3]; }
        }

        //AxisMotionUI[] AxisMotionControl = new AxisMotionUI[10];

        public AllinoneSDCtrlUI()
        {
            InitializeComponent();
            InitialInternal();
        }

        bool m_MainEnable = false;
        System.Threading.Thread m_threadForPrcess = null;
        bool m_CheckBuffProcessIsOnOff = true;

        void InitialInternal()
        {
            tpnlCover = jzTransparentPanel1;

            //AxisMotionControl[0] = axisMotionUI1;
            //AxisMotionControl[1] = axisMotionUI2;
            //AxisMotionControl[2] = axisMotionUI3;
            //AxisMotionControl[3] = axisMotionUI4;
            //AxisMotionControl[4] = axisMotionUI5;
            //AxisMotionControl[5] = axisMotionUI6;
            //AxisMotionControl[6] = axisMotionUI7;
            //AxisMotionControl[7] = axisMotionUI8;
            //AxisMotionControl[8] = axisMotionUI9;
            //AxisMotionControl[9] = axisMotionUI10;

            btnMute = button7;
            btnRemoveAlarm = button8;
            btnReset = button1;

            lblAlarm = label5;
            lsbEvent = listBox1;

            btnStart = button2;
            btnPause = button3;
            btnStop = button4;
            btnModeAutoManual = button5;
            //btnAlarmClean = button2;

            lblEMC = label12;
            lblLampRed = label8;
            lblLampGreen = label9;
            lblLampYellow = label10;
            lblBuzzer = label1;
            lblDoor = label2;
            lblVaccFeed = label3;
            lblVaccTake = label4;

            lblMotorSetup = label7;
            lblLight = label13;
            lblBUFF = label14;
            lblCHECKBUFF = label15;
            lblBLOW = label16;

            lblLoadOne = label17; ;
            lblUnLoadOne = label18;


            lblState = label11;

            btnTAKEPASSLOAD = button6;
            btnTAKEPASSUNLOAD = button9;
            btnTAKEPASSCLEAN = button10;
            btnTAKENGLOAD = button13;
            btnTAKENGUNLOAD = button12;
            btnTAKENGCLEAN = button11;

            btnResetPartial = button14;

            btnLoadOne = button15;
            btnUnloadOne = button16;
            btnMotorSetup = button17;
            btnCheckBuff = button18;

            regionUI[(int)RegionEnum.TAKE_PASS] = mainSDOpUI1;
            regionUI[(int)RegionEnum.TAKE_NG] = mainSDOpUI2;
            regionUI[(int)RegionEnum.FEED_Z1] = mainSDOpUI3;
            regionUI[(int)RegionEnum.FEED_Z2] = mainSDOpUI4;

            //IOControl = allinoneIOUI1;
            //STATEControl = allinoneSTATEUI1;
        }

        public void Initial(VersionEnum version, OptionEnum option, JzMainSDMachineClass machine)
        {
            VERSION = version;
            OPTION = option;
            MACHINE = machine;

            tpnlCover.Location = new Point(10, 60);
            tpnlCover.Size = new Size(316, 233);

            switch (OPTION)
            {
                case OptionEnum.MAIN_SD:

                    int i = 0;
                    while (i < (int)RegionEnum.COUNT)
                    {
                        regionUI[i].SetRegion((RegionEnum)i);
                        regionUI[i].Init(version, option, machine);
                        i++;
                    }

                    //AxisMotionControl[(int)MotionEnum.M0].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M0], VERSION, OPTION);
                    //AxisMotionControl[(int)MotionEnum.M1].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M1], VERSION, OPTION);
                    //AxisMotionControl[(int)MotionEnum.M2].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M2], VERSION, OPTION);

                    //AxisMotionControl[(int)MotionEnum.M3].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M3], VERSION, OPTION);
                    //AxisMotionControl[(int)MotionEnum.M4].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M4], VERSION, OPTION);
                    //AxisMotionControl[(int)MotionEnum.M5].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M5], VERSION, OPTION);

                    //AxisMotionControl[(int)MotionEnum.M6].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M6], VERSION, OPTION);
                    //AxisMotionControl[(int)MotionEnum.M7].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M7], VERSION, OPTION);
                    //AxisMotionControl[(int)MotionEnum.M8].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M8], VERSION, OPTION);

                    //AxisMotionControl[(int)MotionEnum.M9].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M8], VERSION, OPTION);

                    //IOControl.Initial(MACHINE);
                    //STATEControl.Initial(MACHINE);

                    tabControl1.SelectedIndexChanged += TabControl1_SelectedIndexChanged;

                    //btnMute.Click += BtnMute_Click;
                    //btnRemoveAlarm.Click += BtnRemoveAlarm_Click;
                    //btnReset.Click += BtnReset_Click;


                    btnStart.Tag = TagSD.START;
                    btnPause.Tag = TagSD.PAUSE;
                    btnStop.Tag = TagSD.STOP;
                    btnModeAutoManual.Tag = TagSD.MODE_ATUO_MANUAL;
                    btnMute.Tag = TagSD.MUTE;
                    btnRemoveAlarm.Tag = TagSD.ALARM_CLEAN;
                    btnReset.Tag = TagSD.RESET;

                    btnTAKEPASSLOAD.Tag = TagSD.TAKEPASSLOAD;
                    btnTAKEPASSUNLOAD.Tag = TagSD.TAKEPASSUNLOAD;
                    btnTAKEPASSCLEAN.Tag = TagSD.TAKEPASSCLEAN;
                    btnTAKENGLOAD.Tag = TagSD.TAKENGLOAD;
                    btnTAKENGUNLOAD.Tag = TagSD.TAKENGUNLOAD;
                    btnTAKENGCLEAN.Tag = TagSD.TAKENGCLEAN;
                    btnResetPartial.Tag = TagSD.RESET_PARTIAL;

                    btnLoadOne.Tag = TagSD.LOAD_ONE;
                    btnUnloadOne.Tag = TagSD.UNLOAD_ONE;
                    btnMotorSetup.Tag = TagSD.MOTOR_SETUP;
                    btnCheckBuff.Tag = TagSD.CHECK_BUFF;

                    btnQueryAlarmMsg.Tag = TagSD.QUERY_ALARM_LOG;

                    btnStart.Click += Btn_Click;
                    btnPause.Click += Btn_Click;
                    btnStop.Click += Btn_Click;
                    btnModeAutoManual.Click += Btn_Click;
                    btnMute.Click += Btn_Click;
                    btnRemoveAlarm.Click += Btn_Click;
                    btnReset.Click += Btn_Click;

                    btnTAKEPASSLOAD.Click += Btn_Click;
                    btnTAKEPASSUNLOAD.Click += Btn_Click;
                    btnTAKEPASSCLEAN.Click += Btn_Click;
                    btnTAKENGLOAD.Click += Btn_Click;
                    btnTAKENGUNLOAD.Click += Btn_Click;
                    btnTAKENGCLEAN.Click += Btn_Click;
                    btnResetPartial.Click += Btn_Click;


                    btnLoadOne.Click+= Btn_Click;
                    btnUnloadOne.Click += Btn_Click;
                    btnMotorSetup.Click += Btn_Click;
                    btnCheckBuff.Click += Btn_Click;
                    btnQueryAlarmMsg.Click += Btn_Click;


                    lblEMC.Tag = TagSD.EMC;
                    lblLampRed.Tag = TagSD.LAMP_RED;
                    lblLampGreen.Tag = TagSD.LAMP_GREEN;
                    lblLampYellow.Tag = TagSD.LAMP_YELLOW;
                    lblBuzzer.Tag = TagSD.BUZZER;
                    lblDoor.Tag = TagSD.DOOR;
                    lblVaccFeed.Tag = TagSD.VACC_FEED;
                    lblVaccTake.Tag = TagSD.VACC_TAKE;
                    lblMotorSetup.Tag = TagSD.MOTOR_SETUP;
                    lblLight.Tag = TagSD.LIGHT;
                    lblBUFF.Tag = TagSD.RESET_BUFF;
                    lblCHECKBUFF.Tag = TagSD.CHECK_BUFF;
                    lblBLOW.Tag = TagSD.BLOW;
                    lblLoadOne.Tag = TagSD.LOAD_ONE;
                    lblUnLoadOne.Tag = TagSD.UNLOAD_ONE;

                    lblEMC.DoubleClick += Lbl_DoubleClick;
                    lblLampRed.DoubleClick += Lbl_DoubleClick;
                    lblLampGreen.DoubleClick += Lbl_DoubleClick;
                    lblLampYellow.DoubleClick += Lbl_DoubleClick;
                    lblBuzzer.DoubleClick += Lbl_DoubleClick;
                    lblDoor.DoubleClick += Lbl_DoubleClick;
                    lblVaccFeed.DoubleClick += Lbl_DoubleClick;
                    lblVaccTake.DoubleClick += Lbl_DoubleClick;
                    lblMotorSetup.DoubleClick += Lbl_DoubleClick;
                    lblLight.DoubleClick += Lbl_DoubleClick;
                    lblBUFF.DoubleClick += Lbl_DoubleClick;
                    lblCHECKBUFF.DoubleClick+= Lbl_DoubleClick;
                    lblBLOW.DoubleClick += Lbl_DoubleClick;

                    lblLoadOne.DoubleClick+= Lbl_DoubleClick;
                    lblUnLoadOne.DoubleClick+= Lbl_DoubleClick;

                    CommonLogClass.Instance.SetRichTextBox(richTextBox1);

                    StopAllProcess("INIT");

                    ThreadStart();

                    break;
            }

            MACHINE.EVENT.Initial(lsbEvent);
            MACHINE.EVENT.Initial(lblAlarm);

            MACHINE.TriggerAction += MACHINE_TriggerAction;
            MACHINE.EVENT.TriggerAlarm += EVENT_TriggerAlarm;
        }

        private void EVENT_TriggerAlarm(bool IsBuzzer)
        {
            MACHINE.PLCIO.ADR_BUZZER = IsBuzzer;
            if (!IsBuzzer)
            {
                SetNormalLight();
            }
        }

        bool IsEMCTriggered = false;

        private void MACHINE_TriggerAction(MachineEventEnum machineevent, object obj = null)
        {
            switch (machineevent)
            {
                case MachineEventEnum.ALARM_SERIOUS:
                    IsAlarmsSeriousX = true;
                    //SetAbnormalLight();
                    break;
                case MachineEventEnum.ALARM_COMMON:
                    IsAlarmsCommonX = true;
                    //SetAbnormalLight();
                    break;
                case MachineEventEnum.EMC:
                    IsEMCTriggered = true;
                    break;
            }
        }

        Allinone.FormSpace.frmMainSDMotorSetup mMotorFrom;

        JetEazy.ControlSpace.CCDCollectionClass CCDCollection
        {
            get
            {
                return Universal.CCDCollection;
            }

        }

       bool IsNoUsePLC
        {
            get { return Universal.IsNoUseIO; }
        }


        private void Lbl_DoubleClick(object sender, EventArgs e)
        {
            Label lbl = (Label)sender;
            TagSD _tag = (TagSD)lbl.Tag;

            log("双击标签 " + _tag.ToString());

            switch (_tag)
            {
                case TagSD.EMC:
                    break;
                case TagSD.LAMP_RED:
                    MACHINE.PLCIO.Red = !MACHINE.PLCIO.Red;
                    break;
                case TagSD.LAMP_GREEN:
                    MACHINE.PLCIO.Green = !MACHINE.PLCIO.Green;
                    break;
                case TagSD.LAMP_YELLOW:
                    MACHINE.PLCIO.Yellow = !MACHINE.PLCIO.Yellow;
                    break;
                case TagSD.BUZZER:
                    MACHINE.PLCIO.ADR_BUZZER = !MACHINE.PLCIO.ADR_BUZZER;
                    break;
                case TagSD.DOOR:
                    MACHINE.PLCIO.ADR_DOOR = !MACHINE.PLCIO.ADR_DOOR;
                    break;
                case TagSD.VACC_FEED:
                    MACHINE.PLCIO.ADR_FEEDVACC = !MACHINE.PLCIO.ADR_FEEDVACC;
                    break;
                case TagSD.VACC_TAKE:
                    MACHINE.PLCIO.ADR_TAKEVACC = !MACHINE.PLCIO.ADR_TAKEVACC;
                    break;
                case TagSD.MOTOR_SETUP:

                    if (!Universal.IsOpenMotorWindows)
                    {
                        OnTrigger(ActionEnum.ACT_MOTOR_SETUP, "");

                        Universal.IsOpenMotorWindows = true;
                        MACHINE.PLCReadCmdNormalTemp(true);
                        System.Threading.Thread.Sleep(500);
                        mMotorFrom = new FormSpace.frmMainSDMotorSetup();
                        mMotorFrom.Show();
                    }

                    break;
                case TagSD.LIGHT:
                    MACHINE.PLCIO.ADR_LIGHT = !MACHINE.PLCIO.ADR_LIGHT;
                    break;
                case TagSD.RESET_BUFF:

                    myTestRegionBuff = !myTestRegionBuff;
                   
                    //int iret = CheckBuffProduct();
                    //MessageBox.Show((iret == 0 ? "无产品" : "有产品"));
                    //myTestRegionBuff = iret != 0;

                    break;
                case TagSD.CHECK_BUFF:

                    M_WARNING_FRM = new MessageForm(true, "!!!警告!!!\t\n请确认测试X轴是否可运动，点击确认进入检查BUFF区有无料流程。");
                    if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                    {
                        if (!checkbuffprocess.IsOn)
                            checkbuffprocess.Start("");
                    }
                    M_WARNING_FRM.Close();
                    M_WARNING_FRM.Dispose();

                    break;
                case TagSD.BLOW:

                    MACHINE.PLCIO.ADR_BLOW = !MACHINE.PLCIO.ADR_BLOW;

                    break;
                case TagSD.LOAD_ONE:

                    M_WARNING_FRM = new MessageForm(true, "请在供料区放入产品，是否载入？");
                    if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                    {
                        if (!loadoneprocess.IsOn)
                            loadoneprocess.Start("");
                    }
                    M_WARNING_FRM.Close();
                    M_WARNING_FRM.Dispose();

                    break;
                case TagSD.UNLOAD_ONE:

                    M_WARNING_FRM = new MessageForm(true, "是否载出？");
                    if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                    {
                        if (!Unloadoneprocess.IsOn)
                            Unloadoneprocess.Start("");
                    }
                    M_WARNING_FRM.Close();
                    M_WARNING_FRM.Dispose();

                    break;
            }
        }


        Allinone.FormSpace.AlarmQueryForm AlarmQueryForm = null;

        private void Btn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            TagSD _tag = (TagSD)btn.Tag;

            log("单击按钮 " + _tag.ToString());

            switch(_tag)
            {
                case TagSD.QUERY_ALARM_LOG:


                    if (!Universal.IsOpenAlarmWindows)
                    {
                        AlarmQueryForm = new FormSpace.AlarmQueryForm();
                        AlarmQueryForm.Show();
                    }


                    break;
                case TagSD.LOAD_ONE:

                    M_WARNING_FRM = new MessageForm(true, "请在供料区放入产品，是否载入？");
                    if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                    {
                        if (!loadoneprocess.IsOn)
                            loadoneprocess.Start("");
                    }
                    M_WARNING_FRM.Close();
                    M_WARNING_FRM.Dispose();

                    break;
                case TagSD.UNLOAD_ONE:

                    M_WARNING_FRM = new MessageForm(true, "是否载出？");
                    if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                    {
                        if (!Unloadoneprocess.IsOn)
                            Unloadoneprocess.Start("");
                    }
                    M_WARNING_FRM.Close();
                    M_WARNING_FRM.Dispose();

                    break;
                case TagSD.MOTOR_SETUP:

                    if (!Universal.IsOpenMotorWindows)
                    {
                        OnTrigger(ActionEnum.ACT_MOTOR_SETUP, "");

                        Universal.IsOpenMotorWindows = true;
                        MACHINE.PLCReadCmdNormalTemp(true);
                        System.Threading.Thread.Sleep(500);
                        mMotorFrom = new FormSpace.frmMainSDMotorSetup();
                        mMotorFrom.Show();
                    }

                    break;
                case TagSD.CHECK_BUFF:

                    M_WARNING_FRM = new MessageForm(true, "!!!警告!!!\t\n请确认测试X轴是否可运动，点击确认进入检查BUFF区有无料流程。");
                    if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                    {
                        if (!checkbuffprocess.IsOn)
                            checkbuffprocess.Start("");
                    }
                    M_WARNING_FRM.Close();
                    M_WARNING_FRM.Dispose();

                    break;

                case TagSD.START:

                    #region START

                    if (MACHINE.PLCIO.IsAlarmsCommon || MACHINE.PLCIO.IsAlarmsSerious)
                    {
                        M_WARNING_FRM = new MessageForm(true, "报警中，请复位后再进行启动。", "");
                        if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                        {
                        }
                        M_WARNING_FRM.Close();
                        M_WARNING_FRM.Dispose();

                        return;
                    }

                    if (MOTOR_REGION_PASS.PositionNow <= 0 || MOTOR_REGION_PASS.IsReachHomeBound)//硬件达到满盒 提示并停机
                    {
                        M_WARNING_FRM = new MessageForm(true, "PASS区满盒或不在收料位置，请检查。", "");
                        if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                        {
                        }
                        M_WARNING_FRM.Close();
                        M_WARNING_FRM.Dispose();

                        return;
                    }

                    if (MOTOR_REGION_NG.PositionNow <= 0 || MOTOR_REGION_NG.IsReachHomeBound)//硬件达到满盒 提示并停机
                    {
                        M_WARNING_FRM = new MessageForm(true, "NG区满盒或不在收料位置，请检查。", "");
                        if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                        {
                        }
                        M_WARNING_FRM.Close();
                        M_WARNING_FRM.Dispose();

                        return;
                    }

                    if (m_MainSDProcess.IsOn)
                    {
                        M_WARNING_FRM = new MessageForm(true, "正在流程中，请勿重复点击。","");
                        if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                        {
                        }
                        M_WARNING_FRM.Close();
                        M_WARNING_FRM.Dispose();

                        return;
                    }

                    if (resetprocess.IsOn)
                    {
                        M_WARNING_FRM = new MessageForm(true, "复位中，无法启动。", "");
                        if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                        {
                        }
                        M_WARNING_FRM.Close();
                        M_WARNING_FRM.Dispose();

                        return;
                    }

                    if (alarmresetprocess.IsOn)
                    {
                        M_WARNING_FRM = new MessageForm(true, "报警清除中，无法启动。", "");
                        if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                        {
                        }
                        M_WARNING_FRM.Close();
                        M_WARNING_FRM.Dispose();

                        return;
                    }

                    if (m_CheckBuffProcessIsOnOff)
                    {
                        if (checkbuffprocess.IsOn)
                        {
                            M_WARNING_FRM = new MessageForm(true, "执行检查BUFF区产品中，请稍后。", "");
                            if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                            {
                            }
                            M_WARNING_FRM.Close();
                            M_WARNING_FRM.Dispose();

                            return;
                        }
                    }

                    int iret = CheckPrepared();
                    if (iret != 0)
                        return;

                    M_WARNING_FRM = new MessageForm(true, "是否启动？");
                    //M_WARNING_FRM = new MessageForm(true, "!!!警告!!!\t\n请确认测试BUFF区料已拿出，拿出再启动。");
                    if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                    {

                        //myTestRegionBuff = false;
                        //m_MainSDProcess.Start();

                        if (m_CheckBuffProcessIsOnOff)
                        {
                            if (!checkbuffprocess.IsOn)
                                checkbuffprocess.Start("START_MAIN");
                        }
                        else
                        {
                            m_MainSDProcess.Start();
                        }
                    }
                    M_WARNING_FRM.Close();
                    M_WARNING_FRM.Dispose();

                    #endregion

                    break;
                case TagSD.PAUSE:
                    //M_WARNING_FRM = new MessageForm(true, "是否暂停?");
                    //if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                    //{
                    //    //MACHINE.PLCIO.ADR_START = true;
                    //}
                    //M_WARNING_FRM.Close();
                    //M_WARNING_FRM.Dispose();
                    break;
                case TagSD.STOP:
                    M_WARNING_FRM = new MessageForm(true, "是否停止?");
                    if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                    {
                        StopAllProcess("USERSTOP");
                    }
                    M_WARNING_FRM.Close();
                    M_WARNING_FRM.Dispose();
                    break;
                case TagSD.MODE_ATUO_MANUAL:
                    //M_WARNING_FRM = new MessageForm(true, "是否切换模式?");
                    //if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                    //{
                    //    //MACHINE.PLCIO.ADR_START = true;
                    //}
                    //M_WARNING_FRM.Close();
                    //M_WARNING_FRM.Dispose();
                    break;
                case TagSD.MUTE:
                    MACHINE.PLCIO.ADR_BUZZER = false;

                    //IsAlarmsCommonX = true;
                    break;
                case TagSD.ALARM_CLEAN:

                    //if (m_MainSDProcess.IsOn)
                    //{
                    //    M_WARNING_FRM = new MessageForm(true, "正在流程中，请勿重复点击。", "");
                    //    if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                    //    {
                    //    }
                    //    M_WARNING_FRM.Close();
                    //    M_WARNING_FRM.Dispose();

                    //    return;
                    //}

                    //if (resetprocess.IsOn)
                    //{
                    //    M_WARNING_FRM = new MessageForm(true, "复位中，无法启动。", "");
                    //    if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                    //    {
                    //    }
                    //    M_WARNING_FRM.Close();
                    //    M_WARNING_FRM.Dispose();

                    //    return;
                    //}

                    //if (alarmresetprocess.IsOn)
                    //{
                    //    M_WARNING_FRM = new MessageForm(true, "报警清除中，无法启动。", "");
                    //    if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                    //    {
                    //    }
                    //    M_WARNING_FRM.Close();
                    //    M_WARNING_FRM.Dispose();

                    //    return;
                    //}

                    //if (MACHINE.PLCIO.ADR_ISEMC)
                    //{
                    //    M_WARNING_FRM = new MessageForm(true, "急停按钮被按下，无法操作。", "");
                    //    if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                    //    {
                    //    }
                    //    M_WARNING_FRM.Close();
                    //    M_WARNING_FRM.Dispose();

                    //    return;
                    //}

                    //if (alarmresetprocess.IsOn)
                    //{
                    //    M_WARNING_FRM = new MessageForm(true, "已经在执行，请勿重复操作。", "");
                    //    if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                    //    {
                    //    }
                    //    M_WARNING_FRM.Close();
                    //    M_WARNING_FRM.Dispose();

                    //    return;
                    //}

                    M_WARNING_FRM = new MessageForm(true, "請檢查警報是否清除?");
                    if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                    {
                        MACHINE.ClearAlarm = true;
                        MACHINE.EVENT.RemoveAlarm();

                        //if (MACHINE.PLCIO.IsAlarmsSerious)
                        //{
                        //    alarmresetprocess.Start("SERIOUS");
                        //}
                        //else if (MACHINE.PLCIO.IsAlarmsCommon)
                        //    alarmresetprocess.Start("COMMON");
                        //else
                        //    alarmresetprocess.Start();

                    }
                    M_WARNING_FRM.Close();
                    M_WARNING_FRM.Dispose();
                    break;
                case TagSD.RESET:

                    if (m_MainSDProcess.IsOn)
                    {
                        M_WARNING_FRM = new MessageForm(true, "正在流程中，请勿重复点击。", "");
                        if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                        {
                        }
                        M_WARNING_FRM.Close();
                        M_WARNING_FRM.Dispose();

                        return;
                    }

                    //if (resetprocess.IsOn)
                    //{
                    //    M_WARNING_FRM = new MessageForm(true, "复位中，无法启动。", "");
                    //    if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                    //    {
                    //    }
                    //    M_WARNING_FRM.Close();
                    //    M_WARNING_FRM.Dispose();

                    //    return;
                    //}

                    if (alarmresetprocess.IsOn)
                    {
                        M_WARNING_FRM = new MessageForm(true, "报警清除中，无法启动。", "");
                        if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                        {
                        }
                        M_WARNING_FRM.Close();
                        M_WARNING_FRM.Dispose();

                        return;
                    }

                    iret = CheckPrepared();
                    if (iret != 0)
                        return;

                    M_WARNING_FRM = new MessageForm(true, "是否進行復位?");
                    if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                    {
                        if (!resetprocess.IsOn)
                            resetprocess.Start();
                        else
                            resetprocess.Stop();
                        //MACHINE.PLCIO.ADR_RESET = true;
                    }
                    M_WARNING_FRM.Close();
                    M_WARNING_FRM.Dispose();
                    break;

                case TagSD.TAKEPASSLOAD:
                    //if (!m_TakePASSloadprocess.IsOn)
                    //    m_TakePASSloadprocess.Start();
                    //else
                    //{
                    //    m_TakePASSloadprocess.Stop();
                    //    MOTOR_REGION_PASS.Stop();
                    //}
                    break;
                case TagSD.TAKEPASSUNLOAD:
                    //if (!m_TakePASSUnloadprocess.IsOn)
                    //    m_TakePASSUnloadprocess.Start();
                    //else
                    //{
                    //    m_TakePASSUnloadprocess.Stop();
                    //    MOTOR_REGION_PASS.Stop();
                    //}
                    break;
                case TagSD.TAKENGLOAD:
                    //if (!m_TakeNGloadprocess.IsOn)
                    //    m_TakeNGloadprocess.Start();
                    //else
                    //{
                    //    m_TakeNGloadprocess.Stop();
                    //    MOTOR_REGION_NG.Stop();
                    //}
                    break;
                case TagSD.TAKENGUNLOAD:
                    //if (!m_TakeNGUnloadprocess.IsOn)
                    //    m_TakeNGUnloadprocess.Start();
                    //else
                    //{
                    //    m_TakeNGUnloadprocess.Stop();
                    //    MOTOR_REGION_NG.Stop();
                    //}
                    break;
                case TagSD.TAKEPASSCLEAN:

                    //M_WARNING_FRM = new MessageForm(true, "是否要复位收料PASS区计数");
                    //if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                    //{
                    //    //JzMainSDPositionParas.INSPECT_PASSINDEX = 0;
                    //    JzMainSDPositionParas.PassZero();
                    //    MACHINE.IsUserFroceCount = false;
                    //}
                    //M_WARNING_FRM.Close();
                    //M_WARNING_FRM.Dispose();

                    break;
                case TagSD.TAKENGCLEAN:

                    //M_WARNING_FRM = new MessageForm(true, "是否要复位收料NG区计数");
                    //if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                    //{
                    //    //JzMainSDPositionParas.INSPECT_NGINDEX = 0;
                    //    JzMainSDPositionParas.NgZero();
                    //    MACHINE.IsUserFroceCount = false;
                    //}
                    //M_WARNING_FRM.Close();
                    //M_WARNING_FRM.Dispose();

                    break;
                case TagSD.RESET_PARTIAL:

                    #region RESET_PARTIAL

                    if (m_MainSDProcess.IsOn)
                    {
                        M_WARNING_FRM = new MessageForm(true, "正在流程中，请勿重复点击。", "");
                        if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                        {
                        }
                        M_WARNING_FRM.Close();
                        M_WARNING_FRM.Dispose();

                        return;
                    }

                    if (resetprocess.IsOn)
                    {
                        M_WARNING_FRM = new MessageForm(true, "复位中，无法启动。", "");
                        if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                        {
                        }
                        M_WARNING_FRM.Close();
                        M_WARNING_FRM.Dispose();

                        return;
                    }

                    if (alarmresetprocess.IsOn)
                    {
                        M_WARNING_FRM = new MessageForm(true, "报警清除中，无法启动。", "");
                        if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                        {
                        }
                        M_WARNING_FRM.Close();
                        M_WARNING_FRM.Dispose();

                        return;
                    }

                    if (MACHINE.PLCIO.ADR_ISEMC)
                    {
                        M_WARNING_FRM = new MessageForm(true, "急停按钮被按下，无法操作。", "");
                        if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                        {
                        }
                        M_WARNING_FRM.Close();
                        M_WARNING_FRM.Dispose();

                        return;
                    }

                    if (resetpartialprocess.IsOn)
                    {
                        M_WARNING_FRM = new MessageForm(true, "已经在执行，请勿重复操作。", "");
                        if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                        {
                        }
                        M_WARNING_FRM.Close();
                        M_WARNING_FRM.Dispose();

                        return;
                    }

                    if (MACHINE.PLCIO.IsAlarmsCommon)
                    {
                        M_WARNING_FRM = new MessageForm(true, "请先清除报警后进行小复位", "");
                        if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                        {
                        }
                        M_WARNING_FRM.Close();
                        M_WARNING_FRM.Dispose();

                        return;
                    }
                    if (MACHINE.PLCIO.IsAlarmsSerious)
                    {
                        M_WARNING_FRM = new MessageForm(true, "严重报警中，是否强制进行小复位。", "");
                        if (DialogResult.Yes != M_WARNING_FRM.ShowDialog())
                        {
                            return;
                        }
                        M_WARNING_FRM.Close();
                        M_WARNING_FRM.Dispose();
                    }

                    M_WARNING_FRM = new MessageForm(true, "是否进行小复位？");
                    if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                    {
                        resetpartialprocess.Start();
                    }
                    M_WARNING_FRM.Close();
                    M_WARNING_FRM.Dispose();

                    #endregion

                    break;

            }
        }

        ProcessClass loadoneprocess = new ProcessClass();
        private void LoadOneTick()
        {
            ProcessClass Process = loadoneprocess;

            if (Process.IsOn)
            {
                if (MACHINE.PLCIO.ADR_ISEMC)
                {
                    //if (!m_IsCheckResetOK)
                    {
                        //m_IsCheckResetOK = true;
                        if (M_WARNING_FRM != null)
                        {
                            M_WARNING_FRM.Close();
                            M_WARNING_FRM.Dispose();
                        }
                    }
                    Process.Stop();
                }

                switch (Process.ID)
                {
                    case 5:
                        m_IsCheckResetOK = false;

                        SetRunningLight();

                        M_WARNING_FRM = new MessageForm("载入中，請稍後...", true);
                        M_WARNING_FRM.Show();

                        //MACHINE.PLCIO.RESET = true;

                        MACHINE.PLCIO.LOAD_ONE_START= true;

                        Process.NextDuriation = 1000;
                        Process.ID = 10;

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.PLCIO.LOAD_ONE_END)
                            {
                                SetNormalLight();

                                if (M_WARNING_FRM != null)
                                {
                                    M_WARNING_FRM.Close();
                                    M_WARNING_FRM.Dispose();
                                }

                                Process.Stop();
                            }
                        }
                        break;
                }
            }
        }

        ProcessClass Unloadoneprocess = new ProcessClass();
        private void UnLoadOneTick()
        {
            ProcessClass Process = Unloadoneprocess;

            if (Process.IsOn)
            {
                if (MACHINE.PLCIO.ADR_ISEMC)
                {
                    //if (!m_IsCheckResetOK)
                    {
                        //m_IsCheckResetOK = true;
                        if (M_WARNING_FRM != null)
                        {
                            M_WARNING_FRM.Close();
                            M_WARNING_FRM.Dispose();
                        }
                    }
                    Process.Stop();
                }

                switch (Process.ID)
                {
                    case 5:
                        m_IsCheckResetOK = false;

                        SetRunningLight();

                        M_WARNING_FRM = new MessageForm("载出中，請稍後...", true);
                        M_WARNING_FRM.Show();

                        //MACHINE.PLCIO.RESET = true;

                        MACHINE.PLCIO.UNLOAD_ONE_START = true;

                        Process.NextDuriation = 1000;
                        Process.ID = 10;

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.PLCIO.UNLOAD_ONE_END)
                            {
                                SetNormalLight();

                                if (M_WARNING_FRM != null)
                                {
                                    M_WARNING_FRM.Close();
                                    M_WARNING_FRM.Dispose();
                                }

                                Process.Stop();
                            }
                        }
                        break;
                }
            }
        }


        //bool m_IsCheckBuffOK = false;
        ProcessClass checkbuffprocess = new ProcessClass();
        private void CheckBuffTick()
        {
            ProcessClass Process = checkbuffprocess;

            if (Process.IsOn)
            {
                if (MACHINE.PLCIO.ADR_ISEMC)
                {
                    //if (!m_IsCheckBuffOK)
                    //{
                    //    m_IsCheckBuffOK = true;
                    //    if (M_WARNING_FRM != null)
                    //    {
                    //        M_WARNING_FRM.Close();
                    //        M_WARNING_FRM.Dispose();
                    //    }
                    //}
                    Process.Stop();
                }

                switch (Process.ID)
                {
                    case 5:
                        //m_IsCheckBuffOK = false;
                        //SetRunningLight();

                        switch (Process.RelateString)
                        {
                            case "START_MAIN":

                                if (!MACHINE.PLCIO.SingleFEED.IsRunning)
                                    MACHINE.PLCIO.SingleFEED.Start = true;

                                //if (!m_MainSDProcess.IsOn)
                                //    m_MainSDProcess.Start();

                                break;
                            default:
                                SetRunningLight();
                                break;
                        }

                        //打开灯 
                        MACHINE.PLCIO.ADR_LIGHT = true;

                        //测试X轴 到达待命位置

                        if (!IsInRange(MOTOR_TEST_X.PositionNow, JzMainSDPositionParas.TEST_READY_XPOS, 0.5f))
                        {
                            MOTOR_TEST_X.Go(JzMainSDPositionParas.TEST_READY_XPOS);
                        }

                        Process.NextDuriation = 200;
                        Process.ID = 10;

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (MOTOR_TEST_X.IsOnSite && IsInRange(MOTOR_TEST_X.PositionNow, JzMainSDPositionParas.TEST_READY_XPOS, 0.5f) && MACHINE.PLCIO.ADR_LIGHT)
                            {
                                //m_GetImageProcess.Start();

                                Process.NextDuriation = 1300;
                                Process.ID = 1510;
                            }
                        }
                        break;
                    
                    case 1510:
                        if (Process.IsTimeup)
                        {
                            CCDCollection.GetImage();
                            //Process.Stop();

                            Process.NextDuriation = 0;
                            Process.ID = 15;
                        }
                        break;

                    case 15:
                        if (Process.IsTimeup)
                        {
                            //if (!m_GetImageProcess.IsOn)
                            {
                                //SetNormalLight();
                                Process.Stop();

                                int iret = CheckBuffProduct();
                                myTestRegionBuff = iret == 0;

                                switch (Process.RelateString)
                                {
                                    case "START_MAIN":

                                        if (!m_MainSDProcess.IsOn)
                                            m_MainSDProcess.Start();

                                        break;
                                    default:
                                        SetNormalLight();
                                        break;
                                }
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 判断BUFF区是否有料
        /// </summary>
        /// <returns>=0 有料  =-1无料</returns>
        private int CheckBuffProduct()
        {
            int iret = 0;
            int camIndex = 1;
            Rectangle rect = new Rectangle(2293, 959, 1660, 1590);

            //CCDCollection.GetImage(camIndex);
            Bitmap tempbmp = new Bitmap(CCDCollection.GetBMP(camIndex, false));
            Bitmap bmpCheckBuff = tempbmp.Clone(rect, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            bmpCheckBuff.Save(@"D:\LOA\BMPCHECK.BMP");
            JetGrayImg grayimage = new JetGrayImg(bmpCheckBuff);
            JetImgproc.Threshold(grayimage, 50, grayimage);
            grayimage.ToBitmap().Save(@"D:\LOA\BMPCHECKgray.BMP");
            JetBlob jetBlob = new JetBlob();
            jetBlob.Labeling(grayimage, JConnexity.Connexity4, JBlobLayer.BlackLayer);
            int iCount = jetBlob.BlobCount;
            //int maxarea = -1000;
            for (int i = 0; i < iCount; i++)
            {
                int iArea = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.Area);
                JRotatedRectangleF jetrect = JetBlobFeature.ComputeMinRectangle(jetBlob, i);
                if (iArea > 10000)
                {
                    JzToolsClass jzTools = new JzToolsClass();
                    string str = "Area=" + iArea.ToString() + Environment.NewLine;
                    str += "fWidth=" + jetrect.fWidth.ToString() + Environment.NewLine;
                    str += "fHeight=" + jetrect.fHeight.ToString() + Environment.NewLine;
                    jzTools.DrawText(bmpCheckBuff, str);
                    bmpCheckBuff.Save(@"D:\LOA\BMPCHECK.BMP");

                    if (IsInRangeRatio(iArea, INI.ChkArea, 15) && IsInRangeRatio(jetrect.fWidth, INI.ChkWidth, 15) && IsInRangeRatio(jetrect.fHeight, INI.ChkHeight, 15))
                    {
                        iret = -1;
                        break;
                    }

                    //JzToolsClass jzTools = new JzToolsClass();
                    //string str = "Area=" + iArea.ToString() + Environment.NewLine;
                    //str += "fWidth=" + jetrect.fWidth.ToString() + Environment.NewLine;
                    //str += "fHeight=" + jetrect.fHeight.ToString() + Environment.NewLine;
                    //jzTools.DrawText(bmpCheckBuff, str);
                    //bmpCheckBuff.Save(@"D:\LOA\BMPCHECK.BMP");
                }
            }

            tempbmp.Dispose();
            bmpCheckBuff.Dispose();

            return iret;
        }

        bool IsInRangeRatio(double FromValue, double CompValue, double Ratio)
        {
            return (FromValue >= (CompValue * (1 - (Ratio / 100d)))) && (FromValue <= (CompValue * (1 + (Ratio / 100d))));
        }
        bool IsInRange(float FromValue, float CompValue, float DiffValue)
        {
            return Math.Abs(FromValue - CompValue) < DiffValue;
        }
        private int CheckPrepared()
        {
            if (!IsNoUsePLC)
            {
                if (MACHINE.PLCIO.ADR_DOOR)
                {
                    M_WARNING_FRM = new MessageForm(true, "门被打开，无法操作。", "");
                    if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                    {
                    }
                    M_WARNING_FRM.Close();
                    M_WARNING_FRM.Dispose();

                    return -1;
                }

                if (MACHINE.PLCIO.ADR_ISEMC)
                {
                    M_WARNING_FRM = new MessageForm(true, "急停按钮被按下，无法操作。", "");
                    if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                    {
                    }
                    M_WARNING_FRM.Close();
                    M_WARNING_FRM.Dispose();

                    return -1;
                }
            }

            if (JzMainSDPositionParas.INSPECT_PASSINDEX >= INI.USER_SET_FULL_PASSCOUNT && !MACHINE.IsUserFroceCount)//达到用户设定的PASS数量 提示并停机
            {
                M_WARNING_FRM = new MessageForm(true, "收料区PASS区满盒，是否继续计数。");
                if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                {
                    MACHINE.PLCIO.TAKE_COMPLETE_TOPLC = true;
                    MACHINE.IsUserFroceCount = true;

                    //JzMainSDPositionParas.INSPECT_PASSINDEX = 0;
                    //JzMainSDPositionParas.SaveRecord();
                }
                //else
                //    MACHINE.IsUserFroceCount = false;
                M_WARNING_FRM.Close();
                M_WARNING_FRM.Dispose();
                if (!MACHINE.IsUserFroceCount)
                    return -1;
            }
            if (JzMainSDPositionParas.INSPECT_NGINDEX >= INI.USER_SET_FULL_NGCOUNT && !MACHINE.IsUserFroceCount)//达到用户设定的PASS数量 提示并停机
            {
                M_WARNING_FRM = new MessageForm(true, "收料区NG区满盒，是否继续计数。");
                if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                {
                    MACHINE.PLCIO.TAKE_COMPLETE_TOPLC = true;
                    MACHINE.IsUserFroceCount = true;
                    //JzMainSDPositionParas.INSPECT_NGINDEX = 0;
                    //JzMainSDPositionParas.SaveRecord();
                }
                //else
                //    MACHINE.IsUserFroceCount = false;
                M_WARNING_FRM.Close();
                M_WARNING_FRM.Dispose();

                if (!MACHINE.IsUserFroceCount)
                    return -1;
            }
            return 0;
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_MainEnable)
            {
                //switch (tabControl1.SelectedIndex)
                //{
                //    case 0:
                //    case 3:
                //        tpnlCover.Visible = false;
                //        break;
                //}

                tpnlCover.Visible = !(tabControl1.SelectedIndex == 0 || tabControl1.SelectedIndex == 3 || tabControl1.SelectedIndex == 4);
                //tpnlCover.Visible = !(tabControl1.SelectedIndex == 3);
            }
        }
        public void SetEnable(bool isenable)
        {
            m_MainEnable = isenable;
            tpnlCover.Visible = !isenable;

            if (!isenable)
            {
                tabControl1.SelectedIndex = 0;
                tpnlCover.Visible = false;
            }

            this.BackColor = (isenable ? SystemColors.Control : Color.DarkGray);
            this.tabPage1.BackColor = (isenable ? SystemColors.Control : Color.DarkGray);
            this.tabPage2.BackColor = (isenable ? SystemColors.Control : Color.DarkGray);
            this.tabPage3.BackColor = (isenable ? SystemColors.Control : Color.DarkGray);
            this.tabPage4.BackColor = (isenable ? SystemColors.Control : Color.DarkGray);
            this.tabPage5.BackColor = (isenable ? SystemColors.Control : Color.DarkGray);
            //this.tabPage6.BackColor = (isenable ? SystemColors.Control : Color.DarkGray);

            //this.tabPage7.BackColor = (isenable ? SystemColors.Control : Color.DarkGray);

        }
        public void Tick()
        {

            #region ALARM

            if (IsAlarmsSeriousX)
            {
                SetAbnormalLight();

                IsAlarmsSeriousX = false;
                StopAllProcess();
                SetSeriousAlarms0();
                SetSeriousAlarms1();

                //StopAllProcess();
            }

            if (IsAlarmsCommonX)
            {
                SetAbnormalLight();

                IsAlarmsCommonX = false;
                StopAllProcess();
                SetCommonAlarms();
               
            }

            if (!IsNoUsePLC)
            {
                if (IsEMCTriggered)
                {
                    SetAbnormalLight();

                    IsEMCTriggered = false;
                    StopAllProcess();
                    OnTrigger(ActionEnum.ACT_ISEMC, "");
                }
            }

            #endregion

            LoadOneTick();
            UnLoadOneTick();

            ResetPartialTick();
            ResetTick();
            //AlarmResetTick();
            CheckBuffTick();
            GetImageTick();

            btnStart.BackColor = (MACHINE.PLCIO.ADR_START ? Color.Red : Color.FromArgb(192, 255, 192));
            btnPause.BackColor = (MACHINE.PLCIO.ADR_PAUSE ? Color.Red : Color.FromArgb(192, 255, 192));
            //btnStop.BackColor = (MACHINE.PLCIO.ADR_START ? Color.Red : Color.FromArgb(192, 255, 192));
            //btnModeAutoManual.BackColor = (MACHINE.PLCIO.ADR_START ? Color.Red : Color.FromArgb(192, 255, 192));
            //btnAlarmClean = button2;

            lblEMC.BackColor = (MACHINE.PLCIO.ADR_ISEMC ? Color.Red : Color.Black);
            lblLampRed.BackColor = (MACHINE.PLCIO.Red ? Color.Red : Color.Black);
            lblLampGreen.BackColor = (MACHINE.PLCIO.Green ? Color.Green : Color.Black);
            lblLampYellow.BackColor = (MACHINE.PLCIO.Yellow ? Color.Yellow : Color.Black);
            lblBuzzer.BackColor = (MACHINE.PLCIO.ADR_BUZZER ? Color.Red : Color.Black);
            lblDoor.BackColor = (MACHINE.PLCIO.ADR_DOOR ? Color.Red : Color.Black);
            lblVaccFeed.BackColor = (MACHINE.PLCIO.ADR_FEEDVACC ? Color.Green : Color.Black);
            lblVaccTake.BackColor = (MACHINE.PLCIO.ADR_TAKEVACC ? Color.Green : Color.Black);
            lblLight.BackColor = (MACHINE.PLCIO.ADR_LIGHT ? Color.Green : Color.Black);
            lblBLOW.BackColor = (MACHINE.PLCIO.ADR_BLOW ? Color.Green : Color.Black);


            btnTAKEPASSLOAD.BackColor = (m_TakePASSloadprocess.IsOn ? Color.Green : Color.FromArgb(192, 255, 192));
            btnTAKEPASSUNLOAD.BackColor = (m_TakePASSUnloadprocess.IsOn ? Color.Green : Color.FromArgb(192, 255, 192));
            //btnTAKEPASSCLEAN.BackColor = (MACHINE.PLCIO.ADR_BLOW ? Color.Green : Color.Black);
            btnTAKENGLOAD.BackColor = (m_TakeNGloadprocess.IsOn ? Color.Green : Color.FromArgb(192, 255, 192));
            btnTAKENGUNLOAD.BackColor = (m_TakeNGUnloadprocess.IsOn ? Color.Green : Color.FromArgb(192, 255, 192));
            //btnTAKENGCLEAN.BackColor = (MACHINE.PLCIO.ADR_BLOW ? Color.Green : Color.Black);


            //AxisMotionControl[(int)MotionEnum.M0].Tick();
            //AxisMotionControl[(int)MotionEnum.M1].Tick();
            //AxisMotionControl[(int)MotionEnum.M2].Tick();

            //AxisMotionControl[(int)MotionEnum.M3].Tick();
            //AxisMotionControl[(int)MotionEnum.M4].Tick();
            //AxisMotionControl[(int)MotionEnum.M5].Tick();

            //AxisMotionControl[(int)MotionEnum.M6].Tick();
            //AxisMotionControl[(int)MotionEnum.M7].Tick();
            //AxisMotionControl[(int)MotionEnum.M8].Tick();

            //AxisMotionControl[(int)MotionEnum.M9].Tick();

            //IOControl.Tick();
            //STATEControl.Tick();

            //myTick();

            int i = 0;
            while (i < (int)RegionEnum.COUNT)
            {
                regionUI[i].Tick();
                i++;
            }

            UpdateStateUI();
        }

        bool m_IsCheckResetOK = false;
        ProcessClass resetprocess = new ProcessClass();
        private void ResetTick()
        {
            ProcessClass Process = resetprocess;

            if (Process.IsOn)
            {
                if (MACHINE.PLCIO.ADR_ISEMC)
                {
                    if (!m_IsCheckResetOK)
                    {
                        m_IsCheckResetOK = true;
                        if (M_WARNING_FRM != null)
                        {
                            M_WARNING_FRM.Close();
                            M_WARNING_FRM.Dispose();
                        }
                    }
                    Process.Stop();
                }

                switch (Process.ID)
                {
                    case 5:
                        m_IsCheckResetOK = false;

                        SetRunningLight();

                        M_WARNING_FRM = new MessageForm("馬達復位中，請稍後...", true);
                        M_WARNING_FRM.Show();

                        //MACHINE.PLCIO.RESET = true;

                        MACHINE.PLCIO.SingleRESET.Start = true;

                        Process.NextDuriation = 1000;
                        Process.ID = 10;

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.PLCIO.SingleRESET.IsComplete && !MACHINE.PLCIO.SingleRESET.IsRunning)
                            {
                                SetNormalLight();

                                if (M_WARNING_FRM != null)
                                {
                                    M_WARNING_FRM.Close();
                                    M_WARNING_FRM.Dispose();
                                }

                                Process.Stop();
                            }
                        }
                        break;
                }
            }
        }

        bool m_IsCheckResetPartialOK = false;
        ProcessClass resetpartialprocess = new ProcessClass();
        private void ResetPartialTick()
        {
            ProcessClass Process = resetpartialprocess;

            if (Process.IsOn)
            {
                if (MACHINE.PLCIO.ADR_ISEMC)
                {
                    if (!m_IsCheckResetPartialOK)
                    {
                        m_IsCheckResetPartialOK = true;
                        if (M_WARNING_FRM != null)
                        {
                            M_WARNING_FRM.Close();
                            M_WARNING_FRM.Dispose();
                        }
                    }
                    Process.Stop();
                }

                switch (Process.ID)
                {
                    case 5:
                        m_IsCheckResetPartialOK = false;

                        SetRunningLight();

                        M_WARNING_FRM = new MessageForm("常规报警復位中，請稍後...", true);
                        M_WARNING_FRM.Show();

                        MACHINE.PLCIO.AlarmCommonResetStart = true;

                        Process.NextDuriation = 1000;
                        Process.ID = 10;

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (!MACHINE.PLCIO.AlarmCommonReseting && MACHINE.PLCIO.AlarmCommonResetComplete)
                            {
                                SetNormalLight();

                                if (M_WARNING_FRM != null)
                                {
                                    M_WARNING_FRM.Close();
                                    M_WARNING_FRM.Dispose();
                                }

                                Process.Stop();
                            }
                        }
                        break;
                }
            }
        }

        bool m_IsCheckAlarmResetOK = false;
        ProcessClass alarmresetprocess = new ProcessClass();
        private void AlarmResetTick()
        {
            ProcessClass Process = alarmresetprocess;

            if (Process.IsOn)
            {
                if (MACHINE.PLCIO.ADR_ISEMC)
                {
                    if (!m_IsCheckAlarmResetOK)
                    {
                        m_IsCheckAlarmResetOK = true;
                        if (M_WARNING_FRM != null)
                        {
                            M_WARNING_FRM.Close();
                            M_WARNING_FRM.Dispose();
                        }
                    }
                    Process.Stop();
                }

                switch (Process.ID)
                {
                    case 5:

                        m_IsCheckAlarmResetOK = false;
                        Process.NextDuriation = 2000;
                        Process.ID = 1010;

                        //switch (Process.RelateString)
                        //{
                        //    case "SERIOUS":
                                
                        //        Process.ID = 1020;
                        //        break;
                        //    case "COMMON":

                        //        Process.ID = 1010;
                        //        break;
                        //    default:
                        //        Process.NextDuriation = 1000;
                        //        Process.ID = 1030;
                        //        break;

                        //}

                        break;

                    case 1010:
                        if (Process.IsTimeup)
                        {
                            Process.Stop();
                            MACHINE.EVENT.RemoveAlarm();

                            //if (MACHINE.PLCIO.IsAlarmsCommon)
                            //{
                            //    Process.Stop();
                            //}
                            //else
                            //{
                            //    MACHINE.EVENT.RemoveAlarm();
                            //    Process.Pause();
                            //    M_WARNING_FRM = new MessageForm(true, "是否要只执行小复位？");
                            //    if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                            //    {
                            //        Process.NextDuriation = 200;
                            //        Process.ID = 10;
                            //        Process.Continue();
                            //    }

                            //}
                        }
                        break;
                    case 1020:
                        if (Process.IsTimeup)
                        {
                            Process.Stop();
                            MACHINE.EVENT.RemoveAlarm();
                        }
                        break;

                    case 1030:
                        if (Process.IsTimeup)
                        {
                            Process.Stop();
                            MACHINE.EVENT.RemoveAlarm();

                            //MACHINE.EVENT.RemoveAlarm();
                            //Process.Pause();
                            //M_WARNING_FRM = new MessageForm(true, "是否要只执行小复位？");
                            //if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                            //{
                            //    Process.NextDuriation = 200;
                            //    Process.ID = 10;
                            //    Process.Continue();
                            //}

                        }
                        break;

                    case 10:
                        if (Process.IsTimeup)
                        {
                            SetRunningLight();

                            M_WARNING_FRM = new MessageForm("常规报警復位中，請稍後...", true);
                            M_WARNING_FRM.Show();

                            MACHINE.PLCIO.AlarmCommonResetStart = true;

                            Process.NextDuriation = 1000;
                            Process.ID = 15;
                        }
                        break;
                    case 15:
                        if (Process.IsTimeup)
                        {
                            if (!MACHINE.PLCIO.AlarmCommonReseting && MACHINE.PLCIO.AlarmCommonResetComplete)
                            {
                                SetNormalLight();

                                if (M_WARNING_FRM != null)
                                {
                                    M_WARNING_FRM.Close();
                                    M_WARNING_FRM.Dispose();
                                }

                                Process.Stop();
                            }
                        }
                        break;
                }
            }
        }

        #region ALARMS

        bool IsAlarmsSeriousX = false;
        bool IsAlarmsCommonX = false;

        void SetSeriousAlarms0()
        {
            foreach (PLCAlarmsItemDescriptionClass item in MACHINE.PLCIO.PLCALARMS[(int)MainSDAlarmsEnum.ALARMS_ADR_SERIOUS0].PLCALARMSDESCLIST)
            {
                if (MACHINE.PLCIO.GetAlarmsAddress(item.BitNo, item.ADR_Address))
                {
                    MACHINE.EVENT.GenEvent("A0001", EventActionTypeEnum.AUTOMATIC, item.ADR_Chinese, ACCDB.DataNow);
                }
            }
        }
        void SetSeriousAlarms1()
        {
            foreach (PLCAlarmsItemDescriptionClass item in MACHINE.PLCIO.PLCALARMS[(int)MainSDAlarmsEnum.ALARMS_ADR_SERIOUS1].PLCALARMSDESCLIST)
            {
                if (MACHINE.PLCIO.GetAlarmsAddress(item.BitNo, item.ADR_Address))
                {
                    MACHINE.EVENT.GenEvent("A0001", EventActionTypeEnum.AUTOMATIC, item.ADR_Chinese, ACCDB.DataNow);
                }
            }
        }
        void SetCommonAlarms()
        {
            foreach (PLCAlarmsItemDescriptionClass item in MACHINE.PLCIO.PLCALARMS[(int)MainSDAlarmsEnum.ALARMS_ADR_COMMON].PLCALARMSDESCLIST)
            {
                if (MACHINE.PLCIO.GetAlarmsAddress(item.BitNo, item.ADR_Address))
                {
                    MACHINE.EVENT.GenEvent("A0002", EventActionTypeEnum.AUTOMATIC, item.ADR_Chinese, ACCDB.DataNow);
                }
            }
        }

        void SetNormalLight()
        {
            MACHINE.PLCIO.Red = false;
            MACHINE.PLCIO.Yellow = true;
            MACHINE.PLCIO.Green = false;
        }
        void SetAbnormalLight()
        {
            MACHINE.PLCIO.Red = true;
            MACHINE.PLCIO.Yellow = false;
            MACHINE.PLCIO.Green = false;
        }
        void SetRunningLight()
        {
            MACHINE.PLCIO.Red = false;
            MACHINE.PLCIO.Yellow = false;
            MACHINE.PLCIO.Green = true;
        }
        void SetBuzzer(bool IsON)
        {
            //USEIO.Buzzer = IsON;
            MACHINE.PLCIO.ADR_BUZZER = IsON;
        }



        #endregion


        #region MAIN_SD PROCESS TICK

        /// <summary>
        /// 线程启动中标志
        /// </summary>
        bool m_thread_runniing = false;

        /// <summary>
        /// 释放资源并关闭线程
        /// </summary>
        public void SDDispose()
        {
            MACHINE.PLCIO.ADR_LIGHT = false;

            SetNormalLight();

            ThreadStop();
        }

        private void ThreadStart()
        {

            MACHINE.PLCIO.ADR_LIGHT = true;
            MACHINE.PLCIO.ADR_BLOW = true;
            SetNormalLight();

            m_thread_runniing = true;

            if (m_threadForPrcess == null)
                m_threadForPrcess = new System.Threading.Thread(new System.Threading.ThreadStart(MainSDThread));

            m_threadForPrcess.IsBackground = true;
            m_threadForPrcess.Start();
        }
        private void ThreadStop()
        {
            m_thread_runniing = false;
            if (m_threadForPrcess != null)
            {
                if (m_threadForPrcess.ThreadState != System.Threading.ThreadState.Aborted)
                {
                    m_threadForPrcess.Abort();
                }
                m_threadForPrcess = null;
            }
            MACHINE.PLCIO.ADR_BLOW = false;
        }
        void MainSDThread()
        {
            while (m_thread_runniing)
            {
                System.Threading.Thread.Sleep(1);
                myTick();
            }
        }


        /// <summary>
        /// 用于流程的间隔时间
        /// </summary>
        int[] iNextDurtime = new int[10];
        /// <summary>
        /// 记录测试区域是否有料的缓存值
        /// </summary>
        bool myTestRegionBuff = false;
        /// <summary>
        /// 判断第一次是否放料计数 true 不计数 false 计数
        /// </summary>
        bool IsJudgeFirstTake = false;
        ///// <summary>
        ///// 记录当前放料的片数
        ///// </summary>
        // int iCurrentTake = 0;

        /// <summary>
        /// 停机需要看模式
        /// </summary>
        /// <param name="eStrMode">FULL 满盒  USERSTOP 用户按钮停止</param>
        public void StopAllProcess(string eStrMode = "")
        {
            m_MainSDProcess.Stop();
            m_FeedProcess.Stop();
            //m_TakeProcess.Stop();
            m_TestProcess.Stop();
            //alarmresetprocess.Stop();

            loadoneprocess.Stop();
            Unloadoneprocess.Stop();

            StopRegionProcess();

            switch (eStrMode)
            {
                case "FULL":
                    m_TakeProcess.Stop();
                    SetNormalLight();
                    break;

                case "INIT":
                case "USERSTOP":

                    if (m_TakeProcess.ID <= 10)
                        m_TakeProcess.Stop();

                    alarmresetprocess.Stop();
                    resetprocess.Stop();
                    resetpartialprocess.Stop();

                    checkbuffprocess.Stop();
                    m_GetImageProcess.Stop();

                    //MACHINE.PLCIO.SingleTAKE.Stop = true;
                    //MACHINE.PLCIO.TAKE_PASS = false;
                    //MACHINE.PLCIO.TAKE_NG = false;

                    MACHINE.PLCIO.SingleFEED.Stop = true;
                    //MACHINE.PLCIO.SingleTESTLEFT.Stop = true;
                    //MACHINE.PLCIO.SingleTESTLEFTRIGHT.Stop = true;
                    SetNormalLight();
                    break;

                default:
                    m_TakeProcess.Stop();

                    MACHINE.PLCIO.SingleTAKE.Stop = true;
                    MACHINE.PLCIO.TAKE_PASS = false;
                    MACHINE.PLCIO.TAKE_NG = false;

                    MACHINE.PLCIO.SingleFEED.Stop = true;
                    MACHINE.PLCIO.SingleTESTLEFT.Stop = true;
                    MACHINE.PLCIO.SingleTESTLEFTRIGHT.Stop = true;

                    break;
            }

            
        }

        private void myTick()
        {
            MainSDTick();
            FeedTick();
            TestTick();
            TakeTick();

            BuzzerTick();

            //RegionTick();
        }

        ProcessClass m_baseProcess = new ProcessClass();
        private void BaseTick()
        {
            ProcessClass Process = m_baseProcess;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:
                        Process.NextDuriation = 1000;
                        Process.ID = 10;

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            Process.NextDuriation = 1000;
                            Process.ID = 15;
                        }
                        break;
                    case 15:
                        if (Process.IsTimeup)
                        {
                            Process.Stop();
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 主流程
        /// </summary>
        ProcessClass m_MainSDProcess = new ProcessClass();
        private void MainSDTick()
        {
            ProcessClass Process = m_MainSDProcess;
            iNextDurtime[0] = 200;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        SetRunningLight();

                        //MACHINE.SetLight("ALL");
                        if (!MACHINE.PLCIO.ADR_LIGHT)
                            MACHINE.PLCIO.ADR_LIGHT = true;

                        OnTrigger(ActionEnum.ACT_SETCAMEXPOSURE, "");

                        if (myTestRegionBuff)
                        {
                            Process.NextDuriation = iNextDurtime[0];
                            Process.ID = 5100;

                            IsJudgeFirstTake = false;
                        }
                        else
                        {
                            Process.NextDuriation = iNextDurtime[0];
                            Process.ID = 10;

                            IsJudgeFirstTake = true;
                        }

                        break;

                    #region 开始启动检查缓存区是否有料流程

                    case 5100:
                        if (Process.IsTimeup)
                        {
                            if (!m_TakeProcess.IsOn)
                                m_TakeProcess.Start();

                            Process.NextDuriation = iNextDurtime[0];
                            Process.ID = 5110;
                        }
                        break;
                    case 5110:
                        if (Process.IsTimeup)
                        {
                            if (!m_TakeProcess.IsOn)
                            {
                                Process.NextDuriation = iNextDurtime[0];
                                Process.ID = 10;
                            }
                        }
                        break;

                    #endregion

                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (!m_FeedProcess.IsOn)
                                m_FeedProcess.Start();

                            Process.NextDuriation = iNextDurtime[0];
                            Process.ID = 15;
                        }
                        break;
                    case 15:
                        if (Process.IsTimeup)
                        {
                            if (!m_FeedProcess.IsOn)
                            {
                                if (!m_TakeProcess.IsOn)
                                {
                                    if (IsJudgeFirstTake)
                                    {
                                        IsJudgeFirstTake = false;
                                        m_TakeProcess.Start();

                                        Process.NextDuriation = iNextDurtime[0];
                                        Process.ID = 20;
                                    }
                                    else
                                    {
                                        //if (MACHINE.IsGetImageComplete)
                                        {
                                            m_TakeProcess.Start("COUNT");

                                            Process.NextDuriation = iNextDurtime[0];
                                            Process.ID = 20;
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case 20:
                        if (Process.IsTimeup)
                        {
                            if (!m_TakeProcess.IsOn)
                            {
                                Process.NextDuriation = iNextDurtime[0];
                                Process.ID = 30;
                            }
                        }
                        break;
                    case 30:
                        if (Process.IsTimeup)
                        {
                            //if (!MACHINE.PLCIO.SingleFEED.IsComplete)//new 20230731
                            if (MACHINE.PLCIO.FEED_IsHaveProduct)
                            {
                                Process.NextDuriation = iNextDurtime[0];
                                Process.ID = 10;
                            }
                            else
                            {
                                if (myTestRegionBuff)
                                {
                                    if (!m_FeedProcess.IsOn)
                                        m_FeedProcess.Start();

                                    Process.NextDuriation = iNextDurtime[0];
                                    Process.ID = 35;
                                }
                                else
                                {
                                    Process.NextDuriation = iNextDurtime[0];
                                    Process.ID = 40;
                                }
                            }
                        }
                        break;
                    case 35:
                        if (Process.IsTimeup)
                        {
                            if (!m_FeedProcess.IsOn)
                            {
                                if (MACHINE.IsGetImageComplete)
                                {
                                    if (!m_TakeProcess.IsOn)
                                        m_TakeProcess.Start("COUNT_LAST");

                                    Process.NextDuriation = iNextDurtime[0];
                                    Process.ID = 3501;
                                }
                            }
                        }
                        break;
                    case 3501:
                        if (Process.IsTimeup)
                        {
                            if (!m_TakeProcess.IsOn)
                            {
                                Process.NextDuriation = iNextDurtime[0];
                                Process.ID = 40;
                            }
                        }
                        break;
                    case 40:
                        if (Process.IsTimeup)
                        {
                            //myTestRegionBuff = false;
                            //Process.Stop();
                            if (MOTOR_TEST_X.IsOnSite && !MACHINE.PLCIO.FEED_YMoving && !MACHINE.PLCIO.TAKE_YMoving)
                            {
                                //结束后 测试X轴 到待命位置
                                //MOTOR_TEST_X.Go(JzMainSDPositionParas.TEST_READY_XPOS);

                                if (!IsInRange(MOTOR_TEST_X.PositionNow, JzMainSDPositionParas.TEST_READY_XPOS, 0.5f))
                                {
                                    MOTOR_TEST_X.Go(JzMainSDPositionParas.TEST_READY_XPOS);
                                }

                                Process.NextDuriation = 1500;// iNextDurtime[0];
                                Process.ID = 4010;

                                //SetNormalLight();
                            }
                        }
                        break;

                    case 4010:
                        if (Process.IsTimeup)
                        {
                            //myTestRegionBuff = false;
                            //Process.Stop();
                            if (MOTOR_TEST_X.IsOnSite && !MACHINE.PLCIO.FEED_YMoving && !MACHINE.PLCIO.TAKE_YMoving)
                            {
                                //结束后 测试X轴 到待命位置
                                //MOTOR_TEST_X.Go(JzMainSDPositionParas.TEST_READY_XPOS);

                                if (!IsInRange(MOTOR_TEST_X.PositionNow, JzMainSDPositionParas.TEST_READY_XPOS, 0.5f))
                                {
                                    MOTOR_TEST_X.Go(JzMainSDPositionParas.TEST_READY_XPOS);
                                }

                                Process.NextDuriation = iNextDurtime[0];
                                Process.ID = 50;

                                //SetNormalLight();
                            }
                        }
                        break;

                    case 50:
                        if (Process.IsTimeup)
                        {
                            if (MOTOR_TEST_X.IsOnSite && IsInRange(MOTOR_TEST_X.PositionNow, JzMainSDPositionParas.TEST_READY_XPOS, 0.5f))
                            {
                                //myTestRegionBuff = false;
                                Process.Stop();
                                //结束后 测试X轴 到待命位置
                                //MOTOR_TEST_X.Go(JzMainSDPositionParas.TEST_READY_XPOS);

                                SetNormalLight();

                                MACHINE.PLCIO.ADR_LIGHT = false;

                                m_BuzzerProcess.Start();
                            }
                        }
                        break;
                }
            }
        }

        #region MainSD bak
        /*MainSD BAK
        private void MainSDTickBAK20211202()
        {
            ProcessClass Process = m_MainSDProcess;
            iNextDurtime[0] = 200;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        SetRunningLight();

                        //MACHINE.SetLight("ALL");
                        if (!MACHINE.PLCIO.ADR_LIGHT)
                            MACHINE.PLCIO.ADR_LIGHT = true;

                        OnTrigger(ActionEnum.ACT_SETCAMEXPOSURE, "");

                        if (myTestRegionBuff)
                        {
                            Process.NextDuriation = iNextDurtime[0];
                            Process.ID = 5120;

                            IsJudgeFirstTake = false;
                        }
                        else
                        {
                            Process.NextDuriation = iNextDurtime[0];
                            Process.ID = 10;

                            IsJudgeFirstTake = true;
                        }

                        break;

                    #region 开始启动检查缓存区是否有料流程

                    case 5100:
                        if (Process.IsTimeup)
                        {
                            if (!m_TestProcess.IsOn)
                                m_TestProcess.Start();

                            Process.NextDuriation = iNextDurtime[0];
                            Process.ID = 5110;
                        }
                        break;
                    case 5110:
                        if (Process.IsTimeup)
                        {
                            if (!m_TestProcess.IsOn)
                            {
                                Process.NextDuriation = iNextDurtime[0];
                                Process.ID = 10;
                            }
                        }
                        break;

                    case 5120:
                        if (Process.IsTimeup)
                        {
                            if (!m_GetImageProcess.IsOn && MACHINE.PLCIO.ADR_LIGHT)
                            {
                                m_GetImageProcess.Start();

                                Process.NextDuriation = iNextDurtime[0];
                                Process.ID = 5130;
                            }
                        }
                        break;
                    case 5130:
                        if (Process.IsTimeup)
                        {
                            if (!m_GetImageProcess.IsOn)
                            {
                                Process.NextDuriation = iNextDurtime[0];
                                Process.ID = 5100;
                            }
                        }
                        break;

                    #endregion

                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (!m_FeedProcess.IsOn)
                                m_FeedProcess.Start();

                            Process.NextDuriation = iNextDurtime[0];
                            Process.ID = 15;
                        }
                        break;
                    case 15:
                        if (Process.IsTimeup)
                        {
                            if (!m_FeedProcess.IsOn)
                            {
                                if (!m_TakeProcess.IsOn)
                                {
                                    if (IsJudgeFirstTake)
                                    {
                                        //IsJudgeFirstTake = false;
                                        m_TakeProcess.Start("");
                                    }
                                    else
                                        m_TakeProcess.Start("COUNT");
                                }

                                Process.NextDuriation = iNextDurtime[0];
                                Process.ID = 20;
                            }
                        }
                        break;
                    case 20:
                        if (Process.IsTimeup)
                        {
                            if (!m_TakeProcess.IsOn)
                            {
                                if (myTestRegionBuff)
                                {
                                    if (!m_TestProcess.IsOn)
                                        m_TestProcess.Start();

                                    Process.NextDuriation = iNextDurtime[0];
                                    Process.ID = 25;
                                }
                                else
                                {
                                    Process.NextDuriation = iNextDurtime[0];
                                    Process.ID = 30;
                                }
                            }
                        }
                        break;
                    case 25:
                        if (Process.IsTimeup)
                        {
                            if (!m_TestProcess.IsOn)
                            {
                                Process.NextDuriation = iNextDurtime[0];
                                Process.ID = 30;
                            }
                        }
                        break;
                    case 30:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.PLCIO.FEED_IsHaveProduct)
                            {
                                Process.NextDuriation = iNextDurtime[0];
                                Process.ID = 10;

                                //if (IsJudgeFirstTake)
                                //{
                                //    IsJudgeFirstTake = false;
                                //}
                            }
                            else
                            {
                                if (myTestRegionBuff)
                                {
                                    if (!m_FeedProcess.IsOn)
                                        m_FeedProcess.Start();

                                    Process.NextDuriation = iNextDurtime[0];
                                    Process.ID = 35;
                                }
                                else
                                {
                                    Process.NextDuriation = iNextDurtime[0];
                                    Process.ID = 40;
                                }
                            }
                        }
                        break;
                    case 35:
                        if (Process.IsTimeup)
                        {
                            if (!m_FeedProcess.IsOn)
                            {
                                if (!m_TakeProcess.IsOn)
                                    m_TakeProcess.Start("COUNT");

                                Process.NextDuriation = iNextDurtime[0];
                                Process.ID = 3501;
                            }
                        }
                        break;
                    case 3501:
                        if (Process.IsTimeup)
                        {
                            if (!m_TakeProcess.IsOn)
                            {
                                Process.NextDuriation = iNextDurtime[0];
                                Process.ID = 40;
                            }
                        }
                        break;
                    case 40:
                        if (Process.IsTimeup)
                        {
                            //myTestRegionBuff = false;
                            //Process.Stop();
                            if (MOTOR_TEST_X.IsOnSite && !MACHINE.PLCIO.FEED_YMoving && !MACHINE.PLCIO.TAKE_YMoving)
                            {
                                //结束后 测试X轴 到待命位置
                                //MOTOR_TEST_X.Go(JzMainSDPositionParas.TEST_READY_XPOS);

                                if (!IsInRange(MOTOR_TEST_X.PositionNow, JzMainSDPositionParas.TEST_READY_XPOS, 0.5f))
                                {
                                    MOTOR_TEST_X.Go(JzMainSDPositionParas.TEST_READY_XPOS);
                                }

                                Process.NextDuriation = 1500;// iNextDurtime[0];
                                Process.ID = 4010;

                                //SetNormalLight();
                            }
                        }
                        break;

                    case 4010:
                        if (Process.IsTimeup)
                        {
                            //myTestRegionBuff = false;
                            //Process.Stop();
                            if (MOTOR_TEST_X.IsOnSite && !MACHINE.PLCIO.FEED_YMoving && !MACHINE.PLCIO.TAKE_YMoving)
                            {
                                //结束后 测试X轴 到待命位置
                                //MOTOR_TEST_X.Go(JzMainSDPositionParas.TEST_READY_XPOS);

                                if (!IsInRange(MOTOR_TEST_X.PositionNow, JzMainSDPositionParas.TEST_READY_XPOS, 0.5f))
                                {
                                    MOTOR_TEST_X.Go(JzMainSDPositionParas.TEST_READY_XPOS);
                                }

                                Process.NextDuriation = iNextDurtime[0];
                                Process.ID = 50;

                                //SetNormalLight();
                            }
                        }
                        break;

                    case 50:
                        if (Process.IsTimeup)
                        {
                            if (MOTOR_TEST_X.IsOnSite && IsInRange(MOTOR_TEST_X.PositionNow, JzMainSDPositionParas.TEST_READY_XPOS, 0.5f))
                            {
                                //myTestRegionBuff = false;
                                Process.Stop();
                                //结束后 测试X轴 到待命位置
                                //MOTOR_TEST_X.Go(JzMainSDPositionParas.TEST_READY_XPOS);

                                SetNormalLight();

                                MACHINE.PLCIO.ADR_LIGHT = false;

                                m_BuzzerProcess.Start();
                            }
                        }
                        break;
                }
            }
        }
        */
        #endregion

        /// <summary>
        /// 供料流程
        /// </summary>
        ProcessClass m_FeedProcess = new ProcessClass();
        private void FeedTick()
        {
            ProcessClass Process = m_FeedProcess;
            iNextDurtime[1] = 200;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:
                        if (MACHINE.ContinueNGIndex >= INI.CONTINUE_NG_COUNT)
                        {
                            Process.Stop();
                            JzMainSDPositionParas.SaveRecord();
                            StopAllProcess("FULL");
                            OnTrigger(ActionEnum.ACT_CONTINUE_COUNT, "");
                        }
                        else if (JzMainSDPositionParas.INSPECT_PASSINDEX >= INI.USER_SET_FULL_PASSCOUNT && !MACHINE.IsUserFroceCount)//达到用户设定的数量 提示并停机
                        {
                            Process.Stop();
                            JzMainSDPositionParas.SaveRecord();
                            StopAllProcess("FULL");
                            OnTrigger(ActionEnum.ACT_USER_FULL_PASS, "");
                        }
                        else if (JzMainSDPositionParas.INSPECT_NGINDEX >= INI.USER_SET_FULL_NGCOUNT && !MACHINE.IsUserFroceCount)//达到用户设定的数量 提示并停机
                        {
                            Process.Stop();
                            JzMainSDPositionParas.SaveRecord();
                            StopAllProcess("FULL");
                            OnTrigger(ActionEnum.ACT_USER_FULL_NG, "");
                        }
                        else if (MOTOR_REGION_PASS.PositionNow <= 0 || MOTOR_REGION_PASS.IsReachHomeBound)//硬件达到满盒 提示并停机
                        {
                            Process.Stop();
                            JzMainSDPositionParas.SaveRecord();
                            StopAllProcess("FULL");
                            OnTrigger(ActionEnum.ACT_SENSOR_FULL_PASS, "");
                        }
                        else if (MOTOR_REGION_NG.PositionNow <= 0 || MOTOR_REGION_NG.IsReachHomeBound)//硬件达到满盒 提示并停机
                        {
                            Process.Stop();
                            JzMainSDPositionParas.SaveRecord();
                            StopAllProcess("FULL");
                            OnTrigger(ActionEnum.ACT_SENSOR_FULL_NG, "");
                        }
                        else
                        {

                            Process.NextDuriation = iNextDurtime[1];
                            Process.ID = 10;

                            if (!MACHINE.PLCIO.SingleFEED.IsRunning)
                                MACHINE.PLCIO.SingleFEED.Start = true;
                        }

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            //这里修改判断逻辑 判断是否供料完成 因为供料2个区分为单独得2个部分
                            //if(!MACHINE.PLCIO.SingleFEED.IsComplete)//new 20230731
                            if (MACHINE.PLCIO.FEED_IsHaveProduct)//old
                            //if (MACHINE.PLCIO.FEED_IsHaveProduct && !MACHINE.PLCIO.FEED_YMoving)
                            {
                                Process.NextDuriation = iNextDurtime[1];
                                Process.ID = 1001;
                            }
                            else if (MACHINE.PLCIO.SingleFEED.IsComplete)//new 20230731
                            //else if (!MACHINE.PLCIO.FEED_IsHaveProduct)//old
                            {
                                if (myTestRegionBuff)
                                //if (myTestRegionBuff && !MACHINE.PLCIO.FEED_YMoving)
                                {
                                    //无料 且 缓存区 有料 右吸
                                    MACHINE.PLCIO.SingleTESTRIGHT.Start = true;

                                    Process.NextDuriation = iNextDurtime[1];
                                    Process.ID = 25;
                                }
                                else if (!myTestRegionBuff)
                                {
                                    Process.Stop();
                                }
                            }
                        }
                        break;
                    case 1001://有料 再判断料的上位
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.PLCIO.FEED_IsHaveComplete)
                            //if (MACHINE.PLCIO.FEED_IsHaveComplete && !MACHINE.PLCIO.FEED_YMoving)
                            {
                                if (!myTestRegionBuff)
                                {
                                    //myTestRegionBuff = true;

                                    MACHINE.PLCIO.SingleTESTLEFT.Start = true;

                                    Process.NextDuriation = iNextDurtime[1];
                                    Process.ID = 15;
                                }
                                else
                                {
                                    MACHINE.PLCIO.SingleTESTLEFTRIGHT.Start = true;

                                    Process.NextDuriation = iNextDurtime[1];
                                    Process.ID = 20;
                                }
                            }
                        }
                        break;

                    case 15:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.PLCIO.SingleTESTLEFT.IsComplete)
                            {
                                myTestRegionBuff = true;
                                Process.Stop();
                            }
                        }
                        break;
                    case 20:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.PLCIO.SingleTESTLEFTRIGHT.IsComplete)
                            {
                                myTestRegionBuff = true;
                                Process.Stop();
                            }
                        }
                        break;
                    case 25:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.PLCIO.SingleTESTRIGHT.IsComplete)
                            {
                                myTestRegionBuff = false;
                                Process.Stop();
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 测试流程-取像完成
        /// </summary>
        ProcessClass m_TestProcess = new ProcessClass();
        private void TestTick()
        {
            ProcessClass Process = m_TestProcess;
            //iNextDurtime[3] = 1000;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:
                        Process.Stop();
                        return;

                        if (MACHINE.ContinueNGIndex >= INI.CONTINUE_NG_COUNT)
                        {
                            Process.Stop();
                            JzMainSDPositionParas.SaveRecord();
                            StopAllProcess("FULL");
                            OnTrigger(ActionEnum.ACT_CONTINUE_COUNT, "");
                        }
                        else if(JzMainSDPositionParas.INSPECT_PASSINDEX >= INI.USER_SET_FULL_PASSCOUNT && !MACHINE.IsUserFroceCount)//达到用户设定的数量 提示并停机
                        {
                            Process.Stop();
                            JzMainSDPositionParas.SaveRecord();
                            StopAllProcess("FULL");
                            OnTrigger(ActionEnum.ACT_USER_FULL_PASS, "");
                        }
                        else if (JzMainSDPositionParas.INSPECT_NGINDEX >= INI.USER_SET_FULL_NGCOUNT && !MACHINE.IsUserFroceCount)//达到用户设定的数量 提示并停机
                        {
                            Process.Stop();
                            JzMainSDPositionParas.SaveRecord();
                            StopAllProcess("FULL");
                            OnTrigger(ActionEnum.ACT_USER_FULL_NG, "");
                        }
                        else if (MOTOR_REGION_PASS.PositionNow <= 0 || MOTOR_REGION_PASS.IsReachHomeBound)//硬件达到满盒 提示并停机
                        {
                            Process.Stop();
                            JzMainSDPositionParas.SaveRecord();
                            StopAllProcess("FULL");
                            OnTrigger(ActionEnum.ACT_SENSOR_FULL_PASS, "");
                        }
                        else if (MOTOR_REGION_NG.PositionNow <= 0 || MOTOR_REGION_NG.IsReachHomeBound)//硬件达到满盒 提示并停机
                        {
                            Process.Stop();
                            JzMainSDPositionParas.SaveRecord();
                            StopAllProcess("FULL");
                            OnTrigger(ActionEnum.ACT_SENSOR_FULL_NG, "");
                        }
                        else
                        {
                            //Process.NextDuriation = 0;
                            //Process.ID = 10;

                            //MACHINE.SetLight("ALL");
                            //置位 off
                            MACHINE.IsGetImageComplete = false;//no use
                            //MACHINE.PLCIO.PCGetImageComplete = false;

                            OnTrigger(ActionEnum.ACT_TEST_GETIMAGE, "");//no use

                            Process.NextDuriation = 0;
                            Process.ID = 15;

                        }

                        break;
                    //case 10:
                    //    if (Process.IsTimeup)
                    //    {
                    //        //if (MACHINE.PLCIO.ADR_LIGHT)
                    //        {
                    //            //开始取像测试
                    //            OnTrigger(ActionEnum.ACT_TEST_GETIMAGE, "");

                    //            Process.NextDuriation = 100;
                    //            Process.ID = 15;
                    //        }
                    //    }
                    //    break;
                    case 15:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.IsGetImageComplete)//no use
                            //if (MACHINE.PLCIO.PCGetImageComplete)
                            {
                                Process.Stop();
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 放料流程
        /// </summary>
        ProcessClass m_TakeProcess = new ProcessClass();
        private void TakeTick()
        {
            ProcessClass Process = m_TakeProcess;
            iNextDurtime[2] = 100;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        if (MACHINE.ContinueNGIndex >= INI.CONTINUE_NG_COUNT)
                        {
                            Process.Stop();
                            JzMainSDPositionParas.SaveRecord();
                            StopAllProcess("FULL");
                            OnTrigger(ActionEnum.ACT_CONTINUE_COUNT, "");
                        }
                        else if (JzMainSDPositionParas.INSPECT_PASSINDEX >= INI.USER_SET_FULL_PASSCOUNT && !MACHINE.IsUserFroceCount)//达到用户设定的数量 提示并停机
                        {
                            Process.Stop();
                            JzMainSDPositionParas.SaveRecord();
                            StopAllProcess("FULL");
                            OnTrigger(ActionEnum.ACT_USER_FULL_PASS, "");
                        }
                        else if (JzMainSDPositionParas.INSPECT_NGINDEX >= INI.USER_SET_FULL_NGCOUNT && !MACHINE.IsUserFroceCount)//达到用户设定的数量 提示并停机
                        {
                            Process.Stop();
                            JzMainSDPositionParas.SaveRecord();
                            StopAllProcess("FULL");
                            OnTrigger(ActionEnum.ACT_USER_FULL_NG, "");
                        }
                        else if (MOTOR_REGION_PASS.PositionNow <= 0 || MOTOR_REGION_PASS.IsReachHomeBound)//硬件达到满盒 提示并停机
                        {
                            Process.Stop();
                            JzMainSDPositionParas.SaveRecord();
                            StopAllProcess("FULL");
                            OnTrigger(ActionEnum.ACT_SENSOR_FULL_PASS, "");
                        }
                        else if (MOTOR_REGION_NG.PositionNow <= 0 || MOTOR_REGION_NG.IsReachHomeBound)//硬件达到满盒 提示并停机
                        {
                            Process.Stop();
                            JzMainSDPositionParas.SaveRecord();
                            StopAllProcess("FULL");
                            OnTrigger(ActionEnum.ACT_SENSOR_FULL_NG, "");
                        }
                        else
                        {
                            Process.NextDuriation = iNextDurtime[2];
                            Process.ID = 10;
                        }

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            //if (!MACHINE.PLCIO.TAKE_PASS && !MACHINE.PLCIO.TAKE_NG)
                            //if (!MACHINE.PLCIO.TAKE_PASS && !MACHINE.PLCIO.TAKE_NG && !MACHINE.PLCIO.TAKE_YMoving)
                            {
                                if (!MACHINE.PLCIO.SingleTAKE.IsRunning)
                                    MACHINE.PLCIO.SingleTAKE.Start = true;

                                Process.NextDuriation = iNextDurtime[2];
                                Process.ID = 1510;
                            }
                        }
                        break;

                    case 1510:
                        if (Process.IsTimeup)
                        {
                            if(MACHINE.PLCIO.TAKE_PLC_GETIMAGEONOFF)
                            {
                                switch (Process.RelateString)
                                {

                                    case "":

                                        m_GetImageProcess.Start();

                                        Process.NextDuriation = iNextDurtime[2];
                                        Process.ID = 1520;

                                        break;

                                    case "COUNT"://计数
                                    case "COUNT_LAST":

                                        if (MACHINE.IsGetImageComplete)
                                        {

                                            if (JzMainSDPositionParas.INSPECT_RESULT)
                                            {
                                                JzMainSDPositionParas.INSPECT_PASSINDEX++;
                                                MACHINE.ContinueNGIndex = 0;//测到PASS 将NG的序号置0
                                            }
                                            else
                                            {
                                                JzMainSDPositionParas.INSPECT_NGINDEX++;
                                                MACHINE.ContinueNGIndex++;

                                                JzMainSDPositionParas.ReportAUTOSave(JzMainSDPositionParas.INSPECT_NGINDEX, false);
                                            }

                                            JzMainSDPositionParas.SaveRecord();

                                            switch(Process.RelateString)
                                            {
                                                case "COUNT":

                                                    m_GetImageProcess.Start();
                                                    Process.NextDuriation = iNextDurtime[2];
                                                    Process.ID = 1520;

                                                    break;
                                                case "COUNT_LAST":

                                                    
                                                    Process.NextDuriation = iNextDurtime[2];
                                                    Process.ID = 15;

                                                    break;
                                            }

                                          

                                        }

                                        break;


                                       
                                }
                            }
                        }
                        break;
                    case 1520:
                        if (Process.IsTimeup)
                        {
                            if (!m_GetImageProcess.IsOn)
                            {
                                Process.NextDuriation = 0;
                                Process.ID = 15;

                                //取完像 开始测试

                                //置位 off
                                MACHINE.IsGetImageComplete = false;
                                //MACHINE.PLCIO.PCGetImageComplete = false;

                                OnTrigger(ActionEnum.ACT_TEST_GETIMAGE, "");

                            }
                        }
                        break;

                    case 15:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.PLCIO.SingleTAKE.IsComplete)
                            {

                                MACHINE.PLCIO.TAKE_PLC_GETIMAGEONOFF = false;

                                Process.Stop();
                            }
                        }
                        break;
                }
            }
        }

        #region BAK TAKETICK
        /* TakeTick bak
        private void TakeTickBAK20211202()
        {
            ProcessClass Process = m_TakeProcess;
            iNextDurtime[2] = 200;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        if (MACHINE.ContinueNGIndex >= INI.CONTINUE_NG_COUNT)
                        {
                            Process.Stop();
                            JzMainSDPositionParas.SaveRecord();
                            StopAllProcess("FULL");
                            OnTrigger(ActionEnum.ACT_CONTINUE_COUNT, "");
                        }
                        else if (JzMainSDPositionParas.INSPECT_PASSINDEX >= INI.USER_SET_FULL_PASSCOUNT && !MACHINE.IsUserFroceCount)//达到用户设定的数量 提示并停机
                        {
                            Process.Stop();
                            JzMainSDPositionParas.SaveRecord();
                            StopAllProcess("FULL");
                            OnTrigger(ActionEnum.ACT_USER_FULL_PASS, "");
                        }
                        else if (JzMainSDPositionParas.INSPECT_NGINDEX >= INI.USER_SET_FULL_NGCOUNT && !MACHINE.IsUserFroceCount)//达到用户设定的数量 提示并停机
                        {
                            Process.Stop();
                            JzMainSDPositionParas.SaveRecord();
                            StopAllProcess("FULL");
                            OnTrigger(ActionEnum.ACT_USER_FULL_NG, "");
                        }
                        else if (MOTOR_REGION_PASS.PositionNow <= 0 || MOTOR_REGION_PASS.IsReachHomeBound)//硬件达到满盒 提示并停机
                        {
                            Process.Stop();
                            JzMainSDPositionParas.SaveRecord();
                            StopAllProcess("FULL");
                            OnTrigger(ActionEnum.ACT_SENSOR_FULL_PASS, "");
                        }
                        else if (MOTOR_REGION_NG.PositionNow <= 0 || MOTOR_REGION_NG.IsReachHomeBound)//硬件达到满盒 提示并停机
                        {
                            Process.Stop();
                            JzMainSDPositionParas.SaveRecord();
                            StopAllProcess("FULL");
                            OnTrigger(ActionEnum.ACT_SENSOR_FULL_NG, "");
                        }
                        else
                        {
                            Process.NextDuriation = iNextDurtime[2];
                            Process.ID = 10;
                        }

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            //if (!MACHINE.PLCIO.TAKE_PASS && !MACHINE.PLCIO.TAKE_NG)
                            //if (!MACHINE.PLCIO.TAKE_PASS && !MACHINE.PLCIO.TAKE_NG && !MACHINE.PLCIO.TAKE_YMoving)
                            {
                                if (!MACHINE.PLCIO.SingleTAKE.IsRunning)
                                    MACHINE.PLCIO.SingleTAKE.Start = true;

                                Process.NextDuriation = iNextDurtime[2];
                                Process.ID = 1510;
                            }
                        }
                        break;

                    case 1510:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.PLCIO.TAKE_PLC_GETIMAGEONOFF)
                            {

                                if (IsJudgeFirstTake)
                                    IsJudgeFirstTake = false;

                                switch (Process.RelateString)
                                {
                                    case "COUNT"://计数

                                        if (JzMainSDPositionParas.INSPECT_RESULT)
                                        {
                                            JzMainSDPositionParas.INSPECT_PASSINDEX++;
                                            MACHINE.ContinueNGIndex = 0;//测到PASS 将NG的序号置0
                                        }
                                        else
                                        {
                                            JzMainSDPositionParas.INSPECT_NGINDEX++;
                                            MACHINE.ContinueNGIndex++;

                                            JzMainSDPositionParas.ReportAUTOSave(JzMainSDPositionParas.INSPECT_NGINDEX, false);
                                        }

                                        JzMainSDPositionParas.SaveRecord();

                                        break;
                                }

                                m_GetImageProcess.Start();

                                Process.NextDuriation = iNextDurtime[2];
                                Process.ID = 1520;
                            }
                        }
                        break;
                    case 1520:
                        if (Process.IsTimeup)
                        {
                            if (!m_GetImageProcess.IsOn)
                            {
                                Process.NextDuriation = 0;
                                Process.ID = 15;
                            }
                        }
                        break;

                    case 15:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.PLCIO.SingleTAKE.IsComplete)
                            {

                                MACHINE.PLCIO.TAKE_PLC_GETIMAGEONOFF = false;

                                Process.Stop();
                            }
                        }
                        break;
                }
            }
        }
        private void TakeTickBAK()
        {
            ProcessClass Process = m_TakeProcess;
            iNextDurtime[2] = 200;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        if (MACHINE.ContinueNGIndex >= INI.CONTINUE_NG_COUNT)
                        {
                            Process.Stop();
                            JzMainSDPositionParas.SaveRecord();
                            StopAllProcess("FULL");
                            OnTrigger(ActionEnum.ACT_CONTINUE_COUNT, "");
                        }
                        else if (JzMainSDPositionParas.INSPECT_PASSINDEX >= INI.USER_SET_FULL_PASSCOUNT && !MACHINE.IsUserFroceCount)//达到用户设定的数量 提示并停机
                        {
                            Process.Stop();
                            JzMainSDPositionParas.SaveRecord();
                            StopAllProcess("FULL");
                            OnTrigger(ActionEnum.ACT_USER_FULL_PASS, "");
                        }
                        else if (JzMainSDPositionParas.INSPECT_NGINDEX >= INI.USER_SET_FULL_NGCOUNT && !MACHINE.IsUserFroceCount)//达到用户设定的数量 提示并停机
                        {
                            Process.Stop();
                            JzMainSDPositionParas.SaveRecord();
                            StopAllProcess("FULL");
                            OnTrigger(ActionEnum.ACT_USER_FULL_NG, "");
                        }
                        else if (MOTOR_REGION_PASS.PositionNow <= 0 || MOTOR_REGION_PASS.IsReachHomeBound)//硬件达到满盒 提示并停机
                        {
                            Process.Stop();
                            JzMainSDPositionParas.SaveRecord();
                            StopAllProcess("FULL");
                            OnTrigger(ActionEnum.ACT_SENSOR_FULL_PASS, "");
                        }
                        else if (MOTOR_REGION_NG.PositionNow <= 0 || MOTOR_REGION_NG.IsReachHomeBound)//硬件达到满盒 提示并停机
                        {
                            Process.Stop();
                            JzMainSDPositionParas.SaveRecord();
                            StopAllProcess("FULL");
                            OnTrigger(ActionEnum.ACT_SENSOR_FULL_NG, "");
                        }
                        else
                        {
                            Process.NextDuriation = iNextDurtime[2];
                            Process.ID = 10;
                        }

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            //if (!MACHINE.PLCIO.TAKE_PASS && !MACHINE.PLCIO.TAKE_NG)
                            //if (!MACHINE.PLCIO.TAKE_PASS && !MACHINE.PLCIO.TAKE_NG && !MACHINE.PLCIO.TAKE_YMoving)
                            {
                                if (!MACHINE.PLCIO.SingleTAKE.IsRunning)
                                    MACHINE.PLCIO.SingleTAKE.Start = true;

                                Process.NextDuriation = iNextDurtime[2];
                                Process.ID = 15;
                            }
                        }
                        break;
                    case 15:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.PLCIO.SingleTAKE.IsComplete)
                            {
                                switch (Process.RelateString)
                                {
                                    case "COUNT"://计数

                                        if (JzMainSDPositionParas.INSPECT_RESULT)
                                        {
                                            JzMainSDPositionParas.INSPECT_PASSINDEX++;
                                            MACHINE.ContinueNGIndex = 0;//测到PASS 将NG的序号置0
                                        }
                                        else
                                        {
                                            JzMainSDPositionParas.INSPECT_NGINDEX++;
                                            MACHINE.ContinueNGIndex++;
                                        }

                                        JzMainSDPositionParas.SaveRecord();

                                        break;
                                }

                                Process.Stop();
                            }
                        }
                        break;
                }
            }
        }
        */
        #endregion

        /// <summary>
        /// 叫的第几次
        /// </summary>
        int m_BuzzerIndex = 0;
        /// <summary>
        /// 叫几声
        /// </summary>
        int m_BuzzerCount = 3;

        /// <summary>
        /// 蜂鸣器叫几声流程
        /// </summary>
        ProcessClass m_BuzzerProcess = new ProcessClass();
        private void BuzzerTick()
        {
            ProcessClass Process = m_BuzzerProcess;
            //iNextDurtime[3] = 1000;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        Process.NextDuriation = 100;
                        Process.ID = 10;

                        switch (Process.RelateString)
                        {
                            default:
                                m_BuzzerIndex = 0;
                                m_BuzzerCount = 3;
                                break;
                        }

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if(m_BuzzerIndex < m_BuzzerCount)
                            {
                                MACHINE.PLCIO.ADR_BUZZER = true;

                                Process.NextDuriation = 500;
                                Process.ID = 15;

                                m_BuzzerIndex++;
                            }
                            else
                            {
                                MACHINE.PLCIO.ADR_BUZZER = false;
                                Process.Stop();
                            }
                        }
                        break;
                    case 15:
                        if (Process.IsTimeup)
                        {
                            MACHINE.PLCIO.ADR_BUZZER = false;

                            Process.NextDuriation = 500;
                            Process.ID = 10;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 拍照流程
        /// </summary>
        ProcessClass m_GetImageProcess = new ProcessClass();
        private void GetImageTick()
        {
            ProcessClass Process = m_GetImageProcess;
            //iNextDurtime[3] = 1000;

            if (Process.IsOn)
            {
                if (MACHINE.PLCIO.ADR_ISEMC)
                {
                    Process.Stop();
                }

                switch (Process.ID)
                {
                    case 5:

                        Process.NextDuriation = INI.MAINSD_GETIMAGE_DELAYTIME;
                        Process.ID = 10;

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            CCDCollection.GetImage();
                            Process.Stop();
                        }
                        break;
                }
            }
        }


        /// <summary>
        /// 停掉放料载入载出流程
        /// </summary>
        void StopRegionProcess()
        {
            //m_TakePASSloadprocess.Stop();
            //m_TakeNGloadprocess.Stop();
            //m_TakePASSUnloadprocess.Stop();
            //m_TakeNGUnloadprocess.Stop();


            //MOTOR_REGION_PASS.Stop();
            //MOTOR_REGION_NG.Stop();

            int i = 0;
            while (i < (int)RegionEnum.COUNT)
            {
                regionUI[i].StopRegionProcess();
                i++;
            }
        }

        PLCMotionClass MOTOR_REGION_PASS
        {
            get { return MACHINE.PLCMOTIONCollection[(int)MotionEnum.M6]; }
        }
        PLCMotionClass MOTOR_REGION_NG
        {
            get { return MACHINE.PLCMOTIONCollection[(int)MotionEnum.M7]; }
        }

        void RegionTick()
        {
            TakePASSUnLoadTick();
            TakePASSLoadTick();

            TakeNGUnLoadTick();
            TakeNGLoadTick();
        }

        ProcessClass m_TakePASSloadprocess = new ProcessClass();
        private void TakePASSLoadTick()
        {
            ProcessClass Process = m_TakePASSloadprocess;

            if (Process.IsOn)
            {
                if (MACHINE.PLCIO.ADR_DOOR)
                {
                    Process.Stop();
                    MOTOR_REGION_PASS.Stop();
                }

                switch (Process.ID)
                {
                    case 5:
                        m_TakePASSUnloadprocess.Stop();
                        //MOTOR_REGION_PASS.Go(JzMainSDPositionParas.TAKE_Z1POS1);
                        MOTOR_REGION_PASS.Forward();
                        Process.NextDuriation = 200;
                        Process.ID = 10;

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if(MACHINE.PLCIO.TAKEPASS_IsHaveProductUp)
                            //if (MOTOR_REGION_PASS.IsReachUpperBound)
                            //if (IsInRange(MOTOR_REGION_PASS.PositionNow, JzMainSDPositionParas.TAKE_Z1POS1, 0.5f) && MOTOR_REGION_PASS.IsOnSite)
                            {
                                Process.Stop();
                                MOTOR_REGION_PASS.Stop();
                            }
                        }
                        break;
                }
            }
        }
        ProcessClass m_TakeNGloadprocess = new ProcessClass();
        private void TakeNGLoadTick()
        {
            ProcessClass Process = m_TakeNGloadprocess;

            if (Process.IsOn)
            {
                if (MACHINE.PLCIO.ADR_DOOR)
                {
                    Process.Stop();
                    MOTOR_REGION_NG.Stop();
                }
                switch (Process.ID)
                {
                    case 5:
                        m_TakeNGUnloadprocess.Stop();
                        //MOTOR_REGION_NG.Go(JzMainSDPositionParas.TAKE_Z2POS1);
                        MOTOR_REGION_NG.Forward();
                        Process.NextDuriation = 200;
                        Process.ID = 10;

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if(MACHINE.PLCIO.TAKENG_IsHaveProductUp)
                            //if (MOTOR_REGION_NG.IsReachUpperBound)
                            //if (IsInRange(MOTOR_REGION_NG.PositionNow, JzMainSDPositionParas.TAKE_Z2POS1, 0.5f) && MOTOR_REGION_NG.IsOnSite)
                            {
                                Process.Stop();
                                MOTOR_REGION_NG.Stop();
                            }
                        }
                        break;
                }
            }
        }


        ProcessClass m_TakePASSUnloadprocess = new ProcessClass();
        private void TakePASSUnLoadTick()
        {
            ProcessClass Process = m_TakePASSUnloadprocess;

            if (Process.IsOn)
            {
                if (MACHINE.PLCIO.ADR_DOOR)
                {
                    Process.Stop();
                    MOTOR_REGION_PASS.Stop();
                }
                switch (Process.ID)
                {
                    case 5:

                        m_TakePASSloadprocess.Stop();
                        MOTOR_REGION_PASS.Backward();
                        //MOTOR_REGION_PASS.Go(0);

                        Process.NextDuriation = 200;
                        Process.ID = 10;

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if(MOTOR_REGION_PASS.IsReachHomeBound)
                            //if (IsInRange(MOTOR_REGION_PASS.PositionNow, 0, 0.5f) && MOTOR_REGION_PASS.IsOnSite)
                            //if (MOTOR_REGION_PASS.IsReachLowerBound)
                            {
                                Process.Stop();
                                MOTOR_REGION_PASS.Stop();
                            }
                        }
                        break;
                }
            }
        }
        ProcessClass m_TakeNGUnloadprocess = new ProcessClass();
        private void TakeNGUnLoadTick()
        {
            ProcessClass Process = m_TakeNGUnloadprocess;

            if (Process.IsOn)
            {
                if (MACHINE.PLCIO.ADR_DOOR)
                {
                    Process.Stop();
                    MOTOR_REGION_NG.Stop();
                }
                switch (Process.ID)
                {
                    case 5:

                        m_TakeNGloadprocess.Stop();
                        MOTOR_REGION_NG.Backward();
                        //MOTOR_REGION_NG.Go(0);


                        Process.NextDuriation = 200;
                        Process.ID = 10;

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if(MOTOR_REGION_NG.IsReachHomeBound)
                            //if (MOTOR_REGION_NG.IsReachLowerBound)
                            //if (IsInRange(MOTOR_REGION_NG.PositionNow, 0, 0.5f) && MOTOR_REGION_NG.IsOnSite)
                            {
                                Process.Stop();
                                MOTOR_REGION_NG.Stop();
                            }
                        }
                        break;
                }
            }
        }

        private void UpdateStateUI()
        {

            if (m_TestProcess.IsOn)
                lblState.Text = "执行-测试区取像中 " + m_TestProcess.ID.ToString();
            else if (m_GetImageProcess.IsOn)
                lblState.Text = "取像中 " + m_GetImageProcess.ID.ToString();
            else if (checkbuffprocess.IsOn)
                lblState.Text = "检测BUFF料中 " + checkbuffprocess.ID.ToString();
            else if (m_TakeProcess.IsOn)
                lblState.Text = "执行-放料中 " + m_TakeProcess.ID.ToString();
            else if (m_FeedProcess.IsOn)
                lblState.Text = "执行-供料中 " + m_FeedProcess.ID.ToString();
            else if (m_MainSDProcess.IsOn)
                lblState.Text = "跑线中 " + m_MainSDProcess.ID.ToString();
            else if(resetpartialprocess.IsOn)
                lblState.Text = "小复位中 " + resetpartialprocess.ID.ToString();
            else if (resetprocess.IsOn)
                lblState.Text = "复位中 " + resetprocess.ID.ToString();
            else
                lblState.Text = "待机";

            btnStart.BackColor = (m_MainSDProcess.IsOn ? Color.Red : Color.FromArgb(192, 255, 192));
            btnReset.BackColor = (resetprocess.IsOn ? Color.Red : Color.FromArgb(192, 255, 192));
            btnResetPartial.BackColor = (resetpartialprocess.IsOn ? Color.Red : Color.FromArgb(192, 255, 192));
            lblBUFF.BackColor = (myTestRegionBuff ? Color.Green : Color.Black);

            if (MACHINE.PLCIO.ADR_ISEMC)
            {
                lblState.Text = "急停中";
                lblState.BackColor = Color.Red;
            }
            else if (MACHINE.PLCIO.ADR_DOOR)
            {
                lblState.Text = "门被打开";
                lblState.BackColor = Color.Red;
            }
            else
            {
                lblState.BackColor = Color.Black;
            }

            //if (lblState.Text != "待机")
            //{
            //    c = (c == Color.Green ? Color.Black : Color.Green);
            //    lblState.BackColor = c;
            //}
        }
        //Color c = Color.Green;


        private void log(string elogStr)
        {
            CommonLogClass.Instance.LogMessage(elogStr);
        }

        #endregion


        public delegate void TriggerHandler(ActionEnum action, string opstr);
        public event TriggerHandler TriggerAction;
        public void OnTrigger(ActionEnum action, string opstr)
        {
            if (TriggerAction != null)
            {
                TriggerAction(action, opstr);
            }
        }

        private void btnTestSIM_Click(object sender, EventArgs e)
        {
            MACHINE.EVENT.GenEvent("A0001", EventActionTypeEnum.AUTOMATIC, "sim test alarm", ACCDB.DataNow);
        }
    }
}
