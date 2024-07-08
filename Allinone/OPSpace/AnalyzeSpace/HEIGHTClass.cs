using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allinone.OPSpace.AnalyzeSpace
{
    public class HEIGHTClass
    {
        public HeightMethodEnum HeightMethod = HeightMethodEnum.NONE;

        public WorkStatusCollectionClass TrainStatusCollection = new WorkStatusCollectionClass();
        public WorkStatusCollectionClass RunStatusCollection = new WorkStatusCollectionClass();

        public string RelateAnalyzeString = "";
        public string RelateAnalyzeInformation = "";

        public HEIGHTClass()
        {

        }
        public HEIGHTClass(string str)
        {
            FromString(str);
        }
        public override string ToString()
        {
            string str = "";

            str += ((int)HeightMethod).ToString() + Universal.SeperateCharB;     //0

            str += "";

            return str;
        }
        public void FromString(string str)
        {
            string[] strs = str.Split(Universal.SeperateCharB);

            HeightMethod = (HeightMethodEnum)int.Parse(strs[0]);
        }
        public void Suicide()
        {
            TrainStatusCollection.Clear();
            RunStatusCollection.Clear();
        }

        public void Reset()
        {
            HeightMethod = HeightMethodEnum.NONE;
        }

        public void FromPropertyChange(string changeitemstring, string valuestring)
        {
            string[] str = changeitemstring.Split(';');

            if (str[0] != "07.Height")
                return;

            switch (str[1])
            {
                case "HeightMethod":
                    HeightMethod = (HeightMethodEnum)Enum.Parse(typeof(HeightMethodEnum), valuestring, true);
                    break;
            }
        }

        /// <summary>
        /// 在做大量運算前要清除的相關資料
        /// </summary>
        public void ResetRunStatus()
        {
            RunStatusCollection.Clear();
        }
        /// <summary>
        /// 將產生出來的過程寫出去
        /// </summary>
        /// <param name="processstringlist"></param>
        /// <param name="runstatuslist"></param>
        public void FillRunStatus(WorkStatusCollectionClass runstatuscollection)
        {
            foreach (WorkStatusClass runstatus in RunStatusCollection.WorkStatusList)
            {
                runstatuscollection.Add(runstatus);
            }
        }
    }
}
