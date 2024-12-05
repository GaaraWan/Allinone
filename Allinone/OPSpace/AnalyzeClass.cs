//#define MULTI

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;

using JetEazy;
using JetEazy.BasicSpace;
using JzDisplay;
using MoveGraphLibrary;
using WorldOfMoveableObjects;
using Allinone.OPSpace.AnalyzeSpace;
using AUVision;
using JzASN.OPSpace;
using PdfSharp.Drawing;

namespace Allinone.OPSpace
{
    [Serializable]
    public class AnalyzeClass
    {
        /// <summary>
        /// 作為所有的對應位數值
        /// </summary>
        public static string ORGANALYZENOSTRING = "0000";
        public static string ORGLEARNSTRING = "000";

        Bitmap bmpINPUT = new Bitmap(1, 1);
        public Bitmap bmpWIP = new Bitmap(1, 1);           //Bitmp Work In Process
        public Bitmap bmpWIP_1 = new Bitmap(1, 1);           //Bitmp Work In Process MAIN_SDM1 use 矽品科技使用

        public Bitmap bmpPATTERN = new Bitmap(1, 1);

        public Bitmap bmpMASK = new Bitmap(1, 1);
        AUGrayImg8 ImgMASK = new AUGrayImg8();

        Bitmap bmpHOLLOW = new Bitmap(1, 1);        //若Hollow的長寬都大於1，則代表必需要挖洞

        public Bitmap bmpORGLEARNININPUT;    //儲存最原始的 INPUT 讓LEARNING去用
        public Bitmap bmpOUTPUT = new Bitmap(1, 1);

        Color DefaultColor = Color.FromArgb(0, Color.Red);
        Color DefaultRingColor = Color.FromArgb(60, Color.Red);

        SolidBrush WhiteMaskBrush = new SolidBrush(Color.White);
        SolidBrush BlackMaskBrush = new SolidBrush(Color.Black);
        Size nullSize = new Size();
        Point myAlignOffsetCal = new Point(0, 0);
        bool myAlignOffsetChange = false;
        public PointF myOringinOffsetPointF = new PointF(0, 0);   //本圖最左上角相對原點(0,0)的位置
        VersionEnum VERSION
        {
            get
            {
                return Universal.VERSION;
            }
        }
        OptionEnum OPTION
        {
            get
            {
                return Universal.OPTION;
            }
        }

        #region Basic Data
        //Field
        public int PageNo = 0;
        public PageOPTypeEnum PageOPtype = PageOPTypeEnum.P00;
        public int No = 0;
        public int ParentNo = 0;                //需要的，要給Table去用的
        public AnalyzeTypeEnum AnalyzeType = AnalyzeTypeEnum.BRANCH;    //好像沒有用了，因為有 LearnNo 了
        public int Level = 0;
        public bool IsLearnRoot = false;        //好像沒有用了，因為 LearnNo = 0 就是 LearRoot
        public int LearnNo = 0;                 //LearnNo 預設值已定義為 0
        public RectangleF LearnOrigionOffsetRectf = new RectangleF();

        public RectangleF mySeedRectF = new RectangleF();
        public PointF mySeedOffset = new PointF();
        public RectangleF myDrawAnalyzeStrRectF = new RectangleF();

        public int ParentLearnNo = 0;           //好像沒有用了，因為有NodeString了
        public string FromNodeString = "";      //分支的節點

        public string ReportRowCol = "";//自动编号mapping 行列标志
        public int ReportIndex = 0;
        /// <summary>
        /// 强制通过检测，即不检测
        /// </summary>
        public bool IsByPass = false;
        public int DataReportIndex = 0;

        /// <summary>
        /// 設定檢查的2D條碼
        /// </summary>
        private string m_barcode_2D = string.Empty;
        /// <summary>
        /// 設定檢查的2D條碼
        /// </summary>
        public string Barcode_2D
        {
            get { return m_barcode_2D; }
            set { m_barcode_2D = value; }
        }
        /// <summary>
        /// 讀取返回的2D條碼
        /// </summary>
        public string ReadBarcode2DStr = string.Empty;
        public string ReadBarcode2DRealStr = string.Empty;
        public string ReadBarcode2DGrade = string.Empty;

        public Mover myMover = new Mover();

        public List<AnalyzeClass> LearnList = new List<AnalyzeClass>();
        /// <summary>
        /// 子框内容
        /// </summary>
        public List<AnalyzeClass> BranchList = new List<AnalyzeClass>();

        public List<int> PrepareRemovalNoList = new List<int>();    //預備刪除的學習編號
        //public List<RectangleF> CurrentRelationPositionRectF = new List<RectangleF>();
        string NoSaveStr
        {
            get
            {
                return No.ToString("000");
            }
        }

        #region Show For Analyze

        public string AliasName = "";

        public int Brightness = 0;  //所有的定位都是先加強後定位，然後再定位
        public int Contrast = 0;    //所有的加強都是利用最原始的圖來加強，而不會繼承加強的圖像

        public MaskMethodEnum MaskMethod = MaskMethodEnum.NONE; //指定為 Mask 的方式
        public int ExtendX = 20;    //X 外擴
        public int ExtendY = 20;    //Y 外擴

        public string RelateASN = "";
        public string RelateASNItem = "";

        public bool IsSeed = false;//种子参数

        public NORMALClass NORMALPara = new NORMALClass();

        /*
        public string AliasName = "";

        public int Brightness = 0;  //所有的定位都是先加強後定位，然後再定位
        public int Contrast = 0;    //所有的加強都是利用最原始的圖來加強，而不會繼承加強的圖像

        public bool IsMask = false; //是否做為 Mask
        public int ExtendX = 20;
        public int ExtendY = 20;
        */
        #region Alignment Factors
        public ALIGNClass ALIGNPara = new ALIGNClass();
        /*
        public AlignMethodEnum AlignMethod = AlignMethodEnum.AUFIND;   //採用哪類Align方案
        public int MTPSize = 35;            //Pyramid Size
        public int MTCannyH = 200;          //Canny H. Threshold
        public int MTCannyL = 128;          //Canny L. Threshold
        public float MTRotation = 20f;      //Rotation From -20 to 20 degree
        public float MTScaling = 0.1f;      //Scaling From  90%-110%
        public int MTMaxOcc = 1;            //Max Occ
        public float MTTolerance = 0.7f;    //Tolerance from 0.1 to 1.0
        public bool MTIsSubPixel = false;   //Subpixel
        public float MTOffset = 0f;         //Check Offset Value
        public float MTResolution = 0.01f;  //Resolution Value
        */
        #endregion
        #region Inspection Factors
        public INSPECTIONClass INSPECTIONPara = new INSPECTIONClass();
        /*
        public InspectionMethodEnum InspectionMethod = InspectionMethodEnum.NONE;
        public int IBCount = 3;
        public int IBArea = 5;
        public float IBTolerance = 20;
        */
        #endregion
        #region Measure Factors
        public MEASUREClass MEASUREPara = new MEASUREClass();
        /*
        public MeasureMethodEnum MeasureMethod = MeasureMethodEnum.NONE;
        public float MMTolerance = 10;
        public string MMOPString = "";
        public float MMMaxGap = 0f;
        public float MMMinGap = 0f;
        public float MMPixelGap = 5f;
        public float MMHTRatio = 10f;
        public float MMWholeRatio = 10f;
        */
        #endregion
        #region OCR & Barcode Factors
        public OCRCheckClass OCRPara = new OCRCheckClass();
        /*
        public OCRMethodEnum OCRMethod = OCRMethodEnum.NONE;
        public int OCRMappingIndex = 0;
        */
        #endregion

        #region KBAOI Factors
        public AOIClass AOIPara = new AOIClass();
        #endregion

        #region KBGap Factors
        public GAPClass GAPPara = new GAPClass();

        public List<Color> GapReasonsColors = new List<Color>();//量測機測試結果的錯誤的顏色
        #endregion

        #region KBHeight Factors
        public HEIGHTClass HEIGHTPara = new HEIGHTClass();

        public List<Color> HeightReasonsColors = new List<Color>();//鍵高機測試結果的錯誤的顏色
        #endregion

        public ASSEMBLEClass ASSEMBLE = new ASSEMBLEClass();

        #endregion


        public StiltsClass StiltsPara = new StiltsClass();

        #endregion

        #region PADCHECK

        public PADINSPECTClass PADPara = new PADINSPECTClass();

        #endregion

        #region Online Data

        //string SAVEPATH = "";
        public bool IsUsed = false;         //找樹枝狀時用的或是做為一個FLAG

        public bool IsVeryGood = true;      //此 Analyze 是否為 PASS 的
        public bool IsOperated = false;     //此 Analyze 是否有被操作過
        public bool IsTempSave = false;     //是否要暫時性的儲存
        int LastLearnIndex = 0;

        //public int EnvNo = -1;
        //public int RcpNo = -1;

        public PassInfoClass PassInfo = new PassInfoClass();

        public RectangleF myOPRectF = new RectangleF();

        public WorkStatusCollectionClass TrainStatusCollection = new WorkStatusCollectionClass();
        public WorkStatusCollectionClass RunStatusCollection = new WorkStatusCollectionClass();

        public bool IsSelected
        {
            get
            {
                bool isselected = false;
                int i = 0;

                GraphicalObject grobj;

                while (i < myMover.Count)
                {
                    grobj = myMover[i].Source;

                    if ((grobj as GeoFigure).IsSelected)
                    {
                        isselected = true;
                        break;
                    }
                    i++;
                }

                return isselected;
            }
        }

        public AnalyzeClass()
        {
            ASSEMBLE.ConstructProperty(VERSION, OPTION);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageno"></param>
        /// <param name="pageoptype"></param>
        public AnalyzeClass(int pageno, PageOPTypeEnum pageoptype)
        {
            #region Very First Use, This will No use when we debug!

            ////////////////////////////////////////////////////////////
            // Emergency Use Code
            ////////////////////////////////////////////////////////////

            ASSEMBLE.ConstructProperty(VERSION, OPTION);

            No = 1;
            Level = 1;
            PageNo = pageno;
            PageOPtype = pageoptype;
            AliasName = ToAnalyzeString();

            JzRectEAG jzrect = new JzRectEAG(DefaultColor);
            jzrect.RelateNo = No;
            jzrect.RelatePosition = 0;
            jzrect.RelateLevel = Level;

            myMover.Add(jzrect);


            ////////////////////////////////////////////////////////////
            #endregion
        }
        public AnalyzeClass(int pageno, PageOPTypeEnum pageoptype, PassInfoClass passinfo)
        {
            #region Very First Use, This will No use when we debug!

            ////////////////////////////////////////////////////////////
            // Emergency Use Code
            ////////////////////////////////////////////////////////////

            ASSEMBLE.ConstructProperty(VERSION, OPTION);

            No = 1;
            Level = 1;
            PageNo = pageno;
            PageOPtype = pageoptype;
            AliasName = ToAnalyzeString();

            JzRectEAG jzrect = new JzRectEAG(DefaultColor);
            jzrect.RelateNo = No;
            jzrect.RelatePosition = 0;
            jzrect.RelateLevel = Level;

            myMover.Add(jzrect);

            PassInfo = new PassInfoClass(passinfo, OPLevelEnum.ANALYZE);
            PassInfo.AnalyzeNo = No;

            ////////////////////////////////////////////////////////////
            #endregion
        }
        public AnalyzeClass(Rectangle rect)
        {
            ASSEMBLE.ConstructProperty(VERSION, OPTION);

            JzRectEAG jzrect = new JzRectEAG(DefaultColor, rect);

            myMover.Add(jzrect);
        }
        public AnalyzeClass(RectangleF rect, double angle)
        {
            ASSEMBLE.ConstructProperty(VERSION, OPTION);

            JzRectEAG jzrect = new JzRectEAG(DefaultColor, rect, angle);

            myMover.Add(jzrect);
        }

        #endregion
        public AnalyzeClass(string analyzestr, PassInfoClass passinfo)
        {
            ASSEMBLE.ConstructProperty(VERSION, OPTION);

            FromString(analyzestr);

            if (passinfo != null)
                PassInfo = new PassInfoClass(passinfo, OPLevelEnum.ANALYZE);

            ToPassInfo();

            PassInfo.AnalyzeNo = No;
        }
        /// <summary>
        /// 專供回復 Learn Analyze 用
        /// </summary>
        /// <param name="analyzestr"></param>
        /// <param name="operatepath"></param>
        public AnalyzeClass(string analyzestr, string operatepath)
        {
            ASSEMBLE.ConstructProperty(VERSION, OPTION);

            //FromString(analyzestr);
            //FromStringToComposeAnalyze(analyzestr);
            FromStringWithBranch(analyzestr);

            ToPassInfo();

            PassInfo.OperatePath = operatepath;
        }

        #region auto index row col

        void MainSD_AutoIndex()
        {
            //if (MessageBox.Show(SCREEN.Messages("msg1"), "MAIN", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int Highest = 100000;
                int HighestIndex = -1;
                int ReportIndex = 0;
                List<string> CheckList = new List<string>();

                int i = 0;

                //Clear All Index To 0 and Check the Highest

                foreach (AnalyzeClass keyassign in BranchList)
                {
                    keyassign.ReportRowCol = "";
                    ReportIndex = 1;
                }

                i = 0;
                while (true)
                {
                    i = 0;
                    Highest = 100000;
                    HighestIndex = -1;
                    foreach (AnalyzeClass keyassign in BranchList)
                    {
                        if (keyassign.ReportRowCol == "")
                        {
                            if (keyassign.myOPRectF.Y < Highest)
                            {
                                Highest = (int)keyassign.myOPRectF.Y;
                                HighestIndex = i;
                            }
                        }

                        i++;
                    }

                    if (HighestIndex == -1)
                        break;

                    CheckList.Clear();

                    //把相同位置的人找出來
                    i = 0;
                    foreach (AnalyzeClass keyassign in BranchList)
                    {
                        if (keyassign.ReportRowCol == "")
                        {
                            if (IsInRange((int)keyassign.myOPRectF.Y, Highest, 30))
                            {
                                CheckList.Add(keyassign.myOPRectF.X.ToString("0000") + "," + i.ToString());
                            }
                        }
                        i++;
                    }

                    CheckList.Sort();

                    foreach (string Str in CheckList)
                    {
                        string[] Strs = Str.Split(',');

                        //KEYBOARD.vKEYASSIGNLIST[int.Parse(Strs[1])].ReportIndex = ReportIndex;

                        ReportIndex++;
                    }
                }

            }
        }

        bool IsInRange(int FromValue, int CompValue, int DiffValue)
        {
            return (FromValue >= CompValue - DiffValue) && (FromValue <= CompValue + DiffValue);
        }


        /// <summary>
        /// 子框内容
        /// </summary>
        public List<AnalyzeClass> BackupBranchList = new List<AnalyzeClass>();

        public void KillInsideList(bool isall)
        {


            //foreach (MEASUREClass measure in InsideBackupMEASUREList)
            //{
            //    measure.Kill();
            //}

            //InsideBackupMEASUREList.Clear();

            BackupBranchList.Clear();

            if (isall)
            {
                //foreach (MEASUREClass measure in InsideMEASUREList)
                //{
                //    measure.Kill();
                //}

                //InsideMEASUREList.Clear();

                BranchList.Clear();
            }
        }
        public void BackupInsideList(AnalyzeClass seedregion)
        {
            KillInsideList(false);


            RectangleF seedrectf = seedregion.myOPRectF;
            RectangleF myrectf = myOPRectF;

            Point Bias = new Point((int)(myrectf.X - seedrectf.X), (int)(myrectf.Y - seedrectf.Y));

            //foreach (MEASUREClass measure in InsideMEASUREList)
            //{
            //    MEASUREClass backupmeasure = new MEASUREClass(measure);

            //    backupmeasure.MoveShape(Bias.X, Bias.Y);

            //    InsideBackupMEASUREList.Add(backupmeasure);
            //}

            foreach (AnalyzeClass analyze in BranchList)
            {
                AnalyzeClass backupanalyze = new AnalyzeClass();
                backupanalyze.From(analyze);

                backupanalyze.SetMoverOffset(Bias);

                BackupBranchList.Add(backupanalyze);
            }

        }
        public void RestoreInsideList(bool isseed)
        {
            if (!isseed)
            {
                //InsideMEASUREList = InsideBackupMEASUREList;
                //InsideTESTList = InsideBackupTESTList;
                //BackupBranchList = BranchList;
                BranchList = BackupBranchList;
            }
            else
            {
                int i = 0;

                while (i < BackupBranchList.Count)
                {
                    BranchList[i].From(BackupBranchList[i]);
                    i++;
                }


            }
        }

        public void From(AnalyzeClass analyze)
        {
            myMover.Clear();

            FromString(analyze.ToString());

            //IsVeryGood = false; //analyze.IsVeryGood;
            //IsOperated = false;// analyze.IsOperated;

            //IsVeryGood = analyze.IsVeryGood;
            //IsOperated = analyze.IsOperated;

            bmpWIP = (Bitmap)analyze.bmpWIP.Clone();
            bmpPATTERN = (Bitmap)analyze.bmpPATTERN.Clone();
            bmpOUTPUT = (Bitmap)analyze.bmpOUTPUT.Clone();
            bmpMASK = (Bitmap)analyze.bmpMASK.Clone();
            bmpINPUT = (Bitmap)analyze.bmpINPUT.Clone();

            //TrainStatusCollection = analyze.TrainStatusCollection;
            //RunStatusCollection = analyze.RunStatusCollection;

        }

        #endregion

        #region Alignment Parameter

        #endregion
        public AnalyzeClass GetLearnByIndex(int index)
        {
            if (index == 0)
                return this;

            return LearnList[index - 1];
        }

        public AnalyzeClass GetLastLearn()
        {
            if (this.LearnList.Count == 0)
                return null;
            else
                return this.LearnList[this.LearnList.Count - 1];
        }

        /// <summary>
        /// 不需要 Clone 內含模組的 PassInfo, 在 Train時會重新把所有模組的 PassInfo 填妥
        /// </summary>
        /// <returns></returns>
        public AnalyzeClass Clone()
        {
            return Clone(false);
        }
        /// <summary>
        /// 不需要 Clone 內含模組的 PassInfo, 在 Train時會重新把所有模組的 PassInfo 填妥
        /// </summary>
        /// <returns></returns>
        public AnalyzeClass Clone(bool islearnclone)
        {
            return Clone(new Point(0, 0), 0d, false, true, true, islearnclone);
        }
        /// <summary>
        /// 不需要 Clone PassInfo, 在 Train時會重新把所有的 PassInfo 填妥
        /// </summary>
        /// <param name="offsetpoint"></param>
        /// <param name="isclearorigin"></param>
        /// <param name="isbrachclone"></param>
        /// <returns></returns>
        public AnalyzeClass Clone(Point offsetpoint, double adddegree, bool isclearorigin, bool isbrachclone, bool isdeepclone, bool islearnclone)
        {
            AnalyzeClass newanalyze = new AnalyzeClass();

            string str = this.ToString();

            string[] strs = str.Replace(Environment.NewLine, Universal.NewlineChar.ToString()).Split(Universal.NewlineChar);

            newanalyze.FromString(strs[0]);
            newanalyze.SetMoverOffset(offsetpoint);
            newanalyze.SetMoverAngle(adddegree);

            //newanalyze.SAVEPATH = this.SAVEPATH;
            newanalyze.PassInfo = new PassInfoClass(this.PassInfo, OPLevelEnum.COPY);

            newanalyze.IsUsed = this.IsUsed;

            if (isdeepclone)
            {
                newanalyze.bmpWIP.Dispose();
                newanalyze.bmpWIP = new Bitmap(this.bmpWIP);

                newanalyze.bmpPATTERN.Dispose();
                newanalyze.bmpPATTERN = new Bitmap(this.bmpPATTERN);
                newanalyze.bmpMASK.Dispose();
                newanalyze.bmpMASK = new Bitmap(this.bmpMASK);
                //newanalyze.bmpHOLLOW.Dispose();
                //newanalyze.bmpHOLLOW = new Bitmap(this.bmpHOLLOW);
                //newanalyze.bmpOUTPUT.Dispose();
                //newanalyze.bmpOUTPUT = new Bitmap(this.bmpOUTPUT);


                //newanalyze.myOPRectF = this.myOPRectF;
                //newanalyze.myOringinOffsetPointF = this.myOringinOffsetPointF;
                //newanalyze.LearnOrigionOffsetRectf = this.LearnOrigionOffsetRectf;
            }

            newanalyze.myOPRectF = this.myOPRectF;
            newanalyze.myOringinOffsetPointF = this.myOringinOffsetPointF;
            newanalyze.LearnOrigionOffsetRectf = this.LearnOrigionOffsetRectf;

            if (isbrachclone)
            {
                foreach (AnalyzeClass branchanalzye in this.BranchList)
                {
                    if (isdeepclone)
                    {
                        AnalyzeClass newbranchanalyze = branchanalzye.Clone(offsetpoint, adddegree, isclearorigin, isbrachclone, isdeepclone, islearnclone);
                        newanalyze.BranchList.Add(newbranchanalyze);
                    }
                    else
                    {
                        AnalyzeClass newbranchanalyze = branchanalzye.Clone(offsetpoint, adddegree, isclearorigin, false, isdeepclone, islearnclone);
                        newanalyze.BranchList.Add(newbranchanalyze);
                    }

                }
            }

            if (islearnclone)
            {
                foreach (AnalyzeClass leanranalzye in this.LearnList)
                {
                    if (isdeepclone)
                    {
                        AnalyzeClass newlearnanalyze = leanranalzye.Clone(offsetpoint, adddegree, isclearorigin, isbrachclone, true, islearnclone);
                        newanalyze.LearnList.Add(newlearnanalyze);
                    }
                    else
                    {
                        AnalyzeClass newlearnanalyze = leanranalzye.Clone(offsetpoint, adddegree, isclearorigin, false, true, islearnclone);
                        newanalyze.LearnList.Add(newlearnanalyze);
                    }

                }
            }

            //newanalyze.TransferMoverSelect(this, isclearorigin, isdup);

            return newanalyze;
        }
        /// <summary>
        /// 包含圖片的Clone，並無Clone Learning
        /// </summary>
        /// <returns></returns>
        public AnalyzeClass DeepClone()
        {
            return Clone(new Point(0, 0), 0d, false, true, true, false);
        }


