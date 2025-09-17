using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using JetEazy;
using JetEazy.UISpace;
using JetEazy.BasicSpace;
using JzDisplay;
using Allinone.OPSpace;
using JzASN.OPSpace;
using JzOCR.OPSpace;
using Allinone.BasicSpace;
using MoveGraphLibrary;
using WorldOfMoveableObjects;
using System.Reflection;

namespace Allinone.UISpace
{
    public partial class AinfoUI : UserControl
    {
        enum TagEnum
        {
            ADDSHAPE,
            DELSHAPE,
            REVISESHAPE,

            RESETANALYZE,
        }

        VersionEnum VERSION;
        OptionEnum OPTION;

        ASNCollectionClass ASNCollection
        {
            get
            {
                return Universal.ASNCollection;
            }
        }
        OCRCollectionClass OCRCollection
        {
            get
            {
                return Universal.OCRCollection;
            }
        }


        AnalyzeClass AnalyzeNow;

        Button btnAddShape;
        Button btnDelShape;
        Button btnReviseShape;
        Button btnResetAnalyze;

        ComboBox cboShape;

        ListBox lsbShape;
        Label lblShapeInformation;

        //AnalyzeSettings AnalyzeSetting = new AnalyzeSettings();
        //PropertyGrid ppgAnalyze;
        myPropertyGrid ppgAnalyze;

        bool IsNeedToChange = false;
        int PreviousShapeSelectIndex = 0;

        public bool IsLearn = false;            //是否在學習時操作

        public AinfoUI()
        {
            InitializeComponent();
            InitialInternal();
        }

        void InitialInternal()
        {
            btnAddShape = button7;
            btnDelShape = button8;
            btnReviseShape = button1;
            btnResetAnalyze = button2;

            cboShape = comboBox1;
            lsbShape = listBox1;
            lblShapeInformation = label4;

            ppgAnalyze = myPropertyGrid1;
            //ppgAnalyze = propertyGrid1;
            //ppgAnalyze.CanShowVisualStyleGlyphs = false;
            ppgAnalyze.PropertySort = PropertySort.CategorizedAlphabetical;
            ppgAnalyze.PropertyValueChanged += PpgAnalyze_PropertyValueChanged;

            btnAddShape.Tag = TagEnum.ADDSHAPE;
            btnDelShape.Tag = TagEnum.DELSHAPE;
            btnReviseShape.Tag = TagEnum.REVISESHAPE;
            btnResetAnalyze.Tag = TagEnum.RESETANALYZE;

            btnAddShape.Click += btn_Click;
            btnDelShape.Click += btn_Click;
            btnReviseShape.Click += btn_Click;
            btnResetAnalyze.Click += btn_Click;

            lsbShape.SelectedIndexChanged += lsbShape_SelectedIndexChanged;

        }
        void InitialShape()
        {
            int i = 0;

            while (i < (int)ShapeEnum.COUNT)
            {
                cboShape.Items.Add((ShapeEnum)i);
                i++;
            }

            cboShape.SelectedIndex = 0;
        }

        void FillASNParaData(bool ischangeASNItem)
        {
            int i = 0;
            int j = 0;
            bool isrelateasn = false;

            JzASNStringConverter.ParaData = new string[ASNCollection.myDataList.Count + 1];
            JzASNStringConverter.ParaData[0] = "None";

            JzASNItemStringConverter.ParaData = new string[1];
            JzASNItemStringConverter.ParaData[0] = "None";

            i = 1;
            foreach (ASNClass asn in ASNCollection.myDataList)
            {
                JzASNStringConverter.ParaData[i] = asn.ToASNString();

                if (AnalyzeNow.RelateASN == asn.ToASNString())
                {
                    JzASNItemStringConverter.ParaData = new string[asn.ASNItemList.Count + 1];
                    JzASNItemStringConverter.ParaData[0] = "None";

                    j = 1;
                    foreach (ASNItemClass asnitem in asn.ASNItemList)
                    {
                        JzASNItemStringConverter.ParaData[j] = asnitem.ToASNItemRelateString();

                        if(AnalyzeNow.RelateASNItem == asnitem.ToASNItemRelateString())
                        {
                            isrelateasn = true;
                        }
                        j++;
                    }
                }
                i++;
            }

            if(!isrelateasn || AnalyzeNow.RelateASN == "None")
            {
                if (ischangeASNItem)
                {
                    AnalyzeNow.RelateASNItem = "None";
                    OnChange("01.Normal;RelateASNItem", "None");
                }
            }
        }


