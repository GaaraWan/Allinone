﻿using System;
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
using System.Security.Cryptography;

namespace Allinone.OPSpace.ResultSpace
{
    public class JzR15ResultClass : GeoResultClass
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

        public string ARTWORKNAME = "";
        public string MODELNAME = "";
        public string ORGBARCODESTR = "";
        public string HOUSINGID = "";
       // public string RELATECOLORSTR = "";
        public string SNSTARTOPSTR
        {
            get
            {
                return VER + "$" + ARTWORKNAME + "-" + RELATECOLORSTR;
            }
        }

        JzR15MachineClass MACHINE;

        JzToolsClass JzTools = new JzToolsClass();
        SoundPlayer PlayerPass = new SoundPlayer();
        SoundPlayer PlayerFail = new SoundPlayer();

        public JzR15ResultClass(Result_EA resultea,VersionEnum version,OptionEnum option,MachineCollectionClass machinecollection)
        {
            myResultEA = resultea;
            VERSION = version;
            OPTION = option;

            PlayerPass.SoundLocation = Universal.PlayerPASSPATH;
            PlayerFail.SoundLocation = Universal.PlayerFAILPATH;
            PlayerPass.Load();
            PlayerFail.Load();

            DUP = new DupClass();

            MACHINE = (JzR15MachineClass)machinecollection.MACHINE;
            
            MainProcess = new ProcessClass();
        }

        private void CCDCollection_TriggerAction(string operationstr)
        {
            throw new NotImplementedException();
        }

