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
using JzOCR.OPSpace;
using MoveGraphLibrary;
//调用DLL所需的命名空间
using System.Runtime.InteropServices;
using System.Diagnostics;
using JetEazy;
using OCRByPaddle;

namespace JzOCR.UISpace
{
    public partial class OcrUI : UserControl
    {
       // JzOCR.BasicSpace.KeyBoardhook KeyBoardhook;

        /// <summary>
        /// UI按键信息
        /// </summary>
        enum TagEnum
        {
            /// <summary>
            /// 增加一个强制截图学习功能
            /// </summary>
            ADDMETHOD,
            /// <summary>
            /// 删除一个强制截图学习功能
            /// </summary>
            DELMETHOD,
            /// <summary>
            /// 试算
            /// </summary>
            TEST,
            /// <summary>
            /// 直接取样
            /// </summary>
            DIRECTGET,
            /// <summary>
            /// 取样
            /// </summary>
            GET,
            /// <summary>
            /// 更取样
            /// </summary>
            GETRRR,
            /// <summary>
            /// 增加1个字元学习
            /// </summary>
            ADDONE,
            /// <summary>
            /// 增加列表中所有字元学习
            /// </summary>
            ADDALL,
            /// <summary>
            /// 删除已学习字元
            /// </summary>
            DELITEM,
            /// <summary>
            /// 删除已学习所有字元
            /// </summary>
            DELITEMALL,
            /// <summary>
            /// 下一个测试图片
            /// </summary>
            NEXT,
            /// <summary>
            /// 自动测试下一个图片
            /// </summary>
            AUTOTEST,
            /// <summary>
            /// 上一步
            /// </summary>
            Return,

            /// <summary>
            /// 合并（连体字）
            /// </summary>
            LIANTIE,

            /// <summary>
            /// AI解码
            /// </summary>
            AiDecode,

            /// <summary>
            /// AI2解码
            /// </summary>
            Ai2Decode,
        }

       public DispUI SHOWDISP;
        DispUI METHODISP;
        Mover ShowMover = new Mover();
        Mover MethodMover = new Mover();
        OCRClass OCR;
     
        bool ISQSMC = true;
         List<ColorMessage> myColor;
        class ColorMessage
        {
            public string Name = "";
            public string[] mySnColorList ;
        }
        

        PropertyGrid ppgMethod;
        FlowLayoutPanel flpOnlineOCRItem;
        FlowLayoutPanel flpOCRItem;
        GroupBox gbPar;
        GroupBox gbImageChanged;
        CheckBox cbJetOCR;
        CheckBox cbNoParOCR;
        CheckBox cbDefect;
        CheckBox cbML;
        CheckBox cbImagePlus;
        NumericUpDown nudPoint;
        NumericUpDown nudArea;
        NumericUpDown nudColorDifference;
        NumericUpDown nudDifference;
        NumericUpDown nudExcludeScore;
        NumericUpDown nudBright;
        NumericUpDown nudContrast;

        TextBox tbSNTemp;

        /// <summary>
        /// 强制学习列表
        /// </summary>
        ListBox lstMethod;
        /// <summary>
        /// 增加一个强制截图学习功能
        /// </summary>
        Button btnAddMethod;
        /// <summary>
        /// 删除一个强制截图学习功能
        /// </summary>
        Button btnDelMethod;
        /// <summary>
        /// 试算
        /// </summary>
        Button btnTest;
        /// <summary>
        /// 直接取样
        /// </summary>
        Button btnDirectGet;
        /// <summary>
        /// 取样
        /// </summary>
        Button btnGet;
        Button btnLianTie;
        /// <summary>
        /// 硬取样
        /// </summary>
        Button btnGetRRR;
        /// <summary>
        /// 增加1个字元学习
        /// </summary>
        Button btnAddOne;
        /// <summary>
        /// 增加列表中所有字元学习
        /// </summary>
        Button btnAddAll;
        /// <summary>
        /// 删除已学习字元
        /// </summary>
        Button btnDelItem;
        /// <summary>
        /// 删除已学习所有字元
        /// </summary>
        Button btnDelALLItem;
        /// <summary>
        /// 下一个测试图片
        /// </summary>
        Button btnNext;
        /// <summary>
        /// 上一步
        /// </summary>
        Button btnReturn;
        /// <summary>
        /// 自动测试下一个图片
        /// </summary>
        Button btnAutoTest;

        Button btnAiDecode;
        Button btnAi2Decode;
        /// <summary>
        /// 自动测试用
        /// </summary>
        Timer mytimer;
        /// <summary>
        /// 测试时用到的文件序号
        /// </summary>
        int index = 0;
        /// <summary>
        /// 测试时保存的文件地址
        /// </summary>
        string[] strFileNameS;
        Bitmap bmpMyTest = null;

        bool IsNeedToChange;
        bool isNext = false;
        bool isReturn = false;
        bool ISAUTOMATION = false;
        /// <summary>
        /// ctrl 是否被按下
        /// </summary>
      public static  bool isCtrlDeyDown = false;

        /// <summary>
        /// 返回强制学习列表选中编号
        /// </summary>
        int OCRMethodIndex
        {
            get
            {
                return lstMethod.SelectedIndex;
            }
        }
        public OcrUI()
        {
            InitializeComponent();
            InitialInside();
        }

