using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JetEazy;
using JetEazy.BasicSpace;
using JetEazy.ControlSpace;
using JetEazy.ControlSpace.MotionSpace;

using Allinone.ControlSpace.IOSpace;
using MotoCan;

namespace Allinone.ControlSpace.MachineSpace
{
    public class JzDFlyMachineClass : GeoMachineClass
    {
        const int MSDuriation = 10;

        public JzDFlyPLCIOClass PLCIO;
        public JzDFlyMachineClass(Machine_EA machineea, VersionEnum version, OptionEnum option, string opstr, string workpath, bool isnouseplc)
        {
            IsNoUseIO = isnouseplc;

            myMachineEA = machineea;
            VERSION = version;
            OPTION = option;

            WORKPATH = workpath;

            GetOPString(opstr);

            MainProcess = new ProcessClass();

            myJzTimes = new JzTimes();
            myJzTimes.Cut();
        }
        public override void GetOPString(string opstr)
        {
            string[] strs = opstr.Split(',');

            PLCCount = int.Parse(strs[0]);
            MotionCount = int.Parse(strs[1]);

            PLCCollection = new FatekPLCClass[PLCCount];

            CANMOTIONControl = new CanMotoControl();
            CANMOTIONCollection = new CanMotionClass[MotionCount];
        }
        public override bool Initial(bool isnouseio,bool isnousemotor)
        {
            int i = 0;
            bool ret = true;

            IsNoUseIO = isnouseio;
            IsNoUseMotor = isnousemotor;

            i = 0;
            while (i < PLCCount)
            {
                PLCCollection[i] = new FatekPLCClass();

                if (!isnouseio)
                    ret &= PLCCollection[i].Open(WORKPATH + "\\" + myMachineEA.ToString() + "\\PLCCONTROL" + i.ToString() + ".INI", isnouseio);

                PLCCollection[i].Name = "PLC" + i.ToString();
                PLCCollection[i].ReadAction += ReadAction;

                i++;
            }

            if (!IsNoUseMotor)
            {
                CANMOTIONControl.Initial(MotionCount, INI.IPSTR);

                i = 0;
                while (i < MotionCount)
                {
                    CANMOTIONCollection[i] = new CanMotionClass();

                    switch(i)
                    {
                        case 1:
                            CANMOTIONCollection[i].Initial(WORKPATH + "\\" + myMachineEA.ToString(), (MotionEnum)i, CANMOTIONControl, CANMOTIONControl.CanMotor[i], false, IsNoUseMotor);
                            break;
                        case 0:
                        case 2:
                            CANMOTIONCollection[i].Initial(WORKPATH + "\\" + myMachineEA.ToString(), (MotionEnum)i, CANMOTIONControl, CANMOTIONControl.CanMotor[i], true, IsNoUseMotor);
                            break;
                    }
                    i++;
                }
            }

            PLCIO = new JzDFlyPLCIOClass();
            PLCIO.Initial(WORKPATH + "\\" + myMachineEA.ToString(), OPTION, PLCCollection);

            return ret;
        }

        private void ReadAction(char[] readbuffer, string operationstring, string myname)
        {
            switch (myname)
            {
                case "PLC0":
                    PLC0ReadAction(readbuffer, operationstring);
                    break;
            }
        }
        void PLC0ReadAction(char[] readbuffer, string operationstring)
        {
            switch (operationstring)
            {
                case "Get All X":
                    PLC0GetAllX(readbuffer);
                    break;
                case "Get All Y":
                    PLC0GetAllY(readbuffer);
                    break;
            }
        }
        void PLC0GetAllX(char[] readbuffer)
        {
            String Str = new string(readbuffer, 6, 10); //X0000

            //UInt32 GetInt = HEX32(Str.Substring(0, 4));
            int i = 0;

            while (i < Str.Length)
            {
                bool ison = Str.Substring(i, 1) == "1";

                PLCCollection[0].IOData.SetXBit(i, ison);

                i++;
            }
        }
        void PLC0GetAllY(char[] readbuffer)
        {
            String Str = new string(readbuffer, 6, 10); //Y0000

            //UInt32 GetInt = HEX32(Str.Substring(0, 4));
            int i = 0;

            while (i < Str.Length)
            {
                //bool ison = (GetInt & (1 << i)) == (1 << i);
                bool ison = Str.Substring(i, 1) == "1";

                PLCCollection[0].IOData.SetYBit(i, ison);

                i++;
            }
        }
        public override void Tick()
        {
            if (myJzTimes.msDuriation < MSDuriation)
                return;

            if (CANMOTIONControl.CanMotor == null)
                return;

            CANMOTIONControl.Tick();

            CheckEvent();

            myJzTimes.Cut();
        }

        bool IsOldEMC = false;
        public override void CheckEvent()
        {
            //Trigger EMC
            bool isemcnow = PLCIO.IsEMC;

            if (isemcnow && IsOldEMC != isemcnow)
            {
                OnTrigger(MachineEventEnum.EMC);
            }
            IsOldEMC = isemcnow;

            //Check If PLC Disconnect
            foreach (FatekPLCClass plc in PLCCollection)
            {
                plc.Tick();
            }
        }
        public override void GetStart(bool isdirect, bool isnouseplc)
        {
            throw new NotImplementedException();
        }
        public override void SetDelayTime()
        {
            throw new NotImplementedException();
        }
        public override void MainProcessTick()
        {
            throw new NotImplementedException();
        }
        public void PLCRetry()
        {
            foreach (FatekPLCClass plc in PLCCollection)
            {
                plc.RetryConn();
            }
        }

        public override void GoHome()
        {
            CANMOTIONCollection[0].Home();
            CANMOTIONCollection[1].Home();
            CANMOTIONCollection[2].Home();
        }
        public string GetPosition()
        {
            string posstr = "";

            if (CANMOTIONCollection[0] == null)
            {
                posstr = "0,0,0";
            }
            else
            {
                posstr = CANMOTIONCollection[0].PositionNowString + ",";
                posstr += CANMOTIONCollection[1].PositionNowString + ",";
                posstr += CANMOTIONCollection[2].PositionNowString;
            }
            return posstr;
        }
        public void GoPosition(string str)
        {
            string[] strs = str.Split(',');

            CANMOTIONCollection[0].Go(float.Parse(strs[0]));
            CANMOTIONCollection[1].Go(float.Parse(strs[1]));
            CANMOTIONCollection[2].Go(float.Parse(strs[2]));
        }
        public bool ISAllOnSite()
        {
            return CANMOTIONCollection[0].IsOnSite && CANMOTIONCollection[1].IsOnSite && CANMOTIONCollection[2].IsOnSite;
        }
    }
}
