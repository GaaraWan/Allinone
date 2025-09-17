using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JetEazy.ControlSpace;
using JetEazy;
using static Allinone.ControlSpace.IOSpace.JzCipMainX6IO1Class;


namespace Allinone.ControlSpace.IOSpace
{
    public enum MainX6AddressEnum : int
    {
        COUNT = 20,
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


        iQcNum=12,
        iQcXTotal=13,
        iQcYTotal=14,
        bSoftwareReady=15,
        bHeartBeat=16,
        iQcResult=17,
        RecipeName=18,
        Map=19,
    }
    public class JzMainX6IOClass : GeoIOClass
    {
        bool m_IsDebug = false;
        string QcDebugStr = $"2,2,4";
        bool[] m_TcpStart = new bool[100];

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

            ADDRESSARRAY[(int)MainX6AddressEnum.iQcNum] = new FATEKAddressClass(ReadINIValue("Status Address", MainX6AddressEnum.iQcNum.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainX6AddressEnum.iQcXTotal] = new FATEKAddressClass(ReadINIValue("Status Address", MainX6AddressEnum.iQcXTotal.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainX6AddressEnum.iQcYTotal] = new FATEKAddressClass(ReadINIValue("Status Address", MainX6AddressEnum.iQcYTotal.ToString(), "", INIFILE));


            ADDRESSARRAY[(int)MainX6AddressEnum.ADR_BUSY] = new FATEKAddressClass(ReadINIValue("Operation Address", MainX6AddressEnum.ADR_BUSY.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainX6AddressEnum.ADR_READY] = new FATEKAddressClass(ReadINIValue("Operation Address", MainX6AddressEnum.ADR_READY.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainX6AddressEnum.ADR_PASS] = new FATEKAddressClass(ReadINIValue("Operation Address", MainX6AddressEnum.ADR_PASS.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainX6AddressEnum.ADR_FAIL] = new FATEKAddressClass(ReadINIValue("Operation Address", MainX6AddressEnum.ADR_FAIL.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainX6AddressEnum.ADR_TOPLIGHT] = new FATEKAddressClass(ReadINIValue("Operation Address", MainX6AddressEnum.ADR_TOPLIGHT.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainX6AddressEnum.ADR_FRONTLIGHT] = new FATEKAddressClass(ReadINIValue("Operation Address", MainX6AddressEnum.ADR_FRONTLIGHT.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainX6AddressEnum.ADR_BACKLIGHT] = new FATEKAddressClass(ReadINIValue("Operation Address", MainX6AddressEnum.ADR_BACKLIGHT.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainX6AddressEnum.ADR_GETIMAGEOK] = new FATEKAddressClass(ReadINIValue("Operation Address", MainX6AddressEnum.ADR_GETIMAGEOK.ToString(), "", INIFILE));

            ADDRESSARRAY[(int)MainX6AddressEnum.bSoftwareReady] = new FATEKAddressClass(ReadINIValue("Operation Address", MainX6AddressEnum.bSoftwareReady.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainX6AddressEnum.bHeartBeat] = new FATEKAddressClass(ReadINIValue("Operation Address", MainX6AddressEnum.bHeartBeat.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainX6AddressEnum.iQcResult] = new FATEKAddressClass(ReadINIValue("Operation Address", MainX6AddressEnum.iQcResult.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainX6AddressEnum.RecipeName] = new FATEKAddressClass(ReadINIValue("Operation Address", MainX6AddressEnum.RecipeName.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainX6AddressEnum.Map] = new FATEKAddressClass(ReadINIValue("Operation Address", MainX6AddressEnum.Map.ToString(), "0:Gvl_QcPC.Map", INIFILE));


            m_IsDebug = ReadINIValue("Parameters", "IsDebug", "0", INIFILE) == "1";

            loadQcDebugStr();
            for (int i = 0; i < 100; i++)
            {
                m_TcpStart[i] = false;
            }
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
                if (string.IsNullOrEmpty(address.Address0))
                    return false;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":

                        return PLC[address.SiteNo].ReadVari(address.Address0).ToLower() == "true";

                        break;

                }
                
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
                if (string.IsNullOrEmpty(address.Address0))
                    return false;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":

                        return PLC[address.SiteNo].ReadVari(address.Address0).ToLower() == "true";

                        break;

                }
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
                if (string.IsNullOrEmpty(address.Address0))
                    return false;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":

                        return PLC[address.SiteNo].ReadVari(address.Address0).ToLower() == "true";

                        break;

                }
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
                if (string.IsNullOrEmpty(address.Address0))
                    return false;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":

                        return PLC[address.SiteNo].ReadVari(address.Address0).ToLower() == "true";

                        break;

                }
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
        public bool IsGetTcpStart(int index = 0)
        {
            if (index < 0 || index >= m_TcpStart.Length)
                return false;
            return m_TcpStart[index];
            //get
            //{
            //    //if (m_TcpStart)
            //    //{
            //    //    m_TcpStart = false;
            //    //    return true;
            //    //}
            //    //return false;

            //    return m_TcpStart;
            //}
            //set
            //{
            //    m_TcpStart = value;
            //}
        }
        public void SetTcpStart(int index = 0, bool ison = false)
        {
            if (index < 0 || index >= m_TcpStart.Length)
                return;
            m_TcpStart[index] = ison;
        }

        /// <summary>
        /// 拍照次数
        /// </summary>
        public int QcNum
        {
            get
            {
                if (m_IsDebug)
                {
                    string str = getQcDebugStrIndex(0);
                    if (!string.IsNullOrEmpty(str))
                        return int.Parse(str);
                    return 1;
                }

                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.iQcNum];
                if (string.IsNullOrEmpty(address.Address0))
                    return 1;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":

                        string ret = PLC[address.SiteNo].ReadVari(address.Address0);
                        int iret = 1;
                        int.TryParse(ret, out iret);
                        return iret;
                        break;
                }
                return 1;
            }
        }
        /// <summary>
        /// X方向总颗数
        /// </summary>
        public int QcXTotal
        {
            get
            {
                if (m_IsDebug)
                {
                    string str = getQcDebugStrIndex(1);
                    if (!string.IsNullOrEmpty(str))
                        return int.Parse(str);
                    return 1;
                }

                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.iQcXTotal];
                if (string.IsNullOrEmpty(address.Address0))
                    return 1;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":

                        string ret = PLC[address.SiteNo].ReadVari(address.Address0);
                        int iret = 1;
                        int.TryParse(ret, out iret);
                        return iret;
                        break;
                }
                return 1;
                
            }
        }
        /// <summary>
        /// Y方向总颗数
        /// </summary>
        public int QcYTotal
        {
            get
            {
                if (m_IsDebug)
                {
                    string str = getQcDebugStrIndex(2);
                    if (!string.IsNullOrEmpty(str))
                        return int.Parse(str);
                    return 1;
                }
                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.iQcYTotal];
                if (string.IsNullOrEmpty(address.Address0))
                    return 1;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":

                        string ret = PLC[address.SiteNo].ReadVari(address.Address0);
                        int iret = 1;
                        int.TryParse(ret, out iret);
                        return iret;
                        break;
                }
                return 1;
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
                if (string.IsNullOrEmpty(address.Address0))
                    return false;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":

                        return PLC[address.SiteNo].ReadVari(address.Address0).ToLower() == "true";

                        break;

                }
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
                if (string.IsNullOrEmpty(address.Address0))
                    return;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":
                        PLC[address.SiteNo].WriteVari(address.Address0, (value ? "true" : "false"));
                        return;
                        break;

                }
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
                if (string.IsNullOrEmpty(address.Address0))
                    return false;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":

                        return PLC[address.SiteNo].ReadVari(address.Address0).ToLower() == "true";

                        break;

                }
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
                if (string.IsNullOrEmpty(address.Address0))
                    return;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":
                        PLC[address.SiteNo].WriteVari(address.Address0, (value ? "true" : "false"));
                        return;
                        break;

                }
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
                if (string.IsNullOrEmpty(address.Address0))
                    return false;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":
                        //if(QcResult == 1)
                        return QcResult == 1;
                        //return PLC[address.SiteNo].ReadVari(address.Address0).ToLower() == "true";

                        break;

                }
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
                if (string.IsNullOrEmpty(address.Address0))
                    return;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":
                        if (value)
                            QcResult = 1;
                        else
                            QcResult = 0;
                        //else
                        //    QcResult = 1;
                        //PLC[address.SiteNo].WriteVari(address.Address0, (value ? "true" : "false"));
                        return;
                        break;

                }
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
                if (string.IsNullOrEmpty(address.Address0))
                    return false;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":
                        return QcResult == 2;
                        //return PLC[address.SiteNo].ReadVari(address.Address0).ToLower() == "true";

