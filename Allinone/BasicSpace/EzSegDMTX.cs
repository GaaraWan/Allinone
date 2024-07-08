﻿using EzSegClientLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;

namespace Allinone.BasicSpace
{
    public class JetMutilDecode
    {
        public JetMutilDecode()
        {

        }
        private Bitmap m_InputImage = null;
        private string m_BarcodeStr = string.Empty;
        private long m_ElapsedTime = 0;
        public Bitmap InputImage
        {
            get { return m_InputImage; }
            set
            {
                m_InputImage = value;
                //if (value == null)
                //    m_InputImage = null;
                //else
                //    m_InputImage = new Bitmap(value);
            }
        }
        public string BarcodeStr
        {
            get { return m_BarcodeStr; }
            set { m_BarcodeStr = value; }
        }
        public long ElapsedTime
        {
            get { return m_ElapsedTime; }
        }

        public int Run()
        {
            int iret = -1;
            m_ElapsedTime = 0;
            m_BarcodeStr = string.Empty;

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Restart();

            List<EzSegDMTX> lists = new List<EzSegDMTX>();

            //目前10个方法 
            int fun_count = 10;
            //Bitmap[] bitmaps = new Bitmap[fun_count];
            for (int i = 0; i < fun_count; i++)
            {
                //bitmaps[i] = new Bitmap(m_InputImage);

                EzSegDMTX ezSeg = new EzSegDMTX();
                ezSeg.InputImage = new Bitmap(m_InputImage);
                ezSeg.CalIndex = i;
                //Task.Run(() =>
                //{
                //    ezSeg.RunSingle(i);
                //});

                lists.Add(ezSeg);
            }

            Parallel.ForEach(lists, list =>
            {
                list.RunSingle();
            });

            int j = 0;
            while (j < lists.Count)
            {

                if (!string.IsNullOrEmpty(lists[j].BarcodeStr))
                {
                    m_BarcodeStr = lists[j].BarcodeStr;
                    iret = 0;
                    break;
                }

                j++;
            }

            stopwatch.Stop();
            m_ElapsedTime = stopwatch.ElapsedMilliseconds;


            return iret;
        }
    }

    public class EzSegDMTX
    {
        public EzSegDMTX()
        {

        }
        public IEzSeg model
        {
            get { return Universal.model; }
        }

        bool m_UseAIFromPy = true;
        //private EzSegClientLib.IEzSeg m_iEzSeg = null;
        private Bitmap m_InputImage = null;
        private string m_BarcodeStr = string.Empty;
        private long m_ElapsedTime = 0;
        //private Bitmap m_PreInputImage = new Bitmap(1, 1);
        private string m_PathFile = "Z:\\data_matrix";
        private int m_PathCount = 0;
        private int m_calindex = -1;

        //List<Bitmap> m_InputImages = new List<Bitmap>();
        //public Bitmap[] PreImages
        //{
        //    get { return m_InputImages.ToArray(); }
        //}
        //public Bitmap PreInputImageOut
        //{
        //    get { return m_PreInputImage; }
        //    set
        //    {
        //        m_PreInputImage = value;
        //        //if (value == null)
        //        //    m_InputImage = null;
        //        //else
        //        //    m_InputImage = new Bitmap(value);
        //    }
        //}
        public Bitmap InputImage
        {
            get { return m_InputImage; }
            set
            {
                m_InputImage = value;
                //if (value == null)
                //    m_InputImage = null;
                //else
                //    m_InputImage = new Bitmap(value);
            }
        }
        public string BarcodeStr
        {
            get { return m_BarcodeStr; }
            set { m_BarcodeStr = value; }
        }
        public long ElapsedTime
        {
            get { return m_ElapsedTime; }
        }
        public int CalIndex
        {
            get { return m_calindex; }
            set { m_calindex = value; }
        }
        public int Run()
        {
            //m_InputImages.Clear();

            m_PathFile = "Z:\\data_matrix\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            //if (!System.IO.Directory.Exists(m_PathFile))
            //    System.IO.Directory.CreateDirectory(m_PathFile);
            m_PathCount = 0;

            //ConcurrentQueue<string> qu = new System.Collections.Concurrent.ConcurrentQueue();
            //ConcurrentStack

            //System.Threading.Thread.Sleep(1000);
            //Task task1 = Task.Run(() =>
            //{

            //});
            //Task task2 = Task.Run(() =>
            //{

            //});
            //Task.WaitAny(task1, task2);
            //Task.Factory.StartNew(() =>
            //{

            //});

            //System.Threading.Thread.Sleep(1000);

            int iret = -1;
            m_ElapsedTime = 0;

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Restart();

            iret = _get2d_victor();
            if (iret != 0)
                iret = _get2d_g1();
            if (iret != 0)
                iret = _get2d_g2();
            if (iret != 0)
                iret = _get2d_v2();
            if (iret != 0)
                iret = _get2d_v3();
            if (iret != 0)
                iret = _get2d_v5_1();
            if (iret != 0)
                iret = _get2d_v5_2();
            if (iret != 0)
                iret = _get2d_v5_5();
            if (iret != 0)
                iret = _get2d_v5_6();
            if (iret != 0)
                iret = _get2d_v5_7();

            stopwatch.Stop();
            m_ElapsedTime = stopwatch.ElapsedMilliseconds;

            return iret;
        }
        public int RunSingle()
        {
            int iret = -1;
            m_ElapsedTime = 0;

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Restart();

            switch(m_calindex)
            {
                case 0:
                    iret = _get2d_victor();
                    break;
                case 1:
                    iret = _get2d_g1();
                    break;
                case 2:
                    iret = _get2d_g2();
                    break;
                case 3:
                    iret = _get2d_v2();
                    break;
                case 4:
                    iret = _get2d_v3();
                    break;
                case 5:
                    iret = _get2d_v5_1();
                    break;
                case 6:
                    iret = _get2d_v5_2();
                    break;
                case 7:
                    iret = _get2d_v5_5();
                    break;
                case 8:
                    iret = _get2d_v5_6();
                    break;
                case 9:
                    iret = _get2d_v5_7();
                    break;
            }
            stopwatch.Stop();
            m_ElapsedTime = stopwatch.ElapsedMilliseconds;

            return iret;
        }

