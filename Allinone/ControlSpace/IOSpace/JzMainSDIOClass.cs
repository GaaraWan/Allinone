
using JetEazy;
using JetEazy.ControlSpace;
//using JetEazy.ControlSpace.PLCSpace;


namespace Allinone.ControlSpace.IOSpace
{
    public enum MainSDAddressEnum : int
    {
        COUNT = 16,
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

        ADR_ISEMC=5,
        ADR_RESET=6,
        ADR_PAUSE=7,
        ADR_STOP=8,
        ADR_BUZZER=9,
        ADR_DOOR=10,
        ADR_RUNMODE=11,
        ADR_FEEDVACC=12,
        ADR_TAKEVACC=13,
        ADR_START = 14,
        ADR_LIGHT=15,

    }
    public enum MainSDAlarmsEnum : int
    {
        ALARMSCOUNT = 3,
        ALARMS_ADR_COMMON = 0,
        ALARMS_ADR_SERIOUS0 = 1,
        ALARMS_ADR_SERIOUS1 = 2,
    }

    public class JzMainSDIOClass : GeoIOClass
    {

        public PLCAlarmsClass[] PLCALARMS;

        #region 用类代表五个区

        /// <summary>
        /// 供料单流程
        /// </summary>
        public MainSDSingleProcess SingleFEED = null;
        /// <summary>
        /// 测试区单左吸流程
        /// </summary>
        public MainSDSingleProcess SingleTESTLEFT = null;
        /// <summary>
        /// 测试区单右吸流程
        /// </summary>
        public MainSDSingleProcess SingleTESTRIGHT = null;
        /// <summary>
        /// 测试区左右吸流程
        /// </summary>
        public MainSDSingleProcess SingleTESTLEFTRIGHT = null;
        /// <summary>
        /// 放料区流程
        /// </summary>
        public MainSDSingleProcess SingleTAKE = null;
        /// <summary>
        /// 复位流程
        /// </summary>
        public MainSDSingleProcess SingleRESET = null;

        #endregion

