using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace AJZReportViewer
{
    public partial class AllinoneViewerUI : UserControl
    {
        private string m_imagepath = @"D:\report\work\Image";
        private string m_path = @"D:\report\work\auto";
        private string m_wholeImage = "";
        public string ReportPath
        {
            get { return m_path; }
            set { m_path = value; }
        }
        public string WholeImage
        {
            get { return m_wholeImage; }
        }
        Label lblHeadStr;
        Panel pnlViewer;

        public AllinoneViewerUI()
        {
            InitializeComponent();
            InitUI();
        }
        void InitUI()
        {
            lblHeadStr = label1;
            pnlViewer = panel1;
        }

        private string m_lot = "";
        private int m_count = 0;
        private string m_filename = "";
        private string m_date = "";

        public string Date
        {
            get { return m_date; }
            set { m_date = value; }
        }
        public string ReportLot
        {
            get { return m_lot; }
            set { m_lot = value; }
        }
        public int ReportCount
        {
            get { return m_count; }
            set { m_count = value; }
        }
        public string ReportFilename
        {
            get { return m_filename; }
            set { m_filename = value; }
        }

        public void Set(string epathfilename, int eCount)
        {
            if (!File.Exists(epathfilename))
                return;

            m_count = eCount;

            string[] strs = epathfilename.Split('\\');

            if (strs.Length == 7)
            {
                m_date = strs[strs.Length - 3];
            }
            else
            {
                m_date = string.Empty;
            }

            m_filename = strs[strs.Length - 1].Replace(".csv", "");
            m_lot = strs[strs.Length - 2];


            m_wholeImage = m_imagepath + "\\" + m_lot + "\\" + m_filename.Split('-')[2] + "\\000\\result.jpg";

            if (!string.IsNullOrEmpty(m_date))
            {
                m_wholeImage = m_imagepath + "\\" + m_date + "\\" + m_lot + "\\" + m_filename.Split('-')[2] + "\\000\\result.jpg";
            }
        }

        public void UpdateViewer()
        {
            lblHeadStr.Text = Environment.NewLine;
            lblHeadStr.Text += "批号:" + "" + Environment.NewLine;
            lblHeadStr.Text += Environment.NewLine;
            lblHeadStr.Text += "总数:" + "" + Environment.NewLine;
            lblHeadStr.Text += Environment.NewLine;
            lblHeadStr.Text += "档案名称:" + "" + Environment.NewLine;
            lblHeadStr.Text += Environment.NewLine;

            pnlViewer.Controls.Clear();

            string _path_filename = m_path + "\\" + m_lot + "\\" + m_filename + ".csv";
            if (!string.IsNullOrEmpty(m_date))
            {
                _path_filename = m_path + "\\" + m_date + "\\" + m_lot + "\\" + m_filename + ".csv";
            }


            if (!System.IO.File.Exists(_path_filename))
                return;

            lblHeadStr.Text = Environment.NewLine;
            lblHeadStr.Text += "批号:" + m_lot + Environment.NewLine;
            lblHeadStr.Text += Environment.NewLine;
            lblHeadStr.Text += "总数:" + m_count.ToString() + Environment.NewLine;
            lblHeadStr.Text += Environment.NewLine;
            lblHeadStr.Text += "档案名称:" + m_filename + Environment.NewLine;
            lblHeadStr.Text += Environment.NewLine;

            #region CSV FILL

            int irow = 1;
            int icol = 1;
            Point pttemp = new Point(1, 1);
            StreamReader sr = new StreamReader(_path_filename, Encoding.Default);
            while (!sr.EndOfStream)
            {
                string Str = sr.ReadLine();
                string[] Strs = Str.Split(',');
                if (Strs.Length >= 6)
                {

                    Label lbl = new Label();
                    lbl.Name = Strs[0];
                    Point pt= new Point(int.Parse(Strs[1]), int.Parse(Strs[2]));
                    lbl.Location = new Point(int.Parse(Strs[1]), int.Parse(Strs[2]));
                    lbl.Size=new Size(int.Parse(Strs[3]), int.Parse(Strs[4]));
                    lbl.BackColor = _getColor(int.Parse(Strs[5]));

                    if (Strs.Length > 10)
                    {
                        string strtag = Strs[7] + "," + Strs[8] + "," + Strs[9] + "," + Strs[10] + "," + Strs[11] + "," + Strs[5];
                        if (Strs.Length > 12)
                            strtag = Strs[7] + "," + Strs[8] + "," + Strs[9] + "," + Strs[10] + "," + Strs[11] + "," + Strs[5] + "," + Strs[12];
                        lbl.Tag = strtag;

                        lbl.DoubleClick += Lbl_DoubleClick;
                    }

                        //(Strs[5] == "1" ? Color.Red : Color.Green);

                    if (pt.Y != pttemp.Y && pttemp.Y > 0)
                    {
                        pttemp.Y = pt.Y;
                        if (pt.Y > 5)
                            irow++;
                        icol = 1;
                    }
                    else
                    {
                        icol++;
                    }

                    lbl.Text = irow.ToString() + "-" + icol.ToString();

                    pnlViewer.Controls.Add(lbl);

                }
            }

            sr.Close();
            sr.Dispose();

            #endregion

        }
        frmStripShow m_ShowStrip = null;
        private void Lbl_DoubleClick(object sender, EventArgs e)
        {
            Label label = (Label)sender;
            string str = label.Tag as string;
            if (string.IsNullOrEmpty(str))
                return;

            string[] vs = str.Split(',');
            int pageno = int.Parse(vs[0]);
            RectangleF rectangleF = new RectangleF(float.Parse(vs[1]),
                float.Parse(vs[2]),
                float.Parse(vs[3]),
                float.Parse(vs[4]));

            Color color = _getColor(int.Parse(vs[5]));
            string _2dBarcode = string.Empty;
            if (vs.Length > 6)
                _2dBarcode = vs[6];

            //加载图片路径

            //RectangleF[] rectangleFs = new RectangleF[1];
            //rectangleFs[0] = rectangleF;

            string _imagepath = m_imagepath + "\\" + m_lot + "\\" + m_filename.Split('-')[2] + "\\000\\P00-" + pageno.ToString("000") + ".png";

            if (!string.IsNullOrEmpty(m_date))
            {
                _imagepath = m_imagepath + "\\" + m_date + "\\" + m_lot + "\\" + m_filename.Split('-')[2] + "\\000\\P00-" + pageno.ToString("000") + ".png";

            }

            if (File.Exists(_imagepath))
            {
                m_ShowStrip = new frmStripShow(_imagepath, rectangleF, color, _2dBarcode);
                m_ShowStrip.ShowDialog();
                //Bitmap bitmap1 = new Bitmap(_imagepath);
                //Bitmap bitmap2 = new Bitmap(bitmap1);
                //bitmap1.Dispose();
                //Graphics g = Graphics.FromImage(bitmap2);
                //g.DrawRectangles(new Pen(Color.Red, 3), rectangleFs);
                //g.Dispose();

                //bitmap2.Save("D:\\viewer.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }

        Color _getColor(int eIndex)
        {
            Color eColor = Color.Green;
            switch(eIndex)
            {
                case 1:
                    eColor = Color.Cyan;
                    break;
                case 2:
                    eColor = Color.Violet;
                    break;
                case 3:
                    eColor = Color.Yellow;
                    break;
                case 4:
                    eColor = Color.Red;
                    break;
                case 5:
                    eColor = Color.Purple;
                    break;
                case 6:
                    eColor = Color.Blue;
                    break;
                case 7:
                    eColor = Color.Orange;
                    break;
                case 8:
                    eColor = Color.Fuchsia;
                    break;
                case 9:
                    eColor = Color.LightPink;
                    break;
                default:
                    eColor = Color.Green;
                    break;
            }
            return eColor;
        }
        int _getColorIndex(Color eColor)
        {
            int iret = 0;
            if (eColor == Color.Cyan)
            {
                iret = 1;
            }
            else if (eColor == Color.Violet)
            {
                iret = 2;
            }
            else if (eColor == Color.Yellow)
            {
                iret = 3;
            }
            else if (eColor == Color.Red)
            {
                iret = 4;
            }
            else if (eColor == Color.Purple)
            {
                iret = 5;
            }
            else if (eColor == Color.Blue)
            {
                iret = 6;
            }
            return iret;
        }

    }
}