        #region PRIVATE FUNTION

        private int _get2d_victor()
        {
            m_BarcodeStr = String.Empty;
            if (m_InputImage == null)
            {
                return -2;//图像错误
            }

            Bitmap bmpinput = new Bitmap(m_InputImage);
            Bitmap bmpinputsize168 = new Bitmap(m_InputImage);

            //图像尺寸 最小 168*168
            if (bmpinput.Width < 168 || bmpinput.Height < 168)
            {
                bmpinputsize168.Dispose();
                bmpinputsize168 = new Bitmap(resizeImage(bmpinput, new Size(168, 168)));
            }

            //Gray
            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            Bitmap bmpgray = grayscale.Apply(bmpinputsize168);

            //Otsu
            AForge.Imaging.Filters.OtsuThreshold otsuThreshold = new AForge.Imaging.Filters.OtsuThreshold();
            Bitmap bmpotsu = otsuThreshold.Apply(bmpgray);

            //Erosion
            AForge.Imaging.Filters.Erosion erosion = new AForge.Imaging.Filters.Erosion();
            Bitmap bmperosion = erosion.Apply(bmpotsu);

            //AForge.Imaging.Filters.Dilatation dilatation = new AForge.Imaging.Filters.Dilatation();
            //Bitmap bmpdilatation = dilatation.Apply(bmperosion);
            //Bitmap bmpdilatation2 = dilatation.Apply(bmpdilatation);

            ZXing.BarcodeReader reader = new ZXing.BarcodeReader();
            reader.AutoRotate = true;
            reader.Options = new ZXing.Common.DecodingOptions();
            reader.Options.TryHarder = true;
            reader.Options.TryInverted = true;
            reader.Options.PossibleFormats = new[] { ZXing.BarcodeFormat.DATA_MATRIX };

            Bitmap bmpLast = new Bitmap(bmperosion);

            int ireadIndex = 0;
            int ireadCount = 3;
            while (ireadIndex < ireadCount)
            {
                ZXing.Result result = reader.Decode(bmpLast);
                if (result != null)
                {
                    m_BarcodeStr = result.Text;
                    //stopwatch.Stop();
                    //m_ElapsedTime = stopwatch.ElapsedMilliseconds;
                    return 0;//成功读码
                }

                string dmtxStr = _getDMTX(bmpLast);
                if (!string.IsNullOrEmpty(dmtxStr))
                {
                    m_BarcodeStr = dmtxStr;
                    return 0;//成功读码
                }

                bmpLast.Dispose();
                //Size
                AForge.Imaging.Filters.ResizeBilinear resizeBilinear = new AForge.Imaging.Filters.ResizeBilinear(bmperosion.Width * 2, bmperosion.Height * 2);
                bmpLast = resizeBilinear.Apply(bmperosion);

                ireadIndex++;
            }


            bmpinput.Dispose();
            bmpinputsize168.Dispose();
            bmpgray.Dispose();
            bmpotsu.Dispose();
            bmperosion.Dispose();
            bmpLast.Dispose();
            //bmpdilatation.Dispose();
            //bmpdilatation2.Dispose();

            //stopwatch.Stop();
            //m_ElapsedTime = stopwatch.ElapsedMilliseconds;
            return -1;//读码失败
        }
        private int _get2d_g1()
        {
            m_BarcodeStr = String.Empty;
            if (m_InputImage == null)
            {
                return -2;//图像错误
            }

            Bitmap bmpinput = new Bitmap(m_InputImage);
            Bitmap bmpinputsize168 = new Bitmap(m_InputImage);

            //图像尺寸 最小 168*168
            if (bmpinput.Width < 168 || bmpinput.Height < 168)
            {
                bmpinputsize168.Dispose();
                bmpinputsize168 = new Bitmap(resizeImage(bmpinput, new Size(168, 168)));
            }

            //Gray
            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            Bitmap bmpgray = grayscale.Apply(bmpinputsize168);

            //Otsu
            AForge.Imaging.Filters.OtsuThreshold otsuThreshold = new AForge.Imaging.Filters.OtsuThreshold();
            Bitmap bmpotsu = otsuThreshold.Apply(bmpgray);

            //Erosion
            AForge.Imaging.Filters.Erosion erosion = new AForge.Imaging.Filters.Erosion();
            Bitmap bmperosion = erosion.Apply(bmpotsu);

            AForge.Imaging.Filters.Dilatation dilatation = new AForge.Imaging.Filters.Dilatation();
            Bitmap bmpdilatation = dilatation.Apply(bmperosion);
            Bitmap bmpdilatation2 = dilatation.Apply(bmpdilatation);

            ZXing.BarcodeReader reader = new ZXing.BarcodeReader();
            reader.AutoRotate = true;
            reader.Options = new ZXing.Common.DecodingOptions();
            reader.Options.TryHarder = true;
            reader.Options.TryInverted = true;
            reader.Options.PossibleFormats = new[] { ZXing.BarcodeFormat.DATA_MATRIX };

            Bitmap bmpLast = new Bitmap(bmpdilatation2);

            int ireadIndex = 0;
            int ireadCount = 3;
            while (ireadIndex < ireadCount)
            {
                ZXing.Result result = reader.Decode(bmpLast);
                if (result != null)
                {
                    m_BarcodeStr = result.Text;
                    //stopwatch.Stop();
                    //m_ElapsedTime = stopwatch.ElapsedMilliseconds;
                    return 0;//成功读码
                }

                string dmtxStr = _getDMTX(bmpLast);
                if (!string.IsNullOrEmpty(dmtxStr))
                {
                    m_BarcodeStr = dmtxStr;
                    return 0;//成功读码
                }

                bmpLast.Dispose();
                //Size
                AForge.Imaging.Filters.ResizeBilinear resizeBilinear = new AForge.Imaging.Filters.ResizeBilinear(bmperosion.Width * 2, bmperosion.Height * 2);
                bmpLast = resizeBilinear.Apply(bmperosion);

                ireadIndex++;
            }


            bmpinput.Dispose();
            bmpinputsize168.Dispose();
            bmpgray.Dispose();
            bmpotsu.Dispose();
            bmperosion.Dispose();
            bmpLast.Dispose();
            bmpdilatation.Dispose();
            bmpdilatation2.Dispose();

            //stopwatch.Stop();
            //m_ElapsedTime = stopwatch.ElapsedMilliseconds;
            return -1;//读码失败
        }
        private int _get2d_g2()
        {
            m_BarcodeStr = String.Empty;
            if (m_InputImage == null)
            {
                return -2;//图像错误
            }

            Bitmap bmpinput = new Bitmap(m_InputImage);
            Bitmap bmpinputsize168 = new Bitmap(m_InputImage);

            //图像尺寸 最小 168*168
            if (bmpinput.Width < 168 || bmpinput.Height < 168)
            {
                bmpinputsize168.Dispose();
                bmpinputsize168 = new Bitmap(resizeImage(bmpinput, new Size(168, 168)));
            }

            //Gray
            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            Bitmap bmpgray = grayscale.Apply(bmpinputsize168);

            ////Otsu
            //AForge.Imaging.Filters.OtsuThreshold otsuThreshold = new AForge.Imaging.Filters.OtsuThreshold();
            //Bitmap bmpotsu = otsuThreshold.Apply(bmpgray);

            ////Erosion
            //AForge.Imaging.Filters.Erosion erosion = new AForge.Imaging.Filters.Erosion();
            //Bitmap bmperosion = erosion.Apply(bmpotsu);

            //AForge.Imaging.Filters.Dilatation dilatation = new AForge.Imaging.Filters.Dilatation();
            //Bitmap bmpdilatation = dilatation.Apply(bmperosion);
            //Bitmap bmpdilatation2 = dilatation.Apply(bmpdilatation);

            ZXing.BarcodeReader reader = new ZXing.BarcodeReader();
            reader.AutoRotate = true;
            reader.Options = new ZXing.Common.DecodingOptions();
            reader.Options.TryHarder = true;
            reader.Options.TryInverted = true;
            reader.Options.PossibleFormats = new[] { ZXing.BarcodeFormat.DATA_MATRIX };

            Bitmap bmpLast = new Bitmap(bmpgray);

            int ireadIndex = 0;
            int ireadCount = 1;
            while (ireadIndex < ireadCount)
            {
                ZXing.Result result = reader.Decode(bmpLast);
                if (result != null)
                {
                    m_BarcodeStr = result.Text;
                    //stopwatch.Stop();
                    //m_ElapsedTime = stopwatch.ElapsedMilliseconds;
                    return 0;//成功读码
                }

                string dmtxStr = _getDMTX(bmpLast);
                if (!string.IsNullOrEmpty(dmtxStr))
                {
                    m_BarcodeStr = dmtxStr;
                    return 0;//成功读码
                }

                bmpLast.Dispose();
                //Size
                //AForge.Imaging.Filters.ResizeBilinear resizeBilinear = new AForge.Imaging.Filters.ResizeBilinear(bmperosion.Width * 2, bmperosion.Height * 2);
                //bmpLast = resizeBilinear.Apply(bmperosion);

                ireadIndex++;
            }


            bmpinput.Dispose();
            bmpinputsize168.Dispose();
            bmpgray.Dispose();
            //bmpotsu.Dispose();
            //bmperosion.Dispose();
            bmpLast.Dispose();
            //bmpdilatation.Dispose();
            //bmpdilatation2.Dispose();

            //stopwatch.Stop();
            //m_ElapsedTime = stopwatch.ElapsedMilliseconds;
            return -1;//读码失败
        }
        private int _get2d_v2()
        {
            m_BarcodeStr = String.Empty;
            if (m_InputImage == null)
            {
                return -2;//图像错误
            }

            Bitmap bmpinput = new Bitmap(m_InputImage);
            Bitmap bmpinputsize168 = new Bitmap(m_InputImage);

            ////图像尺寸 最小 168*168
            //if (bmpinput.Width < 168 || bmpinput.Height < 168)
            //{
            //    bmpinputsize168.Dispose();
            //    bmpinputsize168 = new Bitmap(resizeImage(bmpinput, new Size(168, 168)));
            //}

            //Gray
            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            Bitmap bmpgray = grayscale.Apply(bmpinputsize168);

            //Gray
            AForge.Imaging.Filters.ContrastStretch contrastStretch = new AForge.Imaging.Filters.ContrastStretch();
            Bitmap bmpcontrastStretch = contrastStretch.Apply(bmpgray);

            //Otsu
            AForge.Imaging.Filters.BradleyLocalThresholding bradleyLocalThresholding = new AForge.Imaging.Filters.BradleyLocalThresholding();
            Bitmap bmpbradleyLocalThresholding = bradleyLocalThresholding.Apply(bmpcontrastStretch);

            //Erosion
            AForge.Imaging.Filters.Erosion erosion = new AForge.Imaging.Filters.Erosion();
            Bitmap bmperosion = erosion.Apply(bmpbradleyLocalThresholding);

            //AForge.Imaging.Filters.Dilatation dilatation = new AForge.Imaging.Filters.Dilatation();
            //Bitmap bmpdilatation = dilatation.Apply(bmperosion);
            //Bitmap bmpdilatation2 = dilatation.Apply(bmpdilatation);

            ZXing.BarcodeReader reader = new ZXing.BarcodeReader();
            reader.AutoRotate = true;
            reader.Options = new ZXing.Common.DecodingOptions();
            reader.Options.TryHarder = true;
            reader.Options.TryInverted = true;
            reader.Options.PossibleFormats = new[] { ZXing.BarcodeFormat.DATA_MATRIX };

            Bitmap bmpLast = new Bitmap(bmperosion);

            int ireadIndex = 0;
            int ireadCount = 1;
            while (ireadIndex < ireadCount)
            {
                ZXing.Result result = reader.Decode(bmpLast);
                if (result != null)
                {
                    m_BarcodeStr = result.Text;
                    //stopwatch.Stop();
                    //m_ElapsedTime = stopwatch.ElapsedMilliseconds;
                    return 0;//成功读码
                }

                string dmtxStr = _getDMTX(bmpLast);
                if (!string.IsNullOrEmpty(dmtxStr))
                {
                    m_BarcodeStr = dmtxStr;
                    return 0;//成功读码
                }

                bmpLast.Dispose();
                //Size
                AForge.Imaging.Filters.ResizeBilinear resizeBilinear = new AForge.Imaging.Filters.ResizeBilinear(bmperosion.Width * 2, bmperosion.Height * 2);
                bmpLast = resizeBilinear.Apply(bmperosion);

                ireadIndex++;
            }


            bmpinput.Dispose();
            bmpinputsize168.Dispose();
            bmpgray.Dispose();
            //bmpotsu.Dispose();
            bmperosion.Dispose();
            bmpLast.Dispose();
            //bmpdilatation.Dispose();
            //bmpdilatation2.Dispose();

            //stopwatch.Stop();
            //m_ElapsedTime = stopwatch.ElapsedMilliseconds;
            return -1;//读码失败
        }
        private int _get2d_v3()
        {
            m_BarcodeStr = String.Empty;
            if (m_InputImage == null)
            {
                return -2;//图像错误
            }

            Bitmap bmpinput = new Bitmap(m_InputImage);
            Bitmap bmpinputsize168 = new Bitmap(m_InputImage);

            ////图像尺寸 最小 168*168
            //if (bmpinput.Width < 168 || bmpinput.Height < 168)
            //{
            //    bmpinputsize168.Dispose();
            //    bmpinputsize168 = new Bitmap(resizeImage(bmpinput, new Size(168, 168)));
            //}

            ////Gray
            //AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            //Bitmap bmpgray = grayscale.Apply(bmpinputsize168);

            //Gray
            AForge.Imaging.Filters.ContrastStretch contrastStretch = new AForge.Imaging.Filters.ContrastStretch();
            Bitmap bmpcontrastStretch = contrastStretch.Apply(bmpinputsize168);

            //Otsu
            AForge.Imaging.Filters.FiltersSequence filtersSequence = new AForge.Imaging.Filters.FiltersSequence(
                            AForge.Imaging.Filters.Grayscale.CommonAlgorithms.BT709,
                            new AForge.Imaging.Filters.DifferenceEdgeDetector());
            Bitmap bmpdifference = filtersSequence.Apply(bmpcontrastStretch);

            //Erosion
            AForge.Imaging.Filters.OtsuThreshold otsuThreshold = new AForge.Imaging.Filters.OtsuThreshold();
            Bitmap bmpotsuThreshold = otsuThreshold.Apply(bmpdifference);

            AForge.Imaging.Filters.Dilatation dilatation = new AForge.Imaging.Filters.Dilatation();
            Bitmap bmpdilatation = dilatation.Apply(bmpotsuThreshold);

            AForge.Imaging.Filters.Erosion erosion = new AForge.Imaging.Filters.Erosion();
            Bitmap bmpErosion = erosion.Apply(bmpdilatation);

            //Bitmap bmpdilatation2 = dilatation.Apply(bmpdilatation);

            ZXing.BarcodeReader reader = new ZXing.BarcodeReader();
            reader.AutoRotate = true;
            reader.Options = new ZXing.Common.DecodingOptions();
            reader.Options.TryHarder = true;
            reader.Options.TryInverted = true;
            reader.Options.PossibleFormats = new[] { ZXing.BarcodeFormat.DATA_MATRIX };

            Bitmap bmpLast = new Bitmap(bmpErosion);

            int ireadIndex = 0;
            int ireadCount = 1;
            while (ireadIndex < ireadCount)
            {
                ZXing.Result result = reader.Decode(bmpLast);
                if (result != null)
                {
                    m_BarcodeStr = result.Text;
                    //stopwatch.Stop();
                    //m_ElapsedTime = stopwatch.ElapsedMilliseconds;
                    return 0;//成功读码
                }

                string dmtxStr = _getDMTX(bmpLast);
                if (!string.IsNullOrEmpty(dmtxStr))
                {
                    m_BarcodeStr = dmtxStr;
                    return 0;//成功读码
                }

                bmpLast.Dispose();
                //Size
                AForge.Imaging.Filters.ResizeBilinear resizeBilinear =
                    new AForge.Imaging.Filters.ResizeBilinear(bmpErosion.Width * 2, bmpErosion.Height * 2);
                bmpLast = resizeBilinear.Apply(bmpErosion);

                ireadIndex++;
            }


            bmpinput.Dispose();
            bmpinputsize168.Dispose();
            //bmpgray.Dispose();
            //bmpotsu.Dispose();
            bmpErosion.Dispose();
            bmpLast.Dispose();
            //bmpdilatation.Dispose();
            //bmpdilatation2.Dispose();

            //stopwatch.Stop();
            //m_ElapsedTime = stopwatch.ElapsedMilliseconds;
            return -1;//读码失败
        }

