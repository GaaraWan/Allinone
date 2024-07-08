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
                retstr += _highestValue.ToString();

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

            public override string ToString()
            {
                string retstr = "";

                retstr += (pannel ? "1" : "0");
               
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
                    }
                    i++;
                }
            }
            
            
            [CategoryAttribute("Light Settings"),
                    DefaultValueAttribute(true)]
            [DisplayName("平板燈")]
            public bool PANNEL
            {
                get { return pannel; }
                set { pannel = value; }
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

                while(i < position.Length)
                {
                    retstr += position[i] + ";";
                    i++;
                }
                retstr = retstr.Remove(retstr.Length - 1, 1);

                return retstr;
            }

            public void SetPosition(int index,string str)
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
            [DisplayName("01.拍照开始位置"),Description("即开始拍照位置")]
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

            cboEnv.SelectedIndexChanged += cboEnv_SelectedIndexChanged;
        }

        public void Initial(OptionEnum option,AlbumClass album)
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

            switch(OPTION)
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

            switch(OPTION)
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

                    btnSetEnd.Visible = false;
                    btnGoPosition.Visible = false;
                    btnSetPosition.Visible = false;

                    ppgPosition.Visible = false;
                    lblPostion.Visible = false;

                    break;
            }


            FillDisplay();
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
            if (MessageBox.Show("是否要刪除此環境?", "SYSTEM", MessageBoxButtons.YesNo) == DialogResult.Yes)
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

            OnTrigger(RCPStatusEnum.SHOWDETAIL,"");
        }
        void Compound()
        {
            OnTrigger(RCPStatusEnum.SHOWCOMPOUND,"");
        }
        void SetPosition()
        {
            OnTrigger(RCPStatusEnum.SETPOSITION,"");
        }
        void SetEnd()
        {
            OnTrigger(RCPStatusEnum.SETEND,"");
        }
        void GoPosition()
        {
            OnTrigger(RCPStatusEnum.GOPOSITION,GetPosition());
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
            switch(OPTION)
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
            switch(OPTION)
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
                
                switch(OPTION)
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

                switch(OPTION)
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
                TriggerAction(status,opstr);
            }
        }
    }
}
