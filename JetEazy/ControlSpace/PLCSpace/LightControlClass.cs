
#define JC_DCV24120_8TS

#if JC_DCV24120_8TS

using JetEazy.BasicSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JetEazy.ControlSpace.PLCSpace
{
    public class LightControlClass : COMClass
    {
        //protected char STX = '\x02';
        protected char ETX = '!';

        int iCountTemp = 0;
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

        public string Live = "●";

        JzTimes PLCDuriationTime = new JzTimes();
        public int msDuriation = 0;

        protected override void COMPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                // 大略是這樣子用....有時指令不會一次傳完，因此要檢查最後回傳的檢查位元
                BytesToRead = COMPort.BytesToRead;
                COMPort.Read(ReadBuffer, ReadStart, BytesToRead);
                ReadStart = ReadStart + BytesToRead;
                //
                if (Analyze(ReadStart - 1)) //此時 ReadBuffer裏有東西，利用 BytesToRead, ReadStart 及 ReadBuffer來取得所需要的資料
                {
                    base.COMPort_DataReceived(sender, e);

                    if (Live == "●")
                        Live = "○";
                    else
                        Live = "●";
                }
            }
            catch (Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
                base.COMPort_DataReceived(sender, e);

                if (Live == "●")
                    Live = "○";
                else
                    Live = "●";
            }
        }
        protected bool Analyze(int LastIndex)
        {
            bool ret = false;

            if (ReadBuffer[LastIndex] != ETX)
            {
                return ret;
            }
            else
                ret = true;


            //取得時間差

            msDuriation = PLCDuriationTime.msDuriation;

            //紀錄開始時間

            PLCDuriationTime.Cut();

            if (!watch.IsRunning)
                watch.Start();
            if (watch.ElapsedMilliseconds > 1000)
            {
                watch.Reset();
                iCount = iCountTemp;
                iCountTemp = 0;
            }
            else
                iCountTemp++;

            if (LastCommad == null)
                return ret;
            OnRead(ReadBuffer, LastCommad.GetName(), Name);

            return ret;
        }
        //public void SetIO(bool IsOn, string ioname)
        //{
        //    int address = 0;
        //    int.TryParse(ioname.Substring(1), out address);

        //    ioname = ioname.Substring(0, 1) + address.ToString("0000");

        //    if (IsOn)
        //        Command("Set Bit On", ioname);
        //    else
        //        Command("Set Bit Off", ioname);
        //}
        //public void SetData(string data, string ioname)
        //{
        //    Command("Set Data", ioname + data);
        //}

        /// <summary>
        ///// 写数据到PLC
        ///// </summary>
        ///// <param name="Address">PLC地址</param>
        ///// <param name="data">数据</param>
        ///// <param name="word">如为2 则写入连续两个地址</param>
        //public bool SetData(string Address, int data, byte word = 1)
        //{
        //    if (word == 2)
        //    {
        //        long setH = data >> 16;
        //        long setL = data % 65536;

        //        int address = 0;
        //        if (!int.TryParse(Address.Substring(1), out address))
        //            return false;


        //        string filst = Address[0].ToString();


        //        SetData(ValueToHEX(setL, 4), filst + address.ToString("00000"));
        //        SetData(ValueToHEX(setH, 4), filst + (address + 1).ToString("00000"));

        //        return true;
        //    }
        //    else
        //    {
        //        int address = 0;
        //        if (!int.TryParse(Address.Substring(1), out address))
        //            return false;

        //        string filst = Address[0].ToString();

        //        long setL = data % 65536;
        //        SetData(ValueToHEX(setL, 4), filst + address.ToString("00000"));

        //        return true;
        //    }

        //}

        public void SetRGBWValue(int r, int g, int b, int w = 0)
        {
            if (IsSimulater)
                return;

            string str = string.Empty;

            str += "S";

            switch (ChannelCount)
            {
                case 2:
                    str += (r == 0 ? "000F" : r.ToString("000") + "T");
                    str += (b == 0 ? "000F" : b.ToString("000") + "T");
                    break;
                case 4:
                    str += (r == 0 ? "000F" : r.ToString("000") + "T");
                    str += (g == 0 ? "000F" : g.ToString("000") + "T");
                    str += (b == 0 ? "000F" : b.ToString("000") + "T");
                    str += (w == 0 ? "000F" : w.ToString("000") + "T");
                    break;
                case 8:
                    str += (r == 0 ? "000F" : r.ToString("000") + "T");
                    str += (g == 0 ? "000F" : g.ToString("000") + "T");
                    str += (b == 0 ? "000F" : b.ToString("000") + "T");
                    str += (w == 0 ? "000F" : w.ToString("000") + "T");
                    str += (r == 0 ? "000F" : r.ToString("000") + "T");
                    str += (g == 0 ? "000F" : g.ToString("000") + "T");
                    str += (b == 0 ? "000F" : b.ToString("000") + "T");
                    str += (w == 0 ? "000F" : w.ToString("000") + "T");
                    break;
            }
           
            str += "C#";

            if (COMPort == null)
                return;

            //if (COMPort.IsOpen)
            {
                COMPort.Write(str);
            }
        }

        protected override void WriteCommand()
        {
            if (IsSimulater)
                return;

            //string Str = LastCommad.GetSite() + Checksum(LastCommad.GetPLCCommad());
            //COMPort.Write(STX + Str + ETX);
        }
        protected override string Checksum(string OrgString)
        {
            int j = 0;
            char[] Chars = OrgString.ToCharArray();

            j = 99;
            foreach (char ichar in Chars)
                j = j + ichar;
            return OrgString + ("00" + j.ToString("X")).Substring(("00" + j.ToString("X")).Length - 2, 2);
        }

        public override void Tick()
        {
            base.Tick();
        }

        //當有Input Trigger時，產生OnTrigger
        public delegate void TriggerHandler(string OperationString);
        public event TriggerHandler TriggerAction;
        public void OnTrigger(String OperationString)
        {
            if (TriggerAction != null)
            {
                TriggerAction(OperationString);
            }
        }

        //當有Input Read
        public delegate void ReadHandler(char[] readbuffer, string operationstring, string myname);
        public event ReadHandler ReadAction;
        public void OnRead(char[] readbuffer, string operationstring, string myname)
        {
            if (ReadAction != null)
            {
                ReadAction(readbuffer, operationstring, myname);
            }
        }
    }
}


#endif

