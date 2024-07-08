using System;
using System.Collections.Generic;
using System.ComponentModel;
//using System.Runtime.InteropServices;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using JetEazy.BasicSpace;

namespace Arges.FormSpace
{
    public partial class PatternForm : Form
    {
        //[DllImport("user32.dll")]
        //static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X,
        //   int Y, int cx, int cy, uint uFlags);

        //const UInt32 SWP_NOSIZE = 0x0001;
        //const UInt32 SWP_NOMOVE = 0x0002;
        //const UInt32 SWP_NOACTIVATE = 0x0010;

        //static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        Timer myTimer;

        public PatternForm(int width,int height,ref Bitmap bmp)
        {
            InitializeComponent();

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(1440, 0);
            this.Size = new Size(width, height);
            this.BackgroundImage = bmp;

            //pictureBox1.Location = new Point(0, 0);
            //pictureBox1.Size = new Size(width + 128, height);
            //pictureBox1.Image = bmp;

            //this.SetStyle(ControlStyles.
            //this.TopMost = true;
            //SetBottom();
        }
        public PatternForm(int width, int height, ref Bitmap bmp,int showtime)
        {
            InitializeComponent();

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(1440, 0);
            this.Size = new Size(width, height);
            this.BackgroundImage = bmp;

            //pictureBox1.Location = new Point(0, 0);
            //pictureBox1.Size = new Size(width + 128, height);
            //pictureBox1.Image = bmp;

            myTimer = new Timer();
            myTimer.Interval = showtime;
            myTimer.Tick += new EventHandler(myTimer_Tick);
            myTimer.Start();
            //this.TopMost = true;

            //SetBottom();
        }

        //public void SetBottom()
        //{
        //    SetWindowPos(this.Handle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
        //}

        void myTimer_Tick(object sender, EventArgs e)
        {
            this.Close();
        }

        public void SetImage(ref Bitmap bmp)
        {
            this.BackgroundImage = bmp;
        }

    }
}
