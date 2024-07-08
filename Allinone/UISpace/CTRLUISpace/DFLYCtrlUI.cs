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

namespace Allinone.UISpace.CTRLUISpace
{
    public partial class DFLYCtrlUI : UserControl
    {
        enum TagEnum
        {
            RECONNECT,
            CAPTUREONCE,
            CAPTURETEST,

            RESET,
        }

        const int MSDuriation = 10;

        JzTransparentPanel tpnlCover;

        VersionEnum VERSION = VersionEnum.ALLINONE;
        OptionEnum OPTION = OptionEnum.MAIN;
        JzDFlyMachineClass MACHINE;

        AxisMotionUI[] AxisMotionControl = new AxisMotionUI[3];
        DFLYIOUI IOControl;
        TabControl tabSelect;

        Button btnReconnect;
        Button btnCaptureOnce;
        Button btnCaptureTest;
        Button btnReset;
        CheckBox chkIsMicroscope;

        bool IsMicroScope
        {
            get
            {
                return chkIsMicroscope.Checked;
            }
        }

        JzTimes myJzTimer = new JzTimes();

        public DFLYCtrlUI()
        {  
            InitializeComponent();
            InitialInternal();
        }

        void InitialInternal()
        {
            tpnlCover = jzTransparentPanel1;
            tabSelect = tabControl1;

            btnReconnect = button1;
            btnCaptureOnce = button3;
            btnCaptureTest = button2;
            btnReset = button4;
            chkIsMicroscope = checkBox1;

            AxisMotionControl[0] = axisMotionUI1;
            AxisMotionControl[1] = axisMotionUI2;
            AxisMotionControl[2] = axisMotionUI3;

            IOControl = dflyioui1;

            btnReconnect.Tag = TagEnum.RECONNECT;
            btnCaptureOnce.Tag = TagEnum.CAPTUREONCE;
            btnCaptureTest.Tag = TagEnum.CAPTURETEST;
            btnReset.Tag = TagEnum.RESET;

            btnReconnect.Click += Btn_Click;
            btnCaptureOnce.Click += Btn_Click;
            btnCaptureTest.Click += Btn_Click;
            btnReset.Click += Btn_Click;
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            switch ((TagEnum)btn.Tag)
            {
                case TagEnum.RECONNECT:

                    break;
                case TagEnum.CAPTUREONCE:
                    OnTrigger(ActionEnum.CAPTUREONCE, IsMicroScope ? "M" : "");
                   
                    break;
                case TagEnum.CAPTURETEST:
                    OnTrigger(ActionEnum.CAPTURETEST, IsMicroScope ? "M" : "");
                    break;
                case TagEnum.RESET:
                    OnTrigger(ActionEnum.ALLRESET, "");
                    break;
            }
        }

        public void Initial(VersionEnum version, OptionEnum option, JzDFlyMachineClass machine)
        {
            VERSION = version;
            OPTION = option;
            MACHINE = machine;

            tpnlCover.Location = new Point(3, 29);
            tpnlCover.Size = new Size(220, 249);

            switch (OPTION)
            {
                case OptionEnum.MAIN_DFLY:
                    AxisMotionControl[(int)MotionEnum.M0].Initial(MACHINE.CANMOTIONCollection[(int)MotionEnum.M0], VERSION, OPTION, false);
                    AxisMotionControl[(int)MotionEnum.M1].Initial(MACHINE.CANMOTIONCollection[(int)MotionEnum.M1], VERSION, OPTION);
                    AxisMotionControl[(int)MotionEnum.M2].Initial(MACHINE.CANMOTIONCollection[(int)MotionEnum.M2], VERSION, OPTION);
                    IOControl.Initial(MACHINE);
                    break;
            }

            IOControl.Initial(machine);

            myJzTimer.Cut();

        }

        public void SetEnable(bool isenable)
        {
            tpnlCover.Visible = !isenable;

            this.BackColor = (isenable ? SystemColors.Control : Color.DarkGray);

            tabSelect.TabPages[0].BackColor = (isenable ? SystemColors.Control : Color.DarkGray);
            tabSelect.TabPages[1].BackColor = (isenable ? SystemColors.Control : Color.DarkGray);
            tabSelect.TabPages[2].BackColor = (isenable ? SystemColors.Control : Color.DarkGray);
        }
        public void Tick()
        {
            if (myJzTimer.msDuriation < MSDuriation)
                return;

            MACHINE.Tick();



            AxisMotionControl[(int)MotionEnum.M0].Tick();
            AxisMotionControl[(int)MotionEnum.M1].Tick();
            AxisMotionControl[(int)MotionEnum.M2].Tick();

            IOControl.Tick();
            
            myJzTimer.Cut();
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
