using Allinone.UISpace;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Allinone.FormSpace
{
    public partial class R5_IO_Test_Form : Form
    {
        Timer timer = new Timer();
        List<MotorParameter> myMotorParList = new List<MotorParameter>();
        List<MotorControl> motorList = new List<MotorControl>();


        List<R5MotorControlUI> R5ControlList = new List<R5MotorControlUI>();
        Button btnOpenMotor;
        JetEazy.ControlSpace.FatekPLCClass mPLC
        {
            get
            {
                return Universal.MACHINECollection.MACHINE.PLCCollection[0];
            }
        }
        public R5_IO_Test_Form()
        {
            InitializeComponent();
            this.Load += R5_IO_Test_Form_Load;
            this.FormClosed += R5_IO_Test_Form_FormClosed;
        }

        private void R5_IO_Test_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            mPLC.isNormalTempNO = false;
        }

        private void R5_IO_Test_Form_Load(object sender, EventArgs e)
        {
            mPLC.isNormalTempNO = true;
            btnOpenMotor = button1;
            btnOpenMotor.Click += BtnOpenMotor_Click;
         //   motorList.Add(motorControl1);

            for(int i=0;i<6;i++)
            {
                MotorParameter mp = new MotorParameter();
                SETMotoPar(out mp, i+1);
                myMotorParList.Add(mp);

                MotorControl motor = new MotorControl();
                motor.Location = new System.Drawing.Point(0, 0);
                motor.Name = "motor"+i;
                motor.Size = new System.Drawing.Size(372, 508);
          //      motor.TabIndex = 0;

                foreach (Control control in Controls )
                {
                    if (control.Name == "tabControl1")
                    {
                        foreach (Control contro2 in control.Controls)
                        {
                            if (contro2.Name == ("tabPage" + (1 + i)))
                                contro2.Controls.Add(motor);
                        }
                    }
                }

                motorList.Add(motor);
            }


            myMotorParList[0].Name = "后X轴";
            myMotorParList[0].Text1 = "+";
            myMotorParList[0].Text2 = "-";
            myMotorParList[1].Name = "后Y轴";
            myMotorParList[1].Text1 = "+";
            myMotorParList[1].Text2 = "-";

            myMotorParList[2].Name = "前X轴";
            myMotorParList[2].Text1 = "+";
            myMotorParList[2].Text2 = "-";
            myMotorParList[3].Name = "前Y轴";
            myMotorParList[3].Text1 = "+";
            myMotorParList[3].Text2 = "-";

            myMotorParList[4].Name = "镭雕轴";
            myMotorParList[4].Text1 = "+";
            myMotorParList[4].Text2 = "-";
            myMotorParList[5].Name = "定位轴";
            myMotorParList[5].Text1 = "+";
            myMotorParList[5].Text2 = "-";

            for (int i=0;i< motorList.Count;i++)
                motorList[i].Initial(myMotorParList[i]);

            string path= Universal.WORKPATH + "\\R5ControlMax.csv";
            if (System.IO.File.Exists(path))
            {
                List<R5ControlParameter> mylist = new List<R5ControlParameter>();
                using (System.IO.StreamReader sr = new System.IO.StreamReader(path))
                {
                    string line = sr.ReadLine();
                    
                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line = sr.ReadLine()) != null)
                    {
                       if(line!="")
                        {
                            string[] data = line.Split(',');
                            if(data.Length>4)
                            {
                                R5ControlParameter r5par = new R5ControlParameter();
                                r5par.strIO = data[0];
                                r5par.strName = data[1];
                                r5par.strMotoNowIO = data[2];
                                r5par.strMotoLocaIO = data[3];
                                r5par.strMotoGOIO = data[4];

                                mylist.Add(r5par);
                            }
                        }
                    }
                }

                R5MotorControlUI R5ControlUITemp = new R5MotorControlUI();
                R5ControlUITemp.Location = new System.Drawing.Point(0, 6);
                // R5ControlUITemp.Name = "r5MotorControlUI1";
                R5ControlUITemp.Size = new System.Drawing.Size(674, 720);
                R5ControlUITemp.Initial(mylist);
                this.tabPage7.Controls.Add(R5ControlUITemp);
                R5ControlList.Add(R5ControlUITemp);

            }
             path = Universal.WORKPATH + "\\R5ControlMin.csv";
            if (System.IO.File.Exists(path))
            {
                List<R5ControlParameter> mylist = new List<R5ControlParameter>();
                using (System.IO.StreamReader sr = new System.IO.StreamReader(path))
                {
                    string line = sr.ReadLine();


                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line != "")
                        {
                            string[] data = line.Split(',');
                            if (data.Length > 4)
                            {
                                R5ControlParameter r5par = new R5ControlParameter();
                                r5par.strIO = data[0];
                                r5par.strName = data[1];
                                r5par.strMotoNowIO = data[2];
                                r5par.strMotoLocaIO = data[3];
                                r5par.strMotoGOIO = data[4];

                                mylist.Add(r5par);
                            }
                        }
                    }
                }
                

                R5MotorControlUI R5ControlUI = new R5MotorControlUI();
                R5ControlUI.Location = new System.Drawing.Point(0, 6);
                // R5ControlUI.Name = "r5MotorControlUI1";
                R5ControlUI.Size = new System.Drawing.Size(674, 720);
                R5ControlUI.Initial(mylist);
                this.tabPage8.Controls.Add(R5ControlUI);

                R5ControlList.Add(R5ControlUI);
            }

            timer = new Timer();
            timer.Interval = 100;
            timer.Tick += Timer_Tick;
            timer.Start();
            this.CenterToScreen();
            //checkBox1.CheckedChanged += CheckBox1_CheckedChanged;
        }

        private void BtnOpenMotor_Click(object sender, EventArgs e)
        {
            string path = @"D:\AUTOMATION\Eazy AOI DX\MotoRs485\MotoRs485\bin\Debug\MotoRs485.exe";
            if(System.IO.File.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
            }
            else
            {
                MessageBox.Show(path + Environment.NewLine + "路径下没有应用程序!");
            }
        }

        /// <summary>
        /// 设定轴参数
        /// </summary>
        /// <param name="MoPAR">轴</param>
        void SETMotoPar(out UISpace.MotorParameter MoPAR,int iDouble)
        {
            MoPAR = new UISpace.MotorParameter();

            MoPAR.StrAlert = "M1208";
            MoPAR.StrBrake = "M1208";
            MoPAR.StrServo = "M1208";
            MoPAR.StrUPSenser = "M" + (iDouble * 10 + 7);
            MoPAR.StrDownSenser = "M" + (iDouble * 10 + 9);
            MoPAR.StrHomeSenser = "M" + (iDouble * 10 + 8);
            MoPAR.strLocationStart = "M" + (iDouble * 10 + 5);
            MoPAR.strHomeStrat = "M" + (iDouble * 10 + 3);
            MoPAR.strReversal = "M" +( iDouble * 10 + 2);
            MoPAR.strForward = "M" +(iDouble * 10 + 1);

            MoPAR.StrPosition = "R"+( iDouble * 100+2);
            MoPAR.StrHandSpeed = "R" + (iDouble * 100) ;
            MoPAR.StrAutoSpeed = "R" + (iDouble * 100+4);
            MoPAR.StrHomeSpeed_H = "R" + (iDouble * 100 + 8);
            MoPAR.StrHomeSpeed_L = "R" + (iDouble * 100 + 10);
            MoPAR.StrLocation = "R" + (iDouble * 100 + 6);
            //MoPAR.dHandSpeedTemp = mRun.mPAR.dHandSpeedX;
            //MoPAR.dAutoSpeed = mRun.mPAR.dAutoSpeedX;
        }

        

        private void Timer_Tick(object sender, EventArgs e)
        {
            //label1.Text = mPLC.IOData.GetData("D98",1).ToString();
            for (int i = 0; i < motorList.Count; i++)
                motorList[i].Tick();

            for (int i = 0; i < R5ControlList.Count; i++)
                R5ControlList[i].Tick();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //int data = (int)numericUpDown1.Value;
            //mPLC.SetData(textBox1.Text, data,1);
        }
    }
}
