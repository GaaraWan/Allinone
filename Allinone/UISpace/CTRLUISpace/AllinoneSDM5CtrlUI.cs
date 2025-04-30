using JetEazy.BasicSpace;
using JetEazy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Allinone.ControlSpace.MachineSpace;
using Allinone.FormSpace.Motor;
using JetEazy.FormSpace;
using Allinone.ControlSpace.IOSpace;
using JetEazy.ControlSpace;
using JetEazy.DBSpace;
using JetEazy.PlugSpace;
using System.Threading;

namespace Allinone.UISpace.CTRLUISpace
{
    public partial class AllinoneSDM5CtrlUI : UserControl
    {
        VersionEnum VERSION;
        OptionEnum OPTION;
        JzTransparentPanel tpnlCover;
        JzTimes myTime;

        Button btnStart;
        Button btnStop;
        Button btnReset;
        Button btnClearAlm;
        Button btnMute;
        Button btnAutoAndManual;
        Button btnOnekeyGetImage;

        Button btnBigProduct;
        Button btnSmallProduct;
        Button btnSaveImage;
        Button btnManualChangeRecipe;

        Button btnOneKeyGetImageAreaCam;
        Button btnSaveImageAreaCam;
        Button btnReLoadTrainRecipe;

        Label lblAXIS;
        Label lblTestPoint;
        Label lblIO;

        Label lblVacc;
        Label lblByPassDoor;

        Label lblAlarm;
        Label lblState;

        ListBox lsbEvent;

        JzMainSDM5MachineClass MACHINE;

        System.Threading.Thread m_ThreadPlc = null;
        bool m_ThRunning = false;

        AccDBClass ACCDB
        {
            get
            {
                return Universal.ACCDB;
            }
        }

        MessageForm M_WARNING_FRM = null;

        public AllinoneSDM5CtrlUI()
        {
            InitializeComponent();
        }

        public void Initial(VersionEnum version, OptionEnum option, JzMainSDM5MachineClass machine)
        {
            VERSION = version;
            OPTION = option;
            MACHINE = machine;

            btnStart = button2;
            btnStop = button4;
            btnReset = button1;

            btnClearAlm = button8;
            btnMute = button7;
            btnAutoAndManual = button5;
            btnOnekeyGetImage = button3;

            btnBigProduct = button9;
            btnSmallProduct = button10;
            btnSaveImage = button6;
            btnManualChangeRecipe = button11;
            btnOneKeyGetImageAreaCam = button12;
            btnSaveImageAreaCam = button13;
            btnReLoadTrainRecipe = button14;

            lblAlarm = label5;
            lblState = label11;
            lsbEvent = listBox1;

            btnStart.Click += BtnStart_Click;
            btnStop.Click += BtnStop_Click;
            btnReset.Click += BtnReset_Click;
            btnClearAlm.Click += BtnClearAlm_Click;
            btnMute.Click += BtnMute_Click;
            btnAutoAndManual.Click += BtnAutoAndManual_Click;
            btnOnekeyGetImage.Click += BtnOnekeyGetImage_Click;
            btnBigProduct.Click += BtnBigProduct_Click;
            btnSmallProduct.Click += BtnSmallProduct_Click;
            btnSaveImage.Click += BtnSaveImage_Click;
            btnManualChangeRecipe.Click += BtnManualChangeRecipe_Click;
            btnOneKeyGetImageAreaCam.Click += BtnOneKeyGetImageAreaCam_Click;
            btnSaveImageAreaCam.Click += BtnSaveImageAreaCam_Click;
            btnReLoadTrainRecipe.Click += BtnReLoadTrainRecipe_Click;

            //lblVacc = label4;
            lblIO = label3;
            //lblLIGHT = label1;
            lblAXIS = label2;
            //lblCamDpiSetup = label1;
            //lblTestPoint = label1;
            //lblTestPoint.Visible = false;
            //numericUpDown1.Visible = false;
            lblByPassDoor = label1;

            //tpnlCover = new JzTransparentPanel();
            //tpnlCover.BackColor = System.Drawing.Color.Transparent;
            //tpnlCover.Location = new System.Drawing.Point(6, 30);
            //tpnlCover.Name = "panel1";
            //tpnlCover.Size = this.Size;
            //tpnlCover.TabIndex = 0;
            //this.Controls.Add(tpnlCover);
            //tpnlCover.BringToFront();

            //lblLIGHT.DoubleClick += LblLIGHT_DoubleClick;
            lblAXIS.DoubleClick += LblAXIS_DoubleClick;
            //lblCamDpiSetup.DoubleClick += LblCamDpiSetup_DoubleClick;
            //lblCamDpiSetup.Visible = false;
            lblIO.DoubleClick += LblIO_DoubleClick;
            //btnByPassDoor.Click += BtnByPassDoor_Click;
            lblByPassDoor.DoubleClick += LblByPassDoor_DoubleClick;

            //lblTestPoint.DoubleClick += LblTestPoint_DoubleClick;
            //lblIO.Visible = false;

            MACHINE.EVENT.Initial(lsbEvent);
            MACHINE.EVENT.Initial(lblAlarm);

            MACHINE.TriggerAction += MACHINE_TriggerAction;
            MACHINE.EVENT.TriggerAlarm += EVENT_TriggerAlarm;

            myTime = new JzTimes();
            myTime.Cut();

            SetEnable(false);

            if (Universal.IsUseThreadReviceTcp)
            {
                if (m_ThreadPlc == null)
                {
                    m_ThRunning = true;
                    m_ThreadPlc = new System.Threading.Thread(new System.Threading.ThreadStart(PlcTick));
                    m_ThreadPlc.IsBackground = true;
                    m_ThreadPlc.Start();
                }
            }
        }

