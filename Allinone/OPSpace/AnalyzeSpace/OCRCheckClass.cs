﻿
#define OPT_USE_MVD_BARCODE
#define OPT_USE_JET_BARCODE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using JETLIB;
using System.Collections.Concurrent;
using System.Drawing.Imaging;
using AUVision;
using Allinone.BasicSpace;
using System.Diagnostics;

namespace Allinone.OPSpace.AnalyzeSpace
{
    public class OCRCheckClass
    {
        public OCRMethodEnum OCRMethod = OCRMethodEnum.NONE;
        // public OCRSETEnum OCRMappingMethod = OCRSETEnum.NONE; //"None";
        public string OCRMappingMethod = "None";

        public WorkStatusCollectionClass TrainStatusCollection = new WorkStatusCollectionClass();
        public WorkStatusCollectionClass RunStatusCollection = new WorkStatusCollectionClass();

        public Bitmap bmpErr = new Bitmap(1, 1);
        JetEazy.PlugSpace.BarcodeEx.BarcodeAll_MVD m_MvdCnnReader = new JetEazy.PlugSpace.BarcodeEx.BarcodeAll_MVD();

        public string BarcodeGrade = string.Empty;
        string m_BarcodeReadStr = string.Empty;

        public OCRCheckClass()
        {

        }
        public OCRCheckClass(string str)
        {
            FromString(str);
        }
        public override string ToString()
        {
            string str = "";

            str += ((int)OCRMethod).ToString() + Universal.SeperateCharB;   //0
            str += OCRMappingMethod + Universal.SeperateCharB;    //1

            str += "";

            return str;
        }
        public void FromString(string str)
        {
            string[] strs = str.Split(Universal.SeperateCharB);

            OCRMethod = (OCRMethodEnum)int.Parse(strs[0]);
            //  OCRMappingMethod = (OCRSETEnum)int.Parse(strs[1]);
            OCRMappingMethod = strs[1];
        }
        public void Reset()
        {
            OCRMethod = OCRMethodEnum.NONE;
            OCRMappingMethod = "None";
            //  OCRMappingMethod = OCRSETEnum.NONE;
        }

        public void FromPropertyChange(string changeitemstring, string valuestring)
        {
            string[] str = changeitemstring.Split(';');

            if (str[0] != "05.OCR or Barcode")
                return;

            switch (str[1])
            {
                case "OCRMethod":
                    OCRMethod = (OCRMethodEnum)Enum.Parse(typeof(OCRMethodEnum), valuestring, true);
                    break;
                case "OCRMappingMethod":
                    //   OCRMappingMethod = (OCRSETEnum)Enum.Parse(typeof(OCRSETEnum), valuestring, true);// valuestring;
                    OCRMappingMethod = valuestring;
                    break;
            }
        }

