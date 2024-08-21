//#define IDS
#define EPIX
#define TIS
#define TISUSB
//#define PTG
//#define AISYS
//#define ICAM
//#define MVS
//#define IWIN
#define DVP2

#if DVP2
using DVPCameraType;
using JetEazy.ControlSpace.CCDDriver;
#endif

#if TIS || TISUSB
using TIS.Imaging;
using TIS.Imaging.VCDHelpers;
#endif

#if PTG
using FlyCapture2Managed;
#endif

#if AISYS
using AxAltairUDrv;
using AxAxAltairUDrv;
#endif

#if ICAM
using Camera_NET;
using DirectShowLib;
#endif

#if IWIN
using MVSDK;//使用SDK接口
using CameraHandle = System.Int32;
using MvApi = MVSDK.MvApi;
#endif

#if MVS
using MvCamCtrl.NET;
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using JetEazy;

using System.Threading.Tasks;

using JetEazy.BasicSpace;
using System.Diagnostics;

namespace JetEazy.ControlSpace
{
    public class SizeDefClass
    {
        public Size OrgSize = new Size();
        public Size ToSize = new Size();

        public SizeMethodEnum SizeMethod = SizeMethodEnum.NONE;

        public SizeDefClass(string str)
        {
            string[] strs;
            int[] sizes;


            if (str.IndexOf('a') > -1)
            {
                SizeMethod = SizeMethodEnum.CORP;
                strs = str.Split('a');
                sizes = Array.ConvertAll(strs[0].Split('x'), int.Parse);
                OrgSize = new Size(sizes[0], sizes[1]);
                sizes = Array.ConvertAll(strs[1].Split('x'), int.Parse);
                ToSize = new Size(sizes[0], sizes[1]);
            }
            else if (str.IndexOf('b') > -1)
            {
                SizeMethod = SizeMethodEnum.EXTEND;
                strs = str.Split('b');
                sizes = Array.ConvertAll(strs[0].Split('x'), int.Parse);
                OrgSize = new Size(sizes[0], sizes[1]);
                sizes = Array.ConvertAll(strs[1].Split('x'), int.Parse);
                ToSize = new Size(sizes[0], sizes[1]);
            }
            else if (str.IndexOf('c') > -1)
            {
                SizeMethod = SizeMethodEnum.ZOOM;
                strs = str.Split('c');
                sizes = Array.ConvertAll(strs[0].Split('x'), int.Parse);
                OrgSize = new Size(sizes[0], sizes[1]);
                sizes = Array.ConvertAll(strs[1].Split('x'), int.Parse);
                ToSize = new Size(sizes[0], sizes[1]);
            }
            else
            {
                SizeMethod = SizeMethodEnum.NONE;
                sizes = Array.ConvertAll(str.Split('x'), int.Parse);
                OrgSize = new Size(sizes[0], sizes[1]);
                ToSize = OrgSize;
            }
        }

        public void SizeDefBMP(ref Bitmap bmp)
        {
            Bitmap bmptmp;
            RectangleF rectf;

            switch (SizeMethod)
            {
                case SizeMethodEnum.NONE:

                    break;
                case SizeMethodEnum.CORP:
                    bmptmp = new Bitmap(bmp);
                    rectf = GetCenterRect(bmp.Size);
                    bmp.Dispose();
                    bmp = (Bitmap)bmptmp.Clone(rectf, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    bmptmp.Dispose();
                    break;
            }

        }

        RectangleF GetCenterRect(Size fromsize)
        {
            RectangleF rectf = new RectangleF(0, 0, ToSize.Width, ToSize.Height);

            rectf.X = (float)(fromsize.Width - ToSize.Width) / 2f;
            rectf.Y = (float)(fromsize.Height - ToSize.Height) / 2f;

            return rectf;
        }

    }
    public class CCDWithRelateClass
    {
        public CCDClass CCD;
        public int RelateIndex = 0;

        public CCDWithRelateClass(CCDClass ccd, int relateindex)
        {
            CCD = ccd;
            RelateIndex = relateindex;
        }

        public void GetImage()
        {
            CCD.GetImage(RelateIndex);
        }
        public void GetImage(List<Bitmap> bmplist)
        {
            CCD.GetImage(bmplist, RelateIndex);
        }
        public Bitmap GetBMP()
        {
            return CCD.GetBMP(RelateIndex);
        }

    }

    public class CCDRectRelateIndexClass   //畫面圖像各個區域的大小
    {
        public int Index = 0;
        public Rectangle SizedRect = new Rectangle();
        //public float Ratio = 0f;
        public SizeF SizedRatio = new SizeF();

        public CCDRectRelateIndexClass()
        {

        }
        //public CCDRectRelateIndexClass(string str,List<Bitmap> bmplist,float ratio)
        //{
        //    string[] strs = str.Split(':');

        //    Index = int.Parse(strs[0]);

        //    string[] locationstrs = strs[1].Split(',');

        //    Point location = new Point(int.Parse(locationstrs[0]), int.Parse(locationstrs[1]));
        //    Rectangle OrgRect = new Rectangle(location, bmplist[Index].Size);

        //    SizedRect = ResizeWithLocation(OrgRect, ratio);

        //    Ratio = ratio;
        //}
        public CCDRectRelateIndexClass(string str, List<Bitmap> bmplist, List<Size> showsizelist)
        {
            string[] strs = str.Split(':');

            Index = int.Parse(strs[0]);

            string[] locationstrs = strs[1].Split(',');

            Point location = new Point(int.Parse(locationstrs[0]), int.Parse(locationstrs[1]));
            //Rectangle OrgRect = new Rectangle(location, bmplist[Index].Size);

            SizedRect = new Rectangle(location, showsizelist[Index]);

            SizedRatio = new SizeF((float)showsizelist[Index].Width / (float)bmplist[Index].Width, (float)showsizelist[Index].Height / (float)bmplist[Index].Height);

            //Ratio = ratio;
        }
        public CCDRectRelateIndexClass Clone()
        {
            CCDRectRelateIndexClass retccdrectrelateindex = new CCDRectRelateIndexClass();

            retccdrectrelateindex.Index = Index;
            retccdrectrelateindex.SizedRect = SizedRect;
            retccdrectrelateindex.SizedRatio = SizedRatio;

            return retccdrectrelateindex;
        }
        public void Move(Point bias)
        {
            SizedRect.Offset(bias);
        }
        Rectangle ResizeWithLocation(Rectangle rect, float ratio)
        {
            return new Rectangle((int)((float)rect.X * ratio), (int)((float)rect.Y * ratio), (int)((float)rect.Width * ratio), (int)((float)rect.Height * ratio));
        }

        public override string ToString()
        {
            string retstr = "";

            retstr = Index.ToString("00") + "# ";
            retstr += SizedRect.X.ToString() + ",";
            retstr += SizedRect.Y.ToString() + ",";
            retstr += SizedRect.Width.ToString() + ",";
            retstr += SizedRect.Height.ToString();

            return retstr;
        }
        public string ToCCDLocationString()
        {
            string retstr = "";

            retstr += Index.ToString() + ":";
            //retstr += ((int)((float)SizedRect.X / Ratio)).ToString() + "," + ((int)((float)SizedRect.Y / Ratio)).ToString();
            retstr += SizedRect.X.ToString() + "," + SizedRect.Y.ToString();

            return retstr;
        }

    }
    public class CCDCollectionClass //所有型號CCD的集合
    {
        VersionEnum VERSION;
        OptionEnum OPTION;

        bool IsNoUseCCD = false;

        string readstr = "";

        List<int> CCDINDEXLIST = new List<int>();

        int CCDCOUNT = 0;
        public int EXTEND = 100;
        string BACKGROUND = "";

        List<Size> ShowSizeList = new List<Size>();
        List<Bitmap> BMPList = new List<Bitmap>();
        List<Bitmap> bmpR5list = new List<Bitmap>();
        List<Bitmap> bmpR5list80002 = new List<Bitmap>();

        public Bitmap bmp80002 = new Bitmap(1, 1);

        List<CCDClass> CCDList = new List<CCDClass>();
        List<Rectangle> CCDRectList = new List<Rectangle>();
        public List<CCDRectRelateIndexClass> CCDRectRelateIndexList = new List<CCDRectRelateIndexClass>();

        public Bitmap bmpAll = new Bitmap(1, 1);
        public Bitmap bmpBackGround = new Bitmap(1, 1);

        JzToolsClass JzTools = new JzToolsClass();

        JzTimes myTimer = new JzTimes();

        string WORKPATH = "";
        private bool IsConnectFail = false;
        public bool IsCCDError
        {
            get { return IsConnectFail; }
        }

        public int GetCCDCount
        {
            get
            {
                //switch(VERSION)
                //{
                //    case VersionEnum.ALLINONE:
                //        switch(OPTION)
                //        {
                //            case OptionEnum.R32:
                //                return CCDRectList.Count;
                //            default:
                //                return CCDCOUNT;
                //        }
                //    default:
                //        return CCDCOUNT;

                //}

                return BMPList.Count;
            }
        }

        public int GetCCDCountWord
        {
            get
            {

                int ccdCount = 0;

                foreach (CCDClass ccd in CCDList)
                {
                    ccdCount += ccd.CCDRelateIndexArray.Length;
                }
                return ccdCount;

                //switch (VERSION)
                //{
                //    case VersionEnum.ALLINONE:
                //        switch (OPTION)
                //        {
                //            case OptionEnum.R32:
                //                return CCDRectList.Count;
                //            default:
                //                return CCDCOUNT;
                //        }
                //    default:
                //        return CCDCOUNT;

                //}

                //    return BMPList.Count;
            }
        }
        public CCDCollectionClass(string workpath, bool isnouseccd, VersionEnum version, OptionEnum option, int iccdtypeIndex = 0)
        {
            VERSION = version;
            OPTION = option;

            IsNoUseCCD = isnouseccd;
            WORKPATH = workpath;
            //DEBUGRAWPATH = debugrawpath;

            CCDClass.WORKPATH = WORKPATH;


            string ccd_filename = "CCD.INI";

            switch (option)
            {
                case OptionEnum.MAIN_X6:

                    switch (iccdtypeIndex)
                    {
                        //TYPE18 1800W
                        case 1:

                            ccd_filename = "CCD_TYPE18.INI";

                            break;
                    }

                    break;
            }


            #region Get Each CCD Information

            JzTools.ReadData(ref readstr, WORKPATH + "\\" + ccd_filename);
            string[] readstrs = readstr.Replace(Environment.NewLine, "$").Split('$');

            foreach (string str in readstrs)
            {
                if (str.IndexOf("CCDCOUNT") > -1)
                {
                    CCDCOUNT = int.Parse(JzTools.GetFromINIFormat(str, '='));
                }
                else if (str.IndexOf("EXTEND") > -1)
                {
                    EXTEND = int.Parse(JzTools.GetFromINIFormat(str, '='));
                }
                else if (str.IndexOf("CCDINFO") > -1)
                {
                    CCDClass CCD = new CCDClass(JzTools.GetFromINIFormat(str, '='), version, option);
                    CCD.TriggerAction += CCD_TriggerAction;
                    CCDList.Add(CCD);
                }
                else if (str.IndexOf("BACKGROUND") > -1)
                {
                    BACKGROUND = JzTools.GetFromINIFormat(str, '=');

                    if (BACKGROUND != "")
                        JzTools.GetBMP(BACKGROUND, ref bmpBackGround);

                }
                else if (str.IndexOf("CAM") > -1)
                {
                    string[] strs = JzTools.GetFromINIFormat(str, '=').Split(';');

                    Size newsize = JzTools.StringToSize(strs[0]);

                    //CCDRectList[int.Parse(strs[0])].
                    ShowSizeList.Add(JzTools.StringToSize(strs[1]));

                    Bitmap bmp = new Bitmap(newsize.Width, newsize.Height);
                    BMPList.Add(bmp);
                }
                else if (str.IndexOf("RESET") > -1)
                {
                    string[] strs = JzTools.GetFromINIFormat(str, '=').Split(';');
                    string[] strindexs = strs[0].Trim().Split(',');

                    foreach (string strtemp in strindexs)
                    {
                        try
                        {
                            int index = int.Parse(strtemp);
                            CCDINDEXLIST.Add(index);
                        }
                        catch
                        {
                            MessageBox.Show(WORKPATH + "\\CCD.INI目录中的 RESET=  字段" + strtemp + "设定错误");

                        }
                    }
                    if (CCDINDEXLIST.Count != 12)
                        MessageBox.Show(WORKPATH + "\\CCD.INI目录中的 (RESET=  )总数不对,需要设定12个位置,且不能重复");

                }
            }

            #endregion

            #region Get CCD Locations

            LoadCCDLocation();

            //CreatebmpAll();

            #endregion
        }


        private void CCD_TriggerAction(string operationstr)
        {
            string[] strs = operationstr.Split(',');

            switch (strs[1])
            {
                case "EPIX":
                    GetImage(int.Parse(strs[0]));
                    break;
            }

            OnTrigger(operationstr);
        }

        /// <summary>
        /// 重组第一张图的螺丝顺序
        /// </summary>
        /// <param name="bmpSce">源图</param>
        /// <returns>重组后的图</returns>
        public Bitmap GetResetZeroBmp(Bitmap bmpSce)
        {
            if (CCDINDEXLIST.Count < 2)
                return bmpSce;

            Bitmap bmpTemp = new Bitmap(bmpSce.Width, bmpSce.Height);
            Graphics g = Graphics.FromImage(bmpTemp);

            Size mysize = new Size(bmpSce.Width / 4, bmpSce.Height / 3);
            List<Bitmap> bmpList = new List<Bitmap>();
            List<Point> poilist = new List<Point>();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Point poi = new Point(j * mysize.Width, i * mysize.Height);
                    poilist.Add(poi);


                    Rectangle rect = new Rectangle(poi, mysize);
                    Bitmap bmptemp = bmpSce.Clone(rect, PixelFormat.Format32bppPArgb);
                    bmpList.Add(bmptemp);
                }
            }
            string[] readstrs = readstr.Replace(Environment.NewLine, "$").Split('$');

            for (int i = 0; i < CCDINDEXLIST.Count; i++)
            {
                int indextemp = CCDINDEXLIST[i] - 1;
                g.DrawImage(bmpList[indextemp], poilist[i]);
            }


            g.Dispose();

            return bmpTemp;
        }

        void CreatebmpAll()
        {
            Rectangle AllRect = new Rectangle(0, 0, 1, 1);

            if (BACKGROUND != "")
            {
                AllRect = JzTools.SimpleRect(bmpBackGround.Size);
            }
            else
            {
                SmoothenCCDRectRelateIndex(CCDRectRelateIndexList);

                foreach (CCDRectRelateIndexClass ccdrectrelateindex in CCDRectRelateIndexList)
                {
                    AllRect = JzTools.MergeTwoRects(AllRect, ccdrectrelateindex.SizedRect);
                }
            }

            if (CCDRectRelateIndexList.Count > 10)
                MessageBox.Show("ERROR!!!");

            AllRect.Inflate(EXTEND, EXTEND);

            bmpAll.Dispose();
            bmpAll = new Bitmap(AllRect.Width, AllRect.Height);

            JzTools.FillupColor(bmpAll, Color.Green);

            if (BACKGROUND != "")
                JzTools.DrawImage(bmpBackGround, bmpAll, new Rectangle(new Point(EXTEND, EXTEND), bmpBackGround.Size));

            //bmpWHOLE.Save(@"D:\\LOA\BMPWHOLE" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

        }
        int Seq = 0;
        /// <summary>
        /// 循序抓取有定義的 CCD Relate Index 的畫面
        /// </summary>
        public void GetBmpSeq()
        {
            GetBmpAll(CCDRectRelateIndexList[Seq].Index);

            Seq++;

            if (Seq == CCDRectRelateIndexList.Count)
                Seq = 0;
        }
        /// <summary>
        /// index = -2 不抓圖直用內部資料，-1抓所有的圖，其他就是指定的像機
        /// </summary>
        /// <param name="index"></param>
        public void GetBmpAll(int index)
        {
            if (index >= -1)
            {
                if (index == -1)
                {
                    foreach (CCDRectRelateIndexClass ccdrelateindex in CCDRectRelateIndexList)
                    {
                        GetImage(ccdrelateindex.Index);
                    }
                }
                else
                {
                    GetImage(index);
                }
            }

            Graphics g = Graphics.FromImage(bmpAll);
            foreach (CCDRectRelateIndexClass ccdrelateindex in CCDRectRelateIndexList)
            {
                Rectangle rect = ccdrelateindex.SizedRect;
                rect.Offset(EXTEND, EXTEND);

                g.DrawImage(BMPList[ccdrelateindex.Index], rect, JzTools.SimpleRect(BMPList[ccdrelateindex.Index].Size), GraphicsUnit.Pixel);
            }
            g.Dispose();

            //bmpAll.Save(@"D:\\LOA\BMPALL" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
        }
        public Bitmap GetBMP(int relateindex, bool isreget)
        {
            if (isreget)
                GetImage(relateindex);

            return BMPList[relateindex];
        }
        public void SetBMP(Bitmap bmp, int relateindex)
        {

            BMPList[relateindex] = bmp;
        }
        public void MoveCCDRectRelateIndex(Point bias, int[] indexs)
        {
            foreach (CCDRectRelateIndexClass ccdrectrelateindex in CCDRectRelateIndexList)
            {
                if (Array.IndexOf(indexs, ccdrectrelateindex.Index) > -1)
                {
                    ccdrectrelateindex.Move(bias);
                }
            }
            CreatebmpAll();
            GetBmpAll(-2);
        }

        List<CCDRectRelateIndexClass> backuplist = new List<CCDRectRelateIndexClass>();

        public void BackupList()
        {
            backuplist.Clear();

            foreach (CCDRectRelateIndexClass cri in CCDRectRelateIndexList)
            {
                backuplist.Add(cri.Clone());
            }
        }
        public void RestoreList()
        {
            CCDRectRelateIndexList.Clear();

            foreach (CCDRectRelateIndexClass cri in backuplist)
            {
                CCDRectRelateIndexList.Add(cri.Clone());
            }
        }
        public string GetRectRelateData()
        {
            string retstr = "";

            foreach (CCDRectRelateIndexClass cri in CCDRectRelateIndexList)
            {
                if (cri.Index == 2)
                    retstr += cri.ToString() + ";";
            }
            if (retstr.Length > 1)
                retstr = retstr.Remove(retstr.Length - 1, 1);

            return retstr;
        }
        public CCDRectRelateIndexClass GetRectRelateIndexData(int index)
        {
            int i = 0;

            foreach (CCDRectRelateIndexClass cri in CCDRectRelateIndexList)
            {
                if (cri.Index == index)
                    break;
                i++;
            }
            if (index >= CCDRectRelateIndexList.Count)
                return CCDRectRelateIndexList[0];

            return CCDRectRelateIndexList[i];
        }
        public void MoveCCDRectRelateIndex(Point bias, string moveindexstring)
        {
            foreach (CCDRectRelateIndexClass ccdrectrelateindex in CCDRectRelateIndexList)
            {
                if (moveindexstring.IndexOf(ccdrectrelateindex.Index.ToString("00")) > -1)
                {
                    ccdrectrelateindex.Move(bias);
                }
            }

            CreatebmpAll();
            GetBmpAll(-2);
        }
        /// <summary>
        /// 在移動畫面上的圖型後，將位置調整為正確的位置
        /// </summary>
        /// <param name="ccdrectrelateindexlist"></param>
        void SmoothenCCDRectRelateIndex(List<CCDRectRelateIndexClass> ccdrectrelateindexlist)
        {
            int minx = 10000;
            int miny = 10000;

            foreach (CCDRectRelateIndexClass ccdrectrelateindex in ccdrectrelateindexlist)
            {
                Point pt = ccdrectrelateindex.SizedRect.Location;

                if (pt.X < minx)
                {
                    minx = pt.X;
                }
                if (pt.Y < miny)
                {
                    miny = pt.Y;
                }
            }

            minx = -minx;
            miny = -miny;

            foreach (CCDRectRelateIndexClass ccdrectrelateindex in ccdrectrelateindexlist)
            {
                ccdrectrelateindex.Move(new Point(minx, miny));
            }

        }
        public void SaveCCDLocation()
        {
            string savestr = "";

            foreach (CCDRectRelateIndexClass cri in CCDRectRelateIndexList)
            {
                savestr += cri.ToCCDLocationString() + Environment.NewLine;
            }

            savestr = savestr.Remove(savestr.Length - 2, 2);

            JzTools.SaveData(savestr, WORKPATH + "\\CCDLOCATION.INI");
        }
        public void LoadCCDLocation()
        {
            string readstr = "";

            JzTools.ReadData(ref readstr, WORKPATH + "\\CCDLOCATION.INI");

            string[] readstrs = readstr.Replace(Environment.NewLine, Universal.NewlineChar.ToString()).Split(Universal.NewlineChar);

            CCDRectRelateIndexList.Clear();

            foreach (string readlocationstr in readstrs)
            {
                if (readlocationstr.IndexOf(":") < 0)
                    continue;

                CCDRectRelateIndexClass ccdrectrelateindex = new CCDRectRelateIndexClass(readlocationstr, BMPList, ShowSizeList);
                CCDRectRelateIndexList.Add(ccdrectrelateindex);
            }

            CreatebmpAll();
        }
        public void SetDebugPath(string debugpath)
        {
            CCDClass.WORKPATH = debugpath;
        }
        public void SetDebugEnvPath(string envpath)
        {
            CCDClass.ENVPATH = envpath;
        }
        public void SetPageOPType(string pageoptypestr)
        {
            CCDClass.PAGEOPTYPE = pageoptypestr;
        }
        public bool Initial()
        {
            bool ret = false;

            foreach (CCDClass ccd in CCDList)
            {
                //ret = ccd.Initial(BMPList);
                ret = ccd.Initial();

                if (!ret)
                    break;
            }

            if (ret)
            {
                GetImage();
            }


            return ret;
        }
        public bool Initial(string path)
        {
            bool ret = false;

            foreach (CCDClass ccd in CCDList)
            {
                //ret = ccd.Initial(BMPList);
                ret = ccd.Initial(path);

                if (!ret)
                    break;
            }

            if (ret)
            {
                GetImage();
            }


            return ret;
        }


