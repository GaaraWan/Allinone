using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static MFApi.Motion;

namespace JetEazy.ControlSpace.RobotSpace
{
    public class RobotHCFA : IDisposable
    {
        #region Config Access Functions
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        void WriteINIValue(string section, string key, string value, string filepath)
        {
            WritePrivateProfileString(section, key, value, filepath);
        }
        string ReadINIValue(string section, string key, string defaultvaluestring, string filepath)
        {
            string retStr = "";

            StringBuilder temp = new StringBuilder(1024);
            int Length = GetPrivateProfileString(section, key, "", temp, 1024, filepath);

            retStr = temp.ToString();

            if (retStr == "")
                retStr = defaultvaluestring;
            //else
            //    retStr = retStr.Split('/')[0]; //把說明排除掉

            return retStr;

        }
        #endregion

        MFApi.Motion.MOTION_INFO info = new MFApi.Motion.MOTION_INFO();
        MFApi.Motion.CFG cfg = new MFApi.Motion.CFG();
        MFApi.IO.IO_INFO io_info = new MFApi.IO.IO_INFO();
        MFApi.SYS.REGISTER_MULTY_DATA temp = new MFApi.SYS.REGISTER_MULTY_DATA();

        int iCount = 0;
        public int SerialCount = 0;

        protected int RetryCount = 0;
        protected int RetryIndex = 0;
        protected int Timeoutinms = 0;

        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        System.Threading.Thread m_Thread = null;
        //System.Threading.Timer m_Timer = null;
        System.Windows.Forms.Timer m_Timer = null;
        bool m_Running = false;
        bool m_error_comm = false;

        private IntPtr handle = IntPtr.Zero;
        private string m_ip = "127.0.0.1";
        private bool m_debug = false;
        private string m_msg = string.Empty;
        private int m_running = 0;
        private MOTION_STATE m_motionstate = MOTION_STATE.STATE_READY;
        private int m_errorcode = 0;

        private int m_userid = 0;
        private double m_x = 0;
        private double m_y = 0;
        private double m_z = 0;
        private double m_c = 0;
        private double m_speed = 20;
        private bool m_motionenable = false;

