using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using JetEazy;
using JetEazy.BasicSpace;
using JetEazy.UISpace;

using Allinone.OPSpace;
using JetEazy.FormSpace;
using Allinone.FormSpace;
using iTextSharp.text.pdf;
using static System.Net.Mime.MediaTypeNames;
using JetEazy.PlugSpace;
using Allinone.BasicSpace;
using JetEazy.DBSpace;
using Allinone.ZGa.Mvc.Model.MapModel;

namespace Allinone.UISpace.RUNUISpace
{
    public partial class RunUI : UserControl
    {
        const int ShinningDuriation = 50;
        const int ShiningTimes = 2;

        bool IsResultPass = false;

        AccDBClass ACCDB
        {
            get
            {
                return Universal.ACCDB;
            }
        }

        //Language Setup

        Label lblPass;
        Label lblBigPass;

        Label lblDuriation;

        GroupBox grpShopFloor;
        GroupBox grpUserData;
        TextBox txtOp;
        TextBox txtResource;
        TextBox txtBarcode;

        CheckBox chkIsSaveRaw;
        CheckBox chkIsSaveNGRaw;
        CheckBox chkIsSaveDebug;
        CheckBox chkIsSaveOCR;
        Button btnChecked;

        Button btnTAKEPASSZero;
        Button btnTAKENGZero;

        Button btnReady;

        Panel pnlResult;

        RunMainX6JcetSFUI JCETSFUI;

        List<PictureBox> PRUNList = new List<PictureBox>();

        RptUI RPTUI;

        //Button btnDetail;
        //ParaForm PARAFrm;

        //Button btnReady;
        bool IsReady = false;

        VersionEnum VERSION
        {
            get
            {
                return Universal.VERSION;
            }

        }
        OptionEnum OPTION
        {
            get
            {
                return Universal.OPTION;
            }
        }
        AlbumClass AlbumNow
        {
            get
            {
                return Universal.ALBNow;
            }
        }

        JzTimes myTimes = new JzTimes();
        JzTimes myJudgeTimes = new JzTimes();

        public string BARCODE
        {
            get
            {
                return txtBarcode.Text;
            }
        }
        public string OPERATOR
        {
            get
            {
                return txtOp.Text;
            }
        }

        public bool IsSaveRaw
        {
            get
            {
                return chkIsSaveRaw.Checked;
            }
        }
        public bool IsSaveNGRaw
        {
            get
            {
                return chkIsSaveNGRaw.Checked;
            }
        }
        public bool IsSaveDebug
        {
            get
            {
                return chkIsSaveDebug.Checked;
            }
        }
        public bool IsSaveOCR
        {
            get
            {
                return chkIsSaveOCR.Checked;
            }
        }
        /// <summary>
        /// RUNUI上两个区块的 Enabled 是否可操作
        /// </summary>
        public bool GrpEnabled
        {
            set
            {
                grpShopFloor.Enabled = ACCDB.DataNow.AllowSetup;
                grpUserData.Enabled = ACCDB.DataNow.AllowSetup;
                RPTUI.Enabled = ACCDB.DataNow.AllowSetup;
                switch(Universal.OPTION)
                {
                    case OptionEnum.MAIN_SDM2:
                        btnReady.Enabled = ACCDB.DataNow.AllowSetup;

                        btnBypass.Enabled = ACCDB.DataNow.AllowUseShopFloor;
                        break;
                }
            }
        }

        public RunUI()
        {
            InitializeComponent();
        }

        #region MAIN SD MAPPING

        //ToolTip[] toolTipsItem;
        Label[] m_MappingItem;
        int m_Mapping_Col = 0;
        int m_Mapping_Row = 0;

        //左下角排序
        //public void QcRandomMappingInit(int eRow, int eCol)
        //{
        //    m_Mapping_Row = eRow;
        //    m_Mapping_Col = eCol;

        //    lblBigPass.Visible = true;
        //    if (!Universal.IsUseMappingUI)
        //    {
        //        lblBigPass.Text = "PASS";
        //        lblPass.Text = "PASS";

        //        lblBigPass.ForeColor = (true ? Color.Lime : Color.Red);
        //        lblPass.ForeColor = (true ? Color.Lime : Color.Red);

        //        return;
        //    }
        //    lblBigPass.Visible = false;

        //    #region 生成随机抽检的Mapping页面

        //    int iMappingCount = m_Mapping_Row * m_Mapping_Col;

        //    int iMappingItemWidth = ((pnlResult.Width - 10) - m_Mapping_Col * 3) / m_Mapping_Col;
        //    int iMappingItemHeight = iMappingItemWidth;
        //    iMappingItemHeight = ((pnlResult.Height - 10) - m_Mapping_Row * 3) / m_Mapping_Row;

        //    m_MappingItem = new Label[iMappingCount];

        //    this.pnlResult.Controls.Clear();
        //    int ix = 0;
        //    int iy = 0;

        //    //string colname = "A";
        //    int colindex = 0;

        //    int colnameindex = 0;
        //    int i = 0;
        //    i = 0;
        //    while (i < iMappingCount)
        //    {
        //        m_MappingItem[i] = new Label();

        //        m_MappingItem[i].Name = "lbl" + colindex.ToString() + "-" + colnameindex.ToString();
        //        m_MappingItem[i].Text = colindex.ToString() + "-" + colnameindex.ToString();

        //        m_MappingItem[i].AccessibleName = colindex.ToString() + "-" + colnameindex.ToString();

        //        m_MappingItem[i].BackColor = Color.Silver;
        //        m_MappingItem[i].Font = new System.Drawing.Font("黑体", 9F);
        //        m_MappingItem[i].TextAlign = ContentAlignment.MiddleCenter;
        //        m_MappingItem[i].Width = iMappingItemWidth;
        //        m_MappingItem[i].Height = iMappingItemHeight;
        //        m_MappingItem[i].Location = new Point(5 + ix, pnlResult.Height - 10 - 5 - iy - m_MappingItem[i].Height);
        //        m_MappingItem[i].DoubleClick += RunUI_DoubleClick;
        //        m_MappingItem[i].MouseEnter += RunUI_MouseEnter;
        //        ix += m_MappingItem[i].Width + 3;
        //        colnameindex++;
        //        if ((i + 1) % m_Mapping_Col == 0)
        //        {
        //            iy += m_MappingItem[i].Height + 3;
        //            ix = 0;

        //            colindex++;

        //            colnameindex = 0;

        //            //m_MappingItem[i].Text = (i + 1).ToString();
        //        }
        //        this.pnlResult.Controls.Add(m_MappingItem[i]);

        //        i++;
        //    }

        //    //foreach (Label lbl in m_MappingItem)
        //    //{
        //    //    lbl.BackColor = Color.Gray;
        //    //    lbl.Refresh();
        //    //}

        //    #endregion

        //}

        //左上角排序
        public void QcRandomMappingInit(int eRow, int eCol)
        {
            m_Mapping_Row = eRow;
            m_Mapping_Col = eCol;

            lblBigPass.Visible = true;
            if (!Universal.IsUseMappingUI)
            {
                lblBigPass.Text = "PASS";
                lblPass.Text = "PASS";

                lblBigPass.ForeColor = (true ? Color.Lime : Color.Red);
                lblPass.ForeColor = (true ? Color.Lime : Color.Red);

                return;
            }
            lblBigPass.Visible = false;

            #region 生成随机抽检的Mapping页面

            int iMappingCount = m_Mapping_Row * m_Mapping_Col;

            int iMappingItemWidth = ((pnlResult.Width - 10) - m_Mapping_Col * 3) / m_Mapping_Col;
            int iMappingItemHeight = iMappingItemWidth;
            iMappingItemHeight = ((pnlResult.Height - 10) - m_Mapping_Row * 3) / m_Mapping_Row;

            m_MappingItem = new Label[iMappingCount];

            this.pnlResult.Controls.Clear();
            int ix = 0;
            int iy = 0;

            //string colname = "A";
            int colindex = 0;

            int colnameindex = 0;
            int i = 0;
            i = 0;
            while (i < iMappingCount)
            {
                m_MappingItem[i] = new Label();

                m_MappingItem[i].Name = "lbl" + colindex.ToString() + "-" + colnameindex.ToString();
                m_MappingItem[i].Text = colindex.ToString() + "-" + colnameindex.ToString();

                m_MappingItem[i].AccessibleName = colindex.ToString() + "-" + colnameindex.ToString();

                m_MappingItem[i].BackColor = Color.Green;
                m_MappingItem[i].Font = new System.Drawing.Font("黑体", 9F);
                m_MappingItem[i].TextAlign = ContentAlignment.MiddleCenter;
                m_MappingItem[i].Width = iMappingItemWidth;
                m_MappingItem[i].Height = iMappingItemHeight;
                m_MappingItem[i].Location = new Point(5 + ix, 5 + iy);
                m_MappingItem[i].DoubleClick += RunUI_DoubleClick;
                m_MappingItem[i].MouseEnter += RunUI_MouseEnter;
                ix += m_MappingItem[i].Width + 3;
                colnameindex++;
                if ((i + 1) % m_Mapping_Col == 0)
                {
                    iy += m_MappingItem[i].Height + 3;
                    ix = 0;

                    colindex++;

                    colnameindex = 0;

                    //m_MappingItem[i].Text = (i + 1).ToString();
                }
                this.pnlResult.Controls.Add(m_MappingItem[i]);

                i++;
            }

            #endregion

        }
        public void QcRandomSetResult(string eCurrentPos, Color c, string txt = "")
        {
            foreach (Label lbl in m_MappingItem)
            {
                if (lbl.Text == eCurrentPos)
                {
                    lbl.BackColor = c;// (ePass ? Color.Green : Color.Red);
                    lbl.Tag = txt;
                    break;
                }
            }
        }

        public void QcMappingAInit(int eRow, int eCol, int eStepCount)
        {
            m_Mapping_Row = eRow;
            m_Mapping_Col = eCol;

            lblBigPass.Visible = true;
            if (!Universal.IsUseMappingUI)
            {
                lblBigPass.Text = "PASS";
                lblPass.Text = "PASS";

                lblBigPass.ForeColor = (true ? Color.Lime : Color.Red);
                lblPass.ForeColor = (true ? Color.Lime : Color.Red);

                return;
            }
            lblBigPass.Visible = false;

            #region 生成随机抽检的Mapping页面

            int iMappingCount = m_Mapping_Row * m_Mapping_Col;

            int iMappingItemWidth = ((pnlResult.Width - 10) - m_Mapping_Col * 3) / m_Mapping_Col;
            int iMappingItemHeight = iMappingItemWidth;
            iMappingItemHeight = ((pnlResult.Height - 10) - m_Mapping_Row * 3) / m_Mapping_Row;

            m_MappingItem = new Label[iMappingCount];

            this.pnlResult.Controls.Clear();
            int ix = 0;
            int iy = 0;

            //string colname = "A";
            int colindex = 1;

            int colnameindex = 1;
            int i = 0;
            i = 0;
            while (i < iMappingCount)
            {
                m_MappingItem[i] = new Label();

                m_MappingItem[i].Name = "lbl" + colindex.ToString() + "-" + colnameindex.ToString();
                m_MappingItem[i].Text = colindex.ToString() + "-" + colnameindex.ToString();

                m_MappingItem[i].AccessibleName = colindex.ToString() + "-" + colnameindex.ToString();

                m_MappingItem[i].BackColor = Color.Silver;
                m_MappingItem[i].Font = new System.Drawing.Font("黑体", 9F);
                m_MappingItem[i].TextAlign = ContentAlignment.MiddleCenter;
                m_MappingItem[i].Width = iMappingItemWidth;
                m_MappingItem[i].Height = iMappingItemHeight;
                //m_MappingItem[i].Location = new Point(5 + ix, pnlResult.Height - 10 - 5 - iy - m_MappingItem[i].Height);
                m_MappingItem[i].Location = new Point(5 + ix, 5 + iy);
                m_MappingItem[i].DoubleClick += RunUI_DoubleClick;
                m_MappingItem[i].MouseEnter += RunUI_MouseEnter;
                ix += m_MappingItem[i].Width + 3;
                colnameindex++;
                if ((i + 1) % m_Mapping_Col == 0)
                {
                    iy += m_MappingItem[i].Height + 3;
                    ix = 0;

                    colindex++;

                    colnameindex = 1;

                    //m_MappingItem[i].Text = (i + 1).ToString();
                }
                this.pnlResult.Controls.Add(m_MappingItem[i]);

                i++;
            }

            //foreach (Label lbl in m_MappingItem)
            //{
            //    lbl.BackColor = Color.Gray;
            //    lbl.Refresh();
            //}

            #endregion

            CamActClass.Instance.StepCurrent = 0;
            CamActClass.Instance.SetStepCount(eStepCount);

        }
        public void QcMappingAReset()
        {
            int i = 0;
            while (i < m_MappingItem.Length)
            {
                m_MappingItem[i].BackColor = Color.Silver;
                m_MappingItem[i].Tag = "";

                i++;
            }
        }
        public void QcMappingAUpdate(int[] eIntResult, string[] eTxt = null)
        {
            int i = 0;
            while (i < eIntResult.Length)
            {
                m_MappingItem[i].BackColor = _getIndexColor(eIntResult[i]);
                if (eTxt != null)
                    m_MappingItem[i].Tag = eTxt[i];

                i++;
            }
        }
        public void QcMappingAUpdate(List<JzSliderItemClass> mapList)
        {
            int i = 0;
            while (i < mapList.Count)
            {
                m_MappingItem[i].BackColor = _getIndexColor(mapList[i].IntResult);
                m_MappingItem[i].Tag = mapList[i].StrMessage;
                m_MappingItem[i].AccessibleName = $"{mapList[i].IntStepIndex},{mapList[i].AnalyzeOpeateStr}";
                i++;
            }
        }

