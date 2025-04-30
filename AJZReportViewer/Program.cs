using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AJZReportViewer
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "-SDM2":
                        Global.VER = VERSION.SDM2;
                        break;
                    case "-SDM5":
                        Global.VER = VERSION.SDM5;
                        break;
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }
}
