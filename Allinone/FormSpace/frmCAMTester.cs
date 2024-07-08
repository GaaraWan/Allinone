using JetEazy.ControlSpace;
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
    public partial class frmCAMTester : Form
    {
        CCDCollectionClass CCDCollection;
        Button btnGetImage;
        DispUI m_DispUICAM0;


        public frmCAMTester()
        {
            InitializeComponent();

            this.Load += FrmCAMTester_Load;
            this.FormClosed += FrmCAMTester_FormClosed;
        }

        private void FrmCAMTester_FormClosed(object sender, FormClosedEventArgs e)
        {
            CCDDispose();
        }

        private void FrmCAMTester_Load(object sender, EventArgs e)
        {
            bool bOK = CCDInit();
            btnGetImage = button1;
            btnGetImage.Click += BtnGetImage_Click;

            init_Display();
            update_Display();
        }

        private void BtnGetImage_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            //CCDCollection.SetExposure(30, 0);
            CCDCollection.GetImage(0);
            Bitmap bitmap = CCDCollection.GetBMP(0, false);

            stopwatch.Stop();
            long ms = stopwatch.ElapsedMilliseconds;
            double fps = 1000.0 / ms;
            m_DispUICAM0.SetDisplayImage(bitmap);
            this.Text = "Fps:" + fps.ToString();

        }

        bool CCDInit()
        {
            bool ret = true;

            CCDCollection = new CCDCollectionClass(Universal.WORKPATH, Universal.IsNoUseCCD, Universal.VERSION, Universal.OPTION);

            ret = CCDCollection.Initial(Universal.WORKPATH);

            if (ret)
                CCDCollection.GetBmpAll(-2);

            return ret;
        }
        void CCDDispose()
        {
            if (CCDCollection != null)
            {
                CCDCollection.Close();
            }
        }
        void init_Display()
        {
            m_DispUICAM0 = dispUI1;
            m_DispUICAM0.Initial(100, 0.01f);
            m_DispUICAM0.SetDisplayType(DisplayTypeEnum.NORMAL);

            //m_DispUI.MoverAction += M_DispUI_MoverAction;
            //m_DispUI.AdjustAction += M_DispUI_AdjustAction;
        }
        void update_Display()
        {
            m_DispUICAM0.Refresh();
            m_DispUICAM0.DefaultView();
        }
    }
}
