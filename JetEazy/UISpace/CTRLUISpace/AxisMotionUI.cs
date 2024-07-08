using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using JetEazy.BasicSpace;
using JetEazy.ControlSpace;
using JetEazy.ControlSpace.MotionSpace;

namespace JetEazy.UISpace
{
    public partial class AxisMotionUI : UserControl
    {
        const int MSDuriation = 100;

        VersionEnum VERSION;
        OptionEnum OPTION;


        LayoutEnum LAYOUT
        {
            get
            {
                return Universal.LAYOUT;
            }
        }


        Button btnForward;
        Button btnBackward;
        Label lblPositionNow;

        Button btnHome;

        NumericUpDown numGoPosition;

        Button btnAdd;
        Button btnSub;

        Button btnGo;

        CheckBox chkIsSlow;

        Button btnSetReadyPosition;
        Button btnGoReadyPosition;
        Label lblReadyPosition;
        Label lblReload;

        JzTimes myTime;
        
        PLCMotionClass PLCMOTION;
        CanMotionClass CANMOTION;

        GeoMotionClass MOTION;

        GroupBox grpMotion;

        //Status Labels
        Label lblBreak;
        Label lblSVOn;
        Label lblError;
        Label lblUpperLimit;
        Label lblLowerLimit;

        Label lblRulerDescription;
        Label lblRulerPositionNow;

        bool IsNeedToChange = false;
        
        public AxisMotionUI()
        {
            switch(LAYOUT)
            {
                case LayoutEnum.L1280X800:
                    InitializeComponent1280X800();
                    break;
                default:
                    InitializeComponent1440X900();
                    break;
            }
        }

        void InitialInternal()
        {
            numGoPosition = numericUpDown1;

            btnForward = button5;
            btnBackward = button4;

            btnHome = button17;

            btnAdd = button11;
            btnSub = button10;
            btnGo = button12;

            btnSetReadyPosition = button3;
            btnGoReadyPosition = button7;

            grpMotion = groupBox1;
            lblReload = label26;

            btnForward.MouseDown += new MouseEventHandler(btnForward_MouseDown);
            btnForward.MouseUp += new MouseEventHandler(btnForward_MouseUp);
            btnBackward.MouseDown += new MouseEventHandler(btnBackward_MouseDown);
            btnBackward.MouseUp += new MouseEventHandler(btnBackward_MouseUp);

            btnHome.Click += new EventHandler(btnHome_Click);

            btnAdd.Click += new EventHandler(btnAdd_Click);
            btnSub.Click += new EventHandler(btnSub_Click);
            btnGo.Click += new EventHandler(btnGo_Click);

            btnSetReadyPosition.Click += new EventHandler(btnSetReadyPosition_Click);
            btnGoReadyPosition.Click += new EventHandler(btnGoReadyPosition_Click);
            lblReload.DoubleClick += LblReload_DoubleClick;

            chkIsSlow = checkBox1;
            chkIsSlow.CheckedChanged += new EventHandler(chkIsSlow_CheckedChanged);

            lblPositionNow = label23;
            lblReadyPosition = label25;

            lblRulerDescription = label7;
            lblRulerPositionNow = label6;

            lblBreak = label4;
            lblSVOn = label1;
            lblError = label5;
            lblUpperLimit = label3;
            lblLowerLimit = label2;

            lblSVOn.DoubleClick += LblSVOn_DoubleClick;
            lblBreak.DoubleClick += LblBreak_DoubleClick;
            lblError.DoubleClick += LblError_DoubleClick;
            
            myTime = new JzTimes();
            myTime.Cut();

            FillDisplay();
        }

        public void Initial(GeoMotionClass motion,VersionEnum version,OptionEnum option,bool isupdown = true)
        {
            VERSION = version;
            OPTION = option;
            
            MOTION = motion;

            InitialInternal();

            if(isupdown)
            {
                btnForward.Text = "";
                btnBackward.Text =  "";
            }
            else
            {
                btnForward.Text = "";
                btnBackward.Text = "";
            }

            if (MOTION == null)
                return;

            //Set Default Speed
            //MOTION.SetSpeed(SpeedTypeEnum.HOMESLOW);
            //MOTION.SetSpeed(SpeedTypeEnum.HOMEHIGH);
            MOTION.SetSpeed(SpeedTypeEnum.MANUAL);
            MOTION.SetSpeed(SpeedTypeEnum.GO);
        }
        private void LblError_DoubleClick(object sender, EventArgs e)
        {
            MOTION.Reset();
        }

        private void LblBreak_DoubleClick(object sender, EventArgs e)
        {
            MOTION.Break();
        }

        private void LblSVOn_DoubleClick(object sender, EventArgs e)
        {
            MOTION.SVOn();
        }

