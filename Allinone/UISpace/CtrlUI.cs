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
using JetEazy.UISpace;
using Allinone.UISpace.CTRLUISpace;
using Allinone.ControlSpace.MachineSpace;

namespace Allinone.UISpace
{
    public partial class CtrlUI : UserControl
    {
        VersionEnum VERSION;
        OptionEnum OPTION;

        LayoutEnum LAYOUT
        {
            get
            {
                return Universal.LAYOUT;
            }
        }

        AllinoneSDM5CtrlUI AllinoneSDM5CTRL;
        AllinoneSDM3CtrlUI AllinoneSDM3CTRL;
        MainServiceCtrlUI MainServiceCTRL;
        AllinoneSDM2CtrlUI AllinoneSDM2CTRL;
        AllinoneSDM1CtrlUI AllinoneSDM1CTRL;
        MainX6CtrlUI MainX6CTRL;
        AllinoneSDCtrlUI AllinoneSDCTRL;
        AlliononeCtrlUI AllinoneCTRL;
        AudixCtrlUI AudixCTRL;
        R32CtrlUI R32CTRL;
        DFLYCtrlUI DFlyCTRL;
        RXXCtrlUI RXXCTRL;
        R15CtrlUI R15CTRL;
        R9CtrlUI R9CTRL;
        R5CtrlUI R5CTRL;
        R3CtrlUI R3CTRL;
        C3CtrlUI C3CTRL;
        R1CtrlUI R1CTRL;
        public CtrlUI()
        {
            switch (LAYOUT)
            {
                case LayoutEnum.L1280X800:
                    InitializeComponent();
                    //InitializeComponent1280X800();
                    break;
                case LayoutEnum.L1440X900:
                    InitializeComponent1440X900();
                    break;
            }

            InitialInternal();
        }

        void InitialInternal()
        {

        }

