using JetEazy;
using JetEazy.ControlSpace;
using JetEazy.ControlSpace.MotionSpace;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allinone.ControlSpace.IOSpace
{
    public class JzMainSDM5IOClass : GeoIOClass
    {
        public enum ADRMainSDM5 : int
        {
            COUNT = 8,

            ADR_ISEMC = 0,
            ADR_ISRESET = 1,
            ADR_ISSTART = 2,
            ADR_ISSTOP = 3,

            ADR_YELLOW = 4,
            ADR_GREEN = 5,
            ADR_RED = 6,
            ADR_BUZZER = 7,
        }

        public PLCAlarmsClass[] PLCALARMS;

        public JzMainSDM5IOClass()
        {

        }
        public void Initial(string path, FatekPLCClass[] plc)
        {

            ADDRESSARRAY = new FATEKAddressClass[(int)ADRMainSDM5.COUNT];
            PLCALARMS = new PLCAlarmsClass[(int)AlarmsEnum.ALARMSCOUNT];

            PLC = plc;

            INIFILE = path + "\\IO.INI";

            LoadData();

        }
        public override void LoadData()
        {

            ADDRESSARRAY[(int)ADRMainSDM5.ADR_ISEMC] = new FATEKAddressClass(ReadINIValue("Status Address", "ADR_ISEMC", "", INIFILE));
            ADDRESSARRAY[(int)ADRMainSDM5.ADR_ISRESET] = new FATEKAddressClass(ReadINIValue("Status Address", "ADR_ISRESET", "", INIFILE));
            ADDRESSARRAY[(int)ADRMainSDM5.ADR_ISSTART] = new FATEKAddressClass(ReadINIValue("Status Address", "ADR_ISSTART", "", INIFILE));
            ADDRESSARRAY[(int)ADRMainSDM5.ADR_ISSTOP] = new FATEKAddressClass(ReadINIValue("Status Address", "ADR_ISSTOP", "", INIFILE));


            ADDRESSARRAY[(int)ADRMainSDM5.ADR_RED] = new FATEKAddressClass(ReadINIValue("Operation Address", "ADR_RED", "", INIFILE));
            ADDRESSARRAY[(int)ADRMainSDM5.ADR_GREEN] = new FATEKAddressClass(ReadINIValue("Operation Address", "ADR_GREEN", "", INIFILE));
            ADDRESSARRAY[(int)ADRMainSDM5.ADR_YELLOW] = new FATEKAddressClass(ReadINIValue("Operation Address", "ADR_YELLOW", "", INIFILE));
            ADDRESSARRAY[(int)ADRMainSDM5.ADR_BUZZER] = new FATEKAddressClass(ReadINIValue("Operation Address", "ADR_BUZZER", "", INIFILE));


            #region 读取csv- ALARM

            string alarm0_path = INIFILE.Replace("IO.INI", "ALARMIO0.csv");
            System.IO.StreamReader _sr = null;
            try
            {
                _sr = new System.IO.StreamReader(alarm0_path, Encoding.Default);
                PLCALARMS[(int)AlarmsEnum.ALARMS_ADR_COMMON] = new PLCAlarmsClass("D00004:M0256");
                //PLCALARMS[(int)AlarmsEnum.ALARMS_ADR_COMMON] = new PLCAlarmsClass("MW0000:MX0.0,MW0001:MX2.0,MW0002:MX4.0,MW0003:MX6.0,MW0004:MX8.0,MW0005:MX10.0,MW0006:MX12.0,MW0007:MX14.0");

                PLCALARMS[(int)AlarmsEnum.ALARMS_ADR_SERIOUS] = new PLCAlarmsClass("D00000:M0224,D00002:M0240");
                PLCALARMS[(int)AlarmsEnum.ALARMS_ADR_WARNING] = new PLCAlarmsClass("D00004:M0256");
                string strRead = string.Empty;
                while (!_sr.EndOfStream)
                {
                    strRead = _sr.ReadLine();
                    string[] strs = strRead.Split(',').ToArray();
                    if (strs.Length >= 4)
                    {
                        switch (strs[0])
                        {
                            case "COMMON":
                                PLCALARMS[(int)AlarmsEnum.ALARMS_ADR_COMMON].PLCAlarmsAddDescription("0," + strs[2] + "," + strs[3]);
                                break;
                            case "SERIOUS":
                                PLCALARMS[(int)AlarmsEnum.ALARMS_ADR_SERIOUS].PLCAlarmsAddDescription("0," + strs[2] + "," + strs[3]);
                                break;
                            case "WARNING":
                                PLCALARMS[(int)AlarmsEnum.ALARMS_ADR_WARNING].PLCAlarmsAddDescription("0," + strs[2] + "," + strs[3]);
                                break;
                        }
                    }
                }

                _sr.Close();
                _sr.Dispose();
                _sr = null;
            }
            catch
            {

            }

            if (_sr != null)
                _sr.Dispose();

            #endregion

        }

        public override void SaveData()
        {

        }

        public bool IsAlarmsSerious
        {
            get
            {
                //FATEKAddressClass address = new FATEKAddressClass("0:M1060");
                //return PLC[address.SiteNo].IOData.GetBit(address.Address0);

                return GetBit(220) || GetBit(221);
            }
        }

        public bool IsAlarmsCommon
        {
            get
            {
                //FATEKAddressClass address = new FATEKAddressClass("0:M1065");
                //return PLC[address.SiteNo].IOData.GetBit(address.Address0);
                return GetBit(222);
            }
        }
        //public bool IsAlarmsWarning
        //{
        //    get
        //    {
        //        FATEKAddressClass address = new FATEKAddressClass("0:D00420");
        //        return PLC[address.SiteNo].IOData.GetData(address.Address0);
        //    }
        //}
        //public int IntAlarmsSerious
        //{
        //    get
        //    {
        //        FATEKAddressClass address = new FATEKAddressClass("0:D00500");
        //        return PLC[address.SiteNo].IOData.GetData(address.Address0);
        //    }
        //}

        //public int IntAlarmsCommon
        //{
        //    get
        //    {
        //        FATEKAddressClass address = new FATEKAddressClass("0:D00502");
        //        return PLC[address.SiteNo].IOData.GetData(address.Address0);
        //    }
        //}
        //public int IntAlarmsWarning
        //{
        //    get
        //    {
        //        FATEKAddressClass address = new FATEKAddressClass("0:D00420");
        //        return PLC[address.SiteNo].IOData.GetData(address.Address0);
        //    }
        //}
        public bool CLEARALARMS
        {
            get
            {
                return GetBit(223);
            }
            set
            {
                SetBit(223, value);
            }
        }
        public bool GetAlarmsAddress(int iSiteNo, string strAddress)
        {
            if (string.IsNullOrEmpty(strAddress))
                return false;
            return PLC[iSiteNo].IOData.GetBit(strAddress);
        }
        public bool ADR_ISPAUSE
        {
            get
            {
                return GetBit(3);
            }
        }

        public bool ADR_ISEMC
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)ADRMainSDM5.ADR_ISEMC];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool ADR_ISRESET
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)ADRMainSDM5.ADR_ISRESET];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool ADR_ISSTART
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)ADRMainSDM5.ADR_ISSTART];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool ADR_ISSTOP
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)ADRMainSDM5.ADR_ISSTOP];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }


        public bool ADR_RED
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)ADRMainSDM5.ADR_RED];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)ADRMainSDM5.ADR_RED];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool ADR_GREEN
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)ADRMainSDM5.ADR_GREEN];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)ADRMainSDM5.ADR_GREEN];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool ADR_YELLOW
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)ADRMainSDM5.ADR_YELLOW];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)ADRMainSDM5.ADR_YELLOW];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool ADR_BUZZER
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)ADRMainSDM5.ADR_BUZZER];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)ADRMainSDM5.ADR_BUZZER];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }

        public bool ADR_RESET
        {
            get
            {
                //FATEKAddressClass address = new FATEKAddressClass("0:M1001");
                //return PLC[address.SiteNo].IOData.GetBit(address.Address0);
                return GetBit(183);
            }
            set
            {
                //FATEKAddressClass address = new FATEKAddressClass("0:M1001");
                //PLC[address.SiteNo].SetIO(value, address.Address0);
                SetBit(183, value);
            }
        }
        public bool ADR_RESETING
        {
            get
            {
                //FATEKAddressClass address = new FATEKAddressClass("0:M1062");
                //return PLC[address.SiteNo].IOData.GetBit(address.Address0);
                return GetBit(184);
            }
            //set
            //{
            //    FATEKAddressClass address = new FATEKAddressClass("0:M1062");
            //    PLC[address.SiteNo].SetIO(value, address.Address0);
            //}
        }
        public bool ADR_RESETCOMPLETE
        {
            get
            {
                //FATEKAddressClass address = new FATEKAddressClass("0:M1063");
                //return PLC[address.SiteNo].IOData.GetBit(address.Address0);
                return GetBit(185);
            }
            //set
            //{
            //    FATEKAddressClass address = new FATEKAddressClass("0:M1063");
            //    PLC[address.SiteNo].SetIO(value, address.Address0);
            //}
        }

        public bool ADR_PROCESS
        {
            get
            {
                //FATEKAddressClass address = new FATEKAddressClass("0:M1001");
                //return PLC[address.SiteNo].IOData.GetBit(address.Address0);
                return GetBit(180);
            }
            set
            {
                //FATEKAddressClass address = new FATEKAddressClass("0:M1001");
                //PLC[address.SiteNo].SetIO(value, address.Address0);
                SetBit(180, value);
            }
        }
        public bool ADR_PROCESSING
        {
            get
            {
                //FATEKAddressClass address = new FATEKAddressClass("0:M1062");
                //return PLC[address.SiteNo].IOData.GetBit(address.Address0);
                return GetBit(181);
            }
            //set
            //{
            //    FATEKAddressClass address = new FATEKAddressClass("0:M1062");
            //    PLC[address.SiteNo].SetIO(value, address.Address0);
            //}
        }
        public bool ADR_PROCESSCOMPLETE
        {
            get
            {
                //FATEKAddressClass address = new FATEKAddressClass("0:M1063");
                //return PLC[address.SiteNo].IOData.GetBit(address.Address0);
                return GetBit(182);
            }
            //set
            //{
            //    FATEKAddressClass address = new FATEKAddressClass("0:M1063");
            //    PLC[address.SiteNo].SetIO(value, address.Address0);
            //}
        }

        public bool ADR_LineScanStart
        {
            get
            {
                FATEKAddressClass address = new FATEKAddressClass("0:M0186");
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = new FATEKAddressClass("0:M0186");
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool ADR_LineScaning
        {
            get
            {
                FATEKAddressClass address = new FATEKAddressClass("0:M0187");
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool ADR_LineScanComplete
        {
            get
            {
                FATEKAddressClass address = new FATEKAddressClass("0:M0188");
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }

        public bool ADR_TrayLineScanStart
        {
            get
            {
                FATEKAddressClass address = new FATEKAddressClass("0:M0180");
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = new FATEKAddressClass("0:M0180");
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool ADR_TrayLineScaning
        {
            get
            {
                FATEKAddressClass address = new FATEKAddressClass("0:M0181");
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool ADR_TrayLineScanComplete
        {
            get
            {
                FATEKAddressClass address = new FATEKAddressClass("0:M0182");
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }

        public bool ADR_Door
        {
            get
            {
                //FATEKAddressClass address = new FATEKAddressClass("0:M1062");
                //return PLC[address.SiteNo].IOData.GetBit(address.Address0);
                return GetBit(9);
            }
            //set
            //{
            //    FATEKAddressClass address = new FATEKAddressClass("0:M1062");
            //    PLC[address.SiteNo].SetIO(value, address.Address0);
            //}
        }
        public bool ADR_LinePCToPlcSign
        {
            get
            {
                FATEKAddressClass address = new FATEKAddressClass("0:M0015");
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = new FATEKAddressClass("0:M0015");
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        ///// <summary>
        ///// 线扫不出盘结束的时候关掉这个点位
        ///// </summary>
        //public bool ADR_LinePCToPlcSign2
        //{
        //    get
        //    {
        //        FATEKAddressClass address = new FATEKAddressClass("0:M0015");
        //        return PLC[address.SiteNo].IOData.GetBit(address.Address0);
        //    }
        //    set
        //    {
        //        FATEKAddressClass address = new FATEKAddressClass("0:M0014");
        //        PLC[address.SiteNo].SetIO(value, address.Address0);
        //    }
        //}

        /// <summary>
        /// 单次抓图
        /// </summary>
        public bool ADR_OnceGetImage
        {
            get
            {
                FATEKAddressClass address = new FATEKAddressClass("0:M0186");
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = new FATEKAddressClass("0:M0186");
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }

        public int GetAlmValue
        {
            get
            {
                return GetDValue(0) + GetDValue(2);
            }
        }
        public bool IsWarningGetbarcode
        {
            get
            {
                return GetBit(256);
            }
        }
        public bool IsWarningBindbarcode
        {
            get
            {
                return GetBit(257);
            }
        }

        /// <summary>
        /// 强制终止plc中的流程
        /// </summary>
        public bool ForceStopPlcProcess
        {
            get
            {
                return GetBit(55);
            }
            set
            {
                SetBit(55, value);
            }
        }
        /// <summary>
        /// 屏蔽門禁
        /// </summary>
        public bool BypassDoor
        {
            get
            {
                return GetBit(47);
            }
            set
            {
                SetBit(47, value);
            }
        }
        /// <summary>
        /// plc試運行
        /// </summary>
        public bool PlcTestRun
        {
            get
            {
                return GetBit(51);
            }
            set
            {
                SetBit(51, value);
            }
        }
        public bool ADR_ISAUTO_AND_MANUAL
        {
            get
            {
                return GetBit(52);
            }
            set
            {
                SetBit(52, value);
            }
        }

        public bool Ready
        {
            get
            {
                return GetBit(56);
            }
            set
            {
                SetBit(56, value);
            }
        }
        public bool Pass
        {
            get
            {
                return GetBit(53);
            }
            set
            {
                SetBit(53, value);
            }
        }
        public bool Fail
        {
            get
            {
                return GetBit(54);
            }
            set
            {
                SetBit(54, value);
            }
        }

        public bool GetBit(int index)
        {
            if (index < 0)
                return false;
            string addr = "0:M" + index.ToString("0000");
            return GetBit(addr);
        }
        public bool GetBit(string addStr)
        {
            FATEKAddressClass address = new FATEKAddressClass(addStr);
            return PLC[address.SiteNo].IOData.GetBit(address.Address0);
        }
        public void SetBit(int index, bool ison)
        {
            string addr = "0:M" + index.ToString("0000");
            FATEKAddressClass address = new FATEKAddressClass(addr);
            PLC[address.SiteNo].SetIO(ison, address.Address0);
        }
        public void SetBit(string addStr, bool ison)
        {
            FATEKAddressClass address = new FATEKAddressClass(addStr);
            PLC[address.SiteNo].SetIO(ison, address.Address0);
        }
        //public short GetMW(string addStr)
        //{
        //    FATEKAddressClass address = new FATEKAddressClass(addStr);
        //    return PLC[address.SiteNo].IOData.GetMW(address.Address0);
        //}

        public int GetData(string addStr)
        {
            FATEKAddressClass address = new FATEKAddressClass(addStr);
            return HEXSigned32(ValueToHEX(PLC[address.SiteNo].IOData.GetData(address.Address0), 4));
        }
        public int GetDValue(int index)
        {
            string addr = "0:D" + index.ToString("00000");
            return GetData(addr);
        }
        public int GetRValue(int index)
        {
            string addr = "0:R" + index.ToString("00000");
            return GetData(addr);
        }
        public void SetValue(int ivalue, string ioaddr0, string ioaddr1 = "")
        {
            long setH = ivalue >> 16;
            long setL = ivalue % 65536;

            PLC[0].SetData(ValueToHEX(setL, 4), ioaddr0);
            if (!string.IsNullOrEmpty(ioaddr1))
                PLC[0].SetData(ValueToHEX(setH, 4), ioaddr1);
        }
    }
}
