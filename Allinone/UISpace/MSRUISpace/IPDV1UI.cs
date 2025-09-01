using AForge.Imaging;
using MoveGraphLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Allinone.UISpace.MSRUISpace
{
    public partial class IPDV1UI : UserControl
    {
        #region IPD WAN02

        Button btnLoadIPD;
        ComboBox cboColorChannel;
        NumericUpDown numThresholdRatio;
        NumericUpDown numObjectFilterRatio;
        NumericUpDown numEDCount;
        NumericUpDown numShortenRatio;
        NumericUpDown numBangBangOffsetVal;
        TextBox txtBangBangRectStr;
        TextBox txtNeverOutsideRect;

        Label lblIPDMeanValue;
        Label lblIPDThreshold;
        Label lblIPDPassNG;
        TextBox txtIPDresult;

        ComboBox cboIPDMethod;

        #endregion

        bool IsNeedChange = true;

        OPSpace.AnalyzeClass analyze;

        Bitmap bmpBitmap = null;
        Bitmap bmpRun = new Bitmap(1, 1);

        string DefaultString = "";
        bool IsCaptureBBRegion = false;
        bool IsCaptureOutRegion = false;

        public IPDV1UI()
        {
            InitializeComponent();
            InitialInside();
        }
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
            //propertyGrid.SelectedObject = checkBaseParaPropertyGrid;

            bmpBitmap = new Bitmap(analyze.bmpPATTERN);
            //pbx1.Image = bmpBitmap;
            DS1.ReplaceDisplayImage(bmpBitmap);

            IsNeedChange = true;
        }
        public string GetDataValueString()
        {
            string retstring = "";

            retstring = GetRGB.ToString() + ",";
            retstring += numThresholdRatio.Value.ToString() + ",";
            retstring += numObjectFilterRatio.Value.ToString() + ",";
            retstring += numEDCount.Value.ToString() + ",";
            retstring += numShortenRatio.Value.ToString() + ",";
            retstring += txtBangBangRectStr.Text.ToString() + ",";
            retstring += numBangBangOffsetVal.Value.ToString() + ",";
            retstring += txtNeverOutsideRect.Text.ToString() + ",";
            retstring += cboIPDMethod.SelectedIndex.ToString();

            return retstring;
        }

        void InitialInside()
        {
            IsNeedChange = false;

            InitialWAN02();

            //DefaultString = GetDataValueString();
            //Initial(DefaultString);
            init_Display();
            update_Display();

        }

        short GetRGB
        {
            get
            {
                switch (cboColorChannel.SelectedIndex)
                {
                    case 0:
                        return RGB.R;
                    case 1:
                        return RGB.G;
                    case 2:
                        return RGB.B;
                    default:
                        return RGB.R;

                }
            }
            set
            {
                cboColorChannel.SelectedIndex = value;
            }
        }
        void InitialWAN02()
        {
            btnLoadIPD = button9;
            cboColorChannel = comboBox3;
            numThresholdRatio = numericUpDown13;
            numObjectFilterRatio = numericUpDown14;
            numEDCount = numericUpDown15;
            numShortenRatio = numericUpDown16;
            txtBangBangRectStr = textBox2;
            txtIPDresult = textBox1;

            txtNeverOutsideRect = textBox3;

            numBangBangOffsetVal = numericUpDown17;

            lblIPDPassNG = label31;

            lblIPDMeanValue = label21;
            lblIPDThreshold = label24;

            cboIPDMethod = comboBox4;

            btnLoadIPD.Click += BtnLoadIPD_Click;

            cboColorChannel.Items.Add("R");
            cboColorChannel.Items.Add("G");
            cboColorChannel.Items.Add("B");

            cboColorChannel.SelectedIndex = 1;

            cboIPDMethod.Items.Add("IPD");
            cboIPDMethod.Items.Add("NORMAL");

            cboIPDMethod.SelectedIndex = 0;

            numThresholdRatio.ValueChanged += WAN02_ValueChanged;
            numObjectFilterRatio.ValueChanged += WAN02_ValueChanged;
            numEDCount.ValueChanged += WAN02_ValueChanged;
            numShortenRatio.ValueChanged += WAN02_ValueChanged;

            cboColorChannel.SelectedIndexChanged += CboColorChannel_SelectedIndexChanged;
        }

        private void WAN02_ValueChanged(object sender, EventArgs e)
        {
            //if (pbx1.Image != null)
            //    GetDataIPD(bmpOrg);

            if (!IsNeedChange)
                return;
            checkIPD();
        }
        private void CboColorChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (IsNeedChange)
            //{
            //    //pbx2.Image = null;

            //    if (pbx1.Image != null)
            //        GetDataIPD(bmpOrg);

            //    //bmp.Dispose();
            //}

            if (!IsNeedChange)
                return;
            checkIPD();

        }
        private void BtnLoadIPD_Click(object sender, EventArgs e)
        {
            checkIPD();
            //string _path = OpenFilePicker("");
            //if (!string.IsNullOrEmpty(_path))
            //{
            //    Bitmap bmptmp = new Bitmap(_path);

            //    if (bmpOrg != null)
            //        bmpOrg.Dispose();

            //    bmpOrg = CopyImage(bmptmp);

            //    GetDataIPD(bmptmp);
            //    //GetDataHX(bmptmp);

            //    bmptmp.Dispose();
            //}
        }
        void SetValue(string str)
        {
            string[] strs = str.Split(',');

            try
            {
                if (strs.Length > 5)
                {
                    GetRGB = short.Parse(strs[0]);
                    numThresholdRatio.Value = decimal.Parse(strs[1]);
                    numObjectFilterRatio.Value = decimal.Parse(strs[2]);
                    numEDCount.Value = decimal.Parse(strs[3]);
                    numShortenRatio.Value = decimal.Parse(strs[4]);
                    txtBangBangRectStr.Text = strs[5];
                }
                if (strs.Length > 8)
                {
                    numBangBangOffsetVal.Value = decimal.Parse(strs[6]);
                    txtNeverOutsideRect.Text = strs[7];
                    cboIPDMethod.SelectedIndex = int.Parse(strs[8]);
                }
            }
            catch (Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
                SetValue(DefaultString);
            }
        }
        void checkIPD()
        {
            Bitmap bmp = new Bitmap(bmpBitmap);

            analyze.PADPara.PADINSPECTOPString = GetDataValueString();
            analyze.PADPara.PadInspectMethod = PadInspectMethodEnum.PAD_V1;
            analyze.PADPara.TrainStatusCollection.Clear();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();


            string retstr = analyze.PADPara.GetDataIPD(bmpBitmap, true);

            stopwatch.Stop();
            long ms = stopwatch.ElapsedMilliseconds;

            //pbx2.Image = analyze.PADPara.sideAnalyze.bmpFirst;
            //pbx22.Image = analyze.PADPara.sideAnalyze.bmpSecond;

            DS2.ReplaceDisplayImage(analyze.PADPara.bmplist[1]);
            DS3.ReplaceDisplayImage(analyze.PADPara.bmplist[3]);

            bmp.Dispose();


            txtIPDresult.Text = analyze.PADPara.jzsideanalyzeex.AnalyzeResultStr;

            //lblIPDMeanValue.Text = analyze.PADPara.jzsideanalyzeex.;
            //lblIPDThreshold.Text = analyze.PADPara.jzsideanalyzeex.thresholdstr;

            lblIPDPassNG.Text = retstr;// analyze.PADPara.sideAnalyze.PASSNG;

            if (retstr == "PASS")
            {
                lblIPDPassNG.BackColor = Color.Lime;
            }
            else
            {
                lblIPDPassNG.BackColor = Color.Red;
            }
        }

        void init_Display()
        {
            DS1.Initial(100, 0.01f);
            DS1.SetDisplayType(JzDisplay.DisplayTypeEnum.NORMAL);
            DS1.CaptureAction += DS1_CaptureAction;
            DS2.Initial(100, 0.01f);
            DS2.SetDisplayType(JzDisplay.DisplayTypeEnum.SHOW);
            DS3.Initial(100, 0.01f);
            DS3.SetDisplayType(JzDisplay.DisplayTypeEnum.SHOW);
        }

        private void DS1_CaptureAction(RectangleF rectf)
        {
            //GraphicalObject grobj = myMover[0].Source;
            RectangleF rectf_org = new RectangleF(10, 10, 10, 10);
            RectangleF rectf_des = new RectangleF(10, 10, 10, 10);

            if (IsCaptureBBRegion)
            {
                BoundRect(ref rectf, bmpBitmap.Size);
                if (rectf.Width > 1 && rectf.Height > 1)
                {
                    txtBangBangRectStr.Text = $"{rectf.X};{rectf.Y};{rectf.Width};{rectf.Height}";
                    IsCaptureBBRegion = false;
                    SetBTNBK(button1, IsCaptureBBRegion);
                    checkIPD();

                }
            }
            else if (IsCaptureOutRegion)
            {
                BoundRect(ref rectf, bmpBitmap.Size);
                if (rectf.Width > 1 && rectf.Height > 1)
                {
                    txtNeverOutsideRect.Text = $"{rectf.X};{rectf.Y};{rectf.Width};{rectf.Height}";
                    IsCaptureOutRegion = false;
                    SetBTNBK(button2, IsCaptureOutRegion);
                    checkIPD();

                }
            }
        }

        void update_Display()
        {
            DS1.Refresh();
            DS1.DefaultView();
            DS2.Refresh();
            DS2.DefaultView();
            DS3.Refresh();
            DS3.DefaultView();
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

        private void button1_Click(object sender, EventArgs e)
        {
            IsCaptureBBRegion = !IsCaptureBBRegion;
            SetBTNBK(button1, IsCaptureBBRegion);
        }

        void SetBTNBK(Button btn, bool ison)
        {
            btn.BackColor = (ison ? Color.Red : Control.DefaultBackColor);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            IsCaptureOutRegion = !IsCaptureOutRegion;
            SetBTNBK(button2, IsCaptureOutRegion);
        }
    }
}
