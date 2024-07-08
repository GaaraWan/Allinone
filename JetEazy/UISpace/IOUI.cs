using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using JetEazy.ControlSpace;

namespace JetEazy.UISpace
{
    public enum IOEnum : int
    {
        OUTCOUNT = 5,
        COUNT = 12,

        EMCON = 0,
        STARTON = 1,
        GATEUPON = 2,
        GATEDOWNON = 3,

        PRESSUP = 4,
        PRESSDOWN = 5,

        LEFTOUT = 6,
        LEFTIN = 7,
        RIGHTOUT = 8,
        RIGHTIN = 9,
        INPOSRT = 10,
        INPOSLB = 11,

        GATE = 0,
        PRESS = 1,
        LEFTLOCK = 2,
        RIGHTLOCK = 3,
        SLIDE = 4,

        POSITION = 0,

        ZOPCOUNT = 11,
        UZOPCOUNT = 7,

        GOCW = 0,
        GOCCW = 1,
        ADDMOVE = 2,
        SUBMOVE = 3,
        GO = 4,

        SETPOSITION1 = 5,
        GOPOSITION1 = 6,

        SETPOSITION2 = 7,
        GOPOSITION2 = 8,

        SETPOSITION3 = 9,
        GOPOSITION3 = 10,

        GOPOSITIONVALUE = 11,

        SYNCADD = 12,
        SYNCSUB = 13,
        SYNCVALUE = 14,

    }

    public partial class IOUI : UserControl
    {
        Button[] btnOut = new Button[(int)IOEnum.OUTCOUNT];
        Label[] lblIn = new Label[(int)IOEnum.COUNT];

        Button[] btnZOP = new Button[(int)IOEnum.ZOPCOUNT];
        Button[] btnUZOP = new Button[(int)IOEnum.UZOPCOUNT];

        Button btnSyncAdd;
        Button btnSyncSub;
        NumericUpDown numSyncValue;

        Label lblZPosition;
        Label lblUZPosition;

        Label lblZCalibrationPosition;
        Label lblZTest1Position;
        Label lblZTest2Position;

        Label lblUZPressPosition;

        NumericUpDown numZOPMovValue;

        NumericUpDown numUZOPMoveValue;
        NumericUpDown numUZOPBufferValue;

        Panel pnlZControl;
        Panel pnlUZControl;

        FatekPLCClass PLC;

        bool IsNeedToChange =false;
        
        public IOUI()
        {
            InitializeComponent();
        }

