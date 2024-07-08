using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JetEazy.UISpace
{
    public enum RegionEnum : int
    {
        COUNT=4,

        FEED_Z1=0,
        FEED_Z2,
        TAKE_PASS,
        TAKE_NG,
    }

    public partial class MainSDOpUI : UserControl
    {
        Label lblRegionUpper;
        Label lblRegionProductUpper;
        Label lblRegionProductIsHave;
        Label lblRegionProductCount;
        Label lblRegionUserFull;
        Label lblRegionSensorFull;
        Label lblRegionLower;

        Button btnLoad;
        Button btnUnLoad;
        Button btnClear;

        myVerticalProgressBar myVerticalProgress;

        RegionEnum m_Region = RegionEnum.TAKE_PASS;


        public MainSDOpUI()
        {
            InitializeComponent();
            InitUI();
        }
        void InitUI()
        {
            lblRegionUpper = label1;
            lblRegionProductUpper = label1;
            lblRegionProductIsHave = label1;
            lblRegionProductCount = label1;
            lblRegionUserFull = label1;
            lblRegionSensorFull = label1;
            lblRegionLower = label1;

            btnLoad = button1;
            btnUnLoad = button1;
            btnClear = button1;

            myVerticalProgress = myVerticalProgressBar1;
        }

        public void Init()
        {

        }

        public void Tick()
        {
            switch(m_Region)
            {
                case RegionEnum.TAKE_PASS:

                    lblRegionUpper = label1;
                    lblRegionProductUpper = label1;
                    lblRegionProductIsHave = label1;
                    lblRegionProductCount = label1;
                    lblRegionUserFull = label1;
                    lblRegionSensorFull = label1;
                    lblRegionLower = label1;

                    break;
            }
        }
    }
}
