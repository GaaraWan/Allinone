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
    [DefaultPropertyAttribute("Environment Position")]
    public class PositionSettings
    {
        private string[] position = new string[100];

        [CategoryAttribute("DFly Method"),
        DefaultValueAttribute(DFlyMethodEnum.DIRECTPOS), ReadOnly(false)]
        [TypeConverter(typeof(JzEnumConverter))]
        public DFlyMethodEnum DFlyMethod
        {
            get; set;
        }

        [CategoryAttribute("POS Settings"),
        DefaultValueAttribute("0"), ReadOnly(false)]
        public string[] POS
        {
            get { return position; }
            set { position = value; }
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

            retstr = DFlyMethod.ToString() + ";" + retstr;

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


        public void FromString(string str)
        {
            string[] positionstrs = str.Split(';');

            int i = 0;

            foreach (string strx in positionstrs)
            {
                if (i == 0)
                {
                    DFlyMethod = (DFlyMethodEnum)Enum.Parse(typeof(DFlyMethodEnum), positionstrs[0], true);
                }
                else
                {
                    if (i < GetPosCount())
                        SetPosition(i - 1, strx);
                }
                i++;
            }
        }

    }

    public partial class DFlyAlbUI : UserControl
    {
        enum TagEnum
        {
            ADD,
            DEL,
            DETAIL,
            RELATE,

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
            private bool light = true;

            public override string ToString()
            {
                string retstr = "";

                retstr = (light ? "1" : "0");

                return retstr;
            }

            [CategoryAttribute("Light Settings"),
            DefaultValueAttribute(true)]
            public bool LIGHT
            {
                get { return light; }
                set { light = value; }
            }
        }

        
        Button btnAdd;
        Button btnDel;
        ComboBox cboEnv;

        Button btnDetail;

        Label lblPostion;
        Button btnSetPosition;
        Button btnSetEnd;
        Button btnGoPosition;

        PropertyGrid ppgLight;
        PropertyGrid ppgPosition;

        OptionEnum OPTION;
        AlbumClass ALBUM;

        LightSettings LightSetting;
        PositionSettings PositionSetting;

        EnvClass ENVNow
        {
            get
            {
                return ALBUM.ENVList[cboEnv.SelectedIndex];
            }
        }

        bool IsNeedToChange = false;

        public DFlyAlbUI()
        {
            InitializeComponent();
            InitialInternal();
        }

        protected void InitialInternal()
        {
            btnAdd = button7;
            btnDel = button8;
            btnDetail = button10;

            lblPostion = label3;
            btnSetPosition = button1;
            btnSetEnd = button3;
            btnGoPosition = button4;

            cboEnv = comboBox1;

            ppgLight = propertyGrid1;
            ppgPosition = propertyGrid3;


            btnAdd.Tag = TagEnum.ADD;
            btnDel.Tag = TagEnum.DEL;
            btnDetail.Tag = TagEnum.DETAIL;
            btnSetPosition.Tag = TagEnum.SETPOSITION;
            btnSetEnd.Tag = TagEnum.SETEND;
            btnGoPosition.Tag = TagEnum.GOPOSITION;

            ppgLight.PropertySort = PropertySort.NoSort;
            ppgPosition.PropertySort = PropertySort.NoSort;

            ppgLight.PropertyValueChanged += PpgLight_PropertyValueChanged;
            ppgPosition.PropertyValueChanged += PpgPosition_PropertyValueChanged;

            LightSetting = new LightSettings();
            PositionSetting = new PositionSettings();

            btnAdd.Click += btn_Click;
            btnDel.Click += btn_Click;
            btnDetail.Click += btn_Click;

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

            cboEnv.Items.Clear();

            foreach (EnvClass env in album.ENVList)
            {
                cboEnv.Items.Add(env.ToEnvString());
            }

            IsNeedToChange = true;
            cboEnv.SelectedIndex = (album.ENVCount > 0 ? 0 : -1);

            FillDsiaply();
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

            FillDsiaply();

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

            if(ALBUM.ENVCount == 0)
                envno = 0;
            else
                envno = ALBUM.LastENV.No + 1;

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

                FillDsiaply();
            }
        }
        void Detail()
        {
            JzToolsClass.PassingInteger = ENVNow.No;

            OnTrigger(RCPStatusEnum.SHOWDETAIL,"");
        }
        void SetPosition()
        {
            OnTrigger(RCPStatusEnum.SETPOSITION,"");
        }
        public void SetPosition(string posstr)
        {
            int startindex = -1;

            switch(((PositionSettings) ppgPosition.SelectedObject).DFlyMethod)
            {
                case DFlyMethodEnum.DIRECTPOS:
                    startindex = 0;
                    break;
                case DFlyMethodEnum.RELATEPOS:
                    startindex = 3;
                    break;
                default:
                    startindex = 0;
                    break;
            }

            int index = -1;

            bool isint = int.TryParse(ppgPosition.SelectedGridItem.Label.Replace("[", "").Replace("]", ""), out index);

            //if(ppgPosition.SelectedGridItem.Label.Replace("[","").Replace("]"))
            //PositionSetting.POS[index] = posstr;

            if (isint && index >= startindex)
            {
                PositionSetting.POS[index] = posstr;
                WriteBackPosition();
                FillPosition();

            }
        }
        void SetEnd()
        {
            OnTrigger(RCPStatusEnum.SETEND, "");
        }
        void GoPosition()
        {
            string goposstr = GetPosition();

            if(goposstr != "")
                OnTrigger(RCPStatusEnum.GOPOSITION, goposstr);
        }
        string GetPosition()
        {
            int startindex = -1;
            string retstr = "";

            switch (((PositionSettings)ppgPosition.SelectedObject).DFlyMethod)
            {
                case DFlyMethodEnum.DIRECTPOS:
                    startindex = 0;
                    break;
                case DFlyMethodEnum.RELATEPOS:
                    startindex = 3;
                    break;
                default:
                    startindex = 0;
                    break;
            }

            int index = -1;

            bool isint = int.TryParse(ppgPosition.SelectedGridItem.Label.Replace("[", "").Replace("]", ""), out index);

            if (isint && index >= startindex)
            {
                retstr = PositionSetting.POS[index];
            }

            if(retstr.Length - retstr.Replace(",","").Length != 2)
            {
                retstr = "";
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
            ENVNow.GeneralLight = LightSetting.ToString();
        }
        void WriteBackPosition()
        {
            ENVNow.GeneralPosition = PositionSetting.ToString();
        }
        void FillDsiaply()
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

            if (ALBUM.ENVCount == 0)
            {
                ppgLight.SelectedObject = null;
            }
            else
            {
                int i = 0;

                string LightString = ENVNow.GeneralLight;

                string[] lightstrs = LightString.Split(',');

                i = 0;
                foreach (string str in lightstrs)
                {
                    switch (i)
                    {
                        case 0:
                            LightSetting.LIGHT = str == "1";
                            break;
                    }
                    i++;
                }
                ppgLight.SelectedObject = LightSetting;
            }

            IsNeedToChange = true;
        }
        void FillPosition()
        {
            IsNeedToChange = false;

            if (ALBUM.ENVCount == 0)
            {
                ppgPosition.SelectedObject = null;

            }
            else
            {
                int i = 0;
                string PositionString = ENVNow.GeneralPosition;

                PositionSetting.FromString(PositionString);

                ppgPosition.SelectedObject = PositionSetting;

                ppgPosition.ExpandAllGridItems();
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
    }
}
