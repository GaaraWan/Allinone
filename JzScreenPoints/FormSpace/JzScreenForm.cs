//#define OPT_JUMBO

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Drawing.Imaging;

using JzScreenPoints.BasicSpace;
using JzScreenPoints.ControlSpace;
using JzScreenPoints.UISpace;
using JzScreenPoints.OPSpace;
using JzScreenPoints;
using JetEazy.ControlSpace;

#if(OPT_JUMBO)
using AllinOne.Jumbo.Net;
#endif

namespace JzScreenPoints
{
    public partial class JzScreenForm : Form
    {
        CCDCollectionClass CCDCollection;
        JzScreenPointsClass JZSCREENPOINTS;
        int m_istep = 0;
        public JzScreenForm(JzScreenPointsClass _jzscreen,int istep, CCDCollectionClass ccdcollection)
        {
            InitializeComponent();

            JZSCREENPOINTS = _jzscreen;
            CCDCollection = ccdcollection;

            if (istep >= JZSCREENPOINTS.StepCount)
                m_istep = 0;
            else
                m_istep = istep;

            this.Load += new EventHandler(JzScreenForm_Load);
            this.FormClosing += new FormClosingEventHandler(JzScreenForm_FormClosing);
            
        }

        void JzScreenForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        void JzScreenForm_Load(object sender, EventArgs e)
        {
            Initial();
        }

        DispUI DISPUI;
        OPDisplayCell OPDisplay;
        Bitmap bmpDISPVIEW;

        Bitmap bmp = new Bitmap(1, 1);
        Bitmap bmptmp = new Bitmap(1, 1);
        Bitmap opbmp = new Bitmap(1, 1);

        OPDisplayCell opsmalldisply;
        JzFindObjectClass jzfind = new JzFindObjectClass();
        Button btnSAVE;
        Button btnAutoFind;

        ComboBox cboSideList;
        ComboBox cboPointsDataList;
        ComboBox cboCCDCamList;

        Button btnGetBmpEx;
        Button btnGetBmp;
        Button btnDraw;
        Button btnAutoTest;
        Button btnAutoLine;
        Button btnSavePoints;
        Button btnSavebmp;

        Button btnSaveAllPoints;

        NumericUpDown numRatio;

        bool IsNeedToChange = false;

        List<BASISClass> BasisList
        {
            get {
                return JZSCREENPOINTS.m_JzScreenList[m_istep].BasisList;
            }
        }
        JzScreenItemClass JzScreenItem
        {
            get
            {
                return JZSCREENPOINTS.m_JzScreenList[m_istep];
            }
        }

#if(OPT_JUMBO)
        public static IxJumbo m_IxJumbo;
#endif

