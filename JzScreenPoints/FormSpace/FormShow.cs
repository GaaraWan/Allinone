using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JzScreenPoints
{
    public partial class FormShow : Form
    {
        public FormShow(int i)
        {
            InitializeComponent();
            _Init(i);
        }
        public FormShow(int i,Point m_ptstart,Point m_ptend,int m_ioffsetx,int m_ioffsety)
        {
            InitializeComponent();
            _Init(i);

            ptStart = m_ptstart;
            ptEnd = m_ptend;
            ioffsetx = m_ioffsetx;
            ioffsety = m_ioffsety;

            this.Text = "Allinone.Paint";

            _calibrate();
        }

        #region Calibrate Points
        Point ptStart = new Point(0, 0);
        Point ptEnd = new Point(100, 100);
        int ioffsetx = 10;
        int ioffsety = 10;
        #endregion

        ContextMenuStrip m_cMenu_Ptsize;
        List<Rectangle> m_Rect_All = new List<Rectangle>();
        SolidBrush sb = new SolidBrush(Color.White);
        Point pt_Mouse_location = new Point();
        List<Point> pt_Mouse_location_list = new List<Point>();
        Label lblMyMousePoint;
        TextBox txtInupt;
        PtSizeEnum m_ptsize = PtSizeEnum.PtSize_3X3;

        Timer t;

        void _Init(int iScreenIndex)
        {
            if (iScreenIndex >= Screen.AllScreens.Length)
                iScreenIndex = Screen.AllScreens.Length - 1;

            this.Size = new System.Drawing.Size(Screen.AllScreens[iScreenIndex].Bounds.Width, Screen.AllScreens[iScreenIndex].Bounds.Height);
            this.BackColor = Color.Black;

            this.Text = "MyPaint";

            m_cMenu_Ptsize = new ContextMenuStrip();
            int i = 0;
            while (i < (int)PtSizeEnum.COUNT)
            {
                ToolStripMenuItem m_tool = new ToolStripMenuItem();
                m_tool.Text = ((PtSizeEnum)i).ToString();
                m_tool.Tag = (PtSizeEnum)i;
                m_tool.Click += new EventHandler(m_tool_Click);
                m_cMenu_Ptsize.Items.Add(m_tool);

                i++;
            }
            m_cMenu_Ptsize.Visible = false;

            lblMyMousePoint = new Label();
            lblMyMousePoint.AutoSize = false;
            lblMyMousePoint.Size = new Size(400, 30);
            //lblMyMousePoint.BorderStyle = BorderStyle.Fixed3D;
            lblMyMousePoint.BackColor = Color.Black;
            lblMyMousePoint.ForeColor = Color.Lime;
            //lblMyMousePoint.Visible = false;
            lblMyMousePoint.Font = new Font("宋体", 20);
            this.Controls.Add(lblMyMousePoint);

            txtInupt = textBox1;
            txtInupt.KeyPress += new KeyPressEventHandler(txtInupt_KeyPress);
            this.MouseClick += new MouseEventHandler(FormShow_MouseClick);
            this.DoubleClick += new EventHandler(FormShow_DoubleClick);
            this.MouseMove += new MouseEventHandler(FormShow_MouseMove);

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(FormShow_KeyDown);

            PointsVisable = m_pointsvisable;
            m_pointsvisable = !m_pointsvisable;
            t = new Timer();
            t.Interval = 100;
            t.Enabled = true;
            t.Tick += T_Tick;
        }

        bool IsFirst = false;

        private void T_Tick(object sender, EventArgs e)
        {
            if (Screen.AllScreens.Length > 1)
            {
                if (!IsFirst)
                {
                    IsFirst = true;
                    this.Location = new Point(Screen.AllScreens[0].Bounds.Width, 0);
                    this.Size = new System.Drawing.Size(Screen.AllScreens[1].Bounds.Width, Screen.AllScreens[1].Bounds.Height);
                    this.BackColor = Color.Black;
                }
            }
            else
                IsFirst = false;
        }

        void txtInupt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                Rectangle rect;

                pt_Mouse_location.X = int.Parse(txtInupt.Text.Split(',')[0]);
                pt_Mouse_location.Y = int.Parse(txtInupt.Text.Split(',')[1]);

                rect = Tools.SimpleRect(pt_Mouse_location);
                rect.Inflate(2, 2);
                m_Rect_All.Add(rect);
                FillDisplay();
            }
        }
        void FormShow_MouseMove(object sender, MouseEventArgs e)
        {
            pt_Mouse_location = e.Location;
            lblMyMousePoint.Location = new Point(pt_Mouse_location.X + 20, pt_Mouse_location.Y);
            lblMyMousePoint.Text = "(X=" + pt_Mouse_location.X + ",Y=" + pt_Mouse_location.Y + ")";
        }

        private void _calibrate()
        {
            pt_Mouse_location_list.Clear();
            
            for (int i = ptStart.Y; i < ptEnd.Y; i += ioffsety)
            {
                for (int j = ptStart.X; j < ptEnd.X; j += ioffsetx)
                {
                    Point pttmp = new Point(j, i);
                    pt_Mouse_location_list.Add(pttmp);
                }
            }
            _drawpointsList(PtSizeEnum.PtSize_3X3);

        }
        private bool PointsVisable
        {
            set
            {
                txtInupt.Visible = value;
                lblMyMousePoint.Visible = value;
            }
        }
        private bool m_pointsvisable = false;
        void FormShow_KeyDown(object sender, KeyEventArgs e)
        {
            //清除最後一個點位
            if (e.Control && e.KeyCode == Keys.Z)
            {
                if (m_Rect_All.Count > 0)
                {
                    m_Rect_All.RemoveAt(m_Rect_All.Count - 1);
                    FillDisplay();
                }
            }
            //else if (e.Control && e.KeyCode == Keys.A)
            //{
            //    pt_Mouse_location_list.Clear();
            //    Point ptStart = new Point(20, 20);
            //    Point ptEnd = new Point(1000, 1000);
            //    int ioffsetx = 20;
            //    int ioffsety = 40;
            //    for (int i = ptStart.Y; i < ptEnd.Y; i += ioffsety)
            //    {
            //        for (int j = ptStart.X; j < ptEnd.X; j += ioffsetx)
            //        {
            //            Point pttmp = new Point(j, i);
            //            pt_Mouse_location_list.Add(pttmp);
            //        }
            //    }
            //    _drawpointsList(PtSizeEnum.PtSize_5X5);
            //}
            else if(e.Control && e.KeyCode == Keys.C)
            {
                if (m_Rect_All.Count > 0)
                {
                    m_Rect_All.Clear();
                    FillDisplay();
                }
            }
            else if (e.Alt && e.Control && e.KeyCode == Keys.S)
            {
                PointsVisable = m_pointsvisable;
                m_pointsvisable = !m_pointsvisable;
            }
            else if (e.Alt && e.Control && e.KeyCode == Keys.R)
            {
                _calibrate();
            }
        }
        void m_tool_Click(object sender, EventArgs e)
        {
            PtSizeEnum tag = (PtSizeEnum)((ToolStripMenuItem)sender).Tag;
            m_ptsize = tag;
            _drawpoints(m_ptsize);
        }
        private void _drawpoints(PtSizeEnum ptSize)
        {
            Rectangle rect;
            switch (ptSize)
            {
                case PtSizeEnum.PtBk_Color:

                    ColorDialog colordlg = new ColorDialog();
                    colordlg.Color = this.BackColor;
                    if (colordlg.ShowDialog() == DialogResult.OK)
                        sb = new SolidBrush(colordlg.Color);
                        //this.BackColor = colordlg.Color;

                    break;
                case PtSizeEnum.PtSize_5X5:

                    rect = Tools.SimpleRect(pt_Mouse_location);
                    rect.Inflate(2, 2);
                    m_Rect_All.Add(rect);
                    FillDisplay();

                    break;
                case PtSizeEnum.PtSize_11X11:

                    rect = Tools.SimpleRect(pt_Mouse_location);
                    rect.Inflate(5, 5);
                    m_Rect_All.Add(rect);
                    FillDisplay();

                    break;

                case PtSizeEnum.PtSize_1X1:

                    rect = Tools.SimpleRect(pt_Mouse_location);
                    m_Rect_All.Add(rect);
                    FillDisplay();

                    break;
                //case PtSizeEnum.PtSize_2X2:
                //    break;
                case PtSizeEnum.PtSize_3X3:
                    rect = Tools.SimpleRect(pt_Mouse_location);
                    rect.Inflate(1, 1);
                    m_Rect_All.Add(rect);
                    FillDisplay();
                    break;

            }
        }
        /// <summary>
        /// 畫出傳過來list點
        /// </summary>
        /// <param name="ptSize">點的尺寸</param>
        private void _drawpointsList(PtSizeEnum ptSize)
        {
            m_Rect_All.Clear();//清空所有點位，重置

            Rectangle rect;
            switch (ptSize)
            {
                case PtSizeEnum.PtBk_Color:

                    ColorDialog colordlg = new ColorDialog();
                    colordlg.Color = this.BackColor;
                    if (colordlg.ShowDialog() == DialogResult.OK)
                    {
                        sb = new SolidBrush(colordlg.Color);
                        m_ptsize = PtSizeEnum.PtSize_5X5;
                    }
                    //this.BackColor = colordlg.Color;

                    break;
                case PtSizeEnum.PtSize_5X5:

                    foreach (Point pts in pt_Mouse_location_list)
                    {
                        rect = Tools.SimpleRect(pts);
                        rect.Inflate(2, 2);
                        m_Rect_All.Add(rect);
                    }
                    
                    FillDisplay();

                    break;
                case PtSizeEnum.PtSize_11X11:

                    foreach (Point pts in pt_Mouse_location_list)
                    {
                        rect = Tools.SimpleRect(pts);
                        rect.Inflate(5, 5);
                        m_Rect_All.Add(rect);
                    }
                    
                    FillDisplay();

                    break;

                case PtSizeEnum.PtSize_1X1:

                    foreach (Point pts in pt_Mouse_location_list)
                    {
                        rect = Tools.SimpleRect(pts);
                        //rect.Inflate(5, 5);
                        m_Rect_All.Add(rect);
                    }
                    
                    FillDisplay();

                    break;
                //case PtSizeEnum.PtSize_2X2:
                //    break;
                case PtSizeEnum.PtSize_3X3:

                    foreach (Point pts in pt_Mouse_location_list)
                    {
                        rect = Tools.SimpleRect(pts);
                        rect.Inflate(1, 1);
                        m_Rect_All.Add(rect);
                    }
                    
                    FillDisplay();

                    break;

            }
        }
        void FormShow_DoubleClick(object sender, EventArgs e)
        {
            this.Close();
        }
        void FormShow_MouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case System.Windows.Forms.MouseButtons.Right:

                    //pt_Mouse_location = e.Location;
                    this.m_cMenu_Ptsize.Show(this, e.Location);
                    this.m_cMenu_Ptsize.Visible = true;

                    break;
            }
        }
        public void DrawMyPaints(List<Point> m_ptlist)
        {
            pt_Mouse_location_list = m_ptlist;
            _drawpointsList(m_ptsize);
        }
        public void DrawMyPaint(Point pt)
        {
            pt_Mouse_location = pt;
            _drawpoints(m_ptsize);
        }
        public void DrawMyPaintRect(Rectangle rect)
        {
            m_Rect_All.Add(rect);
            FillDisplay();
        }
        public void DrawMyPaintRectS(List<Rectangle> m_rectlist)
        {
            m_Rect_All.Clear();
            foreach (Rectangle rect in m_rectlist)
            {
                m_Rect_All.Add(rect);
            }
            FillDisplay();
        }
        /// <summary>
        /// 刷新所有點介面
        /// </summary>
        private void FillDisplay()
        {
            //建立與屏幕一樣大小的圖片
            Bitmap BMP = new Bitmap(this.Width, this.Height);
            Graphics g = Graphics.FromImage(BMP);
            g.Clear(Color.Black);
            Rectangle[] rects = new Rectangle[m_Rect_All.Count];
            //所有填充矩形框的點
            foreach (Rectangle _rect in m_Rect_All)
                g.FillRectangle(sb, _rect);
            //保存顯示圖片
            if (!System.IO.Directory.Exists("D:\\LOA"))
                System.IO.Directory.CreateDirectory("D:\\LOA");
            BMP.Save(@"D:\LOA\Screen.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            //背景顯示
            Bitmap bmpbackimage = new Bitmap(BMP);
            this.BackgroundImage = bmpbackimage;
            BMP.Dispose();
            //this.Refresh();
        }
    }
}
