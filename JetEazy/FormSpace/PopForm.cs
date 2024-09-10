using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JetEazy.FormSpace
{
    public partial class PopForm : Form
    {

        [DllImport("user32")]
        private static extern bool AnimateWindow(IntPtr hwnd, int dwTime, int dwFlags);
        //下面是可用的常量,按照不合的动画结果声明本身须要的
        private const int AW_HOR_POSITIVE = 0x0001;//自左向右显示窗口,该标记可以在迁移转变动画和滑动动画中应用。应用AW_CENTER标记时忽视该标记
        private const int AW_HOR_NEGATIVE = 0x0002;//自右向左显示窗口,该标记可以在迁移转变动画和滑动动画中应用。应用AW_CENTER标记时忽视该标记
        private const int AW_VER_POSITIVE = 0x0004;//自顶向下显示窗口,该标记可以在迁移转变动画和滑动动画中应用。应用AW_CENTER标记时忽视该标记
        private const int AW_VER_NEGATIVE = 0x0008;//自下向上显示窗口,该标记可以在迁移转变动画和滑动动画中应用。应用AW_CENTER标记时忽视该标记该标记
        private const int AW_CENTER = 0x0010;//若应用了AW_HIDE标记,则使窗口向内重叠;不然向外扩大
        private const int AW_HIDE = 0x10000;//隐蔽窗口
        private const int AW_ACTIVE = 0x20000;//激活窗口,在应用了AW_HIDE标记后不要应用这个标记
        private const int AW_SLIDE = 0x40000;//应用滑动类型动画结果,默认为迁移转变动画类型,当应用AW_CENTER标记时,这个标记就被忽视
        private const int AW_BLEND = 0x80000;//应用淡入淡出结果

        Label lblMessage;
        Timer timer;

        int m_Durtime = 5;
        string m_Message = string.Empty;
        public PopForm(string eStrCmd, bool isok, int eDurtime = 15)
        {
            InitializeComponent();

            lblMessage = label1;
            lblMessage.Text = eStrCmd;

            m_Durtime = eDurtime;

            string[] vs = eStrCmd.Split(',');
            //if (vs.Length == 2)
            //{
            lblMessage.Text = eStrCmd + Environment.NewLine;
            //lblMessage.Text += (!isok ? "上传失败" : "上传成功");
            lblMessage.BackColor = (!isok ? Color.Red : Color.Green);
            //this.Text = (!isok ? "上传失败" : "上传成功");
            m_Message = eStrCmd;// (!isok ? "上传失败" : "上传成功");
            //}
            //else
            //{
            //    lblMessage.Text = eStrCmd;
            //}


            this.Load += PopForm_Load;
            this.FormClosing += PopForm_FormClosing;
        }

        private void PopForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            AnimateWindow(this.Handle, 200, AW_BLEND | AW_HIDE);
        }

        private void PopForm_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            timer = new Timer();
            timer.Interval = 1000;
            timer.Enabled = true;
            timer.Tick += Timer_Tick;

            //this.Text = "Hive上传返回信息";

            int x = (Screen.PrimaryScreen.WorkingArea.Right - this.Width) / 2;
            int y = 10;// Screen.PrimaryScreen.WorkingArea.Bottom - this.Height;
            this.Location = new Point(x, y);//设置窗体在屏幕右下角显示
            AnimateWindow(this.Handle, 500, AW_SLIDE | AW_ACTIVE | AW_VER_NEGATIVE);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (m_Durtime < 0)
            {
                this.timer.Enabled = false;
                this.Close();
            }
            this.Text = "信息显示：" + m_Message + "[" + m_Durtime.ToString() + " 秒]";
            m_Durtime--;

        }
    }
}