        public void GetImage(int relateindex)
        {
            int i = 0;
            List<CCDWithRelateClass> ParallelCCDList = new List<CCDWithRelateClass>();

            switch (VERSION)
            {
                case VersionEnum.ALLINONE:

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN:
                            foreach (CCDClass ccd in CCDList)
                            {
                                if (ccd.GetImage(BMPList, relateindex))
                                {
                                    break;
                                }
                            }
                            break;
                        case OptionEnum.R32:
                            switch (relateindex)
                            {
                                case 0:
                                    if (IsNoUseCCD)
                                    {
                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        i = 0;
                                        while (i < 10)
                                        {
                                            foreach (CCDClass ccd in CCDList)
                                            {
                                                if (ccd.CheckIndex(i))
                                                {
                                                    ParallelCCDList.Add(new CCDWithRelateClass(ccd, i));
                                                    break;
                                                }
                                            }
                                            i++;
                                        }

                                        //myTimer.Cut();

                                        Parallel.ForEach<CCDWithRelateClass>(ParallelCCDList, ccd =>
                                        {
                                            ccd.GetImage();
                                        });

                                        //foreach (CCDWithRelateClass ccd in ParallelCCDList)
                                        //{
                                        //    ccd.GetImage();
                                        //}

                                        //int ms = myTimer.msDuriation;

                                        GetR32AssembleImage(BMPList[0], ParallelCCDList);
                                    }
                                    break;
                                case 1:
                                case 2:
                                case 3:
                                case 4:
                                case 5:
                                case 6:
                                    if (IsNoUseCCD)
                                    {
                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        int bias = 9;

                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex, bias))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    break;
                            }
                            break;
                        case OptionEnum.R26:
                            switch (relateindex)
                            {
                                case 0:
                                    if (IsNoUseCCD)
                                    {
                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        i = 0;
                                        while (i < 6)
                                        {
                                            foreach (CCDClass ccd in CCDList)
                                            {
                                                if (ccd.CheckIndex(i))
                                                {
                                                    ParallelCCDList.Add(new CCDWithRelateClass(ccd, i));
                                                    break;
                                                }
                                            }
                                            i++;
                                        }

                                        //myTimer.Cut();

                                        Parallel.ForEach<CCDWithRelateClass>(ParallelCCDList, ccd =>
                                        {
                                            ccd.GetImage();
                                        });

                                        //foreach (CCDWithRelateClass ccd in ParallelCCDList)
                                        //{
                                        //    ccd.GetImage();
                                        //}

                                        //int ms = myTimer.msDuriation;

                                        GetR26AssembleImage(BMPList[0], ParallelCCDList);
                                    }
                                    break;
                                case 1:
                                case 2:
                                case 3:
                                case 4:
                                case 5:
                                case 6:
                                case 7:
                                    if (IsNoUseCCD)
                                    {
                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        int bias = 5;

                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex, bias))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    break;
                            }
                            break;
                        case OptionEnum.R15:
                            switch (relateindex)
                            {
                                case 0:
                                    if (IsNoUseCCD)
                                    {
                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        i = 0;
                                        while (i < 6)
                                        {
                                            foreach (CCDClass ccd in CCDList)
                                            {
                                                if (ccd.CheckIndex(i))
                                                {
                                                    ParallelCCDList.Add(new CCDWithRelateClass(ccd, i));
                                                    break;
                                                }
                                            }
                                            i++;
                                        }

                                        //myTimer.Cut();

                                        Parallel.ForEach<CCDWithRelateClass>(ParallelCCDList, ccd =>
                                        {
                                            ccd.GetImage();
                                        });

                                        //foreach (CCDWithRelateClass ccd in ParallelCCDList)
                                        //{
                                        //    ccd.GetImage();
                                        //}

                                        //int ms = myTimer.msDuriation;

                                        GetR26AssembleImage(BMPList[0], ParallelCCDList);
                                    }
                                    break;
                                case 1:
                                case 2:
                                case 3:
                                case 4:
                                case 5:
                                case 6:
                                case 7:
                                case 8:
                                case 9:
                                    if (IsNoUseCCD)
                                    {
                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        int bias = 5;

                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex, bias))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    break;
                            }
                            break;
                        case OptionEnum.R9:
                            switch (relateindex)
                            {
                                case 0:
                                    if (IsNoUseCCD)
                                    {
                                        foreach (CCDClass ccd in CCDList)
                                        {

                                            if (ccd.GetImage(BMPList, relateindex))
                                            {
                                                break;
                                            }

                                        }
                                    }
                                    else
                                    {
                                        i = 0;
                                        while (i < 6)
                                        {
                                            foreach (CCDClass ccd in CCDList)
                                            {
                                                if (ccd.CheckIndex(i))
                                                {
                                                    ParallelCCDList.Add(new CCDWithRelateClass(ccd, i));
                                                    break;
                                                }
                                            }
                                            i++;
                                        }

                                        //myTimer.Cut();

                                        Parallel.ForEach<CCDWithRelateClass>(ParallelCCDList, ccd =>
                                        {
                                            ccd.GetImage();
                                        });

                                        //foreach (CCDWithRelateClass ccd in ParallelCCDList)
                                        //{
                                        //    ccd.GetImage();
                                        //}

                                        //int ms = myTimer.msDuriation;

                                        GetR26AssembleImage(BMPList[0], ParallelCCDList);
                                    }
                                    break;
                                case 1:
                                case 2:
                                case 3:
                                case 4:
                                case 5:
                                case 6:
                                    if (IsNoUseCCD)
                                    {
                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        int bias = 5;

                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex, bias))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    break;
                            }
                            break;
                        case OptionEnum.R5:
                            switch (relateindex)
                            {
                                case 0:
                                    if (IsNoUseCCD)
                                    {
                                        foreach (CCDClass ccd in CCDList)
                                        {

                                            if (ccd.GetImage(BMPList, relateindex))
                                            {
                                                break;
                                            }

                                        }
                                    }
                                    else
                                    {
                                        i = 0;
                                        while (i < 2)
                                        {
                                            foreach (CCDClass ccd in CCDList)
                                            {
                                                if (ccd.CheckIndex(i))
                                                {
                                                    ParallelCCDList.Add(new CCDWithRelateClass(ccd, i));
                                                    break;
                                                }
                                            }
                                            i++;
                                        }

                                        //myTimer.Cut();

                                        Parallel.ForEach<CCDWithRelateClass>(ParallelCCDList, ccd =>
                                        {
                                            ccd.GetImage();
                                        });

                                        //foreach (CCDWithRelateClass ccd in ParallelCCDList)
                                        //{
                                        //    ccd.GetImage();
                                        //}

                                        //int ms = myTimer.msDuriation;

                                        // GetR5AssembleImage(BMPList[0], ParallelCCDList);

                                        if (!is80002)
                                        {
                                            bmpR5list.Add(new Bitmap(ParallelCCDList[0].GetBMP()));
                                            bmpR5list.Add(new Bitmap(ParallelCCDList[1].GetBMP()));

                                        }
                                        else
                                        {

                                            bmpR5list80002.Add(new Bitmap(ParallelCCDList[0].GetBMP()));
                                            bmpR5list80002.Add(new Bitmap(ParallelCCDList[1].GetBMP()));
                                        }
                                    }
                                    break;
                                case 1:
                                case 2:
                                case 3:
                                case 4:
                                case 5:
                                case 6:
                                    if (IsNoUseCCD)
                                    {
                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        int bias = 1;

                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex, bias))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    break;
                            }
                            break;
                        case OptionEnum.R1:
                            switch (relateindex)
                            {
                                case 0:
                                case 1:
                                case 2:
                                case 3:
                                case 4:
                                    if (IsNoUseCCD)
                                    {
                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        int bias = 0;

                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex, bias))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    break;
                            }
                            break;
                        case OptionEnum.R3:
                        case OptionEnum.C3:
                            switch (relateindex)
                            {
                                case 0:
                                case 1:
                                case 2:
                                case 3:
                                case 4:
                                    if (IsNoUseCCD)
                                    {
                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        int bias = 0;

                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex, bias))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    break;
                            }
                            break;
                        default:
                            foreach (CCDClass ccd in CCDList)
                            {
                                if (ccd.GetImage(BMPList, relateindex))
                                {
                                    break;
                                }
                            }
                            break;
                    }
                    break;
                default:
                    foreach (CCDClass ccd in CCDList)
                    {
                        if (ccd.GetImage(BMPList, relateindex))
                        {
                            break;
                        }
                    }
                    break;
            }
        }
        public void GetImageSDM1(int relateindex, int basecount = 0)
        {
            int i = 0;
            foreach (CCDClass ccd in CCDList)
            {
                if (ccd.GetImageSDM1(BMPList, relateindex, basecount))
                {
                    break;
                }
            }
        }

        public void GetImageDX(int relateindex, int CCDindex)
        {
            int i = 0;
            List<CCDWithRelateClass> ParallelCCDList = new List<CCDWithRelateClass>();

            switch (VERSION)
            {
                case VersionEnum.ALLINONE:

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN:
                            foreach (CCDClass ccd in CCDList)
                            {
                                if (ccd.GetImage(BMPList, relateindex))
                                {
                                    break;
                                }
                            }
                            break;
                        case OptionEnum.R32:
                            switch (relateindex)
                            {
                                case 0:
                                    if (IsNoUseCCD)
                                    {
                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImageDX(BMPList, relateindex, 0, CCDindex))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        i = 0;
                                        while (i < 10)
                                        {
                                            foreach (CCDClass ccd in CCDList)
                                            {
                                                if (ccd.CheckIndex(i))
                                                {
                                                    ParallelCCDList.Add(new CCDWithRelateClass(ccd, i));
                                                    break;
                                                }
                                            }
                                            i++;
                                        }

                                        //myTimer.Cut();

                                        Parallel.ForEach<CCDWithRelateClass>(ParallelCCDList, ccd =>
                                        {
                                            ccd.GetImage();
                                        });

                                        //foreach (CCDWithRelateClass ccd in ParallelCCDList)
                                        //{
                                        //    ccd.GetImage();
                                        //}

                                        //int ms = myTimer.msDuriation;

                                        GetR32AssembleImage(BMPList[0], ParallelCCDList);
                                    }
                                    break;
                                case 1:
                                case 2:
                                case 3:
                                    if (IsNoUseCCD)
                                    {
                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        int bias = 9;

                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex, bias))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    break;
                            }
                            break;
                        case OptionEnum.R26:
                            switch (relateindex)
                            {
                                case 0:
                                    if (IsNoUseCCD)
                                    {
                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        i = 0;
                                        while (i < 6)
                                        {
                                            foreach (CCDClass ccd in CCDList)
                                            {
                                                if (ccd.CheckIndex(i))
                                                {
                                                    ParallelCCDList.Add(new CCDWithRelateClass(ccd, i));
                                                    break;
                                                }
                                            }
                                            i++;
                                        }

                                        //myTimer.Cut();

                                        Parallel.ForEach<CCDWithRelateClass>(ParallelCCDList, ccd =>
                                        {
                                            ccd.GetImage();
                                        });

                                        //foreach (CCDWithRelateClass ccd in ParallelCCDList)
                                        //{
                                        //    ccd.GetImage();
                                        //}

                                        //int ms = myTimer.msDuriation;

                                        GetR26AssembleImage(BMPList[0], ParallelCCDList);
                                    }
                                    break;
                                case 1:
                                case 2:
                                case 3:
                                case 4:
                                case 5:
                                    if (IsNoUseCCD)
                                    {
                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        int bias = 5;

                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex, bias))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    break;
                            }
                            break;
                        case OptionEnum.R15:
                            switch (relateindex)
                            {
                                case 0:
                                    if (IsNoUseCCD)
                                    {
                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        i = 0;
                                        while (i < 6)
                                        {
                                            foreach (CCDClass ccd in CCDList)
                                            {
                                                if (ccd.CheckIndex(i))
                                                {
                                                    ParallelCCDList.Add(new CCDWithRelateClass(ccd, i));
                                                    break;
                                                }
                                            }
                                            i++;
                                        }

                                        //myTimer.Cut();

                                        Parallel.ForEach<CCDWithRelateClass>(ParallelCCDList, ccd =>
                                        {
                                            ccd.GetImage();
                                        });

                                        //foreach (CCDWithRelateClass ccd in ParallelCCDList)
                                        //{
                                        //    ccd.GetImage();
                                        //}

                                        //int ms = myTimer.msDuriation;

                                        GetR26AssembleImage(BMPList[0], ParallelCCDList);
                                    }
                                    break;
                                case 1:
                                case 2:
                                case 3:
                                case 4:
                                case 5:
                                case 6:
                                case 7:
                                case 8:
                                case 9:
                                    if (IsNoUseCCD)
                                    {
                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        int bias = 5;

                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex, bias))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    break;
                            }
                            break;
                        case OptionEnum.R9:
                            switch (relateindex)
                            {
                                case 0:
                                    if (IsNoUseCCD)
                                    {
                                        foreach (CCDClass ccd in CCDList)
                                        {

                                            if (ccd.GetImage(BMPList, relateindex))
                                            {
                                                break;
                                            }

                                        }
                                    }
                                    else
                                    {
                                        i = 0;
                                        while (i < 6)
                                        {
                                            foreach (CCDClass ccd in CCDList)
                                            {
                                                if (ccd.CheckIndex(i))
                                                {
                                                    ParallelCCDList.Add(new CCDWithRelateClass(ccd, i));
                                                    break;
                                                }
                                            }
                                            i++;
                                        }

                                        //myTimer.Cut();

                                        Parallel.ForEach<CCDWithRelateClass>(ParallelCCDList, ccd =>
                                        {
                                            ccd.GetImage();
                                        });

                                        //foreach (CCDWithRelateClass ccd in ParallelCCDList)
                                        //{
                                        //    ccd.GetImage();
                                        //}

                                        //int ms = myTimer.msDuriation;

                                        GetR26AssembleImage(BMPList[0], ParallelCCDList);
                                    }
                                    break;
                                case 1:
                                case 2:
                                case 3:
                                    if (IsNoUseCCD)
                                    {
                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        int bias = 5;

                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex, bias))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    break;
                            }
                            break;
                        case OptionEnum.R5:
                            switch (relateindex)
                            {
                                case 0:
                                    if (IsNoUseCCD)
                                    {
                                        foreach (CCDClass ccd in CCDList)
                                        {

                                            if (ccd.GetImage(BMPList, relateindex))
                                            {
                                                break;
                                            }

                                        }
                                    }
                                    else
                                    {
                                        i = 0;
                                        while (i < 2)
                                        {
                                            foreach (CCDClass ccd in CCDList)
                                            {
                                                if (ccd.CheckIndex(i))
                                                {
                                                    ParallelCCDList.Add(new CCDWithRelateClass(ccd, i));
                                                    break;
                                                }
                                            }
                                            i++;
                                        }

                                        //myTimer.Cut();

                                        Parallel.ForEach<CCDWithRelateClass>(ParallelCCDList, ccd =>
                                        {
                                            ccd.GetImage();
                                        });

                                        //foreach (CCDWithRelateClass ccd in ParallelCCDList)
                                        //{
                                        //    ccd.GetImage();
                                        //}

                                        //int ms = myTimer.msDuriation;

                                        GetR26AssembleImage(BMPList[0], ParallelCCDList);
                                    }
                                    break;
                                case 1:
                                case 2:
                                case 3:
                                    if (IsNoUseCCD)
                                    {
                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        int bias = 5;

                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex, bias))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    break;
                            }
                            break;
                        case OptionEnum.R1:
                            switch (relateindex)
                            {
                                case 0:
                                case 1:
                                case 2:
                                case 3:
                                case 4:
                                    if (IsNoUseCCD)
                                    {
                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        int bias = 0;

                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex, bias))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    break;
                            }
                            break;
                        case OptionEnum.R3:
                        case OptionEnum.C3:
                            switch (relateindex)
                            {
                                case 0:
                                case 1:
                                case 2:
                                case 3:
                                case 4:
                                    if (IsNoUseCCD)
                                    {
                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        int bias = 0;

                                        foreach (CCDClass ccd in CCDList)
                                        {
                                            if (ccd.GetImage(BMPList, relateindex, bias))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
                default:
                    foreach (CCDClass ccd in CCDList)
                    {
                        if (ccd.GetImage(BMPList, relateindex))
                        {
                            break;
                        }
                    }
                    break;
            }
        }

        public Bitmap GetImageWord(int ccdindex)
        {

            Bitmap bmp = null;
            foreach (CCDClass ccd in CCDList)
            {
                if (ccd.CheckIndex(ccdindex))
                {
                    ccd.GetImage(ccdindex);
                    bmp = ccd.GetBMP(ccdindex);
                    break;
                }
            }
            return bmp;
        }

        #region R32 Special Function
        public void GetR32AssembleImage(Bitmap bmp, List<CCDWithRelateClass> parallelccdlist)
        {
            int i = 0;

            Graphics g = Graphics.FromImage(bmp);

            Bitmap bmpsample = parallelccdlist[0].GetBMP();

            int width = bmpsample.Width;
            int height = bmpsample.Height;

            while (i < 10)
            {
                int locx = (int)((i % 4) * width);
                int locy = (int)((i / 4) * height);

                g.DrawImage(parallelccdlist[i].GetBMP(), new Rectangle(locx, locy, width, height));

                i++;
            }

            g.Dispose();

        }
        public void GetR32Image(string indexstr = "")
        {
            int i = 0;

            if (indexstr == "")
            {
                i = 0;

                while (i < GetCCDCount)
                {
                    indexstr += i.ToString() + ",";
                    i++;
                }

                indexstr = indexstr.Remove(indexstr.Length - 1, 1);
            }

            string[] strs = indexstr.Split(',');
            List<CCDWithRelateClass> ParallelCCDList = new List<CCDWithRelateClass>();

            foreach (string str in strs)
            {
                if (int.Parse(str) == 0)
                {
                    GetImage(0);
                }
                else
                {
                    foreach (CCDClass ccd in CCDList)
                    {

                        if (IsNoUseCCD)
                        {
                            if (ccd.CheckIndex(int.Parse(str)))
                            {
                                ParallelCCDList.Add(new CCDWithRelateClass(ccd, int.Parse(str)));
                                break;
                            }
                        }
                        else
                        {
                            int indextemp = int.Parse(str) + 9;
                            if (ccd.CheckIndex(indextemp))
                            {
                                ParallelCCDList.Add(new CCDWithRelateClass(ccd, int.Parse(str) + 9));
                                break;
                            }
                        }

                    }

                }
            }
            //foreach (CCDWithRelateClass ccd in ParallelCCDList)
            //{
            //    ccd.GetImage();
            //}

            Parallel.ForEach<CCDWithRelateClass>(ParallelCCDList, ccd =>
            {
                ccd.GetImage();
            });

            i = 0;
            foreach (string str in strs)
            {
                if (int.Parse(str) == 0)
                {
                    continue;
                }
                else
                {
                    int index = int.Parse(str);

                    Bitmap bmp = BMPList[index];
                    BMPList.RemoveAt(index);

                    bmp.Dispose();
                    bmp = new Bitmap(ParallelCCDList[i].GetBMP());
                    BMPList.Insert(index, bmp);

                    i++;
                }
            }
        }
        #endregion
        #region R26 Special Function
        public void GetR26AssembleImage(Bitmap bmp, List<CCDWithRelateClass> parallelccdlist)
        {
            int i = 0;

            Graphics g = Graphics.FromImage(bmp);

            Bitmap bmpsample = parallelccdlist[0].GetBMP();

            int width = bmpsample.Width;
            int height = bmpsample.Height;

            while (i < 6)
            {
                int locx = (int)((i % 4) * width);
                int locy = (int)((i / 4) * height);

                g.DrawImage(parallelccdlist[i].GetBMP(), new Rectangle(locx, locy, width, height));

                i++;
            }

            g.Dispose();

        }
        public void GetR26Image(string indexstr = "")
        {
            int i = 0;

            if (indexstr == "")
            {
                i = 0;

                while (i < GetCCDCount)
                {
                    indexstr += i.ToString() + ",";
                    i++;
                }

                indexstr = indexstr.Remove(indexstr.Length - 1, 1);
            }

            string[] strs = indexstr.Split(',');
            List<CCDWithRelateClass> ParallelCCDList = new List<CCDWithRelateClass>();

            foreach (string str in strs)
            {
                if (int.Parse(str) == 0)
                {
                    GetImage(0);
                }
                else
                {
                    foreach (CCDClass ccd in CCDList)
                    {

                        if (IsNoUseCCD)
                        {
                            if (ccd.CheckIndex(int.Parse(str)))
                            {
                                ParallelCCDList.Add(new CCDWithRelateClass(ccd, int.Parse(str)));
                                break;
                            }
                        }
                        else
                        {
                            int indextemp = int.Parse(str) + 5;
                            if (ccd.CheckIndex(indextemp))
                            {
                                ParallelCCDList.Add(new CCDWithRelateClass(ccd, int.Parse(str) + 5));
                                break;
                            }
                        }

                    }

                }
            }
            //foreach (CCDWithRelateClass ccd in ParallelCCDList)
            //{
            //    ccd.GetImage();
            //}

            Parallel.ForEach<CCDWithRelateClass>(ParallelCCDList, ccd =>
            {
                ccd.GetImage();
            });

            i = 0;
            foreach (string str in strs)
            {
                if (int.Parse(str) == 0)
                {
                    continue;
                }
                else
                {
                    int index = int.Parse(str);

                    Bitmap bmp = BMPList[index];
                    BMPList.RemoveAt(index);

                    bmp.Dispose();
                    bmp = new Bitmap(ParallelCCDList[i].GetBMP());
                    BMPList.Insert(index, bmp);

                    i++;
                }
            }
        }

        public void GetR15Image(string indexstr = "")
        {
            int i = 0;

            if (indexstr == "")
            {
                i = 0;

                while (i < GetCCDCount)
                {
                    indexstr += i.ToString() + ",";
                    i++;
                }

                indexstr = indexstr.Remove(indexstr.Length - 1, 1);
            }

            string[] strs = indexstr.Split(',');
            List<CCDWithRelateClass> ParallelCCDList = new List<CCDWithRelateClass>();

            foreach (string str in strs)
            {
                if (int.Parse(str) == 0)
                {
                    GetImage(0);
                }
                else
                {
                    foreach (CCDClass ccd in CCDList)
                    {

                        if (IsNoUseCCD)
                        {
                            if (ccd.CheckIndex(int.Parse(str)))
                            {
                                ParallelCCDList.Add(new CCDWithRelateClass(ccd, int.Parse(str)));
                                break;
                            }
                        }
                        else
                        {
                            int indextemp = int.Parse(str) + 5;
                            if (ccd.CheckIndex(indextemp))
                            {
                                ParallelCCDList.Add(new CCDWithRelateClass(ccd, int.Parse(str) + 5));
                                break;
                            }
                        }

                    }

                }
            }
            //foreach (CCDWithRelateClass ccd in ParallelCCDList)
            //{
            //    ccd.GetImage();
            //}

            Parallel.ForEach<CCDWithRelateClass>(ParallelCCDList, ccd =>
            {
                ccd.GetImage();
            });

            i = 0;
            foreach (string str in strs)
            {
                if (int.Parse(str) == 0)
                {
                    continue;
                }
                else
                {
                    int index = int.Parse(str);

                    Bitmap bmp = BMPList[index];
                    BMPList.RemoveAt(index);

                    bmp.Dispose();
                    bmp = new Bitmap(ParallelCCDList[i].GetBMP());
                    BMPList.Insert(index, bmp);

                    i++;
                }
            }
        }


        public void GetR9Image(string indexstr = "")
        {
            int i = 0;

            if (indexstr == "")
            {
                i = 0;

                while (i < GetCCDCount)
                {
                    indexstr += i.ToString() + ",";
                    i++;
                }

                indexstr = indexstr.Remove(indexstr.Length - 1, 1);
            }

            string[] strs = indexstr.Split(',');
            List<CCDWithRelateClass> ParallelCCDList = new List<CCDWithRelateClass>();

            foreach (string str in strs)
            {
                if (int.Parse(str) == 0)
                {
                    GetImage(0);
                }
                else
                {
                    foreach (CCDClass ccd in CCDList)
                    {

                        if (IsNoUseCCD)
                        {
                            if (ccd.CheckIndex(int.Parse(str)))
                            {
                                ParallelCCDList.Add(new CCDWithRelateClass(ccd, int.Parse(str)));
                                break;
                            }
                        }
                        else
                        {
                            int indextemp = int.Parse(str) + 5;
                            if (ccd.CheckIndex(indextemp))
                            {
                                ParallelCCDList.Add(new CCDWithRelateClass(ccd, int.Parse(str) + 5));
                                break;
                            }
                        }

                    }

                }
            }
            //foreach (CCDWithRelateClass ccd in ParallelCCDList)
            //{
            //    ccd.GetImage();
            //}

            Parallel.ForEach<CCDWithRelateClass>(ParallelCCDList, ccd =>
            {
                ccd.GetImage();
            });

            i = 0;
            foreach (string str in strs)
            {
                if (int.Parse(str) == 0)
                {
                    continue;
                }
                else
                {
                    int index = int.Parse(str);

                    Bitmap bmp = BMPList[index];
                    BMPList.RemoveAt(index);

                    bmp.Dispose();
                    bmp = new Bitmap(ParallelCCDList[i].GetBMP());
                    BMPList.Insert(index, bmp);

                    i++;
                }
            }
        }
        public void GetR3Image(string indexstr = "")
        {
            int i = 0;

            if (indexstr == "")
            {
                i = 0;

                while (i < GetCCDCount)
                {
                    indexstr += i.ToString() + ",";
                    i++;
                }

                indexstr = indexstr.Remove(indexstr.Length - 1, 1);
            }

            string[] strs = indexstr.Split(',');
            List<CCDWithRelateClass> ParallelCCDList = new List<CCDWithRelateClass>();

            foreach (string str in strs)
            {
                if (int.Parse(str) == 0)
                {
                    GetImage(0);
                }
                else
                {
                    foreach (CCDClass ccd in CCDList)
                    {

                        if (IsNoUseCCD)
                        {
                            if (ccd.CheckIndex(int.Parse(str)))
                            {
                                ParallelCCDList.Add(new CCDWithRelateClass(ccd, int.Parse(str)));
                                break;
                            }
                        }
                        else
                        {
                            int indextemp = int.Parse(str);
                            if (ccd.CheckIndex(indextemp))
                            {
                                ParallelCCDList.Add(new CCDWithRelateClass(ccd, int.Parse(str)));
                                break;
                            }
                        }

                    }

                }
            }

            Parallel.ForEach<CCDWithRelateClass>(ParallelCCDList, ccd =>
            {
                ccd.GetImage();
            });

            i = 0;
            foreach (string str in strs)
            {
                if (int.Parse(str) == 0)
                {
                    continue;
                }
                else
                {
                    int index = int.Parse(str);

                    Bitmap bmp = BMPList[index];
                    BMPList.RemoveAt(index);

                    bmp.Dispose();
                    bmp = new Bitmap(ParallelCCDList[i].GetBMP());
                    BMPList.Insert(index, bmp);

                    i++;
                }
            }
        }
        public void GetR1Image(string indexstr = "")
        {
            int i = 0;

            if (indexstr == "")
            {
                i = 0;

                while (i < GetCCDCount)
                {
                    indexstr += i.ToString() + ",";
                    i++;
                }

                indexstr = indexstr.Remove(indexstr.Length - 1, 1);
            }

            string[] strs = indexstr.Split(',');
            List<CCDWithRelateClass> ParallelCCDList = new List<CCDWithRelateClass>();

            foreach (string str in strs)
            {
                if (int.Parse(str) == 0)
                {
                    GetImage(0);
                }
                else
                {
                    foreach (CCDClass ccd in CCDList)
                    {

                        if (IsNoUseCCD)
                        {
                            if (ccd.CheckIndex(int.Parse(str)))
                            {
                                ParallelCCDList.Add(new CCDWithRelateClass(ccd, int.Parse(str)));
                                break;
                            }
                        }
                        else
                        {
                            int indextemp = int.Parse(str);
                            if (ccd.CheckIndex(indextemp))
                            {
                                ParallelCCDList.Add(new CCDWithRelateClass(ccd, int.Parse(str)));
                                break;
                            }
                        }

                    }

                }
            }

            Parallel.ForEach<CCDWithRelateClass>(ParallelCCDList, ccd =>
            {
                ccd.GetImage();
            });

            i = 0;
            foreach (string str in strs)
            {
                if (int.Parse(str) == 0)
                {
                    continue;
                }
                else
                {
                    int index = int.Parse(str);

                    Bitmap bmp = BMPList[index];
                    BMPList.RemoveAt(index);

                    bmp.Dispose();
                    bmp = new Bitmap(ParallelCCDList[i].GetBMP());
                    BMPList.Insert(index, bmp);

                    i++;
                }
            }
        }
        #endregion

        public int R5proindex = 0;

        public int iCCD1EX = 0;
        public int iCCD2EX = 0;
        public int iCCD1 = 0;
        public int iCCD2 = 0;
        /// <summary>
        /// 延时
        /// </summary>
        public int iNextDuriation = 150;

        public int iNextDuriationPLC = 200;
        /// <summary>
        /// 切换为80002取像
        /// </summary>
        bool is80002 = false;
        /// <summary>
        /// 是否要用80002取像
        /// </summary>
        public bool isGet80002 = true;
        Stopwatch stoptemp = new Stopwatch();
        Stopwatch stoptempPLC = new Stopwatch();


        public int R5RUNCount = 4;
        int R5RUNCountTemp = 0;
        public bool GetR5ImageOneTick(FatekPLCClass mplc)
        {
            iNextDuriationPLC = iNextDuriation;
            switch (R5proindex)
            {
                case 10:
                    //  if (isGet80002)
                    setR5CCDEX(false);
                    is80002 = false;
                    //       mplc.SetIO(false, "M0485");
                    mplc.SetIO(true, "M0482");
                    bmpR5list.Clear();
                    bmpR5list80002.Clear();
                    R5proindex = 15;
                    stoptempPLC.Restart();
                    R5RUNCountTemp = 0;
                    break;

                case 14:
                    //        mplc.SetIO(false, "M0485");
                    mplc.SetIO(true, "M0482");
                    R5RUNCountTemp = 0;
                    R5proindex = 15;
                    stoptempPLC.Restart();
                    break;
                case 15:
                    if (mplc.IOData.GetBit("M0505"))
                        R5proindex = 20;
                    if (stoptempPLC.ElapsedMilliseconds > 300)
                        R5proindex = 14;
                    if (mplc.IOData.GetBit("M0485"))
                        R5proindex = 25;
                    break;
                case 20:
                    if (mplc.IOData.GetBit("M0485"))
                        R5proindex = 25;
                    break;
                case 25:

                    if (R5RUNCountTemp < R5RUNCount)
                    {
                        stoptemp.Restart();
                        R5proindex = 26;
                    }
                    else
                        R5proindex = 100;

                    break;
                case 26:
                    if (stoptemp.ElapsedMilliseconds > iNextDuriation)
                        R5proindex = 30;
                    break;
                case 30:
                    GetImage(0);
                    R5RUNCountTemp++;
                    if (isGet80002)
                    {
                        is80002 = true;
                        setR5CCDEX(true);
                        R5proindex = 31;
                        stoptemp.Restart();
                    }
                    else
                        R5proindex = 35;

                    break;
                case 31:
                    if (stoptemp.ElapsedMilliseconds > iNextDuriationPLC)
                        R5proindex = 32;
                    break;
                case 32:
                    GetImage(0);
                    is80002 = false;
                    setR5CCDEX(false);
                    R5proindex = 35;
                    break;
                case 35:
                    //    mplc.SetIO(false, "M0485");
                    mplc.SetIO(true, "M0486");
                    R5proindex = 39;
                    stoptempPLC.Restart();
                    //   stoptemp.Restart();
                    break;
                case 39:
                    if (stoptempPLC.ElapsedMilliseconds > iNextDuriationPLC)
                        R5proindex = 40;
                    break;
                case 40:
                    if (mplc.IOData.GetBit("M0485"))
                        R5proindex = 25;

                    if (R5RUNCountTemp >= R5RUNCount)
                        R5proindex = 100;

                    break;
                case 100:
                    R5proindex = 0;

                    BMPList[0] = GetR5AssembleImage();
                    if (isGet80002)
                        GetR5AssembleImage80002(out bmp80002);
                    return true;
            }

            return false;
        }

        public bool GetR5ImageOneTick(MotoRs485.SetMotorForm setMotor)
        {
            iNextDuriationPLC = iNextDuriation;
            switch (R5proindex)
            {
                case 10:
                    // if (isGet80002)
                    setR5CCDEX(false);
                    is80002 = false;
                    bmpR5list.Clear();
                    bmpR5list80002.Clear();
                    R5proindex = 20;
                    R5RUNCountTemp = 0;
                    break;
                case 20:
                    if (R5RUNCountTemp < R5RUNCount)
                    {
                        stoptemp.Restart();
                        R5proindex = 30;
                    }
                    else
                        R5proindex = 100;
                    break;
                case 30:
                    setMotor.GoPosition(R5RUNCountTemp);

                    R5proindex = 40;
                    stoptempPLC.Restart();
                    break;
                case 40:
                    if (setMotor.IsGoPositionOK(R5RUNCountTemp))
                    {
                        R5proindex = 50;
                        stoptemp.Restart();
                    }
                    break;
                case 50:
                    if (stoptemp.ElapsedMilliseconds > iNextDuriation)
                        R5proindex = 60;
                    break;
                case 60:
                    GetImage(0);
                    R5RUNCountTemp++;
                    if (isGet80002)
                    {
                        is80002 = true;
                        setR5CCDEX(true);
                        R5proindex = 70;
                        stoptemp.Restart();
                    }
                    else
                        R5proindex = 20;

                    break;
                case 70:
                    if (stoptemp.ElapsedMilliseconds > iNextDuriationPLC)
                        R5proindex = 80;
                    break;
                case 80:
                    GetImage(0);
                    is80002 = false;
                    setR5CCDEX(false);
                    R5proindex = 20;
                    break;
                case 100:
                    setMotor.GoPosition(0);
                    R5proindex = 0;

                    BMPList[0] = GetR5AssembleImage();
                    if (isGet80002)
                        GetR5AssembleImage80002(out bmp80002);
                    return true;
            }

            return false;
        }

        void setR5CCDEX(bool ismy80002 = false)
        {
            if (!ismy80002)
            {
                if (iCCD1 != -1 && iCCD2 != -1)
                {
                    SetExposure(iCCD1, 0);
                    SetExposure(iCCD2, 1);
                }
            }
            else
            {
                if (iCCD1EX != -1 && iCCD2EX != -1)
                {
                    SetExposure(iCCD1EX, 0);
                    SetExposure(iCCD2EX, 1);
                }
            }
        }
        public void GetR5Image(string indexstr = "")
        {
            int i = 0;

            if (indexstr == "")
            {
                i = 0;

                while (i < GetCCDCount)
                {
                    indexstr += i.ToString() + ",";
                    i++;
                }

                indexstr = indexstr.Remove(indexstr.Length - 1, 1);
            }

            string[] strs = indexstr.Split(',');
            List<CCDWithRelateClass> ParallelCCDList = new List<CCDWithRelateClass>();

            foreach (string str in strs)
            {
                if (int.Parse(str) == 0)
                {
                    GetImage(0);
                }
                else
                {
                    foreach (CCDClass ccd in CCDList)
                    {

                        if (IsNoUseCCD)
                        {
                            if (ccd.CheckIndex(int.Parse(str)))
                            {
                                ParallelCCDList.Add(new CCDWithRelateClass(ccd, int.Parse(str)));
                                break;
                            }
                        }
                        else
                        {
                            int indextemp = int.Parse(str) + 1;
                            if (ccd.CheckIndex(indextemp))
                            {
                                ParallelCCDList.Add(new CCDWithRelateClass(ccd, int.Parse(str) + 1));
                                break;
                            }
                        }

                    }

                }
            }
            //foreach (CCDWithRelateClass ccd in ParallelCCDList)
            //{
            //    ccd.GetImage();
            //}

            Parallel.ForEach<CCDWithRelateClass>(ParallelCCDList, ccd =>
            {
                ccd.GetImage();
            });

            i = 0;
            foreach (string str in strs)
            {
                if (int.Parse(str) == 0)
                {
                    continue;
                }
                else
                {
                    int index = int.Parse(str);

                    Bitmap bmp = BMPList[index];
                    BMPList.RemoveAt(index);

                    bmp.Dispose();
                    bmp = new Bitmap(ParallelCCDList[i].GetBMP());
                    BMPList.Insert(index, bmp);

                    i++;
                }
            }
        }
        public void GetR5AssembleImage80002(out Bitmap bmp)
        {
            int i = 0;
            Bitmap bmpsample = bmpR5list80002[0];

            int width = bmpsample.Width;
            int height = bmpsample.Height;
            //bmp = new Bitmap(width * 4, height * 3);
            //Graphics g = Graphics.FromImage(bmp);

            //i = 0;
            //for (int j = 0; j < bmpR5list80002.Count; j += 2)
            //{
            //    g.DrawImage(bmpR5list80002[j], new Rectangle(i * width, 0, width, height));

            //    i++;
            //    //bmpR5list[j].Save("D:\\1-"+i+".png");
            //}
            //i = 0;
            //for (int j = 1; j < bmpR5list80002.Count; j += 2)
            //{
            //    g.DrawImage(bmpR5list80002[j], new Rectangle(i * width, height, width, height));

            //    i++;
            //    //bmpR5list[j].Save("D:\\2-" + i + ".png");
            //}


            if (R5RUNCount == 5 && bmpR5list80002.Count >= 10)
            {
                bmp = new Bitmap(width * 4, height * 3);
                Graphics g = Graphics.FromImage(bmp);
                g.FillRectangle(new SolidBrush(Color.Black), new RectangleF(0, 0, bmp.Width, bmp.Height));

                i = 0;
                for (int j = 0; j < bmpR5list80002.Count; j += 2)
                {
                    g.DrawImage(bmpR5list80002[j], new Rectangle(i * width, 0, width, height));
                    i++;

                    if (i == 4)
                        break;
                }
                i = 0;
                for (int j = 1; j < bmpR5list80002.Count; j += 2)
                {
                    g.DrawImage(bmpR5list80002[j], new Rectangle(i * width, height * 2, width, height));

                    i++;
                    //bmpR5list[j].Save("D:\\2-" + i + ".png");

                    if (i == 4)
                        break;
                }
                g.DrawImage(bmpR5list80002[8], new Rectangle(0, height, width, height));
                g.DrawImage(bmpR5list80002[9], new Rectangle(width * 3, height, width, height));

                //bmpR5list[j].Save("D:\\2-" + i + ".png");
                g.Dispose();
            }
            else
            {
                bmp = new Bitmap(width * 4, height * 2);
                Graphics g = Graphics.FromImage(bmp);
                g.FillRectangle(new SolidBrush(Color.Black), new RectangleF(0, 0, bmp.Width, bmp.Height));

                //改为第一个相机拍的图放在图片的第一排
                //第二个相机拍的图放在第二排
                i = 0;
                for (int j = 0; j < bmpR5list80002.Count; j += 2)
                {
                    g.DrawImage(bmpR5list80002[j], new Rectangle(i * width, 0, width, height));
                    i++;
                    //bmpR5list[j].Save("D:\\1-"+i+".png");
                }
                i = 0;
                for (int j = 1; j < bmpR5list80002.Count; j += 2)
                {
                    g.DrawImage(bmpR5list80002[j], new Rectangle(i * width, height, width, height));
                    i++;
                    //bmpR5list[j].Save("D:\\2-" + i + ".png");
                }
                g.Dispose();
            }

        }
        public Bitmap GetR5AssembleImage()
        {
            Bitmap bmp;
            int i = 0;
            Bitmap bmpsample = bmpR5list[0];
            int width = bmpsample.Width;
            int height = bmpsample.Height;

            if (R5RUNCount == 5 && bmpR5list.Count >= 10)
            {
                bmp = new Bitmap(width * 4, height * 3);
                Graphics g = Graphics.FromImage(bmp);
                g.FillRectangle(new SolidBrush(Color.Black), new RectangleF(0, 0, bmp.Width, bmp.Height));

                i = 0;
                for (int j = 0; j < bmpR5list.Count; j += 2)
                {
                    //int locx = (int)((i % 4) * width);
                    //int locy = (int)((i / 4) * height);

                    g.DrawImage(bmpR5list[j], new Rectangle(i * width, 0, width, height));

                    i++;

                    if (i == 4)
                        break;
                    //bmpR5list[j].Save("D:\\1-"+i+".png");
                }
                i = 0;
                for (int j = 1; j < bmpR5list.Count; j += 2)
                {
                    //int locx = (int)((i % 4) * width);
                    //int locy = (int)((i / 4) * height);

                    g.DrawImage(bmpR5list[j], new Rectangle(i * width, height * 2, width, height));

                    i++;
                    //bmpR5list[j].Save("D:\\2-" + i + ".png");

                    if (i == 4)
                        break;
                }
                g.DrawImage(bmpR5list[8], new Rectangle(0, height, width, height));
                g.DrawImage(bmpR5list[9], new Rectangle(width * 3, height, width, height));

                //bmpR5list[j].Save("D:\\2-" + i + ".png");
                g.Dispose();
            }
            else
            {
                bmp = new Bitmap(width * 4, height * 2);
                Graphics g = Graphics.FromImage(bmp);
                g.FillRectangle(new SolidBrush(Color.Black), new RectangleF(0, 0, bmp.Width, bmp.Height));

                //改为第一个相机拍的图放在图片的第一排
                //第二个相机拍的图放在第二排
                i = 0;
                for (int j = 0; j < bmpR5list.Count; j += 2)
                {
                    //int locx = (int)((i % 4) * width);
                    //int locy = (int)((i / 4) * height);

                    g.DrawImage(bmpR5list[j], new Rectangle(i * width, 0, width, height));

                    i++;
                    //bmpR5list[j].Save("D:\\1-"+i+".png");
                }
                i = 0;
                for (int j = 1; j < bmpR5list.Count; j += 2)
                {
                    //int locx = (int)((i % 4) * width);
                    //int locy = (int)((i / 4) * height);

                    g.DrawImage(bmpR5list[j], new Rectangle(i * width, height, width, height));

                    i++;
                    //bmpR5list[j].Save("D:\\2-" + i + ".png");
                }
                g.Dispose();
            }

            //bmp.Save("D:\\test.png");
            return bmp;
        }

        /// <summary>
        /// This fuction maybe replace by GetImageFX
        /// </summary>
        /// <param name="indexstring"></param>
        public void GetImage(string indexstring)
        {
            //JzTimes jztime = new JzTimes();
            //jztime.Cut();
            int i = 0;
            string[] strs = indexstring.Split(',');

            foreach (string str in strs)
            {
                GetImage(int.Parse(str));

                //foreach (CCDClass ccd in CCDList)
                //{
                //    if (ccd.GetImage(BMPList, int.Parse(strs[i])))
                //    {
                //        break;
                //    }
                //}

                //i++;
            }

            //int dur = jztime.msDuriation;


        }
        /// <summary>
        /// This fuction maybe replace by GetImageFX
        /// </summary>
        /// <param name="indexstring"></param>
        public void GetImageEX(string indexstring)
        {
            //JzTimes jztime = new JzTimes();
            //jztime.Cut();

            int i = 0;
            string[] strs = indexstring.Split(',');

            List<CCDClass> ccdlist = new List<CCDClass>();
            List<string> strslist = new List<string>();

            strslist.Clear();
            foreach (string str in strs)
            {
                strslist.Add(str);
                //foreach (CCDClass ccd in CCDList)
                //{
                //    if (ccd.CheckIndex(int.Parse(strs[i])))
                //    {
                //        ccd.Relationbmpindex = int.Parse(strs[i]);
                //        ccdlist.Add(ccd);
                //        break;
                //    }
                //}
                //i++;
            }
            //Bitmap[] bmp = new Bitmap[strslist.Count];

            Parallel.ForEach<string>(strslist, stritem =>
            {
                foreach (CCDClass ccd in CCDList)
                {
                    int index = int.Parse(stritem);
                    if (ccd.CheckIndex(index))
                    {
                        //Bitmap bmp = new Bitmap(1, 1);
                        //  ccd.GetImageEX(BMPList[index], index);

                        ccd.GetImage(index);
                        //BMPList[iindex] = new Bitmap(bmp);
                        //bmp.Dispose();
                    }
                }
            }
           );

            //int dur = jztime.msDuriation;
            //Parallel.ForEach<CCDClass>(ccdlist, ccditem =>
            ////foreach (ENHANCEClass enhance in side.ENHANCEList)
            //{
            //    ccditem.GetImageEX(BMPList[ccditem.Relationbmpindex], ccditem.Relationbmpindex);
            //}
            //);

        }
        public void GetImageFX(string indexstr)
        {
            int i = 0;

            if (indexstr == "")
            {
                i = 0;

                while (i < GetCCDCount)
                {
                    indexstr += i.ToString() + ",";
                    i++;
                }

                indexstr = indexstr.Remove(indexstr.Length - 1, 1);
            }

            string[] strs = indexstr.Split(',');
            List<CCDWithRelateClass> ParallelCCDList = new List<CCDWithRelateClass>();

            foreach (string str in strs)
            {
                foreach (CCDClass ccd in CCDList)
                {
                    if (ccd.CheckIndex(int.Parse(str)))
                    {
                        ParallelCCDList.Add(new CCDWithRelateClass(ccd, int.Parse(str)));
                        break;
                    }
                }
            }

            //if()
            //foreach(CCDWithRelateClass ccd in ParallelCCDList)
            //{
            //    //Thread.Sleep(200);
            //    ccd.GetImage(BMPList);
            //}
            Parallel.ForEach<CCDWithRelateClass>(ParallelCCDList, ccd =>
            {
                //Thread.Sleep(200);
                ccd.GetImage(BMPList);
            });
        }
        public void GetImageFX(string indexstr, int iDelayCaptureTime = 100)
        {
            int i = 0;

            if (indexstr == "")
            {
                i = 0;

                while (i < GetCCDCount)
                {
                    indexstr += i.ToString() + ",";
                    i++;
                }

                indexstr = indexstr.Remove(indexstr.Length - 1, 1);
            }

            string[] strs = indexstr.Split(',');
            List<CCDWithRelateClass> ParallelCCDList = new List<CCDWithRelateClass>();

            foreach (string str in strs)
            {
                foreach (CCDClass ccd in CCDList)
                {
                    if (ccd.CheckIndex(int.Parse(str)))
                    {
                        ParallelCCDList.Add(new CCDWithRelateClass(ccd, int.Parse(str)));
                        break;
                    }
                }
            }
            //foreach (CCDWithRelateClass ccd in ParallelCCDList)
            //{
            //    //Thread.Sleep(200);
            //    ccd.GetImage(BMPList);
            //}
            Parallel.ForEach<CCDWithRelateClass>(ParallelCCDList, ccd =>
            {
                if (iDelayCaptureTime != 0)
                    Thread.Sleep(iDelayCaptureTime);
                ccd.GetImage(BMPList);
            });
        }
        public Bitmap GetCombineBMP(string combinindexstring)
        {
            string[] strs = combinindexstring.Split(',');

            int width = BMPList[int.Parse(strs[0])].Height;
            int height = BMPList[int.Parse(strs[0])].Width;

            Size sz = new Size(width * 3, height);

            Bitmap bmp = new Bitmap(sz.Width, sz.Height);

            Graphics g = Graphics.FromImage(bmp);

            Bitmap bmprotate = new Bitmap(BMPList[int.Parse(strs[0])]);
            bmprotate.RotateFlip(RotateFlipType.Rotate270FlipNone);

            g.DrawImage(bmprotate, new Rectangle(0, 0, width, height), new Rectangle(0, 0, width, height), GraphicsUnit.Pixel);

            bmprotate.Dispose();
            bmprotate = new Bitmap(BMPList[int.Parse(strs[1])]);
            bmprotate.RotateFlip(RotateFlipType.Rotate270FlipNone);

            g.DrawImage(bmprotate, new Rectangle(width, 0, width, height), new Rectangle(0, 0, width, height), GraphicsUnit.Pixel);

            bmprotate.Dispose();
            bmprotate = new Bitmap(BMPList[int.Parse(strs[2])]);
            bmprotate.RotateFlip(RotateFlipType.Rotate270FlipNone);

            g.DrawImage(bmprotate, new Rectangle(width * 2, 0, width, height), new Rectangle(0, 0, width, height), GraphicsUnit.Pixel);
            g.Dispose();

            bmprotate.Dispose();

            return bmp;
        }
        public Bitmap GetCombineBMPEX(string combinindexstring)
        {
            string[] strs = combinindexstring.Split(',');

            int width = BMPList[int.Parse(strs[0])].Width;
            int height = BMPList[int.Parse(strs[0])].Height;

            Size sz = new Size(width, height * 3);

            Bitmap bmp = new Bitmap(sz.Width, sz.Height);

            Graphics g = Graphics.FromImage(bmp);

            g.DrawImage(BMPList[int.Parse(strs[0])], new Rectangle(0, 0, width, height), new Rectangle(0, 0, width, height), GraphicsUnit.Pixel);

            g.DrawImage(BMPList[int.Parse(strs[1])], new Rectangle(0, height, width, height), new Rectangle(0, 0, width, height), GraphicsUnit.Pixel);

            g.DrawImage(BMPList[int.Parse(strs[2])], new Rectangle(0, height * 2, width, height), new Rectangle(0, 0, width, height), GraphicsUnit.Pixel);
            g.Dispose();

            return bmp;
        }
        public void GetImage()
        {
            //int i = 0;

            //while (i < GetCCDCount)
            //{
            //    GetImage(i);
            //    i++;
            //}

            Parallel.For(0, GetCCDCount, i =>
            {
                GetImage(i);
            });


        }
        public void GetImage(bool isfirsttime)
        {



        }
        public void SetExposure(float exposevalue, int relateindex, bool isindexADD = false)
        {

            int biasbvalue = 0;

            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch (OPTION)
                    {
                        case OptionEnum.R32:
                            biasbvalue = 9;
                            break;
                        case OptionEnum.R26:
                        case OptionEnum.R15:
                        case OptionEnum.R9:
                            biasbvalue = 5;
                            break;
                        case OptionEnum.R5:
                            biasbvalue = 1;
                            break;
                        case OptionEnum.R1:
                            biasbvalue = 0;
                            break;
                        case OptionEnum.R3:
                        case OptionEnum.C3:
                            biasbvalue = 0;
                            break;

                    }
                    break;
            }

            if (!isindexADD || relateindex == 0)
                biasbvalue = 0;
            foreach (CCDClass ccd in CCDList)
            {
                if (ccd.SetExposure(exposevalue, relateindex + biasbvalue))
                {
                    break;
                }
            }
        }
        public void SetExposure(string exposurestr, int relateindex)
        {
            int i = 0;

            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch (OPTION)
                    {
                        case OptionEnum.R32:
                        case OptionEnum.R26:
                        case OptionEnum.R15:
                        case OptionEnum.R9:
                        case OptionEnum.R5:
                        case OptionEnum.R3:
                        case OptionEnum.C3:
                        case OptionEnum.R1:
                            switch (relateindex)
                            {
                                case 0:

                                    if (exposurestr.IndexOf(':') < 0)
                                    {
                                        int val = 0;

                                        if (int.TryParse(exposurestr, out val))
                                            SetExposure(val, 0);
                                    }
                                    else
                                    {
                                        string[] strs = exposurestr.Split(',');

                                        i = 0;
                                        foreach (string str in strs)
                                        {
                                            string[] strsx = str.Split(':');
                                            SetExposure(float.Parse(strsx[1]), int.Parse(strsx[0]));

                                            i++;
                                        }
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
            }



            //foreach (CCDClass ccd in CCDList)
            //{
            //    if (ccd.SetExposure(exposurestr, relateindex))
            //    {
            //        break;
            //    }
            //}
        }
        public void SetExposure(string camstring)
        {
            if (camstring.Trim() == "")
                return;

            int i = 0;
            //int biasvalue = 0;
            string[] strs = camstring.Split(';');

            foreach (string str in strs)
            {
                string[] strss = strs[i].Split('#');

                switch (VERSION)
                {
                    case VersionEnum.ALLINONE:
                        switch (OPTION)
                        {
                            case OptionEnum.R32:
                            case OptionEnum.R26:
                            case OptionEnum.R15:
                            case OptionEnum.R9:
                            case OptionEnum.R5:
                            case OptionEnum.R3:
                            case OptionEnum.C3:
                            case OptionEnum.R1:
                                switch (int.Parse(strss[0]))
                                {
                                    case 0:
                                        SetExposure(strss[1], int.Parse(strss[0]) + int.Parse(strss[2]));
                                        break;
                                    default:
                                        SetExposure(float.Parse(strss[1]), int.Parse(strss[0]) + int.Parse(strss[2]));
                                        break;
                                }
                                break;
                            default:
                                SetExposure(float.Parse(strss[1]), int.Parse(strss[0]));
                                break;
                        }
                        break;
                    default:
                        SetExposure(float.Parse(strss[1]), int.Parse(strss[0]) + int.Parse(strss[2]));
                        break;
                }

                i++;
            }
        }

        public int SetPropetry(CCDProcAmpProperty cCDProcAmpProperty, int relateindex, int Value)
        {
#if ICAM
            foreach (CCDClass ccd in CCDList)
               return ccd.SetProperty(relateindex, cCDProcAmpProperty, Value);
#endif
            return -1;
        }
        public int SetPropetry(IntPtr intPtr, int relateindex)
        {
#if ICAM
            foreach (CCDClass ccd in CCDList)
            {
                if (ccd.CheckIndex(relateindex))
                {
                    ccd.SetProperty(intPtr, relateindex);
                    break;
                }
            }
#endif
            return 0;
        }
        public void GetRangeProcAmpProperty(
            int relateindex,
            CCDProcAmpProperty property,
            ref int max,
            ref int min,
            ref int Value,
            ref int defaultValue)
        {
#if ICAM
            foreach (CCDClass ccd in CCDList)
                ccd.GetRangeProcAmpProperty(relateindex, property, ref max,ref min ,ref Value,ref defaultValue);
#endif
        }
        public void Close()
        {
            foreach (CCDClass ccd in CCDList)
            {
                ccd.Close();
            }
        }

        public void Reset()
        {
            if (IsNoUseCCD)
                return;

            foreach (CCDClass ccd in CCDList)
            {
                ccd.IsConnectionFail = false;
            }
        }
        public void Tick()
        {
            if (IsNoUseCCD)
                return;

            foreach (CCDClass ccd in CCDList)
            {
                ccd.Tick();

                switch (ccd.CCDType)
                {
                    case CCDTYPEEnum.EPIX:
                        IsConnectFail = ccd.IsConnectionFail;
                        break;
                }
            }
        }

        //當有Input Trigger時，產生OnTrigger
        public delegate void TriggerHandler(string operationstr);
        public event TriggerHandler TriggerAction;
        public void OnTrigger(String operationstr)
        {
            if (TriggerAction != null)
            {
                TriggerAction(operationstr);
            }
        }
    }

    public class CCDClass           //每型CCD不同使用方式的定義
    {
        VersionEnum VERSION = VersionEnum.ALLINONE;
        OptionEnum OPTION = OptionEnum.MAIN_X6;


#if EPIX
        #region Library

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
        private static extern int pxd_goSnap(int c_unitmap, int c_buffer);
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
        [DllImport("XCLIBW64.dll")]
        //public static extern int pxd_SILICONVIDEO_setExposureColorGainOffsets(int c_unitmap, int c_rsvd, double c_exposure, double c_digitalgain, double c_rsvd2, double c_rsvd3, double c_rsvd4);
        public static extern int pxd_SILICONVIDEO_setExposureColorGainOffsets(int unitmap, int rsvd, double exposure, IntPtr gainsA, IntPtr gainsB, IntPtr offsetsA, IntPtr offsetsB);

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
#endif
#if ICAM
        Camera[] CamerList;
#endif
#if PTG
        
        ManagedImage[] PTGRowImage;
        ManagedImage[] PTGProcessedImage;

        ManagedBusManager PTGBusManage;

        ManagedPGRGuid[] PTGCAMGuid;
        ManagedGigECamera[] PTGCAM;

        CameraProperty[] PTGCAMProperty;

#endif
#if TIS || TISUSB


        int TISMaxGain = 24;
        int TISMinGain = 4;

        int TISRangeDiff
        {
            get
            {
                return TISMaxGain - TISMinGain;
            }
        }

        //FrameSnapSink[] snapSinks;
        FrameFilter RotateFlipFilter;
        public ICImagingControl[] TISCAM;
        TIS.Imaging.ImageBuffer[] TISImageBuffer;
        VCDSimpleProperty[] TISCAMTUNING;
        VCDSwitchProperty[] TISPolaritySwitch;
        JzToolsClass JzTools = new JzToolsClass();
        //USE FOR USB TRIGGER
        VCDSwitchProperty[] TISTriggerEnable;
        VCDSwitchProperty[] TISBrightnessSwitch;
        VCDButtonProperty[] TISSoftTrigger;
        List<string> CamSizeArray = new List<string>();
#endif

#if IWIN
        #region IWIN_library
        CameraHandle[] m_hCamera;             // 句柄
        //IntPtr[] m_ImageBufferSnapshot;     // 抓拍通道RGB图像缓存
        tSdkCameraCapbility[] tCameraCapability;  // 相机特性描述

        IntPtr[] m_ImageBuffer;             // 预览通道RGB图像缓存
        //int[] m_iDisplayedFrames;    //已经显示的总帧数
        //CAMERA_SNAP_PROC[] m_CaptureCallback;
        //IntPtr[] m_iCaptureCallbackCtx;     //图像回调函数的上下文参数
        //Thread m_tCaptureThread;          //图像抓取线程
        //bool m_bExitCaptureThread = false;//采用线程采集时，让线程退出的标志
        //IntPtr m_iSettingPageMsgCallbackCtx; //相机配置界面消息回调函数的上下文参数   
        //tSdkFrameHead[] m_tFrameHead;
        tSdkImageResolution[] tResolution;

        int WIND_Width = 2048;
        int WIND_Height = 1536;

        bool m_wind_live = false;
        PictureBox SnapshotBox;
        public List<Control> IWindControlList = new List<Control>();

        //bool[] m_bEraseBk = false;
        //bool[] m_bSaveImage = false;

        JzToolsClass JzTools = new JzToolsClass();
        #endregion
#endif
#if MVS
        struct _MV_MATCH_INFO_NET_DETECT_
        {
            public UInt64 nReviceDataSize;    // 已接收数据大小  [统计StartGrabbing和StopGrabbing之间的数据量]
            public UInt32 nLostPacketCount;   // 丢失的包数量
            public uint nLostFrameCount;    // 丢帧数量
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public uint[] nReserved;          // 保留
        };

        /// <summary>
        /// 海康相机 定义相机结构体
        /// </summary>
        public struct CAMERA//定义相机结构体
        {
            public MyCamera Cam_Info;
            public uint m_nBufSizeForSaveImage;
            public byte[] m_pBufForSaveImage;         // 用于保存图像的缓存
            public bool isGetBmp;
            public Bitmap Bmp;


            /// <summary>
            /// 相机编号
            /// </summary>
            public int index { get; set; }
            /// <summary>
            /// 相机型号
            /// </summary>
            public string ModelName { get; set; }
            /// <summary>
            /// 相机序列号
            /// </summary>
            public string CamerSerialNumber { get; set; }
            /// <summary>
            /// 图片旋转
            /// </summary>
            public RotateFlipType Rotate { get; set; }
        }
        /// <summary>
        /// 海康相机触发方式枚举
        /// </summary>
        public enum TriggerMode : int
        {
            Line0 = 0,
            Line1 = 1,
            Line2 = 2,
            Line3 = 3,
            Counter = 4,
            Software = 7,
        }
        /// <summary>
        /// 海康相机参数
        /// </summary>
        class CameraPar
        {

            /// <summary>
            /// 相机编号
            /// </summary>
            public int index { get; set; }
            /// <summary>
            /// 相机型号
            /// </summary>
            public string ModelName { get; set; }
            /// <summary>
            /// 相机序列号
            /// </summary>
            public string CamerSerialNumber { get; set; }
            /// <summary>
            /// 图片旋转
            /// </summary>
            public RotateFlipType Rotate { get; set; }

            /// <summary>
            /// 保存路径
            /// </summary>
            public string FilePath { get; set; }
            /// <summary>
            /// 文件名
            /// </summary>
            public string FileName
            {
                get
                {
                    return index  + "_" + CamerSerialNumber;
                }
            }

            /// <summary>
            /// 保存属性
            /// </summary>
            /// <param name="camera">相机对像</param>
            /// <returns>是否保存成功</returns>
            public bool Save(MyCamera camera)
            {

                if (!Directory.Exists(FilePath + "\\MVS\\"))//若文件夹不存在则新建文件夹  
                    Directory.CreateDirectory(FilePath + "\\MVS\\"); //新建文件夹  

                string strfileparh = FilePath +"\\MVS\\"+ FileName + ".par";

                int nRet = camera.MV_CC_FeatureSave_NET(strfileparh);

                if (MyCamera.MV_OK != nRet)
                    return false;
                else
                    return true;
            }
            /// <summary>
            /// 载入属性
            /// </summary>
            /// <param name="camera">相机对像</param>
            /// <returns>是否保存成功</returns>
            public bool Load(MyCamera camera)
            {

                if (!Directory.Exists(FilePath + "\\MVS\\"))//若文件夹不存在则新建文件夹  
                    Directory.CreateDirectory(FilePath + "\\MVS\\"); //新建文件夹  

                string strfileparh = FilePath + "\\MVS\\" + FileName + ".par";
                

              int  nRet = camera.MV_CC_FeatureLoad_NET(strfileparh);

                if (MyCamera.MV_OK != nRet)
                    return false;
                else
                    return true;
            }
        }

        MyCamera.cbOutputdelegate cbImage;
        MyCamera.MV_CC_DEVICE_INFO_LIST m_pDeviceList;
        public CAMERA[] m_pMyCamera;

        /// <summary>
        /// 设备使用数量
        /// </summary>
        public int m_nCanOpenDeviceNum;
        /// <summary>
        /// 在线设备数量
        /// </summary>
        public int m_nDevNum;
        /// <summary>
        /// 相机帧数
        /// </summary>
        int[] m_nFrames;
        IntPtr[] m_hDisplayHandle;
#endif

#if DVP2

        #region 度申相机

        Dvp2Class[] m_dvp2Class;
        int dvp2Init(string eWorkPath)
        {
            m_dvp2Class = new Dvp2Class[CCDRelateIndexArray.Count()];

            string Str = string.Empty;
            string _pathname = eWorkPath + "\\CCDSEQ.INI";
            if (!File.Exists(_pathname))
                Dvp2Class.DeviceListAcq(_pathname);
            System.Threading.Thread.Sleep(100);
            JzTools.ReadData(ref Str, _pathname);

            string[] strs;
            Str = Str.Replace(Environment.NewLine, "@");
            strs = Str.Split('@');

            int i = 0;
            foreach (string str in strs)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    m_dvp2Class[i] = new Dvp2Class();

                    string[] vs = str.Split(',');

                    m_dvp2Class[i].Init(vs[0], eWorkPath);
                    if (vs.Length > 1)
                        m_dvp2Class[i].Rotate = int.Parse(vs[1]);

                    i++;
                }
            }

            return 0;
        }
        void dvp2Dispose()
        {
            if (m_dvp2Class != null)
            {
                foreach (var dvp2Class in m_dvp2Class)
                {
                    dvp2Class.Dispose();
                }
            }
        }
        void dvp2SetExpo(float expo)
        {
            if (m_dvp2Class != null)
            {
                foreach (var dvp2Class in m_dvp2Class)
                {
                    dvp2Class.SetExposure(expo);
                }
            }
        }
        void dvp2SetExpo(int index, float expo)
        {
            if (index >= m_dvp2Class.Length)
                return;
            if (m_dvp2Class != null)
            {
                m_dvp2Class[index].SetExposure(expo);
            }
        }
        void dvp2SetGain(float gain)
        {
            if (m_dvp2Class != null)
            {
                foreach (var dvp2Class in m_dvp2Class)
                {
                    dvp2Class.SetGain(gain);
                }
            }
        }
        void dvp2SetGain(int index, float gain)
        {
            if (index >= m_dvp2Class.Length)
                return;
            if (m_dvp2Class != null)
            {
                m_dvp2Class[index].SetGain(gain);
            }
        }
        Bitmap dvp2GetImageNow(int index)
        {
            if (index >= m_dvp2Class.Length)
                return null;
            Bitmap bmp = null;
            if (m_dvp2Class != null)
            {
                bmp = m_dvp2Class[index].GetImageNow();
            }
            return bmp;
        }

        #endregion

