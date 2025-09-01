using Allinone.FormSpace.PADForm.NoHaveInspect;
using Allinone.UISpace.MSRUISpace;
using JetEazy.BasicSpace;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Allinone.FormSpace.PADForm
{
    public partial class PadNoHaveForm : Form
    {
        string InitialString = "";
        ComboBox cboMeasureMethod;
        //public Bitmap bmpRun = new Bitmap(1, 1);
        N1UI mN1;
        N2UI mN2;

        Button btnOK;
        Button btnCancel;

        public PadNoHaveForm(string initialstr)
        {
            InitialString = initialstr;
            InitializeComponent();
            InitialInside();
        }

        void InitialInside()
        {
            cboMeasureMethod = comboBox1;
            mN1 = n11;
            mN2 = n2UI1;

            btnOK = button4;
            btnCancel = button6;

            btnOK.Click += BtnOK_Click;
            btnCancel.Click += BtnCancel_Click;

            InitialMeasureMethod();

            InitialUIs();

            string[] strs = InitialString.Split('#');

            ChipNoHave mm = (ChipNoHave)Enum.Parse(typeof(ChipNoHave), strs[0], true);

            cboMeasureMethod.SelectedIndex = (int)mm + 1;

            switch (mm)
            {
                case ChipNoHave.NONE:


                    break;
                case ChipNoHave.Normal:

                    mN1.Initial(strs[1]);
                    mN1.Visible = true;

                    break;
                case ChipNoHave.BlobCount:

                    mN2.Initial(strs[1]);
                    mN2.Visible = true;

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

            switch ((ChipNoHave)(cboMeasureMethod.SelectedIndex - 1))
            {
                case ChipNoHave.NONE:


                    break;
                case ChipNoHave.Normal:

                    mN1.Visible = true;
                    break;

                case ChipNoHave.BlobCount:

                    mN2.Visible = true;
                    break;


            }

        }

        void InitialMeasureMethod()
        {
            int i = -1;

            while (i < Enum.GetNames(typeof(ChipNoHave)).Length - 1)
            {
                cboMeasureMethod.Items.Add(GetEnumDescription((ChipNoHave)i)).ToString();
                //cboMeasureMethod.Items.Add(((ChipNoHave)i).ToString());
                i++;
            }
        }
        void InitialUIs()
        {
            //PADUI.Visible = false;
            //iPDV1UI.Visible = false;

            //PADUI.Location = new Point(9, 51);
            //iPDV1UI.Location = new Point(9, 51);
            mN1.Visible = false;
            mN1.Dock = DockStyle.Fill;

            mN2.Visible = false;
            mN2.Dock = DockStyle.Fill;
        }

        string GetReturnString()
        {
            string retstr = "";

            retstr = (ChipNoHave)(cboMeasureMethod.SelectedIndex - 1) + "#";

            switch ((ChipNoHave)(cboMeasureMethod.SelectedIndex - 1))
            {
                case ChipNoHave.Normal:
                    retstr += mN1.GetDataValueString();
                    break;
                case ChipNoHave.BlobCount:
                    retstr += mN2.GetDataValueString();
                    break;
            }

            return retstr;
        }


        string GetEnumDescription(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var descriptionAttributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (descriptionAttributes.Length > 0)
            {
                return descriptionAttributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }

    }
}
