using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using JetEazy;
using JetEazy.ControlSpace;
using JetEazy.BasicSpace;
using JetEazy.PlugSpace;
using System.Threading;
using JetEazy.Interface;

namespace Allinone.OPSpace.ResultSpace
{
    public enum Result_EA : int
    {
        Allinone = 0,
        KBAOI = 1,
        KBHeight = 2,
        KBOffset = 3,
        KBGap = 4,

        Audix = 5,
        AudixDfly = 6,
        R32 = 7,
        RXX = 8,
        R15 = 9,
        R9 = 10,
        R3 = 11,
        R1 = 12,
        R5 = 13,
        C3 = 14,
        MAIN_SD = 15,
        MAIN_X6=16,
        MAIN_SDM1=17,
        MAIN_SDM2= 18,
        MAIN_SERVICE=19,
        MAIN_SDM3 = 20,
        MAIN_SDM5 = 21,
    };

    public class DupItemClass
    {
        string ID = "";
        int Count = 0;
        DateTime LastTime;

        public DupItemClass(string id)
        {
            ID = id;
            Count = 1;
            LastTime = DateTime.Now;
        }
        public bool CheckDup(string id)
        {
            bool ret = false;

            if (ID == id)
            {
                Count++;

                if (Count < 3)
                    LastTime = DateTime.Now;

                ret = true;
            }
            return ret;
        }
        public bool ChekcOverTime()
        {
            bool IsOverTime = false;

            if (DateTime.Now.Subtract(LastTime).TotalSeconds > 90)
            {
                IsOverTime = true;
                LastTime = DateTime.Now;
            }
            else
                IsOverTime = false;

            return Count == 2 || (Count > 2 && IsOverTime);
        }

        public int ChekcTimes()
        {
            return Count;
        }
    }
    public class DupClass
    {
        List<DupItemClass> DupItemList = new List<DupItemClass>();

        public DupClass()
        {

        }
        public bool CheckIsOK(string id)
        {
            bool IsOKToCountinue = false;

            int i = 0;

            if (DupItemList.Count == 0)
            {
                IsOKToCountinue = true;
            }
            else
            {
                while (i < DupItemList.Count)
                {
                    if (DupItemList[i].CheckDup(id))
                    {
                        if (DupItemList[i].ChekcOverTime())
                            IsOKToCountinue = true;

                        break;
                    }
                    i++;
                }

                if (i == DupItemList.Count)
                    IsOKToCountinue = true;
            }

            return IsOKToCountinue;
        }

        public int CheckTime(string id)
        {
            int i = 0;
            int ret = 0;

            if (DupItemList.Count == 0)
            {
                Check(id, false);
            }
            else
            {
                while (i < DupItemList.Count)
                {
                    if (DupItemList[i].CheckDup(id))
                    {
                        ret = DupItemList[i].ChekcTimes();
                        break;
                    }
                    i++;
                }
            }

            return ret;
        }
        public void CheckTime(string id, bool ispass)
        {
            int i = DupItemList.Count - 1;

            while (i > -1)
            {
                if (DupItemList[i].CheckDup(id))
                {
                    if (ispass)
                        DupItemList.RemoveAt(i);
                    break;
                }
                i--;
            }
        }

        public void Check(string id, bool ispass)
        {
            bool IsDup = false;

            int i = DupItemList.Count - 1;

            while (i > -1)
            {
                if (DupItemList[i].CheckDup(id))
                {
                    if (ispass)
                        DupItemList.RemoveAt(i);

                    IsDup = true;
                    break;
                }
                i--;
            }
            if (!IsDup)
            {
                DupItemList.Add(new DupItemClass(id));
            }
        }
    }

    [Serializable]
    public abstract class GeoResultClass
    {
        protected VersionEnum VERSION;
        protected OptionEnum OPTION;
        public string RELATECOLORSTR = "";

        protected Result_EA myResultEA;
        protected JzTimes myJztimes;

        public int DirIndex;
        protected string DebugSavePath;
        protected string DebugDirPath;
        protected string[] Dirs;
        public string DebugStringNow;
        public string LastDirPath;
        protected string DebugRecipeSaveName;

        public ProcessClass MainProcess;

        protected AlbumClass AlbumWork;
        protected CCDCollectionClass CCDCollection;
        //protected IxLineScanCam m_IxLinescanCamera;
        //protected bool IsDirect = false;
        protected TestMethodEnum TestMethod = TestMethodEnum.BUTTON;
        protected bool IsNoUseCCD = false;
        protected int EnvIndex = 0;
        protected bool IsPass = false;
        public int[] DelayTime = new int[10];
        protected bool IsCheckSnPass = false;