#endif

        public static string SystemWORKPATH = "";
        public static string WORKPATH = "";
        public static string ENVPATH = "";
        public static string PAGEOPTYPE = "";

        public CCDTYPEEnum CCDType = CCDTYPEEnum.FILE;
        public int[] CCDRelateIndexArray;
        int[] RotationDegreeArray;
        float[] ExposureBaseArray;
        float[] FrameRateArray;
        string[] ErrorCaptureArray;
        int[] LastCapturedCount;
        SizeDefClass[] SizeDefArray;

        public int Relationbmpindex = 0;
        Bitmap[] bmpR32Captured;

        public string ErrorConnection = "";
        public CCDClass(string ccdinfostr, VersionEnum version, OptionEnum option)
        {
            int i = 0;
            string[] infostrs = ccdinfostr.Split('#');

            VERSION = version;
            OPTION = option;


            CCDType = (CCDTYPEEnum)Enum.Parse(typeof(CCDTYPEEnum), infostrs[0], true);
            CCDRelateIndexArray = Array.ConvertAll(infostrs[1].Split(','), int.Parse);
            RotationDegreeArray = Array.ConvertAll(infostrs[2].Split(','), int.Parse);
            ExposureBaseArray = Array.ConvertAll(infostrs[3].Split(','), float.Parse);
            FrameRateArray = Array.ConvertAll(infostrs[4].Split(','), float.Parse);
            SizeDefArray = GetSizeDef(infostrs[5]);

            LastCapturedCount = new int[FrameRateArray.Length];

            ErrorCaptureArray = new string[FrameRateArray.Length];
            CountErrorRetry = new int[FrameRateArray.Length];
            LastCount = new int[FrameRateArray.Length];
            SideErrorRetry = new int[FrameRateArray.Length];


            switch (VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch (OPTION)
                    {
                        case OptionEnum.R32:
                        case OptionEnum.R26:
                        case OptionEnum.R15:
                        case OptionEnum.R9:
                        case OptionEnum.R5:
                        case OptionEnum.R3:
                        case OptionEnum.C3:
                        case OptionEnum.R1:
                            bmpR32Captured = new Bitmap[CCDRelateIndexArray.Length];
                            while (i < CCDRelateIndexArray.Length)
                            {
                                Size size = SizeDefArray[i].ToSize;
                                bmpR32Captured[i] = new Bitmap(size.Width, size.Height);

                                i++;
                            }

                            break;
                    }
                    break;
                default:

                    break;
            }



        }
        //public bool Initial(List<Bitmap> bmplist)
        public bool Initial()
        {
            bool ret = false;

            switch (CCDType)
            {
                case CCDTYPEEnum.FILE:

                    //foreach(int ccdrelateindex in CCDRelateIndexArray)
                    //{
                    //    Bitmap bmp = bmplist[ccdrelateindex];
                    //    string bmpstring = WORKPATH + "\\" + ccdrelateindex.ToString("000") + Universal.GlobalImageTypeString;

                    //    bmplist.RemoveAt(ccdrelateindex);
                    //    bmp.Dispose();

                    //    bmp = new Bitmap(1, 1);
                    //    GetBMP(bmpstring, ref bmp);

                    //    bmplist.Insert(ccdrelateindex, bmp);
                    //}
                    ret = true;

                    break;
                case CCDTYPEEnum.EPIX:

                    ret = InitialEPIX(Universal.WORKPATH); //, bmplist);

                    break;
#if PTG
                case CCDTYPEEnum.PTG:
                    ret = InitialPTG(Universal.WORKPATH);
                    break;
#endif
#if TIS || TISUSB
                case CCDTYPEEnum.TIS:
                case CCDTYPEEnum.TISUSB:

                    ret = InitialUSBTIS(Universal.WORKPATH);


                    break;
#endif
#if ICAM
                case CCDTYPEEnum.ICAM:
                    ret = InitialICAM(Universal.WORKPATH);
                    break;
#endif
#if IWIN
                case CCDTYPEEnum.IWIN:

                    ret= WINDVisionInitialEx(Universal.WORKPATH);

                    break;
#endif
            }
            return ret;
        }

        public bool Initial(string path)
        {
            bool ret = false;
            SystemWORKPATH = path;
            switch (CCDType)
            {
                case CCDTYPEEnum.FILE:

                    //foreach(int ccdrelateindex in CCDRelateIndexArray)
                    //{
                    //    Bitmap bmp = bmplist[ccdrelateindex];
                    //    string bmpstring = WORKPATH + "\\" + ccdrelateindex.ToString("000") + Universal.GlobalImageTypeString;

                    //    bmplist.RemoveAt(ccdrelateindex);
                    //    bmp.Dispose();

                    //    bmp = new Bitmap(1, 1);
                    //    GetBMP(bmpstring, ref bmp);

                    //    bmplist.Insert(ccdrelateindex, bmp);
                    //}
                    ret = true;

                    break;
                case CCDTYPEEnum.EPIX:

                    ret = InitialEPIX(path); //, bmplist);

                    break;
#if PTG
                case CCDTYPEEnum.PTG:
                    ret = InitialPTG(path);
                    break;
#endif
#if TIS || TISUSB
                case CCDTYPEEnum.TIS:
                case CCDTYPEEnum.TISUSB:

                    ret = InitialUSBTIS(path);


                    break;
#endif
#if ICAM
                case CCDTYPEEnum.ICAM:
                    ret = InitialICAM(path);
                    break;
#endif
#if IWIN
                case CCDTYPEEnum.IWIN:

                    ret = WINDVisionInitialEx(path);

                    break;
#endif
#if MVS
                case CCDTYPEEnum.MVS:

                    ret = MVS_Main();

                    break;
#endif
#if DVP2

                case CCDTYPEEnum.DVP2:

                    //ret = MVS_Main();
                    ret = dvp2Init(WORKPATH) == 0;

                    break;

#endif
            }
            return ret;
        }
        bool InitialEPIX(string fmtpath) //, List<Bitmap> bmplist)
        {
            int i = 0;
            int ret = 1;

            //while (ret != 0 && i < 5)
            {
                pxd_PIXCIclose();

                ret = pxd_PIXCIopen("", null, fmtpath + "\\EPIX.FMT");

                //i++;
            }

            i = 0;

            foreach (int ccdrelateindex in CCDRelateIndexArray)
            {
                //if (FrameRateArray[i] > -1)
                //{
                //EPIXLive(i);
                EPIXUnLive(i);
                //}
                //else
                //{
                LastCapturedCount[i] = pxd_capturedFieldCount(1 << i);
                //}

                //GetImage(bmplist, ccdrelateindex);
                //bmplist.Insert(ccdrelateindex, bmp);
                i++;
            }

            return ret == 0;

            //InitialEPIX(0);
        }
        public void EPIXLive(int Index)
        {
            pxd_goLive(1 << Index, 1);
        }
        public void EPIXUnLive(int Index)
        {
            pxd_goUnLive(1 << Index);
            pxd_goSnap(1 << Index, 1);
            //  pxd_goLive(1 << Index, 1);
        }

