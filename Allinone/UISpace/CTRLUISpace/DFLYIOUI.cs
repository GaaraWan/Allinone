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
    public partial class DFLYIOUI : UserControl
    {
        const int MSDuriation = 100;

        JzDFlyMachineClass MACHINE;
        JzDFlyPLCIOClass PLCIO
        {
            get
            {
                return MACHINE.PLCIO;
            }
        }
        

        Label lblRed;
        Label lblGreen;
        Label lblBlue;
        Label lblWhite;
        Label lblDoor;

        Label lblEMC;
        
        JzTimes myTime = new JzTimes();

        public DFLYIOUI()
        {
            InitializeComponent();
            InitialInternal();
        }

        void InitialInternal()
        {
            lblRed = label1;
            lblBlue = label4;
            lblGreen = label3;

            lblWhite = label6;
            lblEMC = label7;
            lblDoor = label2;

            lblRed.DoubleClick += LblRed_DoubleClick;
            lblBlue.DoubleClick += LblBlue_DoubleClick;
            lblGreen.DoubleClick += LblGreen_DoubleClick;
            lblWhite.DoubleClick += LblWhite_DoubleClick;
            lblDoor.DoubleClick += LblDoor_DoubleClick;
        }

        private void LblDoor_DoubleClick(object sender, EventArgs e)
        {
            PLCIO.Door = !PLCIO.Door;
        }

        private void LblWhite_DoubleClick(object sender, EventArgs e)
        {
            PLCIO.White = !PLCIO.White;
        }
        
        private void LblBlue_DoubleClick(object sender, EventArgs e)
        {
            PLCIO.Blue = !PLCIO.Blue;
        }

        private void LblGreen_DoubleClick(object sender, EventArgs e)
        {
            PLCIO.Green = !PLCIO.Green;
        }

        private void LblRed_DoubleClick(object sender, EventArgs e)
        {
            PLCIO.Red = !PLCIO.Red;
        }

        public void Initial(JzDFlyMachineClass machine)
        {
            MACHINE = machine;
            myTime.Cut();
        }
        public void Tick()
        {
            if (myTime.msDuriation < MSDuriation)
                return;

            lblRed.BackColor = (PLCIO.Red ? Color.Red : Color.Black);
            lblBlue.BackColor = (PLCIO.Blue ? Color.Blue : Color.Black);
            lblGreen.BackColor = (PLCIO.Green ? Color.Lime : Color.Black);
            lblWhite.BackColor = (PLCIO.White ? Color.White : Color.Black);
            lblDoor.BackColor = (PLCIO.Door ? Color.Green : Color.Black);
            lblEMC.BackColor = (PLCIO.IsEMC ? Color.Red : Color.Black);

            myTime.Cut();
        }
        


    }
}
