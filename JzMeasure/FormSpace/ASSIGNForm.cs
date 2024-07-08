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
using JzMSR.OPSpace;

namespace JzMSR.FormSpace
{
    public partial class ASSIGNForm : Form
    {
        enum TagEnum
        {
            OK,
            CANCEL,
        }

        public ASSIGNForm(MSRClass _msr)
        {
            InitializeComponent();
            MSR = _msr;
            _Initial();
        }

        MSRClass MSR;

        NumericUpDown numXDirCount;
        NumericUpDown numYDirCount;
        NumericUpDown numLTX;
        NumericUpDown numLTY;
        NumericUpDown numXGap;
        NumericUpDown numYGap;

        Button btnOK;
        Button btnCancel;
        private void _Initial()
        {
            numXDirCount = numericUpDown1;
            numYDirCount = numericUpDown2;
            numLTX = numericUpDown3;
            numLTY = numericUpDown4;
            numXGap = numericUpDown6;
            numYGap = numericUpDown5;

            btnOK = button4;
            btnCancel = button6;

            btnOK.Tag = TagEnum.OK;
            btnCancel.Tag = TagEnum.CANCEL;

            btnOK.Click += btn_Click;
            btnCancel.Click += btn_Click;

            FillDisplay();
        }

        private void btn_Click(object sender, EventArgs e)
        {
            TagEnum KEY = (TagEnum)((Button)sender).Tag;

            switch (KEY)
            {
                case TagEnum.OK:

                    JzToolsClass.PassingString = numXDirCount.Value.ToString() + "," +
                    numYDirCount.Value.ToString() + "," +
                    numLTX.Value.ToString() + "," +
                    numLTY.Value.ToString() + "," +
                    numXGap.Value.ToString() + "," +
                    numYGap.Value.ToString();

                    this.DialogResult = DialogResult.OK;

                    break;
                case TagEnum.CANCEL:

                    JzToolsClass.PassingString = "";

                    this.DialogResult = DialogResult.Cancel;

                    break;
            }

        }

        void FillDisplay()
        {
            numXDirCount.Value = MSR.XDirCount;
            numYDirCount.Value = MSR.YDirCount;
            numLTX.Value = (decimal)MSR.LTX;
            numLTY.Value = (decimal)MSR.LTY;
            numXGap.Value = (decimal)MSR.XGap;
            numYGap.Value = (decimal)MSR.YGap;

        }
    }
}
