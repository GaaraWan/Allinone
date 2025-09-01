using Allinone.FormSpace.BasicPG;
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

namespace Allinone.FormSpace.PADForm.PadInspect
{
    public partial class PADG2UI : UserControl
    {
        //bool mCropRegion = false;
        Bitmap bmpOperate = new Bitmap(1, 1);
        PADG2Class padg2Class = new PADG2Class();

        Mover xMovers
        {
            get { return padg2Class.myMover; }
        }

        public PADG2UI()
        {
            InitializeComponent();

            init_Display();
            update_Display();

            SizeChanged += N1_SizeChanged;

            //btnLoadImage.Click += BtnLoadImage_Click;
            btnTest.Click += BtnTest_Click;
            btnTrain0.Click += BtnTrain0_Click;

            btnAdd.Click += BtnAdd_Click;
            btnDel.Click += BtnDel_Click;
            btnClear.Click += BtnClear_Click;
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            xMovers.Clear();
            DS1.ClearMover();
            //xMoverIndex = 0;
            //InspectX2Class.Instance.rectangles.Clear();
        }

        private void BtnDel_Click(object sender, EventArgs e)
        {
            Mover xMoversTemp = new Mover();
            xMoversTemp.Clear();
            int i = 0;
            while (i < xMovers.Count)
            {
                GraphicalObject grobj = xMovers[i].Source;

                if (!(grobj as JzRectEAG).IsSelected)
                {
                    xMoversTemp.Add(grobj as JzRectEAG);
                }

                i++;
            }
            xMovers.Clear();
            DS1.ClearMover();
            //InspectX2Class.Instance.rectangles.Clear();
            i = 0;
            while (i < xMoversTemp.Count)
            {
                GraphicalObject grobj = xMoversTemp[i].Source;
                xMovers.Add(grobj as JzRectEAG);
                i++;
            }

            DS1.SetMover(xMovers);
            DS1.RefreshDisplayShape();
            DS1.MappingSelect();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            Rectangle rectangle = new Rectangle(0, 0, 50, 50);
            if (xMovers.Count == 0)
            {
                JzRectEAG RatioRectEAG = new JzRectEAG(Color.FromArgb(0, Color.Red), rectangle);
                RatioRectEAG.RelateNo = 2;
                xMovers.Add(RatioRectEAG);
            }
            else
            {
                bool bFound = false;
                int i = 0;
                while (i < xMovers.Count)
                {
                    GraphicalObject grobj_t0 = xMovers[i].Source;

                    if ((grobj_t0 as JzRectEAG).IsSelected)
                    {
                        bFound = true;
                        (grobj_t0 as JzRectEAG).IsSelected = false;
                        break;
                    }

                    i++;
                }

                if (!bFound)
                {
                    GraphicalObject grobj = xMovers[xMovers.Count - 1].Source;

                    JzRectEAG RatioRectEAG = new JzRectEAG(Color.FromArgb(0, Color.Red), (grobj as JzRectEAG).RealRectangleAround(0, 0));
                    RatioRectEAG.RelateNo = 2;
                    RatioRectEAG.SetOffset(new Point(20, 20));

                    xMovers.Add(RatioRectEAG);
                }
                else
                {
                    GraphicalObject grobj = xMovers[i].Source;

                    JzRectEAG RatioRectEAG = new JzRectEAG(Color.FromArgb(0, Color.Red), (grobj as JzRectEAG).RealRectangleAround(0, 0));
                    RatioRectEAG.RelateNo = 2;
                    RatioRectEAG.SetOffset(new Point(20, 20));
                    //RatioRectEAG.IsSelected = true;

                    xMovers.Add(RatioRectEAG);
                }
            }

            DS1.SetMover(xMovers);
            DS1.RefreshDisplayShape();
            DS1.MappingSelect();
        }

        private void BtnTrain0_Click(object sender, EventArgs e)
        {
            analyze.PADPara.PADINSPECTOPString = GetDataValueString();

            analyze.ResetTrainStatus();
            analyze.ResetRunStatus();
            analyze.Z02_CreateTrainRequirement(Allinone.FormSpace.MainForm.DETAILFRM.PAGEUI.PageNow.GetbmpORG(), new PointF(0, 0));
            analyze.Z05_AlignTrainProcess();

            bmpOperate.Dispose();
            bmpOperate = new Bitmap(analyze.PADPara.bmpPadBolbOutput);
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

            //analyze.PADPara.CheckYinJiaoIrregular(false, bmpInput, new RectangleF(0, 0, 1, 1));

            bmpOperate.Dispose();
            bmpOperate = new Bitmap(analyze.PADPara.bmpPadBolbOutput);
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
            pg.SelectedObject = padg2Class;

            bmpOperate.Dispose();
            bmpOperate = new Bitmap(analyze.PADPara.EzMVDPatMatchPADG2.bmpItem);
            DS1.ReplaceDisplayImage(bmpOperate);


            DS1.ClearMover();
            //xMovers.Clear();

            //int i = 0;
            //while (i < InspectX2Class.Instance.rectangles.Count)
            //{
            //    JzRectEAG _rect = new JzRectEAG(Color.FromArgb(0, Color.Blue), InspectX2Class.Instance.rectangles[i]);
            //    _rect.RelateLevel = 2;
            //    _rect.RelateNo = i;
            //    _rect.RelatePosition = 0;
            //    xMovers.Add(_rect);

            //    i++;
            //}

            DS1.SetMover(xMovers);
            DS1.RefreshDisplayShape();
            DS1.MappingSelect();

            //pbx1.Image = bmpBitmap;

            //IsNeedChange = true;
        }
        public string GetDataValueString()
        {
            string retstring = "";
            retstring = padg2Class.ToParaString();
            return retstring;
        }
        void SetValue(string str)
        {
            try
            {
                padg2Class.FromString(str);
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
