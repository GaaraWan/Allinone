using Allinone.ControlSpace;
using Allinone.ControlSpace.IOSpace;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Allinone.FormSpace
{
    public partial class SimFormOffline : Form
    {
        const int IO_COUNT = 8;

        JzMainX6IOClass PLCIO
        {
            get
            {
                return ((ControlSpace.MachineSpace.JzMainX6MachineClass)MACHINECollection.MACHINE).PLCIO;
            }
        }

        MachineCollectionClass MACHINECollection
        {
            get
            {
                return Universal.MACHINECollection;
            }
        }

        Label[] lblX = new Label[IO_COUNT];
        Label[] lblY = new Label[IO_COUNT];

        Timer m_Timer = null;

        public SimFormOffline()
        {
            INI.show_simform = true;
            InitializeComponent();

            this.TopMost = true;

            this.Load += SimFormOffline_Load;
            this.FormClosed += SimFormOffline_FormClosed;
        }

        private void SimFormOffline_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_Timer.Enabled = false;
            m_Timer = null;
            INI.show_simform = false;
        }

        private void SimFormOffline_Load(object sender, EventArgs e)
        {
            Init();

            m_Timer = new Timer();
            m_Timer.Interval = 50;
            m_Timer.Enabled = true;
            m_Timer.Tick += M_Timer_Tick;
        }

        private void M_Timer_Tick(object sender, EventArgs e)
        {
            int i = 0;
            while (i < IO_COUNT)
            {
                lblX[i].BackColor = (PLCIO.M_X[i] ? Color.Red : Control.DefaultBackColor);
                i++;
            }
            i = 0;
            while (i < IO_COUNT)
            {
                lblY[i].BackColor = (PLCIO.M_Y[i] ? Color.Red : Control.DefaultBackColor);
                i++;
            }
        }

        void Init()
        {
            int i = 0;
            while (i < IO_COUNT)
            {
                lblX[i] = new Label();
                lblX[i].Name = "Xlbl" + i.ToString();
                lblX[i].Tag = i.ToString();
                lblX[i].AutoSize = false;
                lblX[i].Size = new Size(80, 30);
                lblX[i].TextAlign = ContentAlignment.MiddleCenter;
                lblX[i].Text = "X" + i.ToString("00");
                lblX[i].Location = new Point(10 + i * (lblX[i].Size.Width + 5), 10);
                lblX[i].DoubleClick += SimFormOfflinelbl_DoubleClick;

                this.Controls.Add(lblX[i]);

                i++;
            }

            i = 0;
            while (i < IO_COUNT)
            {
                lblY[i] = new Label();
                lblY[i].Name = "Ylbl" + i.ToString();
                lblY[i].Tag = i.ToString();
                lblY[i].AutoSize = false;
                lblY[i].Size = new Size(80, 30);
                lblY[i].TextAlign = ContentAlignment.MiddleCenter;
                lblY[i].Text = "Y" + i.ToString("00");
                lblY[i].Location = new Point(10 + i * (lblY[i].Size.Width + 5), 60);
                //lblY[i].DoubleClick += SimFormOfflinelbl_DoubleClick;

                this.Controls.Add(lblY[i]);

                i++;
            }

        }

        private void SimFormOfflinelbl_DoubleClick(object sender, EventArgs e)
        {
            Label lblx = (Label)sender;
            int i = int.Parse((string)lblx.Tag);

            PLCIO.M_X[i] = !PLCIO.M_X[i];
        }
    }
}
