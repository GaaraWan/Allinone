using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using JetEazy;
using JetEazy.FormSpace;
using JetEazy.BasicSpace;
using JetEazy.DBSpace;
using JetEazy.ControlSpace;
using JetEazy.AccountMgr;
using JetEazy.QUtilities;
using System.Diagnostics;


namespace JetEazy.UISpace
{
    public partial class EssUI : UserControl
    {
        protected enum TagEnum
        {
            LOGIN,
            LOGOUT,

            ACCOUNTMANAGEMENT,
            RECIPESELECTION,

            CLEARPASS,
            CLEARFAIL,

            FASTCAL,

            RUN,
            RECIPE,
            SETUP,
            
            CHANGERECIPE,
        }
        protected enum StyleEnum
        {
            COMBOBOX,
            BUTTONSELECT,
        }

        LayoutEnum LAYOUT
        {
            get
            {
                return Universal.LAYOUT;
            }
        }

        //Language Setup
        JzLanguageClass myLanguage = new JzLanguageClass();

        PictureBox picExit;

        Button btnLanguage;

        protected Button btnLogin;
        protected Button btnLogout;

        protected Label lblDateTime;
        protected Label lblAccountName;
        protected Label lblMainStatus;
        protected Label lblRecipeName;

        protected Label lblPassCount;
        protected Label lblFailCount;
        protected Label lblAllCount;
        protected Label lblBCNGCount;
        protected Label lblVer;

        protected Button btnAccountManagement;

        protected Button btnClearPass;
        protected Button btnClearFail;

        protected Button btnRun;
        public Button btnRecipe;
        protected Button btnSetup;

        protected Button btnFastCal;
        
        protected ComboBox cboChangeRecipe;
        protected Button btnRcpSelect;

        bool IsNeedToChange = true;
        /// <summary>
        /// 判断是否在RUN的状态 如果在则开始计时 否则停止计时
        /// </summary>
        bool m_IsRunState = false;
        bool m_IsRunTempState = false;
        Stopwatch m_RunWatch = new Stopwatch();
        int m_RunWatchTime = 30;

        public int RunWatchTime
        {
            get { return m_RunWatchTime; }
            set { m_RunWatchTime = value; }
        }

        JzTimes myTimes = new JzTimes();
        
        AccDBClass ACCDB;
        EsssDBClass ESSDB;
        string UIPath = "";
        int LanguageIndex = 0;

        VersionEnum VERSION;
        OptionEnum OPTION;

        int SamplingTimems = 1000;

        public EssUI()
        {
           // InitializeComponent();
            //switch (LAYOUT)
            //{
            //    case LayoutEnum.L1280X800:
                    InitializeComponent();
                //    break;
                //case LayoutEnum.L1440X900:
              //    InitializeComponent1440X900();
                //    break;
            //}

            InitialInternal();
        }

        protected void InitialInternal()
        {
            picExit = pictureBox1;

            lblDateTime = label2;
            lblAccountName = label3;
            lblMainStatus = label4;
            lblRecipeName = label5;

            lblPassCount = label8;
            lblFailCount = label9;
            lblVer = label10;
            lblBCNGCount = label11;
            lblAllCount = label12;


            lblAllCount.Text = "";
            lblBCNGCount.Text = "";

            btnLanguage = button11;
            btnLanguage.Click += BtnLanguage_Click;

            btnLogin = button2;
            btnLogin.Tag = TagEnum.LOGIN;
            btnLogout = button1;
            btnLogout.Tag = TagEnum.LOGOUT;

            btnAccountManagement = button3;
            btnAccountManagement.Tag = TagEnum.ACCOUNTMANAGEMENT;

            btnClearPass = button5;
            btnClearPass.Tag = TagEnum.CLEARPASS;
            btnClearFail = button6;
            btnClearFail.Tag = TagEnum.CLEARFAIL;

            btnRun = button7;
            btnRun.Tag = TagEnum.RUN;
            btnRecipe = button8;
            btnRecipe.Tag = TagEnum.RECIPE;
            btnSetup = button9;
            btnSetup.Tag = TagEnum.SETUP;

            cboChangeRecipe = comboBox1;
            cboChangeRecipe.Tag = TagEnum.CHANGERECIPE;

            btnRcpSelect = button4;
            btnRcpSelect.Tag = TagEnum.CHANGERECIPE;

            btnLogin.Click += new EventHandler(btn_Click);
            btnLogout.Click += new EventHandler(btn_Click);

            btnAccountManagement.Click += new EventHandler(btn_Click);

            btnClearPass.Click += new EventHandler(btn_Click);
            btnClearFail.Click += new EventHandler(btn_Click);

            btnRun.Click += new EventHandler(btn_Click);
            btnRecipe.Click += new EventHandler(btn_Click);
            btnSetup.Click += new EventHandler(btn_Click);
            
            btnRcpSelect.Click+= new EventHandler(btn_Click);

            btnFastCal = button10;
            btnFastCal.Tag = TagEnum.FASTCAL;
            btnFastCal.Click += new EventHandler(btn_Click);

            cboChangeRecipe.SelectedIndexChanged += new EventHandler(cbo_SelectedIndexChanged);

            picExit.DoubleClick += new EventHandler(picExit_DoubleClick);
        }

