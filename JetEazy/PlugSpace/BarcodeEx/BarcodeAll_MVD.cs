﻿
#define OPT_USE_MVD_BARCODE
#if OPT_USE_MVD_BARCODE
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using VisionDesigner;
using VisionDesigner.Code2DReader;

namespace JetEazy.PlugSpace.BarcodeEx
{
    public class BarcodeItem
    {
        public C2DCodeInfo DCodeInfo = null;
        public string OverGrade = string.Empty;
        public string DecodeGrade = string.Empty;
        public void Reset()
        {
            OverGrade = string.Empty;
            DecodeGrade = string.Empty;
            DCodeInfo = null;
        }
    }

    public class BarcodeAll_MVD
    {

        private long m_DurTime = 0;

        public BarcodeItem GetBarcodeItem = new BarcodeItem();

        public BarcodeAll_MVD()
        {

        }
        /// <summary>
        /// 解码 DATAMATRIX
        /// </summary>
        /// <param name="ebmpInput">输入图片 需要白底黑字</param>
        /// <returns>返回 条码字符串</returns>
        public string DecodeStr(Bitmap ebmpInput)
        {
            return Decode(ebmpInput);
        }
        public string DecodeGrade(Bitmap ebmpInput)
        {
            return _DecodeGrade(ebmpInput);
        }
        public long DecodeStrDurtime
        {
            get { return m_DurTime; }
        }

