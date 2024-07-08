using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allinone.OPSpace.AnalyzeSpace
{
    public class StiltsClass
    {
        public STILTSMethodEnum StiltsMethod = STILTSMethodEnum.NONE;

        public WorkStatusCollectionClass TrainStatusCollection = new WorkStatusCollectionClass();
        public WorkStatusCollectionClass RunStatusCollection = new WorkStatusCollectionClass();

        public string RelateAnalyzeString = "";
        public string RelateAnalyzeInformation = "";



        /// <summary>
        /// 允许的高跷度
        /// </summary>
        public virtual int StiltsOffSet
        {
            get; set;
        }

        /// <summary>
        /// 阴影区灰度
        /// </summary>
        public virtual int StiltsGrayValue
        {
            get; set;
        }

        /// <summary>
        /// 非阴影区最小灰度
        /// </summary>
        public virtual int StiltsNOGrayValue
        {
            get; set;
        }

        public int StiltsLength = 0;


        public StiltsClass()
        {

        }
        public StiltsClass(string str)
        {
            FromString(str);
        }
        public override string ToString()
        {
            string str = "";

            str += ((int)StiltsMethod).ToString() + Universal.SeperateCharB;     //0
            str += StiltsOffSet.ToString() + Universal.SeperateCharB;
            str +=StiltsGrayValue .ToString() + Universal.SeperateCharB;
            str += StiltsNOGrayValue .ToString() + Universal.SeperateCharB;
            str += "";

            return str;
        }
        public void FromString(string str)
        {
            string[] strs = str.Split(Universal.SeperateCharB);

        
            if(strs.Length>1)
            {
                StiltsMethod = (STILTSMethodEnum)int.Parse(strs[0]);
                StiltsOffSet = int.Parse(strs[1]);
                if (strs.Length > 3)
                {
                  StiltsGrayValue   = int.Parse(strs[2]);
                    StiltsNOGrayValue = int.Parse(strs[3]);
                }
            }
        }
        public void Reset()
        {
            StiltsMethod = STILTSMethodEnum.NONE;

            StiltsOffSet = 0;
            StiltsGrayValue = 0;
            StiltsNOGrayValue = 0;
        }
        public void FromPropertyChange(string changeitemstring, string valuestring)
        {
            string[] str = changeitemstring.Split(';');

            if (str[0] != "09.Stilts")
                return;

            switch (str[1])
            {
                case "STILTSMethod":
                    StiltsMethod = (STILTSMethodEnum)Enum.Parse(typeof(STILTSMethodEnum), valuestring, true);
                    break;
                case "StiltsOffSet":
                    StiltsOffSet = int.Parse( valuestring);
                    break;
                case "StiltsGrayValue":
                    StiltsGrayValue = int.Parse(valuestring);
                    break;
                case "StiltsNOGrayValue":
                    StiltsNOGrayValue = int.Parse(valuestring);
                    break;

            }
        }


        public Bitmap FindBlob(bool istrain, Bitmap bmppattern,Bitmap bmpAligned, Bitmap bmpFindBlob, PassInfoClass passInfo, out bool isgood)
        {
            if (StiltsMethod != STILTSMethodEnum.STILTS)
            {
                isgood = true;
                return null;
            }

            WorkStatusClass workstatus = new WorkStatusClass(JetEazy.AnanlyzeProcedureEnum.STILTS);
            string processstring = "Start  STILTSCHRCK." + Environment.NewLine;
            string errorstring = "";
            JetEazy.ReasonEnum reason = JetEazy.ReasonEnum.PASS;

         Bitmap   bmpBlob = new Bitmap(bmpFindBlob);
            //if (INI.ISSAVEOCRIMAGE)
            //    bmpFind.Save(Universal.OCRIMAGEPATH + barcode + "_" + OCRMappingMethod + Universal.GlobalImageTypeString, Universal.GlobalImageFormat);
            //  Graphics g = Graphics.FromImage(bmpBlob);

            Point center = new Point(bmpBlob.Width / 2, bmpBlob.Height / 2);
            List<FindLineResult> MyResults = new List<FindLineResult>();

            //Stopwatch watch = new Stopwatch();
            //watch.Start();
            //第一区
            for (int ia = 1; ia <= 45;)
            {
                double radians = (Math.PI / 180) * ia;
                FindLineResult findLine = new FindLineResult();
                List<FindLineResult> results = new List<FindLineResult>();

                int lx = 0;
                int ly = 0;
                bool isok = false;
                for (int x = 1; x < (bmpBlob.Width / 2); x++)
                {
                double da=    Math.Tan(radians);
                    int y = (int)(da * x);

                    lx = x + center.X;
                    ly =  center.Y-y;

                    if (lx >= bmpBlob.Width)
                        lx = bmpBlob.Width - 1;
                    if (ly >= bmpBlob.Height)
                        ly = bmpBlob.Height - 1;

                    if (lx < 0)
                        lx = 0;
                    if (ly < 0)
                        ly = 0;


                    Color color = bmpBlob.GetPixel(lx, ly);
               
                    if (color .R>200)
                    {
                        //bmpBlob.SetPixel(lx, ly, Color.Blue);

                        isok = false;
                     
                        if (findLine.isStart)
                        {
                            findLine.isStart = false;
                            findLine.PointB = new Point(lx, ly);
                            results.Add(findLine);
                        }
                    }
                    else
                    {
                        //bmpBlob.SetPixel(lx, ly, Color.Red);

                        if (!isok)
                        {
                            isok = true;
                            findLine = new FindLineResult();
                            findLine.isStart = true;
                            findLine.PointA = new Point(lx, ly);
                            findLine.iAngle = ia;
                            findLine.Section = FindLineResult.SectionEnum.A;
                        }
                    }


                    if (isok)
                    {
                        findLine.iLength++;
                    }

                   
                }
                if (findLine.isStart)
                {
                    findLine.PointB = new Point(lx, ly);
                    results.Add(findLine);
                }

                int TempIndex = 0;
                int TempLength = 0;
                for(int i=0;i<results.Count;i++)
                {
                    if(results[i].iLength>=TempLength)
                    {
                        TempIndex = i;
                        TempLength = results[i].iLength;
                    }
                }
                if (results.Count > 0)
                    MyResults.Add(results[TempIndex]);

                results.Clear();
                ia += 2;
            }
            for (int ia = 1; ia <= 45;)
            {
                double radians = (Math.PI / 180) * ia;
                FindLineResult findLine = new FindLineResult();
                List<FindLineResult> results = new List<FindLineResult>();

                int lx = 0;
                int ly = 0;
                bool isok = false;
                for (int y = 1; y < (bmpBlob.Height / 2); y++)
                {
                    double da = Math.Tan(radians);
                    int x = (int)(da * y);

                    lx = x + center.X;
                    ly = center.Y - y;

                    if (lx >= bmpBlob.Width)
                        lx = bmpBlob.Width - 1;
                    if (ly >= bmpBlob.Height)
                        ly = bmpBlob.Height - 1;

                    if (lx < 0)
                        lx = 0;
                    if (ly < 0)
                        ly = 0;


                    Color color = bmpBlob.GetPixel(lx, ly);

                    if (color.R > 200)
                    {
                        //bmpBlob.SetPixel(lx, ly, Color.Blue);

                        isok = false;

                        if (findLine.isStart)
                        {
                            findLine.isStart = false;
                            findLine.PointB = new Point(lx, ly);
                            results.Add(findLine);
                        }
                    }
                    else
                    {
                        //bmpBlob.SetPixel(lx, ly, Color.Red);

                        if (!isok)
                        {
                            isok = true;
                            findLine = new FindLineResult();
                            findLine.isStart = true;
                            findLine.PointA = new Point(lx, ly);
                            findLine.iAngle =90 - ia;
                            findLine.Section = FindLineResult.SectionEnum.A;
                        }
                    }


                    if (isok)
                    {
                        findLine.iLength++;
                    }


                }
                if (findLine.isStart)
                {
                    findLine.PointB = new Point(lx, ly);
                    results.Add(findLine);
                }

                int TempIndex = 0;
                int TempLength = 0;
                for (int i = 0; i < results.Count; i++)
                {
                    if (results[i].iLength >= TempLength)
                    {
                        TempIndex = i;
                        TempLength = results[i].iLength;
                    }
                }
                if (results.Count > 0)
                    MyResults.Add(results[TempIndex]);

                results.Clear();
                ia += 2;
            }
            //第二区
            for (int ia = 1; ia <= 45;)
            {
                double radians = (Math.PI / 180) * ia;
                FindLineResult findLine = new FindLineResult();
                List<FindLineResult> results = new List<FindLineResult>();

                int lx = 0;
                int ly = 0;
                bool isok = false;
                for (int y = 1; y < (bmpBlob.Height / 2); y++)
                {
                    double da = Math.Tan(radians);
                    int x = (int)(da * y);

                    lx = center.X - x;
                    ly = center.Y - y;

                    if (lx < 0)
                        lx = 0;
                    if (ly < 0)
                        ly = 0;

                    if (lx >= bmpBlob.Width)
                        lx = bmpBlob.Width - 1;
                    if (ly >= bmpBlob.Height)
                        ly = bmpBlob.Height - 1;


                    Color color = bmpBlob.GetPixel(lx, ly);

                    if (color.R > 200)
                    {
                        //bmpBlob.SetPixel(lx, ly, Color.Blue);

                        isok = false;

                        if (findLine.isStart)
                        {
                            findLine.isStart = false;
                            findLine.PointB = new Point(lx, ly);
                            results.Add(findLine);
                        }
                    }
                    else
                    {
                        //bmpBlob.SetPixel(lx, ly, Color.Red);

                        if (!isok)
                        {
                            isok = true;
                            findLine = new FindLineResult();
                            findLine.isStart = true;
                            findLine.PointA = new Point(lx, ly);
                            findLine.iAngle = ia+90;
                            findLine.Section = FindLineResult.SectionEnum.B;
                        }
                    }


                    if (isok)
                    {
                        findLine.iLength++;
                    }


                }
                if (findLine.isStart)
                {
                    findLine.PointB = new Point(lx, ly);
                    results.Add(findLine);
                }

                int TempIndex = 0;
                int TempLength = 0;
                for (int i = 0; i < results.Count; i++)
                {
                    if (results[i].iLength >= TempLength)
                    {
                        TempIndex = i;
                        TempLength = results[i].iLength;
                    }
                }
                if (results.Count > 0)
                    MyResults.Add(results[TempIndex]);

                results.Clear();
                ia += 2;
            }
            for (int ia = 1; ia <= 45;)
            {
                double radians = (Math.PI / 180) * ia;
                FindLineResult findLine = new FindLineResult();
                List<FindLineResult> results = new List<FindLineResult>();

                int lx = 0;
                int ly = 0;
                bool isok = false;
                for (int x = 1; x < (bmpBlob.Width / 2); x++)
                {
                    double da = Math.Tan(radians);
                    int y = (int)(da * x);

                    lx =  center.X-x;
                    ly = center.Y - y;

                    if (lx >= bmpBlob.Width)
                        lx = bmpBlob.Width - 1;
                    if (ly >= bmpBlob.Height)
                        ly = bmpBlob.Height - 1;
                    if (lx < 0)
                        lx = 0;
                    if (ly < 0)
                        ly = 0;

                    Color color = bmpBlob.GetPixel(lx, ly);

                    if (color.R > 200)
                    {
                        //bmpBlob.SetPixel(lx, ly, Color.Blue);

                        isok = false;

                        if (findLine.isStart)
                        {
                            findLine.isStart = false;
                            findLine.PointB = new Point(lx, ly);
                            results.Add(findLine);
                        }
                    }
                    else
                    {
                        //bmpBlob.SetPixel(lx, ly, Color.Red);

                        if (!isok)
                        {
                            isok = true;
                            findLine = new FindLineResult();
                            findLine.isStart = true;
                            findLine.PointA = new Point(lx, ly);
                            findLine.iAngle =180- ia;
                            findLine.Section = FindLineResult.SectionEnum.B;
                        }
                    }


                    if (isok)
                    {
                        findLine.iLength++;
                    }


                }
                if (findLine.isStart)
                {
                    findLine.PointB = new Point(lx, ly);
                    results.Add(findLine);
                }

                int TempIndex = 0;
                int TempLength = 0;
                for (int i = 0; i < results.Count; i++)
                {
                    if (results[i].iLength >= TempLength)
                    {
                        TempIndex = i;
                        TempLength = results[i].iLength;
                    }
                }
                if (results.Count > 0)
                    MyResults.Add(results[TempIndex]);

                results.Clear();
                ia += 2;
            }

            //第三区
            for (int ia = 1; ia <= 45;)
            {
                double radians = (Math.PI / 180) * ia;
                FindLineResult findLine = new FindLineResult();
                List<FindLineResult> results = new List<FindLineResult>();

                int lx = 0;
                int ly = 0;
                bool isok = false;
                for (int x = 1; x < (bmpBlob.Width / 2); x++)
                {
                    double da = Math.Tan(radians);
                    int y = (int)(da * x);

                    lx = center.X - x;
                    ly = center.Y + y;
                    if (lx < 0)
                        lx = 0;
                    if (ly < 0)
                        ly = 0;

                    if (lx >= bmpBlob.Width)
                        lx = bmpBlob.Width - 1;
                    if (ly >= bmpBlob.Height)
                        ly = bmpBlob.Height - 1;


                    Color color = bmpBlob.GetPixel(lx, ly);

                    if (color.R > 200)
                    {
                        //bmpBlob.SetPixel(lx, ly, Color.Blue);

                        isok = false;

                        if (findLine.isStart)
                        {
                            findLine.isStart = false;
                            findLine.PointB = new Point(lx, ly);
                            results.Add(findLine);
                        }
                    }
                    else
                    {
                        //bmpBlob.SetPixel(lx, ly, Color.Red);

                        if (!isok)
                        {
                            isok = true;
                            findLine = new FindLineResult();
                            findLine.isStart = true;
                            findLine.PointA = new Point(lx, ly);
                            findLine.iAngle = 180 + ia;
                            findLine.Section = FindLineResult.SectionEnum.C;
                        }
                    }


                    if (isok)
                    {
                        findLine.iLength++;
                    }


                }
                if (findLine.isStart)
                {
                    findLine.PointB = new Point(lx, ly);
                    results.Add(findLine);
                }

                int TempIndex = 0;
                int TempLength = 0;
                for (int i = 0; i < results.Count; i++)
                {
                    if (results[i].iLength >= TempLength)
                    {
                        TempIndex = i;
                        TempLength = results[i].iLength;
                    }
                }
                if (results.Count > 0)
                    MyResults.Add(results[TempIndex]);

                results.Clear();
                ia += 2;
            }
            for (int ia = 1; ia <= 45;)
            {
                double radians = (Math.PI / 180) * ia;
                FindLineResult findLine = new FindLineResult();
                List<FindLineResult> results = new List<FindLineResult>();

                int lx = 0;
                int ly = 0;
                bool isok = false;
                for (int y = 1; y < (bmpBlob.Height / 2); y++)
                {
                    double da = Math.Tan(radians);
                    int x = (int)(da * y);

                    lx = center.X - x;
                    ly = center.Y + y;
                    if (lx < 0)
                        lx = 0;
                    if (ly < 0)
                        ly = 0;

                    if (lx >= bmpBlob.Width)
                        lx = bmpBlob.Width - 1;
                    if (ly >= bmpBlob.Height)
                        ly = bmpBlob.Height - 1;


                    Color color = bmpBlob.GetPixel(lx, ly);

                    if (color.R > 200)
                    {
                        //bmpBlob.SetPixel(lx, ly, Color.Blue);

                        isok = false;

                        if (findLine.isStart)
                        {
                            findLine.isStart = false;
                            findLine.PointB = new Point(lx, ly);
                            results.Add(findLine);
                        }
                    }
                    else
                    {
                        //bmpBlob.SetPixel(lx, ly, Color.Red);

                        if (!isok)
                        {
                            isok = true;
                            findLine = new FindLineResult();
                            findLine.isStart = true;
                            findLine.PointA = new Point(lx, ly);
                            findLine.iAngle =270- ia ;
                            findLine.Section = FindLineResult.SectionEnum.C;
                        }
                    }


                    if (isok)
                    {
                        findLine.iLength++;
                    }


                }
                if (findLine.isStart)
                {
                    findLine.PointB = new Point(lx, ly);
                    results.Add(findLine);
                }

                int TempIndex = 0;
                int TempLength = 0;
                for (int i = 0; i < results.Count; i++)
                {
                    if (results[i].iLength >= TempLength)
                    {
                        TempIndex = i;
                        TempLength = results[i].iLength;
                    }
                }
                if (results.Count > 0)
                    MyResults.Add(results[TempIndex]);

                results.Clear();
                ia += 2;
            }

            //第四 区
            for (int ia = 1; ia <= 45;)
            {
                double radians = (Math.PI / 180) * ia;
                FindLineResult findLine = new FindLineResult();
                List<FindLineResult> results = new List<FindLineResult>();

                int lx = 0;
                int ly = 0;
                bool isok = false;
                for (int y = 1; y < (bmpBlob.Height / 2); y++)
                {
                    double da = Math.Tan(radians);
                    int x = (int)(da * y);

                    lx = center.X + x;
                    ly = center.Y + y;
                    if (lx < 0)
                        lx = 0;
                    if (ly < 0)
                        ly = 0;

                    if (lx >= bmpBlob.Width)
                        lx = bmpBlob.Width - 1;
                    if (ly >= bmpBlob.Height)
                        ly = bmpBlob.Height - 1;


                    Color color = bmpBlob.GetPixel(lx, ly);

                    if (color.R > 200)
                    {
                        //bmpBlob.SetPixel(lx, ly, Color.Blue);

                        isok = false;

                        if (findLine.isStart)
                        {
                            findLine.isStart = false;
                            findLine.PointB = new Point(lx, ly);
                            results.Add(findLine);
                        }
                    }
                    else
                    {
                        //bmpBlob.SetPixel(lx, ly, Color.Red);

                        if (!isok)
                        {
                            isok = true;
                            findLine = new FindLineResult();
                            findLine.isStart = true;
                            findLine.PointA = new Point(lx, ly);
                            findLine.iAngle = 270 + ia;
                            findLine.Section = FindLineResult.SectionEnum.D;
                        }
                    }


                    if (isok)
                    {
                        findLine.iLength++;
                    }


                }
                if (findLine.isStart)
                {
                    findLine.PointB = new Point(lx, ly);
                    results.Add(findLine);
                }

                int TempIndex = 0;
                int TempLength = 0;
                for (int i = 0; i < results.Count; i++)
                {
                    if (results[i].iLength >= TempLength)
                    {
                        TempIndex = i;
                        TempLength = results[i].iLength;
                    }
                }
                if (results.Count > 0)
                    MyResults.Add(results[TempIndex]);

                results.Clear();
                ia += 2;
            }
            for (int ia = 1; ia <= 45;)
            {
                double radians = (Math.PI / 180) * ia;
                FindLineResult findLine = new FindLineResult();
                List<FindLineResult> results = new List<FindLineResult>();

                int lx = 0;
                int ly = 0;
                bool isok = false;
                for (int x = 1; x < (bmpBlob.Width / 2); x++)
                {
                    double da = Math.Tan(radians);
                    int y = (int)(da * x);

                    lx = center.X +  x;
                    ly = center.Y + y;
                    if (lx < 0)
                        lx = 0;
                    if (ly < 0)
                        ly = 0;

                    if (lx >= bmpBlob.Width)
                        lx = bmpBlob.Width - 1;
                    if (ly >= bmpBlob.Height)
                        ly = bmpBlob.Height - 1;


                    Color color = bmpBlob.GetPixel(lx, ly);

                    if (color.R > 200)
                    {
                        //bmpBlob.SetPixel(lx, ly, Color.Blue);

                        isok = false;

                        if (findLine.isStart)
                        {
                            findLine.isStart = false;
                            findLine.PointB = new Point(lx, ly);
                            results.Add(findLine);
                        }
                    }
                    else
                    {
                        //bmpBlob.SetPixel(lx, ly, Color.Red);

                        if (!isok)
                        {
                            isok = true;
                            findLine = new FindLineResult();
                            findLine.isStart = true;
                            findLine.PointA = new Point(lx, ly);
                            findLine.iAngle = 360 - ia;
                            findLine.Section = FindLineResult.SectionEnum.C;
                        }
                    }


                    if (isok)
                    {
                        findLine.iLength++;
                    }


                }
                if (findLine.isStart)
                {
                    findLine.PointB = new Point(lx, ly);
                    results.Add(findLine);
                }

                int TempIndex = 0;
                int TempLength = 0;
                for (int i = 0; i < results.Count; i++)
                {
                    if (results[i].iLength >= TempLength)
                    {
                        TempIndex = i;
                        TempLength = results[i].iLength;
                    }
                }
                if (results.Count > 0)
                    MyResults.Add(results[TempIndex]);

                results.Clear();
                ia += 2;
            }

            //watch.Stop();
            //long time = watch.ElapsedMilliseconds;

            int Index = 0;
            StiltsLength = 0;
            for (int i = 0; i < MyResults.Count; i++)
            {
                if (MyResults[i].iLength >= StiltsLength)
                {
                    Index = i;
                    StiltsLength = MyResults[i].iLength;
                }
            }

            Bitmap bmpErr = new Bitmap(bmpAligned);
            Graphics g = Graphics.FromImage(bmpErr);
            if (StiltsLength > StiltsOffSet)
            {
                reason = JetEazy.ReasonEnum.NG;

               

                g.DrawLine(new Pen(Color.Red, 3), MyResults[Index].PointA, MyResults[Index].PointB);
                g.DrawString(StiltsLength.ToString(), new Font("宋体", 80, FontStyle.Bold), new SolidBrush(Color.Red), MyResults[Index].PointA);
               

                errorstring += "螺丝高跷错误,标准: " + StiltsOffSet + " 实际: " + StiltsLength;
                workstatus.SetWorkStatus(new Bitmap(bmppattern), bmpBlob, bmpErr, reason, errorstring, processstring, passInfo);
            }
            else
                workstatus.SetWorkStatus(new Bitmap(bmppattern), bmpBlob, bmpBlob, reason, errorstring, processstring, passInfo);

            g.Dispose();
            MyResults.Clear();

            //bmpBlob.Save("D:\\blob.png");
            

            if (reason == JetEazy.ReasonEnum.NG)
                isgood = false;
            else
                isgood = true;


            RunStatusCollection.Add(workstatus);

            return bmpErr;
        }
        class FindLineResult
        {
            public bool isStart = false;

            public enum SectionEnum
            {
                A,
                B,
                C,
                D,
            }
            /// <summary>
            /// 角度
            /// </summary>
            public int iAngle { get; set; }
            /// <summary>
            /// 长度
            /// </summary>
            public int iLength { get; set; }

            /// <summary>
            /// 位置 区间
            /// </summary>
            public SectionEnum Section { get; set; }

            /// <summary>
            /// 启始点
            /// </summary>
            public Point PointA { get; set; }
            /// <summary>
            /// 结束点
            /// </summary>
            public Point PointB { get; set; }
        }

        public void Suicide()
        {
            if (StiltsMethod == STILTSMethodEnum.STILTS)
            {
                TrainStatusCollection.Clear();
                RunStatusCollection.Clear();
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
