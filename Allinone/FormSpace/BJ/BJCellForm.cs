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
    public partial class BJCellForm : Form
    {
        public BJCellForm(string[] strs)
        {
            InitializeComponent();

            listBox1.Items.Clear();
            listBox1.Items.AddRange(strs);
            listBox1.SelectedIndexChanged += ListBox1_SelectedIndexChanged;
            if (listBox1.Items.Count > 0)
                listBox1.SelectedIndex = 0;

            button1.Click += Button1_Click;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if(listBox1.SelectedIndex < 0)
            {
                MessageBox.Show("请选择一个cell", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Allinone.Universal.MapCellIndex = listBox1.SelectedIndex;
            this.DialogResult = DialogResult.OK;

        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
    }
}
