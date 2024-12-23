using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using JetEazy.BasicSpace;
using JetEazy.DBSpace;

namespace JetEazy.FormSpace
{
    public partial class AccountForm : Form
    {
        enum TagEnum
        {
            ADD,
            MODIFY,
            DEL,

            OK,
            CANCEL,
            EXIT,
        }
        
        //Language Setup
        JzLanguageClass myLanguage = new JzLanguageClass();
        
        GroupBox grpACCData;
        ComboBox cboACCName;

        TextBox txtName;
        TextBox txtPassword;

        CheckBox chkAllowSetup;
        CheckBox chkAllowManageAccount;
        CheckBox chkAllowSetupRecipe;
        CheckBox chkAllowUseShopFloor;

        Button btnAdd;
        Button btnModify;
        Button btnDelete;
        Button btnOK;
        Button btnCancel;
        Button btnExit;

        bool IsNeedToChange;

        AccDBClass ACCDB;
        string UIPath = "";
        int LanguageIndex = 0;
        AccClass OperateDataNow
        {
            get
            {
                if (DBStatus == DBStatusEnum.ADD)
                    return ACCDB.DataLast;
                else
                    return ACCDB.myDataList[cboACCName.SelectedIndex];
            }
        }

        public AccountForm(AccDBClass accdb, string uipath, int langindex)
        {
            ACCDB = accdb;
            UIPath = uipath;
            LanguageIndex = langindex;

            InitializeComponent();
            Initial();

        }
        void Initial()
        {
            myLanguage.Initial(UIPath + "\\AccountForm.jdb", LanguageIndex, this);

            grpACCData = groupBox1;
            cboACCName = comboBox1;
           
            txtName = textBox1;
            txtPassword = textBox2;

            chkAllowSetup = checkBox1;
            chkAllowManageAccount = checkBox2;
            chkAllowSetupRecipe = checkBox3;
            chkAllowUseShopFloor = checkBox4;
            
            btnAdd = button1;
            btnAdd.Tag = TagEnum.ADD;
            btnModify = button2;
            btnModify.Tag = TagEnum.MODIFY;
            btnDelete = button3;
            btnDelete.Tag = TagEnum.DEL;
            btnOK = button4;
            btnOK.Tag = TagEnum.OK;
            btnCancel = button5;
            btnCancel.Tag = TagEnum.CANCEL;
            btnExit = button6;
            btnExit.Tag = TagEnum.EXIT;
            
            btnAdd.Click += new EventHandler(btn_Click);
            btnModify.Click += new EventHandler(btn_Click);
            btnDelete.Click += new EventHandler(btn_Click);
            btnOK.Click += new EventHandler(btn_Click);
            btnCancel.Click += new EventHandler(btn_Click);
            btnExit.Click+=new EventHandler(btn_Click);

            cboACCName.Items.Clear();

            foreach (AccClass acc in ACCDB.myDataList)
            {
                cboACCName.Items.Add(acc.Name);
            }

            cboACCName.SelectedIndexChanged += new EventHandler(cboACCName_SelectedIndexChanged);
            cboACCName.SelectedIndex = 0;

            IsNeedToChange = true;

            DBStatus = DBStatusEnum.NONE;

            FillDisplay();
        
        }

