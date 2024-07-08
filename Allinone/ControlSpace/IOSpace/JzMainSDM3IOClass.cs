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
    public enum MainSDM3AddressEnum : int
    {
        COUNT = 19,

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
        ADR_ABS=16,

        ADR_ISUSERSTART=17,
        ADR_ISUSERSTOP=18,

    }
    public class JzMainSDM3IOClass : GeoIOClass
    {

        public JzMainSDM3IOClass()
        {

        }
        public void Initial(string path, OptionEnum option, FatekPLCClass[] plc)
        {
            ADDRESSARRAY = new FATEKAddressClass[(int)MainSDM3AddressEnum.COUNT];

            PLC = plc;

            INIFILE = path + "\\IO.INI";

            LoadData();

        }
        public override void LoadData()
        {
            ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_ISSTART] = new FATEKAddressClass(ReadINIValue("Status Address", MainSDM3AddressEnum.ADR_ISSTART.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_ISEMC] = new FATEKAddressClass(ReadINIValue("Status Address", MainSDM3AddressEnum.ADR_ISEMC.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_ISRESET] = new FATEKAddressClass(ReadINIValue("Status Address", MainSDM3AddressEnum.ADR_ISRESET.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_ISUSERSTART] = new FATEKAddressClass(ReadINIValue("Status Address", MainSDM3AddressEnum.ADR_ISUSERSTART.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_ISUSERSTOP] = new FATEKAddressClass(ReadINIValue("Status Address", MainSDM3AddressEnum.ADR_ISUSERSTOP.ToString(), "", INIFILE));


            ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_ISGETIMAGE] = new FATEKAddressClass(ReadINIValue("Status Address", MainSDM3AddressEnum.ADR_ISGETIMAGE.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_GETIMAGERESET] = new FATEKAddressClass(ReadINIValue("Status Address", MainSDM3AddressEnum.ADR_GETIMAGERESET.ToString(), "", INIFILE));


            ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_BUSY] = new FATEKAddressClass(ReadINIValue("Operation Address", MainSDM3AddressEnum.ADR_BUSY.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_READY] = new FATEKAddressClass(ReadINIValue("Operation Address", MainSDM3AddressEnum.ADR_READY.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_PASS] = new FATEKAddressClass(ReadINIValue("Operation Address", MainSDM3AddressEnum.ADR_PASS.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_FAIL] = new FATEKAddressClass(ReadINIValue("Operation Address", MainSDM3AddressEnum.ADR_FAIL.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_GETIMAGEOK] = new FATEKAddressClass(ReadINIValue("Operation Address", MainSDM3AddressEnum.ADR_GETIMAGEOK.ToString(), "", INIFILE));

            ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_CLEAR_ALARM] = new FATEKAddressClass(ReadINIValue("Operation Address", MainSDM3AddressEnum.ADR_CLEAR_ALARM.ToString(), "", INIFILE));

            ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_ABS] = new FATEKAddressClass(ReadINIValue("Operation Address", MainSDM3AddressEnum.ADR_ABS.ToString(), "", INIFILE));


            ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_TOPLIGHT] = new FATEKAddressClass(ReadINIValue("Operation Address", MainSDM3AddressEnum.ADR_TOPLIGHT.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_COAXIALLIGHT] = new FATEKAddressClass(ReadINIValue("Operation Address", MainSDM3AddressEnum.ADR_COAXIALLIGHT.ToString(), "", INIFILE));
           
            ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_RED] = new FATEKAddressClass(ReadINIValue("Operation Address", MainSDM3AddressEnum.ADR_RED.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_YELLOW] = new FATEKAddressClass(ReadINIValue("Operation Address", MainSDM3AddressEnum.ADR_YELLOW.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_GREEN] = new FATEKAddressClass(ReadINIValue("Operation Address", MainSDM3AddressEnum.ADR_GREEN.ToString(), "", INIFILE));
            
        }

        public override void SaveData()
        {
            throw new NotImplementedException();
        }

        public bool Red
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_RED];
                if (string.IsNullOrEmpty(address.Address0))
                    return false;
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_RED];
                if (string.IsNullOrEmpty(address.Address0))
                    return;
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool Yellow
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_YELLOW];
                if (string.IsNullOrEmpty(address.Address0))
                    return false;
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_YELLOW];
                if (string.IsNullOrEmpty(address.Address0))
                    return;
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool Green
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_GREEN];
                if (string.IsNullOrEmpty(address.Address0))
                    return false;
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_GREEN];
                if (string.IsNullOrEmpty(address.Address0))
                    return;
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool IsStart
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_ISSTART];

                switch(robotType1)
                {
                    case RobotType.HCFA:
                        return robotHCFA.GetDI(int.Parse(address.Address0));
                        break;
                }
                
                //if (Ready)
                //    return PLC[address.SiteNo].IOData.GetBit(address.Address1);

                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool IsUserStart
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_ISUSERSTART];

                switch (robotType1)
                {
                    case RobotType.HCFA:
                        return robotHCFA.GetDI(int.Parse(address.Address0));
                        break;
                }

                //if (Ready)
                //    return PLC[address.SiteNo].IOData.GetBit(address.Address1);

                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool IsUserStop
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_ISUSERSTOP];

                switch (robotType1)
                {
                    case RobotType.HCFA:
                        return robotHCFA.GetDI(int.Parse(address.Address0));
                        break;
                }

                //if (Ready)
                //    return PLC[address.SiteNo].IOData.GetBit(address.Address1);

                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool IsEMC
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_ISEMC];

                switch (robotType1)
                {
                    case RobotType.HCFA:
                        return !robotHCFA.GetDI(int.Parse(address.Address0));
                        break;
                }

                return !PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool IsRESET
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_ISRESET];
                if (string.IsNullOrEmpty(address.Address0))
                    return false;
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool IsALARM
        {
            get
            {
                FATEKAddressClass address = new FATEKAddressClass("0:IX64.2");
                if (string.IsNullOrEmpty(address.Address0))
                    return false;
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool TopLight
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_TOPLIGHT];

                switch (robotType1)
                {
                    case RobotType.HCFA:
                        return robotHCFA.GetDO(int.Parse(address.Address0));
                        break;
                }

                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_TOPLIGHT];

                switch (robotType1)
                {
                    case RobotType.HCFA:
                        robotHCFA.SetDO(int.Parse(address.Address0), value);
                        return;
                        break;
                }

                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool CoaxialLight
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_COAXIALLIGHT];
                if (string.IsNullOrEmpty(address.Address0))
                    return false;
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_COAXIALLIGHT];
                if (string.IsNullOrEmpty(address.Address0))
                    return;
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool ClearAlarm
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_CLEAR_ALARM];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_CLEAR_ALARM];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }

        public bool IsGetImage
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_ISGETIMAGE];
                if (string.IsNullOrEmpty(address.Address0))
                    return false;
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool IsGetImageReset
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_GETIMAGERESET];
                if (string.IsNullOrEmpty(address.Address0))
                    return false;
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }

        public bool Busy
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_BUSY];
                if (string.IsNullOrEmpty(address.Address0))
                    return false;
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_BUSY];
                if (string.IsNullOrEmpty(address.Address0))
                    return;
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool Ready
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_READY];

                switch (robotType1)
                {
                    case RobotType.HCFA:
                        return robotHCFA.GetDO(int.Parse(address.Address0));
                        break;
                }

                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_READY];

                switch (robotType1)
                {
                    case RobotType.HCFA:
                        robotHCFA.SetDO(int.Parse(address.Address0), value);
                        return;
                        break;
                }

                PLC[address.SiteNo].SetIO(value, address.Address0);

                //Task task = new Task(new Action(() =>
                //{
                //    FATEKAddressClass address1 = new FATEKAddressClass("0:IX64.0");
                //    System.Threading.Thread.Sleep(20);
                //    System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                //    watch.Restart();
                //    while (!PLC[address1.SiteNo].IOData.GetBit(address1.Address0))
                //    {
                //        if (watch.ElapsedMilliseconds > 500)
                //            break;
                //        //PLC[address1.SiteNo].IOData.GetBit(address1.Address0);
                //    }
                //    //System.Threading.Thread.Sleep(500);
                //    //AddressClass address = ADDRESSARRAY[(int)MotionAddressEnum.ADR_GO];
                //    PLC[address.SiteNo].SetIO(false, address.Address0);
                //}));
                //task.Start();
            }
        }
        public bool Pass
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_PASS];

                switch (robotType1)
                {
                    case RobotType.HCFA:
                        return robotHCFA.GetDO(int.Parse(address.Address0));
                        break;
                }

                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_PASS];

                switch (robotType1)
                {
                    case RobotType.HCFA:
                        robotHCFA.SetDO(int.Parse(address.Address0), value);
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
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_FAIL];
                if (string.IsNullOrEmpty(address.Address0))
                    return false;
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_FAIL];
                if (string.IsNullOrEmpty(address.Address0))
                    return;
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool GetImageOK
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_GETIMAGEOK];
                if (string.IsNullOrEmpty(address.Address0))
                    return false;
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_GETIMAGEOK];
                if (string.IsNullOrEmpty(address.Address0))
                    return;
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool RobotAbs
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_ABS];
                if (string.IsNullOrEmpty(address.Address0))
                    return false;
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDM3AddressEnum.ADR_ABS];
                if (string.IsNullOrEmpty(address.Address0))
                    return;
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }

        public void SetAxisJJS(string eMwAddress, int eValue)
        {
            FATEKAddressClass address = new FATEKAddressClass(eMwAddress);
            if (address.SiteNo == -1)
                return;
            //long setH = value >> 16;
            long setL = eValue % 65536;

            PLC[address.SiteNo].SetData(ValueToHEX(setL, 4), address.Address0);
            //PLC[address.SiteNo].SetData(ValueToHEX(setH, 4), address.Address1);
        }
        public int ADR_AXIS_X_JJS
        {
            set
            {
                FATEKAddressClass address = new FATEKAddressClass("0:R0120");

                //long setH = value >> 16;
                long setL = value % 65536;

                PLC[address.SiteNo].SetData(ValueToHEX(setL, 4), address.Address0);
                //PLC[address.SiteNo].SetData(ValueToHEX(setH, 4), address.Address1);
            }
        }
        public int ADR_AXIS_Y_JJS
        {
            set
            {
                FATEKAddressClass address = new FATEKAddressClass("0:R0220");

                //long setH = value >> 16;
                long setL = value % 65536;

                PLC[address.SiteNo].SetData(ValueToHEX(setL, 4), address.Address0);
                //PLC[address.SiteNo].SetData(ValueToHEX(setH, 4), address.Address1);
            }
        }
        public int ADR_AXIS_Z_JJS
        {
            set
            {
                FATEKAddressClass address = new FATEKAddressClass("0:R0320");

                //long setH = value >> 16;
                long setL = value % 65536;

                PLC[address.SiteNo].SetData(ValueToHEX(setL, 4), address.Address0);
                //PLC[address.SiteNo].SetData(ValueToHEX(setH, 4), address.Address1);
            }
        }
        public bool ADR_WRITE_JJS
        {
            get
            {
                FATEKAddressClass address = new FATEKAddressClass("0:M0031");
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = new FATEKAddressClass("0:M0031");
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }


        #region ROBOT HCFA CONTROL

        //private IntPtr handle = IntPtr.Zero;
        //private bool m_debug = false;
        JetEazy.ControlSpace.RobotSpace.RobotHCFA robotHCFA;
        RobotType robotType1 = RobotType.NONE;

        public void RobotParaSetup(JetEazy.ControlSpace.RobotSpace.RobotHCFA robot, RobotType robotType)
        {
            //handle = eHandle;
            //m_debug = eDebug;
            robotHCFA = robot;
            robotType1 = robotType;
        }
        public bool RobotEnable
        {
            get {

                switch (robotType1)
                {
                    case RobotType.NONE:
                        return false;
                }

                return robotHCFA.Motionenable; }
            set {

                switch (robotType1)
                {
                    case RobotType.NONE:
                        return;
                }


                robotHCFA.Motionenable = value; }
        }
        public bool RobotRunning
        {
            get {

                switch (robotType1)
                {
                    case RobotType.NONE:
                        return false;
                }

                return robotHCFA.RobotRunning; }
            //set { robotHCFA.RobotRunning = value; }
        }
        public string RobotState
        {
            get {

                switch (robotType1)
                {
                    case RobotType.NONE:
                        return string.Empty;
                }

                return robotHCFA.GetMotionState(); }
        }
        public int RobotErrorCode
        {
            get {

                switch (robotType1)
                {
                    case RobotType.NONE:
                        return 0;
                }

                return robotHCFA.ErrorCode; }
        }
        public string RobotErrorMessage
        {
            get {

                switch (robotType1)
                {
                    case RobotType.NONE:
                        return string.Empty;
                }

                return robotHCFA.GetErrorCode(); }
        }
        public double RobotSpeedValue
        {
            get {

                switch (robotType1)
                {
                    case RobotType.NONE:
                        return 0;
                }

                return robotHCFA.Speed; }
            set {

                switch (robotType1)
                {
                    case RobotType.NONE:
                        return;
                }

                robotHCFA.Speed = value; }
        }
        #endregion


    }
}
