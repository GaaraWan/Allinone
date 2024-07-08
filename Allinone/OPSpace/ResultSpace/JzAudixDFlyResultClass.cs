using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;

using JetEazy;
using JetEazy.BasicSpace;
using JetEazy.ControlSpace;
using Allinone.ControlSpace.MachineSpace;
using Allinone.ControlSpace;

using Allinone.UISpace.ALBUISpace;

namespace Allinone.OPSpace.ResultSpace
{
    class JzAudixDflyResultClass : GeoResultClass
    {
        JzDFlyMachineClass MACHINE;

        bool IsMicroScope = false;
        bool IsJustRunTest = false;

        int CellCount = 0;
        int CellStartIndex = 0;
        int CellIndex = 0;
        int PageIndex = 0;

        int XDirCount = 0;
        int YDirCount = 0;
        int XDirIndex = 0;
        int YDirIndex = 0;

        string XDirPos = "";
        string YDirPos = "";

        float XDirOffset = 0f;
        float YDirOffset = 0f;

        PositionSettings PositionSetting = new PositionSettings();
        public ProcessClass CaptureOnceProcess = new ProcessClass();

        public JzAudixDflyResultClass(Result_EA resultea,VersionEnum version,OptionEnum option,MachineCollectionClass machinecollection)
        {
            myResultEA = resultea;
            VERSION = version;
            OPTION = option;

            MACHINE = (JzDFlyMachineClass)machinecollection.MACHINE;
            
            MainProcess = new ProcessClass();

        }
        private void CCDCollection_TriggerAction(string operationstr)
        {
            
        }

        public void StartCaputreOnce(string str,bool ismicroscope)
        {
            if(CaptureOnceProcess.IsOn)
            {
                CaptureOnceProcess.Stop();
            }
            else
            {
                IsMicroScope = ismicroscope;

                XDirIndex = 0;
                YDirIndex = 0;
                
                PositionSetting.FromString(str);

                CellCount = GetCellCount(PositionSetting, ref CellStartIndex);

                CaptureOnceProcess.Start();
            }
        }

        
        public void StartCaptrueTest(string str,bool ismicroscope)
        {
            if (MainProcess.IsOn)
            {
                IsMicroScope = ismicroscope;
                IsJustRunTest = true;

                MainProcess.Stop();
            }
            else
            {
                IsMicroScope = ismicroscope;

                IsJustRunTest = true;

                XDirIndex = 0;
                YDirIndex = 0;

                PositionSetting.FromString(str);

                CellCount = GetCellCount(PositionSetting, ref CellStartIndex);
                
                MainProcess.Start();
            }

        }

        public override void GetStart(AlbumClass albumwork,CCDCollectionClass ccdcollection,TestMethodEnum testmethod, bool isnouseccd)
        {
            if(MainProcess.IsOn)
            {
                MainProcess.Stop();
                OnTrigger(ResultStatusEnum.FORECEEND);
                
                return;
            }

            OnTrigger(ResultStatusEnum.CALSTART);

            AlbumWork = albumwork;
            CCDCollection = ccdcollection;

            TestMethod = testmethod;
            IsNoUseCCD = isnouseccd;

            ResetData(-1);

            IsMicroScope = false;

            IsJustRunTest = false;

            XDirIndex = 0;
            YDirIndex = 0;

            PositionSetting.FromString(AlbumWork.GetEnvByIndex(0).GeneralPosition);

            CellCount = GetCellCount(PositionSetting, ref CellStartIndex);

            MainProcess.Start();

        }
        public override void Tick()
        {
            MainProcessTick();
            CaptureOnceTick();

        }
        public override void GenReport()
        {

        }
        public override void SetDelayTime()
        {
            DelayTime[0] = INI.DELAYTIME;
        }

