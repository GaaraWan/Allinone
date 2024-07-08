using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using JzKHC.DBSpace;
//using Jumbo301.UniversalSpace;
using JetEazy.BasicSpace;
using JzKHC.ControlSpace;
using JzKHC.OPSpace;


namespace JzKHC.FormSpace
{
    public partial class KHCForm : Form
    {
        //TextBox txtFastSearch;
        Bitmap bmpView = new Bitmap(1, 1);

        bool IsNeedChange = true;
        //Label lblDatetime;
        //ListBox lstName;
        ComboBox cboName;

        Button btnDelete;
        //Button btnSelect;
        Button btnCancel;

        KHCClass KHCCollection;

        //OPScreenUIClass OPScreenUI;
        List<string> ListName = new List<string>();
        int itmp = 0;

        //ScreenClass SCREEN;

        //AccountDBClass ACCOUNTDB
        //{
        //    get
        //    {
        //        return Universal.ACCOUNTDB;
        //    }
        //}
        RecipeDBClass RECIPEDB
        {
            get
            {
                return Universal.RECIPEDB;
            }
        }

        CCDOfflineClass CCD
        {
            get { return Universal.CCD; }
        }

        public KHCForm(KHCClass khccollection)
        {
            InitializeComponent();
            KHCCollection = khccollection;

        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            RECIPEDB.RECIPEDBUI = recipedbuiControl1;
            RECIPEDB.RefreshUI();

            Initial();

            
        }
        void Initial()
        {
            //SCREEN = new ScreenClass("screen_recipeselection", this);
            //SCREEN.SetLanguage();

            btnDelete = button2;
            //btnSelect = button1;
            btnCancel = button3;

            cboName = comboBox1;

            //txtFastSearch = textBox1;
            //lstName = listBox1;
            //lblDatetime = label10;

            //txtFastSearch.TextChanged += new EventHandler(txtFastSearch_TextChanged);
            btnDelete.Click += new EventHandler(btnDelete_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
            //btnSelect.Click += new EventHandler(btnSelect_Click);
            this.Activated += new EventHandler(RecipeSelectionForm_Activated);

            //btnDelete.Enabled = ACCOUNTDB.Islogin && ACCOUNTDB.CanManageRecipe;

            //OPScreenUI = new OPScreenUIClass(opScreenUIControl1, 0, -2, 1);

            GetNameList();

            //lstName.SelectedIndexChanged += new EventHandler(lstName_SelectedIndexChanged);
            //lstName.SelectedIndex = itmp;

            cboName.SelectedIndexChanged += CboName_SelectedIndexChanged;
            cboName.SelectedIndex = itmp;

        }

        private void CboName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsNeedChange)
            {
                Universal.GlobalPassInteger = RECIPEDB.ID;

                //if (lstName.SelectedIndices.Count == 1)
                //{
                Universal.GlobalPassInteger = int.Parse(ListName[cboName.SelectedIndex].Split(',')[1]);

                RECIPEDB.Goto(Universal.GlobalPassInteger);

                RECIPEDB.Load(false);

                RECIPEDB.FillDisplay();

                INI.LAST_RECIPEID = RECIPEDB.ID;
                INI.Save();
                //this.DialogResult = DialogResult.OK;
                //}
                //else
                //    MessageBox.Show("請選擇單一選項。", "MAIN", MessageBoxButtons.OK);

                //ShowViewBMP(int.Parse(ListName[cboName.SelectedIndex].Split(',')[1]));
            }
        }

        void RecipeSelectionForm_Activated(object sender, EventArgs e)
        {
            //txtFastSearch.Focus();
        }
        void txtFastSearch_TextChanged(object sender, EventArgs e)
        {
            //if (txtFastSearch.Text == "")
            //    GetNameList();
            //else
            //    GetNameList(txtFastSearch.Text);

            //if (ListName.Count > 0)
            //    lstName.SelectedIndex = 0;
            //else
            //{
            //    lblDatetime.Text = "";
            //    ShowViewBMP(0);
            //}
        }

