

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

using JetEazy;
using JetEazy.BasicSpace;
using JetEazy.ControlSpace;
using Allinone.ControlSpace;
using Allinone.ControlSpace.MachineSpace;
using JzMSR.OPSpace;

using AllinOne.Jumbo.Net;
using Allinone.Crystal.Net;

using JzKHC.OPSpace;

#if (OPT_USE_THROUGH)
using Jumbo301Client.ControlSpace;
using Jumbo301Client.UniversalSpace;
#endif

using Allinone.ControlSpace.IOSpace;
using JetEazy.DBSpace;


namespace Allinone.OPSpace.ResultSpace
{
    class JzAllInoneResultClass : GeoResultClass
    {
        JzAllinoneMachineClass MACHINE;

        AccDBClass ACCDB
        {
            get
            {
                return Universal.ACCDB;
            }
        }

        ProcessClass JumboCalibrateProcess = new ProcessClass();
        ProcessClass CrystalProcess = new ProcessClass();
        JzToolsClass JzTools = new JzToolsClass();

        KHCClass KHCCollection
        {
            get { return Universal.KHCCollection; }
        }

        /// <summary>
        /// 連接至鍵高服務器
        /// </summary>
        IxConnectJumbo301 IXCONNECTJUMBO
        {
            get { return Universal.IXCONNECTJUMBO; }
        }
        /// <summary>
        /// 連接至量測服務器
        /// </summary>
        IxConnectCrystal501 IXCONNECTCRYSTAL
        {
            get { return Universal.IXCONNECTCRYSTAL; }
        }
        public JzAllInoneResultClass(Result_EA resultea, VersionEnum version, OptionEnum option, MachineCollectionClass machinecollection)
        {
            myResultEA = resultea;
            VERSION = version;
            OPTION = option;

            MACHINE = (JzAllinoneMachineClass)machinecollection.MACHINE;
            MACHINE.TriggerAction += MACHINE_TriggerAction;
            MACHINE.EVENT.TriggerAlarm += EVENT_TriggerAlarm;
            MainProcess = new ProcessClass();

            //JUMBOSERVEREVENT.LocalBroadCastEvent += JUMBOSERVEREVENT_LocalBroadCastEvent;
            //IXCONNECTJUMBO.BroadCastEvent += new BroadCastEventHandler(JUMBOSERVEREVENT.BroadCasting);

        }

        private void EVENT_TriggerAlarm(bool IsBuzzer)
        {
            MACHINE.PLCIO.BUZZER = IsBuzzer;
            if (!IsBuzzer)
                MACHINE.ClearAlarm = true;

            //SetRunningLight();
        }

        private void MACHINE_TriggerAction(MachineEventEnum machineevent)
        {
            switch (machineevent)
            {
                case MachineEventEnum.ALARM_SERIOUS:
                    IsAlarmsSeriousX = true;
                    //SetAbnormalLight();
                    break;
                case MachineEventEnum.ALARM_COMMON:
                    IsAlarmsCommonX = true;
                    //SetAbnormalLight();
                    break;
            }
        }

#region ALARMS

        bool IsAlarmsSeriousX = false;
        bool IsAlarmsCommonX = false;

        void SetSeriousAlarms()
        {
            foreach (PLCAlarmsItemDescriptionClass item in MACHINE.PLCIO.PLCALARMS[(int)AllinoneAlarmsEnum.ALARMS_ADR_SERIOUS].PLCALARMSDESCLIST)
            {
                if (MACHINE.PLCIO.GetAlarmsAddress(item.BitNo, item.ADR_Address))
                {
                    MACHINE.EVENT.GenEvent("A0001", EventActionTypeEnum.AUTOMATIC, item.ADR_Chinese, ACCDB.DataNow);
                }
            }
        }
        void SetCommonAlarms()
        {
            foreach (PLCAlarmsItemDescriptionClass item in MACHINE.PLCIO.PLCALARMS[(int)AllinoneAlarmsEnum.ALARMS_ADR_COMMON].PLCALARMSDESCLIST)
            {
                if (MACHINE.PLCIO.GetAlarmsAddress(item.BitNo, item.ADR_Address))
                {
                    MACHINE.EVENT.GenEvent("A0002", EventActionTypeEnum.AUTOMATIC, item.ADR_Chinese, ACCDB.DataNow);
                }
            }
        }

        void SetNormalLight()
        {
            MACHINE.PLCIO.Red = false;
            MACHINE.PLCIO.Yellow = true;
            MACHINE.PLCIO.Green = false;
        }
        void SetAbnormalLight()
        {
            MACHINE.PLCIO.Red = true;
            MACHINE.PLCIO.Yellow = false;
            MACHINE.PLCIO.Green = false;
        }
        void SetRunningLight()
        {
            MACHINE.PLCIO.Red = false;
            MACHINE.PLCIO.Yellow = false;
            MACHINE.PLCIO.Green = true;
        }
        void SetBuzzer(bool IsON)
        {
            //USEIO.Buzzer = IsON;
            MACHINE.PLCIO.BUZZER = IsON;
        }

        void StopAllProcess(bool bCancel)
        {
            MainProcess.Stop();
            JumboCalibrateProcess.Stop();
            CrystalProcess.Stop();

            //SetNormalLight();
        }

#endregion

        private void CCDCollection_TriggerAction(string operationstr)
        {
            throw new NotImplementedException();
        }

        public override void GetStart(AlbumClass albumwork, CCDCollectionClass ccdcollection, TestMethodEnum testmethod, bool isnouseccd)
        {
            if (MainProcess.IsOn)
            {
                //MainProcess.Stop();
                StopAllProcess(false);

                OnTrigger(ResultStatusEnum.FORECEEND);

                return;
            }

            OnTrigger(ResultStatusEnum.CALSTART);

            AlbumWork = albumwork;
            CCDCollection = ccdcollection;

            TestMethod = testmethod;
            IsNoUseCCD = isnouseccd;

            ResetData(-1);

            MainProcess.Start();

        }
        public override void Tick()
        {
            if (IsAlarmsSeriousX)
            {
                IsAlarmsSeriousX = false;
                SetSeriousAlarms();
                StopAllProcess(false);
            }

            if (IsAlarmsCommonX)
            {
                IsAlarmsCommonX = false;
                SetCommonAlarms();
                //StopAllProcess(false);
            }

            if (!IsNoUseCCD && (MACHINE.PLCIO.IsEMC))// || MACHINE.PLCIO.IsLightCurtain))
            {
                StopAllProcess(true);
            }

            MainProcessTick();
            JumboCalibrateTick();
            CrystalCalibrateTick();
            MACHINE.Tick();
        }
        public override void GenReport()
        {

        }
        public override void SetDelayTime()
        {
            DelayTime[0] = INI.DELAYTIME;
        }

        int JumboOffline = 6;
        string GeneralPosition = "";
        string[] GeneralPositions;
        int GeneralPositionIndex = 0;
        List<Point> PGList = new List<Point>();//
        List<Rectangle> PGRECTList = new List<Rectangle>();
        MSRClass MSR
        {
            get { return Universal.MSRCollection.DataLast; }
        }
        string m_running_files = @"D:\TMP_HEIGHT_CHECK";
        JzTimes TestTimer = new JzTimes();
        int[] Testms = new int[100];
        //用在測試字符綫程中，判斷計算是否完成
        bool m_ismainworkcomplete = false;

        bool IsFristSetExposure = true;
        int iRpiReviced = -1;//rpi points NONE