        public override void GetStart(AlbumClass albumwork,CCDCollectionClass ccdcollection,TestMethodEnum testmethod, bool isnouseccd)
        {
            if(MainProcess.IsOn)
            {
                MainProcess.Stop();

                if (INI.ISHIVECLIENT)
                {
                    MACHINE.SetMachineState(MachineState.Idle);
                }

                OnTrigger(ResultStatusEnum.FORECEEND);
                
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

            switch(TestMethod)
            {
                case TestMethodEnum.QSMCSF:
                    if (INI.ISHIVECLIENT)
                    {
                        MACHINE.SetMachineState(MachineState.Running);
                        if (MACHINE.GetCurrentMachineState == MachineState.Running)
                            MACHINE.HIVECLIENT.Hiveclient_ConfigurationMap(BARCODE, "SF", INI.DATA_Program, INI.DATA_Building_Config, _get_qsmc_sndata_json());
                    }
                    break;
            }

        }
        public override void Tick()
        {
            if(!IsNoUseCCD && MACHINE.PLCIO.IsUPSError)
            {
                if (MainProcess.IsOn)
                {
                    MainProcess.Stop();
                    IsStopNormalTick = false;
                    MessageBox.Show("UPS Error 请检查UPS 或相关接线！");
                }
            }
            FOXCONNTick();
            MainProcessTick();

            switch(OPTION)
            {
                case OptionEnum.R15:
                    QSMCSFR15Tick();
                    break;
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

        protected override void MainProcessTick()
        {
            switch(OPTION)
            {
                case OptionEnum.R32:
                    R32Tick();
                    break;
                case OptionEnum.R26:
                    //R26Tick();
                    break;
                case OptionEnum.R15:
                    R15Tick();
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
        public void FillProcessImage(string opstr)
        {
            int i = 0;

            EnvClass env = AlbumWork.ENVList[EnvIndex];

            foreach (PageClass page in env.PageList)
            {
                if((opstr + ",").IndexOf(page.No.ToString(PageClass.ORGPAGENOSTRING) + ",") >-1 )
                    page.SetbmpRUN(PageOPTypeEnum.P00, CCDCollection.GetBMP(page.CamIndex, false));

                i++;
            }
        }
        public void FillProcessImage(string opstr,Bitmap bmp)
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
   
        void QSMCSFR15Tick()
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
                         Universal.   WipeFile(filestr, 100);
                        }
                        catch(Exception ex)
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
                        R15LastProcess();

                        break;
                }
            }


        }
        
        void R15Tick()
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
                        Universal.ISCHECKSN = false;

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

                            CCDCollection.GetR15Image("0,1,2,3");
                            //FillProcessImage();
                            //  Universal.ALBCollection.AlbumWork.CPD.bmpOCRCheckErr = null;
                            //OnEnvTrigger(ResultStatusEnum.SAVEDEBUGRAW, EnvIndex, "-1");

                            MACHINE.SetLight("");
                            MACHINE.PLCIO.GodLight = true;      //關所有的燈後開神燈

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

                            CCDCollection.GetR15Image("4,5");
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
                            OnEnvTrigger(ResultStatusEnum.SAVEDEBUGRAW, EnvIndex, "-1");

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
                        OnEnvTrigger(ResultStatusEnum.SETCAMLIGHT, EnvIndex, "ALL");
                        Process.Stop();
                        R15LastProcess();

                        break;
                }
            }
        }
        /// <summary>
        /// R15 結束流程後要做的雞巴毛事
        /// </summary>
        void R15LastProcess()
        {
            if (IsPass && !Universal.ISCHECKSN)
            {
                IsPass = false;
                MessageBox.Show("参数中没有检测 SN 的设定，请相关工程人员检查！");

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
            }
            if (INI.ISQSMCALLSAVE || INI.ISHIVECLIENT)
                _saveAllResultPictures();
            if (Universal.RESULT.TestMethod == TestMethodEnum.QSMCSF && INI.SFFACTORY == FactoryShopfloor.QSMC)
            {
                string strsndata = BARCODE + ",OCR," + JzTimes.DateTimeSerialString + "," + (IsPass ? "P" : "F");
                string filename = INI.SHOPFLOORPATH + "\\" + BARCODE + "OCR.txt";
                JzTools.SaveData(strsndata, filename);
            }
            if (INI.ISFOXCONNSF)
            {
                _playsound();
                _httpuploaddata(IsPass);
               
            }
            _savefoxconnreport();
            OnTrigger(ResultStatusEnum.COUNTEND);
            OnTrigger(ResultStatusEnum.CALEND);

            if (INI.ISHIVECLIENT)
            {
                if (MACHINE.GetCurrentMachineState == MachineState.Running)
                {
                    //MACHINE.HIVECLIENT.Hiveclient_MachineData(BARCODE, BARCODE, IsPass, m_input_time, DateTime.Now, _get_ocr_result_data_json());

                    //Create by Gaara [Find Path_Hive_Pictures all files]

                    List<string> _listHivePicture = new List<string>();
                    if (Directory.Exists(Path_Hive_Pictures))
                    {
                        string[] _filesHivePicture = Directory.GetFiles(Path_Hive_Pictures);
                        if (_filesHivePicture.Length > 0)
                        {
                            _listHivePicture = _filesHivePicture.ToList<string>();
                        }
                    }
                    if (File.Exists(FullPathName_Hive_Reports))
                    {
                        _listHivePicture.Add(FullPathName_Hive_Reports);
                    }
                    string strApplePath = @"D:\ALLRESULTPIC\REPORTS\FORMAT01\";
                    string strAppleFileName = JzTimes.DateSerialString + "_" + INI.DATA_FIXTUREID + ".csv";
                    string strFullFileName = strApplePath + strAppleFileName;
                    if (File.Exists(strFullFileName))
                    {
                        _listHivePicture.Add(strFullFileName);
                    }

                    //MACHINE.HIVECLIENT.Hiveclient_MachineData_Files(BARCODE,
                    //                                                                                                     BARCODE,
                    //                                                                                                     IsPass,
                    //                                                                                                     m_input_time,
                    //                                                                                                     DateTime.Now,
                    //                                                                                                     VER,
                    //                                                                                                     ARTWORKNAME,
                    //                                                                                                     RELATECOLORSTR,
                    //                                                                                                     _listHivePicture);

                    JzHiveItemMessageClass _msghiveitem = new JzHiveItemMessageClass();

                    _msghiveitem.unit_sn = BARCODE;
                    _msghiveitem.serials = BARCODE;
                    _msghiveitem.ispass = IsPass;
                    _msghiveitem.input_time = m_input_time;
                    _msghiveitem.output_time = m_input_time.AddMilliseconds(Testms[0]);
                    _msghiveitem.eVer = VER;
                    _msghiveitem.eArtWorkName = ARTWORKNAME;
                    _msghiveitem.eColor = RELATECOLORSTR;

                    _msghiveitem.machineName = INI.MACHINENAME;
                    _msghiveitem.machineID = INI.MACHINENAMEID;
                    _msghiveitem.KBCountryCode = VER;
                    _msghiveitem.TestTime = Testms[0].ToString();

                    _msghiveitem.format01Head = m_HiveAppleFormat01Head;
                    _msghiveitem.format01Value = m_HiveAppleFormat01Value;

                    MACHINE.HIVECLIENT.Hiveclient_MachineData_Files(_msghiveitem, _listHivePicture);

                    //string strresult_filepath = "";//測試結果檔案路徑+文件名稱
                    //MACHINE.HIVECLIENT.Hiveclient_MachineData_Files(BARCODE, BARCODE, IsPass, strresult_filepath, m_input_time, DateTime.Now);
                }
                //if (IsPass)
                //    MACHINE.SetMachineState(MachineState.Running);
                //else
                    MACHINE.SetMachineState(MachineState.Idle);
            }

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

        }

        void FillOperaterString(string opstr)
        {
            foreach(EnvClass env in AlbumWork.ENVList)
            {
                env.PassInfo.OperateString = opstr;

                foreach(PageClass page in env.PageList)
                {
                    page.PassInfo.OperateString = opstr;

                    foreach(AnalyzeClass analyzeroot in page.AnalyzeRootArray)
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
                switch(INI.SFFACTORY)
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
            strReportMsg += VER + ",";
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

                        ////if(!m_CheckLaserSnError)
                        //{
                        //    if(analyze.OCRPara.OCRMethod == OCRMethodEnum.MAPPING)
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

        #region apple report formats

        /// <summary>
        /// 用於記錄重複測試SN記錄次數
        /// </summary>
        List<string> m_BarcodeCount = new List<string>();//用於記錄重複測試SN記錄次數
        bool m_IsInit = false;
        void ReadData(ref string DataStr, string FileName)
        {
            FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.None);
            StreamReader Srr = new StreamReader(fs, Encoding.Default);

            DataStr = Srr.ReadToEnd();

            Srr.Close();
            Srr.Dispose();
        }
        public void SaveData(string DataStr, string FileName, bool isappend)
        {
            StreamWriter Swr = new StreamWriter(FileName, isappend, System.Text.Encoding.UTF8);
            Swr.Write(DataStr);
            Swr.Flush();
            Swr.Close();
            Swr.Dispose();
        }
        private void _calBarcodeTestCount(string eBarcode, ref string eCount)
        {
            if (!m_IsInit)
            {
                m_IsInit = true;
                if (File.Exists(Application.StartupPath + "\\DailyReportSN.log.csv"))
                {
                    string strinit = "";
                    ReadData(ref strinit, Application.StartupPath + "\\DailyReportSN.log.csv");
                    string[] strsss = strinit.Replace(Environment.NewLine, "@").Split('@');
                    foreach (string strx in strsss)
                    {
                        if (strx != "")
                            m_BarcodeCount.Add(strx);
                    }
                }
            }

            eCount = "1";
            int k = -1;
            bool isdel = false;
            string strnew = eBarcode + ",1";
            foreach (string str in m_BarcodeCount)
            {
                string[] strs = str.Split(',');
                k++;
                if (strs[0] == eBarcode.Trim())
                {
                    isdel = true;
                    int value = int.Parse(strs[1]) + 1;
                    eCount = value.ToString();
                    strnew = strs[0] + "," + value.ToString();
                    break;
                }
            }

            if (isdel)
                m_BarcodeCount.RemoveAt(k);

            m_BarcodeCount.Add(strnew);

            if (m_BarcodeCount.Count >= 2800)
            {
                k = 0;
                while (k < 2700)
                {
                    m_BarcodeCount.RemoveAt(0);
                    k++;
                }
            }

            string strresult = "";
            foreach (string str in m_BarcodeCount)
            {
                strresult += str + Environment.NewLine;
            }
            SaveData(strresult, Application.StartupPath + "\\DailyReportSN.log.csv", false);
        }

        /*
         * SerialNumber	Date	Time	FixtureName	SNCount	Machine Cycle Time	ShopFloor(1=YES)	Reserved 1	Reserved 2	Reserved 3		
         * 1KV_KB_Version	
         * 2KD_KB_Defect	
         * 3LEV_LaserEtch_Version	
         * 3LESN	
         * 4LED_LaserEtch_Defect	
         * 5Sc1_Screw1_Color	5Sc2	5Sc3	5Sc4	5Sc5	5Sc6	
         * 5Sm1_Screw1_Missing	5Sm2	5Sm3	5Sm4	5Sm5	5Sm6	
         * 6Sh1_Screw1_Height	6Sh2	6Sh3	6Sh4	6Sh5	6Sh6	
         * PASS/FAIL
										USL(mm)	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	0.999	
										LSL(mm)	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	
            C02C4010MNHP	20200120	3	J152_AOI_D15_AOI_01	1	8	1					0	0	0	0	0	0	0	0	0	1	0	0	0	0	0	0	0	0	0	0	0	0	0	Fail

         */
        string m_HiveAppleFormat01Head = "";
        string m_HiveAppleFormat01Value = "";

        private void _saveAppleFormat01()
        {
            m_HiveAppleFormat01Head = "";
            m_HiveAppleFormat01Value = "";

            int _screwCount = 6;//判断螺丝的个数
            if (INI.DATA_SCREW_TEN)
                _screwCount = 10;

            string _barcodeTestCount = "1";
            _calBarcodeTestCount(BARCODE, ref _barcodeTestCount);

            string strApplePath = @"D:\ALLRESULTPIC\REPORTS\FORMAT01\";
            string strAppleFileName = JzTimes.DateSerialString + "_" + INI.DATA_FIXTUREID + ".csv";
            string strFullFileName = strApplePath + strAppleFileName;

            if (!System.IO.Directory.Exists(strApplePath))
                System.IO.Directory.CreateDirectory(strApplePath);

            string strReportMsg = "";
            string strHead = "SerialNumber,Date,Time,FixtureName,SNCount,Machine Cycle Time,ShopFloor(1=YES),Reserved 1,Reserved 2,Reserved 3,,";

            switch (INI.HIVE_model)
            {
                case "J293":
                case "J313":

                    strHead = "SerialNumber,Date,Time,FixtureName,SNCount,Machine_Cycle_Time,ShopFloor,Reserved 1,Reserved 2,Reserved 3,,";

                    break;
            }

            if (INI.DATA_SCREW_TEN)
            {
                //H0 标注 为了定义变量 H0[0]=1KV_KB_Vision ...
                strHead += "1KV_KB_Version,2KD_KB_Defect,3LEV_LaserEtch_Version,3LESN,4LED_LaserEtch_Defect,";
                strHead += "5Sc1_Screw1_Color,5Sc2,5Sc3,5Sc4,5Sc5,5Sc6,5Sc7,5Sc8,5Sc9,5Sc10,";//H1 标注 为了定义变量
                strHead += "5Sm1_Screw1_Missing,5Sm2,5Sm3,5Sm4,5Sm5,5Sm6,5Sm7,5Sm8,5Sm9,5Sm10,";//H2 标注 为了定义变量
                strHead += "6Sh1_Screw1_Height,6Sh2,6Sh3,6Sh4,6Sh5,6Sh6,6Sh7,6Sh8,6Sh9,6Sh10,";//H3 标注 为了定义变量
                strHead += "PASS/FAIL," + Environment.NewLine;

                strHead += ",,,,,,,,,,USL(mm),0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999," + Environment.NewLine;
                strHead += ",,,,,,,,,,LSL(mm),0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0," + Environment.NewLine;
            }
            else
            {

                //H0 标注 为了定义变量 H0[0]=1KV_KB_Vision ...
                strHead += "1KV_KB_Version,2KD_KB_Defect,3LEV_LaserEtch_Version,3LESN,4LED_LaserEtch_Defect,";
                strHead += "5Sc1_Screw1_Color,5Sc2,5Sc3,5Sc4,5Sc5,5Sc6,";//H1 标注 为了定义变量
                strHead += "5Sm1_Screw1_Missing,5Sm2,5Sm3,5Sm4,5Sm5,5Sm6,";//H2 标注 为了定义变量
                strHead += "6Sh1_Screw1_Height,6Sh2,6Sh3,6Sh4,6Sh5,6Sh6,";//H3 标注 为了定义变量
                strHead += "PASS/FAIL," + Environment.NewLine;

                strHead += ",,,,,,,,,,USL(mm),0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999,0.999," + Environment.NewLine;
                strHead += ",,,,,,,,,,LSL(mm),0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0," + Environment.NewLine;
            }

            if (!System.IO.File.Exists(strFullFileName))
                strReportMsg += strHead;

            strReportMsg += BARCODE + ",";
            strReportMsg += JzTimes.DateSerialString + ",";
            strReportMsg += m_input_time.AddMilliseconds(Testms[0]).ToString("HH:mm:ss") + ",";
            strReportMsg += INI.DATA_FIXTUREID + ",";
            strReportMsg += _barcodeTestCount + ",";
            strReportMsg += Testms[0].ToString() + ",";
            strReportMsg += "1" + ",,,,,";

            int[] _H0 = new int[5];
            int[] _H1 = new int[_screwCount];
            int[] _H2 = new int[_screwCount];
            int[] _H3 = new int[_screwCount];

            _H0[0] = 0;
            _H0[1] = 0;
            _H0[2] = 0;
            _H0[3] = 0;
            _H0[4] = 0;

            #region 结果组合

            int i = 0;
            int j = 0;
            int k = 0;

            i = 0;
            while (i < _screwCount)
            {
                _H1[i] = 0;
                _H2[i] = 0;
                _H3[i] = 0;
                i++;
            }

            foreach (PageClass page in AlbumWork.ENVList[0].PageList)
            {
                AnalyzeClass analyzeroot = page.AnalyzeRoot;

                switch (page.PageRunNo)
                {
                    case 0:

                        foreach (AnalyzeClass analyze in analyzeroot.BranchList)
                        {
                            if (analyze.RunStatusCollection.COUNT > 0)
                            {
                                if (!IsPass)
                                {
                                    if (analyze.RunStatusCollection.NGCOUNT > 0)
                                    {
                                        j = 0;
                                        while (j < analyze.RunStatusCollection.WorkStatusList.Count)
                                        {
                                            switch (analyze.RunStatusCollection.WorkStatusList[j].AnalyzeProcedure)
                                            {
                                                case AnanlyzeProcedureEnum.MEASURE:

                                                    if (analyze.RunStatusCollection.WorkStatusList[j].Reason != ReasonEnum.PASS)
                                                    {
                                                        if (analyze.BranchList.Count > 0)
                                                        {
                                                            k = 1;
                                                            while (k < _screwCount + 1)
                                                            {
                                                                if (analyze.BranchList[0].RelateASNItem.IndexOf("S" + k.ToString() + "(") > -1)
                                                                {
                                                                    _H1[k - 1] = 1;
                                                                }
                                                                k++;
                                                            }
                                                        }
                                                    }

                                                    break;
                                                default:

                                                    if (analyze.RunStatusCollection.WorkStatusList[j].Reason != ReasonEnum.PASS)
                                                    {
                                                        if (analyze.BranchList.Count > 0)
                                                        {
                                                            k = 1;
                                                            while (k < _screwCount + 1)
                                                            {
                                                                if (analyze.BranchList[0].RelateASNItem.IndexOf("S" + k.ToString() + "(") > -1)
                                                                {
                                                                    _H2[k - 1] = 1;
                                                                }
                                                                k++;
                                                            }
                                                        }
                                                    }

                                                    break;
                                            }
                                            j++;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                i = 0;
                                while (i < analyze.BranchList.Count)
                                {
                                    if (analyze.BranchList[i].RunStatusCollection.COUNT > 0)
                                    {
                                        if (!IsPass)
                                        {
                                            if (analyze.BranchList[i].RunStatusCollection.NGCOUNT > 0)
                                            {
                                                j = 0;
                                                while (j < analyze.BranchList[i].RunStatusCollection.WorkStatusList.Count)
                                                {
                                                    switch (analyze.BranchList[i].RunStatusCollection.WorkStatusList[j].AnalyzeProcedure)
                                                    {
                                                        case AnanlyzeProcedureEnum.MEASURE:

                                                            if (analyze.BranchList[i].RunStatusCollection.WorkStatusList[j].Reason != ReasonEnum.PASS)
                                                            {
                                                                k = 1;
                                                                while (k < _screwCount + 1)
                                                                {
                                                                    if (analyze.BranchList[i].RelateASNItem.IndexOf("S" + k.ToString() + "(") > -1)
                                                                    {
                                                                        _H1[k - 1] = 1;
                                                                    }
                                                                    k++;
                                                                }

                                                            }

                                                            break;
                                                        default:

                                                            if (analyze.BranchList[i].RunStatusCollection.WorkStatusList[j].Reason != ReasonEnum.PASS)
                                                            {

                                                                k = 1;
                                                                while (k < _screwCount + 1)
                                                                {
                                                                    if (analyze.BranchList[i].RelateASNItem.IndexOf("S" + k.ToString() + "(") > -1)
                                                                    {
                                                                        _H2[k - 1] = 1;
                                                                    }
                                                                    k++;
                                                                }
                                                            }

                                                            break;
                                                    }
                                                    j++;
                                                }
                                            }
                                        }
                                    }
                                    i++;
                                }
                            }
                        }


                        break;
                    case 1:

                        //if (_H0[4] == 0)
                        {
                            foreach (AnalyzeClass analyze in analyzeroot.BranchList)
                            {
                                if (analyze.RunStatusCollection.COUNT > 0)
                                {
                                    if (!IsPass)
                                    {
                                        if (analyze.RunStatusCollection.NGCOUNT > 0)
                                        {
                                            j = 0;
                                            while (j < analyze.RunStatusCollection.WorkStatusList.Count)
                                            {
                                                switch (analyze.RunStatusCollection.WorkStatusList[j].AnalyzeProcedure)
                                                {
                                                    case AnanlyzeProcedureEnum.CHECKOCR:

                                                        if (analyze.RunStatusCollection.WorkStatusList[j].Reason != ReasonEnum.PASS)
                                                        {
                                                            _H0[3] = 1;
                                                            //break;
                                                        }

                                                        break;
                                                    default:

                                                        if (analyze.RunStatusCollection.WorkStatusList[j].Reason != ReasonEnum.PASS)
                                                        {
                                                            _H0[4] = 1;
                                                            //break;
                                                        }

                                                        break;
                                                }
                                                j++;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    i = 0;
                                    while (i < analyze.BranchList.Count)
                                    {
                                        if (analyze.BranchList[i].RunStatusCollection.COUNT > 0)
                                        {
                                            if (!IsPass)
                                            {
                                                if (analyze.BranchList[i].RunStatusCollection.NGCOUNT > 0)
                                                {
                                                    j = 0;
                                                    while (j < analyze.BranchList[i].RunStatusCollection.WorkStatusList.Count)
                                                    {
                                                        switch (analyze.BranchList[i].RunStatusCollection.WorkStatusList[j].AnalyzeProcedure)
                                                        {
                                                            case AnanlyzeProcedureEnum.CHECKOCR:

                                                                if (analyze.BranchList[i].RunStatusCollection.WorkStatusList[j].Reason != ReasonEnum.PASS)
                                                                {
                                                                    _H0[3] = 1;
                                                                    //break;
                                                                }

                                                                break;
                                                            default:

                                                                if (analyze.BranchList[i].RunStatusCollection.WorkStatusList[j].Reason != ReasonEnum.PASS)
                                                                {
                                                                    _H0[4] = 1;
                                                                    //break;
                                                                }

                                                                break;
                                                        }
                                                        j++;
                                                    }
                                                }
                                            }
                                        }
                                        i++;
                                    }
                                }
                            }
                        }

                        break;
                    case 2:
                    case 3:

                        if (_H0[1] == 0)
                        {
                            foreach (AnalyzeClass analyze in analyzeroot.BranchList)
                            {
                                if (analyze.RunStatusCollection.COUNT > 0)
                                {
                                    if (!IsPass)
                                    {
                                        if (analyze.RunStatusCollection.NGCOUNT > 0)
                                        {
                                            _H0[1] = 1;//发现键盘错误时 跳出
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    i = 0;
                                    while (i < analyze.BranchList.Count)
                                    {
                                        if (analyze.BranchList[i].RunStatusCollection.COUNT > 0)
                                        {
                                            if (!IsPass)
                                            {
                                                if (analyze.BranchList[i].RunStatusCollection.NGCOUNT > 0)
                                                {
                                                    _H0[1] = 1;//发现键盘错误时 跳出
                                                    break;
                                                }
                                            }
                                        }
                                        i++;
                                    }
                                }
                            }
                        }

                        break;
                }
            }

            #endregion

            strReportMsg += _H0[0] + "," + _H0[1] + "," + _H0[2] + "," + _H0[3] + "," + _H0[4] + ",";
            //strReportMsg += _H1[0] + "," + _H1[1] + "," + _H1[2] + "," + _H1[3] + "," + _H1[4] + "," + _H1[5] + ",";
            //strReportMsg += _H2[0] + "," + _H2[1] + "," + _H2[2] + "," + _H2[3] + "," + _H2[4] + "," + _H2[5] + ",";
            //strReportMsg += _H3[0] + "," + _H3[1] + "," + _H3[2] + "," + _H3[3] + "," + _H3[4] + "," + _H3[5] + ",";

            i = 0;
            while (i < _screwCount)
            {
                strReportMsg += _H1[i] + ",";
                i++;
            }
            i = 0;
            while (i < _screwCount)
            {
                strReportMsg += _H2[i] + ",";
                i++;
            }
            i = 0;
            while (i < _screwCount)
            {
                strReportMsg += _H3[i] + ",";
                i++;
            }

            strReportMsg += (IsPass ? "PASS" : "FAIL") + "," + Environment.NewLine;

            m_HiveAppleFormat01Head = strHead;
            //string[] _valuestemp = strReportMsg.Replace(Environment.NewLine, "@").Split('@');
            if (!System.IO.File.Exists(strFullFileName))
                m_HiveAppleFormat01Value = strReportMsg.Replace(Environment.NewLine, "@").Split('@')[3];
            else
                m_HiveAppleFormat01Value = strReportMsg;

            JzTools.SaveDataEX(strReportMsg, strFullFileName);
        }

        #endregion

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

            //Create by Gaara
            Path_Hive_Pictures = AllSavePath;
            //_saveSkynetSingleReports();
            _saveAppleFormat01();

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
        private void _saveSkynetSingleReports()
        {
            string DatePath = INI.ALLRESULTPIC + "\\Reports\\" + JzTimes.DateSerialString;
            string _SingleReportName = "Single_";

            if (!Directory.Exists(DatePath))
                Directory.CreateDirectory(DatePath);

            _SingleReportName += (IsPass ? "P-" : "F-");
            _SingleReportName += JzTimes.DateTimeSerialString + "_";
            _SingleReportName += INI.DATA_FIXTUREID + "_";
            _SingleReportName += (string.IsNullOrEmpty(BARCODE) ? "Null_SN" : BARCODE) + "_";
            _SingleReportName += VER + "_";
            _SingleReportName += ARTWORKNAME + "_";
            _SingleReportName += RELATECOLORSTR;

            List<string> RegionReportList = new List<string>();

            int i = 0;
            foreach (PageClass page in AlbumWork.ENVList[0].PageList)
            {
                AnalyzeClass analyzeroot = page.AnalyzeRoot;
                foreach (AnalyzeClass analyze in analyzeroot.BranchList)
                {
                    if (analyze.RunStatusCollection.COUNT > 0)
                    {
                        string str = analyze.AliasName + "," + analyze.RunStatusCollection.GetRunStatus(0).Reason.ToString();
                        if (!IsPass)
                        {
                            if (analyze.RunStatusCollection.NGCOUNT > 0)
                            {
                                str = analyze.AliasName + "," + analyze.RunStatusCollection.GetNGRunStatus(0).Reason.ToString();
                            }
                        }
                        RegionReportList.Add(str);
                    }
                    else
                    {
                        i = 0;
                        while (i < analyze.BranchList.Count)
                        {
                            if (analyze.BranchList[i].RunStatusCollection.COUNT > 0)
                            {
                                string str = analyze.AliasName + "," + analyze.BranchList[i].RunStatusCollection.GetRunStatus(0).Reason.ToString();
                                if (!IsPass)
                                {
                                    if (analyze.BranchList[i].RunStatusCollection.NGCOUNT > 0)
                                    {
                                        str = analyze.AliasName + "," + analyze.BranchList[i].RunStatusCollection.GetNGRunStatus(0).Reason.ToString();
                                    }
                                }
                                RegionReportList.Add(str);
                            }
                            i++;
                        }
                    }
                }
            }

            RegionReportList.Sort();
            Newtonsoft.Json.Linq.JObject jb0 = new Newtonsoft.Json.Linq.JObject();

            i = 0;
            foreach (string str in RegionReportList)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    string[] tmp = str.Split(',');
                    if (tmp.Length >= 2)
                    {
                        if (!string.IsNullOrEmpty(tmp[0]))
                        {
                            jb0.Add(str.Split(',')[0] + "ex" + i.ToString(), str.Split(',')[1]);
                        }
                        else
                        {
                            jb0.Add("Un" + i.ToString(), str.Split(',')[1]);
                        }
                    }
                }
                i++;
            }

            string strjson = jb0.ToString(Formatting.Indented, null);

            FullPathName_Hive_Reports = DatePath + "\\" + _SingleReportName + ".csv";
            SaveData(strjson, FullPathName_Hive_Reports);//保存本地數據

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
                Directory.CreateDirectory(strpath);
            m.Save(strpath + "\\" + (string.IsNullOrEmpty(BARCODE) ? JzTimes.TimeSerialString : BARCODE) + ".jpg", ImageFormat.Jpeg);
            m.Dispose();
        }

        #endregion
     
    }
}
