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

namespace Allinone.ControlSpace.MachineSpace
{
    public class JzAllinoneMachineClass : GeoMachineClass
    {
        const int MSDuriation = 300;

        public JzAllinoneIOClass PLCIO;
        public EventClass EVENT;
        public JzAllinoneMachineClass(Machine_EA machineea, VersionEnum version, OptionEnum option,string opstr,string workpath,bool isnouseplc)
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

            if (PLCCount > 0)
                PLCCollection = new FatekPLCClass[PLCCount];

            if (MotionCount > 0)
                PLCMOTIONCollection = new PLCMotionClass[MotionCount];
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

            i = 0;
            while(i < MotionCount)
            {
                PLCMOTIONCollection[i] = new PLCMotionClass();
                PLCMOTIONCollection[i].Intial(WORKPATH + "\\" + myMachineEA.ToString(), (MotionEnum)i, PLCCollection, IsNoUseMotor);

                i++;
            }

            PLCIO = new JzAllinoneIOClass();
            PLCIO.Initial(WORKPATH + "\\" + myMachineEA.ToString(), OPTION, PLCCollection);

            EVENT = new EventClass(WORKPATH + "\\EVENT.jdb");

            return ret;
        }

        private void ReadAction(char[] readbuffer, string operationstring, string myname)
        {
            switch(myname)
            {
                case "PLC0":
                    PLC0ReadAction(readbuffer, operationstring);
                    break;
            }
        }
        void PLC0ReadAction(char[] readbuffer, string operationstring)
        {
            switch(operationstring)
            {
                case "Get Alarms":
                    PLC0GetAlarms(readbuffer, 550, 3);
                    break;
                case "Get MOTION M":
                    PLC0GetMOTIONM(readbuffer);
                    break;
                case "Get All M":
                    PLC0GetAllMEX(readbuffer);
                    break;
                case "Get Data P1X":
                    PLC0GetData(readbuffer, 502, 2);
                    break;
                case "Get Data P2X":
                    PLC0GetData(readbuffer, 514, 2);
                    break;
                case "Get Data P3X":
                    PLC0GetData(readbuffer, 526, 2);
                    break;
                case "Get Data P1":
                    PLC0GetData(readbuffer, 496, 8);
                    break;
                case "Get Data P2":
                    PLC0GetData(readbuffer, 504, 8);
                    break;
                case "Get Data P3":
                    PLC0GetData(readbuffer, 512, 8);
                    break;
                case "Get Data P4":
                    PLC0GetData(readbuffer, 520, 8);
                    break;
                case "Get Data P5":
                    PLC0GetData(readbuffer, 528, 8);
                    break;
                case "Get Data P6":
                    PLC0GetData(readbuffer, 536, 8);
                    break;
            }
        }
        void PLC0GetAllM(char [] readbuffer)
        {
            String Str = new string(readbuffer, 6, 8); //M1008

            UInt32 GetInt = HEX32(Str.Substring(0,4));
            int i = 0;

            while (i < 16)
            {
                bool ison = (GetInt & (1 << i)) == (1 << i);

                PLCCollection[0].IOData.SetMBit(1008 + i, ison);

                i++;
            }

            GetInt = HEX32(Str.Substring(4, 4));
            i = 0;

            while (i < 16)
            {
                bool ison = (GetInt & (1 << i)) == (1 << i);

                PLCCollection[0].IOData.SetMBit(1024 + i, ison);

                i++;
            }


            Str = new string(readbuffer, 14, 8); //M1040
            GetInt = HEX32(Str.Substring(0,4));
            i = 0;

            while (i < 16)
            {
                bool ison = (GetInt & (1 << i)) == (1 << i);

                PLCCollection[0].IOData.SetMBit(1040 + i, ison);
                i++;
            }

            GetInt = HEX32(Str.Substring(4, 4));
            i = 0;

            while (i < 16)
            {
                bool ison = (GetInt & (1 << i)) == (1 << i);

                PLCCollection[0].IOData.SetMBit(1056 + i, ison);
                i++;
            }

            Str = new string(readbuffer, 22, 8); //M1200
            GetInt = HEX32(Str.Substring(0,4));
            i = 0;

            while (i < 16)
            {
                bool ison = (GetInt & (1 << i)) == (1 << i);

                PLCCollection[0].IOData.SetMBit(1200 + i, ison);
                i++;
            }

            GetInt = HEX32(Str.Substring(4, 4));
            i = 0;

            while (i < 16)
            {
                bool ison = (GetInt & (1 << i)) == (1 << i);

                PLCCollection[0].IOData.SetMBit(1216 + i, ison);
                i++;
            }
          
        }

