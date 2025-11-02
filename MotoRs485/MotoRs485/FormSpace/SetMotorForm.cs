using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;


namespace MotoRs485
{
    public partial class SetMotorForm : Form
    {
        /// <summary>
        /// 模拟跑线
        /// </summary>
        bool isDebug = false;

        TabControl TabMotor;
       
        BaseSpace.SERIALPORT mCom;
      //  BaseSpace.SERIALPORT mCom2;

        List<UISpace.LS_MotorControl> lsMotoControlList = new List<UISpace.LS_MotorControl>();
        List<BaseSpace.LS_ControlMotorClass> lsMotoList = new List<BaseSpace.LS_ControlMotorClass>();

        RichTextBox rtbResult;
        Button btnClear;
        ListBox lbParList;
        Button btnParADD;
        Button btnParCut;
        Button btnParGetSet;
        Button btnParSave;
        Button btnParGo;
        Button btnClose;
        TextBox tbParName;

        TextBox tbParPosition5;
        TextBox tbParPosition6;
        TextBox tbParPosition7;
        TextBox tbParPosition8;
        TextBox tbParPosition9;

        string Path = @"D:\JETEAZY\ALLINONE-R5\WORK\Motor\";

        Timer timer = new Timer();
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

        List<ParMotoPosition> myParList = new List<ParMotoPosition>();
        ParMotoPosition ParNow = new ParMotoPosition();

        List<UISpace.PositionUI> myPositionlist = new List<UISpace.PositionUI>();
        bool isallinone = false;

         int iMotoCount = 9;
         int iR5RUNCOUNT = 4;

        public SetMotorForm()
        {
            InitializeComponent();
            this.Load += MainForm_Load;
        }

        public void Initial(string strPath, int MotoCount, int R5RUNCOUNT, bool debug)
        {
            iMotoCount = MotoCount;
            iR5RUNCOUNT = R5RUNCOUNT;
            Path = strPath;
            isDebug = debug;
            //this.Load += MainForm_Load;
            isallinone = true;
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            rtbResult = richTextBox1;
            btnClear = button1;
            btnClear.Click += BtnClear_Click;
            tbParName = textBox4;
            tbParPosition5 = textBox14;
            tbParPosition6 = textBox5;
            tbParPosition7 = textBox1;
            tbParPosition8 = textBox2;
            tbParPosition9 = textBox3;
            lbParList = listBox1;
            btnParADD = button3;
            btnParCut = button4;
            btnParGo = button2;
            btnParGetSet = button5;
            btnParSave = button6;
            TabMotor = tabControl1;
            btnClose = button7;
            
            string strADD = Path;// System.AppDomain.CurrentDomain.BaseDirectory;
            mCom = new BaseSpace.SERIALPORT(strADD + "COM.INI", isDebug);
            mCom.TRIGGERMESSIGE += MCom_TRIGGERMESSIGE;

            //mCom2 = new BaseSpace.SERIALPORT(strADD + "COM_2.INI", isDebug);
            //mCom2.TRIGGERMESSIGE += MCom_TRIGGERMESSIGE;

            for (int i = 1; i < (iMotoCount+1); i++)
            {
                TabPage page = new TabPage();
                page.Text = "轴" + i;
                BaseSpace.LS_ControlMotorClass mMotor;
                mMotor = new BaseSpace.LS_ControlMotorClass(mCom, strADD + "MOTOR_" + i + ".INI");

                mMotor.TRIGGERMESSIGE += Motor_TRIGGERMESSIGE;

                UISpace.LS_MotorControl lS_Motor = new UISpace.LS_MotorControl();
                lS_Motor.Location = new Point(0, 0);
                lS_Motor.Initial(mMotor);

                lsMotoControlList.Add(lS_Motor);
                lsMotoList.Add(mMotor);

                page.Controls.Add(lS_Motor);
                TabMotor.TabPages.Add(page);
            }


            if(iMotoCount==6)
            {
                label27.Visible = false;
                tbParPosition7.Visible = false;
                label2.Visible = false;
                tbParPosition8.Visible = false;
                label3.Visible = false;
                tbParPosition9.Visible = false;
            }
            if (iMotoCount == 7)
            {
                label2.Visible = false;
                tbParPosition8.Visible = false;
                label3.Visible = false;
                tbParPosition9.Visible = false;
            }
            if (iMotoCount == 8)
            {
                label3.Visible = false;
                tbParPosition9.Visible = false;
            }


            myPositionlist.Add(positionUI1);
            myPositionlist.Add(positionUI2);
            myPositionlist.Add(positionUI3);
            myPositionlist.Add(positionUI4);
            myPositionlist.Add(positionUI5);

            for (int i = 0; i < myPositionlist.Count; i++)
            {
                myPositionlist[i].Visible = false;
            }

            for (int i=0;i< iR5RUNCOUNT;i++)
            {
                myPositionlist[i].Visible = true;
            }

            lbParList.SelectedIndexChanged += LbParList_SelectedIndexChanged;
            timer.Interval = 20;
            timer.Tick += Timer_Tick;
            timer.Start();

            loadin();

            for (int i = 1; i < myPositionlist.Count+1; i++)
            {
                myPositionlist[i-1].Initial(i, ParNow.positions[i - 1]);
                myPositionlist[i-1].TRIGGERMESSIGE += MainForm_TRIGGERMESSIGE;
            }

            btnParADD.Click += BtnParADD_Click;
            btnParCut.Click += BtnParCut_Click;
            btnParSave.Click += BtnParSave_Click;
            btnParGetSet.Click += BtnParGetSet_Click;
            btnParGo.Click += BtnParGo_Click;
            btnClose.Click += BtnClose_Click;
            this.FormClosed += MainForm_FormClosed;

            this.CenterToScreen();

            this.Hide();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            if (isallinone)
                this.Hide();
            else
                this.Close();

        }

