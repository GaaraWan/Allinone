using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JetEazy.ControlSpace;
using JetEazy;


namespace Allinone.ControlSpace.IOSpace
{
    public enum DFlyAddressEnum : int
    {
        COUNT = 7,

        ADR_ISEMC = 0,

        ADR_RED = 1,
        ADR_GREEN = 2,
        ADR_BLUE = 3,
        ADR_WHITE = 4,
        ADR_DOOR = 5,
        ADR_STILTS = 6,
    }
    public class JzDFlyPLCIOClass : GeoIOClass
    {
        public JzDFlyPLCIOClass()
        {   

        }
        public void Initial(string path,OptionEnum option,FatekPLCClass [] plc)
        {
            ADDRESSARRAY = new FATEKAddressClass[(int)DFlyAddressEnum.COUNT];

            PLC = plc;

            INIFILE = path + "\\IO.INI";

            LoadData();
        }
        public override void LoadData()
        {
            ADDRESSARRAY[(int)DFlyAddressEnum.ADR_ISEMC] = new FATEKAddressClass(ReadINIValue("Status Address", DFlyAddressEnum.ADR_ISEMC.ToString(), "", INIFILE));

            ADDRESSARRAY[(int)DFlyAddressEnum.ADR_RED] = new FATEKAddressClass(ReadINIValue("Operation Address", DFlyAddressEnum.ADR_RED.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)DFlyAddressEnum.ADR_BLUE] = new FATEKAddressClass(ReadINIValue("Operation Address", DFlyAddressEnum.ADR_BLUE.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)DFlyAddressEnum.ADR_GREEN] = new FATEKAddressClass(ReadINIValue("Operation Address", DFlyAddressEnum.ADR_GREEN.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)DFlyAddressEnum.ADR_WHITE] = new FATEKAddressClass(ReadINIValue("Operation Address", DFlyAddressEnum.ADR_WHITE.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)DFlyAddressEnum.ADR_DOOR] = new FATEKAddressClass(ReadINIValue("Operation Address", DFlyAddressEnum.ADR_DOOR.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)DFlyAddressEnum.ADR_STILTS] = new FATEKAddressClass(ReadINIValue("Operation Address", DFlyAddressEnum.ADR_STILTS.ToString(), "", INIFILE));
        }

        public override void SaveData()
        {
            throw new NotImplementedException();
        }

        public bool Red
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)DFlyAddressEnum.ADR_RED];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)DFlyAddressEnum.ADR_RED];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }

        public bool Blue
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)DFlyAddressEnum.ADR_BLUE];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)DFlyAddressEnum.ADR_BLUE];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool Green
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)DFlyAddressEnum.ADR_GREEN];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)DFlyAddressEnum.ADR_GREEN];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool White
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)DFlyAddressEnum.ADR_WHITE];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)DFlyAddressEnum.ADR_WHITE];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool Door
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)DFlyAddressEnum.ADR_DOOR];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)DFlyAddressEnum.ADR_DOOR];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool IsEMC
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)DFlyAddressEnum.ADR_ISEMC];
                return !PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
    }
}
