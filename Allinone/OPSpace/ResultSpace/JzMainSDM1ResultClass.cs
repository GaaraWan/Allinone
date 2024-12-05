using Allinone.ControlSpace;
using Allinone.ControlSpace.MachineSpace;
using JetEazy;
using JetEazy.BasicSpace;
using JetEazy.ControlSpace;
using JetEazy.FormSpace;
using JetEazy.PlugSpace;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Allinone.OPSpace.ResultSpace
{
    public class JzMainSDM1ResultClass : GeoResultClass
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr _lopen(string lpPathName, int iReadWrite);
        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);
        public const int OF_READWRITE = 2;
        public const int OF_SHARE_DENY_NONE = 0x40;
        public readonly IntPtr HFILE_ERROR = new IntPtr(-1);

        bool m_IsEMC = false;
        bool m_IsReset = false;
        bool m_IsOneKey = false;

        string GeneralPosition = "";
        string[] GeneralPositions;
        int GeneralPositionIndex = 0;

        public string BARCODE = "";
        public string VER = "";
        bool IsGetImageStartOld = false;
        bool IsGetImageResetOld = false;
        bool IsGetTestStartOld = false;

        public string ARTWORKNAME = "";
        public string MODELNAME = "";
        public string ORGBARCODESTR = "";
        public string HOUSINGID = "";
        //   public string RELATECOLORSTR = "";
        public string SNSTARTOPSTR
        {
            get
            {
                return VER + "$" + ARTWORKNAME + "-" + RELATECOLORSTR;
            }
        }

        JzMainSDM1MachineClass MACHINE;

        JzToolsClass JzTools = new JzToolsClass();
        SoundPlayer PlayerPass = new SoundPlayer();
        SoundPlayer PlayerFail = new SoundPlayer();

        public JzMainSDM1ResultClass(Result_EA resultea, VersionEnum version, OptionEnum option, MachineCollectionClass machinecollection)
        {
            myResultEA = resultea;
            VERSION = version;
            OPTION = option;

            PlayerPass.SoundLocation = Universal.PlayerPASSPATH;
            PlayerFail.SoundLocation = Universal.PlayerFAILPATH;
            PlayerPass.Load();
            PlayerFail.Load();

            DUP = new DupClass();

            MACHINE = (JzMainSDM1MachineClass)machinecollection.MACHINE;
            MACHINE.TriggerAction += MACHINE_TriggerAction;
            

            MainProcess = new ProcessClass();
        }

        private void MACHINE_TriggerAction(MachineEventEnum machineevent, object obj = null)
        {
            switch (machineevent)
            {
                case MachineEventEnum.EMC:
                    m_IsEMC = true;
                    break;
                case MachineEventEnum.RESET:
                    m_IsReset = true;
                    break;
            }
        }

        void StopAllProcess(bool bCancel = false)
        {
            MainProcess.Stop();
            m_OnekeyGetImageProcess.Stop();
            m_ResetProcess.Stop();
        }

        private void CCDCollection_TriggerAction(string operationstr)
        {
            throw new NotImplementedException();
        }

        public override void SetPara(AlbumClass albumwork, CCDCollectionClass ccocollection)
        {
            base.SetPara(albumwork, ccocollection);
            AlbumWork = albumwork;
            if (AlbumWork != null && AlbumWork.CPD != null)
                AlbumWork.CPD.bmpOCRCheckErr = null;
            CCDCollection = ccocollection;
        }

        public override void GetStart(AlbumClass albumwork, CCDCollectionClass ccdcollection, TestMethodEnum testmethod, bool isnouseccd)
        {
            //if (MainProcess.IsOn)
            //{
            //    MainProcess.Stop();
            //    OnTrigger(ResultStatusEnum.FORECEEND);

            //    return;
            //}

            if (MainProcess.IsOn || m_OnekeyGetImageProcess.IsOn || m_ResetProcess.IsOn)
            {
                _LOG_MSG_ERR("START 重复启动");
                return;
            }
            if (MACHINE.PLCIO.IsEMC && !isnouseccd)
            {
                _LOG_MSG_ERR("急停中 无法测试");
                return;
            }

            OnTrigger(ResultStatusEnum.CALSTART);

            AlbumWork = albumwork;
            if (AlbumWork != null && AlbumWork.CPD != null)
                AlbumWork.CPD.bmpOCRCheckErr = null;
            CCDCollection = ccdcollection;

            TestMethod = testmethod;
            IsNoUseCCD = isnouseccd;

            ResetData(-1);

            MainProcess.Start();

            //switch (TestMethod)
            //{
            //    case TestMethodEnum.QSMCSF:
            //        if (INI.ISHIVECLIENT)
            //        {
            //            MACHINE.SetMachineState(MachineState.Running);
            //            if (MACHINE.GetCurrentMachineState == MachineState.Running)
            //                MACHINE.HIVECLIENT.Hiveclient_ConfigurationMap(BARCODE, "SF", INI.DATA_Program, INI.DATA_Building_Config, _get_qsmc_sndata_json());
            //        }
            //        break;
            //}

        }
        public override void Tick()
        {
            MainProcessTick();
            if (m_IsEMC)
            {
                m_IsEMC = false;
                StopAllProcess();

                _LOG_MSG_ERR("EMC 急停中");
            }
            if (m_IsReset)
            {
                _LOG_MSG("RESET 复位按下");
                m_IsReset = false;
                if (!MainProcess.IsOn && !m_OnekeyGetImageProcess.IsOn && !m_ResetProcess.IsOn && !MACHINE.PLCIO.IsEMC)
                {
                    _LOG_MSG("RESET 复位启动");
                    if (!Universal.IsNoUseCCD)
                        m_ResetProcess.Start();
                }
            }
            if (m_IsOneKey)
            {
                _LOG_MSG("ONEKEY 一键取像按下");
                m_IsOneKey = false;
                if (!MainProcess.IsOn && !m_OnekeyGetImageProcess.IsOn && !m_ResetProcess.IsOn && (Universal.IsNoUseCCD || !MACHINE.PLCIO.IsEMC))
                {
                    _LOG_MSG("ONEKEY 一键取像启动");
                    //if (!Universal.IsNoUseCCD)
                        m_OnekeyGetImageProcess.Start();
                }
            }
        }
        public override void GenReport()
        {

        }
        public override void SetDelayTime()
        {
            DelayTime[0] = INI.DELAYTIME;
        }

        JzTimes TestTimer = new JzTimes();
        int[] Testms = new int[100];
        DateTime m_input_time = DateTime.Now;
        int m_ProcessTmpID = 0;

        protected override void MainProcessTick()
        {
            switch (OPTION)
            {
                case OptionEnum.MAIN_SDM1:

                    MainSDM1Tick();

                    switch(Universal.CAMACT)
                    {
                        case CameraActionMode.CAM_MOTOR:
                        case CameraActionMode.CAM_MOTOR_MODE2:

                            OnekeyGetImageTick();
                            ResetTick();

                            break;
                    }

                    break;
            }


        }
        public override void FillProcessImage()
        {
            EnvClass env = AlbumWork.ENVList[EnvIndex];

            foreach (PageClass page in env.PageList)
            {
                page.SetbmpRUN(PageOPTypeEnum.P00, CCDCollection.GetBMP(page.CamIndex, false));
            }
        }
        public void FillProcessImageMotor()
        {
            EnvClass env = AlbumWork.ENVList[EnvIndex];

            foreach (PageClass page in env.PageList)
            {
                page.SetbmpRUN(PageOPTypeEnum.P00, CamActClass.Instance.GetImage(page.CamIndex));
            }
        }
        public void FillProcessImageMotorPageIndex(int index)
        {
            EnvClass env = AlbumWork.ENVList[EnvIndex];

            if (index >= env.PageList.Count || index < 0)
                return;

            PageClass page = env.PageList[index];
            page.SetbmpRUN(PageOPTypeEnum.P00, CCDCollection.GetBMP(0, false));
        }
        public void FillProcessImage(string opstr)
        {
            int i = 0;

            EnvClass env = AlbumWork.ENVList[EnvIndex];

            foreach (PageClass page in env.PageList)
            {
                if ((opstr + ",").IndexOf(page.No.ToString(PageClass.ORGPAGENOSTRING) + ",") > -1)
                    page.SetbmpRUN(PageOPTypeEnum.P00, CCDCollection.GetBMP(page.CamIndex, false));

                i++;
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

        void MainSDM1Tick()
        {
            ProcessClass Process = MainProcess;

            if (Process.IsOn)
            {
                if (m_ProcessTmpID != Process.ID)
                {
                    m_ProcessTmpID = Process.ID;
                    _LOG_MSG(Process.ID.ToString() + " MainSDM1Tick Running...");
                }

                switch (Process.ID)
                {
                    case 5:

                        LogProcessSwitch = true;
                        LogProcessIDTimer(Process.ID, "流程开始");
                        _LOG_MSG(Process.ID.ToString() + " MainSDM1Tick " + "流程开始");

                        MACHINE.PLCIO.Busy = true;
                        MACHINE.SetLight("1,1,1");

                        if (INI.IsCollectPictures)
                            Universal.MainX6_Path = "D:\\CollectPictures_" + Universal.OPTION.ToString() + "\\pictures\\" + JzTimes.DateSerialString;


                        TestTimer.Cut();
                        m_input_time = DateTime.Now;
                        OnEnvTrigger(ResultStatusEnum.CHANGEDIRECTORY, 0, "");
                        OnTrigger(ResultStatusEnum.COUNTSTART);
                        //把要檢測的東西放進去
                        FillOperaterString(RELATECOLORSTR);

                        Process.NextDuriation = 0;

                        if (IsNoUseCCD)
                            Process.ID = 20;
                        else
                            Process.ID = 10;


                        switch (Universal.CAMACT)
                        {
                            case CameraActionMode.CAM_MOTOR_MODE2:

                                GeneralPosition = AlbumWork.ENVList[0].GeneralPosition;
                                GeneralPositions = GeneralPosition.Split(';');
                                GeneralPositionIndex = 0;

                                ENVAnalyzePostion = new EnvAnalyzePostionSettings(GeneralPosition);
                                ENVAnalyzePostion.EnvAnalyzePostions();

                                CamActClass.Instance.SetStepCount(ENVAnalyzePostion.GetImagePostions.Length);//这里设定抓取图像的所有位置

                                CamActClass.Instance.StepCurrent = 0;

                                break;
                        }

                        Process.NextDuriation =  INI.MAINSDM1_GETSTART_DELAYTIME;
                        //Process.ID = 10;
                        _LOG_MSG(Process.ID.ToString() + " MainSDM1Tick 延时启动时间" + INI.MAINSDM1_GETSTART_DELAYTIME.ToString());


                        break;
                    case 10:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
                        if (Process.IsTimeup)
                        {
                            switch (Universal.CAMACT)
                            {
                                case CameraActionMode.CAM_MOTOR_MODE2:

                                    _LOG_MSG(Process.ID.ToString() + " MainSDM1Tick POS=" + ENVAnalyzePostion.GetImagePostions[GeneralPositionIndex].ToString());

                                    MACHINE.GoXPosition(ENVAnalyzePostion.GetImagePostions[GeneralPositionIndex].ToString());

                                    //_LOG_MSG(Process.ID.ToString() + " MainSDM1Tick POS=" + GeneralPositions[GeneralPositionIndex]);

                                    //MACHINE.GoXPosition(GeneralPositions[GeneralPositionIndex]);
                                    GeneralPositionIndex++;

                                    OnEnvTrigger(ResultStatusEnum.SETCAMLIGHT, EnvIndex, "ALL");
                                    Process.NextDuriation = 200;
                                    Process.ID = 10090;

                                    break;
                                case CameraActionMode.CAM_MOTOR:
                                    Process.NextDuriation = 500;
                                    Process.ID = 1030;
                                    break;
                                case CameraActionMode.CAM_STATIC:
                                    OnEnvTrigger(ResultStatusEnum.SETCAMLIGHT, EnvIndex, "ALL");
                                    Process.NextDuriation = 500;
                                    Process.ID = 1030;
                                    break;
                            }
                        }
                        break;


                    #region 马达模式2  
                    case 10090:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.IsOnSite() || Universal.IsNoUseMotor)
                            {
                                Process.NextDuriation = INI.MAINSD_GETIMAGE_DELAYTIME;
                                Process.ID = 10100;

                                LogProcessIDTimer(Process.ID, "取像延时=" + INI.MAINSD_GETIMAGE_DELAYTIME.ToString());

                            }
                        }
                        break;
                    case 10100:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.IsOnSite() || Universal.IsNoUseMotor)
                            {
                                //抓图
                                CCDCollection.GetImage();
                                //CCDCollection.GetImage();
                                if (CamActClass.Instance.StepCurrent >= CamActClass.Instance.StepCount)
                                    CamActClass.Instance.ResetStepCurrent();
                                FillProcessImageMotorPageIndex(CamActClass.Instance.StepCurrent);
                                //CamActClass.Instance.StepCurrent++;

                                CamActClass.Instance.SetImage(CCDCollection.GetBMP(0, false), CamActClass.Instance.StepCurrent);

                                Process.NextDuriation = 0;
                                Process.ID = 10110;

                                LogProcessIDTimer(Process.ID, "测试位置=" + CamActClass.Instance.StepCurrent.ToString());
                            }
                        }
                        break;
                    case 10110:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
                        if (Process.IsTimeup)
                        {

                            //计算线程
                            System.Threading.Thread thread_DL_Test = new System.Threading.Thread(DLCalPageIndex);
                            thread_DL_Test.Start(CamActClass.Instance.StepCurrent);

                            CamActClass.Instance.StepCurrent++;

                            if (CamActClass.Instance.StepCurrent < CamActClass.Instance.StepCount)
                            {
                                ////发送拍照完成信号
                                //System.Threading.Thread thread = new System.Threading.Thread(SendSignToPLCImageOK);
                                //thread.Start();

                                //m_DLGetImageOK.Start();

                                //IsGetImageStartOld = true;

                                Process.NextDuriation = 0;
                                Process.ID = 10;


                                LogProcessIDTimer(Process.ID, "取像完成 Send==>Sign ImageOK");
                            }
                            else
                            {

                                Process.NextDuriation = 0;
                                Process.ID = 10120;

                                MACHINE.GoXReadyPosition();

                                //马达回待命位置
                                LogProcessIDTimer(Process.ID, "取像完成 Motor Ready");
                            }

                        }
                        break;
                    case 10120:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
                        if (Process.IsTimeup)
                        {
                            //最后一次抓完
                            //if (AlbumWork.GetPageTestState(CamActClass.Instance.StepCurrent - 1))
                            if (AlbumWork.GetAllPageTestComplete())
                            {
                                Process.NextDuriation = 0;
                                Process.ID = 1030;

                                LogProcessIDTimer(Process.ID, "所有页面测试完成");
                                _LOG_MSG(Process.ID.ToString() + " MainSDM1Tick " + "所有页面测试完成");
                            }
                        }
                        break;
                    case 10130:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
                        if (Process.IsTimeup)
                        {
                            ////等待信号抓图
                            //if (MACHINE.PLCIO.IsGetImage)
                            //{
                            //    Process.NextDuriation = 0;
                            //    Process.ID = 10;
                            //}

                            //if (!MACHINE.PLCIO.GetImageOK && !m_DLGetImageOK.IsOn)
                            //{
                            //    //Trigger Start
                            //    bool isGetImageStart = MACHINE.PLCIO.IsGetImage;

                            //    if (isGetImageStart && IsGetTestStartOld != isGetImageStart)
                            //    {
                            //        Process.NextDuriation = 0;
                            //        Process.ID = 10;

                            //        LogProcessIDTimer(Process.ID, "获取取像信号 Rev==>Sign GetImage");
                            //    }
                            //    IsGetTestStartOld = isGetImageStart;
                            //}

                        }
                        break;

                    #endregion


                    case 1030:                        //拍片完後關燈
                        if (Process.IsTimeup)
                        {

                            switch (Universal.CAMACT)
                            {
                                case CameraActionMode.CAM_MOTOR:
                                    FillProcessImageMotor();
                                    break;
                                case CameraActionMode.CAM_STATIC:
                                    CCDCollection.GetImage();
                                    FillProcessImage();
                                    break;
                            }

                            OnEnvTrigger(ResultStatusEnum.SAVEDEBUGRAW, EnvIndex, "-1");

                            if (!INI.IsLightAlwaysOn)
                                MACHINE.SetLight("");

                            Process.NextDuriation = 0;
                            Process.ID = 30;
                        }
                        break;
                    case 20:
                        if (Process.IsTimeup)       //抓圖
                        {
                            //Testms[0] = TestTimer.msDuriation;
                            //TestTimer.Cut();

                            if (!IsNoUseCCD)
                                OnTrigger(ResultStatusEnum.COUNTSTART);

                            OnEnvTrigger(ResultStatusEnum.CHANGEENVDIRECTORY, EnvIndex, PageOPTypeEnum.P00.ToString());

                            //Testms[1] = TestTimer.msDuriation;
                            //TestTimer.Cut();
                            //CCDCollection.GetImage();
                            //CCDCollection.GetR32Image();

                            //Parallel.For(0, AlbumWork.ENVList[EnvIndex].PageList.Count, i =>
                            //{
                            //    CCDCollection.GetImageSDM1(0, i);
                            //    PageClass page = AlbumWork.ENVList[EnvIndex].PageList[i];
                            //    page.SetbmpRUN(PageOPTypeEnum.P00, CCDCollection.GetBMP(page.CamIndex, false));
                            //});

                            int pageindex = 0;
                            foreach (var item in AlbumWork.ENVList[EnvIndex].PageList)
                            {
                                CCDCollection.GetImageSDM1(0, pageindex);
                                PageClass page = AlbumWork.ENVList[EnvIndex].PageList[pageindex];
                                page.SetbmpRUN(PageOPTypeEnum.P00, CCDCollection.GetBMP(page.CamIndex, false));

                                CamActClass.Instance.SetImage(CCDCollection.GetBMP(page.CamIndex, false), pageindex);

                                pageindex++;
                            }

                            //Testms[1] = TestTimer.msDuriation;
                            //TestTimer.Cut();

                            //FillProcessImage();

                            //Testms[2] = TestTimer.msDuriation;
                            //TestTimer.Cut();

                            Process.NextDuriation = 0;
                            Process.ID = 30;
                        }
                        break;
                    case 30:
                        if (Process.IsTimeup)
                        {
                            //Testms[3] = TestTimer.msDuriation;
                            TestTimer.Cut();

                            if (IsNoUseCCD)
                                OnTrigger(ResultStatusEnum.COUNTSTART);


                            switch (Universal.CAMACT)
                            {
                                case CameraActionMode.CAM_MOTOR_MODE2:
                                    if (IsNoUseCCD)
                                        AlbumWork.A08_RunProcess(PageOPTypeEnum.P00);

                                    break;
                                default:
                                    AlbumWork.A08_RunProcess(PageOPTypeEnum.P00);
                                    break;
                            }

                            AlbumWork.FillRunStatus(RunStatusCollection);

                            //取得Compound 在這個 ENV 裏的資料
                            AlbumWork.CPD.CollectRUNVIEWData(AlbumWork, AlbumWork.ENVList[EnvIndex].No);

                            Testms[0] = TestTimer.msDuriation;

                            //强制通过定位失败的芯片
                            if (INI.CHIP_forceALIGNRUN_pass)
                            {
                                WorkStatusCollectionClass RunStatusCollectionTemp = new WorkStatusCollectionClass();
                                RunStatusCollectionTemp.Clear();
                                foreach (WorkStatusClass workStatus in RunStatusCollection.WorkStatusList)
                                {
                                    if (workStatus.AnalyzeProcedure != AnanlyzeProcedureEnum.ALIGNRUN)
                                    {
                                        WorkStatusClass workStatus1= workStatus.Clone();
                                        RunStatusCollectionTemp.Add(workStatus1);
                                    }
                                }
                                RunStatusCollection.Clear();
                                foreach (WorkStatusClass workStatus in RunStatusCollectionTemp.WorkStatusList)
                                {
                                    WorkStatusClass workStatus1 = workStatus.Clone();
                                    RunStatusCollection.Add(workStatus1);
                                }
                            }

                            IsPass = RunStatusCollection.NGCOUNT == 0;

                            if (IsPass)
                            {
                                EnvIndex++;

                                if (EnvIndex < AlbumWork.ENVCount)
                                {
                                    ResetData(EnvIndex);

                                    Process.NextDuriation = 0;
                                    Process.ID = 10;

                                    return;
                                }
                            }
                            else
                            {
                                OnEnvTrigger(ResultStatusEnum.SAVENGRAW, EnvIndex, "-1");
                            }

                            Process.NextDuriation = 0;
                            Process.ID = 40;

                            LogProcessIDTimer(Process.ID, "整合资料结果完成");
                            _LOG_MSG(Process.ID.ToString() + " MainSDM1Tick " + "整合资料结果完成");
                        }
                        break;
                    case 40:

                        Process.Stop();
                        RXXLastProcess();

                        _LOG_MSG(Process.ID.ToString() + " MainSDM1Tick " + "流程结束");
                        LogProcessIDTimer(Process.ID, "流程结束");
                        LogProcessIDTimer(Process.ID, "==================");
                        LogProcessIDTimer(Process.ID, "==================");
                        LogProcessIDTimer(Process.ID, "==================");
                        GC.Collect();
                        break;
                }
            }


        }

        private void DLCalPageIndex(object obj)
        {
            int pageindex = (int)obj;
            AlbumWork.SetPageTestState(pageindex, false);
            AlbumWork.A08_RunProcess(PageOPTypeEnum.P00, pageindex);
            AlbumWork.SetPageTestState(pageindex, true);
        }

        public override void Reset()
        {
            base.Reset();
            m_IsReset = true;
        }
        public override void OneKeyGetImage()
        {
            base.OneKeyGetImage();
            m_IsOneKey = true;
        }

        public ProcessClass m_OnekeyGetImageProcess = new ProcessClass();
        void OnekeyGetImageTick()
        {
            ProcessClass Process = m_OnekeyGetImageProcess;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        MACHINE.SetLight("1,1,1");

                        GeneralPosition = AlbumWork.ENVList[0].GeneralPosition;
                        GeneralPositions = GeneralPosition.Split(';');
                        GeneralPositionIndex = 0;

                        ENVAnalyzePostion = new EnvAnalyzePostionSettings(GeneralPosition);
                        ENVAnalyzePostion.EnvAnalyzePostions();

                        CamActClass.Instance.SetStepCount(ENVAnalyzePostion.GetImagePostions.Length);//这里设定抓取图像的所有位置
                        CamActClass.Instance.StepCurrent = 0;

                        Process.TimeUnit = TimeUnitEnum.ms;
                        Process.NextDuriation = 0;// INI.MAINSDM1_GETSTART_DELAYTIME;
                        Process.ID = 10;
                        //_LOG_MSG(Process.ID.ToString() + " OnekeyGetImageTick 延时启动时间" + INI.MAINSDM1_GETSTART_DELAYTIME.ToString());
                        _LOG_MSG(Process.ID.ToString() + " OnekeyGetImageTick 一键取像中");

                        break;

                    case 10:
                        if (Process.IsTimeup)
                        {
                            _LOG_MSG(Process.ID.ToString() + " OnekeyGetImageTick GoX_POS="+ ENVAnalyzePostion.GetImagePostions[GeneralPositionIndex].ToString());
                            //_LOG_MSG(Process.ID.ToString() + " OnekeyGetImageTick GoX_POS=" + GeneralPositions[GeneralPositionIndex]);

                            MACHINE.GoXPosition(ENVAnalyzePostion.GetImagePostions[GeneralPositionIndex].ToString());
                            //MACHINE.GoXPosition(GeneralPositions[GeneralPositionIndex]);
                            GeneralPositionIndex++;
                            OnEnvTrigger(ResultStatusEnum.SETCAMLIGHT, EnvIndex, "ALL");
                            Process.NextDuriation = 1000;
                            Process.ID = 10090;
                        }
                        break;
                    case 10090:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.IsOnSite() || Universal.IsNoUseMotor)
                            {
                                Process.NextDuriation = INI.MAINSD_GETIMAGE_DELAYTIME;
                                Process.ID = 10100;

                                LogProcessIDTimer(Process.ID, "Onekey 取像延时=" + INI.MAINSD_GETIMAGE_DELAYTIME.ToString());
                                _LOG_MSG(Process.ID.ToString() + " OnekeyGetImageTick " + "Onekey 取像延时=" + INI.MAINSD_GETIMAGE_DELAYTIME.ToString());
                            }
                        }
                        break;
                    case 10100:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.IsOnSite() || Universal.IsNoUseMotor)
                            {
                                CCDCollection.GetImage();
                                Bitmap bitmap = CCDCollection.GetBMP(0, false);

                                if (CamActClass.Instance.StepCurrent >= CamActClass.Instance.StepCount)
                                    CamActClass.Instance.ResetStepCurrent();
                                CamActClass.Instance.SetImage(bitmap, CamActClass.Instance.StepCurrent);
                                //CamActClass.Instance.StepCurrent++;
                                
                                Process.NextDuriation = 0;
                                Process.ID = 10110;

                                LogProcessIDTimer(Process.ID, "Onekey 测试位置=" + CamActClass.Instance.StepCurrent.ToString());
                                _LOG_MSG(Process.ID.ToString() + " OnekeyGetImageTick " + "Onekey 测试位置=" + CamActClass.Instance.StepCurrent.ToString());
                            }
                        }
                        break;
                    case 10110:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
                        if (Process.IsTimeup)
                        {
                            CamActClass.Instance.StepCurrent++;
                            if (CamActClass.Instance.StepCurrent < CamActClass.Instance.StepCount)
                            {
                                Process.NextDuriation = 0;
                                Process.ID = 10;
                                LogProcessIDTimer(Process.ID, "Onekey 取像完成 ");
                                _LOG_MSG(Process.ID.ToString() + " OnekeyGetImageTick " + "Onekey 取像完成 ");
                            }
                            else
                            {

                                Process.NextDuriation = 0;
                                Process.ID = 10120;

                                MACHINE.GoXReadyPosition();

                                if (!INI.IsLightAlwaysOn)
                                    MACHINE.SetLight("");

                                //马达回待命位置
                                LogProcessIDTimer(Process.ID, "Onekey 取像完成 Motor Ready");
                                _LOG_MSG(Process.ID.ToString() + " OnekeyGetImageTick " + "Onekey 取像完成 Motor Ready");
                            }

                        }
                        break;
                    case 10120:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.IsOnSite() || Universal.IsNoUseMotor)
                            {
                                Process.Stop();
                                LogProcessIDTimer(Process.ID, "Onekey Stop");
                                _LOG_MSG(Process.ID.ToString() + " OnekeyGetImageTick " + "Onekey Stop");
                            }
                        }
                        break;

                }
            }
        }

        public ProcessClass m_ResetProcess = new ProcessClass();
        void ResetTick()
        {
            ProcessClass Process = m_ResetProcess;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        Process.TimeUnit = TimeUnitEnum.ms;
                        Process.NextDuriation = 1000;
                        Process.ID = 10;

                        //来一个清除报警

                        MACHINE.XHome();

                        _LOG_MSG("ResetTick 复位中");

                        break;

                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.IsHome() || Universal.IsNoUseMotor)
                            {
                                _LOG_MSG("ResetTick 复位完成");

                                Process.Stop();
                            }
                        }
                        break;
                }
            }
        }

        #region MAIN_SDM1 馬達運動取像流程

        public ProcessClass m_DLGetImageProcess = new ProcessClass();
        void DLGetImageTick()
        {
            ProcessClass Process = m_DLGetImageProcess;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        Process.TimeUnit = TimeUnitEnum.ms;
                        Process.NextDuriation = INI.MAINSD_GETIMAGE_DELAYTIME;
                        Process.ID = 10;

                        break;

                    case 10:
                        if (Process.IsTimeup)
                        {
                            Process.Stop();
                            Universal.CCDCollection.GetImage();
                            Bitmap bitmap = Universal.CCDCollection.GetBMP(0, false);

                            if (CamActClass.Instance.StepCurrent >= CamActClass.Instance.StepCount)
                                CamActClass.Instance.ResetStepCurrent();
                            CamActClass.Instance.SetImage(bitmap, CamActClass.Instance.StepCurrent);
                            CamActClass.Instance.StepCurrent++;

                            m_DLGetImageOK.Start();
                        }
                        break;
                }
            }
        }

        public ProcessClass m_DLGetImageOK = new ProcessClass();
        void DLGetImageOKTick()
        {
            ProcessClass Process = m_DLGetImageOK;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        Process.TimeUnit = TimeUnitEnum.ms;
                        Process.NextDuriation = INI.handle_delaytime;
                        Process.ID = 10;
                        MACHINE.PLCIO.GetImageOK = true;

                        break;

                    case 10:
                        if (Process.IsTimeup)
                        {
                            Process.Stop();
                            MACHINE.PLCIO.GetImageOK = false;

                        }
                        break;
                }
            }
        }

        public ProcessClass m_DLResultOK = new ProcessClass();
        void DLResultOKTick()
        {
            ProcessClass Process = m_DLResultOK;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        Process.TimeUnit = TimeUnitEnum.ms;
                        Process.NextDuriation = INI.handle_delaytime;
                        Process.ID = 10;

                        MACHINE.PLCIO.Busy = false;
                        if (MACHINE.PLCIO.Ready)
                        {
                            MACHINE.PLCIO.Pass = IsPass;
                            MACHINE.PLCIO.Fail = !IsPass;
                        }

                        break;

                    case 10:
                        if (Process.IsTimeup)
                        {
                            Process.Stop();
                            MACHINE.PLCIO.Pass = false;
                            MACHINE.PLCIO.Fail = false;
                        }
                        break;
                }
            }
        }

        #endregion


        /// <summary>
        /// RXX 結束流程後要做的雞巴毛事
        /// </summary>
        void RXXLastProcess()
        {
            ////System.Threading.Thread thread = new System.Threading.Thread(SendSignToPLC);
            ////thread.Start(IsPass);

            ////m_DLResultOK.Start((IsPass ? "PASS" : "FAIL"));

            //m_DLResultOK.Start();
            //LogProcessIDTimer(8888, "发送结果信号PC==>PLC");

            //switch (Universal.CAMACT)
            //{
            //    case CameraActionMode.CAM_MOTOR_MODE2:
            //        //System.Threading.Thread threadGetImageOK = new System.Threading.Thread(SendSignToPLCImageOK);
            //        //threadGetImageOK.Start();

            //        m_DLGetImageOK.Start();
            //        LogProcessIDTimer(8889, "发送最后取像完成信号PC==>PLC");
            //        break;
            //}


            if (IsPass)
            {
                PlayerPass.Play();
                OnTrigger(ResultStatusEnum.CALPASS);
                MACHINE.PLCIO.Pass = false;
            }
            else
            {
                PlayerFail.Play();
                OnTrigger(ResultStatusEnum.CALNG);

                if (INI.CHIP_force_pass)
                    MACHINE.PLCIO.Pass = false;
            }
            

            //if (INI.ISQSMCALLSAVE)
            //    _saveAllResultPictures();

            if (INI.IsCollectPictures)
                MainX6Save();

            OnTrigger(ResultStatusEnum.COUNTEND);
            OnTrigger(ResultStatusEnum.CALEND);
        }
        void FillOperaterString(string opstr)
        {
            foreach (EnvClass env in AlbumWork.ENVList)
            {
                env.PassInfo.OperateString = opstr;

                foreach (PageClass page in env.PageList)
                {
                    page.PassInfo.OperateString = opstr;

                    foreach (AnalyzeClass analyzeroot in page.AnalyzeRootArray)
                    {
                        analyzeroot.SetPassInfoOPString(opstr);
                    }
                }
            }
        }
        public void SavePrintScreenForMainX6()
        {
            string screen_SavePath = "D:\\CollectPictures_" + Universal.OPTION.ToString() + "\\Screen\\" + JzTimes.DateSerialString;

            if (!Directory.Exists(screen_SavePath))
                Directory.CreateDirectory(screen_SavePath);

            int width = Screen.PrimaryScreen.Bounds.Width;
            int height = Screen.PrimaryScreen.Bounds.Height;

            Bitmap m = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(m))
            {
                g.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size);
                g.Dispose();
            }

            m.Save(screen_SavePath + "\\Screen_" + JzTimes.DateTimeSerialString + ".jpg", ImageFormat.Jpeg);
            m.Dispose();
        }
        private void MainX6Save()
        {
            if (INI.CHIP_NG_collect)
            {
                if (IsPass)
                    return;
            }

            string mainx6_path = Universal.MainX6_Path + "\\" + (IsPass ? "P-" : "F-") + JzTimes.DateTimeSerialString;

            if (!Directory.Exists(mainx6_path + "\\000"))
                Directory.CreateDirectory(mainx6_path + "\\000");

            EnvClass env = AlbumWork.ENVList[0];

            int qi = 0;
            foreach (PageClass page in env.PageList)
            {
                page.GetbmpRUN().Save(mainx6_path + "\\000\\P00-" + qi.ToString("000") + ".png", ImageFormat.Png);
                qi++;
            }
        }
        private void _LOG_MSG(string eMsg)
        {
            CommonLogClass.Instance.LogMessage(eMsg);
        }
        private void _LOG_MSG_ERR(string eMsg)
        {
            CommonLogClass.Instance.LogError(eMsg);
        }

        #region QSMC Saving AllResultPictures //ADD Gaara

        string PicturePath = INI.ALLRESULTPIC + "\\Pictures\\" + JzTimes.DateSerialString + "\\" + JzTimes.DateTimeSerialString + "_" + INI.DATA_FIXTUREID + "_NULLSN" + "_Pictures";
        void ThreadForSavePictures()
        {
            System.Threading.Thread m_thread = new System.Threading.Thread(new System.Threading.ThreadStart(_saveAllResultPictures));
            m_thread.Start();
        }
        private void _saveAllResultPictures()
        {
            string DatePath = INI.ALLRESULTPIC + "\\Pictures\\" + JzTimes.DateSerialString;

            if (!Directory.Exists(DatePath))
                Directory.CreateDirectory(DatePath);

            string addstr = (IsPass ? "P-" : "F-");
            string strbarcodetmp = (string.IsNullOrEmpty(BARCODE) ? "Null_SN" : BARCODE);

            //所有CAM存圖的路徑
            string AllSavePath = DatePath + "\\" + addstr + JzTimes.DateTimeSerialString + "_" + INI.DATA_FIXTUREID + "_" + strbarcodetmp + "_" + VER + "_" + ARTWORKNAME + "_" + RELATECOLORSTR + "_Pictures";
            string AllSaveName = INI.DATA_FIXTUREID + "_" + strbarcodetmp + "_" + VER + "_" + ARTWORKNAME + "_" + RELATECOLORSTR + "_CAM";

            PicturePath = AllSavePath;//用於截圖存儲的路徑

            if (!Directory.Exists(AllSavePath))
                Directory.CreateDirectory(AllSavePath);

            EnvClass env = AlbumWork.ENVList[0];

            int qi = 0;
            foreach (PageClass page in env.PageList)
            {
                page.GetbmpRUN().Save(AllSavePath + "\\" + AllSaveName + qi.ToString("000") + ".jpg", ImageFormat.Jpeg);
                qi++;
            }
        }
        public void SavePrintScreen()
        {
            string strpath = @"D:\PRINTSCREEN\" + JzTimes.DateSerialString;
            string Qsmcpath = INI.ALLRESULTPIC + "\\NGPictures\\" + JzTimes.DateSerialString;

            if (!Directory.Exists(strpath))
                Directory.CreateDirectory(strpath);

            if (!Directory.Exists(Qsmcpath))
                Directory.CreateDirectory(Qsmcpath);

            int width = Screen.PrimaryScreen.Bounds.Width;
            int height = Screen.PrimaryScreen.Bounds.Height;

            Bitmap m = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(m))
            {
                g.CopyFromScreen(0, 0, 0, 0, Screen.AllScreens[0].Bounds.Size);
                g.Dispose();
            }

            if (!IsPass)
                m.Save(Qsmcpath + "\\" + (string.IsNullOrEmpty(BARCODE) ?
                                                                    JzTimes.DateTimeSerialString + "_" + INI.DATA_FIXTUREID + "_NULLSN_OCR"
                                                                    :
                                                                    JzTimes.DateTimeSerialString + "_" + INI.DATA_FIXTUREID + "_" + BARCODE + "_OCR") + ".jpg", ImageFormat.Jpeg);
            if (!Directory.Exists(PicturePath))
                Directory.CreateDirectory(PicturePath);

            m.Save(PicturePath + "\\" + (string.IsNullOrEmpty(BARCODE) ?
                                                                    JzTimes.DateTimeSerialString + "_" + INI.DATA_FIXTUREID + "_NULLSN_OCR"
                                                                    :
                                                                    JzTimes.DateTimeSerialString + "_" + INI.DATA_FIXTUREID + "_" + BARCODE + "_OCR") + ".jpg", ImageFormat.Jpeg);
            if (!Directory.Exists(strpath))
            {
                Directory.CreateDirectory(strpath);
            }
            m.Save(strpath + "\\" + (string.IsNullOrEmpty(BARCODE) ? JzTimes.TimeSerialString : BARCODE) + ".jpg", ImageFormat.Jpeg);
            m.Dispose();
        }

        #endregion
    }
}
