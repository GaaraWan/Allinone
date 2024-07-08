using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JetEazy.ControlSpace;
using JetEazy;


namespace Allinone.ControlSpace.IOSpace
{
    public enum R5AddressEnum : int
    {
        COUNT = 25,
        /// <summary>
        /// START
        /// </summary>
        ADR_ISSTART = 0,
        /// <summary>
        /// UPS
        /// </summary>
        ADR_ISUPSERROR = 1,

        ADR_RED = 2,
        ADR_YELLOW = 3,
        ADR_GREEN = 4,

        /// <summary>
        /// 頂燈
        /// </summary>
        ADR_TOPLIGHT = 5,
        /// <summary>
        ///四周燈管
        /// </summary>
        ADR_ARROUNDLIGHT = 6,
        /// <summary>
        /// 鐳雕平板燈
        /// </summary>
        ADR_PANNELLIGHT = 7,
        /// <summary>
        /// 神燈
        /// </summary>
        ADR_GODLIGHT = 8,
        /// <summary>
        /// 小圓燈
        /// </summary>
        ADR_CIRCLELIGHT = 9,

        /// <summary>
        /// 螺丝高跷燈
        /// </summary>
        ADR_STILTSLIGHT = 12,

        ADR_BLUE = 10,
        ADR_WHITE = 11,

        /// <summary>
        /// STOP
        /// </summary>
        ADR_ISSTOP = 14,
        /// <summary>
        /// 大电脑
        /// </summary>
        MAXCOMPER = 15,
        RESET = 16,
        STOP = 17,
        /// <summary>
        /// 小电脑
        /// </summary>
        MINCOMPER = 18,
        /// <summary>
        /// 启动程序给PLC信号
        /// </summary>
        STATR5 = 19,
        /// <summary>
        /// 警报信息
        /// </summary>
        ALARM = 20,
        /// <summary>
        /// PLC流程中
        /// </summary>
        PLCProcess,
        /// <summary>
        /// PLC复位中
        /// </summary>
        PLCReset,
        /// <summary>
        /// PLC复位步号
        /// </summary>
        PLCResetNO,
        /// <summary>
        /// PLC跑线步号
        /// </summary>
        PLCRunNO,
    }
    public class JzR5IOClass : GeoIOClass
    {

        public JzR5IOClass()
        {

        }
        public void Initial(string path, OptionEnum option, FatekPLCClass[] plc)
        {
            ADDRESSARRAY = new FATEKAddressClass[(int)R5AddressEnum.COUNT];

            PLC = plc;

            INIFILE = path + "\\IO.INI";

            LoadData();

        }
        public override void LoadData()
        {
            ADDRESSARRAY[(int)R5AddressEnum.ADR_ISSTART] = new FATEKAddressClass(ReadINIValue("Status Address", R5AddressEnum.ADR_ISSTART.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)R5AddressEnum.ADR_ISSTOP] = new FATEKAddressClass(ReadINIValue("Status Address", R5AddressEnum.ADR_ISSTOP.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)R5AddressEnum.ADR_ISUPSERROR] = new FATEKAddressClass(ReadINIValue("Status Address", R5AddressEnum.ADR_ISUPSERROR.ToString(), "", INIFILE));

            ADDRESSARRAY[(int)R5AddressEnum.ADR_TOPLIGHT] = new FATEKAddressClass(ReadINIValue("Operation Address", R5AddressEnum.ADR_TOPLIGHT.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)R5AddressEnum.ADR_ARROUNDLIGHT] = new FATEKAddressClass(ReadINIValue("Operation Address", R5AddressEnum.ADR_ARROUNDLIGHT.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)R5AddressEnum.ADR_PANNELLIGHT] = new FATEKAddressClass(ReadINIValue("Operation Address", R5AddressEnum.ADR_PANNELLIGHT.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)R5AddressEnum.ADR_GODLIGHT] = new FATEKAddressClass(ReadINIValue("Operation Address", R5AddressEnum.ADR_GODLIGHT.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)R5AddressEnum.ADR_CIRCLELIGHT] = new FATEKAddressClass(ReadINIValue("Operation Address", R5AddressEnum.ADR_CIRCLELIGHT.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)R5AddressEnum.ADR_STILTSLIGHT] = new FATEKAddressClass(ReadINIValue("Operation Address", R5AddressEnum.ADR_STILTSLIGHT.ToString(), "", INIFILE));

            ADDRESSARRAY[(int)R5AddressEnum.ADR_RED] = new FATEKAddressClass(ReadINIValue("Operation Address", R5AddressEnum.ADR_RED.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)R5AddressEnum.ADR_YELLOW] = new FATEKAddressClass(ReadINIValue("Operation Address", R5AddressEnum.ADR_YELLOW.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)R5AddressEnum.ADR_GREEN] = new FATEKAddressClass(ReadINIValue("Operation Address", R5AddressEnum.ADR_GREEN.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)R5AddressEnum.ADR_BLUE] = new FATEKAddressClass(ReadINIValue("Operation Address", R5AddressEnum.ADR_BLUE.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)R5AddressEnum.ADR_WHITE] = new FATEKAddressClass(ReadINIValue("Operation Address", R5AddressEnum.ADR_WHITE.ToString(), "", INIFILE));



            ADDRESSARRAY[(int)R5AddressEnum.MAXCOMPER] = new FATEKAddressClass(ReadINIValue("Operation Address", R5AddressEnum.MAXCOMPER.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)R5AddressEnum.MINCOMPER] = new FATEKAddressClass(ReadINIValue("Operation Address", R5AddressEnum.MINCOMPER.ToString(), "", INIFILE));

            ADDRESSARRAY[(int)R5AddressEnum.RESET] = new FATEKAddressClass(ReadINIValue("Operation Address", R5AddressEnum.RESET.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)R5AddressEnum.STOP] = new FATEKAddressClass(ReadINIValue("Operation Address", R5AddressEnum.STOP.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)R5AddressEnum.STATR5] = new FATEKAddressClass(ReadINIValue("Operation Address", R5AddressEnum.STATR5.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)R5AddressEnum.ALARM] = new FATEKAddressClass(ReadINIValue("Operation Address", R5AddressEnum.ALARM.ToString(), "", INIFILE));

            ADDRESSARRAY[(int)R5AddressEnum.PLCProcess] = new FATEKAddressClass(ReadINIValue("Operation Address", R5AddressEnum.PLCProcess.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)R5AddressEnum.PLCReset] = new FATEKAddressClass(ReadINIValue("Operation Address", R5AddressEnum.PLCReset.ToString(), "", INIFILE));

            ADDRESSARRAY[(int)R5AddressEnum.PLCResetNO] = new FATEKAddressClass(ReadINIValue("Operation Address", R5AddressEnum.PLCResetNO.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)R5AddressEnum.PLCRunNO] = new FATEKAddressClass(ReadINIValue("Operation Address", R5AddressEnum.PLCRunNO.ToString(), "", INIFILE));

        }

