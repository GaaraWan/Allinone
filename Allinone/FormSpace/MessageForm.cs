using System;
using System.Windows.Forms;

namespace Allinone.FormSpace
{
    public partial class MessageFormFoUser : Form
    {
        Button btnOK;
        Label lblShow;

        public MessageFormFoUser()
        {
            InitializeComponent();
            this.Load += MessageForm_Load;
        }

        private void MessageForm_Load(object sender, EventArgs e)
        {
            btnOK = button1; ;
            btnOK.Click += BtnOK_Click;
            lblShow = label1;

            this.CenterToParent();
        }

        public string Message
        {
            get
            {
                return lblShow.Text;
            }
            set
            {
                lblShow.Text = value;
            }
        }
        private void BtnOK_Click(object sender, EventArgs e)
        {
            this.Close();
          //  this.DialogResult == DialogResult.OK;
        }
    }
}
