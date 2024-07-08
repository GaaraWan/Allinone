using Allinone.OPSpace.AnalyzeSpace;
using JETLIB;
using JzDisplay;
using MoveGraphLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WorldOfMoveableObjects;

namespace Allinone.FormSpace
{
    public partial class CorrectForm : Form
    {
        OPSpace.AnalyzeSpace.CorrestClass Correct;
        CorrectTool mTool;
        JetEazy.ControlSpace.CCDCollectionClass mCCD;
        ComboBox cobCCD;
        NumericUpDown nudCCDBright;
        Button btnGetImage, btnInputImage, btnOK, btnCencer, btnRun, btnFindPoint;
        PropertyGrid PGr;
        int index = 0;
        JzDisplay.UISpace.DispUI DISPUI;
        Mover myMover = new Mover();
        CheckBox cbSaveImage;
        public CorrectForm()
        {
            InitializeComponent();
            mCCD = Universal.CCDCollection;
            Correct = Universal.Correct;
            this.Load += CorrectForm_Load;
        }

        private void CorrectForm_Load(object sender, EventArgs e)
        {
            cobCCD = comboBox1;
            nudCCDBright = numericUpDown1;
            PGr = propertyGrid1;
            btnGetImage = button2;
            btnInputImage = button1;
            btnRun = button3;
            btnOK = button4;
            btnCencer = button5;
            DISPUI = dispUI1;
            cbSaveImage = checkBox1;
            btnFindPoint = button6;

            DISPUI.Initial();
            DISPUI.SetDisplayType(DisplayTypeEnum.NORMAL);
            DISPUI.MoverAction += DISPUI_MoverAction;

            for (int i = 0; i < mCCD.GetCCDCountWord; i++)
            {
                cobCCD.Items.Add("CCD:" + i.ToString());
            }
            if (Correct.ToolList.Count != mCCD.GetCCDCountWord)
            {
                if (Correct.ToolList.Count > mCCD.GetCCDCountWord)
                {
                    for (int j = Correct.ToolList.Count - 1; j > mCCD.GetCCDCountWord; j--)
                        Correct.ToolList.RemoveAt(j);
                }
                else
                    Correct.ADD(mCCD.GetCCDCountWord - Correct.ToolList.Count);

                Correct.ToUpdata();
                Correct.Save();
            }
            cobCCD.SelectedIndexChanged += CobCCD_SelectedIndexChanged;
            cobCCD.SelectedIndex = index;

            nudCCDBright.ValueChanged += NudCCDBright_ValueChanged;
            btnGetImage.Click += BtnGetImage_Click;

            btnCencer.Click += BtnCencer_Click;
            btnOK.Click += BtnOK_Click;
            btnInputImage.Click += BtnInputImage_Click;
            btnFindPoint.Click += BtnFindPoint_Click;
            btnRun.Click += BtnRun_Click;
        }

        private void BtnFindPoint_Click(object sender, EventArgs e)
        {
            Bitmap bmp = mTool.bmp.Clone(mTool.Rect, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            FindPoint(bmp);
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(mTool.bmp);//.Clone(rect, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Rectangle rect = new Rectangle((int)mTool.Rect.X, (int)mTool.Rect.Y, (int)mTool.Rect.Width, (int)mTool.Rect.Height);

            Rectangle rect1 = new Rectangle(0, 0, bmp.Width, rect.Y);
            Rectangle rect2 = new Rectangle(0, 0, rect.X, bmp.Height);
            Rectangle rect3 = new Rectangle(rect.X + rect.Width, 0, bmp.Width - (rect.X + rect.Width), bmp.Height);
            Rectangle rect4 = new Rectangle(0, rect.Y + rect.Height, bmp.Width, bmp.Height - (rect.Y + rect.Height));

            Graphics gg = Graphics.FromImage(bmp);
            Color color = Color.Black;
            if (!mTool.Par.ISBlack)
                color = Color.White;

            gg.FillRectangle(new SolidBrush(color), rect1);
            gg.FillRectangle(new SolidBrush(color), rect2);
            gg.FillRectangle(new SolidBrush(color), rect3);
            gg.FillRectangle(new SolidBrush(color), rect4);
            gg.Dispose();

            bool isok = ViweToWord(bmp);
            DISPUI.SetDisplayImage(bmp);
            if (isok)
                MessageBox.Show("矫正OK");

        }

        private void BtnInputImage_Click(object sender, EventArgs e)
        {

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;//该值确定是否可以选择多个文件
            dialog.Title = "请选择文件夹";
            dialog.Filter = "所有文件(*.png)|*.png;*.bmp|JGP文件|*.jpg";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = dialog.FileName;
                Bitmap mybmptemp = new Bitmap(file);
                Bitmap bmpSet = new Bitmap(mybmptemp);
                mybmptemp.Dispose();

                Correct.ToolList[index].bmp = new Bitmap(bmpSet);
                UPDATA();
            }

        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            Correct.Save();
            this.Close();
        }

