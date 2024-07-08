﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JetEazy.ControlSpace;
using JetEazy;


namespace Allinone.ControlSpace.IOSpace
{
    public enum MainX6AddressEnum : int
    {
        COUNT = 12,
        /// <summary>
        /// START
        /// </summary>
        ADR_ISSTART = 0,

        ADR_BUSY=1,
        ADR_READY=2,
        ADR_PASS=3,
        ADR_FAIL=4,

        ADR_TOPLIGHT=5,
        ADR_FRONTLIGHT=6,
        ADR_BACKLIGHT=7,

        ADR_ISGETIMAGE=8,
        ADR_GETIMAGEOK=9,

        /// <summary>
        /// 复位取像的步数
        /// </summary>
        ADR_GETIMAGERESET=10,
        ADR_ISHANDLEROK = 11,
    }
    public class JzMainX6IOClass : GeoIOClass
    {
        bool m_IsDebug = false;

        public JzMainX6IOClass()
        {   

        }
        public void Initial(string path,OptionEnum option,FatekPLCClass [] plc)
        {
            ADDRESSARRAY = new FATEKAddressClass[(int)MainX6AddressEnum.COUNT];

            PLC = plc;

            INIFILE = path + "\\IO.INI";

            LoadData();

        }
        public override void LoadData()
        {
            ADDRESSARRAY[(int)MainX6AddressEnum.ADR_ISSTART] = new FATEKAddressClass(ReadINIValue("Status Address", MainX6AddressEnum.ADR_ISSTART.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainX6AddressEnum.ADR_ISGETIMAGE] = new FATEKAddressClass(ReadINIValue("Status Address", MainX6AddressEnum.ADR_ISGETIMAGE.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainX6AddressEnum.ADR_GETIMAGERESET] = new FATEKAddressClass(ReadINIValue("Status Address", MainX6AddressEnum.ADR_GETIMAGERESET.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainX6AddressEnum.ADR_ISHANDLEROK] = new FATEKAddressClass(ReadINIValue("Status Address", MainX6AddressEnum.ADR_ISHANDLEROK.ToString(), "", INIFILE));


            ADDRESSARRAY[(int)MainX6AddressEnum.ADR_BUSY] = new FATEKAddressClass(ReadINIValue("Operation Address", MainX6AddressEnum.ADR_BUSY.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainX6AddressEnum.ADR_READY] = new FATEKAddressClass(ReadINIValue("Operation Address", MainX6AddressEnum.ADR_READY.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainX6AddressEnum.ADR_PASS] = new FATEKAddressClass(ReadINIValue("Operation Address", MainX6AddressEnum.ADR_PASS.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainX6AddressEnum.ADR_FAIL] = new FATEKAddressClass(ReadINIValue("Operation Address", MainX6AddressEnum.ADR_FAIL.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainX6AddressEnum.ADR_TOPLIGHT] = new FATEKAddressClass(ReadINIValue("Operation Address", MainX6AddressEnum.ADR_TOPLIGHT.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainX6AddressEnum.ADR_FRONTLIGHT] = new FATEKAddressClass(ReadINIValue("Operation Address", MainX6AddressEnum.ADR_FRONTLIGHT.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainX6AddressEnum.ADR_BACKLIGHT] = new FATEKAddressClass(ReadINIValue("Operation Address", MainX6AddressEnum.ADR_BACKLIGHT.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainX6AddressEnum.ADR_GETIMAGEOK] = new FATEKAddressClass(ReadINIValue("Operation Address", MainX6AddressEnum.ADR_GETIMAGEOK.ToString(), "", INIFILE));


            m_IsDebug = ReadINIValue("Parameters", "IsDebug", "0", INIFILE) == "1";
        }

        public override void SaveData()
        {
            throw new NotImplementedException();
        }

        public bool[] M_X = new bool[8];
        public bool[] M_Y = new bool[8];

        public bool IsStart
        {
            get
            {
                if (m_IsDebug)
                {
                    return M_X[0];
                }

                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.ADR_ISSTART];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                if (m_IsDebug)
                {
                    M_X[0] = value;
                }
            }
        }
        public bool IsGetImage
        {
            get
            {
                if (m_IsDebug)
                {
                    return M_X[1];
                }

                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.ADR_ISGETIMAGE];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                if (m_IsDebug)
                {
                    M_X[1] = value;
                }
            }
        }
        public bool IsGetImageReset
        {
            get
            {
                if (m_IsDebug)
                {
                    return M_X[2];
                }

                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.ADR_GETIMAGERESET];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                if (m_IsDebug)
                {
                    M_X[2] = value;
                }
            }
        }
        public bool IsHandlerOK
        {
            get
            {
                if (m_IsDebug)
                {
                    return M_X[3];
                }

                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.ADR_ISHANDLEROK];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                if (m_IsDebug)
                {
                    M_X[3] = value;
                }
            }
        }

        public bool Busy
        {
            get
            {
                if (m_IsDebug)
                {
                    return M_Y[0];
                }

                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.ADR_BUSY];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                if (m_IsDebug)
                {
                    M_Y[0] = value;
                    return;
                }

                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.ADR_BUSY];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool Ready
        {
            get
            {
                if (m_IsDebug)
                {
                    return M_Y[1];
                }

                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.ADR_READY];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                if (m_IsDebug)
                {
                    M_Y[1] = value;
                    return;
                }
                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.ADR_READY];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool Pass
        {
            get
            {

                if (m_IsDebug)
                {
                    return M_Y[2];
                }

                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.ADR_PASS];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                if (m_IsDebug)
                {
                    M_Y[2] = value;
                    return;
                }
                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.ADR_PASS];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool Fail
        {
            get
            {
                if (m_IsDebug)
                {
                    return M_Y[3];
                }

                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.ADR_FAIL];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                if (m_IsDebug)
                {
                    M_Y[3] = value;
                    return;
                }
                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.ADR_FAIL];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool GetImageOK
        {
            get
            {
                if (m_IsDebug)
                {
                    return M_Y[4];
                }

                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.ADR_GETIMAGEOK];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                if (m_IsDebug)
                {
                    M_Y[4] = value;
                    return;
                }
                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.ADR_GETIMAGEOK];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }

        public bool TopLight
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.ADR_TOPLIGHT];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.ADR_TOPLIGHT];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool FrontLight
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.ADR_FRONTLIGHT];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.ADR_FRONTLIGHT];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool BackLight
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.ADR_BACKLIGHT];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.ADR_BACKLIGHT];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }

        public void SetRGBWLight()
        {

        }

    }
}