        private void BtnLanguage_Click(object sender, EventArgs e)
        {
            LanguageForm language = new LanguageForm();
            language.ShowDialog();
            OnTrigger(ESSStatusEnum.LANGUAGE);
        }

        public void Initial(EsssDBClass essdb,
            AccDBClass accdb,
            string uipath,
            int langindex,
            VersionEnum ver,
            OptionEnum opt,
            int samplingtimems,
            string strVer="")
        {
            ESSDB = essdb;
            ACCDB = accdb;
            UIPath = uipath;
            LanguageIndex = langindex;
            VERSION = ver;
            OPTION = opt;

            myLanguage.Initial(UIPath + "\\ESSUI.jdb", LanguageIndex, this);

            switch (VERSION)
            {
                case VersionEnum.KBAOI:
                    switch(OPTION)
                    {
                        case OptionEnum.MAIN:
                            SetStyle(StyleEnum.BUTTONSELECT);
                            break;
                    }
                    break;
                case VersionEnum.ALLINONE:
                case VersionEnum.AUDIX:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_SDM5:
                        case OptionEnum.MAIN_SERVICE:
                        case OptionEnum.MAIN_SDM3:
                        case OptionEnum.MAIN_SDM2:
                        case OptionEnum.MAIN_SDM1:
                        case OptionEnum.MAIN_X6:
                        case OptionEnum.MAIN_SD:
                        case OptionEnum.MAIN:
                        case OptionEnum.NOIO:
                            SetStyle(StyleEnum.BUTTONSELECT);
                            break;
                        case OptionEnum.MAIN_DFLY:
                            SetStyle(StyleEnum.BUTTONSELECT);
                            break;
                        case OptionEnum.R26:
                        case OptionEnum.R32:
                        case OptionEnum.R15:
                        case OptionEnum.R9:
                        case OptionEnum.R5:
                        case OptionEnum.R3:
                        case OptionEnum.C3:
                        case OptionEnum.R1:
                            SetStyle(StyleEnum.BUTTONSELECT);
                            break;
                    }
                    break;
            }

            LOGINStatus = ESSStatusEnum.LOGOUT;
            MAINStatus = ESSStatusEnum.RUN;

            FillDisplay();

            SetRun(false);

            SamplingTimems = samplingtimems;

            myTimes.Cut();
            lblVer.Text = strVer;
            lblVer.Visible = true;

            pictureBox1.Controls.Add(lblVer);

            //pictureBox1.BackgroundImage.Save("D:\\Esslogo.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

        }

        public Button GetbtnLogin
        {
            get
            {
                return btnLogin;
            }
        }
        public Button GetbtnAccountManagement
        {
            get
            {
                return btnAccountManagement;
            }
        }
        public Label GetAccountName
        {
            get
            {
                return lblAccountName;
            }
        }

        void cbo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            TagEnum TAG = (TagEnum)((ComboBox)sender).Tag;