        public string DeCode(string barcode, bool istrain, Bitmap bmppattern, Bitmap bmpFind, PassInfoClass passInfo, out bool isgood)
        {
            WorkStatusClass workstatus = new WorkStatusClass(JetEazy.AnanlyzeProcedureEnum.CHECKBARCODE);
            string processstring = "Start  Decode." + Environment.NewLine;
            string errorstring = "";
            JetEazy.ReasonEnum reason = JetEazy.ReasonEnum.PASS;

            ZXing.BarcodeReader reader = new ZXing.BarcodeReader();
            ZXing.Result result = reader.Decode(bmpFind);

            if (result != null && barcode != result.Text && !istrain)
            {
                errorstring += "1DBarcode Mismatch." + Environment.NewLine; ;
                processstring += "1DBarcode Mismatch." + Environment.NewLine;
                reason = JetEazy.ReasonEnum.NG;

                Universal.R3UI.isSNResult = false;
                isgood = false;
            }
            else
            {
                processstring += "1DBarcode Check OK." + Environment.NewLine;
                errorstring += "";
                reason = JetEazy.ReasonEnum.PASS;
                isgood = true;
            }
            //CHECKBARCODE(new Bitmap(bmpFind));

            if (INI.ISSAVEOCRIMAGE)
            {
                if (System.IO.Directory.Exists(Universal.BarcodeIMAGEPATH) == false)//如果不存在就创建file文件夹
                    System.IO.Directory.CreateDirectory(Universal.BarcodeIMAGEPATH);
                bmpFind.Save(Universal.BarcodeIMAGEPATH + barcode + "_" + OCRMappingMethod + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            }

            workstatus.SetWorkStatus(bmppattern, bmpFind, bmpErr, reason, errorstring, processstring, passInfo);

            if (istrain)
                TrainStatusCollection.Add(workstatus);
            else
                RunStatusCollection.Add(workstatus);

            return result == null ? "" : result.Text;
        }

        public string DeCode2d(string eName, bool eCheckBarcode, string barcode, bool istrain, Bitmap bmppattern, Bitmap bmpFind, PassInfoClass passInfo, out bool isgood, out string barcodeStrRead)
        {
            WorkStatusClass workstatus = new WorkStatusClass(JetEazy.AnanlyzeProcedureEnum.CHECKBARCODE);
            string processstring = "Start  DeCode2d." + Environment.NewLine;
            string errorstring = "";
            JetEazy.ReasonEnum reason = JetEazy.ReasonEnum.PASS;

            string myBarcode = "";
            barcodeStrRead = string.Empty;
            m_BarcodeReadStr = string.Empty;

            switch (Universal.OPTION)
            {
                case JetEazy.OptionEnum.MAIN_SDM2:
                    Universal.bmpProvideAI.Dispose();
                    Universal.bmpProvideAI = new Bitmap(bmpFind);
                    break;
            }

#if OPT_USE_MVD_BARCODE

            switch (Universal.OPTION)
            {
                case JetEazy.OptionEnum.MAIN_X6:
                    if (!istrain)
                    {
                        try
                        {
                            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                            stopwatch.Restart();

                            //JetEazy.BasicSpace.JzToolsClass jzTools = new JetEazy.BasicSpace.JzToolsClass();
                            //Rectangle croprect = jzTools.SimpleRect(bmpFind.Size);
                            //croprect.Inflate(-40, -40);
                            //Bitmap myBmp001 = bmpFind.Clone(croprect, PixelFormat.Format24bppRgb);

                            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
                            Bitmap bmpgray = grayscale.Apply(bmpFind);

                            //string tmpStr = m_MvdCnnReader.DecodeStr(bmpgray);

                            //AForge.Imaging.Filters.Invert invert = new AForge.Imaging.Filters.Invert();
                            //Bitmap bmpinvert = invert.Apply(bmpgray);

                            AForge.Imaging.Filters.ResizeBilinear resizeBilinear = new AForge.Imaging.Filters.ResizeBilinear(bmpFind.Width * 1, bmpFind.Height * 1);
                            Bitmap bmpresizeBilinear = resizeBilinear.Apply(bmpgray);

                            string tmpStr = m_MvdCnnReader.DecodeStr(bmpresizeBilinear);

                            if (string.IsNullOrEmpty(tmpStr))
                            {
                                resizeBilinear = new AForge.Imaging.Filters.ResizeBilinear(bmpFind.Width * 2, bmpFind.Height * 2);
                                bmpresizeBilinear = resizeBilinear.Apply(bmpgray);

                                tmpStr = m_MvdCnnReader.DecodeStr(bmpresizeBilinear);
                            }

                            //if (string.IsNullOrEmpty(tmpStr))
                            //    bmpinvert.Save("d:\\testtest\\2d\\" + eName + "_" + ".png", ImageFormat.Png);

                            bmpgray.Dispose();
                            //bmpinvert.Dispose();
                            bmpresizeBilinear.Dispose();

                            stopwatch.Stop();
                            if (!string.IsNullOrEmpty(tmpStr))
                            {
                                barcodeStrRead = tmpStr;
                                myBarcode = tmpStr;// + " HIK用时:" + stopwatch.ElapsedMilliseconds.ToString() + " ms";
                            }
                            else
                            {
                                barcodeStrRead = tmpStr;
                                myBarcode = tmpStr;// + " HIK用时:" + stopwatch.ElapsedMilliseconds.ToString() + " ms" + m_MvdCnnReader.ErrMsg;

                            }

                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        m_MvdCnnReader.DecodeTrain();
                    }
                    break;
            }

#endif

#if OPT_USE_JET_BARCODE

            var timerStart = DateTime.Now.Ticks;

            if (string.IsNullOrEmpty(myBarcode) && !istrain)
            {
                //JetEazyBarcodeG.Interface.IBarcode IxBarcode = new JetEazyBarcodeG.Model.BarcodeGzx1Class();
                EzSegDMTX IxBarcode = new EzSegDMTX();
                IxBarcode.InputImage = bmpFind;
                switch (OCRMethod)
                {
                    case OCRMethodEnum.QRCODE:
                        IxBarcode.BarcodeFormat = ZXing.BarcodeFormat.QR_CODE;
                        break;
                    default:
                        IxBarcode.BarcodeFormat = ZXing.BarcodeFormat.DATA_MATRIX;
                        break;
                }
                int iret = IxBarcode.Run();
                
                var timerStop = DateTime.Now.Ticks;
                TimeSpan span = new TimeSpan(timerStop - timerStart);

                if (iret == 0)
                {
                    barcodeStrRead = IxBarcode.BarcodeStr;
                    myBarcode = IxBarcode.BarcodeStr;// + " Jet用时:" + span.Milliseconds.ToString() + " ms";
                    BarcodeGrade = "A";
                }
            }

#endif



            //if (string.IsNullOrEmpty(myBarcode) && !istrain)
            //{
            //    if (INI.chipUseAI)
            //    {
            //        try
            //        {
            //            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            //            stopwatch.Restart();

            //            EzSegDMTX ezSegDMTX = new EzSegDMTX();
            //            ezSegDMTX.InputImage = bmpFind;
            //            int iret = ezSegDMTX.Run();

            //            stopwatch.Stop();
            //            if (iret == 0)
            //            {
            //                barcodeStrRead = ezSegDMTX.BarcodeStr;
            //                myBarcode = ezSegDMTX.BarcodeStr;// + " Model用时:" + stopwatch.ElapsedMilliseconds.ToString() + " ms";
            //            }
            //        }
            //        catch
            //        {

            //        }
            //    }
            //}

            //span.Milliseconds
            if (istrain)
            {
                processstring += "2DBarcode Check OK." + Environment.NewLine;
                errorstring += "";
                reason = JetEazy.ReasonEnum.PASS;
                isgood = true;
            }
            else
            {
                if (string.IsNullOrEmpty(barcodeStrRead))
                {
                    errorstring += "2DBarcode ReadError." + Environment.NewLine; ;
                    processstring += "2DBarcode ReadError." + Environment.NewLine;
                    reason = JetEazy.ReasonEnum.NG;

                    isgood = false;
                }
                else
                {
                    if (barcodeStrRead != barcode && eCheckBarcode)
                    {
                        errorstring += "2DBarcode Mismatch." + Environment.NewLine; ;
                        processstring += "2DBarcode Mismatch." + Environment.NewLine;
                        reason = JetEazy.ReasonEnum.NG;
                        workstatus.AnalyzeProcedure = JetEazy.AnanlyzeProcedureEnum.CHECKMISBARCODE;
                        isgood = false;
                    }
                    else
                    {
                        processstring += "2DBarcode Check OK." + Environment.NewLine;
                        errorstring += "";
                        reason = JetEazy.ReasonEnum.PASS;
                        isgood = true;
                    }
                }
            }

            workstatus.SetWorkStatus(bmppattern, bmpFind, bmpErr, reason, errorstring, processstring, passInfo);


            if (!isgood)
            {
                if (INI.IsCollectErrorSmall)
                {
                    if (!System.IO.Directory.Exists(Universal.MainX6_Path))
                        System.IO.Directory.CreateDirectory(Universal.MainX6_Path);

                    bmpFind.Save(Universal.MainX6_Path + "\\Bar_" + eName + "_Input" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                }
            }

            if (istrain)
                TrainStatusCollection.Add(workstatus);
            else
                RunStatusCollection.Add(workstatus);

            return myBarcode;
        }
        public string DeCode2dGrade(string eName, bool eCheckBarcode, string barcode, bool istrain, Bitmap bmppattern, Bitmap bmpFind, PassInfoClass passInfo, out bool isgood, out string barcodeStrRead)
        {
            WorkStatusClass workstatus = new WorkStatusClass(JetEazy.AnanlyzeProcedureEnum.CHECKBARCODE);
            string processstring = "Start  DeCode2dGrade." + Environment.NewLine;
            string errorstring = "";
            JetEazy.ReasonEnum reason = JetEazy.ReasonEnum.PASS;

            string myBarcode = "";
            barcodeStrRead = string.Empty;
            BarcodeGrade = string.Empty;
            m_BarcodeReadStr = string.Empty;

#if OPT_USE_MVD_BARCODE

            if (!istrain)
            {
                try
                {
                    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                    stopwatch.Restart();

                    AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
                    Bitmap bmpgray = grayscale.Apply(bmpFind);

                    //string tmpStr = m_MvdCnnReader.DecodeStr(bmpgray);

                    //AForge.Imaging.Filters.Invert invert = new AForge.Imaging.Filters.Invert();
                    //Bitmap bmpinvert = invert.Apply(bmpgray);

                    AForge.Imaging.Filters.ResizeBilinear resizeBilinear = new AForge.Imaging.Filters.ResizeBilinear(bmpFind.Width * 1, bmpFind.Height * 1);
                    Bitmap bmpresizeBilinear = resizeBilinear.Apply(bmpgray);

                    string tmpStr = m_MvdCnnReader.DecodeGrade(bmpresizeBilinear);

                    if (string.IsNullOrEmpty(tmpStr))
                    {
                        resizeBilinear = new AForge.Imaging.Filters.ResizeBilinear(bmpFind.Width * 2, bmpFind.Height * 2);
                        bmpresizeBilinear = resizeBilinear.Apply(bmpgray);

                        tmpStr = m_MvdCnnReader.DecodeGrade(bmpresizeBilinear);
                    }

                    //if (string.IsNullOrEmpty(tmpStr))
                    //    bmpresizeBilinear.Save("d:\\testtest\\2d\\" + eName + "_" + ".png", ImageFormat.Png);


                    if (!string.IsNullOrEmpty(tmpStr))
                    {
                        barcodeStrRead = tmpStr;
                        myBarcode = tmpStr;
                        m_BarcodeReadStr = barcodeStrRead;
                        BarcodeGrade = m_MvdCnnReader.GetBarcodeItem.DecodeGrade;
                    }

                    if (string.IsNullOrEmpty(tmpStr))
                    {
                        JetEazyBarcodeG.Interface.IBarcode IxBarcode = new JetEazyBarcodeG.Model.BarcodeGzx1Class();
                        IxBarcode.InputImage = bmpFind;
                        int iret = IxBarcode.Run();
                        if (iret == 0)
                        {
                            barcodeStrRead = IxBarcode.BarcodeStr;
                            myBarcode = IxBarcode.BarcodeStr;
                            m_BarcodeReadStr = barcodeStrRead;
                            BarcodeGrade = "A";
                        }
                    }

                    bmpgray.Dispose();
                    //bmpinvert.Dispose();
                    bmpresizeBilinear.Dispose();

                    stopwatch.Stop();


                }
                catch
                {

                }
            }
            else
            {
                m_MvdCnnReader.DecodeTrain();
            }
#endif

#if OPT_USE_JET_BARCODE

            var timerStart = DateTime.Now.Ticks;

            if (string.IsNullOrEmpty(myBarcode) && !istrain)
            {
                //JetEazyBarcodeG.Interface.IBarcode IxBarcode = new JetEazyBarcodeG.Model.BarcodeGzx1Class();
                EzSegDMTX IxBarcode = new EzSegDMTX();
                IxBarcode.InputImage = bmpFind;
                switch (OCRMethod)
                {
                    case OCRMethodEnum.QRCODE:
                        IxBarcode.BarcodeFormat = ZXing.BarcodeFormat.QR_CODE;
                        break;
                    default:
                        IxBarcode.BarcodeFormat = ZXing.BarcodeFormat.DATA_MATRIX;
                        break;
                }
                int iret = IxBarcode.Run();
                
                var timerStop = DateTime.Now.Ticks;
                TimeSpan span = new TimeSpan(timerStop - timerStart);

                if (iret == 0)
                {
                    barcodeStrRead = IxBarcode.BarcodeStr;
                    myBarcode = IxBarcode.BarcodeStr;// + " Jet用时:" + span.Milliseconds.ToString() + " ms";
                    BarcodeGrade = "A";
                }
            }

            //var timerStart = DateTime.Now.Ticks;

            //JetEazyBarcodeG.Interface.IBarcode IxBarcode = new JetEazyBarcodeG.Model.BarcodeGzx1Class();
            //IxBarcode.InputImage = bmpFind;
            //int iret = IxBarcode.Run();

            //var timerStop = DateTime.Now.Ticks;
            //TimeSpan span = new TimeSpan(timerStop - timerStart);

            //if (iret == 0)
            //{
            //    barcodeStrRead = IxBarcode.BarcodeStr;
            //    myBarcode = IxBarcode.BarcodeStr;// + " Jet用时:" + span.Milliseconds.ToString() + " ms";
            //    BarcodeGrade = "A";
            //}

#endif



            //span.Milliseconds
            if (istrain)
            {
                processstring += "2DBarcode Check OK." + Environment.NewLine;
                errorstring += "";
                reason = JetEazy.ReasonEnum.PASS;
                isgood = true;
            }
            else
            {
                if (string.IsNullOrEmpty(barcodeStrRead))
                {
                    errorstring += "2DBarcode ReadError." + m_MvdCnnReader.ErrMsg + Environment.NewLine; ;
                    processstring += "2DBarcode ReadError." + m_MvdCnnReader.ErrMsg + Environment.NewLine;
                    reason = JetEazy.ReasonEnum.NG;

                    isgood = false;
                }
                else
                {
                    if (barcodeStrRead != barcode && eCheckBarcode)
                    {
                        errorstring += "2DBarcode Mismatch." + Environment.NewLine; ;
                        processstring += "2DBarcode Mismatch." + Environment.NewLine;
                        reason = JetEazy.ReasonEnum.NG;
                        workstatus.AnalyzeProcedure = JetEazy.AnanlyzeProcedureEnum.CHECKMISBARCODE;
                        isgood = false;
                    }
                    else
                    {
                        processstring += "2DBarcode Check OK." + Environment.NewLine;
                        errorstring += "";
                        reason = JetEazy.ReasonEnum.PASS;
                        isgood = true;
                    }
                }
            }

            workstatus.SetWorkStatus(bmppattern, bmpFind, bmpErr, reason, errorstring, processstring, passInfo);

            if (!isgood)
            {
                if (INI.IsCollectErrorSmall)
                {
                    if (!System.IO.Directory.Exists(Universal.MainX6_Path))
                        System.IO.Directory.CreateDirectory(Universal.MainX6_Path);

                    bmpFind.Save(Universal.MainX6_Path + "\\Bar2DGrade_" + eName + "_Input" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                }
            }

            if (istrain)
                TrainStatusCollection.Add(workstatus);
            else
                RunStatusCollection.Add(workstatus);

            return myBarcode;
        }

        public string FindOCR(string barcode, bool istrain, Bitmap bmppattern, Bitmap bmpFind, PassInfoClass passInfo, out bool isgood)
        {
            if (istrain || Universal.IsDebug)
            {
                isgood = true;
                return barcode;
            }
            string strSN = "";
            try
            {
                bmpErr = new Bitmap(bmpFind);
                WorkStatusClass workstatus = new WorkStatusClass(JetEazy.AnanlyzeProcedureEnum.CHECKOCR);
                string processstring = "Start  OCRCHRCK." + Environment.NewLine;
                string errorstring = "";
                JetEazy.ReasonEnum reason = JetEazy.ReasonEnum.PASS;

                Bitmap bmpfindTemp = new Bitmap(bmpFind);
                JzOCR.OPSpace.OCRItemClass[] ocritemlist = null;

                Stopwatch stopwatch = Stopwatch.StartNew();
                stopwatch.Start();
                JetEazy.LoggerClass.Instance.WriteLog("SN padd 开始 SN=" + barcode);


                string strSNAI = Universal.mOCRByPaddle.OCR(bmpfindTemp);
                strSN = strSNAI.Replace(Environment.NewLine, "");
                //Universal.OCRAISN = strSN;

                //if (barcode != strSN)
                //{
                //    JetEazy.LoggerClass.Instance.WriteLog("SN AI 开始 SN=" + barcode);
                //    strSNAI = Universal.OCRCollection.DecodePic(bmpfindTemp);
                //    strSN = strSNAI;
                //    //Universal.OCRAISN = strSNAI;

                //}

                if (barcode != strSN)
                {
                    string path = "D:\\LOA\\OcrAiSD\\";
                    if (!System.IO.Directory.Exists(path)) //若此文件夹不存在
                        System.IO.Directory.CreateDirectory(path); //创建此文件夹
                    bmpfindTemp.Save(path + barcode + "_" + strSN + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png");
                }
                string[] strs = { "8", "B", "S", "U", "Z" };
                foreach (string str in strs)
                {
                    if (barcode.IndexOf(str) > -1)
                    {
                        string path = "D:\\LOA\\OcrAiSave\\";
                        if (!System.IO.Directory.Exists(path)) //若此文件夹不存在
                            System.IO.Directory.CreateDirectory(path); //创建此文件夹
                        bmpfindTemp.Save(path + barcode + "_" + strSNAI + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png");

                        break;
                    }
                }

                JetEazy.LoggerClass.Instance.WriteLog("SN AI 完成 字符：" +
                    strSN + "结果：" +
                    (strSN == barcode ? "True" : "Fail") +
                    "用时:" + stopwatch.ElapsedMilliseconds + " ms");
                stopwatch.Restart();

                if (strSNAI != barcode)
                {
                    JzOCR.OPSpace.OCRClass MYOCR = null;
                    ocritemlist = new JzOCR.OPSpace.OCRItemClass[barcode.Length];
                    foreach (JzOCR.OPSpace.OCRClass ocr in Universal.OCRCollection.myDataList)
                    {
                        if (ocr.Name + "(" + ocr.No + ")" == OCRMappingMethod)
                        {
                            MYOCR = ocr;
                            break;
                        }
                    }

                    if (MYOCR != null)
                    {
                        //   barcode = "C02HL2F7DJWV";
                        //  bool[] defectlist = new bool[barcode.Length];
                        lock (MYOCR)
                        {
                            if (MYOCR != null)
                            {
                                MYOCR.strBarcode = barcode;
                                //if (INI.ISOCRBIG)
                                //{


                                //    if (OCRMappingMethod.IndexOf("BASALT") > -1 || OCRMappingMethod.IndexOf("BLUE") > -1)
                                //        strSN = MYOCR.OCRRUNLINE(bmpfindTemp, out bmpErr, out ocritemlist);
                                //    else
                                //        strSN = MYOCR.OCRRUNLINE(barcode, bmpfindTemp, out bmpErr, out ocritemlist);
                                //}
                                //else
                                {
                                    strSN = MYOCR.OCRRUNLINE(bmpfindTemp, out bmpErr, out ocritemlist);
                                }

                                JetEazy.LoggerClass.Instance.WriteLog("OCR SN 完成第一次 字符：" +
                                                                       strSN + "结果：" +
                                                                       (strSN == barcode ? "True" : "Fail") +
                                                                        "用时:" + stopwatch.ElapsedMilliseconds + " ms");
                                stopwatch.Restart();


                                if (MYOCR.isML && barcode != strSN && ocritemlist != null)
                                {
                                    if (strSN.Length == ocritemlist.Length)
                                    {
                                        for (int i = 0; i < strSN.Length; i++)
                                        {
                                            if (i < barcode.Length)
                                            {
                                                if (barcode[i] != strSN[i])
                                                {
                                                    string s = barcode[i].ToString();
                                                    MYOCR.AIRead(s, ocritemlist[i]);

                                                    if (ocritemlist[i] != null)
                                                    {
                                                        if (ocritemlist[i].strRelateName == s)
                                                        {
                                                            string strFilst = "", strLast = "";
                                                            if (i != 0)
                                                                strFilst = strSN.Substring(0, i);
                                                            if (i != strSN.Length - 1)
                                                                strLast = strSN.Substring(i + 1, strSN.Length - i - 1);

                                                            strSN = strFilst + s + strLast;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                //如果AI读出来的 与 OCR读出来的都不匹配,测看看他们
                                if (barcode != strSN)
                                {
                                    bool isCheckOk = true;
                                    for (int i = 0; i < barcode.Length; i++)
                                    {
                                        bool istestok = false;
                                        if (strSN.Length > i)
                                        {
                                            if (barcode[i] == strSN[i])
                                                istestok = true;
                                        }
                                        if (strSNAI.Length > i)
                                        {
                                            if (barcode[i] == strSNAI[i])
                                                istestok = true;
                                        }

                                        if (!istestok)
                                            isCheckOk = false;
                                    }

                                    if (isCheckOk)
                                        strSN = barcode;
                                }

                                if (barcode != strSN && false)
                                {
                                    if (strSN.Length > barcode.Length)
                                    {
                                        string strTemp = strSN.Replace("?", "");
                                        if (strTemp == barcode)
                                            strSN = barcode;
                                    }
                                    if (barcode != strSN)
                                    {
                                        //if (INI.ISOCRBIG)
                                        //    strSN = MYOCR.OCRRUNLINEAURO(barcode, bmpfindTemp, ref bmpErr, ref ocritemlist);
                                        //else
                                        {
                                            if (OCRMappingMethod.IndexOf("BASALT") < 0 && OCRMappingMethod.IndexOf("BLUE") < 0)
                                                strSN = MYOCR.OCRRUNLINEAURO(barcode, bmpfindTemp, ref bmpErr, ref ocritemlist);
                                        }
                                    }

                                    JetEazy.LoggerClass.Instance.WriteLog("OCR SN 完成第二次 字符：" +
                                                                           strSN + "结果：" +
                                                                           (strSN == barcode ? "True" : "Fail") +
                                                                            "用时:" + stopwatch.ElapsedMilliseconds + " ms");
                                    stopwatch.Restart();

                                    if (MYOCR.isML && barcode != strSN && ocritemlist != null)
                                    {
                                        if (strSN.Length == ocritemlist.Length)
                                        {
                                            for (int i = 0; i < strSN.Length; i++)
                                            {
                                                if (i < barcode.Length)
                                                {
                                                    if (barcode[i] != strSN[i])
                                                    {
                                                        string s = barcode[i].ToString();
                                                        MYOCR.AIRead(s, ocritemlist[i]);

                                                        if (ocritemlist[i] != null)
                                                        {
                                                            if (ocritemlist[i].strRelateName == s)
                                                            {
                                                                string strFilst = "", strLast = "";
                                                                if (i != 0)
                                                                    strFilst = strSN.Substring(0, i);
                                                                if (i != strSN.Length - 1)
                                                                    strLast = strSN.Substring(i + 1, strSN.Length - i - 1);

                                                                strSN = strFilst + s + strLast;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    //如果AI读出来的 与 OCR读出来的都不匹配,测看看他们
                                    if (barcode != strSN)
                                    {
                                        bool isCheckOk = true;
                                        for (int i = 0; i < barcode.Length; i++)
                                        {
                                            bool istestok = false;
                                            if (strSN.Length > i)
                                            {
                                                if (barcode[i] == strSN[i])
                                                    istestok = true;
                                            }
                                            if (strSNAI.Length > i)
                                            {
                                                if (barcode[i] == strSNAI[i])
                                                    istestok = true;
                                            }

                                            if (!istestok)
                                                isCheckOk = false;
                                        }

                                        if (isCheckOk)
                                            strSN = barcode;
                                    }
                                }
                                if (barcode != strSN && false)
                                {
                                    if (strSN.Length > barcode.Length)
                                    {
                                        string strTemp = strSN.Replace("?", "");
                                        if (strTemp == barcode)
                                            strSN = barcode;

                                    }
                                    if (barcode != strSN)
                                    {
                                        //if (INI.ISOCRBIG)
                                        //    strSN = MYOCR.OCRRUNLINEAURO(barcode, bmpfindTemp, ref bmpErr, ref ocritemlist);
                                        //else
                                        {
                                            if (OCRMappingMethod.IndexOf("BASALT") < 0 && OCRMappingMethod.IndexOf("BLUE") < 0)
                                                strSN = MYOCR.OCRRUNLINEAURO(barcode, bmpfindTemp, ref bmpErr, ref ocritemlist);
                                        }

                                        JetEazy.LoggerClass.Instance.WriteLog("OCR SN 完成第三次 字符：" +
                                                                            strSN + "结果：" +
                                                                            (strSN == barcode ? "True" : "Fail") +
                                                                            "用时:" + stopwatch.ElapsedMilliseconds + " ms");
                                        stopwatch.Restart();

                                        if (MYOCR.isML && barcode != strSN && ocritemlist != null)
                                        {
                                            if (strSN.Length == ocritemlist.Length)
                                            {
                                                for (int i = 0; i < strSN.Length; i++)
                                                {
                                                    if (i < barcode.Length)
                                                    {
                                                        if (barcode[i] != strSN[i])
                                                        {
                                                            string s = barcode[i].ToString();
                                                            MYOCR.AIRead(s, ocritemlist[i]);

                                                            if (ocritemlist[i] != null)
                                                            {
                                                                if (ocritemlist[i].strRelateName == s)
                                                                {
                                                                    string strFilst = "", strLast = "";
                                                                    if (i != 0)
                                                                        strFilst = strSN.Substring(0, i);
                                                                    if (i != strSN.Length - 1)
                                                                        strLast = strSN.Substring(i + 1, strSN.Length - i - 1);

                                                                    strSN = strFilst + s + strLast;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if (barcode != strSN)
                                        {
                                            if (strSN.Length > barcode.Length)
                                            {
                                                string strTemp = strSN.Replace("?", "");
                                                if (strTemp == barcode)
                                                    strSN = barcode;

                                            }
                                        }
                                        //如果AI读出来的 与 OCR读出来的都不匹配,测看看他们
                                        if (barcode != strSN)
                                        {
                                            bool isCheckOk = true;
                                            for (int i = 0; i < barcode.Length; i++)
                                            {
                                                bool istestok = false;
                                                if (strSN.Length > i)
                                                {
                                                    if (barcode[i] == strSN[i])
                                                        istestok = true;
                                                }
                                                if (strSNAI.Length > i)
                                                {
                                                    if (barcode[i] == strSNAI[i])
                                                        istestok = true;
                                                }

                                                if (!istestok)
                                                    isCheckOk = false;
                                            }

                                            if (isCheckOk)
                                                strSN = barcode;
                                        }
                                    }
                                }
                                //         bmpErr.Save("d://BMPERR.PNG");
                            }
                            else
                                strSN = "";

                            //bmpfindTemp.Save("D:\\find.png");
                            //bmpErr.Save("D:\\err.png");
                        }
                        bmpfindTemp.Dispose();
                    }
                }

                if (barcode != strSN)
                {
                    errorstring += "SN Mismatch." + Environment.NewLine;
                    processstring += "SN Mismatch." + Environment.NewLine;
                    reason = JetEazy.ReasonEnum.NG;

                    Universal.R3UI.isSNResult = false;
                }
                else
                {
                    processstring += "SN Check OK." + Environment.NewLine;
                    errorstring += "";
                    reason = JetEazy.ReasonEnum.PASS;
                }

                bool isok = false;
                if (ocritemlist != null)
                {
                    foreach (JzOCR.OPSpace.OCRItemClass ocritem in ocritemlist)
                    {
                        if (ocritem != null && !ocritem.isDefect)
                        {
                            isok = true;
                            break;
                        }
                    }
                }
                else if (strSNAI != barcode)
                    isok = true;

                if (isok && INI.ISCHECKSNDEFECT)
                {
                    errorstring += "SN Defect." + Environment.NewLine;
                    processstring += "SN Defect." + Environment.NewLine;
                    reason = JetEazy.ReasonEnum.NG;

                    Universal.R3UI.isSNResult = false;
                }

                if (INI.ISSAVEOCRIMAGE)
                    bmpFind.Save(Universal.OCRIMAGEPATH + barcode + "_" + OCRMappingMethod + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                workstatus.SetWorkStatus(bmppattern, bmpFind, bmpErr, reason, errorstring, processstring, passInfo);
                if (!istrain)
                {
                    if (ocritemlist != null && ocritemlist.Length == barcode.Length)
                    {
                        Bitmap bmpDefect = new Bitmap(bmpErr.Width, bmpErr.Height * 3);
                        Graphics g = Graphics.FromImage(bmpDefect);
                        g.DrawImage(bmpFind, new PointF(0, 0));
                        Font font = new Font("宋体", 30f);

                        for (int i = 0; i < ocritemlist.Length; i++)
                        {

                            JzOCR.OPSpace.OCRItemClass ocritem = ocritemlist[i];
                            if (ocritem == null)
                                continue;

                            Rectangle rect = new Rectangle(ocritem.rect.X, bmpErr.Height + ocritem.rect.Y, ocritem.rect.Width, ocritem.rect.Height);
                            if (ocritem.bmpDifference != null)
                                g.DrawImage(ocritem.bmpDifference, rect);
                            else
                                g.DrawImage(ocritem.bmpItemTo, rect);

                            SizeF size = g.MeasureString(ocritem.strRelateName, font);
                            RectangleF rect2 = new RectangleF(new PointF(ocritem.rect.X, bmpErr.Height * 2 + ocritem.rect.Y), size);
                            if (barcode[i].ToString() == ocritem.strRelateName)
                            {
                                g.FillRectangle(Brushes.White, rect2);
                                g.DrawString(ocritem.strRelateName, font, Brushes.Lime, new PointF(rect2.X, rect2.Y));
                            }
                            else
                            {
                                g.FillRectangle(Brushes.Red, rect2);
                                g.DrawString(barcode[i].ToString(), font, Brushes.Black, new PointF(rect2.X, rect2.Y));
                            }
                            //else if (!ocritem.isDefect && barcode[i].ToString() == ocritem.strRelateName)
                            //{
                            //    g.FillRectangle(Brushes.Blue, rect2);
                            //    g.DrawString(ocritem.strRelateName, font, Brushes.Red, new PointF(rect2.X, rect2.Y));
                            //}
                            //else
                            //{
                            //    g.FillRectangle(Brushes.Red, rect2);
                            //    g.DrawString(ocritem.strRelateName, font, Brushes.Black, new PointF(rect2.X, rect2.Y));
                            //}


                        }
                        bmpDefect = new Bitmap(bmpDefect, new Size(bmpDefect.Size.Width * 2, bmpDefect.Height * 2));
                        if (reason == JetEazy.ReasonEnum.NG)
                            Universal.ALBCollection.AlbumWork.CPD.bmpOCRCheckErr = bmpDefect;
                        else if (reason == JetEazy.ReasonEnum.PASS)
                            Universal.ALBCollection.AlbumWork.CPD.bmpOCRCheckErr = null;
                    }
                    else
                    {
                        if (bmpErr != null && bmpErr.Size.Width > 1 && bmpErr.Size.Height > 1)
                            bmpErr = new Bitmap(bmpErr, new Size(bmpErr.Size.Width * 2, bmpErr.Height * 2));
                        else
                            bmpErr = new Bitmap(bmpFind);

                        if (reason == JetEazy.ReasonEnum.NG)
                            Universal.ALBCollection.AlbumNow.CPD.bmpOCRCheckErr = bmpErr;
                        else if (reason == JetEazy.ReasonEnum.PASS)
                            Universal.ALBCollection.AlbumNow.CPD.bmpOCRCheckErr = null;
                    }
                    //Bitmap bmpresult = Universal.ALBCollection.AlbumWork.CPD.bmpRUNVIEW;
                    //Point lo = new Point();
                    //lo.X = 20;
                    //lo.Y = bmpresult.Height - bmpErr.Height - 20;
                    //Graphics g = Graphics.FromImage(bmpresult);
                    //g.DrawImage(bmpErr, lo);
                    //g.Dispose();

                    //bmpresult.Save("D://test.png");
                }

                if (!istrain)
                {
                    if (reason == JetEazy.ReasonEnum.NG)
                        isgood = false;
                    else
                        isgood = true;
                }
                else
                    isgood = true;

                if (istrain)
                    TrainStatusCollection.Add(workstatus);
                else
                    RunStatusCollection.Add(workstatus);

                JetEazy.LoggerClass.Instance.WriteLog("SN 完成 字符：" +
                                                      strSN + "结果：" +
                                                      (strSN == barcode ? "True" : "Fail") +
                                                      "用时:" + stopwatch.ElapsedMilliseconds + " ms");
            }
            catch (Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteLog("OCR ERR 闪退 :" + ex.ToString());
                isgood = false;

                //WorkStatusClass workstatus = new WorkStatusClass(JetEazy.AnanlyzeProcedureEnum.CHECKOCR);
                //string processstring = "Start  OCRCHRCK." + Environment.NewLine;
                //string errorstring = "";
                //errorstring += "SN Defect." + Environment.NewLine;
                //processstring += "SN Defect." + Environment.NewLine;
                //JetEazy.ReasonEnum reason = JetEazy.ReasonEnum.NG;

                //workstatus.SetWorkStatus(bmppattern, bmpFind, bmpErr, reason, errorstring, processstring, passInfo);

                //if (istrain)
                //    TrainStatusCollection.Add(workstatus);
                //else
                //    RunStatusCollection.Add(workstatus);
            }
            return strSN;
        }

        /// <summary>
        /// 检查月份
        /// </summary>
        /// <param name="istrain"></param>
        /// <param name="bmppattern"></param>
        /// <param name="bmpFind"></param>
        /// <param name="passInfo"></param>
        /// <param name="isMoth">是否是月份（否则是年份）</param>
        /// <param name="isgood"></param>
        /// <returns></returns>
        public string FindOCR_YEARAndMONTH(bool istrain, Bitmap bmppattern, Bitmap bmpFind, PassInfoClass passInfo, bool isMonth, out bool isgood, string strData = "")
        {
            if (istrain)
            {
                isgood = true;
                return DateTime.Now.Month.ToString("00");
            }
            string strSN = "";
            try
            {
                bmpErr = new Bitmap(bmpFind);
                WorkStatusClass workstatus = new WorkStatusClass(isMonth ? JetEazy.AnanlyzeProcedureEnum.MONTH : JetEazy.AnanlyzeProcedureEnum.YEAR);
                string processstring = "Start  CHRCKDATE." + Environment.NewLine;
                string errorstring = "";
                JetEazy.ReasonEnum reason = JetEazy.ReasonEnum.PASS;

                Bitmap bmpfindTemp = new Bitmap(bmpFind);
                JzOCR.OPSpace.OCRClass MYOCR = null;
                foreach (JzOCR.OPSpace.OCRClass ocr in Universal.OCRCollection.myDataList)
                {
                    if (ocr.Name + "(" + ocr.No + ")" == OCRMappingMethod)
                    {
                        MYOCR = ocr;
                        break;
                    }
                }

                Stopwatch stopwatch = Stopwatch.StartNew();
                stopwatch.Start();
                JetEazy.LoggerClass.Instance.WriteLog("SN padd 开始 SN=" + strData);


                string strSNAI = Universal.mOCRByPaddle.OCR(bmpfindTemp);
                strSN = strSNAI.Replace(Environment.NewLine, "");

                //if (strData != strSN)
                //{
                //    JetEazy.LoggerClass.Instance.WriteLog("SN AI 开始 SN=" + strData);
                //    strSNAI = Universal.OCRCollection.DecodePic(bmpfindTemp);
                //    strSN = strSNAI;
                //}

                if (strData != strSN)
                {
                    string path = "D:\\LOA\\OcrAiSD\\";
                    if (!System.IO.Directory.Exists(path)) //若此文件夹不存在
                        System.IO.Directory.CreateDirectory(path); //创建此文件夹
                    bmpfindTemp.Save(path + strData + "_" + strSN + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png");
                }
                string[] strs = { "8", "B", "S", "U", "Z" };
                foreach (string str in strs)
                {
                    if (strData.IndexOf(str) > -1)
                    {
                        string path = "D:\\LOA\\OcrAiSave\\";
                        if (!System.IO.Directory.Exists(path)) //若此文件夹不存在
                            System.IO.Directory.CreateDirectory(path); //创建此文件夹
                        bmpfindTemp.Save(path + strData + "_" + strSNAI + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png");

                        break;
                    }
                }

                JetEazy.LoggerClass.Instance.WriteLog("OCR AI 完成 用时:" + stopwatch.ElapsedMilliseconds + " ms" + (isMonth ? " 找月份" : " 找年份"));
                stopwatch.Restart();

                bool isCheckOK = false;
                //月份 
                if (isMonth && strSN != "")
                {
                    strSN = strSN.Trim();
                    if (strSN == "DFC")
                        strSN = "DEC";
                    if (strSN == "D5C")
                        strSN = "DEC";
                    if (strSN == "SFP")
                        strSN = "SEP";
                    if (strSN == "S5P")
                        strSN = "SEP";
                    string[] MMM = { "JAN", " FEB", " MAR", "APR", " MAY", " JUN", " JUL", "AUG", " SEP", "OCT", "NOV", "DEC" };
                    foreach (string str in MMM)
                    {
                        int INDEX = strSN.IndexOf(str);
                        if (INDEX > -1)
                        {
                            strSN = str;
                            isCheckOK = true;
                            break;
                        }
                    }
                }
                else if (!isCheckOK && strSN != "")
                {
                    if (strSN.IndexOf("2023") > -1)
                    {
                        strSN = "2023";
                        isCheckOK = true;
                    }
                    if (strSN.IndexOf("2024") > -1)
                    {
                        strSN = "2024";
                        isCheckOK = true;
                    }
                    if (strSN.IndexOf("2025") > -1)
                    {
                        strSN = "2025";
                        isCheckOK = true;
                    }
                    if (strSN.IndexOf("2026") > -1)
                    {
                        strSN = "2026";
                        isCheckOK = true;
                    }
                    if (strSN.IndexOf("2027") > -1)
                    {
                        strSN = "2027";
                        isCheckOK = true;
                    }
                }
                if (!isCheckOK && MYOCR != null)
                {
                    lock (MYOCR)
                    {
                        MYOCR.strBarcode = strData;
                        if (MYOCR != null)
                        {
                            JetEazy.LoggerClass.Instance.WriteLog("OCR Find 开始" + (isMonth ? " 找月份" : " 找年份"));
                            bool isDef = false;
                            strSN = MYOCR.OCRRUNLINE(bmpfindTemp, ref bmpErr, ref isDef);

                            //if(strSN != strData)
                            //{
                            //    JetEazy.LoggerClass.Instance.WriteLog("OCR Find 第一次完成 用时：" + stopwatch.ElapsedMilliseconds + " ms");
                            //    stopwatch.Restart();
                            //    JzOCR.OPSpace.OCRItemClass[] ocritemlist = null;
                            //    strSN = MYOCR.OCRRUNLINEAURO(strData,bmpfindTemp, ref bmpErr, ref ocritemlist);
                            //}

                            JetEazy.LoggerClass.Instance.WriteLog("OCR Find 完成:" + stopwatch.ElapsedMilliseconds + " ms" + (isMonth ? " 找月份" : " 找年份"));
                            stopwatch.Restart();
                        }
                        else
                            strSN = "";
                        //bmpfindTemp.Save("D:\\find.png");
                        //bmpErr.Save("D:\\err.png");
                    }
                    bmpfindTemp.Dispose();

                    if (isMonth && strSN != "")
                    {
                        strSN = strSN.Trim();
                        if (strSN == "DFC")
                            strSN = "DEC";
                        if (strSN == "D5C")
                            strSN = "DEC";
                        if (strSN == "SFP")
                            strSN = "SEP";
                        if (strSN == "S5P")
                            strSN = "SEP";
                        string[] MMM = { "JAN", " FEB", " MAR", "APR", " MAY", " JUN", " JUL", "AUG", " SEP", "OCT", "NOV", "DEC" };
                        foreach (string str in MMM)
                        {
                            int INDEX = strSN.IndexOf(str);
                            if (INDEX > -1)
                            {
                                strSN = str;
                                isCheckOK = true;
                                break;
                            }
                        }
                    }
                    else if (!isCheckOK && strSN != "")
                    {
                        if (strSN.IndexOf("2023") > -1)
                        {
                            strSN = "2023";
                            isCheckOK = true;
                        }
                        if (strSN.IndexOf("2024") > -1)
                        {
                            strSN = "2024";
                            isCheckOK = true;
                        }
                        if (strSN.IndexOf("2025") > -1)
                        {
                            strSN = "2025";
                            isCheckOK = true;
                        }
                        if (strSN.IndexOf("2026") > -1)
                        {
                            strSN = "2026";
                            isCheckOK = true;
                        }
                        if (strSN.IndexOf("2027") > -1)
                        {
                            strSN = "2027";
                            isCheckOK = true;
                        }
                    }
                }
                if (isMonth)
                {
                    string[] MMM = { "JAN", " FEB", " MAR", "APR", " MAY", " JUN", " JUL", "AUG", " SEP", "OCT", "NOV", "DEC" };

                    int iMonth = DateTime.Now.Month;

                    string M = MMM[iMonth - 1];

                    string strFastMonth = "FAIL";

                    bool isok = true;
                    if (strData != "")
                    {
                        try
                        {
                            int imonth = int.Parse(strData);
                            M = MMM[imonth - 1];
                            isok = false;
                        }
                        catch { }
                    }

                    if (isok)
                    {
                        int iDay = DateTime.Now.Day;
                        int iHour = DateTime.Now.Hour;
                        int iMinute = DateTime.Now.Minute;
                        if (iDay == 1 && iHour == 0 && iMinute < 30)
                        {
                            if (iMonth == 1)
                                strFastMonth = MMM[11];
                            else
                                strFastMonth = MMM[iMonth - 2];
                        }
                    }


                    if (M == strSN || strFastMonth == strSN)
                    {
                        processstring += "月份检查OK." + Environment.NewLine;
                        errorstring += "";
                        reason = JetEazy.ReasonEnum.PASS;
                    }
                    else
                    {
                        errorstring += "月份错误 ." + Environment.NewLine;
                        processstring += "月份错误." + Environment.NewLine;
                        reason = JetEazy.ReasonEnum.NG;

                    }
                }
                else
                {
                    int iYear = DateTime.Now.Year;

                    string strFastMonth = "FAIL";

                    bool isok = true;
                    if (strData != "")
                    {
                        try
                        {
                            iYear = int.Parse(strData);
                            isok = false;
                        }
                        catch { }
                    }

                    if (isok)
                    {
                        int iMonth = DateTime.Now.Month;
                        int iDay = DateTime.Now.Day;
                        int iHour = DateTime.Now.Hour;
                        int iMinute = DateTime.Now.Minute;
                        if (iMonth == 1 && iDay == 1 && iHour == 0 && iMinute < 30)
                            strFastMonth = (iYear - 1).ToString("0000");
                    }

                    if (iYear.ToString("0000") == strSN || strFastMonth == strSN)
                    {
                        processstring += "年份检查OK." + Environment.NewLine;
                        errorstring += "";
                        reason = JetEazy.ReasonEnum.PASS;
                    }
                    else
                    {
                        errorstring += "年份错误 ." + Environment.NewLine;
                        processstring += "年份错误." + Environment.NewLine;
                        reason = JetEazy.ReasonEnum.NG;

                    }
                }
                workstatus.SetWorkStatus(bmppattern, bmpFind, bmpErr, reason, errorstring, processstring, passInfo);

                if (!istrain)
                {
                    if (reason == JetEazy.ReasonEnum.NG)
                        isgood = false;
                    else
                        isgood = true;
                }
                else
                    isgood = true;

                if (istrain)
                    TrainStatusCollection.Add(workstatus);
                else
                    RunStatusCollection.Add(workstatus);
                JetEazy.LoggerClass.Instance.WriteLog("完成 用时:" + stopwatch.ElapsedMilliseconds + " ms" + (isMonth ? " 找月份" : " 找年份"));

            }
            catch (Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteLog("OCR ERR 闪退 :" + ex.ToString());
                isgood = false;

            }
            return strSN;
        }
        public string FindOCR_WEEK(bool istrain, Bitmap bmppattern, Bitmap bmpFind, PassInfoClass passInfo, bool isMonth, out bool isgood, string strData = "")
        {
            if (istrain)
            {
                isgood = true;
                return DateTime.Now.Month.ToString("00");
            }
            string strSN = "";
            try
            {
                bmpErr = new Bitmap(bmpFind);
                WorkStatusClass workstatus = new WorkStatusClass(JetEazy.AnanlyzeProcedureEnum.WEEK);
                string processstring = "Start  CHRCKDATE." + Environment.NewLine;
                string errorstring = "";
                JetEazy.ReasonEnum reason = JetEazy.ReasonEnum.PASS;

                Bitmap bmpfindTemp = new Bitmap(bmpFind);
                JzOCR.OPSpace.OCRClass MYOCR = null;
                foreach (JzOCR.OPSpace.OCRClass ocr in Universal.OCRCollection.myDataList)
                {
                    if (ocr.Name + "(" + ocr.No + ")" == OCRMappingMethod)
                    {
                        MYOCR = ocr;
                        break;
                    }
                }

                Stopwatch stopwatch = Stopwatch.StartNew();
                stopwatch.Start();
                JetEazy.LoggerClass.Instance.WriteLog("SN padd 开始 SN=" + strData);

                string strSNAI = Universal.mOCRByPaddle.OCR(bmpfindTemp);
                strSN = strSNAI.Replace(Environment.NewLine, "");

                //if (strData != strSN)
                //{
                //    JetEazy.LoggerClass.Instance.WriteLog("SN AI 开始 SN=" + strData);
                //    strSNAI = Universal.OCRCollection.DecodePic(bmpfindTemp);
                //    strSN = strSNAI;
                //}

                if (strData != strSN)
                {
                    string path = "D:\\LOA\\OcrAiSD\\";
                    if (!System.IO.Directory.Exists(path)) //若此文件夹不存在
                        System.IO.Directory.CreateDirectory(path); //创建此文件夹
                    bmpfindTemp.Save(path + strData + "_" + strSN + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png");
                }

                JetEazy.LoggerClass.Instance.WriteLog("OCR AI 完成 用时:" + stopwatch.ElapsedMilliseconds + " ms 找周");
                //stopwatch.Restart();

                int iweek = 1;
                int.TryParse(strSN, out iweek);

                if (strData == strSN || int.Parse(strData) == iweek)
                {
                    processstring += "周检查OK." + Environment.NewLine;
                    errorstring += "";
                    reason = JetEazy.ReasonEnum.PASS;
                }
                else
                {
                    errorstring += "周错误 ." + Environment.NewLine;
                    processstring += "周错误." + Environment.NewLine;
                    reason = JetEazy.ReasonEnum.NG;
                }

                workstatus.SetWorkStatus(bmppattern, bmpFind, bmpErr, reason, errorstring, processstring, passInfo);

                if (!istrain)
                {
                    if (reason == JetEazy.ReasonEnum.NG)
                        isgood = false;
                    else
                        isgood = true;
                }
                else
                    isgood = true;

                if (istrain)
                    TrainStatusCollection.Add(workstatus);
                else
                    RunStatusCollection.Add(workstatus);
                JetEazy.LoggerClass.Instance.WriteLog("完成 用时:" + stopwatch.ElapsedMilliseconds + " ms 找周");

            }
            catch (Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteLog("OCR ERR 闪退 :" + ex.ToString());
                isgood = false;

            }
            return strSN;
        }

        /// <summary>
        /// 检查镭雕变动字符
        /// </summary>
        /// <param name="istrain"></param>
        /// <param name="bmppattern"></param>
        /// <param name="bmpFind"></param>
        /// <param name="passInfo"></param>
        /// <param name="isgood"></param>
        /// <returns></returns>
        public string FindOCR_Biandong(string strBar, bool istrain, Bitmap bmppattern, Bitmap bmpFind, PassInfoClass passInfo, out bool isgood)
        {
            if (istrain)
            {
                isgood = true;
                return DateTime.Now.Month.ToString("00");
            }
            string strSN = "";
            try
            {
                bmpErr = new Bitmap(bmpFind);
                WorkStatusClass workstatus = new WorkStatusClass(JetEazy.AnanlyzeProcedureEnum.LASER);
                string processstring = "Start  CHRCKDATE." + Environment.NewLine;
                string errorstring = "";
                JetEazy.ReasonEnum reason = JetEazy.ReasonEnum.PASS;

                Bitmap bmpfindTemp = new Bitmap(bmpFind);
                JzOCR.OPSpace.OCRClass MYOCR = null;
                foreach (JzOCR.OPSpace.OCRClass ocr in Universal.OCRCollection.myDataList)
                {
                    if (ocr.Name + "(" + ocr.No + ")" == OCRMappingMethod)
                    {
                        MYOCR = ocr;
                        break;
                    }
                }
                Stopwatch stopwatch = Stopwatch.StartNew();
                stopwatch.Start();
                JetEazy.LoggerClass.Instance.WriteLog("镭雕变动字符 OCR padd 开始");


                string strSNAI = Universal.mOCRByPaddle.OCR(bmpfindTemp);
                strSN = strSNAI.Replace(Environment.NewLine, "");

                //if (strBar != strSN)
                //{
                //    JetEazy.LoggerClass.Instance.WriteLog("镭雕变动字符 OCR AI 开始");
                //    strSNAI = Universal.OCRCollection.DecodePic(bmpfindTemp);
                //    strSN = strSNAI;
                //}

                if (strBar != strSN)
                {
                    string path = "D:\\LOA\\OcrAiSD\\";
                    if (!System.IO.Directory.Exists(path)) //若此文件夹不存在
                        System.IO.Directory.CreateDirectory(path); //创建此文件夹
                    bmpfindTemp.Save(path + strBar + "_" + strSN + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png");
                }

                string[] strs = { "8", "B", "S", "U", "Z" };
                foreach (string str in strs)
                {
                    if (strBar.IndexOf(str) > -1)
                    {
                        string path = "D:\\LOA\\OcrAiSave\\";
                        if (!System.IO.Directory.Exists(path)) //若此文件夹不存在
                            System.IO.Directory.CreateDirectory(path); //创建此文件夹
                        bmpfindTemp.Save(path + strBar + "_" + strSNAI + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png");

                        break;
                    }
                }


                JetEazy.LoggerClass.Instance.WriteLog("镭雕变动字符 OCR AI 完成" + stopwatch.ElapsedMilliseconds + " ms");
                stopwatch.Restart();
                if (strBar != strSN && MYOCR != null)
                {
                    lock (MYOCR)
                    {
                        MYOCR.strBarcode = strBar;
                        if (MYOCR != null)
                        {
                            JetEazy.LoggerClass.Instance.WriteLog("镭雕变动字符 OCR Find 开始");
                            bool isDef = false;
                            strSN = MYOCR.OCRRUNLINE(bmpfindTemp, ref bmpErr, ref isDef);

                            //if (strSN != strBar)
                            //{
                            //    JetEazy.LoggerClass.Instance.WriteLog("镭雕变动字符 OCR Find 第一次完成 用时：" + stopwatch.ElapsedMilliseconds + " ms");
                            //    stopwatch.Restart();
                            //    JzOCR.OPSpace.OCRItemClass[] ocritemlist = null;
                            //    strSN = MYOCR.OCRRUNLINEAURO(strBar, bmpfindTemp, ref bmpErr, ref ocritemlist);

                            //}

                            JetEazy.LoggerClass.Instance.WriteLog("镭雕变动字符 OCR Find 完成 用时：" + stopwatch.ElapsedMilliseconds + " ms");
                            stopwatch.Restart();
                        }
                        else
                            strSN = "";
                        //bmpfindTemp.Save("D:\\find.png");
                        //bmpErr.Save("D:\\err.png");
                    }
                    bmpfindTemp.Dispose();
                }
                if (strBar == strSN)
                {
                    processstring += "镭雕检查OK." + Environment.NewLine;
                    errorstring += "";
                    reason = JetEazy.ReasonEnum.PASS;
                }
                else
                {
                    errorstring += "镭雕错误 ." + Environment.NewLine;
                    processstring += "镭雕错误." + Environment.NewLine;
                    reason = JetEazy.ReasonEnum.NG;
                }

                Bitmap bmpPatternTemp = new Bitmap(bmppattern.Width, bmppattern.Height);
                Graphics gg = Graphics.FromImage(bmpPatternTemp);
                gg.FillRectangle(Brushes.Black, 0, 0, bmppattern.Width, bmppattern.Height);
                gg.DrawString(strBar, new Font("宋体", 40), Brushes.White, new Point(0, 0));
                gg.Dispose();

                gg = Graphics.FromImage(bmpErr);
                gg.FillRectangle(Brushes.Black, 0, 0, bmpErr.Width, bmpErr.Height);
                gg.DrawString(strSN, new Font("宋体", 40), Brushes.White, new Point(0, 0));
                gg.Dispose();

                workstatus.SetWorkStatus(bmpPatternTemp, bmpFind, bmpErr, reason, errorstring, processstring, passInfo);
                if (!istrain)
                {
                    if (reason == JetEazy.ReasonEnum.NG)
                        isgood = false;
                    else
                        isgood = true;
                }
                else
                    isgood = true;

                if (istrain)
                    TrainStatusCollection.Add(workstatus);
                else
                    RunStatusCollection.Add(workstatus);

                JetEazy.LoggerClass.Instance.WriteLog("镭雕变动字符 完成 用时：" + stopwatch.ElapsedMilliseconds + " ms");

            }
            catch (Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteLog("OCR ERR 闪退 :" + ex.ToString());
                isgood = false;
            }
            return strSN;
        }

        public void Suicide()
        {
            TrainStatusCollection.Clear();
            RunStatusCollection.Clear();
        }

        /// <summary>
        /// 在做大量運算前要清除的相關資料
        /// </summary>
        public void ResetTrainStatus()
        {
            TrainStatusCollection.Clear();
            BarcodeGrade = string.Empty;
            m_BarcodeReadStr = string.Empty;
        }
        /// <summary>
        /// 在做大量運算前要清除的相關資料
        /// </summary>
        public void ResetRunStatus()
        {
            RunStatusCollection.Clear();
            BarcodeGrade = string.Empty;
            m_BarcodeReadStr = string.Empty;
        }
        /// <summary>
        /// 將產生出來的過程寫出去
        /// </summary>
        /// <param name="runstatuscollection"></param>
        public void FillRunStatus(WorkStatusCollectionClass runstatuscollection)
        {
            foreach (WorkStatusClass runstatus in RunStatusCollection.WorkStatusList)
            {
                runstatuscollection.Add(runstatus);
            }
        }
        /// <summary>
        /// 將產生出來的過程寫出去
        /// </summary>
        /// <param name="runstatuscollection"></param>
        public void FillTrainStatus(WorkStatusCollectionClass trainstatuscollection)
        {
            foreach (WorkStatusClass trainstatus in TrainStatusCollection.WorkStatusList)
            {
                trainstatuscollection.Add(trainstatus);
            }
        }
        public void AddTrainLogString(string logstr)
        {
            foreach (WorkStatusClass works in TrainStatusCollection.WorkStatusList)
            {
                if (works.LogString.IndexOf(logstr) < 0)
                {
                    works.LogString += logstr;
                }
            }
        }
        public void AddRunLogString(string logstr)
        {
            foreach (WorkStatusClass works in RunStatusCollection.WorkStatusList)
            {
                if (works.LogString.IndexOf(logstr) < 0)
                {
                    works.LogString += logstr;
                }
            }
        }

        public bool CheckRepeatCode(List<string> eCodes, Bitmap bmppattern, Bitmap bmpFind, PassInfoClass passInfo,int irepeatCount=1)
        {
            bool result = true;
            WorkStatusClass workstatus = new WorkStatusClass(JetEazy.AnanlyzeProcedureEnum.CHECKREPEATBARCODE);
            string processstring = "Start  CheckRepeatCode." + Environment.NewLine;
            string errorstring = "";
            JetEazy.ReasonEnum reason = JetEazy.ReasonEnum.PASS;

            switch (OCRMethod)
            {
                case OCRMethodEnum.DATAMATRIXGRADE:

                    int recordPCS = 0;
                    if (!string.IsNullOrEmpty(m_BarcodeReadStr))
                    {
                        foreach (string s in eCodes)
                        {
                            //if (s.Contains(m_BarcodeReadStr))
                            if (s.Trim() == m_BarcodeReadStr.Trim())
                            {
                                recordPCS++;
                            }
                        }

                        if (recordPCS > irepeatCount)
                        {
                            errorstring += "2DBarcode Repeat." + Environment.NewLine; ;
                            processstring += "2DBarcode Repeat." + Environment.NewLine;
                            reason = JetEazy.ReasonEnum.NG;
                            workstatus.AnalyzeProcedure = JetEazy.AnanlyzeProcedureEnum.CHECKREPEATBARCODE;
                            result = false;
                        }
                    }

                    workstatus.SetWorkStatus(bmppattern, bmpFind, bmpErr, reason, errorstring, processstring, passInfo);
                    RunStatusCollection.Add(workstatus);

                    break;
            }

            return result;
        }
        public bool CheckBarCode(bool istrain, Bitmap bmpFind, PassInfoClass passInfo, int IBTolerance, int IBCount, int IBArea)
        {

            WorkStatusClass workstatus = new WorkStatusClass(JetEazy.AnanlyzeProcedureEnum.CHECKBARCODE);
            string processstring = "Start  Decode." + Environment.NewLine;
            string errorstring = "";
            JetEazy.ReasonEnum reason = JetEazy.ReasonEnum.PASS;

            bool isGood = true;
            if (!istrain)
            {
                Bitmap bmpErr = new Bitmap(1, 1);
                Bitmap bmpErrResult = new Bitmap(1, 1);
                isGood = CHECKBARCODE(new Bitmap(bmpFind), ref bmpErr, ref bmpErrResult, IBTolerance, IBCount, IBArea);

                if (!isGood)
                {
                    reason = JetEazy.ReasonEnum.NG;
                    workstatus.SetWorkStatus(bmpFind, bmpErr, bmpErrResult, reason, errorstring, processstring, passInfo);

                }
            }

            //if (INI.ISSAVEOCRIMAGE)
            //    bmpFind.Save(Universal.BarcodeIMAGEPATH + barcode + "_" + OCRMappingMethod + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);



            if (istrain)
                TrainStatusCollection.Add(workstatus);
            else
                RunStatusCollection.Add(workstatus);

            return isGood;
        }
        public bool CHECKBARCODE(Bitmap bmpFind, ref Bitmap bmpErr, ref Bitmap bmpErrResurt, int IBTolerance, int IBCount, int IBArea)
        {
            if (Universal.OPTION == JetEazy.OptionEnum.R3)
            {
                Universal.R3UI.isCheckBarcodeErr = false;

            }
            if (Universal.OPTION == JetEazy.OptionEnum.C3)
            {
                Universal.C3UI.isCheckBarcodeErr = false;

            }
            string filesPath = "D:\\Testtest\\Barcode\\";
            string filesPathFils = "D:\\Testtest\\Barcode\\Bar\\";
            bool isgood = true;
            bool isSave = INI.ISSAVEDebugIMAGE;
            if (isSave)
            {
                if (System.IO.Directory.Exists(filesPathFils) == false)//如果不存在就创建file文件夹
                    System.IO.Directory.CreateDirectory(filesPathFils);
            }

            List<ResultClass> Resultlist = new List<ResultClass>();
            if (isSave)
                bmpFind.Save(filesPath + "Barcode_Rec.png");


            Bitmap bmpFindToTemp = bmpFind.Clone(new Rectangle(0, 0, bmpFind.Width, bmpFind.Height), PixelFormat.Format24bppRgb);
            Bitmap bmpFindTo = bmpFind.Clone(new Rectangle(0, 0, bmpFind.Width, bmpFind.Height), PixelFormat.Format24bppRgb);

            int ivalue = JetEazy.BasicSpace.myImageProcessor.Balance(bmpFindToTemp, ref bmpFindToTemp, JetEazy.BasicSpace.myImageProcessor.EnumThreshold.Minimum);

            //      bmpFindToTemp.Save("D:\\testtest\\barcode\\balance.png");

            Bitmap bmpFindToTemp2 = bmpFind.Clone(new Rectangle(0, 0, bmpFind.Width, bmpFind.Height), PixelFormat.Format24bppRgb);

            //    bmpFindToTemp2.Save("D:\\testtest\\barcode\\balance2.png");
            int[] Histgram2 = new int[256];
            DrawHistGram(bmpFindToTemp2, Histgram2, ivalue);
            bmpFindToTemp2.Dispose();

            int bmax2 = 0;
            int indexTemp2 = 0;
            for (int i = 0; i < ivalue; i++)
            {
                if (bmax2 < Histgram2[i])
                {
                    bmax2 = Histgram2[i];
                    indexTemp2 = i;
                }
            }
            int iMax = indexTemp2 + IBCount;
            int iMin = indexTemp2 - 20;

            if (iMin < 0)
                iMin = 0;
            if (iMax > 255)
                iMax = 255;

            if (isSave)
            {
                Bitmap bmpHist = DrawHist(Histgram2, iMax, iMin);
                bmpHist.Save(filesPath + "Hist.png");
                bmpHist.Dispose();
            }


            Bitmap bmptemp = FindToDefect2(bmpFindTo, iMax, iMin, ref Resultlist);

            List<int> LengthList = new List<int>();
            //排序
            for (int i = 0; i < Resultlist.Count; i++)
            {
                for (int j = i + 1; j < Resultlist.Count; j++)
                {
                    if (Resultlist[i].ptCenter.X > Resultlist[j].ptCenter.X)
                    {
                        ResultClass result = Resultlist[i];
                        Resultlist[i] = Resultlist[j];
                        Resultlist[j] = result;

                    }
                }
                LengthList.Add(Resultlist[i].Rect.Height);
            }
            LengthList.Sort();
            int ileng = LengthList.Count / 3;
            int iAvg = 0;
            int iCount = 0;
            for (int i = ileng; i < LengthList.Count; i++)
            {
                iAvg += LengthList[i];
                iCount++;
            }

            if (iCount != 0)
                iAvg = iAvg / iCount;
            else
                isgood = false;
            LengthList.Clear();


            if (isSave)
                bmptemp.Save(filesPath + "bmpFind.png");


            bmptemp.Dispose();

            List<Bitmap> mylistbmp = new List<Bitmap>();
            Bitmap bmpFindTemp = new Bitmap(bmpFind.Width, bmpFind.Height);
            Graphics ggg = Graphics.FromImage(bmpFindTemp);
            Graphics gFind = Graphics.FromImage(bmpFind);
            ggg.FillRectangle(new SolidBrush(Color.White), new RectangleF(0, 0, bmpFindTemp.Width, bmpFindTemp.Height));
            //   ggg.DrawImage(bmpFind, new PointF(0, 0));

            int index = 0;
            foreach (ResultClass result in Resultlist)
            {
                //PointF[] srcPoints = new PointF[4];     // 扭曲的四邊點  
                //srcPoints[0] = result.pos[0];
                //srcPoints[1] = result.pos[1];
                //srcPoints[2] = result.pos[2];
                //srcPoints[3] = result.pos[3];

                //Bitmap bmp = EzWarper.AoiWarperUtil.AutoRotate(bmpFindTo, srcPoints);
                //Bitmap bmp = bmpFindTo.Clone(result.Rect, PixelFormat.Format24bppRgb);

                Bitmap bmp = ScaleRotate(bmpFindTo, result.Rect, result.fAngle);

                int[] Histgram = new int[256];
                DrawHistGram(bmp, Histgram);
                int bmax = 0;
                int indexTemp = 0;
                for (int i = 0; i < ivalue; i++)
                {
                    if (bmax < Histgram[i])
                    {
                        bmax = Histgram[i];
                        indexTemp = i;
                    }
                }


                Bitmap bmp1 = DrawHistABValue(bmp, indexTemp, IBTolerance, Color.White);
                Bitmap bmp2 = DrawHistABValue(bmp, indexTemp, IBTolerance, Color.Red);

                //Bitmap bmpHist = DrawHist(Histgram);
                //bmpHist.Save("D:\\TestTest\\Barcode\\" + index + ".png");

                if (isSave)
                    bmp1.Save(filesPathFils + index + ".png");

                bool isOK = FindBlob(bmp1, IBCount, IBArea);
                bmp1.Dispose();
                //int ix = 1;
                //int iy = 3;
                //bmp = bmp.Clone(new Rectangle(ix, iy, bmp.Width - ix * 2, bmp.Height - iy * 2), bmp.PixelFormat);
                Pen p = new Pen(Color.Lime, 1);
                if (iAvg == 0 || !isOK || Math.Abs(result.Rect.Height - iAvg) > 5)
                {
                    Universal.R3UI.isCheckBarcodeErr = true;
                    isgood = false;
                    p.Color = Color.Red;
                }
                gFind.DrawLine(p, new Point(result.pos[0].X - 1, result.pos[0].Y - 1), new Point(result.pos[1].X, result.pos[1].Y - 1));
                gFind.DrawLine(p, result.pos[1], result.pos[2]);
                gFind.DrawLine(p, result.pos[2], result.pos[3]);
                gFind.DrawLine(p, new Point(result.pos[3].X - 1, result.pos[3].Y), new Point(result.pos[0].X - 1, result.pos[0].Y - 1));

                //if (bmp.Width > bmp.Height)
                //    bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                ggg.DrawImage(bmp2, result.pos[0].X, result.pos[0].Y);

                bmp2.Dispose();
                index++;
            }
            ggg.Dispose();
            gFind.Dispose();


            //if (isgood)
            //    return isgood;

            bmpErr = bmpFind;
            bmpErrResurt = bmpFindTemp;

            Bitmap bmpFindTemp2 = new Bitmap(bmpFind.Width, bmpFind.Height * 2);
            Graphics g = Graphics.FromImage(bmpFindTemp2);
            g.FillRectangle(new SolidBrush(Color.White), new RectangleF(0, 0, bmpFindTemp2.Width, bmpFindTemp2.Height));
            g.DrawImage(bmpFind, new PointF(0, 0));
            g.DrawImage(bmpFindTemp, new PointF(0, bmpFindTemp.Height));
            g.Dispose();

            Universal.R3UI.bmpBarcodeCHECKERR = bmpFindTemp2;
            if (isSave)
                bmpFindTemp2.Save(filesPath + "TestFind.png");

            return isgood;
        }
        /// <summary>
        /// 与绝对值比较,找出不同的地方
        /// </summary>
        /// <param name="iValue">平均值</param>
        /// <param name="iDifference">允许的差异</param>
        Bitmap DrawHistABValue(Bitmap Bmp, int iValue, int iDifference, Color color)
        {
            if (Bmp.PixelFormat == PixelFormat.Format24bppRgb)
            {
                Bitmap bmpOutPut = Bmp.Clone(new Rectangle(0, 0, Bmp.Width, Bmp.Height), PixelFormat.Format24bppRgb);
                BitmapData BmpData = Bmp.LockBits(new Rectangle(0, 0, Bmp.Width, Bmp.Height), ImageLockMode.ReadOnly, Bmp.PixelFormat);
                BitmapData BmpDataOut = bmpOutPut.LockBits(new Rectangle(0, 0, bmpOutPut.Width, bmpOutPut.Height), ImageLockMode.ReadOnly, bmpOutPut.PixelFormat);
                unsafe
                {
                    Parallel.ForEach(Partitioner.Create(0, BmpData.Height), (H) =>
                    {

                        int X, Y, Width, Height, Stride;//, pixB = 0, pixG = 0, pixR = 0;
                                                        //   byte Red, Green, Blue;
                        byte* Scan0, CurP, Scan1, CurP1;

                        // double pixelR = 0, pixelG = 0, pixelB = 0;
                        Width = BmpData.Width;
                        Height = BmpData.Height;
                        Stride = BmpData.Stride;
                        Scan0 = (byte*)BmpData.Scan0;
                        Scan1 = (byte*)BmpDataOut.Scan0;

                        for (Y = H.Item1; Y < H.Item2; Y++)
                        {
                            CurP = Scan0 + Y * Stride;
                            CurP1 = Scan1 + Y * Stride;
                            for (X = 0; X < Width; X++)
                            {

                                //byte Value = (byte)*(CurP);
                                byte Value = (byte)((float)*(CurP) * 0.114f + (float)*(CurP + 1) * 0.587f + (float)*(CurP + 2) * 0.299f);

                                if (Math.Abs(Value - iValue) > iDifference)
                                {
                                    *CurP1 = color.B;
                                    *(CurP1 + 1) = color.G;
                                    *(CurP1 + 2) = color.R;
                                }
                                else
                                {
                                    *CurP1 = 0;
                                    *(CurP1 + 1) = 0;
                                    *CurP1 = 0;
                                }

                                CurP += 3;
                                CurP1 += 3;
                            }
                        }
                    });
                }
                Bmp.UnlockBits(BmpData);
                bmpOutPut.UnlockBits(BmpDataOut);

                return bmpOutPut;
            }
            return null;
        }

        /// <summary>
        /// 找有问题的点
        /// </summary>
        /// <param name="bmp">原图</param>
        /// <param name="Count">个数</param>
        /// <param name="Area">面积</param>
        /// <returns></returns>
        bool FindBlob(Bitmap bmp, int Count, int Area)
        {
            //bmp.Save("D:\\testtest\\barcode\\aaa.png");
            //Bitmap bitmap = new Bitmap(bmp);
            JetGrayImg grayimage = new JetGrayImg(bmp);

            //grayimage.ToBitmap().Save("D:\\testtest\\barcode\\bbb.png");
            //bitmap.Dispose();

            JetImgproc.Threshold(grayimage, 250, grayimage);
            JetBlob jetBlob = new JetBlob();
            jetBlob.Labeling(grayimage, JConnexity.Connexity4, JBlobLayer.WhiteLayer);
            int icount = jetBlob.BlobCount;


            if (icount > Count)
                return false;

            for (int i = 0; i < icount; i++)
            {
                int iArea = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.Area);
                if (iArea > Area)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 直方图
        /// </summary>
        /// <param name="SrcBmp"></param>
        /// <param name="Histgram"></param>
        public void DrawHistGram(Bitmap Bmp, int[] Histgram)
        {
            if (Bmp.PixelFormat == PixelFormat.Format24bppRgb)
            {
                BitmapData BmpData = Bmp.LockBits(new Rectangle(0, 0, Bmp.Width, Bmp.Height), ImageLockMode.ReadOnly, Bmp.PixelFormat);
                unsafe
                {
                    Parallel.ForEach(Partitioner.Create(0, BmpData.Height), (H) =>
                    {

                        int X, Y, Width, Height, Stride;//, pixB = 0, pixG = 0, pixR = 0;
                                                        //   byte Red, Green, Blue;
                        byte* Scan0, CurP;

                        // double pixelR = 0, pixelG = 0, pixelB = 0;
                        Width = BmpData.Width;
                        Height = BmpData.Height;
                        Stride = BmpData.Stride;
                        Scan0 = (byte*)BmpData.Scan0;

                        for (Y = H.Item1; Y < H.Item2; Y++)
                        {
                            CurP = Scan0 + Y * Stride;
                            for (X = 0; X < Width; X++)
                            {
                                //        byte Value = *CurP;
                                byte Value = (byte)((float)*(CurP) * 0.114f + (float)*(CurP + 1) * 0.587f + (float)*(CurP + 2) * 0.299f);
                                lock (Histgram)
                                    Histgram[Value]++;
                                CurP += 3;

                            }
                        }
                    });
                }
                Bmp.UnlockBits(BmpData);
            }
        }

        /// <summary>
        /// 直方图
        /// </summary>
        /// <param name="SrcBmp"></param>
        /// <param name="Histgram"></param>
        public void DrawHistGram(Bitmap Bmp, int[] Histgram, int Max)
        {
            if (Bmp.PixelFormat == PixelFormat.Format24bppRgb)
            {
                BitmapData BmpData = Bmp.LockBits(new Rectangle(0, 0, Bmp.Width, Bmp.Height), ImageLockMode.ReadOnly, Bmp.PixelFormat);
                unsafe
                {
                    Parallel.ForEach(Partitioner.Create(0, BmpData.Height), (H) =>
                    {

                        int X, Y, Width, Height, Stride;//, pixB = 0, pixG = 0, pixR = 0;
                                                        //   byte Red, Green, Blue;
                        byte* Scan0, CurP;

                        // double pixelR = 0, pixelG = 0, pixelB = 0;
                        Width = BmpData.Width;
                        Height = BmpData.Height;
                        Stride = BmpData.Stride;
                        Scan0 = (byte*)BmpData.Scan0;

                        for (Y = H.Item1; Y < H.Item2; Y++)
                        {
                            CurP = Scan0 + Y * Stride;

                            //        byte Value = *CurP;

                            for (X = 0; X < Width; X++)
                            {
                                byte Value = (byte)((float)*(CurP) * 0.114f + (float)*(CurP + 1) * 0.587f + (float)*(CurP + 2) * 0.299f);

                                if (Value < Max)
                                {
                                    lock (Histgram)
                                        Histgram[Value]++;
                                }
                                CurP += 3;

                            }
                        }
                    });
                }
                Bmp.UnlockBits(BmpData);
            }
        }
        public Bitmap DrawHist(int[] Histgram)
        {
            int bmax = 0;
            int indexTemp = 0;
            for (int i = 0; i < 256; i++)
            {
                if (bmax < Histgram[i])
                {
                    bmax = Histgram[i];
                    indexTemp = i;
                }
            }

            Bitmap bitmap = new Bitmap(Histgram.Length, bmax, PixelFormat.Format8bppIndexed);

            BitmapData BmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            unsafe
            {
                Parallel.ForEach(Partitioner.Create(0, BmpData.Width), (H) =>
                {

                    int X, Y, Width, Height, Stride;//, pixB = 0, pixG = 0, pixR = 0;
                                                    //   byte Red, Green, Blue;
                    byte* Scan0, CurP;

                    // double pixelR = 0, pixelG = 0, pixelB = 0;
                    Width = BmpData.Width;
                    Height = BmpData.Height;
                    Stride = BmpData.Stride;
                    Scan0 = (byte*)BmpData.Scan0;

                    for (X = H.Item1; X < H.Item2; X++)
                    {
                        for (Y = 0; Y < bmax - Histgram[X]; Y++)
                        {
                            CurP = Scan0 + Y * Stride + X;

                            *(CurP) = 255;
                        }
                    }
                });
            }
            bitmap.UnlockBits(BmpData);

            return bitmap;
        }

        public Bitmap DrawHist(int[] Histgram, int Max, int Min)
        {
            int bmax = 0;
            int indexTemp = 0;
            for (int i = 0; i < 256; i++)
            {
                if (bmax < Histgram[i])
                {
                    bmax = Histgram[i];
                    indexTemp = i;
                }
            }

            if (Histgram == null || Histgram.Length < 2 || bmax < 2)
                return new Bitmap(1, 1);

            Bitmap bitmap = new Bitmap(Histgram.Length, bmax, PixelFormat.Format24bppRgb);

            BitmapData BmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            unsafe
            {
                Parallel.ForEach(Partitioner.Create(0, BmpData.Width), (H) =>
                {

                    int X, Y, Width, Height, Stride;//, pixB = 0, pixG = 0, pixR = 0;
                                                    //   byte Red, Green, Blue;
                    byte* Scan0, CurP;

                    // double pixelR = 0, pixelG = 0, pixelB = 0;
                    Width = BmpData.Width;
                    Height = BmpData.Height;
                    Stride = BmpData.Stride;
                    Scan0 = (byte*)BmpData.Scan0;

                    for (X = H.Item1; X < H.Item2; X++)
                    {

                        if (X == Min || X == Max)
                        {
                            for (Y = 0; Y < bmax; Y++)
                            {
                                CurP = Scan0 + Y * Stride + X * 3;

                                *(CurP) = 0;
                                *(CurP + 1) = 0;
                                *(CurP + 2) = 255;
                            }
                        }
                        else
                        {
                            for (Y = 0; Y < bmax - Histgram[X]; Y++)
                            {
                                CurP = Scan0 + Y * Stride + X * 3;

                                *(CurP) = 255;
                                *(CurP + 1) = 255;
                                *(CurP + 2) = 255;
                            }
                        }
                    }
                });
            }
            bitmap.UnlockBits(BmpData);

            return bitmap;
        }

        /// <summary>
        /// 找缺失
        /// </summary>
        /// <param name="bmpCheck"></param>
        /// <param name="iMax"></param>
        /// <param name="iMin"></param>
        /// <param name="Resultlist"></param>
        /// <returns></returns>
        public Bitmap FindToDefect2(Bitmap bmpCheck, int iMax, int iMin, ref List<ResultClass> Resultlist)
        {
            Resultlist = new List<ResultClass>();

            if (bmpCheck == null)
                return null;
            //Stopwatch watch = new Stopwatch();
            //watch.Start();

            Bitmap bmp = (Bitmap)bmpCheck.Clone();// new Bitmap(bmpCheck);
            SetImage24(bmp, iMax, iMin);

            JetGrayImg grayimage = new JetGrayImg(bmp);
            JetImgproc.Threshold(grayimage, 10, grayimage);
            JetBlob jetBlob = new JetBlob();
            jetBlob.Labeling(grayimage, JConnexity.Connexity4, JBlobLayer.BlackLayer);
            int icount = jetBlob.BlobCount;

            //Graphics gg = Graphics.FromImage(bmpCheck);
            for (int i = 0; i < icount; i++)
            {
                int iArea = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.Area);
                if (iArea > 50)
                {
                    JRotatedRectangleF jetrect = JetBlobFeature.ComputeMinRectangle(jetBlob, i);

                    int itop = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.TopMost);
                    int iLeft = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.LeftMost);
                    int iRight = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.RightMost);
                    int iBottom = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.BottomMost);

                    if (itop == 0)
                        continue;
                    if (iLeft == 0)
                        continue;
                    if (iRight == bmpCheck.Width - 1)
                        continue;
                    if (iBottom == bmpCheck.Height - 1)
                        continue;


                    Point ptCenter = new Point((int)jetrect.fCX, (int)jetrect.fCY);
                    Rectangle myRect = SimpleRect(ptCenter, (int)jetrect.fWidth / 2, (int)jetrect.fHeight / 2);
                    //         myRect.Inflate(new Size(2, 2));

                    //转换矩形的四个角
                    Point[] myPts = RectToPoint(myRect, jetrect.fAngle);

                    for (int s = 0; s < myPts.Length; s++)
                    {
                        for (int j = s + 1; j < myPts.Length; j++)
                        {
                            if (myPts[s].Y > myPts[j].Y)
                            {
                                Point poif = myPts[s];
                                myPts[s] = myPts[j];
                                myPts[j] = poif;
                            }
                        }
                    }
                    if (myPts[0].X > myPts[1].X)
                    {
                        Point poi = myPts[0];
                        myPts[0] = myPts[1];
                        myPts[1] = poi;
                    }
                    if (myPts[2].X < myPts[3].X)
                    {
                        Point poi = myPts[2];
                        myPts[2] = myPts[3];
                        myPts[3] = poi;
                    }

                    double angleOfLine = Math.Atan2((myPts[2].Y - myPts[3].Y), (myPts[2].X - myPts[3].X)) * 180 / Math.PI;

                    if (myRect.Width > myRect.Height)
                    {
                        int itemp = myRect.Width;
                        myRect.Width = myRect.Height;
                        myRect.Height = itemp;
                    }
                    myRect = new Rectangle(ptCenter.X - myRect.Width / 2, ptCenter.Y - myRect.Height / 2, myRect.Width, myRect.Height);

                    //Point[] myPts = new Point[4];
                    //myPts[0] = new Point(iLeft, itop);
                    //myPts[1] = new Point(iRight, itop);
                    //myPts[2] = new Point(iLeft, iBottom);
                    //myPts[3] = new Point(iRight, iBottom);


                    ResultClass result = new ResultClass();
                    result.Area = iArea;
                    result.fAngle = angleOfLine;
                    result.Rect = myRect;
                    result.ptCenter = ptCenter;
                    result.pos = myPts;
                    Resultlist.Add(result);


                }

            }
            //gg.Dispose();

            return bmp;
        }

        /// <summary>
        /// 找缺失
        /// </summary>
        /// <param name="bmpCheck"></param>
        /// <param name="iMax"></param>
        /// <param name="iMin"></param>
        /// <param name="Resultlist"></param>
        /// <returns></returns>
        public Bitmap FindToDefect(Bitmap bmpCheck, int iMax, int iMin, ref List<ResultClass> Resultlist)
        {
            Resultlist = new List<ResultClass>();

            if (bmpCheck == null)
                return null;
            //Stopwatch watch = new Stopwatch();
            //watch.Start();

            Bitmap bmp = (Bitmap)bmpCheck.Clone();// new Bitmap(bmpCheck);
            SetImage24(bmp, iMax, iMin);

            JetGrayImg grayimage = new JetGrayImg(bmp);
            JetImgproc.Threshold(grayimage, 10, grayimage);
            JetBlob jetBlob = new JetBlob();
            jetBlob.Labeling(grayimage, JConnexity.Connexity4, JBlobLayer.BlackLayer);
            int icount = jetBlob.BlobCount;

            Graphics gg = Graphics.FromImage(bmpCheck);
            for (int i = 0; i < icount; i++)
            {
                int iArea = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.Area);
                if (iArea > 500)
                {
                    JRotatedRectangleF jetrect = JetBlobFeature.ComputeMinRectangle(jetBlob, i);

                    int itop = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.TopMost);
                    int iLeft = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.LeftMost);
                    int iRight = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.RightMost);
                    int iBottom = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.BottomMost);

                    if (itop == 0)
                        continue;
                    if (iLeft == 0)
                        continue;
                    if (iRight == bmpCheck.Width - 1)
                        continue;
                    if (iBottom == bmpCheck.Height - 1)
                        continue;

                    Point ptCenter = new Point((int)jetrect.fCX, (int)jetrect.fCY);
                    Rectangle myRect = SimpleRect(ptCenter, (int)jetrect.fWidth / 2, (int)jetrect.fHeight / 2);
                    //         myRect.Inflate(new Size(2, 2));

                    //转换矩形的四个角
                    Point[] myPts = RectToPoint(myRect, -jetrect.fAngle);
                    Pen p = new Pen(Color.Blue, 1);
                    // p.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;

                    Point[] ptnew = new Point[4];
                    int iHeight = 0;
                    int iWidth = 0;
                    double angleOfLine = 0;
                    if (myPts[0].Y > myPts[1].Y && myPts[0].Y > myPts[2].Y && myPts[0].Y > myPts[3].Y)
                    {

                        double da = distance(myPts[0], myPts[1]);
                        double db = distance(myPts[0], myPts[2]);

                        if (da > db)
                        {
                            iHeight = (int)db;
                            iWidth = (int)da;
                            angleOfLine = Math.Atan2((myPts[1].Y - myPts[0].Y), (myPts[1].X - myPts[0].X)) * 180 / Math.PI;

                            ptnew[0] = myPts[2];
                            ptnew[1] = myPts[3];
                            ptnew[2] = myPts[0];
                            ptnew[3] = myPts[1];
                        }
                        else
                        {
                            iHeight = (int)da;
                            iWidth = (int)db;
                            angleOfLine = Math.Atan2((myPts[2].Y - myPts[0].Y), (myPts[2].X - myPts[0].X)) * 180 / Math.PI;

                            ptnew[0] = myPts[3];
                            ptnew[1] = myPts[1];
                            ptnew[2] = myPts[2];
                            ptnew[3] = myPts[0];
                        }
                    }
                    else if (myPts[1].Y > myPts[0].Y && myPts[1].Y > myPts[2].Y && myPts[1].Y > myPts[3].Y)
                    {

                        double da = distance(myPts[1], myPts[0]);
                        double db = distance(myPts[1], myPts[3]);

                        if (da > db)
                        {
                            iHeight = (int)db;
                            iWidth = (int)da;
                            angleOfLine = Math.Atan2((myPts[0].Y - myPts[1].Y), (myPts[0].X - myPts[1].X)) * 180 / Math.PI;

                            ptnew[0] = myPts[3];
                            ptnew[1] = myPts[2];
                            ptnew[2] = myPts[1];
                            ptnew[3] = myPts[0];
                        }
                        else
                        {
                            iHeight = (int)da;
                            iWidth = (int)db;
                            angleOfLine = Math.Atan2((myPts[3].Y - myPts[1].Y), (myPts[3].X - myPts[1].X)) * 180 / Math.PI;

                            ptnew[0] = myPts[2];
                            ptnew[1] = myPts[0];
                            ptnew[2] = myPts[3];
                            ptnew[3] = myPts[1];
                        }
                    }
                    else if (myPts[2].Y > myPts[0].Y && myPts[2].Y > myPts[1].Y && myPts[2].Y > myPts[3].Y)
                    {

                        double da = distance(myPts[2], myPts[0]);
                        double db = distance(myPts[2], myPts[3]);

                        if (da > db)
                        {
                            iHeight = (int)db;
                            iWidth = (int)da;
                            angleOfLine = Math.Atan2((myPts[0].Y - myPts[2].Y), (myPts[0].X - myPts[2].X)) * 180 / Math.PI;

                            ptnew[0] = myPts[3];
                            ptnew[1] = myPts[1];
                            ptnew[2] = myPts[2];
                            ptnew[3] = myPts[0];
                        }
                        else
                        {
                            iHeight = (int)da;
                            iWidth = (int)db;
                            angleOfLine = Math.Atan2((myPts[3].Y - myPts[2].Y), (myPts[3].X - myPts[2].X)) * 180 / Math.PI;

                            ptnew[0] = myPts[1];
                            ptnew[1] = myPts[0];
                            ptnew[2] = myPts[3];
                            ptnew[3] = myPts[2];
                        }
                    }
                    else if (myPts[3].Y > myPts[0].Y && myPts[3].Y > myPts[1].Y && myPts[3].Y > myPts[2].Y)
                    {

                        double da = distance(myPts[3], myPts[1]);
                        double db = distance(myPts[3], myPts[2]);

                        if (da > db)
                        {
                            iHeight = (int)db;
                            iWidth = (int)da;
                            angleOfLine = Math.Atan2((myPts[1].Y - myPts[3].Y), (myPts[1].X - myPts[3].X)) * 180 / Math.PI;

                            ptnew[0] = myPts[2];
                            ptnew[1] = myPts[0];
                            ptnew[2] = myPts[3];
                            ptnew[3] = myPts[1];
                        }
                        else
                        {
                            iHeight = (int)da;
                            iWidth = (int)db;
                            angleOfLine = -Math.Atan2((myPts[2].Y - myPts[3].Y), (myPts[2].X - myPts[3].X)) * 180 / Math.PI;

                            ptnew[0] = myPts[0];
                            ptnew[1] = myPts[1];
                            ptnew[2] = myPts[2];
                            ptnew[3] = myPts[3];
                        }
                    }
                    else if (myPts[0].X == myPts[1].X)
                    {
                        double da = distance(myPts[0], myPts[1]);
                        double db = distance(myPts[0], myPts[2]);

                        if (da > db)
                        {
                            iHeight = (int)db;
                            iWidth = (int)da;
                            angleOfLine = 90;

                            ptnew[0] = myPts[0];
                            ptnew[1] = myPts[1];
                            ptnew[2] = myPts[2];
                            ptnew[3] = myPts[3];
                        }
                        else
                        {
                            iHeight = (int)da;
                            iWidth = (int)db;
                            angleOfLine = 0;

                            ptnew[0] = myPts[0];
                            ptnew[1] = myPts[2];
                            ptnew[2] = myPts[1];
                            ptnew[3] = myPts[3];
                        }
                    }
                    else if (myPts[0].Y == myPts[1].Y)
                    {

                        double da = distance(myPts[0], myPts[1]);
                        double db = distance(myPts[0], myPts[2]);

                        if (da > db)
                        {
                            iHeight = (int)db;
                            iWidth = (int)da;
                            angleOfLine = 0;

                            ptnew[0] = myPts[0];
                            ptnew[1] = myPts[1];
                            ptnew[2] = myPts[2];
                            ptnew[3] = myPts[3];
                        }
                        else
                        {
                            iHeight = (int)da;
                            iWidth = (int)db;
                            angleOfLine = 90;

                            ptnew[0] = myPts[1];
                            ptnew[1] = myPts[3];
                            ptnew[2] = myPts[0];
                            ptnew[3] = myPts[2];
                        }
                    }

                    if (angleOfLine < -90)
                        angleOfLine = -(180 + angleOfLine);

                    Rectangle rect = SimpleRect(ptCenter, iWidth / 2, iHeight / 2);
                    ResultClass result = new ResultClass();
                    result.Area = iArea;
                    result.fAngle = angleOfLine;// - jetrect.fAngle;
                    result.Rect = rect;
                    result.ptCenter = ptCenter;
                    result.pos = (Point[])ptnew.Clone();
                    Resultlist.Add(result);


                    //gg.DrawLine(p, ptnew[0], ptnew[1]);
                    //gg.DrawLine(p, ptnew[0], ptnew[2]);
                    //gg.DrawLine(p, ptnew[1], ptnew[3]);
                    //gg.DrawLine(new Pen(Color.Lime, 1), ptnew[2], ptnew[3]);
                    //Point poi = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
                    //gg.DrawString("角度:"+angleOfLine.ToString("0.00"), new Font( "宋体", 20), new SolidBrush(Color.Yellow), poi);

                    //Point[] pos = RectToPoint(result.Rect, -result.fAngle);
                    // p = new Pen(Color.Red, 1);
                    //gg.DrawLine(p, pos[0], pos[1]);
                    //gg.DrawLine(p, pos[0], pos[2]); 
                    //gg.DrawLine(p, pos[1], pos[3]);
                    //gg.DrawLine(new Pen(Color.Pink, 8), pos[2], pos[3]);
                    //gg.DrawRectangle(p, result.Rect);

                }

            }
            gg.Dispose();

            //watch.Stop();
            //   this.Text = "用时: " + watch.ElapsedMilliseconds +  " ms 共找到: " + Resultlist.Count + " 个斑点";
            //   bmp.Save(Application.StartupPath + "\\result.bmp");
            return bmp;
        }
        public static double distance(Point p1, Point p2)
        {
            double result = 0;
            result = Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
            return result;
        }
        public Point[] RectToPoint(Rectangle xRect, double xAngle)
        {
            Point[] pts = new Point[4];

            Point ptCenter = GetRectCenter(xRect);
            pts[0] = xRect.Location;
            pts[1] = new Point(xRect.Right, xRect.Y);
            pts[2] = new Point(xRect.Right, xRect.Bottom);
            pts[3] = new Point(xRect.X, xRect.Bottom);

            pts[0] = PointRotate(ptCenter, pts[0], xAngle);
            pts[1] = PointRotate(ptCenter, pts[1], xAngle);
            pts[2] = PointRotate(ptCenter, pts[2], xAngle);
            pts[3] = PointRotate(ptCenter, pts[3], xAngle);



            return pts;
        }
        public Point GetRectCenter(Rectangle Rect)
        {
            return new Point(Rect.X + (Rect.Width >> 1), Rect.Y + (Rect.Height >> 1));
        }
        /// <summary>  
        /// 对一个坐标点按照一个中心进行旋转  
        /// </summary>  
        /// <param name="center">中心点</param>  
        /// <param name="p1">要旋转的点</param>  
        /// <param name="angle">旋转角度，笛卡尔直角坐标</param>  
        /// <returns></returns>  
        private Point PointRotate(Point center, Point p1, double angle)
        {
            Point tmp = new Point();
            double angleHude = angle * Math.PI / 180;/*角度变成弧度*/
            double x1 = (p1.X - center.X) * Math.Cos(angleHude) + (p1.Y - center.Y) * Math.Sin(angleHude) + center.X;
            double y1 = -(p1.X - center.X) * Math.Sin(angleHude) + (p1.Y - center.Y) * Math.Cos(angleHude) + center.Y;
            tmp.X = (int)x1;
            tmp.Y = (int)y1;
            return tmp;
        }

        public Rectangle SimpleRect(Point Pt, int Width, int Height)
        {
            Rectangle rect = SimpleRect(Pt);
            rect.Inflate(Width, Height);

            return rect;
        }
        Rectangle SimpleRect(Point Pt)
        {
            return new Rectangle(Pt.X, Pt.Y, 1, 1);
        }
        /// <summary>
        /// RGB都做调整
        /// </summary>
        /// <param name="bmp">需要调的图</param>
        void SetImage24(Bitmap Bmp, int imax, int imin)
        {
            if (Bmp.PixelFormat == PixelFormat.Format24bppRgb)
            {
                BitmapData BmpData = Bmp.LockBits(new Rectangle(0, 0, Bmp.Width, Bmp.Height), ImageLockMode.ReadOnly, Bmp.PixelFormat);
                unsafe
                {
                    Parallel.ForEach(Partitioner.Create(0, BmpData.Height), (H) =>
                    {

                        int X, Y, Width, Height, Stride;//, pixB = 0, pixG = 0, pixR = 0;
                                                        //   byte Red, Green, Blue;
                        byte* Scan0, CurP;

                        // double pixelR = 0, pixelG = 0, pixelB = 0;
                        Width = BmpData.Width;
                        Height = BmpData.Height;
                        Stride = BmpData.Stride;
                        Scan0 = (byte*)BmpData.Scan0;

                        for (Y = H.Item1; Y < H.Item2; Y++)
                        {
                            CurP = Scan0 + Y * Stride;
                            for (X = 0; X < Width; X++)
                            {
                                byte B = *CurP;
                                byte G = *(CurP + 1);
                                byte R = *(CurP + 2);

                                byte Value = (byte)((float)*(CurP) * 0.114f + (float)*(CurP + 1) * 0.587f + (float)*(CurP + 2) * 0.299f);
                                // if (Math.Abs(Blue - Green) < ites && Math.Abs(Blue - Red) < ites && Math.Abs(Blue - Red) < ites)
                                if (Value <= imax && Value >= imin)
                                {
                                    *CurP = 0;
                                    *(CurP + 1) = 0;
                                    *(CurP + 2) = 0;

                                }
                                else
                                {
                                    *CurP = 255;
                                    *(CurP + 1) = 255;
                                    *(CurP + 2) = 255;
                                }
                                //if (R < G && B < G && Math.Abs(R - B) > 10 && G - R > 20 && G - B > 10)
                                //{
                                //    *CurP = 0;
                                //    *(CurP + 1) = 0;
                                //    *(CurP + 2) = 0;
                                //}
                                CurP += 3;

                            }
                        }
                    });
                }
                Bmp.UnlockBits(BmpData);
            }
        }

        /// <summary>
        /// 旋转并取得旋转后的图
        /// </summary>
        /// <param name="bmp">源图</param>
        /// <param name="rect">位置</param>
        /// <param name="fAngle">旋转角度</param>
        Bitmap ScaleRotate(Bitmap bmp, Rectangle rect, double fAngle)
        {
            float fTargetCX = rect.Width / 2;
            float fTargetCY = rect.Height / 2;
            float fX1 = rect.Width / 2 + rect.X;
            float fY1 = rect.Height / 2 + rect.Y;

            AUColorImg24 imginput24 = new AUColorImg24();
            imginput24.SetImage(0);
            AUUtility.DrawBitmapToAUColorImg24(bmp, ref imginput24);

            AUColorImg24 imgoutput24 = new AUColorImg24();
            imgoutput24.SetImage(0);
            AUUtility.DrawBitmapToAUColorImg24(bmp, ref imgoutput24);

            AUImage.ScaleRotate(imginput24, imgoutput24,
                  fX1, fY1,
                  fX1, fY1,
                 (float)fAngle, 1.0f, 1.0f, eInterpolationBits.eInterpolationBits_8);

            //imgoutput24.Save("D:\\testtest\\barcode\\output.png", eImageFormat.eImageFormat_PNG);
            //imginput24.Save("D:\\testtest\\barcode\\input.png", eImageFormat.eImageFormat_PNG);

            Bitmap bmpResult = new Bitmap(bmp);
            AUUtility.DrawAUColorImg24ToBitmap(imginput24, ref bmpResult);
            bmpResult = bmpResult.Clone(rect, PixelFormat.Format24bppRgb);
            //      bmpResult = bmpResult.Clone(new Rectangle(0,0,rect.Width,rect.Height), PixelFormat.Format24bppRgb);

            //imgoutput24.Save("D:\\testtest\\barcode\\output.png", eImageFormat.eImageFormat_PNG);
            //imginput24.Save("D:\\testtest\\barcode\\input.png", eImageFormat.eImageFormat_PNG);
            return bmpResult;
        }
    }
    [Serializable]
    public class ResultClass
    {
        public double fAngle { get; set; }
        public Rectangle Rect { get; set; }
        public int Area { get; set; }
        public Point ptCenter { get; set; }
        public byte Gray { get; set; }
        public Point[] pos { get; set; }

        public ResultClass Clone()
        {
            ResultClass result = new ResultClass();
            result.fAngle = fAngle;
            result.Rect = Rect;
            result.Area = Area;
            result.ptCenter = ptCenter;
            result.Gray = Gray;
            if (pos != null)
                result.pos = (Point[])pos.Clone();
            return result;
        }
    }
}