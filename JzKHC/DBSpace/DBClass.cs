using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

using JetEazy.BasicSpace;

namespace JzKHC.DBSpace
{
    public enum DBStatusEnum
    {
        ADD,
        DELETE,
        MODIFY,
        COPY,
        NONE,
        CLIENT,
    }
    /// <summary>
    /// This Class is For ADO Dataset Use From Victor In 2007/11/26
    /// </summary>
    abstract class DBClass
    {
        //protected FrameworkClass Framework;
        //protected MessageClass MSG;

        public int RecordIndex = 0;
        public int LastIndex = 0;

        protected string TableString = "";
        protected DataRow OPRow;

        protected string IndexField = "";
        protected string NameField = "";
        protected string DBPresentName = "";

        public int RecordCount
        {
            get
            {
                return tbl.Rows.Count;
            }
        }

        protected OleDbConnection Datacn;
        protected OleDbDataAdapter da;
        protected OleDbCommandBuilder cb;
        protected OleDbCommand cmd;

        public List<string> ListName = new List<string>();

        public DataTable tbl = new DataTable();

        protected DBStatusEnum mStatus = DBStatusEnum.NONE;

        public DBClass(string rTableString,string rIndexString,string rNameField,string rDBPresentName,OleDbConnection rDatacn)
        {

        }
        public virtual void Initial()
        {
            string DbStr = "SELECT * FROM " + TableString + " ORDER BY " + IndexField;

            Datacn.Open();
            da = new OleDbDataAdapter(DbStr, Datacn);
            cb = new OleDbCommandBuilder(da);
            tbl.Clear();
            da.Fill(tbl);
            Datacn.Close();

            GetNameList();
        }
        public int Add()
        {
            //MSG.Occur(1, DBPresentName + ",ADD");
            Copy(0);
            OPRow[NameField] = "新資料" + DBPresentName + " " + ((int)OPRow[IndexField] + 1).ToString();

            return (int)OPRow[IndexField];
        }

        public void DeleteID(int rID)
        {
            Delete(GetIndexFromID(rID));
        }
        public void DeleteIDDirect(int rID)
        {
            DeleteDirect(GetIndexFromID(rID));
        }

