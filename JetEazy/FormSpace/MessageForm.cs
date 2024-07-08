using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using JetEazy.BasicSpace;

namespace JetEazy.FormSpace
{
    public partial class MessageForm : Form
    {
        Label lblMessage;
        Timer t;
        int syssec = 0;

        Button btnOK;
        Button btnCancel;

        TextBox txtLot;
        Label lblLot;

        Timer MSGTimer;

        public MessageForm(string rMessageStr)
        {
            InitializeComponent();
            Initial(rMessageStr);
        }
        public MessageForm(string rMessageStr,bool IsTopMost)
        {
            InitializeComponent();
            Initial(rMessageStr);

            this.TopMost = IsTopMost;
        }
        public MessageForm(string rMessageStr, int timer)
        {
            InitializeComponent();
            Initial(rMessageStr);

            MSGTimer = new Timer();
            MSGTimer.Interval = timer * 2000;
            MSGTimer.Tick += new EventHandler(MSGTimer_Tick);
            MSGTimer.Start();

            this.TopMost = true;
        }

        public MessageForm(bool IsYesNo,string rMessageStr)
        {
            InitializeComponent();
            Initial(rMessageStr);

            btnOK.Visible = IsYesNo;
            btnCancel.Visible = IsYesNo;

            this.TopMost = true;
        }

        public string NewLot
        {
            get { return txtLot.Text.Trim(); }
        }

        public MessageForm(bool IsYesNo, string rMessageStr, string eLot, string eLotx = "")
        {
            InitializeComponent();
            Initial(rMessageStr);

            btnOK.Visible = IsYesNo;
            btnCancel.Visible = IsYesNo;

            lblLot.Visible = true;
            txtLot.Visible = true;

            txtLot.Text = eLot;
            txtLot.SelectAll();
            txtLot.Focus();

            this.TopMost = true;
        }

        public MessageForm(bool IsYesNo, string rMessageStr, string strDesc)
        {
            InitializeComponent();
            Initial(rMessageStr);

            btnOK.Visible = IsYesNo;
            btnCancel.Visible = false;
            btnOK.Location = new Point(btnCancel.Location.X, btnCancel.Location.Y);

            this.TopMost = true;
        }

        public MessageForm(int sec, bool IsTopMost)
        {
            InitializeComponent();

            syssec = sec;

            t = new Timer();
            t.Interval = 1000;
            t.Enabled = true;
            t.Tick += new EventHandler(t_Tick);

            this.TopMost = IsTopMost;
        }

        void t_Tick(object sender, EventArgs e)
        {
            Initial("Ê±¼äµ¹Êý " + syssec.ToString("00") + " s");
            syssec--;

            if (syssec == 0)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        void MSGTimer_Tick(object sender, EventArgs e)
        {
            this.Close();
        }
        void Initial(string rMessageStr)
        {
            lblMessage = label1;
            int iHeight = lblMessage.Height;
            int iWidth = lblMessage.Width;
            lblMessage.Text = rMessageStr;

            this.TopMost = true;

            //this.Location = new Point(620, 400);

            btnOK = button4;
            btnCancel = button6;

            txtLot = textBox1;
            lblLot = label2;

            btnOK.Click += new EventHandler(btnOK_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
            
            this.CenterToParent();
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.No;
            this.Close();
        }

        void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.Close();
        }

    }
}