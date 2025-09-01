using Allinone.FormSpace.BasicPG;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Allinone.FormSpace.PADForm.NoHaveInspect
{
    public partial class N2UI : UserControl
    {
        bool mCropRegion = false;
        Bitmap bmpOperate = new Bitmap(1, 1);
        N2Class n2Class = new N2Class();

        public N2UI()
        {
            InitializeComponent();

            init_Display();
            update_Display();

            SizeChanged += N1_SizeChanged;

            btnLoadImage.Click += BtnLoadImage_Click;
            btnTest.Click += BtnTest_Click;

        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            mCropRegion = !mCropRegion;
            btnTest.BackColor = (mCropRegion ? Color.Red : Color.FromArgb(128, 255, 128));
        }

        private void BtnLoadImage_Click(object sender, EventArgs e)
        {
            string _file = OpenFilePicker("", "");
            if (!string.IsNullOrEmpty(_file))
            {
                bmpOperate.Dispose();
                bmpOperate = new Bitmap(_file);

                DS1.ReplaceDisplayImage(bmpOperate);
            }
        }

        OPSpace.AnalyzeClass analyze;
        string DefaultString = "";

        public void Initial(string str)
        {
            if (Allinone.FormSpace.MainForm.DETAILFRM.PAGEUI.PageNow != null)
            {
                analyze = Allinone.FormSpace.MainForm.DETAILFRM.PAGEUI.AnalyzeSelectNow;
                analyze.ResetTrainStatus();
                analyze.ResetRunStatus();
                analyze.Z02_CreateTrainRequirement(Allinone.FormSpace.MainForm.DETAILFRM.PAGEUI.PageNow.GetbmpORG(), new PointF(0, 0));
                analyze.Z05_AlignTrainProcess();
            }
            else
                analyze = Allinone.FormSpace.MainForm.DETAILFRM.PAGEUI.AnalyzeRootNow;


            SetValue(str);
            pg.SelectedObject = n2Class;

            //bmpBitmap = new Bitmap(analyze.bmpPATTERN);
            //pbx1.Image = bmpBitmap;

            //IsNeedChange = true;
        }
        public string GetDataValueString()
        {
            string retstring = "";
            //N1Class n1 = new N1Class();
            retstring = n2Class.ToParaString();
            //retstring = numGammaCorrelation.Value.ToString() + ",";
            //retstring += numBlurCount.Value.ToString() + ",";
            //retstring += numholesratio.Value.ToString() + ",";
            //retstring += numRangeRatio.Value.ToString() + ",";
            //retstring += numleft.Value.ToString() + ",";
            //retstring += numright.Value.ToString() + ",";
            //retstring += numtop.Value.ToString() + ",";
            //retstring += numbottom.Value.ToString() + ",";
            //retstring += (chkisneedclose.Checked ? "1" : "0");

            return retstring;
        }
        void SetValue(string str)
        {
            //string[] strs = str.Split(',');

            try
            {
                n2Class.FromString(str);
            }
            catch (Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
                SetValue(DefaultString);
            }
        }

        private void N1_SizeChanged(object sender, EventArgs e)
        {
            update_Display();
        }

        void init_Display()
        {
            DS1.Initial(100, 0.01f);
            DS1.SetDisplayType(JzDisplay.DisplayTypeEnum.NORMAL);
            DS1.CaptureAction += DS1_CaptureAction;
            //DS2.Initial(100, 0.01f);
            //DS2.SetDisplayType(JzDisplay.DisplayTypeEnum.SHOW);
            //DS3.Initial(100, 0.01f);
            //DS3.SetDisplayType(JzDisplay.DisplayTypeEnum.SHOW);
        }

        private void DS1_CaptureAction(RectangleF rectf)
        {
            //GraphicalObject grobj = myMover[0].Source;
            RectangleF rectf_org = new RectangleF(10, 10, 10, 10);
            RectangleF rectf_des = new RectangleF(10, 10, 10, 10);

            if (mCropRegion)
            {
                BoundRect(ref rectf, bmpOperate.Size);
                if (rectf.Width > 1 && rectf.Height > 1)
                {
                    //txtBangBangRectStr.Text = $"{rectf.X};{rectf.Y};{rectf.Width};{rectf.Height}";
                    mCropRegion = false;
                    //SetBTNBK(button1, IsCaptureBBRegion);
                    //checkIPD();

                    Bitmap bmp0 = bmpOperate.Clone(rectf, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    bool bOK = analyze.PADPara.checkNoHaveRunBlobCount(bmp0, n2Class.ThresholdValue, n2Class.IsWhite, n2Class.Count);
                    label1.Text = $"结果:{(bOK ? "有芯片" : "无芯片")}";
                    label1.BackColor = (bOK ? Color.Lime : Color.Red);

                    btnTest.BackColor = (mCropRegion ? Color.Red : Color.FromArgb(128, 255, 128));
                }
            }
            //else if (IsCaptureOutRegion)
            //{
            //    BoundRect(ref rectf, bmpBitmap.Size);
            //    if (rectf.Width > 1 && rectf.Height > 1)
            //    {
            //        txtNeverOutsideRect.Text = $"{rectf.X};{rectf.Y};{rectf.Width};{rectf.Height}";
            //        IsCaptureOutRegion = false;
            //        SetBTNBK(button2, IsCaptureOutRegion);
            //        checkIPD();

            //    }
            //}
        }

        void update_Display()
        {
            DS1.Refresh();
            DS1.DefaultView();
            //DS2.Refresh();
            //DS2.DefaultView();
            //DS3.Refresh();
            //DS3.DefaultView();
        }
        public void BoundRect(ref Rectangle InnerRect, Size BoundSize)
        {
            InnerRect.X = Math.Min(Math.Max(InnerRect.X, 0), (BoundSize.Width - InnerRect.Width < 0 ? 0 : BoundSize.Width - InnerRect.Width));
            InnerRect.Y = Math.Min(Math.Max(InnerRect.Y, 0), (BoundSize.Height - InnerRect.Height < 0 ? 0 : BoundSize.Height - InnerRect.Height));

            if (BoundSize.Width <= InnerRect.X + InnerRect.Width)
                InnerRect.Width = BoundValue(InnerRect.Width, BoundSize.Width - InnerRect.X, 1);
            if (BoundSize.Height <= InnerRect.Height + InnerRect.Height)
                InnerRect.Height = BoundValue(InnerRect.Height, BoundSize.Height - InnerRect.Y, 1);
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
        public int BoundValue(int Value, int Max, int Min)
        {
            return Math.Max(Math.Min(Value, Max), Min);

        }
        public float BoundValue(float Value, float Max, float Min)
        {
            return Math.Max(Math.Min(Value, Max), Min);

        }
        string OpenFilePicker(string DefaultPath, string DefaultName)
        {
            string retStr = "";

            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "BMP Files (*.bmp)|*.BMP|" + "JPG Files (*.jpg)|*.JPG|" + "PNG Files (*.png)|*.PNG|" + "All files (*.*)|*.*";
            dlg.Filter = DefaultPath;
            dlg.FileName = DefaultName;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                retStr = dlg.FileName;
            }
            return retStr;
        }
    }
}
