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
using JetEazy;
using JetEazy.BasicSpace;

namespace Allinone.UISpace.ALBUISpace
{
    public partial class AudixAlbUI : UserControl
    {
        enum TagEnum
        {
            ADD,
            DEL,
            DETAIL,
            RELATE,

            SETPOSITION,

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

        [DefaultPropertyAttribute("Environment Position")]
        public class PositionSettings
        {
            private string position = "0";

            private string [] setuppos = new string[100];

            [CategoryAttribute("POS Settings"),
            DefaultValueAttribute("0")]
            public string POS
            {
                get { return position; }
                set { position = value; }
            }

            public override string ToString()
            {
                string retstr = "";

                retstr = POS + "";

                return retstr;
            }

        }

        Button btnAdd;
        Button btnDel;
        ComboBox cboEnv;

        Button btnDetail;
        Button btnSetPosition;
        Button btnRelate;

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

        public AudixAlbUI()
        {
            InitializeComponent();
            InitialInternal();
        }

        protected void InitialInternal()
        {
            btnAdd = button7;
            btnDel = button8;
            btnDetail = button10;
            btnRelate = button2;
            btnSetPosition = button1;

            cboEnv = comboBox1;

            ppgLight = propertyGrid1;
            ppgPosition = propertyGrid3;


            btnAdd.Tag = TagEnum.ADD;
            btnDel.Tag = TagEnum.DEL;
            btnDetail.Tag = TagEnum.DETAIL;
            btnRelate.Tag = TagEnum.RELATE;
            btnSetPosition.Tag = TagEnum.SETPOSITION;

            ppgLight.PropertySort = PropertySort.NoSort;
            ppgPosition.PropertySort = PropertySort.NoSort;

            ppgLight.PropertyValueChanged += PpgLight_PropertyValueChanged;
            ppgPosition.PropertyValueChanged += PpgPosition_PropertyValueChanged;

            LightSetting = new LightSettings();
            PositionSetting = new PositionSettings();

            btnAdd.Click += btn_Click;
            btnDel.Click += btn_Click;
            btnDetail.Click += btn_Click;
            btnRelate.Click += btn_Click;
            btnSetPosition.Click += btn_Click;

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
            //cboEnv.SelectedIndex = 0;
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
                case TagEnum.RELATE:
                    Assign();
                    break;
                case TagEnum.SETPOSITION:
                    SetPosition();
                    break;
            }
        }
        void Add()
        {
            ALBUM.AddEnv(ENVNow);

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
            }
        }
        void Detail()
        {
            JzToolsClass.PassingInteger = ENVNow.No;

            OnTrigger(RCPStatusEnum.SHOWDETAIL);
        }
        void Assign()
        {
            OnTrigger(RCPStatusEnum.SHOWASSIGN);
        }
        void SetPosition()
        {
            OnTrigger(RCPStatusEnum.SETPOSITION);
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

            btnDel.Enabled = ENVNow.No != 0;

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

                string[] positionstrs = PositionString.Split(';');

                i = 0;
                foreach (string str in positionstrs)
                {
                    switch (i)
                    {
                        case 0:
                            PositionSetting.POS = str;
                            break;
                    }
                    i++;
                }
                ppgPosition.SelectedObject = PositionSetting;
            }

            IsNeedToChange = true;
        }

        public delegate void TriggerHandler(RCPStatusEnum status);
        public event TriggerHandler TriggerAction;
        public void OnTrigger(RCPStatusEnum status)
        {
            if (TriggerAction != null)
            {
                TriggerAction(status);
            }
        }
    }
}