#if PTG
        bool InitialPTG(string seqpath)
        {
            int i = 0;
            int j = 0;
            string Str = "";
            string CCDSEQStr = "";

            int ccdcount = CCDRelateIndexArray.Length;

            List<string> DeviceList = new List<string>();
            List<string> SerialList = new List<string>();
            List<string> FormatList = new List<string>();

            PTGCAM = new ManagedGigECamera[ccdcount];
            PTGRowImage = new ManagedImage[ccdcount];
            PTGProcessedImage = new ManagedImage[ccdcount];
            PTGCAMProperty = new CameraProperty[ccdcount];

            PTGBusManage = new ManagedBusManager();

            try
            {

                CCDSEQStr = seqpath + "\\PTGCCDSEQ.INI";

                while (i < ccdcount)
                {
                    PTGCAM[i] = new ManagedGigECamera();
                    PTGCAMProperty[i] = new CameraProperty();

                    PTGRowImage[i] = new ManagedImage();
                    PTGProcessedImage[i] = new ManagedImage();
                    
        #region Check CCD Serial
                    if (i == 0)
                    {
                        if (File.Exists(CCDSEQStr))
                        {
                            ReadData(ref Str, CCDSEQStr);

                            string[] strs;

                            Str = Str.Replace(Environment.NewLine, "@");
                            strs = Str.Split('@');

                            foreach (string str in strs)
                            {
                                SerialList.Add(str);
                            }
                        }
                        else
                        {
                            uint camcount = PTGBusManage.GetNumOfCameras();

                            j = 0;

                            while (j < camcount)
                            {
                                SerialList.Add(PTGBusManage.GetCameraSerialNumberFromIndex((uint)j).ToString());
                                j++;
                            }

                            Str = "";

                            foreach (string str in SerialList)
                            {
                                Str += str + Environment.NewLine;
                            }

                            Str = Str.Remove(Str.Length - 2, 2);

                            SaveData(Str, CCDSEQStr);
                        }

                    }

        #endregion

                    PTGCAM[i].Connect(PTGBusManage.GetCameraFromSerialNumber(uint.Parse(SerialList[i])));

                    j = 0;
                    while (j < (int)PropertyType.Unspecified)
                    {
                        if ((j > 2 && j < 12) || (j > 13))
                        {
                            j++;
                            continue;
                        }


                        PTGCAMProperty[i] = PTGCAM[i].GetProperty((PropertyType)j);

                        PTGCAMProperty[i].absControl = false;
                        PTGCAMProperty[i].autoManualMode = false;
                        

                        switch ((PropertyType)j)
                        {
                            case PropertyType.Brightness:
                                PTGCAMProperty[i].valueA = 399;
                                break;
                            case PropertyType.AutoExposure:
                                PTGCAMProperty[i].onOff = true;
                                PTGCAMProperty[i].valueA = 292;
                                break;
                            case PropertyType.Sharpness:
                                PTGCAMProperty[i].onOff = true;
                                PTGCAMProperty[i].valueA = 1536;
                                break;
                            case PropertyType.Gamma:
                                PTGCAMProperty[i].onOff = true;
                                PTGCAMProperty[i].valueA = 1024;
                                break;
                            case PropertyType.Shutter:
                                PTGCAMProperty[i].valueA = 986;
                                break;
                            case PropertyType.Gain:
                                PTGCAMProperty[i].valueA = 8;
                                break;
                            case PropertyType.FrameRate:
                                PTGCAMProperty[i].autoManualMode = true;
                                break;
                        }

                        PTGCAM[i].SetProperty(PTGCAMProperty[i]);

                        j++;
                    }

                    PTGCAM[i].StartCapture();

                    i++;

                }
            }
            catch (Exception ex)
            { 
        JetEazy.LoggerClass.Instance.WriteException(ex);
                MessageBox.Show("Initializing PTG Cameras Fail " + e.ToString());
            }

            return true;
        }
#endif

