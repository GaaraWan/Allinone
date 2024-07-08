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
using JetEazy.BasicSpace;
using JzASN.OPSpace;
using JzASN.UISpace;

namespace JzASN.FormSpace
{
    public partial class ASNForm : Form
    {
        enum TagEnum
        {
            COPY,
            MODIFY,
            DEL,
            REGET,

            LEAVE,
            OK,
            CANCEL,
        }

        VersionEnum VERSION;
        OptionEnum OPTION;

        Button btnModify;
        Button btnCopy;
        Button btnDel;

        Button btnOK;
        Button btnCancel;
        Button btnLeave;

        GroupBox grpASNData;
        ComboBox cboASNName;
        TextBox txtASNName;
        TextBox txtRemark;
        Label lblModifyDateTime;

        bool IsNeedToChange = false;

        ASNCollectionClass ASNCollection;

        ASNClass DataNow
        {
            get
            {
                return ASNCollection.DataNow;
            }
        }


        List<ASNItemClass> ASNItemList = new List<ASNItemClass>();

        AsnUI ASNUI;

        public ASNForm(ASNCollectionClass asncollection,VersionEnum version,OptionEnum option)
        {
            InitializeComponent();
            Initial(asncollection,version,option);
        }

        void Initial(ASNCollectionClass asncollection, VersionEnum version, OptionEnum option)
        {
            VERSION = version;
            OPTION = option;

            ASNCollection = asncollection;

            lblModifyDateTime = label4;

            btnModify = button3;
            btnCopy = button2;
            btnDel = button7;

            btnOK = button4;
            btnCancel = button6;
            btnLeave = button8;

            btnModify.Tag = TagEnum.MODIFY;
            btnCopy.Tag = TagEnum.COPY;
            btnDel.Tag = TagEnum.DEL;

            btnOK.Tag = TagEnum.OK;
            btnCancel.Tag = TagEnum.CANCEL;
            btnLeave.Tag = TagEnum.LEAVE;

            btnModify.Click += btn_Click;
            btnCopy.Click += btn_Click;
            btnDel.Click += btn_Click;

            btnOK.Click += btn_Click;
            btnCancel.Click += btn_Click;
            btnLeave.Click += btn_Click;

            grpASNData = groupBox2;

            cboASNName = comboBox1;
            txtASNName = textBox1;
            txtRemark = textBox2;

            //ASNUI = asnUI1;

            InitialASNName();

            this.KeyPreview = true;
            this.KeyDown += AssignForm_KeyDown;
            this.KeyUp += AssignForm_KeyUp;
            //FillDisplay();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ASNUI = asnUI1;
            FillDisplay();

            DBStatus = DBStatusEnum.NONE;
            //ASNUI.Initial(ASNCollection.DataNow);
        }

        private void AssignForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (DBStatus == DBStatusEnum.NONE)
                return;

            if (e.Control)
            {
                ASNUI.HoldSelect();
                ASNUI.MoveMover(e.KeyCode);
            }

