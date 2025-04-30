using AUVision;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VisionDesigner;
using VisionDesigner.AlmightyPatMatch;
using ZXing;

namespace Allinone.BasicSpace
{


    public class JzMVDJudgeRecipeClass
    {
        private string m_PatternPath = "";
        private PMatchType m_PMatchType = PMatchType.HPM;

        public float MTRotation { get; set; } = 30;
        public float MTTolerance { get; set; } = 0.7f;
        public float MTMaxOcc { get; set; } = 1;

        //VisionDesigner.AlmightyPatMatch.CAlmightyPattern[] cAlmightyPatternObj = null;
        VisionDesigner.AlmightyPatMatch.CAlmightyPatMatchTool cAlmightyPatmatchToolObj = null;
        List<CAlmightyPattern> cAlmightyPatterns = new List<CAlmightyPattern>();

        public JzMVDJudgeRecipeClass(string eDir, PMatchType pMatchType)
        {
            m_PatternPath = eDir;
            m_PMatchType = pMatchType;
        }
        public int Init()
        {
            cAlmightyPatterns.Clear();
            //string[] files = System.IO.Directory.GetFiles(eDir, "*.hpmxml;*.fmxml");
            string search = "*.hpmxml";
            switch (m_PMatchType)
            {
                case PMatchType.FM:
                    search = "*.fmxml";
                    break;
            }
            string[] files = System.IO.Directory.GetFiles(m_PatternPath, search);//使用高精度匹配

            Parallel.ForEach(files, file =>
            {

                if (File.Exists(file))
                {
                    _loadMVDPattern(file);
                }

            });

            //Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = 4 }, file =>
            // {
            //     if (File.Exists(file))
            //     {
            //         _loadMVDPattern(file);
            //     }
            // });

            //foreach (string file in files)
            //{
            //    if (File.Exists(file))
            //    {
            //        _loadMVDPattern(file);
            //    }
            //}

            return 0;
        }
        public string GetRecipeName(Bitmap bmpinput, string mxType = "")
        {
            string retstr = string.Empty;
            ////先判断最下面一行的个数

            //JzCheckRecipeParaClass jzCheckRecipe = new JzCheckRecipeParaClass();
            //jzCheckRecipe.FromingStr(INI.SDM5FindCount);
            //if (jzCheckRecipe.IsOpenFindCount)
            //{

            //}

            // CreateToolInstance
            if (cAlmightyPatmatchToolObj == null)
                cAlmightyPatmatchToolObj = new VisionDesigner.AlmightyPatMatch.CAlmightyPatMatchTool();

            //Set type
            switch (m_PMatchType)
            {
                case PMatchType.FM:
                    cAlmightyPatmatchToolObj.Type = PatMatchAlgorithmType.FastFeature;
                    break;
                case PMatchType.HPM:
                    cAlmightyPatmatchToolObj.Type = PatMatchAlgorithmType.HPFeature;
                    break;
            }

            //// Set ROI region (optional)
            cAlmightyPatmatchToolObj.RegionList.Clear();
            var region2 = new VisionDesigner.CMvdRectangleF(bmpinput.Width * 0.5f, bmpinput.Height * 0.5f, bmpinput.Width, bmpinput.Height);
            cAlmightyPatmatchToolObj.RegionList.Add(new CAlmightyPatMatchRegion(region2, true));

            // Set basic parameter

            cAlmightyPatmatchToolObj.BasicParam.ShowOutlineStatus = false;

            cAlmightyPatmatchToolObj.SetRunParam("AngleStart", (-MTRotation).ToString());
            cAlmightyPatmatchToolObj.SetRunParam("AngleEnd", MTRotation.ToString());
            cAlmightyPatmatchToolObj.SetRunParam("MinScore", MTTolerance.ToString());
            cAlmightyPatmatchToolObj.SetRunParam("MaxMatchNum", MTMaxOcc.ToString());

            // Set input image
            //Bitmap bmp24 = bmpinput.Clone(new Rectangle(0, 0, bmpinput.Width, bmpinput.Height), PixelFormat.Format8bppIndexed);
            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            Bitmap bmp24 = grayscale.Apply(bmpinput);
            VisionDesigner.CMvdImage cInputImg2 = BitmapToCMvdImage(bmp24);
            if (cInputImg2.PixelFormat != MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08)
            {
                //当前程序仅支持mono8。因此像素格会转换.
                cInputImg2.ConvertImagePixelFormat(MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08);
            }
            //cInputImg2.InitImage("InputTest2.bmp");

            //cInputImg2.SaveImage($"D:\\Data\\cInputImg2run_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.bmp", MVD_FILE_FORMAT.MVD_FILE_BMP);

            bmp24.Dispose();
            grayscale = null;

            cAlmightyPatmatchToolObj.InputImage = cInputImg2;


            //cAlmightyPatmatchToolObj.InputImage.SaveImage("D:\\Data\\matchrun_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + Universal.GlobalImageTypeString);

            //foreach (var item in cHPMatchRes.MatchInfoList)

            //{

            //    Console.WriteLine("MatchPoint: ({0},{1})", item.MatchPoint.fX, item.MatchPoint.fY);

            //}

            // Set Pattern
            foreach (var item in cAlmightyPatterns)
            {
                if (!string.IsNullOrEmpty(mxType))
                {
                    if (!item.Name.Contains(mxType))
                    {
                        continue;
                    }
                }

                cAlmightyPatmatchToolObj.Pattern = item;
                try
                {
                    // Running
                    cAlmightyPatmatchToolObj.Run();
                }
                catch
                {

                }

                // Get the result
                VisionDesigner.AlmightyPatMatch.CAlmightyPatMatchResult cHPMatchRes = cAlmightyPatmatchToolObj.Result;

                //cAlmightyPatmatchToolObj.Pattern = cAlmightyPatternObj;

                if (cHPMatchRes != null)
                {
                    if (cHPMatchRes.MatchInfoList.Count > 0)
                    {
                        retstr = item.Name;
                        break;
                    }
                }
            }

            return retstr;
        }

