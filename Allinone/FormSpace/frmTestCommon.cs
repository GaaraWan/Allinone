using Allinone.BasicSpace;
using JzDisplay;
using JzDisplay.UISpace;
using MoveGraphLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WorldOfMoveableObjects;

namespace Allinone.FormSpace
{
    public partial class frmTestCommon : Form
    {

        DispUI m_DispUI;
        Bitmap m_bitmap = new Bitmap(1, 1);
        Rectangle m_Rectangle = new Rectangle(0, 0, 200, 200);
        MoveGraphLibrary.Mover m_Mover = new MoveGraphLibrary.Mover();
        public frmTestCommon()
        {
            InitializeComponent();
            this.Load += FrmTestCommon_Load;
            this.SizeChanged += FrmTestCommon_SizeChanged;

        }

        private void FrmTestCommon_SizeChanged(object sender, EventArgs e)
        {
            update_Display();
        }

        private void FrmTestCommon_Load(object sender, EventArgs e)
        {
            init_Display();

            m_Mover.Clear();
            JzRectEAG jzRectEAG = new JzRectEAG(Color.FromArgb(0, 120, 120, 0), m_Rectangle);
            jzRectEAG.RelateLevel = 2;
            jzRectEAG.RelateNo = 1;

            m_Mover.Add(jzRectEAG);
            m_DispUI.SetMover(m_Mover);

            btnTest.Click += BtnTest_Click;
            btnLoad.Click += BtnLoad_Click;
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            string filterstr = "PNG Files (*.png)|*.PNG|" + "JPG Files (*.png)|*.JPG|" + "BMP Files (*.bmp)|*.BMP|" + "All files (*.*)|*.*";
            string _pathfile = OpenFilePicker(filterstr, "");
            if (!string.IsNullOrEmpty(_pathfile))
            {
                this.Text = _pathfile;

                Bitmap bmpinput = new Bitmap(_pathfile);
                m_bitmap.Dispose();
                //bmpOperate = new Bitmap(bmpinput.Width, bmpinput.Height, System.Drawing.Imaging.PixelFormat.Format4bppIndexed);
                m_bitmap = (Bitmap)bmpinput.Clone();
                m_DispUI.SetDisplayImage(m_bitmap);
                bmpinput.Dispose();
            }
        }
        private string OpenFilePicker(string DefaultPath, string DefaultName)
        {
            string retStr = "";

            OpenFileDialog dlg = new OpenFileDialog();

            //dlg.Filter = "BMP Files (*.bmp)|*.BMP|" + "All files (*.*)|*.*";
            dlg.Filter = DefaultPath;
            dlg.FileName = DefaultName;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                retStr = dlg.FileName;
            }
            return retStr;
        }


        private void BtnTest_Click(object sender, EventArgs e)
        {
            if (m_bitmap == null)
                return;

            JetEazy.BasicSpace.JzToolsClass jzTools = new JetEazy.BasicSpace.JzToolsClass();
            Rectangle rect1 = new Rectangle(0, 0, 168, 168);
            GraphicalObject grob0 = m_Mover[0].Source;
            JzRectEAG jzRectEAG = (JzRectEAG)grob0;
            RectangleF rectF = jzRectEAG.RealRectangleAround(0, 0);
            rect1 = new Rectangle((int)rectF.X, (int)rectF.Y, (int)rectF.Width, (int)rectF.Height);
            jzTools.BoundRect(ref rect1, m_bitmap.Size);

            Bitmap bmp2 = (Bitmap)m_bitmap.Clone(rect1, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            EzSegDMTX IxBarcode = new EzSegDMTX();
            IxBarcode.InputImage = bmp2;
            //if (m_UseAIFromPy)
            //    IxBarcode.SetEzSeg(model);
            int iret = IxBarcode.Run();
            if (iret == 0)
            {
                lblResult.Text = "读取成功：" + IxBarcode.BarcodeStr + Environment.NewLine;
                lblResult.Text += "耗时：" + IxBarcode.ElapsedTime.ToString() + " ms";
            }
            else
                lblResult.Text = "读取失败：错误码=" + iret.ToString();


            bmp2.Dispose();
        }

        void init_Display()
        {
            m_DispUI = dispUI1;
            m_DispUI.Initial();
            m_DispUI.SetDisplayType(DisplayTypeEnum.NORMAL);
        }
        void update_Display()
        {
            m_DispUI.Refresh();
            m_DispUI.DefaultView();
        }
    }
}
