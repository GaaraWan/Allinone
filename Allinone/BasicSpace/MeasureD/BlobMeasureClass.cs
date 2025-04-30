using JetEazy.BasicSpace;
using JetEazy.UISpace;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Allinone.OPSpace;
using JetEazy;
using FreeImageAPI;
//using System.Windows.Shapes;

namespace Allinone.BasicSpace.MeasureD
{

    public enum BlobFun
    {
        V1 = 0,
        G1 = 1,
    }

    public class MBlobParaPropertyGridClass
    {
        public MBlobParaPropertyGridClass()
        {

        }
        const string cat0 = "A00.图像设定";

        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("1.灰阶阈值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 255)]
        [Browsable(true)]
        public int chkThresholdValue { get; set; } = 128;
        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("2.寻找方式")]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 255)]
        [Browsable(true)]
        [TypeConverter(typeof(JzEnumConverter))]
        public BlobMode chkblobmode { get; set; } = BlobMode.White;

        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("3.过滤最小面积")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 255)]
        [Browsable(true)]
        public int chkMinArea { get; set; } = 5;

        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("4.过滤长度")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 255)]
        [Browsable(true)]
        public int chkMinWidth { get; set; } = 5;

        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("5.过滤宽度")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 255)]
        [Browsable(true)]
        public int chkMinHeight { get; set; } = 5;

        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("6.横向外扩")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 255)]
        [Browsable(true)]
        public int extendx { get; set; } = 5;

        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("7.纵向外扩")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 255)]
        [Browsable(true)]
        public int extendy { get; set; } = 5;

        [CategoryAttribute(cat0), DescriptionAttribute("")]
        [DisplayName("8.边缘处理次数")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 255)]
        [Browsable(true)]
        public int erosionCount { get; set; } = 1;


        const string cat1 = "A01.检测规格";

        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("1.寻找方式")]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 255)]
        [Browsable(true)]
        [TypeConverter(typeof(JzEnumConverter))]
        public BlobFun chkBlobsFun { get; set; } = BlobFun.V1;

        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("2.PIN重合比例")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100)]
        [Browsable(true)]
        public double checkarearatio { get; set; } = 70;

        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("3.PIN面积比例")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100)]
        [Browsable(true)]
        public double checkAreaResultRatio { get; set; } = 70;

        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("5.PIN最大颗面积比例")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100)]
        [Browsable(true)]
        public double checkAreaMaxResultRatio { get; set; } = 30;

        [CategoryAttribute(cat1), DescriptionAttribute("")]
        [DisplayName("4.是否判断最大颗的比例")]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 100)]
        [Browsable(true)]
        public bool ischeckMaxRatio { get; set; } = false;


        public void FromingStr(string str)
        {
            string[] parts = str.Split(',');
            if (parts.Length > 3)
            {
                chkThresholdValue = int.Parse(parts[0]);
                chkblobmode = (BlobMode)int.Parse(parts[1]);
                chkMinArea = int.Parse(parts[2]);
                checkarearatio = double.Parse(parts[3]);
            }
            if (parts.Length > 5)
            {
                checkAreaResultRatio = double.Parse(parts[4]);
            }
            if (parts.Length > 10)
            {
                chkMinWidth = int.Parse(parts[5]);
                chkMinHeight = int.Parse(parts[6]);
                extendx = int.Parse(parts[7]);
                extendy = int.Parse(parts[8]);
                erosionCount = int.Parse(parts[9]);
                chkBlobsFun = (BlobFun)int.Parse(parts[10]);
            }
            if (parts.Length > 12)
            {
                ischeckMaxRatio = parts[11] == "1";
                checkAreaMaxResultRatio = double.Parse(parts[12]);
            }
        }
        public string ToParaString()
        {
            string str = string.Empty;

            str += chkThresholdValue.ToString() + ",";
            str += ((int)chkblobmode).ToString() + ",";
            str += chkMinArea.ToString() + ",";
            str += checkarearatio.ToString() + ",";
            str += checkAreaResultRatio.ToString() + ",";

            str += chkMinWidth.ToString() + ",";
            str += chkMinHeight.ToString() + ",";
            str += extendx.ToString() + ",";
            str += extendy.ToString() + ",";
            str += erosionCount.ToString() + ",";
            str += ((int)chkBlobsFun).ToString() + ",";
            str += (ischeckMaxRatio ? "1" : "0") + ",";
            str += checkAreaMaxResultRatio.ToString() + ",";

            return str;
        }

        /// <summary>
        /// 训练的所有blob记录下来
        /// </summary>
        public List<FoundClass> ClassList = new List<FoundClass>();
        public List<FoundClass> ClassRunList = new List<FoundClass>();
        public FoundClass MaxFound = new FoundClass();
        public FoundClass MaxFoundRun = new FoundClass();
    }
    public class BlobMeasureClass
    {
        MBlobParaPropertyGridClass mblobParaPropertyGrid = new MBlobParaPropertyGridClass();
        //Bitmap bmpInput = new Bitmap(1, 1);
        JzToolsClass m_JzTools = new JzToolsClass();
        JzFindObjectClass m_JzFind = new JzFindObjectClass();

        public BlobMeasureClass(string str)
        {
            FromString(str);
        }
        public BlobMeasureClass()
        {

        }

        public bool CheckBlobs(Bitmap bmpinput, Bitmap bmpmask, ref Bitmap bmpoutput, bool istrain, WorkStatusClass workstatus, PassInfoClass passinfo)
        {
            bool flag = false;
            switch (mblobParaPropertyGrid.chkBlobsFun)
            {
                case BlobFun.V1:
                    flag = CheckBlobsV1(bmpinput, bmpmask, ref bmpoutput, istrain, workstatus, passinfo);
                    break;
                case BlobFun.G1:
                    flag = CheckBlobsG1(bmpinput, bmpmask, ref bmpoutput, istrain, workstatus, passinfo);
                    break;
            }
            return flag;
        }

        bool CheckBlobsG1(Bitmap bmpinput, Bitmap bmpmask, ref Bitmap bmpoutput, bool istrain, WorkStatusClass workstatus, PassInfoClass passinfo)
        {
            bool ret = false;
            string str = "";
            //mblobParaPropertyGrid.MaxFound = new FoundClass();
            //mblobParaPropertyGrid.MaxFoundRun = new FoundClass();
            if (istrain)
                mblobParaPropertyGrid.ClassList.Clear();
            else
                mblobParaPropertyGrid.ClassRunList.Clear();

            //Bitmap bb = new Bitmap(bmpinput);
            //m_JzFind.GetMaskedImage(bb, bmpmask, Color.White, Color.Black);
            //bb.Save("D:\\LOA\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);

            //bmpInput.Dispose();
            //Bitmap bmpInput = new Bitmap(bmpinput);
            Bitmap bmpInput = (Bitmap)bmpinput.Clone();
            Rectangle myRect = m_JzTools.SimpleRect(bmpinput.Size);

            m_JzFind.GetMaskedImage(bmpInput, bmpmask, Color.White, Color.Black);

            //jzFindObjectClass.AH_SetThreshold(bmpinput, ref bmpInput, checkBaseParaPropertyGrid.chkThresholdValue);
            //m_JzFind.SetThresholdEX(bmpInput, myRect, 0, mblobParaPropertyGrid.chkThresholdValue, 0, !(mblobParaPropertyGrid.chkblobmode == BlobMode.White));

            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            bmpInput = grayscale.Apply(bmpInput);

            AForge.Imaging.Filters.Threshold threshold = new AForge.Imaging.Filters.Threshold(mblobParaPropertyGrid.chkThresholdValue);
            bmpInput = threshold.Apply(bmpInput);

            AForge.Imaging.Filters.BlobsFiltering blobsFiltering = new AForge.Imaging.Filters.BlobsFiltering();
            blobsFiltering.MinWidth = mblobParaPropertyGrid.chkMinWidth;
            blobsFiltering.MinHeight = mblobParaPropertyGrid.chkMinHeight;
            bmpInput = blobsFiltering.Apply(bmpInput);

            m_JzFind.AH_FindBlob(bmpInput, true);
            //m_JzFind.Find(bmpInput, Color.Red);
            m_JzFind.SortByArea();

            //int myArea = myRect.Width * myRect.Height;

            bmpoutput.Dispose();
            //bmpoutput = new Bitmap(bmpinput);
            bmpoutput = (Bitmap)bmpinput.Clone();
            Graphics gg = Graphics.FromImage(bmpoutput);


            List<Rectangle> list = new List<Rectangle>();
            list.Clear();
            int iCount = m_JzFind.FoundList.Count;
            if (iCount > 0)
            {
                if (istrain)
                {
                    foreach (FoundClass found in m_JzFind.FoundList)
                    {
                        if (found.Area > mblobParaPropertyGrid.chkMinArea)
                        {
                            mblobParaPropertyGrid.ClassList.Add(found);
                            //m_JzTools.DrawRectEx(bmpoutput, found.rect, new Pen(Color.Red));
                            //gg.DrawRectangle(new Pen(Color.Red, 3), found.rect);

                            list.Add(found.rect);
                        }
                    }
                    if (mblobParaPropertyGrid.ischeckMaxRatio)
                        mblobParaPropertyGrid.MaxFound = m_JzFind.FoundList[m_JzFind.GetMaxRectIndex()];
                    if (list.Count > 0)
                        gg.DrawRectangles(new Pen(Color.Red, 3), list.ToArray());
                    iCount = mblobParaPropertyGrid.ClassList.Count;
                    str = $"训练找到的个数{iCount}";
                }
                else
                {
                    foreach (FoundClass found in m_JzFind.FoundList)
                    {
                        if (found.Area > mblobParaPropertyGrid.chkMinArea)
                        {
                            mblobParaPropertyGrid.ClassRunList.Add(found);
                            //m_JzTools.DrawRectEx(bmpoutput, found.rect, new Pen(Color.Red));
                            list.Add(found.rect);
                        }
                    }
                    if (mblobParaPropertyGrid.ischeckMaxRatio)
                        mblobParaPropertyGrid.MaxFoundRun = m_JzFind.FoundList[m_JzFind.GetMaxRectIndex()];
                    iCount = mblobParaPropertyGrid.ClassRunList.Count;
                    str = $"RUN找到的个数{iCount}";
                }
                workstatus.ProcessString += str + Environment.NewLine;
            }

            if (istrain)
            {
                {
                    str = "train blobs is PASS ";
                    workstatus.ProcessString += str + Environment.NewLine;
                    workstatus.Reason = ReasonEnum.PASS;

                    ret = true;
                }
            }
            else
            {
                if (iCount != mblobParaPropertyGrid.ClassList.Count)
                {
                    str = $"Run blobs is NG , 数量错误{iCount}不等于{mblobParaPropertyGrid.ClassList.Count}";
                    workstatus.ProcessString += str + Environment.NewLine;
                    workstatus.Reason = ReasonEnum.NG;
                    workstatus.Desc = "PIN数量错误";
                    ret = false;

                    if (list.Count > 0)
                    {
                        gg.DrawRectangles(new Pen(Color.Red, 5), list.ToArray());
                        gg.DrawString(str, new Font("宋体", 11), new SolidBrush(Color.Red), new Point(5, 5));
                    }
                }
                else
                {

                    string strErr = string.Empty;
                    foreach (FoundClass foundorg in mblobParaPropertyGrid.ClassList)
                    {
                        bool bHave = false;
                        foreach (FoundClass foundrun in mblobParaPropertyGrid.ClassRunList)
                        {
                            Rectangle rectTempFind = foundrun.rect;
                            rectTempFind.Inflate(10, 0);

                            if (foundorg.rect.IntersectsWith(rectTempFind))
                            {
                                bHave = true;

                                RectangleF rectfintersectORG = rectTempFind;
                                rectfintersectORG.Intersect(foundorg.rect);
                                double dAreaRatio = (float)(rectfintersectORG.Width * rectfintersectORG.Height) / (float)(foundorg.rect.Width * foundorg.rect.Height);
                                if (dAreaRatio < mblobParaPropertyGrid.checkarearatio * 0.01)
                                {
                                    gg.DrawRectangle(new Pen(new SolidBrush(Color.Red), 5), Rectangle.Round(foundrun.rect));
                                    gg.DrawRectangle(new Pen(new SolidBrush(Color.Gold), 3), Rectangle.Round(foundorg.rect));

                                    gg.DrawString(((float)dAreaRatio).ToString("0.00"), new Font("宋体", 11), new SolidBrush(Color.Red), foundorg.rect.Location);

                                    strErr += $"PIN研磨不均匀";
                                }
                                else
                                {
                                    // abs(org-run)/org
                                    double dc = Math.Abs(foundorg.Area - foundrun.Area) * 1.0 / foundorg.Area * 1.0;
                                    //dc = foundrun.Area / foundorg.Area;
                                    if (dc > mblobParaPropertyGrid.checkAreaResultRatio * 0.01)
                                    {

                                        gg.DrawRectangle(new Pen(new SolidBrush(Color.Red), 5), Rectangle.Round(foundrun.rect));
                                        gg.DrawRectangle(new Pen(new SolidBrush(Color.Gold), 3), Rectangle.Round(foundorg.rect));

                                        gg.DrawString(((float)dc).ToString("0.00"), new Font("宋体", 11), new SolidBrush(Color.Red), foundorg.rect.Location);

                                        strErr += $"PIN研磨不均匀";

                                    }
                                }
                            }
                        }
                        //没有对应的点
                        if (!bHave)
                        {
                            gg.DrawRectangle(new Pen(new SolidBrush(Color.Red), 3), Rectangle.Round(foundorg.rect));
                            strErr += $"PIN未研磨";
                        }
                    }

                    if (mblobParaPropertyGrid.ischeckMaxRatio)
                    {
                        if (string.IsNullOrEmpty(strErr))
                        {
                            // abs(org-run)/org
                            double dc = Math.Abs(mblobParaPropertyGrid.MaxFound.Area - mblobParaPropertyGrid.MaxFoundRun.Area) * 1.0 / mblobParaPropertyGrid.MaxFound.Area * 1.0;
                            //dc = foundrun.Area / foundorg.Area;
                            if (dc > mblobParaPropertyGrid.checkAreaMaxResultRatio * 0.01)
                            {

                                gg.DrawRectangle(new Pen(new SolidBrush(Color.Red), 5), Rectangle.Round(mblobParaPropertyGrid.MaxFoundRun.rect));
                                gg.DrawRectangle(new Pen(new SolidBrush(Color.Gold), 3), Rectangle.Round(mblobParaPropertyGrid.MaxFound.rect));

                                gg.DrawString(((float)dc).ToString("0.00"), new Font("宋体", 11), new SolidBrush(Color.Red), mblobParaPropertyGrid.MaxFound.rect.Location);

                                strErr += $"PIN研磨不均匀";

                            }
                        }
                    }


                    if (string.IsNullOrEmpty(strErr))
                    {
                        str = "Run blobs is PASS ";
                        workstatus.ProcessString += str + Environment.NewLine;
                        workstatus.Reason = ReasonEnum.PASS;
                        ret = true;
                    }
                    else
                    {
                        str = strErr;
                        workstatus.ProcessString += str + Environment.NewLine;
                        workstatus.Reason = ReasonEnum.NG;
                        workstatus.Desc = "PIN研磨不均匀";
                        ret = false;
                    }
                }
            }

            bmpInput.Dispose();
            gg.Dispose();

            return ret;
        }
        bool CheckBlobsV1(Bitmap bmpinput, Bitmap bmpmask, ref Bitmap bmpoutput, bool istrain, WorkStatusClass workstatus, PassInfoClass passinfo)
        {
            bool ret = false;
            string str = "";

            if (istrain)
                mblobParaPropertyGrid.ClassList.Clear();
            else
                mblobParaPropertyGrid.ClassRunList.Clear();

            //Bitmap bmpInput = new Bitmap(bmpinput);
            Bitmap bmpInput = (Bitmap)bmpinput.Clone();
            Rectangle myRect = m_JzTools.SimpleRect(bmpinput.Size);
            m_JzFind.GetMaskedImage(bmpInput, bmpmask, Color.White, Color.Black);

            AForge.Imaging.Filters.Grayscale grayscale = new AForge.Imaging.Filters.Grayscale(0.299, 0.587, 0.114);
            bmpInput = grayscale.Apply(bmpInput);

            AForge.Imaging.Filters.SISThreshold sISThreshold = new AForge.Imaging.Filters.SISThreshold();
            bmpInput = sISThreshold.Apply(bmpInput);

            AForge.Imaging.Filters.BlobsFiltering blobsFiltering = new AForge.Imaging.Filters.BlobsFiltering();
            blobsFiltering.MinWidth = mblobParaPropertyGrid.chkMinWidth;
            blobsFiltering.MinHeight = mblobParaPropertyGrid.chkMinHeight;
            bmpInput = blobsFiltering.Apply(bmpInput);

            //jzFindObjectClass.AH_SetThreshold(bmpinput, ref bmpInput, checkBaseParaPropertyGrid.chkThresholdValue);
            //m_JzFind.SetThresholdEX(bmpInput, myRect, 0, mblobParaPropertyGrid.chkThresholdValue, 0, !(mblobParaPropertyGrid.chkblobmode == BlobMode.White));
            m_JzFind.AH_FindBlob(bmpInput, true);
            //m_JzFind.Find(bmpInput, Color.Red);
            m_JzFind.SortByArea();

            //int myArea = myRect.Width * myRect.Height;

            bmpoutput.Dispose();
            //bmpoutput = new Bitmap(bmpinput);
            bmpoutput = (Bitmap)bmpinput.Clone();
            Graphics gg = Graphics.FromImage(bmpoutput);


            List<Rectangle> list = new List<Rectangle>();
            list.Clear();
            int iCount = m_JzFind.FoundList.Count;
            if (iCount > 0)
            {
                if (istrain)
                {
                    foreach (FoundClass found in m_JzFind.FoundList)
                    {
                        if (found.Area > mblobParaPropertyGrid.chkMinArea)
                        {
                            mblobParaPropertyGrid.ClassList.Add(found);
                            //m_JzTools.DrawRectEx(bmpoutput, found.rect, new Pen(Color.Red));
                            //gg.DrawRectangle(new Pen(Color.Red, 3), found.rect);

                            list.Add(found.rect);
                        }
                    }
                    if (mblobParaPropertyGrid.ischeckMaxRatio)
                        mblobParaPropertyGrid.MaxFound = m_JzFind.FoundList[m_JzFind.GetMaxRectIndex()];
                    gg.DrawRectangles(new Pen(Color.Red, 3), list.ToArray());
                    iCount = mblobParaPropertyGrid.ClassList.Count;
                    str = $"训练找到的个数{iCount}";
                }
                else
                {
                    foreach (FoundClass found in m_JzFind.FoundList)
                    {
                        if (found.Area > mblobParaPropertyGrid.chkMinArea)
                        {
                            mblobParaPropertyGrid.ClassRunList.Add(found);
                            //m_JzTools.DrawRectEx(bmpoutput, found.rect, new Pen(Color.Red));
                            list.Add(found.rect);
                        }
                    }
                    if (mblobParaPropertyGrid.ischeckMaxRatio)
                        mblobParaPropertyGrid.MaxFoundRun = m_JzFind.FoundList[m_JzFind.GetMaxRectIndex()];
                    iCount = mblobParaPropertyGrid.ClassRunList.Count;
                    str = $"RUN找到的个数{iCount}";
                }
                workstatus.ProcessString += str + Environment.NewLine;
            }

            if (istrain)
            {
                {
                    str = "train blobs is PASS ";
                    workstatus.ProcessString += str + Environment.NewLine;
                    workstatus.Reason = ReasonEnum.PASS;

                    ret = true;
                }
            }
            else
            {
                if (iCount != mblobParaPropertyGrid.ClassList.Count)
                {
                    str = $"Run blobs is NG , 数量错误{iCount}不等于{mblobParaPropertyGrid.ClassList.Count}";
                    workstatus.ProcessString += str + Environment.NewLine;
                    workstatus.Reason = ReasonEnum.NG;
                    workstatus.Desc = "PIN数量错误";
                    ret = false;

                    gg.DrawRectangles(new Pen(Color.Red, 5), list.ToArray());
                    gg.DrawString(str, new Font("宋体", 11), new SolidBrush(Color.Red), new Point(5, 5));
                }
                else
                {

                    string strErr = string.Empty;
                    foreach (FoundClass foundorg in mblobParaPropertyGrid.ClassList)
                    {
                        bool bHave = false;
                        foreach (FoundClass foundrun in mblobParaPropertyGrid.ClassRunList)
                        {
                            Rectangle rectTempFind = foundrun.rect;
                            rectTempFind.Inflate(mblobParaPropertyGrid.extendx, mblobParaPropertyGrid.extendy);

                            if (foundorg.rect.IntersectsWith(rectTempFind))
                            {
                                bHave = true;

                                RectangleF rectfintersectORG = rectTempFind;
                                rectfintersectORG.Intersect(foundorg.rect);
                                double dAreaRatio = (float)(rectfintersectORG.Width * rectfintersectORG.Height) / (float)(foundorg.rect.Width * foundorg.rect.Height);
                                if (dAreaRatio < mblobParaPropertyGrid.checkarearatio * 0.01)
                                {
                                    gg.DrawRectangle(new Pen(new SolidBrush(Color.Red), 5), Rectangle.Round(foundrun.rect));
                                    gg.DrawRectangle(new Pen(new SolidBrush(Color.Gold), 3), Rectangle.Round(foundorg.rect));

                                    gg.DrawString(((float)dAreaRatio).ToString("0.00"), new Font("宋体", 11), new SolidBrush(Color.Red), foundorg.rect.Location);

                                    strErr += $"PIN研磨不均匀";
                                }
                                else
                                {
                                    // abs(org-run)/org
                                    double dc = Math.Abs(foundorg.Area - foundrun.Area) * 1.0 / foundorg.Area * 1.0;
                                    //dc = foundrun.Area / foundorg.Area;
                                    if (dc > mblobParaPropertyGrid.checkAreaResultRatio * 0.01)
                                    {

                                        gg.DrawRectangle(new Pen(new SolidBrush(Color.Red), 5), Rectangle.Round(foundrun.rect));
                                        gg.DrawRectangle(new Pen(new SolidBrush(Color.Gold), 3), Rectangle.Round(foundorg.rect));

                                        gg.DrawString(((float)dc).ToString("0.00"), new Font("宋体", 11), new SolidBrush(Color.Red), foundorg.rect.Location);

                                        strErr += $"PIN研磨不均匀";

                                    }
                                }
                            }
                        }
                        //没有对应的点
                        if (!bHave)
                        {
                            gg.DrawRectangle(new Pen(new SolidBrush(Color.Red), 3), Rectangle.Round(foundorg.rect));
                            strErr += $"PIN未研磨";
                        }
                    }

                    if (mblobParaPropertyGrid.ischeckMaxRatio)
                    {
                        if (string.IsNullOrEmpty(strErr))
                        {
                            // abs(org-run)/org
                            double dc = Math.Abs(mblobParaPropertyGrid.MaxFound.Area - mblobParaPropertyGrid.MaxFoundRun.Area) * 1.0 / mblobParaPropertyGrid.MaxFound.Area * 1.0;
                            //dc = foundrun.Area / foundorg.Area;
                            if (dc > mblobParaPropertyGrid.checkAreaMaxResultRatio * 0.01)
                            {

                                gg.DrawRectangle(new Pen(new SolidBrush(Color.Red), 5), Rectangle.Round(mblobParaPropertyGrid.MaxFoundRun.rect));
                                gg.DrawRectangle(new Pen(new SolidBrush(Color.Gold), 3), Rectangle.Round(mblobParaPropertyGrid.MaxFound.rect));

                                gg.DrawString(((float)dc).ToString("0.00"), new Font("宋体", 11), new SolidBrush(Color.Red), mblobParaPropertyGrid.MaxFound.rect.Location);

                                strErr += $"PIN研磨不均匀";

                            }
                        }
                    }

                    if (string.IsNullOrEmpty(strErr))
                    {
                        str = "Run blobs is PASS ";
                        workstatus.ProcessString += str + Environment.NewLine;
                        workstatus.Reason = ReasonEnum.PASS;
                        ret = true;
                    }
                    else
                    {
                        str = strErr;
                        workstatus.ProcessString += str + Environment.NewLine;
                        workstatus.Reason = ReasonEnum.NG;
                        workstatus.Desc = "PIN研磨不均匀";
                        ret = false;
                    }
                }
            }

            bmpInput.Dispose();
            gg.Dispose();

            return ret;
        }

        void FromString(string str)
        {
            mblobParaPropertyGrid.FromingStr(str);
        }
        bool IsInRangeRatio(double runvalue, double orgvalue, double ratio)
        {
            return (runvalue >= (orgvalue * (1 - ratio))) && (runvalue <= (orgvalue * (1 + ratio)));
        }
    }
}
