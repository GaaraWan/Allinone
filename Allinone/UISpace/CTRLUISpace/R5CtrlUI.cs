using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using JetEazy;
using JetEazy.BasicSpace;
using JetEazy.UISpace;
using Allinone.ControlSpace;
using Allinone.ControlSpace.MachineSpace;
using Allinone.ControlSpace.IOSpace;

namespace Allinone.UISpace
{
    public partial class R5CtrlUI : UserControl
    {
        enum TagEnum
        {
            /// <summary>
            /// 頂燈
            /// </summary>
            TOPLIGHT,
            /// <summary>
            ///四周燈管
            /// </summary>
            ARROUNDLIGHT,
            /// <summary>
            /// 鐳雕平板燈
            /// </summary>
            PANNELLIGHT,
            /// <summary>
            /// 神燈
            /// </summary>
            GODLIGHT,
            /// <summary>
            /// 小圓燈
            /// </summary>
            CIRCLELIGHT,
            /// <summary>
            /// 高跷的灯
            /// </summary>
            STILTSLIGHT,
            RED,
            GREEN,
            YELLOW,
            BLUE,
            WHITE,

            /// <summary>
            /// 切换机种
            /// </summary>
            MaxBox,
            /// <summary>
            /// 重置
            /// </summary>
            MinBox,

            SET,
        }


        const int MSDuriation = 10;

        JzTransparentPanel tpnlCover;
        VersionEnum VERSION = VersionEnum.ALLINONE;
        OptionEnum OPTION = OptionEnum.MAIN;
        JzR5MachineClass MACHINE;
        Label lblShow;
        JzR5IOClass PLCIO
        {
            get
            {
                return MACHINE.PLCIO;
            }
        }

        Label lblIsStart;
        Label lblIsUPSError;

        Label lblTopLight;
        Label lblAroundLight;
        Label lblPannelLight;
        Label lblGodLight;
        Label lblCircleLight;
        Label lblRed;
        Label lblGreen;
        Label lblYellow;
        Label lblBlue;
        Label lblWhite;
        Label lblSTILTSLIGHT;
        Label lblMaxBox;
        Label lblMinBox;
        Label lblSET;
        JzTimes myJzTimer = new JzTimes();

        ListBox lbAlarm;
        public R5CtrlUI()
        {
            InitializeComponent();
            InitialInternal();
        }

        void InitialInternal()
        {
            tpnlCover = jzTransparentPanel1;

            lblIsStart = label1;
            lblIsUPSError = label4;

            lblTopLight = label2;
            lblAroundLight = label3;
            lblPannelLight = label5;
            lblGodLight = label6;
            lblCircleLight = label7;
            lblRed = label8;
            lblGreen = label9;
            lblYellow = label10;
            lblBlue = label14;
            lblWhite = label13;
            lblSTILTSLIGHT = label11;

            lblMaxBox = label12;
            lblMinBox = label15;

            lblSET = label16;
            lblShow = label17;
            lbAlarm = listBox1;

            lblMaxBox.Tag = TagEnum.MaxBox;
            lblMinBox.Tag = TagEnum.MinBox;
            lblTopLight.Tag = TagEnum.TOPLIGHT;
            lblAroundLight.Tag = TagEnum.ARROUNDLIGHT;
            lblPannelLight.Tag = TagEnum.PANNELLIGHT;
            lblGodLight.Tag = TagEnum.GODLIGHT;
            lblCircleLight.Tag = TagEnum.CIRCLELIGHT;
            lblRed.Tag = TagEnum.RED;
            lblGreen.Tag = TagEnum.GREEN;
            lblYellow.Tag = TagEnum.YELLOW;
            lblBlue.Tag = TagEnum.BLUE;
            lblWhite.Tag = TagEnum.WHITE;
            lblSTILTSLIGHT.Tag = TagEnum.STILTSLIGHT;
            lblSET.Tag = TagEnum.SET;

            lblTopLight.DoubleClick += lbl_DoubleClick;
            lblAroundLight.DoubleClick += lbl_DoubleClick;
            lblPannelLight.DoubleClick += lbl_DoubleClick;
            lblGodLight.DoubleClick += lbl_DoubleClick;
            lblCircleLight.DoubleClick += lbl_DoubleClick;
            lblRed.DoubleClick += lbl_DoubleClick;
            lblGreen.DoubleClick += lbl_DoubleClick;
            lblYellow.DoubleClick += lbl_DoubleClick;
            lblBlue.DoubleClick += lbl_DoubleClick;
            lblWhite.DoubleClick += lbl_DoubleClick;
            lblSTILTSLIGHT.DoubleClick += lbl_DoubleClick;
            lblMaxBox.DoubleClick += lbl_DoubleClick;
            lblMinBox.DoubleClick += lbl_DoubleClick;
            lblSET.DoubleClick += lbl_DoubleClick;
        }

