using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;

namespace JetEazy.DBSpace
{
    public class AccClass
    {
        public int No{get;set;}
        public string Name { get; set; }
        public string Password { get; set; }
        public bool AllowSetup { get; set; }
        public bool AllowManageAccount { get; set; }
        public bool AllowSetupRecipe { get; set; }
        public bool AllowUseShopFloor { get; set; }

        public bool IsSuperUser
        {
            get
            {
                return No == 0;
            }
        }
        public bool IsOperating(int index)
        {   
            return No == index;
        }
    }
    public class AccDBClass
    {
        public int Index = -1;
        AccClass DataNull = new AccClass();
        public AccClass DataNow
        {
            get
            {
                if (Index < 0)
                    return DataNull;
                else
                {
                    return myDataList[Index];
                }
            }
        }
        public AccClass DataLast
        {
            get
            {
                return myDataList[myDataList.Count - 1];
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
            
            while(i < myDataList.Count)
            {
                if(myDataList[i].No == no)
                {
                    ret = i;
                    break;
                }
                i++;
            }

            return ret;
        }
        //Inside Variable
        DataTable myDataTable;
        public List<AccClass> myDataList = new List<AccClass>();
        public AccDBClass(DataTable datatable)
        {
            myDataTable = datatable;
            Load();
        }
        public void Load()
        {
            foreach(DataRow datarow in myDataTable.Rows)
            {
                AccClass data = new AccClass();

                Mapping(datarow, data);

                myDataList.Add(data);
            }

            Index = 0;
        }
        public void Save()
        {
            int i = 0;

            foreach(AccClass data in myDataList)
            {
                DataRow datarow = myDataTable.Rows[i];
                Mapping(data, datarow, -1);

                i++;
            }

            Universal.UpdateTable(myDataTable.TableName);
        }
        public void Add()
        {
            int LastIndex = DataLast.No + 1;
            DataRow newdatarow = myDataTable.NewRow();

            Mapping(DataNow, newdatarow, LastIndex);
            
            myDataTable.Rows.Add(newdatarow);

            AccClass newacc = new AccClass();
            Mapping(newdatarow, newacc);

            myDataList.Add(newacc);

            Index = myDataList.Count - 1;
        }
        public void Add(int indicator)
        {
            int LastIndex = DataLast.No + 1;
            DataRow newdatarow = myDataTable.NewRow();

            Mapping(myDataList[indicator], newdatarow, LastIndex);

            myDataTable.Rows.Add(newdatarow);

            AccClass newacc = new AccClass();
            Mapping(newdatarow, newacc);

            myDataList.Add(newacc);

            //Indicator = myDataList.Count - 1;
        }
        public void Mapping(AccClass fromadata,DataRow todatarow, int assignindex)
        {
            if(assignindex != -1)
                todatarow["No"] = assignindex;
            else
                todatarow["No"] = fromadata.No;

            todatarow["Name"] = fromadata.Name;
            todatarow["Password"] = fromadata.Password;
            todatarow["AllowSetup"] = fromadata.AllowSetup;
            todatarow["AllowManageAccount"] = fromadata.AllowManageAccount;
            todatarow["AllowSetupRecipe"] = fromadata.AllowSetupRecipe;
            todatarow["AllowUseShopFloor"] = fromadata.AllowUseShopFloor;
        }
        public void Mapping(DataRow fromdatarow, AccClass todata)
        {
            todata.No = (int)fromdatarow["No"];

            todata.Name = (string)fromdatarow["Name"];
            todata.Password = (string)fromdatarow["Password"];
            todata.AllowSetup = (bool)fromdatarow["AllowSetup"];
            todata.AllowManageAccount = (bool)fromdatarow["AllowManageAccount"];
            todata.AllowSetupRecipe = (bool)fromdatarow["AllowSetupRecipe"];
            todata.AllowUseShopFloor = (bool)fromdatarow["AllowUseShopFloor"];
        }
        public void Delete(int indicator)
        {
            string deletestring = "DELETE FROM " + myDataTable.TableName + " WHERE [No] = " + myDataList[indicator].No;
            
            myDataList.RemoveAt(indicator);
            myDataTable.Rows.RemoveAt(indicator);

            Universal.DeleteTableRow(myDataTable.TableName, deletestring);
        }
        public void DeleteLast()
        {
            Delete(myDataList.Count - 1);
        }
        public bool CheckIsDuplicate(string namestr,int checkindex)
        {
            bool ret = false;

            foreach(AccClass data in myDataList)
            {
                if (data.No == checkindex)
                    continue;

                if(data.Name.Trim().ToUpper() == namestr.Trim().ToUpper())
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }
        public bool CheckIsCertified(string name, string password,bool isupdateindicator)
        {
            int i = 0;
            bool IsPass = false;
            foreach (AccClass data in myDataList)
            {
                if (data.Name.Trim().ToUpper() == name.Trim().ToUpper() &&
                    data.Password.Trim().ToUpper() == password.Trim().ToUpper())
                {
                    IsPass = true;

                    if (isupdateindicator)
                    {
                        Index = i;
                    }
                    break;
                }
                i++;
            }
            return IsPass;
        }

        public bool CheckIsCertifiedTo( bool isupdateindicator,string name="admin")
        {
            int i = 0;
            bool IsPass = false;
            foreach (AccClass data in myDataList)
            {
                if (data.Name.Trim().ToUpper() == name.Trim().ToUpper())
                {
                    IsPass = true;

                    if (isupdateindicator)
                    {
                        Index = i;
                    }
                    break;
                }
                i++;
            }
            return IsPass;
        }

        int BackIndicator = 0;
        public void BackupIndicator()
        {
            BackIndicator = Index;
            Index = 0;
        }
        public void RestoreIndicator()
        {
            Index = BackIndicator;
        }
    }
}
