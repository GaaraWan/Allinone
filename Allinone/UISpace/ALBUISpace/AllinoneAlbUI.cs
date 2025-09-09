using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Allinone.OPSpace;
using Allinone.FormSpace;
using JetEazy;
using JetEazy.BasicSpace;
using System.Reflection;
using System.IO;
using JetEazy.UISpace;
using System.Drawing.Design;

namespace Allinone.UISpace.ALBUISpace
{
    public partial class AllinoneAlbUI : UserControl
    {
        enum TagEnum
        {
            ADD,
            DEL,
            DETAIL,
            COMPOUND,

            SETPOSITION,
            SETEND,
            GOPOSITION,
        }

        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
        public class DisplayModeAttribute : Attribute
        {
            private readonly string mode;
            public DisplayModeAttribute(string mode)
            {
                this.mode = mode ?? "";

            }
            public override bool Match(object obj)
            {
                var other = obj as DisplayModeAttribute;
                if (other == null) return false;

                if (other.mode == mode) return true;

                // allow for a comma-separated match, in either direction
                if (mode.IndexOf(',') >= 0)
                {
                    string[] tokens = mode.Split(',');
                    if (Array.IndexOf(tokens, other.mode) >= 0) return true;
                }
                else if (other.mode.IndexOf(',') >= 0)
                {
                    string[] tokens = other.mode.Split(',');
                    if (Array.IndexOf(tokens, mode) >= 0) return true;
                }
                return false;
            }
        }

        [DefaultPropertyAttribute("Environment Light")]
        public class LightSettings
        {
            private bool top = true;
            private bool mylar = true;

            public override string ToString()
            {
                string retstr = "";

                retstr = (top ? "1" : "0") + ",";
                retstr += (mylar ? "1" : "0");

                return retstr;
            }

            public void GetString(string str)
            {
                int i = 0;
                string[] strs = str.Split(',');

                i = 0;
                foreach (string strx in strs)
                {
                    switch (i)
                    {
                        case 0:
                            TOP = strx == "1";
                            break;
                        case 1:
                            MYLAR = strx == "1";
                            break;
                    }
                    i++;
                }
            }


            [CategoryAttribute("Light Settings"),
            DefaultValueAttribute(true)]
            public bool TOP
            {
                get { return top; }
                set { top = value; }
            }

            [CategoryAttribute("Light Settings"),
            DefaultValueAttribute(true)]
            public bool MYLAR
            {
                get { return mylar; }
                set { mylar = value; }
            }
        }
        [DefaultPropertyAttribute("Environment Light")]
        public class Light2Settings
        {
            private bool top = true;
            private bool front = true;
            private bool back = true;

            private int _r = 255;
            private int _g = 255;
            private int _b = 255;
            private int _w = 255;
            private int _highestValue = 138;

            private int _thresholdvalue = 128;
            private PointF _ptfCenter = new PointF(0, 0);
            private RectangleF _rectF = new RectangleF(0, 0, 100, 100);
            private bool _iswhite = false;

            private bool _IsCheckBarcodeOpen = false;
            private bool _IsOpenCheckRepeatCode = false;
            private bool _IsOpenCheckCurLotRepeatCode = false;


            private int _row = 1;
            private int _col = 1;
            private PathPlan _pathplan = PathPlan.p1;


            [CategoryAttribute("Light Settings"),
            DefaultValueAttribute(true), DescriptionAttribute("true: 比对 false:不比对")]
            [Browsable(true)]
            [DisplayName("A00.是否比对条码")]
            public bool IsCheckBarcodeOpen
            {
                get { return _IsCheckBarcodeOpen; }
                set { _IsCheckBarcodeOpen = value; }
            }
            [CategoryAttribute("Light Settings"),
            DefaultValueAttribute(true), DescriptionAttribute("true: 比对 false:不比对")]
            [DisplayName("A01.是否比对重复码")]
            [Browsable(true)]
            public bool IsOpenCheckRepeatCode
            {
                get { return _IsOpenCheckRepeatCode; }
                set { _IsOpenCheckRepeatCode = value; }
            }
            [CategoryAttribute("Light Settings"),
            DefaultValueAttribute(true), DescriptionAttribute("true: 比对 false:不比对 注:需要先开启 是否比对重复码")]
            [DisplayName("A02.是否比对当前批号重复码")]
            [Browsable(true)]
            public bool IsOpenCheckCurLotRepeatCode
            {
                get { return _IsOpenCheckCurLotRepeatCode; }
                set { _IsOpenCheckCurLotRepeatCode = value; }
            }

            [CategoryAttribute("Light Settings"),
            DefaultValueAttribute(true), DescriptionAttribute("Chip的行数")]
            [DisplayName("A03.行数")]
            public int ChipRow
            {
                get { return _row; }
                set { _row = value; }
            }
            [CategoryAttribute("Light Settings"),
            DefaultValueAttribute(true), DescriptionAttribute("Chip的列数")]
            [DisplayName("A04.列数")]
            public int ChipCol
            {
                get { return _col; }
                set { _col = value; }
            }
            [CategoryAttribute("Light Settings"),
           DefaultValueAttribute(true), DescriptionAttribute("Chip的列数")]
            [DisplayName("A05.路径")]
            [TypeConverter(typeof(JzEnumConverter))]
            public PathPlan ChipPathPlan
            {
                get { return _pathplan; }
                set { _pathplan = value; }
            }

            public override string ToString()
            {
                string retstr = "";

                retstr += (top ? "1" : "0") + ",";
                retstr += (front ? "1" : "0") + ",";
                retstr += (back ? "1" : "0") + ",";
                retstr += _r.ToString() + ",";
                retstr += _g.ToString() + ",";
                retstr += _b.ToString() + ",";
                retstr += _w.ToString() + ",";
                retstr += _highestValue.ToString() + ",";

                retstr += _thresholdvalue.ToString() + ",";
                retstr += PointF000ToString(_ptfCenter) + ",";
                retstr += RectFToString(_rectF) + ",";
                retstr += (_iswhite ? "1" : "0") + ",";
                retstr += (_IsCheckBarcodeOpen ? "1" : "0") + ",";
                retstr += (_IsOpenCheckRepeatCode ? "1" : "0") + ",";
                retstr += (_IsOpenCheckCurLotRepeatCode ? "1" : "0") + ",";

                retstr += _row.ToString() + ",";
                retstr += _col.ToString() + ",";
                retstr += ((int)_pathplan).ToString() + ",";

                return retstr;
            }

