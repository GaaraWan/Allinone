using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

using System.Windows.Forms;

using JetEazy.BasicSpace;
using JetEazy.UISpace;
using JetEazy.OPSpace;

namespace JetEazy.ControlSpace
{
    public class OPDisplayCell : OPDisplayNormal
    {
        Pen PSelect = new Pen(Color.Yellow, 1);
        Pen PBorder = new Pen(Color.Black, 3);

        //List<ASSIGNClass> ASSIGNList;
        //List<BASISClass> BASISList;
        //List<ENHANCEClass> ENHANCEList;

        List<CellClass> CELLList = new List<CellClass>();

        Rectangle rectCellSelect = new Rectangle();

        Size szCellMove = new Size();
        Size szCellResize = new Size();

        OPTypeEnum OPType = OPTypeEnum.REG;
        Pen PKeyBorder = new Pen(Color.Yellow, 1);

        int FirstCellIndex = -1;
        int CellSelectCount = 0;
        bool IsMultiSelect = false;
        string SelectionStr = "";

        public bool IsDrawLine = false;

        public int XBias = 0;
        public int YBias = 0;

        bool IsLeftMouseDown = false;

        public OPDisplayCell(DispUI dispui, int defaultratio, int minratio, int maxratio)
            : base(dispui, defaultratio, minratio, maxratio)
        {


        }


        List<CellClass> BackupCellList = new List<CellClass>();

        public void BackupCell()
        {
            BackupCellList.Clear();

            foreach (CellClass cell in CELLList)
            {
                CellClass newcell = cell.Clone();
                BackupCellList.Add(newcell);
            }
        }
        public void RestoreCell()
        {
            CELLList.Clear();

            foreach (CellClass cell in BackupCellList)
            {
                CELLList.Add(cell.Clone());
            }
        }
        public void SetCellList(List<CellClass> celllist)
        {
            OPType = OPTypeEnum.BAS;

            PKeyBorder.DashStyle = DashStyle.Dash;

            FirstCellIndex = -1;

            CELLList.Clear();

            foreach (CellClass cell in celllist)
            {
                CELLList.Add(cell);
            }
        }

        /* Set Assign and Other Things 
        public void SetASSIGNList(List<ASSIGNClass> assignlist)
        {
            OPType = OPTypeEnum.ASN;

            FirstCellIndex = -1;

            ASSIGNList = assignlist;
            CELLList.Clear();

            foreach (ASSIGNClass assign in ASSIGNList)
            {
                CELLList.Add(assign.Cell);
            }
        }
        public void SetBASISList(List<BASISClass> basislist)
        {
            OPType = OPTypeEnum.BAS;

            PKeyBorder.DashStyle = DashStyle.Dash;

            FirstCellIndex = -1;

            BASISList = basislist;
            CELLList.Clear();

            foreach (BASISClass basis in BASISList)
            {
                CELLList.Add(basis.Cell);
            }
        }
        public void SetENHANCEList(List<ENHANCEClass> enhancelist, List<BASISClass> basislist)
        {
            OPType = OPTypeEnum.EHS;

            PKeyBorder.DashStyle = DashStyle.Dash;

            FirstCellIndex = -1;

            ENHANCEList = enhancelist;
            BASISList = basislist;
            CELLList.Clear();

            foreach (ENHANCEClass enhance in ENHANCEList)
            {
                CELLList.Add(enhance.Cell);
            }
        }
        */

        protected override void picOperation_MouseDown(object sender, MouseEventArgs e)
        {
            base.picOperation_MouseDown(sender, e);

            if (e.Button == MouseButtons.Left)
            {
                IsLeftMouseDown = true;
                MouseDown();
            }

        }
        protected override void picOperation_MouseUp(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                IsLeftMouseDown = false;
                MultiSelectionSet();

                switch (DISPOPType)
                {
                    case DisplayOPTypeEnum.SELECT:
                        OnSelect();
                        break;
                    case DisplayOPTypeEnum.RESIZE:
                    case DisplayOPTypeEnum.MOVE:
                        OnChange();
                        break;
                }
            }