        void cboACCName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsNeedToChange)
            {
                FillDisplay();
            }
        }

        void btn_Click(object sender, EventArgs e)
        {
            TagEnum KEY = (TagEnum)((Button)sender).Tag;

            switch (KEY)
            {
                case TagEnum.ADD:
                    ACCDB.Add(cboACCName.SelectedIndex);
                    DBStatus = DBStatusEnum.ADD;
                    FillDisplay();
 
                    break;
                case TagEnum.MODIFY:
                    DBStatus = DBStatusEnum.MODIFY;
                    break;
                case TagEnum.DEL:
                    if (MessageBox.Show(myLanguage.Messages("msg1",LanguageIndex), "SYS", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        int cboLast = cboACCName.SelectedIndex;

                        ACCDB.Delete(cboLast);

                        IsNeedToChange = false;
                        cboACCName.Items.RemoveAt(cboLast);
                        IsNeedToChange = true;

                        if (cboLast == cboACCName.Items.Count)
                            cboLast--;

                        cboACCName.SelectedIndex = cboLast;

                        //WriteBack(false);
                    }
                    break;
                case TagEnum.CANCEL:
                    if (DBStatus == DBStatusEnum.ADD)
                        ACCDB.DeleteLast();

                    DBStatus = DBStatusEnum.NONE;
                    
                    FillDisplay();

                    break;
                case TagEnum.OK:

                    if (ACCDB.CheckIsDuplicate(txtName.Text, OperateDataNow.No))
                    {
                        MessageBox.Show(myLanguage.Messages("msg2", LanguageIndex), "SYS", MessageBoxButtons.OK);
                        txtName.Focus();
                        break;
                    }
                    WriteBack(true);

                    FillDisplay();
                    RefreshComboBox();

                    DBStatus = DBStatusEnum.NONE;
                    break;
                case TagEnum.EXIT:
                    this.Close();
                    break;
            }
        }
        
        void RefreshComboBox()
        {
            int cboLast = cboACCName.SelectedIndex;

            IsNeedToChange = false;

            cboACCName.Items.Clear();

            foreach (AccClass acc in ACCDB.myDataList)
            {
                cboACCName.Items.Add(acc.Name);
            }

            if (DBStatus == DBStatusEnum.ADD)
                cboACCName.SelectedIndex = cboACCName.Items.Count - 1;
            else
                cboACCName.SelectedIndex = cboLast;

            IsNeedToChange = true;
        }

        void WriteBack(bool IsWithChange)
        {
            if (IsWithChange)
            {
                OperateDataNow.Name = txtName.Text;
                OperateDataNow.Password = txtPassword.Text;

                OperateDataNow.AllowSetup = chkAllowSetup.Checked;
                OperateDataNow.AllowManageAccount = chkAllowManageAccount.Checked;
                OperateDataNow.AllowSetupRecipe = chkAllowSetupRecipe.Checked;
                OperateDataNow.AllowUseShopFloor = chkAllowUseShopFloor.Checked;
            }

            ACCDB.Save();
        }

        void FillDisplay()
        {
            txtName.Text = OperateDataNow.Name;
            txtPassword.Text = OperateDataNow.Password;

            chkAllowSetup.Checked = OperateDataNow.AllowSetup;
            chkAllowManageAccount.Checked = OperateDataNow.AllowManageAccount;
            chkAllowSetupRecipe.Checked = OperateDataNow.AllowSetupRecipe;
            chkAllowUseShopFloor.Checked = OperateDataNow.AllowUseShopFloor;

            btnDelete.Enabled = !OperateDataNow.IsSuperUser && !OperateDataNow.IsOperating(ACCDB.DataNow.No);
            btnModify.Enabled = !OperateDataNow.IsSuperUser;
        }

        DBStatusEnum myDBStatus = DBStatusEnum.NONE;
        DBStatusEnum DBStatus
        {
            get
            {
                return myDBStatus;
            }
            set
            {
                myDBStatus = value;

                switch (myDBStatus)
                {
                    case DBStatusEnum.ADD:
                    case DBStatusEnum.MODIFY:
                        grpACCData.Enabled = true;
                        cboACCName.Visible = false;

                        btnAdd.Visible = false;
                        btnModify.Visible = false;
                        btnDelete.Visible = false;

                        btnOK.Visible = true;
                        btnCancel.Visible = true;

                        btnExit.Visible = false;

                        break;
                    case DBStatusEnum.NONE:
                        grpACCData.Enabled = false;
                        cboACCName.Visible = true;

                        btnAdd.Visible = true;
                        btnModify.Visible = true;
                        btnDelete.Visible = true;

                        btnOK.Visible = false;
                        btnCancel.Visible = false;

                        btnExit.Visible = true;
                        
                        break;
                }
            }
        }



    }
}