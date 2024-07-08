using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Design;
using System.Drawing;
using System.Drawing.Imaging;
using System.ComponentModel;
using JetEazy.BasicSpace;
using JetEazy.UISpace;
using JetEazy;
using AUVision;

using MoveGraphLibrary;
using WorldOfMoveableObjects;

namespace Allinone.OPSpace.AnalyzeSpace
{
    public class CornerClass : ICustomClass
    {
        PropertyList publicproperties = new PropertyList();
        //ICustomClass implementation
        public PropertyList PublicProperties
        {
            get
            {
                return publicproperties;
            }
            set
            {
                publicproperties = value;
            }
        }
        
        int _Width = 20;
        int _Height = 20;
        float _Tolerance = 0f;
        
        [DefaultValue(20)]
        [Description("Width \rRange 0 <-> 1000, default 40")]
        [DisplayName("Width")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 1000)]
        public int Width
        {
            get
            {
                return _Width;
            }
            set
            {
                _Width = value;
            }
        }

        [DefaultValue(20)]
        [Description("Height \rRange 0 <-> 1000, default 40")]
        [DisplayName("Height")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 1000)]
        public int Height
        {
            get
            {
                return _Height;
            }
            set
            {
                _Height = value;
            }
        }

        [DefaultValue(0)]
        [Description("Similar Ratio.\rRange 0 <-> 1.,  default 0.\rDisable is 0.")]
        [DisplayName("Sim.Ratio")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 1f, 0.1f, 2)]
        public float Tolerance
        {
            get
            {
                return _Tolerance;
            }
            set
            {
                _Tolerance = value;
            }
        }

        [DefaultValue(0)]
        [Description("0 is No Need.\rRange -200 <-> 200, default 0")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-200, 200)]
        public int Brightness
        {
            get; set;
        }
        [DefaultValue(0)]
        [Description("0 is No Need.\rRange -200 <-> 200, default 0")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-200, 200)]
        public int Contrast
        {
            get; set;
        }

        //Bitmap bmpPattern = new Bitmap(1, 1);
        //Bitmap bmpOutput = new Bitmap(1, 1);

        //PositionEnum myPosition = PositionEnum.LeftTop;
        JetEazy.CornerEnum Corner = JetEazy.CornerEnum.LT;

        ALIGNClass AlignPara = new ALIGNClass();


        public WorkStatusCollectionClass TrainStatusCollection = new WorkStatusCollectionClass();
        public WorkStatusCollectionClass RunStatusCollection = new WorkStatusCollectionClass();

        string RelateAnalyzeString = "";
        //string RelateAnalyzeInformation = "";
        PassInfoClass PassInfo = new PassInfoClass();

        public bool IsNeedToCheck
        {
            get
            {
                return Tolerance > 0;

            }
        }

        #region Operation Data
        public RectangleF FromRectF
        {
            get; set;
        }
        
        /// <summary>
        /// 取得檢測資料
        /// </summary>
        /// <param name="position"></param>
        /// <param name="bmpinput"></param>
        /// <param name="bmpmask"></param>
        public bool GetCornerData(JetEazy.CornerEnum corner,Bitmap bmpinput,Bitmap bmpmask,string relateanalyzestring,PassInfoClass passinfo)
        {
            string str = "";
            bool isgood = true;

            Corner = corner;

            RelateAnalyzeString = relateanalyzestring;
            //RelateAnalyzeInformation = relateanalyzeinformation;
            PassInfo = new PassInfoClass(passinfo, OPLevelEnum.COPY);
            PassInfo.Corner = corner;

            if (!IsNeedToCheck)
                return true;

            WorkStatusClass workstatus = new WorkStatusClass(AnanlyzeProcedureEnum.ALIGNTRAIN);
            string processstring = "Start " + RelateAnalyzeString + "#" + PassInfo.CornerNameString + " Train." + Environment.NewLine;
            string errorstring = "";
            ReasonEnum reason = ReasonEnum.PASS;

            RectangleF rectffrom = new RectangleF(0, 0, bmpinput.Width, bmpinput.Height);

            FromRectF = GetPartRectangleF(rectffrom, Corner, 0, Width, Height);
            
            Bitmap mybmpInput = (Bitmap)bmpinput.Clone(FromRectF, PixelFormat.Format32bppArgb);
            Bitmap mybmpMask = (Bitmap)bmpmask.Clone(FromRectF, PixelFormat.Format32bppArgb);
            Bitmap mybmpOutput = (Bitmap)bmpmask.Clone(FromRectF, PixelFormat.Format32bppArgb);

            AlignPara.AlignMethod = AlignMethodEnum.AUFIND;
            AlignPara.AlignMode = AlignModeEnum.AREA;
            AlignPara.Rotation = 5;
            AlignPara.MTTolerance = Tolerance;

            //bmpmask.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + relateanalyzestring + " Real Mask" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

            //mybmpInput.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + relateanalyzestring + " Input" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            //mybmpMask.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + relateanalyzestring + " MaskA" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

            #region Log Process

            //AlignPara.ResetTrainStatus();

            isgood = AlignPara.AlignTrainProcess(mybmpInput,ref mybmpInput, mybmpMask, Brightness, Contrast, RelateAnalyzeString + "#" + PassInfo.CornerNameString, PassInfo, true);
            //AlignPara.IsTempSave = true;
            //isgood = AlignPara.AuFindRun(mybmpInput, ref mybmpOutput, true, Brightness, Contrast);
            //AlignPara.IsTempSave = false;

            AlignPara.FillTrainStatus(TrainStatusCollection);

            #endregion


            if (isgood)
            {
                str = RelateAnalyzeString + "#" + PassInfo.CornerNameString + " Corner Train Succesful.";
                processstring = str + Environment.NewLine;

                reason = ReasonEnum.PASS;
            }
            else
            {
                str = "<[###ERROR###]>" + RelateAnalyzeString + "#" + PassInfo.CornerNameString + " Corner Train Error.";
                processstring = str + Environment.NewLine;

                reason = ReasonEnum.NG;
            }

            workstatus.SetWorkStatus(mybmpInput, mybmpOutput, mybmpMask, reason, errorstring, processstring, PassInfo);

            TrainStatusCollection.Add(workstatus);

            //LogMessage(str);

            mybmpInput.Dispose();
            mybmpMask.Dispose();
            mybmpOutput.Dispose();

            return isgood;
        }
        /// <summary>
        /// 取得顯示的資料
        /// </summary>
        /// <param name="position"></param>
        /// <param name="biaspointf"></param>
        /// <param name="sizef"></param>
        /// <returns></returns>
        public RectangleF GetCornerData(PositionEnum position, PointF biaspointf,SizeF sizef, Bitmap bmp)
        {
            RectangleF rectffrom = new RectangleF(biaspointf, sizef);
            rectffrom = GetPartRectangleF(rectffrom, position, 0, Width, Height);

            //Draw For Show
            RectangleF rectfdraw = new RectangleF(new PointF(0,0), sizef);
            rectfdraw = GetPartRectangleF(rectfdraw, position, 0, Width, Height);
            Rectangle rectdraw = new Rectangle((int)rectfdraw.X, (int)rectfdraw.Y, (int)rectfdraw.Width, (int)rectfdraw.Height);
            SetBrightContrast(bmp, rectdraw, Brightness, Contrast);

            return rectffrom;
        }

        public bool RunCornerData(Bitmap bmpinput,bool istrain)
        {
            string str = "";
            bool isgood = true;

            if (!IsNeedToCheck)
                return true;
           
            RectangleF rectffrom = new RectangleF(0, 0, bmpinput.Width, bmpinput.Height);

            FromRectF = GetPartRectangleF(rectffrom, Corner, 0, Width, Height);

            Bitmap mybmpInput = (Bitmap)bmpinput.Clone(FromRectF, PixelFormat.Format32bppPArgb);
            Bitmap mybmpoutput = new Bitmap(1, 1);

            AlignPara.AlignMethod = AlignMethodEnum.AUFIND;
            AlignPara.AlignMode = AlignModeEnum.AREA;
            AlignPara.Rotation = 5;
            AlignPara.MTTolerance = Tolerance;



            //mybmpInput.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + relateanalyzestring + " Input" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            //mybmpMask.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + relateanalyzestring + " Mask" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

            //if (istrain)
            //    AlignPara.ResetTrainStatus();
            //else
            //    AlignPara.ResetRunStatus();

            isgood = AlignPara.AuFindRun(mybmpInput, ref mybmpoutput, istrain, Brightness, Contrast);

            if (istrain)
                AlignPara.FillTrainStatus(TrainStatusCollection, null);
            else
                AlignPara.FillRunStatus(RunStatusCollection);

            if (isgood)
                str = RelateAnalyzeString + "#" + PassInfo.CornerNameString + " Corner Run Succesful.";
            else
                str = RelateAnalyzeString + "#" + PassInfo.CornerNameString + " Corner Run Error.";

            //LogMessage(str);

            mybmpInput.Dispose();
            mybmpoutput.Dispose();

            return isgood;
        }
        

        #endregion

        public CornerClass()
        {

        }
        public CornerClass(string cornerstr)
        {
            FromString(cornerstr);

        }
        public override string ToString()
        {
            string str = "";
            
            str += Width.ToString() + ";";              //0
            str += Height.ToString() + ";";             //1
            str += Tolerance.ToString() + ";";          //2
            str += Brightness.ToString() + ";";         //3
            str += Contrast.ToString();                 //4

            return str;
        }
        public void FromString(string str)
        {
            string[] strs = str.Split(';');

            Width = int.Parse(strs[0]);
            Height = int.Parse(strs[1]);
            Tolerance = float.Parse(strs[2]);

            if(strs.Length > 3)
            {
                Brightness = int.Parse(strs[3]);
                Contrast = int.Parse(strs[4]);
            }
        }

        public void Reset()
        {
            Width = 20;
            Height = 20;
            Tolerance = 0;

            Brightness = 0;
            Contrast = 0;
            
        }

        /// <summary>
        /// 在做大量運算前要清除的相關資料
        /// </summary>
        public void ResetTrainStatus()
        {
            TrainStatusCollection.Clear();

            AlignPara.ResetTrainStatus();

        }
        /// <summary>
        /// 在做大量運算前要清除的相關資料
        /// </summary>
        public void ResetRunStatus()
        {
            RunStatusCollection.Clear();

            AlignPara.ResetRunStatus();
        }
        /// <summary>
        /// 將產生出來的過程寫出去
        /// </summary>
        /// <param name="processstringlist"></param>
        /// <param name="runstatuslist"></param>
        public void FillRunStatus(WorkStatusCollectionClass runstatuscollection)
        {
            foreach (WorkStatusClass runstatus in RunStatusCollection.WorkStatusList)
            {
                runstatuscollection.Add(runstatus);
            }
        }
        /// <summary>
        /// 將產生出來Train的過程寫出去
        /// </summary>
        /// <param name="workstatuscollection"></param>
        public void FillTrainStatus(WorkStatusCollectionClass workstatuscollection)
        {
            foreach (WorkStatusClass workstatus in TrainStatusCollection.WorkStatusList)
            {
                workstatuscollection.Add(workstatus);
            }
        }
        public void FillTrainStatus(WorkStatusCollectionClass workstatuscollection, string filltoanalyzestr)
        {
            foreach (WorkStatusClass workstatus in TrainStatusCollection.WorkStatusList)
            {
                if(filltoanalyzestr == null)
                {
                    if(workstatus.LogString == "")
                    {
                        workstatuscollection.Add(workstatus);
                    }
                }
                else 
                {
                    if (workstatus.LogString.IndexOf(filltoanalyzestr) < 0)
                    {
                        workstatus.LogString += filltoanalyzestr;
                        workstatuscollection.Add(workstatus);
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
            AlignPara.AddTrainLogString(logstr);
        }
        public void ConstructProperty()
        {
            publicproperties.Add(new myProperty("Width", "06.AOI"));
            publicproperties.Add(new myProperty("Height", "06.AOI"));
            publicproperties.Add(new myProperty("Tolerance", "06.AOI"));
            publicproperties.Add(new myProperty("Brightness", "06.AOI"));
            publicproperties.Add(new myProperty("Contrast", "06.AOI"));
        }
        public void Suicide()
        {
            AlignPara.Suicide();

            TrainStatusCollection.Clear();
            RunStatusCollection.Clear();
        }

        #region Tools Operation
        RectangleF GetPartRectangleF(RectangleF rectffrom, JetEazy.CornerEnum corner, float ratio, float width, float height)
        {
            RectangleF rectf = new RectangleF();

            switch (corner)
            {
                case JetEazy.CornerEnum.XDIR:
                    rectf.Width = rectffrom.Width;
                    rectf.Height = rectffrom.Height * ratio / 100f;

                    rectf.X = rectffrom.X;
                    rectf.Y = rectffrom.Y + (rectffrom.Height / 2) - (rectf.Height / 2);
                    break;
                case JetEazy.CornerEnum.YDIR:
                    rectf.Width = rectffrom.Width * (float)ratio / 100f;
                    rectf.Height = rectffrom.Height;

                    rectf.X = rectffrom.X + (rectffrom.Width / 2) - (rectf.Width / 2);
                    rectf.Y = rectffrom.Y;

                    break;
                case JetEazy.CornerEnum.LT:

                    rectf.Width = width;
                    rectf.Height = height;

                    rectf.X = rectffrom.X;
                    rectf.Y = rectffrom.Y;
                    break;
                case JetEazy.CornerEnum.LB:

                    rectf.Width = width;
                    rectf.Height = height;

                    rectf.X = rectffrom.X;
                    rectf.Y = rectffrom.Bottom - rectf.Height;

                    break;
                case JetEazy.CornerEnum.RT:

                    rectf.Width = width;
                    rectf.Height = height;

                    rectf.X = rectffrom.Right - rectf.Width;
                    rectf.Y = rectffrom.Y;

                    break;
                case JetEazy.CornerEnum.RB:

                    rectf.Width = width;
                    rectf.Height = height;

                    rectf.X = rectffrom.Right - rectf.Width;
                    rectf.Y = rectffrom.Bottom - rectf.Height;

                    break;
            }

            return rectf;
        }
        RectangleF GetPartRectangleF(RectangleF rectffrom, PositionEnum position, float ratio, float width, float height)
        {
            RectangleF rectf = new RectangleF();

            switch (position)
            {
                case PositionEnum.XDir:
                    rectf.Width = rectffrom.Width;
                    rectf.Height = rectffrom.Height * ratio / 100f;

                    rectf.X = rectffrom.X;
                    rectf.Y = rectffrom.Y + (rectffrom.Height / 2) - (rectf.Height / 2);
                    break;
                case PositionEnum.YDir:
                    rectf.Width = rectffrom.Width * (float)ratio / 100f;
                    rectf.Height = rectffrom.Height;

                    rectf.X = rectffrom.X + (rectffrom.Width / 2) - (rectf.Width / 2);
                    rectf.Y = rectffrom.Y;

                    break;
                case PositionEnum.LeftTop:

                    rectf.Width = width;
                    rectf.Height = height;

                    rectf.X = rectffrom.X;
                    rectf.Y = rectffrom.Y;
                    break;
                case PositionEnum.LeftBottom:

                    rectf.Width = width;
                    rectf.Height = height;

                    rectf.X = rectffrom.X;
                    rectf.Y = rectffrom.Bottom - rectf.Height;

                    break;
                case PositionEnum.RightTop:

                    rectf.Width = width;
                    rectf.Height = height;

                    rectf.X = rectffrom.Right - rectf.Width;
                    rectf.Y = rectffrom.Y;

                    break;
                case PositionEnum.RightBottom:

                    rectf.Width = width;
                    rectf.Height = height;

                    rectf.X = rectffrom.Right - rectf.Width;
                    rectf.Y = rectffrom.Bottom - rectf.Height;

                    break;
            }

            return rectf;
        }
        RectangleF GetPartRectangleF(RectangleF rectfrom, PositionEnum position, float ratio, float width, float height, int extendx, int extendy)
        {
            RectangleF rectf = GetPartRectangleF(rectfrom, position, ratio, width, height);

            rectf.Inflate(extendx, extendy);

            return rectf;
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
        #endregion
    }

    public class WHClass : ICustomClass
    {
        PropertyList publicproperties = new PropertyList();
        //ICustomClass implementation
        public PropertyList PublicProperties
        {
            get
            {
                return publicproperties;
            }
            set
            {
                publicproperties = value;
            }
        }

        float _RangeRatio = 20f;
        float _Diff = 0f;
        float _ThreshodRatio = 20;
        int _SampleGap = 5;
        RectangleF _FromRectF = new RectangleF();

        [DefaultValue(20)]
        [Description("Range Ratio\rFrom 0 <-> 100%, default 20%")]
        [DisplayName("R.Ratio")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 5f, 2)]
        public float RangeRatio
        {
            get
            {
                return _RangeRatio;
            }
            set
            {
                _RangeRatio = value;
            }
        }

        [DefaultValue(20)]
        [Description("Threshold Ratio\rFrom -100 <-> 100%, default 20%")]
        [DisplayName("T.Ratio")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-100f, 100f, 5f, 2)]
        public float ThresholdRatio
        {
            get
            {
                return _ThreshodRatio;
            }
            set
            {
                _ThreshodRatio = value;
            }
        }

        [DefaultValue(0)]
        [Description("Diff Length.\rFrom 0 <-> 100mm.,  default 0.2.\rDisable is 0")]
        [DisplayName("Diff Length")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100f, 0.1f, 2)]
        public float Diff
        {
            get
            {
                return _Diff;
            }
            set
            {
                _Diff = value;
            }
        }

        [DefaultValue(0)]
        [Description("0 is No Need.\rRange -200 <-> 200, default 0")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-200, 200)]
        public int Brightness
        {
            get; set;
        }
        [DefaultValue(0)]
        [Description("0 is No Need.\rRange -200 <-> 200, default 0")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(-200, 200)]
        public int Contrast
        {
            get; set;
        }
        [DefaultValue(5)]
        [DisplayName("Sampling")]
        [Description("Sampling Gap 5 <-> 200, Min is 5")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(5, 200)]
        public int SampleGap
        {
            get
            {
                return _SampleGap;
            }
            set
            {
                _SampleGap = value;
            }
        }

        public RectangleF FromRectF
        {
            get
            {
                return _FromRectF;
            }
            set
            {
                _FromRectF = value;
            }
        }

        Bitmap bmpPattern = new Bitmap(1, 1);
        Bitmap bmpMask = new Bitmap(1, 1);

        JzFindObjectClass JzFind = new JzFindObjectClass();

        #region Online Data

        PositionEnum myPosition = PositionEnum.XDir;


        public WorkStatusCollectionClass TrainStatusCollection = new WorkStatusCollectionClass();
        public WorkStatusCollectionClass RunStatusCollection = new WorkStatusCollectionClass();

        float OrgLength = 0f;
        float RunLength = 0f;

        string RelateAnalyzeString = "";
        //string RelateAnalyzeInformation = "";
        PassInfoClass PassInfo = new PassInfoClass();

        float Resolution = 0f;

        public bool IsNeedToCheck
        {
            get
            {
                return Diff > 0;
            }
        }


        #endregion

        public WHClass()
        {

        }
        public WHClass(string whstr)
        {
            FromString(whstr);

        }
        public override string ToString()
        {
            string str = "";

            str += RangeRatio.ToString() + ";";         //0
            str += Diff.ToString() + ";";               //1
            str += Brightness.ToString() + ";";         //2
            str += Contrast.ToString() + ";";           //3
            str += ThresholdRatio.ToString() + ";";     //4
            str += SampleGap.ToString();

            return str;
        }
        public void FromString(string str)
        {
            string[] strs = str.Split(';');

            RangeRatio = float.Parse(strs[0]);
            Diff = float.Parse(strs[1]);

            if (strs.Length > 2)
            {
                Brightness = int.Parse(strs[2]);
                Contrast = int.Parse(strs[3]);
            }
            if(strs.Length > 4)
            {
                ThresholdRatio = float.Parse(strs[4]);
            }
            if (strs.Length > 5)
            {
                SampleGap = int.Parse(strs[5]);
            }

        }
        public void Reset()
        {
            RangeRatio = 20f;
            Diff = 0f;

            Brightness = 0;
            Contrast = 0;

            ThresholdRatio = 20f;
            SampleGap = 5;

        }
        public void ConstructProperty()
        {
            publicproperties.Add(new myProperty("RangeRatio", "06.AOI"));
            publicproperties.Add(new myProperty("ThresholdRatio", "06.AOI"));
            publicproperties.Add(new myProperty("Diff", "06.AOI"));
            publicproperties.Add(new myProperty("Brightness", "06.AOI"));
            publicproperties.Add(new myProperty("Contrast", "06.AOI"));
            publicproperties.Add(new myProperty("SampleGap", "06.AOI"));
        }
        public RectangleF GetWHData(PositionEnum position, PointF biaspointf, SizeF sizef,Bitmap bmpshowpattern,Bitmap bmpshowmask)
        {
            RectangleF rectffrom = new RectangleF(biaspointf, sizef);

            rectffrom = GetPartRectangleF(rectffrom, position, RangeRatio, 0, 0);

            //Draw For Show
            RectangleF rectfdraw = new RectangleF(new PointF(0, 0), sizef);
            rectfdraw = GetPartRectangleF(rectfdraw, position, RangeRatio, 0, 0);
            
            Bitmap bmpshowpt = (Bitmap)bmpshowpattern.Clone(rectfdraw, PixelFormat.Format32bppPArgb);
            Bitmap bmpshowmsk = (Bitmap)bmpshowmask.Clone(rectfdraw, PixelFormat.Format32bppPArgb);
            
            GetLengthForShow(bmpshowpt, bmpshowmsk, Brightness, Contrast);

            //bmpshowpt.Save(Universal.TESTPATH + "\\ANALYZETEST\\SHOWPT 00" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

            DrawImage(bmpshowpt, bmpshowpattern, rectfdraw);

            //bmpshowpattern.Save(Universal.TESTPATH + "\\ANALYZETEST\\SHOWPT 01" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

            bmpshowpt.Dispose();
            bmpshowmsk.Dispose();

            return rectffrom;
        }
        public void GetWHData(PositionEnum position, Bitmap bmpinput, Bitmap bmpmask,string relateanalyzestring, PassInfoClass passinfo,float resolution)
        {
            RelateAnalyzeString = relateanalyzestring;
            //RelateAnalyzeInformation = relateanalyzeinformation;
            PassInfo = new PassInfoClass(passinfo, OPLevelEnum.COPY);

            Resolution = resolution;

            if (!IsNeedToCheck)
                return;

            WorkStatusClass workstatus = new WorkStatusClass(AnanlyzeProcedureEnum.CHECKWH);
            string processstring = "Start " + RelateAnalyzeString + "#" + myPosition.ToString() + " Train." + Environment.NewLine;
            string errorstring = "";
            ReasonEnum reason = ReasonEnum.PASS;

            RectangleF rectffrom = new RectangleF(0, 0, bmpinput.Width, bmpinput.Height);

            myPosition = position;

            FromRectF = GetPartRectangleF(rectffrom, myPosition, RangeRatio, 0, 0);

            bmpPattern.Dispose();
            bmpPattern = (Bitmap)bmpinput.Clone(FromRectF, PixelFormat.Format32bppPArgb);

            bmpMask.Dispose();
            bmpMask = (Bitmap)bmpmask.Clone(FromRectF, PixelFormat.Format32bppPArgb);

            //bmpPattern.Save(Universal.TESTPATH + "\\ANALYZETEST\\WHPATTERN " + RelateAnalyzeString + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            //bmpMask.Save(Universal.TESTPATH + "\\ANALYZETEST\\WHMASK " + RelateAnalyzeString + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

            processstring += RelateAnalyzeString + "#" + myPosition.ToString() + "Set Brightness to " + Brightness.ToString() + " and Contrast to " + Contrast.ToString() + Environment.NewLine;

            OrgLength = GetLength(bmpPattern, bmpMask, Brightness, Contrast);

            processstring += RelateAnalyzeString + "#" + myPosition.ToString() + " Origin value is " + OrgLength.ToString("0.000") + Environment.NewLine;
            processstring += RelateAnalyzeString + "#" + myPosition.ToString() + " WH Train Successful." + Environment.NewLine;
            
            workstatus.SetWorkStatus(bmpPattern, bmpPattern, bmpMask, reason, errorstring, processstring, PassInfo);
            TrainStatusCollection.Add(workstatus);

        }
        public bool RunWHData(Bitmap bmpinput,bool istrain)
        {
            bool isgood = true;

            WorkStatusClass workstatus = new WorkStatusClass(AnanlyzeProcedureEnum.CHECKWH);
            string processstring = "Start " + RelateAnalyzeString + "#" + myPosition.ToString() + " Measure." + Environment.NewLine;
            string errorstring = "";
            ReasonEnum reason = ReasonEnum.PASS;

            Bitmap mybmpInput = (Bitmap)bmpinput.Clone(FromRectF, PixelFormat.Format32bppPArgb);
            
            RunLength = GetLength(mybmpInput, bmpMask, Brightness, Contrast);

            float rundiff = Math.Abs(RunLength - OrgLength) * Resolution;

            if (rundiff > Diff)
            {
                processstring += "Check For " + RelateAnalyzeString + "#" + myPosition.ToString() + " Measure " + rundiff.ToString("0.000") + " > " + Diff.ToString("0.000") + " ." + Environment.NewLine;
                errorstring += "Error For " + RelateAnalyzeString + "#" + myPosition.ToString() + " Measure " + rundiff.ToString("0.000") + " > " + Diff.ToString("0.000") + " ." + Environment.NewLine;
                
                processstring += RelateAnalyzeString + " " + myPosition.ToString() + " Measure Error." + Environment.NewLine;

                reason = ReasonEnum.NG;

                DrawText(mybmpInput, rundiff.ToString("0.000"), 50);

                isgood = false;
            }
            else
            {
                processstring += "Check For " + RelateAnalyzeString + " " + myPosition.ToString() + " Measure " + (Math.Abs(RunLength - OrgLength) * Resolution).ToString("0.00") + " <= " + Diff.ToString("0.00") + " ." + Environment.NewLine;
                processstring += RelateAnalyzeString + " " + myPosition.ToString() + " Measure Successful." + Environment.NewLine;
            }
            
            workstatus.SetWorkStatus(bmpPattern, mybmpInput, bmpPattern, reason, errorstring, processstring, PassInfo);

            if (istrain)
                TrainStatusCollection.Add(workstatus);
            else
                RunStatusCollection.Add(workstatus);

            mybmpInput.Dispose();

            return isgood;
        }

        public void Suicide()
        {
            bmpPattern.Dispose();
            bmpMask.Dispose();

            TrainStatusCollection.Clear();
            RunStatusCollection.Clear();
        }

        /// <summary>
        /// 取得WH的距離
        /// </summary>
        /// <param name="bmpinput"></param>
        /// <param name="bmpmask"></param>
        /// <param name="brightness"></param>
        /// <param name="contrast"></param>
        /// <returns></returns>
        float GetLength(Bitmap bmpinput, Bitmap bmpmask, int brightness, int contrast)
        {
            int i = 0;
            float retlength = 0f;

            SetBrightContrast(bmpinput, Brightness, Contrast);

            HistogramClass histo = new HistogramClass(2);

            histo.GetHistogram(bmpinput, bmpmask, true);

            int max = histo.MaxGrade;
            int min = histo.MinGrade;
            int mean = histo.MeanGrade;

            int thresholdvalue = (int)((float)(max - min) * Math.Abs(ThresholdRatio) / 100) + min;

            //bmpinput.Save(Universal.TESTPATH + "\\ANALYZETEST\\WHFIND 00 " + myPosition.ToString() + " " +  RelateAnalyzeString + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

            JzFind.SetThreshold(bmpinput, bmpmask, 255, thresholdvalue, 0, 255, (ThresholdRatio > 0));

            //JzFind.Find(bmpinput, Color.Red);
            switch (myPosition)
            {
                case PositionEnum.XDir:

                    retlength = (float)GetWidth(bmpinput);

                    break;
                case PositionEnum.YDir:

                    retlength = (float)GetHeight(bmpinput);

                    break;
            }

            return retlength;
        }
        /// <summary>
        /// 取得WH要的顯示的東東
        /// </summary>
        /// <param name="bmpinput"></param>
        /// <param name="bmpmask"></param>
        /// <param name="brightness"></param>
        /// <param name="contrast"></param>
        /// <returns></returns>
        float GetLengthForShow(Bitmap bmpinput, Bitmap bmpmask, int brightness, int contrast)
        {
            int i = 0;
            float retlength = 0f;

            SetBrightContrast(bmpinput, Brightness, Contrast);

            HistogramClass histo = new HistogramClass(2);

            histo.GetHistogram(bmpinput, bmpmask, true);

            int max = histo.MaxGrade;
            int min = histo.MinGrade;
            int mean = histo.MeanGrade;

            int thresholdvalue = (int)((float)(max - min) * Math.Abs(ThresholdRatio) / 100) + min;

            //bmpinput.Save(Universal.TESTPATH + "\\ANALYZETEST\\WHFIND 00 " + myPosition.ToString() + " " +  RelateAnalyzeString + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

            JzFind.SetThresholdForShow(bmpinput, bmpmask, 255, thresholdvalue, 0, 255, (ThresholdRatio > 0));

            return retlength;
        }

        double GetWidth(Bitmap bmp)
        {
            double totallength = 0d;
            List<int> lenlist = new List<int>();

            int sampling = SampleGap + 2;

            List<PointF> lfptlist = new List<PointF>();
            List<PointF> rtptlist = new List<PointF>();

            JzFind.GetInnerLineFromCenter(bmp, true, 3, ref lfptlist, ref rtptlist, ref lenlist);

            PointF[] lfptf = lfptlist.ToArray();
            PointF[] rtptf = rtptlist.ToArray();

            //找尋左邊的線 x= ay + b
            QvLineFit lfline = new QvLineFit();
            lfline.Swap = true;
            lfline.LeastSquareFit(lfptf);

            //找尋右邊的線 x= ay + b
            QvLineFit rtline = new QvLineFit();
            rtline.Swap = true;
            rtline.LeastSquareFit(rtptf);

            //在左邊的線找出 n 個點來平均距離

            PointF ptFhead = lfptlist[0];
            PointF ptFtail = lfptlist[lfptlist.Count - 1];

            float gapvalue = (ptFtail.Y - ptFhead.Y) / (float)sampling;

            int i = 0;
            totallength = 0;

            while (i < SampleGap)
            {
                float y = (ptFhead.Y + (float)(i + 1) * gapvalue);
                float x = (float)(lfline.A * y + lfline.B);

                PointF chkptf = new PointF(x, y);

                double len = rtline.GetPointLength(chkptf);

                totallength += len;

                i++;
            }

            totallength = totallength / (double)SampleGap;

            return totallength;
        }
        double GetHeight(Bitmap bmp)
        {
            double totallength = 0d;
            List<int> lenlist = new List<int>();

            int sampling = SampleGap + 2;

            List<PointF> tpptlist = new List<PointF>();
            List<PointF> btptlist = new List<PointF>();

            JzFind.GetInnerLineFromCenter(bmp, false, 3, ref tpptlist, ref btptlist, ref lenlist);

            PointF[] tpptf = tpptlist.ToArray();
            PointF[] btptf = btptlist.ToArray();

            //找尋左邊的線 x= ay + b
            QvLineFit tpline = new QvLineFit();
            tpline.Swap = false;
            tpline.LeastSquareFit(tpptf);

            //找尋右邊的線 x= ay + b
            QvLineFit btline = new QvLineFit();
            btline.Swap = false;
            btline.LeastSquareFit(btptf);

            //在左邊的線找出 n 個點來平均距離

            PointF ptFhead = tpptlist[0];
            PointF ptFtail = tpptlist[tpptlist.Count - 1];

            float gapvalue = (ptFtail.X - ptFhead.X) / (float)sampling;

            int i = 0;
            totallength = 0;

            while (i < SampleGap)
            {
                float x = (ptFhead.X + (float)(i + 1) * gapvalue);
                float y = (float)(tpline.A * x + tpline.B);

                PointF chkptf = new PointF(x, y);

                double len = btline.GetPointLength(chkptf);

                totallength += len;

                i++;
            }

            totallength = totallength / (double)SampleGap;

            return totallength;
        }
        /// <summary>
        /// 在做大量運算前要清除的相關資料
        /// </summary>
        public void ResetTrainStatus()
        {
            TrainStatusCollection.Clear();
        }
        /// <summary>
        /// 在做大量運算前要清除的相關資料
        /// </summary>
        public void ResetRunStatus()
        {
            RunStatusCollection.Clear();
        }
        /// <summary>
        /// 將產生出來的過程寫出去
        /// </summary>
        /// <param name="processstringlist"></param>
        /// <param name="runstatuslist"></param>
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
        /// <param name="trainstatuscollection"></param>
        public void FillTrainStatus(WorkStatusCollectionClass trainstatuscollection)
        {
            foreach (WorkStatusClass trainstatus in TrainStatusCollection.WorkStatusList)
            {
                trainstatuscollection.Add(trainstatus);
            }
        }
        public void FillTrainStatus(WorkStatusCollectionClass trainstatuscollection,string filltoanalyzestr)
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
        #region Tools Operation
        RectangleF GetPartRectangleF(RectangleF rectffrom, PositionEnum position, float ratio, float width, float height)
        {
            RectangleF rectf = new RectangleF();

            switch (position)
            {
                case PositionEnum.XDir:
                    rectf.Width = rectffrom.Width;
                    rectf.Height = rectffrom.Height * ratio / 100f;

                    rectf.X = rectffrom.X;
                    rectf.Y = rectffrom.Y + (rectffrom.Height / 2) - (rectf.Height / 2);
                    break;
                case PositionEnum.YDir:
                    rectf.Width = rectffrom.Width * (float)ratio / 100f;
                    rectf.Height = rectffrom.Height;

                    rectf.X = rectffrom.X + (rectffrom.Width / 2) - (rectf.Width / 2);
                    rectf.Y = rectffrom.Y;

                    break;
                case PositionEnum.LeftTop:

                    rectf.Width = width;
                    rectf.Height = height;

                    rectf.X = rectffrom.X;
                    rectf.Y = rectffrom.Y;
                    break;
                case PositionEnum.LeftBottom:

                    rectf.Width = width;
                    rectf.Height = height;

                    rectf.X = rectffrom.X;
                    rectf.Y = rectffrom.Bottom - rectf.Height;

                    break;
                case PositionEnum.RightTop:

                    rectf.Width = width;
                    rectf.Height = height;

                    rectf.X = rectffrom.Right - rectf.Width;
                    rectf.Y = rectffrom.Y;

                    break;
                case PositionEnum.RightBottom:

                    rectf.Width = width;
                    rectf.Height = height;

                    rectf.X = rectffrom.Right - rectf.Width;
                    rectf.Y = rectffrom.Bottom - rectf.Height;

                    break;
            }

            return rectf;
        }
        RectangleF GetPartRectangleF(RectangleF rectfrom, PositionEnum position, float ratio, float width, float height, int extendx, int extendy)
        {
            RectangleF rectf = GetPartRectangleF(rectfrom, position, ratio, width, height);

            rectf.Inflate(extendx, extendy);

            return rectf;
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

        void DrawText(Bitmap bmp, string text,int fontsize)
        {
            SolidBrush B = new SolidBrush(Color.Lime);
            Font MyFont = new Font("Arial", fontsize);

            Graphics g = Graphics.FromImage(bmp);
            g.DrawString(text, MyFont, B, new PointF(5, 5));
            g.Dispose();
        }
        Rectangle SimpleRect(Size sz)
        {
            return new Rectangle(0, 0, sz.Width, sz.Height);
        }
        void DrawImage(Bitmap bmpfrom, Bitmap bmpto, RectangleF destrectf)
        {
            Graphics g = Graphics.FromImage(bmpto);
            g.DrawImage(bmpfrom, destrectf, SimpleRect(bmpfrom.Size), GraphicsUnit.Pixel);
            g.Dispose();
        }
        Rectangle RectFToRect(RectangleF rectf)
        {
            Rectangle rect = new Rectangle((int)rectf.X, (int)rectf.Y, (int)rectf.Width, (int)rectf.Height);

            return rect;
        }
        #endregion
    }

    public class AOIClass
    {
        public AOIMethodEnum AOIMethod = AOIMethodEnum.NONE; //AOI項目
        public CheckDirtMethodEnum CheckDirtMethod = CheckDirtMethodEnum.NONE;          //指定檢查髒污的方法
        public UselessMethodEnum CheckColorMethod = UselessMethodEnum.NONE;       //指定鍵帽顏色的方法，不會再用了

        public bool IsNG = false;               //是否為NG的圖形

        public float DirtRatio = 30f;           //髒污的對比
        public int DirtArea = 10;               //最小髒污

        public float ColorRatio = 70f;          //顏色對比，不會再用了
        public float TotalColorRatio = 70f;     //顏色比例，不會再用了

        public CornerClass[] CornerArray = new CornerClass[(int)CornerEnum.COUNT];
        public WHClass[] WHArray = new WHClass[(int)PositionEnum.DIRCOUNT];

        #region Online Data


        public WorkStatusCollectionClass TrainStatusCollection = new WorkStatusCollectionClass();
        public WorkStatusCollectionClass RunStatusCollection = new WorkStatusCollectionClass();

        HistogramClass myHistogram = new HistogramClass(2);

        Bitmap bmpPattern = new Bitmap(1, 1);
        public Bitmap bmpDirt = new Bitmap(1, 1);
        Bitmap bmpInput = new Bitmap(1, 1);

        float OrgDirtMean = 0;
        float RunDirtMean = 0;

        JzFindObjectClass JzFind = new JzFindObjectClass();

        string RelateAnalyzeString = "";
        //string RelateAnalyzeInformation = "";
        PassInfoClass PassInfo = new PassInfoClass();
        PassInfoClass DirtPassInfo = new PassInfoClass();

        public bool IsHaveCornerOrWH
        {
            get
            {
                int i = 0;
                bool ret = false;

                i = 0;
                while(i < (int)CornerEnum.COUNT)
                {
                    if(CornerArray[i].IsNeedToCheck)
                    {
                        ret = true;
                        break;
                    }
                    i++;
                }

                if (!ret)
                {
                    i = 0;
                    while (i < (int)PositionEnum.DIRCOUNT)
                    {
                        if (WHArray[i].IsNeedToCheck)
                        {
                            ret = true;
                            break;
                        }
                        i++;
                    }
                }

                return ret;
            }
        }

        public bool IsPass = true;

        #endregion

        public AOIClass()
        {
            int i = 0;

            i = 0;
            while (i < (int)CornerEnum.COUNT)
            {
                CornerArray[i] = new CornerClass();
                CornerArray[i].ConstructProperty();

                i++;
            }

            i = 0;
            while( i < (int)PositionEnum.DIRCOUNT)
            {
                WHArray[i] = new WHClass();
                WHArray[i].ConstructProperty();

                i++;
            }

        }
        public AOIClass(string str)
        {
            FromString(str);
        }
        public override string ToString()
        {
            string str = "";

            str += ((int)AOIMethod).ToString() + Universal.SeperateCharB;           //0
            str += ((int)CheckDirtMethod).ToString() + Universal.SeperateCharB;                //1
            str += ((int)CheckColorMethod).ToString() + Universal.SeperateCharB;    //2
            str += (IsNG ? "1" : "0") + Universal.SeperateCharB;                    //3

            str += DirtRatio.ToString() + Universal.SeperateCharB;                  //4
            str += DirtArea.ToString() + Universal.SeperateCharB;                   //5
            str += ColorRatio.ToString() + Universal.SeperateCharB;                 //6
            str += TotalColorRatio.ToString() + Universal.SeperateCharB;            //7
            str += CornerArray[(int)CornerEnum.LT].ToString() + Universal.SeperateCharB; //8 
            str += CornerArray[(int)CornerEnum.RT].ToString() + Universal.SeperateCharB; //9 
            str += CornerArray[(int)CornerEnum.LB].ToString() + Universal.SeperateCharB; //10 
            str += CornerArray[(int)CornerEnum.RB].ToString() + Universal.SeperateCharB; //11 

            str += WHArray[(int)PositionEnum.XDir].ToString() + Universal.SeperateCharB; //10 
            str += WHArray[(int)PositionEnum.YDir].ToString() + Universal.SeperateCharB; //10 

            str += "";

            return str;
        }
        public void FromString(string str)
        {
            string[] strs = str.Split(Universal.SeperateCharB);

            AOIMethod = (AOIMethodEnum)int.Parse(strs[0]);
            CheckDirtMethod = (CheckDirtMethodEnum)int.Parse(strs[1]);
            CheckColorMethod = (UselessMethodEnum)int.Parse(strs[2]);
            IsNG = strs[3] == "1";

            DirtRatio = float.Parse(strs[4]);
            DirtArea = int.Parse(strs[5]);
            ColorRatio = float.Parse(strs[6]);
            TotalColorRatio = float.Parse(strs[7]);

            if (strs.Length > 9)
            {
                CornerArray[(int)CornerEnum.LT].FromString(strs[8]);
                CornerArray[(int)CornerEnum.RT].FromString(strs[9]);
                CornerArray[(int)CornerEnum.LB].FromString(strs[10]);
                CornerArray[(int)CornerEnum.RB].FromString(strs[11]);

                WHArray[(int)PositionEnum.XDir].FromString(strs[12]);
                WHArray[(int)PositionEnum.YDir].FromString(strs[13]);
            }
        }
        public void FromPropertyChange(string changeitemstring, string valuestring)
        {
            string[] str = changeitemstring.Split(';');

            if (str[0] == "06.AOI")
            {
                switch (str[1])
                {
                    case "AOIMethod":
                        AOIMethod = (AOIMethodEnum)Enum.Parse(typeof(AOIMethodEnum), valuestring, true);
                        break;
                    case "CheckDirtMethod":
                        CheckDirtMethod = (CheckDirtMethodEnum)Enum.Parse(typeof(CheckDirtMethodEnum), valuestring, true);
                        break;
                    //case "CheckColorMethod":
                    //    CheckColorMethod = (CheckColorMethodEnum)Enum.Parse(typeof(CheckColorMethodEnum), valuestring, true);
                    //    break;
                    case "IsNG":
                        IsNG = bool.Parse(valuestring);
                        break;
                    case "DirtRatio":
                        DirtRatio = float.Parse(valuestring);
                        break;
                    case "DirtArea":
                        DirtArea = int.Parse(valuestring);
                        break;
                    //case "ColorRatio":
                    //    ColorRatio = float.Parse(valuestring);
                    //    break;
                    //case "TotalColorRatio":
                    //    TotalColorRatio = float.Parse(valuestring);
                    //    break;
                }
            }
            else if (str[0] == "LTCorner")
            {
                switch (str[1])
                {
                    case "Brightness":
                        CornerArray[(int)CornerEnum.LT].Brightness = int.Parse(valuestring);
                        break;
                    case "Contrast":
                        CornerArray[(int)CornerEnum.LT].Contrast = int.Parse(valuestring);
                        break;
                    case "Width":
                        CornerArray[(int)CornerEnum.LT].Width = int.Parse(valuestring);
                        break;
                    case "Height":
                        CornerArray[(int)CornerEnum.LT].Height = int.Parse(valuestring);
                        break;
                    case "Tolerance":
                        CornerArray[(int)CornerEnum.LT].Tolerance = float.Parse(valuestring);
                        break;
                }
            }
            else if (str[0] == "RTCorner")
            {
                switch (str[1])
                {
                    case "Brightness":
                        CornerArray[(int)CornerEnum.RT].Brightness = int.Parse(valuestring);
                        break;
                    case "Contrast":
                        CornerArray[(int)CornerEnum.RT].Contrast = int.Parse(valuestring);
                        break;
                    case "Width":
                        CornerArray[(int)CornerEnum.RT].Width = int.Parse(valuestring);
                        break;
                    case "Height":
                        CornerArray[(int)CornerEnum.RT].Height = int.Parse(valuestring);
                        break;
                    case "Tolerance":
                        CornerArray[(int)CornerEnum.RT].Tolerance = float.Parse(valuestring);
                        break;
                }
            }
            else if (str[0] == "LBCorner")
            {
                switch (str[1])
                {
                    case "Brightness":
                        CornerArray[(int)CornerEnum.LB].Brightness = int.Parse(valuestring);
                        break;
                    case "Contrast":
                        CornerArray[(int)CornerEnum.LB].Contrast = int.Parse(valuestring);
                        break;
                    case "Width":
                        CornerArray[(int)CornerEnum.LB].Width = int.Parse(valuestring);
                        break;
                    case "Height":
                        CornerArray[(int)CornerEnum.LB].Height = int.Parse(valuestring);
                        break;
                    case "Tolerance":
                        CornerArray[(int)CornerEnum.LB].Tolerance = float.Parse(valuestring);
                        break;
                }
            }
            else if (str[0] == "RBCorner")
            {
                switch (str[1])
                {
                    case "Brightness":
                        CornerArray[(int)CornerEnum.RB].Brightness = int.Parse(valuestring);
                        break;
                    case "Contrast":
                        CornerArray[(int)CornerEnum.RB].Contrast = int.Parse(valuestring);
                        break;
                    case "Width":
                        CornerArray[(int)CornerEnum.RB].Width = int.Parse(valuestring);
                        break;
                    case "Height":
                        CornerArray[(int)CornerEnum.RB].Height = int.Parse(valuestring);
                        break;
                    case "Tolerance":
                        CornerArray[(int)CornerEnum.RB].Tolerance = float.Parse(valuestring);
                        break;
                }
            }
            else if (str[0] == "XDIR Measure")
            {
                switch (str[1])
                {
                    case "Brightness":
                        WHArray[(int)PositionEnum.XDir].Brightness = int.Parse(valuestring);
                        break;
                    case "Contrast":
                        WHArray[(int)PositionEnum.XDir].Contrast = int.Parse(valuestring);
                        break;
                    case "SampleGap":
                        WHArray[(int)PositionEnum.XDir].SampleGap = int.Parse(valuestring);
                        break;
                    case "ThresholdRatio":
                        WHArray[(int)PositionEnum.XDir].ThresholdRatio = float.Parse(valuestring);
                        break;
                    case "RangeRatio":
                        WHArray[(int)PositionEnum.XDir].RangeRatio = float.Parse(valuestring);
                        break;
                    case "Diff":
                        WHArray[(int)PositionEnum.XDir].Diff = float.Parse(valuestring);
                        break;
                }
            }
            else if (str[0] == "YDIR Measure")
            {
                switch (str[1])
                {
                    case "Brightness":
                        WHArray[(int)PositionEnum.YDir].Brightness = int.Parse(valuestring);
                        break;
                    case "Contrast":
                        WHArray[(int)PositionEnum.YDir].Contrast = int.Parse(valuestring);
                        break;
                    case "SampleGap":
                        WHArray[(int)PositionEnum.YDir].SampleGap = int.Parse(valuestring);
                        break;
                    case "ThresholdRatio":
                        WHArray[(int)PositionEnum.YDir].ThresholdRatio = float.Parse(valuestring);
                        break;
                    case "RangeRatio":
                        WHArray[(int)PositionEnum.YDir].RangeRatio = float.Parse(valuestring);
                        break;
                    case "Diff":
                        WHArray[(int)PositionEnum.YDir].Diff = float.Parse(valuestring);
                        break;
                }
            }
        }
        public void Reset()
        {
            AOIMethod = AOIMethodEnum.NONE; //AOI項目
            CheckDirtMethod = CheckDirtMethodEnum.NONE;          //指定檢查髒污的方法
            CheckColorMethod = UselessMethodEnum.NONE;       //指定鍵帽顏色的方法

            IsNG = false;               //是否為NG的圖形

            DirtRatio = 30f;           //髒污的對比
            DirtArea = 10;               //最小髒污

            ColorRatio = 70f;          //顏色對比
            TotalColorRatio = 70f;     //顏色比例

            foreach(CornerClass corner in CornerArray)
            {
                corner.Reset();
            }
            foreach(WHClass wh in WHArray)
            {
                wh.Reset();
            }
        }
        /// <summary>
        /// 取得顯示相關的方塊
        /// </summary>
        /// <param name="rectffrom"></param>
        /// <param name="showmover"></param>
        public void GetShowMovers(RectangleF rectffrom,Mover showmover,Bitmap bmpshowpattern,Bitmap bmpshowmask)
        {
            int i = 0;
            i = 0;
            
            while (i < (int)CornerEnum.COUNT)
            {
                if (CornerArray[i].IsNeedToCheck)
                {
                    JzRectEAG jzrect;

                    RectangleF rectf = CornerArray[i].GetCornerData((PositionEnum)(i + 2), rectffrom.Location, rectffrom.Size, bmpshowpattern);
                    jzrect = new JzRectEAG(Color.FromArgb(30, Color.OrangeRed), rectf, true);
                    showmover.Add(jzrect);
                }
                i++;
            }

            i = 0;
            while (i < (int)PositionEnum.DIRCOUNT)
            {
                if (WHArray[i].IsNeedToCheck)
                {
                    JzRectEAG jzrect;
                    RectangleF rectf = WHArray[i].GetWHData((PositionEnum)i, rectffrom.Location, rectffrom.Size, bmpshowpattern, bmpshowmask);

                    jzrect = new JzRectEAG(Color.FromArgb(30, Color.OrangeRed), rectf, true);
                    showmover.Add(jzrect);

                    //Added XY DIR Measure


                }
                i++;
            }
        }
        public bool A00_GetAOIData(Bitmap bmpinput, Bitmap bmpmask, string relateanalyzestr,PassInfoClass passinfo,float resolution)
        {
            bool isgood = true;
            int i = 0;

            RelateAnalyzeString = relateanalyzestr;
            //RelateAnalyzeInformation = relateanalyzeinformation;
            PassInfo = new PassInfoClass(passinfo, OPLevelEnum.COPY);

            #region Get Corner Data
            i = 0;
            while (i < (int)CornerEnum.COUNT)
            {
                if (CornerArray[i].IsNeedToCheck)
                {
                    //CornerArray[i].ResetTrainStatus();
                    isgood &= CornerArray[i].GetCornerData((CornerEnum)(i), bmpinput, bmpmask, relateanalyzestr, passinfo);
                    CornerArray[i].FillTrainStatus(TrainStatusCollection);
                }

                if (!isgood)
                    break;

                i++;
            }
            #endregion

            #region Get Width Height Origin Data

            if (isgood)
            {
                i = 0;
                while (i < (int)PositionEnum.DIRCOUNT)
                {
                    if (WHArray[i].IsNeedToCheck)
                    {
                        //WHArray[i].ResetTrainStatus();
                        WHArray[i].GetWHData((PositionEnum)i, bmpinput, bmpmask, relateanalyzestr, PassInfo, resolution);
                        WHArray[i].FillTrainStatus(TrainStatusCollection);
                    }
                    i++;
                }
            }
            #endregion


            return isgood;
        }

        public bool A08_RunAOIData(Bitmap bmpinput,bool istrain)
        {
            bool isgood = true;
            int i = 0;

            #region Get Corner Data
            i = 0;
            while (i < (int)CornerEnum.COUNT)
            {
                if (CornerArray[i].IsNeedToCheck)
                {
                    //if (istrain)
                    //    CornerArray[i].ResetTrainStatus();
                    //else
                    //    CornerArray[i].ResetRunStatus();

                    isgood &= CornerArray[i].RunCornerData(bmpinput, istrain);

                    if (istrain)
                        CornerArray[i].FillTrainStatus(TrainStatusCollection, null);
                    else
                        CornerArray[i].FillRunStatus(RunStatusCollection);
                }

                if (!isgood)
                    break;

                i++;
            }
            #endregion

            #region Get Width Height Origin Data

            if (isgood)
            {                
                i = 0;
                while (i < (int)PositionEnum.DIRCOUNT)
                {
                    if (WHArray[i].IsNeedToCheck)
                    {
                        //if (istrain)
                        //    WHArray[i].ResetTrainStatus();
                        //else
                        //    WHArray[i].ResetRunStatus();

                        isgood &= WHArray[i].RunWHData(bmpinput, istrain);

                        if (istrain)
                            WHArray[i].FillTrainStatus(TrainStatusCollection, null);
                        else
                            WHArray[i].FillRunStatus(RunStatusCollection);
                    }

                    i++;
                }
            }

            #endregion

            return isgood;
        }

        public void Suicide()
        {
            bmpPattern.Dispose();
            bmpDirt.Dispose();
            bmpInput.Dispose();

            foreach(CornerClass corner in CornerArray)
            {
                corner.Suicide();
            }
            foreach(WHClass wh in WHArray)
            {
                wh.Suicide();
            }

            TrainStatusCollection.Clear();
            RunStatusCollection.Clear();

        }
        /// <summary>
        /// 在做大量運算前要清除的相關資料
        /// </summary>
        public void ResetTrainStatus()
        {
            TrainStatusCollection.Clear();

            foreach(CornerClass corner in CornerArray)
            {
                corner.ResetTrainStatus();
            }
            foreach(WHClass wh in WHArray)
            {
                wh.ResetTrainStatus();
            }

        }
        /// <summary>
        /// 在做大量運算前要清除的相關資料
        /// </summary>
        public void ResetRunStatus()
        {
            RunStatusCollection.Clear();

            foreach (CornerClass corner in CornerArray)
            {
                corner.ResetRunStatus();
            }
            foreach (WHClass wh in WHArray)
            {
                wh.ResetRunStatus();
            }
        }
        /// <summary>
        /// 將產生出來的過程寫出去
        /// </summary>
        /// <param name="processstringlist"></param>
        /// <param name="runstatuslist"></param>
        public void FillRunStatus(WorkStatusCollectionClass runstatuscollection)
        {
            foreach (WorkStatusClass runstatus in RunStatusCollection.WorkStatusList)
            {
                runstatuscollection.Add(runstatus);
            }
        }
        public void FillRunStatus(WorkStatusCollectionClass runstatuscollection, string filltoanalyzestring)
        {
            foreach (WorkStatusClass runstatus in RunStatusCollection.WorkStatusList)
            {
                if (runstatus.LogString.IndexOf(filltoanalyzestring) < 0)
                {
                    runstatus.LogString += filltoanalyzestring;
                    runstatuscollection.Add(runstatus);
                }
            }

            //foreach (CornerClass corner in CornerArray)
            //{
            //    foreach (WorkStatusClass trainstatus in corner.TrainStatusCollection.WorkStatusList)
            //    {
            //        if (trainstatus.LogString.IndexOf(filltoanalyzestring) < 0)
            //        {
            //            trainstatus.LogString += filltoanalyzestring;
            //        }
            //    }
            //}
            //foreach (WHClass wh in WHArray)
            //{
            //    foreach (WorkStatusClass trainstatus in wh.TrainStatusCollection.WorkStatusList)
            //    {
            //        if (trainstatus.LogString.IndexOf(filltoanalyzestring) < 0)
            //        {
            //            trainstatus.LogString += filltoanalyzestring;
            //        }
            //    }
            //}
        }
        /// <summary>
        /// 將產生出來的過程寫出去
        /// </summary>
        /// <param name="runstatuscollection"></param>
        public void FillTrainStatus(WorkStatusCollectionClass trainstatuscollection,string filltoanalyzestring)
        {
            foreach (WorkStatusClass trainstatus in TrainStatusCollection.WorkStatusList)
            {
                if (trainstatus.LogString.IndexOf(filltoanalyzestring) < 0)
                {
                    trainstatus.LogString += filltoanalyzestring;
                    trainstatuscollection.Add(trainstatus);
                }
            }

            //foreach (CornerClass corner in CornerArray)
            //{
            //    foreach (WorkStatusClass trainstatus in corner.TrainStatusCollection.WorkStatusList)
            //    {
            //        if (trainstatus.LogString.IndexOf(filltoanalyzestring) < 0)
            //        {
            //            trainstatus.LogString += filltoanalyzestring;
            //        }
            //    }
            //}
            //foreach (WHClass wh in WHArray)
            //{
            //    foreach (WorkStatusClass trainstatus in wh.TrainStatusCollection.WorkStatusList)
            //    {
            //        if (trainstatus.LogString.IndexOf(filltoanalyzestring) < 0)
            //        {
            //            trainstatus.LogString += filltoanalyzestring;
            //        }
            //    }
            //}
        }

        public void FillTrainMessage(ref string processmessage, string filltoanalyzestring)
        {
            foreach(WorkStatusClass trainstatus in TrainStatusCollection.WorkStatusList)
            {
                if (trainstatus.LogString.IndexOf(filltoanalyzestring) < 0)
                {
                    trainstatus.LogString += filltoanalyzestring;
                    processmessage += trainstatus.ProcessString;
                }
            }

            //foreach (CornerClass corner in CornerArray)
            //{
            //    foreach (WorkStatusClass trainstatus in corner.TrainStatusCollection.WorkStatusList)
            //    {
            //        if (trainstatus.LogString.IndexOf(filltoanalyzestring) < 0)
            //        {
            //            trainstatus.LogString += filltoanalyzestring;
            //        }
            //    }
            //}
            //foreach (WHClass wh in WHArray)
            //{
            //    foreach (WorkStatusClass trainstatus in wh.TrainStatusCollection.WorkStatusList)
            //    {
            //        if (trainstatus.LogString.IndexOf(filltoanalyzestring) < 0)
            //        {
            //            trainstatus.LogString += filltoanalyzestring;
            //        }
            //    }
            //}
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
        /// <summary>
        /// 檢查Dirty 是否正確，
        /// 不能使用原圖來相減．因為後來的圖上的東西可能會偏移，造成MASK無法包含兩者的物體
        /// </summary>
        /// <param name="bmpinput"></param>
        /// <param name="bmpmask"></param>
        /// <returns></returns>
        public bool CheckDirt(Bitmap bmpinput, Bitmap bmpmask, bool istrain,PassInfoClass passinfo)
        {
            bool isgood = true;

            WorkStatusClass workstatus = new WorkStatusClass(AnanlyzeProcedureEnum.CHECKMEAN);
            string processstring = "Start " + RelateAnalyzeString + " Check Mean Grade Ratio." + Environment.NewLine;
            string errorstring = "";
            ReasonEnum reason = ReasonEnum.PASS;
            
            bmpDirt.Dispose();
            bmpDirt = new Bitmap(bmpinput);

            myHistogram.GetHistogram(bmpDirt, bmpmask, false);

            //bmpDirt.Save(Universal.TESTPATH + "\\ANALYZETEST\\HISTOGRAM " + RelateAnalyzeString + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

            if (istrain)
            {
                DirtPassInfo = new PassInfoClass(passinfo, OPLevelEnum.COPY);

                processstring = "Start " + RelateAnalyzeString + " Get Mean Grade Ratio." + Environment.NewLine;

                bmpPattern.Dispose();
                bmpPattern = new Bitmap(bmpDirt);

                OrgDirtMean = myHistogram.MeanGrade;
                RunDirtMean = OrgDirtMean;

                OrgDirtMean = Math.Max(1, OrgDirtMean);
                RunDirtMean = Math.Max(1, RunDirtMean);

                processstring += RelateAnalyzeString + " Get Origin Mean Grade " + OrgDirtMean.ToString("0.000") + Environment.NewLine;

                workstatus.SetWorkStatus(bmpPattern, bmpDirt, bmpPattern, reason, errorstring, processstring, DirtPassInfo);
                TrainStatusCollection.Add(workstatus);
            }
            else
            {
                bmpInput.Dispose();
                bmpInput = new Bitmap(bmpinput);

                RunDirtMean = myHistogram.MeanGrade;
                RunDirtMean = Math.Max(1, RunDirtMean);

                processstring += "Checking " + RelateAnalyzeString + " Mean Grade." + Environment.NewLine;
                
                switch (CheckDirtMethod)
                {
                    case CheckDirtMethodEnum.ALLCHECK:
                        if (Math.Abs(OrgDirtMean - RunDirtMean) / OrgDirtMean > DirtRatio / 100)
                        {
                            processstring += "Checking " + RelateAnalyzeString + " Mean Grade not in Ratio " + DirtRatio.ToString("0.00") + "%." + Environment.NewLine;
                            errorstring += RelateAnalyzeString + " Mean Grade not in Ratio " + DirtRatio.ToString("0.00") + "%." + Environment.NewLine;
                            reason = ReasonEnum.NG;

                            isgood = false;
                        }
                        break;
                    case CheckDirtMethodEnum.OMMITDARK:
                        if ((RunDirtMean - OrgDirtMean) / OrgDirtMean > DirtRatio / 100)
                        {
                            processstring += "Checking " + RelateAnalyzeString + " Mean Grade not in Ratio " + DirtRatio.ToString("0.00") + "%." + Environment.NewLine;
                            errorstring += RelateAnalyzeString + " Mean Grade not in Ratio " + DirtRatio.ToString("0.00") + "%." + Environment.NewLine;
                            reason = ReasonEnum.NG;

                            isgood = false;
                        }
                        break;
                    case CheckDirtMethodEnum.OMMITBRIGHT:
                        if ((OrgDirtMean - RunDirtMean) / OrgDirtMean > DirtRatio / 100)
                        {
                            processstring += "Checking " + RelateAnalyzeString + " Mean Grade not in Ratio " + DirtRatio.ToString("0.00") + "%." + Environment.NewLine;
                            errorstring += RelateAnalyzeString + " Mean Grade not in Ratio " + DirtRatio.ToString("0.00") + "%." + Environment.NewLine;
                            reason = ReasonEnum.NG;

                            isgood = false;
                        }
                        break;
                }

                workstatus.SetWorkStatus(bmpPattern, bmpDirt, bmpPattern, reason, errorstring, processstring, DirtPassInfo);
                RunStatusCollection.Add(workstatus);
            }

            if (isgood)
            {
                WorkStatusClass checkdirtworkstatus = new WorkStatusClass(AnanlyzeProcedureEnum.CHECKDIRT);
                processstring = "Start " + RelateAnalyzeString + " Check Dirt Area." + Environment.NewLine;
                errorstring = "";

                reason = ReasonEnum.PASS;

                bmpDirt.Dispose();
                bmpDirt = new Bitmap(bmpinput);

                float ratio = RunDirtMean * DirtRatio / 100;

                switch (CheckDirtMethod)
                {
                    case CheckDirtMethodEnum.ALLCHECK:
                        JzFind.SetThreshold(bmpDirt, bmpmask, 0, (int)RunDirtMean, (int)ratio, (int)ratio, false);
                        break;
                    case CheckDirtMethodEnum.OMMITDARK:
                        JzFind.SetThreshold(bmpDirt, bmpmask, 0, (int)RunDirtMean, (int)ratio, 255, false);
                        break;
                    case CheckDirtMethodEnum.OMMITBRIGHT:
                        JzFind.SetThreshold(bmpDirt, bmpmask, 0, (int)RunDirtMean, 255, (int)ratio, false);
                        break;
                }

                JzFind.Find(bmpDirt, Color.Red);

                //bmpDirt.Save(Universal.TESTPATH + "\\ANALYZETEST\\HISTOGRAM " + RelateAnalyzeString + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                int GetMaxArea = JzFind.GetMaxArea();

                if (GetMaxArea > DirtArea)
                {
                    processstring += "Check " + RelateAnalyzeString + " Check Dirt " + GetMaxArea.ToString() + " > " + DirtArea.ToString() + "." + Environment.NewLine;
                    errorstring += RelateAnalyzeString + " Check Dirt " + GetMaxArea.ToString() + " > " + DirtArea.ToString() + "." + Environment.NewLine;
                    reason = ReasonEnum.NG;

                    foreach(FoundClass found in JzFind.FoundList)
                    {
                        DrawRect(bmpDirt, found.rect, new Pen(Color.Lime, 3), 10);
                    }

                    //bmpDirt.Save(Universal.TESTPATH + "\\ANALYZETEST\\DIRT " + RelateAnalyzeString + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                    isgood = false;
                }
                else
                {
                    processstring += "Check " + RelateAnalyzeString + " Check Dirt " + GetMaxArea.ToString() + " <= " + DirtArea.ToString() + "." + Environment.NewLine;
                }

                checkdirtworkstatus.SetWorkStatus(bmpPattern, bmpInput, bmpDirt, reason, errorstring, processstring, DirtPassInfo);

                if (istrain)
                    TrainStatusCollection.Add(checkdirtworkstatus);
                else
                    RunStatusCollection.Add(checkdirtworkstatus);
            }


            return isgood;
        }

        #region Tools Operation

        void DrawRect(Bitmap BMP, Rectangle Rect, Pen RoundPen, int Enlarge)
        {
            DrawRect(BMP, new Rectangle(Rect.X - Enlarge, Rect.Y - Enlarge, ((int)Rect.Width) + (Enlarge << 1), ((int)Rect.Height) + (Enlarge << 1)), RoundPen);
        }
        void DrawRect(Bitmap BMP, Rectangle Rect, Pen RoundPen)
        {
            Graphics g = Graphics.FromImage(BMP);
            g.DrawRectangle(RoundPen, Rect);
            g.Dispose();
        }
        void DrawImage(Bitmap bmpfrom, Bitmap bmpto, Rectangle destrect)
        {
            Graphics g = Graphics.FromImage(bmpto);
            g.DrawImage(bmpfrom, destrect, SimpleRect(bmpfrom.Size), GraphicsUnit.Pixel);
            g.Dispose();
        }
        Rectangle SimpleRect(Size sz)
        {
            return new Rectangle(0, 0, sz.Width, sz.Height);
        }

        Rectangle RectFToRect(RectangleF rectf)
        {
            Rectangle rect = new Rectangle((int)rectf.X, (int)rectf.Y, (int)rectf.Width, (int)rectf.Height);

            return rect;
        }
        #endregion



    }
}