        public void Delete(int Index)
        {
            if (MessageBox.Show("是否要刪除此筆資料?", "MAIN", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                OPRow = tbl.Rows[Index];

                string SQLCmd = "DELETE FROM " + TableString + " WHERE " + IndexField + " = " + OPRow[IndexField];

                Datacn.Open();
                cmd = new OleDbCommand(SQLCmd, Datacn);
                cmd.ExecuteNonQuery();
                Datacn.Close();

                OPRow.Delete();
                OPRow.AcceptChanges();

                if (Index.Equals(0))
                    RecordIndex = 0;
                else
                    RecordIndex = Index - 1;

                GetNameList();

                //MSG.Occur(2, DBPresentName + "," + DBStatus.DELETE.ToString());
            }
        }
        public void DeleteDirect(int Index)
        {
            OPRow = tbl.Rows[Index];

            string SQLCmd = "DELETE FROM " + TableString + " WHERE " + IndexField + " = " + OPRow[IndexField];

            Datacn.Open();
            cmd = new OleDbCommand(SQLCmd, Datacn);
            cmd.ExecuteNonQuery();
            Datacn.Close();

            OPRow.Delete();
            OPRow.AcceptChanges();
        }

        public void Modify()
        {
            //MSG.Occur(1, DBPresentName + ",MODIFY");
            OPRow = tbl.Rows[RecordIndex];

            LastIndex = RecordIndex;
        }
        int GetIndexFromID(int rID)
        {
            int i = 0;
            while (i < RecordCount)
            {
                if (tbl.Rows[i][IndexField].ToString() == rID.ToString())
                    break;

                i++;
            }
            return i; 
        }
        public void CopyID(int rID)
        {
            Copy(GetIndexFromID(rID));
        }
        public int Copy(int Index)
        {
            int i = 0;
            int MaxIndex = GetMaxIndex() + 1;

            //if (!Index.Equals(0))
            //    MSG.Occur(1, DBPresentName + ",COPY");

            OPRow = tbl.Rows.Add();

            while (i < tbl.Columns.Count)
            {
                OPRow[i] = tbl.Rows[Index][i];
                i++;
            }

            OPRow[IndexField] = MaxIndex;
            OPRow[NameField] = "(C)" + DBPresentName + OPRow[NameField].ToString();

            LastIndex = RecordIndex;
            RecordIndex = RecordCount - 1;

            return (int)OPRow[IndexField];
        }
        public void Goto(int ID)
        {
            int i = 0, FoundIndex = 0;
            while (i < RecordCount)
            {
                if (((int)tbl.Rows[i][IndexField]).Equals(ID))
                {
                    FoundIndex = i;
                    break;
                }
                i++;
            }
                
            RecordIndex = FoundIndex;
        }
        public void GotoIndex(int Index)
        {
            RecordIndex = Index;
        }
        public void Rollback(DBStatusEnum Status)
        {
            int i = 0;
            string SQLCmd = "";
            string SQLSubCmd = "";

            OPRow = tbl.Rows[RecordIndex];

            switch (Status)
            {
                case DBStatusEnum.ADD:
                case DBStatusEnum.COPY:
                    SQLCmd = "INSERT INTO " + TableString + "(";
                    SQLSubCmd = ") VALUES (";
                    break;
                case DBStatusEnum.MODIFY:
                    SQLCmd = "UPDATE " + TableString + " SET ";
                    OPRow = tbl.Rows[RecordIndex];
                    break;
            }

            while (i < tbl.Columns.Count)
            {
                switch (Status)
                {
                    case DBStatusEnum.ADD:
                    case DBStatusEnum.COPY:
                        SQLCmd += tbl.Columns[i].ColumnName + ",";
                        if (tbl.Columns[i].DataType.IsValueType)
                            SQLSubCmd += OPRow[i].ToString() + ",";
                        else
                            SQLSubCmd += "'" + OPRow[i].ToString() + "',";
                        break;
                    case DBStatusEnum.MODIFY:
                        SQLCmd += tbl.Columns[i].ColumnName + " = ";

                        if (tbl.Columns[i].DataType.IsValueType)
                            SQLCmd += OPRow[i].ToString() + ",";
                        else
                            SQLCmd += "'" + OPRow[i].ToString() + "',";
                        break;
                }
                i++;
            }   

            switch (Status)
            {
                case DBStatusEnum.ADD:
                case DBStatusEnum.COPY:
                    SQLCmd = SQLCmd.Remove(SQLCmd.Length - 1, 1) + SQLSubCmd.Remove(SQLSubCmd.Length - 1, 1) + ")";
                    break;
                case DBStatusEnum.MODIFY:
                    SQLCmd = SQLCmd.Remove(SQLCmd.Length - 1, 1) + " WHERE " + IndexField + "=" + OPRow[IndexField];
                    break;
            }

            //MSG.Occur(2, DBPresentName + "," + Status.ToString());
            OPRow.AcceptChanges();

            Datacn.Open();
            cmd = new OleDbCommand(SQLCmd, Datacn);
            cmd.ExecuteNonQuery();
            Datacn.Close();

            GetNameList();
        }
        public void Cancel(DBStatusEnum Status)
        {
            //MSG.Occur(3, DBPresentName + "," + Status.ToString());

            OPRow.RejectChanges();
            if (Status.Equals(DBStatusEnum.ADD) || Status.Equals(DBStatusEnum.COPY))
                OPRow.Delete();

            RecordIndex = LastIndex;
        }

        protected bool IsIDExist(int ID)
        {
            bool ret = false;
            int i = 0;
            while (i < RecordCount)
            {
                if (((int)tbl.Rows[i][IndexField]).Equals(ID))
                {
                    ret = true;
                    break;
                }
                i++;
            }
            return ret;
        }

        protected int GetMaxIndex()
        {
            return (int)tbl.Rows[RecordCount - 1][IndexField];
        }
        public bool IsContainSameName(string Name,int myID)
        {
            bool ret = false;

            int i = 0;
            while (i < RecordCount)
            {
                if (myID != (int)tbl.Rows[i][IndexField])
                {
                    if (Name.Equals((string)tbl.Rows[i][NameField]))
                    {
                        ret = true;
                        break;
                    }
                }
                i++;
            }

            return ret;
        }

        protected void GetNameList()
        {
            ListName.Clear();

            int i = 0;
            while (i < RecordCount)
            {
                ListName.Add(tbl.Rows[i][NameField].ToString() + "," + tbl.Rows[i][IndexField].ToString());
                i++;
            }

            ListName.Sort();
        }
        public int GetNameListIndex()
        {
            int i = 0;
            while (i < ListName.Count)
            {
                if (ListName[i].Split(';')[1] == RecordIndex.ToString())
                {
                    break;
                }
                i++;
            }
            return i;
        }
    }

}