        protected override void MainProcessTick()
        {
            ProcessClass Process = MainProcess;

            if(Process.IsOn)
            {
                switch(Process.ID)
                {
                    case 5:
                        
                        OnEnvTrigger(ResultStatusEnum.CHANGEDIRECTORY,0,"");

                        if(!IsJustRunTest)
                            OnEnvTrigger(ResultStatusEnum.LOGPROCESS, -1, "Start Process.");

                        Process.NextDuriation = 50;

                        IsPass = true;
                        IsCaptureOnceReverse = false;

                        //if (IsSimulator)
                        //    Process.ID = 20;
                        //else
                        //    Process.ID = 10;

                        Process.ID = 10;

                        break;
                    case 10:                        //變換CCD亮度設定及光源設定
                        if (Process.IsTimeup)
                        {
                            //OnEnvTrigger(ResultStatusEnum.SETCAMLIGHT, EnvIndex, "ALL");
                            OnTrigger(ResultStatusEnum.COUNTSTART);

                            CellIndex = 0;
                            CaptureOnceProcess.Start();

                            if (!IsJustRunTest)
                                OnEnvTrigger(ResultStatusEnum.LOGPROCESS, -1, "Capturing X: " + 
                                            (!IsCaptureOnceReverse ? XDirIndex.ToString("000") : (XDirCount - XDirIndex - 1).ToString("000")) 
                                            + ", Y:" + YDirIndex.ToString("000"));

                            Process.NextDuriation = DelayTime[0];
                            Process.ID = 20;
                        }
                        break;
                    case 20:
                        if (Process.IsTimeup)       //抓圖
                        {
                            if (!CaptureOnceProcess.IsOn)
                            {
                                if (!IsJustRunTest)
                                {
                                    AlbumWork.A08_RunProcess(PageOPTypeEnum.P00);
                                    AlbumWork.FillRunStatus(RunStatusCollection);
                                    IsPass &= RunStatusCollection.NGCOUNT == 0;
                                }

                                OnEnvTrigger(ResultStatusEnum.SAVEDEBUGRAW, EnvIndex, "-1:" + (!IsCaptureOnceReverse ? XDirIndex.ToString("000") : (XDirCount - XDirIndex -1).ToString("000")) + "_" + YDirIndex.ToString("000"));

                                XDirIndex++;

                                if (XDirIndex < XDirCount)
                                {
                                    CaptureOnceProcess.Start();

                                    if (!IsJustRunTest)
                                        OnEnvTrigger(ResultStatusEnum.LOGPROCESS, -1, "Capturing X: " +
                                                    (!IsCaptureOnceReverse ? XDirIndex.ToString("000") : (XDirCount - XDirIndex - 1).ToString("000"))
                                                    + ", Y:" + YDirIndex.ToString("000"));

                                    Process.NextDuriation = 200;
                                    Process.ID = 20;

                                    return;
                                }
                                else
                                {
                                    YDirIndex++;
                                    XDirIndex = 0;

                                    IsCaptureOnceReverse = ((YDirIndex % 2) == 0 ? false : true);

                                    if (YDirIndex < YDirCount)
                                    {
                                        CaptureOnceProcess.Start();

                                        if (!IsJustRunTest)
                                            OnEnvTrigger(ResultStatusEnum.LOGPROCESS, -1, "Capturing X: " +
                                                        (!IsCaptureOnceReverse ? XDirIndex.ToString("000") : (XDirCount - XDirIndex - 1).ToString("000"))
                                                        + ", Y:" + YDirIndex.ToString("000"));

                                        Process.NextDuriation = 200;
                                        Process.ID = 20;

                                        return;
                                    }
                                }

                                Process.NextDuriation = 50;
                                Process.ID = 40;
                            }
                        }
                        break;
                    case 40:
                        if (!IsJustRunTest)
                        {
                            if (IsPass)
                            {
                                //MACHINE.SetPass(true);
                                OnTrigger(ResultStatusEnum.CALPASS);
                            }
                            else
                            {
                                //MACHINE.SetPass(false);
                                OnTrigger(ResultStatusEnum.CALNG);
                            }

                            OnTrigger(ResultStatusEnum.COUNTEND);
                            OnTrigger(ResultStatusEnum.CALEND);
                        }

                        //CellIndex = CellStartIndex;
                        //PageIndex = 0;

                        //MACHINE.GoPosition(PositionSetting.POS[CellIndex]);
                        CellIndex = CellStartIndex;
                        XDirIndex = 0;
                        YDirIndex = 0;

                        MACHINE.GoPosition(CombinPosition(PositionSetting.POS[CellIndex], IsMicroScope, XDirIndex, YDirIndex));
                        
                        Process.NextDuriation = 500;
                        Process.ID = 50;

                        break;
                    case 50:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.ISAllOnSite())
                            {
                                OnTrigger(ResultStatusEnum.PROCESSEND);

                                if (!IsJustRunTest)
                                    OnEnvTrigger(ResultStatusEnum.LOGPROCESS, -1, "End Process.");
                                //MACHINE.SetNothing();
                                Process.Stop();
                            }
                        }
                        break;
                }
            }
            

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

