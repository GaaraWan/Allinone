using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScreenForJumbo
{
    public partial class JetLogo : UserControl
    {
        public JetLogo()
        {
            InitializeComponent();
            _Init();
        }
        Timer t;
        Label lblred;
        Label lblblue;
        Label lblgreen;
        Label lblwhite;
        void _Init()
        {
            lblred = label1;
            lblblue = label2;
            lblgreen = label3;
            lblwhite = label4;

            lblred.Text = "";
            lblblue.Text = "";
            lblgreen.Text = "";
            lblwhite.Text = "";

            t = new Timer();
            t.Enabled = false;
            t.Interval = 1000;
            t.Tick += new EventHandler(t_Tick);

            _RefreshLed(4);
        }

        enum Led
        {
            red=0,
            blue=1,
            green=3,
            white=2,
            all=4,
        }

        int i = 0;

        void t_Tick(object sender, EventArgs e)
        {
            _RefreshLed(i);
            i++;
            if (i == 5)
                i = 0;
        }

        void _RefreshLed(int ix)
        {
            Led ledtag = (Led)ix;
            switch (ledtag)
            {
                case Led.red:

                    t.Interval = 100;

                    lblred.BackColor = Color.Red;
                    lblblue.BackColor = Control.DefaultBackColor;
                    lblgreen.BackColor = Control.DefaultBackColor;
                    lblwhite.BackColor = Control.DefaultBackColor;
                    break;
                case Led.blue:
                    lblred.BackColor = Control.DefaultBackColor;
                    lblblue.BackColor = Color.Blue;
                    lblgreen.BackColor = Control.DefaultBackColor;
                    lblwhite.BackColor = Control.DefaultBackColor;
                    break;
                case Led.green:
                    lblred.BackColor = Control.DefaultBackColor;
                    lblblue.BackColor = Control.DefaultBackColor;
                    lblgreen.BackColor = Color.Green;
                    lblwhite.BackColor = Control.DefaultBackColor;
                    break;
                case Led.white:
                    lblred.BackColor = Control.DefaultBackColor;
                    lblblue.BackColor = Control.DefaultBackColor;
                    lblgreen.BackColor = Control.DefaultBackColor;
                    lblwhite.BackColor = Color.White;
                    break;
                case Led.all:
                    lblred.BackColor = Color.Red;
                    lblblue.BackColor = Color.Blue;
                    lblgreen.BackColor = Color.Green;
                    lblwhite.BackColor = Color.White;

                    t.Interval = 3000;

                    break;
                default:
                    lblred.BackColor = Control.DefaultBackColor;
                    lblblue.BackColor = Control.DefaultBackColor;
                    lblgreen.BackColor = Control.DefaultBackColor;
                    lblwhite.BackColor = Control.DefaultBackColor;
                    break;
            }

            

        }
    }
}