        void FillOCRParaData(bool ischangeOCRdata)
        {
            int i = 0;

            JzOCRStringConverter.ParaData = new string[1];
            JzOCRStringConverter.ParaData[0] = "None";

            if (AnalyzeNow.OCRPara.OCRMethod == OCRMethodEnum.MAPPING)
            {
                JzOCRStringConverter.ParaData = new string[OCRCollection.myDataList.Count];

                i = 0;
                foreach (OCRClass ocr in OCRCollection.myDataList)
                {
                    string ocrparastr = ocr.Name + "(" + ocr.No + ")";

                    JzOCRStringConverter.ParaData[i] = ocrparastr;

                    i++;
                }
            }
            else
            {
                switch(Universal.OPTION)
                {
                    case OptionEnum.MAIN_X6:

                        string[] datas = Allinone.Universal.MapBuilder.GetCellContent(Allinone.Universal.MapCellIndex);

                        JzOCRStringConverter.ParaData = new string[datas.Length];

                        i = 0;
                        foreach (string s in datas)
                        {
                            //string ocrparastr = ocr.Name + "(" + ocr.No + ")";

                            JzOCRStringConverter.ParaData[i] = $"{s}#{i}";

                            i++;
                        }

                        break;
                }
            }

            if (AnalyzeNow.OCRPara.OCRMethod  != OCRMethodEnum.MAPPING)
            {
                switch (Universal.OPTION)
                {
                    case OptionEnum.MAIN_X6:

                        if (ischangeOCRdata)
                        {
                            AnalyzeNow.OCRPara.OCRMappingMethod = JzOCRStringConverter.ParaData[0];
                            OnChange("05.OCR or Barcode;OCRMappingMethod", JzOCRStringConverter.ParaData[0]);
                        }

                        break;
                    default:

                        if (ischangeOCRdata)
                        {
                            // AnalyzeNow.OCRPara.OCRMappingMethod = OCRSETEnum.NONE;//"None";
                            AnalyzeNow.OCRPara.OCRMappingMethod = "None";
                            OnChange("05.OCR or Barcode;OCRMappingMethod", "None");
                        }

                        break;
                }

                
            }
            else
            {
                if (ischangeOCRdata)
                {
                   // AnalyzeNow.OCRPara.OCRMappingMethod = (OCRSETEnum)Enum.Parse(typeof(OCRSETEnum), JzOCRStringConverter.ParaData[0], true); //JzOCRStringConverter.ParaData[0];
                    AnalyzeNow.OCRPara.OCRMappingMethod = JzOCRStringConverter.ParaData[0];
                    OnChange("05.OCR or Barcode;OCRMappingMethod", JzOCRStringConverter.ParaData[0]);
                }
            }
        }
        //void FillOCRParaData(bool ischangeOCRdata)
        //{
        //    int i = 0;

        //    JzOCRStringConverter.ParaData = new string[1];
        //    JzOCRStringConverter.ParaData[0] = "None";

        //    if (AnalyzeNow.OCRPara.OCRMethod == OCRMethodEnum.MAPPING)
        //    {
        //        JzOCRStringConverter.ParaData = new string[OCRCollection.myDataList.Count];

        //        i = 0;
        //        foreach (OCRClass ocr in OCRCollection.myDataList)
        //        {
        //            string ocrparastr = ocr.Name + "(" + ocr.No + ")";

        //            JzOCRStringConverter.ParaData[i] = ocrparastr;

        //            i++;
        //        }
        //    }

        //    if (AnalyzeNow.OCRPara.OCRMethod != OCRMethodEnum.MAPPING)
        //    {
        //        if (ischangeOCRdata)
        //        {
        //            // AnalyzeNow.OCRPara.OCRMappingMethod = OCRSETEnum.NONE;//"None";
        //            AnalyzeNow.OCRPara.OCRMappingMethod = "None";
        //            OnChange("05.OCR or Barcode;OCRMappingMethod", "None");
        //        }
        //    }
        //    else
        //    {
        //        if (ischangeOCRdata)
        //        {
        //            // AnalyzeNow.OCRPara.OCRMappingMethod = (OCRSETEnum)Enum.Parse(typeof(OCRSETEnum), JzOCRStringConverter.ParaData[0], true); //JzOCRStringConverter.ParaData[0];
        //            AnalyzeNow.OCRPara.OCRMappingMethod = JzOCRStringConverter.ParaData[0];
        //            OnChange("05.OCR or Barcode;OCRMappingMethod", JzOCRStringConverter.ParaData[0]);
        //        }
        //    }
        //}

        #region Event Operation
        private void PpgAnalyze_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (!IsNeedToChange)
                return;

