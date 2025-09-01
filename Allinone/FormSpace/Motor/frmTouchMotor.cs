using Allinone.ControlSpace;
using Allinone.ControlSpace.MachineSpace;
using Common;
using JetEazy.ControlSpace.MotionSpace;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Allinone.FormSpace.Motor
{
    public partial class frmTouchMotor : Form
    {
        const int AXIS_COUNT = 4;
        VsTouchMotorUI[] VSAXISUI = new VsTouchMotorUI[AXIS_COUNT];
        MotionTouchPanelUIClass[] AXISUI = new MotionTouchPanelUIClass[AXIS_COUNT];

        /// <summary>
        /// 顶升气缸1伸出
        /// </summary>
        Button btnCyTopRise1Open;
        Button btnCyTopRise2Open;
        /// <summary>
        /// 顶升气缸1缩回
        /// </summary>
        Button btnCyTopRise1Close;
        Button btnCyTopRise2Close;

        /// <summary>
        /// 阻挡气缸1伸出
        /// </summary>
        Button btnCyBlock1Open;
        Button btnCyBlock2Open;
        /// <summary>
        /// 阻挡气缸1缩回
        /// </summary>
        Button btnCyBlock1Close;
        Button btnCyBlock2Close;

        Button[] btnMotorForward = new Button[4];
        Button[] btnMotorBackward = new Button[4];

        Timer mMotorTimer = null;
        MachineCollectionClass MACHINECollection
        {
            get
            {
                return Universal.MACHINECollection;
            }
        }
        //JzMainSDM2MachineClass MACHINE
        //{
        //    get { return (JzMainSDM2MachineClass)MACHINECollection.MACHINE; }
        //}
        public frmTouchMotor()
        {
            InitializeComponent();

            this.TopMost = true;

            this.Load += FrmTouchMotor_Load;
            this.FormClosed += FrmTouchMotor_FormClosed;
        }
        private void FrmTouchMotor_FormClosed(object sender, FormClosedEventArgs e)
        {
            Universal.IsOpenMotorWindows = false;
        }
        private void FrmTouchMotor_Load(object sender, EventArgs e)
        {
            this.Text = "轴设定视窗";
            Init();
        }
        void Init()
        {
            #region 位置设定控件

            VSAXISUI[0] = vsTouchMotorUI3;
            VSAXISUI[1] = vsTouchMotorUI2;
            VSAXISUI[2] = vsTouchMotorUI1;
            VSAXISUI[3] = vsTouchMotorUI4;

            int i = 0;
            while (i < AXIS_COUNT)
            {
                AXISUI[i] = new MotionTouchPanelUIClass(VSAXISUI[i]);

                switch (Universal.OPTION)
                {
                    case JetEazy.OptionEnum.MAIN_SDM2:
                        AXISUI[i].Initial(((JzMainSDM2MachineClass)MACHINECollection.MACHINE).PLCMOTIONCollection[i]);
                        break;
                    case JetEazy.OptionEnum.MAIN_SDM3:
                        AXISUI[i].Initial(((JzMainSDM3MachineClass)MACHINECollection.MACHINE).PLCMOTIONCollection[i]);
                        break;
                    case JetEazy.OptionEnum.MAIN_SDM5:
                        AXISUI[i].Initial(((JzMainSDM5MachineClass)MACHINECollection.MACHINE).PLCMOTIONCollection[i]);
                        break;
                }


                i++;
            }

            #endregion

            mMotorTimer = new Timer();
            mMotorTimer.Interval = 50;
            mMotorTimer.Enabled = true;
            mMotorTimer.Tick += MMotorTimer_Tick;

            btnCyTopRise1Open = button2;
            btnCyTopRise1Close = button1;
            btnCyTopRise2Open = button4;
            btnCyTopRise2Close = button3;
            btnCyBlock1Open = button6;
            btnCyBlock1Close = button5;
            btnCyBlock2Open = button8;
            btnCyBlock2Close = button7;

            btnMotorForward = new Button[4]
            {
                 button9,  button12,
                 button16,  button14,
            };
            btnMotorBackward = new Button[4]
            {
                button10,  button11,
                button15,  button13,
            };


            switch(Universal.OPTION)
            {
                case JetEazy.OptionEnum.MAIN_SDM5:

                    i = 0;
                    while (i < 4)
                    {
                        btnMotorForward[i].Tag = i;
                        btnMotorForward[i].MouseDown += Motor_ForwardMouseDown;
                        btnMotorForward[i].MouseUp += Motor_ForwardMouseUp;

                        i++;
                    }

                    i = 0;
                    while (i < 4)
                    {
                        btnMotorBackward[i].Tag = i;
                        btnMotorBackward[i].MouseDown += Motor_BackwardMouseDown;
                        btnMotorBackward[i].MouseUp += Motor_BackwardMouseUp;

                        i++;
                    }


                    btnCyTopRise1Open.Click += BtnCyTopRise1Open_Click;
                    btnCyTopRise1Close.Click += BtnCyTopRise1Close_Click;
                    btnCyTopRise2Open.Click += BtnCyTopRise2Open_Click;
                    btnCyTopRise2Close.Click += BtnCyTopRise2Close_Click;
                    btnCyBlock1Open.Click += BtnCyBlock1Open_Click;
                    btnCyBlock1Close.Click += BtnCyBlock1Close_Click;
                    btnCyBlock2Open.Click += BtnCyBlock2Open_Click;
                    btnCyBlock2Close.Click += BtnCyBlock2Close_Click;

                    break;
            }


            //btnCylider = button1;
            //btnCylider.Click += BtnCylider_Click;

            //MACHINE.TriggerAction += MACHINE_TriggerAction;

            //FillDisplay();

            //LanguageExClass.Instance.EnumControls(this);
        }

        private void Motor_BackwardMouseUp(object sender, MouseEventArgs e)
        {
            int index = (int)((Button)sender).Tag;
            PLCMotionClass pLCMotion = getMotion(index);
            pLCMotion.Stop();
        }

        private void Motor_BackwardMouseDown(object sender, MouseEventArgs e)
        {
            int index = (int)((Button)sender).Tag;
            PLCMotionClass pLCMotion = getMotion(index);
            pLCMotion.Backward();
        }

        private void Motor_ForwardMouseUp(object sender, MouseEventArgs e)
        {
            int index = (int)((Button)sender).Tag;
            PLCMotionClass pLCMotion = getMotion(index);
            pLCMotion.Stop();
        }

        private void Motor_ForwardMouseDown(object sender, MouseEventArgs e)
        {
            int index = (int)((Button)sender).Tag;
            PLCMotionClass pLCMotion = getMotion(index);
            pLCMotion.Forward();
        }

        private void BtnCyBlock2Close_Click(object sender, EventArgs e)
        {
            setIO(22, false);
            setIO(23, true);
        }

        private void BtnCyBlock2Open_Click(object sender, EventArgs e)
        {
            setIO(22, true);
            setIO(23, false);
        }

        private void BtnCyBlock1Close_Click(object sender, EventArgs e)
        {
            setIO(20, false);
            setIO(21, true);
        }

        private void BtnCyBlock1Open_Click(object sender, EventArgs e)
        {
            setIO(20, true);
            setIO(21, false);
        }

        private void BtnCyTopRise2Close_Click(object sender, EventArgs e)
        {
            setIO(18, false);
            setIO(19, true);
        }

        private void BtnCyTopRise2Open_Click(object sender, EventArgs e)
        {
            setIO(18, true);
            setIO(19, false);
        }

        private void BtnCyTopRise1Close_Click(object sender, EventArgs e)
        {
            setIO(16, false);
            setIO(17, true);
        }

        private void BtnCyTopRise1Open_Click(object sender, EventArgs e)
        {
            setIO(16, true);
            setIO(17, false);
        }

        //private void BtnCylider_Click(object sender, EventArgs e)
        //{
        //    bool ison = ((JzMainSDM5MachineClass)MACHINECollection.MACHINE).PLCIO.GetBit(16);
        //    ((JzMainSDM5MachineClass)MACHINECollection.MACHINE).PLCIO.SetBit(16, !ison);
        //}

        private void MMotorTimer_Tick(object sender, EventArgs e)
        {
            //if (!Universal.IsNoUseIO)
            //{
            //    if (IsEMCTriggered)
            //    {
            //        //SetAbnormalLight();

            //        IsEMCTriggered = false;
            //        StopAllProcess();
            //        //OnTrigger(ActionEnum.ACT_ISEMC, "");
            //    }
            //}

            switch(Universal.OPTION)
            {
                case JetEazy.OptionEnum.MAIN_SDM5:

                    btnCyTopRise1Open.BackColor = (getIO(39) ? Color.Lime : Control.DefaultBackColor);
                    btnCyTopRise1Close.BackColor = (getIO(40) ? Color.Lime : Control.DefaultBackColor);
                    btnCyTopRise2Open.BackColor = (getIO(41) ? Color.Lime : Control.DefaultBackColor);
                    btnCyTopRise2Close.BackColor = (getIO(42) ? Color.Lime : Control.DefaultBackColor);
                    btnCyBlock1Open.BackColor = (getIO(43) ? Color.Lime : Control.DefaultBackColor);
                    btnCyBlock1Close.BackColor = (getIO(44) ? Color.Lime : Control.DefaultBackColor);
                    btnCyBlock2Open.BackColor = (getIO(45) ? Color.Lime : Control.DefaultBackColor);
                    btnCyBlock2Close.BackColor = (getIO(46) ? Color.Lime : Control.DefaultBackColor);

                    break;
            }
            

            int i = 0;
            while (i < AXIS_COUNT)
            {
                AXISUI[i].Tick();
                i++;
            }
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        bool getIO(int eMindex)
        {
            bool ison = ((JzMainSDM5MachineClass)MACHINECollection.MACHINE).PLCIO.GetBit(eMindex);
            return ison;
        }
        void setIO(int eMindex, bool ison)
        {
            ((JzMainSDM5MachineClass)MACHINECollection.MACHINE).PLCIO.SetBit(eMindex, !ison);
        }

        PLCMotionClass getMotion(int eIndex)
        {
            return ((JzMainSDM5MachineClass)MACHINECollection.MACHINE).PLCMOTIONCollection[eIndex];
        }

    }
}
