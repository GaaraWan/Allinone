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
using Allinone.ControlSpace.MachineSpace;
using Allinone.ControlSpace.IOSpace;

namespace Allinone.UISpace.CTRLUISpace
{
    public partial class AllinoneIOUI : UserControl
    {
        const int MSDuriation = 100;

        JzAllinoneMachineClass MACHINE;
        JzAllinoneIOClass PLCIO
        {
            get
            {
                return MACHINE.PLCIO;
            }
        }
        

        Label lblRed;
        Label lblYellow;
        Label lblGreen;

        Label lblStart;
        Label lblEMC;
        Label lblLightCurtain;

        Label lblUUp;
        Label lblUDn;

        Label lblCyOut;
        Label lblCyIn;

        Label lblTopLight;
        Label lblMylarLight;
        Label lblAroundLight;

        Button btnUCylinder;
        Button btnLCylinederIn;
        Button btnLCylinederOut;
        Button btnPG;
        Button btnASM;

        Label lblFeeding;
        Label lblFeedComplete;

        Label lblPGTime;

        JzTimes myTime = new JzTimes();

        public AllinoneIOUI()
        {
            InitializeComponent();
            InitialInternal();
        }

        void InitialInternal()
        {
            lblRed = label1;
            lblYellow = label4;
            lblGreen = label3;

            lblStart = label6;
            lblEMC = label7;
            lblLightCurtain = label10;

            lblUUp = label2;
            lblUDn = label5;
            lblCyOut = label9;
            lblCyIn = label8;
            lblPGTime = label11;

            lblTopLight = label12;
            lblMylarLight = label13;
            lblAroundLight = label14;

            lblFeeding = label15;
            lblFeedComplete = label16;

            btnUCylinder = button1;
            btnLCylinederIn = button2;
            btnLCylinederOut = button5;
            btnPG = button3;
            btnASM = button4;

            lblRed.DoubleClick += LblRed_DoubleClick;
            lblYellow.DoubleClick += LblYellow_DoubleClick;
            lblGreen.DoubleClick += LblGreen_DoubleClick;

            btnUCylinder.Click += BtnUCylinder_Click;
            btnLCylinederIn.Click += BtnLCylinederIn_Click;
            btnLCylinederOut.Click += BtnLCylinederOut_Click;
            lblFeedComplete.DoubleClick += LblFeedComplete_DoubleClick;

            btnPG.Click += BtnPG_Click;
            btnASM.Click += BtnASM_Click;

            lblTopLight.DoubleClick += LblTopLight_DoubleClick;
            lblMylarLight.DoubleClick += LblMylarLight_DoubleClick;
            lblAroundLight.DoubleClick += LblAroundLight_DoubleClick;

            lblPGTime.DoubleClick += LblPGTime_DoubleClick;
        }

        private void LblFeedComplete_DoubleClick(object sender, EventArgs e)
        {
            if(PLCIO.IsFeedComplete)
            {
                MACHINE.ResetAutoStart();
            }
        }

        private void LblPGTime_DoubleClick(object sender, EventArgs e)
        {
            PLCIO.PGPOWER = !PLCIO.PGPOWER;
            lblPGTime.BackColor = (PLCIO.PGPOWER ? Color.Lime : Color.Yellow);
        }
        
        private void BtnASM_Click(object sender, EventArgs e)
        {
            if(PLCIO.ASMIn)
            {
                PLCIO.ASMOut = true;
            }
            else
            {
                PLCIO.ASMIn = true;
            }
        }
        private void LblAroundLight_DoubleClick(object sender, EventArgs e)
        {
            PLCIO.AroundLight = !PLCIO.AroundLight;
        }
        private void LblMylarLight_DoubleClick(object sender, EventArgs e)
        {
            PLCIO.MylarLight = !PLCIO.MylarLight;
        }

        private void LblTopLight_DoubleClick(object sender, EventArgs e)
        {
            PLCIO.TopLight = !PLCIO.TopLight;
        }
        
        private void BtnPG_Click(object sender, EventArgs e)
        {
            PLCIO.PG = true;
        }

        private void BtnLCylinederIn_Click(object sender, EventArgs e)
        {
            PLCIO.LCylinderIn = !PLCIO.IsCYIN;
        }
        private void BtnLCylinederOut_Click(object sender, EventArgs e)
        {
            PLCIO.LCylinderOut = !PLCIO.IsCYOUT;
        }

        private void BtnUCylinder_Click(object sender, EventArgs e)
        {
            PLCIO.UCylinder = !PLCIO.IsUDN;
        }

        private void LblYellow_DoubleClick(object sender, EventArgs e)
        {
            PLCIO.Yellow = !PLCIO.Yellow;
        }

        private void LblGreen_DoubleClick(object sender, EventArgs e)
        {
            PLCIO.Green = !PLCIO.Green;
        }

        private void LblRed_DoubleClick(object sender, EventArgs e)
        {
            PLCIO.Red = !PLCIO.Red;
        }

        public void Initial(JzAllinoneMachineClass machine)
        {
            MACHINE = machine;
            myTime.Cut();
        }
        public void Tick()
        {
            if (myTime.msDuriation < MSDuriation)
                return;

            lblRed.BackColor = (PLCIO.Red ? Color.Red : Color.Black);
            lblYellow.BackColor = (PLCIO.Yellow ? Color.Yellow : Color.Black);
            lblGreen.BackColor = (PLCIO.Green ? Color.Green : Color.Black);

            lblStart.BackColor = (PLCIO.IsStart ? Color.Green : Color.Black);
            lblEMC.BackColor = (PLCIO.IsEMC ? Color.Red : Color.Black);
            lblLightCurtain.BackColor = (PLCIO.IsLightCurtain ? Color.Red : Color.Black);

            lblUUp.BackColor = (PLCIO.IsUUP ? Color.Green : Color.Black);
            lblUDn.BackColor = (PLCIO.IsUDN ? Color.Green : Color.Black);
            lblCyOut.BackColor = (PLCIO.IsCYOUT ? Color.Green : Color.Black);
            lblCyIn.BackColor = (PLCIO.IsCYIN ? Color.Green : Color.Black);
            lblPGTime.Text = PLCIO.PGTime.ToString();

            lblFeeding.BackColor = (PLCIO.IsFeeding ? Color.Green : Color.Black);
            lblFeedComplete.BackColor = (PLCIO.IsFeedComplete ? Color.Green : Color.Black);

            lblMylarLight.BackColor = (PLCIO.MylarLight ? Color.Green : Color.Black);
            lblTopLight.BackColor = (PLCIO.TopLight ? Color.Green : Color.Black);
            lblAroundLight.BackColor = (PLCIO.AroundLight ? Color.Green : Color.Black);

            btnPG.BackColor = (PLCIO.PG ? Color.Red : Color.FromArgb(255, 224, 192));
            btnASM.BackColor = (PLCIO.ASMIn ? Color.Red : Color.FromArgb(255, 224, 192));

            myTime.Cut();
        }
        


    }
}
