using System;
using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

//using JzKHC.UniversalSpace;
using JzKHC.ControlSpace;
using JetEazy.BasicSpace;
using JzKHC.DBSpace;
using JzKHC.FormSpace;
using JetEazy.ImageBuffer;
//using JetEazy.Drivers.CameraControl;

namespace JzKHC.ControlSpace
{
    public class CCDOfflineClass
    {
        #region Library

            public static int OrgWidth = 3488;
            public static int OrgHeight = 2616;
            
            public static int RefreshTime = 30;

            const int STRETCH_DELETESCANS = 3;

            //[DllImport("c:\\xclib\\xclibwnt.dll")]
            //private static extern int pxd_PIXCIopen(string c_driverparms, string c_formatname, string c_formatfile);

            [DllImport("XCLIBW64.dll")]
            private static extern int pxd_PIXCIopen(string c_driverparms, string c_formatname, string c_formatfile);
            [DllImport("XCLIBW64.dll")]
            private static extern int pxd_PIXCIclose();

            [DllImport("XCLIBW64.dll")]
            private static extern int pxd_doSnap(int c_unitmap, int c_buffer, int timeout);
            [DllImport("XCLIBW64.dll")]
            private static extern int pxd_goLive(int c_unitmap, int c_buffer);
            [DllImport("XCLIBW64.dll")]
            private static extern int pxd_goUnLive(int c_unitmap);
            [DllImport("XCLIBW64.dll")]
            private static extern int pxd_renderStretchDIBits(int c_unitmap, int c_buf, int c_ulx, int c_uly, int c_lrx, int c_lry, int c_options, IntPtr c_hDC, int c_nX, int c_nY, int c_nWidth, int c_nHeight, int c_winoptions);
            [DllImport("XCLIBW64.dll")]
            public static extern int pxd_SV9M001_setExposureAndGain(int c_unitmap, int c_rsvd, double c_exposure, double c_redgain, double c_grnrgain, double c_bluegain, double c_grnbgain);
            [DllImport("XCLIBW64.dll")]
            public static extern int pxd_SV9M001_setExposureAndDigitalGain(int c_unitmap, int c_rsvd, double c_exposure, double c_digitalgain, double c_rsvd2, double c_rsvd3, double c_rsvd4);

            //_pxd_mesgFault@4
            [DllImport("XCLIBW64.dll")]
            private static extern int pxd_mesgFault(int c_unitmap);
            //_pxd_mesgFaultText@12
            [DllImport("XCLIBW64.dll")]
            private static extern int pxd_mesgFaultText(int c_unitmap, StringBuilder buf, int bufsize);
            //_pxd_loadBmp@36
            [DllImport("XCLIBW64.dll", EntryPoint = "_pxd_loadBmp@36")]
            private static extern int EPIXLOADBMP(int c_unitmap, string c_pathname, int c_framebuf, int c_ulx, int c_uly, int c_lrx, int c_lry, int c_loadmode, int c_options);
            [DllImport("XCLIBW64.dll", EntryPoint = "_pxd_imageZdim@0")]
            private static extern int pxd_imageZdim();
            [DllImport("XCLIBW64.dll", EntryPoint = "_pxd_loadTiff@36")]
            private static extern int pxd_loadTiff(int c_unitmap, StringBuilder c_pathname, int c_buf, int c_ulx, int c_uly, int c_lrx, int c_lry, int c_loadmode, int c_options);
            [DllImport("XCLIBW64.dll", EntryPoint = "_pxd_xclibEscape@12")]
            private static extern IntPtr pxd_xclibEscape(int rsvd1, int rsvd2, int rsvd3);
            [DllImport("XCLIBW64.dll", EntryPoint = "_pxd_xclibEscaped@12")]
            private static extern int pxd_xclibEscaped(int rsvd1, int rsvd2, int rsvd3);
            [DllImport("XCLIBW64.dll", EntryPoint = "pxd_capturedFieldCount")]
            private static extern int pxd_capturedFieldCount(int c_unitmap);
            [DllImport("exBayerPhase.dll", EntryPoint = "_pxd_setBayerPhaseEx")]
            private static extern int pxd_setBayerPhaseEx(int iCamID, int iPhase, IntPtr pXclibs);

            [DllImport("gdi32.dll")]
            private static extern int SetStretchBltMode(IntPtr hDC, int mode);

