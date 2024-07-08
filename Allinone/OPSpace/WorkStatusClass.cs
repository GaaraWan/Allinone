using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

using JetEazy;
using Allinone;
using MoveGraphLibrary;
using WorldOfMoveableObjects;

namespace Allinone.OPSpace
{
    public class WorkStatusCollectionClass
    {
        const char SeperateCharC = '\x02';
        const char SeperateCharD = '\x03';
        const char SeperateCharE = '\x04';
        const char SeperateCharF = '\x05';
        public List<WorkStatusClass> WorkStatusList = new List<WorkStatusClass>();

        public int COUNT = 0;
        public int NGCOUNT = 0;
        public int PASSCOUNT = 0;

        List<int> NGIndexList = new List<int>();

        public string AllProcessString = "";
        public string AllErrorString = "";

        public void FromString(string Str)
        {
            string[] vs = Str.Split(SeperateCharF);
            COUNT = int.Parse(vs[0]);
            NGCOUNT = int.Parse(vs[1]);
            PASSCOUNT = int.Parse(vs[2]);
            AllProcessString = vs[3];
            AllErrorString = vs[4];

            NGIndexList.Clear();
            string[] vs2 = vs[5].Split(SeperateCharE);
            foreach (string s in vs2)
            {
                if (!string.IsNullOrEmpty(s))
                    NGIndexList.Add(int.Parse(s));
            }

            WorkStatusList.Clear();
            string[] vs1 = vs[6].Split(SeperateCharD);
            foreach (string s in vs1)
            {
                if (!string.IsNullOrEmpty(s))
                    WorkStatusList.Add(new WorkStatusClass(s));
            }
        }
        public override string ToString()
        {
            string Str = string.Empty;

            Str += COUNT.ToString() + SeperateCharF;
            Str += NGCOUNT.ToString() + SeperateCharF;
            Str += PASSCOUNT.ToString() + SeperateCharF;
            Str += AllProcessString.ToString() + SeperateCharF;
            Str += AllErrorString.ToString() + SeperateCharF;

            string StrNGList = string.Empty;
            foreach (int ix in NGIndexList)
            {
                StrNGList += ix.ToString() + SeperateCharE;
            }
            StrNGList = RemoveLastChar(StrNGList, 1);
            Str += StrNGList.ToString() + SeperateCharF;

            string StrWork = string.Empty;
            foreach (WorkStatusClass workStatus in WorkStatusList)
            {
                StrWork += workStatus.ToString() + SeperateCharD;
            }
            StrWork = RemoveLastChar(StrWork, 1);
            Str += StrWork.ToString();

            return Str;
        }
        public WorkStatusClass GetRunStatus(int index)
        {
            return WorkStatusList[index];
        }

        public void Add(WorkStatusClass workstatus)
        {
            if (workstatus.Reason == ReasonEnum.NG)
            {
                NGIndexList.Add(COUNT);
                NGCOUNT++;

                workstatus.NGIndex = NGCOUNT;
            }
            else
            {
                PASSCOUNT++;

                workstatus.PassIndex = PASSCOUNT;
            }

            COUNT++;

            workstatus.Index = COUNT;

            WorkStatusList.Add(workstatus);

            AllProcessString += workstatus.ProcessString + Environment.NewLine;
            AllErrorString += workstatus.ErrorString + Environment.NewLine;

        }
        public void Clear()
        {
            COUNT = 0;
            NGCOUNT = 0;
            PASSCOUNT = 0;

            AllProcessString = "";
            AllErrorString = "";

            lock (WorkStatusList)
            {
                foreach (WorkStatusClass works in WorkStatusList)
                {
                    works.Suicide();
                }

                WorkStatusList.Clear();
            }
            NGIndexList.Clear();

        }

        public WorkStatusClass GetNGRunStatus(int index)
        {
            return WorkStatusList[NGIndexList[index]];
        }

        public void SaveProcessAndError(string pathstr)
        {
            if (!Directory.Exists(pathstr))
                Directory.CreateDirectory(pathstr);

            SaveData(AllProcessString, pathstr + "\\Process.log");
            SaveData(AllErrorString, pathstr + "\\Error.log");
        }
        public void SaveProcessAndError(string pathstr,string addstr)
        {
            if (!Directory.Exists(pathstr))
                Directory.CreateDirectory(pathstr);

            SaveData(AllProcessString, pathstr + "\\Process " + addstr + " .log");
            SaveData(AllErrorString, pathstr + "\\Error " + addstr + " .log");
        }

