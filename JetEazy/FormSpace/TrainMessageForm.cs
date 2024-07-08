#define M_Parallel
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using JetEazy.BasicSpace;

namespace JetEazy.FormSpace
{
    public partial class TrainMessageForm : Form
    {
        ListBox lstMessage;
        Button btnOK;
        public bool isInvoke = true;
        string DataSaveString = "";

        public TrainMessageForm(string datasavestring)
        {
            DataSaveString = datasavestring;
            
            InitializeComponent();
            Initial();
        }
        void Initial()
        {
            lstMessage = listBox1;
            btnOK = button4;

            btnOK.Click += new EventHandler(btnOK_Click);

            btnOK.Visible = false;

            this.Load+=new EventHandler(TrainMessageForm_Load);
        }

        void  TrainMessageForm_Load(object sender, EventArgs e)
        {
            //if (!Universal.IsDebug)
            //    this.TopMost = true;

            this.Text = "Train For " + DataSaveString;
            
            this.Refresh();
        }

        public void SetString(List<string> strlist)
        {
            lock (strlist)
            {
                foreach (string str in strlist)
                {
                    SetString(str);
                }
            }
        }

        public void SetString(string Str)
        {
#if(M_Parallel)
            if ( Universal.isRcpUIOKClick && Universal.IsMultiThread )
                SetListPropertyValue(lstMessage, Str);
            else
                SetListPropertyValue_Invoke(lstMessage, Str);
#else
            int i = 0;

            if (lstMessage.Items.Count > 5000)
            {
                i = 0;
                while (i < 20)
                {
                    lstMessage.Items.RemoveAt(0);
                    i++;
                }
            }


            lstMessage.Items.Add(Str);

            lstMessage.SelectedIndex = lstMessage.Items.Count - 1;
#endif
        }

        public void SetComplete()
        {
            SetComplete(false);
        }

        public void SetComplete(bool isstay)
        {
            btnOK.Visible = true;
#if(M_Parallel)
            SetListPropertyValue(lstMessage, "Complete and press OK to continue...");
#else
            lstMessage.Items.Add("Complete and press OK to continue...");

            lstMessage.SelectedIndex = lstMessage.Items.Count - 1;
#endif

            if (!isstay)
            {
                DataSave();
                this.Close();
            }
        }
        public void SetCancel()
        {
            btnOK.Visible = true;

#if(M_Parallel)
            SetListPropertyValue(lstMessage, "Have errors and press OK to exit...");
#else
            lstMessage.Items.Add("Have errors and press OK to exit...");

            lstMessage.SelectedIndex = lstMessage.Items.Count - 1;
#endif
            btnOK.BackColor = Color.Red;
        }
        void btnOK_Click(object sender, EventArgs e)
        {
            DataSave();
            this.Close();
        }

        void DataSave()
        {
            string Str = "";

            JzToolsClass myJzTools = new JzToolsClass();

            int i = 0;

            while (i < lstMessage.Items.Count)
            {
                Str += lstMessage.Items[i].ToString() + Environment.NewLine;
                i++;
            }

            myJzTools.SaveData(Str, @"D:\LOA\" + DataSaveString + ".log");

        }

        delegate void SetListValueCallback(ListBox lst, object propValue);
        private void SetListPropertyValue(ListBox lst, string propValue)
        {
            lst.BeginInvoke (new Action<string>((string value) =>
            {
                lst.Items.Add(value.ToString());
                lst.SelectedIndex = lstMessage.Items.Count - 1;
                lst.Refresh();
            }), propValue);

            //if (lst.InvokeRequired)
            //{
            //    SetListValueCallback d = new SetListValueCallback(SetListPropertyValue);
            //    lst.Invoke(d, new object[] { lst, propValue });
            //}
            //else
            //{
            //    int i = 0;

            //    if (lst.Items.Count > 5000)
            //    {
            //        i = 0;
            //        while (i < 20)
            //        {
            //            lst.Items.RemoveAt(0);
            //            i++;
            //        }
            //    }


            //    lst.Items.Add(propValue.ToString());
            //    lst.SelectedIndex = lstMessage.Items.Count - 1;
              
            //    //lst.Items.Add(propValue.ToString());
            //    //lst.SelectedIndex = lst.Items.Count - 1;
            //}
        }

        private void SetListPropertyValue_Invoke(ListBox lst, string propValue)
        {
            lst.Invoke(new Action<string>((string value) =>
            {
                lst.Items.Add(value.ToString());
                lst.SelectedIndex = lstMessage.Items.Count - 1;
                lst.Refresh();
            }), propValue);
            
        }
    }
}
