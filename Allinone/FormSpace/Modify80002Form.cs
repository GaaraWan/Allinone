using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using JetEazy.BasicSpace;

namespace Allinone.FormSpace 
{
    public partial class Modify80000Form : Form
    {
        Label lblMessage;

        Button btnOK;
        Button btnCancel;

        Timer MSGTimer;

        public int iPar80000 = 80002;

        public Modify80000Form(string rMessageStr)
        {
            InitializeComponent();
       //     Initial(rMessageStr);
        }
        public Modify80000Form(string rMessageStr,bool IsTopMost)
        {
            InitializeComponent();
        //    Initial(rMessageStr);

            this.TopMost = IsTopMost;
        }
        public Modify80000Form(string rMessageStr, int timer)
        {
            InitializeComponent();
        //    Initial(rMessageStr);

            MSGTimer = new Timer();
            MSGTimer.Interval = timer * 2000;
            MSGTimer.Tick += new EventHandler(MSGTimer_Tick);
            MSGTimer.Start();

            this.TopMost = true;
        }

        public Modify80000Form(bool IsYesNo,string rMessageStr)
        {
            InitializeComponent();
       //     Initial(rMessageStr);

            btnOK.Visible = IsYesNo;
            btnCancel.Visible = IsYesNo;

            this.TopMost = true;
        }

        public Modify80000Form( bool IsTopMost,int par80000)
        {
            InitializeComponent();
            iPar80000 = par80000;

            Initial();

          
            btnOK.Visible = true;
            btnCancel.Visible = true;
            this.TopMost = IsTopMost;
        }
        
        void MSGTimer_Tick(object sender, EventArgs e)
        {
            this.Close();
        }
        void Initial()
        {
            lblMessage = label1;

            lblMessage.Text = lblMessage.Text + iPar80000 + "?";

            //int iHeight = lblMessage.Height;
            //int iWidth = lblMessage.Width;
           
            this.TopMost = true;

            //this.Location = new Point(620, 400);

            btnOK = button4;
            btnCancel = button6;

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
            btnOK.Enabled = false;
            btnCancel.Enabled = false;

            if (iPar80000 == 80002)
                Universal.ADD80002(progressBar1, label2);
          else  if (iPar80000 == 80003)
                Universal.ADD80003(progressBar1, label2);
         else   if (iPar80000 == 80004)
                Universal.ADD80004(progressBar1, label2);

            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.Close();
        }

    }
}