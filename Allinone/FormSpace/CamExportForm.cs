using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JetEazy.BasicSpace;

namespace Allinone.FormSpace
{
    public partial class CamExportForm : Form
    {
        enum TagEnum
        {
            ADD,
            DEL,
            EQUAL,
            
            CLEAR,

            FINISH,
            CANCEL,                         
        }
        string EnterStr = "";
        int CamCount = 10;

        Button btnAdd;
        Button btnDel;
        Button btnEqual;
       
        Button btnClear_Data;

        Button btnFinish;
        Button btnCancel;

        ComboBox cboCAMNO;

        NumericUpDown numUDExposure;

        DataGridView CAMExposure_Data;        

        public CamExportForm()
        {
            InitializeComponent();
            InitialInternal();
        }
        public CamExportForm(string enterstr,int camcount)
        {
            EnterStr = enterstr;
            CamCount = camcount;

            InitializeComponent();
            InitialInternal();
        }
        void InitialInternal()
        {
            btnAdd = button1;
            btnDel = button2;
            btnEqual = button3;
            btnClear_Data = button4;
            btnFinish = button5;
            btnCancel = button6;

            cboCAMNO = comboBox1;

            numUDExposure = numericUpDown1;

            CAMExposure_Data = dataGridView1;            

            btnAdd.Tag = TagEnum.ADD;
            btnDel.Tag = TagEnum.DEL;
            btnEqual.Tag = TagEnum.EQUAL;
            btnClear_Data.Tag = TagEnum.CLEAR;
            btnFinish.Tag = TagEnum.FINISH;
            btnCancel.Tag = TagEnum.CANCEL;
                       
            btnAdd.Click += Btn_Click;
            btnDel.Click += Btn_Click;
            btnEqual.Click += Btn_Click;
            btnClear_Data.Click += Btn_Click;
            btnFinish.Click += Btn_Click;
            btnCancel.Click += Btn_Click;

            cboCAMNO.SelectedIndexChanged += cboCAMNO_SelectedIndexChanged;
            CAMExposure_Data.MouseClick += CAMExposure_Data_MouseClick;
            numUDExposure.ValueChanged += NumUDExposure_ValueChanged;


            this.Load += CamExportForm_Load;

        }

        private void NumUDExposure_ValueChanged(object sender, EventArgs e)
        {
            EQUAL();
        }

        private void CamExportForm_Load(object sender, EventArgs e)
        {
            CameraExposure_Load();
        }

        static int Count_;

        public string[] CAMNnb;

        public string OutStr;

        bool listclear = false;

        int Strlength;

         void CameraExposure_Load()
        {
            string[] tmpEntreStr;
            if (CamCount > 0)
            {
                if (EnterStr != string.Empty)
                {
                    tmpEntreStr = EnterStr.Split(',');
                    if (tmpEntreStr.Length <= CamCount)
                    {
                        Strlength = tmpEntreStr.Length;
                        CAMCount_table(Strlength, tmpEntreStr);
                    }
                    //else
                    //{
                    //    MessageBox.Show("相機數與數值不符!");
                    //}               
                }
                else
                {
                    EnterStr_Empty();

                    btnDel.Enabled = false;
                    listclear = true;
                }

                for (int i = 0; i < CamCount; i++)
                    cboCAMNO.Items.Add(i);

                cboCAMNO.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("無相機數!");
                btnAdd.Enabled = false;
                btnDel.Enabled = false;
                listclear = true;
            }
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            switch ((TagEnum)btn.Tag)
            {
                case TagEnum.ADD:
                    ADD();
                    break;
                case TagEnum.DEL:
                    DEL();
                    break;
                case TagEnum.EQUAL:
                    EQUAL();
                    break;
                case TagEnum.CLEAR:
                    ClearData();
                    break;
                case TagEnum.FINISH:
                    Finish();
                    break;
                case TagEnum.CANCEL:
                    this.DialogResult = DialogResult.Cancel;
                    break;               
            }
        }
        
        bool fristOpen = true;

        private void cboCAMNO_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (fristOpen)
                {
                    if (EnterStr != string.Empty)
                    {
                        cboCAMNO.SelectedIndex = int.Parse(CAMExposure_Data.Rows[0].Cells[0].Value.ToString());
                        numUDExposure.Value = Decimal.Parse(CAMExposure_Data.Rows[0].Cells[1].Value.ToString());
                    }

                    fristOpen = false;
                }