        public JzMainSDIOClass()
        {   

        }
        public void Initial(string path, OptionEnum option, FatekPLCClass[] plc)
        {
            ADDRESSARRAY = new FATEKAddressClass[(int)MainSDAddressEnum.COUNT];
            PLCALARMS = new PLCAlarmsClass[(int)MainSDAlarmsEnum.ALARMSCOUNT];

            PLC = plc;
            //PLC = plc;

            INIFILE = path + "\\IO.INI";

            LoadData();

            SingleFEED = new MainSDSingleProcess("供料", PLC[0], 51);
            //SingleFEED.ADR_IsComplete = 87;
            //SingleFEED.ADR_IsRunniing = 88;
            SingleFEED.ADR_Stop = 83;



            SingleTESTLEFT = new MainSDSingleProcess("测试单边左吸", PLC[0], 54);
            SingleTESTLEFT.ADR_IsRunning = 55;
            SingleTESTLEFT.ADR_IsComplete = 56;
            SingleTESTLEFT.ADR_Stop = 84;
            SingleTESTLEFT.ADR_Mode = 65;
            SingleTESTLEFT.IsUseMode = true;

            SingleTESTRIGHT = new MainSDSingleProcess("测试单边右吸", PLC[0], 54);
            SingleTESTRIGHT.ADR_IsRunning = 55;
            SingleTESTRIGHT.ADR_IsComplete = 56;
            SingleTESTRIGHT.ADR_Stop = 84;
            SingleTESTRIGHT.ADR_Mode = 66;
            SingleTESTRIGHT.IsUseMode = true;

            SingleTESTLEFTRIGHT = new MainSDSingleProcess("测试双边吸", PLC[0], 54);
            SingleTESTLEFTRIGHT.ADR_IsRunning = 55;
            SingleTESTLEFTRIGHT.ADR_IsComplete = 56;
            SingleTESTLEFTRIGHT.ADR_Stop = 84;
            SingleTESTLEFTRIGHT.ADR_Mode = 67;
            SingleTESTLEFTRIGHT.IsUseMode = true;



            SingleTAKE = new MainSDSingleProcess("放料", PLC[0], 60);
            SingleTAKE.ADR_Stop = 86;
            SingleTAKE.ADR_IsComplete = 63;
            SingleRESET = new MainSDSingleProcess("复位", PLC[0], 80);

        }
        public override void LoadData()
        {
            ADDRESSARRAY[(int)MainSDAddressEnum.ADR_ISSTART] = new FATEKAddressClass(ReadINIValue("Status Address", MainSDAddressEnum.ADR_ISSTART.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainSDAddressEnum.ADR_ISUPSERROR] = new FATEKAddressClass(ReadINIValue("Status Address", MainSDAddressEnum.ADR_ISUPSERROR.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainSDAddressEnum.ADR_ISEMC] = new FATEKAddressClass(ReadINIValue("Status Address", "ADR_ISEMC", "", INIFILE));


            ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED] = new FATEKAddressClass(ReadINIValue("Operation Address", MainSDAddressEnum.ADR_RED.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainSDAddressEnum.ADR_YELLOW] = new FATEKAddressClass(ReadINIValue("Operation Address", MainSDAddressEnum.ADR_YELLOW.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)MainSDAddressEnum.ADR_GREEN] = new FATEKAddressClass(ReadINIValue("Operation Address", MainSDAddressEnum.ADR_GREEN.ToString(), "", INIFILE));

           
            ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RESET] = new FATEKAddressClass(ReadINIValue("Operation Address", "ADR_RESET", "", INIFILE));
            ADDRESSARRAY[(int)MainSDAddressEnum.ADR_PAUSE] = new FATEKAddressClass(ReadINIValue("Operation Address", "ADR_PAUSE", "", INIFILE));
            ADDRESSARRAY[(int)MainSDAddressEnum.ADR_STOP] = new FATEKAddressClass(ReadINIValue("Operation Address", "ADR_STOP", "", INIFILE));
            ADDRESSARRAY[(int)MainSDAddressEnum.ADR_BUZZER] = new FATEKAddressClass(ReadINIValue("Operation Address", "ADR_BUZZER", "", INIFILE));
            ADDRESSARRAY[(int)MainSDAddressEnum.ADR_DOOR] = new FATEKAddressClass(ReadINIValue("Operation Address", "ADR_DOOR", "", INIFILE));
            ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RUNMODE] = new FATEKAddressClass(ReadINIValue("Operation Address", "ADR_RUNMODE", "", INIFILE));
            ADDRESSARRAY[(int)MainSDAddressEnum.ADR_FEEDVACC] = new FATEKAddressClass(ReadINIValue("Operation Address", "ADR_FEEDVACC", "", INIFILE));
            ADDRESSARRAY[(int)MainSDAddressEnum.ADR_TAKEVACC] = new FATEKAddressClass(ReadINIValue("Operation Address", "ADR_TAKEVACC", "", INIFILE));
            ADDRESSARRAY[(int)MainSDAddressEnum.ADR_START] = new FATEKAddressClass(ReadINIValue("Operation Address", "ADR_START", "", INIFILE));
            ADDRESSARRAY[(int)MainSDAddressEnum.ADR_LIGHT] = new FATEKAddressClass(ReadINIValue("Operation Address", "ADR_LIGHT", "", INIFILE));

            int iindex = 0;
            string str = "";
            //string str_alarms = ReadINIValue("Parameters", "ALARMS_ADR_SERIOUS", "", INIFILE.Replace("IO.INI", "ALARMIO0.INI"));
            PLCALARMS[(int)MainSDAlarmsEnum.ALARMS_ADR_SERIOUS0] = new PLCAlarmsClass(ReadINIValue("Parameters", MainSDAlarmsEnum.ALARMS_ADR_SERIOUS0.ToString(), "", INIFILE.Replace("IO.INI", "ALARMIO0.INI")));
            foreach (PLCAlarmsItemClass item in PLCALARMS[(int)MainSDAlarmsEnum.ALARMS_ADR_SERIOUS0].PLCALARMSLIST)
            {
                iindex = 0;
                while (iindex < 16)
                {
                    str = ReadINIValue(item.ADR_Address, "Bit " + iindex.ToString(), "", INIFILE.Replace("IO.INI", "ALARMIO0.INI"));
                    PLCALARMS[(int)MainSDAlarmsEnum.ALARMS_ADR_SERIOUS0].PLCAlarmsAddDescription("0," + item.CovertToNormalAddress(iindex) + "," + str);
                    iindex++;
                }
            }

            PLCALARMS[(int)MainSDAlarmsEnum.ALARMS_ADR_SERIOUS1] = new PLCAlarmsClass(ReadINIValue("Parameters", MainSDAlarmsEnum.ALARMS_ADR_SERIOUS1.ToString(), "", INIFILE.Replace("IO.INI", "ALARMIO0.INI")));
            foreach (PLCAlarmsItemClass item in PLCALARMS[(int)MainSDAlarmsEnum.ALARMS_ADR_SERIOUS1].PLCALARMSLIST)
            {
                iindex = 0;
                while (iindex < 16)
                {
                    str = ReadINIValue(item.ADR_Address, "Bit " + iindex.ToString(), "", INIFILE.Replace("IO.INI", "ALARMIO0.INI"));
                    PLCALARMS[(int)MainSDAlarmsEnum.ALARMS_ADR_SERIOUS1].PLCAlarmsAddDescription("0," + item.CovertToNormalAddress(iindex) + "," + str);
                    iindex++;
                }
            }

            PLCALARMS[(int)MainSDAlarmsEnum.ALARMS_ADR_COMMON] = new PLCAlarmsClass(ReadINIValue("Parameters", MainSDAlarmsEnum.ALARMS_ADR_COMMON.ToString(), "", INIFILE.Replace("IO.INI", "ALARMIO0.INI")));
            foreach (PLCAlarmsItemClass item in PLCALARMS[(int)MainSDAlarmsEnum.ALARMS_ADR_COMMON].PLCALARMSLIST)
            {
                iindex = 0;
                while (iindex < 16)
                {
                    str = ReadINIValue(item.ADR_Address, "Bit " + iindex.ToString(), "", INIFILE.Replace("IO.INI", "ALARMIO0.INI"));
                    PLCALARMS[(int)MainSDAlarmsEnum.ALARMS_ADR_COMMON].PLCAlarmsAddDescription("0," + item.CovertToNormalAddress(iindex) + "," + str);
                    iindex++;
                }
            }

        }

