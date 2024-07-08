using Allinone.ControlSpace.MachineSpace;
using JetEazy;
using JetEazy.BasicSpace;
using JetEazy.ControlSpace.MotionSpace;
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
    public enum RegionEnum : int
    {
        COUNT=4,

        FEED_Z1=0,
        FEED_Z2=1,
        TAKE_PASS=2,
        TAKE_NG=3,
    }

    public partial class MainSDOpUI : UserControl
    {
        Label lblRegionUpper;
        Label lblRegionProductUpper;
        Label lblRegionProductIsHave;
        Label lblRegionProductCount;
        Label lblRegionUserFull;
        Label lblRegionSensorFull;
        Label lblRegionLower;

        Button btnLoad;
        Button btnUnLoad;
        Button btnClear;
        Button btnLook;

        GroupBox grpControl;

        //myVerticalProgressBar myVerticalProgress;

        RegionEnum m_Region = RegionEnum.TAKE_PASS;

        VersionEnum VERSION = VersionEnum.ALLINONE;
        OptionEnum OPTION = OptionEnum.MAIN_SD;
        JzMainSDMachineClass MACHINE;

        MessageForm M_WARNING_FRM;

        JzMainSDPositionParaClass JzMainSDPositionParas
        {
            get { return Universal.JZMAINSDPOSITIONPARA; }
        }


        public MainSDOpUI()
        {
            InitializeComponent();
            InitUI();
        }
        void InitUI()
        {
            lblRegionUpper = label4;
            lblRegionProductUpper = label1;
            lblRegionProductIsHave = label6;
            lblRegionProductCount = label7;
            lblRegionUserFull = label3;
            lblRegionSensorFull = label2;
            lblRegionLower = label5;

            btnLoad = button1;
            btnUnLoad = button2;
            btnClear = button3;
            btnLook = button4;

            //myVerticalProgress = myVerticalProgressBar1;
            grpControl = groupBox1;
        }
        public void SetRegion(RegionEnum eRegion)
        {
            m_Region = eRegion;
        }
        public void Init(VersionEnum version, OptionEnum option, JzMainSDMachineClass machine)
        {
            VERSION = version;
            OPTION = option;
            MACHINE = machine;

            switch (m_Region)
            {
                case RegionEnum.TAKE_PASS:
                    grpControl.Text = "PASS区";
                    //myVerticalProgress.Maximum = 100;

                    //if (JzMainSDPositionParas.INSPECT_PASSINDEX >= myVerticalProgress.Maximum)
                    //{
                    //    myVerticalProgress.Maximum += 100;
                    //}

                    break;
                case RegionEnum.TAKE_NG:
                    grpControl.Text = "NG区";
                    //myVerticalProgress.Maximum = 100;

                    //if (JzMainSDPositionParas.INSPECT_NGINDEX >= myVerticalProgress.Maximum)
                    //{
                    //    myVerticalProgress.Maximum += 100;
                    //}

                    break;
                case RegionEnum.FEED_Z1:
                    grpControl.Text = "供料1区";

                    lblRegionProductCount.Visible = false;
                    lblRegionUserFull.Visible = false;
                    lblRegionSensorFull.Visible = false;

                    btnClear.Visible = false;
                    btnLook.Visible = false;

                    //myVerticalProgress.Visible = false;

                    break;

                case RegionEnum.FEED_Z2:
                    grpControl.Text = "供料2区";

                    lblRegionProductCount.Visible = false;
                    lblRegionUserFull.Visible = false;
                    lblRegionSensorFull.Visible = false;

                    btnClear.Visible = false;
                    btnLook.Visible = false;

                    //myVerticalProgress.Visible = false;

                    break;
               
            }


            btnLoad.Click += BtnLoad_Click;
            btnUnLoad.Click += BtnUnLoad_Click;
            btnClear.Click += BtnClear_Click;
            btnLook.Click += BtnLook_Click;
        }

        private void BtnLook_Click(object sender, EventArgs e)
        {
           
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            switch (m_Region)
            {
                case RegionEnum.TAKE_PASS:

                    M_WARNING_FRM = new MessageForm(true, "是否要复位收料PASS区计数？");
                    if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                    {
                        //JzMainSDPositionParas.INSPECT_PASSINDEX = 0;
                        JzMainSDPositionParas.PassZero();
                        MACHINE.IsUserFroceCount = false;
                    }
                    M_WARNING_FRM.Close();
                    M_WARNING_FRM.Dispose();

                    break;
                case RegionEnum.TAKE_NG:

                    M_WARNING_FRM = new MessageForm(true, "是否要复位收料NG区计数？");
                    if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                    {
                        //JzMainSDPositionParas.INSPECT_NGINDEX = 0;
                        JzMainSDPositionParas.NgZero();
                        MACHINE.IsUserFroceCount = false;

                        M_WARNING_FRM = new MessageForm(true, "是否更换批号？不更换则点取消", JzMainSDPositionParas.Report_LOT, "");
                        if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                        {
                            if (JzMainSDPositionParas.Report_LOT == M_WARNING_FRM.NewLot)
                                JzMainSDPositionParas.Report_LOT = "new_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                            else
                                JzMainSDPositionParas.Report_LOT = M_WARNING_FRM.NewLot;

                            JzMainSDPositionParas.SaveRecord();
                        }

                    }
                    M_WARNING_FRM.Close();
                    M_WARNING_FRM.Dispose();

                    break;
                case RegionEnum.FEED_Z1:
                case RegionEnum.FEED_Z2:
                    break;

            }
        }

        private void BtnUnLoad_Click(object sender, EventArgs e)
        {
            M_WARNING_FRM = new MessageForm(true, "是否启动载出？");
            if (DialogResult.Yes != M_WARNING_FRM.ShowDialog())
            {
                return;
            }
            //M_WARNING_FRM.Close();
            //M_WARNING_FRM.Dispose();

            switch (m_Region)
            {
                case RegionEnum.TAKE_PASS:

                    if (!m_TakePASSUnloadprocess.IsOn)
                        m_TakePASSUnloadprocess.Start();
                    else
                    {
                        m_TakePASSUnloadprocess.Stop();
                        MOTOR_REGION_PASS.Stop();
                    }

                    break;
                case RegionEnum.TAKE_NG:

                    if (!m_TakeNGUnloadprocess.IsOn)
                        m_TakeNGUnloadprocess.Start();
                    else
                    {
                        m_TakeNGUnloadprocess.Stop();
                        MOTOR_REGION_NG.Stop();
                    }

                    break;
                case RegionEnum.FEED_Z1:

                    if (!m_FEEDZ1Unloadprocess.IsOn)
                        m_FEEDZ1Unloadprocess.Start();
                    else
                    {
                        m_FEEDZ1Unloadprocess.Stop();
                        MOTOR_FEED_Z1.Stop();
                    }

                    break;
                case RegionEnum.FEED_Z2:

                    if (!m_FEEDZ2Unloadprocess.IsOn)
                        m_FEEDZ2Unloadprocess.Start();
                    else
                    {
                        m_FEEDZ2Unloadprocess.Stop();
                        MOTOR_FEED_Z2.Stop();
                    }

                    break;

            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            M_WARNING_FRM = new MessageForm(true, "是否启动载入？");
            if (DialogResult.Yes != M_WARNING_FRM.ShowDialog())
            {
                return;
            }
            //M_WARNING_FRM.Close();
            //M_WARNING_FRM.Dispose();

            switch (m_Region)
            {
                case RegionEnum.TAKE_PASS:

                    if (!m_TakePASSloadprocess.IsOn)
                        m_TakePASSloadprocess.Start();
                    else
                    {
                        m_TakePASSloadprocess.Stop();
                        MOTOR_REGION_PASS.Stop();
                    }

                    break;
                case RegionEnum.TAKE_NG:

                    if (!m_TakeNGloadprocess.IsOn)
                        m_TakeNGloadprocess.Start();
                    else
                    {
                        m_TakeNGloadprocess.Stop();
                        MOTOR_REGION_NG.Stop();
                    }

                    break;
                case RegionEnum.FEED_Z1:

                    if (!m_FEEDZ1loadprocess.IsOn)
                        m_FEEDZ1loadprocess.Start();
                    else
                    {
                        m_FEEDZ1loadprocess.Stop();
                        MOTOR_FEED_Z1.Stop();
                    }

                    break;
                case RegionEnum.FEED_Z2:

                    if (!m_FEEDZ2loadprocess.IsOn)
                        m_FEEDZ2loadprocess.Start();
                    else
                    {
                        m_FEEDZ2loadprocess.Stop();
                        MOTOR_FEED_Z2.Stop();
                    }

                    break;

            }
        }

        public void Tick()
        {
            if (MACHINE == null)
                return;

            switch(m_Region)
            {
                case RegionEnum.TAKE_PASS:

                    lblRegionUpper.BackColor = (MACHINE.PLCIO.IsGetBit("", "M0043") ? Color.Red : Color.Black);
                    lblRegionProductUpper.BackColor = (MACHINE.PLCIO.IsGetBit("", "M0018") ? Color.Green : Color.Black);
                    lblRegionProductIsHave.BackColor = (MACHINE.PLCIO.IsGetBit("", "M0017") ? Color.Green : Color.Black);
                    //lblRegionProductCount.BackColor = (MACHINE.PLCIO.IsGetBit("", "") ? Color.Red : Color.Black);
                    lblRegionUserFull.BackColor = (JzMainSDPositionParas.INSPECT_PASSINDEX >= INI.USER_SET_FULL_PASSCOUNT ? Color.Red : Color.Black);
                    lblRegionSensorFull.BackColor = (MACHINE.PLCIO.IsGetBit("", "M0027") ? Color.Red : Color.Black);
                    lblRegionLower.BackColor = (MACHINE.PLCIO.IsGetBit("", "M0044") ? Color.Red : Color.Black);

                    lblRegionProductCount.Text = JzMainSDPositionParas.INSPECT_PASSINDEX.ToString();

                    btnLoad.BackColor = (m_TakePASSloadprocess.IsOn ? Color.Red : Color.FromArgb(128, 255, 128));
                    btnUnLoad.BackColor = (m_TakePASSUnloadprocess.IsOn ? Color.Red : Color.FromArgb(128, 255, 128));

                    //if (JzMainSDPositionParas.INSPECT_PASSINDEX >= myVerticalProgress.Maximum)
                    //{
                    //    myVerticalProgress.Maximum += 100;
                    //}
                    //myVerticalProgress.Value = JzMainSDPositionParas.INSPECT_PASSINDEX;

                    break;
                case RegionEnum.TAKE_NG:

                    lblRegionUpper.BackColor = (MACHINE.PLCIO.IsGetBit("", "M0045") ? Color.Red : Color.Black);
                    lblRegionProductUpper.BackColor = (MACHINE.PLCIO.IsGetBit("", "M0020") ? Color.Green : Color.Black);
                    lblRegionProductIsHave.BackColor = (MACHINE.PLCIO.IsGetBit("", "M0019") ? Color.Green : Color.Black);
                    //lblRegionProductCount.BackColor = (MACHINE.PLCIO.IsGetBit("", "") ? Color.Red : Color.Black);
                    lblRegionUserFull.BackColor = (JzMainSDPositionParas.INSPECT_NGINDEX >= INI.USER_SET_FULL_NGCOUNT ? Color.Red : Color.Black);
                    lblRegionSensorFull.BackColor = (MACHINE.PLCIO.IsGetBit("", "M0028") ? Color.Red : Color.Black);
                    lblRegionLower.BackColor = (MACHINE.PLCIO.IsGetBit("", "M0046") ? Color.Red : Color.Black);

                    lblRegionProductCount.Text = JzMainSDPositionParas.INSPECT_NGINDEX.ToString();

                    btnLoad.BackColor = (m_TakeNGloadprocess.IsOn ? Color.Red : Color.FromArgb(128, 255, 128));
                    btnUnLoad.BackColor = (m_TakeNGUnloadprocess.IsOn ? Color.Red : Color.FromArgb(128, 255, 128));

                    //if (JzMainSDPositionParas.INSPECT_NGINDEX >= myVerticalProgress.Maximum)
                    //{
                    //    myVerticalProgress.Maximum += 100;
                    //}
                    //myVerticalProgress.Value = JzMainSDPositionParas.INSPECT_NGINDEX;

                    break;
                case RegionEnum.FEED_Z1:

                    lblRegionUpper.BackColor = (MACHINE.PLCIO.IsGetBit("", "M0037") ? Color.Red : Color.Black);
                    lblRegionProductUpper.BackColor = (MACHINE.PLCIO.IsGetBit("", "M0014") ? Color.Green : Color.Black);
                    lblRegionProductIsHave.BackColor = (MACHINE.PLCIO.IsGetBit("", "M0013") ? Color.Green : Color.Black);
                    //lblRegionProductCount.BackColor = (MACHINE.PLCIO.IsGetBit("", "") ? Color.Red : Color.Black);
                    //lblRegionUserFull.BackColor = (MACHINE.PLCIO.IsGetBit("", "") ? Color.Red : Color.Black);
                    lblRegionSensorFull.BackColor = (MACHINE.PLCIO.IsGetBit("", "M0024") ? Color.Red : Color.Black);
                    lblRegionLower.BackColor = (MACHINE.PLCIO.IsGetBit("", "M0038") ? Color.Red : Color.Black);

                    btnLoad.BackColor = (m_FEEDZ1loadprocess.IsOn ? Color.Red : Color.FromArgb(128, 255, 128));
                    btnUnLoad.BackColor = (m_FEEDZ1Unloadprocess.IsOn ? Color.Red : Color.FromArgb(128, 255, 128));

                    break;
                case RegionEnum.FEED_Z2:

                    lblRegionUpper.BackColor = (MACHINE.PLCIO.IsGetBit("", "M0039") ? Color.Red : Color.Black);
                    lblRegionProductUpper.BackColor = (MACHINE.PLCIO.IsGetBit("", "M0016") ? Color.Green : Color.Black);
                    lblRegionProductIsHave.BackColor = (MACHINE.PLCIO.IsGetBit("", "M0015") ? Color.Green : Color.Black);
                    //lblRegionProductCount.BackColor = (MACHINE.PLCIO.IsGetBit("", "") ? Color.Red : Color.Black);
                    //lblRegionUserFull.BackColor = (MACHINE.PLCIO.IsGetBit("", "") ? Color.Red : Color.Black);
                    lblRegionSensorFull.BackColor = (MACHINE.PLCIO.IsGetBit("", "M0025") ? Color.Red : Color.Black);
                    lblRegionLower.BackColor = (MACHINE.PLCIO.IsGetBit("", "M0040") ? Color.Red : Color.Black);

                    btnLoad.BackColor = (m_FEEDZ2loadprocess.IsOn ? Color.Red : Color.FromArgb(128, 255, 128));
                    btnUnLoad.BackColor = (m_FEEDZ2Unloadprocess.IsOn ? Color.Red : Color.FromArgb(128, 255, 128));

                    break;
            }

            RegionTick();
        }


        #region REGION TICK

        /// <summary>
        /// 停掉放料载入载出流程
        /// </summary>
        public void StopRegionProcess()
        {
            m_TakePASSloadprocess.Stop();
            m_TakeNGloadprocess.Stop();
            m_TakePASSUnloadprocess.Stop();
            m_TakeNGUnloadprocess.Stop();

            m_FEEDZ1loadprocess.Stop();
            m_FEEDZ2loadprocess.Stop();
            m_FEEDZ1Unloadprocess.Stop();
            m_FEEDZ2Unloadprocess.Stop();

            MOTOR_REGION_PASS.Stop();
            MOTOR_REGION_NG.Stop();
            MOTOR_FEED_Z1.Stop();
            MOTOR_FEED_Z2.Stop();
        }

        PLCMotionClass MOTOR_REGION_PASS
        {
            get { return MACHINE.PLCMOTIONCollection[(int)MotionEnum.M6]; }
        }
        PLCMotionClass MOTOR_REGION_NG
        {
            get { return MACHINE.PLCMOTIONCollection[(int)MotionEnum.M7]; }
        }

        PLCMotionClass MOTOR_FEED_Z1
        {
            get { return MACHINE.PLCMOTIONCollection[(int)MotionEnum.M1]; }
        }
        PLCMotionClass MOTOR_FEED_Z2
        {
            get { return MACHINE.PLCMOTIONCollection[(int)MotionEnum.M2]; }
        }

        void RegionTick()
        {
            TakePASSUnLoadTick();
            TakePASSLoadTick(); 
            FEEDZ1LoadTick();
            FEEDZ2LoadTick();

            TakeNGUnLoadTick();
            TakeNGLoadTick();
            FEEDZ1UnLoadTick();
            FEEDZ2UnLoadTick();
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
              

                        if (MACHINE.PLCIO.TAKEPASS_IsHaveProductUp)
                        //if (MOTOR_REGION_PASS.IsReachUpperBound)
                        //if (IsInRange(MOTOR_REGION_PASS.PositionNow, JzMainSDPositionParas.TAKE_Z1POS1, 0.5f) && MOTOR_REGION_PASS.IsOnSite)
                        {
                            Process.Stop();
                            MOTOR_REGION_PASS.Stop();
                        }
                        else
                        {
                            MOTOR_REGION_PASS.Forward();
                            Process.NextDuriation = 200;
                            Process.ID = 10;
                        }

                      

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.PLCIO.TAKEPASS_IsHaveProductUp)
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
                       
                        if (MACHINE.PLCIO.TAKENG_IsHaveProductUp)
                        //if (MOTOR_REGION_NG.IsReachUpperBound)
                        //if (IsInRange(MOTOR_REGION_NG.PositionNow, JzMainSDPositionParas.TAKE_Z2POS1, 0.5f) && MOTOR_REGION_NG.IsOnSite)
                        {
                            Process.Stop();
                            MOTOR_REGION_NG.Stop();
                        }
                        else
                        {
                            MOTOR_REGION_NG.Forward();
                            Process.NextDuriation = 200;
                            Process.ID = 10;

                        }

                     

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.PLCIO.TAKENG_IsHaveProductUp)
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
        ProcessClass m_FEEDZ1loadprocess = new ProcessClass();
        private void FEEDZ1LoadTick()
        {
            ProcessClass Process = m_FEEDZ1loadprocess;

            if (Process.IsOn)
            {
                if (MACHINE.PLCIO.ADR_DOOR)
                {
                    Process.Stop();
                    MOTOR_FEED_Z1.Stop();
                }

                switch (Process.ID)
                {
                    case 5:
                        m_FEEDZ1Unloadprocess.Stop();
                        //MOTOR_REGION_PASS.Go(JzMainSDPositionParas.TAKE_Z1POS1);
                      
                        if (MACHINE.PLCIO.IsGetBit("", "M0014"))
                        //if (MOTOR_REGION_PASS.IsReachUpperBound)
                        //if (IsInRange(MOTOR_REGION_PASS.PositionNow, JzMainSDPositionParas.TAKE_Z1POS1, 0.5f) && MOTOR_REGION_PASS.IsOnSite)
                        {
                            Process.Stop();
                            MOTOR_FEED_Z1.Stop();
                        }
                        else
                        {
                            MOTOR_FEED_Z1.Forward();
                            Process.NextDuriation = 200;
                            Process.ID = 10;
                        }

                      

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.PLCIO.IsGetBit("", "M0014"))
                            //if (MOTOR_REGION_PASS.IsReachUpperBound)
                            //if (IsInRange(MOTOR_REGION_PASS.PositionNow, JzMainSDPositionParas.TAKE_Z1POS1, 0.5f) && MOTOR_REGION_PASS.IsOnSite)
                            {
                                Process.Stop();
                                MOTOR_FEED_Z1.Stop();
                            }
                        }
                        break;
                }
            }
        }
        ProcessClass m_FEEDZ2loadprocess = new ProcessClass();
        private void FEEDZ2LoadTick()
        {
            ProcessClass Process = m_FEEDZ2loadprocess;

            if (Process.IsOn)
            {
                if (MACHINE.PLCIO.ADR_DOOR)
                {
                    Process.Stop();
                    MOTOR_FEED_Z2.Stop();
                }

                switch (Process.ID)
                {
                    case 5:
                        m_FEEDZ2Unloadprocess.Stop();
                        //MOTOR_REGION_PASS.Go(JzMainSDPositionParas.TAKE_Z1POS1);
                     

                        if (MACHINE.PLCIO.IsGetBit("", "M0016"))
                        //if (MOTOR_REGION_PASS.IsReachUpperBound)
                        //if (IsInRange(MOTOR_REGION_PASS.PositionNow, JzMainSDPositionParas.TAKE_Z1POS1, 0.5f) && MOTOR_REGION_PASS.IsOnSite)
                        {
                            Process.Stop();
                            MOTOR_FEED_Z2.Stop();
                        }
                        else
                        {
                            MOTOR_FEED_Z2.Forward();
                            Process.NextDuriation = 200;
                            Process.ID = 10;
                        }

                      

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.PLCIO.IsGetBit("", "M0016"))
                            //if (MOTOR_REGION_PASS.IsReachUpperBound)
                            //if (IsInRange(MOTOR_REGION_PASS.PositionNow, JzMainSDPositionParas.TAKE_Z1POS1, 0.5f) && MOTOR_REGION_PASS.IsOnSite)
                            {
                                Process.Stop();
                                MOTOR_FEED_Z2.Stop();
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
                       
                        //MOTOR_REGION_PASS.Go(0);

                        if (MOTOR_REGION_PASS.IsReachHomeBound)
                        //if (IsInRange(MOTOR_REGION_PASS.PositionNow, 0, 0.5f) && MOTOR_REGION_PASS.IsOnSite)
                        //if (MOTOR_REGION_PASS.IsReachLowerBound)
                        {
                            Process.Stop();
                            MOTOR_REGION_PASS.Stop();
                        }
                        else
                        {
                            MOTOR_REGION_PASS.Backward();
                            Process.NextDuriation = 200;
                            Process.ID = 10;
                        }

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (MOTOR_REGION_PASS.IsReachHomeBound)
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
                        
                        //MOTOR_REGION_NG.Go(0);

                        if (MOTOR_REGION_NG.IsReachHomeBound)
                        //if (MOTOR_REGION_NG.IsReachLowerBound)
                        //if (IsInRange(MOTOR_REGION_NG.PositionNow, 0, 0.5f) && MOTOR_REGION_NG.IsOnSite)
                        {
                            Process.Stop();
                            MOTOR_REGION_NG.Stop();
                        }
                        else
                        {
                            MOTOR_REGION_NG.Backward();
                            Process.NextDuriation = 200;
                            Process.ID = 10;
                        }

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (MOTOR_REGION_NG.IsReachHomeBound)
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
        ProcessClass m_FEEDZ1Unloadprocess = new ProcessClass();
        private void FEEDZ1UnLoadTick()
        {
            ProcessClass Process = m_FEEDZ1Unloadprocess;

            if (Process.IsOn)
            {
                if (MACHINE.PLCIO.ADR_DOOR)
                {
                    Process.Stop();
                    MOTOR_FEED_Z1.Stop();
                }
                switch (Process.ID)
                {
                    case 5:

                        m_FEEDZ1loadprocess.Stop();
                       
                        //MOTOR_REGION_PASS.Go(0);

                        if (MOTOR_FEED_Z1.IsReachHomeBound)
                        //if (IsInRange(MOTOR_REGION_PASS.PositionNow, 0, 0.5f) && MOTOR_REGION_PASS.IsOnSite)
                        //if (MOTOR_REGION_PASS.IsReachLowerBound)
                        {
                            Process.Stop();
                            MOTOR_FEED_Z1.Stop();
                        }
                        else
                        {
                            MOTOR_FEED_Z1.Backward();
                            Process.NextDuriation = 200;
                            Process.ID = 10;

                        }


                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (MOTOR_FEED_Z1.IsReachHomeBound)
                            //if (IsInRange(MOTOR_REGION_PASS.PositionNow, 0, 0.5f) && MOTOR_REGION_PASS.IsOnSite)
                            //if (MOTOR_REGION_PASS.IsReachLowerBound)
                            {
                                Process.Stop();
                                MOTOR_FEED_Z1.Stop();
                            }
                        }
                        break;
                }
            }
        }
        ProcessClass m_FEEDZ2Unloadprocess = new ProcessClass();
        private void FEEDZ2UnLoadTick()
        {
            ProcessClass Process = m_FEEDZ2Unloadprocess;

            if (Process.IsOn)
            {
                if (MACHINE.PLCIO.ADR_DOOR)
                {
                    Process.Stop();
                    MOTOR_FEED_Z2.Stop();
                }
                switch (Process.ID)
                {
                    case 5:

                        m_FEEDZ2loadprocess.Stop();

                        //MOTOR_REGION_PASS.Go(0);
                        if (MOTOR_FEED_Z2.IsReachHomeBound)
                        //if (IsInRange(MOTOR_REGION_PASS.PositionNow, 0, 0.5f) && MOTOR_REGION_PASS.IsOnSite)
                        //if (MOTOR_REGION_PASS.IsReachLowerBound)
                        {
                            Process.Stop();
                            MOTOR_FEED_Z2.Stop();
                        }
                        else
                        {
                            MOTOR_FEED_Z2.Backward();
                            Process.NextDuriation = 200;
                            Process.ID = 10;
                        }

                     

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (MOTOR_FEED_Z2.IsReachHomeBound)
                            //if (IsInRange(MOTOR_REGION_PASS.PositionNow, 0, 0.5f) && MOTOR_REGION_PASS.IsOnSite)
                            //if (MOTOR_REGION_PASS.IsReachLowerBound)
                            {
                                Process.Stop();
                                MOTOR_FEED_Z2.Stop();
                            }
                        }
                        break;
                }
            }
        }

        #endregion

    }
}
