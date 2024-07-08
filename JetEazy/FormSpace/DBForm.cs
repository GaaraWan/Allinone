using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using System.Data.OleDb;

using JetEazy;
using JetEazy.BasicSpace;

namespace JetEazy.FormSpace
{
    public partial class DBForm : Form
    {
        ListBox lstDB;
        ListBox lstProcess;

        TextBox txtDBName;
        TextBox txtSerial;

        Button btnOK;
        Button btnCancel;

        OleDbConnection LOGCN;

        string DefaultString = "";
        string LOGDBPathString = "";
        string LOGTXTPathString = "";

        public DBForm(ref OleDbConnection cn,string defaultstr,string logdbpathstring,string logtxtpathstring)
        {
            LOGDBPathString = logdbpathstring;
            LOGTXTPathString = logtxtpathstring;

            InitializeComponent();
            Initial(ref cn,defaultstr);
        }

        void Initial(ref OleDbConnection cn,string defaultstr)
        {
            LOGCN = cn;
            DefaultString = defaultstr;

            lstDB = listBox1;
            lstProcess = listBox2;
            txtDBName = textBox1;
            txtSerial = textBox2;

            btnOK = button1;
            btnCancel = button2;

            lstDB.Items.Add("(Blank)");

            FindDirectoryDB();

            lstDB.SelectedIndexChanged += new EventHandler(lstDB_SelectedIndexChanged);
            lstProcess.SelectedIndexChanged += new EventHandler(lstProcess_SelectedIndexChanged);
            btnOK.Click += new EventHandler(btnOK_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);

        }

        void lstProcess_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(lstProcess.SelectedIndex > -1)
            {
                txtSerial.Text = lstProcess.Text;
            }
        }
        void FindDirectoryDB()
        {
            string [] Dir = Directory.GetFiles(LOGDBPathString, "L*.MDB");

            string Str = "";
            string[] Strs;
            int i = 0;
            while (i < Dir.Length)
            {
                Str = Dir[i].Split('.')[0];

                Strs = Str.Split('\\');

                lstDB.Items.Add(Strs[Strs.Length - 1].Remove(0, 1));
                i++;
            }
        }

        void lstDB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDB.SelectedIndex > -1)
            {
                if (lstDB.Text != "(Blank)")
                {
                    txtDBName.Text = lstDB.Text;
                    txtSerial.Text = "";

                    FillProcess();
                }
                else
                {
                    txtDBName.Text = "";
                    txtSerial.Text = "";
                    lstProcess.Items.Clear();
                }

            }
        }
        void FillProcess()
        {
            string LogcnString = DefaultString;

            LOGCN.Close();
            //LOGCN.Dispose();

            LOGCN.ConnectionString = LogcnString.Replace("Template", "L" + txtDBName.Text);
            //LOGCN = new OleDbConnection(LogcnString.Replace("Template", "L" + txtDBName.Text));
            LOGCN.Open();

            String SQLStr = "SELECT log03 FROM logdb ORDER by log03"; 
            OleDbDataAdapter da;
            OleDbCommandBuilder cb;
            DataTable tbl = new DataTable();

            da = new OleDbDataAdapter(SQLStr, LOGCN);
            cb = new OleDbCommandBuilder(da);

            da.Fill(tbl);


            int i = 0;
            string Str = "";
            string LastStr = "";
            lstProcess.Items.Clear();

            while (i < tbl.Rows.Count)
            {
                Str = ((string)tbl.Rows[i][0]).Split(',')[0].Trim();

                if (LastStr != Str)
                {
                    lstProcess.Items.Add(Str);
                    LastStr = Str;
                }
                i++;
            }

            tbl.Clear();
            tbl.Dispose();
            cb.Dispose();
            da.Dispose();

            LOGCN.Close();
        }


        void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        void btnOK_Click(object sender, EventArgs e)
        {
            int i = 0;
            bool IsAlive = false;

            if (txtDBName.Text != "" && txtSerial.Text == "")
            {
                MessageBox.Show("製程編碼不得為空白。", "MAIN", MessageBoxButtons.OK);
                txtSerial.Focus();
            }
            else if (txtDBName.Text == "" && txtSerial.Text == "")
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                while (i < lstDB.Items.Count)
                {
                    if (lstDB.Items[i].ToString() == txtDBName.Text.Trim())
                    {
                        IsAlive = true;
                        break;
                    }
                    i++;
                }

                if (!IsAlive)
                {
                    CreateDB();
                }
                else
                {
                    LOGCN.Close();
                    //LOGCN.Dispose();

                    LOGCN.ConnectionString = DefaultString.Replace("Template", "L" + txtDBName.Text);
                    LOGCN.Open();

                    LOGCN.Close();
                    //LOGCN = new OleDbConnection(DefaultString.Replace("Template", "L" + txtDBName.Text));
                }

                if (!Directory.Exists(LOGTXTPathString + "\\" + txtDBName.Text))
                    Directory.CreateDirectory(LOGTXTPathString + "\\" + txtDBName.Text);

                if (!Directory.Exists(LOGTXTPathString + "\\" + txtDBName.Text + "\\" + txtSerial.Text))
                    Directory.CreateDirectory(LOGTXTPathString + "\\" + txtDBName.Text + "\\" + txtSerial.Text);

                JzToolsClass.PassingString = txtDBName.Text.Trim() + "-" + txtSerial.Text.Trim();


                this.DialogResult = DialogResult.OK;
            }
        }
        void CreateDB()
        {
            File.Copy(LOGDBPathString + "\\Template.mdb", LOGDBPathString + "\\L" + txtDBName.Text + ".mdb");

            LOGCN.Close();
            //LOGCN.Dispose();

            LOGCN.ConnectionString = DefaultString.Replace("Template", "L" + txtDBName.Text);
            LOGCN.Open();

            LOGCN.Close();
            //LOGCN = new OleDbConnection(DefaultString.Replace("Template", "L" + txtDBName.Text));

        }
    }
}