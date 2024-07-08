using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Allinone.OPSpace;

namespace Allinone.UISpace.MSRUISpace
{
    public partial class BkUI : UserControl
    {
        NumericUpDown numdiffratio;
        NumericUpDown numthreadratio;
        NumericUpDown numfilterratio;
        NumericUpDown numtolerance;

        OPSpace.AnalyzeClass analyze;

        PictureBox picBitmap;
        PictureBox picRunBmp;
        Bitmap bmpBitmap = null;
        Bitmap bmpRun = new Bitmap(1, 1);

        Button btnReset;
        string DefaultString = "";
        //List<AnalyzeClass> AnalyzeList = new List<AnalyzeClass>();

        //int AnalyzeSelectNo = 0;
        //AnalyzeClass AnalyzeSelectNow
        //{
        //    get
        //    {
        //        return AnalyzeList.Find(x => x.No == AnalyzeSelectNo);
        //    }
        //}

        public BkUI(string str)
        {
            InitializeComponent();
            InitialInside();

            SetValue(str);

        }
        public BkUI()
        {
            InitializeComponent();
            InitialInside();
        }


        void InitialInside()
        {
            numdiffratio = numericUpDown1;
            numthreadratio = numericUpDown2;
            numfilterratio = numericUpDown3;
            numtolerance = numericUpDown4;
            picBitmap = pictureBox1;
            picRunBmp = pictureBox2;


            DefaultString = GetDataValueString();

            btnReset = button1;

            btnReset.Click += BtnReset_Click;
            numdiffratio.ValueChanged += Num_ValueChanged;
            numthreadratio.ValueChanged += Num_ValueChanged;
            numfilterratio.ValueChanged += Num_ValueChanged;
            numtolerance.ValueChanged += Num_ValueChanged;
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

            bmpBitmap = new Bitmap(analyze.bmpPATTERN);
            picBitmap.Image = bmpBitmap;

            Num_ValueChanged(new object(), new EventArgs());
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            SetValue(DefaultString);
        }
        public string GetDataValueString()
        {
            string retstring = "";

            retstring = numdiffratio.Value.ToString() + ",";
            retstring += numthreadratio.Value.ToString() + ",";
            retstring += numfilterratio.Value.ToString() + ",";
            retstring += numtolerance.Value.ToString();


            return retstring;
        }
        void SetValue(string str)
        {
            string[] strs = str.Split(',');

            try
            {
                numdiffratio.Value = decimal.Parse(strs[0]);
                numthreadratio.Value = decimal.Parse(strs[1]);
                numfilterratio.Value = decimal.Parse(strs[2]);
                decimal value = decimal.Parse(strs[3]);
                numtolerance.Value = value;// decimal.Parse(strs[3]);


            }
            catch (Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
                SetValue(DefaultString);
            }
        }

        private void Num_ValueChanged(object sender, EventArgs e)
        {

            analyze.MEASUREPara.MMOPString = numdiffratio.Value.ToString() + ",";// = float.Parse(strs[0]);
            analyze.MEASUREPara.MMOPString += numthreadratio.Value.ToString() + ",";
            analyze.MEASUREPara.MMOPString += numfilterratio.Value.ToString() + ",";
            analyze.MEASUREPara.MMOPString += numtolerance.Value.ToString() + ",";

            analyze.MEASUREPara.MeasureProcess(analyze.bmpWIP, analyze.bmpPATTERN, analyze.bmpMASK, ref bmpRun, analyze.Brightness, analyze.Contrast, analyze.ToAnalyzeString(), analyze.PassInfo, true);
            //  Bitmap bmptemp = new Bitmap(bmpBitmap);
            //Bitmap bmpTempMask = new Bitmap(bmptemp.Width, bmptemp.Height);
            //Graphics g = Graphics.FromImage(bmpTempMask);
            //g.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, bmpTempMask.Width, bmpTempMask.Height));
            //g.Dispose();
            //OPSpace.WorkStatusClass workstatus = new OPSpace.WorkStatusClass(JetEazy.AnanlyzeProcedureEnum.MEASURE);
            //analyze.MEASUREPara. BlindMeasure(bmptemp, bmpTempMask, ref bmpRun, true, workstatus);
            picRunBmp.Image = bmpRun;
        }
    }
}
