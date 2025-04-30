using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using JetEazy;
using JetEazy.ControlSpace;
using JetEazy.BasicSpace;

using Allinone.OPSpace.ResultSpace;
using AllinOne.Jumbo.Net;
using AllinOne.Jumbo.Net.Common;

using Allinone.Crystal.Net;
using Allinone.Crystal.Net.Common;
using JetEazy.Interface;

namespace Allinone.OPSpace
{
    public class ResultClass
    {
        public TestMethodEnum TestMethod = TestMethodEnum.BUTTON;

        bool IsNoUseCCD
        {
            get
            {
                return Universal.IsNoUseCCD;
            }
        }
        VersionEnum VERSION
        {
            get
            {
                return Universal.VERSION;
            }
        }
        OptionEnum OPTION
        {
            get
            {
                return Universal.OPTION;
            }
        }
        CameraActionMode CAMACT
        {
            get
            {
                return Universal.CAMACT;
            }
        }
        AlbumCollectionClass ALBCollection
        {
            get
            {
                return Universal.ALBCollection;
            }
        }
        AlbumClass AlbumWork
        {
            get
            {
                return ALBCollection.AlbumWork;
            }
        }
        AlbumClass AlbumWorkNow
        {
            get
            {
                return ALBCollection.AlbumNow;
            }
        }
        CCDCollectionClass CCDCollection
        {
            get
            {
                return Universal.CCDCollection;
            }
        }
        IxLineScanCam IxLINESCANCAMERA
        {
            get
            {
                return Universal.IxLineScan;
            }
        }
        public WorkStatusCollectionClass RunstatusCollection
        {
            get
            {
                return myResult.RunStatusCollection;
            }
        }

        /// <summary>
        /// 連接至鍵高服務器
        /// </summary>
        IxConnectJumbo301 IXCONNECTJUMBO
        {
            get { return Universal.IXCONNECTJUMBO; }
        }
        /// <summary>
        /// 服務器向客戶端發送信息
        /// </summary>
        AllinoneEvent JUMBOSERVEREVENT
        {
            get
            {
                return Universal.JUMBOSERVEREVENT;
            }
        }

        /// <summary>
        /// 連接至量測服務器
        /// </summary>
        IxConnectCrystal501 IXCONNECTCRYSTAL
        {
            get { return Universal.IXCONNECTCRYSTAL; }
        }
        /// <summary>
        /// 量測服務器向客戶端發送信息
        /// </summary>
        AllinoneCrystalEvent CRYSTALSERVEREVENT
        {
            get
            {
                return Universal.CRYSTALSERVEREVENT;
            }
        }

        string SavePath = "";

        public GeoResultClass myResult;

        /// <summary>
        /// 是否要停止 QSMCSFTICK，算是 Normal Tick
        /// </summary>
        public bool IsStopNormalTick
        {
            get
            {
                return myResult.IsStopNormalTick;
            }
            set
            {
                myResult.IsStopNormalTick = value;
            }
        }

        public bool ReStartJumboBroadCast()
        {
            if (JUMBOSERVEREVENT != null && IXCONNECTJUMBO != null && INI.ISUSEJUMBOSERVER)
            {
                //JUMBOSERVEREVENT.LocalBroadCastEvent += JUMBOSERVEREVENT_LocalBroadCastEvent;
                try
                {
                    IXCONNECTJUMBO.BroadCastEvent += new AllinOne.Jumbo.Net.BroadCastEventHandler(JUMBOSERVEREVENT.BroadCasting);
                    return true;
                }
                catch (Exception ex)
                {
                    JetEazy.LoggerClass.Instance.WriteException(ex);
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return false;
                }
            }
            return false;
        }
        public bool ReStartCrystalBroadCast()
        {
            if (CRYSTALSERVEREVENT != null && IXCONNECTCRYSTAL != null && INI.ISUSECRYSTALSERVER)
            {
                //CRYSTALSERVEREVENT.LocalBroadCastEvent += CRYSTALSERVEREVENT_LocalBroadCastEvent;
                try
                {
                    IXCONNECTCRYSTAL.BroadCastEvent += new Allinone.Crystal.Net.BroadCastEventHandler(CRYSTALSERVEREVENT.BroadCasting);
                    return true;
                }
                catch (Exception ex)
                {
                    JetEazy.LoggerClass.Instance.WriteException(ex);
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return false;
                }
            }
            return false;
        }