        public bool IsStopNormalTick = false;

        protected DupClass DUP;
        public int DirsCount
        {
            get { return Dirs.Length; }
        }

        //Create by Gaara 2020/10/21
        /// <summary>
        /// 存放初始的图片 用于hive上传的资料
        /// </summary>
        public List<Bitmap> listBmpHiveTemp = new List<Bitmap>();
        //Bitmap m_bmptemp = new Bitmap(1, 1);
        public void SetAlbumWorkBmps(AlbumClass eAlbum)
        {
            listBmpHiveTemp.Clear();
            string strPath = "D:\\LOA\\HIVETEMP";
            if (!Directory.Exists(strPath))
                Directory.CreateDirectory(strPath);
            foreach (PageClass page in eAlbum.ENVList[0].PageList)
            {
                //m_bmptemp.Dispose();
                Bitmap m_bmptemp = new Bitmap(page.GetbmpRUN()); //Bitmap m_bmptemp = new Bitmap(CCDCollection.GetBMP(page.CamIndex, false));
                listBmpHiveTemp.Add(m_bmptemp);
                m_bmptemp.Save(strPath + "\\" + page.PageRunNo + ".png");
                //m_bmptemp.Dispose();
            }
        }

        //Create by Gaara 2019/12/26
        /// <summary>
        /// 存放測試結束後，產生的相機圖片及截圖，路徑供Hive調用
        /// </summary>
        public string Path_Hive_Pictures = "";
        /// <summary>
        /// 存放測試結束後，產生的單片數據，完整路徑名稱供Hive調用
        /// </summary>
        public string FullPathName_Hive_Reports = "";

        public WorkStatusCollectionClass RunStatusCollection = new WorkStatusCollectionClass();

        public virtual void SetPara(AlbumClass albumwork, CCDCollectionClass ccocollection)
        {

        }
        //public virtual void AddLinescanCamera(IxLineScanCam elinescancam)
        //{
            
        //}
        /// <summary>
        /// 準備要開始的一些資料
        /// </summary>
        public abstract void GetStart(AlbumClass albumwork, CCDCollectionClass ccocollection, TestMethodEnum testmethod, bool isnouseccd);
        /// <summary>
        /// 
        /// </summary>
        public abstract void Tick();
        public abstract void GenReport();
        protected abstract void MainProcessTick();
        /// <summary>
        /// 若OperationIndex為-1時則是回復到最初的樣子
        /// </summary>
        /// <param name="operationindex"></param>
        public abstract void ResetData(int operationindex);
        public abstract void SetDelayTime();
        public abstract void FillProcessImage();
        public void SetSaveDirectory(string debugsavepath)
        {
            DebugSavePath = debugsavepath;
        }
        public void RefreshDebugSrcDirectory(string debugsrcpath)
        {
            DebugDirPath = debugsrcpath;

            Dirs = Directory.GetDirectories(DebugDirPath);
            DirIndex = 0;
        }

        int debugindex = 0;
        List<string> listPath = new List<string>();
        public string GetDirPath(string debugPath)
        {
            List<string> listpath = new List<string>();
            DirectoryInfo root = new DirectoryInfo(debugPath);
            foreach (DirectoryInfo d in root.GetDirectories())
            {
                listpath.Add(d.Name);
            }

            if (debugindex >= listpath.Count)
                debugindex = 0;
            string name = listpath[debugindex];
            debugindex++;

            return name;
        }


        /// <summary>
        /// 返回路经
        /// </summary>
        /// <returns></returns>
        public string GetLastDirPath(string debugsrcpath)
        {
            DebugDirPath = debugsrcpath;

            Dirs = Directory.GetDirectories(DebugDirPath);
            if (DirIndex >= Dirs.Length)
                DirIndex = 0;

            string Str = Dirs[DirIndex];
            LastDirPath = Str;
            return LastDirPath;
        }
        public string GetDebugDirectory()
        {
            string Str = Dirs[DirIndex];

            LastDirPath = Str;

            string[] strs;
            strs = Str.Split('\\');

            DebugStringNow = "(" + DirIndex.ToString("000") + ") " + strs[strs.Length - 1];

            DirIndex += 1;

            if (DirIndex == Dirs.Length)
            {
                RefreshDebugSrcDirectory(DebugDirPath);
            }

            return DebugStringNow;
        }