        void Initial()
        {
            this.Text = "自動校正介面";

            INI.Initial();

            DISPUI = dispUI1;
            OPDisplay = new OPDisplayCell(DISPUI, -3, -6, 2);

            Bitmap a = new Bitmap(JzScreenItem.bmpORG);
            bmpDISPVIEW = new Bitmap(a);
            a.Dispose();
            OPDisplay.SetDispImage(bmpDISPVIEW);
            OPDisplay.SelectAction += new OPDisplayCell.SelectrHandler(OPDisplay_SelectAction);
            OPDisplay.UseHotKeyToResize(true);
            opsmalldisply = new OPDisplayCell(dispUI2, 0, -5, 4);

            //JetEazy.Universal.InitialCCD();

            InitialServer();

//#if(!OPT_JUMBO)
            //_dragonDrv = new DragonDrv.DragonDrvClass();//這裡直接通過校正程式調檔
            //string strLoadPath = Application.StartupPath + "\\000";//自己設定檔案路徑
            //_dragonDrv = new DragonDrv.DragonDrvClass(strLoadPath);
//#endif
            cboSideList = comboBox1;
            cboCCDCamList = comboBox3;
            numRatio = numericUpDown1;

            int i = 0;
            while (i < 1) // _dragonDrv.SIDEList.Count)
            {
                cboSideList.Items.Add("CAM" + i.ToString("000"));
                i++;
            }

            i = 0;
            while (i < CCDCollection.GetCCDCount) // _dragonDrv.SIDEList.Count)
            {
                cboCCDCamList.Items.Add("CAM" + i.ToString("000"));
                i++;
            }

            numRatio.Value = (decimal)INI.Allinone_Cam_Ratio;
            numRatio.ValueChanged += NumRatio_ValueChanged;

            if (INI.Allinone_Calibrate_Cam_Index > cboCCDCamList.Items.Count)
                INI.Allinone_Calibrate_Cam_Index = 0;

            cboCCDCamList.SelectedIndex = INI.Allinone_Calibrate_Cam_Index;
            cboCCDCamList.SelectedIndexChanged += CboCCDCamList_SelectedIndexChanged;
            cboSideList.SelectedIndex = 0;

            cboPointsDataList = comboBox2;
            i = 0;
            while (i <= JZSCREENPOINTS.StepCount)
            {
                cboPointsDataList.Items.Add(i.ToString("000"));
                i++;
            }

            cboPointsDataList.SelectedIndexChanged += CboPointsDataList_SelectedIndexChanged;
            cboPointsDataList.SelectedIndex = m_istep;
            

            //PointF ptf = _dragonDrv.SIDEList[cboCamList.SelectedIndex].WorldToViewEx(ptfview);
            //PointF ptf = _dragonDrv.SIDEList[cboCamList.SelectedIndex].ViewToWorldEx(ptfview);

            //LoadBasis();

            btnSAVE = button1;
            btnSAVE.Click += new EventHandler(btnSAVE_Click);

            btnAutoFind = button2;
            btnAutoFind.Click += new EventHandler(btnAutoFind_Click);

            btnGetBmp = button3;
            btnGetBmp.Click += new EventHandler(btnGetBmp_Click);

            btnDraw = button4;
            btnDraw.Click += new EventHandler(btnDraw_Click);

            btnAutoTest = button5;
            btnAutoTest.Click += new EventHandler(btnAutoTest_Click);

            btnAutoLine = button6;
            btnAutoLine.Click += new EventHandler(btnAutoLine_Click);

            btnSavePoints = button7;
            btnSavePoints.Click += new EventHandler(btnSavePoints_Click);

            btnSavebmp = button8;
            btnSavebmp.Click += BtnSavebmp_Click;

            btnSaveAllPoints = button9;
            btnSaveAllPoints.Click += BtnSaveAllPoints_Click;

            btnGetBmpEx = button10;
            btnGetBmpEx.Click += BtnGetBmpEx_Click;

            btnAutoTest.Visible = false;
            btnAutoLine.Visible = false;

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            this.KeyUp += new KeyEventHandler(Form1_KeyUp);

            SyncAll();

            IsNeedToChange = true;
        }

        private void NumRatio_ValueChanged(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            INI.Allinone_Cam_Ratio = (float)numRatio.Value;
            INI.Save();

            SyncAll();
        }

        private void CboCCDCamList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            INI.Allinone_Calibrate_Cam_Index = cboCCDCamList.SelectedIndex;
            INI.Save();

            SyncAll();
        }

        private void BtnGetBmpEx_Click(object sender, EventArgs e)
        {
            opbmp.Dispose();
            //MOD GAARA
            opbmp = new Bitmap(CCDCollection.GetBMP(INI.Allinone_Calibrate_Cam_Index, true));
            //opbmp.RotateFlip(RotateFlipType.Rotate270FlipNone);

            OPDisplay.SetDispImage(opbmp);
            //opbmp.Save(Application.StartupPath + "\\Test" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

            //JZSCREENPOINTS.m_JzScreenList[cboPointsDataList.SelectedIndex].bmpORG.Dispose();
            //JZSCREENPOINTS.m_JzScreenList[cboPointsDataList.SelectedIndex].bmpORG = new Bitmap(opbmp);

            //bmpDISPVIEW.Dispose();
            //bmpDISPVIEW = new Bitmap(opbmp);
        }

