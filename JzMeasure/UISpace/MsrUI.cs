#define OPT_GAARA

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using JetEazy.BasicSpace;
using JzDisplay;
using JzDisplay.UISpace;
using JzMSR.OPSpace;
using JzMSR.FormSpace;

using MoveGraphLibrary;
using WorldOfMoveableObjects;

namespace JzMSR.UISpace
{
    public partial class MsrUI : UserControl
    {
        enum TagEnum
        {
            AUTOFIND,
            ASSIGNPOSITION,
        }

        DispUI SHOWDISP;
        Mover ShowMover = new Mover();

        ListBox lstItem;
        
        DispUI ITEMDISP;
        Mover ItemMover = new Mover();

        PropertyGrid ppgMethod;

        MSRClass MSR;
        JzToolsClass JzTools = new JzToolsClass();

        Label lblItemVWMsg;

        Button btnAutoFind;
        Button btnAssignPosition;

        List<int> SelectIndexList = new List<int>();

        int MSRItemIndex
        {
            get
            {
                return lstItem.SelectedIndex;
            }
        }

        bool IsNeedToChange;
        
        public MsrUI()
        {
            InitializeComponent();
            InitialInside();
        }

        void InitialInside()
        {
            btnAutoFind = button1;
            btnAssignPosition = button2;

            btnAutoFind.Tag = TagEnum.AUTOFIND;
            btnAssignPosition.Tag = TagEnum.ASSIGNPOSITION;

            btnAutoFind.Click += btn_Click;
            btnAssignPosition.Click += btn_Click;

            lstItem = listBox1;

            lblItemVWMsg = label2;

            SHOWDISP = dispUI1;
            ITEMDISP = dispUI2;

            ppgMethod = myPropertyGrid1;
          
            SHOWDISP.Initial();
            SHOWDISP.SetDisplayType(DisplayTypeEnum.NORMAL);
            SHOWDISP.MoverAction += SHOWDISP_MoverAction;
            SHOWDISP.AdjustAction += SHOWDISP_AdjustAction;

            ITEMDISP.Initial();
            ITEMDISP.SetDisplayType(DisplayTypeEnum.NORMAL);

            lstItem.SelectedIndexChanged += lstMethod_SelectedIndexChanged;
        }

        private void SHOWDISP_AdjustAction(PointF ptfoffset)
        {
            //throw new NotImplementedException();

            int i = 0;

            //SHOWDISP.GenSearchImage()


        }

        private void SHOWDISP_MoverAction(MoverOpEnum moverop, string opstring)
        {
            int i = 0;
            string[] strs = opstring.Split(',');

            switch (moverop)
            {
                case MoverOpEnum.SELECT:
                    DISPSelectActionDX(strs);
                    break;
                
            }

            IsNeedToChange = true;
        }
        /// <summary>
        /// With Sub Selection
        /// </summary>
        /// <param name="strs"></param>
        void DISPSelectActionDX(string[] strs)
        {
            int i = 0;
            List<string> selectanalyzestringlist = new List<string>();
            int iselectcount = 0;

            if (strs[0] == "")
            {
                lstItem.SelectedIndex = -1;

            }
            else
            {
                IsNeedToChange = false;

                int FirstNo = -1;

                lstItem.SelectedIndex = -1;

                foreach (string str in strs)
                {
                    string[] strxs = str.Split(':');
                    int no = int.Parse(strxs[0]);

                    //if (FirstNo == -1)
                    //    FirstNo = no;

                    i = 0;
                    while(i < lstItem.Items.Count)
                    {
                        if(int.Parse(lstItem.Items[i].ToString().Substring(2)) == no)
                        {
                            lstItem.SetSelected(i, true);
                            iselectcount++;
                            if (FirstNo == -1)
                                FirstNo = i;
                            break;
                        }
                        i++;
                    }
                }

                label3.Text = iselectcount.ToString();

                IsNeedToChange = true;

                FillDisplay(FirstNo);
            }
        }

        private void BtnAutoFind_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void lstMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            int i = 0;

            if (lstItem.SelectedIndices.Count > 0)
            {
                i = 0;

                List<int> lsbselectlist = new List<int>();

                while(i < lstItem.SelectedIndices.Count)
                {
                    lsbselectlist.Add(lstItem.SelectedIndices[i]);

                    i++;
                }

                SHOWDISP.MappingLsbSelect(lsbselectlist);

            }