        public void SetDebugRecipeSaveName(string debugrecipesavename)
        {
            DebugRecipeSaveName = debugrecipesavename;
        }

        /// <summary>
        /// savestring 要能用 000,001,002 來指定要存的內容
        /// </summary>
        /// <param name="savetostr"></param>
        /// <param name="saveenv"></param>
        /// <param name="savestring"></param>
        /// <param name="pageoptype"></param>
        public void SaveImage(string savetostr, EnvClass saveenv, string savestring, PageOPTypeEnum pageoptype)
        {
         //   int i = 0;
            string savepath = DebugSavePath + "\\" + DebugRecipeSaveName;

            //if (!Directory.Exists(savepath))
            //    Directory.CreateDirectory(savepath);

            string strMess = "";
            string[] savestrs;
            string saveSNpath = "";
            switch (OPTION)
            {
                case OptionEnum.R3:
                case OptionEnum.C3:
                    strMess = JzToolsClass.PassingBarcode;
                    saveSNpath = DebugSavePath + "\\" + DebugRecipeSaveName + " " + strMess + "_" + savetostr + "\\OCRTexting.txt";
                    break;
                default:
                   // strMess = Universal.DATASNTXT.Replace(',', ' ').Replace('/', ' ').Replace('$', ' ');
                    strMess = JzToolsClass.PassingBarcode;
                    saveSNpath = DebugSavePath + "\\" + DebugRecipeSaveName + " " + strMess+"_" + savetostr + "\\SN.txt";
                    break;


            }
            savestrs = (savestring + ":").Split(':');
            if (savestrs[1] == "")
                savepath = savepath + " " + strMess + "_" + savetostr + "\\" + saveenv.No.ToString(EnvClass.ORGENVNOSTRING);
            else
                savepath = savepath + " " + strMess+"_" + savetostr + "\\" + saveenv.No.ToString(EnvClass.ORGENVNOSTRING) + "\\" + savestrs[1];

            if (!Directory.Exists(savepath))
                Directory.CreateDirectory(savepath);

            string strSaveMess = "";
            if (Universal.DATASNTXT.IndexOf("$") > -1)
                strSaveMess = Universal.DATASNTXT;
            else
                strSaveMess = Universal.DATASNTXT + ",,,,,," + Universal.RELATECOLORSTR;

            switch (OPTION)
            {
                case OptionEnum.R3:
                case OptionEnum.C3:
                    strSaveMess = Universal.DATASNTXT;
                    break;
            }

            StreamWriter writer = new StreamWriter(saveSNpath);
            writer.Write(strSaveMess);
            writer.Close();

            //if (saveenv.PageList.Count > 1)
            //{
            //    saveenv.PageList[0].SaveRUNBMP(savepath, savestrs[0], pageoptype);
            //    saveenv.PageList[1].SaveRUNBMP(savepath, savestrs[0], pageoptype);
            //}
            foreach (PageClass page in saveenv.PageList)
            {
                page.SaveRUNBMP(savepath, savestrs[0], pageoptype);
            }
        }

