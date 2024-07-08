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
    public partial class AlarmQueryForm : Form
    {
        private string m_alarm_log_path = @"D:\EVENTLOG\ALARM_MAINSD_LOG";

        public AlarmQueryForm()
        {
            Universal.IsOpenAlarmWindows = true;

            InitializeComponent();

            this.Load += AlarmQueryForm_Load;
            this.FormClosed += AlarmQueryForm_FormClosed;
            btnQueryAlarmMsg.Click += BtnQueryAlarmMsg_Click;
        }

        private void BtnQueryAlarmMsg_Click(object sender, EventArgs e)
        {
            _queryDataAlarmMsg(dateTimePicker1.Value, dateTimePicker2.Value);
        }

        private void AlarmQueryForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Universal.IsOpenAlarmWindows = false;
        }

        private void AlarmQueryForm_Load(object sender, EventArgs e)
        {
            this.Text = "报警记录查询";
            _queryDataAlarmMsg(DateTime.Now, DateTime.Now);
        }




        private void _queryDataAlarmMsg(DateTime dt1, DateTime dt2)
        {
            double d1 = double.Parse(dt1.ToString("yyyyMMdd"));
            double d2 = double.Parse(dt2.ToString("yyyyMMdd"));

            if (d1 > d2)
                return;


            string[] dirs = Directory.GetDirectories(m_alarm_log_path);
            List<string> needpath = new List<string>();
            foreach (string str in dirs)
            {
                double d3 = double.Parse(str.Split('\\')[str.Split('\\').Length - 1]);
                if (d3 >= d1 && d3 <= d2)
                {
                    needpath.Add(str);
                }
            }

            DGV.Rows.Clear();

            foreach (string str in needpath)
            {
                string[] files = Directory.GetFiles(str);
                foreach (string str1 in files)
                {
                    if (str1.IndexOf(".log.csv") > -1)
                    {

                        if (System.IO.File.Exists(str1))
                        {
                            _updatedataview(str1);
                        }

                    }

                }
            }

            //int i = 0;
            //int j = 0;
            //double icount = d2 - d1;
            //string _pathfilename = m_alarm_log_path;
            //while (i <= icount)
            //{
            //    while (j < 24)
            //    {
            //        _pathfilename = m_alarm_log_path + "\\ALARM.LOG" + d1.ToString() + "\\" + d1.ToString() + "_" + j.ToString("00") + ".log.csv";

            //        if (System.IO.File.Exists(_pathfilename))
            //        {
            //            _updatedataview(_pathfilename);
            //        }

            //        j++;
            //    }
            //    i++;
            //}
        }
        private void _updatedataview(string epathname)
        {
            if (!System.IO.File.Exists(epathname))
            {
                return;
            }

            StreamReader sr = new StreamReader(epathname);
            string read = string.Empty;
            while (!sr.EndOfStream)
            {
                read = sr.ReadLine();
                string[] readstrs = read.Split(',');

                if (readstrs.Length >= 6)
                {
                    int index = this.DGV.Rows.Add();
                    this.DGV.Rows[index].Cells[0].Value = readstrs[0];
                    this.DGV.Rows[index].Cells[1].Value = readstrs[1];
                    this.DGV.Rows[index].Cells[2].Value = readstrs[4];
                    this.DGV.Rows[index].Cells[3].Value = readstrs[5];
                }
            }
            sr.Close();
            sr.Dispose();
        }

    }
}
