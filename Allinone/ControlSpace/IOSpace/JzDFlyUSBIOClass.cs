using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JetEazy;
using System.Runtime.InteropServices;

namespace Allinone.ControlSpace.IOSpace
{
    public class JzDFlyUSBIOClass : GeoIOClass
    {
        #region MC100
        [DllImport("mc100.dll", EntryPoint = "mc100_scan_device")]
        public static extern Int32 mc100_scan_device(Int32 id);
        [DllImport("mc100.dll", EntryPoint = "mc100_open")]
        public static extern Int32 mc100_open(Int32 id);
        [DllImport("mc100.dll", EntryPoint = "mc100_close")]
        public static extern Int32 mc100_close(Int32 id);
        [DllImport("mc100.dll", EntryPoint = "mc100_set_pin")]
        public static extern Int32 mc100_set_pin(Int32 id, Int32 pin);
        [DllImport("mc100.dll", EntryPoint = "mc100_clear_pin")]
        public static extern Int32 mc100_clear_pin(Int32 id, Int32 pin);
        [DllImport("mc100.dll", EntryPoint = "mc100_check_pin")]
        public static extern Int32 mc100_check_pin(Int32 id, Int32 pin);
        [DllImport("mc100.dll", EntryPoint = "mc100_set_push_pull")]
        public static extern Int32 mc100_set_push_pull(Int32 id, Int32 port, Int32 value);
        [DllImport("mc100.dll", EntryPoint = "mc100_set_pull_up")]
        public static extern Int32 mc100_set_pull_up(Int32 id, Int32 port, Int32 value);
        [DllImport("mc100.dll", EntryPoint = "mc100_read_port")]
        public static extern Int32 mc100_read_port(Int32 id, Int32 port);
        /// <returns></returns>
        [DllImport("mc100.dll", EntryPoint = "mc100_write_port")]
        public static extern Int32 mc100_write_port(Int32 id, Int32 port, Int32 value);

        const Int32 MC100_PORTA = 0;
        const Int32 MC100_PORTB = 1;
        const Int32 MC100_PORTC = 2;

        #endregion

        bool[] MC100IO = new bool[(int)MC100IONameEnum.COUNT];

        public bool Red
        {
            get
            {
                return MC100IO[(int)MC100IONameEnum.PC00];
            }
        }
        public bool Green
        {
            get
            {
                return MC100IO[(int)MC100IONameEnum.PC01];
            }
        }
        public bool Blue
        {
            get
            {
                return MC100IO[(int)MC100IONameEnum.PC02];
            }
        }
        public bool White
        {
            get
            {
                return MC100IO[(int)MC100IONameEnum.PC03];
            }
        }
        public bool Door
        {
            get
            {
                return MC100IO[(int)MC100IONameEnum.PC04];
            }
        }
        public override void LoadData()
        {
            //InitialIO();
        }

        /// <summary>
        /// 初始化IO卡
        /// </summary>
        public bool InitialIO()
        {
            if (mc100_open(0) < 0)
            {
                return false;
            }

            byte value = 0;
            mc100_write_port(0, MC100_PORTA, value);
            mc100_write_port(0, MC100_PORTB, value);
            mc100_write_port(0, MC100_PORTC, value);

            mc100_set_push_pull(0, MC100_PORTA, value);
            mc100_set_push_pull(0, MC100_PORTB, value);
            value = 31;
            mc100_set_push_pull(0, MC100_PORTC, value);

            value = 0;
            mc100_set_pull_up(0, MC100_PORTA, value);
            value = 254;
            mc100_set_pull_up(0, MC100_PORTB, value);
            value = 254;
            mc100_set_pull_up(0, MC100_PORTC, value);

            //mc100_close(0);

            return true;
        }
        /// <summary>
        /// 设定输出
        /// </summary>
        /// <param name="isPass">Pass Or Fail</param>
        public void SetOutput(int outputno,bool ison)
        {
            Int32 readvalue = mc100_read_port(0, MC100_PORTC);

            readvalue |= (ison ? 1 : 0) << outputno;
            
            mc100_write_port(0, MC100_PORTC, readvalue);
        }
        
        /// <summary>
        /// 更新IO状态
        /// </summary>
        public void GetAllIO()
        {
            byte[] arg = new byte[4];
            arg[0] = Convert.ToByte(mc100_read_port(0, MC100_PORTA));
            arg[1] = Convert.ToByte(mc100_read_port(0, MC100_PORTB));
            arg[2] = Convert.ToByte(mc100_read_port(0, MC100_PORTC));

            MC100IO[(int)MC100IONameEnum.PA00] = ((arg[0] & (1 << 0)) >> 0) == 1;
            MC100IO[(int)MC100IONameEnum.PA01] = ((arg[0] & (1 << 1)) >> 1) == 1;
            MC100IO[(int)MC100IONameEnum.PA02] = ((arg[0] & (1 << 2)) >> 2) == 1;
            MC100IO[(int)MC100IONameEnum.PA03] = ((arg[0] & (1 << 3)) >> 3) == 1;
            MC100IO[(int)MC100IONameEnum.PA04] = ((arg[0] & (1 << 4)) >> 4) == 1;
            MC100IO[(int)MC100IONameEnum.PA05] = ((arg[0] & (1 << 5)) >> 5) == 1;
            MC100IO[(int)MC100IONameEnum.PA06] = ((arg[0] & (1 << 6)) >> 6) == 1;
            MC100IO[(int)MC100IONameEnum.PA07] = ((arg[0] & (1 << 7)) >> 7) == 1;

            MC100IO[(int)MC100IONameEnum.PB00] = ((arg[1] & (1 << 0)) >> 0) == 1;

            MC100IO[(int)MC100IONameEnum.PC00] = ((arg[2] & (1 << 0)) >> 0) == 1;
            MC100IO[(int)MC100IONameEnum.PC01] = ((arg[2] & (1 << 1)) >> 1) == 1;
            MC100IO[(int)MC100IONameEnum.PC02] = ((arg[2] & (1 << 2)) >> 2) == 1;
            MC100IO[(int)MC100IONameEnum.PC03] = ((arg[2] & (1 << 3)) >> 3) == 1;
            MC100IO[(int)MC100IONameEnum.PC04] = ((arg[2] & (1 << 4)) >> 4) == 1;
        }
        public override void SaveData()
        {
            
        }

    }
}
