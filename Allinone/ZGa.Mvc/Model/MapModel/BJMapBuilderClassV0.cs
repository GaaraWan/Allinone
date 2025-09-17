using Org.BouncyCastle.Asn1.Pkcs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allinone.ZGa.Mvc.Model.MapModel
{
    public class BJMapBuilderClassV0 : IxMapBuilder
    {
        List<BJDataItem> _bjDataList = new List<BJDataItem>();
        public int AnylazeCount => _bjDataList.Count;
        public bool CreateMap(string filename)
        {
            if (!File.Exists(filename))
                return false;
            _bjDataList.Clear();
            using (System.IO.StreamReader streamReader = new System.IO.StreamReader(filename))
            {
                string _line = string.Empty;// streamReader.ReadLine();
                int _index = 0;
                while (!streamReader.EndOfStream)
                {
                    _line = streamReader.ReadLine();
                    _index = 0;
                    if (!string.IsNullOrEmpty(_line))
                    {
                        BJDataItem bJDataItem = new BJDataItem();
                        bJDataItem.Index = _index;
                        bJDataItem.RunAnalyze(_line);
                        _bjDataList.Add(bJDataItem);
                        _index++;
                    }
                }
            }
            return true;
        }
        public string GetText(int posIndex, int index)
        {
            if (posIndex < 0 || posIndex >= _bjDataList.Count)
                return string.Empty;
            BJDataItem bJDataItem = _bjDataList[posIndex];
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
            for (int i = 0; i < _bjDataList[cellIndex].Datas.Length - 1; i++)
            {
                cells[i] = _bjDataList[cellIndex].Datas[i + 1];
            }
            return cells;
        }
    }
    public class BJDataItem
    {
        public string[] Datas = null;

        public BJDataItem() { }
        public int Index = 0;
        public string DataStr = string.Empty;
        
        public void RunAnalyze(string eDataStr)
        {
            if (string.IsNullOrEmpty(eDataStr))
                return;
            DataStr = eDataStr.Trim();
            Datas = DataStr.Replace("[", ":").Replace("]", ":").Replace(";", ":").Split(':');
        }
        public string GetDataText(int eIndex)
        {
            if (Datas == null)
                return string.Empty;
            int iLastIndex = eIndex + 1;
            if (iLastIndex < 1 || iLastIndex >= Datas.Length)
                return string.Empty;
            return Datas[iLastIndex];
        }
    }
}