        bool isTestTimer = true;
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        protected override void MainProcessTick()
        {
            ProcessClass Process = MainProcess;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        if (isTestTimer)
                            TestTimeTemp(Process.ID);

                        switch (TestMethod)
                        {
                            case TestMethodEnum.BUTTON:

                                //MACHINE.GoXReadyPosition();
                                MACHINE.PLCIO.AroundLight = true;
                                MACHINE.PLCIO.TopLight = INI.ISUSECRYSTALSERVER; //间隙开启顶灯
                                MACHINE.PLCIO.MylarLight = !INI.ISUSECRYSTALSERVER;//间隙开启关闭mylar

                                Process.NextDuriation = 50;
                                Process.ID = 510;

                                break;
                            case TestMethodEnum.IO:

                                Process.NextDuriation = 0;
                                Process.ID = 510;

                                break;
                        }

                        break;
                    case 510:
                        if (MACHINE.ISAllOnSite() || IsNoUseCCD)
                        //if (MACHINE.PLCIO.ASMIn && MACHINE.ISAllOnSite())
                        {
                            if (isTestTimer)
                                TestTimeTemp(Process.ID);

                            TestTimer.Cut();

                            OnEnvTrigger(ResultStatusEnum.CHANGEDIRECTORY, 0, "");
                            OnTrigger(ResultStatusEnum.COUNTSTART);

                            Process.NextDuriation = 50;

                            //Clear PG
                            PGList.Clear();
                            Universal.SCREENPOINTS.m_showmypoints.DrawMyPaints(PGList);

                            if (IsNoUseCCD)
                                Process.ID = 20;
                            else
                            {
                                OnEnvTrigger(ResultStatusEnum.SETCAMLIGHT, EnvIndex, "1,1");
                                //Process.ID = 10;  //MENTHON 001

                                //Process.ID = 11; //MENTHON 000

                                Process.ID = 12; //MENTHON 002
                            }
                        }
                        break;

#region MENTHON 000
                    /*
                     * 1.字符抓圖測試
                     * 2.間隙抓圖測試
                     * 3.鍵高測試
                     * 4.送到OP的位置
                     * 5.給PLC送測試完成信號
                     * 
                    */

                    case 11:
                        if (Process.IsTimeup)
                        {
                            if (isTestTimer)
                                TestTimeTemp(Process.ID);

                            MACHINE.PLCIO.TopLight = true;
                            MACHINE.PLCIO.MylarLight = true;
                            MACHINE.PLCIO.AroundLight = false;

                            Process.NextDuriation = 10;
                            Process.ID = 1100;
                        }
                        break;
                    case 1100:
                        if (Process.IsTimeup)
                        {
                            if (isTestTimer)
                                TestTimeTemp(Process.ID);

                            if (MACHINE.PLCIO.TopLight && MACHINE.PLCIO.MylarLight)
                            {
                                Process.NextDuriation = 100;
                                Process.ID = 1110;
                            }
                        }
                        break;
                    case 1110:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.PLCIO.TopLight && MACHINE.PLCIO.MylarLight)
                            {
                                if (isTestTimer)
                                    TestTimeTemp(Process.ID);

                                CCDCollection.GetImageFX(INI.CRYSTAL_SERVER_CAM_RELATION + ",009");

                                //MACHINE.PLCIO.ASMOut = true;
                                FillProcessImage(INI.CRYSTAL_SERVER_CAM_RELATION + ",009");
                                OnEnvTrigger(ResultStatusEnum.SAVEDEBUGRAW, EnvIndex, INI.CRYSTAL_SERVER_CAM_RELATION + ",009:");

                                //取得Compound 在這個 ENV 裏的資料
                                AlbumWork.CPD.CollectRUNVIEWData(AlbumWork, AlbumWork.ENVList[EnvIndex].No);

                                if (!IsNoUseCCD)
                                {
                                    //另開綫程用來測試字符
                                    System.Threading.Thread m_thread = new System.Threading.Thread(new System.Threading.ThreadStart(_mainwork));
                                    m_thread.Start();

                                    //CAM0-CAM9 
                                    //string fristExposure = "0#40;1#40;2#40;3#40;4#40";
                                    string fristExposure = INI.KEYCAPEXPOSURE;
                                    CCDCollection.SetExposure(fristExposure);
                                }

                                Process.NextDuriation = 100;
                                Process.ID = 1120;
                            }
                        }
                        break;
                    case 1120:                        //拍片完後關燈，然後到下一個拍片的地方，就換萬子投射了… 鍵帽取像
                        if (Process.IsTimeup)
                        {
                            //if (MACHINE.PLCIO.ASMOut)
                            {
                                if (isTestTimer)
                                    TestTimeTemp(Process.ID);

                                //給Crystal抓圖測試
                                if (INI.ISUSECRYSTALSERVER)
                                {
#if (OPT_USE_THROUGH_CRYSTAL)
                                    Universal.M_CRYSTALCLIENT.RESULT.IsKGMComplete = false;
#endif
                                    CrystalProcess.Start("AUTO");

                                    Process.NextDuriation = 10;
                                    Process.ID = 1130;
                                }
                                else
                                {
                                    Process.NextDuriation = 0;
                                    Process.ID = 1140;
                                }
                            }
                        }
                        break;
                    case 1130:                        //拍片完後關燈，然後到下一個拍片的地方，就換萬子投射了… 鍵帽取像
                        if (Process.IsTimeup)
                        {
                            if (!CrystalProcess.IsOn)
                            {
                                if (isTestTimer)
                                    TestTimeTemp(Process.ID);

                                MACHINE.PLCIO.ASMIn = true;

                                Process.NextDuriation = 0;
                                Process.ID = 1140;
                            }
                        }
                        break;
                    case 1140:                        //拍片完後關燈，然後到下一個拍片的地方，就換萬子投射了… 鍵帽取像
                        if (Process.IsTimeup)
                        {
                            if (INI.ISUSEJUMBOSERVER)
                            {
                                //if (MACHINE.PLCIO.ASMIn)
                                {
                                    if (isTestTimer)
                                        TestTimeTemp(Process.ID);

#if OPT_USE_THROUGH
                                    Universal.M_JUMBOCLIENT.RESULT_EVO.IsKHCComplete = false;

#endif

                                    JumboCalibrateProcess.Start("ONLINE");
                                    Process.NextDuriation = 10;
                                    Process.ID = 1150;
                                }
                            }
                            else
                            {
                                if (isTestTimer)
                                    TestTimeTemp(Process.ID);

                                Process.NextDuriation = 10;
                                Process.ID = 1160;
                            }
                        }
                        break;
                    case 1150:                        //鍵高測試結束 
                        if (Process.IsTimeup)
                        {
                            if (!JumboCalibrateProcess.IsOn)
                            {
                                if (isTestTimer)
                                    TestTimeTemp(Process.ID);

                                MACHINE.PLCIO.ASMOut = true;

                                Process.NextDuriation = 0;
                                Process.ID = 1160;
                            }
                        }
                        break;
                    case 1160:
                        if (Process.IsTimeup)
                        {
                            if (isTestTimer)
                                TestTimeTemp(Process.ID);

                            //if (MACHINE.PLCIO.ASMOut)
                            {
                                //取圖后 馬達到op拿料的位置
                                if (!Universal.SCREENPOINTS.IsNoUseMotion)
                                {
                                    MACHINE.GoXPosition(GeneralPositions[0]);
                                    //MACHINE.GoXReadyPosition();
                                }
                            }

                            Process.NextDuriation = 0;
                            Process.ID = 1170;
                        }
                        break;
                    case 1170:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.ISAllOnSite())
                            {
                                if (isTestTimer)
                                    TestTimeTemp(Process.ID);

                                Process.NextDuriation = 0;
                                Process.ID = 3011; //进入组合结果
                            }
                        }
                        break;

#endregion

