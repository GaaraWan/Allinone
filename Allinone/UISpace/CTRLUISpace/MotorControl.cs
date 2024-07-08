using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Allinone.FormSpace;
//using HiBird.RUNSpace;

namespace Allinone.UISpace
{
    public partial class MotorControl : UserControl
    {
        MotorParameter mPAR;
        //RunClass mRun;
        //UserPassWordForm userform;

        CheckBox ckbHLSpeed;
        Button btn_UP;
        Button btn_Down;
        Button btn_Home;
        Button btn_HandSpeed;
        Button btn_AutoSpeed;
        Button btn_HomeSpeed_H;
        Button btn_HomeSpeed_L;
        Button btn_Location;
        Label lblBrake;
        Label lblServo;
        Label lblAlert;
        Label lblUPSenser;
        Label lblDOWNSenser;
        Label lblHOMESenser;
        Label lblHand;
        Label lblAuto;
        Label lblHomeSpeed_H;
        Label lblHomeSpeed_L;
        Label lblLocation;
        Label lblNowHand;
        Label lblNowAuto;
        Label lblNowHome_H;
        Label lblNowHome_L;
        Label lblNowLocation;
        Label lblNOW;
        NumericUpDown nudHandSpeed;
        NumericUpDown nudAutoSpeed;
        NumericUpDown nudHomeSpeed_H;
        NumericUpDown nudHomeSpeed_L;
        NumericUpDown nudLocation;

       //ControlSpace.MachineCollectionClass mPLC
            JetEazy.ControlSpace.FatekPLCClass mPLC
        {
            get
            {

                return Universal.MACHINECollection.MACHINE.PLCCollection[0];
            }
        }

        public string MyText
        {
            set
            {
                groupBox2.Text = value;
            }
        }
        public string UpText
        {
            set
            {
                btn_UP.Text = value;
            }
        }
        public string DownText
        {
            set
            {
                btn_Down.Text = value;
            }
        }

        public MotorControl()
        {
            InitializeComponent();

        }
        public void Initial(MotorParameter PAR)
        {
            mPAR = PAR;
            //mRun = RunClass.MainRun();
            Loading();

            MyText = PAR.Name;
            UpText = PAR.Text1;
            DownText = PAR.Text2;
        }
        void Loading()
        {
            nudAutoSpeed = numericUpDown10;
            nudHandSpeed = numericUpDown9;
            nudLocation = numericUpDown1;
            nudHomeSpeed_L = numericUpDown2;
            nudHomeSpeed_H = numericUpDown3;

            
            ckbHLSpeed = checkBox1;
            lblBrake = label39;
            lblServo = label10;
            lblAlert = label9;
            lblUPSenser = label8;
            lblDOWNSenser = label3;
            lblHOMESenser = label2;

            lblHand = label27;
            lblAuto = label32;
            lblLocation = label6;
            lblNOW = label28;
            lblNowHand = label11;
            lblNowAuto = label31;
            lblNowLocation = label5;

            lblNowHome_H = label16;
            lblNowHome_L = label13;
            lblHomeSpeed_H = label17;
            lblHomeSpeed_L = label14;
            
            btn_UP = button5;
            btn_Down = button15;
            btn_Home = button18;
            btn_HandSpeed = button4;
            btn_AutoSpeed = button19;
            btn_Location = button3;
            btn_HomeSpeed_L = button1;
            btn_HomeSpeed_H = button2;

            btn_UP.MouseDown += new MouseEventHandler(btn_UP_MouseDown);
            btn_UP.MouseUp += new MouseEventHandler(btn_UP_MouseUp);
            btn_Down.MouseDown += new MouseEventHandler(btn_Down_MouseDown);
            btn_Down.MouseUp += new MouseEventHandler(btn_Down_MouseUp);

            btn_HandSpeed.Click += new EventHandler(btn_HandSpeed_Click);
            btn_Location.Click += new EventHandler(btn_Location_Click);
            btn_AutoSpeed.Click += new EventHandler(btn_AutoSpeed_Click);
            btn_Home.Click += new EventHandler(btn_Home_Click);

            btn_HomeSpeed_L.Click += Btn_HomeSpeed_L_Click;
            btn_HomeSpeed_H.Click += Btn_HomeSpeed_H_Click;

            ckbHLSpeed.CheckedChanged += CkbHLSpeed_CheckedChanged;
            nudLocation.Click += Nud_Click;
            nudHandSpeed.Click += Nud_Click;
            nudAutoSpeed.Click += Nud_Click;


            UpData();
     //       mPAR.dHandSpeedTemp = mPAR.dHandSpeed;

            //mRun.mPLC.WriteRD(mPAR.StrHandSpeed, (int)mPAR.dHandSpeed, 2);
            //mRun.mPLC.WriteRD(mPAR.StrAutoSpeed, (int)mPAR.dAutoSpeed, 2);

            //  mPLC.IOData.GetData("")

        //    mPLC.SetData(mPAR.StrHandSpeed, (int)mPAR.dHandSpeed, 2);
        //    mPLC.SetData(mPAR.StrAutoSpeed, (int)mPAR.dAutoSpeed, 2);
        }

