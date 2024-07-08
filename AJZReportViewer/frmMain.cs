using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace AJZReportViewer
{
    public partial class frmMain : Form
    {
        string m_path = @"D:\report\work";

        TreeView trvLot;
        ListBox lstFilename;

        AllinoneViewerUI Viewer;

        AnalyzeClass root = null;
        AnalyzeClass AnalyzeLst = null;

        Button btnTrvUpdate;

        Button btnLstUp;
        Button btnLstDown;
        Button btnLstGo;
        Button btnLstUpdate;

        public frmMain()
        {
            InitializeComponent();

            this.Load += FrmMain_Load;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            Init();
        }

        void Init()
        {
            this.Text = "德龙AOI 资料查看器 Ver:" + Application.ProductVersion;
            this.Text = "资料查看器 Ver:" + Application.ProductVersion;
            this.Text = "JetEazy Viewer";

            trvLot = treeView1;
            lstFilename = listBox1;

            btnTrvUpdate = button4;

            btnLstUp = button5;
            btnLstDown = button6;
            btnLstGo = button7;
            btnLstUpdate = button8;

            Viewer = allinoneViewerUI1;

            trvLot.HideSelection = false; //可让选中节点保持高亮。
            trvLot.DrawMode = TreeViewDrawMode.OwnerDrawText;
            trvLot.AfterSelect += TrvLot_AfterSelect;
            lstFilename.SelectedIndexChanged += LstFilename_SelectedIndexChanged;
            trvLot.DrawNode += TrvLot_DrawNode;

            updateTreeview();

            btnTrvUpdate.Click += BtnTrvUpdate_Click;

            btnLstUp.Click += BtnLstUp_Click;
            btnLstDown.Click += BtnLstDown_Click;
            btnLstUpdate.Click += BtnLstUpdate_Click;

            this.WindowState = FormWindowState.Minimized;
        }

        private void TrvLot_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            e.DrawDefault = true; //我这里用默认颜色即可，只需要在TreeView失去焦点时选中节点仍然突显

            if ((e.State & TreeNodeStates.Selected) != 0)
            {
                //演示为绿底白字
                e.Graphics.FillRectangle(Brushes.DarkBlue, e.Node.Bounds);
                Font nodeFont = e.Node.NodeFont;
                if (nodeFont == null) nodeFont = ((TreeView)sender).Font;
                e.Graphics.DrawString(e.Node.Text, nodeFont, Brushes.White, Rectangle.Inflate(e.Bounds, 2, 0));
            }
            else
            {
                e.DrawDefault = true;
            }


            if ((e.State & TreeNodeStates.Focused) != 0)
            {
                using (Pen focusPen = new Pen(Color.Black))
                {
                    focusPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    Rectangle focusBounds = e.Node.Bounds;
                    focusBounds.Size = new Size(focusBounds.Width - 1,
                    focusBounds.Height - 1);
                    e.Graphics.DrawRectangle(focusPen, focusBounds);
                }
            }
        }

        private void BtnLstDown_Click(object sender, EventArgs e)
        {
            if (AnalyzeLst == null)
                return;

            if (lstFilename.Items.Count == 0)
                return;

            if (lstFilename.SelectedIndex < 0 || lstFilename.SelectedIndex > lstFilename.Items.Count)
                lstFilename.SelectedIndex = 0;
            else if (lstFilename.SelectedIndex == lstFilename.Items.Count - 1)
                lstFilename.SelectedIndex = 0;
            else
                lstFilename.SelectedIndex++;
        }

        private void BtnLstUp_Click(object sender, EventArgs e)
        {
            if (AnalyzeLst == null)
                return;

            if (lstFilename.Items.Count == 0)
                return;

            if (lstFilename.SelectedIndex <= 0 || lstFilename.SelectedIndex >= lstFilename.Items.Count)
                lstFilename.SelectedIndex = lstFilename.Items.Count - 1; 
            else if (lstFilename.SelectedIndex > 0)
                lstFilename.SelectedIndex--;
        }

        private void BtnLstUpdate_Click(object sender, EventArgs e)
        {
            updateListbox(AnalyzeLst);
        }

        private void LstFilename_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AnalyzeLst == null)
                return;

            int index = lstFilename.SelectedIndex;

            if (index < 0)
                return;

            Viewer.Set(AnalyzeLst.FullPath + "\\" + lstFilename.SelectedItem.ToString() + ".csv", lstFilename.Items.Count);
            Viewer.UpdateViewer();

        }

        private void BtnTrvUpdate_Click(object sender, EventArgs e)
        {
            updateTreeview();
        }

        private void TrvLot_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level == 2)
            {
                //root.BranchList[0].BranchList[]
                //updateListbox()
                //e.Node.BackColor = Color.Blue;
                TreeNode par1 = e.Node.Parent;
                TreeNode par2 = par1.Parent;
                int i = 0;
                int j = 0;
                int k = 0;
                bool bOK = false;
                foreach (AnalyzeClass analyze in root.BranchList)
                {
                    foreach (AnalyzeClass analyze1 in analyze.BranchList)
                    {
                        if (analyze1.Name == e.Node.Text && analyze.Name == par1.Text)
                        {
                            bOK = true;
                            break;
                        }

                        j++;
                    }

                    if (bOK)
                        break;

                    i++;
                }

                if (i >= root.BranchList.Count)
                {
                    AnalyzeLst = null;
                    ClearListbox();
                    return;
                }
                if (j >= root.BranchList[i].BranchList.Count)
                {
                    AnalyzeLst = null;
                    ClearListbox();
                    return;
                }

                AnalyzeLst = root.BranchList[i].BranchList[j];
                updateListbox(AnalyzeLst);

            }
            else if (e.Node.Level == 3)
            {
                //e.Node.BackColor = Color.Blue;
                //root.BranchList[0].BranchList[]
                //updateListbox()

                TreeNode par1 = e.Node.Parent;
                TreeNode par2 = par1.Parent;
                int i = 0;
                int j = 0;
                int k = 0;
                bool bOK = false;
                foreach (AnalyzeClass analyze in root.BranchList)
                {
                    j = 0;
                    foreach (AnalyzeClass analyze1 in analyze.BranchList)
                    {
                        k = 0;
                        foreach (AnalyzeClass analyze2 in analyze1.BranchList)
                        {
                            if (analyze2.Name == e.Node.Text && analyze1.Name == par1.Text)
                            {
                                bOK = true;
                                break;
                            }
                            k++;
                        }

                        if (bOK)
                            break;

                        j++;
                    }

                    if (bOK)
                        break;

                    i++;
                }

                if (i >= root.BranchList.Count)
                {
                    AnalyzeLst = null;
                    ClearListbox();
                    return;
                }
                if (j >= root.BranchList[i].BranchList.Count)
                {
                    AnalyzeLst = null;
                    ClearListbox();
                    return;
                }
                if (k >= root.BranchList[i].BranchList[j].BranchList.Count)
                {
                    AnalyzeLst = null;
                    ClearListbox();
                    return;
                }

                AnalyzeLst = root.BranchList[i].BranchList[j].BranchList[k];
                updateListbox(AnalyzeLst);

            }
            else
            {
                ClearListbox();
            }
        }

        void ClearTreeview()
        {
            trvLot.Nodes.Clear();
        }
        void updateTreeview()
        {
            ClearTreeview();


            if (!Directory.Exists(m_path))
                Directory.CreateDirectory(m_path);

            root = new AnalyzeClass(m_path);
            TreeNode tnroot = new TreeNode(root.Name);
            trvLot.Nodes.Add(tnroot);
            string[] rootDirs = Directory.GetDirectories(root.FullPath);
            foreach (string str in rootDirs)
            {
                if (str == "D:\\report\\work\\auto")
                {
                    AnalyzeClass branch = new AnalyzeClass(str);
                    root.BranchList.Add(branch);

                    TreeNode tn1 = new TreeNode(branch.Name);
                    tnroot.Nodes.Add(tn1);

                    string[] branchDirs = Directory.GetDirectories(branch.FullPath);
                    foreach (string str2 in branchDirs)
                    {
                        AnalyzeClass branch2 = new AnalyzeClass(str2);
                        branch.BranchList.Add(branch2);

                        TreeNode tn2 = new TreeNode(branch2.Name);
                        tn1.Nodes.Add(tn2);

                        string[] branchDirs2 = Directory.GetDirectories(branch2.FullPath);
                        foreach (string str3 in branchDirs2)
                        {
                            AnalyzeClass branch3 = new AnalyzeClass(str3);
                            branch2.BranchList.Add(branch3);

                            TreeNode tn3 = new TreeNode(branch3.Name);
                            tn2.Nodes.Add(tn3);

                            //string[] branchFiles2 = Directory.GetFiles(branch2.FullPath);
                            //foreach (string str3 in branchFiles2)
                            //{
                            //    AnalyzeClass branch3 = new AnalyzeClass(str3);
                            //    branch2.BranchList.Add(branch3);

                            //    TreeNode tn3 = new TreeNode(branch3.Name);
                            //    tn2.Nodes.Add(tn3);
                            //}

                        }
                    }
                }
            }

            trvLot.ExpandAll();
            
        }

        void ClearListbox()
        {
            lstFilename.Items.Clear();
        }

        void updateListbox(AnalyzeClass eAnalyze)
        {
            if (eAnalyze == null)
                return;

            ClearListbox();

            string[] branchFiles2 = Directory.GetFiles(eAnalyze.FullPath);
            foreach (string str3 in branchFiles2)
            {
                string[] strs = str3.Split('\\');
                string csv = strs[strs.Length - 1];

                lstFilename.Items.Add(csv.Replace(".csv", ""));
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