            SHOWDISP.ReDraw();

            FillDisplay(lstItem.SelectedIndex);
        }

        public void Initial(MSRClass msr)
        {
            MSR = msr;

            IsNeedToChange = false;

            InitialMover();
            SHOWDISP.ReplaceDisplayImage(MSR.bmpCalibrate);
            InitialListItem();

        }
        private void btn_Click(object sender, EventArgs e)
        {
            TagEnum KEY = (TagEnum)((Button)sender).Tag;

            switch (KEY)
            {
                case TagEnum.AUTOFIND:
                    AutoFind();
                    break;
                case TagEnum.ASSIGNPOSITION:
                    AssignPosition();
                    break;
            }
        }

        void InitialMover()
        {
            ShowMover.Clear();
            SHOWDISP.ClearMover();//ADD Gaara 因為不清除，導致上次選擇的還存在而導致超出索引範圍

            foreach (MSRItemClass msritem in MSR.MSRItemList)
            {
                ShowMover.Add(msritem.RectEAG);
            }
            SHOWDISP.SetMover(ShowMover);

        }

        void InitialListItem()
        {
            IsNeedToChange = false;

            lstItem.Items.Clear();

            foreach (MSRItemClass msritem in MSR.MSRItemList)
            {
                lstItem.Items.Add(msritem.ToMethodIndexString());
            }

            IsNeedToChange = true;
            
            FillDisplay(-1);
        }

