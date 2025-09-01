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

namespace Allinone.FormSpace.PADForm.PadInspect
{
    public partial class PADG1UI : UserControl
    {
        //bool mCropRegion = false;
        Bitmap bmpOperate = new Bitmap(1, 1);
        PADG1Class padg1Class = new PADG1Class();

        public PADG1UI()
        {
            InitializeComponent();

            init_Display();
            update_Display();

            SizeChanged += N1_SizeChanged;

            //btnLoadImage.Click += BtnLoadImage_Click;
            btnTest.Click += BtnTest_Click;
            btnTrain0.Click += BtnTrain0_Click;
        }

        private void BtnTrain0_Click(object sender, EventArgs e)
        {
            analyze.PADPara.PADINSPECTOPString = GetDataValueString();

            analyze.ResetTrainStatus();
            analyze.ResetRunStatus();
            analyze.Z02_CreateTrainRequirement(Allinone.FormSpace.MainForm.DETAILFRM.PAGEUI.PageNow.GetbmpORG(), new PointF(0, 0));
            analyze.Z05_AlignTrainProcess();

            bmpOperate.Dispose();
            bmpOperate = new Bitmap(analyze.PADPara.bmpPadFindOutput);
            DS1.ReplaceDisplayImage(bmpOperate);
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            bool btemp = INI.IsDEBUGCHIP;
            //mCropRegion = !mCropRegion;
            //btnTest.BackColor = (mCropRegion ? Color.Red : Color.Green);
            INI.IsDEBUGCHIP = false;
            analyze.PADPara.PADINSPECTOPString = GetDataValueString();

            analyze.ResetTrainStatus();
            analyze.ResetRunStatus();
            analyze.Z02_CreateTrainRequirement(Allinone.FormSpace.MainForm.DETAILFRM.PAGEUI.PageNow.GetbmpORG(), new PointF(0, 0));
            analyze.Z05_AlignTrainProcess(false);

            bmpOperate.Dispose();
            bmpOperate = new Bitmap(analyze.PADPara.bmpMeasureOutput);
            DS1.ReplaceDisplayImage(bmpOperate);

            INI.IsDEBUGCHIP = btemp;
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
            pg.SelectedObject = padg1Class;

            bmpOperate.Dispose();
            bmpOperate = new Bitmap(analyze.PADPara.bmpPadFindOutput);
            DS1.ReplaceDisplayImage(bmpOperate);

            //pbx1.Image = bmpBitmap;

            //IsNeedChange = true;
        }
        public string GetDataValueString()
        {
            string retstring = "";
            retstring = padg1Class.ToParaString();
            return retstring;
        }
        void SetValue(string str)
        {
            try
            {
                padg1Class.FromString(str);
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

            //if (mCropRegion)
            //{
            //    BoundRect(ref rectf, bmpOperate.Size);
            //    if (rectf.Width > 1 && rectf.Height > 1)
            //    {
            //        //txtBangBangRectStr.Text = $"{rectf.X};{rectf.Y};{rectf.Width};{rectf.Height}";
            //        mCropRegion = false;
            //        //SetBTNBK(button1, IsCaptureBBRegion);
            //        //checkIPD();

            //        Bitmap bmp0 = bmpOperate.Clone(rectf, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //        bool bOK = analyze.PADPara.checkNoHaveRunBlobCount(bmp0, n2Class.ThresholdValue, n2Class.IsWhite, n2Class.Count);
            //        label1.Text = $"结果:{(bOK ? "有芯片" : "无芯片")}";


            //        btnTest.BackColor = (mCropRegion ? Color.Red : Color.Green);
            //    }
            //}
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
    }
}
