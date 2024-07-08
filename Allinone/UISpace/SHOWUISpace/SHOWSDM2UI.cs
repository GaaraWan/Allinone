using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Allinone.UISpace.SHOWUISpace
{
    public partial class SHOWSDM2UI : UserControl
    {
        public SHOWSDM2UI()
        {
            InitializeComponent();
            //this.SizeChanged += SHOWSDM2UI_SizeChanged;
            //init_Display();
            //update_Display();
        }

        public void SetImage(Bitmap ebmpinput)
        {
            pictureBox1.Image = (Bitmap)ebmpinput.Clone();

            //DS.SetImode(-1);
            //DS.SetDisplayImage(ebmpinput);

            ////DS.BeginInvoke(new EventHandler(delegate
            ////{
            ////    DS.ReplaceDisplayImage(ebmpinput);

            ////}));
            //DS.SetImode(0);
        }

        private void SHOWSDM2UI_SizeChanged(object sender, EventArgs e)
        {
            update_Display();
        }

        void init_Display()
        {
            //DS.Width = (int)(tabControl1.TabPages[0].Width * 0.7);
            //DS.Height = (int)(tabControl1.TabPages[0].Height);
            //DS.Location = new Point(0, 0);

            //DS = dispUI1;
            DS.Initial(100, 0.01f);
            DS.SetDisplayType(JzDisplay.DisplayTypeEnum.SHOW);
            //DS.SetImode(-1);

            //m_DispUI.MoverAction += M_DispUI_MoverAction;
            //m_DispUI.AdjustAction += M_DispUI_AdjustAction;
        }
        void update_Display(bool eChangeToDefault = true)
        {
            //DS.Width = (int)(tabControl1.TabPages[0].Width * 0.7);
            //DS.Height = (int)(tabControl1.TabPages[0].Height);
            //DS.Location = new Point(0, 0);

            DS.Refresh();
            if (eChangeToDefault)
                DS.DefaultView();
        }
    }
}
