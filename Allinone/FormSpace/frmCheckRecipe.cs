using Allinone.BasicSpace;
using FreeImageAPI;
using JetEazy.BasicSpace;
using JetEazy.Interface;
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
    public partial class frmCheckRecipe : Form
    {
        IxLineScanCam m_IxAreaCam
        {
            get
            {
                return Universal.IxAreaCam;
            }
        }

        JzCheckRecipeParaClass m_CheckRecipePara = new JzCheckRecipeParaClass();

        DispUI m_DispUI;
        Bitmap m_bitmap = new Bitmap(1, 1);
        Rectangle m_Rectangle = new Rectangle(0, 0, 200, 200);
        MoveGraphLibrary.Mover m_Mover = new MoveGraphLibrary.Mover();
        Bitmap m_bmpOperate = new Bitmap(1, 1);

        PropertyGrid m_PropertyGrid;

        public string ResultParaStr
        {
            get { return m_CheckRecipePara.ToParaString(); }
        }

        public frmCheckRecipe(string str = "")
        {
            m_CheckRecipePara.FromingStr(str);

            InitializeComponent();

            this.Load += FrmCheckRecipe_Load;
            this.SizeChanged += FrmCheckRecipe_SizeChanged;
        }

        private void FrmCheckRecipe_SizeChanged(object sender, EventArgs e)
        {
            update_Display();
        }

        private void FrmCheckRecipe_Load(object sender, EventArgs e)
        {
            this.Text = "设定读码参数页面";
            init_Display();

            m_PropertyGrid = propertyGrid1;

            m_Mover.Clear();
            JzRectEAG jzRectEAG = new JzRectEAG(Color.FromArgb(0, 120, 120, 0), m_CheckRecipePara.RectF);
            jzRectEAG.RelateLevel = 2;
            jzRectEAG.RelateNo = 1;

            m_Mover.Add(jzRectEAG);
            m_DispUI.SetMover(m_Mover);

            m_PropertyGrid.SelectedObject = m_CheckRecipePara;
            m_PropertyGrid.PropertyValueChanged += M_PropertyGrid_PropertyValueChanged;

            //btnTest.Click += BtnTest_Click;
            //btnLoad.Click += BtnLoad_Click;
            //btnGetImage.Click += BtnGetImage_Click;
            //btnSetupPos.Click += BtnSetupPos_Click;

            //btnSaveExit.Click += BtnSaveExit_Click;
            //btnExit.Click += BtnExit_Click;
        }

        private void M_PropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            switch (e.ChangedItem.PropertyDescriptor.Name)
            {
                //case "nBrightness":
                //    m_CheckRecipePara.nBrightness = (int)e.ChangedItem.Value;
                //    if (m_bmpOperate != null)
                //        m_bmpOperate.Dispose();
                //    m_bmpOperate = _preImage();
                //    if (m_bmpOperate != null)
                //        m_DispUI.ReplaceDisplayImage(m_bmpOperate);

                //    break;
                //case "nContrast":
                //    m_BarcodePara.nContrast = (int)e.ChangedItem.Value;
                //    if (m_bmpOperate != null)
                //        m_bmpOperate.Dispose();
                //    m_bmpOperate = _preImage();
                //    if (m_bmpOperate != null)
                //        m_DispUI.ReplaceDisplayImage(m_bmpOperate);

                //    break;
                case "IsOpenFindCount":
                    m_CheckRecipePara.IsOpenFindCount = (bool)e.ChangedItem.Value;
                    break;
                case "nThresholdValue":
                    m_CheckRecipePara.nThresholdValue = (int)e.ChangedItem.Value;
                    break;
                case "nAreaMin":
                    m_CheckRecipePara.nAreaMin = (int)e.ChangedItem.Value;
                    break;
                case "nAreaMax":
                    m_CheckRecipePara.nAreaMax = (int)e.ChangedItem.Value;
                    break;
            }
        }


        private void btnLoad_Click(object sender, EventArgs e)
        {
            string filterstr = "PNG Files (*.png)|*.PNG|" + "JPG Files (*.png)|*.JPG|" + "BMP Files (*.bmp)|*.BMP|" + "All files (*.*)|*.*";
            string _pathfile = OpenFilePicker(filterstr, "");
            if (!string.IsNullOrEmpty(_pathfile))
            {
                //this.Text = _pathfile;

                Bitmap bmpinput = new Bitmap(_pathfile);
                m_bitmap.Dispose();
                //bmpOperate = new Bitmap(bmpinput.Width, bmpinput.Height, System.Drawing.Imaging.PixelFormat.Format4bppIndexed);
                m_bitmap = (Bitmap)bmpinput.Clone();
                m_DispUI.SetDisplayImage(m_bitmap);
                bmpinput.Dispose();
            }
        }

        private void btnGetImage_Click(object sender, EventArgs e)
        {
            string msg = "是否重新取像？";
            if (!Universal.IsNoUseIO)
                m_IxAreaCam.IsGrapImageComplete = false;
            if (VsMSG.Instance.Question(msg) != DialogResult.OK)
            {
                return;
            }


            if (Universal.IsNoUseIO)
            {
                btnLoad.PerformClick();
                return;
            }
            m_IxAreaCam.SoftTrigger();
            System.Threading.Thread.Sleep(300);

            //Universal.CCDCollection.SetExposure(m_BarcodePara.CamExpo, 0);
            //Universal.CCDCollection.GetImage();
            m_bitmap.Dispose();
            m_bitmap = new Bitmap(m_IxAreaCam.GetFreeImageBitmap().ToBitmap());

            m_DispUI.SetDisplayImage(m_bitmap);
        }

        private void btnTest_Click(object sender, EventArgs e)
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

            m_CheckRecipePara.RectF = new RectangleF(rectF.X, rectF.Y, rectF.Width, rectF.Height);
            //Bitmap bmp2 = (Bitmap)m_bitmap.Clone(rect1, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            m_PropertyGrid.SelectedObject = m_CheckRecipePara;

            int iret = m_CheckRecipePara.GetBottomCount(m_bitmap);
            if (iret > 0)
            {
                richTextBox1.Text = "读取个数：" + iret.ToString() + Environment.NewLine;
                richTextBox1.Text += "耗时：" + m_CheckRecipePara.CheckTime.ToString() + " ms";
            }
            else
                richTextBox1.Text = "读取失败" + iret.ToString();


            m_DispUI.SetDisplayImage(m_CheckRecipePara.bmpOutput);
        }

        private void btnSaveExit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
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
