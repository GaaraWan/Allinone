using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JetEazy;
using JetEazy.BasicSpace;

using Allinone.ControlSpace.IOSpace;

namespace Allinone.ControlSpace.MachineSpace
{
    public class JzAudixMachineClass : GeoMachineClass
    {
        const int MSDuriation = 10;

        public JzAudixIOClass AUDIXIO;

        bool IsPass = false;
        bool IsNG = false;
        
        public JzAudixMachineClass(Machine_EA machineea, VersionEnum version, OptionEnum option, string opstr, string workpath, bool isnouseplc)
        {
            IsNoUseIO = isnouseplc;

            myMachineEA = machineea;
            VERSION = version;
            OPTION = option;

            WORKPATH = workpath;

            GetOPString(opstr);

            MainProcess = new ProcessClass();

            myJzTimes = new JzTimes();
            myJzTimes.Cut();
        }

        public override bool Initial(bool isnouseio,bool isnousemotor)
        {
            bool ret = false;

            AUDIXIO = new JzAudixIOClass();
            
            IsNoUseMotor = isnousemotor;

            if (isnouseio)
            {
                IsNoUseIO = isnouseio;
                return true;
            }
            
            switch(OPTION)
            {
                case OptionEnum.MAIN:
                    ret = AUDIXIO.InitialIO();
                    break;
                default:
                    ret = true;
                    break;
            }

            return ret;
        }

        public override void GetOPString(string opstr)
        {
            //Do Nothing
        }
        public override void Tick()
        {
            if (myJzTimes.msDuriation < MSDuriation)
                return;

            CheckEvent();

            myJzTimes.Cut();
        }
        public override void MainProcessTick()
        {
            throw new NotImplementedException();
        }
        public override void SetDelayTime()
        {
            throw new NotImplementedException();
        }
        public override void GetStart(bool isdirect, bool isnouseplc)
        {
            throw new NotImplementedException();
        }
        public override void CheckEvent()
        {
            if (IsNoUseIO)
                return;

            switch (OPTION)
            {
                case OptionEnum.MAIN:
                    AUDIXIO.GetAllIO();
                    break;
                default:
                    
                    break;
            }
        }
        public void SetPass(bool ispass)
        {
            if (IsNoUseIO)
                return;

            switch (OPTION)
            {
                case OptionEnum.MAIN:
                    AUDIXIO.SetPass(ispass);
                    break;
                default:

                    break;
            }
        }
        public void SetPassOutput(bool ison)
        {
            if (IsNoUseIO)
                return;

            switch (OPTION)
            {
                case OptionEnum.MAIN:
                    AUDIXIO.SetPassOutput(ison);
                    break;
                default:

                    break;
            }
        }
        public void SetNGOutput(bool ison)
        {
            if (IsNoUseIO)
                return;

            switch (OPTION)
            {
                case OptionEnum.MAIN:
                    AUDIXIO.SetNGOutput(ison);
                    break;
                default:

                    break;
            }
        }
        public void SetNothing()
        {
            if (IsNoUseIO)
                return;

            switch (OPTION)
            {
                case OptionEnum.MAIN:
                    AUDIXIO.SetNothing();
                    break;
                default:

                    break;
            }
        }

        public override void GoHome()
        {
        }

    }
}