                for (int i = 0; i < CAMExposure_Data.RowCount; i++)
                {
                    if (cboCAMNO.SelectedIndex == int.Parse(CAMExposure_Data.Rows[i].Cells[0].Value.ToString()))
                        numUDExposure.Value = Decimal.Parse(CAMExposure_Data.Rows[i].Cells[1].Value.ToString());
                }
            }
            catch(Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
                MessageBox.Show("当前设定与相机数目不符！将清空原有设定。");
            }
        }

        bool fristOpCam = true;
        private void CAMExposure_Data_MouseClick(object sender, MouseEventArgs e)
        {
            bool IsHaveMessYN = false;
            if (CAMExposure_Data.RowCount != 0)
            {
                if (cboCAMNO.SelectedIndex != -1)
                {

                    for (int i = 0; i < CAMExposure_Data.RowCount; i++)
                    {
                        if (cboCAMNO.SelectedIndex == int.Parse(CAMExposure_Data.Rows[i].Cells[0].Value.ToString()))
                        {
                            if (numUDExposure.Value != Decimal.Parse(CAMExposure_Data.Rows[i].Cells[1].Value.ToString()))
                            {
                                DialogResult dr = MessageBox.Show("上一筆資料沒確實更改到，是否需要回到上一筆資料更改。", "注意", MessageBoxButtons.YesNo);
                                if (dr == DialogResult.No)
                                {
                                    cboCAMNO.SelectedIndex = int.Parse(CAMExposure_Data.Rows[CAMExposure_Data.CurrentRow.Index].Cells[0].Value.ToString());
                                    numUDExposure.Value = Decimal.Parse(CAMExposure_Data.Rows[CAMExposure_Data.CurrentRow.Index].Cells[1].Value.ToString());
                                    IsHaveMessYN = true;
                                }
                                else if (dr == DialogResult.Yes)
                                {
                                    CAMExposure_Data.Rows[i].Selected = true;
                                    cboCAMNO.SelectedIndex = int.Parse(CAMExposure_Data.Rows[i].Cells[0].Value.ToString());
                                    IsHaveMessYN = true;
                                }
                                break;
                            }
                        }
                    }
                    if (!IsHaveMessYN)
                    {
                        cboCAMNO.SelectedIndex = int.Parse(CAMExposure_Data.CurrentRow.Cells[0].Value.ToString());                       
                    }
                    if (fristOpCam)
                    {
                        numUDExposure.Value = Decimal.Parse(CAMExposure_Data.Rows[CAMExposure_Data.CurrentRow.Index].Cells[1].Value.ToString());
                        fristOpCam = false;
                    }
                }
                else
                {
                    cboCAMNO.SelectedIndex = CAMExposure_Data.CurrentRow.Index;
                }
            }
        }

        void CAMCount_table(int Count, String[] EntreStr)
        {
            string[] CAMExposure;

            DataGridViewColumnCollection Column = CAMExposure_Data.Columns;
            Column.Add("", "相機編號");
            Column.Add("", "數值");

            CAMExposure_Data.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            CAMExposure_Data.Columns[0].Width = 70;
            CAMExposure_Data.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            CAMExposure_Data.Columns[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            DataGridViewRowCollection Rows = CAMExposure_Data.Rows;
            for (int i = 0; i < Count; i++)
            {
                CAMExposure = EntreStr[i].Split(':');
                Rows.Add(new Object[] { CAMExposure[0], CAMExposure[1] });
            }


            CAMExposure_Data.Sort(CAMExposure_Data.Columns[0], System.ComponentModel.ListSortDirection.Ascending);
            CAMExposure_Data.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;

            //cboCAMNO.SelectedIndex = 0;
        }

        void EnterStr_Empty()
        {
            //for (int i = 0; i < CamCount; i++)
            //    cboCAMNO.Items.Add(i);

            DataGridViewColumnCollection Column = CAMExposure_Data.Columns;
            Column.Add("", "相機編號");
            Column.Add("", "數值");

            CAMExposure_Data.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            CAMExposure_Data.Columns[0].Width = 70;
            CAMExposure_Data.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            CAMExposure_Data.Columns[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            CAMExposure_Data.Sort(CAMExposure_Data.Columns[0], System.ComponentModel.ListSortDirection.Ascending);
            CAMExposure_Data.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        void ADD()
        {
            bool IsHaveSame = false;
            if (cboCAMNO.SelectedIndex != -1)
            {
                for (int i = 0; i < CAMExposure_Data.RowCount; i++)
                {
                    if (cboCAMNO.SelectedIndex == int.Parse(CAMExposure_Data.Rows[i].Cells[0].Value.ToString()))
                    {
                        DialogResult dr = MessageBox.Show("已經有相同編號的相機編號!", "注意");
                        IsHaveSame = true;
                        break;
                    }
                }

                if (!IsHaveSame)
                {
                    DataGridViewRowCollection Rows = CAMExposure_Data.Rows;
                    Rows.Add(new Object[] { cboCAMNO.Text.ToString(), numUDExposure.Value.ToString() });
                    CAMExposure_Data.Rows[CAMExposure_Data.RowCount - 1].Selected = true;
                    Count_ = CAMExposure_Data.RowCount;
                    for (int j = CAMExposure_Data.RowCount - 1; j > 0; j--)
                    {
                        DataRowChange(CAMExposure_Data.Rows[j].Cells[0], CAMExposure_Data.Rows[j - 1].Cells[0], CAMExposure_Data.Rows[j].Cells[1], CAMExposure_Data.Rows[j - 1].Cells[1]);
                    }
                    CAMExposure_Data.Rows[Count_ - 1].Selected = true;
                    if (listclear)
                    {
                        btnDel.Enabled = true;
                        listclear = false;
                    }

                }
            }else
            {
                MessageBox.Show("無相機數!");
            }
        }

        void DataRowChange(DataGridViewCell NewNo, DataGridViewCell No, DataGridViewCell NewNumber, DataGridViewCell Number)
        {
            int tmpNo, tmpNumber;

            if (int.Parse(NewNo.Value.ToString()) < int.Parse(No.Value.ToString()))
            {
                tmpNo = int.Parse(No.Value.ToString());
                No.Value = NewNo.Value;
                NewNo.Value = tmpNo;

                tmpNumber = int.Parse(Number.Value.ToString());
                Number.Value = NewNumber.Value;
                NewNumber.Value = tmpNumber;
                Count_ = Count_ - 1;
            }
        }

        void DEL()
        {
            DialogResult dr = MessageBox.Show("確認要刪除此相機編號:" + CAMExposure_Data.CurrentRow.Cells[0].Value.ToString(), "注意", MessageBoxButtons.YesNo);

            if (dr == DialogResult.Yes)
            {
                if (CAMExposure_Data.RowCount > 1)
                {
                    CAMExposure_Data.Rows.Remove(CAMExposure_Data.CurrentRow);
                    CAMExposure_Data.CurrentRow.Selected = true;
                    cboCAMNO.SelectedIndex = int.Parse(CAMExposure_Data.CurrentRow.Cells[0].Value.ToString());
                    numUDExposure.Value = Decimal.Parse(CAMExposure_Data.CurrentRow.Cells[1].Value.ToString());
                }
                else
                {
                    CAMExposure_Data.Rows.Remove(CAMExposure_Data.CurrentRow);
                    listclear = true;
                    btnDel.Enabled = false;
                }
            }
        }

        void EQUAL()
        {
            bool IsHaveTheCam = false;
            for (int i = 0; i < CAMExposure_Data.RowCount; i++)
            {
                if (cboCAMNO.SelectedIndex == int.Parse(CAMExposure_Data.Rows[i].Cells[0].Value.ToString()))
                {
                    CAMExposure_Data.Rows[i].Cells[1].Value = numUDExposure.Value; ;
                    IsHaveTheCam = true;
                    break;
                }
            }

            if (!IsHaveTheCam)
            {
                MessageBox.Show("表單上無此號相機!");
            }
        }

        void ClearData()
        {
            int DataRowCount = CAMExposure_Data.RowCount;
            for (int i = 0; i < DataRowCount; i++)
                CAMExposure_Data.Rows.Remove(CAMExposure_Data.Rows[0]);
            btnDel.Enabled = false;
            listclear = true;
        }

        void Finish()
        {
            string tmpoutstr = string.Empty;
            for (int i = 0; i < CAMExposure_Data.RowCount; i++)
            {
                tmpoutstr = tmpoutstr + CAMExposure_Data.Rows[i].Cells[0].Value.ToString() + ":" + CAMExposure_Data.Rows[i].Cells[1].Value.ToString() + ",";
            }

            if (tmpoutstr == string.Empty)
            {
                //MessageBox.Show("表格內無資料,將送出空值!");
                OutStr = "";
            }
            else
            {
                OutStr = tmpoutstr.Substring(0, tmpoutstr.Length - 1);
            }

            JzToolsClass.PassingString = OutStr;

            this.DialogResult = DialogResult.OK;
        }
    }
}