                //MACHINE.SetNothing();
            }
            else
            {
                EnvIndex = operationindex;
                AlbumWork.SetEnvRunIndex(EnvIndex);
            }
        }


        #region CaptureOnce Process

        bool IsCaptureOnceReverse = false;
        void CaptureOnceTick()
        {
            ProcessClass Process = CaptureOnceProcess;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        if (IsCaptureOnceReverse)
                        {
                            CellIndex = CellStartIndex + CellCount - 1;
                            PageIndex = CellCount - 1;

                            MACHINE.GoPosition(CombinPosition(PositionSetting.POS[CellIndex], IsMicroScope, (XDirCount - XDirIndex - 1), YDirIndex));

                        }
                        else
                        {
                            CellIndex = CellStartIndex;
                            PageIndex = 0;

                            //MACHINE.GoPosition(PositionSetting.POS[CellIndex]);

                            MACHINE.GoPosition(CombinPosition(PositionSetting.POS[CellIndex], IsMicroScope, XDirIndex, YDirIndex));
                        }

                        Process.NextDuriation = 500;
                        Process.ID = 10;

                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.ISAllOnSite())
                            {
                                Process.NextDuriation = INI.DELAYTIME;
                                Process.ID = 15;
                            }
                        }
                        break;
                    case 15:
                        if(Process.IsTimeup)
                        {
                            OnTriggerOP(ResultStatusEnum.CAPTUREONCE, PageIndex.ToString() + "," + (IsJustRunTest ? "T" : "P"));

                            if (IsCaptureOnceReverse)
                            {
                                PageIndex--;
                                CellIndex--;

                                if (CellIndex >= (CellStartIndex))
                                {
                                    MACHINE.GoPosition(CombinPosition(PositionSetting.POS[CellIndex], IsMicroScope, (XDirCount - XDirIndex -1), YDirIndex));

                                    Process.NextDuriation = 500;
                                    Process.ID = 10;
                                }
                                else
                                {
                                    Process.NextDuriation = 200;
                                    Process.ID = 20;
                                }
                            }
                            else
                            {
                                PageIndex++;
                                CellIndex++;

                                if (CellIndex < (CellCount + CellStartIndex))
                                {
                                    MACHINE.GoPosition(CombinPosition(PositionSetting.POS[CellIndex], IsMicroScope, XDirIndex, YDirIndex));

                                    Process.NextDuriation = 500;
                                    Process.ID = 10;
                                }
                                else
                                {
                                    Process.NextDuriation = 200;
                                    Process.ID = 20;
                                }
                            }
                        }
                        break;
                    case 20:
                        if (Process.IsTimeup)
                        {
                            if (MACHINE.ISAllOnSite())
                            {
                                OnTriggerOP(ResultStatusEnum.CAPTUREONCEEND , "");
                                Process.Stop();
                            }
                        }
                        break;
                }
            }
        }
        int GetCellCount(PositionSettings possetting,ref int cellindex)
        {
            int ret = -1;

            switch(possetting.DFlyMethod)
            {
                case DFlyMethodEnum.DIRECTPOS:
                    int i = 0;

                    while(i < possetting.POS.Length)
                    {
                        if(possetting.POS[i].Trim() == "")
                        {
                            break;
                        }
                        i++;
                    }
                    ret = i + 1;
                    cellindex = 0;

                    break;
                case DFlyMethodEnum.RELATEPOS:

                    XDirCount = int.Parse(possetting.POS[0]);
                    YDirCount = int.Parse(possetting.POS[1]);

                    ret = int.Parse(possetting.POS[2]);

                    cellindex = 3;

                    XDirPos = possetting.POS[cellindex + ret];
                    YDirPos = possetting.POS[cellindex + ret + 1];

                    XDirOffset = float.Parse(XDirPos.Split(',')[0]) - float.Parse(possetting.POS[cellindex].Split(',')[0]);
                    YDirOffset = float.Parse(YDirPos.Split(',')[1]) - float.Parse(possetting.POS[cellindex].Split(',')[1]);

                    break;
            }
            return ret;
        }
        string CombinPosition(string orgpos,bool ismicroscope,int xdir,int ydir)
        {
            string retstr = "";

            float[] orgposs = new float[3];
            string[] orgstrs = orgpos.Split(',');

            //string[] xdirstrs = xdirpos.Split(',');
            //string[] ydirstrs = ydirpos.Split(',');

            //float xoffset = float.Parse(xdirstrs[0]) - float.Parse(orgstrs[0]);
            //float yoffset = float.Parse(ydirstrs[1]) - float.Parse(orgstrs[1]);
            
            orgposs[0] = float.Parse(orgstrs[0]);
            orgposs[1] = float.Parse(orgstrs[1]);
            orgposs[2] = float.Parse(orgstrs[2]);
            
            if (ismicroscope)
            {
                orgposs[0] += (INI.LENSOFFSET[0] + ((float)xdir * XDirOffset));
                orgposs[1] += (INI.LENSOFFSET[1]+ ((float)ydir * YDirOffset));
                orgposs[2] += (INI.LENSOFFSET[2]);


            }
            else
            {
                //orgposs[0] += (((float)xdir * xoffset) + ((float)ydir * yoffset));
                //orgposs[1] += (((float)xdir * xoffset) + ((float)ydir * yoffset));
                //orgposs[2] += (((float)xdir * xoffset) + ((float)ydir * yoffset));

                orgposs[0] += ((float)xdir * XDirOffset);
                orgposs[1] += ((float)ydir * YDirOffset);
                //orgposs[2] += (((float)xdir * xoffset) + ((float)ydir * yoffset));
            }

            retstr = orgposs[0].ToString() + "," + orgposs[1].ToString() + "," + orgposs[2].ToString();

            return retstr;
        }
        
        #endregion



    }
}