#region MENTHON 001
                    /*
                     * 1.鍵高測試
                     * 2.字符抓圖測試
                     * 3.間隙測試
                     * 4.送到OP的位置
                     * 5.給PLC送測試完成信號
                     * 
                    */

                    case 10:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
                        if (Process.IsTimeup)
                        {
                            if (isTestTimer)
                                TestTimeTemp(Process.ID);

                            MACHINE.PLCIO.TopLight = false;
                            MACHINE.PLCIO.MylarLight = false;
                            MACHINE.PLCIO.AroundLight = false;

                            //合起壓框

                            //MACHINE.PLCIO.ASMIn = true;

                            //////

                            Process.NextDuriation = 100;
                            Process.ID = 1010;
                        }
                        break;
                    case 1010:                        //壓框合起後，開始測鍵高
                        if (Process.IsTimeup)
                        {
                            //if (MACHINE.PLCIO.ASMIn)
                            {
                                if (isTestTimer)
                                    TestTimeTemp(Process.ID);

                                if (INI.ISUSEJUMBOSERVER)
                                {
                                    JumboCalibrateProcess.Start("ONLINE");
                                    Process.NextDuriation = 10;
                                    Process.ID = 102001;
                                }
                                else
                                {
                                    Process.NextDuriation = 10;
                                    Process.ID = 1030;
                                }
                            }
                        }
                        break;
                    case 102001:                        //鍵高測試結束 開始燈光抓圖測試字符
                        if (Process.IsTimeup)
                        {
                            if (!JumboCalibrateProcess.IsOn)
                            {
                                if (isTestTimer)
                                    TestTimeTemp(Process.ID);

                                MACHINE.PLCIO.TopLight = true;
                                MACHINE.PLCIO.MylarLight = true;
                                MACHINE.PLCIO.AroundLight = false;

                                Process.NextDuriation = 10;
                                Process.ID = 1030;
                            }
                        }
                        break;
                    case 1030:                        //拍片完後關燈，然後到下一個拍片的地方，就換萬子投射了…
                        if (Process.IsTimeup)
                        {
                            if (isTestTimer)
                                TestTimeTemp(Process.ID);

                            CCDCollection.GetImageFX(INI.CRYSTAL_SERVER_CAM_RELATION + ",009");
                            FillProcessImage(INI.CRYSTAL_SERVER_CAM_RELATION + ",009");

                            OnEnvTrigger(ResultStatusEnum.SAVEDEBUGRAW, EnvIndex, INI.CRYSTAL_SERVER_CAM_RELATION + ",009:");

                            //取得Compound 在這個 ENV 裏的資料
                            AlbumWork.CPD.CollectRUNVIEWData(AlbumWork, AlbumWork.ENVList[EnvIndex].No);

                            if (!IsNoUseCCD)
                            {
                                //另開綫程用來測試字符
                                System.Threading.Thread m_thread = new System.Threading.Thread(new System.Threading.ThreadStart(_mainwork));
                                m_thread.Start();

                                MACHINE.PLCIO.ASMOut = true;

                                //CAM0-CAM9 
                                //string fristExposure = "0#40;1#40;2#40;3#40;4#40";
                                string fristExposure = INI.KEYCAPEXPOSURE;
                                CCDCollection.SetExposure(fristExposure);
                            }

                            Process.NextDuriation = 100;
                            Process.ID = 103010;
                        }
                        break;
                    case 103010:                        //拍片完後關燈，然後到下一個拍片的地方，就換萬子投射了… 鍵帽取像
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.PLCIO.ASMOut)
                            {
                                if (isTestTimer)
                                    TestTimeTemp(Process.ID);

                                //給Crystal抓圖測試
                                if (INI.ISUSECRYSTALSERVER)
                                {
                                    CrystalProcess.Start("AUTO");

                                    Process.NextDuriation = 10;
                                    Process.ID = 103020;
                                }
                                else
                                {
                                    Process.NextDuriation = 0;
                                    Process.ID = 103030;
                                }
                            }
                        }
                        break;
                    case 103020:                        //拍片完後關燈，然後到下一個拍片的地方，就換萬子投射了… 鍵帽取像
                        if (Process.IsTimeup)
                        {
                            if (!CrystalProcess.IsOn)
                            {
                                if (isTestTimer)
                                    TestTimeTemp(Process.ID);

                                Process.NextDuriation = 0;
                                Process.ID = 103030;
                            }
                        }
                        break;
                    case 103030:
                        if (Process.IsTimeup)
                        {
                            if (isTestTimer)
                                TestTimeTemp(Process.ID);

                            //取圖后 馬達到op拿料的位置
                            if (!Universal.SCREENPOINTS.IsNoUseMotion)
                            {
                                MACHINE.GoXPosition(GeneralPositions[0]);
                                //MACHINE.GoXReadyPosition();
                            }

                            Process.NextDuriation = 10;
                            Process.ID = 103040;
                        }
                        break;
                    case 103040:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.ISAllOnSite())
                            {
                                if (isTestTimer)
                                    TestTimeTemp(Process.ID);

                                Process.NextDuriation = 10;
                                Process.ID = 3011; //进入组合结果
                            }
                        }
                        break;
#endregion

#region MENTHON 002
                    /*
                     * 1.間隙抓圖測試
                     * 2.字符抓圖測試
                     * 
                     * 3.鍵高測試
                     * 4.送到OP的位置
                     * 5.給PLC送測試完成信號
                     * 
                    */

                    case 12:
                        if (Process.IsTimeup)
                        {
                            if (isTestTimer)
                                TestTimeTemp(Process.ID);

                            if (IsFristSetExposure)
                            {
                                //string fristExposure = INI.KEYCAPEXPOSURE;
                                string fristExposure = INI.FRAMEEXPOSURE;
                                CCDCollection.SetExposure(fristExposure);
                                string[] strs = INI.FRAMEEXPOSURE.Split(';');
                                //string secondExposure = "0#70;1#70;2#70;3#70;4#70";
                                CCDCollection.SetExposure(strs[0]);
                                //CCDCollection.SetExposure(fristExposure);

                                Process.NextDuriation = 300;
                                Process.ID = 1200;
                            }
                            else
                            {
                                Process.NextDuriation = 10;
                                Process.ID = 1200;
                            }
                        }
                        break;
                    case 1200:
                        if (Process.IsTimeup)
                        {
                            string[] strs = INI.FRAMEEXPOSURE.Split(';');
                            //string secondExposure = "0#70;1#70;2#70;3#70;4#70";
                            CCDCollection.SetExposure(strs[0]);

                            if (isTestTimer)
                                TestTimeTemp(Process.ID);

                            //if (MACHINE.PLCIO.TopLight && MACHINE.PLCIO.MylarLight)
                            {
                                if (IsFristSetExposure)
                                {
                                    Process.NextDuriation = 300;
                                    Process.ID = 1210;

                                    
                                }
                                else
                                {
                                    Process.NextDuriation = 10;
                                    Process.ID = 1210;
                                }
                            }
                        }
                        break;
                    case 1210:                        //拍片完後關燈，然後到下一個拍片的地方，就換萬子投射了… 鍵帽取像
                        if (Process.IsTimeup)
                        {
                            //if (MACHINE.PLCIO.ASMOut)
                            {
                                if (isTestTimer)
                                    TestTimeTemp(Process.ID);

                                //給Crystal抓圖測試
                                if (INI.ISUSECRYSTALSERVER)
                                {
#if OPT_USE_THROUGH_CRYSTAL
                                    Universal.M_CRYSTALCLIENT.RESULT.IsKGMComplete = false;
#endif

                                    CrystalProcess.Start("AUTO");

                                    Process.NextDuriation = 10;
                                    Process.ID = 1220;
                                }
                                else
                                {
                                    Process.NextDuriation = 0;
                                    Process.ID = 1220;
                                }
                            }
                        }
                        break;
                    case 1220:                        //拍片完後關燈，然後到下一個拍片的地方，就換萬子投射了… 鍵帽取像
                        if (Process.IsTimeup)
                        {
                            if (!CrystalProcess.IsOn || !INI.ISUSECRYSTALSERVER)
                            {
                             

                                string camstring = "";
                                EnvClass env = AlbumWork.ENVList[EnvIndex];
                                camstring = env.GetCamString();
                                CCDCollection.SetExposure(camstring);

                                MACHINE.PLCIO.ASMIn = true;

                                Process.NextDuriation = 380;
                                Process.ID = 1230;

                                if (isTestTimer)
                                    TestTimeTemp(Process.ID);
                            }
                        }
                        break;
                    case 1230:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.PLCIO.ASMIn)
                            //if (MACHINE.PLCIO.TopLight && MACHINE.PLCIO.MylarLight && MACHINE.PLCIO.ASMIn)
                            {
                                if (isTestTimer)
                                    TestTimeTemp(Process.ID);

                                CCDCollection.GetImageFX(INI.CRYSTAL_SERVER_CAM_RELATION + ",009", 0);//This is here , DelayTime default 100.
                                MACHINE.PLCIO.AroundLight = false;
                                MACHINE.PLCIO.TopLight = false;
                                MACHINE.PLCIO.MylarLight = false;

                                m_ismainworkcomplete = false;

                                FillProcessImage(INI.CRYSTAL_SERVER_CAM_RELATION + ",009");
                                OnEnvTrigger(ResultStatusEnum.SAVEDEBUGRAW, EnvIndex, INI.CRYSTAL_SERVER_CAM_RELATION + ",009:");

                                //取得Compound 在這個 ENV 裏的資料
                                AlbumWork.CPD.CollectRUNVIEWData(AlbumWork, AlbumWork.ENVList[EnvIndex].No);

                                if (!IsNoUseCCD)
                                {
                                    //另開綫程用來測試字符
                                    System.Threading.Thread m_thread = new System.Threading.Thread(new System.Threading.ThreadStart(_mainwork));
                                    m_thread.Start();

                                    //AlbumWork.A08_RunProcess(PageOPTypeEnum.P00);
                                    //AlbumWork.FillRunStatus(RunStatusCollection);
                                    //m_ismainworkcomplete = true;
                                }

                                JzTools.CGOperate();

                                Process.NextDuriation = 100;
                                Process.ID = 1240;
                            }
                        }
                        break;

                    case 1240:                        //拍片完後關燈，然後到下一個拍片的地方，就換萬子投射了… 鍵帽取像
                        if (Process.IsTimeup)
                        {
                            if (INI.ISUSEJUMBOSERVER)
                            {
                                //if (MACHINE.PLCIO.ASMIn)
                                //if (!MACHINE.PLCIO.TopLight && !MACHINE.PLCIO.MylarLight && !MACHINE.PLCIO.AroundLight)
                                {
                                    if (isTestTimer)
                                        TestTimeTemp(Process.ID);

#if OPT_USE_THROUGH
                                    Universal.M_JUMBOCLIENT.RESULT_EVO.IsKHCComplete = false;

#endif

                                    JumboCalibrateProcess.Start("ONLINE");
                                    Process.NextDuriation = 10;
                                    Process.ID = 1250;
                                }
                            }
                            else
                            {
                                if (isTestTimer)
                                    TestTimeTemp(Process.ID);

                                Process.NextDuriation = 10;
                                Process.ID = 1250;
                            }
                        }
                        break;
                    case 1250:                        //鍵高測試結束 
                        if (Process.IsTimeup)
                        {
                            if (!JumboCalibrateProcess.IsOn || !INI.ISUSEJUMBOSERVER)
                            {
                                if (isTestTimer)
                                    TestTimeTemp(Process.ID);

                                MACHINE.PLCIO.ASMOut = true;

                               

                                //主要來自于自動跑線 門off 就是上去 
                                switch (TestMethod)
                                {
                                    case TestMethodEnum.IO:

                                        //Test Complete Open Light
                                        MACHINE.PLCIO.TopLight = true;
                                        //MACHINE.PLCIO.MylarLight = true;
                                        MACHINE.PLCIO.AroundLight = true;

                                        MACHINE.PLCIO.OpDoor = false;

                                        break;
                                        
                                }

                                Process.NextDuriation = 0;
                                Process.ID = 1260;
                            }
                        }
                        break;
                    case 1260:
                        if (Process.IsTimeup)
                        {
                            if (isTestTimer)
                                TestTimeTemp(Process.ID);

                            //if (MACHINE.PLCIO.ASMOut)
                            {
                                //取圖后 馬達到op拿料的位置
                                if (!Universal.SCREENPOINTS.IsNoUseMotion)
                                {
                                    switch (TestMethod)
                                    {
                                        case TestMethodEnum.BUTTON:

                                            MACHINE.GoXReadyPosition();

                                            break;
                                        case TestMethodEnum.IO:

                                            MACHINE.GoXPosition(GeneralPositions[0]);

                                            break;
                                    }
                                }

                                if (IsFristSetExposure)
                                    IsFristSetExposure = false;

                                string fristExposure = INI.FRAMEEXPOSURE;
                                CCDCollection.SetExposure(fristExposure);
                                string[] strs = INI.FRAMEEXPOSURE.Split(';');
                                //string secondExposure = "0#70;1#70;2#70;3#70;4#70";
                                CCDCollection.SetExposure(strs[0]);
                                //CCDCollection.SetExposure(fristExposure);
                            }

                            Process.NextDuriation = 0;
                            Process.ID = 1270;
                        }
                        break;
                    case 1270:
                        if (Process.IsTimeup)
                        {
                            //if (MACHINE.ISAllOnSite())
                            {
                                if (isTestTimer)
                                    TestTimeTemp(Process.ID);

                                Process.NextDuriation = 100;
                                Process.ID = 3011; //进入组合结果
                                //Process.ID = 3011; //进入组合结果
                            }
                        }
                        break;