        public void MappingInit()
        {
            lblBigPass.Visible = true;
            if (!Universal.IsUseMappingUI)
            {
                lblBigPass.Text = "PASS";
                lblPass.Text = "PASS";

                lblBigPass.ForeColor = (true ? Color.Lime : Color.Red);
                lblPass.ForeColor = (true ? Color.Lime : Color.Red);

                return;
            }

            switch(Universal.jetMappingType)
            {
                case JetMappingType.MAPPING_A:

                    AlbumClass album = AlbumNow;
                    if (album.ENVList.Count == 0)
                        return;
                    EnvClass env = album.ENVList[0];
                    ALBUISpace.AllinoneAlbUI.Light2Settings _light = new ALBUISpace.AllinoneAlbUI.Light2Settings();
                    _light.GetString(env.GeneralLight);
                    QcMappingAInit(_light.ChipRow, _light.ChipCol, env.StepCount);

                    return;
                    break;
            }

            switch (OPTION)
            {
                case OptionEnum.MAIN_X6:
                case OptionEnum.MAIN_SD:
                case OptionEnum.MAIN_SERVICE:
                    lblBigPass.Visible = false;

                    AlbumClass album = AlbumNow;
                    if (album.ENVList.Count == 0)
                        return;
                    EnvClass env = album.ENVList[0];

                    int _count = env.PageList.Count;
                    CamActClass.Instance.SetStepCount(_count);

                    int bmpwidth = env.PageList[0].GetbmpORG().Width;
                    List<AnalyzeClass> BranchList = new List<AnalyzeClass>();
                    BranchList.Clear();
                    string[] _genLight = env.GeneralLight.Split(',');
                    int _highestValue = 138;
                    if (_genLight.Length >= 8)
                        int.TryParse(_genLight[7], out _highestValue);

                    int MappingYOffset = 0;

                    foreach (PageClass page in env.PageList)
                    {
                        if (page.CamIndex >= Universal.CCDCollection.CCDRectRelateIndexList.Count)
                            MappingYOffset = 0;
                        else
                            MappingYOffset = Universal.CCDCollection.CCDRectRelateIndexList[page.CamIndex].SizedRect.Y;


                        foreach (AnalyzeClass analyze in page.AnalyzeRoot.BranchList)
                        {
                            //analyze.myOPRectF.X += page.GetbmpORG().Width * page.No;

                            if (analyze.ALIGNPara.AbsAlignMode == AbsoluteAlignEnum.MAIN
                                || analyze.ALIGNPara.AbsAlignMode == AbsoluteAlignEnum.MAIN_LEFT
                                || analyze.ALIGNPara.AbsAlignMode == AbsoluteAlignEnum.MAIN_RIGHT)
                                continue;

                            AnalyzeClass analyze1 = new AnalyzeClass();
                            analyze1.FromString(analyze.ToString());
                            analyze1.myOPRectF = new RectangleF(analyze.myOPRectF.X,
                                                                analyze.myOPRectF.Y + MappingYOffset,
                                                                analyze.myOPRectF.Width,
                                                                analyze.myOPRectF.Height);
                            BranchList.Add(analyze1);
                        }
                    }

                    foreach (AnalyzeClass analyze in BranchList)
                    {
                        analyze.myOPRectF.X += bmpwidth * analyze.PageNo;
                        //BranchList.Add(analyze);
                    }

                    int Highest = 100000;
                    int HighestIndex = -1;
                    int ReportIndex = 0;
                    List<string> CheckList = new List<string>();

                    int i = 0;

                    //Clear All Index To 0 and Check the Highest

                    foreach (AnalyzeClass keyassign in BranchList)
                    {
                        keyassign.ReportRowCol = "";
                        keyassign.ReportIndex = 0;
                        ReportIndex = 1;
                    }

                    i = 0;
                    while (true)
                    {
                        i = 0;
                        Highest = 100000;
                        HighestIndex = -1;
                        foreach (AnalyzeClass keyassign in BranchList)
                        {
                            if (keyassign.ReportIndex == 0)
                            {
                                if (keyassign.myOPRectF.Y < Highest)
                                {
                                    Highest = (int)keyassign.myOPRectF.Y;
                                    HighestIndex = i;
                                }
                            }

                            i++;
                        }

                        if (HighestIndex == -1)
                            break;

                        CheckList.Clear();

                        //把相同位置的人找出來
                        i = 0;
                        foreach (AnalyzeClass keyassign in BranchList)
                        {
                            if (keyassign.ReportIndex == 0)
                            {
                                if (IsInRange((int)keyassign.myOPRectF.Y, Highest, _highestValue))
                                {
                                    CheckList.Add(keyassign.myOPRectF.X.ToString("00000000") + "," + i.ToString());
                                }
                            }
                            i++;
                        }

                        CheckList.Sort();

                        i = 1;
                        foreach (string Str in CheckList)
                        {
                            string[] Strs = Str.Split(',');

                            //KEYBOARD.vKEYASSIGNLIST[int.Parse(Strs[1])].ReportIndex = ReportIndex;

                            BranchList[int.Parse(Strs[1])].ReportIndex = ReportIndex;
                            BranchList[int.Parse(Strs[1])].ReportRowCol = CheckList.Count.ToString() + "-" + i.ToString();

                            ReportIndex++;
                            i++;
                        }
                    }

                    if (BranchList.Count == 0)
                        return;

                    m_Mapping_Col = int.Parse(BranchList[0].ReportRowCol.Split('-')[0]);
                    m_Mapping_Row = BranchList.Count / m_Mapping_Col;

                    int iMappingItemWidth = ((pnlResult.Width - 10) - m_Mapping_Col * 3) / m_Mapping_Col;
                    int iMappingItemHeight = iMappingItemWidth;
                    iMappingItemHeight = ((pnlResult.Height - 10) - m_Mapping_Row * 3) / m_Mapping_Row;

                    m_MappingItem = new Label[BranchList.Count];
                    //toolTipsItem = new ToolTip[BranchList.Count];

                    this.pnlResult.Controls.Clear();
                    int ix = 0;
                    int iy = 0;

                    //string colname = "A";
                    int colindex = 1;

                    int colnameindex = 1;

                    i = 0;
                    while (i < BranchList.Count)
                    {
                        //m_MappingItem[i] = new Label();
                        //m_MappingItem[i].Name = BranchList[i].ToAnalyzeString();

                        m_MappingItem[i] = new Label();

                        foreach (AnalyzeClass analyzetmp in BranchList)
                        {
                            if ((i + 1) == analyzetmp.ReportIndex)
                            {
                                m_MappingItem[i].Name = analyzetmp.ToAnalyzeString();
                                m_MappingItem[i].Text = colindex.ToString() + "-" + (int.Parse(analyzetmp.ReportRowCol.Split('-')[1])).ToString("00");
                                //m_MappingItem[i].Text = colindex.ToString() + "-" + colnameindex.ToString();

                                m_MappingItem[i].AccessibleName = colindex.ToString() + "-" + analyzetmp.ReportRowCol.Split('-')[1];

                                break;
                            }
                        }


                        m_MappingItem[i].BackColor = Color.Silver;
                        m_MappingItem[i].Font = new System.Drawing.Font("黑体", 9F);
                        m_MappingItem[i].TextAlign = ContentAlignment.MiddleCenter;
                        m_MappingItem[i].Width = iMappingItemWidth;
                        m_MappingItem[i].Height = iMappingItemHeight;
                        m_MappingItem[i].Location = new Point(5 + ix, 5 + iy);
                        m_MappingItem[i].DoubleClick += RunUI_DoubleClick;
                        m_MappingItem[i].MouseEnter += RunUI_MouseEnter;
                        ix += m_MappingItem[i].Width + 3;
                        colnameindex++;
                        if ((i + 1) % m_Mapping_Col == 0)
                        {
                            iy += m_MappingItem[i].Height + 3;
                            ix = 0;

                            colindex++;

                            colnameindex = 1;

                            //m_MappingItem[i].Text = (i + 1).ToString();
                        }
                        this.pnlResult.Controls.Add(m_MappingItem[i]);

                        i++;
                    }

                    break;
            }
        }

        private void RunUI_MouseEnter(object sender, EventArgs e)
        {
            if (((Label)sender).Tag == null)
                return;

            Label lbl = (Label)sender;
            ToolTip tip = new ToolTip();
            tip.SetToolTip(lbl, (string)lbl.Tag);
        }

        private void RunUI_DoubleClick(object sender, EventArgs e)
        {
            //if (((Label)sender).Tag == null)
            //    return;

            //PassInfoClass passinfo = (PassInfoClass)((Label)sender).Tag;
            ////MessageBox.Show(passinfo.ToInformation());
            //if (passinfo.RcpNo == 80005)
            //    return;

            //OnLearn(passinfo, LearnOperEnum.LEARN);
        }

        #region auto index row col

        bool IsInRange(int FromValue, int CompValue, int DiffValue)
        {
            return (FromValue >= CompValue - DiffValue) && (FromValue <= CompValue + DiffValue);
        }

        #endregion

        #endregion

