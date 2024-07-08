using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using JzDisplay;
using JzDisplay.UISpace;
using JetEazy.BasicSpace;
using Allinone;
using Allinone.OPSpace;

namespace Allinone.UISpace
{
    public partial class StiltsUI : UserControl
    {
        DispUI DISPUI;

        Label lblPass;


        public StiltsUI()
        {
            InitializeComponent();
            InitialInside();
            Initial();
        }

        void InitialInside()
        {
            DISPUI = dispUI1;

            lblPass = label1;
            lblPass.Visible = false;

        }


        
        public void Initial()
        {
            DISPUI.Initial();
            DISPUI.SetDisplayType(DisplayTypeEnum.SHOW);

        }
         void SetImage(Bitmap bmp,bool isreplace)
        {
         
            if (isreplace)
                DISPUI.ReplaceDisplayImage(bmp);
            else
                DISPUI.SetDisplayImage(bmp);

            DISPUI.DefaultView();
        }
        public void SetStilts(AnalyzeClass analyze)
        {
            if (analyze.StiltsPara.StiltsMethod == STILTSMethodEnum.STILTS)
            {

               
                Bitmap bmp = new Bitmap(1, 1);
                Bitmap bmpRun = new Bitmap(1, 1);
                bool isok = analyze.CheckStilts(ref bmp,ref bmpRun);

                lblPass.Visible = true;

                string mess = "检测到 " + analyze.StiltsPara.StiltsLength + Environment.NewLine;
                mess += "参数为 " + analyze.StiltsPara.StiltsOffSet + Environment.NewLine;
                mess += (isok ? "PASS" : "NG");

                lblPass.Text = mess;
                lblPass.BackColor = (isok ? Color.Lime : Color.Red);

                int h = bmp.Height > bmpRun.Height ? bmp.Height : bmpRun.Height;
                Bitmap bmpDff = new Bitmap(bmp.Width + bmpRun.Width, h);
                Graphics gg = Graphics.FromImage(bmpDff);
                gg.DrawImage(bmp, new PointF(0, 0));
                gg.DrawImage(bmpRun, new PointF(bmp.Width, 0));
                gg.Dispose();
                bmp.Dispose();
                bmpRun.Dispose();
                SetImage(bmpDff, true);
            }

        }



    }
}
