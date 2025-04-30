﻿using JzDisplay;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JzOCR.FormSpace
{
    public partial class ShowResultForm : Form
    {
        Button btnOK;
        TextBox tbResult;
        Label lblDefect;
        Bitmap bmptmp;
        JzDisplay.UISpace.DispUI ResultDISP;

        public ShowResultForm(Bitmap bmpTemp,string strOCR,bool isDefect)
        {
            InitializeComponent();

            btnOK = button1;
            tbResult = textBox1;
            lblDefect = label2;
            //ResultDISP = dispUI2;
            //ResultDISP.Initial();

            init_Display();
            update_Display();

            bmptmp = bmpTemp;
            // ResultDISP.SetDisplayType(DisplayTypeEnum.NORMAL);

            //ResultDISP.
            //ResultDISP.RefreshDisplayShape();
            tbResult.Text = strOCR;
            lblDefect.Text = isDefect ? "有缺失" : "无缺失";

            
            btnOK.Click += BtnOK_Click;
            this.CenterToParent();
            this.Load += ShowResult_Load;
            this.SizeChanged += ShowResultForm_SizeChanged;
        }

        private void ShowResultForm_SizeChanged(object sender, EventArgs e)
        {
            update_Display();
        }

        private void ShowResult_Load(object sender, EventArgs e)
        {
            ResultDISP.SetDisplayImage(bmptmp);
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void init_Display()
        {
            ResultDISP = dispUI2;
            ResultDISP.Initial();
            ResultDISP.SetDisplayType(DisplayTypeEnum.NORMAL);
        }
        void update_Display()
        {
            ResultDISP.Refresh();
            ResultDISP.DefaultView();
        }
    }
}