        public void Initial()
        {
            //myLanguage.Initial(INI.UI_PATH + "\\RunUI.jdb", INI.LANGUAGE, this);

            lblPass = label4;
            lblBigPass = label10;
            lblDuriation = label2;

            txtOp = textBox1;
            txtResource = textBox2;
            txtBarcode = textBox3;

            grpShopFloor = groupBox3;
            grpUserData = groupBox2;
            btnChecked = button4;
            //btnReady = button2;

            btnReady = button3;

            txtOp.KeyDown += new KeyEventHandler(txtOp_KeyDown);
            txtOp.LostFocus += new EventHandler(txtOp_LostFocus);
            txtResource.KeyDown += new KeyEventHandler(txtResource_KeyDown);
            txtResource.LostFocus += new EventHandler(txtResource_LostFocus);
            txtBarcode.KeyDown += new KeyEventHandler(txtBarcode_KeyDown);
            btnChecked.Click += btnChecked_Click;
            btnReady.Click += BtnReady_Click;

            chkIsSaveRaw = checkBox1;
            chkIsSaveNGRaw = checkBox2;
            chkIsSaveDebug = checkBox3;
            chkIsSaveOCR = checkBox4;

            pnlResult = panel1;

            pnlResult.Location = new Point(228, 2);
            pnlResult.Size = new Size(641, 297);
            //pnlResult.Visible = false;
            chkIsSaveOCR.Checked = INI.ISSAVEOCRIMAGE;
            chkIsSaveOCR.CheckedChanged += ChkIsSaveOCR_CheckedChanged;

            chkIsSaveDebug.CheckedChanged += ChkIsSaveDebug_CheckedChanged;

            RPTUI = rptUI1;
            RPTUI.Initial(Universal.LOGTXTPATH, Universal.LOGDBPATH);
            RPTUI.TriggerAction += RPTUI_TriggerAction1;
            RPTUI.Location = new Point(9, 180);

            DS.Location = new Point(lblBigPass.Location.X, lblBigPass.Location.Y);
            init_Display();
            update_Display();

            //lblSeperateLine = label15;
            DS.Visible = false;
            btnChecked.Visible = false;
            JCETSFUI = runMainX6JcetSFUI1;

            switch (OPTION)
            {
                case OptionEnum.MAIN_SDM3:

                    label19.Text = "定位错误";
                    label18.Text = "检测错误";
                    label17.Text = "量测错误";
                    label16.Text = "表面边角缺陷";
                    //label21.Text = "边角缺陷";

                    //label16.Visible = false;
                    label21.Visible = false;

                    break;
            }

            switch (OPTION)
            {
                case OptionEnum.MAIN_SDM5:
                    btnReady.Visible = true;
                    break;
                case OptionEnum.MAIN_SDM2:
                case OptionEnum.MAIN_SDM1:
                case OptionEnum.MAIN_SDM3:
                    lblPass.DoubleClick += LblPass_DoubleClick;

                    btnReady.Visible = true;
                    btnBypass.Visible = true;

                    grpERRSDM1.Location = groupBox3.Location;
                    grpERRSDM1.Visible = true;
                    break;
                case OptionEnum.MAIN_X6:
                //case OptionEnum.MAIN_SERVICE:

                    if (Universal.IsUseMappingUI)
                    {
                        grpErrorColor.Location = groupBox3.Location;
                        grpErrorColor.Visible = true;

                    }
                    else
                    {
                        JCETSFUI.Location = groupBox3.Location;
                        JCETSFUI.Visible = INI.JCET_IS_USE_SHOPFLOOR;
                        JCETSFUI.Init((ControlSpace.MachineSpace.JzMainX6MachineClass)Universal.MACHINECollection.MACHINE);

                        JCETSFUI.TriggerAction += JCETSFUI_TriggerAction;
                    }

                    btnReady.Visible = true;
                    label20.DoubleClick += Label20_DoubleClick;

                    switch(Universal.FACTORYNAME)
                    {
                        case FactoryName.DONGGUAN:
                        case FactoryName.RIYUEXING:

                            label11.Visible = false;
                            label24.Visible = false;
                            label20.Visible = Universal.IsNoUseIO;
                            break;
                    }

                    break;

                case OptionEnum.MAIN_SD:

                    btnTAKENGZero = button1;
                    btnTAKEPASSZero = button2;

                    //btnTAKEPASSZero.Visible = true;
                    //btnTAKENGZero.Visible = true;

                    btnTAKEPASSZero.Click += BtnTAKEPASSZero_Click;
                    btnTAKENGZero.Click += BtnTAKENGZero_Click;

                    break;
            }

        }

        private void LblPass_DoubleClick(object sender, EventArgs e)
        {
            DS.Visible = false;
        }
        public void SetShowChip(Bitmap ebmpinput)
        {
            if (!DS.Visible)
                DS.Visible = true;
            DS.SetDisplayImage(ebmpinput);
        }

        SimFormOffline simFormOffline = null;
        //bool m_simshow = false;
        private void Label20_DoubleClick(object sender, EventArgs e)
        {
            if (Universal.IsNoUseIO && !INI.show_simform)
            {
                simFormOffline = new SimFormOffline();
                simFormOffline.Show();
            }
        }

        private void JCETSFUI_TriggerAction(RunStatusEnum Status)
        {
            if (INI.JCET_IS_USE_SHOPFLOOR)
            {
                switch (Status)
                {
                    case RunStatusEnum.JCET_CHANGE_RECIPE:
                        OnTrigger(RunStatusEnum.JCET_CHANGE_RECIPE);
                        break;
                    case RunStatusEnum.JCET_CLEAR:
                        OnTrigger(RunStatusEnum.JCET_CLEAR);
                        break;
                }
            }
        }

        private void BtnReady_Click(object sender, EventArgs e)
        {
            switch (OPTION)
            {
                case OptionEnum.MAIN_SDM5:
                    OnTrigger(RunStatusEnum.SDM5_READY);
                    break;
                case OptionEnum.MAIN_SDM3:
                    OnTrigger(RunStatusEnum.SDM3_READY);
                    break;
                case OptionEnum.MAIN_SDM2:
                    OnTrigger(RunStatusEnum.SDM2_READY);

                    break;
                case OptionEnum.MAIN_SDM1:
                    OnTrigger(RunStatusEnum.SDM1_READY);

                    break;
                case OptionEnum.MAIN_X6:
                case OptionEnum.MAIN_SERVICE:
                    //PLCIO.Ready = !PLCIO.Ready;

                    OnTrigger(RunStatusEnum.X6_READY);

                    break;

                case OptionEnum.MAIN_SD:



                    break;
            }
        }

        public void btnReadyBK(bool ison)
        {
            btnReady.BackColor = (ison ? Color.Red : Color.FromArgb(192, 255, 192));

        }
        public void btnBYPASSBK(bool ison)
        {
            btnBypass.BackColor = (ison ? Color.Red : Color.FromArgb(192, 255, 192));

        }

        JzMainSDPositionParaClass JzMainSDPositionParas
        {
            get { return Universal.JZMAINSDPOSITIONPARA; }
        }
        MessageForm M_WARNING_FRM;

        private void BtnTAKENGZero_Click(object sender, EventArgs e)
        {
            //M_WARNING_FRM = new MessageForm(true, "是否要复位收料NG区计数");
            //if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
            //{
            //    JzMainSDPositionParas.NgZero();
            //}
            //M_WARNING_FRM.Close();
            //M_WARNING_FRM.Dispose();
        }

        private void BtnTAKEPASSZero_Click(object sender, EventArgs e)
        {
            //M_WARNING_FRM = new MessageForm(true, "是否要复位收料PASS区计数");
            //if (DialogResult.Yes == M_WARNING_FRM.ShowDialog())
            //{
            //    JzMainSDPositionParas.PassZero();
            //}
            //M_WARNING_FRM.Close();
            //M_WARNING_FRM.Dispose();
        }

        private void ChkIsSaveDebug_CheckedChanged(object sender, EventArgs e)
        {
            INI.ISSAVEDebugIMAGE = chkIsSaveDebug.Checked;
        }

        private void ChkIsSaveOCR_CheckedChanged(object sender, EventArgs e)
        {
            INI.ISSAVEOCRIMAGE = chkIsSaveOCR.Checked;
            INI.SAVEOCRIMAGE();
        }

        private void btnChecked_Click(object sender, EventArgs e)
        {
            OnTrigger(RunStatusEnum.BACKTONORMAL);
        }

        private void RPTUI_TriggerAction1(RunStatusEnum runstatus)
        {
            OnTrigger(runstatus);
        }

        //void btnDetail_Click(object sender, EventArgs e)
        //{
        //    if (OPVIEWNow.SETUPList.Count > 0)
        //    {
        //        PARAFrm = new ParaForm(OPVIEWNow);
        //        PARAFrm.ShowDialog();
        //    }
        //    else
        //        MessageBox.Show("請先測試一片。");
        //}

        //void chkIsPauseShopfloor_CheckedChanged(object sender, EventArgs e)
        //{
        //    INI.ISBYDPAUSESHOPFLOOR = !INI.ISBYDPAUSESHOPFLOOR;
        //}

        void RPTUI_TriggerAction(RunStatusEnum runstatus)
        {
            OnTrigger(runstatus);
        }

        public bool SetShopFloorVisible
        {
            set
            {
                grpShopFloor.Visible = value;
            }
        }
        public bool SetUserDataVisible
        {
            set
            {
                grpUserData.Visible = value;
            }
        }
        public bool SetReportUIVisible
        {
            set
            {
                RPTUI.Visible = false;// value;
            }

        }

