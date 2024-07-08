using Allinone.ControlSpace;
using Allinone.ControlSpace.MachineSpace;
using JetEazy;
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
using JetEazy.ControlSpace.MotionSpace;
using JetEazy.BasicSpace;
using JetEazy.FormSpace;
using Allinone.ControlSpace.IOSpace;

namespace Allinone.FormSpace
{
    enum TagfrmMotorPosition
    {
        COUNT=12,

        //供料区
        //FEED_COUNT=2,
        FEED_YPOS1=0,
        FEED_YPOS2=1,

        //测试区
        //TEST_COOUNT=4,
        TEST_XPOS1=2,
        TEST_XPOS2=3,
        TEST_ZUPPOS=4,
        TEST_ZDOWNPOS=5,

        //收料区
        //TAKE_COUNT=2,
        TAKE_YPOS1=6,
        TAKE_YPOS2=7,

        //TAKE_PRODUCT_COUNT_USER=8,

        SETUP_VACC_OVERTIME=8,
        TEST_READY_XPOS=9,

        TAKE_Z1POS1=10,
        TAKE_Z2POS1=11,
    }

    enum TagDebug
    {
        COUNT=7,

        /// <summary>
        /// 供料
        /// </summary>
        FEED=0,
        /// <summary>
        /// 单吸左
        /// </summary>
        TEST_ACT1_LEFT=1,
        /// <summary>
        /// 单吸右
        /// </summary>
        TEST_ACT1_RIGHT = 2,
        /// <summary>
        /// 双吸
        /// </summary>
        TEST_ACT1_LEFTRIGHT =3,
        /// <summary>
        /// 放料
        /// </summary>
        TEST_ACT2=4,
        /// <summary>
        /// PASS区
        /// </summary>
        TAKE_PASS=5,
        /// <summary>
        /// NG区
        /// </summary>
        TAKE_NG=6,

    }

    public partial class frmMainSDMotorSetup : Form
    {
        const int AXIS_COUNT = 8;

        JzMainSDPositionParaClass JzMainSDPositionParas
        {
            get { return Universal.JZMAINSDPOSITIONPARA; }
        }

        MachineCollectionClass MACHINECollection
        {
            get
            {
                return Universal.MACHINECollection;
            }
        }

        MessageForm M_WARNING_FRM;

        AxisMotionUI[] AxisMotionControl = new AxisMotionUI[AXIS_COUNT];
        AxisSpeedUI[] AxisSpeedControl = new AxisSpeedUI[AXIS_COUNT];

        VersionEnum VERSION = VersionEnum.ALLINONE;
        OptionEnum OPTION = OptionEnum.MAIN_SD;
        JzMainSDMachineClass MACHINE
        {
            get { return (JzMainSDMachineClass)MACHINECollection.MACHINE; }
        }

        PLCMotionClass MOTOR_FEED_Y
        {
            get { return MACHINE.PLCMOTIONCollection[(int)MotionEnum.M0]; }
        }
        PLCMotionClass MOTOR_TEST_X
        {
            get { return MACHINE.PLCMOTIONCollection[(int)MotionEnum.M3]; }
        }
        PLCMotionClass MOTOR_TEST_Z
        {
            get { return MACHINE.PLCMOTIONCollection[(int)MotionEnum.M4]; }
        }
        PLCMotionClass MOTOR_TAKE_Y
        {
            get { return MACHINE.PLCMOTIONCollection[(int)MotionEnum.M5]; }
        }

        PLCMotionClass MOTOR_REGION_PASS
        {
            get { return MACHINE.PLCMOTIONCollection[(int)MotionEnum.M6]; }
        }
        PLCMotionClass MOTOR_REGION_NG
        {
            get { return MACHINE.PLCMOTIONCollection[(int)MotionEnum.M7]; }
        }

        Timer mMotorTimer = null;

        TextBox[] txtPos = new TextBox[(int)TagfrmMotorPosition.COUNT];
        Button[] btnSet=new Button[(int)TagfrmMotorPosition.COUNT];
        Button[] btnGo=new Button[(int)TagfrmMotorPosition.COUNT];
        Button[] btnDebug = new Button[(int)TagDebug.COUNT];

        Button btnRestoreData;
        Button btnReadData;
        Button btnWriteData;
        Button btnTopmost;
        Button btnOK;
        Button btnCancel;

        public frmMainSDMotorSetup()
        {
            InitializeComponent();

            this.Load += FrmMainSDMotorSetup_Load;
            this.FormClosed += FrmMainSDMotorSetup_FormClosed;
        }

        private void FrmMainSDMotorSetup_FormClosed(object sender, FormClosedEventArgs e)
        {
            MACHINE.PLCReadCmdNormalTemp(false);
            Universal.IsOpenMotorWindows = false;
        }

        private void FrmMainSDMotorSetup_Load(object sender, EventArgs e)
        {
            //MACHINE.PLCReadCmdNormalTemp(true);
            //Universal.IsOpenMotorWindows = true;
            Init();
        }