        public ResultClass(GeoResultClass result)
        {
            myResult = result;
            myResult.TriggerAction += MyResult_TriggerAction;
            myResult.EnvTriggerAction += MyResult_EnvTriggerAction;
            myResult.TriggerOPAction += MyResult_TriggerOPAction;
            myResult.TriggerShowImageCurrent += MyResult_TriggerShowImageCurrent;

            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN:
                            if (JUMBOSERVEREVENT != null && IXCONNECTJUMBO != null && INI.ISUSEJUMBOSERVER)
                            {
                                JUMBOSERVEREVENT.LocalBroadCastEvent += JUMBOSERVEREVENT_LocalBroadCastEvent;
                                try
                                {
                                    IXCONNECTJUMBO.BroadCastEvent += new AllinOne.Jumbo.Net.BroadCastEventHandler(JUMBOSERVEREVENT.BroadCasting);
                                }
                                catch (Exception ex)
                                {
                                    JetEazy.LoggerClass.Instance.WriteException(ex);
                                    System.Diagnostics.Debug.WriteLine(ex.Message);
                                }
                            }

                            if (CRYSTALSERVEREVENT != null && IXCONNECTCRYSTAL != null && INI.ISUSECRYSTALSERVER)
                            {
                                CRYSTALSERVEREVENT.LocalBroadCastEvent += CRYSTALSERVEREVENT_LocalBroadCastEvent;
                                try
                                {
                                    IXCONNECTCRYSTAL.BroadCastEvent += new Allinone.Crystal.Net.BroadCastEventHandler(CRYSTALSERVEREVENT.BroadCasting);
                                }
                                catch (Exception ex)
                                {
                                    JetEazy.LoggerClass.Instance.WriteException(ex);
                                    System.Diagnostics.Debug.WriteLine(ex.Message);
                                }
                            }
                            break;
                        default:

                            break;
                    }

                    break;
            }

