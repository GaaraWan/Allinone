using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetEazy;
using JetEazy.ControlSpace;
using JetEazy.ControlSpace.MotionSpace;

namespace JetEazy.ControlSpace.MotionSpace
{
    public enum MotionAddressEnum : int
    {
        COUNT = 22,

        ADR_ISHOME = 0,
        ADR_ISONSITE = 1,
        ADR_ISREACHUPPERLIMIT = 2,
        ADR_ISREACHLOWERLIMIT = 3,

        ADR_GO = 4,
        ADR_HOME = 5,
        ADR_FORWARD = 6,
        ADR_BACKWARD = 7,

        ADR_GOSPEED = 8,
        ADR_MANUALSPEED = 9,

        ADR_HOMESLOWSPEED = 10,
        ADR_HOMEHIGHSPEED = 11,

        ADR_STEPPOSITIONNOW = 12,
        ADR_STEPPOSITIONSET = 13,

        ADR_RULERSTEPPOSITIONNOW = 14,

        ADR_ISSVON = 15,
        ADR_SVON = 16,

        ADR_ISBREAK = 17,
        ADR_BREAK = 18,

        ADR_ISERROR = 19,
        ADR_RESET = 20,
        ADR_ISREACHHOME = 21,

        ONEMMSTEP = 100,
        RULERONEMMSTEP = 101,
        MANUALSPEED = 102,
        MANUALSLOWSPEED = 103,
        GOSPEED = 104,
        GOSLOWSPEED = 105,
        HOMEHIGHSPEED = 106,
        HOMESLOWSPEED = 107,

        RATIO = 108,
        SOFTUPPERBOUND = 109,
        SOFTLOWERBOUND = 110,

        MOTIONTYPE = 111,
        READYPOSITION = 112,

        MOTIONALIAS = 113,

    }
    public class PLCMotionClass : GeoMotionClass
    {
        FatekPLCClass[] PLC;
        FATEKAddressClass[] ADDRESSARRAY = new FATEKAddressClass[(int)MotionAddressEnum.COUNT];

        public override bool IsHaveBreakOption
        {
            get
            {
                return ADDRESSARRAY[(int)MotionAddressEnum.ADR_BREAK].SiteNo != -1;
            }
        }
        public override bool IsHaveSVOnOption
        {
            get
            {
                return ADDRESSARRAY[(int)MotionAddressEnum.ADR_SVON].SiteNo != -1;
            }
        }
        public override bool IsHaveResetOption
        {
            get
            {
                return ADDRESSARRAY[(int)MotionAddressEnum.ADR_RESET].SiteNo != -1;
            }
        }

        public override bool IsHaveRulerOption
        {
            get
            {
                return ADDRESSARRAY[(int)MotionAddressEnum.ADR_RULERSTEPPOSITIONNOW].SiteNo != -1;
            }
        }


        public PLCMotionClass()
        {

        }

        public void Intial(string path, MotionEnum motionname, FatekPLCClass[] plc, bool isnousemotor)
        {
            PLC = plc;
            MOTIONNAME = motionname;

            IsNoUseMotor = isnousemotor;
            INIFILE = path + "\\Motion" + (int)MOTIONNAME + ".INI";

            LoadData();
        }