        private void LblReload_DoubleClick(object sender, EventArgs e)
        {
            if(MessageBox.Show("是否要載入原始設定?","SYSTEM",MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                MOTION.LoadData();

                FillDisplay();
            }
        }

        void btnGoReadyPosition_Click(object sender, EventArgs e)
        {
            MOTION.Go(MOTION.READYPOSITION);
        }

        void btnSetReadyPosition_Click(object sender, EventArgs e)
        {
            MOTION.READYPOSITION = MOTION.PositionNow;
            lblReadyPosition.Text = MOTION.READYPOSITION.ToString("0.000");

            MOTION.SaveData();
        }

        void chkIsSlow_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;


            if (chkIsSlow.Checked)
            {
                MOTION.SetSpeed(SpeedTypeEnum.MANUALSLOW);
                MOTION.SetSpeed(SpeedTypeEnum.GOSLOW);
            }
            else
            {
                MOTION.SetSpeed(SpeedTypeEnum.MANUAL);
                MOTION.SetSpeed(SpeedTypeEnum.GO);
            }
        }
        void ResetSlow()
        {
            chkIsSlow.Checked = false;
            MOTION.SetSpeed(SpeedTypeEnum.MANUAL);
        }
        void btnBackward_MouseUp(object sender, MouseEventArgs e)
        {
            MOTION.Stop();
        }

        void btnBackward_MouseDown(object sender, MouseEventArgs e)
        {
            MOTION.Backward();
        }

        void btnForward_MouseUp(object sender, MouseEventArgs e)
        {
            MOTION.Stop();
        }

        void btnForward_MouseDown(object sender, MouseEventArgs e)
        {
            MOTION.Forward();
        }
        void btnGo_Click(object sender, EventArgs e)
        {
            MOTION.Go((float)numGoPosition.Value);
        }
        void btnSub_Click(object sender, EventArgs e)
        {
            MOTION.Go(MOTION.PositionNow - (float)numGoPosition.Value);
        }
        void btnAdd_Click(object sender, EventArgs e)
        {
            MOTION.Go((float)numGoPosition.Value + MOTION.PositionNow);
        }
        void btnHome_Click(object sender, EventArgs e)
        {
            MOTION.Home();
        }

        public void FillDisplay()
        {
            if (MOTION == null)
                return;

            IsNeedToChange = false;

            switch(OPTION)
            {
                case OptionEnum.MAIN_SD:
                    grpMotion.Text = MOTION.MOTIONALIAS.ToString();
                    break;
                default:
                    grpMotion.Text = MOTION.MOTIONNAME.ToString();
                    break;
            }

            lblReadyPosition.Text = MOTION.READYPOSITION.ToString("0.000");
            //grpMotion.Text = MOTION.MOTIONNAME.ToString();

            lblBreak.Visible = MOTION.IsHaveBreakOption;
            lblSVOn.Visible = MOTION.IsHaveSVOnOption;

            lblRulerDescription.Visible = MOTION.IsHaveRulerOption;
            lblRulerPositionNow.Visible = MOTION.IsHaveRulerOption;
           
            ResetSlow();

            IsNeedToChange = true;
        }
        
        public void Tick()
        {
            if (myTime.msDuriation < MSDuriation)
                return;

            if (MOTION == null)
                return;

            btnHome.BackColor = (MOTION.IsHome ? SystemColors.Control : Color.Red);
            btnGo.BackColor = (MOTION.IsOK ? SystemColors.Control : Color.Red);
            btnAdd.BackColor = (MOTION.IsOK ? SystemColors.Control : Color.Red);
            btnSub.BackColor = (MOTION.IsOK ? SystemColors.Control : Color.Red);
            btnBackward.BackColor = (MOTION.IsOK ? SystemColors.Control : Color.Red);
            btnForward.BackColor = (MOTION.IsOK ? SystemColors.Control : Color.Red);

            switch(OPTION)
            {
                case OptionEnum.MAIN_SD:
                    lblUpperLimit.BackColor = (!MOTION.IsReachUpperBound ? Color.Green : Color.Black);
                    lblLowerLimit.BackColor = (!MOTION.IsReachLowerBound ? Color.Green : Color.Black);
                    break;
                default:
                    lblUpperLimit.BackColor = (MOTION.IsReachUpperBound ? Color.Green : Color.Black);
                    lblLowerLimit.BackColor = (MOTION.IsReachLowerBound ? Color.Green : Color.Black);
                    break;
            }

            

            if (lblBreak.Visible)
                lblBreak.BackColor = (MOTION.IsBreack ? Color.Red : Color.Black);
            if (lblSVOn.Visible)
                lblSVOn.BackColor = (MOTION.IsSVOn ? Color.Green : Color.Black);
            if (lblError.Visible)
                lblError.BackColor = (MOTION.IsError ? Color.Red : Color.Black);

            if (lblRulerPositionNow.Visible)
                lblRulerPositionNow.Text = MOTION.RulerPositionNowString;

            lblPositionNow.Text = MOTION.PositionNowString;

            myTime.Cut();

        }
        ////當 MainStatus 變化時，產生OnTrigger
        //public delegate void TriggerHandler(StatusEnum Status);
        //public event TriggerHandler TriggerAction;
        //public void OnTrigger(StatusEnum Status)
        //{
        //    if (TriggerAction != null)
        //    {
        //        TriggerAction(Status);
        //    }
        //}


    }
}
