using JetEazy.BasicSpace;
using JetEazy.ControlSpace.MotionSpace;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace JetEazy.UISpace
{
    public enum TagMotorSpeed
    {
        COUNT=4,
        SPEED_AUTO=0,
        SPEED_HAND=1,
        SPEED_HOME_H=2,
        SPEED_HOME_L=3,
    }

    public partial class AxisSpeedUI : UserControl
    {
        NumericUpDown[] num = new NumericUpDown[(int)TagMotorSpeed.COUNT];
        Label[] lbl=new Label[(int)TagMotorSpeed.COUNT];
        Button[] btn=new Button[(int)TagMotorSpeed.COUNT];

        const int MSDuriation = 100;

        VersionEnum VERSION;
        OptionEnum OPTION;

        GeoMotionClass MOTION;
        JzTimes myTime;

        GroupBox grpMotorSpeed;

        bool IsNeedToChange = false;

        public AxisSpeedUI()
        {
            InitializeComponent();
            //InitialInternal();
        }
        void InitialInternal()
        {
            grpMotorSpeed = groupBox1;

            num[(int)TagMotorSpeed.SPEED_AUTO] = numericUpDown1;
            num[(int)TagMotorSpeed.SPEED_HAND] = numericUpDown2;
            num[(int)TagMotorSpeed.SPEED_HOME_H] = numericUpDown3;
            num[(int)TagMotorSpeed.SPEED_HOME_L] = numericUpDown4;

            lbl[(int)TagMotorSpeed.SPEED_AUTO] = label2;
            lbl[(int)TagMotorSpeed.SPEED_HAND] = label3;
            lbl[(int)TagMotorSpeed.SPEED_HOME_H] = label5;
            lbl[(int)TagMotorSpeed.SPEED_HOME_L] = label7;

            btn[(int)TagMotorSpeed.SPEED_AUTO] = button1;
            btn[(int)TagMotorSpeed.SPEED_HAND] = button2;
            btn[(int)TagMotorSpeed.SPEED_HOME_H] = button3;
            btn[(int)TagMotorSpeed.SPEED_HOME_L] = button4;

            int i = 0;
            while(i<(int)TagMotorSpeed.COUNT)
            {
                num[i].Maximum = 9999999999;
                num[i].Minimum = 0;

                num[i].Tag = (TagMotorSpeed)i;
                lbl[i].Tag = (TagMotorSpeed)i;
                btn[i].Tag = (TagMotorSpeed)i;
                btn[i].Click += btn_Click;

                i++;
            }

            myTime = new JzTimes();
            myTime.Cut();

            FillDisplay();
        }

        private void btn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            TagMotorSpeed _tag = (TagMotorSpeed)btn.Tag;

            switch(_tag)
            {
                case TagMotorSpeed.SPEED_AUTO:
                    MOTION.GOSPEED = (int)num[(int)_tag].Value;
                    ((PLCMotionClass)MOTION).SaveData(MotionAddressEnum.GOSPEED, MOTION.GOSPEED.ToString());
                    MOTION.SetSpeed(SpeedTypeEnum.GO);
                    break;
                case TagMotorSpeed.SPEED_HAND:
                    MOTION.MANUALSPEED = (int)num[(int)_tag].Value;
                    ((PLCMotionClass)MOTION).SaveData(MotionAddressEnum.MANUALSPEED, MOTION.MANUALSPEED.ToString());
                    MOTION.SetSpeed(SpeedTypeEnum.MANUAL);
                    break;
                case TagMotorSpeed.SPEED_HOME_H:
                    MOTION.HOMEHIGHSPEED = (int)num[(int)_tag].Value;
                    ((PLCMotionClass)MOTION).SaveData(MotionAddressEnum.HOMEHIGHSPEED, MOTION.HOMEHIGHSPEED.ToString());
                    MOTION.SetSpeed(SpeedTypeEnum.HOMEHIGH);
                    break;
                case TagMotorSpeed.SPEED_HOME_L:
                    MOTION.HOMESLOWSPEED = (int)num[(int)_tag].Value;
                    ((PLCMotionClass)MOTION).SaveData(MotionAddressEnum.HOMESLOWSPEED, MOTION.HOMESLOWSPEED.ToString());
                    MOTION.SetSpeed(SpeedTypeEnum.HOMESLOW);
                    break;
            }

            OnTrigger(_tag, "btnClick");
        }

        public void Initial(GeoMotionClass motion, VersionEnum version, OptionEnum option)
        {
            VERSION = version;
            OPTION = option;

            MOTION = motion;

            InitialInternal();

            //if (MOTION == null)
            //    return;

            //Set Default Speed
            //MOTION.SetSpeed(SpeedTypeEnum.HOMESLOW);
            //MOTION.SetSpeed(SpeedTypeEnum.HOMEHIGH);
            //MOTION.SetSpeed(SpeedTypeEnum.MANUAL);
            //MOTION.SetSpeed(SpeedTypeEnum.GO);
        }
        //public void Initial(MACH motion, VersionEnum version, OptionEnum option)
        //{
        //    VERSION = version;
        //    OPTION = option;

        //    MOTION = motion;

        //    InitialInternal();

        //    //if (MOTION == null)
        //    //    return;

        //    //Set Default Speed
        //    //MOTION.SetSpeed(SpeedTypeEnum.HOMESLOW);
        //    //MOTION.SetSpeed(SpeedTypeEnum.HOMEHIGH);
        //    //MOTION.SetSpeed(SpeedTypeEnum.MANUAL);
        //    //MOTION.SetSpeed(SpeedTypeEnum.GO);
        //}

        public void FillDisplay()
        {
            if (MOTION == null)
                return;

            IsNeedToChange = false;
           
            grpMotorSpeed.Text = MOTION.MOTIONALIAS.ToString();

            num[(int)TagMotorSpeed.SPEED_AUTO].Value = MOTION.GetSpeed(SpeedTypeEnum.GO);
            num[(int)TagMotorSpeed.SPEED_HAND].Value = MOTION.GetSpeed(SpeedTypeEnum.MANUAL);
            num[(int)TagMotorSpeed.SPEED_HOME_H].Value = MOTION.GetSpeed(SpeedTypeEnum.HOMEHIGH);
            num[(int)TagMotorSpeed.SPEED_HOME_L].Value = MOTION.GetSpeed(SpeedTypeEnum.HOMESLOW);

            IsNeedToChange = true;
        }

        public void Tick()
        {
            if (myTime.msDuriation < MSDuriation)
                return;

            if (MOTION == null)
                return;

            lbl[(int)TagMotorSpeed.SPEED_AUTO].Text = MOTION.GetSpeed(SpeedTypeEnum.GO).ToString();
            lbl[(int)TagMotorSpeed.SPEED_HAND].Text = MOTION.GetSpeed(SpeedTypeEnum.MANUAL).ToString();
            lbl[(int)TagMotorSpeed.SPEED_HOME_H].Text = MOTION.GetSpeed(SpeedTypeEnum.HOMEHIGH).ToString();
            lbl[(int)TagMotorSpeed.SPEED_HOME_L].Text = MOTION.GetSpeed(SpeedTypeEnum.HOMESLOW).ToString();

            myTime.Cut();

        }

        public delegate void TriggerHandler(TagMotorSpeed action, string opstr);
        public event TriggerHandler TriggerAction;
        public void OnTrigger(TagMotorSpeed action, string opstr)
        {
            if (TriggerAction != null)
            {
                TriggerAction(action, opstr);
            }
        }
    }
}