        public override void LoadData()
        {
            ADDRESSARRAY[(int)MotionAddressEnum.ADR_ISHOME] = new FATEKAddressClass(ReadINIValue("Status Address", MotionAddressEnum.ADR_ISHOME.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MotionAddressEnum.ADR_ISONSITE] = new FATEKAddressClass(ReadINIValue("Status Address", MotionAddressEnum.ADR_ISONSITE.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MotionAddressEnum.ADR_ISREACHUPPERLIMIT] = new FATEKAddressClass(ReadINIValue("Status Address", MotionAddressEnum.ADR_ISREACHUPPERLIMIT.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MotionAddressEnum.ADR_ISREACHLOWERLIMIT] = new FATEKAddressClass(ReadINIValue("Status Address", MotionAddressEnum.ADR_ISREACHLOWERLIMIT.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MotionAddressEnum.ADR_ISSVON] = new FATEKAddressClass(ReadINIValue("Status Address", MotionAddressEnum.ADR_ISSVON.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MotionAddressEnum.ADR_ISBREAK] = new FATEKAddressClass(ReadINIValue("Status Address", MotionAddressEnum.ADR_ISBREAK.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MotionAddressEnum.ADR_ISERROR] = new FATEKAddressClass(ReadINIValue("Status Address", MotionAddressEnum.ADR_ISERROR.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MotionAddressEnum.ADR_ISREACHHOME] = new FATEKAddressClass(ReadINIValue("Status Address", MotionAddressEnum.ADR_ISREACHHOME.ToString(), "", INIFILE));


            ADDRESSARRAY[(int)MotionAddressEnum.ADR_GO] = new FATEKAddressClass(ReadINIValue("Operation Address", MotionAddressEnum.ADR_GO.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MotionAddressEnum.ADR_HOME] = new FATEKAddressClass(ReadINIValue("Operation Address", MotionAddressEnum.ADR_HOME.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MotionAddressEnum.ADR_FORWARD] = new FATEKAddressClass(ReadINIValue("Operation Address", MotionAddressEnum.ADR_FORWARD.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MotionAddressEnum.ADR_BACKWARD] = new FATEKAddressClass(ReadINIValue("Operation Address", MotionAddressEnum.ADR_BACKWARD.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MotionAddressEnum.ADR_SVON] = new FATEKAddressClass(ReadINIValue("Operation Address", MotionAddressEnum.ADR_SVON.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MotionAddressEnum.ADR_BREAK] = new FATEKAddressClass(ReadINIValue("Operation Address", MotionAddressEnum.ADR_BREAK.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MotionAddressEnum.ADR_RESET] = new FATEKAddressClass(ReadINIValue("Operation Address", MotionAddressEnum.ADR_RESET.ToString(), "", INIFILE));

            ADDRESSARRAY[(int)MotionAddressEnum.ADR_GOSPEED] = new FATEKAddressClass(ReadINIValue("Data Address", MotionAddressEnum.ADR_GOSPEED.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MotionAddressEnum.ADR_MANUALSPEED] = new FATEKAddressClass(ReadINIValue("Data Address", MotionAddressEnum.ADR_MANUALSPEED.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MotionAddressEnum.ADR_HOMESLOWSPEED] = new FATEKAddressClass(ReadINIValue("Data Address", MotionAddressEnum.ADR_HOMESLOWSPEED.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MotionAddressEnum.ADR_HOMEHIGHSPEED] = new FATEKAddressClass(ReadINIValue("Data Address", MotionAddressEnum.ADR_HOMEHIGHSPEED.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MotionAddressEnum.ADR_STEPPOSITIONNOW] = new FATEKAddressClass(ReadINIValue("Data Address", MotionAddressEnum.ADR_STEPPOSITIONNOW.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MotionAddressEnum.ADR_STEPPOSITIONSET] = new FATEKAddressClass(ReadINIValue("Data Address", MotionAddressEnum.ADR_STEPPOSITIONSET.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MotionAddressEnum.ADR_RULERSTEPPOSITIONNOW] = new FATEKAddressClass(ReadINIValue("Data Address", MotionAddressEnum.ADR_RULERSTEPPOSITIONNOW.ToString(), "", INIFILE));

            MOTIONTYPE = (MotionTypeEnum)Enum.Parse(typeof(MotionTypeEnum), ReadINIValue("Parameters", MotionAddressEnum.MOTIONTYPE.ToString(), MOTIONTYPE.ToString(), INIFILE), false);
            ONEMMSTEP = int.Parse(ReadINIValue("Parameters", MotionAddressEnum.ONEMMSTEP.ToString(), ONEMMSTEP.ToString(), INIFILE));
            RULERONEMMSTEP = int.Parse(ReadINIValue("Parameters", MotionAddressEnum.RULERONEMMSTEP.ToString(), RULERONEMMSTEP.ToString(), INIFILE));
            MANUALSPEED = int.Parse(ReadINIValue("Parameters", MotionAddressEnum.MANUALSPEED.ToString(), MANUALSPEED.ToString(), INIFILE));
            MANUALSLOWSPEED = int.Parse(ReadINIValue("Parameters", MotionAddressEnum.MANUALSLOWSPEED.ToString(), MANUALSLOWSPEED.ToString(), INIFILE));
            GOSPEED = int.Parse(ReadINIValue("Parameters", MotionAddressEnum.GOSPEED.ToString(), GOSPEED.ToString(), INIFILE));
            GOSLOWSPEED = int.Parse(ReadINIValue("Parameters", MotionAddressEnum.GOSLOWSPEED.ToString(), GOSLOWSPEED.ToString(), INIFILE));
            HOMEHIGHSPEED = int.Parse(ReadINIValue("Parameters", MotionAddressEnum.HOMEHIGHSPEED.ToString(), HOMEHIGHSPEED.ToString(), INIFILE));
            HOMESLOWSPEED = int.Parse(ReadINIValue("Parameters", MotionAddressEnum.HOMESLOWSPEED.ToString(), HOMESLOWSPEED.ToString(), INIFILE));
            RATIO = int.Parse(ReadINIValue("Parameters", MotionAddressEnum.RATIO.ToString(), RATIO.ToString(), INIFILE));
            READYPOSITION = float.Parse(ReadINIValue("Parameters", MotionAddressEnum.READYPOSITION.ToString(), READYPOSITION.ToString(), INIFILE));
            SOFTUPPERBOUND = float.Parse(ReadINIValue("Parameters", MotionAddressEnum.SOFTUPPERBOUND.ToString(), SOFTUPPERBOUND.ToString(), INIFILE));
            SOFTLOWERBOUND = float.Parse(ReadINIValue("Parameters", MotionAddressEnum.SOFTLOWERBOUND.ToString(), SOFTLOWERBOUND.ToString(), INIFILE));

            MOTIONALIAS = ReadINIValue("Parameters", "MOTIONALIAS", MOTIONNAME.ToString(), INIFILE);

        }
        public override void SaveData()
        {
            WriteINIValue("Parameters", MotionAddressEnum.READYPOSITION.ToString(), READYPOSITION.ToString("0.000"), INIFILE);
        }
        public void SaveData(MotionAddressEnum eMotionAddress, string eValue)
        {
            WriteINIValue("Parameters", eMotionAddress.ToString(), eValue, INIFILE);
        }
        public override bool IsHome
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MotionAddressEnum.ADR_ISHOME];
                if (string.IsNullOrEmpty(address.Address0))
                    return false;
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {

            }
        }
        public override bool IsOnSite
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MotionAddressEnum.ADR_ISONSITE];
                if (string.IsNullOrEmpty(address.Address0))
                    return false;
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public override bool IsSVOn
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MotionAddressEnum.ADR_ISSVON];
                if (string.IsNullOrEmpty(address.Address0))
                    return false;
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public override bool IsBreack
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MotionAddressEnum.ADR_ISBREAK];
                if (string.IsNullOrEmpty(address.Address0))
                    return false;
                return !PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public override bool IsError
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MotionAddressEnum.ADR_ISERROR];
                if (string.IsNullOrEmpty(address.Address0))
                    return false;
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public override string PositionNowString
        {
            get
            {
                return PositionNow.ToString("0.000");
            }
        }
        public override float PositionNow
        {
            get
            {
                if (MotionMMMode == 1)
                {
                    FATEKAddressClass address = ADDRESSARRAY[(int)MotionAddressEnum.ADR_STEPPOSITIONNOW];

                    if (address.SiteNo == -1)
                        return 0f;

                    short setH = PLC[address.SiteNo].IOData.GetMW(address.Address0);
                    short setL = PLC[address.SiteNo].IOData.GetMW(address.Address1);

                    //接收plc数据
                    byte[] bytesH = BitConverter.GetBytes(setH);
                    byte[] bytesL = BitConverter.GetBytes(setL);

                    byte[] d = new byte[bytesH.Length + bytesL.Length];
                    Array.Copy(bytesH, 0, d, 0, bytesH.Length);
                    Array.Copy(bytesL, 0, d, bytesH.Length, bytesL.Length);

                    float myFloat = BitConverter.ToSingle(d, 0);
                    return myFloat;
                }

                return (float)StepPositionNow / (float)ONEMMSTEP;
            }
        }
        public override string RulerPositionNowString
        {
            get
            {
                return RulerPositionNow.ToString("0.000");
            }
        }
        public override float RulerPositionNow
        {
            get
            {
                if (MotionMMMode == 1)
                {
                    FATEKAddressClass address = ADDRESSARRAY[(int)MotionAddressEnum.ADR_RULERSTEPPOSITIONNOW];

                    if (address.SiteNo == -1)
                        return 0f;

                    short setH = PLC[address.SiteNo].IOData.GetMW(address.Address0);
                    short setL = PLC[address.SiteNo].IOData.GetMW(address.Address1);

                    //接收plc数据
                    byte[] bytesH = BitConverter.GetBytes(setH);
                    byte[] bytesL = BitConverter.GetBytes(setL);

                    byte[] d = new byte[bytesH.Length + bytesL.Length];
                    Array.Copy(bytesH, 0, d, 0, bytesH.Length);
                    Array.Copy(bytesL, 0, d, bytesH.Length, bytesL.Length);

                    float myFloat = BitConverter.ToSingle(d, 0);
                    return myFloat;
                }

                if (RULERONEMMSTEP == 0)
                    return 0;
                else
                    return (float)RulerStepPositionNow / (float)RULERONEMMSTEP;
            }
        }
        public override int StepPositionNow
        {
            get
            {
                if (IsNoUseMotor)
                {
                    return SIMPositionStep;
                }

                //ulong ret = 0;
                Int32 ret = 0;

                FATEKAddressClass address = ADDRESSARRAY[(int)MotionAddressEnum.ADR_STEPPOSITIONNOW];

                //ret = PLC[address.SiteNo].IOData.GetData(address.Address1);
                //ret += (ret << 16) + PLC[address.SiteNo].IOData.GetData(address.Address0);
                //if (address.SiteNo == -1)
                //    return ret;

                ret = HEXSigned32(ValueToHEX(PLC[address.SiteNo].IOData.GetData(address.Address1), 4) + ValueToHEX(PLC[address.SiteNo].IOData.GetData(address.Address0), 4));

                return (int)ret;
            }
        }
        public override int StepPositionSet
        {
            set
            {
                if (IsNoUseMotor)
                {
                    SIMPositionStep = value;
                    return;
                }

                FATEKAddressClass address = ADDRESSARRAY[(int)MotionAddressEnum.ADR_STEPPOSITIONSET];

                long setH = value >> 16;
                long setL = value % 65536;

                //PLC[address.SiteNo].SetData(address.Address0, ValueToHEX(setL, 4));
                //PLC[address.SiteNo].SetData(address.Address1, ValueToHEX(setH, 4));
                PLC[address.SiteNo].SetData(ValueToHEX(setL, 4), address.Address0);
                PLC[address.SiteNo].SetData(ValueToHEX(setH, 4), address.Address1);
            }
        }
        public float StepPositionSetfloat
        {
            set
            {
                if (IsNoUseMotor)
                {
                    SIMPositionStep = (int)value;
                    return;
                }

                FATEKAddressClass address = ADDRESSARRAY[(int)MotionAddressEnum.ADR_STEPPOSITIONSET];

                //发送给plc数据
                byte[] hex = BitConverter.GetBytes(value);
                byte[] h = new byte[2];
                byte[] l = new byte[2];

                h[0] = hex[0];
                h[1] = hex[1];
                l[0] = hex[2];
                l[1] = hex[3];

                ushort setH = BitConverter.ToUInt16(h, 0);
                ushort setL = BitConverter.ToUInt16(l, 0);

                PLC[address.SiteNo].SetData_ushort(setH, address.Address0);
                PLC[address.SiteNo].SetData_ushort(setL, address.Address1);
            }
        }
        public override int RulerStepPositionNow
        {
            get
            {
                Int32 ret = 0;

                FATEKAddressClass address = ADDRESSARRAY[(int)MotionAddressEnum.ADR_RULERSTEPPOSITIONNOW];

                //ret += PLC[address.SiteNo].IOData.GetData(address.Address1);
                //ret += (ret << 16) + PLC[address.SiteNo].IOData.GetData(address.Address0);

                ret = HEXSigned32(ValueToHEX(PLC[address.SiteNo].IOData.GetData(address.Address1), 4) + ValueToHEX(PLC[address.SiteNo].IOData.GetData(address.Address0), 4));

                return (int)ret;
            }
        }
        public override bool IsReachHomeBound
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MotionAddressEnum.ADR_ISREACHHOME];
                if (string.IsNullOrEmpty(address.Address0))
                    return false;
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public override bool IsReachUpperBound
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MotionAddressEnum.ADR_ISREACHUPPERLIMIT];
                if (string.IsNullOrEmpty(address.Address0))
                    return false;
                return !PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public override bool IsReachLowerBound
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MotionAddressEnum.ADR_ISREACHLOWERLIMIT];
                if (string.IsNullOrEmpty(address.Address0))
                    return false;
                return !PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public override void Go(float position)
        {
            switch (MotionMMMode)
            {
                case 1:
                    StepPositionSetfloat = position;
                    break;
                default:

                    //@LETIAN: 原先的寫法會有數值誤差!!!
                    //   案例 position = 36.35 會被處理成 3634 (36.34) 
                    //old code >>> int setposition = (int)(position * (float)ONEMMSTEP);
                    int setposition = (int)Math.Round((double)position * ONEMMSTEP);
                    StepPositionSet = setposition;

                    break;
            }

            //int setposition = (int)(position * (float)ONEMMSTEP);

            //StepPositionSet = setposition;

            FATEKAddressClass address = ADDRESSARRAY[(int)MotionAddressEnum.ADR_GO];
            if (address.SiteNo == -1)
                return;
            PLC[address.SiteNo].SetIO(true, address.Address0);

            //Task task = new Task(new Action(() =>
            //{
            //    FATEKAddressClass address1 = ADDRESSARRAY[(int)MotionAddressEnum.ADR_GO];
            //    System.Threading.Thread.Sleep(500);
            //    //AddressClass address = ADDRESSARRAY[(int)MotionAddressEnum.ADR_GO];
            //    PLC[address1.SiteNo].SetIO(false, address1.Address0);
            //}));
            //task.Start();
        }
        public override void Home()
        {
            if (IsNoUseMotor)
            {
                SIMPositionStep = 0;
                return;
            }


            FATEKAddressClass address = ADDRESSARRAY[(int)MotionAddressEnum.ADR_HOME];
            PLC[address.SiteNo].SetIO(true, address.Address0);
        }
        public override void SVOn()
        {
            FATEKAddressClass address = ADDRESSARRAY[(int)MotionAddressEnum.ADR_SVON];

            if (address.SiteNo != -1)
                PLC[address.SiteNo].SetIO(true, address.Address0);
        }
        public override void Reset()
        {
            FATEKAddressClass address = ADDRESSARRAY[(int)MotionAddressEnum.ADR_RESET];

            if (address.SiteNo != -1)
                PLC[address.SiteNo].SetIO(true, address.Address0);
        }
        public override void Break()
        {
            FATEKAddressClass address = ADDRESSARRAY[(int)MotionAddressEnum.ADR_BREAK];

            if (address.SiteNo != -1)
                PLC[address.SiteNo].SetIO(!IsBreack, address.Address0);
        }
        public override void Forward()
        {
            if (IsNoUseMotor)
            {
                SIMPositionStep++;
                return;
            }

            FATEKAddressClass forwardaddress = ADDRESSARRAY[(int)MotionAddressEnum.ADR_FORWARD];
            PLC[forwardaddress.SiteNo].SetIO(true, forwardaddress.Address0);

            FATEKAddressClass backwardaddress = ADDRESSARRAY[(int)MotionAddressEnum.ADR_BACKWARD];
            PLC[backwardaddress.SiteNo].SetIO(false, backwardaddress.Address0);
        }
        public override void Backward()
        {
            if (IsNoUseMotor)
            {
                SIMPositionStep--;
                return;
            }

            FATEKAddressClass forwardaddress = ADDRESSARRAY[(int)MotionAddressEnum.ADR_FORWARD];
            PLC[forwardaddress.SiteNo].SetIO(false, forwardaddress.Address0);

            FATEKAddressClass backwardaddress = ADDRESSARRAY[(int)MotionAddressEnum.ADR_BACKWARD];
            PLC[backwardaddress.SiteNo].SetIO(true, backwardaddress.Address0);

        }
        public override void Stop()
        {
            FATEKAddressClass forwardaddress = ADDRESSARRAY[(int)MotionAddressEnum.ADR_FORWARD];
            PLC[forwardaddress.SiteNo].SetIO(false, forwardaddress.Address0);

            FATEKAddressClass backwardaddress = ADDRESSARRAY[(int)MotionAddressEnum.ADR_BACKWARD];
            PLC[backwardaddress.SiteNo].SetIO(false, backwardaddress.Address0);
        }
        public override void SetSpeed(SpeedTypeEnum speedtype)
        {
            MotionAddressEnum motionaddress = MotionAddressEnum.ADR_HOMEHIGHSPEED;
            long speedvalue = 0;

            switch (speedtype)
            {
                case SpeedTypeEnum.HOMEHIGH:
                    motionaddress = MotionAddressEnum.ADR_HOMEHIGHSPEED;
                    speedvalue = HOMEHIGHSPEED;
                    break;
                case SpeedTypeEnum.HOMESLOW:
                    motionaddress = MotionAddressEnum.ADR_HOMESLOWSPEED;
                    speedvalue = HOMESLOWSPEED;
                    break;
                case SpeedTypeEnum.MANUALSLOW:
                    motionaddress = MotionAddressEnum.ADR_MANUALSPEED;
                    speedvalue = MANUALSLOWSPEED;
                    break;
                case SpeedTypeEnum.MANUAL:
                    motionaddress = MotionAddressEnum.ADR_MANUALSPEED;
                    speedvalue = MANUALSPEED;
                    break;
                case SpeedTypeEnum.GOSLOW:
                    motionaddress = MotionAddressEnum.ADR_GOSPEED;
                    speedvalue = GOSLOWSPEED;
                    break;
                case SpeedTypeEnum.GO:
                    motionaddress = MotionAddressEnum.ADR_GOSPEED;
                    speedvalue = GOSPEED;
                    break;
            }

            if (IsNoUseMotor)
            {
                SIMSpeed = (int)speedvalue;
                return;
            }

            FATEKAddressClass address = ADDRESSARRAY[(int)motionaddress];
            if (address.SiteNo == -1)
                return;

            //if (MotionMMMode == 1)
            //{
            //    float speedvaluetmp = (float)speedvalue;
            //    //发送给plc数据
            //    byte[] hex = BitConverter.GetBytes(speedvaluetmp);
            //    byte[] h = new byte[2];
            //    byte[] l = new byte[2];

            //    h[0] = hex[0];
            //    h[1] = hex[1];
            //    l[0] = hex[2];
            //    l[1] = hex[3];

            //    ushort setHF = BitConverter.ToUInt16(h, 0);
            //    ushort setLF = BitConverter.ToUInt16(l, 0);

            //    if (!string.IsNullOrEmpty(address.Address0))
            //        PLC[address.SiteNo].SetData_ushort(setHF, address.Address0);
            //    if (!string.IsNullOrEmpty(address.Address1))
            //        PLC[address.SiteNo].SetData_ushort(setLF, address.Address1);

            //    return;
            //}

            long setH = speedvalue >> 16;
            long setL = speedvalue % 65536;
           
            PLC[address.SiteNo].SetData(ValueToHEX(setL, 4), address.Address0);
            PLC[address.SiteNo].SetData(ValueToHEX(setH, 4), address.Address1);
        }
        public override long GetSpeed(SpeedTypeEnum speedtype)
        {
            MotionAddressEnum motionaddress = MotionAddressEnum.ADR_HOMEHIGHSPEED;
            long speedvalue = 0;

            switch (speedtype)
            {
                case SpeedTypeEnum.HOMEHIGH:
                    motionaddress = MotionAddressEnum.ADR_HOMEHIGHSPEED;
                    speedvalue = HOMEHIGHSPEED;
                    break;
                case SpeedTypeEnum.HOMESLOW:
                    motionaddress = MotionAddressEnum.ADR_HOMESLOWSPEED;
                    speedvalue = HOMESLOWSPEED;
                    break;
                case SpeedTypeEnum.MANUALSLOW:
                    motionaddress = MotionAddressEnum.ADR_MANUALSPEED;
                    speedvalue = MANUALSLOWSPEED;
                    break;
                case SpeedTypeEnum.MANUAL:
                    motionaddress = MotionAddressEnum.ADR_MANUALSPEED;
                    speedvalue = MANUALSPEED;
                    break;
                case SpeedTypeEnum.GOSLOW:
                    motionaddress = MotionAddressEnum.ADR_GOSPEED;
                    speedvalue = GOSLOWSPEED;
                    break;
                case SpeedTypeEnum.GO:
                    motionaddress = MotionAddressEnum.ADR_GOSPEED;
                    speedvalue = GOSPEED;
                    break;
            }

            if (IsNoUseMotor)
            {
                SIMSpeed = (int)speedvalue;
                return SIMSpeed;
            }

            //long setH = speedvalue >> 16;
            //long setL = speedvalue % 65536;

            //FATEKAddressClass address = ADDRESSARRAY[(int)motionaddress];

            //PLC[address.SiteNo].SetData(ValueToHEX(setL, 4), address.Address0);
            //PLC[address.SiteNo].SetData(ValueToHEX(setH, 4), address.Address1);

            FATEKAddressClass address = ADDRESSARRAY[(int)motionaddress];
            if (address.SiteNo == -1)
                return speedvalue;
            //if (MotionMMMode == 1)
            //{

            //    short setH = PLC[address.SiteNo].IOData.GetMW(address.Address0);
            //    short setL = PLC[address.SiteNo].IOData.GetMW(address.Address1);

            //    //接收plc数据
            //    byte[] bytesH = BitConverter.GetBytes(setH);
            //    byte[] bytesL = BitConverter.GetBytes(setL);

            //    byte[] d = new byte[bytesH.Length + bytesL.Length];
            //    Array.Copy(bytesH, 0, d, 0, bytesH.Length);
            //    Array.Copy(bytesL, 0, d, bytesH.Length, bytesL.Length);

            //    float myFloat = BitConverter.ToSingle(d, 0);
            //    return (long)myFloat;
            //}
            
            if (!string.IsNullOrEmpty(address.Address1))
                speedvalue = HEXSigned32(ValueToHEX(PLC[address.SiteNo].IOData.GetData(address.Address1), 4) +
                                                                  ValueToHEX(PLC[address.SiteNo].IOData.GetData(address.Address0), 4));
            else
                speedvalue = HEXSigned32(ValueToHEX(PLC[address.SiteNo].IOData.GetData(address.Address0), 4));

            return speedvalue;
        }

    }
}