        void PLC0GetMOTIONM(char[] readbuffer)
        {
            String Str = new string(readbuffer, 6, 8); //M0496

            UInt32 GetInt = HEX32(Str);
            int i = 0;

            while (i < 32)
            {
                bool ison = ((GetInt >> i) % 2) == 1;

                PLCCollection[0].IOData.SetMBit(496 + i, ison);
                
                i++;
            }

            Str = new string(readbuffer, 14, 8); //M0528
            GetInt = HEX32(Str);
            i = 0;

            while (i < 32)
            {
                bool ison = ((GetInt >> i) % 2) == 1;

                PLCCollection[0].IOData.SetMBit(528 + i, ison);
                i++;
            }

            //Str = new string(readbuffer, 22, 8); //M1200
            //GetInt = HEX32(Str);
            //i = 0;

            //while (i < 32)
            //{
            //    bool ison = ((GetInt >> i) % 2) == 1;

            //    PLCCollection[0].IOData.SetMBit(1200 + i, ison);
            //    i++;
            //}

        }
        void PLC0GetAllMEX(char[] readbuffer)
        {
            String Str = new string(readbuffer, 6, 8); //M1008

            UInt32 GetInt = HEX32(Str);
            int i = 0;

            while (i < 32)
            {
                bool ison = ((GetInt >> i) % 2) == 1;

                PLCCollection[0].IOData.SetMBit(1008 + i, ison);

                i++;
            }

            Str = new string(readbuffer, 14, 8); //M1040
            GetInt = HEX32(Str);
            i = 0;

            while (i < 32)
            {
                bool ison = ((GetInt >> i) % 2) == 1;

                PLCCollection[0].IOData.SetMBit(1040 + i, ison);
                i++;
            }
            
            Str = new string(readbuffer, 22, 8); //M1200
            GetInt = HEX32(Str);
            i = 0;

            while (i < 32)
            {
                bool ison = ((GetInt >> i) % 2) == 1;

                PLCCollection[0].IOData.SetMBit(1200 + i, ison);
                i++;
            }
            
        }
        void PLC0GetData(char[] readbuffer, int offsetaddress,int wordcount)
        {
            String Str = new string(readbuffer, 6, 4 * wordcount);
            UInt16 GetInt = 0;
            int i = 0;
            while(i < wordcount)
            {
                GetInt = HEX16(Str.Substring(i * 4, 4));
                PLCCollection[0].IOData.SetRData(offsetaddress + i, (int)GetInt);

                i++;
            }
        }
        void PLC0GetAlarms(char[] readbuffer, int offsetaddress, int wordcount)
        {
            String Str = new string(readbuffer, 6, 4 * wordcount);
            UInt16 GetInt = 0;
            int i = 0;
            int j = 0;
            while (i < wordcount)
            {
                GetInt = HEX16(Str.Substring(i * 4, 4));
                j = 0;
                while (j < 16)
                {
                    bool ison = ((GetInt >> j) % 2) == 1;
                    PLCCollection[0].IOData.SetMBit(1280 + j + i * 16, ison);
                    //Gaara DEBUG
                    string strlog = "M" + (1280 + j + i * 16).ToString("0000") + ":" + (ison ? "true" : "false");
                    System.Diagnostics.Debug.WriteLine(strlog);
                    j++;
                }

                i++;
            }
        }
        public override void Tick()
        {
            if (myJzTimes.msDuriation < MSDuriation)
                return;

            CheckEvent();

            myJzTimes.Cut();
        }

        bool IsOldAutoStart = false;
        bool IsOldStart = false;
        bool IsOldEMC = false;
        bool IsOldCurtain = false;

        #region Alarms Define

        bool IsOldAlarmSerious = false;
        bool IsOldAlarmCommon = false;
        bool IsSureClear = false;
        JzTimes jztimeClear = new JzTimes();

        public void ResetAutoStart()
        {
            IsOldAutoStart = false;
        }

        public bool ClearAlarm
        {
            set
            {
                if (value)
                {
                    IsOldAlarmSerious = false;
                    IsOldAlarmCommon = false;
                    IsSureClear = true;
                    jztimeClear.Cut();
                }
                PLCIO.CLEARALARMS = value;
            }
        }

        #endregion

