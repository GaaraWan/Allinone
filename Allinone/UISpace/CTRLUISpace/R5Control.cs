using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Allinone.UISpace
{
    public partial class R5Control : UserControl
    {
        Label lblName;
        Label lblShow;
        NumericUpDown numValue;
        Button btnSet;
        Button btnGo;
        Button btnGetNow;
        R5ControlParameter mPAR;
        JetEazy.ControlSpace.FatekPLCClass mPLC
        {
            get
            {
                return Universal.MACHINECollection.MACHINE.PLCCollection[0];
            }
        }
        public R5Control()
        {
            InitializeComponent();
            lblName = label1;
            lblShow = label3;
            numValue = numericUpDown1;
            btnSet = button1;
            btnGetNow = button3;
            btnGo = button2;

            btnSet.Click += BtnSet_Click;
            btnGo.Click += BtnGo_Click;
            btnGetNow.Click += BtnGetNow_Click;
        }

        private void BtnGetNow_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show( "你真的要把当前轴位置设定到"+ lblName.Text +"吗?", "SYS", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                int idata = mPLC.IOData.GetData(mPAR.strMotoNowIO, 2);
                mPLC.SetData(mPAR.strIO, idata, 2);
            }
        }

        private void BtnGo_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(lblName.Text + " 你真的要跑到这个位置吗?", "SYS", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                int idata = mPLC.IOData.GetData(mPAR.strIO, 2);

                mPLC.SetData(mPAR.strMotoLocaIO, idata, 2);
                mPLC.SetIO(true, mPAR.strMotoGOIO);
            }
        }

        public void Initial(R5ControlParameter mpar)
        {
            mPAR = mpar;
            lblName.Text = mpar.strName;
        }
        public void Tick()
        {
           int idata= mPLC.IOData.GetData(mPAR.strIO, 2);
            lblShow.Text = idata.ToString();

        }

        private void BtnSet_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(lblName.Text + " 真的要设定这个值吗?", "SYS", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                int iData = (int)numValue.Value;

                mPLC.SetData(mPAR.strIO, iData, 2);
                numValue.Value = 0;
            }
        }
    }

    public class R5ControlParameter
    {
        /// <summary>
        /// 位置名称
        /// </summary>
        public string strName = "";
        /// <summary>
        /// 保存的位置点
        /// </summary>
        public string strIO = "";
        /// <summary>
        /// 轴的当前位置
        /// </summary>
        public string strMotoNowIO;
        /// <summary>
        /// 轴的定位位置(ABS)
        /// </summary>
        public string strMotoLocaIO;
        /// <summary>
        /// 轴启动点
        /// </summary>
        public string strMotoGOIO;
        
    }
}
