using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Allinone.FormSpace
{
    public partial class frmUserOption : Form
    {
        RadioButton rdoAll;
        RadioButton rdoSide;
        RadioButton rdoSelected;

        Button btnOK;
        Button btnCancel;

        UserOptionEnum PassOption
        {
            set
            {
                Universal.PassOption = value;
            }
        }

        public frmUserOption(bool IsSelected)
        {
            InitializeComponent();
            Initial(IsSelected);
        }

        void Initial(bool IsSelected)
        {
            //myLanguage.Initial(INI.UI_PATH + "\\OptionForm.jdb", INI.LANGUAGE, this);


            PassOption = UserOptionEnum.ALL;

            rdoAll = radioButton1;
            rdoSide = radioButton2;
            rdoSelected = radioButton3;

            rdoSelected.Visible = IsSelected;

            btnOK = button4;
            btnCancel = button6;

            btnOK.Click += new EventHandler(btnOK_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);

            this.TopMost = true;

            JetEazy.BasicSpace.LanguageExClass.Instance.EnumControls(this);
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;

        }

        void btnOK_Click(object sender, EventArgs e)
        {
            if (rdoAll.Checked)
            {
                PassOption = UserOptionEnum.ALL;
            }
            if (rdoSide.Checked)
            {
                PassOption = UserOptionEnum.SIDE;
            }
            if (rdoSelected.Checked)
            {
                PassOption = UserOptionEnum.SELECTED;
            }

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

    }
}
