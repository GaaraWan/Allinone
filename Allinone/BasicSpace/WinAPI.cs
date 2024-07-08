using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Collections;
using System.Diagnostics;

/*Note
 * API
 */

namespace EazyWinAPI
{
    class WinAPI
    {

        /// <summary>
        /// Win32 API Imports
        /// </summary>
        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool IsZoomed(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);
        [DllImport("user32.dll")]
        public static extern IntPtr AttachThreadInput(IntPtr idAttach, IntPtr idAttachTo, int fAttach);

        /// <summary>
        /// Win32 API Constants for ShowWindowAsync()
        /// </summary>
        public const int SW_HIDE = 0;
        public const int SW_SHOWNORMAL = 1;
        public const int SW_SHOWMINIMIZED = 2;
        public const int SW_SHOWMAXIMIZED = 3;
        public const int SW_SHOWNOACTIVATE = 4;
        public const int SW_RESTORE = 9;
        public const int SW_SHOWDEFAULT = 10;

        public const int WM_SETTEXT = 0x000C;
        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WM_KEYCHAR = 0x0102;

        #region WinodwsAPI
        ///// <summary>
        ///// 自定义的结构
        ///// </summary>
        //public struct My_lParam
        //{
        //    public int i;
        //    public string s;
        //}
        ///// <summary>
        ///// 使用COPYDATASTRUCT来传递字符串
        ///// </summary>
        //[StructLayout(LayoutKind.Sequential)]
        //public struct COPYDATASTRUCT
        //{
        //    public IntPtr dwData;
        //    public int cbData;
        //    [MarshalAs(UnmanagedType.LPStr)]
        //    public string lpData;
        //}

        //消息发送API
        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(
            IntPtr hWnd,        // 信息发往的窗口的句柄
            int Msg,            // 消息ID
            int wParam,         // 参数1
            int lParam            // 参数2
        );


        //public   const int WM_SETTEXT = 0x0c;
        ////消息发送API
        //[DllImport("User32.dll", EntryPoint = "PostMessage")]
        //public static extern int PostMessage(
        //    IntPtr hWnd,        // 信息发往的窗口的句柄
        //    int Msg,            // 消息ID
        //    int wParam,         // 参数1
        //    ref My_lParam lParam //参数2
        //);

        ////异步消息发送API
        //[DllImport("User32.dll", EntryPoint = "PostMessage")]
        //public static extern int PostMessage(
        //    IntPtr hWnd,        // 信息发往的窗口的句柄
        //    int Msg,            // 消息ID
        //    int wParam,         // 参数1
        //    ref  COPYDATASTRUCT lParam  // 参数2
        //);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);//设定焦点

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);//设定窗口前置

        //设定窗口前置或后置
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern bool SetWindowPos(
        IntPtr hWnd,
        IntPtr hWndInsertAfter,
        int X,
        int Y,
        int cx,
        int cy,
        uint uFlags
        );
        public class SetWindowPosPar
        {
            /// <summary>
            ///  将窗口置于所有非顶层窗口之上。即使窗口未被激活窗口也将保持顶级位置。查看该参数的使用方法，请看说明部分
            /// </summary>
            public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
            /// <summary>
            /// 将窗口置于所有非顶层窗口之上（即在所有顶层窗口之后）。如果窗口已经是非顶层窗口则该标志不起作用。
            /// </summary>
            public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
            /// <summary>
            /// 将窗口置于Z序的顶部
            /// </summary>
            public static readonly IntPtr HWND_TOP = new IntPtr(0);

            public static UInt32 SWP_NOSIZE = 0x0001;
            public static UInt32 SWP_NOMOVE = 0x0002;
            public static UInt32 SWP_NOZORDER = 0x0004;
            public static UInt32 SWP_NOREDRAW = 0x0008;
            public static UInt32 SWP_NOACTIVATE = 0x0010;
            public static UInt32 SWP_FRAMECHANGED = 0x0020;
            public static UInt32 SWP_SHOWWINDOW = 0x0040;
            public static UInt32 SWP_HIDEWINDOW = 0x0080;
            public static UInt32 SWP_NOCOPYBITS = 0x0100;
            public static UInt32 SWP_NOOWNERZORDER = 0x0200;
            public static UInt32 SWP_NOSENDCHANGING = 0x0400;
            public static UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;
        }

