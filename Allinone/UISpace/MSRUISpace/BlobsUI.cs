using Allinone.BasicSpace;
using Allinone.BasicSpace.MeasureD;
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
    public partial class BlobsUI : UserControl
    {
        MBlobParaPropertyGridClass checkBaseParaPropertyGrid = new MBlobParaPropertyGridClass();

        OPSpace.AnalyzeClass analyze;

        PictureBox picBitmap;
        PictureBox picRunBmp;
        Bitmap bmpBitmap = null;
        Bitmap bmpRun = new Bitmap(1, 1);

        Button btnAutoAnalyze;
        PropertyGrid propertyGrid;

        string DefaultString = "";
        public BlobsUI()
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
            propertyGrid.SelectedObject = checkBaseParaPropertyGrid;

            bmpBitmap = new Bitmap(analyze.bmpPATTERN);
            picBitmap.Image = bmpBitmap;
        }
        public string GetDataValueString()
        {
            string retstring = "";

            retstring = checkBaseParaPropertyGrid.ToParaString();

            return retstring;
        }

        void InitialInside()
        {
            btnAutoAnalyze = button1;
            propertyGrid = propertyGrid1;
            picBitmap = pictureBox1;
            picRunBmp = pictureBox2;
            DefaultString = GetDataValueString();
            if (DesignMode)
                Initial(DefaultString);
            btnAutoAnalyze.Click += BtnAutoAnalyze_Click;
            propertyGrid.PropertyValueChanged += PropertyGrid_PropertyValueChanged;
        }

        private void PropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            analyze.MEASUREPara.MMOPString = GetDataValueString();
            analyze.MEASUREPara.MeasureMethod = MeasureMethodEnum.BLOBS;
            analyze.MEASUREPara.TrainStatusCollection.Clear();
            bool bOK = analyze.MEASUREPara.MeasureProcess(analyze.bmpWIP, analyze.bmpPATTERN, analyze.bmpMASK, ref bmpRun, analyze.Brightness, analyze.Contrast, analyze.ToAnalyzeString(), analyze.PassInfo, true);


            richTextBox1.BackColor = bOK ? Color.White : Color.Red;
            richTextBox1.Text = analyze.MEASUREPara.TrainStatusCollection.AllProcessString;
            picRunBmp.Image = bmpRun;
        }

        void SetValue(string str)
        {
            if (string.IsNullOrEmpty(str))
                str = checkBaseParaPropertyGrid.ToParaString();
            checkBaseParaPropertyGrid.FromingStr(str);
        }

        private void BtnAutoAnalyze_Click(object sender, EventArgs e)
        {

        }
    }
}