        AUTOFINDForm AUTOFINDFRM;
        void AutoFind()
        {
            if(MessageBox.Show("自動尋找會清除之前的資料，是否確認?","SYS",MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            AUTOFINDFRM = new AUTOFINDForm();

            if(AUTOFINDFRM.ShowDialog() == DialogResult.OK)
            {
                string[] strs = JzToolsClass.PassingString.Split(',');

                MSR.AutoFind(strs[0] == "1", int.Parse(strs[1]),int.Parse(strs[2]));

                InitialMover();
                SHOWDISP.RefreshDisplayShape();
                SHOWDISP.ReDraw();

                InitialListItem();
            }

            AUTOFINDFRM.Close();
            AUTOFINDFRM.Dispose();
        }

        ASSIGNForm ASSIGNFRM;
        void AssignPosition()
        {
            ASSIGNFRM = new ASSIGNForm(MSR);

            if (ASSIGNFRM.ShowDialog() == DialogResult.OK)
            {
                MSR.AssignPosition(JzToolsClass.PassingString);
            }
        }
        
        public void AddItem()
        {
            MSRItemClass msritem = new MSRItemClass();

            int i = 0;

            if (MSR.MSRItemList.Count > 0)
            {
                msritem.No = MSR.MSRItemList[MSR.MSRItemList.Count - 1].No + 1;
                //RectangleF _recttmp = MSR.MSRItemList[MSR.MSRItemList.Count - 1].RectRange;

                //GraphicalObject grobj;
                //grobj = MSR.MSRItemList[MSR.MSRItemList.Count - 1].RectEAG;
                //RectangleF FromRectF = (grobj as GeoFigure).RealRectangleAround(0, 0);
                //(grobj as GeoFigure).IsSelected = false;
                //(grobj as GeoFigure).IsFirstSelected = false;
                //FromRectF.Offset(100, 100);

                //msritem.RectEAG = new JzRectEAG(Color.FromArgb(60, Color.Red), FromRectF);
            }
            else
                msritem.No = 0;

            SHOWDISP.GenSearchImage(ref msritem.bmpItem);

            MSR.MSRItemList.Add(msritem);

            IsNeedToChange = false;

            lstItem.Items.Add(msritem.ToMethodIndexString());

            IsNeedToChange = true;

            lstItem.SelectedIndex = MSR.MSRItemList.Count - 1;

            SetEnable(true);

            InitialMover();
        }
        public void DelItem()
        {
#if (OPT_VICTOR)
            if (lstItem.SelectedIndex > -1)
            {
                int i = lstItem.SelectedIndex;

                MSRItemClass msritem = MSR.MSRItemList[i];
                msritem.Suicide();

                MSR.MSRItemList.RemoveAt(i);

                IsNeedToChange = false;
                lstItem.Items.RemoveAt(i);
                IsNeedToChange = true;

                if (i == lstItem.Items.Count)
                    i--;

                lstItem.SelectedIndex = i;

                InitialMover();
            }
#endif

#if (OPT_GAARA)
            int i = 0;
            List<MSRItemClass> OPERAMSRItemList = MSR.MSRItemList;

            if (OPERAMSRItemList.Count > 0)
            {
                i = OPERAMSRItemList.Count - 1;

                while (i > -1)
                {
                    MSRItemClass _msritem = OPERAMSRItemList[i];

                    if (_msritem.RectEAG.IsSelected)
                    {
                        OPERAMSRItemList[i].Suicide();
                        OPERAMSRItemList.RemoveAt(i);
                    }

                    i--;
                }
            }

            InitialMover();
            InitialListItem();
#endif
        }
        public void SetEnable(bool isenable)
        {
           
            ppgMethod.Enabled = (isenable && MSR.MSRItemList.Count > 0);

            if (isenable)
            {
                SHOWDISP.Enabled = true;

                if(MSR.MSRItemList.Count > 0)
                {
                    ITEMDISP.Enabled = true;
                }

                btnAutoFind.Enabled = true;
                btnAssignPosition.Enabled = true;
                lstItem.Enabled = true;
            }
            else
            {
                SHOWDISP.Enabled = false;
                ITEMDISP.Enabled = false;
                lstItem.Enabled = false;

                btnAutoFind.Enabled = false;
                btnAssignPosition.Enabled = false;
            }
        }
        public void SetDefaultView()
        {
            SHOWDISP.SetDisplayImage();
            ITEMDISP.SetDisplayImage();
        }
        void FillDisplay(int index)
        {
            IsNeedToChange = false;

            bool IsEnable = false;

            if (index == -1)
            {
                ITEMDISP.ClearAll();
                ppgMethod.SelectedObject = null;
                lblItemVWMsg.Text = "";

                //lstItem.Items.Clear();
            }
            else
            {
                Bitmap bmp1 = new Bitmap(1, 1);
                MSRItemClass msritem = MSR.MSRItemList[index];
                (msritem.RectEAG as GeoFigure).GenSearchImage(0, 0, MSR.bmpCalibrate, ref msritem.bmpItem, ref bmp1);
                //msritem.FindCenter(msritem.bmpItem);
                //msritem.RelatePointF = MSR.ViewToWorld(msritem.CenterPointF);

                lblItemVWMsg.Text = "View POS: " + JzTools.PointF000ToString(msritem.CenterPointF) + Environment.NewLine +
                                    "World POS: " + JzTools.PointF000ToString(msritem.RelatePointF);

                //bmp0.Save("D:\\0.BMP");
                //bmp1.Save("D:\\1.BMP");

                ITEMDISP.SetDisplayImage(msritem.bmpItem);

                msritem.ConstructProperty();
                ppgMethod.SelectedObject = msritem;

            }
            IsNeedToChange = true;
        }

        /// <summary>
        /// 按住Control多選
        /// </summary>
        public void HoldSelect()
        {
            SHOWDISP.HoldSelect();
        }
        /// <summary>
        /// 放開Control不選了
        /// </summary>
        public void ReleaseSelect()
        {
            SHOWDISP.ReleaseSelect();
        }
        /// <summary>
        /// 移動所有的Mover
        /// </summary>
        /// <param name="KEY"></param>
        public void MoveMover(Keys KEY)
        {
            int Step = 1;

            switch (KEY)
            {
                case Keys.Left:
                    SHOWDISP.MoveMover(-Step, 0);
                    break;
                case Keys.Right:
                    SHOWDISP.MoveMover(Step, 0);
                    break;
                case Keys.Up:
                    SHOWDISP.MoveMover(0, -Step);
                    break;
                case Keys.Down:
                    SHOWDISP.MoveMover(0, Step);
                    break;
            }
        }

        public delegate void TriggerHandler(string opstatus);
        public event TriggerHandler TriggerAction;
        public void OnTrigger(string opstatus)
        {
            if (TriggerAction != null)
            {
                TriggerAction(opstatus);
            }
        }

    }
}
