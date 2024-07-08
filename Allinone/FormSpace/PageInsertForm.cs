using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using JetEazy.BasicSpace;
using Allinone.OPSpace;

namespace Allinone.FormSpace
{
    public partial class PageInsertForm : Form
    {
        ListBox lsbCTOItem;

        Button btnInsert;
        Button btnAdd;
        Button btnCancel;

        //ViewItemClass CTOViewItem;
        AlbumClass ALBUM;

        public PageInsertForm(AlbumClass album)
        {
            //CTOViewItem = ctoviewitem;

            ALBUM = album;
            InitializeComponent();

            Initial();
        }

        void Initial()
        {
            lsbCTOItem = listBox1;

            btnInsert = button1;
            btnAdd = button4;
            btnCancel = button6;

            btnInsert.Click += new EventHandler(btnInsert_Click);
            btnAdd.Click += new EventHandler(btnAdd_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);

            FillPageName();
        }
        void FillPageName()
        {
            foreach (PageClass page in ALBUM.ENVList[0].PageList)
            {
                lsbCTOItem.Items.Add(page.ToPageSelectString_RelateToVersion());
            }

            if (lsbCTOItem.Items.Count > 0)
                lsbCTOItem.SelectedIndex = 0;
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        void btnAdd_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        void btnInsert_Click(object sender, EventArgs e)
        {
            JzToolsClass.PassingInteger = lsbCTOItem.SelectedIndex;

            this.DialogResult = DialogResult.Yes;
        }
    }
}