            public void GetString(string str)
            {
                int i = 0;
                string[] strs = str.Split(',');

                i = 0;
                foreach (string strx in strs)
                {
                    switch (i)
                    {
                        case 0:
                            TOP = strx == "1";
                            break;
                        case 1:
                            FRONT = strx == "1";
                            break;
                        case 2:
                            BACK = strx == "1";
                            break;
                        case 3:
                            _r = int.Parse(strx);
                            break;
                        case 4:
                            _g = int.Parse(strx);
                            break;
                        case 5:
                            _b = int.Parse(strx);
                            break;
                        case 6:
                            _w = int.Parse(strx);
                            break;
                        case 7:
                            _highestValue = int.Parse(strx);
                            break;
                        case 8:
                            _thresholdvalue = int.Parse(strx);
                            break;
                        case 9:
                            _ptfCenter = StringToPointF(strx);
                            break;
                        case 10:
                            _rectF = StringToRectF(strx);
                            break;
                        case 11:
                            _iswhite = strx == "1";
                            break;
                        case 12:
                            _IsCheckBarcodeOpen = strx == "1";
                            break;
                        case 13:
                            _IsOpenCheckRepeatCode = strx == "1";
                            break;
                        case 14:
                            _IsOpenCheckCurLotRepeatCode = strx == "1";
                            break;
                        case 15:
                            _row = int.Parse(strx);
                            break;
                        case 16:
                            _col = int.Parse(strx);
                            break;
                        case 17:
                            if (!string.IsNullOrEmpty(strx))
                            {
                                _pathplan = (PathPlan)int.Parse(strx);
                            }
                            break;
                    }
                    i++;
                }
            }


            [CategoryAttribute("Light Settings"),
            DefaultValueAttribute(true)]
            public bool TOP
            {
                get { return top; }
                set { top = value; }
            }

            [CategoryAttribute("Light Settings"),
            DefaultValueAttribute(true)]
            public bool FRONT
            {
                get { return front; }
                set { front = value; }
            }
            [CategoryAttribute("Light Settings"),
           DefaultValueAttribute(true)]
            public bool BACK
            {
                get { return back; }
                set { back = value; }
            }

            [CategoryAttribute("Light Settings"),
           DefaultValueAttribute(true)]
            public int R
            {
                get { return _r; }
                set { _r = value; }
            }
            [CategoryAttribute("Light Settings"),
           DefaultValueAttribute(true)]
            public int G
            {
                get { return _g; }
                set { _g = value; }
            }
            [CategoryAttribute("Light Settings"),
           DefaultValueAttribute(true)]
            public int B
            {
                get { return _b; }
                set { _b = value; }
            }
            [CategoryAttribute("Light Settings"),
           DefaultValueAttribute(true)]
            public int W
            {
                get { return _w; }
                set { _w = value; }
            }
            [CategoryAttribute("Light Settings"),
          DefaultValueAttribute(true), DisplayName("高度差")]
            public int HighestValue
            {
                get { return _highestValue; }
                set { _highestValue = value; }
            }

            [CategoryAttribute("Light Settings"),
         DefaultValueAttribute(true), DisplayName("阈值"), Browsable(false)]
            public int ThresholdValue
            {
                get { return _thresholdvalue; }
                set { _thresholdvalue = value; }
            }
            [CategoryAttribute("Light Settings"),
        DefaultValueAttribute(true), DisplayName("中心"), Browsable(false)]
            public PointF PtfCenter
            {
                get { return _ptfCenter; }
                set { _ptfCenter = value; }
            }
            [CategoryAttribute("Light Settings"),
        DefaultValueAttribute(true), DisplayName("寻找区域"), Browsable(false)]
            public RectangleF RectF
            {
                get { return _rectF; }
                set { _rectF = value; }
            }

            [CategoryAttribute("Light Settings"),
       DefaultValueAttribute(true), DisplayName("找白色"), Browsable(false)]
            public bool IsWhite
            {
                get { return _iswhite; }
                set { _iswhite = value; }
            }


            public string PointF000ToString(PointF PTF)
            {
                return PTF.X.ToString("0.000") + ";" + PTF.Y.ToString("0.000");
            }
            public PointF StringToPointF(string Str)
            {
                string[] strs = Str.Split(';');
                return new PointF(float.Parse(strs[0]), float.Parse(strs[1]));
            }
            public string RectFToString(RectangleF RectF)
            {
                string Str = "";

                Str += RectF.X.ToString("0.00") + ";";
                Str += RectF.Y.ToString("0.00") + ";";
                Str += RectF.Width.ToString("0.00") + ";";
                Str += RectF.Height.ToString("0.00");

                return Str;
            }
            public RectangleF StringToRectF(string Str)
            {
                string[] strs = Str.Split(';');
                RectangleF rectF = new RectangleF();

                rectF.X = float.Parse(strs[0]);
                rectF.Y = float.Parse(strs[1]);
                rectF.Width = float.Parse(strs[2]);
                rectF.Height = float.Parse(strs[3]);

                return rectF;


            }
        }

        [DefaultPropertyAttribute("Environment Light")]
        public class R32LightSettings
        {
            private bool top = true;
            private bool arround = true;
            private bool god = true;
            private bool pannel = true;
            private bool circle = true;
            private bool stilts = true;

