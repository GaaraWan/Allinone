using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JetEazy;
using JetEazy.BasicSpace;
using JetEazy.ControlSpace;
using JetEazy.ControlSpace.MotionSpace;

using Allinone.ControlSpace.MachineSpace;
using System.Windows.Forms;
using iTextSharp.text.pdf.collection;

namespace Allinone.ControlSpace
{
    public class MachineCollectionClass
    {
        VersionEnum VERSION;
        OptionEnum OPTION;

        public GeoMachineClass MACHINE;

        public MachineCollectionClass()
        {

        }

        public void Intial(VersionEnum version,OptionEnum option,GeoMachineClass machine)
        {
            VERSION = version;
            OPTION = option;

            MACHINE = machine;

            MACHINE.TriggerAction += MACHINE_TriggerAction;
            SetConfig();
        }

        public void RetryConnect()
        {
            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_SDM2:
                            switch (((JzMainSDM2MachineClass)MACHINE).mRobotType)
                            {
                                case RobotType.HCFA:

                                    ((JzMainSDM2MachineClass)MACHINE).PLCRetry();

                                    break;
                                case RobotType.NONE:


                                    break;
                            }

                            break;
                        default:

                            foreach (FatekPLCClass plc in MACHINE.PLCCollection)
                            {
                                if (plc.IsConnectionFail)
                                    plc.RetryConn();
                            }

                            break;
                    }
                    break;
                case VersionEnum.AUDIX:

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_DFLY:
                            break;
                    }
                    break;
            }
        }
        public void SetConfig()
        {
            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN:
                            break;
                        case OptionEnum.MAIN_SDM1:
                            break;
                        case OptionEnum.MAIN_SDM2:
                            switch (((JzMainSDM2MachineClass)MACHINE).mRobotType)
                            {
                                case RobotType.HCFA:

                                    ((JzMainSDM2MachineClass)MACHINE).PLCIO.RobotSpeedValue = INI.RobotSpeedValue;

                                    break;
                                case RobotType.NONE:

                                    ((JzMainSDM2MachineClass)MACHINE).PLCIO.SetAxisJJS("0:MW0110", INI.AXIS_MANUAL_JJS_ADD);
                                    ((JzMainSDM2MachineClass)MACHINE).PLCIO.SetAxisJJS("0:MW0111", INI.AXIS_MANUAL_JJS_SUB);
                                    ((JzMainSDM2MachineClass)MACHINE).PLCIO.SetAxisJJS("0:MW0113", INI.AXIS_AUTO_JJS_ADD);
                                    ((JzMainSDM2MachineClass)MACHINE).PLCIO.SetAxisJJS("0:MW0114", INI.AXIS_AUTO_JJS_SUB);


                                    break;
                            }

                            break;
                        default:

                            break;
                    }
                    break;
                case VersionEnum.AUDIX:

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_DFLY:
                            break;
                    }
                    break;
            }
            
        }

        private void MACHINE_TriggerAction(MachineEventEnum machineevent, object obj = null)
        {
            OnTrigger(machineevent);
        }
        public void SetLight(string str)
        {
            //string[] strs = str.Split(',');

            switch(VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch(OPTION)
                    {
                        case OptionEnum.MAIN_X6:
                            ((JzMainX6MachineClass)MACHINE).SetLight(str);
                            break;
                        case OptionEnum.MAIN_SD:
                            ((JzMainSDMachineClass)MACHINE).SetLight(str);
                            break;
                        case OptionEnum.MAIN:
                            //((JzAllinoneMachineClass)MACHINE).PLCIO.TopLight = strs[0] == "1";
                            //((JzAllinoneMachineClass)MACHINE).PLCIO.MylarLight = strs[1] == "1";
                            ((JzAllinoneMachineClass)MACHINE).SetLight(str);
                            break;
                        case OptionEnum.R32:
                            //((JzR32MachineClass)MACHINE).PLCIO.TopLight = strs[0] == "1";
                            //((JzR32MachineClass)MACHINE).PLCIO.MylarLight = strs[1] == "1";

                            ((JzR32MachineClass)MACHINE).SetLight(str);
                            break;
                        case OptionEnum.R26:
                            //((JzR32MachineClass)MACHINE).PLCIO.TopLight = strs[0] == "1";
                            //((JzR32MachineClass)MACHINE).PLCIO.MylarLight = strs[1] == "1";
                            ((JzRXXMachineClass)MACHINE).SetLight(str);
                            break;
                        case OptionEnum.R15:
                            //((JzR32MachineClass)MACHINE).PLCIO.TopLight = strs[0] == "1";
                            //((JzR32MachineClass)MACHINE).PLCIO.MylarLight = strs[1] == "1";
                            ((JzR15MachineClass)MACHINE).SetLight(str);
                            break;
                        case OptionEnum.R9:
                            //((JzR32MachineClass)MACHINE).PLCIO.TopLight = strs[0] == "1";
                            //((JzR32MachineClass)MACHINE).PLCIO.MylarLight = strs[1] == "1";
                            ((JzR9MachineClass)MACHINE).SetLight(str);
                            break;
                        case OptionEnum.R5:
                            //((JzR32MachineClass)MACHINE).PLCIO.TopLight = strs[0] == "1";
                            //((JzR32MachineClass)MACHINE).PLCIO.MylarLight = strs[1] == "1";
                            ((JzR5MachineClass)MACHINE).SetLight(str);
                            break;
                        case OptionEnum.R3:
                            //((JzR32MachineClass)MACHINE).PLCIO.TopLight = strs[0] == "1";
                            //((JzR32MachineClass)MACHINE).PLCIO.MylarLight = strs[1] == "1";
                            ((JzR3MachineClass)MACHINE).SetLight(str);
                            break;
                        case OptionEnum.C3:
                            //((JzR32MachineClass)MACHINE).PLCIO.TopLight = strs[0] == "1";
                            //((JzR32MachineClass)MACHINE).PLCIO.MylarLight = strs[1] == "1";
                            ((JzC3MachineClass)MACHINE).SetLight(str);
                            break;
                        case OptionEnum.R1:
                            //((JzR32MachineClass)MACHINE).PLCIO.TopLight = strs[0] == "1";
                            //((JzR32MachineClass)MACHINE).PLCIO.MylarLight = strs[1] == "1";
                            ((JzR1MachineClass)MACHINE).SetLight(str);
                            break;
                    }
                    break;
            }

        }
        public string GetPosition()
        {
            string posstr = "";

            switch(VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch(OPTION)
                    {
                        case OptionEnum.MAIN:
                            posstr = ((JzAllinoneMachineClass)MACHINE).GetPosition();
                            break;
                        case OptionEnum.MAIN_SDM1:
                            posstr = ((JzMainSDM1MachineClass)MACHINE).GetPosition();
                            break;
                        case OptionEnum.MAIN_SDM2:
                            posstr = ((JzMainSDM2MachineClass)MACHINE).GetPosition();
                            break;
                        case OptionEnum.MAIN_SDM3:
                            posstr = ((JzMainSDM3MachineClass)MACHINE).GetPosition();
                            break;
                        default:

                            break;
                    }

                    
                    break;
                case VersionEnum.AUDIX:
                    switch(OPTION)
                    {
                        case OptionEnum.MAIN:

                            break;
                        case OptionEnum.MAIN_DFLY:
                            posstr = ((JzDFlyMachineClass)MACHINE).GetPosition();
                            break;
                    }
                    break;
            }
            return posstr;

        }
        public void MoveGo()
        {
            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN:
                            break;
                        case OptionEnum.MAIN_SDM1:
                            break;
                        case OptionEnum.MAIN_SDM2:
                            Task task = new Task(() =>
                            {
                                ((JzMainSDM2MachineClass)MACHINE).PLCIO.RobotAbs = true;
                                System.Threading.Thread.Sleep(500);
                                ((JzMainSDM2MachineClass)MACHINE).PLCIO.RobotAbs = false;
                            });
                            task.Start();
                            break;
                        case OptionEnum.MAIN_SDM3:
                            Task task3 = new Task(() =>
                            {
                                ((JzMainSDM3MachineClass)MACHINE).PLCIO.RobotAbs = true;
                                System.Threading.Thread.Sleep(500);
                                ((JzMainSDM3MachineClass)MACHINE).PLCIO.RobotAbs = false;
                            });
                            task3.Start();
                            break;
                        default:

                            break;
                    }
                    break;
                case VersionEnum.AUDIX:

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_DFLY:
                            break;
                    }
                    break;
            }
        }
        public void GoPosition(string opstr, bool isMove = true)
        {
            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN:
                            ((JzAllinoneMachineClass)MACHINE).GoPosition(opstr);
                            break;
                        case OptionEnum.MAIN_SDM1:
                            ((JzMainSDM1MachineClass)MACHINE).GoXPosition(opstr);
                            break;
                        case OptionEnum.MAIN_SDM2:
                            ((JzMainSDM2MachineClass)MACHINE).GoPosition(opstr, isMove);
                            break;
                        case OptionEnum.MAIN_SDM3:
                            ((JzMainSDM3MachineClass)MACHINE).GoPosition(opstr, isMove);
                            break;
                        default:

                            break;
                    }
                    break;
                case VersionEnum.AUDIX:

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_DFLY:
                            ((JzDFlyMachineClass)MACHINE).GoPosition(opstr);
                            break;
                    }
                    break;
            }
        }

        public void GoHome()
        {
            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN:
                            ((JzAllinoneMachineClass)MACHINE).GoHome();
                            break;
                        default:

                            break;
                    }
                    break;
                case VersionEnum.AUDIX:

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_DFLY:
                            ((JzDFlyMachineClass)MACHINE).GoHome();
                            break;
                    }
                    break;
            }



        }
        public string GetFps()
        {
            string Str = string.Empty;
            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_SDM2:
                            Str = ((JzMainSDM2MachineClass)MACHINE).Fps();
                            break;
                        case OptionEnum.MAIN_SDM3:
                            Str = ((JzMainSDM3MachineClass)MACHINE).Fps();
                            break;
                        case OptionEnum.MAIN_SDM5:
                            //Str = ((JzMainSDM5MachineClass)MACHINE).Fps();
                            break;
                        default:

                            break;
                    }
                    break;
                case VersionEnum.AUDIX:

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_DFLY:
                            break;
                    }
                    break;
            }
            return Str;
        }

        #region AUDIX use function

        public void SetPass(bool ispass)
        {
            switch(VERSION)
            {
                case VersionEnum.AUDIX:
                    ((JzAudixMachineClass)MACHINE).SetPass(ispass);
                    break;
            }
        }

        #endregion

        public delegate void TriggerHandler(MachineEventEnum machineevent);
        public event TriggerHandler TriggerAction;
        public void OnTrigger(MachineEventEnum machineevent)
        {
            if (TriggerAction != null)
            {
                TriggerAction(machineevent);
            }
        }


    }
}