        private void lbl_DoubleClick(object sender, EventArgs e)
        {
            TagEnum KEYS = (TagEnum)((Label)sender).Tag;

            switch (KEYS)
            {
                case TagEnum.TOPLIGHT:
                    PLCIO.TopLight = !PLCIO.TopLight;
                    break;
                case TagEnum.ARROUNDLIGHT:
                    PLCIO.ArroundLight = !PLCIO.ArroundLight;
                    break;
                case TagEnum.PANNELLIGHT:
                    PLCIO.PannelLight = !PLCIO.PannelLight;
                    break;
                case TagEnum.GODLIGHT:
                    PLCIO.GodLight = !PLCIO.GodLight;
                    break;
                case TagEnum.CIRCLELIGHT:
                    PLCIO.CircleLight = !PLCIO.CircleLight;
                    break;
                case TagEnum.STILTSLIGHT:
                    PLCIO.StiltsLight = !PLCIO.StiltsLight;
                    break;
                case TagEnum.RED:
                    PLCIO.Red = !PLCIO.Red;
                    break;
                case TagEnum.GREEN:
                    PLCIO.Green = !PLCIO.Green;
                    break;
                case TagEnum.YELLOW:
                    PLCIO.Yellow = !PLCIO.Yellow;
                    break;
                case TagEnum.BLUE:
                    //PLCIO.Blue = !PLCIO.Blue;
                    break;
                case TagEnum.WHITE:
                    if (INI.ISHIVECLIENT)
                        MACHINE.MachineStateForPlannedDown = !PLCIO.White;
                    //PLCIO.White = !PLCIO.White;
                    break;

                case TagEnum.MaxBox:
                    PLCIO.MAXCOMPER = true;
                    break;
                case TagEnum.MinBox:
                    PLCIO.MINCOMPER = true;
                    //  PLCIO.RESET = true;// !PLCIO.RESET;
                    break;
                case TagEnum.SET:
                    if (INI.isR5_MOTOR_TO_Rs485)
                    {
                        Universal.mSetMotor.Show();
                        Universal.mSetMotor.TopMost = true;
                    }
                    else
                    {
                        Allinone.FormSpace.R5_IO_Test_Form firm = new FormSpace.R5_IO_Test_Form();
                        firm.Show();
                    }
                    break;
            }

        }

        public void Initial(VersionEnum version, OptionEnum option, JzR5MachineClass machine)
        {
            VERSION = version;
            OPTION = option;
            MACHINE = machine;

            tpnlCover.Location = new Point(0, 0);
            tpnlCover.Size = new Size(336, 303);

            myJzTimer.Cut();

            if (!INI.isR5_MOTOR_TO_Rs485)
                LoadAlarm();

        }