        private void BtnCencer_Click(object sender, EventArgs e)
        {
            Correct.Load();
            this.Close();
        }

        private void DISPUI_MoverAction(MoverOpEnum moverop, string opstring)
        {
            int i = 0;
            string[] strs = opstring.Split(',');

            switch (moverop)
            {
                case MoverOpEnum.SELECT:


                    GraphicalObject Gobtemp = myMover[i].Source;
                    if (Gobtemp is JzRectEAG)
                    {
                        JzRectEAG EAG = Gobtemp as JzRectEAG;

                        Correct.ToolList[index].Rect = EAG.GetRectF;
                    }

                    // List<int> selectindexlist = new List<int>();
                    if (opstring == "")
                    {
                        //  while (i < lsbShapes.Items.Count)
                        //{
                        //  lsbShapes.SetSelected(i, false);
                        //    i++;
                        //}
                    }
                    else
                    {
                        //foreach (string str in strs)
                        //{
                        //    selectindexlist.Add(int.Parse(str));
                        //}

                        //if (selectindexlist.Count > 0)
                        //{
                        //    i = 0;
                        //  while (i < lsbShapes.Items.Count)
                        //{
                        //  lsbShapes.SetSelected(i, selectindexlist.IndexOf(i) > -1);
                        //    i++;
                        //}

                        //  if (lsbShapes.SelectedIndex != selectindexlist[0])
                        //  {
                        //   lsbShapes.SelectedIndex = selectindexlist[0];
                        //}
                    }

                    break;
                case MoverOpEnum.ADD:
                    foreach (string str in strs)
                    {
                        // lsbShapes.Items.Add(str);
                    }
                    break;
                case MoverOpEnum.DEL:

                    foreach (string str in strs)
                    {
                        if (str == "")
                            continue;

                        int index = int.Parse(str.Split(':')[0]);

                        if (index < 4)
                            continue;


                    }
                    break;
                case MoverOpEnum.READYTOMOVE:

                    break;
            }
        }

        private void BtnGetImage_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(1, 1);
            //Correct.ToolList[index].bmp = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            switch (Universal.OPTION)
            {
                case JetEazy.OptionEnum.MAIN_SD:

                    mCCD.GetImage(index);
                    bmp = mCCD.GetBMP(index, false);


                    break;
                default:

                    bmp = mCCD.GetImageWord(index);

                    break;
            }

            Correct.ToolList[index].bmp = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            UPDATA();
        }

        private void NudCCDBright_ValueChanged(object sender, EventArgs e)
        {
            Correct.ToolList[index].CCDBright = (int)nudCCDBright.Value;
            mCCD.SetExposure(Correct.ToolList[index].CCDBright, index);


        }

        private void CobCCD_SelectedIndexChanged(object sender, EventArgs e)
        {
            index = cobCCD.SelectedIndex;
            mTool = Correct.ToolList[index];
            PGr.SelectedObject = mTool.Par;
            nudCCDBright.Value = mTool.CCDBright;

            UPDATA();
        }

        void UPDATA()
        {
            RectangleF rectf = Correct.ToolList[index].Rect;
            JzRectEAG jzrect = new JzRectEAG(Color.FromArgb(0, 128, 0, 9), rectf);
            jzrect.IsSelected = false;
            jzrect.IsFirstSelected = false;
            jzrect.RelateNo = 1;
            jzrect.RelatePosition = 0;
            jzrect.RelateLevel = 1;

            myMover.Clear();
            myMover.Add(jzrect);

            DISPUI.SetDisplayImage(Correct.ToolList[index].bmp);
            DISPUI.SetMover(myMover);
            DISPUI.MappingSelect();
            DISPUI.RefreshDisplayShape();
        }