            [DllImport("XCLIBW64.dll", EntryPoint = "_pxd_renderDIBCreate@32")]
            public static extern IntPtr pxd_renderDIBCreate(int unitmap, int buf, int ulx, int uly, int lrx, int lry, int mode, int options);

        #endregion
        
        //PictureBox testPictureBox = new PictureBox();

        Bitmap bmpTmp = new Bitmap(1, 1);
        public Bitmap bmpAllBMP = new Bitmap(1, 1);

        bool IsInitialOK = false;

        const int ReConnectionConut = 1;
        const int CountErrorCount = 3;

        Bitmap bmpError = new Bitmap(1, 1);
        Bitmap[] bmpLive = new Bitmap[(int)SideEnum.COUNT];
        Bitmap[] bmpLiveSized = new Bitmap[(int)SideEnum.COUNT];

        int[] LastCount = new int[(int)SideEnum.COUNT];
        int [] CountErrorRetry = new int[(int)SideEnum.COUNT];
        int[] SideErrorRetry = new int[(int)SideEnum.COUNT];

        Size szbmpLive = new Size();
        Size szbmpLiveSized = new Size();

        StreamWriter ErrorWriter;

        public const int MainOperationUIVirtaulRatio = -2;
        private StringBuilder m_strErr = new StringBuilder(1024);

        //private List<IAisysCameraControl> m_cams = new List<IAisysCameraControl>();
        private int[] icamsIndex;
        public int GetCount = 0;
        public bool IsComplete = false;

        //private int WritingSerial = 0;

        public string CCDSendStr = "";
        public string CCDLastSendStr = "";

        bool IsDebug
        {
            get
            {
                return true;
            }
        }

        //AllinoneCAM_MODE CAM_MODE
        //{
        //    get
        //    {
        //        return Universal.CAM_MODE;
        //    }
        //}

        string ConfigFile = @"D:\AUTOMATION\Eazy Key Height Check\Jumbo\WORK\CONFIGFX.fmt";
        JzToolsClass JzTools = new JzToolsClass();

        public string AllinoneBmpFile_Path = @"D:\HEIGHTFILEPATH";
        
        public CCDOfflineClass()
        {
            int i = 0;

            if (IsDebug)
            {
                i = 0;
                while (i < (int)SideEnum.COUNT)
                {
                    bmpTmp.Dispose();
                    bmpTmp = new Bitmap(1,1);
                    //bmpTmp = new Bitmap(@"D:\AUTOMATION\Eazy Key Height Check\Jumbo\WORK\00" + i.ToString() + ".BMP");
                    LastCount[i] = -1;

                    bmpLive[i] = new Bitmap(bmpTmp);
                    bmpLiveSized[i] = new Bitmap(bmpTmp, JzTools.Resize(bmpTmp.Size, VirtaulRatio));
                    i++;
                }
            }
            else
            {
                Initial();
            }
        }
        public void Initial()
        {
            int i = 0, j = 0;

            szbmpLive = new Size(INI.CCDWIDTH, INI.CCDHEIGHT);
            szbmpLiveSized = JzTools.Resize(szbmpLive, VirtaulRatio);


            while (i < (int)SideEnum.COUNT)
            {
                SideErrorRetry[i] = 0;
                bmpLive[i] = new Bitmap(INI.CCDWIDTH, INI.CCDHEIGHT);
                bmpLiveSized[i] = new Bitmap(szbmpLiveSized.Width, szbmpLiveSized.Height);

                i++;
            }



            bmpTmp.Dispose();
            bmpTmp = new Bitmap(@"D:\AUTOMATION\Eazy Key Height Check\Jumbo\WORK\Error.BMP");

            bmpError.Dispose();
            bmpError = new Bitmap(INI.CCDWIDTH, INI.CCDHEIGHT);

            ErrorWriter = new StreamWriter(@"D:\CCDLOG.TXT", true, Encoding.Default);
            ErrorWriter.AutoFlush = true;

            ErrorWriter.WriteLine(JzTimes.DateTimeString + "," + "Start Program");
 

            int Reconnect = 2;

            while (Reconnect > 1)
            {
                pxd_PIXCIclose();
                Application.DoEvents();
                pxd_PIXCIclose();
                Application.DoEvents();
                pxd_PIXCIclose();
                Application.DoEvents();
                pxd_PIXCIclose();
                Application.DoEvents();
                pxd_PIXCIclose();
                Application.DoEvents();

                int iret = -1, RetryLoop = 0;

                while (iret < 0)
                {
                    iret = pxd_PIXCIopen("", null, ConfigFile);
                    //Application.DoEvents();

                    if (iret >= 0)
                    {
                        IsInitialOK = true;

                        i = 0;
                        while (i < 5)
                        {
                            j = 0;
                            while (j < INI.SIDECOUNT)
                            {
                                iret = pxd_mesgFaultText((1 << j), m_strErr, 1024);

                                if (iret > 0)
                                {
                                    IsInitialOK = false;
                                }

                                j++;
                            }
                            i++;
                        }
                        break;
                    }

                    pxd_PIXCIclose();

                    RetryLoop++;

                    if (RetryLoop > 2)
                        break;
                }

                Reconnect--;
            }

            if (!IsInitialOK)
            {
                i = 0;
                while(i < INI.SIDECOUNT)
                {
                    bmpLive[i] = (Bitmap)bmpError.Clone();
                    bmpLiveSized[i] = new Bitmap(bmpError, JzTools.Resize(bmpTmp.Size, VirtaulRatio));
                    i++;
                }
                //MessageBox.Show("Camera Initial Error，Please Restart the Program。", "MAIN", MessageBoxButtons.OK);
            }
            else
            {
                i = 0;
                while (i < INI.SIDECOUNT)
                {
                    Live((SideEnum)i);
                    Render((SideEnum)i);
                    i++;
                }

                #region Test Region
                //SetExposure(SideEnum.SIDE0, 20);
                //SetExposure(SideEnum.SIDE1, 20);

                //TimerClass TestTimer = new TimerClass();

                //TestTimer.Cut();
                //i = 0;
                //while (i < 20)
                //{
                //    Render(SideEnum.SIDE0);
                //    Render(SideEnum.SIDE1);
                //    i++;
                //}

                //RefreshTime = (int)(((double)TestTimer.msDuriation / 40d) * 1.2d);

                //MessageBox.Show(TestTimer.msDuriation.ToString());
                #endregion
            }
        }

