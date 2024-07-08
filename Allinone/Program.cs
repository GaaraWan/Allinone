using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Allinone
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Allinone.FormSpace.MainForm());
            //Application.Run(new FormSpace.frmCAMTester());
            //當你發現跑到這行然後又NG時，代表你某個物件裏的圖被刪除而無法顯示了，之前會有大✕，現在只有無聲的抗議，愚蠢!!!

        }
    }
}
