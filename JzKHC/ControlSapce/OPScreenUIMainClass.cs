using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

using System.Windows.Forms;

using Jumbo301.UniversalSpace;
using Jumbo301.DBSpace;
using Jumbo301.ControlSpace;
using JetEazy.BasicSpace;

namespace Jumbo301.ControlSpace
{
    class OPScreenUIMainClass : OPScreenUIClass
    {
        CCDClass CCD
        {
            get
            {
                return Universal.CCD;
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

        int CCDVirtualRatio
        {
            get
            {
                return CCD.VirtaulRatio;
            }
        }
        
        Point ptMoveLocation = new Point();
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

                szTmp.Width = JzTools.BoundValue(ptLiveLocationIndicatorWithVirtualRatio.X - ptStartLocationIndicatorWithVirtualRatio.X, JzTools.ShiftValue(szOrigin.Width - 1,-VirtualRatio), 1);
                szTmp.Height = JzTools.BoundValue(ptLiveLocationIndicatorWithVirtualRatio.Y - ptStartLocationIndicatorWithVirtualRatio.Y, JzTools.ShiftValue(szOrigin.Height - 1,-VirtualRatio), 1);

                return szTmp;
            }
        }

        public bool IsShowNoVirtualTip = false;

        public bool IsShowKeyboardRangeRect = false;
        Pen KeyboardRangePen = new Pen(Color.Orange, 2);

        public bool IsShowKeyboardInformation = false;
        //public string KeyboardInformation = "";

        public OPScreenUIMainClass(OPScreenUIControl rOPScreenUI, int DefaultRatio, int rMinRatio, int rMaxRatio)
            : base(rOPScreenUI, DefaultRatio, rMinRatio, rMaxRatio)
        {
            VirtualRatio = CCDVirtualRatio;
            //RefreshbmpOrigin();
        }

        Rectangle rectLastKeyboardRange = new Rectangle();