        int CameraOnLineIndex = 0;

 
        //void CCDClass_OnChannelCreated(int ImageWidth, int ImageHeight)
        //{
        //    Cameras[CameraOnLineIndex].SurfaceQueueCapacity = 2;
        //    Cameras[CameraOnLineIndex].EmptySurfaceQueue();
        //    Cameras[CameraOnLineIndex].EnableSurfaceQueue = true;
        //}

        //ReconnectionForm RECONNECTFRM;
        bool IsReconnecting = false;

        public void ReconnectInit()
        {
            //INI.SetPass(1);
            //Universal.RESULT.lblPass.Text = INI.PASS.ToString();


            int Reconnect = 2;
            int i = 0, j = 0;

           // RECONNECTFRM = new ReconnectionForm();

            IsReconnecting = true;
            IsInitialOK = false;

            //RECONNECTFRM.Show();
            //RECONNECTFRM.Refresh();

            while (!IsInitialOK)
            {
                Reconnect = 2;

                while (Reconnect > 1)
                {
                    pxd_PIXCIclose();

                    int iret = -1, RetryLoop = 0;

                    while (iret < 0)
                    {
                        iret = pxd_PIXCIopen("", null, ConfigFile);
                        //Application.DoEvents();

                        if (iret >= 0)
                        {
                            IsInitialOK = true;

                            i = 0;
                            while (i < 5)
                            {
                                j = 0;
                                while (j < INI.SIDECOUNT)
                                {
                                    iret = pxd_mesgFaultText((1 << j), m_strErr, 1024);

                                    if (iret > 0)
                                    {
                                        IsInitialOK = false;
                                    }

                                    j++;
                                }
                                i++;
                            }
                            break;
                        }

                        pxd_PIXCIclose();

                        RetryLoop++;

                        //RECONNECTFRM.Tick();
                        //RECONNECTFRM.Refresh();

                        if (RetryLoop > 2)
                            break;
                    }

                    //RECONNECTFRM.Tick();
                    //RECONNECTFRM.Refresh();

                    Reconnect--;
                }
            }

            //RECONNECTFRM.Close();
            //RECONNECTFRM.Dispose();

            IsReconnecting = false;

            i = 0;
            while (i < INI.SIDECOUNT)
            {
                Live((SideEnum)i);
                Render((SideEnum)i);
                i++;
            }


            CountErrorRetry = new int[(int)SideEnum.COUNT];
            SideErrorRetry = new int[(int)SideEnum.COUNT];

        }

