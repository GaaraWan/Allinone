﻿
using JetEazy.ControlSpace;
using JetEazy.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace JetEazy.CCDSpace
{
    public class CameraConfig
    {

        #region INI Access Functions
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        void WriteINIValue(string section, string key, string value, string filepath)
        {
            WritePrivateProfileString(section, key, value, filepath);
        }
        string ReadINIValue(string section, string key, string defaultvaluestring, string filepath)
        {
            string retStr = "";

            StringBuilder temp = new StringBuilder(200);
            int Length = GetPrivateProfileString(section, key, "", temp, 200, filepath);

            retStr = temp.ToString();

            if (retStr == "")
                retStr = defaultvaluestring;
            //else
            //    retStr = retStr.Split('/')[0]; //把說明排除掉

            return retStr;

        }
        #endregion

        string INIFILE = "CAMERA.INI";


        private static readonly CameraConfig _instance = new CameraConfig();
        public static CameraConfig Instance
        {
            get
            {
                return _instance;
            }
        }

        public int COUNT = 2;

        public CameraPara[] cameras;

        public void Initial(string epath)
        {
            INIFILE = epath + "\\CAMERA.ini";
            Load();
        }

        public void Load()
        {
            COUNT = int.Parse(ReadINIValue("Camera Basic", "COUNT", COUNT.ToString(), INIFILE));

            cameras = new CameraPara[COUNT];
            int i = 0;
            while (i < COUNT)
            {
                cameras[i] = new CameraPara();
                cameras[i].Index = int.Parse(ReadINIValue("Camera " + i.ToString(), "Index", cameras[i].Index.ToString(), INIFILE));
                cameras[i].SerialNumber = ReadINIValue("Camera " + i.ToString(), "SerialNumber", cameras[i].SerialNumber.ToString(), INIFILE);
                cameras[i].IsDebug = ReadINIValue("Camera " + i.ToString(), "IsDebug", cameras[i].IsDebug.ToString(), INIFILE) == "1";
                cameras[i].Rotate = int.Parse(ReadINIValue("Camera " + i.ToString(), "Rotate", cameras[i].Rotate.ToString(), INIFILE));
                cameras[i].CfgPath = INIFILE.Replace("\\CAMERA.ini", "");
                cameras[i].CfgPath = ReadINIValue("Camera " + i.ToString(), "CfgPath", cameras[i].CfgPath.ToString(), INIFILE);

                i++;
            }
        }
        public void Save()
        {

            WriteINIValue("Camera Basic", "COUNT", COUNT.ToString(), INIFILE);

            //cameras = new CameraPara[COUNT];
            int i = 0;
            while (i < COUNT)
            {
                WriteINIValue("Camera " + i.ToString(), "Index", cameras[i].Index.ToString(), INIFILE);
                WriteINIValue("Camera " + i.ToString(), "SerialNumber", cameras[i].SerialNumber.ToString(), INIFILE);
                WriteINIValue("Camera " + i.ToString(), "IsDebug", (cameras[i].IsDebug ? "1" : "0"), INIFILE);
                WriteINIValue("Camera " + i.ToString(), "Rotate", cameras[i].Rotate.ToString(), INIFILE);
                WriteINIValue("Camera " + i.ToString(), "CfgPath", cameras[i].CfgPath.ToString(), INIFILE);

                i++;
            }

        }
    }
    public class CameraPara
    {
        public int Index { get; set; } = 0;
        public string SerialNumber { get; set; } = string.Empty;
        public bool IsDebug { get; set; } = false;
        public int Rotate { get; set; } = 0;
        public string CfgPath { get; set; } = "WORK";
        public string ToCameraString()
        {
            string str = "";

            str += Index + "@";
            str += SerialNumber + "@";
            str += (IsDebug ? "1" : "0") + "@";
            str += Rotate + "@";
            str += CfgPath + "@";

            return str;
        }
        public void FromCameraString(string eStr)
        {
            string[] strs = eStr.Split('@').ToArray();
            Index = int.Parse(strs[0]);
            SerialNumber = strs[1];
            IsDebug = strs[2] == "1";
            Rotate = int.Parse(strs[3]);
            CfgPath = strs[4];
        }
    }
#if OPT_NOT_USED
    public class CAMERAClass : ICam
    {
        Dvp2Class _cam = null;
        //CAM_HIKVISION _cam = null;
        Bitmap m_BmpError = new Bitmap(1, 1);
        //Bitmap m_BmpDebug = new Bitmap(1, 1);
        List<string> list_debugFiles = new List<string>();
        int dbgIndex = 0;

        CameraPara _camCfg = new CameraPara();
        //public void Dispose()
        //{
        //    if (m_BmpDebug != null)
        //        m_BmpDebug.Dispose();
        //    m_BmpDebug = null;
        //    if (m_BmpError != null)
        //        m_BmpError.Dispose();
        //    m_BmpError = null;
        //}
        public CameraPara CameraCfg { get { return _camCfg; } }
        public bool IsSim()
        {
            return _camCfg.IsDebug;
        }
        public int Initial(string inipara)
        {
            _camCfg.FromCameraString(inipara);
            string err_bmpPath = _camCfg.CfgPath + "\\Error.bmp";
            if (System.IO.File.Exists(err_bmpPath))
            {
                Bitmap bmp = new Bitmap(err_bmpPath);
                m_BmpError.Dispose();
                m_BmpError = new Bitmap(bmp);
                bmp.Dispose();
            }
            else
            {
                m_BmpError.Dispose();
                m_BmpError = new Bitmap(1, 1);
                Graphics g = Graphics.FromImage(m_BmpError);
                g.Clear(Color.Red);
                g.Dispose();
            }
            if (_camCfg.IsDebug)
            {
                list_debugFiles.Clear();
                dbgIndex = 0;
                string dbg_bmppath = _camCfg.CfgPath + "\\cam" + _camCfg.Index.ToString();
                if (System.IO.Directory.Exists(dbg_bmppath))
                {
                    string[] myFiles = System.IO.Directory.GetFiles(dbg_bmppath, "*.bmp");
                    foreach (string str in myFiles)
                        list_debugFiles.Add(str);
                    myFiles = System.IO.Directory.GetFiles(dbg_bmppath, "*.png");
                    foreach (string str in myFiles)
                        list_debugFiles.Add(str);
                    myFiles = System.IO.Directory.GetFiles(dbg_bmppath, "*.jpg");
                    foreach (string str in myFiles)
                        list_debugFiles.Add(str);
                    myFiles = System.IO.Directory.GetFiles(dbg_bmppath, "*.jpeg");
                    foreach (string str in myFiles)
                        list_debugFiles.Add(str);

                    //if(list_debugFiles.Count > 0)
                    //{
                    //    Bitmap bmp = new Bitmap(list_debugFiles[dbgIndex]);
                    //    m_BmpDebug.Dispose();
                    //    m_BmpDebug = new Bitmap(bmp);
                    //    bmp.Dispose();
                    //}
                }

                return 0;
            }

            //_cam = new CAM_HIKVISION(new System.Windows.Forms.PictureBox(), _camCfg.Index);
            ////_cam.Init(_camCfg.SerialNumber);
            //_cam.Init(_camCfg);
            ////_cam.RotateAngle = _camCfg.Rotate = 90;

            _cam = new Dvp2Class(new System.Windows.Forms.PictureBox(), _camCfg.Index);
            //_cam.Init(_camCfg.SerialNumber);
            return _cam.Init(_camCfg.SerialNumber, _camCfg.CfgPath);
            //_cam.RotateAngle = _camCfg.Rotate = 90;
        }
        public void Close()
        {
            //if (m_BmpDebug != null)
            //    m_BmpDebug.Dispose();
            //m_BmpDebug = null;
            if (m_BmpError != null)
                m_BmpError.Dispose();
            m_BmpError = null;

            if (_camCfg.IsDebug)
                return;
            if (_cam == null)
                return;

            _cam.Dispose();
        }
        public void SetExposure(int val)
        {
            if (_camCfg.IsDebug)
                return;

            if (_cam == null)
                return;
            _cam.SetExposure((float)(val * 1000f));
        }
        public void SetExposure(float val)
        {
            if (_camCfg.IsDebug)
                return;

            if (_cam == null)
                return;
            _cam.SetExposure((float)(val));
        }
        public void SetGain(float val)
        {
            if (_camCfg.IsDebug)
                return;

            if (_cam == null)
                return;

            _cam.SetGain(val);
        }
        public void StartCapture()
        {
            if (_camCfg.IsDebug)
                return;
            if (_cam == null)
                return;

            _cam.TriggerMode(true);
        }
        public void StopCapture()
        {
            if (_camCfg.IsDebug)
                return;
            if (_cam == null)
                return;

            _cam.TriggerMode(false);
        }
        public void Snap()
        {
            if (_camCfg.IsDebug)
                return;
            if (_cam == null)
                return;
            
            //_cam.TriggerSoftwareX();
        }
        public int RotateAngle
        {
            get { return _camCfg.Rotate; }
            set { _camCfg.Rotate = value; }
        }

        /// <summary>
        /// The caller must maintain the life cycle of the Bitmap returned by this function!!!
        /// </summary>
        /// <param name="msec"></param>
        /// <returns></returns>
        public Bitmap GetSnap(int msec = 1000)
        {
            #region DEBUG RETURN
            if (_camCfg.IsDebug)
            {
                Bitmap ret = null;  //  this ret must be new bitmap or clone() !!

                //is it possible that m_BmpError is null here?
                {
                    if (list_debugFiles.Count <= 0)
                    {
                        ret = (Bitmap)m_BmpError.Clone();
                    }
                    else
                    {
                        if (dbgIndex >= list_debugFiles.Count)
                            dbgIndex = 0;

                        Bitmap bmp = new Bitmap(list_debugFiles[dbgIndex]);
                        ret = new Bitmap(bmp);
                        bmp.Dispose();

                        dbgIndex++;
                    }
                }
                return ret;
            }
            #endregion

            if (_cam == null)
                return (Bitmap)m_BmpError.Clone();  // ok

            Bitmap newBitmapFrame = _cam.GetImageNow();
            //不旋轉圖像
            //if (newBitmapFrame != null)
            //    return newBitmapFrame;
            if (newBitmapFrame != null)
            {
                if (_camCfg.Rotate == 0)
                    return newBitmapFrame;
                Bitmap bitmap = new Bitmap(newBitmapFrame);
                switch (_camCfg.Rotate)
                {
                    case 90:
                        bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case 270:
                        bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                    case 180:
                        bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                }
                newBitmapFrame.Dispose();
                return bitmap;
            }
            return (Bitmap)m_BmpError.Clone();

            #region MASK is old Funtion
            //Stopwatch watch = new Stopwatch();
            //watch.Start();
            //while (true)
            //{
            //    Bitmap newBitmapFrame = _cam.CaptureBmp(_camCfg.Rotate);

            //    if (newBitmapFrame != null)
            //    {
            //        //var ret= (Bitmap)bmptemp.Clone();
            //        //bmptemp.Dispose();
            //        //return ret;
            //        return newBitmapFrame;
            //    }

            //    if (watch.ElapsedMilliseconds > msec)
            //        break;
            //}
            //return (Bitmap)m_BmpError.Clone();
            #endregion
        }
        public int GetFps()
        {
            if (_camCfg.IsDebug)
                return 0;
            if (_cam == null)
                return 0;
            return 0;
            //return _cam.iFpsCount;
        }
    }
    public class CAMERADAHUAClass : ICam
    {
        CAM_DAHUAVISON _cam = null;
        //CAM_HIKVISION _cam = null;
        Bitmap m_BmpError = new Bitmap(1, 1);
        //Bitmap m_BmpDebug = new Bitmap(1, 1);
        List<string> list_debugFiles = new List<string>();
        int dbgIndex = 0;

        CameraPara _camCfg = new CameraPara();
        //public void Dispose()
        //{
        //    if (m_BmpDebug != null)
        //        m_BmpDebug.Dispose();
        //    m_BmpDebug = null;
        //    if (m_BmpError != null)
        //        m_BmpError.Dispose();
        //    m_BmpError = null;
        //}
        public CameraPara CameraCfg { get { return _camCfg; } }
        public bool IsSim()
        {
            return _camCfg.IsDebug;
        }
        public int Initial(string inipara)
        {
            _camCfg.FromCameraString(inipara);
            string err_bmpPath = _camCfg.CfgPath + "\\Error.bmp";
            if (System.IO.File.Exists(err_bmpPath))
            {
                Bitmap bmp = new Bitmap(err_bmpPath);
                m_BmpError.Dispose();
                m_BmpError = new Bitmap(bmp);
                bmp.Dispose();
            }
            else
            {
                m_BmpError.Dispose();
                m_BmpError = new Bitmap(1, 1);
                Graphics g = Graphics.FromImage(m_BmpError);
                g.Clear(Color.Red);
                g.Dispose();
            }
            if (_camCfg.IsDebug)
            {
                list_debugFiles.Clear();
                dbgIndex = 0;
                string dbg_bmppath = _camCfg.CfgPath + "\\cam" + _camCfg.Index.ToString();
                if (System.IO.Directory.Exists(dbg_bmppath))
                {
                    string[] myFiles = System.IO.Directory.GetFiles(dbg_bmppath, "*.bmp");
                    foreach (string str in myFiles)
                        list_debugFiles.Add(str);
                    myFiles = System.IO.Directory.GetFiles(dbg_bmppath, "*.png");
                    foreach (string str in myFiles)
                        list_debugFiles.Add(str);
                    myFiles = System.IO.Directory.GetFiles(dbg_bmppath, "*.jpg");
                    foreach (string str in myFiles)
                        list_debugFiles.Add(str);
                    myFiles = System.IO.Directory.GetFiles(dbg_bmppath, "*.jpeg");
                    foreach (string str in myFiles)
                        list_debugFiles.Add(str);

                    //if(list_debugFiles.Count > 0)
                    //{
                    //    Bitmap bmp = new Bitmap(list_debugFiles[dbgIndex]);
                    //    m_BmpDebug.Dispose();
                    //    m_BmpDebug = new Bitmap(bmp);
                    //    bmp.Dispose();
                    //}
                }

                return 0;
            }

            //_cam = new CAM_HIKVISION(new System.Windows.Forms.PictureBox(), _camCfg.Index);
            ////_cam.Init(_camCfg.SerialNumber);
            //_cam.Init(_camCfg);
            ////_cam.RotateAngle = _camCfg.Rotate = 90;

            _cam = new CAM_DAHUAVISON(new System.Windows.Forms.PictureBox(), _camCfg.Index);
            //_cam.Init(_camCfg.SerialNumber);
            return _cam.Init(_camCfg);
            //_cam.RotateAngle = _camCfg.Rotate = 90;
        }
        public void Close()
        {
            //if (m_BmpDebug != null)
            //    m_BmpDebug.Dispose();
            //m_BmpDebug = null;
            if (m_BmpError != null)
                m_BmpError.Dispose();
            m_BmpError = null;

            if (_camCfg.IsDebug)
                return;
            if (_cam == null)
                return;

            _cam.Dispose();
        }
        public void SetExposure(int val)
        {
            if (_camCfg.IsDebug)
                return;

            if (_cam == null)
                return;
            _cam.SetExposure((float)(val * 1000f));
        }
        public void SetExposure(float val)
        {
            if (_camCfg.IsDebug)
                return;

            if (_cam == null)
                return;
            _cam.SetExposure((float)(val));
        }
        public void SetGain(float val)
        {
            if (_camCfg.IsDebug)
                return;

            if (_cam == null)
                return;

            _cam.SetGain(val);
        }
        public void StartCapture()
        {
            if (_camCfg.IsDebug)
                return;
            if (_cam == null)
                return;

            _cam.TriggerMode(0);
        }
        public void StopCapture()
        {
            if (_camCfg.IsDebug)
                return;
            if (_cam == null)
                return;

            _cam.TriggerMode(1);
        }
        public void Snap()
        {
            if (_camCfg.IsDebug)
                return;
            if (_cam == null)
                return;

            //_cam.TriggerSoftwareX();
        }
        public int RotateAngle
        {
            get { return _camCfg.Rotate; }
            set { _camCfg.Rotate = value; }
        }

        /// <summary>
        /// The caller must maintain the life cycle of the Bitmap returned by this function!!!
        /// </summary>
        /// <param name="msec"></param>
        /// <returns></returns>
        public Bitmap GetSnap(int msec = 1000)
        {
            #region DEBUG RETURN
            if (_camCfg.IsDebug)
            {
                Bitmap ret = null;  //  this ret must be new bitmap or clone() !!

                //is it possible that m_BmpError is null here?
                {
                    if (list_debugFiles.Count <= 0)
                    {
                        ret = (Bitmap)m_BmpError.Clone();
                    }
                    else
                    {
                        if (dbgIndex >= list_debugFiles.Count)
                            dbgIndex = 0;

                        Bitmap bmp = new Bitmap(list_debugFiles[dbgIndex]);
                        ret = new Bitmap(bmp);
                        bmp.Dispose();

                        dbgIndex++;
                    }
                }
                return ret;
            }
            #endregion

            if (_cam == null)
                return (Bitmap)m_BmpError.Clone();  // ok

            Bitmap newBitmapFrame = _cam.CaptureBmp();
            //不旋轉圖像
            //if (newBitmapFrame != null)
            //    return newBitmapFrame;
            if (newBitmapFrame != null)
            {
                if (_camCfg.Rotate == 0)
                    return newBitmapFrame;
                Bitmap bitmap = new Bitmap(newBitmapFrame);
                switch (_camCfg.Rotate)
                {
                    case 90:
                        bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case 270:
                        bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                    case 180:
                        bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                }
                newBitmapFrame.Dispose();
                return bitmap;
            }
            return (Bitmap)m_BmpError.Clone();

            #region MASK is old Funtion
            //Stopwatch watch = new Stopwatch();
            //watch.Start();
            //while (true)
            //{
            //    Bitmap newBitmapFrame = _cam.CaptureBmp(_camCfg.Rotate);

            //    if (newBitmapFrame != null)
            //    {
            //        //var ret= (Bitmap)bmptemp.Clone();
            //        //bmptemp.Dispose();
            //        //return ret;
            //        return newBitmapFrame;
            //    }

            //    if (watch.ElapsedMilliseconds > msec)
            //        break;
            //}
            //return (Bitmap)m_BmpError.Clone();
            #endregion
        }
        public int GetFps()
        {
            if (_camCfg.IsDebug)
                return 0;
            if (_cam == null)
                return 0;

            return _cam.iFpsCount;
        }
    }
    public class CAMERAHIKClass : ICam
    {
        //CAM_DAHUAVISON _cam = null;
        CAM_HIKVISION _cam = null;
        Bitmap m_BmpError = new Bitmap(1, 1);
        //Bitmap m_BmpDebug = new Bitmap(1, 1);
        List<string> list_debugFiles = new List<string>();
        int dbgIndex = 0;

        CameraPara _camCfg = new CameraPara();
        //public void Dispose()
        //{
        //    if (m_BmpDebug != null)
        //        m_BmpDebug.Dispose();
        //    m_BmpDebug = null;
        //    if (m_BmpError != null)
        //        m_BmpError.Dispose();
        //    m_BmpError = null;
        //}
        public bool IsSim()
        {
            return _camCfg.IsDebug;
        }
        public int Initial(string inipara)
        {
            _camCfg.FromCameraString(inipara);
            string err_bmpPath = _camCfg.CfgPath + "\\Error.bmp";
            if (System.IO.File.Exists(err_bmpPath))
            {
                Bitmap bmp = new Bitmap(err_bmpPath);
                m_BmpError.Dispose();
                m_BmpError = new Bitmap(bmp);
                bmp.Dispose();
            }
            else
            {
                m_BmpError.Dispose();
                m_BmpError = new Bitmap(1, 1);
                Graphics g = Graphics.FromImage(m_BmpError);
                g.Clear(Color.Red);
                g.Dispose();
            }
            if (_camCfg.IsDebug)
            {
                list_debugFiles.Clear();
                dbgIndex = 0;
                string dbg_bmppath = _camCfg.CfgPath + "\\cam" + _camCfg.Index.ToString();
                if (System.IO.Directory.Exists(dbg_bmppath))
                {
                    string[] myFiles = System.IO.Directory.GetFiles(dbg_bmppath, "*.bmp");
                    foreach (string str in myFiles)
                        list_debugFiles.Add(str);
                    myFiles = System.IO.Directory.GetFiles(dbg_bmppath, "*.png");
                    foreach (string str in myFiles)
                        list_debugFiles.Add(str);
                    myFiles = System.IO.Directory.GetFiles(dbg_bmppath, "*.jpg");
                    foreach (string str in myFiles)
                        list_debugFiles.Add(str);
                    myFiles = System.IO.Directory.GetFiles(dbg_bmppath, "*.jpeg");
                    foreach (string str in myFiles)
                        list_debugFiles.Add(str);

                    //if(list_debugFiles.Count > 0)
                    //{
                    //    Bitmap bmp = new Bitmap(list_debugFiles[dbgIndex]);
                    //    m_BmpDebug.Dispose();
                    //    m_BmpDebug = new Bitmap(bmp);
                    //    bmp.Dispose();
                    //}
                }

                return 0;
            }

            _cam = new CAM_HIKVISION(new System.Windows.Forms.PictureBox(), _camCfg.Index);
            //_cam.Init(_camCfg.SerialNumber);
            _cam.Init(_camCfg);
            //_cam.RotateAngle = _camCfg.Rotate = 90;

            //_cam = new CAM_DAHUAVISON(new System.Windows.Forms.PictureBox(), _camCfg.Index);
            ////_cam.Init(_camCfg.SerialNumber);
            //_cam.Init(_camCfg);
            ////_cam.RotateAngle = _camCfg.Rotate = 90;
            ///
            return 0;
        }
        public void Close()
        {
            //if (m_BmpDebug != null)
            //    m_BmpDebug.Dispose();
            //m_BmpDebug = null;
            if (m_BmpError != null)
                m_BmpError.Dispose();
            m_BmpError = null;

            if (_camCfg.IsDebug)
                return;
            if (_cam == null)
                return;

            _cam.Dispose();
        }
        public void SetExposure(int val)
        {
            if (_camCfg.IsDebug)
                return;

            if (_cam == null)
                return;

            MvCamCtrl.NET.MyCamera.MVCC_FLOATVALUE stParam = new MvCamCtrl.NET.MyCamera.MVCC_FLOATVALUE();
            _cam.GetFloatValue_NET(ref stParam);
            //if (val < stParam.fMin && val > stParam.fMax)
            //    val = 1000;
            //_cam.SetExposure((float)val / 1000f * stParam.fMax);
            _cam.SetExposure((float)(val * 1000f));
            //_cam.SetFramerate(100);
        }
        public void SetExposure(float val)
        {
            if (_camCfg.IsDebug)
                return;

            if (_cam == null)
                return;

            MvCamCtrl.NET.MyCamera.MVCC_FLOATVALUE stParam = new MvCamCtrl.NET.MyCamera.MVCC_FLOATVALUE();
            _cam.GetFloatValue_NET(ref stParam);
            if (val < stParam.fMin && val > stParam.fMax)
                val = 1000;
            //_cam.SetExposure((float)val / 1000f * stParam.fMax);
            _cam.SetExposure(val);
            //_cam.SetFramerate(100);
        }
        public void SetGain(float val)
        {
            if (_camCfg.IsDebug)
                return;

            if (_cam == null)
                return;

            _cam.SetGain(val);
        }
        public void StartCapture()
        {
            if (_camCfg.IsDebug)
                return;
            if (_cam == null)
                return;

            _cam.TriggerMode(0);
        }
        public void StopCapture()
        {
            if (_camCfg.IsDebug)
                return;
            if (_cam == null)
                return;

            _cam.TriggerMode(1);
        }
        public void Snap()
        {
            if (_camCfg.IsDebug)
                return;
            if (_cam == null)
                return;

            //_cam.TriggerSoftwareX();
        }
        public int RotateAngle
        {
            get { return _camCfg.Rotate; }
            set { _camCfg.Rotate = value; }
        }

        /// <summary>
        /// The caller must maintain the life cycle of the Bitmap returned by this function!!!
        /// </summary>
        /// <param name="msec"></param>
        /// <returns></returns>
        public Bitmap GetSnap(int msec = 1000)
        {
            #region DEBUG RETURN
            if (_camCfg.IsDebug)
            {
                Bitmap ret = null;  //  this ret must be new bitmap or clone() !!

                //is it possible that m_BmpError is null here?
                {
                    if (list_debugFiles.Count <= 0)
                    {
                        ret = (Bitmap)m_BmpError.Clone();
                    }
                    else
                    {
                        if (dbgIndex >= list_debugFiles.Count)
                            dbgIndex = 0;

                        Bitmap bmp = new Bitmap(list_debugFiles[dbgIndex]);
                        ret = new Bitmap(bmp);
                        bmp.Dispose();

                        dbgIndex++;
                    }
                }
                return ret;
            }
            #endregion

            if (_cam == null)
                return (Bitmap)m_BmpError.Clone();  // ok

            Bitmap newBitmapFrame = _cam.GetImageNow();
            //不旋轉圖像
            //if (newBitmapFrame != null)
            //    return newBitmapFrame;
            if (newBitmapFrame != null)
            {
                if (_camCfg.Rotate == 0)
                    return newBitmapFrame;
                Bitmap bitmap = new Bitmap(newBitmapFrame);
                switch (_camCfg.Rotate)
                {
                    case 90:
                        bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case 270:
                        bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                    case 180:
                        bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                }
                newBitmapFrame.Dispose();
                return bitmap;
            }
            return (Bitmap)m_BmpError.Clone();

            #region MASK is old Funtion
            //Stopwatch watch = new Stopwatch();
            //watch.Start();
            //while (true)
            //{
            //    Bitmap newBitmapFrame = _cam.CaptureBmp(_camCfg.Rotate);

            //    if (newBitmapFrame != null)
            //    {
            //        //var ret= (Bitmap)bmptemp.Clone();
            //        //bmptemp.Dispose();
            //        //return ret;
            //        return newBitmapFrame;
            //    }

            //    if (watch.ElapsedMilliseconds > msec)
            //        break;
            //}
            //return (Bitmap)m_BmpError.Clone();
            #endregion
        }
        public int GetFps()
        {
            if (_camCfg.IsDebug)
                return 0;
            if (_cam == null)
                return 0;

            return _cam.iCount;
        }
    }
#endif
}