        // [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true)]
        //  public static extern IntPtr GetForegroundWindow(); //获得本窗体的句柄

        [DllImport("user32.dll", EntryPoint = "GetParent")]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "GetCursorPos")]
        public static extern bool GetCursorPos(out Point pt);

        [DllImport("user32.dll", EntryPoint = "WindowFromPoint", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr WindowFromPoint(Point pt);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, [Out, MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowRect(IntPtr hwnd, ref Rectangle rc);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClientRect(IntPtr hwnd, ref Rectangle rc);



        /// <summary>
        /// 通过句柄改变程序的长宽和坐标
        /// </summary>
        /// <param name="handle">源程序的句柄</param>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="w">宽度</param>
        /// <param name="h">长度</param>
        /// <param name="boolean">是否使用</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int MoveWindow(IntPtr hwnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int ScreenToClient(IntPtr hWnd, ref Rectangle rect);

        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, StringBuilder lParam);//基于Windows消息机制获取TextBox的信息

        /// <summary>
        /// 获取指定名称的窗口句柄
        /// </summary>
        /// <param name="lpClassName"></param>
        /// <param name="lpWindowName"></param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


        /// <summary>
        /// 获取最前台窗口句柄
        /// </summary>
        /// <returns></returns>
        [DllImport("User32.dll")]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// 通过句柄得到程序的X，Y轴
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="rc"></param>
        /// <returns></returns>
        [DllImport("user32")]
        public static extern bool GetWindowRect(IntPtr handle, ref RECT rc);


        [DllImport("User32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, string lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, ref Rectangle lParam);

        [DllImport("User32.dll")]
        public extern static IntPtr GetDC(System.IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "WindowFromPoint")]//指定坐标处窗体句柄
        public static extern int WindowFromPoint(int xPoint, int yPoint);

        #region 查找所有应用程序标题所用的API
        [DllImport("IpHlpApi.dll")]
        public static extern uint GetIfTable(byte[] pIfTable, ref uint pdwSize, bool bOrder);

        [DllImport("User32")]
        private extern static int GetWindow(int hWnd, int wCmd);

        [DllImport("User32")]
        private extern static int GetWindowLongA(int hWnd, int wIndx);

        [DllImport("user32.dll")]
        private static extern bool GetWindowText(int hWnd, StringBuilder title, int maxBufSize);

        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern int GetWindowTextLength(IntPtr hWnd);
        #endregion

        #endregion

        #region 封装API方法
        /// <summary>
        /// 找到句柄
        /// </summary>
        /// <param name="IpClassName">类名</param>
        /// <returns></returns>
        public static IntPtr GetHandle(string IpClassName)
        {
            return FindWindow(IpClassName, null);
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        /// <summary>
        /// 找类名
        /// </summary>
        /// <param name="hWnd">句柄</param>
        /// <returns></returns>
        public static string GetClassName(IntPtr hWnd)
        {
            StringBuilder lpClassName = new StringBuilder(128);
            if (GetClassName(hWnd, lpClassName, lpClassName.Capacity) == 0)
            {
                throw new Exception("not found IntPtr!");
            }
            return lpClassName.ToString();
        }

        /// <summary>
        /// 找到句柄
        /// </summary>
        /// <param name="p">坐标</param>
        /// <returns></returns>
        public static IntPtr GetHandle(Point p)
        {
            return WindowFromPoint(p);
        }

        //鼠标位置的坐标
        public static Point GetCursorPosPoint()
        {
            Point p = new Point();
            if (GetCursorPos(out p))
            {
                return p;
            }
            return default(Point);
        }

        /// <summary>
        /// 发送消息给控件，反回相应（Text）的值
        /// </summary>
        /// <param name="hwnd">句柄</param>
        /// <param name="SB">接收的字符串</param>
        /// <returns></returns>
        public static StringBuilder GetTextXiaoXi(IntPtr hwnd, StringBuilder SB)
        {
            StringBuilder ReceiveSting = new StringBuilder(1024);
            SendMessage(hwnd, 0x00D, 1024, SB);
            return SB;

        }

        /// <summary>
        /// 子窗口句柄
        /// </summary>
        /// <param name="hwndParent">父窗口句柄</param>
        /// <param name="hwndChildAfter">前一个同目录级同名窗口句柄</param>
        /// <param name="lpszClass">类名</param>
        /// <returns></returns>
        public static IntPtr GetChildHandle(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass)
        {
            return FindWindowEx(hwndParent, hwndChildAfter, lpszClass, null);
        }

        /// <summary>
        /// 全部子窗口句柄
        /// </summary>
        /// <param name="hwndParent">父窗口句柄</param>
        /// <param name="className">类名</param>
        /// <returns></returns>
        public static List<IntPtr> GetChildHandles(IntPtr hwndParent, string className)
        {
            List<IntPtr> resultList = new List<IntPtr>();
            for (IntPtr hwndClient = GetChildHandle(hwndParent, IntPtr.Zero, className); hwndClient != IntPtr.Zero; hwndClient = GetChildHandle(hwndParent, hwndClient, className))
            {
                resultList.Add(hwndClient);
            }

            return resultList;
        }

        /// <summary>
        /// 10节循环找全部子窗口句柄
        /// </summary>
        /// <param name="hwndParent">父窗口句柄</param>
        /// <param name="className">类名</param>
        /// <returns></returns>
        public static List<IntPtr> GetChildHandles10(IntPtr hwndParent, string className)
        {
            List<IntPtr> resultList = new List<IntPtr>();
            for (IntPtr hwndClient1 = GetChildHandle(hwndParent, IntPtr.Zero, className);
            hwndClient1 != IntPtr.Zero; hwndClient1 = GetChildHandle(hwndParent, hwndClient1, className))
            {
                resultList.Add(hwndClient1);
                for (IntPtr hwndClient2 = GetChildHandle(hwndClient1, IntPtr.Zero, className); hwndClient2 != IntPtr.Zero; hwndClient2 = GetChildHandle(hwndClient1, hwndClient2, className))
                {
                    resultList.Add(hwndClient2);
                    for (IntPtr hwndClient3 = GetChildHandle(hwndClient2, IntPtr.Zero, className); hwndClient3 != IntPtr.Zero; hwndClient3 = GetChildHandle(hwndClient2, hwndClient3, className))
                    {
                        resultList.Add(hwndClient3);
                        for (IntPtr hwndClient4 = GetChildHandle(hwndClient3, IntPtr.Zero, className); hwndClient4 != IntPtr.Zero; hwndClient4 = GetChildHandle(hwndClient3, hwndClient4, className))
                        {
                            resultList.Add(hwndClient4);
                            for (IntPtr hwndClient5 = GetChildHandle(hwndClient4, IntPtr.Zero, className); hwndClient5 != IntPtr.Zero; hwndClient5 = GetChildHandle(hwndClient4, hwndClient5, className))
                            {
                                resultList.Add(hwndClient5);
                                for (IntPtr hwndClient6 = GetChildHandle(hwndClient5, IntPtr.Zero, className); hwndClient6 != IntPtr.Zero; hwndClient6 = GetChildHandle(hwndClient5, hwndClient6, className))
                                {
                                    resultList.Add(hwndClient6);
                                    for (IntPtr hwndClient7 = GetChildHandle(hwndClient6, IntPtr.Zero, className); hwndClient7 != IntPtr.Zero; hwndClient7 = GetChildHandle(hwndClient6, hwndClient7, className))
                                    {
                                        resultList.Add(hwndClient7);
                                        for (IntPtr hwndClient8 = GetChildHandle(hwndClient7, IntPtr.Zero, className); hwndClient8 != IntPtr.Zero; hwndClient8 = GetChildHandle(hwndClient7, hwndClient8, className))
                                        {
                                            resultList.Add(hwndClient8);
                                            for (IntPtr hwndClient9 = GetChildHandle(hwndClient8, IntPtr.Zero, className); hwndClient9 != IntPtr.Zero; hwndClient9 = GetChildHandle(hwndClient8, hwndClient9, className))
                                            {
                                                resultList.Add(hwndClient9);
                                                for (IntPtr hwndClient10 = GetChildHandle(hwndClient9, IntPtr.Zero, className); hwndClient10 != IntPtr.Zero; hwndClient10 = GetChildHandle(hwndClient9, hwndClient10, className))
                                                {
                                                    resultList.Add(hwndClient10);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return resultList;
        }

        /// <summary>
        /// 给窗口发送内容
        /// </summary>
        /// <param name="hWnd">句柄</param>
        /// <param name="lParam">要发送的内容</param>
        public static void SetText(IntPtr hWnd, string lParam)
        {
            SendMessage(hWnd, WM_SETTEXT, IntPtr.Zero, lParam);
        }


        /// <summary>
        /// 获得窗口内容或标题
        /// </summary>
        /// <param name="hWnd">句柄</param>
        /// <returns></returns>
        public static string GetText(IntPtr hWnd)
        {
            StringBuilder result = new StringBuilder(128);
            GetWindowText(hWnd, result, result.Capacity);
            return result.ToString();
        }

        /// <summary>
        /// 窗口在屏幕位置
        /// </summary>
        /// <param name="hWnd">句柄</param>
        /// <returns></returns>
        public static Rectangle GetWindowRect(IntPtr hWnd)
        {
            Rectangle result = default(Rectangle);
            GetWindowRect(hWnd, ref result);
            return result;
        }

        /// <summary>
        /// 窗口相对屏幕位置转换成父窗口位置
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Rectangle ScreenToClient(IntPtr hWnd, Rectangle rect)
        {
            Rectangle result = rect;
            ScreenToClient(hWnd, ref result);
            return result;
        }

        /// <summary>
        /// 窗口大小
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static Rectangle GetClientRect(IntPtr hWnd)
        {
            Rectangle result = default(Rectangle);
            GetClientRect(hWnd, ref result);
            return result;
        }

        /// <summary>
        /// 获取当前系统的进程ID、名称、路径
        /// </summary>
        /// <returns></returns>
        public static ArrayList GetJinChengIDorName()
        {
            ArrayList arraylist_IDorName = new ArrayList();
            List<int> list_ID = new List<int>();
            List<string> list_Name = new List<string>();
            List<string> list_file = new List<string>();
            foreach (Process p in Process.GetProcesses())//遍历当前的系统进程
            {
                list_ID.Add(p.Id);
                list_Name.Add(p.ProcessName);
                try
                {
                    list_file.Add(p.MainModule.FileName.ToString());
                }
                catch (Exception e)
                {
                    list_file.Add(e.Message.ToString());
                }

            }
            arraylist_IDorName.Add(list_ID);
            arraylist_IDorName.Add(list_Name);
            arraylist_IDorName.Add(list_file);
            return arraylist_IDorName;
        }
        /// <summary>
        /// 获取当前启动的应用程序
        /// </summary>
        /// <returns></returns>
        public static List<string> GetRunText()
        {
            List<string> list_Run = new List<string>();
            Process[] ps = Process.GetProcesses();
            foreach (Process p in ps)
            {

                if (p.MainWindowHandle != null)
                {

                    list_Run.Add(p.MainWindowTitle);

                }

            }
            return list_Run;
        }

        //使用：
        //           listBox1.Items.Clear();
        //            List<string> Apps = WinAPI. FindAllApps((int)this.Handle);
        //           foreach (string app in Apps)
        //           {
        //                listBox1.Items.Add(app);
        //            }
        private const int GW_HWNDFIRST = 0;
        private const int GW_HWNDNEXT = 2;
        private const int GWL_STYLE = (-16);
        private const int WS_VISIBLE = 268435456;
        private const int WS_BORDER = 8388608;
        /// <summary>
        ///  查找所有应用程序标题
        /// </summary>
        /// <param name="Handle"></param>
        /// <returns>应用程序标题(List<string>)</returns>
        public static List<string> FindAllApps(int Handle)
        {
            List<string> Apps = new List<string>();

            int hwCurr;
            hwCurr = GetWindow(Handle, GW_HWNDFIRST);

            while (hwCurr > 0)
            {
                int IsTask = (WS_VISIBLE | WS_BORDER);
                int lngStyle = GetWindowLongA(hwCurr, GWL_STYLE);
                bool TaskWindow = ((lngStyle & IsTask) == IsTask);
                if (TaskWindow)
                {
                    int length = GetWindowTextLength(new IntPtr(hwCurr));
                    StringBuilder sb = new StringBuilder(2 * length + 1);
                    GetWindowText(hwCurr, sb, sb.Capacity);
                    string strTitle = sb.ToString();
                    if (!string.IsNullOrEmpty(strTitle))
                    {
                        Apps.Add(strTitle);
                    }
                }
                hwCurr = GetWindow(hwCurr, GW_HWNDNEXT);
            }

            return Apps;
        }


        #endregion

        public const uint PROCESS_ALL_ACCESS = 0x001F0FFF;
        public const uint KEYEVENTF_EXTENDEDKEY = 0x1;
        public const uint KEYEVENTF_KEYUP = 0x2;
        public readonly int MOUSEEVENTF_LEFTDOWN = 0x2;
        public readonly int MOUSEEVENTF_LEFTUP = 0x4;
        public const uint KBC_KEY_CMD = 0x64;
        public const uint KBC_KEY_DATA = 0x60;

        /// <summary>
        /// 鼠标动作枚举
        /// </summary>
        public enum MouseEventFlag : uint
        {
            move = 0x0001,
            leftdown = 0x0002,
            leftup = 0x0004,
            rightdown = 0x0008,
            rightup = 0x0010,
            middledown = 0x0020,
            middleup = 0x0040,
            xdown = 0x0080,
            xup = 0x0100,
            wheel = 0x0800,
            virtualdesk = 0x4000,
            absolute = 0x8000
        }
        /// <summary>
        /// 键盘动作枚举
        /// </summary>
        public enum VirtualKeys : byte
        {
            //VK_NUMLOCK = 0x90, //数字锁定键
            //VK_SCROLL = 0x91,  //滚动锁定
            //VK_CAPITAL = 0x14, //大小写锁定
            //VK_A = 62,         //键盘A
            VK_LBUTTON = 1,      //鼠标左键 
            VK_RBUTTON = 2,     //鼠标右键 
            VK_CANCEL = 3,    //Ctrl+Break(通常不需要处理) 
            VK_MBUTTON = 4,    //鼠标中键 
            VK_BACK = 8,      //Backspace 
            VK_TAB = 9,      //Tab 
            VK_CLEAR = 12,    //Num Lock关闭时的数字键盘5 
            VK_RETURN = 13,   //Enter(或者另一个) 
            VK_SHIFT = 16,    //Shift(或者另一个) 
            VK_CONTROL = 17,   //Ctrl(或者另一个） 
            VK_MENU = 18,    //Alt(或者另一个) 
            VK_PAUSE = 19,    //Pause 
            VK_CAPITAL = 20,   //Caps Lock 
            VK_ESCAPE = 27,   //Esc 
            VK_SPACE = 32,    //Spacebar 
            VK_PRIOR = 33,    //Page Up 
            VK_NEXT = 34,    //Page Down 
            VK_END = 35,     //End 
            VK_HOME = 36,    //Home 
            VK_LEFT = 37,     //左箭头 
            VK_UP = 38,      //上箭头 
            VK_RIGHT = 39,    //右箭头 
            VK_DOWN = 40,     //下箭头 
            VK_SELECT = 41,    //可选 
            VK_PRINT = 42,    //可选 
            VK_EXECUTE = 43,   //可选 
            VK_SNAPSHOT = 44,  //Print Screen 
            VK_INSERT = 45,   //Insert 
            VK_DELETE = 46,    //Delete 
            VK_HELP = 47,　　    //可选 
            VK_NUM0 = 48,        //0
            VK_NUM1 = 49,        //1
            VK_NUM2 = 50,        //2
            VK_NUM3 = 51,        //3
            VK_NUM4 = 52,        //4
            VK_NUM5 = 53,        //5
            VK_NUM6 = 54,        //6
            VK_NUM7 = 55,        //7
            VK_NUM8 = 56,        //8
            VK_NUM9 = 57,        //9
            VK_A = 65,           //A
            VK_B = 66,           //B
            VK_C = 67,           //C
            VK_D = 68,           //D
            VK_E = 69,           //E
            VK_F = 70,           //F
            VK_G = 71,           //G
            VK_H = 72,           //H
            VK_I = 73,           //I
            VK_J = 74,           //J
            VK_K = 75,           //K
            VK_L = 76,           //L
            VK_M = 77,           //M
            VK_N = 78,           //N
            VK_O = 79,           //O
            VK_P = 80,           //P
            VK_Q = 81,           //Q
            VK_R = 82,           //R
            VK_S = 83,           //S
            VK_T = 84,           //T
            VK_U = 85,           //U
            VK_V = 86,           //V
            VK_W = 87,           //W
            VK_X = 88,           //X
            VK_Y = 89,           //Y
            VK_Z = 90,           //Z
            VK_NUMPAD0 = 96,     //0
            VK_NUMPAD1 = 97,     //1
            VK_NUMPAD2 = 98,     //2
            VK_NUMPAD3 = 99,     //3
            VK_NUMPAD4 = 100,    //4
            VK_NUMPAD5 = 101,    //5
            VK_NUMPAD6 = 102,    //6
            VK_NUMPAD7 = 103,    //7
            VK_NUMPAD8 = 104,    //8
            VK_NUMPAD9 = 105,    //9
            VK_NULTIPLY = 106,  //数字键盘上的* 
            VK_ADD = 107,    //数字键盘上的+ 
            VK_SEPARATOR = 108, //可选 
            VK_SUBTRACT = 109,  //数字键盘上的- 
            VK_DECIMAL = 110,  //数字键盘上的. 
            VK_DIVIDE = 111,　　 //数字键盘上的/
            VK_F1 = 112,
            VK_F2 = 113,
            VK_F3 = 114,
            VK_F4 = 115,
            VK_F5 = 116,
            VK_F6 = 117,
            VK_F7 = 118,
            VK_F8 = 119,
            VK_F9 = 120,
            VK_F10 = 121,
            VK_F11 = 122,
            VK_F12 = 123,
            VK_NUMLOCK = 144,  //Num Lock 
            VK_SCROLL = 145 　   // Scroll Lock 
        }

        /// <summary>
        /// 消息枚举
        /// </summary>
        public enum WMessages : int
        {
            WM_LBUTTONDOWN = 0x201, //Left mousebutton down
            WM_LBUTTONUP = 0x202, //Left mousebutton up
            WM_LBUTTONDBLCLK = 0x203, //Left mousebutton doubleclick
            WM_RBUTTONDOWN = 0x204, //Right mousebutton down
            WM_RBUTTONUP = 0x205, //Right mousebutton up
            WM_RBUTTONDBLCLK = 0x206, //Right mousebutton doubleclick
            WM_KEYDOWN = 0x100, //Key down
            WM_KEYUP = 0x101, //Key up
            WM_KEYCHAR = 0x0102,//key Char
            WM_SETTEXT = 0x000C,//发送信息
            WM_Close = 0x0010,//关闭窗口
        }
    }

    /*
     * 调用User32API.GetCurrentWindowHandle()即可返回进程的主窗口句柄，
     * 如果获取失败则返回IntPtr.Zero。
     */
    /// <summary>
    /// 获取当前主线程的句柄
    /// </summary>
    public class User32API
    {
        private static Hashtable processWnd = null;

        public delegate bool WNDENUMPROC(IntPtr hwnd, uint lParam);

        static User32API()
        {
            if (processWnd == null)
            {
                processWnd = new Hashtable();
            }
        }

        [DllImport("user32.dll", EntryPoint = "EnumWindows", SetLastError = true)]
        public static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, uint lParam);

        [DllImport("user32.dll", EntryPoint = "GetParent", SetLastError = true)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref uint lpdwProcessId);

        [DllImport("user32.dll", EntryPoint = "IsWindow")]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        public static extern void SetLastError(uint dwErrCode);
        /// <summary>
        /// 获取指定ID进程主窗口句柄
        /// </summary>
        /// <param name="i_ipid">进程 ID</param>
        /// <returns></returns>
        public static IntPtr GetCurrentWindowHandle(int i_ipid)
        {
            uint uiPid = (uint)i_ipid;
            IntPtr ptrWnd = IntPtr.Zero;
        //    uint uiPid = (uint)Process.GetCurrentProcess().Id;  // 当前进程 ID

            object objWnd = processWnd[uiPid];

            if (objWnd != null)
            {
                ptrWnd = (IntPtr)objWnd;
                if (ptrWnd != IntPtr.Zero && IsWindow(ptrWnd))  // 从缓存中获取句柄
                {
                    return ptrWnd;
                }
                else
                {
                    ptrWnd = IntPtr.Zero;
                }
            }

            bool bResult = EnumWindows(new WNDENUMPROC(EnumWindowsProc), uiPid);
            // 枚举窗口返回 false 并且没有错误号时表明获取成功
            if (!bResult && Marshal.GetLastWin32Error() == 0)
            {
                objWnd = processWnd[uiPid];
                if (objWnd != null)
                {
                    ptrWnd = (IntPtr)objWnd;
                }
            }

            return ptrWnd;
        }
        /// <summary>
        /// 获取当前主程序ID
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetCurrentWindowHandle()
        {
            IntPtr ptrWnd = IntPtr.Zero;
            uint uiPid = (uint)Process.GetCurrentProcess().Id;  // 当前进程 ID

            object objWnd = processWnd[uiPid];

            if (objWnd != null)
            {
                ptrWnd = (IntPtr)objWnd;
                if (ptrWnd != IntPtr.Zero && IsWindow(ptrWnd))  // 从缓存中获取句柄
                {
                    return ptrWnd;
                }
                else
                {
                    ptrWnd = IntPtr.Zero;
                }
            }

            bool bResult = EnumWindows(new WNDENUMPROC(EnumWindowsProc), uiPid);
            // 枚举窗口返回 false 并且没有错误号时表明获取成功
            if (!bResult && Marshal.GetLastWin32Error() == 0)
            {
                objWnd = processWnd[uiPid];
                if (objWnd != null)
                {
                    ptrWnd = (IntPtr)objWnd;
                }
            }

            return ptrWnd;
        }

        private static bool EnumWindowsProc(IntPtr hwnd, uint lParam)
        {
            uint uiPid = 0;

            if (GetParent(hwnd) == IntPtr.Zero)
            {
                GetWindowThreadProcessId(hwnd, ref uiPid);
                if (uiPid == lParam)    // 找到进程对应的主窗口句柄
                {
                    processWnd[uiPid] = hwnd;   // 把句柄缓存起来
                    SetLastError(0);    // 设置无错误
                    return false;   // 返回 false 以终止枚举窗口
                }
            }

            return true;
        }
    }
}
