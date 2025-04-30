//using AForge.Controls;
using FreeImageAPI;
using JetEazy.BasicSpace;
using JzKHC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Allinone.UISpace.MSRUISpace
{
    public partial class PadUI : UserControl
    {
        #region 萬子的小晶片 WAN01

        Button btnLoad;
        NumericUpDown numGammaCorrelation;
        NumericUpDown numBlurCount;
        NumericUpDown numholesratio;

        NumericUpDown numRangeRatio;

        NumericUpDown numleft;
        NumericUpDown numright;
        NumericUpDown numtop;
        NumericUpDown numbottom;


        PictureBox picOrg;
        PictureBox picResult;
        PictureBox picfinal;

        Label lblresult;
        ComboBox cboResult;
        CheckBox chkisneedclose;

        bool IsNeedChange = true;

        #endregion

        OPSpace.AnalyzeClass analyze;

        Bitmap bmpBitmap = null;
        Bitmap bmpRun = new Bitmap(1, 1);

        string DefaultString = "";
        public PadUI()
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
            pbx1.Image = bmpBitmap;

            IsNeedChange = true;
        }
        public string GetDataValueString()
        {
            string retstring = "";

            retstring = numGammaCorrelation.Value.ToString() + ",";
            retstring += numBlurCount.Value.ToString() + ",";
            retstring += numholesratio.Value.ToString() + ",";
            retstring += numRangeRatio.Value.ToString() + ",";
            retstring += numleft.Value.ToString() + ",";
            retstring += numright.Value.ToString() + ",";
            retstring += numtop.Value.ToString() + ",";
            retstring += numbottom.Value.ToString() + ",";
            retstring += (chkisneedclose.Checked ? "1" : "0");

            return retstring;
        }

        void InitialInside()
        {
            IsNeedChange = false;

            InitialWAN01();

            //DefaultString = GetDataValueString();
            //Initial(DefaultString);

        }

        void InitialWAN01()
        {

            //btnLoad = button8;
            numGammaCorrelation = numericUpDown1;
            numBlurCount = numericUpDown2;
            numholesratio = numericUpDown3;

            cboResult = comboBox1;
            chkisneedclose = checkBox2;

            picOrg = pbx1;
            picResult = pbx2;
            picfinal = pbx22;

            numRangeRatio = numericUpDown8;

            numleft = numericUpDown4;
            numright = numericUpDown5;
            numtop = numericUpDown6;
            numbottom = numericUpDown7;


            //btnLoad.Click += BtnLoad_Click;
            numGammaCorrelation.ValueChanged += Num_ValueChanged;
            numBlurCount.ValueChanged += Num_ValueChanged;
            numholesratio.ValueChanged += Num_ValueChanged;

            numRangeRatio.ValueChanged += Num_ValueChanged;

            numleft.ValueChanged += Num_ValueChanged;
            numright.ValueChanged += Num_ValueChanged;
            numtop.ValueChanged += Num_ValueChanged;
            numbottom.ValueChanged += Num_ValueChanged;


            cboResult.SelectedIndexChanged += CboResult_SelectedIndexChanged;
            chkisneedclose.CheckedChanged += Chkisneedclose_CheckedChanged;
        }

        private void Chkisneedclose_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsNeedChange)
                return;

            Bitmap bmp = new Bitmap(bmpBitmap);

            analyze.PADPara.PADINSPECTOPString = GetDataValueString();
            analyze.PADPara.PadInspectMethod = PadInspectMethodEnum.PAD_SMALL;
            analyze.PADPara.TrainStatusCollection.Clear();
            analyze.PADPara.GetDataHX(bmpBitmap);

            pbx2.Image = analyze.PADPara.sideAnalyze.bmpFirst;
            pbx22.Image = analyze.PADPara.sideAnalyze.bmpSecond;

            bmp.Dispose();
        }

        private void CboResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (IsNeedChange)
            //{
            //    pbx2.Image = null;

            //    Bitmap bmp = new Bitmap(cboResult.Text);

            //    pbx2.Image = new Bitmap(bmp);

            //    bmp.Dispose();
            //}

        }

        private void Num_ValueChanged(object sender, EventArgs e)
        {
            if (!IsNeedChange)
                return;

            Bitmap bmp = new Bitmap(bmpBitmap);

            analyze.PADPara.PADINSPECTOPString =  GetDataValueString();
            analyze.PADPara.PadInspectMethod = PadInspectMethodEnum.PAD_SMALL;
            analyze.PADPara.TrainStatusCollection.Clear();
            analyze.PADPara.GetDataHX(bmpBitmap);

            pbx2.Image = analyze.PADPara.sideAnalyze.bmpFirst;
            pbx22.Image = analyze.PADPara.sideAnalyze.bmpSecond;

            bmp.Dispose();
        }
        void SetValue(string str)
        {
            string[] strs = str.Split(',');

            try
            {
                if (strs.Length > 8)
                {
                    numGammaCorrelation.Value = decimal.Parse(strs[0]);
                    numBlurCount.Value = decimal.Parse(strs[1]);
                    numholesratio.Value = decimal.Parse(strs[2]);
                    numRangeRatio.Value = decimal.Parse(strs[3]);
                    numleft.Value = decimal.Parse(strs[4]);
                    numright.Value = decimal.Parse(strs[5]);
                    numtop.Value = decimal.Parse(strs[6]);
                    numbottom.Value = decimal.Parse(strs[7]);
                    chkisneedclose.Checked = strs[8] == "1";
                }
            }
            catch (Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
                SetValue(DefaultString);
            }
        }
        //void GetDataHX(Bitmap bmp)
        //{
        //    IsNeedChange = false;

        //    pbx1.Image = null;
        //    pbx2.Image = null;

        //    sideAnalyze.Suiccide();
        //    sideAnalyze.GetDataHX(bmp, (double)numGammaCorrelation.Value,
        //        (int)numBlurCount.Value, (double)numholesratio.Value, chkisneedclose.Checked,
        //        cboResult);


        //    int[] sidecount = new int[(int)SIDEEmnum.COUNT];

        //    sidecount[(int)SIDEEmnum.LEFT] = (int)numleft.Value;
        //    sidecount[(int)SIDEEmnum.RIGHT] = (int)numright.Value;
        //    sidecount[(int)SIDEEmnum.TOP] = (int)numtop.Value;
        //    sidecount[(int)SIDEEmnum.BOTTOM] = (int)numbottom.Value;

        //    sideAnalyze.GetBoarder(bmp, (double)numRangeRatio.Value, sidecount);

        //    sideAnalyze.DrawProcessedSides();


        //    IsNeedChange = true;

        //    pbx1.Image = sideAnalyze.bmpOrg;
        //    pbx2.Image = sideAnalyze.bmpFirst;
        //    pbx22.Image = sideAnalyze.bmpSecond;
        //}

    }
}
