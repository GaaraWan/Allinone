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
//using Jumbo301.DBSpace;
//using Jumbo301.UniversalSpace;
using JetEazy;

namespace JzKHC.ControlSpace
{
    class OPScreenUIKeyBaseClass : OPScreenUIClass
    {
        public SideEnum mySide = SideEnum.SIDE0;

        public RadioButton rdoNormal;
        public RadioButton rdoAnalyze;
        public RadioButton rdoExam;
        public CheckBox chkIsNoLine;

        bool IsNoLine
        {
            get
            {
                return chkIsNoLine.Checked;
            }
        }

        public bool IsExam
        {
            get
            {
                return rdoExam.Checked;
            }
        }
        public bool IsAnalyze
        {
            get
            {
                return rdoAnalyze.Checked;
            }
        }

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
        SideClass SIDE
        {
            get
            {
                return KEYBOARD.SIDES[(int)mySide];
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

        Rectangle rectKeyboardRange
        {
            get
            {
                Rectangle rect = new Rectangle(0, 0, szOrigin.Width, szOrigin.Height);
                Point Pt = new Point();

                switch (mySide)
                {
                    case SideEnum.SIDE0:
                        rect.X = 0;
                        rect.Y = 0;
                        break;
                    case SideEnum.SIDE1:
                        rect.X = INI.SIDE1LOCATION.X;
                        rect.Y = INI.SIDE1LOCATION.Y;

                        Pt.X = INI.SIDE1LOCATION.X;
                        Pt.Y = INI.SIDE1LOCATION.Y;

                        break;
                    case SideEnum.SIDE2:
                        rect.X = INI.SIDE2LOCATION.X;
                        rect.Y = INI.SIDE2LOCATION.Y;

                        Pt.X = INI.SIDE2LOCATION.X;
                        Pt.Y = INI.SIDE2LOCATION.Y;
                        break;
                    case SideEnum.SIDE3:
                        rect.X = INI.SIDE3LOCATION.X;
                        rect.Y = INI.SIDE3LOCATION.Y;

                        Pt.X = INI.SIDE3LOCATION.X;
                        Pt.Y = INI.SIDE3LOCATION.Y;
                        break;
                    case SideEnum.SIDE4:
                        rect.X = INI.SIDE4LOCATION.X;
                        rect.Y = INI.SIDE4LOCATION.Y;

                        Pt.X = INI.SIDE4LOCATION.X;
                        Pt.Y = INI.SIDE4LOCATION.Y;
                        break;
                    case SideEnum.SIDE5:
                        rect.X = INI.SIDE5LOCATION.X;
                        rect.Y = INI.SIDE5LOCATION.Y;

                        Pt.X = INI.SIDE5LOCATION.X;
                        Pt.Y = INI.SIDE5LOCATION.Y;
                        break;
                    case SideEnum.SIDE6:
                        rect.X = INI.SIDE6LOCATION.X;
                        rect.Y = INI.SIDE6LOCATION.Y;

                        Pt.X = INI.SIDE6LOCATION.X;
                        Pt.Y = INI.SIDE6LOCATION.Y;
                        break;
                }
                //rect.Intersect(JzTools.Resize(RECIPEDB.rectRecipeRange, VirtualRatio));//Gaara by mask

                rect.X = rect.X - rectOriginViewRange.X - Pt.X;
                rect.Y = rect.Y - rectOriginViewRange.Y - Pt.Y;

                return rect;
            }
        }

        Pen KeycapPen = new Pen(Color.FromArgb(150, Color.Yellow), 2);
        Pen KeybasePen = new Pen(Color.FromArgb(180,Color.Red), 2);

        Rectangle rectConvertToViewDomain(Rectangle HatRect)
        {
            Rectangle rect = rectOriginViewRange;

            rect.Intersect(JzTools.Resize(HatRect, VirtualRatio));

            rect.X = Math.Max(rect.X - rectOriginViewRange.X, 0);
            rect.Y = Math.Max(rect.Y - rectOriginViewRange.Y, 0);

            return JzTools.Resize(rect, (iViewPortRatio - VirtualRatio));

            //JzTools.DrawRect(bmpViewPortCorpZoom, JzTools.Resize(rect, (iViewPortRatio - VirtualRatio)), KeyboardRangePen);

        }
        Rectangle rectKeyboardRangeNext
        {
            get
            {
                Rectangle rect = new Rectangle(0, 0, szOrigin.Width, szOrigin.Height);

                switch (mySide)
                {
                    case SideEnum.SIDE0:
                        rect.X = INI.SIDE1LOCATION.X;
                        rect.Y = INI.SIDE1LOCATION.Y;
                        break;
                    case SideEnum.SIDE1:
                        rect.X = INI.SIDE2LOCATION.X - INI.SIDE1LOCATION.X;
                        rect.Y = INI.SIDE2LOCATION.Y - INI.SIDE1LOCATION.Y;
                        break;
                    case SideEnum.SIDE2:
                        rect.X = INI.SIDE3LOCATION.X - INI.SIDE2LOCATION.X;
                        rect.Y = INI.SIDE3LOCATION.Y - INI.SIDE2LOCATION.Y;
                        break;
                    case SideEnum.SIDE3:
                        rect.X = INI.SIDE4LOCATION.X - INI.SIDE3LOCATION.X;
                        rect.Y = INI.SIDE4LOCATION.Y - INI.SIDE3LOCATION.Y;
                        break;
                    case SideEnum.SIDE4:
                        rect.X = INI.SIDE5LOCATION.X - INI.SIDE4LOCATION.X;
                        rect.Y = INI.SIDE5LOCATION.Y - INI.SIDE4LOCATION.Y;
                        break;
                    case SideEnum.SIDE5:
                        rect.X = INI.SIDE6LOCATION.X - INI.SIDE5LOCATION.X;
                        rect.Y = INI.SIDE6LOCATION.Y - INI.SIDE5LOCATION.Y;
                        break;
                    case SideEnum.SIDE6:
                        //rect.X = INI.SIDE4LOCATION.X - INI.SIDE3LOCATION.X;
                        //rect.Y = INI.SIDE4LOCATION.Y - INI.SIDE3LOCATION.Y;
                        break;
                    default:
                        return new Rectangle();
                }
                //rect.Intersect(JzTools.Resize(RECIPEDB.rectRecipeRange, VirtualRatio));//Gaara by mask

                rect.X = Math.Max(rect.X - rectOriginViewRange.X, 0);
                rect.Y = Math.Max(rect.Y - rectOriginViewRange.Y, 0) - 1000;

                rect.Height = rect.Height + 2000;

                return rect;
            }
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

        public OPScreenUIKeyBaseClass(OPScreenUIControl rOPScreenUI, int DefaultRatio, int rMinRatio, int rMaxRatio)
            : base(rOPScreenUI, DefaultRatio, rMinRatio, rMaxRatio)
        {

        }

        protected override void picOperation_MouseUp(object sender, MouseEventArgs e)
        {
            base.picOperation_MouseUp(sender, e);
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (IsAnalyze)
                        return;

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
                    break;
            }
        }
        protected override void picOperation_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            base.picOperation_MouseDown(sender, e);

            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (IsAnalyze)
                        return;

                    PtLive = PtStart;

                    JzTools.SetSize(ref szInsideMove, PtStart, PtLive, true);

                    #region Hat Operation

                    int i = 0;
                    CornerEnum iCorner = CornerEnum.NONE;
                    KeybaseClass keybasetmp;
                    bool iInside = false;
                    bool iRepeat = false;
                    bool iSelected = false;

                    //先檢測是否在原有的地方點的
                    foreach (KeybaseClass keybase in SIDE.vKEYBASELIST)
                    {
                        iCorner = keybase.IsInsideCorner(ptLiveLocationIndicatorWithVirtualRatio);
                        iInside = keybase.IsInside(ptLiveLocationIndicatorWithVirtualRatio);

                        if (iCorner != CornerEnum.NONE || iInside)
                        {
                            iSelected = true;
                            if (keybase.IsSelected)
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
                                    foreach (KeybaseClass keybase1 in SIDE.vKEYBASELIST)
                                    {
                                        keybase1.IsSelected = false;
                                        keybase1.IsSelectedStart = false;
                                    }

                                    keybase.IsSelectedStart = true;
                                    keybase.IsSelected = true;
                                }
                            }
                            break;
                        }
                        //else if (IsWaitForSelection)
                        //{
                        //    LastStartIndex = 0;

                        //    foreach (KeybaseClass keybase1 in SIDE.vKEYBASELIST)
                        //    {
                        //        if (keybase1.IsSelectedStart)
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
                            foreach (KeybaseClass keybase in SIDE.vKEYBASELIST)
                            {
                                keybase.IsSelected = false;
                                keybase.IsSelectedStart = false;
                            }
                        }
                        else
                        {
                            foreach (KeybaseClass keybase in SIDE.vKEYBASELIST)
                            {
                                keybase.IsSelectedStart = false;
                            }
                        }
                    }
                    else
                    {
                        if (IsWaitForCopy && !IsExam)
                        {
                            int i1 = 0, j1 = SIDE.vKEYBASELIST.Count;

                            OPType = OPTypeEnum.MOVE;

                            while (i1 < j1)
                            {
                                if (SIDE.vKEYBASELIST[i1].IsSelected)
                                {
                                    keybasetmp = SIDE.vKEYBASELIST[i1].CloneAdded(int.Parse(JzTools.GetLastString(SIDE.vKEYBASELIST[SIDE.vKEYBASELIST.Count - 1].Name, 3)), true, bmpBareOrigion);
                                    SIDE.vKEYBASELIST.Add(keybasetmp);

                                    SIDE.vKEYBASELIST[i1].IsSelectedStart = false;
                                    SIDE.vKEYBASELIST[i1].IsSelected = false;
                                }
                                i1++;
                            }

                            foreach (KeybaseClass keybase in SIDE.vKEYBASELIST)
                            {
                                keybase.BackupRect();
                            }

                            OnCopyHat();
                        }
                        else
                        {
                            iCorner = CornerEnum.NONE;
                            i = 0;

                            foreach (KeybaseClass keybase in SIDE.vKEYBASELIST)
                            {
                                keybase.BackupRect();

                                iCorner = keybase.IsInsideCorner(ptLiveLocationIndicatorWithVirtualRatio);

                                if (iCorner != CornerEnum.NONE)
                                {
                                    if (OPType == OPTypeEnum.NONE)
                                    {
                                        OPType = OPTypeEnum.RESIZE;
                                        OPCorner = iCorner;
                                        OPIndex = i;

                                        keybase.IsSelected = true;
                                        keybase.IsSelectedStart = true;
                                    }
                                    else
                                    {
                                        if (!IsWaitForSelection)
                                        {
                                            keybase.IsSelected = false;
                                            keybase.IsSelectedStart = false;
                                        }
                                    }
                                }
                                else if (keybase.IsInside(ptLiveLocationIndicatorWithVirtualRatio))
                                {
                                    if (OPType == OPTypeEnum.NONE)
                                    {
                                        OPType = OPTypeEnum.MOVE;
                                        OPIndex = i;

                                        keybase.IsSelected = true;
                                        keybase.IsSelectedStart = true;
                                    }
                                    else
                                    {
                                        if (!IsWaitForSelection)
                                        {
                                            keybase.IsSelected = false;
                                            keybase.IsSelectedStart = false;
                                        }
                                    }
                                }
                                else
                                {
                                    if (!iRepeat)
                                    {
                                        if (!IsWaitForSelection)
                                        {
                                            keybase.IsSelected = false;
                                        }
                                    }

                                    keybase.IsSelectedStart = false;

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
                    
                    if (IsAnalyze)
                        return;

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

            foreach (KeybaseClass keybass in SIDE.vKEYBASELIST)
            {
                if (RectToIntersect.IntersectsWith(keybass.myrect))
                {
                    if (!IsFoundFirst)
                    {
                        keybass.IsSelected = true;
                        keybass.IsSelectedStart = true;
                        IsFoundFirst = true;
                    }
                    else
                    {
                        keybass.IsSelected = true;
                        keybass.IsSelectedStart = false;
                    }

                    IsOneIntersected = true;
                }
                else
                {
                    if (!IsWaitForSelection)
                    {
                        keybass.IsSelectedStart = false;
                        keybass.IsSelected = false;
                    }
                }
            }

            //if (IsWaitForSelection)
            //{
            //    if (LastStartIndex < SIDE.vKEYBASELIST.Count)
            //    {

            //        if (!IsOneIntersected)
            //        {
            //            SIDE.vKEYBASELIST[LastStartIndex].IsSelectedStart = true;
            //        }
            //        else
            //        {
            //            SIDE.vKEYBASELIST[LastStartIndex].IsSelectedStart = false;
            //        }
            //    }
            //}
            DrawViewPortDomain();
            OnSelectHat();
        }
        void MoveAction()
        {
            if (IsExam || IsAnalyze)
                return;

            foreach (KeybaseClass keybase in SIDE.vKEYBASELIST)
            {
                if (keybase.IsSelected)
                {
                    keybase.MoveRect(JzTools.ShiftValue(szInsideMove.Width, -iViewPortRatio), JzTools.ShiftValue(szInsideMove.Height, -iViewPortRatio));
                }
            }

            DrawViewPortDomain();
            OnSelectHat();
        }
        void ResizeAction()
        {
            if (IsExam || IsAnalyze)
                return;

            foreach (KeybaseClass keybase in SIDE.vKEYBASELIST)
            {
                if (keybase.IsSelected)
                {
                    keybase.SizedRect(JzTools.ShiftValue(szInsideMove.Width, -iViewPortRatio), JzTools.ShiftValue(szInsideMove.Height, -iViewPortRatio), OPCorner);
                }
            }

            DrawViewPortDomain();
            OnSelectHat();
        }
        void RedrawKeycap()
        {
            foreach (KeybaseClass keybase in SIDE.vKEYBASELIST)
            {
                if (keybase.IsSelected)
                {
                    keybase.GetBMP(bmpBareOrigion);
                }
            }
        }

        public void Refresh()
        {
            CorpImageInViewPort(ref ptViewPortLocationLive);
        }

        void DrawInViewPortDomain(Rectangle vRect, Pen P, KeybaseClass keybase)
        {
            int i = 0;
            Rectangle rect = JzTools.Resize(vRect, iViewPortRatio);

            rect.X = rect.X - JzTools.ShiftValue(ptViewPortLocationLive.X, iViewPortRatio);
            rect.Y = rect.Y - JzTools.ShiftValue(ptViewPortLocationLive.Y, iViewPortRatio);

            OPScreenUI.DrawRect(rect, KeyboardNoLockPen);

            OPScreenUI.DrawRect(rect, P);

            Rectangle rectTmp = rect;
            rectTmp.Inflate(-5, -5);

            if (keybase.IsFromBase)
            {
                OPScreenUI.DrawRect(rectTmp, new Pen(Color.Purple,5));
            }
            else if (keybase.IsCalibration)
            {
                OPScreenUI.DrawRect(rectTmp, new Pen(Color.Blue, 5));
            }
            if (IsAnalyze || IsExam)
            {

            }
            else
            {
                i = 0;
                while (i < (int)CornerEnum.COUNT)
                {
                    if ((CornerEnum)i == CornerEnum.RB && (INI.ISUSEPLANE || INI.ISSPACEFLAT))
                    {
                        if (keybase.IsAutoLocation || keybase.IsSpaceFlat)
                        {
                            if (keybase.XPos != 0 && keybase.YPos != 0)
                            {
                                OPScreenUI.DrawRect(JzTools.CornerRect(rect, (CornerEnum)i, KeyAssignClass.CornerSize), new SolidBrush(Color.FromArgb(255, 128, 128)));
                                OPScreenUI.DrawRect(JzTools.CornerRect(rect, (CornerEnum)i, KeyAssignClass.CornerSize >> 1), new SolidBrush(Color.Blue));
                            }
                            else
                            {
                                OPScreenUI.DrawRect(JzTools.CornerRect(rect, (CornerEnum)i, KeyAssignClass.CornerSize), new SolidBrush(Color.FromArgb(255, 255, 250)));
                                OPScreenUI.DrawRect(JzTools.CornerRect(rect, (CornerEnum)i, KeyAssignClass.CornerSize >> 1), new SolidBrush(Color.Blue));
                            }
                        }
                        else if (keybase.XPos != 0 && keybase.YPos != 0)
                        {
                            OPScreenUI.DrawRect(JzTools.CornerRect(rect, (CornerEnum)i, KeyAssignClass.CornerSize), new SolidBrush(Color.FromArgb(255, 128, 128)));
                            OPScreenUI.DrawRect(JzTools.CornerRect(rect, (CornerEnum)i, KeyAssignClass.CornerSize >> 1), new SolidBrush(Color.Blue));
                        }
                        else
                            OPScreenUI.DrawRect(JzTools.CornerRect(rect, (CornerEnum)i, KeyAssignClass.CornerSize), new SolidBrush(P.Color));
                    }
                    else
                        OPScreenUI.DrawRect(JzTools.CornerRect(rect, (CornerEnum)i, KeyAssignClass.CornerSize), new SolidBrush(P.Color));

                    i++;
                }
            }
        }
        void DrawInViewPortDomain(Rectangle FromRect, Rectangle ToRect)
        {
            if (IsNoLine)
                return;

            int i = 0;
            Rectangle rectFrom = JzTools.Resize(FromRect, iViewPortRatio);
            Rectangle rectTo = JzTools.Resize(ToRect, iViewPortRatio);

            rectFrom.X = rectFrom.X - JzTools.ShiftValue(ptViewPortLocationLive.X, iViewPortRatio);
            rectFrom.Y = rectFrom.Y - JzTools.ShiftValue(ptViewPortLocationLive.Y, iViewPortRatio);

            rectTo.X = rectTo.X - JzTools.ShiftValue(ptViewPortLocationLive.X, iViewPortRatio);
            rectTo.Y = rectTo.Y - JzTools.ShiftValue(ptViewPortLocationLive.Y, iViewPortRatio);

            KeybasePen.DashStyle = DashStyle.Dot;


            OPScreenUI.DrawLine(JzTools.GetRectCenter(rectFrom), JzTools.GetRectCenter(rectTo), KeybasePen);
        }
        void DrawViewPortLine(Rectangle FromRect,Rectangle ToRect)
        {
            if (IsNoLine)
                return;

            int i = 0;

            Rectangle rectFrom = JzTools.Resize(FromRect, iViewPortRatio);
            Rectangle rectTo = JzTools.Resize(ToRect, iViewPortRatio);

            rectFrom.X = rectFrom.X - JzTools.ShiftValue(ptViewPortLocationLive.X, iViewPortRatio);
            rectFrom.Y = rectFrom.Y - JzTools.ShiftValue(ptViewPortLocationLive.Y, iViewPortRatio);

            rectTo.X = rectTo.X - JzTools.ShiftValue(ptViewPortLocationLive.X, iViewPortRatio);
            rectTo.Y = rectTo.Y - JzTools.ShiftValue(ptViewPortLocationLive.Y, iViewPortRatio);

            KeycapPen.DashStyle = DashStyle.Dash;


            OPScreenUI.DrawLine(JzTools.GetRectCenter(rectFrom), JzTools.GetRectCenter(rectTo), KeycapPen);
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

            int i = 0;
            Size szTmp = szViewPortLocationMove;

            ViewPortLocation.X = JzTools.BoundValue(ViewPortLocation.X, szOrigin.Width - szTmp.Width - ExtendOutLineLength, 0);
            ViewPortLocation.Y = JzTools.BoundValue(ViewPortLocation.Y, szOrigin.Height - szTmp.Height - ExtendOutLineLength, 0);

            rectOriginViewRange = new Rectangle(ViewPortLocation, szViewPortCorpSize);

            BonudRect(ref rectOriginViewRange, szOrigin);

            ViewPortLocation = rectOriginViewRange.Location;

            bmpTmp.Dispose();

            if (IsExam)
            {
                bmpTmp = (Bitmap)SIDE.bmpBackgroundOrigin.Clone();
            }
            else if (IsAnalyze)
            {
                bmpTmp = (Bitmap)SIDE.bmpAnalyzeOrigin.Clone();
            }
            else
            {
                bmpTmp = (Bitmap)SIDE.bmpBaseOrigin.Clone();
            }

            bmpViewPortCorp.Dispose();
            bmpViewPortCorp = (Bitmap)bmpTmp.Clone(rectOriginViewRange, PixelFormat.Format32bppArgb);

            if (IsExam)
            {
                Rectangle RectTmp = new Rectangle();
                Point BiasPoint = new Point();

                i = 0;

                while (i < SIDE.vKEYBASELIST.Count)
                {
                    if (SIDE.vKEYBASELIST[i].myrect.IntersectsWith(rectOriginViewRange))
                    {
                        RectTmp = SIDE.vKEYBASELIST[i].myrect;

                        BiasPoint = RectTmp.Location;

                        RectTmp.Intersect(rectOriginViewRange);

                        if (RectTmp.Width != 0 && RectTmp.Height != 0)
                        {
                            BiasPoint = JzTools.SubPoint(BiasPoint, RectTmp.Location);

                            RectTmp.X = RectTmp.X - rectOriginViewRange.X;
                            RectTmp.Y = RectTmp.Y - rectOriginViewRange.Y;

                            Substract.SetMask(bmpViewPortCorp, SIDE.vKEYBASELIST[i].bmpProcessed, RectTmp, BiasPoint);
                        }
                    }
                    i++;
                }
            }

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
            int i = 0, j = 0;

            OPScreenUI.DrawStart();

            foreach (KeybaseClass keybase in SIDE.vKEYBASELIST)
            {
                if (IsAnalyze || IsExam)
                {
                    if (IsAnalyze)
                    {
                        if (keybase.IsCalibration)
                            DrawInViewPortDomain(keybase.myrect, keybase.myPen, keybase);
                        else
                            DrawInViewPortDomain(JzTools.GetCenterBiasRect(keybase.myrect, keybase.FoundCenterBias, keybase.FoundAnalyzeCenter), keybase.myPen, keybase);
                    }
                    else
                    {
                        if (keybase.IsSelectedStart)
                            DrawInViewPortDomain(keybase.myrect, KeyboardSelectedVeryFirstPen, keybase);
                        else
                        {
                            if (keybase.IsSelected)
                                DrawInViewPortDomain(keybase.myrect, KeyboardSelectedPen, keybase);
                            else
                                DrawInViewPortDomain(keybase.myrect, keybase.myPen, keybase);
                        }
                    }
                }
                else
                {
                    if (keybase.IsSelectedStart)
                        DrawInViewPortDomain(keybase.myrect, KeyboardSelectedVeryFirstPen, keybase);
                    else
                    {
                        if (keybase.IsSelected)
                            DrawInViewPortDomain(keybase.myrect, KeyboardSelectedPen, keybase);
                        else
                            DrawInViewPortDomain(keybase.myrect, keybase.myPen, keybase);
                    }
                }
                i++;
            }

            if (!IsAnalyze)
            {
                i = 0;
                while (i < SIDE.vKEYBASELIST.Count)
                {
                    if (SIDE.vKEYBASELIST[i].IsFromBase)
                    {
                        j = 0;
                        while (j < SIDE.vKEYBASELIST.Count)
                        {
                            if (j == i)
                            {
                                j++;
                                continue;
                            }

                            if (SIDE.vKEYBASELIST[i].IsRealted(SIDE.vKEYBASELIST[j]))
                            {
                                DrawInViewPortDomain(SIDE.vKEYBASELIST[i].myrect, SIDE.vKEYBASELIST[j].myrect);
                            }
                            j++;
                        }
                    }

                    i++;
                }

                i = 0;
                while (i < SIDE.vKEYBASELIST.Count - 1)
                {
                    j = i + 1;
                    while (j < SIDE.vKEYBASELIST.Count)
                    {
                        if ((!SIDE.vKEYBASELIST[i].IsFromBase && !SIDE.vKEYBASELIST[i].IsCalibration) && (!SIDE.vKEYBASELIST[j].IsFromBase && !SIDE.vKEYBASELIST[j].IsCalibration))
                        {
                            if (SIDE.vKEYBASELIST[i].CornerDefinedList.Count > 0 && SIDE.vKEYBASELIST[j].CornerDefinedList.Count > 0)
                            {
                                if (SIDE.vKEYBASELIST[i].CornerDefinedList[0].AliasName == SIDE.vKEYBASELIST[j].CornerDefinedList[0].AliasName)
                                {
                                    //if (SIDE.vKEYBASELIST[i].CornerDefinedList[0].AliasName == "ESC" && SIDE.vKEYBASELIST[j].CornerDefinedList[0].AliasName == "ESC")
                                    //{

                                    //    SIDE.vKEYBASELIST[i].CornerDefinedList[0].AliasName = SIDE.vKEYBASELIST[i].CornerDefinedList[0].AliasName;
                                    //}


                                    if ((SIDE.vKEYBASELIST[i].CornerDefinedList[0].IndicateCornerEx == CornerExEnum.LT && SIDE.vKEYBASELIST[j].CornerDefinedList[0].IndicateCornerEx == CornerExEnum.RT)
                                        ||
                                        (SIDE.vKEYBASELIST[i].CornerDefinedList[0].IndicateCornerEx == CornerExEnum.RT && SIDE.vKEYBASELIST[j].CornerDefinedList[0].IndicateCornerEx == CornerExEnum.LT))
                                    {
                                        DrawViewPortLine(SIDE.vKEYBASELIST[i].myrect, SIDE.vKEYBASELIST[j].myrect);
                                    }
                                    else if ((SIDE.vKEYBASELIST[i].CornerDefinedList[0].IndicateCornerEx == CornerExEnum.LT && SIDE.vKEYBASELIST[j].CornerDefinedList[0].IndicateCornerEx == CornerExEnum.LB)
                                            ||
                                            (SIDE.vKEYBASELIST[i].CornerDefinedList[0].IndicateCornerEx == CornerExEnum.LB && SIDE.vKEYBASELIST[j].CornerDefinedList[0].IndicateCornerEx == CornerExEnum.LT))
                                    {
                                        DrawViewPortLine(SIDE.vKEYBASELIST[i].myrect, SIDE.vKEYBASELIST[j].myrect);
                                    }
                                    else if ((SIDE.vKEYBASELIST[i].CornerDefinedList[0].IndicateCornerEx == CornerExEnum.RT && SIDE.vKEYBASELIST[j].CornerDefinedList[0].IndicateCornerEx == CornerExEnum.RB)
                                            ||
                                            (SIDE.vKEYBASELIST[i].CornerDefinedList[0].IndicateCornerEx == CornerExEnum.RB && SIDE.vKEYBASELIST[j].CornerDefinedList[0].IndicateCornerEx == CornerExEnum.RT))
                                    {
                                        DrawViewPortLine(SIDE.vKEYBASELIST[i].myrect, SIDE.vKEYBASELIST[j].myrect);
                                    }
                                    else if ((SIDE.vKEYBASELIST[i].CornerDefinedList[0].IndicateCornerEx == CornerExEnum.LB && SIDE.vKEYBASELIST[j].CornerDefinedList[0].IndicateCornerEx == CornerExEnum.RB)
                                            ||
                                            (SIDE.vKEYBASELIST[i].CornerDefinedList[0].IndicateCornerEx == CornerExEnum.RB && SIDE.vKEYBASELIST[j].CornerDefinedList[0].IndicateCornerEx == CornerExEnum.LB))
                                    {
                                        DrawViewPortLine(SIDE.vKEYBASELIST[i].myrect, SIDE.vKEYBASELIST[j].myrect);
                                    }
                                }
                            }
                        }

                        j++;
                    }
                    i++;
                }


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

            KeycapPen.Dispose();
            KeybasePen.Dispose();

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