        private void BtnSaveAllPoints_Click(object sender, EventArgs e)
        {
            JZSCREENPOINTS.m_JzScreenList[cboPointsDataList.SelectedIndex].m_step = cboPointsDataList.SelectedIndex;
            JZSCREENPOINTS.m_JzScreenList[cboPointsDataList.SelectedIndex].Save(Universal.JSPCollectionPath + "\\0000");
            //JZSCREENPOINTS.Save();


        }

        private void CboPointsDataList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            m_istep = cboPointsDataList.SelectedIndex;

            Bitmap bmpA = new Bitmap(JzScreenItem.bmpORG);
            opbmp.Dispose();
            //MOD GAARA

            //JzScreenItem.bmpORG.Save(Application.StartupPath + "\\Test" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            opbmp = new Bitmap(bmpA);
            bmpA.Dispose();
            OPDisplay.SetDispImage(opbmp);
            bmpDISPVIEW.Dispose();
            bmpDISPVIEW = new Bitmap(opbmp);
            //OPDisplay.SetDispImage(JzScreenItem.bmpORG);
            SyncAll();
        }

        private void BtnSavebmp_Click(object sender, EventArgs e)
        {
            bmpDISPVIEW.Save(@"D:\LOA\NEWERA\Test" + cboPointsDataList.Text + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            MessageBox.Show("存圖完成,存圖位置:"+ @"D:\LOA\NEWERA\Test" + cboPointsDataList.Text + ".bmp");
        }

        void btnSavePoints_Click(object sender, EventArgs e)
        {
            List<Point> pts = new List<Point>();
            foreach (BASISClass basis in BasisList)
            {
                Rectangle rect = basis.Cell.RectCell;
                //Point ptLT = rect.Location;
                //Point PTLT = JzTools.PointFToPoint(_dragonDrv.SIDEList[cboCamList.SelectedIndex].ViewToWorldEx(ptLT));
                //pts.Add(PTLT);
                //Point ptLB = new Point(rect.Location.X, rect.Bottom);
                //Point PTLB = JzTools.PointFToPoint(_dragonDrv.SIDEList[cboCamList.SelectedIndex].ViewToWorldEx(ptLB));
                //pts.Add(PTLB);
                //Point ptRT = new Point(rect.Right, rect.Location.Y);
                //Point PTRT = JzTools.PointFToPoint(_dragonDrv.SIDEList[cboCamList.SelectedIndex].ViewToWorldEx(ptRT));
                //pts.Add(PTRT);
                //Point ptRB = new Point(rect.Right, rect.Bottom);
                //Point PTRB = JzTools.PointFToPoint(_dragonDrv.SIDEList[cboCamList.SelectedIndex].ViewToWorldEx(ptRB));
                //pts.Add(PTRB);

                Point ptCenter = JzTools.GetRectCenter(rect);
                //MOD GAARA
                Point ptCenterEx = ptCenter;// JzTools.PointFToPoint(_dragonDrv.SIDEList[cboCamList.SelectedIndex].ViewToWorldEx(ptCenter));

                ptCenterEx.X += 2;
                ptCenterEx.Y += 2;

                pts.Add(ptCenterEx);
            }
            string strPoints = "";

            foreach (Point pt in pts)
            {
                strPoints += JzTools.PointToString(pt) + Environment.NewLine;
            }

            string m_points_data_path = "D:\\POINTSDATA\\";
            string m_points_data_name = "Cali_#" + cboPointsDataList.Text + ".txt";
            if (!Directory.Exists(m_points_data_path))
                Directory.CreateDirectory(m_points_data_path);

            JzTools.SaveData(strPoints, m_points_data_path + m_points_data_name);

            MessageBox.Show("保存數據完成,數據位置:"+ m_points_data_path + m_points_data_name);
        }

        void btnAutoLine_Click(object sender, EventArgs e)
        {
#if(old_find_line)
            string _path = JzTools.OpenFilePicker("", "");
            Bitmap bmp0 = new Bitmap(_path);
            Bitmap bmp1 = new Bitmap(bmp0);
            bmp0.Dispose();

            Bitmap bmp = new Bitmap(bmp1);

            HistogramClass Histogram = new HistogramClass(2);
            JzFindObjectClass FindObject = new JzFindObjectClass();
            List<PointF> pts = new List<PointF>();
            PointF[] linepts;

            Histogram.GetHistogram(bmp);
            FindObject.SetThreshold(bmp, JzTools.SimpleRect(bmp.Size), 10, 255, 0, true);
            bmp.Save(@"D:\LOA\NEWERA\DiplayAutoFind.BMP", System.Drawing.Imaging.ImageFormat.Bmp);
            FindObject.Find(bmp, Color.Red);

            foreach (FoundClass found in FindObject.FoundList)
            {
                if (found.rect.Width > 5 && found.rect.Height > 3)
                {
                    Rectangle rect = found.rect;
                    rect.Inflate(2, 2);
                    //rect.Intersect(JzTools.SimpleRect(bmp.Size));
                    //BASISClass basis = new BASISClass(rect, 0, 0);
                    //BasisList.Add(basis);
                    pts.Add(JzTools.GetRectCenter(rect));
                    JzTools.DrawRectEx(bmp, rect, new Pen(Color.Lime, 2));
                }
            }

            linepts = new PointF[pts.Count];

            bmp.Save(@"D:\LOA\NEWERA\DiplayAutoFind2.BMP", System.Drawing.Imaging.ImageFormat.Bmp);
            //bmp.Dispose();

            int i = 0;
            foreach (PointF pt in pts)
            {
                linepts[i] = pt;
                i++;
            }

            QvLineFit _line = new QvLineFit();
            _line.LeastSquareFit(linepts);

            linepts[0].Y = (float)(_line.A * (double)linepts[0].X + _line.B);
            linepts[linepts.Length - 1].Y = (float)(_line.A * (double)linepts[linepts.Length - 1].X + _line.B);

            LineClass linea = new LineClass(linepts[0], linepts[linepts.Length - 1]);
            PointF pt1 = linea.GetPtFromX(0);
            PointF pt2 = linea.GetPtFromX(1000);

            JzTools.DrawLine(bmp, new Pen(Color.Red, 1), pt1, pt2);
            //JzTools.DrawLine(bmp, new Pen(Color.Red, 1), linepts[0], linepts[linepts.Length - 1]);

            i = 0;
            string str = "";
            double dis = 0d;
            foreach (PointF pt in pts)
            {
                //linepts[i] = pt;
                dis = linea.GetVerticalLength(pt);
                str += pt.X.ToString("0.000") + "," + pt.Y.ToString("0.000") + "," + dis.ToString("0.000") + Environment.NewLine;

                i++;
            }

            JzTools.SaveData(str, @"D:\LOA\NEWERA\" + JzTimes.DateTimeSerialString + ".csv");

            bmp.Save(@"D:\LOA\NEWERA\DiplayAutoFind3.BMP", System.Drawing.Imaging.ImageFormat.Bmp);
            bmp.Dispose();
#endif
        }

        void btnAutoTest_Click(object sender, EventArgs e)
        {
            Bitmap _bmporg = new Bitmap(2000, 2000);
            //Bitmap _bmporg = new Bitmap(4912, 3684);
            Graphics g = Graphics.FromImage(_bmporg);
            g.Clear(Color.Black);
            Rectangle[] _rects = new Rectangle[200];
            SolidBrush sb=new SolidBrush(Color.White);
            int i = 0;
            int _offsetx = 0;
            int _offsety = 0;
            while (i < _rects.Length)
            {
                _rects[i] = new Rectangle(300 + _offsetx, 300 + _offsety, 6, 20);
                _offsetx += 60;
                if ((i+1) % 10 == 0)
                {
                    _offsety += 30;
                    _offsetx = 0;
                    //else
                    //    _offsety = 0;
                }

                i++;
            }
            g.FillRectangles(sb, _rects);

            _bmporg.Save(@"D:\LOA\NEWERA\bmpOrg.BMP", System.Drawing.Imaging.ImageFormat.Bmp);
        }

        void btnDraw_Click(object sender, EventArgs e)
        {
            List<Point> pts = new List<Point>();
            //List<Point> mm_list = new List<Point>();
            foreach (BASISClass basis in BasisList)
            {
                Rectangle rect = basis.Cell.RectCell;
                //Point ptLT = rect.Location;
                //Point PTLT = JzTools.PointFToPoint(_dragonDrv.SIDEList[cboCamList.SelectedIndex].ViewToWorldEx(ptLT));
                //pts.Add(PTLT);
                //Point ptLB = new Point(rect.Location.X, rect.Bottom);
                //Point PTLB = JzTools.PointFToPoint(_dragonDrv.SIDEList[cboCamList.SelectedIndex].ViewToWorldEx(ptLB));
                //pts.Add(PTLB);
                //Point ptRT = new Point(rect.Right, rect.Location.Y);
                //Point PTRT = JzTools.PointFToPoint(_dragonDrv.SIDEList[cboCamList.SelectedIndex].ViewToWorldEx(ptRT));
                //pts.Add(PTRT);
                //Point ptRB = new Point(rect.Right, rect.Bottom);
                //Point PTRB = JzTools.PointFToPoint(_dragonDrv.SIDEList[cboCamList.SelectedIndex].ViewToWorldEx(ptRB));
                //pts.Add(PTRB);
                
                Point ptCenter = JzTools.GetRectCenter(rect);
                //MOD GAARA
                Point ptCenterEx = ptCenter;// JzTools.PointFToPoint(_dragonDrv.SIDEList[cboCamList.SelectedIndex].ViewToWorldEx(ptCenter));

                ptCenterEx.X += 2;
                ptCenterEx.Y += 2;

                pts.Add(ptCenterEx);

#region TEST


                //int i = 0;
                //while (i < 100)
                //{
                //    mm_list.Add(new Point(100 + i * 10, 100));

                //    i++;
                //}
#endregion
            }
#if (OPT_JUMBO)
#region DRAW PAINT
            if (m_IxJumbo != null)
            {
                try
                {
                    m_IxJumbo.DrawPoints(pts);
                    //foreach (Point pt in pts)
                    //{
                    //    m_IxJumbo.DrawPoint(pt);
                    //}
                }
                catch
                { }
            }
#endregion
#endif
        }

        void btnGetBmp_Click(object sender, EventArgs e)
        {
            opbmp.Dispose();
            //MOD GAARA
            opbmp = new Bitmap(CCDCollection.GetBMP(INI.Allinone_Calibrate_Cam_Index, true));
            //opbmp.RotateFlip(RotateFlipType.Rotate270FlipNone);

            OPDisplay.SetDispImage(opbmp);
            //opbmp.Save(Application.StartupPath + "\\Test" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

            JZSCREENPOINTS.m_JzScreenList[cboPointsDataList.SelectedIndex].bmpORG.Dispose();
            JZSCREENPOINTS.m_JzScreenList[cboPointsDataList.SelectedIndex].bmpORG = new Bitmap(opbmp);

            bmpDISPVIEW.Dispose();
            bmpDISPVIEW = new Bitmap(opbmp);
        }


        public static bool InitialServer()
        {
#if (OPT_JUMBO)
            try
            {
                string serverpathstring = Application.StartupPath + "\\AllinOne.Jumbo.Client.config";
                m_IxJumbo = JumboNetClient.GetService(serverpathstring, "192.168.1.10");
            }
            catch(Exception ex)
            {
                return false;
            }
#endif

            return true;
        }

        void OPDisplay_SelectAction()
        {
            foreach (BASISClass basis in BasisList)
            {
                if (basis.Cell.IsSelected)
                {
                    bmp = bmpDISPVIEW.Clone(basis.Cell.RectCell, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    bmptmp = bmp;
                    opsmalldisply.SetDispImage(bmp);
                }
            }
        }

        void btnAutoFind_Click(object sender, EventArgs e)
        {
            //JzScreenItem.PointList.Clear();
            BasisList.Clear();
            Bitmap bmp = new Bitmap(1, 1);
            bmp.Dispose();

            int offsetx = 162;
            int offsety = 1180;

            if (cboPointsDataList.SelectedIndex == 5)
                bmp = (Bitmap)bmpDISPVIEW.Clone(new Rectangle(offsetx, offsety, bmpDISPVIEW.Width - 1 - 2 * offsetx, bmpDISPVIEW.Height - 1 - offsety), PixelFormat.Format24bppRgb);
            else
                bmp = (Bitmap)bmpDISPVIEW.Clone(new Rectangle(offsetx, 0, bmpDISPVIEW.Width - 1 - 2 * offsetx, bmpDISPVIEW.Height), PixelFormat.Format24bppRgb);

            HistogramClass Histogram = new HistogramClass(2);
            JzFindObjectClass FindObject = new JzFindObjectClass();

            if (!Directory.Exists(@"D:\LOA\NEWERA\"))
                Directory.CreateDirectory(@"D:\LOA\NEWERA\");

            //JzTools.SetBrightContrast(bmp, -70, 10);

            bmp.Save(@"D:\LOA\NEWERA\DiplayAutoFind-0.BMP", System.Drawing.Imaging.ImageFormat.Bmp);

            Histogram.GetHistogram(bmp);
            //FindObject.SetThreshold(bmp, JzTools.SimpleRect(bmp.Size), Histogram.MeanGrade * 6, 255, 0, true);
            FindObject.SetThreshold(bmp,
                                                            JzTools.SimpleRect(bmp.Size),
                                                            (Histogram.MinGrade < 5 ? 5 : (int)((Histogram.MaxGrade - Histogram.MinGrade) * INI.Allinone_Cam_Ratio)),
                                                            255,
                                                            0,
                                                            true);
            //FindObject.SetThreshold(bmp, JzTools.SimpleRect(bmp.Size), (Histogram.MinGrade < 5 ? 5 : Histogram.MinGrade << 3), 255, 0, true);
            bmp.Save(@"D:\LOA\NEWERA\DiplayAutoFind-1.BMP", System.Drawing.Imaging.ImageFormat.Bmp);

            FindObject.Find(bmp, Color.Red);

            foreach (FoundClass found in FindObject.FoundList)
            {
                if (found.rect.Width > 20 && found.rect.Height > 20
                    && found.rect.Width < 100 && found.rect.Height < 100
                    )
                {
                    Rectangle rect = found.rect;
                    rect.X += offsetx;

                    if (cboPointsDataList.SelectedIndex == 5)
                        rect.Y += offsety;

                    rect.Inflate(5, 5);
                    rect.Intersect(JzTools.SimpleRect(new Size(bmpDISPVIEW.Width, bmpDISPVIEW.Height)));
                    //rect.Intersect(JzTools.SimpleRect(new Size(4912, 3684)));
                    BASISClass basis = new BASISClass(rect, 0, 0);
                    BasisList.Add(basis);

                    Point ptCenter = JzTools.GetRectCenter(rect);
                    //MOD GAARA
                    Point ptCenterEx = ptCenter;// JzTools.PointFToPoint(_dragonDrv.SIDEList[cboCamList.SelectedIndex].ViewToWorldEx(ptCenter));

                    //ptCenterEx.X += INI.Allinone_Offset_ADDX;
                    //ptCenterEx.Y += INI.Allinone_Offset_ADDY;

                    //JzScreenItem.PointList.Add(ptCenterEx);
                }
            }

            bmp.Save(@"D:\LOA\NEWERA\DiplayAutoFind-2.BMP", System.Drawing.Imaging.ImageFormat.Bmp);
            bmp.Dispose();

            SyncAll();
        }
        void btnSAVE_Click(object sender, EventArgs e)
        {
            //SaveBasis();
        }
        void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ControlKey:
                    OPDisplay.MultiComplete();
                    break;
                case Keys.Menu:
                    OPDisplay.HotKeyToResize(false);
                    break;
            }
        }
        void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F7:
                    AddBasis();
                    break;
                case Keys.F8:
                    DeleteBasis();
                    break;
                case Keys.ControlKey:
                    OPDisplay.MultiBackup();
                    break;
                case Keys.Menu:
                    OPDisplay.HotKeyToResize(true);
                    break;
            }
        }

        void LoadBasis()
        {
            string loaddata = "";

            if (!File.Exists(Application.StartupPath + "\\BASIS.jdb"))
                return;

            JzTools.ReadData(ref loaddata, Application.StartupPath + "\\BASIS.jdb");

            string[] strs = loaddata.Replace(Environment.NewLine, "#").Split('#');

            foreach(string str in strs)
            {
                if (str != "")
                {
                    BASISClass basis = new BASISClass(str);

                    BasisList.Add(basis);
                }
            }
            
        }
        void SaveBasis()
        {
            string savedata = "";
            if (BasisList.Count > 0)
            {
                foreach (BASISClass basis in BasisList)
                {
                    savedata += basis.ToString() + Environment.NewLine;
                }
            }
            JzTools.SaveData(savedata, Application.StartupPath + "\\BASIS.jdb");
        }
        void AddBasis()
        {
            int i = 0;
            int Count = 0;

            List<BASISClass> OPERATEBasisList = BasisList;
            BASISClass LastOperationBasis;

            if (OPERATEBasisList.Count == 0)
            {
                BASISClass NewBasis = new BASISClass();
                //NewBasis.GroupIndex = BasisList[0].GroupIndex * 10 + BasisList[0].Index;

                OPERATEBasisList.Add(NewBasis);
                NewBasis.Cell.SetFirstSelected();
            }
            else
            {
                bool IsCopy = false;

                i = 0;
                Count = OPERATEBasisList.Count;

                while (i < Count)
                {

                    BASISClass BasisNow = OPERATEBasisList[i];
                    CellClass cell = BasisNow.Cell;

                    LastOperationBasis = OPERATEBasisList[OPERATEBasisList.Count - 1];

                    if (cell.IsSelected)
                    {
                        IsCopy = true;

                        BASISClass NewBasis = new BASISClass(BasisNow.ToString(), LastOperationBasis.Index + 1);
                        CellClass Newcell = NewBasis.Cell;

                        Newcell.TransferSelected(cell);
                        Newcell.MoveNew();

                        OPERATEBasisList.Add(NewBasis);
                    }

                    i++;
                }

                if (!IsCopy)
                {
                    LastOperationBasis = OPERATEBasisList[OPERATEBasisList.Count - 1];
                    BASISClass LastBasis = LastOperationBasis;
                    CellClass Lastcell = LastBasis.Cell;

                    BASISClass NewBasis = new BASISClass(LastBasis.ToString(), LastBasis.Index + 1);
                    CellClass NewCell = NewBasis.Cell;

                    NewCell.TransferSelected(Lastcell);
                    NewCell.SetFirstSelected();
                    NewCell.MoveNew();

                    OPERATEBasisList.Add(NewBasis);
                }
            }

            SyncAll();
        }
        void DeleteBasis()
        {
            int i = 0;

            List<BASISClass> OPERATEBasisList = BasisList;

            if (OPERATEBasisList.Count > 0)
            {
                i = OPERATEBasisList.Count - 1;

                while (i > -1)
                {
                    CellClass cell = OPERATEBasisList[i].Cell;

                    if (cell.IsSelected)
                    {
                        OPERATEBasisList[i].Suicide();
                        OPERATEBasisList.RemoveAt(i);
                    }

                    i--;
                }
            }
            SyncAll();
        }

        void SyncAll()
        {
            SyncAll(false);
        }
        void SyncAll(bool IsAll)
        {
            IsNeedToChange = false;

            OPDisplay.SetBASISList(BasisList);
            OPDisplay.FindFirstCell();
            OPDisplay.Refresh();

            IsNeedToChange = true;
        }
    }
}