        void Init()
        {
            this.Text = "马达位置设置窗口";

            //JzMainSDPositionParas = new JzMainSDPositionParaClass(Universal.WORKPATH + "\\pos\\");
            //JzMainSDPositionParas.Initial();

            JzMainSDPositionParas.Load();

            #region 位置设定控件

            btnReadData = button17;
            btnWriteData = button18;
            btnTopmost = button21;
            btnOK = button20;
            btnCancel = button19;
            btnRestoreData = button22;

            btnReadData.Click += BtnReadData_Click;
            btnWriteData.Click += BtnWriteData_Click;
            btnTopmost.Click += BtnTopmost_Click;
            btnOK.Click += BtnOK_Click;
            btnCancel.Click += BtnCancel_Click;
            btnRestoreData.Click += BtnRestoreData_Click;

            txtPos[(int)TagfrmMotorPosition.FEED_YPOS1] = textBox1;
            txtPos[(int)TagfrmMotorPosition.FEED_YPOS2] = textBox2;
            txtPos[(int)TagfrmMotorPosition.TEST_XPOS1] = textBox6;
            txtPos[(int)TagfrmMotorPosition.TEST_XPOS2] = textBox5;
            txtPos[(int)TagfrmMotorPosition.TEST_ZUPPOS] = textBox7;
            txtPos[(int)TagfrmMotorPosition.TEST_ZDOWNPOS] = textBox8;
            txtPos[(int)TagfrmMotorPosition.TAKE_YPOS1] = textBox4;
            txtPos[(int)TagfrmMotorPosition.TAKE_YPOS2] = textBox3;
            txtPos[(int)TagfrmMotorPosition.SETUP_VACC_OVERTIME] = textBox9;
            txtPos[(int)TagfrmMotorPosition.TEST_READY_XPOS] = textBox10;

            txtPos[(int)TagfrmMotorPosition.TAKE_Z1POS1] = textBox11;
            txtPos[(int)TagfrmMotorPosition.TAKE_Z2POS1] = textBox12;

            btnSet[(int)TagfrmMotorPosition.FEED_YPOS1] = button1;
            btnSet[(int)TagfrmMotorPosition.FEED_YPOS2] = button4;
            btnSet[(int)TagfrmMotorPosition.TEST_XPOS1] = button12;
            btnSet[(int)TagfrmMotorPosition.TEST_XPOS2] = button10;
            btnSet[(int)TagfrmMotorPosition.TEST_ZUPPOS] = button14;
            btnSet[(int)TagfrmMotorPosition.TEST_ZDOWNPOS] = button16;
            btnSet[(int)TagfrmMotorPosition.TAKE_YPOS1] = button8;
            btnSet[(int)TagfrmMotorPosition.TAKE_YPOS2] = button6;
            btnSet[(int)TagfrmMotorPosition.SETUP_VACC_OVERTIME] = button31;
            btnSet[(int)TagfrmMotorPosition.TEST_READY_XPOS] = button33;

            btnSet[(int)TagfrmMotorPosition.TAKE_Z1POS1] = button35;
            btnSet[(int)TagfrmMotorPosition.TAKE_Z2POS1] = button37;

            btnGo[(int)TagfrmMotorPosition.FEED_YPOS1] = button2;
            btnGo[(int)TagfrmMotorPosition.FEED_YPOS2] = button3;
            btnGo[(int)TagfrmMotorPosition.TEST_XPOS1] = button11;
            btnGo[(int)TagfrmMotorPosition.TEST_XPOS2] = button9;
            btnGo[(int)TagfrmMotorPosition.TEST_ZUPPOS] = button13;
            btnGo[(int)TagfrmMotorPosition.TEST_ZDOWNPOS] = button15;
            btnGo[(int)TagfrmMotorPosition.TAKE_YPOS1] = button7;
            btnGo[(int)TagfrmMotorPosition.TAKE_YPOS2] = button5;
            btnGo[(int)TagfrmMotorPosition.SETUP_VACC_OVERTIME] = button30;
            btnGo[(int)TagfrmMotorPosition.TEST_READY_XPOS] = button32;

            btnGo[(int)TagfrmMotorPosition.TAKE_Z1POS1] = button34;
            btnGo[(int)TagfrmMotorPosition.TAKE_Z2POS1] = button36;

            btnDebug[(int)TagDebug.FEED] = button23;
            btnDebug[(int)TagDebug.TEST_ACT1_LEFT] = button24;
            btnDebug[(int)TagDebug.TEST_ACT2] = button25;
            btnDebug[(int)TagDebug.TAKE_PASS] = button26;
            btnDebug[(int)TagDebug.TAKE_NG] = button27;
            btnDebug[(int)TagDebug.TEST_ACT1_LEFTRIGHT] = button28;
            btnDebug[(int)TagDebug.TEST_ACT1_RIGHT] = button29;

            int i = 0;
            while(i<(int)TagfrmMotorPosition.COUNT)
            {

                if (i == 8)
                    txtPos[i].ReadOnly = false;
                else
                    txtPos[i].ReadOnly = true;

                btnSet[i].Tag = (TagfrmMotorPosition)i;
                btnGo[i].Tag = (TagfrmMotorPosition)i;
                btnSet[i].Click += btnSet_Click;
                btnGo[i].Click += btnGo_Click;

                i++;
            }

            i = 0;
            while (i < (int)TagDebug.COUNT)
            {
                btnDebug[i].Tag = (TagDebug)i;
                btnDebug[i].Click += btnDebug_Click;

                m_singleProcess[i] = new ProcessClass();

                i++;
            }

            //m_ExecuteSingle[(int)TagDebug.FEED] = MACHINE.PLCIO.SingleFEED;
            //m_ExecuteSingle[(int)TagDebug.TEST_ACT1_LEFT] = MACHINE.PLCIO.SingleTESTLEFT;
            //m_ExecuteSingle[(int)TagDebug.TEST_ACT1_LEFTRIGHT] = MACHINE.PLCIO.SingleTESTLEFTRIGHT;
            //m_ExecuteSingle[(int)TagDebug.TEST_ACT2] = MACHINE.PLCIO.SingleTAKE;


            AxisSpeedControl[0] = axisSpeedUI1;
            AxisSpeedControl[1] = axisSpeedUI2;
            AxisSpeedControl[2] = axisSpeedUI3;
            AxisSpeedControl[3] = axisSpeedUI4;
            AxisSpeedControl[4] = axisSpeedUI5;
            AxisSpeedControl[5] = axisSpeedUI6;
            AxisSpeedControl[6] = axisSpeedUI7;
            AxisSpeedControl[7] = axisSpeedUI8;
            //AxisSpeedControl[8] = axisSpeedUI9;

            AxisMotionControl[0] = axisMotionUI1;
            AxisMotionControl[1] = axisMotionUI2;
            AxisMotionControl[2] = axisMotionUI3;
            AxisMotionControl[3] = axisMotionUI4;
            AxisMotionControl[4] = axisMotionUI5;
            AxisMotionControl[5] = axisMotionUI6;
            AxisMotionControl[6] = axisMotionUI7;
            AxisMotionControl[7] = axisMotionUI8;
            //AxisMotionControl[8] = axisMotionUI9;

            ////马达控制
            //AxisMotionControl[(int)MotionEnum.M0].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M0], VERSION, OPTION);
            //AxisMotionControl[(int)MotionEnum.M1].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M1], VERSION, OPTION);
            //AxisMotionControl[(int)MotionEnum.M2].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M2], VERSION, OPTION);

            //AxisMotionControl[(int)MotionEnum.M3].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M3], VERSION, OPTION);
            //AxisMotionControl[(int)MotionEnum.M4].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M4], VERSION, OPTION);
            //AxisMotionControl[(int)MotionEnum.M5].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M5], VERSION, OPTION);

            //AxisMotionControl[(int)MotionEnum.M6].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M6], VERSION, OPTION);
            //AxisMotionControl[(int)MotionEnum.M7].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M7], VERSION, OPTION);
            ////AxisMotionControl[(int)MotionEnum.M8].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M8], VERSION, OPTION);

            ////速度
            //AxisSpeedControl[(int)MotionEnum.M0].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M0], VERSION, OPTION);
            //AxisSpeedControl[(int)MotionEnum.M1].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M1], VERSION, OPTION);
            //AxisSpeedControl[(int)MotionEnum.M2].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M2], VERSION, OPTION);

            //AxisSpeedControl[(int)MotionEnum.M3].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M3], VERSION, OPTION);
            //AxisSpeedControl[(int)MotionEnum.M4].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M4], VERSION, OPTION);
            //AxisSpeedControl[(int)MotionEnum.M5].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M5], VERSION, OPTION);

            //AxisSpeedControl[(int)MotionEnum.M6].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M6], VERSION, OPTION);
            //AxisSpeedControl[(int)MotionEnum.M7].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M7], VERSION, OPTION);
            ////AxisSpeedControl[(int)MotionEnum.M8].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M8], VERSION, OPTION);

            i = 0;
            while (i < AXIS_COUNT)
            {
                AxisMotionControl[i].Initial(MACHINE.PLCMOTIONCollection[i], VERSION, OPTION);

                AxisSpeedControl[i].Initial(MACHINE.PLCMOTIONCollection[i], VERSION, OPTION);
                AxisSpeedControl[i].Tag = (MotionEnum)i;
                AxisSpeedControl[i].TriggerAction += AxisSpeedControl_TriggerAction;

                i++;
            }

            #endregion

            mMotorTimer = new Timer();
            mMotorTimer.Interval = 50;
            mMotorTimer.Enabled = true;
            mMotorTimer.Tick += MMotorTimer_Tick;

            switch (OPTION)
            {
                case OptionEnum.MAIN_SD:
                    LanguageExClass.Instance.EnumControls(this);
                    break;
                case OptionEnum.MAIN_X6:
                    break;
            }

            FillDisplay();
        }