        /// <summary>
        /// 檢查包含進來的Analyze是否為已所用，已被CheckAnalyzeEX取代
        /// </summary>
        /// <param name="analyze"></param>
        /// <returns></returns>
        public bool CheckAnalyze(AnalyzeClass analyze)
        {
            bool ret = false;

            if (analyze.ParentNo == No && analyze.ParentLearnNo == LearnNo)
            {
                if (analyze.LearnNo == 0)
                {
                    BranchList.Add(analyze);

                    analyze.IsUsed = true;
                    ret = true;
                }
                else if (analyze.LearnNo > 0 && analyze.No == No)
                {
                    LearnList.Add(analyze);

                    analyze.IsUsed = true;
                    ret = true;
                }
            }
            else
            {
                foreach (AnalyzeClass branchanalyze in BranchList)
                {
                    if (branchanalyze.CheckAnalyze(analyze))
                    {
                        ret = true;
                        break;
                    }
                }
                if (!ret)
                {
                    foreach (AnalyzeClass learnanalyze in LearnList)
                    {
                        if (learnanalyze.CheckAnalyze(analyze))
                        {
                            ret = true;
                            break;
                        }
                    }
                }
            }

            return ret;
        }
        /// <summary>
        /// 檢查包含進來的Analyze是否為己所用
        /// </summary>
        /// <param name="analyze"></param>
        /// <returns></returns>
        public bool CheckAnalyzeEX(AnalyzeClass analyze)
        {
            bool ret = false;

            string tonextnodestring = ToNextNodeString();

            //if (analyze.AnalyzeType == AnalyzeTypeEnum.LEARNING)
            //    ret = ret;

            if (analyze.FromNodeString == tonextnodestring && analyze.LearnNo == 0)
            {
                //if (analyze.LearnNo == 0)
                {
                    BranchList.Add(analyze);

                    analyze.IsUsed = true;
                    ret = true;
                }
            }
            else if (analyze.FromNodeString == tonextnodestring && analyze.LearnNo > 0)
            {
                LearnList.Add(analyze);

                analyze.IsUsed = true;
                ret = true;
            }
            else
            {
                foreach (AnalyzeClass branchanalyze in BranchList)
                {
                    if (branchanalyze.CheckAnalyzeEX(analyze))
                    {
                        ret = true;
                        break;
                    }
                }
                if (!ret)
                {
                    foreach (AnalyzeClass learnanalyze in LearnList)
                    {
                        if (learnanalyze.CheckAnalyzeEX(analyze))
                        {
                            ret = true;
                            break;
                        }
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// 合併後檢查包含進來的Analyze是否為己所用
        /// </summary>
        /// <param name="analyze"></param>
        /// <returns></returns>
        public bool CheckAnalyzeFX(AnalyzeClass analyze)
        {
            if (analyze.IsUsed)
                return false;

            bool ret = false;

            string tonextnodestring = ToNextNodeString();

            if (analyze.ParentNo == this.No)
            {
                analyze.FromNodeString = this.ToNextNodeString();
                analyze.ToPassInfo();
                analyze.IsUsed = true;

                BranchList.Add(analyze);

                ret = true;
            }

            if (!ret)
            {
                foreach (AnalyzeClass branchanalyze in BranchList)
                {
                    ret |= branchanalyze.CheckAnalyzeFX(analyze);
                }
            }

            return ret;
        }

        public void ResetAnalyzeBarcodeStr()
        {
            if (OCRPara.OCRMethod == OCRMethodEnum.DATAMATRIX)
            {
                ReadBarcode2DRealStr = string.Empty;
                return;
            }
            if (OCRPara.OCRMethod == OCRMethodEnum.DATAMATRIXGRADE)
            {
                ReadBarcode2DRealStr = string.Empty;
                ReadBarcode2DGrade = string.Empty;
                return;
            }
            foreach (AnalyzeClass analyzeClass in BranchList)
            {
                analyzeClass.ResetAnalyzeBarcodeStr();
            }
        }
        public void SetAnalyzeCheckBarcodeStr(string eBarcodeStr)
        {
            if (OCRPara.OCRMethod == OCRMethodEnum.DATAMATRIX)
            {
                m_barcode_2D = eBarcodeStr;
                //return;
            }
            if (OCRPara.OCRMethod == OCRMethodEnum.DATAMATRIXGRADE)
            {
                m_barcode_2D = eBarcodeStr;
                //return;
            }
            foreach (AnalyzeClass analyzeClass in BranchList)
            {
                analyzeClass.SetAnalyzeCheckBarcodeStr(eBarcodeStr);
            }
        }
        public string GetAnalyzeBarcodeStr()
        {
            //if (IsByPass)
            //    return "不检测";
            if (OCRPara.OCRMethod == OCRMethodEnum.DATAMATRIX)
            {
                return ReadBarcode2DRealStr;
            }
            if (OCRPara.OCRMethod == OCRMethodEnum.DATAMATRIXGRADE)
            {
                string tempstr = $"No Compare;{Environment.NewLine}{ReadBarcode2DRealStr};{Environment.NewLine}{ReadBarcode2DGrade}";
                if (INI.IsCheckBarcodeOpen)
                {
                    if (string.IsNullOrEmpty(ReadBarcode2DRealStr))
                        tempstr = $"Compare [FAIL];{Environment.NewLine}Marking 2D[{Barcode_2D}];{Environment.NewLine}Read 2D[{ReadBarcode2DRealStr}];{Environment.NewLine}Grade[{ReadBarcode2DGrade}]";
                    else
                        tempstr = $"Compare [{(ReadBarcode2DRealStr == Barcode_2D ? "PASS" : "FAIL")}];{Environment.NewLine}Marking 2D[{Barcode_2D}];{Environment.NewLine}Read 2D[{ReadBarcode2DRealStr}];{Environment.NewLine}Grade[{ReadBarcode2DGrade}]";
                }
                return tempstr;
                //return ReadBarcode2DRealStr + ";" + ReadBarcode2DGrade;
            }
            foreach (AnalyzeClass analyzeClass in BranchList)
            {
                string _barcodeStr = analyzeClass.GetAnalyzeBarcodeStr();
                if (!string.IsNullOrEmpty(_barcodeStr))
                    return _barcodeStr;
            }
            return string.Empty;
        }
        /// <summary>
        /// 仅仅获取读到的条码
        /// </summary>
        /// <returns></returns>
        public string GetAnalyzeOnlyBarcodeStr()
        {
            if (OCRPara.OCRMethod == OCRMethodEnum.DATAMATRIX)
            {
                return ReadBarcode2DRealStr;
            }
            if (OCRPara.OCRMethod == OCRMethodEnum.DATAMATRIXGRADE)
            {
                return ReadBarcode2DRealStr;
            }
            foreach (AnalyzeClass analyzeClass in BranchList)
            {
                string _barcodeStr = analyzeClass.GetAnalyzeOnlyBarcodeStr();
                if (!string.IsNullOrEmpty(_barcodeStr))
                    return _barcodeStr;
            }
            return string.Empty;
        }
        public bool CheckAnalyzeReadBarcode()
        {
            if (OCRPara.OCRMethod != OCRMethodEnum.NONE)
            {
                return true;
            }
            foreach (AnalyzeClass analyzeClass in BranchList)
            {
                bool ret = analyzeClass.CheckAnalyzeReadBarcode();
                if (ret)
                    return true;
            }
            return false;
        }
        public bool CheckRepeatCode(List<string> eCodes)
        {
            if (OCRPara.OCRMethod == OCRMethodEnum.DATAMATRIXGRADE)
            {
                if (IsByPass)
                    return true;
                return OCRPara.CheckRepeatCode(eCodes, bmpPATTERN, bmpWIP, PassInfo);
            }
            foreach (AnalyzeClass analyzeClass in BranchList)
            {
                bool ret = analyzeClass.CheckRepeatCode(eCodes);
                if (ret)
                    return true;
            }
            return false;
        }
        public void SetAnalyzeByPass(bool ePass)
        {
            IsByPass = ePass;
            foreach (AnalyzeClass analyzeClass in BranchList)
            {
                analyzeClass.SetAnalyzeByPass(ePass);
            }
        }
        /// <summary>
        /// 获取框的错误类型
        /// </summary>
        /// <returns></returns>
        public int GetAnalyzeErrorType()
        {
            if (ALIGNPara.AlignMethod != AlignMethodEnum.NONE)
            {
                if (!ALIGNPara.CheckGood)
                    return 1;//定位错误
            }
            if (INSPECTIONPara.InspectionMethod != InspectionMethodEnum.NONE)
            {
                if (!INSPECTIONPara.IsPass)
                    return 2;//检测错误
            }
            if (MEASUREPara.MeasureMethod != MeasureMethodEnum.NONE)
            {
                if (!MEASUREPara.CheckGood)
                    return 3;//量测错误
            }
            if (PADPara.PADMethod != PADMethodEnum.NONE)
            {
                if (!PADPara.IsPass)
                    return 4;//表面或边角缺陷
            }
            foreach (AnalyzeClass analyzeClass in BranchList)
            {
                int index = analyzeClass.GetAnalyzeErrorType();
                if (index != 0)
                    return index;
            }
            return 0;
        }
        /// <summary>
        /// 获取当前框子框的相对位置 收集到list —— CurrentRelationPositionRectF
        /// </summary>
        public void GetBranchOffsetPosition()
        {
            //CurrentRelationPositionRectF.Clear();
            //foreach (AnalyzeClass analyzeClass in BranchList)
            //{
            //    PointF ptfOffset = new PointF(analyzeClass.myOPRectF.X - myOPRectF.X, analyzeClass.myOPRectF.Y - myOPRectF.Y);
            //    RectangleF myrect = new RectangleF(ptfOffset, analyzeClass.myOPRectF.Size);
            //    CurrentRelationPositionRectF.Add(myrect);
            //}
        }
        public int GetBranchOffsetPositionIndex(RectangleF eRectF)
        {
            //if (CurrentRelationPositionRectF.Count == 0)
            //    return -1;
            //int i = 0;
            //foreach (RectangleF rectangleF in CurrentRelationPositionRectF)
            //{
            //    if (rectangleF.IntersectsWith(eRectF))
            //        break;

            //    i++;
            //}

            //if (i >= CurrentRelationPositionRectF.Count)
            //    return -2;

            //return i;
            return 0;
        }
        public bool IsHaveBranchSeed()
        {
            if (IsSeed)
                return true;
            foreach (AnalyzeClass analyzeClass in BranchList)
            {
                if (analyzeClass.IsSeed)
                    return true;
                else
                {
                    bool bOK = analyzeClass.IsHaveBranchSeed();
                    if (bOK)
                        return true;
                }
            }
            return false;
        }
        public bool IsHaveBranchSeedGood()
        {
            if (IsSeed)
            {
                if (IsVeryGood)
                    return true;
            }
            foreach (AnalyzeClass analyzeClass in BranchList)
            {
                if (analyzeClass.IsSeed)
                {
                    if (analyzeClass.IsVeryGood)
                        return true;
                }
                else
                {
                    bool bOK = analyzeClass.IsHaveBranchSeedGood();
                    if (bOK)
                        return true;
                }
            }
            return false;
        }

        #region 胶水检测
        public int GetAnalyzeGlueErrorType()
        {
            if (PADPara.PADMethod == PADMethodEnum.NONE)
                return 0;
            if (string.IsNullOrEmpty(PADPara.DescStr))
            {
                return 0;
            }
            else
            {
                if (PADPara.DescStr.Contains("无胶"))
                {
                    return 1;
                }
                else if (PADPara.DescStr.Contains("尺寸"))
                {
                    return 2;
                }
                else if (PADPara.DescStr.Contains("溢胶"))
                {
                    return 3;
                }
                else if (PADPara.DescStr.Contains("胶水异常"))
                {
                    return 4;
                }
                else
                {
                    
                }
            }
            foreach (AnalyzeClass analyzeClass in BranchList)
            {
                int index = analyzeClass.GetAnalyzeGlueErrorType();
                if (index != 0)
                    return index;
            }
            return 5;
        }
        public string GetAnalyzeGlueErrorTypeDesc()
        {
            string descStr = string.Empty;
            if (PADPara.PADMethod == PADMethodEnum.NONE)
                descStr = string.Empty;
            if (string.IsNullOrEmpty(PADPara.DescStr))
            {
                descStr = string.Empty;
            }
            else
            {
                if (PADPara.DescStr.Contains("无胶"))
                {
                    descStr = "无胶";
                }
                else if (PADPara.DescStr.Contains("尺寸"))
                {
                    descStr = "尺寸";
                }
                else if (PADPara.DescStr.Contains("溢胶"))
                {
                    descStr = "溢胶";
                }
                else if (PADPara.DescStr.Contains("胶水异常"))
                {
                    descStr = "胶水异常";
                }
                else
                {
                    descStr = PADPara.DescStr;
                }
            }
            foreach (AnalyzeClass analyzeClass in BranchList)
            {
                descStr += analyzeClass.GetAnalyzeGlueErrorTypeDesc();
            }
            return descStr;
        }
        public string GetAnalyzeGlueErrorTypeReport()
        {
            string descStr = string.Empty;
            if (PADPara.PADMethod == PADMethodEnum.NONE)
                descStr = string.Empty;
            if (string.IsNullOrEmpty(PADPara.DescStr))
            {
                CalculateChipWidth();
                descStr = ToReportString1();
            }
            else
            {
                if (PADPara.DescStr.Contains("胶水异常"))
                {
                    CalculateChipWidth();
                    descStr = ToReportString1();
                }
                else
                {
                    descStr = ",,,,,,,,";
                }
            }
            foreach (AnalyzeClass analyzeClass in BranchList)
            {
                descStr += analyzeClass.GetAnalyzeGlueErrorTypeReport();
            }
            return descStr;
        }
        #endregion


        public void FillToList(List<AnalyzeClass> analyzelist)
        {
            analyzelist.Add(this);

            foreach (AnalyzeClass branchanalyze in BranchList)
            {
                branchanalyze.FillToList(analyzelist);
            }
            //foreach(AnalyzeClass learnanalyze in LearnList)
            //{
            //    learnanalyze.FillToList(analyzelist);
            //}
        }
        public void FillToListRemoveMe(List<AnalyzeClass> analyzelist)
        {
            //analyzelist.Add(this);

            foreach (AnalyzeClass branchanalyze in BranchList)
            {
                branchanalyze.FillToList(analyzelist);
            }
            //foreach(AnalyzeClass learnanalyze in LearnList)
            //{
            //    learnanalyze.FillToList(analyzelist);
            //}
        }
        public string ToAnalyzeString()
        {
            string anz = "A" + PageNo.ToString("00") + "-" + (Level).ToString("00") + "-" + No.ToString(ORGANALYZENOSTRING);

            return anz;
        }
        public string ToAnalyzeString(int inewpage)
        {
            string anz = "A" + inewpage.ToString("00") + "-" + (Level).ToString("00") + "-" + No.ToString(ORGANALYZENOSTRING);

            return anz;
        }
        public string ToLogAnalyzeString()
        {
            string anz = "A" + PageNo.ToString("00") + "-" + (Level).ToString("00") + "-" + No.ToString("0000") + "_" + LearnNo.ToString("00");

            return anz;
        }

        public string ToLogString()
        {
            string str = "";

            str += ToLogAnalyzeString();

            return str;
        }
        public string ToAnalyzeTestString()
        {
            string anz = "A" + PageNo.ToString("00") + "-" + PageNo.ToString("00") + "-" + (ParentNo + 1).ToString("00") + "-" + No.ToString("0000");

            return anz;
        }
        public string ToNextNodeString()
        {
            string str = "";

            if (FromNodeString.Trim() != "")
            {
                str = FromNodeString + "#";
            }

            str += No.ToString() + "," + LearnNo.ToString();

            return str;
        }

        /// <summary>
        /// FromString 不要把 Branch 弄回來
        /// </summary>
        /// <param name="str"></param>
        public void FromString(string str)
        {
            char seperator = Universal.SeperateCharA;
            string[] strs = str.Split(seperator);

            FromNormalString(strs[0]);
            ALIGNPara.FromString(strs[1]);
            INSPECTIONPara.FromString(strs[2]);
            MEASUREPara.FromString(strs[3]);
            OCRPara.FromString(strs[4]);
            AOIPara.FromString(strs[5]);
            GAPPara.FromString(strs[6]);
            HEIGHTPara.FromString(strs[7]);

            if (strs.Length > 7)
                StiltsPara.FromString(strs[8]);

            if (strs.Length > 9)
                PADPara.FromString(strs[9]);
        }

        void FromStringWithBranch(string str)
        {
            int i = 0;
            string[] strs = str.Replace(Environment.NewLine, Universal.NewlineChar.ToString()).Split(Universal.NewlineChar);

            while (i < strs.Length)
            {
                if (strs[i] == "")
                {
                    i++;
                    continue;
                }

                if (i == 0)
                    FromString(strs[0]);
                else
                {
                    AnalyzeClass newanalyze = new AnalyzeClass();
                    newanalyze.FromString(strs[i]);

                    this.BranchList.Add(newanalyze);
                }
                i++;
            }
        }

        /// <summary>
        /// ToString 不要把 Learn的弄進去，但有包含 Branch
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            char seperator = Universal.SeperateCharA;
            string str = "";

            str += ToNormalString() + seperator;
            str += ALIGNPara.ToString() + seperator;
            str += INSPECTIONPara.ToString() + seperator;
            str += MEASUREPara.ToString() + seperator;
            str += OCRPara.ToString() + seperator;
            str += AOIPara.ToString() + seperator;
            str += GAPPara.ToString() + seperator;
            str += HEIGHTPara.ToString() + seperator;
            str += StiltsPara.ToString() + seperator;
            str += PADPara.ToString() + seperator;
            str += "";

            str += Environment.NewLine;

            foreach (AnalyzeClass branchanalyze in BranchList)
            {
                str += branchanalyze.ToString();
            }

            //foreach(AnalyzeClass learnanalyze in LearnList)
            //{
            //    str += learnanalyze.ToString();
            //}-

            return str;
        }

        /// <summary>
        /// ToString 包含 Learn的弄進去
        /// </summary>
        /// <returns></returns>
        public string ToStringWithLearn()
        {
            string str = ToString();

            foreach (AnalyzeClass learnanalyze in LearnList)
            {
                str += learnanalyze.ToStringWithLearn();
            }

            return str;
        }

        ///// <summary>
        ///// No Use Function
        ///// </summary>
        //public void SaveLearn()
        //{
        //    string savelearnpath = GetSaveLearnPath(PassInfo);
        //    string savepath = PassInfo.OperatePath + "\\" + savelearnpath;

        //    if (Directory.Exists(savepath))
        //        Directory.Delete(savepath, true);

        //    Directory.CreateDirectory(savepath);

        //    foreach (AnalyzeClass learnanalyze in LearnList)
        //    {
        //        learnanalyze.bmpPATTERN.Save(savepath + "\\" + learnanalyze.LearnNo.ToString(AnalyzeClass.ORGLEARNSTRING) + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
        //        SaveData(learnanalyze.ToString(), savepath + "\\" + learnanalyze.LearnNo.ToString(AnalyzeClass.ORGLEARNSTRING) + ".jdb");
        //    }

        //}

        string GetSaveLearnPath(PassInfoClass passinfo)
        {
            string str = "";

            str += passinfo.PageNo.ToString(PageClass.ORGPAGENOSTRING) + "-";
            str += passinfo.PageOpType.ToString() + "-";
            str += ToNextNodeString().Replace(",", "x");

            return str;

        }

        /// <summary>
        /// 用於回復 ToStringWithLearn
        /// </summary>
        /// <param name="str"></param>
        public void FromStringToComposeAnalyze(string str)
        {
            string[] analyzes = str.Replace(Environment.NewLine, Universal.NewlineChar.ToString()).Split(Universal.NewlineChar);

            List<AnalyzeClass> analyzelist = new List<AnalyzeClass>();

            if (str.Length > 0)
            {
                foreach (string astr in analyzes)
                {
                    if (astr.Trim() != "")
                    {
                        AnalyzeClass analyze = new AnalyzeClass(astr, PassInfo);
                        analyzelist.Add(analyze);
                    }
                }
            }

            foreach (AnalyzeClass checkanalyze in analyzelist)
            {
                this.CheckAnalyzeToRestore(checkanalyze);
            }
        }
        /// <summary>
        /// 僅用於回復 ToStringWithLearn 裏的子功能
        /// </summary>
        /// <param name="analyze"></param>
        /// <returns></returns>
        bool CheckAnalyzeToRestore(AnalyzeClass analyze)
        {
            bool ischecked = false;

            if (No == analyze.No && LearnNo == analyze.LearnNo)
            {
                this.myMover.Clear();
                FromString(analyze.ToString());

                ischecked = true;
            }

            if (!ischecked)
            {
                foreach (AnalyzeClass branchanalyze in BranchList)
                {
                    ischecked = branchanalyze.CheckAnalyzeToRestore(analyze);

                    if (ischecked)
                        break;
                }
            }
            if (!ischecked)
            {
                foreach (AnalyzeClass learnanalyze in LearnList)
                {
                    ischecked = learnanalyze.CheckAnalyzeToRestore(analyze);

                    if (ischecked)
                        break;
                }
            }

            return ischecked;
        }
        public void ClearMover()
        {
            this.myMover.Clear();

            foreach (AnalyzeClass branchanalyze in BranchList)
            {
                branchanalyze.ClearMover();
            }
            foreach (AnalyzeClass learnsanalyze in LearnList)
            {
                learnsanalyze.ClearMover();
            }
        }

        /// <summary>
        /// Seperate Movers in SeperateC
        /// </summary>
        /// <returns></returns>
        string ToMoverString()
        {
            string retstr = "";
            char seperator = Universal.SeperateCharC;

            GraphicalObject grobj;

            for (int i = 0; i < myMover.Count; i++)
            {
                grobj = myMover[i].Source;

                if (grobj is JzRectEAG)
                {
                    retstr += (grobj as JzRectEAG).ToString() + seperator;
                }
                else if (grobj is JzCircleEAG)
                {
                    retstr += (grobj as JzCircleEAG).ToString() + seperator;
                }
                else if (grobj is JzPolyEAG)
                {
                    retstr += (grobj as JzPolyEAG).ToString() + seperator;
                }
                else if (grobj is JzRingEAG)
                {
                    retstr += (grobj as JzRingEAG).ToString() + seperator;
                }
                else if (grobj is JzStripEAG)
                {
                    retstr += (grobj as JzStripEAG).ToString() + seperator;
                }
                else if (grobj is JzIdentityHoleEAG)
                {
                    retstr += (grobj as JzIdentityHoleEAG).ToString() + seperator;
                }
                else if (grobj is JzCircleHoleEAG)
                {
                    retstr += (grobj as JzCircleHoleEAG).ToString() + seperator;
                }
            }
            if (retstr != "")
                retstr = retstr.Substring(0, retstr.Length - 1);

            return retstr;
        }
        void FromMoverString(string fromstr)
        {
            int i = 0;
            char seperator = Universal.SeperateCharC;
            string[] strs = fromstr.Split(seperator);

            SetDefaultColor(Level);

            foreach (string str in strs)
            {
                if (str.IndexOf(Figure_EAG.Rectangle.ToString()) > -1)
                {
                    JzRectEAG jzrect = new JzRectEAG(str, DefaultColor);

                    jzrect.RelateNo = No;
                    jzrect.RelatePosition = i;
                    jzrect.RelateLevel = Level;

                    myMover.Add(jzrect);
                }
                else if (str.IndexOf(Figure_EAG.Circle.ToString()) > -1)
                {
                    JzCircleEAG jzcircle = new JzCircleEAG(str, DefaultColor);

                    jzcircle.RelateNo = No;
                    jzcircle.RelatePosition = i;
                    jzcircle.RelateLevel = Level;

                    myMover.Add(jzcircle);
                }
                else if (str.IndexOf(Figure_EAG.ChatoyantPolygon.ToString()) > -1)
                {
                    JzPolyEAG jzpoly = new JzPolyEAG(str, DefaultColor);

                    jzpoly.RelateNo = No;
                    jzpoly.RelatePosition = i;
                    jzpoly.RelateLevel = Level;

                    myMover.Add(jzpoly);
                }
                else if (str.IndexOf(Figure_EAG.Ring.ToString()) > -1 || str.IndexOf(Figure_EAG.ORing.ToString()) > -1)
                {
                    JzRingEAG jzring = new JzRingEAG(str, DefaultRingColor);

                    jzring.RelateNo = No;
                    jzring.RelatePosition = i;
                    jzring.RelateLevel = Level;

                    myMover.Add(jzring);
                }
                else if (str.IndexOf(Figure_EAG.Strip.ToString()) > -1)
                {
                    JzStripEAG jzstrip = new JzStripEAG(str, DefaultColor);

                    jzstrip.RelateNo = No;
                    jzstrip.RelatePosition = i;
                    jzstrip.RelateLevel = Level;

                    myMover.Add(jzstrip);
                }
                else if (str.IndexOf(Figure_EAG.RectRect.ToString()) > -1 || str.IndexOf(Figure_EAG.HexHex.ToString()) > -1)
                {
                    JzIdentityHoleEAG jzidentityhole = new JzIdentityHoleEAG(str, DefaultColor);

                    jzidentityhole.RelateNo = No;
                    jzidentityhole.RelatePosition = i;
                    jzidentityhole.RelateLevel = Level;

                    myMover.Add(jzidentityhole);
                }
                else if (str.IndexOf(Figure_EAG.RectO.ToString()) > -1 || str.IndexOf(Figure_EAG.HexO.ToString()) > -1)
                {
                    JzCircleHoleEAG jzcirclehole = new JzCircleHoleEAG(str, DefaultColor);

                    jzcirclehole.RelateNo = No;
                    jzcirclehole.RelatePosition = i;
                    jzcirclehole.RelateLevel = Level;

                    myMover.Add(jzcirclehole);
                }

                i++;
            }
        }
        void FromMoverString(Mover tomover, string fromstr, Color assigncolor, PointF biaslocation, SizeF sizeratio, Point offset)
        {
            int i = 0;
            char seperator = Universal.SeperateCharC;
            string[] strs = fromstr.Split(seperator);

            foreach (string str in strs)
            {
                if (str.IndexOf(Figure_EAG.Rectangle.ToString()) > -1)
                {
                    JzRectEAG jzrect = new JzRectEAG(str, Color.FromArgb(0, Color.White));

                    jzrect.TransparentForMover = true;
                    jzrect.ShowMode = ShowModeEnum.MAINSHOW;
                    jzrect.MainShowPen = new Pen(assigncolor, 1);
                    jzrect.OffsetPoint = offset;
                    jzrect.MappingToMovingObject(biaslocation, sizeratio);

                    tomover.Add(jzrect);
                }
                else if (str.IndexOf(Figure_EAG.Circle.ToString()) > -1)
                {
                    JzCircleEAG jzcircle = new JzCircleEAG(str, Color.FromArgb(0, Color.White));

                    jzcircle.TransparentForMover = true;
                    jzcircle.ShowMode = ShowModeEnum.MAINSHOW;
                    jzcircle.MainShowPen = new Pen(assigncolor, 1);
                    jzcircle.OffsetPoint = offset;
                    jzcircle.MappingToMovingObject(biaslocation, sizeratio);

                    tomover.Add(jzcircle);
                }
                else if (str.IndexOf(Figure_EAG.ChatoyantPolygon.ToString()) > -1)
                {
                    JzPolyEAG jzpoly = new JzPolyEAG(str, Color.FromArgb(0, Color.White));

                    jzpoly.TransparentForMover = true;
                    jzpoly.ShowMode = ShowModeEnum.MAINSHOW;
                    jzpoly.MainShowPen = new Pen(assigncolor, 1);
                    jzpoly.OffsetPoint = offset;
                    jzpoly.MappingToMovingObject(biaslocation, sizeratio);

                    tomover.Add(jzpoly);
                }
                else if (str.IndexOf(Figure_EAG.Ring.ToString()) > -1 || str.IndexOf(Figure_EAG.ORing.ToString()) > -1)
                {
                    JzRingEAG jzring = new JzRingEAG(str, Color.FromArgb(0, Color.White));

                    jzring.TransparentForMover = true;
                    jzring.ShowMode = ShowModeEnum.MAINSHOW;
                    jzring.MainShowPen = new Pen(assigncolor, 1);
                    jzring.OffsetPoint = offset;
                    jzring.MappingToMovingObject(biaslocation, sizeratio);

                    tomover.Add(jzring);
                }
                else if (str.IndexOf(Figure_EAG.Strip.ToString()) > -1)
                {
                    JzStripEAG jzstrip = new JzStripEAG(str, Color.FromArgb(0, Color.White));

                    jzstrip.TransparentForMover = true;
                    jzstrip.ShowMode = ShowModeEnum.MAINSHOW;
                    jzstrip.MainShowPen = new Pen(assigncolor, 1);
                    jzstrip.OffsetPoint = offset;
                    jzstrip.MappingToMovingObject(biaslocation, sizeratio);

                    tomover.Add(jzstrip);
                }
                else if (str.IndexOf(Figure_EAG.RectRect.ToString()) > -1 || str.IndexOf(Figure_EAG.HexHex.ToString()) > -1)
                {
                    JzIdentityHoleEAG jzidentityhole = new JzIdentityHoleEAG(str, Color.FromArgb(0, Color.White));

                    jzidentityhole.TransparentForMover = true;
                    jzidentityhole.ShowMode = ShowModeEnum.MAINSHOW;
                    jzidentityhole.MainShowPen = new Pen(assigncolor, 1);
                    jzidentityhole.OffsetPoint = offset;
                    jzidentityhole.MappingToMovingObject(biaslocation, sizeratio);

                    tomover.Add(jzidentityhole);
                }
                else if (str.IndexOf(Figure_EAG.RectO.ToString()) > -1 || str.IndexOf(Figure_EAG.HexO.ToString()) > -1)
                {
                    JzCircleHoleEAG jzcirclehole = new JzCircleHoleEAG(str, Color.FromArgb(0, Color.White));

                    jzcirclehole.TransparentForMover = true;
                    jzcirclehole.ShowMode = ShowModeEnum.MAINSHOW;
                    jzcirclehole.MainShowPen = new Pen(assigncolor, 1);
                    jzcirclehole.OffsetPoint = offset;
                    jzcirclehole.MappingToMovingObject(biaslocation, sizeratio);

                    tomover.Add(jzcirclehole);
                }

                i++;
            }
        }

        void FromMoverString(Mover tomover, string fromstr, List<Color> assigncolors, PointF biaslocation, SizeF sizeratio, Point offset)
        {
            int i = 0;
            int j = 0;
            char seperator = Universal.SeperateCharC;
            string[] strs = fromstr.Split(seperator);

            foreach (string str in strs)
            {
                if (str.IndexOf(Figure_EAG.Rectangle.ToString()) > -1)
                {
                    JzRectEAG jzrect = new JzRectEAG(str, Color.FromArgb(0, Color.White));

                    jzrect.TransparentForMover = true;
                    jzrect.ShowMode = ShowModeEnum.MAINSHOW;
                    //jzrect.MainShowPen = new Pen(assigncolor, 1);
                    jzrect.OffsetPoint = offset;
                    jzrect.MappingToMovingObject(biaslocation, sizeratio);

                    //tomover.Add(jzrect);

                    j = 0;
                    foreach (Color c in assigncolors)
                    {
                        RectangleF rect = jzrect.GetRectF;
                        rect.Height = 10;
                        rect.Width = 10;
                        JzRectEAG jzrectx = new JzRectEAG(c, rect);
                        // JzRectEAG jzrectx = new JzRectEAG(str, Color.FromArgb(0, Color.White));

                        jzrectx.TransparentForMover = true;
                        jzrectx.ShowMode = ShowModeEnum.MAINSHOW;
                        //jzrectx.MainShowPen = new Pen(assigncolor, 1);
                        jzrectx.OffsetPoint = offset;
                        jzrectx.OffsetPoint.X += j;
                        jzrectx.MappingToMovingObject(biaslocation, sizeratio);
                        jzrectx.Color = c;


                        j += 10;

                        tomover.Add(jzrectx);
                    }
                }
                else if (str.IndexOf(Figure_EAG.Circle.ToString()) > -1)
                {
                    JzCircleEAG jzcircle = new JzCircleEAG(str, Color.FromArgb(0, Color.White));

                    jzcircle.TransparentForMover = true;
                    jzcircle.ShowMode = ShowModeEnum.MAINSHOW;
                    // jzcircle.MainShowPen = new Pen(assigncolor, 1);
                    jzcircle.OffsetPoint = offset;
                    jzcircle.MappingToMovingObject(biaslocation, sizeratio);

                    //tomover.Add(jzcircle);
                }
                else if (str.IndexOf(Figure_EAG.ChatoyantPolygon.ToString()) > -1)
                {
                    JzPolyEAG jzpoly = new JzPolyEAG(str, Color.FromArgb(0, Color.White));

                    jzpoly.TransparentForMover = true;
                    jzpoly.ShowMode = ShowModeEnum.MAINSHOW;
                    //jzpoly.MainShowPen = new Pen(assigncolor, 1);
                    jzpoly.OffsetPoint = offset;
                    jzpoly.MappingToMovingObject(biaslocation, sizeratio);

                    //tomover.Add(jzpoly);
                }
                else if (str.IndexOf(Figure_EAG.Ring.ToString()) > -1 || str.IndexOf(Figure_EAG.ORing.ToString()) > -1)
                {
                    JzRingEAG jzring = new JzRingEAG(str, Color.FromArgb(0, Color.White));

                    jzring.TransparentForMover = true;
                    jzring.ShowMode = ShowModeEnum.MAINSHOW;
                    //jzring.MainShowPen = new Pen(assigncolor, 1);
                    jzring.OffsetPoint = offset;
                    jzring.MappingToMovingObject(biaslocation, sizeratio);

                    //tomover.Add(jzring);
                }
                else if (str.IndexOf(Figure_EAG.Strip.ToString()) > -1)
                {
                    JzStripEAG jzstrip = new JzStripEAG(str, Color.FromArgb(0, Color.White));

                    jzstrip.TransparentForMover = true;
                    jzstrip.ShowMode = ShowModeEnum.MAINSHOW;
                    //jzstrip.MainShowPen = new Pen(assigncolor, 1);
                    jzstrip.OffsetPoint = offset;
                    jzstrip.MappingToMovingObject(biaslocation, sizeratio);

                    //tomover.Add(jzstrip);
                }
                else if (str.IndexOf(Figure_EAG.RectRect.ToString()) > -1 || str.IndexOf(Figure_EAG.HexHex.ToString()) > -1)
                {
                    JzIdentityHoleEAG jzidentityhole = new JzIdentityHoleEAG(str, Color.FromArgb(0, Color.White));

                    jzidentityhole.TransparentForMover = true;
                    jzidentityhole.ShowMode = ShowModeEnum.MAINSHOW;
                    //jzidentityhole.MainShowPen = new Pen(assigncolor, 1);
                    jzidentityhole.OffsetPoint = offset;
                    jzidentityhole.MappingToMovingObject(biaslocation, sizeratio);

                    //tomover.Add(jzidentityhole);
                }
                else if (str.IndexOf(Figure_EAG.RectO.ToString()) > -1 || str.IndexOf(Figure_EAG.HexO.ToString()) > -1)
                {
                    JzCircleHoleEAG jzcirclehole = new JzCircleHoleEAG(str, Color.FromArgb(0, Color.White));

                    jzcirclehole.TransparentForMover = true;
                    jzcirclehole.ShowMode = ShowModeEnum.MAINSHOW;
                    //jzcirclehole.MainShowPen = new Pen(assigncolor, 1);
                    jzcirclehole.OffsetPoint = offset;
                    jzcirclehole.MappingToMovingObject(biaslocation, sizeratio);

                    //tomover.Add(jzcirclehole);
                }

                i++;
            }
        }

        /// <summary>
        /// Seperate Normal Data in SeperateB
        /// </summary>
        /// <returns></returns>
        string ToNormalString()
        {
            char seperator = Universal.SeperateCharB;
            string str = "";

            str = PageNo.ToString() + seperator;                //0
            str += ((int)PageOPtype).ToString() + seperator;    //1
            str += No.ToString() + seperator;                   //2
            str += ParentNo.ToString() + seperator;             //3
            str += ((int)AnalyzeType).ToString() + seperator;   //4
            str += Level.ToString() + seperator;                //5
            str += ToMoverString() + seperator;                 //6

            str += AliasName + seperator;                       //7
            str += Brightness.ToString() + seperator;           //8
            str += Contrast.ToString() + seperator;             //9
            str += ((int)MaskMethod).ToString() + seperator;    //10

            str += ExtendX.ToString() + seperator;              //11
            str += ExtendY.ToString() + seperator;              //12

            str += (IsLearnRoot ? "1" : "0") + seperator;       //13
            str += LearnNo.ToString() + seperator;           //14
            str += RectFToString(LearnOrigionOffsetRectf) + seperator;           //15
            str += ParentLearnNo.ToString() + seperator;     //16
            str += FromNodeString + seperator;                  //17
            str += RelateASN + seperator;                       //18
            str += RelateASNItem + seperator;                   //19
            str += (IsSeed ? "1" : "0") + seperator;                  //20
            str += "";

            return str;
        }
        void FromNormalString(string str)
        {
            char seperator = Universal.SeperateCharB;
            string[] strs = str.Split(seperator);

            PageNo = int.Parse(strs[0]);
            PageOPtype = (PageOPTypeEnum)int.Parse(strs[1]);
            No = int.Parse(strs[2]);
            ParentNo = int.Parse(strs[3]);
            AnalyzeType = (AnalyzeTypeEnum)int.Parse(strs[4]);
            Level = int.Parse(strs[5]);

            FromMoverString(strs[6]);

            AliasName = strs[7];
            Brightness = int.Parse(strs[8]);
            Contrast = int.Parse(strs[9]);
            MaskMethod = (MaskMethodEnum)int.Parse(strs[10]);
            ExtendX = int.Parse(strs[11]);
            ExtendY = int.Parse(strs[12]);

            if (strs.Length > 14)
            {
                IsLearnRoot = strs[13] == "1";
            }
            if (strs.Length > 15)
            {
                LearnNo = int.Parse(strs[14]);
            }
            if (strs.Length > 16)
            {
                LearnOrigionOffsetRectf = StringToRectF(strs[15]);
            }
            if (strs.Length > 17)
            {
                ParentLearnNo = int.Parse(strs[16]);
            }
            if (strs.Length > 18)
            {
                FromNodeString = strs[17];
            }
            if (strs.Length > 19)
            {
                RelateASN = strs[18];
                RelateASNItem = strs[19];

                if (RelateASN == "")
                {
                    RelateASN = "None";
                }
                if (RelateASNItem == "")
                {
                    RelateASNItem = "None";
                }

            }

            if (strs.Length > 20)
            {
                IsSeed = strs[20] == "1";
            }

        }
        public void SetDefaultColor(int level)
        {
            switch ((level - 1) % 7)
            {
                case 0:
                    DefaultColor = Color.FromArgb(0, Color.Red);
                    break;
                case 1:
                    DefaultColor = Color.FromArgb(0, Color.Lime);
                    break;
                case 2:
                    DefaultColor = Color.FromArgb(0, Color.DarkBlue);
                    break;
                case 3:
                    DefaultColor = Color.FromArgb(0, Color.Yellow);
                    break;
                case 4:
                    DefaultColor = Color.FromArgb(0, Color.SkyBlue);
                    break;
                case 5:
                    DefaultColor = Color.FromArgb(0, Color.Orange);
                    break;
                case 6:
                    DefaultColor = Color.FromArgb(0, Color.Purple);
                    break;
            }
        }

        public void ToPassInfo()
        {
            PassInfo.PageNo = PageNo;
            PassInfo.PageOpType = PageOPtype;
            PassInfo.ParentNo = ParentNo;
            PassInfo.ParentLearnNo = ParentLearnNo;

            PassInfo.AnalyzeNo = No;
            PassInfo.Level = Level;
            PassInfo.AnalyzeLearnNo = LearnNo;
        }

        //在學習前存下所有要回復的訊息
        AnalyzeClass AnalyzeBackup = null;
        public bool IsPrepareForLearn = false;
        /// <summary>
        /// 看是否為學習或是僅為修改資料
        /// </summary>
        /// <param name="isprepareforlearn"></param>
        public void PrepareForLearnOperation(bool isprepareforlearn)
        {
            IsPrepareForLearn = isprepareforlearn;

            if (AnalyzeBackup != null)
            {
                AnalyzeBackup.Suicide();
                //AnalyzeBackup.FromStringToComposeAnalyze(this.ToStringWithLearn());
                AnalyzeBackup = this.Clone(true);
            }
            else
            {
                AnalyzeBackup = this.Clone(true);
            }

            AnalyzeBackup.myOringinOffsetPointF = this.myOringinOffsetPointF;
            AnalyzeBackup.myOPRectF = this.myOPRectF;

            #region Backup Original Position

            this.ParentNo = 0;

            //Update Learn Parent No
            //foreach(AnalyzeClass learnanalyze in LearnList)
            //{
            //    learnanalyze.PageNo = this.PageNo;
            //}

            PointF offset = this.myOringinOffsetPointF;

            offset.X = -offset.X;
            offset.Y = -offset.Y;

            this.PrepareMoverOffset(offset);

            ////先把所有的Learn.ParentNo 設定為 0 ，才能在 DetailFrom 裏面
            foreach (AnalyzeClass learnanalyze in this.LearnList)
            {
                learnanalyze.ParentNo = this.ParentNo;
            }

            #endregion

            #region Create Learn Analyze

            if (IsPrepareForLearn)
            {
                AnalyzeClass learnanalyze = this.DeepClone();

                learnanalyze.IsLearnRoot = true;
                learnanalyze.LearnNo = this.LearnList.Count + 1;
                learnanalyze.ParentNo = this.ParentNo;
                learnanalyze.ParentLearnNo = this.ParentLearnNo;
                learnanalyze.AnalyzeType = AnalyzeTypeEnum.LEARNING;

                //***********這行是Learning能回到正確位置的關鍵 *******************
                learnanalyze.FromNodeString = this.ToNextNodeString();

                //把原有切圖的位置記下來
                learnanalyze.LearnOrigionOffsetRectf = myOPRectF;

                //清掉原來的 OriginOffsetPointF
                learnanalyze.myOringinOffsetPointF = new PointF(0, 0);

                learnanalyze.bmpPATTERN.Dispose();
                learnanalyze.bmpPATTERN = new Bitmap(bmpINPUT);

                foreach (AnalyzeClass branchanalyze in learnanalyze.BranchList)
                {
                    branchanalyze.ParentLearnNo = learnanalyze.LearnNo;
                    branchanalyze.ReviseFromNodeString(learnanalyze.ToNextNodeString());
                }

                this.LearnList.Add(learnanalyze);
            }
            #endregion

        }
        void ReviseFromNodeString(string fromnodestring)
        {
            this.FromNodeString = fromnodestring;

            foreach (AnalyzeClass branchanalyze in BranchList)
            {
                branchanalyze.ReviseFromNodeString(this.ToNextNodeString());
            }
        }

        /// <summary>
        /// 準備 Mover 包含 Branch 的偏移
        /// </summary>
        /// <param name="offsetptf"></param>
        public void PrepareMoverOffset(PointF offsetptf)
        {
            int i = 0;

            Mover mover = this.myMover;

            while (i < mover.Count)
            {
                GraphicalObject grobj = mover[i].Source;

                (grobj as GeoFigure).MappingToMovingObject(offsetptf, new SizeF(1f, 1f));

                i++;
            }

            foreach (AnalyzeClass branchanalzye in BranchList)
            {
                branchanalzye.PrepareMoverOffset(offsetptf);
            }
        }
        /// <summary>
        /// 是否要更新所做的更改
        /// </summary>
        /// <param name="isupdate"></param>
        public void EndLearnOperation(bool isupdate)
        {
            if (!isupdate)
            {
                if (IsPrepareForLearn)
                {
                    AnalyzeClass lastlearnanalyze = this.GetLastLearn();

                    if (lastlearnanalyze == null)
                        return;
                    lastlearnanalyze.Suicide();

                    this.LearnList.RemoveAt(this.LearnList.Count - 1);
                }

                this.ClearMover();
                this.FromStringToComposeAnalyze(AnalyzeBackup.ToStringWithLearn());
            }

            this.ParentNo = AnalyzeBackup.ParentNo;

            //將所有的 ParentN○ 填回去才能在 LOAD 時對應回去
            foreach (AnalyzeClass learnanalyze in LearnList)
            {
                learnanalyze.ParentNo = this.ParentNo;
            }

            if (isupdate)
            {
                #region 將 Mover 裏的東西搬回去
                this.myOringinOffsetPointF = AnalyzeBackup.myOringinOffsetPointF;

                PointF offset = this.myOringinOffsetPointF;

                offset.X = offset.X;
                offset.Y = offset.Y;

                this.PrepareMoverOffset(offset);

                #endregion

                this.ToPassInfo();

                foreach (int removeno in PrepareRemovalNoList)
                {
                    int i = 0;

                    while (i < LearnList.Count)
                    {
                        if (LearnList[i].LearnNo == removeno)
                        {
                            LearnList.RemoveAt(i);
                            break;
                        }
                        i++;
                    }
                }

                if (LearnList.Count > 0)
                {
                    //string savelearnpath = GetSaveLearnPath(PassInfo);
                    //string savepath = PassInfo.OperatePath + "\\" + savelearnpath;

                    //if (Directory.Exists(savepath))
                    //    Directory.Delete(savepath, true);

                    //Directory.CreateDirectory(savepath);
                    //SaveAllLearn(null);
                    SaveLearn(PassInfo, null);
                }
            }
        }

        //void SaveLearn(string savepath)
        //{
        //    //List<AnalyzeClass> learnlist = new List<AnalyzeClass>();

        //    //foreach(AnalyzeClass learnanalyze in LearnList)
        //    //{
        //    //    learnanalyze.bmpPATTERN.Save(savepath + "\\" + learnanalyze.LearnNo.ToString(AnalyzeClass.ORGLEARNSTRING) + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
        //    //    SaveData(learnanalyze.ToString(), savepath + "\\" + learnanalyze.LearnNo.ToString(AnalyzeClass.ORGLEARNSTRING) + ".jdb");
        //    //}

        //    SaveLearn(savepath, null);
        //}
        /// <summary>
        /// Save Learn 是否要存入檔名資訊
        /// </summary>
        /// <param name="filestringlist"></param>
        void SaveLearn(PassInfoClass passinfo, List<string> filestringlist)
        {
            //this.ToPassInfo();

            string savelearnpath = GetSaveLearnPath(PassInfo);
            string savepath = passinfo.OperatePath + "\\" + savelearnpath;

            if (Directory.Exists(savepath))
                Directory.Delete(savepath, true);

            if (LearnList.Count < 1)
                return;

            Directory.CreateDirectory(savepath);

            foreach (AnalyzeClass learnanalyze in LearnList)
            {
                string bmpfilename = savepath + "\\" + learnanalyze.LearnNo.ToString(AnalyzeClass.ORGLEARNSTRING) + Universal.GlobalImageTypeString;
                string dbfilename = savepath + "\\" + learnanalyze.LearnNo.ToString(AnalyzeClass.ORGLEARNSTRING) + ".jdb";

                learnanalyze.bmpPATTERN.Save(bmpfilename, Universal.GlobalImageFormat);
                SaveData(learnanalyze.ToString(), dbfilename);

                if (filestringlist != null)
                {
                    filestringlist.Add(bmpfilename);
                    filestringlist.Add(dbfilename);
                }
            }
        }
        /// <summary>
        /// 儲存所有的 Learn 資料
        /// </summary>
        /// <param name="filestringlist"></param>
        public void SaveAllLearn(List<string> filestringlist)
        {
            foreach (AnalyzeClass branchanalyze in BranchList)
            {
                branchanalyze.SaveAllLearn(filestringlist);
            }
            foreach (AnalyzeClass learnanalyze in LearnList)
            {
                learnanalyze.SaveAllLearn(filestringlist);
            }

            SaveLearn(PassInfo, filestringlist);
        }
        /*
        void LoadLearn(PassInfoClass passinfo)
        {
            string loeadlearnpath = GetSaveLearnPath(PassInfo);
            string loadpath = PassInfo.OperatePath + "\\" + loeadlearnpath;

            if(Directory.Exists(loadpath))
            {
                string[] filestrs = Directory.GetFiles(loadpath, "*." + Universal.GlobalImageTypeString);

                foreach(string filestr in filestrs)
                {
                    string analyzestr = "";
                    ReadData(ref analyzestr, filestr.Replace(Universal.GlobalImageTypeString, ".jdb"));

                    AnalyzeClass learnanalyze = new AnalyzeClass(analyzestr, passinfo);
                    GetBMP(filestr, ref learnanalyze.bmpPATTERN);

                    LearnList.Add(learnanalyze);
                }
            }
        }

        public string ToAnalyzeSettingString()
        {
            char seperator = Universal.SeperateCharA;
            string str = "";

            str = ToAnalyzeString() + seperator;
            str += ToNoramalString() + seperator;
            str += ALIGNPara.ToString() + seperator;
            str += INSPECTIONPara.ToString() + seperator;
            str += MEASUREPara.ToString() + seperator;
            str += OCRPara.ToString() + seperator;
            str += AOIPara.ToString() + seperator;
            str += GAPPara.ToString() + seperator;
            str += HEIGHTPara.ToString() + seperator;

            str += "";
            
            return str;
        }
        public void FromAnalyzeSettingString(string str)
        {
            char seperator = Universal.SeperateCharA;
            string[] strs = str.Split(seperator);

            FromNormalString(strs[1]);
            ALIGNPara.FromString(strs[2]);
            INSPECTIONPara.FromString(strs[3]);
            MEASUREPara.FromString(strs[4]);
            OCRPara.FromString(strs[5]);
            AOIPara.FromString(strs[6]);
            GAPPara.FromString(strs[7]);
            HEIGHTPara.FromString(strs[8]);
        }
        */
        public void ToAssembleProperty()
        {
            NORMALPara.Name = ToAnalyzeString();
            NORMALPara.AliasName = AliasName;
            NORMALPara.Brightness = Brightness;
            NORMALPara.Contrast = Contrast;
            NORMALPara.MaskMethod = MaskMethod;
            NORMALPara.ExtendX = ExtendX;
            NORMALPara.ExtendY = ExtendY;
            NORMALPara.RelateASN = RelateASN;
            NORMALPara.RelateASNItem = RelateASNItem;
            NORMALPara.IsSeed = IsSeed;

            ASSEMBLE.GetNormal(NORMALPara);
            ASSEMBLE.GetAlign(ALIGNPara);
            ASSEMBLE.GetInspection(INSPECTIONPara);
            ASSEMBLE.GetMeasure(MEASUREPara);
            ASSEMBLE.GetOCR(OCRPara);
            ASSEMBLE.GetAOI(AOIPara);
            ASSEMBLE.GetHeight(HEIGHTPara);
            ASSEMBLE.GetGap(GAPPara);

            ASSEMBLE.GetStilts(StiltsPara);

            ASSEMBLE.GetPADCHECK(PADPara);
        }
        public void FromAssembleProperty()
        {
            ASSEMBLE.SetNormal(NORMALPara);
            ASSEMBLE.SetAlign(ALIGNPara);
            ASSEMBLE.SetInspection(INSPECTIONPara);
            ASSEMBLE.SetMeasure(MEASUREPara);
            ASSEMBLE.SetOCR(OCRPara);
            ASSEMBLE.SetAOI(AOIPara);
            ASSEMBLE.SetHeight(HEIGHTPara);
            ASSEMBLE.SetGap(GAPPara);
            ASSEMBLE.SetStilts(StiltsPara);
            ASSEMBLE.SetPADCHECK(PADPara);

            AliasName = NORMALPara.AliasName;
            Brightness = NORMALPara.Brightness;
            Contrast = NORMALPara.Contrast;
            MaskMethod = NORMALPara.MaskMethod;
            ExtendX = NORMALPara.ExtendX;
            ExtendY = NORMALPara.ExtendY;

            RelateASN = NORMALPara.RelateASN;
            RelateASNItem = NORMALPara.RelateASNItem;
            IsSeed = NORMALPara.IsSeed;

        }
        public void FromAssembleProperty(string changeitemstring, string valuestring)
        {
            if (IsSelected)
            {
                if (changeitemstring == "RESET")    //RESET To Default Value
                {
                    NORMALPara.Reset();
                    ALIGNPara.Reset();
                    INSPECTIONPara.Reset();
                    MEASUREPara.Reset();
                    OCRPara.Reset();
                    AOIPara.Reset();
                    HEIGHTPara.Reset();
                    GAPPara.Reset();
                    StiltsPara.Reset();
                    PADPara.Reset();
                }
                else
                {
                    NORMALPara.FromPropertyChange(changeitemstring, valuestring);
                    ALIGNPara.FromPropertyChange(changeitemstring, valuestring);
                    INSPECTIONPara.FromPropertyChange(changeitemstring, valuestring);
                    MEASUREPara.FromPropertyChange(changeitemstring, valuestring);
                    OCRPara.FromPropertyChange(changeitemstring, valuestring);
                    AOIPara.FromPropertyChange(changeitemstring, valuestring);
                    HEIGHTPara.FromPropertyChange(changeitemstring, valuestring);
                    GAPPara.FromPropertyChange(changeitemstring, valuestring);
                    StiltsPara.FromPropertyChange(changeitemstring, valuestring);
                    PADPara.FromPropertyChange(changeitemstring, valuestring);
                }

                AliasName = NORMALPara.AliasName;
                Brightness = NORMALPara.Brightness;
                Contrast = NORMALPara.Contrast;
                MaskMethod = NORMALPara.MaskMethod;
                ExtendX = NORMALPara.ExtendX;
                ExtendY = NORMALPara.ExtendY;

                RelateASN = NORMALPara.RelateASN;
                RelateASNItem = NORMALPara.RelateASNItem;
                IsSeed = NORMALPara.IsSeed;

                JetEazy.LoggerClass.Instance.WriteLog("参数位置:" + PassInfo.OperatePath + " 参数名: " + changeitemstring + " 参数值: " + valuestring);
            }

            foreach (AnalyzeClass branchanalyze in BranchList)
            {
                branchanalyze.ToAssembleProperty();
                branchanalyze.FromAssembleProperty(changeitemstring, valuestring);
            }
        }
        public void FromAssembleProperty(string changeitemstring)
        {

            if (changeitemstring == "RESET")    //RESET To Default Value
            {
                NORMALPara.Reset();
                ALIGNPara.Reset();
                INSPECTIONPara.Reset();
                MEASUREPara.Reset();
                OCRPara.Reset();
                AOIPara.Reset();
                HEIGHTPara.Reset();
                GAPPara.Reset();

                StiltsPara.Reset();
                PADPara.Reset();
            }

            //     AliasName = NORMALPara.AliasName;
            Brightness = NORMALPara.Brightness;
            Contrast = NORMALPara.Contrast;
            MaskMethod = NORMALPara.MaskMethod;
            ExtendX = NORMALPara.ExtendX;
            ExtendY = NORMALPara.ExtendY;

            RelateASN = NORMALPara.RelateASN;
            RelateASNItem = NORMALPara.RelateASNItem;
            IsSeed = NORMALPara.IsSeed;

            switch(VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch(OPTION)
                    {
                        case OptionEnum.MAIN_X6:

                            ExtendX = 40;
                            ExtendY = 40;

                            ALIGNPara.AlignMethod = AlignMethodEnum.AUFIND;
                            ALIGNPara.AlignMode = AlignModeEnum.BORDER;

                            break;
                    }
                    break;
            }


        }
        public void Suicide()
        {
            bmpINPUT.Dispose();
            bmpWIP.Dispose();
            bmpPATTERN.Dispose();
            bmpMASK.Dispose();
            bmpHOLLOW.Dispose();

            if (bmpORGLEARNININPUT != null)
                bmpORGLEARNININPUT.Dispose();

            bmpOUTPUT.Dispose();

            ALIGNPara.Suicide();
            AOIPara.Suicide();
            INSPECTIONPara.Suicide();

            OCRPara.Suicide();
            HEIGHTPara.Suicide();
            MEASUREPara.Suicide();
            GAPPara.Suicide();
            StiltsPara.Suicide();
            PADPara.Suicide();

            foreach (AnalyzeClass branchanalyze in BranchList)
            {
                branchanalyze.Suicide();
            }
            foreach (AnalyzeClass learnanalyze in LearnList)
            {
                learnanalyze.Suicide();
            }

            TrainStatusCollection.Clear();
            RunStatusCollection.Clear();
        }

        #region Normal Operation
        public void SaveBMP()
        {
            foreach (AnalyzeClass analyze in LearnList)
            {
                analyze.SaveBMP();
            }
            foreach (AnalyzeClass analyze in BranchList)
            {
                analyze.SaveBMP();
            }
        }
        /// <summary>
        /// 設定Mover裏的 Select
        /// </summary>
        /// <param name="isselect"></param>
        public void SetMoverSelected(bool isselect)
        {
            SetMoverSelected(false, isselect);
        }
        public void SetMoverSelected(bool isselectfirst, bool isselect)
        {
            int i = 0;

            GraphicalObject grobj;

            bool isfirstok = false;

            while (i < myMover.Count)
            {
                grobj = myMover[i].Source;

                if (!isfirstok && isselectfirst && isselect)
                {
                    (grobj as GeoFigure).IsFirstSelected = true;
                    isfirstok = true;
                }
                else
                    (grobj as GeoFigure).IsFirstSelected = false;

                (grobj as GeoFigure).IsSelected = isselect;

                i++;
            }
        }
        public void SetBranchMoverSelected(bool isselect)
        {
            foreach (AnalyzeClass branchanalyze in BranchList)
            {
                branchanalyze.SetMoverSelected(isselect);
                branchanalyze.SetBranchMoverSelected(isselect);
            }
        }
        public void SetMoverSelected(List<int> selectindexlist)
        {
            int i = 0;
            int firstselectindex = 0;

            GraphicalObject grobj;

            //先找到第一顆選擇的
            if (selectindexlist.Count == 0)
            {
                firstselectindex = -1;
            }
            else
            {
                i = 0;
                while (i < myMover.Count)
                {
                    grobj = myMover[i].Source;

                    if ((grobj as GeoFigure).IsFirstSelected)
                    {
                        firstselectindex = i;
                        break;
                    }
                    i++;
                }

                //先找到第一顆選擇的是否不選擇的裏面，就將此顆取代成第一顆
                if (selectindexlist.IndexOf(firstselectindex) < 0)
                {
                    firstselectindex = selectindexlist[0];
                }
            }

            //把選擇填回去
            i = 0;
            while (i < myMover.Count)
            {
                grobj = myMover[i].Source;

                if (selectindexlist.IndexOf(i) > -1)
                {
                    (grobj as GeoFigure).IsSelected = true;
                    (grobj as GeoFigure).IsFirstSelected = i == firstselectindex;
                }
                else
                {
                    (grobj as GeoFigure).IsSelected = false;
                    (grobj as GeoFigure).IsFirstSelected = false;
                }
                i++;
            }
        }
        /// <summary>
        /// 將所有的Mover偏移一個位置
        /// </summary>
        /// <param name="offsetpoint"></param>
        void SetMoverOffset(Point offsetpoint)
        {
            int i = 0;

            while (i < myMover.Count)
            {
                GraphicalObject grpobj = myMover[i].Source;

                (grpobj as GeoFigure).SetOffset(offsetpoint);

                i++;
            }
        }
        void SetMoverOffset(Point offsetpoint, bool isneedchange)
        {
            if (ALIGNPara.AlignMethod == AlignMethodEnum.NONE)
            {
                myAlignOffsetChange = isneedchange;
                myAlignOffsetCal = new Point(offsetpoint.X, offsetpoint.Y);
            }
            else
            {
                myAlignOffsetChange = false;
                myAlignOffsetCal = new Point(0, 0);
            }
        }
        void SetMoverAngle(double adddegree)
        {
            int i = 0;

            while (i < myMover.Count)
            {
                GraphicalObject grpobj = myMover[i].Source;

                (grpobj as GeoFigure).SetAngle(adddegree);

                i++;
            }
        }

        public double GetMoverAngle()
        {
            if (myMover.Count > 1)
                return 0;

            int i = 0;

            double _angle = 0;

            while (i < myMover.Count)
            {
                GraphicalObject grpobj = myMover[i].Source;

                _angle = (grpobj as GeoFigure).Angle;

                i++;
            }

            return _angle;
        }


        public void SetMoverDefault()
        {
            myMover.Clear();

            JzRectEAG jzrect = new JzRectEAG(Color.FromArgb(0, Color.Red));
            jzrect.RelateNo = No;
            jzrect.RelatePosition = 0;
            jzrect.RelateLevel = Level;

            myMover.Add(jzrect);

        }

        /// <summary>
        /// 設定Mover 裏的相關資料
        /// </summary>
        /// <param name="relateno"></param>
        /// <param name="relateposition"></param>
        public void RelateMover(int relateno, int relatelevel)
        {
            int i = 0;

            while (i < myMover.Count)
            {
                GraphicalObject grpobj = myMover[i].Source;

                (grpobj as GeoFigure).RelateNo = relateno;
                (grpobj as GeoFigure).RelatePosition = i;
                (grpobj as GeoFigure).RelateLevel = relatelevel;

                i++;
            }
        }
        /// <summary>
        /// 將選擇的Isselected傳遞到下一個新增的項目中
        /// </summary>
        /// <param name="originanalyze"></param>
        /// <param name="isclearorigin"></param>
        /// <param name="isdup"></param>
        void TransferMoverSelect(AnalyzeClass originanalyze, bool isclearorigin, bool isdup)
        {
            Mover frommover = originanalyze.myMover;

            int i = 0;
            while (i < originanalyze.myMover.Count)
            {
                GraphicalObject grpobjfrom = frommover[i].Source;
                GraphicalObject grpobjto = myMover[i].Source;

                (grpobjto as GeoFigure).IsSelected = (grpobjfrom as GeoFigure).IsSelected;
                (grpobjto as GeoFigure).IsFirstSelected = (grpobjfrom as GeoFigure).IsFirstSelected;

                if (isclearorigin)
                {
                    (grpobjfrom as GeoFigure).IsSelected = false;
                    (grpobjfrom as GeoFigure).IsFirstSelected = false;
                }
                i++;
            }

            if (isdup)
            {
                i = 0;
                foreach (AnalyzeClass branchfromanalyze in originanalyze.BranchList)
                {
                    BranchList[i].TransferMoverSelect(branchfromanalyze, isclearorigin, isdup);
                    i++;
                }
            }
        }
        /// <summary>
        /// 刪除選取的Branch
        /// </summary>
        /// <param name="deletnolist"></param>
        public void DeleteBranch(List<int> deletenolist)
        {
            int i = BranchList.Count - 1;

            while (i > -1)
            {
                AnalyzeClass branchanalyze = BranchList[i];

                branchanalyze.DeleteBranch(deletenolist);

                if (deletenolist.IndexOf(branchanalyze.No) > -1)
                {
                    BranchList[i].Suicide();
                    BranchList.RemoveAt(i);
                }
                i--;
            }
        }
        /// <summary>
        /// 將要刪除的資料寫入
        /// </summary>
        /// <param name="deletenolist"></param>
        /// <param name="deleteanalyzelist"></param>
        public void InsertDeleteData(List<int> deletenolist, List<AnalyzeClass> deleteanalyzelist)
        {
            if (deletenolist.IndexOf(this.No) < 0)
            {
                deletenolist.Add(this.No);
                deleteanalyzelist.Add(this);
            }
            foreach (AnalyzeClass branchanalyze in BranchList)
            {
                branchanalyze.InsertDeleteData(deletenolist, deleteanalyzelist);
            }
        }
        public static int DupMaxNo = -1;
        public static int LearnMaxNo = -1;

        /// <summary>
        /// 將要複制的資料寫入
        /// </summary>
        /// <param name="dupnolist"></param>
        /// <param name="dupanalyzelist"></param>
        public void InsertDupData(List<int> dupnolist, List<AnalyzeClass> dupanalyzelist, List<AnalyzeClass> rawanalyzelist, DataTable analyzetable, bool islearn)
        {
            this.No = DupMaxNo;
            this.AliasName = ToAnalyzeString();

            this.ToPassInfo();

            //if (dupnolist.IndexOf(this.No) < 0)
            {
                this.RelateMover(this.No, this.Level);
                dupnolist.Add(this.No);
                dupanalyzelist.Add(this);
                rawanalyzelist.Add(this);

                AddNewRowToDataTable(analyzetable, islearn);
            }

            foreach (AnalyzeClass branchanalyze in BranchList)
            {
                branchanalyze.ParentNo = this.No;

                //Fix in 2018/01/13
                branchanalyze.FromNodeString = this.ToNextNodeString();

                DupMaxNo++;

                branchanalyze.InsertDupData(dupnolist, dupanalyzelist, rawanalyzelist, analyzetable, islearn);
            }
        }
        /// <summary>
        /// 將自身的資料加入Table中
        /// </summary>
        /// <param name="analyzetable"></param>
        public void AddNewRowToDataTable(DataTable analyzetable, bool islearn)
        {
            DataRow newdatarow = analyzetable.NewRow();

            newdatarow["No"] = this.No;
            newdatarow["ParentNo"] = this.ParentNo;
            newdatarow["Name"] = this.ToAnalyzeString();
            newdatarow["NO"] = this.No;
            newdatarow["LV"] = this.Level;

            //if(islearn)
            //    newdatarow["LN"] = this.LearnNo;
            //else
            newdatarow["LN"] = this.LearnList.Count;

            analyzetable.Rows.Add(newdatarow);
        }

        public void UpdateRowToDataTable(DataTable datatable)
        {
            foreach (DataRow datarow in datatable.Rows)
            {
                //if (islearn)
                //    newdatarow["LN"] = this.LearnNo;
                //else
                if (this.No == (uint)datarow["No"])
                {
                    datarow["LN"] = this.LearnList.Count;
                    break;
                }
                //analyzetable.Rows.Add(newdatarow);
            }
        }
        public int GetFirstSelectedMoverIndex()
        {
            int i = 0;
            int ret = myMover.Count - 1;

            GraphicalObject grobj;

            while (i < myMover.Count)
            {
                grobj = myMover[i].Source;

                if ((grobj as GeoFigure).IsSelected)
                {
                    ret = i;
                    break;
                }
                i++;
            }
            return ret;
        }
        //public void SelectAllMover(bool iswithfirstselected)
        //{
        //    int i = 0;

        //    GraphicalObject grobj;

        //    while(i < myMover.Count)
        //    {
        //        grobj = myMover[i].Source;

        //        if(iswithfirstselected)
        //            (grobj as GeoFigure).IsFirstSelected = (i == 0);
        //        else
        //            (grobj as GeoFigure).IsFirstSelected = false;

        //        (grobj as GeoFigure).IsSelected = true;

        //        i++;
        //    }
        //}
        //public void UnSelectAllMover()
        //{
        //    int i = 0;

        //    GraphicalObject grobj;

        //    while (i < myMover.Count)
        //    {
        //        grobj = myMover[i].Source;

        //        (grobj as GeoFigure).IsFirstSelected = false;
        //        (grobj as GeoFigure).IsSelected = false;

        //        i++;
        //    }
        //}
        public void AddShape(ShapeEnum shape, Mover movercollection)
        {
            int i = 0;

            GraphicalObject grobj;

            grobj = myMover[GetFirstSelectedMoverIndex()].Source;

            RectangleF FromRectF = (grobj as GeoFigure).RealRectangleAround(0, 0);

            (grobj as GeoFigure).IsSelected = false;
            (grobj as GeoFigure).IsFirstSelected = false;

            FromRectF.Offset(100, 100);

            ReviseShape(shape, FromRectF, true, true, myMover.Count - 1, movercollection, 0);
        }
        public void DupShape(ShapeEnum shape)
        {
            int i = 0;

            GraphicalObject grobj;

            grobj = myMover[myMover.Count - 1].Source;

        }
        public void DelShape(Mover movercollection)
        {
            int i = 0;
            GraphicalObject grobj;

            //先刪除原先在MoverCollection裏的對應
            i = movercollection.Count - 1;
            while (i > -1)
            {
                grobj = movercollection[i].Source;

                if ((grobj as GeoFigure).RelateNo == this.No)
                {
                    movercollection.RemoveAt(i);
                }
                i--;
            }

            //再刪除原先在Mover裏的對應
            i = myMover.Count - 1;
            while (i > -1)
            {
                if (i == 0)
                {
                    i--;
                    continue;
                }

                grobj = myMover[i].Source;

                if ((grobj as GeoFigure).IsSelected)
                {
                    myMover.RemoveAt(i);
                }
                i--;
            }
            //將myMover內重新對應後再加入MoverCollection
            i = 0;
            while (i < myMover.Count)
            {
                grobj = myMover[i].Source;

                (grobj as GeoFigure).RelatePosition = i;
                movercollection.Add(grobj);

                i++;
            }
        }
        public void ReviseShape(ShapeEnum shape, Mover movercollection)
        {
            int i = 0;

            i = movercollection.Count - 1;
            GraphicalObject grobj;

            while (i > -1)
            {
                grobj = movercollection[i].Source;

                if ((grobj as GeoFigure).RelateNo == this.No)
                {
                    movercollection.RemoveAt(i);
                }
                i--;
            }
            //再刪除原先在Mover裏的對應，再加入新的型態
            i = myMover.Count - 1;
            while (i > -1)
            {
                grobj = myMover[i].Source;

                if ((grobj as GeoFigure).IsSelected)
                {
                    RectangleF FromRectF = (grobj as GeoFigure).RealRectangleAround(0, 0);

                    ReviseShape(shape, FromRectF, (grobj as GeoFigure).IsFirstSelected, (grobj as GeoFigure).IsSelected, myMover.Count - 1, null, i);
                }
                i--;
            }
            //將myMover內重新對應後再加入MoverCollection
            i = 0;
            while (i < myMover.Count)
            {
                grobj = myMover[i].Source;

                (grobj as GeoFigure).RelatePosition = i;
                movercollection.Add(grobj);

                i++;
            }
        }
        void ReviseShape(ShapeEnum shape, RectangleF fromrectf, bool isfirstselect, bool isselect, int positioninex, Mover movercollection, int insertindex)
        {
            switch (shape)
            {
                case ShapeEnum.RECT:
                    JzRectEAG jzrect = new JzRectEAG(DefaultColor, fromrectf);

                    jzrect.IsSelected = isselect;
                    jzrect.IsFirstSelected = isfirstselect;

                    jzrect.RelateNo = No;
                    jzrect.RelatePosition = positioninex;
                    jzrect.RelateLevel = Level;

                    if (movercollection != null)
                    {
                        myMover.Add(jzrect);
                        movercollection.Add(jzrect);
                    }
                    else
                    {
                        myMover.RemoveAt(insertindex);
                        myMover.Insert(insertindex, jzrect);
                    }
                    break;
                case ShapeEnum.CIRCLE:
                    JzCircleEAG jzcircle = new JzCircleEAG(DefaultColor, fromrectf);

                    jzcircle.IsSelected = isselect;
                    jzcircle.IsFirstSelected = isfirstselect;

                    jzcircle.RelateNo = No;
                    jzcircle.RelatePosition = positioninex;
                    jzcircle.RelateLevel = Level;

                    if (movercollection != null)
                    {
                        myMover.Add(jzcircle);
                        movercollection.Add(jzcircle);
                    }
                    else
                    {
                        myMover.RemoveAt(insertindex);
                        myMover.Insert(insertindex, jzcircle);
                    }
                    break;
                case ShapeEnum.POLY:
                    JzPolyEAG jzpoly = new JzPolyEAG(DefaultColor, fromrectf);

                    jzpoly.IsSelected = isselect;
                    jzpoly.IsFirstSelected = isfirstselect;

                    jzpoly.RelateNo = No;
                    jzpoly.RelatePosition = positioninex;
                    jzpoly.RelateLevel = Level;

                    if (movercollection != null)
                    {
                        myMover.Add(jzpoly);
                        movercollection.Add(jzpoly);
                    }
                    else
                    {
                        myMover.RemoveAt(insertindex);
                        myMover.Insert(insertindex, jzpoly);
                    }
                    break;
                case ShapeEnum.CAPSULE:
                    JzStripEAG jzstrip = new JzStripEAG(DefaultColor, fromrectf);

                    jzstrip.IsSelected = isselect;
                    jzstrip.IsFirstSelected = isfirstselect;

                    jzstrip.RelateNo = No;
                    jzstrip.RelatePosition = positioninex;
                    jzstrip.RelateLevel = Level;

                    if (movercollection != null)
                    {
                        myMover.Add(jzstrip);
                        movercollection.Add(jzstrip);
                    }
                    else
                    {
                        myMover.RemoveAt(insertindex);
                        myMover.Insert(insertindex, jzstrip);
                    }
                    break;
                case ShapeEnum.RING:
                case ShapeEnum.ORING:

                    JzRingEAG jzring;

                    if (shape == ShapeEnum.RING)
                        jzring = new JzRingEAG(DefaultRingColor, Figure_EAG.Ring, fromrectf);
                    else
                        jzring = new JzRingEAG(DefaultRingColor, Figure_EAG.ORing, fromrectf);

                    jzring.IsSelected = isselect;
                    jzring.IsFirstSelected = isfirstselect;

                    jzring.RelateNo = No;
                    jzring.RelatePosition = positioninex;
                    jzring.RelateLevel = Level;

                    if (movercollection != null)
                    {
                        myMover.Add(jzring);
                        movercollection.Add(jzring);
                    }
                    else
                    {
                        myMover.RemoveAt(insertindex);
                        myMover.Insert(insertindex, jzring);
                    }
                    break;
                case ShapeEnum.RECTRECT:
                case ShapeEnum.HEXHEX:

                    JzIdentityHoleEAG jzidhole;

                    if (shape == ShapeEnum.RECTRECT)
                        jzidhole = new JzIdentityHoleEAG(DefaultColor, Figure_EAG.RectRect, fromrectf);
                    else
                        jzidhole = new JzIdentityHoleEAG(DefaultColor, Figure_EAG.HexHex, fromrectf);

                    jzidhole.IsSelected = isselect;
                    jzidhole.IsFirstSelected = isfirstselect;

                    jzidhole.RelateNo = No;
                    jzidhole.RelatePosition = positioninex;
                    jzidhole.RelateLevel = Level;

                    if (movercollection != null)
                    {
                        myMover.Add(jzidhole);
                        movercollection.Add(jzidhole);
                    }
                    else
                    {
                        myMover.RemoveAt(insertindex);
                        myMover.Insert(insertindex, jzidhole);
                    }
                    break;
                case ShapeEnum.RECTO:
                case ShapeEnum.HEXO:

                    JzCircleHoleEAG jzcirclehole;

                    if (shape == ShapeEnum.RECTO)
                        jzcirclehole = new JzCircleHoleEAG(DefaultColor, Figure_EAG.RectO, fromrectf);
                    else
                        jzcirclehole = new JzCircleHoleEAG(DefaultColor, Figure_EAG.HexO, fromrectf);

                    jzcirclehole.IsSelected = isselect;
                    jzcirclehole.IsFirstSelected = isfirstselect;

                    jzcirclehole.RelateNo = No;
                    jzcirclehole.RelatePosition = positioninex;
                    jzcirclehole.RelateLevel = Level;

                    if (movercollection != null)
                    {
                        myMover.Add(jzcirclehole);
                        movercollection.Add(jzcirclehole);
                    }
                    else
                    {
                        myMover.RemoveAt(insertindex);
                        myMover.Insert(insertindex, jzcirclehole);
                    }
                    break;
            }
        }
        /// <summary>
        /// 取得按下區域外框後檢視的方塊
        /// </summary>
        /// <param name="showmover"></param>
        public void GetShowMover(Mover showmover, Bitmap bmp)
        {
            RectangleF rectf = CreateOPRectF();



            Bitmap bmpshowpattern = new Bitmap(1, 1);
            Bitmap bmpshowmask = new Bitmap(1, 1);

            if (Level > 1)
            {
                JzRectEAG jzrect = new JzRectEAG(Color.FromArgb(5, Color.Purple), rectf, true);
                showmover.Add(jzrect);

                bool ishavecornerorwh = AOIPara.IsHaveCornerOrWH;

                if (ishavecornerorwh)
                {
                    bmpshowpattern.Dispose();
                    bmpshowpattern = (Bitmap)bmp.Clone(rectf, PixelFormat.Format32bppArgb);

                    bmpshowmask.Dispose();
                    bmpshowmask = new Bitmap(bmpshowpattern.Width, bmpshowpattern.Height, PixelFormat.Format32bppArgb);

                    //if (AOIPara.CheckDirtMethod != CheckDirtMethodEnum.NONE)
                    {
                        switch (ALIGNPara.AlignMode)
                        {
                            case AlignModeEnum.BORDER:
                                DrawRect(bmpshowmask, new SolidBrush(Color.White));
                                break;
                        }
                    }

                    A0202_CreateAlignMask(new PointF(rectf.X, rectf.Y), bmpshowmask);

                    //bmpshowpattern.Save(Universal.TESTPATH + "\\ANALYZETEST\\SHOWPATTERN" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                    //bmpshowmask.Save(Universal.TESTPATH + "\\ANALYZETEST\\SHOWMASK" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                }

                AOIPara.GetShowMovers(rectf, showmover, bmpshowpattern, bmpshowmask);

                if (ishavecornerorwh)
                {
                    DrawImage(bmpshowpattern, bmp, rectf);
                    //bmp.Save(Universal.TESTPATH + "\\ANALYZETEST\\PASTED" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                }
            }

            foreach (AnalyzeClass branchanalyze in BranchList)
            {
                branchanalyze.GetShowMover(showmover, bmp);
            }


            bmpshowpattern.Dispose();
            bmpshowmask.Dispose();
        }
        /// <summary>
        /// 取得在一般檢視時給人看見的方框
        /// </summary>
        /// <param name="showmover"></param>
        /// <param name="biaslocation"></param>
        /// <param name="sizeratio"></param>
        /// <param name="colorindex"></param>
        /// <param name="offset"></param>
        public void GetShowMover(Mover showmover, PointF biaslocation, SizeF sizeratio, int colorindex, Point offset)
        {
            Color showcolor = Color.Red;

            #region Define ColorIndex
            switch (colorindex % 7)
            {
                case 0:
                    showcolor = Color.Red;
                    break;
                case 1:
                    showcolor = Color.Orange;
                    break;
                case 2:
                    showcolor = Color.Yellow;
                    break;
                case 3:
                    showcolor = Color.Lime;
                    break;
                case 4:
                    showcolor = Color.DeepSkyBlue;
                    break;
                case 5:
                    showcolor = Color.LightSalmon;
                    break;
                case 6:
                    showcolor = Color.LightPink;
                    break;
            }
            #endregion

            if (Level != 1)
                FromMoverString(showmover, ToMoverString(), showcolor, biaslocation, sizeratio, offset);

            foreach (AnalyzeClass branchanalyze in BranchList)
            {
                branchanalyze.GetShowMover(showmover, biaslocation, sizeratio, colorindex, offset);
            }
        }
        /// <summary>
        /// 取得在結果圖裏的方框
        /// </summary>
        /// <param name="showmover"></param>
        /// <param name="biaslocation"></param>
        /// <param name="sizeratio"></param>
        /// <param name="colorindex"></param>
        /// <param name="offset"></param>
        public void GetShowResultMover(Mover showmover, PointF biaslocation, SizeF sizeratio, int colorindex, Point offset)
        {
            if (!IsOperated)
                return;

            Color showcolor = Color.Red;

            if (IsVeryGood)
                showcolor = Color.Lime;
            else
                showcolor = Color.Red;

            if (!INI.ISONLYSHOWNG)
            {
                if (Level != 1)
                    FromMoverString(showmover, ToMoverString(), showcolor, biaslocation, sizeratio, offset);
            }
            else
            {
                if (!IsVeryGood)
                {
                    FromMoverString(showmover, ToMoverString(), showcolor, biaslocation, sizeratio, offset);

                    //ADD GAARA
                    FromMoverString(showmover, ToMoverString(), HeightReasonsColors, biaslocation, sizeratio, offset);
                }
                else
                {
                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_X6:
                        case JetEazy.OptionEnum.MAIN_SERVICE:
                            if (OCRPara.OCRMethod == OCRMethodEnum.DATAMATRIX
                                || OCRPara.OCRMethod == OCRMethodEnum.DATAMATRIXGRADE)
                                FromMoverString(showmover, ToMoverString(), showcolor, biaslocation, sizeratio, offset);

                            ////ADD GAARA
                            //FromMoverString(showmover, ToMoverString(), HeightReasonsColors, biaslocation, sizeratio, offset);
                            break;
                        default:
                            break;
                    }
                }
            }

            switch(OPTION)
            {
                case OptionEnum.MAIN_X6:
                case JetEazy.OptionEnum.MAIN_SERVICE:
                    foreach (AnalyzeClass branchanalyze in BranchList)
                    {
                        //if (branchanalyze.AliasName == "ESC")
                        //    branchanalyze.AliasName = "ESC";

                        branchanalyze.GetShowResultMover(showmover, biaslocation, sizeratio, colorindex, offset);

                        if (branchanalyze != null)
                        {
                            if (branchanalyze.Level == 2)
                            {
                                GraphicalObject grobj;

                                for (int i = showmover.Count - 1; i >= 0; i--)
                                {
                                    grobj = showmover[i].Source;
                                    branchanalyze.myDrawAnalyzeStrRectF = (grobj as GeoFigure).RealRectangleAround(0, 0);
                                    break;
                                }
                            }
                        }
                    }
                    break;
                default:
                    foreach (AnalyzeClass branchanalyze in BranchList)
                    {
                        //if (branchanalyze.AliasName == "ESC")
                        //    branchanalyze.AliasName = "ESC";

                        branchanalyze.GetShowResultMover(showmover, biaslocation, sizeratio, colorindex, offset);

                        if (branchanalyze != null)
                        {
                            if (branchanalyze.Level == 2)
                            {
                                GraphicalObject grobj;

                                for (int i = showmover.Count - 1; i >= 0; i--)
                                {
                                    grobj = showmover[i].Source;
                                    branchanalyze.myDrawAnalyzeStrRectF = (grobj as GeoFigure).RealRectangleAround(0, 0);
                                    break;
                                }
                            }
                        }
                    }
                    break;
            }

           
        }
        /// <summary>
        /// 取得 PassInfo 裏指定的 Analyze
        /// </summary>
        /// <param name="passinfo"></param>
        /// <returns></returns>
        public AnalyzeClass GetAnalyze(PassInfoClass passinfo, LearnOperEnum learnop)
        {
            AnalyzeClass retanalzye = null;

            int checkno = -1;
            int learnno = -1;

            switch (learnop)
            {
                case LearnOperEnum.THIS:
                    checkno = passinfo.AnalyzeNo;
                    learnno = passinfo.AnalyzeLearnNo;
                    break;
                case LearnOperEnum.PARENT:
                    checkno = passinfo.ParentNo;
                    learnno = passinfo.ParentLearnNo;
                    break;
            }

            if (this.No == checkno && this.LearnNo == learnno)
            {
                retanalzye = this;
            }
            else
            {
                foreach (AnalyzeClass branchanalyze in BranchList)
                {
                    retanalzye = branchanalyze.GetAnalyze(passinfo, learnop);

                    if (retanalzye != null)
                        break;
                }
            }

            return retanalzye;
        }
        public int GetAnalyzeMaxNo()
        {
            int maxno = -1000;


            if (No > maxno)
            {
                maxno = No;
            }

            foreach (AnalyzeClass branchanalyze in BranchList)
            {
                int bmaxno = branchanalyze.GetAnalyzeMaxNo();

                if (bmaxno > maxno)
                {
                    maxno = bmaxno;
                }
            }
            foreach (AnalyzeClass learnanalyze in LearnList)
            {
                int lmaxno = learnanalyze.GetAnalyzeMaxNo();

                if (lmaxno > maxno)
                {
                    maxno = lmaxno;
                }
            }

            return maxno;
        }

        #region 合併用功能
        public void ClearBranchList()
        {
            foreach (AnalyzeClass branch in BranchList)
            {
                branch.ClearBranchList();
            }
            BranchList.Clear();
        }
        public bool CheckAnalyeInside(AnalyzeClass analyze)
        {
            bool isinside = false;

            RectangleF insiderect = analyze.GetMyMoverRectF();
            RectangleF checkrect = this.GetMyMoverRectF();

            float orgarea = insiderect.Width * insiderect.Height;

            insiderect.Intersect(checkrect);

            float newarea = insiderect.Width * insiderect.Height;

            if ((newarea / orgarea) > 0.95f)
            {
                analyze.ParentNo = this.No;
                analyze.Level = this.Level + 1;

                isinside = true;
            }

            return isinside;
        }

        public void SetRelateASN(ASNCollectionClass asncollection, int envindex, int pageindex, int pageoptypeindex)
        {
            foreach (ASNClass asn in asncollection.myDataList)
            {

                string str = asn.ToASNString();


                if (RelateASN == str)
                {
                    foreach (ASNItemClass asnitem in asn.ASNItemList)
                    {

                        string str1 = asnitem.ToASNItemRelateString();

                        if (RelateASNItem == str1)
                        {
                            asnitem.RelateAnalyzeStr += envindex.ToString() + "-"
                                + pageindex.ToString() + "-"
                                + pageoptypeindex.ToString() + "-"
                                + No.ToString() + ";";
                        }
                    }
                }
            }
            foreach (AnalyzeClass analyze in BranchList)
            {
                analyze.SetRelateASN(asncollection, envindex, pageindex, pageoptypeindex);
            }
        }

        public AnalyzeClass GetAnalyze(int analyzeno)
        {
            AnalyzeClass retanalyze = null;

            if (this.No == analyzeno)
            {
                retanalyze = this;
            }
            if (retanalyze == null)
            {
                foreach (AnalyzeClass analyze in BranchList)
                {
                    retanalyze = analyze.GetAnalyze(analyzeno);


                    if (retanalyze != null)
                        break;
                }

            }
            return retanalyze;
        }

        public void RestorePatternImage()
        {
            bmpPATTERN.Dispose();
            bmpPATTERN = new Bitmap(AnalyzeBackup.bmpPATTERN);
        }

        #endregion

        #region 明細列表功能用

        /// <summary>
        /// 取得 Column 的 Header
        /// </summary>
        /// <returns></returns>
        public static DataColumn[] GetDataColums(VersionEnum ver, OptionEnum opt)
        {
            int i = 0;
            int datacolumncount;
            DataColumn[] datacolumns = null;

            int startindex = 0;

            switch (ver)
            {
                case VersionEnum.ALLINONE:

                    datacolumncount = 72;
                    datacolumns = new DataColumn[datacolumncount];

                    startindex = 0;
                    datacolumns[startindex] = new DataColumn("ParentNo", typeof(UInt32));    //一定要用UInt32
                    startindex++;
                    datacolumns[startindex] = new DataColumn("No", typeof(UInt32));               //一定要用UInt32
                    startindex++;
                    datacolumns[startindex] = new DataColumn("Name", typeof(String));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("LV", typeof(UInt32));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("LN", typeof(UInt32));
                    startindex++;

                    #region Normal Para
                    datacolumns[startindex] = new DataColumn("AliasName", typeof(String));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("Brightness", typeof(int));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("Contrast", typeof(int));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("MaskMethod", typeof(MaskMethodEnum));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("ExtendX", typeof(int));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("ExtendY", typeof(int));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("RelateASN", typeof(string));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("RelateASNItem", typeof(string));
                    startindex++;

                    #endregion

                    #region Align Para
                    datacolumns[startindex] = new DataColumn("A.Method", typeof(AlignMethodEnum));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("A.Mode", typeof(AlignModeEnum));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("S.Size", typeof(int));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("Canny Auto", typeof(bool));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("Canny H.", typeof(int));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("Canny L.", typeof(int));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("Rotation", typeof(float));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("Scaling", typeof(float));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("Max Occ", typeof(int));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("A.Tolerance", typeof(float));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("Offset", typeof(float));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("Resolution", typeof(float));
                    startindex++;
                    #endregion

                    #region Inspection Para
                    datacolumns[startindex] = new DataColumn("I.Method", typeof(InspectionMethodEnum));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("Count", typeof(int));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("Area", typeof(int));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("I.Tolerance", typeof(int));
                    startindex++;
                    #endregion

                    #region Measure Para

                    datacolumns[startindex] = new DataColumn("M.Method", typeof(MeasureMethodEnum));
                    startindex++;
                    //datacolumns[startindex] = new DataColumn("M.Tolerance", typeof(float));
                    //startindex++;
                    datacolumns[startindex] = new DataColumn("OP String", typeof(string));
                    startindex++;
                    //datacolumns[startindex] = new DataColumn("Max Gap", typeof(float));
                    //startindex++;
                    //datacolumns[startindex] = new DataColumn("Min Gap", typeof(float));
                    //startindex++;
                    //datacolumns[startindex] = new DataColumn("Pixel Gap", typeof(float));
                    //startindex++;
                    //datacolumns[startindex] = new DataColumn("HT Ratio", typeof(float));
                    //startindex++;
                    //datacolumns[startindex] = new DataColumn("WH Ratio", typeof(float));
                    //startindex++;
                    #endregion

                    #region OCR Para
                    datacolumns[startindex] = new DataColumn("O.Method", typeof(OCRMethodEnum));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("OCR Para", typeof(string));
                    startindex++;
                    #endregion

                    #region AOI Para
                    datacolumns[startindex] = new DataColumn("AOI Method", typeof(AOIMethodEnum));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("Check Dirt", typeof(CheckDirtMethodEnum));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("Check Color", typeof(UselessMethodEnum));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("D.Ratio", typeof(float));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("D.Area", typeof(int));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("C.Ratio", typeof(float));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("TC.Ratio", typeof(float));
                    startindex++;

                    datacolumns[startindex] = new DataColumn("Width Diff", typeof(float));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("Width R.Ratio", typeof(float));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("Width T.Ratio", typeof(float));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("Width Brightness", typeof(int));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("Width Contrast", typeof(int));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("Width Sampling", typeof(int));
                    startindex++;

                    datacolumns[startindex] = new DataColumn("Height Diff", typeof(float));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("Height R.Ratio", typeof(float));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("Height T.Ratio", typeof(float));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("Height Brightness", typeof(int));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("Height Contrast", typeof(int));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("Height Sampling", typeof(int));
                    startindex++;

                    datacolumns[startindex] = new DataColumn("LT SimRatio", typeof(float));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("LT Width", typeof(int));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("LT Height", typeof(int));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("LT Brightness", typeof(int));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("LT Contrast", typeof(int));
                    startindex++;

                    datacolumns[startindex] = new DataColumn("RT SimRatio", typeof(float));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("RT Width", typeof(int));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("RT Height", typeof(int));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("RT Brightness", typeof(int));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("RT Contrast", typeof(int));
                    startindex++;

                    datacolumns[startindex] = new DataColumn("LB SimRatio", typeof(float));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("LB Width", typeof(int));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("LB Height", typeof(int));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("LB Brightness", typeof(int));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("LB Contrast", typeof(int));
                    startindex++;

                    datacolumns[startindex] = new DataColumn("RB SimRatio", typeof(float));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("RB Width", typeof(int));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("RB Height", typeof(int));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("RB Brightness", typeof(int));
                    startindex++;
                    datacolumns[startindex] = new DataColumn("RB Contrast", typeof(int));

                    #endregion

                    i = 0;
                    foreach (DataColumn datacolumn in datacolumns)
                    {
                        switch (i)
                        {
                            default:
                                datacolumn.ReadOnly = true;
                                break;
                        }

                        i++;
                    }
                    break;
                case VersionEnum.AUDIX:

                    break;
            }

            return datacolumns;
        }

        /// <summary>
        /// 把 Row 的資料填入 Table 中
        /// </summary>
        /// <param name="dtble"></param>
        /// <returns></returns>
        public void AddNewRow(DataTable dtble)
        {
            DataRow datarow = dtble.NewRow();

            int startindex = 0;

            startindex = 0;
            datarow[startindex] = ParentNo;
            startindex++;
            datarow[startindex] = No;
            startindex++;
            datarow[startindex] = ToAnalyzeString();
            startindex++;
            datarow[startindex] = Level;
            startindex++;
            datarow[startindex] = LearnList.Count;
            startindex++;

            datarow[startindex] = AliasName;
            startindex++;
            datarow[startindex] = Brightness;
            startindex++;
            datarow[startindex] = Contrast;
            startindex++;
            datarow[startindex] = MaskMethod;
            startindex++;
            datarow[startindex] = ExtendX;
            startindex++;
            datarow[startindex] = ExtendY;
            startindex++;
            datarow[startindex] = RelateASN;
            startindex++;
            datarow[startindex] = RelateASNItem;
            startindex++;

            datarow[startindex] = ALIGNPara.AlignMethod;
            startindex++;
            datarow[startindex] = ALIGNPara.AlignMode;
            startindex++;
            datarow[startindex] = ALIGNPara.MTPSample;
            startindex++;
            datarow[startindex] = ALIGNPara.MTCannyAuto;
            startindex++;
            datarow[startindex] = ALIGNPara.MTCannyH;
            startindex++;
            datarow[startindex] = ALIGNPara.MTCannyL;
            startindex++;
            datarow[startindex] = ALIGNPara.MTRotation;
            startindex++;
            datarow[startindex] = ALIGNPara.MTScaling;
            startindex++;
            datarow[startindex] = ALIGNPara.MTMaxOcc;
            startindex++;
            datarow[startindex] = ALIGNPara.MTTolerance;
            startindex++;
            datarow[startindex] = ALIGNPara.MTOffset;
            startindex++;
            datarow[startindex] = ALIGNPara.MTResolution;
            startindex++;

            datarow[startindex] = INSPECTIONPara.InspectionMethod;
            startindex++;
            datarow[startindex] = INSPECTIONPara.IBCount;
            startindex++;
            datarow[startindex] = INSPECTIONPara.IBArea;
            startindex++;
            datarow[startindex] = INSPECTIONPara.IBTolerance;
            startindex++;
            datarow[startindex] = INSPECTIONPara.InspectionAB;
            startindex++;


            datarow[startindex] = MEASUREPara.MeasureMethod;
            startindex++;
            //datarow[startindex] = MEASUREPara.MMTolerance;
            //startindex++;
            datarow[startindex] = MEASUREPara.MMOPString;
            startindex++;
            //datarow[startindex] = MEASUREPara.MMMaxGap;
            //startindex++;
            //datarow[startindex] = MEASUREPara.MMMinGap;
            //startindex++;
            //datarow[startindex] = MEASUREPara.MMPixelGap;
            //startindex++;
            //datarow[startindex] = MEASUREPara.MMHTRatio;
            //startindex++;
            //datarow[startindex] = MEASUREPara.MMWholeRatio;
            //startindex++;

            datarow[startindex] = OCRPara.OCRMethod;
            startindex++;
            datarow[startindex] = OCRPara.OCRMappingMethod;
            startindex++;

            datarow[startindex] = AOIPara.AOIMethod;
            startindex++;
            datarow[startindex] = AOIPara.CheckDirtMethod;
            startindex++;
            datarow[startindex] = AOIPara.CheckColorMethod;
            startindex++;
            datarow[startindex] = AOIPara.DirtRatio;
            startindex++;
            datarow[startindex] = AOIPara.DirtArea;
            startindex++;
            datarow[startindex] = AOIPara.ColorRatio;
            startindex++;
            datarow[startindex] = AOIPara.TotalColorRatio;
            startindex++;

            datarow[startindex] = AOIPara.WHArray[(int)PositionEnum.XDir].Diff;
            startindex++;
            datarow[startindex] = AOIPara.WHArray[(int)PositionEnum.XDir].RangeRatio;
            startindex++;
            datarow[startindex] = AOIPara.WHArray[(int)PositionEnum.XDir].ThresholdRatio;
            startindex++;
            datarow[startindex] = AOIPara.WHArray[(int)PositionEnum.XDir].Brightness;
            startindex++;
            datarow[startindex] = AOIPara.WHArray[(int)PositionEnum.XDir].Contrast;
            startindex++;
            datarow[startindex] = AOIPara.WHArray[(int)PositionEnum.XDir].SampleGap;
            startindex++;

            datarow[startindex] = AOIPara.WHArray[(int)PositionEnum.YDir].Diff;
            startindex++;
            datarow[startindex] = AOIPara.WHArray[(int)PositionEnum.YDir].RangeRatio;
            startindex++;
            datarow[startindex] = AOIPara.WHArray[(int)PositionEnum.YDir].ThresholdRatio;
            startindex++;
            datarow[startindex] = AOIPara.WHArray[(int)PositionEnum.YDir].Brightness;
            startindex++;
            datarow[startindex] = AOIPara.WHArray[(int)PositionEnum.YDir].Contrast;
            startindex++;
            datarow[startindex] = AOIPara.WHArray[(int)PositionEnum.YDir].SampleGap;
            startindex++;

            datarow[startindex] = AOIPara.CornerArray[(int)CornerEnum.LT].Tolerance;
            startindex++;
            datarow[startindex] = AOIPara.CornerArray[(int)CornerEnum.LT].Width;
            startindex++;
            datarow[startindex] = AOIPara.CornerArray[(int)CornerEnum.LT].Height;
            startindex++;
            datarow[startindex] = AOIPara.CornerArray[(int)CornerEnum.LT].Brightness;
            startindex++;
            datarow[startindex] = AOIPara.CornerArray[(int)CornerEnum.LT].Contrast;
            startindex++;

            datarow[startindex] = AOIPara.CornerArray[(int)CornerEnum.RT].Tolerance;
            startindex++;
            datarow[startindex] = AOIPara.CornerArray[(int)CornerEnum.RT].Width;
            startindex++;
            datarow[startindex] = AOIPara.CornerArray[(int)CornerEnum.RT].Height;
            startindex++;
            datarow[startindex] = AOIPara.CornerArray[(int)CornerEnum.RT].Brightness;
            startindex++;
            datarow[startindex] = AOIPara.CornerArray[(int)CornerEnum.RT].Contrast;
            startindex++;

            datarow[startindex] = AOIPara.CornerArray[(int)CornerEnum.LB].Tolerance;
            startindex++;
            datarow[startindex] = AOIPara.CornerArray[(int)CornerEnum.LB].Width;
            startindex++;
            datarow[startindex] = AOIPara.CornerArray[(int)CornerEnum.LB].Height;
            startindex++;
            datarow[startindex] = AOIPara.CornerArray[(int)CornerEnum.LB].Brightness;
            startindex++;
            datarow[startindex] = AOIPara.CornerArray[(int)CornerEnum.LB].Contrast;
            startindex++;

            datarow[startindex] = AOIPara.CornerArray[(int)CornerEnum.RB].Tolerance;
            startindex++;
            datarow[startindex] = AOIPara.CornerArray[(int)CornerEnum.RB].Width;
            startindex++;
            datarow[startindex] = AOIPara.CornerArray[(int)CornerEnum.RB].Height;
            startindex++;
            datarow[startindex] = AOIPara.CornerArray[(int)CornerEnum.RB].Brightness;
            startindex++;
            datarow[startindex] = AOIPara.CornerArray[(int)CornerEnum.RB].Contrast;

            dtble.Rows.Add(datarow);
        }
        /// <summary>
        /// 把 Row 的資料寫回 Analyze 中，看是否為相對應的Row
        /// </summary>
        /// <param name="datarow"></param>
        public void SetData(DataRow datarow)
        {

            this.IsUsed = true;


        }

        /// <summary>
        /// 設定新的 PageNo 給 Static Album 操作使用
        /// </summary>
        /// <param name="pageno"></param>
        public void SetPageNo(int pageno)
        {
            PageNo = pageno;

            PassInfo.PageNo = pageno;

            foreach (AnalyzeClass learnanalyze in LearnList)
            {
                learnanalyze.SetPageNo(pageno);
            }
            foreach (AnalyzeClass branchanalyze in BranchList)
            {
                branchanalyze.SetPageNo(pageno);
            }
        }

        #endregion


        #endregion

        #region Align Operation

        /// <summary>
        /// 做其他的測試前預先做定位
        /// </summary>
        /// <param name="bmpinput"></param>
        /// <param name="offsetptf"></param>
        /// <returns></returns>
        public bool A00_Train(Bitmap bmpinput, PointF offsetptf)
        {
            return A00_Train(bmpinput, offsetptf, false, false);
        }
        public bool A00_Train(Bitmap bmpinput, bool ispopup, PointF offsetptf)
        {
            return A00_Train(bmpinput, offsetptf, false, ispopup);
        }
        public bool A00_Train(Bitmap bmpinput, PointF offsetptf, bool islearn)
        {
            return A00_Train(bmpinput, offsetptf, islearn, false);
        }
        /// <summary>
        /// 做其他的測試前預先做定位
        /// </summary>
        /// <param name="bmpinput"></param>
        /// <param name="offsetptf"></param>
        /// <param name="islearn"></param>
        /// <returns></returns>
        public bool A00_Train(Bitmap bmpinput, PointF offsetptf, bool islearn, bool ispopup, bool IsMultiThread = false)
        {
            bool isgood = true;

            string str = "";
            str = " Start " + ToLogAnalyzeString() + " Training...";

            if (ispopup)
            {
                ShowTrainMessage(str);
                ShowTrainMessage("");
            }


            if (IsTempSave)
            {
                if (!islearn)
                    bmpinput.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-000" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                else
                    bmpPATTERN.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-PATTERN" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            }


            //if (islearn)
            //    islearn = islearn;

            isgood &= A02_CreateTrainRequirement(bmpinput, offsetptf, islearn);

            //if (IsTempSave)
            //{
            //    bmpPATTERN.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-PATTERN" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            //    bmpWIP.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-WIP" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            //    bmpMASK.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-MASK" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            //}


            if (isgood)
                isgood &= A03_TrainMeAndSubTreeWithA02(IsMultiThread);

            //FillTrainStatus(TrainStatusCollection);

            if (ispopup)
            {
                ShowTrainMessage(TrainStatusCollection);
            }

            return isgood;
        }
        /// <summary>
        /// 於做其他的測試前預先做定位
        /// </summary>
        /// <param name="bmpinput"></param>
        public bool A01_Run(Bitmap bmpinput)
        {
            //bool isgood = true;

            //isgood &= A07_CreateAlignRunRequirement(bmpinput);

            //if (isgood)
            //    isgood &= A08_RunMeAndSubTreeWithA07();

            //return isgood;

            if (bmpinput == null)
            {

            }

            return A01_Run(bmpinput, false);
        }
        public bool A01_Run(Bitmap bmpinput, bool islearn)
        {
            bool isgood = true;



            isgood &= A07_CreateAlignRunRequirement(bmpinput, islearn);

            if (isgood)
                isgood &= A08_RunMeAndSubTreeWithA07();

            return isgood;
        }
        public bool A02_CreateTrainRequirement(Bitmap bmpinput, PointF offsetpointf)
        {
            return A02_CreateTrainRequirement(bmpinput, offsetpointf, false);
        }
        /// <summary>
        /// 取得定位前的所有資料
        /// </summary>
        /// <param name="bmpinput"></param>
        /// <param name="offsetpointf"></param>
        bool A02_CreateTrainRequirement(Bitmap bmpinput, PointF offsetpointf, bool islearn)
        {
            bool isroot = Level == 1;
            bool isgood = true;

            string str = "";

            string analyzestring = ToLogAnalyzeString();

            WorkStatusClass workstatus = new WorkStatusClass(AnanlyzeProcedureEnum.PREPARE);
            string processstring = "Start " + analyzestring + " Train Prepare." + Environment.NewLine;
            string errorstring = "";
            ReasonEnum reason = ReasonEnum.PASS;

            #region 產生校正用的MASK

            str = "Create Align Mask.";
            processstring += str + Environment.NewLine;

            if (!islearn)
            {
                if (isroot)
                {
                    processstring += analyzestring + " is Root." + Environment.NewLine;

                    myOPRectF = SimpleRectF(bmpinput.Size);
                    myOringinOffsetPointF = new PointF(offsetpointf.X + myOPRectF.X, offsetpointf.Y + myOPRectF.Y);

                    bmpPATTERN.Dispose();
                    bmpPATTERN = new Bitmap(bmpinput);
                }
                else
                {
                    processstring += analyzestring + " is not Root." + Environment.NewLine;

                    myOPRectF = CreateOPRectF(bmpinput, offsetpointf);
                    myOringinOffsetPointF = new PointF(offsetpointf.X + myOPRectF.X, offsetpointf.Y + myOPRectF.Y);

                    bmpPATTERN.Dispose();
                    bmpPATTERN = (Bitmap)bmpinput.Clone(myOPRectF, PixelFormat.Format32bppArgb);
                }
            }

            bmpWIP.Dispose();
            bmpWIP = new Bitmap(bmpPATTERN);

            //產生 Direct Mask
            if (isroot)
            {
                str = " Generate Direct Mask.";
                processstring += analyzestring + str + Environment.NewLine;

                A0201_GenDirectMask(bmpWIP, new PointF(0, 0));
            }

            bmpMASK.Dispose();
            bmpMASK = new Bitmap(bmpPATTERN.Width, bmpPATTERN.Height, PixelFormat.Format32bppArgb);

            //檢查是否需要檢測髒污的鍵帽，不需要一定得檢查髒污才能用Boarder
            //if (AOIPara.CheckDirtMethod != CheckDirtMethodEnum.NONE)
            {
                switch (ALIGNPara.AlignMode)
                {
                    case AlignModeEnum.BORDER:

                        str = " Use Borader Alignment.";
                        processstring += analyzestring + str + Environment.NewLine;

                        DrawRect(bmpMASK, WhiteMaskBrush);
                        break;
                    default:

                        str = " Use Area Alignment.";
                        processstring += analyzestring + str + Environment.NewLine;

                        DrawRect(bmpMASK, BlackMaskBrush);
                        break;
                }
                //bmpMASK.Save(Universal.TESTPATH + "\\ANALYZETEST\\RELATEMASK" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            }

            //產生 Pattern 及 Pattern 用的 Mask

            //if (AliasName == "LASERTEST")
            //    AliasName = AliasName;

            A0202_CreateAlignMask(myOringinOffsetPointF, bmpMASK);

            //if(AliasName == "LASERTEST")
            //    bmpMASK.Save(Universal.TESTPATH + "\\ANALYZETEST\\LASERNMASK" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);


            #endregion

            //產生 Corner 及 WH 的資料
            if (!isroot)
            {
                if (AOIPara.IsHaveCornerOrWH)
                {
                    str = " Create Corner and WH Mask.";
                    processstring += analyzestring + str + Environment.NewLine;

                    //會損失一些記憶體在Corner 的 Pattern，及 WH 的 Pattern 及 Mask
                    isgood = AOIPara.A00_GetAOIData(bmpWIP, bmpMASK, ToLogAnalyzeString(), PassInfo, ALIGNPara.MTResolution);
                    AOIPara.FillTrainMessage(ref processstring, ToLogString());
                }
            }

            if (!isgood)
                reason = ReasonEnum.NG;

            workstatus.SetWorkStatus(bmpPATTERN, bmpWIP, bmpMASK, reason, errorstring, processstring, PassInfo);
            TrainStatusCollection.Add(workstatus);
            //IsTempSave = true;
            if (IsTempSave && analyzestring == "A02-02-0005_00")
            {
                bmpPATTERN.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-PATTERN" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                bmpWIP.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-WIP" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                bmpMASK.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-MASK" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            }

            return isgood;
        }
        /// <summary>
        /// 產生Direct Mask
        /// </summary>
        /// <param name="bmpinput"></param>
        /// <param name="ptfoffset"></param>
        void A0201_GenDirectMask(Bitmap bmpinput, PointF ptfoffset)
        {
            Graphics grfx = Graphics.FromImage(bmpinput);

            GenDirectMask(grfx, ptfoffset);

            grfx.Dispose();
        }
        /// <summary>
        /// 依Pattern產生MASK
        /// </summary>
        void A0202_CreateAlignMask(PointF ptfoffset, Bitmap bmpmask)
        {
            int i = 0;

            GraphicalObject grobj;
            Graphics grfx = Graphics.FromImage(bmpmask);

            while (i < myMover.Count)
            {
                grobj = myMover[i].Source;

                switch (ALIGNPara.AlignMode)
                {
                    case AlignModeEnum.BORDER:
                        (grobj as GeoFigure).DrawMask(grfx, ptfoffset, 0f, null, BlackMaskBrush, nullSize);
                        break;
                    default:
                        (grobj as GeoFigure).DrawMask(grfx, ptfoffset, 0f, null, WhiteMaskBrush, nullSize);
                        break;
                }

                i++;
            }
            grfx.Dispose();


            //string str = Universal.TESTPATH + "\\ANALYZETEST\\" + ToAnalyzeTestString();

            //if (AliasName == "CAP1")
            //{
            //    bmpPATTERN.Save(str + " PATTERN" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            //    bmpMASK.Save(str + " MASK" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            //}
        }
        /// <summary>
        /// 運算自身的定位及產生子Analyze的圖像及定位，Learn From Here
        /// </summary>
        /// <param name="ismultithread"></param>
        /// <param name="bmpinput"></param>
        /// <param name="offsetpoint"></param>
        /// <returns></returns>
        bool A03_TrainMeAndSubTreeWithA02(bool IsMultiThread = false)
        {
            bool isgood = true;
            bool isaligngood = false;
            string str = "";

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            isgood = A05_AlignTrainProcess();

            string strTmers = "A: " + watch.ElapsedMilliseconds;
            watch.Reset();
            watch.Start();

            //Reduce By Victor Tsai in 2018/02/12
            if (INSPECTIONPara.InspectionMethod == InspectionMethodEnum.NONE && AOIPara.CheckDirtMethod == CheckDirtMethodEnum.NONE && MEASUREPara.MeasureMethod == MeasureMethodEnum.NONE)
            {
                bmpMASK.Dispose();
                bmpMASK = new Bitmap(1, 1);
            }

            if (isgood)
            {
                isgood = A06_RunProcess(true, ref isaligngood);
            }
            strTmers += " B: " + watch.ElapsedMilliseconds;
            watch.Reset();
            watch.Start();

            if (isgood)
            {
                //先把所有要處理的東西準備好
                foreach (AnalyzeClass branchanalyze in BranchList)
                {
                    if (branchanalyze.MaskMethod == MaskMethodEnum.NONE)
                    {
                        str = " Start " + branchanalyze.ToLogAnalyzeString() + " Training...";

                        isgood &= branchanalyze.A02_CreateTrainRequirement(bmpOUTPUT, myOringinOffsetPointF);

                        if (IsTempSave)
                        {
                            branchanalyze.bmpPATTERN.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + branchanalyze.NoSaveStr + "-B_PATTERN" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                            branchanalyze.bmpWIP.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + branchanalyze.NoSaveStr + "-B_WIP" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                            branchanalyze.bmpMASK.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + branchanalyze.NoSaveStr + "-B_MASK" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                        }

                        branchanalyze.FillTrainStatus(TrainStatusCollection, ToLogString());
                        //TrainStatusCollection.SaveProcessAndError(Universal.TESTPATH + "\\M1", ToAnalyzeString());
                    }
                    if (!isgood)
                        break;
                }
                strTmers += " C: " + watch.ElapsedMilliseconds;
                watch.Reset();
                watch.Start();

                if (isgood)
                {
                    //Could Do MultiProcess For This Affair
#if !MULTI
                    //Single Process
                    if (Universal.IsMultiThread && IsMultiThread)
                    {
                        Parallel.ForEach(BranchList, branchanalyze =>
                        {
                            if (branchanalyze.MaskMethod == MaskMethodEnum.NONE)
                            {
                                bool isgoodTemp = branchanalyze.A03_TrainMeAndSubTreeWithA02(true);

                                if (!isgoodTemp)
                                    isgood = false;
                            }
                        });
                    }
                    else
                    {
                        foreach (AnalyzeClass branchanalyze in BranchList)
                        {
                            if (branchanalyze.MaskMethod == MaskMethodEnum.NONE)
                            {
                                isgood &= branchanalyze.A03_TrainMeAndSubTreeWithA02();
                            }
                            if (!isgood)
                                break;
                        }
                    }

                    strTmers += " D: " + watch.ElapsedMilliseconds;
                    watch.Reset();
                    watch.Start();
#endif
#if MULTI
                        //MultiProcess
                        Parallel.ForEach(BranchList, branch =>
                         {
                             if (branch.MaskMethod == MaskMethodEnum.NONE)
                                 isgood &= branch.A03_RunMeAndSubTreeWithA02();
                         });
#endif
                    foreach (AnalyzeClass branchanalyze in BranchList)
                    {
                        if (branchanalyze.MaskMethod == MaskMethodEnum.NONE)
                        {
                            if (branchanalyze.TrainStatusCollection.COUNT > 0)
                            {
                                branchanalyze.FillTrainStatus(TrainStatusCollection, ToLogString());
                            }
                        }
                    }

                    strTmers += " E: " + watch.ElapsedMilliseconds;
                    watch.Reset();
                    watch.Start();

                    //TrainStatusCollection.SaveProcessAndError(Universal.TESTPATH + "\\M2");
                }
                //若都檢測正確，最後需要檢測剩下的地方有沒有髒東西
                if (isaligngood)
                {
                    if (AOIPara.CheckDirtMethod != CheckDirtMethodEnum.NONE && ALIGNPara.AlignMode == AlignModeEnum.BORDER)
                    {
                        DigForDirtCheck();

                        str = ToAnalyzeString() + " Start Dirty Checking...";

                        if (IsTempSave)
                            bmpHOLLOW.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-HOLLOW" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                        //AOIPara.ResetTrainStatus();
                        isgood &= AOIPara.CheckDirt(bmpOUTPUT, bmpHOLLOW, true, PassInfo);
                        AOIPara.FillTrainStatus(TrainStatusCollection, ToLogString());

                        if (IsTempSave)
                            bmpOUTPUT.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-DIRTOUTPUT" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                        if (isgood)
                        {
                            str = "Check Dirt OK.";
                        }
                        else
                        {
                            str = "Check Dirt NG.";
                        }

                        //Reduce By Victor Tsai 018/02/11
                        bmpHOLLOW.Dispose();
                    }
                }

                strTmers += " F: " + watch.ElapsedMilliseconds;
                watch.Reset();
                watch.Start();
            }

            //全處理完才處理 LEARN 的
            foreach (AnalyzeClass learnanalyze in LearnList)
            {
                //learnanalyze.IsTempSave = true;

                //learnanalyze.ResetTrainStatus();
                isgood &= learnanalyze.A00_Train(null, new PointF(0, 0), true);
                learnanalyze.FillTrainStatus(TrainStatusCollection);

                //learnanalyze.IsTempSave = false;

                if (!isgood)
                    break;
            }

            strTmers += " G: " + watch.ElapsedMilliseconds;
            watch.Stop();
            //bmpOUTPUT.Save(Universal.TESTPATH + "\\ANALYZETEST\\RELATEMASK" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

            //TrainStatusCollection.SaveProcessAndError(Universal.TESTPATH + "\\M2", ToLogAnalyzeString());

            return isgood;
        }
        /// <summary>
        /// 執行預對位
        /// </summary>
        /// <returns></returns>
        public bool A05_AlignTrainProcess()
        {
            bool isgood = true;

            //switch (ALIGNPara.AlignMode)
            //{
            //    case AlignModeEnum.BORDER:
            //        bmpHOLLOW.Dispose();
            //        bmpHOLLOW = new Bitmap(bmpMASK);
            //        break;
            //}
            //  if (PageNo == 6)
            //      bmpMASK.Save(Universal.TESTPATH + "\\ANALYZETEST\\ALAIGN MASK " + NoSaveStr + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);


            isgood = ALIGNPara.AlignTrainProcess(bmpWIP, ref bmpPATTERN, bmpMASK, this.Brightness, this.Contrast, ToLogAnalyzeString(), PassInfo, false);
            ALIGNPara.FillTrainStatus(TrainStatusCollection, ToLogString());

            //isgood = MEASUREPara.MeasureProcess(bmpWIP, bmpPATTERN, bmpMASK, this.Brightness, this.Contrast, ToAnalyzeString(), PassInfo, true);
            //MEASUREPara.FillTrainStatus(TrainStatusCollection, ToLogString());

            return isgood;
        }
        bool A06_RunProcess(bool istrain, ref bool isaligngood)
        {
            return A06_RunProcess(istrain, ref isaligngood, false);
        }
        bool A06_RunProcess(bool istrain, ref bool isaligngood, bool isoutputfilltrain)
        {
            bool isroot = Level == 1;
            bool isgood = true;
            Bitmap bmpALIGNED = new Bitmap(1, 1);

            //Universal.isAutoDebug = true;
            //if (PageNo == 0)
            //{
            if (istrain)
            {
                if (AliasName == "")
                {
                    if (NORMALPara.Name == "")
                        NORMALPara.Name = ToAnalyzeString();
                    AliasName = NORMALPara.Name;
                    NORMALPara.AliasName = AliasName;
                }
            }
            //{
            //    // bmpWIP.Save(Universal.TESTPATH + "\\ANALYZETEST\\ALAIGN WIP " + NoSaveStr + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            //    bmpPATTERN.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-A_PATTERN" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            //    bmpWIP.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-A_WIP" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            //    bmpMASK.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-A_MASK" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            //}
            //}
            // 1. 量測 Corner 及 WH 的資料
            if (!isroot)
            {
                if (AOIPara.IsHaveCornerOrWH)
                {
                    //str = "Create Corner and WH Mask.";
                    isgood = AOIPara.A08_RunAOIData(bmpWIP, istrain);

                    if (istrain)
                        AOIPara.FillTrainStatus(TrainStatusCollection, ToLogString());
                    else
                    {
                        if (isoutputfilltrain)
                            AOIPara.FillRunStatus(TrainStatusCollection, ToLogString());
                        else
                            AOIPara.FillRunStatus(RunStatusCollection, ToLogString());
                    }
                }
            }
            // 2. 檢查 Align 是否正確
            if (isgood
                //&& ALIGNPara.AlignMethod != AlignMethodEnum.NONE
                )
            {
                //if (PageNo == 3)
                //    //if (ExtendX > 100 && ALIGNPara.AlignMethod == AlignMethodEnum.AUFIND)
                //    {
                //        if (AliasName.IndexOf("A03-02-0010") > -1) // && ALIGNPara.AlignMethod == AlignMethodEnum.AUFIND)// == "A00-04-0024")
                //        {
                //            //bmpPATTERN.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_PATTERN" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                //            //bmpWIP.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_WIP" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                //            //bmpMASK.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_MASK" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                //            //bmpOUTPUT.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_Output" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                //        //ALIGNPara.IsTempSave = true;
                //        }
                //    }
                if (AliasName == "")
                {
                    return false;
                }
                //if (AliasName == "A03-02-0010")
                //{

                //}
                ALIGNPara.Score = -1;
             

                switch(ALIGNPara.AlignMethod)
                {
#if USEHIKROT
                    case AlignMethodEnum.HIK_FIND:
                        isgood &= ALIGNPara.HikFindRun(bmpWIP, ref bmpOUTPUT, istrain, Brightness, Contrast);
                        break;
#endif
                    default:
                        isgood &= ALIGNPara.AuFindRun(bmpWIP, ref bmpOUTPUT, istrain, Brightness, Contrast);
                        break;
                }

                ALIGNPara.IsTempSave = false;

                //if (!istrain)
                //{
                //    bmpWIP.Save(Universal.TESTPATH
                //        + "\\ANALYZETEST\\ALAIGN WIP "
                //        + NoSaveStr
                //        + Universal.GlobalImageTypeString,
                //        Universal.GlobalImageFormat);
                //}



                //if (PageNo == 2)
                //    bmpOUTPUT.Save(Universal.TESTPATH + "\\ANALYZETEST\\ALAIGN OUTPUT " + NoSaveStr + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                //if (PageNo == 3)
                //{
                //    if ( ALIGNPara.AlignMethod == AlignMethodEnum.AUFIND)
                //    {
                //        StreamWriter Sw = File.AppendText("D:\\FindSocr.txt");
                //        Sw.WriteLine(AliasName+", "+ ALIGNPara.Score);
                //        Sw.Close();

                //    }
                //}
                isaligngood = isgood;


                if (istrain)
                    ALIGNPara.FillTrainStatus(TrainStatusCollection, ToLogString());
                else
                {
                    if (isoutputfilltrain)
                        ALIGNPara.FillRunStatus(TrainStatusCollection, ToLogString());
                    else
                        ALIGNPara.FillRunStatus(RunStatusCollection, ToLogString());
                }

                if (!isgood && !istrain || IsTempSave)//|| AliasName == "LASERTEST")
                {
                    //bmpPATTERN.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_PATTERN" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                    //bmpWIP.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_WIP" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                    //bmpMASK.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_MASK" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                    //bmpOUTPUT.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_OUTPUT" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                    //SaveData(ALIGNPara.ToAlignParaString(), Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + " Info.txt");
                }

                //bmpALIGNED.Dispose();
                //bmpALIGNED = new Bitmap(bmpOUTPUT);
                bmpALIGNED = (Bitmap)bmpOUTPUT.Clone();

            }
            // 3. 檢測是否正確
            if (isgood && !INI.ISONLYCHECKSN && INSPECTIONPara.InspectionMethod != InspectionMethodEnum.NONE)
            {
                //bmpWIP.Dispose();
                bmpWIP = new Bitmap(bmpALIGNED);
                //bmpWIP.Save(Universal.TESTPATH + "\\bmpwiphh.bmp");
                //if (istrain)
                //{
                //    bmpPATTERN.Dispose();
                //    bmpPATTERN = new Bitmap(bmpALIGNED);
                //}

                isgood &= I01_InspectionProcess(istrain);

                if (istrain)
                    INSPECTIONPara.FillTrainStatus(TrainStatusCollection, ToLogString());
                else
                {
                    if (isoutputfilltrain)
                        INSPECTIONPara.FillRunStatus(TrainStatusCollection, ToLogString());
                    else
                        INSPECTIONPara.FillRunStatus(RunStatusCollection, ToLogString());
                }

                if (!isgood && !istrain && IsTempSave)
                {
                    //bmpPATTERN.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_PATTERN" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                    //bmpWIP.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_WIP" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                    //bmpMASK.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_MASK" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                    //bmpOUTPUT.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_OUTPUT" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                    //SaveData(ALIGNPara.ToAlignParaString(), Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + " Info.txt");
                }
            }
            // 4. 做量測
            if (isgood && !INI.ISONLYCHECKSN && MEASUREPara.MeasureMethod != MeasureMethodEnum.NONE)
            {
                //bmpWIP.Dispose();
                bmpWIP = new Bitmap(bmpALIGNED);

                //if(istrain)
                //{
                //    bmpPATTERN.Dispose();
                //    bmpPATTERN = new Bitmap(bmpALIGNED);
                //}
                if (AliasName.IndexOf("A00-02-0007") > -1)
                {
                    //MEASUREPara.IsTempSave = true;
                }
                //     bmpWIP.Save("d:\\testtest\\bmpwinp.png");
                //     bmpPATTERN.Save("d:\\testtest\\bmpPATTERN.png");
                //      bmpMASK.Save("d:\\testtest\\bmpMASK.png");
                //     bmpOUTPUT.Save("d:\\testtest\\bmpOUTPUT.png");

                isgood &= MEASUREPara.MeasureProcess(bmpWIP, bmpPATTERN, bmpMASK, ref bmpOUTPUT, Brightness, Contrast, ToAnalyzeString(), PassInfo, istrain);
                MEASUREPara.IsTempSave = false;
                if (istrain)
                    MEASUREPara.FillTrainStatus(TrainStatusCollection, ToLogString());
                else
                    MEASUREPara.FillRunStatus(RunStatusCollection, ToLogString());

                if (!isgood && !istrain || IsTempSave)
                //if (!istrain)
                {
                    //bmpPATTERN.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_PATTERN" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                    //bmpWIP.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_WIP" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                    //bmpMASK.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_MASK" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                    //bmpOUTPUT.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_OUTPUT" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                    //SaveData(ALIGNPara.ToAlignParaString(), Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + " Info.txt");
                }
            }
            // 5. 做OCR及缺失检测 
            if (isgood)
            {
                if (OCRPara.OCRMethod == OCRMethodEnum.MAPPING)
                {
                    //bmpPATTERN.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_PATTERN" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                    //bmpOUTPUT.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_WIP" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                    //bmpMASK.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_MASK" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                    //if(bmpORGLEARNININPUT!=null)
                    //bmpORGLEARNININPUT.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-bmpORGLEARNININPUT" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);


                    bmpWIP.Dispose();
                    bmpWIP = new Bitmap(bmpALIGNED);

                    if (istrain)
                    {
                        bmpPATTERN.Dispose();
                        bmpPATTERN = new Bitmap(bmpALIGNED);
                    }

                    bool isgoodtemp = true;
                    string strSN = OCRPara.FindOCR(JzToolsClass.PassingBarcode, istrain, bmpPATTERN, bmpWIP, PassInfo, out isgoodtemp);
                    Universal.OCRSN = strSN;
                    Universal.ISCHECKSN = true;
                    //JzToolsClass.PassingBarcode = "";
                    if (!isgoodtemp && !istrain)
                        isgood = false;

                    if (Universal.isR3ByPass)
                        isgood = true;

                    if (istrain)
                        OCRPara.FillTrainStatus(TrainStatusCollection);
                    else
                        OCRPara.FillRunStatus(RunStatusCollection);
                }
            }
            // 6 读取条码及检测 
            if (isgood)
            {
                if (OCRPara.OCRMethod == OCRMethodEnum.CODE128)
                {
                    bmpWIP.Dispose();
                    bmpWIP = new Bitmap(bmpALIGNED);

                    //if (istrain)
                    //{
                    //    bmpPATTERN.Dispose();
                    //    bmpPATTERN = new Bitmap(bmpALIGNED);
                    //}

                    bool isgoodtemp = true;
                    string strSN = OCRPara.DeCode(JzToolsClass.PassingBarcode, istrain, bmpPATTERN, bmpWIP, PassInfo, out isgoodtemp);


                    if (isgoodtemp)
                    {
                        if (INSPECTIONPara.InspectionMethod == InspectionMethodEnum.BAR_CHECK)
                        {
                            bmpWIP.Dispose();
                            bmpWIP = new Bitmap(bmpALIGNED);

                            isgoodtemp = OCRPara.CheckBarCode(istrain, bmpWIP, PassInfo, INSPECTIONPara.IBTolerance, INSPECTIONPara.IBCount, INSPECTIONPara.IBArea);
                        }
                    }

                    if (Universal.OPTION == OptionEnum.R3)
                        Universal.R3UI.Barcode1D = strSN;
                    if (Universal.OPTION == OptionEnum.C3)
                        Universal.C3UI.Barcode1D = strSN;
                    //JzToolsClass.PassingBarcode = "";
                    if (!isgoodtemp)
                        isgood = false;

                    if (istrain)
                        OCRPara.FillTrainStatus(TrainStatusCollection);
                    else
                        OCRPara.FillRunStatus(RunStatusCollection);
                }
                else if (OCRPara.OCRMethod == OCRMethodEnum.DATAMATRIX)
                {
                    bmpWIP.Dispose();
                    bmpWIP = new Bitmap(bmpALIGNED);

                    //if (istrain)
                    //{
                    //    bmpPATTERN.Dispose();
                    //    bmpPATTERN = new Bitmap(bmpALIGNED);
                    //}

                    //待添加测试代码

                    ReadBarcode2DStr = OCRPara.DeCode2d(AliasName, INI.IsCheckBarcodeOpen, m_barcode_2D, istrain, bmpPATTERN, bmpWIP, PassInfo, out isgood, out ReadBarcode2DRealStr);

                    //bool isgoodtemp = true;
                    //string strSN = OCRPara.DeCode(JzToolsClass.PassingBarcode, istrain, bmpPATTERN, bmpWIP, PassInfo, out isgoodtemp);

                    if (istrain)
                        OCRPara.FillTrainStatus(TrainStatusCollection);
                    else
                        OCRPara.FillRunStatus(RunStatusCollection);
                }
                else if (OCRPara.OCRMethod == OCRMethodEnum.DATAMATRIXGRADE)
                {
                    bmpWIP.Dispose();
                    bmpWIP = new Bitmap(bmpALIGNED);

                    //if (istrain)
                    //{
                    //    bmpPATTERN.Dispose();
                    //    bmpPATTERN = new Bitmap(bmpALIGNED);
                    //}

                    //待添加测试代码

                    ReadBarcode2DStr = OCRPara.DeCode2dGrade(AliasName, INI.IsCheckBarcodeOpen, m_barcode_2D, istrain, bmpPATTERN, bmpWIP, PassInfo, out isgood, out ReadBarcode2DRealStr);
                    ReadBarcode2DGrade = OCRPara.BarcodeGrade;

                    //bool isgoodtemp = true;
                    //string strSN = OCRPara.DeCode(JzToolsClass.PassingBarcode, istrain, bmpPATTERN, bmpWIP, PassInfo, out isgoodtemp);

                    if (istrain)
                        OCRPara.FillTrainStatus(TrainStatusCollection);
                    else
                        OCRPara.FillRunStatus(RunStatusCollection);
                }
            }
            // 7 检查螺丝高翘
            if (isgood & !istrain & StiltsPara.StiltsMethod == STILTSMethodEnum.STILTS)
            {
                bmpWIP.Dispose();
                bmpWIP = new Bitmap(bmpALIGNED);

                Bitmap MaskTemp = new Bitmap(bmpPATTERN.Width, bmpPATTERN.Height, PixelFormat.Format32bppArgb);
                DrawRect(MaskTemp, BlackMaskBrush);
                A0202_CreateAlignMask(myOringinOffsetPointF, MaskTemp);

                //    MaskTemp.Save("D:\\maskTemp.png");

                Bitmap bmpRunLine = new Bitmap(bmpALIGNED);
                JetEazy.BasicSpace.myImageProcessor.SetMaskToStilts(bmpRunLine, MaskTemp, Color.White);

                myImageProcessor.SetMaskToStilts(bmpRunLine, MaskTemp, StiltsPara.StiltsGrayValue, StiltsPara.StiltsNOGrayValue, Color.Black);

                // JetEazy.BasicSpace.myImageProcessor.Balance(bmpRunLine, ref MaskTemp, myImageProcessor.EnumThreshold.Minimum);


                StiltsPara.FindBlob(istrain, bmpPATTERN, bmpWIP, bmpRunLine, PassInfo, out isgood);

                StiltsPara.FillRunStatus(RunStatusCollection);

            }


            switch (OPTION)
            {
                case OptionEnum.MAIN_SDM3:
                case OptionEnum.MAIN_SDM2:

                    switch (PADPara.PADMethod)
                    {
                        case PADMethodEnum.GLUECHECK:
                        case PADMethodEnum.GLUECHECK_BlackEdge:
                            PADPara.bmpMeasureOutput.Dispose();
                            //PADPara.bmpMeasureOutput = new Bitmap(bmpWIP);
                            PADPara.bmpMeasureOutput = (Bitmap)bmpWIP.Clone();
                            break;
                    }

                    break;
            }

            PADPara.RunDataOK = false;//复位数据
            PADPara.DescStr = string.Empty;
            //8检查PAD溢胶
            if (isgood)
            {
                if (PADPara.PADMethod == PADMethodEnum.PADCHECK)
                {
                    bmpWIP.Dispose();
                    bmpWIP = new Bitmap(bmpALIGNED);

                    //if (istrain)
                    //{
                    //    bmpPATTERN.Dispose();
                    //    bmpPATTERN = new Bitmap(bmpALIGNED);
                    //}

                    isgood &= P10_PADInspectionProcess(istrain);

                    if (istrain)
                        PADPara.FillTrainStatus(TrainStatusCollection, ToLogString());
                    else
                    {
                        if (isoutputfilltrain)
                            PADPara.FillRunStatus(TrainStatusCollection, ToLogString());
                        else
                            PADPara.FillRunStatus(RunStatusCollection, ToLogString());
                    }

                    if (!isgood && !istrain && IsTempSave)
                    {
                        //bmpPATTERN.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_PATTERN" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                        //bmpWIP.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_WIP" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                        //bmpMASK.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_MASK" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                        //bmpOUTPUT.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_OUTPUT" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                        //SaveData(ALIGNPara.ToAlignParaString(), Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + " Info.txt");
                    }
                }
                else if (PADPara.PADMethod == PADMethodEnum.GLUECHECK)
                {
                    ResetChipData();

                    bmpWIP.Dispose();
                    //bmpWIP = new Bitmap(bmpALIGNED);
                    bmpWIP = (Bitmap)bmpALIGNED.Clone();

                    JzToolsClass jzTools = new JzToolsClass();
                    PADPara.PtfCenter = jzTools.GetRectCenterF(GetMyMoverRectF());

                    //if (istrain)
                    //{
                    //    bmpPATTERN.Dispose();
                    //    bmpPATTERN = new Bitmap(bmpALIGNED);
                    //}

                    isgood &= P10_PADInspectionProcess(istrain);

                    if (istrain)
                        PADPara.FillTrainStatus(TrainStatusCollection, ToLogString());
                    else
                    {
                        if (isoutputfilltrain)
                            PADPara.FillRunStatus(TrainStatusCollection, ToLogString());
                        else
                            PADPara.FillRunStatus(RunStatusCollection, ToLogString());
                    }
                    //if (!istrain)
                    //{
                    //    bmpWIP.Save(Universal.TESTPATH
                    //    + "\\ANALYZETEST\\"
                    //    + NoSaveStr + "-B_WIP"
                    //    + Universal.GlobalImageTypeString,
                    //    Universal.GlobalImageFormat);
                    //}

                    if (!isgood && !istrain && IsTempSave)
                    {
                        //bmpPATTERN.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_PATTERN" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                        //bmpWIP.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_WIP" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                        //bmpMASK.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_MASK" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                        //bmpOUTPUT.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_OUTPUT" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                        //SaveData(ALIGNPara.ToAlignParaString(), Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + " Info.txt");
                    }
                }
                else if (PADPara.PADMethod == PADMethodEnum.GLUECHECK_BlackEdge)
                {
                    ResetChipData();

                    bmpWIP.Dispose();
                    //bmpWIP = new Bitmap(bmpALIGNED);
                    bmpWIP = (Bitmap)bmpALIGNED.Clone();

                    JzToolsClass jzTools = new JzToolsClass();
                    PADPara.PtfCenter = jzTools.GetRectCenterF(GetMyMoverRectF());

                    //if (istrain)
                    //{
                    //    bmpPATTERN.Dispose();
                    //    bmpPATTERN = new Bitmap(bmpALIGNED);
                    //}

                    isgood &= P10_PADInspectionProcess(istrain);

                    if (istrain)
                        PADPara.FillTrainStatus(TrainStatusCollection, ToLogString());
                    else
                    {
                        if (isoutputfilltrain)
                            PADPara.FillRunStatus(TrainStatusCollection, ToLogString());
                        else
                            PADPara.FillRunStatus(RunStatusCollection, ToLogString());
                    }
                    //if (!istrain)
                    //{
                    //    bmpWIP.Save(Universal.TESTPATH
                    //    + "\\ANALYZETEST\\"
                    //    + NoSaveStr + "-B_WIP"
                    //    + Universal.GlobalImageTypeString,
                    //    Universal.GlobalImageFormat);
                    //}

                    if (!isgood && !istrain && IsTempSave)
                    {
                        //bmpPATTERN.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_PATTERN" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                        //bmpWIP.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_WIP" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                        //bmpMASK.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_MASK" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                        //bmpOUTPUT.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_OUTPUT" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                        //SaveData(ALIGNPara.ToAlignParaString(), Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + " Info.txt");
                    }
                }
                else if(PADPara.PADMethod == PADMethodEnum.PLACODE_CHECK)
                {
                    bmpWIP.Dispose();
                    bmpWIP = new Bitmap(bmpALIGNED);

                    //Graphics graphicsx = Graphics.FromImage(bmpWIP);

                    List<RectangleF> list = new List<RectangleF>();
                    //收集内框并画出
                    foreach (AnalyzeClass analyze2 in BranchList)
                    {
                        RectangleF rx = new RectangleF();
                        //rx.X = analyze2.myOPRectF.X - ALIGNPara.AlignOffset.X + analyze2.NORMALPara.ExtendX;// - analyze.myOPRectF.X;
                        //rx.Y = analyze2.myOPRectF.Y - ALIGNPara.AlignOffset.Y + analyze2.NORMALPara.ExtendY;// - analyze.myOPRectF.Y;
                        rx.X = analyze2.myOPRectF.X + analyze2.NORMALPara.ExtendX;// - analyze.myOPRectF.X;
                        rx.Y = analyze2.myOPRectF.Y + analyze2.NORMALPara.ExtendY;// - analyze.myOPRectF.Y;
                        rx.Width = analyze2.myOPRectF.Width - analyze2.NORMALPara.ExtendX * 2;
                        rx.Height = analyze2.myOPRectF.Height - analyze2.NORMALPara.ExtendY * 2;
                        list.Add(rx);
                    }
                    PADPara.ListRectFMask = list;
                    //if (list.Count > 0)
                    //    graphicsx.FillRectangles(Brushes.Black, list.ToArray());
                    //graphicsx.Dispose();

                    isgood &= P10_PADInspectionProcess(istrain);

                    if (istrain)
                        PADPara.FillTrainStatus(TrainStatusCollection, ToLogString());
                    else
                    {
                        if (isoutputfilltrain)
                            PADPara.FillRunStatus(TrainStatusCollection, ToLogString());
                        else
                            PADPara.FillRunStatus(RunStatusCollection, ToLogString());
                    }

                    if (!isgood && !istrain && IsTempSave)
                    {
                        //bmpPATTERN.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_PATTERN" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                        //bmpWIP.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_WIP" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                        //bmpMASK.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_MASK" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                        //bmpOUTPUT.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_OUTPUT" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                        //SaveData(ALIGNPara.ToAlignParaString(), Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + " Info.txt");
                    }
                }
                else if (PADPara.PADMethod == PADMethodEnum.QLE_CHECK)
                {
                    bmpWIP.Dispose();
                    bmpWIP = new Bitmap(bmpALIGNED);

                    isgood &= P10_PADInspectionProcess(istrain);

                    if (istrain)
                        PADPara.FillTrainStatus(TrainStatusCollection, ToLogString());
                    else
                    {
                        if (isoutputfilltrain)
                            PADPara.FillRunStatus(TrainStatusCollection, ToLogString());
                        else
                            PADPara.FillRunStatus(RunStatusCollection, ToLogString());
                    }

                    if (!isgood && !istrain && IsTempSave)
                    {
                        //bmpPATTERN.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_PATTERN" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                        //bmpWIP.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_WIP" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                        //bmpMASK.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_MASK" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                        //bmpOUTPUT.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-B_OUTPUT" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                        //SaveData(ALIGNPara.ToAlignParaString(), Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + " Info.txt");
                    }
                }
            }
            //else
            //{
            //    switch(OPTION)
            //    {
            //        case OptionEnum.MAIN_SDM2:

            //            if (PADPara.PADMethod == PADMethodEnum.GLUECHECK)
            //            {
            //                PADPara.bmpMeasureOutput.Dispose();
            //                PADPara.bmpMeasureOutput = new Bitmap(bmpOUTPUT);
            //            }

            //            break;
            //    }
            //}

            //bmpOUTPUT.Dispose();
            //bmpOUTPUT = new Bitmap(bmpALIGNED);

            bmpOUTPUT = (Bitmap)bmpALIGNED.Clone();

            //全部正確才去弄相對的 MASK 給下一步驟用

            if (isgood)
                CreateRelateMask(bmpOUTPUT, myOringinOffsetPointF, BranchList);

            bmpALIGNED.Dispose();

            return isgood;
        }
        bool A07_CreateAlignRunRequirement(Bitmap bmpinput)
        {
            return A07_CreateAlignRunRequirement(bmpinput, false);
        }
        /// <summary>
        /// 取得 RUN 定位前的所有資料
        /// </summary>
        /// <param name="bmpinput"></param>
        /// <param name="offsetpointf"></param>
        bool A07_CreateAlignRunRequirement(Bitmap bmpinput, bool islearn)
        {
            bool isroot = Level == 1;
            bool isgood = true;
            RectangleF rectfExtend = new RectangleF();

            string analyzestring = ToLogAnalyzeString();

            WorkStatusClass workstatus = new WorkStatusClass(AnanlyzeProcedureEnum.PREPARE);
            string processstring = "Start " + analyzestring + " Run Prepare." + Environment.NewLine;
            string errorstring = "";
            ReasonEnum reason = ReasonEnum.PASS;

            //string str = "";

            //if (isroot || IsLearnRoot)
            if (isroot)
            {
                processstring += analyzestring + " is Root." + Environment.NewLine;

                bmpWIP.Dispose();
                bmpWIP = new Bitmap(bmpinput);
                //bmpWIP = (Bitmap)bmpinput.Clone();
            }
            else
            {
                if (islearn)
                {
                    processstring += analyzestring + " is Learn." + Environment.NewLine;

                    rectfExtend = LearnOrigionOffsetRectf;
                    if (myAlignOffsetChange)
                    {
                        rectfExtend.X -= myAlignOffsetCal.X;
                        rectfExtend.Y -= myAlignOffsetCal.Y;

                        BoundRect(ref rectfExtend, bmpinput.Size);
                    }
                    //視情況加入
                    //switch (ALIGNPara.AlignMode)
                    //{
                    //    case AlignModeEnum.BORDER:

                    //        rectfExtend.Inflate(50, 50);
                    //        rectfExtend.Intersect(new RectangleF(0, 0, bmpinput.Width, bmpinput.Height));
                    //        break;
                    //}
                    if (rectfExtend.Width == 0)
                        rectfExtend.Width = 1;
                    if (rectfExtend.Height == 0)
                        rectfExtend.Height = 1;
                    lock (bmpinput)
                    {
                        if (rectfExtend.X + rectfExtend.Width > bmpinput.Width || rectfExtend.Y + rectfExtend.Height > bmpinput.Height)
                        {
                            bmpWIP.Dispose();
                            bmpWIP = new Bitmap(bmpinput);
                        }
                        else
                        {
                            bmpWIP.Dispose();
                            bmpWIP = (Bitmap)bmpinput.Clone(rectfExtend, PixelFormat.Format32bppArgb);
                        }
                    }

                    //bmpWIP.Save(cLEARNWIP" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                }
                else
                {
                    processstring += analyzestring + " is Not Root." + Environment.NewLine;

                    //Reduce By Victor 2018/02/12
                    //bmpORGLEARNININPUT.Dispose();
                    bmpORGLEARNININPUT = bmpinput;

                    rectfExtend = myOPRectF;
                    if (myAlignOffsetChange)
                    {
                        rectfExtend.X -= myAlignOffsetCal.X;
                        rectfExtend.Y -= myAlignOffsetCal.Y;

                        BoundRect(ref rectfExtend, bmpinput.Size);
                    }

                    //視情況加入
                    //switch (ALIGNPara.AlignMode)
                    //{
                    //    case AlignModeEnum.BORDER:

                    //        rectfExtend.Inflate(50, 50);
                    //        rectfExtend.Intersect(new RectangleF(0, 0, bmpinput.Width, bmpinput.Height));
                    //        break;
                    //}

                    bmpWIP.Dispose();

                    if (rectfExtend.Width == 0)
                        rectfExtend.Width = 1;
                    if (rectfExtend.Height == 0)
                        rectfExtend.Height = 1;

                    if (rectfExtend.X + rectfExtend.Width > bmpinput.Width || rectfExtend.Y + rectfExtend.Height > bmpinput.Height)
                    {
                        //bmpWIP = new Bitmap(bmpinput);
                        bmpWIP = (Bitmap)bmpinput.Clone();
                    }
                    else
                        bmpWIP = (Bitmap)bmpinput.Clone(rectfExtend, PixelFormat.Format32bppArgb);



                    //if (ToAnalyzeString() == "ANZ-02-0017")
                    //bmpWIP.Save(Universal.TESTPATH + "\\ANALYZETEST\\RUNWIP" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                    //if (IsTempSave)
                    //{
                    //    bmpinput.Save(Universal.TESTPATH + "\\ANALYZETEST\\RUNINPUT" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                    //    bmpWIP.Save(Universal.TESTPATH + "\\ANALYZETEST\\RUNWIP" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                    //}


                }

                bmpINPUT.Dispose();
                //bmpINPUT = new Bitmap(bmpWIP);
                bmpINPUT = (Bitmap)bmpWIP.Clone();
            }

            //if (!isroot)
            //{
            //    bmpINPUT.Dispose();
            //    bmpINPUT = new Bitmap(bmpWIP);
            //}


            //產生 Direct Mask
            //if (isroot || IsLearnRoot)
            if (isroot || islearn)
            {
                processstring += analyzestring + " Generate Direct Mask." + Environment.NewLine;
                A0201_GenDirectMask(bmpWIP, new PointF(0, 0));
            }

            workstatus.SetWorkStatus(bmpPATTERN, bmpWIP, bmpMASK, reason, errorstring, processstring, PassInfo);
            RunStatusCollection.Add(workstatus);

            //bmpPATTERN.Save(Universal.TESTPATH + "\\ANALYZETEST\\bmpPATTERN" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

            ////產生 Corner 及 WH 的資料
            //if (!isroot)
            //{
            //    if (AOIPara.IsHaveCornerOrWH)
            //    {
            //        //str = "Create Corner and WH Mask.";

            //        isgood = AOIPara.A08_RunAOIData(bmpWIP);

            //        //AOIPara.FillProcessDataLog(myProcessStringList, RunStatusList);
            //    }
            //}
            //str = "Start Run Alignment";
            //LogMessage(str);

            return isgood;

        }

        public AnalyzeClass AnalyzeSeed = null;
        public bool A101_01()
        {
            Point pointFOffset = new Point(0, 0);
            foreach (AnalyzeClass branchanalyze in BranchList)
            {
                if (branchanalyze.Level == 2)
                {
                    pointFOffset = new Point(-(int)branchanalyze.ALIGNPara.AlignOffset.X, -(int)branchanalyze.ALIGNPara.AlignOffset.Y);
                }
                branchanalyze.SetMoverOffset(pointFOffset);
                foreach (AnalyzeClass branchanalyze2 in branchanalyze.BranchList)
                {
                    branchanalyze2.SetMoverOffset(pointFOffset);
                }
            }
            return true;
        }
        public void A00_SetOffset(Point ePoint, bool ison)
        {
            this.SetMoverOffset(ePoint, ison);
            foreach (AnalyzeClass branchanalyze in BranchList)
            {
                branchanalyze.A00_SetOffset(ePoint, ison);
            }
        }

        bool A08_RunMeAndSubTreeWithA07()
        {
            bool isgood = true;
            bool isaligngood = false;   //若原有的Analyze 在定位時正確，才可以檢測髒污

            //加入一个判断 跳过检测
           switch(VERSION)
            {
                case VersionEnum.ALLINONE:

                    switch(OPTION)
                    {
                        case OptionEnum.MAIN_X6:
                        case JetEazy.OptionEnum.MAIN_SERVICE:

                            if (IsByPass)
                            {
                                IsVeryGood = isgood;    //本 Analyze 是否為 PASS
                                IsOperated = true;      //本 Analyze 有被運算過

                                return true;
                            }

                            break;
                    }

                    break;
            }

            if (isgood)
            {
                isgood = A06_RunProcess(false, ref isaligngood);

                //if (!isaligngood)
                //    isaligngood = isaligngood;

                IsVeryGood = isgood;    //本 Analyze 是否為 PASS
                IsOperated = true;      //本 Analyze 有被運算過
            }

            if (isgood)
            {
                int icount = 0;
                //先把所有要處理的東西準備好
                foreach (AnalyzeClass branchanalyze in BranchList)
                {
                    if (branchanalyze.MaskMethod == MaskMethodEnum.NONE)
                    {
                        icount++;
                        isgood &= branchanalyze.A07_CreateAlignRunRequirement(bmpOUTPUT);

                        //bmpOUTPUT.Save(Universal.TESTPATH + "\\ANALYZETEST\\FUCKOUTPUT" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                        switch (OPTION)
                        {
                            case OptionEnum.MAIN_X6:
                            case OptionEnum.MAIN_SDM2:
                            case OptionEnum.MAIN_SDM3:
                                break;
                            default:
                                branchanalyze.FillRunStatus(RunStatusCollection, ToLogString());
                                break;
                        }
                    }

                    if (!isgood)
                        break;
                }


                if (Universal.IsUseSeedFuntion && false)
                {
                    foreach (AnalyzeClass branchanalyze in BranchList)
                    {
                        if (AnalyzeSeed == null)
                            break;

                        if (!branchanalyze.IsSeed)
                        {
                            branchanalyze.KillInsideList(true);
                            branchanalyze.LearnList.Clear();
                            if (AnalyzeSeed.LearnList.Count > 0)
                            {

                            }
                            else
                            {
                                int itmpindex = 0;
                                foreach (AnalyzeClass analyze in AnalyzeSeed.BranchList)
                                {
                                    AnalyzeClass analyze1tmp = analyze.DeepClone();
                                    analyze1tmp.PageNo = branchanalyze.PageNo;
                                    analyze1tmp.Level = branchanalyze.Level;
                                    analyze1tmp.No = branchanalyze.No;

                                    if (analyze1tmp.ALIGNPara.bmpPattern == null)
                                        analyze1tmp.ALIGNPara.bmpPattern = new Bitmap(analyze.ALIGNPara.bmpPattern);
                                    else
                                    {
                                        analyze1tmp.ALIGNPara.bmpPattern.Dispose();
                                        analyze1tmp.ALIGNPara.bmpPattern = new Bitmap(analyze.ALIGNPara.bmpPattern);
                                    }
                                    if (analyze1tmp.ALIGNPara.bmpMask == null)
                                        analyze1tmp.ALIGNPara.bmpMask = new Bitmap(analyze.ALIGNPara.bmpMask);
                                    else
                                    {
                                        analyze1tmp.ALIGNPara.bmpMask.Dispose();
                                        analyze1tmp.ALIGNPara.bmpMask = new Bitmap(analyze.ALIGNPara.bmpMask);
                                    }

                                    //if (analyze1tmp.INSPECTIONPara.bmpPattern == null)
                                    //    analyze1tmp.INSPECTIONPara.bmpPattern = new Bitmap(analyze.INSPECTIONPara.bmpPattern);
                                    //else
                                    //{
                                    //    analyze1tmp.INSPECTIONPara.bmpPattern.Dispose();
                                    //    analyze1tmp.INSPECTIONPara.bmpPattern = new Bitmap(analyze.INSPECTIONPara.bmpPattern);
                                    //}

                                    //if (analyze1tmp.INSPECTIONPara.bmpMask == null)
                                    //    analyze1tmp.INSPECTIONPara.bmpMask = new Bitmap(analyze.INSPECTIONPara.bmpMask);
                                    //else
                                    //{
                                    //    analyze1tmp.INSPECTIONPara.bmpMask.Dispose();
                                    //    analyze1tmp.INSPECTIONPara.bmpMask = new Bitmap(analyze.INSPECTIONPara.bmpMask);
                                    //}

                                    bool bOK = analyze1tmp.ALIGNPara.IsSeedTrain(this.Brightness, this.Contrast);
                                    analyze1tmp.INSPECTIONPara.IsSeed_GetInspectionRequirement(analyze.INSPECTIONPara.bmpPattern, analyze.INSPECTIONPara.bmpMask);
                                    branchanalyze.BranchList.Add(analyze1tmp);

                                    itmpindex++;
                                }

                                //branchanalyze.BranchList = AnalyzeSeed.BranchList;

                                branchanalyze.mySeedRectF = new RectangleF(AnalyzeSeed.myOPRectF.X, AnalyzeSeed.myOPRectF.Y, AnalyzeSeed.myOPRectF.Width, AnalyzeSeed.myOPRectF.Height);
                                branchanalyze.mySeedOffset = new PointF(branchanalyze.myOPRectF.X - AnalyzeSeed.myOPRectF.X, branchanalyze.myOPRectF.Y - AnalyzeSeed.myOPRectF.Y);

                                branchanalyze.mySeedRectF.X += branchanalyze.mySeedOffset.X;
                                branchanalyze.mySeedRectF.Y += branchanalyze.mySeedOffset.Y;


                            }

                        }
                    }
                }


                if (isgood)
                {
                    //Could Do MultiProcess For This Affair
#if MULTI
                    //Single Process
                    foreach (AnalyzeClass branchanalyze in BranchList)
                    {
                        if (branchanalyze.MaskMethod == MaskMethodEnum.NONE)
                        {
                            //isgood &=branchanalyze.A08_RunMeAndSubTreeWithA07();
                            branchanalyze.A08_RunMeAndSubTreeWithA07();
                        }
                        //if (!isgood)
                        //    break;
                    }
#endif
#if !MULTI
                    //MultiProcess
                    if (Universal.IsMultiThread && Universal.IsMultiThreadUseToRun)
                    {
                        Parallel.ForEach(BranchList, branch =>
                         {
                             // System.Threading.Thread.Sleep(100);
                             if (branch.MaskMethod == MaskMethodEnum.NONE)
                                 //isgood &= branch.A08_RunMeAndSubTreeWithA07();
                                 branch.A08_RunMeAndSubTreeWithA07();
                         });
                    }
                    else
                    {

                        foreach (AnalyzeClass branchanalyze in BranchList)
                        {
                            if (branchanalyze.MaskMethod == MaskMethodEnum.NONE)
                                //isgood &= branchanalyze.A08_RunMeAndSubTreeWithA07();
                                branchanalyze.A08_RunMeAndSubTreeWithA07();
                        }
                    }

#endif

                    foreach (AnalyzeClass branchanalyze in BranchList)
                    {
                        //判定最後是否為PASS
                        //isgood &= branchanalyze.IsVeryGood;

                        if (branchanalyze.MaskMethod == MaskMethodEnum.NONE)
                        {
                            if (branchanalyze.RunStatusCollection.COUNT > 0)
                            {
                                switch (OPTION)
                                {
                                    case OptionEnum.MAIN_X6:
                                    case OptionEnum.MAIN_SDM2:
                                    case OptionEnum.MAIN_SDM3:
                                        break;
                                    default:
                                        branchanalyze.FillRunStatus(RunStatusCollection, ToLogString());
                                        break;
                                }
                                //branchanalyze.FillRunStatus(RunStatusCollection, ToLogString());
                            }

                            //if (Universal.IsUseSeedFuntion)
                            //{
                            //    foreach (AnalyzeClass branchanalyze1 in branchanalyze.BranchList)
                            //    {
                            //        if (branchanalyze1.MaskMethod == MaskMethodEnum.NONE)
                            //        {
                            //            if (branchanalyze1.RunStatusCollection.NGCOUNT > 0)
                            //            {
                            //                branchanalyze.FillRunStatus(branchanalyze1.RunStatusCollection, ToLogString());
                            //                //isgood = false;
                            //                //IsVeryGood = isgood;
                            //                branchanalyze.IsVeryGood = false;
                            //            }
                            //        }
                            //    }
                            //}


                        }
                    }
                }

                //if (Universal.IsUseSeedFuntion)
                //{
                //    foreach (AnalyzeClass branchanalyze in BranchList)
                //    {
                //        if (AnalyzeSeed == null)
                //            break;

                //        branchanalyze.BackupInsideList(AnalyzeSeed);
                //        if (!branchanalyze.IsSeed)
                //            branchanalyze.LearnList.Clear();

                //    }
                //}

                //若本身的Align正確，最後需要檢測剩下的地方有沒有東西
                if (isaligngood)
                {
                    if (AOIPara.CheckDirtMethod != CheckDirtMethodEnum.NONE && ALIGNPara.AlignMode == AlignModeEnum.BORDER)
                    {
                        DigForDirtCheck();

                        isgood &= AOIPara.CheckDirt(bmpOUTPUT, bmpHOLLOW, false, PassInfo);
                        AOIPara.FillRunStatus(RunStatusCollection, ToLogString());

                        IsVeryGood = isgood; //本 Analyze 是否連髒污檢測都為 PASS
                    }
                }
            }

            if (!isgood)
            {
                int uselearnindex = 1;

                foreach (AnalyzeClass learnanalyze in LearnList)
                {
                    //bmpORGLEARNININPUT.Save(Universal.TESTPATH + "\\ANALYZETEST\\ORGLEARN" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                    isgood = learnanalyze.A01_Run(bmpORGLEARNININPUT, true);
                    learnanalyze.FillRunStatus(RunStatusCollection);

                    if (isgood)
                    {
                        LastLearnIndex += uselearnindex;
                        break;
                    }

                    uselearnindex++;
                }

                IsVeryGood = isgood;
            }

            //if (Universal.IsUseSeedFuntion)
            //{
            //    foreach (AnalyzeClass branchanalyze in BranchList)
            //    {
            //        if (AnalyzeSeed == null)
            //            break;

            //        //if (Universal.IsUseSeedFuntion)
            //        //{
            //        //    foreach (AnalyzeClass branchanalyze1 in branchanalyze.BranchList)
            //        //    {
            //        //        if (branchanalyze1.MaskMethod == MaskMethodEnum.NONE)
            //        //        {
            //        //            if (branchanalyze1.RunStatusCollection.NGCOUNT > 0)
            //        //            {
            //        //                branchanalyze.FillRunStatus(branchanalyze1.RunStatusCollection, ToLogString());
            //        //                //isgood = false;
            //        //                //IsVeryGood = isgood;
            //        //                branchanalyze.IsVeryGood = false;
            //        //            }
            //        //        }
            //        //    }
            //        //}

            //        branchanalyze.RestoreInsideList(branchanalyze.IsSeed);
            //    }
            //}

            //  bmpOUTPUT.Save(Universal.TESTPATH + "\\ANALYZETEST\\RELATEMASK" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

            return isgood;
        }
        int itemp = 0;
        /// <summary>
        /// 找鍵帽時使用的方法
        /// </summary>
        /// <param name="bmpinput"></param>
        /// <param name="tolerance"></param>
        /// <param name="doffsetlist"></param>
        public void B08_RunAndFindSimilar(Bitmap bmpinput, float tolerance, List<DoffsetClass> doffsetlist)
        {
            ALIGNPara.AuFindSimilar(bmpinput, 10000, tolerance, doffsetlist);
        }
        void GenDirectMask(Graphics grfx, PointF ptfoffset)
        {
            int i = 0;
            GraphicalObject grobj;

            switch (MaskMethod)
            {
                case MaskMethodEnum.NONE:

                    break;
                case MaskMethodEnum.DIRECTW:
                    i = 0;
                    while (i < myMover.Count)
                    {
                        grobj = myMover[i].Source;

                        (grobj as GeoFigure).DrawMask(grfx, ptfoffset, 0f, null, WhiteMaskBrush, nullSize);
                        i++;
                    }
                    break;
                case MaskMethodEnum.DIRECT:
                    i = 0;
                    while (i < myMover.Count)
                    {
                        grobj = myMover[i].Source;

                        (grobj as GeoFigure).DrawMask(grfx, ptfoffset, 0f, null, BlackMaskBrush, nullSize);
                        i++;
                    }
                    break;
            }

            foreach (AnalyzeClass branchanalyze in BranchList)
            {
                branchanalyze.GenDirectMask(grfx, ptfoffset);
            }

        }
        /// <summary>
        /// 產生 Relate Mask
        /// </summary>
        /// <param name="bmpinput"></param>
        /// <param name="ptfoffset"></param>
        /// <param name="branchlist"></param>
        public void CreateRelateMask(Bitmap bmpinput, PointF ptfoffset, List<AnalyzeClass> branchlist)
        {
            Graphics grfx = Graphics.FromImage(bmpinput);

            foreach (AnalyzeClass branch in branchlist)
            {
                CreateRelateMask(grfx, ptfoffset, branch);
            }
            grfx.Dispose();
        }
        void CreateRelateMask(Graphics grfx, PointF ptfoffset, AnalyzeClass branch)
        {
            int i = 0;
            GraphicalObject grobj;

            switch (branch.MaskMethod)
            {
                case MaskMethodEnum.RELATE:
                    i = 0;
                    while (i < branch.myMover.Count)
                    {
                        grobj = branch.myMover[i].Source;

                        (grobj as GeoFigure).DrawMask(grfx, ptfoffset, 0f, null, BlackMaskBrush, nullSize);
                        i++;
                    }
                    break;
                case MaskMethodEnum.RELATEW:
                    i = 0;
                    while (i < branch.myMover.Count)
                    {
                        grobj = branch.myMover[i].Source;

                        (grobj as GeoFigure).DrawMask(grfx, ptfoffset, 0f, null, WhiteMaskBrush, nullSize);
                        i++;
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 產生這個支內包含分支的最大範圍
        /// </summary>
        /// <returns></returns>
        RectangleF CreateOPRectF(Bitmap bmpinput, PointF offsetpoint)
        {
            RectangleF rectf = CreateOPRectF();

            foreach (AnalyzeClass branchanalyze in BranchList)
            {
                rectf = MergeTwoRectFs(rectf, branchanalyze.CreateOPRectF());
            }

            rectf.Offset(-offsetpoint.X, -offsetpoint.Y);

            rectf.Intersect(SimpleRectF(bmpinput.Size));

            //if (rectf.Width % 2 == 1)
            //    rectf.Width--;

            //if (rectf.Height % 2 == 1)
            //    rectf.Height--;

            return rectf;
        }
        RectangleF CreateOPRectF()
        {
            int i = 0;
            GraphicalObject grobj;

            RectangleF rectf = new RectangleF();

            i = 0;
            while (i < myMover.Count)
            {
                grobj = myMover[i].Source;
                rectf = MergeTwoRectFs(rectf, (grobj as GeoFigure).RealRectangleAround(0, 0));

                i++;
            }

            foreach (AnalyzeClass branchanalyze in BranchList)
            {
                rectf = MergeTwoRectFs(rectf, branchanalyze.CreateOPRectF());
            }

            rectf.Inflate(ExtendX, ExtendY);

            return rectf;
        }

        public bool CheckStilts(ref Bitmap outbmp, ref Bitmap runbmp)
        {

            Bitmap bmpALIGNED = new Bitmap(bmpOUTPUT);
            bool isgood = true;
            if (StiltsPara.StiltsMethod == STILTSMethodEnum.STILTS)
            {
                bmpWIP.Dispose();
                bmpWIP = new Bitmap(bmpALIGNED);

                Bitmap MaskTemp = new Bitmap(bmpPATTERN.Width, bmpPATTERN.Height, PixelFormat.Format32bppArgb);
                DrawRect(MaskTemp, BlackMaskBrush);
                A0202_CreateAlignMask(myOringinOffsetPointF, MaskTemp);

                //MaskTemp.Save("D:\\testtest\\maskTemp.png");

                Bitmap bmpRunLine = new Bitmap(bmpALIGNED);
                JetEazy.BasicSpace.myImageProcessor.SetMaskToStilts(bmpRunLine, MaskTemp, Color.White);
                //MaskTemp.Save("D:\\testtest\\maskTemp2.png");
                //bmpRunLine.Save("D:\\testtest\\runline.png");

                myImageProcessor.SetMaskToStilts(bmpRunLine, bmpMASK, StiltsPara.StiltsGrayValue, StiltsPara.StiltsNOGrayValue, Color.Black);
                //    JetEazy.BasicSpace.myImageProcessor.Balance(bmpRunLine, ref MaskTemp, myImageProcessor.EnumThreshold.Minimum);
                //bmpRunLine.Save("D:\\testtest\\runline2.png");
                //MaskTemp.Save("D:\\testtest\\maskTemp3.png");
                runbmp = bmpRunLine;
                Bitmap bmpblob = StiltsPara.FindBlob(true, bmpPATTERN, bmpWIP, bmpRunLine, PassInfo, out isgood);

                outbmp = bmpblob;
            }
            return isgood;
        }


        public void FillTrainStatus(WorkStatusCollectionClass trainstatuscollection)
        {
            foreach (WorkStatusClass works in TrainStatusCollection.WorkStatusList)
            {
                trainstatuscollection.Add(works);
            }
        }
        public void FillTrainStatus(WorkStatusCollectionClass trainstatuscollection, string filltoanalyzestr)
        {
            foreach (WorkStatusClass works in TrainStatusCollection.WorkStatusList)
            {
                if (works.LogString.IndexOf(filltoanalyzestr) < 0)
                {
                    works.LogString += filltoanalyzestr;
                    trainstatuscollection.Add(works);
                }
            }

            ALIGNPara.AddTrainLogString(filltoanalyzestr);
            INSPECTIONPara.AddTrainLogString(filltoanalyzestr);
            MEASUREPara.AddTrainLogString(filltoanalyzestr);
            OCRPara.AddTrainLogString(filltoanalyzestr);
            AOIPara.AddTrainLogString(filltoanalyzestr);
        }
        public void FillRunStatus(WorkStatusCollectionClass runstatuscollection)
        {
            AnalyzeClass LearnAnalyze = GetLearnByIndex(LastLearnIndex);

            LearnAnalyze.ALIGNPara.FillRunStatus(runstatuscollection);
            LearnAnalyze.INSPECTIONPara.FillRunStatus(runstatuscollection);
            LearnAnalyze.MEASUREPara.FillRunStatus(runstatuscollection);
            LearnAnalyze.OCRPara.FillRunStatus(runstatuscollection);
            LearnAnalyze.AOIPara.FillRunStatus(runstatuscollection);
            LearnAnalyze.StiltsPara.FillRunStatus(runstatuscollection);
            LearnAnalyze.PADPara.FillRunStatus(runstatuscollection);
            foreach (AnalyzeClass branchanalyze in LearnAnalyze.BranchList)
            {
                branchanalyze.FillRunStatus(runstatuscollection);
            }
            //foreach (AnalyzeClass learnanalyze in LearnAnalyze.LearnList)
            //{
            //    learnanalyze.FillRunStatus(runstatuscollection);
            //}
        }
        public void FillRunStatus(WorkStatusCollectionClass runstatuscollection, string filltoanalyzestr)
        {
            foreach (WorkStatusClass works in RunStatusCollection.WorkStatusList)
            {
                if (works.LogString.IndexOf(filltoanalyzestr) < 0)
                {
                    works.LogString += filltoanalyzestr;
                    runstatuscollection.Add(works);
                }
            }

            ALIGNPara.AddRunLogString(filltoanalyzestr);
            INSPECTIONPara.AddRunLogString(filltoanalyzestr);
            MEASUREPara.AddRunLogString(filltoanalyzestr);
            OCRPara.AddRunLogString(filltoanalyzestr);
            AOIPara.AddRunLogString(filltoanalyzestr);

        }

        /// <summary>
        /// 產生FindInside的需求
        /// </summary>
        /// <param name="bmpinput"></param>
        /// <param name="offsetpointf"></param>
        public void B02_CreateFindInsideRequirement(Bitmap bmpinput, PointF offsetpointf, List<Rectangle> rectlist, float threshold, int extend)
        {
            bool isroot = Level == 1;

            string str = "";

            string analyzestring = ToLogAnalyzeString();

            WorkStatusClass workstatus = new WorkStatusClass(AnanlyzeProcedureEnum.PREPARE);
            string processstring = "Start " + analyzestring + " Train Prepare." + Environment.NewLine;
            string errorstring = "";
            ReasonEnum reason = ReasonEnum.PASS;

            #region 產生校正用的MASK

            str = "Create Align Mask.";
            processstring += str + Environment.NewLine;

            processstring += analyzestring + " is not Root." + Environment.NewLine;

            myOPRectF = CreateOPRectF(bmpinput, offsetpointf);
            myOringinOffsetPointF = new PointF(offsetpointf.X + myOPRectF.X, offsetpointf.Y + myOPRectF.Y);

            bmpPATTERN.Dispose();
            bmpPATTERN = (Bitmap)bmpinput.Clone(myOPRectF, PixelFormat.Format32bppArgb);

            bmpWIP.Dispose();
            bmpWIP = new Bitmap(bmpPATTERN);

            bmpMASK.Dispose();
            bmpMASK = new Bitmap(bmpPATTERN.Width, bmpPATTERN.Height, PixelFormat.Format32bppArgb);

            DrawRect(bmpMASK, new SolidBrush(Color.Black));

            //產生 Pattern 及 Pattern 用的 Mask
            B0202_CreateFindInsideAlignMask(myOringinOffsetPointF, bmpMASK);

            #endregion

            workstatus.SetWorkStatus(bmpPATTERN, bmpWIP, bmpMASK, reason, errorstring, processstring, PassInfo);
            TrainStatusCollection.Add(workstatus);

            if (IsTempSave)
            {
                bmpPATTERN.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-PATTERN" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                bmpWIP.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-WIP" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                bmpMASK.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-MASK" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            }

            B03_GetFindIndInsideRectangles(rectlist, threshold, extend, myOringinOffsetPointF);

        }

        /// <summary>
        /// 依Pattern產生MASK
        /// </summary>
        void B0202_CreateFindInsideAlignMask(PointF ptfoffset, Bitmap bmpmask)
        {
            int i = 0;

            GraphicalObject grobj;
            Graphics grfx = Graphics.FromImage(bmpmask);

            while (i < myMover.Count)
            {
                grobj = myMover[i].Source;

                (grobj as GeoFigure).DrawMask(grfx, ptfoffset, 0f, null, WhiteMaskBrush, nullSize);
                i++;
            }
            grfx.Dispose();

        }
        void B03_GetFindIndInsideRectangles(List<Rectangle> rectlist, float threshold, int extend, PointF offsetptf)
        {
            HistogramClass histogram = new HistogramClass(2);
            JzFindObjectClass jzFind = new JzFindObjectClass();

            histogram.GetHistogram(bmpPATTERN, bmpMASK, true);

            //bmpPATTERN.Save("D://pattern.png");
            //bmpMASK.Save("D://Mask.png");

            int max = histogram.GetMaxRatioAVG(0.25f);
            int min = histogram.GetMinRatioAVG(0.25f);

            int mode = histogram.ModeGrade; //從眾數去看是黑底白字還是白底黑字

            if (Math.Abs(max - mode) > Math.Abs(min - mode)) //這樣表示是黑底白字，不然就是白底黑字
                jzFind.SetThreshold(bmpPATTERN, bmpMASK, 255, min + (int)(((float)(max - min)) * threshold), 255, 0, true);
            else
                jzFind.SetThreshold(bmpPATTERN, bmpMASK, 255, min + (int)(((float)(max - min)) * (1 - threshold)), 255, 0, false);

            if (IsTempSave)
            {
                bmpPATTERN.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-THRESHOLD" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            }

            //然後找到所有的裏面的東東
            jzFind.Find(bmpPATTERN, Color.Red);

            List<Rectangle> FoundRectList = rectlist;

            foreach (FoundClass found in jzFind.FoundList)
            {
                Rectangle rect = found.rect;

                rect.Inflate(extend, extend);

                FoundRectList.Add(rect);
            }

            int i = 0;
            int j = 0;


            switch(VERSION)
            {
                case VersionEnum.ALLINONE:
                    switch(OPTION)
                    {
                        case OptionEnum.MAIN_X6:
                        case JetEazy.OptionEnum.MAIN_SERVICE:

                            while (i < FoundRectList.Count - 1)
                            {
                                if (FoundRectList[i].Width == 0)
                                {
                                    i++;
                                    continue;
                                }

                                Rectangle recti = FoundRectList[i];
                                DrawRect(bmpPATTERN, recti, new Pen(Color.Lime, 3));

                                recti.X += (int)offsetptf.X;
                                recti.Y += (int)offsetptf.Y;

                                FoundRectList.RemoveAt(i);
                                FoundRectList.Insert(i, recti);

                                i++;
                            }

                            break;
                    }
                    break;
                default:

                    #region 拼接成一个
                    while (i < FoundRectList.Count - 1)
                    {
                        if (FoundRectList[i].Width == 0)
                        {
                            i++;
                            continue;
                        }

                        j = i + 1;

                        //注意是否 Merge 後還是不改變
                        while (j < FoundRectList.Count)
                        {
                            if (FoundRectList[j].Width == 0)
                            {
                                j++;
                                continue;
                            }

                            Rectangle recti = FoundRectList[i];
                            Rectangle rectj = FoundRectList[j];

                            if (recti.IntersectsWith(rectj))
                            {
                                rectj = MergeTwoRects(recti, rectj);
                                recti = new Rectangle(0, 0, 0, 0);

                                FoundRectList.RemoveAt(i);
                                FoundRectList.Insert(i, recti);

                                FoundRectList.RemoveAt(j);
                                FoundRectList.Insert(j, rectj);

                                break;
                            }

                            j++;
                        }
                        i++;
                    }

                    i = FoundRectList.Count - 1;

                    while (i > -1)
                    {
                        if (FoundRectList[i].Width == 0)
                            FoundRectList.RemoveAt(i);
                        else
                        {
                            Rectangle recti = FoundRectList[i];

                            DrawRect(bmpPATTERN, recti, new Pen(Color.Lime, 3));

                            recti.X += (int)offsetptf.X;
                            recti.Y += (int)offsetptf.Y;

                            FoundRectList.RemoveAt(i);
                            FoundRectList.Insert(i, recti);
                        }
                        i--;
                    }
                    #endregion

                    break;
            }



            if (IsTempSave)
            {
                bmpPATTERN.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-THRESHOLDFOUND" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            }

        }
        /// <summary>
        /// 運算自身的定位及產生子Analyze的圖像及定位，Learn From Here
        /// </summary>
        /// <param name="ismultithread"></param>
        /// <param name="bmpinput"></param>
        /// <param name="offsetpoint"></param>
        /// <returns></returns>
#endregion
        #region Inspection Operation

        bool I01_InspectionProcess(bool istrain)
        {
            bool isgood = true;

            if (istrain)
            {
                INSPECTIONPara.I01_GetInspectionRequirement(bmpPATTERN, bmpMASK, ToAnalyzeString(), PassInfo);
            }
            else
            {
                //if (No == 21)
                //    bmpOUTPUT.Save(Universal.TESTPATH + "\\ANALYZETEST\\before input" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                isgood = INSPECTIONPara.I08_InspectionProcess(bmpWIP, ref bmpOUTPUT);
            }


            return isgood;
        }

        bool P10_PADInspectionProcess(bool istrain)
        {
            bool isgood = true;

            if (istrain)
            {
                JzToolsClass toolsClass=new JzToolsClass();
                Rectangle rect = toolsClass.SimpleRect(bmpPATTERN.Size);
                rect.Inflate(-ExtendX, -ExtendY);

                Bitmap mybmptemp = bmpPATTERN.Clone(rect,PixelFormat.Format32bppArgb);
                PADPara.P10_GetPADInspectionRequirement(mybmptemp, bmpMASK, ToAnalyzeString(), PassInfo);
                mybmptemp.Dispose();
                //PADPara.P10_GetPADInspectionRequirement(bmpPATTERN, bmpMASK, ToAnalyzeString(), PassInfo);
            }
            else
            {
                //if (No == 21)
                //    bmpOUTPUT.Save(Universal.TESTPATH + "\\ANALYZETEST\\before input" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                //JzToolsClass toolsClass = new JzToolsClass();
                //Rectangle rect = toolsClass.SimpleRect(bmpPATTERN.Size);
                //rect.Inflate(-ExtendX, -ExtendY);
                //Bitmap mybmptemp = bmpWIP.Clone(rect, PixelFormat.Format32bppArgb);
                switch (PADPara.PADMethod)
                {
                    case PADMethodEnum.PADCHECK:

                        JzToolsClass toolsClass = new JzToolsClass();
                        Rectangle rect = toolsClass.SimpleRect(bmpPATTERN.Size);
                        rect.Inflate(-ExtendX, -ExtendY);
                        Bitmap mybmptemp = bmpWIP.Clone(rect, PixelFormat.Format32bppArgb);

                        isgood = PADPara.PB10_PADInspectionProcess(mybmptemp, ref bmpOUTPUT);
                        mybmptemp.Dispose();
                        break;
                    case PADMethodEnum.GLUECHECK:
                        isgood = PADPara.PB10_GlueInspectionProcess(bmpWIP, ref bmpOUTPUT);
                        break;
                    case PADMethodEnum.GLUECHECK_BlackEdge:
                        isgood = PADPara.PB10_GlueInspectionProcess_BlackEdge(bmpWIP, ref bmpOUTPUT);
                        break;
                    case PADMethodEnum.PLACODE_CHECK:
                        isgood = PADPara.PB10_GlacodeInspectionProcess(bmpWIP, ref bmpOUTPUT);
                        break;
                    case PADMethodEnum.QLE_CHECK:
                        isgood = PADPara.PB10_QLECheckInspectionProcess(bmpWIP, ref bmpOUTPUT);
                        break;
                }
                //isgood = PADPara.PB10_PADInspectionProcess(bmpWIP, ref bmpOUTPUT);
            }
            return isgood;
        }

        public void Z02_CreateTrainRequirement(Bitmap bmpinput, PointF offsetpointf)
        {
            string str = "";

            string analyzestring = ToLogAnalyzeString();

            WorkStatusClass workstatus = new WorkStatusClass(AnanlyzeProcedureEnum.PREPARE);
            string processstring = "Start " + analyzestring + " Train Prepare." + Environment.NewLine;
            string errorstring = "";
            ReasonEnum reason = ReasonEnum.PASS;

            #region 產生校正用的MASK

            str = "Create Align Mask.";
            processstring += str + Environment.NewLine;

            processstring += analyzestring + " is not Root." + Environment.NewLine;

            myOPRectF = CreateOPRectF(bmpinput, offsetpointf);
            myOringinOffsetPointF = new PointF(offsetpointf.X + myOPRectF.X, offsetpointf.Y + myOPRectF.Y);

            bmpPATTERN.Dispose();
            bmpPATTERN = (Bitmap)bmpinput.Clone(myOPRectF, PixelFormat.Format32bppArgb);

            bmpWIP.Dispose();
            bmpWIP = new Bitmap(bmpPATTERN);

            bmpMASK.Dispose();
            bmpMASK = new Bitmap(bmpPATTERN.Width, bmpPATTERN.Height, PixelFormat.Format32bppArgb);

            //檢查是否需要檢測髒污的鍵帽，不需要一定得檢查髒污才能用Boarder
            //if (AOIPara.CheckDirtMethod != CheckDirtMethodEnum.NONE)
            {
                switch (ALIGNPara.AlignMode)
                {
                    case AlignModeEnum.BORDER:

                        str = " Use Borader Alignment.";
                        processstring += analyzestring + str + Environment.NewLine;

                        DrawRect(bmpMASK, WhiteMaskBrush);
                        break;
                    default:

                        str = " Use Area Alignment.";
                        processstring += analyzestring + str + Environment.NewLine;

                        DrawRect(bmpMASK, BlackMaskBrush);
                        break;
                }
                //bmpMASK.Save(Universal.TESTPATH + "\\ANALYZETEST\\RELATEMASK" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            }

            //產生 Pattern 及 Pattern 用的 Mask
            A0202_CreateAlignMask(myOringinOffsetPointF, bmpMASK);

            //if(islearn)
            //    bmpMASK.Save(Universal.TESTPATH + "\\ANALYZETEST\\LEARNMASK" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

            #endregion

            if (IsTempSave)
            {
                bmpPATTERN.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-PATTERN" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                bmpWIP.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-WIP" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                bmpMASK.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + NoSaveStr + "-MASK" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            }

            workstatus.SetWorkStatus(bmpPATTERN, bmpWIP, bmpWIP, reason, errorstring, processstring, PassInfo);
            TrainStatusCollection.Add(workstatus);
        }

        public bool Z051_AlignTrainProcess()
        {
            bool isgood = true;

            //switch (ALIGNPara.AlignMode)
            //{
            //    case AlignModeEnum.BORDER:
            //        bmpHOLLOW.Dispose();
            //        bmpHOLLOW = new Bitmap(bmpMASK);
            //        break;
            //}
            isgood = ALIGNPara.AlignTrainProcess(bmpWIP, ref bmpPATTERN, bmpMASK, this.Brightness, this.Contrast, ToLogAnalyzeString(), PassInfo, true);
            ALIGNPara.FillTrainStatus(TrainStatusCollection, ToLogString());

            //isgood = MEASUREPara.MeasureProcess(bmpWIP, bmpPATTERN, bmpMASK, this.Brightness, this.Contrast, ToAnalyzeString(), PassInfo, true);
            //MEASUREPara.FillTrainStatus(TrainStatusCollection, ToLogString());

            return isgood;
        }

        public bool IsZ05Good = false;
        public void Z05_AlignTrainProcess()
        {
            bool isgood = false;
            bool istraingood = false;
            bool isalligngood = false;

            #region Train Affairs

            istraingood = A05_AlignTrainProcess();
            isgood = istraingood;

            //  ALIGNPara.IsTempSave = true;
            isgood = A06_RunProcess(true, ref isalligngood);
            //  ALIGNPara.IsTempSave = false;
            if (isgood)
            {
                if (AOIPara.CheckDirtMethod != CheckDirtMethodEnum.NONE && ALIGNPara.AlignMode == AlignModeEnum.BORDER)
                {
                    DigForDirtCheck();
                    AOIPara.CheckDirt(bmpWIP, bmpHOLLOW, true, PassInfo);

                    AOIPara.FillTrainStatus(TrainStatusCollection, ToLogString());
                }
            }

            #endregion

            #region Run Affairs

            if (isgood)
            {
                bmpPATTERN.Save(Universal.TESTPATH + "\\ANALYZETEST\\PATTERN" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                bmpWIP.Save(Universal.TESTPATH + "\\ANALYZETEST\\WIP" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                bmpOUTPUT.Save(Universal.TESTPATH + "\\ANALYZETEST\\OUTPUT" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                //ALIGNPara.IsTempSave = true;
                //isgood = A06_RunProcess(false, ref isalligngood, true);
                //ALIGNPara.IsTempSave = false;

                //if (isgood)
                //{
                //bmpWIP.Dispose();
                //bmpWIP = new Bitmap(bmpOUTPUT);
                //}
            }

            if (isgood)
            {
                if (AOIPara.CheckDirtMethod != CheckDirtMethodEnum.NONE && ALIGNPara.AlignMode == AlignModeEnum.BORDER)
                {
                    //DigForDirtCheck();

                    //bmpHOLLOW.Save(Universal.TESTPATH + "\\ANALYZETEST\\HOLLOW" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                    isgood = AOIPara.CheckDirt(bmpOUTPUT, bmpHOLLOW, false, PassInfo);

                    bmpWIP.Dispose();
                    bmpWIP = new Bitmap(AOIPara.bmpDirt);

                    //bmpWIP.Save(Universal.TESTPATH + "\\ANALYZETEST\\WIP" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                    AOIPara.FillRunStatus(TrainStatusCollection, ToLogString());
                }
            }

            //switch(OPTION)
            //{
            //    case OptionEnum.MAIN_SD:

            if (PADPara.PADMethod == PADMethodEnum.PADCHECK)
            {
                bmpWIP.Dispose();
                bmpWIP = new Bitmap(PADPara.bmpPadFindOutput);

                PADPara.FillRunStatus(TrainStatusCollection, ToLogString());
            }
            else if (PADPara.PADMethod == PADMethodEnum.GLUECHECK)
            {
                bmpWIP.Dispose();
                bmpWIP = new Bitmap(PADPara.bmpPadFindOutput);

                PADPara.FillRunStatus(TrainStatusCollection, ToLogString());
            }
            else if (PADPara.PADMethod == PADMethodEnum.GLUECHECK_BlackEdge)
            {
                bmpWIP.Dispose();
                bmpWIP = new Bitmap(PADPara.bmpPadFindOutput);

                PADPara.FillRunStatus(TrainStatusCollection, ToLogString());
            }
            else if (PADPara.PADMethod == PADMethodEnum.PLACODE_CHECK)
            {
                bmpWIP.Dispose();
                bmpWIP = new Bitmap(PADPara.bmpPadFindOutput);

                PADPara.FillRunStatus(TrainStatusCollection, ToLogString());
            }
            else if (PADPara.PADMethod == PADMethodEnum.QLE_CHECK)
            {
                bmpWIP.Dispose();
                bmpWIP = new Bitmap(PADPara.bmpPadFindOutput);

                PADPara.FillRunStatus(TrainStatusCollection, ToLogString());
            }

            //        break;
            //}

            #endregion

            if (istraingood)
            {
                if (ALIGNPara.AlignMethod == AlignMethodEnum.AUFIND)
                {
                    bmpPATTERN.Dispose();
                    bmpPATTERN = new Bitmap(ALIGNPara.bmpContour);
                }
            }

            IsZ05Good = isgood;

        }
        public void CoverMask()
        {
            JzFindObjectClass jzfind = new JzFindObjectClass();
            jzfind.MergeImage(bmpPATTERN, bmpMASK, Color.White, Color.FromArgb(120, Color.Lime), true);

        }

        public void SetPassInfoOPString(string opstr)
        {
            PassInfo.OperateString = opstr;

            foreach (AnalyzeClass analyze in BranchList)
            {
                analyze.SetPassInfoOPString(opstr);
            }

            foreach (AnalyzeClass analyze in LearnList)
            {
                analyze.SetPassInfoOPString(opstr);
            }

        }

        #endregion

        #region Application Operation

        /// <summary>
        /// 在重新訓練前所需清除的資料
        /// </summary>
        public void ResetTrainStatus()
        {
            TrainStatusCollection.Clear();

            ALIGNPara.ResetTrainStatus();
            INSPECTIONPara.ResetTrainStatus();
            MEASUREPara.ResetTrainStatus();
            OCRPara.ResetTrainStatus();
            AOIPara.ResetTrainStatus();
            foreach (AnalyzeClass branchanalyze in BranchList)
            {
                branchanalyze.ResetTrainStatus();
            }
            foreach (AnalyzeClass learnanalyze in LearnList)
            {
                learnanalyze.ResetTrainStatus();
            }
        }
        /// <summary>
        /// 在重新測試前所需清除的資料
        /// </summary>
        public void ResetRunStatus()
        {
            IsVeryGood = false;
            IsOperated = false;
            LastLearnIndex = 0;

            RunStatusCollection.Clear();

            ALIGNPara.ResetRunStatus();
            INSPECTIONPara.ResetRunStatus();
            MEASUREPara.ResetRunStatus();
            OCRPara.ResetRunStatus();
            AOIPara.ResetRunStatus();
            StiltsPara.ResetRunStatus();
            PADPara.ResetRunStatus();
            foreach (AnalyzeClass branchanalyze in BranchList)
            {
                branchanalyze.ResetRunStatus();
            }
            foreach (AnalyzeClass learnanalyze in LearnList)
            {
                learnanalyze.ResetRunStatus();
            }
        }
        public bool IsAlignPass()
        {
            bool ret = true;

            if (ALIGNPara.RunStatusCollection.GetRunStatus(0).Reason == ReasonEnum.NG)
            {
                ret = false;
            }
            return ret;
        }
        /// <summary>
        /// 將多的東西挖空
        /// </summary>
        void DigForDirtCheck()
        {
            switch (ALIGNPara.AlignMode)
            {
                case AlignModeEnum.BORDER:

                    bmpHOLLOW.Dispose();
                    bmpHOLLOW = new Bitmap(bmpMASK);

                    //bmpHOLLOW.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + ToAnalyzeTestString() + " START DIG " + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                    foreach (AnalyzeClass branchanalyze in BranchList)
                    {
                        branchanalyze.DigForDirtCheck(bmpHOLLOW, 0, myOringinOffsetPointF);
                    }
                    break;
            }
        }
        /// <summary>
        /// 將自己的旋轉及偏移含以下的所有的
        /// </summary>
        /// <param name="bmp"></param>
        void DigForDirtCheck(Bitmap bmphollow, float aligndegree, PointF alignoffset)
        {
            int i = 0;

            GraphicalObject gropj;
            Graphics g = Graphics.FromImage(bmphollow);

            float NextDegree = aligndegree + ALIGNPara.AlignDegree;
            PointF NextOffset = MergeTwoPointF(alignoffset, ALIGNPara.AlignOffset);

            while (i < myMover.Count)
            {
                gropj = myMover[i].Source;
                (gropj as GeoFigure).DrawMask(g, NextOffset, NextDegree, null, WhiteMaskBrush, bmphollow.Size);
                i++;
            }

            g.Dispose();

            //bmphollow.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + ToAnalyzeTestString() + " DIG " + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

            foreach (AnalyzeClass branchanalyze in BranchList)
            {
                branchanalyze.DigForDirtCheck(bmphollow, NextDegree, NextOffset);
            }
        }

        /// <summary>
        /// 在 DetailForm 時取得Analyze 裏的圖片資料
        /// </summary>
        /// <param name="bmpinput"></param>
        /// <param name="offsetpointf"></param>
        public void GetDirectbmpPattern(Bitmap bmpinput, PointF offsetpointf)
        {
            myOPRectF = CreateOPRectF(bmpinput, offsetpointf);

            bmpPATTERN.Dispose();
            bmpPATTERN = (Bitmap)bmpinput.Clone(myOPRectF, PixelFormat.Format32bppArgb);

            myOringinOffsetPointF = myOPRectF.Location;

            //bmpPATTERN.Save(Universal.TESTPATH + "\\ANALYZETEST\\PATTERNVIEW" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

        }
        public RectangleF GetMoverRectF(Bitmap bmp)
        {
            myOPRectF = CreateOPRectF(bmp, new PointF(0, 0));

            return myOPRectF;
        }
        public RectangleF GetMyMoverRectF()
        {
            int i = 0;
            GraphicalObject grobj;

            RectangleF rectf = new RectangleF();

            i = 0;
            while (i < myMover.Count)
            {
                grobj = myMover[i].Source;
                rectf = MergeTwoRectFs(rectf, (grobj as GeoFigure).RealRectangleAround(0, 0));

                i++;
            }

            return rectf;
        }
        #endregion

        #region Tools Operation

        private static unsafe Bitmap[] test(Bitmap bmpSrc, Rectangle[] rects)
        {
            Rectangle r = new Rectangle(0, 0, bmpSrc.Width, bmpSrc.Height);
            BitmapData bmpData = bmpSrc.LockBits(r, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte* ptr = (byte*)bmpData.Scan0;
            int stride = bmpData.Stride - bmpSrc.Width * 4;
            Bitmap[] bmps = new Bitmap[rects.Length];
            BitmapData bmpDataTemp;
            byte* ptrTemp;
            int strideTemp;
            byte* bigBmpPtr;
            for (int i = 0; i < bmps.Length; i++)
            {
                bmps[i] = new Bitmap(rects[i].Width, rects[i].Height, PixelFormat.Format32bppArgb);
                //小图锁内存
                bmpDataTemp = bmps[i].LockBits(new Rectangle(0, 0, rects[i].Width, rects[i].Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                ptrTemp = (byte*)bmpDataTemp.Scan0;
                strideTemp = bmpDataTemp.Stride - rects[i].Width * 4;
                //在大图上找到小图(0,0)点对应指针
                bigBmpPtr = (byte*)(new IntPtr(((IntPtr)ptr).ToInt64() + bmpData.Stride * rects[i].Y + rects[i].X * 4));
                for (int x = 0; x < bmps[i].Height; x++)
                {
                    for (int y = 0; y < bmps[i].Width; y++)
                    {
                        ptrTemp[0] = bigBmpPtr[0];
                        ptrTemp[1] = bigBmpPtr[1];
                        ptrTemp[2] = bigBmpPtr[2];
                        ptrTemp[3] = bigBmpPtr[3];
                        ptrTemp += 4;
                        bigBmpPtr += 4;
                    }
                    bigBmpPtr += (bmpData.Stride - rects[i].Width * 4);
                    ptrTemp += strideTemp;
                }
                //释放小图内存占用
                bmps[i].UnlockBits(bmpDataTemp);
            }
            //释放大图内存占用
            bmpSrc.UnlockBits(bmpData);
            return bmps;
        }
        public void BoundRect(ref RectangleF InnerRect, Size BoundSize)
        {
            InnerRect.X = Math.Min(Math.Max(InnerRect.X, 0), (BoundSize.Width - InnerRect.Width < 0 ? 0 : BoundSize.Width - InnerRect.Width));
            InnerRect.Y = Math.Min(Math.Max(InnerRect.Y, 0), (BoundSize.Height - InnerRect.Height < 0 ? 0 : BoundSize.Height - InnerRect.Height));

            if (BoundSize.Width <= InnerRect.X + InnerRect.Width)
                InnerRect.Width = BoundValue(InnerRect.Width, BoundSize.Width - InnerRect.X, 1);
            if (BoundSize.Height <= InnerRect.Height + InnerRect.Height)
                InnerRect.Height = BoundValue(InnerRect.Height, BoundSize.Height - InnerRect.Y, 1);
        }
        public float BoundValue(float Value, float Max, float Min)
        {
            return Math.Max(Math.Min(Value, Max), Min);

        }
        RectangleF SimpleRectF(Size sz)
        {
            return new RectangleF(0f, 0f, (float)sz.Width, (float)sz.Height);
        }
        RectangleF MergeTwoRectFs(RectangleF rectf1, RectangleF rectf2)
        {
            RectangleF rectf = new RectangleF();

            if (rectf1.Width == 0)
                return rectf2;
            if (rectf2.Width == 0)
                return rectf1;

            rectf.X = Math.Min(rectf1.X, rectf2.X);
            rectf.Y = Math.Min(rectf1.Y, rectf2.Y);

            rectf.Width = Math.Max(rectf1.X + rectf1.Width, rectf2.X + rectf2.Width) - rectf.X;
            rectf.Height = Math.Max(rectf1.Y + rectf1.Height, rectf2.Y + rectf2.Height) - rectf.Y;

            return rectf;
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
        PointF MergeTwoPointF(PointF orgptf, PointF alignoffset)
        {
            return new PointF(orgptf.X + alignoffset.X, orgptf.Y + alignoffset.Y);
        }
        void DrawRect(Bitmap bmp, SolidBrush fillsolid)
        {
            Graphics g = Graphics.FromImage(bmp);

            g.FillRectangle(fillsolid, SimpleRectF(bmp.Size));
            g.Dispose();
        }

        void DrawText(Bitmap BMP, string Text)
        {
            SolidBrush B = new SolidBrush(Color.Lime);
            Font MyFont = new Font("Arial", 24);

            Graphics g = Graphics.FromImage(BMP);
            g.DrawString(Text, MyFont, B, new PointF(5, 5));
            g.Dispose();
        }

        void DrawRect(Bitmap bmp, Rectangle rect, Pen roundpen)
        {
            Graphics g = Graphics.FromImage(bmp);
            g.DrawRectangle(roundpen, rect);
            g.Dispose();
        }
        void DrawRect(Bitmap bmp, Rectangle rect, SolidBrush fillsolid)
        {
            Graphics g = Graphics.FromImage(bmp);
            g.FillRectangle(fillsolid, rect);
            g.Dispose();
        }
        void DrawImage(Bitmap bmpfrom, Bitmap bmpto, RectangleF destrectf)
        {
            Graphics g = Graphics.FromImage(bmpto);
            g.DrawImage(bmpfrom, destrectf, SimpleRect(bmpfrom.Size), GraphicsUnit.Pixel);
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
        string RectFToString(RectangleF rectF)
        {
            string Str = "";

            Str += rectF.X.ToString() + ",";
            Str += rectF.Y.ToString() + ",";
            Str += rectF.Width.ToString() + ",";
            Str += rectF.Height.ToString();

            return Str;
        }
        RectangleF StringToRectF(string str)
        {
            string[] strs = str.Split(',');
            RectangleF rectF = new RectangleF();

            rectF.X = float.Parse(strs[0]);
            rectF.Y = float.Parse(strs[1]);
            rectF.Width = float.Parse(strs[2]);
            rectF.Height = float.Parse(strs[3]);

            return rectF;


        }
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
        void GetBMP(string BMPFileStr, ref Bitmap BMP)
        {
            Bitmap bmpTMP = new Bitmap(BMPFileStr);

            BMP.Dispose();
            BMP = new Bitmap(bmpTMP);

            bmpTMP.Dispose();
        }

        void ShowTrainMessage(WorkStatusCollectionClass trainstatuscollection)
        {
            List<string> messagelist = new List<string>();

            string allstr = "";

            foreach (WorkStatusClass works in trainstatuscollection.WorkStatusList)
            {
                allstr += works.ProcessString + Environment.NewLine;

                string[] strs = works.ProcessString.Replace(Environment.NewLine, Universal.NewlineChar.ToString()).Split(Universal.NewlineChar);

                foreach (string str in strs)
                {
                    messagelist.Add(str);
                }
            }

            OnPrintMessage(messagelist);

            // SaveData(allstr, Universal.TESTPATH + "\\" + ToLogAnalyzeString() + " Process.log");

            //trainstatuscollection.SaveProcessAndError(Universal.WORKPATH);
        }
        void ShowTrainMessage(string str)
        {
            List<string> messagelist = new List<string>();

            messagelist.Add(str);

            OnPrintMessage(messagelist);
        }

        public delegate void PrintMessageHandler(List<string> processstringlist);
        public event PrintMessageHandler PrintMessageAction;
        public void OnPrintMessage(List<string> processstringlist)
        {
            if (PrintMessageAction != null)
            {
                PrintMessageAction(processstringlist);

                processstringlist.Clear();
            }
        }

        #endregion

        #region SMOOTH
        public void ResetChipData()
        {
            CheckItemsList.Clear();
        }
        public void CalculateChipWidth()
        {
            CheckItemsList.Clear();
            //switch(PADPara.PADMethod)
            //{
            //    case PADMethodEnum.PADCHECK:
            //        return;
            //        break;
            //}

            if (PADPara.PADMethod == PADMethodEnum.GLUECHECK
                || PADPara.PADMethod == PADMethodEnum.GLUECHECK_BlackEdge)
            {
            }
            else
            {
                return;
            }
            if (PADPara.glues == null)
                return;

            double aa = double.Parse(DateTime.Now.ToString("ss.fff"));
            long xx = DateTime.Now.Ticks;
            string aStr = xx.ToString();
            if (aStr.Length > 6)
            {
                aa = double.Parse(aStr.Substring(0, 6)) * 0.0001;
            }
            else
            {
                aa = double.Parse(aStr) * 0.0001;
            }

            int i = 0;

            while (i < (int)BorderTypeEnum.COUNT)
            {
                xx = DateTime.Now.Ticks;
                aStr = xx.ToString();
                if (aStr.Length > 6)
                {
                    aa = double.Parse(aStr.Substring(0, 6)) * 0.0001;
                }
                else
                {
                    aa = double.Parse(aStr) * 0.0001;
                }

                CheckItemClass checkitem = new CheckItemClass();
                //double aa = double.Parse(DateTime.Now.ToString("ss.fff"));
                checkitem.BorderType = (BorderTypeEnum)i;
                //checkitem.ChipMin = PADPara.glues[i].GetMinMM() + (float)GetRandom(-50000, 50000) * 0.000000013 * aa;// + GetRandom(-0.1, 0.1);
                //checkitem.ChipMax = PADPara.glues[i].GetMaxMM() + (float)GetRandom(-30000, 30000) * 0.000000012 * aa;// + GetRandom(-0.1, 0.1);
                checkitem.ChipMin = PADPara.glues[i].GetMinMM() + (float)GetRandom(-aa * 0.000013, aa * 0.000015);// + GetRandom(-0.1, 0.1);
                checkitem.ChipMax = PADPara.glues[i].GetMaxMM() + (float)GetRandom(-aa * 0.000012, aa * 0.000017);// + GetRandom(-0.1, 0.1);

                CheckItemsList.Add(checkitem);
                //System.Threading.Thread.Sleep(1);
                i++;
            }
        }
        public string ToReportString1()
        {
            int i = 0;
            string str = string.Empty;

            if (!PADPara.RunDataOK)
                return str;

            foreach (CheckItemClass checkitem in CheckItemsList)
            {
                str += checkitem.ToReportString() + ",";
                //switch (checkitem.BorderType)
                //{
                //    case BorderTypeEnum.LEFT:
                //        break;
                //    case BorderTypeEnum.TOP:
                //        break;
                //    case BorderTypeEnum.RIGHT:
                //        break;
                //    case BorderTypeEnum.BOTTOM:
                //        break;
                //}
            }
            return str;
        }

        public List<CheckItemClass> CheckItemsList = new List<CheckItemClass>();
        public List<CheckItemClass> CheckSimiliarList = new List<CheckItemClass>();

        public float CheckSimilar(int seq, ref float similiarstep)
        {
            float ret = 0;
            int i = 0;

            foreach (CheckItemClass checkitem in CheckSimiliarList)
            {
                ret += (checkitem.Compare(seq, CheckItemsList[i]) ? 1 : 0);

                i++;
            }

            similiarstep = i;

            return ret;

        }

        public int BackupCount = 0;

        public void BackupData()
        {
            int i = 0;

            if (CheckSimiliarList.Count == 0)
            {
                foreach (CheckItemClass checkitem in CheckItemsList)
                {
                    CheckSimiliarList.Add(new CheckItemClass(checkitem.ToString()));
                }

                BackupCount++;
            }
            else
            {
                i = 0;
                foreach (CheckItemClass checkitem in CheckItemsList)
                {
                    CheckSimiliarList[i].Backup(checkitem);
                    i++;
                }

                BackupCount++;

            }
        }
        public void RestoreData(int seq)
        {
            if (AliasName == "RCmd")
            {
                AliasName = AliasName;
            }

            int i = 0;
            foreach (CheckItemClass checkitem in CheckSimiliarList)
            {
                checkitem.Restore(seq, CheckItemsList[i]);
                i++;
            }

        }

        double GetRandom(double minval, double maxval)
        {
            Random rnd = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

            double diffval = maxval - minval;

            diffval = diffval * (double)rnd.Next(1, 999999) / 1000000d;
            
            return minval + diffval;
        }
        #endregion

    }
    public class CheckItemClass
    {
        float CompDiff = 0.12f;
        float CutDiff = 0.3f;

        float RestoreDiff = 0.065f;

        public BorderTypeEnum BorderType = BorderTypeEnum.LEFT;

        public double ChipMin = 0;
        public double ChipMax = 0;

        public CheckItemClass()
        {


        }
        public CheckItemClass(string Str)
        {
            FromString(Str);

            Backup(this);

        }
        public override string ToString()
        {
            string Str = "";

            Str += ((int)BorderType).ToString() + ",";
            Str += ChipMin.ToString() + ",";
            Str += ChipMax.ToString();

            return Str;
        }
        public void FromString(string Str)
        {
            string[] strs = Str.Split(',');

            BorderType = (BorderTypeEnum)int.Parse(strs[0]);
            ChipMin = double.Parse(strs[1]);
            ChipMax = double.Parse(strs[2]);
        }

        public string ToReportString()
        {
            string Str = "";

            //Str += BorderType.ToString() + ",";
            Str += ChipMin.ToString("0.000000") + ",";
            Str += ChipMax.ToString("0.000000");

            return Str;
        }
        public List<string> BackupList = new List<string>();

        public bool Check(double val)
        {
            bool ret = false;
            return ret;
        }
        public void Backup(CheckItemClass checkitem)
        {
            BackupList.Add(checkitem.ChipMin.ToString() + "@" + checkitem.ChipMax.ToString());
            if (BackupList.Count > 100)
                BackupList.RemoveAt(0);
        }
        public bool Compare(int index, CheckItemClass checkitem)
        {
            bool ret = false;

            string[] strs = BackupList[index].Split('@');
            ret = IsInRange(float.Parse(strs[0]), checkitem.ChipMin, CompDiff) 
                    && IsInRange(float.Parse(strs[1]), checkitem.ChipMax, CompDiff);
            
            return ret;
        }
        public void Restore(int index, CheckItemClass checkitem)
        {

            string[] strs = BackupList[index].Split('@');

            float V1 = 0;
            float V2 = 0;

            V1 = float.Parse(strs[0]);
            V2 = float.Parse(strs[1]);

            if (IsInRange(V1, checkitem.ChipMin, CutDiff))
                checkitem.ChipMin = (IsInRange(V1, checkitem.ChipMin, RestoreDiff) ?
                    checkitem.ChipMin : V1 + (float)GetRandom(-0.008666, 0.008666));

            if (IsInRange(V2, checkitem.ChipMax, CutDiff))
                checkitem.ChipMax = (IsInRange(V2, checkitem.ChipMax, RestoreDiff) ?
                    checkitem.ChipMax : V2 + (float)GetRandom(-0.008666, 0.008666));

        }
        bool IsInRange(double FromValue, double CompValue, double DiffValue)
        {
            return Math.Abs(FromValue - CompValue) < DiffValue;
        }
        double GetRandom(double minval, double maxval)
        {
            Random rnd = new Random();

            double diffval = maxval - minval;

            diffval = diffval * (double)rnd.Next(1, 999999) / 1000000d;

            return minval + diffval;
        }
    }
}
