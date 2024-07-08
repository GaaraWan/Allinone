using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JetEazy.ControlSpace;
using JetEazy;


namespace Allinone.ControlSpace.IOSpace
{
    public enum AllinoneAddressEnum : int
    {
        COUNT = 39,

        ADR_ISSTART = 0,
        ADR_ISEMC = 1,
        ADR_ISUUP = 2,
        ADR_ISUDN = 3,
        ADR_ISCYOUT = 4,
        ADR_ISCYIN = 5,
        ADR_ISLIGHTCURTAIN = 6,

        ADR_RED = 7,
        ADR_YELLOW = 8,
        ADR_GREEN = 9,

        ADR_LIGHTBOARD = 10,
        ADR_LIGHTBAR = 11,

        ADR_UCYLINDER = 12,
        ADR_LCYLINDERIN = 13,
        ADR_PG = 14,
        ADR_PGTIME = 15,
        ADR_PGTIMESET = 16,
        ADR_PGPOWER = 17,

        ADR_ISASMIN = 18, 
        ADR_ISASMOUT= 19,
        ADR_ASMIN = 20,
        ADR_ASMOUT = 21,

        ADR_LCYLINDEROUT = 22,
        ADR_LIGHTAROUND = 23,

        ADR_XMOTION_TAKEPOS = 24,
        ADR_XMOTION_BLOWINGPOS = 25,

        ADR_ISFEEDING = 26,
        ADR_ISFEEDCOMPLETE = 27,

        ADR_DOOR = 28,
        ADR_PCTOPLC_COMPLETE_SENSOR = 29,
        ADR_ISDOOR_UP = 30,
        ADR_ISDOOR_DOWN = 31,

        ADR_ISALARMS_SERIOUS = 32,
        ADR_ISALARMS_COMMON = 33,
        ADR_CLEARALARMS = 34,
        ADR_RESET = 35,
        ADR_ISRESET_COMPLETE = 36,
        ADR_TESTING = 37,
        ADR_BUZZER = 38,

        PGTIMESET = 100,
    }

    public enum AllinoneAlarmsEnum : int
    {
        ALARMSCOUNT = 2,
        ALARMS_ADR_SERIOUS = 0,
        ALARMS_ADR_COMMON = 1,
    }
    public class JzAllinoneIOClass : GeoIOClass
    {

        int PGTIMESET = 15;
        public PLCAlarmsClass[] PLCALARMS;

