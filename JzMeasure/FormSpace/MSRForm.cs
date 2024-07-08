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
using JetEazy.DBSpace;
using JetEazy.ControlSpace;
using JzMSR.UISpace;
using JzMSR.OPSpace;

namespace JzMSR.FormSpace
{
    public partial class MSRForm : Form
    {
        enum TagEnum
        {
            ADD,
            COPY,
            MODIFY,
            DEL,
            REGET,
            
            LEAVE,
            OK,
            CANCEL,
        }

        Button btnAdd;
        Button btnModify;
        Button btnCopy;
        Button btnDel;

        Button btnOK;
        Button btnCancel;
        Button btnLeave;

        GroupBox grpMSRData;
        ComboBox cboMSRName;
        TextBox txtMSRName;
        TextBox txtRemark;

        ComboBox cboCamera;
        NumericUpDown numExposure;
        Button btnReget;

        CCDCollectionClass CCDCollection;
        MSRCollectionClass MSRCollection;

        bool IsNeedToChange = false;

        MsrUI MSRUI;
        Label lblModifyDateTime;

        public MSRClass DataNow
        {
            get
            {
                return MSRCollection.DataNow;
            }
        }

        public MSRForm(MSRCollectionClass MSRcollection,CCDCollectionClass ccdcollection)
        {
            InitializeComponent();

            CCDCollection = ccdcollection;
            MSRCollection = MSRcollection;

            MSRCollection.GotoIndex(0);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Initial();
        }

        void Initial()
        {
            lblModifyDateTime = label4;

            btnAdd = button5;
            btnModify = button3;
            btnCopy = button2;
            btnDel = button7;

            btnOK = button4;
            btnCancel = button6;
            btnLeave = button8;

            btnReget = button1;

            btnAdd.Tag = TagEnum.ADD;
            btnModify.Tag = TagEnum.MODIFY;
            btnCopy.Tag = TagEnum.COPY;
            btnDel.Tag = TagEnum.DEL;

            btnReget.Tag = TagEnum.REGET;

            btnOK.Tag = TagEnum.OK;
            btnCancel.Tag = TagEnum.CANCEL;
            btnLeave.Tag = TagEnum.LEAVE;

            btnAdd.Click += btn_Click;
            btnModify.Click += btn_Click;
            btnCopy.Click += btn_Click;
            btnDel.Click += btn_Click;

            btnOK.Click += btn_Click;
            btnCancel.Click += btn_Click;
            btnLeave.Click += btn_Click;

            btnReget.Click += btn_Click;

            grpMSRData = groupBox2;

            cboMSRName = comboBox1;
            numExposure = numericUpDown1;

            txtMSRName = textBox1;
            txtRemark = textBox2;
            cboCamera = comboBox2;

            MSRUI = msrUI1;

            InitialMSRName();
            InitialCamera();
            InitialMSRUI();

            this.KeyPreview = true;
            this.KeyDown += detailForm_KeyDown;
            this.KeyUp += detailForm_KeyUp;

            FillDisplay();

            DBStatus = DBStatusEnum.NONE;
        }

        void InitialMSRName()
        {
            cboMSRName.Items.Clear();

            string[] strs = MSRCollection.ToMSRComboItem();

            foreach (string str in strs)
            {
                cboMSRName.Items.Add(str);
            }

            cboMSRName.SelectedIndex = 0;

            cboMSRName.SelectedIndexChanged += cboMSRName_SelectedIndexChanged;
        }

        void RefreshMSRName()
        {
            IsNeedToChange = false;

            cboMSRName.Items.Clear();

            string[] strs = MSRCollection.ToMSRComboItem();

            foreach (string str in strs)
            {
                cboMSRName.Items.Add(str);
            }

            cboMSRName.SelectedIndex = MSRCollection.FindIndex(DataNow.No);

            IsNeedToChange = true;
        }
        private void cboMSRName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            MSRCollection.GotoIndex(cboMSRName.SelectedIndex);
            FillDisplay();
        }
        void InitialMSRUI()
        {
            

        }

        void InitialCamera()
        {
            int i = 0;

            cboCamera.Items.Clear();

            while (i < CCDCollection.GetCCDCount)
            {
                cboCamera.Items.Add("CAM" + i.ToString("00"));
                i++;
            }

            cboCamera.SelectedIndex = 0;
        }
        private void btn_Click(object sender, EventArgs e)
        {
            TagEnum KEY = (TagEnum)((Button)sender).Tag;

            switch (KEY)
            {
                case TagEnum.ADD:
                    Add(MSRClass.OrgMSRNoString, false);
                    break;
                case TagEnum.COPY:
                    Add(DataNow.MSRNoString, true);
                    break;
                case TagEnum.MODIFY:

                    MSRCollection.DataNow.Backup();
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
                case TagEnum.REGET:
                    Reget();
                    break;
            }
        }

