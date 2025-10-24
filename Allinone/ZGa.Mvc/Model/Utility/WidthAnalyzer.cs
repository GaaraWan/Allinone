using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allinone.ZGa.Mvc.Model.Utility
{
    public class WidthAnalyzer
    {
        public static List<DataPoint> FindOutliersWithZScore(List<double> values, double zThreshold = 2.0)
        {
            var results = new List<DataPoint>();

            if (values == null || values.Count < 3)
                return results;

            double mean = values.Average();
            double stdDev = CalculateStandardDeviation(values);

            for (int i = 0; i < values.Count; i++)
            {
                double zScore = Math.Abs((values[i] - mean) / stdDev);
                bool isOutlier = zScore > zThreshold;

                results.Add(new DataPoint
                {
                    Index = i,
                    Value = values[i],
                    IsOutlier = isOutlier,
                    ZScore = zScore
                });
            }

            return results;
        }
        public static List<DataPoint> FindOutliersWithMovingAverage(
    List<double> values,
    int windowSize = 5,
    double sigmaThreshold = 2.0)
        {
            var results = new List<DataPoint>();

            for (int i = 0; i < values.Count; i++)
            {
                // 获取窗口内的数据
                var window = GetWindowValues(values, i, windowSize);

                if (window.Count < 3)
                {
                    results.Add(new DataPoint
                    {
                        Index = i,
                        Value = values[i],
                        IsOutlier = false
                    });
                    continue;
                }

                double windowMean = window.Average();
                double windowStdDev = CalculateStandardDeviation(window);

                // 如果标准差为0，说明窗口内数值相同
                if (windowStdDev == 0)
                {
                    results.Add(new DataPoint
                    {
                        Index = i,
                        Value = values[i],
                        IsOutlier = false
                    });
                    continue;
                }

                double zScore = Math.Abs((values[i] - windowMean) / windowStdDev);
                bool isOutlier = zScore > sigmaThreshold;

                results.Add(new DataPoint
                {
                    Index = i,
                    Value = values[i],
                    IsOutlier = isOutlier,
                    ZScore = zScore
                });
            }

            return results;
        }

        private static List<double> GetWindowValues(List<double> values, int centerIndex, int windowSize)
        {
            int halfWindow = windowSize / 2;
            int start = Math.Max(0, centerIndex - halfWindow);
            int end = Math.Min(values.Count - 1, centerIndex + halfWindow);

            var window = new List<double>();
            for (int i = start; i <= end; i++)
            {
                if (i != centerIndex) // 排除当前点
                    window.Add(values[i]);
            }

            return window;
        }
        public static List<DataPoint> FindOutliersWithPercentile(
    List<double> values,
    double percentileThreshold = 0.95)
        {
            var results = new List<DataPoint>();

            if (values == null || values.Count == 0)
                return results;

            // 计算阈值
            var sortedValues = values.OrderBy(x => x).ToList();
            int thresholdIndex = (int)(sortedValues.Count * percentileThreshold);
            double thresholdValue = sortedValues[thresholdIndex];

            for (int i = 0; i < values.Count; i++)
            {
                bool isOutlier = values[i] > thresholdValue;

                results.Add(new DataPoint
                {
                    Index = i,
                    Value = values[i],
                    IsOutlier = isOutlier
                });
            }

            return results;
        }

        public static void VisualizeOutliers(List<double> values)
        {
            var outliers = FindOutliersWithZScore(values, 2.0);

            Console.WriteLine("数据可视化:");
            foreach (var point in outliers)
            {
                string bar = new string('█', (int)(point.Value * 2)); // 简单的条形图
                string marker = point.IsOutlier ? " ← 异常!" : "";
                Console.WriteLine($"{point.Index:D2}: {bar} {point.Value:F1}{marker}");
            }
        }

        private static double CalculateStandardDeviation(List<double> values)
        {
            double mean = values.Average();
            double sumOfSquares = values.Sum(value => Math.Pow(value - mean, 2));
            return Math.Sqrt(sumOfSquares / (values.Count - 1));
        }
    }

    public class DataPoint
    {
        public int Index { get; set; }
        public double Value { get; set; }
        public bool IsOutlier { get; set; }
        public double ZScore { get; set; }
    }
}
