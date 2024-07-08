using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JetEazy;
using JetEazy.BasicSpace;
using JetEazy.ControlSpace;
using JetEazy.ControlSpace.MotionSpace;
//using JetEazy.ControlSpace.PLCSpace;

using Allinone.ControlSpace.IOSpace;

namespace Allinone.ControlSpace.MachineSpace
{
    public class JzMainSDMachineClass : GeoMachineClass
    {
        const int MSDuriation = 100;

        public JzMainSDIOClass PLCIO;

        public EventClass EVENT;
        public JzHiveClass HIVECLIENT
        {
            get { return Universal.JZHIVECLIENT; }
        }
        public JzMainSDMachineClass(Machine_EA machineea, VersionEnum version, OptionEnum option,string opstr,string workpath,bool isnouseplc)
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

            //switch (OPTION)
            //{
            //    case OptionEnum.MAIN_SD:
            //        PLCCollection = new FatekPLCClass[0];

            //        if (PLCCount > 0)
            //            PLCCollection = new Mitsubishi_FX3UClass[PLCCount];
            //        break;
            //    default:
                    
            //        break;
            //}

            if(MotionCount > 0)
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
                switch (OPTION)
                {
                    //case OptionEnum.MAIN_SD:
                    //    //PLCCollection[i] = new Mitsubishi_FX3UClass();
                    //    //if (!isnouseio)
                    //    //    ret &= PLCCollection[i].Open(WORKPATH + "\\" + myMachineEA.ToString() + "\\PLCCONTROL" + i.ToString() + ".INI", isnouseio);

                    //    //PLCCollection[i].Name = "PLC" + i.ToString();
                    //    //PLCCollection[i].ReadAction += ReadAction;
                    //    break;
                    default:
                        PLCCollection[i] = new FatekPLCClass();
                        if (!isnouseio)
                            ret &= PLCCollection[i].Open(WORKPATH + "\\" + myMachineEA.ToString() + "\\PLCCONTROL" + i.ToString() + ".INI", isnouseio);

                        PLCCollection[i].Name = "PLC" + i.ToString();
                        PLCCollection[i].ReadAction += ReadAction;
                        break;
                }
                
                i++;
            }

            i = 0;
            while(i < MotionCount)
            {
                PLCMOTIONCollection[i] = new PLCMotionClass();
                PLCMOTIONCollection[i].Intial(WORKPATH + "\\" + myMachineEA.ToString(), (MotionEnum)i, PLCCollection, IsNoUseMotor);

                i++;
            }
            switch (OPTION)
            {
                //case OptionEnum.MAIN_SD:
                //    PLCIO = new JzMainSDIOClass();
                //    PLCIO.Initial(WORKPATH + "\\" + myMachineEA.ToString(), OPTION, PLCCollection);
                //    break;
                default:
                    PLCIO = new JzMainSDIOClass();
                    PLCIO.Initial(WORKPATH + "\\" + myMachineEA.ToString(), OPTION, PLCCollection);
                    break;
            }

            EVENT = new EventClass(WORKPATH + "\\EVENT.jdb");

