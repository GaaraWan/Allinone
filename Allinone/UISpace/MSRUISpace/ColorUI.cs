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
    public partial class ColorUI : UserControl
    {
        ComboBox cboColorType;
        ComboBox cboColorMethod;

        NumericUpDown nummmratio;
        NumericUpDown numcolorratio;
        
        NumericUpDown numextendsize;
        NumericUpDown numlinegap;
        NumericUpDown numminratio;
        NumericUpDown nummaxratio;

        Button btnReset;
        string DefaultString = "";
        public ColorUI(string str)
        {
            InitializeComponent();
            InitialInside();

            SetValue(str);
        }
        public ColorUI()
        {
            InitializeComponent();
            InitialInside();
        }


        void InitialInside()
        {

            cboColorType = comboBox1;
            cboColorMethod = comboBox2;

            numextendsize = numericUpDown1;
            numlinegap = numericUpDown2;
            
            numminratio = numericUpDown3;

            nummmratio = numericUpDown4;
            numcolorratio = numericUpDown5;
            nummaxratio = numericUpDown6;

            btnReset = button1;

            btnReset.Click += BtnReset_Click;

            int i = 0;
            
            while(i < (int)ColorCheckTypeEnum.COUNT)
            {
                cboColorType.Items.Add(((ColorCheckTypeEnum)i).ToString());
                i++;
            }

            cboColorType.SelectedIndex = 0;

            i = 0;

            while (i < (int)ColorCheckMethodEnum .COUNT)
            {
                cboColorMethod.Items.Add(((ColorCheckMethodEnum)i).ToString());
                i++;
            }

            cboColorMethod.SelectedIndex = 0;

            DefaultString = GetDataValueString();
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

            retstring = ((ColorCheckTypeEnum)(cboColorType.SelectedIndex)).ToString() +  ",";
            retstring += ((ColorCheckMethodEnum)(cboColorMethod.SelectedIndex)).ToString() + ",";
            retstring += nummmratio.Value.ToString() + ",";
            retstring += numcolorratio.Value.ToString() + ",";
            retstring += numlinegap.Value.ToString() + ",";
            retstring += numextendsize.Value.ToString() + ",";
            retstring += numminratio.Value.ToString() + ",";
            retstring += nummaxratio.Value.ToString();

            return retstring;
        }
        void SetValue(string str)
        {
            string[] strs = str.Split(',');

            try
            {
                cboColorType.SelectedIndex = (int)(ColorCheckTypeEnum)Enum.Parse(typeof(ColorCheckTypeEnum), strs[0], true);
                cboColorMethod.SelectedIndex = (int)(ColorCheckMethodEnum)Enum.Parse(typeof(ColorCheckMethodEnum), strs[1], true);

                nummmratio.Value = decimal.Parse(strs[2]);
                numcolorratio.Value = decimal.Parse(strs[3]);
                numlinegap.Value = int.Parse(strs[4]);
                numextendsize.Value = int.Parse(strs[5]);
                numminratio.Value = decimal.Parse(strs[6]);
                nummaxratio.Value = decimal.Parse(strs[7]);
            }
            catch(Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
                SetValue(DefaultString);
            }
        }
    }
}
