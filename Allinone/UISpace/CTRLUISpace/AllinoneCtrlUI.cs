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
using JetEazy.UISpace;
using JetEazy.BasicSpace;
using Allinone.ControlSpace;
using Allinone.ControlSpace.MachineSpace;

namespace Allinone.UISpace.CTRLUISpace
{
    public partial class AlliononeCtrlUI : UserControl
    {
        JzTransparentPanel tpnlCover;

        VersionEnum VERSION = VersionEnum.ALLINONE;
        OptionEnum OPTION = OptionEnum.MAIN;
        JzAllinoneMachineClass MACHINE;

        AxisMotionUI[] AxisMotionControl = new AxisMotionUI[3];
        AllinoneIOUI IOControl;
        AllinoneSTATEUI STATEControl;

        bool m_MainEnable = false;

        TextBox txtErrorInfo;
        Button btnPLCResent;

        Label lblXMOTIONTAKEPOS;
        Label lblXMOTIONBLOWINGPOS;

        Button btnXMotionSetTakePos;
        Button btnXMotionSetBlowingPos;

        Button btnXMotionGoTakePos;
        Button btnXMotionGoBlowingPos;
        
        public AlliononeCtrlUI()
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
            IOControl = allinoneIOUI1;
            STATEControl = allinoneSTATEUI1;

            lblXMOTIONTAKEPOS = label25;
            lblXMOTIONBLOWINGPOS = label3;

            btnXMotionSetTakePos = button3;
            btnXMotionSetBlowingPos = button4;

            btnXMotionGoTakePos = button7;
            btnXMotionGoBlowingPos = button2;

            txtErrorInfo = textBox1;
            btnPLCResent = button1;
        }

        public void Initial(VersionEnum version, OptionEnum option,JzAllinoneMachineClass machine)
        {
            VERSION = version;
            OPTION = option;
            MACHINE = machine;

            tpnlCover.Location = new Point(10, 30);
            tpnlCover.Size = new Size(316, 263);

            switch (OPTION)
            {
                case OptionEnum.MAIN:
                    AxisMotionControl[(int)MotionEnum.M0].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M0], VERSION, OPTION);
                    AxisMotionControl[(int)MotionEnum.M1].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M1], VERSION, OPTION);
                    AxisMotionControl[(int)MotionEnum.M2].Initial(MACHINE.PLCMOTIONCollection[(int)MotionEnum.M2], VERSION, OPTION);
                    IOControl.Initial(MACHINE);
                    STATEControl.Initial(MACHINE);

                    btnXMotionSetTakePos.Click += BtnXMotionSetTakePos_Click;
                    btnXMotionSetBlowingPos.Click += BtnXMotionSetBlowingPos_Click;
                    btnXMotionGoTakePos.Click += BtnXMotionGoTakePos_Click;
                    btnXMotionGoBlowingPos.Click += BtnXMotionGoBlowingPos_Click;

                    tabControl1.SelectedIndexChanged += TabControl1_SelectedIndexChanged;

                    //lblXMOTIONTAKEPOS.Text = ((float)MACHINE.PLCIO.XMotionTakePos / (float)1000).ToString("0.000");
                    //lblXMOTIONBLOWINGPOS.Text = ((float)MACHINE.PLCIO.XMotionBlowingPos / (float)1000).ToString("0.000");

                    break;
            }

            btnPLCResent.Click += BtnPLCResent_Click;
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(!m_MainEnable)
            {
                tpnlCover.Visible = !(tabControl1.SelectedIndex == 3);
            }
        }

        private void BtnXMotionGoBlowingPos_Click(object sender, EventArgs e)
        {
            MACHINE.PLCMOTIONCollection[(int)MotionEnum.M2].Go(float.Parse(lblXMOTIONBLOWINGPOS.Text));
        }

        private void BtnXMotionGoTakePos_Click(object sender, EventArgs e)
        {
            MACHINE.PLCMOTIONCollection[(int)MotionEnum.M2].Go(float.Parse(lblXMOTIONTAKEPOS.Text));
        }

        private void BtnXMotionSetBlowingPos_Click(object sender, EventArgs e)
        {
            float xmotionnow = MACHINE.PLCMOTIONCollection[(int)MotionEnum.M2].PositionNow;
            MACHINE.PLCIO.XMotionBlowingPos = (int)(xmotionnow * 1000);
            //lblXMOTIONBLOWINGPOS.Text = (int)(xmotionnow * 1000).ToString("");
        }

        private void BtnXMotionSetTakePos_Click(object sender, EventArgs e)
        {
            float xmotionnow = MACHINE.PLCMOTIONCollection[(int)MotionEnum.M2].PositionNow;
            MACHINE.PLCIO.XMotionTakePos = (int)(xmotionnow * 1000);
            //lblXMOTIONTAKEPOS.Text = (int)(xmotionnow * 1000);
        }

        private void BtnPLCResent_Click(object sender, EventArgs e)
        {
            MACHINE.PLCRetry();
        }

        public void SetEnable(bool isenable)
        {
            m_MainEnable = isenable;
            tpnlCover.Visible = !isenable;

            if (!isenable)
                tabControl1.SelectedIndex = 3;

            this.BackColor = (isenable ? SystemColors.Control : Color.DarkGray);
            this.tabPage1.BackColor = (isenable ? SystemColors.Control : Color.DarkGray);
            this.tabPage2.BackColor = (isenable ? SystemColors.Control : Color.DarkGray);
            this.tabPage3.BackColor = (isenable ? SystemColors.Control : Color.DarkGray);

        }
        public void Tick()
        {
            AxisMotionControl[(int)MotionEnum.M0].Tick();
            AxisMotionControl[(int)MotionEnum.M1].Tick();
            AxisMotionControl[(int)MotionEnum.M2].Tick();
            IOControl.Tick();
            STATEControl.Tick();

            //lblXMOTIONTAKEPOS.Text = ((float)MACHINE.PLCIO.XMotionTakePos / (float)1000).ToString("0.000");
            //lblXMOTIONBLOWINGPOS.Text = ((float)MACHINE.PLCIO.XMotionBlowingPos / (float)1000).ToString("0.000");
        }




    }
}
