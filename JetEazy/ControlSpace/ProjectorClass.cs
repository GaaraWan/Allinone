using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JetEazy.ControlSpace
{
    public class ProjectorClass :FatekPLCClass
    {
        protected override void COMPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                // 大略是這樣子用....有時指令不會一次傳完，因此要檢查最後回傳的檢查位元
                BytesToRead = COMPort.BytesToRead;
                COMPort.Read(ReadBuffer, ReadStart, BytesToRead);
                ReadStart = ReadStart + BytesToRead;
                //
                if (true) //此時 ReadBuffer裏有東西，利用 BytesToRead, ReadStart 及 ReadBuffer來取得所需要的資料
                {
                    SendCommand();
                    ReadStart = 0;
                    RetryIndex = 0;
                    ReadBuffer = new char[300];

                    if (Live == "●")
                        Live = "○";
                    else
                        Live = "●";
                }
            }
            catch (Exception ex)
            {
                base.COMPort_DataReceived(sender, e);

                if (Live == "●")
                    Live = "○";
                else
                    Live = "●";
            }
        }

        protected override void WriteCommand()
        {
            string Str = Checksum(LastCommad.GetSite() + "," + LastCommad.GetPLCCommad());

            Str = STX + Str;
            byte[] CommandByte = new byte[Str.Length];

            int i = 0;
            while (i < Str.Length)
            {
                CommandByte[i] = (byte)Str[i];
                i++;
            }
            COMPort.Write(CommandByte, 0, CommandByte.Length);
            
            //COMPort.Write(STX + Str);

            
        }

        protected override string Checksum(string OrgString)
        {
            string retStr = "";

            string[] strs = OrgString.Split(',');

            int Sum = 0;

            foreach (string str in strs)
            {
                char chr = ConvertToChar(str);
                Sum += Convert.ToInt32(chr);
                retStr += chr;
            }

            Sum += Convert.ToInt32(ETX);

            retStr += ETX;
            retStr += (char)(Sum & 0xff);

            return retStr;
        }

        public void SwitchOutside()
        {
            Command("Switch Outside");
        }
        public void SwitchPatternV100()
        {
            Command("Switch Pattern W100");
        }
        public void SwitchPatternV50()
        {
            Command("Switch Pattern W50");
        }
        public void SwitchPatternFull()
        {
            Command("Switch Pattern Full");
        }
        public void SwitchFlipNormal()
        {
            Command("Switch Flip Normal");
        }
        public void SwitchFlipCelling()
        {
            Command("Switch Flip Celling");
        }
        public void SwitchFlipMirror()
        {
            Command("Switch Flip Mirror");
        }
        public void SwitchCellMirror()
        {
            Command("Switch Flip CellMirror");
        }
        public void SetLight(int Val)
        {
            int data = 0x80 | Val;

            string datastr = ValueToHEX(data, 2);

            Command("Light",datastr);
        }
        public void SetPosition(int Val)
        {
            int data3 = 0x80 | (Val & 0x7f);
            int data2 = 0x80 | (Val >> 7);

            string datastr3 = ValueToHEX(data3, 2);
            string datastr2 = ValueToHEX(data2, 2);

            Command("Position", datastr2 + "," + datastr3);
        }

        public void SetCW(int Val)
        {
            int data4 = 0x80 | (Val & 0x7f);
            int data3 = 0x80 | (Val >> 7);

            string datastr4 = ValueToHEX(data4, 2);
            string datastr3 = ValueToHEX(data3, 2);

            Command("CW", datastr3 + "," + datastr4);
        }
        public void SetCCW(int Val)
        {
            int data4 = 0x80 | (Val & 0x7f);
            int data3 = 0x80 | (Val >> 7);

            string datastr4 = ValueToHEX(data4, 2);
            string datastr3 = ValueToHEX(data3, 2);

            Command("CCW", datastr3 + "," + datastr4);
        }
        public override void Tick()
        {
            //base.Tick();
        }

        public void Home()
        {
            SetPosition(0);
        }

        char ConvertToChar(string str)
        {
            char ret = '0';

            int GetInt32 = Convert.ToInt32(str, 16);

            ret = (char)GetInt32;

            return ret;

        }

    }
}