        void InitialInside()
        {
            btnAddMethod = button11;
            btnDelMethod = button10;

            lstMethod = listBox1;

            btnTest = button1;
            btnAiDecode= button15;
            btnAi2Decode = button16;
            btnDirectGet = button6;
            btnGet = button5;
            btnLianTie = button14;
            btnGetRRR = button12;
            btnAddOne = button3;
            btnAddAll = button2;
            btnDelItem = button4;
            btnDelALLItem = button13;

            flpOnlineOCRItem = flowLayoutPanel1;
            flpOCRItem = flowLayoutPanel2;
            cbDefect = checkBox4;
            cbML = checkBox2;
            cbImagePlus = checkBox5;

            tbSNTemp = textBox1;

            //SHOWDISP = dispUI1;
            METHODISP = dispUI2;
            ppgMethod = myPropertyGrid1;

            btnAddMethod.Tag = TagEnum.ADDMETHOD;
            btnDelMethod.Tag = TagEnum.DELMETHOD;
            btnTest.Tag = TagEnum.TEST;
            btnDirectGet.Tag = TagEnum.DIRECTGET;
            btnLianTie.Tag = TagEnum.LIANTIE;
            btnGet.Tag = TagEnum.GET;
            btnGetRRR.Tag = TagEnum.GETRRR;
            btnAddOne.Tag = TagEnum.ADDONE;
            btnAddAll.Tag = TagEnum.ADDALL;
            btnDelItem.Tag = TagEnum.DELITEM;
            btnDelALLItem.Tag = TagEnum.DELITEMALL;
            btnAiDecode.Tag = TagEnum.AiDecode;
            btnAi2Decode.Tag = TagEnum.Ai2Decode;
            btnNext = button7;
            btnNext.Tag = TagEnum.NEXT;
            btnAutoTest = button8;
            btnAutoTest.Tag = TagEnum.AUTOTEST;
            btnReturn = button9;
            btnReturn.Tag = TagEnum.Return;

            btnReturn.Click += btn_Click;
            btnNext.Click += btn_Click;
            btnAutoTest.Click += btn_Click;
            btnAiDecode.Click += btn_Click;
            btnAddMethod.Click += btn_Click;
            btnDelMethod.Click += btn_Click;
            btnTest.Click += btn_Click;
            btnDirectGet.Click += btn_Click;
            btnGet.Click += btn_Click;
            btnLianTie.Click+= btn_Click;
            btnGetRRR.Click += btn_Click;
            btnAddOne.Click += btn_Click;
            btnAddAll.Click += btn_Click;
            btnDelItem.Click += btn_Click;
            btnDelALLItem.Click += btn_Click;
            btnAi2Decode.Click += btn_Click;

            init_Display();
            update_Display();

            //SHOWDISP.Initial();
            //SHOWDISP.SetDisplayType(DisplayTypeEnum.NORMAL);
            METHODISP.Initial();
            METHODISP.SetDisplayType(DisplayTypeEnum.NORMAL);


            // IsNeedToChange = true;
            gbImageChanged = groupBox3;
            gbPar = groupBox1;
            cbJetOCR = checkBox1;
            cbNoParOCR = checkBox3;
            nudBright = numericUpDown4;
            nudContrast = numericUpDown5;
            nudColorDifference = numericUpDown1;
            nudDifference = numericUpDown2;
            nudExcludeScore = numericUpDown3;
            nudArea = numericUpDown9;
            nudPoint = numericUpDown8;

            nudArea.ValueChanged += NudArea_ValueChanged;
            nudPoint.ValueChanged += NudPoint_ValueChanged;
            nudBright.ValueChanged += NudBright_ValueChanged;
            nudColorDifference.ValueChanged += NudColorDifference_ValueChanged;
            nudContrast.ValueChanged += NudContrast_ValueChanged;
            nudDifference.ValueChanged += NudDifference_ValueChanged;
            nudExcludeScore.ValueChanged += NudExcludeScore_ValueChanged;

            cbNoParOCR.CheckedChanged += CbNoParOCR_CheckedChanged; ;
            cbJetOCR.CheckedChanged += CbJetOCR_CheckedChanged;
            cbDefect.CheckedChanged += CbDefect_CheckedChanged;
            cbML.CheckedChanged += CbML_CheckedChanged;
            cbImagePlus.CheckedChanged += CbImagePlus_CheckedChanged;
            flowLayoutPanel2.MouseDoubleClick += FlowLayoutPanel2_MouseDoubleClick;



            lstMethod.SelectedIndexChanged += lstMethod_SelectedIndexChanged;
            // lstMethod.Height = 544;

            //   KeyBoardhook = new BasicSpace.KeyBoardhook();
            //   KeyBoardhook.KeyDownEvent += KeyBoardhook_KeyDownEvent;
            //   KeyBoardhook.KeyUpEvent += KeyBoardhook_KeyUpEvent;
            ////   KeyBoardhook.KeyPressEvent += KeyBoardhook_KeyPressEvent;
            //   KeyBoardhook.Start();//安装键盘钩子

            this.SizeChanged += OcrUI_SizeChanged;
            
            mytimer = new Timer();
            mytimer.Interval = 20;
            mytimer.Tick += new EventHandler(mytimer_Tick);
            mytimer.Start();
        }

        private void OcrUI_SizeChanged(object sender, EventArgs e)
        {
            update_Display();
        }

        private void CbImagePlus_CheckedChanged(object sender, EventArgs e)
        {
            OCR.isImagePlus = cbImagePlus.Checked;
        }

        private void CbML_CheckedChanged(object sender, EventArgs e)
        {
           OCR.isML = cbML.Checked;
        }
        OCRCollectionClass OCRCollection;
        OCRByPaddle.OCRByPaddle mOCRByPaddle;
        public void Initial( OCRCollectionClass OCRC  , OCRByPaddle.OCRByPaddle Paddle)
        {
            OCR = OCRC.DataNow;
            mOCRByPaddle= Paddle;
            OCRCollection = OCRC;

            IsNeedToChange = false;

            ShowMover.Clear();
            lstMethod.Items.Clear();

            ShowMover.Add(OCR.RectEAG);
            SHOWDISP.SetMover(ShowMover);

            SHOWDISP.ReplaceDisplayImage(OCR.bmpLast);

            foreach (OCRMethdClass ocrmethod in OCR.OCRMethodList)
            {
                lstMethod.Items.Add(ocrmethod.ToMethodIndexString());
            }

            nudBright.Value = OCR.iBright;
            nudContrast.Value = OCR.iContrast;
            nudColorDifference.Value = OCR.iColorDifference;
            nudDifference.Value = (decimal)OCR.fDifference;
            nudExcludeScore.Value = (decimal)OCR.fExcludeScore;
            cbJetOCR.Checked = OCR.isJetOCR;
            cbNoParOCR.Checked = OCR.isNoParOCR;
            cbImagePlus.Checked = OCR.isImagePlus;
            cbDefect.Checked = OCR.isDefect;
            cbML.Checked = OCR.isML;
            nudArea.Value = OCR.iArea;
            nudPoint.Value = OCR.iPoint;

            if (lstMethod.Items.Count > 0)
            {
                lstMethod.SelectedIndex = -1;
                lstMethod.SelectedIndex = 0;
            }

            DisplayOCRPar();
        }