        public void Live(SideEnum Side)
        {
            if (Side == SideEnum.COUNT)
            {
                int i = 0;
                while (i < (int)SideEnum.COUNT)
                {
                    //if (INI.ISAISYS)
                    //    Cameras[i].Live();
                    //else
                    //    pxd_goLive(1 << i, 1);

                    //if (!INI.ISAISYS)
                        pxd_goLive(1 << i, 1);

                    i++;
                }
            }
            else
            {
                //if (INI.ISAISYS)
                //    Cameras[(int)Side].Live();
                //else
                //    pxd_goLive(1 << (int)Side, 1);

                //if (!INI.ISAISYS)
                    pxd_goLive(1 << (int)Side, 1);
            }
        }
        public void Stop(SideEnum Side)
        {
            if (Side == SideEnum.COUNT)
            {
                int i = 0;
                while (i < (int)SideEnum.COUNT)
                {
                    //if (INI.ISAISYS)
                    //    Cameras[i].Freeze();
                    //else
                    //    pxd_goUnLive(1 << i);
                    //if(!INI.ISAISYS)
                        pxd_goUnLive(1 << i);

                    i++;
                }
            }
            else
            {
                //if (INI.ISAISYS)
                //    Cameras[(int)Side].Freeze();
                //else
                //    pxd_goUnLive(1 << (int)Side);
                //if(!INI.ISAISYS)
                    pxd_goUnLive(1 << (int)Side);

            }
        }
        public void Snap(SideEnum Side)
        {
            //switch (Universal.COMPANY_MODE)
            //{
            //    case MODE_DIFFERENT_COMPANY.MODE_Allinone:

            //switch (CAM_MODE)
            //{
            //    case AllinoneCAM_MODE.OFFLINE:
            AllinoneBmpFile_Path = Universal.PICPATH + Universal.RECIPEDB.ID.ToString("0000");
                            string m_file_path = AllinoneBmpFile_Path + "\\THH" + (int)Side + ".png";

                            if (!System.IO.File.Exists(m_file_path))
                            {
                                bmpLive[(int)Side].Dispose();
                                bmpLive[(int)Side] = new Bitmap(INI.CCDWIDTH, INI.CCDHEIGHT);
                            }
                            else
                            {
                                bmpTMP.Dispose();
                                bmpTMP = new Bitmap(m_file_path);

                                bmpLive[(int)Side].Dispose();
                                bmpLive[(int)Side] = new Bitmap(bmpTMP);

                                bmpTMP.Dispose();
                            }
                            //break;
                    //    case AllinoneCAM_MODE.ONLINE:

                    //        bmpTMP.Dispose();
                    //        bmpTMP = new Bitmap(Universal.CCDEX.GetCombineBMPEX());
                    //        //bmpTMP = new Bitmap(Universal.CCDEX.GetCombineBMP());

                    //        bmpLive[(int)Side].Dispose();
                    //        bmpLive[(int)Side] = new Bitmap(bmpTMP);

                    //        bmpTMP.Dispose();

                    //        break;
                    //}

                //    break;
                //default:
                //    if (!IsDebug)
                //    {
                //        //if (!INI.ISAISYS)
                //        pxd_doSnap((1 << (int)Side), 1, 0);

                //        Render(Side, true);
                //    }
                //    break;
            //}
        }
        Bitmap bmpTMP = new Bitmap(1, 1);
        
        /*
        public void SnapEX(SideEnum Side)
        {
            if (!IsDebug)
            {
                pxd_doSnap((1 << (int)Side), 1, 0);

                IntPtr hDIB = pxd_renderDIBCreate((1 << (int)Side), 1, 0, 0, -1, -1, 0, 0);
                CxDibImage dibImage = new CxDibImage(hDIB);

                //<1> Using BitmapData is more faster !!!
                //BitmapData bmpd = dibImage.LockBits();
                //// apply your algorithm on bmpd.
                //dibImage.UnlockBits(bmpd);

                //<2> Using Bitmap is slower
                Rectangle rect = new Rectangle(0, 0, dibImage.Width, dibImage.Height);
                //Bitmap bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);
                Graphics gxTo = Graphics.FromImage(bmpLive[(int)Side]);
                dibImage.Render(gxTo, ref rect, ref rect);
                gxTo.Dispose();
                // apply your algorithm on bmp
                //bmp.Dispose();

                //<3> Dispose dibImage: don't forget "dispose" when you are no longer using dibImage !!!
                dibImage.Dispose();

            }
        }
        */

