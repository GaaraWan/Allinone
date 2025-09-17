using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allinone.ZGa.Mvc.Model.MapModel
{
    public interface IxMapBuilder
    {
        bool CreateMap(string filename);
        string GetText(int posIndex, int index);
        int AnylazeCount {  get; }
        string[] GetCells();
        string[] GetCellContent(int cellIndex = 0);
    }
}
