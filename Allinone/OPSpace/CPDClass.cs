using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

using MoveGraphLibrary;
using WorldOfMoveableObjects;
using JetEazy;
using JetEazy.BasicSpace;
using JzDisplay;
using Allinone.OPSpace.CPDSpace;
using JetEazy.ControlSpace;
using JzASN.OPSpace;

namespace Allinone.OPSpace
{
    public class CPDCollectionClass
    {
        

    }
    public class CPDClass
    {
        VersionEnum VERSION;
        OptionEnum OPTION;

        public Bitmap bmpVIEW = new Bitmap(1, 1);
        public Bitmap bmpRUNVIEW = new Bitmap(1, 1);
        public Bitmap bmpOCRCheckErr = new Bitmap(1, 1);

        public GAPResult mGapResult = new GAPResult();


        public JzRectEAG RangeRectEAG = new JzRectEAG(Color.FromArgb(10, Color.Lime), new RectangleF(10, 10, 200, 200));
        public Size BaseSize = new Size(1280, 800);
        public List<CPDItemClass> CPDItemList = new List<CPDItemClass>();

        public CPDClass(VersionEnum version,OptionEnum option)
        {
            VERSION = version;
            OPTION = option;
        }

        public CPDClass(string str,VersionEnum version,OptionEnum option)
        {
            VERSION = version;
            OPTION = option;

            FromString(str);
        }

        public CPDClass Clone()
        {
            CPDClass cpd = new CPDClass(this.ToString(), VERSION, OPTION);

            //int i = 0;

            //while(i < this.CPDItemList.Count)
            //{
            //    cpd.CPDItemList[i].SetBMP(this.CPDItemList[i].bmpITEM);
            //    i++;
            //}
            return cpd;
        }
        public void Dupe(CPDClass cpd)
        {
            this.FromString(cpd.ToString());

            this.bmpVIEW.Dispose();
            this.bmpVIEW = new Bitmap(cpd.bmpVIEW);

            //int i = 0;
            //while (i < this.CPDItemList.Count)
            //{
            //    CPDItemList[i].SetBMP(cpd.CPDItemList[i].bmpITEM);
            //    i++;
            //}
        }
        public void LoadBMP(string bmppath)
        {
            GetBMP(bmppath + "\\VIEW" + Universal.GlobalImageTypeString, ref bmpVIEW);
        }
        public void LoadCPDItem(string bmppath)
        {
            foreach(CPDItemClass cpditem in CPDItemList)
            {
                cpditem.LoadBMP(bmppath);
            }
        }
        public void SaveBMP(string bmppath)
        {
            SaveBMP(bmppath + "\\VIEW" + Universal.GlobalImageTypeString, ref bmpVIEW, Universal.GlobalImageFormat);
        }
        public void SaveCPDItem(string bmppath)
        {
            if (!Directory.Exists(bmppath + "\\CPD\\"))
                Directory.CreateDirectory(bmppath + "\\CPD\\");
            else
            {
                string [] files = Directory.GetFiles(bmppath + "\\CPD\\");

                foreach(string file in files)
                {
                    File.Delete(file);
                }
            }

            foreach (CPDItemClass cpditem in CPDItemList)
            {
                cpditem.SaveBMP(bmppath);
            }
        }
        public override string ToString()
        {
            string str = "";

            str += RangeRectEAG.ToString() + Environment.NewLine;
            str += SizeFtoString(BaseSize) + Environment.NewLine;

            foreach(CPDItemClass citem in CPDItemList)
            {
                str += citem.ToString() + Environment.NewLine;
            }

            if (str != "")
                str = str.Remove(str.Length - 2, 2);

            return str;
        }
        public void FromString(string str)
        {
            int i = 0;

            char Seperator = Universal.NewlineChar;

            string[] strs = str.Replace(Environment.NewLine, Seperator.ToString()).Split(Seperator);

            foreach (CPDItemClass citem in CPDItemList)
            {
                citem.Suicide();
            }
            CPDItemList.Clear();

            i = 0;
            foreach (string strx in strs)
            {
                switch(i)
                {
                    case 0:
                        RangeRectEAG.FromString(strx);
                        RangeRectEAG.Color = Color.FromArgb(10, Color.Lime);

                        RangeRectEAG.RelateNo = -1;
                        RangeRectEAG.RelateLevel = -1;
                        RangeRectEAG.RelatePosition = -1;

                        break;
                    case 1:
                        BaseSize = StringToSize(strx);
                        break;
                    default:
                        CPDItemClass citem = new CPDItemClass(strx,VERSION,OPTION);
                        CPDItemList.Add(citem);
                        break;
                }

                i++;
            }
        }
        public void GetBMP(string bmpfilestr, ref Bitmap bmp)
        {
            Bitmap bmptmp = new Bitmap(bmpfilestr);

            bmp.Dispose();
            bmp = new Bitmap(bmptmp);

            bmptmp.Dispose();
        }
        public void SaveBMP(string bmpfilestr, ref Bitmap bmp, ImageFormat imgformat)
        {
            Bitmap bmptmp = new Bitmap(bmp);

            bmptmp.Save(bmpfilestr, imgformat);

            bmptmp.Dispose();
        }
        string SizeFtoString(Size size)
        {
            return size.Width.ToString() + "," + size.Height.ToString();
        }
        Size StringToSize(string str)
        {
            int[] sizevalue = Array.ConvertAll(str.Split(','), int.Parse);

            return new Size(sizevalue[0], sizevalue[1]);
        }


