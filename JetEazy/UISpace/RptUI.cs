using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using JetEazy.DBSpace;
using JetEazy.FormSpace;
using JetEazy.BasicSpace;

namespace JetEazy.UISpace
{
    public partial class RptUI : UserControl
    {
        enum TagEnum
        {
            STARTRECORD,
            STOPRECORD,
            EXPORTREPORT,
        }

        Label lblRecordName;

        Button btnStartRecord;
        Button btnStopRecord;
        Button btnExportReport;

        string LOGCNString = "";
        string LOGDBPathString = "";
        string LOGTXTPathString = "";
        string StartString = "";
        
        OleDbConnection LOGCN;

        bool IsRecording = false;

        public RptUI()
        {
            InitializeComponent();
            InitialInternal();
        }

        void InitialInternal()
        {
            lblRecordName = label15;

            btnStartRecord = button12;
            btnStopRecord = button16;
            btnExportReport = button13;

            btnStartRecord.Tag = TagEnum.STARTRECORD;
            btnStopRecord.Tag = TagEnum.STOPRECORD;
            btnExportReport.Tag = TagEnum.EXPORTREPORT;

            btnStartRecord.Click += new EventHandler(btn_Click);
            btnStopRecord.Click += new EventHandler(btn_Click);
            btnExportReport.Click += new EventHandler(btn_Click);
        }

        public void Initial(string logtxtpathstring,string logdbpathstring)
        {
            LOGCNString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + logdbpathstring + "\\Template.MDB";

            LOGDBPathString = logdbpathstring;
            LOGTXTPathString = logtxtpathstring;

            LOGCN = new OleDbConnection(LOGCNString);
        }

        void btn_Click(object sender, EventArgs e)
        {
            TagEnum KEYS = (TagEnum)((Button)sender).Tag;

            switch (KEYS)
            {
                case TagEnum.STARTRECORD:
                    StartRecord();
                    break;
                case TagEnum.STOPRECORD:
                    StopRecord();
                    break;
                case TagEnum.EXPORTREPORT:
                    ExportRecord();
                    break;
            }
        }

        DBForm DBFrm;

        void StartRecord()
        {
            OnTrigger(RunStatusEnum.SHOWFORM);

            DBFrm = new DBForm(ref LOGCN, LOGCNString, LOGDBPathString, LOGTXTPathString);

            DBFrm.ShowDialog();

            if (LOGCN.ConnectionString != LOGCNString)
            {
                IsRecording = true;

                lblRecordName.Text = JzToolsClass.PassingString;

                StartString = JzToolsClass.PassingString;
            }

            OnTrigger(RunStatusEnum.HIDEFORM);


        }
        public void StopRecord()
        {
            IsRecording = false;

            LOGCN.Close();
            LOGCN.Dispose();

            LOGCN = new OleDbConnection(LOGCNString);

            lblRecordName.Text = "";

        }

        ExportForm EXPORTFrm;
        void ExportRecord()
        {
            OnTrigger(RunStatusEnum.SHOWFORM);

            EXPORTFrm = new ExportForm(LOGDBPathString, LOGTXTPathString);

            EXPORTFrm.ShowDialog();
            
            OnTrigger(RunStatusEnum.HIDEFORM);
        }

        public void LogRecord(string reportstr, string savename,string rcpname,string rcpver)
        {
            if (!IsRecording)
                return;

            string[] strs = StartString.Split('-');

            string SaveName = savename + JzTimes.TimeSerialString;

            SaveData(reportstr, LOGTXTPathString + "\\" + strs[0] + "\\" + strs[1] + "\\" + SaveName + ".csv");

            OleDbCommand cmd = new OleDbCommand();
            string SQLCmd = "";

            SQLCmd = "INSERT INTO logdb (log01,log02,log03,log04) VALUES ('" +
                JzTimes.DateString.Replace("-", "/") + "','" +
                JzTimes.TimeString + "','" +
                strs[1].PadRight(20) + "," +
                (rcpname + "-" + rcpver).PadRight(20) + "," +
                savename + "','" + SaveName + "')";

            LOGCN.Open();

            cmd = new OleDbCommand(SQLCmd, LOGCN);
            cmd.ExecuteNonQuery();

            LOGCN.Close();
        }

        public delegate void TriggerHandler(RunStatusEnum runstatus);
        public event TriggerHandler TriggerAction;
        public void OnTrigger(RunStatusEnum runstatus)
        {
            if (TriggerAction != null)
            {
                TriggerAction(runstatus);
            }
        }

        void SaveData(string DataStr, string FileName)
        {
            File.WriteAllText(FileName, DataStr, Encoding.Default);
        }

    }
}
