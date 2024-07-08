using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AJZReportViewer
{
    public class AnalyzeClass
    {

        public string Name = "";
        public string FullPath = "";

        public List<AnalyzeClass> BranchList = new List<AnalyzeClass>();

        public AnalyzeClass(string eFullPath)
        {
            FullPath = eFullPath;
            string[] strs = eFullPath.Split('\\');
            Name = strs[strs.Length - 1];
        }
    }
}