        void txtResource_LostFocus(object sender, EventArgs e)
        {
            //RUNDB.SetResourceID(txtResource.Text);
        }
        void txtOp_LostFocus(object sender, EventArgs e)
        {
            //RUNDB.SetOPID(txtOp.Text);
            //INI.OP_ID = txtOp.Text.Trim();
            //INI.SaveQSMCData();
        }
        void txtResource_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyData == Keys.Enter)
            //{
            //    txtBarcode.Focus();
            //    txtBarcode.SelectionStart = 0;
            //    txtBarcode.SelectionLength = 100;
            //}
        }
        void txtOp_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyData == Keys.Enter)
            //{
            //    txtResource.Focus();

            //    txtResource.SelectionStart = 0;
            //    txtResource.SelectionLength = 100;
            //}
        }
        public string Barcode_BYD = "";
        void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {

        }

        public void InitialResultPanel()
        {
            switch (OPTION)
            {
                case OptionEnum.MAIN_X6:
                case OptionEnum.MAIN_SD:
                case OptionEnum.MAIN_SERVICE:
                    if (Universal.IsUseMappingUI)
                    {
                        lblDuriation.Text = "";
                        lblDuriation.Invalidate();

                        if (m_MappingItem == null)
                            return;

                        foreach (Label lbl in m_MappingItem)
                        {
                            //if (INI.IsOpenQcRandom)
                            //    lbl.BackColor = Color.Purple;
                            //else
                            //    lbl.BackColor = Color.Green;

                            switch(Universal.jetMappingType)
                            {
                                case JetMappingType.MAPPING_A:
                                    break;
                                default:
                                    lbl.Text = lbl.AccessibleName;
                                    break;
                            }
                            lbl.Tag = string.Empty;
                            lbl.BackColor = Color.Silver;
                            //if(lbl.DoubleClick != null)
                            //{
                            //    lbl.DoubleClick-=   
                            //}

                        }
                    }
                    else
                    {
                        int ix = 0;
                        while (ix < pnlResult.Controls.Count)
                        {
                            if (pnlResult.Controls[ix].ToString().IndexOf("Picture") > -1)
                            {
                                ((PictureBox)pnlResult.Controls[ix]).Image = null;
                            }
                            ix++;
                        }

                        //lblSeperateLine.Visible = false;

                        pnlResult.Controls.Clear();
                        pnlResult.AutoScroll = false;

                        lblBigPass.Visible = true;

                        lblDuriation.Text = "";
                        lblDuriation.Invalidate();

                        btnChecked.Visible = false;
                    }



                    break;
                default:

                    int i = 0;
                    while (i < pnlResult.Controls.Count)
                    {
                        if (pnlResult.Controls[i].ToString().IndexOf("Picture") > -1)
                        {
                            ((PictureBox)pnlResult.Controls[i]).Image = null;
                        }
                        i++;
                    }

                    //lblSeperateLine.Visible = false;

                    pnlResult.Controls.Clear();
                    pnlResult.AutoScroll = false;

                    lblBigPass.Visible = true;

                    lblDuriation.Text = "";
                    lblDuriation.Invalidate();

                    btnChecked.Visible = false;

                    DS.Visible = false;

                    break;
            }
        }

        Color myCheckAnalyzeResult(AnalyzeClass eanalyze, WorkStatusCollectionClass runstatuscollection, out PassInfoClass passInfo)
        {
            Color c = Color.Green;
            int i = 0;

            passInfo = new PassInfoClass();

            if (runstatuscollection.NGCOUNT == 0)
                return c;

            bool bfind = false;

            //先找偏移的错误
            i = 0;
            while (i < runstatuscollection.NGCOUNT)
            {
                if (eanalyze.PassInfo.ToString() == runstatuscollection.GetNGRunStatus(i).PassInfo.ToString()
                    &&
                    runstatuscollection.GetNGRunStatus(i).AnalyzeProcedure == AnanlyzeProcedureEnum.BIAS
                    )
                {
                    bfind = true;
                    break;
                }
                i++;
            }

            if (!bfind)
            {
                i = 0;
                while (i < runstatuscollection.NGCOUNT)
                {
                    if (eanalyze.PassInfo.ToString() == runstatuscollection.GetNGRunStatus(i).PassInfo.ToString())
                    {
                        bfind = true;
                        break;
                    }
                    i++;
                }
            }

            if (bfind)
            {
                c = myAnalyzeProcedure(runstatuscollection.GetNGRunStatus(i).AnalyzeProcedure);
                //passInfo.FromPassInfo(runstatuscollection.GetNGRunStatus(i).PassInfo, OPLevelEnum.COPY);
                passInfo = new PassInfoClass(runstatuscollection.GetNGRunStatus(i).PassInfo, OPLevelEnum.COPY);

                if (runstatuscollection.GetNGRunStatus(i).AnalyzeProcedure == AnanlyzeProcedureEnum.BIAS)
                {
                    passInfo.OperateString = runstatuscollection.GetNGRunStatus(i).ProcessString;
                }

                return c;
            }

            bfind = false;
            i = 0;

            foreach (AnalyzeClass analyze in eanalyze.BranchList)
            {
                i = 0;
                while (i < runstatuscollection.NGCOUNT)
                {
                    if (analyze.PassInfo.ToString() == runstatuscollection.GetNGRunStatus(i).PassInfo.ToString()
                        &&
                        runstatuscollection.GetNGRunStatus(i).AnalyzeProcedure == AnanlyzeProcedureEnum.BIAS
                        )
                    {
                        bfind = true;
                        break;
                    }
                    i++;
                }

                if (!bfind)
                {
                    i = 0;
                    while (i < runstatuscollection.NGCOUNT)
                    {
                        if (analyze.PassInfo.ToString() == runstatuscollection.GetNGRunStatus(i).PassInfo.ToString())
                        {
                            bfind = true;
                            break;
                        }
                        i++;
                    }
                }

                if (bfind)
                {
                    c = myAnalyzeProcedure(runstatuscollection.GetNGRunStatus(i).AnalyzeProcedure);

                    //passInfo.FromPassInfo(runstatuscollection.GetNGRunStatus(i).PassInfo, OPLevelEnum.COPY);
                    passInfo = new PassInfoClass(runstatuscollection.GetNGRunStatus(i).PassInfo, OPLevelEnum.COPY);

                    if (runstatuscollection.GetNGRunStatus(i).AnalyzeProcedure == AnanlyzeProcedureEnum.BIAS)
                    {
                        passInfo.OperateString = runstatuscollection.GetNGRunStatus(i).ProcessString;
                    }

                    break;
                }

                c = myCheckAnalyzeResult(analyze, runstatuscollection, out passInfo);

            }

            return c;
        }

        Color myAnalyzeProcedure(AnanlyzeProcedureEnum ananlyzeProcedure)
        {
            Color c = Color.Red;

            switch (ananlyzeProcedure)
            {
                case AnanlyzeProcedureEnum.LASER:
                case AnanlyzeProcedureEnum.MONTH:
                case AnanlyzeProcedureEnum.YEAR:
                case AnanlyzeProcedureEnum.ALIGNRUN:
                    c = Color.Cyan;
                    break;
                case AnanlyzeProcedureEnum.INSPECTION:
                    c = Color.Red;
                    break;
                case AnanlyzeProcedureEnum.BIAS:
                    c = Color.Violet;
                    break;
                case AnanlyzeProcedureEnum.CHECKDIRT:
                    c = Color.Yellow;
                    break;
                case AnanlyzeProcedureEnum.CHECKBARCODE:
                    c = Color.Fuchsia;
                    break;
                case AnanlyzeProcedureEnum.CHECKMISBARCODE:
                    c = Color.Orange;
                    break;
                case AnanlyzeProcedureEnum.CHECKREPEATBARCODE:
                    c = Color.LightPink;
                    break;
                default:
                    break;
            }

            return c;
        }
        int _getColorIndex(Color eColor)
        {
            int iret = 0;
            if (eColor == Color.Cyan)
            {
                iret = 1;
            }
            else if (eColor == Color.Violet)
            {
                iret = 2;
            }
            else if (eColor == Color.Yellow)
            {
                iret = 3;
            }
            else if (eColor == Color.Red)
            {
                iret = 4;
            }
            else if (eColor == Color.Purple)
            {
                iret = 5;
            }
            else if (eColor == Color.Blue)
            {
                iret = 6;
            }
            else if (eColor == Color.Orange)
            {
                iret = 7;
            }
            else if (eColor == Color.Fuchsia)
            {
                iret = 8;
            }
            else if (eColor == Color.LightPink)
            {
                iret = 9;
            }
            return iret;
        }
        Color _getIndexColor(int eIndex)
        {
            Color _clr = Color.Green;


            switch (eIndex)
            {
                case -1:
                    _clr = Color.Silver;
                    break;
                case 1:
                    _clr = Color.Cyan;
                    break;
                case 2:
                    _clr = Color.Violet;
                    break;
                case 3:
                    _clr = Color.Yellow;
                    break;
                case 4:
                    _clr = Color.Red;
                    break;
                case 5:
                    _clr = Color.Purple;
                    break;
                case 6:
                    _clr = Color.Blue;
                    break;
                case 7:
                    _clr = Color.Orange;
                    break;
                case 8:
                    _clr = Color.Fuchsia;
                    break;
                case 9:
                    _clr = Color.LightPink;
                    break;
            }


            return _clr;

        }
        string _getAnalyzeBarcodeStr(AnalyzeClass eAnalyze)
        {
            if (eAnalyze.OCRPara.OCRMethod == OCRMethodEnum.DATAMATRIX || eAnalyze.OCRPara.OCRMethod == OCRMethodEnum.QRCODE)
            {
                string tempstr = $"No Compare;{eAnalyze.ReadBarcode2DRealStr}";
                if (INI.IsCheckBarcodeOpen)
                {
                    if (string.IsNullOrEmpty(eAnalyze.ReadBarcode2DRealStr))
                        tempstr = $"Compare [FAIL];Marking 2D[{eAnalyze.Barcode_2D}];Read 2D[{eAnalyze.ReadBarcode2DRealStr}]";
                    else
                        tempstr = $"Compare [{(eAnalyze.ReadBarcode2DRealStr == eAnalyze.Barcode_2D ? "PASS" : "FAIL")}];Marking 2D[{eAnalyze.Barcode_2D}];Read 2D[{eAnalyze.ReadBarcode2DRealStr}]";
                }
                return tempstr;
                //return eAnalyze.ReadBarcode2DStr;
            }
            else if (eAnalyze.OCRPara.OCRMethod == OCRMethodEnum.DATAMATRIXGRADE)
            {
                string tempstr = $"No Compare;{eAnalyze.ReadBarcode2DRealStr};{eAnalyze.ReadBarcode2DGrade}";
                if (INI.IsCheckBarcodeOpen)
                {
                    if (INI.IsOpenShowGrade)
                    {
                        if (string.IsNullOrEmpty(eAnalyze.ReadBarcode2DRealStr))
                            tempstr = $"Compare [FAIL];Marking 2D[{eAnalyze.Barcode_2D}];Read 2D[{eAnalyze.ReadBarcode2DRealStr}];Grade[{eAnalyze.ReadBarcode2DGrade}]";
                        else
                            tempstr = $"Compare [{(eAnalyze.ReadBarcode2DRealStr == eAnalyze.Barcode_2D ? "PASS" : "FAIL")}];Marking 2D[{eAnalyze.Barcode_2D}];Read 2D[{eAnalyze.ReadBarcode2DRealStr}];Grade[{eAnalyze.ReadBarcode2DGrade}]";

                    }
                    else
                    {
                        if (string.IsNullOrEmpty(eAnalyze.ReadBarcode2DRealStr))
                            tempstr = $"Compare [FAIL];Marking 2D[{eAnalyze.Barcode_2D}];Read 2D[{eAnalyze.ReadBarcode2DRealStr}]";
                        else
                            tempstr = $"Compare [{(eAnalyze.ReadBarcode2DRealStr == eAnalyze.Barcode_2D ? "PASS" : "FAIL")}];Marking 2D[{eAnalyze.Barcode_2D}];Read 2D[{eAnalyze.ReadBarcode2DRealStr}]";

                    }

                }
                return tempstr;
                //return eAnalyze.ReadBarcode2DStr + ";" + eAnalyze.ReadBarcode2DGrade;
            }
            foreach (AnalyzeClass analyzeClass in eAnalyze.BranchList)
            {
                string _barcodeStr = _getAnalyzeBarcodeStr(analyzeClass);
                if (!string.IsNullOrEmpty(_barcodeStr))
                    return _barcodeStr;
            }
            return string.Empty;
        }
        int _getAnalyzeBarcodeStr(AnalyzeClass eAnalyze, out string outReadBarcode)
        {
            if (eAnalyze.OCRPara.OCRMethod == OCRMethodEnum.DATAMATRIX || eAnalyze.OCRPara.OCRMethod == OCRMethodEnum.QRCODE)
            {
                int tempstr = -1;
                if (INI.IsCheckBarcodeOpen)
                {
                    if (string.IsNullOrEmpty(eAnalyze.ReadBarcode2DRealStr))
                        tempstr = -1;
                    else
                        tempstr = (eAnalyze.ReadBarcode2DRealStr == eAnalyze.Barcode_2D ? 0 : -2);
                }
                outReadBarcode = eAnalyze.ReadBarcode2DRealStr;
                return tempstr;
                //return eAnalyze.ReadBarcode2DStr;
            }
            else if (eAnalyze.OCRPara.OCRMethod == OCRMethodEnum.DATAMATRIXGRADE)
            {
                int tempstr = -1;
                if (INI.IsCheckBarcodeOpen)
                {
                    if (string.IsNullOrEmpty(eAnalyze.ReadBarcode2DRealStr))
                        tempstr = -1;
                    else
                        tempstr = (eAnalyze.ReadBarcode2DRealStr == eAnalyze.Barcode_2D ? 0 : -2);
                }
                outReadBarcode = eAnalyze.ReadBarcode2DRealStr;
                return tempstr;
                //return eAnalyze.ReadBarcode2DStr + ";" + eAnalyze.ReadBarcode2DGrade;
            }
            foreach (AnalyzeClass analyzeClass in eAnalyze.BranchList)
            {
                int iret = _getAnalyzeBarcodeStr(analyzeClass, out outReadBarcode);
                if (!string.IsNullOrEmpty(outReadBarcode))
                    return iret;
            }
            outReadBarcode = "N/A";
            return 0;
        }
        string _getLabelText(string eText, string eFormat = "00")
        {
            string[] strs = eText.Split('-');
            int row = int.Parse(strs[0]);
            int col = int.Parse(strs[1]);
            return row.ToString(eFormat) + "-" + col.ToString(eFormat);
        }

        public void ShowResult(WorkStatusCollectionClass runstatuscollection)
        {
            if (Universal.IsUseMappingUI)
            {
                switch (OPTION)
                {
                    case OptionEnum.MAIN_X6:
                    case OptionEnum.MAIN_SD:
                    case OptionEnum.MAIN_SERVICE:


                        if (INI.IsOpenQcRandom)
                        {
                            if (INI.IsOpenCip)
                            {
                                //int[] ints = new int[m_MappingItem.Length];
                                List<int> list = new List<int>();
                                //StringBuilder sb = new StringBuilder();
                                foreach (var myLabel in m_MappingItem)
                                {
                                    int _colorIndex0 = _getColorIndex(myLabel.BackColor);

                                    //RGB 0 128 0   
                                    int _colorIndex1;
                                    if (myLabel.BackColor.R == 0 && myLabel.BackColor.G == 128 && myLabel.BackColor.B == 0)
                                    {
                                        _colorIndex1 = 1;
                                    }
                                     else
                                    {
                                        _colorIndex1 = 0;
                                    }

                                    list.Add(_colorIndex1);
                                    //sb.Append(_colorIndex);
                                    //sb.Append(" ");
                                }

                                //List<int> ints = new List<int>();
                                //for(int k=0; k < m_MappingItem.Length; k++)
                                //{
                                //    ints.Add(k);
                                //}
                                //Universal.CipExtend.QcMapResult(ints.ToArray());

                                Universal.CipExtend.QcMapResult(list.ToArray());

                            }
                        }


                        switch (Universal.jetMappingType)
                        {
                            case JetMappingType.MAPPING_A:

                                #region MAPPING_A 的数据存储

                                JzMainSDPositionParas.ReportReset();

                                //EnvClass env2 = AlbumNow.ENVList[0];
                                if (m_MappingItem == null)
                                    return;
                                string messageStr2 = string.Empty;
                                int reportIndex2 = 0;
                                while (reportIndex2 < m_MappingItem.Length)
                                {
                                    messageStr2 = string.Empty;
                                    Label lbl = m_MappingItem[reportIndex2];
                                    string STR = lbl.Name + ",";
                                    STR += lbl.Location.X + ",";
                                    STR += lbl.Location.Y + ",";
                                    STR += lbl.Size.Width + ",";
                                    STR += lbl.Size.Height + ",";
                                    STR += _getColorIndex(lbl.BackColor).ToString() + ",";
                                    STR += lbl.Text + ",";
                                    STR += lbl.AccessibleName + ",";
                                    messageStr2 = (string)lbl.Tag;
                                    JzMainSDPositionParas.ReportGradeAdd(_getLabelText(lbl.Text) + ";" + messageStr2 + ";");
                                    STR += _getLabelText(lbl.Text) + ";" + messageStr2 + ";" + ",";
                                    JzMainSDPositionParas.ReportAdd(STR);

                                    reportIndex2++;
                                }

                                #endregion

                                return;
                                break;
                        }


                        JzMainSDPositionParas.ReportReset();

                        AlbumClass album = AlbumNow;
                        EnvClass env = album.ENVList[0];
                        List<AnalyzeClass> BranchList = new List<AnalyzeClass>();
                        BranchList.Clear();

                        foreach (PageClass page in env.PageList)
                        {
                            foreach (AnalyzeClass analyze in page.AnalyzeRoot.BranchList)
                            {
                                if (analyze.ALIGNPara.AbsAlignMode == AbsoluteAlignEnum.MAIN
                                    || analyze.ALIGNPara.AbsAlignMode == AbsoluteAlignEnum.MAIN_LEFT
                                || analyze.ALIGNPara.AbsAlignMode == AbsoluteAlignEnum.MAIN_RIGHT)
                                    continue;
                                BranchList.Add(analyze);
                            }
                        }

                        if (BranchList.Count == 0)
                            return;
                        if (m_MappingItem == null)
                            return;

                        //JzMainSDPositionParas.ReportAdd("row," + m_Mapping_Row.ToString() + ",col," + m_Mapping_Col.ToString());

                        string messageStr = string.Empty;

                        int reportIndex = 0;
                        while (reportIndex < BranchList.Count)
                        {
                            Label lbl = m_MappingItem[reportIndex];

                            foreach (AnalyzeClass analyze in BranchList)
                            {
                                messageStr = string.Empty;

                                //if((reportIndex+1) == analyze.ReportIndex)
                                if (lbl.Name == analyze.ToAnalyzeString())
                                {
                                    PassInfoClass passInfo = new PassInfoClass();
                                    Color color = myCheckAnalyzeResult(analyze, runstatuscollection, out passInfo);
                                    if (analyze.IsByPass)
                                    {
                                        lbl.BackColor = Color.Purple;
                                        color = Color.Purple;
                                    }
                                    else
                                    {
                                        lbl.BackColor = color;
                                        //if (color == Color.Fuchsia)
                                        //{
                                        //    //先判断是否读到码和比对是否成功 0->OK  -1->没读到码 -2->码比对错误
                                        //    int iretbar = _getAnalyzeBarcodeStr(analyze, out string tempoutbarcode);
                                        //    switch (iretbar)
                                        //    {
                                        //        case -1:
                                        //            lbl.BackColor = Color.Fuchsia;
                                        //            break;
                                        //        case -2:
                                        //            lbl.BackColor = Color.Orange;
                                        //            break;
                                        //    }
                                        //}
                                        //else
                                        //{
                                        //    lbl.BackColor = color;
                                        //}
                                    }

                                    string STR = lbl.Name + ",";
                                    STR += lbl.Location.X + ",";
                                    STR += lbl.Location.Y + ",";
                                    STR += lbl.Size.Width + ",";
                                    STR += lbl.Size.Height + ",";
                                    //STR += (!analyze.IsVeryGood ? "1" : "0") + ",";

                                    STR += _getColorIndex(lbl.BackColor).ToString() + ",";
                                    //STR += (color != Color.Green ? "1" : "0") + ",";
                                    STR += lbl.Text + ",";

                                    STR += analyze.PageNo.ToString() + ",";

                                    PointF ptfoffset = new PointF(analyze.ALIGNPara.RunCenter.X - analyze.ALIGNPara.OrgCenter.X,
                                        analyze.ALIGNPara.RunCenter.Y - analyze.ALIGNPara.OrgCenter.Y);

                                    //添加图片中方框的位置
                                    //RectangleF stripRectF = new RectangleF(analyze.myOPRectF.X - ptfoffset.X,
                                    //                                                                    analyze.myOPRectF.Y - ptfoffset.Y,
                                    //                                                                    analyze.myOPRectF.Width,
                                    //                                                                    analyze.myOPRectF.Height);
                                    //添加图片中方框的位置
                                    RectangleF stripRectF = new RectangleF(analyze.myOPRectF.X,
                                                                                                        analyze.myOPRectF.Y,
                                                                                                        analyze.myOPRectF.Width,
                                                                                                        analyze.myOPRectF.Height);

                                    STR += stripRectF.Location.X + ",";
                                    STR += stripRectF.Location.Y + ",";
                                    STR += stripRectF.Width + ",";
                                    STR += stripRectF.Height + ",";

                                    switch (OPTION)
                                    {
                                        case OptionEnum.MAIN_SD:

                                            int irunindex = 0;
                                            while (irunindex < runstatuscollection.WorkStatusList.Count)
                                            {
                                                if (analyze.PassInfo.ToString() == runstatuscollection.WorkStatusList[irunindex].PassInfo.ToString())
                                                {
                                                    if (runstatuscollection.WorkStatusList[irunindex].AnalyzeProcedure == AnanlyzeProcedureEnum.PADINSPECT)
                                                    {
                                                        messageStr += runstatuscollection.WorkStatusList[irunindex].ProcessString + Environment.NewLine;
                                                        break;
                                                    }

                                                }

                                                irunindex++;
                                            }

                                            break;
                                        case OptionEnum.MAIN_SERVICE:
                                        case OptionEnum.MAIN_X6:
                                            messageStr = _getAnalyzeBarcodeStr(analyze);
                                            JzMainSDPositionParas.ReportGradeAdd(_getLabelText(lbl.Text) + ";" + messageStr + ";");
                                            break;
                                    }

                                    STR += _getLabelText(lbl.Text) + ";" + messageStr + ";" + ",";
                                    JzMainSDPositionParas.ReportAdd(STR);

                                    if (!string.IsNullOrEmpty(messageStr))
                                        lbl.Tag = messageStr;
                                    else
                                        lbl.Tag = string.Empty;

                                    if (color == Color.Violet)
                                    {
                                        lbl.Text = passInfo.OperateString;
                                        lbl.Tag = passInfo.OperateString;
                                    }

                                    break;
                                }
                            }


                            reportIndex++;
                        }

                        //foreach (Label lbl in m_MappingItem)
                        //{
                            
                        //}


                        #region 20230727BAK NO USE

                        /*
                         * 
                         *  foreach (Label lbl in m_MappingItem)
                        {
                            foreach (AnalyzeClass analyze in BranchList)
                            {
                                messageStr = string.Empty;
                                if (lbl.Name == analyze.ToAnalyzeString())
                                {
                                    PassInfoClass passInfo = new PassInfoClass();
                                    Color color = myCheckAnalyzeResult(analyze, runstatuscollection, out passInfo);
                                    if (analyze.IsByPass)
                                    {
                                        lbl.BackColor = Color.Purple;
                                        color = Color.Purple;
                                    }
                                    else
                                        lbl.BackColor = color;

                                    string STR = lbl.Name + ",";
                                    STR += lbl.Location.X + ",";
                                    STR += lbl.Location.Y + ",";
                                    STR += lbl.Size.Width + ",";
                                    STR += lbl.Size.Height + ",";
                                    //STR += (!analyze.IsVeryGood ? "1" : "0") + ",";

                                    STR += _getColorIndex(lbl.BackColor).ToString() + ",";
                                    //STR += (color != Color.Green ? "1" : "0") + ",";
                                    STR += lbl.Text + ",";

                                    STR += analyze.PageNo.ToString() + ",";

                                    PointF ptfoffset = new PointF(analyze.ALIGNPara.RunCenter.X - analyze.ALIGNPara.OrgCenter.X,
                                        analyze.ALIGNPara.RunCenter.Y - analyze.ALIGNPara.OrgCenter.Y);

                                    //添加图片中方框的位置
                                    //RectangleF stripRectF = new RectangleF(analyze.myOPRectF.X - ptfoffset.X,
                                    //                                                                    analyze.myOPRectF.Y - ptfoffset.Y,
                                    //                                                                    analyze.myOPRectF.Width,
                                    //                                                                    analyze.myOPRectF.Height);
                                    //添加图片中方框的位置
                                    RectangleF stripRectF = new RectangleF(analyze.myOPRectF.X,
                                                                                                        analyze.myOPRectF.Y,
                                                                                                        analyze.myOPRectF.Width,
                                                                                                        analyze.myOPRectF.Height);

                                    STR += stripRectF.Location.X + ",";
                                    STR += stripRectF.Location.Y + ",";
                                    STR += stripRectF.Width + ",";
                                    STR += stripRectF.Height + ",";

                                    JzMainSDPositionParas.ReportAdd(STR);

                                    switch (OPTION)
                                    {
                                        case OptionEnum.MAIN_SD:

                                            int irunindex = 0;
                                            while (irunindex < runstatuscollection.WorkStatusList.Count)
                                            {
                                                if (analyze.PassInfo.ToString() == runstatuscollection.WorkStatusList[irunindex].PassInfo.ToString())
                                                {
                                                    if (runstatuscollection.WorkStatusList[irunindex].AnalyzeProcedure == AnanlyzeProcedureEnum.PADINSPECT)
                                                    {
                                                        messageStr += runstatuscollection.WorkStatusList[irunindex].ProcessString + Environment.NewLine;
                                                        break;
                                                    }

                                                }

                                                irunindex++;
                                            }

                                            break;

                                        case OptionEnum.MAIN_X6:
                                            messageStr = _getAnalyzeBarcodeStr(analyze);
                                            JzMainSDPositionParas.ReportGradeAdd(_getLabelText(lbl.Text) + ";" + messageStr + ";");
                                            break;
                                    }

                                    if (!string.IsNullOrEmpty(messageStr))
                                        lbl.Tag = messageStr;
                                    else
                                        lbl.Tag = string.Empty;

                                    if (color == Color.Violet)
                                    {
                                        lbl.Text = passInfo.OperateString;
                                        lbl.Tag = passInfo.OperateString;
                                    }

                                }
                            }
                        }

                         * 
                         * 
                         */

                        #endregion


                        return;

                        break;
                }
            }

            if (Universal.OPTION == OptionEnum.MAIN_SDM3)
                return;
            if (Universal.OPTION == OptionEnum.MAIN_SDM2)
                return;

            if (runstatuscollection.NGCOUNT == 0)
                return;

            if (Universal.OPTION == OptionEnum.R3 && Universal.isR3ByPass)
                return;

            if (Universal.OPTION == OptionEnum.C3 && Universal.isC3ByPass)
                return;

            lblBigPass.Visible = false;

            int i = 0;

            int picwidthheight = 168;

            Point InitialPosition = new Point(9, 31);
            Point IndexLabelPostion = new Point(0, -26);
            Point DescriptionLabelPostion = new Point(picwidthheight + 2, -26);

            int Columns = 1;
            int ColumeGap = picwidthheight * 2;
            int RowGap = picwidthheight + 26 + 9 + 2;

            Size PicSize = new Size(picwidthheight, picwidthheight);
            Size IndexLabelSize = new Size(picwidthheight, 23);
            Size DescriptLabelSize = new Size(picwidthheight * 2, 23);

            //lblSeperateLine.Visible = runstatuscollection.NGCOUNT > 1;
            //if (runstatuscollection.NGCOUNT > 1)
            //{
            //    Label lblseperate = new Label();
            //    lblseperate.BackColor = Color.Black;
            //    lblseperate.Location = new Point(320, 1);
            //    lblseperate.Size = new Size(2, 290);

            //    pnlResult.Controls.Add(lblseperate);
            //}

            pnlResult.AutoScroll = (runstatuscollection.NGCOUNT > 1);

            btnChecked.Visible = true;

            string debugsavedatetimestring = JzTimes.DateTimeSerialString;

            switch (Universal.OPTION)
            {
                //case OptionEnum.MAIN_SDM3:
                case OptionEnum.MAIN_SDM2:

                    if (!Directory.Exists(Universal.MainSDM2NG_Path + debugsavedatetimestring))
                        Directory.CreateDirectory(Universal.MainSDM2NG_Path + debugsavedatetimestring);


                    break;
                case OptionEnum.MAIN_SDM1:
                    break;
            }

            while (i < runstatuscollection.NGCOUNT)
            {
                //if (i == 8)
                //    i = i;

                switch (Universal.OPTION)
                {
                    case OptionEnum.MAIN_SDM5:
                        if (i >= 30)
                            return;
                        break;
                }

                Label lblIndex = new Label();
                lblIndex.BackColor = Color.Yellow;
                lblIndex.Location = new Point(InitialPosition.X + (i % Columns) * ColumeGap + IndexLabelPostion.X, InitialPosition.Y + (i / (Columns)) * RowGap + IndexLabelPostion.Y);
                lblIndex.Size = IndexLabelSize;
                lblIndex.TextAlign = ContentAlignment.MiddleCenter;
                lblIndex.Font = new Font("新細明體", 12);

                Label lblDescipt = new Label();
                lblDescipt.BackColor = Color.FromArgb(255, 192, 192);
                lblDescipt.Location = new Point(InitialPosition.X + (i % Columns) * ColumeGap + DescriptionLabelPostion.X, InitialPosition.Y + (i / (Columns)) * RowGap + DescriptionLabelPostion.Y);
                lblDescipt.Size = DescriptLabelSize;
                lblDescipt.TextAlign = ContentAlignment.MiddleCenter;
                lblDescipt.Font = new Font("新細明體", 8);

                PictureBox POrg = new PictureBox();
                POrg.BackColor = Color.Black;
                POrg.Location = new Point(InitialPosition.X + (i % Columns) * ColumeGap, InitialPosition.Y + (i / (Columns)) * RowGap);
                POrg.Size = PicSize;
                POrg.SizeMode = PictureBoxSizeMode.Zoom;
                POrg.Tag = runstatuscollection.GetNGRunStatus(i).PassInfo;
                POrg.DoubleClick += new EventHandler(POrg_DoubleClick);

                PictureBox PRun = new PictureBox();
                PRun.BackColor = Color.Black;
                PRun.Location = new Point(InitialPosition.X + (i % Columns) * ColumeGap + PicSize.Width + 2, InitialPosition.Y + (i / (Columns)) * RowGap);
                PRun.Size = PicSize;
                PRun.SizeMode = PictureBoxSizeMode.Zoom;
                PRun.Tag = runstatuscollection.GetNGRunStatus(i).PassInfo;
                PRun.DoubleClick += new EventHandler(PRun_DoubleClick);

                PictureBox PResult = new PictureBox();
                PResult.BackColor = Color.Black;
                PResult.Location = new Point(InitialPosition.X + (i % Columns) * ColumeGap + PicSize.Width * 2 + 4, InitialPosition.Y + (i / (Columns)) * RowGap);
                PResult.Size = PicSize;
                PResult.SizeMode = PictureBoxSizeMode.Zoom;
                PResult.Tag = runstatuscollection.GetNGRunStatus(i).PassInfo;
                PResult.DoubleClick += new EventHandler(PResult_DoubleClick);

                switch (runstatuscollection.GetNGRunStatus(i).AnalyzeProcedure)
                {
                    case AnanlyzeProcedureEnum.CHECKOCR:
                        POrg.DoubleClick -= new EventHandler(POrg_DoubleClick);
                        PRun.DoubleClick -= new EventHandler(PRun_DoubleClick);
                        //PResult.DoubleClick -= new EventHandler(PResult_DoubleClick);
                        break;
                    case AnanlyzeProcedureEnum.STILTS:
                        POrg.DoubleClick -= new EventHandler(POrg_DoubleClick);
                        PRun.DoubleClick -= new EventHandler(PRun_DoubleClick);
                        //PResult.DoubleClick -= new EventHandler(PResult_DoubleClick);
                        break;
                }




                POrg.Image = runstatuscollection.GetNGRunStatus(i).bmpORG;
                PRun.Image = runstatuscollection.GetNGRunStatus(i).bmpRUN;
                PResult.Image = runstatuscollection.GetNGRunStatus(i).bmpDIFF;

                //if(IsSaveDebug)
                //{

                //    PassInfoClass passinfo = runstatuscollection.GetNGRunStatus(i).PassInfo;
                //    string dirpath = Universal.DEBUGRESULTPATH + "\\" + debugsavedatetimestring + "\\" + passinfo.ToDebugPath();

                //    if (!Directory.Exists(dirpath))
                //        Directory.CreateDirectory(dirpath);

                //    string filepath = dirpath + "\\" + passinfo.ToDebugFile();

                //    runstatuscollection.GetNGRunStatus(i).bmpORG.Save(filepath + "-R00" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                //    runstatuscollection.GetNGRunStatus(i).bmpRUN.Save(filepath + "-R01" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                //    runstatuscollection.GetNGRunStatus(i).bmpDIFF.Save(filepath + "-R02" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                //}

                //POrg.Image.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + i.ToString("00") + "ORG" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                //PRun.Image.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + i.ToString("00") + "RUN" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
                //PResult.Image.Save(Universal.TESTPATH + "\\ANALYZETEST\\" + i.ToString("00") + "RESULT" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                //bmpTmp[i, 0].Save(@"D:\TEMP\ERRO" + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);

                lblIndex.Text = runstatuscollection.GetNGRunStatus(i).NGIndexStr;
                //lblDescipt.Text = runstatuscollection.GetNGRunStatus(i).AnalyzeProcedure.ToString() + "(" + runstatuscollection.GetNGRunStatus(i).RelateAnalyzeInformation + ")";
                lblDescipt.Text = runstatuscollection.GetNGRunStatus(i).AnalyzeProcedure.ToString();

                if (runstatuscollection.GetNGRunStatus(i).AnalyzeProcedure == AnanlyzeProcedureEnum.BIAS)
                {
                    //lblDescipt.Text += "偏移=" + runstatuscollection.GetNGRunStatus(i).ErrorString;
                    lblDescipt.Text = runstatuscollection.GetNGRunStatus(i).ErrorString;
                }

                switch (Universal.OPTION)
                {
                    //case OptionEnum.MAIN_SDM3:
                    case OptionEnum.MAIN_SDM2:

                        if (runstatuscollection.GetNGRunStatus(i).AnalyzeProcedure == AnanlyzeProcedureEnum.GLUEINSPECT)
                        {
                            //lblDescipt.Text += "偏移=" + runstatuscollection.GetNGRunStatus(i).ErrorString;
                            lblDescipt.Text = runstatuscollection.GetNGRunStatus(i).Desc + "(" + runstatuscollection.GetNGRunStatus(i).PassInfo.ToAnalyzeString() + ")";
                        }

                        runstatuscollection.GetNGRunStatus(i).bmpORG.Save(Universal.MainSDM2NG_Path + debugsavedatetimestring
                            + "\\" + i.ToString() + "_org.png", System.Drawing.Imaging.ImageFormat.Png);
                        runstatuscollection.GetNGRunStatus(i).bmpRUN.Save(Universal.MainSDM2NG_Path + debugsavedatetimestring
                           + "\\" + i.ToString() + "_run.png", System.Drawing.Imaging.ImageFormat.Png);
                        runstatuscollection.GetNGRunStatus(i).bmpDIFF.Save(Universal.MainSDM2NG_Path + debugsavedatetimestring
                           + "\\" + i.ToString() + "_diff.png", System.Drawing.Imaging.ImageFormat.Png);


                        //POrg.Image = runstatuscollection.GetNGRunStatus(i).bmpORG;
                        //PRun.Image = runstatuscollection.GetNGRunStatus(i).bmpRUN;
                        //PResult.Image = runstatuscollection.GetNGRunStatus(i).bmpDIFF;
                        //if (runstatuscollection.GetNGRunStatus(i).AnalyzeProcedure == AnanlyzeProcedureEnum.GLUEINSPECT)
                        //{
                        //    //lblDescipt.Text += "偏移=" + runstatuscollection.GetNGRunStatus(i).ErrorString;
                        //    lblDescipt.Text = runstatuscollection.GetNGRunStatus(i).Desc + "(" + runstatuscollection.GetNGRunStatus(i).PassInfo.ToPassInfoNameString() + ")";
                        //}

                        break;
                    case OptionEnum.MAIN_SDM1:

                        if (runstatuscollection.GetNGRunStatus(i).AnalyzeProcedure == AnanlyzeProcedureEnum.GLUEINSPECT)
                        {
                            //lblDescipt.Text += "偏移=" + runstatuscollection.GetNGRunStatus(i).ErrorString;
                            lblDescipt.Text = runstatuscollection.GetNGRunStatus(i).Desc + "(" + runstatuscollection.GetNGRunStatus(i).PassInfo.ToAnalyzeString() + ")";
                        }

                        break;
                    case OptionEnum.MAIN_SDM5:

                        if (runstatuscollection.GetNGRunStatus(i).AnalyzeProcedure == AnanlyzeProcedureEnum.MEASURE)
                        {
                            //lblDescipt.Text += "偏移=" + runstatuscollection.GetNGRunStatus(i).ErrorString;
                            lblDescipt.Text = runstatuscollection.GetNGRunStatus(i).Desc + "(" + runstatuscollection.GetNGRunStatus(i).PassInfo.ToAnalyzeString() + ")";
                        }

                        break;
                }

                pnlResult.Controls.Add(lblIndex);
                pnlResult.Controls.Add(lblDescipt);
                pnlResult.Controls.Add(POrg);
                pnlResult.Controls.Add(PRun);
                pnlResult.Controls.Add(PResult);

#if DEBUG_BMP
                POrg.Image.Save(@"D:\ERRORS\" + Universal.TestName + "-" + i.ToString() + "01 Org.jpg", ImageFormat.Jpeg);
                PRun.Image.Save(@"D:\ERRORS\" + Universal.TestName + "-" + i.ToString() + "02 RunError.jpg", ImageFormat.Jpeg);
#endif


                i++;
            }
        }

        public int SetByPass(bool[] bAllValue,ref string datalogStr)
        {
            int iret = 0;

            AlbumClass album = AlbumNow;
            EnvClass env = album.ENVList[0];
            List<AnalyzeClass> BranchList = new List<AnalyzeClass>();
            BranchList.Clear();

            foreach (PageClass page in env.PageList)
            {
                foreach (AnalyzeClass analyze in page.AnalyzeRoot.BranchList)
                {
                    if (analyze.ALIGNPara.AbsAlignMode == AbsoluteAlignEnum.MAIN
                        || analyze.ALIGNPara.AbsAlignMode == AbsoluteAlignEnum.MAIN_LEFT
                                || analyze.ALIGNPara.AbsAlignMode == AbsoluteAlignEnum.MAIN_RIGHT)
                        continue;
                    BranchList.Add(analyze);
                }
            }

            datalogStr = string.Empty;

            if (BranchList.Count == 0)
                return -1;
            if (m_MappingItem == null)
                return -2;
            if (bAllValue == null)
                return -3;
            datalogStr += BranchList.Count.ToString() + ",";
            datalogStr += m_MappingItem.Length.ToString() + ",";
            datalogStr += bAllValue.Length.ToString();

            if (m_MappingItem.Length != bAllValue.Length)
                return -4;
            if (BranchList.Count != bAllValue.Length)
                return -5;

            int i = 0;
            bool bOK = false;
            while (i < BranchList.Count)
            {
                Label lbl = m_MappingItem[i];
                bOK = false;
                foreach (PageClass page in env.PageList)
                {
                    foreach (AnalyzeClass analyze in page.AnalyzeRoot.BranchList)
                    {
                        //if ((i + 1) == analyze.ReportIndex)
                        if (lbl.Name == analyze.ToAnalyzeString())
                        {
                            analyze.SetAnalyzeByPass(bAllValue[i]);
                            bOK = true;
                            break;
                        }
                    }
                    if (bOK)
                        break;
                }
                i++;
            }

            //old funtion nouse 
            //int i = 0;
            //foreach (Label lbl in m_MappingItem)
            //{
            //    foreach (PageClass page in env.PageList)
            //    {
            //        foreach (AnalyzeClass analyze in page.AnalyzeRoot.BranchList)
            //        {
            //            if (lbl.Name == analyze.ToAnalyzeString())
            //            {
            //                analyze.SetAnalyzeByPass(bAllValue[i]);

            //                //analyze.IsByPass = bAllValue[i];
            //                //lbl.BackColor = (analyze.IsByPass ? Color.Purple : Color.Green);
            //            }
            //        }
            //    }

            //    i++;
            //}


            return 0;

        }
        public int SetCheckBarcode(string[] bAllBarcodeValue, ref string datalogStr)
        {
            int iret = 0;

            AlbumClass album = AlbumNow;
            EnvClass env = album.ENVList[0];
            List<AnalyzeClass> BranchList = new List<AnalyzeClass>();
            BranchList.Clear();

            foreach (PageClass page in env.PageList)
            {
                foreach (AnalyzeClass analyze in page.AnalyzeRoot.BranchList)
                {
                    if (analyze.ALIGNPara.AbsAlignMode == AbsoluteAlignEnum.MAIN
                        || analyze.ALIGNPara.AbsAlignMode == AbsoluteAlignEnum.MAIN_LEFT
                                || analyze.ALIGNPara.AbsAlignMode == AbsoluteAlignEnum.MAIN_RIGHT)
                        continue;
                    BranchList.Add(analyze);
                }
            }

            datalogStr = string.Empty;

            if (BranchList.Count == 0)
                return -1;
            if (m_MappingItem == null)
                return -2;
            if (bAllBarcodeValue == null)
                return -3;
            datalogStr += BranchList.Count.ToString() + ",";
            datalogStr += m_MappingItem.Length.ToString() + ",";
            datalogStr += bAllBarcodeValue.Length.ToString();

            if (m_MappingItem.Length != bAllBarcodeValue.Length)
                return -4;
            if (BranchList.Count != bAllBarcodeValue.Length)
                return -5;

            int i = 0;
            foreach (Label lbl in m_MappingItem)
            {
                foreach (PageClass page in env.PageList)
                {
                    foreach (AnalyzeClass analyze in page.AnalyzeRoot.BranchList)
                    {
                        if (lbl.Name == analyze.ToAnalyzeString())
                        {
                            analyze.SetAnalyzeCheckBarcodeStr(bAllBarcodeValue[i]);
                        }
                    }
                }

                i++;
            }

            return 0;

        }
        public int SetAnalyzeMapping(IxMapBuilder eMap, ref string datalogStr)
        {
            int iret = 0;

            AlbumClass album = AlbumNow;
            EnvClass env = album.ENVList[0];
            List<AnalyzeClass> BranchList = new List<AnalyzeClass>();
            BranchList.Clear();

            foreach (PageClass page in env.PageList)
            {
                foreach (AnalyzeClass analyze in page.AnalyzeRoot.BranchList)
                {
                    if (analyze.ALIGNPara.AbsAlignMode == AbsoluteAlignEnum.MAIN
                        || analyze.ALIGNPara.AbsAlignMode == AbsoluteAlignEnum.MAIN_LEFT
                                || analyze.ALIGNPara.AbsAlignMode == AbsoluteAlignEnum.MAIN_RIGHT)
                        continue;
                    BranchList.Add(analyze);
                }
            }

            datalogStr = string.Empty;

            if (BranchList.Count == 0)
                return -1;
            if (m_MappingItem == null)
                return -2;
            if (eMap == null)
                return -3;
            datalogStr += BranchList.Count.ToString() + ",";
            datalogStr += m_MappingItem.Length.ToString() + ",";
            datalogStr += eMap.AnylazeCount.ToString();

            if (m_MappingItem.Length != eMap.AnylazeCount)
                return -4;
            if (BranchList.Count != eMap.AnylazeCount)
                return -5;

            int i = 0;
            foreach (Label lbl in m_MappingItem)
            {
                foreach (PageClass page in env.PageList)
                {
                    foreach (AnalyzeClass analyze in page.AnalyzeRoot.BranchList)
                    {
                        if (lbl.Name == analyze.ToAnalyzeString())
                        {
                            //转换label Text的数据 行列=>第几个
                            //算出当前位置是第几个
                            string[] _curr = lbl.Text.Split('-');
                            if (_curr.Length == 2)
                            {
                                int x = 0;
                                int y = 0;
                                int.TryParse(_curr[0].Trim(), out x);
                                int.TryParse(_curr[1].Trim(), out y);
                                int posCurr = (x - 1) * m_Mapping_Row + (y - 1);
                                analyze.SetAnalyzeCheckBarcodeStr(posCurr, eMap);
                            }
                        }
                    }
                }

                i++;
            }

            return 0;

        }
        private void Lbl_DoubleClick(object sender, EventArgs e)
        {

        }

        frmShowPicture showPicture = null;
        private void PResult_DoubleClick(object sender, EventArgs e)
        {
            PassInfoClass passinfo = (PassInfoClass)((PictureBox)sender).Tag;
            //MessageBox.Show(passinfo.ToInformation());

            OnLearn(passinfo, LearnOperEnum.COMP);

            showPicture = new frmShowPicture();
            showPicture.SetImage((Bitmap)((PictureBox)sender).Image);
            showPicture.ShowDialog();
            showPicture.Dispose();
            showPicture = null;
        }

        //public string ErrorReason(ReasonEnum reason)
        //{
        //    string retStr = "";

        //    switch (reason)
        //    {
        //        case ReasonEnum.ALIGNREGIONERROR:
        //            retStr = "區域定位錯誤";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "Region Align Error";

        //            break;
        //        case ReasonEnum.ALIGNREGIONNG:
        //            retStr = "區域定位失敗";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "Region Align NG";

        //            break;
        //        case ReasonEnum.REGIONSIZENG:
        //            retStr = "區域尺吋錯誤";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "Region Size NG";

        //            break;
        //        case ReasonEnum.REGIONDIRTYNG:
        //            retStr = "區域髒污";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "Region Dirty";
        //            break;
        //        case ReasonEnum.REGIONSHIFTNG:
        //            retStr = "區域偏移";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "Region Shift";
        //            break;
        //        case ReasonEnum.NODEFINENG:
        //            retStr = "無定義此參數";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "Region No Define";
        //            break;
        //        case ReasonEnum.REGIONCOLORNG:
        //            retStr = "區域色彩錯誤";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "Region Color Error";
        //            break;
        //        case ReasonEnum.BLINDNG:
        //            retStr = "盲鍵錯誤";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "Blind NG";
        //            break;
        //        case ReasonEnum.WHITENNG:
        //            retStr = "發白";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "Whiten NG";

        //            break;
        //        case ReasonEnum.TESTSHIFTNG:
        //            retStr = "檢測偏移";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "Shift NG";
        //            break;
        //        case ReasonEnum.MEASURENG:
        //            retStr = " 量測錯誤";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "Measure NG";
        //            break;
        //        case ReasonEnum.ALIGNTESTNG:
        //            retStr = "檢測對象不同";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "Test Align NG";

        //            break;
        //        case ReasonEnum.INSPECTTESTNG:
        //            retStr = "檢測缺失";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "Inspection NG";

        //            break;
        //        case ReasonEnum.GAPNG:
        //            retStr = "間隙差異過大";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "Gap NG";
        //            break;
        //        case ReasonEnum.NOISENG:
        //            retStr = "雜訊";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "Noise NG";
        //            break;
        //        case ReasonEnum.QRCODENG:
        //            retStr = "二維碼讀取錯誤";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "QR Code NG";
        //            break;
        //        case ReasonEnum.BARCODENG:
        //            retStr = "一維碼讀取錯誤";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "1D Barcode Code NG";
        //            break;
        //        case ReasonEnum.CENTEROVER:
        //            retStr = "中心鬆脫";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "Center Over";
        //            break;
        //        case ReasonEnum.EDGEOVER:
        //            retStr = "兩側鬆脫";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "Edge Over";
        //            break;
        //        case ReasonEnum.PURECOLORERROR:
        //            retStr = "純色不同";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "Not Pure Color";
        //            break;
        //        case ReasonEnum.SCREWCOLORERROR:
        //            retStr = "顏色不同";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "Diff Color";
        //            break;
        //        case ReasonEnum.LABELCOLORERROR:
        //            retStr = "盒色不同";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "Diff Label Color";
        //            break;
        //        case ReasonEnum.BASECOLORERROR:
        //            retStr = "底色不同";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "Diff Base Color";
        //            break;
        //        case ReasonEnum.BLOBNG:
        //            retStr = "斑點不足";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "Blob NG";
        //            break;
        //        case ReasonEnum.BLOBLIMITNG:
        //            retStr = "斑點過大";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "Blob Limit NG";
        //            break;
        //        case ReasonEnum.SCREFINDNG:
        //            retStr = "螺絲環錯誤";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "Screw Ring NG";
        //            break;
        //        case ReasonEnum.WIDTHNG:
        //            retStr = "寬度/高度過大";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "Width Over";
        //            break;
        //        case ReasonEnum.RADIUSNG:
        //            retStr = "直徑過寬";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "Radius NG";
        //            break;
        //        case ReasonEnum.VERSIONNG:
        //            retStr = "版本不同";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "Version NG";
        //            break;
        //        case ReasonEnum.NGFIT:
        //            retStr = "符合錯誤";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "In NG";
        //            break;
        //        case ReasonEnum.SCREWNG:
        //            retStr = "螺絲錯誤";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "Screw NG";
        //            break;
        //        case ReasonEnum.VHBNG:
        //            retStr = "VHB錯誤";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "VHB NG";
        //            break;
        //        case ReasonEnum.VHBGAPNG:
        //            retStr = "VHB GAP錯誤";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "VHB GAP NG";
        //            break;
        //        case ReasonEnum.CUSTCONFIGNG:
        //            retStr = "配置信息錯誤";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "CUSTCONFIGNG NG";
        //            break;
        //        default:
        //            retStr = "未定義";

        //            if (INI.LANGUAGE == 1)
        //                retStr = "Undefined";
        //            break;
        //    }


        //    return retStr;
        //}

        //LearnForm LEARNFRM;
        //TrainMessageForm TRAINMSGFRM;

        //This Double Click will edit the New setup in the Train Chain
        void PRun_DoubleClick(object sender, EventArgs e)
        {
            PassInfoClass passinfo = (PassInfoClass)((PictureBox)sender).Tag;
            //MessageBox.Show(passinfo.ToInformation());
            if (passinfo.RcpNo == 80005)
                return;

            OnLearn(passinfo, LearnOperEnum.LEARN);
        }

        //This Double Click will add the Origin setup in to Train Chain
        void POrg_DoubleClick(object sender, EventArgs e)
        {
            PassInfoClass passinfo = (PassInfoClass)((PictureBox)sender).Tag;
            if (passinfo.RcpNo == 80005)
                return;
            //MessageBox.Show(passinfo.ToInformation());
            OnLearn(passinfo, LearnOperEnum.TUNE);
        }

        //void TranLearn(REGIONClass region, SIDEClass side, SETUPClass setup)
        //{
        //    bool IsNoTrained = false;

        //    TRAINMSGFRM = new TrainMessageForm();
        //    TRAINMSGFRM.Show();

        //    List<string> strlist = new List<string>();

        //    TRAINMSGFRM.SetString("Start REGION " + region.IndexStr + " Learning  Procedure...");

        //    //IsNoTrained = !region.TrainWithLearn(ref strlist, side.bmpOrg);
        //    IsNoTrained = !region.Train(ref strlist, side.cogImgFixturedOrg, side.bmpOrg, side.cogImgFixtured24Org);

        //    TRAINMSGFRM.SetString(strlist);

        //    setup.SaveSIDE(false);

        //    if (IsNoTrained)
        //        TRAINMSGFRM.SetCancel();
        //    else
        //        TRAINMSGFRM.SetComplete();
        //}

        public void SetLanguage()
        {
            //myLanguage.SetControlLanguage(this, INI.LANGUAGE);
        }

        public bool Disable
        {
            set
            {
                this.Enabled = !value;
                this.Visible = !value;
            }
        }

        public void FocusBarcode(bool IsPass)
        {
            txtBarcode.Enabled = true;
            //Application.DoEvents();
            //SavePrintScreen(IsPass);

            if (IsPass)
                txtBarcode.Text = "";

            txtBarcode.Focus();
            txtBarcode.SelectionStart = 0;
            txtBarcode.SelectionLength = 100;
        }

        void SavePrintScreen(bool IsPass)
        {
            /*
            if (!IsPass)
            {
                string strpath = @"D:\PRINTSCREEN\" + JzTimes.DateSerialString;

                if (!Directory.Exists(strpath))
                {
                    Directory.CreateDirectory(strpath);
                }

                int width = Screen.PrimaryScreen.Bounds.Width;
                int height = Screen.PrimaryScreen.Bounds.Height;

                Bitmap m = new Bitmap(width, height);
                
                using (Graphics g = Graphics.FromImage(m))
                {
                    g.CopyFromScreen(0, 0, 0, 0, Screen.AllScreens[0].Bounds.Size);
                    g.Dispose();
                }

                m.Save(strpath + "\\" + (BARCODETXT == "" ? JzTimes.TimeSerialString : BARCODETXT) + ".jpg", ImageFormat.Jpeg);

                m.Dispose();
            }
            */
            //string strpath = @"D:\PRINTSCREEN\" + JzTimes.DateSerialString;

            //string Qsmcpath = INI.ALLRESULTPIC + "\\NGPictures\\" + JzTimes.DateSerialString;

            //string strlogpath = INI.WORK_PATH + "\\_PrintScreen\\" + JzTimes.DateSerialString;

            //if (!Directory.Exists(strpath))
            //{
            //    Directory.CreateDirectory(strpath);
            //}

            //if (!Directory.Exists(Qsmcpath))
            //{
            //    Directory.CreateDirectory(Qsmcpath);
            //}

            //if (!Directory.Exists(strlogpath))
            //{
            //    Directory.CreateDirectory(strlogpath);
            //}

            //int width = Screen.PrimaryScreen.Bounds.Width;
            //int height = Screen.PrimaryScreen.Bounds.Height;

            //Bitmap m = new Bitmap(width, height);

            //using (Graphics g = Graphics.FromImage(m))
            //{
            //    g.CopyFromScreen(0, 0, 0, 0, Screen.AllScreens[0].Bounds.Size);
            //    g.Dispose();
            //}

            //if (!IsPass)
            //{
            //    m.Save(strpath + "\\" + (BARCODE == "" ? JzTimes.TimeSerialString : BARCODE) + ".jpg", ImageFormat.Jpeg);
            //    m.Save(Qsmcpath + "\\" + (RESULT.BARCODE == "" ? JzTimes.DateTimeSerialString + "_" + INI.FIXTUREID + "_NULLSN_OCR" : JzTimes.DateTimeSerialString + "_" + INI.FIXTUREID + "_" + RESULT.BARCODE + "_OCR") + ".jpg", ImageFormat.Jpeg);
            //}

            //m.Save(strlogpath + "\\" + JzTimes.DateTimeSerialString + ".jpg", ImageFormat.Jpeg);

            //if (!Directory.Exists(RESULT.picturePath))
            //    Directory.CreateDirectory(RESULT.picturePath);

            //m.Save(RESULT.picturePath + "\\" + (RESULT.BARCODE == "" ? JzTimes.DateTimeSerialString + "_" + INI.FIXTUREID + "_NULLSN_OCR" : JzTimes.DateTimeSerialString + "_" + INI.FIXTUREID + "_" + RESULT.BARCODE + "_OCR") + ".jpg", ImageFormat.Jpeg);
            //m.Dispose();
        }

        public void FillDisplay()
        {
            lblPass.Text = "PASS";
            lblDuriation.Text = "";

            //txtOp.Text = RUNDB.OPID;
            //txtResource.Text = RUNDB.ResourceID;

        }

        public void ShinningPause()
        {
            ShinningProcess.Pause();
        }
        public void ShinningContinue()
        {
            ShinningProcess.Continue();
        }

        public void AddInspectCurrentStrip(bool ispass)
        {
            JCETSFUI.AddInspectCurrentStrip(ispass);
        }

        bool IsSet = false;
        public void Tick()
        {
            if (myTimes.msDuriation > ShinningDuriation)
            {
                #region 是否進行復判
                //if (INI.ISJUDGEPASS && INI.JUDGE)
                //{
                //    if (PLC.IsJudge && !IsSet)
                //    {
                //        IsSet = true;
                //    }

                //    if (myJudgeTimes.msDuriation >= INI.JUDGEDELAYTIME)
                //    {
                //        INI.JUDGE = false;

                //        if (IsSet)
                //        {
                //            IsSet = false;
                //            Universal.IsManualpass = true;
                //        }

                //        ShinningContinue();
                //    }
                //}
                #endregion

                switch (Universal.OPTION)
                {
                    case OptionEnum.MAIN_SDM5:

                        btnReadyBK(((ControlSpace.MachineSpace.JzMainSDM5MachineClass)Universal.MACHINECollection.MACHINE).PLCIO.Ready);
                        

                        break;
                    case OptionEnum.MAIN_X6:

                        if (!Universal.IsUseMappingUI)
                        {
                            JCETSFUI.Visible = INI.JCET_IS_USE_SHOPFLOOR;
                            JCETSFUI.Tick();
                        }

                        break;
                    case OptionEnum.MAIN_SDM3:

                        btnReadyBK(((ControlSpace.MachineSpace.JzMainSDM3MachineClass)Universal.MACHINECollection.MACHINE).PLCIO.Ready);
                        btnBYPASSBK(((ControlSpace.MachineSpace.JzMainSDM3MachineClass)Universal.MACHINECollection.MACHINE).PLCIO.Pass);


                        break;
                    case OptionEnum.MAIN_SDM2:

                        btnReadyBK(((ControlSpace.MachineSpace.JzMainSDM2MachineClass)Universal.MACHINECollection.MACHINE).PLCIO.Ready);
                        btnBYPASSBK(((ControlSpace.MachineSpace.JzMainSDM2MachineClass)Universal.MACHINECollection.MACHINE).PLCIO.Pass);


                        break;
                    case OptionEnum.MAIN_SDM1:
                        btnReadyBK(((ControlSpace.MachineSpace.JzMainSDM1MachineClass)Universal.MACHINECollection.MACHINE).PLCIO.Ready);
                        btnBYPASSBK(((ControlSpace.MachineSpace.JzMainSDM1MachineClass)Universal.MACHINECollection.MACHINE).PLCIO.Pass);

                        break;
                }

                ShinningTick();
                myTimes.Cut();
            }
        }
        public void StartShinnig(bool isresultpass)
        {
            IsResultPass = isresultpass;
            ShinningProcess.Start();
        }
        public void SetDuriation(string inputstr)
        {
            lblDuriation.Text = inputstr;
            lblDuriation.Invalidate();
        }

        public void SaveLog(string rptstr, string savename, string rcpname, string rcpver)
        {
            RPTUI.LogRecord(rptstr, savename, rcpname, rcpver);
        }

        public bool IsShinning
        {
            get
            {
                return ShinningProcess.IsOn;
            }
        }

        public void SetRunUING()
        {
            lblPass.Text = "NG";
            lblBigPass.Text = "NG";
            lblPass.ForeColor = Color.Yellow;
            lblBigPass.ForeColor = Color.Yellow;
            lblPass.BackColor = Color.DarkRed;
            lblBigPass.BackColor = Color.DarkRed;
            lblPass.Refresh();
            lblBigPass.Refresh();

        }

        int ShinigCount = 0;
        ProcessClass ShinningProcess = new ProcessClass();
        public void ShinningTick()
        {
            ProcessClass Process = ShinningProcess;

            if (Process.IsOn)
            {
                switch (Process.ID)
                {
                    case 5:

                        //lblBigPass.Visible = IsPass;
                        if (ShinigCount == 0)
                        {
                            Process.TimeUnit = TimeUnitEnum.ms;
                            lblBigPass.Text = (IsResultPass ? "PASS" : "NG");
                            lblPass.Text = (IsResultPass ? "PASS" : "NG");
                        }

                        if (ShinigCount == 0 || Process.IsTimeup)
                        {
                            lblBigPass.ForeColor = (IsResultPass ? Color.Lime : Color.Red);
                            lblPass.ForeColor = (IsResultPass ? Color.Lime : Color.Red);

                            //if (IsResultPass)
                            //    ShineGreen();
                            //else
                            //    ShineRed();

                            lblBigPass.Invalidate();
                            lblPass.Invalidate();

                            Process.ID = 10;
                            Process.NextDuriation = 100;
                        }
                        break;
                    case 10:
                        if (Process.IsTimeup)
                        {

                            lblBigPass.ForeColor = (IsResultPass ? Color.Green : Color.DarkRed);
                            lblPass.ForeColor = (IsResultPass ? Color.Green : Color.DarkRed);

                            //ShineNothing();

                            lblBigPass.Invalidate();
                            lblPass.Invalidate();

                            ShinigCount++;

                            if (ShinigCount > ShiningTimes)
                            {

                                ShinigCount = 0;
                                //OnTrigger((IsPass ? StatusEnum.CALPASS : StatusEnum.CALNG));

                                //OnTrigger(StatusEnum.CALEND);
                                OnTrigger(RunStatusEnum.SHINNIGEND);

                                Process.Stop();
                            }
                            else
                                Process.ID = 5;
                        }
                        break;
                }
            }
        }

        public delegate void TriggerHandler(RunStatusEnum runstatus);
        public event TriggerHandler TriggerAction;
        public void OnTrigger(RunStatusEnum runstatus)
        {
            if (TriggerAction != null)
            {
                TriggerAction(runstatus);
            }
        }

        public delegate void BarcodeHandler(string barcode);
        public event BarcodeHandler BarcodeAction;
        public void OnBarcode(string barcode)
        {
            if (BarcodeAction != null)
            {
                BarcodeAction(barcode);
            }
        }

        public delegate void LearningHandler(PassInfoClass passinfo, LearnOperEnum learnoper);
        public event LearningHandler LearnAction;
        public void OnLearn(PassInfoClass passinfo, LearnOperEnum learnoper)
        {
            if (LearnAction != null)
            {
                LearnAction(passinfo, learnoper);
            }
        }

        private void btnBypass_Click(object sender, EventArgs e)
        {
            switch (OPTION)
            {
                case OptionEnum.MAIN_SDM3:
                    OnTrigger(RunStatusEnum.SDM3_BYPASS);
                    break;
                case OptionEnum.MAIN_SDM2:
                    OnTrigger(RunStatusEnum.SDM2_BYPASS);
                    break;
                case OptionEnum.MAIN_SDM1:
                    OnTrigger(RunStatusEnum.SDM1_BYPASS);
                    break;
            }
        }

        void init_Display()
        {
            //DS = dispUI1;
            DS.Initial(100, 0.01f);
            DS.SetDisplayType(JzDisplay.DisplayTypeEnum.SHOW);
            //DS.SetImode(-1);

            //m_DispUI.MoverAction += M_DispUI_MoverAction;
            //m_DispUI.AdjustAction += M_DispUI_AdjustAction;
        }
        void update_Display(bool eChangeToDefault = true)
        {
            DS.Refresh();
            if (eChangeToDefault)
                DS.DefaultView();
        }
    }
}