                        break;

                }
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
                if (string.IsNullOrEmpty(address.Address0))
                    return;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":
                        if (value)
                            QcResult = 2;
                        else
                            QcResult = 0;
                        //else
                        //    QcResult = 1;
                        //PLC[address.SiteNo].WriteVari(address.Address0, (value ? "true" : "false"));
                        return;
                        break;

                }
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
                if (string.IsNullOrEmpty(address.Address0))
                    return false;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":

                        return PLC[address.SiteNo].ReadVari(address.Address0).ToLower() == "true";

                        break;

                }
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
                if (string.IsNullOrEmpty(address.Address0))
                    return;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":
                        PLC[address.SiteNo].WriteVari(address.Address0, (value ? "true" : "false"));
                        return;
                        break;

                }
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }

        public bool TopLight
        {
            get
            {
                if (m_IsDebug)
                    return false;
                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.ADR_TOPLIGHT];
                if (string.IsNullOrEmpty(address.Address0))
                    return false;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":

                        return PLC[address.SiteNo].ReadVari(address.Address0).ToLower() == "true";

                        break;

                }
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                if (m_IsDebug)
                    return;
                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.ADR_TOPLIGHT];
                if (string.IsNullOrEmpty(address.Address0))
                    return;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":
                        PLC[address.SiteNo].WriteVari(address.Address0, (value ? "true" : "false"));
                        return;
                        break;

                }
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool FrontLight
        {
            get
            {
                if (m_IsDebug)
                    return false;
                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.ADR_FRONTLIGHT];
                if (string.IsNullOrEmpty(address.Address0))
                    return false;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":

                        return PLC[address.SiteNo].ReadVari(address.Address0).ToLower() == "true";

                        break;

                }
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                if (m_IsDebug)
                    return;
                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.ADR_FRONTLIGHT];
                if (string.IsNullOrEmpty(address.Address0))
                    return;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":
                        PLC[address.SiteNo].WriteVari(address.Address0, (value ? "true" : "false"));
                        return;
                        break;

                }
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool BackLight
        {
            get
            {
                if (m_IsDebug)
                    return false;
                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.ADR_BACKLIGHT];
                if (string.IsNullOrEmpty(address.Address0))
                    return false;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":

                        return PLC[address.SiteNo].ReadVari(address.Address0).ToLower() == "true";

                        break;

                }
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                if (m_IsDebug)
                    return;
                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.ADR_BACKLIGHT];
                if (string.IsNullOrEmpty(address.Address0))
                    return;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":
                        PLC[address.SiteNo].WriteVari(address.Address0, (value ? "true" : "false"));
                        return;
                        break;

                }
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }

        //public void SetRGBWLight()
        //{

        //}

        public bool SoftwareReady
        {
            get
            {
                if (m_IsDebug)
                    return false;
                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.bSoftwareReady];
                if (string.IsNullOrEmpty(address.Address0))
                    return false;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":

                        return PLC[address.SiteNo].ReadVari(address.Address0).ToLower() == "true";

                        break;

                }
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                if (m_IsDebug)
                    return;
                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.bSoftwareReady];
                if (string.IsNullOrEmpty(address.Address0))
                    return;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":
                        PLC[address.SiteNo].WriteVari(address.Address0, (value ? "true" : "false"));
                        return;
                        break;

                }
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool HeartBeat
        {
            get
            {
                if (m_IsDebug)
                    return false;
                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.bHeartBeat];
                if (string.IsNullOrEmpty(address.Address0))
                    return false;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":

                        return PLC[address.SiteNo].ReadVari(address.Address0).ToLower() == "true";

                        break;

                }
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                if (m_IsDebug)
                    return;
                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.bHeartBeat];
                if (string.IsNullOrEmpty(address.Address0))
                    return;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":
                        PLC[address.SiteNo].WriteVari(address.Address0, (value ? "true" : "false"));
                        //return;
                        break;

                }
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        int QcResult
        {
            get
            {
                if (m_IsDebug)
                    return 1;
                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.iQcResult];
                string ret = PLC[address.SiteNo].ReadVari(address.Address0);
                int iret = 1;
                int.TryParse(ret, out iret);
                return iret;
            }
            set
            {
                if (m_IsDebug)
                    return;
                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.iQcResult];
                if (string.IsNullOrEmpty(address.Address0))
                    return;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":
                        PLC[address.SiteNo].WriteVari(address.Address0, value.ToString());

                        break;

                }
            }
        }
        public string RecipeName
        {
            get
            {
                if (m_IsDebug)
                    return string.Empty;
                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.RecipeName];
                if (string.IsNullOrEmpty(address.Address0))
                    return "";

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":

                        return PLC[address.SiteNo].ReadVari(address.Address0);

                        break;

                }
                return "";
            }
            set
            {
                if (m_IsDebug)
                    return;
                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.RecipeName];
                if (string.IsNullOrEmpty(address.Address0))
                    return;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":
                        PLC[address.SiteNo].WriteVari(address.Address0, value);

                        break;

                }

            }
        }
        public string QcMap
        {
            get
            {
                if (m_IsDebug)
                {
                    string str = getQcDebugStrIndex(3);
                    if (!string.IsNullOrEmpty(str))
                        return str;
                    return string.Empty;
                }

                FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.Map];
                if (string.IsNullOrEmpty(address.Address0))
                    return string.Empty;

                switch (PLC[address.SiteNo].TypeStr)
                {
                    case "CIP":

                        return PLC[address.SiteNo].ReadVari(address.Address0);

                        //int icount = QcXTotal * QcYTotal;
                        //bool[] strings = new bool[icount];
                        //for (int i = 0; i < icount; i++)
                        //{
                        //    strings[i] = CIP.ReadVari($"{address.Address0}[{i}]") == "0";
                        //}
                        //return strings;
                        //break;

                }
                return string.Empty;
            }
            //set
            //{
            //    FATEKAddressClass address = ADDRESSARRAY[(int)MainX6AddressEnum.Map];
            //    if (string.IsNullOrEmpty(address.Address0))
            //        return;

            //    switch (PLC[address.SiteNo].TypeStr)
            //    {
            //        case "CIP":
            //            //PLC[address.SiteNo].WriteVari(address.Address0, value);

            //            break;

            //    }

            //}
        }

        void loadQcDebugStr()
        {
            QcDebugStr = ReadINIValue("Parameters", "QcDebugStr", QcDebugStr, INIFILE);
        }
        string getQcDebugStrIndex(int eIndex)
        {
            loadQcDebugStr();
            string[] strings = QcDebugStr.Split(',');
            if (eIndex < strings.Length)
            {
                return strings[eIndex];
            }
            return null;
        }

    }
}
