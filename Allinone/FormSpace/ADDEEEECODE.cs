using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Allinone.FormSpace
{
    public partial class ADDEEEECODE : Form
    {
        Button BTNSET;
        Button BTNCLOSE;
        ListBox LSBLIST;

        TextBox txbData;

        string PATH;
        public ADDEEEECODE( string STRPATH)
        {
            InitializeComponent();
            PATH = STRPATH;
            txbData = textBox1;
            BTNSET = button1;
            BTNCLOSE = button2;
            this.Load += ADDEEEECODE_Load;

            BTNSET.Click += BTNSET_Click;
            BTNCLOSE.Click += BTNCLOSE_Click;
            this.TopMost = true;
            this.CenterToParent();
        }

        private void BTNCLOSE_Click(object sender, EventArgs e)
        {
            this.Close  ();

            JetEazy.LoggerClass.Instance.WriteLog("放弃保存 EEEE CODE");
        }

        private void BTNSET_Click(object sender, EventArgs e)
        {
            string STRSAVE = txbData.Text.Trim() ;
            

            StreamWriter writer = new StreamWriter(PATH,false );
            writer.Write(STRSAVE);
            writer.Close();
            this.Close();

            JetEazy.LoggerClass.Instance.WriteLog("保存了 EEEE CODE");
        }

        private void ADDEEEECODE_Load(object sender, EventArgs e)
        {
            File.Copy(PATH, PATH +"."+ DateTime.Now.ToString("yyyyMMddHHmmss"));
            StreamReader reader = new StreamReader(PATH);
            string TEMP;
            txbData.Text = "";
            while ((TEMP =reader.ReadLine())!=null)
            {
                if(TEMP!="")
                    txbData.Text+=TEMP+Environment.NewLine ;
                
            }
            reader.Close();
        }
    }
}
