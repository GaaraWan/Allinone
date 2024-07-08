using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using JetEazy.BasicSpace;
//using Jumbo301.UniversalSpace;

namespace JzKHC.ControlSpace
{
    public partial class OPScreenUIControl : UserControl
    {
        Label lblInformation;
        public PictureBox picOperation;
        Bitmap myOPBMP = new Bitmap(1, 1);
        Bitmap myOPBMPOrg = new Bitmap(1, 1);
        Bitmap myOPBMPDraw = new Bitmap(1, 1);
        protected JzToolsClass JzTools = new JzToolsClass();
        public Size picOperationSize
        {
            get
            {
                return new Size(this.Width, this.Height - lblInformation.Height);
            }
        }
        public Point picOriginBiasPoint
        {
            get
            {
                return new Point(this.Location.X + picOperation.Location.X, this.Location.Y + picOperation.Location.Y);
            }

        }

        public OPScreenUIControl()
        {
            InitializeComponent();
            Initial();
        }
        void Initial()
        {
            lblInformation = label1;
            picOperation = pictureBox1;
        }

        public void SetInformation(string Information)
        {
            lblInformation.Text = Information;
        }
        public void SetPicture(Bitmap rmyBMP)
        {
            myOPBMP.Dispose();
            myOPBMP = (Bitmap)rmyBMP.Clone();

            myOPBMPOrg.Dispose();
            myOPBMPOrg = (Bitmap)rmyBMP.Clone();

            //SetTip("HELLO WORLD");
            picOperation.Image = myOPBMP;
        }

        Graphics g;
        public void DrawStart()
        {
            //picOperation.Refresh();
            //g = picOperation.CreateGraphics();
            //myOPBMPDraw.Dispose();

            // No Shinning...
            myOPBMPDraw.Dispose();
            myOPBMPDraw = (Bitmap)myOPBMPOrg.Clone();

            g = Graphics.FromImage(myOPBMPDraw);
        }
        public void DrawRect(Rectangle rect, Pen P)
        {
            g.DrawRectangle(P, rect);
        }
        public void DrawText(string Str,PointF Pt)
        {
            Font font = new Font("Arial",16);
            SolidBrush sb = new SolidBrush(Color.Red);

            g.DrawString(Str, font, sb,Pt);
        }
        public void DrawRect(Rectangle rect, SolidBrush B)
        {
            g.FillRectangle(B, rect);
        }
        public void DrawLine(Point PtFrom, Point PtTo,Pen DPen)
        {
            g.DrawLine(DPen, PtFrom, PtTo);

        }
        public void DrawEnd()
        {
            g.Dispose();

            picOperation.Image = myOPBMPDraw;
            //Application.DoEvents();
        }
        public void DrawRect(Rectangle [] rect, Pen P)
        {
            picOperation.Refresh();
            JzTools.DrawRect(picOperation, rect, P);
        }
        protected override void OnLoad(EventArgs e)
        {
            lblInformation.Width = this.Width;
            picOperation.Width = this.Width;
            picOperation.Height = this.Height - lblInformation.Height;

            base.OnLoad(e);
        }
        public void DisposeBMP()
        {
            myOPBMP.Dispose();
            myOPBMPDraw.Dispose();
            myOPBMPOrg.Dispose();
        }
    }
}
