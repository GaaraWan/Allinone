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
    public partial class SizeForm : Form
    {
        Button btnOK;
        Button btnCancel;

        NumericUpDown numWidth;
        NumericUpDown numHeight;

        public SizeForm()
        {
            InitializeComponent();
            InitialInside();
        }

        void InitialInside()
        {
            numWidth = numericUpDown1;
            numHeight = numericUpDown2;

            btnOK = button4;
            btnCancel = button6;

            btnOK.Click += BtnOK_Click;
            btnCancel.Click += BtnCancel_Click;


            string[] strs = JzToolsClass.PassingString.Split(',');

            numWidth.Value = int.Parse(strs[0]);
            numHeight.Value = int.Parse(strs[1]);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            JzToolsClass.PassingString = numWidth.Value.ToString() + "," + numHeight.Value.ToString();

            this.DialogResult = DialogResult.OK;
        }
    }
}