            public override string ToString()
            {
                string retstr = "";

                retstr = (top ? "1" : "0") + ",";
                retstr += (arround ? "1" : "0") + ",";
                retstr += (god ? "1" : "0") + ",";
                retstr += (pannel ? "1" : "0") + ",";
                retstr += (circle ? "1" : "0") + ",";
                retstr += (stilts ? "1" : "0");
                return retstr;
            }
            public void GetString(string str)
            {
                int i = 0;
                string[] strs = str.Split(',');

                i = 0;
                foreach (string strx in strs)
                {
                    switch (i)
                    {
                        case 0:
                            TOP = strx == "1";
                            break;
                        case 1:
                            AROUND = strx == "1";
                            break;
                        case 2:
                            GOD = strx == "1";
                            break;
                        case 3:
                            PANNEL = strx == "1";
                            break;
                        case 4:
                            CIRCLE = strx == "1";
                            break;
                        case 5:
                            STILTS = strx == "1";
                            break;
                    }
                    i++;
                }
            }
            [CategoryAttribute("Light Settings"),
            DefaultValueAttribute(true)]
            [DisplayName("頂燈")]
            public bool TOP
            {
                get { return top; }
                set { top = value; }
            }

            [CategoryAttribute("Light Settings"),
                DefaultValueAttribute(true)]
            [DisplayName("四週燈管")]
            public bool AROUND
            {
                get { return arround; }
                set { arround = value; }
            }
            [CategoryAttribute("Light Settings"),
                    DefaultValueAttribute(true)]
            [DisplayName("神燈")]
            public bool GOD
            {
                get { return god; }
                set { god = value; }
            }
            [CategoryAttribute("Light Settings"),
                    DefaultValueAttribute(true)]
            [DisplayName("鐳雕平板燈")]
            public bool PANNEL
            {
                get { return pannel; }
                set { pannel = value; }
            }
            [CategoryAttribute("Light Settings"),
                    DefaultValueAttribute(true)]
            [DisplayName("小圓燈")]
            public bool CIRCLE
            {
                get { return circle; }
                set { circle = value; }
            }

            [CategoryAttribute("Light Settings"),
                 DefaultValueAttribute(true)]
            [DisplayName("高跷灯")]
            public bool STILTS
            {
                get { return stilts; }
                set { stilts = value; }
            }
        }
        [DefaultPropertyAttribute("Environment Light")]
        public class MainsdLightSettings
        {
            private bool pannel = true;
            private bool bottom = false;
            private bool _bCheckNoHave = false;
            private float _CheckNoHaveRatio = 0.1f;
            private bool _bCheckNoDIE = false;

            public override string ToString()
            {
                string retstr = "";

                retstr += (pannel ? "1" : "0") + ",";
                retstr += (bottom ? "1" : "0") + ",";
                retstr += (_bCheckNoHave ? "1" : "0") + ",";
                retstr += _CheckNoHaveRatio.ToString() + ",";
                retstr += (_bCheckNoDIE ? "1" : "0");

                return retstr;
            }
            public void GetString(string str)
            {
                int i = 0;
                string[] strs = str.Split(',');

                i = 0;
                foreach (string strx in strs)
                {
                    switch (i)
                    {
                        case 0:
                            PANNEL = strx == "1";
                            break;
                        case 1:
                            bottom = strx == "1";
                            break;
                        case 2:
                            _bCheckNoHave = strx == "1";
                            break;
                        case 3:
                            _CheckNoHaveRatio = float.Parse(strx);
                            break;
                        case 4:
                            _bCheckNoDIE = strx == "1";
                            break;
                    }
                    i++;
                }
            }


            [CategoryAttribute("Light Settings"),
                    DefaultValueAttribute(true)]
            [DisplayName("外同轴")]
            public bool PANNEL
            {
                get { return pannel; }
                set { pannel = value; }
            }

            [CategoryAttribute("Light Settings"),
                    DefaultValueAttribute(true)]
            [DisplayName("内同轴")]
            public bool BOTTOMLED
            {
                get { return bottom; }
                set { bottom = value; }
            }
            [CategoryAttribute("Light Settings"),
                    DefaultValueAttribute(true)]
            [DisplayName("开启无芯片比例检测")]
            public bool bCheckNoHave
            {
                get { return _bCheckNoHave; }
                set { _bCheckNoHave = value; }
            }
            [CategoryAttribute("Light Settings"),
                    DefaultValueAttribute(true)]
            [DisplayName("无芯片比例值")]
            [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(0, 1, 2)]
            public float CheckNoHaveRatio
            {
                get { return _CheckNoHaveRatio; }
                set { _CheckNoHaveRatio = value; }
            }
            [CategoryAttribute("Light Settings"),
                    DefaultValueAttribute(true)]
            [DisplayName("开启缺Die检测")]
            public bool bCheckNoDIE
            {
                get { return _bCheckNoDIE; }
                set { _bCheckNoDIE = value; }
            }

        }


        [DefaultPropertyAttribute("Environment Position")]
        public class PositionSettings
        {
            private string[] position = new string[10];

            [CategoryAttribute("POS Settings"),
            DefaultValueAttribute("0"), ReadOnly(false)]
            public string POS0
            {
                get { return position[0]; }
                set { position[0] = value; }
            }
            [CategoryAttribute("POS Settings"),
            DefaultValueAttribute("0"), ReadOnly(false)]
            public string POS1
            {
                get { return position[1]; }
                set { position[1] = value; }
            }
            [CategoryAttribute("POS Settings"),
            DefaultValueAttribute("0"), ReadOnly(false)]
            public string POS2
            {
                get { return position[2]; }
                set { position[2] = value; }
            }
            [CategoryAttribute("POS Settings"),
            DefaultValueAttribute("0"), ReadOnly(false)]
            public string POS3
            {
                get { return position[3]; }
                set { position[3] = value; }
            }
            [CategoryAttribute("POS Settings"),
            DefaultValueAttribute("0"), ReadOnly(false)]
            public string POS4
            {
                get { return position[4]; }
                set { position[4] = value; }
            }
            [CategoryAttribute("POS Settings"),
            DefaultValueAttribute("0"), ReadOnly(false)]
            public string POS5
            {
                get { return position[5]; }
                set { position[5] = value; }
            }
            [CategoryAttribute("POS Settings"),
            DefaultValueAttribute("0"), ReadOnly(false)]
            public string POS6
            {
                get { return position[6]; }
                set { position[6] = value; }
            }
            [CategoryAttribute("POS Settings"),
            DefaultValueAttribute("0"), ReadOnly(false)]
            public string POS7
            {
                get { return position[7]; }
                set { position[7] = value; }
            }
            [CategoryAttribute("POS Settings"),
            DefaultValueAttribute("0"), ReadOnly(false)]
            public string POS8
            {
                get { return position[8]; }
                set { position[8] = value; }
            }
            [CategoryAttribute("POS Settings"),
            DefaultValueAttribute("0"), ReadOnly(false)]
            public string POS9
            {
                get { return position[9]; }
                set { position[9] = value; }
            }