#endregion

                    case 20:                        //離線測試
                        if (Process.IsTimeup)       //offline test
                        {
                            if (isTestTimer)
                                TestTimeTemp(Process.ID);

                            //Testms[0] = TestTimer.msDuriation;
                            //TestTimer.Cut();

                            //if (!IsNoUseCCD)
                            //    OnTrigger(ResultStatusEnum.COUNTSTART);

                            OnEnvTrigger(ResultStatusEnum.CHANGEENVDIRECTORY, EnvIndex, PageOPTypeEnum.P00.ToString());

                            //Testms[1] = TestTimer.msDuriation;
                            //TestTimer.Cut();

                            CCDCollection.GetImageFX("000,001,002,006");

                            //Testms[1] = TestTimer.msDuriation;
                            //TestTimer.Cut();

                            FillProcessImage("000,001,002,006");
                            //OnEnvTrigger(ResultStatusEnum.SAVEDEBUGRAW, EnvIndex, "000,001,002,006:");

                            //取得Compound 在這個 ENV 裏的資料
                            AlbumWork.CPD.CollectRUNVIEWData(AlbumWork, AlbumWork.ENVList[EnvIndex].No);

                            if (IsNoUseCCD)
                            {
                                //另開綫程用來測試字符
                                System.Threading.Thread m_thread = new System.Threading.Thread(new System.Threading.ThreadStart(_mainwork));
                                m_thread.Start();

                                //AlbumWork.A08_RunProcess(PageOPTypeEnum.P00);
                                //AlbumWork.FillRunStatus(RunStatusCollection);
                            }

#region KHC DEBUG
                            //string str_HeightPath = LastDirPath + "\\000\\HEIGHT\\";
                            //Bitmap bmpDebugHeight = new Bitmap(1, 1);
                            //int itmp = 0;
                            //while (itmp < 6)
                            //{
                            //    bmpDebugHeight.Dispose();
                            //    JzTools.GetBMP(str_HeightPath + "\\THH" + itmp.ToString() + Universal.GlobalImageTypeString, ref bmpDebugHeight);
                            //    KHCCollection.SetRealTimeBmp(itmp, bmpDebugHeight);

                            //    itmp++;
                            //}

                            //Old Use Service
                            try
                            {
                                int iJumboIndex = 0;
                                Bitmap bmpJumbo = new Bitmap(1, 1);
                                while (iJumboIndex < INI.JUMBO_SERVER_CAM_COUNT)
                                {
                                    //抓取測試圖给鍵高機
                                    bmpJumbo.Dispose();
                                    bmpJumbo = new Bitmap(1, 1);
                                    JzTools.GetBMP(LastDirPath + "\\000\\HEIGHT\\THH" + iJumboIndex.ToString() + Universal.GlobalImageTypeString, ref bmpJumbo);
                                    //bmpJumbo.Save(m_running_files + "\\THH" + (iJumboIndex).ToString() + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                                    //byte[] _msBmp = JzTools.GetByteBmp(bmpJumbo);
                                    //IXCONNECTJUMBO.SetKHCOnlineBmp(iJumboIndex, _msBmp);

#if (OPT_USE_THROUGH)
                                    //UniversalThrough.JUMBO_RESULT_EVO.SetRealtimeClientBmp(iJumboIndex, bmpJumbo);
                                    //JUMBORESULT.SetRealtimeClientBmp(iJumboIndex, bmpJumbo);
                                    Universal.M_JUMBOCLIENT.RESULT_EVO.SetRealtimeClientBmp(iJumboIndex, bmpJumbo);
#else
                                    IXCONNECTJUMBO.FileStartPath = LastDirPath + "\\000\\HEIGHT\\";
#endif
                                    iJumboIndex++;
                                }
                                IXCONNECTJUMBO.Start();

                            }
                            catch (Exception ex)
                            {
                                JetEazy.LoggerClass.Instance.WriteException(ex);
                            }
#endregion

                            try
                            {
                                int iCrystalIndex = 0;
                                while (iCrystalIndex < INI.CRYSTAL_SERVER_CAM_COUNT)
                                {
                                    Bitmap bmpCrystal = new Bitmap(CCDCollection.GetBMP(iCrystalIndex, false));
                                    //byte[] _msBmp = JzTools.GetByteBmp(bmpCrystal);
                                    //IXCONNECTCRYSTAL.SetDTOnlineBmp(iCrystalIndex, _msBmp);
                                    //IXCONNECTCRYSTAL.SetOnlineBmp(iCrystalIndex, _msBmp);
                                    //bmpCrystal.Save(m_running_files + "P00-" + iCrystalIndex.ToString("000") + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                                    //bmpCrystal.Dispose();

#if (OPT_USE_THROUGH_CRYSTAL)
                                    //UniversalThrough.JUMBO_RESULT_EVO.SetRealtimeClientBmp(iJumboIndex, bmpJumbo);
                                    //JUMBORESULT.SetRealtimeClientBmp(iJumboIndex, bmpJumbo);
                                    Universal.M_CRYSTALCLIENT.RESULT.SetRealtimeCrystalClientDTBmp(iCrystalIndex, bmpCrystal);
                                    Universal.M_CRYSTALCLIENT.RESULT.SetRealtimeCrystalClientBmp(iCrystalIndex, bmpCrystal);
                                    bmpCrystal.Dispose();
#else
                                    //IXCONNECTJUMBO.FileStartPath = LastDirPath + "\\000\\HEIGHT\\";
#endif

                                    iCrystalIndex++;
                                }

                                //IXCONNECTCRYSTAL.CrystalFileStartPath = LastDirPath + "\\000\\";
                                IXCONNECTCRYSTAL.CrystalStart();
                            }
                            catch (Exception ex)
                            {
                                JetEazy.LoggerClass.Instance.WriteException(ex);
                            }



                            //Testms[2] = TestTimer.msDuriation;
                            //TestTimer.Cut();

                            Process.NextDuriation = 50;
                            Process.ID = 3011;
                        }
                        break;
                    case 3010:
                        if (Process.IsTimeup)
                        {
                            if (isTestTimer)
                                TestTimeTemp(Process.ID);

                            TestTimer.Cut();

                            //if (IsNoUseCCD)
                            //{
                            //    OnTrigger(ResultStatusEnum.COUNTSTART);
                            //    //New Use KHC Test
                            //    //KHCCollection.FastCalculateSub();
                            //}

                            //鍵高機資料-參數名稱-國別
                            //string strRecord = KHCCollection.GetKHCReport() + Universal.SeperateCharG +
                            //                                 KHCCollection.GetRcpName() + Universal.SeperateCharG +
                            //                                 KHCCollection.GetRcpVer();

                            //OnTriggerOP(ResultStatusEnum.RECORDHEIGHTREPORT, strRecord);

                            Process.NextDuriation = 0;
                            Process.ID = 3011;
                        }
                        break;