        public void CollectRUNVIEWData(AlbumClass albumwork, int runenvno)
        {
            JzToolsClass jzTools=new JzToolsClass();
            foreach(CPDItemClass cpditem in CPDItemList)
            {
                if(cpditem.NORMALPara.RelatePA.IndexOf("ENV") > -1)
                {
                    string[] strs = cpditem.NORMALPara.RelatePA.Split('-');

                    int envno = int.Parse(strs[0].Replace("ENV", ""));
                    int pageno = int.Parse(strs[1].Replace("PAGE", ""));
                    int pageopindex = int.Parse(strs[2].Replace("P",""));

                    if (runenvno == envno)
                    {
                        EnvClass env = albumwork.GetEnv(envno);
                        PageClass page = env.GetPageRun(pageno);    //由於有些 env 裏的 page 是經由80000組合起來的，因此需要由PageRunNo來指定Page 編號

                        if (page == null)
                            break;
                        //cpditem.bmpITEMRUN = new Bitmap(page.GetbmpRUN((PageOPTypeEnum)pageopindex));
                        cpditem.bmpITEMRUN = (Bitmap)page.GetbmpRUN((PageOPTypeEnum)pageopindex).Clone();
                        //try
                        //{
                        //    cpditem.bmpITEMRUN = new Bitmap(page.GetbmpRUN((PageOPTypeEnum)pageopindex));
                        //}
                        //catch
                        //{
                        //    try
                        //    {
                        //        cpditem.bmpITEMRUN = new Bitmap(page.GetbmpORG((PageOPTypeEnum)pageopindex));
                        //    }
                        //    catch
                        //    {

                        //    }
                        //}

                        switch (VERSION)
                        {
                            case VersionEnum.ALLINONE:

                                switch(OPTION)
                                {
                                    case OptionEnum.MAIN_X6:
                                    case JetEazy.OptionEnum.MAIN_SERVICE:

                                        //jzTools.SetBrightContrast(cpditem.bmpITEMRUN, jzTools.SimpleRect(cpditem.bmpITEMRUN.Size), 50, 100);
                                        //jzTools.SetBrightContrast(cpditem.bmpITEMRUN, jzTools.SimpleRect(cpditem.bmpITEMRUN.Size), cpditem.NORMALPara.Brightness, cpditem.NORMALPara.Contrast);


                                        break;
                                }

                                break;
                        }

                        //PointF biaslocationF = cpditem.RatioRectEAG.GetRectF.Location;

                        //biaslocationF.X = RangeRectEAG.GetRectF.X - biaslocationF.X;
                        //biaslocationF.Y = RangeRectEAG.GetRectF.Y - biaslocationF.Y;

                        PointF biaslocationF = cpditem.RatioRectEAG.GetRectF.Location;

                        biaslocationF.X = biaslocationF.X - RangeRectEAG.GetRectF.X;
                        biaslocationF.Y = biaslocationF.Y - RangeRectEAG.GetRectF.Y;

                        cpditem.BiasLocation = biaslocationF;
                    }
                }
            }
        }
        public void CollectRUNVIEWData(AlbumClass albumwork, int runenvno,Bitmap bmpInput)
        {
            JzToolsClass jzTools = new JzToolsClass();
            foreach (CPDItemClass cpditem in CPDItemList)
            {
                if (cpditem.NORMALPara.RelatePA.IndexOf("ENV") > -1)
                {
                    string[] strs = cpditem.NORMALPara.RelatePA.Split('-');

                    int envno = int.Parse(strs[0].Replace("ENV", ""));
                    int pageno = int.Parse(strs[1].Replace("PAGE", ""));
                    int pageopindex = int.Parse(strs[2].Replace("P", ""));

                    if (runenvno == envno)
                    {
                        EnvClass env = albumwork.GetEnv(envno);
                        PageClass page = env.GetPageRun(pageno);    //由於有些 env 裏的 page 是經由80000組合起來的，因此需要由PageRunNo來指定Page 編號

                        if (page == null)
                            break;
                        cpditem.bmpITEMRUN = new Bitmap(bmpInput);
                        //try
                        //{
                        //    cpditem.bmpITEMRUN = new Bitmap(bmpInput);
                        //}
                        //catch
                        //{
                        //    try
                        //    {
                        //        cpditem.bmpITEMRUN = new Bitmap(page.GetbmpORG((PageOPTypeEnum)pageopindex));
                        //    }
                        //    catch
                        //    {

                        //    }
                        //}

                        switch (VERSION)
                        {
                            case VersionEnum.ALLINONE:

                                switch (OPTION)
                                {
                                    case OptionEnum.MAIN_X6:
                                    case JetEazy.OptionEnum.MAIN_SERVICE:

                                        //jzTools.SetBrightContrast(cpditem.bmpITEMRUN, jzTools.SimpleRect(cpditem.bmpITEMRUN.Size), 50, 100);
                                        //jzTools.SetBrightContrast(cpditem.bmpITEMRUN, jzTools.SimpleRect(cpditem.bmpITEMRUN.Size), cpditem.NORMALPara.Brightness, cpditem.NORMALPara.Contrast);


                                        break;
                                }

                                break;
                        }

                        //PointF biaslocationF = cpditem.RatioRectEAG.GetRectF.Location;

                        //biaslocationF.X = RangeRectEAG.GetRectF.X - biaslocationF.X;
                        //biaslocationF.Y = RangeRectEAG.GetRectF.Y - biaslocationF.Y;

                        PointF biaslocationF = cpditem.RatioRectEAG.GetRectF.Location;

                        biaslocationF.X = biaslocationF.X - RangeRectEAG.GetRectF.X;
                        biaslocationF.Y = biaslocationF.Y - RangeRectEAG.GetRectF.Y;

                        cpditem.BiasLocation = biaslocationF;
                    }
                }
            }
        }
        public void GenRUNVIEWData(ASNCollectionClass asncollection)
        {
            bmpRUNVIEW.Dispose();
            bmpRUNVIEW = new Bitmap(bmpVIEW);
            Bitmap myBitbmp = new Bitmap(1, 1);
            JzToolsClass jztools = new JzToolsClass();
            Graphics g = Graphics.FromImage(bmpRUNVIEW);

            foreach (CPDItemClass cpditem in CPDItemList)
            {
                if (cpditem.NORMALPara.RelatePA.IndexOf("ENV") > -1)
                {
                    RectangleF rectf = cpditem.RatioRectEAG.GetRectF;

                    rectf.X = cpditem.BiasLocation.X;
                    rectf.Y = cpditem.BiasLocation.Y;
                    if (cpditem.bmpITEMRUN != null)
                    {
                        switch (OPTION)
                        {
                            case JetEazy.OptionEnum.MAIN_SERVICE:
                            case OptionEnum.MAIN_X6:
                            case OptionEnum.MAIN_SDM1:
                            case OptionEnum.MAIN_SDM2:
                            case OptionEnum.MAIN_SDM3:
                                Rectangle myCloneRectbase = jztools.SimpleRect(cpditem.bmpITEMRUN.Size);
                                Rectangle myCloneRect = new Rectangle(myCloneRectbase.X + cpditem.NORMALPara.iLeft,
                                                                                                     myCloneRectbase.Y + cpditem.NORMALPara.iTop,
                                                                                                     myCloneRectbase.Width - cpditem.NORMALPara.iLeft - cpditem.NORMALPara.iRight,
                                                                                                     myCloneRectbase.Height - cpditem.NORMALPara.iTop - cpditem.NORMALPara.iBottom);

                                jztools.BoundRect(ref myCloneRect, cpditem.bmpITEMRUN.Size);

                                if (myCloneRect.Width > 100 && myCloneRect.Height > 100)
                                {
                                    myBitbmp.Dispose();
                                    myBitbmp = cpditem.bmpITEMRUN.Clone(myCloneRect, cpditem.bmpITEM.PixelFormat);
                                    cpditem.RenewSize(myBitbmp.Size);
                                    g.DrawImage(myBitbmp, rectf);
                                }
                                else
                                {
                                    g.DrawImage(cpditem.bmpITEMRUN, rectf);
                                }

                                break;
                            default:
                                g.DrawImage(cpditem.bmpITEMRUN, rectf);
                                break;
                        }
                    }
                        
                }
                else
                {
                    string[] strs = cpditem.NORMALPara.RelatePA.Split(':');

                    int asnno = int.Parse(strs[0]);

                    ASNClass asn = asncollection.GetASN(asnno);

                    //if (strs[1] == "KHC" || strs[1] == "KGM")
                    if (strs[1].IndexOf("KHC") == 0 || strs[1].IndexOf("KGM") == 0)
                    {
                        //跳过更换图片。Gaara
                    }
                    else
                        cpditem.bmpITEMRUN = asn.bmpASN;

                    PointF biaslocationF = cpditem.RatioRectEAG.GetRectF.Location;
                    
                    biaslocationF.X = biaslocationF.X - RangeRectEAG.GetRectF.X;
                    biaslocationF.Y = biaslocationF.Y - RangeRectEAG.GetRectF.Y;

                    cpditem.BiasLocation = biaslocationF;

                    RectangleF rectf = cpditem.RatioRectEAG.GetRectF;

                    rectf.X = cpditem.BiasLocation.X;
                    rectf.Y = cpditem.BiasLocation.Y;

                    if (cpditem.bmpITEMRUN != null)
                        g.DrawImage(cpditem.bmpITEMRUN, rectf);
                    
                }
            }
            g.Dispose();
            
        }


