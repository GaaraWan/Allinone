using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using JetEazy;
using Allinone.OPSpace;

namespace Allinone.FormSpace
{
    public partial class ListForm : Form
    {
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

        List<AnalyzeClass> AnalyzeList;

        Button btnOK;
        Button btnCancel;

        DataGridView dgvAnalyze;
        DataTable AnalyzeTable = new DataTable();

        public ListForm(List<AnalyzeClass> analyzelist)
        {
            InitializeComponent();
            InitialInside(analyzelist);
        }
        void InitialInside(List<AnalyzeClass> analyzelist)
        {
            AnalyzeList = analyzelist;

            dgvAnalyze = dataGridView1;
            dgvAnalyze.AllowUserToAddRows = false;
            dgvAnalyze.AllowUserToDeleteRows = false;

            dgvAnalyze.RowPrePaint += DgvAnalyze_RowPrePaint;

            btnOK = button4;
            btnCancel = button6;

            btnOK.Click += BtnOK_Click;
            btnCancel.Click += BtnCancel_Click;

        }

        private void DgvAnalyze_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            ChangeColor();
        }

        protected override void OnLoad(EventArgs e)
        {
            Initial();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            int i = 0;

            foreach (AnalyzeClass analyze in AnalyzeList)
            {
                analyze.IsUsed = false;
            }

            while (i < AnalyzeTable.Rows.Count)
            {
                foreach (AnalyzeClass analyze in AnalyzeList)
                {
                    if(analyze.IsUsed)
                        continue;

                    analyze.SetData(AnalyzeTable.Rows[i]);

                    if (analyze.IsUsed)
                        break;
                }
                i++;
            }

            this.DialogResult = DialogResult.OK;
        }

        void Initial()
        {
            int i = 0;

            AnalyzeTable.Columns.Clear();
            AnalyzeTable.Clear();

            //先把 Data Column 造出來
            DataColumn[] datacolumns = AnalyzeClass.GetDataColums(VERSION,OPTION);

            i = 0;
            foreach(DataColumn datacolumn in datacolumns)
            {
                switch(i)
                {
                    //case 8:

                    //    DataGridViewComboBoxColumn maskcolumn = new DataGridViewComboBoxColumn();
                    //    maskcolumn.DataPropertyName = "MaskMethod";
                    //    AnalyzeTable.Columns.Add(maskcolumn);

                    //    break;
                    default:
                        AnalyzeTable.Columns.Add(datacolumn);
                        break;
                }
                i++;
            }

            //先檢查是否全無選，全無選則全數列出
            bool isallnoselect = true;

            foreach(AnalyzeClass analyze in AnalyzeList)
            {
                if (analyze.IsSelected)
                {
                    isallnoselect = false;
                    break;
                }
            }
            
            foreach(AnalyzeClass analyze in AnalyzeList)
            {
                //選中的才看
                if(analyze.IsSelected || isallnoselect)
                    analyze.AddNewRow(AnalyzeTable);
            }

            dgvAnalyze.DataSource = AnalyzeTable;

            ChangeColor();
        }

        void ChangeColor()
        {
            foreach (DataGridViewRow drow in dgvAnalyze.Rows)
            {
                Color mycolor = Color.Red;

                switch (((UInt32)drow.Cells[3].Value) % 8)
                {
                    case 1:
                        mycolor = Color.Orange;
                        break;
                    case 2:
                        mycolor = Color.Lime;
                        break;
                    case 3:
                        mycolor = Color.Pink;
                        break;
                    case 4:
                        mycolor = Color.LightGreen;
                        break;
                    case 5:
                        mycolor = Color.Lavender;
                        break;
                    case 6:
                        mycolor = Color.DeepPink;
                        break;
                    case 7:
                        mycolor = Color.Honeydew;
                        break;
                    case 0:
                        mycolor = Color.Gold;
                        break;

                }

                drow.DefaultCellStyle.BackColor = mycolor;
            }

        }

    }
}