            public override string ToString()
            {
                string retstr = "";

                int i = 0;

                while (i < position.Length)
                {
                    retstr += position[i] + ";";
                    i++;
                }
                retstr = retstr.Remove(retstr.Length - 1, 1);

                return retstr;
            }

            public void SetPosition(int index, string str)
            {
                position[index] = str;

            }
            public int GetPosCount()
            {
                return position.Length;
            }

        }

        [DefaultPropertyAttribute("Environment Position")]
        public class SDM1PositionSettings
        {
            private string[] position = new string[10];
            /// <summary>
            /// 拍照开始位置
            /// </summary>
            [CategoryAttribute("POS Settings"),
            DefaultValueAttribute("0"), ReadOnly(false)]
            [DisplayName("01.拍照开始位置"), Description("即开始拍照位置")]
            public string SDM1_POS0
            {
                get { return position[0]; }
                set { position[0] = value; }
            }
            /// <summary>
            /// 拍照结束位置
            /// </summary>
            [CategoryAttribute("POS Settings"),
            DefaultValueAttribute("0"), ReadOnly(false)]
            [DisplayName("02.拍照结束位置"), Description("即结束拍照位置")]
            public string SDM1_POS1
            {
                get { return position[1]; }
                set { position[1] = value; }
            }
            /// <summary>
            /// 拍照次数
            /// </summary>
            [CategoryAttribute("POS Settings"),
            DefaultValueAttribute("0"), ReadOnly(false)]
            [DisplayName("03.拍照次数"), Description("")]
            public string SDM1_POS2
            {
                get { return position[2]; }
                set { position[2] = value; }
            }
            /// <summary>
            /// 行数
            /// </summary>
            [CategoryAttribute("POS Settings"),
            DefaultValueAttribute("0"), ReadOnly(false)]
            [DisplayName("04.行数"), Description("即晶片行数")]
            public string SDM1_POS3
            {
                get { return position[3]; }
                set { position[3] = value; }
            }
            /// <summary>
            /// 列数
            /// </summary>
            [CategoryAttribute("POS Settings"),
            DefaultValueAttribute("0"), ReadOnly(false)]
            [DisplayName("05.列数"), Description("即晶片列数")]
            public string SDM1_POS4
            {
                get { return position[4]; }
                set { position[4] = value; }
            }
            [CategoryAttribute("POS Settings"),
            DefaultValueAttribute("0"), ReadOnly(false)]
            [DisplayName("06.拍照起始位置"), Description("")]
            public string SDM1_POS5
            {
                get { return position[5]; }
                set { position[5] = value; }
            }
            [CategoryAttribute("POS Settings"),
            DefaultValueAttribute("0"), ReadOnly(false)]
            [DisplayName("07.x方向距离及次数"), Description("")]
            public string SDM1_POS6
            {
                get { return position[6]; }
                set { position[6] = value; }
            }
            [CategoryAttribute("POS Settings"),
            DefaultValueAttribute("0"), ReadOnly(false)]
            [DisplayName("08.y方向距离及次数"), Description("")]
            public string SDM1_POS7
            {
                get { return position[7]; }
                set { position[7] = value; }
            }
            [CategoryAttribute("POS Settings"),
            DefaultValueAttribute("0"), ReadOnly(false)]
            [DisplayName("09.区块2拍照起始位置"), Description("")]
            public string SDM1_POS8
            {
                get { return position[8]; }
                set { position[8] = value; }
            }
            [CategoryAttribute("POS Settings"),
            DefaultValueAttribute("0"), ReadOnly(false)]
            [DisplayName("10.区块个数"), Description("")]
            public string SDM1_POS9
            {
                get { return position[9]; }
                set { position[9] = value; }
            }

            public override string ToString()
            {
                string retstr = "";

                int i = 0;

                while (i < position.Length)
                {
                    retstr += position[i] + ";";
                    i++;
                }
                retstr = retstr.Remove(retstr.Length - 1, 1);

                return retstr;
            }

            public void SetPosition(int index, string str)
            {
                position[index] = str;

            }
            public int GetPosCount()
            {
                return position.Length;
            }

        }