        public override void SaveData()
        {
            throw new NotImplementedException();
        }

        public bool Red
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.ADR_RED];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.ADR_RED];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }

        public bool Yellow
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.ADR_YELLOW];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.ADR_YELLOW];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        /// <summary>
        /// 大电脑
        /// </summary>
        public bool MAXCOMPER
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.MAXCOMPER];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.MAXCOMPER];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        /// <summary>
        /// 小电脑
        /// </summary>
        public bool MINCOMPER
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.MINCOMPER];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.MINCOMPER];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }

        /// <summary>
        /// 治具夹缸
        /// </summary>
        public bool RESET
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.RESET];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.RESET];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }

        /// <summary>
        /// 启动程序给PLC信号
        /// </summary>
        public bool STATR5
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.STATR5];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.STATR5];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }

        /// <summary>
        /// 治具夹缸
        /// </summary>
        public bool STOP
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.STOP];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.STOP];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool Green
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.ADR_GREEN];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.ADR_GREEN];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool IsStart
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.ADR_ISSTART];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool IsStop
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.ADR_ISSTOP];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool IsUPSError
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.ADR_ISUPSERROR];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool TopLight
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.ADR_TOPLIGHT];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.ADR_TOPLIGHT];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool ArroundLight
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.ADR_ARROUNDLIGHT];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.ADR_ARROUNDLIGHT];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }

        public bool PannelLight
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.ADR_PANNELLIGHT];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.ADR_PANNELLIGHT];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool GodLight
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.ADR_GODLIGHT];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.ADR_GODLIGHT];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool CircleLight
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.ADR_CIRCLELIGHT];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.ADR_CIRCLELIGHT];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        /// <summary>
        /// 螺丝高翘灯
        /// </summary>
        public bool StiltsLight
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.ADR_STILTSLIGHT];
                if (!string.IsNullOrEmpty(address.Address0))
                    return PLC[address.SiteNo].IOData.GetBit(address.Address0);
                return false;
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.ADR_STILTSLIGHT];
                if (!string.IsNullOrEmpty(address.Address0))
                    PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool Blue
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.ADR_BLUE];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.ADR_BLUE];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool White
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.ADR_WHITE];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.ADR_WHITE];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        /// <summary>
        /// 警报信息
        /// </summary>
        public int ALARM
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.ALARM];
                return PLC[address.SiteNo].IOData.GetData(address.Address0);
            }
        }
        /// <summary>
        /// PLC 流程中
        /// </summary>
        public bool PLCProcess
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.PLCProcess];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }

        /// <summary>
        /// PLC 复位中
        /// </summary>
        public bool PLCReset
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.PLCReset];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }


        /// <summary>
        /// PLC 复位步号
        /// </summary>
        public int PLCResetNO
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.PLCResetNO];
               // return HEXSigned32( PLC[address.SiteNo].IOData.GetData(address.Address0));
               //int ret = HEXSigned32(ValueToHEX(PLC[address.SiteNo].IOData.GetData(address.Address1), 4)

             int  ret = HEXSigned32(ValueToHEX(PLC[address.SiteNo].IOData.GetData(address.Address0), 4));
                return ret;
            }
        }
        /// <summary>
        /// PLC 跑线步号
        /// </summary>
        public int PLCRunNO
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R5AddressEnum.PLCRunNO];
                int ret = HEXSigned32(ValueToHEX(PLC[address.SiteNo].IOData.GetData(address.Address0), 4));
                return ret;
            }
        }

        protected string ValueToHEX(long Value, int Length)
        {
            return ("00000000" + Value.ToString("X")).Substring(("00000000" + Value.ToString("X")).Length - Length, Length);
        }
        protected Int32 HEXSigned32(string HexStr)
        {
            return System.Convert.ToInt32(HexStr, 16);
        }
    }
}
