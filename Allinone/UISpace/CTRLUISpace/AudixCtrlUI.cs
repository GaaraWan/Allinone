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

namespace Allinone.UISpace
{
    public partial class AudixCtrlUI : UserControl
    {
        const int MSDuriation = 10;

        JzTransparentPanel tpnlCover;

        VersionEnum VERSION = VersionEnum.ALLINONE;
        OptionEnum OPTION = OptionEnum.MAIN;
        JzAudixMachineClass MACHINE;

        Label lblPass;
        Label lblNG;

        JzTimes myJzTimer = new JzTimes();
        

        public AudixCtrlUI()
        {  
            InitializeComponent();
            InitialInternal();
        }

        void InitialInternal()
        {
            tpnlCover = jzTransparentPanel1;

            lblPass = label1;
            lblNG = label4;

            lblPass.DoubleClick += LblPass_DoubleClick;
            lblNG.DoubleClick += LblNG_DoubleClick;

        }

        private void LblNG_DoubleClick(object sender, EventArgs e)
        {
            MACHINE.SetNGOutput(!MACHINE.AUDIXIO.IsAudixNG);
        }

        private void LblPass_DoubleClick(object sender, EventArgs e)
        {
            MACHINE.SetPassOutput(!MACHINE.AUDIXIO.IsAudixPass);
        }

        public void Initial(VersionEnum version, OptionEnum option, JzAudixMachineClass machine)
        {
            VERSION = version;
            OPTION = option;
            MACHINE = machine;

            tpnlCover.Location = new Point(0, 0);
            tpnlCover.Size = new Size(336, 303);

            myJzTimer.Cut();

        }

        public void SetEnable(bool isenable)
        {
            tpnlCover.Visible = !isenable;

            this.BackColor = (isenable ? SystemColors.Control : Color.DarkGray);
        }
        public void Tick()
        {
            if (myJzTimer.msDuriation < MSDuriation)
                return;

            MACHINE.Tick();

            lblPass.BackColor = (MACHINE.AUDIXIO.IsAudixPass ? Color.Green : Color.Black);
            lblNG.BackColor = (MACHINE.AUDIXIO.IsAudixNG ? Color.Green : Color.Black);

            myJzTimer.Cut();
        }




    }
}
