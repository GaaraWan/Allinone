using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AJZReportViewer
{
    public partial class frmStripShow : Form
    {
        string m_imagePath = string.Empty;
        RectangleF m_rectF = new RectangleF();
        Color m_color = Color.Green;

        public frmStripShow(string ePathStr,RectangleF rectangleF,Color color)
        {
            m_imagePath = ePathStr;
            m_rectF = rectangleF;
            m_color = color;

            InitializeComponent();
            this.Load += FrmStripShow_Load;
            this.SizeChanged += FrmStripShow_SizeChanged;
        }

        private void FrmStripShow_SizeChanged(object sender, EventArgs e)
        {
            update_Display();
        }

        private void FrmStripShow_Load(object sender, EventArgs e)
        {
            init_Display();
            update_Display();

            this.Text = "错误位置显示";

            if (File.Exists(m_imagePath))
            {
                RectangleF[] rectangleFs = new RectangleF[1];
                rectangleFs[0] = m_rectF;

                Bitmap bitmap1 = new Bitmap(m_imagePath);
                Bitmap bitmap2 = new Bitmap(bitmap1);
                bitmap1.Dispose();
                Graphics g = Graphics.FromImage(bitmap2);
                g.DrawRectangles(new Pen(m_color, 9), rectangleFs);
                g.Dispose();

                DS.SetDisplayImage(bitmap2);
                bitmap2.Dispose();
                //bitmap2.Save("D:\\viewer.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            }

            this.WindowState = FormWindowState.Maximized;
        }

        void init_Display()
        {
            DS.Initial(100, 0.01f);
            DS.SetDisplayType(JzDisplay.DisplayTypeEnum.NORMAL);
        }
        void update_Display(bool eChangeToDefault = true)
        {
            DS.Refresh();
            if (eChangeToDefault)
                DS.DefaultView();
        }
    }
}
