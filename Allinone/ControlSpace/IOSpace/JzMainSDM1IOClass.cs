using JetEazy;
using JetEazy.ControlSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allinone.ControlSpace.IOSpace
{
    public enum MainSDM1AddressEnum : int
    {
        COUNT = 16,

        ADR_ISSTART = 0,
        ADR_ISEMC = 1,

        ADR_RED = 2,
        ADR_YELLOW = 3,
        ADR_GREEN = 4,

        /// <summary>
        /// 平板灯
        /// </summary>
        ADR_TOPLIGHT = 5,
        /// <summary>
        ///同轴光
        /// </summary>
        ADR_COAXIALLIGHT = 6,


        ADR_BUSY = 7,
        ADR_READY = 8,
        ADR_PASS = 9,
        ADR_FAIL = 10,

        ADR_ISGETIMAGE = 11,
        ADR_GETIMAGEOK = 12,

        /// <summary>
        /// 复位取像的步数
        /// </summary>
        ADR_GETIMAGERESET = 13,
        ADR_ISRESET = 14,

        ADR_CLEAR_ALARM=15,

    }
    public class JzMainSDM1IOClass : GeoIOClass
    {

        public JzMainSDM1IOClass()
        {

        }
        public void Initial(string path, OptionEnum option, FatekPLCClass[] plc)
        {
            ADDRESSARRAY = new FATEKAddressClass[(int)MainSDM1AddressEnum.COUNT];

            PLC = plc;

            INIFILE = path + "\\IO.INI";

            LoadData();

        }
        public override void LoadData()
        {
            ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_ISSTART] = new FATEKAddressClass(ReadINIValue("Status Address", MainSDM1AddressEnum.ADR_ISSTART.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_ISEMC] = new FATEKAddressClass(ReadINIValue("Status Address", MainSDM1AddressEnum.ADR_ISEMC.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_ISRESET] = new FATEKAddressClass(ReadINIValue("Status Address", MainSDM1AddressEnum.ADR_ISRESET.ToString(), "", INIFILE));


            ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_ISGETIMAGE] = new FATEKAddressClass(ReadINIValue("Status Address", MainSDM1AddressEnum.ADR_ISGETIMAGE.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_GETIMAGERESET] = new FATEKAddressClass(ReadINIValue("Status Address", MainSDM1AddressEnum.ADR_GETIMAGERESET.ToString(), "", INIFILE));


            ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_BUSY] = new FATEKAddressClass(ReadINIValue("Operation Address", MainSDM1AddressEnum.ADR_BUSY.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_READY] = new FATEKAddressClass(ReadINIValue("Operation Address", MainSDM1AddressEnum.ADR_READY.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_PASS] = new FATEKAddressClass(ReadINIValue("Operation Address", MainSDM1AddressEnum.ADR_PASS.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_FAIL] = new FATEKAddressClass(ReadINIValue("Operation Address", MainSDM1AddressEnum.ADR_FAIL.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_GETIMAGEOK] = new FATEKAddressClass(ReadINIValue("Operation Address", MainSDM1AddressEnum.ADR_GETIMAGEOK.ToString(), "", INIFILE));

            ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_CLEAR_ALARM] = new FATEKAddressClass(ReadINIValue("Operation Address", MainSDM1AddressEnum.ADR_CLEAR_ALARM.ToString(), "", INIFILE));


            ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_TOPLIGHT] = new FATEKAddressClass(ReadINIValue("Operation Address", MainSDM1AddressEnum.ADR_TOPLIGHT.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_COAXIALLIGHT] = new FATEKAddressClass(ReadINIValue("Operation Address", MainSDM1AddressEnum.ADR_COAXIALLIGHT.ToString(), "", INIFILE));
           
            ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_RED] = new FATEKAddressClass(ReadINIValue("Operation Address", MainSDM1AddressEnum.ADR_RED.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_YELLOW] = new FATEKAddressClass(ReadINIValue("Operation Address", MainSDM1AddressEnum.ADR_YELLOW.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_GREEN] = new FATEKAddressClass(ReadINIValue("Operation Address", MainSDM1AddressEnum.ADR_GREEN.ToString(), "", INIFILE));
            
        }

        public override void SaveData()
        {
            throw new NotImplementedException();
        }

        public bool Red
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_RED];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_RED];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool Yellow
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_YELLOW];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_YELLOW];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool Green
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_GREEN];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_GREEN];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool IsStart
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_ISSTART];

                if (Ready)
                    return PLC[address.SiteNo].IOData.GetBit(address.Address1);

                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool IsEMC
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_ISEMC];
                return !PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool IsRESET
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_ISRESET];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool TopLight
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_TOPLIGHT];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_TOPLIGHT];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool CoaxialLight
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_COAXIALLIGHT];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_COAXIALLIGHT];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool ClearAlarm
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_CLEAR_ALARM];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_CLEAR_ALARM];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }

        public bool IsGetImage
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_ISGETIMAGE];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool IsGetImageReset
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_GETIMAGERESET];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }

        public bool Busy
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_BUSY];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_BUSY];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool Ready
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_READY];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_READY];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool Pass
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_PASS];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_PASS];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool Fail
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_FAIL];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_FAIL];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool GetImageOK
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_GETIMAGEOK];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM1AddressEnum.ADR_GETIMAGEOK];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
    }
}