        public void Initial(FatekPLCClass plc)
        {
            int i = 0;

            PLC = plc;

            btnOut[(int)IOEnum.GATE] = button24;
            btnOut[(int)IOEnum.PRESS] = button1;
            btnOut[(int)IOEnum.LEFTLOCK] = button2;
            btnOut[(int)IOEnum.RIGHTLOCK] = button3;
            btnOut[(int)IOEnum.SLIDE] = button20;

            lblIn[(int)IOEnum.EMCON] = label23;
            lblIn[(int)IOEnum.STARTON] = label17;
            lblIn[(int)IOEnum.GATEUPON] = label1;
            lblIn[(int)IOEnum.GATEDOWNON] = label2;

            lblIn[(int)IOEnum.PRESSUP] = label4;
            lblIn[(int)IOEnum.PRESSDOWN] = label3;
            lblIn[(int)IOEnum.LEFTOUT] = label8;
            lblIn[(int)IOEnum.LEFTIN] = label7;
            lblIn[(int)IOEnum.RIGHTOUT] = label6;
            lblIn[(int)IOEnum.RIGHTIN] = label5;
            lblIn[(int)IOEnum.INPOSRT] = label18;
            lblIn[(int)IOEnum.INPOSLB] = label16;


            btnZOP[(int)IOEnum.GOCW] = button11;
            btnZOP[(int)IOEnum.GOCCW] = button10;
            btnZOP[(int)IOEnum.ADDMOVE] = button14;
            btnZOP[(int)IOEnum.SUBMOVE] = button13;
            btnZOP[(int)IOEnum.GO] = button12;
            btnZOP[(int)IOEnum.SETPOSITION1] = button6;
            btnZOP[(int)IOEnum.GOPOSITION1] = button7;
            btnZOP[(int)IOEnum.SETPOSITION2] = button9;
            btnZOP[(int)IOEnum.GOPOSITION2] = button8;
            btnZOP[(int)IOEnum.SETPOSITION3] = button22;
            btnZOP[(int)IOEnum.GOPOSITION3] = button21;

            btnUZOP[(int)IOEnum.GOCW] = button16;
            btnUZOP[(int)IOEnum.GOCCW] = button15;
            btnUZOP[(int)IOEnum.ADDMOVE] = button19;
            btnUZOP[(int)IOEnum.SUBMOVE] = button18;
            btnUZOP[(int)IOEnum.GO] = button17;
            btnUZOP[(int)IOEnum.SETPOSITION1] = button5;
            btnUZOP[(int)IOEnum.GOPOSITION1] = button4;

            pnlZControl = panel1;
            pnlUZControl = panel2;

            btnSyncAdd = button25;
            btnSyncSub = button23;
            numSyncValue = numericUpDown4;

            btnSyncAdd.Tag = IOEnum.SYNCADD;
            btnSyncSub.Tag = IOEnum.SYNCSUB;

            numSyncValue.ValueChanged += new EventHandler(numSyncValue_ValueChanged);

            btnSyncAdd.Click += new EventHandler(btnSyncAdd_Click);
            btnSyncSub.Click += new EventHandler(btnSyncSub_Click);

            i = 0;
            while (i < (int)IOEnum.OUTCOUNT)
            {
                btnOut[i].Tag = (IOEnum)i;
                btnOut[i].Click += new EventHandler(IOUI_Click);
                i++;
            }

            i = 0;
            while (i < (int)IOEnum.ZOPCOUNT)
            {
                btnZOP[i].Tag = (IOEnum)i;
                btnZOP[i].Click += new EventHandler(IOUIZOP_Click);
                btnZOP[i].MouseDown += new MouseEventHandler(IOUIZOP_MouseDown);
                btnZOP[i].MouseUp += new MouseEventHandler(IOUIZOP_MouseUp);

                i++;
            }

            i = 0;
            while (i < (int)IOEnum.UZOPCOUNT)
            {
                btnUZOP[i].Tag = (IOEnum)i;
                btnUZOP[i].Click += new EventHandler(IOUIUZOP_Click);
                btnUZOP[i].MouseDown += new MouseEventHandler(IOUIUZOP_MouseDown);
                btnUZOP[i].MouseUp += new MouseEventHandler(IOUIUZOP_MouseUp);
                i++;
            }

            lblZPosition = label9;
            lblUZPosition = label11;

            lblZCalibrationPosition = label25;
            lblZTest1Position = label28;
            lblZTest2Position = label20;

            lblUZPressPosition = label13;

            numZOPMovValue = numericUpDown1;
            numUZOPMoveValue = numericUpDown2;
            numUZOPBufferValue = numericUpDown3;

            numZOPMovValue.Tag = IOEnum.GOPOSITIONVALUE;
            numUZOPMoveValue.Tag = IOEnum.GOPOSITIONVALUE;

            numUZOPBufferValue.ValueChanged += new EventHandler(numUZOPBufferValue_ValueChanged);

            //switch (Universal.VER)
            //{
            //    case "R7":
            //    case "R5":
            //        pnlUZControl.Visible = false;
            //        break;
            //    default:
            //        pnlUZControl.Visible = true;
            //        break;
            //}

            FillDisplay();
        }

        void btnSyncSub_Click(object sender, EventArgs e)
        {
            //PLC.SYNCSetSpeed(INI.SYNCSLOWSPEED);
            //PLC.SYNCGOPosition(-INI.SYNCTESTVALUE);
            //PLC.SYNCGo = true;
        }
        void btnSyncAdd_Click(object sender, EventArgs e)
        {
            //PLC.SYNCSetSpeed(INI.SYNCSLOWSPEED);
            //PLC.SYNCGOPosition(INI.SYNCTESTVALUE);
            //PLC.SYNCGo = true;
        }
        void numSyncValue_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown num = (NumericUpDown)sender;