        private void BtnReLoadTrainRecipe_Click(object sender, EventArgs e)
        {
            int iret = Universal.InitialChangeRecipe();
            MessageBox.Show("重新加载完成");
        }

        private void BtnSaveImageAreaCam_Click(object sender, EventArgs e)
        {
            string _path = SaveFilePicker($"AreaImage_{DateTime.Now.ToString("yyyyMMddHHmmss")}.png");
            if (string.IsNullOrEmpty(_path))
                return;

            Bitmap bmptemp = new Bitmap(CamActClass.Instance.bmpChangeRecipeTemp);
            bmptemp.Save(_path, System.Drawing.Imaging.ImageFormat.Png);
            bmptemp.Dispose();
            M_WARNING_FRM = new MessageForm(true, "图像保存完成", "");
            if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
            {
            }
            M_WARNING_FRM.Close();
            M_WARNING_FRM.Dispose();
        }

        private void BtnOneKeyGetImageAreaCam_Click(object sender, EventArgs e)
        {
            OnTrigger(ActionEnum.ACT_ONEKEYGETIMAGEAREA, "");
        }

        private void BtnManualChangeRecipe_Click(object sender, EventArgs e)
        {
            OnTrigger(ActionEnum.ACT_MANUALCHANGERECIPE, "");
        }

        private void BtnSaveImage_Click(object sender, EventArgs e)
        {
            string _path = SaveFilePicker($"Image_{DateTime.Now.ToString("yyyyMMddHHmmss")}.png");
            if (string.IsNullOrEmpty(_path))
                return;

            Bitmap bmptemp = new Bitmap(CamActClass.Instance.GetImage(0));
            bmptemp.Save(_path, System.Drawing.Imaging.ImageFormat.Png);
            bmptemp.Dispose();
            M_WARNING_FRM = new MessageForm(true, "图像保存完成", "");
            if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
            {
            }
            M_WARNING_FRM.Close();
            M_WARNING_FRM.Dispose();


        }

        string SaveFilePicker(string DefaultName)
        {
            string retStr = "";

            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = "PNG Files (*.png)|*.PNG|" + "All files (*.*)|*.*";
            //dlg.Filter = DefaultPath;
            dlg.FileName = DefaultName;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                retStr = dlg.FileName;
            }
            return retStr;
        }

        private void BtnSmallProduct_Click(object sender, EventArgs e)
        {
            MACHINE.PLCIO.SetValue(0, "D216");
        }

        private void BtnBigProduct_Click(object sender, EventArgs e)
        {
            MACHINE.PLCIO.SetValue(10, "D216");
        }

        private void BtnOnekeyGetImage_Click(object sender, EventArgs e)
        {
            OnTrigger(ActionEnum.ACT_ONEKEYGETIMAGE, "");
        }

        #region ALARMS

        bool IsAlarmsSeriousX = false;
        bool IsAlarmsCommonX = false;

