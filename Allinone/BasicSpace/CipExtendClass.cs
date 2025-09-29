using Allinone.ControlSpace.IOSpace;
using Allinone.ControlSpace.MachineSpace;
using iTextSharp.text.pdf.collection;
using JetEazy.ControlSpace;
using JetEazy.ControlSpace.PLCSpace;
using MoveGraphLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Allinone.ControlSpace.IOSpace.JzCipMainX6IO1Class;

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
        public void Close()
        {
            if (m_compoletClass == null)
                return;
            if (m_cipplcio == null)
                return;
            m_compoletClass.Close();
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
        public int QcCount
        {
            get
            {
                if (m_compoletClass == null)
                    return 1;
                if (m_cipplcio == null)
                    return 1;
                return m_cipplcio.QcCount;
            }
        }
        public int QcRowCount
        {
            get
            {
                if (m_compoletClass == null)
                    return 1;
                if (m_cipplcio == null)
                    return 1;
                return m_cipplcio.QcRowCount;
            }
        }
        public int QcColCount
        {
            get
            {
                if (m_compoletClass == null)
                    return 1;
                if (m_cipplcio == null)
                    return 1;
                return m_cipplcio.QcColCount;
            }
        }
        public string QcMapStr
        {
            get
            {
                if (m_compoletClass == null)
                    return string.Empty;
                if (m_cipplcio == null)
                    return string.Empty;
                return m_cipplcio.QcMapStr;
            }
        }
        public string QcCurrentPos
        {
            get
            {
                if (m_compoletClass == null)
                    return string.Empty;
                if (m_cipplcio == null)
                    return string.Empty;
                return m_cipplcio.QcCurrentPos;
            }
        }
        public string QcBoatID
        {
            get
            {
                if (m_compoletClass == null)
                    return string.Empty;
                if (m_cipplcio == null)
                    return string.Empty;
                return m_cipplcio.QcBotaID;
            }
            set
            {
                if (m_compoletClass == null)
                    return;
                if (m_cipplcio == null)
                    return;
                m_cipplcio.QcBotaID = value;
            }
        }
        /// <summary>
        /// 是否启用MAPPING测试
        /// </summary>
        public bool QcUseMap
        {
            get
            {
                if (m_compoletClass == null)
                    return false;
                if (m_cipplcio == null)
                    return false;
                return m_cipplcio.QcUseMap;
            }
        }
        /// <summary>
        /// Map需要QC检测 1-检测 2-不检测 5-空
        /// </summary>
        /// <returns>1-检测 2-不检测 5-空</returns>
        public int QcMapNeed
        {
            get
            {
                if (m_compoletClass == null)
                    return 1;
                if (m_cipplcio == null)
                    return 1;
                return m_cipplcio.GetQcMap();
            }
        }
        /// <summary>
        /// 是否启用FILE MAPPING测试
        /// </summary>
        public bool QcUseFileMap
        {
            get
            {
                if (m_compoletClass == null)
                    return false;
                if (m_cipplcio == null)
                    return false;
                return m_cipplcio.QcUseFileMap;
            }
        }
        /// <summary>
        /// 更新测试结果Map到plc
        /// </summary>
        /// <param name="mapResults">根据自定义的错误数组</param>
        public void QcMapResult(int[] mapResults)
        {
            if (mapResults == null)
                return;
            if (m_compoletClass == null)
                return;
            if (m_cipplcio == null)
                return;
            m_cipplcio.iQcMapResult(mapResults);
        }

        #region BJ

        public void SetRowCol(string row, string col)
        {
            if (m_compoletClass == null)
                return;
            if (m_cipplcio == null)
                return;
            m_cipplcio.SetRowCol(row, col);
        }

        #endregion

        #region 东莞-Rayxin

        public string DGMap1
        {
            get
            {
                if (m_compoletClass == null)
                    return string.Empty;
                if (m_cipplcio == null)
                    return string.Empty;
                return m_cipplcio.DGMap1;
            }
        }
        public string DGMap2
        {
            get
            {
                if (m_compoletClass == null)
                    return string.Empty;
                if (m_cipplcio == null)
                    return string.Empty;
                return m_cipplcio.DGMap2;
            }
        }
        public string DGMarkedContent1
        {
            get
            {
                if (m_compoletClass == null)
                    return string.Empty;
                if (m_cipplcio == null)
                    return string.Empty;
                return m_cipplcio.DGMarkedContent1;
            }
        }
        public string DGMarkedContent2
        {
            get
            {
                if (m_compoletClass == null)
                    return string.Empty;
                if (m_cipplcio == null)
                    return string.Empty;
                return m_cipplcio.DGMarkedContent2;
            }
        }

        #endregion

    }
}
