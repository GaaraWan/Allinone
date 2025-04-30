using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JetEazy.FormSpace
{
    public partial class LanguageForm : Form
    {
        ListBox lstLanguageList;

        public LanguageForm()
        {
            InitializeComponent();
            this.Load += LanguageForm_Load;
        }

        private void LanguageForm_Load(object sender, EventArgs e)
        {
            lstLanguageList = listBox1;
            lstLanguageList.Items.Clear();
            for (int i = 0; i < JetEazy.BasicSpace.LanguageExClass.Instance.LanguageList.Count; i++)
            {
                lstLanguageList.Items.Add(JetEazy.BasicSpace.LanguageExClass.Instance.LanguageList[i]);
            }

            if (lstLanguageList.Items.Count > 0)
                lstLanguageList.SelectedIndex = JetEazy.BasicSpace.LanguageExClass.Instance.LanguageIndex;

            lstLanguageList.SelectedIndexChanged += LstLanguageList_SelectedIndexChanged;
        }

        private void LstLanguageList_SelectedIndexChanged(object sender, EventArgs e)
        {
            JetEazy.BasicSpace.LanguageExClass.Instance.LanguageIndex = lstLanguageList.SelectedIndex;
        }
    }
}