        void SetSeriousAlarms0()
        {
            foreach (PLCAlarmsItemDescriptionClass item in MACHINE.PLCIO.PLCALARMS[(int)AlarmsEnum.ALARMS_ADR_SERIOUS].PLCALARMSDESCLIST)
            {
                if (MACHINE.PLCIO.GetAlarmsAddress(item.BitNo, item.ADR_Address))
                {
                    MACHINE.EVENT.GenEvent("A0001", EventActionTypeEnum.AUTOMATIC, item.ADR_Chinese, ACCDB.DataNow);
                }
            }
        }
        void SetSeriousAlarms1()
        {
            foreach (PLCAlarmsItemDescriptionClass item in MACHINE.PLCIO.PLCALARMS[(int)MainSDAlarmsEnum.ALARMS_ADR_SERIOUS1].PLCALARMSDESCLIST)
            {
                if (MACHINE.PLCIO.GetAlarmsAddress(item.BitNo, item.ADR_Address))
                {
                    MACHINE.EVENT.GenEvent("A0001", EventActionTypeEnum.AUTOMATIC, item.ADR_Chinese, ACCDB.DataNow);
                }
            }
        }
        void SetCommonAlarms()
        {
            foreach (PLCAlarmsItemDescriptionClass item in MACHINE.PLCIO.PLCALARMS[(int)AlarmsEnum.ALARMS_ADR_COMMON].PLCALARMSDESCLIST)
            {
                if (MACHINE.PLCIO.GetAlarmsAddress(item.BitNo, item.ADR_Address))
                {
                    MACHINE.EVENT.GenEvent("A0002", EventActionTypeEnum.AUTOMATIC, item.ADR_Chinese, ACCDB.DataNow);
                }
            }
        }

        void SetNormalLight()
        {
            //MACHINE.PLCIO.Red = false;
            //MACHINE.PLCIO.Yellow = true;
            //MACHINE.PLCIO.Green = false;
        }
        void SetAbnormalLight()
        {
            //MACHINE.PLCIO.Red = true;
            //MACHINE.PLCIO.Yellow = false;
            //MACHINE.PLCIO.Green = false;
        }
        void SetRunningLight()
        {
            //MACHINE.PLCIO.Red = false;
            //MACHINE.PLCIO.Yellow = false;
            //MACHINE.PLCIO.Green = true;
        }
        void SetBuzzer(bool IsON)
        {
            //USEIO.Buzzer = IsON;
            MACHINE.PLCIO.ADR_BUZZER = IsON;
        }



        #endregion

        private void EVENT_TriggerAlarm(bool IsBuzzer)
        {
            //MACHINE.PLCIO.ADR_BUZZER = IsBuzzer;
            //if (!IsBuzzer)
            //{
            //    SetNormalLight();
            //}
        }

        bool IsEMCTriggered = false;

        private void MACHINE_TriggerAction(MachineEventEnum machineevent, object obj = null)
        {
            switch (machineevent)
            {
                case MachineEventEnum.ALARM_SERIOUS:
                    IsAlarmsSeriousX = true;
                    //SetAbnormalLight();
                    break;
                case MachineEventEnum.ALARM_COMMON:
                    IsAlarmsCommonX = true;
                    //SetAbnormalLight();
                    break;
                case MachineEventEnum.EMC:
                    IsEMCTriggered = true;
                    break;
            }
        }

        private void BtnAutoAndManual_Click(object sender, EventArgs e)
        {
            MACHINE.PLCIO.ADR_ISAUTO_AND_MANUAL = !MACHINE.PLCIO.ADR_ISAUTO_AND_MANUAL;
        }

        private void BtnMute_Click(object sender, EventArgs e)
        {
            MACHINE.PLCIO.ADR_BUZZER = false;
        }