        private void btnDebug_Click(object sender, EventArgs e)
        {
            M_WARNING_FRM = new MessageForm(true, "是否要执行?");
            if (DialogResult.Yes != M_WARNING_FRM.ShowDialog())
            {
                return;
            }
            //M_WARNING_FRM.Close();
            //M_WARNING_FRM.Dispose();

            Button btn = (Button)sender;
            TagDebug _tag = (TagDebug)btn.Tag;

            //executeSingle = null;
            //executeProcess = new ProcessClass();

            switch (_tag)
            {
                case TagDebug.FEED:
                    //executeProcess = m_singleProcess[(int)TagDebug.FEED];
                    //executeSingle = MACHINE.PLCIO.SingleFEED;
                    if (!MACHINE.PLCIO.SingleFEED.IsRunning)
                        MACHINE.PLCIO.SingleFEED.Start = true;
                    else
                        MACHINE.PLCIO.SingleFEED.Stop = true;

                    //if (!m_singleProcess[(int)TagDebug.FEED].IsOn)
                    //    m_singleProcess[(int)TagDebug.FEED].Start();
                    //else
                    //    m_singleProcess[(int)TagDebug.FEED].Stop();

                    break;
                case TagDebug.TEST_ACT1_LEFT:
                    //executeProcess = m_singleProcess[(int)TagDebug.TEST_ACT1_LEFT];
                    //executeSingle = MACHINE.PLCIO.SingleTESTLEFT;
                    //MACHINE.PLCIO.SingleTESTLEFT.Start = true;

                    if (!MACHINE.PLCIO.SingleTESTLEFT.IsRunning)
                        MACHINE.PLCIO.SingleTESTLEFT.Start = true;
                    else
                        MACHINE.PLCIO.SingleTESTLEFT.Stop = true;

                    //if (!m_singleProcess[(int)TagDebug.TEST_ACT1_LEFT].IsOn)
                    //    m_singleProcess[(int)TagDebug.TEST_ACT1_LEFT].Start();
                    //else
                    //    m_singleProcess[(int)TagDebug.TEST_ACT1_LEFT].Stop();
                    break;
                case TagDebug.TEST_ACT1_RIGHT:
                  
                    if (!MACHINE.PLCIO.SingleTESTRIGHT.IsRunning)
                        MACHINE.PLCIO.SingleTESTRIGHT.Start = true;
                    else
                        MACHINE.PLCIO.SingleTESTRIGHT.Stop = true;

                   
                    break;
                case TagDebug.TEST_ACT1_LEFTRIGHT:
                    //executeProcess = m_singleProcess[(int)TagDebug.TEST_ACT1_LEFTRIGHT];
                    //executeSingle = MACHINE.PLCIO.SingleTESTLEFTRIGHT;
                    //MACHINE.PLCIO.SingleTESTLEFTRIGHT.Start = true;

                    if (!MACHINE.PLCIO.SingleTESTLEFTRIGHT.IsRunning)
                        MACHINE.PLCIO.SingleTESTLEFTRIGHT.Start = true;
                    else
                        MACHINE.PLCIO.SingleTESTLEFTRIGHT.Stop = true;

                    //if (!m_singleProcess[(int)TagDebug.TEST_ACT1_LEFTRIGHT].IsOn)
                    //    m_singleProcess[(int)TagDebug.TEST_ACT1_LEFTRIGHT].Start();
                    //else
                    //    m_singleProcess[(int)TagDebug.TEST_ACT1_LEFTRIGHT].Stop();
                    break;
                case TagDebug.TEST_ACT2:
                    //executeProcess = m_singleProcess[(int)TagDebug.TEST_ACT2];
                    //executeSingle = MACHINE.PLCIO.SingleTAKE;
                    //MACHINE.PLCIO.SingleTAKE.Start = true;

                    if (!MACHINE.PLCIO.SingleTAKE.IsRunning)
                        MACHINE.PLCIO.SingleTAKE.Start = true;
                    else
                        MACHINE.PLCIO.SingleTAKE.Stop = true;

                    //if (!m_singleProcess[(int)TagDebug.TEST_ACT2].IsOn)
                    //    m_singleProcess[(int)TagDebug.TEST_ACT2].Start();
                    //else
                    //    m_singleProcess[(int)TagDebug.TEST_ACT2].Stop();
                    break;
                case TagDebug.TAKE_PASS:
                    //executeProcess = m_singleProcess[(int)TagDebug.TAKE_PASS];
                    //executeSingle = MACHINE.PLCIO.TAKE_PASS;
                    MACHINE.PLCIO.TAKE_PASS = !MACHINE.PLCIO.TAKE_PASS;

                    
                    break;
                case TagDebug.TAKE_NG:
                    //executeProcess = m_singleProcess[(int)TagDebug.TAKE_NG];
                    //executeSingle = MACHINE.PLCIO.SingleFEED;
                    MACHINE.PLCIO.TAKE_NG = !MACHINE.PLCIO.TAKE_NG;
                    break;
            }

            //if (!executeProcess.IsOn)
            //    executeProcess.Start();
            //else
            //    executeProcess.Stop();

        }

