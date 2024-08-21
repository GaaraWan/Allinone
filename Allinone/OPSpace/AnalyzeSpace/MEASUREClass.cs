using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

using Allinone;
using JetEazy;
using JetEazy.BasicSpace;
using Allinone.BasicSpace;

namespace Allinone.OPSpace.AnalyzeSpace
{
    public class ColorMeasureClass
    {
        public ColorCheckTypeEnum colortype = ColorCheckTypeEnum.BLUEKEY;
        public ColorCheckMethodEnum colormethod = ColorCheckMethodEnum.QSMC100;

        float mmratio = 0.3f;
        float colorratio = 0.4f;
        int linegap = 5;
        int extendsize = 3;
        float minratio = 0.4f;
        float maxratio = 1.0f;

        public bool IsTempSave = false;

        public ColorMeasureClass(string str)
        {
            FromString(str);
        }
        public ColorMeasureClass()
        {

        }
        void FromString(string str)
        {
            string[] strs = str.Split(',');

            colortype = (ColorCheckTypeEnum)Enum.Parse(typeof(ColorCheckTypeEnum), strs[0], true);
            colormethod = (ColorCheckMethodEnum)Enum.Parse(typeof(ColorCheckMethodEnum), strs[1], true);
            mmratio = float.Parse(strs[2]);
            colorratio = float.Parse(strs[3]);
            linegap = int.Parse(strs[4]);
            extendsize = int.Parse(strs[5]);
            minratio = float.Parse(strs[6]);
            maxratio = float.Parse(strs[7]);
        }
        public bool CheckColor(Bitmap bmprun, PassInfoClass passinfo)
        {
            bool ret = false;
            float checkedratio = 0f;

            switch (colortype)
            {
                case ColorCheckTypeEnum.BLUEKEY:
                case ColorCheckTypeEnum.GOLDSCREW:
                case ColorCheckTypeEnum.SILVERSCREW:
                case ColorCheckTypeEnum.GREYSCREW:
                case ColorCheckTypeEnum.ROSESCREW:
                    //  IsTempSave = true;
                    checkedratio = GetColorRatio(bmprun, passinfo, false);
                    //  IsTempSave = false;
                    //bmprun.Save("D:\\temp.png");
                    if (checkedratio >= minratio && checkedratio <= maxratio)
                        ret = true;

                    break;
                case ColorCheckTypeEnum.TP:

                    ret = CheckTPColor(bmprun, passinfo);

                    break;
            }

            return ret;
        }


        public string CheckRatioReport(Bitmap bmprun)
        {
            string retstr = "";

            switch (colortype)
            {
                case ColorCheckTypeEnum.BLUEKEY:

                    break;
                case ColorCheckTypeEnum.SILVERSCREW:
                case ColorCheckTypeEnum.GOLDSCREW:
                case ColorCheckTypeEnum.GREYSCREW:
                case ColorCheckTypeEnum.ROSESCREW:

                    float checkedratio = 0f;

                    float maxratio = -1000;
                    ColorCheckTypeEnum maxratiocolor = ColorCheckTypeEnum.SILVERSCREW;

                    int colorindex = 0;


                    colorindex = (int)ColorCheckTypeEnum.SILVERSCREW;

                    while (colorindex <= (int)ColorCheckTypeEnum.ROSESCREW)
                    {
                        colortype = (ColorCheckTypeEnum)colorindex;

                        if (colortype == ColorCheckTypeEnum.GOLDSCREW)
                            IsTempSave = true;

                        checkedratio = GetColorRatio(bmprun, null, false);

                        IsTempSave = false;

                        if (maxratio < checkedratio)
                        {
                            maxratio = checkedratio;
                            maxratiocolor = colortype;
                        }

                        retstr += colortype.ToString().PadRight(13, ' ') + " : " + (checkedratio * 100).ToString("0.00") + "%" + Environment.NewLine;

                        colorindex++;

                    }

                    retstr = maxratiocolor.ToString() + "#" + retstr;

                    break;
                case ColorCheckTypeEnum.TP:

                    retstr = GetTPColor(bmprun).ToString();

                    retstr = retstr + "#" + retstr;

                    break;
            }

            return retstr;
        }



