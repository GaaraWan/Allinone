using Allinone.BasicSpace;
using Allinone.ControlSpace;
using Allinone.ControlSpace.MachineSpace;
using Allinone.FormSpace.Motor;
using JetEazy;
using JetEazy.BasicSpace;
using MFApiDemo;
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
    public partial class frmSetMutilPosition : Form
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

        Button btnADD;
        Button btnDEL;
        Button btnUpdate;
        Button btnGO;
        Button btnOK;
        Button btnCancel;
        Button btnAXIS;
        Button btnRobot;

        Button btnOnekeyDel;
        Button btnOneKeyAdd;
        Button btnOneKeyInputPositionAdd;
        Button btnMoveGo;
        Button btnGetCurrPosition;
        Button btnSetEndPosition;
        Button btnCalStartEndOffsetXY;

        DataGridView DGVIEW;

        //JzMainSDM2MachineClass MACHINE
        //{
        //    get { return (JzMainSDM2MachineClass)MACHINECollection.MACHINE; }
        //}

        private string m_pos = string.Empty;
        private string myText = "位置设定窗口";
        private string myEName = string.Empty;
        /// <summary>
        /// 用来判定位置个数
        /// </summary>
        private int m_pos_count = 100;

        MachineCollectionClass MACHINECollection
        {
            get
            {
                return Universal.MACHINECollection;
            }
        }

        OneKeyPropertyGridClass oneKeyPropertyGrid = new OneKeyPropertyGridClass();

        public frmSetMutilPosition(string epos)
        {
            m_pos = epos;

            InitializeComponent();

            this.Load += FrmSetMutilPosition_Load;
        }

        private void FrmSetMutilPosition_Load(object sender, EventArgs e)
        {
            //this.Location = new Point(Universal.MainFormLocation.X + 5, Universal.MainFormLocation.Y);

            //this.Text = myText + " " + JzToolsClass.GetEnumDescription(m_ModuleName) + " 最多可設定 " + m_pos_count.ToString() + " 個位置";

            this.Text = myText;
            DGVIEW = dataGridView1;

            btnADD = button1;
            btnDEL = button2;
            btnGO = button3;
            btnOK = button4;
            btnCancel = button5;
            btnAXIS = button6;
            btnUpdate = button7;
            btnOnekeyDel = button8;
            btnOneKeyAdd = button9;
            btnOneKeyInputPositionAdd = button10;
            btnMoveGo = button11;
            btnGetCurrPosition = button12;
            btnRobot = button13;
            btnSetEndPosition = button14;
            btnCalStartEndOffsetXY = button15;

            btnADD.Click += BtnADD_Click;
            btnDEL.Click += BtnDEL_Click;
            btnGO.Click += BtnGO_Click;
            btnOK.Click += BtnOK_Click;
            btnCancel.Click += BtnCancel_Click;
            btnAXIS.Click += BtnAXIS_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnOnekeyDel.Click += BtnOnekeyDel_Click;
            btnOneKeyAdd.Click += BtnOneKeyAdd_Click;
            btnOneKeyInputPositionAdd.Click += BtnOneKeyInputPositionAdd_Click;
            btnMoveGo.Click += BtnMoveGo_Click;
            btnGetCurrPosition.Click += BtnGetCurrPosition_Click;
            btnRobot.Click += BtnRobot_Click;
            btnSetEndPosition.Click += BtnSetEndPosition_Click;
            btnCalStartEndOffsetXY.Click += BtnCalStartEndOffsetXY_Click;

            btnGO.Visible = true;

            propertyGrid1.SelectedObject = oneKeyPropertyGrid;

            FillDisplay();

            DGVIEW.SelectionChanged += DGVIEW_SelectionChanged;
            DGVIEW.RowPostPaint += DGVIEW_RowPostPaint;
            //myPICKUI.SetMirrorGrpIndex(0);

            btnAXIS.Visible = false;
        }

        private void BtnCalStartEndOffsetXY_Click(object sender, EventArgs e)
        {
            string msg = "是否计算起点与终点通过行列\n计算间隔？";

            if (VsMSG.Instance.Question(msg) != DialogResult.OK)
            {
                return;
            }

            if (INI.keycol > 1)
                INI.keyoffsetx = (INI.keyendx - INI.keyx) / (INI.keycol - 1);
            else
                INI.keyoffsetx = 0;

            if (INI.keyrow > 1)
                INI.keyoffsety = (INI.keyendy - INI.keyy) / (INI.keyrow - 1);
            else
                INI.keyoffsety = 0;

            propertyGrid1.SelectedObject = oneKeyPropertyGrid;
        }

        private void BtnSetEndPosition_Click(object sender, EventArgs e)
        {
            string msg = "是否将当前位置设为终点？";

            if (VsMSG.Instance.Question(msg) != DialogResult.OK)
            {
                return;
            }

            string str = MACHINECollection.GetPosition();
            string[] strs = str.Split(',');
            if (strs.Length >= 3)
            {
                INI.keyendx = double.Parse(strs[0]);
                INI.keyendy = double.Parse(strs[1]);
                INI.keyendz = double.Parse(strs[2]);

                propertyGrid1.SelectedObject = oneKeyPropertyGrid;
            }
        }

        MFApiDemo.RobotHCFAForm mRobotHCFAForm = null;
        private void BtnRobot_Click(object sender, EventArgs e)
        {

            switch (Universal.OPTION)
            {
                case OptionEnum.MAIN_SDM3:

                    if (!Universal.IsOpenMotorWindows)
                    {
                        Universal.IsOpenMotorWindows = true;
                        mTouchMotor = new frmTouchMotor();
                        mTouchMotor.Show();
                    }

                    break;
                case OptionEnum.MAIN_SDM2:


                    if (!JetEazy.Universal.IsRobotFormOpen)
                    {
                        JetEazy.Universal.IsRobotFormOpen = true;
                        mRobotHCFAForm =
                                 new MFApiDemo.RobotHCFAForm(((JzMainSDM2MachineClass)MACHINECollection.MACHINE).mRobotHCFA.ApiHandle,
                                 ((JzMainSDM2MachineClass)MACHINECollection.MACHINE).mRobotHCFA.UserId);
                        mRobotHCFAForm.Show();
                    }
                    break;
            }
        }


        private void BtnGetCurrPosition_Click(object sender, EventArgs e)
        {
            string msg = "是否将当前位置设为起点？";

            if (VsMSG.Instance.Question(msg) != DialogResult.OK)
            {
                return;
            }

            string str = MACHINECollection.GetPosition();
            string[] strs = str.Split(',');
            if (strs.Length >= 3)
            {
                INI.keyx = double.Parse(strs[0]);
                INI.keyy = double.Parse(strs[1]);
                INI.keyz = double.Parse(strs[2]);

                propertyGrid1.SelectedObject = oneKeyPropertyGrid;
            }


        }

        private void BtnMoveGo_Click(object sender, EventArgs e)
        {
            if (DGVIEW.Rows.Count <= 0)
                return;

            int rowindex = DGVIEW.CurrentCell.RowIndex;
            if (rowindex == -1)
                return;

            string onStrMsg = " Move表第 " + rowindex + " 行位置？";
            string offStrMsg = " Move表第 " + rowindex + " 行位置？";
            string msg = (true ? offStrMsg : onStrMsg);

            if (VsMSG.Instance.Question(msg) != DialogResult.OK)
            {
                return;
            }

            MACHINECollection.MoveGo();
        }

        private void BtnOneKeyInputPositionAdd_Click(object sender, EventArgs e)
        {
            if (this.DGVIEW.Rows.Count < m_pos_count)
            {
                for (int i = 0; i < INI.keyrow; i++)
                {
                    for (int j = 0; j < INI.keycol; j++)
                    {
                        double x = INI.keyx + INI.keyoffsetx * j;
                        double y = INI.keyy + INI.keyoffsety * i;
                        double z = INI.keyz;

                        int index = this.DGVIEW.Rows.Add();
                        this.DGVIEW.Rows[index].Cells[0].Value = x.ToString("0.000");
                        this.DGVIEW.Rows[index].Cells[1].Value = y.ToString("0.000");
                        this.DGVIEW.Rows[index].Cells[2].Value = z.ToString("0.000");
                    }
                }
            }
        }

        private void BtnOneKeyAdd_Click(object sender, EventArgs e)
        {
            if (this.DGVIEW.Rows.Count < m_pos_count)
            {
                string[] strpos = MACHINECollection.GetPosition().Split(',').ToArray();
                if (Allinone.Universal.IsNoUseIO)
                    strpos = INI.op_keyxyz.Split(',').ToArray();

                double startpos = double.Parse(strpos[0]);
                int icount = (int)numericUpDown2.Value;
                double[] vs = new double[icount];
                for (int i = 0; i < icount; i++)
                {
                    vs[i] = startpos + i * (double)numericUpDown1.Value;
                }

                if (strpos.Length >= 3)
                {
                    for (int i = 0; i < icount; i++)
                    {
                        int index = this.DGVIEW.Rows.Add();
                        this.DGVIEW.Rows[index].Cells[0].Value = vs[i].ToString("0.000");
                        this.DGVIEW.Rows[index].Cells[1].Value = strpos[1];
                        this.DGVIEW.Rows[index].Cells[2].Value = strpos[2];
                    }
                }
            }
        }

        private void BtnOnekeyDel_Click(object sender, EventArgs e)
        {
            DGVIEW.Rows.Clear();
        }

        private void DGVIEW_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
                                                                               e.RowBounds.Location.Y,
                                                                               DGVIEW.RowHeadersWidth - 4,
                                                                               e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                                                       DGVIEW.RowHeadersDefaultCellStyle.Font,
                                                       rectangle,
                                                       DGVIEW.RowHeadersDefaultCellStyle.ForeColor,
                                                       TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void DGVIEW_SelectionChanged(object sender, EventArgs e)
        {
            if (DGVIEW.Rows.Count <= 0)
                return;

            int rowindex = DGVIEW.CurrentCell.RowIndex;
            if (rowindex == -1)
                return;

            //myPICKUI.SetMirrorGrpIndex(rowindex);
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (DGVIEW.Rows.Count <= 0)
                return;

            int rowindex = DGVIEW.CurrentCell.RowIndex;
            if (rowindex == -1)
                return;

            string onStrMsg = "更新 表第 " + rowindex + " 行？";
            string offStrMsg = "更新 表第 " + rowindex + " 行？";
            string msg = (true ? offStrMsg : onStrMsg);

            if (VsMSG.Instance.Question(msg) != DialogResult.OK)
            {
                return;
            }

            string[] strpos = MACHINECollection.GetPosition().Split(',').ToArray();

            if (strpos.Length >= 3)
            {
                this.DGVIEW.Rows[rowindex].Cells[0].Value = strpos[0];
                this.DGVIEW.Rows[rowindex].Cells[1].Value = strpos[1];
                this.DGVIEW.Rows[rowindex].Cells[2].Value = strpos[2];
            }

        }

        //frmAXISSetup mMotorForm = null;
        //frmMotor mMotorFromX1 = null;
        frmTouchMotor mTouchMotor = null;

        private void BtnAXIS_Click(object sender, EventArgs e)
        {
            if (!Universal.IsOpenMotorWindows)
            {
                Universal.IsOpenMotorWindows = true;
                //MACHINE.PLCReadCmdNormalTemp(true);
                //System.Threading.Thread.Sleep(500);

                switch (VERSION)
                {
                    case VersionEnum.ALLINONE:
                        switch (OPTION)
                        {
                            case OptionEnum.MAIN_SDM3:
                            case OptionEnum.MAIN_SDM2:

                                mTouchMotor = new frmTouchMotor();
                                mTouchMotor.Show();
                                //mMotorFromX1 = new frmMotor();
                                //mMotorFromX1.StartPosition = FormStartPosition.Manual;
                                //mMotorFromX1.Location = new Point(this.Location.X + 5, this.Location.Y + 220);
                                //mMotorFromX1.Show();

                                break;
                        }
                        break;
                }


            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {

            string strresult = string.Empty;
            int i = 0;
            while (i < DGVIEW.Rows.Count)
            {
                strresult += DGVIEW.Rows[i].Cells[0].Value.ToString() + ",";
                strresult += DGVIEW.Rows[i].Cells[1].Value.ToString() + ",";
                strresult += DGVIEW.Rows[i].Cells[2].Value.ToString() + ";";
                i++;
            }

            strresult = RemoveLastChar(strresult, 1);
            //m_pos = strresult;
            JetEazy.BasicSpace.JzToolsClass.PassingString = strresult;

            this.DialogResult = DialogResult.OK;
        }

        public string RemoveLastChar(string Str, int Count)
        {
            if (Str.Length < Count)
                return "";

            return Str.Remove(Str.Length - Count, Count);
        }

        private void BtnGO_Click(object sender, EventArgs e)
        {
            if (DGVIEW.Rows.Count <= 0)
                return;

            int rowindex = DGVIEW.CurrentCell.RowIndex;
            if (rowindex == -1)
                return;

            string onStrMsg = " 写入表第 " + rowindex + " 行位置？";
            string offStrMsg = " 写入表第 " + rowindex + " 行位置？";
            string msg = (true ? offStrMsg : onStrMsg);

            if (VsMSG.Instance.Question(msg) != DialogResult.OK)
            {
                return;
            }

            string pos = string.Empty;
            pos += DGVIEW.Rows[rowindex].Cells[0].Value.ToString() + ",";
            pos += DGVIEW.Rows[rowindex].Cells[1].Value.ToString() + ",";
            pos += DGVIEW.Rows[rowindex].Cells[2].Value.ToString();

            //((DispensingMachineClass)MACHINECollection.MACHINE).PLCIO.ModulePositionSet(m_ModuleName, 1, pos);
            //((DispensingMachineClass)MACHINECollection.MACHINE).PLCIO.ModulePositionGO(m_ModuleName, 1);

            MACHINECollection.GoPosition(pos, false);
        }

        private void BtnDEL_Click(object sender, EventArgs e)
        {
            if (DGVIEW.Rows.Count <= 0)
                return;

            int rowindex = DGVIEW.CurrentCell.RowIndex;
            if (rowindex == -1)
                return;

            //string onStrMsg = "删除 表第 " + rowindex + " 行？";
            //string offStrMsg = "删除 表第 " + rowindex + " 行？";
            //string msg = (true ? offStrMsg : onStrMsg);

            //if (VsMSG.Instance.Question(msg) != DialogResult.OK)
            //{
            //    return;
            //}

            DataGridViewSelectedRowCollection rowsCollection = DGVIEW.SelectedRows;
            for (int i = 0; i < rowsCollection.Count; i++)
            {
                DGVIEW.Rows.RemoveAt(rowsCollection[i].Index);
            }

            //DGVIEW.Rows.RemoveAt(rowindex);

        }

        private void BtnADD_Click(object sender, EventArgs e)
        {
            //string strpos = AXIS_0.PositionNowString + "," + AXIS_1.PositionNowString + "," + AXIS_2.PositionNowString;

            if (this.DGVIEW.Rows.Count < m_pos_count)
            {

                //string[] strpos = "0,0,0".Split(',').ToArray();

                string[] strpos = MACHINECollection.GetPosition().Split(',').ToArray();

                if (strpos.Length >= 3)
                {
                    int index = this.DGVIEW.Rows.Add();
                    this.DGVIEW.Rows[index].Cells[0].Value = strpos[0];
                    this.DGVIEW.Rows[index].Cells[1].Value = strpos[1];
                    this.DGVIEW.Rows[index].Cells[2].Value = strpos[2];
                }
            }


            //this.DGVIEW.Rows[index].Cells[0].Value = AXIS_0.PositionNowString;
            //this.DGVIEW.Rows[index].Cells[1].Value = AXIS_1.PositionNowString;
            //this.DGVIEW.Rows[index].Cells[2].Value = AXIS_2.PositionNowString;
        }


        void FillDisplay()
        {
            DGVIEW.Rows.Clear();
            List<string> listpos = m_pos.Split(';').ToList();
            if (listpos.Count > 0)
            {
                foreach (string str in listpos)
                {
                    List<string> listpostemp = str.Split(',').ToList();
                    if (listpostemp.Count == 3)
                    {
                        if (string.IsNullOrEmpty(listpostemp[0]) || string.IsNullOrEmpty(listpostemp[1]) || string.IsNullOrEmpty(listpostemp[2]))
                        {

                        }
                        else
                        {
                            if (this.DGVIEW.Rows.Count < m_pos_count)
                            {
                                int index = this.DGVIEW.Rows.Add();
                                this.DGVIEW.Rows[index].Cells[0].Value = listpostemp[0];
                                this.DGVIEW.Rows[index].Cells[1].Value = listpostemp[1];
                                this.DGVIEW.Rows[index].Cells[2].Value = listpostemp[2];
                            }
                        }
                    }
                }
            }
        }
    }
}