#if TIS || TISUSB
        /// <summary>
        /// 初始化USBTIS相机
        /// </summary>
        /// <param name="fmtpath"></param>
        /// <param name="bmplist"></param>
        /// <returns></returns>
        bool InitialUSBTIS(string fmtpath)
        {
            #region TIS TYPE
            try
            {
                //snapSinks = new FrameSnapSink[CCDRelateIndexArray.Count()];
                TISCAM = new ICImagingControl[CCDRelateIndexArray.Count()];
                TISCAMTUNING = new VCDSimpleProperty[CCDRelateIndexArray.Count()];
                TISPolaritySwitch = new VCDSwitchProperty[CCDRelateIndexArray.Count()];
                TISBrightnessSwitch = new VCDSwitchProperty[CCDRelateIndexArray.Count()];
                //USE FOR USB ACTION
                TISTriggerEnable = new VCDSwitchProperty[CCDRelateIndexArray.Count()];

                TISSoftTrigger = new VCDButtonProperty[CCDRelateIndexArray.Count()];
                TISImageBuffer = new TIS.Imaging.ImageBuffer[CCDRelateIndexArray.Count()];

                return InitialTIS(WORKPATH);
            }
            catch (Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
                string str = ex.ToString();

                return false;
            }
            #endregion
        }
        bool InitialTIS(string fmtpath)
        {
            int i = 0;
            int j = 0;
            string Str = "";
            string CCDSEQStr = "";

            List<string> DeviceList = new List<string>();
            List<string> SerialList = new List<string>();
            List<string> FormatList = new List<string>();

            //try
            //{
            CCDSEQStr = fmtpath + "\\CCDSEQ.INI";

            while (i < CCDRelateIndexArray.Count())
            {

                TISCAM[i] = new ICImagingControl();
                TISCAM[i].ImageRingBufferSize = 100;
                TISCAM[i].ImageAvailableExecutionMode = EventExecutionMode.MultiThreaded;

                #region Check CCD Serial
                if (i == 0)
                {
                    if (File.Exists(CCDSEQStr))
                    {
                        JzTools.ReadData(ref Str, CCDSEQStr);

                        string[] strs;

                        Str = Str.Replace(Environment.NewLine, "@");
                        strs = Str.Split('@');

                        foreach (string str in strs)
                        {
                            SerialList.Add(str);
                        }
                    }
                    else
                    {
                        foreach (Device device in TISCAM[i].Devices)
                        {
                            string serial = "";
                            device.GetSerialNumber(out serial);
                            SerialList.Add(serial + ",N");
                        }

                        Str = "";

                        foreach (string str in SerialList)
                        {
                            Str += str + Environment.NewLine;
                        }

                        Str = JzTools.RemoveLastChar(Str, 2);

                        JzTools.SaveData(Str, CCDSEQStr);
                    }

                    foreach (string str in SerialList)
                    {
                        foreach (Device device in TISCAM[i].Devices)
                        {
                            string serial = "";

                            device.GetSerialNumber(out serial);

                            if (str.IndexOf(serial) > -1)
                            {
                                DeviceList.Add(device.Name + "," + str.Split(',')[1]);
                            }
                        }
                    }
                }

                if (DeviceList.Count != CCDRelateIndexArray.Count())
                    return false;

                #endregion

                TISCAM[i].Device = DeviceList[i].Split(',')[0];



                j = 0;

                int index = CCDRelateIndexArray[i];
                Size mySize = new Size(0, 0);
                foreach (string strindex in CamSizeArray)
                {
                    string[] strs = strindex.Split(',');
                    int myindex = int.Parse(strs[0]);
                    if (myindex == index)
                    {
                        mySize = new Size(int.Parse(strs[1]), int.Parse(strs[2]));
                        break;
                    }
                }
                if (mySize.Width != 0)
                {
                    while (j < TISCAM[i].VideoFormats.Length)
                    {
                        Str = TISCAM[i].VideoFormats[j];

                        if (Str.IndexOf("Y800") > -1 && Str.IndexOf(mySize.Width.ToString()) > -1)
                            break;
                        j++;
                    }

                    TISCAM[i].VideoFormat = TISCAM[i].VideoFormats[j].Name;
                }

                RotateFlipFilter = TISCAM[i].FrameFilterCreate("Rotate Flip", "");

                if (DeviceList[i].Split(',')[1][0] == 'R')
                {

                    string[] strRorates = DeviceList[i].Split(',')[1].Split('_');
                    if (strRorates.Length > 1)
                    {
                        int _irorate = 0;
                        bool bOK = int.TryParse(strRorates[1], out _irorate);
                        if (bOK)
                        {
                            RotateFlipFilter.SetIntParameter("Rotation Angle", _irorate);
                            TISCAM[i].DeviceFrameFilters.Add(RotateFlipFilter);
                        }

                    }
                    else
                    {
                        RotateFlipFilter.SetIntParameter("Rotation Angle", 0);
                        TISCAM[i].DeviceFrameFilters.Add(RotateFlipFilter);
                    }


                }

                TISCAM[i].Tag = i;

                TISCAM[i].MemoryCurrentGrabberColorformat = ICImagingControlColorformats.ICY800;
                TISCAM[i].LiveCaptureContinuous = true;
                TISCAM[i].LiveDisplay = false;

                if (TISCAM[i].DeviceValid)
                {

                    switch (CCDType)
                    {
                        case CCDTYPEEnum.TIS:

                            TISPolaritySwitch[i] = (TIS.Imaging.VCDSwitchProperty)TISCAM[i].VCDPropertyItems.FindInterface(TIS.Imaging.VCDIDs.VCDID_TriggerMode
                                + ":" + TIS.Imaging.VCDIDs.VCDElement_TriggerPolarity
                                + ":" + TIS.Imaging.VCDIDs.VCDInterface_Switch);

                            TISPolaritySwitch[i].Switch = true;

                            //TISBrightnessSwitch[i] = TISCAM[i].VCDPropertyItems.Find<VCDSwitchProperty>(VCDGUIDs.VCDID_Brightness, VCDGUIDs.VCDElement_Auto);
                            //TISBrightnessSwitch[i].Switch = false;
                            break;
                        case CCDTYPEEnum.TISUSB:

                            TISTriggerEnable[i] = (TIS.Imaging.VCDSwitchProperty)TISCAM[i].VCDPropertyItems.FindInterface(TIS.Imaging.VCDIDs.VCDID_TriggerMode
                                + ":" + TIS.Imaging.VCDIDs.VCDElement_Value
                                + ":" + TIS.Imaging.VCDIDs.VCDInterface_Switch);

                            TISTriggerEnable[i].Switch = false;

                            // TISTrigger[i] = TISCAM[i].VCDPropertyItems.FindItem(TIS.Imaging.VCDIDs.VCDID_TriggerMode);
                            break;
                    }

                    TISSoftTrigger[i] = (TIS.Imaging.VCDButtonProperty)TISCAM[i].VCDPropertyItems.FindInterface(TIS.Imaging.VCDIDs.VCDID_TriggerMode
                       + ":{FDB4003C-552C-4FAA-B87B-42E888D54147}:"
                       + TIS.Imaging.VCDIDs.VCDInterface_Button);


                    //TIS_AutoExposure(TISCAM[i], false);
                    //TIS_AutoGain(TISCAM[i], false);
                    //TIS_AutoBrightness(TISCAM[i], false);
                    switch (CCDType)
                    {
                        case CCDTYPEEnum.TIS:
                            TIS_SetGainAbs(TISCAM[i], 0);
                            TISCAM[i].DeviceFrameRate = 15;
                            break;
                        case CCDTYPEEnum.TISUSB:
                            ////侦率
                            TISCAM[i].DeviceFrameRate = 140;
                            ////曝光时间
                            //TIS_SetExposureAbs(TISCAM[i], (1d / 1000d));
                            ////Gain值设定
                            //TIS_SetGainAbs(TISCAM[i],3.6d);
                            ////亮度
                            //IC_SetBrightnessAbs(TISCAM[i], 20);

                            break;
                    }

                    //TISCAM[i].LoadShowSaveDeviceState(fmtpath + "\\" + DeviceList[i].Split(',')[0] + ".ini");

                    //TISCAM[i].Sink = new TIS.Imaging.FrameSnapSink();

                    TISCAM[i].ImageAvailable += new EventHandler<ICImagingControl.ImageAvailableEventArgs>(CCDClass_ImageAvailable);
                    TISCAM[i].DeviceLost += new EventHandler<ICImagingControl.DeviceLostEventArgs>(TISCCD_DeviceLost);

                    TISCAM[i].LiveStart();

                    //string strmess=  TISCAM[i].SaveDeviceState();

                    //  TISCAM[i].LoadDeviceState(strmess, true);
                }
                i++;

            }
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show("Initializing TIS Cameras Fail " + e.ToString());
            //    //IsConnectionError = true;
            //}

            return true;
        }

        void CCDClass_ImageAvailable(object sender, ICImagingControl.ImageAvailableEventArgs e)
        {
            ICImagingControl TISCAM = (ICImagingControl)sender;

            TISImageBuffer[(int)TISCAM.Tag] = TISCAM.ImageBuffers[e.bufferIndex];

            //TISImageBuffer[(int)TISCAM.Tag].Lock();

            //CCDBMP[(int)TISCAM.Tag].Dispose();
            //CCDBMP[(int)TISCAM.Tag] = new Bitmap(TISImageBuffer[(int)TISCAM.Tag].Bitmap);

            //TISImageBuffer[(int)TISCAM.Tag].Unlock();
        }

        private void TISCCD_DeviceLost(object sender, ICImagingControl.DeviceLostEventArgs e)
        {
            ICImagingControl TISCAM = (ICImagingControl)sender;

            MessageBox.Show("Initializing TIS Cameras CAM" + ((int)TISCAM.Tag).ToString() + " Fail.");
            //IsConnectionError = true;
        }

        private bool TIS_AutoExposure(TIS.Imaging.ICImagingControl ic, bool onoff)
        {
            TIS.Imaging.VCDSwitchProperty ExposureAuto;
            ExposureAuto = (TIS.Imaging.VCDSwitchProperty)ic.VCDPropertyItems.FindInterface(
                                                          TIS.Imaging.VCDIDs.VCDID_Exposure + ":"
                                                        + TIS.Imaging.VCDIDs.VCDElement_Auto + ":"
                                                        + TIS.Imaging.VCDIDs.VCDInterface_Switch);

            if (ExposureAuto != null && ExposureAuto.Available)
            {
                ExposureAuto.Switch = onoff;
                return true;
            }

            return false;
        }
        private bool TIS_SetExposureAbs(TIS.Imaging.ICImagingControl ic, double value)
        {
            //TIS.Imaging.VCDPropertyItem exposure = ic.VCDPropertyItems.FindItem(TIS.Imaging.VCDGUIDs.VCDID_Exposure);
            //TIS.Imaging.VCDRangeProperty _exposureRange;
            //TIS.Imaging.VCDSwitchProperty _exposureSwitch;
            //if (exposure != null)
            //{
            //    //<<getswitchandrange
            //    // Acquire interfaces to the range and switch interface for value and auto
            //    _exposureRange = exposure.Find<TIS.Imaging.VCDRangeProperty>(TIS.Imaging.VCDGUIDs.VCDElement_Value);
            //    _exposureSwitch = exposure.Find<TIS.Imaging.VCDSwitchProperty>(TIS.Imaging.VCDGUIDs.VCDElement_Auto);
            //    if (_exposureSwitch != null)
            //    {
            //        _exposureSwitch.Switch = false;
            //        //MessageBox.Show("Automation of brightness is not supported by the current device!");
            //    }
            //    if (_exposureRange != null)
            //    {

            //    }
            //}

            TIS.Imaging.VCDAbsoluteValueProperty ExposureAbs;
            ExposureAbs = (TIS.Imaging.VCDAbsoluteValueProperty)ic.VCDPropertyItems.FindInterface(
                                                           TIS.Imaging.VCDIDs.VCDID_Exposure + ":"
                                                         + TIS.Imaging.VCDIDs.VCDElement_Value + ":"
                                                         + TIS.Imaging.VCDIDs.VCDInterface_AbsoluteValue);
            //ExposureAbs = 
            //    (VCDAbsoluteValueProperty)ic.VCDPropertyItems.FindInterface(VCDGUIDs.VCDID_Exposure, VCDGUIDs.VCDElement_Value, VCDGUIDs.VCDInterface_AbsoluteValue);
            if (ExposureAbs != null)
            {
                //double max = ExposureAbs.RangeMax;
                //double min = ExposureAbs.RangeMin;

                ExposureAbs.Value = value;
                return true;
            }
            return false;
        }
        private void TIS_SetExposureAbs(TIS.Imaging.ICImagingControl ic, System.Windows.Forms.NumericUpDown nudCtrl)
        {
            TIS.Imaging.VCDAbsoluteValueProperty ExposureAbs;
            ExposureAbs = (TIS.Imaging.VCDAbsoluteValueProperty)ic.VCDPropertyItems.FindInterface(
                                                           TIS.Imaging.VCDIDs.VCDID_Exposure + ":"
                                                         + TIS.Imaging.VCDIDs.VCDElement_Value + ":"
                                                         + TIS.Imaging.VCDIDs.VCDInterface_AbsoluteValue);

            if (ExposureAbs != null)
            {
                nudCtrl.Maximum = (decimal)ExposureAbs.RangeMax;
                nudCtrl.Minimum = (decimal)ExposureAbs.RangeMin;
                nudCtrl.Value = (decimal)ExposureAbs.Value;
            }
        }
        private bool TIS_SetExposure(TIS.Imaging.ICImagingControl ic, double value)
        {
            TIS.Imaging.VCDAbsoluteValueProperty ExposureAbs;
            ExposureAbs = (TIS.Imaging.VCDAbsoluteValueProperty)ic.VCDPropertyItems.FindInterface(
                                                           TIS.Imaging.VCDIDs.VCDID_Exposure + ":"
                                                         + TIS.Imaging.VCDIDs.VCDElement_Value + ":"
                                                         + TIS.Imaging.VCDIDs.VCDInterface_Switch);

            if (ExposureAbs != null)
            {
                ExposureAbs.Value = value;
                return true;
            }
            return false;
        }

        private bool TIS_AutoGain(TIS.Imaging.ICImagingControl ic, bool onoff)
        {
            TIS.Imaging.VCDSwitchProperty GainAuto;
            GainAuto = (TIS.Imaging.VCDSwitchProperty)ic.VCDPropertyItems.FindInterface(
                                                          TIS.Imaging.VCDIDs.VCDID_Gain + ":"
                                                        + TIS.Imaging.VCDIDs.VCDElement_Auto + ":"
                                                        + TIS.Imaging.VCDIDs.VCDInterface_Switch);

            if (GainAuto != null && GainAuto.Available)
            {
                GainAuto.Switch = onoff;
                return true;
            }
            return false;
        }
        private bool TIS_AutoBrightness(TIS.Imaging.ICImagingControl ic, bool onoff)
        {
            TIS.Imaging.VCDSwitchProperty BrightnessAuto;
            BrightnessAuto = (TIS.Imaging.VCDSwitchProperty)ic.VCDPropertyItems.FindInterface(
                                                          TIS.Imaging.VCDIDs.VCDID_Brightness + ":"
                                                        + TIS.Imaging.VCDIDs.VCDElement_Auto + ":"
                                                        + TIS.Imaging.VCDIDs.VCDInterface_Switch);

            if (BrightnessAuto != null && BrightnessAuto.Available)
            {
                BrightnessAuto.Switch = onoff;
                return true;
            }
            return false;
        }


        private bool TIS_SetGainAbs(TIS.Imaging.ICImagingControl ic, double value)
        {
            TIS.Imaging.VCDAbsoluteValueProperty GainAbs;
            GainAbs = (TIS.Imaging.VCDAbsoluteValueProperty)ic.VCDPropertyItems.FindInterface(
                                                           TIS.Imaging.VCDIDs.VCDID_Gain + ":"
                                                         + TIS.Imaging.VCDIDs.VCDElement_Value + ":"
                                                         + TIS.Imaging.VCDIDs.VCDInterface_AbsoluteValue);

            if (GainAbs != null)
            {
                GainAbs.Value = value;
                return true;
            }
            return false;
        }
        private void TIS_SetGainAbs(TIS.Imaging.ICImagingControl ic, System.Windows.Forms.NumericUpDown nudCtrl)
        {
            TIS.Imaging.VCDAbsoluteValueProperty GainAbs;
            GainAbs = (TIS.Imaging.VCDAbsoluteValueProperty)ic.VCDPropertyItems.FindInterface(
                                                           TIS.Imaging.VCDIDs.VCDID_Gain + ":"
                                                         + TIS.Imaging.VCDIDs.VCDElement_Value + ":"
                                                         + TIS.Imaging.VCDIDs.VCDInterface_AbsoluteValue);

            if (GainAbs != null)
            {
                nudCtrl.Maximum = (decimal)GainAbs.RangeMax;
                nudCtrl.Minimum = (decimal)GainAbs.RangeMin;
                nudCtrl.Value = (decimal)GainAbs.Value;
            }
        }

        private bool IC_SetGain(TIS.Imaging.ICImagingControl ic, int value)
        {
            TIS.Imaging.VCDRangeProperty GainRange;
            string strData = TIS.Imaging.VCDIDs.VCDID_Gain + ":"
                                                         + TIS.Imaging.VCDIDs.VCDElement_Value + ":"
                                                         + TIS.Imaging.VCDIDs.VCDInterface_Range;
            GainRange = (TIS.Imaging.VCDRangeProperty)ic.VCDPropertyItems.FindInterface(strData);

            if (GainRange != null)
            {
                GainRange.Value = value;
                return true;
            }
            return false;
        }
        private bool IC_SetBrightnessAbs(TIS.Imaging.ICImagingControl ic, double value)
        {
            TIS.Imaging.VCDAbsoluteValueProperty BrightnessRange;
            BrightnessRange = (TIS.Imaging.VCDAbsoluteValueProperty)ic.VCDPropertyItems.FindInterface(
                                                           TIS.Imaging.VCDIDs.VCDID_Brightness + ":"
                                                         + TIS.Imaging.VCDIDs.VCDElement_Value + ":"
                                                         + TIS.Imaging.VCDIDs.VCDInterface_AbsoluteValue);

            if (BrightnessRange != null)
            {
                BrightnessRange.Value = value;
                return true;
            }
            return false;
        }
        private void IC_SetBrightnessAbs(TIS.Imaging.ICImagingControl ic, System.Windows.Forms.NumericUpDown nudCtrl)
        {
            TIS.Imaging.VCDAbsoluteValueProperty BrightnessRange;
            BrightnessRange = (TIS.Imaging.VCDAbsoluteValueProperty)ic.VCDPropertyItems.FindInterface(
                                                           TIS.Imaging.VCDIDs.VCDID_Brightness + ":"
                                                         + TIS.Imaging.VCDIDs.VCDElement_Value + ":"
                                                         + TIS.Imaging.VCDIDs.VCDInterface_AbsoluteValue);

            if (BrightnessRange != null)
            {

                nudCtrl.Maximum = (decimal)BrightnessRange.RangeMax;
                nudCtrl.Minimum = (decimal)BrightnessRange.RangeMin;
                nudCtrl.Value = (decimal)BrightnessRange.Value;
            }
        }

        /// <summary>
        /// 设定亮度
        /// </summary>
        /// <param name="ic"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool IC_SetBrightness(TIS.Imaging.ICImagingControl ic, int value)
        {
            TIS.Imaging.VCDRangeProperty BrightnessRange;
            BrightnessRange = (TIS.Imaging.VCDRangeProperty)ic.VCDPropertyItems.FindInterface(
                                                           TIS.Imaging.VCDIDs.VCDID_Brightness + ":"
                                                         + TIS.Imaging.VCDIDs.VCDElement_Value + ":"
                                                         + TIS.Imaging.VCDIDs.VCDInterface_Range);

            if (BrightnessRange != null)
            {
                BrightnessRange.Value = value;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 关闭及清理TIS CCD
        /// </summary>
        private void TISClose()
        {
            foreach (ICImagingControl ccd in TISCAM)
            {
                ccd.LiveStop();
                ccd.Dispose();
            }


            RotateFlipFilter.Dispose();

            foreach (ICImagingControl ic in TISCAM)
            {
                if (ic != null)
                    ic.Dispose();
            }
            foreach (TIS.Imaging.ImageBuffer image in TISImageBuffer)
            {
                if (image != null)
                    image.Dispose();
            }

            //foreach (VCDSwitchProperty vcd in TISPolaritySwitch)
            //    vcd.Dispose();

            //USE FOR USB TRIGGER
            foreach (VCDSwitchProperty vcd in TISTriggerEnable)
            {
                if (vcd != null)
                    vcd.Dispose();
            }


            foreach (VCDButtonProperty vcd in TISSoftTrigger)
            {
                if (vcd != null)
                    vcd.Dispose();
            }
        }
#endif
#if IWIN
        #region
        /// <summary>
        /// 设定相机参数
        /// </summary>
        /// <param name="index">相机编号</param>
        public void WINDVisionSetting(int index)
        {
            if (m_hCamera[index] > 0)
            {
                MvApi.CameraShowSettingPage(m_hCamera[index], 1);//1 show ; 0 hide
            }
        }
        /// <summary>
        /// 設定相機ROI
        /// </summary>
        /// <param name="index">相機編號</param>
        public void WINDVisionCustomizeResolution(int index)
        {
            if (MvApi.CameraCustomizeResolution(m_hCamera[index], ref tResolution[index]) == CameraSdkStatus.CAMERA_STATUS_SUCCESS)
            {
                MvApi.CameraSetImageResolution(m_hCamera[index], ref tResolution[index]);
            }
        }
        /// <summary>
        /// 重新繪製pictureBox
        /// </summary>
        /// <param name="index">相機編號</param>
        /// <param name="pic">pictureBox</param>
        public void WINDVisionRehandle(int index, PictureBox pic)
        {
            switch (CCDType)
            {
                case CCDTYPEEnum.FILE:
                    break;
                case CCDTYPEEnum.IWIN:
                    if (m_hCamera[index] > 0)
                    {
                        //初始化显示模块，使用SDK内部封装好的显示接口
                        MvApi.CameraDisplayInit(m_hCamera[index], pic.Handle);
                        MvApi.CameraSetDisplaySize(m_hCamera[index], pic.Width, pic.Height);
                    }
                    break;
            }
        }
        /// <summary>
        /// 工業相機初始化
        /// </summary>
        /// <param name="configpath">配置文檔路徑</param>
        /// <returns></returns>
        bool WINDVisionInitial(string configpath)
        {
            bool bOK = false;
            for (int i = 0; i < CCDRelateIndexArray.Length; i++)
                IWindControlList.Add(new Label());

            CameraSdkStatus status;
            tSdkCameraDevInfo[] tCameraDevInfoList;
            CAMERA_SNAP_PROC pCaptureCallOld = null;
            status = MvApi.CameraSdkInit(1);
            if (status == CameraSdkStatus.CAMERA_STATUS_SUCCESS)
            {

            }
            else
            {
                MessageBox.Show("IWIN SDK 初始化失敗。");
                return bOK;
            }

            status = MvApi.CameraEnumerateDevice(out tCameraDevInfoList);
            if (status == CameraSdkStatus.CAMERA_STATUS_SUCCESS)
            {

            }
            else
            {
                MessageBox.Show("没有找到相机，如果已经接上相机，可能是权限不够，请尝试使用管理员权限运行程序。");
                return bOK;
            }

            if (CCDRelateIndexArray.Length > tCameraDevInfoList.Length)
            {
                MessageBox.Show("没有那么多IWIN相机！");
                return bOK;
            }
            m_hCamera = new CameraHandle[CCDRelateIndexArray.Length];
            //m_ImageBufferSnapshot = new IntPtr[CCDRelateIndexArray.Length];     // 抓拍通道RGB图像缓存
            tCameraCapability=new tSdkCameraCapbility[CCDRelateIndexArray.Length];  // 相机特性描述
            m_ImageBuffer=new IntPtr[CCDRelateIndexArray.Length];             // 预览通道RGB图像缓存
            //m_iDisplayedFrames=new int[CCDRelateIndexArray];    //已经显示的总帧数
            //m_CaptureCallback=new CAMERA_SNAP_PROC[CCDRelateIndexArray];
            //m_iCaptureCallbackCtx=new IntPtr[CCDRelateIndexArray];     //图像回调函数的上下文参数
            //m_tFrameHead=new tSdkFrameHead[CCDRelateIndexArray];
            tResolution=new tSdkImageResolution[CCDRelateIndexArray.Length];

            string CCDSEQStr = configpath + "\\WINDVISION.INI";
            string strCCDData = "";
            if (!File.Exists(CCDSEQStr))
            {
                for (int i = 0; i < tCameraDevInfoList.Length; i++)
                {
                    string ID = System.Text.Encoding.UTF8.GetString(tCameraDevInfoList[i].acSn);
                    int iindex = ID.IndexOf('\0');
                    ID = ID.Substring(0, iindex);

                    int offsetX = 0; //(2048 - 640) / 2;
                    int offsetY = 0;// (1536 - 480) / 2;

                    WIND_Width = 2048;
                    WIND_Height = 1536;

                    strCCDData += i + ":" + ID + ":" + offsetX.ToString() + ":" + offsetY.ToString() + ":2048:1536" + Environment.NewLine;
                }
                JzTools.SaveData(strCCDData, CCDSEQStr);
            }
            else
                JzTools.ReadData(ref strCCDData, CCDSEQStr);

            List<string> listCameraID = new List<string>();

            strCCDData = strCCDData.Replace(Environment.NewLine, "$");
            string[] IDdata = strCCDData.Split('$');
            int iCount = 0;
            foreach (string id in IDdata)
            {
                if (id == "")
                    continue;

                string[] myid = id.Split(':');
                if (myid.Length > 1)
                {
                    string ID = myid[1];

                    //foreach (int ccdrelateindex in CCDRelateIndexArray)
                    for (int ccdrelateindex = 0; ccdrelateindex < CCDRelateIndexArray.Length; ccdrelateindex++)
                    {
                        int index = ccdrelateindex;// Array.IndexOf(CCDRelateIndexArray, ccdrelateindex);
                        if (index.ToString() == myid[0])
                        {
                            foreach (tSdkCameraDevInfo ds in tCameraDevInfoList)
                            {
                                string IDNow = System.Text.Encoding.UTF8.GetString(ds.acSn);
                                int iindextmp = IDNow.IndexOf('\0');
                                IDNow = IDNow.Substring(0, iindextmp);
                                if (IDNow == ID)
                                {

                                    status = MvApi.CameraInit(ref tCameraDevInfoList[iCount], -1, -1, ref m_hCamera[iCount]);
                                    if (status == CameraSdkStatus.CAMERA_STATUS_SUCCESS)
                                    {
                                        //获得相机特性描述
                                        MvApi.CameraGetCapability(m_hCamera[index], out tCameraCapability[index]);

                                        m_ImageBuffer[index] = Marshal.AllocHGlobal(tCameraCapability[index].sResolutionRange.iWidthMax * tCameraCapability[index].sResolutionRange.iHeightMax * 3 + 1024);
                                        //m_ImageBufferSnapshot[index] = Marshal.AllocHGlobal(tCameraCapability[index].sResolutionRange.iWidthMax * tCameraCapability[index].sResolutionRange.iHeightMax * 3 + 1024);

                                        //初始化显示模块，使用SDK内部封装好的显示接口
                                        //MvApi.CameraDisplayInit(m_hCamera[index], IWindControlList[index].Handle);
                                        //MvApi.CameraSetDisplaySize(m_hCamera[index], IWindControlList[index].Width, IWindControlList[index].Height);

                                        //设置抓拍通道的分辨率。
                                        //tResolution.iIndex = 0xff;
                                        //tResolution.iHeight = tResolution.iHeightFOV = tCameraCapability.sResolutionRange.iHeightMax;
                                        //tResolution.iWidth = tResolution.iWidthFOV = tCameraCapability.sResolutionRange.iWidthMax;

                                        //设置抓拍通道的分辨率。
                                        //tSdkImageResolution tResolution;
                                        tResolution[index].iIndex = 0xff;
                                        tResolution[index].uSkipMode = 0;
                                        tResolution[index].uBinAverageMode = 0;
                                        tResolution[index].uBinSumMode = 0;
                                        tResolution[index].uResampleMask = 0;
                                        tResolution[index].iVOffsetFOV = int.Parse(myid[3]);
                                        tResolution[index].iHOffsetFOV = int.Parse(myid[2]);
                                        //tResolution.iWidthFOV =  tCameraCapability.sResolutionRange.iWidthMax;
                                        //tResolution.iHeightFOV = tCameraCapability.sResolutionRange.iHeightMax;

                                        tResolution[index].iWidthFOV = int.Parse(myid[4]); ;// tCameraCapability.sResolutionRange.iWidthMax;
                                        tResolution[index].iHeightFOV = int.Parse(myid[5]); ;// tCameraCapability.sResolutionRange.iHeightMax;
                                        tResolution[index].iWidth = tResolution[index].iWidthFOV;
                                        tResolution[index].iHeight = tResolution[index].iHeightFOV;
                                        //tResolution.iIndex = 0xff;表示自定义分辨率,如果tResolution.iWidth和tResolution.iHeight
                                        //定义为0，则表示跟随预览通道的分辨率进行抓拍。抓拍通道的分辨率可以动态更改。
                                        //本例中将抓拍分辨率固定为最大分辨率。
                                        tResolution[index].acDescription = new byte[32];//描述信息可以不设置
                                        tResolution[index].iWidthZoomHd = 0;
                                        tResolution[index].iHeightZoomHd = 0;
                                        tResolution[index].iWidthZoomSw = 0;
                                        tResolution[index].iHeightZoomSw = 0;

                                        WIND_Width = int.Parse(myid[4]);
                                        WIND_Height = int.Parse(myid[5]);

                                        MvApi.CameraSetResolutionForSnap(m_hCamera[index], ref tResolution[index]);

                                        //让SDK来根据相机的型号动态创建该相机的配置窗口。
                                        MvApi.CameraCreateSettingPage(m_hCamera[index], IWindControlList[index].Handle, tCameraDevInfoList[0].acFriendlyName,/*SettingPageMsgCalBack*/null,/*m_iSettingPageMsgCallbackCtx*/(IntPtr)null, 0);

                                        //m_CaptureCallback = new CAMERA_SNAP_PROC(ImageCaptureCallback);
                                        //MvApi.CameraSetCallbackFunction(m_hCamera[index], m_CaptureCallback, m_iCaptureCallbackCtx, ref pCaptureCallOld);

                                        WINDVisionLive(index);
                                        iCount++;
                                        continue;
                                    }
                                    else
                                    {
                                        m_hCamera[index] = 0;
                                        String errstr = string.Format("相机初始化错误，错误码{0},错误原因是", status);
                                        String errstring = MvApi.CameraGetErrorString(status);
                                        MessageBox.Show(errstr + errstring, "ERROR");
                                        iCount++;
                                        bOK = false;
                                    }
                                }
                            }
                        }
                    }

                }
                else
                    continue;
            }
            if (iCount == CCDRelateIndexArray.Length)
                bOK = true;
            else
                bOK = false;

            return bOK;
        }
        /// <summary>
        /// 工業相機初始化加载文件
        /// </summary>
        /// <param name="configpath">配置文檔路徑</param>
        /// <returns></returns>
        bool WINDVisionInitialEx(string configpath)
        {
            CameraSdkStatus status;
            tSdkCameraDevInfo[] tCameraDevInfoList;

            status = MvApi.CameraSdkInit(1);//1:Chinese 0:English
            if (status != CameraSdkStatus.CAMERA_STATUS_SUCCESS)
            {
                MessageBox.Show("WIND SDK 初始化失敗。");
                return false;
            }

            status = MvApi.CameraEnumerateDevice(out tCameraDevInfoList);
            if (status != CameraSdkStatus.CAMERA_STATUS_SUCCESS)
            {
                MessageBox.Show("没有找到相机，如果已经接上相机，可能是权限不够，请尝试使用管理员权限运行程序。");
                return false;
            }

            if (CCDRelateIndexArray.Length > tCameraDevInfoList.Length)
            {
                MessageBox.Show("没有那么多WIND_CAM相机！");
                return false;
            }

            m_hCamera = new CameraHandle[CCDRelateIndexArray.Length];
            tCameraCapability = new tSdkCameraCapbility[CCDRelateIndexArray.Length];  // 相机特性描述
            m_ImageBuffer = new IntPtr[CCDRelateIndexArray.Length];             // 预览通道RGB图像缓存
            tResolution = new tSdkImageResolution[CCDRelateIndexArray.Length];


            //读机机编号 以便获得相机排序
            string CCDSEQStr = configpath + "\\WINDVISION.INI";
            string strCCDData = "";
            if (!File.Exists(CCDSEQStr))
            {
                for (int i = 0; i < tCameraDevInfoList.Length; i++)
                {
                    string ID = System.Text.Encoding.UTF8.GetString(tCameraDevInfoList[i].acSn);
                    int iindex = ID.IndexOf('\0');
                    ID = ID.Substring(0, iindex);

                    int offsetX = 0; //(2048 - 640) / 2;
                    int offsetY = 0;// (1536 - 480) / 2;

                    WIND_Width = 2048;
                    WIND_Height = 1536;

                    strCCDData += i + ":" + ID + ":" + offsetX.ToString() + ":" + offsetY.ToString() + ":2048:1536" + Environment.NewLine;
                }
                JzTools.SaveData(strCCDData, CCDSEQStr);
            }
            else
                JzTools.ReadData(ref strCCDData, CCDSEQStr);

            for (int i = 0; i < CCDRelateIndexArray.Length; i++)
                IWindControlList.Add(new Label());

            strCCDData = strCCDData.Replace(Environment.NewLine, "$");
            string[] IDdata = strCCDData.Split('$');
            int index = 0;
            foreach (string id in IDdata)
            {
                if (id == "")
                    continue;

                string[] myid = id.Split(':');
                if (myid.Length > 1)
                {
                    int iindexCCD = int.Parse(myid[0]);
                    string ID = myid[1];

                    int iindexCCDIwin = -1;
                    foreach (tSdkCameraDevInfo ds in tCameraDevInfoList)
                    {
                        iindexCCDIwin++;
                        if (iindexCCD >= m_hCamera.Length)
                            break;

                        string _Wind_SN = System.Text.Encoding.UTF8.GetString(ds.acSn);
                        int iindextmp1 = _Wind_SN.IndexOf('\0');
                        _Wind_SN = _Wind_SN.Substring(0, iindextmp1);
                        if (_Wind_SN != ID)
                            continue;

                        //    string _Wind_SN = System.Text.Encoding.UTF8.GetString(ds.acSn);
                        //int iindextmp = _Wind_SN.IndexOf('\0');
                        //_Wind_SN = _Wind_SN.Substring(0, iindextmp);
                        status = MvApi.CameraInit(ref tCameraDevInfoList[iindexCCDIwin], -1, -1, ref m_hCamera[iindexCCD]);
                        string _wind_config_pathname = configpath + "\\" + _Wind_SN + ".config";
                        if (status == CameraSdkStatus.CAMERA_STATUS_SUCCESS)
                        {
                          //  IWindControlList.Add(new Label());
                            if (!File.Exists(_wind_config_pathname))
                            {
                                MvApi.CameraLoadParameter(m_hCamera[iindexCCD], (int)emSdkParameterTeam.PARAMETER_TEAM_A);
                                MvApi.CameraSaveParameterToFile(m_hCamera[iindexCCD], _wind_config_pathname);
                            }
                            else
                                MvApi.CameraReadParameterFromFile(m_hCamera[iindexCCD], _wind_config_pathname);

                            //曝光时间单位是毫秒 TEST Gaara
                            double pMin = 0, pMax = 100;
                            double _test_exposure = 0d;
                            MvApi.CameraGetExposureTimeRange(m_hCamera[iindexCCD], ref pMin, ref pMax, ref _test_exposure);
                            pMax = ExposureBaseArray[iindexCCD];
                            double dSetExposure = pMin + (pMax - pMin) / 2;

                            if (dSetExposure > pMax)
                                dSetExposure = pMax;
                            //  MvApi.CameraGetExposureTime(m_hCamera[index], ref _test_exposure);
                            //  _test_exposure += 10;
                            WINDVisionSetExposure(iindexCCD, dSetExposure);
                            //  MvApi.CameraGetExposureTime(m_hCamera[index], ref _test_exposure);

                            //获得相机特性描述
                            MvApi.CameraGetCapability(m_hCamera[iindexCCD], out tCameraCapability[iindexCCD]);
                            m_ImageBuffer[iindexCCD] = Marshal.AllocHGlobal(tCameraCapability[iindexCCD].sResolutionRange.iWidthMax * tCameraCapability[iindexCCD].sResolutionRange.iHeightMax * 3 + 1024);
                            //让SDK来根据相机的序列号动态创建该相机的配置窗口。
                            MvApi.CameraCreateSettingPage(m_hCamera[iindexCCD], IWindControlList[iindexCCD].Handle, tCameraDevInfoList[iindexCCDIwin].acSn,/*SettingPageMsgCalBack*/null,/*m_iSettingPageMsgCallbackCtx*/(IntPtr)null, 0);

                            WINDVisionLive(iindexCCD);
                        }
                        else
                        {
                            m_hCamera[iindexCCD] = 0;
                            String errstr = string.Format("相机初始化错误，错误码{0},错误原因是", status);
                            String errstring = MvApi.CameraGetErrorString(status);
                            if (MessageBox.Show(errstr + errstring, "ERROR", MessageBoxButtons.OK) == DialogResult.OK)
                                return false;
                            else
                                return false;
                        }
                        index++;
                        break;
                    }
                }
            }

            if (index != CCDRelateIndexArray.Length)
                return false;

            return true;
        }
        /// <summary>
        /// 相机及时模式
        /// </summary>
        /// <param name="Index">相機編號</param>
        public void WINDVisionLive(int Index)
        {
            MvApi.CameraPlay(m_hCamera[Index]);
            m_wind_live = true;
        }
        /// <summary>
        /// 相机非及时模式
        /// </summary>
        /// <param name="Index">相機編號</param>
        public void WINDVisionUnLive(int Index)
        {
            MvApi.CameraStop(m_hCamera[Index]);
            m_wind_live = false;
        }
        /// <summary>
        /// 获取图像 
        /// </summary>
        /// <param name="index">相機編號</param>
        /// <param name="roattion">角度</param>
        /// <param name="bmp">图像</param>
        public void WINDVisionRender(int index, int roattion, Bitmap bmp)
        {
            switch (roattion)
            {
                case 90:
                case 270:
                    bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
            }

            //if (FrameRateArray[index] == -1)
            //MvApi.CameraPlay(m_hCamera[index]);

            Graphics g = Graphics.FromImage(bmp);
            //IntPtr hDC = g.GetHdc();

        #region 獲取圖像

            tSdkFrameHead tFrameHead;
            IntPtr uRawBuffer;//由SDK中给RAW数据分配内存，并释放
            CameraSdkStatus eStatus;// = MvApi.CameraSnapToBuffer(m_hCamera[index], out tFrameHead, out uRawBuffer, 1000);
            eStatus = MvApi.CameraGetImageBuffer(m_hCamera[index], out tFrameHead, out uRawBuffer, 500);
            if (eStatus == CameraSdkStatus.CAMERA_STATUS_SUCCESS)
            {
                //此时，uRawBuffer指向了相机原始数据的缓冲区地址，默认情况下为8bit位宽的Bayer格式，如果
                //您需要解析bayer数据，此时就可以直接处理了，后续的操作演示了如何将原始数据转换为RGB格式
                //并显示在窗口上。
                //将相机输出的原始数据转换为RGB格式到内存m_ImageBufferSnapshot中
                MvApi.CameraImageProcess(m_hCamera[index], uRawBuffer, m_ImageBuffer[index], ref tFrameHead);
                //CameraSnapToBuffer成功调用后必须用CameraReleaseImageBuffer释放SDK中分配的RAW数据缓冲区
                //否则，将造成死锁现象，预览通道和抓拍通道会被一直阻塞，直到调用CameraReleaseImageBuffer释放后解锁。
                MvApi.CameraReleaseImageBuffer(m_hCamera[index], uRawBuffer);
                //更新抓拍显示窗口。
                //MvApi.CameraImage_DrawToDCFit(m_ImageBufferSnapshot, hDC, 1, 0, 0, bmp.Width, bmp.Height);
                Bitmap m_bmp_Mvapi;
                m_bmp_Mvapi = (Bitmap)MvApi.CSharpImageFromFrame(m_ImageBuffer[index], ref tFrameHead);
                //g.DrawImage(imageX, new Rectangle(0, 0, imageX.Width, imageX.Height), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                //Bitmap bmpxx = new Bitmap(m_bmp_Mvapi);
                //g.DrawImage(m_bmp_Mvapi, new PointF(0, 0));

                g.DrawImage(m_bmp_Mvapi, new Rectangle(0, 0, bmp.Width, bmp.Height), new Rectangle(0, 0, m_bmp_Mvapi.Width, m_bmp_Mvapi.Height), GraphicsUnit.Pixel);
                m_bmp_Mvapi.Dispose();
                
            }
            else
            {
                bmp = new Bitmap(bmp.Width, bmp.Height);
            }

        #endregion

            //g.ReleaseHdc(hDC);
            g.Dispose();

            switch (roattion)
            {
                case 90:
                    bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 270:
                    bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
                case 180:
                    bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
            }
        }
        /// <summary>
        /// 曝光时间 原始值
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="_exposure">原始值</param>
        public void WINDVisionSetExposure(int Index, double _exposure)
        {
            if (m_hCamera[Index] > 0)
            {
                MvApi.CameraSetExposureTime(m_hCamera[Index], _exposure);
            }
        }
        /// <summary>
        /// 曝光时间
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="_exposure">100分比</param>
        public void WINDVisionSetExposureRatio(int index, double _exposure)
        {
            if (m_hCamera[index] > 0)
            {
                double pMin = 0, pMax = 100;
                double _test_exposure = 0d;
                MvApi.CameraGetExposureTimeRange(m_hCamera[index], ref pMin, ref pMax, ref _test_exposure);
                pMax = ExposureBaseArray[index];

                double tempValue = pMax - pMin;
                tempValue = pMin + (tempValue * _exposure / 100);
                MvApi.CameraSetExposureTime(m_hCamera[index], tempValue);
            }
        }
        /// <summary>
        /// 关闭相机
        /// </summary>
        /// <param name="index">相机编号</param>
        public void WINDVisionClose(int index)
        {
            if (m_hCamera[index] == 0 || m_ImageBuffer[index] == IntPtr.Zero) // || m_ImageBufferSnapshot[index] == null)
                return;

            if (m_hCamera[index] != 0)
            {
                MvApi.CameraUnInit(m_hCamera[index]);
                m_hCamera[index] = 0;
            }

            if (m_ImageBuffer[index] != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(m_ImageBuffer[index]);
                m_ImageBuffer[index] = IntPtr.Zero;
            }

            //if (m_ImageBufferSnapshot[index] != null)
            //{
            //    if (m_ImageBufferSnapshot[index] != IntPtr.Zero)
            //    {
            //        Marshal.FreeHGlobal(m_ImageBufferSnapshot[index]);
            //        m_ImageBufferSnapshot[index] = IntPtr.Zero;
            //    }
            //}

        }
        /// <summary>
        /// 更新图像
        /// </summary>
        /// <param name="tFrameHead"></param>
        /// <param name="pRgbBuffer"></param>
        public void UpdateImage(ref tSdkFrameHead tFrameHead, IntPtr pRgbBuffer)
        {
            SnapshotBox.Width = tFrameHead.iWidth;
            SnapshotBox.Height = tFrameHead.iHeight;
            SnapshotBox.Image = MvApi.CSharpImageFromFrame(pRgbBuffer, ref tFrameHead);

            //Bitmap BMP = (Bitmap)MvApi.CSharpImageFromFrame(pRgbBuffer, ref tFrameHead);
            //Bitmap BMPX = new Bitmap(BMP);
            //BMPX.Save("D:\\LOA\\01.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            //BMP.Dispose();

            //panel1.AutoScroll = true;
        }
        /// <summary>
        /// 回调函数
        /// </summary>
        /// <param name="hCamera"></param>
        /// <param name="pFrameBuffer"></param>
        /// <param name="pFrameHead"></param>
        /// <param name="pContext"></param>
        public void ImageCaptureCallback(CameraHandle hCamera, IntPtr pFrameBuffer, ref tSdkFrameHead pFrameHead, IntPtr pContext)
        {
            ////图像处理，将原始输出转换为RGB格式的位图数据，同时叠加白平衡、饱和度、LUT等ISP处理。
            //MvApi.CameraImageProcess(hCamera, pFrameBuffer, m_ImageBuffer, ref pFrameHead);
            ////叠加十字线、自动曝光窗口、白平衡窗口信息(仅叠加设置为可见状态的)。   
            //MvApi.CameraImageOverlay(hCamera, m_ImageBuffer, ref pFrameHead);
            ////调用SDK封装好的接口，显示预览图像
            //MvApi.CameraDisplayRGB24(hCamera, m_ImageBuffer, ref pFrameHead);
            //m_iDisplayedFrames++;

            //if (pFrameHead.iWidth != m_tFrameHead.iWidth || pFrameHead.iHeight != m_tFrameHead.iHeight)
            //{
            //    m_bEraseBk = true;
            //    m_tFrameHead = pFrameHead;
            //}

        }
        #endregion
#endif
#if MVS
        #region 海康相机
        public bool MVS_Main()
        {
            m_pDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();

            m_nCanOpenDeviceNum = 0;
            m_nDevNum = 0;
            m_nDevNum = MVS_GetDeviceListAcq();
            m_pMyCamera = new CAMERA[m_nDevNum];
            //   isGetBmp = new bool[m_nDevNum];
            for (int i = 0; i < m_nDevNum; i++)
            {
                m_pMyCamera[i].m_nBufSizeForSaveImage = 3072 * 2048 * 3 * 3 + 2048;
                m_pMyCamera[i].m_pBufForSaveImage = new byte[3072 * 2048 * 3 * 3 + 2048];

                //   isGetBmp[i] = false;
            }
            m_nFrames = new int[m_nDevNum];
            cbImage = new MyCamera.cbOutputdelegate(MVS_ImageCallBack1);

            m_hDisplayHandle = new IntPtr[m_nDevNum];

            bool isok = MVS_Open();

            return isok;
        }

        /// <summary>
        /// 获取丢帧数
        /// </summary>
        /// <param name="nIndex">相机编号</param>
        /// <returns></returns>
        public string MVS_GetLostFrame(int nIndex)
        {
            MyCamera.MV_CC_DEVICE_INFO stDevInfo = new MyCamera.MV_CC_DEVICE_INFO();

            int nRet = m_pMyCamera[nIndex].Cam_Info.MV_CC_GetDeviceInfo_NET(ref stDevInfo);

            if (stDevInfo.nTLayerType == MyCamera.MV_GIGE_DEVICE)
            {
                MyCamera.MV_ALL_MATCH_INFO pstInfo = new MyCamera.MV_ALL_MATCH_INFO();
                _MV_MATCH_INFO_NET_DETECT_ MV_NetInfo = new _MV_MATCH_INFO_NET_DETECT_();
                pstInfo.nInfoSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(_MV_MATCH_INFO_NET_DETECT_));
                pstInfo.nType = 0x00000001;
                int size = Marshal.SizeOf(MV_NetInfo);
                pstInfo.pInfo = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(MV_NetInfo, pstInfo.pInfo, false);

                m_pMyCamera[nIndex].Cam_Info.MV_CC_GetAllMatchInfo_NET(ref pstInfo);
                MV_NetInfo = (_MV_MATCH_INFO_NET_DETECT_)Marshal.PtrToStructure(pstInfo.pInfo, typeof(_MV_MATCH_INFO_NET_DETECT_));

                string sTemp = MV_NetInfo.nLostFrameCount.ToString();
                Marshal.FreeHGlobal(pstInfo.pInfo);
                return sTemp;
            }
            else// ch:如果不是Gige设备，默认为U3V设备 | en:If not Gige device, default U3V device
            {
                MyCamera.MV_ALL_MATCH_INFO pstInfo = new MyCamera.MV_ALL_MATCH_INFO();
                MyCamera.MV_MATCH_INFO_USB_DETECT MV_NetInfo = new MyCamera.MV_MATCH_INFO_USB_DETECT();
                pstInfo.nInfoSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(MyCamera.MV_MATCH_INFO_USB_DETECT));
                pstInfo.nType = 0x00000004;
                int size = Marshal.SizeOf(MV_NetInfo);
                pstInfo.pInfo = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(MV_NetInfo, pstInfo.pInfo, false);

                m_pMyCamera[nIndex].Cam_Info.MV_CC_GetAllMatchInfo_NET(ref pstInfo);
                MV_NetInfo = (MyCamera.MV_MATCH_INFO_USB_DETECT)Marshal.PtrToStructure(pstInfo.pInfo, typeof(MyCamera.MV_MATCH_INFO_USB_DETECT));

                string sTemp = MV_NetInfo.nErrorFrameCount.ToString();
                Marshal.FreeHGlobal(pstInfo.pInfo);
                return sTemp;
            }
            return "0";
        }

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="stFrameInfo"></param>
        /// <param name="nIndex"></param>
        private void MVS_GetImageNOW(IntPtr pData, MyCamera.MV_FRAME_OUT_INFO stFrameInfo, int nIndex)
        {
            string[] path = { "image1.bmp", "image2.bmp", "image3.bmp", "image4.bmp" };
            int nRet;

            if ((3 * stFrameInfo.nFrameLen + 2048) > m_pMyCamera[nIndex].m_nBufSizeForSaveImage)
            {
                m_pMyCamera[nIndex].m_nBufSizeForSaveImage = 3 * stFrameInfo.nFrameLen + 2048;
                m_pMyCamera[nIndex].m_pBufForSaveImage = new byte[m_pMyCamera[nIndex].m_nBufSizeForSaveImage];
            }

            IntPtr pImage = Marshal.UnsafeAddrOfPinnedArrayElement(m_pMyCamera[nIndex].m_pBufForSaveImage, 0);
            MyCamera.MV_SAVE_IMAGE_PARAM_EX stSaveParam = new MyCamera.MV_SAVE_IMAGE_PARAM_EX();
            stSaveParam.enImageType = MyCamera.MV_SAVE_IAMGE_TYPE.MV_Image_Bmp;
            stSaveParam.enPixelType = stFrameInfo.enPixelType;
            stSaveParam.pData = pData;
            stSaveParam.nDataLen = stFrameInfo.nFrameLen;
            stSaveParam.nHeight = stFrameInfo.nHeight;
            stSaveParam.nWidth = stFrameInfo.nWidth;
            stSaveParam.pImageBuffer = pImage;
            stSaveParam.nBufferSize = m_pMyCamera[nIndex].m_nBufSizeForSaveImage;
            stSaveParam.nJpgQuality = 80;
            nRet = m_pMyCamera[nIndex].Cam_Info.MV_CC_SaveImageEx_NET(ref stSaveParam);
            if (MyCamera.MV_OK != nRet)
            {
                // string temp = "No.  + (nIndex + 1).ToString() +" + "Device save Failed!";
                //  ShowErrorMsg(temp, 0);
            }
            else
            {
                //m_pMyCamera[nIndex].Bmp = new Bitmap(stFrameInfo.nWidth, stFrameInfo.nHeight, stFrameInfo.nWidth * 3, PixelFormat.Format24bppRgb, pImage);

                MemoryStream memory = new MemoryStream(m_pMyCamera[nIndex].m_pBufForSaveImage);
                Bitmap bmp = (Bitmap)Image.FromStream(memory,true,true);
                memory.Close();

                m_pMyCamera[nIndex].Bmp = bmp;

                //if (INIClass.BitRotateFlipType != RotateFlipType.RotateNoneFlipNone)
                //    m_pMyCamera[nIndex].Bmp.RotateFlip(INIClass.BitRotateFlipType);

                m_pMyCamera[nIndex].isGetBmp = true;

                //FileStream file = new FileStream(path[nIndex], FileMode.Create, FileAccess.Write);
                //file.Write(m_pMyCamera[nIndex].m_pBufForSaveImage, 0, (int)stSaveParam.nImageLen);
                //file.Close();
                //string temp = "No." + (nIndex + 1).ToString() + "Device Save Succeed!";
                //ShowErrorMsg(temp,0);
            }
        }

        /// <summary>
        /// 取流回调函数  
        /// Aquisition Callback Function
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="pFrameInfo"></param>
        /// <param name="pUser"></param>
        private void MVS_ImageCallBack1(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO pFrameInfo, IntPtr pUser)
        {
            int nIndex = (int)pUser;
            // ch:抓取的帧数 | en:Aquired Frame Number
            ++m_nFrames[nIndex];


            MVS_GetImageNOW(pData, pFrameInfo, nIndex);
        }


        /// <summary>
        /// 获取即时的图片
        /// </summary>
        /// <param name="index">相机编号</param>
        /// <returns>即时的图片</returns>
        public Bitmap[] MVS_GetImageALL()
        {
            MVS_SetTriggerSoftware();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (; ; )
            {
                bool isok = true;
                for (int i = 0; i < m_pMyCamera.Length; i++)
                {
                    if (!m_pMyCamera[i].isGetBmp)
                        isok = false;
                }
                if (isok)
                    break;

                if (watch.ElapsedMilliseconds > 1000)
                    return null;
            }
            Bitmap[] bmplist = new Bitmap[m_pMyCamera.Length];

            for (int i = 0; i < m_pMyCamera.Length; i++)
                bmplist[i] = m_pMyCamera[i].Bmp.Clone() as Bitmap;

            return bmplist;
        }
        /// <summary>
        /// 获取即时的图片
        /// </summary>
        /// <param name="index">相机编号</param>
        /// <returns>即时的图片</returns>
        public Bitmap MVS_GetImageNOW(int index)
        {
            MVS_SetTriggerSoftware(index);
            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (; ; )
            {
                if (m_pMyCamera[index].isGetBmp)
                {
                    if(m_pMyCamera[index].Bmp ==null)
                        return null;
                    return m_pMyCamera[index].Bmp.Clone() as Bitmap;
                }

                if (watch.ElapsedMilliseconds > 1000)
                    break;
            }
            return null;
        }
        /// <summary>
        /// 获取已取得的图片
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Bitmap MVS_GetImageOLD(int index)
        {
            return m_pMyCamera[index].Bmp.Clone() as Bitmap;
        }


        /// <summary>
        /// 重置相机
        /// </summary>
        public void MVS_ResetMember()
        {
            m_pDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();

            m_nCanOpenDeviceNum = 0;
            m_nDevNum = 0;
            m_nDevNum = MVS_GetDeviceListAcq();
            m_pMyCamera = new CAMERA[m_nDevNum];
            for (int i = 0; i < m_nDevNum; i++)
            {
                m_pMyCamera[i].Cam_Info = new MyCamera();
                m_pMyCamera[i].m_nBufSizeForSaveImage = 3072 * 2048 * 3 * 3 + 2048;
                m_pMyCamera[i].m_pBufForSaveImage = new byte[3072 * 2048 * 3 * 3 + 2048];
            }
            m_nFrames = new int[m_nDevNum];
            cbImage = new MyCamera.cbOutputdelegate(MVS_ImageCallBack1);

            m_hDisplayHandle = new IntPtr[m_nDevNum];
        }

        /// <summary>
        /// ch:枚举设备 
        /// </summary>
        /// <returns>相机数量</returns>
        int MVS_GetDeviceListAcq()
        {
            int nRet;

            System.GC.Collect();
            nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref m_pDeviceList);
            if (0 != nRet)
                return 0;
            return (int)m_pDeviceList.nDeviceNum;
        }
        /// <summary>
        /// 载入海康相机顺序
        /// </summary>
        /// <returns></returns>
        List<CameraPar> MVS_LoadCamerPar()
        {
            string pathfile = SystemWORKPATH + "\\MVS" + "\\catalog.ini";
            if (File.Exists(pathfile))
            {
                return MVS_loadinipar(pathfile);
            }
            else
            {
                bool isok = MVS_SaveFile(SystemWORKPATH);
                if (!isok)
                    return null;

                return MVS_LoadCamerPar();
            }
        }
        /// <summary>
        /// 保存海康相机现在参数及顺序
        /// </summary>
        /// <param name="strPath"></param>
        /// <returns></returns>
        bool MVS_SaveFile(string strPath)
        {
            List<string> camerlist = new List<string>();
            CAMERA[] m_Temp = new CAMERA[m_nDevNum];
            List<CameraPar> camerparList = new List<CameraPar>();
            for (int j = 0; j < m_nDevNum; ++j)
            {

                CameraPar par = new CameraPar();
                par.FilePath = strPath;
                par.index = j;

                m_Temp[j].Cam_Info = new MyCamera();
                if (null == m_pMyCamera)
                    return false;

                MyCamera.MV_CC_DEVICE_INFO device =
                     (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_pDeviceList.pDeviceInfo[j],
                     typeof(MyCamera.MV_CC_DEVICE_INFO));

                int nRet = m_Temp[j].Cam_Info.MV_CC_CreateDevice_NET(ref device);
                if (MyCamera.MV_OK != nRet)
                    return false;

                nRet = m_Temp[j].Cam_Info.MV_CC_OpenDevice_NET();
                if (MyCamera.MV_OK != nRet)
                    return false;

                //获取相机序列号，型号
                if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                {
                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stGigEInfo, 0);
                    MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                    if (gigeInfo.chUserDefinedName != "")
                    {
                        par.ModelName = gigeInfo.chUserDefinedName;
                        par.CamerSerialNumber = gigeInfo.chSerialNumber;
                    }
                    else
                    {
                        par.ModelName = gigeInfo.chModelName;
                        par.CamerSerialNumber = gigeInfo.chSerialNumber;
                    }
                }
                else if (device.nTLayerType == MyCamera.MV_USB_DEVICE)
                {
                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stUsb3VInfo, 0);
                    MyCamera.MV_USB3_DEVICE_INFO usbInfo = (MyCamera.MV_USB3_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                    if (usbInfo.chUserDefinedName != "")
                    {
                        par.ModelName = usbInfo.chUserDefinedName;
                        par.CamerSerialNumber = usbInfo.chSerialNumber;
                    }
                    else
                    {
                        par.ModelName = usbInfo.chModelName;
                        par.CamerSerialNumber = usbInfo.chSerialNumber;
                    }
                }

                camerparList.Add(par);

                camerparList[j].Rotate = m_Temp[j].Rotate;
                bool isok = camerparList[j].Save(m_Temp[j].Cam_Info);

                if (!isok)
                    return false;

                nRet = m_Temp[j].Cam_Info.MV_CC_CloseDevice_NET();


            }

            string strMess = "";
            for (int i = 0; i < camerparList.Count; i++)
            {

                strMess += camerparList[i].index + "_";
                strMess += camerparList[i].ModelName + "_";
                strMess += camerparList[i].CamerSerialNumber + "_";
                strMess += camerparList[i].Rotate + "_" + Environment.NewLine;

            }
            if (!Directory.Exists(strPath + "\\MVS\\"))//若文件夹不存在则新建文件夹  
                Directory.CreateDirectory(strPath + "\\MVS\\"); //新建文件夹  

            File.WriteAllText(strPath + "\\MVS\\" + "\\catalog.ini", strMess, Encoding.ASCII);
            return true;
        }

      

        List<CameraPar> MVS_loadinipar(string strPath)
        {
            string mess = File.ReadAllText(strPath);
            mess = mess.Replace(Environment.NewLine, "$").Trim();
            string[] strfiles = mess.Split('$');
            List<CameraPar> camerparList = new List<CameraPar>();
            for (int i = 0; i < strfiles.Length; i++)
            {
                string temp = strfiles[i];
                if (temp == "")
                    continue;

                string[] strcamers = temp.Split('_');
                if (strcamers.Length > 3)
                {
                    CameraPar camera = new CameraPar();
                    camera.index = int.Parse(strcamers[0]);
                    camera.ModelName = strcamers[1];
                    camera.CamerSerialNumber = strcamers[2];
                    camera.FilePath = SystemWORKPATH;
                    switch (strcamers[3])
                    {
                        case "RotateNoneFlipNone":
                            //  camera.Rotate = RotateFlipType.RotateNoneFlipNone;
                            break;

                        case "Rotate180FlipXY":
                            camera.Rotate = RotateFlipType.Rotate180FlipXY;
                            break;
                        case "Rotate90FlipNone":
                            camera.Rotate = RotateFlipType.Rotate90FlipNone;
                            break;
                        case "Rotate270FlipXY":
                            camera.Rotate = RotateFlipType.Rotate270FlipXY;
                            break;
                        case "Rotate180FlipNone":
                            camera.Rotate = RotateFlipType.Rotate180FlipNone;
                            break;
                        case "RotateNoneFlipXY":
                            camera.Rotate = RotateFlipType.RotateNoneFlipXY;
                            break;
                        case "Rotate270FlipNone":
                            camera.Rotate = RotateFlipType.Rotate270FlipNone;
                            break;
                        case "Rotate90FlipXY":
                            camera.Rotate = RotateFlipType.Rotate90FlipXY;
                            break;
                        case "RotateNoneFlipX":
                            camera.Rotate = RotateFlipType.RotateNoneFlipX;
                            break;
                        case "Rotate180FlipY":
                            camera.Rotate = RotateFlipType.Rotate180FlipY;
                            break;
                        case "Rotate90FlipX":
                            camera.Rotate = RotateFlipType.Rotate90FlipX;
                            break;
                        case "Rotate270FlipY":
                            camera.Rotate = RotateFlipType.Rotate270FlipY;
                            break;
                        case "Rotate180FlipX":
                            camera.Rotate = RotateFlipType.Rotate180FlipX;
                            break;
                        case "RotateNoneFlipY":
                            camera.Rotate = RotateFlipType.RotateNoneFlipY;
                            break;
                        case "Rotate270FlipX":
                            camera.Rotate = RotateFlipType.Rotate270FlipX;
                            break;
                        case "Rotate90FlipY":
                            camera.Rotate = RotateFlipType.Rotate90FlipY;
                            break;
                    }

                    camerparList.Add(camera);
                }
            }

            return camerparList;
        }

        /// <summary>
        /// ch:初始化、打开相机
        /// </summary>
        /// <returns></returns>
        public bool MVS_Open()
        {
            bool bOpened = false;
            int nRet;

            //      LoadCamerPar();

            List<CameraPar> pars = MVS_LoadCamerPar();

            if (pars == null || pars.Count == 0)
                return false;

            int itemp = 0;
            foreach (CameraPar par in pars)
            {
              
                bool isok = false;
                for (int j = 0; j < m_nDevNum; ++j)
                {
                    //ch:获取选择的设备信息 | en:Get Selected Device Information

                    MyCamera.MV_CC_DEVICE_INFO device =
                        (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_pDeviceList.pDeviceInfo[j],
                        typeof(MyCamera.MV_CC_DEVICE_INFO));

                    string strSerialNumber = "";
                    if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                    {
                        IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stGigEInfo, 0);
                        MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_GIGE_DEVICE_INFO));

                        strSerialNumber = gigeInfo.chSerialNumber;
                    }
                    else if (device.nTLayerType == MyCamera.MV_USB_DEVICE)
                    {
                        IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stUsb3VInfo, 0);
                        MyCamera.MV_USB3_DEVICE_INFO usbInfo = (MyCamera.MV_USB3_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_USB3_DEVICE_INFO));

                        strSerialNumber = usbInfo.chSerialNumber;
                    }

                    if (strSerialNumber == par.CamerSerialNumber)
                    {
                        m_pMyCamera[itemp] = new CAMERA();


                        //ch:打开设备 | en:Open Device
                        if (null == m_pMyCamera[itemp].Cam_Info)
                        {
                            m_pMyCamera[itemp].Cam_Info = new MyCamera();
                            if (null == m_pMyCamera)
                            {
                                return false;
                            }
                        }
                        par.Load(m_pMyCamera[itemp].Cam_Info);

                        nRet = m_pMyCamera[itemp].Cam_Info.MV_CC_CreateDevice_NET(ref device);
                        if (MyCamera.MV_OK != nRet)
                        {
                            return false;
                        }

                        nRet = m_pMyCamera[itemp].Cam_Info.MV_CC_OpenDevice_NET();
                        if (MyCamera.MV_OK != nRet)
                        {
                            return false;
                        }
                        else
                        {
                            m_nCanOpenDeviceNum++;

                            // ch:探测网络最佳包大小(只对GigE相机有效) | en:Detection network optimal package size(It only works for the GigE camera)
                            if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                            {
                                int nPacketSize = m_pMyCamera[itemp].Cam_Info.MV_CC_GetOptimalPacketSize_NET();
                                if (nPacketSize > 0)
                                {
                                    nRet = m_pMyCamera[itemp].Cam_Info.MV_CC_SetIntValue_NET("GevSCPSPacketSize", (uint)nPacketSize);
                                    if (nRet != MyCamera.MV_OK)
                                    {
                                        Console.WriteLine("Warning: Set Packet Size failed {0:x8}", nRet);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Warning: Get Packet Size failed {0:x8}", nPacketSize);
                                }
                            }

                            m_pMyCamera[itemp].Cam_Info.MV_CC_SetEnumValue_NET("TriggerMode", 0);
                            m_pMyCamera[itemp].Cam_Info.MV_CC_RegisterImageCallBack_NET(cbImage, (IntPtr)itemp);
                            bOpened = true;
                            itemp++;
                            isok = true;
                            break;
                        }
                    }

                }
                if (!isok)
                    return false;

            }
            bOpened = MVS_StartGrab();
            if (!bOpened)
                return false;

            bOpened = MVS_SetTriggerMode(TriggerMode.Software);
            if (!bOpened)
                return false;

            bOpened = MVS_SetSoftTrigger();
            if (!bOpened)
                return false;

            return bOpened;

        }

        /// <summary>
        /// ch:关闭相机 
        /// </summary>
        /// <returns></returns>
        public bool MVS_Close()
        {
            for (int i = 0; i < m_nCanOpenDeviceNum; ++i)
            {
                int nRet;

                nRet = m_pMyCamera[i].Cam_Info.MV_CC_CloseDevice_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    return false;
                }

                nRet = m_pMyCamera[i].Cam_Info.MV_CC_DestroyDevice_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    return false;
                }
                //m_pMyCamera[i].Close();
            }

            // ch:重置成员变量 | en:Reset member variable
            MVS_ResetMember();

            return true;
        }

        /// <summary>
        /// 停止采集
        /// </summary>
        private void MVS_StopGrab()
        {
            for (int i = 0; i < m_nCanOpenDeviceNum; ++i)
            {
                m_pMyCamera[i].Cam_Info.MV_CC_StopGrabbing_NET();
            }
        }
        /// <summary>
        /// 停止采集
        /// </summary>
        /// <param name="index">相机编号</param>
        private void MVS_StopGrab(int index)
        {
            m_pMyCamera[index].Cam_Info.MV_CC_StopGrabbing_NET();
        }

        /// <summary>
        /// 开始抓取
        /// </summary>
        /// <returns>是否设定成功</returns>
        private bool MVS_StartGrab()
        {
            m_hDisplayHandle = new IntPtr[m_nCanOpenDeviceNum];
            int nRet;
            for (int i = 0; i < m_nCanOpenDeviceNum; i++)
                m_hDisplayHandle[i] = IntPtr.Zero;

            // ch:开始采集 | en:Start Grabbing
            for (int i = 0; i < m_nCanOpenDeviceNum; i++)
            {
                m_nFrames[i] = 0;
                nRet = m_pMyCamera[i].Cam_Info.MV_CC_StartGrabbing_NET();
                if (MyCamera.MV_OK != nRet)
                    return false;

                nRet = m_pMyCamera[i].Cam_Info.MV_CC_Display_NET(m_hDisplayHandle[i]);
                if (MyCamera.MV_OK != nRet)
                    return false;

            }

            return true;
        }
        /// <summary>
        /// 开始抓取
        /// </summary>
        /// <param name="index">相机编号</param>
        /// <returns>是否设定成功</returns>
        private bool MVS_StartGrab(int index)
        {
            if (index < MVS_CCDCount)
            {
                m_hDisplayHandle[index] = IntPtr.Zero;

                // ch:开始采集 | en:Start Grabbing
                m_nFrames[index] = 0;
                int nRet = m_pMyCamera[index].Cam_Info.MV_CC_StartGrabbing_NET();
                if (MyCamera.MV_OK != nRet)
                    return false;

                nRet = m_pMyCamera[index].Cam_Info.MV_CC_Display_NET(m_hDisplayHandle[index]);
                if (MyCamera.MV_OK != nRet)
                    return false;
            }

            return true;
        }
        /// <summary>
        /// 开始抓取
        /// </summary>
        /// <param name="index">相机编号</param>
        /// <param name="intPtr">控件句柄</param>
        /// <returns>是否设定成功</returns>
        private bool MVS_StartGrab(int index, IntPtr intPtr)
        {
            if (index < MVS_CCDCount)
            {
                m_hDisplayHandle[index] = intPtr;

                // ch:开始采集 | en:Start Grabbing
                m_nFrames[index] = 0;
                int nRet = m_pMyCamera[index].Cam_Info.MV_CC_StartGrabbing_NET();
                if (MyCamera.MV_OK != nRet)
                    return false;

                nRet = m_pMyCamera[index].Cam_Info.MV_CC_Display_NET(m_hDisplayHandle[index]);
                if (MyCamera.MV_OK != nRet)
                    return false;
            }

            return true;
        }
        /// <summary>
        /// 开始抓取
        /// </summary>
        /// <param name="intPtrlist">控件句柄集合</param>
        /// <returns>是否设定成功</returns>
        private bool MVS_StartGrab(IntPtr[] intPtrlist)
        {
            if (MVS_CCDCount == intPtrlist.Length)
            {
                m_hDisplayHandle = intPtrlist;
                // ch:开始采集 | en:Start Grabbing
                for (int i = 0; i < m_nCanOpenDeviceNum; i++)
                {
                    m_nFrames[i] = 0;
                    int nRet = m_pMyCamera[i].Cam_Info.MV_CC_StartGrabbing_NET();
                    if (MyCamera.MV_OK != nRet)
                        return false;

                    nRet = m_pMyCamera[i].Cam_Info.MV_CC_Display_NET(m_hDisplayHandle[i]);
                    if (MyCamera.MV_OK != nRet)
                        return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// 所有相机设置为连续采集
        /// </summary>
        /// <returns>是否设定OK</returns>
        public bool MVS_SetContinuesMode()
        {
            bool isok = true;
            for (int i = 0; i < m_nCanOpenDeviceNum; ++i)
            {
                int nRet = m_pMyCamera[i].Cam_Info.MV_CC_SetEnumValue_NET("TriggerMode", 0);
                if (nRet != MyCamera.MV_OK)
                    isok = false;
            }
            return isok;
        }
        /// <summary>
        /// CCD在线数量
        /// </summary>
        public int MVS_CCDCount
        {
            get
            {
                return m_nDevNum;
            }
        }


        /// <summary>
        /// 设定触发模式 为snap 并设为指定触发
        /// </summary>
        bool MVS_SetTriggerMode(TriggerMode triggerMode)
        {
            bool isok = true;
            for (int i = 0; i < m_nCanOpenDeviceNum; ++i)
            {
                m_pMyCamera[i].Cam_Info.MV_CC_SetEnumValue_NET("TriggerMode", 1);

                int nRet = m_pMyCamera[i].Cam_Info.MV_CC_SetEnumValue_NET("TriggerSource", (uint)triggerMode);
                if (nRet != MyCamera.MV_OK)
                    isok = false;

            }
            return isok;
        }
        /// <summary>
        /// 打开触发模式 
        /// </summary>
        /// <param name="index">相机编号</param>
        /// <param name="triggerMode">触发模式</param>
        /// <returns>设定是否OK</returns>
        public bool MVS_SetTriggerMode(int index, TriggerMode triggerMode)
        {
            //触发模式为Snap
            m_pMyCamera[index].Cam_Info.MV_CC_SetEnumValue_NET("TriggerMode", 1);
            int nRet = m_pMyCamera[index].Cam_Info.MV_CC_SetEnumValue_NET("TriggerSource", (uint)triggerMode);
            if (nRet != MyCamera.MV_OK)
                return false;
            else
                return true;
        }
        /// <summary>
        /// 设定所有的相机增益
        /// </summary>
        /// <param name="value">增益值</param>
        /// <returns>设定是否成功</returns>
        public bool MVS_SetGain(float value)
        {
            bool isok = true;

            for (int i = 0; i < m_nCanOpenDeviceNum; ++i)
            {

                m_pMyCamera[i].Cam_Info.MV_CC_SetEnumValue_NET("GainAuto", 0);

                int nRet = m_pMyCamera[i].Cam_Info.MV_CC_SetFloatValue_NET("Gain", value);
                if (MyCamera.MV_OK != nRet)
                    isok = false;
            }
            return isok;
        }
        /// <summary>
        /// 设定相机增益
        /// </summary>
        /// <param name="index">相机编号</param>
        /// <param name="value">增益值</param>
        /// <returns>设定是否成功</returns>
        public bool MVS_SetGain(int index, float value)
        {
            if (index < MVS_CCDCount)
            {
                m_pMyCamera[index].Cam_Info.MV_CC_SetEnumValue_NET("GainAuto", 0);
                int nRet = m_pMyCamera[index].Cam_Info.MV_CC_SetFloatValue_NET("Gain", value);
                if (MyCamera.MV_OK != nRet)
                    return false;
                else
                    return true;
            }
            else
                return false;
        }
        /// <summary>
        /// 设定所有的相机曝光
        /// </summary>
        /// <param name="value">曝光值</param>
        /// <returns>设定是否成功</returns>
        public bool MVS_SetExposure(float value)
        {
            bool isok = true;

            for (int i = 0; i < m_nCanOpenDeviceNum; ++i)
            {

                m_pMyCamera[i].Cam_Info.MV_CC_SetEnumValue_NET("ExposureAuto", 0);

                int nRet = m_pMyCamera[i].Cam_Info.MV_CC_SetFloatValue_NET("ExposureTime", value);
                if (MyCamera.MV_OK != nRet)
                    isok = false;
            }
            return isok;
        }
        /// <summary>
        /// 设定相机曝光
        /// </summary>
        /// <param name="index">相机编号</param>
        /// <param name="value">曝光值</param>
        /// <returns>设定是否成功</returns>
        public bool MVS_SetExposure(int index, float value)
        {
            if (index < MVS_CCDCount)
            {
                m_pMyCamera[index].Cam_Info.MV_CC_SetEnumValue_NET("ExposureAuto", 0);
                int nRet = m_pMyCamera[index].Cam_Info.MV_CC_SetFloatValue_NET("ExposureTime", value);
                if (MyCamera.MV_OK != nRet)
                    return false;
                else
                    return true;
            }
            else
                return false;
        }
        /// <summary>
        /// 将所有的相机触发源设为软触发
        /// </summary>
        /// <returns>是否成功</returns>
        public bool MVS_SetSoftTrigger()
        {
            bool isok = true;
            for (int i = 0; i < m_nCanOpenDeviceNum; ++i)
            {
                int nRet = m_pMyCamera[i].Cam_Info.MV_CC_SetEnumValue_NET("TriggerSource", 7);
                if (MyCamera.MV_OK != nRet)
                    isok = false;
            }
            return isok;
        }
        /// <summary>
        /// 触发源设为软触发
        /// </summary>
        /// <param name="index">相机编号</param>
        /// <returns>是否成功</returns>
        public bool MVS_SetSoftTrigger(int index)
        {
            if (index < MVS_CCDCount)
            {
                int nRet = m_pMyCamera[index].Cam_Info.MV_CC_SetEnumValue_NET("TriggerSource", 7);
                if (MyCamera.MV_OK != nRet)
                    return false;
                else
                    return true;
            }
            else
                return false;
        }
        /// <summary>
        /// 软触发命令
        /// </summary>
        /// <returns>指令下达是否ok</returns>
        public bool MVS_SetTriggerSoftware()
        {
            bool isok = true;
            for (int i = 0; i < m_nCanOpenDeviceNum; ++i)
            {
                m_pMyCamera[i].isGetBmp = false;
                int nRet = m_pMyCamera[i].Cam_Info.MV_CC_SetCommandValue_NET("TriggerSoftware");
                if (MyCamera.MV_OK != nRet)
                    isok = false;
            }
            return isok;
        }
        /// <summary>
        /// 软触发命令
        /// </summary>
        /// <param name="index">相机编号</param>
        /// <returns>指令下达是否ok</returns>
        public bool MVS_SetTriggerSoftware(int index)
        {
            if (index < MVS_CCDCount)
            {
                m_pMyCamera[index].isGetBmp = false;
                int nRet = m_pMyCamera[index].Cam_Info.MV_CC_SetCommandValue_NET("TriggerSoftware");
                if (MyCamera.MV_OK != nRet)
                    return false;
                else
                    return true;
            }
            else
                return false;
        }
        #endregion
#endif


        JzTimes CCDTimes = new JzTimes();
        int CCDms = 0;
        public bool GetImage(List<Bitmap> bmplist, int relateindex, int basecount = 0)
        {
            int index = Array.IndexOf(CCDRelateIndexArray, relateindex + basecount);

            Bitmap bmp;
            if (index > -1)
            {
                //Get Bitmap To Data
                switch (CCDType)
                {
                    case CCDTYPEEnum.FILE:

                        CCDTimes.Cut();

                        //bmp = bmplist[relateindex];
                        string bmpstring = WORKPATH + "\\" + (ENVPATH != "" ? ENVPATH + "\\" + PAGEOPTYPE + "-" : "") + relateindex.ToString("000") + Universal.GlobalImageTypeString;

                        if (!File.Exists(bmpstring))
                            bmpstring = SystemWORKPATH + "\\" + relateindex.ToString("000") + Universal.GlobalImageTypeString;
                        //bmplist.RemoveAt(relateindex);
                        //bmp.Dispose();

                        switch (VERSION)
                        {
                            case VersionEnum.ALLINONE:

                                switch (OPTION)
                                {
                                    case OptionEnum.MAIN_SDM3:
                                    case OptionEnum.MAIN_SDM2:
                                    case OptionEnum.MAIN_SDM1:

                                        bmpstring = WORKPATH + "\\" + (ENVPATH != "" ? ENVPATH + "\\" + PAGEOPTYPE + "-" : "") + (relateindex + basecount).ToString("000") + Universal.GlobalImageTypeString;
                                        if (!File.Exists(bmpstring))
                                            bmpstring = SystemWORKPATH + "\\" + (relateindex + basecount).ToString("000") + Universal.GlobalImageTypeString;

                                        //if (!File.Exists(bmpstring))
                                        //    bmpstring = SystemWORKPATH + "\\" + (relateindex + basecount).ToString("000") + Universal.GlobalImageTypeString;



                                        break;
                                }


                                break;
                        }

                        bmp = new Bitmap(1, 1);


                        GetBMP(bmpstring, ref bmp);

                        if (relateindex >= bmplist.Count)
                            relateindex = 0;
                        RenderBMP(bmp, bmplist[relateindex]);

                        //bmplist.Insert(relateindex, bmp);

                        bmp.Dispose();

                        CCDms = CCDTimes.msDuriation;

                        break;
                    case CCDTYPEEnum.EPIX:

                        bmp = bmplist[relateindex];

                        EPIXRender(index, RotationDegreeArray[index], bmp);

                        switch (VERSION)
                        {
                            case VersionEnum.ALLINONE:

                                switch (OPTION)
                                {
                                    case OptionEnum.MAIN_X6:

                                        AForge.Imaging.Filters.ExtractChannel extract = new AForge.Imaging.Filters.ExtractChannel();
                                        extract.Channel = AForge.Imaging.RGB.R;
                                        bmplist[relateindex] = new Bitmap(extract.Apply(bmp));

                                        break;
                                }


                                break;
                        }

                        break;
#if PTG
                    case CCDTYPEEnum.PTG:
                        try
                        {
                            PTGCAM[index].RetrieveBuffer(PTGRowImage[index]);

                            lock (this)
                            {
                                PTGRowImage[index].Convert(FlyCapture2Managed.PixelFormat.PixelFormatBgr, PTGProcessedImage[index]);

                                //bmp = bmplist[relateindex];
                                //bmp = new Bitmap(PTGProcessedImage[index].bitmap);

                                RenderBMP(PTGProcessedImage[index].bitmap, bmplist[relateindex]);
                                
                                switch(RotationDegreeArray[index])
                                {
                                    case 180:
                                        bmplist[relateindex].RotateFlip(RotateFlipType.Rotate180FlipNone);
                                        break;
                                    case 90:
                                        bmplist[relateindex].RotateFlip(RotateFlipType.Rotate90FlipNone);
                                        break;
                                    case 270:
                                        bmplist[relateindex].RotateFlip(RotateFlipType.Rotate270FlipNone);
                                        break;
                                }   
                            }
                        }
                        catch (FC2Exception ex)
                        {
                         JetEazy.LoggerClass.Instance.WriteException(ex);
                        }

                        break;
#endif
#if ICAM
                    case CCDTYPEEnum.ICAM:
                        Bitmap bmptmp;

                        if (FrameRateArray[index] == -1)
                            bmptmp = CamerList[index].SnapshotSourceImage();
                        else
                            bmptmp = CamerList[index].SnapshotOutputImage();

                        SizeDefArray[index].SizeDefBMP(ref bmptmp);

                        //bmplist[relateindex].Dispose();
                        //bmplist[relateindex] = new Bitmap(bmptmp);

                        RenderBMP(bmptmp, bmplist[relateindex]);

                        bmptmp.Dispose();

                        break;
#endif
#if TIS || TISUSB
                    case CCDTYPEEnum.TIS:
                    // break;
                    case CCDTYPEEnum.TISUSB:
                        TISCAM[index].LiveCapturePause = false;
                        TISCAM[index].LiveCaptureContinuous = true;
                        TISSoftTrigger[index].Push();
                        //取像问题 建立于20240711
                        if (TISImageBuffer[index] != null)
                        {
                            // bmp = bmplist[relateindex];

                            //Bitmap bmptemp = TISImageBuffer[index].Bitmap;
                            //bmptemp = bmptemp.Clone(new Rectangle(0, 0, bmptemp.Width, bmptemp.Height), PixelFormat.Format32bppArgb);
                            //bmplist[relateindex] = bmptemp;// ( TISImageBuffer[index].Bitmap).Clone() as Bitmap;

                            //EPIXRender(index, RotationDegreeArray[index], bmp);

                            //Bitmap tis

                            //3648x2432
                            int iw = 3648;
                            int ih = 2432;
                            iw = SizeDefArray[relateindex].OrgSize.Width;
                            ih = SizeDefArray[relateindex].OrgSize.Height;
                            try
                            {
                                if (bmplist[relateindex] != null)
                                {
                                    iw = bmplist[relateindex].Width;
                                    ih = bmplist[relateindex].Height;
                                }
                            }
                            catch
                            {

                            }

                            Bitmap bmpnewtemp = new Bitmap(iw, ih);
                            RenderBMPSizeChange(TISImageBuffer[index].Bitmap, bmpnewtemp);
                            bmplist[relateindex] = bmpnewtemp;// new Bitmap(bmpnewtemp);

                            //bmplist[relateindex].Save("D://Temp.png");
                        }

                        //TIS.Imaging.FrameSnapSink snapSink = TISCAM[index].Sink as TIS.Imaging.FrameSnapSink;
                        //TIS.Imaging.IFrameQueueBuffer frm = snapSink.SnapSingle(TimeSpan.FromSeconds(5));
                        //RenderBMPSizeChange(frm.CreateBitmapWrap(), bmplist[relateindex]);

                        break;
#endif
#if IWIN
                    case CCDTYPEEnum.IWIN:
                        
                        bmp = bmplist[relateindex];
                        WINDVisionRender(index, RotationDegreeArray[index], bmp);
                     //   CCDms[index] = CCDTimes.msDuriation;

                        break;
#endif
#if MVS
                    case CCDTYPEEnum.MVS:

                        bmp = MVS_GetImageNOW(index);
                   //     bmp.Save("D:\\test.png");
                        bmplist[relateindex] = bmp;
                        break;
#endif
#if DVP2
                    case CCDTYPEEnum.DVP2:

                        bmp = dvp2GetImageNow(index);
                        //bmp.Save("D:\\test.bmp");
                        bmplist[relateindex] = bmp;
                        break;
#endif
                }
            }

            return index > -1;
        }
        public bool GetImageSDM1(List<Bitmap> bmplist, int relateindex, int basecount = 0)
        {
            int index = 0;// Array.IndexOf(CCDRelateIndexArray, relateindex + basecount);

            Bitmap bmp;
            if (index > -1)
            {
                //Get Bitmap To Data
                switch (CCDType)
                {
                    case CCDTYPEEnum.FILE:

                        CCDTimes.Cut();

                        //bmp = bmplist[relateindex];
                        string bmpstring = WORKPATH + "\\" + (ENVPATH != "" ? ENVPATH + "\\" + PAGEOPTYPE + "-" : "") + relateindex.ToString("000") + Universal.GlobalImageTypeString;

                        if (!File.Exists(bmpstring))
                            bmpstring = SystemWORKPATH + "\\" + relateindex.ToString("000") + Universal.GlobalImageTypeString;
                        //bmplist.RemoveAt(relateindex);
                        //bmp.Dispose();

                        switch (VERSION)
                        {
                            case VersionEnum.ALLINONE:

                                switch (OPTION)
                                {
                                    case OptionEnum.MAIN_SDM3:
                                    case OptionEnum.MAIN_SDM2:
                                    case OptionEnum.MAIN_SDM1:

                                        bmpstring = WORKPATH + "\\" + (ENVPATH != "" ? ENVPATH + "\\" + PAGEOPTYPE + "-" : "") + (relateindex + basecount).ToString("000") + Universal.GlobalImageTypeString;
                                        if (!File.Exists(bmpstring))
                                            bmpstring = SystemWORKPATH + "\\" + (relateindex + basecount).ToString("000") + Universal.GlobalImageTypeString;

                                        if (!File.Exists(bmpstring))
                                            bmpstring = WORKPATH + "\\" + (ENVPATH != "" ? ENVPATH + "\\" + PAGEOPTYPE + "-" : "") + (relateindex + basecount).ToString("000") + ".jpg";
                                        if (!File.Exists(bmpstring))
                                            bmpstring = SystemWORKPATH + "\\" + (relateindex + basecount).ToString("000") + ".jpg";

                                        break;
                                }


                                break;
                        }

                        bmp = new Bitmap(1, 1);


                        GetBMP(bmpstring, ref bmp);

                        if (relateindex >= bmplist.Count)
                            relateindex = 0;
                        RenderBMP(bmp, bmplist[relateindex]);

                        //bmplist.Insert(relateindex, bmp);

                        bmp.Dispose();

                        CCDms = CCDTimes.msDuriation;

                        break;
                    case CCDTYPEEnum.EPIX:

                        bmp = bmplist[relateindex];

                        EPIXRender(index, RotationDegreeArray[index], bmp);

                        switch (VERSION)
                        {
                            case VersionEnum.ALLINONE:

                                switch (OPTION)
                                {
                                    case OptionEnum.MAIN_X6:

                                        AForge.Imaging.Filters.ExtractChannel extract = new AForge.Imaging.Filters.ExtractChannel();
                                        extract.Channel = AForge.Imaging.RGB.R;
                                        bmplist[relateindex] = new Bitmap(extract.Apply(bmp));

                                        break;
                                }


                                break;
                        }

                        break;
#if PTG
                    case CCDTYPEEnum.PTG:
                        try
                        {
                            PTGCAM[index].RetrieveBuffer(PTGRowImage[index]);

                            lock (this)
                            {
                                PTGRowImage[index].Convert(FlyCapture2Managed.PixelFormat.PixelFormatBgr, PTGProcessedImage[index]);

                                //bmp = bmplist[relateindex];
                                //bmp = new Bitmap(PTGProcessedImage[index].bitmap);

                                RenderBMP(PTGProcessedImage[index].bitmap, bmplist[relateindex]);
                                
                                switch(RotationDegreeArray[index])
                                {
                                    case 180:
                                        bmplist[relateindex].RotateFlip(RotateFlipType.Rotate180FlipNone);
                                        break;
                                    case 90:
                                        bmplist[relateindex].RotateFlip(RotateFlipType.Rotate90FlipNone);
                                        break;
                                    case 270:
                                        bmplist[relateindex].RotateFlip(RotateFlipType.Rotate270FlipNone);
                                        break;
                                }   
                            }
                        }
                        catch (FC2Exception ex)
                        {
                         JetEazy.LoggerClass.Instance.WriteException(ex);
                        }

                        break;
#endif
#if ICAM
                    case CCDTYPEEnum.ICAM:
                        Bitmap bmptmp;

                        if (FrameRateArray[index] == -1)
                            bmptmp = CamerList[index].SnapshotSourceImage();
                        else
                            bmptmp = CamerList[index].SnapshotOutputImage();

                        SizeDefArray[index].SizeDefBMP(ref bmptmp);

                        //bmplist[relateindex].Dispose();
                        //bmplist[relateindex] = new Bitmap(bmptmp);

                        RenderBMP(bmptmp, bmplist[relateindex]);

                        bmptmp.Dispose();

                        break;
#endif
#if TIS || TISUSB
                    case CCDTYPEEnum.TIS:
                    // break;
                    case CCDTYPEEnum.TISUSB:
                        //TISCAM[index].LiveCapturePause = false;
                        //TISCAM[index].LiveCaptureContinuous = true;

                        //TISSoftTrigger[index].Push();

                        //if (TISImageBuffer[index] != null)
                        //{
                        //    // bmp = bmplist[relateindex];

                        //    //Bitmap bmptemp = TISImageBuffer[index].Bitmap;
                        //    //bmptemp = bmptemp.Clone(new Rectangle(0, 0, bmptemp.Width, bmptemp.Height), PixelFormat.Format32bppArgb);
                        //    //bmplist[relateindex] = bmptemp;// ( TISImageBuffer[index].Bitmap).Clone() as Bitmap;

                        //    //EPIXRender(index, RotationDegreeArray[index], bmp);

                        //    //Bitmap tis

                        //    RenderBMPSizeChange(TISImageBuffer[index].Bitmap, bmplist[relateindex]);

                        //    //bmplist[relateindex].Save("D://Temp.png");
                        //}

                        break;
#endif
#if IWIN
                    case CCDTYPEEnum.IWIN:
                        
                        bmp = bmplist[relateindex];
                        WINDVisionRender(index, RotationDegreeArray[index], bmp);
                     //   CCDms[index] = CCDTimes.msDuriation;

                        break;
#endif
#if MVS
                    case CCDTYPEEnum.MVS:

                        bmp = MVS_GetImageNOW(index);
                   //     bmp.Save("D:\\test.png");
                        bmplist[relateindex] = bmp;
                        break;
#endif
#if DVP2
                    case CCDTYPEEnum.DVP2:
                        bmp = dvp2GetImageNow(index);
                        //bmp.Save("D:\\test.png");
                        bmplist[relateindex] = bmp;
                        break;
#endif
                }
            }

            return index > -1;
        }

        public bool GetImageDX(List<Bitmap> bmplist, int relateindex, int basecount = 0, int ccdindex = 0)
        {
            int index = Array.IndexOf(CCDRelateIndexArray, relateindex + basecount);

            Bitmap bmp;
            if (index > -1)
            {
                //Get Bitmap To Data
                switch (CCDType)
                {
                    case CCDTYPEEnum.FILE:

                        CCDTimes.Cut();

                        //bmp = bmplist[relateindex];
                        string bmpstring = WORKPATH + "\\" + (ENVPATH != "" ? ENVPATH + "\\" + PAGEOPTYPE + "-" : "") + ccdindex.ToString("000") + Universal.GlobalImageTypeString;

                        //bmplist.RemoveAt(relateindex);
                        //bmp.Dispose();

                        bmp = new Bitmap(1, 1);
                        GetBMP(bmpstring, ref bmp);

                        RenderBMP(bmp, bmplist[relateindex]);

                        //bmplist.Insert(relateindex, bmp);

                        bmp.Dispose();

                        CCDms = CCDTimes.msDuriation;

                        break;
                    case CCDTYPEEnum.EPIX:

                        bmp = bmplist[relateindex];

                        EPIXRender(index, RotationDegreeArray[index], bmp);

                        switch (VERSION)
                        {
                            case VersionEnum.ALLINONE:

                                switch (OPTION)
                                {
                                    case OptionEnum.MAIN_X6:

                                        AForge.Imaging.Filters.ExtractChannel extract = new AForge.Imaging.Filters.ExtractChannel();
                                        extract.Channel = AForge.Imaging.RGB.R;
                                        bmplist[relateindex] = new Bitmap(extract.Apply(bmp));

                                        break;
                                }


                                break;
                        }

                        break;
#if PTG
                    case CCDTYPEEnum.PTG:
                        try
                        {
                            PTGCAM[index].RetrieveBuffer(PTGRowImage[index]);

                            lock (this)
                            {
                                PTGRowImage[index].Convert(FlyCapture2Managed.PixelFormat.PixelFormatBgr, PTGProcessedImage[index]);

                                //bmp = bmplist[relateindex];
                                //bmp = new Bitmap(PTGProcessedImage[index].bitmap);

                                RenderBMP(PTGProcessedImage[index].bitmap, bmplist[relateindex]);
                                
                                switch(RotationDegreeArray[index])
                                {
                                    case 180:
                                        bmplist[relateindex].RotateFlip(RotateFlipType.Rotate180FlipNone);
                                        break;
                                    case 90:
                                        bmplist[relateindex].RotateFlip(RotateFlipType.Rotate90FlipNone);
                                        break;
                                    case 270:
                                        bmplist[relateindex].RotateFlip(RotateFlipType.Rotate270FlipNone);
                                        break;
                                }   
                            }
                        }
                        catch (FC2Exception ex)
                        {
                         JetEazy.LoggerClass.Instance.WriteException(ex);
                        }

                        break;
#endif
#if ICAM
                    case CCDTYPEEnum.ICAM:
                        Bitmap bmptmp;

                        if (FrameRateArray[index] == -1)
                            bmptmp = CamerList[index].SnapshotSourceImage();
                        else
                            bmptmp = CamerList[index].SnapshotOutputImage();

                        SizeDefArray[index].SizeDefBMP(ref bmptmp);

                        //bmplist[relateindex].Dispose();
                        //bmplist[relateindex] = new Bitmap(bmptmp);

                        RenderBMP(bmptmp, bmplist[relateindex]);

                        bmptmp.Dispose();

                        break;
#endif
#if TIS || TISUSB
                    case CCDTYPEEnum.TIS:
                    // break;
                    case CCDTYPEEnum.TISUSB:
                        //TISCAM[index].LiveCapturePause = false;
                        //TISCAM[index].LiveCaptureContinuous = true;

                        //TISSoftTrigger[index].Push();

                        //if (TISImageBuffer[index] != null)
                        //{
                        //    // bmp = bmplist[relateindex];

                        //    //Bitmap bmptemp = TISImageBuffer[index].Bitmap;
                        //    //bmptemp = bmptemp.Clone(new Rectangle(0, 0, bmptemp.Width, bmptemp.Height), PixelFormat.Format32bppArgb);
                        //    //bmplist[relateindex] = bmptemp;// ( TISImageBuffer[index].Bitmap).Clone() as Bitmap;

                        //    RenderBMPSizeChange(TISImageBuffer[index].Bitmap, bmplist[relateindex]);

                        //    //bmplist[relateindex].Save("D://Temp.png");
                        //}

                        break;
#endif
#if IWIN
                    case CCDTYPEEnum.IWIN:

                        bmp = bmplist[relateindex];
                        WINDVisionRender(index, RotationDegreeArray[index], bmp);
                        //   CCDms[index] = CCDTimes.msDuriation;

                        break;
#endif
#if MVS
                    case CCDTYPEEnum.MVS:

                        bmp = bmplist[relateindex];
                        bmp = MVS_GetImageNOW(index);

                        break;
#endif
#if DVP2
                    case CCDTYPEEnum.DVP2:

                        bmp = bmplist[relateindex];
                        bmp = dvp2GetImageNow(index);
                        break;
#endif
                }
            }

            return index > -1;
        }
        /// <summary>
        /// This GetImage is R32 Use Only
        /// </summary>
        /// <param name="relateindex"></param>
        /// <param name="basecount"></param>
        /// <returns></returns>
        public bool GetImage(int relateindex, int basecount = 0)
        {
            int index = Array.IndexOf(CCDRelateIndexArray, relateindex + basecount);

            if (index > -1)
            {
                //Get Bitmap To Data
                switch (CCDType)
                {
                    case CCDTYPEEnum.FILE:

                        CCDTimes.Cut();

                        string bmpstring = WORKPATH + "\\" + (ENVPATH != "" ? ENVPATH + "\\" + PAGEOPTYPE + "-" : "") + relateindex.ToString("000") + Universal.GlobalImageTypeString;

                        if (!File.Exists(bmpstring))
                            bmpstring = SystemWORKPATH + "\\" + relateindex.ToString("000") + Universal.GlobalImageTypeString;

                        GetBMP(bmpstring, ref bmpR32Captured[index]);

                        CCDms = CCDTimes.msDuriation;

                        break;
                    case CCDTYPEEnum.EPIX:

                        EPIXRender(index, RotationDegreeArray[index], bmpR32Captured[index]);

                        break;
#if ICAM
                    case CCDTYPEEnum.ICAM:
                        Bitmap bmptmp;
                        
                       if (FrameRateArray[index] == -1)
                            bmptmp = CamerList[index].SnapshotOutputImage();
                        else
                            bmptmp = CamerList[index].SnapshotSourceImage();

                        SizeDefArray[index].SizeDefBMP(ref bmptmp);

                        bmpR32Captured[index].Dispose();
                        bmpR32Captured[index] = new Bitmap(bmptmp);

                        bmptmp.Dispose();

                        break;
#endif
#if IWIN
                    case CCDTYPEEnum.IWIN:

                        Bitmap bmp = bmpR32Captured[index];
                        WINDVisionRender(index, RotationDegreeArray[index], bmp);
                        //   CCDms[index] = CCDTimes.msDuriation;

                        break;
#endif
#if MVS
                    case CCDTYPEEnum.MVS:

                  //      Bitmap mvsbmp = bmpR32Captured[index];
                        bmpR32Captured[index] = MVS_GetImageNOW(index);

               //           bmpR32Captured[index].Save("D:\\mytest.png");
                        break;
#endif
#if DVP2
                    case CCDTYPEEnum.DVP2:

                        //bmp = dvp2GetImageNow(index);
                        //     bmp.Save("D:\\test.png");
                        bmpR32Captured[relateindex] = dvp2GetImageNow(index);
                        break;
#endif
                }
            }

            return index > -1;
        }
        /// <summary>
        /// This Fuction No Use Now
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="relateindex"></param>
        public void GetImageEX(Bitmap bmp, int relateindex)
        {
            int index = Array.IndexOf(CCDRelateIndexArray, relateindex);

            //Bitmap bmp;
            if (index > -1)
            {
                //Get Bitmap To Data
                switch (CCDType)
                {
                    case CCDTYPEEnum.FILE:

                        //CCDTimes.Cut();

                        ////m_bmptmp[index] = bmplist;
                        //string bmpstring = WORKPATH + "\\" + (ENVPATH != "" ? ENVPATH + "\\" + PAGEOPTYPE + "-" : "") + relateindex.ToString("000") + Universal.GlobalImageTypeString;

                        //m_bmptmp[index] = new Bitmap(1, 1);
                        //GetBMP(bmpstring, ref m_bmptmp[index]);

                        //bmplist.Dispose();
                        //bmplist = new Bitmap(m_bmptmp[index]);

                        ////bmplist.Save("D:\\" + index.ToString() + ".PNG");
                        //m_bmptmp[index].Dispose();

                        //CCDms = CCDTimes.msDuriation;

                        break;
                    case CCDTYPEEnum.EPIX:

                        bmpR32Captured[index] = bmp;

                        EPIXRender(index, RotationDegreeArray[index], bmpR32Captured[index]);

                        break;
#if ICAM
                    case CCDTYPEEnum.ICAM:

                        bmpR32Captured[index] = bmp;

                        if (FrameRateArray[index] == -1)
                            bmpR32Captured[index] = CamerList[index].SnapshotSourceImage();
                        else
                            bmpR32Captured[index] = CamerList[index].SnapshotOutputImage();

                        break;
#endif
#if MVS
                    case CCDTYPEEnum.MVS:

                      
                        bmp = MVS_GetImageNOW(index);
                        bmpR32Captured[relateindex] = bmp;
                        break;
#endif
#if DVP2
                    case CCDTYPEEnum.DVP2:

                        bmp = dvp2GetImageNow(index);
                        //     bmp.Save("D:\\test.png");
                        bmpR32Captured[relateindex] = bmp;
                        break;
#endif

                }
            }
        }

        public Bitmap GetBMP(int relateindex, int basecount = 0)
        {
            Bitmap bmp = bmpR32Captured[0];

            int index = Array.IndexOf(CCDRelateIndexArray, relateindex + basecount);

            if (index > -1)
            {
                //Get Bitmap To Data
                bmp = bmpR32Captured[index];
            }

            return bmp;
        }
        /// <summary>
        /// 检测CCD编号对应的位置
        /// </summary>
        /// <param name="relateindex">虚拟编号</param>
        /// <param name="basecount"></param>
        /// <returns></returns>
        public bool CheckIndex(int relateindex, int basecount = 0)
        {
            bool ret = false;

            int index = Array.IndexOf(CCDRelateIndexArray, relateindex + basecount);

            //if (index > -1)
            //{
            //    ret = true;
            //}

            ret = index > -1;

            return ret;
        }
        public void EPIXRender(int index, int roattion, Bitmap bmp)
        {
            switch (roattion)
            {
                case 90:
                case 270:
                    bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
            }

            //int countnow = pxd_capturedFieldCount(1 << index);

            //if (FrameRateArray[index] == -1)
            //  pxd_goLive((1 << index), 0);
            // pxd_goSnap((1 << index), 0);
            pxd_doSnap(1 << index, 1, 1000);
            //countnow = pxd_capturedFieldCount(1 << index);

            Graphics g = Graphics.FromImage(bmp);
            IntPtr hDC = g.GetHdc();
            SetStretchBltMode(hDC, STRETCH_DELETESCANS);

            pxd_renderStretchDIBits((1 << index), 1, 0, 0, -1, -1, 0, hDC, 0, 0, bmp.Width, bmp.Height, 0);

            g.ReleaseHdc(hDC);
            g.Dispose();

            switch (roattion)
            {
                case 90:
                    bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 270:
                    bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
                case 180:
                    bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
            }


            //switch(VERSION)
            //{
            //    case VersionEnum.ALLINONE:

            //        switch (OPTION)
            //        {
            //            case OptionEnum.MAIN_X6:

            //                AForge.Imaging.Filters.ExtractChannel extract = new AForge.Imaging.Filters.ExtractChannel();
            //                extract.Channel = AForge.Imaging.RGB.R;
            //                bmp = extract.Apply(bmp);

            //                break;
            //        }


            //        break;
            //}

        }

        //   public static string Exposure = "";

        public bool SetExposure(float expvalue, int relateindex)
        {
            int index = Array.IndexOf(CCDRelateIndexArray, relateindex);

            //   JetEazy.LoggerClass.Instance.WriteLog("CCD 编号："+ index+" 设一亮度值："+ expvalue);


            //    Exposure +="序号: "+ relateindex+ " CCD号: "+ index+ " 值: " + expvalue + Environment.NewLine;
            if (index > -1)
            {
                switch (CCDType)
                {
                    case CCDTYPEEnum.EPIX:
                        //pxd_SV9M001_setExposureAndDigitalGain(1 << index, 0, (double)((expvalue / 100f) * (float)ExposureBaseArray[index]),0, 0, 0, 0);
                        pxd_SILICONVIDEO_setExposureColorGainOffsets(1 << index, 0, (double)((expvalue / 100f) * (float)ExposureBaseArray[index]), (IntPtr)0, (IntPtr)0, (IntPtr)0, (IntPtr)0);
                        //pxd_SV9M001_setExposureAndDigitalGain
                        break;
#if PTG
                    case CCDTYPEEnum.PTG:
                        PTGCAMProperty[index] = PTGCAM[index].GetProperty(PropertyType.Shutter);
                        PTGCAMProperty[index].valueA = (uint)(((float)expvalue / 100f) * (float)ExposureBaseArray[index]);

                        PTGCAM[index].SetProperty(PTGCAMProperty[index]);

                        break;
#endif
#if TIS || TISUSB
                    case CCDTYPEEnum.TIS:
                    case CCDTYPEEnum.TISUSB:
                        TIS_SetExposureAbs(TISCAM[index], (double)expvalue / (float)ExposureBaseArray[index]);
                        //TISCAM[CameraNo].ShowPropertyDialog();
                        break;
#endif
#if IWIN
                    case CCDTYPEEnum.IWIN:
                      //  WINDVisionSetExposure(index, (double)((expvalue / 100f) * (float)ExposureBaseArray[index]));
                        WINDVisionSetExposureRatio(index, expvalue);
                        break;
#endif
#if MVS
                    case CCDTYPEEnum.MVS:

                        float vslue = ((expvalue / 100f) * (float)ExposureBaseArray[index]);
                        MVS_SetExposure(index, vslue);
                        break;
#endif

#if DVP2
                    case CCDTYPEEnum.DVP2:

                        float value = ((expvalue / 100f) * (float)ExposureBaseArray[index]);
                        dvp2SetExpo(index, value);
                        break;
#endif



                }
            }
            return index > -1;
        }
        //public bool SetExposure(string exposurestr, int relateindex)
        //{
        //    int index = Array.IndexOf(CCDRelateIndexArray, relateindex);

        //    if (index > -1)
        //    {
        //        switch (CCDType)
        //        {
        //            case CCDTYPEEnum.EPIX:
        //                //pxd_SV9M001_setExposureAndDigitalGain(1 << index, 0, (double)((expvalue / 100f) * (float)ExposureBaseArray[index]),0, 0, 0, 0);
        //                //pxd_SILICONVIDEO_setExposureColorGainOffsets(1 << index, 0, (double)((expvalue / 100f) * (float)ExposureBaseArray[index]), (IntPtr)0, (IntPtr)0, (IntPtr)0, (IntPtr)0);
        //                //pxd_SV9M001_setExposureAndDigitalGain
        //                break;
        //        }
        //    }
        //    return index > -1;
        //}
        public void Close()
        {
            switch (CCDType)
            {
                case CCDTYPEEnum.FILE:

                    break;
                case CCDTYPEEnum.EPIX:
                    pxd_PIXCIclose();
                    break;
#if TIS || TISUSB
                case CCDTYPEEnum.TIS:
                case CCDTYPEEnum.TISUSB:
                    TISClose();
                    break;
#endif
#if IWIN
                case CCDTYPEEnum.IWIN:
                    foreach (CameraHandle windcam in m_hCamera)
                    {
                        if (windcam > 0)
                        {
                            MvApi.CameraUnInit(windcam);
                            WINDVisionClose(windcam);
                            //Marshal.FreeHGlobal(m_ImageBuffer);
                            //Marshal.FreeHGlobal(m_ImageBufferSnapshot);
                        }
                    }
                    break;
#endif
#if MVS
                case CCDTYPEEnum.MVS:
                    MVS_Close();
                    break;
#endif

#if DVP2
                case CCDTYPEEnum.DVP2:
                    dvp2Dispose();
                    break;
#endif
            }

            //foreach(int relateindex in CCDRelateIndexArray)
            //{
            //    Close(relateindex);
            //}
        }
        public void Close(int relateindex)
        {
            int index = Array.IndexOf(CCDRelateIndexArray, relateindex);

            if (index > -1)
            {
                //switch(CCDType)
                //{
                //    case CCDTYPEEnum.FILE:

                //        break;
                //    case CCDTYPEEnum.EPIX:
                //        pxd_PIXCIclose();
                //        break;
                //}

            }

        }
        /// <summary>
        /// 若CCD連線有問題時必需重置
        /// </summary>
        public void ResetConnection()
        {

        }


        const int ReConnectionConut = 1000;
        const int CountErrorCount = 188;

        int[] LastCount;
        int[] CountErrorRetry;
        int[] SideErrorRetry;

        StringBuilder m_strErr = new StringBuilder(1024);

        public bool IsConnectionFail = false;
        bool IsLiveAgain = true;
        /// <summary>
        /// 檢查CCD是否有問題
        /// </summary>
        void CheckConnection()
        {
            ErrorConnection = "";

            switch (CCDType)
            {
                case CCDTYPEEnum.EPIX:

                    if (Universal.IsDebug)
                        return;

                    if (IsConnectionFail)
                        return;

                    int sideindex = 0;

                    while (sideindex < LastCapturedCount.Length)
                    {
                        int countnow = pxd_capturedFieldCount(1 << sideindex);

                        if (CountErrorRetry[sideindex] < CountErrorCount)
                        {
                            if (LastCount[sideindex] != countnow)
                            {
                                IsLiveAgain = true;
                                LastCount[sideindex] = countnow;
                                CountErrorRetry[sideindex] = 0;
                            }
                            else
                            {
                                CountErrorRetry[sideindex]++;

                                pxd_goUnLive(1 << sideindex);
                                pxd_goLive(1 << sideindex, 1);

                                //ErrorWriter.WriteLine(TimerClass.DateTimeString + "," + "Camera " + ((int)sideindex + 1).ToString() + " Lost Connection " + CountErrorRetry[(int)sideindex].ToString() + " times.");
                                if (CountErrorRetry[sideindex] >= CountErrorCount)
                                {
                                    IsConnectionFail = true;
                                    IsLiveAgain = false;
                                    //MessageBox.Show("像機 " + ((int)sideindex + 1).ToString() + " 連線錯誤，請檢查傳輸。。", "MAIN", MessageBoxButtons.OK);

                                    //IsConnectionFail = false;
                                    CountErrorRetry[sideindex] = 0;
                                }
                            }
                        }
                        sideindex++;
                    }

                    break;
            }

        }
        /// <summary>
        /// 檢查像機是否連接
        /// </summary>
        public void Tick()
        {
            CheckEPIXTrigger();
            CheckConnection();
        }

        public void CheckEPIXTrigger()
        {
            int i = 0;

            while (i < LastCapturedCount.Length)
            {
                int countnow = pxd_capturedFieldCount(1 << i);

                if (LastCapturedCount[i] != countnow && FrameRateArray[i] == -1)
                {
                    OnTrigger(CCDRelateIndexArray[i].ToString() + ",EPIX");
                    LastCapturedCount[i] = countnow;
                }

                i++;
            }
        }

        #region From JzTool Functions
        void GetBMP(string bmpfilestring, ref Bitmap orgbmp)
        {
            Bitmap bmptmp = null;
            //FreeImageAPI.FreeImageBitmap bmptmp = null;
            if (!File.Exists(bmpfilestring))
            {
                string filename = System.IO.Path.GetFileName(bmpfilestring);//文件名 “P00-003.png”
                filename = SystemWORKPATH + "\\" + filename;

                bmptmp = new Bitmap(filename);
                //bmptmp = new FreeImageAPI.FreeImageBitmap(filename);
            }
            else
            {
                bmptmp = new Bitmap(bmpfilestring);
                //bmptmp = new FreeImageAPI.FreeImageBitmap(bmpfilestring);
            }
            orgbmp.Dispose();
            orgbmp = new Bitmap(bmptmp);
            //orgbmp = (Bitmap)bmptmp.ToBitmap().Clone();
            bmptmp.Dispose();
        }

#if ICAM
        /// <summary>
        /// 初始化ICAM相机
        /// </summary>
        /// <param name="pathstr">相机工作目录</param>
        /// <param name="bmplist">预存图像集合</param>
        /// <returns></returns>
        bool InitialICAM(string pathstr)
        {
            bool isOK = false;
            // 获取视频设备的集合
            DsDevice[] capDevices = DsDevice.GetDevicesOfCat(DirectShowLib.FilterCategory.VideoInputDevice);
            /// <summary>
            /// 图形生成器界面。
            /// </summary>
            IFilterGraph2 m_FilterGraph = new FilterGraph() as IFilterGraph2;
            IBaseFilter captureDeviceFilter = null;

            videoProcAmp = new IAMVideoProcAmp[capDevices.Length];
            for (int i = 0; i < capDevices.Length; i++)
            {
                int hr2 = m_FilterGraph.AddSourceFilterForMoniker(capDevices[i].Mon, null, capDevices[i].Name, out captureDeviceFilter);

                videoProcAmp[i] = captureDeviceFilter as IAMVideoProcAmp;
            }


            CameraChoice CameraChoice = new CameraChoice();
            CameraChoice.UpdateDeviceList();
            if (CCDRelateIndexArray.Length > CameraChoice.Devices.Count)
            {
                MessageBox.Show("没有那么多ICAM相机！");
                return isOK;
            }
            CamerList = new Camera[CCDRelateIndexArray.Length];

            string CCDSEQStr = pathstr + "\\ICAMMESS.INI";
            string strCCDData = "";
            if (!File.Exists(CCDSEQStr))
            {
                for (int i = 0; i < CameraChoice.Devices.Count; i++)
                {

                    string ID = CameraChoice.Devices[i].ClassID.ToString();

                    string[] strs = CameraChoice.Devices[i].DevicePath.Split('#');

                    ID = strs[1];

                    ResolutionList resolutions = Camera.GetResolutionList(CameraChoice.Devices[i].Mon);
                    string Reso = "";

                    foreach (Resolution reso in resolutions)
                        Reso += reso.ToString() + ";";

                    strCCDData += i + ":" + ID + ":" + Reso + Environment.NewLine;
                }
                SaveData(strCCDData, CCDSEQStr);
            }
            else
                ReadData(ref strCCDData, CCDSEQStr);

            List<string> listCamerID = new List<string>();

            strCCDData = strCCDData.Replace(Environment.NewLine, "$");
            string[] IDdata = strCCDData.Split('$');
            int iCount = 0;
            //foreach (string id in IDdata)
            {
                //if (id == "")
                //    continue;

                //string[] myid = id.Split(':');
                //if (myid.Length > 1)
                {
                    //string ID = myid[1];

                    foreach (int ccdrelateindex in CCDRelateIndexArray)
                    {
                        int index = Array.IndexOf(CCDRelateIndexArray, ccdrelateindex);

                        //if (index.ToString() == myid[0])
                        {
                            //foreach (DsDevice ds in CameraChoice.Devices)
                            {
                                DsDevice ds = CameraChoice.Devices[index];

                                string[] strs = ds.DevicePath.Split('#');
                                string IDNow = strs[1];
                                //if (IDNow == ID)
                                {
                                    CamerList[index] = new Camera();
                                    CamerList[index].Initialize(new Label(), ds.Mon);

                                    //foreach (string strs in CamSizeArray)
                                    //{
                                    //设定CCD分辩率
                                    //string[] str = SizeDefArray[ccdrelateindex].Split(',');
                                    //int relateindex = int.Parse(str[0]);
                                    //Size size = new Size(int.Parse(str[1]), int.Parse(str[2]));
                                    SetResolution(index, SizeDefArray[index].OrgSize);
                                    //}

                                    CamerList[index].BuildGraph();
                                    CamerList[index].RunGraph();
                                    iCount++;
                                }
                            }
                        }
                    }

                }
                //else
                //    continue;
            }
            if (iCount == CCDRelateIndexArray.Length)
                isOK = true;
            else
                isOK = false;

            return isOK;

        }
        /// <summary>
        /// 获取CCD属性
        /// </summary>
        /// <param name="relateindex">CCD编号</param>
        /// <param name="Property">属性</param>
        /// <param name="max">属性的最大值</param>
        /// <param name="min">属性最小值</param>
        /// <param name="Value">属性当前值</param>
        /// <param name="defaultValue">属性默认值</param>
        /// <returns></returns>
        public int GetRangeProcAmpProperty(int relateindex, CCDProcAmpProperty property, ref int max, ref int min, ref int Value, ref int defaultValue)
        {
            VideoProcAmpProperty Property = PropertyChange(property);

            int iResult = 0;
            int index = Array.IndexOf(CCDRelateIndexArray, relateindex);
            switch (CCDType)
            {
                case CCDTYPEEnum.ICAM:

                    //DsDevice[] capDevices;
                    //// 获取视频设备的集合
                    //capDevices = DsDevice.GetDevicesOfCat(DirectShowLib.FilterCategory.VideoInputDevice);

                    ///// <summary>
                    ///// 图形生成器界面。
                    ///// </summary>
                    //IFilterGraph2 m_FilterGraph = new FilterGraph() as IFilterGraph2;
                    //IBaseFilter captureDeviceFilter = null;
                    //int hr2 = m_FilterGraph.AddSourceFilterForMoniker(capDevices[index].Mon, null, capDevices[index].Name, out captureDeviceFilter);

                    //IAMVideoProcAmp videoProcAmp = captureDeviceFilter as IAMVideoProcAmp;
                    VideoProcAmpFlags flags = VideoProcAmpFlags.Manual;
                    if (videoProcAmp[index] == null)
                    {
                        iResult = -1;
                        return iResult;
                    }
                    int hr = videoProcAmp[index].GetRange(Property, out min, out max, out Value, out defaultValue, out flags);
                    iResult = hr;
                    break;
            }
            return iResult;
        }

        IAMVideoProcAmp[] videoProcAmp;
        /// <summary>
        /// 相机属性设定
        /// </summary>
        /// <param name="relateindex">相机编号</param>
        /// <param name="property">相机属性</param>
        /// <param name="Value">值(0~255)</param>
        /// <returns></returns>
        public int SetProperty(int relateindex, CCDProcAmpProperty property, int Value)
        {
            VideoProcAmpProperty Property = PropertyChange(property);
            int iResult = 0;
            int index = Array.IndexOf(CCDRelateIndexArray, relateindex);
            switch (CCDType)
            {
                case CCDTYPEEnum.ICAM:

                    ///// <summary>
                    ///// 图形生成器界面。
                    ///// </summary>
                    //IFilterGraph2 m_FilterGraph = new FilterGraph() as IFilterGraph2;
                    //IBaseFilter captureDeviceFilter = null;
                    //int hr2 = m_FilterGraph.AddSourceFilterForMoniker(capDevices[index].Mon, null, capDevices[index].Name, out captureDeviceFilter);

                    //IAMVideoProcAmp videoProcAmp = captureDeviceFilter as IAMVideoProcAmp;
                    VideoProcAmpFlags flags = VideoProcAmpFlags.Manual;
                    if (videoProcAmp[index] == null)
                    {
                        iResult = -1;
                        return iResult;
                    }
                    // int val, min, max, step, defaultValue;

                    // 设置亮度
                    if (Value != -1)
                    {
                        // int hr = videoProcAmp.GetRange(Property, out min, out max, out step, out defaultValue, out flags);
                        //if (0 == hr)
                        //{
                        //videoProcAmp.Get(property, out val, out flags);
                        // val = min + (max - min) * Value / 255;
                        iResult = videoProcAmp[index].Set(Property, Value, flags);
                        //}
                    }
                    break;
            }
            return iResult;
        }

        public void SetProperty(IntPtr hwndOwner, int relateindex)
        {
            int index = Array.IndexOf(CCDRelateIndexArray, relateindex);
            //   CamerList[index].DisplayPropertyPage_SourcePinOutput(hwndOwner);

            Camera.DisplayPropertyPage_Device(CamerList[index].Moniker, hwndOwner);
            //_Camera.DisplayPropertyPage_SourcePinOutput(hwndOwner);
        }
        /// <summary>
        /// 属性转变
        /// </summary>
        /// <returns></returns>
        public VideoProcAmpProperty PropertyChange(CCDProcAmpProperty cCDProcAmpProperty)
        {
            switch (cCDProcAmpProperty)
            {
                case CCDProcAmpProperty.BacklightCompensation:
                    return VideoProcAmpProperty.BacklightCompensation;
                case CCDProcAmpProperty.Brightness:
                    return VideoProcAmpProperty.Brightness;
                case CCDProcAmpProperty.ColorEnable:
                    return VideoProcAmpProperty.ColorEnable;
                case CCDProcAmpProperty.Contrast:
                    return VideoProcAmpProperty.Contrast;
                case CCDProcAmpProperty.Gain:
                    return VideoProcAmpProperty.Gain;
                case CCDProcAmpProperty.Gamma:
                    return VideoProcAmpProperty.Gamma;
                case CCDProcAmpProperty.Hue:
                    return VideoProcAmpProperty.Hue;
                case CCDProcAmpProperty.Saturation:
                    return VideoProcAmpProperty.Saturation;
                case CCDProcAmpProperty.Sharpness:
                    return VideoProcAmpProperty.Sharpness;
                case CCDProcAmpProperty.WhiteBalance:
                    return VideoProcAmpProperty.WhiteBalance;

            }
            return VideoProcAmpProperty.Brightness;

        }
        /// <summary>
        /// 设定分辩率
        /// </summary>
        /// <param name="relateindex">相机编号</param>
        /// <param name="bmpSize">分辩率</param>
        /// <returns>是否设定成功</returns>
        public bool SetResolution(int index, Size bmpSize)
        {
            bool isok = false;
            //int index = Array.IndexOf(CCDRelateIndexArray, relateindex);

            switch (CCDType)
            {
#if ICAM
                case CCDTYPEEnum.ICAM:

                    ResolutionList resolutions = Camera.GetResolutionList(CamerList[index].Moniker);
                    foreach (Resolution reso in resolutions)
                    {
                        if (reso.Width == bmpSize.Width && reso.Height == bmpSize.Height)
                        {
                            CamerList[index].Resolution = reso;
                            isok = true;
                            break;
                        }
                    }

                    break;
#endif

            }

            return isok;
        }
#endif
        void SaveData(string DataStr, string FileName)
        {
            File.WriteAllText(FileName, DataStr, Encoding.Default);
        }
        void ReadData(ref string DataStr, string FileName)
        {
            FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.None);
            StreamReader Srr = new StreamReader(fs, Encoding.Default);

            DataStr = Srr.ReadToEnd();

            Srr.Close();
            Srr.Dispose();
        }

        void RenderBMP(Bitmap bmpfrom, Bitmap bmpto)
        {
            Graphics g = Graphics.FromImage(bmpto);
            g.DrawImage(bmpfrom, new Point(0, 0));
            g.Dispose();
        }
        void RenderBMPSizeChange(Bitmap bmpfrom, Bitmap bmpto)
        {
            Graphics g = Graphics.FromImage(bmpto);
            g.DrawImage(bmpfrom, new Rectangle(0, 0, bmpto.Width, bmpto.Height), new Rectangle(0, 0, bmpfrom.Width, bmpfrom.Height), GraphicsUnit.Pixel);
            g.Dispose();
        }

        SizeDefClass[] GetSizeDef(string Str)
        {
            int i = 0;
            SizeDefClass[] retsizedef;

            string[] strs = Str.Split(',');

            retsizedef = new SizeDefClass[strs.Length];

            foreach (string str in strs)
            {
                retsizedef[i] = new SizeDefClass(str);
                i++;
            }
            return retsizedef;
        }
        #endregion

        //當有Input Trigger時，產生OnTrigger
        public delegate void TriggerHandler(string operationstr);
        public event TriggerHandler TriggerAction;
        public void OnTrigger(String operationstr)
        {
            if (TriggerAction != null)
            {
                TriggerAction(operationstr);
            }
        }

    }
}