        float GetColorRatio(Bitmap bmprun, PassInfoClass passinfo, bool ispurecolor = false, bool isreverse = false)
        {
            float ret = 0f;

            Bitmap bmpthreshold = new Bitmap(bmprun);
            Bitmap bmpruninside = new Bitmap(bmprun);

            if (!ispurecolor)
            {
                JzFindObjectClass jzfind = new JzFindObjectClass();

                //CogHistogramResult histogramresult = myJzCognex.GetHistogram(new CogImage8Grey(bmpthreshold));

                //int Max = histogramresult.Maximum;
                //int Mean = (int)histogramresult.Mean;
                //int Min = histogramresult.Minimum;

                HistogramClass myhisto = new HistogramClass(2);
                myhisto.GetHistogram(bmpthreshold, false);

                int Max = myhisto.MaxGrade;
                int Mean = myhisto.MeanGrade;
                int Min = myhisto.MinGrade;
                IsTempSave = false;

                if (IsTempSave)
                    bmpthreshold.Save(Universal.TESTPATH + "\\ANALYZETEST\\COLORCHECK 0 " + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                //     jzfind.SetThresholdEX(bmpthreshold, SimpleRect(bmpthreshold.Size, 1), (int)((float)(Mean - Min) * mmratio + (float)Min), 0, true, true);
                int imaxvalue = 245;
                if (INI.ISONLYCHICKWB)
                    imaxvalue = 256;
                myImageProcessor.SetThreshold(bmpthreshold, imaxvalue, (int)((float)(Mean - Min) * mmratio + (float)Min), true);
                //jzfind.SetThresholdEX(bmpthreshold, SimpleRect(bmpthreshold.Size, 1), 245, (int)((float)(Mean - Min) * mmratio + (float)Min), true, true);
                if (IsTempSave)
                    bmpthreshold.Save(Universal.TESTPATH + "\\ANALYZETEST\\COLORCHECK 1 " + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                jzfind.FillLines(bmpthreshold, linegap, Color.Black);
                jzfind.Find(bmpthreshold, Color.Red);

                if (IsTempSave)
                    bmpthreshold.Save(Universal.TESTPATH + "\\ANALYZETEST\\COLORCHECK 2 " + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                foreach (FoundClass found in jzfind.FoundList)
                {
                    Rectangle rect = found.rect;

                    rect.Inflate(extendsize, extendsize);

                    DrawRect(bmpruninside, rect, new SolidBrush(Color.Black));
                }

                if (IsTempSave)
                    bmpruninside.Save(Universal.TESTPATH + "\\ANALYZETEST\\COLORCHECK 3 " + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            }

            ret = GetColorRatioSub(bmpruninside);

            if (IsTempSave)
                bmpruninside.Save(Universal.TESTPATH + "\\ANALYZETEST\\COLORCHECK 4 " + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

            bmpthreshold.Dispose();
            bmpruninside.Dispose();

            return ret;
        }

        bool CheckTPColor(Bitmap bmprun, PassInfoClass passinfo)
        {
            bool ret = false;

            if (GetTPColor(bmprun).ToString() == passinfo.OperateString.ToUpper())
            {
                ret = true;
            }

            return ret;
        }

        Rectangle SimpleRect(Size sz, int devide)
        {
            return new Rectangle(0, 0, sz.Width / devide, sz.Height / devide);
        }
        void DrawRect(Bitmap BMP, Rectangle Rect, SolidBrush B)
        {
            Graphics g = Graphics.FromImage(BMP);
            g.FillRectangle(B, Rect);
            g.Dispose();
        }
        public float GetColorRatioSub(Bitmap bmp)
        {
            Rectangle rectbmp = SimpleRect(bmp.Size, 1);
            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr Scan0 = bmpData.Scan0;

            try
            {
                unsafe
                {
                    byte* scan0 = (byte*)(void*)Scan0;
                    byte* pucPtr;
                    byte* pucStart;

                    int xmin = rectbmp.X;
                    int ymin = rectbmp.Y;
                    int xmax = xmin + rectbmp.Width;
                    int ymax = ymin + rectbmp.Height;

                    int x = xmin;
                    int y = ymin;
                    int iStride = bmpData.Stride;

                    //float Ratio = 1f + (float)RatioValue / 100f;
                    //float Ratio = (float)colorratio  / 100f;

                    y = ymin;
                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));


                    float totalcount = 0f;
                    float colorcount = 0f;


                    while (y < ymax)
                    {
                        x = xmin;
                        pucPtr = pucStart;
                        while (x < xmax)
                        {
                            int R = pucPtr[2];
                            int G = pucPtr[1];
                            int B = pucPtr[0];

                            if (R + G + B > 0)
                            {
                                bool isincolorratio = false;
                                bool isincolor = IsInColor(R, G, B, ref isincolorratio);

                                if (isincolor)
                                {
                                    //float colorRatio = (float)pucPtr[2] / ((float)Math.Max(pucPtr[0], pucPtr[1]));
                                    //float colorRatio = (float)Math.Max(pucPtr[0] / ((float)Math.Min(pucPtr[1], pucPtr[2]));
                                    //float colorRatio = (float)R / ((float)Math.Min(G, B));
                                    //if (colorRatio > Ratio && colorRatio <= 1)

                                    if (isincolorratio)
                                    //if (colorRatio > Ratio)
                                    {
                                        colorcount++;

                                        pucPtr[2] = 255;
                                        pucPtr[1] = 0;
                                        pucPtr[0] = 0;
                                    }
                                    else
                                    {
                                        pucPtr[2] = 0;
                                        pucPtr[1] = 0;
                                        pucPtr[0] = 255;
                                    }
                                }

                                totalcount++;
                            }

                            pucPtr += 4;
                            x++;
                        }

                        pucStart += iStride;
                        y++;
                    }

                    bmp.UnlockBits(bmpData);

                    return (totalcount > 0 ? colorcount / totalcount : 0);
                }
            }
            catch (Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
                bmp.UnlockBits(bmpData);

                return 0;
            }
        }
        bool IsInColor(int r, int g, int b, ref bool isincolorratio)
        {
            bool isincolor = false;
            float minvalue = (minratio * 255);
            if (INI.ISONLYCHICKWB && (colortype == ColorCheckTypeEnum.SILVERSCREW || colortype == ColorCheckTypeEnum.GREYSCREW))
            {
                //isincolor = (g > r && b > r && g > 130);
                //  isincolor = (g > r && b > 140 || b > 200);

                int gray = (g * 19595 + b * 38469 + r * 7472) >> 16;
                //isincolor = (g > r && g > 120);


                float maxvalue = (maxratio * 255);
                if (colortype == ColorCheckTypeEnum.SILVERSCREW)
                {
                    if (gray > minvalue)
                        isincolor = true;
                    else
                        isincolor = false;
                }
                else
                {
                    if (gray <= minvalue & colortype == ColorCheckTypeEnum.GREYSCREW)
                        isincolor = true;
                    else
                        isincolor = false;
                }

                if (isincolor)
                    isincolorratio = true;
                else
                    isincolorratio = false;

            }
            else
            {
                switch (colortype)
                {
                    case ColorCheckTypeEnum.SILVERSCREW:
                        switch (colormethod)
                        {
                            case ColorCheckMethodEnum.QSMC100:
                            case ColorCheckMethodEnum.QSMC1000:
                            case ColorCheckMethodEnum.FXCD100:
                                int gray = (g * 19595 + b * 38469 + r * 7472) >> 16;
                                //isincolor = (g > r && b > r && g > 130);
                                //   isincolor = (g > r && b > 100 || b > 200);
                                //isincolor = (g > r && g > 120);
                                isincolor = (g > r && gray > minvalue);
                                if (isincolor)
                                    //isincolorratio = ((((float)r / (float)Math.Max(g, b)) > colorratio || g > 200) ? true : false);
                                    isincolorratio = true;
                                else
                                    isincolorratio = false;
                                break;
                            case ColorCheckMethodEnum.QSMCUSB:
                            case ColorCheckMethodEnum.FXCDUSB:
                                isincolor = (g > r && g > 120);

                                if (isincolor)
                                    //isincolorratio = ((((float)r / (float)Math.Max(g, b)) > colorratio || g > 200) ? true : false);

                                    isincolorratio = true;
                                else
                                    isincolorratio = false;
                                break;
                            default:

                                break;
                        }
                        break;
                    case ColorCheckTypeEnum.GOLDSCREW:
                    case ColorCheckTypeEnum.ROSESCREW:
                        switch (colormethod)
                        {
                            case ColorCheckMethodEnum.QSMC100:
                            case ColorCheckMethodEnum.QSMC1000:
                            case ColorCheckMethodEnum.FXCD100:
                                isincolor = (r > g || r > b);

                                if (isincolor)
                                    //isincolorratio = ((float)g / (float)Math.Max(g, b) > colorratio ? true : false);
                                    isincolorratio = true;
                                else
                                    isincolorratio = false;

                                break;
                            case ColorCheckMethodEnum.QSMCUSB:
                            case ColorCheckMethodEnum.FXCDUSB:
                                isincolor = (r > g || r > b || r > (int)((float)g * 0.8f));

                                if (isincolor)
                                    //isincolorratio = ((float)g / (float)Math.Max(g, b) > colorratio ? true : false);
                                    isincolorratio = true;
                                else
                                    isincolorratio = false;

                                break;
                            default:

                                break;
                        }
                        break;
                    case ColorCheckTypeEnum.GREYSCREW:
                        switch (colormethod)
                        {
                            case ColorCheckMethodEnum.QSMC100:
                            case ColorCheckMethodEnum.QSMC1000:
                            case ColorCheckMethodEnum.FXCD100:

                                int gray2 = (g * 19595 + b * 38469 + r * 7472) >> 16;
                                //     isincolor = (g > r && b < 120);
                                // isincolor = (g > r && g < 100);

                                isincolor = (g > r && b > r && gray2 < minvalue);

                                if (isincolor)
                                    //isincolorratio = ((float)b / (float)Math.Max(g, r) > colorratio ? true : false);
                                    isincolorratio = true;
                                else
                                    isincolorratio = false;

                                break;
                            case ColorCheckMethodEnum.QSMCUSB:
                            case ColorCheckMethodEnum.FXCDUSB:

                                int gray3 = (g * 19595 + b * 38469 + r * 7472) >> 16;

                                //    isincolor = (((float)g * 0.7f) > r && b < 120);
                                isincolor = (((float)g * 0.7f) > r && g < 100);

                                if (isincolor)
                                    //isincolorratio = ((float)b / (float)Math.Max(g, r) > colorratio ? true : false);
                                    isincolorratio = true;
                                else
                                    isincolorratio = false;

                                break;
                            default:

                                break;
                        }
                        break;
                    case ColorCheckTypeEnum.TP:
                        switch (colormethod)
                        {
                            default:

                                break;
                        }
                        break;
                }
            }
            return isincolor;
        }
        bool IsInColor2(int r, int g, int b, ref bool isincolorratio)
        {
            bool isincolor = false;

            switch (colortype)
            {
                case ColorCheckTypeEnum.SILVERSCREW:
                    switch (colormethod)
                    {
                        case ColorCheckMethodEnum.QSMC100:
                        case ColorCheckMethodEnum.QSMC1000:
                        case ColorCheckMethodEnum.FXCD100:
                            //isincolor = (g > r && b > r && g > 130);
                            isincolor = (g > r && b > 140 || b > 200);
                            //isincolor = (g > r && g > 120);

                            if (isincolor)
                                //isincolorratio = ((((float)r / (float)Math.Max(g, b)) > colorratio || g > 200) ? true : false);
                                isincolorratio = true;
                            else
                                isincolorratio = false;
                            break;
                        case ColorCheckMethodEnum.QSMCUSB:
                        case ColorCheckMethodEnum.FXCDUSB:
                            isincolor = (g > r && g > 120);

                            if (isincolor)
                                //isincolorratio = ((((float)r / (float)Math.Max(g, b)) > colorratio || g > 200) ? true : false);

                                isincolorratio = true;
                            else
                                isincolorratio = false;
                            break;
                        default:

                            break;
                    }
                    break;
                case ColorCheckTypeEnum.GOLDSCREW:
                case ColorCheckTypeEnum.ROSESCREW:
                    switch (colormethod)
                    {
                        case ColorCheckMethodEnum.QSMC100:
                        case ColorCheckMethodEnum.QSMC1000:
                        case ColorCheckMethodEnum.FXCD100:
                            isincolor = (r > g || r > b);

                            if (isincolor)
                                //isincolorratio = ((float)g / (float)Math.Max(g, b) > colorratio ? true : false);
                                isincolorratio = true;
                            else
                                isincolorratio = false;

                            break;
                        case ColorCheckMethodEnum.QSMCUSB:
                        case ColorCheckMethodEnum.FXCDUSB:
                            isincolor = (r > g || r > b || r > (int)((float)g * 0.8f));

                            if (isincolor)
                                //isincolorratio = ((float)g / (float)Math.Max(g, b) > colorratio ? true : false);
                                isincolorratio = true;
                            else
                                isincolorratio = false;

                            break;
                        default:

                            break;
                    }
                    break;
                case ColorCheckTypeEnum.GREYSCREW:
                    switch (colormethod)
                    {
                        case ColorCheckMethodEnum.QSMC100:
                        case ColorCheckMethodEnum.QSMC1000:
                        case ColorCheckMethodEnum.FXCD100:
                            isincolor = (g > r && b > g);
                            // isincolor = (g > r && g < 100);

                            if (isincolor)
                                //isincolorratio = ((float)b / (float)Math.Max(g, r) > colorratio ? true : false);
                                isincolorratio = true;
                            else
                                isincolorratio = false;

                            break;
                        case ColorCheckMethodEnum.QSMCUSB:
                        case ColorCheckMethodEnum.FXCDUSB:
                            isincolor = (((float)g) > r && b < 200);
                            // isincolor = (((float)g * 0.7f) > r && g < 100);

                            if (isincolor)
                                //isincolorratio = ((float)b / (float)Math.Max(g, r) > colorratio ? true : false);
                                isincolorratio = true;
                            else
                                isincolorratio = false;

                            break;
                        default:

                            break;
                    }
                    break;
                case ColorCheckTypeEnum.TP:
                    switch (colormethod)
                    {
                        default:

                            break;
                    }
                    break;
            }

            return isincolor;
        }

        public TPColorEnum GetTPColor(Bitmap bmp)
        {
            Rectangle rectbmp = SimpleRect(bmp.Size, 1);
            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr Scan0 = bmpData.Scan0;

            TPColorEnum ret = TPColorEnum.NONE;

            try
            {
                unsafe
                {
                    byte* scan0 = (byte*)(void*)Scan0;
                    byte* pucPtr;
                    byte* pucStart;

                    int xmin = rectbmp.X;
                    int ymin = rectbmp.Y;
                    int xmax = xmin + rectbmp.Width;
                    int ymax = ymin + rectbmp.Height;

                    int x = xmin;
                    int y = ymin;
                    int iStride = bmpData.Stride;

                    y = ymin;
                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                    int silvercolorcount = 0;
                    int goldcolorcount = 0;
                    int greycolorcount = 0;

                    while (y < ymax)
                    {
                        x = xmin;
                        pucPtr = pucStart;
                        while (x < xmax)
                        {
                            int R = pucPtr[2];
                            int G = pucPtr[1];
                            int B = pucPtr[0];

                            bool issilver = false;
                            bool isgold = false;
                            bool isgrey = false;

                            if (R + G + B > 0)
                            {
                                CheckTPColor(R, G, B, ref issilver, ref isgold, ref isgrey);

                                if (issilver)
                                {
                                    silvercolorcount++;
                                }
                                if (isgold)
                                {
                                    goldcolorcount++;
                                }
                                if (isgrey)
                                {
                                    greycolorcount++;
                                }
                            }

                            pucPtr += 4;
                            x++;
                        }

                        pucStart += iStride;
                        y++;
                    }

                    bmp.UnlockBits(bmpData);

                    int max = Math.Max(silvercolorcount, Math.Max(goldcolorcount, greycolorcount));

                    if (silvercolorcount == max)
                    {
                        ret = TPColorEnum.SILVER;
                    }
                    else if (goldcolorcount == max)
                    {
                        ret = TPColorEnum.GOLD;
                    }
                    else if (greycolorcount == max)
                    {
                        ret = TPColorEnum.GREY;
                    }

                    return ret;
                }
            }
            catch (Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
                bmp.UnlockBits(bmpData);

                return TPColorEnum.NONE;
            }
        }

        void CheckTPColor(int r, int g, int b, ref bool issilver, ref bool isgold, ref bool isgrey)
        {
            switch (colormethod)
            {
                default:

                    issilver = (b > r && b > g && b > 170);
                    isgold = (r > g && r > b);
                    isgrey = (b > r && b > g && b < 150);

                    break;
            }
        }

    }
    public class SolderBallMeasureClass
    {
        CheckBaseParaPropertyGridClass checkBaseParaPropertyGrid = new CheckBaseParaPropertyGridClass();
        //Bitmap bmpInput = new Bitmap(1, 1);
        JzToolsClass m_JzTools = new JzToolsClass();
        JzFindObjectClass m_JzFind = new JzFindObjectClass();

        public SolderBallMeasureClass(string str)
        {
            FromString(str);
        }
        public SolderBallMeasureClass()
        {

        }
        public bool CheckSolderBall(Bitmap bmpinput, Bitmap bmpmask, ref Bitmap bmpoutput, bool istrain, WorkStatusClass workstatus, PassInfoClass passinfo)
        {
            bool ret = false;
            string str = "";

            //bmpInput.Dispose();
            Bitmap bmpInput = new Bitmap(bmpinput);
            Rectangle myRect = m_JzTools.SimpleRect(bmpinput.Size);
            
            //jzFindObjectClass.AH_SetThreshold(bmpinput, ref bmpInput, checkBaseParaPropertyGrid.chkThresholdValue);
            m_JzFind.SetThresholdEX(bmpInput, myRect, 0, checkBaseParaPropertyGrid.chkThresholdValue, 0, !(checkBaseParaPropertyGrid.chkblobmode == BlobMode.White));
            //m_JzFind.AH_FindBlob(bmpInput, checkBaseParaPropertyGrid.chkblobmode == BlobMode.White);
            m_JzFind.Find(bmpInput, Color.Red);
            m_JzFind.SortByArea();
        
            int myArea = myRect.Width * myRect.Height;

            bmpoutput.Dispose();
            bmpoutput = new Bitmap(bmpinput);

            if (m_JzFind.FoundList.Count > 0)
            {
                //int maxindex = jzFindObjectClass.GetMaxRectIndex();
                FoundClass foundClass = m_JzFind.GetFoundBySort(0);
                myRect = foundClass.rect;
                myArea = foundClass.Area;

                if (istrain)
                {
                    m_JzFind.GetMaskedImage(bmpoutput, bmpInput, Color.Black);
                    m_JzTools.DrawRectEx(bmpoutput, myRect, new Pen(Color.Red));
                }
                str = $"宽度{myRect.Width.ToString()}高度{myRect.Height.ToString()}面积{myArea.ToString()}";
                workstatus.ProcessString += str + Environment.NewLine;
            }

            if (istrain)
            {
                if (!IsInRangeRatio(myRect.Width, checkBaseParaPropertyGrid.chkWidth, checkBaseParaPropertyGrid.chkWidthUpratio * 0.01))
                {
                    str = "train solderball is NG , width is " + myRect.Width.ToString() + " not in Range " + (checkBaseParaPropertyGrid.chkWidthUpratio).ToString("0.00") + "%";
                    workstatus.ProcessString += str + Environment.NewLine;
                    workstatus.Reason = ReasonEnum.NG;

                    ret = false || !checkBaseParaPropertyGrid.chkIsOpen;
                }
                else if (!IsInRangeRatio(myRect.Height, checkBaseParaPropertyGrid.chkHeight, checkBaseParaPropertyGrid.chkHeightUpratio * 0.01))
                {
                    str = "train solderball is NG , height is " + myRect.Height.ToString() + " not in Range " + (checkBaseParaPropertyGrid.chkHeightUpratio).ToString("0.00") + "%";
                    workstatus.ProcessString += str + Environment.NewLine;
                    workstatus.Reason = ReasonEnum.NG;

                    ret = false || !checkBaseParaPropertyGrid.chkIsOpen;
                }
                else if (!IsInRangeRatio(myArea, checkBaseParaPropertyGrid.chkArea, checkBaseParaPropertyGrid.chkAreaUpratio * 0.01))
                {
                    str = "train solderball is NG , area is " + myArea.ToString() + " not in Range " + (checkBaseParaPropertyGrid.chkAreaUpratio).ToString("0.00") + "%";
                    workstatus.ProcessString += str + Environment.NewLine;
                    workstatus.Reason = ReasonEnum.NG;

                    ret = false || !checkBaseParaPropertyGrid.chkIsOpen;
                }
                else
                {
                    str = "train solderball is PASS ";
                    workstatus.ProcessString += str + Environment.NewLine;
                    workstatus.Reason = ReasonEnum.PASS;

                    ret = true;
                }
            }
            else
            {
                if (!IsInRangeRatio(myRect.Width, checkBaseParaPropertyGrid.chkWidth, checkBaseParaPropertyGrid.chkWidthUpratio * 0.01))
                {
                    str = "Run solderball is NG , width is " + myRect.Width.ToString() + " not in Range " + (checkBaseParaPropertyGrid.chkWidthUpratio).ToString("0.00") + "%";
                    workstatus.ProcessString += str + Environment.NewLine;
                    workstatus.Reason = ReasonEnum.NG;

                    ret = false;// || !checkBaseParaPropertyGrid.chkIsOpen;
                }
                else if (!IsInRangeRatio(myRect.Height, checkBaseParaPropertyGrid.chkHeight, checkBaseParaPropertyGrid.chkHeightUpratio * 0.01))
                {
                    str = "Run solderball is NG , height is " + myRect.Height.ToString() + " not in Range " + (checkBaseParaPropertyGrid.chkHeightUpratio).ToString("0.00") + "%";
                    workstatus.ProcessString += str + Environment.NewLine;
                    workstatus.Reason = ReasonEnum.NG;

                    ret = false;// || !checkBaseParaPropertyGrid.chkIsOpen;
                }
                else if (!IsInRangeRatio(myArea, checkBaseParaPropertyGrid.chkArea, checkBaseParaPropertyGrid.chkAreaUpratio * 0.01))
                {
                    str = "Run solderball is NG , area is " + myArea.ToString() + " not in Range " + (checkBaseParaPropertyGrid.chkAreaUpratio).ToString("0.00") + "%";
                    workstatus.ProcessString += str + Environment.NewLine;
                    workstatus.Reason = ReasonEnum.NG;

                    ret = false;// || !checkBaseParaPropertyGrid.chkIsOpen;
                }
                else
                {
                    str = "Run solderball is PASS ";
                    workstatus.ProcessString += str + Environment.NewLine;
                    workstatus.Reason = ReasonEnum.PASS;

                    ret = true;
                }
            }

            bmpInput.Dispose();
            return ret;
        }

        void FromString(string str)
        {
            checkBaseParaPropertyGrid.FromingStr(str);
        }
        bool IsInRangeRatio(double runvalue, double orgvalue, double ratio)
        {
            return (runvalue >= (orgvalue * (1 - ratio))) && (runvalue <= (orgvalue * (1 + ratio)));
        }
    }

    public class MEASUREClass
    {
        Bitmap bmpPattern = new Bitmap(1, 1);
        Bitmap bmpInput = new Bitmap(1, 1);

        public MeasureMethodEnum MeasureMethod = MeasureMethodEnum.NONE;

        public float MMTolerance = 10;  //已無用
        public string MMOPString = "";
        public float MMMaxGap = 0f;     //已無用
        public float MMMinGap = 0f;     //已無用
        public int MMPixelGap = 5;      //已無用
        public float MMHTRatio = 10f;   //已無用
        public float MMWholeRatio = 10f;//已無用

        public WorkStatusCollectionClass TrainStatusCollection = new WorkStatusCollectionClass();
        public WorkStatusCollectionClass RunStatusCollection = new WorkStatusCollectionClass();
        public bool CheckGood = true;
        public string RelateAnalyzeString = "";
        public PassInfoClass PassInfo = new PassInfoClass();

        public bool IsTempSave = false;

        public MEASUREClass()
        {
            //MMTolerance = 10;
            //MMOPString = "";
            //MMMaxGap = 0f;
            //MMMinGap = 0f;
            //MMPixelGap = 5;
            //MMHTRatio = 10f;
            //MMWholeRatio = 10f;
        }
        public MEASUREClass(string str)
        {
            FromString(str);
        }

        public bool MeasureProcess(Bitmap bmpinput, Bitmap bmppattern, Bitmap bmpmask, ref Bitmap bmpoutput,
            int brightness, int contrast, string relateanalyzestr, PassInfoClass passinfo, bool istrain)
        {
            if (MeasureMethod == MeasureMethodEnum.NONE)
            {
                return true;
            }

            SetBrightContrast(bmpinput, brightness, contrast);

            string str = "";
            bool isgood = true;
            CheckGood = true;
            //保留 Pattern 和 Mask 的圖

            if (istrain)
                RelateAnalyzeString = relateanalyzestr;

            WorkStatusClass workstatus = new WorkStatusClass(AnanlyzeProcedureEnum.MEASURE);
            string processstring = "Start " + RelateAnalyzeString + " Measure." + Environment.NewLine;
            string errorstring = "";
            ReasonEnum reason = ReasonEnum.PASS;

            if (istrain)
                PassInfo = new PassInfoClass(passinfo, OPLevelEnum.COPY);

            str = relateanalyzestr + " Use Method " + MeasureMethod.ToString() + " For Measure";
            workstatus.ProcessString += str + Environment.NewLine;

            //IsTempSave = true;

            switch (MeasureMethod)
            {
                case MeasureMethodEnum.BLIND:

                    isgood = BlindMeasure(bmpinput, bmpmask, ref bmpoutput, istrain, workstatus);

                    break;
                case MeasureMethodEnum.MBCHECK:

                    //IsTempSave = true;

                    isgood = MBCheck(bmpinput, bmpmask, ref bmpoutput, istrain, workstatus);

                    //IsTempSave = false;

                    break;
                case MeasureMethodEnum.COLORCHECK:

                    isgood = ColorMeasure(bmpinput, bmpmask, ref bmpoutput, istrain, workstatus, PassInfo);

                    break;

                case MeasureMethodEnum.SOLDERBALLCHECK:

                    isgood = SolderBallMeasure(bmpinput, bmpmask, ref bmpoutput, istrain, workstatus, PassInfo);

                    break;

            }

            if (isgood)
            {
                str = relateanalyzestr + " Measure Successful.";
                workstatus.ProcessString += str + Environment.NewLine;
                workstatus.Reason = ReasonEnum.PASS;
            }
            else
            {
                str = relateanalyzestr + " Measure Fail.";
                workstatus.ProcessString += str + Environment.NewLine;
                workstatus.Reason = ReasonEnum.NG;
            }

            processstring = workstatus.ProcessString;
            reason = workstatus.Reason;
            errorstring = workstatus.ErrorString;

            workstatus.SetWorkStatus(bmppattern, bmpinput, bmppattern, reason, errorstring, processstring, PassInfo);

            if (istrain)
                TrainStatusCollection.Add(workstatus);
            else
                RunStatusCollection.Add(workstatus);
            CheckGood = isgood;
            return isgood;
        }

        #region Measure Fuctions

        #region Blind Measurement Function
        int ORGBlindArea = 0;
        int RUNBlindArea = 0;
        public bool BlindMeasure(Bitmap bmpinput, Bitmap bmpmask, ref Bitmap bmpoutput, bool istrain, WorkStatusClass workstatus)
        {
            float diffratio = 0.2f;
            float threadratio = 0.3f;
            float filterratio = 0.3f;
            float tolerance = 0.3f;

            string[] strs = MMOPString.Split(',');

            diffratio = float.Parse(strs[0]);
            threadratio = float.Parse(strs[1]);
            filterratio = float.Parse(strs[2]);
            tolerance = float.Parse(strs[3]);


            bool isgood = false;
            bool issomthing = false;

            string str = "";

            //IsTempSave = true;

            //WorkStatusClass workstatus = new WorkStatusClass(AnanlyzeProcedureEnum.MEASURETRAIN);
            if (istrain)
                workstatus.ProcessString += "Start " + RelateAnalyzeString + " Blind Train." + Environment.NewLine;
            else
                workstatus.ProcessString += "Start " + RelateAnalyzeString + " Blind Run." + Environment.NewLine;
            //string errorstring = "";
            //ReasonEnum reason = ReasonEnum.PASS;

            HistogramClass histogram = new HistogramClass(2);
            JzFindObjectClass jzfind = new JzFindObjectClass();

            if (IsTempSave)
            {
                bmpinput.Save(Universal.TESTPATH + "\\ANALYZETEST\\BLIND INPUT-" + RelateAnalyzeString + (istrain ? "-T" : "-R") + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                bmpmask.Save(Universal.TESTPATH + "\\ANALYZETEST\\BLIND MASK-" + RelateAnalyzeString + (istrain ? "-T" : "-R") + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            }


            histogram.GetHistogram(bmpinput, bmpmask, true);

            int mean = histogram.MeanGrade;
            int min = histogram.MinGrade;
            int max = histogram.MaxGrade;
            int mode = histogram.ModeGrade;

            int maxdiff = (int)((float)(max - mode) * threadratio);
            int mindiff = (int)((float)(mode - min) * threadratio);

            issomthing = ((mode - min) > (int)((float)mode * diffratio)) || ((max - mode) > (int)((float)mode * diffratio));

            if (istrain)
            {
                ORGBlindArea = 0;
                RUNBlindArea = 0;

                bmpPattern.Dispose();
                bmpPattern = new Bitmap(bmpinput);

                bmpoutput.Dispose();
                bmpoutput = new Bitmap(bmpinput);

                if (issomthing)
                {

                    jzfind.SetThresholdEX(bmpoutput, bmpmask, 255, mode + maxdiff, mode - mindiff, false, false);
                    jzfind.Find(bmpoutput, Color.Red);

                    int filterarea = jzfind.GetMaxArea();
                    filterarea = (int)((float)filterarea * filterratio);

                    Rectangle rect = jzfind.GetRect(true, filterarea);

                    DrawRect(bmpoutput, rect, new Pen(Color.Lime));

                    ORGBlindArea = rect.Width * rect.Height;
                    RUNBlindArea = ORGBlindArea;

                    str = "BLIND is here and the area is " + ORGBlindArea.ToString();
                    workstatus.ProcessString += str + Environment.NewLine;
                }
                else
                {
                    str = "BLIND is not here";
                    workstatus.ProcessString += str + Environment.NewLine;
                }

                workstatus.Reason = ReasonEnum.PASS;
                isgood = true;
                //workstatus.SetWorkStatus(bmpPattern, bmpinput, bmpPattern, reason, errorstring, processstring, PassInfo);

                //TrainStatusCollection.Add(workstatus);

            }
            else
            {
                bmpInput.Dispose();
                bmpInput = new Bitmap(bmpinput);

                bmpoutput.Dispose();
                bmpoutput = new Bitmap(bmpinput);

                if (ORGBlindArea == 0 && issomthing)
                {
                    str = "BLIND Check Error (Nothing To Something).";
                    workstatus.ProcessString += str + Environment.NewLine;
                    workstatus.Reason = ReasonEnum.NG;

                    isgood = false;
                }
                else if (ORGBlindArea != 0 && !issomthing)
                {
                    str = "BLIND Check Error (Something To Nothing).";
                    workstatus.ProcessString += str + Environment.NewLine;
                    workstatus.Reason = ReasonEnum.NG;

                    isgood = false;
                }
                else
                {
                    jzfind.SetThresholdEX(bmpoutput, bmpmask, 255, mode + maxdiff, mode - mindiff, false, false);
                    //jzfind.SetThresholdEX(bmpoutput, SimpleRect(bmpoutput.Size, 1), mode + maxdiff, mode - mindiff, false, false);

                    //jzfind.SetThreshold(bmpoutput, SimpleRect(bmpoutput.Size, 1), min, (int)((float)(mode - min) * threadratio), 255, true);

                    jzfind.Find(bmpoutput, Color.Red);

                    int filterarea = jzfind.GetMaxArea();
                    filterarea = (int)((float)filterarea * filterratio);

                    Rectangle rect = jzfind.GetRect(true, filterarea);

                    DrawRect(bmpoutput, rect, new Pen(Color.Lime));

                    RUNBlindArea = rect.Width * rect.Height;

                    if (!IsInRangeRatio((double)RUNBlindArea, (double)ORGBlindArea, (double)tolerance))
                    {
                        str = "Run BLIND is NG , Area is " + RUNBlindArea.ToString() + " not in Range " + (tolerance * 100).ToString("0.00") + "%";
                        workstatus.ProcessString += str + Environment.NewLine;
                        workstatus.Reason = ReasonEnum.NG;

                        isgood = false;
                    }
                    else
                    {
                        str = "Run BLIND is PASS , Area is " + RUNBlindArea.ToString() + " in Range " + (tolerance * 100).ToString("0.00") + "%";
                        workstatus.ProcessString += str + Environment.NewLine;
                        workstatus.Reason = ReasonEnum.PASS;

                        isgood = true;
                    }
                }

                //workstatus.SetWorkStatus(bmpPattern, bmpInput, bmpPattern, reason, errorstring, processstring, PassInfo);
                //RunStatusCollection.Add(workstatus);
            }

            if (IsTempSave)//|| true)
            {
                bmpoutput.Save(Universal.TESTPATH + "\\ANALYZETEST\\OUTPUT-" + RelateAnalyzeString + (istrain ? "-T" : "-R") + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            }


            return isgood;
        }
        #endregion

        #region MB Check Fuctions

        enum MBSliceTypeEnum
        {
            HEAD,
            TAIL,

            NORMAL,
        }
        class MBSliceClass
        {
            public MBSliceTypeEnum MBSliceType = MBSliceTypeEnum.NORMAL;
            int MBWidth = 0;
            float _MB_WidthThresholdRatio = 0.3f;
            int _MB_CutLeftWidth = 10;
            int _MB_CutRightWidth = 10;
            float _MB_CheckRatio = 0.3f;
            public bool _MB_IsPass = false;

            public MBSliceClass(MBSliceTypeEnum mbslicetype, Bitmap bmp, bool iswidth,
                                                 float widththresholdradio, int cutleftwidth, int cutrightwidth, float checkratio)
            {
                MBSliceType = mbslicetype;
                _MB_WidthThresholdRatio = widththresholdradio;
                _MB_CutLeftWidth = cutleftwidth;
                _MB_CutRightWidth = cutrightwidth;
                _MB_CheckRatio = checkratio;

                GetBMPAnalyze(bmp, iswidth, true);
            }

            public bool GetBMPAnalyze(Bitmap bmp, bool iswidth, bool istrain = false)
            {
                bool ispass = true;

                int i = 0;

                QvLineFit ltline = new QvLineFit();
                QvLineFit rbline = new QvLineFit();

                PointF[] ltpoints;
                PointF[] rbpoints;

                JzFindObjectClass jzfind = new JzFindObjectClass();
                HistogramClass histogram = new HistogramClass(2);

                List<PointF> ltptlist = new List<PointF>();
                List<PointF> rbptlist = new List<PointF>();
                List<int> lenlist = new List<int>();

                DataHistogramClass datahistogram = new DataHistogramClass(1000, 2);

                histogram.GetHistogram(bmp);

                int mean = histogram.MeanGrade;
                int min = histogram.MinGrade;
                int max = histogram.MaxGrade;
                int mode = histogram.ModeGrade;

                //jzfind.SetThreshold(bmp, SimpleRect(bmp.Size, 1), min, mean - min, 255, true);
                jzfind.SetThreshold(bmp, SimpleRect(bmp.Size, 1), min, ((255 + mean) >> 1) - min, 255, true);

                jzfind.GetInnerLineFromBorder(bmp, iswidth, 2, ref ltptlist, ref rbptlist, ref lenlist);

                bmp.Save(Universal.TESTPATH + "\\ANALYZETEST\\INNER LINE1 " + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                //Get Length Mode Value
                List<int> RemoveList = new List<int>();
                i = 0;
                foreach (int len in lenlist)
                {
                    if (len > 1)
                    {
                        datahistogram.Add(len);
                    }
                    else
                    {
                        RemoveList.Add(i);
                    }
                    i++;
                }
                datahistogram.Complete();



                if (istrain)
                {
                    MBWidth = datahistogram.ModeGrade;
                }
                else
                {
                    //先檢測是否為相同寬度的30%
                    if (!IsInRangeRatio(datahistogram.ModeGrade, MBWidth, _MB_WidthThresholdRatio * 100))
                    {
                        ispass = false;
                    }

                    //再檢測裏面是否被刮傷
                    if (MBSliceType == MBSliceTypeEnum.NORMAL)
                    {
                        //求出上下兩條線
                        RemoveList.Sort();
                        RemoveList.Reverse();

                        i = RemoveList.Count - 1;
                        while (i > 0)
                        {
                            rbptlist.RemoveAt(i);
                            ltptlist.RemoveAt(i);

                            i--;
                        }

                        ltpoints = new PointF[ltptlist.Count];
                        rbpoints = new PointF[rbptlist.Count];

                        i = 0;
                        while (i < ltptlist.Count)
                        {
                            ltpoints[i] = ltptlist[i];
                            rbpoints[i] = rbptlist[i];

                            i++;
                        }

                        ltline.LeastSquareFit(ltpoints);
                        rbline.LeastSquareFit(rbpoints);
                        ////

                        //在圖上畫出兩條寬為2的線
                        //LT Line 兩端點為
                        PointF PT1 = new PointF(0, (float)ltline.B);
                        PointF PT2 = new PointF(bmp.Width, (float)(ltline.A * (double)bmp.Width + ltline.B));

                        //RB Line 兩端點為
                        PointF PT3 = new PointF(0, (float)rbline.B);
                        PointF PT4 = new PointF(bmp.Width, (float)(rbline.A * (double)bmp.Width + rbline.B));

                        //把兩條線畫上去
                        DrawLine(bmp, new Pen(Color.White, 2), PT1, PT2);
                        DrawLine(bmp, new Pen(Color.White, 2), PT3, PT4);

                        //去頭去尾多少可設定為參數
                        int CutLeftWidth = _MB_CutLeftWidth;
                        int CutRightWidth = _MB_CutRightWidth;

                        Rectangle leftrect = new Rectangle(0, 0, CutLeftWidth, bmp.Height);
                        Rectangle rightrect = new Rectangle(bmp.Width - CutRightWidth, 0, CutRightWidth, bmp.Height);

                        FillRect(bmp, leftrect, new SolidBrush(Color.White));
                        FillRect(bmp, rightrect, new SolidBrush(Color.White));

                        bmp.Save(Universal.TESTPATH + "\\ANALYZETEST\\INNER LINE2 " + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);


                        jzfind.Reverse(bmp);
                        jzfind.Find(bmp, Color.Red);

                        bmp.Save(Universal.TESTPATH + "\\ANALYZETEST\\INNER LINE3 " + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);


                        //檢查位於兩線之間找到東西的高度有沒有大於真實高度的1/3
                        i = 0;
                        while (i < jzfind.FoundList.Count)
                        {
                            FoundClass found = jzfind.FoundList[i];

                            float lty = (float)(ltline.A * found.Center.X + ltline.B);
                            float rby = (float)(rbline.A * found.Center.X + rbline.B);

                            if (lty <= found.Center.Y && rby >= found.Center.Y)
                            {
                                if (found.Height > (int)((float)datahistogram.ModeGrade * _MB_CheckRatio))
                                // if (found.Height > (int)((float)datahistogram.ModeGrade / 3))
                                {
                                    ispass = false;
                                    break;
                                }
                            }
                            i++;
                        }
                    }
                }

                _MB_IsPass = ispass;

                return ispass;
            }


            Rectangle SimpleRect(Size sz, int devide)
            {
                return new Rectangle(0, 0, sz.Width / devide, sz.Height / devide);
            }
            bool IsInRangeRatio(double runvalue, double orgvalue, double ratio)
            {
                return (runvalue >= (orgvalue * (1 - (ratio / 100d)))) && (runvalue <= (orgvalue * (1 + (ratio / 100d))));
            }

            void DrawLine(Bitmap bmp, Pen p, PointF fromptf, PointF toptf)
            {
                Graphics g = Graphics.FromImage(bmp);
                g.DrawLine(p, fromptf, toptf);
                g.Dispose();
            }
            void FillRect(Bitmap bmp, Rectangle rect, SolidBrush b)
            {
                Graphics g = Graphics.FromImage(bmp);
                g.FillRectangle(b, rect);
                g.Dispose();
            }
        }

        int MB_UDRange = 10;
        int MB_HeightThreshold = 10;
        bool MB_IsWidth = false;

        float MB_WidthThresholdRatio = 0.3f;
        int MB_CutLeftWidth = 10;
        int MB_CutRightWidth = 10;
        float MB_CheckRatio = 0.3f;

        int MBCCount = 0;
        List<MBSliceClass> MBSliceList = new List<MBSliceClass>();

        bool MBCheck(Bitmap bmpinput, Bitmap bmpmask, ref Bitmap bmpoutput, bool istrain, WorkStatusClass workstatus)
        {
            const float diffratio = 0.2f;
            const float threadratio = 0.3f;

            bool isgood = true;
            bool issomthing = false;

            Bitmap bmp = new Bitmap(1, 1);

            string str = "";

            string[] strinputMBPara = MMOPString.Split(',');
            if (strinputMBPara.Length >= 3)
            {
                MB_UDRange = int.Parse(strinputMBPara[0]);
                MB_HeightThreshold = int.Parse(strinputMBPara[1]);
                MB_IsWidth = int.Parse(strinputMBPara[2]) == 1;
            }
            if (strinputMBPara.Length >= 7)
            {
                MB_WidthThresholdRatio = float.Parse(strinputMBPara[3]);
                MB_CutLeftWidth = int.Parse(strinputMBPara[4]);
                MB_CutRightWidth = int.Parse(strinputMBPara[5]);
                MB_CheckRatio = float.Parse(strinputMBPara[6]);
            }

            //IsTempSave = true;

            //WorkStatusClass workstatus = new WorkStatusClass(AnanlyzeProcedureEnum.MEASURETRAIN);
            if (istrain)
                workstatus.ProcessString += "Start " + RelateAnalyzeString + " MB Train." + Environment.NewLine;
            else
                workstatus.ProcessString += "Start " + RelateAnalyzeString + " MB Run." + Environment.NewLine;
            //string errorstring = "";
            //ReasonEnum reason = ReasonEnum.PASS;

            HistogramClass histogram = new HistogramClass(2);
            JzFindObjectClass jzfind = new JzFindObjectClass();

            if (IsTempSave)
            {
                bmpinput.Save(Universal.TESTPATH + "\\ANALYZETEST\\MBCHECK INPUT-" + RelateAnalyzeString + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                bmpmask.Save(Universal.TESTPATH + "\\ANALYZETEST\\MBCHECK MASK-" + RelateAnalyzeString + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            }

            bmp.Dispose();
            bmp = new Bitmap(bmpinput);

            histogram.GetHistogram(bmpinput, bmpmask, true);

            int mean = histogram.MeanGrade;
            int min = histogram.MinGrade;
            int max = histogram.MaxGrade;
            int mode = histogram.ModeGrade;

            int maxdiff = (int)((float)(max - mode) * threadratio);
            int mindiff = (int)((float)(mode - min) * threadratio);

            if (istrain)
            {
                MBCCount = 0;

                bmpPattern.Dispose();
                bmpPattern = new Bitmap(bmpinput);

                bmpoutput.Dispose();
                bmpoutput = new Bitmap(bmpinput);
                //jzfind.SetThreshold(bmpoutput, SimpleRect(bmpinput.Size, 1), mindiff, (int)(mindiff * 0.5), mindiff, true);
                //jzfind.SetThreshold(bmpoutput, SimpleRect(bmpinput.Size, 1), min, mean - min, 255, true);
                jzfind.SetThresholdEX(bmpoutput, bmpmask, mean, 0, true, true);

                if (IsTempSave)
                {
                    bmpoutput.Save(Universal.TESTPATH + "\\ANALYZETEST\\MBCHECK THRESHOLD-" + RelateAnalyzeString + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                }

                MBCheckSub(bmpinput, bmpoutput, MB_UDRange, MB_HeightThreshold, istrain, ref isgood);

                workstatus.Reason = ReasonEnum.PASS;
                isgood = true;
                //workstatus.SetWorkStatus(bmpPattern, bmpinput, bmpPattern, reason, errorstring, processstring, PassInfo);

                //TrainStatusCollection.Add(workstatus);

            }
            else
            {
                bmpInput.Dispose();
                bmpInput = new Bitmap(bmpinput);

                bmpoutput.Dispose();
                bmpoutput = new Bitmap(bmpinput);

                //jzfind.SetThreshold(bmpoutput, SimpleRect(bmpinput.Size, 1), mindiff, (int)(mindiff * 0.5), mindiff, true);
                //jzfind.SetThreshold(bmpoutput, SimpleRect(bmpinput.Size, 1), min, mean - min, 255, true);
                jzfind.SetThresholdEX(bmpoutput, bmpmask, mean, 0, true, true);
                //jzfind.SetThresholdEX(bmpoutput, bmpmask, SimpleRect(bmpinput.Size, 1), mean, 0, true, true);

                if (IsTempSave)
                {
                    bmpoutput.Save(Universal.TESTPATH + "\\ANALYZETEST\\MBCHECK RUN THRESHOLD-" + RelateAnalyzeString + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                }

                MBCheckSub(bmpinput, bmpoutput, MB_UDRange, MB_HeightThreshold, istrain, ref isgood);

                if (!isgood)
                {
                    str = "Run MB is NG";
                    workstatus.ProcessString += str + Environment.NewLine;
                    workstatus.Reason = ReasonEnum.NG;

                    //isgood = false;
                }
                else
                {
                    str = "Run MB is PASS ";
                    workstatus.ProcessString += str + Environment.NewLine;
                    workstatus.Reason = ReasonEnum.PASS;

                    //isgood = true;
                }

#if(OPT_BLIND_OLD)
                if (ORGBlindArea == 0 && issomthing)
                {
                    str = "MB Check Error (Nothing To Something).";
                    workstatus.ProcessString += str + Environment.NewLine;
                    workstatus.Reason = ReasonEnum.NG;

                    isgood = false;
                }
                else if (ORGBlindArea != 0 && !issomthing)
                {
                    str = "MB Check Error (Something To Nothing).";
                    workstatus.ProcessString += str + Environment.NewLine;
                    workstatus.Reason = ReasonEnum.NG;

                    isgood = false;
                }
                else
                {
                    jzfind.SetThresholdEX(bmpoutput, bmpmask, 255, mode + maxdiff, mode - mindiff, false, false);
                    //jzfind.SetThresholdEX(bmpoutput, SimpleRect(bmpoutput.Size, 1), mode + maxdiff, mode - mindiff, false, false);

                    //jzfind.SetThreshold(bmpoutput, SimpleRect(bmpoutput.Size, 1), min, (int)((float)(mode - min) * threadratio), 255, true);

                    jzfind.Find(bmpoutput, Color.Red);

                    int filterarea = jzfind.GetMaxArea();
                    filterarea = (int)((float)filterarea * 0.3f);

                    Rectangle rect = jzfind.GetRect(true, filterarea);

                    DrawRect(bmpoutput, rect, new Pen(Color.Lime));

                    RUNBlindArea = rect.Width * rect.Height;

                    if (!IsInRangeRatio((double)RUNBlindArea, (double)ORGBlindArea, (double)MMTolerance))
                    {
                        str = "Run MB is NG , Area is " + RUNBlindArea.ToString() + " not in Range " + MMTolerance.ToString("0.00") + "%";
                        workstatus.ProcessString += str + Environment.NewLine;
                        workstatus.Reason = ReasonEnum.NG;

                        isgood = false;
                    }
                    else
                    {
                        str = "Run MB is PASS , Area is " + RUNBlindArea.ToString() + " in Range " + MMTolerance.ToString("0.00") + "%";
                        workstatus.ProcessString += str + Environment.NewLine;
                        workstatus.Reason = ReasonEnum.PASS;

                        isgood = true;
                    }
                }
#endif

                //workstatus.SetWorkStatus(bmpPattern, bmpInput, bmpPattern, reason, errorstring, processstring, PassInfo);
                //RunStatusCollection.Add(workstatus);

            }

            if (IsTempSave)
            {
                bmpoutput.Save(Universal.TESTPATH + "\\ANALYZETEST\\OUTPUT-" + RelateAnalyzeString + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            }


            bmp.Dispose();

            return isgood;
        }

        /// <summary>
        /// MB 檢查副程式
        /// </summary>
        /// <param name="bmporg"></param>
        /// <param name="bmpthreshed"></param>
        /// <param name="udrange"></param>
        void MBCheckSub(Bitmap bmporg, Bitmap bmpthreshed, int udrange, int heightthreshold, bool istrain, ref bool ispass)
        {
            int i = 0;
            int j = 0;

            Bitmap bmp = new Bitmap(1, 1);

            JzFindObjectClass jzfind = new JzFindObjectClass();

            jzfind.Find(bmpthreshed, Color.Red);

            List<Rectangle> rectlist = new List<Rectangle>();

            while (i < jzfind.Count)
            {
                FoundClass findi = jzfind.FoundList[i];

                if (findi.IsChecked)
                {
                    i++;
                    continue;
                }

                Rectangle rect = findi.rect;

                j = 0;
                while (j < jzfind.Count)
                {
                    FoundClass findj = jzfind.FoundList[j];

                    if (findj.IsChecked || j == i)
                    {
                        j++;
                        continue;
                    }
                    if ((findi.rect.Y < findj.rect.Y && findi.rect.Bottom > findj.rect.Bottom)
                        || IsInRange(findi.Center.Y, findj.Center.Y, udrange))

                    {
                        rect = MergeTwoRects(rect, findj.rect);
                        findj.IsChecked = true;
                    }

                    j++;
                }

                rectlist.Add(rect);
                i++;
            }

            //清掉過小的雜訊
            i = rectlist.Count - 1;
            while (i >= 0)
            {
                if (rectlist[i].Height < heightthreshold)
                {
                    rectlist.RemoveAt(i);
                }
                i--;
            }

            //先檢查個數是不是相同
            if (istrain)
            {
                MBCCount = rectlist.Count;
                MBSliceList.Clear();
            }
            else
            {
                if (MBCCount != rectlist.Count)
                {
                    ispass = false;
                    bmp.Dispose();

                    return;
                }
            }

            //Sorting Rectangles From Up To Down
            List<string> SortingList = new List<string>();

            i = 0;
            foreach (Rectangle rect in rectlist)
            {
                string str = rect.Y.ToString("000000") + "," + i.ToString("000");
                SortingList.Add(str);
                i++;
            }

            SortingList.Sort();

            i = 0;
            foreach (string sortstr in SortingList)
            {
                //HistogramClass histogram = new HistogramClass(2);

                Rectangle rect = rectlist[int.Parse(sortstr.Split(',')[1])];

                DrawRect(bmpthreshed, rect, new Pen(Color.Lime, 2));

                bmp.Dispose();
                bmp = (Bitmap)bmporg.Clone(rect, PixelFormat.Format32bppArgb);

                //histogram.GetHistogram(bmp);

                //int mean = histogram.MeanGrade;
                //int min = histogram.MinGrade;
                //int max = histogram.MaxGrade;
                //int mode = histogram.ModeGrade;

                //jzfind.SetThreshold(bmp, SimpleRect(bmp.Size, 1), min, mean - min, 255, true);

                //if (IsTempSave)
                //{
                //    bmp.Save(Universal.TESTPATH + "\\ANALYZETEST\\MBCHECK SLICE- " + i.ToString("00") + " -" + RelateAnalyzeString + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                //}

                MBSliceClass mbslice;

                if (istrain)
                {
                    //MBSliceList.Clear();

                    if (i == 0)
                    {
                        mbslice = new MBSliceClass(MBSliceTypeEnum.HEAD, bmp, MB_IsWidth,
                                                                            MB_WidthThresholdRatio, MB_CutLeftWidth, MB_CutRightWidth, MB_CheckRatio);
                    }
                    else if (i == rectlist.Count - 1)
                    {
                        mbslice = new MBSliceClass(MBSliceTypeEnum.TAIL, bmp, MB_IsWidth,
                                                                            MB_WidthThresholdRatio, MB_CutLeftWidth, MB_CutRightWidth, MB_CheckRatio);
                        //mbslice._MB_WidthThresholdRatio = MB_WidthThresholdRatio;
                        //mbslice._MB_CutLeftWidth = MB_CutLeftWidth;
                        //mbslice._MB_CutRightWidth = MB_CutRightWidth;
                    }
                    else
                    {
                        mbslice = new MBSliceClass(MBSliceTypeEnum.NORMAL, bmp, MB_IsWidth,
                                                                            MB_WidthThresholdRatio, MB_CutLeftWidth, MB_CutRightWidth, MB_CheckRatio);
                    }

                    MBSliceList.Add(mbslice);
                }
                else
                {
                    mbslice = MBSliceList[i];

                    if (MBSliceList[i].MBSliceType == MBSliceTypeEnum.NORMAL)
                        ispass &= mbslice.GetBMPAnalyze(bmp, MB_IsWidth, false);
                    else
                        mbslice.GetBMPAnalyze(bmp, MB_IsWidth, false);

                    if (!mbslice._MB_IsPass)
                        DrawRect(bmpthreshed, rect, new Pen(Color.Blue, 2));

                    //若NG的話就跳出來吧
                    //if (!ispass)
                    //    break;
                }
                i++;
            }

            if (!istrain)
            {
                if (ispass)
                {
                    if (!(MBSliceList[0]._MB_IsPass || MBSliceList[MBSliceList.Count - 1]._MB_IsPass))
                    {
                        ispass = false;
                    }
                }
            }

            if (IsTempSave)
            {
                bmpthreshed.Save(Universal.TESTPATH + "\\ANALYZETEST\\MBCHECK FOUND-" + RelateAnalyzeString + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            }

            bmp.Dispose();
        }



        #endregion

        #region Color Measurement Function

        ColorMeasureClass MyColorMeasure = new ColorMeasureClass();
        bool ColorMeasure(Bitmap bmpinput, Bitmap bmpmask, ref Bitmap bmpoutput, bool istrain, WorkStatusClass workstatus, PassInfoClass passinfo)
        {
            bool isgood = false;
            string str = "";

            if (istrain)
            {
                MyColorMeasure = new ColorMeasureClass(MMOPString);
                str = "Bypass Color Check " + MyColorMeasure.colortype.ToString() + " for " + MyColorMeasure.colormethod.ToString() + " Origin.";
                workstatus.ProcessString += str + Environment.NewLine;
                workstatus.Reason = ReasonEnum.PASS;
                isgood = true;
            }
            else
            {
                if (MyColorMeasure.CheckColor(bmpinput, passinfo))
                {
                    str = "COLOR Check OK.";
                    workstatus.ProcessString += str + Environment.NewLine;
                    workstatus.Reason = ReasonEnum.PASS;

                    isgood = true;
                }
                else
                {
                    str = "COLOR Check NG.";
                    workstatus.ProcessString += str + Environment.NewLine;
                    workstatus.Reason = ReasonEnum.NG;

                    isgood = false;

                }

            }
            return isgood;

        }

        #endregion

        #region SolderBall Measurement Function

        SolderBallMeasureClass MySolderBallMeasure = new SolderBallMeasureClass();
        bool SolderBallMeasure(Bitmap bmpinput, Bitmap bmpmask, ref Bitmap bmpoutput, bool istrain, WorkStatusClass workstatus, PassInfoClass passinfo)
        {
            bool isgood = false;
            string str = "";

            if (istrain)
            {
                MySolderBallMeasure = new SolderBallMeasureClass(MMOPString);
                //str = "Bypass Solder Check " + " Origin.";
                //workstatus.ProcessString += str + Environment.NewLine;
                //workstatus.Reason = ReasonEnum.PASS;
                //isgood = true;

                if (MySolderBallMeasure.CheckSolderBall(bmpinput, bmpmask, ref bmpoutput, true, workstatus, passinfo))
                {
                    str = "SolderBall train OK.";
                    workstatus.ProcessString += str + Environment.NewLine;
                    workstatus.Reason = ReasonEnum.PASS;

                    isgood = true;
                }
                else
                {
                    str = "SolderBall train NG.";
                    workstatus.ProcessString += str + Environment.NewLine;
                    workstatus.Reason = ReasonEnum.NG;

                    isgood = false;

                }

            }
            else
            {
                if (MySolderBallMeasure.CheckSolderBall(bmpinput, bmpmask, ref bmpoutput, false, workstatus, passinfo))
                {
                    str = "SolderBall Check OK.";
                    workstatus.ProcessString += str + Environment.NewLine;
                    workstatus.Reason = ReasonEnum.PASS;

                    isgood = true;
                }
                else
                {
                    str = "SolderBall Check NG.";
                    workstatus.ProcessString += str + Environment.NewLine;
                    workstatus.Reason = ReasonEnum.NG;

                    isgood = false;

                }

            }
            return isgood;

        }

        #endregion

        #endregion



        public override string ToString()
        {
            string str = "";

            str += ((int)MeasureMethod).ToString() + Universal.SeperateCharB;   //0
            str += MMTolerance.ToString() + Universal.SeperateCharB;            //1
            str += MMOPString.ToString() + Universal.SeperateCharB;             //2
            str += MMMaxGap.ToString() + Universal.SeperateCharB;               //3
            str += MMMinGap.ToString() + Universal.SeperateCharB;               //4
            str += MMPixelGap.ToString() + Universal.SeperateCharB;             //5
            str += MMHTRatio.ToString() + Universal.SeperateCharB;              //6
            str += MMWholeRatio.ToString() + Universal.SeperateCharB;           //7

            str += "";

            return str;
        }
        public void FromString(string str)
        {
            string[] strs = str.Split(Universal.SeperateCharB);

            MeasureMethod = (MeasureMethodEnum)int.Parse(strs[0]);
            MMTolerance = float.Parse(strs[1]);
            MMOPString = strs[2];
            MMMaxGap = float.Parse(strs[3]);
            MMMinGap = float.Parse(strs[4]);
            MMPixelGap = int.Parse(strs[5]);
            MMHTRatio = float.Parse(strs[6]);
            MMWholeRatio = float.Parse(strs[7]);
        }

        public void FromPropertyChange(string changeitemstring, string valuestring)
        {
            string[] str = changeitemstring.Split(';');

            if (str[0] != "04.Measure")
                return;

            switch (str[1])
            {
                //case "MeasureMethod":
                //    MeasureMethod = (MeasureMethodEnum)Enum.Parse(typeof(MeasureMethodEnum), valuestring, true);
                //    break;
                case "MMTolerance":
                    MMTolerance = float.Parse(valuestring);
                    break;
                case "MMOPString":
                    MMOPString = valuestring.Split('#')[1];
                    MeasureMethod = (MeasureMethodEnum)Enum.Parse(typeof(MeasureMethodEnum), valuestring.Split('#')[0], true);
                    break;
                case "MMMaxGap":
                    MMMaxGap = float.Parse(valuestring);
                    break;
                case "MMMinGap":
                    MMMinGap = float.Parse(valuestring);
                    break;
                case "MMPixelGap":
                    MMPixelGap = int.Parse(valuestring);
                    break;
                case "MMHTRatio":
                    MMHTRatio = float.Parse(valuestring);
                    break;
                case "MMWholeRatio":
                    MMWholeRatio = float.Parse(valuestring);
                    break;
            }
        }
        public void Suicide()
        {
            bmpPattern.Dispose();
            bmpInput.Dispose();

            TrainStatusCollection.Clear();
            RunStatusCollection.Clear();
        }
        public void Reset()
        {
            MeasureMethod = MeasureMethodEnum.NONE;
            MMTolerance = 10;
            MMOPString = "";
            MMMaxGap = 0f;
            MMMinGap = 0f;
            MMPixelGap = 5;
            MMHTRatio = 10f;
            MMWholeRatio = 10f;
        }
        /// <summary>
        /// 在做大量運算前要清除的相關資料
        /// </summary>
        public void ResetTrainStatus()
        {
            TrainStatusCollection.Clear();
            //CheckGood = true;
        }
        /// <summary>
        /// 在做大量運算前要清除的相關資料
        /// </summary>
        public void ResetRunStatus()
        {
            RunStatusCollection.Clear();
            //CheckGood = true;
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
        public void FillRunStatus(WorkStatusCollectionClass runstatuscollection, string filltoanalyzestr)
        {
            foreach (WorkStatusClass runstatus in RunStatusCollection.WorkStatusList)
            {
                if (filltoanalyzestr == null)
                {
                    if (runstatus.LogString == "")
                    {
                        runstatuscollection.Add(runstatus);
                    }
                }
                else
                {
                    if (runstatus.LogString.IndexOf(filltoanalyzestr) < 0)
                    {
                        runstatus.LogString += filltoanalyzestr;
                        runstatuscollection.Add(runstatus);
                    }
                }
            }
        }

        /// <summary>
        /// 將產生出來的過程寫出去
        /// </summary>
        /// <param name="trainstatuscollection"></param>
        public void FillTrainStatus(WorkStatusCollectionClass trainstatuscollection)
        {
            foreach (WorkStatusClass trainstatus in TrainStatusCollection.WorkStatusList)
            {
                trainstatuscollection.Add(trainstatus);
            }
        }
        public void FillTrainStatus(WorkStatusCollectionClass trainstatuscollection, string filltoanalyzestr)
        {
            foreach (WorkStatusClass trainstatus in TrainStatusCollection.WorkStatusList)
            {
                if (filltoanalyzestr == null)
                {
                    if (trainstatus.LogString == "")
                    {
                        trainstatuscollection.Add(trainstatus);
                    }
                }
                else
                {
                    if (trainstatus.LogString.IndexOf(filltoanalyzestr) < 0)
                    {
                        trainstatus.LogString += filltoanalyzestr;
                        trainstatuscollection.Add(trainstatus);
                    }
                }
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

        #region Tools Operation

        bool IsInRangeRatio(double runvalue, double orgvalue, double ratio)
        {
            return (runvalue >= (orgvalue * (1 - ratio))) && (runvalue <= (orgvalue * (1 + ratio)));
        }
        public bool IsInRange(float FromValue, float CompValue, float DiffValue)
        {
            return Math.Abs(FromValue - CompValue) < DiffValue;
        }
        Rectangle SimpleRect(Size sz, int devide)
        {
            return new Rectangle(0, 0, sz.Width / devide, sz.Height / devide);
        }
        void SetBrightContrast(Bitmap bmp, int brightvalue, int contrastvalue)
        {
            SetBrightContrast(bmp, SimpleRect(bmp.Size, 1), brightvalue, contrastvalue);
        }
        void SetBrightContrast(Bitmap bmp, Rectangle rect, int brightvalue, int contrastvalue)
        {
            if (brightvalue == 0 && contrastvalue == 0)
                return;

            int Grade = 0;
            double contrast = (100.0 + contrastvalue) / 100.0;
            contrast *= contrast;

            double ContrastGrade = 0;

            Rectangle rectbmp = rect;
            BitmapData bmpData = bmp.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr Scan0 = bmpData.Scan0;

            try
            {
                unsafe
                {
                    byte* scan0 = (byte*)(void*)Scan0;
                    byte* pucPtr;
                    byte* pucStart;

                    int xmin = rectbmp.X;
                    int ymin = rectbmp.Y;
                    int xmax = xmin + rectbmp.Width;
                    int ymax = ymin + rectbmp.Height;

                    int x = xmin;
                    int y = ymin;
                    int iStride = bmpData.Stride;

                    y = ymin;
                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));

                    while (y < ymax)
                    {
                        x = xmin;
                        pucPtr = pucStart;
                        while (x < xmax)
                        {
                            Grade = (int)pucPtr[2];

                            if (brightvalue != 0)
                            {
                                Grade += brightvalue;
                                Grade = Math.Min(255, Math.Max(0, Grade));
                            }

                            ContrastGrade = (double)Grade;

                            if (contrastvalue != 0)
                            {
                                ContrastGrade /= 255d;
                                ContrastGrade -= 0.5;
                                ContrastGrade *= (double)contrast;
                                ContrastGrade += 0.5;
                                ContrastGrade *= 255;

                                ContrastGrade = Math.Min(255, Math.Max(0, ContrastGrade));
                            }

                            pucPtr[2] = (byte)ContrastGrade;
                            pucPtr[1] = (byte)ContrastGrade;
                            pucPtr[0] = (byte)ContrastGrade;

                            pucPtr += 4;
                            x++;
                        }

                        pucStart += iStride;
                        y++;
                    }

                    bmp.UnlockBits(bmpData);
                }
            }
            catch (Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
                string Str = ex.ToString();

                bmp.UnlockBits(bmpData);
            }
        }

        void DrawRect(Bitmap bmp, Rectangle rect, Pen roundpen)
        {
            Graphics g = Graphics.FromImage(bmp);
            g.DrawRectangle(roundpen, rect);
            g.Dispose();
        }

        Rectangle MergeTwoRects(Rectangle rect1, Rectangle rect2)
        {
            Rectangle rect = new Rectangle();

            if (rect1.Width == 0)
                return rect2;
            if (rect2.Width == 0)
                return rect1;

            rect.X = Math.Min(rect1.X, rect2.X);
            rect.Y = Math.Min(rect1.Y, rect2.Y);

            rect.Width = Math.Max(rect1.X + rect1.Width, rect2.X + rect2.Width) - rect.X;
            rect.Height = Math.Max(rect1.Y + rect1.Height, rect2.Y + rect2.Height) - rect.Y;

            return rect;
        }

        #endregion




    }
}