        void Reget()
        {
            DataNow.bmpCalibrate.Dispose();
            DataNow.bmpCalibrate = new Bitmap(CCDCollection.GetBMP(cboCamera.SelectedIndex, true));
            //DataNow.bmpCalibrate.RotateFlip(RotateFlipType.Rotate270FlipNone);

            FillDisplay();
        }
        int DataFromNo = 0;

        void Add(string rcpnostring, bool iscopy)
        {
            DataFromNo = DataNow.No;
            MSRCollection.Add(rcpnostring, iscopy);

            FillDisplay();

            DBStatus = DBStatusEnum.ADD;
        }

        void Cancel()
        {
            if (DBStatus == DBStatusEnum.ADD)
            {
                MSRCollection.DeleteLast(DataFromNo);
            }
            else
                RestoreBack();

            FillDisplay();

            DBStatus = DBStatusEnum.NONE;
        }
        void OK()
        {
            if (MSRCollection.CheckIsDuplicate(txtMSRName.Text, DataNow.No))
            {
                MessageBox.Show("名稱重復，請確認。", "SYS", MessageBoxButtons.OK);
                txtMSRName.Focus();

                return;
            }

            WriteBack(true);

            RefreshMSRName();

            FillDisplay();
            DBStatus = DBStatusEnum.NONE;
        }

        void Exit()
        {
            this.Close();
        }
        void Del()
        {
            if(MSRCollection.DataNow.No == 0)
            {
                MessageBox.Show("無法刪除預設參數", "SYS", MessageBoxButtons.OK);
                return;
            }

            if (MessageBox.Show("是否要刪除此筆資料?", "SYS", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int cboLast = cboMSRName.SelectedIndex;

                MSRCollection.Delete(cboLast);

                IsNeedToChange = false;
                cboMSRName.Items.RemoveAt(cboLast);
                IsNeedToChange = true;

                if (cboLast == cboMSRName.Items.Count)
                    cboLast--;

                cboMSRName.SelectedIndex = cboLast;
            }
        }
        void WriteBack(bool iswithchange)
        {
            if (iswithchange)
            {
                DataNow.Name = txtMSRName.Text;
                DataNow.Remark = txtRemark.Text;

                DataNow.CamIndex = cboCamera.SelectedIndex;
                DataNow.Exposure = (int)numExposure.Value;
                
                DataNow.ModifyDatetime = JzTimes.DateTimeString;
            }

            MSRCollection.Save(DataNow.No);
        }
        public void RestoreBack()
        {
            RestoreBack(true);
        }
        public void RestoreBack(bool isneedload)
        {
            MSRCollection.DataNow.Restore();
        }

        void FillDisplay()
        {
            IsNeedToChange = false;

            txtMSRName.Text = MSRCollection.DataNow.Name;
            txtRemark.Text = MSRCollection.DataNow.Remark;

            if (MSRCollection.DataNow.CamIndex >= cboCamera.Items.Count)
                cboCamera.SelectedIndex = 0;
            else
                cboCamera.SelectedIndex = MSRCollection.DataNow.CamIndex;

            numExposure.Value = MSRCollection.DataNow.Exposure;
            
            lblModifyDateTime.Text = MSRCollection.DataNow.ToModifyString();

            MSRUI.Initial(MSRCollection.DataNow);

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

                        grpMSRData.Enabled = true;

                        btnAdd.Visible = false;
                        btnCopy.Visible = false;
                        btnModify.Visible = false;
                        btnDel.Visible = false;
                        cboMSRName.Visible = false;

                        btnOK.Visible = true;
                        btnCancel.Visible = true;

                        btnLeave.Visible = false;

                        MSRUI.SetEnable(true);

                        break;
                    case DBStatusEnum.NONE:
                        grpMSRData.Enabled = false;

                        btnAdd.Visible = true;
                        btnCopy.Visible = true;
                        btnModify.Visible = true;
                        btnDel.Visible = true;
                        cboMSRName.Visible = true;

                        btnOK.Visible = false;
                        btnCancel.Visible = false;

                        btnLeave.Visible = true;

                        MSRUI.SetDefaultView();
                        MSRUI.SetEnable(false);

                        DataNow.Reset();

                        break;
                }
            }
        }

        private void detailForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                MSRUI.HoldSelect();
                MSRUI.MoveMover(e.KeyCode);
            }

            switch (e.KeyCode)
            {
                case Keys.F7:
                    MSRUI.AddItem();
                    break;
                case Keys.F8:
                    MSRUI.DelItem();
                    break;
            }
        }
        private void detailForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
                MSRUI.ReleaseSelect();

        }
    }
}
