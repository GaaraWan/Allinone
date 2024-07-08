using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

using JzKHC.AOISpace;
using JetEazy.BasicSpace;
using JzKHC.DBSpace;
//using Jumbo301.UniversalSpace;
using JetEazy;

namespace JzKHC.ControlSpace
{
    class OPScreenUIKeyAssignClass : OPScreenUIClass
    {
        KeyboardClass KEYBOARD
        {
            get
            {
                return Universal.KEYBOARD;
            }
        }
        RecipeDBClass RECIPEDB
        {
            get
            {
                return Universal.RECIPEDB;
            }

        }
        
        Size szInsideMove = new Size();
        protected Size szLocationMove
        {
            get
            {
                Size szTmp = new Size();

                szTmp.Width = JzTools.ShiftValue(szInsideMove.Width, -(iViewPortRatio - VirtualRatio));
                szTmp.Height = JzTools.ShiftValue(szInsideMove.Height, -(iViewPortRatio - VirtualRatio));

                return szTmp;
            }
        }
        Size szRealLocationMove
        {
            get
            {
                Size szTmp = new Size();

                szTmp.Width = JzTools.BoundValue(ptLiveLocationIndicatorWithVirtualRatio.X - ptStartLocationIndicatorWithVirtualRatio.X, JzTools.ShiftValue(szOrigin.Width - 1, -VirtualRatio), 1);
                szTmp.Height = JzTools.BoundValue(ptLiveLocationIndicatorWithVirtualRatio.Y - ptStartLocationIndicatorWithVirtualRatio.Y, JzTools.ShiftValue(szOrigin.Height - 1, -VirtualRatio), 1);

                return szTmp;
            }
        }

        public bool IsWaitForSelection = false;
        public bool IsWaitForCopy = false;
        SubstractClass Substract = new SubstractClass();

        public OPTypeEnum LastOPType = 0;
        public int LastStartIndex = 0;
        
        Rectangle rectConvertToViewDomain(Rectangle HatRect)
        {
            Rectangle rect = rectOriginViewRange;

            rect.Intersect(JzTools.Resize(HatRect, VirtualRatio));

            rect.X = Math.Max(rect.X - rectOriginViewRange.X, 0);
            rect.Y = Math.Max(rect.Y - rectOriginViewRange.Y, 0);

            return JzTools.Resize(rect, (iViewPortRatio - VirtualRatio));

            //JzTools.DrawRect(bmpViewPortCorpZoom, JzTools.Resize(rect, (iViewPortRatio - VirtualRatio)), KeyboardRangePen);

        }

        Pen KeyboardInsideSpecPenW = new Pen(Color.Blue, 2);
        Pen KeyboardInsideSpecPenH = new Pen(Color.Blue, 2);

        Pen KeyboardOutsideSpecPenW = new Pen(Color.Pink, 2);
        Pen KeyboardOutsideSpecPenH = new Pen(Color.Pink, 2);

        Pen KeyboardCornerPen = new Pen(Color.Blue, 2);
        SolidBrush KeyboardCornerBrush = new SolidBrush(Color.Blue);

        Pen KeyboardSelectPen = new Pen(Color.YellowGreen, 2);
        Pen KeyboardSelectedPen = new Pen(Color.Red, 2);
        Pen KeyboardSelectedVeryFirstPen = new Pen(Color.White, 2);

        Pen KeyboardRangePen = new Pen(Color.Orange, 2);
        Pen KeyboardRangeOutSidePen = new Pen(Color.Blue, 2);

        Pen KeyboardLockPen = new Pen(Color.Blue, 6);
        Pen KeyboardNoLockPen = new Pen(Color.Black, 6);

        //CornerEnum IntersectedCorner = CornerEnum.NONE;
        //int IntersectedCornerIndex = -1;

        public OPScreenUIKeyAssignClass(OPScreenUIControl rOPScreenUI, int DefaultRatio, int rMinRatio, int rMaxRatio)
            : base(rOPScreenUI, DefaultRatio, rMinRatio, rMaxRatio)
        {

        }

