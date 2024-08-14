using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
//using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Allinone.BasicSpace;
using Allinone.OPSpace;
using Allinone.UISpace;
using JetEazy.BasicSpace;
using JzDisplay;
using MoveGraphLibrary;
using WorldOfMoveableObjects;

using AForge.Imaging.Filters;
using JetEazy.PlugSpace;
using JetEazy.FormSpace;
using System.IO;
using JetEazy;

namespace Allinone.FormSpace
{
    public partial class KBAOIForm : Form
    {



        ////显示鼠标
        //ShowCursor(1);

        ////隐藏鼠标
        //ShowCursor(0);


        enum TagEnum
        {
            FINDSIMILIAR,
            FINDSIMILIAR2,
            FINDINSIDE,
            MARKTHESAME,

            EXIT,
        }

        PropertyGrid m_PG;

        PageUI PAGEUI;

        EnvClass ENVNow;

        NumericUpDown numSimRatio;
        Button btnFindSimiliar;
        Button btnFindSimiliarCV;

        NumericUpDown numThreshold;
        NumericUpDown numExtend;
        Button btnFindInside;

        Button btnExit;

        CheckBox IsMeasure;
        CheckBox IsMask;
        CheckBox IsTest;
        CheckBox IsTest2_Relative;

        CheckBox IsRegionPara;
        CheckBox IsTestPara;

        Button btnMarkTheSame;

        Label lblMessage;


        string strAnalyzeCurrentBlock = string.Empty;

        Button btnGetSelectImageBlock;
        Button btnBlockAutoFind;
        Button btnSetSelectBlock;

        Button btnBlockAdd;
        Button btnBlockDelete;
        Button btnBlockMerage;
        AnalyzeClass Analyze_PG_Default = null;

        Button btnAutoRectPosition;
        Button btnAutoAlinameIndex;
        Button btnOnekeyDelAllRegion;

        Mover myMoverCollection = new Mover();

        Bitmap bmpBlockBase = new Bitmap(1, 1);
        Bitmap bmpBlockOperate = new Bitmap(1, 1);

        string m_ParaFileName = "aoipara.ini";

        public KBAOIForm(PageUI pageui)
        {
            InitializeComponent();
            InitiailInside();

            PAGEUI = pageui;
        }
        public KBAOIForm(PageUI pageui, EnvClass envClass)
        {
            InitializeComponent();
            InitiailInside();

            PAGEUI = pageui;

            ENVNow = envClass;
        }
        void InitiailInside()
        {
            numSimRatio = numericUpDown3;

            lblMessage = label1;

            IsMeasure = checkBox2;
            IsMask = checkBox1;
            IsTest = checkBox3;

            //IsRegionPara = checkBox5;
            IsTestPara = checkBox4;
            IsTest2_Relative = checkBox5;

            btnFindSimiliar = button2;
            btnFindSimiliar.Tag = TagEnum.FINDSIMILIAR;

            btnFindSimiliarCV = button14;
            btnFindSimiliarCV.Tag = TagEnum.FINDSIMILIAR2;

            numThreshold = numericUpDown4;
            numExtend = numericUpDown6;
            btnFindInside = button9;
            btnFindInside.Tag = TagEnum.FINDINSIDE;

            btnExit = button6;
            btnExit.Tag = TagEnum.EXIT;

            btnMarkTheSame = button3;
            btnMarkTheSame.Tag = TagEnum.MARKTHESAME;

            btnFindSimiliar.Click += BtnFindSimiliar_Click;
            btnFindInside.Click += BtnFindInside_Click;
            btnExit.Click += BtnExit_Click;
            btnMarkTheSame.Click += BtnMarkTheSame_Click;
            btnFindSimiliarCV.Click += BtnFindSimiliarCV_Click;

            this.TopMost = true;

            init_Display();
            m_PG = propertyGrid1;
            btnGetSelectImageBlock = button1;
            btnBlockAutoFind = button4;
            btnSetSelectBlock = button5;

            btnBlockAdd = button7;
            btnBlockDelete = button8;
            btnBlockMerage = button10;
            btnAutoRectPosition = button11;
            btnAutoAlinameIndex = button12;
            btnOnekeyDelAllRegion = button13;

            if (File.Exists(m_ParaFileName))
            {
                string dataStr = string.Empty;
                ReadData(ref dataStr, m_ParaFileName);
                BlockPara.FromingStr(dataStr);
            }

            m_PG.SelectedObject = BlockPara;
            btnGetSelectImageBlock.Click += BtnGetSelectImageBlock_Click;
            btnBlockAutoFind.Click += BtnBlockAutoFind_Click;
            btnSetSelectBlock.Click += BtnSetSelectBlock_Click;
            m_PG.PropertyValueChanged += M_PG_PropertyValueChanged;

            btnBlockAdd.Click += BtnBlockAdd_Click;
            btnBlockDelete.Click += BtnBlockDelete_Click;
            btnBlockMerage.Click += BtnBlockMerage_Click;
            btnAutoRectPosition.Click += BtnAutoRectPosition_Click;
            btnAutoAlinameIndex.Click += BtnAutoAlinameIndex_Click;
            btnOnekeyDelAllRegion.Click += BtnOnekeyDelAllRegion_Click;

            switch (Universal.VERSION)
            {
                case JetEazy.VersionEnum.ALLINONE:
                    switch (Universal.OPTION)
                    {
                        case JetEazy.OptionEnum.MAIN_SD:

                            groupBox4.Visible = false;
                            checkBox1.Visible = false;
                            checkBox2.Visible = false;
                            checkBox3.Visible = false;

                            button6.Location = new System.Drawing.Point(269, 169);
                            this.Size = new System.Drawing.Size(366, 254);

                            panel1.Visible = false;
                            DS.Visible = false;

                            break;
                        case JetEazy.OptionEnum.MAIN_SERVICE:
                        case JetEazy.OptionEnum.MAIN_X6:
                        case JetEazy.OptionEnum.MAIN_SDM1:
                        case JetEazy.OptionEnum.MAIN_SDM2:
                        case OptionEnum.MAIN_SDM3:
                            groupBox4.Visible = false;
                            checkBox1.Visible = false;
                            checkBox2.Visible = false;

                            btnAutoRectPosition.Visible = true;
                            btnAutoAlinameIndex.Visible = true;
                            btnOnekeyDelAllRegion.Visible = true;

                            //Analyze_PG_Default=new AnalyzeClass();
                            //Analyze_PG_Default.ALIGNPara.AlignMethod = AlignMethodEnum.AUFIND;
                            //Analyze_PG_Default.ALIGNPara.MTResolution = 0.044f;
                            //Analyze_PG_Default.ALIGNPara.Offset = 0.2f;
                            //Analyze_PG_Default.ALIGNPara.MTTolerance = 0.5f;

                            //Analyze_PG_Default.INSPECTIONPara.IBArea = 30;
                            //Analyze_PG_Default.INSPECTIONPara.IBTolerance = 35;
                            //Analyze_PG_Default.INSPECTIONPara.IBCount = 15;
                            //Analyze_PG_Default.INSPECTIONPara.InspectionMethod = InspectionMethodEnum.PIXEL;
                            //Analyze_PG_Default.INSPECTIONPara.InspectionAB = Inspection_A_B_Enum.ABPlus;

                            //Analyze_PG_Default.FromAssembleProperty();
                            //PG_Analyze.SelectedObject = Analyze_PG_Default.ASSEMBLE;


                            break;
                    }
                    break;
            }
        }

        //寻找相似 通过opencv寻找
        private void BtnFindSimiliarCV_Click(object sender, EventArgs e)
        {
            if (PAGEUI.IsPageSelectCorrect())
            {

                USEROPTIONFRM = new frmUserOption(false);
                if (USEROPTIONFRM.ShowDialog() == DialogResult.OK)
                {
                    m_stopwatch.Restart();
                    lblMessage.Text = "执行中...";
                    lblMessage.BackColor = Color.Yellow;
                    Application.DoEvents();

                    JzToolsClass.myShowCursor(0);

                    switch (Universal.PassOption)
                    {
                        case UserOptionEnum.ALL:
                            PAGEUI.FindSimilarEx((float)numSimRatio.Value / 100f);
                            switch (Universal.OPTION)
                            {
                                case JetEazy.OptionEnum.MAIN_SERVICE:
                                case JetEazy.OptionEnum.MAIN_X6:
                                case JetEazy.OptionEnum.MAIN_SD:
                                case JetEazy.OptionEnum.MAIN_SDM1:
                                case JetEazy.OptionEnum.MAIN_SDM2:
                                case OptionEnum.MAIN_SDM3:
                                    //全局寻找同层框

                                    FindSimilarEx((float)numSimRatio.Value / 100f);

                                    break;
                            }

                            break;
                        case UserOptionEnum.SIDE:
                            PAGEUI.FindSimilarEx((float)numSimRatio.Value / 100f);
                            break;
                    }

                    JzToolsClass.myShowCursor(1);

                    m_stopwatch.Stop();
                    lblMessage.Text = "完成，耗时 " + m_stopwatch.ElapsedMilliseconds.ToString() + " ms";
                    lblMessage.BackColor = Color.Green;

                }
            }
            else
            {
                MessageBox.Show("請選擇正確的檢測框。");
            }
        }

        private void BtnOnekeyDelAllRegion_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("是否要清除所有区域框？", "提示", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            //if (PAGEUI.IsPageSelectCorrect())
            {
                USEROPTIONFRM = new frmUserOption(false);
                if (USEROPTIONFRM.ShowDialog() == DialogResult.OK)
                {
                    m_stopwatch.Restart();
                    lblMessage.Text = "执行中...";
                    lblMessage.BackColor = Color.Yellow;
                    Application.DoEvents();

                    JzToolsClass.myShowCursor(0);

                    switch (Universal.PassOption)
                    {
                        case UserOptionEnum.ALL:
                            PAGEUI.DelAllRegion();
                            DelAllRegion();
                            //switch (Universal.OPTION)
                            //{
                            //    case JetEazy.OptionEnum.MAIN_SERVICE:
                            //    case JetEazy.OptionEnum.MAIN_X6:
                            //    case JetEazy.OptionEnum.MAIN_SD:
                            //    case JetEazy.OptionEnum.MAIN_SDM1:
                            //    case JetEazy.OptionEnum.MAIN_SDM2:
                            //        //全局寻找同层框

                            //        FindSimilar((float)numSimRatio.Value / 100f);

                            //        break;
                            //}

                            break;
                        case UserOptionEnum.SIDE:
                            PAGEUI.DelAllRegion();
                            //PAGEUI.FindSimilar((float)numSimRatio.Value / 100f);
                            break;
                    }

                    JzToolsClass.myShowCursor(1);

                    m_stopwatch.Stop();
                    lblMessage.Text = "完成，耗时 " + m_stopwatch.ElapsedMilliseconds.ToString() + " ms";
                    lblMessage.BackColor = Color.Green;

                }
            }
            //else
            //{
            //    MessageBox.Show("請選擇正確的檢測框。");
            //}
        }

        private void BtnAutoAlinameIndex_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否要自动编号？", "提示", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;


            m_stopwatch.Restart();
            lblMessage.Text = "执行中...";
            lblMessage.BackColor = Color.Yellow;
            Application.DoEvents();

            JzToolsClass.myShowCursor(0);

            _ax_AutoReportIndex();

            JzToolsClass.myShowCursor(1);

            m_stopwatch.Stop();
            lblMessage.Text = "完成，耗时 " + m_stopwatch.ElapsedMilliseconds.ToString() + " ms";
            lblMessage.BackColor = Color.Green;

        }

        private void BtnAutoRectPosition_Click(object sender, EventArgs e)
        {
            if (PAGEUI.IsPageSelectCorrect())
            {
                m_stopwatch.Restart();
                lblMessage.Text = "执行中...";
                lblMessage.BackColor = Color.Yellow;
                Application.DoEvents();

                JzToolsClass.myShowCursor(0);

                switch (Universal.OPTION)
                {
                    case JetEazy.OptionEnum.MAIN_SDM1:
                    case JetEazy.OptionEnum.MAIN_SDM2:
                    case OptionEnum.MAIN_SDM3:
                        List<DoffsetClass> DoffsetList = new List<DoffsetClass>();
                        DoffsetList.Clear();

                        _ax_AutoRectPosition(DoffsetList);
                        PAGEUI.FindSimilar(DoffsetList);
                        //PAGEUI.FindSimilar((float)numSimRatio.Value / 100f);
                        //FindSimilar((float)numSimRatio.Value / 100f);

                        _ax_AutoRect(DoffsetList);

                        break;
                }

                JzToolsClass.myShowCursor(1);

                m_stopwatch.Stop();
                lblMessage.Text = "完成，耗时 " + m_stopwatch.ElapsedMilliseconds.ToString() + " ms";
                lblMessage.BackColor = Color.Green;
            }
            else
            {
                MessageBox.Show("請選擇正確的檢測框。");
            }
        }