        void btnSelect_Click(object sender, EventArgs e)
        {
            //if (lstName.SelectedIndices.Count == 1)
            //{
            //    Universal.GlobalPassInteger = int.Parse(ListName[lstName.SelectedIndex].Split(',')[1]);
            //    this.DialogResult = DialogResult.OK;
            //}
            //else
            //    MessageBox.Show("請選擇單一選項。", "MAIN", MessageBoxButtons.OK);
        }
        void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            //this.DialogResult = DialogResult.Cancel;
        }
        void btnDelete_Click(object sender, EventArgs e)
        {
            return;
            if (cboName.Items.Count > 0)
            {
                int i = 0, j = 0;
                while (i < cboName.Items.Count)
                {
                    //if (ListName[lstName.SelectedIndices[i]].Split(',')[1] == Universal.GlobalPassInteger.ToString() || ListName[lstName.SelectedIndices[i]].Split(',')[1] == "1")
                        j++;

                    i++;
                }

                if (j > 0)
                    MessageBox.Show("內容包含不可刪除的預設資料，刪除中止。", "MAIN", MessageBoxButtons.OK);
                else
                {
                    if (MessageBox.Show("是否要刪除所選擇的資料?", "MAIN", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        i = 0;
                        while (i < cboName.Items.Count)
                        {
                            //if (RECIPEDB.ID > int.Parse(ListName[lstName.SelectedIndices[i]].Split(',')[1]))
                            //    RECIPEDB.RecordIndex--;
                            
                            //RECIPEDB.DeleteIDDirect(int.Parse(ListName[lstName.SelectedIndices[i]].Split(',')[1]));

                            //DeleteTree(int.Parse(ListName[lstName.SelectedIndices[i]].Split(',')[1]));

                            i++;
                        }
                    }

                    //if (txtFastSearch.Text == "")
                        GetNameList();
                    //else
                    //    GetNameList(txtFastSearch.Text);


                    if (ListName.Count > 0)
                        cboName.SelectedIndex = 0;
                    else
                    {
                        //lblDatetime.Text = "";
                        ShowViewBMP(0);
                    }
                }
            }
            else
                MessageBox.Show("未選擇要刪除的資料，請先選擇所要刪除的資料。", "MAIN", MessageBoxButtons.OK);


        }
        void DeleteTree(int ID)
        {
            if(Directory.Exists(Universal.PICPATH + ID.ToString("0000") + ""))
            {
                Directory.Delete(Universal.PICPATH + ID.ToString("0000") + "", true);
            }
        }
        void ShowViewBMP(int ID)
        {
            if (ID == 0)
            {
                bmpView.Dispose();
                bmpView = new Bitmap(1, 1);
            }
            else
            {
                Bitmap bmp = new Bitmap(1, 1);

                bmp.Dispose();

                if (File.Exists(Universal.PICPATH + ID.ToString("0000") + "\\KB.BMP"))
                {
                    bmp = new Bitmap(Universal.PICPATH + ID.ToString("0000") + "\\KB.BMP");
                }
                else
                {
                    bmp = new Bitmap(1, 1);
                }

                bmpView.Dispose();
                bmpView = new Bitmap(bmp);

                bmp.Dispose();
            }

            //OPScreenUI.SetBMPDirectly(bmpView);
        }
        void lstName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsNeedChange)
            {
                //if (lstName.SelectedIndex != -1)
                //{
                //    lblDatetime.Text = "建立時間：" + ListName[lstName.SelectedIndex].Split(',')[2].PadRight(40, ' ') + " " +
                //       "修改時間：" + ListName[lstName.SelectedIndex].Split(',')[3];

                //    ShowViewBMP(int.Parse(ListName[lstName.SelectedIndex].Split(',')[1]));
                //}
                //else
                //{
                //    lblDatetime.Text = "";
                //    ShowViewBMP(0);
                //}
            }
        }
        void GetNameList()
        {
            GetNameList("XQ4*");
        }
        void GetNameList(string Filter)
        {
            IsNeedChange = false;

            ListName.Clear();
            DataRowCollection OPRows = RECIPEDB.tbl.Rows;

            string Str = "";

            int i = 0;
            while (i < RECIPEDB.RecordCount)
            {
                Str = OPRows[i]["rcp01"].ToString() + "(" + OPRows[i]["rcp06"].ToString() + ")";

                if (Str.IndexOf(Filter.ToUpper()) > -1 || Filter == "XQ4*")
                {
                    ListName.Add(OPRows[i]["rcp01"].ToString() + "(" + OPRows[i]["rcp06"].ToString() + ")," + //Name(Version)
                                OPRows[i]["rcp00"].ToString() + "," +   //ID
                                OPRows[i]["rcp02"].ToString() + "," +   //Create Date
                                OPRows[i]["rcp03"].ToString()           //Modify Date
                                );
                }
                i++;
            }
            ListName.Sort();

            cboName.Items.Clear();
            i = 0;

            while (i < ListName.Count)
            {
                if (ListName[i].Split(',')[1] == Universal.GlobalPassInteger.ToString())
                {
                    itmp = i;
                }
                cboName.Items.Add(ListName[i].Split(',')[0] + " [" + ListName[i].Split(',')[1] + "]");
                i++;
            }

            IsNeedChange = true;
        }

    }
}