        string _DecodeGrade(Bitmap ebmpInput)
        {
            string retStr = string.Empty;

            try
            {
                GetBarcodeItem.Reset();

                Stopwatch RunLineWatch = new Stopwatch();
                RunLineWatch.Start();

                CMvdImage cInputImg = BitmapToCMvdImage(ebmpInput);
                if (cInputImg.PixelFormat != MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08)
                {
                    //当前程序仅支持mono8。因此像素格会转换.
                    cInputImg.ConvertImagePixelFormat(MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08);
                }

                int nImageWidth = (int)cInputImg.Width;
                int nImageHeight = (int)cInputImg.Height;

                //创建ROI（非必须）
                CMvdRectangleF cRectROIObj = new CMvdRectangleF(nImageWidth / 2, nImageHeight / 2, nImageWidth, nImageHeight);
                cRectROIObj.Angle = 0;

                //二维码识别实例创建与赋值
                C2DCodeReaderTool Code2DReaderTool = new C2DCodeReaderTool();
                Code2DReaderTool.InputImage = cInputImg;
                Code2DReaderTool.ROI = cRectROIObj;
                //Code2DReaderTool.SetRunParam("Polarity", "1");
                Code2DReaderTool.SetRunParam("CodeQRFlag", "0");

                //string strkey = "";
                //Code2DReaderTool.SetRunParam("Polarity", "2");
                //Code2DReaderTool.GetRunParam("Polarity", ref strkey);

                //Code2DReaderTool.SetRunParam("Loc2DCodeNum", "60");
                //Code2DReaderTool.GetRunParam("Loc2DCodeNum", ref strkey);

                //运行
                Code2DReaderTool.Run();

                RunLineWatch.Stop();
                m_DurTime = RunLineWatch.ElapsedMilliseconds;
                RunLineWatch = null;

                if (Code2DReaderTool.Result.CodeInfoList.Count() > 0)
                {
                    retStr = Code2DReaderTool.Result.CodeInfoList[0].Content.ToString();

                    C2DCodeVerifyTool c2DCodeVerifyTool = new C2DCodeVerifyTool();
                    c2DCodeVerifyTool.InputImage = cInputImg;
                    c2DCodeVerifyTool.BasicParam.RecognitionInfo = Code2DReaderTool.Result;
                    c2DCodeVerifyTool.BasicParam.VerifyStandard =
                        MVD_SYMBOL_VERIFY_STANDARD.MVD_SYMBOL_VERIFY_ISO_STANDARD_29158;
                    c2DCodeVerifyTool.Run();


                    if (c2DCodeVerifyTool.Result.QualityInfoList.Count() > 0)
                    {
                        C2DCodeQualityInfoByISO29158 c2DCodeQualityInfoByISO29158
                            = (C2DCodeQualityInfoByISO29158)c2DCodeVerifyTool.Result.QualityInfoList[0];

                        C2DCodeReaderResult c2DCodeReaderResult
                            = c2DCodeVerifyTool.BasicParam.RecognitionInfo;
                        GetBarcodeItem.DCodeInfo = c2DCodeReaderResult.CodeInfoList[0];

                        GetBarcodeItem.OverGrade = _getBarcodeGrade(c2DCodeQualityInfoByISO29158.OverQuality);
                        GetBarcodeItem.DecodeGrade = _getBarcodeGrade(c2DCodeQualityInfoByISO29158.DecodeGrade);
                    }
                }

                Code2DReaderTool.Dispose();
                Code2DReaderTool = null;

                cInputImg.Dispose();
                cInputImg = null;

            }
            catch (Exception ex)
            {

            }
            return retStr;
        }
        string Decode(Bitmap ebmpInput)
        {
            string retStr = string.Empty;

            try
            {
                //Bitmap bmptemp = new Bitmap(strBasicImgPath);
                //Bitmap bmp = bmptemp.Clone(new Rectangle(0, 0, bmptemp.Width, bmptemp.Height), PixelFormat.Format24bppRgb);
                //bmptemp.Dispose();

                Stopwatch RunLineWatch = new Stopwatch();
                RunLineWatch.Start();


                CMvdImage cInputImg = BitmapToCMvdImage(ebmpInput);
                if (cInputImg.PixelFormat != MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08)
                {
                    //当前程序仅支持mono8。因此像素格会转换.
                    cInputImg.ConvertImagePixelFormat(MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08);
                }

                int nImageWidth = (int)cInputImg.Width;
                int nImageHeight = (int)cInputImg.Height;

                //创建ROI（非必须）
                CMvdRectangleF cRectROIObj = new CMvdRectangleF(nImageWidth / 2, nImageHeight / 2, nImageWidth, nImageHeight);
                cRectROIObj.Angle = 0;

                //二维码识别实例创建与赋值
                C2DCodeReaderTool Code2DReaderTool = new C2DCodeReaderTool();
                Code2DReaderTool.InputImage = cInputImg;
                Code2DReaderTool.ROI = cRectROIObj;
                //Code2DReaderTool.SetRunParam("Polarity", "1");
                Code2DReaderTool.SetRunParam("CodeQRFlag", "0");

                //运行
                Code2DReaderTool.Run();

                //listResurt.Items.Add("运行时间:" + Code2DReaderTool.GetAlgRunTime().ToString("0.000") + " ms");
                ////显示结果
                //listResurt.Items.Add("总共读到条码数: " + Code2DReaderTool.Result.CodeInfoList.Count().ToString());
                //for (int i = 0; i < Code2DReaderTool.Result.CodeInfoList.Count(); i++)
                //{
                //    //二维码信息
                //    listResurt.Items.Add("第:" + ((i + 1).ToString() + " 个"));
                //    listResurt.Items.Add("中心点 = {"
                //          + Code2DReaderTool.Result.CodeInfoList[i].Center.nX.ToString() + ", "
                //          + Code2DReaderTool.Result.CodeInfoList[i].Center.nY.ToString() + "}");

                //    listResurt.Items.Add("角度 = " + Code2DReaderTool.Result.CodeInfoList[i].Angle.ToString());
                //    listResurt.Items.Add("类型= " + Code2DReaderTool.Result.CodeInfoList[i].Type.ToString());
                //    listResurt.Items.Add("内容 = " + Code2DReaderTool.Result.CodeInfoList[i].Content.ToString());
                //    listResurt.Items.Add("估计PPM = " + Code2DReaderTool.Result.CodeInfoList[i].Estppm.ToString());

                //    listResurt.Items.Add("");
                //}

                RunLineWatch.Stop();
                m_DurTime = RunLineWatch.ElapsedMilliseconds;
                RunLineWatch = null;

                if (Code2DReaderTool.Result.CodeInfoList.Count() > 0)
                {
                    retStr = Code2DReaderTool.Result.CodeInfoList[0].Content.ToString();
                }

                Code2DReaderTool.Dispose();
                Code2DReaderTool = null;

                cInputImg.Dispose();
                cInputImg = null;

            }
            catch (Exception ex)
            {
                //listResurt.Items.Add(ex.Message);

            }
            return retStr;
        }

        string _getBarcodeLocation(MVD_POINT_I mVD_POINT_I)
        {
            return mVD_POINT_I.nX.ToString() + ":" + mVD_POINT_I.nY.ToString();
        }
        string _getBarcodeGrade(byte ebyte)
        {
            switch (ebyte)
            {
                case 0: return "F";
                case 1: return "D";
                case 2: return "C";
                case 3: return "B";
                case 4: return "A";
            }
            return "A";
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


#endif