        public void SaveBMP(string filename, SideEnum Side)
        {
            Snap(Side);
            Bitmap bmpTmp = new Bitmap(bmpLive[(int)Side]);
            bmpTmp.Save(filename, ImageFormat.Bmp);
            bmpTmp.Dispose();
        }
        public void SaveAllBMP(string PathName)
        {
            Bitmap bmp = new Bitmap(bmpAllBMP);
            bmp.Save(PathName + "KBALL.BMP", ImageFormat.Bmp);

            int i = 0;
            while (i < (int)SideEnum.COUNT)
            {
                bmp.Dispose();
                bmp = new Bitmap(bmpLive[i]);
                bmp.Save(PathName + i.ToString("000") + ".BMP", ImageFormat.Bmp);

                //bmp.Dispose();
                //bmp = new Bitmap(bmpLiveSized[i]);

                //bmp.Save(PathName + "S" + i.ToString("000") + ".BMP", ImageFormat.Bmp);

                i++;
            }
        }

        public void Render(SideEnum Side, bool IsSudden)
        {
            //if (!INI.ISAISYS)
            {
                bmpLive[(int)Side].Dispose();
                bmpLive[(int)Side] = new Bitmap(INI.CCDWIDTH, INI.CCDHEIGHT);

                Graphics g = Graphics.FromImage(bmpLive[(int)Side]);
                IntPtr hDC = g.GetHdc();

                SetStretchBltMode(hDC, STRETCH_DELETESCANS);
                pxd_renderStretchDIBits((1 << (int)Side), 1, 0, 0, -1, -1, 0, hDC, 0, 0, szbmpLive.Width, szbmpLive.Height, 0);
                g.ReleaseHdc(hDC);
                g.Dispose();
            }
            //else
            {
                //int iNumOfPendingSurfaces = Cameras[(int)Side].NumOfPendingSurfaceInQueue;
                //int hSurface = Cameras[(int)Side].PopSurfaceFromQueue(-1);

                //Cameras[(int)Side].DrawSurface(hSurface, (int)hDC, 1.0f, 1.0f, 0, 0);
                //IAisysCameraControl cam = m_cams[(int)Side];
                //Bitmap bmp = null;
                //try
                //{

                //    //string strFileName = "H:\\" + ((int)Side).ToString("00") + "_" + icamsIndex[(int)Side].ToString("00") + ".bmp";
                //    string strFileName = "H:\\" + ((int)Side).ToString("00") + ".bmp";

                //    //if (File.Exists(strFileName))
                //    //    File.Delete(strFileName);


                //    //bool IsShotOK = cam.Snapshot(strFileName);

                //    //while (!IsShotOK)
                //    //{
                //    //    IsShotOK = cam.Snapshot(strFileName);
                //    //}

                //    //icamsIndex[(int)Side]++;

                //    //if (icamsIndex[(int)Side] > 1)
                //    //    icamsIndex[(int)Side] = 0;

                //    if (bmpLive[(int)Side] != null)
                //        bmpLive[(int)Side].Dispose();

                //    bmp = new Bitmap(strFileName);

                //    bmpLive[(int)Side] = new Bitmap(bmp);

                //    bmp.Dispose();

                //    //File.Delete(strFileName);
                //}
                //catch
                //{
                //    if (bmp != null)
                //        bmp.Dispose();
                //}

            }



            //Application.DoEvents();

            //bmpLiveSized[(int)Side].Dispose();
            //bmpLiveSized[(int)Side] = new Bitmap(bmpLive[(int)Side], JzTools.Resize(bmpLive[(int)Side].Size, VirtaulRatio));

            //bmpLive[(int)Side].Dispose();
            //bmpLive[(int)Side] = new Bitmap(1, 1);

        }
        public void Render(SideEnum Side)
        {
            if (SideErrorRetry[(int)Side] <= ReConnectionConut)
            {
                if (!IsDebug)
                {
                    //if (!INI.ISAISYS)
                    {
                        Graphics g = Graphics.FromImage(bmpLiveSized[(int)Side]);

                        //Close();

                        IntPtr hDC = g.GetHdc();
                        SetStretchBltMode(hDC, STRETCH_DELETESCANS);

                        pxd_renderStretchDIBits((1 << (int)Side), 1, 0, 0, -1, -1, 0, hDC, 0, 0, szbmpLiveSized.Width, szbmpLiveSized.Height, 0);

                        g.ReleaseHdc(hDC);
                        g.Dispose();
                    }
                    //else
                    {
                        //IAisysCameraControl cam = m_cams[(int)Side];
                        //Bitmap bmp = null;

                        ////try
                        //{
                        //    string strFileName = "H:\\" + ((int)Side).ToString("00") + ".bmp";

                        //    //if (File.Exists(strFileName))
                        //    //    File.Delete(strFileName);

                        //    //bool IsShotOK = cam.Snapshot(strFileName);

                        //    //while (!IsShotOK)
                        //    //{
                        //    //    IsShotOK = cam.Snapshot(strFileName);
                        //    //}


                        //    //icamsIndex[(int)Side]++;

                        //    //if (icamsIndex[(int)Side] > 1)
                        //    //    icamsIndex[(int)Side] = 0;

                        //    //bmpLive[(int)Side].Dispose();
                        //    bmp = new Bitmap(strFileName);

                        //    //bmpLive[(int)Side] = (Bitmap)bmp.Clone();

                        //    bmpLiveSized[(int)Side].Dispose();
                        //    bmpLiveSized[(int)Side] = new Bitmap(bmp, JzTools.Resize(bmp.Size, VirtaulRatio));


                        //    bmp.Dispose();

                        //    //File.Delete(strFileName);
                        //}
                        ////catch
                        ////{
                        ////    if (bmp != null)
                        ////        bmp.Dispose();
                        ////}


                    }
                    //bmpLiveSized[(int)Side].Save(@"D:\LOA\SIZED" + Side.ToString() +  ".BMP", ImageFormat.Bmp);

                }
                else
                {
                    bmpLiveSized[(int)Side].Dispose();
                    bmpLiveSized[(int)Side] = new Bitmap(bmpLive[(int)Side], JzTools.Resize(bmpLive[(int)Side].Size, VirtaulRatio));
                }
            }
            else
            {
                SetErrorScreen(Side);
            }
        }