            switch (e.KeyCode)
            {
                case Keys.F7:
                    ASNUI.Add();
                    break;
                case Keys.F8:
                    ASNUI.Delete();
                    break;
            }
        }
        private void AssignForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
                ASNUI.ReleaseSelect();

        }
        void InitialASNName()
        {
            cboASNName.Items.Clear();

            string[] strs = ASNCollection.ToASNComboItem();

            foreach (string str in strs)
            {
                cboASNName.Items.Add(str);
            }

            cboASNName.SelectedIndex = 0;

            cboASNName.SelectedIndexChanged += cboASNName_SelectedIndexChanged;
        }
        void InitialASNUI()
        {


        }

        private void cboASNName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            ASNCollection.GotoIndex(cboASNName.SelectedIndex);
            FillDisplay();
        }
        //void GetData()
        //{
        //    ClearData();

        //    foreach(ASNItemClass assignitem in DataNow.ASNItemList)
        //    {
        //        ASNItemClass newassignitem = assignitem.Clone();
        //        ASNItemList.Add(newassignitem);
        //    }

        //    //bmpVIEW.Dispose();
        //    //bmpVIEW = new Bitmap(DataNow.bmpASN);
        //}
        //void SetData()
        //{
        //    foreach(ASNItemClass assignitem in DataNow.ASNItemList)
        //    {
        //        assignitem.Suicide();
        //    }
        //    DataNow.ASNItemList.Clear();

        //    foreach(ASNItemClass assignitem in ASNItemList)
        //    {
        //        DataNow.ASNItemList.Add(assignitem);
        //    }

        //    //DataNow.GetBMP(bmpVIEW);
        //    //bmpVIEW.Dispose();
        //}
        //void ClearData()
        //{
        //    foreach (ASNItemClass assignitem in ASNItemList)
        //    {
        //        assignitem.Suicide();
        //    }
        //    ASNItemList.Clear();

        //    //bmpVIEW.Dispose();
        //}

        private void btn_Click(object sender, EventArgs e)
        {
            TagEnum KEY = (TagEnum)((Button)sender).Tag;

            switch (KEY)
            {
                case TagEnum.COPY:
                    Add(DataNow.ASNNoString, true);
                    break;
                case TagEnum.MODIFY:

                    ASNCollection.DataNow.Backup();
                    DBStatus = DBStatusEnum.MODIFY;

                    break;
                case TagEnum.DEL:
                    Del();
                    break;
                case TagEnum.OK:
                    OK();
                    break;
                case TagEnum.CANCEL:
                    Cancel();
                    break;
                case TagEnum.LEAVE:
                    Exit();
                    break;
            }
        }

        int DataFromNo = 0;
        void Add(string rcpnostring, bool iscopy)
        {
            DataFromNo = DataNow.No;
            ASNCollection.Add(rcpnostring, iscopy);

            FillDisplay();

            DBStatus = DBStatusEnum.ADD;
        }

        void Cancel()
        {
            if (DBStatus == DBStatusEnum.ADD)
            {
                ASNCollection.DeleteLast(DataFromNo);
            }
            else
                RestoreBack();

            FillDisplay();

            DBStatus = DBStatusEnum.NONE;
        }
        void OK()
        {
            if (ASNCollection.CheckIsDuplicate(txtASNName.Text, DataNow.No))
            {
                MessageBox.Show("名稱重復，請確認。", "SYS", MessageBoxButtons.OK);
                txtASNName.Focus();

                return;
            }

            WriteBack(true);

            RefreshASNName();

            FillDisplay();
            DBStatus = DBStatusEnum.NONE;
        }

        void Exit()
        {
            this.Close();
        }
        void Del()
        {
            if (MessageBox.Show("是否要刪除此筆資料?", "SYS", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int cboLast = cboASNName.SelectedIndex;

                ASNCollection.Delete(cboLast);

                IsNeedToChange = false;
                cboASNName.Items.RemoveAt(cboLast);
                IsNeedToChange = true;

                if (cboLast == cboASNName.Items.Count)
                    cboLast--;

                cboASNName.SelectedIndex = cboLast;

            }


        }
        void WriteBack(bool iswithchange)
        {
            if (iswithchange)
            {
                DataNow.Name = txtASNName.Text;
                DataNow.Remark = txtRemark.Text;

                DataNow.ModifyDatetime = JzTimes.DateTimeString;
            }

            ASNCollection.Save(DataNow.No);
        }
        public void RestoreBack()
        {
            RestoreBack(true);
        }
        public void RestoreBack(bool isneedload)
        {
            ASNCollection.DataNow.Restore();
        }
        void RefreshASNName()
        {
            IsNeedToChange = false;

            cboASNName.Items.Clear();

            string[] strs = ASNCollection.ToASNComboItem();

            foreach (string str in strs)
            {
                cboASNName.Items.Add(str);
            }

            cboASNName.SelectedIndex = ASNCollection.FindIndex(DataNow.No);

            IsNeedToChange = true;
        }
        void FillDisplay()
        {
            IsNeedToChange = false;

            txtASNName.Text = ASNCollection.DataNow.Name;
            txtRemark.Text = ASNCollection.DataNow.Remark;

            lblModifyDateTime.Text = ASNCollection.DataNow.ToModifyString();

            ASNUI.Initial(ASNCollection.DataNow, VERSION, OPTION);

            btnDel.Enabled = DataNow.No != 1;

            IsNeedToChange = true;
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

                        grpASNData.Enabled = true;

                        btnCopy.Visible = false;
                        btnModify.Visible = false;
                        btnDel.Visible = false;
                        cboASNName.Visible = false;

                        btnOK.Visible = true;
                        btnCancel.Visible = true;

                        btnLeave.Visible = false;

                        ASNUI.SetEnable(true);

                        break;
                    case DBStatusEnum.NONE:
                        grpASNData.Enabled = false;

                        btnCopy.Visible = true;
                        btnModify.Visible = true;
                        btnDel.Visible = true;
                        cboASNName.Visible = true;

                        btnOK.Visible = false;
                        btnCancel.Visible = false;

                        btnLeave.Visible = true;

                        ASNUI.SetDefaultView();
                        ASNUI.SetEnable(false);

                        DataNow.Reset();

                        break;
                }
            }
        }
    }
}
