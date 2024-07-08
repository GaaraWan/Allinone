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

namespace Allinone.OPSpace.ResultSpace
{
    class JzAudixResultClass : GeoResultClass
    {
        JzAudixMachineClass MACHINE;

        public JzAudixResultClass(Result_EA resultea,VersionEnum version,OptionEnum option,MachineCollectionClass machinecollection)
        {
            myResultEA = resultea;
            VERSION = version;
            OPTION = option;

            MACHINE = (JzAudixMachineClass)machinecollection.MACHINE;
            
            MainProcess = new ProcessClass();

        }
        private void CCDCollection_TriggerAction(string operationstr)
        {
            
        }

        public override void GetStart(AlbumClass albumwork,CCDCollectionClass ccdcollection,TestMethodEnum testmethod, bool isnouseccd)
        {
            if(MainProcess.IsOn)
            {
                //MainProcess.Stop();

                //OnTrigger(ResultStatusEnum.FORECEEND);
                
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
            MainProcessTick();

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

                        Process.NextDuriation = 50;

                        //if (IsSimulator)
                        //    Process.ID = 20;
                        //else
                        //    Process.ID = 10;

                        Process.ID = 20;

                        break;
                    //case 10:                        //變換CCD亮度設定及光源設定
                    //    if (Process.IsTimeup)
                    //    {
                    //        //OnEnvTrigger(ResultStatusEnum.SETCAMLIGHT, EnvIndex, "ALL");

                    //        Process.NextDuriation = DelayTime[0];
                    //        Process.ID = 20;
                    //    }
                    //    break;
                    case 20:
                        if (Process.IsTimeup)       //抓圖
                        {
                            OnTrigger(ResultStatusEnum.COUNTSTART);

                            switch(TestMethod)
                            {
                                case TestMethodEnum.BUTTON:
                                    OnEnvTrigger(ResultStatusEnum.CHANGEENVDIRECTORY, EnvIndex, PageOPTypeEnum.P00.ToString());
                                    CCDCollection.GetImage();
                                    break;
                                case TestMethodEnum.CCDTRIGGER:

                                    break;
                            }
                            
                            FillProcessImage();

                            Process.NextDuriation = 50;
                            Process.ID = 30;
                        }
                        break;
                    case 30:
                        if(Process.IsTimeup)
                        {
                            AlbumWork.A08_RunProcess(PageOPTypeEnum.P00);

                            AlbumWork.FillRunStatus(RunStatusCollection);

                            IsPass = RunStatusCollection.NGCOUNT == 0;

                            OnEnvTrigger(ResultStatusEnum.SAVEDEBUGRAW, EnvIndex, "-1:");

                            if (IsPass)
                            {
                                EnvIndex++;

                                if(EnvIndex < AlbumWork.ENVCount)
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

                        if (IsPass)
                        {
                            MACHINE.SetPass(true);
                            OnTrigger(ResultStatusEnum.CALPASS);
                        }
                        else
                        {
                            MACHINE.SetPass(false);
                            OnTrigger(ResultStatusEnum.CALNG);
                        }

                        OnTrigger(ResultStatusEnum.COUNTEND);
                        OnTrigger(ResultStatusEnum.CALEND);

                        Process.NextDuriation = 100;
                        Process.ID = 50;

                        break;
                    case 50:
                        
                        MACHINE.SetNothing();
                        Process.Stop();

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

                MACHINE.SetNothing();


            }
            else
            {
                EnvIndex = operationindex;
                AlbumWork.SetEnvRunIndex(EnvIndex);
            }
        }


    }
}