        /// <summary>
        /// savestring 要能用 000,001,002 來指定要存的內容
        /// </summary>
        /// <param name="savetostr"></param>
        /// <param name="saveenv"></param>
        /// <param name="savestring"></param>
        /// <param name="pageoptype"></param>
        public void SaveDebugImage(string savetostr, EnvClass saveenv, string savestring, PageOPTypeEnum pageoptype)
        {
            int i = 0;
            string savepath = Universal.DEBUGRESULTPATH + "\\" + DebugRecipeSaveName;

            //if (!Directory.Exists(savepath))
            //    Directory.CreateDirectory(savepath);
            string strMess = "";
            string[] savestrs ;
            string saveSNpath = "";
            switch(OPTION)
            {
                case OptionEnum.R3:
                case OptionEnum.C3:
                    strMess = JzToolsClass.PassingBarcode ;
                    saveSNpath = Universal.DEBUGRESULTPATH + "\\" + DebugRecipeSaveName + " " + strMess + savetostr + "\\OCRTexting.txt";
                    break;
                default:
                    //strMess = Universal.DATASNTXT.Replace(',', ' ').Replace('/', ' ');
                    //saveSNpath = Universal.DEBUGRESULTPATH + "\\" + DebugRecipeSaveName + " " + strMess + savetostr + "\\SN.txt";

                    strMess = JzToolsClass.PassingBarcode;
                    saveSNpath = DebugSavePath + "\\" + DebugRecipeSaveName + " " + strMess + "_" + savetostr + "\\SN.txt";
                    break;

                   
            }
            savestrs = (savestring + ":").Split(':');
            if (savestrs[1] == "")
                savepath = savepath + " " + strMess + savetostr + "\\" + saveenv.No.ToString(EnvClass.ORGENVNOSTRING);
            else
                savepath = savepath + " " + strMess + savetostr + "\\" + saveenv.No.ToString(EnvClass.ORGENVNOSTRING) + "\\" + savestrs[1];


            if (!Directory.Exists(savepath))
                Directory.CreateDirectory(savepath);

            string strSaveMess = "";
            if (Universal.DATASNTXT.IndexOf("$") > -1)
                strSaveMess = Universal.DATASNTXT;
            else
                strSaveMess = Universal.DATASNTXT + ",,,,,," + Universal.RELATECOLORSTR;

            switch (OPTION)
            {
                case OptionEnum.R3:
                case OptionEnum.C3:
                    strSaveMess = Universal.DATASNTXT;
                    break;
            }

            StreamWriter writer = new StreamWriter(saveSNpath,true, Encoding.Default );
            writer.Write(strSaveMess);
            writer.Close();
            //if (saveenv.PageList.Count > 1)
            //    saveenv.PageList[1].SaveRUNBMP(savepath, savestrs[0], pageoptype);
            foreach (PageClass page in saveenv.PageList)
            {
                page.SaveRUNBMP(savepath, savestrs[0], pageoptype);
            }
        }

        /// <summary>
        /// savestring 要能用 000,001,002 來指定要存的內容
        /// </summary>
        /// <param name="savetostr"></param>
        /// <param name="saveenv"></param>
        /// <param name="savestring"></param>
        /// <param name="pageoptype"></param>
        public void SaveDebugImageA(string savetostr, EnvClass saveenv, string savestring, PageOPTypeEnum pageoptype)
        {
            int i = 0;
            string savepath = Universal.TESTRESULTPATH + "\\" + DebugRecipeSaveName;

            //if (!Directory.Exists(savepath))
            //    Directory.CreateDirectory(savepath);
            string strMess = "";
            string[] savestrs;
            string saveSNpath = "";
            switch (OPTION)
            {
                case OptionEnum.R3:
                case OptionEnum.C3:
                    strMess = JzToolsClass.PassingBarcode;
                    saveSNpath = Universal.DEBUGRESULTPATH + "\\" + DebugRecipeSaveName + " " + strMess + savetostr + "\\OCRTexting.txt";
                    break;
                default:
                    //strMess = Universal.DATASNTXT.Replace(',', ' ').Replace('/', ' ');
                    //saveSNpath = Universal.DEBUGRESULTPATH + "\\" + DebugRecipeSaveName + " " + strMess + savetostr + "\\SN.txt";

                    strMess = JzToolsClass.PassingBarcode;
                    saveSNpath = DebugSavePath + "\\" + DebugRecipeSaveName + " " + strMess + "_" + savetostr + "\\SN.txt";
                    break;


            }
            savestrs = (savestring + ":").Split(':');
            if (savestrs[1] == "")
                savepath = savepath + " " + strMess + savetostr + "\\" + saveenv.No.ToString(EnvClass.ORGENVNOSTRING);
            else
                savepath = savepath + " " + strMess + savetostr + "\\" + saveenv.No.ToString(EnvClass.ORGENVNOSTRING) + "\\" + savestrs[1];

            if (!Directory.Exists(savepath))
                Directory.CreateDirectory(savepath);

            string strSaveMess = "";
            if (Universal.DATASNTXT.IndexOf("$") > -1)
                strSaveMess = Universal.DATASNTXT;
            else
                strSaveMess = Universal.DATASNTXT + ",,,,,," + Universal.RELATECOLORSTR;

            StreamWriter writer = new StreamWriter(Universal.DEBUGRESULTPATH + "\\" + DebugRecipeSaveName + " " + strMess + savetostr + "\\SN.txt");
            writer.Write(strSaveMess);
            writer.Close();
            //if (saveenv.PageList.Count > 1)
            //    saveenv.PageList[1].SaveRUNBMP(savepath, savestrs[0], pageoptype);
            foreach (PageClass page in saveenv.PageList)
            {
                page.SaveRUNBMP(savepath, savestrs[0], pageoptype);
            }
        }

