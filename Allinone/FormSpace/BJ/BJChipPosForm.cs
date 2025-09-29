using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Allinone.FormSpace.BJ
{
    public partial class BJChipPosForm : Form
    {
        public BJChipPosForm()
        {
            InitializeComponent();
            Load += BJChipPosForm_Load;
        }

        private void BJChipPosForm_Load(object sender, EventArgs e)
        {
            this.Text = "选择拍照模板的位置界面";

            int rowCount = Universal.CipExtend.QcRowCount;
            int colCount = Universal.CipExtend.QcColCount;

            JetEazy.LoggerClass.Instance.WriteLog($"打开模板位置设定 行总数{rowCount} 列总数{colCount}");

            cboRow.Items.Clear();
            cboCol.Items.Clear();

            int i = 0;
            while (i < rowCount)
            {
                cboRow.Items.Add(i);
                i++;
            }
            i = 0;
            while (i < colCount)
            {
                cboCol.Items.Add(i);
                i++;
            }
            if (cboRow.Items.Count > 0)
            {
                cboRow.SelectedIndex = 0;
            }
            if (cboCol.Items.Count > 0)
            {
                cboCol.SelectedIndex = 0;
            }

            btnSelect.Click += BtnSelect_Click;

        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            string row = cboRow.SelectedIndex.ToString();
            string col = cboCol.SelectedIndex.ToString();
            Universal.CipExtend.SetRowCol(row, col);
            JetEazy.LoggerClass.Instance.WriteLog($"设定位置 行{row} 列{col}");
            this.DialogResult = DialogResult.OK;
        }
    }
}