        private void BtnClearAlm_Click(object sender, EventArgs e)
        {
            MACHINE.ClearAlarm = true;
            MACHINE.EVENT.RemoveAlarm();
            //M_WARNING_FRM = new MessageForm(true, "請檢查警報是否清除?");
            //if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
            //{
            //    MACHINE.ClearAlarm = true;
            //    MACHINE.EVENT.RemoveAlarm();
            //}
            //M_WARNING_FRM.Close();
            //M_WARNING_FRM.Dispose();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            if (MACHINE.PLCIO.IsAlarmsCommon || MACHINE.PLCIO.IsAlarmsSerious)
            {
                M_WARNING_FRM = new MessageForm(true, "报警中，请清除后再复位。", "");
                if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                {
                }
                M_WARNING_FRM.Close();
                M_WARNING_FRM.Dispose();

                return;
            }
            if (MACHINE.PLCIO.ADR_PROCESSING)
            {
                M_WARNING_FRM = new MessageForm(true, "流程中，无法复位。", "");
                if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                {
                }
                M_WARNING_FRM.Close();
                M_WARNING_FRM.Dispose();

                return;
            }
            if (MACHINE.PLCIO.ADR_RESETING)
            {
                M_WARNING_FRM = new MessageForm(true, "复位中，请勿重复点击。", "");
                if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                {
                }
                M_WARNING_FRM.Close();
                M_WARNING_FRM.Dispose();

                return;
            }
            M_WARNING_FRM = new MessageForm(true, "是否進行復位?");
            if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
            {
                if (!MACHINE.PLCIO.ADR_ISRESET)
                    MACHINE.PLCIO.ADR_RESET = true;
            }
            M_WARNING_FRM.Close();
            M_WARNING_FRM.Dispose();
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            MACHINE.PLCIO.ForceStopPlcProcess = true;
            //M_WARNING_FRM = new MessageForm(true, "是否停止?");
            //if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
            //{
            //    //StopAllProcess("USERSTOP");
            //}
            //M_WARNING_FRM.Close();
            //M_WARNING_FRM.Dispose();
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            if (MACHINE.PLCIO.IsAlarmsCommon || MACHINE.PLCIO.IsAlarmsSerious)
            {
                M_WARNING_FRM = new MessageForm(true, "报警中，请复位后再进行启动。", "");
                if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                {
                }
                M_WARNING_FRM.Close();
                M_WARNING_FRM.Dispose();

                return;
            }
            if (MACHINE.PLCIO.ADR_RESETING)
            {
                M_WARNING_FRM = new MessageForm(true, "复位中，无法启动。", "");
                if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                {
                }
                M_WARNING_FRM.Close();
                M_WARNING_FRM.Dispose();

                return;
            }
            if (MACHINE.PLCIO.ADR_PROCESSING)
            {
                M_WARNING_FRM = new MessageForm(true, "流程中，请勿重复点击。", "");
                if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                {
                }
                M_WARNING_FRM.Close();
                M_WARNING_FRM.Dispose();

                return;
            }
            M_WARNING_FRM = new MessageForm(true, "开启自动运行模式？");
            if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
            {
                if (!MACHINE.PLCIO.ADR_PROCESSING)
                    MACHINE.PLCIO.ADR_PROCESS = true;
            }
            M_WARNING_FRM.Close();
            M_WARNING_FRM.Dispose();
        }

        private void LblByPassDoor_DoubleClick(object sender, EventArgs e)
        {
            MACHINE.PLCIO.BypassDoor = !MACHINE.PLCIO.BypassDoor;
        }

        frmIO mIOForm = null;
        private void LblIO_DoubleClick(object sender, EventArgs e)
        {
            if (!INI.IsOpenIOWindows)
            {
                INI.IsOpenIOWindows = true;
                mIOForm = new frmIO();
                mIOForm.TopMost = true;
                mIOForm.Show();
            }
        }

        private void LblTestPoint_DoubleClick(object sender, EventArgs e)
        {
            //string add = "0:QB" + (numericUpDown1.Value).ToString("0000.0");
            //bool ison = MACHINE.PLCIO.GetQXQB(add);
            //lblTestPoint.BackColor = (ison ? Color.Green : Control.DefaultBackColor);
        }

        frmTouchMotor mMotorFrom = null;

        private void LblAXIS_DoubleClick(object sender, EventArgs e)
        {
            if (!Universal.IsOpenMotorWindows)
            {
                //OnTrigger(ActionEnum.ACT_MOTOR_SETUP, "");

                //MACHINE.SetNormalTemp(true);

                Universal.IsOpenMotorWindows = true;
                //MACHINE.PLCReadCmdNormalTemp(true);
                //System.Threading.Thread.Sleep(500);
                mMotorFrom = new frmTouchMotor();
                mMotorFrom.Show();
            }
        }

        public void SetEnable(bool isendable)
        {
            //tpnlCover.Visible = !isendable;

            //Color fillcolor = SystemColors.Control;

            //if (!isendable)
            //    fillcolor = Color.Silver;
        }

