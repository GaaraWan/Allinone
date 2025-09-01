using JzDisplay;
using JzDisplay.UISpace;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Allinone.FormSpace
{
    public partial class frmShowPicture : Form
    {
        Bitmap m_bmp = new Bitmap(1, 1);
        DispUI m_DispUI;

        public void SetImage(Bitmap bmpinput)
        {
            m_bmp.Dispose();
            m_bmp=new Bitmap(bmpinput);
            //m_DispUI.ReplaceDisplayImage(bmpinput);
        }

        public frmShowPicture()
        {
            InitializeComponent();
            this.Load += FrmShowPicture_Load;
            this.SizeChanged += FrmShowPicture_SizeChanged;
        }

        private void FrmShowPicture_SizeChanged(object sender, EventArgs e)
        {
            update_Display();
        }

        private void FrmShowPicture_Load(object sender, EventArgs e)
        {
            init_Display();
            update_Display();

            this.Text = $"显示图片界面";

            m_DispUI.ReplaceDisplayImage(m_bmp);
        }

        void init_Display()
        {
            m_DispUI = dispUI1;
            m_DispUI.Initial();
            m_DispUI.SetDisplayType(DisplayTypeEnum.NORMAL);
        }
        void update_Display()
        {
            m_DispUI.Refresh();
            m_DispUI.DefaultView();
        }
    }
}
