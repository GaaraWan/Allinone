using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using JetEazy;
using JzDisplay;
using JzDisplay.UISpace;
using Allinone.OPSpace;
using Allinone.OPSpace.AnalyzeSpace;
using Allinone.BasicSpace;
using Allinone.FormSpace;
using MoveGraphLibrary;
using WorldOfMoveableObjects;
using JetEazy.BasicSpace;
using JzASN.OPSpace;
using JetEazy.FormSpace;
using AForge.Imaging.Filters;
using PdfSharp.Pdf.Content.Objects;

namespace Allinone.UISpace
{
    public partial class PageUI : UserControl
    {
        enum OPSourceEnum
        {
            PAGE,
            ANALYZE,
        }

        CheckBox chkLock;
        CheckBox chkIsOnly;
        CheckBox chkDetail;

        NumericUpDown numLevel;
        NumericUpDown numStep;

        AtreeUI ATREEUI;
        AinfoUI AINFOUI;
        DispUI DISPUI;
        SubUI SUBUI;

        public AnalyzeClass AnalyzeRootNow;
        AnalyzeClass AnalyzeOperate;

        public PageClass PageNow;

        DetailForm DETAILFRM;

        OPSourceEnum OPSource = OPSourceEnum.PAGE;

        bool IsNeedToChange = false;

        Mover myMoverCollection = new Mover();

        List<AnalyzeClass> AnalyzeList = new List<AnalyzeClass>();

        int AnalyzeSelectNo = 0;
        public AnalyzeClass AnalyzeSelectNow
        {
            get
            {
                return AnalyzeList.Find(x => x.No == AnalyzeSelectNo);
            }
        }
        bool IsShowDetail
        {
            get
            {
                return chkDetail.Checked;
            }
        }

        int Step
        {
            get
            {
                return (int)numStep.Value;
            }
        }

        VersionEnum VERSION = VersionEnum.KBAOI;
        OptionEnum OPTION = OptionEnum.MAIN;

        public bool IsLearn = false;
        public PageUI()
        {
            InitializeComponent();
            InitialInside();
        }
        void InitialInside()
        {
            chkLock = checkBox1;
            chkIsOnly = checkBox2;
            chkDetail = checkBox3;

            numLevel = numericUpDown1;
            numStep = numericUpDown2;

            chkLock.CheckedChanged += chkLock_CheckedChanged;
            chkIsOnly.CheckedChanged += chkIsOnly_CheckedChanged;
            numLevel.ValueChanged += numLevel_ValueChanged;
            numLevel.Enabled = false;
            chkIsOnly.Enabled = false;

            ATREEUI = atreeUI1;
            AINFOUI = ainfoUI1;
            DISPUI = dispUI1;
            DISPUI.Initial(10, 0.01f);
            DISPUI.SetDisplayType(DisplayTypeEnum.NORMAL);

            SUBUI = subUI1;
            SUBUI.Initial();
            SUBUI.Visible = false;
        }
        public void SetIsLearn()
        {
            IsLearn = true;

            ATREEUI.SetLearn();

            AINFOUI.IsLearn = true;

        }
        public void Initial(VersionEnum version, OptionEnum opt)
        {
            VERSION = version;
            OPTION = opt;

            ATREEUI.Initial(VERSION, OPTION);
            AINFOUI.Initial(VERSION, OPTION);

            ATREEUI.TriggerAction += ATREEUI_TriggerAction;
            ATREEUI.ChangeAction += ATREEUI_ChangeAction;
            ATREEUI.ChangeLearnAction += ATREEUI_ChangeLearnAction;
            ATREEUI.BackwardAction += ATREEUI_BackwardAction;
            ATREEUI.ChangeMoverAction += ATREEUI_ChangeMoverAction;
            ATREEUI.LearnTuneAction += ATREEUI_LearnTuneAction;
            ATREEUI.GetOperateAction += ATREEUI_GetOperateAction;

            AINFOUI.ShapenAction += AINFOUI_ShapenAction;
            AINFOUI.BackwardAction += AINFOUI_BackwardAction;
            AINFOUI.ChangeAction += AINFOUI_ChangeAction;
            DISPUI.MoverAction += DISPUI_MoverAction;

            SUBUI.OperateAction += SUBUI_OperateAction;
            chkDetail.CheckedChanged += ChkDetail_CheckedChanged;

            //this.MouseDown += PageUI_MouseDown;
            //this.MouseUp += PageUI_MouseUp;


            DISPUI.CaptureAction += DISPUI_CaptureAction;
        }

        private void DISPUI_CaptureAction(RectangleF rectf)
        {
            OnCaptureTrigger(rectf);
        }

        //string _collectOperateStr = string.Empty;
        //private void PageUI_MouseUp(object sender, MouseEventArgs e)
        //{
        //    if (string.IsNullOrEmpty(_collectOperateStr))
        //        return;
        //    string[] strs = _collectOperateStr.Split(',');
        //    DISPSelectActionDX(strs);
        //}

        //private void PageUI_MouseDown(object sender, MouseEventArgs e)
        //{
        //    _collectOperateStr = string.Empty;
        //}

        private void ChkDetail_CheckedChanged(object sender, EventArgs e)
        {
            SUBUI.Visible = chkDetail.Checked;

            FillSUBUI(false);
        }
        private void SUBUI_OperateAction(SubOperEnum oper)
        {
            switch (oper)
            {
                case SubOperEnum.OUTPUT:
                    FillSUBUI(true, false);
                    break;
                case SubOperEnum.PATTERN:
                case SubOperEnum.MASK:
                //case SubOperEnum.OUTPUT:
                case SubOperEnum.CHANGE:

                    FillSUBUI(true);

                    break;
            }
        }
        private void ATREEUI_GetOperateAction(int no, int learnno)
        {
            switch (OPSource)
            {
                case OPSourceEnum.PAGE:

                    foreach (AnalyzeClass analyze in AnalyzeList)
                    {
                        if (analyze.No == no && analyze.LearnNo == learnno)
                        {
                            analyze.GetDirectbmpPattern(DISPUI.GetOrgBMP(), new PointF(0, 0));
                        }
                    }
                    break;
                case OPSourceEnum.ANALYZE:

                    break;

            }
        }
        private void ATREEUI_LearnTuneAction(AnalyzeClass tuneanalyze)
        {
            ShowTune(tuneanalyze);
        }
        void ShowTune(AnalyzeClass tuneanalyze)
        {
            tuneanalyze.PrepareForLearnOperation(false);
            //將可能選定的學習清除
            tuneanalyze.PrepareRemovalNoList.Clear();

            DETAILFRM = new DetailForm(tuneanalyze, VERSION, Universal.OPTION);

            if (DETAILFRM.ShowDialog() == DialogResult.OK)
            {
                tuneanalyze.EndLearnOperation(true);
            }
            else
                tuneanalyze.EndLearnOperation(false);


            CollectMover();
            DISPUI.SetMover(myMoverCollection);
            DISPUI.RefreshDisplayShape();

            //IsNeedToShowMover = true;
            //DISPUIShowMover();
            DETAILFRM.Dispose();
        }
        private void ATREEUI_ChangeLearnAction(int index)
        {
            if (IsLearn)
            {
                SetAnalyze(AnalyzeOperate.GetLearnByIndex(index), false);
            }
        }
        private void AINFOUI_ChangeAction(string changeitemstring, string valuestring)
        {
            //若有全選並改變值而不會變的加在這裏
            if (CheckJustChangeOne(changeitemstring))
            {
                AnalyzeSelectNow.ToAssembleProperty();
                AnalyzeSelectNow.FromAssembleProperty(changeitemstring, valuestring);
            }
            else
            {
                AnalyzeRootNow.ToAssembleProperty();
                AnalyzeRootNow.FromAssembleProperty(changeitemstring, valuestring);
            }

            if (changeitemstring.ToUpper().IndexOf("EXTEND") > -1)
                FillSUBUI(false);
            else
                FillSUBUI(true);
        }

        /// <summary>
        /// 檢查是否這是只能改變選定項的值，而不是一次要改變所有的值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        bool CheckJustChangeOne(string str)
        {
            bool ret = true;

            ret &= str.ToUpper().IndexOf("ALIASNAME") > -1;
            ret &= str.ToUpper().IndexOf("RELATEASNITEM") > -1;

            return ret;
        }

