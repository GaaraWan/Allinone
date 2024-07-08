using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using JetEazy;
using JetEazy.ControlSpace;
using JetEazy.DBSpace;
using JetEazy.BasicSpace;
using Allinone.OPSpace;
using Allinone.UISpace.ALBUISpace;

using JzDisplay.UISpace;

namespace Allinone.UISpace.RCPUISpace
{
    public partial class RcpV1UI : UserControl
    {
        enum TagEnum
        {
            COPY,
            MODIFY,

            OK,
            CANCEL,
            CAPTURE,
            CALIBRATION,
        }

        //Language Setup
        JzLanguageClass myLanguage = new JzLanguageClass();
        string UIPath = "";
        int LanguageIndex = 0;

        RcpDBClass RCPDB;
        AlbumCollectionClass ALBCOLLECTION;

        AlbumClass AlbumNow
        {
            get
            {
                return ALBCOLLECTION.AlbumNow;
            }

        }

        VersionEnum VERSION = VersionEnum.STEROPES;
        OptionEnum OPTION = OptionEnum.MAIN;

        string ShowBmpString;

        AlbUI ALBUI;

        GroupBox grpRcpData;

        TextBox txtName;
        TextBox txtVersion;

        Button btnCopy;
        Button btnModify;

        Button btnOK;
        Button btnCancel;
        Label lblModifyDateTime;
        bool IsTriggerFirstTime = true;
        public RcpV1UI()
        {
            InitializeComponent();
            InitialInternal();
        }
        public RcpClass DataNow
        {
            get
            {
                return RCPDB.DataNow;
            }
        }
        protected void InitialInternal()
        {
            grpRcpData = groupBox1;

            txtName = textBox1;
            txtVersion = textBox2;

            btnCopy = button1;
            btnCopy.Tag = TagEnum.COPY;
            btnModify = button7;
            btnModify.Tag = TagEnum.MODIFY;
            btnOK = button8;
            btnOK.Tag = TagEnum.OK;
            btnCancel = button9;
            btnCancel.Tag = TagEnum.CANCEL;

            btnCopy.Click += new EventHandler(btn_Click);
            btnModify.Click += new EventHandler(btn_Click);

            btnOK.Click += new EventHandler(btn_Click);
            btnCancel.Click += new EventHandler(btn_Click);

            lblModifyDateTime = label4;

            ALBUI = albUI1;
            //STPUI.Initial();
            //STPUI.TriggerAction += new StpUI.TriggerHandler(STPUI_TriggerAction);

            //FillDisplay(true);

            //DBStatus = DBStatusEnum.NONE;
        }
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        public void Initial(string uipath,
            int langindex,
            VersionEnum ver,
            OptionEnum opt,
            RcpDBClass rcpdb,
            AlbumCollectionClass albcollection,
            string showbmpstring)
        {
            RCPDB = rcpdb;
            ALBCOLLECTION = albcollection;

            UIPath = uipath;
            LanguageIndex = langindex;
            VERSION = ver;
            OPTION = opt;

            if (IsTriggerFirstTime)
            {
                ALBUI.Initial(VERSION, OPTION, ALBCOLLECTION.AlbumNow);
                myLanguage.Initial(UIPath + "\\RcpUI.jdb", LanguageIndex, this);
                ALBUI.TriggerAction += ALBUI_TriggerAction;

                IsTriggerFirstTime = false;
            }

            ShowBmpString = showbmpstring;

            FillDisplay();

            DBStatus = DBStatusEnum.NONE;

        }
        private void ALBUI_TriggerAction(RCPStatusEnum status, string opstr)
        {
            OnTrigger(status, opstr);

            //switch (status)
            //{
            //    case RCPStatusEnum.SETPOSITION:
            //    case RCPStatusEnum.SETEND:
            //        OnTrigger(RCPStatusEnum.SETPOSITION);
            //        break;
            //    case RCPStatusEnum.SHOWDETAIL:
            //        OnTrigger(RCPStatusEnum.SHOWDETAIL);
            //        break;
            //    case RCPStatusEnum.SHOWRELATE:
            //        OnTrigger(RCPStatusEnum.SHOWRELATE);
            //        break;
            //}
        }
        public void SetPosition(string posstr)
        {
            ALBUI.SetPosition(posstr);
        }
        
        //void STPUI_TriggerAction(RCPStatusEnum status)
        //{
        //    switch (status)
        //    {
        //        case RCPStatusEnum.ASSIGNOK:
        //            OpDisplay.SetDispImage(VIEW.bmpORIGIN);

        //            break;
        //        default:
        //            OnTrigger(status);
        //            break;
        //    }
        //}

