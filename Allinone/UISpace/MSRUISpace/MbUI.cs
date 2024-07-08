using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Allinone.UISpace.MSRUISpace
{
    public partial class MbUI : UserControl
    {
        NumericUpDown numUDRange;
        NumericUpDown numHeightThreshold;

        CheckBox chkIsWidth;

        NumericUpDown numWidthThreshold;
        NumericUpDown numCutLeftWidth;
        NumericUpDown numCutRightWidth;
        NumericUpDown numCheckRatio;

        Button btnReset;
        string DefaultString = "";
        public MbUI(string str)
        {
            InitializeComponent();
            InitialInside();

            SetValue(str);
        }
        public MbUI()
        {
            InitializeComponent();
            InitialInside();
        }


        void InitialInside()
        {
            numUDRange = numericUpDown1;
            numHeightThreshold = numericUpDown2;

            chkIsWidth = checkBox1;

            numWidthThreshold = numericUpDown3;

            numCutLeftWidth = numericUpDown4;
            numCutRightWidth = numericUpDown5;
            numCheckRatio = numericUpDown6;

            DefaultString = GetDataValueString();

            btnReset = button1;

            btnReset.Click += BtnReset_Click;

        }
        public void Initial(string str)
        {
            SetValue(str);
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            SetValue(DefaultString);
        }

        public string GetDataValueString()
        {
            string retstring = "";

            retstring = numUDRange.Value.ToString() + ",";
            retstring += numHeightThreshold.Value.ToString() + ",";
            retstring += (chkIsWidth.Checked ? "1" : "0") + ",";
            retstring += numWidthThreshold.Value.ToString() + ",";
            retstring += numCutLeftWidth.Value.ToString() + ",";
            retstring += numCutRightWidth.Value.ToString() + ",";
            retstring += numCheckRatio.Value.ToString();

            return retstring;
        }
        void SetValue(string str)
        {
            string[] strs = str.Split(',');

            try
            {
                numUDRange.Value = int.Parse(strs[0]);
                numHeightThreshold.Value = int.Parse(strs[1]);
                chkIsWidth.Checked = strs[2] == "1";
                numWidthThreshold.Value = decimal.Parse(strs[3]);
                numCutLeftWidth.Value = int.Parse(strs[4]);
                numCutRightWidth.Value = int.Parse(strs[5]);
                numCheckRatio.Value = decimal.Parse(strs[6]);
            }
            catch(Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
                SetValue(DefaultString);
            }
        }

    }
}
