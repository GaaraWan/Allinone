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

namespace JzMSR.FormSpace
{
    public partial class AUTOFINDForm : Form
    {
        enum TagEnum
        {
            OK,
            CANCEL,
        }

        Button btnOK;
        Button btnCancel;

        CheckBox chkIsBlack;
        NumericUpDown numThresholdRatio;
        NumericUpDown numExtend;

        public AUTOFINDForm()
        {
            InitializeComponent();
            InitialInside();
        }
        void InitialInside()
        {
            btnOK = button4;
            btnCancel = button6;

            chkIsBlack = checkBox1;
            numThresholdRatio = numericUpDown1;
            numExtend = numericUpDown2;

            btnOK.Tag = TagEnum.OK;
            btnCancel.Tag = TagEnum.CANCEL;

            btnOK.Click += btn_Click;
            btnCancel.Click += btn_Click;
        }

        private void btn_Click(object sender, EventArgs e)
        {
            TagEnum KEY = (TagEnum)((Button)sender).Tag;

            switch (KEY)
            {
                case TagEnum.OK:

                    JzToolsClass.PassingString = (chkIsBlack.Checked ? "1" : "0") +
                        "," + numThresholdRatio.Value.ToString() +
                        "," + numExtend.Value.ToString();

                    this.DialogResult = DialogResult.OK;

                    break;
                case TagEnum.CANCEL:

                    JzToolsClass.PassingString = "";

                    this.DialogResult = DialogResult.Cancel;

                    break;
            }

        }
    }
}
