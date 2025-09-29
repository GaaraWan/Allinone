using JetEazy.ControlSpace;
using JetEazy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetEazy.ControlSpace.PLCSpace;
using Newtonsoft.Json.Linq;

namespace Allinone.ControlSpace.IOSpace
{
    public class JzCipMainX6IO1Class : GeoIOClass
    {
        bool m_IsDebug = false;
        string QcDebugStr = $"2,2,4,A;B;C,1,1";

        public enum CipMainX6AddressEnum : int
        {
            COUNT = 9,

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

            ADR_QCUSEMAP=7,
            ADR_QCMAPNEED=8,
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

            CIPADDRESSARRAY[(int)CipMainX6AddressEnum.ADR_QCUSEMAP] =
            new FATEKAddressClass(ReadINIValue("Operation Address", "ADR_QCUSEMAP", "0:Gvl_QcPC.bQcUseMap", INIFILE));
            CIPADDRESSARRAY[(int)CipMainX6AddressEnum.ADR_QCMAPNEED] =
            new FATEKAddressClass(ReadINIValue("Operation Address", "ADR_QCMAPNEED", "0:Gvl_QcPC.iMapNeedQc", INIFILE));

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
        string getQcDebugStrIndex(int eIndex,bool eInit=true)
        {
            if (eInit)
                loadQcDebugStr();
            string[] strings = QcDebugStr.Split(',');
            if (eIndex < strings.Length)
            {
                return strings[eIndex];
            }
            return null;
        }
        void setQcDebugStrIndex(int eIndex, string str, bool eInit = true)
        {
            if (eInit)
                loadQcDebugStr();
            string[] strings = QcDebugStr.Split(',');
            if (eIndex < 0 || eIndex >= strings.Length)
                return;
            strings[eIndex] = str;
            string temp =string.Empty;
            for (int i = 0; i < strings.Length; i++)
            {
                temp += strings[i] + ",";
            }
            QcDebugStr = temp;
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
        /// <summary>
        /// boatID号
        /// </summary>
        public string QcBotaID
        {
            get
            {
                if (m_IsDebug)
                {
                    string str = getQcDebugStrIndex(12, false);
                    return str;
                }
                FATEKAddressClass address = getCipAdress("QcBotaID");
                return CIP.ReadVari(address.Address0);
            }
            set
            {
                if (m_IsDebug)
                {
                    setQcDebugStrIndex(12, value);
                    return;
                }
                FATEKAddressClass address = getCipAdress("QcBotaID");
                CIP.WriteVari(address.Address0, value);
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
        /// <summary>
        /// 是否启用MAPPING测试
        /// </summary>
        public bool QcUseMap
        {
            get
            {
                if (m_IsDebug)
                {
                    string str = getQcDebugStrIndex(6);
                    return str == "1";
                }
                FATEKAddressClass address = CIPADDRESSARRAY[(int)CipMainX6AddressEnum.ADR_QCUSEMAP];
                return CIP.ReadVari(address.Address0).ToLower() == "true";
            }
        }
        /// <summary>
        /// true-Qc写入完成 false-Laser读取完成
        /// </summary>
        public bool QcWriteDone
        {
            get
            {
                if (m_IsDebug)
                {
                    string str = getQcDebugStrIndex(8);
                    return str == "1";
                }
                FATEKAddressClass address = getCipAdress("bQcWriteDone");
                return CIP.ReadVari(address.Address0).ToLower() == "true";
            }
            set
            {
                if (m_IsDebug)
                {
                    return;
                }
                FATEKAddressClass address = getCipAdress("bQcWriteDone");
                CIP.WriteVari(address.Address0, (value ? "true" : "false"));
            }
        }
        /// <summary>
        /// 是否启用FILE MAPPING测试
        /// </summary>
        public bool QcUseFileMap
        {
            get
            {
                if (m_IsDebug)
                {
                    string str = getQcDebugStrIndex(11);
                    return str == "1";
                }
                FATEKAddressClass address = new FATEKAddressClass($"0:Gvl_LaserPC.bUseMap");
                return CIP.ReadVari(address.Address0).ToLower() == "true";
            }
        }
        /// <summary>
        /// 更新测试结果Map到plc
        /// </summary>
        /// <param name="mapResults">根据自定义的错误数组</param>
        public void iQcMapResult(int[] mapResults)
        {
            if (mapResults == null)
                return;
            if (mapResults.Length > 0)
            {
                for (int i = 0; i < mapResults.Length; i++)
                {
                    FATEKAddressClass address = getCipAdress($"iSingleQcResult[{i}]");
                    CIP.WriteVari(address.Address0, mapResults[i].ToString());
                }
            }
            QcWriteDone = true;
        }
        /// <summary>
        /// Map需要QC检测 1-检测 2-不检测 5-空
        /// </summary>
        /// <returns>1-检测 2-不检测 5-空</returns>
        public int GetQcMap()
        {
            int iret = 1;
            string _currPos = QcCurrentPos;//格式 0-0 0-1 ...1-0 1-1...
            string[] strings = _currPos.Split('-');
            if (strings.Length > 1)
            {
                int ir = 0;
                int ic = 0;
                int.TryParse(strings[0], out ir);
                int.TryParse(strings[1], out ic);

                int iMapIndex = ir * QcColCount + ic;

                string currMap = "1";
                if (m_IsDebug)
                {
                    currMap = getQcDebugStrIndex(7);
                }
                else
                {
                    FATEKAddressClass address = getCipAdress($"iMapNeedQc[{iMapIndex}]");
                    currMap = CIP.ReadVari(address.Address0);
                }
                int.TryParse(currMap, out iret);
            }
            return iret;
        }

        #region BJ

        public void SetRowCol(string row,string col)
        {
            if (m_IsDebug)
            {
                return;
            }
            FATEKAddressClass address = getCipAdress("iQcXPos");
            CIP.WriteVari(address.Address0, col);
            FATEKAddressClass address1 = getCipAdress("iQcYPos");
            CIP.WriteVari(address1.Address0, row);

        }

        #endregion

        #region 东莞-Rayxin

        /// <summary>
        /// 读取plc的map  格式 1 1 1 空格隔开
        /// </summary>
        public string DGMap1
        {
            get
            {
                if (m_IsDebug)
                {
                    string str = getQcDebugStrIndex(9);
                    return str;
                }
                FATEKAddressClass address = new FATEKAddressClass($"0:Gvl_Status.Data[5].Map1");
                return CIP.ReadVari(address.Address0);
            }
        }
        /// <summary>
        /// 读取plc的map  格式 1 1 1 空格隔开
        /// </summary>
        public string DGMap2
        {
            get
            {
                if (m_IsDebug)
                {
                    string str = getQcDebugStrIndex(10);
                    return str;
                }
                FATEKAddressClass address = new FATEKAddressClass($"0:Gvl_Status.Data[5].Map2");
                return CIP.ReadVari(address.Address0);
            }
        }
        /// <summary>
        /// 单颗Unit内部的打印信息用";"分隔，Unit与Unit直接用","分隔。
        /// </summary>
        public string DGMarkedContent1
        {
            get
            {
                if (m_IsDebug)
                {
                    string str = $"N33320;PF0810001 100;HI,,ABCDEFG";// getQcDebugStrIndex(11);
                    return str;
                }
                FATEKAddressClass address = new FATEKAddressClass($"0:Gvl_Status.Data[5].MarkedContent1");
                return CIP.ReadVari(address.Address0);
            }
        }
        /// <summary>
        /// 单颗Unit内部的打印信息用";"分隔，Unit与Unit直接用","分隔。
        /// </summary>
        public string DGMarkedContent2
        {
            get
            {
                if (m_IsDebug)
                {
                    string str = $"ABCDEFG";//getQcDebugStrIndex(12);
                    return str;
                }
                FATEKAddressClass address = new FATEKAddressClass($"0:Gvl_Status.Data[5].MarkedContent2");
                return CIP.ReadVari(address.Address0);
            }
        }

        #endregion

        FATEKAddressClass getCipAdress(string eAdrStr)
        {
            FATEKAddressClass address = new FATEKAddressClass($"0:Gvl_QcPC.{eAdrStr}");
            return address;
        }


    }
}
