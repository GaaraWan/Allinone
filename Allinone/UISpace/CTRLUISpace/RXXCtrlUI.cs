using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using JetEazy;
using JetEazy.BasicSpace;
using JetEazy.UISpace;
using Allinone.ControlSpace;
using Allinone.ControlSpace.MachineSpace;
using Allinone.ControlSpace.IOSpace;

namespace Allinone.UISpace
{
    public partial class RXXCtrlUI : UserControl
    {
        enum TagEnum
        {
            /// <summary>
            /// 頂燈
            /// </summary>
            TOPLIGHT,
            /// <summary>
            ///四周燈管
            /// </summary>
            ARROUNDLIGHT,
            /// <summary>
            /// 鐳雕平板燈
            /// </summary>
            PANNELLIGHT,
            /// <summary>
            /// 神燈
            /// </summary>
            GODLIGHT,
            /// <summary>
            /// 小圓燈
            /// </summary>
            CIRCLELIGHT,

            RED,
            GREEN,
            YELLOW,
            BLUE,
            WHITE,
        }


        const int MSDuriation = 10;

        JzTransparentPanel tpnlCover;
        VersionEnum VERSION = VersionEnum.ALLINONE;
        OptionEnum OPTION = OptionEnum.MAIN;
        JzRXXMachineClass MACHINE;

        JzRXXIOClass PLCIO
        {
            get
            {
                return MACHINE.PLCIO;
            }
        }

        Label lblIsStart;
        Label lblIsUPSError;

        Label lblTopLight;
        Label lblAroundLight;
        Label lblPannelLight;
        Label lblGodLight;
        Label lblCircleLight;
        Label lblRed;
        Label lblGreen;
        Label lblYellow;
        Label lblBlue;
        Label lblWhite;

        JzTimes myJzTimer = new JzTimes();
        public RXXCtrlUI()
        {  
            InitializeComponent();
            InitialInternal();
        }

        void InitialInternal()
        {
            tpnlCover = jzTransparentPanel1;

            lblIsStart = label1;
            lblIsUPSError = label4;

            lblTopLight = label2;
            lblAroundLight = label3;
            lblPannelLight = label5;
            lblGodLight = label6;
            lblCircleLight = label7;
            lblRed = label8;
            lblGreen = label9;
            lblYellow = label10;
            lblBlue = label14;
            lblWhite = label13;

            lblTopLight.Tag = TagEnum.TOPLIGHT;
            lblAroundLight.Tag = TagEnum.ARROUNDLIGHT;
            lblPannelLight.Tag = TagEnum.PANNELLIGHT;
            lblGodLight.Tag = TagEnum.GODLIGHT;
            lblCircleLight.Tag = TagEnum.CIRCLELIGHT;
            lblRed.Tag = TagEnum.RED;
            lblGreen.Tag = TagEnum.GREEN;
            lblYellow.Tag = TagEnum.YELLOW;
            lblBlue.Tag = TagEnum.BLUE;
            lblWhite.Tag = TagEnum.WHITE;

            lblTopLight.DoubleClick += lbl_DoubleClick;
            lblAroundLight.DoubleClick += lbl_DoubleClick;
            lblPannelLight.DoubleClick += lbl_DoubleClick;
            lblGodLight.DoubleClick += lbl_DoubleClick;
            lblCircleLight.DoubleClick += lbl_DoubleClick;
            lblRed.DoubleClick += lbl_DoubleClick;
            lblGreen.DoubleClick += lbl_DoubleClick;
            lblYellow.DoubleClick += lbl_DoubleClick;
            lblBlue.DoubleClick += lbl_DoubleClick;
            lblWhite.DoubleClick += lbl_DoubleClick;

        }

        private void lbl_DoubleClick(object sender, EventArgs e)
        {
            TagEnum KEYS = (TagEnum)((Label)sender).Tag;

            switch (KEYS)
            {
                case TagEnum.TOPLIGHT:
                    PLCIO.TopLight = !PLCIO.TopLight;
                    break;
                case TagEnum.ARROUNDLIGHT:
                    PLCIO.ArroundLight = !PLCIO.ArroundLight;
                    break;
                case TagEnum.PANNELLIGHT:
                    PLCIO.PannelLight = !PLCIO.PannelLight;
                    break;
                case TagEnum.GODLIGHT:
                    PLCIO.GodLight = !PLCIO.GodLight;
                    break;
                case TagEnum.CIRCLELIGHT:
                    PLCIO.CircleLight = !PLCIO.CircleLight;
                    break;
                case TagEnum.RED:
                    PLCIO.Red = !PLCIO.Red;
                    break;
                case TagEnum.GREEN:
                    PLCIO.Green = !PLCIO.Green;
                    break;
                case TagEnum.YELLOW:
                    PLCIO.Yellow = !PLCIO.Yellow;
                    break;
                case TagEnum.BLUE:
                    //PLCIO.Blue = !PLCIO.Blue;
                    break;
                case TagEnum.WHITE:
                    if (INI.ISHIVECLIENT)
                        MACHINE.MachineStateForPlannedDown = !PLCIO.White;
                    //PLCIO.White = !PLCIO.White;
                    break;
            }

        }

        public void Initial(VersionEnum version, OptionEnum option, JzRXXMachineClass machine)
        {
            VERSION = version;
            OPTION = option;
            MACHINE = machine;

            tpnlCover.Location = new Point(0, 0);
            tpnlCover.Size = new Size(336, 303);

            myJzTimer.Cut();

        }

        public void SetEnable(bool isenable)
        {
            tpnlCover.Visible = !isenable;

            this.BackColor = (isenable ? SystemColors.Control : Color.DarkGray);
        }
        public void Tick()
        {
            if (myJzTimer.msDuriation < MSDuriation)
                return;

            MACHINE.Tick();

            lblIsStart.BackColor = (PLCIO.IsStart ? Color.Green : Color.Black);
            lblIsUPSError.BackColor = (PLCIO.IsUPSError ? Color.Red : Color.Black);

            lblTopLight.BackColor = (PLCIO.TopLight ? Color.Green : Color.Black);
            lblAroundLight.BackColor = (PLCIO.ArroundLight ? Color.Green : Color.Black);
            lblPannelLight.BackColor = (PLCIO.PannelLight ? Color.Green : Color.Black);
            lblGodLight.BackColor = (PLCIO.GodLight ? Color.Green : Color.Black);
            lblCircleLight.BackColor = (PLCIO.CircleLight ? Color.Green : Color.Black);

            lblRed.BackColor = (PLCIO.Red ? Color.Red : Color.Black);
            lblYellow.BackColor = (PLCIO.Yellow ? Color.Yellow : Color.Black);
            lblGreen.BackColor = (PLCIO.Green ? Color.Green : Color.Black);
            lblBlue.BackColor = (PLCIO.Blue ? Color.Blue : Color.Black);
            lblWhite.BackColor = (PLCIO.White ? Color.White : Color.Black);

            myJzTimer.Cut();
        }




    }
}