            myResult.TriggerOPMess += MyResult_TriggerOPMess;
        }

        private void MyResult_TriggerShowImageCurrent(Bitmap ebmpInput)
        {
            OnTriggerShowImageCurrent(ebmpInput);
        }

        private void MyResult_TriggerOPMess(string str)
        {
            OnTriggerMess(str);
        }

        private void CRYSTALSERVEREVENT_LocalBroadCastEvent(string info)
        {
            if (System.Windows.Forms.MessageBox.Show("是否要執行外部抓圖作業?", "SYSTEM.Crystal", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }

            switch (info)
            {
                case "Crystal_Reget"://取像
                    StartCalibrate("Crystal_Reget");
                    break;
                case "Crystal_Online"://TEST
                    StartCalibrate("Crystal_Online");
                    break;
            }
        }

        private void MyResult_TriggerOPAction(ResultStatusEnum resultstatus, string str)
        {
            OnTriggerOP(resultstatus, str);
        }

        private void JUMBOSERVEREVENT_LocalBroadCastEvent(string info)
        {
            if (System.Windows.Forms.MessageBox.Show("是否要執行外部抓圖作業?", "SYSTEM.Jumbo", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }

            switch (info)
            {
                case "0"://取像
                    StartCalibrate("0");
                    break;
                case "1"://解析
                    StartCalibrate("1");
                    break;
                case "2"://TEST
                    StartCalibrate("2");
                    break;
            }
        }
        private void MyResult_EnvTriggerAction(ResultStatusEnum resultstatus, int envindex, string operpagestr)
        {
            switch (resultstatus)
            {
                case ResultStatusEnum.CHANGEDIRECTORY:
                    if (IsNoUseCCD || Universal.IsNoUseIO || Universal.IsLocalPicture)
                    {
                        string testpath = myResult.GetDebugDirectory();

                        OnEnvTrigger(resultstatus, envindex, testpath);
                    }
                    break;
                case ResultStatusEnum.CHANGEENVDIRECTORY:
                    if (IsNoUseCCD || Universal.IsNoUseIO || Universal.IsLocalPicture)
                    {
                        CCDCollection.SetDebugPath(myResult.LastDirPath);
                        CCDCollection.SetDebugEnvPath(envindex.ToString("000"));

                        CCDCollection.SetPageOPType(operpagestr);
                    }
                    break;
                default:
                    OnEnvTrigger(resultstatus, envindex, operpagestr);
                    break;
            }
        }
        private void MyResult_TriggerAction(ResultStatusEnum resultstatus)
        {
            OnTrigger(resultstatus);
        }
        public void RefreshDebugSrcDirectory(string debugsrcpath)
        {
            myResult.RefreshDebugSrcDirectory(debugsrcpath);
        }
        void GenSavePath()
        {
            SavePath = JzTimes.DateTimeSerialString;
        }
        public int GetDirsCount()
        {
            return myResult.DirsCount;
        }
        public void SaveRUNBMP(int envindex, string savestr)
        {
            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                case VersionEnum.AUDIX:
                    myResult.SetDebugRecipeSaveName(AlbumWork.RelateRCP.RcpNoString);
                    myResult.SaveImage(SavePath, AlbumWork.ENVList[envindex], savestr, PageOPTypeEnum.P00);
                    break;
                default:
                    myResult.SetDebugRecipeSaveName(AlbumWork.RelateRCP.RcpNoString);
                    myResult.SaveImage(SavePath, AlbumWork.ENVList[envindex], savestr, PageOPTypeEnum.P00);
                    break;
            }
        }
        public void SaveDebugBMP(int envindex, string savestr)
        {
            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                case VersionEnum.AUDIX:
                    myResult.SetDebugRecipeSaveName(AlbumWork.RelateRCP.RcpNoString);
                    myResult.SaveDebugImage(SavePath, AlbumWork.ENVList[envindex], savestr, PageOPTypeEnum.P00);
                    break;
            }
        }
        public void SaveHEIGHTBMP(int envindex, string savestr)
        {
            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    myResult.SetDebugRecipeSaveName(AlbumWork.RelateRCP.RcpNoString);
                    ((JzAllInoneResultClass)myResult).SaveHeightImage(SavePath, AlbumWork.ENVList[envindex], savestr, PageOPTypeEnum.P00);
                    break;
                default:
                    myResult.SetDebugRecipeSaveName(AlbumWork.RelateRCP.RcpNoString);
                    myResult.SaveImage(SavePath, AlbumWork.ENVList[envindex], savestr, PageOPTypeEnum.P00);
                    break;
            }
        }

        public void StartCalibrate(string strTest)
        {
            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    ((JzAllInoneResultClass)myResult).StartCalibration(ALBCollection.AlbumNow, CCDCollection, strTest);
                    break;
            }
        }

        /// <summary>
        /// Used By Dragon Fly
        /// </summary>
        /// <param name="positionstr"></param>
        /// <param name="ismicroscope"></param>
        public void StartCaptureOnce(string positionstr, bool ismicroscope)
        {
            switch (VERSION)
            {
                case VersionEnum.AUDIX:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_DFLY:
                            ((JzAudixDflyResultClass)myResult).StartCaputreOnce(positionstr, ismicroscope);
                            break;
                    }
                    break;
            }
        }

        public bool GetStartCaptureOnceStatus()
        {
            return ((JzAudixDflyResultClass)myResult).CaptureOnceProcess.IsOn;
        }


        public void StartCaptureTest(string str, bool ismicroscope)
        {
            switch (VERSION)
            {
                case VersionEnum.AUDIX:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_DFLY:
                            ((JzAudixDflyResultClass)myResult).StartCaptrueTest(str, ismicroscope);
                            break;
                    }
                    break;
            }
        }

        public void Tick()
        {
            myResult.Tick();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isdirect"></param>
        public void Calculate()
        {
            GenSavePath();
            switch(Universal.OPTION)
            {
                case OptionEnum.MAIN_X6:
                    myResult.GetStart(AlbumWorkNow, CCDCollection, TestMethod, IsNoUseCCD);
                    break;
                default:
                    myResult.GetStart(AlbumWork, CCDCollection, TestMethod, IsNoUseCCD);
                    break;
            }
            

            //switch(CAMACT)
            //{
            //    case CameraActionMode.CAM_MOTOR_LINESCAN:

            //        myResult.AddLinescanCamera(IxLINESCANCAMERA);

            //        break;
            //}
        }
        public void CalculateSMD2()
        {
            GenSavePath();
            myResult.GetStart(AlbumWorkNow, CCDCollection, TestMethod, IsNoUseCCD);
        }
        public delegate void TriggerHandler(ResultStatusEnum resultstatus);
        public event TriggerHandler TriggerAction;
        public void OnTrigger(ResultStatusEnum resultstatus)
        {
            if (TriggerAction != null)
            {
                TriggerAction(resultstatus);
            }
        }

        public delegate void TriggerOPHandler(ResultStatusEnum resultstatus, string str);
        public event TriggerOPHandler TriggerOPAction;
        public void OnTriggerOP(ResultStatusEnum resultstatus, string str)
        {
            if (TriggerOPAction != null)
            {
                TriggerOPAction(resultstatus, str);
            }
        }

        public delegate void EnvTriggerHandler(ResultStatusEnum resultstatus, int envindex, string operpagestr);
        public event EnvTriggerHandler EnvTriggerAction;
        public void OnEnvTrigger(ResultStatusEnum resultstatus, int envindex, string operpagestr)
        {
            if (EnvTriggerAction != null)
            {
                EnvTriggerAction(resultstatus, envindex, operpagestr);
            }
        }


        public delegate void TriggerOPMessIng(string str);
        public event TriggerOPMessIng TriggerOPMess;
        public void OnTriggerMess(string str)
        {
            if (TriggerOPMess != null)
            {
                TriggerOPMess(str);
            }
        }

        public delegate void TriggerShowImage(Bitmap ebmpInput);
        public event TriggerShowImage TriggerShowImageCurrent;
        public void OnTriggerShowImageCurrent(Bitmap ebmpInput)
        {
            if (TriggerShowImageCurrent != null)
            {
                OnTriggerShowImageCurrent(ebmpInput);
            }
        }
    }
}