        public void SetEnable(bool isenable)
        {
            tpnlCover.Visible = !isenable;

            this.BackColor = (isenable ? SystemColors.Control : Color.DarkGray);
        }
        public void Tick()
        {
            if (myJzTimer.msDuriation < MSDuriation)
                return;

            MACHINE.Tick();

            lblIsStart.BackColor = (PLCIO.IsStart ? Color.Green : Color.Black);
            lblIsUPSError.BackColor = (PLCIO.IsUPSError ? Color.Red : Color.Black);

            lblTopLight.BackColor = (PLCIO.TopLight ? Color.Green : Color.Black);
            lblAroundLight.BackColor = (PLCIO.ArroundLight ? Color.Green : Color.Black);
            lblPannelLight.BackColor = (PLCIO.PannelLight ? Color.Green : Color.Black);
            lblGodLight.BackColor = (PLCIO.GodLight ? Color.Green : Color.Black);
            lblCircleLight.BackColor = (PLCIO.CircleLight ? Color.Green : Color.Black);
            lblSTILTSLIGHT.BackColor = (PLCIO.StiltsLight ? Color.Green : Color.Black);

            lblRed.BackColor = (PLCIO.Red ? Color.Red : Color.Black);
            lblYellow.BackColor = (PLCIO.Yellow ? Color.Yellow : Color.Black);
            lblGreen.BackColor = (PLCIO.Green ? Color.Green : Color.Black);
            lblYellow.ForeColor = lblYellow.BackColor == Color.Yellow ? Color.Black : Color.Yellow;
            lblBlue.BackColor = (PLCIO.Blue ? Color.Blue : Color.Black);
            lblWhite.BackColor = (PLCIO.White ? Color.White : Color.Black);

            lblMaxBox.BackColor = (PLCIO.MAXCOMPER ? Color.Green : Color.Black);
            lblMinBox.BackColor = (PLCIO.MINCOMPER ? Color.Green : Color.Black);

            if (!INI.isR5_MOTOR_TO_Rs485)
                UpDataAlarm();
            myJzTimer.Cut();
        }

        List<string> ListAlarm = new List<string>();
        void LoadAlarm()
        {
            string path = Universal.WORKPATH + "\\" + Universal.OPTION.ToString() + "\\Alarm.csv";

            if (System.IO.File.Exists(path))
            {
                JzToolsClass jzTools = new JzToolsClass();
                string data = "";
                jzTools.ReadData(ref data, path);

                data = data.Replace(Environment.NewLine, "#");
                string[] mydatatemp = data.Split('#');
                for (int i = 0; i < mydatatemp.Length; i++)
                {
                    ListAlarm.Add("");
                    if (mydatatemp[i] != "")
                    {
                        string[] temp = mydatatemp[i].Split(',');
                        if (temp.Length > 1)
                            ListAlarm[i] = temp[1];
                        else
                            ListAlarm[i] = "";
                    }
                }
            }
        }

        void UpDataAlarm()
        {
            lblShow.Text = "PLC复位步号:" + PLCIO.PLCResetNO + " 跑线步号:" + PLCIO.PLCRunNO;
            int iAlarm = PLCIO.ALARM;

            string strAlarm = System.Convert.ToString(iAlarm, 2);
            strAlarm = strAlarm.PadLeft(16, '0'); //= string.Format("{0:d16}", strAlarm);

            List<string> alarmList = new List<string>();
            int ij = 0;
            for (int i = strAlarm.Length-1;i>=0; i--)
            {
                if (strAlarm[i] == '1')
                {
                    alarmList.Add(ij.ToString());
                }

                ij++;
            }

            if(PLCIO.PLCReset)
                alarmList.Add("PLC 正在复位中...");
            if (PLCIO.PLCProcess)
                alarmList.Add("PLC 正在跑流程中...");

            if (alarmList.Count == lbAlarm.Items.Count)
            {
                for (int i = 0; i < alarmList.Count; i++)
                {
                    for (int j = 0; j < ListAlarm.Count; j++)
                    {
                        if (j.ToString() == alarmList[i])
                            lbAlarm.Items[i] = ListAlarm[j];
                    }
                }
            }
            else
            {
                lbAlarm.Items.Clear();
                for (int i = 0; i < alarmList.Count; i++)
                {
                    for (int j = 0; j < ListAlarm.Count; j++)
                    {
                        if (j.ToString() == alarmList[i])
                            lbAlarm.Items.Add(ListAlarm[j]);
                    }
                }
            }
            if (PLCIO.PLCReset)
            {
                bool ishave = false;
                foreach(string str in lbAlarm.Items)
                {
                    if (str == "PLC 正在复位中...")
                        ishave = true;
                }
                if(!ishave)
                lbAlarm.Items.Add("PLC 正在复位中...");
            }
                
            if (PLCIO.PLCProcess)
            {
                bool ishave = false;
                foreach (string str in lbAlarm.Items)
                {
                    if (str == "PLC 正在跑流程中...")
                        ishave = true;
                }
                if (!ishave)
                    lbAlarm.Items.Add("PLC 正在跑流程中...");
            }
        }
    }
}