        private int _get2d_v5_1()
        {
            m_BarcodeStr = String.Empty;
            if (m_InputImage == null)
            {
                return -2;//图像错误
            }

            Bitmap bmpinput = new Bitmap(m_InputImage);
            Bitmap bmpinputsize168 = new Bitmap(m_InputImage);

            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            Bitmap bmpgray = grayscale.Apply(bmpinputsize168);

            AForge.Imaging.Filters.HomogenityEdgeDetector detector = new AForge.Imaging.Filters.HomogenityEdgeDetector();
            Bitmap bmpdetector = detector.Apply(bmpgray);

            AForge.Imaging.Filters.SISThreshold sISThreshold = new AForge.Imaging.Filters.SISThreshold();
            Bitmap bmpsISThreshold = sISThreshold.Apply(bmpdetector);

            AForge.Imaging.Filters.Dilatation Dilatation3x3 = new AForge.Imaging.Filters.Dilatation();
            Bitmap bmpDilatation3x3 = Dilatation3x3.Apply(bmpsISThreshold);

            AForge.Imaging.Filters.Invert invert = new AForge.Imaging.Filters.Invert();
            Bitmap bmpinvert = invert.Apply(bmpDilatation3x3);

            AForge.Imaging.Filters.Erosion erosion3X3 = new AForge.Imaging.Filters.Erosion();
            Bitmap bmerosion3X3 = erosion3X3.Apply(bmpinvert);

            ZXing.BarcodeReader reader = new ZXing.BarcodeReader();
            reader.AutoRotate = true;
            reader.Options = new ZXing.Common.DecodingOptions();
            reader.Options.TryHarder = true;
            reader.Options.TryInverted = true;
            reader.Options.PossibleFormats = new[] { ZXing.BarcodeFormat.DATA_MATRIX };

            Bitmap bmpLast = new Bitmap(bmerosion3X3);

            int ireadIndex = 0;
            int ireadCount = 1;
            while (ireadIndex < ireadCount)
            {
                ZXing.Result result = reader.Decode(bmpLast);
                if (result != null)
                {
                    m_BarcodeStr = result.Text;
                    return 0;//成功读码
                }

                string dmtxStr = _getDMTX(bmpLast);
                if (!string.IsNullOrEmpty(dmtxStr))
                {
                    m_BarcodeStr = dmtxStr;
                    return 0;//成功读码
                }

                bmpLast.Dispose();

                ireadIndex++;
            }


            bmpinput.Dispose();
            bmpinputsize168.Dispose();
            //bmpgray.Dispose();
            //bmpotsu.Dispose();
            bmpLast.Dispose();
            //bmpdilatation.Dispose();
            //bmpdilatation2.Dispose();

            //stopwatch.Stop();
            //m_ElapsedTime = stopwatch.ElapsedMilliseconds;
            return -1;//读码失败
        }
        private int _get2d_v5_2()
        {
            m_BarcodeStr = String.Empty;
            if (m_InputImage == null)
            {
                return -2;//图像错误
            }

            Bitmap bmpinput = new Bitmap(m_InputImage);
            Bitmap bmpinputsize168 = new Bitmap(m_InputImage);

            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            Bitmap bmpgray = grayscale.Apply(bmpinputsize168);

            AForge.Imaging.Filters.ContrastStretch contrastStretch = new AForge.Imaging.Filters.ContrastStretch();
            Bitmap bmpcontrast = contrastStretch.Apply(bmpgray);

            AForge.Imaging.Filters.HomogenityEdgeDetector detector = new AForge.Imaging.Filters.HomogenityEdgeDetector();
            Bitmap bmpdetector = detector.Apply(bmpcontrast);

            AForge.Imaging.Filters.SISThreshold sISThreshold = new AForge.Imaging.Filters.SISThreshold();
            Bitmap bmpsISThreshold = sISThreshold.Apply(bmpdetector);

            AForge.Imaging.Filters.FillHoles fillHoles = new AForge.Imaging.Filters.FillHoles();
            fillHoles.MaxHoleHeight = 10;
            fillHoles.MaxHoleWidth = 10;
            Bitmap bmpfillHoles = fillHoles.Apply(bmpsISThreshold);

            AForge.Imaging.Filters.Invert invert = new AForge.Imaging.Filters.Invert();
            Bitmap bmpinvert = invert.Apply(bmpfillHoles);

            AForge.Imaging.Filters.Erosion erosion3X3 = new AForge.Imaging.Filters.Erosion();
            Bitmap bmerosion3X3 = erosion3X3.Apply(bmpinvert);

            ZXing.BarcodeReader reader = new ZXing.BarcodeReader();
            reader.AutoRotate = true;
            reader.Options = new ZXing.Common.DecodingOptions();
            reader.Options.TryHarder = true;
            reader.Options.TryInverted = true;
            reader.Options.PossibleFormats = new[] { ZXing.BarcodeFormat.DATA_MATRIX };

            Bitmap bmpLast = new Bitmap(bmerosion3X3);

            int ireadIndex = 0;
            int ireadCount = 2;
            while (ireadIndex < ireadCount)
            {
                ZXing.Result result = reader.Decode(bmpLast);
                if (result != null)
                {
                    m_BarcodeStr = result.Text;
                    return 0;//成功读码
                }

                string dmtxStr = _getDMTX(bmpLast);
                if (!string.IsNullOrEmpty(dmtxStr))
                {
                    m_BarcodeStr = dmtxStr;
                    return 0;//成功读码
                }

                bmpLast.Dispose();

                AForge.Imaging.Filters.Dilatation dilatation3X3 = new AForge.Imaging.Filters.Dilatation();
                bmpLast = dilatation3X3.Apply(bmerosion3X3);

                ireadIndex++;
            }


            bmpinput.Dispose();
            bmpinputsize168.Dispose();
            //bmpgray.Dispose();
            //bmpotsu.Dispose();
            bmpLast.Dispose();
            //bmpdilatation.Dispose();
            //bmpdilatation2.Dispose();

            //stopwatch.Stop();
            //m_ElapsedTime = stopwatch.ElapsedMilliseconds;
            return -1;//读码失败
        }
        private int _get2d_v5_5()
        {
            m_BarcodeStr = String.Empty;
            if (m_InputImage == null)
            {
                return -2;//图像错误
            }

            Bitmap bmpinput = new Bitmap(m_InputImage);
            Bitmap bmpinputsize168 = new Bitmap(m_InputImage);

            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            Bitmap bmpgray = grayscale.Apply(bmpinputsize168);

            AForge.Imaging.Filters.ContrastStretch contrastStretch = new AForge.Imaging.Filters.ContrastStretch();
            Bitmap bmpcontrast = contrastStretch.Apply(bmpgray);

            AForge.Imaging.Filters.BradleyLocalThresholding bradleyLocalThresholding = new AForge.Imaging.Filters.BradleyLocalThresholding();
            Bitmap bmpbradleyLocalThresholding = bradleyLocalThresholding.Apply(bmpcontrast);


            AForge.Imaging.Filters.FillHoles fillHoles = new AForge.Imaging.Filters.FillHoles();
            fillHoles.MaxHoleHeight = 5;
            fillHoles.MaxHoleWidth = 5;
            fillHoles.CoupledSizeFiltering = false;
            Bitmap bmpfillHoles = fillHoles.Apply(bmpbradleyLocalThresholding);

            AForge.Imaging.Filters.Invert invert = new AForge.Imaging.Filters.Invert();
            Bitmap bmpinvert = invert.Apply(bmpfillHoles);

            AForge.Imaging.Filters.BlobsFiltering blobsFiltering = new AForge.Imaging.Filters.BlobsFiltering();
            blobsFiltering.MinHeight = 5;
            blobsFiltering.MinWidth = 5;
            blobsFiltering.CoupledSizeFiltering = false;
            Bitmap bmpblobsFiltering = blobsFiltering.Apply(bmpinvert);

            AForge.Imaging.Filters.FillHoles fillHoles1 = new AForge.Imaging.Filters.FillHoles();
            fillHoles1.MaxHoleHeight = 5;
            fillHoles1.MaxHoleWidth = 5;
            fillHoles1.CoupledSizeFiltering = false;
            Bitmap bmpfillHoles1 = fillHoles1.Apply(bmpblobsFiltering);


            ZXing.BarcodeReader reader = new ZXing.BarcodeReader();
            reader.AutoRotate = true;
            reader.Options = new ZXing.Common.DecodingOptions();
            reader.Options.TryHarder = true;
            reader.Options.TryInverted = true;
            reader.Options.PossibleFormats = new[] { ZXing.BarcodeFormat.DATA_MATRIX };

            Bitmap bmpLast = new Bitmap(bmpfillHoles1);

            int ireadIndex = 0;
            int ireadCount = 1;
            while (ireadIndex < ireadCount)
            {
                ZXing.Result result = reader.Decode(bmpLast);
                if (result != null)
                {
                    m_BarcodeStr = result.Text;
                    return 0;//成功读码
                }


                string dmtxStr = _getDMTX(bmpLast);
                if (!string.IsNullOrEmpty(dmtxStr))
                {
                    m_BarcodeStr = dmtxStr;
                    return 0;//成功读码
                }

                bmpLast.Dispose();

                //AForge.Imaging.Filters.Dilatation3x3 dilatation3X3 = new AForge.Imaging.Filters.Dilatation3x3();
                //bmpLast = dilatation3X3.Apply(bmerosion3X3);

                ireadIndex++;
            }


            bmpinput.Dispose();
            bmpinputsize168.Dispose();
            //bmpgray.Dispose();
            //bmpotsu.Dispose();
            bmpLast.Dispose();
            //bmpdilatation.Dispose();
            //bmpdilatation2.Dispose();

            //stopwatch.Stop();
            //m_ElapsedTime = stopwatch.ElapsedMilliseconds;
            return -1;//读码失败
        }
        private int _get2d_v5_6()
        {
            m_BarcodeStr = String.Empty;
            if (m_InputImage == null)
            {
                return -2;//图像错误
            }

            Bitmap bmpinput = new Bitmap(m_InputImage);
            Bitmap bmpinputsize168 = new Bitmap(m_InputImage);

            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            Bitmap bmpgray = grayscale.Apply(bmpinputsize168);

            AForge.Imaging.Filters.Threshold threshold = new AForge.Imaging.Filters.Threshold(12);
            Bitmap bmpthreshold = threshold.Apply(bmpgray);

            AForge.Imaging.Filters.OtsuThreshold otsuThreshold = new AForge.Imaging.Filters.OtsuThreshold();
            Bitmap bmpoThreshold = otsuThreshold.Apply(bmpgray);

            AForge.Imaging.Filters.Subtract subtract = new AForge.Imaging.Filters.Subtract(bmpthreshold);
            Bitmap bmpsubtract = subtract.Apply(bmpoThreshold);

            AForge.Imaging.Filters.Invert invert = new AForge.Imaging.Filters.Invert();
            Bitmap bmpinvert = invert.Apply(bmpsubtract);

            AForge.Imaging.Filters.Closing closing = new AForge.Imaging.Filters.Closing();
            Bitmap bmclosing = closing.Apply(bmpinvert);

            ZXing.BarcodeReader reader = new ZXing.BarcodeReader();
            reader.AutoRotate = true;
            reader.Options = new ZXing.Common.DecodingOptions();
            reader.Options.TryHarder = true;
            reader.Options.TryInverted = true;
            reader.Options.PossibleFormats = new[] { ZXing.BarcodeFormat.DATA_MATRIX };

            Bitmap bmpLast = new Bitmap(bmclosing);

            int ireadIndex = 0;
            int ireadCount = 1;
            while (ireadIndex < ireadCount)
            {
                ZXing.Result result = reader.Decode(bmpLast);
                if (result != null)
                {
                    m_BarcodeStr = result.Text;
                    return 0;//成功读码
                }

                string dmtxStr = _getDMTX(bmpLast);
                if (!string.IsNullOrEmpty(dmtxStr))
                {
                    m_BarcodeStr = dmtxStr;
                    return 0;//成功读码
                }

                bmpLast.Dispose();

                ireadIndex++;
            }


            bmpinput.Dispose();
            bmpinputsize168.Dispose();
            //bmpgray.Dispose();
            //bmpotsu.Dispose();
            bmpLast.Dispose();
            //bmpdilatation.Dispose();
            //bmpdilatation2.Dispose();

            //stopwatch.Stop();
            //m_ElapsedTime = stopwatch.ElapsedMilliseconds;
            return -1;//读码失败
        }
        private int _get2d_v5_7()
        {
            m_BarcodeStr = String.Empty;
            if (m_InputImage == null)
            {
                return -2;//图像错误
            }

            Bitmap bmpinput = new Bitmap(m_InputImage);
            Bitmap bmpinputsize168 = new Bitmap(m_InputImage);

            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            Bitmap bmpgray = grayscale.Apply(bmpinputsize168);

            AForge.Imaging.Filters.OtsuThreshold otsuThreshold = new AForge.Imaging.Filters.OtsuThreshold();
            Bitmap bmpoThreshold = otsuThreshold.Apply(bmpgray);

            AForge.Imaging.Filters.Dilatation3x3 dilatation3X3 = new AForge.Imaging.Filters.Dilatation3x3();
            Bitmap bmpdilatation3X3 = dilatation3X3.Apply(bmpoThreshold);

            ZXing.BarcodeReader reader = new ZXing.BarcodeReader();
            reader.AutoRotate = true;
            reader.Options = new ZXing.Common.DecodingOptions();
            reader.Options.TryHarder = true;
            reader.Options.TryInverted = true;
            reader.Options.PossibleFormats = new[] { ZXing.BarcodeFormat.DATA_MATRIX };

            Bitmap bmpLast = new Bitmap(bmpdilatation3X3);

            int ireadIndex = 0;
            int ireadCount = 1;
            while (ireadIndex < ireadCount)
            {
                ZXing.Result result = reader.Decode(bmpLast);
                if (result != null)
                {
                    m_BarcodeStr = result.Text;
                    return 0;//成功读码
                }

                string dmtxStr = _getDMTX(bmpLast);
                if (!string.IsNullOrEmpty(dmtxStr))
                {
                    m_BarcodeStr = dmtxStr;
                    return 0;//成功读码
                }

                bmpLast.Dispose();

                ireadIndex++;
            }


            bmpinput.Dispose();
            bmpinputsize168.Dispose();
            //bmpgray.Dispose();
            //bmpotsu.Dispose();
            bmpLast.Dispose();
            //bmpdilatation.Dispose();
            //bmpdilatation2.Dispose();

            //stopwatch.Stop();
            //m_ElapsedTime = stopwatch.ElapsedMilliseconds;
            return -1;//读码失败
        }
        private int _get2d_v5_8()
        {
            m_BarcodeStr = String.Empty;
            if (m_InputImage == null)
            {
                return -2;//图像错误
            }

            Bitmap bmpinput = new Bitmap(m_InputImage);
            Bitmap bmpinputsize168 = new Bitmap(m_InputImage);

            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            Bitmap bmpgray = grayscale.Apply(bmpinputsize168);

            AForge.Imaging.Filters.OtsuThreshold otsuThreshold = new AForge.Imaging.Filters.OtsuThreshold();
            Bitmap bmpoThreshold = otsuThreshold.Apply(bmpgray);

            AForge.Imaging.Filters.Intersect intersect = new AForge.Imaging.Filters.Intersect();
            Bitmap bmpoIntersect = intersect.Apply(bmpoThreshold);

            AForge.Imaging.Filters.Erosion erosion = new AForge.Imaging.Filters.Erosion();
            Bitmap bmperosion = erosion.Apply(bmpoIntersect);

            AForge.Imaging.Filters.CannyEdgeDetector detector = new AForge.Imaging.Filters.CannyEdgeDetector();
            Bitmap bmpcannyEdgeDetector = detector.Apply(bmpgray);

            AForge.Imaging.Filters.OtsuThreshold otsuThreshold1 = new AForge.Imaging.Filters.OtsuThreshold();
            Bitmap bmpoThreshold1 = otsuThreshold1.Apply(bmpcannyEdgeDetector);



            ZXing.BarcodeReader reader = new ZXing.BarcodeReader();
            reader.AutoRotate = true;
            reader.Options = new ZXing.Common.DecodingOptions();
            reader.Options.TryHarder = true;
            reader.Options.TryInverted = true;
            reader.Options.PossibleFormats = new[] { ZXing.BarcodeFormat.DATA_MATRIX };

            Bitmap bmpLast = new Bitmap(bmpoThreshold1);

            int ireadIndex = 0;
            int ireadCount = 1;
            while (ireadIndex < ireadCount)
            {
                ZXing.Result result = reader.Decode(bmpLast);
                if (result != null)
                {
                    m_BarcodeStr = result.Text;
                    return 0;//成功读码
                }

                bmpLast.Dispose();

                ireadIndex++;
            }


            bmpinput.Dispose();
            bmpinputsize168.Dispose();
            //bmpgray.Dispose();
            //bmpotsu.Dispose();
            bmpLast.Dispose();
            //bmpdilatation.Dispose();
            //bmpdilatation2.Dispose();

            //stopwatch.Stop();
            //m_ElapsedTime = stopwatch.ElapsedMilliseconds;
            return -1;//读码失败
        }

