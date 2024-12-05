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

        public enum CipMainX6AddressEnum : int
        {
            COUNT = 1,

            ADR_MAPPING = 0,
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

            m_IsDebug = ReadINIValue("Parameters", "IsDebug", "0", INIFILE) == "1";
        }

        public override void SaveData()
        {
            
        }

        public string MappingStr
        {
            get
            {
                FATEKAddressClass address = CIPADDRESSARRAY[(int)CipMainX6AddressEnum.ADR_MAPPING];
                return CIP.ReadVari(address.Address0);
            }
            //set
            //{
            //    FATEKAddressClass address = CIPADDRESSARRAY[(int)CipMainX6AddressEnum.ADR_MAPPING];
            //    CIP.WriteVari(address.Address0, value);
            //}
        }
    }
}
