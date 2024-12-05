using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using JetEazy;
using JetEazy.ControlSpace;
using JetEazy.ControlSpace.MotionSpace;
using JetEazy.BasicSpace;
using MotoCan;
using JetEazy.ControlSpace.RobotSpace;
//using JetEazy.ControlSpace.PLCSpace;

namespace Allinone.ControlSpace.MachineSpace
{
    public enum Machine_EA : int
    {
        Allinone = 0,
        KBAOI = 1,
        KBHeight = 2,
        KBOffset = 3,
        KBGap = 4,
        Audix = 5,
        AudixDfly = 6,
        R32 = 7,
        RXX = 8,
        R15 = 9,
        R9 = 10,
        R3 = 11,
        R1 = 12,
        R5 = 13,
        C3 = 14,
        MAIN_SD =15,
        MAIN_X6=16,
        MAIN_SDM1 = 17,
        MAIN_SDM2 = 18,
        MAIN_SERVICE = 19,
        MAIN_SDM3 = 20,
        MAIN_SDM5 = 21,
    };


    [Serializable]
    public abstract class GeoMachineClass
    {
        protected int PLCCount = 0;
        protected int MotionCount = 0;
        protected int LightCount = 0;
        public RobotType mRobotType = RobotType.NONE;

        //public Mitsubishi_FX3UClass[] PLCCollection_FX3U;

        public RobotHCFA mRobotHCFA;

        public JetEazy.ControlSpace.PLCSpace.LightControlClass[] LightCollection;
        public FatekPLCClass[] PLCCollection;
        public PLCMotionClass[] PLCMOTIONCollection;

        public CanMotoControl CANMOTIONControl;
        public CanMotionClass[] CANMOTIONCollection;

        protected VersionEnum VERSION;
        protected OptionEnum OPTION;
        protected CameraActionMode CAMACT;

        protected string WORKPATH;

        protected Machine_EA myMachineEA;
        protected JzTimes myJzTimes;

        protected ProcessClass MainProcess;

        protected bool IsDirect = false;
        protected bool IsNoUseIO = false;
        protected bool IsNoUseMotor = false;

        protected int TickCount = 5;
        protected int CurrentIndex = 0;

        public int[] DelayTime = new int[10];

        //public WorkStatusCollectionClass RunStatusCollection = new WorkStatusCollectionClass();

        /// <summary>
        /// 準備要開始的一些資料
        /// </summary>
        public abstract void GetStart(bool isdirect, bool isnouseplc);
        /// <summary>
        /// 
        /// </summary>
        public abstract void Tick();
        public abstract void MainProcessTick();
        public abstract void SetDelayTime();
        public abstract void CheckEvent();
        public abstract void GoHome();
        public abstract void GetOPString(string opstr);
        public abstract bool Initial(bool isnouseio,bool isnousemotor);

        public virtual void Close()
        {

        }

        public virtual string PLCFps()
        {
            return string.Empty;
        }

        //public abstract void SetNormalTemp(bool ebTemp);

        protected UInt16 HEX16(string HexStr)
        {
            return System.Convert.ToUInt16(HexStr, 16);
        }
        protected UInt32 HEX32(string HexStr)
        {
            return System.Convert.ToUInt32(HexStr, 16);
        }

        //public delegate void TriggerHandler(MachineEventEnum machineevent);
        //public event TriggerHandler TriggerAction;
        //public void OnTrigger(MachineEventEnum machineevent)
        //{
        //    if (TriggerAction != null)
        //    {
        //        TriggerAction(machineevent);
        //    }
        //}

        public delegate void TriggerHandler(MachineEventEnum machineevent, object obj = null);
        public event TriggerHandler TriggerAction;
        public void OnTrigger(MachineEventEnum machineevent, object obj = null)
        {
            if (TriggerAction != null)
            {
                TriggerAction(machineevent, obj);
            }
        }
    }
}