        void SaveData(string DataStr, string FileName)
        {
            File.WriteAllText(FileName, DataStr, Encoding.Default);
        }
        string RemoveLastChar(string Str, int Count)
        {
            if (Str.Length < Count)
                return "";

            return Str.Remove(Str.Length - Count, Count);
        }
    }

    public class WorkStatusClass
    {
        const char SeperateCharG = '\x06';

        public int NGIndex = 0;
        public int PassIndex = 0;
        public int Index = 0;
        
        public string LogString = "";   //標記這筆資料是否已被紀錄過了

        public string NGIndexStr
        {
            get
            {
                return "NO. " + NGIndex.ToString();
            }
        }

        bool ISRESERVEPASSIMAGE
        {
            get
            {
                return Universal.ISRESERVEPASSIMAGE;
            }
        }

        public Bitmap bmpORG = new Bitmap(1, 1);
        public Bitmap bmpRUN = new Bitmap(1, 1);
        public Bitmap bmpDIFF = new Bitmap(1, 1);

        public string ErrorString = "";
        public string ProcessString = "";
        public string Desc = "";

        //public string RelateAnalyzeInformation = "";
        public PassInfoClass PassInfo = new PassInfoClass();

        public AnanlyzeProcedureEnum AnalyzeProcedure = AnanlyzeProcedureEnum.ALIGNTRAIN;
        public ReasonEnum Reason = ReasonEnum.PASS;

        public void FromString(string Str)
        {
            string[] vs = Str.Split(SeperateCharG);
            NGIndex = int.Parse(vs[0]);
            PassIndex = int.Parse(vs[1]);
            Index = int.Parse(vs[2]);
            LogString = vs[3];
            ErrorString = vs[4];
            ProcessString = vs[5];
            Desc = vs[6];
            PassInfo = new PassInfoClass(vs[7]);
            AnalyzeProcedure = (AnanlyzeProcedureEnum)int.Parse(vs[8]);
            Reason = (ReasonEnum)int.Parse(vs[9]);
        }
        public override string ToString()
        {
            string Str = string.Empty;

            Str += NGIndex.ToString() + SeperateCharG;
            Str += PassIndex.ToString() + SeperateCharG;
            Str += Index.ToString() + SeperateCharG;
            Str += LogString.ToString() + SeperateCharG;
            Str += ErrorString.ToString() + SeperateCharG;
            Str += ProcessString.ToString() + SeperateCharG;
            Str += Desc.ToString() + SeperateCharG;
            Str += PassInfo.ToString() + SeperateCharG;
            Str += ((int)AnalyzeProcedure).ToString() + SeperateCharG;
            Str += ((int)Reason).ToString();

            return Str;
        }

        public WorkStatusClass(string Str)
        {
            FromString(Str);
        }
        public WorkStatusClass(AnanlyzeProcedureEnum analyzeprocedure)
        {
            AnalyzeProcedure = analyzeprocedure;
        }

        public void SetWorkStatus(Bitmap bmporg, Bitmap bmprun, Bitmap bmpdiff
            , ReasonEnum myreason, string errorstr, string processsstr
            , PassInfoClass passinfo, string descstr = "")
        {
            bmpORG.Dispose();
            bmpRUN.Dispose();
            bmpDIFF.Dispose();

            if (myreason == ReasonEnum.NG || ISRESERVEPASSIMAGE)
            {
                bmpORG = new Bitmap(bmporg);
                bmpRUN = new Bitmap(bmprun);
                bmpDIFF = new Bitmap(bmpdiff);
            }
            else
            {
                bmpORG = new Bitmap(1, 1);
                bmpRUN = new Bitmap(1, 1);
                bmpDIFF = new Bitmap(1, 1);
            }


            //RelateAnalyzeInformation = relateanalyzeinformation;
            PassInfo = new PassInfoClass(passinfo, OPLevelEnum.COPY);

            ErrorString = errorstr;
            ProcessString = processsstr;
            Desc = descstr;
            Reason = myreason;
        }
        
        public void Suicide()
        {
            bmpORG.Dispose();
            bmpRUN.Dispose();
            bmpDIFF.Dispose();
        }
        public WorkStatusClass Clone()
        {
            WorkStatusClass workStatusClass = new WorkStatusClass(this.AnalyzeProcedure);

            workStatusClass.SetWorkStatus(bmpORG, bmpRUN, bmpDIFF, Reason,
                ErrorString, ProcessString, PassInfo, Desc);

            return workStatusClass;
        }
    }
}