        public void Suicide()
        {
            bmpVIEW.Dispose();
            bmpRUNVIEW.Dispose();

            foreach (CPDItemClass citem in CPDItemList)
            {
                citem.Suicide();
            }
            CPDItemList.Clear();
        }

    }
    public class CPDItemClass
    {
        char SeperateChar = Universal.SeperateCharB;

        public static string ORGCPDNOSTRING = "0000";

        VersionEnum VERSION;
        OptionEnum OPTION;

        public int ParentNo = 0;
        public int No = 0;
        public string Name
        {
            get
            {
                return NORMALPara.Name;
            }
            set
            {
                NORMALPara.Name = value;
            }
        }

        public Bitmap bmpITEM = new Bitmap(1, 1);
        
        public NORMALClass NORMALPara = new NORMALClass();
        public JzRectEAG RatioRectEAG = new JzRectEAG(Color.FromArgb(10, Color.Red), new RectangleF(10, 10, 200, 100));

        public ASSEMBLEClass ASSEMBLE = new ASSEMBLEClass();

        #region RUN Data

        public Bitmap bmpITEMRUN;
        public PointF BiasLocation = new PointF();

        #endregion

        public bool IsSelected
        {
            get
            {   
                return RatioRectEAG.IsSelected;
            }
        }

        //public CPDItemClass()
        //{

