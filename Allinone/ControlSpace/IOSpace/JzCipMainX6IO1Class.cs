using JetEazy.ControlSpace;
using JetEazy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetEazy.ControlSpace.PLCSpace;

namespace Allinone.ControlSpace.IOSpace
{
    public class JzCipMainX6IO1Class : GeoIOClass
    {
        bool m_IsDebug = false;
        string QcDebugStr = $"2,2,4,A;B;C,1,1";

        public enum CipMainX6AddressEnum : int
        {
            COUNT = 7,

            ADR_MAPPING = 0,
            /// <summary>
            /// 抽检的次数
            /// </summary>
            ADR_QCCOUNT = 1,
            /// <summary>
            /// QC行数
            /// </summary>
            ADR_QCROWCOUNT = 2,
            /// <summary>
            /// QC列数
            /// </summary>
            ADR_QCCOLCOUNT = 3,
            /// <summary>
            /// QCMAP数据
            /// </summary>
            ADR_QCMAPSTR = 4,
            ///// <summary>
            ///// QC当前抽检的位置 行和列
            ///// </summary>
            //ADR_QCCURRENTPOS = 5,

            ADR_QCROWINDEX=5,
            ADR_QCCOLINDEX=6,
        }

        public JzCipMainX6IO1Class()
        {

        }
        public void Initial(string path, OptionEnum option, CipCompoletClass plc)
        {
            CIPADDRESSARRAY = new FATEKAddressClass[(int)CipMainX6AddressEnum.COUNT];

            CIP = plc;

            INIFILE = path + "\\IO1.INI";

            LoadData();

        }
        public override void LoadData()
        {
            CIPADDRESSARRAY[(int)CipMainX6AddressEnum.ADR_MAPPING] =
                new FATEKAddressClass(ReadINIValue("Operation Address", "ADR_MAPPING", "", INIFILE));

            CIPADDRESSARRAY[(int)CipMainX6AddressEnum.ADR_QCCOUNT] =
             new FATEKAddressClass(ReadINIValue("Operation Address", "ADR_QCCOUNT", "", INIFILE));

            CIPADDRESSARRAY[(int)CipMainX6AddressEnum.ADR_QCROWCOUNT] =
             new FATEKAddressClass(ReadINIValue("Operation Address", "ADR_QCROWCOUNT", "", INIFILE));

            CIPADDRESSARRAY[(int)CipMainX6AddressEnum.ADR_QCCOLCOUNT] =
             new FATEKAddressClass(ReadINIValue("Operation Address", "ADR_QCCOLCOUNT", "", INIFILE));

            CIPADDRESSARRAY[(int)CipMainX6AddressEnum.ADR_QCMAPSTR] =
             new FATEKAddressClass(ReadINIValue("Operation Address", "ADR_QCMAPSTR", "", INIFILE));

            //CIPADDRESSARRAY[(int)CipMainX6AddressEnum.ADR_QCCURRENTPOS] =
            // new FATEKAddressClass(ReadINIValue("Operation Address", "ADR_QCCURRENTPOS", "", INIFILE));

            CIPADDRESSARRAY[(int)CipMainX6AddressEnum.ADR_QCROWINDEX] =
             new FATEKAddressClass(ReadINIValue("Operation Address", "ADR_QCROWINDEX", "", INIFILE));

            CIPADDRESSARRAY[(int)CipMainX6AddressEnum.ADR_QCCOLINDEX] =
             new FATEKAddressClass(ReadINIValue("Operation Address", "ADR_QCCOLINDEX", "", INIFILE));

            m_IsDebug = ReadINIValue("Parameters", "IsDebug", "0", INIFILE) == "1";

            loadQcDebugStr();
        }

        public override void SaveData()
        {
            
        }

        void loadQcDebugStr()
        {
            QcDebugStr = ReadINIValue("Parameters", "QcDebugStr", QcDebugStr, INIFILE);
        }
        string getQcDebugStrIndex(int eIndex)
        {
            loadQcDebugStr();
            string[] strings = QcDebugStr.Split(',');
            if (eIndex < strings.Length)
            {
                return strings[eIndex];
            }
            return null;
        }

