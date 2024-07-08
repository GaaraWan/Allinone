using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using JetEazy.BasicSpace;

namespace Allinone.FormSpace
{
    public partial class PAGECountForm : Form
    {
        NumericUpDown numPageCount;
        Button btnOK;

        public PAGECountForm()
        {
            InitializeComponent();
            InitialInside();
        }
        void InitialInside()
        {
            numPageCount = numericUpDown1;
            btnOK = button8;

            btnOK.Click += BtnOK_Click;

            this.TopMost = true;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            JzToolsClass.PassingInteger = (int)numPageCount.Value;
            this.Close();

        }
    }
}
