using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace JetEazy.DBSpace
{
    public class EssClass
    {
        public int No { get; set; }
        public int LastRecipeNo { get; set; }
        public int PassCount { get; set; }
        public int NGCount { get; set; }
        
    }


    public class EsssDBClass
    {
        public int Index = -1;
        EssClass DataNull = new EssClass();
        public EssClass DataNow
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
        public EssClass DataLast
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

            while (i < myDataList.Count)
            {
                if (myDataList[i].No == no)
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
        List<EssClass> myDataList = new List<EssClass>();
        public EsssDBClass(DataTable datatable)
        {
            myDataTable = datatable;
            Load();
        }
        void Load()
        {
            foreach (DataRow datarow in myDataTable.Rows)
            {
                EssClass data = new EssClass();

                Mapping(datarow, data);

                myDataList.Add(data);
            }

            Index = 0;
        }
        public void Save()
        {
            int i = 0;

            foreach (EssClass data in myDataList)
            {
                DataRow datarow = myDataTable.Rows[i];
                Mapping(data, datarow, -1);

                i++;
            }
            Universal.UpdateTable(myDataTable.TableName);
        }
        public void AddCount(bool ispass)
        {
            if (ispass)
                DataNow.PassCount++;
            else
                DataNow.NGCount++;

            Universal.UpdateTable(myDataTable.TableName);
        }
        public void Mapping(EssClass fromadata, DataRow todatarow, int assignno)
        {
            if (assignno != -1)
                todatarow["No"] = assignno;
            else
                todatarow["No"] = fromadata.No;

            todatarow["LastRecipeNo"] = fromadata.LastRecipeNo;
            todatarow["PassCount"] = fromadata.PassCount;
            todatarow["NGCount"] = fromadata.NGCount;
        }
        public void Mapping(DataRow fromdatarow, EssClass todata)
        {
            todata.No = (int)fromdatarow["No"];

            todata.LastRecipeNo = (int)fromdatarow["LastRecipeNo"];
            todata.PassCount = (int)fromdatarow["PassCount"];
            todata.NGCount = (int)fromdatarow["NGCount"];
        }
        public void Reset(bool ispass)
        {
            if (ispass)
                DataNow.PassCount = 0;
            else
                DataNow.NGCount = 0;

            Save();
        }
        public void SetFAILCUT()
        {
                DataNow.NGCount--;

            Save();
        }
        public void SetPassNG(bool ispass)
        {
            if (ispass)
                DataNow.PassCount ++;
            else
                DataNow.NGCount ++;

            Save();
        }
        //public void RecipeChange(int recipeno)
        //{
        //    DataNow.LastRecipeNo = recipeno;
        //    Save();
        //}
    

    }
}