        public void MyTest()
        {
            btnNext.Visible = true;
            btnAutoTest.Visible = true;
            btnReturn.Visible = true;
            string subPath = @"D:\LOA\OCR\";
            if (!System.IO.Directory.Exists(subPath))
                //创建pic文件夹
                System.IO.Directory.CreateDirectory(subPath);
            

            strFileNameS = System.IO.Directory.GetFiles(subPath, "*.png");

            if (ISQSMC)
            {
                subPath = "D://Jeteazy//OCR//";
                if (!System.IO.Directory.Exists(subPath))
                    //创建pic文件夹
                    System.IO.Directory.CreateDirectory(subPath);

                System.IO.FileStream fs = new System.IO.FileStream("D://Jeteazy//OCR//COLORTABLE.jdb", System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.None);
                System.IO.StreamReader Srr = new System.IO.StreamReader(fs, Encoding.Default);
                string DataStr = Srr.ReadToEnd().Replace(Environment.NewLine, "{");

                myColor = new List<ColorMessage>();
                string[] strMe = DataStr.Split('{');
                foreach (string strM in strMe)
                {
                    if (strM != "")
                    {
                        string[] strDa = strM.Split(':');
                        if (strDa.Length > 1)
                        {
                            ColorMessage color = new ColorMessage();
                            color.Name = strDa[0];
                            color.mySnColorList = strDa[1].Split(',');
                            myColor.Add(color);
                        }
                    }
                }
                Srr.Close();
                Srr.Dispose();
            }
            

        }

        private void lstMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstMethod.SelectedIndex != -1)
            {
                OCRMethdClass ocrmethod = OCR.OCRMethodList[lstMethod.SelectedIndex];
                METHODISP.ClearAll();
                MethodMover.Clear();
                MethodMover.Add(ocrmethod.RectEAG);
                METHODISP.SetMover(MethodMover);
                METHODISP.ReplaceDisplayImage(ocrmethod.bmpMethod);
            }

            if (!IsNeedToChange)
                return;

            FillDisplay(lstMethod.SelectedIndex);

