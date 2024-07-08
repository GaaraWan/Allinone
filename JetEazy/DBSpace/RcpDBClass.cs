using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using JetEazy.BasicSpace;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace JetEazy.DBSpace
{
    [Serializable]
    public class RcpClass
    {
        public RcpClass()
        {
            No = -1;
            Name = "N/A";
            Version = "N/A";
            Remark = "N/A";

            StartDatetime = "N/A";
            ModifyDatetime = "N/A";
        }
        
        public RcpClass(RcpClass rcp)
        {
            Copy(rcp);
        }

        public void Copy(RcpClass fromrcp)
        {
            No = fromrcp.No;
            Name = fromrcp.Name;
            Version = fromrcp.Version;
            Remark = fromrcp.Remark;

            StartDatetime = fromrcp.StartDatetime;
            ModifyDatetime = fromrcp.ModifyDatetime;
        }

        /// <summary>
        /// 作為新增參數的預設參數，當找不到參數時為回到第一個，第0個為新增參數時用，所以第一個和第0個都很重要
        /// </summary>
        public static string ORGRCPNOSTRING = "00000";      
        public string RcpNoString
        {
            get
            {
                return No.ToString(ORGRCPNOSTRING);
            }
        }

        public int No { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string Remark { get; set; }
        public string StartDatetime { get; set; }
        public string ModifyDatetime { get; set; }

        public string ToModifyString()
        {
            return "Start Time: " + StartDatetime + " Modify Time: " + ModifyDatetime;
        }
        public string ToShortModifyString()
        {
            return "S:" + StartDatetime.Substring(0,10) + Environment.NewLine + "M:" + ModifyDatetime.Substring(0, 10);
        }
        public string ToESSString()
        {
            return "[" + No.ToString() + "] " + Name + "(" + Version + ")";
        }

        public bool CheckFilter(string FilterStr)
        {
            string Str = (Name + "(" + Version + ")").ToUpper();

            return (Str.IndexOf(FilterStr) > -1);
        }
    }

    [Serializable]
    public class RcpDBClass
    {
        public int Index = -1;
        RcpClass DataNull = new RcpClass();
        //Inside Variable
        DataTable myDataTable;
        public List<RcpClass> myDataList = new List<RcpClass>();
        public RcpClass DataNow
        {
            get
            {
                if (Index == -1)
                    return DataNull;
                else
                {
                    return myDataList[Index];
                }
            }
        }
        public RcpClass DataLast
        {
            get
            {
                return myDataList[myDataList.Count - 1];
            }
        }
        public int DataLastNoExcludeStatic
        {
            get
            {
                List<int> datanolist = new List<int>();

                int i = 0;

                while (i < myDataList.Count)
                {
                    if (myDataList[i].No < Universal.StaticStartNo)
                    {
                        datanolist.Add(myDataList[i].No);
                    }
                    i++;
                }

                datanolist.Sort();
                datanolist.Reverse();

                return datanolist[0];
            }
        }
        public RcpDBClass(DataTable datatable)
        {
            myDataTable = datatable;
            Load();
        }
        public RcpDBClass Clone()
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, this);
                ms.Position = 0;

                return (RcpDBClass)formatter.Deserialize(ms);
            }
        }
        public void GotoIndex(int index)
        {
            Index = index;
        }
        public int FindIndex(int no)
        {
            int ret = -1;
            int i = 0;

            while (i < myDataList.Count)
            {
                if (myDataList[i].No == no)
                {
                    ret = i;
                    break;
                }
                i++;
            }
            if (ret == -1)
                ret = 0;

            return ret;
        }
        public RcpClass GetRCP(int index)
        {
            return myDataList[index];
        }
        public int FindVersion(string version, string Name = "")
        {
            int ret = -1;
            int i = 0;

            while (i < myDataList.Count)
            {
                if (Name == "")
                {
                    if (myDataList[i].Version == version)
                    {
                        ret = myDataList[i].No;
                        break;
                    }
                }
                else
                {
                    if (myDataList[i].Version == version && Name== myDataList[i].Name)
                    {
                        ret = myDataList[i].No;
                        break;
                    }
                }
                i++;
            }
            return ret;
        }
        public int FindName(string Name)
        {
            int ret = -1;
            int i = 0;

            while (i < myDataList.Count)
            {
                if (myDataList[i].Name == Name)
                {
                    ret = myDataList[i].No;
                    break;
                }
                i++;
            }
            return ret;
        }
        public void Load()
        {
            myDataList.Clear();

            foreach (DataRow datarow in myDataTable.Rows)
            {
                RcpClass data = new RcpClass();

                Mapping(datarow, data);

                myDataList.Add(data);
            }

            Index = 0;
        }
        public void Save()
        {
            int i = 0;

            foreach (RcpClass data in myDataList)
            {
                DataRow datarow = myDataTable.Rows[i];
                Mapping(data, datarow, -1);

                i++;
            }
            Universal.UpdateTable(myDataTable.TableName);
        }
        public void Add()
        {
            //int LastIndex = DataLast.Index + 1;
            //DataRow newdatarow = myDataTable.NewRow();

            //Mapping(DataNow, newdatarow, LastIndex);

            //newdatarow["Name"] = ((string)newdatarow["Name"])[0] + JzTimes.DateTimeSerialString;
            //newdatarow["StartDatetime"] = JzTimes.DateTimeString;
            //newdatarow["ModifyDatetime"] = JzTimes.DateTimeString;

            //myDataTable.Rows.Add(newdatarow);

            //RcpClass newrcp = new RcpClass();
            //Mapping(newdatarow, newrcp);

            //myDataList.Add(newrcp);

            //Indicator = myDataList.Count - 1;
            Add(FindIndex(DataNow.No));
        }
        public void Add(int index)
        {
            //int LastNo = DataLast.No + 1;
            //Revise in 18/05/05 for Static Recipe
            int LastNo = DataLastNoExcludeStatic + 1;

            DataRow newdatarow = myDataTable.NewRow();

            Mapping(myDataList[index], newdatarow, LastNo);

            newdatarow["Name"] = ((string)newdatarow["Name"])[0] + JzTimes.DateTimeSerialString;
            newdatarow["StartDatetime"] = JzTimes.DateTimeString;
            newdatarow["ModifyDatetime"] = JzTimes.DateTimeString;

            myDataTable.Rows.Add(newdatarow);

            RcpClass newrcp = new RcpClass();
            Mapping(newdatarow, newrcp);

            myDataList.Add(newrcp);

            Index = myDataList.Count - 1;
        }
        public void Mapping(RcpClass fromadata, DataRow todatarow, int assignno)
        {
            if (assignno != -1)
                todatarow["No"] = assignno;
            else
                todatarow["No"] = fromadata.No;

            todatarow["Name"] = fromadata.Name;
            todatarow["Version"] = fromadata.Version;
            todatarow["Remark"] = fromadata.Remark;
            todatarow["StartDatetime"] = fromadata.StartDatetime;
            todatarow["ModifyDatetime"] = fromadata.ModifyDatetime;
        }
        public void Mapping(DataRow fromdatarow, RcpClass todata)
        {
            todata.No = (int)fromdatarow["No"];

            todata.Name = (string)fromdatarow["Name"];
            todata.Version = (string)fromdatarow["Version"];
            try
            {
                todata.Remark = (string)fromdatarow["Remark"];
            }
            catch
            {
                todata.Remark = "Remark";
            }
            todata.StartDatetime = (string)fromdatarow["StartDatetime"];
            todata.ModifyDatetime = (string)fromdatarow["ModifyDatetime"];
        }
        public void Delete(int index)
        {
            string deletestring = "DELETE FROM " + myDataTable.TableName + " WHERE [No] = " + myDataList[index].No;

            myDataList.RemoveAt(index);
            myDataTable.Rows.RemoveAt(index);

            Universal.DeleteTableRow(myDataTable.TableName, deletestring);
        }
        public void DeleteLast(int originno)
        {
            Delete(myDataList.Count - 1);
            GotoIndex(FindIndex(originno));
        }
        public bool CheckIsDuplicate(string namestr, int checkno,string Version)
        {
            bool ret = false;

            foreach (RcpClass data in myDataList)
            {
                if (data.No == checkno)
                    continue;

                if (data.Name.Trim().ToUpper() == namestr.Trim().ToUpper() && data.Version.Trim().ToUpper() == Version.Trim().ToUpper())
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }
        public List<string> GetRecipeStringList()
        {
            List<string> lst = new List<string>();

            //foreach (RcpClass rcp in myDataList)
            //{
            //    if (rcp.Index > 0)
            //        lst.Add(rcpitem.ToESSString() + "?" + rcpitem.IndexStr);
            //}
            return lst;
        }
    }
}
