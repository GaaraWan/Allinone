using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

using System.Windows.Forms;

using JetEazy.BasicSpace;
using JetEazy.UISpace;

namespace JetEazy.ControlSpace
{
    public class OPDisplayMain : OPDisplayNormal
    {   
        public OPDisplayMain(DispUI dispui, int defaultratio, int minratio, int maxratio)
            : base(dispui, defaultratio, minratio, maxratio)
        {


        }
        protected override void picOperation_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    switch (DISPOPType)
                    {
                        case DisplayOPTypeEnum.ADJUST:
                            OnMove(new Point(-10000, 0));
                            break;
                    }
                    break;
            }

            IsOperating = false;
            ptViewPortLocation = ptViewPortLocationLive;
            JzTools.ClearSize(ref szViewPortInsideMove);
            //DISPOPType = DisplayOPTypeEnum.NONE;

            OnZooming();

            //base.picOperation_MouseUp(sender, e);
        }
        protected override void picOperation_MouseMove(object sender, MouseEventArgs e)
        {
            base.picOperation_MouseMove(sender, e);

            switch (e.Button)
            {
                case MouseButtons.Left:
                    switch (DISPOPType)
                    {
                        case DisplayOPTypeEnum.ADJUST:
                            if (tmMovingDelay.msDuriation > MovingDelay)
                            {
                                OnMove(new Point(PtLive.X - PtStart.X, PtLive.Y - PtStart.Y));
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
                    switch (DISPOPType)
                    {
                        case DisplayOPTypeEnum.ADJUST:
                            OnMove(new Point(0, -10000));
                            break;
                    }
                    break;
            }
        }
        
        public delegate void TriggerHandler(Point diffpt);
        public event TriggerHandler MoveAction;
        public void OnMove(Point diffpt)
        {
            if (MoveAction != null)
            {
                MoveAction(diffpt);
            }
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