        public void Initial(VersionEnum version, OptionEnum option, GeoMachineClass machine)
        {
            VERSION = version;
            OPTION = option;

            switch (VERSION)
            {
                case VersionEnum.ALLINONE:

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN:

                            AllinoneCTRL = new AlliononeCtrlUI();
                            AllinoneCTRL.Initial(VERSION, OPTION, (JzAllinoneMachineClass)machine);

                            AllinoneCTRL.Location = new Point(0, 0);
                            this.Controls.Add(AllinoneCTRL);

                            break;
                        case OptionEnum.R32:

                            R32CTRL = new R32CtrlUI();
                            R32CTRL.Initial(VERSION, OPTION, (JzR32MachineClass)machine);

                            R32CTRL.Location = new Point(0, 0);
                            this.Controls.Add(R32CTRL);
                            break;
                        case OptionEnum.R26:

                            RXXCTRL = new RXXCtrlUI();
                            RXXCTRL.Initial(VERSION, OPTION, (JzRXXMachineClass)machine);

                            RXXCTRL.Location = new Point(0, 0);
                            this.Controls.Add(RXXCTRL);
                            break;
                        case OptionEnum.R15:

                            R15CTRL = new R15CtrlUI();
                            R15CTRL.Initial(VERSION, OPTION, (JzR15MachineClass)machine);

                            R15CTRL.Location = new Point(0, 0);
                            this.Controls.Add(R15CTRL);
                            break;
                        case OptionEnum.R9:

                            R9CTRL = new R9CtrlUI();
                            R9CTRL.Initial(VERSION, OPTION, (JzR9MachineClass)machine);

                            R9CTRL.Location = new Point(0, 0);
                            this.Controls.Add(R9CTRL);
                            break;
                        case OptionEnum.R5:

                            R5CTRL = new R5CtrlUI();
                            R5CTRL.Initial(VERSION, OPTION, (JzR5MachineClass)machine);

                            R5CTRL.Location = new Point(0, 0);
                            this.Controls.Add(R5CTRL);
                            break;
                        case OptionEnum.R1:

                            R1CTRL = new R1CtrlUI();
                            R1CTRL.Initial(VERSION, OPTION, (JzR1MachineClass)machine);

                            R1CTRL.Location = new Point(0, 0);
                            this.Controls.Add(R1CTRL);
                            break;
                        case OptionEnum.R3:

                            R3CTRL = new R3CtrlUI();
                            R3CTRL.Initial(VERSION, OPTION, (JzR3MachineClass)machine);

                            R3CTRL.Location = new Point(0, 0);
                            this.Controls.Add(R3CTRL);
                            break;
                        case OptionEnum.C3:

                            C3CTRL = new C3CtrlUI();
                            C3CTRL.Initial(VERSION, OPTION, (JzC3MachineClass)machine);

                            C3CTRL.Location = new Point(0, 0);
                            this.Controls.Add(C3CTRL);
                            break;
                        case OptionEnum.MAIN_SD:

                            AllinoneSDCTRL = new AllinoneSDCtrlUI();
                            AllinoneSDCTRL.Initial(VERSION, OPTION, (JzMainSDMachineClass)machine);

                            AllinoneSDCTRL.Location = new Point(0, 0);
                            this.Controls.Add(AllinoneSDCTRL);

                            AllinoneSDCTRL.TriggerAction += AllinoneSDCTRL_TriggerAction;
                            break;
                        case OptionEnum.MAIN_X6:
                            MainX6CTRL = new MainX6CtrlUI();
                            MainX6CTRL.Initial(VERSION, OPTION, (JzMainX6MachineClass)machine);

                            MainX6CTRL.Location = new Point(0, 0);
                            this.Controls.Add(MainX6CTRL);

                            MainX6CTRL.TriggerAction += CTRL_TriggerAction;

                            break;
                        case OptionEnum.MAIN_SDM1:
                            AllinoneSDM1CTRL = new AllinoneSDM1CtrlUI();
                            AllinoneSDM1CTRL.Initial(VERSION, OPTION, (JzMainSDM1MachineClass)machine);

                            AllinoneSDM1CTRL.Location = new Point(0, 0);
                            this.Controls.Add(AllinoneSDM1CTRL);

                            AllinoneSDM1CTRL.TriggerAction += CTRL_TriggerAction;

                            break;
                        case OptionEnum.MAIN_SDM2:
                            AllinoneSDM2CTRL = new AllinoneSDM2CtrlUI();
                            AllinoneSDM2CTRL.Initial(VERSION, OPTION, (JzMainSDM2MachineClass)machine);

                            AllinoneSDM2CTRL.Location = new Point(0, 0);
                            this.Controls.Add(AllinoneSDM2CTRL);

                            AllinoneSDM2CTRL.TriggerAction += AllinoneSDM2CTRL_TriggerAction;
                            break;
                        case OptionEnum.MAIN_SDM3:
                            AllinoneSDM3CTRL = new AllinoneSDM3CtrlUI();
                            AllinoneSDM3CTRL.Initial(VERSION, OPTION, (JzMainSDM3MachineClass)machine);

                            AllinoneSDM3CTRL.Location = new Point(0, 0);
                            this.Controls.Add(AllinoneSDM3CTRL);

                            AllinoneSDM3CTRL.TriggerAction += AllinoneSDM3CTRL_TriggerAction;

                            break;
                        case OptionEnum.MAIN_SERVICE:
                            MainServiceCTRL = new MainServiceCtrlUI();
                            MainServiceCTRL.Initial(VERSION, OPTION, (JzMainServiceMachineClass)machine);

                            MainServiceCTRL.Location = new Point(0, 0);
                            this.Controls.Add(MainServiceCTRL);

                            break;
                        case OptionEnum.MAIN_SDM5:
                            AllinoneSDM5CTRL = new AllinoneSDM5CtrlUI();
                            AllinoneSDM5CTRL.Initial(VERSION, OPTION, (JzMainSDM5MachineClass)machine);

                            AllinoneSDM5CTRL.Location = new Point(0, 0);
                            this.Controls.Add(AllinoneSDM5CTRL);

                            AllinoneSDM5CTRL.TriggerAction += AllinoneSDM5CTRL_TriggerAction;

                            break;
                    }

                    break;
                case VersionEnum.AUDIX:

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN:
                            AudixCTRL = new AudixCtrlUI();
                            AudixCTRL.Initial(VERSION, OPTION, (JzAudixMachineClass)machine);

                            AudixCTRL.Location = new Point(0, 0);
                            this.Controls.Add(AudixCTRL);
                            break;
                        case OptionEnum.MAIN_DFLY:

                            DFlyCTRL = new DFLYCtrlUI();
                            DFlyCTRL.Initial(VERSION, OPTION, (JzDFlyMachineClass)machine);

                            DFlyCTRL.Location = new Point(0, 0);
                            this.Controls.Add(DFlyCTRL);

                            DFlyCTRL.TriggerAction += DFlyCTRL_TriggerAction;

                            break;
                    }
                    break;
            }
        }

        private void CTRL_TriggerAction(ActionEnum action, string opstr)
        {
            TriggerAction(action, opstr);
        }
        private void AllinoneSDM2CTRL_TriggerAction(ActionEnum action, string opstr)
        {
            TriggerAction(action, opstr);
        }
        private void AllinoneSDM3CTRL_TriggerAction(ActionEnum action, string opstr)
        {
            TriggerAction(action, opstr);
        }
        private void AllinoneSDM5CTRL_TriggerAction(ActionEnum action, string opstr)
        {
            TriggerAction(action, opstr);
        }
        private void AllinoneSDCTRL_TriggerAction(ActionEnum action, string opstr)
        {
            TriggerAction(action, opstr);
        }

        private void DFlyCTRL_TriggerAction(ActionEnum action, string opstr)
        {
            TriggerAction(action, opstr);
        }

        public void Tick()
        {
            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_SDM5:
                            AllinoneSDM5CTRL.Tick();
                            break;
                        case OptionEnum.MAIN_SDM3:
                            AllinoneSDM3CTRL.Tick();
                            break;
                        case OptionEnum.MAIN_SERVICE:
                            MainServiceCTRL.Tick();
                            break;
                        case OptionEnum.MAIN_SDM2:
                            AllinoneSDM2CTRL.Tick();
                            break;
                        case OptionEnum.MAIN_SDM1:
                            AllinoneSDM1CTRL.Tick();
                            break;
                        case OptionEnum.MAIN_X6:
                            MainX6CTRL.Tick();
                            break;
                        case OptionEnum.MAIN_SD:
                            AllinoneSDCTRL.Tick();
                            break;
                        case OptionEnum.MAIN:
                            AllinoneCTRL.Tick();
                            break;
                        case OptionEnum.R32:
                            R32CTRL.Tick();
                            break;
                        case OptionEnum.R26:
                            RXXCTRL.Tick();
                            break;
                        case OptionEnum.R15:
                            R15CTRL.Tick();
                            break;
                        case OptionEnum.R9:
                            R9CTRL.Tick();
                            break;
                        case OptionEnum.R5:
                            R5CTRL.Tick();
                            break;
                        case OptionEnum.R1:
                            R1CTRL.Tick();
                            break;
                        case OptionEnum.R3:
                            R3CTRL.Tick();
                            break;
                        case OptionEnum.C3:
                            C3CTRL.Tick();
                            break;
                    }
                    break;
                case VersionEnum.AUDIX:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN:
                            AudixCTRL.Tick();
                            break;
                        case OptionEnum.MAIN_DFLY:
                            DFlyCTRL.Tick();
                            break;
                    }
                    break;
            }
        }
        public void SetEnable(bool isenable)
        {
            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_SDM5:
                            AllinoneSDM5CTRL.SetEnable(isenable);
                            break;
                        case OptionEnum.MAIN_SDM3:
                            AllinoneSDM3CTRL.SetEnable(isenable);
                            break;
                        case OptionEnum.MAIN_SERVICE:
                            MainServiceCTRL.SetEnable(isenable);
                            break;
                        case OptionEnum.MAIN_SDM2:
                            AllinoneSDM2CTRL.SetEnable(isenable);
                            break;
                        case OptionEnum.MAIN_SDM1:
                            AllinoneSDM1CTRL.SetEnable(isenable);
                            break;
                        case OptionEnum.MAIN_X6:
                            MainX6CTRL.SetEnable(isenable);
                            break;
                        case OptionEnum.MAIN_SD:
                            AllinoneSDCTRL.SetEnable(isenable);
                            break;
                        case OptionEnum.MAIN:
                            AllinoneCTRL.SetEnable(isenable);
                            break;
                        case OptionEnum.R32:
                            R32CTRL.SetEnable(isenable);
                            break;
                        case OptionEnum.R26:
                            RXXCTRL.SetEnable(isenable);
                            break;
                        case OptionEnum.R15:
                            R15CTRL.SetEnable(isenable);
                            break;
                        case OptionEnum.R9:
                            R9CTRL.SetEnable(isenable);
                            break;
                        case OptionEnum.R5:
                            R5CTRL.SetEnable(isenable);
                            break;
                        case OptionEnum.R1:
                            R1CTRL.SetEnable(isenable);
                            break;
                        case OptionEnum.R3:
                            R3CTRL.SetEnable(isenable);
                            break;
                        case OptionEnum.C3:
                            C3CTRL.SetEnable(isenable);
                            break;
                    }
                    break;
                case VersionEnum.AUDIX:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN:
                            AudixCTRL.SetEnable(isenable);
                            break;
                        case OptionEnum.MAIN_DFLY:
                            DFlyCTRL.SetEnable(isenable);
                            break;
                    }
                    break;
            }
        }

        public void myDispose()
        {
            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_SERVICE:
                            MainServiceCTRL.SDDispose();
                            break;
                        case OptionEnum.MAIN_SD:
                            AllinoneSDCTRL.SDDispose();
                            break;
                        case OptionEnum.MAIN_X6:
                            MainX6CTRL.SDDispose();
                            break;
                        case OptionEnum.MAIN_SDM1:
                            AllinoneSDM1CTRL.SDDispose();
                            break;
                        case OptionEnum.MAIN_SDM2:
                            AllinoneSDM2CTRL.SDDispose();
                            break;
                        case OptionEnum.MAIN_SDM3:
                            AllinoneSDM3CTRL.SDDispose();
                            break;
                        case OptionEnum.MAIN_SDM5:
                            //AllinoneSDM5CTRL.SDDispose();
                            break;
                    }
                    break;

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
