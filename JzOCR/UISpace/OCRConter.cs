using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JetEazy;
using JetEazy.UISpace;
using JetEazy.BasicSpace;
using JzOCR.OPSpace;

namespace JzOCR.UISpace
{
    public partial class OCRConter : UserControl
    {
        PictureBox picImageShow;
        TextBox tbValue;
        TextBox tbValue2;

        bool select = false;
        /// <summary>
        /// 参数是否被选中
        /// </summary>
        public bool isSelected
        {
            get
            {
                

                return select;
            }
            set
            {

                if (!value)
                {
                    BackColor = SystemColors.Control;
                    OCRT.isSelected = false;
                    select = false;
                }
                else
                {
                    select = true;
                    OCRT.isSelected = true;
                    BackColor = Color.Blue;
                }
            }
        }

        public OCRItemClass OCRT;


        public OCRItemClass OCRSet
        {
            get
            {
                return OCRT;
            }
            set
            {
                OCRT = value;
                picImageShow.Image = OCRT.bmpItem;
                tbValue.Text = OCRT.strRelateName;
                tbValue2.Text = OCRT.strRelateName2;
            }
        }
        public OCRConter()
        {
            InitializeComponent();

            picImageShow = pictureBox1;
            tbValue = textBox1;
            tbValue2 = textBox2;
            this.Load += OCRConter_Load;
            tbValue.LostFocus += TbValue_LostFocus;
            tbValue.GotFocus += TbValue_GotFocus;
            picImageShow.Click += PicImageShow_Click;
            tbValue2.TextChanged += TbValue2_TextChanged;
            this.Click += PicImageShow_Click;

            MouseEnter += OCRConter_MouseEnter;
            picImageShow.MouseEnter += OCRConter_MouseEnter;
            MouseLeave += OCRConter_MouseLeave;
            picImageShow.MouseLeave += OCRConter_MouseLeave;
        }

        private void OCRConter_MouseLeave(object sender, EventArgs e)
        {
            //if (OcrUI.isCtrlDeyDown)
                this.Cursor = System.Windows.Forms.Cursors.Default;
        }

        private void OCRConter_MouseEnter(object sender, EventArgs e)
        {
            if (OcrUI.isCtrlDeyDown)
                this.Cursor = System.Windows.Forms.Cursors.Cross;
        }

        private void TbValue2_TextChanged(object sender, EventArgs e)
        {
            OCRT.strRelateName2 = tbValue2.Text;
        }

        private void PicImageShow_Click(object sender, EventArgs e)
        {
         

           

            ParameterEventArgs ev = new ParameterEventArgs();
            ev.IsClick = true;
            ev.Message = OCRT.No.ToString();
            OnTitleChanged(ev);
        }

        private void TbValue_GotFocus(object sender, EventArgs e)
        {
            //isSelected = true;
            //OCRT.isSelected = true;
            //this.BackColor = Color.Blue;

            

            ParameterEventArgs ev = new ParameterEventArgs();
            ev.IsClick = true;
            ev.Message = OCRT.No.ToString();
            OnTitleChanged(ev);
        }

        private void TbValue_LostFocus(object sender, EventArgs e)
        {
           // this.BackColor = SystemColors.Control;
        }

        private void OCRConter_Load(object sender, EventArgs e)
        {
            tbValue.TextChanged += TbValue_TextChanged;
        }

        private void TbValue_TextChanged(object sender, EventArgs e)
        {
            OCRT.strRelateName = tbValue.Text;
        }


        /// <summary>
        /// 事件参数类
        /// </summary>
        public class ParameterEventArgs : EventArgs
        {
            public string Message { get; set; } = "";
            public bool IsClick { get; set; } = false;
        }
        // 声明委托
        public delegate void ParameterEventHandler(object sender, ParameterEventArgs e);
        // 定义事件
        public event ParameterEventHandler ONClick;
        // 触发事件的方法
        protected virtual void OnTitleChanged(ParameterEventArgs e)
        {
            ONClick?.Invoke(this, e);
        }

    }
}