        public void SetErrorScreen(SideEnum Side)
        {
            bmpLive[(int)Side].Dispose();
            bmpLive[(int)Side] = (Bitmap)bmpError.Clone();

            bmpLiveSized[(int)Side].Dispose();
            bmpLiveSized[(int)Side] = (Bitmap)bmpError.Clone(new Rectangle(new Point(0, 0), szbmpLive), PixelFormat.Format32bppArgb);
        }

        double[] dcamEXP = new double[(int)SideEnum.COUNT];

        public void SetExposure(SideEnum Side, int Exposure)
        {
            pxd_SV9M001_setExposureAndDigitalGain(1 << (int)Side, 0, (double)Exposure * 15, 0, 0, 0, 0);

        }

        public void EndCapture()
        {
            ErrorWriter.WriteLine(JzTimes.DateTimeString + "," + "AISYS Capture End");
        }

        public void CheckConnection(SideEnum Side)
        {
            if (IsDebug)
                return;

            if (IsReconnecting)
                return;

            //if (INI.ISAISYS)
            //    return;

            int NowCount = pxd_capturedFieldCount(1 << (int)Side);    //判?是否掉?

            if (CountErrorRetry[(int)Side] < CountErrorCount)
            {
                if (LastCount[(int)Side] != NowCount)
                {
                    LastCount[(int)Side] = NowCount;
                    CountErrorRetry[(int)Side] = 0;
                }
                else
                {
                    CountErrorRetry[(int)Side]++;

                    Stop(Side);
                    Live(Side);

                    ErrorWriter.WriteLine(JzTimes.DateTimeString + "," + "Camera " + ((int)Side + 1).ToString() + " Lost Connection " + CountErrorRetry[(int)Side].ToString() + " times.");

                    if (CountErrorRetry[(int)Side] == CountErrorCount)
                    {
                        //SetErrorScreen(Side);
                        //MessageBox.Show("Camera " + ((int)Side + 1).ToString() + " Connection Error,Please Check the Cable。", "MAIN", MessageBoxButtons.OK);

                        ErrorWriter.WriteLine(JzTimes.DateTimeString + "," + "Camera " + ((int)Side + 1).ToString() + " Do Reconnection.");


                        ReconnectInit();
                    }
                }
            }


            if (SideErrorRetry[(int)Side] < ReConnectionConut)
            {
                int i = pxd_mesgFaultText((1 << (int)Side), m_strErr, 1024);
                if (i != 0)
                {
                    SideErrorRetry[(int)Side]++;
                    if (SideErrorRetry[(int)Side] >= ReConnectionConut)
                    {
                        SetErrorScreen(Side);
                        MessageBox.Show("攝像頭 " + ((int)Side + 1).ToString() + " 連線錯誤，請檢查連線是否錯誤。原因為：" + Environment.NewLine + m_strErr.ToString(), "MAIN", MessageBoxButtons.OK);
                    }
                }
            }
            else
                SetErrorScreen(Side);
        }
        public Bitmap GetBMP(SideEnum Side)
        {
            return bmpLive[(int)Side];
        }
        public void ClearBMP(SideEnum Side)
        {
            bmpLive[(int)Side].Dispose();
            bmpLive[(int)Side] = new Bitmap(1, 1);
        }

