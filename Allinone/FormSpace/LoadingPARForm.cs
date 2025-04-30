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
    public partial class LoadingPARForm : Form
    {
        Label lblName;
        Label lblMessage;
        public LoadingPARForm(string strName)
        {
            InitializeComponent();
            lblName = label2;
            lblMessage = label1;

            lblName.Text = strName;
            this.Load += LoadingPARForm_Load;

            JetEazy.BasicSpace.LanguageExClass.Instance.EnumControls(this);
        }
        public LoadingPARForm(string strName,string Msg)
        {
            InitializeComponent();
            lblName = label2;
            lblMessage = label1;

            lblName.Text = strName;
            lblMessage.Text = Msg;
            
            this.Load += LoadingPARForm_Load;
        }

        private void LoadingPARForm_Load(object sender, EventArgs e)
        {
            this.CenterToParent();
            this.Refresh();
        }
    }
}
