using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetNGNum
{
    public class GetNgNum
    {

        public string SourcePath { get; set; } = @"D:\Report\work\auto";
        public string TargetPath { get; set; } = @"D:\Report\work\StatisticsDate";

        // NG类型对应
        public Dictionary<int, string> ngTypeMap = new Dictionary<int, string>
        {
        {0, "正确"},
        {1, "印字错误"},
        {2, "印字偏移"},
        {3, "油墨错误"},
        {4, "印字缺失"},
        {5, "不检测"},
        {6, "其他"},
        {7, "2D对比错误"},
        {8, "2D读取错误"},
        {9, "2D重复"}
        };



        /// <summary>
        /// 统计单个日期每种NG的总数，
        /// </summary>
        /// <param name="date"></param>
        /// <returns>key counts</returns>
        public Dictionary<int, int> GetNgCountsByDate(string date)
        {
            string dateFolder = Path.Combine(SourcePath, date); //原路径  日期
            if (!Directory.Exists(dateFolder))
                return new Dictionary<int, int>();  //<NG类型，数量>

            var counts = new Dictionary<int, int>();
            foreach (var batchFolder in Directory.GetDirectories(dateFolder)) //遍历日期文件夹
            {
                foreach (var file in Directory.GetFiles(batchFolder, "*.csv")) //遍历csv文件
                {
                    var ngCounts = ParseCsvFile(file); //获取数量
                    foreach (var kv in ngCounts)
                    {
                        if (counts.ContainsKey(kv.Key)) //NG索引
                            counts[kv.Key] += kv.Value;
                        else
                            counts[kv.Key] = kv.Value;
                    }
                }
            }
            return counts;
        }


        /// <summary>
        /// 按选定的所有日期解析所有CSV文件
        /// </summary>
        /// <param name="dateList">日期</param>
        public string ParseByDates(List<string> dateList, DateTime start, DateTime end)
        {
            if (dateList == null || dateList.Count == 0)
                return null;

            dateList = dateList.OrderBy(d => d).ToList(); //日期升序

            if (!Directory.Exists(TargetPath))
                Directory.CreateDirectory(TargetPath);

            //导出文件名称
            string startDate = start.ToString("yyyyMMdd");
            string endDate = end.ToString("yyyyMMdd");
            string exportcurTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            string newFileName = Path.Combine(TargetPath, $"StatisticsDate_{startDate}-{endDate}_{exportcurTime}.csv");

            //日期 编号 文件名 ..
            var fileStats = new List<(string date, string batch, string fileName, Dictionary<int, int> ngCounts)>();
            var totalByType = ngTypeMap.Keys.ToDictionary(k => k, k => 0);//各类型NG数
            int totalNG = 0; //总NG数

            foreach (var date in dateList)//遍历日期
            {
                string dateFolder = Path.Combine(SourcePath, date);
                if (!Directory.Exists(dateFolder)) continue;

                foreach (var batchFolder in Directory.GetDirectories(dateFolder))  //遍历批号
                {
                    string batch = new DirectoryInfo(batchFolder).Name;

                    foreach (var file in Directory.GetFiles(batchFolder, "*.csv")) //遍历csv
                    {
                        var ngCounts = ParseCsvFile(file);

                        foreach (var kv in ngCounts)
                        {
                            if (!totalByType.ContainsKey(kv.Key))
                                totalByType[kv.Key] = kv.Value;
                            else
                                totalByType[kv.Key] += kv.Value;
                        }

                        totalNG += ngCounts.Values.Sum();

                        fileStats.Add((date, batch, Path.GetFileNameWithoutExtension(file), ngCounts));
                    }
                }
            }

            if (fileStats.Count == 0)
                return null;



            //写csv
            using (var writer = new StreamWriter(newFileName, false, Encoding.UTF8))
            {

                writer.WriteLine($"时间: {startDate} - {endDate}");
                writer.WriteLine($"总数: {totalNG}");
                writer.WriteLine();
                writer.WriteLine();
                writer.WriteLine();
                writer.WriteLine();
                writer.WriteLine("测试结果,数量,百分比");




                int totalNgCount = totalByType.Values.Sum();
                // 列排布NG
                var ngKeys = ngTypeMap.Keys.OrderBy(k => k).ToList();
                foreach (var k in ngTypeMap.Keys.OrderBy(k => k))
                {
                    string typeName = ngTypeMap[k];
                    int total = totalByType.ContainsKey(k) ? totalByType[k] : 0;
                    string percent = totalNgCount > 0 ? (total * 100.0 / totalNgCount).ToString("F2") + "%" : "0%";
                    writer.WriteLine($"{typeName},{total},{percent}");
                }





                writer.WriteLine();
                writer.WriteLine();
                writer.WriteLine();
                writer.WriteLine();
                writer.Write("日期,批号,片号");
                foreach (var k in ngTypeMap.Keys.OrderBy(k => k))
                    writer.Write("," + ngTypeMap[k]);
                writer.WriteLine();


                foreach (var stat in fileStats)
                {
                    writer.Write($"{stat.date},{stat.batch},{stat.fileName}");
                    foreach (var k in ngTypeMap.Keys.OrderBy(k => k)) //NG类型 升序
                    {
                        int v = stat.ngCounts != null && stat.ngCounts.ContainsKey(k) ? stat.ngCounts[k] : 0;
                        writer.Write("," + v);
                    }
                    writer.WriteLine();
                }
            }

            return newFileName;
        }


        /// <summary>
        /// 统计NG数量
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private Dictionary<int, int> ParseCsvFile(string filePath)
        {
            var counts = new Dictionary<int, int>();

            using (var reader = new StreamReader(filePath, System.Text.Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] columns = line.Split(new char[] { ',', '\t' });

                    if (columns.Length < 6)  //小于6行
                        continue;

                    string col5 = columns[5].Trim();   //从1数的第6行

                    if (int.TryParse(col5, out int ng))
                    {
                        if (counts.ContainsKey(ng))
                            counts[ng]++;
                        else
                            counts[ng] = 1;
                    }
                }
            }
            return counts;
        }



    }
}