        public int VirtaulRatio = MainOperationUIVirtaulRatio;

        //public void Tick()
        //{
        //    m_cams[0].Ping();
        //    m_cams[1].Ping();
        //    m_cams[2].Ping();
        //    m_cams[3].Ping();
        //    m_cams[4].Ping();
        //    m_cams[5].Ping();
        //    m_cams[6].Ping();

        //}

        int GetSequence = 0;
        public void GetAllBMP()
        {
            GetAllBMP(false);
        }
        public void GetAllBMPEX()
        {
            //if (INI.ISAISYS)
            //    return;

            int i = 0;

            if (GetSequence < INI.SIDECOUNT)
            {
                //Render((SideEnum)GetSequence);
                CheckConnection((SideEnum)GetSequence);
            }

            //Stop(SideEnum.COUNT);

            GetSequence++;

            if (GetSequence == INI.SIDECOUNT)
                GetSequence = 0;

            i = 0;
            string Str = "";
            while (i < INI.SIDECOUNT)
            {

                Str += (LastCount[i] % 100).ToString("00") + ",";
                i++;
            }
            Str = Str.Remove(Str.Length - 1, 1);

            //if (!INI.ISAISYS)
                JzTools.DrawText(bmpAllBMP, Str, true);

        }

        //public void CaptureType(bool IsAll)
        //{


