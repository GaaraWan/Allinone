﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using JetEazy;
using Allinone;

namespace Allinone.FormSpace
{
    public partial class BannerForm : Form
    {
        Label lblVersionDate;

        // progressBar1
        ProgressBar ProBar;
        public BannerForm()
        {
            InitializeComponent();
            this.Load += new EventHandler(BannerForm_Load);
            this.Activated += new EventHandler(BannerForm_Activated);
            ProBar = progressBar1;
            ProBar.Visible = false;

         
        }
        public BannerForm(out ProgressBar proBar)
        {
            InitializeComponent();
          
            this.Load += new EventHandler(BannerForm_Load);
            this.Activated += new EventHandler(BannerForm_Activated);
            ProBar = progressBar1;
            proBar = ProBar;
            ProBar.Visible = false;
            ProBar.Visible = true;

            //this.BackgroundImage.Save("D:\\Banner.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }
        void BannerForm_Load(object sender, EventArgs e)
        {
            Initial();
            //this.TopMost = true;
        }

        void Initial()
        {
            lblVersionDate = label1;
            //if (!Universal.IsDebug)
          //  this.TopMost = true;
        }

        void BannerForm_Activated(object sender, EventArgs e)
        {
            lblVersionDate.Text = Universal.VersionDate +"  " + Universal.OPTION;
            this.Refresh();
        }


    }
}