        public void GoPosition(int index)
        {
            lsMotoList[0].Position(ParNow.positions[index].iPosition_1);
            lsMotoList[0].GoPosition();
            lsMotoList[1].Position(ParNow.positions[index].iPosition_2);
            lsMotoList[1].GoPosition();
            lsMotoList[2].Position(ParNow.positions[index].iPosition_3);
            lsMotoList[2].GoPosition();
            lsMotoList[3].Position(ParNow.positions[index].iPosition_4);
            lsMotoList[3].GoPosition();
        }
        public bool IsGoPositionOKToGoing
        {
            get
            {
                bool isok = true;
                for (int i = 0; i < 4; i++)
                {
                    if (!lsMotoList[i].MotorStateNow.ISGoOK)
                        isok = false;
                }

                return isok;
            }
        }

        public bool IsGoPositionOK(int index)
        {
         
                bool isok = true;
           // for (int i = 0; i < 4; i++)
            {
                if (lsMotoList[0].MotorPositionNow != ParNow.positions[index].iPosition_1)
                    isok = false;
                if (lsMotoList[1].MotorPositionNow != ParNow.positions[index].iPosition_2)
                    isok = false;
                if (lsMotoList[2].MotorPositionNow != ParNow.positions[index].iPosition_3)
                    isok = false;
                if (lsMotoList[3].MotorPositionNow != ParNow.positions[index].iPosition_4)
                    isok = false;
            }

                return isok;
            
        }