        /// <summary>
        /// 找点
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public bool FindPoint(Bitmap bmp)
        {

            JetGrayImg grayimage = new JetGrayImg(bmp);
            JetImgproc.Threshold(grayimage, mTool.Par.BrightMax, mTool.Par.BrightMin, grayimage);
            JetBlob jetBlob = new JetBlob();
            if (mTool.Par.ISBlack)
                jetBlob.Labeling(grayimage, JConnexity.Connexity4, JBlobLayer.BlackLayer);
            else
                jetBlob.Labeling(grayimage, JConnexity.Connexity4, JBlobLayer.WhiteLayer);
            int icount = jetBlob.BlobCount;
            
            for (int i = 0; i < icount; i++)
            {
                int iArea = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.Area);

                int ix = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.LeftMost);
                int iy = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.TopMost);
                int iWidth = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.BoundingBoxWidth);
                int iHeight = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.BoundingBoxHeight);
                Rectangle rectEX = new Rectangle(ix + (int)mTool.Rect.X, iy + (int)mTool.Rect.Y, iWidth, iHeight);
                

                Bitmap bmpTemp = new Bitmap(mTool.bmp);
                Graphics g = Graphics.FromImage(bmpTemp);
                g.DrawRectangle(new Pen(Color.Red, 5), rectEX);
                g.Dispose();


                DISPUI.SetDisplayImage(bmpTemp);

                DialogResult resultButton = MessageBox.Show("需要把找到的点信息插入参数吗?或是点" + Environment.NewLine +
                    "高: " + iHeight + Environment.NewLine +
                    "宽: " + iWidth + Environment.NewLine +
                    "面积: " + iArea + Environment.NewLine +
                    "最大面积: " + (iArea + 2000) + Environment.NewLine +
                    "最小面积: " + (iArea - (iArea > 2000 ? 2000 : 10)) + Environment.NewLine
                   , "SYS", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Asterisk);
                if (resultButton == DialogResult.Yes)
                {
                    mTool.Par.AreaMax = (iArea + 2000);
                    mTool.Par.AreaMin = (iArea - (iArea > 2000 ? 2000 : 10));
                    mTool.Par.SizeX = iWidth;
                    mTool.Par.SizeY = iHeight;

                    PGr.Refresh();
                    return true;
                }
                else if (resultButton == DialogResult.Cancel)
                    return false;
                

            }
            
            return false;
        }

        public bool ViweToWord(Bitmap bmp)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            //  Bitmap myfindbmp = new Bitmap(bmp);

            List<List<Rectangle>> mylist = new List<List<Rectangle>>();

            JetGrayImg grayimage = new JetGrayImg(bmp);
            JetImgproc.Threshold(grayimage, mTool.Par.BrightMax, mTool.Par.BrightMin, grayimage);
            JetBlob jetBlob = new JetBlob();
            if (mTool.Par.ISBlack)
                jetBlob.Labeling(grayimage, JConnexity.Connexity4, JBlobLayer.BlackLayer);
            else
                jetBlob.Labeling(grayimage, JConnexity.Connexity4, JBlobLayer.WhiteLayer);
            int icount = jetBlob.BlobCount;

            // Bitmap bmpTemp = new Bitmap(bmp, bmp.Width, bmp.Height);

            Graphics gg = Graphics.FromImage(bmp);
            for (int i = 0; i < icount; i++)
            {
                int iArea = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.Area);
                if (iArea > mTool.Par.AreaMin && iArea < mTool.Par.AreaMax)
                {
                    //  JRotatedRectangleF rotat= JetBlobFeature. ComputeMinRectangle(jetBlob, i);

                    int ix = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.LeftMost);
                    int iy = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.TopMost);
                    int iWidth = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.BoundingBoxWidth);
                    int iHeight = JetBlobFeature.ComputeIntegerFeature(jetBlob, i, JBlobIntFeature.BoundingBoxHeight);
                    Rectangle rectEX = new Rectangle(ix, iy, iWidth, iHeight);

                    if (Math.Abs(iWidth - mTool.Par.SizeX) > 10)
                        continue;
                    if (Math.Abs(iHeight - mTool.Par.SizeY) > 10)
                        continue;

                    Point myFilst = new Point(ix, iy);


                    gg.DrawRectangle(new Pen(Color.Red, 5), rectEX);
                    //if (objFind.FoundList[i].Area < 10 || objFind.FoundList[i].Area > 10000)
                    //    continue;

                    bool isjixu = true;

                    foreach (List<Rectangle> myrect in mylist)
                    {
                        bool isno = true;
                        foreach (Rectangle rect in myrect)
                        {
                            if (rect.X == myFilst.X && rect.Y == myFilst.Y)
                            {
                                isno = false;
                                break;
                            }
                        }
                        if (!isno)
                        {
                            isjixu = false;
                            break;
                        }
                    }

                    if (!isjixu)
                        continue;

                    List<Rectangle> myrectlist = new List<Rectangle>();
                    for (int j = 0; j < icount; j++)
                    {
                        int ixj = JetBlobFeature.ComputeIntegerFeature(jetBlob, j, JBlobIntFeature.LeftMost);
                        int iyj = JetBlobFeature.ComputeIntegerFeature(jetBlob, j, JBlobIntFeature.TopMost);
                        int iWidthj = JetBlobFeature.ComputeIntegerFeature(jetBlob, j, JBlobIntFeature.BoundingBoxWidth);
                        int iHeightj = JetBlobFeature.ComputeIntegerFeature(jetBlob, j, JBlobIntFeature.BoundingBoxHeight);
                        Rectangle rectEXj = new Rectangle(ixj, iyj, iWidth, iHeight);

                        Point myFilstTemp = new Point(ixj, iyj);


                        int iAreaj = JetBlobFeature.ComputeIntegerFeature(jetBlob, j, JBlobIntFeature.Area);
                        if (iAreaj < mTool.Par.AreaMin || iAreaj > mTool.Par.AreaMax)
                            continue;

                        if (Math.Abs(myFilst.Y - myFilstTemp.Y) < mTool.Par.GapYView / 2)
                        {
                            //  myrectlist.Add(objFind.FoundList[j].rect);

                            myrectlist.Add(rectEXj);
                        }
                        else
                        {


                        }

                    }
                    mylist.Add(myrectlist);



                }

            }

            gg.Dispose();

            // bmpTemp.Save("D:\\tempCheck.png");


            for (int i = 0; i < mylist.Count; i++)
            {
                for (int j = 0; j < mylist[i].Count; j++)
                {
                    for (int s = j + 1; s < mylist[i].Count; s++)
                    {
                        if (mylist[i][j].X > mylist[i][s].X)
                        {
                            Rectangle rect = mylist[i][j];
                            mylist[i][j] = mylist[i][s];
                            mylist[i][s] = rect;
                        }
                    }
                }
            }

            if (mylist.Count == 0)
                return false;

            int myX = mylist[0].Count;
            for (int i = 0; i < mylist.Count; i++)
            {
                if (myX != mylist[i].Count)
                    return false;
            }

            PointF[,] View = new PointF[mylist.Count, mylist[0].Count];
            PointF[,] Word = new PointF[mylist.Count, mylist[0].Count];


            Graphics g = Graphics.FromImage(bmp);
            // g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, mybmp.Width, mybmp.Height));

            for (int j = 0; j < mylist.Count; j++)
            {
                for (int i = 0; i < mylist[j].Count; i++)
                {
                    int x = mylist[j][i].X + mylist[j][i].Width / 2;
                    int y = mylist[j][i].Y + mylist[j][i].Height / 2;
                    View[j, i] = new PointF(x, y);

                    //float fx = 0, fy = 0;
                    //if (index == 0)
                    //{
                    //    fx = mPar.Came1.XValue;
                    //    fy = mPar.Came1.YValue;
                    //}
                    //if (index == 1)
                    //{
                    //    fx = mPar.Came2.XValue;
                    //    fy = mPar.Came2.YValue;
                    //}
                    Word[j, i] = new PointF(mTool.Par.PositionX + i * mTool.Par.GapX, mTool.Par.PositionY + j * mTool.Par.GapY);
                    Font font = new Font(FontFamily.GenericSansSerif, 10f, FontStyle.Bold);
                    Brush bush = new SolidBrush(Color.Lime);//填充的颜色
                    g.DrawString(j.ToString() + "_" + i.ToString(), font, bush, new Point(x, y + 40));
                    g.DrawString("View " + "X:" + x + " Y:" + y, font, bush, new Point(x, y));
                    g.DrawString("Word " + "X:" + Word[j, i].X + " Y:" + Word[j, i].Y, font, bush, new Point(x, y + 20));
                }
            }
            g.Dispose();
            if (cbSaveImage.Checked)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "";
                sfd.InitialDirectory = @"D:\";
                sfd.Filter = "图片文件|*.png";
                if (sfd.ShowDialog() == DialogResult.OK)
                {

                    string path = sfd.FileName;
                    if (path != "")
                    {
                        bmp.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                        // return;
                    }
                }

                //using (FileStream fsWrite = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
                //{
                //    //byte[] buffer = Encoding.Default.GetBytes(textBox1.Text);
                //    //fsWrite.Write(buffer, 0, buffer.Length);
                //    //MessageBox.Show("保存成功");

                //}

                //  bmp.Save("D:\\3.png", System.Drawing.Imaging.ImageFormat.Png);
            }

            mTool.pointsForView = View;
            mTool.pointsForWord = Word;

            watch.Stop();

            this.Text = "用时：" + watch.ElapsedMilliseconds + " ms";

            if (View.GetLength(0) >= 3 && View.GetLength(1) >= 3)
                return true;
            else
                return false;
        }
    }
}