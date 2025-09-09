using Allinone.FormSpace.PADForm.NoHaveInspect;
using Allinone.FormSpace.PADForm.PADExtend;
using JetEazy.BasicSpace;
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
    public partial class PadExtendForm : Form
    {
        string InitialString = "";
        PADEX1UI pADEX1;

        Button btnOK;
        Button btnCancel;

        public PadExtendForm(string initialstr)
        {
            InitialString = initialstr;
            InitializeComponent();
            InitialInside();
        }
        void InitialInside()
        {
            pADEX1 = padeX1UI1;

            btnOK = button4;
            btnCancel = button6;

            btnOK.Click += BtnOK_Click;
            btnCancel.Click += BtnCancel_Click;

            InitialUIs();

            pADEX1.Initial(InitialString);
            pADEX1.Visible = true;

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
       
        void InitialUIs()
        {
            pADEX1.Visible = false;
            pADEX1.Dock = DockStyle.Fill;
        }

        string GetReturnString()
        {
            string retstr = "";
            retstr += pADEX1.GetDataValueString();
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