        private void Btn_HomeSpeed_H_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("真的要设定这个值吗?", "SYS", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                return;

            int i_Speed = (int)nudHomeSpeed_H.Value;
            // mRun.mPLC.WriteRD(mPAR.StrHandSpeed, i_Speed, 2);

            mPLC.SetData(mPAR.StrHomeSpeed_H, i_Speed, 2);

            nudHomeSpeed_H.Value = 0;
            mPAR.dHomeSpeedNow_H = i_Speed; ;
        }

        private void Btn_HomeSpeed_L_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("真的要设定这个值吗?", "SYS", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                return;

            int i_Speed = (int) nudHomeSpeed_L .Value;
            // mRun.mPLC.WriteRD(mPAR.StrHandSpeed, i_Speed, 2);

            mPLC.SetData(mPAR.StrHomeSpeed_L, i_Speed, 2);

            nudHomeSpeed_L.Value = 0;
            mPAR.dHomeSpeedNow_L = i_Speed;
        }

        private void Nud_Click(object sender, EventArgs e)
        {
            //if (userform == null)
            //{
            //    NumericUpDown num = sender as NumericUpDown;
            //    userform = new  UserPassWordForm(num);
            //    userform.ShowDialog();
            //    userform.Dispose();
            //    userform = null;
            //}
        }

        public void Tick()
        {
            UpData();
            islblText(mPAR.isServo, lblServo);
            islblText(mPAR.isBrake, lblBrake);
            islblText(mPAR.isAlert, lblAlert);
            islblText(mPAR.isUPSenser, lblUPSenser);
            islblText(mPAR.isDownSenser, lblDOWNSenser);
            islblText(mPAR.isHomeSenser, lblHOMESenser);


            lblHand.Text = mPAR.dHandSpeed.ToString();
            if (mPAR.dHandSpeedNow != 0)
                lblNowHand.Text = mPAR.dHandSpeedNow.ToString();
            else
                lblNowHand.Text = "";
            lblAuto.Text = mPAR.dAutoSpeed.ToString();
            if (mPAR.dAutoSpeedNow != 0)
                lblNowAuto.Text = mPAR.dAutoSpeedNow.ToString();
            else
                lblNowAuto.Text = "";


            lblNowHome_L.Text = mPAR.dHomeSpeed_L.ToString();
            if (mPAR.dHomeSpeedNow_L != 0)
                lblHomeSpeed_L.Text = mPAR.dHomeSpeedNow_L.ToString();
            else
                lblHomeSpeed_L.Text = "";

            lblNowHome_H.Text = mPAR.dHomeSpeed_H.ToString();
            if (mPAR.dHomeSpeedNow_H != 0)
                lblHomeSpeed_H.Text = mPAR.dHomeSpeedNow_H.ToString();
            else
                lblHomeSpeed_H.Text = "";

            lblLocation.Text = mPAR.dLocation.ToString();
            if (mPAR.dLocationNow != 0)
                lblNowLocation.Text = mPAR.dLocationNow.ToString();
            else
                lblNowLocation.Text = "";

           lblNOW.Text = mPAR.dPosition.ToString();
        }

        void UpData()
        {
                if (mPAR.StrAlert != "")
                mPAR.isAlert =mPLC.IOData.GetBit(mPAR.StrAlert );
            if (mPAR.StrBrake != "")
                mPAR.isBrake = mPLC.IOData.GetBit(mPAR.StrBrake);
            mPAR.isServo = mPLC.IOData.GetBit(mPAR.StrServo);
            mPAR.isUPSenser =! mPLC.IOData.GetBit(mPAR.StrUPSenser);
            mPAR.isDownSenser = !mPLC.IOData.GetBit(mPAR.StrDownSenser);
            mPAR.isHomeSenser =! mPLC.IOData.GetBit(mPAR.StrHomeSenser );


            mPAR.dPosition = mPLC.IOData.GetData(mPAR.StrPosition, 2);
            mPAR.dHandSpeed = mPLC.IOData.GetData(mPAR.StrHandSpeed, 2);
            mPAR.dAutoSpeed = mPLC.IOData.GetData(mPAR.StrAutoSpeed, 2);
            mPAR.dHomeSpeed_L = mPLC.IOData.GetData(mPAR.StrHomeSpeed_L, 2);
            mPAR.dHomeSpeed_H = mPLC.IOData.GetData(mPAR.StrHomeSpeed_H, 2);

        }
        void islblText(bool isno, Label lbl)
        {
            if (isno)
            {
                lbl.Text = "●";
                lbl.ForeColor = Color.Red;
            }
            else
            {
                lbl.Text = "○";
                lbl.ForeColor = Color.Black;
            }
        }

