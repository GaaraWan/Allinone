using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using JetEazy;
using Allinone.OPSpace;
using Allinone.UISpace;

namespace Allinone.FormSpace
{
    public partial class CPDForm : Form
    {
        AlbumClass ALBNow
        {
            get
            {
                return Universal.ALBNow;
            }
        }


        CPDClass ORGCPD;
        CPDClass CPD;

        Button btnOK;
        Button btnCancel;
        
        CpdUI CPDUI;
        
        public CPDForm(CPDClass cpd)
        {
            InitializeComponent();
            Initial(cpd);
        }

        void Initial(CPDClass cpd)
        {
            ORGCPD = cpd;
            CPD = ORGCPD.Clone();

            CPD.LoadCPDItem(ALBNow.PassInfo.OperatePath);

            btnOK = button4;
            btnCancel = button6;
            
            btnOK.Click += BtnOK_Click;
            btnCancel.Click += BtnCancel_Click;

            this.KeyPreview = true;
            this.KeyDown += CPDForm_KeyDown;
            this.KeyUp += CPDForm_KeyUp;


            switch (Universal.OPTION)
            {
                case OptionEnum.MAIN_SD:
                case JetEazy.OptionEnum.MAIN_X6:

                    JetEazy.BasicSpace.LanguageExClass.Instance.EnumControls(this);

                    break;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            CPDUI = cpdUI1;
            CPDUI.Initial(CPD);
        }

        private void CPDForm_KeyDown(object sender, KeyEventArgs e)
        {   
            if (e.Control)
            {
                CPDUI.HoldSelect();
                CPDUI.MoveMover(e.KeyCode);
            }

            switch (e.KeyCode)
            {
                case Keys.F7:
                    CPDUI.Add();
                    break;
                case Keys.F8:
                    CPDUI.Delete();
                    break;
            }
        }
        private void CPDForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
                CPDUI.ReleaseSelect();

        }
        void SetData()
        {
            CPDUI.GetCPDBMP();
            CPD.SaveCPDItem(ALBNow.PassInfo.OperatePath);

            ORGCPD.Dupe(CPD);
        }
        private void BtnCancel_Click(object sender, EventArgs e)
        {   
            CPDUI.Suicide();
            CPD.Suicide();

            this.DialogResult = DialogResult.Cancel;
        }
        private void BtnOK_Click(object sender, EventArgs e)
        {
            SetData();

            CPDUI.Suicide();
            CPD.Suicide();

            this.DialogResult = DialogResult.OK;
        }
    }
}