#region 組合多方結果
                    case 3011:
                        if (Process.IsTimeup)
                        {
                            if (INI.ISUSEJUMBOSERVER)
                            {
                                try
                                {
                                    if (IXCONNECTJUMBO.KHCComplete)
                                    {
                                        if (isTestTimer)
                                            TestTimeTemp(Process.ID);

                                        //System.Threading.Thread.Sleep(100);
                                        foreach (CPDItemClass cpditem in AlbumWork.CPD.CPDItemList)
                                        {
                                            string[] strs = cpditem.NORMALPara.RelatePA.Split(':');
                                            if (strs.Length >= 2)
                                            {
                                                if (strs[1] == "KHC" || strs[1].IndexOf("KHC") == 0)
                                                {
#if OPT_USE_THROUGH
                                    cpditem.bmpITEMRUN = new Bitmap(Universal.M_JUMBOCLIENT.RESULT_EVO.bmpKHCResult);

#else


                                                    byte[] BitmapBytes = IXCONNECTJUMBO.GetKHCResultBmp();
                                                    if (BitmapBytes.Length > 0)
                                                    {
                                                        MemoryStream MS = new MemoryStream(BitmapBytes, false);
                                                        cpditem.bmpITEMRUN = (Bitmap)Image.FromStream(MS);
                                                    }

                                                    //cpditem.bmpITEMRUN = new Bitmap(IXCONNECTJUMBO.GetKHCResultBmp);
#endif
                                                }
                                            }
                                        }

                                        Process.NextDuriation = 50;
                                        Process.ID = 3012;
                                    }
                                }
                                catch(Exception ex)
                                {
                                    JetEazy.LoggerClass.Instance.WriteException(ex);
                                    Process.NextDuriation = 50;
                                    Process.ID = 3012;
                                }
                            }
                            else
                            {
                                Process.NextDuriation = 50;
                                Process.ID = 3012;
                            }
                        }
                        break;
                    case 3012:
                        if (Process.IsTimeup)
                        {
                            if (INI.ISUSECRYSTALSERVER)
                            {
                                try
                                {
                                    if (IXCONNECTCRYSTAL.KGMComplete)
                                    {
                                        if (isTestTimer)
                                            TestTimeTemp(Process.ID);

                                        //System.Threading.Thread.Sleep(100);
                                        foreach (CPDItemClass cpditem in AlbumWork.CPD.CPDItemList)
                                        {
                                            string[] strs = cpditem.NORMALPara.RelatePA.Split(':');
                                            if (strs.Length >= 2)
                                            {
                                                if (strs[1] == "KGM" || strs[1].IndexOf("KGM") == 0)
                                                {
#if OPT_USE_THROUGH_CRYSTAL
                                                    cpditem.bmpITEMRUN = new Bitmap(Universal.M_CRYSTALCLIENT.RESULT.bmpKGMResult);
#else
                                                    byte[] BitmapBytes = IXCONNECTCRYSTAL.GetKGMResultBmp();
                                                    if (BitmapBytes.Length > 0)
                                                    {
                                                        MemoryStream MS = new MemoryStream(BitmapBytes, false);
                                                        cpditem.bmpITEMRUN = (Bitmap)Image.FromStream(MS);
                                                    }
#endif

                                                    //cpditem.bmpITEMRUN = new Bitmap(IXCONNECTCRYSTAL.GetKGMResultBmp());
                                                }
                                            }
                                        }

                                        Process.NextDuriation = 50;
                                        Process.ID = 3013;
                                    }
                                }
                                catch(Exception ex)
                                {
                                    JetEazy.LoggerClass.Instance.WriteException(ex);
                                    Process.NextDuriation = 50;
                                    Process.ID = 3013;
                                }
                            }
                            else
                            {
                                Process.NextDuriation = 50;
                                Process.ID = 3013;
                            }
                        }
                        break;
                    case 3013:
                        if (Process.IsTimeup)
                        {
                            if (m_ismainworkcomplete)
                            {
                                if (isTestTimer)
                                    TestTimeTemp(Process.ID);

                                Process.NextDuriation = 50;
                                Process.ID = 30;
                            }
                        }
                        break;
