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
using Allinone.UISpace.MSRUISpace;


namespace Allinone.FormSpace
{
    public partial class MeasureDataForm : Form
    {

        string InitialString = "";
        ComboBox cboMeasureMethod;
        public Bitmap bmpRun = new Bitmap(1,1);

        MbUI MBUI;
        BkUI BKUI;
        ColorUI CLUI;
        SolderUI SLUI;

        Button btnOK;
        Button btnCancel;

        public MeasureDataForm(string initialstr)
        {
            InitialString = initialstr;
            InitializeComponent();
            InitialInside();
        }

        void InitialInside()
        {
            cboMeasureMethod = comboBox1;
            MBUI = mbUI1;
            BKUI = bkUI1;
            CLUI = colorUI1;
            SLUI = solderUI1;

            btnOK = button4;
            btnCancel = button6;

            btnOK.Click += BtnOK_Click;
            btnCancel.Click += BtnCancel_Click;

            InitialMeasureMethod();

            InitialUIs();

            string[] strs = InitialString.Split('#');

            MeasureMethodEnum mm = (MeasureMethodEnum)Enum.Parse(typeof(MeasureMethodEnum), strs[0], true);
            
            cboMeasureMethod.SelectedIndex = (int)mm + 1;

            switch (mm)
            {
                case MeasureMethodEnum.NONE:


                    break;
                case MeasureMethodEnum.BLIND:

                    BKUI.Initial(strs[1]);
                    BKUI.Visible = true;

                    break;
                case MeasureMethodEnum.MBCHECK:

                    MBUI.Initial(strs[1]);
                    MBUI.Visible = true;

                    break;
                case MeasureMethodEnum.COLORCHECK:

                    CLUI.Initial(strs[1]);
                    CLUI.Visible = true;

                    break;

                case MeasureMethodEnum.SOLDERBALLCHECK:

                    SLUI.Initial(strs[1]);
                    SLUI.Visible = true;

                    break;
            }
            cboMeasureMethod.SelectedIndexChanged += CboMeasureMethod_SelectedIndexChanged;

        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            JzToolsClass.PassingString = "";
            this.DialogResult = DialogResult.Cancel;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            JzToolsClass.PassingString = GetReturnString();

            this.DialogResult = DialogResult.OK;
        }

        private void CboMeasureMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitialUIs();

            switch ((MeasureMethodEnum)(cboMeasureMethod.SelectedIndex - 1))
            {
                case MeasureMethodEnum.NONE:


                    break;
                case MeasureMethodEnum.BLIND:

                    BKUI.Visible = true;

                    break;
                case MeasureMethodEnum.MBCHECK:

                    MBUI.Visible = true;

                    break;
                case MeasureMethodEnum.COLORCHECK:

                    CLUI.Visible = true;

                    break;
                case MeasureMethodEnum.SOLDERBALLCHECK:
                    SLUI.Visible = true;
                    break;

            }

        }

        void InitialMeasureMethod()
        {
            int i = -1;

            while(i < (int)MeasureMethodEnum.COUNT)
            {
                cboMeasureMethod.Items.Add(((MeasureMethodEnum)i).ToString());
                i++;
            }
        }
        void InitialUIs()
        {
            MBUI.Visible = false;
            BKUI.Visible = false;
            CLUI.Visible = false;
            SLUI.Visible = false;

            MBUI.Location = new Point(6,58);
            BKUI.Location = new Point(6, 58);
            CLUI.Location = new Point(6, 58);
            SLUI.Location = new Point(6, 58);
        }

        string GetReturnString()
        {
            string retstr = "";

            retstr = (MeasureMethodEnum)(cboMeasureMethod.SelectedIndex - 1) + "#";
            
            switch((MeasureMethodEnum)(cboMeasureMethod.SelectedIndex - 1))
            {
                case MeasureMethodEnum.BLIND:
                    retstr += BKUI.GetDataValueString();
                    break;
                case MeasureMethodEnum.MBCHECK:
                    retstr += MBUI.GetDataValueString();
                    break;
                case MeasureMethodEnum.COLORCHECK:
                    retstr += CLUI.GetDataValueString();
                    break;
                case MeasureMethodEnum.SOLDERBALLCHECK:
                    retstr += SLUI.GetDataValueString();
                    break;
            }

            return retstr;
        }


    }
}
