﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using JetEazy.DBSpace;
using JetEazy.BasicSpace;

namespace JetEazy.FormSpace
{
    public partial class LoginForm : Form
    {
        enum TagEnum
        {
            NAME,
            PASSWORD,
        }

        TextBox txtName;
        TextBox txtPassword;

        Button btnOK;
        Button btnCancel;

        AccDBClass DataDB;
        string UIPath = "";
        int LanguageIndex = 0;


        //Language Setup
        JzLanguageClass myLanguage = new JzLanguageClass();
        
        public LoginForm(AccDBClass accdb,string uipath,int langindex)
        {
            InitializeComponent();

            DataDB = accdb;
            UIPath = uipath;
            LanguageIndex = langindex;

            Initial();
        }
        void Initial()
        {
            myLanguage.Initial(UIPath + "\\LoginForm.jdb", LanguageIndex, this);

            txtName = textBox1;
            txtName.Tag = TagEnum.NAME;

            txtPassword = textBox2;
            txtPassword.Tag = TagEnum.PASSWORD;

            txtName.KeyDown += new KeyEventHandler(txt_KeyDown);
            txtPassword.KeyDown += new KeyEventHandler(txt_KeyDown);
            
            btnOK = button4;
            btnCancel = button6;

            btnOK.Click += new EventHandler(btnOK_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);

            cboName.Items.Clear();
            foreach (AccClass acc in DataDB.myDataList)
            {
                cboName.Items.Add(acc.Name);
            }
            cboName.SelectedIndex = 0;
            this.Text = $"登入窗口";

            JetEazy.BasicSpace.LanguageExClass.Instance.EnumControls(this);
        }

        void txt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TagEnum KEYS = (TagEnum)((TextBox)sender).Tag;

                switch (KEYS)
                {
                    case TagEnum.NAME:
                        txtPassword.Focus();
                        break;
                    case TagEnum.PASSWORD:
                        btnOK.PerformClick();
                        break;
                }
            }
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        void btnOK_Click(object sender, EventArgs e)
        {
            if (DataDB.CheckIsCertified(cboName.Text.Trim(), txtPassword.Text.Trim(), true))
            {
                JetEazy.LoggerClass.Instance.WriteLog("帐户登入: " + txtName.Text.Trim());
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show(ToChangeLanguageCode("LoginForm.msg1"), "SYS", MessageBoxButtons.OK);

                //txtName.Focus();
                //txtName.SelectAll();
            }
        }

        string ToChangeLanguageCode(string eName)
        {
            string retStr = eName;
            retStr = LanguageExClass.Instance.GetLanguageIDName(eName);
            return retStr;
        }
    }
}