        private void MainForm_TRIGGERMESSIGE(object sender, int index, UISpace.UIClick e)
        {
            UISpace.PositionUI UI = (UISpace.PositionUI)sender;
            switch (e)
            {
                case UISpace.UIClick.Go:
                    lsMotoList[0].Position(ParNow.positions[index-1].iPosition_1);
                    lsMotoList[0].GoPosition();
                    lsMotoList[1].Position(ParNow.positions[index -1].iPosition_2);
                    lsMotoList[1].GoPosition();
                    lsMotoList[2].Position(ParNow.positions[index -1].iPosition_3);
                    lsMotoList[2].GoPosition();
                    lsMotoList[3].Position(ParNow.positions[index-1 ].iPosition_4);
                    lsMotoList[3].GoPosition();

                    break;
                case UISpace.UIClick.Set:
                    ParNow.positions[index - 1].iPosition_1 = lsMotoList[0].MotorPositionNow;
                    ParNow.positions[index - 1].iPosition_2 = lsMotoList[1].MotorPositionNow;
                    ParNow.positions[index - 1].iPosition_3 = lsMotoList[2].MotorPositionNow;
                    ParNow.positions[index - 1].iPosition_4 = lsMotoList[3].MotorPositionNow;

                    UI.Updata(ParNow.positions[index - 1]);
                    break;
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            savepar();
        }

        private void BtnParGo_Click(object sender, EventArgs e)
        {
           
            lsMotoList[4].Position(ParNow.iPosition_5);
            lsMotoList[4].GoPosition();
            lsMotoList[5].Position(ParNow.iPosition_6);
            lsMotoList[5].GoPosition();
            if (iMotoCount > 6)
            {
                lsMotoList[6].Position(ParNow.iPosition_7);
                lsMotoList[6].GoPosition();
            }
            if (iMotoCount > 7)
            {
                lsMotoList[7].Position(ParNow.iPosition_8);
            lsMotoList[7].GoPosition();
            }
            if (iMotoCount > 8)
            {
                lsMotoList[8].Position(ParNow.iPosition_9);
                lsMotoList[8].GoPosition();
            }


         // bool isok=  lsMotoList[8].MotorStateNow.ISGoOK;
        }

        private void BtnParGetSet_Click(object sender, EventArgs e)
        {
            ParNow.Name = tbParName.Text;


            ParNow.iPosition_5 = lsMotoList[4].MotorPositionNow;
            ParNow.iPosition_6 = lsMotoList[5].MotorPositionNow;
        
            if (iMotoCount > 6)
            ParNow.iPosition_7 = lsMotoList[6].MotorPositionNow;
            if (iMotoCount > 7)
                ParNow.iPosition_8 = lsMotoList[7].MotorPositionNow;
            if (iMotoCount > 8)
                ParNow.iPosition_9 = lsMotoList[8].MotorPositionNow;

            savepar();
        }

        private void BtnParSave_Click(object sender, EventArgs e)
        {
            ParNow.Name = tbParName.Text;
            savepar();

            MessageBox.Show(ParNow.Name + " 保存成功!");
        }

        private void BtnParCut_Click(object sender, EventArgs e)
        {
            if (myParList.Count > 1)
            {
                int iselectindex = lbParList.SelectedIndex;
                myParList.RemoveAt(iselectindex);

                myParList[0].isSelect = true;
                savepar();
            }
            else
            {
                MessageBox.Show("最少要保留一个参数!");
            }
        }

        private void BtnParADD_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < myParList.Count; i++)
                myParList[i].isSelect = false;


            ParMotoPosition par = new ParMotoPosition();
            par.Name = "NewName";
            par.positions = new List<PositionFor>();
            for (int i = 0; i < 4; i++)
            {
                PositionFor position = new PositionFor();
                position.iPosition_1 = 0;
                position.iPosition_2 = 0;
                position.iPosition_3 = 0;
                position.iPosition_4 = 0;
                par.positions.Add(position);
            }
            par.iPosition_5 = 0;
            par.iPosition_6 = 0;
            par.iPosition_7 = 0;
            par.iPosition_8 = 0;
            par.iPosition_9 = 0;
            par.isSelect = true;
            myParList.Add(par);
            

            savepar();
        }