        protected override void picOperation_MouseUp(object sender, MouseEventArgs e)
        {
            JzTools.ClearPoint(ref ptMoveLocation);

            switch (e.Button)
            {
                case MouseButtons.Left:
                    switch (OPType)
                    {
                        case OPTypeEnum.MOVETOASSEMBLE:
                            INI.SIDE1LOCATION = INI.SIDE1LOCATIONLIVE;
                            INI.SIDE2LOCATION = INI.SIDE2LOCATIONLIVE;
                            INI.SIDE3LOCATION = INI.SIDE3LOCATIONLIVE;
                            INI.SIDE4LOCATION = INI.SIDE4LOCATIONLIVE;
                            INI.SIDE5LOCATION = INI.SIDE5LOCATIONLIVE;
                            INI.SIDE6LOCATION = INI.SIDE6LOCATIONLIVE;
                            INI.SIDE7LOCATION = INI.SIDE7LOCATIONLIVE;
                            INI.SIDE8LOCATION = INI.SIDE8LOCATIONLIVE;
                            INI.SIDE9LOCATION = INI.SIDE9LOCATIONLIVE;
                            INI.SIDE10LOCATION = INI.SIDE10LOCATIONLIVE;

                            RefreshbmpOrigin();
                            break;
                        case OPTypeEnum.GETKEYBOARDRANGE:

                            //KEYBOARD.rectKeyboardRange = new Rectangle(ptStartLocationIndicatorWithVirtualRatio, szRealLocationMove);
                            RECIPEDB.rectRecipeRange = JzTools.RectTwoPoint(ptStartLocationIndicatorWithVirtualRatio, ptLiveLocationIndicatorWithVirtualRatio, JzTools.Resize(szOrigin, - VirtualRatio));
                            //KEYBOARD.SetKeyboardBMP(bmpBareOrigion, JzTools.Resize(RECIPEDB.rectRecipeRange, VirtualRatio));

                            if (RECIPEDB.rectRecipeRange.Width < 1000 || RECIPEDB.rectRecipeRange.Height < 200)
                            {
                                RECIPEDB.rectRecipeRange = rectLastKeyboardRange;
                            }

                            //KEYBOARD.bmpKeyboard.Save(@"D:\LOA\KBALL.BMP", ImageFormat.Bmp);

                            IsShowKeyboardRangeRect = true;
                            RefreshbmpOrigin();
                            OnGetKeyboardRange();
                            break;
                    }
                    break;
            }

            base.picOperation_MouseUp(sender, e);
        }
        protected override void picOperation_MouseDown(object sender, MouseEventArgs e)
        {
            base.picOperation_MouseDown(sender, e);

            switch (e.Button)
            {
                case MouseButtons.Left:
                    switch (OPType)
                    {
                        case OPTypeEnum.GETKEYBOARDRANGE:
                            rectLastKeyboardRange = RECIPEDB.rectRecipeRange;

                            IsShowKeyboardRangeRect = false;
                            CorpImageInViewPort(ref ptViewPortLocationLive);


                            break;
                    }
                    break;
            }
        }
        protected override void picOperation_MouseMove(object sender, MouseEventArgs e)
        {
            base.picOperation_MouseMove(sender, e);

            switch (e.Button)
            {
                case MouseButtons.Left:
                    switch (OPType)
                    {
                        case OPTypeEnum.MOVETOASSEMBLE:
                            if (tmMovingDelay.msDuriation > MovingDelay)
                            {
                                JzTools.SetSize(ref szInsideMove, PtStart, PtLive);

                                if (INI.IsSIDE1LOCATION)
                                {

                                    INI.SIDE1LOCATIONLIVE.X = JzTools.BoundValue(INI.SIDE1LOCATION.X - JzTools.StepValue(JzTools.ShiftValue(szLocationMove.Width, -CCD.VirtaulRatio), IsSlow), INI.CCDWIDTH - ExtendOutLineLength, INI.CCDWIDTH >> 1);
                                    INI.SIDE1LOCATIONLIVE.Y = JzTools.BoundValue(INI.SIDE1LOCATION.Y - JzTools.StepValue(JzTools.ShiftValue(szLocationMove.Height, -CCD.VirtaulRatio), IsSlow), INI.CCDHEIGHT >> 2, -(INI.CCDHEIGHT >> 2));

                                    INI.SetCCDLocation(SideEnum.SIDE1, INI.SIDE1LOCATIONLIVE);

                                }
                                if (INI.IsSIDE2LOCATION)
                                {
                                    INI.SIDE2LOCATIONLIVE.X = JzTools.BoundValue(INI.SIDE2LOCATION.X - JzTools.StepValue(JzTools.ShiftValue(szLocationMove.Width, -CCD.VirtaulRatio), IsSlow), (INI.CCDWIDTH << 1) - ExtendOutLineLength, (INI.CCDWIDTH + (INI.CCDWIDTH >> 1)));
                                    INI.SIDE2LOCATIONLIVE.Y = JzTools.BoundValue(INI.SIDE2LOCATION.Y - JzTools.StepValue(JzTools.ShiftValue(szLocationMove.Height, -CCD.VirtaulRatio), IsSlow), INI.CCDHEIGHT >> 2, -(INI.CCDHEIGHT >> 2));

                                    INI.SetCCDLocation(SideEnum.SIDE2, INI.SIDE2LOCATIONLIVE);
                                }
                                if (INI.IsSIDE3LOCATION)
                                {
                                    INI.SIDE3LOCATIONLIVE.X = JzTools.BoundValue(INI.SIDE3LOCATION.X - JzTools.StepValue(JzTools.ShiftValue(szLocationMove.Width, -CCD.VirtaulRatio), IsSlow), (INI.CCDWIDTH * 3) - ExtendOutLineLength, ((INI.CCDWIDTH << 1) + (INI.CCDWIDTH >> 1)));
                                    INI.SIDE3LOCATIONLIVE.Y = JzTools.BoundValue(INI.SIDE3LOCATION.Y - JzTools.StepValue(JzTools.ShiftValue(szLocationMove.Height, -CCD.VirtaulRatio), IsSlow), INI.CCDHEIGHT >> 2, -(INI.CCDHEIGHT >> 2));

                                    INI.SetCCDLocation(SideEnum.SIDE3, INI.SIDE3LOCATIONLIVE);
                                }
                                if (INI.IsSIDE4LOCATION)
                                {
                                    INI.SIDE4LOCATIONLIVE.X = JzTools.BoundValue(INI.SIDE4LOCATION.X - JzTools.StepValue(JzTools.ShiftValue(szLocationMove.Width, -CCD.VirtaulRatio), IsSlow), (INI.CCDWIDTH * 4) - ExtendOutLineLength, ((INI.CCDWIDTH << 1) + (INI.CCDWIDTH >> 1)));
                                    INI.SIDE4LOCATIONLIVE.Y = JzTools.BoundValue(INI.SIDE4LOCATION.Y - JzTools.StepValue(JzTools.ShiftValue(szLocationMove.Height, -CCD.VirtaulRatio), IsSlow), INI.CCDHEIGHT >> 2, -(INI.CCDHEIGHT >> 2));

                                    INI.SetCCDLocation(SideEnum.SIDE4, INI.SIDE4LOCATIONLIVE);
                                } 
                                if (INI.IsSIDE5LOCATION)
                                {
                                    INI.SIDE5LOCATIONLIVE.X = JzTools.BoundValue(INI.SIDE5LOCATION.X - JzTools.StepValue(JzTools.ShiftValue(szLocationMove.Width, -CCD.VirtaulRatio), IsSlow), (INI.CCDWIDTH * 5) - ExtendOutLineLength, ((INI.CCDWIDTH << 1) + (INI.CCDWIDTH >> 1)));
                                    INI.SIDE5LOCATIONLIVE.Y = JzTools.BoundValue(INI.SIDE5LOCATION.Y - JzTools.StepValue(JzTools.ShiftValue(szLocationMove.Height, -CCD.VirtaulRatio), IsSlow), INI.CCDHEIGHT >> 2, -(INI.CCDHEIGHT >> 2));

                                    INI.SetCCDLocation(SideEnum.SIDE5, INI.SIDE5LOCATIONLIVE);
                                } 
                                if (INI.IsSIDE6LOCATION)
                                {
                                    INI.SIDE6LOCATIONLIVE.X = JzTools.BoundValue(INI.SIDE6LOCATION.X - JzTools.StepValue(JzTools.ShiftValue(szLocationMove.Width, -CCD.VirtaulRatio), IsSlow), (INI.CCDWIDTH * 6) - ExtendOutLineLength, ((INI.CCDWIDTH << 1) + (INI.CCDWIDTH >> 1)));
                                    INI.SIDE6LOCATIONLIVE.Y = JzTools.BoundValue(INI.SIDE6LOCATION.Y - JzTools.StepValue(JzTools.ShiftValue(szLocationMove.Height, -CCD.VirtaulRatio), IsSlow), INI.CCDHEIGHT >> 2, -(INI.CCDHEIGHT >> 2));

                                    INI.SetCCDLocation(SideEnum.SIDE6, INI.SIDE6LOCATIONLIVE);
                                }
                                if (INI.IsSIDE7LOCATION)
                                {
                                    INI.SIDE7LOCATIONLIVE.X = JzTools.BoundValue(INI.SIDE7LOCATION.X - JzTools.StepValue(JzTools.ShiftValue(szLocationMove.Width, -CCD.VirtaulRatio), IsSlow), (INI.CCDWIDTH * 7) - ExtendOutLineLength, ((INI.CCDWIDTH << 1) + (INI.CCDWIDTH >> 1)));
                                    INI.SIDE7LOCATIONLIVE.Y = JzTools.BoundValue(INI.SIDE7LOCATION.Y - JzTools.StepValue(JzTools.ShiftValue(szLocationMove.Height, -CCD.VirtaulRatio), IsSlow), INI.CCDHEIGHT >> 2, -(INI.CCDHEIGHT >> 2));

                                    INI.SetCCDLocation(SideEnum.SIDE7, INI.SIDE7LOCATIONLIVE);
                                }
                                if (INI.IsSIDE8LOCATION)
                                {
                                    INI.SIDE8LOCATIONLIVE.X = JzTools.BoundValue(INI.SIDE8LOCATION.X - JzTools.StepValue(JzTools.ShiftValue(szLocationMove.Width, -CCD.VirtaulRatio), IsSlow), (INI.CCDWIDTH *8) - ExtendOutLineLength, ((INI.CCDWIDTH << 1) + (INI.CCDWIDTH >> 1)));
                                    INI.SIDE8LOCATIONLIVE.Y = JzTools.BoundValue(INI.SIDE8LOCATION.Y - JzTools.StepValue(JzTools.ShiftValue(szLocationMove.Height, -CCD.VirtaulRatio), IsSlow), INI.CCDHEIGHT >> 2, -(INI.CCDHEIGHT >> 2));

                                    INI.SetCCDLocation(SideEnum.SIDE8, INI.SIDE8LOCATIONLIVE);
                                }
                                //if (INI.IsSIDE9LOCATION)
                                //{
                                //    INI.SIDE9LOCATIONLIVE.X = JzTools.BoundValue(INI.SIDE9LOCATION.X - JzTools.StepValue(JzTools.ShiftValue(szLocationMove.Width, -CCD.VirtaulRatio), IsSlow), (INI.CCDWIDTH * 9) - ExtendOutLineLength, ((INI.CCDWIDTH << 1) + (INI.CCDWIDTH >> 1)));
                                //    INI.SIDE9LOCATIONLIVE.Y = JzTools.BoundValue(INI.SIDE9LOCATION.Y - JzTools.StepValue(JzTools.ShiftValue(szLocationMove.Height, -CCD.VirtaulRatio), IsSlow), INI.CCDHEIGHT >> 2, -(INI.CCDHEIGHT >> 2));

                                //    INI.SetCCDLocation(SideEnum.SIDE9, INI.SIDE9LOCATIONLIVE);
                                //}
                                //if (INI.IsSIDE10LOCATION)
                                //{
                                //    INI.SIDE10LOCATIONLIVE.X = JzTools.BoundValue(INI.SIDE10LOCATION.X - JzTools.StepValue(JzTools.ShiftValue(szLocationMove.Width, -CCD.VirtaulRatio), IsSlow), (INI.CCDWIDTH * 10) - ExtendOutLineLength, ((INI.CCDWIDTH << 1) + (INI.CCDWIDTH >> 1)));
                                //    INI.SIDE10LOCATIONLIVE.Y = JzTools.BoundValue(INI.SIDE10LOCATION.Y - JzTools.StepValue(JzTools.ShiftValue(szLocationMove.Height, -CCD.VirtaulRatio), IsSlow), INI.CCDHEIGHT >> 2, -(INI.CCDHEIGHT >> 2));

                                //    INI.SetCCDLocation(SideEnum.SIDE10, INI.SIDE10LOCATIONLIVE);
                                //}
                                
                                RefreshbmpOrigin();

                                tmMovingDelay.Cut();
                            }
                            break;
                        case OPTypeEnum.GETKEYBOARDRANGE:

                            //JzTools.SetSize(ref szInsideMove, PtStart,PtLive,true);

                            OPScreenUI.DrawStart();
                            OPScreenUI.DrawRect(JzTools.RectTwoPoint(PtStart,PtLive,szViewPort), KeyboardRangePen);
                            OPScreenUI.DrawEnd();
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
                    switch (OPType)
                    {
                        case OPTypeEnum.MOVETOASSEMBLE:
                            
                            JzTools.ClearSize(ref szInsideMove);

                            if (INI.IsSIDE1LOCATION)
                            {
                                INI.SIDE1LOCATION.Y = 0;
                                INI.SIDE1LOCATIONLIVE.Y = 0;
                                INI.SetCCDLocation(SideEnum.SIDE1, INI.SIDE1LOCATION);

                            }
                            if (INI.IsSIDE2LOCATION)
                            {
                                INI.SIDE2LOCATION.Y = 0;
                                INI.SIDE2LOCATIONLIVE.Y = 0;
                                INI.SetCCDLocation(SideEnum.SIDE2, INI.SIDE2LOCATION);
                            }
                            if (INI.IsSIDE3LOCATION)
                            {
                                INI.SIDE3LOCATION.Y = 0;
                                INI.SIDE3LOCATIONLIVE.Y = 0;
                                INI.SetCCDLocation(SideEnum.SIDE3, INI.SIDE3LOCATION);
                            }
                            if (INI.IsSIDE4LOCATION)
                            {
                                INI.SIDE4LOCATION.Y = 0;
                                INI.SIDE4LOCATIONLIVE.Y = 0;
                                INI.SetCCDLocation(SideEnum.SIDE4, INI.SIDE4LOCATION);

                            }
                            if (INI.IsSIDE5LOCATION)
                            {
                                INI.SIDE5LOCATION.Y = 0;
                                INI.SIDE5LOCATIONLIVE.Y = 0;
                                INI.SetCCDLocation(SideEnum.SIDE5, INI.SIDE5LOCATION);
                            }
                            if (INI.IsSIDE6LOCATION)
                            {
                                INI.SIDE6LOCATION.Y = 0;
                                INI.SIDE6LOCATIONLIVE.Y = 0;
                                INI.SetCCDLocation(SideEnum.SIDE6, INI.SIDE6LOCATION);
                            }
                            if (INI.IsSIDE7LOCATION)
                            {
                                INI.SIDE7LOCATION.Y = 0;
                                INI.SIDE7LOCATIONLIVE.Y = 0;
                                INI.SetCCDLocation(SideEnum.SIDE7, INI.SIDE7LOCATION);
                            }
                            if (INI.IsSIDE8LOCATION)
                            {
                                INI.SIDE8LOCATION.Y = 0;
                                INI.SIDE8LOCATIONLIVE.Y = 0;
                                INI.SetCCDLocation(SideEnum.SIDE8, INI.SIDE8LOCATION);
                            }
                            //if (INI.IsSIDE9LOCATION)
                            //{
                            //    INI.SIDE9LOCATION.Y = 0;
                            //    INI.SIDE9LOCATIONLIVE.Y = 0;
                            //    INI.SetCCDLocation(SideEnum.SIDE9, INI.SIDE9LOCATION);
                            //}
                            //if (INI.IsSIDE10LOCATION)
                            //{
                            //    INI.SIDE10LOCATION.Y = 0;
                            //    INI.SIDE10LOCATIONLIVE.Y = 0;
                            //    INI.SetCCDLocation(SideEnum.SIDE10, INI.SIDE10LOCATION);
                            //}

                            RefreshbmpOrigin();
                            break;
                    }
                    break;
            }
        }

        protected override void SetInformation(MouseEventArgs e, Point ptLocation)
        {
            Color COLOR = bmpBareOrigion.GetPixel(
                JzTools.ShiftValue(ptLiveLocationIndicatorWithVirtualRatio.X,CCDVirtualRatio),
                JzTools.ShiftValue(ptLiveLocationIndicatorWithVirtualRatio.Y, CCDVirtualRatio));


            OPScreenUI.SetInformation(" 內部座標:" + JzTools.PointtoString(e) + " 景觀座標:" + JzTools.PointtoString(ptLocation) + " " +
                                        "原始座標:" + JzTools.PointtoString(ptLiveLocationIndicatorWithVirtualRatio) + " " +
                                        //"KeyboardRange:" + JzTools.RecttoString(KEYBOARD.rectKeyboardRange) + "; " +
                                        //"End Point:" + JzTools.PointtoString(PtLive) + "; " +
                                        //"MaxRatio:" + MaxRatio.ToString() + "; " +
                                        //"MinRatio:" + MinRatio.ToString() + "; " +
                                        //"Ratio:" + iViewPortRatio.ToString() + "; " +
                                        //"R" + bmp
                                        JzTools.ColorInformation(COLOR) + "");
                                        //"szInsideMove:" + JzTools.SizetoString(szInsideMove));
        }
        public void RefreshbmpOrigin()
        {
            //CCD.GetAllBMP();
            //SetbmpOrigion(CCD.bmpAllBMP);

            //CorpImageInViewPort(ref ptViewPortLocationLive);
            RefreshbmpOrigin(false);
        }
        public void RefreshbmpOrigin(bool IsSudden)
        {
            CCD.GetAllBMP(IsSudden);
            SetbmpOrigion(CCD.bmpAllBMP);

            CorpImageInViewPort(ref ptViewPortLocationLive);
        }
        public void RefreshbmpOriginEX()
        {
            CCD.GetAllBMPEX();
            SetbmpOrigion(CCD.bmpAllBMP);

            CorpImageInViewPort(ref ptViewPortLocationLive);
        }
        protected override void CorpImageInViewPort(ref Point ViewPortLocation)
        {
            Size szTmp = szViewPortLocationMove;

            ViewPortLocation.X = JzTools.BoundValue(ViewPortLocation.X, szOrigin.Width - szTmp.Width - ExtendOutLineLength, 0);
            ViewPortLocation.Y = JzTools.BoundValue(ViewPortLocation.Y, szOrigin.Height - szTmp.Height - ExtendOutLineLength, 0);

            rectOriginViewRange = new Rectangle(ViewPortLocation, szViewPortCorpSize);

            JzTools.BonudRect(ref rectOriginViewRange, szOrigin);

            ViewPortLocation = rectOriginViewRange.Location;

            bmpViewPortCorp.Dispose();
            bmpViewPortCorp = (Bitmap)bmpOperate.Clone(rectOriginViewRange, PixelFormat.Format32bppArgb);

            bmpViewPortCorpZoom.Dispose();
            bmpViewPortCorpZoom = new Bitmap(bmpViewPortCorp, JzTools.Resize(rectOriginViewRange.Size, (iViewPortRatio - VirtualRatio)));


            if (IsShowNoVirtualTip)
                SetTip(bmpViewPortCorpZoom, "詳細檢視");

            if (IsShowKeyboardInformation)
                SetTip(bmpViewPortCorpZoom, RECIPEDB.NAME + "(" + RECIPEDB.VERSION + ")", 20, Brushes.Yellow);

            if (IsShowKeyboardRangeRect)
            {
                Rectangle rect = rectOriginViewRange;

                rect.Intersect(JzTools.Resize(RECIPEDB.rectRecipeRange, VirtualRatio));

                rect.X = Math.Max(rect.X - rectOriginViewRange.X, 0);
                rect.Y = Math.Max(rect.Y - rectOriginViewRange.Y, 0);

                //JzTools.DrawRect(bmpViewPortCorpZoom, JzTools.Resize(rect, (iViewPortRatio - VirtualRatio)), KeyboardRangePen);
            }

            OPScreenUI.SetPicture(bmpViewPortCorpZoom);
        }

        public delegate void GetKeyboardRangeHandler();
        public event GetKeyboardRangeHandler GetKeyboardRangeAction;
        public void OnGetKeyboardRange()
        {
            if (GetKeyboardRangeAction != null)
            {
                GetKeyboardRangeAction();
            }
        }


    }
}