        void _loadMVDPattern(string efilepath)
        {
            //// CreatePatternInstance
            //if (cAlmightyPatternObj == null)
            //    cAlmightyPatternObj = new VisionDesigner.AlmightyPatMatch.CAlmightyPattern();

            //if (File.Exists(efilepath))
            //{
            //    cAlmightyPatternObj.ImportPattern(efilepath);
            //}

            if (File.Exists(efilepath))
            {
                CAlmightyPattern cAlmightyPatternObj = new VisionDesigner.AlmightyPatMatch.CAlmightyPattern();
                cAlmightyPatternObj.ImportPattern(efilepath, PatternImportType.MatchOnly);
                FileInfo fileInfo = new FileInfo(efilepath);
                cAlmightyPatternObj.Name = fileInfo.Name.Replace(fileInfo.Extension, "");
                cAlmightyPatterns.Add(cAlmightyPatternObj);

                cAlmightyPatternObj.Dispose();
            }
        }

        /// <summary>
        /// Bitmap 转成 CMvdImage
        /// </summary>
        /// <param name="bmpInputImg"></param>
        /// <returns></returns>
        private CMvdImage BitmapToCMvdImage(Bitmap bmpInputImg)
        {
            CMvdImage cMvdImage = new CMvdImage();
            System.Drawing.Imaging.PixelFormat bitPixelFormat = bmpInputImg.PixelFormat;
            BitmapData bmData = bmpInputImg.LockBits(new Rectangle(0, 0, bmpInputImg.Width, bmpInputImg.Height), ImageLockMode.ReadOnly, bitPixelFormat);//锁定

            if (bitPixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
            {
                Int32 bitmapDataSize = bmData.Stride * bmData.Height;//bitmap图像缓存长度
                int offset = bmData.Stride - bmData.Width;
                Int32 ImageBaseDataSize = bmData.Width * bmData.Height;//imageBaseData_V2图像真正的缓存长度
                byte[] _BitImageBufferBytes = new byte[bitmapDataSize];
                byte[] _ImageBaseDataBufferBytes = new byte[ImageBaseDataSize];
                Marshal.Copy(bmData.Scan0, _BitImageBufferBytes, 0, bitmapDataSize);
                int bitmapIndex = 0;
                int ImageBaseDataIndex = 0;
                for (int i = 0; i < bmData.Height; i++)
                {
                    for (int j = 0; j < bmData.Width; j++)
                    {
                        _ImageBaseDataBufferBytes[ImageBaseDataIndex++] = _BitImageBufferBytes[bitmapIndex++];
                    }
                    bitmapIndex += offset;
                }
                MVD_IMAGE_DATA_INFO stImageData = new MVD_IMAGE_DATA_INFO();
                stImageData.stDataChannel[0].nRowStep = (uint)bmData.Width;
                stImageData.stDataChannel[0].nLen = (uint)ImageBaseDataSize;
                stImageData.stDataChannel[0].nSize = (uint)ImageBaseDataSize;
                stImageData.stDataChannel[0].arrDataBytes = _ImageBaseDataBufferBytes;
                cMvdImage.InitImage((uint)bmData.Width, (uint)bmData.Height, MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08, stImageData);
            }
            else if (bitPixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
            {
                Int32 bitmapDataSize = bmData.Stride * bmData.Height;//bitmap图像缓存长度
                int offset = bmData.Stride - bmData.Width * 3;
                Int32 ImageBaseDataSize = bmData.Width * bmData.Height * 3;//imageBaseData_V2图像真正的缓存长度
                byte[] _BitImageBufferBytes = new byte[bitmapDataSize];
                byte[] _ImageBaseDataBufferBytes = new byte[ImageBaseDataSize];
                Marshal.Copy(bmData.Scan0, _BitImageBufferBytes, 0, bitmapDataSize);
                int bitmapIndex = 0;
                int ImageBaseDataIndex = 0;
                for (int i = 0; i < bmData.Height; i++)
                {
                    for (int j = 0; j < bmData.Width; j++)
                    {
                        _ImageBaseDataBufferBytes[ImageBaseDataIndex++] = _BitImageBufferBytes[bitmapIndex + 2];
                        _ImageBaseDataBufferBytes[ImageBaseDataIndex++] = _BitImageBufferBytes[bitmapIndex + 1];
                        _ImageBaseDataBufferBytes[ImageBaseDataIndex++] = _BitImageBufferBytes[bitmapIndex];
                        bitmapIndex += 3;
                    }
                    bitmapIndex += offset;
                }
                MVD_IMAGE_DATA_INFO stImageData = new MVD_IMAGE_DATA_INFO();
                stImageData.stDataChannel[0].nRowStep = (uint)bmData.Width * 3;
                stImageData.stDataChannel[0].nLen = (uint)ImageBaseDataSize;
                stImageData.stDataChannel[0].nSize = (uint)ImageBaseDataSize;
                stImageData.stDataChannel[0].arrDataBytes = _ImageBaseDataBufferBytes;
                cMvdImage.InitImage((uint)bmData.Width, (uint)bmData.Height, MVD_PIXEL_FORMAT.MVD_PIXEL_RGB_RGB24_C3, stImageData);
            }
            bmpInputImg.UnlockBits(bmData);  // 解除锁定
            return cMvdImage;
        }

    }
}