        private void AxisSpeedControl_TriggerAction(TagMotorSpeed action, string opstr)
        {
            WriteParasToPLC();
        }

        private void BtnRestoreData_Click(object sender, EventArgs e)
        {
            M_WARNING_FRM = new MessageForm(true, "是否复原数据?");
            if (DialogResult.Yes != M_WARNING_FRM.ShowDialog())
            {
                return;
            }
            //M_WARNING_FRM.Close();
            //M_WARNING_FRM.Dispose();

            JzMainSDPositionParas.Load();
            FillDisplay();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            M_WARNING_FRM = new MessageForm(true, "是否定位?");
            if (DialogResult.Yes != M_WARNING_FRM.ShowDialog())
            {
                return;
            }
            //M_WARNING_FRM.Close();
            //M_WARNING_FRM.Dispose();

            Button btn = (Button)sender;
            TagfrmMotorPosition _tag = (TagfrmMotorPosition)btn.Tag;
            switch (_tag)
            {
                case TagfrmMotorPosition.FEED_YPOS1:
                    MOTOR_FEED_Y.Go(JzMainSDPositionParas.FEED_YPOS1);
                    break;
                case TagfrmMotorPosition.FEED_YPOS2:
                    MOTOR_FEED_Y.Go(JzMainSDPositionParas.FEED_YPOS2);
                    break;
                case TagfrmMotorPosition.TEST_XPOS1:
                    MOTOR_TEST_X.Go(JzMainSDPositionParas.TEST_XPOS1);
                    break;
                case TagfrmMotorPosition.TEST_XPOS2:
                    MOTOR_TEST_X.Go(JzMainSDPositionParas.TEST_XPOS2);
                    break;
                case TagfrmMotorPosition.TEST_ZUPPOS:
                    MOTOR_TEST_Z.Go(JzMainSDPositionParas.TEST_ZUPPOS);
                    break;
                case TagfrmMotorPosition.TEST_ZDOWNPOS:
                    MOTOR_TEST_Z.Go(JzMainSDPositionParas.TEST_ZDOWNPOS);
                    break;
                case TagfrmMotorPosition.TAKE_YPOS1:
                    MOTOR_TAKE_Y.Go(JzMainSDPositionParas.TAKE_YPOS1);
                    break;
                case TagfrmMotorPosition.TAKE_YPOS2:
                    MOTOR_TAKE_Y.Go(JzMainSDPositionParas.TAKE_YPOS2);
                    break;
                case TagfrmMotorPosition.TEST_READY_XPOS:
                    MOTOR_TEST_X.Go(JzMainSDPositionParas.TEST_READY_XPOS);
                    break;
                case TagfrmMotorPosition.TAKE_Z1POS1:
                    MOTOR_REGION_PASS.Go(JzMainSDPositionParas.TAKE_Z1POS1);
                    break;
                case TagfrmMotorPosition.TAKE_Z2POS1:
                    MOTOR_REGION_NG.Go(JzMainSDPositionParas.TAKE_Z2POS1);
                    break;
            }
        }

