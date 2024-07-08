using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JetEazy.FormSpace
{
    public partial class CheckForm : Form
    {

        TextBox txtShow;

        public CheckForm()
        {
            InitializeComponent();
        }
        public CheckForm(string showstr)
        {
            InitializeComponent();

            InitialInside(showstr);

            this.Load += CheckForm_Load;
        }

        private void CheckForm_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
        }

        void InitialInside(string showstr)
        {
            txtShow = textBox1;
            txtShow.Text = showstr;
            txtShow.ReadOnly = true;
        }
    }
}