        //}
         public void GetAllBMP(bool IsSudden)
        {
            int i = 0;

            if (!IsSudden)
            {
                if (GetSequence < INI.SIDECOUNT)
                {
                    //Render((SideEnum)GetSequence);
                    CheckConnection((SideEnum)GetSequence);
                }
            }
            else
            {
                //i = 0;
                //while (i < INI.SIDECOUNT)
                //{
                //    Snap((SideEnum)i);
                //    i++;
                //}
                //return;
                if (GetSequence < INI.SIDECOUNT)
                {
                    Render((SideEnum)GetSequence);
                    CheckConnection((SideEnum)GetSequence);
                }
            }


            bmpAllBMP.Dispose();

            if (INI.SIDECOUNT < 11)
                bmpAllBMP = new Bitmap(JzTools.ShiftValue(INI.SIDE3LOCATIONLIVE.X + INI.CCDWIDTH, VirtaulRatio), JzTools.ShiftValue(INI.CCDHEIGHT, VirtaulRatio));
            else
                bmpAllBMP = new Bitmap(JzTools.ShiftValue(INI.SIDE6LOCATIONLIVE.X + INI.CCDWIDTH, VirtaulRatio), JzTools.ShiftValue(INI.CCDHEIGHT, VirtaulRatio));

            GetSequence++;

            if (GetSequence == INI.SIDECOUNT)
                GetSequence = 0;

            i = (int)INI.SIDECOUNT - 1;

            Graphics g = Graphics.FromImage(bmpAllBMP);
            g.Clear(Color.Black);

            while (i >= 0)
            {
                switch ((SideEnum)i)
                {
                    case SideEnum.SIDE0:
                        g.DrawImage(bmpLiveSized[0], new Rectangle(0, 0, bmpLiveSized[0].Width, bmpLiveSized[0].Height), new Rectangle(0, 0, bmpLiveSized[0].Width, bmpLiveSized[0].Height), GraphicsUnit.Pixel);
                        break;
                    //case SideEnum.SIDE1:
                    //    g.DrawImage(bmpLiveSized[1], new Rectangle(JzTools.ShiftPoint(INI.SIDE1LOCATIONLIVE, VirtaulRatio), bmpLiveSized[1].Size), new Rectangle(0, 0, bmpLiveSized[1].Width, bmpLiveSized[1].Height), GraphicsUnit.Pixel);
                    //    break;
                    //case SideEnum.SIDE2:
                    //    g.DrawImage(bmpLiveSized[2], new Rectangle(JzTools.ShiftPoint(INI.SIDE2LOCATIONLIVE, VirtaulRatio), bmpLiveSized[2].Size), new Rectangle(0, 0, bmpLiveSized[2].Width, bmpLiveSized[2].Height), GraphicsUnit.Pixel);
                    //    break;
                    //case SideEnum.SIDE3:
                    //    g.DrawImage(bmpLiveSized[3], new Rectangle(JzTools.ShiftPoint(INI.SIDE3LOCATIONLIVE, VirtaulRatio), bmpLiveSized[3].Size), new Rectangle(0, 0, bmpLiveSized[3].Width, bmpLiveSized[3].Height), GraphicsUnit.Pixel);
                    //    break;
                    //case SideEnum.SIDE4:
                    //    g.DrawImage(bmpLiveSized[4], new Rectangle(JzTools.ShiftPoint(INI.SIDE4LOCATIONLIVE, VirtaulRatio), bmpLiveSized[4].Size), new Rectangle(0, 0, bmpLiveSized[4].Width, bmpLiveSized[4].Height), GraphicsUnit.Pixel);
                    //    break;
                    //case SideEnum.SIDE5:
                    //    g.DrawImage(bmpLiveSized[5], new Rectangle(JzTools.ShiftPoint(INI.SIDE5LOCATIONLIVE, VirtaulRatio), bmpLiveSized[5].Size), new Rectangle(0, 0, bmpLiveSized[5].Width, bmpLiveSized[5].Height), GraphicsUnit.Pixel);
                    //    break;
                    //case SideEnum.SIDE6:
                    //    g.DrawImage(bmpLiveSized[6], new Rectangle(JzTools.ShiftPoint(INI.SIDE6LOCATIONLIVE, VirtaulRatio), bmpLiveSized[6].Size), new Rectangle(0, 0, bmpLiveSized[6].Width, bmpLiveSized[6].Height), GraphicsUnit.Pixel);
                    //    break;
                    //case SideEnum.SIDE7:
                    //    g.DrawImage(bmpLiveSized[7], new Rectangle(JzTools.ShiftPoint(INI.SIDE7LOCATIONLIVE, VirtaulRatio), bmpLiveSized[7].Size), new Rectangle(0, 0, bmpLiveSized[7].Width, bmpLiveSized[7].Height), GraphicsUnit.Pixel);
                    //    break;
                    //case SideEnum.SIDE8:
                    //    g.DrawImage(bmpLiveSized[8], new Rectangle(JzTools.ShiftPoint(INI.SIDE8LOCATIONLIVE, VirtaulRatio), bmpLiveSized[8].Size), new Rectangle(0, 0, bmpLiveSized[8].Width, bmpLiveSized[8].Height), GraphicsUnit.Pixel);
                    //    break;
                    //case SideEnum.SIDE9:
                    //    g.DrawImage(bmpLiveSized[9], new Rectangle(JzTools.ShiftPoint(INI.SIDE9LOCATIONLIVE, VirtaulRatio), bmpLiveSized[9].Size), new Rectangle(0, 0, bmpLiveSized[9].Width, bmpLiveSized[9].Height), GraphicsUnit.Pixel);
                    //    break;
                    //case SideEnum.SIDE10:
                    //    g.DrawImage(bmpLiveSized[10], new Rectangle(JzTools.ShiftPoint(INI.SIDE10LOCATIONLIVE, VirtaulRatio), bmpLiveSized[10].Size), new Rectangle(0, 0, bmpLiveSized[10].Width, bmpLiveSized[10].Height), GraphicsUnit.Pixel);
                    //    break;
                }
                i--;
            }
            g.Dispose();

            //bmpAllBMP.Save(@"D:\LOA\BMPALL.BMP", ImageFormat.Bmp);

            i = 0;
            string Str = "";
            while (i < INI.SIDECOUNT)
            {

                Str += (LastCount[i] % 100).ToString("00") + ",";
                i++;
            }
            Str =Str.Remove(Str.Length-1,1);

            //if (!INI.ISAISYS)
                JzTools.DrawText(bmpAllBMP, Str, true);

        }

      }
}