        #region Event Operation
        private void AINFOUI_ShapenAction(ShapeOpEnum shapeop, ShapeEnum shape, string opstring)
        {
            switch (shapeop)
            {
                case ShapeOpEnum.ADDSHAPE:
                    AddShape(shape, opstring);
                    break;
                case ShapeOpEnum.DELSHAPE:
                    DelShape(shape, opstring);
                    break;
                case ShapeOpEnum.REVISESHAPE:
                    ReviseShape(shape, opstring);
                    break;
            }

            FillSUBUI(true);

        }
        private void ATREEUI_ChangeMoverAction(List<AnalyzeClass> changedanalyzelist, DBStatusEnum changeaction)
        {
            switch (changeaction)
            {
                case DBStatusEnum.ADD:
                    AddMover(changedanalyzelist);
                    DISPUI.RefreshDisplayShape();
                    DISPUI.SetMover(myMoverCollection);

                    DISPUI.MappingSelect();
                    break;
                case DBStatusEnum.DELETE:
                    DeleteMover(changedanalyzelist);
                    DISPUI.RefreshDisplayShape();
                    DISPUI.SetMover(myMoverCollection);

                    DISPUI.MappingSelect();
                    break;
            }
        }
        private void ATREEUI_ChangeAction(int no)
        {
            if (no > -1)
            {
                AnalyzeSelectNo = no;
                AINFOUI.SetAnalyze(AnalyzeSelectNow);
            }
            else
            {
                AnalyzeSelectNo = no;
                AINFOUI.SetAnalyze(null);
            }

            FillSUBUI(false);

            DISPUI.MappingSelect();

        }
        /// <summary>
        /// 將處理過後的影像顯示在 SUB UI 中
        /// </summary>
        void FillSUBUI(bool isreplace,bool istrain=true)
        {
            if (AnalyzeSelectNo > 1)
            {
                if (IsShowDetail)
                {
                    AnalyzeSelectNow.ResetTrainStatus();
                    AnalyzeSelectNow.ResetRunStatus();

                    if (!IsLearn)
                        AnalyzeSelectNow.Z02_CreateTrainRequirement(PageNow.GetbmpORG(), new PointF(0, 0));
                    switch (AnalyzeSelectNow.PADPara.PADMethod)
                    {
                        case PADMethodEnum.QLE_CHECK:
                            AnalyzeSelectNow.Z05_AlignTrainProcess(istrain);
                            break;
                        default:
                            AnalyzeSelectNow.Z05_AlignTrainProcess();
                            break;
                    }
                    string str = AnalyzeSelectNow.TrainStatusCollection.AllProcessString;

                    SUBUI.SetImage(AnalyzeSelectNow, isreplace);
                    SUBUI.SetLog(str);
                }
            }
            else
            {
                SUBUI.ClearImage();
            }
        }
        private void ATREEUI_TriggerAction(RCPStatusEnum status)
        {
            switch (status)
            {
                case RCPStatusEnum.COMBINEANALYZE:
                    CollectMover();
                    DISPUI.SetMover(myMoverCollection);
                    DISPUI.RefreshDisplayShape();

                    MessageBox.Show("Combine Complete!", "SYS", MessageBoxButtons.OK);

                    break;
                case RCPStatusEnum.EDITDETAIL:

                    CollectMover();
                    DISPUI.SetMover(myMoverCollection);
                    DISPUI.RefreshDisplayShape();

                    AINFOUI.SetAnalyze(AnalyzeSelectNow);

                    FillSUBUI(true);

                    break;
                default:
                    CollectMover();
                    break;
            }
        }
        private void DISPUI_MoverAction(MoverOpEnum moverop, string opstring)
        {
            int i = 0;
            string[] strs = opstring.Split(',');

            IsNeedToChange = false;

            switch (moverop)
            {
                case MoverOpEnum.SELECT:
                    //DISPSelectActionDX(strs);

                    //_collectOperateStr = opstring;
                    break;
                case MoverOpEnum.MOUSEUP:
                    DISPSelectActionDX(strs);

                    //_collectOperateStr = opstring;
                    break;
                    //case MoverOpEnum.ADD:
                    //    foreach (string str in strs)
                    //    {
                    //        lsbShapes.Items.Add(str);
                    //    }
                    //    break;
                    //case MoverOpEnum.DEL:
                    //    List<int> delindexlist = new List<int>();

                    //    foreach (string str in strs)
                    //    {
                    //        delindexlist.Add(int.Parse(str));
                    //    }

                    //    delindexlist.Sort();
                    //    delindexlist.Reverse();

                    //    foreach (int delindex in delindexlist)
                    //    {
                    //        lsbShapes.Items.RemoveAt(delindex);
                    //    }
                    //    break;
            }

            IsNeedToChange = true;
        }
        /// <summary>
        /// For Single Selection
        /// </summary>
        /// <param name="strs"></param>
        void DISPSelectAction(string[] strs)
        {
            List<int> selectanalyzenolist = new List<int>();

            if (strs[0] == "")
            {
                ATREEUI.SetSelectFocus(selectanalyzenolist);
            }
            else
            {
                foreach (string str in strs)
                {
                    string[] strxs = str.Split(':');
                    int analyzeno = int.Parse(strxs[0]);

                    if (selectanalyzenolist.IndexOf(analyzeno) < 0)
                        selectanalyzenolist.Add(analyzeno);
                }
                ATREEUI.SetSelectFocus(selectanalyzenolist);
            }
        }
        /// <summary>
        /// With Sub Selection
        /// </summary>
        /// <param name="strs"></param>
        void DISPSelectActionDX(string[] strs)
        {
            List<string> selectanalyzestringlist = new List<string>();

            if (strs[0] == "")
            {
                ATREEUI.SetSelectFocus(selectanalyzestringlist);

                AnalyzeSelectNo = -1;
                AINFOUI.SetAnalyze(null);
            }
            else
            {
                foreach (string str in strs)
                {
                    //string[] strxs = str.Split(':');
                    //int analyzeno = int.Parse(strxs[0]);

                    if (selectanalyzestringlist.IndexOf(str) < 0)
                        selectanalyzestringlist.Add(str);
                }
                ATREEUI.SetSelectFocus(selectanalyzestringlist);

                AnalyzeSelectNo = ATREEUI.FirstSelectNo;
                AINFOUI.SetAnalyze(AnalyzeSelectNow);
            }

            FillSUBUI(false);
        }
        private void ATREEUI_BackwardAction(List<int> selectnolist)
        {
            if (selectnolist.Count > 0)
            {
                foreach (AnalyzeClass analyze in AnalyzeList)
                {
                    int selectindex = selectnolist.IndexOf(analyze.No);

                    if (selectindex > -1)
                    {
                        analyze.SetMoverSelected(selectindex == 0, true);
                    }
                    else
                        analyze.SetMoverSelected(false);
                }
            }
            else
            {
                ClearMoverSelection();
            }

            DISPUI.MappingSelect();
        }
        private void AINFOUI_BackwardAction(List<int> selectindexlist)
        {
            AnalyzeSelectNow.SetMoverSelected(selectindexlist);
            DISPUI.MappingSelect();
        }
        private void numLevel_ValueChanged(object sender, EventArgs e)
        {
            if (!chkLock.Checked)
                return;

            DISPUI.Lock((int)numLevel.Value, chkIsOnly.Checked);
        }
        private void chkLock_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLock.Checked)
            {
                chkIsOnly.Enabled = true;

                ATREEUI.Lock(true);
                DISPUI.Lock((int)numLevel.Value, chkLock.Checked);
                numLevel.Enabled = true;
            }
            else
            {
                chkIsOnly.Checked = false;
                chkIsOnly.Enabled = false;

                ATREEUI.Lock(false);
                DISPUI.Lock(0, false);
                numLevel.Enabled = false;
            }
        }
        private void chkIsOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkLock.Checked)
                return;

            DISPUI.Lock((int)numLevel.Value, chkIsOnly.Checked);
        }
        #endregion

        #region Normal Operation
        public void SetPage(PageClass pagenow)
        {
            OPSource = OPSourceEnum.PAGE;

            PageNow = pagenow;
            AnalyzeRootNow = pagenow.AnalyzeRoot;

            ConverAnalyzeToList(AnalyzeRootNow);
            ATREEUI.SetAnalyzeTree(AnalyzeList);

            DISPUI.ClearAll();

            CollectMover();
            DISPUI.SetMover(myMoverCollection);

            DISPUI.ReplaceDisplayImage(PageNow.GetbmpORG((PageOPTypeEnum)PageNow.PageOPTypeIndex));

            FillDisplay();
        }
        public void RegetPage(PageOPTypeEnum pageoptype)
        {
            DISPUI.ReplaceDisplayImage(PageNow.GetbmpORG(pageoptype));
        }
        public void SetAnalyze(AnalyzeClass analyzenow, bool isfirsttime)
        {
            OPSource = OPSourceEnum.ANALYZE;

            if (isfirsttime)
            {
                AnalyzeRootNow = analyzenow;
                AnalyzeOperate = analyzenow;

                ConverAnalyzeToList(AnalyzeRootNow);
                ATREEUI.SetAnalyzeTree(AnalyzeList);
                ATREEUI.FillLearnList(AnalyzeOperate);
            }
            else
            {
                AnalyzeRootNow = analyzenow;

                ConverAnalyzeToList(AnalyzeRootNow);
                ATREEUI.SetAnalyzeTree(AnalyzeList);
            }

            DISPUI.ClearAll();

            CollectMover();
            DISPUI.SetMover(myMoverCollection);

            DISPUI.ReplaceDisplayImage(AnalyzeRootNow.bmpPATTERN);

            FillDisplay();
        }
        /// <summary>
        /// 將樹狀結構轉換成明細模式
        /// </summary>
        void ConverAnalyzeToList(AnalyzeClass analyze)
        {
            AnalyzeList.Clear();
            analyze.FillToList(AnalyzeList);
        }
        /// <summary>
        /// 將每個Analyze裏的Mover取得放到Collection裏
        /// </summary>
        void CollectMover()
        {
            myMoverCollection.Clear();

            foreach (AnalyzeClass analyze in AnalyzeList)
            {
                int i = 0;

                Mover mover = analyze.myMover;

                while (i < mover.Count)
                {
                    GraphicalObject grobj = mover[i].Source;

                    //if(IsLearn)
                    //{
                    //    if (analyze.LearnIndex == -1)   
                    //    {
                    //        PointF offset = analyze.myOringineOffsetPointF;

                    //        offset.X = -offset.X;
                    //        offset.Y = -offset.Y;

                    //        (grobj as GeoFigure).MappingToMovingObject(offset, new SizeF(1f, 1f));
                    //    }

                    //}

                    (grobj as GeoFigure).IsSelected = false;
                    (grobj as GeoFigure).IsFirstSelected = false;

                    (grobj as GeoFigure).RelateNo = analyze.No;
                    (grobj as GeoFigure).RelatePosition = i;
                    (grobj as GeoFigure).RelateLevel = analyze.Level;
                    (grobj as GeoFigure).LearnCount = analyze.LearnList.Count;

                    myMoverCollection.Add(grobj);

                    i++;
                }
            }
        }
        /// <summary>
        /// 將新增的Analyze的Mover加入進去
        /// </summary>
        /// <param name="newaddanalyzelist"></param>
        void AddMover(List<AnalyzeClass> newaddanalyzelist)
        {
            foreach (AnalyzeClass analyze in newaddanalyzelist)
            {
                int i = 0;
                Mover mover = analyze.myMover;
                while (i < mover.Count)
                {
                    GraphicalObject grobj = mover[i].Source;

                    myMoverCollection.Add(grobj);

                    i++;
                }
            }
        }
        /// <summary>
        /// 刪除所選Analyze的Mover
        /// </summary>
        /// <param name="deleteanalyzelist"></param>
        void DeleteMover(List<AnalyzeClass> deleteanalyzelist)
        {
            foreach (AnalyzeClass analyze in deleteanalyzelist)
            {
                int i = myMoverCollection.Count - 1;

                while (i > -1)
                {
                    GraphicalObject grpobj = myMoverCollection[i].Source;

                    if ((grpobj as GeoFigure).RelateNo == analyze.No)
                    {
                        myMoverCollection.RemoveAt(i);
                    }
                    i--;
                }
            }
        }
        /// <summary>
        /// 將Mover裏的選擇清除
        /// </summary>
        void ClearMoverSelection()
        {
            foreach (AnalyzeClass analyze in AnalyzeList)
            {
                int i = 0;

                Mover mover = analyze.myMover;

                while (i < mover.Count)
                {
                    GraphicalObject grobj = mover[i].Source;

                    (grobj as GeoFigure).IsSelected = false;
                    (grobj as GeoFigure).IsFirstSelected = false;

                    i++;
                }
            }
        }
        void FillDisplay()
        {



        }
        /// <summary>
        /// 按住Control多選
        /// </summary>
        public void HoldSelect()
        {
            DISPUI.HoldSelect();
        }
        /// <summary>
        /// 放開Control不選了
        /// </summary>
        public void ReleaseSelect()
        {
            DISPUI.ReleaseSelect();
        }
        /// <summary>
        /// 移動所有的Mover
        /// </summary>
        /// <param name="KEY"></param>
        public void MoveMover(Keys KEY)
        {
            switch (KEY)
            {
                case Keys.Left:
                    DISPUI.MoveMover(-Step, 0);
                    break;
                case Keys.Right:
                    DISPUI.MoveMover(Step, 0);
                    break;
                case Keys.Up:
                    DISPUI.MoveMover(0, -Step);
                    break;
                case Keys.Down:
                    DISPUI.MoveMover(0, Step);
                    break;
            }
        }
        /// <summary>
        /// 縮放所有的Mover
        /// </summary>
        /// <param name="KEY"></param>
        public void SizeMover(Keys KEY)
        {
            switch (KEY)
            {
                case Keys.Left:
                    DISPUI.SizeMover(-Step, 0);
                    break;
                case Keys.Right:
                    DISPUI.SizeMover(Step, 0);
                    break;
                case Keys.Up:
                    DISPUI.SizeMover(0, -Step);
                    break;
                case Keys.Down:
                    DISPUI.SizeMover(0, Step);
                    break;
            }
        }
        /// <summary>
        /// 比對圖時使用
        /// </summary>
        /// <param name="bmpmatching"></param>
        /// <param name="matchingmethod"></param>
        public void SetMatching(Bitmap bmpmatching, MatchMethodEnum matchingmethod)
        {
            DISPUI.SetMatching(bmpmatching, matchingmethod);
        }
        /// <summary>
        /// 新增分支
        /// </summary>
        public void AddBranchLevel()
        {
            ATREEUI.AddBranchLevel();

        }
        /// <summary>
        /// 新增同層
        /// </summary>
        public void AddSameLevel()
        {
            ATREEUI.AddSameLevel();
        }
        /// <summary>
        /// 複製所選項目
        /// </summary>
        public void Dup()
        {
            ATREEUI.Dup();
        }
        /// <summary>
        /// 刪除所選項目
        /// </summary>
        public void Delete()
        {
            ATREEUI.Delete();
        }
        /// <summary>
        /// 增加一個SHAPE
        /// </summary>
        /// <param name="shape"></param>
        public void AddShape(ShapeEnum shape, string opstring)
        {
            if (AnalyzeSelectNo > -1)
            {
                AnalyzeSelectNow.AddShape(shape, myMoverCollection);

                DISPUI.RefreshDisplayShape();
                DISPUI.SetMover(myMoverCollection);

                DISPUI.MappingSelect();

                AINFOUI.SetAnalyze(AnalyzeSelectNow);

                FillSUBUI(false);
            }
            else
                MessageBox.Show("請選擇要新增的項目");
        }
        /// <summary>
        /// 刪除所選SHAPE
        /// </summary>
        /// <param name="shape"></param>
        public void DelShape(ShapeEnum shape, string opstring)
        {
            if (AnalyzeSelectNo > -1)
            {
                AnalyzeSelectNow.DelShape(myMoverCollection);

                DISPUI.RefreshDisplayShape();
                DISPUI.SetMover(myMoverCollection);

                DISPUI.MappingSelect();

                AINFOUI.SetAnalyze(AnalyzeSelectNow);

                FillSUBUI(false);
            }
            else
                MessageBox.Show("請選擇要刪除的項目");
        }
        /// <summary>
        /// 變更所選SHAPE
        /// </summary>
        /// <param name="shape"></param>
        public void ReviseShape(ShapeEnum shape, string opstring)
        {
            if (AnalyzeSelectNo > -1)
            {
                //原來是只改變所選擇的一個
                //AnalyzeSelectNow.ReviseShape(shape,myMoverCollection);

                //改成所選的全部改掉
                foreach (AnalyzeClass analyze in AnalyzeList)
                {
                    if (analyze.IsSelected)
                    {
                        analyze.ReviseShape(shape, myMoverCollection);
                    }
                }

                DISPUI.RefreshDisplayShape();
                DISPUI.SetMover(myMoverCollection);

                DISPUI.MappingSelect();

                AINFOUI.SetAnalyze(AnalyzeSelectNow);

                FillSUBUI(false);
            }
            else
                MessageBox.Show("請選擇要變更的項目");

        }
        /// <summary>
        /// 顯示區域外框
        /// </summary>
        public void ShowRange()
        {
            Mover showmover = new Mover();

            DISPUI.BackupImage();

            Bitmap bmpdraw = new Bitmap(PageNow.GetbmpORG((PageOPTypeEnum)PageNow.PageOPTypeIndex));

            //bmpdraw.Save(Universal.TESTPATH + "\\ANALYZETEST\\DRAW" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

            PageNow.AnalyzeRoot.GetShowMover(showmover, bmpdraw);
            DISPUI.SetStaticMover(showmover);

            DISPUI.ReplaceDisplayImage(bmpdraw);

            DISPUI.SaveScreen();

            bmpdraw.Dispose();
        }
        /// <summary>
        /// 清除區域外框
        /// </summary>
        public void ClearRange()
        {
            DISPUI.ClearStaticMover();
            DISPUI.RestoreImage();
        }

        /// <summary>
        /// 顯示圖形差異
        /// </summary>
        public void ShowLumina()
        {
            Mover showmover = new Mover();
            JzFindObjectClass jzfind = new JzFindObjectClass();
            DISPUI.BackupImage();

            Bitmap bmpdraw = new Bitmap(PageNow.GetbmpORG((PageOPTypeEnum)PageNow.PageOPTypeIndex));

            //bmpdraw.Save(Universal.TESTPATH + "\\ANALYZETEST\\DRAW" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

            //PageNow.AnalyzeRoot.GetShowMover(showmover, bmpdraw);
            //DISPUI.SetStaticMover(showmover);

            jzfind.SetColorThreshold(bmpdraw, new Rectangle(0, 0, bmpdraw.Width, bmpdraw.Height), false);

            DISPUI.ReplaceDisplayImage(bmpdraw);

            DISPUI.SaveScreen();

            bmpdraw.Dispose();
        }

        CheckForm CHECKFRM;
        StiltsForm Stiltsui;


        public void CheckGresyscale()
        {
            if (AnalyzeSelectNow != null)
            {
                //產生Mask相關資料
                //AnalyzeSelectNow.IsTempSave = true;
                AnalyzeSelectNow.A02_CreateTrainRequirement(PageNow.GetbmpORG((PageOPTypeEnum)PageNow.PageOPTypeIndex), new PointF(0, 0));

                int gapvalue = 5;
                HistogramClass histogram = new HistogramClass(gapvalue);

                histogram.GetHistogram(AnalyzeSelectNow.bmpPATTERN, AnalyzeSelectNow.bmpMASK, true);

                //列出前10個最多的數值

                int i = 0;
                string showstr = "";
                float totalratio = 0f;

                int max = -1000;
                int min = 10000;

                while (i < 5)
                {
                    int gradecount = histogram.GetGradeCount(i);
                    int gradevalue = histogram.GetGradeValue(i) * gapvalue;
                    totalratio += gradecount;

                    max = Math.Max(gradevalue, max);
                    min = Math.Min(gradevalue, min);

                    showstr += (gradevalue).ToString().PadRight(5, ' ') + "= "
                        + (((float)gradecount * 100f) / (float)histogram.TotalPixelForCount).ToString("0.00") + "%" + Environment.NewLine;

                    i++;
                }
                showstr += Environment.NewLine + "Min : " + min.ToString().PadRight(5, ' ') + ", Max : " + max.ToString().PadRight(5, ' ');
                showstr += Environment.NewLine + "Total Ratio = " + (totalratio * 100f / (float)histogram.TotalPixelForCount).ToString("0.00") + "%";

                CHECKFRM = new CheckForm(showstr);
                CHECKFRM.Show();
            }
        }
        public void CheckColor(bool islong)
        {
            if (AnalyzeSelectNow != null && AnalyzeSelectNow.MEASUREPara.MeasureMethod == MeasureMethodEnum.COLORCHECK)
            {
                //產生Mask相關資料
                //AnalyzeSelectNow.IsTempSave = true;
                AnalyzeSelectNow.A02_CreateTrainRequirement(PageNow.GetbmpORG((PageOPTypeEnum)PageNow.PageOPTypeIndex), new PointF(0, 0));

                JzFindObjectClass jzfind = new JzFindObjectClass();
                Bitmap bmp = new Bitmap(AnalyzeSelectNow.bmpPATTERN);

                jzfind.GetMaskedImage(bmp, AnalyzeSelectNow.bmpMASK, Color.White);

                //  bmp.Save(Universal.TESTPATH + "\\ANALYZETEST\\COLORMASKED" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                ColorMeasureClass colorMeasure = new ColorMeasureClass(AnalyzeSelectNow.MEASUREPara.MMOPString);

                string reportstr = colorMeasure.CheckRatioReport(bmp);

                bmp.Dispose();

                if (islong)
                    CHECKFRM = new CheckForm(reportstr.Split('#')[1]);
                else
                    CHECKFRM = new CheckForm(reportstr.Split('#')[0]);

                CHECKFRM.Show();
            }
        }
        public void CheckNoGlueMeanValue()
        {
            if (AnalyzeSelectNow != null)
            {
                AnalyzeSelectNow.A02_CreateTrainRequirement(PageNow.GetbmpORG((PageOPTypeEnum)PageNow.PageOPTypeIndex), new PointF(0, 0));
                //Bitmap bmpFourSide = new Bitmap(AnalyzeSelectNow.bmpPATTERN,
                //    new Size(AnalyzeSelectNow.bmpPATTERN.Width >> 3, AnalyzeSelectNow.bmpPATTERN.Height >> 3));
                //m_Histogram.GetHistogram(bmpFourSide, 100);
                int _mean = 0;// m_Histogram.MeanGrade;

                _mean = AnalyzeSelectNow.PADPara.GetGrayMinValue(AnalyzeSelectNow.bmpPATTERN);

                CHECKFRM = new CheckForm($"芯片周边灰阶值：{_mean}");
                CHECKFRM.Show();
            }
        }

        public void CheckStilts()
        {

            if (AnalyzeSelectNow == null)
                return;
            Stiltsui = new StiltsForm();



            AnalyzeSelectNow.ResetTrainStatus();
            AnalyzeSelectNow.ResetRunStatus();

            if (!IsLearn)
                AnalyzeSelectNow.Z02_CreateTrainRequirement(PageNow.GetbmpORG(), new PointF(0, 0));
            AnalyzeSelectNow.Z05_AlignTrainProcess();

            Stiltsui.Show();
            Stiltsui.stiltsUI.SetStilts(AnalyzeSelectNow);

            Stiltsui.TopMost = true;

        }
        public void StiltsDispose()
        {
            if (Stiltsui != null)
                Stiltsui.Dispose();

        }


        public void CheckColorRootNow(bool islong)
        {
            if (AnalyzeRootNow != null && AnalyzeRootNow.MEASUREPara.MeasureMethod == MeasureMethodEnum.COLORCHECK)
            {
                //產生Mask相關資料
                //AnalyzeSelectNow.IsTempSave = true;
                //  AnalyzeSelectNow.A02_CreateTrainRequirement(PageNow.GetbmpORG((PageOPTypeEnum)PageNow.PageOPTypeIndex), new PointF(0, 0));

                JzFindObjectClass jzfind = new JzFindObjectClass();
                Bitmap bmp = new Bitmap(AnalyzeRootNow.bmpPATTERN);

                Bitmap bmpmask = new Bitmap(AnalyzeRootNow.bmpPATTERN.Width, AnalyzeRootNow.bmpPATTERN.Height);
                Graphics g = Graphics.FromImage(bmpmask);
                g.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, bmpmask.Width, bmpmask.Height));
                g.Dispose();

                jzfind.GetMaskedImage(bmp, bmpmask, Color.White);

                //  bmp.Save(Universal.TESTPATH + "\\ANALYZETEST\\COLORMASKED" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                ColorMeasureClass colorMeasure = new ColorMeasureClass(AnalyzeRootNow.MEASUREPara.MMOPString);

                string reportstr = colorMeasure.CheckRatioReport(bmp);

                bmp.Dispose();

                if (islong)
                    CHECKFRM = new CheckForm(reportstr.Split('#')[1]);
                else
                    CHECKFRM = new CheckForm(reportstr.Split('#')[0]);

                CHECKFRM.Show();
            }
        }
        public void EndCheck()
        {
            if (CHECKFRM != null)
                CHECKFRM.Dispose();
        }

        #endregion

        #region Outside Operation
        public bool IsPageSelectCorrect()
        {
            bool ret = false;

            ret = AnalyzeSelectNo > 1;

            return ret;
        }

        public void DelAllRegion()
        {
            //清除外框 重新加入新的
            int count = AnalyzeList.Count;
            int i = 0;
            List<int> delnolist = new List<int>();
            while (i < count)
            {
                AnalyzeClass analyze = AnalyzeList[i];
                //if (analyze.No == AnalyzeSelectNow.No)
                //{
                //    foreach (AnalyzeClass analyze1 in analyze.BranchList)
                //    {
                //        delnolist.Add(analyze1.No);
                //    }

                //    i++;
                //    continue;
                //}

                if (analyze.Level == 2)
                    delnolist.Add(analyze.No);

                i++;
            }

            if (delnolist.Count > 0)
            {
                ATREEUI.FirstSelectNo = delnolist[0];
                ATREEUI.CollapseTree();
                ATREEUI.Delete(delnolist, false);
            }
        }

        JzFindObjectClass m_Find = new JzFindObjectClass();
        HistogramClass m_Histogram = new HistogramClass(2);
        JzToolsClass JzTool = new JzToolsClass();

        public Bitmap m_bmpPattern = new Bitmap(1, 1);
        RectangleF m_RectClone = new RectangleF();

        public RectangleF RectClone
        {
            get { return m_RectClone; }
        }
        public void FindSimilar(float tolerance)
        {
            List<DoffsetClass> DoffsetList = new List<DoffsetClass>();
            ATREEUI.DismissAnalyzeTable();
            //Get the Anallyze Data For Find
            Bitmap bmpPageOrg = PageNow.GetbmpORG((PageOPTypeEnum)PageNow.PageOPTypeIndex);

            //AnalyzeSelectNow.IsTempSave = true;

            //先把 Analyze Train 完
            AnalyzeSelectNow.A02_CreateTrainRequirement(bmpPageOrg, new PointF(0, 0));
            AnalyzeSelectNow.A05_AlignTrainProcess();

            AnalyzeSelectNow.B08_RunAndFindSimilar(bmpPageOrg, tolerance, DoffsetList);

            //AnalyzeSelectNow.IsTempSave = false;

            //取得自身的區域
            int selectno = AnalyzeSelectNow.No;
            RectangleF myRectF = AnalyzeSelectNow.GetMoverRectF(bmpPageOrg);
            List<RectangleF> OrgRectFList = new List<RectangleF>();

            int i = 0;
            switch (Universal.OPTION)
            {
                case OptionEnum.MAIN_SDM3:
                case JetEazy.OptionEnum.MAIN_X6:
                case JetEazy.OptionEnum.MAIN_SD:
                case JetEazy.OptionEnum.MAIN_SDM1:
                case OptionEnum.MAIN_SDM2:
                case OptionEnum.MAIN_SERVICE:
                    //清除外框 重新加入新的
                    int count = AnalyzeList.Count;
                    i = 0;
                    List<int> delnolist = new List<int>();
                    while (i < count)
                    {
                        AnalyzeClass analyze = AnalyzeList[i];
                        if (analyze.No == AnalyzeSelectNow.No)
                        {
                            foreach (AnalyzeClass analyze1 in analyze.BranchList)
                            {
                                delnolist.Add(analyze1.No);
                            }

                            i++;
                            continue;
                        }

                        if (analyze.Level == 2)
                            delnolist.Add(analyze.No);

                        i++;
                    }

                    if (delnolist.Count > 0)
                    {
                        ATREEUI.CollapseTree();
                        ATREEUI.Delete(delnolist, false);
                    }

                    break;
            }


            //Bitmap bmpDraw = new Bitmap(bmpPageOrg);
            ////找blob 中心
            //Bitmap bmpAnalyzeSelectNow = (Bitmap)bmpPageOrg.Clone(myRectF, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //m_Histogram.GetHistogram(bmpAnalyzeSelectNow);
            //m_Find.SetThreshold(bmpAnalyzeSelectNow, JzTool.SimpleRect(bmpAnalyzeSelectNow.Size), m_Histogram.MinGrade, m_Histogram.MeanGrade- m_Histogram.MinGrade, 0, true);
            //m_Find.Find(bmpAnalyzeSelectNow, Color.Red);
            ////bmpAnalyzeSelectNow.Save("D:\\LOA\\TrainModel\\bmpAnalyzeSelectNow.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            //Rectangle MaxRect = m_Find.rectMaxRect;

            //MaxRect.X += (int)myRectF.X;
            //MaxRect.Y += (int)myRectF.Y;

            ////JzTool.DrawRectEx(bmpAnalyzeSelectNow, MaxRect, new Pen(Color.Lime, 2));
            ////bmpAnalyzeSelectNow.Save("D:\\LOA\\TrainModel\\bmpAnalyzeSelectNow1.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            ////JzTool.DrawRectEx(bmpDraw, MaxRect, new Pen(Color.Lime, 2));


            //取得所有定義過的區域
            foreach (AnalyzeClass analyze in AnalyzeList)
            {
                if (analyze.No != 1)
                {
                    OrgRectFList.Add(analyze.GetMoverRectF(bmpPageOrg));
                }
            }

            bool IsIncluded = false;


            i = 0;

            //Check Duplicate and Copy Ananlyze
            foreach (DoffsetClass doffset in DoffsetList)
            {
                IsIncluded = false;

                RectangleF foundrectf = OffsetRect(myRectF, doffset.OffsetF);

                ////foundrectf.Inflate(-40, -40);
                //bmpAnalyzeSelectNow = (Bitmap)bmpPageOrg.Clone(foundrectf, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                //m_Histogram.GetHistogram(bmpAnalyzeSelectNow);
                //m_Find.SetThreshold(bmpAnalyzeSelectNow, JzTool.SimpleRect(bmpAnalyzeSelectNow.Size), m_Histogram.MinGrade, m_Histogram.MeanGrade - m_Histogram.MinGrade, 0, true);
                //m_Find.Find(bmpAnalyzeSelectNow, Color.Red);
                ////bmpAnalyzeSelectNow.Save("D:\\LOA\\TrainModel\\bmpAnalyzeSelectNow.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                //MaxRect = m_Find.rectMaxRect;

                //MaxRect.X += (int)foundrectf.X;
                //MaxRect.Y += (int)foundrectf.Y;

                //Point pt = JzTool.GetRectCenter(MaxRect);

                //DoffsetClass tmp = new DoffsetClass(doffset.Degree, pt);
                ////tmp.OffsetF.X -= pt.X;
                ////tmp.OffsetF.Y -= pt.Y;



                ////JzTool.DrawRectEx(bmpAnalyzeSelectNow, MaxRect, new Pen(Color.Lime, 2));
                //bmpAnalyzeSelectNow.Save("D:\\LOA\\TrainModel\\bmpAnalyzeSelectNow" + i.ToString() + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                //JzTool.DrawRectEx(bmpDraw, MaxRect, new Pen(Color.Red, 2));

                //檢查是否有和現有的Analyze重覆超過30%的，有的話視為同一個就不加了
                foreach (RectangleF rectf in OrgRectFList)
                {
                    RectangleF rectfintersect = foundrectf;
                    rectfintersect.Intersect(rectf);

                    if ((float)(rectfintersect.Width * rectfintersect.Height) / (float)(rectf.Width * rectf.Height) > 0.3)
                    {
                        IsIncluded = true;
                        break;
                    }
                }

                //检查是否在大定位框里 不在的话 不要了

                if (!IsIncluded)
                {
                    RectangleF rectfintersectORG = foundrectf;
                    rectfintersectORG.Intersect(PageNow.AnalyzeRoot.GetMoverRectF(bmpPageOrg));

                    if ((float)(rectfintersectORG.Width * rectfintersectORG.Height) / (float)(foundrectf.Width * foundrectf.Height) < 0.8)
                    {
                        IsIncluded = true;
                    }
                }

                //若沒有被包含則加入框裏
                if (!IsIncluded)
                    ATREEUI.AddSameLevel(selectno, doffset, new PointF(myRectF.X + myRectF.Width / 2, myRectF.Y + myRectF.Height / 2));

                i++;
            }
            ATREEUI.RelateAnalyzeTable();
            //bmpDraw.Save("D:\\LOA\\TrainModel\\bmpDraw.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

        }
        public void FindInside(float threshold, int extend)
        {
            int i = 0;

            List<DoffsetClass> DoffsetList = new List<DoffsetClass>();

            //Get the Anallyze Data For Find
            Bitmap bmpPageOrg = PageNow.GetbmpORG((PageOPTypeEnum)PageNow.PageOPTypeIndex);

            int count = AnalyzeList.Count;

            while (i < count)
            //foreach(AnalyzeClass analyze in AnalyzeList)
            {
                if (i >= AnalyzeList.Count)
                    break;

                AnalyzeClass analyze = AnalyzeList[i];
                List<int> delnolist = new List<int>();

                if (analyze.IsSelected && analyze.No != 1)
                {
                    List<Rectangle> foundrectlist = new List<Rectangle>();

                    //analyze.IsTempSave = true;

                    switch (VERSION)
                    {
                        case VersionEnum.ALLINONE:

                            switch (OPTION)
                            {
                                case OptionEnum.MAIN_SDM3:
                                case OptionEnum.MAIN_X6:
                                case OptionEnum.MAIN_SERVICE:

                                    foreach (AnalyzeClass analyze1 in analyze.BranchList)
                                    {
                                        delnolist.Add(analyze1.No);
                                    }
                                    ATREEUI.Delete(delnolist);

                                    //FindAnalyzeBlob(analyze, bmpPageOrg, new PointF(0, 0), foundrectlist, threshold, extend);
                                    analyze.B02_CreateFindInsideRequirement(bmpPageOrg, new PointF(0, 0), foundrectlist, threshold, extend);
                                    break;
                            }

                            break;
                        default:
                            analyze.B02_CreateFindInsideRequirement(bmpPageOrg, new PointF(0, 0), foundrectlist, threshold, extend);
                            break;
                    }

                    //analyze.IsTempSave = false;

                    if (foundrectlist.Count > 0)
                    {
                        foreach (Rectangle foundrect in foundrectlist)
                            ATREEUI.AddBranchLevel(analyze.No, foundrect, extend);
                    }
                }
                i++;
            }

        }
        public void FindInside(List<Rectangle> foundrectlist, AnalyzeClass analyze = null)
        {

            List<int> delnolist = new List<int>();

            switch (VERSION)
            {
                case VersionEnum.ALLINONE:

                    switch (OPTION)
                    {
                        case OptionEnum.MAIN_X6:
                        case OptionEnum.MAIN_SERVICE:
                        case OptionEnum.MAIN_SDM3:

                            ATREEUI.DismissAnalyzeTable();

                            foreach (AnalyzeClass analyze1 in AnalyzeSelectNow.BranchList)
                            {
                                delnolist.Add(analyze1.No);
                            }
                            ATREEUI.Delete(delnolist);


                            if (foundrectlist.Count > 0)
                            {
                                foreach (Rectangle foundrect in foundrectlist)
                                {
                                    //ATREEUI.AddBranchLevel(AnalyzeSelectNow.No, foundrect, 0);

                                    ATREEUI.AddBranchLevel(AnalyzeSelectNow.No, foundrect, 0, 0, analyze);
                                }
                            }

                            ATREEUI.RelateAnalyzeTable();

                            break;
                    }

                    break;
            }

        }
        public void FindSimilar(List<DoffsetClass> DoffsetList)
        {
            //List<DoffsetClass> DoffsetList = new List<DoffsetClass>();

            //Get the Anallyze Data For Find
            Bitmap bmpPageOrg = PageNow.GetbmpORG((PageOPTypeEnum)PageNow.PageOPTypeIndex);

            //AnalyzeSelectNow.IsTempSave = true;

            //先把 Analyze Train 完
            //AnalyzeSelectNow.A02_CreateTrainRequirement(bmpPageOrg, new PointF(0, 0));
            //AnalyzeSelectNow.A05_AlignTrainProcess();

            //AnalyzeSelectNow.B08_RunAndFindSimilar(bmpPageOrg, tolerance, DoffsetList);

            //AnalyzeSelectNow.IsTempSave = false;

            //取得自身的區域
            int selectno = AnalyzeSelectNow.No;
            RectangleF myRectF = AnalyzeSelectNow.GetMoverRectF(bmpPageOrg);
            List<RectangleF> OrgRectFList = new List<RectangleF>();

            int i = 0;
            switch (Universal.OPTION)
            {
                case OptionEnum.MAIN_SDM3:
                case JetEazy.OptionEnum.MAIN_X6:
                case JetEazy.OptionEnum.MAIN_SD:
                case JetEazy.OptionEnum.MAIN_SDM1:
                case OptionEnum.MAIN_SDM2:
                case OptionEnum.MAIN_SERVICE:
                    //清除外框 重新加入新的

                    //DelAllRegion();

                    int count = AnalyzeList.Count;
                    i = 0;
                    List<int> delnolist = new List<int>();
                    while (i < count)
                    {
                        AnalyzeClass analyze = AnalyzeList[i];
                        if (analyze.No == AnalyzeSelectNow.No)
                        {
                            foreach (AnalyzeClass analyze1 in analyze.BranchList)
                            {
                                delnolist.Add(analyze1.No);
                            }

                            i++;
                            continue;
                        }

                        if (analyze.Level == 2)
                            delnolist.Add(analyze.No);

                        i++;
                    }

                    if (delnolist.Count > 0)
                    {
                        ATREEUI.CollapseTree();
                        ATREEUI.Delete(delnolist, false);
                    }


                    break;
            }

            //取得所有定義過的區域
            foreach (AnalyzeClass analyze in AnalyzeList)
            {
                if (analyze.No != 1)
                {
                    OrgRectFList.Add(analyze.GetMoverRectF(bmpPageOrg));
                }
            }

            bool IsIncluded = false;


            i = 0;

            //Check Duplicate and Copy Ananlyze
            foreach (DoffsetClass doffset in DoffsetList)
            {
                IsIncluded = false;

                if (doffset.OffsetF.X < 0 * bmpPageOrg.Width || doffset.OffsetF.X > 1 * bmpPageOrg.Width)
                {
                    IsIncluded = true;
                }

                DoffsetClass doffset1 = new DoffsetClass(0, new PointF(doffset.OffsetF.X, doffset.OffsetF.Y));
                doffset1.OffsetF.X -= 0 * bmpPageOrg.Width;

                RectangleF foundrectf = OffsetRect(myRectF, doffset1.OffsetF);

                if (!IsIncluded)
                {
                    //檢查是否有和現有的Analyze重覆超過30%的，有的話視為同一個就不加了
                    foreach (RectangleF rectf in OrgRectFList)
                    {
                        RectangleF rectfintersect = foundrectf;
                        rectfintersect.Intersect(rectf);

                        if ((float)(rectfintersect.Width * rectfintersect.Height) / (float)(rectf.Width * rectf.Height) > 0.3)
                        {
                            IsIncluded = true;
                            break;
                        }
                    }
                }

                ////检查是否在大定位框里 不在的话 不要了

                //if (!IsIncluded)
                //{
                //    RectangleF rectfintersectORG = foundrectf;
                //    rectfintersectORG.Intersect(PageNow.AnalyzeRoot.GetMoverRectF(bmpPageOrg));

                //    if ((float)(rectfintersectORG.Width * rectfintersectORG.Height) / (float)(foundrectf.Width * foundrectf.Height) < 0.8)
                //    {
                //        IsIncluded = true;
                //    }
                //}

                //若沒有被包含則加入框裏
                if (!IsIncluded)
                    ATREEUI.AddSameLevel(selectno, doffset1, new PointF(myRectF.X + myRectF.Width / 2, myRectF.Y + myRectF.Height / 2));

                i++;
            }

            //bmpDraw.Save("D:\\LOA\\TrainModel\\bmpDraw.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

        }


        public void FindSimilarEx(float tolerance,float eCompressed=0.5f)
        {

            ATREEUI.DismissAnalyzeTable();
            List<DoffsetClass> DoffsetList = new List<DoffsetClass>();

            //Get the Anallyze Data For Find
            Bitmap bmpPageOrg = PageNow.GetbmpORG((PageOPTypeEnum)PageNow.PageOPTypeIndex);

            //AnalyzeSelectNow.IsTempSave = true;

            //先把 Analyze Train 完
            //AnalyzeSelectNow.A02_CreateTrainRequirement(bmpPageOrg, new PointF(0, 0));
            //AnalyzeSelectNow.A05_AlignTrainProcess();

            //AnalyzeSelectNow.B08_RunAndFindSimilar(bmpPageOrg, tolerance, DoffsetList);

            m_RectClone = AnalyzeSelectNow.GetMoverRectF(bmpPageOrg);
            //m_RectClone.Inflate(-AnalyzeSelectNow.ExtendX, -AnalyzeSelectNow.ExtendY);


            OpencvMatchClass opencvMatch = new OpencvMatchClass();
            m_bmpPattern.Dispose();
            m_bmpPattern = bmpPageOrg.Clone(m_RectClone, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Bitmap bitmap = opencvMatch.Recoganize(bmpPageOrg, m_bmpPattern, DoffsetList, tolerance, eCompressed);
            //bitmap.Save("D:\\_tmp\\bmpMatched.png", System.Drawing.Imaging.ImageFormat.Png);
            //AnalyzeSelectNow.IsTempSave = false;
            bitmap.Dispose();
            //bmppattern.Dispose();


            //取得自身的區域
            int selectno = AnalyzeSelectNow.No;
            RectangleF myRectF = AnalyzeSelectNow.GetMoverRectF(bmpPageOrg);
            List<RectangleF> OrgRectFList = new List<RectangleF>();

            //bmpPageOrg.Dispose();
            int i = 0;
            switch (Universal.OPTION)
            {
                case OptionEnum.MAIN_SDM3:
                case JetEazy.OptionEnum.MAIN_X6:
                case JetEazy.OptionEnum.MAIN_SD:
                case JetEazy.OptionEnum.MAIN_SDM1:
                case OptionEnum.MAIN_SDM2:
                case OptionEnum.MAIN_SERVICE:
                    //清除外框 重新加入新的
                    int count = AnalyzeList.Count;
                    i = 0;
                    List<int> delnolist = new List<int>();
                    while (i < count)
                    {
                        AnalyzeClass analyze = AnalyzeList[i];
                        if (analyze.No == AnalyzeSelectNow.No)
                        {
                            foreach (AnalyzeClass analyze1 in analyze.BranchList)
                            {
                                delnolist.Add(analyze1.No);
                            }

                            i++;
                            continue;
                        }

                        if (analyze.Level == 2)
                            delnolist.Add(analyze.No);

                        i++;
                    }

                    if (delnolist.Count > 0)
                    {
                        ATREEUI.CollapseTree();
                        ATREEUI.Delete(delnolist, false);
                    }



                    break;
            }


            //Bitmap bmpDraw = new Bitmap(bmpPageOrg);
            ////找blob 中心
            //Bitmap bmpAnalyzeSelectNow = (Bitmap)bmpPageOrg.Clone(myRectF, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //m_Histogram.GetHistogram(bmpAnalyzeSelectNow);
            //m_Find.SetThreshold(bmpAnalyzeSelectNow, JzTool.SimpleRect(bmpAnalyzeSelectNow.Size), m_Histogram.MinGrade, m_Histogram.MeanGrade- m_Histogram.MinGrade, 0, true);
            //m_Find.Find(bmpAnalyzeSelectNow, Color.Red);
            ////bmpAnalyzeSelectNow.Save("D:\\LOA\\TrainModel\\bmpAnalyzeSelectNow.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            //Rectangle MaxRect = m_Find.rectMaxRect;

            //MaxRect.X += (int)myRectF.X;
            //MaxRect.Y += (int)myRectF.Y;

            ////JzTool.DrawRectEx(bmpAnalyzeSelectNow, MaxRect, new Pen(Color.Lime, 2));
            ////bmpAnalyzeSelectNow.Save("D:\\LOA\\TrainModel\\bmpAnalyzeSelectNow1.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            ////JzTool.DrawRectEx(bmpDraw, MaxRect, new Pen(Color.Lime, 2));


            //取得所有定義過的區域
            foreach (AnalyzeClass analyze in AnalyzeList)
            {
                if (analyze.No != 1)
                {
                    OrgRectFList.Add(analyze.GetMoverRectF(bmpPageOrg));
                }
            }

            bool IsIncluded = false;


            i = 0;

            //Check Duplicate and Copy Ananlyze
            foreach (DoffsetClass doffset in DoffsetList)
            {
                IsIncluded = false;

                RectangleF foundrectf = OffsetRect(myRectF, doffset.OffsetF);

                ////foundrectf.Inflate(-40, -40);
                //bmpAnalyzeSelectNow = (Bitmap)bmpPageOrg.Clone(foundrectf, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                //m_Histogram.GetHistogram(bmpAnalyzeSelectNow);
                //m_Find.SetThreshold(bmpAnalyzeSelectNow, JzTool.SimpleRect(bmpAnalyzeSelectNow.Size), m_Histogram.MinGrade, m_Histogram.MeanGrade - m_Histogram.MinGrade, 0, true);
                //m_Find.Find(bmpAnalyzeSelectNow, Color.Red);
                ////bmpAnalyzeSelectNow.Save("D:\\LOA\\TrainModel\\bmpAnalyzeSelectNow.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                //MaxRect = m_Find.rectMaxRect;

                //MaxRect.X += (int)foundrectf.X;
                //MaxRect.Y += (int)foundrectf.Y;

                //Point pt = JzTool.GetRectCenter(MaxRect);

                //DoffsetClass tmp = new DoffsetClass(doffset.Degree, pt);
                ////tmp.OffsetF.X -= pt.X;
                ////tmp.OffsetF.Y -= pt.Y;



                ////JzTool.DrawRectEx(bmpAnalyzeSelectNow, MaxRect, new Pen(Color.Lime, 2));
                //bmpAnalyzeSelectNow.Save("D:\\LOA\\TrainModel\\bmpAnalyzeSelectNow" + i.ToString() + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                //JzTool.DrawRectEx(bmpDraw, MaxRect, new Pen(Color.Red, 2));

                //檢查是否有和現有的Analyze重覆超過30%的，有的話視為同一個就不加了
                foreach (RectangleF rectf in OrgRectFList)
                {
                    RectangleF rectfintersect = foundrectf;
                    rectfintersect.Intersect(rectf);

                    if ((float)(rectfintersect.Width * rectfintersect.Height) / (float)(rectf.Width * rectf.Height) > 0.5)
                    {
                        IsIncluded = true;
                        break;
                    }
                }

                //检查是否在大定位框里 不在的话 不要了

                if (!IsIncluded)
                {
                    RectangleF rectfintersectORG = foundrectf;
                    rectfintersectORG.Intersect(PageNow.AnalyzeRoot.GetMoverRectF(bmpPageOrg));

                    if ((float)(rectfintersectORG.Width * rectfintersectORG.Height) / (float)(foundrectf.Width * foundrectf.Height) < 0.8)
                    {
                        IsIncluded = true;
                    }
                }

                //若沒有被包含則加入框裏
                if (!IsIncluded)
                    ATREEUI.AddSameLevel(selectno, doffset, new PointF(myRectF.X + myRectF.Width / 2, myRectF.Y + myRectF.Height / 2));

                i++;
            }
            ATREEUI.RelateAnalyzeTable();
            //bmpDraw.Save("D:\\LOA\\TrainModel\\bmpDraw.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

        }

        #region 寻找选定框内部的blob

        private void FindAnalyzeBlob(AnalyzeClass analyze, Bitmap bmpinput, PointF offsetpointf, List<Rectangle> rectlist, float threshold, int extend)
        {

            RectangleF myRectF = analyze.GetMyMoverRectF();
            Bitmap bmpInputOrg = bmpinput.Clone(myRectF, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            HistogramClass histogram = new HistogramClass(2);


        }

        #endregion


        List<AnalyzeClass> m_MarkTheSameList = new List<AnalyzeClass>();
        PointF ShiftPT = new PointF();
        public PointF ShiftPTORG = new PointF();
        PointF ShiftPT2 = new PointF();

        //public int Inflate_WH = 5;
        //public int ThresholdRatio = 0;

        class BlockItemClass
        {
            public int Index = 0;
            public string Name = String.Empty;
            public Rectangle myrect = new Rectangle();
            public int ReportIndex = 0;
        }

        JzFindBlockPropertyGridClass BlockPara = new JzFindBlockPropertyGridClass();

        public void SetBlockPara(JzFindBlockPropertyGridClass eBlockPara)
        {
            BlockPara = eBlockPara;
        }

        bool IsSaveSub = false;

        public RectangleF FindRectSub(Bitmap ebmpInput, RectangleF eRectF, RectangleF analyzeOrgLocation, int eSubPixel = 20)
        {
            #region 這裏原圖先blob一次 找到一堆框

            //IsSaveSub = true;

            Bitmap bmporglocation = ebmpInput.Clone(analyzeOrgLocation, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            HistogramClass histogram = new HistogramClass(2);
            JzFindObjectClass jzFind = new JzFindObjectClass();

            Bitmap bmpblockrun = AForge.Imaging.Image.Clone(bmporglocation, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            Erosion erosion = new Erosion();
            Bitmap bmpDilata = erosion.Apply(bmpblockrun);

            Subtract subtract = new Subtract(bmpDilata);
            Bitmap bmpSub = subtract.Apply(bmpblockrun);

            Grayscale grayscale = new Grayscale(0.299, 0.587, 0.114);
            Bitmap bmpgray = grayscale.Apply(bmpblockrun);

            OtsuThreshold otsuthreshold = new OtsuThreshold();
            Bitmap bmpotsuThreshold = otsuthreshold.Apply(bmpgray);

            //histogram.GetHistogram(bmpSub);

            //AForge.Imaging.Filters.Threshold AforThreshold = new AForge.Imaging.Filters.Threshold(BlockPara.threshlod);
            ////AForge.Imaging.Filters.Threshold AforThreshold = new AForge.Imaging.Filters.Threshold(histogram.MeanGrade);
            //Bitmap bmpThreshold = AforThreshold.Apply(bmpgray);

            //bmpThreshold.Save("D://LOA//Aforge//AforgeGray.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

            //jzFind.SetThreshold(bmpblockrun, JzTool.SimpleRect(bmpThreshold.Size), 188, 255, 0, true);
            jzFind.SetThreshold(bmpotsuThreshold, JzTool.SimpleRect(bmpotsuThreshold.Size), 188, 255, 0, true);

            string _path = "D:\\LOA\\Aforge";
            if (!System.IO.Directory.Exists(_path))
                System.IO.Directory.CreateDirectory(_path);

            if (IsSaveSub)
                bmpotsuThreshold.Save("D://LOA//Aforge//AforgeGrayThreshold.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

            jzFind.Find(bmpotsuThreshold, Color.Red);

            List<Rectangle> FoundRectList = new List<Rectangle>();
            List<BlockItemClass> FoundBlockList = new List<BlockItemClass>();
            FoundBlockList.Clear();
            FoundRectList.Clear();

            foreach (FoundClass found in jzFind.FoundList)
            {
                Rectangle rect = found.rect;
                rect.Inflate(BlockPara.extend, BlockPara.extend);
                rect.Intersect(JzTool.SimpleRect(bmpotsuThreshold.Size));

                if (found.Area > BlockPara.minarea)
                {

                    //FoundRectList.Add(rect);

                    BlockItemClass blockItem = new BlockItemClass();
                    blockItem.myrect = rect;
                    FoundBlockList.Add(blockItem);
                }

            }


            int i = 0;
            int j = 0;
            int k = 0;
            Rectangle rectk = new Rectangle(0, 0, 0, 0);

            #region 排序

            int Highest = 100000;
            int HighestIndex = -1;
            int ReportIndex = 0;
            List<string> CheckList = new List<string>();

            //Clear All Index To 0 and Check the Highest

            foreach (BlockItemClass keyassign in FoundBlockList)
            {
                keyassign.ReportIndex = 0;
                ReportIndex = 1;
            }

            i = 0;
            while (true)
            {
                i = 0;
                Highest = 100000;
                HighestIndex = -1;
                foreach (BlockItemClass keyassign in FoundBlockList)
                {
                    if (keyassign.ReportIndex == 0)
                    {
                        if (keyassign.myrect.X < Highest)
                        {
                            Highest = keyassign.myrect.X;
                            HighestIndex = i;
                        }
                    }

                    i++;
                }

                if (HighestIndex == -1)
                    break;

                CheckList.Clear();

                //把相同位置的人找出來
                i = 0;
                k = 0;
                foreach (BlockItemClass keyassign in FoundBlockList)
                {
                    if (keyassign.ReportIndex == 0)
                    {
                        if (JzTool.IsInRange(keyassign.myrect.X, Highest, 10))
                        {
                            CheckList.Add(keyassign.myrect.Y.ToString("0000") + "," + i.ToString());

                            //rectk = MergeTwoRects(rectk, keyassign.myrect);
                            //k++;
                            //if (k == BlockPara.inflant)
                            //{
                            //    FoundRectList.Add(rectk);
                            //    rectk = new Rectangle(0, 0, 0, 0);
                            //    k = 0;
                            //}

                        }
                    }
                    i++;
                }

                CheckList.Sort();

                foreach (string Str in CheckList)
                {
                    string[] Strs = Str.Split(',');
                    FoundBlockList[int.Parse(Strs[1])].ReportIndex = ReportIndex;
                    ReportIndex++;
                }
            }

            #endregion

            //foreach (BlockItemClass keyassign in FoundBlockList)
            //{
            //    DrawRect(bmpblockrun, keyassign.myrect, new Pen(Color.Lime, 1));
            //    JzTool.DrawText(bmpblockrun, keyassign.ReportIndex.ToString(), keyassign.myrect.Location, 5, Color.Red);
            //}
            //bmpblockrun.Save("D://LOA//Aforge//bmpblockrun.bmp", System.Drawing.Imaging.ImageFormat.Bmp);


            #region 相交的个数

            i = 0;

            foreach (BlockItemClass keyassign in FoundBlockList)
            {
                CheckList.Add(keyassign.ReportIndex.ToString("0000") + "," + i.ToString());
                i++;
            }
            CheckList.Sort();

            foreach (string Str in CheckList)
            {
                string[] Strs = Str.Split(',');
                FoundRectList.Add(FoundBlockList[int.Parse(Strs[1])].myrect);
            }



            #endregion

            //图像高度的几分之几
            double iheightRatio = BlockPara.inflant * 1.0 / 100 * bmpblockrun.Height;

            if (BlockPara.blockdir == BlockDir.VERTICAL)
            {
                #region 找纵向
                //#if FIND_H
                i = 0;
                j = 0;

                while (i < FoundRectList.Count - 1)
                {
                    if (FoundRectList[i].Width == 0)
                    {
                        i++;
                        continue;
                    }

                    j = i + 1;

                    while (j < FoundRectList.Count)
                    {
                        if (FoundRectList[j].Width == 0)
                        {
                            j++;
                            continue;
                        }

                        Rectangle recti = FoundRectList[i];
                        Rectangle rectj = FoundRectList[j];

                        if (JzTool.IsInRange(recti.X, rectj.X, 10))
                        //if (recti.IntersectsWith(rectj))
                        {
                            rectj = MergeTwoRects(recti, rectj);

                            if (rectj.Height < iheightRatio)
                            {
                                recti = new Rectangle(0, 0, 0, 0);

                                FoundRectList.RemoveAt(i);
                                FoundRectList.Insert(i, recti);

                                FoundRectList.RemoveAt(j);
                                FoundRectList.Insert(j, rectj);

                                break;
                            }
                        }

                        j++;
                    }
                    i++;
                }

                i = FoundRectList.Count - 1;

                while (i > -1)
                {
                    if (FoundRectList[i].Width == 0)
                        FoundRectList.RemoveAt(i);
                    else
                    {
                        Rectangle recti = FoundRectList[i];

                        FoundRectList.RemoveAt(i);
                        FoundRectList.Insert(i, recti);
                    }
                    i--;
                }

                //#endif

                #endregion
            }
            else
            {
                #region 找横向

                i = 0;
                j = 0;

                while (i < FoundRectList.Count - 1)
                {
                    if (FoundRectList[i].Height == 0)
                    {
                        i++;
                        continue;
                    }

                    j = i + 1;

                    while (j < FoundRectList.Count)
                    {
                        if (FoundRectList[j].Height == 0)
                        {
                            j++;
                            continue;
                        }

                        Rectangle recti = FoundRectList[i];
                        Rectangle rectj = FoundRectList[j];

                        if (JzTool.IsInRange(recti.Y, rectj.Y, 10))
                        //if (recti.IntersectsWith(rectj))
                        {
                            rectj = MergeTwoRects(recti, rectj);

                            if (rectj.Width < iheightRatio)
                            {
                                recti = new Rectangle(0, 0, 0, 0);

                                FoundRectList.RemoveAt(i);
                                FoundRectList.Insert(i, recti);

                                FoundRectList.RemoveAt(j);
                                FoundRectList.Insert(j, rectj);

                                break;
                            }
                        }

                        j++;
                    }
                    i++;
                }

                i = FoundRectList.Count - 1;

                while (i > -1)
                {
                    if (FoundRectList[i].Height == 0)
                        FoundRectList.RemoveAt(i);
                    else
                    {
                        Rectangle recti = FoundRectList[i];

                        FoundRectList.RemoveAt(i);
                        FoundRectList.Insert(i, recti);
                    }
                    i--;
                }

                #endregion
            }
            #region 这个不用了

            //i = 0;
            //j = 0;

            //while (i < FoundRectList.Count - 1)
            //{
            //    if (FoundRectList[i].Width == 0)
            //    {
            //        i++;
            //        continue;
            //    }

            //    j = i + 1;

            //    while (j < FoundRectList.Count)
            //    {
            //        if (FoundRectList[j].Width == 0)
            //        {
            //            j++;
            //            continue;
            //        }

            //        Rectangle recti = FoundRectList[i];
            //        Rectangle rectj = FoundRectList[j];

            //        if (JzTool.IsInRange(recti.X, rectj.X, 10))
            //        //if (recti.IntersectsWith(rectj))
            //        {
            //            rectj = MergeTwoRects(recti, rectj);

            //            if (rectj.Height < iheightRatio)
            //            {
            //                recti = new Rectangle(0, 0, 0, 0);

            //                FoundRectList.RemoveAt(i);
            //                FoundRectList.Insert(i, recti);

            //                FoundRectList.RemoveAt(j);
            //                FoundRectList.Insert(j, rectj);

            //                break;
            //            }
            //        }

            //        j++;
            //    }
            //    i++;
            //}

            //i = FoundRectList.Count - 1;

            //while (i > -1)
            //{
            //    if (FoundRectList[i].Width == 0)
            //        FoundRectList.RemoveAt(i);
            //    else
            //    {
            //        Rectangle recti = FoundRectList[i];

            //        FoundRectList.RemoveAt(i);
            //        FoundRectList.Insert(i, recti);
            //    }
            //    i--;
            //}

            #endregion


            List<Rectangle> foundRectCheck = new List<Rectangle>();
            Bitmap bitmapDraw = new Bitmap(bmpotsuThreshold);

            foreach (Rectangle rect in FoundRectList)
            {
                rect.Intersect(JzTool.SimpleRect(bitmapDraw.Size));

                JzTool.DrawRect(bitmapDraw, rect, new Pen(Color.Lime, 2));

                Rectangle myGoodRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                myGoodRect.X += (int)analyzeOrgLocation.X;
                myGoodRect.Y += (int)analyzeOrgLocation.Y;


                foundRectCheck.Add(myGoodRect);
            }

            RectangleF _rect_draw_org = new RectangleF(eRectF.X - analyzeOrgLocation.X, eRectF.Y - analyzeOrgLocation.Y, eRectF.Width, eRectF.Height);
            _rect_draw_org.Inflate(15, 15);
            JzTool.DrawRect(bitmapDraw, _rect_draw_org, new Pen(Color.Pink, 2));

            if (IsSaveSub)
                bitmapDraw.Save("D://LOA//Aforge//AforgeGrayThreshold_Rect.bmp", System.Drawing.Imaging.ImageFormat.Bmp);


            #endregion
            int iInflate = 10;
            #region

            //這裏找到 一個合適的框
            RectangleF rectORGx = new RectangleF(eRectF.X, eRectF.Y, eRectF.Width, eRectF.Height);
            rectORGx.Inflate(15, 15);

            bool IsIncluded = false;
            i = 0;
            foreach (Rectangle rect in foundRectCheck)
            {
                RectangleF rectfintersect = rectORGx;
                rectfintersect.Intersect(rect);

                if ((float)(rectfintersect.Width * rectfintersect.Height) / (float)(rect.Width * rect.Height) >= 0.99)
                {
                    IsIncluded = true;
                    break;
                }
                i++;
            }


            #endregion

            RectangleF rectORG = new RectangleF(eRectF.X, eRectF.Y, eRectF.Width, eRectF.Height);
            PointF pointFshift = new PointF(0, 0);
            if (IsIncluded)
            {
                rectORG = new RectangleF(foundRectCheck[i].X, foundRectCheck[i].Y, foundRectCheck[i].Width, foundRectCheck[i].Height);
                rectORG.Intersect(rectORGx);

                //pointFshift = new PointF(foundRectCheck[i].X - eRectF.X, foundRectCheck[i].Y - eRectF.Y);


                //rectORG = new RectangleF(eRectF.X, eRectF.Y, eRectF.Width, eRectF.Height);
            }

            rectORG.Inflate(10, 10);

            double _filterArea = 10;//过滤面积
            double _filterPixel = 2;//过滤边缘的像素

            int iCount = (int)rectORG.Height / eSubPixel;
            int iSubPixel = (int)rectORG.Height % eSubPixel;
            bmpSub = new Bitmap(1, 1);

            if (BlockPara.blockdir == BlockDir.HORIZONTAL)
            {
                iCount = (int)rectORG.Width / eSubPixel;
                iSubPixel = (int)rectORG.Width % eSubPixel;
            }

            _path = "D:\\LOA\\SUB";
            if (!System.IO.Directory.Exists(_path))
                System.IO.Directory.CreateDirectory(_path);

            Rectangle MaxRect = new Rectangle((int)eRectF.X, (int)eRectF.Y, (int)eRectF.Width, (int)eRectF.Height);
            RectangleF rectCorp = new RectangleF();
            RectangleF RectF = new RectangleF();
            RectangleF RectFDraw = new RectangleF();
            i = 0;
            int icheck = 0;
            while (i < iCount)
            {

                //rectCorp = new RectangleF(rectORG.X, rectORG.Y + i * eSubPixel, rectORG.Width, eSubPixel);
                //rectCorp.Intersect(rectORG);

                if (BlockPara.blockdir == BlockDir.HORIZONTAL)
                {
                    rectCorp = new RectangleF(rectORG.X + i * eSubPixel, rectORG.Y, eSubPixel, rectORG.Height);
                    rectCorp.Intersect(rectORG);
                }
                else
                {
                    rectCorp = new RectangleF(rectORG.X, rectORG.Y + i * eSubPixel, rectORG.Width, eSubPixel);
                    rectCorp.Intersect(rectORG);
                }

                bmpSub.Dispose();
                bmpSub = (Bitmap)ebmpInput.Clone(rectCorp, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                m_Histogram.GetHistogram(bmpSub);
                m_Find.SetThreshold(bmpSub, JzTool.SimpleRect(bmpSub.Size), m_Histogram.MeanGrade + 10, 255, 0, true);
                m_Find.Find(bmpSub, Color.Red);

                if (i == 0)
                    icheck = 1;
                else if (i == iCount - 1)
                    icheck = 2;
                else
                    icheck = 0;

                if (BlockPara.blockdir == BlockDir.HORIZONTAL)
                {
                    MaxRect = m_Find.GetRectExpectAround_H(JzTool.SimpleRect(bmpSub.Size), _filterArea, _filterPixel, icheck);
                }
                else
                {
                    MaxRect = m_Find.GetRectExpectAround(JzTool.SimpleRect(bmpSub.Size), _filterArea, _filterPixel, icheck);
                }


                JzTool.DrawRectEx(bmpSub, MaxRect, new Pen(Color.Lime, 2));
                if (IsSaveSub)
                    bmpSub.Save(_path + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff_") + i.ToString() + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                //bmpSub.Save(_path + "\\"  + i.ToString() + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);

                RectangleF RectFX = new RectangleF(MaxRect.X + rectCorp.X, MaxRect.Y + rectCorp.Y, MaxRect.Width, MaxRect.Height);
                //RectFDraw = MergeTwoRects(RectFDraw, RectFX);

                //RectFX = new RectangleF(MaxRect.X, MaxRect.Y + i * eSubPixel, MaxRect.Width, MaxRect.Height);

                if (BlockPara.blockdir == BlockDir.HORIZONTAL)
                {
                    RectFX = new RectangleF(MaxRect.X + i * eSubPixel, MaxRect.Y, MaxRect.Width, MaxRect.Height);
                }
                else
                {
                    RectFX = new RectangleF(MaxRect.X, MaxRect.Y + i * eSubPixel, MaxRect.Width, MaxRect.Height);
                }

                RectF = MergeTwoRects(RectF, RectFX);
                //break;

                i++;
            }

            RectF.X += pointFshift.X - eRectF.X + rectORG.X;
            RectF.Y += pointFshift.Y - eRectF.Y + rectORG.Y;

            //RectF.X += rectORG.X;
            //RectF.Y += rectORG.Y;

            //RectangleF draw = new RectangleF(RectF.Y + rectCorp.X, RectF.Y + rectCorp.X, RectF.Width, RectF.Height);

            //Bitmap bmpDes = new Bitmap(ebmpInput);
            //JzTool.DrawRect(bmpDes, rectORG, new Pen(Color.Lime, 2));
            //bmpDes.Save(_path + "\\" + "des" + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);



            return RectF;
        }
        public RectangleF FindRectSub2(Bitmap ebmpInput, RectangleF eRectF, int eSubPixel = 20)
        {

            int iInflate = 5;
            RectangleF rectORG = new RectangleF(eRectF.X, eRectF.Y, eRectF.Width, eRectF.Height);
            rectORG.Inflate(iInflate, 10);

            Bitmap bmpSubOrg = ebmpInput.Clone(rectORG, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            string _path = "D:\\LOA\\SUB";
            if (!System.IO.Directory.Exists(_path))
                System.IO.Directory.CreateDirectory(_path);
            JzFindObjectClass jzFind = new JzFindObjectClass();


            Bitmap bmpblockrun = AForge.Imaging.Image.Clone(bmpSubOrg, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            Erosion erosion = new Erosion();
            Bitmap bmpDilata = erosion.Apply(bmpblockrun);

            Subtract subtract = new Subtract(bmpDilata);
            Bitmap bmpSub = subtract.Apply(bmpblockrun);

            Grayscale grayscale = new Grayscale(0.299, 0.587, 0.114);
            Bitmap bmpgray = grayscale.Apply(bmpSub);

            m_Histogram.GetHistogram(bmpSub);
            //m_Find.SetThreshold(bmpSub, JzTool.SimpleRect(bmpSub.Size), m_Histogram.MeanGrade + 10, 255, 0, true);

            AForge.Imaging.Filters.Threshold AforThreshold = new AForge.Imaging.Filters.Threshold(m_Histogram.MeanGrade);
            Bitmap bmpThreshold = AforThreshold.Apply(bmpgray);

            bmpThreshold.Save(_path + "//AforgeGraySub.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

            jzFind.SetThreshold(bmpThreshold, JzTool.SimpleRect(bmpThreshold.Size), 188, 255, 0, true);
            jzFind.Find(bmpThreshold, Color.Red);

            RectangleF RectF = new RectangleF();
            Rectangle MaxRect = new Rectangle((int)eRectF.X, (int)eRectF.Y, (int)eRectF.Width, (int)eRectF.Height);

            MaxRect = jzFind.GetRectExpectAround(JzTool.SimpleRect(bmpblockrun.Size), 10, iInflate / 2, 0);
            //JzTool.DrawRect(bmpblockrun, MaxRect, new Pen(Color.Lime, 2));
            RectF = new RectangleF(MaxRect.X, MaxRect.Y, MaxRect.Width, MaxRect.Height);
            //RectF = new RectangleF(MaxRect.X, MaxRect.Y, MaxRect.Width, MaxRect.Height);

            foreach (FoundClass found in jzFind.FoundList)
            {
                Rectangle rect = found.rect;
                rect.Intersect(JzTool.SimpleRect(bmpblockrun.Size));
                JzTool.DrawRect(bmpblockrun, rect, new Pen(Color.Lime, 2));

                //if (found.Area > 50)
                //{
                //    JzTool.DrawRect(bmpblockrun, rect, new Pen(Color.Lime, 2));



                //    RectangleF RectFX = new RectangleF(rect.X + rectORG.X, rect.Y + rectORG.Y, rect.Width, rect.Height);
                //    RectF = MergeTwoRects(RectF, RectFX);
                //}

            }

            bmpblockrun.Save(_path + "\\" + "des" + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);

            //Bitmap bmpDes = new Bitmap(ebmpInput);
            //JzTool.DrawRect(bmpDes, RectFDraw, new Pen(Color.Lime, 2));
            //bmpDes.Save(_path + "\\" + "des" + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);

            return RectF;
        }

        public RectangleF FindRectSub3(Bitmap ebmpInput, RectangleF eRectF, int eSubPixel = 20)
        {
            int iInflate = 10;
            RectangleF rectORG = new RectangleF(eRectF.X, eRectF.Y, eRectF.Width, eRectF.Height);
            rectORG.Inflate(iInflate, 10);

            double _filterArea = 10;//过滤面积
            double _filterPixel = 2;//过滤边缘的像素

            int iCount = (int)rectORG.Height / eSubPixel;
            int iSubPixel = (int)rectORG.Height % eSubPixel;
            Bitmap bmpSub = new Bitmap(1, 1);

            string _path = "D:\\LOA\\SUB";
            if (!System.IO.Directory.Exists(_path))
                System.IO.Directory.CreateDirectory(_path);

            Rectangle MaxRect = new Rectangle((int)eRectF.X, (int)eRectF.Y, (int)eRectF.Width, (int)eRectF.Height);
            RectangleF rectCorp = new RectangleF();
            RectangleF RectF = new RectangleF();
            RectangleF RectFDraw = new RectangleF();
            int i = 0;
            int icheck = 0;
            while (i < iCount)
            {

                rectCorp = new RectangleF(rectORG.X, rectORG.Y + i * eSubPixel, rectORG.Width, eSubPixel);
                rectCorp.Intersect(rectORG);

                bmpSub.Dispose();
                bmpSub = (Bitmap)ebmpInput.Clone(rectCorp, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                m_Histogram.GetHistogram(bmpSub);
                m_Find.SetThreshold(bmpSub, JzTool.SimpleRect(bmpSub.Size), m_Histogram.MeanGrade + 10, 255, 0, true);
                m_Find.Find(bmpSub, Color.Red);

                if (i == 0)
                    icheck = 1;
                else if (i == iCount - 1)
                    icheck = 2;
                else
                    icheck = 0;



                MaxRect = m_Find.GetRectExpectAround(JzTool.SimpleRect(bmpSub.Size), _filterArea, _filterPixel, icheck);
                JzTool.DrawRectEx(bmpSub, MaxRect, new Pen(Color.Lime, 2));
                bmpSub.Save(_path + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff_") + i.ToString() + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                //bmpSub.Save(_path + "\\"  + i.ToString() + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);

                RectangleF RectFX = new RectangleF(MaxRect.X + rectCorp.X, MaxRect.Y + rectCorp.Y, MaxRect.Width, MaxRect.Height);
                //RectFDraw = MergeTwoRects(RectFDraw, RectFX);

                RectFX = new RectangleF(MaxRect.X, MaxRect.Y + i * eSubPixel, MaxRect.Width, MaxRect.Height);
                RectF = MergeTwoRects(RectF, RectFX);

                i++;
            }

            //RectF.X += rectORG.X;
            //RectF.Y += rectORG.Y;

            //RectangleF draw = new RectangleF(RectF.Y + rectCorp.X, RectF.Y + rectCorp.X, RectF.Width, RectF.Height);

            //Bitmap bmpDes = new Bitmap(ebmpInput);
            //JzTool.DrawRect(bmpDes, rectORG, new Pen(Color.Lime, 2));
            //bmpDes.Save(_path + "\\" + "des" + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);

            return RectF;
        }

        public void FindMarkSame()
        {
            int i = 0;

            //List<DoffsetClass> DoffsetList = new List<DoffsetClass>();

            //Get the Anallyze Data For Find
            Bitmap bmpPageOrg = PageNow.GetbmpORG((PageOPTypeEnum)PageNow.PageOPTypeIndex);

            int selectno = AnalyzeSelectNow.No;
            RectangleF myRectF = AnalyzeSelectNow.GetMyMoverRectF();
            List<RectangleF> OrgRectFList = new List<RectangleF>();

            m_MarkTheSameList.Clear();
            AnalyzeSelectNow.FillToListRemoveMe(m_MarkTheSameList);

            if (m_MarkTheSameList.Count == 0)
                return;
            //RectangleF toshaperectfXXX = m_MarkTheSameList[0].GetMyMoverRectF();
            int count = AnalyzeList.Count;

            ATREEUI.DismissAnalyzeTable();

            //同位前 需要删除 除本身和包含的框 外 其他都清除

            i = 0;
            List<int> delnolist = new List<int>();
            while (i < count)
            {
                AnalyzeClass analyze = AnalyzeList[i];
                if (analyze.No == AnalyzeSelectNow.No)
                {
                    i++;
                    continue;
                }

                bool bfound = false;
                foreach (AnalyzeClass analyze1 in m_MarkTheSameList)
                {
                    if (analyze.No == analyze1.No)
                    {
                        bfound = true;
                        break;
                    }
                }

                if (bfound)
                {
                    i++;
                    continue;
                }

                if (analyze.Level == 3)
                    delnolist.Add(analyze.No);

                i++;
            }

            if (delnolist.Count > 0)
            {
                ATREEUI.CollapseTree();
                ATREEUI.Delete(delnolist);
            }


            //Bitmap bmpDraw = new Bitmap(bmpPageOrg);
            ////找blob 中心
            Bitmap bmpAnalyzeSelectNow = new Bitmap(1, 1);

            //PointF ptfORG = new PointF(myRectF.X, myRectF.Y);
            //原始的顶端中心位置PointF
            foreach (AnalyzeClass analyze1 in m_MarkTheSameList)
            {
                RectangleF toshaperectfORG = analyze1.GetMyMoverRectF();
                RectangleF rectGood = FindRectSub(bmpPageOrg, toshaperectfORG, myRectF, 40);
                ShiftPTORG = new PointF(rectGood.X, rectGood.Y);

                break;
            }

            //return;


            i = 0;
            count = AnalyzeList.Count;
            while (i < count)
            //foreach(AnalyzeClass analyze in AnalyzeList)
            {
                AnalyzeClass analyze = AnalyzeList[i];
                if (analyze.No == AnalyzeSelectNow.No)
                {
                    i++;
                    continue;
                }


                RectangleF torectf = analyze.GetMyMoverRectF();
                List<RectangleF> foundrectlist = new List<RectangleF>();
                List<int> foundlevellist = new List<int>();

                if (torectf != myRectF)
                {
                    if (RectIsTheSame(RectFToRect(torectf), RectFToRect(myRectF), 20))
                    {
                        int j = 0;
                        ShiftPT2 = new PointF(0, 0);
                        foreach (AnalyzeClass analyze1 in m_MarkTheSameList)
                        {
                            ShiftPT = new PointF((torectf.X - myRectF.X), (torectf.Y - myRectF.Y));
                            RectangleF toshaperectf = analyze1.GetMyMoverRectF();
                            toshaperectf.Offset(ShiftPT);

                            if (j == 0)
                            {
                                RectangleF toshaperectfORG = analyze1.GetMyMoverRectF();
                                toshaperectfORG.Offset(ShiftPT);
                                RectangleF rectGood = FindRectSub(bmpPageOrg, toshaperectfORG, torectf, 40);
                                ShiftPT2 = new PointF((rectGood.X - ShiftPTORG.X), (rectGood.Y - ShiftPTORG.Y));
                            }

                            toshaperectf.Offset(ShiftPT2);
                            toshaperectf.Intersect(torectf);
                            foundrectlist.Add(toshaperectf);

                            foundlevellist.Add(analyze1.Level);

                            j++;

                        }
                    }
                }


                //if (analyze.IsSelected && analyze.No != 1)
                {
                    //List<Rectangle> foundrectlist = new List<Rectangle>();

                    //analyze.IsTempSave = true;

                    //analyze.B02_CreateFindInsideRequirement(bmpPageOrg, new PointF(0, 0), foundrectlist, threshold, extend);

                    //analyze.IsTempSave = false;

                    if (foundrectlist.Count > 0)
                    {
                        int k = 0;
                        analyze.BranchList.Clear();
                        foreach (RectangleF foundrect in foundrectlist)
                        {
                            ATREEUI.AddBranchLevel(analyze.No, foundrect, 0, 0, m_MarkTheSameList[k]);
                            k++;
                        }
                    }
                }
                i++;
            }

            ATREEUI.RelateAnalyzeTable();
        }
        /// <summary>
        /// 固定偏移位置来找相对位置
        /// </summary>
        public void FindMarkSame2()
        {
            int i = 0;

            ATREEUI.DismissAnalyzeTable();

            //List<DoffsetClass> DoffsetList = new List<DoffsetClass>();

            //Get the Anallyze Data For Find
            Bitmap bmpPageOrg = PageNow.GetbmpORG((PageOPTypeEnum)PageNow.PageOPTypeIndex);

            int selectno = AnalyzeSelectNow.No;
            RectangleF myRectF = AnalyzeSelectNow.GetMyMoverRectF();
            List<RectangleF> OrgRectFList = new List<RectangleF>();

            m_MarkTheSameList.Clear();
            AnalyzeSelectNow.FillToListRemoveMe(m_MarkTheSameList);

            if (m_MarkTheSameList.Count == 0)
                return;
            //RectangleF toshaperectfXXX = m_MarkTheSameList[0].GetMyMoverRectF();
            int count = AnalyzeList.Count;


            //同位前 需要删除 除本身和包含的框 外 其他都清除

            i = 0;
            List<int> delnolist = new List<int>();
            while (i < count)
            {
                AnalyzeClass analyze = AnalyzeList[i];
                if (analyze.No == AnalyzeSelectNow.No)
                {
                    i++;
                    continue;
                }

                bool bfound = false;
                foreach (AnalyzeClass analyze1 in m_MarkTheSameList)
                {
                    if (analyze.No == analyze1.No)
                    {
                        bfound = true;
                        break;
                    }
                }

                if (bfound)
                {
                    i++;
                    continue;
                }

                if (analyze.Level == 3)
                    delnolist.Add(analyze.No);

                i++;
            }

            if (delnolist.Count > 0)
            {
                ATREEUI.CollapseTree();
                ATREEUI.Delete(delnolist);
            }

            //Bitmap bmpDraw = new Bitmap(bmpPageOrg);
            ////找blob 中心
            Bitmap bmpAnalyzeSelectNow = new Bitmap(1, 1);

            //PointF ptfORG = new PointF(myRectF.X, myRectF.Y);
            //原始的顶端中心位置PointF
            foreach (AnalyzeClass analyze1 in m_MarkTheSameList)
            {
                RectangleF toshaperectfORG = analyze1.GetMyMoverRectF();
                //RectangleF rectGood = FindRectSub(bmpPageOrg, toshaperectfORG, myRectF, 40);
                ShiftPTORG = new PointF(toshaperectfORG.X - myRectF.X, toshaperectfORG.Y - myRectF.Y);

                break;
            }

            //return;


            i = 0;
            count = AnalyzeList.Count;
            while (i < count)
            //foreach(AnalyzeClass analyze in AnalyzeList)
            {
                AnalyzeClass analyze = AnalyzeList[i];
                if (analyze.No == AnalyzeSelectNow.No)
                {
                    i++;
                    continue;
                }


                RectangleF torectf = analyze.GetMyMoverRectF();
                List<RectangleF> foundrectlist = new List<RectangleF>();
                List<int> foundlevellist = new List<int>();

                if (torectf != myRectF)
                {
                    if (RectIsTheSame(RectFToRect(torectf), RectFToRect(myRectF), 20))
                    {
                        int j = 0;
                        ShiftPT2 = new PointF(0, 0);
                        foreach (AnalyzeClass analyze1 in m_MarkTheSameList)
                        {
                            ShiftPT = new PointF((torectf.X - myRectF.X), (torectf.Y - myRectF.Y));
                            RectangleF toshaperectf = analyze1.GetMyMoverRectF();
                            toshaperectf.Offset(ShiftPT);

                            if (j == 0)
                            {
                                RectangleF toshaperectfORG = analyze1.GetMyMoverRectF();
                                toshaperectfORG.Offset(ShiftPT);
                                //RectangleF rectGood = FindRectSub(bmpPageOrg, toshaperectfORG, torectf, 40);
                                ShiftPT2 = new PointF((toshaperectfORG.X - torectf.X - ShiftPTORG.X), (toshaperectfORG.Y - torectf.Y - ShiftPTORG.Y));
                            }

                            toshaperectf.Offset(ShiftPT2);
                            toshaperectf.Intersect(torectf);
                            foundrectlist.Add(toshaperectf);

                            foundlevellist.Add(analyze1.Level);

                            j++;

                        }
                    }
                }


                //if (analyze.IsSelected && analyze.No != 1)
                {
                    //List<Rectangle> foundrectlist = new List<Rectangle>();

                    //analyze.IsTempSave = true;

                    //analyze.B02_CreateFindInsideRequirement(bmpPageOrg, new PointF(0, 0), foundrectlist, threshold, extend);

                    //analyze.IsTempSave = false;

                    if (foundrectlist.Count > 0)
                    {
                        int k = 0;
                        analyze.BranchList.Clear();
                        foreach (RectangleF foundrect in foundrectlist)
                        {
                            ATREEUI.AddBranchLevel(analyze.No, foundrect, 0, 0, m_MarkTheSameList[k]);
                            k++;
                        }
                    }
                }
                i++;
            }

            ATREEUI.RelateAnalyzeTable();

        }
        /// <summary>
        /// 同位資料參數  就是選中某一個框 然後將此框的參數同步到除此以外的 同等級的框上面
        /// </summary>
        public void FindMarkSamePara()
        {
            int i = 0;

            //AnalyzeClass analyzeSelectNow = AnalyzeSelectNow;
            int count = AnalyzeList.Count;
            while (i < count)
            {
                AnalyzeClass analyze = AnalyzeList[i];
                if (analyze.No != AnalyzeSelectNow.No)
                {
                    //同等級的參數 同位
                    if (analyze.Level == AnalyzeSelectNow.Level)
                    {
                        analyze.ExtendX = AnalyzeSelectNow.ExtendX;
                        analyze.ExtendY = AnalyzeSelectNow.ExtendY;

                        //analyze.NORMALPara.FromString(AnalyzeSelectNow.NORMALPara.ToString());
                        analyze.ALIGNPara.FromString(AnalyzeSelectNow.ALIGNPara.ToString());
                        analyze.MEASUREPara.FromString(AnalyzeSelectNow.MEASUREPara.ToString());
                        analyze.AOIPara.FromString(AnalyzeSelectNow.AOIPara.ToString());
                        if (analyze.OCRPara.OCRMethod == OCRMethodEnum.DATAMATRIX
                                 || analyze.OCRPara.OCRMethod == OCRMethodEnum.DATAMATRIXGRADE
                                 || analyze.OCRPara.OCRMethod == OCRMethodEnum.QRCODE)
                        {
                        }
                        else
                        {
                            analyze.INSPECTIONPara.FromString(AnalyzeSelectNow.INSPECTIONPara.ToString());
                            analyze.OCRPara.FromString(AnalyzeSelectNow.OCRPara.ToString());
                        }
                        analyze.PADPara.FromString(AnalyzeSelectNow.PADPara.ToString());
                        //if (AnalyzeSelectNow.Level == 2)

                    }
                }
                i++;
            }

            //ConverAnalyzeToList(AnalyzeRootNow);
            //ATREEUI.SetAnalyzeTree(AnalyzeList);
        }
        /// <summary>
        /// 同位相同位置的框的参数
        /// </summary>
        public void FindMarkSameParaPos()
        {
            int i = 0;
            int count = AnalyzeList.Count;
            while (i < count)
            {
                AnalyzeClass analyze = AnalyzeList[i];
                if (analyze.No != AnalyzeSelectNow.No)
                {

                    //if (analyze.AliasName == "A00-03-0086")
                    //    analyze.AliasName = analyze.AliasName;

                    //同等級的參數 同位
                    if (analyze.Level == AnalyzeSelectNow.Level)
                    {
                        RectangleF r1 = AnalyzeSelectNow.myOPRectF;
                        r1.Inflate(-AnalyzeSelectNow.ExtendX, -AnalyzeSelectNow.ExtendY);
                        RectangleF r2 = analyze.myOPRectF;
                        r2.Inflate(-analyze.ExtendX, -analyze.ExtendY);
                        if (r1.IntersectsWith(r2))
                        {
                            bool bOK = true;

                            analyze.ExtendX = AnalyzeSelectNow.ExtendX;
                            analyze.ExtendY = AnalyzeSelectNow.ExtendY;
                            analyze.IsSeed = AnalyzeSelectNow.IsSeed;

                            //analyze.NORMALPara.FromString(AnalyzeSelectNow.NORMALPara.ToString());
                            analyze.ALIGNPara.FromString(AnalyzeSelectNow.ALIGNPara.ToString());
                            analyze.MEASUREPara.FromString(AnalyzeSelectNow.MEASUREPara.ToString());
                            analyze.AOIPara.FromString(AnalyzeSelectNow.AOIPara.ToString());
                            analyze.INSPECTIONPara.FromString(AnalyzeSelectNow.INSPECTIONPara.ToString());
                            analyze.OCRPara.FromString(AnalyzeSelectNow.OCRPara.ToString());
                            analyze.PADPara.FromString(AnalyzeSelectNow.PADPara.ToString());

                        }


                    }
                }
                i++;
            }

        }

        Rectangle MergeTwoRects(Rectangle rect1, Rectangle rect2)
        {
            Rectangle rect = new Rectangle();

            if (rect1.Width == 0)
                return rect2;
            if (rect2.Width == 0)
                return rect1;

            rect.X = Math.Min(rect1.X, rect2.X);
            rect.Y = Math.Min(rect1.Y, rect2.Y);

            rect.Width = Math.Max(rect1.X + rect1.Width, rect2.X + rect2.Width) - rect.X;
            rect.Height = Math.Max(rect1.Y + rect1.Height, rect2.Y + rect2.Height) - rect.Y;

            return rect;
        }
        RectangleF MergeTwoRects(RectangleF rect1, RectangleF rect2)
        {
            RectangleF rect = new RectangleF();

            if (rect1.Width == 0)
                return rect2;
            if (rect2.Width == 0)
                return rect1;

            rect.X = Math.Min(rect1.X, rect2.X);
            rect.Y = Math.Min(rect1.Y, rect2.Y);

            rect.Width = Math.Max(rect1.X + rect1.Width, rect2.X + rect2.Width) - rect.X;
            rect.Height = Math.Max(rect1.Y + rect1.Height, rect2.Y + rect2.Height) - rect.Y;

            return rect;
        }

        public bool RectIsTheSame(Rectangle OrgRect, Rectangle ComRect, int Percent)
        {
            bool ret = true;
            double UB = (100 + (double)Percent) / 100;
            double LB = (100 - (double)Percent) / 100;

            ret = ret & (((int)((double)OrgRect.Width * UB)) >= ComRect.Width && ((int)((double)OrgRect.Width * LB)) <= ComRect.Width);
            ret = ret & (((int)((double)OrgRect.Height * UB)) >= ComRect.Height && ((int)((double)OrgRect.Height * LB)) <= ComRect.Height);
            ret = ret & (((int)((double)GetRectArea(OrgRect) * UB)) >= GetRectArea(ComRect) && ((int)((double)GetRectArea(OrgRect) * LB)) <= GetRectArea(ComRect));

            return ret;
        }
        public int GetRectArea(Rectangle Rect)
        {
            return Rect.Width * Rect.Height;
        }

        public Rectangle RectFToRect(RectangleF RectF)
        {
            Rectangle rect = new Rectangle((int)RectF.X, (int)RectF.Y, (int)RectF.Width, (int)RectF.Height);

            return rect;
        }

        RectangleF OffsetRect(RectangleF orgrectf, PointF centerptf)
        {
            RectangleF rectf = orgrectf;

            rectf.Location = centerptf;

            rectf.X -= rectf.Width / 2;
            rectf.Y -= rectf.Height / 2;

            return rectf;
        }
        public void Suicide()
        {
            SUBUI.Suicide();
            DISPUI.Suicide();
        }

        #endregion


        public delegate void CaptureTriggerHandler(RectangleF captureRectF);
        public event CaptureTriggerHandler CaptureTriggerAction;
        public void OnCaptureTrigger(RectangleF captureRectF)
        {
            if (CaptureTriggerAction != null)
            {
                CaptureTriggerAction(captureRectF);
            }
        }

    }
}
