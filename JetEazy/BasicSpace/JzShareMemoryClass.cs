using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace JetEazy.BasicSpace
{
    public class JzShareMemoryClass
    {
        #region win32 API
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFileMapping(int hFile, IntPtr lpAttributes, uint flProtect, uint dwMaxSizeHi, uint dwMaxSizeLow, string lpName);
        
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr OpenFileMapping(int dwDesiredAccess,[MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,string lpName);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr MapViewOfFile(IntPtr hFileMapping,uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow,uint dwNumberOfBytesToMap);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool UnmapViewOfFile(IntPtr pvBaseAddress);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32", EntryPoint="GetLastError")]
        public static extern int GetLastError ();
        #endregion

        #region 常量
        const int ERROR_ALREADY_EXISTS = 183;

        const int FILE_MAP_COPY = 0x0001;
        const int FILE_MAP_WRITE = 0x0002;
        const int FILE_MAP_READ = 0x0004;
        const int FILE_MAP_ALL_ACCESS = 0x0002 | 0x0004;

        const int PAGE_READONLY = 0x02;
        const int PAGE_READWRITE = 0x04;
        const int PAGE_WRITECOPY = 0x08;
        const int PAGE_EXECUTE = 0x10;
        const int PAGE_EXECUTE_READ = 0x20;
        const int PAGE_EXECUTE_READWRITE = 0x40;

        const int SEC_COMMIT = 0x8000000;
        const int SEC_IMAGE = 0x1000000;
        const int SEC_NOCACHE = 0x10000000;
        const int SEC_RESERVE = 0x4000000;

        const int INVALID_HANDLE_VALUE = -1;
        #endregion

        #region 变量
        IntPtr m_hSharedMemoryFile = IntPtr.Zero;
        IntPtr m_pwData = IntPtr.Zero;
        bool m_bAlreadyExist = false;
        bool m_bInit = false;
        const int m_MemSize = 62000;
        Encoding encoding = Encoding.Unicode;
        #endregion

        #region 构造函数
        public JzShareMemoryClass()
        {
        }
        ~JzShareMemoryClass()
        {
            Close();
        }
        #endregion

        #region 打开化共享内存
        public int OpenShareMemory()
        {
            return OpenShareMemory("JetEazy");
        }
        public int OpenShareMemory(string strName)
        {
            try
            {
                if (strName.Length > 0)
                {
                    //创建内存共享体(INVALID_HANDLE_VALUE)
                    m_hSharedMemoryFile = CreateFileMapping(INVALID_HANDLE_VALUE, IntPtr.Zero, (uint)PAGE_READWRITE, 0, (uint)m_MemSize, strName);
                    if (m_hSharedMemoryFile == IntPtr.Zero)
                    {
                        m_bAlreadyExist = false;
                        m_bInit = false;
                        return 2; //创建共享体失败
                    }
                    else
                    {
                        if (GetLastError() == ERROR_ALREADY_EXISTS)  //已经创建
                        {
                            m_bAlreadyExist = true;
                        }
                        else                                         //新创建
                        {
                            m_bAlreadyExist = false;
                        }
                    }
                    //---------------------------------------
                    //创建内存映射
                    m_pwData = MapViewOfFile(m_hSharedMemoryFile, FILE_MAP_ALL_ACCESS, 0, 0, 0/*(uint)lngSize*/);
                    if (m_pwData == IntPtr.Zero)
                    {
                        m_bInit = false;
                        CloseHandle(m_hSharedMemoryFile);
                        return 3; //创建内存映射失败
                    }
                    else
                    {
                        m_bInit = true;
                        if (m_bAlreadyExist == false)
                        {
                            //初始化
                        }
                    }
                    //----------------------------------------
                }
                else
                {
                    return 1; //参数错误     
                }

                return 0;     //创建成功
            }
            catch (System.Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
                //ToolHelper.Trace(ex);
                return 4;
            }
        }
        #endregion

        #region 关闭共享内存
        public void Close()
        {
            try
            {
                if (m_bInit)
                {
                    UnmapViewOfFile(m_pwData);
                    CloseHandle(m_hSharedMemoryFile);

                    m_bInit = false;
                }
            }
            catch (System.Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
                //ToolHelper.Trace(ex);
            }
        }
        #endregion

        #region 读数据
        public string Read()
        {
            try
            {
                if (m_bInit)
                {
                    //byte[] bytData = new byte[m_MemSize];
                    //Marshal.Copy(m_pwData, bytData, 0, m_MemSize);
                    //string sData = encoding.GetString(bytData);
                    string sData = Marshal.PtrToStringUni(m_pwData);
                    sData = sData.Trim(' ', '\0');
                    if (sData.IndexOf('\0') > 0)
                        sData = sData.Substring(0, sData.IndexOf('\0'));
                    return sData;
                }
                return "";
            }
            catch (System.Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
                //ToolHelper.Trace(ex);
                return "";
            }
        }
        #endregion

        #region 写数据
        public bool Write(string strData)
        {
            try
            {
                if (m_bInit)
                {
                    byte[] bytTemp = encoding.GetBytes(strData);
                    byte[] bytData = new byte[m_MemSize];
                    for (int i = 0; i < m_MemSize; i++)
                        bytData[i] = 0;
                    bytTemp.CopyTo(bytData, 0);
                    Marshal.Copy(bytData, 0, m_pwData, Math.Min(m_MemSize, bytData.Length));
                    return true;
                }
                return false;
            }
            catch (System.Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
                //ToolHelper.Trace(ex);
                return false;
            }
        }
        #endregion

    }
}