        public void SetLanguage()
        {
            myLanguage.SetControlLanguage(this, INI.LANGUAGE);
        }
        public bool Disable
        {
            set
            {
                this.Enabled = !value;
                this.Visible = !value;
            }
        }
        void btn_Click(object sender, EventArgs e)
        {
            TagEnum KEY = (TagEnum)((Button)sender).Tag;

            switch (KEY)
            {
                //case TagEnum.ADD:
                //    Add(RcpClass.ORGRCPNOSTRING);
                //    break;
                case TagEnum.COPY:
                    Add(DataNow.RcpNoString);
                    break;
                case TagEnum.MODIFY:
                    DBStatus = DBStatusEnum.MODIFY;
                    OnTrigger(RCPStatusEnum.EDIT, "");
                    break;
                case TagEnum.CANCEL:
                    Cancel();
                    break;
                case TagEnum.OK:
                    OK();
                    break;
            }
        }
        int RCPFromNo = 0;

        void Add(string fromrcpnostr)
        {
            RCPFromNo = DataNow.No;
            RCPDB.Add();

            //新增或是複製時，需要先複製資料到預設的目錄去
            AlbumClass album = new AlbumClass(DataNow, ALBCOLLECTION.RCPPATH, fromrcpnostr, true);
            ALBCOLLECTION.Add(album);

            DBStatus = DBStatusEnum.ADD;
            FillDisplay();
            OnTrigger(RCPStatusEnum.EDIT, "");
        }
        void Cancel()
        {
            if (DBStatus == DBStatusEnum.ADD)
            {
                RCPDB.DeleteLast(RCPFromNo);
                ALBCOLLECTION.DelLast(RCPFromNo);
            }

            //在新增或是複製時會把自己原有的刪掉，要檢查一下並救回來
            if (!ALBCOLLECTION.CheckIndex(RCPFromNo))
            {
                AlbumClass album = new AlbumClass(DataNow, ALBCOLLECTION.RCPPATH, DataNow.RcpNoString, false);
                ALBCOLLECTION.Add(album);
            }

            //RestoreBack();

            DBStatus = DBStatusEnum.NONE;
            OnTrigger(RCPStatusEnum.MODIFYCANCEL, "");

            FillDisplay();
        }
        void OK()
        {
            if (RCPDB.CheckIsDuplicate(txtName.Text, DataNow.No,txtVersion.Text))
            {
                MessageBox.Show(myLanguage.Messages("msg2", LanguageIndex), "SYS", MessageBoxButtons.OK);
                txtName.Focus();

                return;
            }

            WriteBack(true);

            DBStatus = DBStatusEnum.NONE;
            OnTrigger(RCPStatusEnum.MODIFYCOMPLETE, "");


            FillDisplay();
        }

        public void RestoreBack()
        {
            RestoreBack(true);
        }
        public void RestoreBack(bool isneedload)
        {
            if (isneedload)
                ALBCOLLECTION.AlbumNow.Load();

            ALBUI.SetAlbum(ALBCOLLECTION.AlbumNow);
        }
        void WriteBack(bool iswithchange)
        {
            if (iswithchange)
            {
                DataNow.Name = txtName.Text;
                DataNow.Version = txtVersion.Text;

                DataNow.ModifyDatetime = JzTimes.DateTimeString;
            }

            ALBUI.WriteBack();

            Universal.BackupDATADB();
            RCPDB.Save();
        }
        void FillDisplay()
        {
            txtName.Text = DataNow.Name;
            txtVersion.Text = DataNow.Version;
            lblModifyDateTime.Text = DataNow.ToShortModifyString();

            txtName.ReadOnly = DataNow.No == 1;
            //DISPUI.SetDisplayImage(Universal.RCPPATH + "\\" + DataNow.RcpNoString + "\\" + ShowBmpString);
        }

        public DBStatusEnum GetDBStatus()
        {
            return DBStatus;
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

                        grpRcpData.Enabled = true;
                        
                        btnCopy.Visible = false;
                        btnModify.Visible = false;

                        btnOK.Visible = true;
                        btnCancel.Visible = true;

                        break;
                    case DBStatusEnum.NONE:
                        grpRcpData.Enabled = false;
                        
                        btnCopy.Visible = true;
                        btnModify.Visible = true;

                        btnOK.Visible = false;
                        btnCancel.Visible = false;

                        break;
                }
            }
        }

        public delegate void TriggerHandler(RCPStatusEnum status, string opstr);
        public event TriggerHandler TriggerAction;
        public void OnTrigger(RCPStatusEnum status, string opstr)
        {
            if (TriggerAction != null)
            {
                TriggerAction(status, opstr);
            }
        }


    }
}
