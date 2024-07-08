using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JetEazy.PlugSpace
{
    /*
     * 建立于20221213 Gaara
     * 用于解析参数页面的数据
     * 自动算出拍照位置
     * 然后可以自动算出框选的位置
     * 
     * 
     * 
     */
    public class EnvAnalyzePostionSettings
    {
        public float GetImageStart { get; set; } = 0;
        public float GetImageEnd { get; set; } = 0;
        public int GetImageCount { get; set; } = 1;
        public int RectRowCount { get; set; } = 1;
        public int RectColumnCount { get; set; } = 1;

        public float[] GetImagePostions;

        //SDM2
        public string[,] GetImagePreions;
        public float GetImageStart1x { get; set; } = 0;
        public float GetImageStart1y { get; set; } = 0;

        public float GetImageStart2x { get; set; } = 0;
        public float GetImageStart2y { get; set; } = 0;
        /// <summary>
        /// X方向拍照次数
        /// </summary>
        public int GetImageCountX { get; set; } = 1;
        /// <summary>
        /// Y方向拍照次数
        /// </summary>
        public int GetImageCountY { get; set; } = 1;
        /// <summary>
        /// X方向距离
        /// </summary>
        public float GetImageCountXoffset { get; set; } = 1;
        /// <summary>
        /// Y方向距离
        /// </summary>
        public float GetImageCountYoffset { get; set; } = 1;

        public int SEGCount { get; set; } = 1;

        /// <summary>
        /// 解析马达需要运动的位置参数
        /// </summary>
        public void EnvAnalyzePostions()
        {
            if (GetImageCount == 0)
                GetImageCount = 1;

            GetImagePostions = new float[GetImageCount];
            GetImagePostions[0] = GetImageStart;
            if (GetImageCount == 1)
                return;
            //GetImagePostions[GetImageCount - 1] = GetImageEnd;
            //if (GetImageCount == 2)
            //    return;

            int _move_count = GetImageCount - 1;
            float _singlepos = (GetImageEnd - GetImageStart) / _move_count;

            int _move_index = 0;
            while (_move_index <= _move_count)
            {
                GetImagePostions[_move_index] = GetImageStart + _move_index * _singlepos;
                _move_index++;
            }

        }

        public void EnvAnalyzePostionsSDM2()
        {
            if (SEGCount == 0)
                SEGCount = 1;

            if (GetImageCountX == 0)
                GetImageCountX = 1;
            if (GetImageCountY == 0)
                GetImageCountY = 1;

            GetImagePreions = new string[GetImageCountX, GetImageCountY];

            for (int i = 0; i < GetImageCountX; i++)
            {
                for (int j = 0; j < GetImageCountY; j++)
                {
                    float x = GetImageStart1x + GetImageCountXoffset * i;
                    float y = GetImageStart1y + GetImageCountYoffset * j;
                    GetImagePreions[i, j] = x.ToString() + "," + y.ToString();
                }
            }


            //if (GetImageCount == 0)
            //    GetImageCount = 1;

            //GetImagePostions = new float[GetImageCount];
            //GetImagePostions[0] = GetImageStart;
            //if (GetImageCount == 1)
            //    return;
            ////GetImagePostions[GetImageCount - 1] = GetImageEnd;
            ////if (GetImageCount == 2)
            ////    return;

            //int _move_count = GetImageCount - 1;
            //float _singlepos = (GetImageEnd - GetImageStart) / _move_count;

            //int _move_index = 0;
            //while (_move_index <= _move_count)
            //{
            //    GetImagePostions[_move_index] = GetImageStart + _move_index * _singlepos;
            //    _move_index++;
            //}

        }

        public EnvAnalyzePostionSettings()
        {

        }
        public EnvAnalyzePostionSettings(string str)
        {
            FromString(str);
        }
        void FromString(string str)
        {
            string[] strArr = str.Split(';');
            if (!string.IsNullOrEmpty(strArr[0]))
                GetImageStart = float.Parse(strArr[0]);
            if (!string.IsNullOrEmpty(strArr[1]))
                GetImageEnd = float.Parse(strArr[1]);
            if (!string.IsNullOrEmpty(strArr[2]))
                GetImageCount = int.Parse(strArr[2]);
            if (!string.IsNullOrEmpty(strArr[3]))
                RectRowCount = int.Parse(strArr[3]);
            if (!string.IsNullOrEmpty(strArr[4]))
                RectColumnCount = int.Parse(strArr[4]);

            //GetImageStart1x = float.Parse(strArr[5].Split(',')[0]);
            //GetImageStart1y = float.Parse(strArr[5].Split(',')[1]);
            if (!string.IsNullOrEmpty(strArr[6]))
                GetImageCountXoffset = float.Parse(strArr[6]);
            if (!string.IsNullOrEmpty(strArr[7]))
                GetImageCountYoffset = float.Parse(strArr[7]);

            //GetImageCountXoffset = float.Parse(strArr[6].Split(',')[0]);
            //GetImageCountX = int.Parse(strArr[6].Split(',')[1]);

            //GetImageCountYoffset = float.Parse(strArr[7].Split(',')[0]);
            //GetImageCountY = int.Parse(strArr[7].Split(',')[1]);

            //GetImageStart2x = float.Parse(strArr[8].Split(',')[0]);
            //GetImageStart2y = float.Parse(strArr[8].Split(',')[1]);

            if (!string.IsNullOrEmpty(strArr[9]))
                SEGCount = int.Parse(strArr[9]);
        }
        //public override string ToString()
        //{
        //    return base.ToString();
        //}
    }
}
