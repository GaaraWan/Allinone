using Allinone.BasicSpace;
using Allinone.ControlSpace;
using Allinone.ControlSpace.MachineSpace;
using JetEazy;
using JetEazy.BasicSpace;
using JetEazy.ControlSpace;
using JetEazy.FormSpace;
using JetEazy.Interface;
using JetEazy.PlugSpace;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class JzMainSDM5ResultClass : GeoResultClass
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr _lopen(string lpPathName, int iReadWrite);
        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);
        public const int OF_READWRITE = 2;
        public const int OF_SHARE_DENY_NONE = 0x40;
        public readonly IntPtr HFILE_ERROR = new IntPtr(-1);

        bool m_IsUserStop = false;
        bool m_IsALARM = false;
        bool m_IsEMC = false;
        bool m_IsReset = false;
        bool m_IsOneKey = false;
        bool m_IsErrorRobot = false;

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

        IxLineScanCam m_IxLinescanCamera
        {
            get
            {
                return Universal.IxLineScan;
            }
        }

        JzMainSDM5MachineClass MACHINE;

        JzToolsClass JzTools = new JzToolsClass();
        SoundPlayer PlayerPass = new SoundPlayer();
        SoundPlayer PlayerFail = new SoundPlayer();

        //EnvClass m_EnvNow = null;
        EnvClass m_EnvNow
        {
            get
            {
                return AlbumWork.m_EnvNow;
            }
        }

        public JzMainSDM5ResultClass(Result_EA resultea, VersionEnum version, OptionEnum option, MachineCollectionClass machinecollection)
        {
            myResultEA = resultea;
            VERSION = version;
            OPTION = option;

            PlayerPass.SoundLocation = Universal.PlayerPASSPATH;
            PlayerFail.SoundLocation = Universal.PlayerFAILPATH;
            PlayerPass.Load();
            PlayerFail.Load();

            DUP = new DupClass();

            MACHINE = (JzMainSDM5MachineClass)machinecollection.MACHINE;
            MACHINE.TriggerAction += MACHINE_TriggerAction;


            MainProcess = new ProcessClass();
        }

        private void MACHINE_TriggerAction(MachineEventEnum machineevent, object obj = null)
        {
            switch (machineevent)
            {
                case MachineEventEnum.ALARM:
                    m_IsALARM = true;
                    break;
                case MachineEventEnum.EMC:
                    m_IsEMC = true;
                    break;
                case MachineEventEnum.RESET:
                    m_IsReset = true;
                    break;
                case MachineEventEnum.ALARM_ROBOT:
                    m_IsErrorRobot = true;
                    break;
                case MachineEventEnum.USER_STOP:
                    m_IsUserStop = true;
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
            //if (MACHINE.PLCIO.IsEMC && !Universal.IsNoUseIO)
            //{
            //    _LOG_MSG_ERR("急停中 无法测试");
            //    return;
            //}

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
            if (m_IsUserStop)
            {
                m_IsUserStop = false;
                StopAllProcess();

                _LOG_MSG($"用户按下停止按钮 ");
            }
            //if (m_IsErrorRobot)
            //{
            //    m_IsErrorRobot = false;
            //    StopAllProcess();

            //    _LOG_MSG_ERR($"报警中 {MACHINE.PLCIO.RobotErrorMessage}");
            //}
            if (m_IsEMC)
            {
                m_IsEMC = false;
                StopAllProcess();

                _LOG_MSG_ERR("EMC 急停中");
            }
            if (m_IsALARM)
            {
                m_IsALARM = false;
                StopAllProcess();

                _LOG_MSG_ERR("ALARM 报警中");
            }
            //if (m_IsReset)
            //{
            //    _LOG_MSG("RESET 复位按下");
            //    m_IsReset = false;
            //    if (!MainProcess.IsOn && !m_OnekeyGetImageProcess.IsOn && !m_ResetProcess.IsOn && !MACHINE.PLCIO.IsEMC)
            //    {
            //        _LOG_MSG("RESET 复位启动");
            //        if (!Universal.IsNoUseCCD)
            //            m_ResetProcess.Start();
            //    }
            //}
            //if (m_IsOneKey)
            //{
            //    _LOG_MSG("ONEKEY 一键取像按下");
            //    m_IsOneKey = false;
            //    if (!MainProcess.IsOn && !m_OnekeyGetImageProcess.IsOn && !m_ResetProcess.IsOn && (Universal.IsNoUseCCD || !MACHINE.PLCIO.IsEMC))
            //    {
            //        _LOG_MSG("ONEKEY 一键取像启动");
            //        //if (!Universal.IsNoUseCCD)
            //        m_OnekeyGetImageProcess.Start();
            //    }
            //}
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
        Stopwatch m_GetImageTimeout = new Stopwatch();

        protected override void MainProcessTick()
        {
            switch (OPTION)
            {
                case OptionEnum.MAIN_SDM5:

                    DLResultOKTick();
                    MainSDM5Tick();
                    DLGetImageTick();
                    //ResetTick();

                    //switch (Universal.CAMACT)
                    //{
                    //    case CameraActionMode.CAM_MOTOR:
                    //    case CameraActionMode.CAM_MOTOR_MODE2:

                    //        //OnekeyGetImageTick();
                    //        ResetTick();

                    //        break;
                    //}

                    if (m_IsOneKey)
                    {
                        _LOG_MSG("ONEKEY 一键取像按下");
                        m_IsOneKey = false;
                        if (!MainProcess.IsOn && !m_DLGetImageProcess.IsOn && !m_ResetProcess.IsOn && (Universal.IsNoUseCCD || !MACHINE.PLCIO.ADR_ISEMC))
                        {
                            MACHINE.PLCIO.ADR_OnceGetImage = true;
                            _LOG_MSG("ONEKEY 一键取像启动");
                            //if (!Universal.IsNoUseCCD)
                            m_DLGetImageProcess.Start();
                        }
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
        public void FillProcessImageMotorPageIndexSDM2(int index)
        {
            EnvClass env = m_EnvNow;

            if (index >= env.PageList.Count || index < 0)
                return;

            int getimageindex = 0;
            //if (Universal.IsNoUseIO)
            //    getimageindex = index;

            PageClass page = env.PageList[index];
            page.SetbmpRUN(PageOPTypeEnum.P00, CCDCollection.GetBMP(getimageindex, false));

            EnvClass env2 = AlbumWork.ENVList[EnvIndex];
            PageClass page1 = env2.PageList[page.No];
            page1.SetbmpRUN(PageOPTypeEnum.P00, CCDCollection.GetBMP(getimageindex, false));

            //Universal.SDM2_BMP_SHOW_CURRENT.Dispose();
            //Universal.SDM2_BMP_SHOW_CURRENT = (Bitmap)CCDCollection.GetBMP(getimageindex, false).Clone();// new Bitmap(CCDCollection.GetBMP(getimageindex, false));
            //OnTrigger(ResultStatusEnum.SHOW_CURRENT_IMAGE);
        }

        public void FillProcessImageMotorPageIndex(int index, Bitmap bmpinut)
        {
            EnvClass env = AlbumWork.ENVList[EnvIndex];

            if (index >= env.PageList.Count || index < 0)
                return;

            PageClass page = env.PageList[index];
            //bmpLargeTemp.Dispose();
            //bmpLargeTemp = new FreeImageBitmap(bmpinut);
            //page.SetbmpRUN(PageOPTypeEnum.P00, bmpLargeTemp.ToBitmap());
            page.SetbmpRUN(PageOPTypeEnum.P00, bmpinut);
        }
        public void FillProcessImageMotorPageIndexDebug(int index)
        {
            EnvClass env = AlbumWork.ENVList[EnvIndex];

            if (index >= env.PageList.Count || index < 0)
                return;

            PageClass page = env.PageList[index];
            page.SetbmpRUN(PageOPTypeEnum.P00, CCDCollection.GetBMP(index, false));
        }

        /// <summary>
        /// 计算tray通过第一个位置得来的偏移值 用到其他页面
        /// </summary>
        Point m_AlignFristOffset = new Point(0, 0);
        /// <summary>
        /// 用来记录第一次定位完成计算偏移值的标志
        /// </summary>
        bool m_IsAlignComplete = false;
        /// <summary>
        /// 放入定位位置的图片 并且计算偏移
        /// </summary>
        /// <param name="index"></param>
        public void FillProcessImageMotorAlignSDM2(int index)
        {
            foreach (PageClass page in AlbumWork.ENVList[0].PageList)
            {
                if (page.AliasName == "ALIGN")
                {
                    page.SetbmpRUN(PageOPTypeEnum.P00, CCDCollection.GetBMP(0, false));
                    bool isgood = page.A08_RunProcess(PageOPTypeEnum.P00);
                    if (isgood)
                    {
                        m_AlignFristOffset = page.GetFirstOffset();
                    }
                    else
                    {
                        m_AlignFristOffset = new Point(0, 0);
                    }
                    LogProcessIDTimer(007, "定位页面offset=x:" + m_AlignFristOffset.X.ToString() + ",y:"
                                                            + m_AlignFristOffset.Y.ToString(), false);
                    break;
                }
                else
                {
                    m_AlignFristOffset = new Point(0, 0);
                    LogProcessIDTimer(007, "无定位页面", false);
                }
            }
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
        string m_CurrentPosition = "0,0,0";
        void MainSDM3Tick()
        {
            ProcessClass Process = MainProcess;

            if (Process.IsOn)
            {
                if (m_ProcessTmpID != Process.ID)
                {
                    m_ProcessTmpID = Process.ID;
                    _LOG_MSG(Process.ID.ToString() + " MainSDM2Tick Running...");
                }

                switch (Process.ID)
                {
                    case 5:

                        //m_EnvNow = new EnvClass();
                        RunDataMappingCollection.Clear();


                        LogProcessSwitch = true;
                        LogProcessIDTimer(Process.ID, "流程开始");
                        _LOG_MSG(Process.ID.ToString() + " MainSDM2Tick " + "流程开始");

                        //MACHINE.PLCIO.Busy = true;
                        //MACHINE.SetLight("1,1,1");

                        if (INI.IsCollectPictures)
                            Universal.MainX6_Path = "D:\\CollectPictures_" + Universal.OPTION.ToString() + "\\pictures\\" + JzTimes.DateSerialString;
                        Universal.MainX6_Picture_Path = "D:\\CollectPictures_" + Universal.OPTION.ToString() + "\\pictures_Single\\" + JzTimes.DateSerialString;

                        TestTimer.Cut();
                        m_input_time = DateTime.Now;
                        OnEnvTrigger(ResultStatusEnum.CHANGEDIRECTORY, 0, "");
                        OnTrigger(ResultStatusEnum.COUNTSTART);
                        //把要檢測的東西放進去
                        FillOperaterString(RELATECOLORSTR);
                        Universal.CalTestBarcode = string.Empty;
                        if (Universal.IsNoUseIO)
                        {
                            string[] _dirStrs = LastDirPath.Split('\\');
                            Universal.CalTestBarcode = _dirStrs[_dirStrs.Length - 1];
                            Universal.CalTestPath = @"D:\TESTCOLLECT\" + Universal.CalTestBarcode;
                        }
                        else
                            Universal.CalTestPath = @"D:\TESTCOLLECT\" + JzTimes.DateTimeSerialString;

                        if (!Directory.Exists(Universal.CalTestPath))
                            Directory.CreateDirectory(Universal.CalTestPath);

                        Process.NextDuriation = 0;

                        if (IsNoUseCCD)
                            Process.ID = 20;
                        else
                            Process.ID = 10;


                        switch (Universal.CAMACT)
                        {
                            case CameraActionMode.CAM_MOTOR_MODE2:

                                #region no use mutil pages
                                //int ipageindex = 0;
                                //foreach (PageClass page in AlbumWork.ENVList[0].PageList)
                                //{
                                //    //获取位置
                                //    if (string.IsNullOrEmpty(page.sPagePostion))
                                //    {
                                //        //没有的位置的页面不添加
                                //        //PageClass page1 = page.Clone();
                                //        //page1.PageRunNo = ipageindex;
                                //        //page1.PageRunPos = "0,0,0";
                                //        //ipageindex++;

                                //        //m_EnvNow.PageList.Add(page1);
                                //    }
                                //    else
                                //    {
                                //        foreach (string pos in page.PagePostionList)
                                //        {
                                //            PageClass page1 = page.Clone();
                                //            page1.PageRunNo = ipageindex;
                                //            page1.PageRunPos = pos;
                                //            ipageindex++;

                                //            m_EnvNow.PageList.Add(page1);
                                //        }
                                //    }
                                //}
                                //m_EnvNow.EnvAutoReportIndex();
                                ////训练线程
                                //System.Threading.Thread thread_Train = new System.Threading.Thread(DLTrainSDM2);
                                //thread_Train.Start(new object());

                                //m_EnvNow.ResetTrainStatus();
                                //m_EnvNow.A00_TrainProcess(true);
                                #endregion

                                GeneralPosition = AlbumWork.ENVList[0].GeneralPosition;
                                GeneralPositions = GeneralPosition.Split(';');
                                GeneralPositionIndex = 0;

                                CamActClass.Instance.SetStepCount(m_EnvNow.PageList.Count);//这里设定抓取图像的所有位置

                                CamActClass.Instance.StepCurrent = 0;

                                break;
                        }

                        Process.NextDuriation = INI.MAINSDM1_GETSTART_DELAYTIME;
                        //Process.ID = 10;
                        _LOG_MSG(Process.ID.ToString() + " MainSDM1Tick 延时启动时间" + INI.MAINSDM1_GETSTART_DELAYTIME.ToString());
                        RunNextCCDDelayTime = INI.MAINSD_GETIMAGE_DELAYTIME;

                        break;
                    case 10:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
                        if (Process.IsTimeup)
                        {
                            switch (Universal.CAMACT)
                            {
                                case CameraActionMode.CAM_MOTOR_MODE2:
                                    //if (m_EnvTrainOK)
                                    {
                                        m_CurrentPosition = _getCurrentPos(GeneralPositionIndex);
                                        _LOG_MSG(Process.ID.ToString() + " MainSDM2Tick POS=" + m_CurrentPosition);

                                        //MACHINE.GoPosition(m_CurrentPosition);
                                        GeneralPositionIndex++;

                                        OnEnvTrigger(ResultStatusEnum.SETCAMLIGHT, EnvIndex, "ALL");
                                        Process.NextDuriation = 0;
                                        Process.ID = 10090;

                                        RunStepComplete = true;//第一次强制给完成信号
                                        m_IsAlignComplete = false;//第一次复位标志
                                    }

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
                            //if (MACHINE.IsOnSite()
                            //    && MACHINE.IsOnSitePosition(m_CurrentPosition)
                            //    && RunStepComplete || (Universal.IsNoUseMotor && RunStepComplete))
                            ////if (MACHINE.IsOnSite() && MACHINE.IsOnSitePosition(m_CurrentPosition) || (Universal.IsNoUseMotor && RunStepComplete))
                            //{
                            //    MACHINE.PLCIO.RobotAbs = false;
                            //    RunStepComplete = false;
                            //    Process.NextDuriation = RunNextCCDDelayTime;// INI.MAINSD_GETIMAGE_DELAYTIME;
                            //    Process.ID = 10100;
                            //    if (!INI.IsLightAlwaysOn)
                            //        MACHINE.PLCIO.TopLight = true;
                            //    LogProcessIDTimer(Process.ID, "取像延时=" + RunNextCCDDelayTime.ToString());
                            //    //LogProcessIDTimer(Process.ID, "取像延时=" + INI.MAINSD_GETIMAGE_DELAYTIME.ToString());

                            //    if (Universal.IsNoUseIO || Universal.IsLocalPicture)
                            //    {
                            //        OnEnvTrigger(ResultStatusEnum.CHANGEENVDIRECTORY, EnvIndex, PageOPTypeEnum.P00.ToString());
                            //    }
                            //}
                        }
                        break;
                    case 10100:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
                        if (Process.IsTimeup)
                        {
                            if (
                                //MACHINE.IsOnSite()
                                //&& MACHINE.IsOnSitePosition(m_CurrentPosition)
                                //|| 
                                Universal.IsNoUseMotor)
                            {
                                //抓图
                                if (Universal.IsNoUseIO || Universal.IsLocalPicture)
                                    CCDCollection.GetImageSDM1(0, CamActClass.Instance.StepCurrent);
                                else
                                {
                                    CCDCollection.GetImage();

                                    //CCDCollection.GetImage();
                                }
                                //CCDCollection.GetImage();
                                if (CamActClass.Instance.StepCurrent >= CamActClass.Instance.StepCount)
                                    CamActClass.Instance.ResetStepCurrent();
                                FillProcessImageMotorPageIndexSDM2(CamActClass.Instance.StepCurrent);
                                if (!m_IsAlignComplete)
                                {
                                    m_IsAlignComplete = true;
                                    //FillProcessImageMotorAlignSDM2(CamActClass.Instance.StepCurrent);
                                }
                                //CamActClass.Instance.StepCurrent++;
                                //>> 20230608
                                //CamActClass.Instance.SetImage(CCDCollection.GetBMP(0, false), CamActClass.Instance.StepCurrent);

                                //图片填充完成就可以触发马达前进了
                                #region

                                m_CurrentPosition = _getCurrentPos(GeneralPositionIndex);
                                _LOG_MSG(Process.ID.ToString() + " task-MainSDM2Tick POS=" + m_CurrentPosition);
                                //if (!INI.IsLightAlwaysOn)
                                //    MACHINE.PLCIO.TopLight = false;
                                //MACHINE.GoPosition(m_CurrentPosition, false);//写入位置不执行Move
                                GeneralPositionIndex++;

                                //Task task = new Task(() =>
                                //{
                                //    m_CurrentPosition = _getCurrentPos(GeneralPositionIndex);
                                //    _LOG_MSG(Process.ID.ToString() + " task-MainSDM2Tick POS=" + m_CurrentPosition);
                                //    MACHINE.PLCIO.TopLight = false;
                                //    MACHINE.GoPosition(m_CurrentPosition);
                                //    GeneralPositionIndex++;

                                //    //OnEnvTrigger(ResultStatusEnum.SETCAMLIGHT, EnvIndex, "ALL");
                                //});
                                //task.Start();

                                #endregion

                                Process.NextDuriation = 0;
                                Process.ID = 10110;

                                LogProcessIDTimer(Process.ID, "测试位置=" + CamActClass.Instance.StepCurrent.ToString());
                            }
                        }
                        break;
                    case 10110:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
                        if (Process.IsTimeup)
                        {
                            //>>单步测试
                            ////计算线程
                            //System.Threading.Thread thread_DL_Test = new System.Threading.Thread(DLCalPageIndexSDM2);
                            //thread_DL_Test.Start(CamActClass.Instance.StepCurrent);

                            threadPara para = new threadPara();
                            para.ParaPageIndex = CamActClass.Instance.StepCurrent;
                            para.ParaEnvindex = EnvIndex;

                            //System.Threading.Thread thread_DL_Test = new System.Threading.Thread(DLCalPageOneStepIndex);
                            //thread_DL_Test.Start(para);
                            //thread_DL_Test.Start(CamActClass.Instance.StepCurrent);

                            DLCalPageOneStepIndex(para);
                            //DLCalPageOneStepIndex(CamActClass.Instance.StepCurrent);
                            CamActClass.Instance.StepCurrent++;

                            if (CamActClass.Instance.StepCurrent < CamActClass.Instance.StepCount)
                            {
                                Process.NextDuriation = 0;
                                //Process.ID = 10;
                                Process.ID = 10090;//这里是取完图后立马让马达运动 这样节省时间

                                //MACHINE.PLCIO.RobotAbs = true;//执行Move
                                LogProcessIDTimer(Process.ID, "取像完成 Send==>Sign ImageOK");
                            }
                            else
                            {

                                Process.NextDuriation = 0;
                                Process.ID = 10120;
                                //Task task = new Task(() =>
                                //{
                                //    MACHINE.GoReadyPosition();
                                //    System.Threading.Thread.Sleep(300);
                                //    MACHINE.PLCIO.RobotAbs = true;//执行Ready位置
                                //});
                                //task.Start();

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
                            if ((_getAllPageComplete() || RunStepComplete)
                                //&& (MACHINE.IsOnSite()
                                //&& MACHINE.IsOnSitePosition(_getReadyPos())
                                || Universal.IsNoUseMotor
                                )
                            //if (_getAllPageComplete())
                            {
                                Process.NextDuriation = 0;
                                Process.ID = 1030;

                                //MACHINE.PLCIO.RobotAbs = false;//回到待命后停止

                                LogProcessIDTimer(Process.ID, "所有页面测试完成");
                                _LOG_MSG(Process.ID.ToString() + " MainSDM2Tick " + "所有页面测试完成");
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

                            //if (!INI.IsLightAlwaysOn)
                            //MACHINE.SetLight("");

                            //MACHINE.PLCIO.RobotAbs = false;//回到待命后停止

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
                            //>>20230608
                            //m_EnvNow.FillRunStatus(RunStatusCollection);

                            //AlbumWork.FillRunStatus(RunStatusCollection);

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
                                    if (workStatus.AnalyzeProcedure == AnanlyzeProcedureEnum.CHECKBARCODE)
                                    {
                                        if (workStatus.Reason == ReasonEnum.NG)
                                        {
                                            workStatus.bmpRUN.Save("D:\\Barcode2D.png", ImageFormat.Png);
                                        }
                                        continue;
                                    }

                                    if (workStatus.AnalyzeProcedure != AnanlyzeProcedureEnum.ALIGNRUN)
                                    {
                                        WorkStatusClass workStatus1 = workStatus.Clone();
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
                            _LOG_MSG(Process.ID.ToString() + " MainSDM2Tick " + "整合资料结果完成");
                        }
                        break;
                    case 40:

                        Process.Stop();
                        RXXLastProcess();

                        _LOG_MSG(Process.ID.ToString() + " MainSDM2Tick " + "流程结束");
                        LogProcessIDTimer(Process.ID, "流程结束");
                        LogProcessIDTimer(Process.ID, "==================");
                        LogProcessIDTimer(Process.ID, "==================");
                        LogProcessIDTimer(Process.ID, "==================");
                        GC.Collect();
                        break;
                }
            }


        }

        MessageForm M_WARNING_FRM = null;

        void MainSDM5Tick()
        {
            ProcessClass Process = MainProcess;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        Universal.IsRunningTest = true;

                        LogProcessSwitch = true;
                        LogProcessIDTimer(Process.ID, "流程开始");

                        if (INI.IsReadHandlerOKSign)
                        {
                            MACHINE.PLCIO.Pass = false;
                            MACHINE.PLCIO.Fail = false;
                        }

                        //这里如果开关打开则强制进行全检
                        if (INI.IsOpenForceAllCheck)
                        {
                            EnvClass env = AlbumWork.ENVList[0];
                            foreach (PageClass page in env.PageList)
                            {
                                foreach (AnalyzeClass analyze in page.AnalyzeRoot.BranchList)
                                {
                                    analyze.SetAnalyzeByPass(false);
                                }
                            }
                        }


                        //MACHINE.PLCIO.Busy = true;
                        MACHINE.SetLight(AlbumWork.ENVList[0].GeneralLight);
                        //MACHINE.SetLight("0,0,0,0,0,0,0");
                        //MACHINE.SetLight("1,1,1");
                        //string light = AlbumWork.ENVList[0].GeneralLight;
                        if (INI.IsCollectPictures)
                            Universal.MainX6_Path = "D:\\CollectPictures\\Inspection\\" + JzTimes.DateTimeSerialString;


                        TestTimer.Cut();
                        m_input_time = DateTime.Now;
                        OnEnvTrigger(ResultStatusEnum.CHANGEDIRECTORY, 0, "");
                        //OnTrigger(ResultStatusEnum.COUNTSTART);
                        //把要檢測的東西放進去
                        FillOperaterString(RELATECOLORSTR);

                        Process.NextDuriation = 0;

                        if (IsNoUseCCD)
                            Process.ID = 20;
                        else
                            Process.ID = 10;

                        break;
                    case 10:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
                        if (Process.IsTimeup)
                        {
                            switch (Universal.CAMACT)
                            {
                                case CameraActionMode.CAM_MOTOR_LINESCAN:

                                    m_IxLinescanCamera.IsGrapImageComplete = false;
                                    Process.NextDuriation = INI.MAINSD_GETIMAGE_DELAYTIME;
                                    Process.ID = 10200;

                                    LogProcessIDTimer(Process.ID, "线扫取像延时=" + INI.MAINSD_GETIMAGE_DELAYTIME.ToString());

                                    break;
                                case CameraActionMode.CAM_MOTOR_MODE2:
                                    Process.NextDuriation = INI.MAINSD_GETIMAGE_DELAYTIME;
                                    Process.ID = 10100;

                                    LogProcessIDTimer(Process.ID, "取像延时=" + INI.MAINSD_GETIMAGE_DELAYTIME.ToString());
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

                    #region 线扫模式流程

                    case 10200:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
                        if (Process.IsTimeup)
                        {
                            if (m_IxLinescanCamera.IsGrapImageComplete)
                            {
                                if (m_IxLinescanCamera.IsGrapImageOK)
                                {
                                    //using (FreeImageAPI.FreeImageBitmap bmp =
                                    //   new FreeImageAPI.FreeImageBitmap(m_IxLinescanCamera.ImageWidth,
                                    //   m_IxLinescanCamera.ImageHeight,
                                    //   m_IxLinescanCamera.ImageWidth,
                                    //   PixelFormat.Format8bppIndexed,
                                    //   m_IxLinescanCamera.ImagePbuffer)
                                    //)

                                    using (Bitmap bitmap = m_IxLinescanCamera.GetFreeImageBitmap().ToBitmap())
                                    {
                                        //bmp.Rotate(m_IxLinescanCamera.ImageRotate);

                                        CamActClass.Instance.ResetStepCurrent();
                                        if (CamActClass.Instance.StepCurrent >= CamActClass.Instance.StepCount)
                                            CamActClass.Instance.ResetStepCurrent();

                                        FillProcessImageMotorPageIndex(CamActClass.Instance.StepCurrent, bitmap);
                                        //CamActClass.Instance.SetImage(bitmap, CamActClass.Instance.StepCurrent);
                                        //OnTriggerOP(ResultStatusEnum.SET_CURRENT_IMAGE, "ONLINE#" + CamActClass.Instance.StepCurrent.ToString());

                                        LogProcessIDTimer(Process.ID, "线扫取像完成Step=" + CamActClass.Instance.StepCurrent.ToString());


                                        MACHINE.PLCIO.ADR_LinePCToPlcSign = true;
                                    }
                                    Process.NextDuriation = 0;
                                    Process.ID = 10100;
                                }
                                else
                                {
                                    Process.Stop();
                                    LogProcessIDTimer(Process.ID, "线扫取像失败Step=" + CamActClass.Instance.StepCurrent.ToString());
                                    ////失败
                                    //M_WARNING_FRM = new MessageForm(true, "流程中线扫抓图失败，请检查。。。", "");
                                    //if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                                    //{
                                    //}
                                    //M_WARNING_FRM.Close();
                                    //M_WARNING_FRM.Dispose();
                                }
                            }
                            //else
                            //{
                            //    //可以添加超时
                            //}
                        }
                        break;

                    #endregion


                    #region 马达模式2  

                    case 10100:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
                        if (Process.IsTimeup)
                        {

                            int _currentStep = CamActClass.Instance.GetCurrentStep();

                            if (Universal.IsNoUseIO)
                                OnEnvTrigger(ResultStatusEnum.CHANGEENVDIRECTORY, EnvIndex, PageOPTypeEnum.P00.ToString());

                            switch (Universal.CAMACT)
                            {
                                case CameraActionMode.CAM_MOTOR_LINESCAN:
                                    break;
                                default:

                                    CCDCollection.GetImage();
                                    //CCDCollection.GetImage();
                                    if (_currentStep >= CamActClass.Instance.StepCount)
                                        CamActClass.Instance.ResetStepCurrent();
                                    if (Universal.IsNoUseIO)
                                        FillProcessImageMotorPageIndexDebug(_currentStep);
                                    else
                                        FillProcessImageMotorPageIndex(_currentStep);
                                    //CamActClass.Instance.StepCurrent++;

                                    break;
                            }

                            Process.NextDuriation = 0;
                            Process.ID = 10120;
                            LogProcessIDTimer(Process.ID, "测试位置=" + _currentStep.ToString());

                            LogProcessIDTimer(Process.ID, "线程创建" + _currentStep.ToString());
                            ////计算线程
                            //System.Threading.Thread thread_DL_Test = new System.Threading.Thread(DLCalPageIndex);
                            //thread_DL_Test.IsBackground = true;
                            //thread_DL_Test.Start(_currentStep);

                            OnTrigger(ResultStatusEnum.COUNTSTART);
                            Task task = new Task(() =>
                            {
                                DLCalPageIndex(_currentStep);
                            });
                            task.Start();

                            LogProcessIDTimer(Process.ID, "线程测试" + _currentStep.ToString());


                            //if (CamActClass.Instance.StepCurrent < CamActClass.Instance.StepCount - 1)
                            //{
                            //    //MACHINE.PLCIO.GetImageOK = true;
                            //    m_DLGetImageOK.Start("C," + CamActClass.Instance.StepCurrent.ToString());
                            //    LogProcessIDTimer(Process.ID, "取像完成 Send==>Sign ImageOK");
                            //}
                            IsGetTestStartOld = false;
                        }
                        break;
                    case 10110:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
                        if (Process.IsTimeup)
                        {
                            if (m_IsTaskRun)
                            {
                                CamActClass.Instance.StepCurrent++;

                                if (CamActClass.Instance.StepCurrent < CamActClass.Instance.StepCount)
                                {
                                    m_DLGetImageOK.Start("C," + (CamActClass.Instance.StepCurrent - 1).ToString());
                                    LogProcessIDTimer(Process.ID, "Step=" +
                                        (CamActClass.Instance.StepCurrent - 1).ToString() + "==>取像完成 Send==>Sign ImageOK");

                                    Process.NextDuriation = 300;
                                    Process.ID = 10130;

                                }
                                else
                                {

                                    Process.NextDuriation = 300;
                                    Process.ID = 10120;
                                }
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
                                switch (Universal.CAMACT)
                                {
                                    case CameraActionMode.CAM_MOTOR_LINESCAN:
                                        Process.NextDuriation = 0;
                                        Process.ID = 30;
                                        break;
                                    default:
                                        Process.NextDuriation = 0;
                                        Process.ID = 1030;
                                        break;
                                }

                                LogProcessIDTimer(Process.ID, "所有页面测试完成");
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

                            //    MACHINE.PLCIO.GetImageOK = false;
                            //    LogProcessIDTimer(Process.ID, "获取取像信号 Rev==>Sign GetImage");
                            //}

                            //if (!MACHINE.PLCIO.GetImageOK && !m_DLGetImageOK.IsOn)
                            //{
                            //    //Trigger Start
                            //    bool isGetImageStart = MACHINE.PLCIO.IsGetImage;

                            //    if (isGetImageStart && IsGetTestStartOld != isGetImageStart)
                            //    {
                            //        Process.NextDuriation = 300;
                            //        Process.ID = 10;

                            //        //MACHINE.PLCIO.GetImageOK = false;

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
                                MACHINE.SetLight("0,0,0,0,0,0,0");

                            Process.NextDuriation = 0;
                            Process.ID = 30;
                        }
                        break;
                    case 20:
                        if (Process.IsTimeup)       //抓圖
                        {
                            //Testms[0] = TestTimer.msDuriation;
                            //TestTimer.Cut();

                            //if (!IsNoUseCCD)
                            //    OnTrigger(ResultStatusEnum.COUNTSTART);

                            OnEnvTrigger(ResultStatusEnum.CHANGEENVDIRECTORY, EnvIndex, PageOPTypeEnum.P00.ToString());

                            //Testms[1] = TestTimer.msDuriation;
                            //TestTimer.Cut();
                            CCDCollection.GetImage();
                            //CCDCollection.GetR32Image();

                            //Testms[1] = TestTimer.msDuriation;
                            //TestTimer.Cut();

                            FillProcessImage();

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
                                case CameraActionMode.CAM_MOTOR_LINESCAN:
                                case CameraActionMode.CAM_MOTOR_MODE2:
                                    if (IsNoUseCCD)
                                        AlbumWork.A08_RunProcess(PageOPTypeEnum.P00);
                                    break;
                                default:
                                    AlbumWork.A08_RunProcess(PageOPTypeEnum.P00);
                                    break;
                            }


                            #region 检查重复的码 和 检查是否在此批号中

                            //if (INI.IsOpenCheckRepeatCode)
                            //{
                            //    bool bOK = AlbumWork.A09_RunRepeatCodeProcess(PageOPTypeEnum.P00);
                            //}

                            #endregion


                            AlbumWork.FillRunStatus(RunStatusCollection);

                            //取得Compound 在這個 ENV 裏的資料
                            AlbumWork.CPD.CollectRUNVIEWData(AlbumWork, AlbumWork.ENVList[EnvIndex].No);

                            Testms[0] = TestTimer.msDuriation;

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
                        }
                        break;
                    case 40:


                        Process.NextDuriation = 0;
                        Process.ID = 50;

                        #region 提前发信号 再显示画面
                        if (INI.IsOpenBehindOKSign)
                        {
                            Task task = new Task(() =>
                            {
                                try
                                {
                                    m_DLResultOK.Start();
                                    LogProcessIDTimer(8887, $"发送结果信号PC==>PLC {(IsPass ? "PASS" : "NG")}");
                                }
                                catch (Exception ex)
                                {
                                    JetEazy.LoggerClass.Instance.WriteException(ex);
                                }
                            });
                            task.Start();
                        }
                        else
                        {
                            m_DLResultOK.Start();
                            LogProcessIDTimer(8888, $"发送结果信号PC==>PLC {(IsPass ? "PASS" : "NG")}");
                        }


                        #endregion

                        break;
                    case 50:

                        Process.Stop();
                        RXXLastProcess();

                        LogProcessIDTimer(Process.ID, "流程结束");
                        LogProcessIDTimer(Process.ID, "==================");
                        LogProcessIDTimer(Process.ID, "==================");
                        LogProcessIDTimer(Process.ID, "==================");
                        GC.Collect();
                        break;
                }
            }


        }
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
                        JetEazy.LoggerClass.Instance.WriteLog($"手动抓取图像{Universal.CAMACT.ToString()}");
                        m_GetImageTimeout.Restart();
                        switch (Universal.CAMACT)
                        {
                            case CameraActionMode.CAM_MOTOR_LINESCAN:

                                m_IxLinescanCamera.IsGrapImageComplete = false;
                                Process.ID = 20;

                                break;
                            default:

                                Process.ID = 10;

                                break;
                        }

                        break;

                    #region LINESCAN GETIMAGE PROCESS

                    case 20:
                        if (Process.IsTimeup)
                        {
                            if (m_IxLinescanCamera.IsGrapImageComplete)
                            {
                                if (m_IxLinescanCamera.IsGrapImageOK)
                                {
                                    Process.Stop();
                                    //using (FreeImageAPI.FreeImageBitmap bmp =
                                    //    new FreeImageAPI.FreeImageBitmap(m_IxLinescanCamera.ImageWidth,
                                    //    m_IxLinescanCamera.ImageHeight,
                                    //    m_IxLinescanCamera.ImageWidth,
                                    //    PixelFormat.Format8bppIndexed,
                                    //    m_IxLinescanCamera.ImagePbuffer)
                                    //)
                                    using (Bitmap bitmap = m_IxLinescanCamera.GetFreeImageBitmap().ToBitmap())
                                    {
                                        //bmp.Rotate(m_IxLinescanCamera.ImageRotate);

                                        if (CamActClass.Instance.StepCurrent >= CamActClass.Instance.StepCount)
                                            CamActClass.Instance.ResetStepCurrent();
                                        //bitmap.Save("D:\\LOA\\TEST.PNG", ImageFormat.Png);
                                        CamActClass.Instance.SetImage(bitmap, CamActClass.Instance.StepCurrent);
                                        OnTriggerOP(ResultStatusEnum.SET_CURRENT_IMAGE, "ONLINE#" + CamActClass.Instance.StepCurrent.ToString());
                                        CamActClass.Instance.StepCurrent++;

                                        m_DLGetImageOK.Start();
                                    }
                                    JetEazy.LoggerClass.Instance.WriteLog($"手动抓取图像完成{Universal.CAMACT.ToString()}");

                                }
                                else
                                {
                                    Process.Stop();
                                    //失败
                                    M_WARNING_FRM = new MessageForm(true, "线扫抓图失败，请检查。。。", "");
                                    if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                                    {
                                    }
                                    M_WARNING_FRM.Close();
                                    M_WARNING_FRM.Dispose();
                                }
                            }
                            else
                            {
                                //可以添加超时
                                if (m_GetImageTimeout.ElapsedMilliseconds >= 30 * 1000)
                                {
                                    Process.Stop();
                                    M_WARNING_FRM = new MessageForm(true, "线扫抓图超时，请检查。。。", "");
                                    if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
                                    {
                                    }
                                    M_WARNING_FRM.Close();
                                    M_WARNING_FRM.Dispose();
                                }
                            }
                        }
                        break;

                    #endregion

                    case 10:
                        if (Process.IsTimeup)
                        {
                            Process.Stop();
                            Universal.CCDCollection.GetImage();
                            //Bitmap bitmap = Universal.CCDCollection.GetBMP(0, false);
                            using (Bitmap bitmap = Universal.CCDCollection.GetBMP(0, false))
                            {
                                if (CamActClass.Instance.StepCurrent >= CamActClass.Instance.StepCount)
                                    CamActClass.Instance.ResetStepCurrent();
                                CamActClass.Instance.SetImage(bitmap, CamActClass.Instance.StepCurrent);
                                OnTriggerOP(ResultStatusEnum.SET_CURRENT_IMAGE, "ONLINE#" + CamActClass.Instance.StepCurrent.ToString());
                                CamActClass.Instance.StepCurrent++;

                                m_DLGetImageOK.Start();
                            }
                            JetEazy.LoggerClass.Instance.WriteLog($"手动抓取图像完成{Universal.CAMACT.ToString()}");


                        }
                        break;
                }
            }
        }

        #region BAK 20231109
        /*
         * 
         * 
        
        private void DLCalPageOneStepIndex(object obj)
        {
            if (EnvIndex >= AlbumWork.ENVList.Count)
            {
                RunStepComplete = true;
                return;
            }
            //RunStepComplete = true;
            //return;
            RunStepComplete = true;
            int pageindex = (int)obj;
            System.Diagnostics.Stopwatch watchThreadTime = new System.Diagnostics.Stopwatch();
            watchThreadTime.Restart();

            PageClass page = m_EnvNow.PageList[pageindex];

            int pageindex2 = page.No;
            Universal.CalPageIndex = pageindex;

            AlbumWork.SetPageTestState(pageindex2, false);

            PageClass page0 = AlbumWork.ENVList[EnvIndex].PageList[pageindex2];
            foreach (AnalyzeClass analyze in page0.AnalyzeRoot.BranchList)
            {
                if (analyze.CheckAnalyzeReadBarcode())
                {
                    analyze.ResetAnalyzeBarcodeStr();
                    break;
                }
            }

            Point ptRestore = new Point(m_AlignFristOffset.X, m_AlignFristOffset.Y);
            //偏移所有框的位置
            AlbumWork.SetOffset(pageindex2, ptRestore, true);
            AlbumWork.A08_RunProcess(PageOPTypeEnum.P00, pageindex2);
            AlbumWork.SetPageTestState(pageindex2, true);

            RunStatusCollectionTemp.Clear();
            PageClass page1 = AlbumWork.ENVList[EnvIndex].PageList[pageindex2];
            page1.FillRunStatus(RunStatusCollectionTemp);

            ////填入条码
            //if (string.IsNullOrEmpty(Universal.CalTestBarcode))
            //{

            //}

            foreach (WorkStatusClass workStatusClass in RunStatusCollectionTemp.WorkStatusList)
            {
                RunStatusCollection.Add(workStatusClass.Clone());
            }

            EnvAnalyzePostionSettings envAnalyzePostion = new EnvAnalyzePostionSettings(AlbumWork.ENVList[EnvIndex].GeneralPosition);

            int tmprow = 0;
            int tmpcol = 0;
            int i = 0;

            int itemwidth = 230;
            int itemheight = 230;
            foreach (AnalyzeClass analyze in page1.AnalyzeRoot.BranchList)
            {
                if (itemwidth < analyze.PADPara.bmpMeasureOutput.Width)
                {
                    itemwidth = analyze.PADPara.bmpMeasureOutput.Width;
                }
                if (itemheight < analyze.PADPara.bmpMeasureOutput.Height)
                {
                    itemheight = analyze.PADPara.bmpMeasureOutput.Height;
                }
            }

            Universal.SDM2_BMP_SHOW_CURRENT.Dispose();
            Universal.SDM2_BMP_SHOW_CURRENT = new Bitmap(itemwidth * 2, itemheight * 2);
            Graphics graphics = Graphics.FromImage(Universal.SDM2_BMP_SHOW_CURRENT);


            int drawrow = 0;
            int drawcol = 0;

            for (i = 0; i < page.AnalyzeRoot.BranchList.Count; i++)
            {
                DataMappingClass dataMapping = new DataMappingClass();

                dataMapping.PtfCenter = new PointF(page.PageRunLocation.X - (float)(tmpcol * envAnalyzePostion.GetImageCountXoffset * 1.0 / 2),
                                                                            page.PageRunLocation.Y + (float)(tmprow * envAnalyzePostion.GetImageCountYoffset * 1.0 / 2));

                if (!page.AnalyzeRoot.BranchList[i].CheckAnalyzeReadBarcode())
                {
                    if (page.AnalyzeRoot.BranchList[i].DataReportIndex % page.m_Mapping_Col == 0)
                    {
                        tmprow++;
                        tmpcol = 0;
                    }
                    else
                    {
                        tmpcol++;
                    }

                    //这里添加测试完成页面的结果
                    foreach (AnalyzeClass analyze in page1.AnalyzeRoot.BranchList)
                    {
                        string a1 = analyze.ToAnalyzeString();
                        string a2 = page.AnalyzeRoot.BranchList[i].ToAnalyzeString();

                        if (a1 == a2)
                        {
                            graphics.DrawImage(analyze.PADPara.bmpMeasureOutput,
                                                              new RectangleF(new PointF(0 + itemwidth * drawcol, 0 + itemheight * drawrow), new Size(itemwidth, itemheight)),
                                                              new RectangleF(new PointF(0, 0), analyze.PADPara.bmpMeasureOutput.Size),
                                                              GraphicsUnit.Pixel);

                            dataMapping.bmpResult.Dispose();
                            dataMapping.bmpResult = new Bitmap(analyze.PADPara.bmpMeasureOutput);

                            if (analyze.IsVeryGood)
                            {
                                dataMapping.ReportBinValue = 0;
                                if (analyze.PADPara.PADMethod == PADMethodEnum.GLUECHECK)
                                {
                                    if (string.IsNullOrEmpty(analyze.PADPara.DescStr))
                                    {
                                        analyze.CalculateChipWidth();
                                        dataMapping.ReportStr = analyze.ToReportString1();
                                    }
                                    else
                                    {
                                        if (analyze.PADPara.DescStr.Contains("无胶"))
                                        {
                                            dataMapping.ReportBinValue = 1;
                                        }
                                        else if (analyze.PADPara.DescStr.Contains("尺寸"))
                                        {
                                            dataMapping.ReportBinValue = 2;
                                        }
                                        else if (analyze.PADPara.DescStr.Contains("溢胶"))
                                        {
                                            dataMapping.ReportBinValue = 3;
                                        }
                                        else if (analyze.PADPara.DescStr.Contains("胶水异常"))
                                        {
                                            dataMapping.ReportBinValue = 4;
                                        }
                                        else
                                        {
                                            dataMapping.ReportBinValue = 5;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                dataMapping.ReportStr = string.Empty;
                                if (analyze.PADPara.DescStr.Contains("无胶"))
                                {
                                    dataMapping.ReportBinValue = 1;
                                }
                                else if (analyze.PADPara.DescStr.Contains("尺寸"))
                                {
                                    dataMapping.ReportBinValue = 2;
                                }
                                else if (analyze.PADPara.DescStr.Contains("溢胶"))
                                {
                                    dataMapping.ReportBinValue = 3;
                                }
                                else if (analyze.PADPara.DescStr.Contains("胶水异常"))
                                {
                                    dataMapping.ReportBinValue = 4;
                                }
                                else
                                {
                                    dataMapping.ReportBinValue = 5;
                                }
                            }
                            break;
                        }
                    }

                    if (i != 0 && i % 2 == 1)
                    {
                        drawrow++;
                        drawcol = 0;
                    }
                    else
                    {
                        drawcol++;
                    }

                    RunDataMappingCollection.Add(dataMapping);
                }
                else
                {
                    foreach (AnalyzeClass analyze in page1.AnalyzeRoot.BranchList)
                    {
                        if (analyze.CheckAnalyzeReadBarcode())
                        {
                            Universal.CalTestBarcode = analyze.GetAnalyzeBarcodeStr();
                            OnTriggerOP(ResultStatusEnum.SHOW_BARCODE_RESULT, Universal.CalTestBarcode);
                            if (INI.chipUseAI)
                            {
                                if (string.IsNullOrEmpty(Universal.CalTestBarcode) || Universal.IsNoUseIO)
                                {
                                    //如果没有条码则开启新的线程读码 利用AI的服务器

                                    //Task task = new Task(() =>
                                    //{
                                    //    try
                                    //    {
                                    //        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                                    //        stopwatch.Restart();
                                    //        OnTriggerOP(ResultStatusEnum.SHOW_BARCODE_RESULT, "解码中...");
                                    //        EzSegDMTX ezSegDMTX = new EzSegDMTX();
                                    //        //Rectangle rectangleAI = JzTools.SimpleRect(Universal.bmpProvideAI.Size);
                                    //        //rectangleAI.Inflate(-15, -15);
                                    //        //Bitmap bmp222ai = (Bitmap)Universal.bmpProvideAI.Clone(rectangleAI, PixelFormat.Format24bppRgb);
                                    //        Bitmap bmp222ai = new Bitmap(Universal.bmpProvideAI);

                                    //        //bmp222ai.Save("D:\\AI_ORG.png", ImageFormat.Png);
                                    //        ezSegDMTX.InputImage = bmp222ai;
                                    //        int iret = ezSegDMTX.Run();


                                    //        stopwatch.Stop();
                                    //        if (iret == 0)
                                    //        {
                                    //            Universal.CalTestBarcode = ezSegDMTX.BarcodeStr;
                                    //            OnTriggerOP(ResultStatusEnum.SHOW_BARCODE_RESULT, Universal.CalTestBarcode);

                                    //            //barcodeStrRead = IxBarcode.BarcodeStr;
                                    //            //myBarcode = IxBarcode.BarcodeStr + " Model用时:" + stopwatch.ElapsedMilliseconds.ToString() + " ms";
                                    //        }
                                    //        else
                                    //        {
                                    //            OnTriggerOP(ResultStatusEnum.SHOW_BARCODE_RESULT, "解码失败");
                                    //        }
                                    //    }
                                    //    catch
                                    //    {

                                    //    }
                                    //});
                                    //task.Start();

                                }
                            }
                        }
                    }
                }
            }

            graphics.Dispose();
            //Universal.SDM2_BMP_SHOW_CURRENT.Save("D:\\TEST001.BMP");
            ////放出图像显示
            //OnTriggerShowImageCurrent(bmpshowCurrent);
            OnTrigger(ResultStatusEnum.SHOW_CURRENT_IMAGE);

            watchThreadTime.Stop();
            long _time = watchThreadTime.ElapsedMilliseconds;
            if (_time > INI.MAINSD_GETIMAGE_DELAYTIME)
                RunNextCCDDelayTime = 50;
            else
                RunNextCCDDelayTime = INI.MAINSD_GETIMAGE_DELAYTIME - (int)_time;

            _LOG_MSG("Step=" + pageindex.ToString() + " 用时=" + watchThreadTime.ElapsedMilliseconds + "ms");
            RunStepComplete = true;

            ////还原所有框的位置
            //ptRestore = new Point(-m_AlignFristOffset.X, -(m_AlignFristOffset.Y));
            AlbumWork.SetOffset(pageindex2, ptRestore, false);
        }

        void MainSDM2Tick()
        {
            ProcessClass Process = MainProcess;

            if (Process.IsOn)
            {
                if (m_ProcessTmpID != Process.ID)
                {
                    m_ProcessTmpID = Process.ID;
                    _LOG_MSG(Process.ID.ToString() + " MainSDM2Tick Running...");
                }

                switch (Process.ID)
                {
                    case 5:

                        //m_EnvNow = new EnvClass();
                        RunDataMappingCollection.Clear();


                        LogProcessSwitch = true;
                        LogProcessIDTimer(Process.ID, "流程开始");
                        _LOG_MSG(Process.ID.ToString() + " MainSDM2Tick " + "流程开始");

                        MACHINE.PLCIO.Busy = true;
                        //MACHINE.SetLight("1,1,1");

                        if (INI.IsCollectPictures)
                            Universal.MainX6_Path = "D:\\CollectPictures_" + Universal.OPTION.ToString() + "\\pictures\\" + JzTimes.DateSerialString;

                        TestTimer.Cut();
                        m_input_time = DateTime.Now;
                        OnEnvTrigger(ResultStatusEnum.CHANGEDIRECTORY, 0, "");
                        OnTrigger(ResultStatusEnum.COUNTSTART);
                        //把要檢測的東西放進去
                        FillOperaterString(RELATECOLORSTR);
                        Universal.CalTestBarcode = string.Empty;
                        if (Universal.IsNoUseIO)
                        {
                            string[] _dirStrs = LastDirPath.Split('\\');
                            Universal.CalTestBarcode = _dirStrs[_dirStrs.Length - 1];
                            Universal.CalTestPath = @"D:\TESTCOLLECT\" + Universal.CalTestBarcode;
                        }
                        else
                            Universal.CalTestPath = @"D:\TESTCOLLECT\" + JzTimes.DateTimeSerialString;

                        if (!Directory.Exists(Universal.CalTestPath))
                            Directory.CreateDirectory(Universal.CalTestPath);

                        Process.NextDuriation = 0;

                        if (IsNoUseCCD)
                            Process.ID = 20;
                        else
                            Process.ID = 10;


                        switch (Universal.CAMACT)
                        {
                            case CameraActionMode.CAM_MOTOR_MODE2:

                                #region no use mutil pages
                                //int ipageindex = 0;
                                //foreach (PageClass page in AlbumWork.ENVList[0].PageList)
                                //{
                                //    //获取位置
                                //    if (string.IsNullOrEmpty(page.sPagePostion))
                                //    {
                                //        //没有的位置的页面不添加
                                //        //PageClass page1 = page.Clone();
                                //        //page1.PageRunNo = ipageindex;
                                //        //page1.PageRunPos = "0,0,0";
                                //        //ipageindex++;

                                //        //m_EnvNow.PageList.Add(page1);
                                //    }
                                //    else
                                //    {
                                //        foreach (string pos in page.PagePostionList)
                                //        {
                                //            PageClass page1 = page.Clone();
                                //            page1.PageRunNo = ipageindex;
                                //            page1.PageRunPos = pos;
                                //            ipageindex++;

                                //            m_EnvNow.PageList.Add(page1);
                                //        }
                                //    }
                                //}
                                //m_EnvNow.EnvAutoReportIndex();
                                ////训练线程
                                //System.Threading.Thread thread_Train = new System.Threading.Thread(DLTrainSDM2);
                                //thread_Train.Start(new object());

                                //m_EnvNow.ResetTrainStatus();
                                //m_EnvNow.A00_TrainProcess(true);
                                #endregion

                                GeneralPosition = AlbumWork.ENVList[0].GeneralPosition;
                                GeneralPositions = GeneralPosition.Split(';');
                                GeneralPositionIndex = 0;

                                CamActClass.Instance.SetStepCount(m_EnvNow.PageList.Count);//这里设定抓取图像的所有位置

                                CamActClass.Instance.StepCurrent = 0;

                                break;
                        }

                        Process.NextDuriation = INI.MAINSDM1_GETSTART_DELAYTIME;
                        //Process.ID = 10;
                        _LOG_MSG(Process.ID.ToString() + " MainSDM1Tick 延时启动时间" + INI.MAINSDM1_GETSTART_DELAYTIME.ToString());
                        RunNextCCDDelayTime = INI.MAINSD_GETIMAGE_DELAYTIME;

                        break;
                    case 10:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
                        if (Process.IsTimeup)
                        {
                            switch (Universal.CAMACT)
                            {
                                case CameraActionMode.CAM_MOTOR_MODE2:
                                    //if (m_EnvTrainOK)
                                    {
                                        m_CurrentPosition = _getCurrentPos(GeneralPositionIndex);
                                        _LOG_MSG(Process.ID.ToString() + " MainSDM2Tick POS=" + m_CurrentPosition);

                                        MACHINE.GoPosition(m_CurrentPosition);
                                        GeneralPositionIndex++;

                                        OnEnvTrigger(ResultStatusEnum.SETCAMLIGHT, EnvIndex, "ALL");
                                        Process.NextDuriation = 200;
                                        Process.ID = 10090;

                                        RunStepComplete = true;//第一次强制给完成信号
                                        m_IsAlignComplete = false;//第一次复位标志
                                    }

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
                            if (MACHINE.IsOnSite() && MACHINE.IsOnSitePosition(m_CurrentPosition) && RunStepComplete || (Universal.IsNoUseMotor && RunStepComplete))
                            //if (MACHINE.IsOnSite() && MACHINE.IsOnSitePosition(m_CurrentPosition) || (Universal.IsNoUseMotor && RunStepComplete))

                            {
                                RunStepComplete = false;
                                Process.NextDuriation = RunNextCCDDelayTime;// INI.MAINSD_GETIMAGE_DELAYTIME;
                                Process.ID = 10100;
                                MACHINE.PLCIO.TopLight = true;
                                LogProcessIDTimer(Process.ID, "取像延时=" + RunNextCCDDelayTime.ToString());
                                //LogProcessIDTimer(Process.ID, "取像延时=" + INI.MAINSD_GETIMAGE_DELAYTIME.ToString());

                                if (Universal.IsNoUseIO)
                                {
                                    OnEnvTrigger(ResultStatusEnum.CHANGEENVDIRECTORY, EnvIndex, PageOPTypeEnum.P00.ToString());
                                }
                            }
                        }
                        break;
                    case 10100:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.IsOnSite() && MACHINE.IsOnSitePosition(m_CurrentPosition) || Universal.IsNoUseMotor)
                            {
                                //抓图
                                if (Universal.IsNoUseIO)
                                    CCDCollection.GetImageSDM1(0, CamActClass.Instance.StepCurrent);
                                else
                                {
                                    CCDCollection.GetImage();
                                    //CCDCollection.GetImage();
                                }
                                //CCDCollection.GetImage();
                                if (CamActClass.Instance.StepCurrent >= CamActClass.Instance.StepCount)
                                    CamActClass.Instance.ResetStepCurrent();
                                FillProcessImageMotorPageIndexSDM2(CamActClass.Instance.StepCurrent);
                                if (!m_IsAlignComplete)
                                {
                                    m_IsAlignComplete = true;
                                    FillProcessImageMotorAlignSDM2(CamActClass.Instance.StepCurrent);
                                }
                                //CamActClass.Instance.StepCurrent++;
                                //>> 20230608
                                //CamActClass.Instance.SetImage(CCDCollection.GetBMP(0, false), CamActClass.Instance.StepCurrent);

                                //图片填充完成就可以触发马达前进了
                                #region

                                Task task = new Task(() =>
                                {
                                    m_CurrentPosition = _getCurrentPos(GeneralPositionIndex);
                                    _LOG_MSG(Process.ID.ToString() + " task-MainSDM2Tick POS=" + m_CurrentPosition);
                                    MACHINE.PLCIO.TopLight = false;
                                    MACHINE.GoPosition(m_CurrentPosition);
                                    GeneralPositionIndex++;

                                    //OnEnvTrigger(ResultStatusEnum.SETCAMLIGHT, EnvIndex, "ALL");
                                });
                                task.Start();

                                #endregion

                                Process.NextDuriation = 0;
                                Process.ID = 10110;

                                LogProcessIDTimer(Process.ID, "测试位置=" + CamActClass.Instance.StepCurrent.ToString());
                            }
                        }
                        break;
                    case 10110:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
                        if (Process.IsTimeup)
                        {
                            //>>单步测试
                            ////计算线程
                            //System.Threading.Thread thread_DL_Test = new System.Threading.Thread(DLCalPageIndexSDM2);
                            //thread_DL_Test.Start(CamActClass.Instance.StepCurrent);

                            System.Threading.Thread thread_DL_Test = new System.Threading.Thread(DLCalPageOneStepIndex);
                            thread_DL_Test.Start(CamActClass.Instance.StepCurrent);

                            //DLCalPageOneStepIndex(CamActClass.Instance.StepCurrent);
                            CamActClass.Instance.StepCurrent++;

                            if (CamActClass.Instance.StepCurrent < CamActClass.Instance.StepCount)
                            {
                                Process.NextDuriation = 0;
                                //Process.ID = 10;
                                Process.ID = 10090;//这里是取完图后立马让马达运动 这样节省时间

                                LogProcessIDTimer(Process.ID, "取像完成 Send==>Sign ImageOK");
                            }
                            else
                            {

                                Process.NextDuriation = 0;
                                Process.ID = 10120;

                                MACHINE.GoReadyPosition();

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
                            if (_getAllPageComplete() || RunStepComplete)
                            {
                                Process.NextDuriation = 0;
                                Process.ID = 1030;

                                LogProcessIDTimer(Process.ID, "所有页面测试完成");
                                _LOG_MSG(Process.ID.ToString() + " MainSDM2Tick " + "所有页面测试完成");
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
                            //>>20230608
                            //m_EnvNow.FillRunStatus(RunStatusCollection);

                            //AlbumWork.FillRunStatus(RunStatusCollection);

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
                                    if (workStatus.AnalyzeProcedure == AnanlyzeProcedureEnum.CHECKBARCODE)
                                    {
                                        if (workStatus.Reason == ReasonEnum.NG)
                                        {
                                            workStatus.bmpRUN.Save("D:\\Barcode2D.png", ImageFormat.Png);
                                        }
                                        continue;
                                    }

                                    if (workStatus.AnalyzeProcedure != AnanlyzeProcedureEnum.ALIGNRUN)
                                    {
                                        WorkStatusClass workStatus1 = workStatus.Clone();
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
                            _LOG_MSG(Process.ID.ToString() + " MainSDM2Tick " + "整合资料结果完成");
                        }
                        break;
                    case 40:

                        Process.Stop();
                        RXXLastProcess();

                        _LOG_MSG(Process.ID.ToString() + " MainSDM2Tick " + "流程结束");
                        LogProcessIDTimer(Process.ID, "流程结束");
                        LogProcessIDTimer(Process.ID, "==================");
                        LogProcessIDTimer(Process.ID, "==================");
                        LogProcessIDTimer(Process.ID, "==================");
                        GC.Collect();
                        break;
                }
            }


        }
        */
        #endregion

        bool m_IsTaskRun = false;

        private void DLCalPageIndex(object obj)
        {
            m_IsTaskRun = false;
            int pageindex = (int)obj;
            DateTime dtstart = DateTime.Now;

            m_IsTaskRun = true;

            JetEazy.LoggerClass.Instance.WriteLog("线程开始页面=" + pageindex.ToString());
            //LogProcessIDTimer(68888, "页面=" + pageindex.ToString() + "_线程开始时间=" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            try
            {
                EnvClass env = AlbumWork.ENVList[EnvIndex];

                if (pageindex >= env.PageList.Count || pageindex < 0)
                {
                    //LogProcessIDTimer(68899, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "页面超出，值=" + pageindex.ToString());
                    JetEazy.LoggerClass.Instance.WriteLog("页面超出，值=" + pageindex.ToString());
                    return;
                }

                AlbumWork.SetPageTestState(pageindex, false);
                AlbumWork.A08_RunProcess(PageOPTypeEnum.P00, pageindex);
                AlbumWork.SetPageTestState(pageindex, true);
            }
            catch (Exception ex)
            {
                //AlbumWork.SetPageTestState(pageindex, false);
                JetEazy.LoggerClass.Instance.WriteException(ex);
                JetEazy.LoggerClass.Instance.WriteLog("异常退出，页面=" + pageindex.ToString());
                //LogProcessIDTimer(68888, "页面=" + pageindex.ToString() + "异常退出=" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            }
            JetEazy.LoggerClass.Instance.WriteLog("线程结束页面=" + pageindex.ToString());

            TimeSpan span = DateTime.Now - dtstart;
            JetEazy.LoggerClass.Instance.WriteLog("页面=" + pageindex.ToString() + " 耗时:" + span.TotalMilliseconds.ToString() + " ms");


            //LogProcessIDTimer(68888, "页面=" + pageindex.ToString() + "_线程结束时间=" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        }

        string _getCurrentPos(int index)
        {
            //if (index >= m_EnvNow.PageList.Count)
            //    return MACHINE.GetReadyPosition();
            //return "0,0,0";
            return m_EnvNow.PageList[index].PageRunPos;
        }
        string _getReadyPos()
        {
            return "";// MACHINE.GetReadyPosition();
        }
        bool _getAllPageComplete()
        {
            bool ret = true;
            EnvClass env = m_EnvNow;
            foreach (var item in env.PageList)
            {
                ret &= item.CalComplete;
            }
            return ret;
        }

        #region BAK20230531

        //void MainSDM2Tick()
        //{
        //    ProcessClass Process = MainProcess;

        //    if (Process.IsOn)
        //    {
        //        if (m_ProcessTmpID != Process.ID)
        //        {
        //            m_ProcessTmpID = Process.ID;
        //            _LOG_MSG(Process.ID.ToString() + " MainSDM1Tick Running...");
        //        }

        //        switch (Process.ID)
        //        {
        //            case 5:

        //                LogProcessSwitch = true;
        //                LogProcessIDTimer(Process.ID, "流程开始");
        //                _LOG_MSG(Process.ID.ToString() + " MainSDM1Tick " + "流程开始");

        //                MACHINE.PLCIO.Busy = true;
        //                MACHINE.SetLight("1,1,1");

        //                if (INI.IsCollectPictures)
        //                    Universal.MainX6_Path = "D:\\CollectPictures_" + Universal.OPTION.ToString() + "\\pictures\\" + JzTimes.DateSerialString;


        //                TestTimer.Cut();
        //                m_input_time = DateTime.Now;
        //                OnEnvTrigger(ResultStatusEnum.CHANGEDIRECTORY, 0, "");
        //                OnTrigger(ResultStatusEnum.COUNTSTART);
        //                //把要檢測的東西放進去
        //                FillOperaterString(RELATECOLORSTR);

        //                Process.NextDuriation = 0;

        //                if (IsNoUseCCD)
        //                    Process.ID = 20;
        //                else
        //                    Process.ID = 10;


        //                switch (Universal.CAMACT)
        //                {
        //                    case CameraActionMode.CAM_MOTOR_MODE2:

        //                        GeneralPosition = AlbumWork.ENVList[0].GeneralPosition;
        //                        GeneralPositions = GeneralPosition.Split(';');
        //                        GeneralPositionIndex = 0;

        //                        ENVAnalyzePostion = new EnvAnalyzePostionSettings(GeneralPosition);
        //                        ENVAnalyzePostion.EnvAnalyzePostions();

        //                        ENVAnalyzePostion.EnvAnalyzePostionsSDM2();

        //                        CamActClass.Instance.SetStepCount(ENVAnalyzePostion.GetImagePostions.Length);//这里设定抓取图像的所有位置

        //                        CamActClass.Instance.StepCurrent = 0;

        //                        break;
        //                }

        //                Process.NextDuriation = INI.MAINSDM1_GETSTART_DELAYTIME;
        //                //Process.ID = 10;
        //                _LOG_MSG(Process.ID.ToString() + " MainSDM1Tick 延时启动时间" + INI.MAINSDM1_GETSTART_DELAYTIME.ToString());


        //                break;
        //            case 10:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
        //                if (Process.IsTimeup)
        //                {
        //                    switch (Universal.CAMACT)
        //                    {
        //                        case CameraActionMode.CAM_MOTOR_MODE2:

        //                            _LOG_MSG(Process.ID.ToString() + " MainSDM1Tick POS=" + ENVAnalyzePostion.GetImagePostions[GeneralPositionIndex].ToString());

        //                            MACHINE.GoPosition(ENVAnalyzePostion.GetImagePostions[GeneralPositionIndex].ToString());

        //                            //_LOG_MSG(Process.ID.ToString() + " MainSDM1Tick POS=" + GeneralPositions[GeneralPositionIndex]);

        //                            //MACHINE.GoXPosition(GeneralPositions[GeneralPositionIndex]);
        //                            GeneralPositionIndex++;

        //                            OnEnvTrigger(ResultStatusEnum.SETCAMLIGHT, EnvIndex, "ALL");
        //                            Process.NextDuriation = 200;
        //                            Process.ID = 10090;

        //                            break;
        //                        case CameraActionMode.CAM_MOTOR:
        //                            Process.NextDuriation = 500;
        //                            Process.ID = 1030;
        //                            break;
        //                        case CameraActionMode.CAM_STATIC:
        //                            OnEnvTrigger(ResultStatusEnum.SETCAMLIGHT, EnvIndex, "ALL");
        //                            Process.NextDuriation = 500;
        //                            Process.ID = 1030;
        //                            break;
        //                    }
        //                }
        //                break;


        //            #region 马达模式2  
        //            case 10090:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
        //                if (Process.IsTimeup)
        //                {
        //                    if (MACHINE.IsOnSite() || Universal.IsNoUseMotor)
        //                    {
        //                        Process.NextDuriation = INI.MAINSD_GETIMAGE_DELAYTIME;
        //                        Process.ID = 10100;

        //                        LogProcessIDTimer(Process.ID, "取像延时=" + INI.MAINSD_GETIMAGE_DELAYTIME.ToString());

        //                    }
        //                }
        //                break;
        //            case 10100:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
        //                if (Process.IsTimeup)
        //                {
        //                    if (MACHINE.IsOnSite() || Universal.IsNoUseMotor)
        //                    {
        //                        //抓图
        //                        CCDCollection.GetImage();
        //                        //CCDCollection.GetImage();
        //                        if (CamActClass.Instance.StepCurrent >= CamActClass.Instance.StepCount)
        //                            CamActClass.Instance.ResetStepCurrent();
        //                        FillProcessImageMotorPageIndex(CamActClass.Instance.StepCurrent);
        //                        //CamActClass.Instance.StepCurrent++;

        //                        CamActClass.Instance.SetImage(CCDCollection.GetBMP(0, false), CamActClass.Instance.StepCurrent);

        //                        Process.NextDuriation = 0;
        //                        Process.ID = 10110;

        //                        LogProcessIDTimer(Process.ID, "测试位置=" + CamActClass.Instance.StepCurrent.ToString());
        //                    }
        //                }
        //                break;
        //            case 10110:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
        //                if (Process.IsTimeup)
        //                {

        //                    //计算线程
        //                    System.Threading.Thread thread_DL_Test = new System.Threading.Thread(DLCalPageIndex);
        //                    thread_DL_Test.Start(CamActClass.Instance.StepCurrent);

        //                    CamActClass.Instance.StepCurrent++;

        //                    if (CamActClass.Instance.StepCurrent < CamActClass.Instance.StepCount)
        //                    {
        //                        ////发送拍照完成信号
        //                        //System.Threading.Thread thread = new System.Threading.Thread(SendSignToPLCImageOK);
        //                        //thread.Start();

        //                        //m_DLGetImageOK.Start();

        //                        //IsGetImageStartOld = true;

        //                        Process.NextDuriation = 0;
        //                        Process.ID = 10;


        //                        LogProcessIDTimer(Process.ID, "取像完成 Send==>Sign ImageOK");
        //                    }
        //                    else
        //                    {

        //                        Process.NextDuriation = 0;
        //                        Process.ID = 10120;

        //                        MACHINE.GoXReadyPosition();

        //                        //马达回待命位置
        //                        LogProcessIDTimer(Process.ID, "取像完成 Motor Ready");
        //                    }

        //                }
        //                break;
        //            case 10120:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
        //                if (Process.IsTimeup)
        //                {
        //                    //最后一次抓完
        //                    //if (AlbumWork.GetPageTestState(CamActClass.Instance.StepCurrent - 1))
        //                    if (AlbumWork.GetAllPageTestComplete())
        //                    {
        //                        Process.NextDuriation = 0;
        //                        Process.ID = 1030;

        //                        LogProcessIDTimer(Process.ID, "所有页面测试完成");
        //                        _LOG_MSG(Process.ID.ToString() + " MainSDM1Tick " + "所有页面测试完成");
        //                    }
        //                }
        //                break;
        //            case 10130:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
        //                if (Process.IsTimeup)
        //                {
        //                    ////等待信号抓图
        //                    //if (MACHINE.PLCIO.IsGetImage)
        //                    //{
        //                    //    Process.NextDuriation = 0;
        //                    //    Process.ID = 10;
        //                    //}

        //                    //if (!MACHINE.PLCIO.GetImageOK && !m_DLGetImageOK.IsOn)
        //                    //{
        //                    //    //Trigger Start
        //                    //    bool isGetImageStart = MACHINE.PLCIO.IsGetImage;

        //                    //    if (isGetImageStart && IsGetTestStartOld != isGetImageStart)
        //                    //    {
        //                    //        Process.NextDuriation = 0;
        //                    //        Process.ID = 10;

        //                    //        LogProcessIDTimer(Process.ID, "获取取像信号 Rev==>Sign GetImage");
        //                    //    }
        //                    //    IsGetTestStartOld = isGetImageStart;
        //                    //}

        //                }
        //                break;

        //            #endregion


        //            case 1030:                        //拍片完後關燈
        //                if (Process.IsTimeup)
        //                {

        //                    switch (Universal.CAMACT)
        //                    {
        //                        case CameraActionMode.CAM_MOTOR:
        //                            FillProcessImageMotor();
        //                            break;
        //                        case CameraActionMode.CAM_STATIC:
        //                            CCDCollection.GetImage();
        //                            FillProcessImage();
        //                            break;
        //                    }

        //                    OnEnvTrigger(ResultStatusEnum.SAVEDEBUGRAW, EnvIndex, "-1");

        //                    if (!INI.IsLightAlwaysOn)
        //                        MACHINE.SetLight("");

        //                    Process.NextDuriation = 0;
        //                    Process.ID = 30;
        //                }
        //                break;
        //            case 20:
        //                if (Process.IsTimeup)       //抓圖
        //                {
        //                    //Testms[0] = TestTimer.msDuriation;
        //                    //TestTimer.Cut();

        //                    if (!IsNoUseCCD)
        //                        OnTrigger(ResultStatusEnum.COUNTSTART);

        //                    OnEnvTrigger(ResultStatusEnum.CHANGEENVDIRECTORY, EnvIndex, PageOPTypeEnum.P00.ToString());

        //                    //Testms[1] = TestTimer.msDuriation;
        //                    //TestTimer.Cut();
        //                    //CCDCollection.GetImage();
        //                    //CCDCollection.GetR32Image();

        //                    //Parallel.For(0, AlbumWork.ENVList[EnvIndex].PageList.Count, i =>
        //                    //{
        //                    //    CCDCollection.GetImageSDM1(0, i);
        //                    //    PageClass page = AlbumWork.ENVList[EnvIndex].PageList[i];
        //                    //    page.SetbmpRUN(PageOPTypeEnum.P00, CCDCollection.GetBMP(page.CamIndex, false));
        //                    //});

        //                    int pageindex = 0;
        //                    foreach (var item in AlbumWork.ENVList[EnvIndex].PageList)
        //                    {
        //                        CCDCollection.GetImageSDM1(0, pageindex);
        //                        PageClass page = AlbumWork.ENVList[EnvIndex].PageList[pageindex];
        //                        page.SetbmpRUN(PageOPTypeEnum.P00, CCDCollection.GetBMP(page.CamIndex, false));

        //                        CamActClass.Instance.SetImage(CCDCollection.GetBMP(page.CamIndex, false), pageindex);

        //                        pageindex++;
        //                    }

        //                    //Testms[1] = TestTimer.msDuriation;
        //                    //TestTimer.Cut();

        //                    //FillProcessImage();

        //                    //Testms[2] = TestTimer.msDuriation;
        //                    //TestTimer.Cut();

        //                    Process.NextDuriation = 0;
        //                    Process.ID = 30;
        //                }
        //                break;
        //            case 30:
        //                if (Process.IsTimeup)
        //                {
        //                    //Testms[3] = TestTimer.msDuriation;
        //                    TestTimer.Cut();

        //                    if (IsNoUseCCD)
        //                        OnTrigger(ResultStatusEnum.COUNTSTART);


        //                    switch (Universal.CAMACT)
        //                    {
        //                        case CameraActionMode.CAM_MOTOR_MODE2:
        //                            if (IsNoUseCCD)
        //                                AlbumWork.A08_RunProcess(PageOPTypeEnum.P00);

        //                            break;
        //                        default:
        //                            AlbumWork.A08_RunProcess(PageOPTypeEnum.P00);
        //                            break;
        //                    }

        //                    AlbumWork.FillRunStatus(RunStatusCollection);

        //                    //取得Compound 在這個 ENV 裏的資料
        //                    AlbumWork.CPD.CollectRUNVIEWData(AlbumWork, AlbumWork.ENVList[EnvIndex].No);

        //                    Testms[0] = TestTimer.msDuriation;

        //                    //强制通过定位失败的芯片
        //                    if (INI.CHIP_forceALIGNRUN_pass)
        //                    {
        //                        WorkStatusCollectionClass RunStatusCollectionTemp = new WorkStatusCollectionClass();
        //                        RunStatusCollectionTemp.Clear();
        //                        foreach (WorkStatusClass workStatus in RunStatusCollection.WorkStatusList)
        //                        {
        //                            if (workStatus.AnalyzeProcedure != AnanlyzeProcedureEnum.ALIGNRUN)
        //                            {
        //                                WorkStatusClass workStatus1 = workStatus.Clone();
        //                                RunStatusCollectionTemp.Add(workStatus1);
        //                            }
        //                        }
        //                        RunStatusCollection.Clear();
        //                        foreach (WorkStatusClass workStatus in RunStatusCollectionTemp.WorkStatusList)
        //                        {
        //                            WorkStatusClass workStatus1 = workStatus.Clone();
        //                            RunStatusCollection.Add(workStatus1);
        //                        }
        //                    }

        //                    IsPass = RunStatusCollection.NGCOUNT == 0;

        //                    if (IsPass)
        //                    {
        //                        EnvIndex++;

        //                        if (EnvIndex < AlbumWork.ENVCount)
        //                        {
        //                            ResetData(EnvIndex);

        //                            Process.NextDuriation = 0;
        //                            Process.ID = 10;

        //                            return;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        OnEnvTrigger(ResultStatusEnum.SAVENGRAW, EnvIndex, "-1");
        //                    }

        //                    Process.NextDuriation = 0;
        //                    Process.ID = 40;

        //                    LogProcessIDTimer(Process.ID, "整合资料结果完成");
        //                    _LOG_MSG(Process.ID.ToString() + " MainSDM1Tick " + "整合资料结果完成");
        //                }
        //                break;
        //            case 40:

        //                Process.Stop();
        //                RXXLastProcess();

        //                _LOG_MSG(Process.ID.ToString() + " MainSDM1Tick " + "流程结束");
        //                LogProcessIDTimer(Process.ID, "流程结束");
        //                LogProcessIDTimer(Process.ID, "==================");
        //                LogProcessIDTimer(Process.ID, "==================");
        //                LogProcessIDTimer(Process.ID, "==================");
        //                GC.Collect();
        //                break;
        //        }
        //    }


        //}

        #endregion

        public WorkStatusCollectionClass RunStatusCollectionTemp = new WorkStatusCollectionClass();
        /// <summary>
        /// 收集结果数据集合
        /// </summary>
        List<DataMappingClass> RunDataMappingCollection = new List<DataMappingClass>();
        public int RunNextCCDDelayTime = 800;
        public bool RunStepComplete = false;
        public struct threadPara
        {
            public int ParaPageIndex;
            public int ParaEnvindex;
        }
        public class DrawResultClass
        {
            public RectangleF xRectF;
            public Color xColor;
            public Rectangle xGetRect()
            {
                Rectangle rectangle = new Rectangle();

                rectangle.X = (int)xRectF.X;
                rectangle.Y = (int)xRectF.Y;
                rectangle.Width = (int)xRectF.Width;
                rectangle.Height = (int)xRectF.Height;

                return rectangle;
            }
        }
        private void DLCalPageOneStepIndex(object obj)
        {
            threadPara para = (threadPara)obj;
            int pageindex = para.ParaPageIndex;
            int envindex = para.ParaEnvindex;
            //int pageindex = (int)obj;
            //int envindex = 0;// para.ParaEnvindex;
            if (envindex >= AlbumWork.ENVList.Count)
            {
                RunStepComplete = true;
                return;
            }
            //RunStepComplete = true;
            //return;
            RunStepComplete = false;

            System.Diagnostics.Stopwatch watchThreadTime = new System.Diagnostics.Stopwatch();
            watchThreadTime.Restart();

            PageClass page = m_EnvNow.PageList[pageindex];
            //m_EnvNow.ResetRunStatus();
            int pageindex2 = page.No;
            Universal.CalPageIndex = pageindex;

            AlbumWork.SetPageTestState(envindex, pageindex2, false);
            PageClass page1 = AlbumWork.ENVList[envindex].PageList[pageindex2];
            //AlbumWork.ResetRunStatus();
            //page1.ResetRunStatus();
            //PageClass page0 = AlbumWork.ENVList[envindex].PageList[pageindex2];
            //foreach (AnalyzeClass analyze in page0.AnalyzeRoot.BranchList)
            //{
            //    if (analyze.CheckAnalyzeReadBarcode())
            //    {
            //        analyze.ResetAnalyzeBarcodeStr();
            //        break;
            //    }
            //}

            Point ptRestore = new Point(m_AlignFristOffset.X, m_AlignFristOffset.Y);
            //偏移所有框的位置
            //AlbumWork.SetOffset(envindex, pageindex2, ptRestore, true);
            AlbumWork.A08_RunProcess(PageOPTypeEnum.P00, pageindex2, envindex);
            AlbumWork.SetPageTestState(envindex, pageindex2, true);

            RunStatusCollectionTemp.Clear();
            //PageClass page1 = AlbumWork.ENVList[envindex].PageList[pageindex2];
            //page1.ResetRunStatus();
            page1.FillRunStatus(RunStatusCollectionTemp);

            ////填入条码
            //if (string.IsNullOrEmpty(Universal.CalTestBarcode))
            //{

            //}

            foreach (WorkStatusClass workStatusClass in RunStatusCollectionTemp.WorkStatusList)
            {
                if (workStatusClass.Reason != ReasonEnum.PASS)
                    RunStatusCollection.Add(workStatusClass.Clone());
                //RunStatusCollection.Add(workStatusClass);
            }

            EnvAnalyzePostionSettings envAnalyzePostion = new EnvAnalyzePostionSettings(AlbumWork.ENVList[envindex].GeneralPosition);
            envAnalyzePostion.GetImageCountXoffset = 36.5f;
            envAnalyzePostion.GetImageCountYoffset = 36.5f;
            //int tmprow = 0;
            //int tmpcol = 0;
            int i = 0;

            int itemwidth = 230;
            int itemheight = 230;
            foreach (AnalyzeClass analyze in page1.AnalyzeRoot.BranchList)
            {
                if (itemwidth < analyze.bmpWIP.Width)
                {
                    itemwidth = analyze.bmpWIP.Width;
                }
                if (itemheight < analyze.bmpWIP.Height)
                {
                    itemheight = analyze.bmpWIP.Height;
                }
            }

            //如果页面里面

            Universal.SDM2_BMP_SHOW_CURRENT.Dispose();
            Universal.SDM2_BMP_SHOW_CURRENT = new Bitmap(itemwidth * page.m_Mapping_Col, itemheight * page.m_Mapping_Row);
            Graphics graphics = Graphics.FromImage(Universal.SDM2_BMP_SHOW_CURRENT);


            int drawrow = 0;
            int drawcol = 0;

            for (i = 0; i < page.AnalyzeRoot.BranchList.Count; i++)
            {
                DataMappingClass dataMapping = new DataMappingClass();

                if (i > 0 && i % page.m_Mapping_Col == 0)
                {
                    drawrow++;
                    drawcol = 0;
                }
                else
                {
                    if (i > 0)
                        drawcol++;
                }

                dataMapping.PtfCenter = new PointF(page.PageRunLocation.X + (float)(drawcol * envAnalyzePostion.GetImageCountXoffset * 1.0 / page.m_Mapping_Col),
                                                                            page.PageRunLocation.Y - (float)(drawrow * envAnalyzePostion.GetImageCountYoffset * 1.0 / page.m_Mapping_Row));


                #region

                //这里添加测试完成页面的结果
                foreach (AnalyzeClass analyze in page1.AnalyzeRoot.BranchList)
                {
                    string a1 = analyze.ToAnalyzeString();
                    string a2 = page.AnalyzeRoot.BranchList[i].ToAnalyzeString();

                    if (a1 == a2)
                    {
                        dataMapping.ReportBinValue = analyze.GetAnalyzeErrorType();

                        if (dataMapping.ReportBinValue == 4)
                        {
                            graphics.DrawImage(analyze.PADPara.bmpMeasureOutput,
                                                        new RectangleF(new PointF(0 + itemwidth * drawcol, 0 + itemheight * drawrow), new Size(itemwidth, itemheight)),
                                                        new RectangleF(new PointF(0, 0), analyze.PADPara.bmpMeasureOutput.Size),
                                                        GraphicsUnit.Pixel);
                            dataMapping.bmpResult.Dispose();
                            dataMapping.bmpResult = new Bitmap(analyze.PADPara.bmpMeasureOutput);
                        }
                        else
                        {
                            graphics.DrawImage(analyze.bmpWIP,
                                                          new RectangleF(new PointF(0 + itemwidth * drawcol, 0 + itemheight * drawrow), new Size(itemwidth, itemheight)),
                                                          new RectangleF(new PointF(0, 0), analyze.bmpWIP.Size),
                                                          GraphicsUnit.Pixel);
                            dataMapping.bmpResult.Dispose();
                            dataMapping.bmpResult = new Bitmap(analyze.bmpWIP);
                            Graphics graphicsx = Graphics.FromImage(dataMapping.bmpResult);
                            List<RectangleF> list = new List<RectangleF>();
                            List<RectangleF> list_ng = new List<RectangleF>();
                            //List<DrawResultClass> list_draw = new List<DrawResultClass>();
                            //收集内框并画出
                            foreach (AnalyzeClass analyze2 in analyze.BranchList)
                            {
                                RectangleF rx = new RectangleF();
                                rx.X = analyze2.myOPRectF.X - analyze.ALIGNPara.AlignOffset.X + analyze2.NORMALPara.ExtendX;// - analyze.myOPRectF.X;
                                rx.Y = analyze2.myOPRectF.Y - analyze.ALIGNPara.AlignOffset.Y + analyze2.NORMALPara.ExtendY;// - analyze.myOPRectF.Y;
                                rx.Width = analyze2.myOPRectF.Width - analyze2.NORMALPara.ExtendX * 2;
                                rx.Height = analyze2.myOPRectF.Height - analyze2.NORMALPara.ExtendY * 2;
                                if (analyze2.IsVeryGood)
                                    list.Add(rx);
                                else
                                    list_ng.Add(rx);
                            }
                            if (list.Count > 0)
                            {
                                graphics.DrawRectangles(new Pen(Color.Lime, 1), list.ToArray());
                                graphicsx.DrawRectangles(new Pen(Color.Lime, 1), list.ToArray());
                            }
                            if (list_ng.Count > 0)
                            {
                                graphics.DrawRectangles(new Pen(Color.Red, 3), list_ng.ToArray());
                                graphicsx.DrawRectangles(new Pen(Color.Red, 3), list_ng.ToArray());
                            }

                            graphicsx.Dispose();
                            graphics.Dispose();
                        }

                        break;
                    }
                }
                RunDataMappingCollection.Add(dataMapping);

                #endregion
            }

            //graphics.Dispose();
            //Universal.SDM2_BMP_SHOW_CURRENT.Save("D:\\TEST001.BMP");
            ////放出图像显示
            //OnTriggerShowImageCurrent(bmpshowCurrent);
            //OnTrigger(ResultStatusEnum.SHOW_CURRENT_IMAGE);

            watchThreadTime.Stop();
            long _time = watchThreadTime.ElapsedMilliseconds;
            if (_time > INI.MAINSD_GETIMAGE_DELAYTIME)
            {
                //if (INI.MAINSD_GETIMAGE_DELAYTIME < 70)
                //    INI.MAINSD_GETIMAGE_DELAYTIME = 70;

                RunNextCCDDelayTime = INI.MAINSD_GETIMAGE_DELAYTIME;
            }
            else
                RunNextCCDDelayTime = INI.MAINSD_GETIMAGE_DELAYTIME - (int)_time;

            _LOG_MSG("Step=" + pageindex.ToString() + " 用时=" + watchThreadTime.ElapsedMilliseconds + "ms");
            RunStepComplete = true;

            ////还原所有框的位置
            //ptRestore = new Point(-m_AlignFristOffset.X, -(m_AlignFristOffset.Y));
            //AlbumWork.SetOffset(envindex, pageindex2, ptRestore, false);
            OnTrigger(ResultStatusEnum.SHOW_CURRENT_IMAGE);
        }

        //private void DLCalPageIndex(object obj)
        //{
        //    int pageindex = (int)obj;
        //    AlbumWork.SetPageTestState(pageindex, false);
        //    AlbumWork.A08_RunProcess(PageOPTypeEnum.P00, pageindex);
        //    AlbumWork.SetPageTestState(pageindex, true);
        //}
        //private void DLCalPageIndexSDM2(object obj)
        //{
        //    int pageindex = (int)obj;
        //    m_EnvNow.PageList[pageindex].CalComplete = false;
        //    m_EnvNow.A08_RunProcess(PageOPTypeEnum.P00, pageindex);
        //    m_EnvNow.PageList[pageindex].CalComplete = true;
        //}
        //bool m_EnvTrainOK = false;
        //private void DLTrainSDM2(object obj)
        //{
        //    m_EnvTrainOK = false;
        //    m_EnvNow.ResetTrainStatus();
        //    m_EnvNow.A00_TrainProcess(true);
        //    m_EnvTrainOK = true;
        //}

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

                        //MACHINE.SetLight("1,1,1");

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
                            _LOG_MSG(Process.ID.ToString() + " OnekeyGetImageTick GoX_POS=" + ENVAnalyzePostion.GetImagePostions[GeneralPositionIndex].ToString());
                            //_LOG_MSG(Process.ID.ToString() + " OnekeyGetImageTick GoX_POS=" + GeneralPositions[GeneralPositionIndex]);

                            //MACHINE.GoPosition(ENVAnalyzePostion.GetImagePostions[GeneralPositionIndex].ToString());
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
                            //if (MACHINE.IsOnSite() || Universal.IsNoUseMotor)
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
                            //if (MACHINE.IsOnSite() || Universal.IsNoUseMotor)
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

                                //MACHINE.GoReadyPosition();

                                //if (!INI.IsLightAlwaysOn)
                                //    MACHINE.SetLight("");

                                //马达回待命位置
                                LogProcessIDTimer(Process.ID, "Onekey 取像完成 Motor Ready");
                                _LOG_MSG(Process.ID.ToString() + " OnekeyGetImageTick " + "Onekey 取像完成 Motor Ready");
                            }

                        }
                        break;
                    case 10120:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
                        if (Process.IsTimeup)
                        {
                            //if (MACHINE.IsOnSite() || Universal.IsNoUseMotor)
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

                        //MACHINE.XHome();
                        //MACHINE.YHome();

                        _LOG_MSG("ResetTick 复位中");

                        break;

                    case 10:
                        if (Process.IsTimeup)
                        {
                            //if (MACHINE.IsXHome() && MACHINE.IsYHome() || Universal.IsNoUseMotor)
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

        //public ProcessClass m_DLGetImageProcess = new ProcessClass();
        //void DLGetImageTick()
        //{
        //    ProcessClass Process = m_DLGetImageProcess;

        //    if (Process.IsOn)
        //    {
        //        switch (Process.ID)
        //        {
        //            case 5:

        //                Process.TimeUnit = TimeUnitEnum.ms;
        //                Process.NextDuriation = INI.MAINSD_GETIMAGE_DELAYTIME;
        //                Process.ID = 10;

        //                break;

        //            case 10:
        //                if (Process.IsTimeup)
        //                {
        //                    Process.Stop();
        //                    Universal.CCDCollection.GetImage();
        //                    Bitmap bitmap = Universal.CCDCollection.GetBMP(0, false);

        //                    if (CamActClass.Instance.StepCurrent >= CamActClass.Instance.StepCount)
        //                        CamActClass.Instance.ResetStepCurrent();
        //                    CamActClass.Instance.SetImage(bitmap, CamActClass.Instance.StepCurrent);
        //                    CamActClass.Instance.StepCurrent++;

        //                    m_DLGetImageOK.Start();
        //                }
        //                break;
        //        }
        //    }
        //}

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
                        //MACHINE.PLCIO.GetImageOK = true;

                        break;

                    case 10:
                        if (Process.IsTimeup)
                        {
                            Process.Stop();
                            //MACHINE.PLCIO.GetImageOK = false;

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

                        //MACHINE.PLCIO.Busy = false;
                        if (MACHINE.PLCIO.Ready)
                        {
                            if (INI.CHIP_force_pass)
                            {
                                MACHINE.PLCIO.Pass = true;
                                MACHINE.PLCIO.Fail = false;
                            }
                            else
                            {
                                MACHINE.PLCIO.Pass = IsPass;
                                MACHINE.PLCIO.Fail = !IsPass;
                            }
                        }

                        break;

                    case 10:
                        if (Process.IsTimeup)
                        {
                            Process.Stop();
                            //MACHINE.PLCIO.Pass = false;
                            //MACHINE.PLCIO.Fail = false;
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
            switch (Universal.CAMACT)
            {
                case CameraActionMode.CAM_MOTOR_MODE2:
                    break;
            }

            if (IsPass)
            {
                OnTrigger(ResultStatusEnum.CALPASS);
            }
            else
            {
                OnTrigger(ResultStatusEnum.CALNG);

                //JzMainSDPositionParas.INSPECT_NGINDEX++;
                //JzMainSDPositionParas.SaveRecord();
            }
            //if (INI.ISQSMCALLSAVE)
            //    _saveAllResultPictures();

            if (INI.IsCollectPictures)
                MainX6Save();

            OnTrigger(ResultStatusEnum.COUNTEND);
            OnTrigger(ResultStatusEnum.CALEND);

            //if (!IsPass)
            //{
            //    JzMainSDPositionParas.ReportAUTOSave(JzMainSDPositionParas.INSPECT_NGINDEX, false, true);
            //    if (INI.IsCollectStripPictures)
            //        MainX6StripImageDataSave();//保存strip图片
            //}
            //JzMainSDPositionParas.ReportGradeSave(JzMainSDPositionParas.INSPECT_NGINDEX, false);

            //if(!INI.IsOpenBehindOKSign)
            //{
            //    m_DLResultOK.Start();
            //    LogProcessIDTimer(8888, $"发送结果信号PC==>PLC {(IsPass ? "PASS" : "NG")}");
            //}

            LogProcessIDTimer(8889, $"测试结束");
            Universal.IsRunningTest = false;

        }
        bool IsInRange(int FromValue, int CompValue, int DiffValue)
        {
            return (FromValue >= CompValue - DiffValue) && (FromValue <= CompValue + DiffValue);
        }
        Color _getColor(int eIndex)
        {
            return MappingClass.GetBinColor(eIndex);
            //Color eColor = Color.Green;
            //switch (eIndex)
            //{
            //    case 1:
            //        eColor = Color.Cyan;
            //        break;
            //    case 2:
            //        eColor = Color.Violet;
            //        break;
            //    case 3:
            //        eColor = Color.Yellow;
            //        break;
            //    case 4:
            //        eColor = Color.Red;
            //        break;
            //    case 5:
            //        eColor = Color.Purple;
            //        break;
            //    case 6:
            //        eColor = Color.Blue;
            //        break;
            //    case 7:
            //        eColor = Color.Orange;
            //        break;
            //    case 8:
            //        eColor = Color.Fuchsia;
            //        break;
            //    case 9:
            //        eColor = Color.LightPink;
            //        break;
            //    default:
            //        eColor = Color.Green;
            //        break;
            //}
            //return eColor;
        }

        List<string> AssignRankList = new List<string>();
        List<AnalyzeClass> analyzeClasses = new List<AnalyzeClass>();

        public int Smoothen()
        {
            AssignRankList.Clear();
            analyzeClasses.Clear();
            int envindex = 0;
            int pageindex = 0;
            int analyzeindex = 0;
            //foreach (EnvClass env in AlbumWork.ENVList)
            {
                pageindex = 0;
                foreach (PageClass page in m_EnvNow.PageList)
                {
                    analyzeindex = 0;
                    foreach (AnalyzeClass analyze in page.AnalyzeRoot.BranchList)
                    {
                        if (analyze.PADPara.PADMethod == PADMethodEnum.GLUECHECK)
                        {
                            analyzeClasses.Add(analyze);
                            AssignRankList.Add(envindex.ToString() + "," + pageindex.ToString() + "," + analyzeindex.ToString());
                        }
                        analyzeindex++;
                    }
                    pageindex++;
                }
                //envindex++;
            }

            if (!INI.CHIP_ISSMOOTHEN)
                return -1;

            int i = 0;
            int j = 0;
            int RestoreSeq = -1;

            //VICTOR MADE
            float SimilarCheck = 0;
            float SimiliarCount = 0;
            float SimiliarStep = 0;
            bool IsRestored = false;

            if (analyzeClasses.Count <= 0)
                return -1;

            i = Math.Min(100, analyzeClasses[0].BackupCount);

            if (i > 0)
            {
                j = 0;
                while (j < i)
                {
                    //Check Similiar First

                    SimilarCheck = 0;
                    SimiliarCount = 0;
                    SimiliarStep = 0;

                    foreach (string str in AssignRankList)
                    {
                        int envseq = int.Parse(str.Split(',')[0]);
                        int pageseq = int.Parse(str.Split(',')[1]);
                        int analyzeseq = int.Parse(str.Split(',')[2]);
                        AnalyzeClass assign = m_EnvNow.PageList[pageseq].AnalyzeRoot.BranchList[analyzeseq];

                        SimilarCheck += assign.CheckSimilar(j, ref SimiliarStep);
                        SimiliarCount += SimiliarStep;
                        //SETUPList[assign.SetupIndex].SIDEList[assign.SideIndex].DecorateBMP(bmpwork[assign.SideIndex]);
                    }

                    float SimilarRatio = SimilarCheck / SimiliarCount;
                    //Modified By Victor In 2015/08/24 ORG is 0.7
                    if (SimilarRatio > 0.88)
                    {
                        foreach (string str in AssignRankList)
                        {
                            int envseq = int.Parse(str.Split(',')[0]);
                            int pageseq = int.Parse(str.Split(',')[1]);
                            int analyzeseq = int.Parse(str.Split(',')[2]);
                            AnalyzeClass assign = m_EnvNow.PageList[pageseq].AnalyzeRoot.BranchList[analyzeseq];
                            assign.RestoreData(j);
                            RestoreSeq = j;
                        }

                        IsRestored = true;

                        break;
                    }

                    j++;
                }
            }

            if (!IsRestored)
            {
                foreach (string str in AssignRankList)
                {
                    int envseq = int.Parse(str.Split(',')[0]);
                    int pageseq = int.Parse(str.Split(',')[1]);
                    int analyzeseq = int.Parse(str.Split(',')[2]);
                    AnalyzeClass assign = m_EnvNow.PageList[pageseq].AnalyzeRoot.BranchList[analyzeseq];

                    assign.BackupData();
                }
            }

            return RestoreSeq;
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
            Task task = new Task(() =>
            {
                try
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

                    m.Save(screen_SavePath + "\\" + (IsPass ? "P-" : "F-") + "Screen_" + Universal.CalTestBarcode + ".jpg", ImageFormat.Jpeg);
                    m.Dispose();
                }
                catch (Exception ex)
                {
                    JetEazy.LoggerClass.Instance.WriteException(ex);
                }
            });
            task.Start();

            //string screen_SavePath = "D:\\CollectPictures_" + Universal.OPTION.ToString() + "\\Screen\\" + JzTimes.DateSerialString;

            //if (!Directory.Exists(screen_SavePath))
            //    Directory.CreateDirectory(screen_SavePath);

            //int width = Screen.PrimaryScreen.Bounds.Width;
            //int height = Screen.PrimaryScreen.Bounds.Height;

            //Bitmap m = new Bitmap(width, height);
            //using (Graphics g = Graphics.FromImage(m))
            //{
            //    g.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size);
            //    g.Dispose();
            //}

            //m.Save(screen_SavePath + "\\" + (IsPass ? "P-" : "F-") + "Screen_" + Universal.CalTestBarcode + ".jpg", ImageFormat.Jpeg);
            //m.Dispose();
        }
        private void MainX6Save()
        {
            if (INI.CHIP_NG_collect)
            {
                if (IsPass)
                    return;
            }
            Task task = new Task(() =>
            {
                try
                {
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
                catch (Exception ex)
                {
                    JetEazy.LoggerClass.Instance.WriteException(ex);
                }
            });
            task.Start();

            //string mainx6_path = Universal.MainX6_Path + "\\" + (IsPass ? "P-" : "F-") + JzTimes.DateTimeSerialString;

            //if (!Directory.Exists(mainx6_path + "\\000"))
            //    Directory.CreateDirectory(mainx6_path + "\\000");

            //EnvClass env = AlbumWork.ENVList[0];

            //int qi = 0;
            //foreach (PageClass page in env.PageList)
            //{
            //    page.GetbmpRUN().Save(mainx6_path + "\\000\\P00-" + qi.ToString("000") + ".png", ImageFormat.Png);
            //    qi++;
            //}
        }
        private void SMD2Save()
        {
            if (INI.CHIP_NG_collect)
            {
                if (IsPass)
                    return;
            }

            Task task = new Task(() =>
            {
                try
                {
                    string mainx6_path = Universal.MainX6_Path + "\\" + (IsPass ? "P-" : "F-") + Universal.CalTestBarcode;

                    if (!Directory.Exists(mainx6_path + "\\000"))
                        Directory.CreateDirectory(mainx6_path + "\\000");

                    EnvClass env = m_EnvNow;

                    int qi = 0;
                    foreach (PageClass page in env.PageList)
                    {
                        FreeImageAPI.FreeImageBitmap freeImage = new FreeImageAPI.FreeImageBitmap(page.GetbmpRUN());
                        freeImage.Save(mainx6_path + "\\000\\P00-" + qi.ToString("000") + ".jpg", FreeImageAPI.FREE_IMAGE_FORMAT.FIF_JPEG);
                        freeImage.Dispose();
                        //page.GetbmpRUN().Save(mainx6_path + "\\000\\P00-" + qi.ToString("000") + ".jpg", ImageFormat.Jpeg);
                        qi++;
                    }
                }
                catch (Exception ex)
                {
                    JetEazy.LoggerClass.Instance.WriteException(ex);
                }
            });
            task.Start();

            //string mainx6_path = Universal.MainX6_Path + "\\" + (IsPass ? "P-" : "F-") + Universal.CalTestBarcode;

            //if (!Directory.Exists(mainx6_path + "\\000"))
            //    Directory.CreateDirectory(mainx6_path + "\\000");

            //EnvClass env = m_EnvNow;

            //int qi = 0;
            //foreach (PageClass page in env.PageList)
            //{
            //    page.GetbmpRUN().Save(mainx6_path + "\\000\\P00-" + qi.ToString("000") + ".jpg", ImageFormat.Jpeg);
            //    qi++;
            //}
        }
        private void _LOG_MSG(string eMsg)
        {
            CommonLogClass.Instance.Log(eMsg);
            //CommonLogClass.Instance.LogMessage(eMsg);
        }
        private void _LOG_MSG_ERR(string eMsg)
        {
            CommonLogClass.Instance.Log(eMsg);
            //CommonLogClass.Instance.LogError(eMsg);
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