            return ret;
        }

        private void ReadAction(char[] readbuffer, string operationstring, string myname)
        {
            switch (OPTION)
            {
                case OptionEnum.MAIN_SD:

                    switch (myname)
                    {
                        case "PLC0":
                            PLC0FX1NReadAction(readbuffer, operationstring);
                            break;
                    }

                    break;
                default:

                    switch (myname)
                    {
                        case "PLC0":
                            PLC0ReadAction(readbuffer, operationstring);
                            break;
                    }

                    break;
            }
        }

        #region 三菱 FX3U

        void PLC0FX1NReadAction(char[] readbuffer, string operationstring)
        {
            switch (operationstring)
            {
                case "Get FX M0":
                    GET_FX_MXXX(readbuffer, 0);
                    break;
                case "Get FX M120":
                    GET_FX_MXXX(readbuffer, 120);
                    break;
                case "Get FX M240":
                    GET_FX_MXXX(readbuffer, 240);
                    break;
                case "Get FX M480":
                    GET_FX_MXXX(readbuffer, 480);
                    break;
                //case "Get FX D0":
                //    GET_FX_D0(readbuffer);
                //    break;

                #region AXIS CURRENT POSITION

                case "Get FX2 D000":
                    GET_FX_DXXX(readbuffer, 0, 2);
                    break;

                case "Get FX D200":
                    GET_FX_DXXX(readbuffer, 200);
                    break;
                case "Get FX2 D200":
                    GET_FX_DXXX(readbuffer, 200, 2);
                    break;
                case "Get FX D220":
                    GET_FX_DXXX(readbuffer, 220);
                    break;
                case "Get FX D240":
                    GET_FX_DXXX(readbuffer, 240);
                    break;
                case "Get FX D260":
                    GET_FX_DXXX(readbuffer, 260);
                    break;
                case "Get FX D280":
                    GET_FX_DXXX(readbuffer, 280);
                    break;


                case "Get FX D300":
                    GET_FX_DXXX(readbuffer, 300);
                    break;
                case "Get FX D320":
                    GET_FX_DXXX(readbuffer, 320);
                    break;
                case "Get FX2 D320":
                    GET_FX_DXXX(readbuffer, 320, 2);
                    break;
                case "Get FX D340":
                    GET_FX_DXXX(readbuffer, 340);
                    break;
                case "Get FX2 D340":
                    GET_FX_DXXX(readbuffer, 340, 2);
                    break;
                case "Get FX D360":
                    GET_FX_DXXX(readbuffer, 360);
                    break;
                case "Get FX D380":
                    GET_FX_DXXX(readbuffer, 380);
                    break;

                    #endregion

                    //case "Get FX ALARM":
                    //    GET_FX_ALARM(readbuffer);
                    //    break;
            }
        }

        void PLC0FX1NReadActionBAK(char[] readbuffer, string operationstring)
        {
            switch (operationstring)
            {

                //case "Get FX X":
                //    GET_FX_X(readbuffer);
                //    break;
                //case "Get FX Y":
                //    GET_FX_Y(readbuffer);
                //    break;
                case "Get FX M0":
                    GET_FX_M0(readbuffer);
                    break;
                case "Get FX M120":
                    GET_FX_M120(readbuffer);
                    break;
                case "Get FX D0":
                    GET_FX_D0(readbuffer);
                    break;
                case "Get FX D200":
                    GET_FX_D200(readbuffer);
                    break;
                case "Get FX D220":
                    GET_FX_D220(readbuffer);
                    break;
                case "Get FX D240":
                    GET_FX_D240(readbuffer);
                    break;
                case "Get FX D260":
                    GET_FX_D260(readbuffer);
                    break;
                case "Get FX D":
                    GET_FX_D(readbuffer);
                    break;
                case "Get FX D180":
                    GET_FX_D180(readbuffer);
                    break;
                case "Get FX D180D190":
                    GET_FX_D180D190(readbuffer);
                    break;
                case "Get FX D6":
                    GET_FX_D6(readbuffer);
                    break;
                    //case "Get FX ALARM":
                    //    GET_FX_ALARM(readbuffer);
                    //    break;
            }
        }

        void GET_FX_MXXX(char[] readbuffer,int iMAddress)
        {
            String Str = new string(readbuffer, 1, 2); //M0
            UInt32 GetInt = HEX32(Str);
            int i = 0;
            //String Str1 = "00";

            int j = 0;
            while (j < 15)
            {
                Str = new string(readbuffer, 1 + j * 2, 2); //M0
                //Str1 = Str[1].ToString() + Str[0].ToString();
                GetInt = HEX32(Str);


                i = 0;
                while (i < 8)
                {
                    bool ison = ((GetInt >> i) % 2) == 1;
                    PLCCollection[0].IOData.SetMBit(iMAddress + j * 8 + i, ison);
                    //PLCCollection[0].IOData.SetMBit(8 + j * 8 + i, ison);
                    i++;
                }

                j++;
            }
        }

        void GET_FX_M0(char[] readbuffer)
        {
            String Str = new string(readbuffer, 1, 2); //M0
            UInt32 GetInt = HEX32(Str);
            int i = 0;
            //String Str1 = "00";

            int j = 0;
            while (j < 15)
            {
                Str = new string(readbuffer, 1 + j * 2, 2); //M0
                //Str1 = Str[1].ToString() + Str[0].ToString();
                GetInt = HEX32(Str);


                i = 0;
                while (i < 8)
                {
                    bool ison = ((GetInt >> i) % 2) == 1;
                    PLCCollection[0].IOData.SetMBit(0 + j * 8 + i, ison);
                    //PLCCollection[0].IOData.SetMBit(8 + j * 8 + i, ison);
                    i++;
                }

                j++;
            }
        }
        void GET_FX_M120(char[] readbuffer)
        {
            String Str = new string(readbuffer, 1, 2); //M0
            UInt32 GetInt = HEX32(Str);
            int i = 0;
            //String Str1 = "00";

            int j = 0;
            while (j < 15)
            {
                Str = new string(readbuffer, 1 + j * 2, 2); //M0
                //Str1 = Str[1].ToString() + Str[0].ToString();
                GetInt = HEX32(Str);


                i = 0;
                while (i < 8)
                {
                    bool ison = ((GetInt >> i) % 2) == 1;
                    PLCCollection[0].IOData.SetMBit(120 + j * 8 + i, ison);
                    //PLCCollection[0].IOData.SetMBit(8 + j * 8 + i, ison);
                    i++;
                }

                j++;
            }
        }
        void GET_FX_M240(char[] readbuffer)
        {
            String Str = new string(readbuffer, 1, 2); //M0
            UInt32 GetInt = HEX32(Str);
            int i = 0;
            //String Str1 = "00";

            int j = 0;
            while (j < 15)
            {
                Str = new string(readbuffer, 1 + j * 2, 2); //M0
                //Str1 = Str[1].ToString() + Str[0].ToString();
                GetInt = HEX32(Str);


                i = 0;
                while (i < 8)
                {
                    bool ison = ((GetInt >> i) % 2) == 1;
                    PLCCollection[0].IOData.SetMBit(240 + j * 8 + i, ison);
                    //PLCCollection[0].IOData.SetMBit(8 + j * 8 + i, ison);
                    i++;
                }

                j++;
            }
        }

        void GET_FX_D0(char[] readbuffer)
        {
            String Str = new string(readbuffer, 1, 4); //D0
            UInt32 GetInt = HEX32(Str);
            int i = 0;
            String Str1 = "0000";

            int j = 0;
            while (j < 1)
            {
                Str = new string(readbuffer, 1 + j * 4, 4);
                Str1 = Str.Substring(2) + Str.Substring(0, 2);
                GetInt = HEX32(Str1);

                PLCCollection[0].IOData.SetDData(0 + j, (int)GetInt);//D0开始

                j++;
            }
        }
        void GET_FX_D(char[] readbuffer)
        {
            String Str = new string(readbuffer, 1, 4); //D100
            UInt32 GetInt = HEX32(Str);
            int i = 0;
            String Str1 = "0000";

            int j = 0;
            while (j < 80)
            {
                Str = new string(readbuffer, 1 + j * 4, 4);
                Str1 = Str.Substring(2) + Str.Substring(0, 2);
                GetInt = HEX32(Str1);

                PLCCollection[0].IOData.SetDData(100 + j, (int)GetInt);//D100开始

                j++;
            }
        }

        void GET_FX_D180(char[] readbuffer)
        {
            String Str = new string(readbuffer, 1, 4); //D180
            UInt32 GetInt = HEX32(Str);
            int i = 0;
            String Str1 = "0000";

            int j = 0;
            while (j < 80)
            {
                Str = new string(readbuffer, 1 + j * 4, 4);
                Str1 = Str.Substring(2) + Str.Substring(0, 2);
                GetInt = HEX32(Str1);

                PLCCollection[0].IOData.SetDData(180 + j, (int)GetInt);//D180开始

                j++;
            }
        }
        void GET_FX_D6(char[] readbuffer)
        {
            String Str = new string(readbuffer, 1, 4); //D6
            UInt32 GetInt = HEX32(Str);
            int i = 0;
            String Str1 = "0000";

            int j = 0;
            while (j < 1)
            {
                Str = new string(readbuffer, 1 + j * 4, 4);
                Str1 = Str.Substring(2) + Str.Substring(0, 2);
                GetInt = HEX32(Str1);

                PLCCollection[0].IOData.SetDData(6 + j, (int)GetInt);//D6开始

                j++;
            }
        }
        void GET_FX_D180D190(char[] readbuffer)
        {
            String Str = new string(readbuffer, 1, 4); //D180
            UInt32 GetInt = HEX32(Str);
            int i = 0;
            String Str1 = "0000";

            int j = 0;
            while (j < 6)
            {
                Str = new string(readbuffer, 1 + j * 4, 4);
                Str1 = Str.Substring(2) + Str.Substring(0, 2);
                GetInt = HEX32(Str1);

                PLCCollection[0].IOData.SetDData(180 + j, (int)GetInt);//D180开始

                j++;
            }
        }

        void GET_FX_D200(char[] readbuffer)
        {
            String Str = new string(readbuffer, 1, 4);
            UInt32 GetInt = HEX32(Str);
            int i = 0;
            String Str1 = "0000";

            int j = 0;
            while (j < 6)
            {
                Str = new string(readbuffer, 1 + j * 4, 4);
                Str1 = Str.Substring(2) + Str.Substring(0, 2);
                GetInt = HEX32(Str1);

                PLCCollection[0].IOData.SetDData(200 + j, (int)GetInt);

                j++;
            }
        }
        void GET_FX_D220(char[] readbuffer)
        {
            String Str = new string(readbuffer, 1, 4);
            UInt32 GetInt = HEX32(Str);
            int i = 0;
            String Str1 = "0000";

            int j = 0;
            while (j < 6)
            {
                Str = new string(readbuffer, 1 + j * 4, 4);
                Str1 = Str.Substring(2) + Str.Substring(0, 2);
                GetInt = HEX32(Str1);

                PLCCollection[0].IOData.SetDData(220 + j, (int)GetInt);

                j++;
            }
        }
        void GET_FX_D240(char[] readbuffer)
        {
            String Str = new string(readbuffer, 1, 4);
            UInt32 GetInt = HEX32(Str);
            int i = 0;
            String Str1 = "0000";

            int j = 0;
            while (j < 6)
            {
                Str = new string(readbuffer, 1 + j * 4, 4);
                Str1 = Str.Substring(2) + Str.Substring(0, 2);
                GetInt = HEX32(Str1);

                PLCCollection[0].IOData.SetDData(240 + j, (int)GetInt);

                j++;
            }
        }
        void GET_FX_D260(char[] readbuffer)
        {
            String Str = new string(readbuffer, 1, 4);
            UInt32 GetInt = HEX32(Str);
            int i = 0;
            String Str1 = "0000";

            int j = 0;
            while (j < 6)
            {
                Str = new string(readbuffer, 1 + j * 4, 4);
                Str1 = Str.Substring(2) + Str.Substring(0, 2);
                GetInt = HEX32(Str1);

                PLCCollection[0].IOData.SetDData(260 + j, (int)GetInt);

                j++;
            }
        }

        /// <summary>
        /// 读取轴的当前位置
        /// </summary>
        /// <param name="readbuffer">数据缓存</param>
        /// <param name="iDAddress">轴的地址</param>
        void GET_FX_DXXX(char[] readbuffer,int iDAddress)
        {
            String Str = new string(readbuffer, 1, 4);
            UInt32 GetInt = HEX32(Str);
            int i = 0;
            String Str1 = "0000";

            int j = 0;
            while (j < 20)
            {
                Str = new string(readbuffer, 1 + j * 4, 4);
                Str1 = Str.Substring(2) + Str.Substring(0, 2);
                GetInt = HEX32(Str1);

                PLCCollection[0].IOData.SetDData(iDAddress + j, (int)GetInt);

                j++;
            }
        }
        /// <summary>
        /// 读取轴的当前位置
        /// </summary>
        /// <param name="readbuffer">数据缓存</param>
        /// <param name="iDAddress">轴的地址</param>
        /// <param name="iLength">读取长度</param>
        void GET_FX_DXXX(char[] readbuffer, int iDAddress,int iLength)
        {
            String Str = new string(readbuffer, 1, 4);
            UInt32 GetInt = HEX32(Str);
            int i = 0;
            String Str1 = "0000";

            int j = 0;
            while (j < iLength)
            {
                Str = new string(readbuffer, 1 + j * 4, 4);
                Str1 = Str.Substring(2) + Str.Substring(0, 2);
                GetInt = HEX32(Str1);

                PLCCollection[0].IOData.SetDData(iDAddress + j, (int)GetInt);

                j++;
            }
        }

        #endregion

        void PLC0ReadAction(char[] readbuffer, string operationstring)
        {
            switch(operationstring)
            {
                case "Get All M":
                    PLC0GetAllMEX(readbuffer);
                    break;
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
        void PLC0GetAllMEX(char[] readbuffer)
        {
            String Str = new string(readbuffer, 6, 8); //M0048

            UInt32 GetInt = HEX32(Str);
            int i = 0;

            while (i < 32)
            {
                bool ison = ((GetInt >> i) % 2) == 1;

                PLCCollection[0].IOData.SetMBit(48 + i, ison);

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
        public override void Tick()
        {
            if (myJzTimes.msDuriation < MSDuriation)
                return;

            CheckEvent();

            myJzTimes.Cut();
        }

        #region Alarms Define


        bool AlarmSeriousTrigered = false;
        bool AlarmSeriousnow = false;
        public bool IsAlarmSerious
        {
            get
            {
                return AlarmSeriousnow;
            }
            set
            {
                if (AlarmSeriousnow != value)
                {
                    if (value)
                        AlarmSeriousTrigered = true;
                    else
                        AlarmSeriousTrigered = false;

                    AlarmSeriousnow = value;
                }
                else
                    AlarmSeriousTrigered = false;
            }
        }

        bool AlarmCommonTrigered = false;
        bool AlarmCommonnow = false;
        public bool IsAlarmCommon
        {
            get
            {
                return AlarmCommonnow;
            }
            set
            {
                if (AlarmCommonnow != value)
                {
                    if (value)
                        AlarmCommonTrigered = true;
                    else
                        AlarmCommonTrigered = false;

                    AlarmCommonnow = value;
                }
                else
                    AlarmCommonTrigered = false;
            }
        }

        bool EMCTrigered = false;
        bool IsEMCnow = false;
        public bool IsEMC
        {
            get
            {
                return IsEMCnow;
            }
            set
            {
                if (IsEMCnow != value)
                {
                    if (value)
                        EMCTrigered = true;
                    else
                        EMCTrigered = false;

                    IsEMCnow = value;
                }
                else
                    EMCTrigered = false;
            }
        }


        public bool ClearAlarm
        {
            set
            {
                if (value)
                {
                    AlarmSeriousnow = false;
                    AlarmCommonnow = false;
                }
                PLCIO.CLEARALARMS = value;
            }
        }

        #endregion

        bool IsOldStart = false;
        //bool IsOldEMC = false;
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
                HIVECLIENT.Hiveclient_MachineState((int)m_machinestate_tmp, !IsInitHiveclient);
            }
        }
        /// <summary>
        /// 設定5色燈狀態，任何情況只有一個燈亮
        /// </summary>
        /// <param name="m_machinestate">狀態</param>
        public void SetMachineState(MachineState m_machinestate)
        {
            switch (OPTION)
            {
                case OptionEnum.MAIN_SD:
                   
                    break;
                default:
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

                        HIVECLIENT.Hiveclient_MachineState((int)m_machinestate, !IsInitHiveclient);

                        if (!IsInitHiveclient)
                            IsInitHiveclient = true;
                        //将切换的状态缓存下来，以便达到状态变化时再改变灯的颜色。
                        m_machinestate_tmp = m_machinestate;
                    }
                    break;
            }

            
        }

        public void SetLight(string lightstr)
        {
            switch (OPTION)
            {
                case OptionEnum.MAIN_SD:

                    if (lightstr == "")
                    {
                        PLCIO.ADR_LIGHT = false;
                    }
                    else
                    {
                        string[] strs = lightstr.Split(',');

                        if (strs.Length < 5)
                        {
                            lightstr = "1,1,1,1,1";
                            strs = lightstr.Split(',');
                        }

                        PLCIO.ADR_LIGHT = strs[0] == "1";

                        //PLCIO.TopLight = strs[0] == "1";
                        //PLCIO.ArroundLight = strs[1] == "1";
                        //PLCIO.GodLight = strs[2] == "1";

                        //PLCIO.PannelLight = strs[3] == "1";
                        //PLCIO.CircleLight = strs[4] == "1";
                    }

                    //PLCIO.ADR_LIGHT = (!string.IsNullOrEmpty(lightstr) ? true : false);

                    break;
                default:
                    if (lightstr == "")
                    {
                        //PLCIO.TopLight = false;
                        //PLCIO.ArroundLight = false;

                        //PLCIO.GodLight = false;
                        //PLCIO.PannelLight = false;
                        //PLCIO.CircleLight = false;
                    }
                    else
                    {
                        string[] strs = lightstr.Split(',');

                        if (strs.Length < 5)
                        {
                            lightstr = "1,1,1,1,1";
                            strs = lightstr.Split(',');
                        }

                        //PLCIO.TopLight = strs[0] == "1";
                        //PLCIO.ArroundLight = strs[1] == "1";
                        //PLCIO.GodLight = strs[2] == "1";

                        //PLCIO.PannelLight = strs[3] == "1";
                        //PLCIO.CircleLight = strs[4] == "1";
                    }
                    break;
            }

           
        }
        public override void GoHome()
        {
        }
        public override void CheckEvent()
        {
            switch (OPTION)
            {
                default:

                    IsAlarmSerious = PLCIO.IsAlarmsSerious;
                    if (AlarmSeriousTrigered)
                    {
                        OnTrigger(MachineEventEnum.ALARM_SERIOUS);
                    }

                    IsAlarmCommon = PLCIO.IsAlarmsCommon;
                    if (AlarmCommonTrigered)
                    {
                        OnTrigger(MachineEventEnum.ALARM_COMMON);
                    }

                    IsEMC = PLCIO.ADR_ISEMC;
                    if (EMCTrigered)
                    {
                        OnTrigger(MachineEventEnum.EMC);
                    }

                    ////Trigger EMC
                    //bool isemcnow = PLCIO.ADR_ISEMC;

                    //if (isemcnow && IsOldEMC != isemcnow)
                    //{
                    //    OnTrigger(MachineEventEnum.EMC);
                    //}
                    //IsOldEMC = isemcnow;

                    //Check If PLC Disconnect
                    foreach (FatekPLCClass plc in PLCCollection)
                    {
                        plc.Tick();
                    }
                    break;
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
            switch (OPTION)
            {
                //case OptionEnum.MAIN_SD:

                //    break;
                default:
                    foreach (FatekPLCClass plc in PLCCollection)
                    {
                        plc.RetryConn();
                    }
                    break;
            }
        }
        /// <summary>
        /// 读取缓存的指令
        /// </summary>
        /// <param name="eCmdOnOff">指令的开关 true:开 false:关</param>
        public void PLCReadCmdNormalTemp(bool eCmdOnOff)
        {
            switch (OPTION)
            {
                //case OptionEnum.MAIN_SD:

                //    break;
                default:
                    foreach (FatekPLCClass plc in PLCCollection)
                    {
                        plc.isNormalTempNO = eCmdOnOff;

                        //plc.RetryConn();
                    }
                    break;
            }
        }


        private bool m_IsGetImageComplete = false;
        /// <summary>
        /// 取像完成信号 中继点
        /// </summary>
        public bool IsGetImageComplete
        {
            get { return m_IsGetImageComplete; }
            set { m_IsGetImageComplete = value; }
        }

        
        private int m_ContinueNGIndex = 0;
        /// <summary>
        /// 连续NG计数标志
        /// </summary>
        public int ContinueNGIndex
        {
            get { return m_ContinueNGIndex; }
            set { m_ContinueNGIndex = value; }
        }


        private bool m_UserFroceCount = false;
        /// <summary>
        /// 用户强制不清空数目 等待硬件满盒
        /// </summary>
        public bool IsUserFroceCount
        {
            get { return m_UserFroceCount; }
            set { m_UserFroceCount = value; }
        }

    }
}
