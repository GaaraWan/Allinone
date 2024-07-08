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
using JzASN.OPSpace;
using Allinone.OPSpace;

namespace Allinone.FormSpace
{
    public partial class CPDListForm : Form
    {
        ListBox lsbElement;

        Button btnOK;
        Button btnCancel;

        public CPDListForm(AlbumClass album,ASNCollectionClass asncollection)
        {
            InitializeComponent();
            InitialInside(album, asncollection);
        }
        void InitialInside(AlbumClass album,ASNCollectionClass asncollection)
        {
            lsbElement = listBox1;

            btnOK = button4;
            btnCancel = button6;

            btnOK.Click += BtnOK_Click;
            btnCancel.Click += BtnCancel_Click;

            foreach (ASNClass asn in asncollection.myDataList)
            {
                lsbElement.Items.Add(asn.ToCPDListString());
            }

            foreach (EnvClass env in album.ENVList)
            {
                foreach(PageClass page in env.PageList)
                {
                    int i = 0;

                    while (i < Universal.PAGEOPTYPECOUNT)
                    {
                        lsbElement.Items.Add(env.ToEnvString() + "-" + page.ToPageIndexString() + "-" + i.ToString("00"));
                        i++;
                    }
                }
            }

            
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            JzToolsClass.PassingString = "";

            int i = 0;

            while (i < lsbElement.SelectedItems.Count)
            {
                JzToolsClass.PassingString += lsbElement.SelectedItems[i].ToString() + Universal.NewlineChar;

                i++;
            }

            if (JzToolsClass.PassingString != "")
            {
                JzToolsClass.PassingString = JzToolsClass.PassingString.Remove(JzToolsClass.PassingString.Length - 1, 1);
                this.DialogResult = DialogResult.OK;
            }
            else
                this.DialogResult = DialogResult.Cancel;
        }
    }
}