        //[DefaultPropertyAttribute("Environment Position")]
        //public class SDM1PositionSettings
        //{
        //    private string[] position = new string[10];
        //    /// <summary>
        //    /// 拍照开始位置
        //    /// </summary>
        //    [CategoryAttribute("POS Settings"),
        //    DefaultValueAttribute("0"), ReadOnly(false)]
        //    [DisplayName("01.拍照开始位置"), Description("即开始拍照位置")]
        //    public string SDM1_POS0
        //    {
        //        get { return position[0]; }
        //        set { position[0] = value; }
        //    }
        //    /// <summary>
        //    /// 拍照结束位置
        //    /// </summary>
        //    [CategoryAttribute("POS Settings"),
        //    DefaultValueAttribute("0"), ReadOnly(false)]
        //    [DisplayName("02.拍照结束位置"), Description("即结束拍照位置")]
        //    public string SDM1_POS1
        //    {
        //        get { return position[1]; }
        //        set { position[1] = value; }
        //    }
        //    /// <summary>
        //    /// 拍照次数
        //    /// </summary>
        //    [CategoryAttribute("POS Settings"),
        //    DefaultValueAttribute("0"), ReadOnly(false)]
        //    [DisplayName("03.拍照次数"), Description("")]
        //    public string SDM1_POS2
        //    {
        //        get { return position[2]; }
        //        set { position[2] = value; }
        //    }
        //    /// <summary>
        //    /// 行数
        //    /// </summary>
        //    [CategoryAttribute("POS Settings"),
        //    DefaultValueAttribute("0"), ReadOnly(false)]
        //    [DisplayName("04.行数"), Description("即晶片行数")]
        //    public string SDM1_POS3
        //    {
        //        get { return position[3]; }
        //        set { position[3] = value; }
        //    }
        //    /// <summary>
        //    /// 列数
        //    /// </summary>
        //    [CategoryAttribute("POS Settings"),
        //    DefaultValueAttribute("0"), ReadOnly(false)]
        //    [DisplayName("05.列数"), Description("即晶片列数")]
        //    public string SDM1_POS4
        //    {
        //        get { return position[4]; }
        //        set { position[4] = value; }
        //    }
        //    [CategoryAttribute("POS Settings"),
        //    DefaultValueAttribute("0"), ReadOnly(false)]
        //    [DisplayName("A预留1"), Description("")]
        //    public string SDM1_POS5
        //    {
        //        get { return position[5]; }
        //        set { position[5] = value; }
        //    }
        //    [CategoryAttribute("POS Settings"),
        //    DefaultValueAttribute("0"), ReadOnly(false)]
        //    [DisplayName("A预留2"), Description("")]
        //    public string SDM1_POS6
        //    {
        //        get { return position[6]; }
        //        set { position[6] = value; }
        //    }
        //    [CategoryAttribute("POS Settings"),
        //    DefaultValueAttribute("0"), ReadOnly(false)]
        //    [DisplayName("A预留3"), Description("")]
        //    public string SDM1_POS7
        //    {
        //        get { return position[7]; }
        //        set { position[7] = value; }
        //    }
        //    [CategoryAttribute("POS Settings"),
        //    DefaultValueAttribute("0"), ReadOnly(false)]
        //    [DisplayName("A预留4"), Description("")]
        //    public string SDM1_POS8
        //    {
        //        get { return position[8]; }
        //        set { position[8] = value; }
        //    }
        //    [CategoryAttribute("POS Settings"),
        //    DefaultValueAttribute("0"), ReadOnly(false)]
        //    [DisplayName("A预留5"), Description("")]
        //    public string SDM1_POS9
        //    {
        //        get { return position[9]; }
        //        set { position[9] = value; }
        //    }

        //    public override string ToString()
        //    {
        //        string retstr = "";

        //        int i = 0;

        //        while (i < position.Length)
        //        {
        //            retstr += position[i] + ";";
        //            i++;
        //        }
        //        retstr = retstr.Remove(retstr.Length - 1, 1);

        //        return retstr;
        //    }

        //    public void SetPosition(int index, string str)
        //    {
        //        position[index] = str;

        //    }
        //    public int GetPosCount()
        //    {
        //        return position.Length;
        //    }

        //}

        Button btnAdd;
        Button btnDel;
        ComboBox cboEnv;

        Button btnDetail;
        Button btnCompound;

        Label lblPostion;
        Button btnSetPosition;
        Button btnSetEnd;
        Button btnGoPosition;

        Button btnReadBarcodeSetup;

        PropertyGrid ppgLight;
        PropertyGrid ppgPosition;

        OptionEnum OPTION;
        AlbumClass ALBUM;

        LightSettings LightSetting;
        R32LightSettings R32LightSetting;
        MainsdLightSettings MAINSDLightSetting;
        Light2Settings Light2Setting;

        PositionSettings PositionSetting;
        SDM1PositionSettings SDM1PositionSetting;

        EnvClass ENVNow
        {
            get
            {
                return ALBUM.ENVList[cboEnv.SelectedIndex];
            }

        }

        int ENVNowIndex
        {
            get
            {
                return cboEnv.SelectedIndex;
            }
        }

        bool IsNeedToChange = false;

        public AllinoneAlbUI()
        {
            InitializeComponent();
            InitialInternal();
        }

        protected void InitialInternal()
        {
            btnAdd = button7;
            btnDel = button8;
            btnDetail = button10;
            btnCompound = button2;
            btnSetPosition = button1;
            btnSetEnd = button3;
            btnGoPosition = button4;
            lblPostion = label3;
            btnReadBarcodeSetup = button5;

            cboEnv = comboBox1;

            ppgLight = propertyGrid1;
            ppgPosition = propertyGrid3;

            btnAdd.Tag = TagEnum.ADD;
            btnDel.Tag = TagEnum.DEL;
            btnDetail.Tag = TagEnum.DETAIL;
            btnCompound.Tag = TagEnum.COMPOUND;
            btnSetPosition.Tag = TagEnum.SETPOSITION;
            btnSetEnd.Tag = TagEnum.SETEND;
            btnGoPosition.Tag = TagEnum.GOPOSITION;

            ppgLight.PropertySort = PropertySort.NoSort;
            ppgPosition.PropertySort = PropertySort.NoSort;

            ppgLight.PropertyValueChanged += PpgLight_PropertyValueChanged;
            ppgPosition.PropertyValueChanged += PpgPosition_PropertyValueChanged;

            btnAdd.Click += btn_Click;
            btnDel.Click += btn_Click;
            btnDetail.Click += btn_Click;
            btnCompound.Click += btn_Click;
            btnSetPosition.Click += btn_Click;
            btnSetEnd.Click += btn_Click;
            btnGoPosition.Click += btn_Click;

            btnReadBarcodeSetup.Click += BtnReadBarcodeSetup_Click;

            cboEnv.SelectedIndexChanged += cboEnv_SelectedIndexChanged;
        }

        frmTestCommon mfrmTestCommon = null;
        private void BtnReadBarcodeSetup_Click(object sender, EventArgs e)
        {
            mfrmTestCommon = new frmTestCommon(ENVNow.GeneralBarcodeSetup);
            if (mfrmTestCommon.ShowDialog() == DialogResult.OK)
            {
                ENVNow.GeneralBarcodeSetup = mfrmTestCommon.ResultParaStr;
            }
            mfrmTestCommon.Close();
            mfrmTestCommon.Dispose();
            mfrmTestCommon = null;
        }

