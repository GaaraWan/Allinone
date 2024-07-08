using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace MFApiDemo
{
    public partial class RobotHCFAForm : Form
    {
        private IntPtr handle = IntPtr.Zero;
        MFApi.Motion.MOTION_INFO info = new MFApi.Motion.MOTION_INFO();
        MFApi.Motion.CFG cfg = new MFApi.Motion.CFG();
        MFApi.IO.IO_INFO io_info = new MFApi.IO.IO_INFO();
        MFApi.SYS.REGISTER_MULTY_DATA temp = new MFApi.SYS.REGISTER_MULTY_DATA();

        const int JOG_DIC_NUM = 4;
        int jogType = 0;
        int[] jogDic = new int[JOG_DIC_NUM];

        int m_userid = 0;

        System.Windows.Forms.Timer timer = null;
        System.Windows.Forms.Timer timer1 = null;
        public RobotHCFAForm(IntPtr ehandle,int euserid=-1)
        {
            handle = ehandle;
            m_userid = euserid;
            InitializeComponent();
            Init();
            this.FormClosed += RobotHCFAForm_FormClosed;
        }

        private void RobotHCFAForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            JetEazy.Universal.IsRobotFormOpen = false;

            if(timer != null)
            {
                timer.Stop();
                timer = null;
            }
            if (timer1 != null)
            {
                timer1.Stop();
                timer1 = null;
            }

        }

        void Init()
        {
            groupBox1.Visible = false;
            groupBox2.Visible = false;
            groupBox3.Visible = false;
            groupBox4.Visible = false;
            groupBox5.Visible = false;
            groupBox6.Visible = false;
            groupBox7.Visible = false;
            groupBox10.Visible = false;
            //groupBox11.Visible = false;

            jogType = 0;
            for (int i = 0; i < JOG_DIC_NUM; i++)
            {
                jogDic[i] = 0;
            }
            initTable();
            this.connect.Text = "未连接";
            timer = new System.Windows.Forms.Timer();
            timer.Tick += new EventHandler(updateInfo);
            timer.Interval = 200;
            timer.Start();
            timer1 = new System.Windows.Forms.Timer();
            timer1.Tick += new EventHandler(updataJog);
            timer1.Interval = 100;
            timer1.Start();

            btnStopMotion.Click += BtnStopMotion_Click;

            if (handle != IntPtr.Zero)
            {
                bool ret = MFApi.Motion.getCFG(handle, ref cfg);
                if (ret == true)
                {
                    this.L1.Text = String.Format("杆长1:{0:0.000}毫米", cfg.robot.link_length[0] * 1000.0);
                    this.L2.Text = String.Format("杆长2:{0:0.000}毫米", cfg.robot.link_length[1] * 1000.0);
                    this.gear1.Text = String.Format("减速比1:{0:0.000}", cfg.robot.joint[0].gear_ratio);
                    this.gear2.Text = String.Format("减速比2:{0:0.000}", cfg.robot.joint[1].gear_ratio);
                    this.gear3.Text = String.Format("减速比3:{0:0.000}", cfg.robot.joint[2].gear_ratio);
                    this.gear4.Text = String.Format("减速比4:{0:0.000}", cfg.robot.joint[3].gear_ratio);
                }
            }
        }

        private void BtnStopMotion_Click(object sender, EventArgs e)
        {
            MFApi.Motion.motionStop(handle, 0, true);
        }

        private void initTable()
        {
            DataTable mydt1 = new DataTable();
            mydt1.Columns.Add("name", Type.GetType("System.String"));
            for (int i = 0; i < 16; i++)
            {
                mydt1.Columns.Add(string.Format("{0:D}", i + 1), Type.GetType("System.String"));
            }
            string[] names1 = { "SYS DI", "SYS DO", "USER DI", "USER DO", "AI", "AO" };
            for (int i = 0; i < names1.Length; i++)
            {
                DataRow myRow = mydt1.NewRow();
                myRow[0] = names1[i];
                for (int j = 0; j < 16; j++)
                {
                    myRow[j + 1] = "0";
                }
                mydt1.Rows.Add(myRow);
            }
            this.tableIO.DataSource = mydt1;

            DataTable mydt2 = new DataTable();
            for (int i = 0; i < 10; i++)
            {
                mydt2.Columns.Add(string.Format("{0:D}", i), Type.GetType("System.String"));
            }
            for (int i = 0; i < 10; i++)
            {
                DataRow myRow = mydt2.NewRow();
                for (int j = 0; j < 10; j++)
                {
                    myRow[j] = "0";
                }
                mydt2.Rows.Add(myRow);
            }
            this.tableSR.DataSource = mydt2;

            DataTable mydt3 = new DataTable();
            for (int i = 0; i < 10; i++)
            {
                mydt3.Columns.Add(string.Format("{0:D}", i), Type.GetType("System.String"));
            }
            for (int i = 0; i < 10; i++)
            {
                DataRow myRow = mydt3.NewRow();
                for (int j = 0; j < 10; j++)
                {
                    myRow[j] = "0";
                }
                mydt3.Rows.Add(myRow);
            }
            this.tableR.DataSource = mydt3;
        }

        private void updataJog(object sender, EventArgs e)
        {
            if (jogType == 1)//关节点动
            {
                for (int i = 0; i < JOG_DIC_NUM; i++)
                {
                    if (jogDic[i] != 0)
                    {
                        MFApi.Motion.LOAD load = new MFApi.Motion.LOAD();
                        //speed:move speed in percent, range from 0.0001(0.01%) to 1(100%)
                        MFApi.Motion.jogJoint(handle, 0, i, load, 0.25 * jogDic[i]);
                        break;
                    }
                }
            }
            else if (jogType == 2)//空间点动
            {
                if (jogDic[0] != 0)
                {
                    //speed:move speed in m/s
                    MFApi.Motion.jogLine(handle, 0, MFApi.Motion.JOG_TYPE.JOG_USER_X, m_userid, -1, -1, 0.25 * jogDic[0]);
                }
                else if (jogDic[1] != 0)
                {
                    //speed:move speed in m/s
                    MFApi.Motion.jogLine(handle, 0, MFApi.Motion.JOG_TYPE.JOG_USER_Y, m_userid, -1, -1, 0.25 * jogDic[1]);
                }
                else if (jogDic[2] != 0)
                {
                    //speed:move speed in m/s
                    MFApi.Motion.jogLine(handle, 0, MFApi.Motion.JOG_TYPE.JOG_USER_Z, m_userid, -1, -1, 0.25 * jogDic[2]);
                }
                else if (jogDic[3] != 0)
                {
                    //speed:move speed in rad/s
                    MFApi.Motion.jogRotation(handle, 0, MFApi.Motion.JOG_TYPE.JOG_USER_Z, m_userid, -1, -1, 0.25 * jogDic[3]);
                }
            }
        }

        private void updateInfo(object sender, EventArgs e)
        {
            if (handle == IntPtr.Zero)
            {
                this.connect.Text = "未连接";
                //handle = MFApi.Connection.connect("192.168.1.220");
                ////handle = MFApi.Connection.connect("127.0.0.1");
                //if (handle != IntPtr.Zero)
                //{
                //    bool ret = MFApi.Motion.getCFG(handle, ref cfg);
                //    if (ret == true)
                //    {
                //        this.L1.Text = String.Format("杆长1:{0:0.000}毫米", cfg.robot.link_length[0] * 1000.0);
                //        this.L2.Text = String.Format("杆长2:{0:0.000}毫米", cfg.robot.link_length[1] * 1000.0);
                //        this.gear1.Text = String.Format("减速比1:{0:0.000}", cfg.robot.joint[0].gear_ratio);
                //        this.gear2.Text = String.Format("减速比2:{0:0.000}", cfg.robot.joint[1].gear_ratio);
                //        this.gear3.Text = String.Format("减速比3:{0:0.000}", cfg.robot.joint[2].gear_ratio);
                //        this.gear4.Text = String.Format("减速比4:{0:0.000}", cfg.robot.joint[3].gear_ratio);
                //    }
                //}
            }
            else
            {
                this.connect.Text = "已连接";
                MFApi.Motion.ROB_JOINT joint = new MFApi.Motion.ROB_JOINT();
                MFApi.Motion.EXT_JOINT ext = new MFApi.Motion.EXT_JOINT();
                MFApi.Motion.BASE_JOINT base_ = new MFApi.Motion.BASE_JOINT();
                MFApi.Motion.FRAME frame = new MFApi.Motion.FRAME();
                int config = 0;
                bool ret1 = MFApi.Motion.getCurJoint(handle, 0, m_userid, -1, ref joint, ref ext, ref base_, ref frame, ref config);
                if (ret1 == true)
                {
                    this.X.Text = String.Format("X:{0:0.000}毫米", frame.pos[0] * 1000);
                    this.Y.Text = String.Format("Y:{0:0.000}毫米", frame.pos[1] * 1000);
                    this.Z.Text = String.Format("Z:{0:0.000}毫米", frame.pos[2] * 1000);
                    this.C.Text = String.Format("C:{0:0.000}度", frame.rot[2] * 180 / Math.PI);
                }
                bool ret2 = MFApi.Motion.getMotionInfo(handle, 0, ref info);
                if (ret2 == true)
                {
                    this.joint1.Text = String.Format("关节1:{0:0.000}度", info.rob_joint[0].joint_pos * 180 / Math.PI);
                    this.joint2.Text = String.Format("关节2:{0:0.000}度", info.rob_joint[1].joint_pos * 180 / Math.PI);
                    this.joint3.Text = String.Format("关节3:{0:0.000}毫米", info.rob_joint[2].joint_pos * 1000);
                    this.joint4.Text = String.Format("关节4:{0:0.000}度", info.rob_joint[3].joint_pos * 180 / Math.PI);
                    this.encoder1.Text = String.Format("编码器1:{0:D}", info.rob_joint[0].motor_fbk_apos);
                    this.encoder2.Text = String.Format("编码器2:{0:D}", info.rob_joint[1].motor_fbk_apos);
                    this.encoder3.Text = String.Format("编码器3:{0:D}", info.rob_joint[2].motor_fbk_apos);
                    this.encoder4.Text = String.Format("编码器4:{0:D}", info.rob_joint[3].motor_fbk_apos);
                    this.single1.Text = String.Format("单圈值1:{0:D}", info.rob_joint[0].motor_fbk_single_cycle_value);
                    this.single2.Text = String.Format("单圈值2:{0:D}", info.rob_joint[1].motor_fbk_single_cycle_value);
                    this.single3.Text = String.Format("单圈值3:{0:D}", info.rob_joint[2].motor_fbk_single_cycle_value);
                    this.single4.Text = String.Format("单圈值4:{0:D}", info.rob_joint[3].motor_fbk_single_cycle_value);
                    this.multy1.Text = String.Format("多圈值1:{0:D}", info.rob_joint[0].motor_fbk_multi_cycle_value);
                    this.multy2.Text = String.Format("多圈值2:{0:D}", info.rob_joint[1].motor_fbk_multi_cycle_value);
                    this.multy3.Text = String.Format("多圈值3:{0:D}", info.rob_joint[2].motor_fbk_multi_cycle_value);
                    this.multy4.Text = String.Format("多圈值4:{0:D}", info.rob_joint[3].motor_fbk_multi_cycle_value);
                    if (info.state >= MFApi.Motion.MOTION_STATE.STATE_ENABLING)
                    {
                        this.enable.Text = "下使能";
                    }
                    else
                    {
                        this.enable.Text = "上使能";
                    }
                    this.inputVel.Value = (decimal)(info.vel_scale * 100.0);
                }
                bool ret3 = MFApi.IO.getIOInfo(handle, ref io_info);
                if (ret3 == true)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        setTable(this.tableIO, i + 1, 0, io_info.sys_di[i] ? "1" : "0");
                        setTable(this.tableIO, i + 1, 1, io_info.sys_do[i] ? "1" : "0");
                        setTable(this.tableIO, i + 1, 2, io_info.user_di[i] ? "1" : "0");
                        setTable(this.tableIO, i + 1, 3, io_info.user_do[i] ? "1" : "0");
                        setTable(this.tableIO, i + 1, 4, String.Format("{0:D}", io_info.ai[i]));
                        setTable(this.tableIO, i + 1, 5, String.Format("{0:D}", io_info.ao[i]));
                    }
                }
                bool ret4 = MFApi.SYS.readMultiSysRegister(handle, 0, ref temp);
                if (ret4 == true)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            setTable(this.tableSR, j, i, String.Format("{0:D}", temp.data[i * 10 + j]));
                        }
                    }
                }
                bool ret5 = MFApi.SYS.readMultiUserRegister(handle, 0, ref temp);
                if (ret5 == true)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            setTable(this.tableR, j, i, String.Format("{0:D}", temp.data[i * 10 + j]));
                        }
                    }
                }
                if (ret1 == false && ret2 == false)
                {
                    this.connect.Text = "未连接";
                    MFApi.Connection.disConnect(handle);
                    handle = IntPtr.Zero;
                }
            }
        }

        private void setTable(DataGridView view, int col, int row, string str)
        {
            if (view[col, row].Value.ToString() != str)
            {
                view[col, row].Value = str;
            }
        }

        private void start_Click(object sender, EventArgs e)
        {
            MFApi.Motion.MOTOR_ZERO zero = new MFApi.Motion.MOTOR_ZERO();
            //获取零位编码器值
            MFApi.Motion.getMotorZero(handle, ref zero);
            //设置零位
            //MFApi.Motion.setMotorZero(handle, zero);
            MFApi.Motion.CFG cfg = new MFApi.Motion.CFG();
            //获取机器人参数
            MFApi.Motion.getCFG(handle, ref cfg);
            //cfg.robot.link_length[0] = 10;//L1
            //cfg.robot.link_length[1] = 10;//L2
            //cfg.robot.joint[0].gear_ratio = XXX;//J1减速比
            //cfg.robot.joint[1].gear_ratio = XXX;//J2减速比
            //cfg.robot.joint[2].gear_ratio = XXX;//J3减速比
            //cfg.robot.joint[3].gear_ratio = XXX;//J4减速比
            //设置机器人参数
            //ret = MFApi.Motion.setCFG(handle, cfg);
        }

        private void enable_Click(object sender, EventArgs e)
        {
            if (handle != IntPtr.Zero)
            {
                //自动模式下上下使能
                if (info.state >= MFApi.Motion.MOTION_STATE.STATE_ENABLING)
                {
                    MFApi.Motion.autoDisable(handle, 0);
                }
                else
                {
                    MFApi.Motion.autoEnable(handle, 0);
                }
            }
        }

        private void goto_joint_MouseDown(object sender, MouseEventArgs e)
        {
            MFApi.Motion.getReady(handle, 0, true);
            IntPtr a = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MFApi.Motion.JOINT_TARGET)));
            MFApi.Motion.JOINT_TARGET to_joint = (MFApi.Motion.JOINT_TARGET)Marshal.PtrToStructure(a, typeof(MFApi.Motion.JOINT_TARGET));
            to_joint.rob.datas[0] = (double)this.inputJ1.Value * Math.PI / 180;
            to_joint.rob.datas[1] = (double)this.inputJ2.Value * Math.PI / 180;
            to_joint.rob.datas[2] = (double)this.inputJ3.Value / 1000;
            to_joint.rob.datas[3] = (double)this.inputJ4.Value * Math.PI / 180;
            to_joint.rob.datas[4] = 0;
            to_joint.rob.datas[5] = 0;
            to_joint.ext.datas[0] = 0;
            to_joint.ext.datas[1] = 0;
            to_joint.ext.datas[2] = 0;
            to_joint.ext.datas[3] = 0;
            to_joint.ext.datas[4] = 0;
            to_joint.ext.datas[5] = 0;
            to_joint.base_.datas[0] = 0;
            to_joint.base_.datas[1] = 0;
            to_joint.base_.datas[2] = 0;
            MFApi.Motion.LOAD load = new MFApi.Motion.LOAD();
            MFApi.Motion.MOVE_ADDIN addin = new MFApi.Motion.MOVE_ADDIN();
            //speed:move speed in percent, range from 0.0001(0.01%) to 1(100%)
            MFApi.Motion.moveJoint(handle, 0, to_joint, load, 0.25, 0, addin);
            Marshal.FreeHGlobal(a);
        }

        private void goto_joint_MouseUp(object sender, MouseEventArgs e)
        {
            MFApi.Motion.motionStop(handle, 0, true);
        }

        private void goto_point_MouseDown(object sender, MouseEventArgs e)
        {
            bool bReady = MFApi.Motion.getReady(handle, 0, true);
            IntPtr a = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MFApi.Motion.FRAME_TARGET)));
            MFApi.Motion.FRAME_TARGET target = (MFApi.Motion.FRAME_TARGET)Marshal.PtrToStructure(a, typeof(MFApi.Motion.FRAME_TARGET));
            target.rob.pos[0] = (double)this.inputX.Value / 1000;
            target.rob.pos[1] = (double)this.inputY.Value / 1000;
            target.rob.pos[2] = (double)this.inputZ.Value / 1000;
            target.rob.rot[0] = 0;
            target.rob.rot[1] = 0;
            target.rob.rot[2] = (double)this.inputA.Value * Math.PI / 180;
            target.ext.datas[0] = 0;
            target.ext.datas[1] = 0;
            target.ext.datas[2] = 0;
            target.ext.datas[3] = 0;
            target.ext.datas[4] = 0;
            target.ext.datas[5] = 0;
            target.base_.datas[0] = 0;
            target.base_.datas[1] = 0;
            target.base_.datas[2] = 0;
            MFApi.Motion.MOVE_ADDIN addin = new MFApi.Motion.MOVE_ADDIN();
            //speed:move speed in percent, range from 0.0001(0.01%) to 1(100%)
            long ret = MFApi.Motion.movePoint(handle, 0, 0, target, m_userid, -1, -1, MFApi.Motion.PATH_TYPE.PATH_ON_USER, 0.25, 0, addin);
            Marshal.FreeHGlobal(a);

            //int pin = (int)65;
            //short v = (short)0;
            //MFApi.SYS.readSysRegister(handle, pin, ref v);
        }

        private void goto_point_MouseUp(object sender, MouseEventArgs e)
        {
            MFApi.Motion.motionStop(handle, 0, true);
        }

        private void J1Minus_MouseDown(object sender, MouseEventArgs e)
        {
            MFApi.Motion.getReady(handle, 0, true);
            jogType = 1;
            jogDic[0] = -1;
        }

        private void J1Minus_MouseUp(object sender, MouseEventArgs e)
        {
            jogType = 0;
            for (int i = 0; i < JOG_DIC_NUM; i++)
            {
                jogDic[i] = 0;
            }
            MFApi.Motion.motionStop(handle, 0, true);
        }

        private void J2Minus_MouseDown(object sender, MouseEventArgs e)
        {
            MFApi.Motion.getReady(handle, 0, true);
            jogType = 1;
            jogDic[1] = -1;
        }

        private void J2Minus_MouseUp(object sender, MouseEventArgs e)
        {
            jogType = 0;
            for (int i = 0; i < JOG_DIC_NUM; i++)
            {
                jogDic[i] = 0;
            }
            MFApi.Motion.motionStop(handle, 0, true);
        }

        private void J3Minus_MouseDown(object sender, MouseEventArgs e)
        {
            MFApi.Motion.getReady(handle, 0, true);
            jogType = 1;
            jogDic[2] = -1;
        }

        private void J3Minus_MouseUp(object sender, MouseEventArgs e)
        {
            jogType = 0;
            for (int i = 0; i < JOG_DIC_NUM; i++)
            {
                jogDic[i] = 0;
            }
            MFApi.Motion.motionStop(handle, 0, true);
        }

        private void J4Minus_MouseDown(object sender, MouseEventArgs e)
        {
            MFApi.Motion.getReady(handle, 0, true);
            jogType = 1;
            jogDic[3] = -1;
        }

        private void J4Minus_MouseUp(object sender, MouseEventArgs e)
        {
            jogType = 0;
            for (int i = 0; i < JOG_DIC_NUM; i++)
            {
                jogDic[i] = 0;
            }
            MFApi.Motion.motionStop(handle, 0, true);
        }

        private void J1Plus_MouseDown(object sender, MouseEventArgs e)
        {
            MFApi.Motion.getReady(handle, 0, true);
            jogType = 1;
            jogDic[0] = 1;
        }

        private void J1Plus_MouseUp(object sender, MouseEventArgs e)
        {
            jogType = 0;
            for (int i = 0; i < JOG_DIC_NUM; i++)
            {
                jogDic[i] = 0;
            }
            MFApi.Motion.motionStop(handle, 0, true);
        }

        private void J2Plus_MouseDown(object sender, MouseEventArgs e)
        {
            MFApi.Motion.getReady(handle, 0, true);
            jogType = 1;
            jogDic[1] = 1;
        }

        private void J2Plus_MouseUp(object sender, MouseEventArgs e)
        {
            jogType = 0;
            for (int i = 0; i < JOG_DIC_NUM; i++)
            {
                jogDic[i] = 0;
            }
            MFApi.Motion.motionStop(handle, 0, true);
        }

        private void J3Plus_MouseDown(object sender, MouseEventArgs e)
        {
            MFApi.Motion.getReady(handle, 0, true);
            jogType = 1;
            jogDic[2] = 1;
        }

        private void J3Plus_MouseUp(object sender, MouseEventArgs e)
        {
            jogType = 0;
            for (int i = 0; i < JOG_DIC_NUM; i++)
            {
                jogDic[i] = 0;
            }
            MFApi.Motion.motionStop(handle, 0, true);
        }

        private void J4Plus_MouseDown(object sender, MouseEventArgs e)
        {
            MFApi.Motion.getReady(handle, 0, true);
            jogType = 1;
            jogDic[3] = 1;
        }

        private void J4Plus_MouseUp(object sender, MouseEventArgs e)
        {
            jogType = 0;
            for (int i = 0; i < JOG_DIC_NUM; i++)
            {
                jogDic[i] = 0;
            }
            MFApi.Motion.motionStop(handle, 0, true);
        }

        private void XMinus_MouseDown(object sender, MouseEventArgs e)
        {
            MFApi.Motion.getReady(handle, 0, true);
            jogType = 2;
            jogDic[0] = -1;
        }

        private void XMinus_MouseUp(object sender, MouseEventArgs e)
        {
            jogType = 0;
            for (int i = 0; i < JOG_DIC_NUM; i++)
            {
                jogDic[i] = 0;
            }
            MFApi.Motion.motionStop(handle, 0, true);
        }

        private void YMinus_MouseDown(object sender, MouseEventArgs e)
        {
            MFApi.Motion.getReady(handle, 0, true);
            jogType = 2;
            jogDic[1] = -1;
        }

        private void YMinus_MouseUp(object sender, MouseEventArgs e)
        {
            jogType = 0;
            for (int i = 0; i < JOG_DIC_NUM; i++)
            {
                jogDic[i] = 0;
            }
            MFApi.Motion.motionStop(handle, 0, true);
        }

        private void ZMinus_MouseDown(object sender, MouseEventArgs e)
        {
            MFApi.Motion.getReady(handle, 0, true);
            jogType = 2;
            jogDic[2] = -1;
        }

        private void ZMinus_MouseUp(object sender, MouseEventArgs e)
        {
            jogType = 0;
            for (int i = 0; i < JOG_DIC_NUM; i++)
            {
                jogDic[i] = 0;
            }
            MFApi.Motion.motionStop(handle, 0, true);
        }

        private void CMinus_MouseDown(object sender, MouseEventArgs e)
        {
            MFApi.Motion.getReady(handle, 0, true);
            jogType = 2;
            jogDic[3] = -1;
        }

        private void CMinus_MouseUp(object sender, MouseEventArgs e)
        {
            jogType = 0;
            for (int i = 0; i < JOG_DIC_NUM; i++)
            {
                jogDic[i] = 0;
            }
            MFApi.Motion.motionStop(handle, 0, true);
        }

        private void XPlus_MouseDown(object sender, MouseEventArgs e)
        {
            MFApi.Motion.getReady(handle, 0, true);
            jogType = 2;
            jogDic[0] = 1;
        }

        private void XPlus_MouseUp(object sender, MouseEventArgs e)
        {
            jogType = 0;
            for (int i = 0; i < JOG_DIC_NUM; i++)
            {
                jogDic[i] = 0;
            }
            MFApi.Motion.motionStop(handle, 0, true);
        }

        private void YPlus_MouseDown(object sender, MouseEventArgs e)
        {
            MFApi.Motion.getReady(handle, 0, true);
            jogType = 2;
            jogDic[1] = 1;
        }

        private void YPlus_MouseUp(object sender, MouseEventArgs e)
        {
            jogType = 0;
            for (int i = 0; i < JOG_DIC_NUM; i++)
            {
                jogDic[i] = 0;
            }
            MFApi.Motion.motionStop(handle, 0, true);
        }

        private void ZPlus_MouseDown(object sender, MouseEventArgs e)
        {
            MFApi.Motion.getReady(handle, 0, true);
            jogType = 2;
            jogDic[2] = 1;
        }

        private void ZPlus_MouseUp(object sender, MouseEventArgs e)
        {
            jogType = 0;
            for (int i = 0; i < JOG_DIC_NUM; i++)
            {
                jogDic[i] = 0;
            }
            MFApi.Motion.motionStop(handle, 0, true);
        }

        private void CPlus_MouseDown(object sender, MouseEventArgs e)
        {
            MFApi.Motion.getReady(handle, 0, true);
            jogType = 2;
            jogDic[3] = 1;
        }

        private void CPlus_MouseUp(object sender, MouseEventArgs e)
        {
            jogType = 0;
            for (int i = 0; i < JOG_DIC_NUM; i++)
            {
                jogDic[i] = 0;
            }
            MFApi.Motion.motionStop(handle, 0, true);
        }

        private void inputVel_ValueChanged(object sender, EventArgs e)
        {
            double scale = (double)this.inputVel.Value / 100.0;
            MFApi.Motion.setVelScale(handle, 0, scale);
        }

        private void IOSet_Click(object sender, EventArgs e)
        {
            if (this.cbIOType.Text == "SYS DI")
            {
                int pin = (int)this.numIOPin.Value - 1;
                int v = (int)this.numIOValue.Value;
                MFApi.IO.writeSysDi(handle, pin, !(v == 0));
            }
            else if (this.cbIOType.Text == "USER DI")
            {
                int pin = (int)this.numIOPin.Value - 1;
                int v = (int)this.numIOValue.Value;
                MFApi.IO.writeDi(handle, pin, !(v == 0));
            }
            else if (this.cbIOType.Text == "USER DO")
            {
                int pin = (int)this.numIOPin.Value - 1;
                int v = (int)this.numIOValue.Value;
                MFApi.IO.writeDo(handle, pin, !(v == 0));
            }
            else if (this.cbIOType.Text == "AI")
            {
                int pin = (int)this.numIOPin.Value - 1;
                int v = (int)this.numIOValue.Value;
                MFApi.IO.writeAi(handle, pin, v);
            }
            else if (this.cbIOType.Text == "AO")
            {
                int pin = (int)this.numIOPin.Value - 1;
                int v = (int)this.numIOValue.Value;
                MFApi.IO.writeAo(handle, pin, v);
            }
        }

        private void btnRegisterSet_Click(object sender, EventArgs e)
        {
            if (this.cbRegisterType.Text == "SYSTEM")
            {
                int pin = (int)this.numRegisterPin.Value;
                short v = (short)this.numRegisterValue.Value;
                MFApi.SYS.writeSysRegister(handle, pin, v);
            }
            else if (this.cbRegisterType.Text == "USER")
            {
                int pin = (int)this.numRegisterPin.Value;
                short v = (short)this.numRegisterValue.Value;
                MFApi.SYS.writeUserRegister(handle, pin, v);
            }
        }
    }
}