        public int UserId
        {
            get { return m_userid; }
            set { m_userid = value; }
        }
        public double CurrX
        {
            get { return m_x; }
        }
        public double CurrY
        { get { return m_y; } }
        public double CurrZ { get { return m_z; } }
        public double CurrC { get { return m_c; } }
        public double Speed
        {
            get { 
                return m_speed * 100.0; }
            set
            {
                m_speed = value / 100.0;
                //double scale = (double)m_speed / 100.0;
                if (handle != IntPtr.Zero)
                    MFApi.Motion.setVelScale(handle, 0, m_speed);
            }
        }
        public bool Motionenable
        {
            get { return m_motionenable; }
            set
            {
                //m_motionenable = value;
                if (handle != IntPtr.Zero)
                {
                    if(value)
                        MFApi.Motion.autoEnable(handle, 0);
                    else
                        MFApi.Motion.autoDisable(handle, 0);

                    ////自动模式下上下使能
                    //if (info.state >= MFApi.Motion.MOTION_STATE.STATE_ENABLING)
                    //{
                    //    if (!value)
                    //        MFApi.Motion.autoDisable(handle, 0);
                    //}
                    //else
                    //{
                    //    if (value)
                    //        MFApi.Motion.autoEnable(handle, 0);
                    //}
                }
            }
        }
        public void MoveTo(double x, double y, double z)
        {
            if (handle == IntPtr.Zero)
                return;

            bool bReady = MFApi.Motion.getReady(handle, 0, true);
            IntPtr a = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MFApi.Motion.FRAME_TARGET)));
            MFApi.Motion.FRAME_TARGET target = (MFApi.Motion.FRAME_TARGET)Marshal.PtrToStructure(a, typeof(MFApi.Motion.FRAME_TARGET));
            target.rob.pos[0] = (double)x / 1000;
            target.rob.pos[1] = (double)y / 1000;
            target.rob.pos[2] = (double)z / 1000;
            target.rob.rot[0] = 0;
            target.rob.rot[1] = 0;
            target.rob.rot[2] = 0;// (double)this.inputA.Value * Math.PI / 180;
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
        }
        public void ForceStop()
        {
            if (handle == IntPtr.Zero)
                return;
            MFApi.Motion.motionStop(handle, 0, true);
        }
        public bool GetDI(int pin)
        {
            if (handle == IntPtr.Zero)
                return false;
            pin = pin - 1;
            return (io_info.user_di[pin] ? "1" : "0") == "1";
        }
        public bool GetDO(int pin)
        {
            if (handle == IntPtr.Zero)
                return false;
            pin = pin - 1;
            return (io_info.user_do[pin] ? "1" : "0") == "1";
        }
        public void SetDO(int pin, bool ison)
        {
            if (handle == IntPtr.Zero)
                return;
            pin = pin - 1;
            MFApi.IO.writeDo(handle, pin, ison);
        }
        public bool RobotRunning
        {
            get { return m_running != 0; }
        }
        public string GetMotionState()
        {
            string ret = string.Empty;

//            STATE_ERROR 错误
//STATE_DISABLE 未使能
//STATE_DISABLING 掉使能中
//STATE_ENABLING 使能中
//STATE_ENABLE 使能
//STATE_READY 就绪状态，可接收运动指令
//STATE_MOVE 运动中
//STATE_STOP 停止中
//STATE_EMERGSTOP 急停
//STATE_DRAG 拖拽状态

            switch (m_motionstate)
            {
                case MOTION_STATE.STATE_ERROR:
                    ret = "错误";
                    break;
                case MOTION_STATE.STATE_DISABLE:
                    ret = "未使能";
                    break;
                case MOTION_STATE.STATE_DISABLING:
                    ret = "掉使能中";
                    break;
                case MOTION_STATE.STATE_ENABLING:
                    ret = "使能中";
                    break;
                case MOTION_STATE.STATE_ENABLE:
                    ret = "使能";
                    break;
                case MOTION_STATE.STATE_READY:
                    ret = "就绪";
                    break;
                case MOTION_STATE.STATE_MOVE:
                    ret = "运动中";
                    break;
                case MOTION_STATE.STATE_STOP:
                    ret = "停止中";
                    break;
                case MOTION_STATE.STATE_EMERGSTOP:
                    ret = "急停";
                    break;
                case MOTION_STATE.STATE_DRAG:
                    ret = "拖拽状态";
                    break;
            }

            return ret;
        }
        public int ErrorCode
        {
            get { return m_errorcode; }
        }
        public string GetErrorCode()
        {
            string ret = string.Empty;

            //            ERROR_NONE 无错误
            //ERROR_SYS 系统错误
            //ERROR_SYS_INTERNAL 系统内部错误
            //ERROR_SYS_EMERG_STOP 急停
            //ERROR_SYS_AUTO_STOP 自动停机
            //ERROR_SYS_GUARD_STOP 保护停机
            //ERROR_SYS_COL_SUP_STOP 碰撞检测停止
            //ERROR_OTHER 其他错误
            //ERROR_COL_DI_STOP 碰撞 DI 停止
            //ERROR_ECAT_ERROR ECAT 总线错误
            //ERROR_SRV_ERROR_CODE 伺服错误
            //ERROR_MOT 运动错误
            //ERROR_MOT_INTERNAL_ERROR 运动内部错误
            //ERROR_MOT_ROB_JOINT_OUT_LIMIT 机器人关节超限
            //ERROR_MOT_EXT_JOINT_OUT_LIMIT 外轴超限
            //ERROR_MOT_BASE_X_JOINT_OUT_LIMIT 基座轴 X 超限
            //ERROR_MOT_BASE_Y_JOINT_OUT_LIMIT 基座轴 Y 超限
            //ERROR_MOT_BASE_Z_JOINT_OUT_LIMIT 基座轴 Z 超限
            //ERROR_MOT_ROB_TRQ_LACK 关节扭矩不足
            //ERROR_MOT_ROB_UNREACHABLE 位置不可达
            //ERROR_MOT_ROB_SINGULARITY 机器人奇异点错误
            //ERROR_MOT_ROB_ONLY_ROT_Z 四轴机型只支持绕 Z 轴运动
            //ERROR_MOT_ROB_INVALID_CFG 无效形态配置
            //ERROR_MOT_ROB_INV_KINE_FAILED 逆运动学计算错误
            //ERROR_MOT_CNV_TRACK_FAILED_BY_FORBID_MOVE 跟踪计算错误
            //ERROR_MOT_WEAVE_FAILED 摆弧运动错误
            ret = $"错误码[{m_errorcode}] ";
            ERROR_CODE eRROR = (ERROR_CODE)m_errorcode;
            switch(eRROR)
            {
                case ERROR_CODE.ERROR_NONE:
                    ret += "无错误";
                    break;
                case ERROR_CODE.ERROR_SYS:
                    ret += "系统错误";
                    break;
                case ERROR_CODE.ERROR_SYS_INTERNAL:
                    ret += "系统内部错误";
                    break;
                case ERROR_CODE.ERROR_SYS_EMERG_STOP:
                    ret += "急停";
                    break;
                case ERROR_CODE.ERROR_SYS_AUTO_STOP:
                    ret += "自动停机";
                    break;
                case ERROR_CODE.ERROR_SYS_GUARD_STOP:
                    ret += "保护停机";
                    break;
                case ERROR_CODE.ERROR_SYS_COL_SUP_STOP:
                    ret += "碰撞检测停止";
                    break;
                case ERROR_CODE.ERROR_OTHER:
                    ret += "其他错误";
                    break;
                case ERROR_CODE.ERROR_COL_DI_STOP:
                    ret += "碰撞 DI 停止";
                    break;
                case ERROR_CODE.ERROR_ECAT_ERROR:
                    ret += "ECAT 总线错误";
                    break;
                case ERROR_CODE.ERROR_SRV_ERROR_CODE:
                    ret += "伺服错误";
                    break;
                case ERROR_CODE.ERROR_MOT:
                    ret += "运动错误";
                    break;
                case ERROR_CODE.ERROR_MOT_INTERNAL_ERROR:
                    ret += "运动内部错误";
                    break;
                case ERROR_CODE.ERROR_MOT_ROB_JOINT_OUT_LIMIT:
                    ret += "机器人关节超限";
                    break;
                case ERROR_CODE.ERROR_MOT_EXT_JOINT_OUT_LIMIT:
                    ret += "外轴超限";
                    break;
                case ERROR_CODE.ERROR_MOT_BASE_X_JOINT_OUT_LIMIT:
                    ret += "基座轴 X 超限";
                    break;
                case ERROR_CODE.ERROR_MOT_BASE_Y_JOINT_OUT_LIMIT:
                    ret += "基座轴 Y 超限";
                    break;
                case ERROR_CODE.ERROR_MOT_BASE_Z_JOINT_OUT_LIMIT:
                    ret += "基座轴 Z 超限";
                    break;
                case ERROR_CODE.ERROR_MOT_ROB_TRQ_LACK:
                    ret += "关节扭矩不足";
                    break;
                case ERROR_CODE.ERROR_MOT_ROB_UNREACHABLE:
                    ret += "位置不可达";
                    break;
                case ERROR_CODE.ERROR_MOT_ROB_SINGULARITY:
                    ret += "机器人奇异点错误";
                    break;
                case ERROR_CODE.ERROR_MOT_ROB_ONLY_ROT_Z:
                    ret += "四轴机型只支持绕 Z 轴运动";
                    break;
                case ERROR_CODE.ERROR_MOT_ROB_INVALID_CFG:
                    ret += "无效形态配置";
                    break;
                case ERROR_CODE.ERROR_MOT_ROB_INV_KINE_FAILED:
                    ret += "逆运动学计算错误";
                    break;
                case ERROR_CODE.ERROR_MOT_CNV_TRACK_FAILED_BY_FORBID_MOVE:
                    ret += "跟踪计算错误";
                    break;
                case ERROR_CODE.ERROR_MOT_WEAVE_FAILED:
                    ret += "摆弧运动错误";
                    break;
            }

            return ret;
        }

        public RobotHCFA()
        {

        }
        public IntPtr ApiHandle
        {
            get { return handle; }
        }
        public bool IsDebug
        {
            get { return m_debug; }
        }
        public bool Init(string eFilename, bool eDebug = false)
        {
            bool bOK = false;

            m_debug = eDebug;
            m_ip = ReadINIValue("Communication", "IP", m_ip, eFilename);

            RetryCount = int.Parse(ReadINIValue("Other", "Retry", RetryCount.ToString(), eFilename));
            Timeoutinms = int.Parse(ReadINIValue("Other", "Timeout(ms)", Timeoutinms.ToString(), eFilename));

            if (m_debug)
                bOK = true;
            else
            {
                handle = MFApi.Connection.connect(m_ip);
                bOK = handle != IntPtr.Zero;
            }

            if (bOK && !m_debug)
            {
                //if (m_Thread == null)
                //{
                //    m_Running = true;
                //    m_Thread = new System.Threading.Thread(new System.Threading.ThreadStart(_Scan));
                //    m_Thread.Priority = System.Threading.ThreadPriority.Normal;
                //    m_Thread.IsBackground = true;
                //    m_Thread.Start();
                //}
                if(m_Timer == null)
                {
                    m_Running = true;
                    m_Timer = new System.Windows.Forms.Timer();
                    m_Timer.Interval = 10;
                    m_Timer.Enabled = true;
                    m_Timer.Tick += M_Timer_Tick;
                }
            }

            return bOK;
        }
        public bool RetryConnect()
        {
            bool bOK = false;
            if (m_debug)
                bOK = true;
            else
            {
                if(SerialCount == 0)
                {
                    handle = MFApi.Connection.connect(m_ip);
                    bOK = handle != IntPtr.Zero;
                }
                else
                {
                    bOK = true;
                }
            }
            return bOK;
        }

        private void M_Timer_Tick(object sender, EventArgs e)
        {
            Tick();
        }

        public void Dispose()
        {
            m_Running = false;
            if (m_debug)
                return;
            if (handle != IntPtr.Zero)
            {
                MFApi.Connection.disConnect(handle);
                handle = IntPtr.Zero;
            }
            if (m_Thread != null)
            {
                //if (m_Thread_Hsl.ThreadState != System.Threading.ThreadState.Stopped)
                {
                    m_Thread.Abort();
                    m_Thread = null;
                }
            }
            if (m_Timer != null)
            {
                //if (m_Thread_Hsl.ThreadState != System.Threading.ThreadState.Stopped)
                {
                    m_Timer.Enabled = false;
                    m_Timer = null;
                }
            }
        }
        public void Tick()
        {
            if (handle == IntPtr.Zero)
            {
                m_msg = "未连接";
                SerialCount = 0;
            }
            else
            {
                if (!watch.IsRunning)
                    watch.Start();
                if (watch.ElapsedMilliseconds > 1000)
                {
                    watch.Reset();
                    SerialCount = iCount;
                    iCount = 0;
                }
                else
                    iCount++;

                m_msg = "已连接"; 
                MFApi.Motion.ROB_JOINT joint = new MFApi.Motion.ROB_JOINT();
                MFApi.Motion.EXT_JOINT ext = new MFApi.Motion.EXT_JOINT();
                MFApi.Motion.BASE_JOINT base_ = new MFApi.Motion.BASE_JOINT();
                MFApi.Motion.FRAME frame = new MFApi.Motion.FRAME();
                int config = 0;
                bool ret1 = MFApi.Motion.getCurJoint(handle, 0, m_userid, -1, ref joint, ref ext, ref base_, ref frame, ref config);
                if (ret1 == true)
                {
                    m_x = frame.pos[0] * 1000;
                    m_y = frame.pos[1] * 1000;
                    m_z = frame.pos[2] * 1000;
                    m_c = frame.rot[2] * 180 / Math.PI;
                    //this.X.Text = String.Format("X:{0:0.000}毫米", frame.pos[0] * 1000);
                    //this.Y.Text = String.Format("Y:{0:0.000}毫米", frame.pos[1] * 1000);
                    //this.Z.Text = String.Format("Z:{0:0.000}毫米", frame.pos[2] * 1000);
                    //this.C.Text = String.Format("C:{0:0.000}度", frame.rot[2] * 180 / Math.PI);
                }
                bool ret2 = MFApi.Motion.getMotionInfo(handle, 0, ref info);
                if (ret2 == true)
                {
                    m_running = info.is_moving;
                    m_motionstate = info.state;
                    m_errorcode = (int)info.error_code;
                    //this.joint1.Text = String.Format("关节1:{0:0.000}度", info.rob_joint[0].joint_pos * 180 / Math.PI);
                    //this.joint2.Text = String.Format("关节2:{0:0.000}度", info.rob_joint[1].joint_pos * 180 / Math.PI);
                    //this.joint3.Text = String.Format("关节3:{0:0.000}毫米", info.rob_joint[2].joint_pos * 1000);
                    //this.joint4.Text = String.Format("关节4:{0:0.000}度", info.rob_joint[3].joint_pos * 180 / Math.PI);
                    //this.encoder1.Text = String.Format("编码器1:{0:D}", info.rob_joint[0].motor_fbk_apos);
                    //this.encoder2.Text = String.Format("编码器2:{0:D}", info.rob_joint[1].motor_fbk_apos);
                    //this.encoder3.Text = String.Format("编码器3:{0:D}", info.rob_joint[2].motor_fbk_apos);
                    //this.encoder4.Text = String.Format("编码器4:{0:D}", info.rob_joint[3].motor_fbk_apos);
                    //this.single1.Text = String.Format("单圈值1:{0:D}", info.rob_joint[0].motor_fbk_single_cycle_value);
                    //this.single2.Text = String.Format("单圈值2:{0:D}", info.rob_joint[1].motor_fbk_single_cycle_value);
                    //this.single3.Text = String.Format("单圈值3:{0:D}", info.rob_joint[2].motor_fbk_single_cycle_value);
                    //this.single4.Text = String.Format("单圈值4:{0:D}", info.rob_joint[3].motor_fbk_single_cycle_value);
                    //this.multy1.Text = String.Format("多圈值1:{0:D}", info.rob_joint[0].motor_fbk_multi_cycle_value);
                    //this.multy2.Text = String.Format("多圈值2:{0:D}", info.rob_joint[1].motor_fbk_multi_cycle_value);
                    //this.multy3.Text = String.Format("多圈值3:{0:D}", info.rob_joint[2].motor_fbk_multi_cycle_value);
                    //this.multy4.Text = String.Format("多圈值4:{0:D}", info.rob_joint[3].motor_fbk_multi_cycle_value);
                    if (info.state >= MFApi.Motion.MOTION_STATE.STATE_ENABLING)
                    {
                        m_motionenable = true;
                        //this.enable.Text = "下使能";
                    }
                    else
                    {
                        m_motionenable = false;
                        //this.enable.Text = "上使能";
                    }
                    //m_speed = info.vel_scale * 100.0;
                }
                bool ret3 = MFApi.IO.getIOInfo(handle, ref io_info);
                if (ret3 == true)
                {
                    //for (int i = 0; i < 16; i++)
                    //{
                    //    setTable(this.tableIO, i + 1, 0, io_info.sys_di[i] ? "1" : "0");
                    //    setTable(this.tableIO, i + 1, 1, io_info.sys_do[i] ? "1" : "0");
                    //    setTable(this.tableIO, i + 1, 2, io_info.user_di[i] ? "1" : "0");
                    //    setTable(this.tableIO, i + 1, 3, io_info.user_do[i] ? "1" : "0");
                    //    setTable(this.tableIO, i + 1, 4, String.Format("{0:D}", io_info.ai[i]));
                    //    setTable(this.tableIO, i + 1, 5, String.Format("{0:D}", io_info.ao[i]));
                    //}
                }
                //bool ret4 = MFApi.SYS.readMultiSysRegister(handle, 0, ref temp);
                //if (ret4 == true)
                //{
                //    //for (int i = 0; i < 10; i++)
                //    //{
                //    //    for (int j = 0; j < 10; j++)
                //    //    {
                //    //        setTable(this.tableSR, j, i, String.Format("{0:D}", temp.data[i * 10 + j]));
                //    //    }
                //    //}
                //}
                //bool ret5 = MFApi.SYS.readMultiUserRegister(handle, 0, ref temp);
                //if (ret5 == true)
                //{
                //    //for (int i = 0; i < 10; i++)
                //    //{
                //    //    for (int j = 0; j < 10; j++)
                //    //    {
                //    //        setTable(this.tableR, j, i, String.Format("{0:D}", temp.data[i * 10 + j]));
                //    //    }
                //    //}
                //}
                if (ret3 == false && ret2 == false)
                {
                    m_msg = "未连接";
                    //this.connect.Text = "未连接";
                    MFApi.Connection.disConnect(handle);
                    handle = IntPtr.Zero;

                    SerialCount = 0;
                }
            }
        }
        private void _Scan()
        {
            while (m_Running)
            {
                if (m_debug)
                {
                    System.Threading.Thread.Sleep(1);
                    continue;
                }
                try
                {
                    if (!watch.IsRunning)
                        watch.Start();
                    if (watch.ElapsedMilliseconds > 1000)
                    {
                        watch.Reset();
                        SerialCount = iCount;
                        iCount = 0;
                    }
                    else
                        iCount++;

                    if (RetryIndex > RetryCount)
                    {
                        if (!m_error_comm)
                        {
                            m_error_comm = true;
                            //CommError(Name);
                        }
                    }
                    else
                    {
                        m_error_comm = false;
                    }

                    if (m_error_comm)
                    {
                        iCount = 0;//通訊中斷
                        System.Threading.Thread.Sleep(1);
                        continue;
                    }
                    System.Threading.Thread.Sleep(50);
                    Tick();
                }
                catch (Exception ex)
                {
                    m_error_comm = true;
                }
            }
        }



    }
}