        private void LbParList_SelectedIndexChanged(object sender, EventArgs e)
        {
           for(int i=0;i<myParList.Count;i++)
            {
                myParList[i].isSelect = false;
                if (i==lbParList.SelectedIndex)
                {
                    myParList[i].isSelect = true;
                    ParNow = myParList[i];
                    tbParName.Text = ParNow.Name;
                    tbParPosition5.Text = ParNow.iPosition_5.ToString();
                    tbParPosition6.Text = ParNow.iPosition_6.ToString();
                    tbParPosition7.Text = ParNow.iPosition_7.ToString();
                    tbParPosition8.Text = ParNow.iPosition_8.ToString();
                    tbParPosition9.Text = ParNow.iPosition_9.ToString();
                    if (ParNow.positions == null)
                        ParNow.positions = new List<PositionFor>();

                    if ( ParNow.positions.Count<1)
                    {
                        for (int k = 0; k < 4; k++)
                        {
                            PositionFor position = new PositionFor();
                            position.iPosition_1 = 0;
                            position.iPosition_2 = 0;
                            position.iPosition_3 = 0;
                            position.iPosition_4 = 0;
                            ParNow.positions.Add(position);
                        }
                    }

                    for (int j = 0; j < myPositionlist.Count; j++)
                    {
                        if (ParNow.positions.Count == j)
                            ParNow.positions.Add(new PositionFor());

                        myPositionlist[j].Updata(ParNow.positions[j]);
                      
                    }
                }
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            rtbResult.Text = "";
        }

        private void Motor_TRIGGERMESSIGE(object sender, BaseSpace.LS_MODBUS_MOTOR.MESSIGEEventArgs e)
        {
            switch (e.MyType)
            {
                case BaseSpace.LS_MODBUS_MOTOR.MESSIGEEventArgs.MessageType.Warning:
                    LogWarning(e.MyMessage);
                    break;
                case BaseSpace.LS_MODBUS_MOTOR.MESSIGEEventArgs.MessageType.Message:
                    LogMessage(e.MyMessage);
                    break;
                case BaseSpace.LS_MODBUS_MOTOR.MESSIGEEventArgs.MessageType.Error:
                    LogError(e.MyMessage);
                    break;
            }
        }

        private void MCom_TRIGGERMESSIGE(object sender, BaseSpace.SERIALPORT.MESSIGEEventArgs e)
        {
             BaseSpace.SERIALPORT Com = (BaseSpace.SERIALPORT)sender;
            switch (e.MyType)
            {
                case BaseSpace.SERIALPORT.MESSIGEEventArgs.MessageType.Warning:
                    LogWarning(e.MyMessage);
                    if (System.Windows.Forms.MessageBox.Show(e.MyMessage + "是否重新连接",
                       "Error", System.Windows.Forms.MessageBoxButtons.OKCancel)
                       == System.Windows.Forms.DialogResult.OK)
                    {
                        Com.ResetCOM();
                    }
                    else
                        this.Close();
                    break;
                case BaseSpace.SERIALPORT.MESSIGEEventArgs.MessageType.Message:
                    LogMessage(e.MyMessage);
                    break;
                case BaseSpace.SERIALPORT.MESSIGEEventArgs.MessageType.Error:
                    LogError(e.MyMessage);
                    break;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Tick();
        }

        public void Tick()
        {
            //  watch.Start();
            mCom.Tick();
            //mCom2.Tick();

            for (int i = 0; i < lsMotoList.Count; i++)
            {
                lsMotoControlList[i].Tick();
                lsMotoList[i].Tick();
            }

            watch.Stop();
            this.Text = mCom.mSerial.PortName + " 每秒通信: " + mCom.iCount.ToString() + " " +
              //  mCom2.mSerial.PortName + " 每秒通信: " + mCom2.iCount.ToString() +
                " 刷新间隔:" + (watch.ElapsedMilliseconds).ToString();
            watch.Restart();
        }
        

        #region 日志记录、支持其他线程访问 
        public delegate void LogAppendDelegate(Color color, string text);
        /// <summary> 
        /// 追加显示文本 
        /// </summary> 
        /// <param name="color">文本颜色</param> 
        /// <param name="text">显示文本</param> 
        private void LogAppend(Color color, string text)
        {

            rtbResult.SelectionColor = color;
            rtbResult.AppendText(text);
            rtbResult.AppendText("\n");
            rtbResult.HideSelection = false;
            // rtbMess.Select(rtbMess.Text.Length, 0);
        }
        /// <summary> 
        /// 显示错误日志 
        /// </summary> 
        /// <param name="text"></param> 
        public void LogError(string text)
        {
            LogAppendDelegate la = new LogAppendDelegate(LogAppend);
            rtbResult.Invoke(la, Color.Red,  text);
        }
        /// <summary> 
        /// 显示警告信息 
        /// </summary> 
        /// <param name="text"></param> 
        public void LogWarning(string text)
        {
            LogAppendDelegate la = new LogAppendDelegate(LogAppend);
            rtbResult.Invoke(la, Color.Violet, text);
        }
        /// <summary> 
        /// 显示信息 
        /// </summary> 
        /// <param name="text"></param> 
        public void LogMessage(string text)
        {
            LogAppendDelegate la = new LogAppendDelegate(LogAppend);
            rtbResult.Invoke(la, Color.Black, text);
        }

        #endregion

        /// <summary>
        /// 更新参数列表
        /// </summary>
        void UpdateNameList()
        {
            lbParList.Items.Clear();
            for (int i = 0; i < myParList.Count; i++)
            {
                lbParList.Items.Add(myParList[i].Name);
                if (myParList[i].isSelect)
                {
                    lbParList.SelectedIndex = i;
                }
            }
        }
    
        /// <summary>
        /// 载入参数
        /// </summary>
        void loadin()
        {
            try
            {
                //    string strADD = System.AppDomain.CurrentDomain.BaseDirectory;
                string strADD = Path;
                ArrayList array = new ArrayList();
                Read(out array, strADD + "myPar.vv");

                if (array.Count > 0)
                {
                    myParList = (List<ParMotoPosition>)array[0];
                }
                else
                {
                    ParMotoPosition par = new ParMotoPosition();
                    par.Name = "NewName";
                    par.positions = new List<PositionFor>();
                    for (int i = 0; i < 4; i++)
                    {
                        PositionFor position = new PositionFor();
                        position.iPosition_1 = 0;
                        position.iPosition_2 = 0;
                        position.iPosition_3 = 0;
                        position.iPosition_4 = 0;
                        par.positions.Add(position);
                    }
                    par.iPosition_5 = 0;
                    par.iPosition_6 = 0;
                    par.iPosition_7 = 0;
                    par.iPosition_8 = 0;
                    par.iPosition_9 = 0;
                    par.isSelect = true;
                    myParList.Add(par);
                }
                UpdateNameList();
            }
            catch
            {
                ParMotoPosition par = new ParMotoPosition();
                par.Name = "NewName";
                par.positions = new List<PositionFor>();
                for (int i = 0; i < 4; i++)
                {
                    PositionFor position = new PositionFor();
                    position.iPosition_1 = 0;
                    position.iPosition_2 = 0;
                    position.iPosition_3 = 0;
                    position.iPosition_4 = 0;
                    par.positions.Add(position);
                }
                par.iPosition_5 = 0;
                par.iPosition_6 = 0;
                par.iPosition_7 = 0;
                par.iPosition_8 = 0;
                par.iPosition_9 = 0;
                par.isSelect = true;
                myParList.Add(par);

                savepar();
            }
        }
        /// <summary>
        /// 保存参数
        /// </summary>
        void savepar()
        {
         //   string strADD = System.AppDomain.CurrentDomain.BaseDirectory;
            string strADD = Path;
            ArrayList array = new ArrayList();
            array.Add(myParList);

            Write ( array, strADD + "myPar.vv");
            UpdateNameList();
        }

        /// <summary>
        /// 读出参数
        /// </summary>
        /// <param name="myArray">out 传入的集合</param>
        /// <param name="st_File">读哪个文件</param>
        /// <returns></returns>
        public bool Read(out ArrayList myArray, string st_File)
        {
            try
            {
                System.Runtime.Serialization.IFormatter formater = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                Stream stream = new FileStream(st_File, FileMode.Open);
                myArray = (ArrayList)formater.Deserialize(stream);

                stream.Close();
                stream.Dispose();

                GC.Collect();//强制进行拉圾回收
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                myArray = null;
                GC.Collect();//强制进行拉圾回收
                return false;
            }

        }
        /// <summary>
        /// 记录参数
        /// </summary>
        /// <param name="mylist">需记录的集合</param>
        /// <param name="st_File">存放的路径</param>
        /// <returns></returns>
        public bool Write(ArrayList mylist, string st_File)
        {
            try
            {
                FileStream fs = new FileStream(st_File, FileMode.Create);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, mylist);
                fs.Close();

                fs.Dispose();
                GC.Collect();//强制进行拉圾回收
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                GC.Collect();//强制进行拉圾回收
                return false;
            }
        }
        
    }
    [Serializable]
    public class ParMotoPosition
    {
        /// <summary>
        /// 是否选中了
        /// </summary>
        public bool isSelect { get; set; }
        /// <summary>
        /// 参数名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 跑线位置
        /// </summary>
        public List<PositionFor> positions = new List<PositionFor>();


        /// <summary>
        /// 5轴(镭雕)位置
        /// </summary>
        public int iPosition_5 { get; set; }
        /// <summary>
        /// 6轴(后推)位置
        /// </summary>
        public int iPosition_6 { get; set; }
        /// <summary>
        /// 7轴(左推)位置
        /// </summary>
        public int iPosition_7 { get; set; }
        /// <summary>
        /// 8轴(前推)位置
        /// </summary>
        public int iPosition_8 { get; set; }
        /// <summary>
        /// 9轴(右推)位置
        /// </summary>
        public int iPosition_9 { get; set; }
    }
    [Serializable]
    public  class PositionFor
    {
        /// <summary>
        /// 1轴(后X)位置
        /// </summary>
        public int iPosition_1 { get; set; }
        /// <summary>
        /// 2轴(后Y)位置
        /// </summary>
        public int iPosition_2 { get; set; }
        /// <summary>
        /// 3轴(前X)位置
        /// </summary>
        public int iPosition_3 { get; set; }
        /// <summary>
        /// 4轴(前Y)位置
        /// </summary>
        public int iPosition_4 { get; set; }
    }
}