            bmpMyTest = null;
        }
        private void NudPoint_ValueChanged(object sender, EventArgs e)
        {
            OCR.iPoint = (int)nudPoint.Value;
        }

        private void NudArea_ValueChanged(object sender, EventArgs e)
        {
            OCR.iArea = (int)nudArea.Value;
        }
        private void NudExcludeScore_ValueChanged(object sender, EventArgs e)
        {
            OCR.fExcludeScore = (float)nudExcludeScore.Value;
        }

        private void NudDifference_ValueChanged(object sender, EventArgs e)
        {
            OCR.fDifference = (float)nudDifference.Value;
        }

        private void NudContrast_ValueChanged(object sender, EventArgs e)
        {
            OCR.iContrast = (int)nudContrast.Value;

            if (lstMethod.SelectedIndex != -1)
            {
                OCRMethdClass ocrmethod = OCR.OCRMethodList[lstMethod.SelectedIndex];
                BrightCotrast(ocrmethod.bmpMethod, OCR.iBright, OCR.iContrast);
            }
        }

        private void NudColorDifference_ValueChanged(object sender, EventArgs e)
        {
            OCR.iColorDifference = (int)nudColorDifference.Value;
        }

        private void NudBright_ValueChanged(object sender, EventArgs e)
        {
            OCR.iBright = (int)nudBright.Value;
            if (lstMethod.SelectedIndex != -1)
            {
                OCRMethdClass ocrmethod = OCR.OCRMethodList[lstMethod.SelectedIndex];

                BrightCotrast(ocrmethod.bmpMethod, OCR.iBright, OCR.iContrast);
            }
        }

        private void CbJetOCR_CheckedChanged(object sender, EventArgs e)
        {
            OCR.isJetOCR = cbJetOCR.Checked;
        }

        private void CbNoParOCR_CheckedChanged(object sender, EventArgs e)
        {
            OCR.isNoParOCR = cbNoParOCR.Checked;
        }
        private void CbDefect_CheckedChanged(object sender, EventArgs e)
        {
            OCR.isDefect = cbDefect.Checked;
        }
        private void FlowLayoutPanel2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (flpOCRItem.Width < 300)
                {

                    flpOCRItem.Location = flpOnlineOCRItem.Location;
                    flpOCRItem.Width = 166+609-411;
                   // gbPar.Visible = true;
                   // lstMethod.Height = 256;

                    flpOCRItem.Controls.Clear();
                    foreach (OCRItemClass ocritem in OCR.OCRItemList)
                    {
                        //Fill OCRItem
                        OCRConter ui = new OCRConter();
                        ui.Width = 192;
                        ui.OCRSet = ocritem;
                        ui.ONClick += Ui_ONClick;
                        flpOCRItem.Controls.Add(ui);
                    }

                }
                else
                {
                    Restore();
                    //flpOCRItem.Location = new Point(976, 305);
                    //flpOCRItem.Width = 177;
                    //gbPar.Visible = false;
                    //lstMethod.Height = 544;

                    //flpOCRItem.Controls.Clear();
                    //foreach (OCRItemClass ocritem in OCR.OCRItemList)
                    //{
                    //    //Fill OCRItem
                    //    OCRConter ui = new OCRConter();

                    //    ui.OCRSet = ocritem;
                    //    flpOCRItem.Controls.Add(ui);
                    //}
                }
            }
        }


        private void btn_Click(object sender, EventArgs e)
        {
            TagEnum KEY = (TagEnum)((Button)sender).Tag;

            switch (KEY)
            {
                case TagEnum.ADDMETHOD:
                    AddMethod();
                    break;
                case TagEnum.DELMETHOD:
                    DelMethod();
                    break;
                case TagEnum.TEST:
                    //   SetDefaultView();
                    TestInOCR();
                    break;
                case TagEnum.DIRECTGET:
                    DIRECTGET();
                    break;
                case TagEnum.GET:
                    GETPar(false);
                    break;
                case TagEnum.LIANTIE:
                    LIANTIEPar(false);
                    break;
                case TagEnum.GETRRR:
                    GETPar(true);
                    break;
                case TagEnum.ADDONE:
                    ADDONEMethd();
                    DisplayOCRPar();
                    break;
                case TagEnum.ADDALL:
                    ADDALLMethd();
                    DisplayOCRPar();
                    break;
                case TagEnum.DELITEM:
                    DELITEM();
                    DisplayOCRPar();
                    break;
                case TagEnum.DELITEMALL:
                    DELITEMALL();
                    DisplayOCRPar();
                    break;
                case TagEnum.NEXT:
                    TestNext();
                    break;
                case TagEnum.Return:
                    TestReturn();
                    break;
                case TagEnum.AUTOTEST:
                    if (!ISAUTOMATION)
                        ISAUTOMATION = true;
                    else
                        ISAUTOMATION = false;
                    break;
                case TagEnum.AiDecode:
                    TestAiOCR();
                    break;
                case TagEnum.Ai2Decode:
                    TestAi2OCR();
                    break;
            }
            SetEnable(true);
        }

        /// <summary>
        /// 把选中参数删除
        /// </summary>
        void DELITEM()
        {
            foreach (OCRItemClass ocrTrem in OCR.OCRItemList)
            {
                if (ocrTrem.isSelected)
                {
                    OCR.OCRItemList.Remove(ocrTrem);
                    DELITEM();
                    break;
                }
            }
            OCR.TrainPar();
        }
        /// <summary>
        /// 把选中参数删除
        /// </summary>
        void DELITEMALL()
        {
            OCR.OCRItemList.Clear();
            OCR.TrainPar();
        }
        /// <summary>
        /// 把选中的临时参数加载进来
        /// </summary>
        void ADDONEMethd()
        {
            OCRMethdClass ocrmethod;
            if (lstMethod.SelectedIndex > -1 && lstMethod.SelectedIndex < OCR.OCRMethodList.Count)
                ocrmethod = OCR.OCRMethodList[lstMethod.SelectedIndex];
            else
            {
                if (MessageBox.Show("请选择列表中的一个参数并取样后再继续……", "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
                else
                    return;
            }

            foreach (OCRItemClass ocrTrem in OCR.OCROnlineItemList)
            {
                if (ocrTrem.isSelected)
                {
                    int myindex = 0;
                    OCRItemClass myocr = ocrTrem.Clone();
                    foreach (OCRItemClass ocritem in OCR.OCRItemList)
                    {
                        if (myindex < ocritem.No)
                            myindex = ocritem.No;
                    }
                    int iNo = myindex + 1;
                    myocr.No = iNo;
                    OCR.OCRItemList.Add(myocr);
                    ocrTrem.isSelected = false;
                }
            }
            OCR.TrainPar();
            //DisplayOCRPar();
        }
        /// <summary>
        /// 把临时参数全部加载进来
        /// </summary>
        void ADDALLMethd()
        {
            OCRMethdClass ocrmethod;
            if (lstMethod.SelectedIndex > -1 && lstMethod.SelectedIndex < OCR.OCRMethodList.Count)
                ocrmethod = OCR.OCRMethodList[lstMethod.SelectedIndex];
            else
            {
                if (MessageBox.Show("请选择列表中的一个参数并取样后再继续……", "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
                else
                    return;
            }

            foreach (OCRItemClass ocrTrem in OCR.OCROnlineItemList)
            {
                int MYindex = 0;
                OCRItemClass myocr = ocrTrem.Clone();
                foreach (OCRItemClass ocritem in OCR.OCRItemList)
                {
                    if (MYindex < ocritem.No)
                        MYindex = ocritem.No;
                }
                int iNo = MYindex + 1;
                myocr.No = iNo;
                OCR.OCRItemList.Add(myocr);
                ocrTrem.isSelected = false;

            }
            OCR.TrainPar();
            //DisplayOCRPar();

        }
        /// <summary>
        /// 拆分图片字符，并变成参数
        /// </summary>
      /// <param name="isRRR">是否强制</param>
        void GETPar(bool isRRR)
        {
            float fScore = OCR.fExcludeScore;
            if (isRRR)
                OCR.fExcludeScore = 1;

            Bitmap BmpTemp = null;
            if (bmpMyTest == null)
            {
                OCRMethdClass ocrmethod;
                if (lstMethod.SelectedIndex > -1 && lstMethod.SelectedIndex < OCR.OCRMethodList.Count)
                    ocrmethod = OCR.OCRMethodList[lstMethod.SelectedIndex];
                else
                {
                    if (MessageBox.Show("请选择列表中的一个参数后再继续……", "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                        return;
                    else
                        return;
                }

                BmpTemp = new Bitmap(ocrmethod.GetBitmMap);

                OCR.OCROnlineItemList.Clear();
                //     OCR.OCROnlineItemList = OCR.SplitToOCRTrain(ocrmethod.bmpMethod, ref BmpTemp);
                if (tbSNTemp.Text == "")
                    OCR.OCROnlineItemList = OCR.SplitToOCRTrainSet(ocrmethod.bmpMethod);
                else
                {
                    Bitmap mybmpErr = new Bitmap(ocrmethod.bmpMethod);
                    OCRItemClass[] oCRItems = new OCRItemClass[tbSNTemp.Text.Length];
                    string strMess = OCR.OCRRUNLINEAURO(tbSNTemp.Text, ocrmethod.bmpMethod, ref mybmpErr, ref oCRItems);

                    for (int i = 0; i < oCRItems.Length; i++)
                        OCR.OCROnlineItemList.Add(oCRItems[i]);
                } 
            }
            else
            {
                BmpTemp = new Bitmap(bmpMyTest);
                OCR.OCROnlineItemList.Clear();
               // OCR.OCROnlineItemList = OCR.SplitToOCRTrain(bmpMyTest, ref BmpTemp);

                OCR.OCROnlineItemList = OCR.SplitToOCRTrainSet(bmpMyTest);

            }
            if (isRRR)
                OCR.fExcludeScore = fScore;

            DisplayOCRPar();

            //METHODISP.ClearAll();
            //METHODISP.ReplaceDisplayImage(BmpTemp);
        }


        /// <summary>
        /// 拆分图片字符，并变成参数
        /// </summary>
        /// <param name="isRRR">是否强制</param>
        void LIANTIEPar(bool isRRR)
        {
            float fScore = OCR.fExcludeScore;
            if (isRRR)
                OCR.fExcludeScore = 1;

            Bitmap BmpTemp = null;
            if (bmpMyTest == null)
            {
                OCRMethdClass ocrmethod;
                if (lstMethod.SelectedIndex > -1 && lstMethod.SelectedIndex < OCR.OCRMethodList.Count)
                    ocrmethod = OCR.OCRMethodList[lstMethod.SelectedIndex];
                else
                {
                    if (MessageBox.Show("请选择列表中的一个参数后再继续……", "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                        return;
                    else
                        return;
                }

                BmpTemp = new Bitmap(ocrmethod.GetBitmMap);

                OCR.OCROnlineItemList.Clear();
                //     OCR.OCROnlineItemList = OCR.SplitToOCRTrain(ocrmethod.bmpMethod, ref BmpTemp);
                if (tbSNTemp.Text == "")
                    OCR.OCROnlineItemList = OCR.SplitToOCRTrainLianTie(ocrmethod.bmpMethod);
                else
                {
                    Bitmap mybmpErr = new Bitmap(ocrmethod.bmpMethod);
                    OCRItemClass[] oCRItems = new OCRItemClass[OCR.OCRMethodList.Count];
                    string strMess = OCR.OCRRUNLINEAURO(tbSNTemp.Text, ocrmethod.bmpMethod, ref mybmpErr, ref oCRItems);

                    for (int i = 0; i < oCRItems.Length; i++)
                        OCR.OCROnlineItemList.Add(oCRItems[i]);
                }
            }
            else
            {
                BmpTemp = new Bitmap(bmpMyTest);
                OCR.OCROnlineItemList.Clear();
                // OCR.OCROnlineItemList = OCR.SplitToOCRTrain(bmpMyTest, ref BmpTemp);

                OCR.OCROnlineItemList = OCR.SplitToOCRTrainLianTie(bmpMyTest);

            }
            if (isRRR)
                OCR.fExcludeScore = fScore;

            DisplayOCRPar();

            //METHODISP.ClearAll();
            //METHODISP.ReplaceDisplayImage(BmpTemp);
        }

        /// <summary>
        /// 直接取图到参数中去
        /// </summary>
        void DIRECTGET()
        {
            OCR.OCROnlineItemList.Clear();
            Bitmap bmp = new Bitmap(1, 1);
            METHODISP.GenSearchImage(ref bmp);
            if (OCR.isImagePlus)
                bmp=OCR.ApplyFilter(new AForge.Imaging.Filters.ContrastStretch(), bmp);
            int ibackColor = 0;
            Bitmap bmpT = new Bitmap(bmp);
            int iThr = JetEazy.BasicSpace.myImageProcessor.Balance(bmpT, ref bmpT, ref ibackColor, JetEazy.BasicSpace.myImageProcessor.EnumThreshold.Intermodes);
            bmpT.Dispose();
            OCRItemClass myocr = new OCRItemClass();
            myocr.bmpItem = bmp;
            myocr.strRelateName = "?";
            myocr.iBackColor = ibackColor;
            OCR.OCROnlineItemList.Add(myocr);

            DisplayOCRPar();
        }
        //导入dll文件
        [DllImport("user32.dll")]
        //函数声明读取 按键的 Ascll码，看对应的建是不是被按下
        public static extern int GetAsyncKeyState(int vKey);
        void mytimer_Tick(object sender, EventArgs e)
        {
            //17==Ctrl 13==Enter
            if (GetAsyncKeyState(17) != 0)
                isCtrlDeyDown = true;
            else
                isCtrlDeyDown = false;
            
            //for(int I=0;I<255;I++)
            //{
            //    if (GetAsyncKeyState(I) != 0)
            //        isCtrlDeyDown = true;
            //}
            if (ISAUTOMATION)
                TestNext();
        }

        void TestReturn()
        {
            
            if (strFileNameS.Length > 0)
            {
                isReturn = true;
                if (isNext)
                {
                    index-=2;
                    isNext = false;
                }

                if (index < 0)
                    index = 0;

                int iCount = 0;
                while (true)
                {
                   
                    if (strFileNameS.Length > index)
                    {
                        string strFile = strFileNameS[index];

                        bool isRun = false;
                        if (ISQSMC)
                        {
                       string[]  myColorTemp=     System.IO.Path.GetFileNameWithoutExtension(strFile).Split('_');

                            string colorBace = myColorTemp[1];
                            int indexC = colorBace.IndexOf('(');
                            colorBace = colorBace.Substring(0, indexC);

                            if (colorBace == OCR.Name)
                            {
                                //foreach (ColorMessage mysn in myColor)
                                //{
                                //    if (mysn.Name == OCR.Name)
                                //    {
                                //        //foreach (string str in mysn.mySnColorList)
                                //        //{
                                //        //    if (colorBace == str)
                                //        //    {
                                isRun = true;
                               //break;
                                //        //    }
                                //        //}
                                //    }
                                //    if (isRun)
                                //        break;
                                //}
                            }
                        }
                        else
                        {
                            if (strFile.IndexOf("-" + OCR.Name) > -1)
                                isRun = true;

                        }

                        if (isRun)
                        {
                            TestInOCR(strFileNameS[index]);
                            index--;
                            break;
                        }
                        index--;
                    }
                    else
                        index = 0;

                    if (index < 0)
                        index = strFileNameS.Length - 1;

                    iCount++;
                    if (iCount > strFileNameS.Length)
                        break;
                }
            }

        }
        void TestNext()
        {
         
            if (strFileNameS.Length > 0)
            {
                isNext = true;
                if (isReturn)
                {
                    index+=2;
                    isReturn = false;
                }
                int iCount = 0;
                while (true)
                {
                    if (strFileNameS.Length > index)
                    {
                        if (index == -1)
                            index = 0;

                        string strFile = strFileNameS[index];
                        bool isRun = false;
                        if (ISQSMC)
                        {
                            string[] myColorTemp = System.IO.Path.GetFileNameWithoutExtension(strFile).Split('_');

                            string colorBace = myColorTemp[1];
                            int indexC = colorBace.IndexOf('(');
                            if (indexC > 0)
                                colorBace = colorBace.Substring(0, indexC);

                            if (colorBace == OCR.Name)
                            {
                                //foreach (ColorMessage mysn in myColor)
                                //{
                                //    if (mysn.Name == OCR.Name)
                                //    {
                                //        //foreach (string str in mysn.mySnColorList)
                                //        //{
                                //        //    if (colorBace == str)
                                //        //    {
                                isRun = true;
                                //           break;
                                //        //    }
                                //        //}
                                //    }
                                //    if (isRun)
                                //        break;
                                //}
                            }
                        }
                        else
                        {
                            if (strFile.IndexOf("-" + OCR.Name) > -1)
                                isRun = true;
                        }

                        if (isRun)
                        {
                            TestInOCR(strFileNameS[index]);
                            index++;
                            break;
                        }
                        index++;
                    }
                    else
                    {
                        ISAUTOMATION = false;
                        index = 0;
                        MessageBox.Show("已跑线完成,再次点击会从头开始重新跑线!");
                        
                    }

                    iCount++;
                    if (iCount > strFileNameS.Length)
                        break;
                }
            }
        
        }
        /// <summary>
        /// 测试OCR
        /// </summary>
        void TestInOCR()
        {
            OCRMethdClass ocrmethod;

            if (lstMethod.SelectedIndex > -1 && lstMethod.SelectedIndex < OCR.OCRMethodList.Count)
                ocrmethod = OCR.OCRMethodList[lstMethod.SelectedIndex];
            else
            {
                if (MessageBox.Show("请选择列表中的一个参数后再继续……", "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
                else
                    return;
            }
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();

            Bitmap BmpTemp = new Bitmap(ocrmethod.bmpMethod);

            Bitmap bmpErr = null;
            bool isResult=true ;
            string strMess = "";
            if (tbSNTemp.Text == "")
                strMess = OCR.OCRRUNLINE(BmpTemp, ref bmpErr, ref isResult);
            else
            {
                OCRItemClass[] oCRItems = new OCRItemClass[OCR.OCRMethodList.Count];

                strMess = OCR.OCRRUNLINE(tbSNTemp.Text, BmpTemp, out bmpErr, out oCRItems);
             
                if(strMess!= tbSNTemp.Text )
                    strMess = OCR.OCRRUNLINEAURO(tbSNTemp.Text, BmpTemp, ref bmpErr, ref oCRItems);

            }

            timer.Stop();

            FormSpace.ShowResultForm show = new FormSpace.ShowResultForm(bmpErr, strMess, isResult);
            show.ShowDialog();
        }

        /// <summary>
        /// 测试OCR
        /// </summary>
        void TestAiOCR()
        {
            OCRMethdClass ocrmethod;

            if (lstMethod.SelectedIndex > -1 && lstMethod.SelectedIndex < OCR.OCRMethodList.Count)
                ocrmethod = OCR.OCRMethodList[lstMethod.SelectedIndex];
            else
            {
                if (MessageBox.Show("请选择列表中的一个参数后再继续……", "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
                else
                    return;
            }
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();

            Bitmap BmpTemp = new Bitmap(ocrmethod.bmpMethod);

            //Bitmap bmpErr = null;
            bool isResult = true;
            string strMess =OCRCollection.DecodePic(BmpTemp);

            timer.Stop();

            FormSpace.ShowResultForm show = new FormSpace.ShowResultForm(BmpTemp, strMess, isResult);
            show.ShowDialog();

            BmpTemp.Dispose();
        }
        /// <summary>
        /// 测试OCR
        /// </summary>
        void TestAi2OCR()
        {
            OCRMethdClass ocrmethod;

            if (lstMethod.SelectedIndex > -1 && lstMethod.SelectedIndex < OCR.OCRMethodList.Count)
                ocrmethod = OCR.OCRMethodList[lstMethod.SelectedIndex];
            else
            {
                if (MessageBox.Show("请选择列表中的一个参数后再继续……", "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
                else
                    return;
            }
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();

            Bitmap BmpTemp = new Bitmap(ocrmethod.bmpMethod);

            //Bitmap bmpErr = null;
            bool isResult = true;
            string strMess = mOCRByPaddle.OCR(BmpTemp); 

            timer.Stop();

            FormSpace.ShowResultForm show = new FormSpace.ShowResultForm(BmpTemp, strMess, isResult);
            show.ShowDialog();

            BmpTemp.Dispose();
        }

        /// <summary>
        /// 测试OCR
        /// </summary>
        void TestInOCR(string strFileName)
        {

            Bitmap BmpTemp = new Bitmap(strFileName);
            string strName = System.IO.Path.GetFileNameWithoutExtension(strFileName);
            strName = strName.Substring(0, 10);

            if (bmpMyTest != null)
            {
                bmpMyTest.Dispose();
                bmpMyTest = new Bitmap(BmpTemp);
            }
            else
                bmpMyTest = new Bitmap(BmpTemp);

            BmpTemp.Dispose();


            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            Bitmap bmpErr = null;
            //    bool isResult = true;
            //    string strMess = OCR.OCRRUNLINE(strName,bmpMyTest, ref bmpErr, ref isResult);
            OCRItemClass[] ocritemlist = new JzOCR.OPSpace.OCRItemClass[strName.Length];


            //string strMess  = OCRCollection.DecodePic(bmpMyTest);
            //if (strMess != strName)
            string strMess = OCR.OCRRUNLINE(strName, bmpMyTest, out bmpErr, out ocritemlist);
            timer.Stop();

            if (strName != strMess)
                ISAUTOMATION = false;

            Bitmap bmpShow = new Bitmap(bmpMyTest.Width + 50, bmpMyTest.Height + 100);
            Graphics g = Graphics.FromImage(bmpShow);
            g.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, bmpShow.Width, bmpShow.Height));
            g.DrawImage(bmpMyTest, new Point(0, 0));


            g.DrawString(strMess + " " + timer.ElapsedMilliseconds.ToString() + "ms", new Font("隶书", 30),
                          Brushes.Red,
                          new Point(0, bmpMyTest.Height));

            g.DrawString(strName + " " + index, new Font("隶书", 30),
                      Brushes.Blue,
                      new Point(0, bmpMyTest.Height + 50));

            g.Dispose();

            // SHOWDISP.ClearAll();
            SHOWDISP.ReplaceDisplayImage(bmpShow);

            METHODISP.ClearAll();
            METHODISP.ReplaceDisplayImage(bmpErr);
            if (bmpErr != null)
                bmpErr.Dispose();
            bmpShow.Dispose();

        }
        void AddMethod()
        {
            OCRMethdClass ocrmethod = new OCRMethdClass();

            if (OCR.OCRMethodList.Count > 0)
                ocrmethod.No = OCR.OCRMethodList[OCR.OCRMethodList.Count - 1].No + 1;
            else
                ocrmethod.No = 0;

            SHOWDISP.GenSearchImage(ref ocrmethod.bmpMethod);

            OCR.OCRMethodList.Add(ocrmethod);

            IsNeedToChange = false;

            lstMethod.Items.Add(ocrmethod.ToMethodIndexString());

            IsNeedToChange = true;

            lstMethod.SelectedIndex = OCR.OCRMethodList.Count - 1;

            SetEnable(true);
        }
        void DelMethod()
        {
            if (lstMethod.SelectedIndex > -1)
            {
                int i = lstMethod.SelectedIndex;

                OCRMethdClass ocrmethod = OCR.OCRMethodList[i];
                ocrmethod.Suicide();

                OCR.OCRMethodList.RemoveAt(i);

                IsNeedToChange = false;
                lstMethod.Items.RemoveAt(i);
                IsNeedToChange = true;

                if (i == lstMethod.Items.Count)
                    i--;

                lstMethod.SelectedIndex = i;
            }
        }
        void DisplayOCRPar()
        {
            flpOnlineOCRItem.Controls.Clear();
            flpOCRItem.Controls.Clear();

            for (int i =0;i< OCR.OCRItemList.Count; i++)
            {
                OCRItemClass ocra = OCR.OCRItemList[i];
                string strA = ocra.strRelateName;
                int ia = Asc(strA);
                for (int j = i+1; j < OCR.OCRItemList.Count; j++)
                {
                    OCRItemClass ocrb = OCR.OCRItemList[j];
                    string strB = ocrb.strRelateName;
                    int ib = Asc(strB);

                    if (ia > ib)
                    {
                        OCRItemClass TEMP = OCR.OCRItemList[i].Clone();
                        OCR.OCRItemList[i] = OCR.OCRItemList[j].Clone();
                        OCR.OCRItemList[j] = TEMP.Clone();

                        strA = OCR.OCRItemList[i].strRelateName;
                        ia = Asc(strA);
                    }

                }

            }


            foreach (OCRItemClass ocritem in OCR.OCRItemList)
            {
                //Fill OCRItem
                ocritem.isSelected = false;
                OCRConter ui = new OCRConter();
                ui.OCRSet = ocritem;
                ui.ONClick += Ui_ONClick;
                flpOCRItem.Controls.Add(ui);

            }
            foreach (OCRItemClass ocritem in OCR.OCROnlineItemList)
            {
                //Fill OCRItem
                ocritem.isSelected = false;
                OCRConter ui = new OCRConter();
                ui.OCRSet = ocritem;
                ui.ONClick += Ui_ONClick1;
                flpOnlineOCRItem.Controls.Add(ui);
            }


        }
        
        /// <summary>
        /// 字符转ASCII码：
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public  int Asc(string character)
        {
            if (character.Length == 1)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                int intAsciiCode = (int)asciiEncoding.GetBytes(character)[0];
                return (intAsciiCode);
            }
            else
            {
                return 300;
            }

        }
        public void SetEnable(bool isenable)
        {
            btnAddMethod.Enabled = isenable;
            btnDelMethod.Enabled = (isenable && OCR.OCRMethodList.Count > 0);
            btnTest.Enabled = (isenable && OCR.OCRMethodList.Count > 0);
            btnLianTie.Enabled = (isenable && OCR.OCRMethodList.Count > 0);

            btnDirectGet.Enabled = (isenable && OCR.OCRMethodList.Count > 0);
            btnGet.Enabled = (isenable && OCR.OCRMethodList.Count > 0);
            btnGetRRR.Enabled = (isenable && OCR.OCRMethodList.Count > 0);

            btnAddOne.Enabled = (isenable && OCR.OCROnlineItemList.Count > 0);
            btnAddAll.Enabled = (isenable && OCR.OCROnlineItemList.Count > 0);

            btnDelItem.Enabled = (isenable && OCR.OCRItemList.Count > 0);
            btnDelALLItem.Enabled = (isenable && OCR.OCRItemList.Count > 0);
            ppgMethod.Enabled = (isenable && OCR.OCRMethodList.Count > 0);

            flpOnlineOCRItem.Enabled = (isenable && OCR.OCROnlineItemList.Count > 0);
            bool isTemp = isenable && OCR.OCRItemList.Count > 0;
            try
            {
                if (!isTemp)
                    flpOCRItem.Controls.Clear();

                if (flpOCRItem.Enabled != isTemp)
                    flpOCRItem.Enabled = isTemp;
            }
            catch(Exception ex)
            { JetEazy.LoggerClass.Instance.WriteException(ex); }

            gbImageChanged.Enabled = isenable;
            gbPar.Enabled = isenable;
            lstMethod.Enabled = isenable;
            btnNext.Enabled = isenable;
            btnReturn.Enabled = isenable;
            btnAutoTest.Enabled = isenable;

            if (lstMethod.Enabled)
                IsNeedToChange = true;

            if (isenable)
            {
                SHOWDISP.Enabled = true;

                if (OCR.OCRMethodList.Count > 0)
                {
                    METHODISP.Enabled = true;
                }
            }
            else
            { 
                flpOCRItem.Controls.Clear();
                flpOnlineOCRItem.Controls.Clear();
                SHOWDISP.Enabled = false;
                METHODISP.Enabled = false;
            }
        }

        public void SetDefaultView()
        {
            SHOWDISP.SetDisplayImage();
            METHODISP.SetDisplayImage();
        }

        void FillDisplay(int index)
        {
            IsNeedToChange = false;

            if (index == -1)
            {

                METHODISP.ClearAll();

                ppgMethod.SelectedObject = null;

                lstMethod.Items.Clear();

                flpOCRItem.Controls.Clear();
                flpOnlineOCRItem.Controls.Clear();

                SetEnable(false);
            }
            else
            {
                OCRMethdClass ocrmethod = OCR.OCRMethodList[index];

                METHODISP.ClearAll();

                MethodMover.Clear();
                MethodMover.Add(ocrmethod.RectEAG);
                METHODISP.SetMover(MethodMover);
                METHODISP.ReplaceDisplayImage(ocrmethod.bmpMethod);

                ocrmethod.ConstructProperty();
                ppgMethod.SelectedObject = ocrmethod;

                ocrmethod.BrightnesChange += Ocrmethod_BrightnesChange;
                ocrmethod.CotrastChange += Ocrmethod_CotrastChange;
                ocrmethod.RatioChange += Ocrmethod_RatioChange;

                //flpOnlineOCRItem.Controls.Clear();
                //flpOCRItem.Controls.Clear();
                //foreach (OCRItemClass ocritem in OCR.OCRItemList)
                //{
                //    //Fill OCRItem
                //    OCRConter ui = new OCRConter();
                //    ui.OCRSet = ocritem;
                //    flpOCRItem.Controls.Add(ui);
                //}

                SetEnable(true);
            }
            IsNeedToChange = true;
        }
        /// <summary>
        /// OCR参数还原
        /// </summary>
        public void Restore()
        {
            //flpOCRItem.Location = new Point(607, 241);
            //flpOCRItem.Width = 216;

          //  gbPar.Visible = false;
           // lstMethod.Height = 544;

            flpOCRItem.Controls.Clear();
            foreach (OCRItemClass ocritem in OCR.OCRItemList)
            {
                //Fill OCRItem
                OCRConter ui = new OCRConter();

                ui.OCRSet = ocritem;
                ui.ONClick += Ui_ONClick;
                flpOCRItem.Controls.Add(ui);
            }
        }
        /// <summary>
        /// flpOnlineOCRItem 列表的
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ui_ONClick1(object sender, OCRConter.ParameterEventArgs e)
        {

            foreach (Control control in flpOnlineOCRItem.Controls)
            {
                OCRConter ocrconter = (OCRConter)control;
                if (!isCtrlDeyDown)
                    ocrconter.isSelected = false;

            }

            OCRConter temp = (OCRConter)sender;
            if (!temp.isSelected)
                temp.isSelected = true;
            else
                temp.isSelected = false;

            foreach (Control control in flpOCRItem.Controls)
            {
                OCRConter ocrconter = (OCRConter)control;
                ocrconter.isSelected = false;
            }
        }
        private void Ui_ONClick(object sender, OCRConter.ParameterEventArgs e)
        {
            OCRConter temp = (OCRConter)sender;
            foreach (Control control in flpOCRItem.Controls)
            {
                OCRConter ocrconter = (OCRConter)control;
                if (!isCtrlDeyDown)
                    ocrconter.isSelected = false;
            }

            if (!temp.isSelected)
                temp.isSelected = true;
            else
                temp.isSelected = false;

            foreach (Control control in flpOnlineOCRItem.Controls)
            {
                OCRConter ocrconter = (OCRConter)control;
                ocrconter.isSelected = false;

            }
        }

        private void Ocrmethod_RatioChange(OCRMethdClass myThis, float iValue)
        {
            // BrightCotrast(myThis.bmpMethod, myThis.Brightness, myThis.Contrast);
        }

        private void Ocrmethod_CotrastChange(OCRMethdClass myThis, int iValue)
        {
            BrightCotrast(myThis.bmpMethod, myThis.Brightness, myThis.Contrast);
        }

        private void Ocrmethod_BrightnesChange(OCRMethdClass myThis, int iValue)
        {
            BrightCotrast(myThis.bmpMethod, myThis.Brightness, myThis.Contrast);
        }

        Bitmap BrightCotrast(Bitmap bmpT, int iBrightnes, int iCotrast)
        {
            Bitmap BmpTemp = new Bitmap(bmpT);
            JetEazy.BasicSpace.myImageProcessor.SetBrightContrastR(BmpTemp, iBrightnes, iCotrast);

            OCRMethdClass ocrmethod = OCR.OCRMethodList[lstMethod.SelectedIndex];
            METHODISP.ClearAll();
            MethodMover.Clear();
            MethodMover.Add(ocrmethod.RectEAG);
            METHODISP.SetMover(MethodMover);
            METHODISP.ReplaceDisplayImage(BmpTemp);
            return BmpTemp;
        }

        void init_Display()
        {
            SHOWDISP = dispUI1;
            SHOWDISP.Initial();
            SHOWDISP.SetDisplayType(DisplayTypeEnum.NORMAL);
        }
        void update_Display()
        {
            SHOWDISP.Refresh();
            SHOWDISP.DefaultView();
        }

        public delegate void TriggerHandler(string opstatus);
        public event TriggerHandler TriggerAction;
        public void OnTrigger(string opstatus)
        {
            if (TriggerAction != null)
            {
                TriggerAction(opstatus);
            }
        }

    }
}