        //}

        //public CPDItemClass(string str)
        //{
        //    FromString(str);
        //}
        public CPDItemClass(VersionEnum version, OptionEnum option, SizeF sizef)
        {
            VERSION = version;
            OPTION = option;

            ASSEMBLE.ConstructProperty(VERSION, OPTION);

            No = 1;

            RatioRectEAG = new JzRectEAG(Color.FromArgb(20, Color.Red), new RectangleF(10, 10, sizef.Width / 4f, sizef.Height / 4f));
            RatioRectEAG.RelateNo = No;
            RatioRectEAG.RelatePosition = 0;
            RatioRectEAG.RelateLevel = 2;

            RatioRectEAG.Rotatable = false;
            RatioRectEAG.Resizable = false;

            NORMALPara.Ratio = 0.25f;
        }
        public CPDItemClass(VersionEnum version, OptionEnum option,SizeF sizef,PointF ptf,int no)
        {
            VERSION = version;
            OPTION = option;

            ASSEMBLE.ConstructProperty(VERSION, OPTION);

            No = no;

            RatioRectEAG = new JzRectEAG(Color.FromArgb(20, Color.Red), new RectangleF(ptf.X, ptf.Y, sizef.Width / 4f, sizef.Height / 4f));
            RatioRectEAG.RelateNo = No;
            RatioRectEAG.RelatePosition = 0;
            RatioRectEAG.RelateLevel = 2;

            RatioRectEAG.Rotatable = false;
            RatioRectEAG.Resizable = false;

            NORMALPara.Ratio = 0.25f;

            //myMover.Add(jzrect);

        }
        public CPDItemClass(string str, VersionEnum version, OptionEnum option)
        {
            VERSION = version;
            OPTION = option;

            FromString(str);
            ASSEMBLE.ConstructProperty(VERSION, OPTION);
        }
        public CPDItemClass Clone()
        {
            CPDItemClass asn = new CPDItemClass(this.ToString(), VERSION, OPTION);

            return asn;
        }
        public CPDItemClass Clone(Point offsetpoint)
        {
            CPDItemClass asn = new CPDItemClass(this.ToString(), VERSION, OPTION);

            asn.SetMoverOffset(offsetpoint);

            this.SetMoverSelected(false);

            return asn;
        }
        public void AddNewRowToDataTable(DataTable datatable)
        {
            DataRow newdatarow = datatable.NewRow();

            newdatarow["ParentNo"] = this.ParentNo;
            newdatarow["No"] = this.No;
            newdatarow["Name"] = this.ToCpdItemString();

            datatable.Rows.Add(newdatarow);
        }
        public void InsertDeleteData(List<int> deletenolist, List<CPDItemClass> deletecpdlist)
        {
            if (deletenolist.IndexOf(this.No) < 0)
            {
                deletenolist.Add(this.No);
                deletecpdlist.Add(this);
            }
        }
        void SetMoverOffset(Point offsetpoint)
        {
            RatioRectEAG.SetOffset(offsetpoint);
        }
        public void SetMoverSelected(bool isselect)
        {
            SetMoverSelected(false, isselect);
        }
        public void SetMoverSelected(bool isselectfirst, bool isselect)
        {
            RatioRectEAG.IsFirstSelected = isselectfirst;
            RatioRectEAG.IsSelected = isselect;
        }
        public void RelateMover(int relateno, int relatelevel)
        {
            RatioRectEAG.RelateNo = relateno;
            RatioRectEAG.RelatePosition = 0;
            RatioRectEAG.RelateLevel = relatelevel;

        }
        public void SetBMP(Bitmap bmp)
        {
            bmpITEM.Dispose();
            bmpITEM = new Bitmap(bmp);

        }
        public void LoadBMP(string bmppath)
        {
            string bmppathx = bmppath + "\\CPD\\" + No.ToString("000") + Universal.GlobalImageTypeString;

            GetBMP(bmppathx, ref bmpITEM);
        }
        public void SaveBMP(string bmppath)
        {


            string bmppathx = bmppath + "\\CPD\\" + No.ToString("000") + Universal.GlobalImageTypeString;

            SaveBMP(bmppathx, ref bmpITEM, Universal.GlobalImageFormat);
            bmpITEM.Dispose();
        }
        public override string ToString()
        {
            char seperator = Universal.SeperateCharA;
            string str = "";

            str += ParentNo.ToString() + seperator;
            str += No.ToString() + seperator;
            str += NORMALPara.ToString() + seperator;
            str += RatioRectEAG.ToString() + seperator;
            str += "";

            return str;
        }
        void FromString(string str)
        {
            char seperator = Universal.SeperateCharA;
            string[] strs = str.Split(seperator);

            ParentNo = int.Parse(strs[0]);
            No = int.Parse(strs[1]);

            NORMALPara.FromString(strs[2]);
            RatioRectEAG.FromString(strs[3]);

            RatioRectEAG.RelateNo = No;
            RatioRectEAG.RelatePosition = 0;
            RatioRectEAG.RelateLevel = 2;

            RatioRectEAG.Rotatable = false;
            RatioRectEAG.Resizable = false;
        }
        public void FromAssembleProperty()
        {
            ASSEMBLE.SetNormal(NORMALPara);
        }
        public void FromAssembleProperty(string changeitemstring, string valuestring)
        {
            if (IsSelected)
            {
                if (changeitemstring == "RESET")    //RESET To Default Value
                {
                    NORMALPara.Reset();
                }
                else
                {
                    NORMALPara.FromPropertyChange(changeitemstring, valuestring);

                    //switch(changeitemstring)
                    //{
                    //    case "Ratio":
                    if (changeitemstring.IndexOf("Ratio") > -1)
                        RatioRectEAG.SetRectRatio(NORMALPara.OrgSizeF, NORMALPara.Ratio);

                    //        break;
                    //}
                }
            }
        }
        public void ToAssembleProperty()
        {   
            ASSEMBLE.GetNormal(NORMALPara);
        }
        string SizeFtoString(SizeF sizef)
        {
            return sizef.Width.ToString() + "," + sizef.Height.ToString();
        }
        SizeF StringToSizeF(string str)
        {
            float[] sizevalue = Array.ConvertAll(str.Split(','), float.Parse);

            return new SizeF(sizevalue[0], sizevalue[1]);
        }
        public void GetBMP(string bmpfilestr, ref Bitmap bmp)
        {
            Bitmap bmptmp = new Bitmap(bmpfilestr);

            bmp.Dispose();
            bmp = new Bitmap(bmptmp);

            bmptmp.Dispose();
        }
        public void SaveBMP(string bmpfilestr, ref Bitmap bmp, ImageFormat imgformat)
        {
            Bitmap bmptmp = new Bitmap(bmp);

            bmptmp.Save(bmpfilestr, imgformat);

            bmptmp.Dispose();
        }
        public string ToCpdItemString()
        {
            string anz = "CPD" + "-" + No.ToString(ORGCPDNOSTRING);

            return anz;
        }

