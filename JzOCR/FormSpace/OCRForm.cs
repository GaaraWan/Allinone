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
using JzOCR.UISpace;
using JzOCR.OPSpace;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace JzOCR.FormSpace
{
    public partial class OCRForm : Form
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

        GroupBox grpOCRData;
        ComboBox cboOCRName;
        TextBox txtOCRName;
        TextBox txtRemoark;

        ComboBox cboCamera;
        Button btnReget;

        public int iCCDCount = 0;
      
        CCDCollectionClass CCDCollection;
        OCRCollectionClass OCRCollection;

        OCRByPaddle.OCRByPaddle mOCRByPaddle;

        bool IsNeedToChange = false;

        OcrUI OCRUI;
        NumericUpDown numLight;
        NumericUpDown numAdjContrast;
        Label lblModifyDateTime;
        OptionEnum OPTION = OptionEnum.R32;
        bool isDebug = false;

        public static bool ISDEBUG = false;
        public OCRClass DataNow
        {
            get
            {
                return OCRCollection.DataNow;
            }
        }
        public OCRForm(bool isNoCCD)
        {
            InitializeComponent();

            // CCDCollection = ccdcollection;
            OCRCollectionClass ocrcollection = new OCRCollectionClass();
            ocrcollection.Initial();
            OCRCollection = ocrcollection;

            OCRCollection.GotoIndex(0);
            Initial();


            OCRUI.MyTest();
            this.isDebug = isNoCCD;

           
        }

        public OCRForm(OCRCollectionClass ocrcollection, OCRByPaddle.OCRByPaddle Paddle ,CCDCollectionClass ccdcollection,bool isNoCCD, OptionEnum option= OptionEnum.R32)
        {
            this.isDebug = isNoCCD;
            OPTION = option;
            InitializeComponent();

            CCDCollection = ccdcollection;
            mOCRByPaddle = Paddle;
            iCCDCount = CCDCollection.GetCCDCount;

            OCRCollection = ocrcollection;

            OCRCollection.GotoIndex(0);
            Initial();
            //iCCDCount = TransferFoOCR.TransferClass.GetCamerCount;

            OCRUI.MyTest();

            numAdjContrast.Value = (decimal)OCRCollection.AdjContrast;
            numAdjContrast.ValueChanged += NumAdjContrast_ValueChanged;

            JetEazy.BasicSpace.LanguageExClass.Instance.EnumControls(this);
        }

        private void NumAdjContrast_ValueChanged(object sender, EventArgs e)
        {
            OCRCollection.OnContrast((float)numAdjContrast.Value);
        }

        protected override void OnLoad(EventArgs e)
        {
            //base.OnLoad(e);
            //Initial();

            cboOCRName.SelectedIndex = 0;
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
            numLight = numericUpDown1;
            numAdjContrast = numericUpDown2;
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

            grpOCRData = groupBox2;

            cboOCRName = comboBox1;
            txtOCRName = textBox1;
            txtRemoark = textBox2;
            cboCamera = comboBox2;

            OCRUI = ocrUI1;
            
            InitialOCRName();
            InitialCamera();
            InitialOCRUI();

            FillDisplay();

            DBStatus = DBStatusEnum.NONE;

            numLight.ValueChanged += NumLight_ValueChanged;

            //Universal.mLanguage.EnumControls(this);
        }

        private void NumLight_ValueChanged(object sender, EventArgs e)
        {
            int index =cboCamera .SelectedIndex;
            CCDCollection.SetExposureToSetOCR((float)numLight.Value, index, true);
        }

        void InitialOCRName()
        {
            cboOCRName.Items.Clear();
            
            string[] strs = OCRCollection.ToOCRComboItem();

            foreach(string str in strs)
            {
                cboOCRName.Items.Add(str);
            }

            cboOCRName.SelectedIndexChanged += cboOCRName_SelectedIndexChanged;
            //cboOCRName.SelectedIndex = 0;
        }
        void RefreshOCRName()
        {
            IsNeedToChange = false;

            cboOCRName.Items.Clear();

            string[] strs = OCRCollection.ToOCRComboItem();

            foreach (string str in strs)
            {
                cboOCRName.Items.Add(str);
            }

            cboOCRName.SelectedIndex = OCRCollection.FindIndex(DataNow.No);

            IsNeedToChange = true;
        }

        void InitialOCRUI()
        {
            

        }

        private void cboOCRName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboOCRName.SelectedIndex == 0)
                btnDel.Enabled = false;
            else
                btnDel.Enabled = true;

            if (!IsNeedToChange)
                return;
            
            OCRCollection.GotoIndex(cboOCRName.SelectedIndex);
            FillDisplay();

            int index = cboOCRName.SelectedIndex;
    //        CCDCollection.SetExposure((float)numLight.Value, index);
        }

        void InitialCamera()
        {
            int i = 0;

            cboCamera.Items.Clear();

            while (i < iCCDCount)
            {
                cboCamera.Items.Add("CAM" + i.ToString("00"));
                i++;
            }
            if (iCCDCount != 0)
                cboCamera.SelectedIndex = 0;
        }
        private void btn_Click(object sender, EventArgs e)
        {
            TagEnum KEY = (TagEnum)((Button)sender).Tag;

            switch (KEY)
            {
                case TagEnum.ADD:
                    Add(OCRClass.OrgOCRNoString, false);
                    break;
                case TagEnum.COPY:
                    Add(DataNow.OCRNoString, true);
                    break;
                case TagEnum.MODIFY:
                    OCRCollection.DataNow.Backup();
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
                    Bitmap bmp = null;
                    //if (TransferFoOCR.TransferClass.ISDEBUG)
                    //    bmp = GetBitmap();
                    //else
                    //bmp = TransferFoOCR.TransferClass.GetBitmap(cboCamera.SelectedIndex);
                    //switch (OPTION)
                    //{
                    //    case OptionEnum.R32:
                    //        CCDCollection.GetR32Image(cboCamera.SelectedIndex.ToString());
                    //        break;
                    //    case OptionEnum.R26:
                    //        CCDCollection.GetR26Image(cboCamera.SelectedIndex.ToString());
                    //        break;
                    //    case OptionEnum.R15:
                    //        CCDCollection.GetR15Image(cboCamera.SelectedIndex.ToString());
                    //        break;
                    //    case OptionEnum.R9:
                    //        CCDCollection.GetR9Image(cboCamera.SelectedIndex.ToString());
                    //        break;
                    //}
                    if (isDebug)
                    {
                        OpenFileDialog dialog = new OpenFileDialog();
                        dialog.Multiselect = true;//该值确定是否可以选择多个文件
                        dialog.Title = "请选择文件";
                        dialog.Filter = "png格式（*.png）|*.png|所有文件|*.*";
                        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            string file = dialog.FileName;
                            Bitmap bmp1 = new Bitmap(file);
                            Bitmap bmpTemp = new Bitmap(bmp1);
                            bmp1.Dispose();

                            OCRUI.SHOWDISP.SetDisplayImage(bmpTemp);
                        }
                    }
                    else
                    {

                        bmp = CCDCollection.GetBMP(cboCamera.SelectedIndex, true);
                        if (bmp != null)
                            OCRCollection.DataNow.bmpLast = bmp.Clone() as Bitmap;
                        OCRUI.SHOWDISP.SetDisplayImage(bmp);
                    }
                    break;
            }

        }

        Bitmap GetBitmap()
        {
            Bitmap bmp = null;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;//该值确定是否可以选择多个文件
            dialog.Title = "请选择文件夹";
            dialog.Filter = "所有文件(*.png)|*.png;*.bmp;*.jpg";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = dialog.FileName;

                Bitmap bmptemp = new Bitmap(file);
                bmp = new Bitmap(bmptemp);
                bmptemp.Dispose();
            }
            return bmp; 
        }

        int DataFromNo = 0;

        void Add(string rcpnostring,bool iscopy)
        {
            DataFromNo = DataNow.No;
            OCRCollection.Add(rcpnostring, iscopy);
            
            FillDisplay();
            
            DBStatus = DBStatusEnum.ADD;
        }

        void Cancel()
        {
            if (DBStatus == DBStatusEnum.ADD)
            {
                OCRCollection.DeleteLast(DataFromNo);
            }
            else
                RestoreBack();
            
            FillDisplay();

            DBStatus = DBStatusEnum.NONE;
            OCRUI.SetEnable(false);
            OCRUI.Restore();
        }
        void OK()
        {
            if (OCRCollection.CheckIsDuplicate(txtOCRName.Text, DataNow.No))
            {
                MessageBox.Show("名稱重復，請確認。", "SYS", MessageBoxButtons.OK);
                txtOCRName.Focus();

                return;
            }
            
            WriteBack(true);

            RefreshOCRName();

            FillDisplay();
            DBStatus = DBStatusEnum.NONE;
            OCRUI.SetEnable(false);

            OCRUI.Restore();
        }

        void Exit()
        {
            this.Close();

        }
        void Del()
        {
            if (MessageBox.Show("是否要刪除此筆資料?", "SYS", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int cboLast = cboOCRName.SelectedIndex;

                OCRCollection.Delete(cboLast);

                IsNeedToChange = false;
                cboOCRName.Items.RemoveAt(cboLast);
                IsNeedToChange = true;

                if (cboLast == cboOCRName.Items.Count)
                    cboLast--;

                cboOCRName.SelectedIndex = cboLast;

            }


        }
        void WriteBack(bool iswithchange)
        {
            if (iswithchange)
            {
                DataNow.Name = txtOCRName.Text;
                DataNow.Remark = txtRemoark.Text;

                DataNow.ModifyDatetime = JzTimes.DateTimeString;
            }

            OCRCollection.Save(DataNow.No);
        }
        public void RestoreBack()
        {
            RestoreBack(true);
        }
        public void RestoreBack(bool isneedload)
        {
            OCRCollection.DataNow.Restore();
        }
        
        void FillDisplay()
        {
            IsNeedToChange = false;

            txtOCRName.Text = OCRCollection.DataNow.Name;
            if (txtOCRName.Text == "SYSTEM")
                txtOCRName.Enabled = false;
            else
                txtOCRName.Enabled = true;
            txtRemoark.Text = OCRCollection.DataNow.Remark;

            lblModifyDateTime.Text = OCRCollection.DataNow.ToModifyString();

            OCRUI.Initial(OCRCollection,mOCRByPaddle);
            
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

                        grpOCRData.Enabled = true;

                        btnAdd.Visible = false;
                        btnCopy.Visible = false;
                        btnModify.Visible = false;
                        btnDel.Visible = false;
                        cboOCRName.Visible = false;

                        btnOK.Visible = true;
                        btnCancel.Visible = true;

                        btnLeave.Visible = false;

                        OCRUI.SetEnable(true);

                        break;
                    case DBStatusEnum.NONE:
                        grpOCRData.Enabled = false;

                        btnAdd.Visible = true;
                        btnCopy.Visible = true;
                        btnModify.Visible = true;
                        btnDel.Visible = true;
                        cboOCRName.Visible = true;

                        btnOK.Visible = false;
                        btnCancel.Visible = false;

                        btnLeave.Visible = true;

                        OCRUI.SetDefaultView();
                        OCRUI.SetEnable(false);

                        DataNow.Reset();

                        break;
                }
            }
        }
    }
}
