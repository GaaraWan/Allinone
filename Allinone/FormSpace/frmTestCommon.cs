using Allinone.BasicSpace;
using Allinone.ControlSpace;
using JetEazy.BasicSpace;
using JetEazy.PlugSpace.BarcodeEx;
using JzDisplay;
using JzDisplay.UISpace;
using MoveGraphLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WorldOfMoveableObjects;

namespace Allinone.FormSpace
{
    public partial class frmTestCommon : Form
    {

        JzBarcodeParaGridClass m_BarcodePara = new JzBarcodeParaGridClass();

        DispUI m_DispUI;
        Bitmap m_bitmap = new Bitmap(1, 1);
        Rectangle m_Rectangle = new Rectangle(0, 0, 200, 200);
        MoveGraphLibrary.Mover m_Mover = new MoveGraphLibrary.Mover();
        Bitmap m_bmpOperate = new Bitmap(1, 1);

        MachineCollectionClass MACHINECollection
        {
            get
            {
                return Universal.MACHINECollection;
            }
        }

        public string ResultParaStr
        {
            get { return m_BarcodePara.ToParaString(); }
        }

        PropertyGrid m_PropertyGrid;
        public frmTestCommon(string str = "")
        {
            m_BarcodePara.FromingStr(str);

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
            this.Text = "设定读码参数页面";
            init_Display();

            m_PropertyGrid = propertyGrid1;

            m_Mover.Clear();
            JzRectEAG jzRectEAG = new JzRectEAG(Color.FromArgb(0, 120, 120, 0), m_BarcodePara.RectF);
            jzRectEAG.RelateLevel = 2;
            jzRectEAG.RelateNo = 1;

            m_Mover.Add(jzRectEAG);
            m_DispUI.SetMover(m_Mover);

            m_PropertyGrid.SelectedObject = m_BarcodePara;
            m_PropertyGrid.PropertyValueChanged += M_PropertyGrid_PropertyValueChanged;

            btnTest.Click += BtnTest_Click;
            btnLoad.Click += BtnLoad_Click;
            btnGetImage.Click += BtnGetImage_Click;
            btnSetupPos.Click += BtnSetupPos_Click;
            btnGoSetupPos.Click += BtnGoSetupPos_Click;

            btnSaveExit.Click += BtnSaveExit_Click;
            btnExit.Click += BtnExit_Click;
        }

        private void BtnGoSetupPos_Click(object sender, EventArgs e)
        {
            string msg = "是否运行至拍照位置？";

            if (VsMSG.Instance.Question(msg) != DialogResult.OK)
            {
                return;
            }
            string _pos = m_BarcodePara.MotorPositionStr.Replace(";", ",");
            MACHINECollection.GoPosition(_pos);

        }