        protected override void picOperation_MouseUp(object sender, MouseEventArgs e)
        {
            base.picOperation_MouseUp(sender, e);

            switch (OPType)
            {
                case OPTypeEnum.SELECT:
                    SelectAction();
                    OnPlaceHat();
                    break;
                case OPTypeEnum.MOVE:
                    MoveAction();
                    RedrawKeycap();
                    OnPlaceHat();
                    break;
                case OPTypeEnum.RESIZE:
                    ResizeAction();
                    RedrawKeycap();
                    OnPlaceHat();
                    break;
            }
            LastOPType = OPType;

            OPType = OPTypeEnum.NONE;
            CorpImageInViewPort(ref ptViewPortLocationLive);

        }
        protected override void picOperation_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            base.picOperation_MouseDown(sender, e);

            switch (e.Button)
            {
                case MouseButtons.Left:
                    PtLive = PtStart;

                    JzTools.SetSize(ref szInsideMove, PtStart, PtLive, true);

                    #region Hat Operation

                    int i = 0;
                    CornerEnum iCorner = CornerEnum.NONE;
                    KeyAssignClass keyassigntmp;

                    bool iInside = false;
                    bool iRepeat = false;
                    bool iSelected = false;

                    //先檢測是否在原有的地方點的
                    foreach (KeyAssignClass keyassign in KEYBOARD.vKEYASSIGNLIST)
                    {
                        iCorner = keyassign.IsInsideCorner(ptLiveLocationIndicatorWithVirtualRatio);
                        iInside = keyassign.IsInside(ptLiveLocationIndicatorWithVirtualRatio);

                        if (iCorner != CornerEnum.NONE || iInside)
                        {
                            iSelected = true;
                            if (keyassign.IsSelected)
                            {
                                iRepeat = true;
                            }
                            else //ADDED BY VICTOR !!!
                            {
                                if (IsWaitForSelection)
                                {
                                    foreach (KeyAssignClass keyassign1 in KEYBOARD.vKEYASSIGNLIST)
                                    {
                                        //keyassign1.IsSelected = false;
                                        keyassign1.IsSelectedStart = false;
                                    }
                                }
                                else
                                {
                                    foreach (KeyAssignClass keyassign1 in KEYBOARD.vKEYASSIGNLIST)
                                    {
                                        keyassign1.IsSelected = false;
                                        keyassign1.IsSelectedStart = false;
                                    }
                                }
                                keyassign.IsSelectedStart = true;
                                keyassign.IsSelected = true;
                            }
                            break;
                        }
                        //else if (IsWaitForSelection)
                        //{
                        //    LastStartIndex=0;

                        //    foreach (KeyAssignClass keyassign1 in KEYBOARD.vKEYASSIGNLIST)
                        //    {
                        //        if (keyassign1.IsSelectedStart)
                        //            break;
                        //        LastStartIndex++;
                        //    }
                        //}
                    }

                    if (iSelected == false)
                    {
                        OPType = OPTypeEnum.SELECT;
                        if (!IsWaitForSelection)
                        {
                            foreach (KeyAssignClass keyassign in KEYBOARD.vKEYASSIGNLIST)
                            {
                                keyassign.IsSelected = false;
                                keyassign.IsSelectedStart = false;
                            }
                        }
                        else
                        {
                            foreach (KeyAssignClass keyassign in KEYBOARD.vKEYASSIGNLIST)
                            {
                                keyassign.IsSelectedStart = false;
                            }
                        }
                    }
                    else
                    {
                        if (IsWaitForCopy)
                        {
                            int i1 = 0, j1 = KEYBOARD.vKEYASSIGNLIST.Count;

                            OPType = OPTypeEnum.MOVE;

                            while (i1 < j1)
                            {
                                if (KEYBOARD.vKEYASSIGNLIST[i1].IsSelected)
                                {
                                    keyassigntmp = KEYBOARD.vKEYASSIGNLIST[i1].CloneAdded(int.Parse(JzTools.GetLastString(KEYBOARD.vKEYASSIGNLIST[KEYBOARD.vKEYASSIGNLIST.Count - 1].Name, 3)), true, bmpBareOrigion);
                                    KEYBOARD.vKEYASSIGNLIST.Add(keyassigntmp);

                                    KEYBOARD.vKEYASSIGNLIST[i1].IsSelectedStart = false;
                                    KEYBOARD.vKEYASSIGNLIST[i1].IsSelected = false;
                                }
                                i1++;
                            }

                            foreach (KeyAssignClass keyassign in KEYBOARD.vKEYASSIGNLIST)
                            {
                                keyassign.BackupRect();
                            }

                            OnCopyHat();
                        }
                        else
                        {
                            iCorner = CornerEnum.NONE;
                            i = 0;

                            foreach (KeyAssignClass keyassign in KEYBOARD.vKEYASSIGNLIST)
                            {
                                keyassign.BackupRect();

                                iCorner = keyassign.IsInsideCorner(ptLiveLocationIndicatorWithVirtualRatio);

                                if (iCorner != CornerEnum.NONE)
                                {
                                    if (OPType == OPTypeEnum.NONE)
                                    {
                                        OPType = OPTypeEnum.RESIZE;
                                        OPCorner = iCorner;
                                        OPIndex = i;

                                        keyassign.IsSelected = true;
                                        keyassign.IsSelectedStart = true;
                                    }
                                    else
                                    {
                                        if (!IsWaitForSelection)
                                        {
                                            keyassign.IsSelected = false;
                                            keyassign.IsSelectedStart = false;
                                        }
                                    }
                                }
                                else if (keyassign.IsInside(ptLiveLocationIndicatorWithVirtualRatio))
                                {
                                    if (OPType == OPTypeEnum.NONE)
                                    {
                                        OPType = OPTypeEnum.MOVE;
                                        OPIndex = i;

                                        keyassign.IsSelected = true;
                                        keyassign.IsSelectedStart = true;
                                    }
                                    else
                                    {
                                        if (!IsWaitForSelection)
                                        {
                                            keyassign.IsSelected = false;
                                            keyassign.IsSelectedStart = false;
                                        }
                                    }
                                }
                                else
                                {
                                    if (!iRepeat)
                                    {
                                        if (!IsWaitForSelection)
                                        {
                                            keyassign.IsSelected = false;
                                        }
                                    }

                                    keyassign.IsSelectedStart = false;

                                }
                                i++;
                            }
                        }
                    }

                    #endregion

                    DrawViewPortDomain();
                    OnSelectHat();
                    break;
            }
        }
        protected override void picOperation_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            base.picOperation_MouseMove(sender, e);

