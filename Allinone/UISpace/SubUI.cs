using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using JzDisplay;
using JzDisplay.UISpace;
using JetEazy.BasicSpace;
using Allinone;
using Allinone.OPSpace;

namespace Allinone.UISpace
{
    public partial class SubUI : UserControl
    {
        DispUI DISPUI;

        GroupBox grpOperation;
        RadioButton rdoPattern;
        RadioButton rdoMask;
        RadioButton rdoOutput;

        TextBox txtLog;

        Label lblPass;

        public SubOperEnum SubOperateNow
        {
            get
            {
                SubOperEnum retoper = SubOperEnum.PATTERN;

                if (rdoMask.Checked)
                    retoper = SubOperEnum.MASK;

                if (rdoOutput.Checked)
                    retoper = SubOperEnum.OUTPUT;
                
                return retoper;
            }
        }

        public SubUI()
        {
            InitializeComponent();
            InitialInside();
        }

        void InitialInside()
        {
            DISPUI = dispUI1;

            grpOperation = groupBox1;

            rdoPattern = radioButton1;
            rdoMask = radioButton2;
            rdoOutput = radioButton3;

            txtLog = textBox1;
            lblPass = label1;
            lblPass.Visible = false;

            rdoPattern.Click += RdoPattern_Click;
            rdoMask.Click += RdoMask_Click;
            rdoOutput.Click += RdoOutput_Click;
        }


        
        public void Initial()
        {
            DISPUI.Initial();
            DISPUI.SetDisplayType(DisplayTypeEnum.SHOW);

            ClearImage();
        }
        public void SetImage(Bitmap bmp,bool isreplace)
        {
            grpOperation.Enabled = true;

            if (isreplace)
                DISPUI.ReplaceDisplayImage(bmp);
            else
                DISPUI.SetDisplayImage(bmp);
        }
        public void SetImage(AnalyzeClass analyze,bool isreplace)
        {
            switch(SubOperateNow)
            {
                case SubOperEnum.PATTERN:
                    SetImage(analyze.bmpWIP, isreplace);
                    break;
                case SubOperEnum.MASK:
                    analyze.CoverMask();
                    SetImage(analyze.bmpPATTERN,isreplace);
                    break;
                case SubOperEnum.OUTPUT:
                    SetImage(analyze.bmpOUTPUT, isreplace);
                    break;
            }

            lblPass.Visible = true;
            lblPass.Text = (analyze.IsZ05Good ? "PASS" : "NG");
            lblPass.BackColor = (analyze.IsZ05Good ? Color.Lime : Color.Red);

        }

        public void SetLog(string str)
        {
            txtLog.Text = str.Trim();

            //txtLog.Focus();
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();

        }

        public void ClearImage()
        {
            lblPass.Visible = false;

            grpOperation.Enabled = false;

            DISPUI.ClearAll();

            txtLog.Text = "";
        }
        private void RdoMask_Click(object sender, EventArgs e)
        {
            OnOperate(SubOperEnum.MASK);
        }
        private void RdoPattern_Click(object sender, EventArgs e)
        {
            OnOperate(SubOperEnum.PATTERN);
        }
        private void RdoOutput_Click(object sender, EventArgs e)
        {
            OnOperate(SubOperEnum.OUTPUT);
        }

        public void Suicide()
        {
            DISPUI.Suicide();
        }


        public delegate void OperateHandler(SubOperEnum oper);
        public event OperateHandler OperateAction;
        public void OnOperate(SubOperEnum oper)
        {
            if (OperateAction != null)
            {
                OperateAction(oper);
            }
        }
    }
}
