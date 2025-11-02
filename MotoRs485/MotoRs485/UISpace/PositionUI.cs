using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MotoRs485.UISpace
{
    public partial class PositionUI : UserControl
    {
        MotoRs485.PositionFor Position;

        Button btnGo;
        Button btnSet;
        TextBox tbParPosition1;
        TextBox tbParPosition2;
        TextBox tbParPosition3;
        TextBox tbParPosition4;

        int MyIndex = 0;
        public PositionUI()
        {
            InitializeComponent();
        }

        public void Initial(int index, MotoRs485.PositionFor Pos)
        {
            Position = Pos;
            MyIndex = index;

            groupBox4.Text = "位置 " + MyIndex;

            btnGo = button1;
            btnSet = button2;
            tbParPosition1 = textBox1;
            tbParPosition2 = textBox2;
            tbParPosition3 = textBox3;
            tbParPosition4 = textBox4;

            tbParPosition1.Text = Position.iPosition_1.ToString();
            tbParPosition2.Text = Position.iPosition_2.ToString();
            tbParPosition3.Text = Position.iPosition_3.ToString();
            tbParPosition4.Text = Position.iPosition_4.ToString();

            btnGo.Click += BtnGo_Click;
            btnSet.Click += BtnSet_Click;
        }
        public void Updata(PositionFor Pos)
        {
            Position = Pos;
            tbParPosition1 = textBox1;
            tbParPosition2 = textBox2;
            tbParPosition3 = textBox3;
            tbParPosition4 = textBox4;

            tbParPosition1.Text = Position.iPosition_1.ToString();
            tbParPosition2.Text = Position.iPosition_2.ToString();
            tbParPosition3.Text = Position.iPosition_3.ToString();
            tbParPosition4.Text = Position.iPosition_4.ToString();
        }

        private void BtnSet_Click(object sender, EventArgs e)
        {
            OnTRIGGERMESSIGE(MyIndex, UIClick.Set);
        }

        private void BtnGo_Click(object sender, EventArgs e)
        {
            OnTRIGGERMESSIGE(MyIndex, UIClick.Go);
        }

        // 声明委托
        public delegate void MESSIGEEventHandler(object sender, int index, UIClick e);
        /// <summary>
        /// COM口发生的所有事件
        /// </summary>
        public event MESSIGEEventHandler TRIGGERMESSIGE;
        // 触发事件的方法
        protected virtual void OnTRIGGERMESSIGE(int index, UIClick e)
        {
            TRIGGERMESSIGE?.Invoke(this,index, e);
        }
    }

    public enum UIClick
    {
        Set,
        Go,
    }
}