        public void RenewSize(SizeF sizef)
        {
            RectangleF rectf = RatioRectEAG.GetRectF;
            rectf.Size = new SizeF(sizef.Width * NORMALPara.Ratio ,sizef.Height * NORMALPara.Ratio);
            RatioRectEAG.FromRectangleF(rectf);

            NORMALPara.OrgSizeF = sizef;
        }

        public void Suicide()
        {
            bmpITEM.Dispose();
        }
    }

    /// <summary>
    /// 测镭雕偏移
    /// </summary>
    public class GAPResult
    {
        public bool ISANGLE { get; set; }
        public string STRANGLE { get; set; }

        public bool ISRange { get; set; }
        public string STRRange { get; set; }

        public bool ISRangeLR { get; set; }
        public string STRRangeLR { get; set; }


        public bool isA{get;set;}
        public string strA { get; set; }
        public bool isB { get; set; }
        public string strB { get; set; }
        public bool isC { get; set; }
        public string strC { get; set; }
        public bool isD { get; set; }
        public string strD { get; set; }
        public bool isE { get; set; }
        public string strE { get; set; }
        public bool isF { get; set; }
        public string strF { get; set; }
        public bool isG { get; set; }
        public string strG { get; set; }
        public bool isH { get; set; }
        public string strH { get; set; }

    }
         
}