        public void Initial(OptionEnum option, AlbumClass album)
        {
            IsNeedToChange = false;

            OPTION = option;
            ALBUM = album;

 

            switch (OPTION)
            {
                case JetEazy.OptionEnum.MAIN_SERVICE:
                case OptionEnum.MAIN_X6:
                    Light2Setting = new Light2Settings();
                    break;
                case OptionEnum.MAIN_SDM3:
                case OptionEnum.MAIN_SDM2:
                case OptionEnum.MAIN_SDM1:
                case OptionEnum.MAIN_SD:
                    btnReadBarcodeSetup.Visible = true;
                    MAINSDLightSetting = new MainsdLightSettings();
                    break;
                case OptionEnum.MAIN:
                    LightSetting = new LightSettings();
                    break;
                case OptionEnum.R32:
                case OptionEnum.R26:
                case OptionEnum.R15:
                case OptionEnum.R9:
                case OptionEnum.R5:
                case OptionEnum.R3:
                case OptionEnum.C3:
                case OptionEnum.R1:
                    //case OptionEnum.MAIN_SD:
                    R32LightSetting = new R32LightSettings();
                    break;
            }

            switch (OPTION)
            {
                case OptionEnum.MAIN_SDM3:
                case OptionEnum.MAIN_SDM2:
                case OptionEnum.MAIN_SDM1:
                    SDM1PositionSetting = new SDM1PositionSettings();
                    break;
                default:
                    PositionSetting = new PositionSettings();
                    break;
            }


            cboEnv.Items.Clear();

            foreach (EnvClass env in album.ENVList)
            {
                cboEnv.Items.Add(env.ToEnvString());
            }

            IsNeedToChange = true;
            //cboEnv.SelectedIndex = 0;

            cboEnv.SelectedIndex = (album.ENVCount > 0 ? 0 : -1);

            switch (OPTION)
            {
                case OptionEnum.MAIN_SDM3:
                case OptionEnum.MAIN_SDM2:
                case OptionEnum.MAIN_SDM1:
                case OptionEnum.MAIN:

                    btnSetEnd.Visible = false;
                    btnGoPosition.Visible = false;
                    btnSetPosition.Visible = false;
                    ppgPosition.Visible = false;

                    break;
                case OptionEnum.R32:
                case OptionEnum.R26:
                case OptionEnum.R15:
                case OptionEnum.R9:
                case OptionEnum.R5:
                case OptionEnum.R3:
                case OptionEnum.C3:
                case OptionEnum.R1:
                case OptionEnum.MAIN_SD:
                case OptionEnum.MAIN_X6:
                case JetEazy.OptionEnum.MAIN_SERVICE:

                    ppgLight.Width = 320;
                    btnSetEnd.Visible = false;
                    btnGoPosition.Visible = false;
                    btnSetPosition.Visible = false;

                    ppgPosition.Visible = false;
                    lblPostion.Visible = false;

                    break;
            }


            FillDisplay();

            _changeLanguageLightSettings();
        }
        private void PpgPosition_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (!IsNeedToChange)
                return;

            WriteBackPosition();
        }
        private void PpgLight_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (!IsNeedToChange)
                return;