            //if (IsNeedToChange)
            //{
            //    INI.SYNCTESTVALUE = (float)num.Value;
            //}
        }

        void IOUIZOP_MouseUp(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;

            //switch ((IOEnum)btn.Tag)
            //{
            //    case IOEnum.GOCW:
            //        PLC.ZCW = false;
            //        break;
            //    case IOEnum.GOCCW:
            //        PLC.ZCCW = false;
            //        break;
            //}
        }
        void IOUIZOP_MouseDown(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;

            //switch ((IOEnum)btn.Tag)
            //{
            //    case IOEnum.GOCW:
            //        PLC.ZCW = true;
            //        break;
            //    case IOEnum.GOCCW:
            //        PLC.ZCCW = true;
            //        break;
            //}
        }
        void IOUIUZOP_MouseUp(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;

            //switch ((IOEnum)btn.Tag)
            //{
            //    case IOEnum.GOCW:
            //        PLC.UZCW = false;
            //        break;
            //    case IOEnum.GOCCW:
            //        PLC.UZCCW = false;
            //        break;
            //}
        }
        void IOUIUZOP_MouseDown(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;

            //switch ((IOEnum)btn.Tag)
            //{
            //    case IOEnum.GOCW:
            //        PLC.UZCW = true;
            //        break;
            //    case IOEnum.GOCCW:
            //        PLC.UZCCW = true;
            //        break;
            //}
        }

        void numUZOPBufferValue_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown num = (NumericUpDown)sender;