#endregion

                    case 30:
                        if (Process.IsTimeup)
                        {
                            if (isTestTimer)
                                TestTimeTemp(Process.ID);

                            //Testms[3] = TestTimer.msDuriation;
                            //ADD GAARA Every Key Find Result from Server.

                            Testms[0] = TestTimer.msDuriation;

                            IsPass = RunStatusCollection.NGCOUNT == 0;

                            if (INI.ISUSEJUMBOSERVER)
                                IsPass &= IXCONNECTJUMBO.IsPass;

                            if (INI.ISUSECRYSTALSERVER)
                                IsPass &= IXCONNECTCRYSTAL.IsPass;

                            if (IsPass)
                            {
                                EnvIndex++;

                                if (EnvIndex < AlbumWork.ENVCount)
                                {
                                    ResetData(EnvIndex);

                                    Process.NextDuriation = 50;
                                    Process.ID = 10;

                                    return;
                                }
                            }
                            else
                            {
                                OnEnvTrigger(ResultStatusEnum.SAVENGRAW, EnvIndex, "-1");
                            }

                            ////主要來自于自動跑線 門off 就是上去 
                            //switch (TestMethod)
                            //{
                            //    case TestMethodEnum.IO:

                            //        MACHINE.PLCIO.OpDoor = false;

                            //        break;
                            //}

                            Process.NextDuriation = 50;
                            Process.ID = 40;
                        }
                        break;
                    case 40:
                        if (Process.IsTimeup)
                        {
                            if (isTestTimer)
                                TestTimeTemp(Process.ID);

                            Process.Stop();

                            if (IsPass)
                            {
                                OnTrigger(ResultStatusEnum.CALPASS);
                            }
                            else
                            {
                                OnTrigger(ResultStatusEnum.CALNG);
                            }

                            OnTrigger(ResultStatusEnum.COUNTEND);
                            OnTrigger(ResultStatusEnum.CALEND);


                            JzTools.CGOperate();

                            //主要來自于自動跑線 門off 就是上去 然後給PLC測試完成信號
                            switch (TestMethod)
                            {
                                case TestMethodEnum.IO:

                                    //if(MACHINE.PLCIO.IsDoorUP)
                                    MACHINE.PLCIO.PcToPlcCompleteSensor = true;

                                    break;
                            }
                        }
                        break;
                }
            }
        }

        void TestTimeTemp(int ID, string IDname = "")
        {
            watch.Stop();
            string timerTemp = IDname + ID + ": " + watch.ElapsedMilliseconds + "ms";
            watch.Reset();
            watch.Start();

            string logpath = @"D:\log\LogTimes\" + JzTimes.DateSerialString;
            string logname_HH = "TestTimer" + DateTime.Now.ToString("yyyyMMdd_HH") + ".txt";

            if (!Directory.Exists(logpath))
                Directory.CreateDirectory(logpath);

            //StreamWriter writer = new StreamWriter("D:\\TestTimer.txt", true, Encoding.UTF8);
            StreamWriter writer = new StreamWriter(logpath + "\\" + logname_HH, true, Encoding.UTF8);
            writer.WriteLine(timerTemp);
            writer.Close();
        }

        /// <summary>
        /// 主綫程測試
        /// </summary>
        private void _mainwork()
        {
            m_ismainworkcomplete = false;

            //FillProcessImage(INI.CRYSTAL_SERVER_CAM_RELATION + ",009");
            //OnEnvTrigger(ResultStatusEnum.SAVEDEBUGRAW, EnvIndex, INI.CRYSTAL_SERVER_CAM_RELATION + ",009:");

            ////取得Compound 在這個 ENV 裏的資料
            //AlbumWork.CPD.CollectRUNVIEWData(AlbumWork, AlbumWork.ENVList[EnvIndex].No);

            AlbumWork.A08_RunProcess(PageOPTypeEnum.P00);
            AlbumWork.FillRunStatus(RunStatusCollection);
            m_ismainworkcomplete = true;

            //JzTools.CGOperate();
        }
        public override void FillProcessImage()
        {
            int i = 0;

            EnvClass env = AlbumWork.ENVList[EnvIndex];

            foreach (PageClass page in env.PageList)
            {
                page.SetbmpRUN(PageOPTypeEnum.P00, CCDCollection.GetBMP(i, false));
                i++;
            }
        }
        public void FillProcessImage(string opstr)
        {
            EnvClass env = AlbumWork.ENVList[EnvIndex];

            if (Universal.IsMultiThread)
            {
                Parallel.ForEach<PageClass>(env.PageList, page =>
                {
                    //Thread.Sleep(200);
                    if ((opstr + ",").IndexOf(page.No.ToString(PageClass.ORGPAGENOSTRING) + ",") > -1)
                    {
                        //page.SetbmpRUN(PageOPTypeEnum.P00, CCDCollection.GetBMP(i, false));
                        page.SetbmpRUN(PageOPTypeEnum.P00, CCDCollection.GetBMP(page.CamIndex, false));

                    }
                });
            }
            else
            {
                int i = 0;
                foreach (PageClass page in env.PageList)
                {
                    if ((opstr + ",").IndexOf(page.No.ToString(PageClass.ORGPAGENOSTRING) + ",") > -1)
                    {
                        //page.SetbmpRUN(PageOPTypeEnum.P00, CCDCollection.GetBMP(i, false));
                        page.SetbmpRUN(PageOPTypeEnum.P00, CCDCollection.GetBMP(page.CamIndex, false));

                    }

                    i++;
                }
            }
        }
        public void FillProcessImage(string opstr, Bitmap bmp)
        {
            int i = 0;

            EnvClass env = AlbumWork.ENVList[EnvIndex];

            foreach (PageClass page in env.PageList)
            {
                if ((opstr + ",").IndexOf(page.No.ToString(PageClass.ORGPAGENOSTRING) + ",") > -1)
                    page.SetbmpRUN(PageOPTypeEnum.P00, bmp);

                i++;
            }
        }
        public override void ResetData(int operationindex)
        {
            if (operationindex == -1)
            {
                AlbumWork.ResetRunStatus();

                EnvIndex = 0;
                AlbumWork.SetEnvRunIndex(EnvIndex);

                RunStatusCollection.Clear();

                SetDelayTime();
                SetSaveDirectory(Universal.DEBUGRAWPATH);

            }
            else
            {
                EnvIndex = operationindex;
                AlbumWork.SetEnvRunIndex(EnvIndex);
            }
        }
        public void SaveHeightImage(string savetostr, EnvClass saveenv, string savestring, PageOPTypeEnum pageoptype)
        {
            int i = 0;
            string savepath = DebugSavePath + "\\" + DebugRecipeSaveName;

            if (!Directory.Exists(savepath))
                Directory.CreateDirectory(savepath);

            savepath = savepath + "\\" + savetostr + "\\" + saveenv.No.ToString(EnvClass.ORGENVNOSTRING) + "\\HEIGHT";

            if (!Directory.Exists(savepath))
                Directory.CreateDirectory(savepath);

            foreach (PageClass page in saveenv.PageList)
            {
                page.SaveHEIGHTBMP(savepath, savestring, pageoptype);
            }
        }

        public void StartCalibration(AlbumClass albumwork, CCDCollectionClass ccdcollection, string strTest)
        {
            AlbumWork = albumwork;
            CCDCollection = ccdcollection;

            switch (strTest)
            {
                case "Local":
                    JumboCalibrateProcess.Start("Local");
                    break;
                case "0":
                case "1":
                    JumboCalibrateProcess.Start("");
                    break;
                case "2":
                    JumboCalibrateProcess.Start("ONLINE");
                    break;
                case "Crystal_Online":
                    CrystalProcess.Start("CRYSTAL_ONLINE");
                    break;
                case "Crystal_Reget":
                    CrystalProcess.Start("");
                    break;
            }
        }

        void JumboCalibrateTick()
        {
            ProcessClass Process = JumboCalibrateProcess;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:
                        if (isTestTimer)
                            TestTimeTemp(Process.ID, "Jumbo ");

                        GeneralPosition = AlbumWork.ENVList[0].GeneralPosition;
                        GeneralPositions = GeneralPosition.Split(';');
                        GeneralPositionIndex = 1;
                        JumboOffline = INI.JUMBO_SERVER_CAM_COUNT;

                        if (isTestTimer)
                            TestTimeTemp(Process.ID, "JumboSetPosition ");

                        Process.NextDuriation = 50;
                        Process.ID = 10;

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (isTestTimer)
                                TestTimeTemp(Process.ID, "Jumbo ");

                            if (!Universal.SCREENPOINTS.IsNoUseMotion)
                                MACHINE.GoXPosition(GeneralPositions[GeneralPositionIndex]);

                            GeneralPositionIndex++;

                            PGList.Clear();
                            foreach (Point pt in Universal.SCREENPOINTS.m_JzScreenList[INI.JUMBO_SERVER_CAM_COUNT - 1].PointList)
                            {
                                PointF ptf = MSR.ViewToWorld(pt);
                                PGList.Add(new Point((int)ptf.X, (int)ptf.Y));
                            }
                            Universal.SCREENPOINTS.m_showmypoints.DrawMyPaints(PGList);

                            if (isTestTimer)
                                TestTimeTemp(Process.ID, "Jumbo Go");

                            Process.NextDuriation = 380;
                            Process.ID = 20;
                            isrunok = true;
                        }
                        break;
                    case 11:
                        if (Process.IsTimeup)
                        {
                            Process.NextDuriation = 100;
                            Process.ID = 20;
                        }
                        break;
                    case 20:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.ISAllOnSite() && isrunok)
                            {
                                iRpiReviced = Universal.SCREENPOINTS.m_showmypoints.DrawMyPaints(PGList);

                                if (isTestTimer)
                                    TestTimeTemp(Process.ID, "Jumbo ");

                                Process.NextDuriation = 238;// DelayTime[0];
                                Process.ID = 25;
                            }
                        }
                        break;
                    case 25:
                        if (Process.IsTimeup && isrunok && iRpiReviced ==0)
                        {
                            if (isTestTimer)
                                TestTimeTemp(Process.ID, "Jumbo ");

                            CCDCollection.GetImageFX(INI.JUMBO_SERVER_CAM_RELATION);

                            if (isTestTimer)
                                TestTimeTemp(Process.ID, "JumboGetImageStop ");
                            JumboOffline--;

                            if (GeneralPositions[GeneralPositionIndex].Trim() == "")
                            {
                                //CCDCollection.GetImageFX(INI.JUMBO_SERVER_CAM_RELATION);
                                #region 这是最后一次抓图
                                Bitmap bmpDT = CCDCollection.GetCombineBMPEX(INI.JUMBO_SERVER_CAM_RELATION);
                                if (Universal.IsSaveRaw)
                                {
                                    FillProcessImage(INI.JUMBO_SERVER_CAM_RELATION.Split(',')[0], bmpDT);
                                    OnEnvTrigger(ResultStatusEnum.SAVEHIGHTRAW, EnvIndex, INI.JUMBO_SERVER_CAM_RELATION.Split(',')[0] + "," + (GeneralPositionIndex - 2).ToString());
                                }
                                switch (Process.RelateString)
                                {
                                    case "Local":
                                        //string strFilePath = IXCONNECTJUMBO.FileGetAndAnalyzePath;//鍵高參數路徑
                                                                                                  //string strFilePath = KHCCollection.KHC_FileGetAndAnalyzePath;
                                        bmpDT.Save(m_running_files + "\\THH" + (JumboOffline).ToString() + ".bmp", ImageFormat.Bmp);
                                        break;
                                    case "":

                                        //Old Use Service
                                        string strFilePath = IXCONNECTJUMBO.FileGetAndAnalyzePath;//鍵高參數路徑
                                                                                                  //string strFilePath = KHCCollection.KHC_FileGetAndAnalyzePath;
                                        bmpDT.Save(strFilePath + "\\THH" + (JumboOffline).ToString() + ".bmp", ImageFormat.Bmp);
                                        //bmp.Save(strFilePath + "\\THH" + (GeneralPositionIndex - 2).ToString() + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                                        break;
                                    case "ONLINE":
                                        m_running_files = @"D:\TMP_HEIGHT_CHECK\";
                                        if (!Directory.Exists(m_running_files))
                                            Directory.CreateDirectory(m_running_files);

                                        //bmp.Save(m_running_files + "THH" + (GeneralPositionIndex - 2).ToString() + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                                        //bmp.Save(m_running_files + "H" + (GeneralPositionIndex - 2).ToString() + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

#if (OPT_USE_THROUGH)
                                        Universal.M_JUMBOCLIENT.RESULT_EVO.SetRealtimeClientBmp(JumboOffline, bmpDT);
                                        //bmpDT.Save(m_running_files + "THH" + (JumboOffline - 1).ToString() + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                                        //Universal.M_JUMBOCLIENT.RESULT_EVO.SetRealtimeClientBmp(GeneralPositionIndex - 2, bmp);
#else
                                        bmpDT.Save(m_running_files + "THH" + (JumboOffline).ToString() + ".bmp", ImageFormat.Bmp);

                                        //IXCONNECTJUMBO.GetRealtime(m_cam_mode, JumboOffline);
#endif

                                        break;
                                }
                                bmpDT.Dispose();
#endregion

                                PGList.Clear();
                                Universal.SCREENPOINTS.m_showmypoints.DrawMyPaints(PGList);
                                switch (Process.RelateString)
                                {
                                    case "Local":
                                    case "":

                                        ////End Of KeyHeight Check
                                        if (!Universal.SCREENPOINTS.IsNoUseMotion)
                                        {
                                            //MACHINE.GoXPosition(GeneralPositions[0]);
                                            MACHINE.GoXReadyPosition();
                                        }

                                        break;
                                    case "ONLINE":

                                        IXCONNECTJUMBO.FileStartPath = m_running_files;
                                        IXCONNECTJUMBO.Start();

                                        break;
                                }

                                Process.NextDuriation = 100;
                                Process.ID = 30;
                                ///
                            }
                            else
                            {
                                //PGList.Clear();
                                //foreach (Point pt in Universal.SCREENPOINTS.m_JzScreenList[JumboOffline - 1].PointList)
                                //{
                                //    PointF ptf = MSR.ViewToWorld(pt);
                                //    PGList.Add(new Point((int)ptf.X, (int)ptf.Y));
                                //}
                                //Universal.SCREENPOINTS.m_showmypoints.DrawMyPaints(PGList);

                                Process.NextDuriation = 100;
                                Process.ID = 3002;
                            }
                        }
                        break;
                    case 2601:
                        if (Process.IsTimeup)
                        {

                        }
                            break;
                    case 3001:
                        if (Process.IsTimeup)
                        {
                            if (isTestTimer)
                                TestTimeTemp(Process.ID, "Jumbo ");

                            MACHINE.GoXPosition(GeneralPositions[GeneralPositionIndex]);
                            GeneralPositionIndex++;
                            //JumboOffline--;

                            Process.NextDuriation = 100;
                            Process.ID = 20;
                        }
                        break;
                    case 3002:
                        if (Process.IsTimeup)
                        {
                            if (isTestTimer)
                                TestTimeTemp(Process.ID, "Jumbo ");

                            string strThreadPara = JumboOffline.ToString() + "#" + Process.RelateString;
                            isrunok = false;
                            System.Threading.Thread thread = new System.Threading.Thread(ThrJumboImage);
                            thread.Start(strThreadPara);

                            //ThrJumboImage(strThreadPara);

                            if (isTestTimer)
                                TestTimeTemp(Process.ID, "Jumbo ");

                            Process.NextDuriation = 0;
                            Process.ID = 3001;
                        }
                        break;
                    case 30:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.ISAllOnSite())
                            {
                                if (isTestTimer)
                                    TestTimeTemp(Process.ID, "Jumbo ");

                                Process.Stop();

                                switch (Process.RelateString)
                                {
                                    case "Local":
                                    case "":
                                        OnTrigger(ResultStatusEnum.CALIBRATEEND);
                                        break;
                                    case "ONLINE":

                                        break;
                                }
                            }
                        }
                        break;
                }
            }
        }
        bool isrunok = false;
        void ThrJumboImage(object obj)
        {
            string strGetPara = (string)obj;
            string[] strs = strGetPara.Split('#');
            int myJumboOffline = int.Parse(strs[0]);
            string strRelate = strs[1];
            //int myJumboOffline = (int)obj;

#region 
            //OnEnvTrigger(ResultStatusEnum.SAVEHIGHTRAW, EnvIndex, INI.JUMBO_SERVER_CAM_RELATION);
            Bitmap bmp = CCDCollection.GetCombineBMPEX(INI.JUMBO_SERVER_CAM_RELATION);
            if (Universal.IsSaveRaw)
            {
                FillProcessImage(INI.JUMBO_SERVER_CAM_RELATION.Split(',')[0], bmp);
                OnEnvTrigger(ResultStatusEnum.SAVEHIGHTRAW, EnvIndex, INI.JUMBO_SERVER_CAM_RELATION.Split(',')[0] + "," + (GeneralPositionIndex - 2).ToString());
            }

            m_running_files = @"D:\TMP_HEIGHT_CHECK\";
            if (!Directory.Exists(m_running_files))
                Directory.CreateDirectory(m_running_files);

            switch (strRelate)
            {
                case "Local":
                    //Old Use Service
                    //string strFilePath = IXCONNECTJUMBO.FileGetAndAnalyzePath;//鍵高參數路徑
                                                                              //string strFilePath = KHCCollection.KHC_FileGetAndAnalyzePath;
                    bmp.Save(m_running_files + "\\THH" + (myJumboOffline).ToString() + ".bmp", ImageFormat.Bmp);
                    //bmp.Save(strFilePath + "\\THH" + (GeneralPositionIndex - 2).ToString() + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                    break;

                case "":

                    //Old Use Service
                    string strFilePath = IXCONNECTJUMBO.FileGetAndAnalyzePath;//鍵高參數路徑
                                                                              //string strFilePath = KHCCollection.KHC_FileGetAndAnalyzePath;
                    bmp.Save(strFilePath + "\\THH" + (myJumboOffline).ToString() + ".bmp", ImageFormat.Bmp);
                    //bmp.Save(strFilePath + "\\THH" + (GeneralPositionIndex - 2).ToString() + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);


                    break;
                case "ONLINE":

#if (OPT_USE_THROUGH)
                    Universal.M_JUMBOCLIENT.RESULT_EVO.SetRealtimeClientBmp(myJumboOffline, bmp);
                    //bmp.Save(m_running_files + "THH" + (JumboOffline - 1).ToString() + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                    //Universal.M_JUMBOCLIENT.RESULT_EVO.SetRealtimeClientBmp(GeneralPositionIndex - 2, bmp);
#else
                    bmp.Save(m_running_files + "THH" + (myJumboOffline).ToString() + ".bmp", ImageFormat.Bmp);

                    //IXCONNECTJUMBO.GetRealtime(m_cam_mode, myJumboOffline);
#endif

                    break;
            }

            //bmp.Dispose();
            //isrunok = true;

            PGList.Clear();
            foreach (Point pt in Universal.SCREENPOINTS.m_JzScreenList[JumboOffline - 1].PointList)
            {
                PointF ptf = MSR.ViewToWorld(pt);
                PGList.Add(new Point((int)ptf.X, (int)ptf.Y));
            }
            //Universal.SCREENPOINTS.m_showmypoints.DrawMyPaints(PGList);

            bmp.Dispose();
            isrunok = true;

            //JzTools.CGOperate();
            #endregion
        }
        void CrystalCalibrateTick()
        {
            ProcessClass Process = CrystalProcess;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:
                        switch (Process.RelateString)
                        {
                            case "AUTO":

                                if (IsFristSetExposure)
                                {
                                    Process.NextDuriation = 10;
                                    Process.ID = 6;
                                }
                                else
                                {
                                    Process.NextDuriation = 10;
                                    Process.ID = 6;
                                }

                                break;
                            default:

                                IsFristSetExposure = true;

                                string secondExposure = INI.FRAMEEXPOSURE;
                                //string secondExposure = "0#70;1#70;2#70;3#70;4#70";
                                CCDCollection.SetExposure(secondExposure);

                                MACHINE.PLCIO.TopLight = true;
                                MACHINE.PLCIO.MylarLight = false;
                                MACHINE.PLCIO.AroundLight = true;

                                Process.NextDuriation = 500;
                                Process.ID = 510;

                                break;
                        }
                        break;
                    case 510:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.PLCIO.TopLight && MACHINE.ISAllOnSite())
                            {
                                Process.NextDuriation = 500;
                                Process.ID = 6;
                            }
                        }
                        break;
                    case 6: //間隙第一次取圖
                        if (Process.IsTimeup)
                        {
                            if (isTestTimer)
                                TestTimeTemp(Process.ID,"CCDGetimageStart");
                            CCDCollection.GetImageFX(INI.CRYSTAL_SERVER_CAM_RELATION);
                            if (isTestTimer)
                                TestTimeTemp(Process.ID, "CCDGetimageStop");

                            //CAM0-CAM9 
                            //string secondExposure = INI.FRAMEEXPOSURE;
                            string secondExposure = INI.KEYCAPEXPOSURE;
                            //string secondExposure = "0#70;1#70;2#70;3#70;4#70";
                            CCDCollection.SetExposure(secondExposure);

                            //MACHINE.PLCIO.AroundLight = false;
                            MACHINE.PLCIO.TopLight = false;

                            if (isTestTimer)
                                TestTimeTemp(Process.ID, "SaveImageStart");

                            m_running_files = @"D:\TMP_HEIGHT_CHECK\";
                            if (!Directory.Exists(m_running_files))
                                Directory.CreateDirectory(m_running_files);

                            //IXCONNECTCRYSTAL.CrystalFileStartPath = m_running_files;
                            //int ixItem = 0;
                            //while (ixItem < INI.CRYSTAL_SERVER_CAM_COUNT)
                            Parallel.For(0, INI.CRYSTAL_SERVER_CAM_COUNT, ixItem =>
                            {
                                //抓取定位測試圖
                                Bitmap bmpCrystal = new Bitmap(CCDCollection.GetBMP(ixItem, false));

#if (OPT_USE_THROUGH_CRYSTAL)
                                Universal.M_CRYSTALCLIENT.RESULT.SetRealtimeCrystalClientDTBmp(ixItem, bmpCrystal);
                                bmpCrystal.Save(m_running_files + "DTP00-" + ixItem.ToString("000") + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
#else
                                bmpCrystal.Save(m_running_files + "P00-" + ixItem.ToString("000") + ".bmp", ImageFormat.Bmp);
                                //bmpCrystal.Save(m_running_files + "DTP00-" + ixItem.ToString("000") + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

#endif
                                bmpCrystal.Dispose();
                                ixItem++;
                            }
                            );
                            if (isTestTimer)
                                TestTimeTemp(Process.ID, "SaveImageStop");

                            Process.NextDuriation = 0;// DelayTime[0];
                            Process.ID = 7;
                        }
                        break;
                    case 7:
                        if (Process.IsTimeup)
                        {
                            //if (MACHINE.PLCIO.TopLight && MACHINE.ISAllOnSite())
                            {
                                string[] strs = INI.KEYCAPEXPOSURE.Split(';');
                                //string secondExposure = "0#70;1#70;2#70;3#70;4#70";
                                CCDCollection.SetExposure(strs[0]);

                                Process.NextDuriation = 238;
                                Process.ID = 10;
                            }
                        }
                        break;
                    case 10://間隙第二次取圖
                        if (Process.IsTimeup)
                        {
                            if (isTestTimer)
                                TestTimeTemp(Process.ID, "CCDGetimageStart");

                            CCDCollection.GetImageFX(INI.CRYSTAL_SERVER_CAM_RELATION);

                            if (isTestTimer)
                                TestTimeTemp(Process.ID, "CCDGetimageStop");

                            switch (Process.RelateString)
                            {
                                case "AUTO":
                                    MACHINE.PLCIO.MylarLight = true;
                                    break;
                            }
                            if (isTestTimer)
                                TestTimeTemp(Process.ID, "SaveImageStart");
                            //int ixItem = 0;
                            //while (ixItem < INI.CRYSTAL_SERVER_CAM_COUNT)
                            Parallel.For(0, INI.CRYSTAL_SERVER_CAM_COUNT, ixItem =>
                            {
                                //抓取定位測試圖
                                Bitmap bmpCrystal = new Bitmap(CCDCollection.GetBMP(ixItem, false));

                                //switch (Process.RelateString)
                                //{
                                //    case "AUTO":
#if (OPT_USE_THROUGH_CRYSTAL)
                                Universal.M_CRYSTALCLIENT.RESULT.SetRealtimeCrystalClientBmp(ixItem, bmpCrystal);
                                bmpCrystal.Save(m_running_files + "P00-" + ixItem.ToString("000") + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
#else
                                bmpCrystal.Save(m_running_files + "DTP00-" + ixItem.ToString("000") + ".bmp", ImageFormat.Bmp);

#endif

                                bmpCrystal.Dispose();
                                ixItem++;
                            }
                            );
                            if (isTestTimer)
                                TestTimeTemp(Process.ID, "SaveImageStop");

                            Process.NextDuriation = 10;
                            Process.ID = 20;

                        }
                        break;
                    case 20: //間隙測試
                        if (Process.IsTimeup)
                        {
                            //   if (MACHINE.ISAllOnSite())
                            {
                                switch (Process.RelateString)
                                {
                                    case "CRYSTAL_ONLINE":
                                    case "AUTO":
                                        //try
                                        //{
                                        //if (INI.ISUSECRYSTALSERVER)
                                        //{
                                        IXCONNECTCRYSTAL.CrystalFileStartPath = m_running_files;
                                        IXCONNECTCRYSTAL.CrystalStart();
                                        //}
                                        //}
                                        //catch (Exception ex)
                                        //{
                                        //    System.Diagnostics.Debug.WriteLine("CrystalCalibrateTick:" + ex.Message);
                                        //}

                                        Process.NextDuriation = 280;
                                        Process.ID = 50;

                                        break;
                                    default:

                                        IXCONNECTCRYSTAL.CrystalFileStartPath = m_running_files;

                                        Process.NextDuriation = 280;
                                        Process.ID = 50;
                                        break;
                                }
                            }
                        }
                        break;

#region OLD USE
                    /*
                case 30:
                    if (Process.IsTimeup)
                    {
                        if (!Universal.SCREENPOINTS.IsNoUseMotion)
                            MACHINE.GoXReadyPosition();

                        Process.NextDuriation = DelayTime[0];
                        Process.ID = 40;
                    }
                    break;
                case 40:
                    if (Process.IsTimeup)
                    {
                        if (MACHINE.ISAllOnSite())
                            Process.ID = 50;

                    }
                    break;
                    */
#endregion

                    case 50:
                        Process.Stop();
                        switch (Process.RelateString)
                        {
                            case "":
                                OnTrigger(ResultStatusEnum.CALIBRATEEND);
                                break;
                            case "AUTO":
                            case "CRYSTAL_ONLINE":

                                break;
                        }
                        break;
                }
            }
        }
    }
}