            //OnChange(((System.Windows.Forms.PropertyGridInternal.GridEntry)e.ChangedItem), e.ChangedItem.Value.ToString());

            string changeitemstr = e.ChangedItem.Parent.Label + ";" + e.ChangedItem.PropertyDescriptor.Name;


            #region NO USE
            //var item = ppgAnalyze.SelectedObject;
            //SetPropertyReadOnly(item, "MMOPString", false);

            //bool b = (bool)e.ChangedItem.Value;
            //if (b)
            //{
            //    //SetPropertyVisibility(item, "WindowSize", b);//WindowSize必须是自定义属性中的属性名，如下也是
            //    //SetPropertyVisibility(item, "WindowFont", b);
            //    //SetPropertyReadOnly(item, "WindowFont", b);
            //    SetPropertyReadOnly(item, "MMOPString", b);
            //}
            //else
            //{
            //    //SetPropertyVisibility(item, "WindowSize", b);
            //    //SetPropertyVisibility(item, "WindowFont", b);
            //    //SetPropertyReadOnly(item, "WindowFont", b);
            //    SetPropertyReadOnly(item, "MMOPString", b);
            //}
            #endregion

            OnChange(changeitemstr, e.ChangedItem.Value.ToString());
            
            if((changeitemstr + "#").IndexOf("RelateASN#") > -1)
                FillASNParaData(true);

            if ((changeitemstr + "#").IndexOf("OCRMethod#") > -1)
                FillOCRParaData(true);

            //SetPropertyReadOnly(item, "MMOPString", true);
            //ppgAnalyze.SelectedObject = item;

            //AnalyzeNow.FromAssembleProperty();
            FillDisplay();
            
        }
        public void Initial(VersionEnum version, OptionEnum opt)
        {
            VERSION = version;
            OPTION = opt;

            InitialShape();

        }
        public void SetAnalyze(AnalyzeClass analyzenow)
        {   
            if (analyzenow == null)
            {
                AnalyzeNow = null;
                FillDisplay();
            }
            else
            {
                AnalyzeNow = analyzenow;
                FillDisplay();

                FillASNParaData(false);
                FillOCRParaData(false);
            }
        }
        private void lsbShape_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            int i = 0;
            List<int> selectnoindexlist = new List<int>();