            //if (IsNeedToChange)
            //{
            //    INI.UZBUFFERVALUE = (float)num.Value;
            //}
        }

        void IOUIZOP_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            //switch ((IOEnum)btn.Tag)
            //{
            //    case IOEnum.ADDMOVE:
            //        PLC.ZGoPosition((float)numZOPMovValue.Value, true);
            //        PLC.ZGo = true;
            //        break;
            //    case IOEnum.SUBMOVE:
            //        PLC.ZGoPosition((float)numZOPMovValue.Value, false);
            //        PLC.ZGo = true;
            //        break;
            //    case IOEnum.GO:
            //        PLC.ZGoPosition((float)numZOPMovValue.Value);
            //        PLC.ZGo = true;
            //        break;
            //    case IOEnum.SETPOSITION1:
            //        INI.ZCALIBRATIONPOSITION = PLC.PositionNow[(int)MotionEnum.ZAXIS];
            //        break;
            //    case IOEnum.GOPOSITION1:
            //        PLC.ZGoPosition(INI.ZCALIBRATIONPOSITION);
            //        PLC.ZGo = true;
            //        break;
            //    case IOEnum.SETPOSITION2:
            //        INI.ZTEST1POSITION = PLC.PositionNow[(int)MotionEnum.ZAXIS];
            //        break;
            //    case IOEnum.GOPOSITION2:
            //        PLC.ZGoPosition(INI.ZTEST1POSITION);
            //        PLC.ZGo = true;
            //        break;
            //    case IOEnum.SETPOSITION3:
            //        INI.ZTEST2POSITION = PLC.PositionNow[(int)MotionEnum.ZAXIS];
            //        break;
            //    case IOEnum.GOPOSITION3:
            //        PLC.ZGoPosition(INI.ZTEST2POSITION);
            //        PLC.ZGo = true;
            //        break;
            //}

            FillDisplay();
            
        }
        void IOUIUZOP_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            //switch ((IOEnum)btn.Tag)
            //{
            //    case IOEnum.ADDMOVE:
            //        PLC.UZGoPosition((float)numUZOPMoveValue.Value, true);
            //        PLC.UZGo = true;
            //        break;
            //    case IOEnum.SUBMOVE:
            //        PLC.UZGoPosition((float)numUZOPMoveValue.Value, false);
            //        PLC.UZGo = true;
            //        break;
            //    case IOEnum.GO:
            //        PLC.UZGoPosition((float)numUZOPMoveValue.Value);
            //        PLC.UZGo = true;
            //        break;
            //    case IOEnum.SETPOSITION1:
            //        INI.UZPRESSPOSITION = PLC.PositionNow[(int)MotionEnum.UZAXIS];
            //        break;
            //    case IOEnum.GOPOSITION1:
            //        PLC.UZGoPosition(INI.UZPRESSPOSITION);
            //        PLC.UZGo = true;
            //        break;
            //}

            FillDisplay();
        }

        void IOUI_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            //switch ((IOEnum)btn.Tag)
            //{
            //    case IOEnum.GATE:
            //        PLC.Gate = PLC.IsGateUp;
            //        break;
            //    case IOEnum.PRESS:
            //        PLC.Press = PLC.IsPressUp;
            //        break;
            //    case IOEnum.LEFTLOCK:
            //        PLC.LeftLock = PLC.IsLeftLockIn;
            //        break;
            //    case IOEnum.RIGHTLOCK:
            //        PLC.RightLock = PLC.IsRightLockIn;
            //        break;
            //    case IOEnum.SLIDE:
            //        PLC.Slide = !PLC.IsInPosRT;
            //        break;
            //}

            FillDisplay();
        }

        public void Tick()
        {
            //if (Universal.VER == "R7")
            //    return;

            //btnOut[(int)IOEnum.GATE].BackColor = (PLC.IsGateDown ? Color.Red : SystemColors.Control);
            ////btnOut[(int)IOEnum.PRESS].BackColor = (PLC.IsPressDown ? Color.Red : SystemColors.Control);
            ////btnOut[(int)IOEnum.LEFTLOCK].BackColor = (PLC.IsLeftLockOut ? Color.Red : SystemColors.Control);
            ////btnOut[(int)IOEnum.RIGHTLOCK].BackColor = (PLC.IsRightLockOut ? Color.Red : SystemColors.Control);
            ////btnOut[(int)IOEnum.SLIDE].BackColor = (PLC.IsSlideFront ? Color.Red : SystemColors.Control);
 
            //lblIn[(int)IOEnum.EMCON].BackColor = (PLC.IsEMC ? Color.Red : Color.Black);
            //lblIn[(int)IOEnum.STARTON].BackColor = (PLC.IsStart ? Color.Red : Color.Black);
            //lblIn[(int)IOEnum.GATEUPON].BackColor = (PLC.IsGateUp ? Color.Red : Color.Black);
            //lblIn[(int)IOEnum.GATEDOWNON].BackColor = (PLC.IsGateDown ? Color.Red : Color.Black);


            ////lblIn[(int)IOEnum.PRESSUP].BackColor = (PLC.IsPressUp ? Color.Red : Color.Black);
            ////lblIn[(int)IOEnum.PRESSDOWN].BackColor = (PLC.IsPressDown ? Color.Red : Color.Black);
            ////lblIn[(int)IOEnum.LEFTIN].BackColor = (PLC.IsLeftLockIn ? Color.Red : Color.Black);
            ////lblIn[(int)IOEnum.LEFTOUT].BackColor = (PLC.IsLeftLockOut ? Color.Red : Color.Black);
            ////lblIn[(int)IOEnum.RIGHTIN].BackColor = (PLC.IsRightLockIn ? Color.Red : Color.Black);
            ////lblIn[(int)IOEnum.RIGHTOUT].BackColor = (PLC.IsRightLockOut ? Color.Red : Color.Black);
            //lblIn[(int)IOEnum.INPOSRT].BackColor = (PLC.IsInPosRT ? Color.Red : Color.Black);
            //lblIn[(int)IOEnum.INPOSLB].BackColor = (PLC.IsInPosLB ? Color.Red : Color.Black);

            //lblZPosition.Text = PLC.PositionNow[(int)MotionEnum.ZAXIS].ToString("0.000");
            //lblUZPosition.Text = PLC.PositionNow[(int)MotionEnum.UZAXIS].ToString("0.000");

            //btnZOP[(int)IOEnum.GOCW].BackColor = (PLC.IsMotionOnSite[(int)MotionEnum.ZAXIS] && PLC.IsMotionHomed[(int)MotionEnum.ZAXIS] ? SystemColors.Control : Color.Red);
            //btnZOP[(int)IOEnum.GOCCW].BackColor = (PLC.IsMotionOnSite[(int)MotionEnum.ZAXIS] && PLC.IsMotionHomed[(int)MotionEnum.ZAXIS] ? SystemColors.Control : Color.Red);
            //btnZOP[(int)IOEnum.ADDMOVE].BackColor = (PLC.IsMotionOnSite[(int)MotionEnum.ZAXIS] && PLC.IsMotionHomed[(int)MotionEnum.ZAXIS] ? SystemColors.Control : Color.Red);
            //btnZOP[(int)IOEnum.SUBMOVE].BackColor = (PLC.IsMotionOnSite[(int)MotionEnum.ZAXIS] && PLC.IsMotionHomed[(int)MotionEnum.ZAXIS] ? SystemColors.Control : Color.Red);
            //btnZOP[(int)IOEnum.GO].BackColor = (PLC.IsMotionOnSite[(int)MotionEnum.ZAXIS] && PLC.IsMotionHomed[(int)MotionEnum.ZAXIS] ? SystemColors.Control : Color.Red);
            //btnZOP[(int)IOEnum.GOPOSITION1].BackColor = (PLC.IsMotionOnSite[(int)MotionEnum.ZAXIS] && PLC.IsMotionHomed[(int)MotionEnum.ZAXIS] ? SystemColors.Control : Color.Red);
            //btnZOP[(int)IOEnum.GOPOSITION2].BackColor = (PLC.IsMotionOnSite[(int)MotionEnum.ZAXIS] && PLC.IsMotionHomed[(int)MotionEnum.ZAXIS] ? SystemColors.Control : Color.Red);
            //btnZOP[(int)IOEnum.GOPOSITION3].BackColor = (PLC.IsMotionOnSite[(int)MotionEnum.ZAXIS] && PLC.IsMotionHomed[(int)MotionEnum.ZAXIS] ? SystemColors.Control : Color.Red);

            //btnUZOP[(int)IOEnum.GOCW].BackColor = (PLC.IsMotionOnSite[(int)MotionEnum.UZAXIS] && PLC.IsMotionHomed[(int)MotionEnum.UZAXIS] ? SystemColors.Control : Color.Red);
            //btnUZOP[(int)IOEnum.GOCCW].BackColor = (PLC.IsMotionOnSite[(int)MotionEnum.UZAXIS] && PLC.IsMotionHomed[(int)MotionEnum.UZAXIS] ? SystemColors.Control : Color.Red);
            //btnUZOP[(int)IOEnum.ADDMOVE].BackColor = (PLC.IsMotionOnSite[(int)MotionEnum.UZAXIS] && PLC.IsMotionHomed[(int)MotionEnum.UZAXIS] ? SystemColors.Control : Color.Red);
            //btnUZOP[(int)IOEnum.SUBMOVE].BackColor = (PLC.IsMotionOnSite[(int)MotionEnum.UZAXIS] && PLC.IsMotionHomed[(int)MotionEnum.UZAXIS] ? SystemColors.Control : Color.Red);
            //btnUZOP[(int)IOEnum.GO].BackColor = (PLC.IsMotionOnSite[(int)MotionEnum.UZAXIS] && PLC.IsMotionHomed[(int)MotionEnum.UZAXIS] ? SystemColors.Control : Color.Red);
            //btnUZOP[(int)IOEnum.GOPOSITION1].BackColor = (PLC.IsMotionOnSite[(int)MotionEnum.UZAXIS] && PLC.IsMotionHomed[(int)MotionEnum.UZAXIS] ? SystemColors.Control : Color.Red);
        }

        void FillDisplay()
        {
            IsNeedToChange = false;

            //lblUZPressPosition.Text = INI.UZPRESSPOSITION.ToString("0.000");

            //lblZCalibrationPosition.Text = INI.ZCALIBRATIONPOSITION.ToString("0.000");
            //lblZTest1Position.Text = INI.ZTEST1POSITION.ToString("0.000");
            //lblZTest2Position.Text = INI.ZTEST2POSITION.ToString("0.000");

            //numUZOPBufferValue.Value = (decimal)INI.UZBUFFERVALUE;
            //numSyncValue.Value = (decimal)INI.SYNCTESTVALUE;

            IsNeedToChange = true;

        }
        public void SetMoveZero()
        {
            numUZOPMoveValue.Value = 0;
            numZOPMovValue.Value = 0;
        }

    }
}