        void btn_Down_MouseUp(object sender, MouseEventArgs e)
        {
            btn_Down.BackColor = SystemColors.Control;
            //mRun.mPLC.SetMY(mPAR.strReversal, false);

            mPLC. SetIO(false, mPAR.strReversal);
        }
        void btn_Down_MouseDown(object sender, MouseEventArgs e)
        {
            btn_Down.BackColor = Color.Red;
          //  mRun.mPLC.SetMY(mPAR.strReversal, true);
            mPLC.SetIO(true, mPAR.strReversal);
        }
        void btn_UP_MouseUp(object sender, MouseEventArgs e)
        {
            btn_UP.BackColor = SystemColors.Control;
           // mRun.mPLC.SetMY(mPAR.strForward, false);
            mPLC.SetIO(false, mPAR.strForward);
        }
        void btn_UP_MouseDown(object sender, MouseEventArgs e)
        {
            btn_UP.BackColor = Color.Red;
            //  mRun.mPLC.SetMY(mPAR.strForward, true);
            mPLC.SetIO(true, mPAR.strForward);
        }
        void btn_HandSpeed_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("真的要设定这个值吗?", "SYS", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                return;

            int i_Speed = (int)numericUpDown9.Value;
           // mRun.mPLC.WriteRD(mPAR.StrHandSpeed, i_Speed, 2);

            mPLC.SetData(mPAR.StrHandSpeed, i_Speed, 2);

            numericUpDown9.Value = 0;
            mPAR.dHandSpeedTemp = i_Speed;

            //switch (mPAR.Name)
            //{
                //case "X":
                //    mPAR.dHandSpeedX = i_Speed;
                //    break;
                //case "Y":
                //    mPAR.dHandSpeedY = i_Speed;
                //    break;
                //case "Z":
                //    mPAR.dHandSpeedZ = i_Speed;
                //    break;
                //case "W":
                //    mPAR.dHandSpeedW = i_Speed;
                //    break;
            //}
        }
        void btn_AutoSpeed_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("真的要设定这个值吗?", "SYS", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                return;
            int i_Speed = (int)numericUpDown10.Value;
          //  mRun.mPLC.WriteRD(mPAR.StrAutoSpeed, i_Speed, 2);
            mPLC.SetData(mPAR.StrAutoSpeed, i_Speed, 2);
            numericUpDown10.Value = 0;
            mPAR.dAutoSpeed = i_Speed;

            //switch(mPAR.Name)
            //{
            //    case "X":
            //        mRun.mPAR.dAutoSpeedX = i_Speed;
            //        break;
            //    case "Y":
            //        mRun.mPAR.dAutoSpeedY = i_Speed;
            //        break;
            //    case "Z":
            //        mRun.mPAR.dAutoSpeedZ = i_Speed;
            //        break;
            //    case "W":
            //        mRun.mPAR.dAutoSpeedW = i_Speed;
            //        break;
            //}
        }
        void btn_Location_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("真的要设定这个值吗?", "SYS", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                return;
            int i_ShuDu = (int)numericUpDown1.Value;
          //  mRun.mPLC.WriteRD(mPAR.StrLocation, i_ShuDu, 2);

            mPLC.SetData(mPAR.StrLocation, i_ShuDu, 2);
            numericUpDown1.Value = 0;
           // mRun.mPLC.SetMY(mPAR.strLocationStart, true);
            mPLC.SetIO(true, mPAR.strLocationStart);
            // System.Threading.Thread.Sleep(50);
            //   mRun.mPLC.MYset(mPAR.strLocationStart, false);

        }
        void btn_Home_Click(object sender, EventArgs e)
        {
          //  mRun.mPLC.SetMY(mPAR.strHomeStrat, true);

            mPLC.SetIO(true, mPAR.strHomeStrat);
            //   System.Threading.Thread.Sleep(50);
            //    mRun.mPLC.MYset("M300", false);
        }
        private void CkbHLSpeed_CheckedChanged(object sender, EventArgs e)
        {
          
            double dspeed = 0;
            if (!ckbHLSpeed.Checked)
                dspeed = mPAR.dHandSpeedTemp;
            else
                dspeed = mPAR.dAutoSpeed;

            //mRun.mPLC.WriteRD(mPAR.StrHandSpeed, (int)dspeed, 2);

            mPLC.SetData(mPAR.StrHandSpeed, (int)dspeed, 2);

        }

