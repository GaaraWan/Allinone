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
using JetEazy.FormSpace;
using JetEazy.ControlSpace;
using Allinone.ControlSpace;
using Allinone.ControlSpace.MachineSpace;
using JzMSR.OPSpace;

using AllinOne.Jumbo.Net;
using AllinOne.Jumbo.Net.Common;
using Newtonsoft.Json;
using System.Collections;
using System.Media;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using JetEazy.PlugSpace;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using JetEazy.Interface;
using FreeImageAPI;

namespace Allinone.OPSpace.ResultSpace
{
    public class JzMainX6ResultClass : GeoResultClass
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr _lopen(string lpPathName, int iReadWrite);
        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);
        public const int OF_READWRITE = 2;
        public const int OF_SHARE_DENY_NONE = 0x40;
        public readonly IntPtr HFILE_ERROR = new IntPtr(-1);

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

        JzMainX6MachineClass MACHINE;
        MessageForm M_WARNING_FRM;

        JzMainSDPositionParaClass JzMainSDPositionParas
        {
            get { return Universal.JZMAINSDPOSITIONPARA; }
        }
        ClientSocket X6_HANDLE_CLIENT
        {
            get { return Universal.X6_HANDLE_CLIENT; }
        }
        IxLineScanCam m_IxLinescanCamera
        {
            get
            {
                return Universal.IxLineScan;
            }
        }

        JzToolsClass JzTools = new JzToolsClass();
        SoundPlayer PlayerPass = new SoundPlayer();
        SoundPlayer PlayerFail = new SoundPlayer();
        FreeImageBitmap bmpLargeTemp = new FreeImageBitmap(1, 1);

        bool isGetImageReset = false;

        public JzMainX6ResultClass(Result_EA resultea, VersionEnum version, OptionEnum option, MachineCollectionClass machinecollection)
        {
            myResultEA = resultea;
            VERSION = version;
            OPTION = option;

            PlayerPass.SoundLocation = Universal.PlayerPASSPATH;
            PlayerFail.SoundLocation = Universal.PlayerFAILPATH;
            PlayerPass.Load();
            PlayerFail.Load();

            DUP = new DupClass();

            MACHINE = (JzMainX6MachineClass)machinecollection.MACHINE;

            MainProcess = new ProcessClass();
            if (Universal.IsUseThreadReviceTcp)
            {
                Task.Run(() =>
                {
                    _TcpRun();
                });
            }
        }

        private void CCDCollection_TriggerAction(string operationstr)
        {
            throw new NotImplementedException();
        }
        //public override void AddLinescanCamera(IxLineScanCam elinescancam)
        //{
        //    base.AddLinescanCamera(elinescancam);
        //    m_IxLinescanCamera = elinescancam;
        //}
        public override void GetStart(AlbumClass albumwork, CCDCollectionClass ccdcollection, TestMethodEnum testmethod, bool isnouseccd)
        {

            switch (Universal.CAMACT)
            {
                case CameraActionMode.CAM_MOTOR_LINESCAN:
                case CameraActionMode.CAM_MOTOR_MODE2:

                    if (MainProcess.IsOn) //如果正在流程中 则跳出
                        return;

                    break;
                default:

                    if (MainProcess.IsOn)
                    {
                        MainProcess.Stop();
                        OnTrigger(ResultStatusEnum.FORECEEND);
                        return;
                    }

                    break;
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

            switch (TestMethod)
            {
                case TestMethodEnum.QSMCSF:
                    //if (INI.ISHIVECLIENT)
                    //{
                    //    MACHINE.SetMachineState(MachineState.Running);
                    //    if (MACHINE.GetCurrentMachineState == MachineState.Running)
                    //        MACHINE.HIVECLIENT.Hiveclient_ConfigurationMap(BARCODE, "SF", INI.DATA_Program, INI.DATA_Building_Config, _get_qsmc_sndata_json());
                    //}
                    break;
            }

        }
        public override void Tick()
        {
            //if(!IsNoUseCCD)
            //{
            //    if (MainProcess.IsOn)
            //    {
            //        MainProcess.Stop();
            //        IsStopNormalTick = false;
            //        MessageBox.Show("UPS Error 请检查UPS 或相关接线！");
            //    }
            //}
            //FOXCONNTick();
            MainProcessTick();
            if (!Universal.IsUseThreadReviceTcp)
                _TcpAndIoTick();

            //switch(OPTION)
            //{
            //    case OptionEnum.R32:
            //        QSMCSFR32Tick();
            //        break;
            //    case OptionEnum.R26:
            //        QSMCSFR26Tick();
            //        break;
            //    case OptionEnum.R15:
            //      //  QSMCSFR15Tick();
            //        break;
            //}
        }
        public override void GenReport()
        {

        }
        public override void SetDelayTime()
        {
            DelayTime[0] = INI.DELAYTIME;
        }
        public void _TcpAndIoTick()
        {
            DLResultOKTick();
            switch (Universal.CAMACT)
            {
                case CameraActionMode.CAM_MOTOR_LINESCAN:
                case CameraActionMode.CAM_MOTOR_MODE2:
                case CameraActionMode.CAM_MOTOR:

                    DLGetImageTick();
                    DLGetImageOKTick();
                    //DLResultOKTick();

                    //Trigger Start
                    bool isGetImageStart = MACHINE.PLCIO.IsStart;

                    if (isGetImageStart && IsGetImageStartOld != isGetImageStart)
                    {
                        m_DLGetImageProcess.Start();
                    }
                    IsGetImageStartOld = isGetImageStart;


                    //Trigger Reset
                    isGetImageReset = MACHINE.PLCIO.IsGetImageReset;

                    if (isGetImageReset && IsGetImageResetOld != isGetImageReset)
                    {
                        JetEazy.PlugSpace.CamActClass.Instance.ResetStepCurrent();
                        //if (!MainProcess.IsOn)
                        MainProcess.Stop();
                        MACHINE.PLCIO.Busy = false;
                        if (m_DLGetImageOK.IsOn)
                            m_DLGetImageOK.Stop();
                        if (m_DLResultOK.IsOn)
                            m_DLResultOK.Stop();

                        Universal.IsRunningTest = false;

                    }
                    IsGetImageResetOld = isGetImageReset;

                    break;
            }
        }
        public void _TcpRun()
        {
            while (true)
            {
                _TcpAndIoTick();
                Thread.Sleep(2);
            }
        }
        //public override void thread_func(object arg)
        //{
        //    while (is_thread_running())
        //    {
        //        try
        //        {
        //            _TcpAndIoTick();
        //        }
        //        catch (Exception ex)
        //        {
        //            LogProcessIDTimer(9999, $"TcpIo刷新异常{ex.Message}");
        //            break;
        //        }

        //        Thread.Sleep(3);
        //    }
        //    base.thread_func(arg);
        //}

        JzTimes TestTimer = new JzTimes();
        int[] Testms = new int[100];
        DateTime m_input_time = DateTime.Now;
        /// <summary>
        /// 判断线程是否启动完成
        /// </summary>
        bool m_IsTaskRun = false;

        protected override void MainProcessTick()
        {
            switch (OPTION)
            {
                //case OptionEnum.R32:
                //    R32Tick();
                //    break;
                //case OptionEnum.R26:
                //    R26Tick();
                //    break;
                case OptionEnum.MAIN_X6:
                    MainX6Tick();
                    break;
            }


        }
        public override void FillProcessImage()
        {
            EnvClass env = AlbumWork.ENVList[EnvIndex];
            if (IsNoUseCCD)
            {
                OnTriggerOP(ResultStatusEnum.SET_CURRENT_IMAGE, "DEBUG#" + (env.PageList.Count - 1).ToString());
                int i = 0;
                foreach (PageClass page in env.PageList)
                {
                    page.SetbmpRUN(PageOPTypeEnum.P00, CCDCollection.GetBMP(i, false));
                    i++;
                }
            }
            else
            {
                foreach (PageClass page in env.PageList)
                {
                    page.SetbmpRUN(PageOPTypeEnum.P00, CCDCollection.GetBMP(page.CamIndex, false));
                }
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
        public void FillProcessImageMotorPageIndex(int index, Bitmap bmpinut)
        {
            EnvClass env = AlbumWork.ENVList[EnvIndex];

            if (index >= env.PageList.Count || index < 0)
                return;

            PageClass page = env.PageList[index];
            bmpLargeTemp.Dispose();
            bmpLargeTemp = new FreeImageBitmap(bmpinut);
            page.SetbmpRUN(PageOPTypeEnum.P00, bmpLargeTemp.ToBitmap());
        }
        public void FillProcessImageMotorPageIndexDebug(int index)
        {
            EnvClass env = AlbumWork.ENVList[EnvIndex];

            if (index >= env.PageList.Count || index < 0)
                return;

            PageClass page = env.PageList[index];
            page.SetbmpRUN(PageOPTypeEnum.P00, CCDCollection.GetBMP(index, false));
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

        void QSMCSFR32Tick()
        {
            if (IsStopNormalTick)
                return;

            bool isgotbarcode = false;
            //if (INI.ISQSMCSF)
            {
                string[] Files;

                //bool IsFound = false;

                Files = Directory.GetFiles(INI.SHOPFLOORPATH);

                if (Files.Length > 0)
                {
                    string filestr = "";

                    foreach (string strFile in Files)
                    {
                        if (strFile.ToUpper().IndexOf("\\SN.TXT") > -1)
                        {
                            filestr = strFile;

                            isgotbarcode = TestAndReadData(ref ORGBARCODESTR, strFile);

                            if (isgotbarcode)
                            {
                                ORGBARCODESTR = ORGBARCODESTR.Trim();
                                break;
                            }
                        }
                    }

                    if (isgotbarcode)
                    {
                        try
                        {
                            Universal.DATASNTXT = ORGBARCODESTR;
                            ORGBARCODESTR += ",,,,,,";

                            VER = ORGBARCODESTR.Split(',')[1].Trim();
                            BARCODE = ORGBARCODESTR.Split(',')[0].Trim();
                            JzToolsClass.PassingBarcode = BARCODE;
                            ARTWORKNAME = ORGBARCODESTR.Split(',')[2].Trim();

                            if (ORGBARCODESTR.Split(',').Length > 3)
                                MODELNAME = ORGBARCODESTR.Split(',')[3].Trim();

                            switch (INI.SFFACTORY)
                            {
                                case FactoryShopfloor.FOXCONN:
                                    RELATECOLORSTR = ORGBARCODESTR.Split(',')[6];
                                    HOUSINGID = ORGBARCODESTR.Split(',')[7];
                                    break;
                                default:
                                    if (INI.ISSFCOLOR)
                                    {
                                        switch (ORGBARCODESTR.Split(',')[6].Trim())
                                        {
                                            case "B":
                                                RELATECOLORSTR = "SILVER";
                                                break;
                                            case "A":
                                                RELATECOLORSTR = "GREY";
                                                break;
                                            case "C":
                                                RELATECOLORSTR = "GOLD";
                                                break;
                                        }
                                    }
                                    else
                                        RELATECOLORSTR = Universal.COLORTABLE.Check(ORGBARCODESTR.Split(',')[0].Substring(ORGBARCODESTR.Split(',')[0].Trim().Length - 4));

                                    if (RELATECOLORSTR == "NULL")
                                    {
                                        if (filestr != "")
                                            File.Delete(filestr);

                                        System.Windows.Forms.MessageBox.Show("无此:' " + BARCODE.Substring(BARCODE.Length - 4, 4) + " ' EEEE CODE,请添加!", "SYS",
                                            System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                                        string strsndata = BARCODE + ",OCR," + JzTimes.DateTimeSerialString + "," + "F";
                                        string filename = INI.SHOPFLOORPATH + "\\" + BARCODE + "OCR.txt";
                                        JzTools.SaveData(strsndata, filename);

                                        return;
                                    }

                                    bool ischeckok = false;
                                    foreach (JetEazy.DBSpace.RcpClass rcp in Universal.RCPDB.myDataList)
                                    {
                                        if (rcp.Version == VER)
                                        {
                                            ischeckok = true;
                                            break;
                                        }
                                    }
                                    if (!ischeckok)
                                    {
                                        if (filestr != "")
                                            File.Delete(filestr);

                                        System.Windows.Forms.MessageBox.Show("无此:' " + VER + " ' 国别,请联系厂商注册!", "SYS",
                                            System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                                        string strsndata = BARCODE + ",OCR," + JzTimes.DateTimeSerialString + "," + "F";
                                        string filename = INI.SHOPFLOORPATH + "\\" + BARCODE + "OCR.txt";
                                        JzTools.SaveData(strsndata, filename);
                                        return;
                                    }

                                    break;
                            }


                            #region Old Code
                            //QSMCSQLSF = new QSMCSQLSFClass(ORGBARCODESTR);
                            //QSMCSQLSF = new QSMCSQLSFClass();

                            //QSMCSQLSF.SetData(SFSQLEnum.UNITSN, BARCODE);
                            //QSMCSQLSF.SetData(SFSQLEnum.MODELNO, MODELNAME);
                            //QSMCSQLSF.SetData(SFSQLEnum.COUNTRYCODE, ORGBARCODESTR.Split(',')[4]);
                            //QSMCSQLSF.SetData(SFSQLEnum.ASSETTAG, ORGBARCODESTR.Split(',')[5]);
                            //REGIONClass.myQSMCSF = QSMCSQLSF.Clone();

                            /*
                            switch (Universal.VER)
                            {
                                default:
                                    switch (Universal.OPTION)
                                    {
                                        default:
                                            VER = ORGBARCODESTR.Split(',')[1].Trim();
                                            BARCODE = ORGBARCODESTR.Split(',')[0].Trim();

                                            if (INI.ISCTOMODE)
                                            {
                                                switch (Universal.VER)
                                                {
                                                    case "R33":
                                                        ORGBARCODESTR += ",,,,,,,,,,,,,,,";

                                                        QSMCKBSQLSF = new QSMCKBOCRSFClass(ORGBARCODESTR);

                                                        VERSION = QSMCKBSQLSF.GetData(KBOCRSFEnum.KBCOUNTRYCODE) + ":" + QSMCKBSQLSF.GetData(KBOCRSFEnum.KBVISABLE);
                                                        BARCODE = ORGBARCODESTR.Split(',')[12].Trim();

                                                        REGIONClass.myQSMCKBSF = QSMCKBSQLSF.Clone();

                                                        break;
                                                    case "R17":
                                                        ARTWORKNAME = ORGBARCODESTR.Split(',')[2].Trim();

                                                        if (ORGBARCODESTR.Split(',').Length > 3)
                                                            MODELNAME = ORGBARCODESTR.Split(',')[3].Trim();

                                                        break;
                                                    case "R27":
                                                        ARTWORKNAME = ORGBARCODESTR.Split(',')[2].Trim();

                                                        if (ORGBARCODESTR.Split(',').Length > 3)
                                                            MODELNAME = ORGBARCODESTR.Split(',')[3].Trim();

                                                        break;

                                                    case "R26":
                                                    case "R32":

                                                        ORGBARCODESTR += ",,,,,,";

                                                        VERSION = ORGBARCODESTR.Split(',')[1].Trim();
                                                        BARCODE = ORGBARCODESTR.Split(',')[0].Trim();

                                                        ARTWORKNAME = ORGBARCODESTR.Split(',')[2].Trim();

                                                        if (ORGBARCODESTR.Split(',').Length > 3)
                                                            MODELNAME = ORGBARCODESTR.Split(',')[3].Trim();

                                                        RELATECOLORSTR = Universal.COLORTABLE.Check(ORGBARCODESTR.Split(',')[0].Substring(ORGBARCODESTR.Split(',')[0].Trim().Length - 4));

                                                        //QSMCSQLSF = new QSMCSQLSFClass(ORGBARCODESTR);
                                                        QSMCSQLSF = new QSMCSQLSFClass();

                                                        QSMCSQLSF.SetData(SFSQLEnum.UNITSN, BARCODE);
                                                        QSMCSQLSF.SetData(SFSQLEnum.MODELNO, MODELNAME);
                                                        QSMCSQLSF.SetData(SFSQLEnum.COUNTRYCODE, ORGBARCODESTR.Split(',')[4]);
                                                        QSMCSQLSF.SetData(SFSQLEnum.ASSETTAG, ORGBARCODESTR.Split(',')[5]);

                                                        REGIONClass.myQSMCSF = QSMCSQLSF.Clone();

                                                        switch (Universal.OPTION)
                                                        {
                                                            case "AUTOBARCODE":
                                                                IsBYDSecondRun = true;
                                                                break;
                                                            case "FOXCONN":
                                                                RELATECOLORSTR = ORGBARCODESTR.Split(',')[6];
                                                                HOUSINGID = ORGBARCODESTR.Split(',')[7];
                                                                break;
                                                            default:
                                                                RELATECOLORSTR = Universal.COLORTABLE.Check(ORGBARCODESTR.Split(',')[0].Substring(ORGBARCODESTR.Split(',')[0].Trim().Length - 4));
                                                                break;
                                                        }

                                                        break;
                                                        //case "R32":
                                                        //    ARTWORKNAME = BARCODE.Split(',')[2].Trim();

                                                        //    if (BARCODE.Split(',').Length > 3)
                                                        //        MODELNAME = BARCODE.Split(',')[3].Trim();

                                                        //    RELATECOLORSTR = Universal.COLORTABLE.Check(BARCODE.Split(',')[0].Substring(BARCODE.Split(',')[0].Trim().Length - 4));

                                                        //    break;
                                                }
                                            }
                                            //VERSION = ORGBARCODESTR.Split(',')[1].Trim();
                                            //BARCODE = ORGBARCODESTR.Split(',')[0].Trim();

                                            break;
                                    }
                                    break;
                            }

                            */
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            JetEazy.LoggerClass.Instance.WriteException(ex);
                            foreach (string filestrrr in Files)
                            {
                                File.Delete(filestrrr);
                            }

                            if (INI.ISFOXCONNSF)
                            {
                                Universal.Memory.Write("E,ErrorSN");
                                //IsSFActive = false;

                                //LogActionClass.LogAction("QSMCTick(),Memory.Write(E-ErrorSN)" + ",IsSFActive = false");
                            }

                            //LogActionClass.LogAction("QSMCTick(),###Exception=" + "SN.txt格式錯誤:" + "/t" + ee.ToString());
                            //MessageBox.Show("SN.txt格式錯誤:" + "/t" + ee.ToString());

                            return;
                        }

                        if (filestr != "")
                            File.Delete(filestr);

                        IsPass = false;

                        if (INI.ISCHECKQSMCDUP)
                        {
                            if (DUP.CheckIsOK(BARCODE))
                            {
                                OnEnvTrigger(ResultStatusEnum.SNSTART, -1, SNSTARTOPSTR);

                                //bool_SNRun = true;
                                //IsSFActive = true;
                                //OnTrigger(StatusEnum.QSMCSF);
                            }
                            else
                            {
                                MessageForm DUPFRM = new MessageForm("DUPLICATE ID ERROR.", 5);
                                DUPFRM.Show();
                            }
                        }
                        else
                        {
                            //  OnEnvTrigger(ResultStatusEnum.SNSTART, -1, VER);
                            OnEnvTrigger(ResultStatusEnum.SNSTART, -1, SNSTARTOPSTR);
                            //OnTrigger(StatusEnum.COUNTSTART);

                            //bool_SNRun = true;
                            //IsSFActive = true;

                            //OnTrigger(StatusEnum.QSMCSF);
                        }

                        //IsFound = true;

                        //break;
                    }

                    //else
                    //{
                    //    //IsFound = false;
                    //    //  break;
                    //}
                }
                //if (IsFound)
                //{
                //    LogActionClass.LogAction("QSMCTick(),Find#SN.txt");

                //    //if (Universal.OPTION != "CLIENT")
                //    //{
                //    //    foreach (string filestr in Files)
                //    //    {
                //    //        File.Delete(filestr);
                //    //    }
                //    //}
                //}

            }

        }

        void QSMCSFR26Tick()
        {
            if (IsStopNormalTick)
                return;

            bool isgotbarcode = false;
            //if (INI.ISQSMCSF)
            {
                string[] Files;

                //bool IsFound = false;

                Files = Directory.GetFiles(INI.SHOPFLOORPATH);

                if (Files.Length > 0)
                {
                    string filestr = "";

                    foreach (string strFile in Files)
                    {
                        if (strFile.ToUpper().IndexOf("\\SN.TXT") > -1)
                        {
                            filestr = strFile;

                            isgotbarcode = TestAndReadData(ref ORGBARCODESTR, strFile);

                            if (isgotbarcode)
                            {
                                ORGBARCODESTR = ORGBARCODESTR.Trim();
                                break;
                            }
                        }
                    }

                    if (isgotbarcode)
                    {
                        try
                        {
                            Universal.DATASNTXT = ORGBARCODESTR;
                            ORGBARCODESTR += ",,,,,,";

                            VER = ORGBARCODESTR.Split(',')[1].Trim();
                            BARCODE = ORGBARCODESTR.Split(',')[0].Trim();
                            JzToolsClass.PassingBarcode = BARCODE;
                            ARTWORKNAME = ORGBARCODESTR.Split(',')[2].Trim();

                            if (ORGBARCODESTR.Split(',').Length > 3)
                                MODELNAME = ORGBARCODESTR.Split(',')[3].Trim();

                            switch (INI.SFFACTORY)
                            {
                                case FactoryShopfloor.FOXCONN:
                                    RELATECOLORSTR = ORGBARCODESTR.Split(',')[6];
                                    HOUSINGID = ORGBARCODESTR.Split(',')[7];
                                    break;
                                default:
                                    if (INI.ISSFCOLOR)
                                    {
                                        switch (ORGBARCODESTR.Split(',')[6].Trim())
                                        {
                                            case "B":
                                                RELATECOLORSTR = "SILVER";
                                                break;
                                            case "A":
                                                RELATECOLORSTR = "GREY";
                                                break;
                                            case "C":
                                                RELATECOLORSTR = "GOLD";
                                                break;
                                        }
                                    }
                                    else
                                        RELATECOLORSTR = Universal.COLORTABLE.Check(ORGBARCODESTR.Split(',')[0].Substring(ORGBARCODESTR.Split(',')[0].Trim().Length - 4));

                                    if (RELATECOLORSTR == "NULL")
                                    {
                                        if (filestr != "")
                                            File.Delete(filestr);

                                        System.Windows.Forms.MessageBox.Show("无此:' " + BARCODE.Substring(BARCODE.Length - 4, 4) + " ' EEEE CODE,请添加!", "SYS",
                                            System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                                        string strsndata = BARCODE + ",OCR," + JzTimes.DateTimeSerialString + "," + "F";
                                        string filename = INI.SHOPFLOORPATH + "\\" + BARCODE + "OCR.txt";
                                        JzTools.SaveData(strsndata, filename);

                                        return;
                                    }

                                    bool ischeckok = false;
                                    foreach (JetEazy.DBSpace.RcpClass rcp in Universal.RCPDB.myDataList)
                                    {
                                        if (rcp.Version == VER)
                                        {
                                            ischeckok = true;
                                            break;
                                        }
                                    }
                                    if (!ischeckok)
                                    {
                                        if (filestr != "")
                                            File.Delete(filestr);

                                        System.Windows.Forms.MessageBox.Show("无此:' " + VER + " ' 国别,请联系厂商注册!", "SYS",
                                            System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                                        string strsndata = BARCODE + ",OCR," + JzTimes.DateTimeSerialString + "," + "F";
                                        string filename = INI.SHOPFLOORPATH + "\\" + BARCODE + "OCR.txt";
                                        JzTools.SaveData(strsndata, filename);
                                        return;
                                    }

                                    break;
                            }


                            #region Old Code
                            //QSMCSQLSF = new QSMCSQLSFClass(ORGBARCODESTR);
                            //QSMCSQLSF = new QSMCSQLSFClass();

                            //QSMCSQLSF.SetData(SFSQLEnum.UNITSN, BARCODE);
                            //QSMCSQLSF.SetData(SFSQLEnum.MODELNO, MODELNAME);
                            //QSMCSQLSF.SetData(SFSQLEnum.COUNTRYCODE, ORGBARCODESTR.Split(',')[4]);
                            //QSMCSQLSF.SetData(SFSQLEnum.ASSETTAG, ORGBARCODESTR.Split(',')[5]);
                            //REGIONClass.myQSMCSF = QSMCSQLSF.Clone();

                            /*
                            switch (Universal.VER)
                            {
                                default:
                                    switch (Universal.OPTION)
                                    {
                                        default:
                                            VER = ORGBARCODESTR.Split(',')[1].Trim();
                                            BARCODE = ORGBARCODESTR.Split(',')[0].Trim();

                                            if (INI.ISCTOMODE)
                                            {
                                                switch (Universal.VER)
                                                {
                                                    case "R33":
                                                        ORGBARCODESTR += ",,,,,,,,,,,,,,,";

                                                        QSMCKBSQLSF = new QSMCKBOCRSFClass(ORGBARCODESTR);

                                                        VERSION = QSMCKBSQLSF.GetData(KBOCRSFEnum.KBCOUNTRYCODE) + ":" + QSMCKBSQLSF.GetData(KBOCRSFEnum.KBVISABLE);
                                                        BARCODE = ORGBARCODESTR.Split(',')[12].Trim();

                                                        REGIONClass.myQSMCKBSF = QSMCKBSQLSF.Clone();

                                                        break;
                                                    case "R17":
                                                        ARTWORKNAME = ORGBARCODESTR.Split(',')[2].Trim();

                                                        if (ORGBARCODESTR.Split(',').Length > 3)
                                                            MODELNAME = ORGBARCODESTR.Split(',')[3].Trim();

                                                        break;
                                                    case "R27":
                                                        ARTWORKNAME = ORGBARCODESTR.Split(',')[2].Trim();

                                                        if (ORGBARCODESTR.Split(',').Length > 3)
                                                            MODELNAME = ORGBARCODESTR.Split(',')[3].Trim();

                                                        break;

                                                    case "R26":
                                                    case "R32":

                                                        ORGBARCODESTR += ",,,,,,";

                                                        VERSION = ORGBARCODESTR.Split(',')[1].Trim();
                                                        BARCODE = ORGBARCODESTR.Split(',')[0].Trim();

                                                        ARTWORKNAME = ORGBARCODESTR.Split(',')[2].Trim();

                                                        if (ORGBARCODESTR.Split(',').Length > 3)
                                                            MODELNAME = ORGBARCODESTR.Split(',')[3].Trim();

                                                        RELATECOLORSTR = Universal.COLORTABLE.Check(ORGBARCODESTR.Split(',')[0].Substring(ORGBARCODESTR.Split(',')[0].Trim().Length - 4));

                                                        //QSMCSQLSF = new QSMCSQLSFClass(ORGBARCODESTR);
                                                        QSMCSQLSF = new QSMCSQLSFClass();

                                                        QSMCSQLSF.SetData(SFSQLEnum.UNITSN, BARCODE);
                                                        QSMCSQLSF.SetData(SFSQLEnum.MODELNO, MODELNAME);
                                                        QSMCSQLSF.SetData(SFSQLEnum.COUNTRYCODE, ORGBARCODESTR.Split(',')[4]);
                                                        QSMCSQLSF.SetData(SFSQLEnum.ASSETTAG, ORGBARCODESTR.Split(',')[5]);

                                                        REGIONClass.myQSMCSF = QSMCSQLSF.Clone();

                                                        switch (Universal.OPTION)
                                                        {
                                                            case "AUTOBARCODE":
                                                                IsBYDSecondRun = true;
                                                                break;
                                                            case "FOXCONN":
                                                                RELATECOLORSTR = ORGBARCODESTR.Split(',')[6];
                                                                HOUSINGID = ORGBARCODESTR.Split(',')[7];
                                                                break;
                                                            default:
                                                                RELATECOLORSTR = Universal.COLORTABLE.Check(ORGBARCODESTR.Split(',')[0].Substring(ORGBARCODESTR.Split(',')[0].Trim().Length - 4));
                                                                break;
                                                        }

                                                        break;
                                                        //case "R32":
                                                        //    ARTWORKNAME = BARCODE.Split(',')[2].Trim();

                                                        //    if (BARCODE.Split(',').Length > 3)
                                                        //        MODELNAME = BARCODE.Split(',')[3].Trim();

                                                        //    RELATECOLORSTR = Universal.COLORTABLE.Check(BARCODE.Split(',')[0].Substring(BARCODE.Split(',')[0].Trim().Length - 4));

                                                        //    break;
                                                }
                                            }
                                            //VERSION = ORGBARCODESTR.Split(',')[1].Trim();
                                            //BARCODE = ORGBARCODESTR.Split(',')[0].Trim();

                                            break;
                                    }
                                    break;
                            }

                            */
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            JetEazy.LoggerClass.Instance.WriteException(ex);
                            foreach (string filestrrr in Files)
                            {
                                File.Delete(filestrrr);
                            }

                            if (INI.ISFOXCONNSF)
                            {
                                Universal.Memory.Write("E,ErrorSN");
                                //IsSFActive = false;

                                //LogActionClass.LogAction("QSMCTick(),Memory.Write(E-ErrorSN)" + ",IsSFActive = false");
                            }

                            //LogActionClass.LogAction("QSMCTick(),###Exception=" + "SN.txt格式錯誤:" + "/t" + ee.ToString());
                            //MessageBox.Show("SN.txt格式錯誤:" + "/t" + ee.ToString());

                            return;
                        }
                        //if (filestr != "")
                        //{
                        //    //判断文件是否被占用.
                        //    IntPtr vHandle = _lopen(filestr, OF_READWRITE | OF_SHARE_DENY_NONE);
                        //    if (vHandle == HFILE_ERROR)
                        //        System.Threading.Thread.Sleep(200);

                        //    File.Delete(filestr);
                        //}
                        //if (filestr != "")
                        //    File.Delete(filestr);

                        try
                        {
                            Universal.WipeFile(filestr, 100);
                        }
                        catch (Exception ex)
                        { JetEazy.LoggerClass.Instance.WriteException(ex); }

                        IsPass = false;

                        if (INI.ISCHECKQSMCDUP)
                        {
                            if (DUP.CheckIsOK(BARCODE))
                            {
                                OnEnvTrigger(ResultStatusEnum.SNSTART, -1, SNSTARTOPSTR);

                                //bool_SNRun = true;
                                //IsSFActive = true;
                                //OnTrigger(StatusEnum.QSMCSF);
                            }
                            else
                            {
                                MessageForm DUPFRM = new MessageForm("DUPLICATE ID ERROR.", 5);
                                DUPFRM.Show();
                            }
                        }
                        else
                        {
                            //  OnEnvTrigger(ResultStatusEnum.SNSTART, -1, VER);
                            OnEnvTrigger(ResultStatusEnum.SNSTART, -1, SNSTARTOPSTR);
                            //OnTrigger(StatusEnum.COUNTSTART);

                            //bool_SNRun = true;
                            //IsSFActive = true;

                            //OnTrigger(StatusEnum.QSMCSF);
                        }

                        foreach (string myfiles in Files)
                        {
                            if (myfiles.ToUpper().ToUpper().IndexOf("OCR.TXT") > -1)
                            {
                                if (File.Exists(myfiles))
                                    File.Delete(myfiles);
                            }
                        }
                        //IsFound = true;

                        //break;
                    }

                    //else
                    //{
                    //    //IsFound = false;
                    //    //  break;
                    //}
                }
                //if (IsFound)
                //{
                //    LogActionClass.LogAction("QSMCTick(),Find#SN.txt");

                //    //if (Universal.OPTION != "CLIENT")
                //    //{
                //    //    foreach (string filestr in Files)
                //    //    {
                //    //        File.Delete(filestr);
                //    //    }
                //    //}
                //}

            }

        }
        void FOXCONNTick()
        {
            if (INI.ISFOXCONNSF)
            {
                #region FOXCONN MODE
                string st_Read = Universal.Memory.Read();
                if (st_Read != "")
                {
                    try
                    {
                        string[] Mess = st_Read.Split('#');

                        if (Mess[0] == "1-BARCODE")
                        {
                            Universal.Memory.Write("Y,Text");
                            JzTools.SaveData(Mess[1], @"D:\DATA\SN.TXT");

                            //LogActionClass.LogAction("MainTimer:" + Mess[1]);
                            System.Threading.Thread.Sleep(50);
                        }
                    }
                    catch (Exception ex)
                    {
                        JetEazy.LoggerClass.Instance.WriteException(ex);
                        //LogActionClass.LogAction("MainTimer:" + ex.Message);
                    }
                }
                #endregion
            }
        }

        void MainX6Tick()
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


                        MACHINE.PLCIO.Busy = true;
                        MACHINE.SetLight(AlbumWork.ENVList[0].GeneralLight);
                        //MACHINE.SetLight("0,0,0,0,0,0,0");
                        //MACHINE.SetLight("1,1,1");
                        //string light = AlbumWork.ENVList[0].GeneralLight;
                        if (INI.IsCollectPictures)
                            Universal.MainX6_Path = "D:\\CollectPictures\\Inspection\\" + JzTimes.DateTimeSerialString;


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
                                    using (Bitmap bitmap = m_IxLinescanCamera.GetPageBitmap())
                                    {
                                        CamActClass.Instance.ResetStepCurrent();
                                        if (CamActClass.Instance.StepCurrent >= CamActClass.Instance.StepCount)
                                            CamActClass.Instance.ResetStepCurrent();

                                        FillProcessImageMotorPageIndex(CamActClass.Instance.StepCurrent, bitmap);
                                        //CamActClass.Instance.SetImage(bitmap, CamActClass.Instance.StepCurrent);
                                        //OnTriggerOP(ResultStatusEnum.SET_CURRENT_IMAGE, "ONLINE#" + CamActClass.Instance.StepCurrent.ToString());

                                        LogProcessIDTimer(Process.ID, "线扫取像完成Step=" + CamActClass.Instance.StepCurrent.ToString());

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
                            Process.ID = 10110;
                            LogProcessIDTimer(Process.ID, "测试位置=" + _currentStep.ToString());

                            LogProcessIDTimer(Process.ID, "线程创建" + _currentStep.ToString());
                            ////计算线程
                            //System.Threading.Thread thread_DL_Test = new System.Threading.Thread(DLCalPageIndex);
                            //thread_DL_Test.IsBackground = true;
                            //thread_DL_Test.Start(_currentStep);

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
                                Process.NextDuriation = 0;
                                Process.ID = 1030;

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

                            if (!MACHINE.PLCIO.GetImageOK && !m_DLGetImageOK.IsOn)
                            {
                                //Trigger Start
                                bool isGetImageStart = MACHINE.PLCIO.IsGetImage;

                                if (isGetImageStart && IsGetTestStartOld != isGetImageStart)
                                {
                                    Process.NextDuriation = 300;
                                    Process.ID = 10;

                                    //MACHINE.PLCIO.GetImageOK = false;

                                    LogProcessIDTimer(Process.ID, "获取取像信号 Rev==>Sign GetImage");
                                }
                                IsGetTestStartOld = isGetImageStart;
                            }

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

                            //if (IsNoUseCCD)
                            //    OnTrigger(ResultStatusEnum.COUNTSTART);


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



#if OPT_X6_BAK
void MainX6Tick()
        {
            ProcessClass Process = MainProcess;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:


                        MACHINE.PLCIO.Busy = true;
                        MACHINE.SetLight("1,1,1");

                        if (INI.IsCollectPictures)
                            Universal.MainX6_Path = "D:\\CollectPictures\\Inspection\\" + JzTimes.DateTimeSerialString;


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

                        break;
                    case 10:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
                        if (Process.IsTimeup)
                        {
                            switch (Universal.CAMACT)
                            {
                                case CameraActionMode.CAM_MOTOR:
                                    break;
                                case CameraActionMode.CAM_STATIC:
                                    OnEnvTrigger(ResultStatusEnum.SETCAMLIGHT, EnvIndex, "ALL");
                                    break;
                            }
                            

                            Process.NextDuriation = 500;
                            Process.ID = 1030;
                        }
                        break;
                    case 1030:                        //拍片完後關燈
                        if (Process.IsTimeup)
                        {

                            switch(Universal.CAMACT)
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

                            AlbumWork.A08_RunProcess(PageOPTypeEnum.P00);
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
                        }
                        break;
                    case 40:

                        Process.Stop();
                        RXXLastProcess();

                        GC.Collect();
                        break;
                }
            }


        }

#endif

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

        #region MAIN_X6 德龍馬達運動取像流程

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
                                    using (Bitmap bitmap = m_IxLinescanCamera.GetPageBitmap())
                                    {
                                        if (CamActClass.Instance.StepCurrent >= CamActClass.Instance.StepCount)
                                            CamActClass.Instance.ResetStepCurrent();
                                        //bitmap.Save("D:\\LOA\\TEST.PNG", ImageFormat.Png);
                                        CamActClass.Instance.SetImage(bitmap, CamActClass.Instance.StepCurrent);
                                        OnTriggerOP(ResultStatusEnum.SET_CURRENT_IMAGE, "ONLINE#" + CamActClass.Instance.StepCurrent.ToString());
                                        CamActClass.Instance.StepCurrent++;

                                        m_DLGetImageOK.Start();
                                    }
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
                        LogProcessIDTimer(80, Process.RelateString + "Finish ON", false);

                        if (INI.IsReadHandlerOKSign)
                        {
                            if (INI.IsSendHandlerTcpOKSign)
                            {
                                int _currentStep = CamActClass.Instance.StepCurrent;
                                _tcpSendCompleteOKSign(1, _currentStep, -1);
                                LogProcessIDTimer(80, "1," + _currentStep.ToString() + ",-1", false);

                                //if (Process.RelateString.Length >= 3)
                                //{
                                //    switch(Process.RelateString[0])
                                //    {
                                //        case 'C':
                                //            int _currentStep = int.Parse(Process.RelateString.Split(',')[1]);
                                //            _tcpSendCompleteOKSign(1, _currentStep, -1);
                                //            LogProcessIDTimer(80, "1," + _currentStep.ToString() + ",-1", false);
                                //            break;
                                //    }
                                //}
                            }
                        }

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (INI.IsReadHandlerOKSign)
                            {
                                if (MACHINE.PLCIO.IsHandlerOK)
                                {
                                    Process.Stop();
                                    MACHINE.PLCIO.GetImageOK = false;
                                    LogProcessIDTimer(81, Process.RelateString + "Finish OFF", false);
                                }
                            }
                            else
                            {
                                Process.Stop();
                                MACHINE.PLCIO.GetImageOK = false;
                                LogProcessIDTimer(81, Process.RelateString + "Finish OFF", false);
                            }
                        }
                        break;
                }
            }
        }
        //void DLGetImageOKTick()
        //{
        //    ProcessClass Process = m_DLGetImageOK;

        //    if (Process.IsOn)
        //    {
        //        switch (Process.ID)
        //        {
        //            case 5:

        //                Process.TimeUnit = TimeUnitEnum.ms;
        //                Process.NextDuriation = INI.handle_delaytime;
        //                Process.ID = 6;
        //                MACHINE.PLCIO.GetImageOK = true;
        //                LogProcessIDTimer(80, "Finish ON", false);
        //                break;

        //            case 6:
        //                if (Process.IsTimeup)
        //                {
        //                    if (MACHINE.PLCIO.GetImageOK)
        //                    {
        //                        Process.NextDuriation = 0;
        //                        Process.ID = 10;
        //                    }
        //                }
        //                break;

        //            case 10:
        //                if (Process.IsTimeup)
        //                {
        //                    if (INI.IsReadHandlerOKSign)
        //                    {
        //                        if (MACHINE.PLCIO.IsHandlerOK)
        //                        {
        //                            //Process.Stop();
        //                            MACHINE.PLCIO.GetImageOK = false;
        //                            Process.NextDuriation = 0;
        //                            Process.ID = 11;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        //Process.Stop();
        //                        MACHINE.PLCIO.GetImageOK = false;
        //                        //LogProcessIDTimer(81, "Finish OFF", false);
        //                        Process.NextDuriation = 0;
        //                        Process.ID = 11;
        //                    }

        //                }
        //                break;
        //            case 11:
        //                if (Process.IsTimeup)
        //                {
        //                    if (!MACHINE.PLCIO.GetImageOK)
        //                    {
        //                        Process.Stop();
        //                        LogProcessIDTimer(81, "Finish OFF", false);
        //                    }
        //                }
        //                break;
        //        }
        //    }
        //}

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

                        if (MACHINE.PLCIO.Ready)
                        {
                            MACHINE.PLCIO.Pass = IsPass;
                            MACHINE.PLCIO.Fail = !IsPass;
                        }

                        switch (Universal.CAMACT)
                        {
                            case CameraActionMode.CAM_MOTOR_LINESCAN:
                            case CameraActionMode.CAM_MOTOR_MODE2:
                                MACHINE.PLCIO.GetImageOK = true;
                                LogProcessIDTimer(80, "Result Finish ON", false);

                                if (INI.IsReadHandlerOKSign)
                                {
                                    if (INI.IsSendHandlerTcpOKSign)
                                    {
                                        int currentStep = CamActClass.Instance.StepCount - 1;
                                        _tcpSendCompleteOKSign(1, currentStep, (IsPass ? 0 : 1));
                                        LogProcessIDTimer(90, "1," + currentStep.ToString() + "," + (IsPass ? "0" : "1"), false);
                                        //if (Process.RelateString.Length >= 3)
                                        //{
                                        //    switch (Process.RelateString[0])
                                        //    {
                                        //        case 'C':
                                        //            int _currentStep = int.Parse(Process.RelateString.Split(',')[1]);
                                        //            _tcpSendCompleteOKSign(1, _currentStep, -1);
                                        //            break;
                                        //    }
                                        //}
                                    }
                                }

                                //LogProcessIDTimer(8889, "发送最后取像完成信号PC==>PLC ON");
                                break;
                        }

                        break;

                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (INI.IsReadHandlerOKSign)
                            {
                                if (MACHINE.PLCIO.IsHandlerOK)
                                {
                                    Process.Stop();
                                    MACHINE.PLCIO.Busy = false;

                                    switch (Universal.CAMACT)
                                    {
                                        case CameraActionMode.CAM_MOTOR_LINESCAN:
                                        case CameraActionMode.CAM_MOTOR_MODE2:
                                            MACHINE.PLCIO.GetImageOK = false;
                                            LogProcessIDTimer(81, "Result Finish OFF", false);
                                            //LogProcessIDTimer(8889, "发送最后取像完成信号PC==>PLC OFF");
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                Process.Stop();
                                MACHINE.PLCIO.Busy = false;
                                MACHINE.PLCIO.Pass = false;
                                MACHINE.PLCIO.Fail = false;

                                switch (Universal.CAMACT)
                                {
                                    case CameraActionMode.CAM_MOTOR_LINESCAN:
                                    case CameraActionMode.CAM_MOTOR_MODE2:
                                        MACHINE.PLCIO.GetImageOK = false;
                                        LogProcessIDTimer(81, "Result Finish OFF", false);
                                        //LogProcessIDTimer(8889, "发送最后取像完成信号PC==>PLC OFF");
                                        break;
                                }
                            }
                        }
                        break;
                }
            }
        }

        private void _tcpSendCompleteOKSign(int eCompleteSign, int eCurrentStep, int eResult)
        {
            string Str = "CompleteSign=" + eCompleteSign.ToString();
            Str += "CurrentStep=" + eCurrentStep.ToString();
            Str += "Result=" + eResult.ToString();

            X6_HANDLE_CLIENT.Log.Log2("tcpCmd.CMD_SENDCOMPLETESIGN" + Str);
            string _cmd = eCompleteSign.ToString() + "," + eCurrentStep.ToString() + "," + eResult.ToString();
            byte[] bytedata2 = Encoding.UTF8.GetBytes(_cmd);

            byte[] bytedata = new byte[32 + bytedata2.Length];
            bytedata[0] = 29;
            //bytedata[4] = 4;
            bytedata[8] = 0;
            //bytedata[32] = (iret == 0 ? (byte)1 : (byte)3);

            for (int i = 0; i < bytedata2.Length; i++)
            {
                bytedata[32 + i] = bytedata2[i];
            }

            int tu5x = bytedata2.Length;
            bytedata[4] = (byte)(tu5x & 0xFF);
            bytedata[5] = (byte)(tu5x >> 8 & 0xFF);
            bytedata[6] = (byte)(tu5x >> 16 & 0xFF);
            bytedata[7] = (byte)(tu5x >> 24 & 0xFF);

            //byte[] a2 = bytes.Skip(4).Take(4).ToArray();
            //Int32 aa2 = BitConverter.ToInt32(bytes5x, 0);

            //bytedata[4] = (byte)bytedata2.Length;//数据长度

            try
            {
                X6_HANDLE_CLIENT.Send(bytedata);
            }
            catch (Exception ex)
            {
                X6_HANDLE_CLIENT.Log.Log2("tcpCmd.CMD_SENDCOMPLETESIGN:Exception" + ex.Message);
            }

        }

        #endregion

        void R32Tick()
        {
            ProcessClass Process = MainProcess;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        TestTimer.Cut();

                        m_input_time = DateTime.Now;

                        OnEnvTrigger(ResultStatusEnum.CHANGEDIRECTORY, 0, "");
                        OnTrigger(ResultStatusEnum.COUNTSTART);


                        //把要檢測的東西放進去
                        FillOperaterString(RELATECOLORSTR);

                        Process.NextDuriation = 50;

                        if (IsNoUseCCD)
                            Process.ID = 20;
                        else
                            Process.ID = 10;

                        break;
                    case 10:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
                        if (Process.IsTimeup)
                        {
                            OnEnvTrigger(ResultStatusEnum.SETCAMLIGHT, EnvIndex, "ALL");

                            Process.NextDuriation = 500;
                            Process.ID = 1030;
                        }
                        break;
                    case 1030:                        //拍片完後關燈
                        if (Process.IsTimeup)
                        {

                            CCDCollection.GetR32Image();
                            FillProcessImage();
                            //  Universal.ALBCollection.AlbumWork.CPD.bmpOCRCheckErr = null;
                            OnEnvTrigger(ResultStatusEnum.SAVEDEBUGRAW, EnvIndex, "-1");

                            MACHINE.SetLight("");

                            //////
                            Process.NextDuriation = 100;
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

                            CCDCollection.GetR32Image();

                            //Testms[1] = TestTimer.msDuriation;
                            //TestTimer.Cut();

                            FillProcessImage();

                            //Testms[2] = TestTimer.msDuriation;
                            //TestTimer.Cut();

                            Process.NextDuriation = 50;
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

                            AlbumWork.A08_RunProcess(PageOPTypeEnum.P00);
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

                                    Process.NextDuriation = 50;
                                    Process.ID = 10;

                                    return;
                                }
                            }
                            else
                            {
                                OnEnvTrigger(ResultStatusEnum.SAVENGRAW, EnvIndex, "-1");
                            }

                            Process.NextDuriation = 50;
                            Process.ID = 40;
                        }
                        break;
                    case 40:

                        Process.Stop();
                        RXXLastProcess();

                        //foreach (EnvClass env in AlbumWork.ENVList)
                        //{
                        //    foreach (PageClass page in env.PageList)
                        //    {
                        //        foreach (AnalyzeClass anay in page.AnalyzeRootArray)
                        //        {
                        //            anay.bmpOUTPUT.Dispose();
                        //            anay.bmpWIP.Dispose();
                        //            if (anay.bmpORGLEARNININPUT != null)
                        //                anay.bmpORGLEARNININPUT.Dispose();
                        //            // anay.bmpPATTERN.Dispose();
                        //            // anay.bmpMASK.Dispose();

                        //            foreach (AnalyzeClass anayTemp in anay.BranchList)
                        //            {
                        //                anayTemp.bmpOUTPUT.Dispose();
                        //                anayTemp.bmpWIP.Dispose();
                        //                if (anayTemp.bmpORGLEARNININPUT != null)
                        //                    anayTemp.bmpORGLEARNININPUT.Dispose();
                        //                //  anayTemp.bmpPATTERN.Dispose();
                        //                //  anayTemp.bmpMASK.Dispose();
                        //            }
                        //        }

                        //        //foreach (Bitmap bmptemp in page.bmpRUN)
                        //        //    bmptemp.Dispose();

                        //    }
                        //}
                        GC.Collect();
                        break;
                }
            }


        }

        void R26Tick()
        {
            ProcessClass Process = MainProcess;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:
                        bool is8001OK = false;
                        foreach (EnvClass env in Universal.ALBCollection.AlbumNow.ENVList)
                        {
                            foreach (PageClass page in env.PageList)
                            {
                                if (page.RelateToRcpNo == 80001)
                                {
                                    is8001OK = true;
                                }
                            }
                        }
                        if (!is8001OK)
                        {
                            IsStopNormalTick = false;
                            Process.Stop();
                            MessageBox.Show("本参数没有对应到 镭雕参数，请相关工程人员确认！");

                            return;
                        }

                        TestTimer.Cut();

                        m_input_time = DateTime.Now;

                        OnEnvTrigger(ResultStatusEnum.CHANGEDIRECTORY, 0, "");
                        OnTrigger(ResultStatusEnum.COUNTSTART);


                        //把要檢測的東西放進去
                        FillOperaterString(RELATECOLORSTR);
                        Process.NextDuriation = 50;

                        MACHINE.SetLight(Universal.ALBCollection.AlbumNow.ENVList[0].GeneralLight);

                        if (IsNoUseCCD)
                            Process.ID = 20;
                        else
                            Process.ID = 10;

                        break;
                    case 10:                        //變換CCD亮度設定及光源設定，並且合起鍵盤的壓框
                        if (Process.IsTimeup)
                        {
                            OnEnvTrigger(ResultStatusEnum.SETCAMLIGHT, EnvIndex, "ALL");

                            Process.NextDuriation = INI.DELAYTIME;
                            Process.ID = 1030;
                        }
                        break;
                    case 1030:                        //拍片完後關燈
                        if (Process.IsTimeup)
                        {

                            CCDCollection.GetR26Image("0,1,2,3");
                            //FillProcessImage();
                            //  Universal.ALBCollection.AlbumWork.CPD.bmpOCRCheckErr = null;
                            //OnEnvTrigger(ResultStatusEnum.SAVEDEBUGRAW, EnvIndex, "-1");

                            MACHINE.SetLight("");
                            MACHINE.PLCIO.TopLight = true;      //關所有的燈後開神燈

                            //////
                            Process.NextDuriation = 10;
                            Process.ID = 1033;
                        }
                        break;
                    case 1033:                        //神燈開後等一下
                        if (Process.IsTimeup)
                        {
                            Process.NextDuriation = INI.DELAYTIME; ;
                            Process.ID = 1035;
                        }
                        break;
                    case 1035:                        //拍片完後關燈
                        if (Process.IsTimeup)
                        {

                            CCDCollection.GetR26Image("4,5");
                            FillProcessImage();
                            //  Universal.ALBCollection.AlbumWork.CPD.bmpOCRCheckErr = null;
                            OnEnvTrigger(ResultStatusEnum.SAVEDEBUGRAW, EnvIndex, "-1");

                            OnEnvTrigger(ResultStatusEnum.SETCAMLIGHT, EnvIndex, "ALL");
                            MACHINE.SetLight("");
                            //MACHINE.PLCIO.GodLight = true;

                            //////
                            Process.NextDuriation = 100;
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
                            MACHINE.SetLight("");
                            CCDCollection.GetR26Image();

                            //Testms[1] = TestTimer.msDuriation;
                            //TestTimer.Cut();

                            FillProcessImage();

                            //Testms[2] = TestTimer.msDuriation;
                            //TestTimer.Cut();

                            Process.NextDuriation = 50;
                            Process.ID = 30;
                        }
                        break;
                    case 30:
                        if (Process.IsTimeup)
                        {
                            //Testms[3] = TestTimer.msDuriation;
                            TestTimer.Cut();
                            OnEnvTrigger(ResultStatusEnum.SAVEHIGHTRAW, EnvIndex, "-1");

                            if (IsNoUseCCD)
                                OnTrigger(ResultStatusEnum.COUNTSTART);

                            AlbumWork.A08_RunProcess(PageOPTypeEnum.P00);
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

                                    Process.NextDuriation = 50;
                                    Process.ID = 10;

                                    return;
                                }
                            }
                            else
                            {
                                OnEnvTrigger(ResultStatusEnum.SAVENGRAW, EnvIndex, "-1");
                            }

                            Process.NextDuriation = 50;
                            Process.ID = 40;
                        }
                        break;
                    case 40:

                        Process.Stop();
                        RXXLastProcess();
                        OnEnvTrigger(ResultStatusEnum.SETCAMLIGHT, EnvIndex, "ALL");
                        break;
                }
            }
        }

        /// <summary>
        /// RXX 結束流程後要做的雞巴毛事
        /// </summary>
        void RXXLastProcess()
        {
            //m_DLResultOK.Start((IsPass ? "PASS" : "FAIL"));
            //m_DLResultOK.Start();
            //LogProcessIDTimer(8888, "发送结果信号PC==>PLC");

            switch (Universal.CAMACT)
            {
                case CameraActionMode.CAM_MOTOR_MODE2:
                    //System.Threading.Thread threadGetImageOK = new System.Threading.Thread(SendSignToPLCImageOK);
                    //threadGetImageOK.Start();

                    //m_DLGetImageOK.Start();
                    //LogProcessIDTimer(8889, "发送最后取像完成信号PC==>PLC");
                    break;
            }

            if (IsPass)
            {
                PlayerPass.Play();
                OnTrigger(ResultStatusEnum.CALPASS);
            }
            else
            {
                PlayerFail.Play();
                OnTrigger(ResultStatusEnum.CALNG);

                JzMainSDPositionParas.INSPECT_NGINDEX++;
                JzMainSDPositionParas.SaveRecord();
            }
            if (INI.ISQSMCALLSAVE)
                _saveAllResultPictures();

            if (INI.IsCollectPictures)
                MainX6Save();

            OnTrigger(ResultStatusEnum.COUNTEND);
            OnTrigger(ResultStatusEnum.CALEND);

            if (!IsPass)
            {
                JzMainSDPositionParas.ReportAUTOSave(JzMainSDPositionParas.INSPECT_NGINDEX, false, true);
                if (INI.IsCollectStripPictures)
                    MainX6StripImageDataSave();//保存strip图片
            }
            JzMainSDPositionParas.ReportGradeSave(JzMainSDPositionParas.INSPECT_NGINDEX, false);

            m_DLResultOK.Start();
            LogProcessIDTimer(8888, $"发送结果信号PC==>PLC {(IsPass ? "PASS" : "NG")}");
            Universal.IsRunningTest = false;

        }

        //int tickCmd = Environment.TickCount;
        void SendSignToPLC(object obj)
        {
            bool ispass = (bool)obj;

            MACHINE.PLCIO.Busy = false;
            if (MACHINE.PLCIO.Ready)
            {
                MACHINE.PLCIO.Pass = ispass;
                MACHINE.PLCIO.Fail = !ispass;
            }

            //延时一定时间关闭点位
            int tickCmd = Environment.TickCount;
            while (Environment.TickCount - tickCmd < INI.handle_delaytime) { Application.DoEvents(); }

            MACHINE.PLCIO.Pass = false;
            MACHINE.PLCIO.Fail = false;

        }
        //int tickCmdImageOK = Environment.TickCount;
        void SendSignToPLCImageOK()
        {
            MACHINE.PLCIO.GetImageOK = true;
            //延时一定时间关闭点位
            int tickCmdImageOK = Environment.TickCount;
            while (Environment.TickCount - tickCmdImageOK < INI.handle_delaytime) { Application.DoEvents(); }
            MACHINE.PLCIO.GetImageOK = false;
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

        /// <summary>
        /// 播放声音
        /// </summary>
        void _playsound()
        {
            if (INI.ISPLAYSOUND)
            {
                switch (INI.SFFACTORY)
                {
                    case FactoryShopfloor.FOXCONN:
                        if (IsPass)
                        {
                            switch (HOUSINGID)
                            {
                                case "0":
                                    JzToolsClass.Playing(Universal.MAINPATH + "\\WORK\\Housing0.WAV");
                                    break;
                                case "1":
                                    JzToolsClass.Playing(Universal.MAINPATH + "\\WORK\\Housing1.WAV");
                                    break;
                                default:
                                    JzToolsClass.Playing(Universal.MAINPATH + "\\WORK\\Housing0.WAV");
                                    break;
                            }
                        }
                        else
                            JzToolsClass.Playing(Universal.MAINPATH + "\\WORK\\Fail.WAV");
                        break;
                }
            }
        }
        #region FOXCONN MODE
        /// <summary>
        /// 检查镭雕SN是否错误
        /// </summary>
        bool m_CheckLaserSnError = false;
        /// <summary>
        /// 富士康报表保存
        /// </summary>
        void _savefoxconnreport()
        {
            string strFoxconnPath = @"D:\ALLRESULTPIC\REPORTS\";
            string strFoxconnFileName = JzTimes.DateSerialString + ".csv";
            string strFullFileName = strFoxconnPath + strFoxconnFileName;

            if (!System.IO.Directory.Exists(strFoxconnPath))
                System.IO.Directory.CreateDirectory(strFoxconnPath);

            string strReportMsg = "";
            string strHead = "SN,MachineName,MachineID,Color,KBCountryCode,LaserCountryCode,TestTime,StartTime,Result" + Environment.NewLine;

            if (!System.IO.File.Exists(strFullFileName))
                strReportMsg += strHead;

            strReportMsg += BARCODE + ",";
            strReportMsg += INI.MACHINENAME + ",";
            strReportMsg += INI.MACHINENAMEID + ",";
            strReportMsg += RELATECOLORSTR + ",";
            strReportMsg += VERSION + ",";
            strReportMsg += ARTWORKNAME + ",";
            strReportMsg += Testms[0].ToString() + ",";
            strReportMsg += m_input_time.ToString("yyyy/MM/dd HH:mm:ss") + ",";
            strReportMsg += (IsPass ? "PASS" : "FAIL") + Environment.NewLine;

            JzTools.SaveDataEX(strReportMsg, strFullFileName);

        }
        /// <summary>
        /// 组合错误原因
        /// </summary>
        /// <param name="m_ispass">检测是否PASS</param>
        void _httpuploaddata(bool m_ispass)
        {
            string Str_Name = "";
            string Str_Code = "";
            string HTTPStr_Name = "";
            string HTTPStr_Code = "";

            #region APPLE NAME AND VALUE
            List<string> RegionReportList = new List<string>();
            m_CheckLaserSnError = false;

            int i = 0;
            foreach (PageClass page in AlbumWork.ENVList[0].PageList)
            {
                AnalyzeClass analyzeroot = page.AnalyzeRoot;
                foreach (AnalyzeClass analyze in analyzeroot.BranchList)
                {
                    if (analyze.RunStatusCollection.NGCOUNT > 0)
                    {
                        //m_CheckLaserSnError = false;
                        foreach (WorkStatusClass work in analyze.RunStatusCollection.WorkStatusList)
                        {
                            if (work.AnalyzeProcedure == AnanlyzeProcedureEnum.CHECKOCR)
                            {
                                if (work.Reason == ReasonEnum.NG)
                                {
                                    m_CheckLaserSnError = true;
                                    break;
                                }
                            }
                        }
                        string str = analyze.AliasName + "," + analyze.RunStatusCollection.GetNGRunStatus(0).Reason.ToString();
                        if (m_CheckLaserSnError)
                        {
                            RegionReportList.Add(analyze.AliasName + "," + INI.CHECKSNERRORCODE);
                        }
                        else
                            RegionReportList.Add(str);
                        //string str = analyze.AliasName + "," + analyze.RunStatusCollection.GetNGRunStatus(0).Reason.ToString();
                        ////RegionReportList.Add(str);
                        ////if (!m_CheckLaserSnError)
                        //{
                        //    if (analyze.OCRPara.OCRMethod == OCRMethodEnum.MAPPING)
                        //    {
                        //        if (analyze.RunStatusCollection.GetNGRunStatus(0).Reason == ReasonEnum.NG)
                        //            RegionReportList.Add(analyze.AliasName + "," + INI.CHECKSNERRORCODE);
                        //    }
                        //    else
                        //    {
                        //        RegionReportList.Add(str);
                        //    }
                        //}
                    }
                    else
                    {
                        i = 0;
                        while (i < analyze.BranchList.Count)
                        {
                            if (analyze.BranchList[i].RunStatusCollection.NGCOUNT > 0)
                            {
                                //m_CheckLaserSnError = false;
                                foreach (WorkStatusClass work in analyze.BranchList[i].RunStatusCollection.WorkStatusList)
                                {
                                    if (work.AnalyzeProcedure == AnanlyzeProcedureEnum.CHECKOCR)
                                    {
                                        if (work.Reason == ReasonEnum.NG)
                                        {
                                            m_CheckLaserSnError = true;
                                            break;
                                        }
                                    }
                                }
                                string str = analyze.AliasName + "," + analyze.BranchList[i].RunStatusCollection.GetNGRunStatus(0).Reason.ToString();
                                if (m_CheckLaserSnError)
                                {
                                    RegionReportList.Add(analyze.AliasName + "," + INI.CHECKSNERRORCODE);
                                }
                                else
                                    RegionReportList.Add(str);
                                //string str = analyze.AliasName + "," + analyze.BranchList[i].RunStatusCollection.GetNGRunStatus(0).Reason.ToString();
                                ////RegionReportList.Add(str);
                                ////if (!m_CheckLaserSnError)
                                //{
                                //    if (analyze.OCRPara.OCRMethod == OCRMethodEnum.MAPPING)
                                //    {
                                //        if (analyze.BranchList[i].RunStatusCollection.GetNGRunStatus(0).Reason == ReasonEnum.NG)
                                //            RegionReportList.Add(analyze.AliasName + "," + INI.CHECKSNERRORCODE);
                                //    }
                                //    else
                                //    {
                                //        RegionReportList.Add(str);
                                //    }
                                //}
                            }
                            i++;
                        }
                    }
                }
            }

            RegionReportList.Sort();

            foreach (string str in RegionReportList)
            {
                if (str != "")
                {
                    string[] tmp = str.Split(',');
                    if (tmp.Length >= 2)
                    {
                        //HTTPStr_Name += tmp[1] + "/";
                        //HTTPStr_Code += tmp[0] + "/";
                        HTTPStr_Name = tmp[1] + "/";
                        HTTPStr_Code = (m_CheckLaserSnError ? INI.CHECKSNERRORCODE : "NG") + "/";
                        if (m_CheckLaserSnError)
                            break;
                    }
                }
            }


            foreach (string str in RegionReportList)
            {
                if (str != "")
                {
                    string[] tmp = str.Split(',');
                    if (tmp.Length >= 2)
                    {
                        Str_Name += tmp[0] + "%" + tmp[1] + "^";
                    }
                }
            }

            Str_Code = INI.MACHINENAME;
            #endregion

            if (HTTPStr_Name != "")
                HTTPStr_Name = HTTPStr_Name.Remove(HTTPStr_Name.Length - 1);
            if (HTTPStr_Code != "")
                HTTPStr_Code = HTTPStr_Code.Remove(HTTPStr_Code.Length - 1);

            string Pass = (m_ispass ? "PASS" : "FAIL");
            //传信息给http
            string st_Data = "B," + BARCODE + "," +
                            Pass + "," +
                            Str_Name + "," +
                            Str_Code + "," +
                            Universal.VersionDate + JzHiveClass.HiveVersion + "," +
                            HTTPStr_Name + "," +
                            HTTPStr_Code + "," +
                            VERSION;

            Universal.Memory.Write(st_Data);

            //Universal.WriteLog("Write HttpMessage:" + st_Data);
        }
        #endregion
        private string _get_qsmc_sndata_json()
        {
            Hashtable hash = new Hashtable();
            hash.Add("VER", VER);
            hash.Add("BARCODE", BARCODE);
            hash.Add("ARTWORKNAME", ARTWORKNAME);
            hash.Add("MODELNAME", MODELNAME);
            hash.Add("RELATECOLORSTR", RELATECOLORSTR);
            string strjson = JsonConvert.SerializeObject(hash);//{"key1":"val1","key2":"val2"}
            return strjson;
        }
        private string _get_ocr_result_data_json()
        {
            Hashtable hash = new Hashtable();
            hash.Add("data1", "OK");
            hash.Add("data2", "OK");
            hash.Add("data3", "OK");
            string strjson = JsonConvert.SerializeObject(hash);
            return strjson;
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

        string mainx6_path = "D:\\CollectPictures";
        private void MainX6Save()
        {
            mainx6_path = "D:\\CollectPictures\\" + JzTimes.DateSerialString + "\\" + (IsPass ? "P-" : "F-") + JzTimes.DateTimeSerialString;

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
        private void MainX6StripImageDataSave()
        {
            string _imagePath = "D:\\REPORT\\work\\Image\\auto_" + JzMainSDPositionParas.Report_LOT + "\\" + JzMainSDPositionParas.INSPECT_NGINDEX.ToString("00000");
            _imagePath = "D:\\REPORT\\work\\Image\\" + JzTimes.DateSerialString + "\\auto_" + JzMainSDPositionParas.Report_LOT + "\\" + JzMainSDPositionParas.INSPECT_NGINDEX.ToString("00000");

            if (!Directory.Exists(_imagePath + "\\000"))
                Directory.CreateDirectory(_imagePath + "\\000");

            EnvClass env = AlbumWork.ENVList[0];

            int qi = 0;
            foreach (PageClass page in env.PageList)
            {
                page.GetbmpRUN().Save(_imagePath + "\\000\\P00-" + qi.ToString("000") + ".png", ImageFormat.Png);
                qi++;
            }
        }
        public void SavePrintScreenForMainX6()
        {
            string screen_SavePath = "D:\\CollectPictures\\Screen\\" + JzTimes.DateSerialString;

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

        #endregion
    }
}
