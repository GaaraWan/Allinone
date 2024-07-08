using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JetEazy;

namespace Allinone.OPSpace
{
    [Serializable]
    public class PassInfoClass
    {
        public string PassInfoName = string.Empty;

        public int RcpNo = -1;
        public int EnvNo = -1;
        public int PageNo = -1;
        public PageOPTypeEnum PageOpType = PageOPTypeEnum.P00;

        public int ParentNo = -1;
        public int ParentLearnNo = -1;
        public int AnalyzeNo = -1;
        public int Level = -1;
        public int AnalyzeLearnNo = -1;

        public CornerEnum Corner = CornerEnum.NONE;
        public PositionEnum Position = PositionEnum.NONE;

        public string CornerNameString
        {
            get
            {
                string str = "";

                if (Corner == CornerEnum.NONE)
                    str = "";
                else
                    str = Corner.ToString();

                return str;
            }

        }

        public string OperatePath = "";

        public string OperateString = ""; //此 OperateString 為多用途，可以亂用
        //public string BiasOffset = "";
        public PassInfoClass()
        {

        }
        public PassInfoClass(string str)
        {
            FromString(str);
        }
        public PassInfoClass(PassInfoClass passinfo, OPLevelEnum oplevel)
        {
            FromPassInfo(passinfo, oplevel);
        }
        public void FromPassInfo(PassInfoClass passinfo, OPLevelEnum oplevel)
        {
            switch (oplevel)
            {
                case OPLevelEnum.COPY:
                    FromString(passinfo.ToString());
                    break;
                case OPLevelEnum.ALB:

                    break;
                case OPLevelEnum.ENV:

                    RcpNo = passinfo.RcpNo;
                    OperatePath = passinfo.OperatePath + "\\" + EnvNo.ToString(EnvClass.ORGENVNOSTRING);
                    break;
                case OPLevelEnum.PAGE:

                    RcpNo = passinfo.RcpNo;
                    EnvNo = passinfo.EnvNo;
                    OperatePath = passinfo.OperatePath;

                    break;
                case OPLevelEnum.ANALYZE:

                    RcpNo = passinfo.RcpNo;
                    EnvNo = passinfo.EnvNo;
                    PageNo = passinfo.PageNo;

                    OperatePath = passinfo.OperatePath;
                    break;
            }
        }
        public void FromString(string str)
        {
            string[] strs = str.Split(',');

            RcpNo = int.Parse(strs[0]);
            EnvNo = int.Parse(strs[1]);
            PageNo = int.Parse(strs[2]);
            PageOpType = (PageOPTypeEnum)int.Parse(strs[3]);

            ParentNo = int.Parse(strs[4]);
            ParentLearnNo = int.Parse(strs[5]);
            AnalyzeNo = int.Parse(strs[6]);
            Level = int.Parse(strs[7]);
            AnalyzeLearnNo = int.Parse(strs[8]);
            OperatePath = strs[9];

            Corner = (CornerEnum)int.Parse(strs[10]);
            Position = (PositionEnum)int.Parse(strs[11]);

            if(strs.Length > 12)
                OperateString = strs[12];
        }

        public override string ToString()
        {
            string str = "";

            str += RcpNo.ToString() + ",";              //0
            str += EnvNo.ToString() + ",";              //1

            str += PageNo.ToString() + ",";             //2
            str += ((int)PageOpType).ToString() + ",";  //3
            str += ParentNo.ToString() + ",";           //4
            str += ParentLearnNo.ToString() + ",";           //5
            str += AnalyzeNo.ToString() + ",";          //6
            str += Level.ToString() + ",";              //7

            str += AnalyzeLearnNo.ToString() + ",";         //8
            str += OperatePath + ",";                   //9
            str += ((int)Corner).ToString() + ",";      //10
            str += ((int)Position).ToString() + ",";    //11
            str += OperateString;                       //12

            return str;
        }

        public string ToInformation()
        {
            string str = "";

            str += "Recipe No. " +  RcpNo.ToString() + Environment.NewLine;              
            str += "Env No. " + EnvNo.ToString() + Environment.NewLine;              

            str += "Page No. " + PageNo.ToString() + Environment.NewLine;             
            str += "Page Otype. " + PageOpType.ToString() + Environment.NewLine;  
            str += "Parent No. " + ParentNo.ToString() + Environment.NewLine;
            str += "Parent Learn Index. " + ParentLearnNo.ToString() + Environment.NewLine;
            str += "Analyze No. " + AnalyzeNo.ToString() + Environment.NewLine;         
            str += "Level No. " + Level.ToString() + Environment.NewLine;              

            str += "Learn Index. " + AnalyzeLearnNo.ToString() + Environment.NewLine;      
            str += "Operate Path. " + OperatePath + Environment.NewLine;               
            str += "Corner Name. " + Corner.ToString() + Environment.NewLine;      
            str += "Position Name. " + Position.ToString();          

            return str;

        }
        /// <summary>
        /// No Use Anymore
        /// </summary>
        /// <returns></returns>
        public string ToAnalyzePath()
        {
            string analyzepath = "";
            
            analyzepath = OperatePath;
            analyzepath += "\\" + PageNo.ToString(PageClass.ORGPAGENOSTRING) + "-" 
                + PageOpType.ToString() + "-" 
                + AnalyzeNo.ToString(AnalyzeClass.ORGANALYZENOSTRING);
            
            return analyzepath;
        }

        public string ToDebugPath()
        {
            string debugpath = "";

            debugpath = EnvNo.ToString(EnvClass.ORGENVNOSTRING);

            return debugpath;
        }
        public string ToDebugFile()
        {
            string debugpath = "";

            debugpath = PageNo.ToString(PageClass.ORGPAGENOSTRING) + "-"
                + PageOpType.ToString() + "-"
                + AnalyzeNo.ToString(AnalyzeClass.ORGANALYZENOSTRING);

            return debugpath;
        }
        public string ToAnalyzeString()
        {
            string anz = "A" + PageNo.ToString("00") + "-" + (Level).ToString("00") + "-" + AnalyzeNo.ToString("0000");

            return anz;
        }
        public string ToPassInfoNameString()
        {
            string anz = PassInfoName;

            return anz;
        }

    }
}
