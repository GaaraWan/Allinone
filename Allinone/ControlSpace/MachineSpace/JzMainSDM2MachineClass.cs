using Allinone.ControlSpace.IOSpace;
using JetEazy;
using JetEazy.BasicSpace;
using JetEazy.ControlSpace;
using JetEazy.ControlSpace.MotionSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Allinone.ControlSpace.MachineSpace
{
    public class JzMainSDM3MachineClass : GeoMachineClass
    {
        private IntPtr handle
        {
            get { return mRobotHCFA.ApiHandle; }
        }

        const int MSDuriation = 10;

        public JzMainSDM3IOClass PLCIO;
        
        public JzMainSDM3MachineClass(Machine_EA machineea, VersionEnum version, OptionEnum option, string opstr, string workpath, bool isnouseplc)
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

            switch (strs[2])
            {
                case "HCFA0":
                    mRobotType = RobotType.HCFA;
                    break;
                default:
                    mRobotType = RobotType.NONE;
                    break;
            }

            if (PLCCount > 0)
                PLCCollection = new FatekPLCClass[PLCCount];
            if (MotionCount > 0)
                PLCMOTIONCollection = new PLCMotionClass[MotionCount];
        }
        public override bool Initial(bool isnouseio, bool isnousemotor)
        {
            int i = 0;
            bool ret = true;

            IsNoUseIO = isnouseio;
            IsNoUseMotor = isnousemotor;

            i = 0;
            while (i < PLCCount)
            {
                PLCCollection[i] = new FatekPLCClass();

                switch (mRobotType)
                {
                    case RobotType.HCFA:
                        break;
                    case RobotType.NONE:
                        if (!isnouseio)
                            ret &= PLCCollection[i].Open(WORKPATH + "\\" + myMachineEA.ToString() + "\\PLCCONTROL" + i.ToString() + ".INI", isnouseio);
                        break;
                }

               
                PLCCollection[i].Name = "PLC" + i.ToString();
                PLCCollection[i].ReadAction += ReadAction;
                PLCCollection[i].ReadListAction += DispensingMachineClass_ReadListAction;
                PLCCollection[i].ReadListUintAction += DispensingMachineClass_ReadListUintAction;
                PLCCollection[i].CommErrorStringAction += DispensingPLC_CommErrorStringAction;
                PLCCollection[i].ReadListUshortAction += MainLSMachineClass_ReadListUshortAction;

                i++;
            }

            i = 0;
            while (i < MotionCount)
            {
                PLCMOTIONCollection[i] = new PLCMotionClass();
                PLCMOTIONCollection[i].Intial(WORKPATH + "\\" + myMachineEA.ToString(), (MotionEnum)i, PLCCollection, IsNoUseMotor);

                switch (mRobotType)
                {
                    case RobotType.HCFA:
                        PLCMOTIONCollection[i].Intial(WORKPATH + "\\" + myMachineEA.ToString(), (MotionEnum)i, PLCCollection, true);

                        break;
                    case RobotType.NONE:
                        PLCMOTIONCollection[i].Intial(WORKPATH + "\\" + myMachineEA.ToString(), (MotionEnum)i, PLCCollection, IsNoUseMotor);

                        break;
                }

                i++;
            }

            PLCIO = new JzMainSDM3IOClass();
            PLCIO.Initial(WORKPATH + "\\" + myMachineEA.ToString(), OPTION, PLCCollection);

            switch (mRobotType)
            {
                case RobotType.HCFA:

                    mRobotHCFA = new JetEazy.ControlSpace.RobotSpace.RobotHCFA();
                    ret &= mRobotHCFA.Init(WORKPATH + "\\" + myMachineEA.ToString() + "\\RobotControl0" + ".INI", isnouseio);
                    PLCIO.RobotParaSetup(mRobotHCFA, mRobotType);

                    break;
            }

            return ret;
        }

        private void MainLSMachineClass_ReadListUshortAction(ushort[] readbuffer, string operationstring, string myname)
        {
            switch (myname)
            {
                case "PLC0":
                    PLC0ReadAction(readbuffer, operationstring);
                    break;
            }
        }
        void PLC0ReadAction(ushort[] readbuffer, string operationstring)
        {
            if (operationstring.Contains("Get MW"))
            {
                string[] vs = operationstring.Split('W');
                PLC0GetMWFX(readbuffer, int.Parse(vs[1]));
            }
        }
        void PLC0GetMWFX(ushort[] readbuffer, int index)
        {
            int i = 0;
            while (i < readbuffer.Length)
            {
                ushort ison = readbuffer[i];
                PLCCollection[0].IOData.SetMWData(index + i, (short)ison);

                int j = 0;
                while (j < 16)
                {
                    bool isonj = ((ison >> j) % 2) == 1;

                    PLCCollection[0].IOData.SetMXBit(index + i * 16 + j, isonj);

                    j++;
                }

                i++;
            }
        }

        private void DispensingPLC_CommErrorStringAction(string str)
        {
            //MachineCommError(str);
        }

        private void DispensingMachineClass_ReadListUintAction(short[] readbuffer, string operationstring, string myname)
        {
            switch (myname)
            {
                case "PLC0":
                    PLC0ReadAction(readbuffer, operationstring);
                    break;
            }
        }
        void PLC0ReadAction(short[] readbuffer, string operationstring)
        {
            //if (operationstring.Contains("Get MW"))
            //{
            //    string[] vs = operationstring.Split('W');
            //    PLC0GetMWFX(int.Parse(vs[1]), readbuffer);
            //}
            //switch (operationstring)
            //{
            //    case "Get MW0":
            //        PLC0GetMW0000(readbuffer);
            //        break;
            //    case "Get MW1000":
            //        PLC0GetMW1000(readbuffer);
            //        break;
            //    case "Get MW1300":
            //        PLC0GetMW1300(readbuffer);
            //        break;
            //    case "Get MW1320":
            //        PLC0GetMW1320(readbuffer);
            //        break;
            //}
        }
        void PLC0GetMW0000(short[] readbuffer)
        {
            int i = 0;
            while (i < readbuffer.Length)
            {
                short ison = readbuffer[i];
                PLCCollection[0].IOData.SetMWData(0 + i, ison);

                //UInt32 GetInt = ison;
                int j = 0;
                while (j < 16)
                {
                    bool isonj = ((ison >> j) % 2) == 1;

                    PLCCollection[0].IOData.SetMXBit(0 + i * 16 + j, isonj);

                    j++;
                }

                i++;
            }
        }
        void PLC0GetMW1000(short[] readbuffer)
        {
            int i = 0;
            while (i < readbuffer.Length)
            {
                short ison = readbuffer[i];
                PLCCollection[0].IOData.SetMWData(1000 + i, ison);

                i++;
            }
        }
        void PLC0GetMW1300(short[] readbuffer)
        {
            int i = 0;
            while (i < readbuffer.Length)
            {
                short ison = readbuffer[i];
                PLCCollection[0].IOData.SetMWData(1300 + i, ison);

                i++;
            }
        }
        void PLC0GetMW1320(short[] readbuffer)
        {
            int i = 0;
            while (i < readbuffer.Length)
            {
                short ison = readbuffer[i];
                PLCCollection[0].IOData.SetMWData(1320 + i, ison);

                i++;
            }
        }

        private void DispensingMachineClass_ReadListAction(bool[] readbuffer, string operationstring, string myname)
        {
            switch (myname)
            {
                case "PLC0":
                    PLC0ReadAction(readbuffer, operationstring);
                    break;
            }
        }
        void PLC0ReadAction(bool[] readbuffer, string operationstring)
        {
            if (operationstring.Contains("Get QX"))
            {
                string[] vs = operationstring.Split('X');
                PLC0GetQXFX(int.Parse(vs[1]), readbuffer);
            }
            else if (operationstring.Contains("Get IX"))
            {
                string[] vs = operationstring.Split('X');
                PLC0GetIXFX(int.Parse(vs[1]), readbuffer);
            }
        }
        void PLC0GetQXFX(int iaddress, bool[] readbuffer)
        {
            int i = 0;
            while (i < readbuffer.Length)
            {
                bool ison = readbuffer[i];
                PLCCollection[0].IOData.SetQXBit(iaddress * 8 + i, ison);

                i++;
            }
        }
        void PLC0GetIXFX(int iaddress, bool[] readbuffer)
        {

            int i = 0;
            while (i < readbuffer.Length)
            {
                bool ison = readbuffer[i];
                PLCCollection[0].IOData.SetIXBit(iaddress * 8 + i, ison);

                i++;
            }
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
                case "Get All M":
                    PLC0GetAllMEX(readbuffer);
                    break;
                case "Get All M992":
                    PLC0GetAllMEX(readbuffer, 992);
                    break;
                case "Get All M32":
                    PLC0GetAllMEX(readbuffer, 32);
                    break;
                case "Get All X":
                    PLC0GetAllX(readbuffer);
                    break;
                case "Get All Y":
                    PLC0GetAllY(readbuffer);
                    break;
                case "Get Data P1X":
                    PLC0GetData(readbuffer, 102, 2);
                    break;
                case "Get Data P2X":
                    PLC0GetData(readbuffer, 202, 2);
                    break;
                case "Get Data P3X":
                    PLC0GetData(readbuffer, 302, 2);
                    break;
            }
        }
        void PLC0GetAllX(char[] readbuffer)
        {
            String Str = new string(readbuffer, 6, 10); //X0000

            UInt32 GetInt = HEX32(Str);
            int i = 0;
            while (i < 32)
            {
                bool ison = ((GetInt >> i) % 2) == 1;

                PLCCollection[0].IOData.SetXBit(0 + i, ison);

                i++;
            }

            //UInt32 GetInt = HEX32(Str.Substring(0, 4));
            //int i = 0;

            //while (i < Str.Length)
            //{
            //    bool ison = Str.Substring(i, 1) == "1";

            //    PLCCollection[0].IOData.SetXBit(i, ison);

            //    i++;
            //}
        }
        void PLC0GetAllY(char[] readbuffer)
        {
            String Str = new string(readbuffer, 6, 10); //Y0000
            UInt32 GetInt = HEX32(Str);
            // string Yio = Convert.ToString(GetInt, 2);
            int i = 0;
            while (i < 32)
            {
                bool ison = ((GetInt >> i) % 2) == 1;

                PLCCollection[0].IOData.SetYBit(0 + i, ison);

                i++;
            }

            //UInt32 GetInt = HEX32(Str.Substring(0, 4));
            //int i = 0;

            //while (i < Str.Length)
            //{
            //    //bool ison = (GetInt & (1 << i)) == (1 << i);
            //    bool ison = Str.Substring(i, 1) == "1";

            //    PLCCollection[0].IOData.SetYBit(i, ison);

            //    i++;
            //}
        }
        void PLC0GetAllMEX(char[] readbuffer, int indexAddr = 0)
        {
            String Str = new string(readbuffer, 6, 8); //M0048

            UInt32 GetInt = HEX32(Str);
            int i = 0;

            while (i < 32)
            {
                bool ison = ((GetInt >> i) % 2) == 1;

                PLCCollection[0].IOData.SetMBit(indexAddr + i, ison);

                i++;
            }

            //Str = new string(readbuffer, 14, 8); //M00
            //GetInt = HEX32(Str);
            //i = 0;

            //while (i < 32)
            //{
            //    bool ison = ((GetInt >> i) % 2) == 1;

            //    PLCCollection[0].IOData.SetMBit(1040 + i, ison);
            //    i++;
            //}

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
        void PLC0GetData(char[] readbuffer, int offsetaddress, int wordcount)
        {
            String Str = new string(readbuffer, 6, 4 * wordcount);
            UInt16 GetInt = 0;
            int i = 0;
            while (i < wordcount)
            {
                GetInt = HEX16(Str.Substring(i * 4, 4));
                PLCCollection[0].IOData.SetRData(offsetaddress + i, (int)GetInt);

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

        bool IsOlduserStart = false;
        bool IsOlduserStop = false;
        int ierrorcodetemp = 0;

        bool IsOldAlarm = false;
        bool IsOldStart = false;
        bool IsOldEMC = false;
        bool IsOldReset = false;
        bool IsInitHiveclient = false;
        /// <summary>
        /// 记录当前的机器状态
        /// </summary>
        MachineState m_machinestate_tmp = MachineState.Running;
        public MachineState GetCurrentMachineState
        {
            get { return m_machinestate_tmp; }
        }
        public bool MachineStateForPlannedDown
        {
            set
            {
                m_machinestate_tmp = (value ? MachineState.Planned_downtime : MachineState.Idle);
                PLCIO.Green = (int)m_machinestate_tmp == 1;
                PLCIO.Yellow = (int)m_machinestate_tmp == 2;
                //PLCIO.Blue = (int)m_machinestate_tmp == 3;
                //PLCIO.White = (int)m_machinestate_tmp == 4;
                PLCIO.Red = (int)m_machinestate_tmp == 5;
                //HIVECLIENT.Hiveclient_MachineState((int)m_machinestate_tmp, !IsInitHiveclient);
            }
        }
        /// <summary>
        /// 設定5色燈狀態，任何情況只有一個燈亮
        /// </summary>
        /// <param name="m_machinestate">狀態</param>
        public void SetMachineState(MachineState m_machinestate)
        {
            //將來吧
            if (m_machinestate_tmp == MachineState.Planned_downtime)
                return;

            if (m_machinestate_tmp != m_machinestate)
            {
                PLCIO.Green = (int)m_machinestate == 1;
                PLCIO.Yellow = (int)m_machinestate == 2;
                //PLCIO.Blue = (int)m_machinestate == 3;
                //PLCIO.White = (int)m_machinestate == 4;
                PLCIO.Red = (int)m_machinestate == 5;

                //HIVECLIENT.Hiveclient_MachineState((int)m_machinestate, !IsInitHiveclient);

                if (!IsInitHiveclient)
                    IsInitHiveclient = true;
                //将切换的状态缓存下来，以便达到状态变化时再改变灯的颜色。
                m_machinestate_tmp = m_machinestate;
            }
        }

        public void SetLight(string lightstr)
        {
            if (lightstr == "")
            {
                PLCIO.TopLight = false;
                PLCIO.CoaxialLight = false;
            }
            else
            {
                string[] strs = lightstr.Split(',');

                if (strs.Length < 5)
                {
                    lightstr = "1,1,1,1,1";
                    strs = lightstr.Split(',');
                }

                PLCIO.TopLight = strs[0] == "1";
                PLCIO.CoaxialLight = strs[1] == "1";
            }
        }
        public override void GoHome()
        {
        }
        public override void CheckEvent()
        {
            switch (mRobotType)
            {
                case RobotType.HCFA:

                    //mRobotHCFA.Tick();

                    #region USER CONTROL

                    bool isuserstartnow = PLCIO.IsUserStart;

                    if (isuserstartnow && IsOlduserStart != isuserstartnow)
                    {
                        //PLCIO.Ready = true;
                        PLCIO.RobotEnable = true;
                    }
                    IsOlduserStart = isuserstartnow;

                    bool isuserstopnow = PLCIO.IsUserStop;

                    if (isuserstopnow && IsOlduserStop != isuserstopnow)
                    {
                        PLCIO.Pass = false;
                        PLCIO.Ready = false;
                        PLCIO.RobotEnable = false;
                        OnTrigger(MachineEventEnum.USER_STOP);
                    }
                    IsOlduserStop = isuserstopnow;

                    int ierrorcode = PLCIO.RobotErrorCode;

                    if (ierrorcodetemp != ierrorcode)
                    {
                        if (ierrorcode != 0)
                            OnTrigger(MachineEventEnum.ALARM_ROBOT);
                    }
                    ierrorcodetemp = ierrorcode;

                    #endregion

                    break;
                default:
                    break;
            }

            //Trigger Start
            bool isstartnow = PLCIO.IsStart;

            if (isstartnow && IsOldStart != isstartnow)
            {
                //如果产品到位则将皮带停止
                if (PLCIO.Ready)
                {
                    PLCIO.Pass = true;
                    OnTrigger(MachineEventEnum.START);
                }
                //no test
                //OnTrigger(MachineEventEnum.START);
            }
            IsOldStart = isstartnow;

            //Trigger EMC
            bool isemcnow = PLCIO.IsEMC;

            if (isemcnow && IsOldEMC != isemcnow)
            {
                PLCIO.Pass = false;
                PLCIO.Ready = false;
                OnTrigger(MachineEventEnum.EMC);
            }
            IsOldEMC = isemcnow;

            //Trigger RESET
            bool isresetnow = PLCIO.IsRESET;

            if (isresetnow && IsOldReset != isresetnow)
            {
                OnTrigger(MachineEventEnum.RESET);
            }
            IsOldReset = isresetnow;

            //Trigger RESET
            bool isalarmnow = PLCIO.IsALARM;

            if (isalarmnow && IsOldAlarm != isalarmnow)
            {
                OnTrigger(MachineEventEnum.ALARM);
            }
            IsOldAlarm = isalarmnow;

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

        public string GetPosition()
        {
            string posstr = "";

            switch (mRobotType)
            {
                case RobotType.HCFA:
                    double cx = mRobotHCFA.CurrX;
                    double cy = mRobotHCFA.CurrY;
                    double cz = mRobotHCFA.CurrZ;
                    //float cc = (float)frame.rot[2] * 180 / Math.PI;

                    posstr = cx.ToString("0.000");
                    if (PLCMOTIONCollection.Length > 1)
                        posstr += "," + cy.ToString("0.000");
                    if (PLCMOTIONCollection.Length > 2)
                        posstr += "," + cz.ToString("0.000");

                    break;
                case RobotType.NONE:
                    posstr = PLCMOTIONCollection[0].PositionNowString;
                    if (PLCMOTIONCollection.Length > 1)
                        posstr += "," + PLCMOTIONCollection[1].PositionNowString;
                    if (PLCMOTIONCollection.Length > 2)
                        posstr += "," + PLCMOTIONCollection[2].PositionNowString;

                    break;
            }

            
            return posstr;
        }
        public void GoReadyPosition()
        {
            float x = PLCMOTIONCollection[0].READYPOSITION;
            float y = PLCMOTIONCollection[1].READYPOSITION;
            float z = PLCMOTIONCollection[2].READYPOSITION;

            switch (mRobotType)
            {
                case RobotType.HCFA:

                    mRobotHCFA.MoveTo(x, y, z);

                    break;
                default:
                    PLCMOTIONCollection[0].Go(x);
                    if (PLCMOTIONCollection.Length > 1)
                        PLCMOTIONCollection[1].Go(y);
                    if (PLCMOTIONCollection.Length > 2)
                        PLCMOTIONCollection[2].Go(z);
                    break;
            }
        }
        public string GetReadyPosition()
        {
            string str = string.Empty;

            switch (mRobotType)
            {
                case RobotType.HCFA:

                    str = $"{PLCMOTIONCollection[0].READYPOSITION}," +
               $"{PLCMOTIONCollection[1].READYPOSITION}," +
               $"{PLCMOTIONCollection[2].READYPOSITION}";

                    break;
                default:
                    str = $"{PLCMOTIONCollection[0].READYPOSITION}," +
                $"{PLCMOTIONCollection[1].READYPOSITION}," +
                $"{PLCMOTIONCollection[2].READYPOSITION}";
                    break;
            }

            return str;
            //PLCMOTIONCollection[0].Go(PLCMOTIONCollection[0].READYPOSITION);
            //if (PLCMOTIONCollection.Length > 1)
            //    PLCMOTIONCollection[1].Go(PLCMOTIONCollection[1].READYPOSITION);
            //PLCMOTIONCollection[1].Go(PLCMOTIONCollection[1].READYPOSITION);
        }
        public void GoPosition(string str, bool isMove = true)
        {
            string[] strs = str.Split(',');

            float x = (float)Math.Round(float.Parse(strs[0]), 2);
            float y = (float)Math.Round(float.Parse(strs[1]), 2);
            float z = (float)Math.Round(float.Parse(strs[2]), 2);

            switch (mRobotType)
            {
                case RobotType.HCFA:
                    mRobotHCFA.MoveTo(x, y, z);
                    break;
                case RobotType.NONE:
                    PLCMOTIONCollection[0].Go(x);
                    if (strs.Length > 1)
                        PLCMOTIONCollection[1].Go(y);
                    if (strs.Length > 2)
                        PLCMOTIONCollection[2].Go(z);
                    //System.Threading.Thread.Sleep(10);
                    if (isMove)
                        PLCIO.RobotAbs = true;
                    break;
            }


        }
        public bool IsOnSite()
        {
            bool bOK = false;

            switch (mRobotType)
            {
                case RobotType.HCFA:
                    bOK = !mRobotHCFA.RobotRunning;
                    //bOK = true;
                    break;
                case RobotType.NONE:
                    bOK = PLCMOTIONCollection[0].IsOnSite
                                && PLCMOTIONCollection[1].IsOnSite
                                && PLCMOTIONCollection[2].IsOnSite;
                                
                    break;
            }
            return bOK;

        }

        public bool IsOnSitePosition(string eCurrentPositionStr)
        {
            bool bOK = false;

            string[] strs = eCurrentPositionStr.Split(',');
            float x = (float)Math.Round(float.Parse(strs[0]), 2);
            float y = (float)Math.Round(float.Parse(strs[1]), 2);
            float z = (float)Math.Round(float.Parse(strs[2]), 2);

            switch (mRobotType)
            {
                case RobotType.HCFA:
                    double cx = mRobotHCFA.CurrX;
                    double cy = mRobotHCFA.CurrY;
                    double cz = mRobotHCFA.CurrZ;
                    bOK = IsInRange(cx, x, 0.05) && IsInRange(cy, y, 0.05) && IsInRange(cz, z, 0.05);
                    break;
                case RobotType.NONE:
                    bOK = IsInRange(PLCMOTIONCollection[0].PositionNow, x, 0.05)
                && IsInRange(PLCMOTIONCollection[1].PositionNow, y, 0.05)
                && IsInRange(PLCMOTIONCollection[2].PositionNow, z, 0.05)
                ;
                    break;
            }

            return bOK;
        }

        public void XHome()
        {
            PLCMOTIONCollection[0].Home();
        }
        public bool IsXHome()
        {
            return PLCMOTIONCollection[0].IsHome;//&& PLCMOTIONCollection[1].IsOnSite;
        }
        public void YHome()
        {
            PLCMOTIONCollection[1].Home();
        }
        public bool IsYHome()
        {
            return PLCMOTIONCollection[1].IsHome;//&& PLCMOTIONCollection[1].IsOnSite;
        }
        public void PLCRetry()
        {
            switch (mRobotType)
            {
                case RobotType.HCFA:
                    mRobotHCFA.RetryConnect();
                    break;
                default:
                    foreach (FatekPLCClass plc in PLCCollection)
                    {
                        plc.RetryConn();
                    }
                    break;
            }
        }
        public string Fps()
        {
            string Str = string.Empty;

            switch (mRobotType)
            {
                case RobotType.HCFA:
                    Str += mRobotHCFA.SerialCount.ToString();
                    break;
                case RobotType.NONE:
                    foreach (FatekPLCClass plc in PLCCollection)
                    {
                        Str += plc.SerialCount.ToString() + ";";
                    }
                    break;
            }
            return Str;
        }
        public void Dispose()
        {
            switch(mRobotType)
            {
                case RobotType.HCFA:
                    mRobotHCFA.Dispose();
                    break;
            }
        }

        public bool IsInRange(double FromValue, double CompValue, double DiffValue)
        {
            return Math.Abs(FromValue - CompValue) < DiffValue;
        }
        public bool IsInRange(float FromValue, float CompValue, float DiffValue)
        {
            return Math.Abs(FromValue - CompValue) < DiffValue;
        }

    }
}