            while(i < lsbShape.SelectedIndices.Count)
            {
                selectnoindexlist.Add(lsbShape.SelectedIndices[i]);
                i++;
            }
            OnBackward(selectnoindexlist);
        }
        private void cboShape_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsNeedToChange)
                return;

            IsNeedToChange = false;

            int selectindex = lsbShape.SelectedIndex;

            lsbShape.Items.RemoveAt(selectindex);
            lsbShape.Items.Insert(selectindex, cboShape.Text);
            
            lsbShape.SelectedIndex = selectindex;

            //DefineShapeNow.SetShape((ShapeEnum)cboShape.SelectedIndex);

            IsNeedToChange = true;
        }
        private void btn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            switch ((TagEnum)btn.Tag)
            {
                case TagEnum.ADDSHAPE:
                    AddShape();
                    break;
                case TagEnum.DELSHAPE:
                    DelShape();
                    break;
                case TagEnum.REVISESHAPE:
                    ReviseShape();
                    break;
                case TagEnum.RESETANALYZE:
                    ResetAnalyze();
                    break;
            }
        }
        #endregion

        void SetPropertyVisibility(object obj, string propertyName, bool visible)
        {
            Type type = typeof(BrowsableAttribute);
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(obj);
            AttributeCollection attrs = props[propertyName].Attributes;
            FieldInfo fld = type.GetField("browsable", BindingFlags.Instance | BindingFlags.NonPublic);
            fld.SetValue(attrs[type], visible);
        }

        void SetPropertyReadOnly(object obj, string propertyName, bool readOnly)
        {
            Type type = typeof(System.ComponentModel.ReadOnlyAttribute);
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(obj);
            AttributeCollection attrs = props[propertyName].Attributes;
            FieldInfo fld = type.GetField("isReadOnly", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance);
            fld.SetValue(attrs[type], readOnly);
        }

        #region Normal Operation
        void FillDisplay()
        {
            IsNeedToChange = false;

            lsbShape.Items.Clear();
            
            if (AnalyzeNow != null)
            {
                for (int i = 0; i < AnalyzeNow.myMover.Count; i++)
                {
                    GraphicalObject grobj = AnalyzeNow.myMover[i].Source;
                    lsbShape.Items.Add((grobj as GeoFigure).ToShapeEnum());

                    lsbShape.SetSelected(i, (grobj as GeoFigure).IsSelected);
                }

                //AnalyzeSetting.FromString(AnalyzeNow.ToAnalyzeSettingString());
                AnalyzeNow.ToAssembleProperty();

                ppgAnalyze.Enabled = true;
                lsbShape.Enabled = true;

                btnAddShape.Enabled = true;
                btnDelShape.Enabled = true;
                btnReviseShape.Enabled = true;
                btnResetAnalyze.Enabled = true;
                cboShape.SelectedIndex = 0;
                cboShape.Enabled = true;

                //ppgAnalyze.SelectedObject = AnalyzeNow.ASSEMBLE;
                ppgAnalyze.SelectedObject = AnalyzeNow.ToLanguage();

                //FillASNParaData();
            }
            else
            {
                //AnalyzeSetting.FromString("");
                //AnalyzeNow.ToAssembleProperty(true);

                ppgAnalyze.Enabled = false;
                lsbShape.Enabled = false;

                btnAddShape.Enabled = false;
                btnDelShape.Enabled = false;
                btnReviseShape.Enabled = false;
                btnResetAnalyze.Enabled = false;

                if(cboShape.SelectedIndex != -1)
                {
                    PreviousShapeSelectIndex = cboShape.SelectedIndex;
                }

                cboShape.SelectedIndex = -1;
                cboShape.Enabled = false;

                if (ppgAnalyze.SelectedObject != null)
                    ppgAnalyze.SelectedObject = null;
            }
            
            IsNeedToChange = true;

            //if (lsbShape.Items.Count > 0)
            //    lsbShape.SelectedIndex = 0;

        }
        void AddShape()
        {
            string opstring = "";
            OnShapen(ShapeOpEnum.ADDSHAPE, (ShapeEnum)cboShape.SelectedIndex,opstring);
        }
        void DelShape()
        {
            string opstring = "";
            OnShapen(ShapeOpEnum.DELSHAPE, (ShapeEnum)cboShape.SelectedIndex,opstring);
        }
        void ReviseShape()
        {
            string opstring = "";
            OnShapen(ShapeOpEnum.REVISESHAPE, (ShapeEnum)cboShape.SelectedIndex,opstring);
        }

        void ResetAnalyze()
        {
            if (MessageBox.Show(ToChangeLanguage("是否要將選定的框都回復成預設參數設定?"), "SYSTEM", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                OnChange("RESET", "");
                FillDisplay();
            }
        }
        public void ResetAnalyzeTemp()
        {
            OnChange("RESET", "");
            FillDisplay();
        }

        //void FillASNData()
        //{
        //    int i = 0;
        //    int j = 0;

        //    JzASNStringConverter.ParaData = new string[ASNCollection.myDataList.Count + 1];
        //    JzASNStringConverter.ParaData[0] = "None";

        //    i = 1;
        //    foreach (ASNClass asn in ASNCollection.myDataList)
        //    {
        //        JzASNStringConverter.ParaData[i] = asn.ToASNString();

        //        if (AnalyzeNow.RelateASN == asn.Name)
        //        {
        //            JzASNItemStringConverter.ParaData = new string[asn.ASNItemList.Count];
        //            JzASNItemStringConverter.ParaData[0] = "None";

        //            j = 1;
        //            foreach (ASNItemClass asnitem in asn.ASNItemList)
        //            {
        //                JzASNItemStringConverter.ParaData[j] = asnitem.ToAsnItemString();
        //                j++;
        //            }
        //        }
        //        i++;
        //    }


        //}

        string ToChangeLanguage(string eText)
        {
            string retStr = eText;
            retStr = LanguageExClass.Instance.GetLanguageText(eText);
            return retStr;
        }

        #endregion

        public delegate void BackwardHandler(List<int> selectnolist);
        public event BackwardHandler BackwardAction;
        public void OnBackward(List<int> selectnolist)
        {
            if (BackwardAction != null)
            {
                BackwardAction(selectnolist);
            }
        }

        public delegate void ShapeHandler(ShapeOpEnum shapeop,ShapeEnum shape,string opstring);
        public event ShapeHandler ShapenAction;
        public void OnShapen(ShapeOpEnum shapeop, ShapeEnum shape,string opstring)
        {
            if (ShapenAction != null)
            {
                ShapenAction(shapeop, shape, opstring);
            }
        }

        public delegate void ChangeHandler(string changeitemstring,string valuestring);
        public event ChangeHandler ChangeAction;
        public void OnChange(string changeitemstring, string valusestring)
        {
            if (ChangeAction != null)
            {
                ChangeAction(changeitemstring,valusestring);
            }
        }


    }
}