            base.picOperation_MouseUp(sender, e);



            Refresh();
        }
        protected override void picOperation_MouseMove(object sender, MouseEventArgs e)
        {
            base.picOperation_MouseMove(sender, e);

            if (tmMovingDelay.msDuriation > MovingDelay)
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        MouseMove();
                        break;
                    default:
                        MouseHover();
                        break;

                }
                tmMovingDelay.Cut();
            }
        }

        protected override void DrawViewPort()
        {
            DISPUI.DrawStart();

            DrawCell();

            //switch (OPType)
            //{
            //    case OPTypeEnum.BAS:
            //        DrawLines();
            //        break;
            //    case OPTypeEnum.EHS:
            //        if (IsDrawLine)
            //        {
            //            switch (Universal.VER)
            //            {
            //                case "R6":
            //                    DrawBasisCell();
            //                    break;
            //                default:
            //                    DrawLines();
            //                    break;
            //            }
            //        }
            //        break;
            //}

            switch (DISPOPType)
            {
                case DisplayOPTypeEnum.SELECT:

                    Rectangle rect = JzTools.RectTwoPoint(PtStart, PtLive, szViewPort);

                    DISPUI.DrawRect(rect, PBorder);
                    DISPUI.DrawRect(rect, PSelect);

                    break;
            }

            if (READYOPType != DisplayOPTypeEnum.NONE)
                DISPUI.DrawText(READYOPType.ToString(), new Point(0, 0), 12);

            //if (Universal.IsDebug)
            //{
            //    DISPUI.DrawText("Count:" + CellSelectCount.ToString(), new Point(0, 20), 12);
            //    DISPUI.DrawText("Selection:" + SelectionStr, new Point(0, 40), 12);
            //}



            DISPUI.DrawEnd();
        }

        #region Cell Operation

        public void AddCellList(List<CellClass> celllist,CellPropertyEnum cellproperty , string name)
        {
            foreach (CellClass cell in celllist)
            {
                cell.CellProperty = cellproperty;
                cell.CellGroup = name;
                cell.PNormal = new Pen(Color.Yellow, 2);
                cell.PNormal.DashStyle = DashStyle.Dot;


                CELLList.Add(cell);
            }
        }
        public void RemoveCellList(string name)
        {
            int i = CELLList.Count -1;

            while (i > -1)
            {
                if (CELLList[i].CellGroup == name)
                {
                    CELLList.RemoveAt(i);
                }
                i--;
            }
        }

        void MouseDown()
        {
            int i = 0;

            PtLive = PtStart;
            JzTools.SetSize(ref szInsideMove, PtStart, PtLive, true);

            Point  ptReal = ptLiveLocationIndicatorWithVirtualRatio;

            DISPOPType = DisplayOPTypeEnum.SELECT;

            i = 0;

            foreach (CellClass cell in CELLList)
            {
                cell.Backup();

                CornerEnum CatchCorner = cell.CatchCorner(ptReal);
                bool IsCatchBorder = cell.IsCatchBorder(ptReal);

                if ((CatchCorner != CornerEnum.NONE && DISPOPType == DisplayOPTypeEnum.SELECT)
                    || (IsCatchBorder && DISPOPType == DisplayOPTypeEnum.SELECT))
                {
                    if (FirstCellIndex == -1)
                    {
                        FirstCellIndex = i;
                        cell.SetFirstSelected();
                    }
                    else if (FirstCellIndex != i)
                    {
                        if (cell.IsSelected)
                        {
                            CELLList[FirstCellIndex].SetSelected();

                            FirstCellIndex = i;
                            cell.SetFirstSelected();
                        }
                        else
                        {
                            if (CellSelectCount > 0)
                            {
                                if (IsMultiSelect)
                                {
                                    cell.SetSelected();
                                }
                                else
                                {
                                    ClearAllCellSelect();

                                    FirstCellIndex = i;
                                    cell.SetFirstSelected();
                                }
                            }
                            else
                            {
                                FirstCellIndex = i;
                                cell.SetFirstSelected();
                            }
                        }
                    }

                    if (CatchCorner != CornerEnum.NONE)
                        DISPOPType = DisplayOPTypeEnum.RESIZE;
                    else
                        DISPOPType = DisplayOPTypeEnum.MOVE;
                }

                i++;
            }
            MouseMove();
        }
        void MouseHover()
        {
            Point ptReal = ptLiveLocationIndicatorWithVirtualRatio;

            READYOPType = DisplayOPTypeEnum.NONE;

            foreach (CellClass cell in CELLList)
            {
                if (cell.CellProperty == CellPropertyEnum.STATIC)
                    continue;

                if (cell.CatchCorner(ptReal) != CornerEnum.NONE)
                {
                    READYOPType = DisplayOPTypeEnum.RESIZE;
                    break;
                }
                else if (cell.IsCatchBorder(ptReal))
                {
                    READYOPType = DisplayOPTypeEnum.MOVE;
                    break;
                }
            }
            DrawViewPort();
        }
        void MouseMove()
        {
            JzTools.SetSize(ref szInsideMove, PtStart, PtLive, true);

            switch (DISPOPType)
            {
                case DisplayOPTypeEnum.SELECT:
                    rectCellSelect = rectRealRect;
                    break;
                case DisplayOPTypeEnum.RESIZE:
                    szCellResize = szLocationMove;
                    break;
                case DisplayOPTypeEnum.MOVE:
                    szCellMove = szLocationMove;
                    break;
            }
            DrawViewPort();
        }
        void DrawCell()
        {
            int i = 0;
            Rectangle szRect = JzTools.SimpleRect(szOrigin);

            CellSelectCount = 0;

            foreach (CellClass cell in CELLList)
            {
                cell.RectBoundary = szRect;

                switch (DISPOPType)
                {
                    case DisplayOPTypeEnum.SELECT:
                        if (cell.IsIntersected(rectCellSelect))
                        {
                            if (FirstCellIndex < 0 || FirstCellIndex == i)
                            {
                                FirstCellIndex = i;
                                cell.SetFirstSelected();
                            }
                            else
                            {
                                if (rectCellSelect.Width < 10 && rectCellSelect.Height < 10) //若是用點的
                                {
                                    FirstCellIndex = i;
                                    cell.SetFirstSelected();
                                }
                                else
                                    cell.SetSelected();
                            }
                        }
                        else
                        {
                            if (IsMultiSelect)
                            {
                                if (SelectionStr.IndexOf(cell.RelateIndexName) < 0)
                                    cell.SetNoSelected();
                            }
                            else
                            {
                                if (FirstCellIndex == i)
                                    FirstCellIndex = -1;

                                cell.SetNoSelected();
                            }
                        }
                        break;
                    case DisplayOPTypeEnum.MOVE:

                        if(cell.IsSelected)
                            cell.Move(szCellMove, true);
                        break;
                    case DisplayOPTypeEnum.RESIZE:

                        if (cell.IsSelected)
                            cell.ReSize(szCellResize, true, CELLList[FirstCellIndex].CornerCatched);
                        break;
                }

                if (cell.IsSelected)
                    CellSelectCount++;

                DrawInViewPortDomain(cell);

                //switch (OPType)
                //{
                //    case OPTypeEnum.EHS:
                //        DrawImage(i);
                //        break;
                //}

                //switch (OPType)
                //{
                //    case OPTypeEnum.ASN:
                //        //DrawAssignAffairs(i);
                //        DrawInViewPortDomain(cell);
                //        break;
                //    default:
                //        if (BASISList.Count > 0)
                //            DrawInViewPortDomain(cell, BASISList[i].IsDatum);
                //        else
                //            DrawInViewPortDomain(cell);

                //        break;
                //}

                //DrawInViewPortDomain(cell, BASISList[i].IsDatum);

                //switch (OPType)
                //{
                //    case OPTypeEnum.ASN:
                //        DrawAssignAffairs(i);
                //        break;
                //}

                i++;
            }

            //switch (DISPOPType)
            //{
            //    case DisplayOPTypeEnum.SELECT:
            //        OnSelect();
            //        break;
            //    case DisplayOPTypeEnum.RESIZE:
            //    case DisplayOPTypeEnum.MOVE:
            //        OnChange();
            //        break;
            //}
        }

        void DrawBasisCell()
        {
            //int i = 0;
            //Rectangle szRect = JzTools.SimpleRect(szOrigin);

            //List<CellClass> BasisCellList = new List<CellClass>();

            //foreach (BASISClass basis in BASISList)
            //{
            //    if(!basis.IsCalibration)
            //        BasisCellList.Add(basis.Cell);
            //}

            //CellSelectCount = 0;

            //foreach (CellClass cell in BasisCellList)
            //{
            //    cell.RectBoundary = szRect;

            //    switch (OPType)
            //    {
            //        default:
            //            DrawInViewPortDomain(cell, false, true);
            //            break;
            //    }
            //    //DrawInViewPortDomain(cell, BASISList[i].IsDatum);

            //    i++;
            //}

        }

        void DrawLines()
        {
            //int i = 0;
            //int j =0;

            //while (i < BASISList.Count - 1)
            //{
            //    if (BASISList[i].Cell.CellProperty != CellPropertyEnum.STATIC)
            //    {
            //        i++;
            //        continue;
            //    }
            //    j = i + 1;

            //    while (j < BASISList.Count)
            //    {
            //        if (OPType == OPTypeEnum.EHS)
            //        {
            //            if (BASISList[j].IsDatum)
            //                DrawDatum(j);
            //        }

            //        if (BASISList[j].Cell.CellProperty != CellPropertyEnum.STATIC)
            //        {
            //            j++;
            //            continue;
            //        }
            //        foreach (BorderClass borderfrom in BASISList[i].BorderList)
            //        {
            //            foreach (BorderClass borderto in BASISList[j].BorderList)
            //            {
            //                if (borderto.ToCheckString() == borderfrom.ToCheckString())
            //                {
            //                    DrawCellLine(BASISList[i].Cell.RectCell, BASISList[j].Cell.RectCell);
            //                }
            //            }
            //        }
            //        j++;
            //    }
            //    i++;
            //}
        }
        void DrawCellLine(Rectangle FromRect, Rectangle ToRect)
        {
            int i = 0;

            Rectangle frect = FromRect;
            Rectangle trect = ToRect;

            //if (OPType == OPTypeEnum.EHS)
            //{
            //    frect.Offset(XBias, YBias);
            //    trect.Offset(XBias, YBias);
            //}

            Rectangle rectFrom = JzTools.Resize(frect, iViewPortRatio);
            Rectangle rectTo = JzTools.Resize(trect, iViewPortRatio);

            PKeyBorder.Color = Color.Yellow;
            //if (!INI.ISREVERSE)
            //    PKeyBorder.Color = Color.Yellow;
            //else
            //    PKeyBorder.Color = Color.Purple;


            rectFrom.X = rectFrom.X - JzTools.ShiftValue(ptViewPortLocationLive.X, iViewPortRatio);
            rectFrom.Y = rectFrom.Y - JzTools.ShiftValue(ptViewPortLocationLive.Y, iViewPortRatio);

            rectTo.X = rectTo.X - JzTools.ShiftValue(ptViewPortLocationLive.X, iViewPortRatio);
            rectTo.Y = rectTo.Y - JzTools.ShiftValue(ptViewPortLocationLive.Y, iViewPortRatio);

            DISPUI.DrawLine(JzTools.GetRectCenter(rectFrom), JzTools.GetRectCenter(rectTo), PKeyBorder);
        }
        void DrawImage(int Index)
        {
            //ENHANCEClass enhance = ENHANCEList[Index];

            //if (enhance.Brightness != 0 || enhance.Contrast != 0)
            //{
            //    CellClass cell = enhance.Cell;

            //    Rectangle rect = JzTools.Resize(cell.RectCell, iViewPortRatio);

            //    rect.X = rect.X - JzTools.ShiftValue(ptViewPortLocationLive.X, iViewPortRatio);
            //    rect.Y = rect.Y - JzTools.ShiftValue(ptViewPortLocationLive.Y, iViewPortRatio);

            //    DISPUI.DrawImage(enhance.GetMIDDLEBMP(), rect);
            //}
        }
        void DrawDatum(int Index)
        {
            //BASISClass basis = BASISList[Index];

            //CellClass cell = basis.Cell;

            //Rectangle crect = cell.RectCell;

            //crect.Offset(XBias, YBias);

            //Rectangle rect = JzTools.Resize(crect, iViewPortRatio);

            //rect.X = rect.X - JzTools.ShiftValue(ptViewPortLocationLive.X, iViewPortRatio);
            //rect.Y = rect.Y - JzTools.ShiftValue(ptViewPortLocationLive.Y, iViewPortRatio);

            //DISPUI.DrawRect(rect, cell.SBorder);

        }

        void DrawAssignAffairs(int Index)
        {
            //int i = 0;

            //ASSIGNClass assign = ASSIGNList[Index];

            //CellClass cell = assign.Cell;

            //Rectangle rect = JzTools.Resize(cell.RectCell, iViewPortRatio);

            //rect.X = rect.X - JzTools.ShiftValue(ptViewPortLocationLive.X, iViewPortRatio);
            //rect.Y = rect.Y - JzTools.ShiftValue(ptViewPortLocationLive.Y, iViewPortRatio);

            //Rectangle LineRect = rect;

            //i = 0;
            //while (i < 5)
            //{
            //    Pen PLine = new Pen(Color.Red, 2);
            //    PLine.DashStyle = DashStyle.DashDot;

            //    LineRect.Inflate(-3, -3);

            //    switch (i)
            //    {
            //        case 0:
            //            PLine = new Pen(Color.Red, 2);
            //            break;
            //        case 1:
            //            PLine = new Pen(Color.Blue, 2);
            //            break;
            //        case 2:
            //            PLine = new Pen(Color.Yellow, 2);
            //            break;
            //        case 3:
            //            PLine = new Pen(Color.Green, 2);
            //            break;
            //        case 4:
            //            PLine = new Pen(Color.Purple, 2);
            //            break;
            //    }

            //    foreach (BorderClass border in assign.BorderList)
            //    {
            //        if (border.BorderSerial == i)
            //        {
            //            switch (border.BorderType)
            //            {
            //                case BorderTypeEnum.LEFT:
            //                    DISPUI.DrawLine(new Point(LineRect.Left, LineRect.Top), new Point(LineRect.Left, LineRect.Bottom), PLine);
            //                    break;
            //                case BorderTypeEnum.RIGHT:
            //                    DISPUI.DrawLine(new Point(LineRect.Right, LineRect.Top), new Point(LineRect.Right, LineRect.Bottom), PLine);
            //                    break;
            //                case BorderTypeEnum.TOP:
            //                    DISPUI.DrawLine(new Point(LineRect.Left, LineRect.Top), new Point(LineRect.Right, LineRect.Top), PLine);
            //                    break;
            //                case BorderTypeEnum.BOTTOM:
            //                    DISPUI.DrawLine(new Point(LineRect.Left, LineRect.Bottom), new Point(LineRect.Right, LineRect.Bottom), PLine);
            //                    break;
            //            }
            //        }
            //    }
            //    i++;
            //}

            //DISPUI.DrawText(assign.ReportIndexStr, new Point(rect.Location.X + 5, rect.Location.Y + (rect.Height >> 1) - 5));
        }

        public void FindFirstCell()
        {
            int i = 0;

            FirstCellIndex = -1;

            foreach (CellClass cell in CELLList)
            {
                if (cell.IsFirstSelected)
                {
                    FirstCellIndex = i;
                    break;
                }
                i++;
            }
        }
        void ClearAllCellSelect()
        {
            foreach (CellClass cell in CELLList)
            {
                cell.SetNoSelected();
            }
        }

        public void MultiBackup()
        {
            if (IsMultiSelect)
                return;

            IsMultiSelect = true;
            MultiSelectionSet();
        }
        public void MultiSelectionSet()
        {
            if (!IsMultiSelect)
                return;

            SelectionStr = "";

            foreach (CellClass cell in CELLList)
            {
                if (cell.IsSelected)
                {
                    SelectionStr += cell.RelateIndexName + ",";
                }
            }
            SelectionStr = JzTools.RemoveLastChar(SelectionStr, 1);
        }
        public void MultiComplete()
        {
            IsMultiSelect = false;
            SelectionStr = "";
        }

        #endregion

        void DrawInViewPortDomain(CellClass cell)
        {
            //int i = 0;

            //Rectangle rect = JzTools.Resize(cell.RectCell, iViewPortRatio);

            //rect.X = rect.X - JzTools.ShiftValue(ptViewPortLocationLive.X, iViewPortRatio);
            //rect.Y = rect.Y - JzTools.ShiftValue(ptViewPortLocationLive.Y, iViewPortRatio);

            //switch (cell.CellProperty)
            //{
            //    case CellPropertyEnum.DYNAMIC:
            //        DISPUI.DrawRect(rect, cell.PBorder);
            //        DISPUI.DrawRect(rect, cell.POperate);
            //        break;
            //    case CellPropertyEnum.STATIC:
            //        DISPUI.DrawRect(rect, cell.PBorder);
            //        DISPUI.DrawRect(rect, cell.POperate);
            //        break;
            //}

            //if (cell.CellProperty == CellPropertyEnum.STATIC)
            //    return;

            //i = 0;
            //while (i < (int)CornerEnum.COUNT)
            //{
            //    DISPUI.DrawRect(cell.RectCorner(rect, (CornerEnum)i), cell.SOperate);
            //    i++;
            //}

            DrawInViewPortDomain(cell, false);
        }
        void DrawInViewPortDomain(CellClass cell, bool IsBasDatum)
        {
            DrawInViewPortDomain(cell, IsBasDatum, false);
        }
        void DrawInViewPortDomain(CellClass cell, bool IsBasDatum, bool isHallow)
        {
            int i = 0;

            Rectangle rect = JzTools.Resize(cell.RectCell, iViewPortRatio);

            rect.X = rect.X - JzTools.ShiftValue(ptViewPortLocationLive.X, iViewPortRatio);
            rect.Y = rect.Y - JzTools.ShiftValue(ptViewPortLocationLive.Y, iViewPortRatio);

            switch (cell.CellProperty)
            {
                case CellPropertyEnum.DYNAMIC:
                    DISPUI.DrawRect(rect, cell.PBorder);
                    DISPUI.DrawRect(rect, cell.POperate);

                    //DISPUI.DrawRect(rect, new Pen(Color.Red));

                    break;
                case CellPropertyEnum.STATIC:

                    DISPUI.DrawRect(rect, cell.PBorder);
                    DISPUI.DrawRect(rect, cell.POperate);

                    //DISPUI.DrawRect(rect, new Pen(Color.Red));

                    if (IsBasDatum)
                    {
                        Rectangle rectdatum = rect;
                        rectdatum.Inflate(5, 5);

                        DISPUI.DrawRect(rectdatum, cell.PBorder);
                        DISPUI.DrawRect(rectdatum, cell.PDatum);
                    }

                    break;
            }

            if (cell.CellProperty == CellPropertyEnum.STATIC)
                return;

            i = 0;
            while (i < (int)CornerEnum.COUNT)
            {
                DISPUI.DrawRect(cell.RectCorner(rect, (CornerEnum)i), cell.SOperate);
                i++;
            }

        }
        public void Tick()
        {
            if (IsLeftMouseDown)
                MouseMove();
        }

        public delegate void SelectrHandler();
        public event SelectrHandler SelectAction;
        public void OnSelect()
        {
            if (SelectAction != null)
            {
                SelectAction();
            }
        }
        public delegate void ChangeHandler();
        public event ChangeHandler ChangeAction;
        public void OnChange()
        {
            if (ChangeAction != null)
            {
                ChangeAction();
            }
        }

    }
}
