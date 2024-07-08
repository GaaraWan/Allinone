using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JetEazy.ControlSpace;
using JetEazy;


namespace Allinone.ControlSpace.IOSpace
{
    public enum R32AddressEnum : int
    {
        COUNT = 14,
        /// <summary>
        /// START
        /// </summary>
        ADR_ISSTART = 0,
        /// <summary>
        /// STOP
        /// </summary>
        ADR_ISSTOP = 13,
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
    }
    public class JzR32IOClass : GeoIOClass
    {

        public JzR32IOClass()
        {   

        }
        public void Initial(string path,OptionEnum option,FatekPLCClass [] plc)
        {
            ADDRESSARRAY = new FATEKAddressClass[(int)R32AddressEnum.COUNT];

            PLC = plc;

            INIFILE = path + "\\IO.INI";

            LoadData();

        }
        public override void LoadData()
        {
            ADDRESSARRAY[(int)R32AddressEnum.ADR_ISSTART] = new FATEKAddressClass(ReadINIValue("Status Address", R32AddressEnum.ADR_ISSTART.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)R32AddressEnum.ADR_ISSTOP] = new FATEKAddressClass(ReadINIValue("Status Address", R32AddressEnum.ADR_ISSTOP.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)R32AddressEnum.ADR_ISUPSERROR] = new FATEKAddressClass(ReadINIValue("Status Address", R32AddressEnum.ADR_ISUPSERROR.ToString(), "", INIFILE));

            ADDRESSARRAY[(int)R32AddressEnum.ADR_TOPLIGHT] = new FATEKAddressClass(ReadINIValue("Operation Address", R32AddressEnum.ADR_TOPLIGHT.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)R32AddressEnum.ADR_ARROUNDLIGHT] = new FATEKAddressClass(ReadINIValue("Operation Address", R32AddressEnum.ADR_ARROUNDLIGHT.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)R32AddressEnum.ADR_PANNELLIGHT] = new FATEKAddressClass(ReadINIValue("Operation Address", R32AddressEnum.ADR_PANNELLIGHT.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)R32AddressEnum.ADR_GODLIGHT] = new FATEKAddressClass(ReadINIValue("Operation Address", R32AddressEnum.ADR_GODLIGHT.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)R32AddressEnum.ADR_CIRCLELIGHT] = new FATEKAddressClass(ReadINIValue("Operation Address", R32AddressEnum.ADR_CIRCLELIGHT.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)R32AddressEnum.ADR_STILTSLIGHT] = new FATEKAddressClass(ReadINIValue("Operation Address", R32AddressEnum.ADR_STILTSLIGHT.ToString(), "", INIFILE));

            ADDRESSARRAY[(int)R32AddressEnum.ADR_RED] = new FATEKAddressClass(ReadINIValue("Operation Address", R32AddressEnum.ADR_RED.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)R32AddressEnum.ADR_YELLOW] = new FATEKAddressClass(ReadINIValue("Operation Address", R32AddressEnum.ADR_YELLOW.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)R32AddressEnum.ADR_GREEN] = new FATEKAddressClass(ReadINIValue("Operation Address", R32AddressEnum.ADR_GREEN.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)R32AddressEnum.ADR_BLUE] = new FATEKAddressClass(ReadINIValue("Operation Address", R32AddressEnum.ADR_BLUE.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)R32AddressEnum.ADR_WHITE] = new FATEKAddressClass(ReadINIValue("Operation Address", R32AddressEnum.ADR_WHITE.ToString(), "", INIFILE));
        }

        public override void SaveData()
        {
            throw new NotImplementedException();
        }

        public bool Red
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R32AddressEnum.ADR_RED];
                if (!string.IsNullOrEmpty(address.Address0))
                    return PLC[address.SiteNo].IOData.GetBit(address.Address0);
                return false;//return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R32AddressEnum.ADR_RED];
                if (!string.IsNullOrEmpty(address.Address0))
                    PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }

        public bool Yellow
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R32AddressEnum.ADR_YELLOW];
                if (!string.IsNullOrEmpty(address.Address0))
                    return PLC[address.SiteNo].IOData.GetBit(address.Address0);
                return false;//return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R32AddressEnum.ADR_YELLOW];
                if (!string.IsNullOrEmpty(address.Address0))
                    PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool Green
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R32AddressEnum.ADR_GREEN];
                if (!string.IsNullOrEmpty(address.Address0))
                    return PLC[address.SiteNo].IOData.GetBit(address.Address0);
                return false;//return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R32AddressEnum.ADR_GREEN];
                if (!string.IsNullOrEmpty(address.Address0))
                    PLC[address.SiteNo].SetIO(value, address.Address0); //PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool IsStart
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R32AddressEnum.ADR_ISSTART];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool IsStop
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R32AddressEnum.ADR_ISSTOP];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool IsUPSError
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R32AddressEnum.ADR_ISUPSERROR];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool TopLight
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R32AddressEnum.ADR_TOPLIGHT];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R32AddressEnum.ADR_TOPLIGHT];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool ArroundLight
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R32AddressEnum.ADR_ARROUNDLIGHT];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R32AddressEnum.ADR_ARROUNDLIGHT];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }

        public bool PannelLight
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R32AddressEnum.ADR_PANNELLIGHT];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R32AddressEnum.ADR_PANNELLIGHT];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool GodLight
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R32AddressEnum.ADR_GODLIGHT];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R32AddressEnum.ADR_GODLIGHT];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool CircleLight
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R32AddressEnum.ADR_CIRCLELIGHT];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R32AddressEnum.ADR_CIRCLELIGHT];
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
                FATEKAddressClass address = ADDRESSARRAY[(int)R32AddressEnum.ADR_STILTSLIGHT];
                if (!string.IsNullOrEmpty(address.Address0))
                    return PLC[address.SiteNo].IOData.GetBit(address.Address0);
                return false;
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R32AddressEnum.ADR_STILTSLIGHT];
                if (!string.IsNullOrEmpty(address.Address0))
                    PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool Blue
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R32AddressEnum.ADR_BLUE];
                if (!string.IsNullOrEmpty(address.Address0))
                    return PLC[address.SiteNo].IOData.GetBit(address.Address0);
                return false;// PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R32AddressEnum.ADR_BLUE];
                if (!string.IsNullOrEmpty(address.Address0))
                    PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool White
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R32AddressEnum.ADR_WHITE];
                if (!string.IsNullOrEmpty(address.Address0))
                    return PLC[address.SiteNo].IOData.GetBit(address.Address0);
                return false;//PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)R32AddressEnum.ADR_WHITE];
                if (!string.IsNullOrEmpty(address.Address0))
                    PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
    }
}