            switch (TAG)
            {
                case TagEnum.CHANGERECIPE:
                    
                    string [] selectedrecipe = cboChangeRecipe.Text.Split(']');
 
                    selectedrecipe[0] = selectedrecipe[0].Replace("[","");
                    ESSDB.DataNow.LastRecipeNo = int.Parse(selectedrecipe[0]);

                    ESSDB.Save();

                    OnTrigger(ESSStatusEnum.RECIPESELECTED);
                    break;
            }

        }

        Color m_lblAccountNameColor = Color.Yellow;
        public void Tick()
        {
            if (myTimes.msDuriation > SamplingTimems)
            {
                lblDateTime.Text = JzTimes.DateTimeString;

                myTimes.Cut();
            }


            switch(VERSION)
            {
                case VersionEnum.ALLINONE:

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_SDM2:
                        case OptionEnum.MAIN_X6:

                            m_IsRunState = lblMainStatus.Text == "RUN";
                            if (m_IsRunState && m_RunWatchTime > 0)
                            {
                                if (m_RunWatchTime < 10)
                                    m_RunWatchTime = 10;

                                if (!m_IsRunTempState)
                                {
                                    if (!string.IsNullOrEmpty(lblAccountName.Text) && lblAccountName.Text.ToUpper() != "OP")
                                    {
                                        m_IsRunTempState = true;
                                        m_RunWatch.Restart();
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(lblAccountName.Text) && lblAccountName.Text.ToUpper() != "OP")
                                    {
                                        if (lblAccountName.BackColor != Color.Yellow)
                                            lblAccountName.BackColor = Color.Yellow;
                                        else
                                            lblAccountName.BackColor = Color.FromArgb(255, 255, 192);

                                        if (m_RunWatch.ElapsedMilliseconds > m_RunWatchTime * 1000)
                                        {
                                            m_IsRunTempState = false;
                                            m_RunWatch.Stop();

                                            lblAccountName.BackColor = Color.FromArgb(255, 255, 192);

                                            ACCDB.Index = -1;
                                            LOGINStatus = ESSStatusEnum.LOGOUT;
                                            MAINStatus = ESSStatusEnum.LOGOUT;

                                        }
                                    }
                                    else
                                    {
                                        m_IsRunTempState = false;
                                        m_RunWatch.Stop();

                                        lblAccountName.BackColor = Color.FromArgb(255, 255, 192);
                                    }
                                }
                            }
                            else
                            {
                                m_RunWatch.Stop();
                                m_IsRunTempState = false;
                                lblAccountName.BackColor = Color.FromArgb(255, 255, 192);
                            }

                            break;
                    }

                    break;
            }

        }
        public void SetLanguage(int langindex)
        {
            myLanguage.SetControlLanguage(this, langindex);
        }

        void picExit_DoubleClick(object sender, EventArgs e)
        {
            if (LOGINStatus != ESSStatusEnum.LOGOUT)
                return;

            if (MessageBox.Show(ToChangeLanguageCode("EssUI.msg1"), "SYS", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                OnTrigger(ESSStatusEnum.EXIT);
            }
        }
        void btn_Click(object sender, EventArgs e)
        {
            TagEnum TAG = (TagEnum)((Button)sender).Tag;

            switch (TAG)
            {
                case TagEnum.LOGIN:
                    Login();
                    break;
                case TagEnum.LOGOUT:
                   
                    Logout();
                    break;
                case TagEnum.ACCOUNTMANAGEMENT:
                    AccountManagement();
                    break;
                case TagEnum.CLEARPASS:
                    ClearPass();
                    break;
                case TagEnum.CLEARFAIL:
                    ClearFail();
                    break;
                case TagEnum.RUN:
                    MAINStatus = ESSStatusEnum.RUN;
                    OnTrigger(MAINStatus);
                    break;
                case TagEnum.RECIPE:
                    MAINStatus = ESSStatusEnum.RECIPE;
                    OnTrigger(MAINStatus);
                    break;
                case TagEnum.SETUP:
                    MAINStatus = ESSStatusEnum.SETUP;
                    OnTrigger(MAINStatus);
                    break;
                case TagEnum.FASTCAL:
                    OnTrigger(ESSStatusEnum.FASTCAL);
                    break;
                case TagEnum.CHANGERECIPE:
                    OnTrigger(ESSStatusEnum.CHANGERECIPE);
                    break;
            }
        }

        public void ShowPLC_RxTime(string str)
        {
            lblVer.Text = str;
            lblVer.Refresh();
        }
        public void SetCheatTag(bool IsCheat)
        {
            btnClearFail.Text = (IsCheat ? "O" : "0");
        }

        public void SetRecipeName(string rcpname)
        {
            lblRecipeName.Text = rcpname.Substring(0,Math.Min(rcpname.Length,20));
        }

        #region Button Fuctions
        void Login()
        {
            OnTrigger(ESSStatusEnum.LOGIN);
        }
        void Logout()
        {
            if (MessageBox.Show(ToChangeLanguageCode("EssUI.msg2"), "SYS", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ACCDB.Index = -1;
                LOGINStatus = ESSStatusEnum.LOGOUT;
                MAINStatus = ESSStatusEnum.LOGOUT;
            }
        }
        public void ClearPass()
        {
            ESSDB.Reset(true);
            FillDisplay();
        }
        public void ClearFail()
        {
            ESSDB.Reset(false);
            FillDisplay();
        }
        public void SetPass(bool ISCUT)
        {
            ESSDB.SetPassNG(true);
            if(ISCUT)
                ESSDB.SetFAILCUT();
            
            FillDisplay();
        }
        public void SetPass()
        {
            ESSDB.SetPassNG(true);
            FillDisplay();
        }

        public void SetNG(bool isADD)
        {
            if (isADD)
                ESSDB.SetPassNG(false);
            FillDisplay();
        }
        public void SetNG()
        {
            ESSDB.SetPassNG(false);
            FillDisplay();
        }
        void AccountManagement()
        {
            OnTrigger(ESSStatusEnum.ACCOUNTMANAGE);
        }

        #endregion

        public void FillDisplay()
        {
            lblAccountName.Text = ACCDB.DataNow.Name;
            lblPassCount.Text = ESSDB.DataNow.PassCount.ToString();
            lblFailCount.Text = ESSDB.DataNow.NGCount.ToString();

            
        }

        public void FillBCCOUNT(int iAllCount,int iBcNGCount)
        {
            lblAllCount.Text = iAllCount.ToString();
            lblBCNGCount.Text = iBcNGCount.ToString();
        }

        public void Set111(string filestr)
        {
            Bitmap bmp = new Bitmap(filestr);
            
            picExit.Image = new Bitmap(bmp);

            bmp.Dispose();
        }
        public string AccountName
        {
            get
            {
                return lblAccountName.Text;
            }
        }

        public ESSStatusEnum LoginStatus
        {
            get
            {
                return LOGINStatus;
            }
        }

        ESSStatusEnum myLOGINStatus = ESSStatusEnum.LOGOUT;
        ESSStatusEnum LOGINStatus
        {
            get
            {
                return myLOGINStatus;
            }
            set
            {
                myLOGINStatus = value;
                
                switch (myLOGINStatus)
                {
                    case ESSStatusEnum.LOGIN:

                        btnLogin.Visible = false;
                        btnLogout.Visible = true;

                        lblAccountName.Text = ACCDB.DataNow.Name;
                        btnAccountManagement.Enabled = ACCDB.DataNow.AllowManageAccount;
                        
                        break;
                    case ESSStatusEnum.LOGOUT:

                        btnLogin.Visible = true;
                        btnLogout.Visible = false;

                        ACCDB.GotoIndex(-1);
                        lblAccountName.Text = ACCDB.DataNow.Name;
                        btnAccountManagement.Enabled = false;

                        break;
                }
            }
        }

        ESSStatusEnum LastMainStatus = ESSStatusEnum.RUN;
        public bool Disable
        {
            set
            {
                this.Enabled = !value;

                if (value)
                {
                    if (MAINStatus == ESSStatusEnum.EDIT)
                        return;

                    LastMainStatus = MAINStatus;
                    MAINStatus = ESSStatusEnum.EDIT;
                }
                else
                {
                    MAINStatus = LastMainStatus;
                }
            }
        }
        public void FocusButton()
        {
            btnFastCal.Focus();
        }

        public void SetMainLoginStatus(ESSStatusEnum status)
        {
            LOGINStatus = status;

        }

        public void login()
        {

            btnRun.BackColor = JzToolsClass.UsedColor;
            btnRecipe.BackColor = JzToolsClass.NormalColor;
            btnSetup.BackColor = JzToolsClass.NormalColor;

            btnRun.Enabled = true;
            btnRecipe.Enabled = true;
            btnSetup.Enabled = true;

            if (LOGINStatus == ESSStatusEnum.LOGIN)
                btnFastCal.Enabled = true;
            if (LoginStatus == ESSStatusEnum.LOGOUT)
                btnFastCal.Enabled = false;

            btnRcpSelect.Enabled = true;

            lblAccountName.Text = "Admin";

        }
        public void SetMainStatus(ESSStatusEnum status)
        {
            MAINStatus = status;
        }
        public ESSStatusEnum GetMainStatus()
        {
            return MAINStatus;
        }
        public void SetRecipeCombo(List<string> recipelist)
        {
            int i = 0;
            int selectedindex = 0;

            IsNeedToChange = false;

            cboChangeRecipe.Items.Clear();

            foreach (string str in recipelist)
            {
                string[] strs = str.Split('?');

                cboChangeRecipe.Items.Add(strs[0]);

                if (int.Parse(strs[1]) == ESSDB.DataNow.LastRecipeNo)
                    selectedindex = i;

                i++;
            }

            cboChangeRecipe.SelectedIndex = selectedindex;

            IsNeedToChange = true;
        }
        public void SetRun(bool isrun)
        {
            btnFastCal.Enabled = isrun;
        }

        ESSStatusEnum myMAINStatus = ESSStatusEnum.RUN;
        ESSStatusEnum MAINStatus
        {
            get
            {
                return myMAINStatus;
            }
            set
            {
                if(myMAINStatus == ESSStatusEnum.EDIT && (value == ESSStatusEnum.RECIPE || value == ESSStatusEnum.SETUP))
                {
                    OnTrigger(ESSStatusEnum.CHECKLIVE);
                }
                else if (myMAINStatus == ESSStatusEnum.SETUP && (value == ESSStatusEnum.EDIT))
                {
                    OnTrigger(ESSStatusEnum.SHOWSETUP);
                }


                myMAINStatus = value;

                OnTrigger(myMAINStatus);

                lblMainStatus.Text = myMAINStatus.ToString();

                //m_IsRunState = myMAINStatus == ESSStatusEnum.RUN;

                switch (myMAINStatus)
                {
                    case ESSStatusEnum.RUN:

                        btnRun.BackColor = JzToolsClass.UsedColor;
                        btnRecipe.BackColor = JzToolsClass.NormalColor;
                        btnSetup.BackColor = JzToolsClass.NormalColor;

                        btnRun.Enabled = true;
                        btnRecipe.Enabled = ACCDB.DataNow.AllowSetupRecipe;
                        btnSetup.Enabled = ACCDB.DataNow.AllowSetup;

                        if (LOGINStatus == ESSStatusEnum.LOGIN)
                            btnFastCal.Enabled = true;
                        if(LoginStatus== ESSStatusEnum.LOGOUT)
                            btnFastCal.Enabled = false;

                        btnRcpSelect.Enabled = true;

                        break;
                    case ESSStatusEnum.RECIPE:

                        btnRun.BackColor = JzToolsClass.NormalColor;
                        btnRecipe.BackColor = JzToolsClass.UsedColor;
                        btnSetup.BackColor = JzToolsClass.NormalColor;
                        
                        btnRun.Enabled = true;
                        btnRecipe.Enabled = true;
                        btnSetup.Enabled = ACCDB.DataNow.AllowSetup;
                        btnFastCal.Enabled = false;

                        btnRcpSelect.Enabled = true;

                        break;
                    case ESSStatusEnum.SETUP:

                        btnRun.BackColor = JzToolsClass.NormalColor;
                        btnRecipe.BackColor = JzToolsClass.NormalColor;
                        btnSetup.BackColor = JzToolsClass.UsedColor;
                        
                        btnRun.Enabled = true;
                        btnRecipe.Enabled = ACCDB.DataNow.AllowSetupRecipe;
                        btnSetup.Enabled = true;
                        btnFastCal.Enabled = false;

                        btnRcpSelect.Enabled = false;

                        break;
                    case ESSStatusEnum.EDIT:

                        btnRun.Enabled = false;
                        btnRecipe.Enabled = false;
                        btnSetup.Enabled = false;
                        btnFastCal.Enabled = false;
                        
                        btnRcpSelect.Enabled = false;

                        break;
                }
            }
        }

        protected void SetStyle(StyleEnum style)
        {
            switch (style)
            {
                case StyleEnum.COMBOBOX:

                    btnRcpSelect.Visible = false;
                    cboChangeRecipe.Visible = true;

                    lblRecipeName.Width = 222;
                    cboChangeRecipe.Width = 227;
                    cboChangeRecipe.Location = new Point(1, 170);
                    
                    
                    break;
                case StyleEnum.BUTTONSELECT:

                    btnRcpSelect.Visible = true;
                    cboChangeRecipe.Visible = false;

                    lblRecipeName.Width = 160;

                    break;

            }

        }

        //當 MainStatus 變化時，產生OnTrigger
        public delegate void TriggerHandler(ESSStatusEnum status);
        public event TriggerHandler TriggerAction;
        public void OnTrigger(ESSStatusEnum status)
        {
            if (TriggerAction != null)
            {
                TriggerAction(status);
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
