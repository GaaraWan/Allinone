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
using Allinone.OPSpace;
using WorldOfMoveableObjects;
using MoveGraphLibrary;

namespace Allinone.FormSpace
{
    public partial class frmCorpBmp : Form
    {
        DispUI m_DispUI;
        Bitmap m_bitmap = new Bitmap(1, 1);
        Rectangle m_Rectangle = new Rectangle(0, 0, 3840, 2764);
        MoveGraphLibrary.Mover m_Mover = new MoveGraphLibrary.Mover();
        
        AlbumClass Album80001
        {
            get
            {
                return AlbumCollection.GetStaticAlbum(80001);
            }
        }
        AlbumCollectionClass AlbumCollection
        {
            get
            {
                return Universal.ALBCollection;
            }
        }

        public Rectangle GetResult()
        {
            return m_Rectangle;
        }

        public frmCorpBmp(Rectangle erectangle)
        {
            m_Rectangle = erectangle;

            InitializeComponent();

            this.Load += FrmCorpBmp_Load;
            this.SizeChanged += FrmCorpBmp_SizeChanged;
        }

        private void FrmCorpBmp_SizeChanged(object sender, EventArgs e)
        {
            update_Display();
        }

        private void FrmCorpBmp_Load(object sender, EventArgs e)
        {
            Init();
        }

        void Init()
        {
            init_Display();

            m_Mover.Clear();
            JzRectEAG jzRectEAG = new JzRectEAG(Color.FromArgb(0, 120, 120, 0), m_Rectangle);
            jzRectEAG.RelateLevel = 2;
            jzRectEAG.RelateNo = 1;

            m_Mover.Add(jzRectEAG);
            m_DispUI.SetMover(m_Mover);

            init_cboPages();

            btnSaveExit.Click += BtnSaveExit_Click;
            btnExit.Click += BtnExit_Click;

        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void BtnSaveExit_Click(object sender, EventArgs e)
        {
            GraphicalObject grob0 = m_Mover[0].Source;
            JzRectEAG jzRectEAG = (JzRectEAG)grob0;
            m_Rectangle = jzRectEAG.GetRect;

            this.DialogResult = DialogResult.OK;
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
        void init_cboPages()
        {
            cboPages.Items.Clear();
            string strmsg = "";
            int ienv = 0;
            int ipage = 0;

            if (Album80001 != null)
            {
                foreach (EnvClass env in Album80001.ENVList)
                {
                    foreach (PageClass page in env.PageList)
                    {
                        strmsg = "No-" + ienv.ToString() + "-" + ipage.ToString();
                        cboPages.Items.Add(strmsg);

                        ipage++;
                    }
                    ienv++;
                }
            }

            cboPages.SelectedIndexChanged += CboPages_SelectedIndexChanged;

            if (cboPages.Items.Count > 0)
                cboPages.SelectedIndex = 0;
        }

        private void CboPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strcbo = cboPages.Text;
            if (!string.IsNullOrEmpty(strcbo))
            {
                string[] strs = strcbo.Split('-');
                int ienv = int.Parse(strs[1]);
                int ipage = int.Parse(strs[2]);

                PageClass pageClass = Album80001.ENVList[ienv].PageList[ipage];
                m_bitmap.Dispose();
                m_bitmap = new Bitmap(pageClass.GetbmpORG());

                m_DispUI.SetDisplayImage(m_bitmap);
                m_DispUI.DefaultView();
            }
        }
    }
}