        private void btnSet_Click(object sender, EventArgs e)
        {

            M_WARNING_FRM = new MessageForm(true, "是否设定?");
            if (DialogResult.Yes != M_WARNING_FRM.ShowDialog())
            {
                return;
            }
            //M_WARNING_FRM.Close();
            //M_WARNING_FRM.Dispose();

            Button btn = (Button)sender;
            TagfrmMotorPosition _tag = (TagfrmMotorPosition)btn.Tag;
            switch (_tag)
            {
                case TagfrmMotorPosition.FEED_YPOS1:
                    JzMainSDPositionParas.FEED_YPOS1 = MOTOR_FEED_Y.PositionNow;
                    break;
                case TagfrmMotorPosition.FEED_YPOS2:
                    JzMainSDPositionParas.FEED_YPOS2 = MOTOR_FEED_Y.PositionNow;
                    break;
                case TagfrmMotorPosition.TEST_XPOS1:
                    JzMainSDPositionParas.TEST_XPOS1 = MOTOR_TEST_X.PositionNow;
                    break;
                case TagfrmMotorPosition.TEST_XPOS2:
                    JzMainSDPositionParas.TEST_XPOS2 = MOTOR_TEST_X.PositionNow;
                    break;
                case TagfrmMotorPosition.TEST_ZUPPOS:
                    JzMainSDPositionParas.TEST_ZUPPOS = MOTOR_TEST_Z.PositionNow;
                    break;
                case TagfrmMotorPosition.TEST_ZDOWNPOS:
                    JzMainSDPositionParas.TEST_ZDOWNPOS = MOTOR_TEST_Z.PositionNow;
                    break;
                case TagfrmMotorPosition.TAKE_YPOS1:
                    JzMainSDPositionParas.TAKE_YPOS1 = MOTOR_TAKE_Y.PositionNow;
                    break;
                case TagfrmMotorPosition.TAKE_YPOS2:
                    JzMainSDPositionParas.TAKE_YPOS2 = MOTOR_TAKE_Y.PositionNow;
                    break;
                case TagfrmMotorPosition.SETUP_VACC_OVERTIME:
                    int itemp = 15;
                    bool bOK = int.TryParse(txtPos[(int)TagfrmMotorPosition.SETUP_VACC_OVERTIME].Text, out itemp);
                    if (bOK)
                        JzMainSDPositionParas.SETUP_VACC_OVERTIME = itemp;
                    break;
                case TagfrmMotorPosition.TEST_READY_XPOS:
                    JzMainSDPositionParas.TEST_READY_XPOS = MOTOR_TEST_X.PositionNow;
                    break;
                case TagfrmMotorPosition.TAKE_Z1POS1:
                    JzMainSDPositionParas.TAKE_Z1POS1 = MOTOR_REGION_PASS.PositionNow;
                    break;
                case TagfrmMotorPosition.TAKE_Z2POS1:
                    JzMainSDPositionParas.TAKE_Z2POS1 = MOTOR_REGION_NG.PositionNow;
                    break;
                //case TagfrmMotorPosition.TAKE_PRODUCT_COUNT_USER:
                //    int itemp = 10;
                //    bool bOK = int.TryParse(txtPos[(int)TagfrmMotorPosition.TAKE_PRODUCT_COUNT_USER].Text, out itemp);
                //    if (bOK)
                //        JzMainSDPositionParas.TAKE_PRODUCT_COUNT_USER = itemp;
                //    break;
            }
            FillDisplay();

        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {

            JzMainSDPositionParas.Load();
            this.Close();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            //保存资料

            int itemp = 15;
            bool bOK = int.TryParse(txtPos[(int)TagfrmMotorPosition.SETUP_VACC_OVERTIME].Text, out itemp);
            if (bOK)
                JzMainSDPositionParas.SETUP_VACC_OVERTIME = itemp;

            WriteToPlc();
            JzMainSDPositionParas.Save();
            this.Close();
        }

        private void BtnTopmost_Click(object sender, EventArgs e)
        {
            this.TopMost = !this.TopMost;
            btnTopmost.BackColor = (this.TopMost ? Color.Green : Control.DefaultBackColor);
        }

        private void BtnWriteData_Click(object sender, EventArgs e)
        {
            M_WARNING_FRM = new MessageForm(true, "是否将当前数据写入PLC?");
            if (DialogResult.Yes != M_WARNING_FRM.ShowDialog())
            {
                return;
            }
            //M_WARNING_FRM.Close();
            //M_WARNING_FRM.Dispose();

            WriteToPlc();
            JzMainSDPositionParas.Save();
            FillDisplay();
        }

        private void BtnReadData_Click(object sender, EventArgs e)
        {
            
        }


        public void FillDisplay()
        {
            txtPos[(int)TagfrmMotorPosition.FEED_YPOS1].Text = JzMainSDPositionParas.FEED_YPOS1.ToString();
            txtPos[(int)TagfrmMotorPosition.FEED_YPOS2].Text = JzMainSDPositionParas.FEED_YPOS2.ToString();
            txtPos[(int)TagfrmMotorPosition.TEST_XPOS1].Text = JzMainSDPositionParas.TEST_XPOS1.ToString();
            txtPos[(int)TagfrmMotorPosition.TEST_XPOS2].Text = JzMainSDPositionParas.TEST_XPOS2.ToString();
            txtPos[(int)TagfrmMotorPosition.TEST_ZUPPOS].Text = JzMainSDPositionParas.TEST_ZUPPOS.ToString();
            txtPos[(int)TagfrmMotorPosition.TEST_ZDOWNPOS].Text = JzMainSDPositionParas.TEST_ZDOWNPOS.ToString();
            txtPos[(int)TagfrmMotorPosition.TAKE_YPOS1].Text = JzMainSDPositionParas.TAKE_YPOS1.ToString();
            txtPos[(int)TagfrmMotorPosition.TAKE_YPOS2].Text = JzMainSDPositionParas.TAKE_YPOS2.ToString();
            txtPos[(int)TagfrmMotorPosition.SETUP_VACC_OVERTIME].Text = JzMainSDPositionParas.SETUP_VACC_OVERTIME.ToString();
            txtPos[(int)TagfrmMotorPosition.TEST_READY_XPOS].Text = JzMainSDPositionParas.TEST_READY_XPOS.ToString();

            txtPos[(int)TagfrmMotorPosition.TAKE_Z1POS1].Text = JzMainSDPositionParas.TAKE_Z1POS1.ToString();
            txtPos[(int)TagfrmMotorPosition.TAKE_Z2POS1].Text = JzMainSDPositionParas.TAKE_Z2POS1.ToString();

            //txtPos[(int)TagfrmMotorPosition.TAKE_PRODUCT_COUNT_USER].Text = JzMainSDPositionParas.TAKE_PRODUCT_COUNT_USER.ToString();
        }
        public void WriteToPlc()
        {

            //return;

            MACHINE.PLCIO.SetValue(JzMainSDPositionParas.FEED_YPOS1, "D0256", "D0257");
            MACHINE.PLCIO.SetValue(JzMainSDPositionParas.FEED_YPOS2, "D0258", "D0259");

            MACHINE.PLCIO.SetValue(JzMainSDPositionParas.TEST_XPOS1, "D0216", "D0217");
            MACHINE.PLCIO.SetValue(JzMainSDPositionParas.TEST_XPOS2, "D0218", "D0219");
            MACHINE.PLCIO.SetValue(JzMainSDPositionParas.TEST_ZUPPOS, "D0236", "D0237");
            MACHINE.PLCIO.SetValue(JzMainSDPositionParas.TEST_ZDOWNPOS, "D0238", "D0239");

            MACHINE.PLCIO.SetValue(JzMainSDPositionParas.TAKE_YPOS1, "D0316", "D0317");
            MACHINE.PLCIO.SetValue(JzMainSDPositionParas.TAKE_YPOS2, "D0318", "D0319");

            MACHINE.PLCIO.SetValue(JzMainSDPositionParas.SETUP_VACC_OVERTIME, "D0370", "");
            MACHINE.PLCIO.SetValue(JzMainSDPositionParas.TEST_READY_XPOS, "D0380", "D0381");

            WriteParasToPLC();
        }

        private void WriteParasToPLC()
        {
            MACHINE.PLCIO.PLCWriteParas = true;
            MACHINE.PLCIO.PLCWriteParas = false;
        }
        private void MMotorTimer_Tick(object sender, EventArgs e)
        {

            //ExecuteTick();

            int i = 0;
            while (i < AXIS_COUNT)
            {
                AxisMotionControl[i].Tick();
                AxisSpeedControl[i].Tick();

                i++;
            }
            Tick();

        }


        #region SINGLE PROCESS

        public void StopSingleProcess()
        {
            ////停止程式流程
            //int i = 0;
            //while (i < (int)TagDebug.COUNT)
            //{
            //    m_singleProcess[i].Stop();
            //    i++;
            //}
            //停止plc流程
            MACHINE.PLCIO.SingleFEED.Stop = true;
            MACHINE.PLCIO.SingleTESTLEFT.Stop = true;
            MACHINE.PLCIO.SingleTESTRIGHT.Stop = true;
            MACHINE.PLCIO.SingleTESTLEFTRIGHT.Stop = true;
            MACHINE.PLCIO.SingleTAKE.Stop = true;
            MACHINE.PLCIO.TAKE_PASS = false;
            MACHINE.PLCIO.TAKE_NG = false;

        }

        private void Tick()
        {
            //FEED_Tick();
            //TEST_ACT1_LEFT_Tick();
            //TEST_ACT1_LEFTRIGHT_Tick();
            //TEST_ACT2_Tick();

            //int i = 0;
            //while (i < (int)TagDebug.COUNT)
            //{

            //    btnDebug[i].BackColor = (m_singleProcess[i].IsOn ? Color.Red : Control.DefaultBackColor);
            //    i++;
            //}

            btnDebug[(int)TagDebug.FEED].BackColor = (MACHINE.PLCIO.SingleFEED.IsRunning ? Color.Lime : Control.DefaultBackColor);
            btnDebug[(int)TagDebug.TEST_ACT1_LEFT].BackColor = (MACHINE.PLCIO.SingleTESTLEFT.IsRunning ? Color.Lime : Control.DefaultBackColor);
            btnDebug[(int)TagDebug.TEST_ACT1_RIGHT].BackColor = (MACHINE.PLCIO.SingleTESTRIGHT.IsRunning ? Color.Lime : Control.DefaultBackColor);
            btnDebug[(int)TagDebug.TEST_ACT1_LEFTRIGHT].BackColor = (MACHINE.PLCIO.SingleTESTLEFTRIGHT.IsRunning ? Color.Lime : Control.DefaultBackColor);
            btnDebug[(int)TagDebug.TEST_ACT2].BackColor = (MACHINE.PLCIO.SingleTAKE.IsRunning ? Color.Lime : Control.DefaultBackColor);
            btnDebug[(int)TagDebug.TAKE_PASS].BackColor = (MACHINE.PLCIO.TAKE_PASS ? Color.Lime : Control.DefaultBackColor);
            btnDebug[(int)TagDebug.TAKE_NG].BackColor = (MACHINE.PLCIO.TAKE_NG ? Color.Lime : Control.DefaultBackColor);

            btnDebug[(int)TagDebug.FEED].Text = (MACHINE.PLCIO.SingleFEED.IsRunning ? "供料ON" : "供料");
            btnDebug[(int)TagDebug.TEST_ACT1_LEFT].Text = (MACHINE.PLCIO.SingleTESTLEFT.IsRunning ? "单吸料(左)ON" : "单吸料(左)");
            btnDebug[(int)TagDebug.TEST_ACT1_RIGHT].Text = (MACHINE.PLCIO.SingleTESTRIGHT.IsRunning ? "单吸料(右)ON" : "单吸料(右)");
            btnDebug[(int)TagDebug.TEST_ACT1_LEFTRIGHT].Text = (MACHINE.PLCIO.SingleTESTLEFTRIGHT.IsRunning ? "双吸料ON" : "双吸料");
            btnDebug[(int)TagDebug.TEST_ACT2].Text = (MACHINE.PLCIO.SingleTAKE.IsRunning ? "放料ON" : "放料");
            btnDebug[(int)TagDebug.TAKE_PASS].Text = (MACHINE.PLCIO.TAKE_PASS ? "PASS区ON" : "PASS区");
            btnDebug[(int)TagDebug.TAKE_NG].Text = (MACHINE.PLCIO.TAKE_NG ? "NG区ON" : "NG区");

        }

        ProcessClass[] m_singleProcess = new ProcessClass[(int)TagDebug.COUNT];
        //MainSDSingleProcess[] m_ExecuteSingle = new MainSDSingleProcess[(int)TagDebug.COUNT];

        //MainSDSingleProcess executeSingle = null;
        //ProcessClass executeProcess = new ProcessClass();
        //private void ExecuteTick()
        //{
        //    ProcessClass Process = executeProcess;

        //    if (Process.IsOn)
        //    {
        //        switch (Process.ID)
        //        {
        //            case 5:

        //                if(executeSingle == null)
        //                {
        //                    Process.Stop();
        //                    return;
        //                }

        //                M_WARNING_FRM = new MessageForm(executeSingle.Name + "中，請稍後...", true);
        //                M_WARNING_FRM.Show();

        //                //MACHINE.PLCIO.RESET = true;
        //                executeSingle.Start = true;

        //                Process.NextDuriation = 1000;
        //                Process.ID = 10;

        //                break;
        //            case 10:
        //                if (Process.IsTimeup)
        //                {
        //                    if (executeSingle.Complete && !executeSingle.Running)
        //                    {
        //                        M_WARNING_FRM.Close();
        //                        M_WARNING_FRM.Dispose();

        //                        Process.Stop();
        //                    }
        //                }
        //                break;
        //        }
        //    }
        //}

        private void FEED_Tick()
        {
            ProcessClass Process = m_singleProcess[(int)TagDebug.FEED];
            MainSDSingleProcess mainSDSingle = MACHINE.PLCIO.SingleFEED;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        if (mainSDSingle == null)
                        {
                            Process.Stop();
                            return;
                        }

                        M_WARNING_FRM = new MessageForm(mainSDSingle.Name + "中，請稍後...", true);
                        M_WARNING_FRM.Show();

                        mainSDSingle.Start = true;

                        Process.NextDuriation = 1000;
                        Process.ID = 10;

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (mainSDSingle.IsComplete && !mainSDSingle.IsRunning)
                            {
                                M_WARNING_FRM.Close();
                                M_WARNING_FRM.Dispose();

                                Process.Stop();
                            }
                        }
                        break;
                }
            }
        }
        private void TEST_ACT1_LEFT_Tick()
        {
            ProcessClass Process = m_singleProcess[(int)TagDebug.TEST_ACT1_LEFT];
            MainSDSingleProcess mainSDSingle = MACHINE.PLCIO.SingleTESTLEFT;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        if (mainSDSingle == null)
                        {
                            Process.Stop();
                            return;
                        }

                        M_WARNING_FRM = new MessageForm(mainSDSingle.Name + "中，請稍後...", true);
                        M_WARNING_FRM.Show();

                        mainSDSingle.Start = true;

                        Process.NextDuriation = 1000;
                        Process.ID = 10;

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (mainSDSingle.IsComplete && !mainSDSingle.IsRunning)
                            {
                                M_WARNING_FRM.Close();
                                M_WARNING_FRM.Dispose();

                                Process.Stop();
                            }
                        }
                        break;
                }
            }
        }
        private void TEST_ACT1_LEFTRIGHT_Tick()
        {
            ProcessClass Process = m_singleProcess[(int)TagDebug.TEST_ACT1_LEFTRIGHT];
            MainSDSingleProcess mainSDSingle = MACHINE.PLCIO.SingleTESTLEFTRIGHT;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        if (mainSDSingle == null)
                        {
                            Process.Stop();
                            return;
                        }

                        M_WARNING_FRM = new MessageForm(mainSDSingle.Name + "中，請稍後...", true);
                        M_WARNING_FRM.Show();

                        mainSDSingle.Start = true;

                        Process.NextDuriation = 1000;
                        Process.ID = 10;

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (mainSDSingle.IsComplete && !mainSDSingle.IsRunning)
                            {
                                M_WARNING_FRM.Close();
                                M_WARNING_FRM.Dispose();

                                Process.Stop();
                            }
                        }
                        break;
                }
            }
        }
        private void TEST_ACT2_Tick()
        {
            ProcessClass Process = m_singleProcess[(int)TagDebug.TEST_ACT2];
            MainSDSingleProcess mainSDSingle = MACHINE.PLCIO.SingleTAKE;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        if (mainSDSingle == null)
                        {
                            Process.Stop();
                            return;
                        }

                        M_WARNING_FRM = new MessageForm(mainSDSingle.Name + "中，請稍後...", true);
                        M_WARNING_FRM.Show();

                        mainSDSingle.Start = true;

                        Process.NextDuriation = 1000;
                        Process.ID = 10;

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (mainSDSingle.IsComplete && !mainSDSingle.IsRunning)
                            {
                                M_WARNING_FRM.Close();
                                M_WARNING_FRM.Dispose();

                                Process.Stop();
                            }
                        }
                        break;
                }
            }
        }

        #endregion

    }

}
