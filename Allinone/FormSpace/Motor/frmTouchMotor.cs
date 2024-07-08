using Allinone.ControlSpace;
using Allinone.ControlSpace.MachineSpace;
using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Allinone.FormSpace.Motor
{
    public partial class frmTouchMotor : Form
    {
        const int AXIS_COUNT = 4;
        VsTouchMotorUI[] VSAXISUI = new VsTouchMotorUI[AXIS_COUNT];
        MotionTouchPanelUIClass[] AXISUI = new MotionTouchPanelUIClass[AXIS_COUNT];

        Timer mMotorTimer = null;
        MachineCollectionClass MACHINECollection
        {
            get
            {
                return Universal.MACHINECollection;
            }
        }
        //JzMainSDM2MachineClass MACHINE
        //{
        //    get { return (JzMainSDM2MachineClass)MACHINECollection.MACHINE; }
        //}
        public frmTouchMotor()
        {
            InitializeComponent();

            this.TopMost = true;

            this.Load += FrmTouchMotor_Load;
            this.FormClosed += FrmTouchMotor_FormClosed;
        }
        private void FrmTouchMotor_FormClosed(object sender, FormClosedEventArgs e)
        {
            Universal.IsOpenMotorWindows = false;
        }
        private void FrmTouchMotor_Load(object sender, EventArgs e)
        {
            this.Text = "轴设定视窗";
            Init();
        }
        void Init()
        {
            #region 位置设定控件

            VSAXISUI[0] = vsTouchMotorUI3;
            VSAXISUI[1] = vsTouchMotorUI2;
            VSAXISUI[2] = vsTouchMotorUI1;
            VSAXISUI[3] = vsTouchMotorUI4;

            int i = 0;
            while (i < AXIS_COUNT)
            {
                AXISUI[i] = new MotionTouchPanelUIClass(VSAXISUI[i]);

                switch (Universal.OPTION)
                {
                    case JetEazy.OptionEnum.MAIN_SDM2:
                        AXISUI[i].Initial(((JzMainSDM2MachineClass)MACHINECollection.MACHINE).PLCMOTIONCollection[i]);
                        break;
                    case JetEazy.OptionEnum.MAIN_SDM3:
                        AXISUI[i].Initial(((JzMainSDM3MachineClass)MACHINECollection.MACHINE).PLCMOTIONCollection[i]);
                        break;
                }


                i++;
            }

            #endregion

            mMotorTimer = new Timer();
            mMotorTimer.Interval = 50;
            mMotorTimer.Enabled = true;
            mMotorTimer.Tick += MMotorTimer_Tick;

            //MACHINE.TriggerAction += MACHINE_TriggerAction;

            //FillDisplay();

            //LanguageExClass.Instance.EnumControls(this);
        }
        private void MMotorTimer_Tick(object sender, EventArgs e)
        {
            //if (!Universal.IsNoUseIO)
            //{
            //    if (IsEMCTriggered)
            //    {
            //        //SetAbnormalLight();

            //        IsEMCTriggered = false;
            //        StopAllProcess();
            //        //OnTrigger(ActionEnum.ACT_ISEMC, "");
            //    }
            //}

            int i = 0;
            while (i < AXIS_COUNT)
            {
                AXISUI[i].Tick();
                i++;
            }
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
