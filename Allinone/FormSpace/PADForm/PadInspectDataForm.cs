﻿using Allinone.UISpace.MSRUISpace;
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
    public partial class PadInspectDataForm : Form
    {
        string InitialString = "";
        ComboBox cboMeasureMethod;
        public Bitmap bmpRun = new Bitmap(1, 1);

        PadUI PADUI;
        IPDV1UI iPDV1UI;

        Button btnOK;
        Button btnCancel;

        public PadInspectDataForm(string initialstr)
        {
            InitialString = initialstr;
            InitializeComponent();
            InitialInside();
        }

        void InitialInside()
        {
            cboMeasureMethod = comboBox1;
            PADUI = padUI1;
            iPDV1UI = ipdV1UI1;

            btnOK = button4;
            btnCancel = button6;

            btnOK.Click += BtnOK_Click;
            btnCancel.Click += BtnCancel_Click;

            InitialMeasureMethod();

            InitialUIs();

            string[] strs = InitialString.Split('#');

            PadInspectMethodEnum mm = (PadInspectMethodEnum)Enum.Parse(typeof(PadInspectMethodEnum), strs[0], true);

            cboMeasureMethod.SelectedIndex = (int)mm + 1;

            switch (mm)
            {
                case PadInspectMethodEnum.NONE:


                    break;
                case PadInspectMethodEnum.PAD_SMALL:

                    PADUI.Initial(strs[1]);
                    PADUI.Visible = true;

                    break;
                case PadInspectMethodEnum.PAD_V1:

                    iPDV1UI.Initial(strs[1]);
                    iPDV1UI.Visible = true;

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

            switch ((PadInspectMethodEnum)(cboMeasureMethod.SelectedIndex - 1))
            {
                case PadInspectMethodEnum.NONE:


                    break;
                case PadInspectMethodEnum.PAD_SMALL:

                    PADUI.Visible = true;
                    break;

                case PadInspectMethodEnum.PAD_V1:
                    iPDV1UI.Visible = true;

                    break;


            }

        }

        void InitialMeasureMethod()
        {
            int i = -1;

            while (i < (int)PadInspectMethodEnum.COUNT)
            {
                cboMeasureMethod.Items.Add(((PadInspectMethodEnum)i).ToString());
                i++;
            }
        }
        void InitialUIs()
        {
            PADUI.Visible = false;
            iPDV1UI.Visible = false;

            PADUI.Location = new Point(9, 51);
            iPDV1UI.Location = new Point(9, 51);
        }

        string GetReturnString()
        {
            string retstr = "";

            retstr = (PadInspectMethodEnum)(cboMeasureMethod.SelectedIndex - 1) + "#";

            switch ((PadInspectMethodEnum)(cboMeasureMethod.SelectedIndex - 1))
            {
                case PadInspectMethodEnum.PAD_SMALL:
                    retstr += PADUI.GetDataValueString();
                    break;
                case PadInspectMethodEnum.PAD_V1:
                    retstr += iPDV1UI.GetDataValueString();
                    break;
            }

            return retstr;
        }

    }
}
