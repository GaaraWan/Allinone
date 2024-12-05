using Allinone.BasicSpace;
using JetEazy.BasicSpace;
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
    public partial class frmMark : Form
    {
        string m_ParaStr = string.Empty;
        //Bitmap m_bmporg = new Bitmap(1, 1);
        //RectangleF m_rectorg = new RectangleF();
        public MarkParaPropertyGridClass markParaPropertyGridClass = new MarkParaPropertyGridClass();

        //Button btnSave;
        //Button btnCancel;
        //Button btnFind;

        public frmMark(string paraStr)
        {
            INI.ShowMarkFrm = true;
            //m_bmporg.Dispose();
            //m_bmporg = new Bitmap(bmpinput);

            //m_rectorg = eRectF;
            m_ParaStr = paraStr;
            InitializeComponent();

            this.Load += FrmMark_Load;
            this.Text = "Mark参数页面";
            this.TopMost = true;
            this.FormClosed += FrmMark_FormClosed;
            this.SizeChanged += FrmMark_SizeChanged;
        }

        private void FrmMark_SizeChanged(object sender, EventArgs e)
        {
            update_Display();
        }

        private void FrmMark_FormClosed(object sender, FormClosedEventArgs e)
        {
            INI.ShowMarkFrm = false;
        }

        public string ParaStr
        {
            get { return markParaPropertyGridClass.ToParaString(); }
        }
        public void SetImage(Bitmap ebmp)
        {
            DS.SetDisplayImage(ebmp);
        }
        private void FrmMark_Load(object sender, EventArgs e)
        {
            markParaPropertyGridClass.FromingStr(m_ParaStr);
            propertyGrid1.SelectedObject = markParaPropertyGridClass;


            init_Display();
            update_Display();
        }

        void init_Display()
        {
            DS.Initial(100, 0.01f);
            DS.SetDisplayType(JzDisplay.DisplayTypeEnum.NORMAL);
            //DS.DebugAction += DS_DebugAction;
        }

        void update_Display()
        {
            DS.Refresh();
            DS.DefaultView();
        }
    }
}