        public override void CheckEvent()
        {
            if (!IsSureClear)
            {
                //jztimeClear.Cut();
                //Trigger ALARM SERIOUS
                bool isalarmserioousnow = PLCIO.IsAlarmsSerious;

                if (isalarmserioousnow && IsOldAlarmSerious != isalarmserioousnow)
                {
                    OnTrigger(MachineEventEnum.ALARM_SERIOUS);
                }
                IsOldAlarmSerious = isalarmserioousnow;

                //Trigger ALARM COMMON
                bool isalarmcommonnow = PLCIO.IsAlarmsCommon;

                if (isalarmcommonnow && IsOldAlarmCommon != isalarmcommonnow)
                {
                    OnTrigger(MachineEventEnum.ALARM_COMMON);
                }
                IsOldAlarmCommon = isalarmcommonnow;
            }
            else
            {
                if (jztimeClear.msDuriation >= 500)
                {
                    IsSureClear = false;
                    if (IsOldAlarmSerious || IsOldAlarmCommon)
                    {
                        IsOldAlarmSerious = false;
                        IsOldAlarmCommon = false;
                    }
                    jztimeClear.Cut();
                }
            }

            //Trigger Auto Start
            bool isautostartnow = PLCIO.IsFeedComplete;

            if (isautostartnow && IsOldAutoStart != isautostartnow)
            {
                PLCIO.FeedPctoPlcSensor = true;
                OnTrigger(MachineEventEnum.AUTOSTART);
            }
            IsOldAutoStart = isautostartnow;

            //Trigger Start
            bool isstartnow = PLCIO.IsStart;

            if (isstartnow && IsOldStart != isstartnow)
            {
                OnTrigger(MachineEventEnum.START);
            }
            IsOldStart = isstartnow;

            //Trigger EMC
            bool isemcnow = PLCIO.IsEMC;

            if (isemcnow && IsOldEMC != isemcnow)
            {
                OnTrigger(MachineEventEnum.EMC);
            }
            IsOldEMC = isemcnow;

            //Trigger Curtain
            bool iscurtainnow = PLCIO.IsLightCurtain;

            if (iscurtainnow && IsOldCurtain != iscurtainnow)
            {
                OnTrigger(MachineEventEnum.CURTAIN);
            }
            IsOldCurtain = iscurtainnow;

            //Check If PLC Disconnect
            foreach(FatekPLCClass plc in PLCCollection)
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

        public void SetLight(string lightstr)
        {
            if (lightstr == "")
            {
                PLCIO.TopLight = false;
                PLCIO.MylarLight = false;
            }
            else
            {
                string[] strs = lightstr.Split(',');

                PLCIO.TopLight = strs[0] == "1";
                PLCIO.MylarLight = strs[1] == "1";
            }
        }
        public string GetPosition()
        {
            string posstr = "";

            posstr = PLCMOTIONCollection[0].PositionNowString + ",";
            posstr += PLCMOTIONCollection[1].PositionNowString;

            return posstr;
        }
        public void GoPosition(string str)
        {
            string[] strs = str.Split(',');
            
            PLCMOTIONCollection[0].Go(float.Parse(strs[0]));
            PLCMOTIONCollection[1].Go(float.Parse(strs[1]));
        }
        public void GoReadyPosition()
        {
            PLCMOTIONCollection[0].Go(PLCMOTIONCollection[0].READYPOSITION);
            PLCMOTIONCollection[1].Go(PLCMOTIONCollection[1].READYPOSITION);
        }
        public void GoXPosition(string str)
        {
            string[] strs = str.Split(',');

            PLCMOTIONCollection[0].Go(float.Parse(strs[0]));
            //PLCMOTIONCollection[1].Go(float.Parse(strs[1]));
        }
        public void GoXReadyPosition()
        {
            PLCMOTIONCollection[0].Go(PLCMOTIONCollection[0].READYPOSITION);
            //PLCMOTIONCollection[1].Go(PLCMOTIONCollection[1].READYPOSITION);
        }
        public bool ISAllOnSite()
        {
            return PLCMOTIONCollection[0].IsOnSite && PLCMOTIONCollection[1].IsOnSite;
        }

        public override void GoHome()
        {
            PLCMOTIONCollection[0].Home();
            PLCMOTIONCollection[1].Home();
        }
        public void PLCRetry()
        {
            foreach(FatekPLCClass plc in PLCCollection)
            {
                plc.RetryConn();
            }
        }

        
    }
}