            switch (e.Button)
            {
                case MouseButtons.Left:

                    JzTools.SetSize(ref szInsideMove, PtStart, PtLive, true);

                    switch (OPType)
                    {
                        case OPTypeEnum.SELECT:
                            if (tmMovingDelay.msDuriation > MovingDelay)
                            {
                                SelectAction();
                                tmMovingDelay.Cut();
                            }
                            break;
                        case OPTypeEnum.MOVE:
                            if (tmMovingDelay.msDuriation > MovingDelay)
                            {
                                MoveAction();
                                tmMovingDelay.Cut();
                            }
                            break;
                        case OPTypeEnum.RESIZE:
                            if (tmMovingDelay.msDuriation > MovingDelay)
                            {
                                ResizeAction();
                                tmMovingDelay.Cut();
                            }
                            break;
                    }
                    break;
            }
        }
        protected override void picOperation_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            base.picOperation_MouseDoubleClick(sender, e);
            switch (e.Button)
            {
                case MouseButtons.Left:
                    //while (i < SIDE.LiveIndexList.Count)
                    //{
                    //    SIDE.LiveList[SIDE.LiveIndexList[i]].IsHaveModified = IsWaitForSelection;
                    //    i++;
                    //}
                    //SIDE.DblSeletedLive();
                    //DrawViewPortDomain();
                    OnPlaceHat();
                    break;
            }
        }

        void SelectAction()
        {
            Rectangle RectToIntersect = JzTools.RectTwoPoint(ptStartLocationIndicatorWithVirtualRatio, ptLiveLocationIndicatorWithVirtualRatio, JzTools.Resize(szOrigin, -VirtualRatio));
            bool IsFoundFirst = false;
            bool IsOneIntersected = false;

            foreach (KeyAssignClass keyassign in KEYBOARD.vKEYASSIGNLIST)
            {
                if (RectToIntersect.IntersectsWith(keyassign.myrect))
                {
                    if (!IsFoundFirst)
                    {
                        keyassign.IsSelected = true;
                        keyassign.IsSelectedStart = true;
                        IsFoundFirst = true;
                    }
                    else
                    {
                        keyassign.IsSelected = true;
                        keyassign.IsSelectedStart = false;
                    }

                    IsOneIntersected = true;
                }
                else
                {
                    if (!IsWaitForSelection)
                    {
                        keyassign.IsSelectedStart = false;
                        keyassign.IsSelected = false;
                    }
                }
            }

            //if (IsWaitForSelection)
            //{
            //    if (LastStartIndex < KEYBOARD.vKEYASSIGNLIST.Count)
            //    {

            //        if (!IsOneIntersected)
            //        {
            //            KEYBOARD.vKEYASSIGNLIST[LastStartIndex].IsSelectedStart = true;
            //        }
            //        else
            //        {
            //            KEYBOARD.vKEYASSIGNLIST[LastStartIndex].IsSelectedStart = false;
            //        }
            //    }
            //}

            DrawViewPortDomain();
            OnSelectHat();
        }
        void MoveAction()
        {
            foreach (KeyAssignClass keyassign in KEYBOARD.vKEYASSIGNLIST)
            {
                if (keyassign.IsSelected)
                {
                    keyassign.MoveRect(JzTools.ShiftValue(szInsideMove.Width, -iViewPortRatio), JzTools.ShiftValue(szInsideMove.Height, -iViewPortRatio));
                }
            }

            DrawViewPortDomain();
            OnSelectHat();
        }
        void ResizeAction()
        {
            foreach (KeyAssignClass keyassign in KEYBOARD.vKEYASSIGNLIST)
            {
                if (keyassign.IsSelected)
                {
                    keyassign.SizedRect(JzTools.ShiftValue(szInsideMove.Width, -iViewPortRatio), JzTools.ShiftValue(szInsideMove.Height, -iViewPortRatio), OPCorner);
                }
            }

            DrawViewPortDomain();
            OnSelectHat();
        }

        void RedrawKeycap()
        {
            foreach (KeyAssignClass keyassign in KEYBOARD.vKEYASSIGNLIST)
            {
                if (keyassign.IsSelected)
                {
                    keyassign.GetBMP(bmpBareOrigion);
                }
            }
        }

        public void Refresh()
        {
            CorpImageInViewPort(ref ptViewPortLocationLive);
        }

        void DrawInViewPortDomain(Rectangle vRect, Pen P, KeyAssignClass keyassign)
        {
            Rectangle rect = JzTools.Resize(vRect, iViewPortRatio);

            rect.X = rect.X - JzTools.ShiftValue(ptViewPortLocationLive.X, iViewPortRatio);
            rect.Y = rect.Y - JzTools.ShiftValue(ptViewPortLocationLive.Y, iViewPortRatio);

            OPScreenUI.DrawRect(rect, KeyboardNoLockPen);

            OPScreenUI.DrawRect(rect, P);

            int i = 0;

            i = 0;
            while (i < (int)CornerEnum.COUNT)
            {
                OPScreenUI.DrawRect(JzTools.CornerRect(rect, (CornerEnum)i, KeyAssignClass.CornerSize), new SolidBrush(P.Color));

                i++;
            }

            PointF Pt = (PointF)JzTools.GetRectCenter(rect);
            Pt.X -= 10;
            Pt.Y -= 10;
            OPScreenUI.DrawText(keyassign.ReportIndex.ToString("0000"), Pt); 

            //Modified By Victor 

            Rectangle rectIndicator = rect;

            rectIndicator.Inflate(-20, -20);

            OPScreenUI.DrawRect(rectIndicator, new Pen(Color.Blue, 2));

            i = 0;
            while (i < (int)CornerExEnum.COUNT)
            {
                if (i < 4)
                {
                    if (keyassign.inBaseIndicator[i] != null)
                        OPScreenUI.DrawRect(JzTools.CornerRect(rectIndicator, (CornerEnum)i, 8), new SolidBrush(Color.Orange));

                    if (keyassign.outBaseIndicator[i] != null)
                        OPScreenUI.DrawRect(JzTools.CornerRect(rectIndicator, (CornerEnum)i, 6), new Pen(Color.Red, 5));
                }
                else
                {
                    if (keyassign.inBaseIndicator[i] != null)
                        OPScreenUI.DrawRect(JzTools.SimpleRect(JzTools.GetRectCenter(rectIndicator), 8), new SolidBrush(Color.Orange));

                    if (keyassign.outBaseIndicator[i] != null)
                        OPScreenUI.DrawRect(JzTools.SimpleRect(JzTools.GetRectCenter(rectIndicator), 6), new Pen(Color.Red, 5));
                }
                i++;
            }
        }
        void DrawInViewPortDomain(ref Rectangle[] vRect)
        {
            int i = 0;
            while (i < vRect.Length)
            {
                vRect[i] = JzTools.Resize(vRect[i], iViewPortRatio);
                vRect[i].X = vRect[i].X - JzTools.ShiftValue(ptViewPortLocationLive.X, iViewPortRatio);
                vRect[i].Y = vRect[i].Y - JzTools.ShiftValue(ptViewPortLocationLive.Y, iViewPortRatio);
                i++;
            }
            OPScreenUI.DrawRect(vRect, KeyboardSelectedPen);
        }

        protected override void CorpImageInViewPort(ref Point ViewPortLocation)
        {
            //return;

            Size szTmp = szViewPortLocationMove;

            ViewPortLocation.X = JzTools.BoundValue(ViewPortLocation.X, szOrigin.Width - szTmp.Width - ExtendOutLineLength, 0);
            ViewPortLocation.Y = JzTools.BoundValue(ViewPortLocation.Y, szOrigin.Height - szTmp.Height - ExtendOutLineLength, 0);

            rectOriginViewRange = new Rectangle(ViewPortLocation, szViewPortCorpSize);

            BonudRect(ref rectOriginViewRange, szOrigin);

            ViewPortLocation = rectOriginViewRange.Location;

            bmpViewPortCorp.Dispose();
            bmpViewPortCorp = (Bitmap)bmpOperate.Clone(rectOriginViewRange, PixelFormat.Format32bppArgb);

            bmpViewPortCorpZoom.Dispose();
            bmpViewPortCorpZoom = new Bitmap(bmpViewPortCorp, JzTools.Resize(rectOriginViewRange.Size, (iViewPortRatio - VirtualRatio)));

            //JzTools.DrawRect(bmpViewPortCorpZoom, JzTools.Resize(rectKeyboardRange, (iViewPortRatio - VirtualRatio)), KeyboardRangePen);
            //JzTools.DrawRect(bmpViewPortCorpZoom, JzTools.Resize(rectKeyboardRangeNext, (iViewPortRatio - VirtualRatio)), KeyboardRangeOutSidePen);

            //SetTip(bmpViewPortCorpZoom, LastOPType.ToString() + "," + OPType.ToString(), 10);
            OPScreenUI.SetPicture(bmpViewPortCorpZoom);

            DrawViewPortDomain();

        }
        public void DrawViewPortDomain()
        {
            int i = 0;

            OPScreenUI.DrawStart();

            foreach (KeyAssignClass keyassign in KEYBOARD.vKEYASSIGNLIST)
            {
                //if (keyassign.belongKeycapIndex == -1)
                //{
                //    if (keyassign.IsSelectedStart)
                //        DrawInViewPortDomain(keyassign.myrect, new Pen(Color.Yellow, 5), keyassign);
                //    else if (keyassign.IsSelected)
                //        DrawInViewPortDomain(keyassign.myrect, new Pen(Color.Orange, 5), keyassign);
                //    else
                //        DrawInViewPortDomain(keyassign.myrect, new Pen(Color.DarkOrange, 5), keyassign);
                //}
                //else
                //{
                if (keyassign.IsSelectedStart)
                    DrawInViewPortDomain(keyassign.myrect, KeyboardSelectedVeryFirstPen, keyassign);
                else
                {
                    if (keyassign.IsSelected)
                        DrawInViewPortDomain(keyassign.myrect, KeyboardSelectedPen, keyassign);
                    else
                        DrawInViewPortDomain(keyassign.myrect, keyassign.myPen, keyassign);
                }
                //}
                i++;
            }

            switch (OPType)
            {
                case OPTypeEnum.SELECT:
                case OPTypeEnum.CHECKSELECT:
                    OPScreenUI.DrawRect(JzTools.RectTwoPoint(PtStart, PtLive, szViewPort), KeyboardSelectPen);
                    break;
            }

            OPScreenUI.DrawEnd();

            //OnSelectHat();
            //OnSelectCheck();
        }

        public override void Dispose()
        {
            base.Dispose();

            KeyboardInsideSpecPenW.Dispose();
            KeyboardInsideSpecPenH.Dispose();

            KeyboardOutsideSpecPenW.Dispose();
            KeyboardOutsideSpecPenH.Dispose();

            KeyboardCornerPen.Dispose();
            KeyboardCornerBrush.Dispose();

            KeyboardSelectPen.Dispose();
            KeyboardSelectedPen.Dispose();
            KeyboardSelectedVeryFirstPen.Dispose();

            KeyboardRangePen.Dispose();
            KeyboardRangeOutSidePen.Dispose();

            KeyboardLockPen.Dispose();
            KeyboardNoLockPen.Dispose();

        }

        public delegate void SelectHatHandler();
        public event SelectHatHandler SelectHatAction;
        public void OnSelectHat()
        {
            if (SelectHatAction != null)
            {
                SelectHatAction();
            }
        }
        public delegate void SelectCheckHandler();
        public event SelectCheckHandler SelectCheckAction;
        public void OnSelectCheck()
        {
            if (SelectCheckAction != null)
            {
                SelectCheckAction();
            }
        }
        public delegate void PlaceHatHandler();
        public event PlaceHatHandler PlaceHatAction;
        public void OnPlaceHat()
        {
            if (PlaceHatAction != null)
            {
                PlaceHatAction();
            }
        }

        public delegate void CopyHatHandler();
        public event CopyHatHandler CopyHatAction;
        public void OnCopyHat()
        {
            if (CopyHatAction != null)
            {
                CopyHatAction();
            }
        }

        private void BonudRect(ref Rectangle InnerRect, Size BoundSize)
        {
            InnerRect.X = Math.Min(Math.Max(InnerRect.X, 0), (BoundSize.Width - InnerRect.Width < 0 ? 0 : BoundSize.Width - InnerRect.Width));
            InnerRect.Y = Math.Min(Math.Max(InnerRect.Y, 0), (BoundSize.Height - InnerRect.Height < 0 ? 0 : BoundSize.Height - InnerRect.Height));

            if (BoundSize.Width <= InnerRect.X + InnerRect.Width)
                InnerRect.Width = _BoundValue(InnerRect.Width, BoundSize.Width - InnerRect.X, 1);
            if (BoundSize.Height <= InnerRect.Height + InnerRect.Height)
                InnerRect.Height = _BoundValue(InnerRect.Height, BoundSize.Height - InnerRect.Y, 1);
        }
        private int _BoundValue(int Value, int Max, int Min)
        {
            return Math.Max(Math.Min(Value, Max), Min);

        }
    }
}