        private Bitmap resizeImage(Bitmap imgToResize, Size size)
        {
            //获取图片宽度
            int sourceWidth = imgToResize.Width;
            //获取图片高度
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            //计算宽度的缩放比例
            nPercentW = ((float)size.Width / (float)sourceWidth);
            //计算高度的缩放比例
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;
            //期望的宽度
            int destWidth = (int)(sourceWidth * nPercent);
            //期望的高度
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((System.Drawing.Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //绘制图像
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return b;
        }

        private string _getDMTX(Bitmap ebmpInput)
        {
            //DataMatrix.net.DmtxImageDecoder dmtxImageDecoder = new DataMatrix.net.DmtxImageDecoder();
            //List<string> list = dmtxImageDecoder.DecodeImage(ebmpInput, 1, new TimeSpan(0, 0, 0, 0, 200));
            //bool flag = list != null && list.Count == 1;
            //if (flag)
            //{
            //    return list[0];
            //}
            return string.Empty;
            //if (!System.IO.Directory.Exists(m_PathFile))
            //    System.IO.Directory.CreateDirectory(m_PathFile);
            //ebmpInput.Save(m_PathFile + "\\" + m_PathCount.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
            //m_PathCount++;
            //var err = model.DecodeDataMatrix(ebmpInput, false, out string[] codes, out Rectangle[] locs);
            //if (err.errCode == 0)
            //{
            //    bool flag1 = codes != null && codes.Length == 1;
            //    if (flag1)
            //    {
            //        return codes[0]+";seg";
            //    }
            //}
            //return string.Empty;
        }
        #endregion

    }
}