        private void M_PropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            switch (e.ChangedItem.PropertyDescriptor.Name)
            {
                case "nBrightness":
                    m_BarcodePara.nBrightness = (int)e.ChangedItem.Value;
                    if (m_bmpOperate != null)
                        m_bmpOperate.Dispose();
                    m_bmpOperate = _preImage();
                    if (m_bmpOperate != null)
                        m_DispUI.ReplaceDisplayImage(m_bmpOperate);

                    break;
                case "nContrast":
                    m_BarcodePara.nContrast = (int)e.ChangedItem.Value;
                    if (m_bmpOperate != null)
                        m_bmpOperate.Dispose();
                    m_bmpOperate = _preImage();
                    if (m_bmpOperate != null)
                        m_DispUI.ReplaceDisplayImage(m_bmpOperate);

                    break;
                case "IsOpenBarcode":
                    m_BarcodePara.IsOpenBarcode = (bool)e.ChangedItem.Value;
                    break;
                case "CamExpo":
                    m_BarcodePara.CamExpo = (float)e.ChangedItem.Value;
                    break;
                case "CamExpoCount":
                    m_BarcodePara.CamExpoCount = (int)e.ChangedItem.Value;
                    break;
                case "CamExpoOffset":
                    m_BarcodePara.CamExpoOffset = (float)e.ChangedItem.Value;
                    break;
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void BtnSaveExit_Click(object sender, EventArgs e)
        {
            //JetEazy.BasicSpace.JzToolsClass jzTools = new JetEazy.BasicSpace.JzToolsClass();
            //Rectangle rect1 = new Rectangle(0, 0, 168, 168);
            //GraphicalObject grob0 = m_Mover[0].Source;
            //JzRectEAG jzRectEAG = (JzRectEAG)grob0;
            //RectangleF rectF = jzRectEAG.RealRectangleAround(0, 0);
            //rect1 = new Rectangle((int)rectF.X, (int)rectF.Y, (int)rectF.Width, (int)rectF.Height);
            //jzTools.BoundRect(ref rect1, m_bitmap.Size);

            //m_BarcodePara.RectF = new RectangleF(rectF.X, rectF.Y, rectF.Width, rectF.Height);

            this.DialogResult = DialogResult.OK;
        }

        private void BtnSetupPos_Click(object sender, EventArgs e)
        {
            string msg = "是否将当前位置设为读码拍照位置？";

            if (VsMSG.Instance.Question(msg) != DialogResult.OK)
            {
                return;
            }

            string str = MACHINECollection.GetPosition();
            m_BarcodePara.MotorPositionStr = str.Replace(",", ";");
            m_PropertyGrid.SelectedObject = m_BarcodePara;
        }

        private void BtnGetImage_Click(object sender, EventArgs e)
        {
            string msg = "是否重新取像？";
            if (VsMSG.Instance.Question(msg) != DialogResult.OK)
            {
                return;
            }


            if(Universal.IsNoUseIO)
            {
                btnLoad.PerformClick();
                return;
            }
            Universal.CCDCollection.SetExposure(m_BarcodePara.CamExpo, 0);
            Universal.CCDCollection.GetImage();
            m_bitmap.Dispose();
            m_bitmap = Universal.CCDCollection.GetBMP(0, false);

            m_DispUI.SetDisplayImage(m_bitmap);
        }

        private void BtnLoad_Click(object sender, EventArgs e)
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

            m_BarcodePara.RectF = new RectangleF(rectF.X, rectF.Y, rectF.Width, rectF.Height);
            Bitmap bmp2 = (Bitmap)m_bitmap.Clone(rect1, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            m_PropertyGrid.SelectedObject = m_BarcodePara;

            Mvd2DReaderClass mvd2DReader = new Mvd2DReaderClass();
            mvd2DReader.Run(bmp2, new RectangleF(0, 0, bmp2.Width, bmp2.Height));

            if (string.IsNullOrEmpty(mvd2DReader.xBarcodeStr))
            {
                AForge.Imaging.Filters.ContrastCorrection contrast = new AForge.Imaging.Filters.ContrastCorrection(m_BarcodePara.nContrast);
                bmp2 = contrast.Apply(bmp2);
                AForge.Imaging.Filters.BrightnessCorrection brightness = new AForge.Imaging.Filters.BrightnessCorrection(m_BarcodePara.nBrightness);
                bmp2 = brightness.Apply(bmp2);

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
            }
            else
            {
                lblResult.Text = "读取成功：" + mvd2DReader.xBarcodeStr + Environment.NewLine;
            }
            bmp2.Dispose();
            if (mvd2DReader != null)
            {
                mvd2DReader.Dispose();
                mvd2DReader = null;
            }

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

        Bitmap _preImage()
        {
            if (m_bitmap == null)
                return null;

            //JetEazy.BasicSpace.JzToolsClass jzTools = new JetEazy.BasicSpace.JzToolsClass();
            //Rectangle rect1 = new Rectangle(0, 0, 168, 168);
            //GraphicalObject grob0 = m_Mover[0].Source;
            //JzRectEAG jzRectEAG = (JzRectEAG)grob0;
            //RectangleF rectF = jzRectEAG.RealRectangleAround(0, 0);
            //rect1 = new Rectangle((int)rectF.X, (int)rectF.Y, (int)rectF.Width, (int)rectF.Height);
            //jzTools.BoundRect(ref rect1, m_bitmap.Size);

            //m_BarcodePara.RectF = new RectangleF(rectF.X, rectF.Y, rectF.Width, rectF.Height);
            //Bitmap bmp2 = (Bitmap)m_bitmap.Clone(rect1, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            //m_PropertyGrid.SelectedObject = m_BarcodePara;

            //AForge.Imaging.Filters.ContrastCorrection contrast = new AForge.Imaging.Filters.ContrastCorrection(m_BarcodePara.nContrast);
            //bmp2 = contrast.Apply(bmp2);
            //AForge.Imaging.Filters.BrightnessCorrection brightness = new AForge.Imaging.Filters.BrightnessCorrection(m_BarcodePara.nBrightness);
            //bmp2 = brightness.Apply(bmp2);

            Bitmap bmp2 = new Bitmap(m_bitmap);

            AForge.Imaging.Filters.ContrastCorrection contrast = new AForge.Imaging.Filters.ContrastCorrection(m_BarcodePara.nContrast);
            bmp2 = contrast.Apply(bmp2);
            AForge.Imaging.Filters.BrightnessCorrection brightness = new AForge.Imaging.Filters.BrightnessCorrection(m_BarcodePara.nBrightness);
            bmp2 = brightness.Apply(bmp2);

            return bmp2;
        }
    }
}
