using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Allinone.ControlSpace.MachineSpace;
using JetEazy.FormSpace;
using JetEazy.BasicSpace;

namespace Allinone.UISpace.CTRLUISpace
{
    public partial class AllinoneSTATEUI : UserControl
    {
        Button btnMute;
        Button btnRemoveAlarm;
        Button btnReset;

        Label lblAlarm;
        ListBox lsbEvent;

        JzAllinoneMachineClass MACHINE;
        MessageForm M_WARNING_FRM;

        public AllinoneSTATEUI()
        {
            InitializeComponent();
            InitialInternal();
        }
        void InitialInternal()
        {
            btnMute = button7;
            btnRemoveAlarm = button8;
            btnReset = button1;

            lblAlarm = label5;
            lsbEvent = listBox1;

            btnMute.Click += BtnMute_Click;
            btnRemoveAlarm.Click += BtnRemoveAlarm_Click;
            btnReset.Click += BtnReset_Click;
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            M_WARNING_FRM = new MessageForm(true, "是否進行復位?");
            if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
            {
                resetprocess.Start();
                //MACHINE.PLCIO.RESET = true;
            }
            M_WARNING_FRM.Close();
            M_WARNING_FRM.Dispose();
        }

        public void Initial(JzAllinoneMachineClass machine)
        {
            MACHINE = machine;

            machine.EVENT.Initial(lsbEvent);
            machine.EVENT.Initial(lblAlarm);
        }

        private void BtnRemoveAlarm_Click(object sender, EventArgs e)
        {
            M_WARNING_FRM = new MessageForm(true, "請檢查警報是否清除?");
            if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
            {
                MACHINE.ClearAlarm = true;
                MACHINE.EVENT.RemoveAlarm();
            }
            M_WARNING_FRM.Close();
            M_WARNING_FRM.Dispose();
        }

        private void BtnMute_Click(object sender, EventArgs e)
        {
            MACHINE.PLCIO.BUZZER = false;
        }

        ProcessClass resetprocess = new ProcessClass();
        private void ResetTick()
        {
            ProcessClass Process = resetprocess;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        M_WARNING_FRM = new MessageForm("馬達復位中，請稍後...", true);
                        M_WARNING_FRM.Show();

                        MACHINE.PLCIO.RESET = true;

                        Process.NextDuriation = 1000;
                        Process.ID = 10;

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if(MACHINE.PLCIO.ISRESETCOMPLETE)
                            {
                                MACHINE.GoReadyPosition();

                                Process.NextDuriation = 1000;
                                Process.ID = 15;
                            }
                        }
                        break;
                    case 15:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.ISAllOnSite())
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

        public void Tick()
        {
            ResetTick();
        }
    }
}
