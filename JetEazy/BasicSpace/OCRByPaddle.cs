using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PaddleOCRSharp;

namespace OCRByPaddle
{
    /// <summary>
    /// 使用百度飞桨进行OCR
    /// </summary>
    public class OCRByPaddle
    {
        /// <summary>
        /// OCR引擎  
        /// </summary>
        private PaddleOCREngine ocrEngine = null;
        /// <summary>
        /// 文本框四个顶点列表(外部访问)
        /// </summary>
        public List<Point[]> pointList = null;
        /// <summary>
        /// 识别分数
        /// </summary>
        public List<float> scoreList = null;
        public OCRByPaddle()
        {
            // 初始化OCR引擎
            ocrEngine = new PaddleOCREngine();
            pointList = new List<Point[]>();
            scoreList=new   List<float>();
        }
        /// <summary>
        /// 传入要识别的图像,可以获得解码分数,以及ROI
        /// </summary>
        /// <param name="bmp">要解码的图像</param>
        /// <returns>解码内容</returns>
        public string OCR(Bitmap bmp)
        {
            OCRStructureResult result = ocrEngine?.DetectStructure(bmp);

            List<List<OCRPoint>> list = new List<List<OCRPoint>>();

            StringBuilder sb = new StringBuilder();

            scoreList.Clear();
            foreach (var line in result.TextBlocks)
            {
                //当前解码区域四个顶点
                list.Add(line.BoxPoints);
                //当前解码分数
                scoreList.Add(line.Score);
                sb.Append($"{line.Text}{Environment.NewLine}");
            }
            PointToPoint(list);
            return sb.ToString();
        }
        /// <summary>
        /// 点对点转换(OCRPoint转Point)
        /// </summary>
        /// <param name="list"></param>
        private void PointToPoint(List<List<OCRPoint>> list)
        {
            pointList.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                OCRPoint[] points = list[i].ToArray();
                Point[] p = new Point[points.Length];
                for (int j = 0; j < p.Length; j++)
                {
                    p[j] = new Point(points[j].X, points[j].Y);
                }
                pointList.Add(p);
            }
        }

        ~OCRByPaddle()
        {
            ocrEngine?.Dispose();
        }
    }
}
