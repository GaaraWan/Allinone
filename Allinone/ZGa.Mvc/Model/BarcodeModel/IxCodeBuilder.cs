using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allinone.ZGa.Mvc.Model.BarcodeModel
{
    public interface IxCodeBuilder : IDisposable
    {
        string NameStr { get; }
        bool Init();
        string Run(Bitmap bmpInput);
    }
}