        public override void SaveData()
        {
            
        }
        public bool ADR_START
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_START];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_START];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool ADR_TAKEVACC
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_TAKEVACC];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_TAKEVACC];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool ADR_FEEDVACC
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_FEEDVACC];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_FEEDVACC];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool ADR_RUNMODE
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RUNMODE];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RUNMODE];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool ADR_DOOR
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_DOOR];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_DOOR];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool ADR_BUZZER
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_BUZZER];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_BUZZER];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool ADR_STOP
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_STOP];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_STOP];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool ADR_PAUSE
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_PAUSE];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_PAUSE];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool ADR_RESET
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RESET];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RESET];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool ADR_LIGHT
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_LIGHT];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_LIGHT];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }

        public bool ADR_ISEMC
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_ISEMC];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            //set
            //{
            //    FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_ISEMC];
            //    PLC[address.SiteNo].SetIO(value, address.Address0);
            //}
        }


        public bool Red
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool Yellow
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_YELLOW];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_YELLOW];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool Green
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_GREEN];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_GREEN];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool IsStart
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_ISSTART];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool IsUPSError
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_ISUPSERROR];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }

        protected int ONEMMSTEP = 1;
        //protected int ONEMMSTEP = 625;

        public void SetValue(float ivalue, string ioaddr0, string ioaddr1)
        {
            int iValue = (int)(ivalue * (float)ONEMMSTEP);
            long setH = iValue >> 16;
            long setL = iValue % 65536;

            PLC[0].SetData(ValueToHEX(setL, 4), ioaddr0);
            //if (!string.IsNullOrEmpty(ioaddr1))
            PLC[0].SetData(ValueToHEX(setH, 4), ioaddr1);
        }
        public void SetValue(int ivalue, string ioaddr0, string ioaddr1)
        {
            long setH = ivalue >> 16;
            long setL = ivalue % 65536;

            PLC[0].SetData(ValueToHEX(setL, 4), ioaddr0);
            //if (!string.IsNullOrEmpty(ioaddr1))
            PLC[0].SetData(ValueToHEX(setH, 4), ioaddr1);
        }

        #region 特殊的设置点位

        /// <summary>
        /// 写plc参数 写on 写完off
        /// </summary>
        public bool PLCWriteParas
        {
            get
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                return PLC[0].IOData.GetBit("M0010");
            }
            set
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                PLC[0].SetIO(value, "M0010");
            }
        }
        /// <summary>
        /// pc取像完成中继信号
        /// </summary>
        public bool PCGetImageComplete
        {
            get
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                return PLC[0].IOData.GetBit("M0064");
            }
            set
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                PLC[0].SetIO(value, "M0064");
            }
        }


        /// <summary>
        /// 供料区有料
        /// </summary>
        public bool FEED_IsHaveProduct
        {
            get
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                return PLC[0].IOData.GetBit("M0013") || PLC[0].IOData.GetBit("M0015");
            }
            //set
            //{
            //    //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
            //    PLC[0].SetIO(value, "M0091");
            //}
        }
        /// <summary>
        /// PASS区有料
        /// </summary>
        public bool TAKEPASS_IsHaveProduct
        {
            get
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                return PLC[0].IOData.GetBit("M0017");
            }
            //set
            //{
            //    //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
            //    PLC[0].SetIO(value, "M0091");
            //}
        }
        /// <summary>
        /// NG区有料
        /// </summary>
        public bool TAKENG_IsHaveProduct
        {
            get
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                return PLC[0].IOData.GetBit("M0019");
            }
            //set
            //{
            //    //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
            //    PLC[0].SetIO(value, "M0091");
            //}
        }


        /// <summary>
        /// PASS区有料上位
        /// </summary>
        public bool TAKEPASS_IsHaveProductUp
        {
            get
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                return PLC[0].IOData.GetBit("M0018");
            }
            //set
            //{
            //    //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
            //    PLC[0].SetIO(value, "M0091");
            //}
        }
        /// <summary>
        /// NG区有料上位
        /// </summary>
        public bool TAKENG_IsHaveProductUp
        {
            get
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                return PLC[0].IOData.GetBit("M0020");
            }
            //set
            //{
            //    //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
            //    PLC[0].SetIO(value, "M0091");
            //}
        }

        /// <summary>
        /// 供料区有料完成
        /// </summary>
        public bool FEED_IsHaveComplete
        {
            get
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                return PLC[0].IOData.GetBit("M0087");
            }
            //set
            //{
            //    //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
            //    PLC[0].SetIO(value, "M0091");
            //}
        }
        /// <summary>
        /// 供料Y移动中
        /// </summary>
        public bool FEED_YMoving
        {
            get
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                return PLC[0].IOData.GetBit("M0088");
            }
            //set
            //{
            //    //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
            //    PLC[0].SetIO(value, "M0091");
            //}
        }
        /// <summary>
        /// 放料Y移动中
        /// </summary>
        public bool TAKE_YMoving
        {
            get
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                return PLC[0].IOData.GetBit("M0089");
            }
            //set
            //{
            //    //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
            //    PLC[0].SetIO(value, "M0091");
            //}
        }

        /// <summary>
        /// PASS区
        /// </summary>
        public bool TAKE_PASS
        {
            get
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                return PLC[0].IOData.GetBit("M0090");
            }
            set
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                PLC[0].SetIO(value, "M0090");
            }
        }
        /// <summary>
        /// NG区
        /// </summary>
        public bool TAKE_NG
        {
            get
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                return PLC[0].IOData.GetBit("M0091");
            }
            set
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                PLC[0].SetIO(value, "M0091");
            }
        }

        /// <summary>
        /// 放料定位完成信号 为了清除BUFF区 需要ON 一下
        /// </summary>
        public bool TAKE_COMPLETE_TOPLC
        {
            get
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                return PLC[0].IOData.GetBit("M0077");
            }
            set
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                PLC[0].SetIO(value, "M0077");
            }
        }

        /// <summary>
        /// 等离子吹气
        /// </summary>
        public bool ADR_BLOW
        {
            get
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                return PLC[0].IOData.GetBit("M0097");
            }
            set
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                PLC[0].SetIO(value, "M0097");
            }
        }

        #endregion

        #region ALARM
        public bool IsAlarmsSerious
        {
            get
            {
                return PLC[0].IOData.GetBit("M0260");
            }
        }
        public bool IsAlarmsCommon
        {
            get
            {
                return PLC[0].IOData.GetBit("M0261");
            }
        }

        public bool CLEARALARMS
        {
            get
            {
                return PLC[0].IOData.GetBit("M0262");
            }
            set
            {
                PLC[0].SetIO(value, "M0262");
            }
        }

        #region 载入载出中

        
        public bool LOAD_ONE_START
        {
            get
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                return PLC[0].IOData.GetBit("M0310");
            }
            set
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                PLC[0].SetIO(value, "M0310");
            }
        }
        public bool LOAD_ONE_END
        {
            get
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                return PLC[0].IOData.GetBit("M0312");
            }
            set
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                PLC[0].SetIO(value, "M0312");
            }
        }
        public bool UNLOAD_ONE_START
        {
            get
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                return PLC[0].IOData.GetBit("M0068");
            }
            set
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                PLC[0].SetIO(value, "M0068");
            }
        }
        public bool UNLOAD_ONE_END
        {
            get
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                return PLC[0].IOData.GetBit("M0070");
            }
            set
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                PLC[0].SetIO(value, "M0070");
            }
        }

        #endregion

        public bool AlarmCommonResetStart
        {
            get
            {
                return PLC[0].IOData.GetBit("M0057");
            }
            set
            {
                PLC[0].SetIO(value, "M0057");
            }
        }
        public bool AlarmCommonReseting
        {
            get
            {
                return PLC[0].IOData.GetBit("M0058");
            }
            //set
            //{
            //    PLC[0].SetIO(value, "M0058");
            //}
        }
        public bool AlarmCommonResetComplete
        {
            get
            {
                return PLC[0].IOData.GetBit("M0059");
            }
            //set
            //{
            //    PLC[0].SetIO(value, "M0059");
            //}
        }

        public bool GetAlarmsAddress(int iSiteNo, string strAddress)
        {
            return PLC[iSiteNo].IOData.GetBit(strAddress);
        }
        #endregion


        #region 测试一些点位

        /// <summary>
        /// 下压等待拍照完成 回
        /// </summary>
        public bool TAKE_PLC_GETIMAGEONOFF
        {
            get
            {
                return PLC[0].IOData.GetBit("M0063");
            }
            set
            {
                PLC[0].SetIO(value, "M0063");
            }
        }

        #endregion

        /// <summary>
        /// 读取点位
        /// </summary>
        /// <param name="eName">定义名称</param>
        /// <param name="eBit">M点</param>
        /// <returns></returns>
        public bool IsGetBit(string eName,string eBit)
        {
            //get
            //{
            //    return PLC[0].IOData.GetBit("M0261");
            //}

            return PLC[0].IOData.GetBit(eBit);

        }


    }


    public class MainSDSingleProcess
    {
        FatekPLCClass m_plc = null;
        int iM_BaseIndex = 0;
        private string name = "";

        /// <summary>
        /// plc流程中地址
        /// </summary>
        private int m_running = 0;
        /// <summary>
        /// plc流程完成地址
        /// </summary>
        private int m_complete = 0;
        /// <summary>
        /// plc流程停止地址
        /// </summary>
        private int m_stop = 0;

        private int m_mode = 0;
        private bool m_IsUseMode = false;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        //public MainSDSingleProcess()
        //{

        //}
        public MainSDSingleProcess(string eNAME, FatekPLCClass ePLC, int eMBaseIndex)
        {
            name = eNAME;
            m_plc = ePLC;
            iM_BaseIndex = eMBaseIndex;

            m_running = iM_BaseIndex + 1;
            m_complete = iM_BaseIndex + 2;
        }

        //private bool m_start = false;
        //private bool m_running = false;
        //private bool m_complete = false;

        public bool Start
        {
            get
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                if (m_IsUseMode)
                    return m_plc.IOData.GetBit("M" + iM_BaseIndex.ToString("0000")) && m_plc.IOData.GetBit("M" + m_mode.ToString("0000"));
                else
                    return m_plc.IOData.GetBit("M" + iM_BaseIndex.ToString("0000"));
            }
            set
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                m_plc.SetIO(value, "M" + iM_BaseIndex.ToString("0000"));
                if (m_IsUseMode)
                    m_plc.SetIO(value, "M" + m_mode.ToString("0000"));

            }
        }
        public bool Stop
        {
            get
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                return m_plc.IOData.GetBit("M" + m_stop.ToString("0000"));
            }
            set
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                m_plc.SetIO(value, "M" + m_stop.ToString("0000"));
            }
        }
        public bool IsRunning
        {
            get
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                return m_plc.IOData.GetBit("M" + m_running.ToString("0000"));
            }
            //set
            //{
            //    //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
            //    m_plc.SetIO(value, "M" + iM_BaseIndex.ToString("0000"));
            //}
        }
        public bool IsComplete
        {
            get
            {
                //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
                return m_plc.IOData.GetBit("M" + m_complete.ToString("0000"));
            }
            //set
            //{
            //    //FATEKAddressClass address = ADDRESSARRAY[(int)MainSDAddressEnum.ADR_RED];
            //    m_plc.SetIO(value, "M" + iM_BaseIndex.ToString("0000"));
            //}
        }
        public int ADR_Start
        {
            get { return iM_BaseIndex; }
            set { iM_BaseIndex = value; }
        }
        public int ADR_IsRunning
        {
            get { return m_running; }
            set { m_running = value; }
        }
        public int ADR_IsComplete
        {
            get { return m_complete; }
            set { m_complete = value; }
        }
        public int ADR_Stop
        {
            get { return m_stop; }
            set { m_stop = value; }
        }
        public int ADR_Mode
        {
            get { return m_mode; }
            set { m_mode = value; }
        }
        public bool IsUseMode
        {
            get { return m_IsUseMode; }
            set { m_IsUseMode = value; }
        }
        //public void SetCompleteAddr(int iAddr)
        //{
        //    m_complete = iAddr;
        //}
        //public void SetRunningAddr(int iAddr)
        //{
        //    m_running = iAddr;
        //}
        //public void SetStopAddr(int iAddr)
        //{
        //    m_stop = iAddr;
        //}

    }


}