            WriteBackLight();
        }
        private void cboEnv_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            FillDisplay();

        }
        private void btn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            switch ((TagEnum)btn.Tag)
            {
                case TagEnum.ADD:
                    Add();
                    break;
                case TagEnum.DEL:
                    Del();
                    break;
                case TagEnum.DETAIL:
                    Detail();
                    break;
                case TagEnum.COMPOUND:
                    Compound();
                    break;
                case TagEnum.SETPOSITION:
                    SetPosition();
                    break;
                case TagEnum.SETEND:
                    SetEnd();
                    break;
                case TagEnum.GOPOSITION:
                    GoPosition();
                    break;
            }
        }

        PAGECountForm PAGECOUNTFRM;
        void Add()
        {
            PAGECOUNTFRM = new PAGECountForm();
            PAGECOUNTFRM.ShowDialog();

            int envno = 0;

            if (ALBUM.ENVCount == 0)
                envno = 0;
            else
                envno = ALBUM.LastENV.No + 1;

            //ALBUM.AddEnv(ENVNow);

            EnvClass NewENV = new EnvClass(envno, JzToolsClass.PassingInteger, ALBUM.PassInfo);

            //if (ALBUM.ENVCount == 0)
            ALBUM.ENVList.Add(NewENV);
            //else
            //    ALBUM.AddEnv(ENVNow);

            IsNeedToChange = false;

            cboEnv.Items.Add(ALBUM.LastENV.ToEnvString());

            IsNeedToChange = true;

            cboEnv.SelectedIndex = ALBUM.ENVList.Count - 1;

        }
        void Del()
        {
            //FOR LASER TRANSLATION
            if (MessageBox.Show(ToChangeLanguage("是否要刪除此環境?"), "SYSTEM", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ALBUM.DelEnv(ENVNow.No);

                IsNeedToChange = false;

                int delindicator = cboEnv.SelectedIndex;

                cboEnv.Items.RemoveAt(delindicator);

                IsNeedToChange = true;

                if (delindicator == ALBUM.ENVList.Count)
                {
                    cboEnv.SelectedIndex = delindicator - 1;
                }
                else
                    cboEnv.SelectedIndex = delindicator;

                FillDisplay();
            }
        }
        void Detail()
        {
            JzToolsClass.PassingInteger = ENVNow.No;

            OnTrigger(RCPStatusEnum.SHOWDETAIL, "");
        }
        void Compound()
        {
            OnTrigger(RCPStatusEnum.SHOWCOMPOUND, "");
        }
        void SetPosition()
        {
            OnTrigger(RCPStatusEnum.SETPOSITION, "");
        }
        void SetEnd()
        {
            OnTrigger(RCPStatusEnum.SETEND, "");
        }
        void GoPosition()
        {
            OnTrigger(RCPStatusEnum.GOPOSITION, GetPosition());
        }
        public void SetPosition(string posstr)
        {
            int i = 0;

            string selectstr = ppgPosition.SelectedGridItem.Label;

            switch (selectstr)
            {
                #region SDM1

                case "01.拍照开始位置":
                    SDM1PositionSetting.SDM1_POS0 = posstr;
                    break;
                case "02.拍照结束位置":
                    SDM1PositionSetting.SDM1_POS1 = posstr;
                    break;

                case "06.拍照起始位置":
                    SDM1PositionSetting.SDM1_POS5 = posstr;
                    break;
                case "09.区块2拍照起始位置":
                    SDM1PositionSetting.SDM1_POS8 = posstr;
                    break;

                #endregion
                case "POS0":
                    PositionSetting.POS0 = posstr;
                    break;
                case "POS1":
                    PositionSetting.POS1 = posstr;
                    break;
                case "POS2":
                    PositionSetting.POS2 = posstr;
                    break;
                case "POS3":
                    PositionSetting.POS3 = posstr;
                    break;
                case "POS4":
                    PositionSetting.POS4 = posstr;
                    break;
                case "POS5":
                    PositionSetting.POS5 = posstr;
                    break;
                case "POS6":
                    PositionSetting.POS6 = posstr;
                    break;
                case "POS7":
                    PositionSetting.POS7 = posstr;
                    break;
                case "POS8":
                    PositionSetting.POS8 = posstr;
                    break;
                case "POS9":
                    PositionSetting.POS9 = posstr;
                    break;
            }

            WriteBackPosition();
            FillPosition();
        }
        public string GetPosition()
        {
            int i = 0;

            string retstr = "";

            string selectstr = ppgPosition.SelectedGridItem.Label;

            switch (selectstr)
            {
                #region SDM1

                case "01.拍照开始位置":
                    retstr = SDM1PositionSetting.SDM1_POS0;
                    break;
                case "02.拍照结束位置":
                    retstr = SDM1PositionSetting.SDM1_POS1;
                    break;

                #endregion
                case "POS0":
                    retstr = PositionSetting.POS0;
                    break;
                case "POS1":
                    retstr = PositionSetting.POS1;
                    break;
                case "POS2":
                    retstr = PositionSetting.POS2;
                    break;
                case "POS3":
                    retstr = PositionSetting.POS3;
                    break;
                case "POS4":
                    retstr = PositionSetting.POS4;
                    break;
                case "POS5":
                    retstr = PositionSetting.POS5;
                    break;
                case "POS6":
                    retstr = PositionSetting.POS6;
                    break;
                case "POS7":
                    retstr = PositionSetting.POS7;
                    break;
                case "POS8":
                    retstr = PositionSetting.POS8;
                    break;
                case "POS9":
                    retstr = PositionSetting.POS9;
                    break;
            }

            return retstr;

        }
        public void WriteBack()
        {
            WriteBackLight();
            WriteBackPosition();
        }
        void WriteBackLight()
        {
            switch (OPTION)
            {
                case JetEazy.OptionEnum.MAIN_SERVICE:
                case OptionEnum.MAIN_X6:
                    ENVNow.GeneralLight = Light2Setting.ToString();
                    break;
                case OptionEnum.MAIN_SDM3:
                case OptionEnum.MAIN_SDM2:
                case OptionEnum.MAIN_SDM1:
                case OptionEnum.MAIN_SD:
                    ENVNow.GeneralLight = MAINSDLightSetting.ToString();
                    break;
                case OptionEnum.MAIN:
                    ENVNow.GeneralLight = LightSetting.ToString();
                    break;
                case OptionEnum.R32:
                case OptionEnum.R26:
                case OptionEnum.R15:
                case OptionEnum.R9:
                case OptionEnum.R5:
                case OptionEnum.R3:
                case OptionEnum.C3:
                case OptionEnum.R1:
                    ENVNow.GeneralLight = R32LightSetting.ToString();
                    break;
            }

            OnTrigger(RCPStatusEnum.CHANGELIGHT, cboEnv.SelectedIndex.ToString() + ";" + ENVNow.GeneralLight);
        }
        void WriteBackPosition()
        {
            switch (OPTION)
            {
                case OptionEnum.MAIN_SDM3:
                case OptionEnum.MAIN_SDM2:
                case OptionEnum.MAIN_SDM1:
                    ENVNow.GeneralPosition = SDM1PositionSetting.ToString();
                    break;
                default:
                    ENVNow.GeneralPosition = PositionSetting.ToString();
                    break;
            }
        }
        void FillDisplay()
        {
            FillLight();
            FillPosition();

            btnDel.Enabled = ALBUM.ENVCount > 0;
            btnDetail.Enabled = ALBUM.ENVCount > 0;

            cboEnv.Enabled = ALBUM.ENVCount > 0;

        }
        void FillLight()
        {
            IsNeedToChange = false;

            IsNeedToChange = false;

            if (ALBUM.ENVCount == 0)
            {
                ppgLight.SelectedObject = null;
            }
            else
            {

                string LightString = ENVNow.GeneralLight;

                switch (OPTION)
                {
                    case JetEazy.OptionEnum.MAIN_SERVICE:
                    case OptionEnum.MAIN_X6:

                        Light2Setting.GetString(LightString);
                        ppgLight.SelectedObject = Light2Setting;

                        break;
                    case OptionEnum.MAIN_SDM3:
                    case OptionEnum.MAIN_SDM2:
                    case OptionEnum.MAIN_SDM1:
                    case OptionEnum.MAIN_SD:
                        MAINSDLightSetting.GetString(LightString);
                        ppgLight.SelectedObject = MAINSDLightSetting;
                        break;
                    case OptionEnum.MAIN:
                        LightSetting.GetString(LightString);
                        ppgLight.SelectedObject = LightSetting;
                        break;
                    case OptionEnum.R32:
                    case OptionEnum.R26:
                    case OptionEnum.R15:
                    case OptionEnum.R9:
                    case OptionEnum.R5:
                    case OptionEnum.R3:
                    case OptionEnum.C3:
                    case OptionEnum.R1:
                        //case OptionEnum.MAIN_SD:
                        R32LightSetting.GetString(LightString);
                        ppgLight.SelectedObject = R32LightSetting;
                        break;
                }
            }

            IsNeedToChange = true;
        }
        void FillPosition()
        {
            IsNeedToChange = false;

            IsNeedToChange = false;

            if (ALBUM.ENVCount == 0)
            {
                ppgPosition.SelectedObject = null;
            }
            else
            {

                int i = 0;
                string PositionString = ENVNow.GeneralPosition;
                string[] positionstrs = PositionString.Split(';');

                switch (OPTION)
                {
                    case OptionEnum.MAIN_SDM3:
                    case OptionEnum.MAIN_SDM2:
                    case OptionEnum.MAIN_SDM1:

                        i = 0;
                        foreach (string str in positionstrs)
                        {
                            if (i < SDM1PositionSetting.GetPosCount())
                                SDM1PositionSetting.SetPosition(i, str);
                            i++;
                        }

                        ppgPosition.SelectedObject = SDM1PositionSetting;

                        break;
                    default:

                        i = 0;
                        foreach (string str in positionstrs)
                        {
                            if (i < PositionSetting.GetPosCount())
                                PositionSetting.SetPosition(i, str);
                            i++;
                        }

                        ppgPosition.SelectedObject = PositionSetting;

                        break;
                }
            }
            IsNeedToChange = true;
        }

        
        public delegate void TriggerHandler(RCPStatusEnum status, string opstr);
        public event TriggerHandler TriggerAction;
        public void OnTrigger(RCPStatusEnum status, string opstr)
        {
            if (TriggerAction != null)
            {
                TriggerAction(status, opstr);
            }
        }

        private Light2Settings _changeLanguageLightSettings()
        {
            string _collectDataStr = string.Empty;
            if (Light2Setting == null)
                return null;

            foreach (System.Reflection.PropertyInfo prop in Light2Setting.GetType().GetProperties())
            {
                string name = prop.Name;
                if (prop.GetCustomAttribute<DisplayNameAttribute>() != null)
                {
                    string dispName = prop.GetCustomAttribute<DisplayNameAttribute>().DisplayName;
                    _collectDataStr += dispName + Environment.NewLine;
                    if (name != "")
                    {
                        string strNewName = ToChangeLanguage(dispName);
                        if (strNewName != dispName)
                        {
                            PropertyDescriptorCollection appSetingAttributes = TypeDescriptor.GetProperties(Light2Setting);
                            Type displayType = typeof(DisplayNameAttribute);
                            FieldInfo fieldInfo = displayType.GetField("_displayName", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance);
                            if (fieldInfo != null)
                            {
                                fieldInfo.SetValue(appSetingAttributes[name].Attributes[displayType], strNewName);
                            }
                        }
                        else
                        {

                        }
                    }
                }

                if (prop.GetCustomAttribute<DescriptionAttribute>() != null)
                {
                    string strDescription = prop.GetCustomAttribute<DescriptionAttribute>().Description;
                    _collectDataStr += strDescription + Environment.NewLine;
                    string strNewName = ToChangeLanguage(strDescription);
                    if (strNewName != strDescription)
                    {
                        PropertyDescriptorCollection appSetingAttributes = TypeDescriptor.GetProperties(Light2Setting);
                        Type displayType = typeof(DescriptionAttribute);
                        FieldInfo fieldInfo = displayType.GetField("description", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance);
                        if (fieldInfo != null)
                            fieldInfo.SetValue(appSetingAttributes[name].Attributes[displayType], strNewName);
                    }
                }

                if (prop.GetCustomAttribute<CategoryAttribute>() != null)
                {
                    string strCategory = prop.GetCustomAttribute<CategoryAttribute>().Category;
                    _collectDataStr += strCategory + Environment.NewLine;
                    string strNewName = ToChangeLanguage(strCategory);
                    if (strNewName != strCategory)
                    {
                        PropertyDescriptorCollection appSetingAttributes = TypeDescriptor.GetProperties(Light2Setting);
                        Type displayType = typeof(CategoryAttribute);
                        FieldInfo fieldInfo = displayType.GetField("categoryValue", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance);
                        if (fieldInfo != null)
                            fieldInfo.SetValue(appSetingAttributes[name].Attributes[displayType], strNewName);
                    }
                }
                switch(Universal.FACTORYNAME)
                {
                    case FactoryName.DAGUI:
                        if (prop.GetCustomAttribute<BrowsableAttribute>() != null)
                        {
                            bool bBrowsable = prop.GetCustomAttribute<BrowsableAttribute>().Browsable;
                            if (name.Contains("IsCheckBarcodeOpen") || name.Contains("IsOpenCheckRepeatCode") || name.Contains("IsOpenCheckCurLotRepeatCode"))
                            {
                                PropertyDescriptorCollection appSetingAttributes = TypeDescriptor.GetProperties(Light2Setting);
                                Type displayType = typeof(BrowsableAttribute);
                                FieldInfo fieldInfo = displayType.GetField("browsable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance);
                                if (fieldInfo != null)
                                    fieldInfo.SetValue(appSetingAttributes[name].Attributes[displayType], false);
                            }
                        }
                        break;
                    case FactoryName.DONGGUAN:
                        if (prop.GetCustomAttribute<BrowsableAttribute>() != null)
                        {
                            bool bBrowsable = prop.GetCustomAttribute<BrowsableAttribute>().Browsable;
                            if (name.Contains("IsOpenCheckRepeatCode") || name.Contains("IsOpenCheckCurLotRepeatCode"))
                            {
                                PropertyDescriptorCollection appSetingAttributes = TypeDescriptor.GetProperties(Light2Setting);
                                Type displayType = typeof(BrowsableAttribute);
                                FieldInfo fieldInfo = displayType.GetField("browsable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance);
                                if (fieldInfo != null)
                                    fieldInfo.SetValue(appSetingAttributes[name].Attributes[displayType], false);
                            }
                        }
                        break;
                }
            }

            //SaveData(_collectDataStr, "D:\\log.csv");

            return Light2Setting;
        }
        string ToChangeLanguage(string eText)
        {
            string retStr = eText;
            retStr = LanguageExClass.Instance.GetLanguageText(eText);
            return retStr;
        }
        void SaveData(string DataStr, string FileName)
        {
            StreamWriter Swr = new StreamWriter(FileName, true, Encoding.Default);

            Swr.Write(DataStr);

            Swr.Flush();
            Swr.Close();
        }
    }
}