        public bool TestAndReadData(ref string datastr, string FileName)
        {
            try
            {
                FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader Srr = new StreamReader(fs, Encoding.Default);

                datastr = Srr.ReadToEnd();

                Srr.Close();
                Srr.Dispose();

                return true;
            }
            catch (Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
                return false;
            }

        }


        public static void CopyEntireDir(string sourcePath, string destPath)
        {
            //Now Create all of the directories        
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destPath));
            //Copy all the files & Replaces any files with the same name      
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(sourcePath, destPath), true);
        }

        private static void CopyFolder(string from, string to)
        {
            if (!Directory.Exists(to))
                Directory.CreateDirectory(to);

            // 子文件夹
            foreach (string sub in Directory.GetDirectories(from))
                CopyFolder(sub + "\\", to + Path.GetFileName(sub) + "\\");

            // 文件
            foreach (string file in Directory.GetFiles(from))
                File.Copy(file, to + Path.GetFileName(file), true);
        }

        public virtual string QFactorySend(JzQFactoryClass eQFactory, QFactoryErrorCode eErrorCode)
        {
            if (eQFactory != null)
                return eQFactory.Send(eErrorCode);

            return "NULL";
        }
        public virtual void Reset()
        {

        }
        public virtual void OneKeyGetImage()
        {

        }
        public virtual void ManualChangeRecipe()
        {

        }
        public virtual void OneKeyAreaGetImage()
        {

        }
        public EnvAnalyzePostionSettings ENVAnalyzePostion = null;

        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        private bool m_LogProcessSwitch = false;
        public bool LogProcessSwitch
        {
            get { return m_LogProcessSwitch; }
            set { m_LogProcessSwitch = value; }
        }
        public virtual void LogProcessIDTimer(int eProcessId, string eProcessIdName = "", bool eUseWatchStop = true)
        {
            if (!m_LogProcessSwitch)
                return;
            string timerElapsedStr = eProcessIdName + " [" + eProcessId + "]: ";

            if (eUseWatchStop)
            {
                watch.Stop();
                timerElapsedStr = eProcessIdName + " [" + eProcessId + "]: " + watch.ElapsedMilliseconds + "ms";
                watch.Reset();
                watch.Start();
            }

            string logpath = @"D:\log\" + OPTION.ToString() + "\\LogProcessIDTimer\\" + JzTimes.DateSerialString;
            string logname_HH = "TestTimer_" + DateTime.Now.ToString("yyyyMMdd_HH") + ".txt";

            if (!Directory.Exists(logpath))
                Directory.CreateDirectory(logpath);

            StreamWriter writer = null;
            try
            {
                if (writer == null)
                    writer = new StreamWriter(logpath + "\\" + logname_HH, true, Encoding.UTF8);

                writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + timerElapsedStr);

            }
            catch
            {

            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                    writer = null;
                }
            }
            ////StreamWriter writer = new StreamWriter("D:\\TestTimer.txt", true, Encoding.UTF8);
            //StreamWriter writer = new StreamWriter(logpath + "\\" + logname_HH, true, Encoding.UTF8);
            //writer.WriteLine(timerElapsedStr);
            //writer.Close();
        }

        public void SaveData(string DataStr, string FileName)
        {
            File.WriteAllText(FileName, DataStr, Encoding.Default);
        }

        #region PRIVATE_THREAD_FUNCTIONS
        private Thread _thread = null;
        private bool _runFlag = false;
        private bool _isThreadStopping = false;

        public virtual bool is_thread_running()
        {
            return _runFlag || _thread != null;
        }
        public virtual void start_scan_thread()
        {
            if (!is_thread_running())
            {
                _runFlag = true;
                _thread = new Thread(thread_func);
                //_thread.Name = this.Name;
                _thread.Start();
            }
            else
            {
                //GdxGlobal.LOG.Warn("有 Thread 尚未結束");
                CommonLogClass.Instance.LogError("有 Thread 尚未結束");
            }
        }
        public virtual void stop_scan_thread(int timeout = 3000)
        {
            if (is_thread_running())
            {
                _runFlag = false;
                var stopFunc = new Action<int>((tmout) =>
                {
                    if (!_isThreadStopping)
                    {
                        _isThreadStopping = true;
                        try
                        {
                            var t = _thread;
                            if (t != null)
                            {
                                if (!t.Join(tmout))
                                    t.Abort();
                                _thread = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            //GdxGlobal.LOG.Warn(ex, "Thread 終止異常!");
                            CommonLogClass.Instance.LogError("Thread 終止異常!");
                        }
                        _isThreadStopping = false;
                    }
                });
                stopFunc.BeginInvoke(timeout, null, null);
            }
        }
        public virtual void thread_func(object arg)
        {
            //var phase = (XRunContext)arg;

            while (_runFlag)
            {
                try
                {
                    ////>>> 確保 PLC 有效 scanned 出現 2次 以上
                    //if (!IsValidPlcScanned(2))
                    //{
                    //    Thread.Sleep(2);
                    //    continue;
                    //}

                    //phase.StepFunc(phase);

                    //if (!phase.Go)
                    //    break;

                    //if (phase.IsCompleted)
                    //{
                    //    if (phase.RunCount == 0)
                    //        _LOG(phase.Name, "補償 = 0");
                    //    break;
                    //}
                }
                catch (Exception ex)
                {
                    //if (_runFlag)
                    //{
                    //    try
                    //    {
                    //        _LOG(ex, "live compensating 異常!");
                    //        SetNextState(9999);
                    //    }
                    //    catch
                    //    {
                    //    }
                    //}
                    break;
                }
            }

            _runFlag = false;
            _thread = null;

            //int nextState = phase.ExitCode;
            //SetNextState(nextState);
            //base.IsOn = true;
        }
        #endregion

        //Folder是需要复制的总目录，lastpath是目标目录
        //private void CopyFile(DirectoryInfo Folders, string lastpath)
        //{
        //    //首先复制目录下的文件
        //    foreach (FileInfo fileInfo in Folders.GetFiles())
        //    {
        //        if (fileInfo.Exists)
        //        {
        //            //如果列表有记录的文件，就跳过
        //            if (filePaths.Contains(fileInfo.FullName))
        //                continue;


        //            string filename = fileInfo.FullName.Substring(fileInfo.FullName.LastIndexOf('\\'));

        //            fileInfo.CopyTo(lastpath + filename, true);
        //        }
        //    }

        //    //其次复制目录下的文件夹，并且进行遍历
        //    foreach (DirectoryInfo Folder in Folders.GetDirectories())
        //    {
        //        //如果有记录在列表中，则跳过该目录
        //        if (folderPaths.Contains(Folder.FullName)) continue;
        //        string Foldername = Folder.FullName.Substring(Folder.FullName.LastIndexOf('\\'));
        //        //复制后文件夹目录
        //        string copypath = lastpath + Foldername;
        //        //创建文件夹
        //        if (!Directory.Exists(copypath))
        //            Directory.CreateDirectory(copypath);
        //        //将目录加深，遍历子目录中的文件
        //        lastpath = copypath;
        //        //子目录递归调用，遍历子目录
        //        CopyFile(Folder, lastpath);
        //        //上一个子目录中归来，还原目录深度，循环至下一子目录
        //        lastpath = lastpath.Substring(0, lastpath.LastIndexOf('\\'));




        //    }
        //}

        public delegate void TriggerHandler(ResultStatusEnum resultstatus);
        public event TriggerHandler TriggerAction;
        public void OnTrigger(ResultStatusEnum resultstatus)
        {
            if (TriggerAction != null)
            {
                TriggerAction(resultstatus);
            }
        }

        public delegate void EnvTriggerHandler(ResultStatusEnum resultstatus, int envindex, string operpagestr);
        public event EnvTriggerHandler EnvTriggerAction;
        public void OnEnvTrigger(ResultStatusEnum resultstatus, int envindex, string operpagestr)
        {
            if (EnvTriggerAction != null)
            {
                EnvTriggerAction(resultstatus, envindex, operpagestr);
            }
        }

        public delegate void TriggerOPHandler(ResultStatusEnum resultstatus, string str);
        public event TriggerOPHandler TriggerOPAction;
        public void OnTriggerOP(ResultStatusEnum resultstatus, string str)
        {
            if (TriggerOPAction != null)
            {
                TriggerOPAction(resultstatus, str);
            }
        }


        public delegate void TriggerOPMessIng( string str);
        public event TriggerOPMessIng TriggerOPMess;
        public void OnTriggerMess( string str)
        {
            if (TriggerOPMess != null)
            {
                TriggerOPMess( str);
            }
        }

        public delegate void TriggerShowImage(Bitmap ebmpInput);
        public event TriggerShowImage TriggerShowImageCurrent;
        public void OnTriggerShowImageCurrent(Bitmap ebmpInput)
        {
            if (TriggerShowImageCurrent != null)
            {
                OnTriggerShowImageCurrent(ebmpInput);
            }
        }
    }
}
