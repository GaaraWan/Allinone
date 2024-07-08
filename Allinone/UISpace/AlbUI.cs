using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Allinone.OPSpace;
using Allinone.UISpace.ALBUISpace;
using JetEazy;
using JetEazy.BasicSpace;

namespace Allinone.UISpace
{
    public partial class AlbUI : UserControl
    {
        LayoutEnum LAYOUT
        {
            get
            {
                return Universal.LAYOUT;
            }
        }

        VersionEnum VERSION;
        OptionEnum OPTION;

        AllinoneAlbUI AllinoneALBUI;
        AudixAlbUI AudixALBUI;
        DFlyAlbUI DFlyALBUI;

        AlbumClass myAlbum;

        public AlbUI()
        {
            switch(LAYOUT)
            {
                case LayoutEnum.L1280X800:
                    InitializeComponent1280X800();
                    break;
                case LayoutEnum.L1440X900:
                    InitializeComponent1440X900();
                    break;
                default:
                    InitializeComponent();
                    break;
            }
            
            InitialInternal();
        }

        void InitialInternal()
        {

        }

        public void Initial(VersionEnum version, OptionEnum option, AlbumClass album)
        {
            VERSION = version;
            OPTION = option;

            myAlbum = album;

            switch (VERSION)
            {
                case VersionEnum.ALLINONE:

                    AllinoneALBUI = new AllinoneAlbUI();
                    AllinoneALBUI.Initial(OPTION, myAlbum);
                    AllinoneALBUI.TriggerAction += AllinoneALBUI_TriggerAction;

                    AllinoneALBUI.Location = new Point(0, 0);
                    this.Controls.Add(AllinoneALBUI);

                    break;
                case VersionEnum.AUDIX:

                    switch(OPTION)
                    {
                        case OptionEnum.MAIN_DFLY:
                            DFlyALBUI = new DFlyAlbUI();
                            DFlyALBUI.Initial(OPTION, myAlbum);

                            DFlyALBUI.TriggerAction += DFlyALBUI_TriggerAction1; ;

                            DFlyALBUI.Location = new Point(0, 0);
                            this.Controls.Add(DFlyALBUI);
                            break;
                        case OptionEnum.MAIN:
                            AudixALBUI = new AudixAlbUI();
                            AudixALBUI.Initial(OPTION, myAlbum);

                            AudixALBUI.TriggerAction += AudixALBUI_TriggerAction;

                            AudixALBUI.Location = new Point(0, 0);
                            this.Controls.Add(AudixALBUI);
                            break;
                    }
                    
                    
                    break;
            }
        }

        private void DFlyALBUI_TriggerAction1(RCPStatusEnum status, string opstr)
        {
            OnTrigger(status, opstr);
        }

        private void AudixALBUI_TriggerAction(RCPStatusEnum status)
        {
            OnTrigger(status, "");
        }

        private void AllinoneALBUI_TriggerAction(RCPStatusEnum status,string opstr)
        {
            OnTrigger(status, opstr);
        }
        public void SetAlbum(AlbumClass album)
        {
            myAlbum = album;

            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    AllinoneALBUI.Initial(OPTION, myAlbum);
                    break;
                case VersionEnum.AUDIX:

                    switch(OPTION)
                    {
                        case OptionEnum.MAIN_DFLY:
                            DFlyALBUI.Initial(OPTION, myAlbum);
                            break;
                        case OptionEnum.MAIN:
                            AudixALBUI.Initial(OPTION, myAlbum);
                            break;
                    }
                    break;
            }
        }
        public void SetPosition(string posstr)
        {
            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    AllinoneALBUI.SetPosition(posstr);
                    break;
                case VersionEnum.AUDIX:
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_DFLY:
                            DFlyALBUI.SetPosition(posstr);
                            break;
                    }
                    break;
            }
        }

        public void WriteBack()
        {
            if (myAlbum.ENVList.Count == 0)
                return;

            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    AllinoneALBUI.WriteBack();
                    break;
                case VersionEnum.AUDIX:
                    switch(OPTION)
                    {
                        case OptionEnum.MAIN_DFLY:
                            DFlyALBUI.WriteBack();
                            break;
                        case OptionEnum.MAIN:
                            AudixALBUI.WriteBack();
                            break;
                    }
                    break;
            }
        }

        public delegate void TriggerHandler(RCPStatusEnum status,string opstr);
        public event TriggerHandler TriggerAction;
        public void OnTrigger(RCPStatusEnum status,string opstr)
        {
            if (TriggerAction != null)
            {
                TriggerAction(status, opstr);
            }
        }

    }
}
