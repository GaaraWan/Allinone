using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JetEazy.BasicSpace;
using JetEazy;
using Allinone.OPSpace;

namespace Allinone.UISpace.RUNUISpace
{
    public partial class RunV1UI : UserControl
    {
        const int ShinningDuriation = 50;
        const int ShiningTimes = 2;

        bool IsResultPass = false;

        Label lblPass;
        ListBox lsbRunProcess;

        CheckBox chkIsSaveRaw;
        CheckBox chkIsSaveNGRaw;
        CheckBox chkIsSaveDebug;


        JzTimes myTimes = new JzTimes();

        public bool IsSaveRaw
        {
            get
            {
                return chkIsSaveRaw.Checked;
            }
        }
        public bool IsSaveNGRaw
        {
            get
            {
                return chkIsSaveNGRaw.Checked;
            }
        }
        public bool IsSaveDebug
        {
            get
            {
                return chkIsSaveDebug.Checked;
            }
        }

        public RunV1UI()
        {
            InitializeComponent();
        }

        public void Initial()
        {
            lblPass = label4;
            lsbRunProcess = listBox1;

            chkIsSaveRaw = checkBox1;
            chkIsSaveNGRaw = checkBox2;
            chkIsSaveDebug = checkBox3;
        }

        public void SetLog(string str,bool isstart = false)
        {
            if(isstart)
            {
                lsbRunProcess.Items.Clear();
                lsbRunProcess.Items.Add(str);
            }
            else
            {
                lsbRunProcess.Items.Add(str);
            }

            lsbRunProcess.SelectedIndex = lsbRunProcess.Items.Count - 1;
        }

        public void Tick()
        {
            if (myTimes.msDuriation > ShinningDuriation)
            {
                #region 是否進行復判
                //if (INI.ISJUDGEPASS && INI.JUDGE)
                //{
                //    if (PLC.IsJudge && !IsSet)
                //    {
                //        IsSet = true;
                //    }

                //    if (myJudgeTimes.msDuriation >= INI.JUDGEDELAYTIME)
                //    {
                //        INI.JUDGE = false;

                //        if (IsSet)
                //        {
                //            IsSet = false;
                //            Universal.IsManualpass = true;
                //        }

                //        ShinningContinue();
                //    }
                //}
                #endregion

                ShinningTick();
                myTimes.Cut();
            }
        }


        public void ShinningPause()
        {
            ShinningProcess.Pause();
        }
        public void ShinningContinue()
        {
            ShinningProcess.Continue();
        }

        bool IsSet = false;
        public void StartShinnig(bool isresultpass)
        {
            IsResultPass = isresultpass;
            ShinningProcess.Start();
        }
        public void SetDuriation(string inputstr)
        {
            //lblDuriation.Text = inputstr;
            //lblDuriation.Invalidate();
        }

        public void SaveLog(string rptstr, string savename)
        {
            //RPTUI.LogRecord(rptstr, savename);
        }

        public bool IsShinning
        {
            get
            {
                return ShinningProcess.IsOn;
            }
        }
        public void SetRunUING()
        {
            lblPass.Text = "NG";
            lblPass.ForeColor = Color.Yellow;
            lblPass.BackColor = Color.DarkRed;
            lblPass.Refresh();
        }

        int ShinigCount = 0;
        ProcessClass ShinningProcess = new ProcessClass();
        public void ShinningTick()
        {
            ProcessClass Process = ShinningProcess;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        //lblBigPass.Visible = IsPass;
                        if (ShinigCount == 0)
                        {
                            Process.TimeUnit = TimeUnitEnum.ms;
                            lblPass.Text = (IsResultPass ? "PASS" : "NG");
                        }

                        if (ShinigCount == 0 || Process.IsTimeup)
                        {
                            lblPass.ForeColor = (IsResultPass ? Color.Lime : Color.Red);

                            //if (IsResultPass)
                            //    ShineGreen();
                            //else
                            //    ShineRed();
                            lblPass.Invalidate();

                            Process.ID = 10;
                            Process.NextDuriation = 100;
                        }
                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            lblPass.ForeColor = (IsResultPass ? Color.Green : Color.DarkRed);

                            //ShineNothing();
                            lblPass.Invalidate();

                            ShinigCount++;

                            if (ShinigCount > ShiningTimes)
                            {

                                ShinigCount = 0;
                                //OnTrigger((IsPass ? StatusEnum.CALPASS : StatusEnum.CALNG));

                                //OnTrigger(StatusEnum.CALEND);
                                OnTrigger(RunStatusEnum.SHINNIGEND);

                                Process.Stop();
                            }
                            else
                                Process.ID = 5;
                        }
                        break;
                }
            }
        }

        public delegate void TriggerHandler(RunStatusEnum runstatus);
        public event TriggerHandler TriggerAction;
        public void OnTrigger(RunStatusEnum runstatus)
        {
            if (TriggerAction != null)
            {
                TriggerAction(runstatus);
            }
        }

        public delegate void BarcodeHandler(string barcode);
        public event BarcodeHandler BarcodeAction;
        public void OnBarcode(string barcode)
        {
            if (BarcodeAction != null)
            {
                BarcodeAction(barcode);
            }
        }

        public delegate void LearningHandler(PassInfoClass passinfo, LearnOperEnum learnoper);
        public event LearningHandler LearnAction;
        public void OnLearn(PassInfoClass passinfo, LearnOperEnum learnoper)
        {
            if (LearnAction != null)
            {
                LearnAction(passinfo, learnoper);
            }
        }


    }
}