        public string MappingStr
        {
            get
            {
                FATEKAddressClass address = CIPADDRESSARRAY[(int)CipMainX6AddressEnum.ADR_MAPPING];
                //return CIP.ReadVari("Gvl_Status.Data[3].Map[0]");

                return CIP.ReadVari(address.Address0);
            }
            //set
            //{
            //    FATEKAddressClass address = CIPADDRESSARRAY[(int)CipMainX6AddressEnum.ADR_MAPPING];
            //    CIP.WriteVari(address.Address0, value);
            //}
        }
        /// <summary>
        /// 拍照次数
        /// </summary>
        public int QcCount
        {
            get
            {
                if (m_IsDebug)
                {
                    string str = getQcDebugStrIndex(0);
                    if (!string.IsNullOrEmpty(str))
                        return int.Parse(str);
                    return 1;
                }

                FATEKAddressClass address = CIPADDRESSARRAY[(int)CipMainX6AddressEnum.ADR_QCCOUNT];
                string ret = CIP.ReadVari(address.Address0);
                int iret = 1;
                int.TryParse(ret, out iret);
                return iret;
            }
        }
        /// <summary>
        /// Y方向总颗数
        /// </summary>
        public int QcRowCount
        {
            get
            {
                if (m_IsDebug)
                {
                    string str = getQcDebugStrIndex(1);
                    if (!string.IsNullOrEmpty(str))
                        return int.Parse(str);
                    return 1;
                }

                FATEKAddressClass address = CIPADDRESSARRAY[(int)CipMainX6AddressEnum.ADR_QCROWCOUNT];
                string ret = CIP.ReadVari(address.Address0);
                int iret = 1;
                int.TryParse(ret, out iret);
                return iret;
            }
        }
        /// <summary>
        /// X方向总颗数
        /// </summary>
        public int QcColCount
        {
            get
            {
                if (m_IsDebug)
                {
                    string str = getQcDebugStrIndex(2);
                    if (!string.IsNullOrEmpty(str))
                        return int.Parse(str);
                    return 1;
                }
                FATEKAddressClass address = CIPADDRESSARRAY[(int)CipMainX6AddressEnum.ADR_QCCOLCOUNT];
                string ret = CIP.ReadVari(address.Address0);
                int iret = 1;
                int.TryParse(ret, out iret);
                return iret;
            }
        }
        public string QcMapStr
        {
            get
            {
                if (m_IsDebug)
                {
                    string str = getQcDebugStrIndex(3);
                    return str;
                }
                FATEKAddressClass address = CIPADDRESSARRAY[(int)CipMainX6AddressEnum.ADR_QCMAPSTR];
                return CIP.ReadVari(address.Address0);
            }
        }
        public string QcCurrentPos
        {
            get
            {
                string ret = "0,0";
                if (m_IsDebug)
                {
                    string str0 = getQcDebugStrIndex(4);
                    string str1 = getQcDebugStrIndex(5);
                    if (!string.IsNullOrEmpty(str0) && !string.IsNullOrEmpty(str1))
                        ret = $"{str0}-{str1}";
                    return ret;
                }
                
                FATEKAddressClass address_row = CIPADDRESSARRAY[(int)CipMainX6AddressEnum.ADR_QCROWINDEX];
                FATEKAddressClass address_col = CIPADDRESSARRAY[(int)CipMainX6AddressEnum.ADR_QCCOLINDEX];
                string rowstr = CIP.ReadVari(address_row.Address0);
                string colstr = CIP.ReadVari(address_col.Address0);
                if (!string.IsNullOrEmpty(rowstr) && !string.IsNullOrEmpty(colstr))
                {
                    ret = $"{rowstr}-{colstr}";
                }
                return ret;
            }
        }



    }
}
