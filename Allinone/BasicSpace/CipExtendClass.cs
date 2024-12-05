using Allinone.ControlSpace.IOSpace;
using Allinone.ControlSpace.MachineSpace;
using iTextSharp.text.pdf.collection;
using JetEazy.ControlSpace.PLCSpace;
using MoveGraphLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allinone.BasicSpace
{
    public class CipExtendClass
    {
        CipCompoletClass m_compoletClass;
        JzCipMainX6IO1Class m_cipplcio;

        //private static CipExtendClass _instance = null;
        //public static CipExtendClass Intance
        //{
        //    get
        //    {
        //        if (_instance == null)
        //            _instance = new CipExtendClass();
        //        return _instance;
        //    }
        //}
        string m_WorkPath = string.Empty;
        public CipExtendClass(string ePath)
        {
            if (!string.IsNullOrEmpty(ePath))
                m_WorkPath = ePath;
        }

        public bool Init()
        {
            m_compoletClass = new CipCompoletClass();
            bool bOK = m_compoletClass.Open(m_WorkPath + "\\" + Machine_EA.MAIN_X6.ToString() + "\\PLCCONTROL1.INI", false);

            m_cipplcio = new JzCipMainX6IO1Class();
            m_cipplcio.Initial(m_WorkPath + "\\" + Machine_EA.MAIN_X6.ToString(), JetEazy.OptionEnum.MAIN_X6, m_compoletClass);

            return bOK;
        }
        public string GetMappingStr
        {
            get
            {
                if (m_compoletClass == null)
                    return string.Empty;
                if (m_cipplcio == null)
                    return string.Empty;

                return m_cipplcio.MappingStr;
            }
        }

    }
}