        public JzAllinoneIOClass()
        {   

        }
        public void Initial(string path,OptionEnum option,FatekPLCClass [] plc)
        {
            ADDRESSARRAY = new FATEKAddressClass[(int)AllinoneAddressEnum.COUNT];
            PLCALARMS = new PLCAlarmsClass[(int)AllinoneAlarmsEnum.ALARMSCOUNT];

            PLC = plc;

            INIFILE = path + "\\IO.INI";

            LoadData();

            PGTimeSet = PGTIMESET;
        }
        public override void LoadData()
        {
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISSTART] = new FATEKAddressClass(ReadINIValue("Status Address", AllinoneAddressEnum.ADR_ISSTART.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISEMC] = new FATEKAddressClass(ReadINIValue("Status Address", AllinoneAddressEnum.ADR_ISEMC.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISUUP] = new FATEKAddressClass(ReadINIValue("Status Address", AllinoneAddressEnum.ADR_ISUUP.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISUDN] = new FATEKAddressClass(ReadINIValue("Status Address", AllinoneAddressEnum.ADR_ISUDN.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISCYOUT] = new FATEKAddressClass(ReadINIValue("Status Address", AllinoneAddressEnum.ADR_ISCYOUT.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISCYIN] = new FATEKAddressClass(ReadINIValue("Status Address", AllinoneAddressEnum.ADR_ISCYIN.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISLIGHTCURTAIN] = new FATEKAddressClass(ReadINIValue("Status Address", AllinoneAddressEnum.ADR_ISLIGHTCURTAIN.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISASMIN] = new FATEKAddressClass(ReadINIValue("Status Address", AllinoneAddressEnum.ADR_ISASMIN.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISASMOUT] = new FATEKAddressClass(ReadINIValue("Status Address", AllinoneAddressEnum.ADR_ISASMOUT.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISFEEDING] = new FATEKAddressClass(ReadINIValue("Status Address", AllinoneAddressEnum.ADR_ISFEEDING.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISFEEDCOMPLETE] = new FATEKAddressClass(ReadINIValue("Status Address", AllinoneAddressEnum.ADR_ISFEEDCOMPLETE.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISDOOR_UP] = new FATEKAddressClass(ReadINIValue("Status Address", AllinoneAddressEnum.ADR_ISDOOR_UP.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISDOOR_DOWN] = new FATEKAddressClass(ReadINIValue("Status Address", AllinoneAddressEnum.ADR_ISDOOR_DOWN.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISALARMS_SERIOUS] = new FATEKAddressClass(ReadINIValue("Status Address", AllinoneAddressEnum.ADR_ISALARMS_SERIOUS.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISALARMS_COMMON] = new FATEKAddressClass(ReadINIValue("Status Address", AllinoneAddressEnum.ADR_ISALARMS_COMMON.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISRESET_COMPLETE] = new FATEKAddressClass(ReadINIValue("Status Address", AllinoneAddressEnum.ADR_ISRESET_COMPLETE.ToString(), "", INIFILE));


            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_RED] = new FATEKAddressClass(ReadINIValue("Operation Address", AllinoneAddressEnum.ADR_RED.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_YELLOW] = new FATEKAddressClass(ReadINIValue("Operation Address", AllinoneAddressEnum.ADR_YELLOW.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_GREEN] = new FATEKAddressClass(ReadINIValue("Operation Address", AllinoneAddressEnum.ADR_GREEN.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_LIGHTBOARD] = new FATEKAddressClass(ReadINIValue("Operation Address", AllinoneAddressEnum.ADR_LIGHTBOARD.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_LIGHTBAR] = new FATEKAddressClass(ReadINIValue("Operation Address", AllinoneAddressEnum.ADR_LIGHTBAR.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_UCYLINDER] = new FATEKAddressClass(ReadINIValue("Operation Address", AllinoneAddressEnum.ADR_UCYLINDER.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_LCYLINDERIN] = new FATEKAddressClass(ReadINIValue("Operation Address", AllinoneAddressEnum.ADR_LCYLINDERIN.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_PG] = new FATEKAddressClass(ReadINIValue("Operation Address", AllinoneAddressEnum.ADR_PG.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_PGPOWER] = new FATEKAddressClass(ReadINIValue("Operation Address", AllinoneAddressEnum.ADR_PGPOWER.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ASMIN] = new FATEKAddressClass(ReadINIValue("Operation Address", AllinoneAddressEnum.ADR_ASMIN.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ASMOUT] = new FATEKAddressClass(ReadINIValue("Operation Address", AllinoneAddressEnum.ADR_ASMOUT.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_DOOR] = new FATEKAddressClass(ReadINIValue("Operation Address", AllinoneAddressEnum.ADR_DOOR.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_PCTOPLC_COMPLETE_SENSOR] = new FATEKAddressClass(ReadINIValue("Operation Address", AllinoneAddressEnum.ADR_PCTOPLC_COMPLETE_SENSOR.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_CLEARALARMS] = new FATEKAddressClass(ReadINIValue("Operation Address", AllinoneAddressEnum.ADR_CLEARALARMS.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_RESET] = new FATEKAddressClass(ReadINIValue("Operation Address", AllinoneAddressEnum.ADR_RESET.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_TESTING] = new FATEKAddressClass(ReadINIValue("Operation Address", AllinoneAddressEnum.ADR_TESTING.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_BUZZER] = new FATEKAddressClass(ReadINIValue("Operation Address", AllinoneAddressEnum.ADR_BUZZER.ToString(), "", INIFILE));


            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_LCYLINDEROUT] = new FATEKAddressClass(ReadINIValue("Operation Address", AllinoneAddressEnum.ADR_LCYLINDEROUT.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_LIGHTAROUND] = new FATEKAddressClass(ReadINIValue("Operation Address", AllinoneAddressEnum.ADR_LIGHTAROUND.ToString(), "", INIFILE));

            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_PGTIME] = new FATEKAddressClass(ReadINIValue("Data Address", AllinoneAddressEnum.ADR_PGTIME.ToString(), "0", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_PGTIMESET] = new FATEKAddressClass(ReadINIValue("Data Address", AllinoneAddressEnum.ADR_PGTIMESET.ToString(), "0", INIFILE));

            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_XMOTION_TAKEPOS] = new FATEKAddressClass(ReadINIValue("Data Address", AllinoneAddressEnum.ADR_XMOTION_TAKEPOS.ToString(), "", INIFILE));
            ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_XMOTION_BLOWINGPOS] = new FATEKAddressClass(ReadINIValue("Data Address", AllinoneAddressEnum.ADR_XMOTION_BLOWINGPOS.ToString(), "", INIFILE));
            

            PGTIMESET = int.Parse(ReadINIValue("Parameters", AllinoneAddressEnum.PGTIMESET.ToString(), PGTIMESET.ToString(), INIFILE));

            int iindex = 0;
            string str = "";
            //string str_alarms = ReadINIValue("Parameters", "ALARMS_ADR_SERIOUS", "", INIFILE.Replace("IO.INI", "ALARMIO0.INI"));
            PLCALARMS[(int)AllinoneAlarmsEnum.ALARMS_ADR_SERIOUS] = new PLCAlarmsClass(ReadINIValue("Parameters", AllinoneAlarmsEnum.ALARMS_ADR_SERIOUS.ToString(), "", INIFILE.Replace("IO.INI", "ALARMIO0.INI")));
            foreach (PLCAlarmsItemClass item in PLCALARMS[(int)AllinoneAlarmsEnum.ALARMS_ADR_SERIOUS].PLCALARMSLIST)
            {
                iindex = 0;
                while (iindex < 16)
                {
                    str = ReadINIValue(item.ADR_Address, "Bit " + iindex.ToString(), "", INIFILE.Replace("IO.INI", "ALARMIO0.INI"));
                    PLCALARMS[(int)AllinoneAlarmsEnum.ALARMS_ADR_SERIOUS].PLCAlarmsAddDescription("0," + item.CovertToNormalAddress(iindex) + "," + str);
                    iindex++;
                }
            }
            PLCALARMS[(int)AllinoneAlarmsEnum.ALARMS_ADR_COMMON] = new PLCAlarmsClass(ReadINIValue("Parameters", AllinoneAlarmsEnum.ALARMS_ADR_COMMON.ToString(), "", INIFILE.Replace("IO.INI", "ALARMIO0.INI")));
            foreach (PLCAlarmsItemClass item in PLCALARMS[(int)AllinoneAlarmsEnum.ALARMS_ADR_COMMON].PLCALARMSLIST)
            {
                iindex = 0;
                while (iindex < 16)
                {
                    str = ReadINIValue(item.ADR_Address, "Bit " + iindex.ToString(), "", INIFILE.Replace("IO.INI", "ALARMIO0.INI"));
                    PLCALARMS[(int)AllinoneAlarmsEnum.ALARMS_ADR_COMMON].PLCAlarmsAddDescription("0," + item.CovertToNormalAddress(iindex) + "," + str);
                    iindex++;
                }
            }
            //if (!string.IsNullOrEmpty(str_alarms))
            //{
            //    //string[] strs = str_alarms.Split(',');
            //    M_ALARMSSERIOUS_COUNT = str_alarms.Split(',').Length;
            //}
            //str_alarms = ReadINIValue("Parameters", "ALARMS_ADR_COMMON", "", INIFILE.Replace("IO.INI", "ALARMIO0.INI"));
            //if (!string.IsNullOrEmpty(str_alarms))
            //{
            //    //string[] strs = str_alarms.Split(',');
            //    M_ALARMSCOMMON_COUNT = str_alarms.Split(',').Length;
            //}
        }

        public override void SaveData()
        {
            throw new NotImplementedException();
        }

        public bool Red
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_RED];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_RED];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }

        public bool Yellow
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_YELLOW];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_YELLOW];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool Green
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_GREEN];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_GREEN];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool IsFeeding
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISFEEDING];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool IsFeedComplete
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISFEEDCOMPLETE];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        /// <summary>
        /// 收到供料启动信号反馈PLC信号
        /// </summary>
        public bool FeedPctoPlcSensor
        {
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_TESTING];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool IsAlarmsSerious
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISALARMS_SERIOUS];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool IsAlarmsCommon
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISALARMS_COMMON];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0) || PLC[address.SiteNo].IOData.GetBit(address.Address1);
            }
        }
        public bool IsStart
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISSTART];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool IsEMC
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISEMC];
                return !PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool IsUUP
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISUUP];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool IsUDN
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISUDN];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool IsCYOUT
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISCYOUT];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool IsCYIN
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISCYIN];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool IsLightCurtain
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISLIGHTCURTAIN];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool IsDoorUP
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISDOOR_UP];
                return !PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool IsDoorDN
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISDOOR_DOWN];
                return !PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }
        public bool OpDoor
        {
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_DOOR];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool PcToPlcCompleteSensor
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_PCTOPLC_COMPLETE_SENSOR];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_PCTOPLC_COMPLETE_SENSOR];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool TopLight
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_LIGHTBOARD];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_LIGHTBOARD];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool MylarLight
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_LIGHTBAR];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_LIGHTBAR];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool AroundLight
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_LIGHTAROUND];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_LIGHTAROUND];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool ASMIn
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISASMIN];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ASMIN];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool ASMOut
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISASMOUT];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ASMOUT];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }

        public bool UCylinder
        {
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_UCYLINDER];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool LCylinderIn
        {
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_LCYLINDERIN];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool LCylinderOut
        {
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_LCYLINDEROUT];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool PG
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_PG];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_PG];
                PLC[address.SiteNo].SetIO(true, address.Address0);
            }
        }
        public bool PGPOWER
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_PGPOWER];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_PGPOWER];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool CLEARALARMS
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_CLEARALARMS];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_CLEARALARMS];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool RESET
        {
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_RESET];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool BUZZER
        {
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_BUZZER];
                PLC[address.SiteNo].SetIO(value, address.Address0);
            }
        }
        public bool ISRESETCOMPLETE
        {
            get
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_ISRESET_COMPLETE];
                return PLC[address.SiteNo].IOData.GetBit(address.Address0);
            }
        }

        public int PGTime
        {
            get
            {
                int ret = 0;

                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_PGTIME];

                ret += (ret << 16) + PLC[address.SiteNo].IOData.GetData(address.Address0);

                return (int)ret;
            }
        }
        public int PGTimeSet
        {
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_PGTIMESET];

                long setL = value % 65536;

                PLC[address.SiteNo].SetData(ValueToHEX(setL, 4), address.Address0);
            }
        }

       
        public int XMotionTakePos
        {
            get
            {
                //ulong ret = 0;
                Int32 ret = 0;

                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_XMOTION_TAKEPOS];
                ret = HEXSigned32(ValueToHEX(PLC[address.SiteNo].IOData.GetData(address.Address1), 4) + ValueToHEX(PLC[address.SiteNo].IOData.GetData(address.Address0), 4));
                
                return (int)ret;
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_XMOTION_TAKEPOS];

                long setH = value >> 16;
                long setL = value % 65536;

                //PLC[address.SiteNo].SetData(address.Address0, ValueToHEX(setL, 4));
                //PLC[address.SiteNo].SetData(address.Address1, ValueToHEX(setH, 4));
                PLC[address.SiteNo].SetData(ValueToHEX(setL, 4), address.Address0);
                PLC[address.SiteNo].SetData(ValueToHEX(setH, 4), address.Address1);
            }
        }
        public int XMotionBlowingPos
        {
            get
            {
                //ulong ret = 0;
                Int32 ret = 0;

                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_XMOTION_BLOWINGPOS];
                ret = HEXSigned32(ValueToHEX(PLC[address.SiteNo].IOData.GetData(address.Address1), 4) + ValueToHEX(PLC[address.SiteNo].IOData.GetData(address.Address0), 4));

                return (int)ret;
            }
            set
            {
                FATEKAddressClass address = ADDRESSARRAY[(int)AllinoneAddressEnum.ADR_XMOTION_BLOWINGPOS];

                long setH = value >> 16;
                long setL = value % 65536;

                //PLC[address.SiteNo].SetData(address.Address0, ValueToHEX(setL, 4));
                //PLC[address.SiteNo].SetData(address.Address1, ValueToHEX(setH, 4));
                PLC[address.SiteNo].SetData(ValueToHEX(setL, 4), address.Address0);
                PLC[address.SiteNo].SetData(ValueToHEX(setH, 4), address.Address1);
            }
        }

        public bool GetAlarmsAddress(int iSiteNo, string strAddress)
        {
            return PLC[iSiteNo].IOData.GetBit(strAddress);
        }
        protected Int32 HEXSigned32(string HexStr)
        {
            return System.Convert.ToInt32(HexStr, 16);
        }

    }
}