        public void SetEnable()
        {
            bool isenable = !tpnlCover.Visible;
            SetEnable(isenable);
            this.Invalidate();
        }

        public void Tick()
        {
            if (myTime.msDuriation > 100)
            {
                if (!Universal.IsUseThreadReviceTcp)
                    MACHINE.Tick();

                myTime.Cut();

                if (IsAlarmsSeriousX)
                {
                    SetAbnormalLight();

                    IsAlarmsSeriousX = false;
                    //StopAllProcess();
                    SetSeriousAlarms0();
                    //SetSeriousAlarms1();

                    //StopAllProcess();
                }

                if (IsAlarmsCommonX)
                {
                    SetAbnormalLight();

                    IsAlarmsCommonX = false;
                    //StopAllProcess();
                    SetCommonAlarms();

                }

                if (MACHINE.PLCIO.ADR_ISEMC)
                {
                    lblState.Text = "急停中";
                    lblState.BackColor = Color.Red;
                }
                else if (MACHINE.PLCIO.ADR_Door && !MACHINE.PLCIO.BypassDoor)
                {
                    lblState.Text = "门被打开";
                    lblState.BackColor = Color.Red;
                }
                else if (MACHINE.PLCIO.ADR_RESETING)
                {
                    lblState.Text = "复位中";
                    lblState.BackColor = Color.Black;
                }
                else if (MACHINE.PLCIO.ADR_PROCESSING)
                {
                    lblState.Text = "运行中";
                    lblState.BackColor = Color.Black;
                }
                else
                {
                    lblState.Text = "待机";
                    lblState.BackColor = Color.Black;
                }

                lblByPassDoor.BackColor = (MACHINE.PLCIO.BypassDoor ? Color.Red : Color.Green);
                lblByPassDoor.Text = (MACHINE.PLCIO.BypassDoor ? "門禁屏蔽" : "門禁打開");

                btnAutoAndManual.Text = (MACHINE.PLCIO.ADR_ISAUTO_AND_MANUAL ? "自动" : "手动");
                btnStart.Text = (MACHINE.PLCIO.ADR_PROCESSING ? "运行中" : "启动");
                btnStop.Text = (MACHINE.PLCIO.ForceStopPlcProcess ? "停止中" : "停止");
                btnReset.Text = (MACHINE.PLCIO.ADR_RESETING ? "复位中" : "复位");
                btnOnekeyGetImage.Text = (MACHINE.PLCIO.ADR_OnceGetImage ? "单次抓图中" : "一键取像(线扫)");

                btnAutoAndManual.BackColor = (MACHINE.PLCIO.ADR_ISAUTO_AND_MANUAL ? Color.Lime : Color.FromArgb(192, 255, 192));
                btnStart.BackColor = (MACHINE.PLCIO.ADR_PROCESSING ? Color.Red : Color.FromArgb(192, 255, 192));
                btnStop.BackColor = (MACHINE.PLCIO.ForceStopPlcProcess ? Color.Yellow : Color.FromArgb(192, 255, 192));
                btnReset.BackColor = (MACHINE.PLCIO.ADR_RESETING ? Color.Red : Color.FromArgb(192, 255, 192));
                btnOnekeyGetImage.BackColor = (MACHINE.PLCIO.ADR_OnceGetImage ? Color.Red : Color.FromArgb(192, 255, 192));
            }
            //lblVacc.BackColor = (MACHINE.PLCIO.ADR_ISVACC ? Color.Green : Color.Black);
        }
        public void PlcTick()
        {
            while (m_ThRunning)
            {
                MACHINE.Tick();
                Thread.Sleep(50);
            }
        }
        /// <summary>
        /// 释放资源并关闭线程
        /// </summary>
        public void SDDispose()
        {
            //MACHINE.PLCIO.Ready = false;

            //MACHINE.PLCIO.Pass = false;
            //MACHINE.PLCIO.Fail = false;

            m_ThRunning = false;
            if (m_ThreadPlc != null)
            {
                m_ThreadPlc.Abort();
                m_ThreadPlc = null;
            }
        }

        public delegate void TriggerHandler(ActionEnum action, string opstr);
        public event TriggerHandler TriggerAction;
        public void OnTrigger(ActionEnum action, string opstr)
        {
            if (TriggerAction != null)
            {
                TriggerAction(action, opstr);
            }
        }
        
    }
}
