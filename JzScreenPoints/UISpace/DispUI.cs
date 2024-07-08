using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using JzScreenPoints.BasicSpace;

namespace JzScreenPoints.UISpace
{
    public partial class DispUI : UserControl
    {
        Label lblInformation;
        public PictureBox picOperation;
        Bitmap myOPBMP = new Bitmap(1, 1);
        Bitmap bmpORIGIN = new Bitmap(1, 1);
        Bitmap bmpOPERATE = new Bitmap(1, 1);

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

        public DispUI()
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

            bmpORIGIN.Dispose();
            bmpORIGIN = (Bitmap)rmyBMP.Clone();

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
            bmpOPERATE.Dispose();
            bmpOPERATE = (Bitmap)bmpORIGIN.Clone();

            g = Graphics.FromImage(bmpOPERATE);
        }
        public void DrawRect(Rectangle rect, Pen P)
        {
            g.DrawRectangle(P, rect);
        }
        public void DrawText(string Str,PointF Pt)
        {
            Font font = new Font("Arial",16);
            SolidBrush sb = new SolidBrush(Color.Lime);

            g.DrawString(Str, font, sb,Pt);
        }
        public void DrawText(string Str, PointF Pt,int size)
        {
            Font font = new Font("Arial", size);
            SolidBrush sb = new SolidBrush(Color.Lime);

            g.DrawString(Str, font, sb, Pt);
        }
        public void DrawRect(Rectangle rect, SolidBrush B)
        {
            g.FillRectangle(B, rect);
        }
        public void DrawLine(Point PtFrom, Point PtTo,Pen DPen)
        {
            g.DrawLine(DPen, PtFrom, PtTo);
        }
        public void DrawCircle(Point Ptcirclecenter, int radius)
        {
            DrawCircle(Ptcirclecenter, radius, new Pen(Color.Pink, 3));
        }

        public void DrawCircle(Point Ptcirclecenter,int radius,Pen P)
        {
            if (Ptcirclecenter.X > 5000 || Ptcirclecenter.X < -5000)
            {
                Ptcirclecenter = new Point();
                radius = 1;
            }

            g.DrawEllipse(P, Ptcirclecenter.X - radius, Ptcirclecenter.Y - radius, radius << 1, radius << 1);
        }

        public void DrawCircle(Point Ptcirclecenter, int radius, SolidBrush sbrush)
        {
            if (Ptcirclecenter.X > 5000 || Ptcirclecenter.X < -5000)
            {
                Ptcirclecenter = new Point();
                radius = 1;
            }

            g.FillEllipse(sbrush, Ptcirclecenter.X - radius, Ptcirclecenter.Y - radius, radius << 1, radius << 1);
        }
        public void DrawImage(Bitmap bmp, Rectangle destrect)
        {
            g.DrawImage(bmp, destrect, JzTools.SimpleRect(bmp.Size), GraphicsUnit.Pixel);
        }

        public void DrawEnd()
        {
            g.Dispose();

            picOperation.Image = bmpOPERATE;
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
        public void Suicide()
        {
            myOPBMP.Dispose();
            bmpOPERATE.Dispose();
            bmpORIGIN.Dispose();
        }
    }
}