        public void Close()
        {
          //  mRun.mPLC.WriteRD(mPAR.StrHandSpeed, (int)mPAR.dHandSpeedTemp, 2);

            mPLC.SetData(mPAR.StrHandSpeed, (int)mPAR.dHandSpeedTemp, 2);
        }
    }

    public class MotorParameter
    {
        /// <summary>
        /// 标题名称
        /// </summary>
        public string Name = "";
        /// <summary>
        /// 正转按扭标题
        /// </summary>
        public string Text1 = "";
        /// <summary>
        /// 反转按扭标题
        /// </summary>
        public string Text2 = "";
        /// <summary>
        /// 上极限Senser
        /// </summary>
        public bool isUPSenser = false;
        /// <summary>
        /// 下极限Senser
        /// </summary>
        public bool isDownSenser = false;
        /// <summary>
        /// Home点Senser
        /// </summary>
        public bool isHomeSenser = false;
        /// <summary>
        /// 刹车
        /// </summary>
        public bool isBrake = false;
        /// <summary>
        /// 伺服NO
        /// </summary>
        public bool isServo = false;
        /// <summary>
        /// 警报
        /// </summary>
        public bool isAlert = false;
        /// <summary>
        /// 手动速度
        /// </summary>
        public double dHandSpeed = 0;
        /// <summary>
        /// 手动速度(暂存)
        /// </summary>
        public double dHandSpeedTemp = 0;
        /// <summary>
        /// 当前手动速度
        /// </summary>
        public double dHandSpeedNow = 0;
        /// <summary>
        /// 自动速度
        /// </summary>
        public double dAutoSpeed = 0;
        /// <summary>
        /// 当前自动速度
        /// </summary>
        public double dAutoSpeedNow = 0;
        public double dHomeSpeedNow_L = 0;
        public double dHomeSpeedNow_H = 0;
        public double dHomeSpeed_L = 0;
        public double dHomeSpeed_H = 0;

        /// <summary>
        /// 定位
        /// </summary>
        public double dLocation = 0;
        /// <summary>
        /// 当前定位点
        /// </summary>
        public double dLocationNow = 0;
        /// <summary>
        /// 当前位置
        /// </summary>
        public double dPosition = 0;

        /// <summary>
        /// 手动正转
        /// </summary>
        public string strForward = "";
        /// <summary>
        /// 手动反转
        /// </summary>
        public string strReversal = "";
        /// <summary>
        /// 上极限IO点
        /// </summary>
        public string StrUPSenser = "'";
        /// <summary>
        /// 下极限IO点
        /// </summary>
        public string StrDownSenser = "";
        /// <summary>
        /// HOME的IO点
        /// </summary>
        public string StrHomeSenser = "";
        /// <summary>
        /// Home启动
        /// </summary>
        public string strHomeStrat = "";
        /// <summary>
        /// 刹车IO点
        /// </summary>
        public string StrBrake = "";
        /// <summary>
        /// 伺服NO的IO点
        /// </summary>
        public string StrServo = "";
        /// <summary>
        /// 警报IO点
        /// </summary>
        public string StrAlert = "";
        /// <summary>
        /// 手动速度IO点
        /// </summary>
        public string StrHandSpeed = "";
        //  public string StrHandSpeedNow = "";
        /// <summary>
        /// 自动速度IO点
        /// </summary>
        public string StrAutoSpeed = "";
        /// <summary>
        /// Home速度IO点
        /// </summary>
        public string StrHomeSpeed_H = "";
        /// <summary>
        /// Home速度IO点
        /// </summary>
        public string StrHomeSpeed_L = "";
        //  public string StrAutoSpeedNow = "";
        /// <summary>
        /// 到定位IO点
        /// </summary>
        public string StrLocation = "";
        /// <summary>
        /// 定位启动IO
        /// </summary>
        public string strLocationStart = "";
        //  public string StrLocationNow = "";
        /// <summary>
        /// 当前位置IO
        /// </summary>
        public string StrPosition = "";
    }
}