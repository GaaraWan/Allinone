using JetEazy.BasicSpace;
using JetEazy.UISpace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Allinone.OPSpace.AnalyzeSpace
{
    /// <summary>
    /// 图像校正
    /// </summary>
    [Serializable]
    public class CorrestClass
    {
        public string SaveFilePath { get; set; }
        public List<CorrectToolRun> CorrestList = new List<CorrectToolRun>();
        public List<CorrectTool> ToolList = new List<CorrectTool>();

        public void Initial(string path)
        {
            SaveFilePath = path+ "\\ViewWorld.vv";
           
            Load();
        }
        /// <summary>
        /// 新增参数
        /// </summary>
        /// <param name="count"></param>
        public void ADD(int count)
        {
            for (int i = 0; i < count; i++)
            {
                CorrectTool tool = new CorrectTool();
                tool.Par = new CorrectPar();
                tool.bmp = new Bitmap(1, 1);
                tool.Rect = new Rectangle(1, 1, 10, 10);
                tool.pointsForView = new PointF[3, 3];
                tool.pointsForWord = new PointF[3, 3];
                ToolList.Add(tool);
            }
        }
        /// <summary>
        /// 更新内存数据
        /// </summary>
        public void ToUpdata()
        {
            CorrestList.Clear();
            foreach (CorrectTool tool in ToolList)
            {
                CorrectToolRun toolrun = new CorrectToolRun();
                toolrun.SetcAoi(tool.pointsForView, tool.pointsForWord);
                CorrestList.Add(toolrun);
            }
        }
        /// <summary>
        /// 载入参数
        /// </summary>
        public void Load()
        {
            if (!File.Exists(SaveFilePath))
            {
                ADD(1);
                Save();
                Load();
                return;
            }

            ArrayList myList = new ArrayList();
            Read(out myList, SaveFilePath );

            if (myList != null && myList.Count < 1)
                ADD(1);
            else
                ToolList = myList[0] as List<CorrectTool>;

            ToUpdata();

        }
        /// <summary>
        /// 保存参数
        /// </summary>
        public void Save()
        {
            ToUpdata();

            ArrayList array = new ArrayList();
            array.Add(ToolList);
            Write(array, SaveFilePath);

           
        }


        /// <summary>
        /// 读出参数
        /// </summary>
        /// <param name="myArray">out 传入的集合</param>
        /// <param name="st_File">读哪个文件</param>
        /// <returns></returns>
        public bool Read(out ArrayList myArray, string st_File)
        {
            try
            {
                System.Runtime.Serialization.IFormatter formater = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                Stream stream = new FileStream(st_File, FileMode.Open);
                myArray = (ArrayList)formater.Deserialize(stream);

                stream.Close();
                stream.Dispose();

                GC.Collect();//强制进行拉圾回收
                return true;
            }
            catch (Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
                MessageBox.Show(ex.ToString());
                myArray = null;
                GC.Collect();//强制进行拉圾回收
                return false;
            }

        }
        /// <summary>
        /// 记录参数
        /// </summary>
        /// <param name="mylist">需记录的集合</param>
        /// <param name="st_File">存放的路径</param>
        /// <returns></returns>
        public bool Write(ArrayList mylist, string st_File)
        {
            try
            {
                FileStream fs = new FileStream(st_File, FileMode.Create);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, mylist);
                fs.Close();

                fs.Dispose();
                GC.Collect();//强制进行拉圾回收
                return true;
            }
            catch (Exception ex)
            {
                JetEazy.LoggerClass.Instance.WriteException(ex);
                MessageBox.Show(ex.ToString());
                GC.Collect();//强制进行拉圾回收
                return false;
            }
        }
    }
    /// <summary>
    /// 图像校正
    /// </summary>
    [Serializable]
    public class CorrectToolRun
    {
        CAoiCalibration cAoi;
        public void SetcAoi(PointF[,] inView, PointF[,] World)
        {
            cAoi = new CAoiCalibration();
            cAoi.SetCalibrationPoints(inView, World);
            cAoi.CalculateTransformMatrix();
        }

        public PointF GetWorld(PointF View)
        {
            PointF world = new PointF(0, 0);
            cAoi.TransformViewToWorld(View, out world);

            return world;
        }
    }
    /// <summary>
    /// 校正参数工具类
    /// </summary>
    [Serializable]
    public class CorrectTool
    {
        /// <summary>
        /// CCD亮度
        /// </summary>
        public int CCDBright { get; set; }
        /// <summary>
        /// CCD编号
        /// </summary>
        public int CCDindex { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public CorrectPar Par { get; set; }
        /// <summary>
        /// 跑线图片
        /// </summary>
        public Bitmap bmp { get; set; }
        /// <summary>
        /// 实标坐标的点位 
        /// </summary>
        public PointF[,] pointsForWord { get; set; }
        /// <summary>
        /// 图片上的点位
        /// </summary>
        public PointF[,] pointsForView { get; set; }
        /// <summary>
        /// 框选位置
        /// </summary>
        public RectangleF Rect { get; set; }

    }

    /// <summary>
    /// 校正参数类(需要在画面上设定）
    /// </summary>
    [Serializable]
    public class CorrectPar
    {
        /// <summary>
        /// 是否是白底黑点
        /// </summary>
        [Category("01.斑点属性"), DefaultValue(true)]
        [Description("为True 找的是白底黑点， 若为false 找的是黑底白点")]
        [DisplayName("0.斑点极性")]
        public bool ISBlack { get; set; }
        /// <summary>
        /// 斑点亮度最大值
        /// </summary>
        [Category("01.斑点属性"), DefaultValue(5)]
        [Description("最亮的斑点亮度值")]
        [DisplayName("1.亮度最大值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 255, 1, 0)]
        public byte BrightMax { get; set; }
        /// <summary>
        /// 斑点亮度最小值
        /// </summary>
        [Category("01.斑点属性"), DefaultValue(5)]
        [Description("最暗的斑点亮度值")]
        [DisplayName("2.亮度最小值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 255, 1, 0)]
        public byte BrightMin { get; set; }
        /// <summary>
        /// 斑点面积最大值
        /// </summary>
        [Category("01.斑点属性"), DefaultValue(5)]
        [Description("需要最大的斑点的面积")]
        [DisplayName("3.面积最大值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 100000, 1, 0)]
        public int AreaMax { get; set; }
        /// <summary>
        /// 斑点面积最小值
        /// </summary>
        [Category("01.斑点属性"), DefaultValue(5)]
        [Description("需要最小的斑点的面积")]
        [DisplayName("4.面积最小值")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 99999, 1, 0)]
        public int AreaMin { get; set; }

        /// <summary>
        /// 斑点间隔X
        /// </summary>
        [Category("02.斑点位置"), DefaultValue(5)]
        [Description("斑点间X方向间隔 单位：毫米")]
        [DisplayName("1.间隔X")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 255,1f, 2)]
        public float GapX { get; set; }
        /// <summary>
        /// 斑点间隔Y
        /// </summary>
        [Category("02.斑点位置"), DefaultValue(5)]
        [Description("斑点间Y方向间隔 单位：毫米")]
        [DisplayName("2.间隔Y")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 255, 1f, 2)]
        public float GapY { get; set; }
        /// <summary>
        /// 启始位置X
        /// </summary>
        [Category("02.斑点位置"), DefaultValue(5)]
        [Description("左上角第一个点位 X 实际位置 单位：毫米")]
        [DisplayName("3.启始X")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 255, 1f, 2)]
        public float PositionX { get; set; }
        /// <summary>
        /// 启始位置Y
        /// </summary>
        [Category("02.斑点位置"), DefaultValue(5)]
        [Description("左上角第一个点位 Y 实际位置 单位：毫米")]
        [DisplayName("4.启始Y")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 255, 1f, 2)]
        public float PositionY { get; set; }

        /// <summary>
        /// 斑点大小X
        /// </summary>
        [Category("02.斑点位置"), DefaultValue(5)]
        [Description("斑点实际的大小 X 方向有多少个点 单位：像素")]
        [DisplayName("5.斑点大小X")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 10000, 1f, 0)]
        public int SizeX { get; set; }
        /// <summary>
        /// 斑点大小Y
        /// </summary>
        [Category("02.斑点位置"), DefaultValue(5)]
        [Description("斑点实际的大小 Y 方向有多少个点 单位：像素")]
        [DisplayName("6.斑点大小Y")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 10000, 1f, 0)]
        public int SizeY { get; set; }

        /// <summary>
        /// 斑点纵向间隔
        /// </summary>
        [Category("02.斑点位置"), DefaultValue(5)]
        [Description("斑点在图面上 Y 方向间隔有多少个点 单位：像素")]
        [DisplayName("6.图面间隔Y")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0f, 10000, 1f, 0)]
        public int GapYView { get; set; }
    }
}