        private void BtnBlockMerage_Click(object sender, EventArgs e)
        {
            //if (MessageBox.Show("是否要合并选择框？", "提示", MessageBoxButtons.YesNo) != DialogResult.Yes)
            //    return;

            List<BlockItemClass> FoundBlockList = new List<BlockItemClass>();
            RectangleF rectangleF1 = new RectangleF();
            bool bMerage = false;
            int i = 0;
            while (i < myMoverCollection.Count)
            {
                BlockItemClass item = new BlockItemClass();
                GraphicalObject grobj = myMoverCollection[i].Source;

                if (!(grobj as JzRectEAG).IsSelected)
                {
                    RectangleF rectangleF = (grobj as JzRectEAG).RealRectangleAround(0, 0);
                    item.myrect = new Rectangle((int)rectangleF.X, (int)rectangleF.Y, (int)rectangleF.Width, (int)rectangleF.Height);
                    item.Index = i;
                    FoundBlockList.Add(item);
                }
                else
                {
                    RectangleF rectangleF = (grobj as JzRectEAG).RealRectangleAround(0, 0);
                    rectangleF1 = MergeTwoRects(rectangleF1, rectangleF);
                    bMerage = true;
                }

                i++;
            }

            DS.ClearMover();
            myMoverCollection.Clear();
            if (bMerage)
            {
                JzRectEAG RatioRectEAG = new JzRectEAG(Color.FromArgb(0, Color.Red), rectangleF1);
                RatioRectEAG.RelateNo = 2;
                myMoverCollection.Add(RatioRectEAG);
            }

            #region 相交的个数

            List<Rectangle> FoundRectList = new List<Rectangle>();
            List<string> CheckList = new List<string>();
            int j = 0;

            i = 0;

            foreach (BlockItemClass keyassign in FoundBlockList)
            {
                CheckList.Add(keyassign.ReportIndex.ToString("0000") + "," + i.ToString());
                i++;
            }
            CheckList.Sort();

            foreach (string Str in CheckList)
            {
                string[] Strs = Str.Split(',');
                FoundRectList.Add(FoundBlockList[int.Parse(Strs[1])].myrect);
            }



            #endregion

            //图像高度的几分之几
            double iheightRatio = BlockPara.inflant * 1.0 / 100 * bmpBlockBase.Height;


            #region 这个不用了

            i = 0;
            j = 0;

            while (i < FoundRectList.Count - 1)
            {
                if (FoundRectList[i].Width == 0)
                {
                    i++;
                    continue;
                }

                j = i + 1;

                while (j < FoundRectList.Count)
                {
                    if (FoundRectList[j].Width == 0)
                    {
                        j++;
                        continue;
                    }

                    Rectangle recti = FoundRectList[i];
                    Rectangle rectj = FoundRectList[j];

                    if (JzTool.IsInRange(recti.X, rectj.X, 10))
                    //if (recti.IntersectsWith(rectj))
                    {
                        rectj = MergeTwoRects(recti, rectj);

                        if (rectj.Height < iheightRatio)
                        {
                            recti = new Rectangle(0, 0, 0, 0);

                            FoundRectList.RemoveAt(i);
                            FoundRectList.Insert(i, recti);

                            FoundRectList.RemoveAt(j);
                            FoundRectList.Insert(j, rectj);

                            break;
                        }
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

                    FoundRectList.RemoveAt(i);
                    FoundRectList.Insert(i, recti);
                }
                i--;
            }

            #endregion

            foreach (Rectangle rect in FoundRectList)
            {
                //rect.Intersect(JzTool.SimpleRect(bmpThreshold.Size));

                JzRectEAG RatioRectEAG = new JzRectEAG(Color.FromArgb(0, Color.Red), rect);
                RatioRectEAG.RelateNo = 2;
                myMoverCollection.Add(RatioRectEAG);
            }

            //foreach (BlockItemClass item in FoundBlockList)
            //{
            //    JzRectEAG RatioRectEAG = new JzRectEAG(Color.FromArgb(0, Color.Red), item.myrect);
            //    RatioRectEAG.RelateNo = 2;
            //    myMoverCollection.Add(RatioRectEAG);
            //}

            DS.SetMover(myMoverCollection);
            DS.RefreshDisplayShape();
            DS.ReDraw();

        }

        private void BtnBlockDelete_Click(object sender, EventArgs e)
        {
            //if (MessageBox.Show("是否要删除选择框？", "提示", MessageBoxButtons.YesNo) != DialogResult.Yes)
            //    return;

            List<BlockItemClass> FoundBlockList = new List<BlockItemClass>();
            int i = 0;
            while (i < myMoverCollection.Count)
            {
                GraphicalObject grobj = myMoverCollection[i].Source;

                if (!(grobj as JzRectEAG).IsSelected)
                {
                    BlockItemClass item = new BlockItemClass();
                    RectangleF rectangleF = (grobj as JzRectEAG).RealRectangleAround(0, 0);
                    item.myrect = new Rectangle((int)rectangleF.X, (int)rectangleF.Y, (int)rectangleF.Width, (int)rectangleF.Height);
                    item.Index = i;
                    FoundBlockList.Add(item);
                }

                i++;
            }
            DS.ClearMover();
            myMoverCollection.Clear();
            foreach (BlockItemClass item in FoundBlockList)
            {
                JzRectEAG RatioRectEAG = new JzRectEAG(Color.FromArgb(0, Color.Red), item.myrect);
                RatioRectEAG.RelateNo = 2;
                myMoverCollection.Add(RatioRectEAG);
            }

            DS.SetMover(myMoverCollection);
            DS.RefreshDisplayShape();
            DS.ReDraw();

        }

        private void BtnBlockAdd_Click(object sender, EventArgs e)
        {

            //if (MessageBox.Show("是否要添加一个框？", "提示", MessageBoxButtons.YesNo) != DialogResult.Yes)
            //    return;

            Rectangle rectangle = new Rectangle(0, 0, 50, 50);
            if (myMoverCollection.Count == 0)
            {
                JzRectEAG RatioRectEAG = new JzRectEAG(Color.FromArgb(0, Color.Red), rectangle);
                RatioRectEAG.RelateNo = 2;
                myMoverCollection.Add(RatioRectEAG);
            }
            else
            {
                GraphicalObject grobj = myMoverCollection[myMoverCollection.Count - 1].Source;

                JzRectEAG RatioRectEAG = new JzRectEAG(Color.FromArgb(0, Color.Red), (grobj as JzRectEAG).RealRectangleAround(0, 0));
                RatioRectEAG.RelateNo = 2;
                RatioRectEAG.SetOffset(new Point(20, 20));

                myMoverCollection.Add(RatioRectEAG);
            }

            DS.SetMover(myMoverCollection);
            DS.RefreshDisplayShape();
            DS.ReDraw();
        }

        private void BtnSetSelectBlock_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("是否要写回资料？", "提示", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            AnalyzeClass analyze = PAGEUI.AnalyzeSelectNow;
            if (analyze == null)
                return;

            if (string.IsNullOrEmpty(strAnalyzeCurrentBlock))
                return;

            if (strAnalyzeCurrentBlock == analyze.ToAnalyzeString())
            {
                List<Rectangle> foundrectlist = new List<Rectangle>();
                RectangleF myRect = analyze.GetMyMoverRectF();
                List<BlockItemClass> FoundBlockList = new List<BlockItemClass>();

                int i = 0;
                while (i < myMoverCollection.Count)
                {
                    GraphicalObject grobj = myMoverCollection[i].Source;

                    RectangleF rectangleF = (grobj as JzRectEAG).RealRectangleAround(0, 0);
                    Rectangle rect = new Rectangle((int)rectangleF.X, (int)rectangleF.Y, (int)rectangleF.Width, (int)rectangleF.Height);

                    rect.X += (int)myRect.X;
                    rect.Y += (int)myRect.Y;

                    BlockItemClass blockItem = new BlockItemClass();
                    blockItem.myrect = rect;
                    FoundBlockList.Add(blockItem);

                    //foundrectlist.Add(rect);

                    i++;
                }


                i = 0;
                int j = 0;
                int k = 0;

                #region 排序

                int Highest = 100000;
                int HighestIndex = -1;
                int ReportIndex = 0;
                List<string> CheckList = new List<string>();

                //Clear All Index To 0 and Check the Highest

                foreach (BlockItemClass keyassign in FoundBlockList)
                {
                    keyassign.ReportIndex = 0;
                    ReportIndex = 1;
                }

                i = 0;
                while (true)
                {
                    i = 0;
                    Highest = 100000;
                    HighestIndex = -1;
                    foreach (BlockItemClass keyassign in FoundBlockList)
                    {
                        if (keyassign.ReportIndex == 0)
                        {
                            if (keyassign.myrect.X < Highest)
                            {
                                Highest = keyassign.myrect.X;
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
                    k = 0;
                    foreach (BlockItemClass keyassign in FoundBlockList)
                    {
                        if (keyassign.ReportIndex == 0)
                        {
                            if (JzTool.IsInRange(keyassign.myrect.X, Highest, 10))
                            {
                                CheckList.Add(keyassign.myrect.Y.ToString("0000") + "," + i.ToString());

                                //rectk = MergeTwoRects(rectk, keyassign.myrect);
                                //k++;
                                //if (k == BlockPara.inflant)
                                //{
                                //    FoundRectList.Add(rectk);
                                //    rectk = new Rectangle(0, 0, 0, 0);
                                //    k = 0;
                                //}

                            }
                        }
                        i++;
                    }

                    CheckList.Sort();

                    foreach (string Str in CheckList)
                    {
                        string[] Strs = Str.Split(',');
                        FoundBlockList[int.Parse(Strs[1])].ReportIndex = ReportIndex;
                        ReportIndex++;
                    }
                }

                #endregion

                i = 0;
                CheckList.Clear();
                foreach (BlockItemClass keyassign in FoundBlockList)
                {
                    CheckList.Add(keyassign.ReportIndex.ToString("0000") + "," + i.ToString());
                    i++;
                }
                CheckList.Sort();

                foreach (string Str in CheckList)
                {
                    string[] Strs = Str.Split(',');
                    foundrectlist.Add(FoundBlockList[int.Parse(Strs[1])].myrect);
                }


                PAGEUI.FindInside(foundrectlist, Analyze_PG_Default);
            }
            else
            {
                MessageBox.Show("当前选定的框与资料框不是同一个框，请检查。");
            }
        }


        class BlockItemClass
        {
            public int Index = 0;
            public string Name = String.Empty;
            public Rectangle myrect = new Rectangle();
            public int ReportIndex = 0;
        }

        //自動尋找blob 並排序
        private void BtnBlockAutoFind_Click(object sender, EventArgs e)
        {

            if (bmpBlockOperate == null)
                return;
            if (bmpBlockOperate.Width <= 1 || bmpBlockOperate.Height <= 1)
            {
                return;
            }

            myMoverCollection.Clear();
            DS.ClearMover();

            HistogramClass histogram = new HistogramClass(2);
            JzFindObjectClass jzFind = new JzFindObjectClass();

            Bitmap bmpblockrun = AForge.Imaging.Image.Clone(bmpBlockOperate, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            //Erosion erosion = new Erosion();
            //Bitmap bmpDilata = erosion.Apply(bmpblockrun);

            //Subtract subtract = new Subtract(bmpDilata);
            //Bitmap bmpSub = subtract.Apply(bmpblockrun);

            Grayscale grayscale = new Grayscale(0.299, 0.587, 0.114);
            Bitmap bmpgray = grayscale.Apply(bmpblockrun);
            //if (BlockPara.threshlod > 0)
            //{
            //    AForge.Imaging.Filters.Threshold threshold = new AForge.Imaging.Filters.Threshold(BlockPara.threshlod);
            //    bmpgray = threshold.Apply(bmpgray);
            //}

            OtsuThreshold otsuthreshold = new OtsuThreshold();
            Bitmap bmpotsuThreshold = otsuthreshold.Apply(bmpgray);

            //histogram.GetHistogram(bmpSub);

            //AForge.Imaging.Filters.Threshold AforThreshold = new AForge.Imaging.Filters.Threshold(BlockPara.threshlod);
            ////AForge.Imaging.Filters.Threshold AforThreshold = new AForge.Imaging.Filters.Threshold(histogram.MeanGrade);
            //Bitmap bmpThreshold = AforThreshold.Apply(bmpgray);

            //bmpotsuThreshold.Save("D://LOA//Aforge//AforgeGray.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

            //jzFind.SetThreshold(bmpblockrun, JzTool.SimpleRect(bmpThreshold.Size), 188, 255, 0, true);
            jzFind.SetThreshold(bmpotsuThreshold, JzTool.SimpleRect(bmpotsuThreshold.Size), 188, 255, 0, true);
            jzFind.Find(bmpotsuThreshold, Color.Red);

            List<Rectangle> FoundRectList = new List<Rectangle>();
            List<BlockItemClass> FoundBlockList = new List<BlockItemClass>();
            FoundBlockList.Clear();
            FoundRectList.Clear();

            foreach (FoundClass found in jzFind.FoundList)
            {
                Rectangle rect = found.rect;
                rect.Inflate(BlockPara.extend, BlockPara.extend);
                rect.Intersect(JzTool.SimpleRect(bmpotsuThreshold.Size));

                if (found.Area > BlockPara.minarea)
                {

                    //FoundRectList.Add(rect);

                    BlockItemClass blockItem = new BlockItemClass();
                    blockItem.myrect = rect;
                    FoundBlockList.Add(blockItem);
                }

            }


            int i = 0;
            int j = 0;
            int k = 0;
            Rectangle rectk = new Rectangle(0, 0, 0, 0);

            #region 排序

            int Highest = 100000;
            int HighestIndex = -1;
            int ReportIndex = 0;
            List<string> CheckList = new List<string>();

            //Clear All Index To 0 and Check the Highest

            foreach (BlockItemClass keyassign in FoundBlockList)
            {
                keyassign.ReportIndex = 0;
                ReportIndex = 1;
            }

            i = 0;
            while (true)
            {
                i = 0;
                Highest = 100000;
                HighestIndex = -1;
                foreach (BlockItemClass keyassign in FoundBlockList)
                {
                    if (keyassign.ReportIndex == 0)
                    {
                        if (keyassign.myrect.X < Highest)
                        {
                            Highest = keyassign.myrect.X;
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
                k = 0;
                foreach (BlockItemClass keyassign in FoundBlockList)
                {
                    if (keyassign.ReportIndex == 0)
                    {
                        if (JzTool.IsInRange(keyassign.myrect.X, Highest, 10))
                        {
                            CheckList.Add(keyassign.myrect.Y.ToString("0000") + "," + i.ToString());

                            //rectk = MergeTwoRects(rectk, keyassign.myrect);
                            //k++;
                            //if (k == BlockPara.inflant)
                            //{
                            //    FoundRectList.Add(rectk);
                            //    rectk = new Rectangle(0, 0, 0, 0);
                            //    k = 0;
                            //}

                        }
                    }
                    i++;
                }

                CheckList.Sort();

                foreach (string Str in CheckList)
                {
                    string[] Strs = Str.Split(',');
                    FoundBlockList[int.Parse(Strs[1])].ReportIndex = ReportIndex;
                    ReportIndex++;
                }
            }

            #endregion

            //foreach (BlockItemClass keyassign in FoundBlockList)
            //{
            //    DrawRect(bmpblockrun, keyassign.myrect, new Pen(Color.Lime, 1));
            //    JzTool.DrawText(bmpblockrun, keyassign.ReportIndex.ToString(), keyassign.myrect.Location, 5, Color.Red);
            //}
            //bmpblockrun.Save("D://LOA//Aforge//bmpblockrun.bmp", System.Drawing.Imaging.ImageFormat.Bmp);


            #region 相交的个数

            i = 0;

            foreach (BlockItemClass keyassign in FoundBlockList)
            {
                CheckList.Add(keyassign.ReportIndex.ToString("0000") + "," + i.ToString());
                i++;
            }
            CheckList.Sort();

            foreach (string Str in CheckList)
            {
                string[] Strs = Str.Split(',');
                FoundRectList.Add(FoundBlockList[int.Parse(Strs[1])].myrect);
            }



            #endregion

            //图像高度的几分之几
            double iheightRatio = BlockPara.inflant * 1.0 / 100 * bmpblockrun.Height;

            if (BlockPara.blockdir == BlockDir.VERTICAL)
            {
                #region 找纵向
                //#if FIND_H
                i = 0;
                j = 0;

                while (i < FoundRectList.Count - 1)
                {
                    if (FoundRectList[i].Width == 0)
                    {
                        i++;
                        continue;
                    }

                    j = i + 1;

                    while (j < FoundRectList.Count)
                    {
                        if (FoundRectList[j].Width == 0)
                        {
                            j++;
                            continue;
                        }

                        Rectangle recti = FoundRectList[i];
                        Rectangle rectj = FoundRectList[j];

                        if (JzTool.IsInRange(recti.X, rectj.X, 10))
                        //if (recti.IntersectsWith(rectj))
                        {
                            rectj = MergeTwoRects(recti, rectj);

                            if (rectj.Height < iheightRatio)
                            {
                                recti = new Rectangle(0, 0, 0, 0);

                                FoundRectList.RemoveAt(i);
                                FoundRectList.Insert(i, recti);

                                FoundRectList.RemoveAt(j);
                                FoundRectList.Insert(j, rectj);

                                break;
                            }
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

                        FoundRectList.RemoveAt(i);
                        FoundRectList.Insert(i, recti);
                    }
                    i--;
                }

                //#endif

                #endregion
            }
            else
            {
                #region 找横向

                i = 0;
                j = 0;

                while (i < FoundRectList.Count - 1)
                {
                    if (FoundRectList[i].Height == 0)
                    {
                        i++;
                        continue;
                    }

                    j = i + 1;

                    while (j < FoundRectList.Count)
                    {
                        if (FoundRectList[j].Height == 0)
                        {
                            j++;
                            continue;
                        }

                        Rectangle recti = FoundRectList[i];
                        Rectangle rectj = FoundRectList[j];

                        if (JzTool.IsInRange(recti.Y, rectj.Y, 10))
                        //if (recti.IntersectsWith(rectj))
                        {
                            rectj = MergeTwoRects(recti, rectj);

                            if (rectj.Width < iheightRatio)
                            {
                                recti = new Rectangle(0, 0, 0, 0);

                                FoundRectList.RemoveAt(i);
                                FoundRectList.Insert(i, recti);

                                FoundRectList.RemoveAt(j);
                                FoundRectList.Insert(j, rectj);

                                break;
                            }
                        }

                        j++;
                    }
                    i++;
                }

                i = FoundRectList.Count - 1;

                while (i > -1)
                {
                    if (FoundRectList[i].Height == 0)
                        FoundRectList.RemoveAt(i);
                    else
                    {
                        Rectangle recti = FoundRectList[i];

                        FoundRectList.RemoveAt(i);
                        FoundRectList.Insert(i, recti);
                    }
                    i--;
                }

                #endregion
            }

            foreach (Rectangle rect in FoundRectList)
            {
                rect.Intersect(JzTool.SimpleRect(bmpotsuThreshold.Size));

                JzRectEAG RatioRectEAG = new JzRectEAG(Color.FromArgb(0, Color.Red), rect);
                RatioRectEAG.RelateNo = 2;
                myMoverCollection.Add(RatioRectEAG);
            }


            DS.SetMover(myMoverCollection);
            DS.RefreshDisplayShape();
            DS.ReDraw();
        }

        void DrawRect(Bitmap bmp, Rectangle rect, Pen roundpen)
        {
            Graphics g = Graphics.FromImage(bmp);
            g.DrawRectangle(roundpen, rect);
            g.Dispose();
        }

        JzFindBlockPropertyGridClass BlockPara = new JzFindBlockPropertyGridClass();

        private void M_PG_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            switch (e.ChangedItem.Label)
            {
                case "brightness":
                    BlockPara.brightness = (int)e.ChangedItem.Value;
                    break;
                case "contrast":
                    BlockPara.contrast = (int)e.ChangedItem.Value;
                    break;
                case "threshlod":
                    BlockPara.threshlod = (int)e.ChangedItem.Value;
                    break;
                case "minarea":
                    BlockPara.minarea = (int)e.ChangedItem.Value;
                    break;
                case "extend":
                    BlockPara.extend = (int)e.ChangedItem.Value;
                    break;
                case "inflant":
                    BlockPara.inflant = (int)e.ChangedItem.Value;
                    break;
                case "blockdir":
                    BlockPara.blockdir = (BlockDir)e.ChangedItem.Value;
                    break;
            }

            FillBlockDisplay();

            btnBlockAutoFind.PerformClick();
        }

        private void BtnGetSelectImageBlock_Click(object sender, EventArgs e)
        {
            Bitmap bmpPageOrg = PAGEUI.PageNow.GetbmpORG((PageOPTypeEnum)PAGEUI.PageNow.PageOPTypeIndex);
            AnalyzeClass analyze = PAGEUI.AnalyzeSelectNow;
            strAnalyzeCurrentBlock = string.Empty;

            if (analyze == null)
                return;

            strAnalyzeCurrentBlock = analyze.ToAnalyzeString();

            RectangleF myRect = analyze.GetMyMoverRectF();
            bmpBlockBase.Dispose();
            bmpBlockBase = bmpPageOrg.Clone(myRect, System.Drawing.Imaging.PixelFormat.Format24bppRgb);


            FillBlockDisplay();

            //順便自動尋找
            btnBlockAutoFind.PerformClick();


            //Analyze_PG_Default = analyze;
            //Analyze_PG_Default.ALIGNPara.AlignMethod = AlignMethodEnum.AUFIND;
            //Analyze_PG_Default.ALIGNPara.MTResolution = 0.044f;
            //Analyze_PG_Default.ALIGNPara.Offset = 0.2f;
            //Analyze_PG_Default.ALIGNPara.MTTolerance = 0.5f;

            //Analyze_PG_Default.INSPECTIONPara.IBArea = 30;
            //Analyze_PG_Default.INSPECTIONPara.IBTolerance = 35;
            //Analyze_PG_Default.INSPECTIONPara.IBCount = 15;
            //Analyze_PG_Default.INSPECTIONPara.InspectionMethod = InspectionMethodEnum.PIXEL;
            //Analyze_PG_Default.INSPECTIONPara.InspectionAB = Inspection_A_B_Enum.ABPlus;

            //Analyze_PG_Default.ToAssembleProperty();
            ////Analyze_PG_Default.FromAssembleProperty();
            ////Analyze_PG_Default.ASSEMBLE.ConstructProperty(JetEazy.VersionEnum.ALLINONE, JetEazy.OptionEnum.MAIN_X6);
            //PG_Analyze.SelectedObject = Analyze_PG_Default.ASSEMBLE;

        }

        void FillBlockDisplay()
        {
            bmpBlockOperate.Dispose();
            bmpBlockOperate = new Bitmap(bmpBlockBase);

            JzTool.SetBrightContrast(bmpBlockOperate, BlockPara.brightness, BlockPara.contrast);

            DS.SetDisplayImage(bmpBlockOperate);
        }

        //同位 資料
        private void BtnMarkTheSame_Click(object sender, EventArgs e)
        {
            if (PAGEUI.IsPageSelectCorrect())
            {
                PAGEUI.SetBlockPara(BlockPara);
                USEROPTIONFRM = new frmUserOption(false);
                if (USEROPTIONFRM.ShowDialog() == DialogResult.OK)
                {
                    m_stopwatch.Restart();
                    lblMessage.Text = "执行中...";
                    lblMessage.BackColor = Color.Yellow;
                    Application.DoEvents();

                    JzToolsClass.myShowCursor(0);

                    if (IsTest.Checked)
                    {
                        //同位檢測框的位置
                        switch (Universal.PassOption)
                        {
                            case UserOptionEnum.ALL:
                                PAGEUI.FindMarkSame();
                                switch (Universal.OPTION)
                                {
                                    case JetEazy.OptionEnum.MAIN_SERVICE:
                                    case JetEazy.OptionEnum.MAIN_X6:
                                    case JetEazy.OptionEnum.MAIN_SDM1:
                                    case JetEazy.OptionEnum.MAIN_SDM2:
                                    case OptionEnum.MAIN_SDM3:
                                        FindMarkSame(PAGEUI.ShiftPTORG);
                                        break;
                                }

                                break;
                            case UserOptionEnum.SIDE:
                                PAGEUI.FindMarkSame();
                                break;
                        }
                    }
                    else if (IsTestPara.Checked)
                    {
                        //同位資料參數
                        switch (Universal.PassOption)
                        {
                            case UserOptionEnum.ALL:
                                PAGEUI.FindMarkSamePara();
                                switch (Universal.OPTION)
                                {
                                    case JetEazy.OptionEnum.MAIN_SERVICE:
                                    case JetEazy.OptionEnum.MAIN_X6:
                                    case JetEazy.OptionEnum.MAIN_SDM1:
                                    case JetEazy.OptionEnum.MAIN_SDM2:
                                    case OptionEnum.MAIN_SDM3:
                                        FindMarkSameALLPara();
                                        break;
                                }

                                break;
                            case UserOptionEnum.SIDE:
                                PAGEUI.FindMarkSamePara();
                                break;
                        }
                    }
                    else if (IsTest2_Relative.Checked)
                    {
                        //同位檢測框的位置
                        switch (Universal.PassOption)
                        {
                            case UserOptionEnum.ALL:
                                PAGEUI.FindMarkSame2();
                                switch (Universal.OPTION)
                                {
                                    case JetEazy.OptionEnum.MAIN_SERVICE:
                                    case JetEazy.OptionEnum.MAIN_X6:
                                    case JetEazy.OptionEnum.MAIN_SDM1:
                                    case JetEazy.OptionEnum.MAIN_SDM2:
                                    case OptionEnum.MAIN_SDM3:
                                        FindMarkSame2(PAGEUI.ShiftPTORG);
                                        break;
                                }

                                break;
                            case UserOptionEnum.SIDE:
                                PAGEUI.FindMarkSame2();
                                break;
                        }
                    }

                    JzToolsClass.myShowCursor(1);

                    m_stopwatch.Stop();
                    lblMessage.Text = "完成，耗时 " + m_stopwatch.ElapsedMilliseconds.ToString() + " ms";
                    lblMessage.BackColor = Color.Green;

                }
            }
            else
            {
                MessageBox.Show("請選擇正確的檢測框。");
            }
        }

        //自動尋找内框參數
        private void BtnFindInside_Click(object sender, EventArgs e)
        {
            if (PAGEUI.IsPageSelectCorrect())
            {
                JzToolsClass.myShowCursor(0);
                PAGEUI.FindInside((float)numThreshold.Value / 100f, (int)numExtend.Value);
                JzToolsClass.myShowCursor(1);
            }
            else
            {
                MessageBox.Show("請選擇需要自動添加項目的檢測框。");
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            SaveData(BlockPara.ToParaString(), m_ParaFileName);

            this.Close();
        }

        frmUserOption USEROPTIONFRM;
        System.Diagnostics.Stopwatch m_stopwatch = new System.Diagnostics.Stopwatch();

        //尋找相似 區域
        private void BtnFindSimiliar_Click(object sender, EventArgs e)
        {
            if (PAGEUI.IsPageSelectCorrect())
            {

                USEROPTIONFRM = new frmUserOption(false);
                if (USEROPTIONFRM.ShowDialog() == DialogResult.OK)
                {
                    m_stopwatch.Restart();
                    lblMessage.Text = "执行中...";
                    lblMessage.BackColor = Color.Yellow;
                    Application.DoEvents();

                    JzToolsClass.myShowCursor(0);

                    switch (Universal.PassOption)
                    {
                        case UserOptionEnum.ALL:
                            PAGEUI.FindSimilar((float)numSimRatio.Value / 100f);
                            switch (Universal.OPTION)
                            {
                                case JetEazy.OptionEnum.MAIN_SERVICE:
                                case JetEazy.OptionEnum.MAIN_X6:
                                case JetEazy.OptionEnum.MAIN_SD:
                                case JetEazy.OptionEnum.MAIN_SDM1:
                                case JetEazy.OptionEnum.MAIN_SDM2:
                                case OptionEnum.MAIN_SDM3:
                                    //全局寻找同层框

                                    FindSimilar((float)numSimRatio.Value / 100f);

                                    break;
                            }

                            break;
                        case UserOptionEnum.SIDE:
                            PAGEUI.FindSimilar((float)numSimRatio.Value / 100f);
                            break;
                    }

                    JzToolsClass.myShowCursor(1);

                    m_stopwatch.Stop();
                    lblMessage.Text = "完成，耗时 " + m_stopwatch.ElapsedMilliseconds.ToString() + " ms";
                    lblMessage.BackColor = Color.Green;

                }
            }
            else
            {
                MessageBox.Show("請選擇正確的檢測框。");
            }

        }


        /// <summary>
        /// 將樹狀結構轉換成明細模式
        /// </summary>
        void ConverAnalyzeToList(AnalyzeClass analyze)
        {
            RawAnalyzeList.Clear();
            analyze.FillToList(RawAnalyzeList);
        }

        List<AnalyzeClass> RawAnalyzeList = new List<AnalyzeClass>();

        AnalyzeClass GetAnalyzeFromTree(int no)
        {
            return RawAnalyzeList.Find(x => x.No == no);
        }

        /// <summary>
        /// 尋找最大的編號
        /// </summary>
        /// <returns></returns>
        int GetMaxNewNoFromRawAnalyzeList()
        {
            int max = -1000;

            if (false)
            {
                max = AnalyzeClass.LearnMaxNo;

                AnalyzeClass.LearnMaxNo++;
            }
            else
            {
                foreach (AnalyzeClass analyze in RawAnalyzeList)
                {
                    if (max < analyze.No)
                    {
                        max = analyze.No;
                    }
                }
            }

            max++;
            return max;
        }

        public void FindSimilar(float tolerance)
        {

            List<DoffsetClass> DoffsetList = new List<DoffsetClass>();

            //Get the Anallyze Data For Find
            Bitmap bmpPageOrg = PAGEUI.PageNow.GetbmpORG((PageOPTypeEnum)PAGEUI.PageNow.PageOPTypeIndex);

            //AnalyzeSelectNow.IsTempSave = true;

            ////先把 Analyze Train 完
            //PAGEUI.AnalyzeSelectNow.A02_CreateTrainRequirement(bmpPageOrg, new PointF(0, 0));
            //PAGEUI.AnalyzeSelectNow.A05_AlignTrainProcess();

            foreach (PageClass page in ENVNow.PageList)
            {
                DoffsetList.Clear();

                if (page.No == PAGEUI.PageNow.No)
                    continue;


                switch (Universal.OPTION)
                {
                    case JetEazy.OptionEnum.MAIN_SERVICE:
                    case JetEazy.OptionEnum.MAIN_X6:
                    case JetEazy.OptionEnum.MAIN_SD:
                    case JetEazy.OptionEnum.MAIN_SDM1:
                    case JetEazy.OptionEnum.MAIN_SDM2:
                    case OptionEnum.MAIN_SDM3:
                        //清除外框 重新加入新的
                        page.AnalyzeRoot.BranchList.Clear();

                        break;
                }

                Bitmap bmpPageOrgtmp = page.GetbmpORG((PageOPTypeEnum)page.PageOPTypeIndex);
                PAGEUI.AnalyzeSelectNow.B08_RunAndFindSimilar(bmpPageOrgtmp, tolerance, DoffsetList);


                //AnalyzeSelectNow.IsTempSave = false;

                //取得自身的區域
                int selectno = PAGEUI.AnalyzeSelectNow.No;
                RectangleF myRectF = PAGEUI.AnalyzeSelectNow.GetMoverRectF(bmpPageOrg);
                List<RectangleF> OrgRectFList = new List<RectangleF>();

                ConverAnalyzeToList(page.AnalyzeRoot);

                //取得所有定義過的區域
                foreach (AnalyzeClass analyze in page.AnalyzeRoot.BranchList)
                {
                    if (analyze.No != 1)
                    {
                        OrgRectFList.Add(analyze.GetMoverRectF(bmpPageOrgtmp));
                    }
                }

                bool IsIncluded = false;

                //Check Duplicate and Copy Ananlyze
                foreach (DoffsetClass doffset in DoffsetList)
                {
                    IsIncluded = false;

                    RectangleF foundrectf = OffsetRect(myRectF, doffset.OffsetF);

                    //檢查是否有和現有的Analyze重覆超過30%的，有的話視為同一個就不加了
                    foreach (RectangleF rectf in OrgRectFList)
                    {
                        RectangleF rectfintersect = foundrectf;
                        rectfintersect.Intersect(rectf);

                        if ((float)(rectfintersect.Width * rectfintersect.Height) / (float)(rectf.Width * rectf.Height) > 0.3)
                        {
                            IsIncluded = true;
                            break;
                        }
                    }

                    //检查是否在大定位框里 不在的话 不要了

                    if (!IsIncluded)
                    {
                        RectangleF rectfintersectORG = foundrectf;
                        rectfintersectORG.Intersect(page.AnalyzeRoot.GetMoverRectF(bmpPageOrgtmp));

                        if ((float)(rectfintersectORG.Width * rectfintersectORG.Height) / (float)(foundrectf.Width * foundrectf.Height) < 0.8)
                        {
                            IsIncluded = true;
                        }
                    }

                    //若沒有被包含則加入框裏
                    if (!IsIncluded)
                        AddSameLevel(page.No, doffset, new PointF(myRectF.X + myRectF.Width / 2, myRectF.Y + myRectF.Height / 2));
                }

            }
        }
        public void FindSimilarEx(float tolerance)
        {
            //Get the Anallyze Data For Find
            //Bitmap bmpPageOrg = PAGEUI.PageNow.GetbmpORG((PageOPTypeEnum)PAGEUI.PageNow.PageOPTypeIndex);

            //AnalyzeSelectNow.IsTempSave = true;

            ////先把 Analyze Train 完
            //PAGEUI.AnalyzeSelectNow.A02_CreateTrainRequirement(bmpPageOrg, new PointF(0, 0));
            //PAGEUI.AnalyzeSelectNow.A05_AlignTrainProcess();
            RectangleF[] rectangleFs = new RectangleF[ENVNow.PageList.Count];
            Bitmap[] bitmaps = new Bitmap[ENVNow.PageList.Count];

            RectangleF[] rectangleFroots = new RectangleF[ENVNow.PageList.Count];
            Bitmap[] bitmaproots = new Bitmap[ENVNow.PageList.Count];

            int i = 0;
            while (i < ENVNow.PageList.Count)
            {
                rectangleFs[i] = new RectangleF(PAGEUI.RectClone.X,
                                                                                 PAGEUI.RectClone.Y,
                                                                                 PAGEUI.RectClone.Width,
                                                                                 PAGEUI.RectClone.Height);

                bitmaproots[i] = new Bitmap(ENVNow.PageList[i].GetbmpORG((PageOPTypeEnum)ENVNow.PageList[i].PageOPTypeIndex));
                RectangleF temprect = ENVNow.PageList[i].AnalyzeRoot.GetMoverRectF(bitmaproots[i]);
                rectangleFroots[i] = new RectangleF(temprect.X,
                                                                                 temprect.Y,
                                                                                 temprect.Width,
                                                                                 temprect.Height);

                bitmaps[i] = new Bitmap(PAGEUI.m_bmpPattern);
                i++;
            }

            //Bitmap bmppattern = new Bitmap(PAGEUI.m_bmpPattern);
            //i = 0;

            //Parallel.ForEach<>
            //Parallel.ForEach(ENVNow.PageList, (page) =>
            //{
            // 对集合中的每个元素item执行操作

            foreach (PageClass page in ENVNow.PageList)
            {
                List<DoffsetClass> DoffsetList = new List<DoffsetClass>();
                DoffsetList.Clear();

                if (page.No == PAGEUI.PageNow.No)
                    continue;

                //if (page.No == PAGEUI.PageNow.No)
                //    return;


                switch (Universal.OPTION)
                {
                    case JetEazy.OptionEnum.MAIN_SERVICE:
                    case JetEazy.OptionEnum.MAIN_X6:
                    case JetEazy.OptionEnum.MAIN_SD:
                    case JetEazy.OptionEnum.MAIN_SDM1:
                    case JetEazy.OptionEnum.MAIN_SDM2:
                    case OptionEnum.MAIN_SDM3:
                        //清除外框 重新加入新的
                        page.AnalyzeRoot.BranchList.Clear();

                        break;
                }

                //Bitmap bmpPageOrgtmp = page.GetbmpORG((PageOPTypeEnum)page.PageOPTypeIndex);
                //PAGEUI.AnalyzeSelectNow.B08_RunAndFindSimilar(bmpPageOrgtmp, tolerance, DoffsetList);

                OpencvMatchClass opencvMatch = new OpencvMatchClass();
                //m_bmpPattern.Dispose();
                //m_bmpPattern = bmpPageOrg.Clone(m_RectClone, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Bitmap bitmap = opencvMatch.Recoganize(bitmaproots[page.No], bitmaps[page.No], DoffsetList, tolerance, 1);
                //bitmap.Save($"D:\\_tmp\\bmpMatched_{page.No}.png", System.Drawing.Imaging.ImageFormat.Png);
                //AnalyzeSelectNow.IsTempSave = false;
                bitmap.Dispose();
                //bmppattern.Dispose();


                //AnalyzeSelectNow.IsTempSave = false;

                ////取得自身的區域
                //int selectno = PAGEUI.AnalyzeSelectNow.No;
                //RectangleF myRectF = PAGEUI.AnalyzeSelectNow.GetMoverRectF(bmpPageOrg);
                //List<RectangleF> OrgRectFList = new List<RectangleF>();
                RectangleF myRectF = rectangleFs[page.No];// new RectangleF(PAGEUI.RectClone.X,
                                                          //PAGEUI.RectClone.Y,
                                                          //PAGEUI.RectClone.Width,
                                                          //PAGEUI.RectClone.Height);

                ConverAnalyzeToList(page.AnalyzeRoot);

                ////取得所有定義過的區域
                //foreach (AnalyzeClass analyze in page.AnalyzeRoot.BranchList)
                //{
                //    if (analyze.No != 1)
                //    {
                //        OrgRectFList.Add(analyze.GetMoverRectF(bmpPageOrgtmp));
                //    }
                //}

                bool IsIncluded = false;

                //Check Duplicate and Copy Ananlyze
                foreach (DoffsetClass doffset in DoffsetList)
                {
                    IsIncluded = false;

                    RectangleF foundrectf = OffsetRect(myRectF, doffset.OffsetF);

                    ////檢查是否有和現有的Analyze重覆超過30%的，有的話視為同一個就不加了
                    //foreach (RectangleF rectf in OrgRectFList)
                    //{
                    //    RectangleF rectfintersect = foundrectf;
                    //    rectfintersect.Intersect(rectf);

                    //    if ((float)(rectfintersect.Width * rectfintersect.Height) / (float)(rectf.Width * rectf.Height) > 0.5)
                    //    {
                    //        IsIncluded = true;
                    //        break;
                    //    }
                    //}

                    //检查是否在大定位框里 不在的话 不要了

                    if (!IsIncluded)
                    {
                        RectangleF rectfintersectORG = foundrectf;
                        //rectfintersectORG.Intersect(page.AnalyzeRoot.GetMoverRectF(bmpPageOrgtmp));
                        rectfintersectORG.Intersect(rectangleFroots[page.No]);

                        if ((float)(rectfintersectORG.Width * rectfintersectORG.Height) / (float)(foundrectf.Width * foundrectf.Height) < 0.8)
                        {
                            IsIncluded = true;
                        }
                    }

                    //若沒有被包含則加入框裏
                    if (!IsIncluded)
                        AddSameLevel(page.No, doffset, new PointF(myRectF.X + myRectF.Width / 2, myRectF.Y + myRectF.Height / 2));
                }

            }
            //);

            i = 0;
            while (i < ENVNow.PageList.Count)
            {
                bitmaproots[i].Dispose();
                bitmaps[i].Dispose();

                i++;
            }
        }
        /// <summary>
        /// 自動新增同階資料
        /// </summary>
        public void AddSameLevel(int selectno, DoffsetClass doffset, PointF orgcenterf)
        {
            List<AnalyzeClass> sameaddedanalyzelist = new List<AnalyzeClass>();
            List<int> selectanalyzenolist = new List<int>();

            //if (FirstSelectNo > -1)
            {
                //List<int> selectnolist = CheckUISelect(FirstSelectNo);

                //foreach (int no in selectnolist)
                {
                    //    if (no == 1)    //排除ROOT
                    //        continue;

                    AnalyzeClass selectanalyze = PAGEUI.AnalyzeSelectNow;

                    PointF offsetptf = new PointF(doffset.OffsetF.X - orgcenterf.X, doffset.OffsetF.Y - orgcenterf.Y);

                    AnalyzeClass sameanalyze = selectanalyze.Clone(new Point((int)offsetptf.X, (int)offsetptf.Y), (double)doffset.Degree, true, false, false, false);

                    sameanalyze.No = GetMaxNewNoFromRawAnalyzeList();
                    sameanalyze.PageNo = selectno;
                    sameanalyze.AliasName = sameanalyze.ToAnalyzeString(selectno);
                    sameanalyze.RelateMover(sameanalyze.No, sameanalyze.Level);
                    sameanalyze.RelateASN = "None";
                    sameanalyze.RelateASNItem = "None";

                    sameanalyze.ToPassInfo();

                    //AddRowToAnalyzeTable(sameanalyze);
                    //sameanalyze.AddNewRowToDataTable(AnalyzeTable, IsLearn);
                    RawAnalyzeList.Add(sameanalyze);

                    //sameaddedanalyzelist.Add(sameanalyze);
                    //selectanalyzenolist.Add(sameanalyze.No);

                    AnalyzeClass parentanalyze = GetAnalyzeFromTree(sameanalyze.ParentNo);
                    if (parentanalyze != null)
                        parentanalyze.BranchList.Add(sameanalyze);
                }
                //FirstSelectNo = branchanalyze.No;

                //OnChangeMover(sameaddedanalyzelist, DBStatusEnum.ADD);

                //dtlvAnalyze.ExpandAll();

                //SetSelectFocus(selectanalyzenolist);
            }
            //else
            //    MessageBox.Show("請選擇需要新增同層的項目", "SYSTEM", MessageBoxButtons.OK);
        }

        RectangleF OffsetRect(RectangleF orgrectf, PointF centerptf)
        {
            RectangleF rectf = orgrectf;

            rectf.Location = centerptf;

            rectf.X -= rectf.Width / 2;
            rectf.Y -= rectf.Height / 2;

            return rectf;
        }

        public void DelAllRegion()
        {
            foreach (PageClass page in ENVNow.PageList)
            {
                if (page.No == PAGEUI.PageNow.No)
                    continue;

                //清除外框 重新加入新的
                page.AnalyzeRoot.BranchList.Clear();
            }
        }

        #region 同位

        JzFindObjectClass m_Find = new JzFindObjectClass();
        HistogramClass m_Histogram = new HistogramClass(2);
        JzToolsClass JzTool = new JzToolsClass();

        List<AnalyzeClass> m_MarkTheSameList = new List<AnalyzeClass>();
        PointF ShiftPT = new PointF();
        //PointF ShiftPTORG = new PointF();
        PointF ShiftPT2 = new PointF();

        //public RectangleF FindRectSub(Bitmap ebmpInput, RectangleF eRectF, int eSubPixel = 20)
        //{
        //    int iInflate = 10;
        //    RectangleF rectORG = new RectangleF(eRectF.X, eRectF.Y, eRectF.Width, eRectF.Height);
        //    rectORG.Inflate(iInflate, 0);

        //    int iCount = (int)rectORG.Height / eSubPixel;
        //    int iSubPixel = (int)rectORG.Height % eSubPixel;
        //    Bitmap bmpSub = new Bitmap(1, 1);

        //    string _path = "D:\\LOA\\SUB";
        //    if (!System.IO.Directory.Exists(_path))
        //        System.IO.Directory.CreateDirectory(_path);

        //    Rectangle MaxRect = new Rectangle((int)eRectF.X, (int)eRectF.Y, (int)eRectF.Width, (int)eRectF.Height);
        //    RectangleF rectCorp = new RectangleF();
        //    RectangleF RectF = new RectangleF();
        //    int i = 0;
        //    while (i < iCount)
        //    {

        //        rectCorp = new RectangleF(rectORG.X, rectORG.Y + i * eSubPixel, rectORG.Width, eSubPixel);
        //        rectCorp.Intersect(rectORG);

        //        bmpSub.Dispose();
        //        bmpSub = (Bitmap)ebmpInput.Clone(rectCorp, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

        //        m_Histogram.GetHistogram(bmpSub);
        //        m_Find.SetThreshold(bmpSub, JzTool.SimpleRect(bmpSub.Size), m_Histogram.MeanGrade + 5, 255, 0, true);
        //        m_Find.Find(bmpSub, Color.Red);

        //        MaxRect = m_Find.GetRectExpectAround(JzTool.SimpleRect(bmpSub.Size), 10, iInflate / 2);
        //        //JzTool.DrawRectEx(bmpSub, MaxRect, new Pen(Color.Lime, 2));

        //        RectangleF RectFX = new RectangleF(MaxRect.X + rectCorp.X, MaxRect.Y + rectCorp.Y, MaxRect.Width, MaxRect.Height);

        //        RectF = MergeTwoRects(RectF, RectFX);

        //        //bmpSub.Save(_path + "\\" + i.ToString() + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);

        //        i++;
        //    }

        //    //RectF.X += rectORG.X;
        //    //RectF.X += rectORG.X;

        //    //Bitmap bmpDes = new Bitmap(ebmpInput);
        //    //JzTool.DrawRect(bmpDes, RectF, new Pen(Color.Lime, 2));
        //    //bmpDes.Save(_path + "\\" + "des" + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);

        //    return RectF;
        //}
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
        RectangleF MergeTwoRects(RectangleF rect1, RectangleF rect2)
        {
            RectangleF rect = new RectangleF();

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

        public void FindMarkSame(PointF ShiftPTORG)
        {
            int i = 0;

            //List<DoffsetClass> DoffsetList = new List<DoffsetClass>();

            //Get the Anallyze Data For Find
            //Bitmap bmpPageOrg = PAGEUI.PageNow.GetbmpORG((PageOPTypeEnum)PageNow.PageOPTypeIndex);

            int selectno = PAGEUI.AnalyzeSelectNow.No;
            RectangleF myRectF = PAGEUI.AnalyzeSelectNow.GetMyMoverRectF();
            List<RectangleF> OrgRectFList = new List<RectangleF>();

            m_MarkTheSameList.Clear();
            PAGEUI.AnalyzeSelectNow.FillToListRemoveMe(m_MarkTheSameList);

            if (m_MarkTheSameList.Count == 0)
                return;

            foreach (PageClass page in ENVNow.PageList)
            {

                if (page.No == PAGEUI.PageNow.No)
                {
                    continue;
                }

                Bitmap bmpPageOrg = page.GetbmpORG((PageOPTypeEnum)page.PageOPTypeIndex);
                Bitmap bmpAnalyzeSelectNow = new Bitmap(1, 1);

                Bitmap bmpDes = new Bitmap(bmpPageOrg);

                ConverAnalyzeToList(page.AnalyzeRoot);

                //RectangleF toshaperectfXXX = m_MarkTheSameList[0].GetMyMoverRectF();
                int count = RawAnalyzeList.Count;

                i = 0;
                while (i < count)
                //foreach(AnalyzeClass analyze in AnalyzeList)
                {
                    AnalyzeClass analyze = RawAnalyzeList[i];
                    if (analyze.No == PAGEUI.AnalyzeSelectNow.No && analyze.PageNo == PAGEUI.AnalyzeSelectNow.PageNo)
                    {
                        i++;
                        continue;
                    }


                    RectangleF torectf = analyze.GetMyMoverRectF();
                    List<RectangleF> foundrectlist = new List<RectangleF>();

                    if (torectf != myRectF)
                    {
                        if (RectIsTheSame(RectFToRect(torectf), RectFToRect(myRectF), 20))
                        {
                            int j = 0;
                            ShiftPT2 = new PointF(0, 0);

                            foreach (AnalyzeClass analyze1 in m_MarkTheSameList)
                            {
                                ShiftPT = new PointF((torectf.X - myRectF.X), (torectf.Y - myRectF.Y));
                                RectangleF toshaperectf = analyze1.GetMyMoverRectF();

                                toshaperectf.Offset(ShiftPT);

                                //if (CheckTESTOverlap(toshaperectf, 20, 50))
                                //    continue;

                                if (j == 0)
                                {
                                    RectangleF toshaperectfORG = analyze1.GetMyMoverRectF();
                                    toshaperectfORG.Offset(ShiftPT);
                                    RectangleF rectGood = PAGEUI.FindRectSub(bmpPageOrg, toshaperectfORG, torectf, 40);
                                    ShiftPT2 = new PointF((rectGood.X - ShiftPTORG.X), (rectGood.Y - ShiftPTORG.Y));

                                    //int iInflate = 10;
                                    //RectangleF rectORG = new RectangleF(toshaperectfORG.X, toshaperectfORG.Y, toshaperectfORG.Width, toshaperectfORG.Height);
                                    //rectORG.Inflate(iInflate, 10);
                                    //JzTool.DrawRect(bmpDes, rectORG, new Pen(Color.Lime, 2));

                                }

                                toshaperectf.Offset(ShiftPT2);
                                toshaperectf.Intersect(torectf);
                                foundrectlist.Add(toshaperectf);

                                j++;

                            }
                        }
                    }


                    //if (analyze.IsSelected && analyze.No != 1)
                    {
                        //List<Rectangle> foundrectlist = new List<Rectangle>();

                        //analyze.IsTempSave = true;

                        //analyze.B02_CreateFindInsideRequirement(bmpPageOrg, new PointF(0, 0), foundrectlist, threshold, extend);

                        //analyze.IsTempSave = false;
                        if (foundrectlist.Count > 0)
                        {
                            int k = 0;
                            analyze.BranchList.Clear();
                            foreach (RectangleF foundrect in foundrectlist)
                            {
                                AddBranchLevel(analyze.No, foundrect, 0, 0, m_MarkTheSameList[k]);
                                k++;
                            }
                        }
                    }
                    i++;
                }


                string _path = "D:\\LOA\\SUB";
                if (!System.IO.Directory.Exists(_path))
                    System.IO.Directory.CreateDirectory(_path);

                //if (IsSaveSub)
                //    bmpDes.Save(_path + "\\" + "Des_Page_" + page.No.ToString() + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            }

        }
        public void FindMarkSame2(PointF ShiftPTORG)
        {
            int i = 0;

            //List<DoffsetClass> DoffsetList = new List<DoffsetClass>();

            //Get the Anallyze Data For Find
            //Bitmap bmpPageOrg = PAGEUI.PageNow.GetbmpORG((PageOPTypeEnum)PageNow.PageOPTypeIndex);

            int selectno = PAGEUI.AnalyzeSelectNow.No;
            RectangleF myRectF = PAGEUI.AnalyzeSelectNow.GetMyMoverRectF();
            List<RectangleF> OrgRectFList = new List<RectangleF>();

            m_MarkTheSameList.Clear();
            PAGEUI.AnalyzeSelectNow.FillToListRemoveMe(m_MarkTheSameList);

            if (m_MarkTheSameList.Count == 0)
                return;

            foreach (PageClass page in ENVNow.PageList)
            {

                if (page.No == PAGEUI.PageNow.No)
                {
                    continue;
                }

                Bitmap bmpPageOrg = page.GetbmpORG((PageOPTypeEnum)page.PageOPTypeIndex);
                Bitmap bmpAnalyzeSelectNow = new Bitmap(1, 1);

                Bitmap bmpDes = new Bitmap(bmpPageOrg);

                ConverAnalyzeToList(page.AnalyzeRoot);

                //RectangleF toshaperectfXXX = m_MarkTheSameList[0].GetMyMoverRectF();
                int count = RawAnalyzeList.Count;

                i = 0;
                while (i < count)
                //foreach(AnalyzeClass analyze in AnalyzeList)
                {
                    AnalyzeClass analyze = RawAnalyzeList[i];
                    if (analyze.No == PAGEUI.AnalyzeSelectNow.No && analyze.PageNo == PAGEUI.AnalyzeSelectNow.PageNo)
                    {
                        i++;
                        continue;
                    }


                    RectangleF torectf = analyze.GetMyMoverRectF();
                    List<RectangleF> foundrectlist = new List<RectangleF>();

                    if (torectf != myRectF)
                    {
                        if (RectIsTheSame(RectFToRect(torectf), RectFToRect(myRectF), 20))
                        {
                            int j = 0;
                            ShiftPT2 = new PointF(0, 0);

                            foreach (AnalyzeClass analyze1 in m_MarkTheSameList)
                            {
                                ShiftPT = new PointF((torectf.X - myRectF.X), (torectf.Y - myRectF.Y));
                                RectangleF toshaperectf = analyze1.GetMyMoverRectF();

                                toshaperectf.Offset(ShiftPT);

                                //if (CheckTESTOverlap(toshaperectf, 20, 50))
                                //    continue;

                                if (j == 0)
                                {
                                    RectangleF toshaperectfORG = analyze1.GetMyMoverRectF();
                                    toshaperectfORG.Offset(ShiftPT);
                                    //toshaperectfORG.X -= ShiftPTORG.X;
                                    //toshaperectfORG.Y -= ShiftPTORG.Y;
                                    //int ixoffset = 2;
                                    //toshaperectfORG.Inflate(ixoffset, ixoffset);
                                    //RectangleF rectGood = PAGEUI.FindRectSub(bmpPageOrg, toshaperectfORG, torectf, 40);
                                    ShiftPT2 = new PointF((toshaperectfORG.X - torectf.X - ShiftPTORG.X), (toshaperectfORG.Y - torectf.Y - ShiftPTORG.Y));
                                    //ShiftPT2 = new PointF((rectGood.X - ShiftPTORG.X - ixoffset * 2), (rectGood.Y - ShiftPTORG.Y - ixoffset * 2));
                                    //int iInflate = 10;
                                    //RectangleF rectORG = new RectangleF(toshaperectfORG.X, toshaperectfORG.Y, toshaperectfORG.Width, toshaperectfORG.Height);
                                    //rectORG.Inflate(iInflate, 10);
                                    //JzTool.DrawRect(bmpDes, rectORG, new Pen(Color.Lime, 2));

                                }

                                toshaperectf.Offset(ShiftPT2);
                                toshaperectf.Intersect(torectf);
                                foundrectlist.Add(toshaperectf);

                                j++;

                            }
                        }
                    }


                    //if (analyze.IsSelected && analyze.No != 1)
                    {
                        //List<Rectangle> foundrectlist = new List<Rectangle>();

                        //analyze.IsTempSave = true;

                        //analyze.B02_CreateFindInsideRequirement(bmpPageOrg, new PointF(0, 0), foundrectlist, threshold, extend);

                        //analyze.IsTempSave = false;
                        if (foundrectlist.Count > 0)
                        {
                            int k = 0;
                            analyze.BranchList.Clear();
                            foreach (RectangleF foundrect in foundrectlist)
                            {
                                AddBranchLevel(analyze.No, foundrect, 0, 0, m_MarkTheSameList[k]);
                                k++;
                            }
                        }
                    }
                    i++;
                }


                string _path = "D:\\LOA\\SUB";
                if (!System.IO.Directory.Exists(_path))
                    System.IO.Directory.CreateDirectory(_path);

                //if (IsSaveSub)
                //    bmpDes.Save(_path + "\\" + "Des_Page_" + page.No.ToString() + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            }

        }
        public void FindMarkSameALLPara()
        {

            foreach (PageClass page in ENVNow.PageList)
            {
                if (page.No == PAGEUI.PageNow.No)
                {
                    continue;
                }

                ConverAnalyzeToList(page.AnalyzeRoot);

                int i = 0;
                int count = RawAnalyzeList.Count;
                while (i < count)
                {
                    AnalyzeClass analyze = RawAnalyzeList[i];
                    //if (analyze.No != PAGEUI.AnalyzeSelectNow.No && analyze.PageNo != PAGEUI.AnalyzeSelectNow.PageNo)
                    {
                        //同等級的參數 同位
                        if (analyze.Level == PAGEUI.AnalyzeSelectNow.Level)
                        {
                            analyze.ExtendX = PAGEUI.AnalyzeSelectNow.ExtendX;
                            analyze.ExtendY = PAGEUI.AnalyzeSelectNow.ExtendY;
                            //analyze.NORMALPara.FromString(PAGEUI.AnalyzeSelectNow.NORMALPara.ToString());
                            analyze.ALIGNPara.FromString(PAGEUI.AnalyzeSelectNow.ALIGNPara.ToString());
                            analyze.MEASUREPara.FromString(PAGEUI.AnalyzeSelectNow.MEASUREPara.ToString());
                            analyze.AOIPara.FromString(PAGEUI.AnalyzeSelectNow.AOIPara.ToString());
                            if (analyze.OCRPara.OCRMethod == OCRMethodEnum.DATAMATRIX
                                 || analyze.OCRPara.OCRMethod == OCRMethodEnum.DATAMATRIXGRADE)
                            {
                            }
                            else
                            {
                                analyze.INSPECTIONPara.FromString(PAGEUI.AnalyzeSelectNow.INSPECTIONPara.ToString());
                                analyze.OCRPara.FromString(PAGEUI.AnalyzeSelectNow.OCRPara.ToString());
                            }

                            analyze.PADPara.FromString(PAGEUI.AnalyzeSelectNow.PADPara.ToString());
                            //if (PAGEUI.AnalyzeSelectNow.Level == 2)
                              
                        }
                    }
                    i++;
                }

            }

        }

        /// <summary>
        /// 自動新增分支資料 带角度的新增
        /// </summary>
        public void AddBranchLevel(int analyzeno, RectangleF branchrect, int extend, double angle, AnalyzeClass analyzeBase = null)
        {
            List<AnalyzeClass> nextaddedanalyzelist = new List<AnalyzeClass>();
            List<int> selectanalyzenolist = new List<int>();

            //if (FirstSelectNo > -1)
            {
                //List<int> selectnolist = CheckUISelect(FirstSelectNo);

                //foreach (int no in selectnolist)
                {
                    AnalyzeClass selectanalyze = GetAnalyzeFromTree(analyzeno);

                    //AnalyzeClass branchanalyze = selectanalyze.Clone(new Point(100, 100), 0d, true, false, false, false);

                    AnalyzeClass branchanalyze = new AnalyzeClass(branchrect, angle);

                    if (analyzeBase != null)
                    {
                        branchanalyze.NORMALPara.ExtendX = analyzeBase.NORMALPara.ExtendX;
                        branchanalyze.NORMALPara.ExtendY = analyzeBase.NORMALPara.ExtendY;

                        //branchanalyze.NORMALPara.FromString(analyzeBase.NORMALPara.ToString());
                        branchanalyze.ALIGNPara.FromString(analyzeBase.ALIGNPara.ToString());
                        branchanalyze.INSPECTIONPara.FromString(analyzeBase.INSPECTIONPara.ToString());
                        branchanalyze.AOIPara.FromString(analyzeBase.AOIPara.ToString());
                        branchanalyze.OCRPara.FromString(analyzeBase.OCRPara.ToString());
                    }
                    else
                    {
                        branchanalyze.NORMALPara.ExtendX = extend;
                        branchanalyze.NORMALPara.ExtendY = extend;

                        branchanalyze.ALIGNPara.AlignMethod = AlignMethodEnum.AUFIND;
                        branchanalyze.ALIGNPara.AlignMode = AlignModeEnum.AREA;

                        branchanalyze.INSPECTIONPara.InspectionMethod = InspectionMethodEnum.PIXEL;
                        branchanalyze.INSPECTIONPara.IBArea = 30;
                        branchanalyze.INSPECTIONPara.IBCount = 10;
                        branchanalyze.INSPECTIONPara.IBTolerance = 35;
                        branchanalyze.INSPECTIONPara.InspectionAB = Inspection_A_B_Enum.ABPlus;
                    }

                    branchanalyze.ParentNo = selectanalyze.No;
                    branchanalyze.FromNodeString = selectanalyze.ToNextNodeString();

                    branchanalyze.PageNo = selectanalyze.PageNo;
                    branchanalyze.PageOPtype = selectanalyze.PageOPtype;
                    branchanalyze.AnalyzeType = AnalyzeTypeEnum.BRANCH;

                    branchanalyze.No = GetMaxNewNoFromRawAnalyzeList();
                    branchanalyze.Level = selectanalyze.Level + 1;
                    branchanalyze.RelateMover(branchanalyze.No, branchanalyze.Level);
                    branchanalyze.AliasName = branchanalyze.ToAnalyzeString();
                    branchanalyze.RelateASN = "None";
                    branchanalyze.RelateASNItem = "None";

                    branchanalyze.ToPassInfo();

                    //branchanalyze.NORMALPara.ExtendX = extend;
                    //branchanalyze.NORMALPara.ExtendY = extend;

                    //branchanalyze.ALIGNPara.AlignMethod = AlignMethodEnum.AUFIND;
                    //branchanalyze.ALIGNPara.AlignMode = AlignModeEnum.AREA;
                    //branchanalyze.INSPECTIONPara.InspectionMethod = InspectionMethodEnum.Equalize;
                    //branchanalyze.INSPECTIONPara.IBArea = 15;
                    //branchanalyze.INSPECTIONPara.IBCount = 10;
                    //branchanalyze.INSPECTIONPara.IBTolerance = 35;
                    //branchanalyze.INSPECTIONPara.InspectionAB = Inspection_A_B_Enum.AB;

                    //AddRowToAnalyzeTable(branchanalyze);
                    //branchanalyze.AddNewRowToDataTable(AnalyzeTable, IsLearn);
                    RawAnalyzeList.Add(branchanalyze);

                    //nextaddedanalyzelist.Add(branchanalyze);
                    //selectanalyzenolist.Add(branchanalyze.No);

                    selectanalyze.BranchList.Add(branchanalyze);
                }
                //FirstSelectNo = branchanalyze.No;

                //OnChangeMover(nextaddedanalyzelist, DBStatusEnum.ADD);

                //dtlvAnalyze.ExpandAll();

                //SetSelectFocus(selectanalyzenolist);
            }
        }

        /// <summary>
        /// 自動新增分支資料 带角度的新增
        /// </summary>
        public void AddBranchLevel(int analyzeno, List<RectangleF> branchrectlist, int extend, double angle, AnalyzeClass analyzeBase = null)
        {
            List<AnalyzeClass> nextaddedanalyzelist = new List<AnalyzeClass>();
            List<int> selectanalyzenolist = new List<int>();

            //if (FirstSelectNo > -1)
            {
                //List<int> selectnolist = CheckUISelect(FirstSelectNo);

                //foreach (int no in selectnolist)
                {
                    AnalyzeClass selectanalyze = GetAnalyzeFromTree(analyzeno);

                    //AnalyzeClass branchanalyze = selectanalyze.Clone(new Point(100, 100), 0d, true, false, false, false);

                    AnalyzeClass branchanalyze = new AnalyzeClass(); // branchrect, angle);

                    if (analyzeBase != null)
                    {
                        branchanalyze.NORMALPara.ExtendX = analyzeBase.NORMALPara.ExtendX;
                        branchanalyze.NORMALPara.ExtendY = analyzeBase.NORMALPara.ExtendY;

                        //branchanalyze.NORMALPara.FromString(analyzeBase.NORMALPara.ToString());
                        branchanalyze.ALIGNPara.FromString(analyzeBase.ALIGNPara.ToString());
                        branchanalyze.INSPECTIONPara.FromString(analyzeBase.INSPECTIONPara.ToString());
                        branchanalyze.AOIPara.FromString(analyzeBase.AOIPara.ToString());
                        branchanalyze.OCRPara.FromString(analyzeBase.OCRPara.ToString());
                    }
                    else
                    {
                        branchanalyze.NORMALPara.ExtendX = extend;
                        branchanalyze.NORMALPara.ExtendY = extend;

                        branchanalyze.ALIGNPara.AlignMethod = AlignMethodEnum.AUFIND;
                        branchanalyze.ALIGNPara.AlignMode = AlignModeEnum.AREA;

                        branchanalyze.INSPECTIONPara.InspectionMethod = InspectionMethodEnum.PIXEL;
                        branchanalyze.INSPECTIONPara.IBArea = 30;
                        branchanalyze.INSPECTIONPara.IBCount = 10;
                        branchanalyze.INSPECTIONPara.IBTolerance = 35;
                        branchanalyze.INSPECTIONPara.InspectionAB = Inspection_A_B_Enum.ABPlus;
                    }

                    branchanalyze.ParentNo = selectanalyze.No;
                    branchanalyze.FromNodeString = selectanalyze.ToNextNodeString();

                    branchanalyze.PageNo = selectanalyze.PageNo;
                    branchanalyze.PageOPtype = selectanalyze.PageOPtype;
                    branchanalyze.AnalyzeType = AnalyzeTypeEnum.BRANCH;

                    branchanalyze.No = GetMaxNewNoFromRawAnalyzeList();
                    branchanalyze.Level = selectanalyze.Level + 1;
                    branchanalyze.RelateMover(branchanalyze.No, branchanalyze.Level);
                    branchanalyze.AliasName = branchanalyze.ToAnalyzeString();
                    branchanalyze.RelateASN = "None";
                    branchanalyze.RelateASNItem = "None";

                    branchanalyze.ToPassInfo();

                    //branchanalyze.NORMALPara.ExtendX = extend;
                    //branchanalyze.NORMALPara.ExtendY = extend;

                    //branchanalyze.ALIGNPara.AlignMethod = AlignMethodEnum.AUFIND;
                    //branchanalyze.ALIGNPara.AlignMode = AlignModeEnum.AREA;
                    //branchanalyze.INSPECTIONPara.InspectionMethod = InspectionMethodEnum.Equalize;
                    //branchanalyze.INSPECTIONPara.IBArea = 15;
                    //branchanalyze.INSPECTIONPara.IBCount = 10;
                    //branchanalyze.INSPECTIONPara.IBTolerance = 35;
                    //branchanalyze.INSPECTIONPara.InspectionAB = Inspection_A_B_Enum.AB;

                    //AddRowToAnalyzeTable(branchanalyze);
                    //branchanalyze.AddNewRowToDataTable(AnalyzeTable, IsLearn);
                    RawAnalyzeList.Add(branchanalyze);

                    //nextaddedanalyzelist.Add(branchanalyze);
                    //selectanalyzenolist.Add(branchanalyze.No);

                    selectanalyze.BranchList.Add(branchanalyze);
                }
                //FirstSelectNo = branchanalyze.No;

                //OnChangeMover(nextaddedanalyzelist, DBStatusEnum.ADD);

                //dtlvAnalyze.ExpandAll();

                //SetSelectFocus(selectanalyzenolist);
            }
        }


        public bool RectIsTheSame(Rectangle OrgRect, Rectangle ComRect, int Percent)
        {
            bool ret = true;
            double UB = (100 + (double)Percent) / 100;
            double LB = (100 - (double)Percent) / 100;

            ret = ret & (((int)((double)OrgRect.Width * UB)) >= ComRect.Width && ((int)((double)OrgRect.Width * LB)) <= ComRect.Width);
            ret = ret & (((int)((double)OrgRect.Height * UB)) >= ComRect.Height && ((int)((double)OrgRect.Height * LB)) <= ComRect.Height);
            ret = ret & (((int)((double)GetRectArea(OrgRect) * UB)) >= GetRectArea(ComRect) && ((int)((double)GetRectArea(OrgRect) * LB)) <= GetRectArea(ComRect));

            return ret;
        }
        public int GetRectArea(Rectangle Rect)
        {
            return Rect.Width * Rect.Height;
        }

        public Rectangle RectFToRect(RectangleF RectF)
        {
            Rectangle rect = new Rectangle((int)RectF.X, (int)RectF.Y, (int)RectF.Width, (int)RectF.Height);

            return rect;
        }

        //RectangleF OffsetRect(RectangleF orgrectf, PointF centerptf)
        //{
        //    RectangleF rectf = orgrectf;

        //    rectf.Location = centerptf;

        //    rectf.X -= rectf.Width / 2;
        //    rectf.Y -= rectf.Height / 2;

        //    return rectf;
        //}

        #endregion


        #region 图像处理

        void init_Display()
        {
            DS.Initial(100, 0.01f);
            DS.SetDisplayType(DisplayTypeEnum.NORMAL);
            DS.DebugAction += DS_DebugAction;
        }

        private void DS_DebugAction(string opstring)
        {
            switch (opstring)
            {
                case "RELEASMOVE":
                    //RefreshCompound();
                    break;
            }
        }

        void update_Display()
        {
            DS.Refresh();
            DS.DefaultView();
        }

        #endregion


        #region 通过页面0和最后一个页面 自动生成所有位置的框
        private bool _ax_CheckPageCountAndStepCount()
        {
            EnvAnalyzePostionSettings envAnalyzePostionSettings = new EnvAnalyzePostionSettings(ENVNow.GeneralPosition);
            envAnalyzePostionSettings.EnvAnalyzePostions();
            return ENVNow.PageList.Count == envAnalyzePostionSettings.GetImageCount;
            //return ENVNow.PageList.Count == CamActClass.Instance.StepCount;
        }
        private void _ax_AutoRectPosition(List<DoffsetClass> doffsetlist)
        {
            if (!_ax_CheckPageCountAndStepCount())
            {
                MessageBox.Show("页面与步数不符，请检查。", "自动生成", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (ENVNow.PageList.Count < 1)
            {
                MessageBox.Show("页面数量不足，请检查。", "自动生成", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            PageClass pagefrist = ENVNow.PageList[0];
            PageClass pagelast = ENVNow.PageList[ENVNow.PageList.Count - 1];

            if (pagefrist.AnalyzeRoot.BranchList.Count <= 0 || pagelast.AnalyzeRoot.BranchList.Count <= 0)
            {
                MessageBox.Show("左上角和右下角的框不存在，生成失败。", "自动生成", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageForm _msgProcessForm = new MessageForm("自动生成中...", true);
            _msgProcessForm.Show();
            _msgProcessForm.Refresh();

            //1.先算出所有页面的宽度(bmp.width*page.count)和高度bmp.height
            int iwidth = pagefrist.GetbmpORG().Width * ENVNow.PageList.Count;
            int iheight = pagefrist.GetbmpORG().Height;

            //2.拿到页面的框的位置

            AnalyzeClass analyze1 = pagefrist.AnalyzeRoot.BranchList[0];
            RectangleF rectffrist = analyze1.GetMyMoverRectF();//   new RectangleF(analyze1.myOPRectF.X,
                                                //analyze1.myOPRectF.Y,
                                                //analyze1.myOPRectF.Width,
                                                //analyze1.myOPRectF.Height);

            AnalyzeClass analyze2 = pagelast.AnalyzeRoot.BranchList[0];
            RectangleF rectflast = analyze2.GetMyMoverRectF();// new RectangleF(analyze2.myOPRectF.X,
                                                              //analyze2.myOPRectF.Y + pagefrist.GetbmpORG().Width * (ENVNow.PageList.Count - 1),
                                                              //analyze2.myOPRectF.Width,
                                                              //analyze2.myOPRectF.Height);
            

            EnvAnalyzePostionSettings envAnalyzePostionSettings = new EnvAnalyzePostionSettings(ENVNow.GeneralPosition);
            int row = envAnalyzePostionSettings.RectRowCount;
            int col = envAnalyzePostionSettings.RectColumnCount;

            PointF r1 = GetRectFCenter(rectffrist);
            PointF r2 = GetRectFCenter(rectflast);

            float rowoffset = (r2.Y - r1.Y) / (row - 1);
            float coloffset = (r2.X + pagefrist.GetbmpORG().Width * (ENVNow.PageList.Count - 1) - r1.X) / (col - 1);

            //List<DoffsetClass> DoffsetList = new List<DoffsetClass>();
            //DoffsetList.Clear();

            List<RectangleF> drawlist = new List<RectangleF>();

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    //PointF ptfcenterfrist = GetRectFCenter(rectffrist);
                    DoffsetClass doffset = new DoffsetClass(0, new PointF(r1.X + j * coloffset, r1.Y + i * rowoffset));
                    doffsetlist.Add(doffset);

                    RectangleF myrecttmp = new RectangleF(doffset.OffsetF, rectffrist.Size);
                    drawlist.Add(myrecttmp);
                }
            }

            //RectangleF[] fs = drawlist.ToArray();
            //Bitmap bmpdraw = new Bitmap(pagefrist.GetbmpORG().Width * (ENVNow.PageList.Count), pagefrist.GetbmpORG().Height);
            //Graphics graphics = Graphics.FromImage(bmpdraw);
            //graphics.Clear(Color.Black);

            //foreach (PageClass page in ENVNow.PageList)
            //{
            //    graphics.DrawImage(page.GetbmpORG(), new PointF(page.No * pagefrist.GetbmpORG().Width, 0));
            //}

            //graphics.DrawRectangles(new Pen(Color.Red, 2), fs);
            //graphics.Dispose();
            //bmpdraw.Save("D:\\LOA\\COLOR\\TMP.BMP", System.Drawing.Imaging.ImageFormat.Bmp);

            _msgProcessForm.Close();
            _msgProcessForm.Dispose();

        }

        private void _ax_AutoRect(List<DoffsetClass> DoffsetList)
        {
            //Get the Anallyze Data For Find
            Bitmap bmpPageOrg = PAGEUI.PageNow.GetbmpORG((PageOPTypeEnum)PAGEUI.PageNow.PageOPTypeIndex);

            foreach (PageClass page in ENVNow.PageList)
            {
                if (page.No == PAGEUI.PageNow.No)
                    continue;


                switch (Universal.OPTION)
                {
                    case JetEazy.OptionEnum.MAIN_SERVICE:
                    case JetEazy.OptionEnum.MAIN_X6:
                    case JetEazy.OptionEnum.MAIN_SD:
                    case JetEazy.OptionEnum.MAIN_SDM1:
                    case JetEazy.OptionEnum.MAIN_SDM2:
                    case OptionEnum.MAIN_SDM3:
                        if (page.AnalyzeRoot.BranchList.Count > 0 && page.No == ENVNow.PageList.Count - 1)
                        {
                            AnalyzeClass analyze1 = new AnalyzeClass();
                            analyze1.FromString(page.AnalyzeRoot.BranchList[0].ToString());

                            //清除外框 重新加入新的
                            page.AnalyzeRoot.BranchList.Clear();
                            page.AnalyzeRoot.BranchList.Add(analyze1);
                        }
                        else
                            page.AnalyzeRoot.BranchList.Clear();

                        break;
                }

                Bitmap bmpPageOrgtmp = page.GetbmpORG((PageOPTypeEnum)page.PageOPTypeIndex);
                //取得自身的區域
                int selectno = PAGEUI.AnalyzeSelectNow.No;
                RectangleF myRectF = PAGEUI.AnalyzeSelectNow.GetMoverRectF(bmpPageOrg);
                List<RectangleF> OrgRectFList = new List<RectangleF>();

                ConverAnalyzeToList(page.AnalyzeRoot);

                //取得所有定義過的區域
                foreach (AnalyzeClass analyze in page.AnalyzeRoot.BranchList)
                {
                    if (analyze.No != 1)
                    {
                        OrgRectFList.Add(analyze.GetMoverRectF(bmpPageOrgtmp));
                    }
                }

                bool IsIncluded = false;

                //Check Duplicate and Copy Ananlyze
                foreach (DoffsetClass doffset in DoffsetList)
                {
                    IsIncluded = false;

                    if (doffset.OffsetF.X < page.No * bmpPageOrg.Width || doffset.OffsetF.X > (page.No + 1) * bmpPageOrg.Width)
                    {
                        IsIncluded = true;
                    }

                    DoffsetClass doffset1 = new DoffsetClass(0, new PointF(doffset.OffsetF.X, doffset.OffsetF.Y));
                    doffset1.OffsetF.X -= page.No * bmpPageOrg.Width;

                    RectangleF foundrectf = OffsetRect(myRectF, doffset1.OffsetF);
                    //foundrectf.X -= page.No * bmpPageOrg.Width;
                    if (!IsIncluded)
                    {
                        //檢查是否有和現有的Analyze重覆超過30%的，有的話視為同一個就不加了
                        foreach (RectangleF rectf in OrgRectFList)
                        {
                            RectangleF rectfintersect = foundrectf;
                            rectfintersect.Intersect(rectf);

                            if ((float)(rectfintersect.Width * rectfintersect.Height) / (float)(rectf.Width * rectf.Height) > 0.3)
                            {
                                IsIncluded = true;
                                break;
                            }
                        }
                    }


                    ////检查是否在大定位框里 不在的话 不要了

                    //if (!IsIncluded)
                    //{
                    //    RectangleF rectfintersectORG = foundrectf;
                    //    rectfintersectORG.Intersect(page.AnalyzeRoot.GetMoverRectF(bmpPageOrgtmp));

                    //    if ((float)(rectfintersectORG.Width * rectfintersectORG.Height) / (float)(foundrectf.Width * foundrectf.Height) < 0.8)
                    //    {
                    //        IsIncluded = true;
                    //    }
                    //}

                    //doffset.OffsetF.X -= page.No * bmpPageOrg.Width;
                   

                    //若沒有被包含則加入框裏
                    if (!IsIncluded)
                        AddSameLevel(page.No, doffset1, new PointF(myRectF.X + myRectF.Width / 2, myRectF.Y + myRectF.Height / 2));
                }

            }
        }
        private PointF GetRectFCenter(RectangleF RectF)
        {
            return new PointF(RectF.X + (RectF.Width / 2), RectF.Y + (RectF.Height / 2));
        }

        int m_Mapping_Col = 0;
        int m_Mapping_Row = 0;
        private void _ax_AutoReportIndex()
        {
            EnvClass env = ENVNow;
            int bmpwidth = env.PageList[0].GetbmpORG().Width;
            List<AnalyzeClass> BranchList = new List<AnalyzeClass>();
            BranchList.Clear();

            int MappingYOffset = 0;
            int ipageindex = 0;

            foreach (PageClass page in env.PageList)
            {
                //if (page.CamIndex >= Universal.CCDCollection.CCDRectRelateIndexList.Count)
                //    MappingYOffset = 0;
                //else
                //    MappingYOffset = Universal.CCDCollection.CCDRectRelateIndexList[page.CamIndex].SizedRect.Y;


                foreach (AnalyzeClass analyze in page.AnalyzeRoot.BranchList)
                {
                    //analyze.myOPRectF.X += page.GetbmpORG().Width * page.No;

                    AnalyzeClass analyze1 = new AnalyzeClass();
                    analyze1.FromString(analyze.ToString());
                    analyze1.myOPRectF = analyze.GetMyMoverRectF();
                    //new RectangleF(analyze.myOPRectF.X,
                    //analyze.myOPRectF.Y + MappingYOffset,
                    //analyze.myOPRectF.Width,
                    //analyze.myOPRectF.Height);
                    analyze1.myOPRectF.X += bmpwidth * ipageindex;
                    BranchList.Add(analyze1);
                }
                ipageindex++;
            }

            //foreach (AnalyzeClass analyze in BranchList)
            //{
            //    analyze.myOPRectF.X += bmpwidth * page.No;
            //    //BranchList.Add(analyze);
            //}

            int Highest = 100000;
            int HighestIndex = -1;
            int ReportIndex = 0;
            List<string> CheckList = new List<string>();

            int i = 0;

            //Clear All Index To 0 and Check the Highest

            foreach (AnalyzeClass keyassign in BranchList)
            {
                keyassign.ReportRowCol = "";
                keyassign.ReportIndex = 0;
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
                    if (keyassign.ReportIndex == 0)
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
                    if (keyassign.ReportIndex == 0)
                    {
                        if (IsInRange((int)keyassign.myOPRectF.Y, Highest, 138))
                        {
                            CheckList.Add(keyassign.myOPRectF.X.ToString("00000000") + "," + i.ToString());
                        }
                    }
                    i++;
                }

                CheckList.Sort();

                i = 1;
                foreach (string Str in CheckList)
                {
                    string[] Strs = Str.Split(',');

                    //KEYBOARD.vKEYASSIGNLIST[int.Parse(Strs[1])].ReportIndex = ReportIndex;

                    BranchList[int.Parse(Strs[1])].ReportIndex = ReportIndex;
                    BranchList[int.Parse(Strs[1])].ReportRowCol = CheckList.Count.ToString() + "-" + i.ToString();

                    ReportIndex++;
                    i++;
                }
            }

            if (BranchList.Count == 0)
                return;

            m_Mapping_Col = int.Parse(BranchList[0].ReportRowCol.Split('-')[0]);
            m_Mapping_Row = BranchList.Count / m_Mapping_Col;
           
            int ix = 0;
            int iy = 0;

            //string colname = "A";
            int colindex = 1;

            int colnameindex = 1;

            i = 0;
            while (i < BranchList.Count)
            {
                //m_MappingItem[i] = new Label();
                //m_MappingItem[i].Name = BranchList[i].ToAnalyzeString();

                //m_MappingItem[i] = new Label();

                foreach (AnalyzeClass analyzetmp in BranchList)
                {
                    if ((i + 1) == analyzetmp.ReportIndex)
                    {
                        analyzetmp.NORMALPara.AliasName = colindex.ToString("00") + "-" + analyzetmp.ReportRowCol.Split('-')[1].PadLeft(2, '0');

                        //m_MappingItem[i].Name = analyzetmp.ToAnalyzeString();
                        //m_MappingItem[i].Text = colindex.ToString() + "-" + analyzetmp.ReportRowCol.Split('-')[1];
                        ////m_MappingItem[i].Text = colindex.ToString() + "-" + colnameindex.ToString();

                        //m_MappingItem[i].AccessibleName = colindex.ToString() + "-" + analyzetmp.ReportRowCol.Split('-')[1];

                        break;
                    }
                }


                //m_MappingItem[i].BackColor = Color.Green;
                //m_MappingItem[i].Font = new System.Drawing.Font("黑体", 9F);
                //m_MappingItem[i].TextAlign = ContentAlignment.MiddleCenter;
                //m_MappingItem[i].Width = iMappingItemWidth;
                //m_MappingItem[i].Height = iMappingItemHeight;
                //m_MappingItem[i].Location = new Point(5 + ix, 5 + iy);
                //m_MappingItem[i].DoubleClick += RunUI_DoubleClick;
                //m_MappingItem[i].MouseEnter += RunUI_MouseEnter;
                //ix += m_MappingItem[i].Width + 3;
                colnameindex++;
                if ((i + 1) % m_Mapping_Col == 0)
                {
                    //iy += m_MappingItem[i].Height + 3;
                    ix = 0;

                    colindex++;

                    colnameindex = 1;

                    //m_MappingItem[i].Text = (i + 1).ToString();
                }
                //this.pnlResult.Controls.Add(m_MappingItem[i]);

                i++;
            }

            foreach (PageClass page in ENVNow.PageList)
            {
                ConverAnalyzeToList(page.AnalyzeRoot);

                i = 0;
                int count = RawAnalyzeList.Count;
                while (i < count)
                {
                    AnalyzeClass analyze = RawAnalyzeList[i];
                    foreach (AnalyzeClass analyzetmp in BranchList)
                    {
                        if (analyze.ToAnalyzeString() == analyzetmp.ToAnalyzeString())
                        {
                            analyze.AliasName = analyzetmp.NORMALPara.AliasName;
                            break;
                        }
                    }
                    i++;
                }

            }

            ////对应参数的名称 然后保存参数
            //foreach (PageClass page in env.PageList)
            //{
            //    foreach (AnalyzeClass analyze in page.AnalyzeRoot.BranchList)
            //    {
            //        foreach (AnalyzeClass analyzetmp in BranchList)
            //        {
            //            if (analyze.NORMALPara.AliasName == analyzetmp.AliasName)
            //            {
            //                analyze.AliasName = analyzetmp.NORMALPara.AliasName;
            //                break;
            //            }
            //        }
            //    }
            //}

        }

        bool IsInRange(int FromValue, int CompValue, int DiffValue)
        {
            return (FromValue >= CompValue - DiffValue) && (FromValue <= CompValue + DiffValue);
        }
        void ReadData(ref string DataStr, string FileName)
        {
            FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.None);
            StreamReader Srr = new StreamReader(fs, Encoding.Default);

            DataStr = Srr.ReadToEnd();

            Srr.Close();
            Srr.Dispose();
        }
        void SaveData(string DataStr, string FileName)
        {
            File.WriteAllText(FileName, DataStr, Encoding.Default);
        }
        #endregion


    }
}
