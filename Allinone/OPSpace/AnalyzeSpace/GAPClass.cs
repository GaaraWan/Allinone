using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allinone.OPSpace.AnalyzeSpace
{
    public class GAPClass
    {
        public GapMethodEnum GapMethod = GapMethodEnum.NONE;

        public WorkStatusCollectionClass TrainStatusCollection = new WorkStatusCollectionClass();
        public WorkStatusCollectionClass RunStatusCollection = new WorkStatusCollectionClass();

        public string RelateAnalyzeString = "";
        public string RelateAnalyzeInformation = "";



        /// <summary>
        /// 偏移角度
        /// </summary>
        public virtual float OffsetAngle
        {
            get; set;
        }

        /// <summary>
        /// 最大距离
        /// </summary>
        public virtual float OffsetToUPMax
        {
            get; set;
        }

        /// <summary>
        /// 最小距离
        /// </summary>
        public float OffsetToUPMin
        {
            get; set;
        }

        /// <summary>
        /// 左右差异
        /// </summary>
        public float OffsetToLeftRight
        {
            get; set;
        }


        /// <summary>
        /// A位置最大值
        /// </summary>
        public float A_Max
        {
            get; set;
        }
        /// <summary>
        /// A位置最小值
        /// </summary>
        public float A_Min
        {
            get; set;
        }
        /// <summary>
        /// B位置最大值
        /// </summary>
        public float B_Max
        {
            get; set;
        }
        /// <summary>
        /// B位置最小值
        /// </summary>
        public float B_Min
        {
            get; set;
        }
        /// <summary>
        /// C位置最大值
        /// </summary>
        public float C_Max
        {
            get; set;
        }
        /// <summary>
        /// C位置最小值
        /// </summary>
        public float C_Min
        {
            get; set;
        }
        /// <summary>
        /// D位置最大值
        /// </summary>
        public float D_Max
        {
            get; set;
        }
        /// <summary>
        /// D位置最小值
        /// </summary>
        public float D_Min
        {
            get; set;
        }
        /// <summary>
        /// E位置最大值
        /// </summary>
        public float E_Max
        {
            get; set;
        }
        /// <summary>
        /// E位置最小值
        /// </summary>
        public float E_Min
        {
            get; set;
        }
        /// <summary>
        /// F位置最大值
        /// </summary>
        public float F_Max
        {
            get; set;
        }
        /// <summary>
        /// F位置最小值
        /// </summary>
        public float F_Min
        {
            get; set;
        }
        /// <summary>
        /// G位置最大值
        /// </summary>
        public float G_Max
        {
            get; set;
        }
        /// <summary>
        /// G位置最小值
        /// </summary>
        public float G_Min
        {
            get; set;
        }
        /// <summary>
        /// H位置最大值
        /// </summary>
        public float H_Max
        {
            get; set;
        }
        /// <summary>
        /// H位置最小值
        /// </summary>
        public float H_Min
        {
            get; set;
        }
        public GAPClass()
        {

        }
        public GAPClass(string str)
        {
            FromString(str);
        }
        public override string ToString()
        {
            string str = "";

            str += ((int)GapMethod).ToString() + Universal.SeperateCharB;     //0
            str += OffsetAngle .ToString() + Universal.SeperateCharB;
            str += OffsetToLeftRight.ToString() + Universal.SeperateCharB;
            str += OffsetToUPMax.ToString() + Universal.SeperateCharB;
            str += OffsetToUPMin.ToString() + Universal.SeperateCharB;
            str += A_Max.ToString() + Universal.SeperateCharB;
            str += A_Min.ToString() + Universal.SeperateCharB;
            str += B_Max.ToString() + Universal.SeperateCharB;
            str += B_Min.ToString() + Universal.SeperateCharB;
            str += C_Max.ToString() + Universal.SeperateCharB;
            str += C_Min.ToString() + Universal.SeperateCharB;
            str += D_Max.ToString() + Universal.SeperateCharB;
            str += D_Min.ToString() + Universal.SeperateCharB;
            str += E_Max.ToString() + Universal.SeperateCharB;
            str += E_Min.ToString() + Universal.SeperateCharB;
            str += F_Max.ToString() + Universal.SeperateCharB;
            str += F_Min.ToString() + Universal.SeperateCharB;
            str += G_Max.ToString() + Universal.SeperateCharB;
            str += G_Min.ToString() + Universal.SeperateCharB;
            str += H_Max.ToString() + Universal.SeperateCharB;
            str += H_Min.ToString() + Universal.SeperateCharB;

            str += "";

            return str;
        }
        public void FromString(string str)
        {
            string[] strs = str.Split(Universal.SeperateCharB);

            GapMethod = (GapMethodEnum)int.Parse(strs[0]);
            if (strs.Length > 4)
            {
                OffsetAngle = float.Parse(strs[1]);
                OffsetToLeftRight = float.Parse(strs[2]);
                OffsetToUPMax = float.Parse(strs[3]);
                OffsetToUPMin = float.Parse(strs[4]);
            }
            if (strs.Length > 19)
            {
                A_Max = float.Parse(strs[5]);
                A_Min = float.Parse(strs[6]);
                B_Max = float.Parse(strs[7]);
                B_Min = float.Parse(strs[8]);
                C_Max = float.Parse(strs[9]);
                C_Min = float.Parse(strs[10]);
                D_Max = float.Parse(strs[11]);
                D_Min = float.Parse(strs[12]);
                E_Max = float.Parse(strs[13]);
                E_Min = float.Parse(strs[14]);
                F_Max = float.Parse(strs[15]);
                F_Min = float.Parse(strs[16]);
                G_Max = float.Parse(strs[17]);
                G_Min = float.Parse(strs[18]);
                H_Max = float.Parse(strs[19]);
                H_Min = float.Parse(strs[20]);
            }
        }
        public void Reset()
        {
            GapMethod = GapMethodEnum.NONE;

            OffsetAngle = 0;
            OffsetToLeftRight = 0;
            OffsetToUPMax = 0;
            OffsetToUPMin = 0;
            A_Max = 0;
            A_Min = 0;
            B_Max = 0;
            B_Min = 0;
            C_Max = 0;
            C_Min = 0;
            D_Max = 0;
            D_Min = 0;
            E_Max = 0;
            E_Min = 0;
            F_Max = 0;
            F_Min = 0;
            G_Max = 0;
            G_Min = 0;
            H_Max = 0;
            H_Min = 0;
        }
        public void FromPropertyChange(string changeitemstring, string valuestring)
        {
            string[] str = changeitemstring.Split(';');

            if (str[0] != "08.Gap")
                return;

            switch (str[1])
            {
                case "GapMethod":
                    GapMethod = (GapMethodEnum)Enum.Parse(typeof(GapMethodEnum), valuestring, true);
                    break;
                case "OffsetAngle":
                    OffsetAngle=float.Parse( valuestring);
                    break;
                case "OffsetToLeftRight":
                    OffsetToLeftRight = float.Parse(valuestring);
                    break;
                case "OffsetToUPMax":
                    OffsetToUPMax = float.Parse(valuestring);
                    break;
                case "OffsetToUPMin":
                    OffsetToUPMin = float.Parse(valuestring);
                    break;
                case "A_Max":
                    A_Max = float.Parse(valuestring);
                    break;
                case "A_Min":
                    A_Min = float.Parse(valuestring);
                    break;
                case "B_Max":
                    B_Max = float.Parse(valuestring);
                    break;
                case "B_Min":
                    B_Min = float.Parse(valuestring);
                    break;
                case "C_Max":
                    C_Max = float.Parse(valuestring);
                    break;
                case "C_Min":
                    C_Min = float.Parse(valuestring);
                    break;
                case "D_Max":
                    D_Max = float.Parse(valuestring);
                    break;
                case "D_Min":
                    D_Min = float.Parse(valuestring);
                    break;
                case "E_Max":
                    E_Max = float.Parse(valuestring);
                    break;
                case "E_Min":
                    E_Min = float.Parse(valuestring);
                    break;
                case "F_Max":
                    F_Max = float.Parse(valuestring);
                    break;
                case "F_Min":
                    F_Min = float.Parse(valuestring);
                    break;
                case "G_Max":
                    G_Max = float.Parse(valuestring);
                    break;
                case "G_Min":
                    G_Min = float.Parse(valuestring);
                    break;
                case "H_Max":
                    H_Max = float.Parse(valuestring);
                    break;
                case "H_Min":
                    H_Min = float.Parse(valuestring);
                    break;
            }
        }
        public void Suicide()
        {
            TrainStatusCollection.Clear();
            RunStatusCollection.Clear();
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
