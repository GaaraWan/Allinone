using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allinone.ZGa.Mvc.Model.MapModel
{
    public class DGMapBuilderClassV0 : IxMapBuilder
    {
        List<DGDataItem> _bjDataList = new List<DGDataItem>();

        public int AnylazeCount => _bjDataList.Count;

        /// <summary>
        /// 东莞是字符串 unit,unit,unit....
        /// </summary>
        /// <param name="filename">字符串</param>
        /// <returns></returns>
        public bool CreateMap(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return false;
            _bjDataList.Clear();
            string[] strings = filename.Split(',');
            int _index = 0;
            foreach (string s in strings)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    DGDataItem bJDataItem = new DGDataItem();
                    bJDataItem.Index = _index;
                    bJDataItem.RunAnalyze(s);
                    _bjDataList.Add(bJDataItem);
                    _index++;
                }
            }
            return true;
        }
        public string GetText(int posIndex, int index)
        {
            if (posIndex < 0 || posIndex >= _bjDataList.Count)
                return string.Empty;
            DGDataItem bJDataItem = _bjDataList[posIndex];
            return bJDataItem.GetDataText(index);
        }
        public string[] GetCells()
        {
            if (_bjDataList.Count == 0)
                return new string[1] { "NONE" };
            string[] cells = new string[_bjDataList.Count];
            for (int i = 0; i < _bjDataList.Count; i++)
            {
                cells[i] = _bjDataList[i].DataStr;
            }
            return cells;
        }
        public string[] GetCellContent(int cellIndex = 0)
        {
            if (cellIndex < 0 || cellIndex >= _bjDataList.Count)
                return new string[1] { "NONE" };
            string[] cells = new string[_bjDataList[cellIndex].Datas.Length];
            for (int i = 0; i < _bjDataList[cellIndex].Datas.Length; i++)
            {
                cells[i] = _bjDataList[cellIndex].Datas[i];
            }
            return cells;
        }
    }
    public class DGDataItem
    {
        public string[] Datas = null;

        public DGDataItem() { }
        public int Index = 0;
        public string DataStr = string.Empty;

        public void RunAnalyze(string eDataStr)
        {
            if (string.IsNullOrEmpty(eDataStr))
                return;
            DataStr = eDataStr.Trim();
            Datas = DataStr.Split(';');
        }
        public string GetDataText(int eIndex)
        {
            if (Datas == null)
                return string.Empty;
            int iLastIndex = eIndex;
            if (iLastIndex < 0 || iLastIndex >= Datas.Length)
                return string.Empty;
            return Datas[iLastIndex];
        }
    }
}
